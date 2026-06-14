---
layout: post
title: 'Classic Problems Have Classic Solutions'
date: 2026-03-05 20:34:52
permalink: /classic-problems-have-classic-solutions/
subtitle: 'TRIZ shows that many hard problems reduce to recurring contradictions with recurring solution patterns.'
---

In 2006, sitting in my 2nd Bachelor year civil engineering in the course "Productinnovatie & IndustriEle Marketing", by prof. [Joost Duflou](https://www.kuleuven.be/wieiswie/nl/person/00016263), I encountered an idea that quietly rewired how I think. He drew a simple diagram:

```
Domain-specific problem  ->  Generalized (abstract) problem
                                         v
Domain-specific solution <-  TRIZ suggestions
```

![](/assets/posts/classic-problems-have-classic-solutions/image-1.png)

*The gist*

That diagram stuck. It's been twenty years and I still think about it.

### What TRIZ Is

[TRIZ](https://en.wikipedia.org/wiki/TRIZ) (*Teoriya Resheniya Izobretatelskikh Zadach* - Theory of Inventive Problem Solving) was developed by Soviet engineer Genrich Altshuller, working at the USSR navy patent office from 1946. He analyzed hundreds of thousands of patents across engineering disciplines and found something striking: virtually all hard technical problems are instances of a small, finite set of universal contradictions.

The core tension TRIZ names is what it calls a **system conflict**: improving one property of a system degrades another. Reliability vs. complexity. Productivity vs. accuracy. Strength vs. formability. These feel unique and local when you're inside them. They're not.

Altshuller catalogued roughly **1,250 typical system conflicts**, mapping them to **39 engineering parameters** and showing that only **40 inventive principles** are needed to resolve them. The **contradiction matrix** is the practical tool: look up the parameter you want to improve and the one that suffers, and it tells you which principles have historically worked.

![](/assets/posts/classic-problems-have-classic-solutions/image-2.png)

*The TRIZ contradiction matrix*

![](/assets/posts/classic-problems-have-classic-solutions/image-3.png)

*The full contradiction matrix*

TRIZ also identified **technology evolution trends** - predictable trajectories that technical systems follow. The segmentation trend, for instance, runs from monolithic solid -> segmented -> granules -> powder -> aerosol. Sugar products follow it. Measuring tools follow it. Shoe soles follow it. Knowing where your technology sits on that curve tells you where it's headed.

![](/assets/posts/classic-problems-have-classic-solutions/image-4.png)

*TRIZ segmentation trend applied to sugar products*

### TRIZ is everywhere

Twenty years later I'm a software architect, and the abstraction bridge shows up everywhere.

The CAP theorem is pure TRIZ: you want to improve consistency, but that degrades availability under partition. It's a system conflict. The theorem doesn't give you a solution - it gives you a *class* of problem, and decades of distributed systems research have filled in the 40 inventive principles for that specific contradiction. When I recognize a caching design as a consistency vs. availability trade-off, I've already stepped into the abstract problem space. The solution library is right there.

The same pattern runs through much of the catalogue of [design patterns](https://en.wikipedia.org/wiki/Design_Patterns). **Mediator** resolves the contradiction between component autonomy and system coordination. The **Circuit Breaker** resolves availability vs. resilience. **CQRS** resolves the tension between write consistency and read performance by separating the two concerns entirely. The **Saga pattern** navigates distributed transaction consistency vs. availability. These patterns aren't clever tricks - they're named solutions to named contradictions, which is exactly the TRIZ philosophy made concrete.

And it's not just old problems. Consider DeepSeek R1 achieving essentially the same reasoning quality as OpenAI's o1, but at a fraction of the energy and compute cost: reasoning capability vs. resource consumption. DeepSeek resolved it by applying two TRIZ principles in combination. **Principle 1 (Segmentation)** - through a Mixture of Experts architecture, only the relevant subset of the network activates per token, so you get full capability without burning the full model every time. **Principle 10 (Prior Action)** - through knowledge distillation, the model pre-loads reasoning patterns from a stronger teacher model rather than discovering them from scratch through expensive training. The conflict isn't solved by brute force. It's resolved by changing the structure of the problem - exactly what TRIZ prescribes.

That's the quote I keep coming back to: **classic problems have classic solutions**.
