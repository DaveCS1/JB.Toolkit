using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace JBToolkit.Extensions.SourceCode
{
    public static class SourceCodeFormatter
    {
        /// <summary>
        /// Formats source code text using Roslyn - Microsoft.CodeAnalsis.CSharp (good for auto generated code).
        /// </summary>
        public static string FormatSourceCodeUsingRoslyn(this string csCode, string indentation = null)
        {
            var tree = CSharpSyntaxTree.ParseText(csCode);
            SyntaxNode root;

            if (!string.IsNullOrEmpty(indentation))
                root = tree.GetRoot().NormalizeWhitespace(indentation: indentation);
            else
                root = tree.GetRoot().NormalizeWhitespace();

            var ret = root.ToFullString();
            return ret;
        }

        /// <summary>
        /// Formats JSON using JSON.Net.
        /// </summary>
        public static string FormatJson(this string json, bool ignoreSyntaxErrors = false)
        {
            try
            {
                dynamic parsedJson = JsonConvert.DeserializeObject(json);
                return JsonConvert.SerializeObject(parsedJson, Newtonsoft.Json.Formatting.Indented);
            }
            catch
            {
                if (ignoreSyntaxErrors)
                    return new LenientJsonFormatter(json).Format();

                throw;
            }
        }

        /// <summary>
        /// Formats XML using XMLWriter.
        /// </summary>
        public static string FormatXml(this string xml, bool ignoreSyntaxErrors = false)
        {
            try
            {
                var stringBuilder = new StringBuilder();
                var element = XElement.Parse(xml);

                var settings = new XmlWriterSettings
                {
                    OmitXmlDeclaration = true,
                    Indent = true,
                    NewLineOnAttributes = true
                };

                using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
                    element.Save(xmlWriter);

                return stringBuilder.ToString();
            }
            catch
            {
                if (ignoreSyntaxErrors)
                {
                    Regex indentingRegex = new Regex(@"\<\s*(?<tag>[\w\-]+)(\s+[\w\-]+\s*=\s*""[^""]*""|'[^']*')*\s*\>[^\<]*\<\s*/\s*\k<tag>\s*\>|\<[!\?]((?<=!)--((?!--\>).)*--\>|(""[^""]*""|'[^']'|[^>])*\>)|\<\s*(?<closing>/)?\s*[\w\-]+(\s+[\w\-]+\s*=\s*""[^""]*""|'[^']*')*\s*((/\s*)|(?<opening>))\>|[^\<]*", RegexOptions.ExplicitCapture | RegexOptions.Singleline);
                    StringBuilder result = new StringBuilder(xml.Length * 2);
                    int indent = 0;
                    for (Match match = indentingRegex.Match(xml); match.Success; match = match.NextMatch())
                    {
                        if (match.Groups["closing"].Success)
                            indent--;
                        result.AppendFormat("{0}{1}\r\n", new String(' ', indent * 2), match.Value);
                        if (match.Groups["opening"].Success && (!match.Groups["closing"].Success))
                            indent++;
                    }
                    return result.ToString();
                }

                throw;
            }
        }

        /// <summary>
        /// Create C# properties class object string from a given SQL table
        /// </summary>
        public static string GetSQLColumnsAsProperties(
            string dbName,
            string connectionString,
            string schemaName,
            string tableOrViewName)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string infoMessage = string.Empty;

                conn.InfoMessage += (sender, e) =>
                {
                    infoMessage = $"{e.Message}";
                };

                conn.Open();
                using (var command = new SqlCommand(@"EXECUTE sp_executesql @cmd", conn))
                {
                    command.Parameters.AddWithValue(
                                            "@cmd",
                                            "EXEC {0}.dbo.UF_Table_To_C_Sharp_Objects {1}, {2}".QFormat(
                                                    dbName,
                                                    tableOrViewName,
                                                    schemaName));
                    command.ExecuteNonQuery();
                }

                return infoMessage.FormatSourceCodeUsingRoslyn();
            }
        }

        /// <summary>
        /// Use to format json without the use of serialisation, in order to try and ignore any syntax errors
        /// </summary>
        public class LenientJsonFormatter
        {
            private readonly StringWalker _walker;
            private readonly IndentWriter _writer = new IndentWriter();
            private readonly StringBuilder _currentLine = new StringBuilder();
            private bool _quoted;

            /// <summary>
            /// Use to format json without the use of serialisation, in order to try and ignore any syntax errors
            /// </summary>
            public LenientJsonFormatter(string json)
            {
                _walker = new StringWalker(json);
                ResetLine();
            }

            public void ResetLine()
            {
                _currentLine.Length = 0;
            }

            public string Format()
            {
                while (MoveNextChar())
                {
                    if (this._quoted == false && this.IsOpenBracket())
                    {
                        this.WriteCurrentLine();
                        this.AddCharToLine();
                        this.WriteCurrentLine();
                        _writer.Indent();
                    }
                    else if (this._quoted == false && this.IsCloseBracket())
                    {
                        this.WriteCurrentLine();
                        _writer.UnIndent();
                        this.AddCharToLine();
                    }
                    else if (this._quoted == false && this.IsColon())
                    {
                        this.AddCharToLine();
                        this.WriteCurrentLine();
                    }
                    else
                    {
                        AddCharToLine();
                    }
                }
                this.WriteCurrentLine();
                return _writer.ToString();
            }

            private bool MoveNextChar()
            {
                bool success = _walker.MoveNext();
                if (this.IsApostrophe())
                {
                    this._quoted = !_quoted;
                }
                return success;
            }

            public bool IsApostrophe()
            {
                return this._walker.CurrentChar == '"' && this._walker.IsEscaped == false;
            }

            public bool IsOpenBracket()
            {
                return this._walker.CurrentChar == '{'
                    || this._walker.CurrentChar == '[';
            }

            public bool IsCloseBracket()
            {
                return this._walker.CurrentChar == '}'
                    || this._walker.CurrentChar == ']';
            }

            public bool IsColon()
            {
                return this._walker.CurrentChar == ',';
            }

            private void AddCharToLine()
            {
                this._currentLine.Append(_walker.CurrentChar);
            }

            private void WriteCurrentLine()
            {
                string line = this._currentLine.ToString().Trim();
                if (line.Length > 0)
                {
                    _writer.WriteLine(line);
                }
                this.ResetLine();
            }

            public class StringWalker
            {
                private readonly string _s;

                public int Index { get; private set; }
                public bool IsEscaped { get; private set; }
                public char CurrentChar { get; private set; }

                public StringWalker(string s)
                {
                    _s = s;
                    this.Index = -1;
                }

                public bool MoveNext()
                {
                    if (this.Index == _s.Length - 1)
                        return false;

                    if (IsEscaped == false)
                        IsEscaped = CurrentChar == '\\';
                    else
                        IsEscaped = false;
                    this.Index++;
                    CurrentChar = _s[Index];
                    return true;
                }
            }

            public class IndentWriter
            {
                private readonly StringBuilder _result = new StringBuilder();
                private int _indentLevel;

                public void Indent()
                {
                    _indentLevel++;
                }

                public void UnIndent()
                {
                    if (_indentLevel > 0)
                        _indentLevel--;
                }

                public void WriteLine(string line)
                {
                    _result.AppendLine(CreateIndent() + line);
                }

                private string CreateIndent()
                {
                    StringBuilder indent = new StringBuilder();
                    for (int i = 0; i < _indentLevel; i++)
                        indent.Append("    ");
                    return indent.ToString();
                }

                public override string ToString()
                {
                    return _result.ToString();
                }
            }
        }
    }
}
