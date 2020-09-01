using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace JBToolkit.AssemblyHelper
{
    /// <summary>
    /// Methods for extracting and utilising resources embedded in the DLL (including assemblies and command line utilities)
    /// </summary>
    public static class EmbeddedResourceHelper
    {
        public enum TargetAssemblyType
        {
            Calling,
            Executing
        }

        /// <summary>
        /// Returns the embedded resource if it's present in the working folder or if it's been extracted. If it's not present it will extract the embedded resource
        /// to the users temp folder and return the full path to it
        /// </summary>
        /// <param name="fileName">Filename of embedded resource. I.e. CsvHelper.dll</param>
        /// <param name="resourcePath">i.e. the folder as a namename excluding the assembly name. I.e. Dependencies.Helpers</param>
        /// <param name="targetAssemblyType">I.e. Calling or Executing</param>
        /// <param name="skipHashCheckOnTempPath">Skips hash check on extracted resource</param>
        /// <returns>Full path to present or extracted directory</returns>
        public static string GetEmbeddedResourcePath(
            string fileName,
            string resourcePath,
            TargetAssemblyType targetAssemblyType,
            bool skipHashCheckOnTempPath = false)
        {
            Assembly assembly;
            if (targetAssemblyType == TargetAssemblyType.Calling)
                assembly = Assembly.GetCallingAssembly();
            else
                assembly = Assembly.GetExecutingAssembly();

            return GetEmbeddedResourcePath(assembly, fileName, resourcePath, skipHashCheckOnTempPath);
        }

        /// <summary>
        /// Returns the embedded resource if it's present in the working folder or if it's been extracted. If it's not present it will extract the embedded resource
        /// to the users temp folder and return the full path to it
        /// </summary>
        /// <param name="fileName">Filename of embedded resource. I.e. CsvHelper.dll</param>
        /// <param name="resourcePath">i.e. the folder as a namename excluding the assembly name. I.e. Dependencies.Helpers</param>
        /// <param name="targetAssembly">A given assembly (i.e. You can do Assembly.GetCallingAssembly() or Assembly.GetExecutingAssembly()</param>
        /// <param name="skipHashCheckOnTempPath">Skips hash check on extracted resource</param>
        /// <returns>Full path to present or extracted directory</returns>
        public static string GetEmbeddedResourcePath(
            Assembly targetAssembly,
            string fileName,
            string resourcePath,
            bool skipHashCheckOnTempPath = false)
        {
            string executingFolder = Path.GetDirectoryName(targetAssembly.Location);
            string filePath = Path.Combine(executingFolder, fileName);

            if (File.Exists(filePath))
                return filePath;

            string tempPath = Path.GetTempPath();
            filePath = Path.Combine(tempPath, fileName);

            if (File.Exists(filePath))
            {
                if (skipHashCheckOnTempPath)
                    return filePath;
                else
                {
                    // File comparison check
                    using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
                    {
                        byte[] ba = null;

                        bool resourcePresent = true;
                        string targetAssemblyName = targetAssembly.GetName().Name;
                        if (targetAssemblyName == "JB.Toolkit")
                            targetAssemblyName = "JBToolkit";

                        using (Stream stm = targetAssembly.GetManifestResourceStream(targetAssemblyName + "." + resourcePath + "." + fileName))
                        {
                            try
                            {
                                ba = new byte[(int)stm.Length];
                                stm.Read(ba, 0, (int)stm.Length);
                            }
                            catch
                            {
                                // Comparison check won't work if we've zipped it... Just continue
                                resourcePresent = false;
                            }
                        }

                        if (resourcePresent)
                        {
                            string fileHash = BitConverter.ToString(sha1.ComputeHash(ba)).Replace("-", string.Empty);

                            byte[] bb = File.ReadAllBytes(filePath);
                            string fileHash2 = BitConverter.ToString(sha1.ComputeHash(bb)).Replace("-", string.Empty);

                            if (fileHash != fileHash2)
                                ExtractEmbeddedResource(targetAssembly, fileName, resourcePath, Path.GetTempPath(), fileName);
                        }
                        else
                            ExtractEmbeddedResource(targetAssembly, fileName, resourcePath, Path.GetTempPath(), fileName);
                    }
                }
            }
            else
                ExtractEmbeddedResource(targetAssembly, fileName, resourcePath, Path.GetTempPath(), fileName);

            if (File.Exists(filePath))
                return filePath;

            throw new Exception("Cannot find embedded resource '" + targetAssembly.GetName().Name.Replace("-", "_") + "." + resourcePath + "." + fileName);
        }

        /// <summary>
        /// Extracts an embedded resource to a given location
        /// </summary>
        /// <param name="fileName">Filename of embedded resource. I.e. CsvHelper.dll</param>
        /// <param name="resourcePath">i.e. the folder as a namename excluding the assembly name. I.e. Dependencies.Helpers</param>
        /// <param name="outputDirectory">The parent directory to outoput to</param>
        /// <param name="outputFilename">The filename to give the extracted resources. Use null to default to the present filename</param>
        /// <param name="targetAssemblyType">I.e Calling or executing</param>
        public static void ExtractEmbeddedResource(
            string fileName,
            string resourcePath,
            string outputDirectory,
            string outputFilename,
            TargetAssemblyType targetAssemblyType)
        {
            Assembly assembly;
            if (targetAssemblyType == TargetAssemblyType.Calling)
                assembly = Assembly.GetCallingAssembly();
            else
                assembly = Assembly.GetExecutingAssembly();

            ExtractEmbeddedResource(assembly, fileName, resourcePath, outputDirectory, outputFilename);
        }

        /// <summary>
        /// Extracts an embedded resource to a given location
        /// </summary>
        /// <param name="fileName">Filename of embedded resource. I.e. CsvHelper.dll</param>
        /// <param name="resourcePath">i.e. the folder as a namename excluding the assembly name. I.e. Dependencies.Helpers</param>
        /// <param name="outputDirectory">The parent directory to outoput to</param>
        /// <param name="outputFilename">The filename to give the extracted resources. Use null to default to the present filename</param>
        /// <param name="targetAssembly">A given assembly (i.e. You can do Assembly.GetCallingAssembly() or Assembly.GetExecutingAssembly()</param>
        public static void ExtractEmbeddedResource(
            Assembly targetAssembly,
            string fileName,
            string resourcePath,
            string outputDirectory,
            string outputFilename)
        {
            bool resourceExists = true;
            string targetAssemblyName = targetAssembly.GetName().Name;
            if (targetAssemblyName == "JB.Toolkit")
                targetAssemblyName = "JBToolkit";

            using (Stream s = targetAssembly.GetManifestResourceStream(targetAssemblyName.Replace("-", "_") + "." + resourcePath + "." + fileName))
            {
                if (s != null)
                {
                    byte[] buffer = new byte[s.Length];
                    s.Read(buffer, 0, buffer.Length);

                    using (BinaryWriter sw = new BinaryWriter(File.Open(Path.Combine(outputDirectory, string.IsNullOrEmpty(outputFilename) ? fileName : outputFilename), FileMode.Create)))
                        sw.Write(buffer);

                    return;
                }
                else
                {
                    resourceExists = false;
                }
            }

            if (!resourceExists)
            {
                // Perhaps we've zipped it?

                string zippedFilename = Path.ChangeExtension(fileName, "zip");
                using (Stream z = targetAssembly.GetManifestResourceStream(targetAssemblyName.Replace("-", "_") + "." + resourcePath + "." + zippedFilename))
                {
                    if (z == null)
                        throw new Exception("Cannot find embedded resource '" + fileName + "'");

                    Zip.ZipExtraction.ExtractToDisk(z, outputDirectory);

                    return;
                }
            }

            throw new Exception("Cannot find embedded resource '" + targetAssemblyName.Replace("-", "_") + "." + resourcePath + "." + fileName);
        }

        /// <summary>
        /// Extracts an embedded resource to a memory stream
        /// </summary>
        /// <param name="fileName">Filename of embedded resource. I.e. CsvHelper.dll</param>
        /// <param name="resourcePath">i.e. the folder as a namename excluding the assembly name. I.e. Dependencies.Helpers</param>
        /// <param name="targetAssembly">A given assembly (i.e. You can do Assembly.GetCallingAssembly() or Assembly.GetExecutingAssembly()</param>
        public static Stream GetEmbeddedResourceStream(
            Assembly targetAssembly,
            string fileName,
            string resourcePath)
        {
            bool resourceExists = true;
            string targetAssemblyName = targetAssembly.GetName().Name;
            if (targetAssemblyName == "JB.Toolkit")
                targetAssemblyName = "JBToolkit";

            using (Stream s = targetAssembly.GetManifestResourceStream(targetAssemblyName.Replace("-", "_") + "." + resourcePath + "." + fileName))
            {
                if (s != null)
                {
                    byte[] buffer = new byte[s.Length];
                    s.Read(buffer, 0, buffer.Length);

                    return s;
                }
                else
                    resourceExists = false;
            }

            if (!resourceExists)
            {
                // Perhaps we've zipped it?

                string zippedFilename = Path.ChangeExtension(resourcePath, "zip");
                using (Stream z = targetAssembly.GetManifestResourceStream(targetAssemblyName.Replace("-", "_") + "." + resourcePath + "." + zippedFilename))
                {
                    if (z == null)
                        throw new Exception("Cannot find embedded resource '" + resourcePath + "'");

                    var bytesCol = Zip.ZipExtraction.ExtractToFilePathAndBytesCollection(z);

                    return new MemoryStream(bytesCol[0].Bytes);
                }
            }

            throw new Exception("Cannot find embedded resource '" + targetAssemblyName.Replace("-", "_") + "." + resourcePath + "." + fileName);
        }

        /// <summary>
        /// Extracts an embedded resource to a memory stream
        /// </summary>
        /// <param name="fileName">Filename of embedded resource. I.e. CsvHelper.dll</param>
        /// <param name="resourcePath">i.e. the folder as a namename excluding the assembly name. I.e. Dependencies.Helpers</param>
        /// <param name="targetAssemblyType">I.e Calling or executing</param>
        public static Stream GetEmbeddedResourceStream(
            string fileName,
            string resourcePath,
            TargetAssemblyType targetAssemblyType)
        {
            Assembly assembly;
            if (targetAssemblyType == TargetAssemblyType.Calling)
                assembly = Assembly.GetCallingAssembly();
            else
                assembly = Assembly.GetExecutingAssembly();

            return GetEmbeddedResourceStream(assembly, fileName, resourcePath);
        }

        /// <summary>
        /// Get an embedded image from the assmebly
        /// </summary>
        /// <param name="fileName">Filename of embedded resource. I.e. favicon.ico</param>
        /// <param name="resourcePath">i.e. the folder as a namename excluding the assembly name. I.e. Dependencies.Helpers</param>
        /// <param name="targetAssemblyType">I.e Calling or executing</param>
        public static System.Drawing.Image GetEmbeddedResourceImage(
            string fileName,
            string resourcePath,
            TargetAssemblyType targetAssemblyType)
        {
            Assembly assembly;
            if (targetAssemblyType == TargetAssemblyType.Calling)
                assembly = Assembly.GetCallingAssembly();
            else
                assembly = Assembly.GetExecutingAssembly();

            return GetEmbeddedResourceImage(assembly, fileName, resourcePath);
        }

        /// <summary>
        /// Get an embedded image from the assmebly
        /// </summary>
        /// <param name="fileName">Filename of embedded resource. I.e. favicon.ico</param>
        /// <param name="resourcePath">i.e. the folder as a namename excluding the assembly name. I.e. Dependencies.Helpers</param>
        /// <param name="targetAssembly">A given assembly (i.e. You can do Assembly.GetCallingAssembly() or Assembly.GetExecutingAssembly()</param>
        public static System.Drawing.Image GetEmbeddedResourceImage(
            Assembly targetAssembly,
            string fileName,
            string resourcePath)
        {
            bool resourceExists = true;
            string targetAssemblyName = targetAssembly.GetName().Name;
            if (targetAssemblyName == "JB.Toolkit")
                targetAssemblyName = "JBToolkit";

            using (Stream s = targetAssembly.GetManifestResourceStream(targetAssemblyName.Replace("-", "_") + "." + resourcePath + "." + fileName))
            {
                if (s != null)
                {
                    byte[] buffer = new byte[s.Length];
                    s.Read(buffer, 0, buffer.Length);

                    return System.Drawing.Image.FromStream(s);
                }
                else
                    resourceExists = false;
            }

            if (!resourceExists)
            {
                // Perhaps we've zipped it?

                string zippedFilename = Path.ChangeExtension(resourcePath, "zip");
                using (Stream z = targetAssembly.GetManifestResourceStream(targetAssemblyName.Replace("-", "_") + "." + resourcePath + "." + zippedFilename))
                {
                    if (z == null)
                        throw new Exception("Cannot find embedded resource '" + resourcePath + "'");

                    var bytesCol = Zip.ZipExtraction.ExtractToFilePathAndBytesCollection(z);

                    return Image.FromStream(new MemoryStream(bytesCol[0].Bytes));
                }
            }

            throw new Exception("Cannot find embedded resource '" + targetAssemblyName.Replace("-", "_") + "." + resourcePath + "." + fileName);
        }

        /// <summary>
        /// Returns the embedded resource if it's present in the working folder or if it's been extracted. If it's not present it will extract the embedded resource
        /// to the users temp folder and return the full path to it. This is only used when calling from within the JBToolkit as the executing assembly.
        /// </summary>
        /// <param name="fileName">Filename of embedded resource. I.e. CsvHelper.dll</param>
        /// <param name="skipHashCheckOnTempPath">Skips hash check on extracted resource</param>
        /// <returns>Full path to present or extracted directory</returns>
        public static string InternalGetEmbeddedResourcePathFromJBToolkit(string fileName, bool skipHashCheckOnTempPath = false)
        {
            string filePath;
            try
            {
                // May not have permission?

                string executingFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                filePath = Path.Combine(executingFolder, fileName);

                if (File.Exists(filePath))
                    return filePath;
            }
            catch { }

            string tempPath = Path.GetTempPath();
            filePath = Path.Combine(tempPath, fileName);

            if (File.Exists(filePath))
            {
                if (skipHashCheckOnTempPath)
                    return filePath;
                else
                {
                    // File comparison check
                    using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
                    {
                        byte[] ba = null;
                        Assembly curAsm = Assembly.GetExecutingAssembly();
                        bool resourcePresent = true;

                        using (Stream stm = curAsm.GetManifestResourceStream("JBToolkit.Dependencies_Embedded." + fileName))
                        {
                            try
                            {
                                ba = new byte[(int)stm.Length];
                                stm.Read(ba, 0, (int)stm.Length);
                            }
                            catch
                            {
                                // Comparison check won't work if we've zipped it... Just continue
                                resourcePresent = false;
                            }
                        }

                        if (resourcePresent)
                        {
                            string fileHash = BitConverter.ToString(sha1.ComputeHash(ba)).Replace("-", string.Empty);

                            byte[] bb = File.ReadAllBytes(filePath);
                            string fileHash2 = BitConverter.ToString(sha1.ComputeHash(bb)).Replace("-", string.Empty);

                            if (fileHash != fileHash2)
                                ExtractEmbeddedResource(Assembly.GetExecutingAssembly(), fileName, "Dependencies_Embedded", Path.GetTempPath(), fileName);
                        }
                        else
                            ExtractEmbeddedResource(Assembly.GetExecutingAssembly(), fileName, "Dependencies_Embedded", Path.GetTempPath(), fileName);
                    }
                }
            }
            else
                ExtractEmbeddedResource(Assembly.GetExecutingAssembly(), fileName, "Dependencies_Embedded", Path.GetTempPath(), fileName);

            if (File.Exists(filePath))
                return filePath;

            return null;
        }

        /// <summary>
        /// Gets an assembly (if it's loaded) by name
        /// </summary>
        public static Assembly GetAssemblyByName(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies().
                   SingleOrDefault(assembly => assembly.GetName().Name == name);
        }
    }
}
