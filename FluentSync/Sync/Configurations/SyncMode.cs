using System;

namespace FluentSync.Sync.Configurations
{
    /// <summary>
    /// The sync mode that determines which items will be inserted/updated/deleted in the source/destination.
    /// </summary>
    public class SyncMode
    {
        private SyncMatchOperation conflictMatches = SyncMatchOperation.None;

        /// <summary>
        /// Presets for the sync mode to facilitate updating source/destination items.
        /// </summary>
        public SyncModePreset SyncModePreset
        {
            get
            {
                if (ItemsInSourceOnly == SyncItemOperation.None && ItemsInDestinationOnly == SyncItemOperation.None
                    && SameMatches == SyncMatchOperation.None && NewerMatches == SyncMatchOperation.None
                    && ConflictMatches == SyncMatchOperation.None)
                    return SyncModePreset.None;

                if (ItemsInSourceOnly == SyncItemOperation.Add && ItemsInDestinationOnly == SyncItemOperation.Add
                    && SameMatches == SyncMatchOperation.None && NewerMatches == SyncMatchOperation.UpdateOldItem
                    && ConflictMatches == SyncMatchOperation.None)
                    return SyncModePreset.TwoWay;

                if (ItemsInSourceOnly == SyncItemOperation.Add && ItemsInDestinationOnly == SyncItemOperation.Delete
                    && SameMatches == SyncMatchOperation.None && NewerMatches == SyncMatchOperation.UpdateDestination
                    && ConflictMatches == SyncMatchOperation.UpdateDestination)
                    return SyncModePreset.MirrorToDestination;

                if (ItemsInSourceOnly == SyncItemOperation.Delete && ItemsInDestinationOnly == SyncItemOperation.Add
                    && SameMatches == SyncMatchOperation.None && NewerMatches == SyncMatchOperation.UpdateSource
                    && ConflictMatches == SyncMatchOperation.UpdateSource)
                    return SyncModePreset.MirrorToSource;

                if (ItemsInSourceOnly == SyncItemOperation.Add && ItemsInDestinationOnly == SyncItemOperation.None
                    && SameMatches == SyncMatchOperation.None && NewerMatches == SyncMatchOperation.UpdateOldDestination
                    && ConflictMatches == SyncMatchOperation.None)
                    return SyncModePreset.UpdateDestination;

                if (ItemsInSourceOnly == SyncItemOperation.None && ItemsInDestinationOnly == SyncItemOperation.Add
                    && SameMatches == SyncMatchOperation.None && NewerMatches == SyncMatchOperation.UpdateOldSource
                    && ConflictMatches == SyncMatchOperation.None)
                    return SyncModePreset.UpdateSource;

                return SyncModePreset.Custom;
            }
            set
            {
                switch (value)
                {
                    case SyncModePreset.None:
                    case SyncModePreset.Custom:
                        ItemsInSourceOnly = SyncItemOperation.None;
                        ItemsInDestinationOnly = SyncItemOperation.None;
                        SameMatches = SyncMatchOperation.None;
                        NewerMatches = SyncMatchOperation.None;
                        ConflictMatches = SyncMatchOperation.None;
                        break;
                    case SyncModePreset.TwoWay:
                        ItemsInSourceOnly = SyncItemOperation.Add;
                        ItemsInDestinationOnly = SyncItemOperation.Add;
                        SameMatches = SyncMatchOperation.None;
                        NewerMatches = SyncMatchOperation.UpdateOldItem;
                        ConflictMatches = SyncMatchOperation.None;
                        break;
                    case SyncModePreset.MirrorToDestination:
                        ItemsInSourceOnly = SyncItemOperation.Add;
                        ItemsInDestinationOnly = SyncItemOperation.Delete;
                        SameMatches = SyncMatchOperation.None;
                        NewerMatches = SyncMatchOperation.UpdateDestination;
                        ConflictMatches = SyncMatchOperation.UpdateDestination;
                        break;
                    case SyncModePreset.MirrorToSource:
                        ItemsInSourceOnly = SyncItemOperation.Delete;
                        ItemsInDestinationOnly = SyncItemOperation.Add;
                        SameMatches = SyncMatchOperation.None;
                        NewerMatches = SyncMatchOperation.UpdateSource;
                        ConflictMatches = SyncMatchOperation.UpdateSource;
                        break;
                    case SyncModePreset.UpdateDestination:
                        ItemsInSourceOnly = SyncItemOperation.Add;
                        ItemsInDestinationOnly = SyncItemOperation.None;
                        SameMatches = SyncMatchOperation.None;
                        NewerMatches = SyncMatchOperation.UpdateOldDestination;
                        ConflictMatches = SyncMatchOperation.None;
                        break;
                    case SyncModePreset.UpdateSource:
                        ItemsInSourceOnly = SyncItemOperation.None;
                        ItemsInDestinationOnly = SyncItemOperation.Add;
                        SameMatches = SyncMatchOperation.None;
                        NewerMatches = SyncMatchOperation.UpdateOldSource;
                        ConflictMatches = SyncMatchOperation.None;
                        break;
                }
            }
        }

        /// <summary>
        /// Items that exist only in source.
        /// </summary>
        public SyncItemOperation ItemsInSourceOnly { get; set; } = SyncItemOperation.None;

        /// <summary>
        /// Items that exist only in destination.
        /// </summary>
        public SyncItemOperation ItemsInDestinationOnly { get; set; } = SyncItemOperation.None;

        /// <summary>
        /// Same items that exist in the source and destination.
        /// </summary>
        public SyncMatchOperation SameMatches { get; set; } = SyncMatchOperation.None;

        /// <summary>
        /// Matches that have either newer source items or newer destination items.
        /// </summary>
        public SyncMatchOperation NewerMatches { get; set; } = SyncMatchOperation.None;

        /// <summary>
        /// Items that have matches in the other list but could not determine which one is newer.
        /// </summary>
        public SyncMatchOperation ConflictMatches
        {
            get => conflictMatches;
            set
            {
                if (value == SyncMatchOperation.UpdateOldSource || value == SyncMatchOperation.UpdateOldItem || value == SyncMatchOperation.UpdateOldDestination)
                    throw new Exception($"Conflict matches operation cannot be set to {value.ToString()}.");

                conflictMatches = value;
            }
        }

        /// <summary>
        /// Returns a string that represents the sync mode.
        /// </summary>
        public override string ToString()
        {
            var result = $"{nameof(SyncModePreset)}: {SyncModePreset}";
            if (SyncModePreset == SyncModePreset.Custom)
                result += $", {nameof(ItemsInSourceOnly)}: {ItemsInSourceOnly}, {nameof(ItemsInDestinationOnly)}: {ItemsInDestinationOnly}, {nameof(SameMatches)}: {SameMatches}, {nameof(NewerMatches)}: {NewerMatches}, {nameof(ConflictMatches)}: {ConflictMatches}";

            return result;
        }
    }
}