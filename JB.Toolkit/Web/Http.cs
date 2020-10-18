using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace JBToolkit.Web
{
    /// <summary>
    /// Generic HTTP methods
    /// </summary>
    public class Http
    {
        public enum LocationType
        {
            FilePath,
            URL,
            Base64String,
            Unknown
        }

        /// <summary>
        /// Return scheme + host + port (i.e. http://localhost:80)
        /// </summary>
        public static string GetSiteRoot()
        {
            return System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Host + ":" + System.Web.HttpContext.Current.Request.Url.Port;
        }

        /// <summary>
        /// Returns just the path of the URL (without query string)
        /// </summary>
        public static string RemoveQueryStringFromUrl(string fullUrl)
        {
            Uri url = new Uri(fullUrl);
            string path = string.Format("{0}{1}{2}{3}", url.Scheme,
                Uri.SchemeDelimiter, url.Authority, url.AbsolutePath);

            return path;
        }

        /// <summary>
        /// Used to test if favicon of quicklink custom site exists, so that if it doesn't we can use a custom image
        /// </summary>
        /// <param name="uriToImage">Remote URL</param>
        /// <returns>True or false</returns>
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

            byte[] _byte = GetImageBytesFromUrl(url);

            if (includeMimeTypeDescriptor)
                return Images.ImageHelper.MimeHelper.GetBase64StringFromBytesWithMime(_byte);

            else
            {
                _sb.Append(Convert.ToBase64String(_byte, 0, _byte.Length));
                return _sb.ToString();
            }
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
        /// Gets a stream from a remote URL file location
        /// </summary>
        /// <param name="url"></param>
        public static Stream GetStreamFromUrl(string url)
        {
            byte[] imageData = null;

            using (var wc = new System.Net.WebClient())
            {
                imageData = wc.DownloadData(url);
            }

            return new MemoryStream(imageData);
        }

        /// <summary>
        /// Based on given input string - it decides if it's a web URL, a local file path or a base64 string.
        /// 
        /// *Don't rely on this. It works by checking the existance of an URL or local file. No acutal string pattern matching is performed.
        /// </summary>
        public static LocationType GetIsPathOrUrlOrBase64(string input)
        {
            if (PathIsLocalFile(input))
            {
                return LocationType.FilePath;
            }
            else if (PathIsUrl(input))
            {
                return LocationType.URL;
            }
            else if (PathIsUrl(input))
            {
                return LocationType.Base64String;
            }
            else
            {
                return LocationType.Base64String;
            }
        }

        /// <summary>
        /// Is the path a local file? I.e. not a web URL or base 64 string
        /// 
        /// *Don't rely on this. It works by checking the existance of a local file. No acutal string pattern matching is performed.
        /// </summary>
        public static bool PathIsLocalFile(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Is the path a local file? I.e. not a web URL or base 64 string
        /// 
        /// *Don't rely on this. It works by checking the existance of an URL end-point. No acutal string pattern matching is performed.
        /// </summary>
        public static bool PathIsUrl(string path)

        {
            if (File.Exists(path))
            {
                return false;
            }

            try
            {
                Uri uri = new Uri(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Decompress a brotli compressed (br) http requst response stream (i.e, Google uses this)
        /// </summary>
        /// <param name="request">WebClient or HttpWebRequest input</param>
        /// <returns>Raw string</returns>
        public static string DecompressBrotliFromHttpRequestResponse(HttpWebRequest request)
        {
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                string result;
                using (var brotli = new Brotli.BrotliStream(
                                                    response.GetResponseStream(),
                                                    System.IO.Compression.CompressionMode.Decompress,
                                                    true))
                {
                    var streamReader = new StreamReader(brotli);
                    result = streamReader.ReadToEnd();
                }

                return result;
            }
        }

        /// <summary>
        /// Decompress a brotli compressed (br) http request response stream (i.e, Google uses this)
        /// </summary>
        /// <param name="request">WebClient or HttpWebRequest input</param>
        /// <returns>Deserialised object as a dynamic object</returns>
        public static dynamic DecompressBrotliFromHttpRequestResponseAssumingJson(HttpWebRequest request)
        {
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                string result;
                using (var brotli = new Brotli.BrotliStream(
                                                    response.GetResponseStream(),
                                                    System.IO.Compression.CompressionMode.Decompress,
                                                    true))
                {
                    var streamReader = new StreamReader(brotli);
                    result = streamReader.ReadToEnd();
                }

                return JsonConvert.DeserializeObject(result);
            }
        }

        /// <summary>
        /// Decompress a brotli compressed (br) http request response stream (i.e, Google uses this)
        /// </summary>
        /// <typeparam name="T">Type of object to deserialise to</typeparam>
        /// <param name="request">WebClient or HttpWebRequest input</param>
        /// <returns>Deserialised object</returns>
        public static T DecompressBrotliFromHttpRequestResponseAssumingJson<T>(HttpWebRequest request)
        {
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                string result;
                using (var brotli = new Brotli.BrotliStream(
                                                    response.GetResponseStream(),
                                                    System.IO.Compression.CompressionMode.Decompress,
                                                    true))
                {
                    var streamReader = new StreamReader(brotli);
                    result = streamReader.ReadToEnd();
                }

                return JsonConvert.DeserializeObject<T>(result);
            }
        }
    }

    public class Url
    {
        /// <summary>
        /// Similar to 'Path.Combine' but for URLs
        /// </summary>
        /// <param name="baseUrl">Parent URL</param>
        /// <param name="method">MEthod URL</param>
        public static string Combine(string baseUrl, string method)
        {
            return baseUrl + (baseUrl.EndsWith("/") ? "" : "/") + method;
        }
    }
}
