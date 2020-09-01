using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace System.Web.Mvc
{
    /// <summary>
    /// Class adds extension objects to the HTML options (i.e. @Html.LabelFor) within views. For example the @Html.FileFor is fully built here
    /// and includes the incorporation of custom validators. It's fully re-usable
    /// </summary>
    public static class HtmlExtensions
    {
        public static string[] UnsupportedFilesTypes = new[] {

            "bat", "dll", "exe", "application", "gadget", "msi", "msp", "com", "scr", "hta", "msc", "jar", "cmd", "vb",
            "vbs", "vbe", "jse", "ws", "wsf", "wsc", "wsh", "ps1", "ps1xml", "ps2", "ps2xml", "psc1", "psc2", "msh",
            "msh1", "msh2", "mshxml", "msh1xml", "msh2xml", "scf", "lnk", "inf", "reg" };

        /// <summary>
        /// @FileFor - File picker control
        /// </summary>
        public static MvcHtmlString FileFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            return helper.FileFor(expression, null);
        }

        /// <summary>
        /// @FileFor - File picker control
        /// </summary>
        public static MvcHtmlString FileFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            object className = "";
            object text = "";
            object style = "";

            if (htmlAttributes != null)
            {
                RouteValueDictionary attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

                if (attributes.ContainsKey("style"))
                {
                    attributes.TryGetValue("style", out style);
                }

                if (attributes.ContainsKey("class"))
                {
                    attributes.TryGetValue("class", out className);
                }

                if (attributes.ContainsKey("text"))
                {
                    attributes.TryGetValue("text", out text);
                }
            }

            if (string.IsNullOrEmpty((string)className))
            {
                className = "btn-default";
            }

            if (string.IsNullOrEmpty((string)text))
            {
                text = "Select Files";
            }

            var id = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
            string label = string.Format("<label class=\"btn {1}\" {2} for=\"{0}\">", id, (string)className, string.IsNullOrEmpty((string)style) ? "" : " style= \"" + (string)style + "\"");

            string input = string.Format("<input name=\"{0}\" type=\"file\" id=\"{0}\" style=\"display: none;\" onchange=\"$('#{0}-file-info').html(getFileNamesString(this, 60))\" multiple ></input>{1}", id, text);
            string labelCloseTag = "</label>&nbsp;&nbsp;&nbsp;";
            string span = string.Format("<span class=\"label label-info\" style=\"font-weight: normal; font-size: 10pt;\" id=\"{0}-file-info\">", id);
            string spanCloseTag = "</span>";

            string fullTags = label + input + labelCloseTag + span + spanCloseTag;

            // Render tag
            return MvcHtmlString.Create(fullTags.Replace("&#39;", "'"));
        }

        public enum AjaxMethods
        {
            GET,
            POST
        }

        /// <summary>
        /// @AsyncPartial - Load partial view asynchronously
        /// </summary>
        public static MvcHtmlString AsyncPartial(this HtmlHelper helper, string controller, string action, string previewPartialName = null, AjaxMethods method = AjaxMethods.GET, TempDataDictionary TempData = null)
        {
            string xo = $"x_{controller}_{action}";
            string js = $"function get_{controller}_{action}()" +
                            "{" +
                                    $"var d= document.getElementById('div-{controller}-{action}'); " +
                                    $"var ds= document.getElementById('div-{controller}-{action}-sh'); " +

                                    "var " + xo + " = new XMLHttpRequest();" +
                                    xo + ".onreadystatechange = function() {" +
                                        "  if (" + xo + ".readyState == XMLHttpRequest.DONE ) {" +
                                            "  if (" + xo + ".status == 200) {" +
                                                 $"d.innerHTML = " + xo + ".responseText.replace(/data-partial-refresh/ig, 'data-partial-refresh onclick=\"get_{controller}_{action}()\"');" +
                                                  $"ds.style.display='none';" +
                                                  $"d.style.display='block';" +
                                            "  }" +
                                            "else if (" + xo + ".status == 400) {" +
                                             "      alert('Error 400');" +
                                             "    }" +
                                             "else{" +
                                             "      alert('Generic error');" +
                                             "    }" +
                                         "  }" +
                                    "};" +
                                      $"ds.style.display='block';" +
                                      $"d.style.display='none';" +
                                    xo + $".open('{method}', '/{controller}/{action}', true);" +
                                    xo + ".send();" +
                             "};" +
                             $"get_{controller}_{action}();";

            MvcHtmlString StringPartial = PartialExtensions.Partial(helper, previewPartialName ?? action + "_preview");

            if (TempData != null)
            {
                TempData["script"] += js;
                return MvcHtmlString.Create($"<div  id='div-{controller}-{action}'>" + StringPartial.ToString() + "</div>" +
                                       $"<div  id='div-{controller}-{action}-sh' style='display:none'>" + StringPartial.ToString() + "</div>"
                                       );
            }
            else
            {
                return MvcHtmlString.Create($"<div  id='div-{controller}-{action}'>" + StringPartial.ToString() + "</div>" +
                                      $"<div  id='div-{controller}-{action}-sh' style='display:none'>" + StringPartial.ToString() + "</div>" +
                                      "<script>" + js + "</script>");
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AttachmentAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value,
          ValidationContext validationContext)
        {
#pragma warning disable IDE0019 // Use pattern matching
            IEnumerable<HttpPostedFileBase> files = value as IEnumerable<HttpPostedFileBase>;
#pragma warning restore IDE0019 // Use pattern matching

            int filesSize = 0;
            List<string> detectedUnsupportedFiles = new List<string>();

            if (files == null)
            {
                return ValidationResult.Success;
            }
            else
            {
                foreach (var file in files.ToList())
                {
                    filesSize += file.ContentLength;

                    var fileExt = IO.Path.GetExtension(file.FileName).Substring(1).ToLower();

                    if (HtmlExtensions.UnsupportedFilesTypes.Contains(fileExt))
                    {
                        detectedUnsupportedFiles.Add(fileExt);
                    }
                }
            }

            if (detectedUnsupportedFiles.Count() > 0)
            {
                return new ValidationResult(string.Format("Unsupported file types attached ({0})", detectedUnsupportedFiles.Aggregate((i, j) => i + ", " + j)));
            }

            // The meximum allowed file size is 24MB.
            if (filesSize > 24 * 1024 * 1024)
            {
                return new ValidationResult("Attached files exceeds 24MB.");
            }

            // Everything OK.
            return ValidationResult.Success;
        }
    }
}