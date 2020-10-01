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
        /// Delete a folder and all files and folders within recursively from a SharePoint document library
        /// 
        /// Note: Use:- JBToolkit.SharePoint.Authentication.GetAppOnlyContext
        ///       or    JBToolkit.SharePoint.Authentication.GetUserContext 
        ///             
        ///       to retrive the Client Context for the site.
        /// </summary>
        /// <param name="cContext">SharePoint client context</param>
        /// <param name="documentLibraryPath">Document collection path (i.e. Share Documents/Subfolder)</param>
        /// <returns>SharePointRequestResult result object</returns>
        public static SharePointRequestResult DeleteFolder(
            ClientContext clientContext,
            string documentLibraryPath)
        {
            var result = new SharePointRequestResult();
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                documentLibraryPath.Replace("\\", "/");

                var files = clientContext.Web.GetFolderByServerRelativeUrl(documentLibraryPath).Files;
                var folders = clientContext.Web.GetFolderByServerRelativeUrl(documentLibraryPath).Folders;

                clientContext.Load(files);
                clientContext.Load(folders);

                clientContext.ExecuteQuery();
                foreach (var file in files)
                {
                    file.DeleteObject();
                    file.Update();
                }

                clientContext.ExecuteQuery();
                foreach (var subFolder in folders)
                {
                    string subfolderName = subFolder.ServerRelativeUrl.ToString().Substring(subFolder.ServerRelativeUrl.ToString().IndexOf(documentLibraryPath));
                    DeleteFolder(clientContext, subFolder.ServerRelativeUrl);
                }

                documentLibraryPath.Replace("\\", "/");

                var folder = clientContext.Web.GetFolderByServerRelativeUrl(documentLibraryPath);
                folder.DeleteObject();
                clientContext.ExecuteQuery();

            }
            catch (Exception e)
            {
                if (e.Message.ToLower().Contains("file not found"))
                {
                    if (documentLibraryPath.StartsWith("Documents"))
                    {
                        string newPath = "Shared Documents" + documentLibraryPath.Substring(9, documentLibraryPath.Length - 9);
                        return DeleteFolder(clientContext, newPath);
                    }
                }

                stopWatch.Stop();
                result.IsError = true;
                result.Elapsed = stopWatch.Elapsed;
                result.ErrorMessage = e.Message;
            }

            result.IsError = false;
            result.Elapsed = stopWatch.Elapsed;
            result.ResultMessage = "OK";

            return result;
        }
    }
}
