using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JBToolkit.Views
{
    /// <summary>
    /// Generates JQquery Feed HTML from a Model
    /// 
    /// Used in conjunction with the 'JQueryFeed.cshtml' view (You can just inherit this from your own model)
    /// 
    /// YOUR PROJECT MUST INCLUDE JAVASCRIPT PACKAGES & CSS for Bootstrap, DataTables, DataTables Extensions and Editor
    /// 
    /// <link href = "~/Scripts/packages/bootstrap-3.3.6/css/bootstrap.min.css" rel="stylesheet" />
    /// <link href = "~/Scripts/packages/bootstrap-tree-view/bootstrap-treeview.min.css" rel="stylesheet" />
    /// 
    /// <!-- DataTables -->
    /// <link href = "~/Scripts/packages/DataTables-1.10.20/media/css/dataTables.bootstrap.min.css" rel="stylesheet" />
    /// <link href = "~/Scripts/packages/DataTables-1.10.20/extensions/Buttons/css/buttons.dataTables.min.css" rel="stylesheet" />
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
    /// <script src = "~/Scripts/dataTablesExtensions.js" ></ script >   
    ///     
    /// </summary>
    [Serializable]
    public class JqueryFeed
    {
        /// <summary>
        /// Main feed title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Custom Filter Header (Replace 'Search' area with something else)
        /// </summary>
        public string CustomFilterHeaderMarkup { get; set; } = string.Empty;

        /// <summary>
        /// Data to show
        /// </summary>
        public List<JQFRow> Rows { get; set; } = new List<JQFRow>();

        /// <summary>
        /// (Optional) Any additional JavaScript to add at the end of the page
        /// </summary>
        public string AdditionalJavaScript { get; set; } = string.Empty;

        public JqueryFeed()
        { }

        public JqueryFeed(string title,
                          List<JQFRow> rows,
                          string additionalJavascript,
                          string uniqueId = "Default")
        {
            Title = title;
            Rows = rows;
            AdditionalJavaScript = additionalJavascript;
            UniqueID = uniqueId;
        }

        /// <summary>
        /// Generates any additional JavaScript specified to the buttom of the scripts section.
        /// 
        /// * Be aware of JavaScript special characters. Use the '<string>.GetJavaScriptAcceptableString()' extension in most circumstances
        /// </summary>
        public string GenerateAdditionalJavaScript()
        {
            return AdditionalJavaScript;
        }

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
                    string cTableName = rgx.Replace(Title, "");

                    m_uniqueId = Guid.NewGuid().ToString() + "_" + cTableName;
                }

                return m_uniqueId;
            }
            set
            {
                m_uniqueId = value;
            }
        }

        /// <summary>
        /// This is just an example chunk of code for building a JQuery Feed. You'd likely dynamically create from a list or DataTable
        /// </summary>
        public async Task<ActionResult> BuildJQueryFeedExample()
        {
            JqueryFeed feed = new JqueryFeed
            {
                Title = "Test Feed",
                UniqueID = "Default",

                Rows = new List<JQFRow>
                {
                    new JQFRow
                    {
                        RowTitle = "Test Row Title 1",
                        Subtitle = "Test Row Subtitle 1",
                        ImageURL = "/Content/img/icon-repo/old-crm/police_hat.png",
                        ButtonAreaText = "Button Area Text",
                        FirstRowHighlightStrips = new List<JQFRowHighlightStrip>
                        {
                            new JQFRowHighlightStrip
                            {
                                HighlightStripType = JQFRowHighlightStrip.HighlightStripTypeEnum.Warning,
                                Text = "Test Strip 1"
                            },
                             new JQFRowHighlightStrip
                            {
                                HighlightStripType = JQFRowHighlightStrip.HighlightStripTypeEnum.Danger,
                                Text = "Test Strip 2"
                            },
                        },
                        SecondRowHighlightStrips = new List<JQFRowHighlightStrip>
                        {
                            new JQFRowHighlightStrip
                            {
                                HighlightStripType = JQFRowHighlightStrip.HighlightStripTypeEnum.Info,
                                Text = "Test Strip 3"
                            }
                        },
                        Buttons = new List<JQFButton>
                        {
                            new JQFButton
                            {
                                Text = "Test Button 1",
                                Url = "https://www.google.co.uk",
                                Target = JQFButton.ButtonTargetEnum.Blank
                            }
                        },
                        Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat."
                    },
                    new JQFRow
                    {
                        RowTitle = "Test Row Title 2",
                        Subtitle = "Test Row Subtitle 2",
                        ImageURL = "/Content/img/icon-repo/old-crm/police_hat.png",
                        ButtonAreaText = "Button Area Text",
                        FirstRowHighlightStrips = new List<JQFRowHighlightStrip>
                        {
                            new JQFRowHighlightStrip
                            {
                                HighlightStripType = JQFRowHighlightStrip.HighlightStripTypeEnum.Warning,
                                Text = "Test Strip 1"
                            },
                             new JQFRowHighlightStrip
                            {
                                HighlightStripType = JQFRowHighlightStrip.HighlightStripTypeEnum.Danger,
                                Text = "Test Strip 2"
                            },
                        },
                        SecondRowHighlightStrips = new List<JQFRowHighlightStrip>
                        {
                            new JQFRowHighlightStrip
                            {
                                HighlightStripType = JQFRowHighlightStrip.HighlightStripTypeEnum.Info,
                                Text = "Test Strip 3"
                            }
                        },
                        Buttons = new List<JQFButton>
                        {
                            new JQFButton
                            {
                                Text = "Test Button 1",
                                Url = "https://www.google.co.uk",
                                IconClass = "glyphicon glyphicon-new-window",
                                Target = JQFButton.ButtonTargetEnum.Blank

                            }
                        },
                        Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat."
                    }
                }
            };

            return await Task.FromResult(PartialView("../Shared/JQueryFeed", feed));
        }

        private ActionResult PartialView(string v, JqueryFeed jqModel)
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

    [Serializable]
    public class JQFRow
    {
        public string RowTitle { get; set; }
        public string Subtitle { get; set; }
        public string ImageURL { get; set; }
        public string ButtonAreaText { get; set; }

        public string Content { get; set; }

        public List<JQFRowHighlightStrip> FirstRowHighlightStrips { get; set; }
        public List<JQFRowHighlightStrip> SecondRowHighlightStrips { get; set; }
        public List<JQFButton> Buttons { get; set; }
    }

    [Serializable]
    public class JQFButton
    {
        public enum ButtonTargetEnum
        {
            Blank,
            Self,
            Top,
            Parent
        }

        public string Url { get; set; }
        public ButtonTargetEnum Target { get; set; } = ButtonTargetEnum.Self;
        public string Text { get; set; }
        public string ButtonClass { get; set; }
        public string IconClass { get; set; }
    }

    [Serializable]
    public class JQFRowHighlightStrip
    {
        /// <summary>
        /// If using 'Custom' - be sure to set the CustomHighlight color, i.e. #3f80ea
        /// </summary>
        [Serializable]
        public enum HighlightStripTypeEnum
        {
            Success,
            Warning,
            Danger,
            Info,
            Primary,
            Grey,
            Custom
        }

        public HighlightStripTypeEnum HighlightStripType { get; set; }
        public string CustomHighlighColour { get; set; }
        public string Text { get; set; }
    }
}

