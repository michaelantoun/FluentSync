using FluentAssertions;
using FluentSync.Comparers;
using FluentSync.Comparers.Configurations;
using FluentSync.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FluentSync.Tests.Comparers.ComparerAgent
{
    // Testing the comparer agent with a class that has a null-able key
    public partial class ComparerAgentTests
    {
        [Fact]
        public async Task Compare_ClassWithNullableKey_NonEmptyLists()
        {
            List<Person> source = new List<Person> {
                new Person{Id = 2, FirstName ="Tom", LastName = "Smith", DOB = new DateTime(2000,1, 1)},
                new Person{Id = 1, FirstName ="Tom", LastName = null, DOB = new DateTime(2000,1, 1)},
                new Person{Id = null, FirstName ="Abby", LastName = "Smith", DOB = null },
                new Person()
            }
            , destination = new List<Person> {
                new Person{Id = 2, FirstName ="Tom", LastName = "Smith", DOB = new DateTime(2000,1, 1)},
                new Person{Id = 1, FirstName ="Tom", LastName = "Tim", DOB = null },
                new Person{Id = 3, FirstName ="Joe", LastName = "Jim", DOB = null }
            };

            var comparisonResult = await ComparerAgent<int?, Person>.Create()
                .SetKeySelector(person => person.Id)
                .SetCompareItemFunc((s, d) => (s.Id == d.Id && s.FirstName == d.FirstName && s.LastName == d.LastName && s.DOB == d.DOB) ? MatchComparisonResultType.Same : MatchComparisonResultType.Conflict)
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            comparisonResult.ItemsInSourceOnly.Should().BeEquivalentTo(new List<Person> { source[2], source[3] });
            comparisonResult.ItemsInDestinationOnly.Should().BeEquivalentTo(new List<Person> { destination[2] });

            comparisonResult.Matches.Should().BeEquivalentTo(new List<MatchComparisonResult<Person>>
            {
                new MatchComparisonResult<Person>{Source = source[0], Destination = destination[0], ComparisonResult = MatchComparisonResultType.Same},
                new MatchComparisonResult<Person>{Source = source[1], Destination = destination[1], ComparisonResult = MatchComparisonResultType.Conflict},
            });
        }

        [Fact]
        public async Task Compare_ClassWithNullableKey_HasDuplicatesInSource()
        {
            List<Person> source = new List<Person> {
                new Person{Id = 1, FirstName ="Tom", LastName = "Smith", DOB = new DateTime(2000,1, 1)},
                new Person{Id = 1, FirstName ="Tom", LastName = "Smith", DOB = new DateTime(2000,1, 1)},
                new Person{Id = null, FirstName ="Abby", LastName = "Smith", DOB = null },
                new Person{Id = null, FirstName ="Abby", LastName = "Smith", DOB = null },
                new Person(),
                new Person(),
                new Person{Id = 3, FirstName ="Joe", LastName = "jim", DOB = null },
                new Person{Id = 3, FirstName ="Joe", LastName = "jim", DOB = null }
            }
            , destination = new List<Person> {
                new Person{Id = 3, FirstName ="Joe", LastName = "Jim", DOB = null },
                new Person{Id = 1, FirstName ="Tom", LastName = "Smith", DOB = new DateTime(2000,1, 1)},
                new Person{Id = 2, FirstName ="Tom", LastName = "Tim", DOB = null }
            };

            var comparisonResult = await ComparerAgent<int?, Person>.Create()
                .SetKeySelector(person => person.Id)
                .SetCompareItemFunc((s, d) => (s.Id == d.Id && s.FirstName == d.FirstName && s.LastName == d.LastName && s.DOB == d.DOB) ? MatchComparisonResultType.Same : MatchComparisonResultType.Conflict)
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            comparisonResult.ItemsInSourceOnly.Should().BeEquivalentTo(new List<Person> { source[1], source[2], source[3], source[4], source[5], source[7] });
            comparisonResult.ItemsInDestinationOnly.Should().BeEquivalentTo(new List<Person> { destination[2] });

            comparisonResult.Matches.Should().BeEquivalentTo(new List<MatchComparisonResult<Person>>
            {
                new MatchComparisonResult<Person>{Source = source[0], Destination = destination[1], ComparisonResult = MatchComparisonResultType.Same},
                new MatchComparisonResult<Person>{Source = source[6], Destination = destination[0], ComparisonResult = MatchComparisonResultType.Conflict},
            });
        }

        [Fact]
        public void Compare_ClassWithNullableKey_PreventDuplicatesInSourceUsingItemComparer()
        {
            List<Person> source = new List<Person> {
                new Person{Id = 1, FirstName ="Tom", LastName = "Smith", DOB = new DateTime(2000,1, 1)},
                new Person{Id = 1, FirstName ="Tom", LastName = "Smith", DOB = new DateTime(2000,1, 1)},
                new Person{Id = null, FirstName ="Abby", LastName = "Smith", DOB = null },
                new Person{Id = null, FirstName ="Abby", LastName = "Smith", DOB = null },
                new Person(),
                new Person(),
                new Person{Id = 3, FirstName ="Joe", LastName = "jim", DOB = null },
                new Person{Id = 3, FirstName ="Joe", LastName = "jim", DOB = null }
            }
            , destination = new List<Person> {
                new Person{Id = 3, FirstName ="Joe", LastName = "Jim", DOB = null },
                new Person{Id = 1, FirstName ="Tom", LastName = "Smith", DOB = new DateTime(2000,1, 1)},
                new Person{Id = 2, FirstName ="Tom", LastName = "Tim", DOB = null }
            };

            Func<Task> act = async () => await ComparerAgent<int?, Person>.Create()
                .Configure((c) => c.AllowDuplicateKeys = RuleAllowanceType.None)
                .SetKeySelector(person => person.Id)
                .SetCompareItemFunc((s, d) => (s.Id == d.Id && s.FirstName == d.FirstName && s.LastName == d.LastName && s.DOB == d.DOB) ? MatchComparisonResultType.Same : MatchComparisonResultType.Conflict)
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().Throw<ArgumentException>().WithMessage("Duplicated items are not allowed in the source list, 5 items were found.");
        }

        [Fact]
        public void Compare_ClassWithNullableKey_PreventDuplicatesInDestinationUsingItemComparer()
        {
            List<Person> source = new List<Person> {
                new Person{Id = 3, FirstName ="Joe", LastName = "Jim", DOB = null },
                new Person{Id = 1, FirstName ="Tom", LastName = "Smith", DOB = new DateTime(2000,1, 1)},
                new Person{Id = 2, FirstName ="Tom", LastName = "Tim", DOB = null }
            }
            , destination = new List<Person> {
                new Person{Id = 1, FirstName ="Tom", LastName = "Smith", DOB = new DateTime(2000,1, 1)},
                new Person{Id = 1, FirstName ="Tom", LastName = "Smith", DOB = new DateTime(2000,1, 1)},
                new Person{Id = null, FirstName ="Abby", LastName = "Smith", DOB = null },
                new Person{Id = null, FirstName ="Abby", LastName = "Smith", DOB = null },
                new Person(),
                new Person(),
                new Person{Id = 3, FirstName ="Joe", LastName = "jim", DOB = null },
                new Person{Id = 3, FirstName ="Joe", LastName = "jim", DOB = null }
            };

            Func<Task> act = async () => await ComparerAgent<int?, Person>.Create()
                .Configure((c) => c.AllowDuplicateKeys = RuleAllowanceType.None)
                .SetKeySelector(person => person.Id)
                .SetCompareItemFunc((s, d) => (s.Id == d.Id && s.FirstName == d.FirstName && s.LastName == d.LastName && s.DOB == d.DOB) ? MatchComparisonResultType.Same : MatchComparisonResultType.Conflict)
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().Throw<ArgumentException>().WithMessage("Duplicated items are not allowed in the destination list, 5 items were found.");
        }

        [Fact]
        public void Compare_ClassWithNullableKey_PreventDuplicatesInSourceUsingItemEqualityComparer()
        {
            List<Person> source = new List<Person> {
                new Person{Id = 1, FirstName ="Tom", LastName = "Smith", DOB = new DateTime(2000,1, 1)},
                new Person{Id = 1, FirstName ="Tom", LastName = "Smith", DOB = new DateTime(2000,1, 1)},
                new Person{Id = null, FirstName ="Abby", LastName = "Smith", DOB = null },
                new Person{Id = null, FirstName ="Abby", LastName = "Smith", DOB = null },
                new Person(),
                new Person(),
                new Person(),
                new Person{Id = null, FirstName ="Abby", LastName = "Smith", DOB = null },
                new Person{Id = 3, FirstName ="Joe", LastName = "jim", DOB = null },
                new Person{Id = null, FirstName ="Abby", LastName = "Smith", DOB = null }
            }
            , destination = new List<Person> {
                new Person{Id = 3, FirstName ="Joe", LastName = "Jim", DOB = null },
                new Person{Id = 1, FirstName ="Tom", LastName = "Smith", DOB = new DateTime(2000,1, 1)},
                new Person{Id = 2, FirstName ="Tom", LastName = "Tim", DOB = null }
            };

            Func<Task> act = async () => await ComparerAgent<int?, Person>.Create()
                .Configure((c) => c.AllowDuplicateItems = RuleAllowanceType.None)
                .SetKeySelector(person => person.Id)
                .SetCompareItemFunc((s, d) => (s.Id == d.Id && s.FirstName == d.FirstName && s.LastName == d.LastName && s.DOB == d.DOB) ? MatchComparisonResultType.Same : MatchComparisonResultType.Conflict)
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().Throw<ArgumentException>().WithMessage("Duplicated items are not allowed in the source list, 6 items were found.");
        }

        [Fact]
        public void Compare_ClassWithNullableKey_PreventDuplicatesInDestinationUsingItemEqualityComparer()
        {
            List<Person> source = new List<Person> {
                new Person{Id = 3, FirstName ="Joe", LastName = "Jim", DOB = null },
                new Person{Id = 1, FirstName ="Tom", LastName = "Smith", DOB = new DateTime(2000,1, 1)},
                new Person{Id = 2, FirstName ="Tom", LastName = "Tim", DOB = null }
            }
            , destination = new List<Person> {
                new Person { Id = 1, FirstName = "Tom", LastName = "Smith", DOB = new DateTime(2000, 1, 1) },
                new Person { Id = 1, FirstName = "Tom", LastName = "Smith", DOB = new DateTime(2000, 1, 1) },
                new Person { Id = null, FirstName = "Abby", LastName = "Smith", DOB = null },
                new Person { Id = null, FirstName = "Abby", LastName = "Smith", DOB = null },
                new Person(),
                new Person(),
                new Person { Id = null, FirstName = "Abby", LastName = "Smith", DOB = null },
                new Person { Id = 3, FirstName = "Joe", LastName = "jim", DOB = null },
                new Person { Id = null, FirstName = "Abby", LastName = "Smith", DOB = null }
            };

            Func<Task> act = async () => await ComparerAgent<int?, Person>.Create()
                .Configure((c) => c.AllowDuplicateItems = RuleAllowanceType.None)
                .SetKeySelector(person => person.Id)
                .SetCompareItemFunc((s, d) => (s.Id == d.Id && s.FirstName == d.FirstName && s.LastName == d.LastName && s.DOB == d.DOB) ? MatchComparisonResultType.Same : MatchComparisonResultType.Conflict)
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().Throw<ArgumentException>().WithMessage("Duplicated items are not allowed in the destination list, 5 items were found.");
        }

        [Fact]
        public void Compare_ClassWithNullableKey_PreventNegativeIdsInSource()
        {
            List<Person> source = new List<Person> {
                new Person{Id = 3, FirstName ="Joe", LastName = "Jim", DOB = null },
                new Person{Id = -1, FirstName ="Tom", LastName = "Smith", DOB = new DateTime(2000,1, 1)},
                new Person{Id = 2, FirstName ="Tom", LastName = "Tim", DOB = null }
            }
            , destination = new List<Person> {
                new Person { Id = 1, FirstName = "Tom", LastName = "Smith", DOB = new DateTime(2000, 1, 1) },
                new Person { Id = 2, FirstName = "Tom", LastName = "Smith", DOB = new DateTime(2000, 1, 1) }
            };

            Func<Task> act = async () => await ComparerAgent<int?, Person>.Create()
                .SetValidateItemsAction((s, d) =>
                {
                    int count = s.Count(x => x.Id < 0);
                    if (count > 0)
                        throw new ArgumentException(string.Format("Negative Ids are invalid in the source list, {0} item{1} {2} found."
                            , count, count == 1 ? "" : "s", count == 1 ? "was" : "were"));

                    count = d.Count(x => x.Id < 0);
                    if (count > 0)
                        throw new ArgumentException(string.Format("Negative Ids are invalid in the destination list, {0} item{1} {2} found."
                            , count, count == 1 ? "" : "s", count == 1 ? "was" : "were"));
                })
                .SetKeySelector(person => person.Id)
                .SetCompareItemFunc((s, d) => (s.Id == d.Id && s.FirstName == d.FirstName && s.LastName == d.LastName && s.DOB == d.DOB) ? MatchComparisonResultType.Same : MatchComparisonResultType.Conflict)
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().Throw<ArgumentException>().WithMessage("Negative Ids are invalid in the source list, 1 item was found.");
        }

        [Fact]
        public void Compare_ClassWithNullableKey_PreventNegativeIdsInDestination()
        {
            List<Person> source = new List<Person> {
                new Person{Id = 3, FirstName ="Joe", LastName = "Jim", DOB = null },
                new Person{Id = 1, FirstName ="Tom", LastName = "Smith", DOB = new DateTime(2000,1, 1)},
                new Person{Id = 2, FirstName ="Tom", LastName = "Tim", DOB = null }
            }
            , destination = new List<Person> {
                new Person { Id = -1, FirstName = "Tom", LastName = "Smith", DOB = new DateTime(2000, 1, 1) },
                new Person { Id = -2, FirstName = "Tom", LastName = "Smith", DOB = new DateTime(2000, 1, 1) }
            };

            Func<Task> act = async () => await ComparerAgent<int?, Person>.Create()
                .SetValidateItemsAction((s, d) =>
                {
                    int count = s.Count(x => x.Id < 0);
                    if (count > 0)
                        throw new ArgumentException(string.Format("Negative Ids are invalid in the source list, {0} item{1} {2} found."
                            , count, count == 1 ? "" : "s", count == 1 ? "was" : "were"));

                    count = d.Count(x => x.Id < 0);
                    if (count > 0)
                        throw new ArgumentException(string.Format("Negative Ids are invalid in the destination list, {0} item{1} {2} found."
                            , count, count == 1 ? "" : "s", count == 1 ? "was" : "were"));
                })
                .SetKeySelector(person => person.Id)
                .SetCompareItemFunc((s, d) => (s.Id == d.Id && s.FirstName == d.FirstName && s.LastName == d.LastName && s.DOB == d.DOB) ? MatchComparisonResultType.Same : MatchComparisonResultType.Conflict)
                .SetSourceProvider(source)
                .SetDestinationProvider(destination)
                .CompareAsync(CancellationToken.None).ConfigureAwait(false);

            act.Should().Throw<ArgumentException>().WithMessage("Negative Ids are invalid in the destination list, 2 items were found.");
        }
    }
}
