using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace JBToolkit.CSV.Native
{
    /// <summary>
    /// Convert CSV from and to a DataTable. This class doesn't require any third party packages to use which can sometimes be useful, 
    /// however it may not account for every single permutation of a CSV file, ony basic. This class does however support strings
    /// wrapped in "Speachmarks" quite nicely, whic quite a lot of CSV to DataTable implementations don't seem to get right.
    /// </summary>
    public class CSVHelper
    {
        /// <summary>
        /// Convert CSV from a DataTable. This class doesn't require any third party packages to use which can sometimes be useful, 
        /// however it may not account for every single permutation of a CSV file, ony basic. This class does however support strings
        /// wrapped in "Speachmarks" quite nicely, whic quite a lot of CSV to DataTable implementations don't seem to get right.
        /// </summary>
        public static DataTable CsvToDataTable(string csvPath, char delimeter = ',')
        {
            if (File.Exists(csvPath))
            {
                string[] lines;
                string csvFilePath = csvPath;

                lines = File.ReadAllLines(csvFilePath);
                while (lines[0].EndsWith(","))
                {
                    lines[0] = lines[0].Remove(lines[0].Length - 1);
                }

                string[] fields;
                fields = lines[0].Split(new char[] { delimeter });
                int cols = fields.GetLength(0);
                var dt = new DataTable();

                var ignoreRowIndexes = new List<int>();

                // 1st row must be column names; force lower case to ensure matching later on.

                for (int i = 0; i < cols; i++)
                {

                    if (!dt.Columns.Contains(fields[i]))
                    {
                        dt.Columns.Add(fields[i], typeof(string));
                    }
                    else
                    {
                        ignoreRowIndexes.Add(i);
                    }
                }

                // Remove any duplicate column names
                var tempFields = fields.ToList();
                for (int i = ignoreRowIndexes.Count - 1; i >= 0; i--)
                {
                    tempFields.RemoveAt(ignoreRowIndexes[i]);
                }

                fields = tempFields.ToArray();

                cols = fields.Length;

                DataRow Row;
                int rowcount = 0;

                try
                {
                    string[] ToBeContinued = new string[] { };
                    bool lineToBeContinued = false;

                    for (int i = 1; i < lines.GetLength(0); i++)
                    {
                        if (!lines[i].Equals(""))
                        {
                            fields = lines[i].Split(new char[] { delimeter });

                            // Remove any duplicate column names
                            tempFields = fields.ToList();
                            for (int k = ignoreRowIndexes.Count - 1; k >= 0; k--)
                            {
                                tempFields.RemoveAt(ignoreRowIndexes[k]);
                            }

                            fields = tempFields.ToArray();

                            string temp0 = string.Join("", fields).Replace("\"\"", "");
                            int quaotCount0 = temp0.Count(c => c == '"');

                            if (fields.GetLength(0) < cols || lineToBeContinued || quaotCount0 % 2 != 0)
                            {
                                if (ToBeContinued.GetLength(0) > 0)
                                {
                                    ToBeContinued[ToBeContinued.Length - 1] += "\n" + fields[0];
                                    fields = fields.Skip(1).ToArray();
                                }

                                string[] newArray = new string[ToBeContinued.Length + fields.Length];
                                Array.Copy(ToBeContinued, newArray, ToBeContinued.Length);
                                Array.Copy(fields, 0, newArray, ToBeContinued.Length, fields.Length);
                                ToBeContinued = newArray;
                                string temp = string.Join("", ToBeContinued).Replace("\"\"", "");
                                int quaotCount = temp.Count(c => c == '"');

                                if (ToBeContinued.GetLength(0) >= cols && quaotCount % 2 == 0)
                                {
                                    fields = ToBeContinued;
                                    ToBeContinued = new string[] { };
                                    lineToBeContinued = false;
                                }
                                else
                                {
                                    lineToBeContinued = true;
                                    continue;
                                }
                            }

                            // handle ',' and '"'
                            // Deserialize CSV following Excel's rule:
                            //  1: If there is commas in a field, quote the field.
                            //  2: Two consecutive quotes indicate a user's quote.

                            var singleLeftquota = new List<int>();
                            var singleRightquota = new List<int>();

                            // combine fileds if number of commas match
                            if (fields.GetLength(0) > cols)
                            {
                                bool lastSingleQuoteIsLeft = true;
                                for (int j = 0; j < fields.GetLength(0); j++)
                                {
                                    bool leftOddquota = false;
                                    bool rightOddquota = false;
                                    if (fields[j].StartsWith("\""))
                                    {
                                        int numberOfConsecutiveQuotes = 0;
                                        foreach (char c in fields[j]) // start with how many "
                                        {
                                            if (c == '"')
                                            {
                                                numberOfConsecutiveQuotes++;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                        if (numberOfConsecutiveQuotes % 2 == 1)// start with odd number of quotes indicate system quote
                                        {
                                            leftOddquota = true;
                                        }
                                    }

                                    if (fields[j].EndsWith("\""))
                                    {
                                        int numberOfConsecutiveQuotes = 0;
                                        for (int jj = fields[j].Length - 1; jj >= 0; jj--)
                                        {
                                            if (fields[j].Substring(jj, 1) == "\"") // end with how many "
                                            {
                                                numberOfConsecutiveQuotes++;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }

                                        if (numberOfConsecutiveQuotes % 2 == 1) // end with odd number of quotes indicate system quote
                                        {
                                            rightOddquota = true;
                                        }
                                    }

                                    if (leftOddquota && !rightOddquota)
                                    {
                                        singleLeftquota.Add(j);
                                        lastSingleQuoteIsLeft = true;
                                    }
                                    else if (!leftOddquota && rightOddquota)
                                    {
                                        singleRightquota.Add(j);
                                        lastSingleQuoteIsLeft = false;
                                    }
                                    else if (fields[j] == "\"") // only one quota in a field
                                    {
                                        if (lastSingleQuoteIsLeft)
                                        {
                                            singleRightquota.Add(j);
                                        }
                                        else
                                        {
                                            singleLeftquota.Add(j);
                                        }
                                    }
                                }

                                if (singleLeftquota.Count == singleRightquota.Count)
                                {
                                    int insideCommas = 0;
                                    for (int indexN = 0; indexN < singleLeftquota.Count; indexN++)
                                    {
                                        insideCommas += singleRightquota[indexN] - singleLeftquota[indexN];
                                    }

                                    if (fields.GetLength(0) - cols >= insideCommas) // probabaly matched
                                    {
                                        int validFildsCount = insideCommas + cols; // (Fields.GetLength(0) - insideCommas) may be exceed the Cols
                                        string[] temp = new string[validFildsCount];
                                        int totalOffSet = 0;

                                        for (int iii = 0; iii < validFildsCount - totalOffSet; iii++)
                                        {
                                            bool combine = false;
                                            int storedIndex = 0;
                                            for (int iInLeft = 0; iInLeft < singleLeftquota.Count; iInLeft++)
                                            {
                                                if (iii + totalOffSet == singleLeftquota[iInLeft])
                                                {
                                                    combine = true;
                                                    storedIndex = iInLeft;
                                                    break;
                                                }
                                            }
                                            if (combine)
                                            {
                                                int offset = singleRightquota[storedIndex] - singleLeftquota[storedIndex];
                                                for (int combineI = 0; combineI <= offset; combineI++)
                                                {
                                                    temp[iii] += fields[iii + totalOffSet + combineI] + ",";
                                                }

                                                temp[iii] = temp[iii].Remove(temp[iii].Length - 1, 1);
                                                totalOffSet += offset;
                                            }
                                            else
                                            {
                                                temp[iii] = fields[iii + totalOffSet];
                                            }
                                        }
                                        fields = temp;
                                    }
                                }
                            }

                            Row = dt.NewRow();
                            for (int f = 0; f < cols; f++)
                            {
                                fields[f] = fields[f].Replace("\"\"", "\""); // Two consecutive quotes indicate a user's quote
                                if (fields[f].StartsWith("\""))
                                {
                                    if (fields[f].EndsWith("\""))
                                    {
                                        fields[f] = fields[f].Remove(0, 1);
                                        if (fields[f].Length > 0)
                                        {
                                            fields[f] = fields[f].Remove(fields[f].Length - 1, 1);
                                        }
                                    }
                                }
                                Row[f] = fields[f];
                            }

                            dt.Rows.Add(Row);
                            rowcount++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("row: " + (rowcount + 2) + ", " + ex.Message);
                }

                return dt;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Convert CSV from and to a DataTable. It will wrapp all fields with speachmarks for simplicity, which is highly compatible, however
        /// If you have a different implementation of CSV reader that type casts you may struggle.
        /// </summary>
        public static void ToCSVFile(DataTable dataTable, string path, char seperator = ',')
        {
            StringBuilder sb = new StringBuilder();
            IEnumerable<string> columnNames = dataTable.Columns.Cast<DataColumn>().
                                                Select(column => column.ColumnName);

            sb.AppendLine(string.Join(seperator.ToString(), columnNames));
            foreach (DataRow row in dataTable.Rows)
            {
                IEnumerable<string> fields = row.ItemArray
                                                    .Select(field => string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));

                sb.AppendLine(string.Join(",", fields));
            }

            File.WriteAllText(path, sb.ToString());
        }
    }
}
