using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace JBToolkit.XmlDoc.Converters
{
    /// <summary>
    /// Uses a decent 3rd party command line tool to perform MS Office / PDF / Html conversions.
    /// 
    /// It can convert Word, Excel, PowerPoint, Visio, Outlook, Image or Text to PDF and back again (PDF to Office) or to another Office format (e.g. Word to PowerPoint)
    /// or to HTML, text or image (jpg, jpeg, png, gif, tiff, tif, svg, bmp) files, and from one type of image (any image) to a standard image (jpg, jpeg, png, gif, tiff, tif, svg, bmp).
    /// It can also convert any image to text using OCR character recognition (use additional argument: -orcquick to perform a quick, less thorough OCR scan).
    ///    
    /// It choses the correct conversion mechanism from the file extension. Exports to new file.
    ///    
    /// </summary>
    public static partial class OfficeHtmlPdfImageConverter
    {
        private static string _error = string.Empty;

        /// <summary>
        /// Converts MS Office document (xlsx, doc, ppt etc) to memory stream (i.e. for use in Web Requests)
        /// </summary>
        /// <param name="docxInputPath">File path</param>
        /// <returns>Memory stream</returns>
        public static MemoryStream ConvertToPdf(string inputPath)
        {
            return PdfDoc.PdfConverter.ConvertToPdf(inputPath);
        }

        /// <summary>
        /// Converts MS Office document (xlsx, doc, ppt etc) to memory stream (i.e. for use in Web Requests)
        /// </summary>
        /// <param name="docxInputPath">File path</param>
        /// <returns>Memory stream</returns>
        public static MemoryStream ConvertToPdf(MemoryStream ms, string fileExtension)
        {
            return PdfDoc.PdfConverter.ConvertToPdf(ms, fileExtension);
        }

        /// <summary>
        /// Save a MS Office doc as a PDF
        /// </summary>
        /// <param name="inputPath">.docx path</param>
        /// <param name="pdfOutputPath">Save PDF file path</param>
        public static void ConvertToPdf(string inputPath, string pdfOutputPath)
        {
            PdfDoc.PdfConverter.ConvertToPdf(inputPath, pdfOutputPath);
        }

        /// <summary>
        /// Uses a decent 3rd party command line tool to perform MS Office / PDF / Html conversions.
        /// 
        /// It can convert Word, Excel, PowerPoint, Visio, Outlook, Image or Text to PDF and back again (PDF to Office) or to another Office format (e.g. Word to PowerPoint)
        /// or to HTML, text or image (jpg, jpeg, png, gif, tiff, tif, svg, bmp) files, and from one type of image (any image) to a standard image (jpg, jpeg, png, gif, tiff, tif, svg, bmp).
        /// It can also convert any image to text using OCR character recognition (use additional argument: -orcquick to perform a quick, less thorough OCR scan).
        ///    
        /// It choses the correct conversion mechanism from the file extension. Exports to new file.
        ///     
        /// It choses the correct conversion mechanism from the file extension. Exports to new file.
        /// </summary>
        /// <param name="inputPath">Document to convert</param>
        /// <param name="outputPath">Converted document (with specifif file extension)</param>
        /// <param name="pages">Pages string (optional). I.e. 1-3,5,7-9 or even sheet names [sheet1],[sheet3] for Excel</param>
        /// <param name="additionalOptions">ReAlPDFc Additional ar guments</param>
        /// <param name="timeoutSeconds">Amount of time before throwing an exception. Default is 60 seconds</param>
        public static void ConvertDocumentOrImage(
            string inputPath,
            string outputPath,
            string pages = null,
            AdditionalOptions additionalOptions = null,
            int timeoutSeconds = 60,
            bool skipCltHashCheck = true)
        {
            RunCommand(
                inputPath,
                outputPath,
                false, pages,
                additionalOptions,
                timeoutSeconds,
                skipCltHashCheck);
        }


        /// <summary>
        /// Uses ReAlPDFc to perform MS Office / PDF / Html conversions.
        /// 
        /// It can convert Word, Excel, PowerPoint, Visio, Outlook, Image or Text to PDF and back again (PDF to Office) or to another Office format (e.g. Word to PowerPoint)
        /// or to HTML, text or image (jpg, jpeg, png, gif, tiff, tif, svg, bmp) files, and from one type of image (any image) to a standard image (jpg, jpeg, png, gif, tiff, tif, svg, bmp).
        /// 
        /// - Multi-threaded
        ///    
        /// It choses the correct conversion mechanism from the file extension. Exports to new file.
        /// </summary>
        /// <param name="inputPaths">Array of input paths to documents you wish to convert</param>
        /// <param name="toExtension">The resulting filetype extension you wish to convert to</param>
        /// <param name="outputDirectory">Path to directory where files will be outputted to</param>
        /// <param name="additionalOptions">ReAlPDFc Additional ar guments</param>
        /// <param name="overwriteExisting">Overwrite any existing files on name conflicts if already present in the output directory</param>
        public static void ConvertDocumentsOrImages(
            string[] inputPaths,
            string toExtension,
            string outputDirectory,
            AdditionalOptions additionalOptions = null,
            bool overwriteExisting = false,
            bool skipCltHashCheck = true)
        {
            RunCommand(
                inputPaths,
                outputDirectory,
                toExtension,
                additionalOptions,
                overwriteExisting,
                skipCltHashCheck);
        }

        /// <summary>
        /// Uses a decent 3rd party command line tool to perform MS Office / PDF / Html conversions. 
        /// It can convert Word, Excel, PowerPoint, Visio, Outlook, Image or Text to PDF and back again (PDF to Office) or to another Office format (i.e. Word to PowerPoint)
        /// 
        /// Note: you can't convert to HTML or from MS Office / PDF to image with this method, however you can use this method to convert from image to another image or html to an image.
        /// It can also convert any image to text using OCR character recognition (use additional argument: -orcquick to perform a quick, less thorough OCR scan).
        ///    
        /// It choses the correct conversion mechanism from the file extension. Exports to new file.
        /// </summary>
        /// <param name="inputPath">Input paths to document you wish to convert</param>
        /// <param name="toExtension">The resulting filetype extension you wish to convert to</param>
        /// <param name="pages">Pages string (optional). I.e. 1-3,5,7-9 or even sheet names [sheet1],[sheet3] for Excel</param>
        /// <param name="additionalOptions">ReAlPDFc Additional ar guments</param>
        /// <param name="timeoutSeconds">Amount of time before throwing an exception. Default is 60 seconds</param>
        public static byte[] ConvertDocumentOrImageAndReturnByteArray(
            string inputPath,
            string extension,
            string pages = null,
            AdditionalOptions additionalOptions = null,
            int timeoutSeconds = 60,
            bool skipCltHashCheck = true)
        {
            return Convert.FromBase64String(
                            RunCommand(
                                inputPath,
                                "dummy." + extension.ToLower().Replace(".", ""),
                                true,
                                pages,
                                additionalOptions,
                                timeoutSeconds,
                                skipCltHashCheck));
        }

        /// <summary>
        /// Uses a decent 3rd party command line tool to perform MS Office / PDF / Html conversions.        
        /// It can convert Word, Excel, PowerPoint, Visio, Outlook, Image or Text to PDF and back again (PDF to Office) or to another Office format (i.e. Word to PowerPoint)
        /// It can also convert any image to text using OCR character recognition (use additional argument: -orcquick to perform a quick, less thorough OCR scan).
        /// 
        /// Note: you can't convert to HTML or from MS Office / PDF to image with this method, however you can use this method to convert from image to another image or html to an image.
        ///    
        /// It choses the correct conversion mechanism from the file extension. Exports to new file
        /// </summary>
        /// <param name="inputPath">Input paths to document you wish to convert</param>
        /// <param name="toExtension">The resulting filetype extension you wish to convert to</param>
        /// <param name="pages">Pages string (optional). I.e. 1-3,5,7-9 or even sheet names [sheet1],[sheet3] for Excel</param>
        /// <param name="additionalOptions">ReAlPDFc Additional ar guments</param>
        /// <param name="timeoutSeconds">Amount of time before throwing an exception. Default is 60 seconds</param>
        public static MemoryStream ConvertDocumentOrImageAndReturnMemoryStream(
            string inputPath,
            string extension,
            string pages = null,
            AdditionalOptions additionalOptions = null,
            int timeoutSeconds = 60,
            bool skipCltHashCheck = true)
        {
            return new MemoryStream(
                Convert.FromBase64String(
                    RunCommand(
                        inputPath,
                        "dummy." + extension.ToLower().Replace(".", ""),
                        true,
                        pages,
                        additionalOptions,
                        timeoutSeconds,
                        skipCltHashCheck)));
        }

        /// <summary>
        /// Uses a decent 3rd party command line tool to perform MS Office / PDF / Html conversions.        
        /// It can convert Word, Excel, PowerPoint, Visio, Outlook, Image or Text to PDF and back again (PDF to Office) or to another Office format (i.e. Word to PowerPoint)
        /// It can also convert any image to text using OCR character recognition (use additional argument: -orcquick to perform a quick, less thorough OCR scan).
        /// 
        /// Note: you can't convert to HTML or from MS Office / PDF to image with this method, however you can use this method to convert from image to another image or html to an image.
        ///    
        /// It choses the correct conversion mechanism from the file extension. Exports to new file
        /// </summary>
        /// <param name="inputPath">Input paths to document you wish to convert</param>
        /// <param name="toExtension">The resulting filetype extension you wish to convert to</param>
        /// <param name="pages">Pages string (optional). I.e. 1-3,5,7-9 or even sheet names [sheet1],[sheet3] for Excel</param>
        /// <param name="additionalOptions">ReAlPDFc Additional ar guments</param>
        /// <param name="timeoutSeconds">Amount of time before throwing an exception. Default is 60 seconds</param>
        public static string ConvertDocumentOrImageAndReturnBase64String(
            string inputPath,
            string extension,
            string pages = null,
            AdditionalOptions additionalOptions = null,
            int timeoutSeconds = 60,
            bool skipCltHashCheck = true)
        {
            return RunCommand(
                inputPath,
                "dummy." + extension.ToLower().Replace(".", ""),
                true,
                pages,
                additionalOptions,
                timeoutSeconds,
                skipCltHashCheck);
        }

        private static StringBuilder _outputStringBuilder = new StringBuilder();
        private static string RunCommand(
            string inputPath,
            string outputPath,
            bool aBase64,
            string pages = null,
            AdditionalOptions additionalOptions = null,
            int timeoutSeconds = 60,
            bool skipCltHashCheck = true)
        {
            int iterations = 0;
            string errorMessage;
            try
            {
                return RunCommandActual(
                    inputPath,
                    outputPath,
                    aBase64,
                    pages,
                    additionalOptions,
                    timeoutSeconds,
                    skipCltHashCheck);
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }

            while (errorMessage.Contains("ReAlPDFc.exe.tmp' already exists.") ||
                   errorMessage.Contains("Access to the path is denied") ||
                   (errorMessage.Contains("The process cannot access the file") &&
                       errorMessage.Contains("ReAlPDFc.exe.tmp'")))
            {
                try
                {
                    return RunCommandActual(
                       inputPath,
                       outputPath,
                       aBase64,
                       pages,
                       additionalOptions,
                       timeoutSeconds,
                       skipCltHashCheck);
                }
                catch (Exception e)
                {
                    if (iterations > 120) // 30 seconds
                        throw new Exception(errorMessage);

                    errorMessage = e.Message;
                    Thread.Sleep(250);

                    iterations++;
                }
            }

            throw new Exception("ReAlPDFc execution error: " + errorMessage);
        }

        private static string RunCommandActual(
            string inputPath,
            string outputPath,
            bool aBase64,
            string pages = null,
            AdditionalOptions additionalOptions = null,
            int timeoutSeconds = 60,
            bool skipCltHashCheck = true)
        {
            _outputStringBuilder = new StringBuilder();
            Process process = new Process();

            int timeoutMs = timeoutSeconds * 1000;

            string directoryName = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directoryName))
            {
                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);
            }

            try
            {
                process.StartInfo.FileName = AssemblyHelper.EmbeddedResourceHelper
                                                .GetEmbeddedResourcePath(Assembly.GetExecutingAssembly(),
                                                                         "ReAlPDFc.exe",
                                                                         "Dependencies_Embedded",
                                                                         skipCltHashCheck);

                process.StartInfo.Arguments = string.Format(@"""{0}"" ""{1}"" {2}{3}{4}",
                                                    inputPath,
                                                    outputPath,
                                                    aBase64 ? "-b64" : "",
                                                    pages != null ? " \"-p=" + pages + "\"" : "",
                                                    additionalOptions == null ? "" : (string.IsNullOrEmpty(additionalOptions.ToString()) ? "" : " " + additionalOptions.ToString()));

                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(process.StartInfo.FileName);
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.EnableRaisingEvents = false;
                process.OutputDataReceived += Process_OutputDataReceived;
                process.ErrorDataReceived += Process_ErrorDataReceived;
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                var processExited = process.WaitForExit(timeoutMs);

                if (processExited == false) // we timed out...
                {
                    process.Kill();
                    throw new Exception("ERROR: ReAlPDFc Process took too long to finish");
                }
                else if (process.ExitCode != 0)
                {
                    var output = _outputStringBuilder.ToString();

                    throw new Exception("ReAlPDFc process exited with non-zero exit code of: " + process.ExitCode + Environment.NewLine +
                    "Output from process: " + _outputStringBuilder.ToString());
                }
            }
            catch (Exception e)
            {
                throw new Exception("ReAlPDFc execution error: " + e.Message);
            }
            finally
            {
                process.Close();

                try
                {
                    process.Kill();
                }
                catch { }

                if (!string.IsNullOrEmpty(_error))
                    throw new ApplicationException(_error);
            }

            return _outputStringBuilder.ToString();
        }

        private static void RunCommand(
            string[] inputPaths,
            string toExtension,
            string outputDirectory = null,
            AdditionalOptions additionalOptions = null,
            bool overwriteExisting = false,
            bool skipCltHashCheck = true)
        {
            int iterations = 0;
            string errorMessage = string.Empty;
            try
            {
                RunCommandActual(
                    inputPaths,
                    toExtension,
                    outputDirectory,
                    additionalOptions,
                    overwriteExisting,
                    skipCltHashCheck);
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }

            while (errorMessage.Contains("ReAlPDFc.exe.tmp' already exists.") ||
                   errorMessage.Contains("Access to the path is denied") ||
                   (errorMessage.Contains("The process cannot access the file") &&
                       errorMessage.Contains("ReAlPDFc.exe.tmp'")))
            {
                try
                {
                    RunCommandActual(
                        inputPaths,
                        toExtension,
                        outputDirectory,
                        additionalOptions,
                        overwriteExisting,
                        skipCltHashCheck);

                    errorMessage = string.Empty;
                }
                catch (Exception e)
                {
                    if (iterations > 120) // 30 seconds
                        throw new Exception(errorMessage);

                    errorMessage = e.Message;
                    Thread.Sleep(250);

                    iterations++;
                }
            }
        }

        private static void RunCommandActual(
            string[] inputPaths,
            string toExtension,
            string outputDirectory = null,
            AdditionalOptions additionalOptions = null,
            bool overwriteExisting = false,
            bool skipCltHashCheck = true)
        {
            _outputStringBuilder = new StringBuilder();
            Process process = new Process();

            if (!string.IsNullOrEmpty(outputDirectory))
                if (!Directory.Exists(outputDirectory))
                    Directory.CreateDirectory(outputDirectory);

            try
            {
                process.StartInfo.FileName = AssemblyHelper.EmbeddedResourceHelper
                                                .GetEmbeddedResourcePath(Assembly.GetExecutingAssembly(),
                                                                         "ReAlPDFc.exe",
                                                                         "Dependencies_Embedded",
                                                                         skipCltHashCheck);

                process.StartInfo.Arguments = string.Format(@"-m -ext={0} {1}{2}{3}{4}",
                                                        toExtension,
                                                        AdjustInputPaths(inputPaths),
                                                        string.IsNullOrEmpty(outputDirectory) ? "" : " \"" + outputDirectory + "\"",
                                                        additionalOptions == null ? "" : (string.IsNullOrEmpty(additionalOptions.ToString()) ? "" : " " + additionalOptions.ToString()),
                                                        overwriteExisting ? " -overwrite" : "");

                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.EnableRaisingEvents = false;
                process.OutputDataReceived += Process_OutputDataReceived;
                process.ErrorDataReceived += Process_ErrorDataReceived;
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    var output = _outputStringBuilder.ToString();

                    throw new Exception("ReAlPDFc process exited with non-zero exit code of: " + process.ExitCode + Environment.NewLine +
                    "Output from process: " + _outputStringBuilder.ToString());
                }
            }
            catch (Exception e)
            {
                throw new Exception("ReAlPDFc execution error: " + e.Message);
            }
            finally
            {
                process.Close();
                try
                {
                    process.Kill();
                }
                catch { }
            }
        }

        private static string AdjustInputPaths(string[] inputPaths)
        {
            // Command line arguments have a max lenghth...
            // If above recommended 6000 characters, write arguments to file and input that
            // to ReAlPDFc instead.

            string paths = string.Join(" ", inputPaths);

            if (paths.Length < 8000)
            {
                for (int i = 0; i < inputPaths.Length; i++)
                    inputPaths[i] = string.Format("\"{0}\"", inputPaths[i]);

                return string.Join(" ", inputPaths);
            }

            else
            {
                string tempFilename = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + ".ReAlPaths");
                File.WriteAllText(tempFilename, string.Join("*", inputPaths));

                return "\"" + tempFilename + "\"";
            }
        }

        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _outputStringBuilder.Append(e.Data);
        }

        private static void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Data) && !e.Data.Contains("diacritics"))
            {
                _error = "ReAlPDFc Error: " + e.Data;
            }
        }

        /// <summary>
        /// Additional input options
        /// </summary>
        public class AdditionalOptions
        {
            /// <summary>
            /// Image quality. 0 - 100
            /// </summary>
            public int? Quality { get; set; } = null;

            /// <summary>
            /// Image conversion. I.e. jpg to png
            /// </summary>
            public bool? RemoveWhiteBackground { get; set; } = null;

            /// <summary>
            /// Try keep text formatting (when converting to txt). Default = true
            /// </summary>
            public bool? TryKeepTextFormatting { get; set; } = null;

            /// <summary>
            /// Any conversion. Default = false;
            /// </summary>
            public bool? Overwrite { get; set; } = null;

            /// <summary>
            /// Document conversion. Default = * (all)
            /// Convert only specifc pages, i.e. "1-3,7,9,11-13" or even sheet names for Exec as "[sheet1],[sheet3]"
            /// </summary>
            public string Pages { get; set; } = null;

            public override string ToString()
            {
                var sb = new StringBuilder();

                if (Quality != null)
                    sb.Append("-q" + Quality + " ");

                if (RemoveWhiteBackground != null)
                    sb.Append("-rwhite ");

                if (Overwrite != null)
                    sb.Append("-overwrite ");

                if (TryKeepTextFormatting != null)
                    sb.Append("-ptxt ");

                if (!string.IsNullOrEmpty(Pages))
                    sb.Append("\"-p=" + Pages + "\" ");

                return sb.ToString();
            }
        }
    }
}
