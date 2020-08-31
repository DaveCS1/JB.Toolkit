using iTextSharp.text.pdf;
using JBToolkit.Windows;
using System;
using System.IO;
using System.Text;

namespace JBToolkit.PdfDoc
{
    /// <summary>
    /// PDF Parsing methods
    /// 
    ///  
    /// </summary>
    public class PdfParser
    {
        /// <summary>
        /// Converts PDF to pure text using iTextSharp (very quick) and optionally falls back to using xPDF PDFtoText command line utility (embedded)
        /// if iTextSharp text extract fails 
        /// </summary>
        /// <param name="path">Path to PDF file</param>
        /// <param name="timeoutSeconds">Timeout in seconds before reporting failure</param>
        /// <param name="throwOnError">Whether not to throw (break) on error just return an empty string</param>
        /// <param name="utilisePDFtoTextCommandLineUtility">Default to true - Flag to optionally use the embedded xPDF PdftoText command line utility (slow) as a fallback when iTextSharp doesn't work</param>
        /// <returns>Content as string</returns>
        public static string GetPDFContentAsString(
            string path,
            int timeoutSeconds = 30,
            bool throwOnError = true,
            bool utilisePDFtoTextCommandLineUtility = true)
        {
            try
            {
                var reader = new PdfReader(path);

                StringBuilder sb = new StringBuilder();

                try
                {
                    for (int page = 1; page <= reader.NumberOfPages; page++)
                    {
                        var cpage = reader.GetPageN(page);
                        var content = cpage.Get(PdfName.CONTENTS);

                        var ir = (PRIndirectReference)content;

                        var value = reader.GetPdfObject(ir.Number);

                        if (value.IsStream())
                        {
                            PRStream stream = (PRStream)value;
                            var streamBytes = PdfReader.GetStreamBytes(stream);
                            var tokenizer = new PRTokeniser(new RandomAccessFileOrArray(streamBytes));

                            try
                            {
                                while (tokenizer.NextToken())
                                {
                                    if (tokenizer.TokenType == PRTokeniser.TK_STRING)
                                    {
                                        string str = tokenizer.StringValue;
                                        sb.Append(str.Replace("x-none", " "));
                                    }
                                }
                            }
                            finally
                            {
                                tokenizer.Close();
                            }
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }

                if (string.IsNullOrEmpty(sb.ToString()))
                {
                    if (utilisePDFtoTextCommandLineUtility)
                    {
                        return GetPDFContentAsString_CmdLineTool(path, timeoutSeconds, throwOnError);
                    }
                    else if (throwOnError)
                    {
                        throw new ApplicationException("Unable to parse PDF");
                    }
                }

                return sb.ToString();
            }
            catch (Exception e)
            {
                if (utilisePDFtoTextCommandLineUtility)
                {
                    return GetPDFContentAsString_CmdLineTool(path, timeoutSeconds, throwOnError);
                }
                else if (throwOnError)
                {
                    throw new ApplicationException("Unable to parse PDF: " + e.Message);
                }

                return string.Empty;
            }
        }


        /// <summary>
        /// Converts PDF to pure text using iTextSharp (very quick) and optionally falls back to using xPDF PDFtoText command line utility (embedded)
        /// if iTextSharp text extract fails.        
        /// </summary>
        /// <param name="path">Path to PDF file</param>
        /// <param name="timeoutSeconds">Timeout in seconds before reporting failure</param>
        /// <param name="throwOnError">Whether not to throw (break) on error just return an empty string</param>
        /// <param name="utilisePDFtoTextCommandLineUtility">Default to true - Flag to optionally use the embedded xPDF PdftoText command line utility (slow) as a fallback when iTextSharp doesn't work</param>
        /// <returns>Content as string</returns>
        public static string GetPDFContentAsString(
            MemoryStream ms,
            int timeoutSeconds = 30,
            bool throwOnError = true,
            bool utilisePDFtoTextCommandLineUtility = true)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                var reader = new PdfReader(ms);

                try
                {
                    for (int page = 1; page <= reader.NumberOfPages; page++)
                    {
                        var cpage = reader.GetPageN(page);
                        var content = cpage.Get(PdfName.CONTENTS);

                        var ir = (PRIndirectReference)content;

                        var value = reader.GetPdfObject(ir.Number);

                        if (value.IsStream())
                        {
                            PRStream stream = (PRStream)value;
                            var streamBytes = PdfReader.GetStreamBytes(stream);
                            var tokenizer = new PRTokeniser(new RandomAccessFileOrArray(streamBytes));

                            try
                            {
                                while (tokenizer.NextToken())
                                {
                                    if (tokenizer.TokenType == PRTokeniser.TK_STRING)
                                    {
                                        string str = tokenizer.StringValue;
                                        sb.Append(str.Replace("x-none", " "));
                                    }
                                }
                            }
                            finally
                            {
                                tokenizer.Close();
                            }
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }

                if (string.IsNullOrEmpty(sb.ToString()))
                {
                    if (utilisePDFtoTextCommandLineUtility)
                    {
                        return GetPDFContentAsString_CmdLineTool(ms, timeoutSeconds, throwOnError);
                    }
                    else if (throwOnError)
                    {
                        throw new ApplicationException("Unable to parse PDF");
                    }
                }

                return sb.ToString();
            }
            catch (Exception e)
            {
                if (utilisePDFtoTextCommandLineUtility)
                {
                    return GetPDFContentAsString_CmdLineTool(ms, timeoutSeconds, throwOnError);
                }
                else if (throwOnError)
                {
                    throw new ApplicationException("Unable to parse PDF: " + e.Message);
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Converts PDF to pure text using xPDF PDFtoText command line utility (embedded)
        /// if iTextSharp text extract fails
        /// </summary>
        /// <param name="path">Path to PDF file</param>
        /// <param name="timeoutSeconds">Timeout in seconds before reporting failure</param>
        /// <param name="throwOnError">Whether not to throw (break) on error just return an empty string</param>
        /// <returns>Content as string</returns>
        public static string GetPDFContentAsString_CmdLineTool(
            string path,
            int timeoutSeconds = 30,
            bool throwOnError = true)
        {
            string execPath = GetPdfToTextExeLocation();
            string content = string.Empty;

            try
            {
                content = ProcessHelper.ExecuteProcessAndReadStdOut(execPath, out string _, "\"" + path + "\" -", "", timeoutSeconds, throwOnError);

                if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(execPath)
                    || content == "\f\r\n\r\n\r\n"
                    || content == "\f\r\n\r\n"
                    || content == "\r\n\f\r\n\r\n")
                {

                    if (throwOnError)
                    {
                        throw new ApplicationException("Unable to parse PDF. Content blank");
                    }

                    return content;
                }

                return content;
            }
            catch (Exception e)
            {
                if (throwOnError)
                {
                    throw new ApplicationException("Unable to parse PDF: " + e.Message);
                }

                return content;
            }
        }

        /// <summary>
        /// Converts PDF to pure text using xPDF PDFtoText command line utility (embedded)
        /// if iTextSharp text extract fails
        /// </summary>
        /// <param name="path">Path to PDF file</param>
        /// <param name="timeoutSeconds">Timeout in seconds before reporting failure</param>
        /// <param name="throwOnError">Whether not to throw (break) on error just return an empty string</param>
        /// <returns>Content as string</returns>
        public static string GetPDFContentAsString_CmdLineTool(
            MemoryStream ms,
            int timeoutSeconds = 30,
            bool throwOnError = true)
        {
            string execPath = GetPdfToTextExeLocation();
            string content = string.Empty;

            string path = Path.Combine(DirectoryHelper.GetTempPath(), DirectoryHelper.GetTempFile() + ".pdf");
            File.WriteAllBytes(path, ms.ToArray());

            try
            {
                content = ProcessHelper.ExecuteProcessAndReadStdOut(execPath, out string _, "\"" + path + "\" -", "", timeoutSeconds, throwOnError);

                try
                {
                    File.Delete(path);
                }
                catch { }

                if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(execPath)
                    || content == "\f\r\n\r\n\r\n"
                    || content == "\f\r\n\r\n"
                    || content == "\r\n\f\r\n\r\n")
                {

                    if (throwOnError)
                    {
                        throw new ApplicationException("Unable to parse PDF. Content blank. You could attempt to use the Google API? JBToolkit.Google.Vision.GetTextFromPDF");
                    }

                    return content;
                }

                return content;
            }
            catch (Exception e)
            {
                try
                {
                    File.Delete(path);
                }
                catch { }

                if (throwOnError)
                {
                    throw new ApplicationException("Unable to parse PDF: " + e.Message + ". You could attempt to use the Google API? JBToolkit.Google.Vision.GetTextFromPDF");
                }

                return content;
            }
        }

        /// <summary>
        /// From resource embedded in JBToolkit.dll
        /// </summary>
        private static string GetPdfToTextExeLocation()
        {
            return AssemblyHelper.EmbeddedResourceHelper.InternalGetEmbeddedResourcePathFromJBToolkit("pdftotext.exe", true);
        }
    }
}
