Title: Optimal usage of Global Usings

URL Source: http://wouteronarchitecture.medium.com/optimal-usage-of-global-usings-fe73552b5bcc

Published Time: 2024-09-04T06:03:43Z

Markdown Content:
[![Image 1: Wouter on Architecture](https://miro.medium.com/v2/resize:fill:32:32/1*AoLeduUi-W4NsYodQ8ERMA.jpeg)](https://wouteronarchitecture.medium.com/?source=post_page---byline--fe73552b5bcc---------------------------------------)

3 min read

Sep 4, 2024

I have a solution with ~40 projects, built during many years with wild usings that I wanted to clean up. The `global usings`[feature](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-10#global-using-directives), introduced in with C# 10.0 and .NET 6 (nov21) to simplify and reduce the repetition of commonly used namespaces throughout a project.

## Get Wouter on Architecture’s stories in your inbox

Join Medium for free to get updates from this writer.

Remember me for faster sign in

The question I have is, how do I ‘get’ the most commonly used namespaces and how do I do that _at scale_.

## Getting an idea of what is ‘used’

This (LINQPad script) gives you an idea of the most commonly used ones, _across your solution_.

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

You will not be surprised with the results:

Press enter or click to view image in full size

![Image 2](https://miro.medium.com/v2/resize:fit:700/0*gnn_44SfMShETdMr)

## Generating GlobalUsings.cs

How do I now add a GlobalUsings.cs file to every one of these 40 projects, with the relevant usings?

1.   I define a variable `commonUsings` which has the most reasonable candidates.
2.   I scan every project directory and tally up the usings.
3.   The intersection between #1 and #2 I write to a GlobalUsings.cs file
4.   Done

var solutionPath = @"path-to-solution";

var globalUsingsFileName = "GlobalUsings.cs";
var commonUsings = new[] { "using System;", "using System.Linq;", "using FluentAssertions;", "using Moq;", "using Xunit;", "using System.Collections.Generic;" };

var projectDirectories = Directory.EnumerateFiles(solutionPath, "*.csproj", SearchOption.AllDirectories)

 .Select(csprojPath => Path.GetDirectoryName(csprojPath))

 .Distinct();

foreach (var projectDirectory in projectDirectories)

{

 

 var csFiles = Directory.EnumerateFiles(projectDirectory, "*.cs", SearchOption.AllDirectories);

var usingStatements = csFiles

 .SelectMany(file => File.ReadLines(file)

 .Where(line => line.TrimStart().StartsWith("using "))

 .Select(line => line.Trim()))

 .GroupBy(usingLine => usingLine)

 .Select(group => new { Using = group.Key, Count = group.Count() })

 .OrderByDescending(x => x.Count)

 .Where(u => u.Count > 1);

Console.WriteLine($"Project: {Path.GetFileName(projectDirectory)}");

 usingStatements.Dump();

var intersectingUsings = usingStatements

 .Select(u => u.Using)

 .Intersect(commonUsings)

 .ToList();

if (intersectingUsings.Any())

 {

 

 var globalUsingsFilePath = Path.Combine(projectDirectory, globalUsingsFileName);

var globalUsingsContent = intersectingUsings

 .Select(usingDirective => $"global {usingDirective}") 

 .Aggregate((current, next) => current + Environment.NewLine + next);

File.WriteAllText(globalUsingsFilePath, globalUsingsContent);

Console.WriteLine($"GlobalUsings.cs file created/updated for project {Path.GetFileName(projectDirectory)} with the following global usings:");

 intersectingUsings.ForEach(Console.WriteLine);

 }

}

Enjoy!

![Image 3](https://miro.medium.com/v2/resize:fit:144/0*6FyNSGDMP1LRmmsE)
