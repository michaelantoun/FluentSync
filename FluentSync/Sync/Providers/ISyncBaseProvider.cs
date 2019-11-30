using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FluentSync.Sync.Providers
{
    /// <summary>
    /// The base sync provider which used by the sync agent to add and update items.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public interface ISyncBaseProvider<TItem>
    {
        /// <summary>
        /// Adds the items.
        /// </summary>
        /// <param name="items">The items to be added.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        Task AddAsync(List<TItem> items, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the items.
        /// </summary>
        /// <param name="pairs">The pairs of the old items and new items to be updated.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        Task UpdateAsync(List<MatchValuePair<TItem>> pairs, CancellationToken cancellationToken);
    }
}
