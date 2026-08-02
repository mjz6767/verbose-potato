# Integrated v2.7 Grand Hearth Companion Art Handoff

The v2.7 Grand Hearth floor and set-piece atlases are accepted, runtime-integrated, and package-verified companion art for the playable Town Hall opening. They extend the existing v2.6 room presentation without replacing the shared v1.61 Midgaard interior atlases. Exact manifest pins, fail-closed loaders, deterministic mappings, RuleSmoke, focused SpriteArtRuntimeSmoke, full RuntimeBoot, the final Windows build, six supported-resolution built-player captures, canonical package inventory, and clean-extracted packaged boot pass. Physical-controller review and a full human playthrough remain manual.

## Delivered floor files

- `Docs/ArtReferences/source-grand-hearth-floor-atlas-v2.7.0-generated.png`
- `Docs/ArtReferences/grand-hearth-floor-atlas-runtime-v2.7.0.png`
- `Docs/ArtReferences/grand-hearth-floor-atlas-runtime-v2.7.0-prompt.txt`
- `Docs/ArtReferences/grand-hearth-floor-atlas-runtime-v2.7.0-validation.json`

The runtime floor sheet is an exact 1536 by 1024 TrueColor PNG arranged as 3 columns by 2 rows of 512-pixel square cells. Its stable row-major order is:

0. Dark civic wood floor A
1. Dark civic wood floor B
2. Oxblood-and-charcoal company runner
3. Company runner medallion
4. Heat-darkened Grand Hearth apron
5. Storm-door threshold

This is opaque room terrain. It does not use transparent sprite gutters and must not be normalized as an isolated-icon sheet.

The accepted final runtime uses the edge-balance ImageGen refinement for cells 0 and 1. Its floor prompt and validation record were updated with the refined source.

## Delivered set-piece files

- `Docs/ArtReferences/source-grand-hearth-setpiece-atlas-v2.7.0-generated.png`
- `Docs/ArtReferences/source-grand-hearth-setpiece-atlas-v2.7.0-alpha.png`
- `Docs/ArtReferences/grand-hearth-setpiece-atlas-runtime-v2.7.0.png`
- `Docs/ArtReferences/grand-hearth-setpiece-atlas-runtime-v2.7.0-prompt.txt`
- `Docs/ArtReferences/grand-hearth-setpiece-atlas-runtime-v2.7.0-validation.json`

The runtime set-piece sheet is an exact RGBA 1536 by 1024 PNG arranged as 3 columns by 2 rows of 512-pixel square cells. Every cell reserves a 32-pixel transparent outer gutter. Its stable row-major order is:

0. Monumental Grand Hearth
1. Interior storm doors
2. Company register
3. Road-company banner
4. Rain-blue window
5. Road-company stores

Set-piece subjects are isolated presentation art. They must not introduce collision, interaction targets, dialogue identities, route blocking, or baked labels and UI cues.

## Source-control and package contract

`Docs/ArtReferences/` is locally ignored through `.git/info/exclude`. The integration deliberately promotes the nine exact files listed above: two runtime atlases plus seven source/alpha/prompt/validation companions. A normal broad add will not reliably promote them, and the rest of the retained ArtReferences workspace must not be added.

Only these two files belong in the Windows player package:

- `grand-hearth-floor-atlas-runtime-v2.7.0.png`
- `grand-hearth-setpiece-atlas-runtime-v2.7.0.png`

The generated source, cleaned alpha source, prompt, and validation files remain source-only provenance.

## Preserved runtime and fallback contracts

- Keep `midgaard-interior-tile-atlas-runtime-v1.61.0.png` and `midgaard-interior-prop-atlas-runtime-v1.61.0.png` pinned and available for every other Midgaard interior and as the Grand Hearth fallback.
- A missing, malformed, or rejected v2.7 companion sheet must fall back to the accepted v2.6 Grand Hearth presentation rather than changing gameplay or drawing a mismatched cell.
- Preserve the stable `midgaard-grand-hearth` zone and fixture IDs, the 10 by 9 room bounds, New Game -> Muster -> first spawn, the oxblood-and-charcoal company runner, the storm-door portal flow, the starting objective, and save-schema behavior.
- Preserve the v1.61 map-table cell 7 and blue road-chest cell 17 fallbacks, the approved v2.4 patron atlas and placements, the exterior Town Hall facade, and the accepted title/Tavern presentation.
- Floor and set-piece selection is presentation-only. It must not add or remove `MapObject` records, alter traversal, move fixtures or patrons, or change encounter, objective, autosave, or departure behavior.
- Fallback set-piece drawing is restricted to the owning fixture footprint and covered by deterministic assertions.

## Integration validation status

Passed: exact manifest pins, fail-closed loaders and draw adapters, deterministic floor/set-piece mappings, fallback-footprint assertions, RuleSmoke, focused SpriteArtRuntimeSmoke, full RuntimeBoot, and the final Windows build. The room, gameplay, and save schema v25 remain stable.

Passed visual evidence: six captures under `QA/v2.7.0-grand-hearth` cover Local and Region at 960 by 600, 1280 by 720, and 1920 by 1080. Every capture reports `complete=True`, `failure=None`; visual review accepts one open hall, continuous oxblood runner and medallion, monumental hearth, open storm doors, six patrons, and a clear route, with no checkerboard seams, magenta residue, square backplates, or cropping.

Passed package evidence: canonical BuildAndPackageWindows v2.7.0 completes from a clean tracked source. The authoritative `QA/v2.7.0-release-integrity/release-integrity-manifest.json` records save v25, `sourceDirty=false`, `sourceStatusEntryCount=0`, `cleanExtractLaunch=true`, `packagedBootMode=batchmode-explore-smoke`, `playerExitCode=0`, `packagedArtCount=82`, and both exact Grand Hearth runtime atlases with their expected hashes.

Pending manual gates: physical-controller review and a complete human New Game -> Muster -> Town Hall -> Midgaard playthrough.
