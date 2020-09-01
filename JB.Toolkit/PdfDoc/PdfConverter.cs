using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.IO;

namespace JBToolkit.PdfDoc
{
    /// <summary>
    /// Methods to convert a document file or image to PDF through M-Files. I've done it this way as it's costs a tonne for a pdf converter license with the likes of Aspose, Spire, ABCpdf etc and M-Files
    /// is already using Aspose so just utilise that. The process involves uploading a file to M-Files, converting it to a PDF there, then downloading it as PDF and finally deleting (destroying) the file
    /// in the M-Files vault
    /// </summary>
    public partial class PdfConverter
    {
        /// <summary>
        /// Save document file or image to PDF
        /// </summary>
        /// <param name="docInputPath">Input document path</param>
        /// <param name="pdfOutputPath">Output .pdf path</param>
        public static void SaveAsPdf(string docInputPath, string pdfOutputPath)
        {
            using (FileStream fs = new FileStream(pdfOutputPath, FileMode.OpenOrCreate))
            {
                ConvertToPdf(docInputPath).CopyTo(fs);
                fs.Flush();
            }
        }

        /// <summary>
        /// Convert and save pretty much any office file (.docx, .xlsx, .pptx, .vsdx, .pub, .msg etc), image or text file to PDF
        /// </summary>
        /// <param name="docInputPath">Input document or image path path</param>
        /// <param name="pdfOutputPath">Output .pdf path</param>
        public static void ConvertToPdf(string docInputPath, string pdfOutputPath)
        {
            using (FileStream fs = new FileStream(pdfOutputPath, FileMode.OpenOrCreate))
            {
                ConvertToPdf(docInputPath).CopyTo(fs);
                fs.Flush();
            }
        }

        /// <summary>
        /// Converts document file or image to PDF memory stream (i.e. for use in Web Requests)
        /// </summary>
        /// <param name="docInputPut">File path</param>
        /// <param name="fileExtension">The PDF converter can't use a memory stream, as a workaround we save a temporary file, so we need a file extension to determine the file type</param>
        /// <returns>Memory stream</returns>
        public static MemoryStream ConvertToPdf(MemoryStream ms, string fileExtension)
        {
            string tempFile = Windows.DirectoryHelper.GetTempFile() + "." + fileExtension.Replace(".", "");
            File.WriteAllBytes(tempFile, ms.ToArray());

            using (MemoryStream nms = ConvertToPdf(tempFile))
            {
                File.Delete(tempFile);
                return nms;
            }
        }

        /// <summary>
        /// Converts document file or image to PDF memory stream (i.e. for use in Web Requests)
        /// </summary>
        /// <param name="docInputPut">File path</param>
        /// <returns>Memory stream</returns>
        public static MemoryStream ConvertToPdf(string docInputPut)
        {
            // Not the foggiest idea why, but somehow the below fixes any memory stream issues

            MemoryStream ms = new MemoryStream();
            using (PdfDocument one = PdfReader.Open(ConvertToPdfMemoryStream(docInputPut), PdfDocumentOpenMode.Import))
            using (PdfDocument outPdf = new PdfDocument())
            {
                CopyPages(one, outPdf);
                outPdf.Save(ms);

                return ms;
            }
        }

        /// <summary>
        /// Basically duplicates a PDF file (useful for creating new memory streams in conversion functions)
        /// </summary>
        private static void CopyPages(PdfDocument from, PdfDocument to)
        {
            for (int i = 0; i < from.PageCount; i++)
            {
                to.AddPage(from.Pages[i]);
            }
        }

        /// <summary>
        /// Convert a file as a PDF memory stream
        /// </summary>
        /// <param name="docInputPath">Path to file to convert</param>
        private static MemoryStream ConvertToPdfMemoryStream(string docInputPath)
        {
            return XmlDoc.Converters.OfficeHtmlPdfImageConverter.ConvertDocumentAndReturnMemoryStream(docInputPath, ".pdf");
        }
    }
}
