using FluentSync.Comparers.Providers;

namespace FluentSync.Sync.Providers
{
    /// <summary>
    /// The key comparer and batch sync providers.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    public interface IComparerBatchSyncProvider<TKey, TItem> : IBatchSyncProvider<TKey, TItem>, IComparerProvider<TKey>
    {
    }
}
