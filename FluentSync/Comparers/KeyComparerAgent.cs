using FluentSync.Comparers.Providers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FluentSync.Comparers
{
    /// <summary>
    /// The key comparer agent compares the source and destination keys, and returns the comparison result. It does not allow null-able nor duplicate keys.
    /// </summary>
    /// <typeparam name="TKey">The type of the item key.</typeparam>
    public class KeyComparerAgent<TKey> : IKeyComparerAgent<TKey>
    {
        /// <summary>
        /// The provider which retrieves the item keys from the source.
        /// </summary>
        public IComparerProvider<TKey> SourceProvider { get; set; }

        /// <summary>
        /// The provider which retrieves the item keys from the destination.
        /// </summary>
        public IComparerProvider<TKey> DestinationProvider { get; set; }

        /// <summary>
        /// Creates a new instance of the key comparer agent.
        /// </summary>
        /// <returns></returns>
        public static KeyComparerAgent<TKey> Create() => new KeyComparerAgent<TKey>();

        /// <summary>
        /// Validates the comparer agent's properties before comparing items.
        /// </summary>
        private void Validate()
        {
            if (SourceProvider == null)
                throw new NullReferenceException($"The {nameof(SourceProvider)} cannot be null.");

            if (DestinationProvider == null)
                throw new NullReferenceException($"The {nameof(DestinationProvider)} cannot be null.");
        }

        /// <summary>
        /// Adds the keys to the SortedSet.
        /// </summary>
        /// <typeparam name="T">The type of the key.</typeparam>
        /// <param name="sortedSet">The keys set.</param>
        /// <param name="keys">The keys to be added.</param>
        /// <param name="listName">The name of the list.</param>
        private static void AddKeysToSet<T>(SortedSet<T> sortedSet, IEnumerable<T> keys, string listName)
        {
            if (keys == null)
                return;

            foreach (var key in keys)
            {
                if (key == null)
                    throw new NullReferenceException($"Null-able keys found in the {listName}.");

                if (!sortedSet.Add(key))
                    throw new ArgumentException($"Key '{key}' already exists in the {listName}.");
            }
        }

        /// <summary>
        /// Compares the source and destination keys, and returns the comparison result.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns>The comparison result.</returns>
        public async Task<KeysComparisonResult<TKey>> CompareAsync(CancellationToken cancellationToken)
        {
            Validate();

            var comparisonResult = new KeysComparisonResult<TKey>();
            SortedSet<TKey> sourceSet = new SortedSet<TKey>()
                , destinationSet = new SortedSet<TKey>();

            // Get entries from source and destination at the same time
            var tskSrc = Task.Run(async () => AddKeysToSet(sourceSet, await SourceProvider.GetAsync(cancellationToken).ConfigureAwait(false), "source list"), cancellationToken);
            var tskDest = Task.Run(async () => AddKeysToSet(destinationSet, await DestinationProvider.GetAsync(cancellationToken).ConfigureAwait(false), "destination list"), cancellationToken);

            await Task.WhenAll(tskSrc, tskDest).ConfigureAwait(false);

            // Compare keys
            foreach (var srcKey in sourceSet)
            {
                if (destinationSet.Remove(srcKey))
                    comparisonResult.Matches.Add(srcKey);
                else
                    comparisonResult.KeysInSourceOnly.Add(srcKey);
            }

            foreach (var destKey in destinationSet)
                comparisonResult.KeysInDestinationOnly.Add(destKey);

            return comparisonResult;
        }
    }
}
