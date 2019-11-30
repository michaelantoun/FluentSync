using FluentSync.Comparers.Configurations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FluentSync.Comparers
{
    /// <summary>
    /// The comparer agent compares the source and the destination items, and provides a comparison result.
    /// </summary>
    /// <typeparam name="TKey">The type of the item key.</typeparam>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public interface IComparerAgent<TKey, TItem> : IBaseComparerAgent<TItem>
    {
        /// <summary>
        /// The configurations of the comparer agent.
        /// </summary>
        ComparerConfigurations Configurations { get; }

        /// <summary>
        /// The compare item function compares two items and determines if they are the same, one of them is newer, or there is a conflict.
        /// </summary>
        CompareItemFunc<TItem> CompareItemFunc { get; set; }

        /// <summary>
        /// The key selector is a function that is executed against the item to return the key.
        /// </summary>
        Func<TItem, TKey> KeySelector { get; set; }

        /// <summary>
        /// An action is called during the validation step to support custom validation.
        /// </summary>
        ValidateComparerItemsAction<TItem> ValidateItemsAction { get; set; }

        /// <summary>
        /// Compares the source and destination items and returns the comparison result.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns>The comparison result.</returns>
        Task<ComparisonResult<TItem>> CompareAsync(CancellationToken cancellationToken);
    }
}