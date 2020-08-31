using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace JBToolkit.PdfDoc
{
    /// <summary>
    /// Methods to split up a PDF document with various criteria such as page quantity multiples, an array of page numbers to split
    /// after or actual string content (included regular expression matching) within the document itself. You can input and  output
    /// a memory stream or save to file.
    /// </summary>
    public class PdfSplitter
    {
        /// <summary>
        /// Split a document on multiples and save to file. the name will be the original number plus the document number
        /// </summary>
        public static void SplitToPageQuantityMultiples(string inputFilePath, string outputDirectory, int pageQuantityMultiple)
        {
            using (PdfDocument inputDocument = PdfReader.Open(inputFilePath, PdfDocumentOpenMode.Import))
            {
                string fileNameWithoutExtension = Path.GetFileName(inputFilePath).Replace(Path.GetExtension(inputFilePath), "");

                PdfDocument outputDocument = null;
                int documentCount = 0;

                for (int i = 0; i < inputDocument.PageCount; i++)
                {
                    if (i % pageQuantityMultiple == 0)
                    {
                        outputDocument = new PdfDocument
                        {
                            Version = inputDocument.Version
                        };

                        outputDocument.Info.Creator = inputDocument.Info.Creator;
                        outputDocument.Info.Title = fileNameWithoutExtension.Wordify();

                        documentCount++;
                    }

                    outputDocument.AddPage(inputDocument.Pages[i]);

                    if (i % pageQuantityMultiple == 0)
                    {
                        outputDocument.Save(string.Format(@"{0}\{1}-{2}.pdf", outputDirectory, fileNameWithoutExtension, documentCount));
                    }
                }
            }
        }

        /// <summary>
        /// Split a document on multiples and save to file. the name will be the original number plus the document number
        /// </summary>
        public static void SplitToPageQuantityMultiples(MemoryStream ms, string title, string outputDirectory, int pageQuantityMultiple)
        {
            using (PdfDocument inputDocument = PdfReader.Open(ms, PdfDocumentOpenMode.Import))
            {
                PdfDocument outputDocument = null;
                int documentCount = 0;

                for (int i = 0; i < inputDocument.PageCount; i++)
                {
                    if (i % pageQuantityMultiple == 0)
                    {
                        outputDocument = new PdfDocument
                        {
                            Version = inputDocument.Version
                        };

                        outputDocument.Info.Creator = inputDocument.Info.Creator;
                        outputDocument.Info.Title = title;

                        documentCount++;
                    }

                    outputDocument.AddPage(inputDocument.Pages[i]);

                    if (i % pageQuantityMultiple == 0)
                    {
                        outputDocument.Save(string.Format(@"{0}\{1}-{2}.pdf", outputDirectory, title, documentCount));
                    }
                }
            }
        }

        /// <summary>
        /// Split a document on multiples and return a Memory Stream list
        /// </summary>
        public static List<MemoryStream> SplitToPageQuantityMultiples(MemoryStream ms, int pageQuantityMultiples)
        {
            List<MemoryStream> outputMsList = new List<MemoryStream>();
            MemoryStream outputMs = null;

            using (PdfDocument inputDocument = PdfReader.Open(ms, PdfDocumentOpenMode.Import))
            {
                PdfDocument outputDocument = null;
                int documentCount = 0;

                for (int i = 0; i < inputDocument.PageCount; i++)
                {
                    if (i % pageQuantityMultiples == 0)
                    {
                        outputMs = new MemoryStream();
                        outputDocument = new PdfDocument
                        {
                            Version = inputDocument.Version
                        };

                        outputDocument.Info.Creator = inputDocument.Info.Creator;
                        outputDocument.Info.Title = inputDocument.Info.Title;

                        documentCount++;
                    }

                    outputDocument.AddPage(inputDocument.Pages[i]);

                    if (i % pageQuantityMultiples == 0)
                    {
                        outputDocument.Save(outputMs);
                        outputMsList.Add(outputMs);
                    }
                }
            }

            return outputMsList;
        }

        /// <summary>
        /// Split a document on specific pages and save to file. the name will be the original number plus the document number
        /// </summary>
        public static void SplitAfterPageNumbers(string inputFilePath, string outputDirectory, int[] pageNumbers)
        {
            if (pageNumbers == null)
            {
                throw new ApplicationException("pageNumbers parameter cannot be null");
            }

            using (PdfDocument inputDocument = PdfReader.Open(inputFilePath, PdfDocumentOpenMode.Import))
            {
                string fileNameWithoutExtension = Path.GetFileName(inputFilePath).Replace(Path.GetExtension(inputFilePath), "");

                PdfDocument outputDocument = null;

                if (pageNumbers[pageNumbers.Length - 1] > inputDocument.PageCount)
                {
                    throw new ApplicationException("pageNumbers last index value cannot be greater than the actual page count of the document");
                }

                int nextSplitPageNumber = pageNumbers[0];
                int documentCount = 0;

                for (int i = 0; i < inputDocument.PageCount; i++)
                {
                    if (i == nextSplitPageNumber)
                    {
                        outputDocument.Save(string.Format(@"{0}\{1}-{2}.pdf", outputDirectory, fileNameWithoutExtension, documentCount));
                    }

                    if (i == 0 || i == nextSplitPageNumber)
                    {
                        outputDocument = new PdfDocument
                        {
                            Version = inputDocument.Version
                        };

                        outputDocument.Info.Creator = inputDocument.Info.Creator;
                        outputDocument.Info.Title = fileNameWithoutExtension.Wordify();

                        if (documentCount < pageNumbers.Length)
                        {
                            nextSplitPageNumber = pageNumbers[documentCount];
                        }

                        documentCount++;
                    }

                    outputDocument.AddPage(inputDocument.Pages[i]);

                    if (i + 1 == inputDocument.PageCount)
                    {
                        outputDocument.Save(string.Format(@"{0}\{1}-{2}.pdf", outputDirectory, fileNameWithoutExtension, documentCount));
                    }
                }
            }
        }

        /// <summary>
        /// Split a document after specific pages and save to file. the name will be the original number plus the document number
        /// </summary>
        public static void SplitAfterPageNumbers(MemoryStream ms, string title, string outputDirectory, int[] pageNumbers)
        {
            if (pageNumbers == null)
            {
                throw new ApplicationException("pageNumbers parameter cannot be null");
            }

            using (PdfDocument inputDocument = PdfReader.Open(ms, PdfDocumentOpenMode.Import))
            {
                PdfDocument outputDocument = null;

                if (pageNumbers[pageNumbers.Length - 1] > inputDocument.PageCount)
                {
                    throw new ApplicationException("pageNumbers last index value cannot be greater than the actual page count of the document");
                }

                int nextSplitPageNumber = pageNumbers[0];
                int documentCount = 0;

                for (int i = 0; i < inputDocument.PageCount; i++)
                {
                    if (i == nextSplitPageNumber)
                    {
                        outputDocument.Save(string.Format(@"{0}\{1}-{2}.pdf", outputDirectory, title, documentCount));
                    }

                    if (i == 0 || i == nextSplitPageNumber)
                    {
                        outputDocument = new PdfDocument
                        {
                            Version = inputDocument.Version
                        };

                        outputDocument.Info.Creator = inputDocument.Info.Creator;
                        outputDocument.Info.Title = title;

                        if (documentCount < pageNumbers.Length)
                        {
                            nextSplitPageNumber = pageNumbers[documentCount];
                        }

                        documentCount++;
                    }

                    outputDocument.AddPage(inputDocument.Pages[i]);

                    if (i + 1 == inputDocument.PageCount)
                    {
                        outputDocument.Save(string.Format(@"{0}\{1}-{2}.pdf", outputDirectory, title, documentCount));
                    }
                }
            }
        }

        /// <summary>
        /// Split a document after specific pages and return a Memory Stream list
        /// </summary>
        public static List<MemoryStream> SplitAfterPageNumbers(MemoryStream ms, int[] pageNumbers)
        {
            if (pageNumbers == null)
            {
                throw new ApplicationException("pageNumbers parameter cannot be null");
            }

            List<MemoryStream> outputMsList = new List<MemoryStream>();
            MemoryStream outputMs = null;

            using (PdfDocument inputDocument = PdfReader.Open(ms, PdfDocumentOpenMode.Import))
            {
                PdfDocument outputDocument = null;

                if (pageNumbers[pageNumbers.Length - 1] > inputDocument.PageCount)
                {
                    throw new ApplicationException("pageNumbers last index value cannot be greater than the actual page count of the document");
                }

                int nextSplitPageNumber = pageNumbers[0];
                int documentCount = 0;

                for (int i = 0; i < inputDocument.PageCount; i++)
                {
                    if (i == nextSplitPageNumber)
                    {
                        outputDocument.Save(outputMs);
                        outputMsList.Add(outputMs);
                    }

                    if (i == 0 || i == nextSplitPageNumber)
                    {
                        outputMs = new MemoryStream();
                        outputDocument = new PdfDocument
                        {
                            Version = inputDocument.Version
                        };

                        outputDocument.Info.Creator = inputDocument.Info.Creator;
                        outputDocument.Info.Title = inputDocument.Info.Title;

                        if (documentCount < pageNumbers.Length)
                        {
                            nextSplitPageNumber = pageNumbers[documentCount];
                        }

                        documentCount++;
                    }

                    outputDocument.AddPage(inputDocument.Pages[i]);

                    if (i + 1 == inputDocument.PageCount)
                    {
                        outputDocument.Save(outputMs);
                        outputMsList.Add(outputMs);
                    }
                }
            }

            return outputMsList;
        }

        /// <summary>
        /// Use the content of the document to determine where to split the document. I.e. on a document code in the header. You may use a static string or a regular expression. You can also set 
        /// the document title to be the value of the match string. Saves to file.
        /// </summary>
        /// <param name="stringCode">Static string or regular expression - Will split 'after' the page where this string exists</param>
        /// <param name="isStringRegularExpression">If the string code is regular expression or not (not being a static string)</param>
        /// <param name="ifUsingRegulareExpressionThenUseMatchValueAsDocumentTile">USe the match string value as part of the filename and title of each document</param>
        public static void SplitAfterPagesWhereStringAppearsInContent(
            string inputFilePath,
            string outputDirectory,
            string stringCode,
            bool isStringRegularExpression = false,
            bool ifUsingRegulareExpressionThenUseMatchValueAsDocumentTile = true,
            bool addTicksToFilename = false)
        {
            if (string.IsNullOrEmpty(stringCode))
            {
                throw new ApplicationException("pageNumbers parameter cannot be null");
            }

            using (PdfDocument inputDocument = PdfReader.Open(inputFilePath, PdfDocumentOpenMode.Import))
            {
                string fileNameWithoutExtension = Path.GetFileName(inputFilePath).Replace(Path.GetExtension(inputFilePath), "");

                PdfDocument outputDocument = null;

                MemoryStream pageMs = new MemoryStream();
                using (FileStream file = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
                {
                    file.CopyTo(pageMs);
                }

                List<MemoryStream> splitAllPages = SplitToPageQuantityMultiples(pageMs, 1);
                List<int> pageNumbersList = new List<int>();
                List<string> matchStrings = new List<string>();

                for (int i = 0; i < splitAllPages.Count; i++)
                {
                    string content = string.Empty;
                    content = PdfParser.GetPDFContentAsString(splitAllPages[i]);

                    if (isStringRegularExpression)
                    {
                        Regex r = new Regex(stringCode);
                        MatchCollection collection = r.Matches(content);
                        if (collection.Count > 0)
                        {
                            pageNumbersList.Add(i);
                            matchStrings.Add(collection[0].Value);
                        }
                    }
                    else
                    {
                        if (content.Contains(stringCode))
                        {
                            pageNumbersList.Add(i);
                        }
                    }
                }

                // For regular expression match, treat multiple match values as same document
                if (isStringRegularExpression)
                {
                    List<int> removeIndexs = new List<int>();
                    string currentMatch = string.Empty;
                    int index = 0;

                    foreach (var matchString in matchStrings)
                    {
                        if (matchString == currentMatch && !string.IsNullOrEmpty(matchString) && !string.IsNullOrEmpty(currentMatch))
                        {
                            removeIndexs.Add(index);
                        }

                        currentMatch = matchString;
                        index++;
                    }

                    for (int i = removeIndexs.Count - 1; i >= 0; i--)
                    {
                        matchStrings.RemoveAt(removeIndexs[i]);
                        pageNumbersList.RemoveAt(removeIndexs[i]);
                    }
                }

                int[] pageNumbers = pageNumbersList.Where(p => p != 0).ToArray();

                int nextSplitPageNumber = pageNumbers[0];
                int documentCount = 0;

                for (int i = 0; i < inputDocument.PageCount; i++)
                {
                    if (i == nextSplitPageNumber)
                    {
                        if (ifUsingRegulareExpressionThenUseMatchValueAsDocumentTile)
                        {
                            outputDocument.Save(string.Format(@"{0}\{1}-{2}{3}.pdf",
                                outputDirectory,
                                matchStrings[documentCount - 1],
                                documentCount,
                                addTicksToFilename ? " [" + Windows.DirectoryHelper.Tickstamp() + "]" : ""));
                        }
                        else
                        {
                            outputDocument.Save(string.Format(@"{0}\{1}-{2}{3}.pdf",
                                outputDirectory,
                                fileNameWithoutExtension,
                                documentCount,
                                addTicksToFilename ? " [" + Windows.DirectoryHelper.Tickstamp() + "]" : ""));
                        }
                    }

                    if (i == 0 || i == nextSplitPageNumber)
                    {
                        outputDocument = new PdfDocument
                        {
                            Version = inputDocument.Version
                        };

                        outputDocument.Info.Creator = inputDocument.Info.Creator;

                        if (isStringRegularExpression && ifUsingRegulareExpressionThenUseMatchValueAsDocumentTile)
                        {
                            outputDocument.Info.Title = matchStrings[documentCount];
                        }
                        else
                        {
                            outputDocument.Info.Title = fileNameWithoutExtension.Wordify();
                        }

                        if (documentCount < pageNumbers.Length)
                        {
                            nextSplitPageNumber = pageNumbers[documentCount];
                        }

                        documentCount++;
                    }

                    outputDocument.AddPage(inputDocument.Pages[i]);

                    if (i + 1 == inputDocument.PageCount)
                    {
                        if (ifUsingRegulareExpressionThenUseMatchValueAsDocumentTile)
                        {
                            outputDocument.Save(string.Format(@"{0}\{1}-{2}{3}.pdf",
                                outputDirectory,
                                matchStrings[documentCount - 1],
                                documentCount,
                                addTicksToFilename ? " [" + Windows.DirectoryHelper.Tickstamp() + "]" : ""));
                        }
                        else
                        {
                            outputDocument.Save(string.Format(@"{0}\{1}-{2}{3}.pdf",
                                outputDirectory,
                                fileNameWithoutExtension,
                                documentCount,
                                addTicksToFilename ? " [" + Windows.DirectoryHelper.Tickstamp() + "]" : ""));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Use the content of the document to determine where to split the document. I.e. on a document code in the header. You may use a static string or a regular expression. You can also set 
        /// the document title to be the value of the match string. Saves to file.
        /// </summary>
        /// <param name="stringCode">Static string or regular expression - Will split 'after' the page where this string exists</param>
        /// <param name="isStringRegularExpression">If the string code is regular expression or not (not being a static string)</param>
        /// <param name="ifUsingRegulareExpressionThenUseMatchValueAsDocumentTile">USe the match string value as part of the filename and title of each document</param>
        public static void SplitAfterPagesWhereStringAppearsInContent(
            MemoryStream ms,
            string title,
            string outputDirectory,
            string stringCode,
            bool isStringRegularExpression = false,
            bool ifUsingRegulareExpressionThenUseMatchValueAsDocumentTile = true,
            bool addTicksToFilename = false)
        {
            if (string.IsNullOrEmpty(stringCode))
            {
                throw new ApplicationException("pageNumbers parameter cannot be null");
            }

            using (PdfDocument inputDocument = PdfReader.Open(ms, PdfDocumentOpenMode.Import))
            {
                PdfDocument outputDocument = null;

                List<MemoryStream> splitAllPages = SplitToPageQuantityMultiples(ms, 1);
                List<int> pageNumbersList = new List<int>();

                List<string> matchStrings = new List<string>();

                for (int i = 0; i < splitAllPages.Count; i++)
                {
                    string content = PdfParser.GetPDFContentAsString(splitAllPages[i]);

                    if (isStringRegularExpression)
                    {
                        Regex r = new Regex(stringCode);
                        MatchCollection collection = r.Matches(content);
                        if (collection.Count > 0)
                        {
                            pageNumbersList.Add(i);
                            matchStrings.Add(collection[0].Value);
                        }
                        else
                        {
                            if (content.Contains(stringCode))
                            {
                                pageNumbersList.Add(i);
                            }
                        }
                    }
                }

                int[] pageNumbers = pageNumbersList.Where(p => p != 0).ToArray();

                int nextSplitPageNumber = pageNumbers[0];
                int documentCount = 0;

                for (int i = 0; i < inputDocument.PageCount; i++)
                {
                    if (i == nextSplitPageNumber)
                    {
                        if (ifUsingRegulareExpressionThenUseMatchValueAsDocumentTile)
                        {
                            outputDocument.Save(string.Format(@"{0}\{1}-{2}{3}.pdf",
                                outputDirectory,
                                matchStrings[documentCount - 1],
                                documentCount,
                                addTicksToFilename ? " [" + Windows.DirectoryHelper.Tickstamp() + "]" : ""));
                        }
                        else
                        {
                            outputDocument.Save(string.Format(@"{0}\{1}-{2}{3}.pdf",
                                outputDirectory,
                                title,
                                documentCount,
                                addTicksToFilename ? " [" + Windows.DirectoryHelper.Tickstamp() + "]" : ""));
                        }
                    }

                    if (i == 0 || i == nextSplitPageNumber)
                    {
                        outputDocument = new PdfDocument
                        {
                            Version = inputDocument.Version
                        };

                        outputDocument.Info.Creator = inputDocument.Info.Creator;

                        if (isStringRegularExpression && ifUsingRegulareExpressionThenUseMatchValueAsDocumentTile)
                        {
                            outputDocument.Info.Title = matchStrings[documentCount];
                        }
                        else
                        {
                            outputDocument.Info.Title = title;
                        }

                        if (documentCount < pageNumbers.Length)
                        {
                            nextSplitPageNumber = pageNumbers[documentCount];
                        }

                        documentCount++;
                    }

                    outputDocument.AddPage(inputDocument.Pages[i]);

                    if (i + 1 == inputDocument.PageCount)
                    {
                        if (ifUsingRegulareExpressionThenUseMatchValueAsDocumentTile)
                        {
                            outputDocument.Save(string.Format(@"{0}\{1}-{2}{3}.pdf",
                                outputDirectory,
                                matchStrings[documentCount - 1],
                                documentCount,
                                addTicksToFilename ? " [" + Windows.DirectoryHelper.Tickstamp() + "]" : ""));
                        }
                        else
                        {
                            outputDocument.Save(string.Format(@"{0}\{1}-{2}{3}.pdf",
                                outputDirectory,
                                title,
                                documentCount,
                                addTicksToFilename ? " [" + Windows.DirectoryHelper.Tickstamp() + "]" : ""));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Use the content of the document to determine where to split the document. I.e. on a document code in the header. You may use a static string or a regular expression. You can also set 
        /// the document title to be the value of the match string. Retuns Memory Stream list.
        /// </summary>
        /// <param name="stringCode">Static string or regular expression - Will split 'after' the page where this string exists</param>
        /// <param name="isStringRegularExpression">If the string code is regular expression or not (not being a static string)</param>
        /// <param name="ifUsingRegulareExpressionThenUseMatchValueAsDocumentTile">USe the match string value as part of the filename and title of each document</param>
        public static List<MemoryStream> SplitAfterPagesWhereStringAppearsInContent(
            MemoryStream ms,
            string stringCode,
            bool isStringRegularExpression = false,
            bool ifUsingRegulareExpressionThenUseMatchValueAsDocumentTile = true)
        {
            if (string.IsNullOrEmpty(stringCode))
            {
                throw new ApplicationException("pageNumbers parameter cannot be null");
            }

            List<MemoryStream> outputMsList = new List<MemoryStream>();
            MemoryStream outputMs = null;

            using (PdfDocument inputDocument = PdfReader.Open(ms, PdfDocumentOpenMode.Import))
            {
                PdfDocument outputDocument = null;

                List<MemoryStream> splitAllPages = SplitToPageQuantityMultiples(ms, 1);
                List<int> pageNumbersList = new List<int>();

                List<string> matchStrings = new List<string>();

                for (int i = 0; i < splitAllPages.Count; i++)
                {
                    string content = PdfParser.GetPDFContentAsString(splitAllPages[i]);

                    if (isStringRegularExpression)
                    {
                        Regex r = new Regex(stringCode);
                        MatchCollection collection = r.Matches(content);
                        if (collection.Count > 0)
                        {
                            pageNumbersList.Add(i);
                            matchStrings.Add(collection[0].Value);
                        }
                    }
                    else
                    {
                        if (content.Contains(stringCode))
                        {
                            pageNumbersList.Add(i);
                        }
                    }
                }

                int[] pageNumbers = pageNumbersList.Where(p => p != 0).ToArray();

                int nextSplitPageNumber = pageNumbers[0];
                int documentCount = 0;

                for (int i = 0; i < inputDocument.PageCount; i++)
                {
                    if (i == nextSplitPageNumber)
                    {
                        outputDocument.Save(outputMs);
                        outputMsList.Add(outputMs);
                    }

                    if (i == 0 || i == nextSplitPageNumber)
                    {
                        outputMs = new MemoryStream();
                        outputDocument = new PdfDocument
                        {
                            Version = inputDocument.Version
                        };

                        outputDocument.Info.Creator = inputDocument.Info.Creator;

                        if (isStringRegularExpression && ifUsingRegulareExpressionThenUseMatchValueAsDocumentTile)
                        {
                            outputDocument.Info.Title = matchStrings[documentCount];
                        }
                        else
                        {
                            outputDocument.Info.Title = inputDocument.Info.Title;
                        }

                        if (documentCount < pageNumbers.Length)
                        {
                            nextSplitPageNumber = pageNumbers[documentCount];
                        }

                        documentCount++;
                    }

                    outputDocument.AddPage(inputDocument.Pages[i]);

                    if (i + 1 == inputDocument.PageCount)
                    {
                        outputDocument.Save(outputMs);
                        outputMsList.Add(outputMs);
                    }
                }
            }

            return outputMsList;
        }
    }
}