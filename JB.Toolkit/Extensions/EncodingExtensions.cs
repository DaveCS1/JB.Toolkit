using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// String encoding methods (i.e. to ASCII)
    /// </summary>
    public static class EncodingExtensions
    {
        private static readonly Dictionary<char, string> Replacements = new Dictionary<char, string>();

        /// <summary>Returns the specified string with characters not representable in ASCII codepage 437 converted to a suitable representative equivalent.  Yes, this is lossy.</summary>
        /// <param name="s">A string.</param>
        /// <returns>The supplied string, with smart quotes, fractions, accents and punctuation marks 'normalized' to ASCII equivalents.</returns>
        /// <remarks>This method is lossy.</remarks>
        public static string Asciify(this string s)
        {
            string output = (String.Join(String.Empty, s.Select(c => Asciify(c)).ToArray()));
            byte[] tempBytes = Encoding.ASCII.GetBytes(output);
            return Encoding.ASCII.GetString(tempBytes);
        }

        /// <summary>Returns the specified string with characters not representable in ASCII codepage 437 converted to a suitable representative equivalent.  Yes, this is lossy.</summary>
        /// <param name="x">A character.</param>
        /// <returns>The supplied character, with smart quotes, fractions, accents and punctuation marks 'normalized' to ASCII equivalents.</returns>
        /// <remarks>This method is lossy.</remarks>
        public static string Asciify(char x)
        {
            string output = Replacements.ContainsKey(x) ? (Replacements[x]) : (x.ToString());
            byte[] tempBytes = Encoding.ASCII.GetBytes(output);
            return Encoding.ASCII.GetString(tempBytes);
        }

        static EncodingExtensions()
        {
            Replacements['’'] = "'";
            Replacements['–'] = "-";
            Replacements['‘'] = "'";
            Replacements['”'] = "\"";
            Replacements['“'] = "\"";
            Replacements['…'] = "...";
            Replacements['£'] = "GBP";
            Replacements['•'] = "*";
            Replacements[' '] = " ";
            Replacements['é'] = "e";
            Replacements['ï'] = "i";
            Replacements['´'] = "'";
            Replacements['—'] = "-";
            Replacements['·'] = "*";
            Replacements['„'] = "\"";
            Replacements['€'] = "EUR";
            Replacements['®'] = "(R)";
            Replacements['¹'] = "(1)";
            Replacements['«'] = "<<";
            Replacements['è'] = "e";
            Replacements['á'] = "a";
            Replacements['™'] = "TM";
            Replacements['»'] = ">>";
            Replacements['ç'] = "c";
            Replacements['½'] = "1/2";
            Replacements['­'] = "-";
            Replacements['°'] = " degrees ";
            Replacements['ä'] = "a";
            Replacements['É'] = "E";
            Replacements['‚'] = ",";
            Replacements['ü'] = "u";
            Replacements['í'] = "i";
            Replacements['ë'] = "e";
            Replacements['ö'] = "o";
            Replacements['à'] = "a";
            Replacements['¬'] = "-";
            Replacements['ó'] = "o";
            Replacements['â'] = "a";
            Replacements['ñ'] = "n";
            Replacements['ô'] = "o";
            Replacements['¨'] = "";
            Replacements['å'] = "a";
            Replacements['ã'] = "a";
            Replacements['ˆ'] = "^";
            Replacements['©'] = "(c)";
            Replacements['Ä'] = "A";
            Replacements['Ï'] = "I";
            Replacements['ò'] = "o";
            Replacements['ê'] = "e";
            Replacements['î'] = "i";
            Replacements['Ü'] = "U";
            Replacements['Á'] = "A";
            Replacements['ß'] = "ss";
            Replacements['¾'] = "3/4";
            Replacements['È'] = "E";
            Replacements['¼'] = "1/4";
            Replacements['†'] = "+";
            Replacements['³'] = "(3)";
            Replacements['²'] = "(2)";
            Replacements['Ø'] = "O";
            Replacements['¸'] = ",";
            Replacements['Ë'] = "E";
            Replacements['ú'] = "u";
            Replacements['Ö'] = "O";
            Replacements['û'] = "u";
            Replacements['Ú'] = "U";
            Replacements['Œ'] = "Oe";
            Replacements['º'] = "?";
            Replacements['‰'] = "0/00";
            Replacements['Å'] = "A";
            Replacements['ø'] = "o";
            Replacements['˜'] = "~";
            Replacements['æ'] = "ae";
            Replacements['ù'] = "u";
            Replacements['‹'] = "<";
            Replacements['±'] = "+/-";
            Replacements['|'] = " ";
            Replacements['®'] = "(m)";
        }
    }
}