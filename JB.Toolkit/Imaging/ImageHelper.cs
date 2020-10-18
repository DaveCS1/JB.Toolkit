using ImageMagick;
using System;
using System.Data.HashFunction.xxHash;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using static JBToolkit.Images.ImageHelper.HtmlImageHelper;

namespace JBToolkit.Images
{
    /// <summary>
    /// Image manipulation helper methods
    /// </summary>
    public partial class ImageHelper
    {
        private static xxHashConfig XxHashConfig = new xxHashConfig() { HashSizeInBits = 64 };
        private static IxxHash XxHashFactory = xxHashFactory.Instance.Create(XxHashConfig);

        public class MimeHelper
        {
            private static int MimeSampleSize = 256;
            private static string DefaultMimeType = "application/octet-stream";

            /// <summary>
            /// If deployed in IIS, must enable 32-bit application
            /// </summary>
            [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
            private extern static uint FindMimeFromData(
                uint pBC,
                [MarshalAs(UnmanagedType.LPStr)] string pwzUrl,
                [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
                uint cbSize,
                [MarshalAs(UnmanagedType.LPStr)] string pwzMimeProposed,
                uint dwMimeFlags,
                out uint ppwzMimeOut,
                uint dwReserverd
            );

            /// <summary>
            /// Gets the mime type from an image byte array
            /// IMPORTANT: Uses urlmon.dll DLLIMPORT -> If you're deployment an app that uses this is IIS, you must enable allow
            /// 32-Bit applications in the AppPool
            /// </summary>
            public static string GetMimeFromBytes(byte[] data)
            {
                try
                {
                    FindMimeFromData(0, null, data, (uint)MimeSampleSize, null, 0, out uint mimeType, 0);

                    var mimePointer = new IntPtr(mimeType);
                    var mime = Marshal.PtrToStringUni(mimePointer);
                    Marshal.FreeCoTaskMem(mimePointer);

                    return mime ?? DefaultMimeType;
                }
                catch
                {
                    return DefaultMimeType;
                }
            }


            /// <summary>
            /// Gets the mime type from a memory stream
            /// IMPORTANT: Uses urlmon.dll DLLIMPORT -> If you're deployment an app that uses this is IIS, you must enable allow
            /// 32-Bit applications in the AppPool
            /// </summary>
            public static string GetMimeFromMemoryStream(MemoryStream ms)
            {
                return GetMimeFromBytes(ms.ToArray());
            }

            /// <summary>
            /// Gets the mime type from a a raw Base64 string (without mime type descriptor)
            /// IMPORTANT: Uses urlmon.dll DLLIMPORT -> If you're deployment an app that uses this is IIS, you must enable allow
            /// 32-Bit applications in the AppPool
            /// </summary>
            public static string GetMimeFromRawBase64String(string rawBase64String)
            {
                try
                {
                    FindMimeFromData(0, null, Convert.FromBase64String(rawBase64String), (uint)MimeSampleSize, null, 0, out uint mimeType, 0);

                    var mimePointer = new IntPtr(mimeType);
                    var mime = Marshal.PtrToStringUni(mimePointer);
                    Marshal.FreeCoTaskMem(mimePointer);

                    return mime ?? DefaultMimeType;
                }
                catch
                {
                    return DefaultMimeType;
                }
            }

            /// <summary>
            /// Returns the MIME type string from a System.Drawing.Image
            /// i.e. image/pdf
            /// </summary>
            public static string GetMimeFromImage(Image image)
            {
                foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
                {
                    if (codec.FormatID == image.RawFormat.Guid)
                        return codec.MimeType;
                }

                return "image/unknown";
            }

            /// <summary>
            /// Returns a base64 string, with detected mime type (using magic numbers) from a image byte array
            /// IMPORTANT: Uses urlmon.dll DLLIMPORT -> If you're deployment an app that uses this is IIS, you must enable allow
            /// 32-Bit applications in the AppPool
            /// </summary>
            public static string GetBase64StringFromBytesWithMime(byte[] imageBytes)
            {
                return "data:" + GetMimeFromBytes(imageBytes) + ";base64," + Convert.ToBase64String(imageBytes);
            }

            /// <summary>
            /// Returns a base64 string, with detected mime type (using magic numbers) from a raw base64 image string (without mime descriptor)
            /// IMPORTANT: Uses urlmon.dll DLLIMPORT -> If you're deployment an app that uses this is IIS, you must enable allow
            /// 32-Bit applications in the AppPool
            /// </summary>
            public static string GetBase64StringWithMimeTypeFromBase64StringWitoutMimeType(string rawBase64String)
            {
                return "data:" + GetMimeFromRawBase64String(rawBase64String) + ";base64," + rawBase64String;
            }
        }

        public class Converter
        {
            /// <summary>
            /// Convert image from one format to another
            /// Requires System.Drawing
            /// </summary>
            public static byte[] ConvertImageFormat(byte[] bytes, ImageFormat newFormat)
            {
                using (var inStream = new MemoryStream(bytes))
                using (var outStream = new MemoryStream())
                {
                    var imageStream = System.Drawing.Image.FromStream(inStream);
                    imageStream.Save(outStream, newFormat);
                    return outStream.ToArray();
                }
            }

            /// <summary>
            /// Convert image from one format to another
            /// Requires System.Drawing
            /// </summary>
            public static System.Drawing.Image ConvertImageFormat(System.Drawing.Image image, ImageFormat newFormat)
            {
                using (var ms = new MemoryStream())
                {
                    image.Save(ms, image.RawFormat);
                    byte[] inputBytes = ms.ToArray();
                    byte[] newBytes = ConvertImageFormat(inputBytes, newFormat);

                    using (var msO = new MemoryStream(newBytes))
                    {
                        return System.Drawing.Image.FromStream(msO);
                    }
                }
            }

            /// <summary>
            /// Convert image from one format to another and save to file
            /// Requires System.Drawing
            /// </summary>
            public static void ConvertImageFormat(string inputPath, string outputPath, ImageFormat newFormat)
            {
                byte[] inputBytes = File.ReadAllBytes(inputPath);
                byte[] outputBytes = ConvertImageFormat(inputBytes, newFormat);
                File.WriteAllBytes(outputPath, outputBytes);
            }

            /// <summary>
            /// Convert image from one format to another and output to byte array. This method uses ImageMagick, and can basically convert 
            /// any image file type to a more common file type, including SVG to png, or something like pjpeg to jpeg.
            /// </summary>
            /// <param name="imageBytes">Input bytes of image</param>
            /// <param name="toExtension">Extension, in order to determine output file type</param>
            /// <param name="removeWhite">Optionally remove white (to make transparent)</param>
            /// <returns>Byte array</returns>
            public static byte[] MagickConvertImageFormat(byte[] imageBytes, string toExtension, bool removeWhite = false)
            {
                return MagickConvertImageFormat(ConvertToMemoryStream(imageBytes), toExtension, removeWhite).ToArray();
            }

            /// <summary>
            /// Convert image from one format to another and output to file. This method uses ImageMagick, and can basically convert 
            /// any image file type to a more common file type, including SVG to png, or something like pjpeg to jpeg.
            /// </summary>
            /// <param name="inputPath">Input path of image</param>
            /// <param name="outputPath">Output path of new image</param>
            /// <param name="removeWhite">Optionally remove white (to make transparent)</param>
            public static void MagickConvertImageFormat(string inputPath, string outputPath, bool removeWhite = false)
            {
                File.WriteAllBytes(outputPath,
                                   MagickConvertImageFormat(
                                       File.ReadAllBytes(inputPath),
                                       Path.GetExtension(outputPath),
                                       removeWhite)
                                   .ToArray());
            }

            /// <summary>
            /// Convert image from one format to another and output to memory stream. This method uses ImageMagick, and can basically convert 
            /// any image file type to a more common file type, including SVG to png, or something like pjpeg to jpeg.
            /// </summary>
            /// <param name="inputMs">Input memory stream of image</param>
            /// <param name="toExtension">Extension, in order to determine output file type</param>
            /// <param name="removeWhite">Optionally remove white (to make transparent)</param>
            /// <returns>Memory stream</returns>
            public static MemoryStream MagickConvertImageFormat(MemoryStream inputMs, string toExtension, bool removeWhite = false)
            {
                var ms = new MemoryStream();

                toExtension = toExtension.ToLower().Replace(".", "");
                toExtension = toExtension.Substring(0, 1).ToUpper() + toExtension.Substring(1, toExtension.Length - 1).ToLower();

                var readSettings = new MagickReadSettings
                {
                    Debug = false,
                    Verbose = false
                };

                using (var tms = new MemoryStream())
                {
                    using (var image = new MagickImage(inputMs, readSettings))
                    {
                        MagickColor mc = new MagickColor(MagickColors.Transparent);
                        image.BackgroundColor = mc;
                        image.Settings.BackgroundColor = mc;
                        image.Transparent(mc);

                        image.Quality = 100;
                        image.Settings.TextAntiAlias = true;
                        image.Settings.StrokeAntiAlias = true;
                        image.Interpolate = PixelInterpolateMethod.Bilinear;

                        if (image.Format == MagickFormat.Ico)
                        {
                            var multiIcon = new IconHelper.Icon(inputMs);
                            var largestIcon = multiIcon.FindIcon(IconHelper.Icon.DisplayType.Largest).ToBitmap();
                            using (var icoImage = new MagickImage(ConvertToByteArray(largestIcon)))
                            {
                                if (toExtension.In("Jpg", "Jpeg"))
                                {
                                    mc = new MagickColor(MagickColors.White);
                                    icoImage.BackgroundColor = mc;
                                    icoImage.Settings.BackgroundColor = mc;
                                    icoImage.Alpha(AlphaOption.Remove);
                                }
                                else
                                {
                                    icoImage.Format = (MagickFormat)Enum.Parse(typeof(MagickFormat), toExtension);
                                    icoImage.BackgroundColor = mc;
                                    icoImage.Settings.BackgroundColor = mc;
                                    icoImage.Transparent(mc);
                                }

                                icoImage.Write(ms);
                            }
                        }
                        else if (toExtension.In("Jpg", "Jpeg"))
                        {
                            mc = new MagickColor(MagickColors.White);
                            image.BackgroundColor = mc;
                            image.Settings.BackgroundColor = mc;
                            image.Alpha(AlphaOption.Remove);
                            image.Format = (MagickFormat)Enum.Parse(typeof(MagickFormat), toExtension);

                            image.Write(ms);
                        }
                        else if (toExtension.In("Ico"))
                        {
                            image.Format = MagickFormat.Png;
                            image.Write(tms);
                        }
                        else if (image.Format == MagickFormat.Svg || image.Format == MagickFormat.Eps || image.Format == MagickFormat.Ai)
                        {
                            mc = new MagickColor(MagickColors.White);
                            image.BackgroundColor = mc;
                            image.Settings.BackgroundColor = mc;
                            image.Transparent(mc);

                            image.Format = (MagickFormat)Enum.Parse(typeof(MagickFormat), toExtension);
                            image.Write(ms);
                        }
                        else if (removeWhite)
                        {
                            mc = new MagickColor(MagickColors.White);
                            image.BackgroundColor = mc;
                            image.Settings.BackgroundColor = mc;
                            image.Transparent(mc);

                            image.Format = (MagickFormat)Enum.Parse(typeof(MagickFormat), toExtension);
                            image.Write(ms);
                        }
                        else
                        {
                            image.Format = (MagickFormat)Enum.Parse(typeof(MagickFormat), toExtension);
                            image.Write(ms);
                        }
                    }

                    if (toExtension.In("Ico"))
                        IconHelper.ConvertToIcon(tms, ms);
                }

                return ms;
            }

            public static byte[] ConvertToByteArray(System.Drawing.Image image)
            {
                using (var ms = new MemoryStream())
                {
                    image.Save(ms, image.RawFormat);
                    return ms.ToArray();
                }
            }

            public static byte[] ConvertToByteArray(MemoryStream ms)
            {
                return ms.ToArray();
            }

            public static byte[] ConvertToByteArray(string base64String)
            {
                return Convert.FromBase64String(base64String);
            }

            public static System.Drawing.Image ConvertToImage(byte[] bytes)
            {
                return System.Drawing.Image.FromStream(new MemoryStream(bytes));
            }

            public static System.Drawing.Image ConvertToImage(MemoryStream ms)
            {
                return ConvertToImage(ms.ToArray());
            }

            public static System.Drawing.Image ConvertToImage(string base64String)
            {
                return ConvertToImage(Convert.FromBase64String(base64String));
            }

            public static MemoryStream ConvertToMemoryStream(System.Drawing.Image image)
            {
                var ms = new MemoryStream();
                image.Save(ms, image.RawFormat);
                return ms;
            }

            public static MemoryStream ConvertToMemoryStream(byte[] imageBytes)
            {
                return new MemoryStream(imageBytes);
            }

            public static MemoryStream ConvertToMemoryStream(string base64String)
            {
                return ConvertToMemoryStream(ConvertToImage(Convert.FromBase64String(base64String)));
            }

            public static string ConvertToBase64String(System.Drawing.Image image, bool includeMimeTypeDescriptor = false)
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();
                    string base64String = Convert.ToBase64String(imageBytes);

                    if (includeMimeTypeDescriptor)
                        return "data:" + ImageHelper.MimeHelper.GetMimeFromImage(image) + ";base64," + base64String;

                    return base64String;
                }
            }

            public static string ConvertToBase64String(byte[] bytes, bool includeMimeTypeDescriptor = false)
            {
                if (includeMimeTypeDescriptor)
                    return ConvertToBase64String(ConvertToImage(bytes), true);

                return ConvertToBase64String(ConvertToImage(bytes));
            }

            public static string ConvertToBase64String(byte[] bytes, ImageFormat format, bool includeMimeTypeDescriptor = false)
            {
                if (includeMimeTypeDescriptor)
                    return "data:image/" + format.ToString().ToLower() + ";base64," + ConvertToBase64String(bytes);
                else
                    return ConvertToBase64String(bytes);
            }

            public static string ConvertToBase64String(MemoryStream ms, bool includeMimeTypeDescriptor = false)
            {
                if (includeMimeTypeDescriptor)
                    return ConvertToBase64String(ConvertToByteArray(ms), true);

                return ConvertToBase64String(ConvertToByteArray(ms));
            }

            public static string ConvertToBase64String(MemoryStream ms, ImageFormat format, bool includeMimeTypeDescriptor = false)
            {
                if (includeMimeTypeDescriptor)
                    return "data:image/" + format.ToString().ToLower() + ";base64," + ConvertToBase64String(ConvertToByteArray(ms));
                else
                    return ConvertToBase64String(ConvertToByteArray(ms));
            }

            public static DataImage ConvertToDataImage(string base64String)
            {
                if (DataImage.TryParse(base64String, out DataImage dataImage))
                    return dataImage;
                else
                    return null;
            }
        }

        /// <summary>
        /// Compresses the image, and also gives the option to resize (max height / width specific while maintaining aspect ration). 
        /// If set to -1 it means to use the current image's width / height;
        /// </summary>
        /// <param name="imageBytes">Byte array of image as input</param>
        /// <param name="quality">0 - 100</param>
        /// <param name="resizeMaxWidth">If set to -1 it means to use the current image's width ;</param>
        /// <param name="resizeMaxHeight">If set to -1 it means to use the current image's height;</param>
        /// <returns>Byte array of imagge</returns>
        public static byte[] CompressImage(
            byte[] imageBytes,
            int quality = 50,
            int resizeMaxWidth = -1,
            int resizeMaxHeight = -1)
        {
            using (var image = new ImageMagick.MagickImage(imageBytes))
            {
                image.Quality = quality;
                image.Strip();

                if (resizeMaxWidth != -1 || resizeMaxHeight != -1)
                {
                    if (resizeMaxWidth == -1 && resizeMaxHeight > -1)
                        resizeMaxWidth = image.Width;

                    if (resizeMaxWidth > -1 && resizeMaxHeight == -1)
                        resizeMaxHeight = image.Height;

                    var resizedSize = GetResizingSize_MaintainAspectRatio(new Size(image.Width, image.Height), resizeMaxWidth, resizeMaxHeight);
                    image.Resize(resizedSize.Width, resizedSize.Height);
                }

                return image.ToByteArray();
            }
        }

        /// <summary>
        /// Compresses the image, and also gives the option to resize (max height / width specific while maintaining aspect ration). 
        /// If set to -1 it means to use the current image's width / height;
        /// </summary>
        /// <param name="inputImage">System.Drawing.Image as input</param>
        /// <param name="quality">0 - 100</param>
        /// <param name="resizeMaxWidth">If set to -1 it means to use the current image's width ;</param>
        /// <param name="resizeMaxHeight">If set to -1 it means to use the current image's height;</param>
        /// <returns>System.Drawing.Image as output</returns>
        public static Image CompressImage(
            Image inputImage,
            int quality = 50,
            int resizeMaxWidth = -1,
            int resizeMaxHeight = -1)
        {
            return Converter.ConvertToImage(
                       CompressImage(
                           Converter.ConvertToByteArray(inputImage),
                           quality,
                           resizeMaxWidth,
                           resizeMaxHeight));
        }


        /// <summary>
        /// Compresses the image, and also gives the option to resize (max height / width specific while maintaining aspect ration). 
        /// If set to -1 it means to use the current image's width / height;
        /// </summary>
        /// <param name="inputImagePath">Input path to image</param>
        /// <param name="quality">0 - 100</param>
        /// <param name="resizeMaxWidth">If set to -1 it means to use the current image's width ;</param>
        /// <param name="resizeMaxHeight">If set to -1 it means to use the current image's height;</param>
        public static void CompressImage(
        string inputImagePath,
        string outputImagePath,
        int quality = 50,
        int resizeMaxWidth = -1,
        int resizeMaxHeight = -1)
        {
            using (var image = new ImageMagick.MagickImage(inputImagePath))
            {
                image.Quality = quality;
                image.Strip();

                if (Path.GetExtension(outputImagePath).ToLower().In("jpg", "jpeg"))
                    image.SetCompression(ImageMagick.CompressionMethod.JPEG);

                if (resizeMaxWidth != -1 || resizeMaxHeight != -1)
                {
                    if (resizeMaxWidth == -1 && resizeMaxHeight > -1)
                        resizeMaxWidth = image.Width;

                    if (resizeMaxWidth > -1 && resizeMaxHeight == -1)
                        resizeMaxHeight = image.Height;

                    var resizedSize = GetResizingSize_MaintainAspectRatio(new Size(image.Width, image.Height), resizeMaxWidth, resizeMaxHeight);
                    image.Resize(resizedSize.Width, resizedSize.Height);
                }

                image.Write(outputImagePath);
            }
        }

        private static Size GetResizingSize_MaintainAspectRatio(
            Size originalSize,
            int maxWidth = 1200,
            int maxHeight = 1200,
            bool enlarge = false)
        {
            if (originalSize.Height < maxHeight && originalSize.Width < maxWidth)
                return originalSize;

            maxWidth = enlarge ? maxWidth : Math.Min(maxWidth, originalSize.Width);
            maxHeight = enlarge ? maxHeight : Math.Min(maxHeight, originalSize.Height);

            decimal rnd = Math.Min(maxWidth / (decimal)originalSize.Width, maxHeight / (decimal)originalSize.Height);
            return new Size((int)Math.Round(originalSize.Width * rnd), (int)Math.Round(originalSize.Height * rnd));
        }

        /// <summary>
        /// Resizes an image based on specific width and height
        /// </summary>
        public static byte[] ResizeImage(byte[] image, int width, int height)
        {
            using (var msO = new MemoryStream(image))
            {
                System.Drawing.Image newImage = ResizeImage(System.Drawing.Image.FromStream(msO), width, height);

                using (var ms = new MemoryStream())
                {
                    newImage.Save(ms, newImage.RawFormat);
                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// Resize an image with specific width and height
        /// </summary>
        public static System.Drawing.Image ResizeImage(System.Drawing.Image image, int width, int height)
        {
            var destRect = new System.Drawing.Rectangle(0, 0, width, height);
            var destImage = new System.Drawing.Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = System.Drawing.Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, System.Drawing.GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        /// <summary>
        /// Resize an image with specific width and height and saved it to file
        /// </summary>
        public static void ResizeImage(string inputPath, string outputPath, int width, int height)
        {
            byte[] inputBytes = File.ReadAllBytes(inputPath);
            byte[] outputBytes = ResizeImage(inputBytes, width, height);
            File.WriteAllBytes(outputPath, outputBytes);
        }

        /// <summary>
        /// Resize an image based on percentage to increase (above 100%) or decrease (below 100%)
        /// </summary>
        public static byte[] ResizeImage(byte[] image, int percentage)
        {
            using (var msO = new MemoryStream(image))
            {
                System.Drawing.Image newImage = ResizeImage(System.Drawing.Image.FromStream(msO), percentage);

                using (var ms = new MemoryStream())
                {
                    newImage.Save(ms, newImage.RawFormat);
                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// Resize an image based on percentage to increase (above 100%) or decrease (below 100%)
        /// </summary>
        public static System.Drawing.Image ResizeImage(System.Drawing.Image image, int percentage)
        {
            //get the height and width of the image
            int originalW = image.Width;
            int originalH = image.Height;

            //get the new size based on the percentage change
            int width = originalW * percentage;
            int height = originalH * percentage;

            var destRect = new System.Drawing.Rectangle(0, 0, width, height);
            var destImage = new System.Drawing.Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = System.Drawing.Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, System.Drawing.GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        /// <summary>
        /// Resizes an image based on specific width and height
        /// </summary>
        public static byte[] ResizeImageMaintainAspectRatio(byte[] image, int width, int height)
        {
            using (var msO = new MemoryStream(image))
            {
                System.Drawing.Image newImage = ResizeImageMaintainAspectRatio(System.Drawing.Image.FromStream(msO), width, height);

                using (var ms = new MemoryStream())
                {
                    newImage.Save(ms, newImage.RawFormat);
                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// Resize an image with specific width and height and saved it to file
        /// </summary>
        public static void ResizeImageMaintainAspectRatio(string inputPath, string outputPath, int width, int height)
        {
            byte[] inputBytes = File.ReadAllBytes(inputPath);
            byte[] outputBytes = ResizeImageMaintainAspectRatio(inputBytes, width, height);
            File.WriteAllBytes(outputPath, outputBytes);
        }

        /// <summary>
        /// Resizes an image and keeps the aspect ratio
        /// </summary>
        public static Image ResizeImageMaintainAspectRatio(Image image, int width, int height)
        {
            int sourceWidth = image.Width;
            int sourceHeight = image.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercentW = width / (float)sourceWidth;
            float nPercentH = height / (float)sourceHeight;
            float nPercent;

            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmpImage = new Bitmap(width, height);
            bmpImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(bmpImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphics.DrawImage(image,
                    new Rectangle(destX, destY, destWidth, destHeight),
                    new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                    GraphicsUnit.Pixel);
            }

            return bmpImage;
        }

        /// <summary>
        /// Adjust the brightness of an image
        /// </summary>
        public static System.Drawing.Image AdjustBrightness(System.Drawing.Image image, float brightness)
        {
            // Make the ColorMatrix.
            float b = brightness;
            ColorMatrix cm = new ColorMatrix(new float[][]
            {
                new float[] {b, 0, 0, 0, 0},
                new float[] {0, b, 0, 0, 0},
                new float[] {0, 0, b, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1},
            });
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(cm);

            // Draw the image onto the new bitmap while applying
            // the new ColorMatrix.
            Point[] points =
            {
                new Point(0, 0),
                new Point(image.Width, 0),
                new Point(0, image.Height),
            };
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

            // Make the result bitmap.
            Bitmap bm = new Bitmap(image.Width, image.Height);
            using (Graphics gr = Graphics.FromImage(bm))
            {
                gr.DrawImage(image, points, rect,
                    GraphicsUnit.Pixel, attributes);
            }

            // Return the result.
            return bm;
        }

        /// <summary>
        /// Lightens an image
        /// </summary>
        public static System.Drawing.Image LightenImage(System.Drawing.Image image)
        {
            Rectangle r = new Rectangle(0, 0, image.Width, image.Height);
            int alpha = 128;
            using (Graphics g = Graphics.FromImage(image))
            {
                using (Brush cloud_brush = new SolidBrush(Color.FromArgb(alpha, Color.White)))
                {
                    g.FillRectangle(cloud_brush, r);
                }
            }

            return image;
        }

        /// <summary>
        /// This looks to see if there's an exif property on the image (meta property on image file that's usually applied
        /// when using the context menu in Windows to rotate an image). If there is it properly rotates the image and removes
        /// the exif meta property.
        /// </summary>
        public static System.Drawing.Image NormalizeOrientation(System.Drawing.Image image)
        {
            int exifOrientationID = 0x112; //274

            if (Array.IndexOf(image.PropertyIdList, exifOrientationID) > -1)
            {
                int orientation;

                orientation = image.GetPropertyItem(exifOrientationID).Value[0];

                if (orientation >= 1 && orientation <= 8)
                {
                    switch (orientation)
                    {
                        case 2:
                            image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                            break;
                        case 3:
                            image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        case 4:
                            image.RotateFlip(RotateFlipType.Rotate180FlipX);
                            break;
                        case 5:
                            image.RotateFlip(RotateFlipType.Rotate90FlipX);
                            break;
                        case 6:
                            image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            break;
                        case 7:
                            image.RotateFlip(RotateFlipType.Rotate270FlipX);
                            break;
                        case 8:
                            image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                    }

                    image.RemovePropertyItem(exifOrientationID);
                }
            }

            return image;
        }

        /// <summary>
        /// Get a 32bit xxHash of an image (very fast and useful for comparing)
        /// </summary>
        public static byte[] GetImageXxHash(Image image)
        {
            var bytes = new byte[1];
            bytes = (byte[])(new ImageConverter()).ConvertTo(image, bytes.GetType());
            return XxHashFactory.ComputeHash(bytes).Hash;
        }

        /// <summary>
        /// Checks if 1 image is the same as another by comparing its SHA hash. Also performs
        /// a quick check on image dimentions to avoid performs cost of unnecessarily generating
        /// a hash
        /// </summary>
        /// <returns>True if the same, false otherwise</returns>
        public static bool IsSameImage(Image imageA, Image imageB)
        {
            if (imageA.Width != imageB.Width) return false;
            if (imageA.Height != imageB.Height) return false;

            var hashA = GetImageXxHash(imageA);
            var hashB = GetImageXxHash(imageB);

            return !hashA
                .Where((nextByte, index) => nextByte != hashB[index])
                .Any();
        }
    }
}
