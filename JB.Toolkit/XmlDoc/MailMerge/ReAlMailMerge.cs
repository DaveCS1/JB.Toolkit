using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace JBToolkit.XmlDoc.MailMerge
{
    /// <summary>
    /// Mail merging using a 3rd party command line tool.
    /// 
    /// It's generally quick and reliable, however it must load the proces which then loads its assemblies. An implementation using .Net managed code
    /// libraries would be more effecient. Especially when performing mail merges in bulk.
    /// </summary>
    public class ReAlMailMerge
    {
        private static string _error = string.Empty;

        /// <summary>
        /// Perform a Mail Merge and export to file
        /// </summary>
        public static void PerformMailMerge(
            string inputPath,
            string outputPath,
            DataTable data,
            int timeoutSeconds = 60,
            bool overwriteExisting = true)
        {
            string tempFile = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + ".csv");
            data.ToCsvFile(tempFile, ',', true, true);

            RunCommand(
                inputPath,
                outputPath,
                tempFile,
                false,
                timeoutSeconds,
                overwriteExisting);

            try
            {
                File.Delete(tempFile);
            }
            catch { }
        }

        /// <summary>
        /// Perform a Mail Merge and export as byte array
        /// </summary>
        public static byte[] PerformMailMergeAndReturnByteArray(
            string inputPath,
            string extension,
            DataTable data,
            int timeoutSeconds = 60)
        {
            string tempFile = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + ".csv");
            data.ToCsvFile(tempFile, ',', true, true);

            var result = Convert.FromBase64String(
                                    RunCommand(
                                        inputPath,
                                        "dummy." + extension.ToLower().Replace(".", ""),
                                        tempFile,
                                        true,
                                        timeoutSeconds));

            try
            {
                File.Delete(tempFile);
            }
            catch { }

            return result;
        }

        /// <summary>
        /// Perform a Mail Merge and export as memory stream
        /// </summary>
        public static MemoryStream PerformMailMergeAndReturnMemoryStream(
            string inputPath,
            string extension,
            DataTable data,
            int timeoutSeconds = 60)
        {
            string tempFile = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + ".csv");
            data.ToCsvFile(tempFile, ',', true, true);

            var result = new MemoryStream(
                                Convert.FromBase64String(
                                            RunCommand(
                                                inputPath,
                                                "dummy." + extension.ToLower().Replace(".", ""),
                                                tempFile,
                                                true,
                                                timeoutSeconds)));

            try
            {
                File.Delete(tempFile);
            }
            catch { }

            return result;
        }

        /// <summary>
        /// Perform a Mail Merge and export as base64 string
        /// </summary>
        public static string PerformMailMergeAndReturnBase64String(
            string inputPath,
            string extension,
            DataTable data,
            int timeoutSeconds = 60)
        {
            string tempFile = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + ".csv");
            data.ToCsvFile(tempFile, ',', true, true);

            var result = RunCommand(
                inputPath,
                "dummy." + extension.ToLower().Replace(".", ""),
                tempFile,
                true,
                timeoutSeconds);

            try
            {
                File.Delete(tempFile);
            }
            catch { }

            return result;
        }

        private static StringBuilder _outputStringBuilder = new StringBuilder();
        private static string RunCommand(
            string inputPath,
            string outputPath,
            string csvDataPath,
            bool aBase64,
            int timeoutSeconds = 60,
            bool overwriteExisting = true)
        {
            int iterations = 0;
            string errorMessage;
            try
            {
                return RunCommandActual(
                    inputPath,
                    outputPath,
                    csvDataPath,
                    aBase64,
                    timeoutSeconds,
                    overwriteExisting);
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
                        csvDataPath,
                        aBase64,
                        timeoutSeconds,
                        overwriteExisting);
                }
                catch (Exception e)
                {
                    if (iterations > 120)
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
          string csvDataPath,
          bool aBase64,
          int timeoutSeconds = 60,
          bool overwriteExisting = true)
        {
            _outputStringBuilder = new StringBuilder();
            var process = new Process();

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
                                                                         true);

                process.StartInfo.Arguments = string.Format(@"""{0}"" ""{1}"" ""{2}""{3}{4}",
                                                        inputPath,
                                                        outputPath,
                                                        csvDataPath,
                                                        aBase64 ? " -b64" : "",
                                                        overwriteExisting ? " -overwrite" : "");

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
                {
                    throw new ApplicationException(_error);
                }
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
                _error = "ReAlPDFc Error: " + e.Data;
            }
        }
    }
}
