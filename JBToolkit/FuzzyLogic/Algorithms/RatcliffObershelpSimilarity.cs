using System;
using System.Linq;

namespace JBToolkit.FuzzyLogic
{
    public static partial class Algorithms
    {
        /// <summary>
        /// https://en.wikipedia.org/wiki/Gestalt_Pattern_Matching
        /// <br /><br />
        /// Gestalt Pattern Matching[1], also Ratcliff/Obershelp Pattern Recognition[2], is a string-matching algorithms for determining the
        /// similarity of two strings. It was developed in 1983 by John W. Ratcliff and John A. Obershelp and published in the Dr. Dobb's Journal in July 1988.[2]
        /// <br /><br />
        /// calculating twice the number of matching characters {\displaystyle K_{m}}K_{m} divided by the total number of characters of both strings. 
        /// The matching characters are defined as the longest common substring (LCS) plus recursively the number of matching characters in the non-matching regions on both sides of the LCS
        /// <br /><br />
        /// Origin: https://github.com/kdjones/fuzzystring
        /// </summary>
        public static double RatcliffObershelpSimilarity(this string source, string target)
        {
            return (2 * Convert.ToDouble(source.Intersect(target).Count())) / (Convert.ToDouble(source.Length + target.Length));
        }
    }
}
