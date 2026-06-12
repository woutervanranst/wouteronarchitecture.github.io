---
layout: post
title: 'You will be assimilated: Microsoft implementations of formerly popular OSS libraries'
date: 2025-01-05 07:23:02
permalink: /you-will-be-assimilated-microsoft-implementations-of-formerly-popular-oss-libraries/
---

The below is a summary of all the libraries Nick mentions in his video.

[https://www.youtube.com/watch?v=PiT-441KR3s](https://www.youtube.com/watch?v=PiT-441KR3s)

| **Purpose** | **Open-Source Package** | **Microsoft Alternative** |
| --- | --- | --- |
| Messaging/Eventing framework | [**MassTransit**](https://github.com/MassTransit/MassTransit)  <br>[**Wolverine**](https://github.com/JasperFx/wolverine)  <br>  <br>(NServiceBus, Rebus too but they are not mentioned in the video) | [Upcoming Microsoft Eventing Framework (possibly in .NET 10)](https://github.com/dotnet/aspnetcore/issues/53219) |
| Lightweight Web Framework for building APIs | [**Nancy FX**](https://github.com/NancyFx/Nancy) *(Archived)*  <br>[**Carter**](https://github.com/CarterCommunity/Carter) | [ASP.NET Core Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis) |
| OpenAPI Documentation Generation | [**Swashbuckle (Swagger)**](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) | [ASP.NET Core OpenAPI Support](https://learn.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger) |
| JSON Serialization | [**Newtonsoft.Json**](https://github.com/JamesNK/Newtonsoft.Json) | [System.Text.Json](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-overview) |
| Dependency Injection Container | [**Autofac**](https://github.com/autofac/Autofac)  <br>[**Lamar**](https://github.com/JasperFx/lamar)  <br>[**Ninject**](https://github.com/ninject/Ninject)  <br>[**Unity (DI Framework)**](https://github.com/unitycontainer/unity) | [Microsoft.Extensions.DependencyInjection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) |
| Hybrid (Local + Distributed) Caching Library | [**FusionCache**](https://github.com/jodydonetti/FusionCache) | [Microsoft Hybrid Cache (Upcoming in ASP.NET Core)](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/overview) |
| Actor Model Framework | [**Akka.NET**](https://github.com/akkadotnet/akka.net) | [Orleans](https://learn.microsoft.com/en-us/dotnet/orleans/) |
