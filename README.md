<img src="https://github.com/michaelantoun/FluentSync/blob/master/Icon512x512.png" alt="FluentSync" width="128px" />
A .Net library with fluent interface for comparing and synchronizing entities/records.


### Overview
Have you ever wanted to synchronize data between multiple databases, systems, database and an API, etc.? Have you ever wanted to resync data between multiple systems because something went wrong and some data is not up-to-date? If the answer is **Yes** then this library is for you.

### Get Started
FluentSync can be installed using the Nuget package manager or the `dotnet` CLI.

```
Install-Package FluentSync
```

### Why FluentSync
* The library is developed with .Net Standard
* Fluent interface
* No dependencies on other libraries
* More than 220+ tests
* 99% Code coverage

### Usage
The FluentSync library has Comparer agents and Sync agents. The Comparer agent compares the source and destination items/entities. Then the Sync agent uses this comparison result to determine which items/entities will be inserted/updated/deleted in the source and destination according to the sync configurations.


**Sync modes**:
* **TwoWay**: updates the source and the destination to have the same items.
* **MirrorToDestination**: updates the destination only to have the same items as in the source and deletes any item in destination that does have a match.
* **MirrorToSource**: updates the source only to have the same items as in the destination and deletes any item in source that does have a match.
* **UpdateDestination**: updates the destination items only to have the same items as in the source. It does not delete any item from the destination and it does not update conflicts.
* **UpdateSource**: updates the source items only to have the same items as in the destination. It does not delete any item from the source and it does not update conflicts.
* **Custom**: you can have your own custom sync mode by customizing the properties of the SyncMode class.

#### ComparerAgent Usage
Basically, you can you compare two lists of any type such as a string with the following code:
```csharp
List<string> source = new List<string> { "Tom", "Tim", "bob", "Zoo" }
	, destination = new List<string> { "Bob", "Sam", "Tim" };

var comparisonResult = await ComparerAgent<string>.Create()
	.SetSourceProvider(source)
	.SetDestinationProvider(destination)
	.CompareAsync(CancellationToken.None).ConfigureAwait(false);
    
// This is the comparison result which is verified by the FluentAssertion library
comparisonResult.ItemsInSourceOnly.Should().BeEquivalentTo(new List<string> { "Tom", "bob", "Zoo" });
comparisonResult.ItemsInDestinationOnly.Should().BeEquivalentTo(new List<string> { "Bob", "Sam" });

comparisonResult.Matches.Should().BeEquivalentTo(new List<MatchComparisonResult<string>>
{
	new MatchComparisonResult<string>{Source = "Tim", Destination = "Tim", ComparisonResult = MatchComparisonResultType.Same}
});
```

You can also have a custom key selector for the ComparerAgent to identify each item.
```csharp
List<string> source = new List<string> { "Tom", "Tim", "bob", "Zoo" }
	, destination = new List<string> { "Bob", "Sam", "Tim" };

var comparisonResult = await ComparerAgent<string>.Create()
	.SetKeySelector(x => x?.ToLower())
	.SetSourceProvider(source)
	.SetDestinationProvider(destination)
	.CompareAsync(CancellationToken.None).ConfigureAwait(false);

// This is the comparison result
comparisonResult.ItemsInSourceOnly.Should().BeEquivalentTo(new List<string> { "Tom", "Zoo" });
comparisonResult.ItemsInDestinationOnly.Should().BeEquivalentTo(new List<string> { "Sam" });

comparisonResult.Matches.Should().BeEquivalentTo(new List<MatchComparisonResult<string>>
{
	new MatchComparisonResult<string>{Source = "Tim", Destination = "Tim", ComparisonResult = MatchComparisonResultType.Same},
	new MatchComparisonResult<string>{Source = "bob", Destination = "Bob", ComparisonResult = MatchComparisonResultType.Conflict}
});
```

Let's compare entities and determine which entities are newer if they exist in the source and destination. The Source/Destination provider for the comparer agent is a provider that implements IComparerAgent<TItem> interface which gets the items/entities.
```csharp
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
```

Do you have a compound primary key for the entity? Yes. No problem, it is supported by using Tuple.
```csharp
var comparisonResult = await ComparerAgent<Tuple<int?, int?>, PersonHobby>.Create()
	.SetKeySelector(x => new Tuple<int?, int?>(x.PersonId, x.HobbyId))
	.SetCompareItemFunc((s, d) => (s.PersonId == d.PersonId && s.HobbyId == d.HobbyId && s.LoveScale == d.LoveScale) ? MatchComparisonResultType.Same : MatchComparisonResultType.Conflict)
	.SetSourceProvider(source)
	.SetDestinationProvider(destination)
	.CompareAsync(CancellationToken.None).ConfigureAwait(false);
```

#### SyncAgent Usage
Simply, you can compare and sync the two lists with the following code:
```csharp
List<int> source = new List<int> { 5, 4, 9 }
	, destination = new List<int> { 6, 10, 5 };

await SyncAgent<int>.Create()
	.Configure((c) => c.SyncMode.SyncModePreset = SyncModePreset.MirrorToDestination)
	.SetComparerAgent(ComparerAgent<int>.Create())
	.SetSourceProvider(source)
	.SetDestinationProvider(destination)
	.SyncAsync(CancellationToken.None).ConfigureAwait(false);

// This code is here to verify the two lists
source.Should().BeEquivalentTo(new List<int> { 5, 4, 9 });
destination.Should().BeEquivalentTo(new List<int> { 5, 4, 9 });
```

Another example of the sync agent with a custom key selector and an item comparer function.
```csharp
await SyncAgent<string>.Create()
	.Configure((c) => c.SyncMode.SyncModePreset = SyncModePreset.TwoWay)
	.SetComparerAgent(ComparerAgent<string>.Create()
		.SetKeySelector(x => x?.ToLower())
		.SetCompareItemFunc((s, d) => s == d ? MatchComparisonResultType.Same : MatchComparisonResultType.Conflict))
	.SetSourceProvider(sourceItems)
	.SetDestinationProvider(destinationItems)
    .SyncAsync(CancellationToken.None).ConfigureAwait(false);
```

You can also compare and sync entities. The Source/Destination provider for the sync agent is a provider that implements IComparerSyncProvider<TItem> interface which does the CRUD operations for the items/entities.
```csharp
await SyncAgent<int?, Event>.Create()
	.Configure((c) => c.SyncMode.ItemsInSourceOnly = SyncItemOperation.Add) // Custom SyncMode
	.SetComparerAgent(ComparerAgent<int?, Event>.Create()
	.SetKeySelector(x => x.Id)
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
	}))
	.SetSourceProvider(source)
	.SetDestinationProvider(destination)
	.SyncAsync(CancellationToken.None).ConfigureAwait(false);
```
**Important note:** the SyncAgent loads all the entities in memory in one call, then compares them, and finally synchronizes the entities. It is not recommended to use this agent if you have thousands of entities, use BatchSyncAgent instead.

#### BatchSyncAgent Usage
It is **highly recommended** to use the **BatchSyncAgent** to synchronize the entities if you have thousands of entities; it synchronizes them in batches. The Source/Destination provider for the batch sync agent is a provider that implements IComparerBatchSyncProvider<TKey, TItem> interface which does the CRUD operations for the items/entities.
```csharp
await BatchSyncAgent<int, Person>.Create()
	.Configure((c) =>
	{
		c.SyncMode.SyncModePreset = SyncModePreset.UpdateDestination;
		c.BatchSize = 50;
	})
	.SetComparerAgent(KeyComparerAgent<int>.Create())
	.SetKeySelector(x => x.BusinessEntityID)
	.SetCompareItemFunc((s, d) =>
	{
		if (s.FirstName == d.FirstName
			&& s.LastName == d.LastName
			&& s.ModifiedDate == d.ModifiedDate)
			return MatchComparisonResultType.Same;
		else if (s.ModifiedDate < d.ModifiedDate)
			return MatchComparisonResultType.NewerDestination;
		else if (s.ModifiedDate > d.ModifiedDate)
			return MatchComparisonResultType.NewerSource;
		else
			return MatchComparisonResultType.Conflict;
	})
	.SetSourceProvider(sourceProvider)
	.SetDestinationProvider(destinationProvider)
    // You can use these actions to show the progress on the screen or in a log file.
	.SetBeforeSyncingKeysAction((cr) => Console.WriteLine($"Before syncing persons > persons in source only: {cr.KeysInSourceOnly.Count}, persons in destination only: {cr.KeysInDestinationOnly.Count}, persons in source and destination: {cr.Matches.Count}"))
	.SetBeforeSyncingAction((cr) => Console.WriteLine($"Syncing persons batch > add: {cr.ItemsInSourceOnly.Count}, remove: {cr.ItemsInDestinationOnly.Count}, update: {cr.Matches.Count(x => x.ComparisonResult != MatchComparisonResultType.Same)}, Same: {cr.Matches.Count(x => x.ComparisonResult == MatchComparisonResultType.Same)}"))
	.SetBeforeDeletingItemsFromSourceAction((list) => Console.WriteLine($"Deleting from source {list.Count} persons"))
	.SetBeforeDeletingItemsFromDestinationAction((list) => Console.WriteLine($"Deleting from destination {list.Count} persons"))
	.SyncAsync(cancellationToken);
```
**Important note:** the BatchSyncAgent loads all the keys in memory in one call at the beginning, which consumes less memory than loading all entities, then compares the keys only. After that it loads the batch entities by the keys and compares them, and finally syncs the entities.

#### More Examples
For more examples you can look at the unit tests project **FluentSync.Tests**.
