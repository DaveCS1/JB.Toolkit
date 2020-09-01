using System;

namespace JBToolkit.Views
{
    /// <summary>
    /// Generates JQquery DataTable HTML from a Model
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
    ///     
    /// </summary>
    [Serializable]
#pragma warning disable IDE1006 // Naming Styles
    public abstract class _JqueryDataTable : IJqueryDataTable
#pragma warning restore IDE1006 // Naming Styles
    {
        /// <summary>
        /// Table name - will display as header - can included spaces
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Hides the top left header table name title text
        /// </summary>
        public bool HideTitle { get; set; } = false;

        /// <summary>
        /// Pulls the navbar bottom border up 2px if there's a gap (sometimes happenes with displaying datatable within a nested divs)
        /// </summary>
        public bool PullNavBarUp { get; set; } = false;

        /// <summary>
        /// Whether or not to include the cool custom header or just use the DataTable default
        /// </summary>
        public bool UseCustomHeader { get; set; } = true;

        /// <summary>
        /// Completely hide the auto filter, search, display amount and buttons
        /// </summary>
        public bool HideHeader { get; set; } = false;

        /// <summary>
        /// Collapse columns that don't find into the width of the screen / window
        /// </summary>
        public bool Responsive { get; set; } = false;

        /// <summary>
        /// Adds custom JavaScript To Bottom of Page
        /// </summary>
        public string AdditionalJavaScript { get; set; } = string.Empty;

        private string m_uniqueId = string.Empty;

        /// <summary>
        /// Used so we don't have HTML ID clashes
        /// </summary>
        public string UniqueID
        {
            get
            {
                if (string.IsNullOrEmpty(m_uniqueId) || m_uniqueId.ToLower() == "default")
                {
                    System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex("[^a-zA-Z0-9 -]");
                    string cTableName = rgx.Replace(TableName, "").Replace(" ", "");

                    m_uniqueId = Guid.NewGuid().ToString() + "_" + cTableName;
                }

                return m_uniqueId;
            }
            set
            {
                m_uniqueId = value;
            }
        }

        public abstract string GenerateFilters();

        public abstract string GenerateRegisterDataTableScript();

        public abstract string GenerateRegisterFiltersScript();

        public abstract string GenerateAdditionalJavaScript();

        public abstract string GenerateHtmlTable();
    }
}