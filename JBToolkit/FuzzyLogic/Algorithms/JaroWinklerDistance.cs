using System;

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
        public static double JaroWinklerDistance(this string source, string target)
        {
            double jaroDistance = source.JaroDistance(target);
            double commonPrefixLength = CommonPrefixLength(source, target);

            return jaroDistance + (commonPrefixLength * 0.1 * (1 - jaroDistance));
        }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Jaro%E2%80%93Winkler_distance
        /// <br /><br />
        /// The Jaro–Winkler distance uses a prefix scale {\displaystyle p}p which gives more favourable ratings to strings that match from 
        /// the beginning for a set prefix length
        /// </summary>
        public static double JaroWinklerDistanceWithPrefixScale(string source, string target, double p)
        {
            double prefixScale;
            if (p > 0.25) { prefixScale = 0.25; } // The maximu mvalue for distance to not exceed 1
            else if (p < 0) { prefixScale = 0; } // The Jaro Distance
            else { prefixScale = p; }

            double jaroDistance = source.JaroDistance(target);
            double commonPrefixLength = CommonPrefixLength(source, target);

            return jaroDistance + (commonPrefixLength * prefixScale * (1 - jaroDistance));
        }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Jaro%E2%80%93Winkler_distance
        /// </summary>
        private static double CommonPrefixLength(string source, string target)
        {
            int maximumPrefixLength = 4;
            int commonPrefixLength = 0;
            if (source.Length <= 4 || target.Length <= 4) { maximumPrefixLength = Math.Min(source.Length, target.Length); }

            for (int i = 0; i < maximumPrefixLength; i++)
            {
                if (source[i].Equals(target[i])) { commonPrefixLength++; }
                else { return commonPrefixLength; }
            }

            return commonPrefixLength;
        }
    }
}
