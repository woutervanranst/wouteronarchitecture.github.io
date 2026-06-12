---
layout: post
title: 'Future-proofing Result<T> Libraries'
date: 2025-02-08 14:20:57
permalink: /resultt-libraries/
---

<p>C# is coming 'soon' with <strong>Type Unions</strong> (see <a href="https://github.com/dotnet/csharplang/blob/main/proposals/TypeUnions.md">the official proposal</a> and <a href="https://www.youtube.com/watch?v=aksjZkCbIWA&amp;ab_channel=NickChapsas">Nick's video on this</a>), which I think is great. However it's not there yet and if you need it now it's important t consider which library will require the least refactoring when this feature becomes part of the official language specification.</p>

<p>This post explores some of the most popular libraries in .NET for implementing this pattern and helps you decide which one fits your project's needs.</p>


<p>You can find all the code snippets in this repository: <a href="https://github.com/woutervanranst/ResultLibraries">woutervanranst/ResultLibraries</a></p>

<h2>Target Syntax</h2>

<p>As an example use case, imagine a method that returns either a Success or an Error (a typical use case in MediatR) - and we <a href="https://andrewlock.net/working-with-the-result-pattern-part-1-replacing-exceptions-as-control-flow/#using-exceptions-for-flow-control">don't want to rely on throwing an Exception for flow control</a>.</p>

<p>This would be the (hypothetical) C# code once type unions are implemented in the language</p>

<pre>public record Success(string Message);
public record Error(string Message);

// Future C# (hypothetical)
union Result { Success; Error; }
Result result = ...
string message = result switch { Success s => s.Message, Error e => e.Message };</pre>

<h2><strong>1. FluentResults</strong> (<a href="https://github.com/altmann/FluentResults">GitHub</a>)</h2>

<p><strong>Refactoring Effort</strong>: ⭐⭐⭐ Medium</p>

<p>FluentResults is not really a type union, rather a library for the <a href="https://andrewlock.net/working-with-the-result-pattern-part-4-is-the-result-pattern-worth-it/">Result pattern</a>, ideal for CQRS workflows. Depending on your use case, this may be 'good enough'. However, it will require a significant refactor down the line.</p>

<h4><strong>Syntax</strong></h4>

<pre>using FluentResults;

Result&lt;string> result = Result.Ok("Hello World"); // Has a built-in Result type
Result&lt;string> failureResult = Result.Fail("Something went wrong");

var message = result.IsSuccess
    ? $"Success: {result.Value}"
    : $"Error: {string.Join(", ", result.Errors)}";

Console.WriteLine(message);</pre>

<h4><strong>Pros</strong></h4>

<ul>
<li><strong>Specialized for Result Handling</strong>: Offers <code>Result&lt;T&gt;</code>, <code>Result</code>, and <code>ResultBase</code> with rich error metadata and supports error chaining, nested errors, and success/error message aggregation.</li>

<li>Minimal boilerplate (built-in Result type) and easy adoption in existing codebases.</li>
</ul>

<h4><strong>Cons</strong></h4>

<ul>
<li>Not really a type union, so will require an more extensive refactor.</li>

<li>Not for other type unions (eg. BillingAmount = kWh | m3) </li>
</ul>

<hr />

<h2>2<strong>. CSharpFunctionalExtensions</strong> (<a href="https://github.com/vkhorikov/CSharpFunctionalExtensions">GitHub</a>)</h2>

<p><strong>Refactoring Effort</strong>: ⭐⭐⭐ Medium</p>

<h4><strong>Overview</strong></h4>

<p>Like FluentResults (not really a type union) also provides a <code>Maybe&lt;T></code> construct (~~ explicit nullability for reference types) and some other functional-programming-inspired constructs.</p>

<h4><strong>Syntax</strong></h4>

<pre>using CSharpFunctionalExtensions;

Result&lt;string> result = Result.Success("Hello World"); // Has a built-in Result type
Result&lt;string> failureResult = Result.Failure&lt;string>("Something went wrong");

string message = result.IsSuccess
    ? $"Success: {result.Value}"
    : $"Error: {result.Error}";

Console.WriteLine(message);</pre>

<h4><strong>Pros</strong></h4>

<ul>
<li>Simple API for basic success/failure.</li>
</ul>

<h4><strong>Cons</strong></h4>

<ul>
<li>Not really a type union, so will require an more extensive refactor.</li>

<li>Not for other type unions (eg. BillingAmount = kWh | m3)</li>
</ul>

<hr />

<h2><strong>3. OneOf</strong> (<a href="https://github.com/mcintyre321/OneOf">GitHub</a>)</h2>

<p><strong>Refactoring Effort</strong>: ⭐ Low</p>

<p>Truly models discriminated unions using generics (<code>OneOf&lt;T1, T2></code>), aligning directly with C#’s proposed native union syntax.</p>

<h4><strong>Syntax</strong></h4>

<pre>using OneOf;

OneOf&lt;Success, Error> result = OneOf&lt;Success, Error>.FromT0(new Success("Hello World"));
OneOf&lt;Success, Error> failureResult = OneOf&lt;Success, Error>.FromT1(new Error("Something went wrong"));

string message = result.Match(
    success => $"Success: {success.Message}",
    error => $"Error: {error.Message}"
);
Console.WriteLine(message);

public record Success(string Message);
public record Error(string Message);</pre>

<h4><strong>Pros</strong></h4>

<ul>
<li>Minimal code changes required.</li>

<li>Enforces exhaustive case handling via <code>Switch()</code>/<code>Match()</code>.</li>
</ul>

<hr />

<h2>4<strong>. LanguageExt</strong> (<a href="https://github.com/louthy/language-ext">GitHub</a>)</h2>

<p><strong>Refactoring Effort</strong>: ⭐ Low</p>

<p>Like OneOf (truly models discriminated unions) but also provides more functional-programming-inspired constructs (<code>Option</code>, <code>Try</code>, ...).</p>

<h4><strong>Syntax</strong></h4>

<pre>using LanguageExt;

Either&lt;Success, Error> result = new Success("Hello World");
Either&lt;Success, Error> failureResult = new Error("Something went wrong");

string message = result.Match(
    Left: msg => $"Success: {msg}",
    Right: err => $"Error: {err}"
);
Console.WriteLine(message);

public record Success(string Message);
public record Error(string Message);</pre>

<h4><strong>Pros</strong></h4>

<ul>
<li>Minimal code changes required.</li>

<li>Enforces exhaustive case handling via <code>Switch()</code>/<code>Match()</code>.</li>

<li>Upramp to other functional programming concepts such as <code>Option</code> and <code>Try</code>.</li>
</ul>

<h4><strong>Cons</strong></h4>

<ul>
<li>Likely overkill if you<em> only</em> want discriminated unions.</li>

<li>Steep learning curve if you're not familiar with functional programming.</li>
</ul>

<hr />

<h2><strong>5. Custom Result Types</strong></h2>

<p><strong>Refactoring Effort</strong>: ⭐⭐⭐⭐ High</p>

<h4><strong>Overview</strong></h4>

<p>You may opt to roll your own <code>Result&lt;T></code> record. This gives you maximum flexibility, but you are reinventing the wheel here and risk a significant refactor down the line.</p>

<h4><strong>Syntax</strong></h4>

<pre>public record Result&lt;T>(bool IsSuccess, T? Data, string? Error);

public class Command : IRequest&lt;Result&lt;string>> { }

public class Handler : IRequestHandler&lt;Command, Result&lt;string>>
{
    public Task&lt;Result&lt;string>> Handle(Command command, CancellationToken token)
        => Task.FromResult(new Result&lt;string>(true, "Done"));
}</pre>

<h4><strong>Pros</strong></h4>

<ul>
<li>Highly customizable.</li>
</ul>

<h4><strong>Cons</strong></h4>

<ul>
<li>No built-in union features or pattern matching.</li>
</ul>

<hr />

<h2>Community Adoption</h2>

<p>Looking at community adoption, OneOf is the winner.</p>

<figure><img src="/wp-content/uploads/2025/02/image-1-1024x557.png" alt=""/><figcaption><a href="https://nugettrends.com/packages?months=72&amp;ids=OneOf&amp;ids=SuccincT&amp;ids=LanguageExt.Core&amp;ids=FluentResults&amp;ids=CSharpFunctionalExtensions">NuGet Trends</a>.</figcaption></figure>

<h2>Summary</h2>

<figure><table><thead><tr><th>Library</th><th>Learning Curve</th><th>Refactoring Effort</th><th>Key Strengths</th><th>Community Adoption</th></tr></thead><tbody><tr><td><strong>FluentResults</strong></td><td>⭐⭐⭐⭐ Lowest</td><td>⭐⭐⭐</td><td>Simple Result&lt;T> Library</td><td>#4</td></tr><tr><td><strong>CSharpFunctionalExtensions</strong></td><td>⭐⭐⭐</td><td>⭐⭐⭐</td><td>Simple Result&lt;T> Library</td><td>#3</td></tr><tr><td><strong>OneOf</strong></td><td>⭐⭐</td><td>⭐ Lowest</td><td>Future proof Result&lt;T></td><td>#1</td></tr><tr><td><strong>LanguageExt</strong></td><td>⭐ Highest</td><td>⭐ Lowest</td><td>Best for functional programming</td><td>#2</td></tr><tr><td><strong>Custom Result Type</strong></td><td></td><td>⭐⭐⭐⭐ Highest</td><td>YMMV ;)</td><td>N/A</td></tr></tbody></table></figure>

<p>If you have a crystal ball and can predict your future use cases, this is your decision tree:</p>

<figure><table><tbody><tr><td></td><td><strong>Not aligned with future <strong>C# </strong>type union</strong></td><td><strong style="font-weight: bold;">Aligned with future <strong>C# </strong>type union</strong></td></tr><tr><td><strong>Use Case = Result Pattern only</strong></td><td>FluentResult</td><td>OneOf</td></tr><tr><td><strong>Use Case = Functional Programming</strong></td><td>CSharpFunctionExtension</td><td>LanguageExt</td></tr></tbody></table></figure>

<p>FluentResult is likely the solution for your immediate need, and LanguageExt is probably overkill if you are reading this (and I assume you are new to FP). In combination with the #1 spot on the community adoption, <strong>OneOf </strong>probably strikes the best balance.</p>

