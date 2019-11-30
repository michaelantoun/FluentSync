using FluentSync.Comparers.Providers;

namespace FluentSync.Sync.Providers
{
    /// <summary>
    /// The comparer and sync providers.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public interface IComparerSyncProvider<TItem> : ISyncProvider<TItem>, IComparerProvider<TItem>
    {
    }
}
