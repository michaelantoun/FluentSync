using FluentSync.Comparers.Configurations;
using FluentSync.Comparers.Providers;
using System;
using System.Collections.Generic;

namespace FluentSync.Comparers
{
    /// <summary>
    /// The extension methods of the comparer agent.
    /// </summary>
    public static class ComparerAgentExtensions
    {
        /// <summary>
        /// Configures the comparer agent.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="comparerAgent">The comparer agent.</param>
        /// <param name="configure">The configure action.</param>
        /// <returns>The comparer agent.</returns>
        public static IComparerAgent<TKey, TItem> Configure<TKey, TItem>(this IComparerAgent<TKey, TItem> comparerAgent, Action<ComparerConfigurations> configure)
        {
            configure?.Invoke(comparerAgent.Configurations);
            return comparerAgent;
        }

        /// <summary>
        /// Sets the source provider.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="comparerAgent">The comparer agent.</param>
        /// <param name="sourceProvider">The source provider of the comparer agent.</param>
        /// <returns>The comparer agent.</returns>
        public static IComparerAgent<TKey, TItem> SetSourceProvider<TKey, TItem>(this IComparerAgent<TKey, TItem> comparerAgent, IComparerProvider<TItem> sourceProvider)
        {
            comparerAgent.SourceProvider = sourceProvider;
            return comparerAgent;
        }

        /// <summary>
        /// Sets the source items.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="comparerAgent">The comparer agent.</param>
        /// <param name="items">The source items.</param>
        /// <returns>The comparer agent.</returns>
        public static IComparerAgent<TKey, TItem> SetSourceProvider<TKey, TItem>(this IComparerAgent<TKey, TItem> comparerAgent, IEnumerable<TItem> items)
        {
            comparerAgent.SourceProvider = new ComparerProvider<TItem> { Items = items };
            return comparerAgent;
        }

        /// <summary>
        /// Sets the destination provider.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="comparerAgent">The comparer agent.</param>
        /// <param name="destinationProvider">The destination provider of the comparer agent.</param>
        /// <returns>The comparer agent.</returns>
        public static IComparerAgent<TKey, TItem> SetDestinationProvider<TKey, TItem>(this IComparerAgent<TKey, TItem> comparerAgent, IComparerProvider<TItem> destinationProvider)
        {
            comparerAgent.DestinationProvider = destinationProvider;
            return comparerAgent;
        }

        /// <summary>
        /// Sets the destination items.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="comparerAgent">The comparer agent.</param>
        /// <param name="items">The destination items.</param>
        /// <returns>The comparer agent.</returns>
        public static IComparerAgent<TKey, TItem> SetDestinationProvider<TKey, TItem>(this IComparerAgent<TKey, TItem> comparerAgent, IEnumerable<TItem> items)
        {
            comparerAgent.DestinationProvider = new ComparerProvider<TItem> { Items = items };
            return comparerAgent;
        }

        /// <summary>
        /// Sets the CompareItemFunc.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="comparerAgent">The comparer agent.</param>
        /// <param name="compareItemFunc">The compare item function compares two items and determines if they are the same, one of them is newer, or there is a conflict.</param>
        /// <returns>The comparer agent.</returns>
        public static IComparerAgent<TKey, TItem> SetCompareItemFunc<TKey, TItem>(this IComparerAgent<TKey, TItem> comparerAgent, CompareItemFunc<TItem> compareItemFunc)
        {
            comparerAgent.CompareItemFunc = compareItemFunc;
            return comparerAgent;
        }

        /// <summary>
        /// Sets the ValidateItemsAction.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="comparerAgent">The comparer agent.</param>
        /// <param name="validateItemsAction">The ValidateItemsAction is a method called during the validation step to support custom validation.</param>
        /// <returns>The comparer agent.</returns>
        public static IComparerAgent<TKey, TItem> SetValidateItemsAction<TKey, TItem>(this IComparerAgent<TKey, TItem> comparerAgent, ValidateComparerItemsAction<TItem> validateItemsAction)
        {
            comparerAgent.ValidateItemsAction = validateItemsAction;
            return comparerAgent;
        }

        /// <summary>
        /// Sets the KeySelector.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="comparerAgent">The comparer agent.</param>
        /// <param name="keySelector"> The key selector is a function that is executed against the item to return the key.</param>
        /// <returns>The comparer agent.</returns>
        public static IComparerAgent<TKey, TItem> SetKeySelector<TKey, TItem>(this IComparerAgent<TKey, TItem> comparerAgent, Func<TItem, TKey> keySelector)
        {
            comparerAgent.KeySelector = keySelector;
            return comparerAgent;
        }

    }
}
