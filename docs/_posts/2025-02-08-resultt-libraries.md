---
layout: post
title: 'Future-proofing Result<T> Libraries'
date: 2025-02-08 14:20:57
permalink: /resultt-libraries/
subtitle: 'A practical look at today’s .NET `Result<T>` libraries through the lens of future C# type unions and eventual refactoring cost.'
---

C# is coming 'soon' with **Type Unions** (see [the official proposal](https://github.com/dotnet/csharplang/blob/main/proposals/TypeUnions.md) and [Nick's video on this](https://www.youtube.com/watch?v=aksjZkCbIWA&ab_channel=NickChapsas)), which I think is great. However it's not there yet and if you need it now it's important t consider which library will require the least refactoring when this feature becomes part of the official language specification.

This post explores some of the most popular libraries in .NET for implementing this pattern and helps you decide which one fits your project's needs.

You can find all the code snippets in this repository: [woutervanranst/ResultLibraries](https://github.com/woutervanranst/ResultLibraries)

## Target Syntax

As an example use case, imagine a method that returns either a Success or an Error (a typical use case in MediatR) - and we [don't want to rely on throwing an Exception for flow control](https://andrewlock.net/working-with-the-result-pattern-part-1-replacing-exceptions-as-control-flow/#using-exceptions-for-flow-control).

This would be the (hypothetical) C# code once type unions are implemented in the language

```csharp
public record Success(string Message);
public record Error(string Message);

// Future C# (hypothetical)
union Result { Success; Error; }
Result result = ...
string message = result switch { Success s => s.Message, Error e => e.Message };
```

## 1. FluentResults ([GitHub](https://github.com/altmann/FluentResults))

**Refactoring Effort**: ⭐⭐⭐ Medium

FluentResults is not really a type union, rather a library for the [Result pattern](https://andrewlock.net/working-with-the-result-pattern-part-4-is-the-result-pattern-worth-it/), ideal for CQRS workflows. Depending on your use case, this may be 'good enough'. However, it will require a significant refactor down the line.

#### Syntax

```csharp
using FluentResults;

Result<string> result = Result.Ok("Hello World"); // Has a built-in Result type
Result<string> failureResult = Result.Fail("Something went wrong");

var message = result.IsSuccess
    ? $"Success: {result.Value}"
    : $"Error: {string.Join(", ", result.Errors)}";

Console.WriteLine(message);
```

#### Pros

-   **Specialized for Result Handling**: Offers `Result<T>`, `Result`, and `ResultBase` with rich error metadata and supports error chaining, nested errors, and success/error message aggregation.
-   Minimal boilerplate (built-in Result type) and easy adoption in existing codebases.

#### Cons

-   Not really a type union, so will require an more extensive refactor.
-   Not for other type unions (eg. BillingAmount = kWh | m3)

---

## 2. CSharpFunctionalExtensions ([GitHub](https://github.com/vkhorikov/CSharpFunctionalExtensions))

**Refactoring Effort**: ⭐⭐⭐ Medium

#### Overview

Like FluentResults (not really a type union) also provides a `Maybe<T>` construct (~~ explicit nullability for reference types) and some other functional-programming-inspired constructs.

#### Syntax

```csharp
using CSharpFunctionalExtensions;

Result<string> result = Result.Success("Hello World"); // Has a built-in Result type
Result<string> failureResult = Result.Failure<string>("Something went wrong");

string message = result.IsSuccess
    ? $"Success: {result.Value}"
    : $"Error: {result.Error}";

Console.WriteLine(message);
```

#### Pros

-   Simple API for basic success/failure.

#### Cons

-   Not really a type union, so will require an more extensive refactor.
-   Not for other type unions (eg. BillingAmount = kWh | m3)

---

## 3. OneOf ([GitHub](https://github.com/mcintyre321/OneOf))

**Refactoring Effort**: ⭐ Low

Truly models discriminated unions using generics (`OneOf<T1, T2>`), aligning directly with C#’s proposed native union syntax.

#### Syntax

```csharp
using OneOf;

OneOf<Success, Error> result = OneOf<Success, Error>.FromT0(new Success("Hello World"));
OneOf<Success, Error> failureResult = OneOf<Success, Error>.FromT1(new Error("Something went wrong"));

string message = result.Match(
    success => $"Success: {success.Message}",
    error => $"Error: {error.Message}"
);
Console.WriteLine(message);

public record Success(string Message);
public record Error(string Message);
```

#### Pros

-   Minimal code changes required.
-   Enforces exhaustive case handling via `Switch()`/`Match()`.

---

## 4. LanguageExt ([GitHub](https://github.com/louthy/language-ext))

**Refactoring Effort**: ⭐ Low

Like OneOf (truly models discriminated unions) but also provides more functional-programming-inspired constructs (`Option`, `Try`, ...).

#### Syntax

```csharp
using LanguageExt;

Either<Success, Error> result = new Success("Hello World");
Either<Success, Error> failureResult = new Error("Something went wrong");

string message = result.Match(
    Left: msg => $"Success: {msg}",
    Right: err => $"Error: {err}"
);
Console.WriteLine(message);

public record Success(string Message);
public record Error(string Message);
```

#### Pros

-   Minimal code changes required.
-   Enforces exhaustive case handling via `Switch()`/`Match()`.
-   Upramp to other functional programming concepts such as `Option` and `Try`.

#### Cons

-   Likely overkill if you *only* want discriminated unions.
-   Steep learning curve if you're not familiar with functional programming.

---

## 5. Custom Result Types

**Refactoring Effort**: ⭐⭐⭐⭐ High

#### Overview

You may opt to roll your own `Result<T>` record. This gives you maximum flexibility, but you are reinventing the wheel here and risk a significant refactor down the line.

#### Syntax

```csharp
public record Result<T>(bool IsSuccess, T? Data, string? Error);

public class Command : IRequest<Result<string>> { }

public class Handler : IRequestHandler<Command, Result<string>>
{
    public Task<Result<string>> Handle(Command command, CancellationToken token)
        => Task.FromResult(new Result<string>(true, "Done"));
}
```

#### Pros

-   Highly customizable.

#### Cons

-   No built-in union features or pattern matching.

---

## Community Adoption

Looking at community adoption, OneOf is the winner.

![](/assets/posts/resultt-libraries/image-1-1024x557.png)

*[NuGet Trends](https://nugettrends.com/packages?months=72&ids=OneOf&ids=SuccincT&ids=LanguageExt.Core&ids=FluentResults&ids=CSharpFunctionalExtensions).*

## Summary

| Library | Learning Curve | Refactoring Effort | Key Strengths | Community Adoption |
| --- | --- | --- | --- | --- |
| **FluentResults** | ⭐⭐⭐⭐ Lowest | ⭐⭐⭐ | Simple Result<T> Library | #4 |
| **CSharpFunctionalExtensions** | ⭐⭐⭐ | ⭐⭐⭐ | Simple Result<T> Library | #3 |
| **OneOf** | ⭐⭐ | ⭐ Lowest | Future proof Result<T> | #1 |
| **LanguageExt** | ⭐ Highest | ⭐ Lowest | Best for functional programming | #2 |
| **Custom Result Type** |  | ⭐⭐⭐⭐ Highest | YMMV ;) | N/A |

If you have a crystal ball and can predict your future use cases, this is your decision tree:

|  | Not aligned with future C# type union | Aligned with future C# type union |
| --- | --- | --- |
| **Use Case = Result pattern only** | FluentResults | OneOf |
| **Use Case = Functional programming** | CSharpFunctionalExtensions | LanguageExt |

FluentResults is likely the solution for your immediate need, and LanguageExt is probably overkill if you are reading this (and I assume you are new to FP). In combination with the #1 spot on community adoption, **OneOf** probably strikes the best balance.
