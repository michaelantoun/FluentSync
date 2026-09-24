using FluentSync.Sync.Providers;

namespace FluentSync.Tests.Sync.Providers
{
    public class SortedSetSyncProviderTests
    {
        [Fact]
        public void SortedSetSyncProviderShouldHaveAValidStringForNullableItems()
        {
            var provider = new SortedSetSyncProvider<int>();

            provider.ToString().ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void SortedSetSyncProviderShouldHaveAValidStringForNonNullableItems()
        {
            var provider = new SortedSetSyncProvider<int>();

            provider.Items = new SortedSet<int>();
            provider.Items.Add(1);

            provider.ToString().ShouldBe(provider.Items.ToString());
        }
    }
}
