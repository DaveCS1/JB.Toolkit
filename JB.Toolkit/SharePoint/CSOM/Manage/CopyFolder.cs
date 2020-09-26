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
        /// Copy a folder from one SharePoint location to another
        /// 
        /// Note: Use:- JBToolkit.SharePoint.Authentication.GetAppOnlyContext
        ///       or    JBToolkit.SharePoint.Authentication.GetUserContext 
        ///             
        ///       to retrive the Client Context for the site.
        /// </summary>
        /// <param name="clientContext">SharePoint client context</param>
        /// <param name="fileLibrarySourcePath">Source document path (i.e. 'Shared Documents/Subfolder1')</param>
        /// <param name="fileLibraryDestinationPath">Destination document path (i.e. 'Shared Documents/Subfolder2'</param>
        /// <returns>SharePointRequestResult result object</returns>
        public static SharePointRequestResult CopyFolder(
            ClientContext clientContext,
            string fileLibrarySourcePath,
            string fileLibraryDestinationPath)
        {
            var result = new SharePointRequestResult();
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                fileLibrarySourcePath.Replace("\\", "/");
                fileLibraryDestinationPath.Replace("\\", "/");

                MoveCopyOptions option = new MoveCopyOptions
                {
                    KeepBoth = false
                };
                MoveCopyUtil.CopyFolder(clientContext, clientContext.Url + "/" + fileLibrarySourcePath, clientContext.Url + "/" + fileLibraryDestinationPath, option);
                clientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                if (e.Message.ToLower().Contains("file not found"))
                {
                    if (fileLibrarySourcePath.StartsWith("Documents") && fileLibraryDestinationPath.StartsWith("Documents"))
                    {
                        string newSourcePath = "Shared Documents" + fileLibrarySourcePath.Substring(9, fileLibrarySourcePath.Length - 9);
                        string newDistinationPath = "Shared Documents" + fileLibraryDestinationPath.Substring(9, fileLibraryDestinationPath.Length - 9);

                        return CopyFolder(clientContext, newSourcePath, newDistinationPath);
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
