---
layout: post
title: 'Choosing a C# Dictionary'
date: 2026-06-14 09:00:00
permalink: /choosing-a-csharp-dictionary/
subtitle: 'A crisp decision flow for Dictionary, FrozenDictionary, ImmutableDictionary, ConcurrentDictionary, ReadOnlyDictionary, and friends.'
mermaid: true
---

I recently ran into `FrozenDictionary<TKey,TValue>` and first read it as "a colder `ReadOnlyDictionary`". That's not it though.

The useful mental model is what each type is optimized for. `ImmutableDictionary<TKey,TValue>` is optimized for building changed (mutable) dictionaries efficiently. `FrozenDictionary<TKey,TValue>` is optimized for fast reads after the dictionary is built. Confusing, right?

That makes the choice more about workload than mutability vocabulary:

-   `Dictionary<TKey,TValue>` is the normal mutable map. Start here.
-   `ReadOnlyDictionary<TKey,TValue>` is an adapter around an existing dictionary. Callers will not be able to write.
-   `ImmutableDictionary<TKey,TValue>` is for code that keeps creating changed versions of a dictionary.
-   `FrozenDictionary<TKey,TValue>` is for data you finish building and then read a lot.
-   `ConcurrentDictionary<TKey,TValue>` is for shared writes from multiple threads.

## The flowchart

```mermaid
flowchart TD
    A["Need key-value lookup?"] -->|No| B["Use List, array, tuple list, or a custom type"]
    A -->|Yes| C{"Keys unique?"}
    C -->|No| D["Use Lookup, grouping, or Dictionary of lists"]
    C -->|Yes| E{"Need ordering or ranges by key?"}
    E -->|Yes| F["SortedDictionary"]
    E -->|No| G{"Shared concurrent writes?"}
    G -->|Yes| H["ConcurrentDictionary"]
    G -->|No| I{"Will contents change after build?"}
    I -->|Yes| J["Dictionary"]
    I -->|No| K{"Need to produce modified versions cheaply?"}
    K -->|Yes| L["ImmutableDictionary"]
    K -->|No| M{"Hot read path and built rarely?"}
    M -->|Yes| N["FrozenDictionary"]
    M -->|No| O{"Need to expose without allowing callers to mutate?"}
    O -->|Yes| P["IReadOnlyDictionary or ReadOnlyDictionary"]
    O -->|No| Q["Dictionary"]
```

## Criteria

| Scenario | Choose | Why |
| --- | --- | --- |
| Normal add, update, remove, lookup | `Dictionary<TKey,TValue>` | Fast, simple, idiomatic default. Set capacity if you know the size. |
| Public API should not mutate your collection | `IReadOnlyDictionary<TKey,TValue>` or `ReadOnlyDictionary<TKey,TValue>` | Communicates read-only access. Remember: a wrapper is not a deep immutable copy. |
| Many readers, shared writers | `ConcurrentDictionary<TKey,TValue>` | Built for concurrent access and atomic operations such as `AddOrUpdate`. |
| Data built once and queried heavily | `FrozenDictionary<TKey,TValue>` | Optimizes the internal representation for lookups after construction. |
| Functional/persistent update model | `ImmutableDictionary<TKey,TValue>` | `Add` and `Remove` return new dictionaries while sharing structure. |
| Sorted enumeration or range-like key work | `SortedDictionary<TKey,TValue>` | Maintains key order, trading away raw hash-map speed. |
| Legacy non-generic object keys | Avoid `Hashtable` | Prefer generic dictionaries unless you have legacy interop constraints. |
| Duplicate logical keys | Not a dictionary | Use grouping, `Lookup<TKey,TValue>`, or `Dictionary<TKey,List<TValue>>`. |

## The terms that tripped me up

`ReadOnlyDictionary<TKey,TValue>` is a wrapper class. You pass it an existing dictionary. The wrapper does not expose `Add`, `Remove`, or a settable indexer, but it still points at the dictionary you gave it.

```csharp
var source = new Dictionary<string, int>
{
    ["BE"] = 32,
    ["NL"] = 31
};

var readOnly = new ReadOnlyDictionary<string, int>(source);

source["LU"] = 352;

Console.WriteLine(readOnly["LU"]); // 352
```

That is the wrapper bit. The "other dictionary" is the `source` object. If `source` changes, the read-only view sees the change.

So `ReadOnlyDictionary` does not mean "nobody can ever change this data". It means "you cannot change it through this object".

If you only need to return something from an API, the interface is often enough:

```csharp
public IReadOnlyDictionary<string, int> CountryCodes => _countryCodes;
```

That says "you can read this" without promising that nobody inside the class will ever change `_countryCodes`.

## Immutable means two different things here

Both `ImmutableDictionary` and `FrozenDictionary` stop you from changing the object in place. That is where the similarity ends.

The word "persistent" in `ImmutableDictionary` does not mean persisted to disk. It means a data structure where updates return a new version while sharing most of the old structure. This is common terminology in functional programming, but it is easy to misread in everyday .NET code.

```csharp
var original = ImmutableDictionary<string, int>.Empty
    .Add("BE", 32)
    .Add("NL", 31);

var changed = original.Add("LU", 352);

Console.WriteLine(original.ContainsKey("LU")); // False
Console.WriteLine(changed.ContainsKey("LU"));  // True
```

That is the point of `ImmutableDictionary`: keep `original`, pass `changed`, and do not copy the whole dictionary for each change.

`FrozenDictionary` has a different job. Build it when the data is done changing.

```csharp
var frozen = new Dictionary<string, int>
{
    ["BE"] = 32,
    ["NL"] = 31,
    ["LU"] = 352
}.ToFrozenDictionary(StringComparer.Ordinal);

Console.WriteLine(frozen["NL"]);
```

There is no `Add` method returning a new `FrozenDictionary`. If the data changes, build a new one. The type spends extra work during construction so lookups can be cheaper later.

That makes sense for long-lived maps: route tables, schema metadata, code-to-handler maps, keyword maps, and lookup tables built from configuration. It makes much less sense for something you rebuild every request.

Use `FrozenDictionary` when all of these are true:

-   The data is stable after construction.
-   Reads dominate construction cost.
-   The lookup is on a hot enough path to matter.
-   You benchmarked with your key type and comparer.

Do not use it just because you want to prevent mutation. If the dictionary is small or not hot, a normal `Dictionary` exposed as `IReadOnlyDictionary` is often the clearest option.

## Benchmark

The benchmark project is here:

`/assets/posts/choosing-a-csharp-dictionary/benchmark/`

It targets `.NET 10` because that is what is installed on my machine. `FrozenDictionary` arrived in .NET 8, but these numbers are from .NET 10.0.5 on an Apple M4.

The lookup benchmark performs 10,000 successful `TryGetValue` calls against string keys in randomized order:

```text
BenchmarkDotNet v0.14.0, macOS 26.5.1, Apple M4
.NET SDK 10.0.201
Runtime=.NET 10.0.5, Arm64 RyuJIT AdvSIMD

| Method                           | Count | Mean      | Ratio | Allocated |
|--------------------------------- |------ |----------:|------:|----------:|
| FrozenDictionary_TryGetValue     | 10000 |  45.38 us |  0.66 |         - |
| ConcurrentDictionary_TryGetValue | 10000 |  60.81 us |  0.89 |         - |
| Dictionary_TryGetValue           | 10000 |  68.90 us |  1.00 |         - |
| ReadOnlyDictionary_TryGetValue   | 10000 |  71.48 us |  1.04 |         - |
| ImmutableDictionary_TryGetValue  | 10000 | 665.74 us |  9.70 |         - |
```

For this string-key scenario, `FrozenDictionary` was about one third faster than `Dictionary`. `ImmutableDictionary` was much slower for lookup. That is not a bug in `ImmutableDictionary`; it is just built for a different job.

Construction tells the other half of the story:

```text
| Method                    | Count | Mean      | Ratio | Allocated  |
|-------------------------- |------ |----------:|------:|-----------:|
| BuildDictionary           | 10000 |  121.3 us |  1.00 |  276.43 KB |
| BuildReadOnlyDictionary   | 10000 |  139.3 us |  1.15 |  276.47 KB |
| BuildConcurrentDictionary | 10000 |  679.8 us |  5.61 | 1022.32 KB |
| BuildFrozenDictionary     | 10000 |  692.6 us |  5.71 | 1102.85 KB |
| BuildImmutableDictionary  | 10000 | 1625.9 us | 13.41 |  625.26 KB |
```

In this run, building a `FrozenDictionary` was about 5.7 times slower than building a `Dictionary`. Fine for a map that lives for a while. Wasteful for a throwaway map.

## Merging dictionaries

Merging has a separate decision: define duplicate-key behavior first.

-   First value wins: use `TryAdd`.
-   Last value wins: use the indexer assignment.
-   Duplicate keys are invalid: use `Add` or `ToDictionary` and let it throw.
-   Duplicate keys must be combined: group explicitly.

The benchmark merges four 2,500-entry dictionaries into one 10,000-entry dictionary:

```text
| Method                   | Count | Mean      | Ratio | Allocated  |
|------------------------- |------ |----------:|------:|-----------:|
| ForEach_Indexer_LastWins | 10000 |  173.6 us |  0.99 |  276.43 KB |
| ForEach_TryAdd_FirstWins | 10000 |  176.2 us |  1.01 |  276.43 KB |
| Linq_Concat_ToDictionary | 10000 |  380.9 us |  2.18 |   920.4 KB |
| Linq_GroupBy_FirstWins   | 10000 | 1089.0 us |  6.23 | 2114.16 KB |
```

This matches the Code Maze result: the boring nested `foreach` loop wins. LINQ is fine for small, cold paths. For large merges or hot paths, write the loop and make the duplicate rule visible.

## Rules of thumb

-   Start with `Dictionary<TKey,TValue>`.
-   Use `TryGetValue` for lookup when a key might be absent.
-   Use `TryAdd`, `Add`, or indexer assignment based on duplicate-key semantics.
-   Use `StringComparer.Ordinal` or `OrdinalIgnoreCase` for non-linguistic string keys.
-   Pre-size dictionaries when you know the count.
-   Expose `IReadOnlyDictionary<TKey,TValue>` when callers should not mutate.
-   Reach for `FrozenDictionary<TKey,TValue>` only when stable data sits on a hot read path.
-   Reach for `ImmutableDictionary<TKey,TValue>` only when you need persistent update semantics.
-   Reach for `ConcurrentDictionary<TKey,TValue>` only when concurrent mutation is part of the design.

## Sources

-   [Dictionary in C# - Code Maze](https://code-maze.com/dictionary-csharp/)
-   [How to Merge Dictionaries in C#? - Code Maze](https://code-maze.com/csharp-how-to-merge-dictionaries/)
-   [A quick tour of dictionaries in C# - Jack Lewis](https://dev.to/jlewis92/a-quick-tour-of-dictionaries-in-c-gg9)
-   [Performance Improvements in .NET 8 - Stephen Toub](https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-8/)
-   [Frozen compared to Immutable collections in .NET 8 - r/dotnet](https://www.reddit.com/r/dotnet/comments/1eckkbu/frozen_compared_to_immutable_collections_in_net_8/)
