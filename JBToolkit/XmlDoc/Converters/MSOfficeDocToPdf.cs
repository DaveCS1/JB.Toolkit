using System.IO;

namespace JBToolkit.XmlDoc.Converters
{
    /// <summary>
    /// Methods to convert Docx to PDF through M-Files. I've done it this way as it's costs a tonne for a docx to pdf converter license with the likes of Aspose, Spire, ABCpdf etc and M-Files
    /// is already using Aspose so just utilise that. The process involves uploading a file to M-Files, converting it to a PDF there, then downloading it as PDF and finally deleting (destroying) the file
    /// in the M-Files vault
    /// </summary>
    public class MSOfficeDocToPdf
    {
        /// <summary>
        /// Converts MS Office document (xlsx, doc, ppt etc) to memory stream (i.e. for use in Web Requests)
        /// </summary>
        /// <param name="docxInputPath">File path</param>
        /// <returns>Memory stream</returns>
        public static MemoryStream ConvertToPDF(string inputPath)
        {
            return PdfDoc.PdfConverter.ConvertToPDF(inputPath);
        }

        /// <summary>
        /// Converts MS Office document (xlsx, doc, ppt etc) to memory stream (i.e. for use in Web Requests)
        /// </summary>
        /// <param name="docxInputPath">File path</param>
        /// <returns>Memory stream</returns>
        public static MemoryStream ConvertToPDF(MemoryStream ms, string fileExtension)
        {
            return PdfDoc.PdfConverter.ConvertToPDF(ms, fileExtension);
        }

        /// <summary>
        /// Save a MS Office doc as a PDF
        /// </summary>
        /// <param name="inputPath">.docx path</param>
        /// <param name="pdfOutputPath">Save PDF file path</param>
        public static void SaveAsPdf(string inputPath, string pdfOutputPath)
        {
            PdfDoc.PdfConverter.SaveAsPdf(inputPath, pdfOutputPath);
        }
    }
}
