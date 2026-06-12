---
layout: post
title: 'Value Objects in C# - Part 2'
date: 2024-02-14 07:00:00
permalink: /value-objects-in-c-part-2/
---

<h2>2. Choosing the property modifiers</h2>

<p>Property setters in C# play a crucial role in defining how you can interact with class and record members. The <code>set</code>, <code>init</code>, and <code>get</code>-only accessors offer different levels of mutability and initialization control, crucial for both mutable and immutable object design. There are a couple of options out there, which one to choose?</p>


<figure><table><tbody><tr><td><code>public string Name { get; }</code></td><td><code>public required </code><code>string Name { get; }</code></td></tr><tr><td><code>public string Name { get; set; }</code></td><td><code>public required string Name { get; set; }</code></td></tr><tr><td><code>public string Name { get; init; }</code></td><td><code>public required string Name { get; init; }</code></td></tr></tbody></table></figure>

<h3>Mutable Properties with <code>set</code></h3>

<p>The <code>set</code> accessor allows properties to be changed at any point in an object's lifetime. It's commonly used in classes where mutability is a requirement.</p>

<pre><code><code>public class Person<br>{<br>    public string Name { get; set; }<br>}<br><br>// Initialization and mutation<br>var person = new Person { Name = "Alice" };<br>person.Name = "Bob"; // Property can be changed after initialization</code></code></pre>

<p>However, records also support this but is generally considered an <strong>anti-pattern</strong>.</p>

<pre><code>public <strong>record </strong>Person
<br>{
<br>    public string Name { get; set; }
<br>}
<br><br>var mutablePerson = new MutablePerson { Name = "Alice" };
<br>mutablePerson.Name = "Bob"; // The Name property can be changed after initialization
</code></pre>

<h3>Immutable Properties with <code>init</code></h3>

<p>The <code>init</code> accessor, introduced in C# 9.0, is designed for scenarios where you want to allow property values to be set at the time of object creation but remain immutable afterward.</p>

<pre><code><code>public &#91;class|record] Person<br>{<br>    public string Name { get; <strong>init;</strong> }<br>}<br><br>// Initialization<br>var immutablePerson = new Person { Name = "Alice" };<br>// immutablePerson.Name = "Bob"; // This line would result in a compile-time error<br></code></code></pre>

<h3>Read-Only Properties with <code>get</code></h3>

<p>Defining a property with only a <code>get</code> accessor makes it read-only. This is useful for both computed properties and ensuring that a property remains unchanged after the object's construction.</p>

<pre><code><code>public &#91;class|record] Person<br>{<br>    public string Name { get; }<br><br>    public Person(string name)<br>    {<br>        Name = name;<br>    }<br>}<br><br>// Initialization<br>var readOnlyPerson = new PersonWithReadOnlyProperty("Alice");<br>// readOnlyPerson.Name = "Bob"; // Not allowed<br></code></code></pre>

<h3>Required Properties with <code>required</code></h3>

<p>The <code>required</code> keyword ensures that certain properties must be initialized during object creation, enhancing compile-time checks. It's applicable to both classes and records.</p>

<pre><code><code>public &#91;class|record] Person<br>{<br>    public <strong>required </strong>string Name { get; init; }<br>}<br><br>// Initialization<br>var requiredPerson = new Person { Name = "Alice" };<br></code><br>// This attempt will fail to compile
<br>var personWithoutName = new Person(); // Compile-time error</code></pre>

<h3>Conclusion</h3>

<div><p><a href="https://mermaid.live/edit#pako:eNpNUMFSgzAQ_ZXMnmknQAHhoFNL60kvOs4oeFhJaDMTkhqS0drpv5tS6pBL8va9fW83R2g041BAK_V3s0NjyUtZK-LPsnp0Fj8lJ9habohQwgqU4het0OruY1SR2Yy88d5ft-S-2nJLem6n5JMeuFX1itJ5s74XW8UZ0YY0KBsn0XokFLE7ThqtemtcY7W5JqymCeU5YUqM7ush-TziSJZDeTOGGv7lhOHs6rm-sBewmQY8VEvG_vVeDgF03HQomP-l47mjBj9px2so_JPxFp20NdTq5KXorH4-qAYKvwUPwO2ZX68UuDXYQdGi7H11j-pd6-4q8hCKI_xAEcfzdJHRJI2iKEzzJAzgAEWWzRNKk5AmUR7niyg-BfA79NN5Ht5EKaV5ltA4TaPw9AcO3YtN"><img src="https://mermaid.ink/img/pako:eNpNUMFSgzAQ_ZXMnmknQAHhoFNL60kvOs4oeFhJaDMTkhqS0drpv5tS6pBL8va9fW83R2g041BAK_V3s0NjyUtZK-LPsnp0Fj8lJ9habohQwgqU4het0OruY1SR2Yy88d5ft-S-2nJLem6n5JMeuFX1itJ5s74XW8UZ0YY0KBsn0XokFLE7ThqtemtcY7W5JqymCeU5YUqM7ush-TziSJZDeTOGGv7lhOHs6rm-sBewmQY8VEvG_vVeDgF03HQomP-l47mjBj9px2so_JPxFp20NdTq5KXorH4-qAYKvwUPwO2ZX68UuDXYQdGi7H11j-pd6-4q8hCKI_xAEcfzdJHRJI2iKEzzJAzgAEWWzRNKk5AmUR7niyg-BfA79NN5Ht5EKaV5ltA4TaPw9AcO3YtN?type=png" alt=""></a></p>
</div>

<ul>
<li><strong>Mutable vs. Immutable</strong>: Use <code>set</code> for mutable properties in classes. Opt for <code>init</code> in records (and classes when appropriate) for immutable properties.</li>

<li><strong>Initialization Control</strong>: <code>init</code> allows properties to be set at initialization time, perfect for immutable data patterns. <code>required</code> ensures all necessary properties are initialized.</li>

<li><strong>Read-Only Properties</strong>: Use <code>get</code>-only for properties that should not change after an object is constructed, suitable for computed properties or fixed values.</li>
</ul>

