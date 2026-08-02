# Accepted v2.4 NPC and Player Sprite Handoff

These are accepted v2.4 runtime atlases, exact-pinned as `WorldNpcCitizenAtlas` and `PlayerExplorationRoleAtlas`. They expand ambient world population and player exploration silhouettes without replacing named Midgaard NPC identities, NPC portraits, the 35-cell combat character atlas, or the existing mixed world-token atlas. Combined release verification remains pending.

## Delivered NPC files

- `Docs/ArtReferences/source-world-npc-citizen-atlas-v2.4.0-generated.png`
- `Docs/ArtReferences/source-world-npc-citizen-atlas-v2.4.0-alpha.png`
- `Docs/ArtReferences/world-npc-citizen-atlas-runtime-v2.4.0.png`
- `Docs/ArtReferences/world-npc-citizen-atlas-runtime-v2.4.0-validation.json`
- `Docs/ArtReferences/world-npc-citizen-atlas-runtime-v2.4.0-prompt.txt`

NPC row-major indices:

0. Human lamplighter
1. Ashling fishmonger
2. Dusk-elf tailor
3. Stoneborn mason
4. Fenkin apothecary
5. Human road pilgrim
6. Ashling gravedigger
7. Dusk-elf caravan guide

## Delivered player files

- `Docs/ArtReferences/source-player-exploration-role-atlas-v2.4.0-generated.png`
- `Docs/ArtReferences/source-player-exploration-role-atlas-v2.4.0-alpha.png`
- `Docs/ArtReferences/player-exploration-role-atlas-runtime-v2.4.0.png`
- `Docs/ArtReferences/player-exploration-role-atlas-runtime-v2.4.0-validation.json`
- `Docs/ArtReferences/player-exploration-role-atlas-runtime-v2.4.0-prompt.txt`

Player row-major indices:

0. Shield
1. Pike
2. Bow
3. Knife
4. Mender
5. Ember
6. Hex
7. Ward

## Runtime contracts

Both atlases are exact 1536 by 768 RGBA images, arranged as 4 columns by 2 rows with 384-pixel square cells. Every cell has zero alpha inside its outer 20-pixel gutter. The NPC cells retain 26.9% to 33.0% visible coverage; player cells retain 22.1% to 42.6% coverage. Neither atlas contains visible pixels in the bright-magenta key range.

`Docs/ArtReferences/*` is locally excluded through `.git/info/exclude`. The release checkpoint must deliberately force-add the ten reviewed art/provenance files; ordinary `git status` does not reveal whether these ignored inputs have been promoted.

## Original integration recommendation and status

Source integration implements the loader, mapping, presentation, and focused coverage described in items 1-7 below; combined test and package execution remains pending.

1. Add separately named `WorldNpcCitizenAtlas` and `PlayerExplorationRoleAtlas` manifest pins. Do not replace `MidgaardNpcAtlas`, `NpcPortraitAtlas`, `CharacterCombatAtlas`, or `WorldMapTokenSpriteAtlas`.
2. Load and validate each candidate as an exact 4 by 2 transparent sheet with all eight populated cells and a 20-pixel safe gutter.
3. Use the citizen atlas first for deterministic non-interactive street and road ambience. These sprites do not yet have matching portrait cells or dialogue identities; do not silently impersonate named NPCs.
4. Give ambient citizens stable coordinate seeds and district-appropriate professions. Keep entrances, route lines, interactable objects, and certified safe-road guidance unobstructed.
5. Route single-character exploration tokens through the dedicated player atlas using the stable role order above. Continue using the existing party-group token when more than one party member is represented, and retain the existing mixed atlas as a fallback.
6. The player sheet is exploration art only. It is deliberately race-neutral at map scale and must not replace race/class combat sprites or inventory portraits.
7. Add focused rule/runtime coverage for manifest pins, geometry, safe gutters, index mappings, citizen draw placement, player role selection, party-group fallback, and no click-target or encounter changes from ambient NPC art.

## Preserved integration boundaries

The earlier combat, opening-title/Grand Hearth, and regional-site coordination blocks are released. Integration preserved the accepted combat layout, title/Tavern presentation, stable Grand Hearth IDs and room bounds, New Game -> Muster -> first-spawn behavior, portal flow, starting tile label, cartography table cell 7, and blue company road chest cell 17. Ambient citizens remain non-interactive and off the tutorial lane; named NPCs, dialogue portraits, combat race/class art, inventory portraits, and the multi-member group marker retain their established ownership.
