using System.Linq;

namespace JBToolkit.FuzzyLogic
{
    public static partial class Algorithms
    {
        /// <summary>
        /// https://en.wikipedia.org/wiki/Jaccard_index#Tanimoto_coefficient_(extended_Jaccard_coefficient)
        /// <br /><br />
        /// Similar to: Jaccard index
        /// <br /><br />
        /// Origin: https://github.com/kdjones/fuzzystring
        /// </summary>
        public static double TanimotoCoefficient(this string source, string target)
        {
            double Na = source.Length;
            double Nb = target.Length;
            double Nc = source.Intersect(target).Count();

            return Nc / (Na + Nb - Nc);
        }
    }
}
