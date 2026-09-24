namespace FluentSync.Tests.Comparers.ComparerAgent
{
    // Testing the comparer agent with a class that has a non null-able key
    public partial class ComparerAgentTests
    {
        [Fact]
        public async Task Compare_ClassWithNonNullableKey_NonEmptyLists()
        {
            List<Hobby> source = new List<Hobby> {
                new Hobby{Id = 2, Name ="Drawing"},
                new Hobby{Id = 1, Name ="reading"},
                new Hobby{Id = default, Name ="Coding" },
                new Hobby()
            }
            , destination = new List<Hobby> {
                new Hobby{Id = 2, Name ="Drawing"},
                new Hobby{Id = 1, Name ="Reading"},
                new Hobby{Id = 3, Name = "Coloring" }
            };

            var comparisonResult = await ComparerAgent<int, Hobby>.Create()
                .SetKeySelector(hobby => hobby.Id)
                .SetCompareItemFunc((s, d) => (s.Id == d.Id && s.Name == d.Name) ? MatchComparisonResultType.Same : MatchComparisonResultType.Conflict)
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None);

            comparisonResult.ItemsInSourceOnly.ShouldBeEquivalentToIgnoringOrder(new List<Hobby> { source[2], source[3] });
            comparisonResult.ItemsInDestinationOnly.ShouldBeEquivalentToIgnoringOrder(new List<Hobby> { destination[2] });

            comparisonResult.Matches.ShouldBeEquivalentToIgnoringOrder(new List<MatchComparisonResult<Hobby>>
            {
                new MatchComparisonResult<Hobby>{Source = source[0], Destination = destination[0], ComparisonResult = MatchComparisonResultType.Same},
                new MatchComparisonResult<Hobby>{Source = source[1], Destination = destination[1], ComparisonResult = MatchComparisonResultType.Conflict},
            });
        }
    }
}
