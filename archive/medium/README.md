# Medium Archive Notes

This folder contains local archives of Medium posts fetched through `r.jina.ai` plus manifests that map archived Medium URLs back to local `docs/_posts` files.

## Discovery Sources

The current archive manifest is built from the union of these public Medium XML/HTML surfaces:

- `https://medium.com/feed/@wouteronarchitecture`
- `https://wouteronarchitecture.medium.com/feed`
- `https://wouteronarchitecture.medium.com/sitemap/sitemap.xml`
- `https://medium.com/@wouteronarchitecture` (public profile HTML, used as a supplement)

## Files

- `manifest.full.json`: merged Medium source manifest with local post mapping
- `manifest.full.tsv`: tab-separated version of the same manifest
- `*.{medium-id}.md`: archived Medium snapshots fetched through `https://r.jina.ai/http://...`

## Current Coverage

Confirmed Medium sources were found for these local posts:

- `2024-01-15-getting-non-functional-requirements-from-business-people.md`
- `2024-01-16-sentix-long-term-investor-trends.md`
- `2024-07-08-adding-a-correlationid-for-exceptions-for-application-insights-draft.md`
- `2024-09-04-optimal-usage-of-global-usings.md`
- `2024-09-05-eagerly-consuming-a-collection-of-completing-tasks.md`
- `2024-09-06-smart-cleanup-of-unused-references.md`
- `2025-01-05-you-will-be-assimilated-microsoft-implementations-of-formerly-popular-oss-libraries.md`
- `2025-03-21-validating-dependency-injection.md`
- `2025-06-11-choose-boring-technology.md`
- `2025-11-05-modern-dotnet-configuration-practices-talk-summary.md`
- `2025-11-06-developing-arius-5-during-the-dawn-of-ai-assisted-code.md`
- `2026-02-25-the-ackoff-lectures-a-blog-series-on-systems-thinking-management-and-the-world-were-building.md`
- `2026-02-26-whos-got-the-monkey.md`
- `2026-03-05-classic-problems-have-classic-solutions.md`
- `2026-03-09-coaching-with-context-applying-situational-leadership-in-1-on-1s.md`
- `2026-03-18-the-coaching-habit-book.md`
- `2026-06-11-arius-7-and-spec-driven-development-at-scale-a-failure-mode.md`

Total confirmed mappings: `17`

## Unresolved Local Posts

These local posts do not currently have a confirmed Medium source from the public XML/HTML surfaces above:

- `2024-02-06-c-string-comparison-explained.md`
- `2024-02-09-choosing-a-cosmosdb-consistency-level.md`
- `2024-02-09-value-objects-in-c-part-1.md`
- `2024-02-14-value-objects-in-c-part-2.md`
- `2024-03-04-overriding-application-insights-log-levels.md`
- `2024-09-15-records-and-interfaces-here-be-dragons.md`
- `2025-01-05-frequently-overlooked-architectural-characteristics-video-review.md`
- `2025-01-07-david-deutsch-on-unsustainability-climate-optimism.md`
- `2025-01-09-exploring-options-for-building-rest-apis-with-c-and-deploying-to-azure.md`
- `2025-01-12-from-layers-to-rings-hexagonal-architectures-explained-by-silas-graffy.md`
- `2025-01-17-implementing-centralized-package-management.md`
- `2025-01-27-transitive-dependencies-gone-wild-why-your-net-microservice-isnt-running-what-you-think.md`
- `2025-02-07-monkeys-bananas-and-why.md`
- `2025-02-08-resultt-libraries.md`
- `2025-02-10-understanding-high-cpu-and-memory-usage-when-to-act-and-when-to-relax.md`
- `2026-06-14-choosing-a-csharp-dictionary.md`

Total unresolved local posts: `16`

## Notes

- Medium's public RSS feeds expose only a rolling window of recent posts.
- Medium's sitemap exposes some additional posts, but not the full local catalog.
- Medium's JSON/original HTML endpoints appear to be partially protected by Cloudflare, so this archive should be treated as "best effort from public surfaces" rather than a guaranteed complete historical export.
- The archived `r.jina.ai` markdown snapshots are useful for content recovery, but they do not always preserve original Medium embed blocks verbatim.
