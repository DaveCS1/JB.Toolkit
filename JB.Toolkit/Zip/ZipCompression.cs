using Ionic.Zip;
using Ionic.Zlib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace JBToolkit.Zip
{
    /// <summary>
    /// A basic dictionary of the filename to save within the zip file and the bytes of that file
    /// </summary>
    public class FilePathAndBytesCollection : List<FilePathAndBytes> { }

    /// <summary>
    /// Filename to store within zip file and it's file as a byte array
    /// </summary>
    public class FilePathAndBytes
    {
        public string Filename { get; set; }
        public byte[] Bytes { get; set; }
    }

    public enum ZipCompressionLevel
    {
        None,
        Fast,
        Best,
        Level0,
        Level1,
        Level2,
        Level3,
        Level4,
        Level5,
        Level6,
        Level7,
        Level8,
        Level9
    }

    public enum ZipCompressionMethod
    {
        None,
        Deflate,
        BZip2
    }

    /// <summary>
    /// Contains multiple method for saving zip files or returning a zip file as a stream
    /// </summary>
    public class ZipCompression
    {
        public static ZipCompressionLevel ZipCompressionLevel { get; set; } = ZipCompressionLevel.Fast;
        public static ZipCompressionMethod ZipCompressionMethod { get; set; } = ZipCompressionMethod.Deflate;

        private static CompressionLevel GetCompressionLevel(ZipCompressionLevel zipCompressionLevel)
        {
            switch (zipCompressionLevel)
            {
                case ZipCompressionLevel.None:
                    return CompressionLevel.None;
                case ZipCompressionLevel.Fast:
                    return CompressionLevel.BestSpeed;
                case ZipCompressionLevel.Best:
                    return CompressionLevel.BestCompression;
                case ZipCompressionLevel.Level0:
                    return CompressionLevel.Level0;
                case ZipCompressionLevel.Level1:
                    return CompressionLevel.Level1;
                case ZipCompressionLevel.Level2:
                    return CompressionLevel.Level2;
                case ZipCompressionLevel.Level3:
                    return CompressionLevel.Level3;
                case ZipCompressionLevel.Level4:
                    return CompressionLevel.Level4;
                case ZipCompressionLevel.Level5:
                    return CompressionLevel.Level5;
                case ZipCompressionLevel.Level6:
                    return CompressionLevel.Level6;
                case ZipCompressionLevel.Level7:
                    return CompressionLevel.Level7;
                case ZipCompressionLevel.Level8:
                    return CompressionLevel.Level8;
                case ZipCompressionLevel.Level9:
                    return CompressionLevel.Level9;
                default:
                    return CompressionLevel.BestSpeed;
            }
        }

        private static CompressionMethod GetCompressionMethod(ZipCompressionMethod zipCompressionMethod)
        {
            switch (zipCompressionMethod)
            {
                case ZipCompressionMethod.None:
                    return CompressionMethod.None;
                case ZipCompressionMethod.Deflate:
                    return CompressionMethod.Deflate;
                case ZipCompressionMethod.BZip2:
                    return CompressionMethod.BZip2;
                default:
                    return CompressionMethod.Deflate;
            }
        }

        // ## STREAM - FLAT FILES ##############################################################################################################################################

        /// <summary>
        /// Returns a zip file containing a flat list of files as a Memory Stream. The input being multiple paramters of a FileByte object
        /// </summary>
        public static MemoryStream GetZipFlatFilesStream(
            ZipCompressionLevel compressionLevel = ZipCompressionLevel.Fast,
            ZipCompressionMethod compressionMethod = ZipCompressionMethod.Deflate,
            params FilePathAndBytes[] fileBytes)
        {
            using (ZipFile zip = new ZipFile())
            {
                zip.CompressionMethod = GetCompressionMethod(compressionMethod);
                zip.CompressionLevel = GetCompressionLevel(compressionLevel);
                foreach (var fileByte in fileBytes)
                {
                    zip.AddEntry(fileByte.Filename, fileByte.Bytes);
                }

                MemoryStream output = new MemoryStream();
                zip.Save(output);

                return output;
            }
        }

        /// <summary>
        /// Returns a zip file containing a flat list of files as a Memory Stream. The input being a FileBytes collection object
        /// </summary>
        public static MemoryStream GetZipFlatFilesStream(
            FilePathAndBytesCollection fileBytes,
            ZipCompressionLevel compressionLevel = ZipCompressionLevel.Fast,
            ZipCompressionMethod compressionMethod = ZipCompressionMethod.Deflate)
        {
            using (ZipFile zip = new ZipFile())
            {
                zip.CompressionMethod = GetCompressionMethod(compressionMethod);
                zip.CompressionLevel = GetCompressionLevel(compressionLevel);
                foreach (var fileByte in fileBytes)
                {
                    zip.AddEntry(fileByte.Filename, fileByte.Bytes);
                }

                MemoryStream output = new MemoryStream();
                zip.Save(output);

                return output;
            }
        }

        /// <summary>
        /// Returns a zip file containing a flat list of files as a Memory Stream. The input being multiple paramters of file path strings
        /// </summary>
        public static MemoryStream GetZipFlatFilesStream(
            ZipCompressionLevel compressionLevel = ZipCompressionLevel.Fast,
            ZipCompressionMethod compressionMethod = ZipCompressionMethod.Deflate,
            params string[] filePaths)
        {
            using (ZipFile zip = new ZipFile())
            {
                zip.CompressionMethod = GetCompressionMethod(compressionMethod);
                zip.CompressionLevel = GetCompressionLevel(compressionLevel);
                foreach (var file in filePaths)
                {
                    zip.AddFile(file);
                }

                MemoryStream output = new MemoryStream();
                zip.Save(output);

                return output;
            }
        }

        /// <summary>
        /// Returns a zip file containing a flat list of files as a Memory Stream. The input being a list of file path strings
        /// </summary>
        public static MemoryStream GetZipFlatFilesStream(
            List<string> filePaths,
            ZipCompressionLevel compressionLevel = ZipCompressionLevel.Fast,
            ZipCompressionMethod compressionMethod = ZipCompressionMethod.Deflate)
        {
            using (ZipFile zip = new ZipFile())
            {
                zip.CompressionMethod = GetCompressionMethod(compressionMethod);
                zip.CompressionLevel = GetCompressionLevel(compressionLevel);
                foreach (var file in filePaths)
                {
                    zip.AddFile(file);
                }

                MemoryStream output = new MemoryStream();
                zip.Save(output);

                return output;
            }
        }

        // ## BYTE ARRAY - FLAT FILES ##########################################################################################################################################

        /// <summary>
        /// Returns a zip file containing a flat list of files as a byte array. The input being multiple paramters of a FileByte object
        /// </summary>
        public static byte[] GetZipFlatFilesByteArray(
            ZipCompressionLevel compressionLevel = ZipCompressionLevel.Fast,
            ZipCompressionMethod compressionMethod = ZipCompressionMethod.Deflate,
            params FilePathAndBytes[] fileBytes)
        {
            return GetZipFlatFilesStream(compressionLevel, compressionMethod, fileBytes).ToArray();
        }

        /// <summary>
        /// Returns a zip file containing a flat list of files as a byte array. The input being a FileBytes collection object
        /// </summary>
        public static byte[] GetZipFlatFilesByteArray(
            FilePathAndBytesCollection fileBytes,
            ZipCompressionLevel compressionLevel = ZipCompressionLevel.Fast,
            ZipCompressionMethod compressionMethod = ZipCompressionMethod.Deflate)
        {
            return GetZipFlatFilesStream(fileBytes, compressionLevel, compressionMethod).ToArray();
        }

        /// <summary>
        /// Returns a zip file containing a flat list of files as a byte array. The input being multiple paramters of file path strings
        /// </summary>
        public static byte[] GetZipFlatFilesByteArray(
            ZipCompressionLevel compressionLevel = ZipCompressionLevel.Fast,
            ZipCompressionMethod compressionMethod = ZipCompressionMethod.Deflate,
            params string[] files)
        {
            return GetZipFlatFilesStream(compressionLevel, compressionMethod, files).ToArray();
        }

        /// <summary>
        /// Returns a zip file containing a flat list of files as a byte array. The input being a list of file path strings
        /// </summary>
        public static byte[] GetZipFlatFilesByteArray(
            List<string> filePaths,
            ZipCompressionLevel compressionLevel = ZipCompressionLevel.Fast,
            ZipCompressionMethod compressionMethod = ZipCompressionMethod.Deflate)
        {
            return GetZipFlatFilesStream(filePaths, compressionLevel, compressionMethod).ToArray();
        }

        // ## FILE CONTENT RESULT - FLAT FILES ################################################################################################################################

        /// <summary>
        /// Returns a zip file containing a flat list of files as a FileContentResult (good for MVC file action controller). The input being multiple paramters of a FileByte object
        /// </summary>
        public static FileContentResult GetZipFlatFilesFileContentResult
            (ZipCompressionLevel compressionLevel = ZipCompressionLevel.Fast,
            ZipCompressionMethod compressionMethod = ZipCompressionMethod.Deflate,
            params FilePathAndBytes[] fileBytes)
        {
            return new FileContentResult(GetZipFlatFilesStream(compressionLevel, compressionMethod, fileBytes).ToArray(), "application/zip");
        }

        /// <summary>
        /// Returns a zip file containing a flat list of files as a FileContentResult (good for MVC file action controller). The input being a FileBytes collection object
        /// </summary>
        public static FileContentResult GetZipFlatFilesFileContentResult(
            FilePathAndBytesCollection fileBytes,
            ZipCompressionLevel compressionLevel = ZipCompressionLevel.Fast,
            ZipCompressionMethod compressionMethod = ZipCompressionMethod.Deflate)
        {
            return new FileContentResult(GetZipFlatFilesStream(fileBytes, compressionLevel, compressionMethod).ToArray(), "application/zip");
        }

        /// <summary>
        /// Returns a zip file containing a flat list of files as a FileContentResult (good for MVC file action controller). The input being multiple file path strings
        /// </summary>
        public static FileContentResult GetZipFlatFilesFileContentResult(
            ZipCompressionLevel compressionLevel = ZipCompressionLevel.Fast,
            ZipCompressionMethod compressionMethod = ZipCompressionMethod.Deflate,
            params string[] filePaths)
        {
            return new FileContentResult(GetZipFlatFilesStream(compressionLevel, compressionMethod, filePaths).ToArray(), "application/zip");
        }

        /// <summary>
        /// Returns a zip file containing a flat list of files as a FileContentResult (good for MVC file action controller). The input being a list of file paths
        /// </summary>
        public static FileContentResult GetZipFlatFilesFileContentResult(
            List<string> filePaths,
            ZipCompressionLevel compressionLevel = ZipCompressionLevel.Fast,
            ZipCompressionMethod compressionMethod = ZipCompressionMethod.Deflate)
        {
            return new FileContentResult(GetZipFlatFilesStream(filePaths, compressionLevel, compressionMethod).ToArray(), "application/zip");
        }

        // SAVE TO DISK - FLAT FILES ##########################################################################################################################################

        /// <summary>
        /// Saves a zip file containing a flat list of files to disk. The input being multiple paramters of a FileByte object
        /// </summary>
        public static void SaveZipFlatFiles(string outputPath,
            ZipCompressionLevel compressionLevel = ZipCompressionLevel.Fast,
            ZipCompressionMethod compressionMethod = ZipCompressionMethod.Deflate,
            params FilePathAndBytes[] fileBytes)
        {
            using (ZipFile zip = new ZipFile())
            {
                zip.CompressionMethod = GetCompressionMethod(compressionMethod);
                zip.CompressionLevel = GetCompressionLevel(compressionLevel);
                foreach (var fileByte in fileBytes)
                {
                    zip.AddEntry(fileByte.Filename, fileByte.Bytes);
                }

                zip.Save(outputPath);
            }
        }

        /// <summary>
        /// Saves a zip file containing a flat list of files to disk. The input being a FileBytes collection object
        /// </summary>
        public static void SaveZipFlatFiles(string outputPath,
            FilePathAndBytesCollection fileBytes,
            ZipCompressionLevel compressionLevel = ZipCompressionLevel.Fast,
            ZipCompressionMethod compressionMethod = ZipCompressionMethod.Deflate)
        {
            using (ZipFile zip = new ZipFile())
            {
                zip.CompressionMethod = GetCompressionMethod(compressionMethod);
                zip.CompressionLevel = GetCompressionLevel(compressionLevel);
                foreach (var fileByte in fileBytes)
                {
                    zip.AddEntry(fileByte.Filename, fileByte.Bytes);
                }

                zip.Save(outputPath);
            }
        }

        /// <summary>
        /// Saves a zip file containing a flat list of files to disk. The input being multiple file path strings
        /// </summary>
        public static void SaveZipFlatFiles(string outputPath,
            ZipCompressionLevel compressionLevel = ZipCompressionLevel.Fast,
            ZipCompressionMethod compressionMethod = ZipCompressionMethod.Deflate,
            params string[] filePaths)
        {
            using (ZipFile zip = new ZipFile())
            {
                zip.CompressionMethod = GetCompressionMethod(compressionMethod);
                zip.CompressionLevel = GetCompressionLevel(compressionLevel);
                foreach (var file in filePaths)
                {
                    zip.AddFile(file);
                }

                zip.Save(outputPath);
            }
        }

        /// <summary>
        /// Saves a zip file containing a flat list of files to disk. The input being a list of file paths
        /// </summary>
        public static void SaveZipFlatFiles(string outputPath,
            List<string> filePaths,
            ZipCompressionLevel compressionLevel = ZipCompressionLevel.Fast,
            ZipCompressionMethod compressionMethod = ZipCompressionMethod.Deflate)
        {
            using (ZipFile zip = new ZipFile())
            {
                zip.CompressionMethod = GetCompressionMethod(compressionMethod);
                zip.CompressionLevel = GetCompressionLevel(compressionLevel);
                foreach (var file in filePaths)
                {
                    zip.AddFile(file);
                }

                zip.Save(outputPath);
            }
        }

        // WHOLE DIRECTORY ####################################################################################################################################################

        /// <summary>
        /// Zips and entire directory as a Memory Stream (including sub-directories). The input being the target directory
        /// </summary>
        public static MemoryStream GetZipDirectoryStream(
            string targetRootDirectory,
            ZipCompressionLevel compressionLevel = ZipCompressionLevel.Fast,
            ZipCompressionMethod compressionMethod = ZipCompressionMethod.Deflate)
        {
            using (ZipFile zip = new ZipFile
            {
                CompressionMethod = GetCompressionMethod(compressionMethod),
                CompressionLevel = GetCompressionLevel(compressionLevel)
            })
            {
                var files = Directory.GetFiles(targetRootDirectory, "*",
                    SearchOption.AllDirectories).
                    Where(f => Path.GetExtension(f).
                        ToLowerInvariant() != ".zip").ToArray();

                foreach (var f in files)
                {
                    zip.AddFile(f,
                        Path.GetDirectoryName(f).
                        Replace(targetRootDirectory, string.Empty));
                }


                MemoryStream output = new MemoryStream();
                zip.Save(output);

                return output;
            }
        }

        /// <summary>
        /// Zips and entire directory as a byte array (including sub-directories). The input being the target directory
        /// </summary>
        public static byte[] GetZipDirectoryByteArray(string targetRootDirectory,
            ZipCompressionLevel compressionLevel = ZipCompressionLevel.Fast,
            ZipCompressionMethod compressionMethod = ZipCompressionMethod.Deflate)
        {
            return GetZipDirectoryStream(targetRootDirectory, compressionLevel, compressionMethod).ToArray();
        }

        /// <summary>
        /// Zips and entire directory as FileContentResult (including sub-directories - Good for MVC file action controller). The input being the target directory
        /// </summary>
        public static FileContentResult GetZipDirectoryFileContentResult(
            string targetRootDirectory,
            ZipCompressionLevel compressionLevel = ZipCompressionLevel.Fast,
            ZipCompressionMethod compressionMethod = ZipCompressionMethod.Deflate)
        {
            return new FileContentResult(GetZipDirectoryStream(targetRootDirectory, compressionLevel, compressionMethod).ToArray(), "application/zip");
        }

        /// <summary>
        /// Zips and entire directory (including sub-directories) and saves to the disk. The input being the target directory
        /// </summary>
        public static void SaveZipDirectory(
            string outputFilename,
            string targetRootDirectory,
            ZipCompressionLevel compressionLevel = ZipCompressionLevel.Fast,
            ZipCompressionMethod compressionMethod = ZipCompressionMethod.Deflate)
        {
            using (ZipFile zip = new ZipFile
            {
                CompressionMethod = GetCompressionMethod(compressionMethod),
                CompressionLevel = GetCompressionLevel(compressionLevel)
            })
            {
                var files = Directory.GetFiles(targetRootDirectory, "*",
                    SearchOption.AllDirectories).
                    Where(f => Path.GetExtension(f).
                        ToLowerInvariant() != ".zip").ToArray();

                foreach (var f in files)
                {
                    zip.AddFile(f,
                        Path.GetDirectoryName(f).
                        Replace(targetRootDirectory, string.Empty));
                }

                zip.Save(outputFilename);
            }
        }
    }
}
