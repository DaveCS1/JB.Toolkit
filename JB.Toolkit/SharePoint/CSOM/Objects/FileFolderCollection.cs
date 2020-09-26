using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBToolkit.SharePoint.CSOM.Objects
{
    public class FileFolderCollection
    {
        public FileCollection Files { get; set; }
        public FolderCollection Folders { get; set; }

    }
}
