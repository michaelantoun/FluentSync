namespace FluentSync.Tests.Comparers.ComparerAgent
{
    public partial class ComparerAgentTests
    {
        /// <summary>
        /// Every other duplicate-key test uses keys that are repeated exactly twice, which is the
        /// one case where counting the affected keys and counting the duplicates agree. These tests
        /// use longer runs so the two differ.
        /// </summary>
        [Fact]
        public async Task Compare_DuplicateKeys_CountsDuplicatesNotAffectedKeys_InSource()
        {
            List<string> source = new List<string> { "a", "a", "a" }
                , destination = new List<string>();

            Func<Task> act = async () => await ComparerAgent<string>.Create()
                .Configure((c) => c.AllowDuplicateKeys = RuleAllowanceType.Destination)
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            await act.Should().ThrowAsync<ArgumentException>().WithMessage("Duplicated keys are not allowed in the source list, 2 keys were found.");
        }

        [Fact]
        public async Task Compare_DuplicateKeys_CountsDuplicatesNotAffectedKeys_InDestination()
        {
            List<string> source = new List<string>()
                , destination = new List<string> { "a", "a", "a" };

            Func<Task> act = async () => await ComparerAgent<string>.Create()
                .Configure((c) => c.AllowDuplicateKeys = RuleAllowanceType.Source)
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            await act.Should().ThrowAsync<ArgumentException>().WithMessage("Duplicated keys are not allowed in the destination list, 2 keys were found.");
        }

        [Fact]
        public async Task Compare_DuplicateKeys_SumsTheDuplicatesOfEveryKey()
        {
            // "a" appears 3 times and "b" twice, so there are 2 + 1 = 3 duplicates across 2 keys.
            List<string> source = new List<string> { "a", "a", "a", "b", "b" }
                , destination = new List<string>();

            Func<Task> act = async () => await ComparerAgent<string>.Create()
                .Configure((c) => c.AllowDuplicateKeys = RuleAllowanceType.Destination)
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            await act.Should().ThrowAsync<ArgumentException>().WithMessage("Duplicated keys are not allowed in the source list, 3 keys were found.");
        }

        [Fact]
        public async Task Compare_DuplicateKeys_CountsNullableAndNonNullableKeysTogether()
        {
            // Id 1 appears 3 times (2 duplicates) and the null key 3 times (2 duplicates).
            List<Person> source = new List<Person>
            {
                new Person{ Id = 1, FirstName = "Tom" },
                new Person{ Id = 1, FirstName = "Tim" },
                new Person{ Id = 1, FirstName = "Tam" },
                new Person{ Id = null, FirstName = "Abby" },
                new Person{ Id = null, FirstName = "Bobby" },
                new Person{ Id = null, FirstName = "Cathy" }
            }
            , destination = new List<Person>();

            Func<Task> act = async () => await ComparerAgent<int?, Person>.Create()
                .Configure((c) => c.AllowDuplicateKeys = RuleAllowanceType.Destination)
                .SetKeySelector(x => x.Id)
                .SetCompareItemFunc((s, d) => s.FirstName == d.FirstName ? MatchComparisonResultType.Same : MatchComparisonResultType.Conflict)
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            await act.Should().ThrowAsync<ArgumentException>().WithMessage("Duplicated keys are not allowed in the source list, 4 keys were found.");
        }

        /// <summary>
        /// The duplicate-key and duplicate-item rules are configured separately, so their messages
        /// have to name the rule that actually fired.
        /// </summary>
        [Fact]
        public async Task Compare_DuplicateKeysAndItems_MessagesNameTheRuleThatFired()
        {
            List<string> source = new List<string> { "a", "a", "a" }
                , destination = new List<string>();

            Func<Task> duplicateKeys = async () => await ComparerAgent<string>.Create()
                .Configure((c) => c.AllowDuplicateKeys = RuleAllowanceType.Destination)
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            Func<Task> duplicateItems = async () => await ComparerAgent<string>.Create()
                .Configure((c) => c.AllowDuplicateItems = RuleAllowanceType.Destination)
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            await duplicateKeys.Should().ThrowAsync<ArgumentException>().WithMessage("Duplicated keys are not allowed in the source list, 2 keys were found.");
            await duplicateItems.Should().ThrowAsync<ArgumentException>().WithMessage("Duplicated items are not allowed in the source list, 2 items were found.");
        }
    }
}
