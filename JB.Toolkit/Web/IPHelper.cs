using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Web;

namespace JBToolkit.Web
{
    public class IPHelper
    {
        /// <summary>
        /// Gets the public / external IP of process
        /// </summary>
        /// <returns>Public / external IP address</returns>
        public static string GetExternalIP()
        {
            try
            {
                string url = "http://checkip.dyndns.org";
                WebRequest req = WebRequest.Create(url);
                req.Timeout = 20000; // 10 seconds
                WebResponse resp = req.GetResponse();
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                string response = sr.ReadToEnd().Trim();
                string[] a = response.Split(':');
                string a2 = a[1].Substring(1);
                string[] a3 = a2.Split('<');
                string a4 = a3[0];
                return a4;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Determines if there's a valid internet connection by querying google. Either by ping or by http request.
        /// </summary>
        /// <returns>True if there is, false otherwise</returns>
        public static bool IsThereAnInternetConnetion(bool useNonPingMethod = false)
        {
            try
            {
                if (useNonPingMethod)
                {
                    using (var client = new WebClient())
                    using (client.OpenRead("http://google.com/generate_204"))
                        return true;
                }
                else
                {

                    Ping myPing = new Ping();
                    string host = "google.com";
                    byte[] buffer = new byte[32];
                    int timeout = 10000;
                    PingOptions pingOptions = new PingOptions();
                    PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);

                    return reply.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get the IP address of the requestor
        /// </summary>
        /// <param name="GetLan">Get the visitor IP address that's on the LAN. Default to false</param>
        /// <returns>Either IPv4 or IPv6 address (if on lan)</returns>
        public static string GetVisitorIPAddress(bool GetLan = false)
        {
            string visitorIPAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (String.IsNullOrEmpty(visitorIPAddress))
                visitorIPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            if (string.IsNullOrEmpty(visitorIPAddress))
                visitorIPAddress = HttpContext.Current.Request.UserHostAddress;

            if (string.IsNullOrEmpty(visitorIPAddress) || visitorIPAddress.Trim() == "::1")
                visitorIPAddress = "127.0.0.1";

            if (GetLan)
            {
                //This is for Local(LAN) Connected ID Address
                string stringHostName = Dns.GetHostName();
                //Get Ip Host Entry
                IPHostEntry ipHostEntries = Dns.GetHostEntry(stringHostName);
                //Get Ip Address From The Ip Host Entry Address List
                IPAddress[] arrIpAddress = ipHostEntries.AddressList;

                try
                {
                    visitorIPAddress = arrIpAddress[arrIpAddress.Length - 2].ToString();
                }
                catch
                {
                    try
                    {
                        visitorIPAddress = arrIpAddress[0].ToString();
                    }
                    catch
                    {
                        try
                        {
                            arrIpAddress = Dns.GetHostAddresses(stringHostName);
                            visitorIPAddress = arrIpAddress[0].ToString();
                        }
                        catch
                        {
                            visitorIPAddress = "127.0.0.1";
                        }
                    }
                }

            }

            return visitorIPAddress;
        }
    }
}
