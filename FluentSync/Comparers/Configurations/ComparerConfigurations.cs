namespace FluentSync.Comparers.Configurations
{
    /// <summary>
    /// The configurations of the comparer agent.
    /// </summary>
    public class ComparerConfigurations
    {
        /// <summary>
        /// Allow duplicate items in source/destination providers.
        /// </summary>
        public RuleAllowanceType AllowDuplicateKeys { get; set; } = RuleAllowanceType.Both;

        /// <summary>
        /// Allow duplicate keys in source/destination providers.
        /// </summary>
        public RuleAllowanceType AllowDuplicateItems { get; set; } = RuleAllowanceType.Both;

        /// <summary>
        /// Allow null-able items in source/destination providers.
        /// </summary>
        public RuleAllowanceType AllowNullableItems { get; set; } = RuleAllowanceType.Both;

        /// <summary>
        /// Returns a string that represents the configurations of the comparer agent.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(AllowDuplicateKeys)}: {AllowDuplicateKeys}, {nameof(AllowDuplicateItems)}: {AllowDuplicateItems}, {nameof(AllowNullableItems)}: {AllowNullableItems}";
        }
    }
}
