---
layout: post
title: 'C# String Comparison Explained'
date: 2024-02-06 06:34:00
permalink: /c-string-comparison-explained/
subtitle: 'A practical guide to choosing the right C# string comparison mode for culture-sensitive, case-insensitive, and internal comparisons.'
---

When doing string comparisons, I always defaulted to `StringComparison.InvariantCultureIgnoreCase` without giving it much further thought.

However, I recently had to explain what it *actually* did (and what it *doesn't*) and I was a bit at a loss. This is what I found out.

Understanding how to compare strings properly is crucial, especially when dealing with internationalization and localization. C# offers several methods for comparing strings, each tailored to specific needs and scenarios. In this blog post, we'll explore the nuances of `CurrentCulture`, `CurrentCultureIgnoreCase`, `InvariantCulture`, `InvariantCultureIgnoreCase`, `Ordinal`, and `OrdinalIgnoreCase` comparisons through practical examples.

### The Basics of String Comparison in C#

String comparison in C# can be done using several approaches, each serving different purposes:

-   **Culture-Sensitive Comparisons**: `CurrentCulture` and `InvariantCulture` comparisons consider linguistic rules of a specific culture, which is essential for displaying data in a way that is familiar to the user.
-   **Case-Insensitive Comparisons**: Adding `IgnoreCase` to `CurrentCulture` or `InvariantCulture` makes the comparison case-insensitive, which is useful when the case should not affect the comparison outcome.
-   **Ordinal Comparisons**: `Ordinal` and `OrdinalIgnoreCase` comparisons are based on the binary values of characters, making them suitable for internal, non-user-facing operations where performance is critical.

### Examples

#### Example 1: The German "Straße" and Culture-Sensitive Comparison

When comparing "straße" (street in German) to "strasse", the differences between comparison methods become evident:

-   **CurrentCulture** (`de-DE`): Considers them equal because it applies German linguistic rules where 'ß' and 'ss' are considered equal.
-   **InvariantCulture**: Sees them as different, as it doesn't apply specific cultural rules.
-   **Ordinal**: Also finds them different, purely based on binary character values.

#### Example 2: French Accented Characters and Culture-Sensitive Comparison

By contrast, in French accents are important so comparing "côte" with "cote" is always different:

-   **CurrentCulture** (`fr-FR`): Treats them as different due to the distinct linguistic importance of accents in French.
-   **InvariantCulture & OrdinalIgnoreCase**: Still considers them different, emphasizing the binary difference without cultural context.

### Conclusion

These examples underscore the importance of choosing the right string comparison method:

-   **User-Facing Content**: Use culture-sensitive comparisons (`CurrentCulture` or `CurrentCultureIgnoreCase`) to respect the linguistic rules and user expectations based on their locale.
-   **Data Storage and Internal Logic**: Opt for `InvariantCulture` or `Ordinal` comparisons for consistency across cultures, ensuring that your application's behavior doesn't change unexpectedly when deployed in different locales.
-   **Performance Considerations**: `Ordinal` comparisons are faster than culture-sensitive ones, making them the preferred choice for operations where performance is critical and cultural nuances are not a concern.
