using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FluentSync.Sync.Providers
{
    /// <summary>
    /// The batch sync provider of the dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public class DictionaryBatchSyncProvider<TKey, TItem> : IComparerBatchSyncProvider<TKey, TItem>
    {
        /// <summary>
        /// The batch sync provider items.
        /// </summary>
        public IDictionary<TKey, TItem> Items { get; set; }

        /// <summary>
        /// The key selector is a function that is executed against the item to return the key.
        /// </summary>
        public Func<TItem, TKey> KeySelector { get; set; }

        /// <summary>
        /// Validates that the required properties are not null-able.
        /// </summary>
        private void Validate()
        {
            if (Items == null)
                throw new NullReferenceException($"The {nameof(Items)} cannot be null.");
            if (KeySelector == null)
                throw new NullReferenceException($"The {nameof(KeySelector)} cannot be null.");
        }

        /// <summary>
        /// Adds the items to the dictionary.
        /// </summary>
        /// <param name="items">The list of items to be added.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        public Task AddAsync(List<TItem> items, CancellationToken cancellationToken)
        {
            Validate();
            return Task.Run(() => items?.ForEach(x => Items.Add(KeySelector(x), x)), cancellationToken);
        }

        /// <summary>
        /// Deletes the items by keys from the dictionary.
        /// </summary>
        /// <param name="keys">The keys of the items.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        public Task DeleteAsync(List<TKey> keys, CancellationToken cancellationToken)
        {
            Validate();
            return Task.Run(() => keys?.ForEach(x => Items.Remove(x)), cancellationToken);
        }

        /// <summary>
        /// Gets the items by keys from the dictionary.
        /// </summary>
        /// <param name="keys">The keys of the items.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        public Task<IEnumerable<TItem>> GetAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken)
        {
            Validate();
            return Task.Run(() => keys?.Where(x => Items.ContainsKey(x)).Select(x => Items[x]), cancellationToken);
        }

        /// <summary>
        /// Gets all the keys from the dictionary.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        public Task<IEnumerable<TKey>> GetAsync(CancellationToken cancellationToken)
        {
            Validate();
            return Task.Run(() => Items.Keys.AsEnumerable(), cancellationToken);
        }

        /// <summary>
        /// Updates the items in the dictionary by using the KeySelector to find them.
        /// </summary>
        /// <param name="items">The items to be updated.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        public Task UpdateAsync(List<MatchValuePair<TItem>> items, CancellationToken cancellationToken)
        {
            Validate();
            return Task.Run(() => items?.ForEach(x =>
            {
                var key = KeySelector(x.CurrentValue);
                Items.Remove(key);
                Items.Add(key, x.NewValue);
            }), cancellationToken);
        }

        /// <summary>
        /// Returns a string that represents the batch sync provider of the dictionary.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Items?.ToString() ?? base.ToString();
        }
    }
}
