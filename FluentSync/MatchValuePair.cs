namespace FluentSync
{
    /// <summary>
    /// A pair of the current and new value of the item.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public class MatchValuePair<TItem>
    {
        /// <summary>
        /// The current value of the item.
        /// </summary>
        public TItem CurrentValue { get; set; }

        /// <summary>
        /// The new value of the item.
        /// </summary>
        public TItem NewValue { get; set; }
    }
}
