﻿using Microsoft.SharePoint.Client;
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
        /// Create a new folder in a document library on SharePoint
        /// 
        /// Note: Use:- JBToolkit.SharePoint.Authentication.GetAppOnlyContext
        ///       or    JBToolkit.SharePoint.Authentication.GetUserContext 
        ///             
        ///       to retrive the Client Context for the site.
        /// </summary>
        /// <param name="clientContext">SharePoint client context</param>
        /// <param name="documentLibraryPath">Document collection path (i.e. Share Documents/Subfolder)</param>
        /// <param name="folderName">Name of folder to create</param>
        /// <returns>SharePointRequestResult result object</returns>
        public static SharePointRequestResult CreateFolder(
            ClientContext clientContext,
            string documentLibraryPath,
            string folderName)
        {
            var result = new SharePointRequestResult();
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                documentLibraryPath.Replace("\\", "/");
                var folder = clientContext.Web.GetFolderByServerRelativeUrl(documentLibraryPath);

                clientContext.Load(folder);

                folder.AddSubFolder(folderName, new ListItemUpdateParameters());
                clientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                if (e.Message.ToLower().Contains("file not found"))
                {
                    if (documentLibraryPath.StartsWith("Documents"))
                    {
                        string newPath = "Shared Documents" + documentLibraryPath.Substring(9, documentLibraryPath.Length - 9);
                        return CreateFolder(clientContext, newPath, folderName);
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