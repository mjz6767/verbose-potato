# Art Wiring Audit — v1.80.0

## Outcome

The installed v1.79 art files were already the newest files in their semantic filename families, and the packaged copies matched source by SHA-256. The stale-looking world map came from selection weights: the preserved foundation painting appeared in 40% of most passable cells and 70% of natural ground.

v1.80.0 changes the presentation and hardens the wiring without replacing binary art:

- The three newer v1.68 ground variants now occupy 75% of passable world cells; the preserved foundation remains at 25%.
- World terrain, overlays, UI, Midgaard terrain/NPCs, portraits, and v1.77 character/enemy sprites are exact approved pins with semantic-version fallbacks for development.
- A release build fails when any approved family has a newer unreviewed PNG.
- Candidate fallback selection is global across both supported art roots, rather than allowing an older file in the first root to win.
- Shield maps to world-token cell 1. Multi-member parties use cell 0; one-member parties use their role cell in both Local and Region Map.
- Every active character, enemy, Midgaard NPC, portrait, and party-token mapping has an exact semantic assertion.
- Runtime validation covers all cells in the 4x4 character/enemy sheets and both 5x4 NPC sheets, including the previously omitted active Midgaard cells 1, 6, and 7.
- Startup uses the current pinned tavern backdrop in both editor and player packages instead of an unpackaged v0.27 recovery image.

## Approved high-visibility files

- `world-map-exploration-tile-atlas-runtime-v1.68.0.png`
- `world-map-material-atlas-runtime-v1.68.0.png`
- `world-map-overlay-atlas-runtime-v0.80.png`
- `world-map-progression-overlay-atlas-runtime-v0.63.png`
- `world-map-ui-atlas-runtime-v1.6.0.png`
- `world-map-token-sprite-atlas-runtime-v1.29.0.png`
- `world-map-region-landmark-atlas-runtime-v1.65.0.png`
- `midgaard-tile-atlas-runtime-v1.6.3.png`
- `midgaard-npc-atlas-runtime-v1.64.1.png`
- `npc-portrait-atlas-runtime-v1.60.0.png`
- `character-combat-atlas-runtime-v1.77.0.png`
- `enemy-sprite-atlas-runtime-v1.77.0.png`

## Deliberately inactive files

`world-map-region-marker-atlas-runtime-v1.30.0.png` is not activated. It is an older generic icon sheet that overlaps the newer v1.65 semantic regional-landmark atlas and the v1.29 object/token families. Wiring it as “latest” would replace newer, typed art with older ambiguous cells. It remains an art reference until it is either retired or assigned a distinct typed feature.

Legacy loaded-but-unused sheets such as the enemy roster and character inventory UI were not removed in this release because they have no visible render call sites and are outside the high-visibility wiring change.

## Required release checks

1. Unity rule smoke: exact pins, latest-family guard, atlas dimensions, cell alpha/coverage/gutters, and semantic index tables.
2. Unity runtime boot smoke: live selected texture names and dimensions, token/class fallbacks, and startup-art parity.
3. Windows build/package: packaged manifest contains each approved file exactly once.
4. Player captures: Local Map, Region Map, Midgaard dialogue, and combat at supported resolution.

Save schema remains v22.
