using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using DataTable = System.Data.DataTable;

namespace JBToolkit.XmlDoc.MailMerge
{
    /// <summary>
    /// Uses OpenXML to perform a Word Mail Merge. 
    /// 
    /// Microsoft Word needs to be installed on the executing machine in order to use this. It uses an Interop API, which means to perform the mail merge,
    /// Word needs to be opened in the background, then the mail merge is run and finally office is closed down.
    /// 
    /// This is the most reliable in terms of output format, and you can extend to run macros, however this isn't the best means in terms of efficiency and 
    /// it isn't cross user thread safe. So if you're wanting to use as part of a web service / web site running on IIS, you'll need to make sure Office is
    /// installed, you employ lock / wait timers and additional configurations are setup, refer to this guides:
    /// 
    /// https://www.ryadel.com/en/office-interop-dcom-config-windows-server-iis-word-excel-access-asp-net-c-sharp/
    /// http://www.bloing.net/2011/01/how-to-make-iis7-play-nice-with-office-interop/ 
    /// 
    /// Don't use if the class will get hammered. Either try the OpenXML implementation in this toolkit or speak to someone to invest in Aspose software.
    /// </summary>
    public class WordInteropMailMerge
    {
        /// <summary>
        /// Performs a mail merges given a dictionary of merge fields and their merge values and returns a byte array
        /// </summary>
        /// <param name="templatePath">Path to mail merge template</param>
        /// <param name="mailMergeData">A Dictionary of Merge Fields and values</param>
        /// <param name="saveAsPDF">Whether to save as PDF or docx</param>
        public static byte[] MergeDocumentToByteArray(string templatePath, DataTable mailMergeData, bool saveAsPDF)
        {
            return MergeDocumentToMemoryStream(templatePath, mailMergeData, saveAsPDF).ToArray();
        }

        /// <summary>
        /// Performs a mail merges given a dictionary of merge fields and their merge values and returns a base64 string (for blobs)
        /// </summary>
        /// <param name="templatePath">Path to mail merge template</param>
        /// <param name="mailMergeData">A Dictionary of Merge Fields and values</param>
        /// <param name="saveAsPDF">Whether to save as PDF or docx</param>
        public static string MergeDocumentAsBase64String(string templatePath, DataTable mailMergeData, bool saveAsPDF)
        {
            return Convert.ToBase64String(MergeDocumentToMemoryStream(templatePath, mailMergeData, saveAsPDF).ToArray());
        }

        /// <summary>
        /// Performs a mail merges given a dictionary of merge fields and their merge values and returns a Memory Stream
        /// </summary>
        /// <param name="templatePath">Path to mail merge template</param>
        /// <param name="mailMergeData">A Dictionary of Merge Fields and values</param>
        /// <param name="saveAsPDF">Whether to save as PDF or docx</param>
        public static MemoryStream MergeDocumentToMemoryStream(string templatePath, DataTable mailMergeData, bool saveAsPDF)
        {
            string tempFile = Path.Combine(Windows.DirectoryHelper.GetTempFile(),
                                    Windows.DirectoryHelper.GetTempFile() + ".docx");

            MergeDocumentAndSave(templatePath,
                                  templatePath,
                                  mailMergeData,
                                  saveAsPDF);

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

            return ms;
        }

        /// <summary>
        /// Performs a mail merges given a dictionary of merge fields and their merge values and saves or opens word for user interaction
        /// </summary>
        /// <param name="templatePath">Path to mail merge template</param>
        /// <param name="mailMergeData">A Dictionary of Merge Fields and values</param>
        /// <param name="saveAsPDF">Whether to save as PDF or docx</param>
        public static void MergeDocumentAndSave(string templatePath, string outputPath, DataTable mailMergeData, bool saveAsPDF, bool openAndRender = false)
        {
            var application = new Application();
            var document = new Document();

            string directoryName = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directoryName))
            {
                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);
            }

            try
            {
                string tempCSVFile = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks.ToString() + ".csv");
                mailMergeData.ToCsvFile(tempCSVFile);

                document = application.Documents.Add(Template: templatePath);

                object subType = WdMergeSubType.wdMergeSubTypeOther;
                document.MailMerge.OpenDataSource(Name: tempCSVFile, SubType: subType, ConfirmConversions: false);

                RemovedUnmappedMergeFields(document, mailMergeData);
                document.MailMerge.Execute(false);

                document.Close(false);
                document = application.Documents.get_Item(1);

                if (openAndRender)
                {
                    application.Visible = true;
                }
                else
                {
                    if (!saveAsPDF)
                    {
                        document.SaveAs2(FileName: outputPath, FileFormat: WdSaveFormat.wdFormatDocumentDefault);
                    }
                    else
                    {
                        document.SaveAs2(FileName: outputPath, FileFormat: WdExportFormat.wdExportFormatPDF);
                    }

                    document.Close(false);
                    application.Quit(false);

                    try
                    {
                        File.Delete(tempCSVFile);
                    }
                    catch { }
                }
            }
            catch (Exception)
            {
                if (document != null)
                {
                    document.Close(false);
                }

                if (application != null)
                {
                    application.Quit(false);
                }

                throw;
            }
        }

        /// <summary>
        /// Is a particular field Word mail merge field
        /// </summary>
        public static bool IsAMergeField(Field field)
        {
            string fullField = field.Code.Text.Trim();
            if (!fullField.StartsWith("MERGEFIELD"))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Remove any unmerged fields remaining in the document
        /// </summary>
        public static void RemovedUnmappedMergeFields(Document document, DataTable mergeData)
        {
            var allColumnNames = new List<string>();

            foreach (DataColumn column in mergeData.Columns)
            {
                allColumnNames.Add(column.ColumnName);
            }

            foreach (Field field in document.Fields)
            {
                if (IsAMergeField(field))
                {
                    if (!allColumnNames.Contains(StripMergeFieldFormatting(field.Code.Text)))
                    {
                        field.Delete();
                    }
                }
            }

            foreach (Shape shape in document.Shapes)
            {
                if (shape.TextFrame.HasText != 0)
                {
                    foreach (Field field in shape.TextFrame.ContainingRange.Fields)
                    {
                        if (IsAMergeField(field))
                        {
                            if (!allColumnNames.Contains(StripMergeFieldFormatting(field.Code.Text)))
                            {
                                field.Delete();
                            }
                        }
                    }
                }
            }

            foreach (Section section in document.Sections)
            {
                foreach (HeaderFooter hf in section.Headers)
                {
                    foreach (Field field in hf.Range.Fields)
                    {
                        if (IsAMergeField(field))
                        {
                            if (!allColumnNames.Contains(StripMergeFieldFormatting(field.Code.Text)))
                            {
                                field.Delete();
                            }
                        }
                    }

                    foreach (Shape shape in hf.Shapes)
                    {
                        if (shape.TextFrame.HasText != 0)
                        {
                            foreach (Field field in shape.TextFrame.ContainingRange.Fields)
                            {
                                if (IsAMergeField(field))
                                {
                                    if (!allColumnNames.Contains(StripMergeFieldFormatting(field.Code.Text)))
                                    {
                                        field.Delete();
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (HeaderFooter hf in section.Footers)
                {
                    foreach (Field field in hf.Range.Fields)
                    {
                        if (IsAMergeField(field))
                        {
                            if (!allColumnNames.Contains(StripMergeFieldFormatting(field.Code.Text)))
                            {
                                field.Delete();
                            }
                        }
                    }

                    foreach (Shape shape in hf.Shapes)
                    {
                        if (shape.TextFrame.HasText != 0)
                        {
                            foreach (Field field in shape.TextFrame.ContainingRange.Fields)
                            {
                                if (IsAMergeField(field))
                                {
                                    if (!allColumnNames.Contains(StripMergeFieldFormatting(field.Code.Text)))
                                    {
                                        field.Delete();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// If a merge field is in the Word document format 'MERGEFIELD SomeMergeField' then just return the field as it's shown between the '«' and '»". 
        /// Useful if you're trying to automate mapping DataTable column names to merge fields
        /// </summary>
        public static string StripMergeFieldFormatting(string mergeFieldText)
        {
            return mergeFieldText.Replace("MERGEFIELD ", "").Replace("\"", "").Trim();
        }
    }
}
