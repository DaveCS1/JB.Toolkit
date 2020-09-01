using System;

namespace JBToolkit.FuzzyLogic
{
    public static partial class Algorithms
    {
        /// <summary>
        /// https://en.wikipedia.org/wiki/Levenshtein_distance
        /// <br /><br />
        /// Calculate the minimum number of single-character edits needed to change the source into the target,
        /// allowing insertions, deletions, and substitutions.
        /// <br/><br/>
        /// Time complexity: at least O(n^2), where n is the length of each string
        /// Accordingly, this algorithm is most efficient when at least one of the strings is very short
        /// <br /><br />
        /// A value of 1 or 2 is okay, 3 is iffy and greater than 4 is a poor match
        /// <br /><br />
        /// Origin: https://github.com/kdjones/fuzzystring
        /// </summary>
        /// <returns>The number of edits required to transform the source into the target. This is at most the length of the longest string, and at least the difference in length between the two strings</returns>
        public static int LevenshteinDistance(this string source, string target)
        {
            int n = source.Length;
            int m = target.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            // Step 7
            return d[n, m];
        }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Levenshtein_distance
        /// <br /><br />
        /// Calculate the minimum number of single-character edits needed to change the source into the target,
        /// allowing insertions, deletions, and substitutions.
        /// <br/><br/>
        /// Accordingly, this algorithm is most efficient when at least one of the strings is very short
        /// <br /><br />        /// 
        /// Damareu-Levenshtein Seems to be more robust for shorter strings
        /// <br /><br />
        /// A value of 1 or 2 is okay, 3 is iffy and greater than 4 is a poor match
        /// <br /><br />
        /// Origin: https://github.com/kdjones/fuzzystring
        /// </summary>
        /// <returns>The number of edits required to transform the source into the target. This is at most the length of the longest string, and at least the difference in length between the two strings</returns>
        public static int DamareuLevenshteinDistance(string source, string target)
        {
            // Return trivial case - where they are equal
            if (source.Equals(target))
            {
                return 0;
            }

            // Return trivial case - where one is empty
            if (String.IsNullOrEmpty(source) || String.IsNullOrEmpty(target))
            {
                return (source ?? "").Length + (target ?? "").Length;
            }


            // Ensure string2 (inner cycle) is longer
            if (source.Length > target.Length)
            {
                var tmp = source;
                source = target;
                target = tmp;
            }

            // Return trivial case - where string1 is contained within string2
            if (target.Contains(source))
            {
                return target.Length - source.Length;
            }

            var length1 = source.Length;
            var length2 = target.Length;

            var d = new int[length1 + 1, length2 + 1];

            for (var i = 0; i <= d.GetUpperBound(0); i++)
            {
                d[i, 0] = i;
            }

            for (var i = 0; i <= d.GetUpperBound(1); i++)
            {
                d[0, i] = i;
            }

            for (var i = 1; i <= d.GetUpperBound(0); i++)
            {
                for (var j = 1; j <= d.GetUpperBound(1); j++)
                {
                    var cost = source[i - 1] == target[j - 1] ? 0 : 1;

                    var del = d[i - 1, j] + 1;
                    var ins = d[i, j - 1] + 1;
                    var sub = d[i - 1, j - 1] + cost;

                    d[i, j] = Math.Min(del, Math.Min(ins, sub));

                    if (i > 1 && j > 1 && source[i - 1] == target[j - 2] && source[i - 2] == target[j - 1])
                    {
                        d[i, j] = Math.Min(d[i, j], d[i - 2, j - 2] + cost);
                    }
                }
            }

            return d[d.GetUpperBound(0), d.GetUpperBound(1)];
        }

        /// <summary>
        /// Calculate the minimum number of single-character edits needed to change the source into the target,
        /// allowing insertions, deletions, and substitutions.
        /// <br/><br/>
        /// Time complexity: at least O(n^2), where n is the length of each string
        /// Accordingly, this algorithm is most efficient when at least one of the strings is very short
        /// </summary>
        /// <returns>The Levenshtein distance, normalized so that the lower bound is always zero, rather than the difference in length between the two strings</returns>
        public static double NormalizedLevenshteinDistance(this string source, string target)
        {
            int unnormalizedLevenshteinDistance = source.LevenshteinDistance(target);

            return unnormalizedLevenshteinDistance - source.LevenshteinDistanceLowerBounds(target);
        }

        /// <summary>
        /// The upper bounds is either the length of the longer string, or the Hamming distance.
        /// </summary>
        public static int LevenshteinDistanceUpperBounds(this string source, string target)
        {
            // If the two strings are the same length then the Hamming Distance is the upper bounds of the Levenshtien Distance.
            if (source.Length == target.Length) { return source.HammingDistance(target); }

            // Otherwise, the upper bound is the length of the longer string.
            else if (source.Length > target.Length) { return source.Length; }
            else if (target.Length > source.Length) { return target.Length; }

            return 9999;
        }

        /// <summary>
        /// The lower bounds is the difference in length between the two strings
        /// </summary>
        public static int LevenshteinDistanceLowerBounds(this string source, string target)
        {
            // If the two strings are different lengths then the lower bounds is the difference in length.
            return Math.Abs(source.Length - target.Length);
        }

    }
}
