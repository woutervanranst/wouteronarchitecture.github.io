---
layout: post
title: 'Value Objects in C# - Part 1'
date: 2024-02-09 06:44:00
permalink: /value-objects-in-c-part-1/
---

<h2 class="wp-block-heading">Choosing between <code>class</code> and <code>record</code></h2>

<figure class="wp-block-image aligncenter size-full"><img src="/wp-content/uploads/2024/02/image-1.png" alt="" class="wp-image-59"/></figure>

<!--more-->

<h3 class="wp-block-heading">Mutability vs. Immutability</h3>

<p><em>Mutable</em> and <em>immutable</em> are terms used to describe whether the state of an object can be changed after it is created.</p>

<ul class="wp-block-list">
<li><strong>Classes</strong> are typically mutable, meaning their properties or fields can be changed after an instance is created. This is useful for objects whose state is expected to change over time.</li>

<li><strong>Records</strong> are designed with immutability in mind. By default, records provide a simple syntax to define immutable properties. This encourages safer, thread-safe programming practices by ensuring an object's state cannot be modified once it's been created.</li>
</ul>

<h3 class="wp-block-heading">Value Semantics vs. Reference Semantics</h3>

<ul class="wp-block-list">
<li><strong>Classes</strong> use reference semantics, meaning that if you have two class instances and you modify one, the other is not affected unless they reference the same object.</li>

<li><strong>Records</strong> introduce value semantics for equality comparison. Two record instances are considered equal if their values are the same, not based on their memory addresses. This is particularly useful for data-carrying objects where the data identity is more relevant than the object identity.</li>
</ul>

<h3 class="wp-block-heading">Inheritance</h3>

<ul class="wp-block-list">
<li><strong>Classes</strong> support inheritance, allowing you to create a derived class that inherits properties and methods from a base class. This is key for many object-oriented design patterns.</li>

<li><strong>Records</strong> also support inheritance, can derive from other record types, and you can override the equality semantics in derived records.</li>
</ul>

<h3 class="wp-block-heading">Duplication</h3>

<ul class="wp-block-list">
<li><strong>Classes </strong>have the concept of concept memberwise clone. It creates a shallow copy of an object. This means that for each field in the original object, a new object is created with a copy of the field's value. If the field is a value type, a direct copy of the value is performed. For reference types, however, the copy is of the reference itself, not the object it points to, leading to both the original and the cloned object referring to the same instances of those reference types. This is the default behavior aka. a <strong>shallow copy</strong>. By contrast, a <strong>deep copy</strong>, where new instances are created for all reference types within the object, custom implementation is required.</li>
</ul>

<pre class="wp-block-syntaxhighlighter-code alignwide">public class Person
{
    public int Age;
    public string Name;
    public IdInfo IdInfo;

    public Person ShallowCopy()
    {
       return (Person) this.MemberwiseClone();
    }

    public Person DeepCopy()
    {
       Person other = (Person) this.MemberwiseClone();
       other.IdInfo = new IdInfo(IdInfo.IdNumber);
       other.Name = String.Copy(Name);
       return other;
    }
}</pre>

<ul class="wp-block-list">
<li><strong>Records</strong> introduce a syntax for non-destructive mutation through <code>with</code> expressions. This allows you to create a new record instance by copying an existing instance while changing some of the properties in the process. This capability is particularly powerful for creating modified copies of immutable objects without cumbersome copy-and-update code.</li>
</ul>

<pre class="wp-block-syntaxhighlighter-code alignwide">var originalRecord = new MyRecord { Property1 = "value1", Property2 = "value2" };
var modifiedRecord = originalRecord with { Property2 = "new value" };</pre>

<h3 class="wp-block-heading">Performance Considerations</h3>

<ul class="wp-block-list">
<li><strong>Classes</strong> and <strong>Records</strong> have different performance characteristics, especially in scenarios involving heavy use of equality checks or copying.</li>

<li><strong>Records</strong>, with their built-in value-based equality and <code>with</code> expressions, introduce overhead not present with simple class references.</li>
</ul>

<h3 class="wp-block-heading">Serialization and Deserialization</h3>

<ul class="wp-block-list">
<li>Both <strong>Classes</strong> and <strong>Records</strong> can be serialized and deserialized, but records offer a more straightforward approach due to their immutability and simple construction syntax, making them ideal for data transfer objects (DTOs) in web services or applications involving data exchange.</li>
</ul>

<h3 class="wp-block-heading">Conclusion</h3>

<div class="wp-block-jetpack-markdown"><p><a href="https://mermaid.live/edit#pako:eNp1ks2O0zAUhV_lyuumSpNJSrMYJJpCWxALQKChYeHaN41RYgfHmZlS9d25iRphjSDKIj_H53zn2hcmjESWsbI2T6Li1sGXvNBA15tDbrADVyGY408UDui_PiF0jjt8_eOmgiCAB9IFwT2sD2ujOyXRwrrmXedrPppRkh92HTzyukfAXz2vlTuDalpjHdduMs19083hm6prn-OI0HcoQWmo1KkKWrSlsQ3XguAEam6V6Savje_19gUgPClXwdcRZzPh7Jq2xgY11VRG-za3Du_-unxCYaz0sW-a7dBTmMHqmYAr_qiMBbqVrtAqN8JqRIlyIt36pLt_j3LrReyHCEkAunO2FwPsENBy59BqaLgTldInKC1NmurU53FsU9zej3v_n0p7L-_Dy-GZSQxHPuwH5bcWS7RI5ciAzViDtC9K0gG7DIYFo21ssGAZPUoseV-7ghX6SlLeO_P5rAXLqA3OWN9KOme54ifLG5aVvO7oa8v1d2OaSUSvLLuwZ5bF8Ty9W4ZJGkXRIl0lixk7s2y5nCdhmCzCJFrFq7sovs7Y73F9OF8tXkVpGK6WSRintOz6B2bK9d0"><img src="https://mermaid.ink/img/pako:eNp1ks2O0zAUhV_lyuumSpNJSrMYJJpCWxALQKChYeHaN41RYgfHmZlS9d25iRphjSDKIj_H53zn2hcmjESWsbI2T6Li1sGXvNBA15tDbrADVyGY408UDui_PiF0jjt8_eOmgiCAB9IFwT2sD2ujOyXRwrrmXedrPppRkh92HTzyukfAXz2vlTuDalpjHdduMs19083hm6prn-OI0HcoQWmo1KkKWrSlsQ3XguAEam6V6Savje_19gUgPClXwdcRZzPh7Jq2xgY11VRG-za3Du_-unxCYaz0sW-a7dBTmMHqmYAr_qiMBbqVrtAqN8JqRIlyIt36pLt_j3LrReyHCEkAunO2FwPsENBy59BqaLgTldInKC1NmurU53FsU9zej3v_n0p7L-_Dy-GZSQxHPuwH5bcWS7RI5ciAzViDtC9K0gG7DIYFo21ssGAZPUoseV-7ghX6SlLeO_P5rAXLqA3OWN9KOme54ifLG5aVvO7oa8v1d2OaSUSvLLuwZ5bF8Ty9W4ZJGkXRIl0lixk7s2y5nCdhmCzCJFrFq7sovs7Y73F9OF8tXkVpGK6WSRintOz6B2bK9d0?type=png" alt=""></a></p>
</div>

<p><strong>Use Classes</strong> when you need objects with mutable state, complex behavior, or extensive use of object-oriented features like inheritance and polymorphism beyond simple data storage.</p>

<p><strong>Use Records</strong> for data-centric models where immutability, simple value-based equality, and straightforward data manipulation (e.g., cloning with modifications) are desired. They're especially suitable for functional programming patterns, DTOs, and quickly comparing complex data structures by value.</p>
