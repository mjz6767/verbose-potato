# Accepted v2.4 World-Threat Habitat Art Handoff

This is accepted v2.4 runtime art, exact-pinned as `WorldThreatHabitatAtlas`. It complements the eight v2.3 destination set-pieces by giving roaming factions stationary, illustrated homes while their existing creature tokens continue to patrol independently. Combined automated release verification passes.

## Delivered files

- `Docs/ArtReferences/source-world-threat-habitat-atlas-v2.4.0-generated.png` - preserved built-in ImageGen source on flat magenta.
- `Docs/ArtReferences/source-world-threat-habitat-atlas-v2.4.0-alpha.png` - locally keyed RGBA authoring source.
- `Docs/ArtReferences/world-threat-habitat-atlas-runtime-v2.4.0.png` - approved normalized runtime atlas.
- `Docs/ArtReferences/world-threat-habitat-atlas-runtime-v2.4.0-validation.json` - deterministic extraction and geometry report.
- `Docs/ArtReferences/world-threat-habitat-atlas-runtime-v2.4.0-prompt.txt` - final prompt and generation provenance.

The local repository excludes `Docs/ArtReferences/*` through `.git/info/exclude`. The release checkpoint must deliberately force-add the five approved files above; ordinary `git status` does not reveal whether these ignored inputs have been promoted.

## Runtime contract

- Exact RGBA size: 1536 by 768.
- Grid: 4 columns by 2 rows; 384-pixel square cells.
- Safe gutter: no alpha inside the outer 20 pixels of any cell.
- Grounding: centered subjects with compatible bottom-center anchors.
- Visible coverage: 46.5% to 50.4% per cell after normalization.
- Chroma check: zero visible pixels remain in the bright-magenta key range.

Stable row-major indices:

0. Rat Warren
1. Plague-Bell Midden
2. Kobold Ambush Camp
3. Kobold Shaman Totem Yard
4. Drow Moon-Silk Watchpost
5. Undead Ossuary
6. Demon Breach
7. Ruined Road Waystation

## Original wiring recommendation and status

Source integration implements items 1-6 below, and their combined test and package execution passes. Item 7 remains a future, advisory exploration-audio opportunity rather than part of the accepted v2.4 source contract.

1. Add a separately named `WorldThreatHabitatAtlas` manifest pin. Do not replace the v2.3 `WorldAreaSetpieceAtlas`.
2. Load and validate this atlas as an exact 4 by 2 transparent sheet alongside the existing world-area set-piece texture.
3. Add a small deterministic presentation rule that maps roaming-threat faction/archetype to indices. Use index 0 for ordinary rats, 1 for plague/mage/cleric rats, 2 for ordinary kobolds, 3 for kobold shamans, 4 for drow, 5 for undead, 6 for demons, and 7 only as a neutral/fallback aftermath site, including inactive or defeated homes.
4. Draw a stationary habitat at each threat's existing `HomeX`/`HomeY`, beneath the mobile roaming-threat token. Suggested scale is approximately 1.35 cells on Region Map and 1.65 cells on Local Map, with bottom-center anchoring and reduced tint when the home is outside the current objective route.
5. Keep certified safe-road cells clear. A habitat marks patrol territory; it must not silently add encounters, move the home cell, or change save data.
6. Extend focused rule/runtime coverage for the manifest pin, exact geometry, all eight populated cells, safe gutters, archetype mappings, stable home anchoring, and draw order beneath the creature token.
7. Pair proximity to `HomeX`/`HomeY` with faction-specific ambience in the exploration-audio pass, while preserving the v2.3 rule that alerted threats and authored destination ambience have priority.

## Preserved integration boundaries

The earlier combat, title/Grand Hearth, and regional-site coordination blocks are released. Integration remained outside the accepted combat layout and preserved stable Grand Hearth IDs and room bounds, New Game -> Muster -> first-spawn behavior, portal flow, the starting tile label, cartography table cell 7, blue company road chest cell 17, and accepted title/Tavern presentation. Ambient world art remains off the tutorial lane.
