using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace JBToolkit.Csv
{
    public class CsvHelper
    {
        public enum CsvTrimOptions
        {
            None,
            TrimInsideQuotes,
            TrimFields
        }

        /// <summary>
        /// Reads CSV file and returns deserialised object T.
        /// Note, if the CSV file has no header, then be sure to include 'Index' attributes on your class properties. I.e:
        /// 
        ///     public class Foo
        ///     {
        ///         [Index(0)]
        ///         public int Id { get; set; }
        ///
        ///         [Index(1)]
        ///         public string Name { get; set; }
        ///     }
        ///     
        /// You can also map by a different header name to your class properties by adding the 'Name' attribute to your properties. I.e:
        /// 
        ///     public class Foo
        ///     {
        ///         [Name("id")]
        ///          public int Id { get; set; }
        ///
        ///         [Name("name")]
        ///         public string Name { get; set; }
        ///     }
        ///     
        /// Other attributes include:
        /// 
        ///     [BooleanTrueValues("yes")]
        ///     [BooleanFalseValues("no")]
        ///     [Optional]
        ///     [Ignored]
        ///       
        /// More information available at: https://joshclose.github.io/CsvHelper/getting-started
        ///     
        /// </summary>
        /// <typeparam name="T">Type of object to deserialise to</typeparam>
        /// <param name="path">File path of CSV file</param>
        /// <param name="delimiter">(Optional) - Single character seperator, i.e: ";" "," "\t" (tab) or "auto" to try and detect</param>
        /// <param name="hasHeaders">(Optional) - Does the CSV file have headers or not?</param>
        /// <param name="ignoreQuotes">(Optional) - If true, quotation marks are treated like any other character</param>
        /// <param name="trimOptions">(Optional) - Trim inside quotes, trim fields, don't trim</param>
        /// <returns>IEnumerable result</returns>
        public static IEnumerable<T> CsvFileToEnumerableObject<T>(
            string path,
            string delimiter = "auto",
            bool hasHeaders = true,
            bool ignoreQuotes = false,
            CsvTrimOptions trimOptions = CsvTrimOptions.TrimFields)
        {
            char selectedDelimeter;
            try
            {
                if (delimiter == "auto")
                {
                    string singleLine = string.Empty;

                    if (hasHeaders)
                    {
                        using (StreamReader readingFile = new StreamReader(path))
                        {
                            singleLine = readingFile.ReadLine();
                        }

                        selectedDelimeter = TryDetectDelimiter(new string[] { singleLine });
                    }
                    else
                    {
                        selectedDelimeter = TryDetectDelimiter(File.ReadAllLines(path));
                    }
                }
                else
                {
                    selectedDelimeter = delimiter.ToCharArray()[0];
                }
            }
            catch
            {
                selectedDelimeter = ',';
            }

            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader))
            {
                csv.Configuration.HasHeaderRecord = hasHeaders;
                csv.Configuration.Delimiter = selectedDelimeter.ToString();
                csv.Configuration.IgnoreBlankLines = true;
                csv.Configuration.IgnoreQuotes = ignoreQuotes;

                switch (trimOptions)
                {
                    case CsvTrimOptions.None:
                        csv.Configuration.TrimOptions = TrimOptions.None;
                        break;
                    case CsvTrimOptions.TrimInsideQuotes:
                        csv.Configuration.TrimOptions = TrimOptions.InsideQuotes;
                        break;
                    case CsvTrimOptions.TrimFields:
                        csv.Configuration.TrimOptions = TrimOptions.Trim;
                        break;
                    default:
                        break;
                }

                return csv.GetRecords<T>().ToList();
            }
        }

        /// <summary>
        /// Reads CSV file and returns dynamic object
        /// Note, if the CSV file has no header, then be sure to include 'Index' attributes on your class properties. I.e:
        /// 
        ///     public class Foo
        ///     {
        ///         [Index(0)]
        ///         public int Id { get; set; }
        ///
        ///         [Index(1)]
        ///         public string Name { get; set; }
        ///     }
        ///     
        /// You can also map by a different header name to your class properties by adding the 'Name' attribute to your properties. I.e:
        /// 
        ///     public class Foo
        ///     {
        ///         [Name("id")]
        ///          public int Id { get; set; }
        ///
        ///         [Name("name")]
        ///         public string Name { get; set; }
        ///     }
        ///     
        /// Other attributes include:
        /// 
        ///     [BooleanTrueValues("yes")]
        ///     [BooleanFalseValues("no")]
        ///     [Optional]
        ///     [Ignored]
        ///       
        /// More information available at: https://joshclose.github.io/CsvHelper/getting-started
        ///       
        /// </summary>
        /// <typeparam name="T">Type of object to deserialise to</typeparam>
        /// <param name="path">File path of CSV file</param>
        /// <param name="delimiter">(Optional) - Single character seperator, i.e: ";" "," "\t" (tab) or "auto" to try and detect</param>
        /// <param name="hasHeaders">(Optional) - Does the CSV file have headers or not?</param>
        /// <param name="ignoreQuotes">(Optional) - If true, quotation marks are treated like any other character</param>
        /// <param name="trimOptions">(Optional) - Trim inside quotes, trim fields, don't trim</param>
        /// <returns>Dynamic result</returns>
        public static dynamic CsvFileToDynamicObject(
            string path,
            string delimiter = "auto",
            bool hasHeaders = true,
            bool ignoreQuotes = false,
            CsvTrimOptions trimOptions = CsvTrimOptions.TrimFields)
        {
            char selectedDelimeter;
            try
            {
                if (delimiter == "auto")
                {
                    string singleLine = string.Empty;

                    if (hasHeaders)
                    {
                        using (StreamReader readingFile = new StreamReader(path))
                        {
                            singleLine = readingFile.ReadLine();
                        }

                        selectedDelimeter = TryDetectDelimiter(new string[] { singleLine });
                    }
                    else
                    {
                        selectedDelimeter = TryDetectDelimiter(File.ReadAllLines(path));
                    }
                }
                else
                {
                    selectedDelimeter = delimiter.ToCharArray()[0];
                }
            }
            catch
            {
                selectedDelimeter = ',';
            }

            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader))
            {
                csv.Configuration.HasHeaderRecord = hasHeaders;
                csv.Configuration.Delimiter = selectedDelimeter.ToString();
                csv.Configuration.IgnoreBlankLines = true;
                csv.Configuration.IgnoreQuotes = ignoreQuotes;

                switch (trimOptions)
                {
                    case CsvTrimOptions.None:
                        csv.Configuration.TrimOptions = TrimOptions.None;
                        break;
                    case CsvTrimOptions.TrimInsideQuotes:
                        csv.Configuration.TrimOptions = TrimOptions.InsideQuotes;
                        break;
                    case CsvTrimOptions.TrimFields:
                        csv.Configuration.TrimOptions = TrimOptions.Trim;
                        break;
                    default:
                        break;
                }

                return csv.GetRecords<dynamic>();
            }
        }

        /// <summary>
        /// Read CSV File and convert to DataTable object
        /// Note, if the CSV file has no header, then be sure to include 'Index' attributes on your class properties. I.e:
        /// 
        ///     public class Foo
        ///     {
        ///         [Index(0)]
        ///         public int Id { get; set; }
        ///
        ///         [Index(1)]
        ///         public string Name { get; set; }
        ///     }
        ///     
        /// You can also map by a different header name to your class properties by adding the 'Name' attribute to your properties. I.e:
        /// 
        ///     public class Foo
        ///     {
        ///         [Name("id")]
        ///          public int Id { get; set; }
        ///
        ///         [Name("name")]
        ///         public string Name { get; set; }
        ///     }
        ///     
        /// Other attributes include:
        /// 
        ///     [BooleanTrueValues("yes")]
        ///     [BooleanFalseValues("no")]
        ///     [Optional]
        ///     [Ignored]
        ///       
        /// More information available at: https://joshclose.github.io/CsvHelper/getting-started
        ///     
        /// </summary>
        /// <param name="path">File path of CSV file</param>
        /// <param name="delimiter">(Optional) - Single character seperator, i.e: ";" "," "\t" (tab) or "auto" to try and detect</param>
        /// <param name="hasHeaders">(Optional) - Does the CSV file have headers or not?</param>
        /// <param name="ignoreQuotes">(Optional) - If true, quotation marks are treated like any other character</param>
        /// <param name="trimOptions">(Optional) - Trim inside quotes, trim fields, don't trim</param>
        /// <returns>DataTable result</returns>
        public static DataTable CsvFileToDataTable(
            string path,
            string delimiter = "auto",
            bool hasHeaders = true,
            bool ignoreQuotes = false,
            CsvTrimOptions trimOptions = CsvTrimOptions.TrimFields)
        {
            var dt = new DataTable();

            char selectedDelimeter;
            try
            {
                if (delimiter == "auto")
                {
                    string singleLine = string.Empty;

                    if (hasHeaders)
                    {
                        using (StreamReader readingFile = new StreamReader(path))
                        {
                            singleLine = readingFile.ReadLine();
                        }

                        selectedDelimeter = TryDetectDelimiter(new string[] { singleLine });
                    }
                    else
                    {
                        selectedDelimeter = TryDetectDelimiter(File.ReadAllLines(path));
                    }
                }
                else
                {
                    selectedDelimeter = delimiter.ToCharArray()[0];
                }
            }
            catch
            {
                selectedDelimeter = ',';
            }

            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader))
            {
                csv.Configuration.HasHeaderRecord = hasHeaders;
                csv.Configuration.Delimiter = selectedDelimeter.ToString();
                csv.Configuration.IgnoreBlankLines = true;
                csv.Configuration.IgnoreQuotes = ignoreQuotes;

                switch (trimOptions)
                {
                    case CsvTrimOptions.None:
                        csv.Configuration.TrimOptions = TrimOptions.None;
                        break;
                    case CsvTrimOptions.TrimInsideQuotes:
                        csv.Configuration.TrimOptions = TrimOptions.InsideQuotes;
                        break;
                    case CsvTrimOptions.TrimFields:
                        csv.Configuration.TrimOptions = TrimOptions.Trim;
                        break;
                    default:
                        break;
                }

                using (var dr = new CsvDataReader(csv))
                {
                    dt.Load(dr);
                }
            }

            return dt;
        }


        /// <summary>
        /// Write IEnumerable object to CSV file
        /// 
        /// More information available at: https://joshclose.github.io/CsvHelper/getting-started
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="path">Output filepath</param>
        /// <param name="delimiter">(Optional) - Seperator, i.e: ; , \t (tab)</param>
        /// <param name="hasHeaders">(Optional) -Include headers or not</param>
        /// <param name="trimOptions">(Optional) - Trim inside quotes, trim fields, don't trim</param>
        public static void ToCsvFile<T>(
            IEnumerable<T> obj,
            string path,
            char delimiter = ',',
            bool hasHeaders = true,
            bool quoteAllFields = true,
            CsvTrimOptions trimOptions = CsvTrimOptions.TrimFields)
        {
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer))
            {
                csv.Configuration.HasHeaderRecord = hasHeaders;
                csv.Configuration.Delimiter = delimiter.ToString();

                if (quoteAllFields)
                {
                    csv.Configuration.ShouldQuote = (field, context) => true;
                }

                switch (trimOptions)
                {
                    case CsvTrimOptions.None:
                        csv.Configuration.TrimOptions = TrimOptions.None;
                        break;
                    case CsvTrimOptions.TrimInsideQuotes:
                        csv.Configuration.TrimOptions = TrimOptions.InsideQuotes;
                        break;
                    case CsvTrimOptions.TrimFields:
                        csv.Configuration.TrimOptions = TrimOptions.Trim;
                        break;
                    default:
                        break;
                }

                csv.WriteRecords(obj);
            }
        }

        /// <summary>
        /// Write IEnumerable object to CSV file
        /// 
        /// More information available at: https://joshclose.github.io/CsvHelper/getting-started
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="path">Output filepath</param>
        /// <param name="delimiter">(Optional) - Seperator, i.e: ; , \t (tab)</param>
        /// <param name="hasHeaders">(Optional) -Include headers or not</param>
        /// <param name="trimOptions">(Optional) - Trim inside quotes, trim fields, don't trim</param>
        public static string ToCsvString<T>(
            IEnumerable<T> obj,
            char delimiter = ',',
            bool hasHeaders = true,
            bool quoteAllFields = true,
            CsvTrimOptions trimOptions = CsvTrimOptions.TrimFields)
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer))
            {
                csv.Configuration.HasHeaderRecord = hasHeaders;
                csv.Configuration.Delimiter = delimiter.ToString();

                if (quoteAllFields)
                {
                    csv.Configuration.ShouldQuote = (field, context) => true;
                }

                switch (trimOptions)
                {
                    case CsvTrimOptions.None:
                        csv.Configuration.TrimOptions = TrimOptions.None;
                        break;
                    case CsvTrimOptions.TrimInsideQuotes:
                        csv.Configuration.TrimOptions = TrimOptions.InsideQuotes;
                        break;
                    case CsvTrimOptions.TrimFields:
                        csv.Configuration.TrimOptions = TrimOptions.Trim;
                        break;
                    default:
                        break;
                }

                csv.WriteRecords(obj);
                writer.Flush();
                stream.Position = 0;

                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Write List to CSV file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="path">Output filepath</param>
        /// <param name="delimiter">(Optional) - Seperator, i.e: ; , \t (tab)</param>
        /// <param name="hasHeaders">(Optional) -Include headers or not</param>
        /// <param name="trimOptions">(Optional) - Trim inside quotes, trim fields, don't trim</param>
        public static void ToCsvFile<T>(
            List<T> list,
            string path,
            char delimiter = ',',
            bool hasHeaders = true,
            bool quoteAllFields = true,
            CsvTrimOptions trimOptions = CsvTrimOptions.TrimFields)
        {
            ToCsvFile(list.AsEnumerable<T>(), path, delimiter, hasHeaders, quoteAllFields, trimOptions);
        }

        /// <summary>
        /// Write List to CSV file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="path">Output filepath</param>
        /// <param name="delimiter">(Optional) - Seperator, i.e: ; , \t (tab)</param>
        /// <param name="hasHeaders">(Optional) -Include headers or not</param>
        /// <param name="trimOptions">(Optional) - Trim inside quotes, trim fields, don't trim</param>
        public static string ToCSVString<T>(
            List<T> list,
            char delimiter = ',',
            bool hasHeaders = true,
            bool quoteAllFields = true,
            CsvTrimOptions trimOptions = CsvTrimOptions.TrimFields)
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer))
            {
                csv.Configuration.HasHeaderRecord = hasHeaders;
                csv.Configuration.Delimiter = delimiter.ToString();

                if (quoteAllFields)
                {
                    csv.Configuration.ShouldQuote = (field, context) => true;
                }

                switch (trimOptions)
                {
                    case CsvTrimOptions.None:
                        csv.Configuration.TrimOptions = TrimOptions.None;
                        break;
                    case CsvTrimOptions.TrimInsideQuotes:
                        csv.Configuration.TrimOptions = TrimOptions.InsideQuotes;
                        break;
                    case CsvTrimOptions.TrimFields:
                        csv.Configuration.TrimOptions = TrimOptions.Trim;
                        break;
                    default:
                        break;
                }

                csv.WriteRecords(list.AsEnumerable<T>());
                writer.Flush();
                stream.Position = 0;
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Converts DataTable to CSV and write to file
        /// </summary>
        /// <param name="dt">Input DataTable to convert</param>
        /// <param name="path">Filepath to write to</param>
        /// <param name="delimiter">(Optional) - Seperator, i.e: ; , \t (tab)</param>
        /// <param name="hasHeaders">(Optional) - Include headers or not</param>
        /// <param name="trimOptions">(Optional) - Trim inside quotes, trim fields, don't trim</param>
        public static void ToCsvFile(
            DataTable dt,
            string path,
            char delimiter = ',',
            bool hasHeaders = true,
            bool quoteAllFields = true,
            CsvTrimOptions trimOptions = CsvTrimOptions.TrimFields)
        {
            ToCsvFile(dt.AsEnumerable_Legacy(), path, delimiter, hasHeaders, quoteAllFields, trimOptions);
        }

        /// <summary>
        /// Converts DataTable object to CSV type string
        /// </summary>
        /// <param name="dt">DataTable object to convert</param>
        /// <param name="delimiter">(Optional) - Seperator, i.e: ; , \t (tab)</param>
        /// <param name="hasHeaders">(Optional) - Include headers or not</param>
        /// <param name="trimOptions">(Optional) - Trim inside quotes, trim fields, don't trim</param>
        /// <returns>CSV string</returns>
        public static string ToCsvString(
            DataTable dt,
            char delimiter = ',',
            bool hasHeaders = true,
            bool quoteAllFields = true,
            CsvTrimOptions trimOptions = CsvTrimOptions.TrimFields)
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer))
            {
                csv.Configuration.HasHeaderRecord = hasHeaders;
                csv.Configuration.Delimiter = delimiter.ToString();

                if (quoteAllFields)
                {
                    csv.Configuration.ShouldQuote = (field, context) => true;
                }

                switch (trimOptions)
                {
                    case CsvTrimOptions.None:
                        csv.Configuration.TrimOptions = TrimOptions.None;
                        break;
                    case CsvTrimOptions.TrimInsideQuotes:
                        csv.Configuration.TrimOptions = TrimOptions.InsideQuotes;
                        break;
                    case CsvTrimOptions.TrimFields:
                        csv.Configuration.TrimOptions = TrimOptions.Trim;
                        break;
                    default:
                        break;
                }

                csv.WriteRecords(dt.AsEnumerable_Legacy());
                writer.Flush();
                stream.Position = 0;
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Returns a list of CSV headers
        /// </summary>
        /// <param name="path">Path to CSV file</param>
        /// <param name="delimiter">(Optional) - Seperator, i.e: ; , \t (tab)</param>
        /// <returns>Headers list</returns> 
        public static List<string> GetCsvHeaderNames(string path, string delimiter = "auto")
        {
            string singleLine = string.Empty;

            using (StreamReader readingFile = new StreamReader(path))
            {
                singleLine = readingFile.ReadLine();
            }

            char seperator;
            if (delimiter == "auto")
            {
                seperator = TryDetectDelimiter(new string[] { singleLine });
            }
            else
            {
                seperator = delimiter[0];
            }

            string[] headers = singleLine.Split(seperator);

            return headers.ToList();
        }

        /// <summary>
        /// Tries to auto detect CSV delimiter (tends to do a very good job of it).
        /// </summary>
        /// <param name="lines">Text lines array</param>
        /// <returns>Likely delimiter</returns>
        public static char TryDetectDelimiter(string[] lines)
        {
            List<char> separators = new List<char> { ';', ',', '\t', '|' };
            IList<int> separatorsCount = new int[separators.Count];

            bool quoted = false;
            bool firstChar = true;

            for (int i = 0; i < lines.Count(); i++)
            {
                foreach (char cha in lines[i])
                {
                    switch (cha)
                    {
                        case '"':
                            if (quoted)
                            {
                                if (cha != '"') // Value is quoted and 
                                    quoted = false;
                                else
                                    continue;
                            }
                            else
                            {
                                if (firstChar)  // Set value as quoted only if this quote is the 
                                    quoted = true;
                            }
                            break;
                        case '\n':
                            if (!quoted)
                            {
                                firstChar = true;
                                continue;
                            }
                            break;
                        default:
                            if (!quoted)
                            {
                                int index = separators.IndexOf(cha);
                                if (index != -1)
                                {
                                    ++separatorsCount[index];
                                    firstChar = true;
                                    continue;
                                }
                            }
                            break;
                    }

                    if (firstChar)
                        firstChar = false;
                }
            }

            int maxCount = separatorsCount.Max();
            return maxCount == 0 ? '\0' : separators[separatorsCount.IndexOf(maxCount)];
        }
    }
}
