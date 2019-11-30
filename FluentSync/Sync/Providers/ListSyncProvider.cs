using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FluentSync.Sync.Providers
{
    /// <summary>
    /// The sync provider of the list.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public class ListSyncProvider<TItem> : IComparerSyncProvider<TItem>
    {
        /// <summary>
        /// The sync provider items.
        /// </summary>
        public IList<TItem> Items { get; set; }

        /// <summary>
        /// Adds the items to the list.
        /// </summary>
        /// <param name="items">The items to be added.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        public Task AddAsync(List<TItem> items, CancellationToken cancellationToken)
        {
            return Task.Run(() => items?.ForEach(x => Items.Add(x)), cancellationToken);
        }

        /// <summary>
        /// Deletes the items from the list.
        /// </summary>
        /// <param name="items">The items to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        public Task DeleteAsync(List<TItem> items, CancellationToken cancellationToken)
        {
            return Task.Run(() => items?.ForEach(x => Items.Remove(x)), cancellationToken);
        }

        /// <summary>
        /// Updates the items in the list.
        /// </summary>
        /// <param name="pairs">The pairs of the old items and new items to be updated.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        public Task UpdateAsync(List<MatchValuePair<TItem>> pairs, CancellationToken cancellationToken)
        {
            return Task.Run(() => pairs?.ForEach(x =>
                {
                    int i = Items.IndexOf(x.CurrentValue);
                    Items[i] = x.NewValue;
                }), cancellationToken);
        }

        /// <summary>
        /// Gets all the items of the list.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        public Task<IEnumerable<TItem>> GetAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Items.AsEnumerable());
        }

        /// <summary>
        /// Returns a string that represents the sync provider of the list.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Items?.ToString() ?? base.ToString();
        }

    }
}
