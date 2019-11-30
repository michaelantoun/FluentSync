using FluentSync.Comparers;
using System.Collections.Generic;

namespace FluentSync
{
    /// <summary>
    /// The compare item function compares two items and determines if they are the same, one of them is newer, or there is a conflict.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="source">The source item.</param>
    /// <param name="destination">The destination item.</param>
    /// <returns></returns>
    public delegate MatchComparisonResultType CompareItemFunc<TItem>(TItem source, TItem destination);

    /// <summary>
    /// The ValidateComparerItems is a method called during the validation step to support custom validation.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="source">The source items.</param>
    /// <param name="destination">The destination items.</param>
    public delegate void ValidateComparerItemsAction<TItem>(IEnumerable<TItem> source, IEnumerable<TItem> destination);

}
