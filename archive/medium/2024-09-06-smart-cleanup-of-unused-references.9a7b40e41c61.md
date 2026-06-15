Title: Smart cleanup of unused references

URL Source: http://wouteronarchitecture.medium.com/smart-cleanup-of-unused-references-wouter-on-architecture-9a7b40e41c61

Published Time: 2024-09-06T06:08:16Z

Markdown Content:
# Smart cleanup of unused references | by Wouter on Architecture | Medium

[Sitemap](http://wouteronarchitecture.medium.com/sitemap/sitemap.xml)

[Open in app](https://play.google.com/store/apps/details?id=com.medium.reader&referrer=utm_source%3DmobileNavBar&source=post_page---top_nav_layout_nav-----------------------------------------)

Sign up

[Sign in](https://medium.com/m/signin?operation=login&redirect=https%3A%2F%2Fwouteronarchitecture.medium.com%2Fsmart-cleanup-of-unused-references-wouter-on-architecture-9a7b40e41c61&source=post_page---top_nav_layout_nav-----------------------global_nav------------------)

[](https://medium.com/?source=post_page---top_nav_layout_nav-----------------------------------------)

Get app

[Write](https://medium.com/m/signin?operation=register&redirect=https%3A%2F%2Fmedium.com%2Fnew-story&source=---top_nav_layout_nav-----------------------new_post_topnav------------------)

[Search](https://medium.com/search?source=post_page---top_nav_layout_nav-----------------------------------------)

Sign up

[Sign in](https://medium.com/m/signin?operation=login&redirect=https%3A%2F%2Fwouteronarchitecture.medium.com%2Fsmart-cleanup-of-unused-references-wouter-on-architecture-9a7b40e41c61&source=post_page---top_nav_layout_nav-----------------------global_nav------------------)

![Image 6: Unknown user](https://miro.medium.com/v2/resize:fill:32:32/1*dmbNkD5D-u45r44go_cf0g.png)

# Smart cleanup of unused references

[![Image 7: Wouter on Architecture](https://miro.medium.com/v2/resize:fill:32:32/1*AoLeduUi-W4NsYodQ8ERMA.jpeg)](http://wouteronarchitecture.medium.com/?source=post_page---byline--9a7b40e41c61---------------------------------------)

[Wouter on Architecture](http://wouteronarchitecture.medium.com/?source=post_page---byline--9a7b40e41c61---------------------------------------)

Follow

2 min read

·

Sep 6, 2024

[](https://medium.com/m/signin?actionUrl=https%3A%2F%2Fmedium.com%2F_%2Fvote%2Fp%2F9a7b40e41c61&operation=register&redirect=https%3A%2F%2Fwouteronarchitecture.medium.com%2Fsmart-cleanup-of-unused-references-wouter-on-architecture-9a7b40e41c61&user=Wouter+on+Architecture&userId=ece60debe2fd&source=---header_actions--9a7b40e41c61---------------------clap_footer------------------)

[](https://medium.com/m/signin?actionUrl=https%3A%2F%2Fmedium.com%2F_%2Frepost%2Fp%2F9a7b40e41c61&operation=register&redirect=https%3A%2F%2Fwouteronarchitecture.medium.com%2Fsmart-cleanup-of-unused-references-wouter-on-architecture-9a7b40e41c61&user=Wouter+on+Architecture&userId=ece60debe2fd&source=---header_actions--9a7b40e41c61---------------------repost_header------------------)

[](https://medium.com/m/signin?actionUrl=https%3A%2F%2Fmedium.com%2F_%2Fbookmark%2Fp%2F9a7b40e41c61&operation=register&redirect=https%3A%2F%2Fwouteronarchitecture.medium.com%2Fsmart-cleanup-of-unused-references-wouter-on-architecture-9a7b40e41c61&source=---header_actions--9a7b40e41c61---------------------bookmark_footer------------------)

[Listen](https://medium.com/m/signin?actionUrl=https%3A%2F%2Fmedium.com%2Fplans%3Fdimension%3Dpost_audio_button%26postId%3D9a7b40e41c61&operation=register&redirect=https%3A%2F%2Fwouteronarchitecture.medium.com%2Fsmart-cleanup-of-unused-references-wouter-on-architecture-9a7b40e41c61&source=---header_actions--9a7b40e41c61---------------------post_audio_button------------------)

Share

Say you have a ‘large project’ (think: clean architecture) with 40+ projects that has gradually evolved over time, has been refactored etc. There may be unused references lying around.

## How do we get unused references?

There are many ways this happens accidentally, for example:

## Get Wouter on Architecture’s stories in your inbox

Join Medium for free to get updates from this writer.

Subscribe

Subscribe

- [x] 

Remember me for faster sign in

 

In the beginning, there was a Test project, which references Moq (a popular mocking library):

![Image 8](https://miro.medium.com/v2/resize:fit:60/0*y_F0anEL-ktGfZcI)

Say we decide to split the projects up. The 2nd Test project also requires a dependency on Moq, but also uses a base class in Test. We now have this situation:

![Image 9](https://miro.medium.com/v2/resize:fit:112/0*CMLgjXy-iYCVrx5R)

Now, say that we move all mocks to OtherTests but we forget to remove the Moq nuget from Test. It s still referenced, but no longer used:

![Image 10](https://miro.medium.com/v2/resize:fit:112/0*cyYFH2j0HBpsPTxz)

## Remove unused references — The naive Way

Visual Studio has a feature [to Remove Unused References](https://learn.microsoft.com/en-us/visualstudio/ide/reference/remove-unused-references?view=vs-2022):

![Image 11](https://miro.medium.com/v2/resize:fit:491/0*L7ZqXWK0G70M0e2_)

Since transitive dependencies are a thing, the order in which you do this is important.

## Remove unused references — The easy way

From [Finding .NET Transitive Dependencies and Tidying Up Your Project](https://guiferreira.me/archive/2022/finding-dotnet-transitive-dependencies-and-tidying-up-your-project/), use Snitch: [spectresystems/snitch: A tool that help you find duplicate transitive package references.](https://github.com/spectresystems/snitch)

[](https://medium.com/m/signin?actionUrl=https%3A%2F%2Fmedium.com%2F_%2Fvote%2Fp%2F9a7b40e41c61&operation=register&redirect=https%3A%2F%2Fwouteronarchitecture.medium.com%2Fsmart-cleanup-of-unused-references-wouter-on-architecture-9a7b40e41c61&user=Wouter+on+Architecture&userId=ece60debe2fd&source=---footer_actions--9a7b40e41c61---------------------clap_footer------------------)

[](https://medium.com/m/signin?actionUrl=https%3A%2F%2Fmedium.com%2F_%2Fvote%2Fp%2F9a7b40e41c61&operation=register&redirect=https%3A%2F%2Fwouteronarchitecture.medium.com%2Fsmart-cleanup-of-unused-references-wouter-on-architecture-9a7b40e41c61&user=Wouter+on+Architecture&userId=ece60debe2fd&source=---footer_actions--9a7b40e41c61---------------------clap_footer------------------)

[](https://medium.com/m/signin?actionUrl=https%3A%2F%2Fmedium.com%2F_%2Frepost%2Fp%2F9a7b40e41c61&operation=register&redirect=https%3A%2F%2Fwouteronarchitecture.medium.com%2Fsmart-cleanup-of-unused-references-wouter-on-architecture-9a7b40e41c61&user=Wouter+on+Architecture&userId=ece60debe2fd&source=---footer_actions--9a7b40e41c61---------------------repost_footer------------------)

[](https://medium.com/m/signin?actionUrl=https%3A%2F%2Fmedium.com%2F_%2Fbookmark%2Fp%2F9a7b40e41c61&operation=register&redirect=https%3A%2F%2Fwouteronarchitecture.medium.com%2Fsmart-cleanup-of-unused-references-wouter-on-architecture-9a7b40e41c61&source=---footer_actions--9a7b40e41c61---------------------bookmark_footer------------------)

[![Image 12: Wouter on Architecture](https://miro.medium.com/v2/resize:fill:48:48/1*AoLeduUi-W4NsYodQ8ERMA.jpeg)](http://wouteronarchitecture.medium.com/?source=post_page---post_author_info--9a7b40e41c61---------------------------------------)

[![Image 13: Wouter on Architecture](https://miro.medium.com/v2/resize:fill:64:64/1*AoLeduUi-W4NsYodQ8ERMA.jpeg)](http://wouteronarchitecture.medium.com/?source=post_page---post_author_info--9a7b40e41c61---------------------------------------)

Follow

## [Written by Wouter on Architecture](http://wouteronarchitecture.medium.com/?source=post_page---post_author_info--9a7b40e41c61---------------------------------------)

[1 follower](http://wouteronarchitecture.medium.com/followers?source=post_page---post_author_info--9a7b40e41c61---------------------------------------)

·[1 following](http://wouteronarchitecture.medium.com/following?source=post_page---post_author_info--9a7b40e41c61---------------------------------------)

Random musings mostly on (Cloud) Architecture, Azure and .NET

Follow

## No responses yet

[](https://policy.medium.com/medium-rules-30e5502c4eb4?source=post_page---post_responses--9a7b40e41c61---------------------------------------)

![Image 14: Unknown user](https://miro.medium.com/v2/resize:fill:32:32/1*dmbNkD5D-u45r44go_cf0g.png)

Write a response

[What are your thoughts?](https://medium.com/m/signin?operation=register&redirect=https%3A%2F%2Fwouteronarchitecture.medium.com%2Fsmart-cleanup-of-unused-references-wouter-on-architecture-9a7b40e41c61&source=---post_responses--9a7b40e41c61---------------------respond_sidebar------------------)

Cancel

Respond

## More from Wouter on Architecture

![Image 15: Arius 7 and Spec-Driven Development at scale: a failure mode](https://miro.medium.com/v2/resize:fit:679/format:webp/0*FQU_nyDiKzVf13TK)

[![Image 16: Wouter on Architecture](https://miro.medium.com/v2/resize:fill:20:20/1*AoLeduUi-W4NsYodQ8ERMA.jpeg)](http://wouteronarchitecture.medium.com/?source=post_page---author_recirc--9a7b40e41c61----0---------------------44dc9bad_0f86_4a40_9ef7_9583200cec4b--------------)

[Wouter on Architecture](http://wouteronarchitecture.medium.com/?source=post_page---author_recirc--9a7b40e41c61----0---------------------44dc9bad_0f86_4a40_9ef7_9583200cec4b--------------)

·

3d ago

## [Arius 7 and Spec-Driven Development at scale: a failure mode ### A follow-up to Developing Arius 5 during the dawn of AI-assisted code.](http://wouteronarchitecture.medium.com/arius-7-and-spec-driven-development-at-scale-a-failure-mode-97e8d7da3e2a?source=post_page---author_recirc--9a7b40e41c61----0---------------------44dc9bad_0f86_4a40_9ef7_9583200cec4b--------------)

[](http://wouteronarchitecture.medium.com/arius-7-and-spec-driven-development-at-scale-a-failure-mode-97e8d7da3e2a?source=post_page---author_recirc--9a7b40e41c61----0---------------------44dc9bad_0f86_4a40_9ef7_9583200cec4b--------------)

[](https://medium.com/m/signin?operation=register&redirect=https%3A%2F%2Fwouteronarchitecture.medium.com%2Fsmart-cleanup-of-unused-references-wouter-on-architecture-9a7b40e41c61&source=---author_recirc--9a7b40e41c61----0-----------------explicit_signal----44dc9bad_0f86_4a40_9ef7_9583200cec4b--------------)

[](https://medium.com/m/signin?actionUrl=https%3A%2F%2Fmedium.com%2F_%2Fbookmark%2Fp%2F97e8d7da3e2a&operation=register&redirect=https%3A%2F%2Fwouteronarchitecture.medium.com%2Farius-7-and-spec-driven-development-at-scale-a-failure-mode-97e8d7da3e2a&source=---author_recirc--9a7b40e41c61----0-----------------bookmark_preview----44dc9bad_0f86_4a40_9ef7_9583200cec4b--------------)

![Image 17: Developing Arius 5 during the dawn of AI assisted code](https://miro.medium.com/v2/resize:fit:679/format:webp/0*z_2uYvtk7YnA8Y7Q.png)

[![Image 18: Wouter on Architecture](https://miro.medium.com/v2/resize:fill:20:20/1*AoLeduUi-W4NsYodQ8ERMA.jpeg)](http://wouteronarchitecture.medium.com/?source=post_page---author_recirc--9a7b40e41c61----1---------------------44dc9bad_0f86_4a40_9ef7_9583200cec4b--------------)

[Wouter on Architecture](http://wouteronarchitecture.medium.com/?source=post_page---author_recirc--9a7b40e41c61----1---------------------44dc9bad_0f86_4a40_9ef7_9583200cec4b--------------)

·

Nov 6, 2025

## [Developing Arius 5 during the dawn of AI assisted code ### End of september, I finally did it. I said goodbye to my ~3 year old codebase of Arius 3 and merged the new v5 code into main. (v4 was a…](http://wouteronarchitecture.medium.com/developing-arius-5-during-the-dawn-of-ai-assisted-code-473fdb05b1e6?source=post_page---author_recirc--9a7b40e41c61----1---------------------44dc9bad_0f86_4a40_9ef7_9583200cec4b--------------)

[](http://wouteronarchitecture.medium.com/developing-arius-5-during-the-dawn-of-ai-assisted-code-473fdb05b1e6?source=post_page---author_recirc--9a7b40e41c61----1---------------------44dc9bad_0f86_4a40_9ef7_9583200cec4b--------------)

[](https://medium.com/m/signin?operation=register&redirect=https%3A%2F%2Fwouteronarchitecture.medium.com%2Fsmart-cleanup-of-unused-references-wouter-on-architecture-9a7b40e41c61&source=---author_recirc--9a7b40e41c61----1-----------------explicit_signal----44dc9bad_0f86_4a40_9ef7_9583200cec4b--------------)

[](https://medium.com/m/signin?actionUrl=https%3A%2F%2Fmedium.com%2F_%2Fbookmark%2Fp%2F473fdb05b1e6&operation=register&redirect=https%3A%2F%2Fwouteronarchitecture.medium.com%2Fdeveloping-arius-5-during-the-dawn-of-ai-assisted-code-473fdb05b1e6&source=---author_recirc--9a7b40e41c61----1-----------------bookmark_preview----44dc9bad_0f86_4a40_9ef7_9583200cec4b--------------)

![Image 19: Coaching with Context: Applying Situational Leadership in 1-on-1s](https://miro.medium.com/v2/resize:fit:679/format:webp/1*gSW-gbU65dQXjQy-Np31cg.png)

[![Image 20: Wouter on Architecture](https://miro.medium.com/v2/resize:fill:20:20/1*AoLeduUi-W4NsYodQ8ERMA.jpeg)](http://wouteronarchitecture.medium.com/?source=post_page---author_recirc--9a7b40e41c61----2---------------------44dc9bad_0f86_4a40_9ef7_9583200cec4b--------------)

[Wouter on Architecture](http://wouteronarchitecture.medium.com/?source=post_page---author_recirc--9a7b40e41c61----2---------------------44dc9bad_0f86_4a40_9ef7_9583200cec4b--------------)

·

Mar 9

## [Coaching with Context: Applying Situational Leadership in 1-on-1s ### Good coaching isn’t about having all the answers — it’s about asking the right questions at the right moment, in the right way. That’s…](http://wouteronarchitecture.medium.com/coaching-with-context-applying-situational-leadership-in-1-on-1s-8bd1bf715eb5?source=post_page---author_recirc--9a7b40e41c61----2---------------------44dc9bad_0f86_4a40_9ef7_9583200cec4b--------------)

[](http://wouteronarchitecture.medium.com/coaching-with-context-applying-situational-leadership-in-1-on-1s-8bd1bf715eb5?source=post_page---author_recirc--9a7b40e41c61----2---------------------44dc9bad_0f86_4a40_9ef7_9583200cec4b--------------)

[](https://medium.com/m/signin?operation=register&redirect=https%3A%2F%2Fwouteronarchitecture.medium.com%2Fsmart-cleanup-of-unused-references-wouter-on-architecture-9a7b40e41c61&source=---author_recirc--9a7b40e41c61----2-----------------explicit_signal----44dc9bad_0f86_4a40_9ef7_9583200cec4b--------------)

[](https://medium.com/m/signin?actionUrl=https%3A%2F%2Fmedium.com%2F_%2Fbookmark%2Fp%2F8bd1bf715eb5&operation=register&redirect=https%3A%2F%2Fwouteronarchitecture.medium.com%2Fcoaching-with-context-applying-situational-leadership-in-1-on-1s-8bd1bf715eb5&source=---author_recirc--9a7b40e41c61----2-----------------bookmark_preview----44dc9bad_0f86_4a40_9ef7_9583200cec4b--------------)

![Image 21: David Deutsch on (un)sustainability & climate optimism](https://miro.medium.com/v2/resize:fit:679/format:webp/0*1n2wYnyU9bpGadjF.jpg)

[![Image 22: Wouter on Architecture](https://miro.medium.com/v2/resize:fill:20:20/1*AoLeduUi-W4NsYodQ8ERMA.jpeg)](http://wouteronarchitecture.medium.com/?source=post_page---author_recirc--9a7b40e41c61----3---------------------44dc9bad_0f86_4a40_9ef7_9583200cec4b--------------)

[Wouter on Architecture](http://wouteronarchitecture.medium.com/?source=post_page---author_recirc--9a7b40e41c61----3---------------------44dc9bad_0f86_4a40_9ef7_9583200cec4b--------------)

·

Jan 7, 2025

## [David Deutsch on (un)sustainability & climate optimism ### David Deutsch’s The Beginning of Infinity brims with ideas that turn conventional wisdom on its head. One especially eye-opening chapter in…](http://wouteronarchitecture.medium.com/david-deutsch-on-un-sustainability-climate-optimism-f6793138a047?source=post_page---author_recirc--9a7b40e41c61----3---------------------44dc9bad_0f86_4a40_9ef7_9583200cec4b--------------)

[](http://wouteronarchitecture.medium.com/david-deutsch-on-un-sustainability-climate-optimism-f6793138a047?source=post_page---author_recirc--9a7b40e41c61----3---------------------44dc9bad_0f86_4a40_9ef7_9583200cec4b--------------)

[](https://medium.com/m/signin?operation=register&redirect=https%3A%2F%2Fwouteronarchitecture.medium.com%2Fsmart-cleanup-of-unused-references-wouter-on-architecture-9a7b40e41c61&source=---author_recirc--9a7b40e41c61----3-----------------explicit_signal----44dc9bad_0f86_4a40_9ef7_9583200cec4b--------------)

[](https://medium.com/m/signin?actionUrl=https%3A%2F%2Fmedium.com%2F_%2Fbookmark%2Fp%2Ff6793138a047&operation=register&redirect=https%3A%2F%2Fwouteronarchitecture.medium.com%2Fdavid-deutsch-on-un-sustainability-climate-optimism-f6793138a047&source=---author_recirc--9a7b40e41c61----3-----------------bookmark_preview----44dc9bad_0f86_4a40_9ef7_9583200cec4b--------------)

[See all from Wouter on Architecture](http://wouteronarchitecture.medium.com/?source=post_page---author_recirc--9a7b40e41c61---------------------------------------)

## Recommended from Medium

![Image 23: MCP is Dead](https://miro.medium.com/v2/resize:fit:679/format:webp/1*Oj5PiyfEi8DadSC8Jy374w.png)

[![Image 24: UX Planet](https://miro.medium.com/v2/resize:fill:20:20/1*A0FnBy5FBoVQC02SZXLXPg.png)](https://uxplanet.org/?source=post_page---read_next_recirc--9a7b40e41c61----0---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

In

[UX Planet](https://uxplanet.org/?source=post_page---read_next_recirc--9a7b40e41c61----0---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

by

[Nick Babich](https://medium.com/@101?source=post_page---read_next_recirc--9a7b40e41c61----0---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

·

Apr 6

## [MCP is Dead ### Why you should avoid using MCP in Claude Code and what to use instead](https://medium.com/@101/mcp-is-dead-cf16b667ba6d?source=post_page---read_next_recirc--9a7b40e41c61----0---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

[4.1K 230 125](https://medium.com/@101/mcp-is-dead-cf16b667ba6d?source=post_page---read_next_recirc--9a7b40e41c61----0---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

[](https://medium.com/m/signin?operation=register&redirect=https%3A%2F%2Fwouteronarchitecture.medium.com%2Fsmart-cleanup-of-unused-references-wouter-on-architecture-9a7b40e41c61&source=---read_next_recirc--9a7b40e41c61----0-----------------explicit_signal----948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

[](https://medium.com/m/signin?actionUrl=https%3A%2F%2Fmedium.com%2F_%2Fbookmark%2Fp%2Fcf16b667ba6d&operation=register&redirect=https%3A%2F%2Fuxplanet.org%2Fmcp-is-dead-cf16b667ba6d&source=---read_next_recirc--9a7b40e41c61----0-----------------bookmark_preview----948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

![Image 25: I Deleted Notion and Obsidian. Here’s What Replaced Them — and Why I’m Never Going Back.](https://miro.medium.com/v2/resize:fit:679/format:webp/1*aowDRdXYqixdKht7inhynw.png)

[![Image 26: Write A Catalyst](https://miro.medium.com/v2/resize:fill:20:20/1*KCHN5TM3Ga2PqZHA4hNbaw.png)](https://medium.com/write-a-catalyst?source=post_page---read_next_recirc--9a7b40e41c61----1---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

In

[Write A Catalyst](https://medium.com/write-a-catalyst?source=post_page---read_next_recirc--9a7b40e41c61----1---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

by

[Suraj Jha](https://medium.com/@suraj_jha?source=post_page---read_next_recirc--9a7b40e41c61----1---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

·

May 12

## [I Deleted Notion and Obsidian. Here’s What Replaced Them — and Why I’m Never Going Back. ### Two of the most popular productivity apps in the world. Both are gone in a week. And honestly? I don’t miss them at all.](https://medium.com/@suraj_jha/i-deleted-notion-and-obsidian-heres-what-replaced-them-and-why-i-m-never-going-back-27a7d8545606?source=post_page---read_next_recirc--9a7b40e41c61----1---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

[2.8K 95 59](https://medium.com/@suraj_jha/i-deleted-notion-and-obsidian-heres-what-replaced-them-and-why-i-m-never-going-back-27a7d8545606?source=post_page---read_next_recirc--9a7b40e41c61----1---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

[](https://medium.com/m/signin?operation=register&redirect=https%3A%2F%2Fwouteronarchitecture.medium.com%2Fsmart-cleanup-of-unused-references-wouter-on-architecture-9a7b40e41c61&source=---read_next_recirc--9a7b40e41c61----1-----------------explicit_signal----948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

[](https://medium.com/m/signin?actionUrl=https%3A%2F%2Fmedium.com%2F_%2Fbookmark%2Fp%2F27a7d8545606&operation=register&redirect=https%3A%2F%2Fmedium.com%2Fwrite-a-catalyst%2Fi-deleted-notion-and-obsidian-heres-what-replaced-them-and-why-i-m-never-going-back-27a7d8545606&source=---read_next_recirc--9a7b40e41c61----1-----------------bookmark_preview----948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

![Image 27: A weathered stone threshold worn smooth into a shallow concave hollow by centuries of footsteps](https://miro.medium.com/v2/resize:fit:679/format:webp/1*OBB8VSehGVHuDHOHfFiOSA.png)

[![Image 28: Level Up Coding](https://miro.medium.com/v2/resize:fill:20:20/1*5D9oYBd58pyjMkV_5-zXXQ.jpeg)](https://levelup.gitconnected.com/?source=post_page---read_next_recirc--9a7b40e41c61----0---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

In

[Level Up Coding](https://levelup.gitconnected.com/?source=post_page---read_next_recirc--9a7b40e41c61----0---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

by

[Michael Lawrence](https://medium.com/@michaelclawrence?source=post_page---read_next_recirc--9a7b40e41c61----0---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

·

May 11

## [AI Isn’t Replacing Developers. It’s Doing Something Worse. ### The replacement narrative is loud. The quiet one is already happening.](https://medium.com/@michaelclawrence/ai-isnt-replacing-developers-it-s-doing-something-worse-2d18fb595362?source=post_page---read_next_recirc--9a7b40e41c61----0---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

[8.1K 200 93](https://medium.com/@michaelclawrence/ai-isnt-replacing-developers-it-s-doing-something-worse-2d18fb595362?source=post_page---read_next_recirc--9a7b40e41c61----0---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

[](https://medium.com/m/signin?operation=register&redirect=https%3A%2F%2Fwouteronarchitecture.medium.com%2Fsmart-cleanup-of-unused-references-wouter-on-architecture-9a7b40e41c61&source=---read_next_recirc--9a7b40e41c61----0-----------------explicit_signal----948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

[](https://medium.com/m/signin?actionUrl=https%3A%2F%2Fmedium.com%2F_%2Fbookmark%2Fp%2F2d18fb595362&operation=register&redirect=https%3A%2F%2Flevelup.gitconnected.com%2Fai-isnt-replacing-developers-it-s-doing-something-worse-2d18fb595362&source=---read_next_recirc--9a7b40e41c61----0-----------------bookmark_preview----948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

![Image 29: Anthropic’s Engineer Said Kill Markdown. Here’s What He Actually Meant.](https://miro.medium.com/v2/resize:fit:679/format:webp/0*ITstR02aTfQsF2bV)

[![Image 30: Generative AI](https://miro.medium.com/v2/resize:fill:20:20/1*M4RBhIRaSSZB7lXfrGlatA.png)](https://generativeai.pub/?source=post_page---read_next_recirc--9a7b40e41c61----1---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

In

[Generative AI](https://generativeai.pub/?source=post_page---read_next_recirc--9a7b40e41c61----1---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

by

[Yanli Liu](https://medium.com/@yanli.liu?source=post_page---read_next_recirc--9a7b40e41c61----1---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

·

May 15

## [Anthropic’s Engineer Said Kill Markdown. Here’s What He Actually Meant. ### HTML vs Markdown ： Here’s the Decision Tree Both Sides Needed.](https://medium.com/@yanli.liu/anthropics-engineer-said-kill-markdown-here-s-what-he-actually-meant-36bee00c0ca2?source=post_page---read_next_recirc--9a7b40e41c61----1---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

[2.3K 109 29](https://medium.com/@yanli.liu/anthropics-engineer-said-kill-markdown-here-s-what-he-actually-meant-36bee00c0ca2?source=post_page---read_next_recirc--9a7b40e41c61----1---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

[](https://medium.com/m/signin?operation=register&redirect=https%3A%2F%2Fwouteronarchitecture.medium.com%2Fsmart-cleanup-of-unused-references-wouter-on-architecture-9a7b40e41c61&source=---read_next_recirc--9a7b40e41c61----1-----------------explicit_signal----948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

[](https://medium.com/m/signin?actionUrl=https%3A%2F%2Fmedium.com%2F_%2Fbookmark%2Fp%2F36bee00c0ca2&operation=register&redirect=https%3A%2F%2Fgenerativeai.pub%2Fanthropics-engineer-said-kill-markdown-here-s-what-he-actually-meant-36bee00c0ca2&source=---read_next_recirc--9a7b40e41c61----1-----------------bookmark_preview----948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

![Image 31: 8 Crazy Things Claude AI Can Do (That ChatGPT Can’t)](https://miro.medium.com/v2/resize:fit:679/format:webp/1*z_2i7m63JolPp5wTTurUIQ.png)

[![Image 32: No Time](https://miro.medium.com/v2/resize:fill:20:20/1*-s0apT5ZWj5xVWPrhOIHHQ.png)](https://medium.com/no-time?source=post_page---read_next_recirc--9a7b40e41c61----2---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

In

[No Time](https://medium.com/no-time?source=post_page---read_next_recirc--9a7b40e41c61----2---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

by

[Pranit naik](https://medium.com/@pranithnaikpranit?source=post_page---read_next_recirc--9a7b40e41c61----2---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

·

Apr 18

## [8 Crazy Things Claude AI Can Do (That ChatGPT Can’t) ### ChatGPT users, take notes](https://medium.com/@pranithnaikpranit/8-crazy-things-claude-ai-can-do-that-chatgpt-cant-ef383eeb16f4?source=post_page---read_next_recirc--9a7b40e41c61----2---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

[6.7K 205 112](https://medium.com/@pranithnaikpranit/8-crazy-things-claude-ai-can-do-that-chatgpt-cant-ef383eeb16f4?source=post_page---read_next_recirc--9a7b40e41c61----2---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

[](https://medium.com/m/signin?operation=register&redirect=https%3A%2F%2Fwouteronarchitecture.medium.com%2Fsmart-cleanup-of-unused-references-wouter-on-architecture-9a7b40e41c61&source=---read_next_recirc--9a7b40e41c61----2-----------------explicit_signal----948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

[](https://medium.com/m/signin?actionUrl=https%3A%2F%2Fmedium.com%2F_%2Fbookmark%2Fp%2Fef383eeb16f4&operation=register&redirect=https%3A%2F%2Fmedium.com%2Fno-time%2F8-crazy-things-claude-ai-can-do-that-chatgpt-cant-ef383eeb16f4&source=---read_next_recirc--9a7b40e41c61----2-----------------bookmark_preview----948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

![Image 33: A Single CLAUDE.md File Went Viral. The Reason Is Embarrassingly Simple.](https://miro.medium.com/v2/resize:fit:679/format:webp/1*wpOHldCy2O2itB-241M5rQ.png)

[![Image 34: Towards Deep Learning](https://miro.medium.com/v2/resize:fill:20:20/1*LF1EF4T2UFrpxYubZ7r_7g.png)](https://www.towardsdeeplearning.com/?source=post_page---read_next_recirc--9a7b40e41c61----3---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

In

[Towards Deep Learning](https://www.towardsdeeplearning.com/?source=post_page---read_next_recirc--9a7b40e41c61----3---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

by

[Sumit Pandey](https://medium.com/@sumit.ai?source=post_page---read_next_recirc--9a7b40e41c61----3---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

·

May 8

## [A Single CLAUDE.md File Went Viral. The Reason Is Embarrassingly Simple. ### 91,000 stars on GitHub. No code. Four rules from Andrej Karpathy that every coding agent should have been following from day one.](https://medium.com/@sumit.ai/a-single-claude-md-file-went-viral-the-reason-is-embarrassingly-simple-5b515c9e4cca?source=post_page---read_next_recirc--9a7b40e41c61----3---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

[3.9K 51 86](https://medium.com/@sumit.ai/a-single-claude-md-file-went-viral-the-reason-is-embarrassingly-simple-5b515c9e4cca?source=post_page---read_next_recirc--9a7b40e41c61----3---------------------948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

[](https://medium.com/m/signin?operation=register&redirect=https%3A%2F%2Fwouteronarchitecture.medium.com%2Fsmart-cleanup-of-unused-references-wouter-on-architecture-9a7b40e41c61&source=---read_next_recirc--9a7b40e41c61----3-----------------explicit_signal----948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

[](https://medium.com/m/signin?actionUrl=https%3A%2F%2Fmedium.com%2F_%2Fbookmark%2Fp%2F5b515c9e4cca&operation=register&redirect=https%3A%2F%2Fwww.towardsdeeplearning.com%2Fa-single-claude-md-file-went-viral-the-reason-is-embarrassingly-simple-5b515c9e4cca&source=---read_next_recirc--9a7b40e41c61----3-----------------bookmark_preview----948d89e0_d39b_442e_b6b8_9cb4455d5bd4--------------)

[See more recommendations](https://medium.com/?source=post_page---read_next_recirc--9a7b40e41c61---------------------------------------)

[Help](https://help.medium.com/hc/en-us?source=post_page-----9a7b40e41c61---------------------------------------)

[Status](https://status.medium.com/?source=post_page-----9a7b40e41c61---------------------------------------)

[About](https://medium.com/about?autoplay=1&source=post_page-----9a7b40e41c61---------------------------------------)

[Careers](https://medium.com/jobs-at-medium/work-at-medium-959d1a85284e?source=post_page-----9a7b40e41c61---------------------------------------)

[Press](mailto:pressinquiries@medium.com)

[Blog](https://blog.medium.com/?source=post_page-----9a7b40e41c61---------------------------------------)

[Store](https://medium.com/store)

[Privacy](https://policy.medium.com/medium-privacy-policy-f03bf92035c9?source=post_page-----9a7b40e41c61---------------------------------------)

[Rules](https://policy.medium.com/medium-rules-30e5502c4eb4?source=post_page-----9a7b40e41c61---------------------------------------)

[Terms](https://policy.medium.com/medium-terms-of-service-9db0094a1e0f?source=post_page-----9a7b40e41c61---------------------------------------)

[Text to speech](https://speechify.com/medium?source=post_page-----9a7b40e41c61---------------------------------------)
