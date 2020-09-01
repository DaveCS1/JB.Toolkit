using Google.Cloud.Vision.V1;
using JBToolkit.PdfDoc;
using JBToolkit.Windows;
using System;
using System.IO;

namespace JBToolkit.GoogeApi
{
    /// <summary>
    /// Perform OCR operation using Google's Vision API
    /// </summary>
    public class Vision
    {
        /// <summary>
        /// Extract text from an a PDF (converts to PNG first)
        /// </summary>
        /// <param name="path">Path of PDF file to extract text from</param>
        /// <param name="timoutSeconds">Timeout in seconds before reporting failure</param>
        /// <param name="throwOnError">Fail (break) or error or just return empty string (useful when using this method in batch operations)</param>
        /// <returns>Text from PDF</returns>
        public static string GetTextFromPdf(
            string path,
            GoogleApiImageToTextType imageToTextType = GoogleApiImageToTextType.Document,
            int timoutSeconds = 30,
            bool throwOnError = true)
        {
            SetGoogleApiCredentialEnvironmentVariable();
            var content = string.Empty;

            try
            {
                var client = ImageAnnotatorClient.Create();

                string text = string.Empty;

                foreach (byte[] imageBytes in PdfConverter.ConvertPdfToImage2DByteArray(path, true, timoutSeconds))
                {
                    var image = Image.FromBytes(imageBytes);

                    if (imageToTextType == GoogleApiImageToTextType.Document)
                    {
                        var annotation = client.DetectDocumentText(image);
                        return annotation.Text;
                    }
                    else
                    {
                        var response = client.DetectText(image);
                        foreach (var annotation in response)
                        {
                            if (annotation.Description != null)
                            {
                                text += annotation.Description;
                            }
                        }

                        return text;
                    }
                }

                return text;
            }
            catch (Exception e)
            {
                if (throwOnError)
                {
                    throw new ApplicationException("Unable to parse using Google Vision API: " + e.Message);
                }

                return content;
            }
        }

        /// <summary>
        /// Extract text from an a PDF (converts to PNG first)
        /// </summary>
        /// <param name="path">Path of PDF file to extract text from</param>
        /// <param name="timoutSeconds">Timeout in seconds before reporting failure</param>
        /// <param name="throwOnError">Fail (break) or error or just return empty string (useful when using this method in batch operations)</param>
        /// <returns>Text from PDF</returns>
        public static string GetTextFromPdf(
            MemoryStream ms,
            GoogleApiImageToTextType imageToTextType =
            GoogleApiImageToTextType.Document,
            int timoutSeconds = 30,
            bool throwOnError = true)
        {
            SetGoogleApiCredentialEnvironmentVariable();
            var content = string.Empty;

            string path = Path.Combine(DirectoryHelper.GetTempPath(), DirectoryHelper.GetTempFile() + ".pdf");
            File.WriteAllBytes(path, ms.ToArray());

            try
            {
                var client = ImageAnnotatorClient.Create();

                string text = string.Empty;

                foreach (byte[] imageBytes in PdfConverter.ConvertPdfToImage2DByteArray(path, true, timoutSeconds))
                {
                    var image = Image.FromBytes(imageBytes);

                    if (imageToTextType == GoogleApiImageToTextType.Document)
                    {
                        var annotation = client.DetectDocumentText(image);
                        return annotation.Text;
                    }
                    else
                    {
                        var response = client.DetectText(image);
                        foreach (var annotation in response)
                        {
                            if (annotation.Description != null)
                            {
                                text += annotation.Description;
                            }
                        }

                        return text;
                    }
                }

                try
                {
                    File.Delete(path);
                }
                catch { }

                return text;
            }
            catch (Exception e)
            {
                try
                {
                    File.Delete(path);
                }
                catch { }

                if (throwOnError)
                {
                    throw new ApplicationException("Unable to parse using Google Vision API: " + e.Message);
                }

                return content;
            }
        }

        public enum GoogleApiImageToTextType
        {
            Document,
            Generic
        }

        /// <summary>
        /// Extracts text from an Image
        /// </summary>
        /// <param name="imagePath">Path of image to extact text from</param>
        /// <param name="timoutSeconds">Timeout in seconds before reporting failure</param>
        /// <param name="throwOnError">Fail (break) or error or just return empty string (useful when using this method in batch operations)</param>
        /// <returns>Text from image</returns>
        public static string GetTextFromImage(
            string imagePath,
            GoogleApiImageToTextType imageToTextType = GoogleApiImageToTextType.Document,
            bool throwOnError = true)
        {
            SetGoogleApiCredentialEnvironmentVariable();
            var content = string.Empty;

            try
            {
                var client = ImageAnnotatorClient.Create();
                var image = Image.FromFile(imagePath);

                if (imageToTextType == GoogleApiImageToTextType.Document)
                {
                    var annotation = client.DetectDocumentText(image);
                    return annotation.Text;
                }
                else
                {
                    var response = client.DetectText(image);
                    var text = string.Empty;
                    foreach (var annotation in response)
                    {
                        if (annotation.Description != null)
                        {
                            text += annotation.Description;
                        }
                    }

                    return text;
                }
            }
            catch (Exception e)
            {
                if (throwOnError)
                {
                    throw new ApplicationException("Unable to parse using Google Vision API: " + e.Message);
                }

                return content;
            }
        }

        /// <summary>
        /// Extracts text from an Image
        /// </summary>
        /// <param name="imageStream">Memory stream of image</param>
        /// <param name="timoutSeconds">Timeout in seconds before reporting failure</param>
        /// <param name="throwOnError">Fail (break) or error or just return empty string (useful when using this method in batch operations)</param>
        /// <returns>Text from image</returns>
        public static string GetTextFromImage(
            MemoryStream imageStream,
            GoogleApiImageToTextType imageToTextType = GoogleApiImageToTextType.Document,
            bool throwOnError = true)
        {
            SetGoogleApiCredentialEnvironmentVariable();
            var content = string.Empty;

            try
            {
                var client = ImageAnnotatorClient.Create();
                var image = Image.FromStream(imageStream);

                if (imageToTextType == GoogleApiImageToTextType.Document)
                {
                    var annotation = client.DetectDocumentText(image);
                    return annotation.Text;
                }
                else
                {
                    var response = client.DetectText(image);
                    var text = string.Empty;
                    foreach (var annotation in response)
                    {
                        if (annotation.Description != null)
                        {
                            text += annotation.Description;
                        }
                    }

                    return text;
                }
            }
            catch (Exception e)
            {
                if (throwOnError)
                {
                    throw new ApplicationException("Unable to parse using Google Vision API: " + e.Message);
                }

                return content;
            }
        }


        /// <summary>
        /// Extracts text from an Image
        /// </summary>
        /// <param name="imageStream">Byte array of image</param>
        /// <param name="timoutSeconds">Timeout in seconds before reporting failure</param>
        /// <param name="throwOnError">Fail (break) or error or just return empty string (useful when using this method in batch operations)</param>
        /// <returns>Text from image</returns>
        public static string GetTextFromImage(
            byte[] imageBytes,
            GoogleApiImageToTextType imageToTextType = GoogleApiImageToTextType.Document,
            bool throwOnError = true)
        {
            SetGoogleApiCredentialEnvironmentVariable();
            var content = string.Empty;

            try
            {
                var client = ImageAnnotatorClient.Create();
                var image = Image.FromBytes(imageBytes);

                if (imageToTextType == GoogleApiImageToTextType.Document)
                {
                    var annotation = client.DetectDocumentText(image);
                    return annotation.Text;
                }
                else
                {
                    var response = client.DetectText(image);
                    var text = string.Empty;
                    foreach (var annotation in response)
                    {
                        if (annotation.Description != null)
                        {
                            text += annotation.Description;
                        }
                    }

                    return text;
                }
            }
            catch (Exception e)
            {
                if (throwOnError)
                {
                    throw new ApplicationException("Unable to parse using Google Vision API: " + e.Message);
                }

                return content;
            }
        }

        private static void SetGoogleApiCredentialEnvironmentVariable()
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", GetGoogleJsonLocation(), EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", GetGoogleJsonLocation(), EnvironmentVariableTarget.Process);
        }

        /// <summary>
        /// Extract google vision security json file from dll embedded resource
        /// </summary>
        /// <returns></returns>
        private static string GetGoogleJsonLocation()
        {
            return AssemblyHelper.EmbeddedResourceHelper.InternalGetEmbeddedResourcePathFromJBToolkit("googlevisionapikey.json", false);
        }
    }
}
