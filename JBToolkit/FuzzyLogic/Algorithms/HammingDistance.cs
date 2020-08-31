namespace JBToolkit.FuzzyLogic
{
    public static partial class Algorithms
    {
        /// <summary>
        /// https://en.wikipedia.org/wiki/Hamming_distance
        /// <br /><br />
        /// Tthe Hamming distance between two strings of equal length is the number of positions at which the corresponding 
        /// symbols are different. In other words, it measures the minimum number of substitutions required to change one string into the other
        /// <br /><br />
        /// Origin: https://github.com/kdjones/fuzzystring
        /// </summary>
        public static int HammingDistance(this string source, string target)
        {
            int distance = 0;

            if (source.Length == target.Length)
            {
                for (int i = 0; i < source.Length; i++)
                {
                    if (!source[i].Equals(target[i]))
                    {
                        distance++;
                    }
                }
                return distance;
            }
            else { return 99999; }
        }
    }
}
