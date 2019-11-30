using FluentSync.Comparers.Providers;
using System.Collections.Generic;

namespace FluentSync.Comparers
{
    /// <summary>
    /// The extension methods of the key comparer agent.
    /// </summary>
    public static class KeyComparerAgentExtensions
    {
        /// <summary>
        /// Sets the source provider.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="comparerAgent">The comparer agent.</param>
        /// <param name="sourceProvider">The source provider of the comparer agent.</param>
        /// <returns>The comparer agent.</returns>
        public static IKeyComparerAgent<TKey> SetSourceProvider<TKey>(this IKeyComparerAgent<TKey> comparerAgent, IComparerProvider<TKey> sourceProvider)
        {
            comparerAgent.SourceProvider = sourceProvider;
            return comparerAgent;
        }

        /// <summary>
        /// Sets the source keys.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="comparerAgent">The comparer agent.</param>
        /// <param name="keys">The source keys.</param>
        /// <returns>The comparer agent.</returns>
        public static IKeyComparerAgent<TKey> SetSourceProvider<TKey>(this IKeyComparerAgent<TKey> comparerAgent, IEnumerable<TKey> keys)
        {
            comparerAgent.SourceProvider = new ComparerProvider<TKey> { Items = keys };
            return comparerAgent;
        }

        /// <summary>
        /// Sets the destination provider.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="comparerAgent">The comparer agent.</param>
        /// <param name="destinationProvider">The destination provider of the comparer agent.</param>
        /// <returns>The comparer agent.</returns>
        public static IKeyComparerAgent<TKey> SetDestinationProvider<TKey>(this IKeyComparerAgent<TKey> comparerAgent, IComparerProvider<TKey> destinationProvider)
        {
            comparerAgent.DestinationProvider = destinationProvider;
            return comparerAgent;
        }

        /// <summary>
        /// Sets the destination keys.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="comparerAgent">The comparer agent.</param>
        /// <param name="keys">The destination keys.</param>
        /// <returns>The comparer agent.</returns>
        public static IKeyComparerAgent<TKey> SetDestinationProvider<TKey>(this IKeyComparerAgent<TKey> comparerAgent, IEnumerable<TKey> keys)
        {
            comparerAgent.DestinationProvider = new ComparerProvider<TKey> { Items = keys };
            return comparerAgent;
        }
    }
}
