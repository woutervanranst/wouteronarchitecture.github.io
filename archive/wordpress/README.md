# WordPress Archive Notes

This folder contains local archives of the original WordPress post content recovered from the historical WordPress database backup stored in git history.

## Discovery Sources

The current WordPress archive was reconstructed from these git-history artifacts:

- commit `7ae892fc0263af1287da7009e8f07fb952dda100` (`feat: intial wordpress import/migraiton`)
- file `backup_2026-06-01-0956_Wouter_on_Architecture_29f94f42e57a-db.gz` from that commit
- commit `42daf4bf4e82abb74acd6fb9900f44402c4d9fd8` confirms those backup artifacts were later removed

The database dump contains:

- `wp_posts` rows with original `post_content`
- `wp_yoast_indexable` rows with the published WordPress permalinks used for mapping

## Files

- `manifest.full.json`: full WordPress source manifest with local post mapping
- `manifest.full.tsv`: tab-separated version of the same manifest
- `manifest.json`: reduced manifest with file, id, source URL, and archive filename
- `manifest.tsv`: tab-separated reduced manifest
- `*.{wordpress-id}.html`: original WordPress `post_content` HTML recovered from the database dump

## Current Coverage

Confirmed WordPress sources were found for these local posts:

- `2024-01-15-getting-non-functional-requirements-from-business-people.md`
- `2024-01-16-sentix-long-term-investor-trends.md`
- `2024-02-06-c-string-comparison-explained.md`
- `2024-02-09-choosing-a-cosmosdb-consistency-level.md`
- `2024-02-09-value-objects-in-c-part-1.md`
- `2024-02-14-value-objects-in-c-part-2.md`
- `2024-03-04-overriding-application-insights-log-levels.md`
- `2024-07-08-adding-a-correlationid-for-exceptions-for-application-insights-draft.md`
- `2024-09-04-optimal-usage-of-global-usings.md`
- `2024-09-05-eagerly-consuming-a-collection-of-completing-tasks.md`
- `2024-09-06-smart-cleanup-of-unused-references.md`
- `2024-09-15-records-and-interfaces-here-be-dragons.md`
- `2025-01-05-frequently-overlooked-architectural-characteristics-video-review.md`
- `2025-01-05-you-will-be-assimilated-microsoft-implementations-of-formerly-popular-oss-libraries.md`
- `2025-01-07-david-deutsch-on-unsustainability-climate-optimism.md`
- `2025-01-09-exploring-options-for-building-rest-apis-with-c-and-deploying-to-azure.md`
- `2025-01-12-from-layers-to-rings-hexagonal-architectures-explained-by-silas-graffy.md`
- `2025-01-17-implementing-centralized-package-management.md`
- `2025-01-27-transitive-dependencies-gone-wild-why-your-net-microservice-isnt-running-what-you-think.md`
- `2025-02-07-monkeys-bananas-and-why.md`
- `2025-02-08-resultt-libraries.md`
- `2025-02-10-understanding-high-cpu-and-memory-usage-when-to-act-and-when-to-relax.md`
- `2025-03-21-validating-dependency-injection.md`
- `2025-06-11-choose-boring-technology.md`

Total confirmed mappings: `24`

## Not Part Of This Archive

These local posts were not part of the historical WordPress import and therefore do not have a WordPress source in this archive:

- `2025-11-05-modern-dotnet-configuration-practices-talk-summary.md`
- `2025-11-06-developing-arius-5-during-the-dawn-of-ai-assisted-code.md`
- `2026-02-25-the-ackoff-lectures-a-blog-series-on-systems-thinking-management-and-the-world-were-building.md`
- `2026-02-26-whos-got-the-monkey.md`
- `2026-03-05-classic-problems-have-classic-solutions.md`
- `2026-03-09-coaching-with-context-applying-situational-leadership-in-1-on-1s.md`
- `2026-03-18-the-coaching-habit-book.md`
- `2026-06-11-arius-7-and-spec-driven-development-at-scale-a-failure-mode.md`
- `2026-06-14-choosing-a-csharp-dictionary.md`

Total local posts outside this WordPress archive: `9`

## Notes

- This archive is based on the WordPress database backup preserved in git history, not on live WordPress endpoints.
- The archived HTML files contain the original WordPress block markup as stored in `wp_posts.post_content`.
- Source URL mapping comes from `wp_yoast_indexable`, which preserves the published permalinks used on the WordPress site.
- Some source URLs point to `www.wouteronarchitecture.com`, some to `wouteronarchitecture.com`, and a few `guid` values still point at the earlier Azure-hosted WordPress instance. The manifest preserves those exact historical values.
