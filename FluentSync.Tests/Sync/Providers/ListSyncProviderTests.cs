using FluentSync.Sync.Providers;

namespace FluentSync.Tests.Sync.Providers
{
    public class ListSyncProviderTests
    {
        [Fact]
        public void ListSyncProviderShouldHaveAValidStringForNullableItems()
        {
            var provider = new ListSyncProvider<int>();

            provider.ToString().ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ListSyncProviderShouldHaveAValidStringForNonNullableItems()
        {
            var provider = new ListSyncProvider<int>();

            provider.Items = new List<int>();
            provider.Items.Add(1);

            provider.ToString().ShouldBe(provider.Items.ToString());
        }
    }
}
