using JBToolkit.Windows;
using System;
using System.IO;

namespace JBToolkit.PdfDoc
{
    /// <summary>
    /// Methods to convert or save PDF file as HTML - Outputs the directory containing the HTML and image files
    /// </summary>
    public partial class PdfConverter
    {
        /// <summary>
        /// Saves a PDF file as HTML file(s) - 1 HTML file per PDF page. The output string is the root path where the files have been created
        /// </summary>
        /// <param name="path">PDF File path to convert</param>
        /// <param name="outputRootPath">Optional - Root path to save to (will create directory if it doesn't exist)</param>
        /// <param name="documentName">Name used for document site / document name</param>
        /// <param name="overwriteIfExists">Attempt to delete current target root directory if it already exists (overwrite)</param>
        /// <param name="timeoutSeconds">Timeout before reporting failing</param>
        /// <returns>RSoot path where the files have been created</returns>
        public static string ConvertPdfToHtml(string path, string documentName, string outputRootPath = "", bool overwriteIfExists = false, int timeoutSeconds = 30)
        {
            if (string.IsNullOrEmpty(documentName))
            {
                throw new ApplicationException("Document name cannot be empty");
            }

            if (overwriteIfExists)
            {
                if (Directory.Exists(Path.Combine(outputRootPath, documentName)))
                {
                    foreach (string filePath in Directory.GetFiles(Path.Combine(outputRootPath, documentName)))
                    {
                        try
                        {
                            File.Delete(filePath);
                        }
                        catch { }
                    }

                    try
                    {
                        Directory.Delete(Path.Combine(outputRootPath, documentName));
                    }
                    catch { }
                }
            }

            if (!Directory.Exists(outputRootPath))
            {
                Directory.CreateDirectory(outputRootPath);
            }

            string execPath = GetPdfToHtmlExeLocation();
            _ = ProcessHelper.ExecuteProcessAndReadStdOut(execPath, out string _, "-r 300 -z 2.0 \"" + path + "\" \"" + documentName + "\"", outputRootPath, timeoutSeconds, true);
            return Path.Combine(string.IsNullOrEmpty(outputRootPath) ? Path.GetDirectoryName(path) : outputRootPath, documentName);
        }

        private static string GetPdfToHtmlExeLocation()
        {
            return AssemblyHelper.EmbeddedResourceHelper.InternalGetEmbeddedResourcePathFromJBToolkit("pdftohtml.exe", true);
        }
    }
}
