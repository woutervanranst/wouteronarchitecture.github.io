---
layout: post
title: 'Optimal usage of Global Usings'
date: 2024-09-04 07:03:00
permalink: /optimal-usage-of-global-usings/
---

<p>I have a solution with ~40 projects, built during many years with wild usings that I wanted to clean up. The <a href="https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-10#global-using-directives"><code>global usings</code> feature</a>, introduced in with C# 10.0 and .NET 6 (nov21) to simplify and reduce the repetition of commonly used namespaces throughout a project.</p>

<p>The question I have is, how do I 'get' the most commonly used namespaces and how do I do that <em>at scale</em>.</p>

<!--more-->

<h2 class="wp-block-heading">Getting an idea of what is 'used'</h2>

<p>This (LINQPad script) gives you an idea of the most commonly used ones, <em>across your solution</em>.</p>

<pre class="wp-block-syntaxhighlighter-code alignwide">var solutionPath = @"path-to-solution";
var files = Directory.EnumerateFiles(solutionPath, "*.cs", SearchOption.AllDirectories);

var usingStatements = files
	.SelectMany(file => File.ReadLines(file)
		.Where(line => line.TrimStart().StartsWith("using "))
		.Select(line => line.Trim()))
	.GroupBy(usingLine => usingLine)
	.Select(group => new { Using = group.Key, Count = group.Count() })
	.OrderByDescending(x => x.Count);

usingStatements.Dump();</pre>

<p>You will not be surprised with the results:</p>

<figure class="wp-block-image size-large"><img src="/wp-content/uploads/2024/09/image.png" alt="" class="wp-image-188"/></figure>

<h2 class="wp-block-heading">Generating GlobalUsings.cs</h2>

<p>How do I now add a GlobalUsings.cs file to every one of these 40 projects, with the relevant usings?</p>

<ol class="wp-block-list">
<li>I define a variable <code>commonUsings</code> which has the most reasonable candidates.</li>

<li>I scan every project directory and tally up the usings.</li>

<li>The intersection between #1 and #2 I write to a GlobalUsings.cs file</li>

<li>Done</li>
</ol>

<pre class="wp-block-syntaxhighlighter-code alignwide">var solutionPath = @"path-to-solution";
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
}</pre>

<p>Enjoy!</p>

<figure class="wp-block-image aligncenter size-large"><img src="/wp-content/uploads/2024/09/image-1.png" alt="" class="wp-image-192"/></figure>
