using SharpCompress.Archives.GZip;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Tar;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.IO;

namespace JBToolkit.Zip
{
    /// <summary>
    /// Extract other types of compressed archive file other than .zip. Support the follow file types:
    /// .zip, .7z, .tar, .gzip, .tar, .tar.gz, .gzip, .rar
    /// 
    /// Notice '.zip' is in there. This class offers more modern support. For example for extracting
    /// LZMA2 compression types.     
    /// </summary>
    public static class ExtractOtherArchiveType
    {
        /// <summary>
        /// First looks at file type extension, then loops through the different extractor methods
        /// to see if one works, and will throw an exception at the end if it's unable to extract
        /// the archive.
        /// </summary>
        /// <param name="archiveFilePath">Full path to compressed archive file</param>
        /// <param name="outputDirectory">Will create output directory if missing</param>
        public static void ExtractArchiveAuto(string archiveFilePath, string outputDirectory)
        {
            bool extracted = false;
            try
            {
                if (Path.GetExtension(archiveFilePath).ToLower() == ".zip")
                {
                    ExtractZip_BetterCompatibility(archiveFilePath, outputDirectory);
                    extracted = true;
                }

                else if (Path.GetExtension(archiveFilePath).ToLower() == ".7z")
                {
                    ExtractSevenZip(archiveFilePath, outputDirectory);
                    extracted = true;
                }

                else if (Path.GetExtension(archiveFilePath).ToLower() == ".tar" ||
                         Path.GetExtension(archiveFilePath).ToLower() == ".tar.gz")
                {
                    ExtractTar(archiveFilePath, outputDirectory);
                    extracted = true;
                }

                else if (Path.GetExtension(archiveFilePath).ToLower() == ".gzip" ||
                         Path.GetExtension(archiveFilePath).ToLower() == ".gz")
                {
                    ExtractGzip(archiveFilePath, outputDirectory);
                    extracted = true;
                }

                else if (Path.GetExtension(archiveFilePath).ToLower() == ".rar")
                {
                    ExtractRar(archiveFilePath, outputDirectory);
                    extracted = true;
                }
            }
            catch { }

            if (extracted)
                return;
            else
            {
                try
                {
                    ExtractZip_BetterCompatibility(archiveFilePath, outputDirectory);
                }
                catch (Exception e1)
                {
                    try
                    {
                        ExtractSevenZip(archiveFilePath, outputDirectory);
                    }
                    catch (Exception e2)
                    {
                        try
                        {
                            ExtractTar(archiveFilePath, outputDirectory);
                        }
                        catch (Exception e3)
                        {
                            try
                            {
                                ExtractGzip(archiveFilePath, outputDirectory);
                            }
                            catch (Exception e4)
                            {
                                try
                                {
                                    ExtractRar(archiveFilePath, outputDirectory);
                                }
                                catch (Exception e5)
                                {
                                    throw new ApplicationException(string.Format(
                                        @"Unable to extract archive. Attempted with the following methods compression types: 
                                        .zip, .7z, .tar, .gzip, .tar, .tar.gz, .gzip, .rar . Exception thrown are: 
                                        {0} :: {1} :: {2} :: {3} :: {4}",
                                        e1.Message,
                                        e2.Message,
                                        e3.Message,
                                        e4.Message,
                                        e5.Message));
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// .7z extension
        /// </summary>
        /// <param name="archiveFilePath">Full path to compressed archive file</param>
        /// <param name="outputDirectory">Will create output directory if missing</param>
        public static void ExtractSevenZip(string archiveFilePath, string outputDirectory)
        {
            using (var archive = SevenZipArchive.Open(archiveFilePath))
            {
                using (var reader = archive.ExtractAllEntries())
                {
                    var options = new ExtractionOptions
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    };

                    if (!Directory.Exists(outputDirectory))
                    {
                        try
                        {
                            Directory.CreateDirectory(outputDirectory);
                        }
                        catch { }
                    }

                    reader.WriteAllToDirectory(outputDirectory, options);
                }
            }
        }

        /// <summary>
        /// .rar extension
        /// </summary>
        /// <param name="archiveFilePath">Full path to compressed archive file</param>
        /// <param name="outputDirectory">Will create output directory if missing</param>
        public static void ExtractRar(string archiveFilePath, string outputDirectory)
        {
            using (var archive = RarArchive.Open(archiveFilePath))
            {
                using (var reader = archive.ExtractAllEntries())
                {
                    var options = new ExtractionOptions
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    };

                    if (!Directory.Exists(outputDirectory))
                    {
                        try
                        {
                            Directory.CreateDirectory(outputDirectory);
                        }
                        catch { }
                    }

                    reader.WriteAllToDirectory(outputDirectory, options);
                }
            }
        }

        /// <summary>
        /// .tar / .tar.gz extension
        /// </summary>
        /// <param name="archiveFilePath">Full path to compressed archive file</param>
        /// <param name="outputDirectory">Will create output directory if missing</param>
        public static void ExtractTar(string archiveFilePath, string outputDirectory)
        {
            using (var archive = TarArchive.Open(archiveFilePath))
            {
                using (var reader = archive.ExtractAllEntries())
                {
                    var options = new ExtractionOptions
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    };

                    if (!Directory.Exists(outputDirectory))
                    {
                        try
                        {
                            Directory.CreateDirectory(outputDirectory);
                        }
                        catch { }
                    }

                    reader.WriteAllToDirectory(outputDirectory, options);
                }
            }
        }

        /// <summary>
        /// .gzip / .gz extension
        /// </summary>
        /// <param name="archiveFilePath">Full path to compressed archive file</param>
        /// <param name="outputDirectory">Will create output directory if missing</param>
        public static void ExtractGzip(string archiveFilePath, string outputDirectory)
        {
            using (var archive = GZipArchive.Open(archiveFilePath))
            {
                using (var reader = archive.ExtractAllEntries())
                {
                    var options = new ExtractionOptions
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    };

                    if (!Directory.Exists(outputDirectory))
                    {
                        try
                        {
                            Directory.CreateDirectory(outputDirectory);
                        }
                        catch { }
                    }

                    reader.WriteAllToDirectory(outputDirectory, options);
                }
            }
        }

        /// <summary>
        /// .zip extension... Can extract LZMA2 compression types.    
        /// </summary>
        public static void ExtractZip_BetterCompatibility(string archiveFilePath, string outputDirectory)
        {
            using (var archive = ZipArchive.Open(archiveFilePath))
            {
                using (var reader = archive.ExtractAllEntries())
                {
                    var options = new ExtractionOptions
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    };

                    if (!Directory.Exists(outputDirectory))
                    {
                        try
                        {
                            Directory.CreateDirectory(outputDirectory);
                        }
                        catch { }
                    }

                    reader.WriteAllToDirectory(outputDirectory, options);
                }
            }
        }
    }
}
