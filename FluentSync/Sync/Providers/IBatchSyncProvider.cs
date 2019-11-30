using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FluentSync.Sync.Providers
{
    /// <summary>
    /// The batch sync provider which used by the batch sync agent to add, update, and delete items.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public interface IBatchSyncProvider<TKey, TItem> : ISyncBaseProvider<TItem>
    {
        /// <summary>
        /// Gets the items by their keys.
        /// </summary>
        /// <param name="keys">The keys of the items.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        Task<IEnumerable<TItem>> GetAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes the items by their keys.
        /// </summary>
        /// <param name="keys">The keys of the items.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        Task DeleteAsync(List<TKey> keys, CancellationToken cancellationToken);
    }
}
