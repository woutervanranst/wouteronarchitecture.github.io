---
layout: post
title: 'Validating Dependency Injection'
date: 2025-03-21 09:22:18
permalink: /validating-dependency-injection/
subtitle: 'Two small DI container settings that catch scoped-lifetime mistakes and registration errors at startup instead of in production.'
---

This is a snippet from Nick's [recent video](https://www.youtube.com/watch?v=uJDrf5TwwAw).

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider((context, options) =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});
```

-   **`UseDefaultServiceProvider`**: Configures the default DI (Dependency Injection) container.
-   **[`ValidateScopes`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.serviceprovideroptions.validatescopes) `= true`**: Ensures that scoped services are not resolved from the root provider, to avoid accidental singleton-like behavior.
-   **[`ValidateOnBuild`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.serviceprovideroptions.validateonbuild) `= true`**: Forces validation of the service provider at application startup to catch misconfigurations early.
