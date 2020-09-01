using System.IO;
using System.Web;

namespace JBToolkit.Web
{
    /// <summary>
    /// Inherits from 'HttpPostedFileBase' - Used to create email attachments, and takes a Stream or MemoryStream)
    /// </summary>
    public class MemoryFile : HttpPostedFileBase
    {
        Stream stream;
        readonly string contentType;
        readonly string fileName;

        /// <summary>
        /// Stream or MemoryStream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="contentTypeMimeString">MIME Type i.e. 'application/pdf' or 'image/png'</param>
        /// <param name="fileName">Given filename for the email attachment</param>
        public MemoryFile(Stream stream, string contentTypeMimeString, string fileName)
        {
            this.stream = stream;
            this.contentType = contentTypeMimeString;
            this.fileName = fileName;
        }

        public override int ContentLength
        {
            get { return (int)stream.Length; }
        }

        public override string ContentType
        {
            get { return contentType; }
        }

        public override string FileName
        {
            get { return fileName; }
        }

        public override Stream InputStream
        {
            get { return stream; }
        }

        /// <summary>
        /// Save a memory stream to file
        /// </summary>
        /// <param name="filename"></param>
        public override void SaveAs(string filename)
        {
            using (var memoryStream = new MemoryStream())
            {
                this.stream.CopyTo(memoryStream);
                File.WriteAllBytes(filename, memoryStream.ToArray());
            }
        }

        /// <summary>
        /// Gets a stream from a remote URL file location
        /// </summary>
        /// <param name="url"></param>
        public static Stream GetStreamFromUrl(string url)
        {
            return Http.GetStreamFromUrl(url);
        }
    }
}
