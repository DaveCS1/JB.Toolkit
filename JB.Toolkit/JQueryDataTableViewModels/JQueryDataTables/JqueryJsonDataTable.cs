using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JBToolkit.Views
{
    /// <summary>
    /// Generates JQuery DataTable HTML from a Model.
    /// 
    /// Uses JSON as a data source.
    /// 
    /// Used in conjunction with the 'JQueryDataTable.cshtml' view (You can just inherit this from your own model)
    /// 
    /// YOUR PROJECT MUST INCLUDE JAVASCRIPT PACKAGES & CSS for Bootstrap, DataTables, DataTables Extensions and Editor
    /// 
    /// <link href = "~/Scripts/packages/bootstrap-3.3.6/css/bootstrap.min.css" rel="stylesheet" />
    /// <link href = "~/Scripts/packages/bootstrap-tree-view/bootstrap-treeview.min.css" rel="stylesheet" />
    /// 
    /// <!-- DataTables -->
    /// <link href = "~/Scripts/packages/DataTables-1.10.20/media/css/dataTables.bootstrap.min.css" rel="stylesheet" />
    /// <link href = "~/Scripts/packages/DataTables-1.10.20/extensions/Buttons/css/buttons.dataTables.min.css" rel="stylesheet" />
    /// <link href = "~/Scripts/packages/DataTables-1.10.20/extensions/Responsive/css/responsive.dataTabl;es.min.css" rel="stylesheet" />
    /// <link href = "~/Scripts/packages/DataTables-1.10.20/extensions/Responsive/css/responsive.boostrap.min.css" rel="stylesheet" />
    /// <link href = "~/Scripts/packages/DataTables-1.10.20/extensions/Scroller/css/scroller.dataTables.min.css" rel="stylesheet" />
    /// <link href = "~/Scripts/packages/DataTables-1.10.20/extensions/Select/css/select.dataTables.min.css" rel="stylesheet" />
    /// <link href = "~/Scripts/packages/DataTables-1.10.20/extensions/ColReorder/css/colReorder.dataTables.min.css" rel="stylesheet" />
    /// <link href = "~/Scripts/packages/DataTables-1.10.20/extensions/Editor/css/editor.bootstrap.min.css" rel="stylesheet" />
    /// <link href = "~/Scripts/packages/DataTables-1.10.20/extensions/Editor/css/editor.dataTables.min.css" rel="stylesheet" />
    /// <script src = "~/Scripts/packages/jquery-1.12.4/jquery-1.12.4.min.js" ></ script >
    /// <script src="~/Scripts/packages/jquery-1.12.4/jquery.validate.min.js"></script>
    /// <script src = "~/Scripts/packages/jquery-1.12.4/jquery.validate.unobtrusive.min.js" ></ script >
    /// <script src="~/Scripts/packages/jquery-1.12.4/jquery.unobtrusive-ajax.min.js"></script>
    /// 
    /// <!-- Bootstrap -->
    /// <script src = "~/Scripts/packages/bootstrap-3.3.6/js/bootstrap.min.js" ></ script >
    /// <script src="~/Scripts/packages/bootstrap-tree-view/bootstrap-treeview.min.js"></script>
    /// 
    /// <!-- DataTables -->
    /// <script src = "~/Scripts/packages/DataTables-1.10.20/media/js/jquery.dataTables.min.js" ></ script >
    /// <script src="~/Scripts/packages/DataTables-1.10.20/media/js/dataTables.bootstrap.min.js"></script>
    /// <script src = "~/Scripts/packages/DataTables-1.10.20/extensions/Buttons/js/dataTables.buttons.min.js" ></ script >
    /// <script src = "~/Scripts/packages/DataTables-1.10.20/extensions/Responsive/js/dataTables.responsive.min.js" ></ script >
    /// <script src = "~/Scripts/packages/DataTables-1.10.20/extensions/Responsive/js/responsive.bootstrap.min.js" ></ script >
    /// <script src="~/Scripts/packages/jszip-2.5.0/jszip.min.js"></script>
    /// <script src = "~/Scripts/packages/pdfmake-0.1.18/pdfmake.min.js" ></ script >
    /// <script src="~/Scripts/packages/pdfmake-0.1.18/vfs_fonts.js"></script>
    /// <script src = "~/Scripts/packages/DataTables-1.10.20/extensions/Scroller/js/dataTables.scroller.min.js" ></ script >
    /// <script src="~/Scripts/packages/DataTables-1.10.20/extensions/ColReorder/js/dataTables.colReorder.min.js"></script>
    /// <script src = "~/Scripts/packages/DataTables-1.10.20/extensions/Select/js/dataTables.select.min.js" ></ script >
    /// <script src="~/Scripts/packages/DataTables-1.10.20/extensions/Buttons/js/buttons.html5.min.js"></script>
    /// <script src = "~/Scripts/packages/DataTables-1.10.20/extensions/Buttons/js/buttons.print.min.js" ></ script >
    /// <script src="~/Scripts/packages/DataTables-1.10.20/extensions/Buttons/js/buttons.colVis.min.js"></script>
    /// <script src = "~/Scripts/packages/DataTables-1.10.20/extensions/Editor/js/dataTables.editor.min.js" ></ script >
    /// <script src="~/Scripts/packages/DataTables-1.10.20/extensions/Editor/js/editor.bootstrap.min.js"></script>
    /// 
    /// Alternatively, you can use a concatonated and minified CDN from here:
    /// https://datatables.net/download/
    /// 
    /// However you will need this regardless:
    /// <script src = "~/Scripts/dataTablesExtensions.js" ></ script >   
    ///     
    /// </summary>
    [Serializable]
    public class JqueryJsonDataTable : _JqueryDataTable
    {
        /// <summary>
        /// Columns - As json string
        /// https://datatables.net/reference/option/columns
        /// </summary>
        public string Columns { get; set; }

        /// <summary>
        /// Option  DataTable column Definitions
        /// https://datatables.net/reference/option/columnDefs
        /// </summary>
        public string ColumnDefinitions { get; set; } = string.Empty;

        /// <summary>
        /// Data - As json string
        /// https://datatables.net/reference/option/columns.data
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// How to sort the column - ASC or DESC (ascending by default - optional) and which column to sort by - As json string
        /// https://datatables.net/reference/option/order
        /// </summary>
        public string Sorting { get; set; }

        /// <summary>
        /// This allows you to run a specific JavaScript function after the 'drawn' event for any extra styling (or anthing else).
        /// Make sure the function name is unique to the scope.
        /// 
        /// The function give the ID of the table. I.e:
        /// 
        /// function(tableId) {
        ///     // Whatever you want to do here
        /// }
        /// </summary>
        public string PostScriptFunctionName { get; set; }

        public JqueryJsonDataTable()
        { }

        public JqueryJsonDataTable(
            string tableName,
            string columns,
            string data,
            string sorting,
            string additionalJavaScript = "",
            string uniqueId = "Default",
            bool useCustomHeader = true,
            bool responsive = false)
        {
            TableName = tableName;
            Columns = columns;
            Data = data;
            AdditionalJavaScript = additionalJavaScript;
            UniqueID = uniqueId;
            UseCustomHeader = useCustomHeader;
            Responsive = responsive;

            if (Sorting == null)
            {
                Sorting = sorting;
            }
            else
            {
                Sorting = sorting;
            }
        }

        /// <summary>
        /// Generates the html for custom filters
        /// </summary>
        public override string GenerateFilters()
        {
            StringBuilder sb = new StringBuilder();

            if (UseCustomHeader)
            {
                dynamic jsonColumns = JsonConvert.DeserializeObject(Columns);

                if (jsonColumns != null)
                {
                    foreach (var column in jsonColumns)
                    {
                        if (column["autofilter"] != null)
                        {
                            if ((bool)column["autofilter"])
                            {
                                sb.Append(
                                @"<li class='dropdown' style='padding: 15px 0px 15px 16px;'>
                                <a href='#' class='dropdown-toggle' data-toggle='dropdown' role='button' aria-haspopup='true' aria-expanded='false' style='padding: 0px !important; margin: 0px !important;'>
                                    " + (column["title"] ?? column["data"]) + @"
                                    <span class='caret'></span>
                                </a>
                                <ul id='tbl-" + UniqueID + @"_JQDT-" + column["data"].ToString().Replace(".", "_") + @"' class='dropdown-menu'>
                                    <li>
                                        <a class='tbl-" + UniqueID + @"_JQDT-" + column["data"].ToString().Replace(".", "_") + @"-Filter' href='#' data=''>
                                            <i class='fa fa-check tbl-" + UniqueID + @"_JQDT-" + column["data"].ToString().Replace(".", "_") + @"-SelectedFilter' aria-hidden='true'></i> All
                                        </a>
                                    </li>
                                    <li role='separator' class='divider'></li>
                                </ul>
                             </li>");
                            }
                        }
                    }
                }
            }
            else
            {
                if (!HideHeader)
                {
                    sb.Append(@"<style>
                            #tbl-" + UniqueID + @"_JQDT_wrapper .dt-buttons {
                                position: relative !important;
                                left: 0px !important;
                            }
                          </style>");
                }
            }

            return sb.ToString().Replace("\n", "").Replace("\t", "").Replace("\r", "");
        }

        /// <summary>
        /// Generates the Jquery DataTable registration call for the DataTable itself
        /// </summary>
        public override string GenerateRegisterDataTableScript()
        {
            return "registerJsonDataTable('" + UniqueID + "_JQDT', '" +
                Sorting.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("  ", " ") + "', '" +
                Columns.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("  ", " ") + "', '" +
                Data.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("  ", " ") + "', '" +
                ColumnDefinitions.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("  ", " ") + "', '" +
                PostScriptFunctionName + "', '" +
                Responsive + "');";
        }

        /// <summary>
        /// Generates the table data in the form of a HTML table
        /// </summary>
        public override string GenerateHtmlTable()
        {
            dynamic jsonColumns = JsonConvert.DeserializeObject(Columns);

            #region Validation

            if (string.IsNullOrEmpty(TableName))
            {
                throw new ApplicationException("JqueryJsonDataTable - No table name defined");
            }

            if (jsonColumns.Count == 0)
            {
                throw new ApplicationException("JqueryJsonDataTable - No headers defined");
            }

            #endregion

            StringBuilder sb = new StringBuilder();

            sb.Append("<table id='tbl-" + UniqueID + "_JQDT' class='table table-striped table-bordered' cellspacing='0' style='width: 100%;'>");

            int index = 0;

            #region Headers

            if (jsonColumns.Count > 0)
            {
                sb.Append("<thead><tr>");
            }

            foreach (var column in jsonColumns)
            {
                sb.Append("<th");

                // AUTO FILTER
                if (column["autofilter"] != null)
                {
                    if ((bool)column["autofilter"])
                    {
                        sb.Append(" data-autofilter='yes' ");
                    }
                }

                // COLUMN ID
                sb.Append(" data-data-columnid=''" + column["data"] + "' ");

                sb.Append(">");

                // TITLE
                if (column["title"] == null)
                {
                    sb.Append(column["data"]);
                }
                else
                {
                    sb.Append(column["title"]);
                }

                sb.Append("</th>");

                index++;
            }

            if (jsonColumns.Count > 0)
            {
                sb.Append("</thead></tr>");
            }

            #endregion

            #region Footers

            if (jsonColumns.Count > 0)
            {
                sb.Append("<tfoot><tr>");
            }

            foreach (var column in jsonColumns)
            {
                sb.Append("<th");

                // AUTO FILTER
                if (column["autofilter"] != null)
                {
                    if ((bool)column["autofilter"])
                    {
                        sb.Append(" data-autofilter='yes' ");
                    }
                }

                //   COLUMN ID
                sb.Append(" data-data-columnid=''" + column["data"] + "' ");

                sb.Append(">");

                // TITLE
                if (column["title"] == null)
                {
                    sb.Append(column["data"]);
                }
                else
                {
                    sb.Append(column["title"]);
                }

                sb.Append("</th>");

                index++;
            }

            if (jsonColumns.Count > 0)
            {
                sb.Append("</tfoot></tr>");
            }

            #endregion

            return sb.ToString();
        }

        /// <summary>
        /// Generates the Jquery DataTable registration call for the filters
        /// </summary>
        public override string GenerateRegisterFiltersScript()
        {
            if (UseCustomHeader)
            {
                StringBuilder sb = new StringBuilder();

                dynamic jsonColumns = JsonConvert.DeserializeObject(Columns);

                if (jsonColumns != null)
                {
                    int index = 0;

                    foreach (var column in jsonColumns)
                    {
                        if (column["autofilter"] != null)
                        {
                            if ((bool)column["autofilter"])
                            {
                                sb.Append(@"registerAutoFilter('" + UniqueID + "_JQDT', '" + column["data"].ToString().Replace(".", "_") + "', " + index + ");");
                            }
                        }

                        index++;
                    }
                }

                return sb.ToString().Replace("\n", "").Replace("\t", "").Replace("\r", "");
            }

            return string.Empty;
        }

        /// <summary>
        /// Generates any additional JavaScript specified to the buttom of the scripts section.
        /// 
        /// * Be aware of JavaScript special characters. Use the '<string>.GetJavaScriptAcceptableString()' extension in most circumstances
        /// </summary>
        public override string GenerateAdditionalJavaScript()
        {
            return AdditionalJavaScript;
        }

        /// <summary>
        /// This is just an example chunk of code for building a JqueryJsonDataTable. Rather than hard code json like the below however, you'd serialise from a model or dynamic object
        /// </summary>
        public async Task<ActionResult> BuildJQueryJsonDataTableExample()
        {
            JqueryJsonDataTable jqModel = new JqueryJsonDataTable
            {
                TableName = "Documents",
                UniqueID = "Default",

                Columns = JsonConvert.SerializeObject(new object[]
                        {
                            new
                            {
                                data = "patientId",
                                title = "Patient ID"
                            },
                            new
                            {
                                data = "otherId",
                                title = "Other ID"
                            },
                            new
                            {
                                data = "firstName",
                                title = "First Name"
                            },
                            new
                            {
                                data = "lastNameT",
                                title = "Last Name"
                            },
                            new
                            {
                                data = "gender",
                                title = "Gender",
                                autofilter = true
                            },
                            new
                            {
                                data = "dob",
                                title = "Date of Birth"
                            }
                            ,
                            new
                            {
                                data = "race",
                                title = "Race",
                                autofilter = true
                            }
                        }),

                Data = JsonConvert.SerializeObject(new object[]
                        {
                            new
                            {
                                patientId = 1,
                                otherId = "LanTest101",
                                firstName = "John",
                                lastName = "Smith",
                                gender = "M",
                                dob = "10/16/1941",
                                race = "Caucasian / White"
                            },
                            new
                            {
                                patientId = 2,
                                otherId = "LanTest102",
                                firstName = "Joe",
                                lastName = "Bloggs",
                                gender = "M",
                                dob = "10/16/1941",
                                race = "Caucasian / White"
                            },
                            new
                            {
                                patientId = 3,
                                otherId = "LanTest103",
                                firstName = "Jane",
                                lastName = "Smith",
                                gender = "F",
                                dob = "10/16/1941",
                                race = "Black"
                            },
                            new
                            {
                                patientId = 4,
                                otherId = "LanTest104",
                                firstName = "Barry",
                                lastName = "Alan",
                                gender = "M",
                                dob = "10/16/1941",
                                race = "Caucasian / White"
                            },
                            new
                            {
                                patientId = 5,
                                otherId = "LanTest105",
                                firstName = "Victor",
                                lastName = "Meldrue",
                                gender = "M",
                                dob = "10/16/1941",
                                race = "Caucasian / White"
                            }
                        }),

                Sorting = JsonConvert.SerializeObject(new object[] { new object[] { 0, "asc" } })
            };

            jqModel.AdditionalJavaScript = "function DoSomething(var parameter) {}";

            return await Task.FromResult(PartialView("../Shared/JQueryDataTable", jqModel));
        }

        private ActionResult PartialView(string v, JqueryJsonDataTable jqModel)
        {
            if (v is null)
            {
                throw new ArgumentNullException(nameof(v));
            }

            if (jqModel is null)
            {
                throw new ArgumentNullException(nameof(jqModel));
            }

            throw new NotImplementedException();
        }
    }
}