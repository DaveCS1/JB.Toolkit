using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JBToolkit.FuzzyLogic
{
    public static partial class Algorithms
    {
        public class BestFuzzyMatch
        {
            public int Index { get; set; }
            public string MatchText { get; set; }
            public string Source { get; set; }
            public double ComparisonResultAverage { get; set; } = 999;
            public string Error { get; set; } = string.Empty;
        }

        public class QuickFuzzyMatch : BestFuzzyMatch
        {
            public bool Match { get; set; }
        }

        public class BestDetailedFuzzyMatch : BestFuzzyMatch
        {
            public bool WeakMatch { get; set; }
            public bool NormalMatch { get; set; }
            public bool StrongMatch { get; set; }
            public List<FuzzyMatch> OrderedMatches { get; set; }
            public bool MultipleBestMatchesFound { get; set; }
        }

        public class FuzzyMatch
        {
            public int ListIndex { get; set; }
            public string MatchText { get; set; }
            public double ComparisonResultAverage { get; set; } = 999;
            public bool WeakMatch { get; set; }
            public bool NormalMatch { get; set; }
            public bool StongMatch { get; set; }
        }

        /// <summary>
        /// Returns the best match in the the form of a BestFuzzyMatch object based on a given source and a list of strings - Identified by the highest comparison result average
        /// based on the provied algorithms to compare with
        /// </summary>
        /// <param name="source">Single string to compare</param>
        /// <param name="compareList">List of strings to compare agains</param>
        /// <param name="options">1 or more fuzzy logic algorithms to compare with</param>
        /// <returns>BestFuzzyMatch object providing a breakdown of what was found along with an ordered list of best, best to worst</returns>
        public static BestFuzzyMatch FindBestBestFuzzyMatchDetailed(this string source, List<string> compareList, bool throwOnError, bool useThreading, params FuzzyStringComparisonOptions[] options)
        {
            var bestFuzzyMatch = new BestDetailedFuzzyMatch();
            var fuzzyMatchList = new List<FuzzyMatch>();

            if (useThreading)
            {
                for (int y = 0; y < 11; y++)
                {
                    try
                    {
                        Parallel.ForEach(compareList, (text, state, index) =>
                        {
                            var fuzzyMatch = new FuzzyMatch
                            {
                                ListIndex = index.To<int>(),
                                MatchText = text
                            };

                            Parallel.For(0, 4,
                                      i =>
                                      {
                                          if (i == 0)
                                          {
                                              lock (fuzzyMatch)
                                              {
                                                  fuzzyMatch.ComparisonResultAverage = source.MatchComparisonResultAverage(text, options);
                                              }
                                          }
                                          else if (i == 1)
                                          {
                                              lock (fuzzyMatch)
                                              {
                                                  fuzzyMatch.WeakMatch = source.ApproximatelyEquals(text, FuzzyStringComparisonTolerance.Weak, null, options);
                                              }
                                          }
                                          else if (i == 2)
                                          {
                                              lock (fuzzyMatch)
                                              {
                                                  fuzzyMatch.NormalMatch = source.ApproximatelyEquals(text, FuzzyStringComparisonTolerance.Normal, null, options);
                                              }
                                          }
                                          else if (i == 3)
                                          {
                                              lock (fuzzyMatch)
                                              {
                                                  fuzzyMatch.StongMatch = source.ApproximatelyEquals(text, FuzzyStringComparisonTolerance.Strong, null, options);
                                              }
                                          }
                                      });

                            lock (fuzzyMatchList)
                            {
                                fuzzyMatchList.Add(fuzzyMatch);
                            }
                        });

                        break;
                    }
                    catch (Exception e)
                    {
                        Thread.Sleep(50);
                        fuzzyMatchList.Clear();

                        if (y > 9)
                        {
                            bestFuzzyMatch.StrongMatch = false;
                            bestFuzzyMatch.WeakMatch = false;
                            bestFuzzyMatch.NormalMatch = false;
                            bestFuzzyMatch.Error = e.Message;

                            if (throwOnError)
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            else
            {
                try
                {
                    int index = 0;
                    foreach (string text in compareList)
                    {
                        var fuzzyMatch = new FuzzyMatch
                        {
                            ListIndex = index,
                            MatchText = text
                        };

                        fuzzyMatch.ComparisonResultAverage = source.MatchComparisonResultAverage(text, options);
                        fuzzyMatch.WeakMatch = source.ApproximatelyEquals(text, FuzzyStringComparisonTolerance.Weak, null, options);
                        fuzzyMatch.NormalMatch = source.ApproximatelyEquals(text, FuzzyStringComparisonTolerance.Normal, null, options);
                        fuzzyMatch.StongMatch = source.ApproximatelyEquals(text, FuzzyStringComparisonTolerance.Strong, null, options);

                        fuzzyMatchList.Add(fuzzyMatch);

                        index++;
                    }
                }
                catch (Exception e)
                {
                    bestFuzzyMatch.StrongMatch = false;
                    bestFuzzyMatch.WeakMatch = false;
                    bestFuzzyMatch.NormalMatch = false;
                    bestFuzzyMatch.Error = e.Message;

                    if (throwOnError)
                    {
                        throw;
                    }

                    return bestFuzzyMatch;
                }
            }

            if (fuzzyMatchList.Count > 0)
            {
                fuzzyMatchList = fuzzyMatchList.OrderBy(x => x.ComparisonResultAverage).ToList();

                bestFuzzyMatch.Index = fuzzyMatchList[0].ListIndex;
                bestFuzzyMatch.MatchText = fuzzyMatchList[0].MatchText;
                bestFuzzyMatch.Source = source;
                bestFuzzyMatch.ComparisonResultAverage = fuzzyMatchList[0].ComparisonResultAverage;
                bestFuzzyMatch.WeakMatch = fuzzyMatchList[0].WeakMatch;
                bestFuzzyMatch.NormalMatch = fuzzyMatchList[0].NormalMatch;
                bestFuzzyMatch.StrongMatch = fuzzyMatchList[0].StongMatch;
                bestFuzzyMatch.OrderedMatches = fuzzyMatchList;
                bestFuzzyMatch.MultipleBestMatchesFound = fuzzyMatchList.Where(x => x.ComparisonResultAverage == fuzzyMatchList[0].ComparisonResultAverage).Count() > 1;
            }

            return bestFuzzyMatch;
        }

        /// <summary>
        /// Returns the best match in the the form of a BestFuzzyMatch object based on a given source and a list of strings - Identified by the highest comparison result average
        /// based on the provied algorithms to compare with
        /// </summary>
        /// <param name="source">Single string to compare</param>
        /// <param name="compareList">List of strings to compare agains</param>
        /// <param name="options">1 or more fuzzy logic algorithms to compare with</param>
        /// <returns>BestFuzzyMatch object providing a breakdown of what was found along with an ordered list of best, best to worst</returns>
        public static BestFuzzyMatch FindBestBestFuzzyMatch(this string source, List<string> compareList, FuzzyStringComparisonTolerance tolerance, bool throwOnError, bool useThreading, params FuzzyStringComparisonOptions[] options)
        {
            var fuzzyMatchList = new List<QuickFuzzyMatch>();
            var bestFuzzyMatch = new QuickFuzzyMatch();

            if (useThreading)
            {
                for (int y = 0; y < 11; y++)
                {
                    try
                    {
                        Parallel.ForEach(compareList, (text, state, index) =>
                        {
                            var fuzzyMatch = new QuickFuzzyMatch
                            {
                                Index = index.To<int>(),
                                MatchText = text,
                                Source = source
                            };

                            Parallel.For(0, 2,
                                      i =>
                                      {
                                          if (i == 0)
                                          {
                                              lock (fuzzyMatch)
                                              {
                                                  fuzzyMatch.ComparisonResultAverage = source.MatchComparisonResultAverage(text, options);
                                              }
                                          }
                                          else if (i == 1)
                                          {
                                              lock (fuzzyMatch)
                                              {
                                                  fuzzyMatch.Match = source.ApproximatelyEquals(text, tolerance, null, options);
                                              }
                                          }
                                      });

                            lock (fuzzyMatchList)
                            {
                                fuzzyMatchList.Add(fuzzyMatch);
                            }
                        });

                        break;
                    }
                    catch (Exception e)
                    {
                        Thread.Sleep(50);
                        fuzzyMatchList.Clear();

                        bestFuzzyMatch.Match = false;
                        bestFuzzyMatch.Error = e.Message;

                        if (y > 9)
                        {
                            if (throwOnError)
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            else
            {
                try
                {
                    int index = 0;
                    foreach (string text in compareList)
                    {
                        var fuzzyMatch = new QuickFuzzyMatch
                        {
                            Index = index,
                            MatchText = text,
                            Source = source
                        };

                        fuzzyMatch.ComparisonResultAverage = source.MatchComparisonResultAverage(text, options);
                        fuzzyMatch.Match = source.ApproximatelyEquals(text, tolerance, null, options);

                        fuzzyMatchList.Add(fuzzyMatch);

                        index++;
                    }
                }
                catch (Exception e)
                {
                    bestFuzzyMatch.Match = false;
                    bestFuzzyMatch.Error = e.Message;

                    if (throwOnError)
                    {
                        throw;
                    }

                    return bestFuzzyMatch;
                }
            }

            if (fuzzyMatchList.Count > 0)
            {
                fuzzyMatchList.RemoveAll(m => m == null);
                bestFuzzyMatch = fuzzyMatchList.OrderBy(m => m.ComparisonResultAverage).First();
            }

            return bestFuzzyMatch;
        }
    }
}
