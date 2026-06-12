---
layout: post
title: 'Choose Boring Technology'
date: 2025-06-11 08:17:50
permalink: /choose-boring-technology/
---

Like wine, (some) technology gets better with age. The blog [Choose Boring Technology](https://boringtechnology.club/) advocates to make technology choices as boring as possible, unless you deliberately choose or have room for the unknown. “Boring” is not a lack of ambition—it’s the deliberate, wise pursuit of excellence. Choose your “innovation tokens” wisely, and your future self will thank you.

I would have loved to see the talk in a YouTube video, but only the slides are available - so I made a longform instead. Enjoy the read.

In the world of software engineering, few things are as seductive as the promise of the new. Fresh frameworks, databases, languages, and infrastructure solutions flood tech news and conference talks. The underlying message? **Adopt the latest and greatest, or risk falling behind.** But for every team that leaps onto the new, there’s another quietly delivering value with so-called “boring” technology. Dan McKinley, in his seminal talk and essay “Choose Boring Technology,” champions the latter approach—not out of nostalgia or fear, but from hard-won experience and clear-eyed analysis.

What follows is not an argument against innovation, but a story about understanding the real cost of our choices. It’s about recognizing that software is as much about people, organizations, and attention as it is about lines of code. And it’s a call to rethink the urge to chase every new shiny tool.

---

## The Real Constraints: Attention, Not Hardware

McKinley’s thesis starts with a question many teams overlook: **What is truly scarce in your engineering organization?** Contrary to popular belief, it’s rarely CPU cycles or disk space. The scarcest resource is always **attention**—the focused, creative energy of the people who keep systems running and features shipping.

Much like Maslow’s hierarchy of needs, engineering organizations have a pyramid of priorities. At the base: keeping systems operational, secure, and reliable. Only when these are satisfied can developers focus on the “higher” needs—delivering product features, optimizing user experience, innovating in the business domain. When teams squander attention by endlessly revisiting basic infrastructure problems, they stall or even regress.

The lesson? **Every non-essential technology choice is a tax on your attention.** And when you’re taxed enough, you’re no longer shipping value—you’re just firefighting.

![](https://boringtechnology.club/slides/slides.054.jpeg)

## The Unknown Unknowns: When Novelty Bites Back

“Boring” technology is not without flaws, but its risks are known and manageable. By contrast, adopting new technology brings **unknown unknowns**—subtle bugs, untested edge cases, and poor documentation.

| Category | Known Tech (e.g., MySQL) | New Tech (e.g., Latest NoSQL DB) |
| --- | --- | --- |
| Failure Modes | Documented, predictable | Undocumented, sometimes catastrophic |
| Support Community | Large, active | Sparse, slow to respond |
| Tooling | Mature, stable | Immature, rapidly changing |
| Hiring | Easy, large talent pool | Difficult, niche skillset |
| Compliance/Security | Proven, best practices | Unproven, higher audit risk |

This doesn’t mean you should never innovate—but recognize that every new dependency is a bet with uncertain odds. The more dependencies, the more complex the system, the more attention you will pay for the privilege.

## The Cautionary Tale of Microservices Mania

A real-world example is the microservices boom of the mid-2010s. Many organizations, inspired by Netflix and Amazon, rushed to break monoliths into dozens of services, each potentially using its own stack. **For Netflix, with armies of SREs and deep pockets, this worked. For most companies, it led to operational chaos:** constant integration pain, fragile deployments, and a hiring bottleneck for each niche technology. In the end, many quietly reverted to a simpler architecture, having learned the cost of complexity the hard way.

## Shipping Value, Not Complexity

In a field obsessed with reinvention, “boring” is a badge of honor. It signals that your team is focused, disciplined, and free to spend its limited attention on what matters—delivering value to users and customers. When you embrace boring technology, you reclaim the ability to innovate where it counts: not in your tool choices, but in your product.

> Production Is Hard

![](https://boringtechnology.club/slides/slides.057.jpeg)

In McKinley’s words, **“Shipping things consistently is the path to developer happiness.”** The next time you feel the itch to adopt the latest and greatest, ask yourself: is this where my team’s attention belongs?
