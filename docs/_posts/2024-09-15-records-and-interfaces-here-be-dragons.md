---
layout: post
title: 'Records and Interfaces: Here Be Dragons'
date: 2024-09-15 07:22:00
permalink: /records-and-interfaces-here-be-dragons/
---

When it comes to C# types—**classes**, **structs**, and **records**—understanding the difference between value and reference equality is crucial. Each type behaves differently in terms of equality checks, inheritance, and how they manage their internal state. However, things can get tricky when you introduce **interfaces** into the mix, particularly when dealing with records.

C# introduced **records** in C# 9.0 to address common scenarios where developers needed to create immutable data types efficiently and with less boilerplate code. The primary purpose of records is to represent **data models** or **DTOs (Data Transfer Objects)** that are focused on holding data rather than behavior, making it easier to create concise, immutable, and value-based objects.

In this blog post, we'll explore how equality works with records and interfaces, and uncover the nuances (and potential pitfalls) of combining these two features in C#. Let’s first start by comparing the basic C# types.

---

### Classes, Structs, and Records: A Comparison

Below is a comparison of **classes**, **structs**, and **records** in terms of value/reference equality, inheritance support, and typical use cases.

| Feature | **Class** | **Struct** | **Record** |
| --- | --- | --- | --- |
| **Type** | Reference Type | Value Type | Reference Type |
| **Equality** | Reference equality | Value equality | Value equality (based on properties) |
| **Inheritance Support** | Yes | No | Yes |
| **Memory Location** | Heap | Stack (or inline in the heap for large structs) | Heap |
| **Default `==` Operator** | Reference comparison | Field-by-field comparison | Value-based comparison |

## Records and Equality

Let's begin with a simple `Foo` type that has a single string property, `Bar`.

```csharp
record Foo(string Bar);
var foo1 = new Foo("bar");
var foo2 = new Foo("bar");

(foo1 == foo2).Should().BeTrue();    // True, since records use value comparison by default
foo1.Equals(foo2).Should().BeTrue(); // True, because Equals is overridden to compare values in records
(foo1.GetHashCode() == foo2.GetHashCode()).Should().BeTrue(); // True, as the hash codes are based on the values of the properties
```

## Records with an Interface and Equality

Now consider this change

```csharp
interface IFoo
{
    string Bar { get; }
}

record Foo(string Bar) : IFoo;

IFoo foo1 = new Foo("bar");
IFoo foo2 = new Foo("bar");
```

Now stop and think, which of the assertions will fail?

The first one!

```csharp
(foo1 == foo2).Should().BeFalse(); // FALSE!!, because now we are comparing interface types, which defaults to reference equality
foo1.Equals(foo2).Should().BeTrue(); // True, because Equals is still overridden in the record and compares values
(foo1.GetHashCode() == foo2.GetHashCode()).Should().BeTrue(); // True, as hash codes are based on the underlying record's properties
```

When `foo1` and `foo2` are cast to the `IFoo` interface, `==` now checks **reference equality**, not value equality. This is a key point: **when using records through interfaces, `==` no longer performs value comparison**.

This bit me when I refactored a record type to make it `internal` and exposed it through a `public interface` - this suddenly broke my unit tests :/. I struggled to understand why, until I stumbled upon [https://stackoverflow.com/questions/73962920/equality-of-interface-types-implemented-by-records](https://stackoverflow.com/questions/73962920/equality-of-interface-types-implemented-by-records).

## Classes with an Interface and Equality

For good measure - a refresher, if now we make Foo a `class`:

```csharp
class Foo(string Bar) : IFoo
{
    public string Bar { get; init; } = Bar;
}

IFoo foo1 = new Foo("bar");
IFoo foo2 = new Foo("bar");

(foo1 == foo2).Should().BeFalse(); // False, like above, since we're still doing reference comparison

foo1.Equals(foo2).Should().BeFalse(); // By default, the Equals method on interfaces also checks reference equality, so this will return false even though the underlying values (Bar) are the same

(foo1.GetHashCode() == foo2.GetHashCode()).Should().BeFalse(); // HashCode is based on the object reference when using interfaces, so the hash codes will be different
```

### Conclusion

Combining **records** with **interfaces** introduces a subtle, but important, behavior change. While records are designed to provide value equality, casting them to an interface causes equality to fall back to **reference equality** when using the `==` operator.

-   **Records use value-based equality**, comparing properties by default.
-   **Classes and structs** behave differently: classes use reference equality by default, while structs use value equality.
-   **When casting records to interfaces**, the `==` operator behaves like it does for regular reference types, meaning it checks **reference equality**, not value equality.
-   **To avoid confusion**, you should rely on `Equals` when working with records through interfaces, or avoid casting records to interfaces when performing equality checks.

| **Scenario** | **`==`** | **`Equals`** | **`GetHashCode()`** |
| --- | --- | --- | --- |
| **Record** | `True` (value equality) | `True` (value equality) | `True` (value-based) |
| **Record with Interface (IFoo)** | `False` (reference equality) | `True` (value equality) | `True` (value-based) |
| **Class with Interface (IFoo)** | `False` (reference equality) | `False` (reference equality) | `False` (reference-based) |
