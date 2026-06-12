---
layout: post
title: 'Overriding Application Insights log levels'
date: 2024-03-04 07:02:00
permalink: /overriding-application-insights-log-levels/
---

(This post is still draft as I work out a minimal example)

## Isolated Functions (.NET 8)

From this docs page: [https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=windows#configure-startup](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=windows#configure-startup)

```
var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .ConfigureLogging(logging =>
    {
        logging.Services.Configure<LoggerFilterOptions>(options =>
        {
            LoggerFilterRule defaultRule = options.Rules.FirstOrDefault(rule => rule.ProviderName
                == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");
            if (defaultRule is not null)
            {
                options.Rules.Remove(defaultRule);
            }
        });
    })
    .Build();

host.Run();
```

Note the removal of the `defaultRule` ***after*** `ConfigureServices`.

Then, in your config.json (or whatever configuration provider you are referring), you can override the log level:

```
    "Logging": {
        "LogLevel": {
            "Default": "Warning",
            "Your.Namespace": "Information"
        },
        "ApplicationInsights": {
            "LogLevel": {
                "Default": "Warning",
                "Your.Namespace": "Information"
            }
        }
    }
```

## ASP.NET Core Web API Background Service

Follow these sections:

-   [https://learn.microsoft.com/en-us/azure/azure-monitor/app/worker-service#ilogger-logs](https://learn.microsoft.com/en-us/azure/azure-monitor/app/worker-service#ilogger-logs)
-   [https://learn.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core?tabs=netcorenew#how-do-i-customize-ilogger-logs-collection](https://learn.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core?tabs=netcorenew#how-do-i-customize-ilogger-logs-collection)
