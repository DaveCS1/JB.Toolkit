using Newtonsoft.Json;
using System;
using System.Collections.Generic;
namespace JBToolkit.Views
{
    /// <summary>
    /// Turns the Ajax call parameters into a DataTableParameter object
    /// </summary>
    [Serializable]
    public class DataTableParameters
    {
#pragma warning disable IDE1006 // Naming Styles
        public Parameters parameters { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public DataTableParameters()
        { }
    }

    [Serializable]
    public class Parameters
    {
        public int draw;
        public DataTableColumn[] columns;
        public DataTableOrder[] order;
        public int start;
        public int length;
        public DataTableSearch search;

        public Parameters()
        { }
    }

    [Serializable]
    public class DataTableColumn
    {
        public string data;
        public string name;
        public bool searchable;
        public bool orderable;
        public DataTableSearch search;
        public DataTableColumn()
        { }
    }

    [Serializable]
    public class DataTableOrder
    {
        public int column;
        public string dir;

        public DataTableOrder()
        { }
    }

    [Serializable]
    public class DataTableSearch
    {
        public string value;
        public bool regex;

        public DataTableSearch()
        { }
    }

    /// <summary>
    /// Data returned to DataTable
    /// </summary>
    [Serializable]
    public class DataTableResultSet
    {
        /// <summary>Array of records. Each element of the array is itself an array of columns</summary>
        public List<object> data = new List<object>();

        public object customData;

        /// <summary>value of draw parameter sent by client</summary>
        public int draw;

        /// <summary>filtered record count</summary>
        public int recordsFiltered;

        /// <summary>total record count in resultset</summary>
        public int recordsTotal;

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    [Serializable]
    public class DataTableResultError : DataTableResultSet
    {
        public string error;
    }
}