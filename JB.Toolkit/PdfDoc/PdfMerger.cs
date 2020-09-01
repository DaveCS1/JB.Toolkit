using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.IO;

namespace JBToolkit.PdfDoc
{
    /// <summary>
    /// Methods to merge PDF documents together, input either memory stream or path (or both) and output as memory stream or save to file
    /// </summary>
    public class PdfMerger
    {
        public static MemoryStream Merge(MemoryStream doc1, MemoryStream doc2)
        {
            MemoryStream ms = new MemoryStream();

            using (PdfDocument one = PdfReader.Open(doc1, PdfDocumentOpenMode.Import))
            using (PdfDocument two = PdfReader.Open(doc2, PdfDocumentOpenMode.Import))
            using (PdfDocument outPdf = new PdfDocument())
            {
                CopyPages(one, outPdf);
                CopyPages(two, outPdf);

                outPdf.Save(ms);

                return ms;
            }
        }

        public static MemoryStream Merge(params MemoryStream[] docs)
        {
            MemoryStream ms = new MemoryStream();
            using (PdfDocument outPdf = new PdfDocument())
            {
                foreach (var document in docs)
                    using (PdfDocument doc = PdfReader.Open(document, PdfDocumentOpenMode.Import))
                        CopyPages(doc, outPdf);

                outPdf.Save(ms);

                return ms;
            }
        }

        public static byte[] Merge(byte[] doc1, byte[] doc2)
        {
            MemoryStream ms = new MemoryStream();

            using (MemoryStream doc1ms = new MemoryStream(doc1))
            using (MemoryStream doc2ms = new MemoryStream(doc2))
            using (PdfDocument one = PdfReader.Open(doc1ms, PdfDocumentOpenMode.Import))
            using (PdfDocument two = PdfReader.Open(doc2ms, PdfDocumentOpenMode.Import))
            using (PdfDocument outPdf = new PdfDocument())
            {
                CopyPages(one, outPdf);
                CopyPages(two, outPdf);

                outPdf.Save(ms);

                return ms.ToArray();
            }
        }

        public static byte[] Merge(params byte[][] docs)
        {
            MemoryStream ms = new MemoryStream();
            using (PdfDocument outPdf = new PdfDocument())
            {
                foreach (var document in docs)
                    using (MemoryStream doc1ms = new MemoryStream(document))
                    using (PdfDocument doc = PdfReader.Open(doc1ms, PdfDocumentOpenMode.Import))
                        CopyPages(doc, outPdf);

                outPdf.Save(ms);

                return ms.ToArray();
            }
        }

        public static MemoryStream Merge(MemoryStream doc1, string doc2)
        {
            MemoryStream ms = new MemoryStream();
            using (PdfDocument one = PdfReader.Open(doc1, PdfDocumentOpenMode.Import))
            using (PdfDocument two = PdfReader.Open(doc2, PdfDocumentOpenMode.Import))
            using (PdfDocument outPdf = new PdfDocument())
            {
                CopyPages(one, outPdf);
                CopyPages(two, outPdf);

                outPdf.Save(ms);

                return ms;
            }
        }

        public static MemoryStream Merge(string doc1, MemoryStream doc2)
        {
            MemoryStream ms = new MemoryStream();
            using (PdfDocument one = PdfReader.Open(doc1, PdfDocumentOpenMode.Import))
            using (PdfDocument two = PdfReader.Open(doc2, PdfDocumentOpenMode.Import))
            using (PdfDocument outPdf = new PdfDocument())
            {
                CopyPages(one, outPdf);
                CopyPages(two, outPdf);

                outPdf.Save(ms);

                return ms;
            }
        }

        public static MemoryStream Merge(string doc1, string doc2)
        {
            MemoryStream ms = new MemoryStream();
            using (PdfDocument one = PdfReader.Open(doc1, PdfDocumentOpenMode.Import))
            using (PdfDocument two = PdfReader.Open(doc2, PdfDocumentOpenMode.Import))
            using (PdfDocument outPdf = new PdfDocument())
            {
                CopyPages(one, outPdf);
                CopyPages(two, outPdf);

                outPdf.Save(ms);

                return ms;
            }
        }

        public static MemoryStream Merge(params string[] docPaths)
        {
            MemoryStream ms = new MemoryStream();
            using (PdfDocument outPdf = new PdfDocument())
            {
                foreach (var document in docPaths)
                    using (PdfDocument doc = PdfReader.Open(document, PdfDocumentOpenMode.Import))
                        CopyPages(doc, outPdf);

                outPdf.Save(ms);

                return ms;
            }
        }

        public static void Merge(MemoryStream doc1, MemoryStream doc2, string outputPath)
        {
            using (PdfDocument one = PdfReader.Open(doc1, PdfDocumentOpenMode.Import))
            using (PdfDocument two = PdfReader.Open(doc2, PdfDocumentOpenMode.Import))
            using (PdfDocument outPdf = new PdfDocument())
            {
                CopyPages(one, outPdf);
                CopyPages(two, outPdf);

                outPdf.Save(outputPath);
            }
        }

        public static void Merge(string outputPath, params MemoryStream[] docs)
        {
            using (PdfDocument outPdf = new PdfDocument())
            {
                foreach (var document in docs)
                    using (PdfDocument doc = PdfReader.Open(document, PdfDocumentOpenMode.Import))
                        CopyPages(doc, outPdf);

                outPdf.Save(outputPath);
            }
        }

        public static void Merge(string doc1, string doc2, string outputPath)
        {
            using (PdfDocument one = PdfReader.Open(doc1, PdfDocumentOpenMode.Import))
            using (PdfDocument two = PdfReader.Open(doc2, PdfDocumentOpenMode.Import))
            using (PdfDocument outPdf = new PdfDocument())
            {
                CopyPages(one, outPdf);
                CopyPages(two, outPdf);

                outPdf.Save(outputPath);
            }
        }

        public static void Merge(string outputPath, params string[] docPaths)
        {
            using (PdfDocument outPdf = new PdfDocument())
            {
                foreach (var document in docPaths)
                    using (PdfDocument doc = PdfReader.Open(document, PdfDocumentOpenMode.Import))
                        CopyPages(doc, outPdf);

                outPdf.Save(outputPath);
            }
        }

        public static void Merge(MemoryStream doc1, string doc2, string outputPath)
        {
            using (PdfDocument one = PdfReader.Open(doc1, PdfDocumentOpenMode.Import))
            using (PdfDocument two = PdfReader.Open(doc2, PdfDocumentOpenMode.Import))
            using (PdfDocument outPdf = new PdfDocument())
            {
                CopyPages(one, outPdf);
                CopyPages(two, outPdf);

                outPdf.Save(outputPath);
            }
        }

        public static void Merge(string doc1, MemoryStream doc2, string outputPath)
        {
            using (PdfDocument one = PdfReader.Open(doc1, PdfDocumentOpenMode.Import))
            using (PdfDocument two = PdfReader.Open(doc2, PdfDocumentOpenMode.Import))
            using (PdfDocument outPdf = new PdfDocument())
            {
                CopyPages(one, outPdf);
                CopyPages(two, outPdf);

                outPdf.Save(outputPath);
            }
        }

        private static void CopyPages(PdfDocument from, PdfDocument to)
        {
            for (int i = 0; i < from.PageCount; i++)
            {
                to.AddPage(from.Pages[i]);
            }
        }
    }
}
