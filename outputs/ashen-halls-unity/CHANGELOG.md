# Ashen Halls Changelog

All changes are original to Ashen Halls. The game is a spiritual successor to old party-based tactical CRPGs, not a clone of Nahlakh.

## v0.49.1 - Graphics UI Repair Verification

- Verified the v0.49 graphics and icon-first combat UI code with a clean script compile.
- Updated the Unity Windows build helper to use v0.49.1 package naming instead of an older v0.46.0 label.
- Rebuilt the Windows package from the last known-good player with the v0.49.1 runtime DLL, docs, and art references.
- Added this repair note so GitHub and packaged builds clearly reflect the checked build.
- Save data remains version 17 because this pass changes packaging/version metadata and verification only.

## v0.49.0 - Tavern Graphics, Icon-First Combat UI, and Floater Cleanup

- Added five generated v0.49 art assets under `Docs/ArtReferences/`: tavern backdrop, tavern UI atlas, combat HUD atlas, spell/floating-text atlas, and enemy/world-object atlas.
- Made the tavern landing prefer the new generated tavern backdrop, with the procedural tavern kept as a fallback.
- Upgraded the sparse tavern loop into a warmer procedural lute/hearth loop with bass, light percussion, and occasional flute tones.
- Added generated icons to the tavern menu buttons for Start Game, Customize Company, Beta Lab, Settings, and Exit Game.
- Reworked combat command buttons to be icon-first, with larger action icons, tiny hotkey badges, and compact state labels instead of word-heavy buttons.
- Routed several enemy and world-object visuals through the new enemy/world-object atlas, including kobolds, ratfolk, drow, demons, caves, caches, shrines, final gates, tree cover, and stone cover.
- Improved floating combat text stacking so repeated events on the same tile separate into small lanes with readable backplates.
- Save data remains version 17 because this pass changes presentation, art loading, and procedural audio only.

## v0.48.0 - Victory Route and Final Gate Completion Scaffold

- Added a tavern route preview panel for the Road to the Final Gate, with chapter chips from the cisterns through the Meteor Crown.
- Expanded late-route story objectives so depth 5 and depth 6 point more clearly toward the Red Gate and final ritual.
- Changed final-boss victory flow so defeating the meteor-crowned encounter ends combat and opens a dedicated beta victory screen.
- Added a victory ledger with survivor count, average level, gold, reached depth, party member rows, and a compact chapter recap.
- Added victory-screen buttons for New Company, Tavern, and Beta Lab so the end of the beta route loops cleanly back into testing.
- Save data remains version 17 because this pass adds route flow and UI, not new saved fields.

## v0.47.2 - Combat UI Readability Pass

- Increased the default window target to 1800x1040 when space allows.
- Replaced the odd in-game top-left header token with a cleaner Ashen Halls mark.
- Renamed the combat side panels from Company to Party and Opposition to Enemy Combatants.
- Gave combat a dedicated grid sizing helper so the turn queue/readout no longer crowd or overlap the tactical board.
- Enlarged combat sprites by reducing board-cell padding.
- Made side-panel party/enemy rows taller with larger portraits, meters, and readable headers.
- Rebuilt bottom combat buttons as wider commands with larger icons, labels, and sublabels instead of tiny icon art under text.
- Enlarged unit overlay badges and status duration chips so class/range/state marks are more useful at the new resolution.
- Save data remains version 17 because this pass changes UI presentation only.

## v0.47.1 - Tavern Landing and Spellbook Overlay Cleanup

- Added a dedicated post-splash Midgaard tavern landing screen with Start Game, Customize Company, Beta Lab, Settings, and Exit Game.
- Moved the character builder behind Customize Company so the first playable screen no longer looks like the debug/customization editor.
- Reworked the muster top bar spacing to avoid overlap between title, party summary, SFX controls, and right-side action buttons.
- Changed Cast behavior so the Spellbook stays closed until Cast is pressed.
- Added a large combat Spellbook overlay that can use most of the screen, with a close button and larger spell-card grid.
- Choosing a spell now closes the Spellbook and arms targeting on the tactical board.
- Narrowed global I/C armory/spell-reference hotkeys so they do not interfere with the tavern menu or the open combat Spellbook.
- Save data remains version 17 because this pass changes UI flow only.

## v0.47.0 - Art Sheet Expansion, Epic Ember Spells, and Final Gate Scaffold

- Added six generated v0.47 runtime/reference sheets under `Docs/ArtReferences/`: boss/enemy atlas, quest/world-object atlas, character/inventory UI atlas, epic spell effects atlas, combat/spellbook UI atlas, and item/equipment atlas.
- Loaded the new sheets at runtime with fallback art paths if any generated PNG is missing.
- Made world-map objects prefer the new quest/world-object art for town, caches, shrines, encounters, stairs/final gate, camps, obelisks, ruins, bridges, and caves.
- Made enemy combat sprites and side portraits prefer the new boss/enemy sheet for many stronger enemy families and the new final boss roles.
- Made action buttons and action hover cards prefer the new combat/spellbook UI sheet for Attack, Cast, Guard, Elixir, Move, and End Turn.
- Made item/gear icons prefer the newer v0.47 equipment atlas.
- Upgraded Fireball and Meteor Shower with new impact art, larger particle bursts, floating spell labels, and layered procedural sound cues.
- Added a first-pass final-gate scaffold: reaching depth 6 starts a special boss encounter with Vhal Rakh, a ritual heart, and escorts.
- Added final boss enemy templates with fire/death pressure, high magic resistance, and readable weaknesses.
- Save data remains version 17 because this pass changes art loading, combat effects, enemy templates, and encounter flow without adding saved fields.

## v0.46.0 - Spellbook Art and Ember Spell Flair

- Added `Docs/ArtReferences/spellbook-combat-ui-atlas-runtime-v0.46.png`, a generated 5x5 atlas with open/closed spellbook art, Fireball/Meteor icons, action icons, tooltip/character/inventory/item frames, damage chips, and environmental object cells.
- Added `Docs/ArtReferences/ember-spell-effects-atlas-runtime-v0.46.png`, a generated 4x4 reference/runtime sheet for Fireball and Meteor Shower visual direction.
- Loaded both v0.46 sheets at runtime with procedural fallbacks if art is missing.
- Made combat action buttons and action hover cards prefer the new sword, cast rune, hourglass, guard, move, and elixir cells.
- Made the Cast panel read more like a Spellbook, including open-book art and new formula icons.
- Added small character/inventory/spellbook icons to the Armory tabs.
- Added Fireball as a classic arcing splash ember spell and made it the default ember spell for quick Spellbook arming.
- Added Meteor Shower as a higher-cost ember spell with multiple falling impact beams and area impact glyphs.
- Save data remains version 17 because this pass changes art, formula data, and effects without changing saved schema.

## v0.45.0 - Combat Tooltip and Action UI Upgrade

- Upgraded combat board hover tooltips into larger tactical cards with title, primary result, supporting rule text, phase/action/move chips, and explicit click instructions.
- Added target and cover mini-panels to combat tooltips, including HP/status for units and integrity/duration for tree or stone cover.
- Added hover help for the combat action buttons so Move, Attack, Cast, Guard, Elixir, and End Turn explain their current availability and tactical effect.
- Improved the active combat readout with a second-line command prompt that says what to do next for the selected action.
- Added Ready/Clear controls to the spell panel so testers can quickly arm a default spell or clear the selected formula.
- Save data remains version 17 because this pass changes UI behavior only.

## v0.44.0 - Combat UI and Spell Polish

- Added a new generated runtime combat UI atlas: `Docs/ArtReferences/combat-ui-atlas-runtime-v0.44.png`.
- Loaded the combat UI atlas for panel trim, Timeline/turn-order icons, action state badges, hover preview icons, and spell-card dressing.
- Renamed the visible event feed from Chronicle to Timeline.
- Improved the combat HUD with a clearer turn-order label, larger hover preview cards, HP/MP meters in the active readout, and more explicit phase/move/action language.
- Stabilized combat sprite boxes by separating steady frame/status space from active-unit sprite bobbing.
- Added four beta-test formulas: Hold Sign, Cold Lance, Mind Break, and Bind Fiend.
- Save data remains version 17 because this pass changes presentation, formula data, and combat rules without changing saved schema.

## v0.43.0 - Runtime Art Atlas Expansion

- Added six new original generated v0.43 art sheets under `Docs/ArtReferences/`: combat sprite source, transparent combat sprite atlas, world/environment atlas, item icon atlas, magic/UI atlas, and enemy roster atlas.
- Made the game prefer `combat-sprite-sheet-alpha-v0.43.png` for the combat board, while keeping older sprite/procedural fallbacks.
- Loaded `world-environment-atlas-runtime-v0.43.png` for exploration objects and new tile overlays: water, roads, cobblestones, cave floor, walls, palisades, sewer grates, moss paths, glass rubble, and red basalt.
- Loaded `magic-ui-atlas-runtime-v0.43.png` for spell cards, magic terrain obstacles, and command button glyphs.
- Loaded `item-icon-atlas-runtime-v0.43.png` in the Armory company rows, pack rows, and cache loot panel.
- Loaded `enemy-roster-atlas-runtime-v0.43.png` for richer enemy side-card portraits.
- Added ratfolk scrappers, cutthroats, plague mages, cistern clerics, and brutes.
- Added drow scouts, blade dancers, crossbows, mages, and priests.
- Added lesser demons as Red Gate/Beta Lab pressure enemies.
- Updated zone encounter pools and Beta Lab spawning so the new families can be tested immediately.
- Save data remains version 17 because this pass changes art loading, encounter tables, and enemy templates without changing saved schema.

## v0.42.0 - World Zones and Story Scaffold

- Renamed the home town to Midgaard across the live game UI and exploration flow.
- Added named world zones with danger ratings, summaries, and one-time story discovery entries.
- Added chapter objective text that updates as the company descends.
- Expanded world generation so caches, shrines, encounters, camps, stairs, obelisks, ruins, bridges, and cave mouths are biased by zone instead of feeling purely scattered.
- Made enemy pools zone-aware, so cisterns, markets, shrines, quarries, glass warrens, fens, and red-gate regions push different opposition.
- Updated the exploration region strip and hover text to show zone danger and story context.
- Bumped save data to version 17 because story chapter, active objective, discovered-zone list, and map-depth repair fields are now saved.

## v0.41.0 - Runtime World-Map Object Art

- Added a new generated runtime world-object atlas: `Docs/ArtReferences/world-object-atlas-runtime-v0.41.png`.
- Loaded the world-object atlas in game for exploration-map objects.
- Caches, shrines, enemy signs, stairs, camps, Midgaard, obelisks, ruins, bridges, and cave mouths now use generated pixel-art icons on the world map.
- Kept the existing procedural map-object drawings as a fallback if the atlas is missing.
- Save data remains version 16.

## v0.40.0 - Runtime Class Icons and Spell Tiers

- Added a new generated runtime class-icon atlas: `Docs/ArtReferences/class-icon-atlas-runtime-v0.40.png`.
- Loaded the class-icon atlas in game and drew class icons in the tavern roster, tavern portrait preview, company cards, turn queue, combat unit-frame badges, and Armory company tab.
- Kept text class abbreviations beside tiny combat icons so class identity remains readable if the art is small or missing.
- Added visible spell tier labels: starter, apprentice, adept, and elder.
- Surfaced spell tiers in combat spell cards, the selected spell detail panel, and the Spell Reference as a scaffold for later level-gated spell learning.
- Save data remains version 16.

## v0.39.0 - RPG Scaffold, Class Identity, and Pact Summoning

- Reduced new parties to a 4-person tavern company for clearer customization and combat testing.
- Added the first tavern-scene backdrop after the splash: warm panel art, hearth, bar/grog shapes, patrons, and a background band silhouette behind the customization UI.
- Added a tiny procedural tavern band loop; exploration and combat remain sparse and effect-driven.
- Added a broader RPG scaffold: race/class fields, level/XP, earned stat points, earned skill points, personal movement, attack speed, weapon damage ranges, item rarity, and item stat bonuses.
- Added five starting races to the scaffold: Human, Dusk Elf, Stoneborn, Fenkin, and Ashling.
- Expanded starting classes around Warrior, Ranger, Rogue, Wizard, Mage, Warlock, Priest, and Paladin.
- Made class identity more visible in combat through the active readout, turn queue, party side cards, and compact unit-frame class badges.
- Added warlock pact magic with Bind Imp, a temporary fragile summoned ally that can move, block lanes, and attack during combat.
- Prevented summoned combat allies from using party elixirs.
- Kept Tree Cover as a useful stall tactic while improving counterplay: ranged enemies can pressure the specific cover blocking their shot, and monsters still break cover that blocks pursuit.
- Shifted early depth encounters toward sewer rats and giant rats before the kobold cave pressure ramps up.
- Added a generated class-icon/tavern UI reference sheet under `Docs/ArtReferences/class-icons-tavern-reference-v0.39.png` for the next cleaned class-icon implementation pass.
- Intentionally made older saves obsolete for this beta scaffold. Save data is now version 16.

## v0.38.0 - Simplified Spell Menu

- Simplified combat magic to one clickable Cast menu: choose a spell card, then click a highlighted target.
- Removed the visible legacy key-code casting path from the live UI, hover text, help, and spell reference.
- Renamed visible spell entries to plain development names such as Tree Cover, Heal, Fire Spark, and Death Burst.
- Added party cover attacks so players can deliberately break tree and stone cover just like enemies can.
- Tightened cover counterplay: arcing spells can pass over breakable cover, direct shots cannot, and enemies now prioritize smashing adjacent cover that blocks their path or sight.
- Made stone blocks participate in direct line-of-sight cover like trees.
- Let caster-style enemy specials, including kobold shamans and bone wizards, arc over cover when their magic type calls for it.
- Added a new generated v0.38 spell-card icon sheet and made the spell menu prefer it at runtime.
- Kept save data at version 15 because this pass changes UI, rules, and art loading without changing saved schema.

## v0.37.0 - Spell Menu and Timed Tree Cover

- Reworked the Cast panel into readable paged spell cards with name, hint, mana, range, and arc/sight behavior.
- Made Tree Cover timed and breakable: it blocks arrows and direct bolts, lasts 8 turns, and can still be battered down by enemies.
- Added temporary-cover duration labels and clearer tree-cover hover previews.
- Added status duration badges on combat sprites and status timers in attack/formula hover previews.
- Kept save data at version 15 because this pass changes combat rules/UI presentation without changing saved schema.

## v0.36.0 - Targeting and Cover Readability

- Added hover target reticles with compact in-tile badges for move cost, attack hit chance, formula path, and blocked states.
- Added blocked-line cover markers so arrows and direct formulas call out which tree cover interrupts the line.
- Added durability pips to breakable tree and stone cover, making the enemy cover-breaking counterplay easier to read.
- Kept save data at version 15 because this pass changes combat feedback and rendering only.

## v0.35.0 - Combat UI Art and Spell Beams

- Added small pixel glyphs to combat action buttons so Move, Attack, Cast, Guard, Elixir, and End Turn read faster than text alone.
- Added caster frame marks for party spellcasters and enemy caster-like units.
- Split beam visuals by effect: arrows, healing, fire, ice, death/hex, and shock/arc now draw with distinct old-school pixel shapes.
- Updated enemy caster beams to use matching visual types for healing, hex, death, shock, ice, poison, and fire pressure.
- Kept save data at version 15 because this pass changes UI art and combat rendering only.

## v0.34.0 - Combat and Magic Feel Pass

- Added formula aim traces on the combat grid: direct casts draw straight pixel lines, blocked casts mark the cover, and arcing formulas draw raised path art.
- Changed actual arcing formula resolution to use matching arc beams when the formula crosses cover.
- Improved right-side combat layout on shorter windows with compact company/enemy cards that preserve the full eight-member roster.
- Added sparse combat SFX for miss, bow, blade, crit, guard, counter, resist, and status events.
- Kept save data at version 15 because this pass changes presentation, audio hooks, and combat feedback only.

## v0.33.0 - Formula Arc Readability and Combat Polish

- Added explicit arcing formula behavior for selected formulas such as `GBH`, `BTF`, circle rites, and splash enemy formulas.
- Tightened line-of-sight balance: direct enemy formulas, arrows, and non-arcing terrain placement respect tree cover, while tagged arc formulas can cross it.
- Upgraded the formula panel with three keyed rune slots and small `arc`, `sight`, `rite`, and `open` tags on formula chips.
- Added more readable combat sprite badges for role, range, caster/threat, guard, ward, and web states.
- Added subtle combat floor variation: cracked stone, moss streaks, scorch marks, bone chips, runes, and grates by depth.
- Improved right-side combat panel layout with wider defensive sizing and compact meter/card drawing to reduce overlap.
- Cleaned up player-facing priest/cleric wording while keeping the internal `mender` role key for save compatibility.
- Kept save data at version 15 because this pass changes rules, rendering, and text only.

## v0.32.0 - Tree Cover Counterplay

- Made splash enemy-targeting formulas explicitly arc over tree cover while direct bolts and arrows still require line of sight.
- Updated formula panel previews and codex rule lines to call out `arc` behavior for splash formulas.
- Added saved cover integrity for blocking combat obstacles: trees start lighter, stone blocks tougher.
- Added enemy cover breaking: enemies that cannot keep pressure because of adjacent tree/stone cover can spend their turn battering it down.
- Made brutes, elites, and fire-themed enemies better at breaking cover.
- Added visible crack marks on damaged tree and stone cover.
- Bumped save data to version 15 because combat obstacle integrity is now saved.

## v0.31.0 - Tactical Formula Aiming

- Added formula area previews while aiming: splash formulas paint the center and adjacent affected tiles before the player commits the cast.
- Fixed formula range highlighting so focused casting's +1 range is reflected on the combat grid.
- Added four formulas that deepen combat choices without changing save data: `OBL` priest light banish, `RBI` ember iceburst, `RKW` hex bind, and `RPX` hex poison burst.
- Improved formula resolution logs so landed/resisted status effects are called out directly in the Timeline.
- Improved status wording across combat UI, previews, and logs: poisoned, bleeding, stunned, sleeping, warded, webbed, and hexed.
- Updated formula codex rule lines to show splash and direct-sight requirements.
- Kept save data at version 14 because this pass changes combat rules and formula definitions only.

## v0.30.0 - Sprite Anchors and Formula UI

- Added runtime sprite-cell trimming for the generated 4x4 combat sheet: transparent padding is scanned once, cached, and removed before drawing.
- Added lower-body sprite anchoring so feet/body pixels, not the raw image rectangle, align to the tile center.
- Changed the formula panel so non-caster turns no longer draw the large casting workspace over the battlefield.
- Reworked the expanded formula panel around selected-formula details: code, craft, mana cost, range, target type, line-of-sight rule, focus state, and effect summary.
- Improved formula hover previews with healing ranges, damage ranges, drain notes, splash counts, status chance after magic resistance, cure details, mana cost, and terrain reactions.
- Centralized formula line-of-sight checks so highlighting, hover previews, and actual casting use the same rule.
- Classified kobold shamans and kobold bone wizards as caster-like enemies for target choice and movement scoring.
- Kept save data at version 14 because this pass changes rendering, previews, and combat rules only.

## v0.29.0 - Beta Formula Lab

- Changed the beta-stage presentation to `Beta Formula Lab`, matching this build's focus on combat/casting stress testing.
- Added a new original generated formula-lab art sheet under `Docs/ArtReferences/`.
- Added a new original generated combat sprite sheet source, converted it to a transparent alpha PNG, and load it at runtime for board units.
- Loaded the v0.29 formula-lab sheet as a second runtime art atlas for formula-school/effect icons in the casting panel.
- Replaced loose concept-sheet combat crops with fixed 4x4 grid slicing and bottom-center sprite anchoring, making combat sprites much less misaligned.
- Added paged formula chips so active spellcasters can reach longer craft lists without leaving combat or memorizing every code.
- Changed the formula panel to collapse into a compact hint strip during non-caster turns, while spellcaster turns get the expanded formula lab UI.
- Added an in-combat Beta Lab toolbar with `Refill`, `Reset`, `Hazards`, `Spawn`, and `SFX` controls.
- `Refill` restores party HP/MP, clears afflictions, tops up elixirs, and wards the company for testing.
- `Hazards` refreshes tree, stone, web, gas, fire, and ice tiles so terrain reactions can be tested repeatedly.
- `Spawn` adds caster-pressure enemies such as kobold shamans, kobold bone wizards, bone priests, glass mages, and cinderlings when space allows.
- Added a visible SFX pulse under the sound controls that shows which sound fired and at what one-shot volume, making audio debugging easier even if Windows app volume is muted.
- Kept save data at version 14 because all new beta-lab state is runtime-only.

## v0.28.0 - Beta Combat Test, Formula Chips, and Runtime Art Atlas

- Marked the splash as `Beta Combat Test` so testers immediately know this is a stress-test build rather than a finished release.
- Added a `Beta Lab` button on the Muster screen that jumps directly into a caster-heavy combat with full mana, extra elixirs, tactical hazards, kobold shamans, kobold bone wizards, and other spellcasting pressure.
- Added clickable formula chips for active spellcasters while keeping the three-letter keyboard formula system intact.
- Added combat phase language to the active-unit readout so testers can see whether they are choosing an action, choosing a target, resolving, or watching enemy thinking.
- Imported a new original generated beta combat/casting UI reference sheet and began using it as a runtime texture atlas for combat figures, enemy mini-portraits, and party portrait areas.
- Kept the procedural rectangle-built sprites as fallback art if the generated atlas cannot be loaded.
- Improved SFX reliability by ensuring the runtime has an audio listener, using non-spatial one-shot audio, raising procedural effect volume, adding formula/turn clips, and adding an on-screen `SFX Test` button.
- Kept save data at version 14 because no saved fields or enum values were added in this pass.

## v0.27.0 - Bolder Graphics and End Turn Combat

- Kept the core name Ashen Halls, but added the subtitle The Old Road in title/splash presentation.
- Added new original splash/title and combat UI/sprite reference sheets under `Docs/ArtReferences/`.
- Added optional runtime loading for the generated v0.27 splash image from the packaged docs folder, with a procedural fallback if image decoding is unavailable.
- Boosted exploration tile colors and interactable object glows so the world map reads less drab.
- Added an older-map landmark repair pass so loaded saves can receive obelisks, ruins, bridges, and cave mouths instead of requiring a fresh map.
- Enlarged combat unit sprites on the tactical board and boosted party sprite saturation/brightness.
- Added strong combat status frames: gold active, red injured, green poisoned/webbed, violet hexed/sleeping/stunned, teal warded/regenerating, and gray guarded.
- Renamed the Wait command presentation to an explicit End Turn button while preserving the old keyboard rhythm with `6` and Space.
- Kept save data at version 14 because no new saved fields or enum values were added in this pass.

## v0.26.0 - UI Polish, Landmarks, and Kobold Casters

- Fixed a recent UI overlap issue by calculating side-panel heights from shared rectangles instead of mismatched estimates.
- Made company cards, enemy cards, top resources, and the combat command bar more defensive on narrow or short windows.
- Added subtle modern-pixel panel treatment with top highlights and clearer resource icons.
- Added a new original UI/world/caster reference sheet under `Docs/ArtReferences/`.
- Added new stored world-map landmark objects: runed obelisks, fallen ruins, old bridges, and cave mouths.
- Added runtime pixel drawings, colors, names, hints, and hover treatment for the new landmarks.
- Added Kobold Bone Wizards to deeper encounters.
- Made Kobold Shamans more threatening with hex/web special casting.
- Gave Kobold Bone Wizards a death-ball splash special and stronger red-black caster sprite details.
- Bumped save data to version 14 because newly generated maps can store the new landmark object types.

## v0.25.0 - Map Graphics and Exploration Readability

- Raised the default window size to 1600x960 for a roomier exploration and combat layout.
- Added a new original generated exploration-map reference sheet under `Docs/ArtReferences/`.
- Increased the exploration scouting radius from 9 to 10 tiles.
- Added region-specific exploration tile styles: old roads, broken paving, mossy paths, quarry stone, fen banks, shallow mire, glass rubble, ash floors, tree walls, cliff stone, red basalt, and dark water.
- Added clearer fog-of-war motifs and darker distance shading around the visible edge.
- Upgraded interactable map markers again with stronger silhouettes, shadows, glows, and detail for caches, shrines, encounters, stairs, camps, and the town marker.
- Added extra generated loop roads and plaza rooms to new maps so exploration routes feel more connected.
- Kept save data at version 13 because this pass does not change saved state shape.

## v0.24.0 - Graphics Direction and Kobold Pass

- Added original generated reference sheets for enemy sprites and map elements under `Docs/ArtReferences/`.
- Staged LibreSprite 1.2 in the workspace tool cache for future hand-drawn pixel sprite sheets, tiles, enemy variants, and UI icons.
- Raised the default window size to 1440x900 for more comfortable combat and exploration readability.
- Added kobold enemy families: Raider, Slinger, Shaman, and Shieldbearer.
- Added a reptilian kobold combat sprite renderer with snouts, horns, tails, claws, role props, and readable weapon silhouettes.
- Updated enemy pools so kobold roles appear across early, middle, and deeper encounters.
- Redrew exploration object markers for caches, shrines, encounters, stairs, camps, and town.
- Updated the Windows package docs copy step so nested art-reference files are included.
- Kept save data at version 13 because this pass does not change saved state shape.

## v0.23.0 - Combat Input and Magic Usability

- Fixed formula typing so letter keys only feed the spell system after the player selects Cast.
- Preserved Armory and Formula Codex hotkeys outside Cast mode: `I` opens Armory and `C` opens Codex normally.
- Let `I` and `C` work as formula letters while Cast mode is armed, so formulas such as OIC, NVC, LBC, FIF, and RIG are usable from the keyboard.
- Added formula recall: in Cast mode, Tab or Enter recalls the last successful formula when no entry is pending.
- Added formula prefix suggestions while typing partial formula codes.
- Expanded NVC cleansing to remove poison, bleed, web, stun, sleep, and hex.
- Made hex more meaningful: hexed units are visibly marked and take amplified formula damage.
- Kept save data at version 13 because this pass does not change saved state shape.

## v0.22.0 - Combat and Formula Magic

- Added focused casting: spellcasters who cast before moving get -1 MP cost, +1 range, and stronger formula damage.
- Added formula terrain reactions: fire burns tree cover and ignites gas/webs, ice can quench fire into steam, and shock can arc through ice/gas/web hazards.
- Added new formulas: LBC circle mend, TBG circle ward, BTF burn cover, RSG shock arc, and RNH hex weakness.
- Added splash healing/warding support for mender circle formulas.
- Added hex weakness as a combat status: hexed targets are easier to hit and hurt, while hexed attackers are less reliable.
- Added sleep wakeups on damage, status-aware attack preview notes, and melee guard counter damage.
- Kept save data at version 13 because this pass does not change saved state shape.

## v0.21.0 - Sprite Polish Pass

- Added combat sprite ground shadows, active-unit accents, and wounded-state marks.
- Added more party sprite detail: arms, belts, boots, tabards, shoulder details, beard/face variants, cloak edges, role accessories, and circlets.
- Expanded visible equipment overlays for scale/mail/leather/robes/cloaks/helms/shield types, ranged quivers, polearm heads, hammer heads, foci, orbs, bells, and thrown weapons.
- Added more enchanted gear marks for haste, keen, weightless, focus, echoes, silence, bleeding, stunning, storm, and similar traits.
- Added enemy rank ornaments, family-specific overlay marks, weakness/resistance indicators, and clearer status-threat details.
- Kept save data at version 13 because this pass does not change saved state shape.

## v0.20.1 - Tool Downloads and Release Hygiene

- Downloaded selected helper tools into the workspace tool cache: Git for Windows, 7-Zip, and Audacity.
- Added a tool-download manifest with source links, install notes, local filenames, and SHA-256 checksums.
- Added a small PowerShell verification script beside the downloads so the staged files can be checked before install.
- Updated packaging docs so future Windows zips can include the tool-download manifest when present.
- Kept save data at version 13 because this pass does not change saved state shape.

## v0.20.0 - Armory and Formula Codex

- Added a modal Armory/Codex overlay.
- Press `I` to inspect company gear and pack loot.
- Press `C` to open directly to the formula codex.
- Added company gear summaries for weapon range, damage type, hit/power modifiers, armor, guard, weight, and magic warding.
- Added pack loot inspection with item traits, best-fit hints, and a `Try Equip` button that re-runs the existing conservative auto-equip check.
- Added formula reference rows with code, name, school, mana, range, target, and effect.
- Kept save data at version 13 because this pass does not change saved state shape.

## v0.19.0 - Exploration Readability

- Added region-name strips above the exploration map.
- Added hover look text for open floors, stone walls, camps, caches, shrines, encounters, stairs, and the town marker.
- Added adjacent movement cues so clickable map steps are easier to read.
- Added object glow outlines using distinct colors for caches, shrines, encounters, stairs, camps, and town.
- Added region-entry Timeline lines and banners while moving through the overworld.
- Kept save data at version 13 because this pass does not change saved state shape.

## v0.18.0 - Enemy Variety and Combat Pressure

- Added new enemy families: Mire Archer, Bone Priest, Cinderling, and Gloam Knight.
- Added veteran and elite enemy ranks with stronger stats, clearer names, and brighter combat markings.
- Added enemy special actions: Bone Priests can heal/ward allies, Adepts can shock, Glass Mages can spread ice, Spores can poison/gas, Cinderlings can burn tiles, and Shades can pressure minds.
- Added distinct pixel silhouettes for the new enemy families.
- Improved the opposition panel with compact tactic/threat lines so enemy roles are easier to read.
- Bumped save data to version 13 because combat units now store enemy rank data.

## v0.17.0 - Combat Comfort and SFX Controls

- Added saved SFX preferences: SFX on/off plus 25/50/75/100 percent volume steps.
- Added global keyboard audio controls: `M` toggles SFX, `+` and `-` adjust SFX volume.
- Added combat action hotkeys: `1` Move, `2` Attack, `3` Cast, `4` Guard, `5` Elixir, `6` or Space Wait.
- Added a hover aim line and stronger hovered-tile outline in combat so targeting, range, and cover intent read faster.
- Bumped save data to version 12 because audio preferences now persist; older supported saves are normalized on load.

## v0.16.3 - Tooling and Project Notes

- Added lightweight contributor and tester notes: `KNOWN_ISSUES.txt`, `Docs/DESIGN_BIBLE.md`, `Docs/RELEASE_CHECKLIST.md`, and `Docs/TOOLING.md`.
- Updated `README_PLAY.txt` so packaged builds explain the companion notes.
- Updated the Windows build helper to copy the changelog, known issues, and docs folder into release packages.
- Kept the Unity package set unchanged: no Asset Store dependencies and no new Unity packages.

## v0.16.2 - Stability Pass

- Hardened save/load failure handling so a bad save does not replace the active game state.
- Prevented hidden keyboard actions while the title splash is visible.
- Reduced UI allocation churn by caching common centered label styles.

## v0.16.1 - Launch and Input Hotfix

- Improved launch/input stability for windowed play.
- Let title splash clicks pass through once the game is ready.
- Added keyboard fallbacks for Quick Start and Begin Company.

## v0.16 - Itemization and Customization

- Expanded generated weapons, armor, forms, materials, traits, and combat effects.
- Added cache loot comparison feedback.
- Added clearer party role identity, gear/look rerolls, and company summary.
- Kept audio sparse with simple one-shot sound effects and no music.

## v0.15 and Earlier

- Built the main vertical slice: muster, exploration, tactical combat, formula magic, generated loot, larger world map, enemies, terrain, save/load, and packaged Windows builds.
