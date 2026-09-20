using FluentSync.Benchmarks.Models;

namespace FluentSync.Benchmarks.Fixtures;

internal static class DataGenerator
{
    public static (List<BenchPerson> Source, List<BenchPerson> Destination) CreateLists(int count, int seed)
    {
        var (src, dest) = CreateDictionaries(count, seed);
        return (src.Values.ToList(), dest.Values.ToList());
    }

    public static (Dictionary<int, BenchPerson> Source, Dictionary<int, BenchPerson> Destination) CreateDictionaries(int count, int seed)
    {
        var source = new Dictionary<int, BenchPerson>(capacity: count);
        var destination = new Dictionary<int, BenchPerson>(capacity: count);

        var random = new Random(seed);
        var baseDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        for (int i = 0; i < count; i++)
        {
            // Bucket: 0=source-only, 1=destination-only, 2=match (same), 3=match (changed)
            int bucket = random.Next(0, 4);
            var src = NewPerson(i, baseDate, random);

            switch (bucket)
            {
                case 0:
                    source[i] = src;
                    break;
                case 1:
                    destination[i] = src;
                    break;
                case 2:
                    source[i] = src;
                    destination[i] = CloneSame(src);
                    break;
                default:
                    source[i] = src;
                    destination[i] = CloneChanged(src, random.Next(-1, 2));
                    break;
            }
        }

        return (source, destination);
    }

    public static (List<int> Source, List<int> Destination) CreateKeySets(int count, int seed)
    {
        var source = new List<int>(capacity: count);
        var destination = new List<int>(capacity: count);

        var random = new Random(seed);

        for (int i = 0; i < count; i++)
        {
            int bucket = random.Next(0, 3);
            switch (bucket)
            {
                case 0:
                    source.Add(i);
                    break;
                case 1:
                    destination.Add(i);
                    break;
                default:
                    source.Add(i);
                    destination.Add(i);
                    break;
            }
        }

        return (source, destination);
    }

    public static List<BenchPerson> CloneList(List<BenchPerson> template)
    {
        var result = new List<BenchPerson>(template.Count);
        foreach (var p in template)
            result.Add(ClonePerson(p));
        return result;
    }

    public static Dictionary<int, BenchPerson> CloneDictionary(Dictionary<int, BenchPerson> template)
    {
        var result = new Dictionary<int, BenchPerson>(template.Count);
        foreach (var kvp in template)
            result.Add(kvp.Key, ClonePerson(kvp.Value));
        return result;
    }

    private static BenchPerson NewPerson(int id, DateTime baseDate, Random random)
    {
        return new BenchPerson
        {
            Id = id,
            FirstName = "First_" + id,
            LastName = "Last_" + id,
            ModifiedDate = baseDate.AddSeconds(random.Next(0, 1_000_000)),
        };
    }

    private static BenchPerson CloneSame(BenchPerson p) => new()
    {
        Id = p.Id,
        FirstName = p.FirstName,
        LastName = p.LastName,
        ModifiedDate = p.ModifiedDate,
    };

    // Same key, different content. dateOffsetSeconds picks the comparison outcome:
    // -1 = newer source, 0 = conflict (same date), 1 = newer destination.
    private static BenchPerson CloneChanged(BenchPerson p, int dateOffsetSeconds) => new()
    {
        Id = p.Id,
        FirstName = p.FirstName + "_d",
        LastName = p.LastName,
        ModifiedDate = p.ModifiedDate.AddSeconds(dateOffsetSeconds),
    };

    private static BenchPerson ClonePerson(BenchPerson p) => new()
    {
        Id = p.Id,
        FirstName = p.FirstName,
        LastName = p.LastName,
        ModifiedDate = p.ModifiedDate,
    };
}
