---
layout: post
title: 'You will be assimilated: Microsoft implementations of formerly popular OSS libraries'
date: 2025-01-05 07:23:02
permalink: /you-will-be-assimilated-microsoft-implementations-of-formerly-popular-oss-libraries/
---

<p>The below is a summary of all the libraries Nick mentions in his video.</p>

<!--more-->

<figure class="wp-block-embed is-type-video is-provider-youtube wp-block-embed-youtube wp-embed-aspect-16-9 wp-has-aspect-ratio"><div class="wp-block-embed__wrapper">
https://www.youtube.com/watch?v=PiT-441KR3s
</div></figure>

<figure class="wp-block-table alignwide"><table class="has-fixed-layout"><thead><tr><th><strong>Purpose</strong></th><th><strong>Open-Source Package</strong></th><th><strong>Microsoft Alternative</strong></th></tr></thead><tbody><tr><td>Messaging/Eventing framework</td><td><a href="https://github.com/MassTransit/MassTransit"><strong>MassTransit</strong></a><br><a href="https://github.com/JasperFx/wolverine"><strong>Wolverine</strong></a><br><br>(NServiceBus, Rebus too but they are not mentioned in the video)</td><td><a href="https://github.com/dotnet/aspnetcore/issues/53219">Upcoming Microsoft Eventing Framework (possibly in .NET 10)</a></td></tr><tr><td>Lightweight Web Framework for building APIs</td><td><a href="https://github.com/NancyFx/Nancy"><strong>Nancy FX</strong></a> <em>(Archived)</em><br><a href="https://github.com/CarterCommunity/Carter"><strong>Carter</strong></a></td><td><a href="https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis">ASP.NET Core Minimal APIs</a></td></tr><tr><td>OpenAPI Documentation Generation</td><td><a href="https://github.com/domaindrivendev/Swashbuckle.AspNetCore"><strong>Swashbuckle (Swagger)</strong></a></td><td><a href="https://learn.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger">ASP.NET Core OpenAPI Support</a></td></tr><tr><td>JSON Serialization</td><td><a href="https://github.com/JamesNK/Newtonsoft.Json"><strong>Newtonsoft.Json</strong></a></td><td><a href="https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-overview">System.Text.Json</a></td></tr><tr><td>Dependency Injection Container</td><td><a href="https://github.com/autofac/Autofac"><strong>Autofac</strong></a><br><a href="https://github.com/JasperFx/lamar"><strong>Lamar</strong></a><br><a href="https://github.com/ninject/Ninject"><strong>Ninject</strong></a><br><a href="https://github.com/unitycontainer/unity"><strong>Unity (DI Framework)</strong></a></td><td><a href="https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection">Microsoft.Extensions.DependencyInjection</a></td></tr><tr><td>Hybrid (Local + Distributed) Caching Library</td><td><a href="https://github.com/jodydonetti/FusionCache"><strong>FusionCache</strong></a></td><td><a href="https://learn.microsoft.com/en-us/aspnet/core/performance/caching/overview">Microsoft Hybrid Cache (Upcoming in ASP.NET Core)</a></td></tr><tr><td>Actor Model Framework</td><td><a href="https://github.com/akkadotnet/akka.net"><strong>Akka.NET</strong></a></td><td><a href="https://learn.microsoft.com/en-us/dotnet/orleans/">Orleans</a></td></tr></tbody></table></figure>

<p></p>
