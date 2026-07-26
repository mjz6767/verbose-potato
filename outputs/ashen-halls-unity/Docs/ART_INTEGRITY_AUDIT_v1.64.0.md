# Art Integrity Audit v1.64.0

## Scope

The audit reviewed every file pinned by `RuntimeArtManifest`, with focused cell-level review of the title, Midgaard gates, named city NPCs, NPC portraits, world-map tokens and props, spell/ability icons, and spell animation sheets.

## Corrected Findings

- The v1.61 title plaque was structurally valid, but a bright decorative rail crossed the subtitle. The v1.64 plaque uses a quieter oak-and-iron sign, reserves a broad lower title field beneath its medallion, and lets Unity render both title lines cleanly.
- The v1.30 gate sheet contained only small, lightly occupied gate cells. The v1.64 sheet replaces cells 0, 1, 6, and 7 with larger closed, open, east-facing, and west-facing structures with clear passages and wall connections.
- Four named world sprites no longer matched their established portraits:
  - Cell 7 now depicts Tessa as a female weaponsmith.
  - Cell 9 now depicts Maud as an elderly female enchanter.
  - Cell 13 now depicts Edda as a wounded female traveler.
  - Cell 18 now depicts Yara as a female Old Road scout.
- Edda remains the canonical authored name. `Edna` is accepted as a portrait-resolution alias so a misspelling cannot fall back to an unrelated face.

## Healthy Assets Left Unchanged

- NPC portrait atlas
- Midgaard wall, town, city-prop, street-life, paving, sewer, and interior atlases
- World-map material, token, prop, biome-prop, landmark, and route atlases
- Ability, signature-spell, magic UI, and spell-animation atlases
- Game application emblem and roaming-threat atlas

## Runtime Guardrails

- The three corrected runtime images are pinned by exact v1.64 filenames.
- `NpcPortraitCatalog.WorldSpriteIndex` centralizes named world-atlas mappings instead of scattering anonymous numbers through rendering code.
- Rule smoke tests verify named NPC mappings, title dimensions, gate/NPC atlas dimensions, cell coverage, and safe gutters.
- Save schema remains v22 because this patch changes presentation and content contracts only.
