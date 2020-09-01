using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

namespace JBToolkit.PdfDoc
{
    /*
      EXAMPLE USES

        // From Website / URL

        PdfConvert.SaveHtmlToPdf(new PdfDocument { Url = "https://www.google.co.uk" }, new PdfOutput
        {
            OutputFilePath = "codaxy.pdf"
        });

        PdfConvert.SaveHtmlToPdf(new PdfDocument
        {
            Url = "https://www.google.co.uk",
            HeaderLeft = "[title]",
            HeaderRight = "[date] [time]",
            FooterCenter = "Page [page] of [topage]",
            FooterFontSize = "10",
            HeaderFontSize = "20",
            HeaderFontName = "Comic Sans MS",
            FooterFontName = "Helvetica"

        }, new PdfOutput
        {
            OutputFilePath = "codaxy_hf.pdf"
        });

        // From HTML

        PdfConvert.SaveHtmlToPdf("<html><h1>test</h1></html>", @"C:\Temp\Test.pdf");

        // Memory Stream
        PdfConvert.ConvertHtmlToPdf("<html><h1>test</h1></html>");

        PdfConvert.SaveHtmlToPdf(
            new PdfDocument { Html = "<html><h1>test</h1></html>" },
            new PdfOutput { OutputFilePath = "inline.pdf" }
        );

        PdfConvert.SaveHtmlToPdf(
            new PdfDocument { Html = File.ReadAllText(path) },
            new PdfOutput { OutputFilePath = "inline.pdf" }
        );
      
     */

    /// <summary>
    /// Methods to convert HTML / Web page / Word Docx to PDF - Output as file or memory stream. Uses wkhtmltopdf.exe as embedded resource for HTML or M-Files API for Word docx
    /// </summary>
    public partial class PdfConverter
    {
        static PdfConvertEnvironment _e;

        public static PdfConvertEnvironment PdfConvertEnvironment
        {
            get
            {
                if (_e == null)
                {
                    _e = new PdfConvertEnvironment
                    {
                        TempFolderPath = Path.GetTempPath(),
                        WkHtmlToPdfPath = GetWkhtmlToPdfExeLocation(),
                        Timeout = 60000
                    };
                }

                return _e;
            }
        }

        private static string GetWkhtmlToPdfExeLocation()
        {
            return AssemblyHelper.EmbeddedResourceHelper.InternalGetEmbeddedResourcePathFromJBToolkit("wkhtmltopdf.exe", true);
        }

        /// <summary>
        ///  Convert HTML text to PDF
        /// </summary>
        /// <param name="html">Input HTML Text</param>
        /// <returns>PDF File memory stream</returns>
        public static MemoryStream ConvertHtmlToPdf(string html)
        {
            MemoryStream ms = new MemoryStream();
            PdfOutput output =
                 SaveHtmlToPdf(
                     new PdfConvertDocument
                     {
                         Html = html
                     },
                     null,
                     new PdfOutput { OutputStream = ms }
                 );

            return output.OutputStream;
        }

        /// <summary>
        /// Converts HTML text to PDF
        /// </summary>
        /// <param name="html">Input HTML text</param>
        /// <param name="paperType">Paper type option</param>
        /// <param name="orientation">Orientation option</param>
        /// <returns>PDF File memory stream</returns>
        public static MemoryStream ConvertHtmlToPdf(string html, PaperTypes paperType, Orientation orientation)
        {
            MemoryStream ms = new MemoryStream();
            PdfOutput output =
                 SaveHtmlToPdf(
                     new PdfConvertDocument
                     {
                         Html = html,
                         PaperType = paperType.ToString(),
                         Orientation = orientation.ToString()
                     },
                     null,
                     new PdfOutput { OutputStream = ms }
                 );

            return output.OutputStream;
        }

        /// <summary>
        /// Converts HTML To PDF
        /// </summary>
        /// <param name="document">PDF Object containing layout options</param>
        /// <param name="environment">Application environment options</param>
        /// <returns></returns>
        public static MemoryStream ConvertHtmlToPdf(PdfConvertDocument document)
        {
            MemoryStream ms = new MemoryStream();

            PdfOutput output =
                 SaveHtmlToPdf(
                     document,
                     null,
                     new PdfOutput { OutputStream = ms }
                 );

            return output.OutputStream;
        }

        /// <summary>
        /// Saves HTML as PDF file
        /// </summary>
        /// <param name="html">Input HTML text</param>
        /// <param name="outputPath">Output file path</param>
        public static void SaveHtmlToPdf(string html, string outputPath)
        {
            SaveHtmlToPdf(
                     new PdfConvertDocument
                     {
                         Html = html
                     },
                     new PdfOutput
                     {
                         OutputFilePath = outputPath
                     }
                 );
        }

        /// <summary>
        /// Save HTML as PDF file
        /// </summary>
        /// <param name="html">Input HTML text</param>
        /// <param name="outputPath">OUtput file path</param>
        /// <param name="paperType">Paper type option</param>
        /// <param name="orientation">Orientation option</param>
        public static void SaveHtmlToPdf(string html, string outputPath, PaperTypes paperType, Orientation orientation)
        {
            SaveHtmlToPdf(
                     new PdfConvertDocument
                     {
                         Html = html,
                         PaperType = paperType.ToString(),
                         Orientation = orientation.ToString()
                     },
                     new PdfOutput
                     {
                         OutputFilePath = outputPath
                     }
                 );
        }

        /// <summary>
        /// Save HTML as a PDF file
        /// </summary>
        /// <param name="document">PDF Document object containing layout options</param>
        /// <param name="output">Output file path</param>
        public static void SaveHtmlToPdf(PdfConvertDocument document, PdfOutput output)
        {
            SaveHtmlToPdf(document, null, output);
        }

        /// <summary>
        /// Save HTML as a PDF file
        /// </summary>
        /// <param name="document">A PDF document object containing PDF options</param>
        /// <param name="environment">Applciaton environment option object</param>
        /// <param name="woutput">Output PDF object</param>
        /// <returns></returns>
        public static PdfOutput SaveHtmlToPdf(PdfConvertDocument document, PdfConvertEnvironment environment, PdfOutput woutput)
        {
            if (environment == null)
                environment = PdfConvertEnvironment;

            if (document.Html != null)
                document.Url = "-";

            string outputPdfFilePath;
            bool delete;
            if (woutput.OutputFilePath != null)
            {
                outputPdfFilePath = woutput.OutputFilePath;
                delete = false;
            }
            else
            {
                outputPdfFilePath = Path.Combine(environment.TempFolderPath, string.Format("{0}.pdf", Guid.NewGuid()));
                delete = true;
            }

            if (!File.Exists(environment.WkHtmlToPdfPath))
            {
                environment.WkHtmlToPdfPath = GetWkhtmlToPdfExeLocation();

                if (!File.Exists(environment.WkHtmlToPdfPath))
                    throw new PdfConvertException(string.Format("File '{0}' not found. Check if wkhtmltopdf application is installed.", environment.WkHtmlToPdfPath));
            }

            StringBuilder paramsBuilder = new StringBuilder();

            if (string.IsNullOrEmpty(document.PaperType))
            {
                document.PaperType = PaperTypes.A4.ToString();
            }

            paramsBuilder.AppendFormat("--page-size {0} ", document.PaperType);

            if (string.IsNullOrEmpty(document.Orientation))
            {
                document.Orientation = Orientation.Portrait.ToString();
            }

            paramsBuilder.AppendFormat("--orientation {0} ", document.Orientation);

            if (!string.IsNullOrEmpty(document.HeaderUrl))
            {
                paramsBuilder.AppendFormat("--header-html {0} ", document.HeaderUrl);
                paramsBuilder.Append("--margin-top 25 ");
                paramsBuilder.Append("--header-spacing 5 ");
            }
            if (!string.IsNullOrEmpty(document.FooterUrl))
            {
                paramsBuilder.AppendFormat("--footer-html {0} ", document.FooterUrl);
                paramsBuilder.Append("--margin-bottom 25 ");
                paramsBuilder.Append("--footer-spacing 5 ");
            }
            if (!string.IsNullOrEmpty(document.HeaderLeft))
            {
                paramsBuilder.AppendFormat("--header-left \"{0}\" ", document.HeaderLeft);
            }

            if (!string.IsNullOrEmpty(document.HeaderCenter))
            {
                paramsBuilder.AppendFormat("--header-center \"{0}\" ", document.HeaderCenter);
            }

            if (!string.IsNullOrEmpty(document.HeaderRight))
            {
                paramsBuilder.AppendFormat("--header-right \"{0}\" ", document.HeaderRight);
            }

            if (!string.IsNullOrEmpty(document.FooterLeft))
            {
                paramsBuilder.AppendFormat("--footer-left \"{0}\" ", document.FooterLeft);
            }

            if (!string.IsNullOrEmpty(document.FooterCenter))
            {
                paramsBuilder.AppendFormat("--footer-center \"{0}\" ", document.FooterCenter);
            }

            if (!string.IsNullOrEmpty(document.FooterRight))
            {
                paramsBuilder.AppendFormat("--footer-right \"{0}\" ", document.FooterRight);
            }

            if (!string.IsNullOrEmpty(document.HeaderFontSize))
            {
                paramsBuilder.AppendFormat("--header-font-size \"{0}\" ", document.HeaderFontSize);
            }

            if (!string.IsNullOrEmpty(document.FooterFontSize))
            {
                paramsBuilder.AppendFormat("--footer-font-size \"{0}\" ", document.FooterFontSize);
            }

            if (!string.IsNullOrEmpty(document.HeaderFontName))
            {
                paramsBuilder.AppendFormat("--header-font-name \"{0}\" ", document.HeaderFontName);
            }

            if (!string.IsNullOrEmpty(document.FooterFontName))
            {
                paramsBuilder.AppendFormat("--footer-font-name \"{0}\" ", document.FooterFontName);
            }

            if (document.ExtraParams != null)
            {
                foreach (var extraParam in document.ExtraParams)
                {
                    paramsBuilder.AppendFormat("--{0} {1} ", extraParam.Key, extraParam.Value);
                }
            }

            if (document.Cookies != null)
            {
                foreach (var cookie in document.Cookies)
                {
                    paramsBuilder.AppendFormat("--cookie {0} {1} ", cookie.Key, cookie.Value);
                }
            }

            paramsBuilder.AppendFormat("\"{0}\" \"{1}\"", document.Url, outputPdfFilePath);

            try
            {
                var output = new StringBuilder();
                var error = new StringBuilder();

                using (Process process = new Process())
                {
                    process.StartInfo.FileName = environment.WkHtmlToPdfPath;
                    process.StartInfo.Arguments = paramsBuilder.ToString();
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.RedirectStandardInput = true;

                    using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                    using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                    {
                        void outputHandler(object sender, DataReceivedEventArgs e)
                        {
                            if (e.Data == null)
                            {
                                outputWaitHandle.Set();
                            }
                            else
                            {
                                output.AppendLine(e.Data);
                            }
                        }

                        void errorHandler(object sender, DataReceivedEventArgs e)
                        {
                            if (e.Data == null)
                            {
                                errorWaitHandle.Set();
                            }
                            else
                            {
                                error.AppendLine(e.Data);
                            }
                        }

                        process.OutputDataReceived += outputHandler;
                        process.ErrorDataReceived += errorHandler;

                        try
                        {
                            process.Start();

                            process.BeginOutputReadLine();
                            process.BeginErrorReadLine();

                            if (document.Html != null)
                            {
                                using (var stream = process.StandardInput)
                                {
                                    byte[] buffer = Encoding.UTF8.GetBytes(document.Html);
                                    stream.BaseStream.Write(buffer, 0, buffer.Length);
                                    stream.WriteLine();
                                }
                            }

                            if (process.WaitForExit(environment.Timeout) && outputWaitHandle.WaitOne(environment.Timeout) && errorWaitHandle.WaitOne(environment.Timeout))
                            {
                                if (process.ExitCode != 0 && !File.Exists(outputPdfFilePath))
                                {
                                    throw new PdfConvertException(string.Format("Html to PDF conversion of '{0}' failed. Wkhtmltopdf output: \r\n{1}", document.Url, error));
                                }
                            }
                            else
                            {
                                if (!process.HasExited)
                                {
                                    process.Kill();
                                }

                                throw new PdfConvertTimeoutException();
                            }
                        }
                        finally
                        {
                            process.OutputDataReceived -= outputHandler;
                            process.ErrorDataReceived -= errorHandler;
                        }
                    }
                }

                if (woutput.OutputStream != null)
                {
                    using (Stream fs = new FileStream(outputPdfFilePath, FileMode.Open))
                    {
                        byte[] buffer = new byte[32 * 1024];
                        int read;

                        while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                            woutput.OutputStream.Write(buffer, 0, read);
                    }
                }

                if (woutput.OutputCallback != null)
                {
                    byte[] pdfFileBytes = File.ReadAllBytes(outputPdfFilePath);
                    woutput.OutputCallback(document, pdfFileBytes);
                }
            }
            finally
            {
                try
                {
                    if (delete && File.Exists(outputPdfFilePath))
                    {
                        File.Delete(outputPdfFilePath);
                    }
                }
                catch { }
            }

            return woutput;
        }

        internal static void ConvertHtmlToPdf(string url, string outputFilePath)
        {
            SaveHtmlToPdf(new PdfConvertDocument { Url = url }, new PdfOutput { OutputFilePath = outputFilePath });
        }
    }

    public enum PaperTypes
    {
        A0,
        A1,
        A2,
        A3,
        A4,
        A5,
        A,
        A7,
        A8,
        A9,
        B0,
        B1,
        B10,
        B2,
        B3,
        B4,
        B5,
        B6,
        B7,
        B8,
        B9,
        C5E,
        Comm10E,
        DLE,
        Executive,
        Folio,
        Ledger,
        Legal,
        Letter,
        Tabloid
    }

    public enum Orientation
    {
        Landscape,
        Portrait
    }

    [Serializable]
    public class PdfConvertException : Exception
    {
        public PdfConvertException() { }

        public PdfConvertException(string msg) : base(msg) { }

        protected PdfConvertException(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class PdfConvertTimeoutException : PdfConvertException
    {
        public PdfConvertTimeoutException() : base("HTML to PDF conversion process has not finished in the given period.") { }

        protected PdfConvertTimeoutException(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }

    public class PdfOutput
    {
        public string OutputFilePath { get; set; }
        public MemoryStream OutputStream { get; set; }
        public Action<PdfConvertDocument, byte[]> OutputCallback { get; set; }
    }

    public class PdfConvertDocument
    {
        public string PaperType { get; set; }
        public string Orientation { get; set; }
        public string Url { get; set; }
        public string Html { get; set; }
        public string HeaderUrl { get; set; }
        public string FooterUrl { get; set; }
        public string HeaderLeft { get; set; }
        public string HeaderCenter { get; set; }
        public string HeaderRight { get; set; }
        public string FooterLeft { get; set; }
        public string FooterCenter { get; set; }
        public string FooterRight { get; set; }
        public object State { get; set; }
        public Dictionary<string, string> Cookies { get; set; }
        public Dictionary<string, string> ExtraParams { get; set; }
        public string HeaderFontSize { get; set; }
        public string FooterFontSize { get; set; }
        public string HeaderFontName { get; set; }
        public string FooterFontName { get; set; }
    }

    public class PdfConvertEnvironment
    {
        public string TempFolderPath { get; set; }
        public string WkHtmlToPdfPath { get; set; }
        public int Timeout { get; set; }
        public bool Debug { get; set; }
    }
}
