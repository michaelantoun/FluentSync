namespace FluentSync.Tests.Internals
{
    public class ShouldlyExtensionsTests
    {
        [Fact]
        public void EquivalentItemsInDifferentOrderShouldPass()
        {
            var actual = new List<Hobby> { new Hobby { Id = 1, Name = "Chess" }, new Hobby { Id = 2, Name = "Golf" } };
            var expected = new List<Hobby> { new Hobby { Id = 2, Name = "Golf" }, new Hobby { Id = 1, Name = "Chess" } };

            actual.ShouldBeEquivalentToIgnoringOrder(expected);
        }

        [Fact]
        public void DifferentCollectionTypesShouldPass()
        {
            var actual = new List<int?> { 5, null, 10 };

            actual.ShouldBeEquivalentToIgnoringOrder(new[] { null, 10, (int?)5 }.Where(x => true));
        }

        [Fact]
        public void DuplicateItemsShouldBeMatchedOneToOne()
        {
            var actual = new List<string> { "a", "a", "b" };

            Should.Throw<ShouldAssertException>(() => actual.ShouldBeEquivalentToIgnoringOrder(new List<string> { "a", "b", "b" }));
        }

        [Fact]
        public void DifferentMemberValueShouldFail()
        {
            var actual = new List<Hobby> { new Hobby { Id = 1, Name = "Chess" } };

            var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeEquivalentToIgnoringOrder(new List<Hobby> { new Hobby { Id = 1, Name = "Golf" } }));
            exception.Message.ShouldBe("Expected an item equivalent to {\"Id\":1,\"Name\":\"Golf\"}, but none was found in [{\"Id\":1,\"Name\":\"Chess\"}].");
        }

        [Fact]
        public void DifferentItemsCountShouldFail()
        {
            var actual = new List<int> { 1, 2 };

            var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeEquivalentToIgnoringOrder(new List<int> { 1 }));
            exception.Message.ShouldBe("Expected 1 items [1], but found 2 items [1,2].");
        }

        [Fact]
        public void NullActualShouldFail()
        {
            List<int> actual = null;

            Should.Throw<ShouldAssertException>(() => actual.ShouldBeEquivalentToIgnoringOrder(new List<int>()));
        }

        [Fact]
        public void NotEquivalentShouldPassForDifferentItems()
        {
            new List<int> { 1, 2 }.ShouldNotBeEquivalentToIgnoringOrder(new List<int> { 1, 3 });
        }

        [Fact]
        public void NotEquivalentShouldFailForEquivalentItems()
        {
            Should.Throw<ShouldAssertException>(() => new List<int> { 1, 2 }.ShouldNotBeEquivalentToIgnoringOrder(new List<int> { 2, 1 }));
        }

        [Fact]
        public void DictionariesWithEquivalentValuesShouldPass()
        {
            IDictionary<int?, Hobby> actual = new Dictionary<int?, Hobby> { { 1, new Hobby { Id = 1, Name = "Chess" } }, { 2, new Hobby { Id = 2, Name = "Golf" } } };
            IDictionary<int?, Hobby> expected = new Dictionary<int?, Hobby> { { 2, new Hobby { Id = 2, Name = "Golf" } }, { 1, new Hobby { Id = 1, Name = "Chess" } } };

            actual.ShouldBeEquivalentToIgnoringOrder(expected);
        }

        [Fact]
        public void DictionaryWithDifferentValueShouldFail()
        {
            IDictionary<int?, Hobby> actual = new Dictionary<int?, Hobby> { { 1, new Hobby { Id = 1, Name = "Chess" } } };
            IDictionary<int?, Hobby> expected = new Dictionary<int?, Hobby> { { 1, new Hobby { Id = 1, Name = "Golf" } } };

            var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeEquivalentToIgnoringOrder(expected));
            exception.Message.ShouldBe("Expected the entry with key 1 to be equivalent to {\"Id\":1,\"Name\":\"Golf\"}, but found {\"Id\":1,\"Name\":\"Chess\"}.");
        }

        [Fact]
        public void DictionaryWithDifferentKeyShouldFail()
        {
            IDictionary<int?, Hobby> actual = new Dictionary<int?, Hobby> { { 1, new Hobby { Id = 1, Name = "Chess" } } };
            IDictionary<int?, Hobby> expected = new Dictionary<int?, Hobby> { { 2, new Hobby { Id = 1, Name = "Chess" } } };

            var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeEquivalentToIgnoringOrder(expected));
            exception.Message.ShouldBe("Expected an entry with key 2, but none was found in [{\"Key\":1,\"Value\":{\"Id\":1,\"Name\":\"Chess\"}}].");
        }

        [Fact]
        public void DictionariesWithDifferentCountShouldFail()
        {
            IDictionary<int?, int> actual = new Dictionary<int?, int> { { 1, 1 }, { 2, 2 } };

            Should.Throw<ShouldAssertException>(() => actual.ShouldBeEquivalentToIgnoringOrder(new Dictionary<int?, int> { { 1, 1 } }));
        }
    }
}
