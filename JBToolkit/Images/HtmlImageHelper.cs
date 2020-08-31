using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace JBToolkit.Images
{
    /// <summary>
    /// Image manipulation helper methods
    /// </summary>
    public partial class ImageHelper
    {
        public class HtmlImageHelper
        {
            /// <summary>
            /// Returns a list of paths to images in a Html file along with a path type (i.e. local, relative, remote, data)
            /// </summary>
            /// <param name="html">Html text as string</param>
            /// <returns>HtmlPaths object consisting of path and path type</returns>
            public static List<HtmlPath> GetHtmlImagePathsFromHtml(string html)
            {
                string regexPattern1 = @"(src|data)(\s?)+=(\s?)(\w?)(""|')(.*?)(""|')";
                string regexPattern2 = @"(url|URL|Url)\((""|')(.*?)(""|')";

                var matches = new List<MatchCollection>();

                var matches1 = Regex.Matches(html, regexPattern1);
                var matches2 = Regex.Matches(html, regexPattern2);

                matches.Add(matches1);
                matches.Add(matches2);

                var paths = new List<HtmlPath>();

                foreach (var matchCol in matches)
                {
                    foreach (Match match in matchCol)
                    {
                        string raw = match.Value.ToString();

                        int idxFirst = raw.IndexOf("\"");
                        if (idxFirst == -1)
                        {
                            idxFirst = raw.IndexOf("\'");
                        }

                        int idxLast = raw.LastIndexOf("\"");
                        if (idxLast == -1)
                        {
                            idxLast = raw.LastIndexOf("\'");
                        }

                        string path = raw.Substring(idxFirst + 1, idxLast - (idxFirst + 1)).Replace(" ", "").Trim();

                        if (path.StartsWith("data:image"))
                        {
                            paths.Add(new HtmlPath
                            {
                                Value = path,
                                PathType = HtmlPath.PathTypeEnum.Data
                            });
                        }
                        else if (path.ToLower().StartsWith("http"))
                        {
                            paths.Add(new HtmlPath
                            {
                                Value = path,
                                PathType = HtmlPath.PathTypeEnum.Remote
                            });
                        }
                        else if (path.Contains(":"))
                        {
                            paths.Add(new HtmlPath
                            {
                                Value = path,
                                PathType = HtmlPath.PathTypeEnum.LocalFull
                            });
                        }
                        else
                        {
                            paths.Add(new HtmlPath
                            {
                                Value = path,
                                PathType = HtmlPath.PathTypeEnum.LocalRelative
                            });
                        }
                    }
                }

                return paths;
            }

            /// <summary>
            /// Peforms a HTTP web request to see if an image exists remotely
            /// </summary>
            public static bool DoesImageExistRemotely(string uriToImage)
            {
                if (!uriToImage.Substring(0, 5).Contains("http"))
                {
                    uriToImage = "http://" + uriToImage;
                }

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uriToImage);

                request.Timeout = 4000; // 4 seconds
                request.Method = "HEAD";

                try
                {
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode == HttpStatusCode.OK && response.ContentType.ToLower().Contains("image"))
                        {
                            return true;
                        }

                        return false;
                    }
                }
                catch (WebException) { return false; }
                catch { return false; }
            }

            /// <summary>
            /// Peforms a HTTP web request and downloads an image
            /// </summary>
            public static string GetImageBase64StringFromUrl(String url, bool includeMimeTypeDescriptor = false)
            {
                StringBuilder _sb = new StringBuilder();

                Byte[] _byte = GetImageBytesFromUrl(url);

                if (includeMimeTypeDescriptor)
                    return Images.ImageHelper.GetBase64StringFromBytesWithMime(_byte);

                _sb.Append(Convert.ToBase64String(_byte, 0, _byte.Length));

                return _sb.ToString();
            }

            /// <summary>
            /// Peforms a HTTP web request and downloads an image
            /// </summary>
            public static byte[] GetImageBytesFromUrl(string url)
            {
                byte[] buf;

                try
                {
                    WebProxy myProxy = new WebProxy();
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

                    HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                    Stream stream = response.GetResponseStream();

                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        int len = (int)(response.ContentLength);
                        buf = br.ReadBytes(len);
                        br.Close();
                    }

                    stream.Close();
                    response.Close();
                }
                catch (Exception)
                {
                    buf = null;
                }

                return (buf);
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
            /// Parse base 64 to DataImage object consisting of raw byte[] data and the image and mime type.
            /// </summary>
            public class DataImage
            {
                public DataImage(string imageType, byte[] rawData)
                {
                    RawData = rawData;
                    ImageType = imageType;
                }

                public static readonly Regex DataUriPattern = new Regex(@"^data\:(?<mimeType>image\/(?<imageType>png|tiff|tif|jpg|jpeg|gif|bmp|svg));base64,(?<data>[A-Z0-9\+\/\=]+)$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

                public DataImage(string mimeType, string imageType, byte[] rawData)
                {
                    MimeType = mimeType;
                    RawData = rawData;
                    ImageType = imageType;
                }

                public string MimeType { get; }
                public byte[] RawData { get; }
                public string ImageType { get; }

                public System.Drawing.Image Image => System.Drawing.Image.FromStream(new MemoryStream(RawData));

                /// <summary>
                ///  Try to parse a base64 encoded image and return a DataImage object of the raw bytes and mime types
                /// </summary>
                /// <param name="dataUri"></param>
                /// <param name="image"></param>
                /// <returns></returns>
                public static bool TryParse(string dataUri, out DataImage image)
                {
                    image = null;
                    if (string.IsNullOrWhiteSpace(dataUri))
                    {
                        return false;
                    }

                    Match match = DataUriPattern.Match(dataUri);
                    if (!match.Success)
                    {
                        return false;
                    }

                    string mimeType = match.Groups["mimeType"].Value;
                    string imageType = match.Groups["imageType"].Value;
                    string base64Data = match.Groups["data"].Value;

                    try
                    {
                        byte[] rawData = Convert.FromBase64String(base64Data);
                        image = rawData.Length == 0 ? null : new DataImage(mimeType, imageType, rawData);
                        return true;
                    }
                    catch (FormatException)
                    {
                        return false;
                    }
                }
            }

            /// <summary>
            /// Paths to image in Html file along with a path type (i.e. local, relative, remote, data)
            /// </summary>
            public class HtmlPath
            {
                public enum PathTypeEnum
                {
                    Remote,
                    LocalFull,
                    LocalRelative,
                    Data
                }

                public string Value { get; set; }
                public PathTypeEnum PathType { get; set; }
            }
        }
    }
}
