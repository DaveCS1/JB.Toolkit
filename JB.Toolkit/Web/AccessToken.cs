using JBToolkit.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JBToolkit.Web
{
    /// <summary>
    /// Generates and validates short lives access tokens - Stores in a database
    /// </summary>
    public class AccessToken
    {
        private static string TableName { get; } = "[dbo].[USR_AG_Shared_AccessTokens_T]";

        private static bool TableExistanceChecked { get; set; } = false;

        /// <summary>
        /// Generates a short live (default 30 seconds) access token (GUID) to be used when authenticating external / public requests.
        /// <br /><br />
        /// Note: Please see the method in JBToolkit: GetShortLivedAccessToken_Request_Example for an example of how the 3rd party would generate the request.
        /// </summary>
        /// <param name="request">The http request from the visitor which must contain the IP address in the json object data root and a predefined API as the value of a the request header with the key matching the application name</param>
        /// <param name="applicationName">A given application name</param>
        /// <param name="applicationApiKeyToCheckAgainst">A predefined application key</param>
        /// <param name="expiryInSeconds">How long in seconds the token lives (default 30 seconds)</param>
        /// <param name="whiteListIps">RECOMMENDED. An array of allowed IP address - typically you'd store in config file</param>
        /// <returns>A GUID string as the expiring token</returns>
        public static HttpResponseMessage GetShortLivedAccessToken(
            HttpRequestMessage request,
            string applicationName,
            string applicationApiKeyToCheckAgainst,
            string dbNameWhereTheTokenIsStored,
            int expiryInSeconds = 30,
            string[] whiteListIps = null)
        {
            return GetShortLivedAccessToken(
                request,
                applicationName,
                applicationApiKeyToCheckAgainst,
                dbNameWhereTheTokenIsStored,
                expiryInSeconds,
                whiteListIps);
        }

        /// <summary>
        /// Generates a short live (default 30 seconds) access token (GUID) to be used when authenticating external / public requests.
        /// <br /><br />
        /// Note: Please see the method in JBToolkit: GetShortLivedAccessToken_Request_Example for an example of how the 3rd party would generate the request.
        /// </summary>
        /// <param name="request">The http request from the visitor which must contain the IP address in the json object data root and a predefined API as the value of a the request header with the key matching the application name</param>
        /// <param name="applicationName">A given application name</param>
        /// <param name="applicationApiKeyToCheckAgainst">A predefined application key</param>
        /// <param name="expiryInSeconds">How long in seconds the token lives (default 30 seconds)</param>
        /// <param name="whiteListIps">RECOMMENDED. An array of allowed IP address - typically you'd store in config file</param>
        /// <returns>A GUID string as the expiring token</returns>
        public static HttpResponseMessage GetShortLivedAccessToken(
            HttpRequestMessage request,
            string applicationName,
            string applicationApiKeyToCheckAgainst,
            string dbNameWhereTheTokenIsStored,
            string dbConnectionString,
            int expiryInSeconds = 30,
            string[] whiteListIps = null)
        {
            HttpResponseMessage response;
            try
            {
                string body = request.Content.ReadAsStringAsync().Result;
                dynamic json = JsonConvert.DeserializeObject(body);

                string endUserIP = json.ip; // i.e. Accent customer on MyAccount
                string serviceCallperIP = IPHelper.GetVisitorIPAddress(); // Prodo server external IP

                if (string.IsNullOrWhiteSpace(endUserIP))
                {
                    endUserIP = json.IP;
                }

                if (string.IsNullOrWhiteSpace(endUserIP))
                {
                    endUserIP = json.Ip;
                }

                string apiKey = string.Empty;

                HttpHeaders headers = request.Headers;

                if (headers.TryGetValues(applicationName, out IEnumerable<string> values))
                {
                    apiKey = values.First();
                }
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("No API key in present header")
                    };
                }
                else if (string.IsNullOrWhiteSpace(endUserIP))
                {
                    response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("No IP address present")
                    };
                }
                else if (!Regex.IsMatch(endUserIP, RegularExpressions.Common.Pattern_IPv4Address))
                {
                    response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Invalid IP address format")
                    };
                }
                else if (apiKey != applicationApiKeyToCheckAgainst)
                {
                    response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                    {
                        Content = new StringContent("Unauthorised")
                    };
                }
                else if (whiteListIps != null && !whiteListIps.Contains(serviceCallperIP))
                {
                    response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                    {
                        Content = new StringContent("Unauthorised")
                    };
                }
                else
                {
                    string token = GenerateAccessToken(endUserIP, dbNameWhereTheTokenIsStored, dbConnectionString, applicationName, expiryInSeconds);
                    response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(token)
                    };
                }
            }
            catch (Exception e)
            {
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(e.Message)
                };
            }

            return response;
        }

        /// <summary>
        /// Get access token example
        /// </summary>
        private static async Task<string> GetShortLivedAccessToken_Request_Example(string ipAddress = "127.0.0.1")
        {
            var uri = "http://localhost:50473/api/GetToken";
            var apiKey = "VoAIYU5LXw5gvpo0987YBHgsww7dnhf8d3d598plzXhVj0uzzostMem5Z";
            var applicationName = "X-Accent-EDRM";
            var jsonBody = JsonConvert.SerializeObject(new { ip = ipAddress });

            var request = (HttpWebRequest)WebRequest.Create(uri);
            var data = Encoding.ASCII.GetBytes(jsonBody);

            var httpClient = new HttpClient();
            var httpContent = new ByteArrayContent(data);
            httpContent.Headers.Add(applicationName, apiKey);

            var asyncResponse = await httpClient.PostAsync(uri, httpContent);
            asyncResponse.EnsureSuccessStatusCode();
            var token = await asyncResponse.Content.ReadAsStringAsync();

            return await Task.Run(() => token);
        }

        /// <summary>
        ///  Generates a short lived access token (GUID - Expirty default 30 seconds)
        /// </summary>
        internal static string GenerateAccessToken(
            string ipAddress,
            string dbName,
            string connectionString,
            string applicationName,
            int expiryInSeconds = 30)
        {
            CreateIfNoTableExists(dbName, connectionString);

            DbGeneric dbCon = new DbGeneric(dbName, connectionString, applicationName);
            string guid = Guid.NewGuid().ToString();
            dbCon.ExecuteNonQuery(string.Format(
                @"IF (SELECT COUNT(*) FROM {4} (NOLOCK) WHERE IPAddress = '{0}') > 0
	                UPDATE {4} SET Token = '{1}', ExpiryDT = DATEADD(second, {2}, GETDATE()), Application = '{3}' WHERE IPAddress = '{0}'
                ELSE
	                INSERT INTO {4} (IPAddress, Token, ExpiryDT, Application) VALUES ('{0}', '{1}', DATEADD(second, {2}, GETDATE()), '{3}')",
                ipAddress, guid, expiryInSeconds, applicationName, TableName), out _);

            return guid;
        }

        /// <summary>
        ///  Checks if token exists in the database and isn't expired
        /// </summary>
        public static bool IsAccessTokenValid(string ipAddress, string dbName, string connectionString, string token)
        {
            DbGeneric dbCon = new DbGeneric(dbName, connectionString, "");
            return dbCon.GetScalerResult(string.Format(@"SELECT ISNULL((SELECT COUNT(*) FROM {2} (NOLOCK) WHERE IPAddress = '{0}' AND Token = '{1}' AND ExpiryDT > GETDATE()), 0)",
                ipAddress, token, TableName)).ToBool();
        }

        /// <summary>
        /// Get's a token expiry date and time for a given IP address if one exists
        /// </summary>
        public static DateTime? GetTokenExpiry(string ipAddress, string dbName, string connectionString)
        {
            DbGeneric dbCon = new DbGeneric(dbName, connectionString, "");
            string expiry = dbCon.GetScalerResult(string.Format(@"SELECT TOP 1 ExpiryDT FROM {1} (NOLOCK) WHERE IPAddress = '{0}'",
                ipAddress, TableName));

            if (string.IsNullOrEmpty(expiry))
            {
                return null;
            }
            else
            {
                return (DateTime?)Convert.ToDateTime(expiry);
            }
        }

        private static void CreateIfNoTableExists(string dbName, string connectionString)
        {
            if (!TableExistanceChecked)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        string command = string.Format(@"
                            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}].{1}') AND type in (N'U'))
                            BEGIN
                            CREATE TABLE [{0}].{1} (
	                            [IPAddress] [varchar](50) NOT NULL,
	                            [Application] [varchar](200) NOT NULL,
	                            [Token] [varchar](40) NOT NULL,
	                            [ExpiryDT] [datetime] NOT NULL,
                            UNIQUE NONCLUSTERED 
                            (
	                            [IPAddress] ASC
                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                            ) ON [PRIMARY]
                            END            
                            SET ANSI_PADDING ON                          
                            IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[{0}].{1}') AND name = N'NonClusteredIndex-20200115-181932')
                            CREATE NONCLUSTERED INDEX [NonClusteredIndex-20200115-181932] ON [{0}].{1}
                            (
	                            [IPAddress] ASC,
	                            [Application] ASC,
	                            [Token] ASC
                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]",
                            dbName, TableName);

                        conn.Open();

                        using (var sqlUpdateCommand = new SqlCommand(@"EXECUTE sp_executesql @cmd", conn))
                        {
                            sqlUpdateCommand.Parameters.AddWithValue("@cmd", command);
                            sqlUpdateCommand.ExecuteNonQuery();
                            TableExistanceChecked = true;
                        }

                        conn.Close();
                    }
                }
                catch { }
            }
        }
    }
}
