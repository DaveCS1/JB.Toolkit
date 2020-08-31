using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace JBToolkit.GoogeApi
{
    /// <summary>
    /// Perform geocoding operations on longitue and latitude or reverse-geocoding (i.e. long lat from address). 
    /// Can Get the full Google geocoding object or just the desired formatted address or lat long coordinates
    /// </summary>
    public class Geocoding
    {
        /// <summary>
        /// Get a formatted address from longitude and latitude coordinates
        /// </summary>
        public static string GetFormattedAddressFromLatLong(string lat, string lng)
        {
            GoogleGeolocationObject gco = GetGeolocationObjectFromLatLong(lat, lng);

            if (gco == null)
            {
                return null;
            }
            else
            {
                return gco.results[0].formatted_address;
            }
        }

        /// <summary>
        /// Get the longitude and latitude coordinates from an address. You can optionally set to 'attempt force resolve' which will iterate
        /// through the address string taking off the first character for each iteration on 'zero result'... Which would hopefully resolve
        /// to the nearest say 'street, area, town, city etc' - However this won't give as accurate result when having to resolve.
        /// </summary>
        public static Location GetLatLongFromAddress(string address, bool attemptForceResolve)
        {
            GoogleGeolocationObject gco = GetGeolocationObjectFromAddress(address, attemptForceResolve);

            if (gco == null)
            {
                return null;
            }
            else
            {
                return gco.results[0].geometry.location;
            }
        }

        /// <summary>
        /// Get the entire Google geocoordinates object (full json repsonse serialised as object) from a given address. You can optionally set to 'attempt force resolve' which will iterate
        /// through the address string taking off the first character for each iteration on 'zero result'... Which would hopefully resolve
        /// to the nearest say 'street, area, town, city etc' - However this won't give as accurate result when having to resolve.
        /// </summary>
        public static GoogleGeolocationObject GetGeolocationObjectFromAddress(string address, bool attemptForceResolve = false)
        {
            string requestUri = string.Format("{0}/geocode/json?address={1}&key={2}", Global.GoogleConfiguration.GoogleApiUrl, Uri.EscapeDataString(address), new NetworkCredential("", Global.GoogleConfiguration.GoogleApiKey).Password);

            WebRequest request = WebRequest.Create(requestUri);
            WebResponse response = request.GetResponse();

            string body = string.Empty;

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                body = reader.ReadToEnd();
            }

            if (body.Contains("ZERO_RESULTS"))
            {
                if (!attemptForceResolve)
                {
                    return null;
                }
                else
                {
                    if (address.Length > 1) // shorten start of address (until we eventually get to 'town', 'city' etc
                    {
                        return GetGeolocationObjectFromAddress(address.Substring(1, address.Length - 1), true);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                GoogleGeolocationObject result = JsonConvert.DeserializeObject<GoogleGeolocationObject>(body);
                return result;
            }
        }

        /// <summary>
        /// Get the entire Google geocoordinates object from a latitute and longitude coordinates.
        /// </summary>
        public static GoogleGeolocationObject GetGeolocationObjectFromLatLong(string lat, string lng)
        {
            string requestUri = string.Format("{0}/geocode/json?latlng={1}&key={2}", Global.GoogleConfiguration.GoogleApiUrl, lat + "," + lng, new NetworkCredential("", Global.GoogleConfiguration.GoogleApiKey).Password);

            WebRequest request = WebRequest.Create(requestUri);
            WebResponse response = request.GetResponse();

            string body = string.Empty;

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                body = reader.ReadToEnd();
            }

            if (body.Contains("ZERO_RESULTS"))
            {
                return null;
            }
            else
            {
                GoogleGeolocationObject result = JsonConvert.DeserializeObject<GoogleGeolocationObject>(body);
                return result;
            }
        }

        public class GoogleGeolocationObject
        {
#pragma warning disable IDE1006 // Naming Styles
            public Result[] results { get; set; }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning disable IDE1006 // Naming Styles
            public string status { get; set; }
#pragma warning restore IDE1006 // Naming Styles
        }

        public class Result
        {
#pragma warning disable IDE1006 // Naming Styles
            public Address_Components[] address_components { get; set; }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning disable IDE1006 // Naming Styles
            public string formatted_address { get; set; }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning disable IDE1006 // Naming Styles
            public Geometry geometry { get; set; }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning disable IDE1006 // Naming Styles
            public string place_id { get; set; }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning disable IDE1006 // Naming Styles
            public string[] types { get; set; }
#pragma warning restore IDE1006 // Naming Styles
        }

        public class Geometry
        {
#pragma warning disable IDE1006 // Naming Styles
            public Location location { get; set; }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning disable IDE1006 // Naming Styles
            public string location_type { get; set; }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning disable IDE1006 // Naming Styles
            public Viewport viewport { get; set; }
#pragma warning restore IDE1006 // Naming Styles
        }

        public class Location
        {
#pragma warning disable IDE1006 // Naming Styles
            public float lat { get; set; }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning disable IDE1006 // Naming Styles
            public float lng { get; set; }
#pragma warning restore IDE1006 // Naming Styles
        }

        public class Viewport
        {
#pragma warning disable IDE1006 // Naming Styles
            public Northeast northeast { get; set; }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning disable IDE1006 // Naming Styles
            public Southwest southwest { get; set; }
#pragma warning restore IDE1006 // Naming Styles
        }

        public class Northeast
        {
#pragma warning disable IDE1006 // Naming Styles
            public float lat { get; set; }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning disable IDE1006 // Naming Styles
            public float lng { get; set; }
#pragma warning restore IDE1006 // Naming Styles
        }

        public class Southwest
        {
#pragma warning disable IDE1006 // Naming Styles
            public float lat { get; set; }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning disable IDE1006 // Naming Styles
            public float lng { get; set; }
#pragma warning restore IDE1006 // Naming Styles
        }

        public class Address_Components
        {
#pragma warning disable IDE1006 // Naming Styles
            public string long_name { get; set; }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning disable IDE1006 // Naming Styles
            public string short_name { get; set; }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning disable IDE1006 // Naming Styles
            public string[] types { get; set; }
#pragma warning restore IDE1006 // Naming Styles
        }
    }
}
