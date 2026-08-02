# Ashen Halls Runtime Art Audit

Last updated for the v2.4.0 source candidate. Combined verification remains pending; older release sections are retained below as historical records.

## Active Runtime Art

These atlas families are loaded by `AshenHallsGame.LoadExternalArt()` and have draw paths in the current immediate-mode UI:

- Title/tavern: splash reference fallback, title card, game icon, tavern backdrop, tavern UI.
- Party/enemies: character combat atlas, enemy sprite atlas, creature/combat sprite fallback atlas, enemy roster, boss enemy fallback, Kobold King boss atlas.
- World map: world environment/object fallback, world-map ground tiles, regional set-pieces, threat habitats, landmarks, overlays, progression overlays, mixed and solo-role token sprites, props, non-interactive citizens, quest world objects, and Midgaard town/tile/NPC/sewer sheets.
- Kobold route: route markers, cave props, dedicated kobold combat terrain, boss route art.
- Combat UI/effects: combat UI, combat UI panel, combat HUD, combat command icons, ability icons, spellbook UI, combat spellbook UI, spellbook open art, magic UI, ember/epic/combat spell effects, spell animation, combat floating text art.
- Inventory/items: item inventory atlas, older item equipment/icon fallbacks, inventory consumable atlas, character inventory UI.
- Journal/scaffold: story cards, NPC portraits, route/dungeon/service/faction scaffold hooks when matching files are present.

## Newly Connected In v2.4.0 - Verification Pending

- Source wiring exact-pins `world-threat-habitat-atlas-runtime-v2.4.0.png`, `world-npc-citizen-atlas-runtime-v2.4.0.png`, and `player-exploration-role-atlas-runtime-v2.4.0.png` as separate 4 by 2, 1536 by 768 transparent contracts with eight populated 384-pixel cells and 20-pixel safe gutters.
- Threat habitats remain stationary at the existing saved home beneath mobile threat tokens and are suppressed on certified safe roads. They add no encounters and mutate no campaign state.
- Ambient citizens are deterministic exterior dressing only. They stay off the Grand Hearth tutorial lane, roads, guidance, entrances, and interactables and never replace a named NPC or dialogue portrait.
- Exactly one represented party member uses the dedicated Shield, Pike, Bow, Knife, Mender, Ember, Hex, or Ward atlas cell. Multi-member parties retain the mixed atlas group marker; combat and inventory character art remain unchanged.
- RuleSmoke, focused sprite-art and combat-UI runtime smoke, full RuntimeBoot, Windows build/package, clean-extracted packaged boot, and final endpoint visual inspection remain pending for the settled v2.4.0 source.

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
