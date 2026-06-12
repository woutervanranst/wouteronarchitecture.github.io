---
layout: post
title: 'Exploring options for building REST APIs with C# and deploying to Azure'
date: 2025-01-09 07:32:41
permalink: /exploring-options-for-building-rest-apis-with-c-and-deploying-to-azure/
---

> Somebody asked me, what is the difference between a C# REST API running as a Web App on an App Service as a container and a C# REST API running as a HTTP Function on an App Service or a Consumption Plan?

There are lots of aspects in this question, so let's unpack this a bit first.

First, let's talk about the various ways we can build REST APIs in .NET.
Then, we'll talk about how you can deploy these, either as code or as a container.
Last, we'll explore - if you end up with a container - which Azure service best fits your needs.

## Building a REST API with .NET

Various options exist to build a REST API in .NET. From the structured and robust **Controller-based APIs** to the lean and efficient **Minimal APIs**, .NET provides solutions that fit every scale and complexity. For developers embracing serverless computing, **HTTP Triggers in Azure Functions** offer a lightweight, event-driven approach, while **ASP.NET Core Integration in Azure Functions** combines the best of serverless scalability with the power of ASP.NET Core's middleware and dependency injection. Each approach is optimized for specific use cases, enabling you to design APIs that are performant, maintainable, and scalable.

| **Aspect** | **Controller-Based API** | **Minimal API** | **Function with HTTP Trigger** | **Function with HTTP Trigger and ASP.NET Core Integration** |
| --- | --- | --- | --- | --- |
| **Environment** | ASP.NET Core Web Server (Kestrel) | ASP.NET Core Web Server (Kestrel) | Azure Functions Runtime | Azure Functions Runtime + ASP.NET Core |
| **Startup** | Runs with `dotnet run` | Runs with `dotnet run` | Runs with Azure Functions Core Tools | Runs with Azure Functions Core Tools |
| **Routing** | Centralized, attribute-driven (`[Route]`, `[HttpGet]`, `[HttpPost]`) | Centralized, handler-based via `Map` methods | Function-level routing via triggers | Function-level routing via triggers |
| **Application Lifecycle** | `[WebApplication.CreateBuilder()](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/webapplication)` | `[WebApplication.CreateBuilder()](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/webapplicatio)` | `[FunctionsApplication.CreateBuilder();](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=hostbuilder%2Cwindows#start-up-and-configuration)` | `[FunctionsApplication.CreateBuilder();](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=hostbuilder%2Cwindows#configure-startup)` |
| **State Management** | Application-wide state possible | Application-wide state possible | Stateless (external storage required) | Application-wide state possible |
| **Use Case** | Best for complex or large APIs | Best for lightweight, fast APIs | Lightweight, single-purpose endpoints | Serverless apps with full ASP.NET Core features |

Link to the samples: [https://github.com/woutervanranst/CSharpRestApis](https://github.com/woutervanranst/CSharpRestApis)

### Controller-Based API (Web App)

[Microsoft Docs](https://learn.microsoft.com/en-us/aspnet/core/web-api)

```
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var app = builder.Build();
app.UseAuthorization();
app.MapControllers();
app.Run();

[ApiController]
[Route("[controller]")]
public class HelloController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Hello world");
    }
}
```

---

### Minimal API (Web App)

[Microsoft Docs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/overview)

```
var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();
app.MapGet("/", () => "Hello World!");

app.Run();
```

---

### HTTP Trigger (Azure Function)

[Microsoft Docs](https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-http-webhook-trigger)

```
var builder = FunctionsApplication.CreateBuilder(args);
builder.Build().Run();

public class HttpTriggerFunction
{
    [Function("HttpExample")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestData req,
        FunctionContext executionContext)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.WriteString("Hello world");

        return response;
    }
}
```

### ASP.NET Core Integration (Azure Function)

[Microsoft Docs](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=hostbuilder%2Cwindows#aspnet-core-integration)

```
var builder = FunctionsApplication.CreateBuilder(args);
builder.ConfigureFunctionsWebApplication();
builder.Build().Run();

public class Function
{
    [Function("HttpFunction")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        return new OkObjectResult("Hello world");
    }
}
```

## Deploy to Azure as Code or deploy as Container

Both on App Service and Functions you have the option to deploy your code directly or as a container:

![](/wp-content/uploads/2025/01/image-3.png)

*Options for Web App (App Service)*

![](/wp-content/uploads/2025/01/image-5.png)

*Options for Azure Functions (Functions Premium or App Service)*

Code deployment is the 'starter' option; this is where you typically begin. As you enter more advanced scenarios, you'd typically go for a containerized deployment. It has advantages, but you'll need to create a `Dockerfile` that describes the dependencies for your application, and probably additional steps in your CI/CD pipeline.

### Advantages of Code Deployment

-   Simpler & cost effective setup, no need to manage Docker images.
-   OS & Platform updates are done for you.
-   Native integration with Azure's managed runtime environments.
-   Best for smaller, straightforward applications with minimal custom requirements.

### Advantages of Container Deployment

-   **Environment Consistency**: Containers ensure that the application runs consistently regardless of where it's deployed (local development, staging, production). All dependencies, runtime versions, and configurations are packaged together in the container, reducing "it works on my machine" issues.
-   **Portability**: Containers allow you to move your application easily between cloud providers or from on-premises to Azure without significant changes. The same container image can run on Azure Kubernetes Service (AKS), Azure App Service, or even another container runtime environment.
-   **Custom Runtime and Dependencies**: Containers allow you to use custom runtimes or versions of frameworks and libraries that may not be natively supported by Azure App Service or Azure Functions. You can include specific operating system dependencies, libraries, and tools that are not available in the default Azure environment.
-   **Version Control**: Container images are versioned and stored in container registries (e.g., Azure Container Registry), allowing you to manage rollbacks and updates more effectively.
-   **Isolation**: Containers provide isolated environments for each application or microservice, preventing conflicts between different applications deployed on the same platform.
-   **Integration with CI/CD Pipelines**: Containers integrate seamlessly with CI/CD pipelines, enabling automated builds, tests, and deployments through tools like Azure DevOps, GitHub Actions, or Jenkins. Container-based CI/CD pipelines ensure consistent environments from development to production.
-   **Future Flexibility**: If you plan to migrate to container orchestration solutions like AKS or another cloud, container deployments make this transition smoother.

In conclusion, use the **code deployment** for small-to-medium applications that need straightforward hosting. Use **containerized deployment** when you already have a container or require portability, flexibility, and when your application requires advanced orchestration (e.g., AKS, ACA, or hybrid cloud setups).

## Container hosting options

So, eventually you'll end up with an application in one or more containers. Before you proceed, do you need orchestration with that (scaling, different microservices that work together) or not?

| Orchestration? | **Service** | **Best For** | **Advantages** | **Limitations** | Hosting options |
| --- | --- | --- | --- | --- | --- |
| **Without Orchestration** | **Azure Functions** | Event-driven, bursty, or sporadic traffic REST APIs | Auto-scaling (to zero), cost-efficient, serverless | Stateless, limited to event-driven workloads | [Serverless & Dedicated](https://learn.microsoft.com/en-us/azure/azure-functions/functions-scale) |
|  | **Azure App Service** | Predictable traffic, fully managed web apps/APIs | Built-in CI/CD, SSL, logging, simplified scaling | Limited customization, tied to HTTP(S) workloads | [Dedicated](https://learn.microsoft.com/en-us/azure/app-service/overview-hosting-plans) |
|  | **Azure Container Instances (ACI)** | Short-lived, simple containerized workloads | Fast deployment, pay-as-you-go pricing | No auto-scaling, limited scalability | Per container |
| **With Orchestration** | **Azure Container Apps (ACA)** | Moderate complexity, microservices, event-driven workloads | Auto-scaling, Dapr support, simpler orchestration | Less granular control compared to AKS | [Serverless & Dedicated](https://learn.microsoft.com/en-us/azure/container-apps/plans) |
|  | **Azure Kubernetes Service (AKS)** | Complex, large-scale, or multi-container orchestrations | Full Kubernetes flexibility, advanced customizations | High operational complexity, requires Kubernetes expertise | Dedicated (VMs) |

Typically, the **serverless** options provide low(er) cost and operational overhead but with limitations in performance and customization. The **dedicated** options provide a high(er) control and performance but require more management and incurs fixed costs.
