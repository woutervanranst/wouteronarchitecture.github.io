---
layout: post
title: 'Adding a CorrelationId for Exceptions for Application Insights (DRAFT)'
date: 2024-07-08 07:02:00
permalink: /adding-a-correlationid-for-exceptions-for-application-insights-draft/
---

<p>Admittedly, this title could be shorter, but it s also a bit of a niche topic.</p>


<p>AppInsights OperationID in REST ASP NET Core API Response bij Exceptions</p>

<h2>Use case / why</h2>

<p>TODO</p>

<h2>Choosing the right approach</h2>

<p>When searching for the right approach, I came across a couple of options to solve this 'aspect' (cross cutting concern) in an elegant way: middleware, filters or the default exception handler.</p>

<h3>Using Middleware</h3>

<p><a href="https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-8.0">ASP.NET Core Middleware </a>are components that are put into an application 'pipeline' to handle requests and responses. Each component in the pipeline processes requests, can pass them to the next component, and can handle responses. They're executed in the order they are added to the pipeline.</p>

<figure><img src="/wp-content/uploads/2024/07/image.png" alt=""/></figure>

<h3>Filters</h3>

<p><a href="https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?view=aspnetcore-8.0">Filters in ASP.NET Core</a> are components that can be applied before or after certain stages in the request pipeline. Exception filters specifically handle exceptions thrown during the execution of an MVC action.</p>

<p>Exception filters catch exceptions thrown in MVC controllers, providing a mechanism to handle these exceptions within the context of the controller actions.</p>

<figure><img src="/wp-content/uploads/2024/07/image-1.png" alt=""/></figure>

<ul>
</ul>

<h4>Default Exception Handler</h4>

<p><strong>What is the Default Exception Handler?</strong></p>

<p><a href="https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-8.0">ASP.NET Core provides a built-in middleware for handling exceptions</a>. This middleware can be configured to customize the error response and include additional information like a trace ID.</p>

<h3>Conclusion</h3>

<p>The following table summarizes the pros/cons of these approaches:</p>

<figure><table><tbody><tr><td></td><td><strong>Pro</strong></td><td><strong>Con</strong></td></tr><tr><td><strong>Middleware</strong></td><td><strong>Global Scope:</strong> Handles exceptions across the entire application.<br><strong>Centralized Control:</strong> Provides a single place to manage exception handling logic.</td><td><strong>Complexity:</strong> Can add complexity to the application setup.<br><strong>Performance:</strong> Adds a slight overhead due to the additional processing layer.</td></tr><tr><td><strong>Filters</strong></td><td><strong>Contextual Handling:</strong> Suitable for handling exceptions within MVC controllers.<br><strong>Easy Integration:</strong> Simple to implement and integrate with MVC actions.</td><td><strong>Limited Scope:</strong> Only handles exceptions in MVC actions.<br><strong>Redundancy:</strong> May require multiple error-handling mechanisms if the application includes non-MVC components.</td></tr><tr><td><strong>Default Exception Handler</strong></td><td><strong>Simplicity:</strong> Easy to set up and requires minimal configuration.<br><strong>Framework Integration:</strong> Leverages existing ASP.NET Core infrastructure.</td><td><strong>Limited Customization:</strong> May not offer the same level of flexibility as custom middleware.<br><strong>Dependency:</strong> Relies on middleware, which might not fit all application types.</td></tr></tbody></table></figure>

<p>Each approach for handling exceptions in ASP.NET Core—Middleware, Filters, and the Default Exception Handler—has its own strengths and limitations. Middleware is ideal for global error handling, ensuring consistent behavior across the application. Filters provide a focused mechanism for handling exceptions within MVC controllers, while the Default Exception Handler offers a straightforward way to leverage built-in framework capabilities with minimal setup. The best choice depends on your application's architecture and specific error-handling requirements.</p>

<p>Since our application already uses middleware, we'll be going for that one. </p>

<h2>Getting the right 'Id'</h2>

<p>Obviously there are a lot of Ids flying around in your context; the clue is to get the <em>right</em> one. I want the one that is front and center in Application Insights.</p>

<figure><img src="/wp-content/uploads/2024/07/image-9.png" alt=""/></figure>

<p>Through trial and error, I found that the <code>TraceId</code> of the current <code>System.Diagnostics.Activity</code> is the one that appears as <code>OperationId</code> in Application Insights:</p>

<figure><img src="/wp-content/uploads/2024/07/image-8.png" alt=""/></figure>

<p>Indeed, from the <a href="https://learn.microsoft.com/en-us/dotnet/core/diagnostics/distributed-tracing-concepts">docs</a>: </p>

<blockquote>
<p>[...] every trace is assigned a globally unique 16-byte trace-id (<a href="https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.activity.traceid#system-diagnostics-activity-traceid">Activity.TraceId</a>), and every Activity within the trace is assigned a unique 8-byte span-id (<a href="https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.activity.spanid#system-diagnostics-activity-spanid">Activity.SpanId</a>). </p>

<p>Each Activity records the trace-id, its own span-id, and the span-id of its parent (<a href="https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.activity.parentspanid#system-diagnostics-activity-parentspanid">Activity.ParentSpanId</a>)</p>
</blockquote>

<p><a href="https://tsuyoshiushio.medium.com/correlation-with-activity-with-application-insights-3-w3c-tracecontext-d9fb143c0ce2">This blog</a> post does a better (visual) job of explaining/visualizing the distributed tracing concept:</p>

<figure><img src="/wp-content/uploads/2024/07/137bb-1u_a4krik7pr3vahdnsf__a.webp" alt=""/><figcaption>Source: <a href="https://tsuyoshiushio.medium.com/?source=post_page-----d9fb143c0ce2--------------------------------">Tsuyoshi Ushio</a>'s blog</figcaption></figure>

<p>Now that we've established that we want to return the TraceId in case of an exception, we have all that we need to write our middleware.</p>

<h2>Adding the Middleware</h2>

<p>Add the middleware class:</p>

<pre><code>public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger&lt;ExceptionHandlingMiddleware&gt; logger)
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
</code></pre>

<p>Register the middleware:</p>

<pre><code>app.UseMiddleware&lt;ExceptionHandlingMiddleware&gt;();</code></pre>



<figure><img src="/wp-content/uploads/2024/07/image-11.png" alt=""/></figure>

<figure><img src="/wp-content/uploads/2024/07/image-10.png" alt=""/></figure>
