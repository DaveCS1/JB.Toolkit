using System.Collections.Generic;
using System.Linq;

namespace JBToolkit.FuzzyLogic
{
    public static partial class Algorithms
    {
        /// <summary>
        /// https://en.wikipedia.org/wiki/Jaro%E2%80%93Winkler_distance
        /// <br /><br />
        /// The Jaro–Winkler distance is a string metric measuring an edit distance between two sequences.
        /// Origin: https://github.com/kdjones/fuzzystring
        /// </summary>
        public static double JaroDistance(this string source, string target)
        {
            int m = source.Intersect(target).Count();

            if (m == 0) { return 0; }
            else
            {
                string sourceTargetIntersetAsString = "";
                string targetSourceIntersetAsString = "";
                IEnumerable<char> sourceIntersectTarget = source.Intersect(target);
                IEnumerable<char> targetIntersectSource = target.Intersect(source);
                foreach (char character in sourceIntersectTarget) { sourceTargetIntersetAsString += character; }
                foreach (char character in targetIntersectSource) { targetSourceIntersetAsString += character; }
                double t = sourceTargetIntersetAsString.LevenshteinDistance(targetSourceIntersetAsString) / 2;
                return ((m / source.Length) + (m / target.Length) + ((m - t) / m)) / 3;
            }
        }
    }
}
