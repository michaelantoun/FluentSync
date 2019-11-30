using FluentSync.Comparers;
using FluentSync.Sync;
using FluentSync.Tests.Models;
using System;
using System.Collections.Generic;

namespace FluentSync.Tests.Sync.BatchSyncAgent.LoadTests
{
    // Putting every load test in a separate class so they run asynchronously.
    public class BatchSyncAgentLoadTests
    {
        protected const int MaxItemsCount = 100000; // 100k items

        internal static void CreateRandomHobbies(out IDictionary<int, Hobby> sourceDictionary, out IDictionary<int, Hobby> destinationDictionary)
        {
            sourceDictionary = new Dictionary<int, Hobby>();
            destinationDictionary = new Dictionary<int, Hobby>();

            Random randomText = new Random(), randomList = new Random();
            int r;

            for (int i = 0; i < MaxItemsCount; i++)
            {
                // Generate item text
                string itemText = null;

                r = randomText.Next(1, 100);
                if (r != 100)
                    itemText = (r % 2 == 0 ? "hobby" : "Hobby") + " # " + i.ToString();

                // Add item to list(s)
                r = randomList.Next(1, 100) % 3;
                if (r == 1)
                    sourceDictionary.TryAdd(i, new Hobby { Id = i, Name = itemText });
                else if (r == 2)
                    destinationDictionary.TryAdd(i, new Hobby { Id = i, Name = itemText });
                else
                {
                    sourceDictionary.TryAdd(i, new Hobby { Id = i, Name = itemText });
                    destinationDictionary.TryAdd(i, new Hobby { Id = i, Name = itemText });
                }
            }
        }

        internal static IBatchSyncAgent<int, Hobby> CreateSyncAgent(IDictionary<int, Hobby> sourceDictionary, IDictionary<int, Hobby> destinationDictionary)
        {
            return BatchSyncAgent<int, Hobby>.Create()
                .SetComparerAgent(KeyComparerAgent<int>.Create())
                .SetKeySelector(x => x.Id)
                .SetCompareItemFunc((s, d) =>
                {
                    if (s.Name == d.Name)
                        return MatchComparisonResultType.Same;
                    else
                        return MatchComparisonResultType.Conflict;
                })
                .SetSourceProvider(sourceDictionary)
                .SetDestinationProvider(destinationDictionary);
        }

    }
}
