using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace JBToolkit.XmlDoc
{
    /// <summary>
    /// Uses a command line tool to extract all images found in an MS Office (docx, xlsx, msg, eml, pptx, vsdx, pub), PDF or HTMl document to a new folder
    /// </summary>
    public class ImageExtractor
    {
        /// <summary>
        /// Uses a command line tool to extract all images found in an MS Office (docx, xlsx, msg, eml, pptx, vsdx, pub), PDF or HTMl document to a new folder
        /// </summary> 
        /// <param name="inputPath">Document input path</param>
        /// <summary>
        public static void ExtractImagesFromDocument(string inputPath, string outputDirectory, int timeoutSeconds = 60)
        {
            int iterations = 0;
            string errorMessage = string.Empty;
            try
            {
                ExtractImagesFromDocumentActual(
                    inputPath,
                    outputDirectory,
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
                    ExtractImagesFromDocumentActual(
                        inputPath,
                        outputDirectory,
                        timeoutSeconds);

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

        private static void ExtractImagesFromDocumentActual(string inputPath, string outputDirectory, int timeoutSeconds = 60)
        {
            Process process = new Process();
            int timeoutMs = timeoutSeconds * 1000;

            if (!string.IsNullOrEmpty(outputDirectory))
                if (!Directory.Exists(outputDirectory))
                    Directory.CreateDirectory(outputDirectory);

            try
            {
                process.StartInfo.FileName = AssemblyHelper.EmbeddedResourceHelper
                                                .GetEmbeddedResourcePath(Assembly.GetExecutingAssembly(),
                                                                         "ReAlPDFc.exe",
                                                                         "Dependencies_Embedded",
                                                                         true);

                process.StartInfo.Arguments = string.Format(@"""{0}"" ""{1}"" -img", inputPath, outputDirectory);
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.EnableRaisingEvents = false;
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
                    throw new Exception("ReAlPDFc process exited with non-zero exit code of: " + process.ExitCode);
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
