using FluentSync.Comparers.Providers;

namespace FluentSync.Comparers
{
    /// <summary>
    /// The common interface of the comparer agent.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    public interface IBaseComparerAgent<T>
    {
        /// <summary>
        /// The provider which retrieves the items from the source.
        /// </summary>
        IComparerProvider<T> SourceProvider { get; set; }

        /// <summary>
        /// The provider which retrieves the items from the destination.
        /// </summary>
        IComparerProvider<T> DestinationProvider { get; set; }

    }
}