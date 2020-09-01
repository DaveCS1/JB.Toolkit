using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JBToolkit.Views
{
    /// <summary>
    /// Editable DataTable: Generates JQquery DataTable HTML from a Model that you can edit fields for.
    /// 
    /// Uses JSON as for data and parameters.
    /// 
    /// Used in conjunction with the 'JQueryDataTable.cshtml' view (You can just inherit this from your own model if you wish)
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
    ///     
    /// </summary>
    [Serializable]
    public class JqueryEditorDataTable : _JqueryDataTable
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
        /// Http GET and Http POST Url
        ///  https://datatables.net/reference/option/ajax
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Editor fields as Json
        /// https://editor.datatables.net/reference/type/DataTables.Editor.Field
        /// </summary>
        public string Fields { get; set; }

        /// <summary>
        /// How to sort the column - ASC or DESC (ascending by default - optional) and which column to sort by - As json string
        /// https://datatables.net/reference/option/order
        /// </summary>
        public string Sorting { get; set; }

        /// <summary>
        /// Edit a cell directly from the table, rather than opening up a modal dialog to edit
        /// </summary>
        public bool EnableInLineEditing { get; set; } = false;

        /// <summary>
        /// Edit cells with a modal dialoge
        /// </summary>
        public bool EnableBubbleEditing { get; set; } = false;

        /// <summary>
        /// Disable the 'edit' column on the last column
        /// </summary>
        public bool DisableActionColumn { get; set; } = false;

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

        public JqueryEditorDataTable()
        { }

        public JqueryEditorDataTable(
            string tableName,
            string columns,
            string columnDefinitions,
            string url,
            string fields,
            string sorting,
            string additionalJavaScript = "",
            string uniqueId = "Default",
            bool useCustomHeader = true,
            bool responsive = false)
        {
            TableName = tableName;
            Columns = columns;
            ColumnDefinitions = columnDefinitions;
            Fields = fields;
            Url = url;
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
            string register = "registerEditorDataTable('" + UniqueID + "_JQDT', '" +
                Sorting.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("  ", " ") + "', '" +
                Columns.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("  ", " ") + "', '" +
                Fields.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("  ", " ") + "', '" +
                EnableInLineEditing.ToString().Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("  ", " ") + "', '" +
                EnableBubbleEditing.ToString().Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("  ", " ") + "', '" +
                DisableActionColumn.ToString().Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("  ", " ") + "', '" +
                ColumnDefinitions.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("  ", " ") + "', '" +
                Url + "', '" +
                PostScriptFunctionName + "', '" +
                Responsive + "');";

            return register;
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
                throw new ApplicationException("JqueryEditorDataTable - No table name defined");
            }

            if (jsonColumns.Count == 0)
            {
                throw new ApplicationException("JqueryEditorDataTable - No headers defined");
            }

            if (EnableBubbleEditing && EnableInLineEditing)
            {
                throw new ApplicationException("JqueryEditorDataTableS - Both bubble editing and in-line editing have been enabled. You can only set one or the other");
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

            if (!DisableActionColumn)
            {
                sb.Append("<th></th>"); // Action column
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

            if (!DisableActionColumn)
            {
                sb.Append("<th></th>"); // Action column
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
        /// This is just an example chunk of code for building a JqueryJsonDataTable. Rather than hard code json, you'd serialise from a model or dynamic object as below
        /// </summary>
        public async Task<ActionResult> BuildJQueryEditorDataTableExample()
        {
            JqueryEditorDataTable jqModel = new JqueryEditorDataTable
            {
                TableName = "Permissions",
                UniqueID = "Default",

                Columns = JsonConvert.SerializeObject(new object[]
                        {
                            new
                            {
                                data = "USR_AG_SS_ProfileOptions_T.Option_Name_VC",
                                title = "Option Name",
                                autofilter = true
                            },
                            new
                            {
                                data = "USR_AG_SS_ProfileOptions_T.Area_VC",
                                title = "Area",
                                autofilter = true
                            },
                            new
                            {
                                data = "departments.Description_VC",
                                title = "Department",
                                autofilter = true
                            },
                            new
                            {
                                data = "USR_AG_SS_ProfileOptions_T.Include_Subs_BIT",
                                title = "Include Subs"
                            },
                            new
                            {
                                data = "USR_AG_SS_ProfileOptions_T.Additional_Data_VC",
                                title = "Additional"
                            }
                        }),

                Sorting = JsonConvert.SerializeObject(new object[] { new object[] { 0, "asc" } }),

                Fields = JsonConvert.SerializeObject(new object[]
                        {
                            new
                            {
                                label = "Option Name",
                                name = "USR_AG_SS_ProfileOptions_T.Option_Name_VC"
                            },
                            new
                            {
                                label = "Area",
                                name = "USR_AG_SS_ProfileOptions_T.Area_VC"
                            },
                            new
                            {
                                label = "Department",
                                name = "USR_AG_SS_ProfileOptions_T.Department_ID",
                                type = "select"
                            },
                            new
                            {
                                label = "Include Subs",
                                name = "USR_AG_SS_ProfileOptions_T.Include_Subs_BIT",
                                type = "select",
                                options = new object[]
                                {
                                    new
                                    {
                                        label = "Yes",
                                        value = "True"
                                    },
                                    new
                                    {
                                        label = "No",
                                        value = "False"
                                    }
                                }
                            },
                            new
                            {
                                label = "Notes",
                                name = "USR_AG_SS_ProfileOptions_T.Additional_Data_VC"
                            },
                        }),

                Url = "/api/AdminApi/"
            };

            jqModel.AdditionalJavaScript = "function DoSomething(var parameter) {}";

            return await Task.FromResult(PartialView("../Shared/JQueryDataTable", jqModel));

            /*
             
            // Exmaple API controller Action

            [NoCache]
            [NoCacheForced]
            [ActionSessionState(System.Web.SessionState.SessionStateBehavior.Required)]
            [SetPartial]
            [HttpGet]
            [HttpPost]
            public IHttpActionResult ProfileOptions()
            {
            var request = HttpContext.Current.Request;

            DBAdmin dbCon = new DBAdmin(Global.UserID);

            using (var db = new Database("sqlserver", Global.ConnectionSettings.ConnectionString))
            {
                var dtEditorResponse = new Editor(db, "USR_AG_SS_ProfileOptions_T", "Profile_Option_ID")

                    .Field(new Field("USR_AG_SS_ProfileOptions_T.Department_ID")

                        .Options(new Options()
                            .Table("Shared_Departments_T")
                            .Value("Shared_Departments_T.Department_ID")
                            .Label("TRIM(Description_VC)")
                            .Where(q => q
                                .Where("Shared_Departments_T.Department_ID", "428", "<>")
                                .AndWhere("Shared_Departments_T.Department_ID", "374", "<>")
                                .AndWhere("Shared_Departments_T.Department_ID", "370", "<>")
                                .AndWhere("Shared_Departments_T.Department_ID", "368", "<>")
                                .AndWhere("Shared_Departments_T.Department_ID", "311", "<>")
                                .AndWhere("Shared_Departments_T.Department_ID", "302", "<>")
                                .AndWhere("Shared_Departments_T.Department_ID", "1", "<>")
                                .AndWhere("Shared_Departments_T.Department_ID", "394", "<>")
                                .AndWhere("Shared_Departments_T.Department_ID", "323", "<>"))
                            .Order("TRIM(Shared_Departments_T.Description_VC)")))

                    .Field(new Field("departments.Description_VC"))

                    .Field(new Field("USR_AG_SS_ProfileOptions_T.Area_VC")
                        .Options(() => dbCon.GetAreas())
                        .Validator(Validation.MinLen(3))
                        .Validator(Validation.MaxLen(50))
                        .Validator(Validation.NotEmpty()))

                    .Field(new Field("USR_AG_SS_ProfileOptions_T.Option_Name_VC")
                        .Options(() => dbCon.GetOptionsNames())
                        .Validator(Validation.MinLen(3))
                        .Validator(Validation.MaxLen(50))
                        .Validator(Validation.NotEmpty()))

                    .Field(new Field("USR_AG_SS_ProfileOptions_T.Include_Subs_BIT")
                        .Options(() => new List<Dictionary<string, object>> {
                                        new Dictionary<string, object>{ {"value", "True"}, {"label", "Yes"} },
                                        new Dictionary<string, object>{ {"value", "False"}, {"label", "No"} },
                                }))

                    .Field(new Field("USR_AG_SS_ProfileOptions_T.Additional_Data_VC"))
                        .LeftJoin("Shared_Departments_T as departments", "departments.Department_ID", "=", "USR_AG_SS_ProfileOptions_T.Department_ID")

                    .Process(request)
                    .Data();

                return Json(dtEditorResponse);
            }

            */
        }

        private ActionResult PartialView(string v, JqueryEditorDataTable jqModel)
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