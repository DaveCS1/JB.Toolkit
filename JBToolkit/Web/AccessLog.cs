using JBToolkit.Database;
using System;
using System.Data.SqlClient;
using System.Net;
using static JBToolkit.Global.DatabaseConfiguration;

namespace JBToolkit.Web
{
    /// <summary>
    /// Used to store remote access logs into the databse
    /// </summary>
    public class AccessLog
    {
        private static string TableName { get; } = "[dbo].[USR_AG_Shared_Remote_AccessLog_T]";

        private static bool TableExistanceChecked { get; set; } = false;

        /// <summary>
        /// Insert an access log entry into the database
        /// </summary>
        /// <param name="ipAddress">Visitor or 3rd party IP</param>
        /// <param name="applicationName">Name of applications requested</param>
        /// <param name="endPoint">API End point or method requested</param>
        /// <param name="accessGranted">Whether or not the request has granted or denied</param>
        /// <param name="accessDeniedReason"></param>
        /// <returns>True if the insert succeeded, false otherwise</returns>
        public static bool LogAccess(
            string dbName,
            DatabaseEnvironmentType environmentType,
            string ipAddress,
            string applicationName,
            string endPoint
            , bool? accessGranted,
            string accessDeniedReason)
        {
            return LogAccess(
                dbName,
                new NetworkCredential("", Global.DatabaseConfiguration.Database.GetEnvironmentConnectionString(environmentType)).Password,
                ipAddress,
                applicationName,
                endPoint,
                accessGranted,
                accessDeniedReason);
        }

        /// <summary>
        /// Insert an access log entry into the database
        /// </summary>
        /// <param name="ipAddress">Visitor or 3rd party IP</param>
        /// <param name="applicationName">Name of applications requested</param>
        /// <param name="endPoint">API End point or method requested</param>
        /// <param name="accessGranted">Whether or not the request has granted or denied</param>
        /// <param name="accessDeniedReason"></param>
        /// <returns>True if the insert succeeded, false otherwise</returns>
        public static bool LogAccess(
            string dbName,
            string connectionString,
            string ipAddress,
            string applicationName,
            string endPoint,
            bool? accessGranted,
            string accessDeniedReason)
        {
            CreateIfNoTableExists(dbName, connectionString);
            DBGeneric dbCon = new DBGeneric(dbName, connectionString, applicationName);

            string accessGrantedStr = "NULL";
            if (accessGranted != null)
            {
                accessGrantedStr = ((bool)accessGranted).ToBoolAsInt().ToString();
            }

            string command = string.Format(@"INSERT INTO {5} (IPAddress, ApplicationName, [EndPoint], AccessGranted, AccessDeniedReason, ConnectionDT)
                                            VALUES ('{0}', '{1}', '{2}', {3}, {4}, GETDATE())",
                                            ipAddress,
                                            applicationName,
                                            endPoint,
                                            accessGrantedStr,
                                            string.IsNullOrEmpty(accessDeniedReason) ? "NULL" : "'" + accessDeniedReason + "'",
                                            TableName);

            dbCon.ExecuteNonQuery(command, out string errMsg, false);

            if (string.IsNullOrEmpty(errMsg))
            {
                return false;
            }
            else
            {
                return true;
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
                            SET ANSI_NULLS ON
                            SET QUOTED_IDENTIFIER ON
                            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}].{1}') AND type in (N'U'))
                            BEGIN
                            CREATE TABLE [{0}].{1}(
	                            [ID] [int] IDENTITY(1,1) NOT NULL,
	                            [IPAddress] [varchar](50) NULL,
	                            [ApplicationName] [varchar](200) NULL,
	                            [EndPoint] [varchar](1000) NULL,
	                            [AccessGranted] [bit] NULL,
	                            [AccessDeniedReason] [varchar](1000) NULL,
	                            [ConnectionDT] [datetime] NULL,
                            PRIMARY KEY CLUSTERED 
                            (
	                            [ID] ASC
                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                            ) ON [PRIMARY]
                            END",
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
