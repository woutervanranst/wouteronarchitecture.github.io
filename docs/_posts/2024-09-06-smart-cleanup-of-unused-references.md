---
layout: post
title: 'Smart cleanup of unused references'
date: 2024-09-06 07:08:00
permalink: /smart-cleanup-of-unused-references/
---

Say you have a 'large project' (think: clean architecture) with 40+ projects that has gradually evolved over time, has been refactored etc. There may be unused references lying around.

## How do we get unused references?

There are many ways this happens accidentally, for example:

In the beginning, there was a Test project, which references Moq (a popular mocking library):

![](/wp-content/uploads/2024/09/image-3.png)

Say we decide to split the projects up. The 2nd Test project also requires a dependency on Moq, but also uses a base class in Test. We now have this situation:

![](/wp-content/uploads/2024/09/image-4.png)

Now, say that we move all mocks to OtherTests but we forget to remove the Moq nuget from Test. It s still referenced, but no longer used:

![](/wp-content/uploads/2024/09/image-5.png)

## Remove unused references - The naive Way

Visual Studio has a feature [to Remove Unused References](https://learn.microsoft.com/en-us/visualstudio/ide/reference/remove-unused-references?view=vs-2022):

![](/wp-content/uploads/2024/09/image-2.png)

Since transitive dependencies are a thing, the order in which you do this is important.

## Remove unused references - The easy way

From [Finding .NET Transitive Dependencies and Tidying Up Your Project](https://guiferreira.me/archive/2022/finding-dotnet-transitive-dependencies-and-tidying-up-your-project/), use Snitch: [spectresystems/snitch: A tool that help you find duplicate transitive package references.](https://github.com/spectresystems/snitch)
