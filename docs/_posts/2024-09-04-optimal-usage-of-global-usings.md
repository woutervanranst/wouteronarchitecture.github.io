---
layout: post
title: 'Optimal usage of Global Usings'
date: 2024-09-04 07:03:00
permalink: /optimal-usage-of-global-usings/
---

I have a solution with ~40 projects, built during many years with wild usings that I wanted to clean up. The [`global usings` feature](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-10#global-using-directives), introduced in with C# 10.0 and .NET 6 (nov21) to simplify and reduce the repetition of commonly used namespaces throughout a project.

The question I have is, how do I 'get' the most commonly used namespaces and how do I do that *at scale*.

## Getting an idea of what is 'used'

This (LINQPad script) gives you an idea of the most commonly used ones, *across your solution*.

```csharp
var solutionPath = @"path-to-solution";
var files = Directory.EnumerateFiles(solutionPath, "*.cs", SearchOption.AllDirectories);

var usingStatements = files
	.SelectMany(file => File.ReadLines(file)
		.Where(line => line.TrimStart().StartsWith("using "))
		.Select(line => line.Trim()))
	.GroupBy(usingLine => usingLine)
	.Select(group => new { Using = group.Key, Count = group.Count() })
	.OrderByDescending(x => x.Count);

usingStatements.Dump();
```

You will not be surprised with the results:

![](/wp-content/uploads/2024/09/image.png)

## Generating GlobalUsings.cs

How do I now add a GlobalUsings.cs file to every one of these 40 projects, with the relevant usings?

1.  I define a variable `commonUsings` which has the most reasonable candidates.
2.  I scan every project directory and tally up the usings.
3.  The intersection between #1 and #2 I write to a GlobalUsings.cs file
4.  Done

```csharp
var solutionPath = @"path-to-solution";
var globalUsingsFileName = "GlobalUsings.cs";

// Define the list of namespaces to intersect with (System, System.Linq)
var commonUsings = new[] { "using System;", "using System.Linq;", "using FluentAssertions;", "using Moq;", "using Xunit;", "using System.Collections.Generic;" };

// Find all directories with a `.csproj` file, assuming each project has one.
var projectDirectories = Directory.EnumerateFiles(solutionPath, "*.csproj", SearchOption.AllDirectories)
    .Select(csprojPath => Path.GetDirectoryName(csprojPath))
    .Distinct(); // Get unique project directories

foreach (var projectDirectory in projectDirectories)
{
    // Get all C# files within the project directory
    var csFiles = Directory.EnumerateFiles(projectDirectory, "*.cs", SearchOption.AllDirectories);

    var usingStatements = csFiles
        .SelectMany(file => File.ReadLines(file)
            .Where(line => line.TrimStart().StartsWith("using "))
            .Select(line => line.Trim()))
        .GroupBy(usingLine => usingLine)
        .Select(group => new { Using = group.Key, Count = group.Count() })
        .OrderByDescending(x => x.Count)
		.Where(u => u.Count > 1);

    // Output the results for each project
    Console.WriteLine($"Project: {Path.GetFileName(projectDirectory)}");
    usingStatements.Dump();

    // Find intersection of most used usings with the specified common usings
    var intersectingUsings = usingStatements
        .Select(u => u.Using)
        .Intersect(commonUsings)
        .ToList();

    if (intersectingUsings.Any())
    {
        // Prepare the path for the GlobalUsings.cs file
        var globalUsingsFilePath = Path.Combine(projectDirectory, globalUsingsFileName);

		// Create or overwrite the GlobalUsings.cs file with the intersecting using statements
		var globalUsingsContent = intersectingUsings
			.Select(usingDirective => $"global {usingDirective}") // Convert to global using
			.Aggregate((current, next) => current + Environment.NewLine + next); // Combine into one string

		File.WriteAllText(globalUsingsFilePath, globalUsingsContent);

		Console.WriteLine($"GlobalUsings.cs file created/updated for project {Path.GetFileName(projectDirectory)} with the following global usings:");
		intersectingUsings.ForEach(Console.WriteLine);
	}
}
```

Enjoy!

![](/wp-content/uploads/2024/09/image-1.png)
