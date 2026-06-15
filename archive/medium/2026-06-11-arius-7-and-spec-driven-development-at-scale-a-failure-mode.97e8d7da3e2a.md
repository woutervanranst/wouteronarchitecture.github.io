Title: Arius 7 and Spec-Driven Development at scale: a failure mode

URL Source: http://wouteronarchitecture.medium.com/arius-7-and-spec-driven-development-at-scale-a-failure-mode-97e8d7da3e2a

Published Time: 2026-06-11T14:07:16Z

Markdown Content:
[![Image 1: Wouter on Architecture](https://miro.medium.com/v2/resize:fill:32:32/1*AoLeduUi-W4NsYodQ8ERMA.jpeg)](https://wouteronarchitecture.medium.com/?source=post_page---byline--97e8d7da3e2a---------------------------------------)

8 min read

3 days ago

_A follow-up to_[_Developing Arius 5 during the dawn of AI-assisted code_](https://wouteronarchitecture.medium.com/developing-arius-5-during-the-dawn-of-ai-assisted-code-473fdb05b1e6)_._

TL;DR:

*   Spec Driven Development got me from idea to working legacy rewrite shockingly fast: five years of hand-built feature parity in roughly a month.
*   But the generated code was maintainability hell: strings everywhere, duplicate helpers everywhere, green tests masking architectural rot.
*   The missing piece is not “better specs” alone; it is domain judgment: knowing when to introduce the right domain concepts, when to refactor, and what code smells matter across the whole codebase.

Since I discovered OpenSpec back in January, I’m a fanboy of Spec Driven Development. So to find out where it breaks, I spent the past few months pointing it at my own pet project and trying to do a full agentic rewrite: [Arius 7](https://github.com/woutervanranst/Arius7), built with OpenCode, vanilla OpenSpec, GitHub Copilot (first Opus 4.6 + Sonnet 4.6 but later I switched to GPT 5.4 for the >400k context window).

It’s a “legacy modernization”, if you want ;) The nice thing about doing it on my own codebase is that I know **exactly** what I want the result to be. For context: Arius is a small archival tool that uses the cheap offline/archive tier of Azure Blob Storage — point it at My Documents, get a backup. The rewrite landed at ~12k lines of production code plus an ~18k-line test suite at ~80% coverage.

Short version of what I found: SDD got me further than I expected, and then it walked me straight into a wall.

## **SDD is magic**

The initial change flowed out of a 2 hour `/opsx-explore`[session](https://github.com/woutervanranst/Arius7/blob/master/openspec/changes/archive/2026-03-24-arius-core-foundation/REQUIREMENTS-Discussion.md) where I described what I wanted to build (I started it off as a ‘[restic](https://restic.net/) rewrite for Azure Archive tier’, because restic doesn’t support that and I assume the training set contained more about restic than Arius5 ;)). It correctly identified some of the tensions I encountered during the past 5 years of ‘manual’ code writing (see below) and we made something **better**.

When the spec was finalized, I pressed enter, it goes _whirrrr_ — AND TWO HOURS LATER **IT WORKS**. It got the basics right. I did a couple more changes and after about a month I had feature parity with something I’d been building on-and-off for five years. I was blown away.

## **And then I looked at the code**

*   Strings. Strings everywhere.
*   Helper methods. Helper methods everywhere. Duplicate helper methods galore.
*   Duplicate code, weird names, a 2 GB file loaded into a byte[] like memory is cheap these days.

It compiled, the tests were green, round-trip backup/restore worked, but making manual changes to the code; there was no end in sight.

As an example: during my `explore` sessions I hinted at some of the domain concepts I introduced during my manual code writing (I’d get the ‘that’s an awesome idea’-validation) but I deliberately left out one of the more useful domain concepts in my codebase: `RelativePath`. I’d introduced it years ago in the hand-written version because the alternative is misery. Most File/Path operations in native C# are string-based, so without that concept you end up with paths-as-strings smeared across the whole codebase. Which is exactly what happened. The agents **never came up with the idea themselves**(because each helper is benign, but on a repository level it’s death by a thousand cuts). At least 35 helper methods to normalize a path: 17 of them assume a trailing slash (`My Documents\Pictures\)`, 10 assume no trailing slash (`My Documents\Pictures`), and 8 of them normalize them first. IT WORKS. Nobody can maintain it.

The domain concepts I _did_ hand the agent during prompting are sitting in there nice and clean (“That’s a brilliant idea and simplifies things a lot!”). The ones I didn’t think to mention never got invented.

## **A first diagnosis**

My provisional conclusion is that **fully unattended SDD workflows are missing a _guardrail_**— something that introduces the right domain concept at the right moment, with the effortful refactor that should come with it.

I augmented my setup with [Superpowers](https://github.com/obra/superpowers), which does TDD, but it stops at red → green. The **refactor** step of red → green → refactor is where a domain concept like `RelativePath`would normally get extracted, and that’s the step that quietly goes missing.

## **Squeezing the toothpaste back into the tube**

Regardless, I had ‘[IKEA effect](https://en.wikipedia.org/wiki/IKEA_effect)’ and wanted to salvage the Arius 7 codebase, and wanted get the strings/helpers/duplication back under control with a [ralph](https://github.com/Th0rgal/open-ralph-wiggum) loops. While it looks simple (“strongly type all the primitive types, make no mistakes”) it’s trickier than you’d think. It overdid it until basically EVERY primitive became strongly typed. It was like the codebase was reduced to [paperclips](https://en.wikipedia.org/wiki/Instrumental_convergence).

In retrospect, I should have regenerated the code based on the specs with the updated guidance. Hindsight 101.

## **OpenSpec + Superpowers: not sold on the combo**

I started with OpenSpec and added Superpowers along the way because everybody kept raving about it. I’m not convinced by the combination, and there’s surprisingly little written about it online (this instigated this post).

My impression is that the brainstorm/explore and the writing-plans steps get in each other’s way. I prefer OpenSpec, mostly because of the `/opsx-archive` it folds the various change specs back into one coherent set of master specs (hindsight: more on that later), so specs and code stay aligned over time. Superpowers’ plans feel like _plan mode on steroids_ — useful in the moment, but they don’t fold back coherently, so I don’t see the long-term value in keeping them around in the repo.

The TDD workflow also goes too far now and then. Before Superpowers would touch my test suite, it wanted a failing test first — so at one point I had a test suite for my test suite (eg. a red test that says that a test method argument should not be a string)… (_sigh_) Got that out with another ralph loop: _“remove all tests that are not testing production code.”_

## **When do I trust this code [with my childhood memories]?**

Here’s the one that got under my skin. The code ‘looks’ like it works, but these are _real_ files I’d like to keep safe. The question that kept nagging me:

> When do I trust this codebase with eg. my childhood photos if I don’t fully _get_ the code myself?

I suspect a lot of enterprises are sitting with some version of that exact question.

## Get Wouter on Architecture’s stories in your inbox

Join Medium for free to get updates from this writer.

Remember me for faster sign in

Where I landed: a fat _behavior_ test suite that runs a handful of scenarios “as it really happens” — archive → restore. I’ve mentally assimilated and verified that test suite. I understand and trust what it does, so I now feel safe about the, even if it’s black to me. Trusting the behavior is a different thing from trusting the code, and for now that’s the lever I have.

Press enter or click to view image in full size

![Image 2](https://miro.medium.com/v2/resize:fit:700/1*Yt1UDzStJpQZX6CceyCWuQ.png)

Source: Anthropic: [Vibe coding in prod](https://www.youtube.com/watch?v=fHWFF_pnqDk&t=414s)

## **Humans & AI, better together**

It’s not all bad. One of the genuine pain points in Arius 5 was a scalability problem on large archives: change one file, and I’d produce a whole new “state” of the entire folder to keep track of, which at scale was an up/download of ~500 MB. Not exactly ideal for responsiveness.

I handed that problem to openspec explore, and _together_ we worked our way to a [Merkle-tree](https://en.wikipedia.org/wiki/Merkle_tree)-style implementation that I would never have pulled off on my own.

## **Local optima**

The above is an example of agents writing brilliant code. I didn’t know Merkle trees, didn’t know that I didn’t know them, but the agents sniffed out of the _pattern_, gave it a name and we ran with it. But it was a local optimum in a rather contained part of the codebase.

The broader smell (like RelativePath) is smeared out over the full codebase as low noise. It doesn’t fully fit in the context window, so it doesn’t hurt the agent as much. They tend to propose **local optima:**solutions that are optimal _within their context window_ — without proactively going to find the context they’d actually need to do better. I think that’s an engineerable problem (a ‘software craftsmanship’ review skill, DDD skill, AST-magic, static code analysis, …). But the current SDD frameworks just don’t do enough about it yet.

Someone else pointed me at the [code-simplifier](https://github.com/anthropics/claude-plugins-official/blob/main/plugins/code-simplifier/agents/code-simplifier.md) skill, but that lands you right back in local-optima territory.

## **The domain-knowledge advantage**

Which is exactly the catch. My advantage on this project is that I **know** the domain, so I can steer the agent — feed it RelativePath and other good concepts, sniff out which refactors are needed.

For a (to me) _unknown_ domain, or a genuinely fully-hands-off-agentic setup, I don’t yet see how we close that gap — and a few of the experts I’ve talked to about this don’t see an obvious fix either. That’s a real risk for new contexts that lack (human) domain experts. Sharpening the spec is only part of the answer, because a spec only gets you so far; the _situational awareness of the code_ is what lets you smell where a refactor belongs, and that’s the part that isn’t in the spec.

Asking the right questions is, and stays, the whole game.

The Rick Rubin meme gets more relevant by the day.

![Image 3](https://miro.medium.com/v2/resize:fit:640/0*FQU_nyDiKzVf13TK)

## **So what might actually fix this?**

For this failure mode, how things evolve and past evolutions, I expect a fix for this to show up in the next 3–6 months. It feels like a tractable, “engineerable” gap rather than a fundamental one.

There are two distinct aspects, and it’s worth keeping them apart:

1.   **Design it properly** — make sure the right domain concepts get introduced in the first place. Still unsolved, as far as I can tell. Perhaps a ‘domain driven design’ skill might work, but it currently feels like a chicken-or-the-egg problem.
2.   **Keeping the toothpaste in the tube** — stop the string/helper/duplicate sprawl from creeping back. This one is more approachable. I set up a hard guardrail with [NetArchTest](https://github.com/BenMorris/NetArchTest). I do use it — but I find it pretty verbose, fairly rigid, and not all that expressive, and it doesn’t really fit the agentic way of working. How do you even encode _“too many helper methods”_ as a NetArchTest rule? I’ll tackle this in the coming weeks with static code analysis (SonarQube or Aikido).

## **A separate gripe: spec drift in OpenSpec**

The promise of SDD is that the code is _grounded in_ the spec. The code becomes a generated artifact and both are always in sync with each other.

But after making follow up changes in a change, eg. after a CodeRabbit review, the code drifts from the spec. Caveat emptor — I though the sync command in OpenSpec did exactly that: update the spec based on the actual code. But that is not what it does.

Maybe I’d need to make a custom OpenSpec flow that grounds before syncing (I took a [first swing at it](https://github.com/woutervanranst/Arius7/blob/master/.opencode/commands/opsx-ground-spec.md), the idea being a “clairvoyant” spec that reads as if it had always perfectly described the implementation that exists today) but too little/too late it made me realize after 3 months that my previous specs are essentially worthless. Exit SDD?

NOTE: A colleague pointed me to [Living Specs vs Static Specs](https://www.augmentcode.com/guides/living-specs-vs-static-specs); coming to all SDD frameworks soon?

## **A philosophical note to end on**

Does all of the above even matter if the functional and non-functional requirements are met? Similarly, do we care if compiler generated code is ‘well crafted’?

For the latter — no.

For the former — maybe.

The counterargument is that the garbage and the hallucinations [_accelerate_](https://circleci.com/blog/dora-ai-amplifier/). Let the string soup and the 35 normalize-helpers compound across a real codebase and you eventually hit the wall I hit — so keeping the toothpaste in the tube genuinely matters. For now. Maybe that’s a problem stronger models just dissolve.

I don’t know yet. That’s kind of the point.
