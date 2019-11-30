using FluentAssertions;
using FluentSync.Comparers;
using FluentSync.Tests.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FluentSync.Tests.Comparers.ComparerAgent
{
    public partial class ComparerAgentTests
    {
        [Fact]
        public async Task Compare_ClassWithNullableKey_Updated()
        {
            List<Event> source = new List<Event> {
                new Event{Id = 2, Title ="Birthday", ModifiedDate = new DateTime(2000,1, 1)},
                new Event{Id = 1, Title ="soccer match", ModifiedDate = new DateTime(2000,1, 2)},
                new Event{Id = null, Title ="Private", ModifiedDate = null },
                new Event(),
                new Event{Id = 4, Title ="Hang-out", ModifiedDate = new DateTime(2000,1, 2)},
                new Event{Id = 5, Title ="bad", ModifiedDate = new DateTime(2000,1, 8)}
            }
            , destination = new List<Event> {
                new Event{Id = 1, Title ="Soccer Match", ModifiedDate = new DateTime(2000,1, 3) },
                new Event{Id = 2, Title ="Birthday", ModifiedDate = new DateTime(2000,1, 1)},
                new Event{Id = 3, Title ="Free-time", ModifiedDate = null },
                new Event{Id = 4, Title ="hang-out", ModifiedDate = new DateTime(2000,1, 1)},
                new Event{Id = 5, Title ="Bad", ModifiedDate = new DateTime(2000,1, 8)}
            };

            var comparisonResult = await ComparerAgent<int?, Event>.Create()
                .SetKeySelector(e => e.Id)
                .SetCompareItemFunc((s, d) =>
                {
                    if (s.Title == d.Title && s.ModifiedDate == d.ModifiedDate)
                        return MatchComparisonResultType.Same;
                    else if (s.ModifiedDate < d.ModifiedDate)
                        return MatchComparisonResultType.NewerDestination;
                    else if (s.ModifiedDate > d.ModifiedDate)
                        return MatchComparisonResultType.NewerSource;
                    else
                        return MatchComparisonResultType.Conflict;
                })
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            comparisonResult.ItemsInSourceOnly.Should().BeEquivalentTo(new List<Event> { source[2], source[3] });
            comparisonResult.ItemsInDestinationOnly.Should().BeEquivalentTo(new List<Event> { destination[2] });

            comparisonResult.Matches.Should().BeEquivalentTo(new List<MatchComparisonResult<Event>>
            {
                new MatchComparisonResult<Event>{Source = source[1], Destination = destination[0], ComparisonResult = MatchComparisonResultType.NewerDestination},
                new MatchComparisonResult<Event>{Source = source[0], Destination = destination[1], ComparisonResult = MatchComparisonResultType.Same},
                new MatchComparisonResult<Event>{Source = source[4], Destination = destination[3], ComparisonResult = MatchComparisonResultType.NewerSource},
                new MatchComparisonResult<Event>{Source = source[5], Destination = destination[4], ComparisonResult = MatchComparisonResultType.Conflict},
            });
        }
    }
}
