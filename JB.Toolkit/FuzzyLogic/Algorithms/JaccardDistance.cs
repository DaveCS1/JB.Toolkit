using System;
using System.Linq;

namespace JBToolkit.FuzzyLogic
{
    public static partial class Algorithms
    {
        /// <summary>
        /// https://en.wikipedia.org/wiki/Jaccard_index
        /// <br /><br />
        /// The Jaccard distance, which measures dissimilarity between sample sets, is complementary to the Jaccard coefficient and is obtained by 
        /// subtracting the Jaccard coefficient from 1, or, equivalently, by dividing the difference of the sizes of the union and the intersection of two sets by the size of the union:
        /// Origin: https://github.com/kdjones/fuzzystring
        /// </summary>
        public static double JaccardDistance(this string source, string target)
        {
            return 1 - source.JaccardIndex(target);
        }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Jaccard_index
        /// <br /><br />
        /// The Jaccard index, also known as Intersection over Union and the Jaccard similarity coefficient (originally given the French name coefficient 
        /// de communauté by Paul Jaccard), is a statistic used for gauging the similarity and diversity of sample sets. The Jaccard coefficient measures 
        /// similarity between finite sample sets, and is defined as the size of the intersection divided by the size of the union of the sample sets:
        /// </summary>
        public static double JaccardIndex(this string source, string target)
        {
            return (Convert.ToDouble(source.Intersect(target).Count())) / (Convert.ToDouble(source.Union(target).Count()));
        }
    }
}
