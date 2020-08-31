using Ionic.Zip;
using System.Collections.Generic;
using System.IO;

namespace JBToolkit.Zip
{
    /// <summary>
    /// Contains multiple method for extracting a zip file to either disk or various streams
    /// </summary>
    public class ZipExtraction
    {
        /// <summary>
        /// Extracts a zip stream to a dictionary of file and Memory Streams
        /// </summary>
        public Dictionary<string, MemoryStream> ExtractToMemoryStreamDictionary(Stream targFileStream)
        {
            Dictionary<string, MemoryStream> files = new Dictionary<string, MemoryStream>();

            using (ZipFile zip = ZipFile.Read(targFileStream))
            {
                foreach (ZipEntry zEntry in zip)
                {
                    MemoryStream tempS = new MemoryStream();
                    zEntry.Extract(tempS);

                    files.Add(zEntry.FileName, tempS);
                }
            }

            return files;
        }

        /// <summary>
        /// Extracts a zip file to a dictionary of file and Memory Streams
        /// </summary>
        public Dictionary<string, MemoryStream> ExtractToMemoryStreamDictionary(string zipFilePath)
        {
            Dictionary<string, MemoryStream> files = new Dictionary<string, MemoryStream>();

            using (ZipFile zip = ZipFile.Read(zipFilePath))
            {
                foreach (ZipEntry zEntry in zip)
                {
                    MemoryStream tempS = new MemoryStream();
                    zEntry.Extract(tempS);

                    files.Add(zEntry.FileName, tempS);
                }
            }

            return files;
        }

        /// <summary>
        /// Extracts a zip stream to a dictionary of file and byte arrays
        /// </summary>
        public static Dictionary<string, byte[]> ExtractToByteArrayDictionary(Stream targetStream)
        {
            Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();

            using (ZipFile zip = ZipFile.Read(targetStream))
            {
                foreach (ZipEntry zEntry in zip)
                {
                    MemoryStream tempS = new MemoryStream();
                    zEntry.Extract(tempS);

                    files.Add(zEntry.FileName, tempS.ToArray());
                }
            }

            return files;
        }

        /// <summary>
        /// Extracts a zip stream to a FilePathAndBytes collection object
        /// </summary>
        public static FilePathAndBytesCollection ExtractToFilePathAndBytesCollection(Stream targetStream)
        {
            FilePathAndBytesCollection files = new FilePathAndBytesCollection();

            using (ZipFile zip = ZipFile.Read(targetStream))
            {
                foreach (ZipEntry zEntry in zip)
                {
                    MemoryStream tempS = new MemoryStream();
                    zEntry.Extract(tempS);

                    files.Add(new FilePathAndBytes
                    {
                        Filename = zEntry.FileName,
                        Bytes = tempS.ToArray()
                    });
                }
            }

            return files;
        }

        /// <summary>
        /// Extracts a zip file to a FilePathAndBytes collection object
        /// </summary>
        public static FilePathAndBytesCollection ExtractToFilePathAndBytesCollection(string zipFilePath)
        {
            FilePathAndBytesCollection files = new FilePathAndBytesCollection();

            using (ZipFile zip = ZipFile.Read(zipFilePath))
            {
                foreach (ZipEntry zEntry in zip)
                {
                    MemoryStream tempS = new MemoryStream();
                    zEntry.Extract(tempS);

                    files.Add(new FilePathAndBytes
                    {
                        Filename = zEntry.FileName,
                        Bytes = tempS.ToArray()
                    });
                }
            }

            return files;
        }

        /// <summary>
        /// Extract a zip file to a dictionary of file paths and byte arrays
        /// </summary>
        public static Dictionary<string, byte[]> ExtractToByteArrayDictionary(string zipFilePath)
        {
            Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();

            using (ZipFile zip = ZipFile.Read(zipFilePath))
            {
                foreach (ZipEntry zEntry in zip)
                {
                    MemoryStream tempS = new MemoryStream();
                    zEntry.Extract(tempS);

                    files.Add(zEntry.FileName, tempS.ToArray());
                }
            }

            return files;
        }

        /// <summary>
        /// Extract a zip stream to disk
        /// </summary>
        public static void ExtractToDisk(Stream zipStream, string targetPath)
        {
            using (ZipFile zip = ZipFile.Read(zipStream))
            {
                zip.ExtractAll(targetPath, ExtractExistingFileAction.OverwriteSilently);
            }
        }

        /// <summary>
        /// Extract a zip file to disk
        /// </summary>
        public static void ExtractToDisk(string zipPath, string targetPath)
        {
            using (ZipFile zip = ZipFile.Read(zipPath))
            {
                zip.ExtractAll(targetPath, ExtractExistingFileAction.OverwriteSilently);
            }
        }
    }
}
