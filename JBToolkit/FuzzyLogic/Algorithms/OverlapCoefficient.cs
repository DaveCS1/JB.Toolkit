using System;
using System.Linq;

namespace JBToolkit.FuzzyLogic
{
    public static partial class Algorithms
    {
        /// <summary>
        /// https://en.wikipedia.org/wiki/Overlap_coefficient
        /// <br /><br />
        /// he overlap coefficient,[1] or Szymkiewicz–Simpson coefficient, is a similarity measure that measures the overlap between two finite sets. 
        /// It is related to the Jaccard index and is defined as the size of the intersection divided by the smaller of the size of the two sets:
        /// <br /><br />
        /// Origin: https://github.com/kdjones/fuzzystring
        /// </summary>
        public static double OverlapCoefficient(this string source, string target)
        {
            return (Convert.ToDouble(source.Intersect(target).Count())) / Convert.ToDouble(Math.Min(source.Length, target.Length));
        }
    }
}
