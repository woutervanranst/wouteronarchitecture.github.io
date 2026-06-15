# Benchmark Results

BenchmarkDotNet v0.14.0, macOS 26.5.1, Apple M4, .NET SDK 10.0.201.

Runtime: .NET 10.0.5, Arm64 RyuJIT AdvSIMD. Job: `ShortRun`, `IterationCount=3`, `LaunchCount=1`, `WarmupCount=3`.

## Lookup

Each benchmark performs 10,000 successful `TryGetValue` calls against string keys in randomized order.

| Method | Count | Mean | Ratio | Allocated |
| --- | ---: | ---: | ---: | ---: |
| FrozenDictionary_TryGetValue | 10000 | 45.38 us | 0.66 | - |
| ConcurrentDictionary_TryGetValue | 10000 | 60.81 us | 0.89 | - |
| Dictionary_TryGetValue | 10000 | 68.90 us | 1.00 | - |
| ReadOnlyDictionary_TryGetValue | 10000 | 71.48 us | 1.04 | - |
| ImmutableDictionary_TryGetValue | 10000 | 665.74 us | 9.70 | - |

## Construction

Each benchmark builds a dictionary-like collection with 10,000 string keys.

| Method | Count | Mean | Ratio | Allocated |
| --- | ---: | ---: | ---: | ---: |
| BuildDictionary | 10000 | 121.3 us | 1.00 | 276.43 KB |
| BuildReadOnlyDictionary | 10000 | 139.3 us | 1.15 | 276.47 KB |
| BuildConcurrentDictionary | 10000 | 679.8 us | 5.61 | 1022.32 KB |
| BuildFrozenDictionary | 10000 | 692.6 us | 5.71 | 1102.85 KB |
| BuildImmutableDictionary | 10000 | 1625.9 us | 13.41 | 625.26 KB |

## Producing a changed version

Each benchmark starts with 10,000 entries and produces a new dictionary-like collection with one extra key. The old collection remains usable.

| Method | Count | Mean | Ratio | Allocated | Alloc Ratio |
| --- | ---: | ---: | ---: | ---: | ---: |
| ImmutableDictionary_Add | 10000 | 202.3 ns | 0.004 | 872 B | 0.003 |
| Dictionary_CopyAndAdd | 10000 | 55.774 us | 1.005 | 283094 B | 1.000 |
| FrozenDictionary_RebuildAndAdd | 10000 | 661.273 us | 11.918 | 1129400 B | 3.989 |

## Merge

Each benchmark merges four 2,500-entry dictionaries into one 10,000-entry `Dictionary<string, int>`.

| Method | Count | Mean | Ratio | Allocated |
| --- | ---: | ---: | ---: | ---: |
| ForEach_Indexer_LastWins | 10000 | 173.6 us | 0.99 | 276.43 KB |
| ForEach_TryAdd_FirstWins | 10000 | 176.2 us | 1.01 | 276.43 KB |
| Linq_Concat_ToDictionary | 10000 | 380.9 us | 2.18 | 920.4 KB |
| Linq_GroupBy_FirstWins | 10000 | 1089.0 us | 6.23 | 2114.16 KB |
