---
layout: post
title: 'Adding a CorrelationId for Exceptions for Application Insights (DRAFT)'
date: 2024-07-08 07:02:00
permalink: /adding-a-correlationid-for-exceptions-for-application-insights-draft/
subtitle: 'How to return the right correlation ID from an ASP.NET Core API exception and line it up with Application Insights `OperationId`.'
---

Admittedly, this title could be shorter, but it s also a bit of a niche topic.

AppInsights OperationID in REST ASP NET Core API Response bij Exceptions

## Use case / why

TODO

## Choosing the right approach

When searching for the right approach, I came across a couple of options to solve this 'aspect' (cross cutting concern) in an elegant way: middleware, filters or the default exception handler.

### Using Middleware

[ASP.NET Core Middleware](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-8.0) are components that are put into an application 'pipeline' to handle requests and responses. Each component in the pipeline processes requests, can pass them to the next component, and can handle responses. They're executed in the order they are added to the pipeline.

![](/assets/posts/adding-a-correlationid-for-exceptions-for-application-insights-draft/image.png)

### Filters

[Filters in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?view=aspnetcore-8.0) are components that can be applied before or after certain stages in the request pipeline. Exception filters specifically handle exceptions thrown during the execution of an MVC action.

Exception filters catch exceptions thrown in MVC controllers, providing a mechanism to handle these exceptions within the context of the controller actions.

![](/assets/posts/adding-a-correlationid-for-exceptions-for-application-insights-draft/image-1.png)

#### Default Exception Handler

**What is the Default Exception Handler?**

[ASP.NET Core provides a built-in middleware for handling exceptions](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-8.0). This middleware can be configured to customize the error response and include additional information like a trace ID.

### Conclusion

The following table summarizes the pros/cons of these approaches:

|  | **Pro** | **Con** |
| --- | --- | --- |
| **Middleware** | **Global Scope:** Handles exceptions across the entire application.  <br>**Centralized Control:** Provides a single place to manage exception handling logic. | **Complexity:** Can add complexity to the application setup.  <br>**Performance:** Adds a slight overhead due to the additional processing layer. |
| **Filters** | **Contextual Handling:** Suitable for handling exceptions within MVC controllers.  <br>**Easy Integration:** Simple to implement and integrate with MVC actions. | **Limited Scope:** Only handles exceptions in MVC actions.  <br>**Redundancy:** May require multiple error-handling mechanisms if the application includes non-MVC components. |
| **Default Exception Handler** | **Simplicity:** Easy to set up and requires minimal configuration.  <br>**Framework Integration:** Leverages existing ASP.NET Core infrastructure. | **Limited Customization:** May not offer the same level of flexibility as custom middleware.  <br>**Dependency:** Relies on middleware, which might not fit all application types. |

Each approach for handling exceptions in ASP.NET Core—Middleware, Filters, and the Default Exception Handler—has its own strengths and limitations. Middleware is ideal for global error handling, ensuring consistent behavior across the application. Filters provide a focused mechanism for handling exceptions within MVC controllers, while the Default Exception Handler offers a straightforward way to leverage built-in framework capabilities with minimal setup. The best choice depends on your application's architecture and specific error-handling requirements.

Since our application already uses middleware, we'll be going for that one.

## Getting the right 'Id'

Obviously there are a lot of Ids flying around in your context; the clue is to get the *right* one. I want the one that is front and center in Application Insights.

![](/assets/posts/adding-a-correlationid-for-exceptions-for-application-insights-draft/image-9.png)

Through trial and error, I found that the `TraceId` of the current `System.Diagnostics.Activity` is the one that appears as `OperationId` in Application Insights:

![](/assets/posts/adding-a-correlationid-for-exceptions-for-application-insights-draft/image-8.png)

Indeed, from the [docs](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/distributed-tracing-concepts):

> \[...\] every trace is assigned a globally unique 16-byte trace-id ([Activity.TraceId](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.activity.traceid#system-diagnostics-activity-traceid)), and every Activity within the trace is assigned a unique 8-byte span-id ([Activity.SpanId](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.activity.spanid#system-diagnostics-activity-spanid)).
>
> Each Activity records the trace-id, its own span-id, and the span-id of its parent ([Activity.ParentSpanId](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.activity.parentspanid#system-diagnostics-activity-parentspanid))

[This blog](https://tsuyoshiushio.medium.com/correlation-with-activity-with-application-insights-3-w3c-tracecontext-d9fb143c0ce2) post does a better (visual) job of explaining/visualizing the distributed tracing concept:

![](/assets/posts/adding-a-correlationid-for-exceptions-for-application-insights-draft/137bb-1u_a4krik7pr3vahdnsf__a.webp)

*Source: [Tsuyoshi Ushio](https://tsuyoshiushio.medium.com/?source=post_page-----d9fb143c0ce2--------------------------------)'s blog*

Now that we've established that we want to return the TraceId in case of an exception, we have all that we need to write our middleware.

## Adding the Middleware

Add the middleware class:

```csharp
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            // Log the Exception
            _logger.LogError(ex, ex.Message);

            // Get the operation ID
            var operationId = Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier;

            // Construct the response
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                message = ex.Message,
                operationId = operationId
            };

            await httpContext.Response.WriteAsJsonAsync(response);
        }
    }
}
```

Register the middleware:

```csharp
app.UseMiddleware<ExceptionHandlingMiddleware>();
```

![](/assets/posts/adding-a-correlationid-for-exceptions-for-application-insights-draft/image-11.png)

![](/assets/posts/adding-a-correlationid-for-exceptions-for-application-insights-draft/image-10.png)
