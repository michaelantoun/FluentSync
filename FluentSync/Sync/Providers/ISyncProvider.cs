using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FluentSync.Sync.Providers
{
    /// <summary>
    /// The sync provider which used by the sync agent to add, update, and delete items.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public interface ISyncProvider<TItem> : ISyncBaseProvider<TItem>
    {
        /// <summary>
        /// Deletes the items.
        /// </summary>
        /// <param name="items">The items to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        Task DeleteAsync(List<TItem> items, CancellationToken cancellationToken);
    }
}
