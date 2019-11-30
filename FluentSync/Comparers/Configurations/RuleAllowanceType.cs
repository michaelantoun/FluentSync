using System;

namespace FluentSync.Comparers.Configurations
{
    /// <summary>
    /// Allow rule on source and/or destination providers.
    /// </summary>
    [Flags]
    public enum RuleAllowanceType
    {
        /// <summary>
        /// Do not allow the rule.
        /// </summary>
        None = 0,
        /// <summary>
        /// Allow the rule on the source provider only.
        /// </summary>
        Source = 1,
        /// <summary>
        /// Allow the rule on the destination provider only.
        /// </summary>
        Destination = 2,
        /// <summary>
        /// Allow the rule on the source and destination providers.
        /// </summary>
        Both = 3
    }
}
