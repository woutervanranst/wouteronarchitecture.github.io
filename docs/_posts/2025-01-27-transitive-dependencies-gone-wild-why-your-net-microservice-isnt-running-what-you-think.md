---
layout: post
title: 'Transitive Dependencies Gone Wild: Why Your .NET Microservice Isn’t Running What You Think'
date: 2025-01-27 20:28:00
permalink: /transitive-dependencies-gone-wild-why-your-net-microservice-isnt-running-what-you-think/
---

<p>While working in a fairly large .NET microservice landscape I came across the following assumption-shattering situation, which led me down a nuget-versioning rabbit hole.</p>

<!--more-->

<h2 class="wp-block-heading">The Pledge</h2>

<p>We operate in a microservices landscape, consider the following ones:</p>

<ul class="wp-block-list">
<li>Our microservice (we'll call it the <strong>Registry microservice</strong>), a ports-and-adapters architecture.</li>

<li>Another one (we'll call it the <strong>Market microservice</strong>), which our has a dependency on.</li>
</ul>

<p>As part of the governance:</p>

<ul class="wp-block-list">
<li>All services communicate over REST with each other;</li>

<li>Instead of every team having to implement a REST client on their own, every team that owns a microservice also publishes a 'client nuget' that other teams can directly import which abstracts away the REST stuff. For example: 
<ul class="wp-block-list">
<li>we operate the Registry microservice and publish the <code>Registry.Client</code> nuget on the feed</li>

<li>the Market microservice team operates it and publishes the <code>Market.Client</code> nuget on the feed</li>
</ul>
</li>

<li>For most cross-cutting concerns there are standardized building blocks in the feed, eg. <code>Blocks.Authentication</code>, <code>Blocks.Exceptions</code>, ...</li>
</ul>

<p>Personally I think this setup is brilliant and a lot better than everyone implementing their own REST clients; but this introduces some coupling, which is where the dragon is already lurking.</p>

<p>To complete the context, we all rely on <code>Blocks.Exceptions</code> v1. The situation is as follows:</p>

<figure class="wp-block-image size-large"><img src="/wp-content/uploads/2025/01/image-2-1024x564.png" alt="" class="wp-image-103"/></figure>

<h2 class="wp-block-heading">The Turn</h2>

<p>The central team which owns the <code>Blocks.Exception</code> package decides to add an Exception type that is widely requested, the <code>AppNotFoundException</code>, signalling that a microservice dependency is down, and they bump their version to v1.1.</p>

<p>The Market Team is quick to adopt the new package, and they bump their nuget version as well to v3.3.</p>

<p>The situation is now as follows, note the red text:</p>

<figure class="wp-block-image aligncenter size-large"><img src="/wp-content/uploads/2025/01/image-4-1024x564.png" alt="" class="wp-image-105"/></figure>

<h2 class="wp-block-heading">The Prestige</h2>

<p>Now pause and think (apart from the social queue of me asking the question): if we compile and deploy our artifact to production, what <code>Blocks.Exception</code> version is running in production?</p>

<figure class="wp-block-image aligncenter size-full"><img src="/wp-content/uploads/2025/01/image-5.png" alt="" class="wp-image-107"/></figure>

<p> Is our Registry microservice code still using v1 while the Market adapter uses v1.1? This is what I thought as well.</p>

<p>Check your <code>\bin\Release</code> folder. There is only one <code>Blocks.Exceptions.dll</code> and it's v1.1.</p>

<figure class="wp-block-image size-large"><img src="/wp-content/uploads/2025/01/image-8-1024x484.png" alt="" class="wp-image-110"/><figcaption class="wp-element-caption">Independence Day - Alien ship not destroyed by the nuclear bomb</figcaption></figure>

<blockquote class="wp-block-quote">
<p>A downstream team can, benignly or maliciously (!), control which nuget version you are deploying.</p>
</blockquote>

<h2 class="wp-block-heading">Why is this happening?</h2>

<p>If you can stomach it, read the relevant docs page: <a href="https://learn.microsoft.com/en-us/nuget/concepts/dependency-resolution">NuGet Package Dependency Resolution | Microsoft Learn</a>.</p>

<p>NuGet’s dependency resolution rules dictates which package versions end up in production, even when teams don’t explicitly opt into upgrades. NuGet prioritizes backward-compatible "lowest applicable" versions across all dependencies. When the Market team updated their client to require <code>Blocks.Exceptions v1.1</code>, NuGet saw this as a compatible upgrade (thanks to semantic versioning’s promise of non-breaking minor versions) and auto-resolved it for the Registry service, overriding its direct <code>v1.0</code> reference. The result? A single <code>v1.1</code> DLL in the build output, forced by a transitive dependency. This behavior stems from .NET’s inability to load multiple versions of the same assembly at runtime, combined with NuGet’s assumption that minor version bumps are safe. While efficient for monolithic apps, this creates hidden coupling in microservices: a downstream team’s dependency update can unintentionally dictate what code <em>your</em> service runs, eroding autonomy and introducing deployment risks.</p>

<p>In detail:</p>

<ol start="1" class="wp-block-list">
<li><strong>Direct vs. Transitive Dependencies</strong>:
<ul class="wp-block-list">
<li>The Registry microservice directly references <code>Blocks.Exceptions v1.0</code> (e.g., <code>&lt;PackageReference Include="Blocks.Exceptions" Version="1.0" /&gt;</code>).</li>

<li>The Market.Client (a direct dependency of the Registry) now requires <code>Blocks.Exceptions &gt;= v1.1</code>.</li>
</ul>
</li>

<li><strong>NuGet’s Resolution Logic</strong>:
<ul class="wp-block-list">
<li>NuGet merges all version constraints across the dependency graph.</li>

<li>The Registry’s direct <code>v1.0</code> constraint implicitly means <code>&gt;= v1.0</code> unless pinned to an exact version (e.g., <code>[1.0]</code>).</li>

<li>The Market.Client’s <code>&gt;= v1.1</code> constraint requires a version <strong>equal to or higher than 1.1</strong>.</li>
</ul>
</li>

<li><strong>"Lowest Applicable Version" in Action</strong>:
<ul class="wp-block-list">
<li>NuGet selects the <strong>lowest version that satisfies all constraints</strong>.</li>

<li><code>v1.1</code> is the lowest version that meets both <code>&gt;= v1.0</code> (Registry) and <code>&gt;= v1.1</code> (Market.Client).</li>
</ul>
</li>

<li><strong>Why No Conflict?</strong>:
<ul class="wp-block-list">
<li>If the Registry had pinned <code>Blocks.Exceptions</code> to an <strong>exact version</strong> (e.g., <code>[1.0]</code>), NuGet would raise an error due to incompatible constraints (<code>1.0</code> vs. <code>&gt;=1.1</code>).</li>

<li>Since the Registry’s dependency was likely a minimum version (<code>1.0</code>), NuGet treats it as <code>&gt;=1.0</code>, allowing <code>v1.1</code> to satisfy both requirements.</li>
</ul>
</li>
</ol>

<h2 class="wp-block-heading">Conclusion</h2>

<p>While this was - to me - unexpected, this was a deliberate design decision of the nuget team, which - most of the time - works as expected.</p>

<p>It is not without flaws, as Jon Skeet has already pointed out in a blog post (<a href="https://codeblog.jonskeet.uk/2019/06/30/versioning-limitations-in-net/">Versioning limitations in .NET | Jon Skeet's coding blog</a>) and has recommended improvements (<a href="https://codeblog.jonskeet.uk/2019/10/25/options-for-nets-versioning-issues/">Options for .NET’s versioning issues | Jon Skeet's coding blog</a>).</p>

<p>For now, being aware of the intricacies is the best recommendation.</p>
