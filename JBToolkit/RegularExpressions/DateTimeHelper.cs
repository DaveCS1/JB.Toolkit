using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace JBToolkit.RegularExpressions
{
    /// <summary>
    /// Methods for DateTime search, validation and manipulation
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// Returns all matched dates from a string (i.e. string of PDF document or a filename) - Tests a variety of formats (even inpropper ones) matching with Regex
        /// </summary>
        /// <param name="input">Input string to find dates within</param>
        /// <param name="matchStrings">Output of the Regex string matches</param>
        /// <returns>List of DateTime objects</returns>
        public static List<DateTime> GetDateMatchesFromString(string input, out List<string> matchStrings)
        {
            matchStrings = new List<string>();
            var DateList = new List<DateTime>();

            // eg. 01/01/2018
            MatchCollection matchCol = Regex.Matches(input, @"\d{2}[-|\/|\\.]\d{2}[-|\/|\\.]\d{4}");

            string possibleDate;
            foreach (Match match in matchCol)
            {
                possibleDate = match.Value.Substring(0, 2) + "/" + match.Value.Substring(3, 2) + "/" + match.Value.Substring(6, 4);

                if (DateTime.TryParseExact(possibleDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out DateTime d2))
                {
                    matchStrings.Add(d2.ToString("dd/MM/yyyy"));
                    DateList.Add(d2);
                }
            }

            // eg. 2018-01-01
            matchCol = Regex.Matches(input, @"\d{4}[-|\/|\\.]\d{2}[-|\/|\\.]\d{2}");
            foreach (Match match in matchCol)
            {
                possibleDate = match.Value.Substring(8, 2) + "/" + match.Value.Substring(5, 2) + "/" + match.Value.Substring(0, 4);

                if (DateTime.TryParseExact(possibleDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out DateTime d2))
                {
                    matchStrings.Add(d2.ToString("dd/MM/yyyy"));
                    DateList.Add(d2);
                }
            }

            // eg. 2018-1-10
            matchCol = Regex.Matches(input, @"\d{4}[-|\/|\\.]\d{1}[-|\/|\\.]\d{2}");
            foreach (Match match in matchCol)
            {
                possibleDate = match.Value.Substring(7, 2) + "/" + "0" + match.Value.Substring(5, 1) + "/" + match.Value.Substring(0, 4);

                if (DateTime.TryParseExact(possibleDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out DateTime d2))
                {
                    matchStrings.Add(d2.ToString("dd/MM/yyyy"));
                    DateList.Add(d2);
                }
            }

            // eg. 2018-10-1
            matchCol = Regex.Matches(input, @"\d{4}[-|\/|\\.]\d{2}[-|\/|\\.]\d{1}");
            foreach (Match match in matchCol)
            {
                possibleDate = "0" + match.Value.Substring(8, 1) + "/" + match.Value.Substring(5, 2) + "/" + match.Value.Substring(0, 4);

                if (DateTime.TryParseExact(possibleDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out DateTime d2))
                {
                    matchStrings.Add(d2.ToString("dd/MM/yyyy"));
                    DateList.Add(d2);
                }
            }

            // eg. 01/1/2018
            matchCol = Regex.Matches(input, @"\d{2}[-|\/|\\.]\d{1}[-|\/|\\.]\d{4}");
            foreach (Match match in matchCol)
            {
                possibleDate = match.Value.Substring(0, 2) + "/" + "0" + match.Value.Substring(3, 1) + "/" + match.Value.Substring(5, 4);

                if (DateTime.TryParseExact(possibleDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out DateTime d2))
                {
                    matchStrings.Add(d2.ToString("dd/MM/yyyy"));
                    DateList.Add(d2);
                }
            }

            // eg. 1/01/2018
            matchCol = Regex.Matches(input, @"\d{1}[-|\/|\\.]\d{2}[-|\/|\\.]\d{4}");
            foreach (Match match in matchCol)
            {
                possibleDate = "0" + match.Value.Substring(0, 1) + "/" + match.Value.Substring(2, 2) + "/" + match.Value.Substring(5, 4);

                if (DateTime.TryParseExact(possibleDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out DateTime d2))
                {
                    matchStrings.Add(d2.ToString("dd/MM/yyyy"));
                    DateList.Add(d2);
                }
            }

            // eg. 1/1/2018
            matchCol = Regex.Matches(input, @"\d{1}[-|\/|\\.]\d{1}[-|\/|\\.]\d{4}");
            foreach (Match match in matchCol)
            {
                possibleDate = "0" + match.Value.Substring(0, 1) + "/" + "0" + match.Value.Substring(2, 1) + "/" + match.Value.Substring(4, 4);

                if (DateTime.TryParseExact(possibleDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out DateTime d2))
                {
                    matchStrings.Add(d2.ToString("dd/MM/yyyy"));
                    DateList.Add(d2);
                }
            }

            // eg. 01/1/18
            matchCol = Regex.Matches(input, @"\d{2}[-|\/|\\.]\d{1}[-|\/|\\.]\d{2}");
            foreach (Match match in matchCol)
            {
                possibleDate = match.Value.Substring(0, 2) + "/" + "0" + match.Value.Substring(3, 1) + "/" + match.Value.Substring(5, 2);

                if (DateTime.TryParseExact(possibleDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out DateTime d2))
                {
                    matchStrings.Add(d2.ToString("dd/MM/yyyy"));
                    DateList.Add(d2);
                }
            }

            // eg. 1/01/18
            matchCol = Regex.Matches(input, @"\d{1}[-|\/|\\.]\d{2}[-|\/|\\.]\d{2}");
            foreach (Match match in matchCol)
            {
                possibleDate = "0" + match.Value.Substring(0, 1) + "/" + match.Value.Substring(2, 2) + "/" + match.Value.Substring(5, 2);

                if (DateTime.TryParseExact(possibleDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out DateTime d2))
                {
                    matchStrings.Add(d2.ToString("dd/MM/yyyy"));
                    DateList.Add(d2);
                }
            }

            // eg. 1/1/18
            matchCol = Regex.Matches(input, @"\d{1}[-|\/|\\.]\d{1}[-|\/|\\.]\d{2}");
            foreach (Match match in matchCol)
            {
                possibleDate = "0" + match.Value.Substring(0, 1) + "/" + "0" + match.Value.Substring(2, 1) + "/" + match.Value.Substring(4, 2);

                if (DateTime.TryParseExact(possibleDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out DateTime d2))
                {
                    matchStrings.Add(d2.ToString("dd/MM/yyyy"));
                    DateList.Add(d2);
                }
            }

            // eg. 01-2018
            matchCol = Regex.Matches(input, @"\d{2}[-|\/|\\.]\d{4}");
            foreach (Match match in matchCol)
            {
                possibleDate = "01" + "/" + match.Value.Substring(0, 2) + "/" + match.Value.Substring(3, 4);

                if (DateTime.TryParseExact(possibleDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out DateTime d2))
                {
                    matchStrings.Add(d2.ToString("dd/MM/yyyy"));
                    DateList.Add(d2);
                }
            }

            // eg. 2018-01
            matchCol = Regex.Matches(input, @"\d{4}[-|\/|\\.]\d{2}");
            foreach (Match match in matchCol)
            {
                possibleDate = "01" + "/" + match.Value.Substring(5, 2) + "/" + match.Value.Substring(0, 4);

                if (DateTime.TryParseExact(possibleDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out DateTime d2))
                {
                    matchStrings.Add(d2.ToString("dd/MM/yyyy"));
                    DateList.Add(d2);
                }
            }

            // eg. 2018-1
            matchCol = Regex.Matches(input, @"\d{4}[-|\/|\\.]\d{1}");
            foreach (Match match in matchCol)
            {
                possibleDate = "01" + "/" + "0" + match.Value.Substring(5, 1) + "/" + match.Value.Substring(0, 4);

                if (DateTime.TryParseExact(possibleDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out DateTime d2))
                {
                    matchStrings.Add(d2.ToString("dd/MM/yyyy"));
                    DateList.Add(d2);
                }
            }

            // not sure
            matchCol = Regex.Matches(input, @"(\b(0?[1-9]|[12]\d|30|31)[^\w\d\r\n:](0?[1-9]|1[0-2])[^\w\d\r\n:](\d{4}|\d{2})\b)|(\b(0?[1-9]|1[0-2])[^\w\d\r\n:](0?[1-9]|[12]\d|30|31)[^\w\d\r\n:](\d{4}|\d{2})\b)");
            foreach (Match match in matchCol)
            {
                if (DateTime.TryParse(match.Value, out DateTime d2))
                {
                    matchStrings.Add(d2.ToString("dd/MM/yyyy"));
                    DateList.Add(d2);
                }
            }

            // not sure
            matchCol = Regex.Matches(input, @"(\b(0?[1-9]|[12]\d|30|31)[^\w\d\r\n:](0?[1-9]|1[0-2])[^\w\d\r\n:](\d{4}|\d{2})\b)|(\b(0?[1-9]|1[0-2])[^\w\d\r\n:](0?[1-9]|[12]\d|30|31)[^\w\d\r\n:](\d{4}|\d{2})\b)", RegexOptions.IgnorePatternWhitespace);
            foreach (Match match in matchCol)
            {
                if (DateTime.TryParse(match.Value, out DateTime d2))
                {
                    matchStrings.Add(d2.ToString("dd/MM/yyyy"));
                    DateList.Add(d2);
                }
            }

            // eg. Jan 01 2018
            matchCol = Regex.Matches(input, @"((Jan)|(Feb)|(Mar)|(Apr)|(May)|(Jun)|(Jul)|(Aug)|(Sep)|(Oct)|(Nov)|(Dec))\s([1-9]|([12][0-9])|(3[01])),\s\d\d\d\d", RegexOptions.IgnoreCase);
            foreach (Match match in matchCol)
            {
                if (DateTime.TryParse(match.Value, out DateTime d2))
                {
                    matchStrings.Add(d2.ToString("dd/MM/yyyy"));
                    DateList.Add(d2);
                }
            }

            // eg. 01 Jan 2018
            matchCol = Regex.Matches(input, @"(([1-9])|([0][1-9])|([1-2][0-9])|([3][0-1]))\ (JAN|FEB|MAR|ARP|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC)\ \d{4}", RegexOptions.IgnoreCase);
            foreach (Match match in matchCol)
            {
                if (DateTime.TryParse(match.Value, out DateTime d2))
                {
                    matchStrings.Add(d2.ToString("dd/MM/yyyy"));
                    DateList.Add(d2);
                }
            }

            // eg. 01 Jan 18
            matchCol = Regex.Matches(input, @"(([1-9])|([0][1-9])|([1-2][0-9])|([3][0-1]))\ (JAN|FEB|MAR|ARP|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC)\ \d{2}", RegexOptions.IgnoreCase);
            foreach (Match match in matchCol)
            {
                if (DateTime.TryParse(match.Value, out DateTime d2))
                {
                    matchStrings.Add(d2.ToString("dd/MM/yyyy"));
                    DateList.Add(d2);
                }
            }

            // eg. 01 January 2018
            matchCol = Regex.Matches(input, @"(([1-9])|([0][1-9])|([1-2][0-9])|([3][0-1]))\ (January|February|March|April|May|June|July|August|September|October|November|December)\ \d{4}", RegexOptions.IgnoreCase);
            foreach (Match match in matchCol)
            {
                if (DateTime.TryParse(match.Value, out DateTime d2))
                {
                    matchStrings.Add(d2.ToString("dd/MM/yyyy"));
                    DateList.Add(d2);
                }
            }

            // eg. 01 January 18
            matchCol = Regex.Matches(input, @"(([1-9])|([0][1-9])|([1-2][0-9])|([3][0-1]))\ (January|February|March|April|May|June|July|August|September|October|November|December)\ \d{2}", RegexOptions.IgnoreCase);
            foreach (Match match in matchCol)
            {
                if (DateTime.TryParse(match.Value, out DateTime d2))
                {
                    matchStrings.Add(d2.ToString("dd/MM/yyyy"));
                    DateList.Add(d2);
                }
            }

            // eg. 20180101
            matchCol = Regex.Matches(input, @"20(\d{6})");
            foreach (Match match in matchCol)
            {
                possibleDate = match.Value.Substring(6, 2) + "/" + match.Value.Substring(4, 2) + "/" + match.Value.Substring(0, 4);

                if (DateTime.TryParseExact(possibleDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out DateTime d2))
                {
                    matchStrings.Add(d2.ToString("dd/MM/yyyy"));
                    DateList.Add(d2);
                }
            }

            // eg. 01012018
            matchCol = Regex.Matches(input, @"(\d{4})20(\d{2})");
            foreach (Match match in matchCol)
            {
                possibleDate = match.Value.Substring(0, 2) + "/" + match.Value.Substring(2, 2) + "/" + match.Value.Substring(4, 4);

                if (DateTime.TryParseExact(possibleDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out DateTime d2))
                {
                    matchStrings.Add(d2.ToString("dd/MM/yyyy"));
                    DateList.Add(d2);
                }
            }

            // 20180101
            matchCol = Regex.Matches(input, @"199(\d{5})");
            foreach (Match match in matchCol)
            {
                possibleDate = match.Value.Substring(6, 2) + "/" + match.Value.Substring(4, 2) + "/" + match.Value.Substring(0, 4);

                if (DateTime.TryParseExact(possibleDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out DateTime d2))
                {
                    matchStrings.Add(d2.ToString("dd/MM/yyyy"));
                    DateList.Add(d2);
                }
            }

            return DateList;
        }

        /// <summary>
        /// Gets a valid Date (DateTime) contained within a string if there is one. Also outputs the matched DateTime as a string.
        /// Tries to convert a variable format date string to DateTime (i.e. 20180101, 20-01-18, 01012018, 1-5-18 etc).
        /// </summary>
        /// <param name="inputString">Filename (not path)</param>
        /// <param name="matchedDateString">(out) DateTime as string</param>
        /// <returns>DateTime? from filename</returns>
        public static DateTime? GetDateContainedInString(string inputString, out string matchedDateString)
        {
            var dateList = GetDateMatchesFromString(inputString, out List<string> dateTimeStringList);

            if (dateList.Count > 0)
            {
                matchedDateString = dateTimeStringList[0];
                return dateList[0];
            }

            matchedDateString = null;
            return null;
        }

        /// <summary>
        /// Gets the Date from a filename if there is one. Also outputs the DateTime as a string. 
        /// Tries to convert a variable format date string to DateTime (i.e. 20180101, 20-01-18, 01012018, 1-5-18 etc).
        /// </summary>
        /// <param name="fileName">Filename (not path)</param>
        /// <param name="dateTimeString">(out) DateTime as string</param>
        /// <returns>DateTime? from filename</returns>
        public static DateTime? GetDateFromFileName(string fileName, out string dateTimeString)
        {
            return GetDateContainedInString(fileName, out dateTimeString);
        }

        /// <summary>
        /// Try convert variable format date string to DateTime (i.e. 20180101, 20-01-18, 01012018, 1-5-18 etc).
        /// Also outputs the matched DateTime as a string.
        /// </summary>
        /// <param name="fileName">Filename (not path)</param>
        /// <param name="dateTimeString">(out) DateTime as string</param>
        /// <returns>DateTime? from filename</returns>
        public static DateTime? ConvertToDate(string dateString, out string dateTimeString)
        {
            return GetDateContainedInString(dateString, out dateTimeString);
        }
    }
}
