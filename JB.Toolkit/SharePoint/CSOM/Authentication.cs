using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using System.Security;

namespace JBToolkit.SharePoint.CSOM
{
    /// <summary>
    /// Authenticate with SharePoint using SCOM API
    /// </summary>
    public class Authentication
    {
        /// <summary>
        /// Retrieve the SharePoint client app only context via app client id and secret key in order to make requests using SCOM
        /// </summary>
        /// <param name="siteUrl">SharePoint site URL</param>
        /// <param name="clientId">App client ID</param>
        /// <param name="clientSecret">App secret key</param>
        /// <returns>SharePoint client app only context</returns>
        public static ClientContext GetAppOnlyContext(string siteUrl, string clientId, string clientSecret)
        {
            var cContext = new AuthenticationManager().GetAppOnlyAuthenticatedContext(siteUrl, clientId, clientSecret);
            return cContext;
        }

        /// <summary>
        /// Retrieve the SharePoint client context user context via username and password in order to make requests using SCOM
        /// </summary>
        /// <param name="siteUrl">SharePoint site URL</param>
        /// <param name="username">Credentials username (email)</param>
        /// <param name="password">Credentials password</param>
        /// <returns>SharePoint client user context</returns>
        public static ClientContext GetUserContext(string siteUrl, string username, string password)
        {
            var securePassword = new SecureString();
            foreach (char c in password)
                securePassword.AppendChar(c);

            var onlineCredentials = new SharePointOnlineCredentials(username, securePassword);

            var cContext = new ClientContext(siteUrl)
            {
                Credentials = onlineCredentials
            };

            return cContext;
        }

        /// <summary>
        /// Retrieve the SharePoint client context user context via username and password in order to make requests using SCOM
        /// </summary>
        /// <param name="siteUrl">SharePoint site URL</param>
        /// <param name="username">Credentials username (email)</param>
        /// <param name="password">Credentials password</param>
        /// <returns>SharePoint client user context</returns>
        public static ClientContext GetUserContext(string siteUrl, string username, SecureString password)
        {
            var onlineCredentials = new SharePointOnlineCredentials(username, password);
            var cContext = new ClientContext(siteUrl)
            {
                Credentials = onlineCredentials
            };

            return cContext;
        }
    }
}