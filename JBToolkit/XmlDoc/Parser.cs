using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace JBToolkit.XmlDoc
{
    /// <summary>
    /// Parses and returns the text an MS Office document (docx, xlsx, msg, eml, pptx, vsdx, pub), PDF, or Image (using OCR)
    /// </summary>
    public class Parser
    {
        private static StringBuilder _outputStringBuilder = new StringBuilder();

        /// <summary>
        /// Parses and returns the text an MS Office document (docx, xlsx, msg, eml, pptx, vsdx, pub), PDF, or Image (using OCR)
        /// </summary>
        /// <param name="inputPath">Document input path</param>
        /// <param name="tryKeepTextPosition">Converts to a PDF memory stream first then extracts text. 
        /// The PDF text extractor is better at maintaining text, paragraph and formatting locations (slow)</param>
        /// <param name="timeoutSeconds">Time in seconds before throwing timeout exception</param>
        /// <returns>Text string</returns>
        public static string GetTextFromDocument(string inputPath, bool tryKeepTextPosition = false, int timeoutSeconds = 60)
        {
            int iterations = 0;
            string errorMessage;
            try
            {
                return GetTextFromDocumentActual(
                    inputPath,
                    tryKeepTextPosition,
                    timeoutSeconds);
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
                    return GetTextFromDocumentActual(
                        inputPath,
                        tryKeepTextPosition,
                        timeoutSeconds);
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

            throw new Exception(errorMessage);
        }

        private static string GetTextFromDocumentActual(string inputPath, bool tryKeepTextPosition = false, int timeoutSeconds = 60)
        {
            _outputStringBuilder = new StringBuilder();
            Process process = new Process();

            int timeoutMs = timeoutSeconds * 1000;

            try
            {
                process.StartInfo.FileName = AssemblyHelper.EmbeddedResourceHelper
                                                .GetEmbeddedResourcePath(Assembly.GetExecutingAssembly(),
                                                                         "ReAlPDFc.exe",
                                                                         "Dependencies_Embedded",
                                                                         true);

                process.StartInfo.Arguments = string.Format(@"""{0}"" " + (tryKeepTextPosition ? "-ptxt" : "-txt"), inputPath);
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
            }

            return _outputStringBuilder.ToString();
        }


        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _outputStringBuilder.Append(e.Data);
        }

        private static void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                throw new ApplicationException("ReAlPDFc Error: " + e.Data);
            }
        }
    }
}
