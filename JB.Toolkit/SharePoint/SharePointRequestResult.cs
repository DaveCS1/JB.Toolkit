using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBToolkit.SharePoint
{
    /// <summary>
    /// SharePoint request (i.e. upload, download, manage) results object
    /// </summary>
    public class SharePointRequestResult
    {
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public string ResultMessage { get; set; }
        public TimeSpan Elapsed { get; set; }

        public string ExecutionTime
        {
            get
            {
                // Format and display the TimeSpan value.
                return string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    Elapsed.Hours, Elapsed.Minutes, Elapsed.Seconds,
                    Elapsed.Milliseconds / 10);
            }
        }
    }
}
