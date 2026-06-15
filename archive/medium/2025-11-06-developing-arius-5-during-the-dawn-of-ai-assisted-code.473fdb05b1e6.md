Title: Developing Arius 5 during the dawn of AI assisted code

URL Source: http://wouteronarchitecture.medium.com/developing-arius-5-during-the-dawn-of-ai-assisted-code-473fdb05b1e6

Published Time: 2025-11-06T07:05:20Z

Markdown Content:
[![Image 1: Wouter on Architecture](https://miro.medium.com/v2/resize:fill:32:32/1*AoLeduUi-W4NsYodQ8ERMA.jpeg)](https://wouteronarchitecture.medium.com/?source=post_page---byline--473fdb05b1e6---------------------------------------)

8 min read

Nov 6, 2025

--

--

Press enter or click to view image in full size

![Image 2](https://miro.medium.com/v2/resize:fit:700/0*jgJq9wjgVdhEYWBa.png)

End of september, I finally did it. I said goodbye to my ~3 year old codebase of Arius 3 and [merged](https://github.com/woutervanranst/Arius/pull/74)the new [v5 code](https://github.com/woutervanranst/Arius/releases/tag/v5.0.112) into main. ([v4](https://github.com/woutervanranst/Arius/tree/feat/arius4-latest) was a botched attempt at a pure ‘clean architecture’ but that suffered from the [second-system effect](https://en.wikipedia.org/wiki/Second-system_effect) was too rigid so I [skipped](https://en.wikipedia.org/wiki/Winamp#Winamp_5) a [version](https://en.wikipedia.org/wiki/Internet_Stream_Protocol))

I initially started the rewrite about 1,5 years ago as my insights about how modern .NET code is written had evolved, and I wanted to give it a spin. It has been an on-and-off effort during that time and during those 2 years, how ‘code is coded’ has changed dramatically.

This blog post is about the why’s and how’s of it.

## The ‘old days’ of writing code

The first versions of Arius were written pre the ChatGPT launch of November ‘22. I had limited experience in writing ‘larger’ .NET codebases (I did other things as a profession) but it always itched. Arius was my ‘pet project’, to say it disrespectfully, at a stab of making a ‘larger, production-ready code base’ and eating my own dog food.

I ‘learned’ to write proper C# code through the first version of Arius, and so I am very grateful to that ‘old’ codebase. I spent days trying to debug/figure out (dare I say “understand”) how the intricacies of `async/await` work, how concurrency works, how exceptions in async context work, inheritance, plowing through StackOverflow and getting ghosted or flamed... so this image really strikes a nerve:

Press enter or click to view image in full size

![Image 3](https://miro.medium.com/v2/resize:fit:700/0*ULDV9RQyNIcIOeG2.png)

## The emergence of AI generated code

Serendipitously, in November 22, I was working closely with somebody who was deeply involved with the cutting edge of AI, so “I was there when it happened”, the day after ChatGPT launched we were using it to write code.

It was magical.

![Image 4](https://miro.medium.com/v2/resize:fit:640/0*z_2uYvtk7YnA8Y7Q.png)

But after a while you start to learn the quirks of the system, and looking back the experience was characterized by 1/ the small context window (4k tokens context window remember) and 2/ very much hit-or-miss. Either it worked and it was nice, or it didn’t. If you were lucky it didn’t compile, but there were nasty bugs that were really hard to pin down because you didn’t write/understand the code in the first place.

This improved dramatically with the launch of ChatGPT 4 in March ‘23. I experienced it as a ‘real jump’ in capability despite people claiming in hindsight it was only a moderate improvement. Problems that 3.5 couldn’t handle, ChatGPT 4 breezed right through them IF it fitted the context window (of 8k context window in the beginning). My `MermaidGraphBuilder.cs` stems from that time and is probably my first ~vibe~coded thing: to this day I don't fully understand why it works, but it does.

Context windows grew larger (with ChatGPT 4–32k, ~ September ‘23) somewhat but looking back it felt a bit like a plateau. All in all, AI generated code was very much a ‘thing on the side’ but very separate from my main dev workflow.

## The DeepSeek moment

In January ‘25, DeepSeek took the world by storm for [multiple](https://arstechnica.com/ai/2025/01/china-is-catching-up-with-americas-best-reasoning-ai-models/)[reasons](https://x.com/satyanadella/status/1883753899255046301)with their DeepSeek-R1 model and chatbot. ‘Simulated Reasoning’ models already existed with o1 (released September ‘24) but were very rate limited and the ‘reasoning chain’ was not visible. DeepSeek blew this all open and it was the first time we could ‘see’ the model ‘think’. It had a 128k context window and was available for free.

Press enter or click to view image in full size

![Image 5](https://miro.medium.com/v2/resize:fit:700/0*dDUWl82Rx_19iRos.png)

As the world jumped on it and they didn’t have the capacity, I discovered [OpenRouter.ai](http://openrouter.ai/). Before, I was ‘tied’ to the ChatGPT interface (or the DeepSeek interface) but with OpenRouter it ‘clicked’ how the models were different from the chat interface.

After that watershed moment, my coding workflow started to change, as the context windows grew increasingly large and the models increasingly powerful to handle multiple classes and rather complex refactors/features. However, it remained largely copy-from-visual-studio-into-the-browser-and-paste-the-response-back.

## The advent of the Coding Agent

I had heard of Cursor but as it didn’t support .NET I didn’t really dive into it further. I found Copilot in VS/VS Code slow, cumbersome and not that good so I preferred the copy-paste way of working.

## Get Wouter on Architecture’s stories in your inbox

Join Medium for free to get updates from this writer.

Remember me for faster sign in

In June ’25 I decided to give [Cline](https://cline.bot/) (in VS Code) a spin to see what all the fuss was about. In combination with my OpenRouter account I discovered a the way of working that was taking the world by storm. Instead of copy-pasting, the changes were made _right there in the IDE!_. You could see the LLM getting fetching the required context, reasoning and making changes. It was magical!

> _Promising but expensive, and not that good_

The enthusiasm subsided somewhat after a couple of day-long programming stints; I did it on the cheap and used [Gemini 2.5 Flash](https://openrouter.ai/google/gemini-2.5-flash) and the results were … meh. What we later came to call ‘AI slop’. It wrote ‘a lot of code’ that was mostly besides the point. And yet it was still “expensive”: until then, I had used the ‘flat fee’ ChatGPT subscription of 20 USD/m and in a one day bender I already blew through 10 USD. Furthermore, Cline seemed to stall at times, get stuck, hang Visual Studio; I gave Roo Code and Kilo Code a try but it was all same same but different.

Press enter or click to view image in full size

![Image 6](https://miro.medium.com/v2/resize:fit:700/0*Hy7TqiZyZBwPdgaG.png)

In retospect, I think that choosing a more powerful (expensive) model at the time would have yielded better results, but I wasn’t emotionally ready to pay big bucks.

## Then came Claude

[Claude Code was introduced in February ‘25](https://www.youtube.com/watch?v=AJpK3YTTKZ4) and the hype really got underway after Anthropic launched the Max plan for 100 USD or 200 USD per month. This was my reaction when [I read it back](https://arstechnica.com/ai/2025/04/anthropic-launches-200-claude-max-ai-plan-with-20x-higher-usage-limits/)in april ‘25:

![Image 7](https://miro.medium.com/v2/resize:fit:220/0*kn4gu5AJ_6MkUQ0P.png)

“What an insane amount of money”.

However, late august I couldn’t hold it any longer. I felt like I was missing out. I entered my credit card number, and just like that it happened:

![Image 8](https://miro.medium.com/v2/resize:fit:618/0*OGLHRQ3eXzX_MhWY.png)

This was it; this was the culmination of where I had seen the industry evolving towards since the DeepSeek moment. And it made sense. Based on the best practices of some coworkers, I configured configured [Serena](https://github.com/oraios/serena) and [Context 7](https://github.com/upstash/context7) MCP servers. In contrast to the earlier Cline experience, it was spot on nearly all the time.

My new workflow became Visual Studio in a large window, with Claude Code CLI in the terminal next to it and doing frequent, small commits and reverting when it was going in the wrong direction. At first, I prompted for small tasks — like I (had) to do with my earlier copy/paste workflow, but quickly I felt it could also take on larger tasks. Hell, even more daunting tasks across multiple files that would be very cumbersome in my old workflow:

*   [Replace FluentAssertions by Shouldly](https://github.com/woutervanranst/Arius/commit/ae791ba995d97f5fb4bde934bdb6ded1b7c3eee2) in my testing suite
*   [Replace MediatR by Mediator](https://github.com/woutervanranst/Arius/commit/01c9d9fab12ffb3235fa0fcb5dfa605703c13f4c) in Arius.Core

These are both ‘low value & nasty’ refactors that used to be sources of accumulating technical debt (since both libraries changed their licensing).

## Vibe (?) coding WPF

![Image 9](https://miro.medium.com/v2/resize:fit:700/0*AuNvaf4Nn-GZ4bmF.png)

Arius Explorer is a rather simple WPF application I wrote in 3 days back in 2023. I love XAML and the declarative nature of it, but I always struggle to get the syntax right. Since I had gutted the the core Arius library, it broke Arius Explorer quite hard so I went for a full rewrite.

I prompted Claude to take a good look at the existing Views and ViewModels and it just one-shotted the [RepositoryExplorerView](https://github.com/woutervanranst/Arius/commit/a0c9ad5ef4e857daf9ee99714b29fa49b2950ec3) and the [ChooseRepositoryView](https://github.com/woutervanranst/Arius/commit/e08a9ab6b765f549a61471a2c855c9651b2ce382). They were right there and they just worked. Dario Amodei’s quote of March 2025 came to mind, where he claimed that in 3–6 months, [AI will be writing 90% of the code](https://www.businessinsider.com/anthropic-ceo-ai-90-percent-code-3-to-6-months-2025-3). Looking back, it probably comes close.

> _Most of Arius 5 is AI written, but 100% checked by a human._

However, this isn’t “vibe coding”. I still care about the code, and I still very much understand what is going on but I see the slippery slope.

This is the long list of topics I wanted to properly integrate in this blog post, but never came around to:

*   I have a love/hate relationship with tests; evolving the test suite as the codebase grew larger was a joy — esp. the repetitive/verbose parts that really nail down every execution branch of a complex method was good to do TDD with.
*   A novel concept: [Codex](https://openai.com/index/introducing-codex/) (June ‘25), an ‘agent in the cloud’, rather autonomously write a broeder test suite.
*   The evolution of tokens on openrouter was going expontential as these coding agents became more powerful:

Press enter or click to view image in full size

![Image 10](https://miro.medium.com/v2/resize:fit:700/0*gb_fX5XFeiwtdUWw.png)

*   Despite the backlash at launch (August ’25) I found the model to be quite good. I used it in Codex (Web) mainly.
*   Editing CI.yml workflows becomes a breeze, often one-shotting what is needed [insert a picture of the typical endless series of `fix` commits]
*   Arius5 coding innovations over v3:

- Modern .NET: [mediator](https://github.com/martinothamar/Mediator) for loose coupling instead of facade pattern

- AI helps with the syntax; I know what I want but I struggle with the syntax

- Getting the right abstraction level is crucial; I searched a lot for a proper File/Directory/FileSystem abstraction (C# native is all strings)

- Eventually landed with [https://github.com/xoofx/zio](https://github.com/xoofx/zio); it’s platform independent (Linux is `/` whereas Windows is `\` leading slash, trailing slash, … I don’t want to deal with that _everywhere_).

- I asked ChatGPT for a proper abstraction and it suggested ZIO

- The `FilePair` is a major innovation that helps a lot in the archive pipeline, simplifies indexing and fan in/ZIP dynamics

- Arius4 had SpecFlow (now deprecated, was upgraded to ReqNRoll) but fuond the abstraction too cumbersome so did away with that

- `HandlerContext` in the handlers is an innovation that I like to benefit maximally from DI; a problem I struggled with in v3. It solves the problem that some dependencies are dependent on parameters we only know after CLI parsing (eg accountname, accountkey, …)
