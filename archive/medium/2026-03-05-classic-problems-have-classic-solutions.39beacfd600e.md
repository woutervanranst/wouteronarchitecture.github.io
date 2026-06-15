Title: Classic Problems Have Classic Solutions

URL Source: http://wouteronarchitecture.medium.com/classic-problems-have-classic-solutions-39beacfd600e

Published Time: 2026-03-05T20:34:52Z

Markdown Content:
## What TRIZ Is

[TRIZ](https://en.wikipedia.org/wiki/TRIZ) (_Teoriya Resheniya Izobretatelskikh Zadach_ — Theory of Inventive Problem Solving) was developed by Soviet engineer Genrich Altshuller, working at the USSR navy patent office from 1946. He analyzed hundreds of thousands of patents across engineering disciplines and found something striking: virtually all hard technical problems are instances of a small, finite set of universal contradictions.

The core tension TRIZ names is what it calls a **system conflict**: improving one property of a system degrades another. Reliability vs. complexity. Productivity vs. accuracy. Strength vs. formability. These feel unique and local when you’re inside them. They’re not.

## Get Wouter on Architecture’s stories in your inbox

Join Medium for free to get updates from this writer.

Remember me for faster sign in

Altshuller catalogued roughly **1,250 typical system conflicts**, mapping them to **39 engineering parameters** and showing that only **40 inventive principles** are needed to resolve them. The **contradiction matrix** is the practical tool: look up the parameter you want to improve and the one that suffers, and it tells you which principles have historically worked.

Press enter or click to view image in full size

![Image 1](https://miro.medium.com/v2/resize:fit:700/0*8FlK02owrMnxcbFd.png)

The TRIZ contradiction matrix

Press enter or click to view image in full size

![Image 2](https://miro.medium.com/v2/resize:fit:1000/0*OGY7T4JXY7Z_-PG6.png)

The full contradiction matrix

TRIZ also identified **technology evolution trends** — predictable trajectories that technical systems follow. The segmentation trend, for instance, runs from monolithic solid → segmented → granules → powder → aerosol. Sugar products follow it. Measuring tools follow it. Shoe soles follow it. Knowing where your technology sits on that curve tells you where it’s headed.

Press enter or click to view image in full size

![Image 3](https://miro.medium.com/v2/resize:fit:700/1*wpKllzdr-g_s2wJhrw4LHg.png)

TRIZ segmentation trend applied to sugar products

## TRIZ is everywhere

Twenty years later I’m a software architect, and the abstraction bridge shows up everywhere.

The CAP theorem is pure TRIZ: you want to improve consistency, but that degrades availability under partition. It’s a system conflict. The theorem doesn’t give you a solution — it gives you a _class_ of problem, and decades of distributed systems research have filled in the 40 inventive principles for that specific contradiction. When I recognize a caching design as a consistency vs. availability trade-off, I’ve already stepped into the abstract problem space. The solution library is right there.

The same pattern runs through much of the catalogue of [design patterns](https://en.wikipedia.org/wiki/Design_Patterns). **Mediator** resolves the contradiction between component autonomy and system coordination. The **Circuit Breaker** resolves availability vs. resilience. **CQRS** resolves the tension between write consistency and read performance by separating the two concerns entirely. The **Saga pattern** navigates distributed transaction consistency vs. availability. These patterns aren’t clever tricks — they’re named solutions to named contradictions, which is exactly the TRIZ philosophy made concrete.

And it’s not just old problems. Consider DeepSeek R1 achieving essentially the same reasoning quality as OpenAI’s o1, but at a fraction of the energy and compute cost: reasoning capability vs. resource consumption. DeepSeek resolved it by applying two TRIZ principles in combination. **Principle 1 (Segmentation)** — through a Mixture of Experts architecture, only the relevant subset of the network activates per token, so you get full capability without burning the full model every time. **Principle 10 (Prior Action)** — through knowledge distillation, the model pre-loads reasoning patterns from a stronger teacher model rather than discovering them from scratch through expensive training. The conflict isn’t solved by brute force. It’s resolved by changing the structure of the problem — exactly what TRIZ prescribes.

That’s the quote I keep coming back to: **classic problems have classic solutions**.
