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
    /// SharePoint file Upload utilities using SCOM
    /// </summary>
    public class Upload
    {
        /// <summary>
        /// Upload a file to a SharePoint document library.
        /// 
        /// Note: Use:- JBToolkit.SharePoint.Authentication.GetAppOnlyContext
        ///       or    JBToolkit.SharePoint.Authentication.GetUserContext 
        ///             
        ///       to retrive the Client Context for the site.
        /// </summary>
        /// <param name="clientContext">SharePoint client context</param>
        /// <param name="documentLibraryPath">Document library root (i.e. 'Shared Documents')</param>
        /// <param name="subFolderPath">Subfolder path after root (i.e. /Subfolder/Subfolder2) of where to upload to</param>
        /// <param name="sourceFilePath">Source path of local file to upload</param>
        /// <param name="overwrite">Whether or not to overwrite if file already exist (defaults to true)</param>
        /// <returns>SharePointRequestResult result object</returns>
        public static SharePointRequestResult UploadFile(
            ClientContext clientContext,
            string documentLibraryPath,
            string subFolderPath,
            string sourceFilePath,
            bool overwrite = true)
        {
            var result = new SharePointRequestResult();
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                var web = clientContext.Web;
                var newFile = new FileCreationInformation();

                if (overwrite)
                    newFile.Overwrite = true;

                byte[] fileContent = System.IO.File.ReadAllBytes(sourceFilePath);
                newFile.ContentStream = new System.IO.MemoryStream(fileContent);
                newFile.Url = System.IO.Path.GetFileName(sourceFilePath);
                List documentLibrary = web.Lists.GetByTitle(documentLibraryPath);
                Folder clientSubFolder = null;

                if (string.IsNullOrEmpty(subFolderPath))
                    clientSubFolder = documentLibrary.RootFolder;
                else
                    clientSubFolder = documentLibrary.RootFolder.Folders.GetByUrl(subFolderPath);

                File uploadFile = clientSubFolder.Files.Add(newFile);
                clientContext.Load(documentLibrary);
                clientContext.Load(uploadFile);
                clientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                if (e.Message.ToLower().Contains("does not exist at site") || documentLibraryPath.StartsWith("Shared Documents"))
                {
                    string newPath = "Documents" + documentLibraryPath.Substring(16, documentLibraryPath.Length - 16);
                    return UploadFile(clientContext, newPath, subFolderPath, sourceFilePath, overwrite);
                }

                stopWatch.Stop();
                result.IsError = true;
                result.Elapsed = stopWatch.Elapsed;
                result.ErrorMessage = e.Message;
            }

            stopWatch.Stop();
            result.IsError = false;
            result.Elapsed = stopWatch.Elapsed;
            result.ResultMessage = "OK";

            return result;
        }
    }
}

