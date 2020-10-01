using Microsoft.SharePoint.Client;

namespace JBToolkit.SharePoint.CSOM
{
    /// <summary>
    /// SharePoint SCOM utility methods
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Often needed for 'Manage' methods - Gets the client context server relative url (i.e. /sites/D365DEV)
        /// 
        /// Note: Use:- JBToolkit.SharePoint.Authentication.GetAppOnlyContext
        ///       or    JBToolkit.SharePoint.Authentication.GetUserContext 
        ///             
        ///       to retrive the Client Context for the site.
        /// </summary>
        /// <param name="clientContext">SharePoint client context</param>
        /// <returns>Server relative path string</returns>
        public static string GetServerRelativeUrl(ClientContext clientContext)
        {
            var web = clientContext.Web;
            List lib = web.Lists.GetByTitle("Documents");
            clientContext.Load(lib, l => l.ParentWeb.ServerRelativeUrl);
            clientContext.ExecuteQuery();

            return lib.ParentWeb.ServerRelativeUrl;
        }

        public static void CopyStream(System.IO.Stream input, System.IO.Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
                output.Write(buffer, 0, len);
        }
    }
}
