using System;
using System.Collections.Generic;
using static JBToolkit.FuzzyLogic.Algorithms;

namespace JBToolkit.FuzzyLogic
{
    internal class FuzzyLogicExample
    {
#pragma warning disable IDE0051 // Remove unused private members
        private void ApproximatelyEqualsExample()
#pragma warning restore IDE0051 // Remove unused private members
        {
            string kevin = "kevin";
            string kevyn = "kevyn";

            // options and algorithms to include in the comparison
            List<FuzzyStringComparisonOptions> options = new List<FuzzyStringComparisonOptions>
            {
                FuzzyStringComparisonOptions.UseJaccardDistance,
                FuzzyStringComparisonOptions.UseNormalizedLevenshteinDistance,
                FuzzyStringComparisonOptions.UseOverlapCoefficient,
                FuzzyStringComparisonOptions.UseLongestCommonSubsequence,
                FuzzyStringComparisonOptions.CaseSensitive
            };

            Console.WriteLine(kevin.ApproximatelyEquals(kevyn, FuzzyStringComparisonTolerance.Weak, null, options.ToArray()));
            Console.WriteLine(kevin.ApproximatelyEquals(kevyn, FuzzyStringComparisonTolerance.Normal, null, options.ToArray()));
            Console.WriteLine(kevin.ApproximatelyEquals(kevyn, FuzzyStringComparisonTolerance.Strong, null, options.ToArray()));
        }

#pragma warning disable IDE0051 // Remove unused private members
        private void BestMatchExample()
#pragma warning restore IDE0051 // Remove unused private members
        {
            List<string> compareList = new List<string> { "william", "simon", "geoff", "edward", "lisa", "kevin", "kevyn" };

            // options and algorithms to include in the comparison
            List<FuzzyStringComparisonOptions> options = new List<FuzzyStringComparisonOptions>
            {
                FuzzyStringComparisonOptions.UseJaccardDistance,
                FuzzyStringComparisonOptions.UseNormalizedLevenshteinDistance,
                FuzzyStringComparisonOptions.UseOverlapCoefficient,
                FuzzyStringComparisonOptions.UseLongestCommonSubsequence
            };

            BestFuzzyMatch bestMatch = "kevine".FindBestBestFuzzyMatchDetailed(compareList, true, true, options.ToArray());

            Console.Out.WriteLine(bestMatch.MatchText);
        }

#pragma warning disable IDE0051 // Remove unused private members
        private void DistanceExample()
#pragma warning restore IDE0051 // Remove unused private members
        {
            Console.Out.WriteLine("thisString".HammingDistance("thisThing"));
            Console.Out.WriteLine("thisString".JaccardDistance("thisThing"));
            Console.Out.WriteLine("thisString".JaroDistance("thisThing"));
            Console.Out.WriteLine("thisString".JaroWinklerDistance("thisThing"));
            Console.Out.WriteLine("thisString".LevenshteinDistance("thisThing"));
            Console.Out.WriteLine("thisString".LongestCommonSubsequence("thisThing"));
            Console.Out.WriteLine("thisString".LongestCommonSubstring("thisThing"));
            Console.Out.WriteLine("thisString".NormalizedLevenshteinDistance("thisThing"));
            Console.Out.WriteLine("thisString".OverlapCoefficient("thisThing"));
            Console.Out.WriteLine("thisString".RatcliffObershelpSimilarity("thisThing"));
            Console.Out.WriteLine("thisString".SorensenDiceDistance("thisThing"));
            Console.Out.WriteLine("thisString".TanimotoCoefficient("thisThing"));
        }
    }
}
