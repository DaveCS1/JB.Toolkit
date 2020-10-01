using Microsoft.SharePoint.Client;

namespace JBToolkit.SharePoint.CSOM.Objects
{
    public class FileFolderCollection
    {
        public FileCollection Files { get; set; }
        public FolderCollection Folders { get; set; }

    }
}
