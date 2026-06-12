---
layout: post
title: 'Transitive Dependencies Gone Wild: Why Your .NET Microservice Isn’t Running What You Think'
date: 2025-01-27 20:28:00
permalink: /transitive-dependencies-gone-wild-why-your-net-microservice-isnt-running-what-you-think/
---

While working in a fairly large .NET microservice landscape I came across the following assumption-shattering situation, which led me down a nuget-versioning rabbit hole.

## The Pledge

We operate in a microservices landscape, consider the following ones:

-   Our microservice (we'll call it the **Registry microservice**), a ports-and-adapters architecture.
-   Another one (we'll call it the **Market microservice**), which our has a dependency on.

As part of the governance:

-   All services communicate over REST with each other;
-   Instead of every team having to implement a REST client on their own, every team that owns a microservice also publishes a 'client nuget' that other teams can directly import which abstracts away the REST stuff. For example:
    -   we operate the Registry microservice and publish the `Registry.Client` nuget on the feed
    -   the Market microservice team operates it and publishes the `Market.Client` nuget on the feed
-   For most cross-cutting concerns there are standardized building blocks in the feed, eg. `Blocks.Authentication`, `Blocks.Exceptions`, ...

Personally I think this setup is brilliant and a lot better than everyone implementing their own REST clients; but this introduces some coupling, which is where the dragon is already lurking.

To complete the context, we all rely on `Blocks.Exceptions` v1. The situation is as follows:

![](/assets/posts/transitive-dependencies-gone-wild-why-your-net-microservice-isnt-running-what-you-think/image-2-1024x564.png)

## The Turn

The central team which owns the `Blocks.Exception` package decides to add an Exception type that is widely requested, the `AppNotFoundException`, signalling that a microservice dependency is down, and they bump their version to v1.1.

The Market Team is quick to adopt the new package, and they bump their nuget version as well to v3.3.

The situation is now as follows, note the red text:

![](/assets/posts/transitive-dependencies-gone-wild-why-your-net-microservice-isnt-running-what-you-think/image-4-1024x564.png)

## The Prestige

Now pause and think (apart from the social queue of me asking the question): if we compile and deploy our artifact to production, what `Blocks.Exception` version is running in production?

![](/assets/posts/transitive-dependencies-gone-wild-why-your-net-microservice-isnt-running-what-you-think/image-5.png)

Is our Registry microservice code still using v1 while the Market adapter uses v1.1? This is what I thought as well.

Check your `\bin\Release` folder. There is only one `Blocks.Exceptions.dll` and it's v1.1.

![](/assets/posts/transitive-dependencies-gone-wild-why-your-net-microservice-isnt-running-what-you-think/image-8-1024x484.png)

*Independence Day - Alien ship not destroyed by the nuclear bomb*

> A downstream team can, benignly or maliciously (!), control which nuget version you are deploying.

## Why is this happening?

If you can stomach it, read the relevant docs page: [NuGet Package Dependency Resolution | Microsoft Learn](https://learn.microsoft.com/en-us/nuget/concepts/dependency-resolution).

NuGet’s dependency resolution rules dictates which package versions end up in production, even when teams don’t explicitly opt into upgrades. NuGet prioritizes backward-compatible "lowest applicable" versions across all dependencies. When the Market team updated their client to require `Blocks.Exceptions v1.1`, NuGet saw this as a compatible upgrade (thanks to semantic versioning’s promise of non-breaking minor versions) and auto-resolved it for the Registry service, overriding its direct `v1.0` reference. The result? A single `v1.1` DLL in the build output, forced by a transitive dependency. This behavior stems from .NET’s inability to load multiple versions of the same assembly at runtime, combined with NuGet’s assumption that minor version bumps are safe. While efficient for monolithic apps, this creates hidden coupling in microservices: a downstream team’s dependency update can unintentionally dictate what code *your* service runs, eroding autonomy and introducing deployment risks.

In detail:

1.  **Direct vs. Transitive Dependencies**:
    -   The Registry microservice directly references `Blocks.Exceptions v1.0` (e.g., `<PackageReference Include="Blocks.Exceptions" Version="1.0" />`).
    -   The Market.Client (a direct dependency of the Registry) now requires `Blocks.Exceptions >= v1.1`.
2.  **NuGet’s Resolution Logic**:
    -   NuGet merges all version constraints across the dependency graph.
    -   The Registry’s direct `v1.0` constraint implicitly means `>= v1.0` unless pinned to an exact version (e.g., `[1.0]`).
    -   The Market.Client’s `>= v1.1` constraint requires a version **equal to or higher than 1.1**.
3.  **"Lowest Applicable Version" in Action**:
    -   NuGet selects the **lowest version that satisfies all constraints**.
    -   `v1.1` is the lowest version that meets both `>= v1.0` (Registry) and `>= v1.1` (Market.Client).
4.  **Why No Conflict?**:
    -   If the Registry had pinned `Blocks.Exceptions` to an **exact version** (e.g., `[1.0]`), NuGet would raise an error due to incompatible constraints (`1.0` vs. `>=1.1`).
    -   Since the Registry’s dependency was likely a minimum version (`1.0`), NuGet treats it as `>=1.0`, allowing `v1.1` to satisfy both requirements.

## Conclusion

While this was - to me - unexpected, this was a deliberate design decision of the nuget team, which - most of the time - works as expected.

It is not without flaws, as Jon Skeet has already pointed out in a blog post ([Versioning limitations in .NET | Jon Skeet's coding blog](https://codeblog.jonskeet.uk/2019/06/30/versioning-limitations-in-net/)) and has recommended improvements ([Options for .NET’s versioning issues | Jon Skeet's coding blog](https://codeblog.jonskeet.uk/2019/10/25/options-for-nets-versioning-issues/)).

For now, being aware of the intricacies is the best recommendation.
