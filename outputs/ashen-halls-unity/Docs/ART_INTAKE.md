# Ashen Halls Art Intake

This project uses generated and hand-cleaned original art atlases from `Docs/ArtReferences/`.
High-visibility runtime art loads an approved exact filename first, then uses a semantic-version-sorted development fallback. Release builds fail when an approved family has a newer file that has not been reviewed and pinned.

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

- Creature, enemy sprite, character combat, spell animation, combat terrain, kobold route, kobold boss, kobold cave prop, and kobold combat terrain atlases are currently read as 4 by 4 grids.
- `ability-icon-atlas-runtime-*` is read as a 4 by 2 grid for older sheets, a 4 by 3 grid for v0.73+ sheets, or a 4 by 5 grid for the expanded v1.31+ warrior/rogue/ranger sheet.
- `signature-spell-icon-atlas-runtime-*` is read as a transparent 5 by 5 grid and provides unique art for the 25 signature formulas listed below.
- `lightning-spell-icon-atlas-runtime-*` is an exact transparent 4 by 2 grid. The v1.79 contract maps Arc Spark, Chain Lightning, Arcane Tempest, Thunder Step, and Thunderclap to cells 0, 1, 3, 4, and 5; Storm Cage, Storm Ward, and Thunderhead remain reserved in cells 2, 6, and 7.
- `magic-ui-atlas-runtime-*` is read as a transparent 4 by 4 grid for shared spell schools, utility effects, and formula fallbacks.
- `ranger-ability-effect-atlas-runtime-*` is read as a 4 by 4 grid for ranger/missile battlefield effects.
- Most UI, item, world object, world-map, story-card, and icon atlases are currently read as 5 by 4 grids. The expanded world-map material and blocked-terrain contracts below are explicit exceptions.
- `roaming-threat-atlas-runtime-*` is a transparent 5 by 4 grid. The v1.62.0 contract reserves cells 0-19 for rat, ratfolk, kobold, demon, drow, undead, elite, boss-escort, and encounter-marker silhouettes; keep each figure centered with a common foot baseline and clear gutters.
- `world-map-ui-atlas-runtime-*` is read as a transparent 5 by 4 grid and is used by exploration/map command buttons.
- `tavern-ui-atlas-runtime-*` is read as a transparent 5 by 4 grid and is used by the tavern/title menu buttons.
- Current active world/Midgaard/combat terrain sheets: `world-map-material-atlas-runtime-v1.68.0.png`, `world-map-exploration-tile-atlas-runtime-v1.68.0.png`, `world-map-overlay-atlas-runtime-v0.80.png`, `world-map-progression-overlay-atlas-runtime-v0.63.png`, `combat-terrain-atlas-runtime-v1.5.8.png`, `midgaard-tile-atlas-runtime-v1.6.3.png`, `midgaard-city-prop-atlas-runtime-v1.29.0.png`, `midgaard-gate-atlas-runtime-v1.64.0.png`, and `midgaard-wall-atlas-runtime-v1.30.0.png`.
- `world-map-material-atlas-runtime-v1.68.0.png` is the active exact 8 by 8, 2048 by 2048 passable-ground contract. Each semantic material owns four adjacent 256-pixel variants.
- `world-map-exploration-tile-atlas-runtime-v1.68.0.png` is the active exact 5 by 8, 1280 by 2048 blocked/fallback terrain contract. Cells 0-19 preserve the repaired v1.24.2 bank and cells 20-39 add blocked-terrain variants. This is a deliberate two-bank expansion, not the older irregular 5 by 7 experiment.
- `unique-item-atlas-runtime-*` is read as a 5 by 4 grid and is checked before generic equipment art.
- `combat-ui-panel-atlas-runtime-*` is read as a 5 by 4 grid and used as subtle panel chrome/backdrop art.
- Current active tavern/combat/UI/ability/effect sheets include `tavern-ui-atlas-runtime-v1.5.9.png`, `world-map-ui-atlas-runtime-v1.6.0.png`, `combat-ui-panel-atlas-runtime-v0.72.png`, `ability-icon-atlas-runtime-v1.31.0.png`, `signature-spell-icon-atlas-runtime-v1.31.0.png`, `lightning-spell-icon-atlas-runtime-v1.79.0.png`, `magic-ui-atlas-runtime-v1.31.0.png`, `spellbook-combat-ui-atlas-runtime-v1.24.0.png`, `combat-spell-effects-atlas-runtime-v0.73.png`, `spell-animation-atlas-runtime-v1.49.0.png`, `combat-command-icon-atlas-runtime-v0.73.png`, `combat-hud-ui-atlas-runtime-v0.73.png`, `combat-spell-float-atlas-runtime-v0.73.png`, and `ranger-ability-effect-atlas-runtime-v0.73.png`.
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

`character-combat-atlas-runtime-vX.XX.X.png` is the main combat-board and roster portrait sheet for player characters. It is read as a transparent 4 by 4 grid and should preserve centered silhouettes, consistent foot anchors, and at least 18 pixels of gutter.

Current runtime sheet: `character-combat-atlas-runtime-v1.77.0.png`, an exact 1024 by 1024 RGBA sheet with 256-pixel cells.

- Cell 0: Human warrior.
- Cell 1: Stoneborn warrior.
- Cell 2: Human rogue.
- Cell 3: Dusk elf rogue.
- Cell 4: Human ranger.
- Cell 5: Dusk elf ranger.
- Cell 6: Fenkin priest.
- Cell 7: Human priest.
- Cell 8: Human/default warlock.
- Cell 9: Dusk elf warlock / alternate hex caster.
- Cell 10: Human wizard.
- Cell 11: Ashling wizard.
- Cell 12: Human paladin.
- Cell 13: Stoneborn paladin.
- Cell 14: Fenkin rogue.
- Cell 15: Dusk elf warrior.

## Ability Icon Atlas Cells

`ability-icon-atlas-runtime-v0.xx.png` is the Combat Skills icon sheet. Older sheets retain their legacy layouts; the current runtime sheet is an exact transparent 4 by 5 grid. Current runtime sheet: `ability-icon-atlas-runtime-v1.31.0.png`, with editable source `source-ability-icon-atlas-v1.31.0.aseprite`.

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

## Signature Spell Icon Atlas Cells

`signature-spell-icon-atlas-runtime-v0.xx.png` is an exact transparent 5 by 5 grid used before generic school art. Current runtime sheet: `signature-spell-icon-atlas-runtime-v1.31.0.png`, with editable source `source-signature-spell-icon-atlas-v1.31.0.aseprite`.

- Cell 0: Tree Cover (GBH).
- Cell 1: Hallowed Circle (HLC).
- Cell 2: Heal (OIC).
- Cell 3: Ward (TBQ).
- Cell 4: Sun Brand (SBN).
- Cell 5: Fire Spark (FIF).
- Cell 6: Fireball (FBL).
- Cell 7: Meteor Shower (MTR).
- Cell 8: Chain Lightning (CLT).
- Cell 9: Frost Bind (FRB).
- Cell 10: Web Snare (WBK).
- Cell 11: Poison Gas (WBP).
- Cell 12: Sleep (RMS).
- Cell 13: Drain Life (INH).
- Cell 14: Death Burst (RLM).
- Cell 15: Summon Imp (IBD).
- Cell 16: Summon Lesser Demon (IBF).
- Cell 17: Summon Greater Demon (IBG).
- Cell 18: Pact Brand (PBR).
- Cell 19: Doom Circle (DMC).
- Cell 20: Cold Lance (RCL).
- Cell 21: Flame Jet (RDF).
- Cell 22: Shock Burst (RSG).
- Cell 23: Circle Heal (LBC).
- Cell 24: Regenerate (TNC).

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

`combat-command-icon-atlas-runtime-v0.xx.png` is the main combat command bar icon sheet. It is read as a 5 by 4 grid.

- Cell 0: Move / step movement.
- Cell 1: Attack.
- Cell 2: Cast / Spellbook.
- Cell 3: Guard.
- Cell 4: Elixir.
- Cell 5: End Turn / wait.
- Cell 6: Skill fallback or martial action.
- Cell 7: Targeting reticle.
- Cell 8: Blocked / invalid action.
- Cell 9: Alternate wait/end marker.
- Remaining cells are reserved for later command-state art.

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

`world-map-material-atlas-runtime-v1.68.0.png` is the authoritative passable world-map ground sheet. It is an opaque 8 by 8 grid with 256-pixel cells. Every four-cell run contains the approved v1.28 foundation followed by generated variants A, B, and C. Runtime selection is coordinate-deterministic, favors the approved foundation, and never changes save data.

- Cells 0-3: City paving.
- Cells 4-7: Market cobbles.
- Cells 8-11: Temple stone.
- Cells 12-15: Keep stone.
- Cells 16-19: Natural ground / forest loam. Forest excludes the bright open-ground cell 16.
- Cells 20-23: Moss.
- Cells 24-27: Ruined paving / ruined wall material.
- Cells 28-31: Packed dirt / bridge-deck fallback.
- Cells 32-35: Fen mud.
- Cells 36-39: Shallow / deep water material.
- Cells 40-43: Quarry stone / cliff material.
- Cells 44-47: Glass rubble.
- Cells 48-51: Red ash / red basalt material.
- Cells 52-55: Gloam stone.
- Cells 56-59: Cistern brick.
- Cells 60-63: Sewer brick.

Keep these cells quiet and edge-to-edge. Large trees, buildings, gates, shrines, stairs, bridges, and other focal objects belong in the transparent prop/landmark sheets or the blocked/fallback terrain sheet.

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

`midgaard-npc-atlas-runtime-v1.81.0.png` is the approved exact 5 by 4 world-sprite contract:
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
- Cells 10-11: Reserved city contacts.
- Cell 12: City Courier Tovan.
- Cell 13: Wounded Traveler Edda.
- Cell 14: Reserved city contact.
- Cell 15: Stable Hand Pell.
- Cell 16: Royal Herald Vann.
- Cell 17: Novice Healer Sera.
- Cell 18: Old Road Scout Yara.
- Cell 19: Reserved city contact.

All 20 cells must remain valid and gutter-safe. Active code mappings use cells 0-9, 12-13, and 15-18.

v1.81.0 replaces the four stylistically mismatched named-NPC inserts in cells
7, 9, 13, and 18. The other sixteen cells remain pixel-identical to v1.64.1.

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

Local and Region Map use cell 0 for a multi-member group and cells 1-8 for a one-member party according to its role.

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
