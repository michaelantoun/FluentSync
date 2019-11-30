# FluentSync
A .Net library with fluent interface for comparing and synchronizing records.

### Get Started
FluentSync can be installed using the Nuget package manager or the `dotnet` CLI.

```
Install-Package FluentSync
```


---
[![][build-img]][build]
[![][nuget-img]][nuget]


### Example
```csharp
await BatchSyncAgent<int, Person>.Create()
	.Configure((c) => c.SyncMode.SyncModePreset = SyncModePreset.UpdateDestination)
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
	.SyncAsync(cancellationToken);
```
