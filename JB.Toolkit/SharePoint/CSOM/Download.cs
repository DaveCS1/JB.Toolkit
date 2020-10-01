using Microsoft.SharePoint.Client;
using System;
using System.Diagnostics;

namespace JBToolkit.SharePoint.CSOM
{
    /// <summary>
    /// SharePoint file and folder download utilities using SCOM
    /// </summary>
    public class Download
    {
        /// <summary>
        /// Downloads a single file from SharePoint
        /// 
        /// Note: Use:- JBToolkit.SharePoint.Authentication.GetAppOnlyContext
        ///       or    JBToolkit.SharePoint.Authentication.GetUserContext 
        ///             
        ///       to retrive the Client Context for the site.
        /// </summary>
        /// <param name="clientContext">SharePoint client context</param>
        /// <param name="documentLibraryPath">Document collection path (i.e. Share Documents/Subfolder)</param>
        /// <param name="fileName">Name of folder to download (i.e. filename.txt)</param>
        /// <param name="downloadFolderPath">Path to folder to download to</param>
        /// <param name="overwrite">Won't overwrite the local file if set to false (default: true)</param>
        /// <param name="renameFilename">Save the file with a different, specified filename</param>
        /// <returns>SharePointRequestResult result object</returns>
        public static SharePointRequestResult DownloadFile(
            ClientContext clientContext,
            string documentLibraryPath,
            string fileName,
            string downloadFolderPath,
            bool overwrite = true,
            string renameFilename = null)
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

                var clientResultStream = file.OpenBinaryStream();
                clientContext.ExecuteQuery();

                var filePath = System.IO.Path.Combine(downloadFolderPath, string.IsNullOrEmpty(renameFilename)
                                                                                    ? file.Name
                                                                                    : renameFilename);
                if (!overwrite && System.IO.File.Exists(filePath))
                {
                    stopWatch.Stop();
                    result.IsError = true;
                    result.ErrorMessage = "File already exists and you've set the method not to overwrite";
                    result.Elapsed = stopWatch.Elapsed;

                    return result;
                }

                using (var stream = clientResultStream.Value)
                using (var fileStream = System.IO.File.Create(filePath))
                    Utils.CopyStream(stream, fileStream);

                stopWatch.Stop();
                result.IsError = false;
                result.ResultMessage = "OK";
                result.Elapsed = stopWatch.Elapsed;

            }
            catch (Exception e)
            {
                if (e.Message.ToLower().Contains("file not found"))
                {
                    if (documentLibraryPath.StartsWith("Documents"))
                    {
                        string newPath = "Shared Documents" + documentLibraryPath.Substring(9, documentLibraryPath.Length - 9);
                        DownloadFile(clientContext, newPath, fileName, downloadFolderPath);
                    }
                }

                stopWatch.Stop();
                result.IsError = true;
                result.ErrorMessage = e.Message;
                result.Elapsed = stopWatch.Elapsed;
            }

            return result;
        }

        /// <summary>
        /// Download all files in a folder from SharePoint (doesn't include subdirectories)
        /// </summary>
        /// <param name="clientContext">SharePoint client context</param>
        /// <param name="documentLibraryPath">Document collection path (i.e. Share Documents/Subfolder)</param>
        /// <param name="downloadFolderPath">Path of folder to download to</param>
        /// <returns>SharePointRequestResult result object</returns>
        public static SharePointRequestResult DownloadFiles(
            ClientContext clientContext,
            string documentLibraryPath,
            string downloadFolderPath)
        {
            var result = new SharePointRequestResult();
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                documentLibraryPath.Replace("\\", "/");

                var files = clientContext.Web.GetFolderByServerRelativeUrl(
                    Utils.GetServerRelativeUrl(clientContext) + "/" + documentLibraryPath).Files;

                clientContext.Load(files);
                clientContext.ExecuteQuery();

                foreach (var file in files)
                {
                    var clientResultStream = file.OpenBinaryStream();
                    clientContext.ExecuteQuery();

                    var filePath = System.IO.Path.Combine(downloadFolderPath, file.Name);
                    using (var stream = clientResultStream.Value)
                    using (var fileStream = System.IO.File.Create(filePath))
                        Utils.CopyStream(stream, fileStream);
                }

                stopWatch.Stop();
                result.IsError = true;
                result.ResultMessage = "OK";
                result.Elapsed = stopWatch.Elapsed;
            }
            catch (Exception e)
            {
                if (e.Message.ToLower().Contains("file not found"))
                {
                    if (documentLibraryPath.StartsWith("Documents"))
                    {
                        string newPath = "Shared Documents" + documentLibraryPath.Substring(9, documentLibraryPath.Length - 9);
                        DownloadFiles(clientContext, newPath, downloadFolderPath);
                    }
                }

                stopWatch.Stop();
                result.IsError = true;
                result.ErrorMessage = e.Message;
                result.Elapsed = stopWatch.Elapsed;
            }

            return result;
        }
    }
}
