using System;

namespace JBToolkit.Testing
{
    class Program
    {
        static void Main()
        {
            JBToolkit.PdfDoc.PdfConverter.ConvertToPDF(@"c:\temp\Test.docx", @"c:\temp\Test.pdf");
        }
    }
}
