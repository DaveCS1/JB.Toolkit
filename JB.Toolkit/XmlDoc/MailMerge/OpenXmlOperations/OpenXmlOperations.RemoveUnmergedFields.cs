using DocumentFormat.OpenXml.Packaging;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace JBToolkit.XmlDoc.MailMerge
{
    /// <summary>
    /// Additional OpenXML mail merge operations helper
    /// </summary>
    public partial class OpenXmlOperations
    {
        /// <summary>
        /// Remove unmerged («Fields») merge fields left on the document 
        /// </summary>
        /// <param name="documentPath">Input document path (after merged)</param>
        public static void IncreaseFontSizeByMergedField(
            string documentPath)
        {
            using (var doc = WordprocessingDocument.Open(documentPath, true))
                RemoveUnmergedFields(doc);
        }

        /// <summary>
        /// Remove unmerged («Fields») merge fields left on the document 
        /// </summary>
        /// <param name="document">WordprocessingDocument document (after merged)</param>
        public static void RemoveUnmergedFields(
            WordprocessingDocument document)
        {
            FindAndReplaceUnmergedFields(document.MainDocumentPart, typeof(MainDocumentPart));
            foreach (HeaderPart headerPart in document.MainDocumentPart.HeaderParts)
                FindAndReplaceUnmergedFields(headerPart, typeof(HeaderPart));

            foreach (FooterPart footerPart in document.MainDocumentPart.FooterParts)
                FindAndReplaceUnmergedFields(footerPart, typeof(FooterPart));

            document.Save();
        }

        private static void FindAndReplaceUnmergedFields(
            object section,
            Type sectionType)
        {
            string docText = null;
            if (sectionType == typeof(MainDocumentPart))
                using (StreamReader sr = new StreamReader(((MainDocumentPart)section).GetStream()))
                    docText = sr.ReadToEnd();

            else if (sectionType == typeof(HeaderPart))
                using (StreamReader sr = new StreamReader(((HeaderPart)section).GetStream()))
                    docText = sr.ReadToEnd();

            else if (sectionType == typeof(FooterPart))
                using (StreamReader sr = new StreamReader(((FooterPart)section).GetStream()))
                    docText = sr.ReadToEnd();

            // Remove empty merge fields
            docText = new Regex(@"«[\s\S]*»").Replace(docText, "");

            if (sectionType == typeof(MainDocumentPart))
                using (StreamWriter sw = new StreamWriter(((MainDocumentPart)section).GetStream(FileMode.Create)))
                    sw.Write(docText);

            else if (sectionType == typeof(HeaderPart))
                using (StreamWriter sw = new StreamWriter(((HeaderPart)section).GetStream(FileMode.Create)))

                    sw.Write(docText);

            else if (sectionType == typeof(FooterPart))
                using (StreamWriter sw = new StreamWriter(((FooterPart)section).GetStream(FileMode.Create)))
                    sw.Write(docText);
        }
    }
}
