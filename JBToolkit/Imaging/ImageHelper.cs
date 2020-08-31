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
        #region MimeType

        public static int MimeSampleSize = 256;

        public static string DefaultMimeType = "application/octet-stream";

        private static xxHashConfig XxHashConfig = new xxHashConfig() { HashSizeInBits = 64 };
        private static IxxHash XxHashFactory = xxHashFactory.Instance.Create(XxHashConfig);

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

        #endregion

        #region Image Manipulation

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
        /// Convert image from one format to another and save to file
        /// Requires System.Drawing
        /// </summary>
        public static void SaveImageAsFormat(string inputPath, string outputPath, ImageFormat newFormat)
        {
            byte[] inputBytes = File.ReadAllBytes(inputPath);
            byte[] outputBytes = ConvertImageFormat(inputBytes, newFormat);
            File.WriteAllBytes(outputPath, outputBytes);
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
        public static void ResizedImage(string inputPath, string outputPath, int width, int height)
        {
            byte[] inputBytes = File.ReadAllBytes(inputPath);
            byte[] outputBytes = ResizeImage(inputBytes, width, height);
            File.WriteAllBytes(outputPath, outputBytes);
        }

        /// <summary>
        /// Resize an image with specific width and height and saved it to file
        /// </summary>
        public static void SaveResizedImage(string inputPath, string outputPath, int width, int height)
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
        /// Resize an image based on percentage to increase (above 100%) or decrease (below 100%) and saves it to file
        /// </summary>
        public static void SaveResizedImage(string inputPath, string outputPath, int percentage)
        {
            byte[] inputBytes = File.ReadAllBytes(inputPath);
            byte[] outputBytes = ResizeImage(inputBytes, percentage);
            File.WriteAllBytes(outputPath, outputBytes);
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
        /// Resize an image with specific width and height and saved it to file
        /// </summary>
        public static void SaveResizedImageMaintainAspectRatio(string inputPath, string outputPath, int width, int height)
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
        /// Compress Jpgeg image
        /// </summary>
        /// <param name="quality">Percentage. I.e. 0 = zero quality level, 50 = 50% quality, 100 = no  </param>
        public static byte[] CompressJpgeg(byte[] data, long quality = 50L)
        {
            var jpgEncoder = GetEncoder(ImageFormat.Jpeg);

            using (var inStream = new MemoryStream(data))
            using (var outStream = new MemoryStream())
            {
                var image = System.Drawing.Image.FromStream(inStream);

                // if we aren't able to retrieve our encoder
                // we should just save the current image and
                // return to prevent any exceptions from happening
                if (jpgEncoder == null)
                {
                    image.Save(outStream, ImageFormat.Jpeg);
                }
                else
                {
                    var qualityEncoder = Encoder.Quality;

                    var encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = new EncoderParameter(qualityEncoder, quality);
                    image.Save(outStream, jpgEncoder, encoderParameters);
                }

                return outStream.ToArray();
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return null;
        }

        #endregion

        #region Image Comparison

        /// <summary>
        /// Get a 32bit xxHash of an image (useful for comparing)
        /// </summary>
        public static byte[] ImageXxHash(Image image)
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

            var hashA = ImageXxHash(imageA);
            var hashB = ImageXxHash(imageB);

            return !hashA
                .Where((nextByte, index) => nextByte != hashB[index])
                .Any();
        }

        #endregion

        #region Image Data Type Conversion

        public static byte[] ConvertImageToByteArray(System.Drawing.Image image)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }

        public static System.Drawing.Image ConvertByteArrayToImage(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                return System.Drawing.Image.FromStream(ms);
            }
        }

        public static MemoryStream ConvertImageToMemoryStream(System.Drawing.Image image)
        {
            using (MemoryStream m = new MemoryStream())
            {
                image.Save(m, image.RawFormat);
                return m;
            }
        }

        public static byte[] ConvertMemoryStreamToByteArray(MemoryStream ms)
        {
            return ms.ToArray();
        }

        public static System.Drawing.Image ConvertMemoryStreamToImage(MemoryStream ms)
        {
            return ConvertByteArrayToImage(ms.ToArray());
        }

        public static string ConvertImageToBase64String(System.Drawing.Image image, bool includeMimeTypeDescriptor = false)
        {
            using (MemoryStream m = new MemoryStream())
            {
                image.Save(m, image.RawFormat);
                byte[] imageBytes = m.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);

                if (includeMimeTypeDescriptor)
                    return "data:" + GetMimeFromImage(image) + ";base64," + base64String;

                return base64String;
            }
        }

        public static string ConvertImageToBase64String(byte[] bytes, ImageFormat format, bool includeMimeTypeDescriptor = false)
        {
            if (includeMimeTypeDescriptor)
                return "data:image/" + format.ToString().ToLower() + ";base64," + ConvertImageToBase64String(ConvertByteArrayToImage(bytes));
            else
                return ConvertImageToBase64String(ConvertByteArrayToImage(bytes));
        }

        public static string ConvertByteArrayToBase64String(byte[] bytes, bool includeMimeTypeDescriptor = false)
        {
            if (includeMimeTypeDescriptor)
                return ConvertImageToBase64String(ConvertByteArrayToImage(bytes), true);

            return ConvertImageToBase64String(ConvertByteArrayToImage(bytes));
        }

        public static string ConvertByteArrayToBase64String(byte[] bytes, ImageFormat format, bool includeMimeTypeDescriptor = false)
        {
            if (includeMimeTypeDescriptor)
                return "data:image/" + format.ToString().ToLower() + ";base64," + ConvertByteArrayToBase64String(bytes);
            else
                return ConvertByteArrayToBase64String(bytes);
        }

        public static string ConvertMemoryStreamToBase64String(MemoryStream ms, bool includeMimeTypeDescriptor = false)
        {
            if (includeMimeTypeDescriptor)
                return ConvertByteArrayToBase64String(ConvertMemoryStreamToByteArray(ms), true);

            return ConvertByteArrayToBase64String(ConvertMemoryStreamToByteArray(ms));
        }

        public static string ConvertMemoryStreamToBase64String(MemoryStream ms, ImageFormat format, bool includeMimeTypeDescriptor = false)
        {
            if (includeMimeTypeDescriptor)
                return "data:image/" + format.ToString().ToLower() + ";base64," + ConvertByteArrayToBase64String(ConvertMemoryStreamToByteArray(ms));
            else
                return ConvertByteArrayToBase64String(ConvertMemoryStreamToByteArray(ms));
        }

        public static byte[] ConvertBase64StringToByteArray(string base64String)
        {
            return Convert.FromBase64String(base64String);
        }

        public static System.Drawing.Image ConvertBase64StringToImage(string base64String)
        {
            return ConvertByteArrayToImage(Convert.FromBase64String(base64String));
        }

        public static MemoryStream ConvertBase64StringToMemoryStream(string base64String)
        {
            return ConvertImageToMemoryStream(ConvertByteArrayToImage(Convert.FromBase64String(base64String)));
        }

        public static DataImage ConvertBase64StringToDataImage(string base64String)
        {
            if (DataImage.TryParse(base64String, out DataImage dataImage))
                return dataImage;
            else
                return null;
        }

        #endregion

        #region Image Orientation

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

        #endregion
    }
}
