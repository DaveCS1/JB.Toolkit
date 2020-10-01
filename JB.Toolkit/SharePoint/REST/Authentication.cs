using System.IO;
using System.Net;
using System.Text;

namespace JBToolkit.SharePoint.REST
{
    /// <summary>
    /// Authenticate with SharePoint using the REST API
    /// </summary>
    public class Authentication
    {
        private static string m_tokenGenUrl = "https://accounts.accesscontrol.windows.net/<tenant-id>/tokens/OAuth/2";
        private static string resourceId = "00000003-0000-0ff1-ce00-000000000000";
        public static string GetAccessToken(
            string siteName,
            string tenantId,
            string clientId,
            string clientSecret)
        {
            string accessTokenUrl = m_tokenGenUrl.Replace("<tenant-id>", tenantId);
            string accessToken = string.Empty;

            var request = WebRequest.Create(accessTokenUrl);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";

            string postData = $"grant_type=client_credentials" +
                              $"&client_id={clientId}%40{tenantId}" +
                              $"&client_secret={WebUtility.UrlEncode(clientSecret)}" +
                              $"&resource={resourceId}%2F{siteName}.sharepoint.com%40{tenantId}";

            var data = Encoding.ASCII.GetBytes(postData);
            using (var stream = request.GetRequestStream())
                stream.Write(data, 0, data.Length);

            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            string[] stArrResponse = responseString.Split(',');
            foreach (var stValues in stArrResponse)
            {
                if (stValues.StartsWith("\"access_token\":"))
                {
                    accessToken = stValues.Substring(16);
                    accessToken = accessToken.Substring(0, accessToken.Length - 2);
                }
            }

            return accessToken;
        }
    }
}
