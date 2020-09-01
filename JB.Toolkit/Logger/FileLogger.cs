using System;
using System.IO;
using System.Reflection;
using System.Web;

namespace JBToolkit.Logger
{
    /// <summary>
    /// Logs errors to files stored in the 'Content' directory - used in the event we're unable to log to the database
    /// </summary>
    public class FileLogger
    {

        /// <summary>
        /// Log error with the exception itself.
        /// </summary>
        /// <param name="outputDirectory">(Optional) Default is bin\logs for an app or class library or /Content/logs for a site</param>
        public static void LogError(Exception ex, string outputDirectory = null)
        {
            try
            {
                string rootPath = string.Empty;

                if (!string.IsNullOrEmpty(outputDirectory))
                {
                    rootPath = outputDirectory;
                }
                else if (HttpContext.Current == null)
                {
                    rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\logs\";
                }
                else
                {
                    rootPath = HttpContext.Current.Server.MapPath("/Content/logs");
                }

                if (!Directory.Exists(rootPath))
                {
                    Directory.CreateDirectory(rootPath);
                }

                string strPath = string.Format("{0}\\log-{1}.txt", rootPath, DateTime.Now.ToString("yyyy-MM-dd"));
                if (!File.Exists(strPath))
                {
                    File.Create(strPath).Dispose();
                }
                using (StreamWriter sw = File.AppendText(strPath))
                {
                    sw.WriteLine(DateTime.Now + "--- Source: " + ex.Source + " -- Message: " + ex.Message + "-- Stack Trace: " + ex.StackTrace);
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Error writing log to file: " + e.Message);
            }
        }

        /// <summary>
        /// Log a message string
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="outputDirectory">(Optional) Default is bin\logs for an app or class library or /Content/logs for a site</param>
        public static void LogError(string message, string outputDirectory = null)
        {
            try
            {
                string rootPath = string.Empty;
                if (!string.IsNullOrEmpty(outputDirectory))
                {
                    rootPath = outputDirectory;
                }
                else if (HttpContext.Current == null)
                {
                    rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\logs\";
                }
                else
                {
                    rootPath = HttpContext.Current.Server.MapPath("/Content/logs");
                }

                if (!Directory.Exists(rootPath))
                {
                    Directory.CreateDirectory(rootPath);
                }

                string strPath = string.Format("{0}\\log-{1}.txt", rootPath, DateTime.Now.ToString("yyyy-MM-dd"));
                if (!File.Exists(strPath))
                {
                    File.Create(strPath).Dispose();
                }
                using (StreamWriter sw = File.AppendText(strPath))
                {
                    sw.WriteLine(DateTime.Now + "--- " + message);
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Error writing log to file: " + e.Message);
            }
        }
    }
}
