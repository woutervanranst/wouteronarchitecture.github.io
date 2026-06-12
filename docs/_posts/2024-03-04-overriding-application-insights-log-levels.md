---
layout: post
title: 'Overriding Application Insights log levels'
date: 2024-03-04 07:02:00
permalink: /overriding-application-insights-log-levels/
---


<p>(This post is still draft as I work out a minimal example)</p>

<h2>Isolated Functions (.NET 8)</h2>

<p>From this docs page: <a href="https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=windows#configure-startup">https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=windows#configure-startup</a></p>

<pre><code>var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .<mark style="background-color:#ffeb00" class="has-inline-color">ConfigureServices</mark>(services =&gt; {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .ConfigureLogging(logging =&gt;
    {
        logging.Services.Configure&lt;LoggerFilterOptions&gt;(options =&gt;
        {
            <mark style="background-color:#ffeb00" class="has-inline-color">LoggerFilterRule defaultRule = options.Rules.FirstOrDefault(rule =&gt; rule.ProviderName
                == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");
            if (defaultRule is not null)
            {
                options.Rules.Remove(defaultRule);
            }</mark>
        });
    })
    .Build();

host.Run();</code></pre>

<p>Note the removal of the <code>defaultRule</code> <strong><em>after</em> </strong><code>ConfigureServices</code>.</p>

<p>Then, in your config.json (or whatever configuration provider you are referring), you can override the log level:</p>

<pre><code>    "Logging": {<br>        "LogLevel": {<br>            "Default": "Warning",<br>            "Your.Namespace": "Information"<br>        },<br>        "ApplicationInsights": {<br>            "LogLevel": {<br>                "Default": "Warning",<br>                "Your.Namespace": "Information"<br>            }<br>        }<br>    }</code></pre>

<h2>ASP.NET Core Web API Background Service</h2>

<p>Follow these sections: </p>

<ul>
<li><a href="https://learn.microsoft.com/en-us/azure/azure-monitor/app/worker-service#ilogger-logs">https://learn.microsoft.com/en-us/azure/azure-monitor/app/worker-service#ilogger-logs</a></li>

<li><a href="https://learn.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core?tabs=netcorenew#how-do-i-customize-ilogger-logs-collection">https://learn.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core?tabs=netcorenew#how-do-i-customize-ilogger-logs-collection</a></li>
</ul>

