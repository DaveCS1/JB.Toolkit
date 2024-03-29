﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace JBToolkit.XmlDoc.Converters
{
    /// <summary>
    /// Uses a decent 3rd party command line tool to perform a bulk 'Document to PDF Conversion' (quickly - multi-threaded)
    /// Will convert typical MS Office documents (docx, xlsx, msg, pptx, vsd, pub and more), images, html and text files it encounters to PDF.
    /// </summary>
    public static partial class OfficeHtmlPdfImageConverter
    {
        private static string __error = string.Empty;

        /// <summary>
        /// Uses a decent 3rd party command line tool to perform a bulk 'Document to PDF Conversion' (quickly - multi-threaded)
        /// Will convert typical MS Office documents (docx, xlsx, msg, pptx, vsd, pub and more), images, html and text files it encounters to PDF.
        ///    
        /// With a given input directory is will scan files recursively (including subfolders) and output
        /// to a given output directory (it will also mimick the file structure of the root input directory given if subfolder are present).
        /// </summary>
        public static void BulkConvertDocumentsToPdf(string inputDirectory, string outputDirectory)
        {
            RunCommand(inputDirectory, outputDirectory);
        }

        private static StringBuilder __outputStringBuilder = new StringBuilder();
        private static void RunCommand(string inputDirectory, string outputDirectory)
        {
            int iterations = 0;
            string errorMessage = string.Empty;
            try
            {
                RunCommandActual(
                    inputDirectory,
                    outputDirectory);
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
                        inputDirectory,
                        outputDirectory);

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

        private static void RunCommandActual(string inputDirectory, string outputDirectory)
        {
            __outputStringBuilder = new StringBuilder();
            var process = new Process();

            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            try
            {
                process.StartInfo.FileName = AssemblyHelper.EmbeddedResourceHelper
                                                .GetEmbeddedResourcePath(Assembly.GetExecutingAssembly(),
                                                                         "ReAlPDFc.exe",
                                                                         "Dependencies_Embedded",
                                                                         true);

                process.StartInfo.Arguments = string.Format(@"""{0}"" ""{1}""", inputDirectory, outputDirectory);
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(process.StartInfo.FileName);
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.EnableRaisingEvents = false;
                process.OutputDataReceived += Process__OutputDataReceived;
                process.ErrorDataReceived += Process__ErrorDataReceived;
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                if (process.ExitCode != 0)
                {
                    var output = __outputStringBuilder.ToString();

                    throw new Exception("ReAlPDFc process exited with non-zero exit code of: " + process.ExitCode + Environment.NewLine +
                    "Output from process: " + __outputStringBuilder.ToString());
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

                if (!string.IsNullOrEmpty(__error))
                {
                    throw new ApplicationException(__error);
                }
            }
        }

        private static void Process__OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            __outputStringBuilder.Append(e.Data);
        }

        private static void Process__ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                __error = "ReAlPDFc Error: " + e.Data;
            }
        }
    }
}
