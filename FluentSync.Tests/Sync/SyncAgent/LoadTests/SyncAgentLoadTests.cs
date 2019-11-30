using FluentSync.Comparers;
using FluentSync.Sync;
using System;
using System.Collections.Generic;

namespace FluentSync.Tests.Sync.SyncAgent.LoadTests
{
    // Putting every load test in a separate class so they run asynchronously.
    public class SyncAgentLoadTests
    {
        protected const int MaxItemsCount = 100000; // 100k items

        protected static void CreateRandomStringLists(out SortedSet<string> sourceItems, out SortedSet<string> destinationItems)
        {
            sourceItems = new SortedSet<string>();
            destinationItems = new SortedSet<string>();
            Random randomText = new Random(), randomList = new Random();
            int r;

            for (int i = 0; i < MaxItemsCount; i++)
            {
                // Generate item text
                string itemText = null;

                r = randomText.Next(1, 100);
                if (r != 100)
                    itemText = (r % 2 == 0 ? "item" : "Item") + " # " + i.ToString();

                // Add item to list(s)
                r = randomList.Next(1, 100) % 3;
                if (r == 1)
                    sourceItems.Add(itemText);
                else if (r == 2)
                    destinationItems.Add(itemText);
                else
                {
                    sourceItems.Add(itemText);
                    destinationItems.Add(itemText);
                }
            }
        }

        protected static ISyncAgent<string, string> CreateSyncAgent(SortedSet<string> sourceItems, SortedSet<string> destinationItems)
        {
            return SyncAgent<string>.Create()
                .SetComparerAgent(ComparerAgent<string>.Create()
                    .SetKeySelector(x => x?.ToLower())
                    .SetCompareItemFunc((s, d) => s == d ? MatchComparisonResultType.Same : MatchComparisonResultType.Conflict))
                .SetSourceProvider(sourceItems)
                .SetDestinationProvider(destinationItems);
        }
    }
}
