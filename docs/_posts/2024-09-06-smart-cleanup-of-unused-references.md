---
layout: post
title: 'Smart cleanup of unused references'
date: 2024-09-06 07:08:00
permalink: /smart-cleanup-of-unused-references/
---

<p>Say you have a 'large project' (think: clean architecture) with 40+ projects that has gradually evolved over time, has been refactored etc. There may be unused references lying around.</p>

<!--more-->

<h2 class="wp-block-heading">How do we get unused references?</h2>

<p>There are many ways this happens accidentally, for example:</p>

<p>In the beginning, there was a Test project, which references Moq (a popular mocking library):</p>

<figure class="wp-block-image aligncenter size-large"><img src="/wp-content/uploads/2024/09/image-3.png" alt="" class="wp-image-197"/></figure>

<p>Say we decide to split the projects up. The 2nd Test project also requires a dependency on Moq, but also uses a base class in Test. We now have this situation:</p>

<figure class="wp-block-image aligncenter size-large"><img src="/wp-content/uploads/2024/09/image-4.png" alt="" class="wp-image-199"/></figure>

<p>Now, say that we move all mocks to OtherTests but we forget to remove the Moq nuget from Test. It s still referenced, but no longer used:</p>

<figure class="wp-block-image aligncenter size-large"><img src="/wp-content/uploads/2024/09/image-5.png" alt="" class="wp-image-201"/></figure>

<h2 class="wp-block-heading">Remove unused references - The naive Way</h2>

<p>Visual Studio has a feature <a href="https://learn.microsoft.com/en-us/visualstudio/ide/reference/remove-unused-references?view=vs-2022">to Remove Unused References</a>:</p>

<figure class="wp-block-image size-large"><img src="/wp-content/uploads/2024/09/image-2.png" alt="" class="wp-image-195"/></figure>

<p>Since transitive dependencies are a thing, the order in which you do this is important.</p>

<h2 class="wp-block-heading">Remove unused references - The easy way</h2>

<p>From <a href="https://guiferreira.me/archive/2022/finding-dotnet-transitive-dependencies-and-tidying-up-your-project/">Finding .NET Transitive Dependencies and Tidying Up Your Project</a>, use Snitch: <a href="https://github.com/spectresystems/snitch">spectresystems/snitch: A tool that help you find duplicate transitive package references.</a></p>
