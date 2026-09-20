namespace FluentSync.Sync.Configurations
{
    /// <summary>
    /// Presets for the sync mode to facilitate updating source/destination items.
    /// </summary>
    public enum SyncModePreset : int
    {
        /// <summary>
        /// Do nothing.
        /// </summary>
        None = 0,
        /// <summary>
        /// Updates the source and the destination to have the same items.
        /// </summary>
        TwoWay,
        /// <summary>
        /// Updates the destination only to have the same items as in the source and deletes any item in destination that does not have a match.
        /// </summary>
        MirrorToDestination,
        /// <summary>
        /// Updates the source only to have the same items as in the destination and deletes any item in source that does not have a match.
        /// </summary>
        MirrorToSource,
        /// <summary>
        /// Updates the destination items only to have the same items as in the source. It does not delete any item from the destination and it does not update conflicts.
        /// </summary>
        UpdateDestination,
        /// <summary>
        /// Updates the source items only to have the same items as in the destination. It does not delete any item from the source and it does not update conflicts.
        /// </summary>
        UpdateSource,
        /// <summary>
        /// Custom sync mode that is not similar to any other presets. This value is returned by
        /// <see cref="SyncMode.SyncModePreset"/> when the sync mode's operations match no other preset.
        /// Assigning it resets every operation to "do nothing", exactly like <see cref="None"/>, so assign it
        /// before customizing the individual <see cref="SyncMode"/> properties rather than after.
        /// </summary>
        Custom
    }
}