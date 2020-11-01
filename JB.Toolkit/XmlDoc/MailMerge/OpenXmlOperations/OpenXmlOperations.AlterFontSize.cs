using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;

namespace JBToolkit.XmlDoc.MailMerge
{
    /// <summary>
    /// Additional OpenXML mail merge operations helper
    /// </summary>
    public partial class OpenXmlOperations
    {
        /// <summary>
        /// You'd use this method in post processing (after document has been merged). Typically, a merge field will be placed
        /// onto the document denoting the text size, ie. «LargeText» (which woulud be hidden) and when that's merged it will end up being something like 
        /// 'BIGFONT15'. Therefore you specifiy the key or 'prefix' to look for (i.e. BIGFONT) and the integer after that represents
        /// the average font size.
        /// 
        /// You don't want to make the whole document text size '15', but rather you want to increase all the text by a given amount to 
        /// maintain some sort of formatting. Therefore you specify the default size (i.e. 12), so that's a difference of 3 font size points,
        /// so that's what this method will increase the font size by (3).
        /// </summary>
        /// <param name="documentPath">Input document path (after merged)</param>
        /// <param name="prefix">Prefix font size key i.e. 'BIGFONT'</param>
        /// <param name="ignoreHeaderAndFooter">Don't change the font size of the header or footer</param>
        /// <param name="defaultFontSize">The default font size of the document (defaults to 12)</param>
        public static void IncreaseFontSizeByMergedField(
            string documentPath,
            string prefix,
            bool ignoreHeaderAndFooter,
            int defaultFontSize = 12)
        {
            using (var doc = WordprocessingDocument.Open(documentPath, true))
                IncreaseFontSizeByMergedField(doc, prefix, ignoreHeaderAndFooter, defaultFontSize);
        }

        /// <summary>
        /// You'd use this method in post processing (after document has been merged). Typically, a merge field will be placed
        /// onto the document denoting the text size, ie. «LargeText» (which woulud be hidden) and when that's merged it will end up being something like 
        /// 'BIGFONT15'. Therefore you specifiy the key or 'prefix' to look for (i.e. BIGFONT) and the integer after that represents
        /// the average font size.
        /// 
        /// You don't want to make the whole document text size '15', but rather you want to increase all the text by a given amount to 
        /// maintain some sort of formatting. Therefore you specify the default size (i.e. 12), so that's a difference of 3 font size points,
        /// so that's what this method will increase the font size by (3).
        /// </summary>
        /// <param name="document">OpenXML WordprocessingDocument (after merged)</param>
        /// <param name="prefix">Prefix font size key i.e. 'BIGFONT'</param>
        /// <param name="ignoreHeaderAndFooter">Don't change the font size of the header or footer</param>
        /// <param name="defaultFontSize">The default font size of the document (defaults to 12)</param>
        public static void IncreaseFontSizeByMergedField(
            WordprocessingDocument document,
            string prefix,
            bool ignoreHeaderAndFooter,
            int defaultFontSize = 12)
        {
            if (string.IsNullOrEmpty(prefix))
                return;

            int fontSize = FindFontSizeField(document, prefix);
            if (fontSize <= defaultFontSize)
                return;

            IncreaseFontSizeByPointAmount(document, fontSize - defaultFontSize, ignoreHeaderAndFooter);
        }

        /// <summary>
        /// You don't want to make the whole document text size '15', but rather you want to increase all the text by a given amount to 
        /// maintain some sort of formatting. Therefore you specify the default size (i.e. 12), so that's a difference of 3 font size points,
        /// so that's what this method will increase the font size by (3).
        /// </summary>
        /// <param name="documentPath">Input document path (after merged)</param>
        /// <param name="prefix">Prefix font size key i.e. 'BIGFONT'</param>
        /// <param name="ignoreHeaderAndFooter">Don't change the font size of the header or footer</param>
        /// <param name="defaultFontSize">The default font size of the document (defaults to 12)</param>
        public static void IncreaseFontSizeByPointAmount(
            string documentPath,
            int pointIncrease,
            bool ignoreHeaderAndFooter)
        {
            using (var doc = WordprocessingDocument.Open(documentPath, true))
                IncreaseFontSizeByPointAmount(doc, pointIncrease, ignoreHeaderAndFooter);
        }

        /// <summary>
        /// You don't want to make the whole document text size '15', but rather you want to increase all the text by a given amount to 
        /// maintain some sort of formatting. Therefore you specify the default size (i.e. 12), so that's a difference of 3 font size points,
        /// so that's what this method will increase the font size by (3).
        /// </summary>
        /// <param name="document">OpenXML WordprocessingDocument (after merged)</param>
        /// <param name="prefix">Prefix font size key i.e. 'BIGFONT'</param>
        /// <param name="ignoreHeaderAndFooter">Don't change the font size of the header or footer</param>
        /// <param name="defaultFontSize">The default font size of the document (defaults to 12)</param>
        public static void IncreaseFontSizeByPointAmount(
            WordprocessingDocument document,
            int pointIncrease,
            bool ignoreHeaderAndFooter)
        {
            AlterSectionFontSize(document.MainDocumentPart, typeof(MainDocumentPart), pointIncrease);

            if (!ignoreHeaderAndFooter)
            {
                foreach (HeaderPart headerPart in document.MainDocumentPart.HeaderParts)
                    AlterSectionFontSize(headerPart, typeof(HeaderPart), pointIncrease);

                foreach (FooterPart footerPart in document.MainDocumentPart.FooterParts)
                    AlterSectionFontSize(footerPart, typeof(FooterPart), pointIncrease);
            }

            document.Save();
        }

        private static void AlterSectionFontSize(
            object part,
            Type sectionType,
            int pointIncrease)
        {
            if (sectionType == typeof(MainDocumentPart))
            {
                ((MainDocumentPart)part).Document.InnerXml = ReplaceFontSize(((MainDocumentPart)part).Document.InnerXml, pointIncrease);
                ((MainDocumentPart)part).StyleDefinitionsPart.RootElement.InnerXml =
                    ReplaceFontSize(((MainDocumentPart)part).StyleDefinitionsPart.RootElement.InnerXml, pointIncrease);
            }
            else if (sectionType == typeof(HeaderPart))
                ((HeaderPart)part).RootElement.InnerXml = ReplaceFontSize(((HeaderPart)part).RootElement.InnerXml, pointIncrease);
            else if (sectionType == typeof(FooterPart))
                ((FooterPart)part).RootElement.InnerXml = ReplaceFontSize(((FooterPart)part).RootElement.InnerXml, pointIncrease);
        }

        private static string ReplaceFontSize(string xml, int pointIncrease)
        {
            try
            {
                return ProcessReplaceFontSize(xml, pointIncrease, false);
            }
            catch
            {
                try
                {
                    return ProcessReplaceFontSize(xml, pointIncrease, true);
                }
                catch { }
            }

            return xml;
        }

        private static string ProcessReplaceFontSize(string xml, int pointIncrease, bool addRootNode)
        {
            if (addRootNode)
                xml = "<rootx>" + xml + "</rootx>";

            var xdoc = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
            foreach (XElement element in xdoc.Descendants())
            {
                if ((element.Name.LocalName == "sz" ||
                        element.Name.LocalName == "szCs") &&
                    element.LastAttribute.Name.LocalName == "val")
                {
                    int currentPoint = Convert.ToInt32(element.LastAttribute.Value);
                    element.LastAttribute.Value = (currentPoint + (pointIncrease * 2)).ToString();
                }
            }

            if (addRootNode)
                return xdoc.ToString(SaveOptions.DisableFormatting)
                           .Replace("<rootx>", "")
                           .Replace("</rootx>", "");

            return xdoc.ToString(SaveOptions.DisableFormatting);
        }

        private static int FindFontSizeField(WordprocessingDocument doc, string prefix)
        {
            int size = FindFontSizeFieldInSection(doc.MainDocumentPart, typeof(MainDocumentPart), prefix);
            if (size == 0)
            {
                foreach (HeaderPart headerPart in doc.MainDocumentPart.HeaderParts)
                {
                    size = FindFontSizeFieldInSection(headerPart, typeof(HeaderPart), prefix);
                    if (size != 0)
                        break;
                }
            }
            if (size == 0)
            {
                foreach (FooterPart footerPart in doc.MainDocumentPart.FooterParts)
                {
                    FindFontSizeFieldInSection(footerPart, typeof(FooterPart), prefix);
                    if (size != 0)
                        break;
                }
            }

            return size / 2;
        }

        private static int FindFontSizeFieldInSection(
            object section,
            Type sectionType,
            string prefix)
        {
            string docText = null;

            try
            {
                if (sectionType == typeof(MainDocumentPart))
                    using (StreamReader sr = new StreamReader(((MainDocumentPart)section).GetStream()))
                        docText = sr.ReadToEnd();

                else if (sectionType == typeof(HeaderPart))
                    using (StreamReader sr = new StreamReader(((HeaderPart)section).GetStream()))
                        docText = sr.ReadToEnd();

                else if (sectionType == typeof(FooterPart))
                    using (StreamReader sr = new StreamReader(((FooterPart)section).GetStream()))
                        docText = sr.ReadToEnd();

                var match = Regex.Match(docText, string.Format(@"{0}\d+", prefix));

                if (match.Success)
                    return Convert.ToInt32(match.Value.Replace(prefix, "").Trim());
            }
            catch { }

            return 0;
        }
    }
}
