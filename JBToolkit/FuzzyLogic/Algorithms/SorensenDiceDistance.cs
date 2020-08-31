using System;
using System.Linq;

namespace JBToolkit.FuzzyLogic
{
    public static partial class Algorithms
    {
        /// <summary>
        /// https://en.wikipedia.org/wiki/S%C3%B8rensen%E2%80%93Dice_coefficient
        /// <br /><br />
        /// The Sørensen–Dice coefficient is a statistic used to gauge the similarity of two samples. 
        /// <br /><br />
        /// A good value would be 0.33 or above, a value under 0.2 is not a good match, from 0.2 to 0.33 is iffy.
        /// <br /><br />
        /// Origin: https://github.com/kdjones/fuzzystring
        /// </summary>
        public static double SorensenDiceDistance(this string source, string target)
        {
            return 1 - source.SorensenDiceIndex(target);
        }

        /// <summary>
        /// https://en.wikipedia.org/wiki/S%C3%B8rensen%E2%80%93Dice_coefficient
        /// <br /><br />
        /// The index is known by several other names, especially Sørensen–Dice index, Sørensen index and Dice's coefficient. 
        /// Other variations include the "similarity coefficient" or "index", such as Dice similarity coefficient (DSC). Common alternate spellings for Sørensen are Sorenson, Soerenson and Sörenson, and all three can also be seen with the –sen ending.
        /// </summary>
        public static double SorensenDiceIndex(this string source, string target)
        {
            return (2 * Convert.ToDouble(source.Intersect(target).Count())) / (Convert.ToDouble(source.Length + target.Length));
        }
    }
}
