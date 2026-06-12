---
layout: post
title: 'Validating Dependency Injection'
date: 2025-03-21 09:22:18
permalink: /validating-dependency-injection/
---

<p>This is a snippet from Nick's <a href="https://www.youtube.com/watch?v=uJDrf5TwwAw">recent video</a></p>

<pre class="wp-block-syntaxhighlighter-code">var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider((context, options) =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});</pre>

<ul class="wp-block-list">
<li><strong><code>UseDefaultServiceProvider</code></strong>: Configures the default DI (Dependency Injection) container.</li>

<li><strong><code><a href="https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.serviceprovideroptions.validatescopes">ValidateScopes</a> = true</code></strong>: Ensures that scoped services are not resolved from the root provider (to avoid accidental singleton-like behavior).</li>

<li><strong><code><a href="https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.serviceprovideroptions.validateonbuild">ValidateOnBuild</a> = true</code></strong>: Forces validation of the service provider at application startup to catch misconfigurations early.</li>
</ul>
