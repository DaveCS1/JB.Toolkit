using JBToolkit.AssemblyHelper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace JBToolkit.Images
{
    /// <summary>
    /// Image manipulation helper methods
    /// </summary>
    public partial class ImageHelper
    {
        /// <summary>
        /// A Helper class for multi-resolution icon files
        /// </summary>
        public class IconHelper
        {
            /// <summary>
            /// Converts a Image to a icon (ico) with all the sizes windows likes
            /// </summary>
            /// <param name="inputBitmap">The input bitmap</param>
            /// <param name="outputStream">The output stream</param>
            /// <returns>Wether or not the icon was succesfully generated</returns>
            public static bool ConvertToIcon(Bitmap inputBitmap, Stream outputStream)
            {
                if (inputBitmap == null)
                {
                    return false;
                }

                int[] sizes = new int[] { 256, 128, 64, 48, 32, 16 };

                // Generate bitmaps for all the sizes and toss them in streams
                List<MemoryStream> imageStreams = new List<MemoryStream>();
                foreach (int size in sizes)
                {
                    Bitmap newBitmap = (Bitmap)ResizeImageMaintainAspectRatio(inputBitmap, size, size);
                    if (newBitmap == null)
                    {
                        return false;
                    }

                    MemoryStream memoryStream = new MemoryStream();
                    newBitmap.Save(memoryStream, ImageFormat.Png);
                    imageStreams.Add(memoryStream);
                }

                BinaryWriter iconWriter = new BinaryWriter(outputStream);
                if (outputStream == null || iconWriter == null)
                {
                    return false;
                }

                int offset = 0;

                // 0-1 reserved, 0
                iconWriter.Write((byte)0);
                iconWriter.Write((byte)0);

                // 2-3 image type, 1 = icon, 2 = cursor
                iconWriter.Write((short)1);

                // 4-5 number of images
                iconWriter.Write((short)sizes.Length);

                offset += 6 + (16 * sizes.Length);

                for (int i = 0; i < sizes.Length; i++)
                {
                    // image entry 1
                    // 0 image width
                    iconWriter.Write((byte)sizes[i]);
                    // 1 image height
                    iconWriter.Write((byte)sizes[i]);

                    // 2 number of colors
                    iconWriter.Write((byte)0);

                    // 3 reserved
                    iconWriter.Write((byte)0);

                    // 4-5 color planes
                    iconWriter.Write((short)0);

                    // 6-7 bits per pixel
                    iconWriter.Write((short)32);

                    // 8-11 size of image data
                    iconWriter.Write((int)imageStreams[i].Length);

                    // 12-15 offset of image data
                    iconWriter.Write(offset);

                    offset += (int)imageStreams[i].Length;
                }

                for (int i = 0; i < sizes.Length; i++)
                {
                    // write image data
                    // png data must contain the whole png data file
                    iconWriter.Write(imageStreams[i].ToArray());
                    imageStreams[i].Close();
                }

                iconWriter.Flush();

                return true;
            }

            /// <summary>
            /// Converts an image to a icon (ico)
            /// </summary>
            /// <param name="input">The input stream</param>
            /// <returns>Wether or not the icon was succesfully generated</returns>
            public static MemoryStream ConvertToIcon(Stream input)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    Bitmap inputBitmap = (Bitmap)Bitmap.FromStream(input);
                    ConvertToIcon(inputBitmap, ms);

                    return ms;
                }
            }

            /// <summary>
            /// Converts a an image to a icon (ico)
            /// </summary>
            /// <param name="inputPath">The input path</param>
            /// <param name="outputPath">The output path</param>
            /// <returns>Wether or not the icon was succesfully generated</returns>
            public static void ConvertToIcon(string inputPath, string outputPath)
            {
                using (FileStream inputStream = new FileStream(inputPath, FileMode.Open))
                using (MemoryStream ms = ConvertToIcon(inputStream))
                {
                    File.WriteAllBytes(outputPath, ms.ToArray());
                }
            }

            /// <summary>
            /// Converts a an image to a icon (ico)
            /// </summary>
            /// <param name="inputBytes">The input byte array </param>
            /// <param name="outputBytes">The output bye array</param>
            /// <returns>Wether or not the icon was succesfully generated</returns>
            public static byte[] ConvertToIcon(byte[] inputBytes)
            {
                using (MemoryStream inputStream = new MemoryStream(inputBytes))
                using (MemoryStream outputStream = new MemoryStream(inputBytes))
                {
                    return ConvertToIcon(inputStream).ToArray();
                }
            }

            /// <summary>
            /// Converts an image to a icon (ico)
            /// </summary>
            /// <param name="inputImage">The input image</param>
            /// <param name="outputPath">The output path</param>
            /// <returns>Wether or not the icon was succesfully generated</returns>
            public static bool ConvertToIcon(System.Drawing.Image inputImage, string outputPath)
            {
                using (FileStream outputStream = new FileStream(outputPath, FileMode.OpenOrCreate))
                {
                    return ConvertToIcon(new Bitmap(inputImage), outputStream);
                }
            }

            public static Image GetLargestImageFromIcon(System.Drawing.Icon icon)
            {
                Icon cIcon = new Icon(icon);
                System.Drawing.Icon lIcon = cIcon.FindIcon(Icon.DisplayType.Largest);

                return lIcon.ToBitmap();
            }

            public static Image GetLargestImageFromIcon(byte[] bytes)
            {
                Icon cIcon = new Icon(bytes);
                System.Drawing.Icon lIcon = cIcon.FindIcon(Icon.DisplayType.Largest);

                return lIcon.ToBitmap();
            }

            public static Image GetLargestImageFromIcon(MemoryStream ms)
            {
                Icon cIcon = new Icon(ms);
                System.Drawing.Icon lIcon = cIcon.FindIcon(Icon.DisplayType.Largest);

                return lIcon.ToBitmap();
            }


            public static Image GetSmallestImageFromIcon(System.Drawing.Icon icon)
            {
                Icon cIcon = new Icon(icon);
                System.Drawing.Icon lIcon = cIcon.FindIcon(Icon.DisplayType.Smallest);

                return lIcon.ToBitmap();
            }

            public static Image GetSmallestImageFromIcon(byte[] bytes)
            {
                Icon cIcon = new Icon(bytes);
                System.Drawing.Icon lIcon = cIcon.FindIcon(Icon.DisplayType.Smallest);

                return lIcon.ToBitmap();
            }

            public static Image GetSmallestImageFromIcon(MemoryStream ms)
            {
                Icon cIcon = new Icon(ms);
                System.Drawing.Icon lIcon = cIcon.FindIcon(Icon.DisplayType.Smallest);

                return lIcon.ToBitmap();
            }

            /// <summary>
            /// Returns an icon based on Window's current file association
            /// </summary>
            /// <param name="fileExtension">I.e. .pdf</param>
            public static System.Drawing.Icon GetWindowsFileAssociationIcon(string fileExtension)
            {
                if (!fileExtension.StartsWith("."))
                    fileExtension = "." + fileExtension;

                string tempPath = Windows.DirectoryHelper.GetTempFile() + fileExtension;
                File.WriteAllText(tempPath, "");

                var icon = System.Drawing.Icon.ExtractAssociatedIcon(tempPath);

                try
                {
                    File.Delete(tempPath);
                }
                catch { }

                return icon;
            }

            /// <summary>
            /// Returns an icon based on Window's current file association
            /// </summary>
            /// <param name="fileExtension">I.e. .pdf</param>
            public static System.Drawing.Image GetFileTypeImageFromAsembly(string fileExtension)
            {
                fileExtension = fileExtension.Replace(".", "");
                Image image = null;

                try
                {
                    image = EmbeddedResourceHelper.GetEmbeddedResourceImage(
                        fileExtension + ".png",
                        "Dependencies_Embedded.Images.FileTypes",
                        EmbeddedResourceHelper.TargetAssemblyType.Executing);
                }
                catch
                { }

                if (image == null)
                {
                    try
                    {
                        // Fallback

                        var icon = GetWindowsFileAssociationIcon(fileExtension);
                        image = icon.ToBitmap();
                    }
                    catch { }
                }

                if (image == null)
                {
                    // Can't find anything... Return blank image

                    image = EmbeddedResourceHelper.GetEmbeddedResourceImage(
                        "_blank.png",
                        "Dependencies_Embedded.Images.FileTypes",
                        EmbeddedResourceHelper.TargetAssemblyType.Executing);
                }

                return image;
            }

            /// <summary>
            /// Extension of the System.Drawing.Icon for getting header information and individsual icons from a multi-resolution icon
            /// </summary>
            public sealed class Icon : IDisposable
            {
                private bool debug = true;
                private MemoryStream icoStream;
                private IconHeader icoHeader;

                public enum DisplayType { Largest = 0, Smallest = 1 }

                /// <summary>
                /// Stored the headers for the icon
                /// </summary>
                private class IconHeader
                {
                    public short Reserved;
                    public short Type;
                    public short Count;

                    public IconHeader(MemoryStream icoStream)
                    {
                        BinaryReader icoFile = new BinaryReader(icoStream);

                        Reserved = icoFile.ReadInt16();
                        Type = icoFile.ReadInt16();
                        Count = icoFile.ReadInt16();
                    }
                }

                /// <summary>
                /// Each icon if the file has its own header with information, stored in this object
                /// </summary>
                public class IconEntry
                {
                    public byte Width;
                    public byte Height;
                    public byte ColorCount;
                    public byte Reserved;
                    public short Planes;
                    public short BitCount;
                    public int BytesInRes;
                    public int ImageOffset;

                    public IconEntry(MemoryStream icoStream)
                    {
                        BinaryReader icoFile = new BinaryReader(icoStream);

                        Width = icoFile.ReadByte();
                        Height = icoFile.ReadByte();
                        ColorCount = icoFile.ReadByte();
                        Reserved = icoFile.ReadByte();
                        Planes = icoFile.ReadInt16();
                        BitCount = icoFile.ReadInt16();
                        BytesInRes = icoFile.ReadInt32();
                        ImageOffset = icoFile.ReadInt32();
                    }
                }

                /// <summary>
                /// Loads the icon file into the memory stream. Returns false on error
                /// </summary>
                private bool LoadFile(string filename)
                {
                    try
                    {
                        FileStream icoFile = new FileStream(filename, FileMode.Open, FileAccess.Read);
                        byte[] icoArray = new byte[icoFile.Length];
                        icoFile.Read(icoArray, 0, (int)icoFile.Length);
                        icoStream = new MemoryStream(icoArray);
                    }
                    catch { return false; }
                    finally { }

                    return true;
                }

                /// <summary>
                /// Loads the icon stream into the memory stream. Returns false on error
                /// </summary>
                private bool LoadStream(MemoryStream ms)
                {
                    try
                    {
                        byte[] icoArray = new byte[ms.Length];
                        ms.Read(icoArray, 0, (int)ms.Length);
                        icoStream = new MemoryStream(icoArray);
                    }
                    catch { return false; }
                    finally { }

                    return true;
                }

                /// <summary>
                /// Loads the icon stream into the memory stream. Returns false on error
                /// </summary>
                private bool LoadIcon(System.Drawing.Icon icon)
                {
                    try
                    {
                        icon.Save(icoStream);

                    }
                    catch { return false; }
                    finally { }

                    return true;
                }

                /// <summary>
                /// Loads the icon bytes into the memory stream. Returns false on error
                /// </summary>
                private bool LoadBytes(byte[] bytes)
                {
                    try
                    {
                        icoStream = new MemoryStream(bytes);
                    }
                    catch { return false; }
                    finally { }

                    return true;
                }

                /// <summary>
                /// Returns a System.Drawing.Icon from the array list of a multi-resolution icon
                /// </summary>
                public System.Drawing.Icon BuildIcon(int index)
                {
                    IconEntry thisIcon = IconsInfo[index];
                    MemoryStream newIcon = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(newIcon);

                    // New Values
                    short newNumber = 1;
                    int newOffset = 22;

                    // Write it
                    writer.Write(icoHeader.Reserved);
                    writer.Write(icoHeader.Type);
                    writer.Write(newNumber);
                    writer.Write(thisIcon.Width);
                    writer.Write(thisIcon.Height);
                    writer.Write(thisIcon.ColorCount);
                    writer.Write(thisIcon.Reserved);
                    writer.Write(thisIcon.Planes);
                    writer.Write(thisIcon.BitCount);
                    writer.Write(thisIcon.BytesInRes);
                    writer.Write(newOffset);

                    // Grab the icon
                    byte[] tmpBuffer = new byte[thisIcon.BytesInRes];
                    icoStream.Position = thisIcon.ImageOffset;
                    icoStream.Read(tmpBuffer, 0, thisIcon.BytesInRes);
                    writer.Write(tmpBuffer);

                    // Finish up
                    writer.Flush();
                    newIcon.Position = 0;
                    return new System.Drawing.Icon(newIcon, thisIcon.Width, thisIcon.Height);
                }

                private System.Drawing.Icon SearchIcons(DisplayType searchKey)
                {
                    int foundIndex = 0;
                    int counter = 0;

                    foreach (IconEntry thisIcon in IconsInfo)
                    {
                        IconEntry current = IconsInfo[foundIndex];

                        if (searchKey == DisplayType.Largest)
                        {
                            if (thisIcon.Width > current.Width && thisIcon.Height > current.Height)
                            { foundIndex = counter; }
                            if (debug)
                            {
                                Console.Write("Search for the largest");
                            }
                        }
                        else
                        {
                            if (thisIcon.Width < current.Width && thisIcon.Height < current.Height)
                            { foundIndex = counter; }
                            if (debug)
                            {
                                Console.Write("Search for the smallest");
                            }
                        }

                        counter++;
                    }

                    return BuildIcon(foundIndex);
                }

                /// <summary>
                /// Information on each imame in the icon object
                /// </summary>
                public List<IconEntry> IconsInfo { get; }

                /// <summary>
                /// Get the largest of small icon based on icon size
                /// </summary>
                public System.Drawing.Icon FindIcon(DisplayType displayType) { return SearchIcons(displayType); }

                public int Count { get { return IconsInfo.Count; } }

                /// <summary>
                /// Loads the icon file into the memory stream
                /// </summary>
                public Icon(string filename)
                {
                    IconsInfo = new List<IconEntry>();

                    // Load the icon Header
                    if (LoadFile(filename))
                    {
                        icoHeader = new IconHeader(icoStream);
                        if (debug) { Console.WriteLine("There are {0} images in this icon file", icoHeader.Count); }

                        // Read the icons
                        for (int counter = 0; counter < icoHeader.Count; counter++)
                        {
                            IconEntry entry = new IconEntry(icoStream);
                            IconsInfo.Add(entry);
                        }
                    }
                }

                /// <summary>
                /// Loads the icon from memory stream
                /// </summary>
                public Icon(MemoryStream ms)
                {
                    IconsInfo = new List<IconEntry>();

                    // Load the icon Header
                    if (LoadStream(ms))
                    {
                        icoHeader = new IconHeader(icoStream);
                        if (debug) { Console.WriteLine("There are {0} images in this icon file", icoHeader.Count); }

                        // Read the icons
                        for (int counter = 0; counter < icoHeader.Count; counter++)
                        {
                            IconEntry entry = new IconEntry(icoStream);
                            IconsInfo.Add(entry);
                        }
                    }
                }

                /// <summary>
                /// Loads the icon from bytes
                /// </summary>
                public Icon(byte[] bytes)
                {
                    IconsInfo = new List<IconEntry>();

                    // Load the icon Header
                    if (LoadBytes(bytes))
                    {
                        icoHeader = new IconHeader(icoStream);
                        if (debug) { Console.WriteLine("There are {0} images in this icon file", icoHeader.Count); }

                        // Read the icons
                        for (int counter = 0; counter < icoHeader.Count; counter++)
                        {
                            IconEntry entry = new IconEntry(icoStream);
                            IconsInfo.Add(entry);
                        }
                    }
                }

                /// <summary>
                /// Loads the icon from bytes
                /// </summary>
                public Icon(System.Drawing.Icon icon)
                {
                    IconsInfo = new List<IconEntry>();

                    // Load the icon Header
                    if (LoadIcon(icon))
                    {
                        icoHeader = new IconHeader(icoStream);
                        if (debug) { Console.WriteLine("There are {0} images in this icon file", icoHeader.Count); }

                        // Read the icons
                        for (int counter = 0; counter < icoHeader.Count; counter++)
                        {
                            IconEntry entry = new IconEntry(icoStream);
                            IconsInfo.Add(entry);
                        }
                    }
                }

#pragma warning disable CA1063 // Implement IDisposable Correctly
                public void Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
                {
                    icoStream.Dispose();
                    Dispose();
                    GC.SuppressFinalize(this);
                }
            }
        }
    }
}
