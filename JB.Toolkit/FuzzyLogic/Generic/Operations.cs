using System;
using System.Collections.Generic;
using System.Linq;

namespace JBToolkit.FuzzyLogic
{
    public enum FuzzyStringComparisonOptions
    {
        UseHammingDistance,

        UseJaccardDistance,

        UseJaroDistance,

        UseJaroWinklerDistance,

        UseLevenshteinDistance,

        UseDamareuLevenshteinDistance,

        UseLongestCommonSubsequence,

        UseLongestCommonSubstring,

        UseNormalizedLevenshteinDistance,

        UseOverlapCoefficient,

        UseRatcliffObershelpSimilarity,

        UseSorensenDiceDistance,

        UseTanimotoCoefficient,

        CaseSensitive
    }

    public enum FuzzyStringComparisonTolerance
    {
        Strong,

        Normal,

        Weak,

        Manual
    }

    public static partial class Operations
    {
        public static string Capitalize(this string source)
        {
            return source.ToUpper();
        }

        public static string[] SplitIntoIndividualElements(string source)
        {
            string[] stringCollection = new string[source.Length];

            for (int i = 0; i < stringCollection.Length; i++)
            {
                stringCollection[i] = source[i].ToString();
            }

            return stringCollection;
        }

        public static string MergeIndividualElementsIntoString(IEnumerable<string> source)
        {
            string returnString = "";

            for (int i = 0; i < source.Count(); i++)
            {
                returnString += source.ElementAt<string>(i);
            }
            return returnString;
        }

        public static List<string> ListPrefixes(this string source)
        {
            List<string> prefixes = new List<string>();

            for (int i = 0; i < source.Length; i++)
            {
                prefixes.Add(source.Substring(0, i));
            }

            return prefixes;
        }

        public static List<string> ListBiGrams(this string source)
        {
            return ListNGrams(source, 2);
        }

        public static List<string> ListTriGrams(this string source)
        {
            return ListNGrams(source, 3);
        }

        public static double PerformDivision(object x, object y)
        {
            double result = 0;

            try
            {
                result = Convert.ToDouble(x) / Convert.ToDouble(y);

                if (double.IsNaN(result) || double.IsInfinity(result))
                {
                    return 0;
                }
            }
            catch (DivideByZeroException)
            {
                return result;
            }

            return result;
        }

        public static List<string> ListNGrams(this string source, int n)
        {
            List<string> nGrams = new List<string>();

            if (n > source.Length)
            {
                return null;
            }
            else if (n == source.Length)
            {
                nGrams.Add(source);
                return nGrams;
            }
            else
            {
                for (int i = 0; i < source.Length - n; i++)
                {
                    nGrams.Add(source.Substring(i, n));
                }

                return nGrams;
            }
        }
    }
}
