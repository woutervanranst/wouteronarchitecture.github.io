---
layout: post
title: 'C# String Comparison Explained'
date: 2024-02-06 06:34:00
permalink: /c-string-comparison-explained/
---

<p>When doing string comparisons, I always defaulted to <code>StringComparison.InvariantCultureIgnoreCase</code> without giving it much further thought. </p>

<p>However, I recently had to explain what it <em>actually</em> did (and what it <em>doesn't</em>) and I was a bit at a loss. This is what I found out.</p>


<p>Understanding how to compare strings properly is crucial, especially when dealing with internationalization and localization. C# offers several methods for comparing strings, each tailored to specific needs and scenarios. In this blog post, we'll explore the nuances of <code>CurrentCulture</code>, <code>CurrentCultureIgnoreCase</code>, <code>InvariantCulture</code>, <code>InvariantCultureIgnoreCase</code>, <code>Ordinal</code>, and <code>OrdinalIgnoreCase</code> comparisons through practical examples.</p>

<h3>The Basics of String Comparison in C#</h3>

<p>String comparison in C# can be done using several approaches, each serving different purposes:</p>

<ul>
<li><strong>Culture-Sensitive Comparisons</strong>: <code>CurrentCulture</code> and <code>InvariantCulture</code> comparisons consider linguistic rules of a specific culture, which is essential for displaying data in a way that is familiar to the user.</li>

<li><strong>Case-Insensitive Comparisons</strong>: Adding <code>IgnoreCase</code> to <code>CurrentCulture</code> or <code>InvariantCulture</code> makes the comparison case-insensitive, which is useful when the case should not affect the comparison outcome.</li>

<li><strong>Ordinal Comparisons</strong>: <code>Ordinal</code> and <code>OrdinalIgnoreCase</code> comparisons are based on the binary values of characters, making them suitable for internal, non-user-facing operations where performance is critical.</li>
</ul>

<h3>Examples</h3>

<h4>Example 1: The German "Straße" and Culture-Sensitive Comparison</h4>

<p>When comparing "straße" (street in German) to "strasse", the differences between comparison methods become evident:</p>

<ul>
<li><strong>CurrentCulture</strong> (<code>de-DE</code>): Considers them equal because it applies German linguistic rules where 'ß' and 'ss' are considered equal.</li>

<li><strong>InvariantCulture</strong>: Sees them as different, as it doesn't apply specific cultural rules.</li>

<li><strong>Ordinal</strong>: Also finds them different, purely based on binary character values.</li>
</ul>

<h4>Example 2: French Accented Characters and Culture-Sensitive Comparison</h4>

<p>By contrast, in French accents are important so comparing "côte" with "cote" is always different:</p>

<ul>
<li><strong>CurrentCulture</strong> (<code>fr-FR</code>): Treats them as different due to the distinct linguistic importance of accents in French.</li>

<li><strong>InvariantCulture &amp; OrdinalIgnoreCase</strong>: Still considers them different, emphasizing the binary difference without cultural context.</li>
</ul>

<h3>Conclusion</h3>

<p>These examples underscore the importance of choosing the right string comparison method:</p>

<ul>
<li><strong>User-Facing Content</strong>: Use culture-sensitive comparisons (<code>CurrentCulture</code> or <code>CurrentCultureIgnoreCase</code>) to respect the linguistic rules and user expectations based on their locale.</li>

<li><strong>Data Storage and Internal Logic</strong>: Opt for <code>InvariantCulture</code> or <code>Ordinal</code> comparisons for consistency across cultures, ensuring that your application's behavior doesn't change unexpectedly when deployed in different locales.</li>

<li><strong>Performance Considerations</strong>: <code>Ordinal</code> comparisons are faster than culture-sensitive ones, making them the preferred choice for operations where performance is critical and cultural nuances are not a concern.</li>
</ul>
