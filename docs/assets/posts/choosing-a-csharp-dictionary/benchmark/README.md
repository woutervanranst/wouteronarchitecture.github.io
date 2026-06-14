# Dictionary Choice Benchmarks

Minimal BenchmarkDotNet project for the post "Choosing a C# Dictionary". It targets `net10.0` because that is the SDK/runtime installed on the machine used for the checked-in results; retarget to `net8.0` if you want to reproduce against the first .NET version that shipped `System.Collections.Frozen`.

Run all benchmarks:

```sh
dotnet run -c Release -- --filter "*"
```

Run one group:

```sh
dotnet run -c Release -- --filter "*Lookup*"
dotnet run -c Release -- --filter "*Construction*"
dotnet run -c Release -- --filter "*Merge*"
```

BenchmarkDotNet writes full reports to `BenchmarkDotNet.Artifacts/results/`. The checked-in `results.md` file contains the run used by the blog post.
