using FluentSync.Comparers.Configurations;
using FluentSync.Comparers.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FluentSync.Comparers
{
    /// <summary>
    /// The comparer agent compares the source and the destination items, and provides a comparison result.
    /// </summary>
    /// <typeparam name="TKey">The type of the item key.</typeparam>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public class ComparerAgent<TKey, TItem> : IComparerAgent<TKey, TItem>
    {
        /// <summary>
        /// The key selector is a function that is executed against the item to return the key.
        /// </summary>
        public Func<TItem, TKey> KeySelector { get; set; }

        /// <summary>
        /// The compare item function compares two items and determines if they are the same, one of them is newer, or there is a conflict.
        /// </summary>
        public CompareItemFunc<TItem> CompareItemFunc { get; set; }

        /// <summary>
        /// An action is called during the validation step to support custom validation.
        /// </summary>
        public ValidateComparerItemsAction<TItem> ValidateItemsAction { get; set; }

        /// <summary>
        /// The configurations of the comparer agent.
        /// </summary>
        public ComparerConfigurations Configurations { get; } = new ComparerConfigurations();

        /// <summary>
        /// The provider which retrieves the items from the source.
        /// </summary>
        public IComparerProvider<TItem> SourceProvider { get; set; }

        /// <summary>
        /// The provider which retrieves the items from the destination.
        /// </summary>
        public IComparerProvider<TItem> DestinationProvider { get; set; }

        /// <summary>
        /// Creates a new instance of the comparer agent.
        /// </summary>
        /// <returns></returns>
        public static ComparerAgent<TKey, TItem> Create() => new ComparerAgent<TKey, TItem>();

        /// <summary>
        /// Validates the comparer agent's properties before comparing items.
        /// </summary>
        private void Validate()
        {
            if (KeySelector == null)
                throw new NullReferenceException($"The {nameof(KeySelector)} cannot be null.");

            if (CompareItemFunc == null)
                throw new NullReferenceException($"The {nameof(CompareItemFunc)} cannot be null.");

            if (SourceProvider == null)
                throw new NullReferenceException($"The {nameof(SourceProvider)} cannot be null.");

            if (DestinationProvider == null)
                throw new NullReferenceException($"The {nameof(DestinationProvider)} cannot be null.");
        }

        /// <summary>
        /// Adds items that have null-able keys to the list and the others to the dictionary.
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="nullableKeysItems"></param>
        /// <param name="items"></param>
        private void AddItemsToDictionary(Dictionary<TKey, List<TItem>> dic, List<TItem> nullableKeysItems, IEnumerable<TItem> items)
        {
            if (items == null)
                return;

            foreach (var item in items)
            {
                var key = KeySelector(item);
                if (key == null)
                {
                    nullableKeysItems.Add(item);
                    continue;
                }

                if (dic.TryGetValue(key, out var list))
                    list.Add(item);
                else
                    dic.Add(key, new List<TItem> { item });
            }
        }

        /// <summary>
        /// Compares the source and destination items, and returns the comparison result.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns>The comparison result.</returns>
        public async Task<ComparisonResult<TItem>> CompareAsync(CancellationToken cancellationToken)
        {
            Validate();

            var comparisonResult = new ComparisonResult<TItem>();
            Dictionary<TKey, List<TItem>> dicSrc = new Dictionary<TKey, List<TItem>>()
                , dicDest = new Dictionary<TKey, List<TItem>>();

            List<TItem> nullableKeysSrcItems = new List<TItem>()
                , nullableKeysDestItems = new List<TItem>();

            // Get entries from source and destination at the same time
            var tskSrc = Task.Run(async () => AddItemsToDictionary(dicSrc, nullableKeysSrcItems, await SourceProvider.GetAsync(cancellationToken).ConfigureAwait(false)), cancellationToken);
            var tskDest = Task.Run(async () => AddItemsToDictionary(dicDest, nullableKeysDestItems, await DestinationProvider.GetAsync(cancellationToken).ConfigureAwait(false)), cancellationToken);

            await Task.WhenAll(tskSrc, tskDest).ConfigureAwait(false);

            Validate(dicSrc, dicDest, nullableKeysSrcItems, nullableKeysDestItems);

            // Compare items that don't have null-able keys
            foreach (var srcKey in dicSrc.Keys)
            {
                var srcItems = dicSrc[srcKey];

                if (dicDest.TryGetValue(srcKey, out var destItems))
                    MatchItemsWithTheSameKey(comparisonResult, srcItems, destItems);
                else
                    comparisonResult.ItemsInSourceOnly.AddRange(srcItems);
            }

            var remainingDestItems = dicDest.Values.SelectMany(x => x).ToArray();
            if (remainingDestItems.Any())
                comparisonResult.ItemsInDestinationOnly.AddRange(remainingDestItems);

            // Compare items that have null-able keys
            MatchItemsWithTheSameKey(comparisonResult, nullableKeysSrcItems, nullableKeysDestItems);

            return comparisonResult;
        }

        /// <summary>
        /// Match items that have the same key.
        /// </summary>
        /// <param name="comparisonResult">The comparison result of the source and destination items.</param>
        /// <param name="srcItems">The source items.</param>
        /// <param name="destItems">The destination items.</param>
        private void MatchItemsWithTheSameKey(ComparisonResult<TItem> comparisonResult, List<TItem> srcItems, List<TItem> destItems)
        {
            foreach (var srcItem in srcItems)
            {
                if (destItems.Any())
                {
                    // Find the best match in the destination list. First find the Same item, then newer source/destination, and finally the conflict
                    var matchResult = destItems.Select(destItem => new MatchComparisonResult<TItem> { Source = srcItem, Destination = destItem, ComparisonResult = CompareItemFunc(srcItem, destItem) })
                        .OrderBy(x => (int)x.ComparisonResult)
                        .First();

                    destItems.Remove(matchResult.Destination);
                    comparisonResult.Matches.Add(matchResult);
                }
                else
                {
                    comparisonResult.ItemsInSourceOnly.Add(srcItem);
                }
            }

            if (destItems.Any())
            {
                comparisonResult.ItemsInDestinationOnly.AddRange(destItems);
                destItems.Clear();
            }
        }

        /// <summary>
        /// Validates the source and destination items.
        /// </summary>
        /// <param name="dicSrc">The source items that have non null-able keys.</param>
        /// <param name="dicDest">The destination items that have non null-able keys.</param>
        /// <param name="nullableKeysSrcItems">The source items that have null-able keys.</param>
        /// <param name="nullableKeysDestItems">The destination items that have null-able keys.</param>
        private void Validate(Dictionary<TKey, List<TItem>> dicSrc, Dictionary<TKey, List<TItem>> dicDest, List<TItem> nullableKeysSrcItems, List<TItem> nullableKeysDestItems)
        {
            ValidateNullableItems(dicSrc, dicDest, nullableKeysSrcItems, nullableKeysDestItems);
            ValidateDuplicateItems(dicSrc, dicDest, nullableKeysSrcItems, nullableKeysDestItems);
            ValidateDuplicateKeys(dicSrc, dicDest, nullableKeysSrcItems, nullableKeysDestItems);

            // Custom validation
            ValidateItemsAction?.Invoke(dicSrc.Values.SelectMany(x => x).Union(nullableKeysSrcItems)
                , dicDest.Values.SelectMany(x => x).Union(nullableKeysDestItems));
        }

        /// <summary>
        /// Validate the item keys and throw an ArgumentException if there is a duplicate.
        /// </summary>
        /// <param name="dicSrc">The source items that have non null-able keys.</param>
        /// <param name="dicDest">The destination items that have non null-able keys.</param>
        /// <param name="nullableKeysSrcItems">The source items that have null-able keys.</param>
        /// <param name="nullableKeysDestItems">The destination items that have null-able keys.</param>
        private void ValidateDuplicateKeys(Dictionary<TKey, List<TItem>> dicSrc, Dictionary<TKey, List<TItem>> dicDest, List<TItem> nullableKeysSrcItems, List<TItem> nullableKeysDestItems)
        {
            int count;

            // Check for duplicated items using the item comparer
            if ((Configurations.AllowDuplicateKeys & RuleAllowanceType.Source) != RuleAllowanceType.Source)
            {
                count = dicSrc.Where(x => x.Value.Count > 1).Select(x => x.Value.Count - 1).Count();
                if (nullableKeysSrcItems.Count > 1)
                    count += nullableKeysSrcItems.Count - 1;
                if (count > 0)
                    throw new ArgumentException(string.Format("Duplicated items are not allowed in the source list, {0} item{1} {2} found."
                            , count, count == 1 ? "" : "s", count == 1 ? "was" : "were"));
            }

            if ((Configurations.AllowDuplicateKeys & RuleAllowanceType.Destination) != RuleAllowanceType.Destination)
            {
                count = dicDest.Where(x => x.Value.Count > 1).Select(x => x.Value.Count - 1).Count();
                if (nullableKeysDestItems.Count > 1)
                    count += nullableKeysDestItems.Count - 1;
                if (count > 0)
                    throw new ArgumentException(string.Format("Duplicated items are not allowed in the destination list, {0} item{1} {2} found."
                            , count, count == 1 ? "" : "s", count == 1 ? "was" : "were"));
            }
        }

        /// <summary>
        /// Validate the items and throw an ArgumentException if there is a duplicate.
        /// </summary>
        /// <param name="dicSrc">The source items that have non null-able keys.</param>
        /// <param name="dicDest">The destination items that have non null-able keys.</param>
        /// <param name="nullableKeysSrcItems">The source items that have null-able keys.</param>
        /// <param name="nullableKeysDestItems">The destination items that have null-able keys.</param>
        private void ValidateDuplicateItems(Dictionary<TKey, List<TItem>> dicSrc, Dictionary<TKey, List<TItem>> dicDest, List<TItem> nullableKeysSrcItems, List<TItem> nullableKeysDestItems)
        {
            int count;

            // Check for duplicated items using the item equality comparer
            if ((Configurations.AllowDuplicateItems & RuleAllowanceType.Source) != RuleAllowanceType.Source)
            {
                count = dicSrc.Sum(x => x.Value.GetDuplicatesCountForUnsortedList(CompareItemFunc))
                    + nullableKeysSrcItems.GetDuplicatesCountForUnsortedList(CompareItemFunc);
                if (count > 0)
                    throw new ArgumentException(string.Format("Duplicated items are not allowed in the source list, {0} item{1} {2} found."
                            , count, count == 1 ? "" : "s", count == 1 ? "was" : "were"));
            }

            if ((Configurations.AllowDuplicateItems & RuleAllowanceType.Destination) != RuleAllowanceType.Destination)
            {
                count = dicDest.Sum(x => x.Value.GetDuplicatesCountForUnsortedList(CompareItemFunc))
                    + nullableKeysDestItems.GetDuplicatesCountForUnsortedList(CompareItemFunc);
                if (count > 0)
                    throw new ArgumentException(string.Format("Duplicated items are not allowed in the destination list, {0} item{1} {2} found."
                            , count, count == 1 ? "" : "s", count == 1 ? "was" : "were"));
            }
        }

        /// <summary>
        /// Validate the items and throw an ArgumentException if there is a null-able item.
        /// </summary>
        /// <param name="dicSrc">The source items that have non null-able keys.</param>
        /// <param name="dicDest">The destination items that have non null-able keys.</param>
        /// <param name="nullableKeysSrcItems">The source items that have null-able keys.</param>
        /// <param name="nullableKeysDestItems">The destination items that have null-able keys.</param>
        private void ValidateNullableItems(Dictionary<TKey, List<TItem>> dicSrc, Dictionary<TKey, List<TItem>> dicDest, List<TItem> nullableKeysSrcItems, List<TItem> nullableKeysDestItems)
        {
            int count;

            if ((Configurations.AllowNullableItems & RuleAllowanceType.Source) != RuleAllowanceType.Source)
            {
                count = dicSrc.Sum(x => x.Value.Count(i => i == null)) + nullableKeysSrcItems.Count(i => i == null);
                if (count > 0)
                    throw new ArgumentException(string.Format("Null-able items are not allowed in the source list, {0} item{1} {2} found."
                            , count, count == 1 ? "" : "s", count == 1 ? "was" : "were"));
            }

            if ((Configurations.AllowNullableItems & RuleAllowanceType.Destination) != RuleAllowanceType.Destination)
            {
                count = dicDest.Sum(x => x.Value.Count(i => i == null)) + nullableKeysDestItems.Count(i => i == null);
                if (count > 0)
                    throw new ArgumentException(string.Format("Null-able items are not allowed in the destination list, {0} item{1} {2} found."
                            , count, count == 1 ? "" : "s", count == 1 ? "was" : "were"));
            }
        }

        /// <summary>
        /// Returns a string that represents the comparer agent.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(Configurations)}: {{{Configurations}}}";
        }
    }

    /// <summary>
    /// The comparer agent compares the source items and the destination items, and provides a comparison result.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public class ComparerAgent<TItem> : ComparerAgent<TItem, TItem>
    {
        /// <summary>
        /// Creates an instance of the comparer agent.
        /// </summary>
        public ComparerAgent()
        {
            KeySelector = (item) => item;
            CompareItemFunc = (src, dest) =>
            {
                if (src == null && dest == null)
                    return MatchComparisonResultType.Same;

                bool same = src != null ? src.Equals(dest) : dest.Equals(src);
                return same ? MatchComparisonResultType.Same : MatchComparisonResultType.Conflict;
            };
        }

        /// <summary>
        /// Creates an instance of the comparer agent.
        /// </summary>
        public new static ComparerAgent<TItem> Create() => new ComparerAgent<TItem>();
    }
}
