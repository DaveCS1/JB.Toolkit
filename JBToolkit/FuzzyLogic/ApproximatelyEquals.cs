using System;
using System.Collections.Generic;
using System.Linq;

namespace JBToolkit.FuzzyLogic
{
    /// <summary>
    /// Origin (before extension): https://github.com/kdjones/fuzzystring
    /// </summary>
    public static partial class Algorithms
    {
        /// <summary>
        /// Determines the average comparison result of all the algorithms to check again and depending on the required tolerance outputs a true or false decision.
        /// Towards 1 = bad, towards 0 = good (i.e. distance)
        /// </summary>
        /// <param name="source">Source string</param>
        /// <param name="target">String we're comparing against</param>
        /// <param name="tolerance">Enum - week, normal, strong</param>
        /// <param name="options">Provided algorithms to use</param>
        /// <returns>True or false decision of match</returns>
        public static double MatchComparisonResultAverage(
            this string source,
            string target,
            params FuzzyStringComparisonOptions[] options)
        {
            _ = source.ApproximatelyEquals(target, FuzzyStringComparisonTolerance.Manual, out double comparisonResultAverage, null, options);

            return comparisonResultAverage;
        }

        /// <summary>
        /// Determines the average comparison result of all the algorithms to check again and depending on the required tolerance outputs a true or false decision
        /// </summary>
        /// <param name="source">Source string</param>
        /// <param name="target">String we're comparing against</param>
        /// <param name="tolerance">Enum - week, normal, strong</param>
        /// <param name="options">Provided algorithms to use</param>
        /// <returns>True or false decision of match</returns>
        public static bool ApproximatelyEquals(
            this string source,
            string target,
            FuzzyStringComparisonTolerance tolerance,
            double? manualToleranceAmount = null,
            params FuzzyStringComparisonOptions[] options)
        {
            return source.ApproximatelyEquals(target, tolerance, out _, manualToleranceAmount, options);
        }

        /// <summary>
        /// Determines the average comparison result of all the algorithms to check again and depending on the required tolerance outputs a true or false decision
        /// </summary>
        /// <param name="source">Source string</param>
        /// <param name="target">String we're comparing against</param>
        /// <param name="tolerance">Enum - week, normal, strong</param>
        /// <param name="comparisonResultAverage">The comparison average results (0.0 - 1.0)</param>
        /// <param name="options">Provided algorithms to use</param>
        /// <returns>True or false decision of match</returns>
        public static bool ApproximatelyEquals(
            this string source,
            string target,
            FuzzyStringComparisonTolerance tolerance,
            out double comparisonResultAverage,
            double? manualToleranceAmount = null,
            params FuzzyStringComparisonOptions[] options)
        {
            List<double> comparisonResults = new List<double>();

            if (!options.Contains(FuzzyStringComparisonOptions.CaseSensitive))
            {
                source = source.Capitalize();
                target = target.Capitalize();
            }

            // Min: 0    Max: source.Length = target.Length
            if (options.Contains(FuzzyStringComparisonOptions.UseHammingDistance))
            {
                if (source.Length == target.Length)
                {
                    comparisonResults.Add(Operations.PerformDivision(source.HammingDistance(target), target.Length));
                }
            }

            // Min: 0    Max: 1
            if (options.Contains(FuzzyStringComparisonOptions.UseJaccardDistance))
            {
                comparisonResults.Add(source.JaccardDistance(target));
            }

            // Min: 0    Max: 1
            if (options.Contains(FuzzyStringComparisonOptions.UseJaroDistance))
            {
                comparisonResults.Add(source.JaroDistance(target));
            }

            // Min: 0    Max: 1
            if (options.Contains(FuzzyStringComparisonOptions.UseJaroWinklerDistance))
            {
                comparisonResults.Add(source.JaroWinklerDistance(target));
            }

            // Min: 0    Max: LevenshteinDistanceUpperBounds - LevenshteinDistanceLowerBounds
            // Min: LevenshteinDistanceLowerBounds    Max: LevenshteinDistanceUpperBounds
            if (options.Contains(FuzzyStringComparisonOptions.UseNormalizedLevenshteinDistance))
            {
                comparisonResults.Add(
                    Operations.PerformDivision(
                        source.NormalizedLevenshteinDistance(target),
                        (Math.Max(source.Length, target.Length)) - source.LevenshteinDistanceLowerBounds(target)));
            }

            else if (options.Contains(FuzzyStringComparisonOptions.UseDamareuLevenshteinDistance))
            {
                comparisonResults.Add(
                    Operations.PerformDivision(
                        source.LevenshteinDistance(target),
                        source.LevenshteinDistanceUpperBounds(target)));
            }

            else if (options.Contains(FuzzyStringComparisonOptions.UseHammingDistance))
            {
                comparisonResults.Add(
                    Operations.PerformDivision(
                        source.LevenshteinDistance(target),
                        source.LevenshteinDistanceUpperBounds(target)));
            }

            if (options.Contains(FuzzyStringComparisonOptions.UseLongestCommonSubsequence))
            {
                comparisonResults.Add(1 - Convert.ToDouble(Operations.PerformDivision(
                    (source.LongestCommonSubsequence(target).Length),
                    Convert.ToDouble(Math.Min(source.Length, target.Length)))));
            }

            if (options.Contains(FuzzyStringComparisonOptions.UseLongestCommonSubstring))
            {
                comparisonResults.Add(1 - Convert.ToDouble(Operations.PerformDivision(
                    (source.LongestCommonSubstring(target).Length),
                    Convert.ToDouble(Math.Min(source.Length, target.Length)))));
            }
            // Min: 0    Max: 1
            if (options.Contains(FuzzyStringComparisonOptions.UseSorensenDiceDistance))
            {
                comparisonResults.Add(source.SorensenDiceDistance(target));
            }

            // Min: 0    Max: 1
            if (options.Contains(FuzzyStringComparisonOptions.UseOverlapCoefficient))
            {
                comparisonResults.Add(1 - source.OverlapCoefficient(target));
            }

            // Min: 0    Max: 1
            if (options.Contains(FuzzyStringComparisonOptions.UseRatcliffObershelpSimilarity))
            {
                comparisonResults.Add(1 - source.RatcliffObershelpSimilarity(target));
            }

            comparisonResultAverage = 0;

            if (comparisonResults.Count == 0)
            {
                return false;
            }

            comparisonResultAverage = comparisonResults.Average();

            if (tolerance == FuzzyStringComparisonTolerance.Strong)
            {
                if (comparisonResultAverage < 0.25)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (tolerance == FuzzyStringComparisonTolerance.Normal)
            {
                if (comparisonResultAverage < 0.5)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (tolerance == FuzzyStringComparisonTolerance.Weak)
            {
                if (comparisonResultAverage < 0.75)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (tolerance == FuzzyStringComparisonTolerance.Manual)
            {
                if (manualToleranceAmount == null)
                {

                    if (comparisonResultAverage < 0.6)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (comparisonResultAverage < manualToleranceAmount)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
    }
}
