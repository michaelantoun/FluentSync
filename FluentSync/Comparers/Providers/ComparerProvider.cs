using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FluentSync.Comparers.Providers
{
    /// <summary>
    /// The comparer provider which retrieves the items for the comparer agent.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    public class ComparerProvider<T> : IComparerProvider<T>
    {
        /// <summary>
        /// The provider items.
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Get all the items.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns></returns>
        public Task<IEnumerable<T>> GetAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Items);
        }
    }
}
