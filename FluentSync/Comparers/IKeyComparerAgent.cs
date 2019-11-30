using System.Threading;
using System.Threading.Tasks;

namespace FluentSync.Comparers
{
    /// <summary>
    /// The key comparer agent compares the source and destination keys, and returns the comparison result. It does not allow null-able nor duplicate keys.
    /// </summary>
    /// <typeparam name="TKey">The type of the item key.</typeparam>
    public interface IKeyComparerAgent<TKey> : IBaseComparerAgent<TKey>
    {
        /// <summary>
        /// Compares the source and destination keys, and returns the comparison result.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns>The comparison result.</returns>
        Task<KeysComparisonResult<TKey>> CompareAsync(CancellationToken cancellationToken);
    }
}