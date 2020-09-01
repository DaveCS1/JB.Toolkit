using System;
using System.IO;

namespace JBToolkit.PdfDoc
{
    /// <summary>
    /// Methods to convert Docx to PDF through M-Files. I've done it this way as it's costs a tonne for a docx to pdf converter license with the likes of Aspose, Spire, ABCpdf etc and M-Files
    /// is already using Aspose so just utilise that. The process involves uploading a file to M-Files, converting it to a PDF there, then downloading it as PDF and finally deleting (destroying) the file
    /// in the M-Files vault
    /// </summary>
    public partial class PdfConverter
    {
        /// <summary>
        /// Save Docx document to PDF to file
        /// <param name="docInputPath">Input .docx path</param>
        /// <param name="pdfOutputPath">Output .pdf path</param>
        public static void SaveMsOfficeDocAsPdf(string docInputPath, string pdfOutputPath)
        {
            if (!new FileInfo(docInputPath).Extension.ToLower().Contains("docx"))
            {
                throw new ArgumentException("The input file type is not .docx", new FileInfo(docInputPath).Extension);
            }

            SaveAsPdf(docInputPath, pdfOutputPath);
        }

        /// <summary>
        /// Converts Docx document to PDF memory stream (i.e. for use in Web Requests)
        /// </summary>
        /// <param name="docInputPath">File path</param>
        /// <returns>Memory stream</returns>
        public static MemoryStream ConvertMsOfficeDocToPdf(string docInputPath)
        {
            string extension = new FileInfo(docInputPath).Extension.ToLower();

            if (!extension.In(Windows.CommonFileTypeExtensions.MicrosoftOffice.Excel) &&
                !extension.In(Windows.CommonFileTypeExtensions.MicrosoftOffice.Word) &&
                !extension.In(Windows.CommonFileTypeExtensions.MicrosoftOffice.PowerPoint) &&
                !extension.In(Windows.CommonFileTypeExtensions.MicrosoftOffice.Visio))
            {
                throw new ArgumentException("The input file type is not a Microsoft office file type of 'Excel, Word, PowerPoint or Visio'", new FileInfo(docInputPath).Extension);
            }

            return ConvertToPdf(docInputPath);
        }

        /// <summary>
        /// Converts Docx document to PDF memory stream (i.e. for use in Web Requests)
        /// </summary>
        /// <param name="docxInputPath">File path</param>
        /// <returns>Memory stream</returns>
        public static MemoryStream ConvertMsOfficeDocToPdf(MemoryStream ms, string fileExtension)
        {
            return ConvertToPdf(ms, fileExtension);
        }
    }
}
