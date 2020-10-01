using Microsoft.SharePoint.Client;
using System;
using System.Diagnostics;

namespace JBToolkit.SharePoint.CSOM
{
    /// <summary>
    /// SharePoint file and folder management utilities using CSOM
    /// </summary>
    public partial class Manage
    {
        /// <summary>
        /// Get whether or not a given file exist in a SharePoint document library
        /// 
        /// Note: Use:- JBToolkit.SharePoint.Authentication.GetAppOnlyContext
        ///       or    JBToolkit.SharePoint.Authentication.GetUserContext 
        ///             
        ///       to retrive the Client Context for the site.
        /// </summary>
        /// <param name="clientContext">SharePoint client context</param>
        /// <param name="documentLibraryPath">Document collection path (i.e. Share Documents/Subfolder)</param>
        /// <param name="fileName">Name of file (i.e. filename.txt)</param>
        /// <returns>SharePointRequestResult result object</returns>
        public static SharePointRequestResult DoesFileExist(
            ClientContext clientContext,
            string documentLibraryPath,
            string fileName)
        {
            var result = new SharePointRequestResult();
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                documentLibraryPath.Replace("\\", "/");
                var file = clientContext.Web.GetFileByServerRelativeUrl(
                    Utils.GetServerRelativeUrl(clientContext) + "/" + documentLibraryPath + "/" + fileName);

                clientContext.Load(file);
                clientContext.ExecuteQuery();

                if (file != null && file.Name != null && file.Name == fileName)
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
                        return DoesFileExist(clientContext, newPath, fileName);
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
