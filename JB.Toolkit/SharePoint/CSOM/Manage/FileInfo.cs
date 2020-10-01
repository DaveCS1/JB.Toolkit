using JBToolkit.SharePoint.CSOM.Objects;
using Microsoft.SharePoint.Client;
using System;

namespace JBToolkit.SharePoint.CSOM
{
    /// <summary>
    /// SharePoint file and folder management utilities using SCCSOMOM
    /// </summary>
    public partial class Manage
    {
        /// <summary>
        /// Get a list of files contained in a SharePoint folder (non-recursive)
        /// 
        /// Note: Use:- JBToolkit.SharePoint.Authentication.GetAppOnlyContext
        ///       or    JBToolkit.SharePoint.Authentication.GetUserContext 
        ///             
        ///       to retrive the Client Context for the site.
        /// </summary>
        /// <param name="clientContext">SharePoint client context</param>
        /// <param name="documentLibraryPath">Document collection path (i.e. Share Documents/Subfolder)</param>
        /// <returns>SharePoint FileCollection object</returns>
        public static FileCollection GetFileCollectionFromFolder(
                        ClientContext clientContext,
                        string documentLibraryPath)
        {
            try
            {
                documentLibraryPath.Replace("\\", "/");
                var files = clientContext.Web.GetFolderByServerRelativeUrl(
                    Utils.GetServerRelativeUrl(clientContext) + "/" + documentLibraryPath).Files;

                clientContext.Load(files);
                clientContext.ExecuteQuery();

                return files;
            }
            catch (Exception e)
            {
                if (e.Message.ToLower().Contains("file not found"))
                {
                    if (documentLibraryPath.StartsWith("Documents"))
                    {
                        string newPath = "Shared Documents" + documentLibraryPath.Substring(9, documentLibraryPath.Length - 9);
                        return GetFileCollectionFromFolder(clientContext, newPath);
                    }
                    else
                        throw;
                }

                throw;
            }
        }

        /// <summary>
        /// Get a list of folders contained in a SharePoint folder (non-recursive)
        /// 
        /// Note: Use:- JBToolkit.SharePoint.Authentication.GetAppOnlyContext
        ///       or    JBToolkit.SharePoint.Authentication.GetUserContext 
        ///             
        ///       to retrive the Client Context for the site.
        /// </summary>
        /// <param name="clientContext">SharePoint client context</param>
        /// <param name="documentLibraryPath">Document collection path (i.e. Share Documents/Subfolder)</param>
        /// <returns>SharePoint FileCollection object</returns>
        public static FolderCollection GetFolderCollectionFromFolder(
                ClientContext clientContext,
                string documentLibraryPath)
        {
            try
            {
                documentLibraryPath.Replace("\\", "/");
                var folders = clientContext.Web.GetFolderByServerRelativeUrl(
                    Utils.GetServerRelativeUrl(clientContext) + "/" + documentLibraryPath).Folders;

                clientContext.Load(folders);
                clientContext.ExecuteQuery();

                return folders;
            }
            catch (Exception e)
            {
                if (e.Message.ToLower().Contains("file not found"))
                {
                    if (documentLibraryPath.StartsWith("Documents"))
                    {
                        string newPath = "Shared Documents" + documentLibraryPath.Substring(9, documentLibraryPath.Length - 9);
                        return GetFolderCollectionFromFolder(clientContext, newPath);
                    }
                    else
                        throw;
                }

                throw;
            }
        }

        /// <summary>
        /// Get an object containing a list of folders and a list of files contained in a SharePoint folder (non-recursive)
        /// 
        /// Note: Use:- JBToolkit.SharePoint.Authentication.GetAppOnlyContext
        ///       or    JBToolkit.SharePoint.Authentication.GetUserContext 
        ///             
        ///       to retrive the Client Context for the site.
        /// </summary>
        /// <param name="clientContext">SharePoint client context</param>
        /// <param name="documentLibraryPath">Document collection path (i.e. Share Documents/Subfolder)</param>
        /// <returns>SharePoint FileCollection object</returns>
        public static FileFolderCollection GetFileAndFolderCollectionFromFolder(
                        ClientContext clientContext,
                        string documentLibraryPath)
        {
            try
            {
                documentLibraryPath.Replace("\\", "/");
                var files = clientContext.Web.GetFolderByServerRelativeUrl(
                    Utils.GetServerRelativeUrl(clientContext) + "/" + documentLibraryPath).Files;

                clientContext.Load(files);
                clientContext.ExecuteQuery();

                var folders = clientContext.Web.GetFolderByServerRelativeUrl(
                                    Utils.GetServerRelativeUrl(clientContext) + "/" + documentLibraryPath).Folders;

                clientContext.Load(folders);
                clientContext.ExecuteQuery();

                return new FileFolderCollection { Files = files, Folders = folders };
            }
            catch (Exception e)
            {
                if (e.Message.ToLower().Contains("file not found"))
                {
                    if (documentLibraryPath.StartsWith("Documents"))
                    {
                        string newPath = "Shared Documents" + documentLibraryPath.Substring(9, documentLibraryPath.Length - 9);
                        return GetFileAndFolderCollectionFromFolder(clientContext, newPath);
                    }
                    else
                        throw;
                }

                throw;
            }
        }
    }
}
