# Ashen Halls Art Intake

This project uses generated and hand-cleaned original art atlases from `Docs/ArtReferences/`.
High-visibility runtime art loads an approved exact filename first, then uses a semantic-version-sorted development fallback. Release builds fail when an approved family has a newer file that has not been reviewed and pinned.

## Active v2.17.0 title menu icon contract

`title-menu-icon-atlas-runtime-v2.16.0.png` is the dedicated transparent title-menu icon strip. It is an exact 1280 by 256 RGBA image arranged as five 256-pixel square cells in stable row-major order: Continue scroll and chevron, New Game road gate and spark, Settings eight-tooth cog, Exit arched door and outward arrow, and development-only Beta Lab flask and spark. The bold silhouettes, dark outlines, and clear cell gutters are designed to remain readable when the parchment menu scales down.

`Tools/BuildTitleMenuIconAtlas.ps1` is the deterministic repo-native authoring source. A repeat generation produces SHA-256 `B4AA9DC263E70805C3276E36541C0E81316AF80B93C8DE594915BB04CFEFD278`; the accepted atlas is 59.31% transparent, and its five cells contain 42.44%, 46.37%, 39.86%, 33.65%, and 41.14% visible pixels. Runtime rejects any replacement that does not match the exact geometry, then falls back to the older tavern UI sheet. The approved manifest pin and `title-menu-icon-atlas-runtime-` package family select only this runtime PNG from the locally excluded ArtReferences workspace.

## Active v2.9.0 combat power contract - automated, package, and visual gates passed

The v2.9 power-art set exact-pins three runtime sheets. `ability-icon-atlas-runtime-v2.9.0.png` is an RGBA 4 by 7 sheet that preserves cells 0-23, maps Sunder, Shadowstep, and Quick Shot to cells 24-26, and keeps cell 27 transparent. `signature-spell-icon-atlas-runtime-v2.9.0.png` remains an RGBA 7 by 8 sheet, preserves cells 0-50, and maps Dawn Pulse, Cinderstorm, Grave Hook, Soul Veil, and Ashen Curse to cells 51-55. `combat-spell-effects-atlas-runtime-v2.9.0.png` is an exact RGBA 1280 by 1280, 4 by 4 sheet with 320-pixel cells, at most 280 by 280 visible content per cell, and at least 20 transparent pixels on every side at alpha greater than 8.

The ability, spell, and effects runtime SHA-256 values are `D36CC925F4560C6D04CE63DE3B31CC2B4DD0B0EAF7DA48A63602595A7F81EF28`, `A2E14C5248B564FB82876DD18D4B1BCA9E86D602B68892CD120C62B74B3DE4D9`, and `F3FE0D14FD6BE218AB64AE177FDA741A4D6F7694BBFF96C89C576CC866D5A92A`. The reviewed provenance set contains thirteen exact files: three runtime PNGs and ten chroma-key, alpha, prompt/design, and validation companions documented by `EARLY_PROGRESSION_V2.9.md` and `COMBAT_EFFECTS_V2.9_ART_HANDOFF.md`. Because `Docs/ArtReferences/` is locally excluded, release integration must promote those thirteen files by exact path; only the three runtime PNGs are package-selected.

Combined RuleSmoke, full RuntimeBoot, focused combat-UI runtime smoke, SpriteArtRuntimeSmoke, the clean Windows build, packaged-art integrity gate, and clean-extracted packaged boot pass with the exact pins and live mappings. The package contains 83 approved runtime art files. Nine built-player combat, power-book, and map captures pass deterministic validation and manual visual review under `QA/v2.9.0-release-visuals-741451a`; capture-set SHA-256 is `006b36b45b3e43672286bf4edc7d410f482ee1b16e8c26168b7ae375fae2c321`. Physical-controller review and a complete human playthrough remain manual.

## Active v2.8.0 Grand Hearth ambience contract - automated, build, and visual gates passed

`grand-hearth-ambience-atlas-runtime-v2.8.0.png` is the exact-pinned transparent ambience atlas: RGBA 1536 by 1024, 3 by 2, six 512-pixel cells ordered warm hearth light, storm-door rain spill, wall sconce, ember haze, rain-window reflection, and patron contact shadow. Every cell keeps at least 36 transparent pixels on every side at alpha > 8 and the accepted runtime contains zero visible bright-magenta residue.

The layer is additive to the v2.7 floor and set-piece sheets. Deterministic presentation places it between established floor, patrons, and fixtures, rotates the storm spill into the east-west departure axis, and reduces Region Map opacity. It introduces no `MapObject`, collision, interaction, save data, or animation state. The accepted source set is the runtime PNG plus generated, alpha, prompt, and validation companions listed in `GRAND_HEARTH_VISUAL_V2.8_HANDOFF.md`; only the runtime PNG may enter the player package.

RuleSmoke, focused SpriteArtRuntimeSmoke, full RuntimeBoot, the Windows build, six supported-resolution Local/Region captures, the deterministic visual packet, canonical packaging, and clean-extracted packaged boot pass. The release-integrity record reports clean tracked source, save v25, player exit 0, 83 packaged art files, and the exact ambience-atlas hash. Physical-controller review and a complete human walkthrough remain manual.

## Active v2.7.0 Grand Hearth companion contract - automated, visual, and package gates passed

`grand-hearth-floor-atlas-runtime-v2.7.0.png` is the exact-pinned opaque floor atlas: an exact 1536 by 1024 TrueColor 3 by 2 sheet with 512-pixel cells ordered dark civic wood A, dark civic wood B, oxblood-and-charcoal company runner, company runner medallion, heat-darkened Grand Hearth apron, and storm-door threshold. `grand-hearth-setpiece-atlas-runtime-v2.7.0.png` is the exact-pinned transparent set-piece atlas: an exact RGBA 1536 by 1024 3 by 2 sheet with 512-pixel cells and a 32-pixel clear gutter, ordered monumental Grand Hearth, interior storm doors, company register, road-company banner, rain-blue window, and road-company stores.

These sheets are additive to the existing v1.61 Midgaard interior tile and prop atlases, which remain active elsewhere and remain the fail-closed Grand Hearth fallback. Exact manifest pins, loaders, deterministic floor selection, set-piece mappings, and live draw adapters are integrated. The accepted source set contains the two runtime atlases plus seven source/alpha/prompt/validation companions: nine files in all, listed in `GRAND_HEARTH_ART_V2.7_HANDOFF.md` and deliberately promoted because `Docs/ArtReferences/` is locally ignored. Only the two runtime PNGs may enter the player package.

Floor cells 0/1 use the final edge-balance ImageGen refinement, with the floor prompt and validation record updated to match. Fallback set-piece presentation is restricted to the owning fixture footprint and covered by assertions.

RuleSmoke, focused SpriteArtRuntimeSmoke, full RuntimeBoot, and the final Windows build pass for v2.7. Six built-player captures under `QA/v2.7.0-grand-hearth` cover Local and Region at 960 by 600, 1280 by 720, and 1920 by 1080; each reports `complete=True`, `failure=None` and passes visual review without checkerboard seams, magenta residue, backplates, cropping, or route obstruction. Canonical package inventory and clean-extracted packaged boot also pass from a clean tracked source. `QA/v2.7.0-release-integrity/release-integrity-manifest.json` is authoritative for the 82-file inventory, batch exploration boot, player exit, and both runtime-atlas hashes. Physical-controller review and a complete human playthrough remain manual. The integration preserves the stable room, gameplay, fallback props, and save schema v25.

## Active v2.6.0 Town Hall patron reuse contract

Town Hall's expanded Grand Hearth authors six fixed gathering patrons from the already approved `world-npc-citizen-atlas-runtime-v2.4.0.png`: Dusk-elf tailor, Stoneborn mason, Human lamplighter, Dusk-elf caravan guide, Human road pilgrim, and Ashling fishmonger. This is a new placement/presentation contract, not a new art asset or identity system. No v2.4 handoff file, atlas geometry, cell order, package pin, or provenance record changes.

Town Hall patrons are deterministic interior presentation figures, separate from the procedural exterior ambient-citizen system described below. They remain on six authored open-floor cells and off the tutorial company runner; the player can walk through them. They have no `MapObject`, collision, hover response, Talk prompt, dialogue, or saved identity and must never be substituted for a named NPC or portrait. The exterior generator remains prohibited from rooms, including Town Hall, so it cannot add or remove these six authored figures.

RuleSmoke, focused sprite-art runtime smoke, full RuntimeBoot, and the Windows build pass for this reuse contract. Four direct built-player Local/Region captures at 1280 by 720 and 1920 by 1080 report `complete=True`, `failure=None`; visual inspection confirms all six distinct figures render cleanly without blocking the company runner. Canonical packaging and clean-extracted packaged boot also pass; the release-integrity record reports `sourceDirty=false`, `cleanExtractLaunch=true`, `playerExitCode=0`, and `packagedArtCount=80`. The package continues to contain the one approved v2.4 citizen atlas rather than a new patron sheet.

## Active v2.4.0 world-map companion contracts

`world-threat-habitat-atlas-runtime-v2.4.0.png` is the approved exact transparent habitat atlas. Its stable row-major cells are Rat Warren, Plague-Bell Midden, Kobold Ambush Camp, Kobold Shaman Totem Yard, Drow Moon-Silk Watchpost, Undead Ossuary, Demon Breach, and Ruined Road Waystation. Runtime presentation anchors one stationary illustration at each threat's existing `HomeX`/`HomeY`, beneath its independently moving token, and suppresses habitat art on certified safe roads. Active threats use their faction/archetype habitat; inactive threats use the neutral ruined-waystation aftermath cell. The layer does not create encounters, move a home, or alter save data.

`world-npc-citizen-atlas-runtime-v2.4.0.png` is the approved exact transparent ambient-citizen atlas. Its stable row-major cells are Human lamplighter, Ashling fishmonger, Dusk-elf tailor, Stoneborn mason, Fenkin apothecary, Human road pilgrim, Ashling gravedigger, and Dusk-elf caravan guide. Citizens are coordinate-stable, district-appropriate, non-interactive exterior ambience, including suitable streets and non-safe roads after the opening tutorial retires. They stay off the Grand Hearth tutorial lane, current guidance, certified safe roads, rooms, water, hazards, entrances, authored regional sites, and cells with interactables, and they never impersonate a named NPC or dialogue portrait.

`player-exploration-role-atlas-runtime-v2.4.0.png` is the approved exact transparent solo exploration atlas. Its stable row-major cells are Shield, Pike, Bow, Knife, Mender, Ember, Hex, and Ward. Exactly one represented party member uses this sheet; multi-member parties retain the mixed world-token atlas's group marker, and the mixed sheet remains the fallback. This atlas does not replace race/class combat sprites or inventory portraits.

All three sheets are exact 1536 by 768 RGBA images arranged as 4 columns by 2 rows of 384-pixel square cells, with zero alpha in the outer 20 pixels of every cell and no visible bright-magenta key residue. The reviewed authoring and provenance set is the fifteen exact files named in `WORLD_MAP_V2.4_ART_HANDOFF.md` and `CHARACTER_ART_V2.4_HANDOFF.md`. Those files must be deliberately promoted despite any local `.git/info/exclude`; only the three normalized runtime PNGs are copied into the player package.

## Active v2.3.0 regional set-piece contract

`world-area-setpiece-atlas-runtime-v2.3.0.png` is the approved exact 4 by 2 transparent regional set-piece atlas. It is 1536 by 768 pixels with 384-pixel cells and at least 20 pixels of transparent gutter around every illustrated subject. Row-major cells are Green Shrine Training Ring, Old Quarry Forge, Gloam Deep Crypt, Glass Lore Library, Dusk Market Hideout, Red Gate Seal, Salt Cistern Gate, and Ash Fen Ancient Grove. Subjects share a bottom-center ground anchor and are drawn as large map illustrations at both Local and Region scales; do not bake labels, route strokes, selection frames, square backplates, or ambience marks into replacement art.

The preserved authoring set is `source-world-area-setpiece-atlas-v2.3.0-generated.png`, `source-world-area-setpiece-atlas-v2.3.0-alpha.png`, `world-area-setpiece-atlas-runtime-v2.3.0-prompt.txt`, and `world-area-setpiece-atlas-runtime-v2.3.0-validation.json`. Runtime validation requires exact RGBA geometry, all eight populated cells, boundary-safe alpha, matching site-to-cell mappings, and the exact manifest pin. Only the normalized runtime atlas is packaged.

## Active v2.2.0 Region Map marker contract

`world-map-region-marker-atlas-runtime-v2.2.0.png` is the approved 5 by 4 transparent Region Map marker atlas. Its 20 row-major cells cover civic/service actors, outer-road sites, danger, travel, party, Midgaard, and atmospheric reserve art. Runtime validation requires exact 1400 by 1120 geometry, safe gutters, visible alpha in every cell, and the matching manifest pin. The generated chroma-key source, cleaned alpha source, prompt, and validation report remain beside the runtime sheet under `Docs/ArtReferences`; only the runtime sheet is packaged.

## Source-Control Promotion

`Docs/ArtReferences/` is both the runtime-art input folder and a local
authoring/history workspace. A release checkpoint commits the exact approved
and package-selected PNGs, not every raw generation, prompt, validation,
contact-sheet, or superseded runtime file in that folder. Preserve those
authoring artifacts in the local/source archive; add promoted PNGs by exact
filename and never use a broad repository add for an art pass.
Promoted runtime PNGs live directly in `Docs/ArtReferences/`; nested
authoring folders are never copied into a player package.

An accepted handoff may explicitly promote a reviewed provenance set as an
exception to the default runtime-only checkpoint. For v2.4.0, force-add the
fifteen exact habitat, citizen, and player-role files named by the two handoff
documents; do not broadly add the locally excluded ArtReferences folder. The
twelve source, alpha, prompt, and validation companions remain source-only,
while the three normalized runtime PNGs remain package-selected.

`Tools/BuildAndPackageWindows.ps1` treats `Docs/PACKAGED_ART.txt` as the
authoritative packaged PNG inventory. Its default release path requires
committed release inputs, requires every package-selected PNG to be tracked,
compares staged and source hashes, and records all packaged PNG hashes in the
release-integrity manifest. `-AllowDirtySource` is only for an explicitly
requested development package and must not be used for a distributable. The
two build/smoke skip switches also require that development override, so a
cleanly labeled final package cannot silently reuse an older staged player or
bypass the clean-extraction boot. Development output uses a separate `-dev`
zip and development evidence folder; it cannot replace the canonical release
artifact or evidence.

A developer may locally exclude the retained ArtReferences authoring/history
pile through `.git/info/exclude` to keep routine status readable. That local
convenience never promotes an asset: the package tracking gate remains the
authority and will reject any newly selected runtime PNG that was not
explicitly committed.

## Runtime Naming

Use these prefixes when creating replacement or expansion atlases:

- `splash-title-reference-v0.xx.png`
- `tavern-backdrop-runtime-v0.xx.png`
- `tavern-ui-atlas-runtime-v0.xx.png`
- `creature-sprite-atlas-runtime-v0.xx.png`
- `combat-sprite-atlas-runtime-v0.xx.png` (accepted alias for creature/combat board sprites)
- `combat-sprite-sheet-alpha-v0.xx.png`
- `enemy-roster-atlas-runtime-v0.xx.png`
- `enemy-sprite-atlas-runtime-vX.XX.X.png`
- `character-combat-atlas-runtime-vX.XX.X.png`
- `enemy-world-object-atlas-runtime-v0.xx.png`
- `roaming-threat-atlas-runtime-vX.XX.X.png`
- `boss-enemy-atlas-runtime-v0.xx.png`
- `kobold-route-atlas-runtime-v0.xx.png`
- `kobold-boss-atlas-runtime-v0.xx.png`
- `kobold-cave-prop-atlas-runtime-v0.xx.png`
- `kobold-combat-terrain-atlas-runtime-v0.xx.png`
- `combat-terrain-atlas-runtime-v0.xx.png`
- `world-environment-atlas-runtime-v0.xx.png`
- `world-object-atlas-runtime-v0.xx.png`
- `world-map-prop-atlas-runtime-v0.xx.png`
- `world-map-material-atlas-runtime-v0.xx.png`
- `world-map-exploration-tile-atlas-runtime-v0.xx.png`
- `world-map-landmark-atlas-runtime-v0.xx.png`
- `world-map-overlay-atlas-runtime-v0.xx.png`
- `world-map-progression-overlay-atlas-runtime-v0.xx.png`
- `world-map-ui-atlas-runtime-v0.xx.png`
- `world-area-setpiece-atlas-runtime-vX.XX.X.png`
- `world-threat-habitat-atlas-runtime-vX.XX.X.png`
- `world-npc-citizen-atlas-runtime-vX.XX.X.png`
- `player-exploration-role-atlas-runtime-vX.XX.X.png`
- `world-map-token-sprite-atlas-runtime-vX.XX.X.png`
- `midgaard-town-atlas-runtime-v0.xx.png`
- `midgaard-tile-atlas-runtime-v0.xx.png`
- `midgaard-npc-atlas-runtime-vX.XX.X.png`
- `midgaard-sewer-atlas-runtime-v0.xx.png`
- `story-card-atlas-runtime-v0.xx.png`
- `npc-portrait-atlas-runtime-vX.XX.X.png`
- `route-scaffold-atlas-runtime-v0.xx.png`
- `dungeon-scaffold-atlas-runtime-v0.xx.png`
- `faction-banner-atlas-runtime-v0.xx.png`
- `service-scaffold-atlas-runtime-v0.xx.png`
- `quest-world-object-atlas-runtime-v0.xx.png`
- `item-equipment-atlas-runtime-v0.xx.png`
- `item-inventory-atlas-runtime-v0.xx.png` (accepted alias for item/equipment icons)
- `item-icon-atlas-runtime-v0.xx.png`
- `unique-item-atlas-runtime-v0.xx.png`
- `inventory-consumable-atlas-runtime-v0.xx.png`
- `class-icon-atlas-runtime-v0.xx.png`
- `combat-ui-atlas-runtime-v0.xx.png`
- `combat-ui-panel-atlas-runtime-v0.xx.png`
- `combat-hud-ui-atlas-runtime-v0.xx.png`
- `combat-command-icon-atlas-runtime-v0.xx.png`
- `ability-icon-atlas-runtime-v0.xx.png`
- `signature-spell-icon-atlas-runtime-v0.xx.png`
- `lightning-spell-icon-atlas-runtime-vX.XX.X.png`
- `power-book-state-icon-atlas-runtime-vX.XX.X.png`
- `ranger-ability-effect-atlas-runtime-v0.xx.png`
- `spellbook-combat-ui-atlas-runtime-v0.xx.png`
- `combat-spellbook-ui-atlas-runtime-v0.xx.png`
- `magic-ui-atlas-runtime-v0.xx.png`
- `spell-card-icons-reference-v0.xx.png`
- `ember-spell-effects-atlas-runtime-v0.xx.png`
- `epic-spell-effects-atlas-runtime-v0.xx.png`
- `combat-spell-effects-atlas-runtime-v0.xx.png` (accepted alias for large spell/effect art)
- `spell-animation-atlas-runtime-v0.xx.png`
- `combat-spell-float-atlas-runtime-v0.xx.png`

Use a monotonically increasing version suffix. Example: `creature-sprite-atlas-runtime-v0.51.png`.

## Atlas Layouts

- Creature, enemy sprite, spell animation, combat terrain, kobold route, kobold boss, kobold cave prop, and kobold combat terrain atlases are currently read as 4 by 4 grids.
- `character-combat-atlas-runtime-v1.93.0.png` is the expanded transparent 5 by 7 player grid described below.
- `ability-icon-atlas-runtime-*` is read as a 4 by 2 grid for older sheets, a 4 by 3 grid for v0.73+ sheets, a 4 by 5 grid for the v1.31-v1.99 warrior/rogue/ranger sheet, a 4 by 6 grid for v2.0, or the active 4 by 7 v2.9 sheet. Cells 24-26 are Sunder, Shadowstep, and Quick Shot; cell 27 is a transparent reserve; cells 0-23 remain pixel-identical to v2.0.
- `signature-spell-icon-atlas-runtime-*` uses the active transparent 7 by 8 v2.9 contract. Cells 0-50 remain pixel-identical to v2.0; cells 51-55 are Dawn Pulse, Cinderstorm, Grave Hook, Soul Veil, and Ashen Curse.
- `lightning-spell-icon-atlas-runtime-*` is an exact transparent 4 by 2 grid. The v1.97 contract maps Arc Spark, Chain Lightning, Arcane Tempest, Thunder Step, and Thunderclap to cells 0, 1, 3, 4, and 5; Storm Cage, Storm Ward, and Thunderhead remain reserved in cells 2, 6, and 7 with distinct forward-compatible symbols.
- `power-book-state-icon-atlas-runtime-*` is an exact transparent 4 by 3, 256 by 192 grid of 64-pixel UI symbols. It reinforces book interaction and availability states but never replaces the adjacent text.
- `magic-ui-atlas-runtime-*` is read as a transparent 4 by 4 grid for shared spell schools, utility effects, and formula fallbacks.
- `ranger-ability-effect-atlas-runtime-*` is read as a 4 by 4 grid for ranger/missile battlefield effects.
- Most UI, item, world object, world-map, story-card, and icon atlases are currently read as 5 by 4 grids. The expanded world-map material and blocked-terrain contracts below are explicit exceptions.
- `roaming-threat-atlas-runtime-*` is a transparent 5 by 4 grid. The v1.62.0 contract reserves cells 0-19 for rat, ratfolk, kobold, demon, drow, undead, elite, boss-escort, and encounter-marker silhouettes; keep each figure centered with a common foot baseline and clear gutters.
- `world-map-ui-atlas-runtime-*` is read as a transparent 5 by 4 grid and is used by exploration/map command buttons.
- `world-area-setpiece-atlas-runtime-v2.3.0.png` is an exact transparent 4 by 2, 1536 by 768 grid of eight large regional illustrations. Its stable row-major order follows the active v2.3.0 regional set-piece contract above; each 384-pixel cell keeps a bottom-center anchor and at least 20 pixels of clear gutter.
- `world-threat-habitat-atlas-runtime-v2.4.0.png` is an exact transparent 4 by 2, 1536 by 768 grid of eight bottom-anchored threat homes. Each 384-pixel cell keeps a 20-pixel clear gutter; visible coverage at alpha 8 remains between 46% and 51%.
- `world-npc-citizen-atlas-runtime-v2.4.0.png` is an exact transparent 4 by 2, 1536 by 768 grid of eight ambient professions. Each 384-pixel cell keeps a 20-pixel clear gutter; visible coverage at alpha 8 remains between 26% and 34%.
- `player-exploration-role-atlas-runtime-v2.4.0.png` is an exact transparent 4 by 2, 1536 by 768 grid of eight solo exploration roles. Each 384-pixel cell keeps a 20-pixel clear gutter; visible coverage at alpha 8 remains between 22% and 43%.
- `tavern-ui-atlas-runtime-*` is read as a transparent 5 by 4 grid and is used by the tavern/title menu buttons.
- `combat-command-icon-atlas-runtime-v1.99.0.png` is the active exact 5 by 4, 1280 by 1024 transparent command contract. Every cell is 256 by 256 with at least 18 pixels of transparent gutter; the live Move, Attack, Cast, Guard, Elixir, End Turn, and Skills mappings remain at cells 0, 1, 2, 3, 4, 5, and 7. The large unframed silhouettes are authored for 56-72 pixel HUD presentation, so do not bake button frames, text, keycaps, or interaction-state rails into replacement art.
- Current active world/Midgaard/combat terrain sheets: `world-map-material-atlas-runtime-v1.92.0.png`, `world-map-exploration-tile-atlas-runtime-v1.68.0.png`, `world-map-overlay-atlas-runtime-v0.80.png`, `world-map-progression-overlay-atlas-runtime-v0.63.png`, `combat-terrain-atlas-runtime-v1.5.8.png`, `midgaard-tile-atlas-runtime-v1.6.3.png`, `midgaard-city-prop-atlas-runtime-v1.29.0.png`, `midgaard-gate-atlas-runtime-v1.93.0.png`, and `midgaard-wall-atlas-runtime-v1.91.0.png`.
- `midgaard-gate-atlas-runtime-v1.93.0.png` is the active exact 5 by 4, 1280 by 1024 gate contract. Cells 0 and 1 preserve the approved front-view sealed/open variants; cells 6 and 7 are a compact wall-aligned West/East pair with two low bastions above and below an open horizontal passage. Runtime maps sealed north and south to cell 0, West to cell 6, and East to cell 7. Side-gate cells must keep local bounds near 62 by 224 pixels, at least 16 pixels of gutter, a fully transparent local-y 104-151 road band, an exact horizontal visible-RGB/alpha mirror, and the authored town-side mass bias that distinguishes West from East. Do not replace them with a front-elevation facade or horizontal wall wings.
- `midgaard-wall-atlas-runtime-v1.91.0.png` is the active exact 5 by 4, 1280 by 1024 wall contract. Cells 0-3 are north, south, west, and east straight runs; cells 4-7 are the four inward-facing corners; cells 8 and 9 are horizontal and vertical structural accents. Continuous foundations and gate joins are renderer-owned: horizontal foundations are 0.56 cells in Local Map and 0.52 in Region Map, while vertical foundations are 0.36 / 0.34 to fit the narrower authored masonry. Open east/west gates inherit the narrow vertical join only above and below the travel lane; their underlying straight-wall tile is suppressed and no sill may cross the road. Replacement art must preserve these connection directions and avoid opaque square-cell backgrounds.
- `world-map-token-sprite-atlas-runtime-v1.91.0.png` is the active exact 5 by 4, 1280 by 1024 mixed-token contract. Cell 0 is the compact near-portrait group marker; cells 1-8 remain the shield, bow, knife, mender, ember, hex, ward, and pike role fallbacks. The dedicated v2.4 player-role atlas now supplies exact one-member exploration silhouettes, while this mixed sheet retains multi-member group ownership and fallback coverage. Keep the group silhouette centered, vertically readable, and inside the shared safe gutter rather than returning to a wide horizontal lineup.
- `world-map-material-atlas-runtime-v1.92.0.png` is the active exact 8 by 8, 2048 by 2048 passable-ground contract. Each semantic material owns four adjacent 256-pixel variants. Cells 0-15 contain the coherent v1.92 civic families and cells 28-31 contain the matched packed-dirt approach bank; cells 16-27 and 32-63 remain pixel-identical to v1.68.
- `world-map-exploration-tile-atlas-runtime-v1.68.0.png` is the active exact 5 by 8, 1280 by 2048 blocked/fallback terrain contract. Cells 0-19 preserve the repaired v1.24.2 bank and cells 20-39 add blocked-terrain variants. This is a deliberate two-bank expansion, not the older irregular 5 by 7 experiment.
- `unique-item-atlas-runtime-*` is read as a 5 by 4 grid and is checked before generic equipment art.
- `combat-ui-panel-atlas-runtime-*` is read as a 5 by 4 grid and used as subtle panel chrome/backdrop art.
- Current active tavern/combat/UI/ability/effect sheets include `tavern-ui-atlas-runtime-v1.5.9.png`, `world-map-ui-atlas-runtime-v1.6.0.png`, `combat-ui-panel-atlas-runtime-v0.72.png`, `ability-icon-atlas-runtime-v2.9.0.png`, `signature-spell-icon-atlas-runtime-v2.9.0.png`, `lightning-spell-icon-atlas-runtime-v1.97.0.png`, `power-book-state-icon-atlas-runtime-v1.97.0.png`, `magic-ui-atlas-runtime-v1.31.0.png`, `spellbook-combat-ui-atlas-runtime-v1.24.0.png`, `combat-spell-effects-atlas-runtime-v2.9.0.png`, `spell-animation-atlas-runtime-v1.49.0.png`, `combat-command-icon-atlas-runtime-v1.99.0.png`, `combat-hud-ui-atlas-runtime-v0.73.png`, `combat-spell-float-atlas-runtime-v0.73.png`, and `ranger-ability-effect-atlas-runtime-v0.73.png`.
- Midgaard town, tile, NPC, and sewer atlases are read as 5 by 4 grids.
- `npc-portrait-atlas-runtime-*` is read as a transparent 5 by 4 grid.
- `route-scaffold-atlas-runtime-*`, `faction-banner-atlas-runtime-*`, and `service-scaffold-atlas-runtime-*` are read as 5 by 4 grids.
- `dungeon-scaffold-atlas-runtime-*` is read as a 4 by 4 square grid.
- `combat-sprite-sheet-alpha-*` uses the existing combat sprite sheet crop logic, so preserve a compatible 4 by 4-ish composition unless code is updated.
- Keep cells clearly separated. Avoid labels, watermarks, and decorative borders inside cells.
- Keep sprites centered with consistent foot/ground anchors. This matters more than raw detail.

## Generic Enemy Sprite Atlas Cells

`enemy-sprite-atlas-runtime-vX.XX.X.png` is the main combat-board sheet for common foes. It is read as a transparent 4 by 4 grid and is pinned by exact approved filename.

The current contract is `enemy-sprite-atlas-runtime-v1.77.0.png`, an exact 1024 by 1024 RGBA sheet with 256-pixel cells and at least 18 pixels of transparent gutter:

- Cell 0: Kobold scout / raider with dagger and small buckler.
- Cell 1: Kobold spearman, reserved for a future reach-role kobold.
- Cell 2: Kobold archer / slinger.
- Cell 3: Kobold shaman.
- Cell 4: Kobold fire-caster / bone wizard.
- Cell 5: Kobold king fallback when no dedicated boss atlas is present.
- Cell 6: Armored kobold guard / shieldbearer / sentry.
- Cell 7: Kobold berserker, reserved for a future brute-role kobold.
- Cell 8: Ratfolk knife fighter / cutthroat / generic ratfolk.
- Cell 9: Ratfolk slinger, reserved for a future ranged ratfolk.
- Cell 10: Ratfolk plague priest / cistern cleric.
- Cell 11: Ratfolk tunnel mage.
- Cell 12: Drow skirmisher / blade / scout / crossbow fallback.
- Cell 13: Drow hex priest / mage / bone-priest caster fallback.
- Cell 14: Imp / small demon.
- Cell 15: Lesser demon / heavy fiend.

Pure sewer rats and other non-humanoid creature shapes should use the creature sheet or a future dedicated creature atlas instead of this humanoid-heavy enemy sheet.

## Character Combat Atlas Cells

`character-combat-atlas-runtime-vX.XX.X.png` is the main combat-board and
roster portrait sheet for player characters. Preserve centered silhouettes,
consistent foot anchors, square cells, and at least 18 pixels of gutter.

Current runtime sheet: `character-combat-atlas-runtime-v1.93.0.png`, an exact
1280 by 1792 RGBA sheet with 256-pixel cells. It is a 5-column by 7-row
matrix:

- Race columns, left to right: human, dusk elf, stoneborn, fenkin, ashling.
- Class rows, top to bottom: warrior, rogue, ranger, priest, warlock,
  wizard/mage, paladin.
- The cell index is `class row * 5 + race column`.

Every selectable race/class combination now has a distinct sprite. The 16
approved v1.77 cells are copied pixel-for-pixel into their new semantic
positions; the remaining 19 cells are new v1.93 art. `mage` intentionally
uses the wizard row.

## Ability Icon Atlas Cells

`ability-icon-atlas-runtime-v0.xx.png` is the Combat Skills icon sheet. Older sheets retain their legacy layouts; the current runtime sheet is the exact transparent 4 by 7 `ability-icon-atlas-runtime-v2.9.0.png`. It preserves cells 0-23 pixel-for-pixel and appends three generated, deterministically normalized capstone icons with an empty reserve at cell 27.

- Cell 0: Charge.
- Cell 1: Execute.
- Cell 2: Rally.
- Cell 3: Whirlwind.
- Cell 4: Shield Bash.
- Cell 5: Cleave.
- Cell 6: Stealth.
- Cell 7: Ambush.
- Cell 8: Eviscerate.
- Cell 9: Throw Knife.
- Cell 10: Hamstring.
- Cell 11: Aimed Shot.
- Cell 12: Pinning Shot.
- Cell 13: Volley.
- Cell 14: Scout Mark.
- Cell 15: Broadhead Shot.
- Cell 16: Disrupting Shot.
- Cell 17: Enrage.
- Cell 18: Hunter Focus.
- Cell 19: Smoke Bomb.
- Cell 20: Rift Pounce.
- Cell 21: Abyssal Whirl.
- Cell 22: Soul Rend.
- Cell 23: Dread Roar.
- Cell 24: Sunder.
- Cell 25: Shadowstep.
- Cell 26: Quick Shot.
- Cell 27: transparent reserve.

## Signature Spell Icon Atlas Cells

`signature-spell-icon-atlas-runtime-v0.xx.png` is an exact transparent 7 by 8 grid in its current runtime form, `signature-spell-icon-atlas-runtime-v2.9.0.png`. It preserves cells 0-50 pixel-for-pixel and fills the five prior reserve cells with deterministically normalized early-progression art.

The row-major contract follows `FormulaCatalog.All` exactly:

- Cells 0-6: Tree Cover (GBH), Stone Block (GBX), Hallowed Circle (HLC), Heal (OIC), Cleanse (NVC), Rift Seal (SRF), Ward (TBQ).
- Cells 7-13: Sanctuary Ward (SGW), Regenerate (TNC), Circle Heal (LBC), Circle Ward (TBG), Light Bolt (OBL), Hold Sign (LNH), Still Water (SWR).
- Cells 14-20: Sun Brand (SBN), Fire Spark (FIF), Fire Floor (WBF), Burn Cover (BTF), Ice Slick (WBI), Cold Lance (RCL), Flame Jet (RDF).
- Cells 21-27: Arc Spark (RIG), Fireball (FBL), Fireburst (RLF), Thunderclap (RSG), Iceburst (RBI), Meteor Shower (MTR), Chain Lightning (CLT).
- Cells 28-34: Frost Bind (FRB), Thunder Step (VST), Arcane Tempest (AST), Web Snare (WBK), Poison Gas (WBP), Doom Circle (DMC), Sleep (RMS).
- Cells 35-41: Weaken (RNH), Night Veil (NVL), Bind (RKW), Poison Burst (RPX), Drain Life (INH), Mind Break (RMB), Death Burst (RLM).
- Cells 42-48: Wither (WTR), Dream Smoke (DSM), Summon Imp (IBD), Summon Lesser Demon (IBF), Pact Brand (PBR), Summon Greater Demon (IBG), Abyssal Ascendance (DFA).
- Cells 49-55: Rift Bolt (RBT), Rift Step (VRS), Dawn Pulse (DWP), Cinderstorm (CNS), Grave Hook (GRH), Soul Veil (SLV), Ashen Curse (ACR).

Every formula receives one dedicated cell. Generic `spellbook-combat-ui` and `magic-ui` art remains a defensive fallback only.

The v1.97 refresh replaces cells 5, 7, 9-11, 13, 17, 27, 31, 33-36, 39, 41-48. All other formula cells retain their v1.96 subjects while being normalized into the same safe-gutter contract.

## Power-Book State Icon Atlas Cells

`power-book-state-icon-atlas-runtime-v1.97.0.png` is the exact transparent 4 by 3 state sheet shared by Spellbook and Skillbook. It is derived from `source-power-book-state-icon-atlas-v1.97.0-*`; every 64-pixel cell has an 8-pixel clear gutter. The deterministic code-generated fallback uses the same semantic order.

- Cell 0: Selection.
- Cell 1: Targeting.
- Cell 2: Locked.
- Cell 3: Low Resource.
- Cell 4: No Target.
- Cell 5: Action Used.
- Cell 6: Disabled.
- Cell 7: Blocked.
- Cell 8: Cost.
- Cell 9: Reach.
- Cell 10: Target.
- Cell 11: Preview.

Preview is transient and noncommitting; Selection identifies the retained browse choice; Targeting identifies the armed power. Availability symbols remain paired with their exact text reason so color or icon recognition is never the only communication channel.

## Magic UI Atlas Cells

`magic-ui-atlas-runtime-v0.xx.png` is the shared transparent 4 by 4 magic-symbol grid. Current runtime sheet: `magic-ui-atlas-runtime-v1.31.0.png`, with editable source `source-magic-ui-atlas-v1.31.0.aseprite`.

- Cells 0-3: tree, stone, heal, ward shield.
- Cells 4-7: fire, ice, shock, death.
- Cells 8-11: web, poison gas, hex/curse, pact/summon.
- Cells 12-15: light bolt, cleanse/water, sanctuary, formula glyph.

## Ranger Ability Effect Atlas Cells

`ranger-ability-effect-atlas-runtime-v0.xx.png` is a battlefield effect sheet for missile/ranger skills. It is read as a 4 by 4 grid.

- Cell 0: Aimed Shot line/reticle.
- Cell 1: Pinning Shot snare/pin burst.
- Cell 2: Volley arrow rain.
- Cell 3: Scout Mark target sigil.
- Cell 4: Arrow impact.
- Cell 5: Ricochet/spark.
- Cell 6: Broken cover splinters.
- Cell 7: Stagger/stun cue.
- Cells 8-15: reserved for later traps, hunting marks, bow enchantments, and projectile variants.

## Combat Spell Float Atlas Cells

`combat-spell-float-atlas-runtime-v0.xx.png` is a transparent 5 by 4 grid used as compact icon pips beside floating combat text. It must not be stretched behind the full label. Current runtime sheet: `combat-spell-float-atlas-runtime-v0.73.png`.

- Cells 0-4: physical strike, radiant light, healing, ward/shield, smoke/evade.
- Cells 5-9: poison/death cloud, bleed, cleanse, sleep, web.
- Cells 10-14: fire, cold, shock, death, hex/mind.
- Cells 15-19: stealth, rally/focus, stun/impact, mark/weakness, loot/gold.

`CombatFeedbackRules` owns the semantic mapping. Damage resolution should pass its damage type explicitly; short status labels may use the shared text classifier. Unknown feedback should draw a plain text chip rather than borrowing an unrelated icon.

## World Map UI Atlas Cells

`world-map-ui-atlas-runtime-v0.xx.png` is the exploration command and map cue icon sheet. It is read as a 5 by 4 grid with transparent backgrounds. Current runtime sheet: `world-map-ui-atlas-runtime-v1.6.0.png`, generated from `source-world-map-ui-atlas-v1.6.0-chromakey.png`.

- Cell 0: Camp/rest.
- Cell 1: Recall/temple circle.
- Cell 2: Descend/stairs.
- Cell 3: Elixir/heal vial.
- Cell 4: Wide/close map view.
- Cell 5: Location Details parchment/pin. Do not use rail, train, or track imagery here.
- Cell 6: Journal/route log.
- Cell 7: Armory/gear.
- Cell 8: Location pin.
- Cell 9: Party marker.
- Cell 10: Quest star.
- Cell 11: Merchant/market.
- Cell 12: Talk/dialogue.
- Cell 13: Danger/warning.
- Cell 14: Save/load scroll.
- Cell 15: Settings/audio.
- Cell 16: Road sign.
- Cell 17: City gate.
- Cell 18: Sewer grate.
- Cell 19: Cave entrance.

## Tavern UI Atlas Cells

`tavern-ui-atlas-runtime-v0.xx.png` is the tavern/title menu icon sheet. It is read as a 5 by 4 grid with transparent backgrounds. Current runtime sheet: `tavern-ui-atlas-runtime-v1.5.9.png`, generated from `source-tavern-ui-atlas-v1.5.9-chromakey.png`.

- Cell 0: Tavern hearth emblem.
- Cell 1: Begin the Old Road / adventure start.
- Cell 2: Customize Party.
- Cell 3: Beta Testing / lab door.
- Cell 4: Settings.
- Cell 5: Exit.
- Cell 6: Save scroll.
- Cell 7: Load scroll.
- Cell 8: Campfire.
- Cell 9: Armory shield.
- Cell 10: Spellbook.
- Cell 11: Sword attack.
- Cell 12: Map route marker.
- Cell 13: Dialogue scroll.
- Cell 14: Warning.
- Cell 15: Coin purse.
- Cell 16: Elixir vial.
- Cell 17: City gate.
- Cell 18: Sewer grate.
- Cell 19: Cave entrance.

## Combat Command Icon Atlas Cells

`combat-command-icon-atlas-runtime-v0.xx.png` is the main combat command bar icon sheet. The active `combat-command-icon-atlas-runtime-v1.99.0.png` is an exact transparent 5 by 4, 1280 by 1024 sheet with 256-pixel cells and at least 18 pixels of empty gutter around every silhouette.

- Cell 0: Move / step movement.
- Cell 1: Attack.
- Cell 2: Cast / Spellbook.
- Cell 3: Guard.
- Cell 4: Elixir.
- Cell 5: End Turn / wait.
- Cell 6: Ranged attack.
- Cell 7: Skills / martial action.
- Cell 8: Targeting reticle.
- Cell 9: Blocked / unavailable.
- Cell 10: Fire / impact.
- Cell 11: Multi-impact.
- Cell 12: Heal / support.
- Cell 13: Stealth.
- Cell 14: Dash.
- Cell 15: Volley.
- Cell 16: Break / obstructed.
- Cell 17: Inspect.
- Cell 18: Selected / confirmed.
- Cell 19: Danger / hostile confirm.

The command art expresses semantic identity only. Runtime presentation owns disabled treatment, the ivory keyboard/controller focus outline, and the gold armed-action rail. Cast and Skills may replace their category emblem with the exact pending formula or ability art only while that power is genuinely armed; unarmed or canceled commands return to cells 2 and 7.

## Spell Animation Atlas Cells

`spell-animation-atlas-runtime-v0.xx.png` is the combat tile-glyph animation sheet. It is read as a 4 by 4 grid with transparent backgrounds. Current runtime sheet: `spell-animation-atlas-runtime-v1.5.8.png`, generated from `source-spell-animation-atlas-v1.5.8-chromakey.png`.

- Cells 0-3: Fireball and ember impact frames.
- Cells 4-7: Meteor shower impact frames.
- Cell 8: Death, mind, poison, and hex burst.
- Cell 9: Shock/lightning arc impact.
- Cell 10: Cold/ice lance impact.
- Cell 11: Heal, mend, and priest recovery glyph.
- Cell 12: Status, ward, terrain-control, sanctuary, curse, web, tree, and stone glyph.
- Cell 13: Pact/summon/rift glyph.
- Cells 14-15: reserved for larger future ritual, boss, or ultimate spell flashes.

## Combat Terrain Atlas Cells

`combat-terrain-atlas-runtime-v0.xx.png` is the general combat ground/hazard sheet. It is read as a 4 by 4 grid and is intentionally opaque because it draws floor cells. Current runtime sheet: `combat-terrain-atlas-runtime-v1.5.8.png`.

v1.5.8 preserves the optimized v0.80 terrain sheet but replaces cell 11, the normal fire hazard cell, with a generated burning-stone tile from `source-fire-combat-terrain-cell-v1.5.8.png`. The replacement should read as dangerous cracked stone/lava rather than a generic red tile.

## Item Inventory Atlas Cells

`item-inventory-atlas-runtime-v0.xx.png` is now preferred for Armory gear icons. It is read as a 5 by 4 grid and falls back to `item-equipment-atlas-runtime-*` or `item-icon-atlas-runtime-*` when missing.

- Cell 0: Sword / default weapon.
- Cell 1: Epee or rapier.
- Cell 2: Dagger or knife.
- Cell 3: Axe or hatchet.
- Cell 4: Spear, pike, lance, or halberd.
- Cell 5: Bow.
- Cell 6: Crossbow or sling.
- Cell 7: Staff, wand, focus, or crystal implement.
- Cell 8: Orb.
- Cell 9: Shield or buckler.
- Cell 10: Leather, hide, or pelt armor.
- Cell 11: Chain or mail armor.
- Cell 12: Plate, adamant, mithril, or helm fallback.
- Cell 13: Robe, cloak, or mantle.
- Cell 14: Potion.
- Cell 15: Elixir or mana vial.
- Cell 16: Ration, supply, or bread.
- Cell 17: Scroll.
- Cell 18: Coin or gold.
- Cell 19: Ring, gem, or jewel.

`inventory-consumable-atlas-runtime-*` still handles most resource/food/cache consumables first, so the item inventory sheet is mostly for Armory gear and fallback item rows.

## Unique Item Atlas Cells

`unique-item-atlas-runtime-v0.xx.png` is an optional 5 by 4 sheet checked before the normal item inventory atlas. It is for named boss/relic gear that should not replace generic sword, armor, ring, coin, or scroll cells.

Current runtime sheet: `unique-item-atlas-runtime-v0.71.png`.

- Cell 0: Sword of Unfathomable Darkness, blackglass/violet stolen adventurer's sword with a mild life-drain/vorpal feel.
- Cells 1-19: reserved for later named relics, boss drops, quest weapons, and unique armor.

v0.71 source notes:
- `source-sword-unfathomable-darkness-v0.71-chromakey.png` is the generated chroma-key source.
- `sword-unfathomable-darkness-runtime-v0.71.png` is the cleaned transparent standalone source used to compose the unique-item atlas.

## Kobold Route Atlas Cells

`kobold-route-atlas-runtime-v0.xx.png` is optional route-specific art for Chapter II. It is read as a 4 by 4 grid.

- Cell 0: Dusk Market ambush / bone-whistle marker.
- Cell 1: Smoke cave / kobold cave entrance marker.
- Cell 2: Kobold King hall / crown-and-shield marker.
- Cell 3: Kobold King bust or boss-route portrait marker, reserved for route UI.
- Cell 4: Shield hall banner.
- Cell 5: Bone charm / quest token.
- Cell 6: Cave drum / warning sign.
- Cell 7: Cleared route / victory marker.
- Other cells are reserved for later kobold banners, charms, cave props, and route UI icons.

## Kobold Boss Atlas Cells

`kobold-boss-atlas-runtime-v0.xx.png` is optional dedicated art for the Chapter II boss. It is read as a 4 by 4 grid and is checked before the generic enemy sprite atlas.

Current runtime sheet: `kobold-boss-atlas-runtime-v0.71.png`, generated for the warrior-mage Varkh pass.

- Cell 0: Varkh, Kobold King combat board sprite, healthy/default pose.
- Cell 1: Varkh wounded or enraged combat board sprite.
- Cell 2: Varkh defeated, staggered, or death-fade pose.
- Cell 3: Varkh roster/sidebar portrait.
- Cell 4: Crown, shield, or rally sigil for future boss banners.
- Cell 5: Shield-hall guard emblem.
- Cell 6: Royal bone charm.
- Cell 7: Boss reward/cache marker.
- Cell 8: Varkh phase-two or near-defeat combat board sprite.
- Cell 9: Varkh casting/rally command pose, reserved for later boss-action tells.
- Cell 10: Royal standard fallback icon.
- Cell 11: Shield-hall drum / ambush-warning route icon.
- Cell 12: Throne-room cut-in or king-hall marker.
- Cell 13: Victory trophy / route-complete marker.
- Cell 14: Defeat or fallen-party warning marker.
- Cell 15: Spare boss marker or crossed weapon/charm icon.

If no dedicated `kobold-route-atlas-runtime-*` exists, route markers can borrow cells 4, 6, 10-13 from this boss atlas.

## Kobold Cave Prop Atlas Cells

`kobold-cave-prop-atlas-runtime-v0.xx.png` is optional Chapter II combat terrain/prop art. It is read as a 4 by 4 grid and only overrides obstacle art during kobold ambush, smoke cave, and king hall encounters.

Current runtime sheet: `kobold-cave-prop-atlas-runtime-v0.71.png`, a hybrid sheet retaining prior smoke-cave cells while upgrading King hall barricades, banners, braziers, ice shards, charge/retreat effects, charms, treasure, crown, broken shield, and sword pickup sparkle.

- Cell 0: Smoke-cave tree cover or fungus/tree obstruction.
- Cell 1: Smoke-cave stone block.
- Cell 2: Kobold web/snare.
- Cell 3: Green smoke or poison gas vent.
- Cell 4: Fire brazier or burning hazard.
- Cell 5: Ice/slick floor hazard.
- Cell 6: Shield-hall stone barricade.
- Cell 7: Shield-hall tree/banner/wooden obstruction.
- Cells 8-15: Reserved for stalls, stakes, drums, cave bones, and later route-specific cover.

## Kobold Combat Terrain Atlas Cells

`kobold-combat-terrain-atlas-runtime-v0.xx.png` is optional Chapter II floor art. It is read as a 4 by 4 grid and is used only during kobold ambush, smoke cave, and Kobold King encounters, falling back to the general combat terrain atlas when absent.

- Cell 0: Damp cave floor.
- Cell 1: Smoky cave floor.
- Cell 2: Shield-hall stone floor.
- Cell 3: Dusk Market cave rubble floor.
- Cell 4: Kobold bone summoning glyph.
- Cell 5: Red demon summoning glyph.
- Cell 6: Green smoke hazard floor.
- Cell 7: Dark demon-rift floor mark.
- Cell 8: Icy slick floor.
- Cell 9: Sticky webbed floor.
- Cell 10: Stone block floor base.
- Cell 11: Wooden barricade floor base.
- Cell 12: Fire-scorched hazard floor.
- Cell 13: Poison gas floor haze.
- Cell 14: Royal shield-hall rune floor.
- Cell 15: Cleared cave/victory floor.

v0.67 adds `glyph` and `demonrift` non-blocking terrain kinds. They currently warn the player and add movement friction; actual demon-spawn timers are reserved for a later balance pass.

## Route Scaffold Atlas Cells

`route-scaffold-atlas-runtime-v0.xx.png` is the general exploration-map sheet for future-content nodes. It is read as a 5 by 4 grid and should use transparent backgrounds with clear centered silhouettes.

- Cell 0: Midgaard quest board / contract board.
- Cell 1: Old Road waystone / recall anchor.
- Cell 2: Green Shrine training ring / practice yard.
- Cell 3: Old Quarry forge / crafting workbench.
- Cell 4: Glass Warrens lore library / spell study node.
- Cell 5: Dusk Market faction hideout / scout camp.
- Cell 6: Cistern dungeon gate / authored dungeon entrance.
- Cell 7: Ash Fen ancient grove / hazard-grove route.
- Cell 8: Gloam crypt / undead or bone-priest route.
- Cell 9: Red Gate portal seal / late chapter lock.
- Cells 10-19: Reserved for later shop signs, dungeon keys, route trophies, world-state changes, and quest-turn-in icons.

## Dungeon Scaffold Atlas Cells

`dungeon-scaffold-atlas-runtime-v0.xx.png` is optional future dungeon-route art. It is read as a 4 by 4 grid.

- Cell 0: Cistern gate.
- Cell 1: Gloam crypt.
- Cell 2: Ancient grove entrance.
- Cell 3: Red Gate portal seal.
- Cell 4: Waystone or recall gate.
- Cells 5-15: Reserved for locks, key doors, stair rooms, boss doors, treasure rooms, traps, and dungeon-complete markers.

## Service Scaffold Atlas Cells

`service-scaffold-atlas-runtime-v0.xx.png` is optional UI/world art for noncombat service nodes. It is read as a 5 by 4 grid.

- Cell 0: Quest board.
- Cell 1: Training ring / trainer.
- Cell 2: Lore library / spell lesson.
- Cell 3: Forge or workbench.
- Cell 4: Faction camp / scout contact.
- Cells 5-19: Reserved for shop menus, repair, enchant, inn, hireling, rumor, bounty, and class-training icons.

## Faction Banner Atlas Cells

`faction-banner-atlas-runtime-v0.xx.png` is optional Journal/contact art. It is read as a 5 by 4 grid.

- Cell 0: Midgaard civic banner.
- Cell 1: Dusk Market scout sign.
- Cell 2: Green Shrine priest sign.
- Cell 3: Old Quarry mason sign.
- Cell 4: Glass Warren adept sign.
- Cell 5: Red Gate herald sign.
- Cell 6: Ratfolk/cistern faction marker.
- Cell 7: Kobold smoke faction marker.
- Cell 8: Gloam Court crypt faction marker.
- Cell 9: Demon/portal faction marker.
- Cells 10-19: Reserved for route alliances, faction reputation, quest chains, and future NPC groups.

## World Map Material Atlas Cells

`world-map-material-atlas-runtime-v1.92.0.png` is the authoritative passable world-map ground sheet. It is an opaque 8 by 8 grid with 256-pixel cells. Cells 0-15 replace the former checkerboard-prone civic bank with four coherent, edge-to-edge families whose scale and value remain closely matched within each four-cell run. Cells 28-31 similarly replace the high-variance packed-dirt bank with four matched orientations for roads and gate approaches. Cells 16-27 and 32-63 remain pixel-identical to `world-map-material-atlas-runtime-v1.68.0.png`.

Runtime selection gives all four variants in a material family equal coordinate-deterministic distribution and never changes save data. A tile beneath a static exploration-object footprint uses its family's quiet variant so ground noise does not compete with buildings, gates, props, or characters.

- Cells 0-3: City paving.
- Cells 4-7: Market cobbles.
- Cells 8-11: Temple stone.
- Cells 12-15: Keep stone.
- Cells 16-19: Natural ground / forest loam. Forest excludes the bright open-ground cell 16.
- Cells 20-23: Moss.
- Cells 24-27: Ruined paving / ruined wall material.
- Cells 28-31: Matched packed dirt / bridge-deck fallback orientations. These share exact luminance and edge statistics so road approaches do not become tan square quilts.
- Cells 32-35: Fen mud.
- Cells 36-39: Shallow / deep water material.
- Cells 40-43: Quarry stone / cliff material.
- Cells 44-47: Glass rubble.
- Cells 48-51: Red ash / red basalt material.
- Cells 52-55: Gloam stone.
- Cells 56-59: Cistern brick.
- Cells 60-63: Sewer brick.

Keep these cells quiet and edge-to-edge. Large trees, buildings, gates, shrines, stairs, bridges, and other focal objects belong in the transparent prop/landmark sheets or the blocked/fallback terrain sheet.

The renderer softens a boundary between different recognized passable material families with three progressively lighter inward bands. It does not feather out of bounds or across blocked cells, gates, or threshold-role cells, preserving the structural seams and traversal silhouettes owned by those features.

## World Map Tile Atlas Cells

`world-map-exploration-tile-atlas-runtime-v1.68.0.png` is the opaque blocked/fallback world-map terrain sheet. It is read as a 5 by 8 grid. Cells 0-19 retain the repaired semantic bank; cells 20-39 are the appended blocked-terrain bank.

- Cell 0: Old Road / dirt road.
- Cell 1: Paved ruin or market floor.
- Cell 2: Moss road / green shrine ground.
- Cell 3: Mire, mud, or cistern wet ground.
- Cell 4: Quarry floor.
- Cell 5: Glass warrens floor.
- Cell 6: Ash or red-gate scorched ground.
- Cell 7: Forest wall / green blocked tile.
- Cell 8: Mire wall / flooded blocked tile.
- Cell 9: Stone or cliff wall.
- Cell 10: Red-gate wall.
- Cell 11: Shallow dark water edge.
- Cell 12: Cave floor.
- Cell 13: Road corner or intersection.
- Cell 14: Fogged unexplored tile.
- Cell 15: Reed bank or shoreline.
- Cell 16: Midgaard town cobbles.
- Cell 17: Old bridge plank floor.
- Cell 18: Stair or threshold floor.
- Cell 19: Glowing danger ground.
- Cells 20-24: Ancient broadleaf, conifer, exposed-root, bramble, and fallen-log forest walls.
- Cells 25-29: Reed mire, willow-root blackwater, flooded peat, cattail bog, and drowned-stump mire walls.
- Cells 30-34: Gray basalt, fractured limestone, quarry escarpment, moss-seamed rock, and crumbling slate cliffs.
- Cells 35-38: Ember basalt, iron-red ash, obsidian fissure, and cooled-lava red walls.
- Cell 39: Overgrown ancient ruined wall.

## World Map Landmark Atlas Cells

`world-map-landmark-atlas-runtime-v0.xx.png` is optional exploration object art. It is read as a 5 by 4 grid and is checked before prop/quest fallback atlases.

- Cell 0: Midgaard / town.
- Cell 1: Camp mark.
- Cell 2: Old bridge.
- Cell 3: Cave mouth.
- Cell 4: Fallen ruin.
- Cell 5: Runed obelisk.
- Cell 6: Down stairs.
- Cell 7: Final/red-gate stairs.
- Cell 8: Shrine.
- Cell 9: Cache or treasure chest.
- Cell 10: Hostile encounter or danger sign.
- Cell 11: Water crossing marker.
- Cell 12: Smoky kobold cave entrance.
- Cell 13: Dusk Market stalls.
- Cell 14: Quarry lift or crane.
- Cell 15: Glass spire shard landmark.
- Cell 16: Fen ferry dock.
- Cell 17: Old Road signpost.
- Cell 18: Final gate ritual marker.
- Cell 19: Hidden cellar hatch.

## Midgaard Start-Zone Atlas Cells

These v0.54+ atlases are optional but preferred for the depth-1 Midgaard town scaffold. All are read as 5 by 4 grids and should avoid text, labels, watermarks, and infographic layouts.

`midgaard-town-atlas-runtime-v0.xx.png`:
- Cell 0: Market Square.
- Cell 1: Temple Square facade.
- Cell 2: Temple fountain.
- Cell 3: Kate's Diner.
- Cell 4: Tavern.
- Cell 5: Basic armorer.
- Cell 6: Weapons vendor.
- Cell 7: Weapon enchanter.
- Cell 8: East Gate.
- Cell 9: West Gate.
- Cell 10: Town guard.
- Cell 11: King's Hall.
- Cell 12: Sewer grate.
- Cell 13: City wall segment.
- Cell 14: Provision stall.
- Cell 15: Rat-pelt quest marker.
- Cell 16: Rat pelt bundle.
- Cell 17: Temple recall circle.
- Cell 18: Market notice board.
- Cell 19: Sewer rat warning marker.

`midgaard-tile-atlas-runtime-v0.xx.png`:
- Cell 0: Market cobbles.
- Cell 1: Temple square clean paving.
- Cell 2: Fountain plaza wet stone.
- Cell 3: Diner lane.
- Cell 4: Tavern lane.
- Cell 5: Armorer row.
- Cell 6: Weapons row.
- Cell 7: Enchanter rune paving.
- Cell 8: East/west gate threshold.
- Cell 9: City wall blocked tile.
- Cell 10: Guard post.
- Cell 11: Royal keep courtyard.
- Cell 12: Sewer grate floor.
- Cell 13: Sewer slime edge.
- Cell 14: Provisions ground.
- Cell 15: Rat-pelt quest ground.
- Cell 16: Rat pelt bundle ground.
- Cell 17: Recall circle ground.
- Cell 18: Notice board ground.
- Cell 19: Rat warning muddy stone.

`midgaard-npc-atlas-runtime-v1.93.0.png` is the approved exact 5 by 4 world-sprite contract:
- Cell 0: Watchman Rusk / west guard.
- Cell 1: Watchwoman Ilyra / east guard.
- Cell 2: King Halvard.
- Cell 3: Market Clerk Nessa.
- Cell 4: Mira / temple healer.
- Cell 5: Tavern Keeper Orren.
- Cell 6: Borin / armorer.
- Cell 7: Tessa / weaponsmith.
- Cell 8: Gate Captain Brann.
- Cell 9: Maud / enchanter.
- Cell 10: Kate / diner cook.
- Cell 11: Lute / provisioner.
- Cell 12: City Courier Tovan.
- Cell 13: Wounded Traveler Edda.
- Cell 14: Dock worker.
- Cell 15: Stable Hand Pell.
- Cell 16: Royal Herald Vann.
- Cell 17: Novice Healer Sera.
- Cell 18: Old Road Scout Yara.
- Cell 19: Scholar.

All 20 cells must remain valid and gutter-safe. Current placed-world mappings
use every cell. Cells 10, 11, 14, and 19 are live through explicit
`DinerCook`, `Provisioner`, `DockWorker`, and `Scholar` world-object types.
They place Kate beside the diner, Lute beside provisions, the dock worker in
the south-quarter works, and the scholar near the keep. Runtime acceptance
requires the exact 1280 by 1024 contract and valid square-cell geometry before
any of those mappings may render.

v1.81.0 replaces the four stylistically mismatched named-NPC inserts in cells
7, 9, 13, and 18. The other sixteen cells remain pixel-identical to v1.64.1.
v1.93.0 replaces only the four former reserved contact cells with Kate, Lute,
the dock worker, and the scholar. The other sixteen cells remain
pixel-identical to v1.81.0.

`midgaard-sewer-atlas-runtime-v0.xx.png`:
- Cell 0: Sewer grate entrance.
- Cell 1: Open sewer ladder.
- Cell 2: Damp sewer archway.
- Cell 3: Slime brick wall.
- Cell 4: Sewer water channel.
- Cell 5: Rat nest.
- Cell 6: Sewer rat token.
- Cell 7: Giant sewer rat token.
- Cell 8: Ratfolk cutthroat token.
- Cell 9: Ratfolk mage token.
- Cell 10: Ratfolk cleric token.
- Cell 11: Ratfolk brute token.
- Cell 12: Rat pelt bundle.
- Cell 13: Rat pelt armor vest.
- Cells 14-19: Sewer key, broken barrel, pipe, warning lantern, blocked tunnel rubble, and sewer shrine alcove.

## World Map Overlay and Token Atlas Cells

`world-map-overlay-atlas-runtime-v0.80.png`, `world-map-progression-overlay-atlas-runtime-v0.63.png`, and `world-map-token-sprite-atlas-runtime-v1.29.0.png` are approved 5 by 4 exploration UI/sprite sheets.

Overlay atlas:
- Cell 0: Adjacent move/passable marker.
- Cell 1: Adjacent blocked marker.
- Cell 2: North route arrow.
- Cell 3: East route arrow.
- Cell 4: South route arrow.
- Cell 5: West route arrow.
- Cell 6: Inspect cursor reticle.
- Cell 7: Danger pulse marker.
- Cell 8: Camp-ready marker.
- Cell 9: Descend-ready marker.
- Cell 10: Fog edge wisp.
- Cell 11: Discovered-zone sparkle.
- Cell 12: Safe-road marker.
- Cell 13: Hostile patrol marker.
- Cell 14: Shrine aura marker.
- Cell 15: Cache glint marker.
- Cell 16: Water-crossing ripple marker.
- Cell 17: Cave clue smoke marker.
- Cell 18: Quest objective ring.
- Cell 19: Reserved/transparent.

Progression overlay atlas:
- Cell 0: Explored-zone banner marker.
- Cell 1: Newly discovered zone sparkle.
- Cell 2: Quest objective ring.
- Cell 3: Chapter milestone stone.
- Cell 4: Royal decree marker.
- Cell 5: Training camp or level-up campfire.
- Cell 6: Spell lesson rune circle.
- Cell 7: Martial training crossed weapons.
- Cell 8: Stat point shrine.
- Cell 9: Skill point token.
- Cell 10: Road fork signpost.
- Cell 11: Locked road gate.
- Cell 12: Cleared encounter wreath.
- Cell 13: Dangerous patrol marker.
- Cell 14: Safe road lantern.
- Cell 15: Midgaard recall beacon.
- Cell 16: Stair descent marker.
- Cell 17: Kobold route smoke charm.
- Cell 18: Red Gate warning seal.
- Cell 19: Final meteor crown omen.

Token atlas:
- Cell 0: Multi-member party group.
- Cell 1: Shield/front-line adventurer.
- Cell 2: Bow/ranged adventurer.
- Cell 3: Knife/rogue adventurer.
- Cell 4: Mender/priest adventurer.
- Cell 5: Ember/wizard adventurer.
- Cell 6: Hex/warlock adventurer.
- Cell 7: Ward/paladin adventurer.
- Cell 8: Pike/reach adventurer.
- Cell 9: Party camp.
- Cell 10: Rat patrol.
- Cell 11: Kobold patrol.
- Cell 12: Ratfolk patrol.
- Cell 13: Drow patrol.
- Cell 14: Demon patrol.
- Cell 15: Greater demon patrol.
- Cell 16: Arcane portal.
- Cell 17: Fallen-party warning.
- Cell 18: Caravan.
- Cell 19: Cave entrance.

Local and Region Map use the dedicated v2.4 player-role atlas for exactly one represented party member in Shield, Pike, Bow, Knife, Mender, Ember, Hex, and Ward order. Multi-member parties continue to use mixed-token cell 0; mixed-token cells 1-8 remain role fallbacks only.

## Story Card Atlas Cells

`story-card-atlas-runtime-v0.xx.png` is optional Journal art. It is read as a 5 by 4 grid.

- Cell 0: Chapter I / Midgaard Road and cisterns.
- Cell 1: Chapter II / Kobold Smoke.
- Cell 2: Chapter III / Bone Road.
- Cell 3: Chapter IV / Glass Warrens.
- Cell 4: Chapter V / Red Gate.
- Cell 5: Chapter VI / Meteor Crown.
- Cell 6: Kobold route card.
- Cell 7: Dusk Market route card.
- Cell 8: Green Shrine or tree-cover lesson.
- Cell 9: Old Quarry or stone-cover lesson.
- Cell 10: Glass Warrens contact scene.
- Cell 11: Ash Fen or mire route card.
- Cell 12: Red Gate warning.
- Cell 13: Victory or route-complete card.
- Cell 14: Defeat or fallen-party card.
- Cells 15-19: Reserved for later chapters, route cards, or ending variants.

## NPC Portrait Atlas Cells

`npc-portrait-atlas-runtime-v1.60.0.png` is the approved 1400 by 1120 transparent 5 by 4 dialogue/contact sheet:

- Cell 0: King Halvard.
- Cell 1: Herald Vann.
- Cell 2: Nessa.
- Cell 3: Watchman Rusk.
- Cell 4: Watchwoman Ilyra.
- Cell 5: Tovan.
- Cell 6: Mira.
- Cell 7: Sera.
- Cell 8: Orren.
- Cell 9: Edda.
- Cell 10: Pell.
- Cell 11: Captain Brann.
- Cell 12: Kate.
- Cell 13: Borin.
- Cell 14: Tessa.
- Cell 15: Maud.
- Cell 16: Yara.
- Cell 17: Lute.
- Cell 18: Dock worker.
- Cell 19: Scholar.

## Art Quality Targets

- Readable at tactical board scale.
- Strong silhouette first, detail second.
- Modern pixel feel without losing old-school CRPG clarity.
- Transparent or flat-background sources are preferred for sprites and objects.
- Terrain should stay usable behind units, highlights, and targeting reticles.

## Handoff Notes

When an art thread creates a new atlas, include:

- Filename and saved path.
- Grid layout.
- Cell inventory.
- Any expected code mapping changes.
- Whether the atlas is a drop-in replacement using an existing prefix or a new art category needing code.

Do not delete older atlases. They remain useful fallback references.
