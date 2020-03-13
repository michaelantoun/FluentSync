using FluentSync.Comparers;
using System.Collections.Generic;

namespace FluentSync
{
    /// <summary>
    /// The extension methods of the list.
    /// </summary>
    internal static class ListExtension
    {
        /// <summary>
        /// Finds the duplicates in the list and returns count.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="list">The list of items.</param>
        /// <param name="compareItemFunc">The function that is used to compare the items.</param>
        /// <returns>The duplicates count.</returns>
        public static int GetDuplicatesCountForUnsortedList<T>(this List<T> list, CompareItemFunc<T> compareItemFunc)
        {
            int duplicatesCount = 0;

            if (list?.Count == 0)
            {
                return 0;
            }
            
            var clonedList = new List<T>(list);

            for (int i = 0; i < clonedList.Count; i++)
            {
                for (int j = i + 1; j < clonedList.Count; j++)
                {
                    if (compareItemFunc(clonedList[i], clonedList[j]) == MatchComparisonResultType.Same)
                    {
                        duplicatesCount++;

                        clonedList.RemoveAt(j);
                        j--;
                    }
                }
            }

            return duplicatesCount;
        }
    }
}
