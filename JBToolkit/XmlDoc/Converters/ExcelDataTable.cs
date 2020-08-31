using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;

namespace JBToolkit.XmlDoc.Converters
{
    public class ExcelDataTable
    {
        public static string ExcelContentType
        {
            get
            {
                return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }
        }

        /// <summary>
        /// Convert DataTable to Excel and output byte aray
        /// </summary>
        public static byte[] DataTableToExcel(DataTable dataTable, string heading = "", bool showSrNo = false)
        {
            byte[] result = null;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(String.Format("{0} Data", heading));
                int startRowFrom = String.IsNullOrEmpty(heading) ? 1 : 3;

                if (showSrNo)
                {
                    DataColumn dataColumn = dataTable.Columns.Add("#", typeof(int));
                    dataColumn.SetOrdinal(0);
                    int index = 1;
                    foreach (DataRow item in dataTable.Rows)
                    {
                        item[0] = index;
                        index++;
                    }
                }

                // add the content into the Excel file  
                workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dataTable, true);

                // autofit width of cells with small content  
                int columnIndex = 1;
                foreach (DataColumn column in dataTable.Columns)
                {
                    ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];
                    int maxLength = columnCells.Max(cell => (cell.Value ?? "").ToString().Length);
                    if (maxLength < 150)
                        workSheet.Column(columnIndex).AutoFit();

                    columnIndex++;
                }

                // format header - bold, yellow on black  
                using (ExcelRange r = workSheet.Cells[startRowFrom, 1, startRowFrom, dataTable.Columns.Count])
                {
                    r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    r.Style.Font.Bold = true;
                    r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0095DA"));
                }

                if (dataTable.Rows.Count > 0)
                {
                    // format cells - add borders  
                    using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 1, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
                    {
                        r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                    }
                }

                if (!String.IsNullOrEmpty(heading))
                {
                    workSheet.Cells["A1"].Value = heading;
                    workSheet.Cells["A1"].Style.Font.Size = 20;

                    workSheet.InsertColumn(1, 1);
                    workSheet.InsertRow(1, 1);
                    workSheet.Column(1).Width = 5;
                }

                result = package.GetAsByteArray();
            }

            return result;
        }

        /// <summary>
        /// Convert list to Excel and output byte array
        /// </summary>
        public static byte[] ListToExcel<T>(List<T> data, string Heading = "", bool showSlno = false)
        {
            return DataTableToExcel(ListToDataTable<T>(data), Heading, showSlno);
        }

        /// <summary>
        /// Save List as Excel file
        /// </summary>
        public static void SaveAsExcel<T>(List<T> data, string path, string Heading = "", bool showSlno = false)
        {
            File.WriteAllBytes(path, DataTableToExcel(ListToDataTable<T>(data), Heading, showSlno));
        }

        /// <summary>
        /// Save DataTable as Excel file
        /// </summary>
        public static void SaveAsExcel(DataTable data, string path, string Heading = "", bool showSlno = false)
        {
            File.WriteAllBytes(path, DataTableToExcel(data, Heading, showSlno));
        }

        /// <summary>
        /// Converts Xlsx document to DataTable input as filepath
        /// </summary>
        public static DataTable ExcelToDataTable(string inputPath, bool hasHeaderRow, string worksheetName, out string errorMessages)
        {
            return ExcelWorksheetToDataTable(File.ReadAllBytes(inputPath), hasHeaderRow, worksheetName, out errorMessages);
        }

        /// <summary>
        /// Converts Xlsx document to DataTable input as byte array
        /// </summary>
        public static DataTable ExcelWorksheetToDataTable(byte[] inputBytes, bool hasHeaderRow, string worksheetName, out string errorMessages)
        {
            DataTable dt = new DataTable();
            errorMessages = "";

            //create a new Excel package in a memorystream
            using (MemoryStream stream = new MemoryStream(inputBytes))
            using (ExcelPackage excelPackage = new ExcelPackage(stream))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[worksheetName];

                //check if the worksheet is completely empty
                if (worksheet.Dimension == null)
                    return dt;

                //add the columns to the datatable
                for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
                {
                    string columnName = "Column " + j;
                    var excelCell = worksheet.Cells[1, j].Value;

                    if (excelCell != null)
                    {
                        var excelCellDataType = excelCell;

                        //if there is a headerrow, set the next cell for the datatype and set the column name
                        if (hasHeaderRow == true)
                        {
                            excelCellDataType = worksheet.Cells[2, j].Value;

                            columnName = excelCell.ToString();

                            //check if the column name already exists in the datatable, if so make a unique name
                            if (dt.Columns.Contains(columnName) == true)
                                columnName = columnName + "_" + j;
                        }

                        //try to determine the datatype for the column (by looking at the next column if there is a header row)
                        if (excelCellDataType is DateTime)
                            dt.Columns.Add(columnName, typeof(DateTime));
                        else if (excelCellDataType is bool)
                            dt.Columns.Add(columnName, typeof(bool));
                        else if (excelCellDataType is double)
                        {
                            //determine if the value is a decimal or int by looking for a decimal separator
                            //not the cleanest of solutions but it works since excel always gives a double
                            if (excelCellDataType.ToString().Contains(".") || excelCellDataType.ToString().Contains(","))
                                dt.Columns.Add(columnName, typeof(decimal));
                            else
                                dt.Columns.Add(columnName, typeof(long));
                        }
                        else
                            dt.Columns.Add(columnName, typeof(string));
                    }
                    else
                        dt.Columns.Add(columnName, typeof(string));
                }

                //start adding data the datatable here by looping all rows and columns
                for (int i = worksheet.Dimension.Start.Row + Convert.ToInt32(hasHeaderRow); i <= worksheet.Dimension.End.Row; i++)
                {
                    //create a new datatable row
                    DataRow row = dt.NewRow();

                    //loop all columns
                    for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
                    {
                        var excelCell = worksheet.Cells[i, j].Value;

                        //add cell value to the datatable
                        if (excelCell != null)
                        {
                            try
                            {
                                row[j - 1] = excelCell;
                            }
                            catch
                            {
                                errorMessages += "Row " + (i - 1) + ", Column " + j + ". Invalid " + dt.Columns[j - 1].DataType.ToString().Replace("System.", "") + " value:  " + excelCell.ToString() + "<br>";
                            }
                        }
                    }

                    //add the new row to the datatable
                    dt.Rows.Add(row);
                }
            }

            return dt;
        }

        /// <summary>
        /// Converts Xlsx document to DataTable input as filepath
        /// </summary>
        public static DataTable ExcelToDataTable(string inputPath, bool hasHeaderRow, int worksheetNumber, out string errorMessages)
        {
            return ExcelWorksheetToDataTable(File.ReadAllBytes(inputPath), hasHeaderRow, worksheetNumber, out errorMessages);
        }

        /// <summary>
        /// Converts Xlsx document to DataTable input as byte array
        /// </summary>
        public static DataTable ExcelWorksheetToDataTable(byte[] inputBytes, bool hasHeaderRow, int worksheetNumber, out string errorMessages)
        {
            DataTable dt = new DataTable();
            errorMessages = "";

            //create a new Excel package in a memorystream
            using (MemoryStream stream = new MemoryStream(inputBytes))
            using (ExcelPackage excelPackage = new ExcelPackage(stream))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[worksheetNumber];

                //check if the worksheet is completely empty
                if (worksheet.Dimension == null)
                    return dt;

                //add the columns to the datatable
                for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
                {
                    string columnName = "Column " + j;
                    var excelCell = worksheet.Cells[1, j].Value;

                    if (excelCell != null)
                    {
                        var excelCellDataType = excelCell;

                        //if there is a headerrow, set the next cell for the datatype and set the column name
                        if (hasHeaderRow == true)
                        {
                            excelCellDataType = worksheet.Cells[2, j].Value;

                            columnName = excelCell.ToString();

                            //check if the column name already exists in the datatable, if so make a unique name
                            if (dt.Columns.Contains(columnName) == true)
                                columnName = columnName + "_" + j;
                        }

                        //try to determine the datatype for the column (by looking at the next column if there is a header row)
                        if (excelCellDataType is DateTime)
                            dt.Columns.Add(columnName, typeof(DateTime));
                        else if (excelCellDataType is bool)
                            dt.Columns.Add(columnName, typeof(bool));
                        else if (excelCellDataType is double)
                        {
                            //determine if the value is a decimal or int by looking for a decimal separator
                            //not the cleanest of solutions but it works since excel always gives a double
                            if (excelCellDataType.ToString().Contains(".") || excelCellDataType.ToString().Contains(","))
                                dt.Columns.Add(columnName, typeof(decimal));
                            else
                                dt.Columns.Add(columnName, typeof(long));
                        }
                        else
                            dt.Columns.Add(columnName, typeof(string));
                    }
                    else
                        dt.Columns.Add(columnName, typeof(string));
                }

                //start adding data the datatable here by looping all rows and columns
                for (int i = worksheet.Dimension.Start.Row + Convert.ToInt32(hasHeaderRow); i <= worksheet.Dimension.End.Row; i++)
                {
                    //create a new datatable row
                    DataRow row = dt.NewRow();

                    //loop all columns
                    for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
                    {
                        var excelCell = worksheet.Cells[i, j].Value;

                        //add cell value to the datatable
                        if (excelCell != null)
                        {
                            try
                            {
                                row[j - 1] = excelCell;
                            }
                            catch
                            {
                                errorMessages += "Row " + (i - 1) + ", Column " + j + ". Invalid " + dt.Columns[j - 1].DataType.ToString().Replace("System.", "") + " value:  " + excelCell.ToString() + "<br>";
                            }
                        }
                    }

                    //add the new row to the datatable
                    dt.Rows.Add(row);
                }
            }

            return dt;
        }

        /// <summary>
        /// Converts Xlsx document to DataSet (mulitple DataTables, 1 per sheet) input as file path
        /// </summary>
        public static DataSet ExcelToDataSet(string inputPath, bool hasHeaderRow, out string errorMessages)
        {
            return ExcelToDataSet(File.ReadAllBytes(inputPath), hasHeaderRow, out errorMessages);
        }

        /// <summary>
        /// Converts Xlsx document to DataSet (mulitple DataTables, 1 per sheet) input as byte array
        /// </summary>
        public static DataSet ExcelToDataSet(byte[] inputBytes, bool hasHeaderRow, out string errorMessages)
        {
            DataSet ds = new DataSet();
            errorMessages = "";

            //create a new Excel package in a memorystream
            using (MemoryStream stream = new MemoryStream(inputBytes))
            using (ExcelPackage excelPackage = new ExcelPackage(stream))
            {
                foreach (ExcelWorksheet worksheet in excelPackage.Workbook.Worksheets)
                {
                    DataTable dt = new DataTable(worksheet.Name);

                    //check if the worksheet is completely empty
                    if (worksheet.Dimension == null)
                        return ds;

                    //add the columns to the datatable
                    for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
                    {
                        string columnName = "Column " + j;
                        var excelCell = worksheet.Cells[1, j].Value;

                        if (excelCell != null)
                        {
                            var excelCellDataType = excelCell;

                            //if there is a headerrow, set the next cell for the datatype and set the column name
                            if (hasHeaderRow == true)
                            {
                                excelCellDataType = worksheet.Cells[2, j].Value;

                                columnName = excelCell.ToString();

                                //check if the column name already exists in the datatable, if so make a unique name
                                if (dt.Columns.Contains(columnName) == true)
                                    columnName = columnName + "_" + j;
                            }

                            //try to determine the datatype for the column (by looking at the next column if there is a header row)
                            if (excelCellDataType is DateTime)
                                dt.Columns.Add(columnName, typeof(DateTime));
                            else if (excelCellDataType is bool)
                                dt.Columns.Add(columnName, typeof(bool));
                            else if (excelCellDataType is double)
                            {
                                //determine if the value is a decimal or int by looking for a decimal separator
                                //not the cleanest of solutions but it works since excel always gives a double
                                if (excelCellDataType.ToString().Contains(".") || excelCellDataType.ToString().Contains(","))
                                    dt.Columns.Add(columnName, typeof(decimal));
                                else
                                    dt.Columns.Add(columnName, typeof(long));
                            }
                            else
                                dt.Columns.Add(columnName, typeof(string));
                        }
                        else
                            dt.Columns.Add(columnName, typeof(string));
                    }

                    //start adding data the datatable here by looping all rows and columns
                    for (int i = worksheet.Dimension.Start.Row + Convert.ToInt32(hasHeaderRow); i <= worksheet.Dimension.End.Row; i++)
                    {
                        //create a new datatable row
                        DataRow row = dt.NewRow();

                        //loop all columns
                        for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
                        {
                            var excelCell = worksheet.Cells[i, j].Value;

                            //add cell value to the datatable
                            if (excelCell != null)
                            {
                                try
                                {
                                    row[j - 1] = excelCell;
                                }
                                catch
                                {
                                    errorMessages += "Row " + (i - 1) + ", Column " + j + ". Invalid " + dt.Columns[j - 1].DataType.ToString().Replace("System.", "") + " value:  " + excelCell.ToString() + "<br>";
                                }
                            }
                        }

                        //add the new row to the datatable
                        dt.Rows.Add(row);
                    }

                    ds.Tables.Add(dt);

                }
            }

            return ds;
        }

        /// <summary>
        /// Converts a list to a DataTable
        /// </summary>
        private static DataTable ListToDataTable<T>(List<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable dataTable = new DataTable();

            for (int i = 0; i < properties.Count; i++)
            {
                PropertyDescriptor property = properties[i];
                dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }

            object[] values = new object[properties.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                    values[i] = properties[i].GetValue(item);

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }
}
