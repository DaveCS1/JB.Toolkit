using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace JBToolkit.XmlDoc.MailMerge
{
    /// <summary>
    /// Uses OpenXML to perform a Word Mail Merge. 
    /// 
    /// It's quick, but no always very reliable. Perhaps this class needs some modifications...
    /// Also, OpenXML doesn't have the facility to convert to PDF, so we have to use the M-Files API to do this for us. It's usually quick, but
    /// can be slow if M-Files is busy.
    /// </summary>
    public class OpenXmlMailMerge
    {
        /// <summary>
        /// Performs a mail merges given a dictionary of merge fields and their merge values and save to file
        /// </summary>
        /// <param name="templatePath">Path to mail merge template</param>
        /// <param name="outputPath">Where to save the file to</param>
        /// <param name="fieldAndValues">A Dictionary of Merge Fields and values</param>
        /// <param name="saveAsPDF">Whether to save as PDF or docx</param>
        public static void MergeDocumentAndSave(
            string templatePath,
            string outputPath,
            Dictionary<string, string> fieldAndValues,
            bool saveAsPDF)
        {
            string file = null;

            if (saveAsPDF)
            {
                FindAndReplace(
                    templatePath,
                    fieldAndValues,
                    Path.Combine(Windows.DirectoryHelper.GetTempFile(), Windows.DirectoryHelper.GetTempFile() + ".docx"));

                PdfDoc.PdfConverter.ConvertToPdf(file, outputPath);

                try
                {
                    File.Delete(file);
                }
                catch { }
            }
            else
            {
                FindAndReplace(
                    templatePath,
                    fieldAndValues,
                    outputPath);
            }
        }

        /// <summary>
        /// Performs a mail merges given a dictionary of merge fields and their merge values and returns a byte array
        /// </summary>
        /// <param name="templatePath">Path to mail merge template</param>
        /// <param name="fieldAndValues">A Dictionary of Merge Fields and values</param>
        /// <param name="saveAsPDF">Whether to save as PDF or docx</param>
        public static byte[] MergeDocumentToByteArray(
            string templatePath,
            Dictionary<string, string> fieldAndValues,
            bool saveAsPDF)
        {
            return MergeDocumentToMemoryStream(templatePath, fieldAndValues, saveAsPDF).ToArray();
        }

        /// <summary>
        /// Performs a mail merges given a dictionary of merge fields and their merge values and returns a base64 string (for blobs)
        /// </summary>
        /// <param name="templatePath">Path to mail merge template</param>
        /// <param name="fieldAndValues">A Dictionary of Merge Fields and values</param>
        /// <param name="saveAsPDF">Whether to save as PDF or docx</param>
        public static string MergeDocumentAsBase64String(
            string templatePath,
            Dictionary<string, string> fieldAndValues,
            bool saveAsPDF)
        {
            return Convert.ToBase64String(MergeDocumentToMemoryStream(templatePath, fieldAndValues, saveAsPDF).ToArray());
        }

        /// <summary>
        /// Performs a mail merges given a dictionary of merge fields and their merge values and returns a Memory Stream
        /// </summary>
        /// <param name="templatePath">Path to mail merge template</param>
        /// <param name="fieldAndValues">A Dictionary of Merge Fields and values</param>
        /// <param name="saveAsPDF">Whether to save as PDF or docx</param>
        public static MemoryStream MergeDocumentToMemoryStream(
            string templatePath,
            Dictionary<string, string> fieldAndValues,
            bool saveAsPDF)
        {
            string tempFile = FindAndReplace(
                                    templatePath,
                                    fieldAndValues,
                                    Path.Combine(Windows.DirectoryHelper.GetTempFile(),
                                    Windows.DirectoryHelper.GetTempFile() + ".docx"));

            MemoryStream ms = new MemoryStream();
            using (FileStream file = new FileStream(tempFile, FileMode.Open, FileAccess.Read))
            {
                file.CopyTo(ms);
            }

            try
            {
                File.Delete(tempFile);
            }
            catch { }

            if (saveAsPDF)
            {
                return PdfDoc.PdfConverter.ConvertMsOfficeDocToPdf(ms, "docx");
            }

            return ms;
        }

        /// <summary>
        /// Basically performs a mail merge by replacing text defined by a dictionary of keys (in the format «key») with the replacing text in the value of the dictionary.
        /// This method's default is to produce a temporary file and output the name of the file that has been created unless the 'SaveAsPath' parameter has been specified
        /// </summary>
        /// <param name="templatePath">Template document path</param>
        /// <param name="values">Dictionary of values to replace</param>
        /// <param name="saveAsPath">Optional output path</param>
        /// <returns>The default temp path of the created mail merge or a specified path</returns>
        public static string FindAndReplace(
            string templatePath,
            Dictionary<string, string> values,
            string saveAsPath = null,
            string tempFolderRoot = null)
        {
            string tempFilePath = Path.Combine(!string.IsNullOrEmpty(tempFolderRoot)
                                                        ? tempFolderRoot
                                                        : Path.GetTempPath(),
                                                DateTime.Now.Ticks + ".docx");

            string returnPath = tempFilePath;

            if (!Directory.Exists(Path.GetDirectoryName(tempFilePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(tempFilePath));
            }

            try
            {
                // Copy Word document.
                File.Copy(templatePath, tempFilePath, true);

                // Open copied document.
                using (var safeStream = new Streams.SafeFileStream(tempFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    safeStream.Open();

                    using (WordprocessingDocument wordDoc = WordprocessingDocument.Open((Stream)safeStream, true))
                    {
                        MergeMainDocumentParts(wordDoc.MainDocumentPart, typeof(MainDocumentPart), values);

                        foreach (HeaderPart headerPart in wordDoc.MainDocumentPart.HeaderParts)
                        {
                            MergeMainDocumentParts(headerPart, typeof(HeaderPart), values);
                        }

                        foreach (FooterPart footerPart in wordDoc.MainDocumentPart.FooterParts)
                        {
                            MergeMainDocumentParts(footerPart, typeof(FooterPart), values);
                        }

                        if (!string.IsNullOrEmpty(saveAsPath))
                        {
                            if (!Directory.Exists(Path.GetDirectoryName(saveAsPath)))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(saveAsPath));
                            }

                            var newDoc = wordDoc.SaveAs(saveAsPath);
                            newDoc.Close();
                            newDoc.Dispose();

                            returnPath = saveAsPath;
                        }
                        else
                        {
                            wordDoc.Save();
                        }
                    }
                }
            }
            catch (Exception)
            {
                try
                {
                    File.Delete(saveAsPath);
                }
                catch { }

                throw;
            }

            try
            {
                if (!string.IsNullOrEmpty(saveAsPath))
                {
                    try
                    {
                        File.Delete(tempFilePath);
                    }
                    catch { }
                }
            }
            catch { }

            return returnPath;
        }

        private static void MergeMainDocumentParts(
            object section,
            Type sectionType,
            Dictionary<string, string> values)
        {
            string docText = null;

            if (sectionType == typeof(MainDocumentPart))
            {
                using (StreamReader sr = new StreamReader(((MainDocumentPart)section).GetStream()))
                {
                    docText = sr.ReadToEnd();
                }
            }
            else if (sectionType == typeof(HeaderPart))
            {
                using (StreamReader sr = new StreamReader(((HeaderPart)section).GetStream()))
                {
                    docText = sr.ReadToEnd();
                }
            }
            else if (sectionType == typeof(FooterPart))
            {
                using (StreamReader sr = new StreamReader(((FooterPart)section).GetStream()))
                {
                    docText = sr.ReadToEnd();
                }
            }

            foreach (var value in values)
            {
                try
                {
                    // Dirty way of including new lines

                    string toReplace = string.IsNullOrEmpty(value.Value) ? " " :
                        value.Value.Replace("\r\n", "</w:t><w:br/><w:t>")
                        .Replace("\n\r", "</w:t><w:br/><w:t>")
                        .Replace("\r", "</w:t><w:br/><w:t>")
                        .Replace("\n", "</w:t><w:br/><w:t>");

                    if (toReplace.ToLower().EndsWith(".gif"))
                    {
                        docText = new Regex("«" + value.Key + "»").Replace(docText, " ");
                    }
                    else
                    {
                        docText = new Regex("«" + value.Key + "»").Replace(docText, toReplace);
                    }
                }
                catch (Exception e) { Console.Out.WriteLine(e.Message); }
            }

            // Remove empty merge fields
            docText = new Regex(@"«[\s\S]*»").Replace(docText, "");

            if (sectionType == typeof(MainDocumentPart))
            {
                using (StreamWriter sw = new StreamWriter(((MainDocumentPart)section).GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }
            }
            else if (sectionType == typeof(HeaderPart))
            {
                using (StreamWriter sw = new StreamWriter(((HeaderPart)section).GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }
            }
            else if (sectionType == typeof(FooterPart))
            {
                using (StreamWriter sw = new StreamWriter(((FooterPart)section).GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }
            }
        }
    }
}
