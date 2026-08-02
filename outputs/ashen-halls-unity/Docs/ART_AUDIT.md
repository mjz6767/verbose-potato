# Ashen Halls Runtime Art Audit

The integrated v2.9.0 candidate exact-pins expanded ability and signature-spell books plus a rebuilt combat-effects atlas, while improving combatant staging and the presentation of existing world-map art. Combined RuleSmoke, focused combat-UI runtime smoke, SpriteArtRuntimeSmoke, and full RuntimeBoot pass. The final Windows build, built-player visual matrix, canonical packaging, and clean-extracted packaged boot remain pending release gates. Physical-controller review and a complete human playthrough remain manual. Older release sections are retained as historical records.

## Active Runtime Art

These atlas families are loaded by `AshenHallsGame.LoadExternalArt()` and have draw paths in the current immediate-mode UI:

- Title/tavern: splash reference fallback, title card, game icon, tavern backdrop, tavern UI.
- Party/enemies: character combat atlas, enemy sprite atlas, creature/combat sprite fallback atlas, enemy roster, boss enemy fallback, Kobold King boss atlas.
- World map: world environment/object fallback, world-map ground tiles, regional set-pieces, threat habitats, landmarks, overlays, progression overlays, mixed and solo-role token sprites, props, procedural non-interactive exterior citizens, six authored non-interactive Town Hall patrons, dedicated Grand Hearth floor/set-piece/ambience companions, quest world objects, and Midgaard town/tile/NPC/sewer sheets.

## Newly Connected In v2.9.0 - Automated Gates Passed; Release Gates Pending

- `ability-icon-atlas-runtime-v2.9.0.png` expands the preserved skill sheet to an exact RGBA 4 by 7 contract. Cells 24-26 are Sunder, Shadowstep, and Quick Shot; cell 27 remains transparent.
- `signature-spell-icon-atlas-runtime-v2.9.0.png` keeps the exact RGBA 7 by 8 contract and maps Dawn Pulse, Cinderstorm, Grave Hook, Soul Veil, and Ashen Curse to former reserve cells 51-55.
- `combat-spell-effects-atlas-runtime-v2.9.0.png` is the exact-pinned RGBA 1280 by 1280, 4 by 4 effect sheet. Each 320-pixel cell limits visible content to 280 by 280 and retains at least 20 transparent pixels on each side at alpha greater than 8. Its stable semantic order supports layered impact motifs and one static Reduced Motion stamp.
- Existing player and enemy sheets now receive normalized board footprints, contact light, faction rims, and a restrained active-unit pulse. Existing world-map sheets gain safer edge framing, more grounded building placement, quieter ambient layers, and citizen clearance without changing their atlas cells or introducing replacement map art.
- The accepted v2.9 power-art source set contains thirteen exact files: three runtime PNGs and ten chroma-key, alpha, prompt/design, and validation companions. All thirteen require exact-path promotion from the locally excluded ArtReferences workspace; only the three runtime PNGs are package candidates.
- Combined RuleSmoke, focused combat-UI runtime smoke, SpriteArtRuntimeSmoke, and full RuntimeBoot pass with the live pins and mappings. The final Windows build, source/staged/packaged hash comparison, built-player visual inspection, canonical package inventory, and clean-extracted packaged boot remain pending.

## Newly Connected In v2.8.0 - Automated, Build, and Visual Gates Passed

- `grand-hearth-ambience-atlas-runtime-v2.8.0.png` is the exact-pinned RGBA 1536 by 1024, 3 by 2 atlas for warm hearth light, storm-door rain spill, wall sconce, ember haze, rain-window reflection, and patron contact shadow.
- All six 512-pixel cells retain at least 36 transparent pixels on every edge at alpha > 8 and contain zero visible bright-magenta residue.
- Deterministic draw rules place the layer between the established floor, patrons, and fixtures, with reduced Region Map opacity. It introduces no map object, collision, interaction, saved identity, or motion state.
- The v2.8 handoff records the built-in ImageGen prompt, flat-key removal, normalization, hashes, semantic cell order, and runtime constraints.
- RuleSmoke, focused SpriteArtRuntimeSmoke, full RuntimeBoot, and the Windows build pass. Six Local/Region captures under `QA/v2.8.0-grand-hearth` cover 960 by 600, 1280 by 720, and 1920 by 1080; all report `complete=True`, `failure=None`, and the deterministic packet passes. Visual review accepts restrained warm/cool depth, horizontal east-door guidance, readable patrons, continuous runner, and unobstructed fixtures. Canonical package inventory and clean-extracted boot pass; the release-integrity record reports clean tracked source, save v25, player exit 0, 83 packaged art files, and the exact ambience-atlas hash.
- Kobold route: route markers, cave props, dedicated kobold combat terrain, boss route art.
- Combat UI/effects: combat UI, combat UI panel, combat HUD, combat command icons, ability icons, spellbook UI, combat spellbook UI, spellbook open art, magic UI, ember/epic/combat spell effects, spell animation, combat floating text art.
- Inventory/items: item inventory atlas, older item equipment/icon fallbacks, inventory consumable atlas, character inventory UI.
- Journal/scaffold: story cards, NPC portraits, route/dungeon/service/faction scaffold hooks when matching files are present.

## Newly Connected In v2.7.0 - Automated, Build, and Visual Gates Passed

- `grand-hearth-floor-atlas-runtime-v2.7.0.png` is the exact-pinned 1536 by 1024 TrueColor 3 by 2 atlas with 512-pixel cells: dark civic wood A/B, oxblood-and-charcoal company runner, company runner medallion, heat-darkened hearth apron, and storm threshold.
- `grand-hearth-setpiece-atlas-runtime-v2.7.0.png` is the exact-pinned RGBA 1536 by 1024 3 by 2 atlas with a 32-pixel transparent cell gutter: monumental Grand Hearth, interior storm doors, company register, road-company banner, rain-blue window, and stores.
- Exact loaders, validation guards, deterministic floor selection, fixture mappings, and live draw adapters are connected. The two sheets are additive companions; the shared v1.61 Midgaard interior tile and prop atlases remain the fail-closed fallback.
- Floor cells 0/1 use the accepted edge-balance ImageGen refinement with updated prompt/validation provenance. Fallback set-piece drawing is footprint-gated and covered by deterministic assertions.
- The accepted source set contains the two runtime PNGs plus seven source/alpha/prompt/validation companions. These nine exact files are recorded in `GRAND_HEARTH_ART_V2.7_HANDOFF.md` and promoted from the locally ignored ArtReferences folder; only the two runtime PNGs are package candidates.
- RuleSmoke, focused SpriteArtRuntimeSmoke, full RuntimeBoot, and the final Windows build pass. Six built-player captures under `QA/v2.7.0-grand-hearth` cover Local and Region at 960 by 600, 1280 by 720, and 1920 by 1080; all report `complete=True`, `failure=None`. Visual review accepts one open hall, continuous oxblood runner/medallion, monumental hearth, open storm doors, six patrons, and clear route without checkerboard seams, magenta residue, square backplates, or cropping. Stable Town Hall geometry, IDs, portal/objective flow, gameplay, and save schema v25 remain protected.
- Canonical package inventory and clean-extracted packaged boot pass from a clean tracked source. The authoritative release-integrity manifest records save v25, zero dirty/status entries, successful batch exploration boot, player exit code 0, 82 packaged art files, and both exact Grand Hearth atlas hashes. Physical-controller review and full human playthrough remain manual.

## Newly Connected In v2.6.0 - Automated, Visual, and Package Gates Passed

- Town Hall's expanded Grand Hearth reuses six distinct approved cells from `world-npc-citizen-atlas-runtime-v2.4.0.png`: tailor, mason, lamplighter, caravan guide, road pilgrim, and fishmonger. No new runtime image, manifest pin, packaged-art entry, atlas geometry, or v2.4 provenance file is introduced.
- These are authored interior presentation placements, not procedural exterior ambient citizens. They remain deterministic, walk-through, and off the tutorial company runner, with no `MapObject`, collision, hover response, Talk prompt, dialogue, named-NPC substitution, portrait identity, or saved identity. Procedural placement remains exterior-only and still excludes every room.
- RuleSmoke verifies the exact six-cell mapping, distinct professions, clear runner, and stable Town Hall contracts. Focused sprite-art runtime smoke resolves every patron through the approved atlas, full RuntimeBoot verifies the live first-spawn room and portal flow, and the Windows build passes. Four direct built-player captures under `QA/v2.6.0-town-hall` cover Local and Region at 1280 by 720 and 1920 by 1080; all report `complete=True`, `failure=None` and visually confirm the six figures, clear route, and unobstructed exit. Canonical packaging and clean-extracted packaged boot pass; the release-integrity record reports `sourceDirty=false`, `cleanExtractLaunch=true`, `playerExitCode=0`, and `packagedArtCount=80` while binding the exact final source revision and hashes.

## Newly Connected In v2.4.0 - Automated Gates Passed

- Source wiring exact-pins `world-threat-habitat-atlas-runtime-v2.4.0.png`, `world-npc-citizen-atlas-runtime-v2.4.0.png`, and `player-exploration-role-atlas-runtime-v2.4.0.png` as separate 4 by 2, 1536 by 768 transparent contracts with eight populated 384-pixel cells and 20-pixel safe gutters.
- Active-threat habitats remain stationary at the existing saved home beneath mobile threat tokens and are suppressed on certified safe roads; inactive threats leave the neutral ruined-waystation aftermath cell. They add no encounters and mutate no campaign state.
- Ambient citizens are deterministic exterior dressing only. Suitable streets and non-safe roads remain eligible, while the Grand Hearth tutorial lane, current guidance, certified safe roads, rooms, water, hazards, entrances, authored sites, and interactables stay clear. Citizens never replace a named NPC or dialogue portrait.
- Exactly one represented party member uses the dedicated Shield, Pike, Bow, Knife, Mender, Ember, Hex, or Ward atlas cell. Multi-member parties retain the mixed atlas group marker; combat and inventory character art remain unchanged.
- RuleSmoke, focused sprite-art and combat-UI runtime smoke, full RuntimeBoot, Windows build/package, and clean-extracted packaged boot pass for the settled v2.4.0 source. Final endpoint visual inspection remains a manual release-readiness check.

## Newly Connected In v1.6.3

- `midgaard-tile-atlas-runtime-v1.6.3.png` is now the latest Midgaard ground-tile sheet and is loaded automatically by the prefix-based runtime loader.
- The sheet was rebuilt from `source-midgaard-tile-atlas-v1.6.3-generated.png` into exact 5x4 cells with cropped gutters and reduced contrast so city floors sit behind player/NPC/object sprites.
- Exploration rendering now draws Midgaard and general floor art at lower opacity, uses fewer/dimmer ambient props, and uses the group-party token as the default world-map party marker.
- `VersionInfo.cs` now owns the runtime/build package version, reducing the chance that art/docs/build packages disagree about the active release.

## Newly Connected In v1.6.2

- `ability-icon-atlas-runtime-v1.6.2.png` replaces the active Combat Skills popup icon sheet.
- The sheet follows the existing 4x3 contract: warrior skills on row 1, rogue skills on row 2, ranger skills on row 3.
- All 12 cells passed validation after chroma-key removal, with visible coverage between 18.8% and 40.9% and centered bounding boxes.

## Newly Connected In v1.6.0

- Audited active runtime atlases for visible-pixel coverage. Current world-map token, prop, landmark, Midgaard town/prop/NPC, tavern UI, combat command, combat panel, and terrain families all pass their current loader thresholds.
- `world-map-ui-atlas-runtime-v1.6.0.png` replaces the active exploration command icon sheet.
- Cell 5 is now a Location Details parchment/pin icon instead of travel-rail art; cell 8 remains the plain location pin for markers.
- The v1.6.0 sheet passed validation: all 20 cells were within the expected 0.04-0.86 visible-coverage range after chroma-key removal.

## Newly Connected In v1.5.9

- `tavern-ui-atlas-runtime-v1.5.9.png` now supplies the active title/tavern menu button icons.
- The generated tavern sheet passed local validation: 63.6% transparent, 36.2% visible overall, and all 20 cells fell within the expected coverage range.
- Combat-board party sprites no longer draw a tiny body sigil; status pips are now smaller and placed as an edge rail instead of over the upper sprite body.
- Title-screen tester doors are now hidden behind the `Beta Testing` panel by default, reducing first-screen debug clutter.
- Exploration now uses a single map-first presentation with a compact Location Details drawer, larger 11x7 Close Map tiles, and reduced world-object padding so tokens read closer to the larger detail-view sprites.

## Newly Connected In v1.5.8

- `spell-animation-atlas-runtime-v1.5.8.png` now supplies transparent tile-glyph art for fireball, meteor, death/hex, shock, cold, heal/ward, and pact/summon spell visuals.
- `combat-terrain-atlas-runtime-v1.5.8.png` keeps the v0.80 terrain contract but replaces the fire hazard cell with a darker burning-stone tile. It is intentionally opaque because combat terrain is floor art.
- Active art metrics from the local audit:
  - `world-map-token-sprite-atlas-runtime-v1.5.3.png`: 68.8% transparent, 31.2% visible.
  - `world-map-prop-atlas-runtime-v1.5.3.png`: 53.9% transparent, 46.1% visible.
  - `world-map-landmark-atlas-runtime-v1.5.3.png`: 60.9% transparent, 39.1% visible.
  - `midgaard-npc-atlas-runtime-v1.5.3.png`: 68.0% transparent, 32.0% visible.
  - `spell-animation-atlas-runtime-v1.5.8.png`: 74.9% transparent, 24.4% visible.
- The v1.5.3 world-map replacement sheets remain usable and are not showing the severe over-pruning pattern that broke the earlier v0.93-style sheets.

## Newly Connected In v1.5.7

- `world-map-ui-atlas-runtime-v1.5.7.png` now has a loader, alpha/visible validation, and a 5x4 cell contract.
- Exploration command buttons now use the generated map UI atlas for camp, recall, descend, elixir, wide/close map, travel focus, journal, and armory actions.
- Procedural `DrawTinyUiIcon` art remains as fallback if a future UI atlas is missing, mostly opaque, over-pruned, or has a bad cell.

## Newly Connected In v0.68.0

- `combat-ui-panel-atlas-runtime-v0.52.png` now has a loader and helper. It is used lightly as generated RPG panel texture on `DrawRpgPanel` surfaces, so the art appears without overwhelming the tactical board.
- `item-inventory-atlas-runtime-v0.51.png` is now preferred over the older v0.47 equipment atlas for Armory item icons.
- A v0.51-specific item cell map was added so common generated item forms point to the intended cells instead of inheriting the older loose mapping.

## Newly Mapped In v0.83.0

- `midgaard-tile-atlas-runtime-v0.80.png` is now mapped by district instead of by the older placeholder order. Market, temple, fountain, diner, tavern, vendor, gate, guard, king, sewer, provisions, rat-pelt, recall, and generic paved tiles now point to cells that match the optimized sheet.
- `world-map-exploration-tile-atlas-runtime-v0.80.png` now supplies deterministic variants for roads, paving, moss, mire, quarry, glass, ash, cliff walls, forest walls, mire walls, and red walls.
- `combat-terrain-atlas-runtime-v0.80.png` now has corrected floor/hazard mappings for fire, ice, web, curse, glyphs, stone, Midgaard/road/cistern/quarry/glass/fen/market/court/red-gate battles, and depth fallbacks.
- `world-map-overlay-atlas-runtime-v0.80.png`, `midgaard-city-prop-atlas-runtime-v0.79.png`, `midgaard-gate-atlas-runtime-v0.79.png`, and `midgaard-wall-atlas-runtime-v0.78.png` remain active runtime families through the latest-prefix loader.

## Reference Or Source Only

Files beginning with `source-` and files ending in `reference-...png` are intentionally not runtime art unless a loader explicitly names that prefix. They are useful for art direction, cleanup, and future atlas generation, but they should not be expected to appear in game.

## Remaining Opportunities

- Add dedicated runtime sheets for route scaffolds, dungeon scaffolds, faction banners, and services. The code already has loader/draw hooks; most of those families simply do not have runtime PNGs yet.
- Hand-clean generated inventory cells for boots, helms, rings, gems, and trait-specific gear.
- Split important combat sprites into pose/animation sheets once the static silhouettes settle.
- Consider a future UI pass that uses the combat UI panel atlas more structurally, after the immediate-mode layout is replaced or stabilized.
