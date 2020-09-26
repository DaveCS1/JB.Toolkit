using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace JBToolkit.SharePoint.CSOM
{
    /// <summary>
    /// SharePoint file and folder management utilities using CSOM
    /// </summary>
    public partial class Manage
    {
        /// <summary>
        /// Get whether or not a given folder exist in a SharePoint document library
        /// 
        /// Note: Use:- JBToolkit.SharePoint.Authentication.GetAppOnlyContext
        ///       or    JBToolkit.SharePoint.Authentication.GetUserContext 
        ///             
        ///       to retrive the Client Context for the site.
        /// </summary>
        /// <param name="clientContext">SharePoint client context</param>
        /// <param name="documentLibraryPath">Document collection path (i.e. Share Documents/Subfolder)</param>
        /// <returns>SharePointRequestResult result object</returns>
        public static SharePointRequestResult DoesFolderExist(
            ClientContext clientContext,
            string documentLibraryPath)
        {
            var result = new SharePointRequestResult();
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                documentLibraryPath.Replace("\\", "/");
                var folder = clientContext.Web.GetFolderByServerRelativeUrl(
                    Utils.GetServerRelativeUrl(clientContext) + "/" + documentLibraryPath);

                clientContext.Load(folder);
                clientContext.ExecuteQuery();

                if (folder != null && folder.Name != null)
                {
                    result.IsError = false;
                    result.Elapsed = stopWatch.Elapsed;
                    result.ResultMessage = "True";

                    return result;
                }
            }
            catch (Exception e)
            {
                if (e.Message.ToLower().Contains("file not found"))
                {
                    if (documentLibraryPath.StartsWith("Documents"))
                    {
                        string newPath = "Shared Documents" + documentLibraryPath.Substring(9, documentLibraryPath.Length - 9);
                        return DoesFolderExist(clientContext, newPath);
                    }
                    else
                    {
                        result.IsError = false;
                        result.Elapsed = stopWatch.Elapsed;
                        result.ResultMessage = "False";

                        return result;
                    }
                }

                stopWatch.Stop();
                result.IsError = true;
                result.Elapsed = stopWatch.Elapsed;
                result.ErrorMessage = e.Message;
            }

            result.IsError = false;
            result.Elapsed = stopWatch.Elapsed;
            result.ResultMessage = "False";

            return result;
        }
    }
}
