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
    // Testing the comparer agent with a class that has a compound primary key
    public partial class ComparerAgentTests
    {
        [Fact]
        public async Task Compare_ClassWithCompoundKey_NonEmptyLists()
        {
            List<PersonHobby> source = new List<PersonHobby> {
                new PersonHobby{PersonId = 10, HobbyId =100, LoveScale = 1},
                new PersonHobby{PersonId = 10, HobbyId = 101, LoveScale = null},
                new PersonHobby{PersonId = 10, HobbyId =102, LoveScale = 2},
                new PersonHobby{PersonId = 20, HobbyId =100, LoveScale = 3},
                new PersonHobby()
            }
            , destination = new List<PersonHobby> {
                new PersonHobby{PersonId = 10, HobbyId =100, LoveScale = 1},
                new PersonHobby{PersonId = 30, HobbyId = 101, LoveScale = null},
                new PersonHobby{PersonId = 20, HobbyId =100, LoveScale = 4},
            };

            var comparisonResult = await ComparerAgent<Tuple<int?, int?>, PersonHobby>.Create()
                .SetKeySelector(x => new Tuple<int?, int?>(x.PersonId, x.HobbyId))
                .SetCompareItemFunc((s, d) => (s.PersonId == d.PersonId && s.HobbyId == d.HobbyId && s.LoveScale == d.LoveScale) ? MatchComparisonResultType.Same : MatchComparisonResultType.Conflict)
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            comparisonResult.ItemsInSourceOnly.Should().BeEquivalentTo(new List<PersonHobby> { source[1], source[2], source[4] });
            comparisonResult.ItemsInDestinationOnly.Should().BeEquivalentTo(new List<PersonHobby> { destination[1] });

            comparisonResult.Matches.Should().BeEquivalentTo(new List<MatchComparisonResult<PersonHobby>>
            {
                new MatchComparisonResult<PersonHobby>{Source = source[0], Destination = destination[0], ComparisonResult = MatchComparisonResultType.Same},
                new MatchComparisonResult<PersonHobby>{Source = source[3], Destination = destination[2], ComparisonResult = MatchComparisonResultType.Conflict},
            });
        }
    }
}
