---
layout: post
title: 'Developing Arius 5 during the dawn of AI assisted code'
date: 2025-11-06 07:05:20
permalink: /developing-arius-5-during-the-dawn-of-ai-assisted-code/
subtitle: 'A reflection on rewriting Arius with modern .NET practices and on how copy-paste AI help evolved into full coding-agent workflows.'
---

![](/assets/posts/developing-arius-5-during-the-dawn-of-ai-assisted-code/image.png)

End of september, I finally did it. I said goodbye to my ~3 year old codebase of Arius 3 and [merged](https://github.com/woutervanranst/Arius/pull/74) the new [v5 code](https://github.com/woutervanranst/Arius/releases/tag/v5.0.112) into main. ([v4](https://github.com/woutervanranst/Arius/tree/feat/arius4-latest) was a botched attempt at a pure clean architecture but that suffered from the [second-system effect](https://en.wikipedia.org/wiki/Second-system_effect), was too rigid, so I [skipped](https://en.wikipedia.org/wiki/Winamp#Winamp_5) a [version](https://en.wikipedia.org/wiki/Internet_Stream_Protocol)).

I initially started the rewrite about 1.5 years ago as my insights about how modern .NET code is written had evolved, and I wanted to give it a spin. It has been an on-and-off effort during that time and during those 2 years, how code is coded changed dramatically.

This blog post is about the whys and hows of it.

## The old days of writing code

The first versions of Arius were written before the ChatGPT launch of November 2022. I had limited experience in writing larger .NET codebases, but it always itched. Arius was my pet project, a stab at making a larger, production-ready codebase and eating my own dog food.

I learned to write proper C# code through the first version of Arius, and so I am very grateful to that old codebase. I spent days trying to debug and understand how `async/await` works, how concurrency works, how exceptions in async context work, inheritance, and more.

## The emergence of AI generated code

Serendipitously, in November 2022, I was working closely with somebody who was deeply involved with the cutting edge of AI, so I was there when it happened. The day after ChatGPT launched we were using it to write code.

It was magical.

But after a while you start to learn the quirks of the system, and looking back the experience was characterized by the small context window and by being very hit-or-miss. Either it worked and it was nice, or it didn't. If you were lucky it didn't compile, but there were nasty bugs that were really hard to pin down because you didn't write or fully understand the code in the first place.

This improved dramatically with the launch of ChatGPT 4 in March 2023. Problems that 3.5 couldn't handle, ChatGPT 4 breezed through if it fit the context window. My [`MermaidGraphBuilder.cs`](https://github.com/woutervanranst/utils/blob/0a8081037a9f2174ad07dc216dca45028b5cc64b/src/WouterVanRanst.Utils/Builders/MermaidGraphBuilder.cs) stems from that time and is probably my first vibe-coded thing: to this day I don't fully understand why it works, but it does.

All in all, AI generated code was still a thing on the side and separate from my main dev workflow.

## The DeepSeek moment

In January 2025, DeepSeek took the world by storm. Simulated reasoning models already existed with o1, but were very rate limited and the reasoning chain was not visible. DeepSeek blew this all open and it was the first time we could see the model think. It had a 128k context window and was available for free.

As the world jumped on it and they didn't have the capacity, I discovered [OpenRouter](https://openrouter.ai/). Before, I was tied to the ChatGPT interface or the DeepSeek interface, but with OpenRouter it clicked how the models were different from the chat interface.

After that watershed moment, my coding workflow started to change, as the context windows grew increasingly large and the models increasingly powerful enough to handle multiple classes and rather complex refactors and features. However, it remained largely copy-from-Visual-Studio-into-the-browser-and-paste-the-response-back.

## The advent of the Coding Agent

I had heard of Cursor but as it didn't support .NET I didn't really dive into it further. I found Copilot in VS and VS Code slow, cumbersome and not that good, so I preferred the copy-paste way of working.

In June 2025 I decided to give [Cline](https://cline.bot/) a spin to see what all the fuss was about. In combination with my OpenRouter account I discovered the way of working that was taking the world by storm. Instead of copy-pasting, the changes were made right there in the IDE. You could see the LLM fetching the required context, reasoning and making changes.

> *Promising but expensive, and not that good*

The enthusiasm subsided somewhat after a couple of day-long programming stints; I used [Gemini 2.5 Flash](https://openrouter.ai/google/gemini-2.5-flash) on the cheap and the results were meh. What we later came to call AI slop. It wrote a lot of code that was mostly besides the point. And yet it was still expensive. Until then, I had used the flat-fee ChatGPT subscription of 20 USD per month and in one day I already blew through 10 USD.

In retrospect, I think that choosing a more powerful model at the time would have yielded better results, but I wasn't emotionally ready to pay big bucks.

## Then came Claude

[Claude Code was introduced in February 2025](https://www.youtube.com/watch?v=AJpK3YTTKZ4) and the hype really got underway after Anthropic launched the Max plan for 100 USD or 200 USD per month. Late August I couldn't hold it any longer. I felt like I was missing out.

This was it; this was the culmination of where I had seen the industry evolving since the DeepSeek moment. Based on the best practices of some coworkers, I configured [Serena](https://github.com/oraios/serena) and [Context 7](https://github.com/upstash/context7) MCP servers. In contrast to the earlier Cline experience, it was spot on nearly all the time.

My new workflow became Visual Studio in a large window, with Claude Code CLI in the terminal next to it and doing frequent, small commits and reverting when it was going in the wrong direction. At first, I prompted for small tasks - like I had to do with my earlier copy/paste workflow - but quickly I felt it could also take on larger tasks across multiple files.

-   [Replace FluentAssertions by Shouldly](https://github.com/woutervanranst/Arius/commit/ae791ba995d97f5fb4bde934bdb6ded1b7c3eee2) in my testing suite
-   [Replace MediatR by Mediator](https://github.com/woutervanranst/Arius/commit/01c9d9fab12ffb3235fa0fcb5dfa605703c13f4c) in `Arius.Core`

These are both low-value and nasty refactors that used to be sources of accumulating technical debt.

## Vibe (?) coding WPF

`Arius Explorer` is a rather simple WPF application I wrote in 3 days back in 2023. I love XAML and the declarative nature of it, but I always struggle to get the syntax right. Since I had gutted the core Arius library, it broke Arius Explorer quite hard so I went for a full rewrite.

I prompted Claude to take a good look at the existing Views and ViewModels and it just one-shotted the [RepositoryExplorerView](https://github.com/woutervanranst/Arius/commit/a0c9ad5ef4e857daf9ee99714b29fa49b2950ec3) and the [ChooseRepositoryView](https://github.com/woutervanranst/Arius/commit/e08a9ab6b765f549a61471a2c855c9651b2ce382). They were right there and they just worked.

> *Most of Arius 5 is AI written, but 100% checked by a human.*

However, this isn't vibe coding. I still care about the code, and I still very much understand what is going on, but I do see the slippery slope.

This is the long list of topics I wanted to properly integrate in this blog post, but never came around to:

-   I have a love/hate relationship with tests; evolving the test suite as the codebase grew larger was a joy - especially the repetitive and verbose parts that really nail down every execution branch of a complex method.
-   A novel concept: [Codex](https://openai.com/index/introducing-codex/), an agent in the cloud, rather autonomously writing a broader test suite.
-   Editing `CI.yml` workflows becomes a breeze, often one-shotting what is needed.
-   Arius 5 coding innovations over v3:
    -   Modern .NET: [Mediator](https://github.com/martinothamar/Mediator) for loose coupling instead of a facade pattern.
    -   AI helps with the syntax; I know what I want but I struggle with the syntax.
    -   Getting the right abstraction level is crucial; I searched a lot for a proper File/Directory/FileSystem abstraction.
    -   Eventually landed with [Zio](https://github.com/xoofx/zio); it's platform independent and avoids string path misery everywhere.
    -   The `FilePair` is a major innovation that helps a lot in the archive pipeline and simplifies indexing and ZIP dynamics.
    -   `HandlerContext` in the handlers is an innovation that I like because it benefits maximally from DI for parameters only known after CLI parsing.
