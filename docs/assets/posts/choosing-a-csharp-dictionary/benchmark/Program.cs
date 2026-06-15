using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;

var config = DefaultConfig.Instance
    .AddJob(Job.ShortRun);

BenchmarkSwitcher.FromAssembly(typeof(LookupBenchmarks).Assembly).Run(args, config);

[MemoryDiagnoser]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[HideColumns("Job", "Error", "StdDev", "Median", "RatioSD")]
public class LookupBenchmarks
{
    private KeyValuePair<string, int>[] _items = [];
    private string[] _lookupKeys = [];
    private Dictionary<string, int> _dictionary = null!;
    private ReadOnlyDictionary<string, int> _readOnlyDictionary = null!;
    private ImmutableDictionary<string, int> _immutableDictionary = null!;
    private FrozenDictionary<string, int> _frozenDictionary = null!;
    private ConcurrentDictionary<string, int> _concurrentDictionary = null!;

    [Params(10_000)]
    public int Count { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _items = CreateItems(Count);
        _dictionary = new Dictionary<string, int>(_items, StringComparer.Ordinal);
        _readOnlyDictionary = new ReadOnlyDictionary<string, int>(_dictionary);
        _immutableDictionary = _items.ToImmutableDictionary(x => x.Key, x => x.Value, StringComparer.Ordinal);
        _frozenDictionary = _items.ToFrozenDictionary(x => x.Key, x => x.Value, StringComparer.Ordinal);
        _concurrentDictionary = new ConcurrentDictionary<string, int>(_items, StringComparer.Ordinal);
        _lookupKeys = _items.Select(x => x.Key).ToArray();

        Shuffle(_lookupKeys);
    }

    [Benchmark(Baseline = true)]
    public int Dictionary_TryGetValue() => Lookup(_dictionary);

    [Benchmark]
    public int ReadOnlyDictionary_TryGetValue() => Lookup(_readOnlyDictionary);

    [Benchmark]
    public int ImmutableDictionary_TryGetValue() => Lookup(_immutableDictionary);

    [Benchmark]
    public int FrozenDictionary_TryGetValue() => Lookup(_frozenDictionary);

    [Benchmark]
    public int ConcurrentDictionary_TryGetValue() => Lookup(_concurrentDictionary);

    private int Lookup(Dictionary<string, int> dictionary)
    {
        var sum = 0;
        foreach (var key in _lookupKeys)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                sum += value;
            }
        }

        return sum;
    }

    private int Lookup(ReadOnlyDictionary<string, int> dictionary)
    {
        var sum = 0;
        foreach (var key in _lookupKeys)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                sum += value;
            }
        }

        return sum;
    }

    private int Lookup(ImmutableDictionary<string, int> dictionary)
    {
        var sum = 0;
        foreach (var key in _lookupKeys)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                sum += value;
            }
        }

        return sum;
    }

    private int Lookup(FrozenDictionary<string, int> dictionary)
    {
        var sum = 0;
        foreach (var key in _lookupKeys)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                sum += value;
            }
        }

        return sum;
    }

    private int Lookup(ConcurrentDictionary<string, int> dictionary)
    {
        var sum = 0;
        foreach (var key in _lookupKeys)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                sum += value;
            }
        }

        return sum;
    }

    private static KeyValuePair<string, int>[] CreateItems(int count) => Enumerable
        .Range(0, count)
        .Select(i => KeyValuePair.Create($"OrderStatus:{i:D5}", i))
        .ToArray();

    private static void Shuffle<T>(T[] items)
    {
        var random = new Random(42);
        for (var i = items.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (items[i], items[j]) = (items[j], items[i]);
        }
    }
}

[MemoryDiagnoser]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[HideColumns("Job", "Error", "StdDev", "Median", "RatioSD")]
public class ConstructionBenchmarks
{
    private KeyValuePair<string, int>[] _items = [];

    [Params(10_000)]
    public int Count { get; set; }

    [GlobalSetup]
    public void Setup() => _items = Enumerable
        .Range(0, Count)
        .Select(i => KeyValuePair.Create($"OrderStatus:{i:D5}", i))
        .ToArray();

    [Benchmark(Baseline = true)]
    public Dictionary<string, int> BuildDictionary() => new(_items, StringComparer.Ordinal);

    [Benchmark]
    public ReadOnlyDictionary<string, int> BuildReadOnlyDictionary() => new(new Dictionary<string, int>(_items, StringComparer.Ordinal));

    [Benchmark]
    public ImmutableDictionary<string, int> BuildImmutableDictionary() => _items.ToImmutableDictionary(x => x.Key, x => x.Value, StringComparer.Ordinal);

    [Benchmark]
    public FrozenDictionary<string, int> BuildFrozenDictionary() => _items.ToFrozenDictionary(x => x.Key, x => x.Value, StringComparer.Ordinal);

    [Benchmark]
    public ConcurrentDictionary<string, int> BuildConcurrentDictionary() => new(_items, StringComparer.Ordinal);
}

[MemoryDiagnoser]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[HideColumns("Job", "Error", "StdDev", "Median", "RatioSD")]
public class VersioningBenchmarks
{
    private Dictionary<string, int> _dictionary = null!;
    private ImmutableDictionary<string, int> _immutableDictionary = null!;
    private FrozenDictionary<string, int> _frozenDictionary = null!;
    private string _newKey = string.Empty;

    [Params(10_000)]
    public int Count { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var items = Enumerable
            .Range(0, Count)
            .Select(i => KeyValuePair.Create($"OrderStatus:{i:D5}", i))
            .ToArray();

        _dictionary = new Dictionary<string, int>(items, StringComparer.Ordinal);
        _immutableDictionary = items.ToImmutableDictionary(x => x.Key, x => x.Value, StringComparer.Ordinal);
        _frozenDictionary = items.ToFrozenDictionary(x => x.Key, x => x.Value, StringComparer.Ordinal);
        _newKey = $"OrderStatus:{Count:D5}";
    }

    [Benchmark(Baseline = true)]
    public Dictionary<string, int> Dictionary_CopyAndAdd()
    {
        var copy = new Dictionary<string, int>(_dictionary, StringComparer.Ordinal)
        {
            [_newKey] = Count
        };

        return copy;
    }

    [Benchmark]
    public ImmutableDictionary<string, int> ImmutableDictionary_Add() => _immutableDictionary.Add(_newKey, Count);

    [Benchmark]
    public FrozenDictionary<string, int> FrozenDictionary_RebuildAndAdd()
    {
        var copy = new Dictionary<string, int>(_frozenDictionary, StringComparer.Ordinal)
        {
            [_newKey] = Count
        };

        return copy.ToFrozenDictionary(StringComparer.Ordinal);
    }
}

[MemoryDiagnoser]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[HideColumns("Job", "Error", "StdDev", "Median", "RatioSD")]
public class MergeBenchmarks
{
    private Dictionary<string, int>[] _dictionaries = [];

    [Params(10_000)]
    public int Count { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _dictionaries = Enumerable.Range(0, 4)
            .Select(partition => Enumerable.Range(partition * Count / 4, Count / 4)
                .ToDictionary(i => $"OrderStatus:{i:D5}", i => i, StringComparer.Ordinal))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public Dictionary<string, int> ForEach_TryAdd_FirstWins()
    {
        var merged = new Dictionary<string, int>(Count, StringComparer.Ordinal);
        foreach (var dictionary in _dictionaries)
        {
            foreach (var item in dictionary)
            {
                merged.TryAdd(item.Key, item.Value);
            }
        }

        return merged;
    }

    [Benchmark]
    public Dictionary<string, int> ForEach_Indexer_LastWins()
    {
        var merged = new Dictionary<string, int>(Count, StringComparer.Ordinal);
        foreach (var dictionary in _dictionaries)
        {
            foreach (var item in dictionary)
            {
                merged[item.Key] = item.Value;
            }
        }

        return merged;
    }

    [Benchmark]
    public Dictionary<string, int> Linq_Concat_ToDictionary() => _dictionaries
        .SelectMany(dictionary => dictionary)
        .ToDictionary(item => item.Key, item => item.Value, StringComparer.Ordinal);

    [Benchmark]
    public Dictionary<string, int> Linq_GroupBy_FirstWins() => _dictionaries
        .SelectMany(dictionary => dictionary)
        .GroupBy(item => item.Key, StringComparer.Ordinal)
        .ToDictionary(group => group.Key, group => group.First().Value, StringComparer.Ordinal);
}
