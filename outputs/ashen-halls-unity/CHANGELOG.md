# Ash & Brimstone Changelog

All changes are original to Ash & Brimstone, formerly developed under the Ashen Halls name. The game is a spiritual successor to old party-based tactical CRPGs, not a clone of Nahlakh.

## v1.99.0 - Commanding Presence

- Rebuilt the bottom combat-command art as an exact transparent 5 by 4, 1280 by 1024 atlas with 256-pixel cells and safe gutters. Move, Attack, Cast, Guard, Elixir, End Turn, and Skills now use bold unframed emblems designed to remain distinct at actual HUD size; the remaining cells preserve a documented semantic expansion set.
- Enlarged and rebalanced the command deck across supported window sizes. Buttons, icon wells, labels, prompts, and keycaps scale together, while every command remains keyboard/controller focusable even when it is unavailable so the existing prompt can explain why it cannot fire.
- Separated command meaning from interaction state. Each command keeps its own semantic color; only a genuinely armed action receives the gold commitment rail, while keyboard/controller focus uses an independent ivory outline. The button transition no longer multiplies the semantic fill a second time, and Attack cannot appear armed when it has no legal target.
- Made Cast and Skills hand off to the actual armed formula or ability art. Their stable category emblems remain visible while browsing or unavailable, eliminating the misleading default Whirlwind icon and keeping the command deck truthful after a power is canceled.
- Strengthened Spellbook and Skillbook detail art with a larger responsive hero emblem and a restrained category-colored halo. Existing Preview, Selection, Targeting, availability, input ownership, costs, ranges, effects, and targeting rules remain unchanged.
- Added exact command-atlas dimension, mapping, coverage, gutter, loading, responsive-layout, focusability, blocked-state, focus-cleanup, and armed-art checks. The current source, player, and editor assemblies compile directly from Unity's generated response files. Six advisory development-player captures at 1280x720 and 1920x1080 exit cleanly and visually confirm blocked and legal Attack, armed Fireball, Spellbook targeting, Skillbook selection, keycap clearance, and responsive command spacing. Unity RuleSmoke, RuntimeBoot, Windows build, and packaging have not passed for this candidate because the local Unity Editor session currently reports no valid license before those suites start. Save schema remains v25.

## v1.98.0 - One Intent, One Highlight

- Rebuilt power-book input ownership around one truthful cursor. Pointer hover is now a quiet, transient Preview; committed Selection keeps the only strong left rail and fill; armed Targeting keeps its distinct gold state. Moving from mouse to keyboard or controller clears stale pointer context instead of leaving two competing highlights.
- Removed the reentrant EventSystem focus loop that appeared in every v1.97 packaged power-book capture. Selecting or focusing a row no longer tries to select that same control again while Unity is already dispatching selection.
- Made preview detail deliberately passive. Hover still explains another spell or skill without changing browse memory, pending power, scroll, MP, movement, or action state, but it no longer advertises a detail-pane button that disappears when the pointer leaves the row. Click or focus the card to commit it; only the committed power can arm or fire.
- Applied the same focus truth to the combat command deck and Armory. A parked mouse can no longer override the prompt for the keyboard/controller-focused command, and reopening Inventory restores visible focus to the committed item without a white multiplicative selection flash.
- Added deterministic staging for Selected, Preview, Targeting, Locked, Low Resource, No Target, Action Used, Disabled, and Blocked book states. Each packaged capture logs its requested state, filter, committed card, detail card, typed state, availability, context icon, preview, and targeting tuple.
- Corrected recovered production Journal copy that still called the playable Old Road a teaser. Borin's claimed reward now points through Sluice Steps toward Dusk Market, and runtime coverage rejects stale teaser language anywhere in a production Journal row.
- Added a release-integrity gate to Windows packaging. The previous good zip and evidence remain recoverable until a newly compressed candidate has been expanded into a clean temporary folder, booted through the packaged exploration path with the exact release/save marker, and bound to a manifest containing the zip, executable, committed release-source state, and all 74 PNGs named by the packaged-art manifest. Final packaging rejects dirty or ignored release inputs, untracked packaged art, stale staged art, package/art-manifest drift, and release-mode smoke/build bypasses. Development overrides use separate `-dev` artifacts; package docs now come only from tracked Unity source.
- Hardened release validation: the focused combat-UI runtime smoke is now a mandatory Windows-build gate, and the visual-QA packet tool derives the current source version or requires an explicit packaged version instead of silently claiming an old release. Rule, focused combat-UI, full runtime, Windows build/package, and twelve direct packaged-player captures pass without the former re-entrant selection warning; the deterministic packet passes with zero failures or warnings. Combat rules, costs, effects, progression, and save data are unchanged; save schema remains v25.

## v1.97.0 - Every State Has a Sign

- Rebuilt the complete 20-cell Combat Skills atlas from separate warrior, rogue, and ranger source sheets. The new silhouettes use stronger class identity, broader value separation, and safer 22-pixel gutters while preserving every gameplay ID and row-major cell.
- Refreshed 22 weaker live Spellbook symbols without disturbing the 49-formula contract. Rift Seal, Sanctuary Ward, Circle Heal, Circle Ward, Light Bolt, Still Water, Burn Cover, Chain Lightning, Web Snare, Doom Circle, Sleep, Weaken, Night Veil, Drain Life, Death Burst, Wither, Dream Smoke, and the complete Pact summoning/transformation suite now read more clearly at card size; approved v1.96 cells remain intact where they were already stronger.
- Added a compact 12-cell power-book state language for Selection, Targeting, Locked, Low Resource, No Target, Action Used, Disabled, Blocked, Cost, Reach, Target, and Preview. Hover remains a noncommitting preview, while the distinct selection and targeting marks reinforce the interaction state without replacing its text label.
- Rebuilt the lightning subset with the refreshed Chain Lightning symbol and new reserved Storm Cage, Storm Ward, and Thunderhead cells while preserving the five live lightning mappings and exact 4 by 2 contract.
- Pinned the exact v1.97 ability, signature-spell, lightning, and 4 by 3 state atlases. Deterministic rules cover dimensions, all populated cells, safe gutters, semantic state indices, manifest identity, and the optional state-texture view contract. The art and presentation pass changes no costs, ranges, effects, targeting, combat resolution, or save data; save schema remains v25.
- Rule smoke, focused combat-UI smoke, full runtime smoke, the Windows build/package, and thirteen direct packaged-player captures pass in Unity 6000.3.18f1. Ember, Mend, Hex, Pact, Warrior, Rogue, and Ranger books were inspected at 1280x720 and 1920x1080 across hover-preview, armed-targeting, long-list, and locked states; every capture reports `complete=True`, `failure=None`, the deterministic packet passes, and all four source, staged, and zipped atlas hashes match.

## v1.96.0 - Every Power Has a Face

- Rebuilt the complete 20-cell Combat Skills sheet around bold action emblems that remain legible at book-card size. Warrior, rogue, and ranger powers now share one forged dark-fantasy pixel-art language while retaining their stable gameplay IDs and atlas cells.
- Expanded dedicated Spellbook art from 25 signature formulas to all 49 prototype formulas in one exact 7 by 7 contract. Fire Floor, Burn Cover, Ice Slick, Light Bolt, Hold Sign, Weaken, Night Veil, Bind, Mind Break, Dream Smoke, Sanctuary Ward, Circle Ward, Abyssal Ascendance, and every other former generic fallback now have their own unmistakable icon.
- Rebuilt the active lightning cells from the same Ember source art, including a heavier Thunder Step boot/chevron and stronger Arc Spark, Chain Lightning, Thunderclap, and Arcane Tempest silhouettes. The three reserved lightning cells remain compatible with the established 4 by 2 contract.
- Preserved every generated chroma-key source, recorded the generation contracts, and applied local deterministic alpha cleanup, despill, centering, 256-pixel cell normalization, and safe gutters. Runtime guards and tests now cover all 49 formula mappings, exact atlas dimensions, populated cells, coverage, and boundary safety.
- Rule smoke, focused combat-UI smoke, full runtime smoke, the Windows build/package, and twelve direct packaged-player book captures pass in Unity 6000.3.18f1. Ready, long-list, Locked, hover-preview, and armed-targeting states are covered for both books at 1280x720 and 1920x1080, with Warrior, Rogue, and Ranger skills all inspected at real card size; every capture reports `complete=True`, `failure=None`. The art pass changes no spell, skill, save, targeting, or combat-resolution rules; save schema remains v25.

## v1.95.0 - Choose With Confidence

- Added destination danger to movement previews without changing combat resolution. Reachable tiles now say whether they are safe, immediately attackable, or reachable after enemy movement, while the responsible enemies receive clear `HIT` or `MOVE` markers.
- Kept unit-target decisions on the persistent target card instead of covering the battlefield with a second large hover card. Attack, spell, and skill target cards now carry their resolved hit, damage, and effect lead; empty tiles and terrain retain a smaller tooltip that chooses the least-obstructed map corner.
- Made attack guidance situational. The command prompt names the available target count and tells a no-target combatant to move, use a power, guard, or end the turn instead of promising a click that cannot work.
- Separated field lifetime from action feedback. Persistent fire, ice, web, smoke, gas, sanctuary, and curse fields use a dedicated lower-left `nR` countdown, with urgent styling at `1R`, leaving the upper-right corner free for move cost and targeting badges.
- Fixed combat UI input ownership. When a command-deck control has keyboard or controller focus, navigation and Submit stay with that control, so Enter/Space cannot activate a command and also end the turn and WASD/arrows cannot also move the combatant. Explicit number and letter hotkeys remain available.
- Tightened Spellbook and Skillbook scanning at 1280x720. Six complete rows now fit without clipping, scroll labels count only fully visible cards and show truthful direction arrows, and locked rows expose `Unlocks L#` beside cost and range.
- Extended deterministic coverage for focused-input routing, projected danger parity, field countdown copy, target-preview routing, supported book row geometry, truthful top/bottom scroll labels, and production unlock metadata. Rule, focused combat-UI, full runtime smoke, the Windows build, and nine direct-player captures pass in Unity 6000.3.18f1. Save schema remains v25.

## v1.94.0 - Clear Intent

- Reworked combat's information hierarchy around the current decision. The top strip now prioritizes round, remaining movement, and action readiness instead of exploration currency, while the active card adds a concise threat readout and uses an honest `ACTION ENEMY` state during hostile turns.
- Added deterministic movement-route previews that use the same weighted grid costs as movement execution. Legal destinations show the complete orthogonal route and cost; blocked or unaffordable destinations remain visibly distinct.
- Made targeting readable without depending on color alone. Legal targets use corner brackets, blocked and out-of-range targets receive a foreground crossed mark, unit team/status frames return above the sprite layer, and attack hover instructions now respect range and line of sight.
- Kept persistent-field round badges visible on the board while leaving verbose terrain explanations in hover detail. Clean capture staging now supports movement, legal/blocked attack, and armed area-spell states without the Beta Lab toolbar.
- Simplified Spellbook and Skillbook language and density. Locked content says `LOCKED`, global action/lock state no longer repeats across every row, untargeted powers describe their real footprint, and the split gives the selected power more reading room.
- Standardized `Back to Battle [Esc]`, changed armed actions to `Resume Targeting`, removed duplicated formula rules and targeting status from the narrative pane, and added right-stick/Page Up/Page Down detail scrolling for long tactical text.
- Expanded deterministic rule and runtime coverage for path cost parity, combat-relevant header data, enemy-turn wording, legal/blocked target sweeps, quieter badge policy, typed target wording, modal geometry, and synthetic long-detail scrolling. Rule, focused combat-UI, runtime, packaged build, and fourteen deterministic packaged-player captures pass. Save schema remains v25.

## v1.93.0 - Faces at the Gate

- Replaced the east and west Midgaard gate cells with true wall-aligned side gates. Two low bastions now connect to the north-south wall above and below a fully transparent east-west road passage instead of laying a mirrored front-elevation facade across the route.
- Kept the authoritative directional contract explicit: runtime maps West to cell 6 and East to cell 7, the cells are exact visible-RGB/alpha mirrors, and direction-specific mass signatures prevent an accidental cell swap. The compact Local and Region footprints keep the gate close to the wall without changing east/west traversal or sealed north/south behavior.
- Reworked the renderer-owned perimeter join. Horizontal parapets retain their 0.56 Local / 0.52 Region foundation, while north-south runs use a narrower 0.36 / 0.34 foundation that no longer exposes broad gray rails. Open side-gate cells suppress the hidden straight-wall tile, reuse neighboring terrain materials through the threshold, and stop their joins above and below the road instead of painting a sill across it.
- Expanded player combat art to a 5 by 7 semantic race/class grid and filled the four former reserved Midgaard NPC cells with distinct art for Kate, Lute, the dock worker, and the scholar. All four contacts now have explicit reachable Midgaard placements, Talk interactions, live world-sprite adapters, dialogue focus, and ambient routing instead of catalog-only identities. Existing approved character and NPC cells remain preserved in their deterministic mappings.
- Pinned the exact v1.93 gate, player-character, and Midgaard NPC atlases. Added source prompts, deterministic atlas validation, side-gate clear-passage and directional checks, updated map-scale geometry coverage, and retained save schema v25.
- Passed the current rule, runtime, and Windows-build suites plus a ten-capture deterministic world-map packet. It covers Local and Region Map at 1280x720 and 1920x1080, all four gates at 1280x720, and east/west inspection views at 3200x1800.
- Made Spellbook and Skillbook hover a transient preview instead of a hidden selection change. Leaving a row restores the committed detail without moving the list or changing browse memory, targeting, MP, movement, or action state; held controller input no longer jumps the initial choice.
- Separated the one browsed selection rail from the armed power's labeled `TARGETING` badge and right rail, added clear `PREVIEW`, `SELECTED`, and `SELECTED • TARGETING` detail context, and made an empty retained view fall back to a useful Known/Ready view after combat state changes.
- Exposed Ready, Known, Progression, and All as four visible counted controls, added adventurer level, power-profile metadata, and nonduplicated tactical notes, and made a hover-preview action commit that preview before any later activation can arm it.
- Unified mouse hover and keyboard/controller focus explanations across the combat command deck, surfaced movement and action economy on the active-unit card, and removed ghost spell/skill target washes when no real power is armed. Full deterministic rules and the focused live combat/book runtime smoke pass; save schema remains v25.
- Hardened the four new Midgaard contacts end to end. Kate and Lute now retain `DinerCook` and `Provisioner` focus through topics, responses, and purchase returns; the dock worker and scholar retain their own one-page conversations. Dedicated contact close-up/dialogue smoke modes validate exact Talk targets, world cells, speakers, portrait cells, and topic shapes. The scholar now follows the keep's royal score and explicitly connects the cistern marks to spellcraft.
- Made the v1.93 Midgaard NPC sheet a semantic exact pin instead of accepting an older same-size sheet with different reserved-cell meanings. If that exact atlas is unavailable, every named city actor now falls back to a role-colored procedural figure; Kate, Lute, the dock worker, and the scholar retain apron/spoon, satchel/lantern, cap/rope, and spectacles/book silhouettes instead of clamping to an unrelated generic icon.
- Completed the combat-book controller/readability pass. Held vertical input repeats after a deliberate delay, selected rows keep semantic focus, reselecting a view preserves cursor/scroll, and view round-trips restore their own browse state. Formula notes lead before dense casting rules inside a dynamically sized scrollable narrative region, device-neutral footer copy names controller controls, and an armed power with zero legal targets can no longer advertise or reopen dead targeting.

## v1.92.0 - The Stones Settle

- Rebuilt material-atlas cells 0-15 as four internally coherent civic families for city paving, market cobbles, temple stone, and keep stone. The production pass normalizes family luminance and gives every variant a shared wrapped edge profile. Packed-dirt cells 28-31 now use four matched orientations instead of a 20-plus-point luminance quilt at the east and west gate approaches; the remaining 44 exterior and dungeon material cells stay pixel-identical to v1.68.
- Added deterministic three-band material feathering between different passable material banks. The feather stays beneath paths, decals, props, actors, and landmarks, and deliberately skips gates, thresholds, walls, and blocked terrain so traversal boundaries remain legible.
- Made static object footprints choose their material bank's calm variant, removed the old black same-passability family seam, and reduced Local and Region route-stroke width so authored paving remains visible beneath navigation topology.
- Corrected feather-strip source traversal so the shared-edge texel stays on the actual boundary, and replaced repeated object-list scans with coordinate lookups while resolving one neighbor sample per three-band feather.
- Pinned the exact v1.92 8 by 8 material atlas and added deterministic coverage for material-feather eligibility, protected thresholds, band width, opacity falloff, crop/flip traversal, atlas filename, dimensions, and runtime loading. Local and Region ground identity remains coordinate-stable, topology and save data are unchanged, and save schema remains v25.

## v1.91.0 - The Walls Hold

- Rebuilt Midgaard's perimeter as a continuous orientation-aware stone band with distinct straight runs, corners, and restrained tower accents. The new composition removes square wall blocks and keeps the masonry joined across the town boundary.
- Replaced the gate atlas with normalized front and side silhouettes. The east and west road gates retain directional portrait art, bounded footprints, and clean wall joins; their final mapping places each large outer tower on the wilderness side and its gate wing toward town. The north and south gates remain sealed and wall-scale. Traversal topology is unchanged: east and west stay passable, while north and south stay closed.
- Replaced the wide horizontal party lineup with a compact four-person marker and recalibrated its Local and Region Map footprint so it remains readable without engulfing a tile.
- Pinned the three exact v1.91 art atlases and expanded deterministic coverage for atlas bounds, gutters, wall connection masks, gate composition, map-scale marker geometry, and gate traversal. Rule and runtime smoke suites pass in Unity 6000.3.18f1, the packaged Windows build completes successfully, and six accepted live captures certify the full Region Map perimeter, Local Map marker, and all four gate close-ups. Save schema remains v25.

## v1.90.0 - The Road Answers

- Made one authoritative guidance plan drive both the persistent `NEXT` rail and the world map. Automatic objectives now paint a restrained bounded gold thread, bracket the recommended next cell with its exact key, mark a visible arrival, and place a path-aware continuation chip at the first viewport edge.
- Corrected movement copy so every route names the physical input that actually moves there: `W / Up` north, `S / Down` south, `A / Left` west, and `D / Right` east. Guidance is shorter without dropping its target, direction, or walking distance.
- Preserved the stronger explicit Journal-waypoint trail and its precedence over story guidance. Nearby summaries now omit the active interaction and guided destination, while Help consistently calls the two board scales Local Map and Region Map.
- Hardened guidance caches against live tile, object, inventory-proof, story, and waypoint changes. Deterministic tests cover all four directions and viewport edges, bounded trail continuity, automatic and marked targets, isolated-interior exits, and same-map topology changes. Save schema remains v25.

## v1.89.0 - Golden Thread

- Replaced passive exploration bearings with one persistent, action-oriented `NEXT` line that remains visible in both compact and Details views. It now names the exact `WASD / arrows` movement input or `E / Space` interaction, the physical target, a path-aware first direction, and walking distance.
- Made objective resolution interior-aware. King's Hall now retargets King Halvard after entry, reward-ready Armory guidance retargets Borin, and completed interior work guides the party back through the reachable exit instead of pointing at an isolated exterior marker.
- Kept explicit Journal waypoints authoritative while making their travel instructions use the same bounded guidance contract. Reaching a mark now gives the exact `J` and Clear action needed to resume the story route. Missing and blocked targets fail safely, and the Chapter II descent is named consistently as Sluice Steps.
- Added deterministic formatting and full runtime story-transition coverage, plus an opt-in local AI visual-QA packet builder. The AI layer performs no network call, exposes only sanitized capture metadata, remains advisory, and cannot override deterministic or human review. Save schema remains v25.
- Isolated every staged visual-smoke and batch boot from campaign writes or legacy-save import, quoted the migrated workspace path correctly during automated builds, synchronized Unity bundle metadata with the release, packaged the QA companion beside its docs, and removed absolute workstation paths from the distributable package note.

## v1.88.0 - Sure Footing

- Repaired movement restoration when a web expires or burns away at turn start. The repaired full allowance now replaces the stale zero-point Undo Move snapshot, so undoing the first post-web step returns the unit to its start tile with its full movement still available.
- Added one authoritative `CombatController` gate for player commands. Resolving round and impact holds, dead or inactive actors, and non-party actors are rejected before callbacks run or resources and combat state can change.
- Preserved End Turn as a deliberate exception for any living active party unit in `ChooseAction` or `ChooseTarget`, including stunned, sleeping, and action-spent units.
- Added deterministic coverage for web expiry and fire removal through move, undo, and move-again flow, plus rejected commands during round and impact holds. Save schema remains v25.

## v1.87.0 - Roundfall

- Added a brief round-resolution gate before initiative resumes. Expiring fields and opened rituals now share one bounded `ROUND N` callout instead of being overwritten by the next unit banner.
- Reserved the next living combatant while the round beat resolves, blocks player input and enemy AI during the hold, then begins that exact turn once. Reduced Motion keeps the same sequencing with a compressed delay.
- Corrected automatic start-turn status order. Damage now wakes sleeping units before skip is decided, fresh ice stuns consume the current turn, web removes movement immediately, and fire restores movement when it burns the web away.
- Preserved lethal poison, bleed, fire, sanctuary, and curse defeats through their contact-and-fall pose before outcome or initiative advances.
- Hardened visual smoke acceptance: captures must match requested and rendered dimensions, decode successfully, and contain real non-black image variation. Save schema remains v25.

## v1.86.0 - The Spell Lands

- Synchronized combatant performance with staged impacts. Targets remain visible through lethal spell travel, then recoil and fall on contact; bound allies materialize at the actual summon beat instead of appearing before the ritual lands. Authoritative combat state still resolves immediately.
- Repaired signature spell rendering so tier-three magic draws its broad semantic aura behind the crisp primary impact frame. Sleep, mind-hex, veil, web, and binding formulas now retain distinct target art even when their audio cue uses a generic fallback.
- Rebuilt the combat-audio crescendo around impact: invocation begins at the caster's battlefield position, release travels through the stereo midpoint, impact lands at the target, and music reaches its deepest duck on contact before a brief hold and recovery. Eight reusable SFX voices protect layered spell tails.
- Moved power callouts into a compact top-chrome strip with intensity, invocation/release/impact/aftermath phases, and a progressing accent rail. Focused spells name their empowered cast without covering the top battlefield row.
- Added deterministic rules for layered impact plans, semantic spell routing, combatant hit/defeat/reveal poses, impact-centered duck envelopes, stereo travel, responsive banner geometry, and the pinned v0.73 epic-effects atlas. Save schema remains v25.

## v1.85.0 - What You Carry

- Reorganized Inventory around a direct select, compare, equip flow. `I` now opens Inventory instead of requiring a second tab choice.
- Hid category controls for common-size packs and reduced larger-pack filters to All, Weapons, and Armor. Upgrade-first ordering remains automatic.
- Replaced 94-pixel metadata-heavy rows, badges, and small Inspect buttons with 72-pixel full-row selection, larger text, and one clear status or best-fit recommendation.
- Changed the detail pane to show the recommended equip target first. The full party appears only after **Choose another adventurer**, while equipped items show one owner state instead of four disabled actions.
- Added explicit armor and equippable classification. Quest materials are labeled as quest items, sort below gear, stay out of armor/upgrade filters, and are rejected by equipment actions.
- Added rule and runtime coverage for responsive compact rows, progressive target disclosure, direct-row selection, adaptive filters, and non-equippable quest materials. Save schema remains v25.

## v1.84.0 - The Road Remembers

- Turned discovered outer-road junctions into selectable waypoints. The Journal now exposes **Mark** and **Clear** actions for charted turns, highlights the active row, and persists one depth-scoped selection across save/load.
- Made explicit waypoints take precedence over automatic destination guidance. The location rail reports a path-aware first step and walking distance, while Local and Region Map draw a restrained gold breadcrumb for the next stretch of the route and a stronger bracket around the marked junction.
- Corrected the production Journal after Borin's reward. It now tracks the live Sluice Steps, Dusk Market ambush, Smoke Cave, Varkh's Hall, and Bone Road sequence instead of calling playable Chapter II content a future teaser; unrelated prototype scaffolds remain hidden.
- Restored the v1.71 exploration-readability contract: the top chrome again spells out Gold, Supplies, and Elixirs, all four party rows show numeric HP and MP over their bars, and both compact and detailed objective cards have more room for current-route copy.
- Added deterministic waypoint identity, validation, save-roundtrip, Mark/Clear, path-guidance, HUD-label, numeric-vitals, and production-Journal coverage. Save schema advances to v25.

## v1.83.0 - Runes for Every Road

- Rebuilt Runesmith Maud's Midgaard service around an explicit duration, weapon, and affinity choice. Any party weapon can receive fire, ice, storm, or radiant damage instead of the old one-time shock mark on the lead adventurer.
- Added an 18-gold temporary temper that lasts through three victories and a 90-gold permanent binding. The service remains available for every weapon, and a temporary mark can sit over a permanent binding without destroying it.
- Made enchantments properties of exact inventory objects. Starter weapons gain a linked inventory record when first enchanted; equipping the weapon moves its runes with it, while replacing a slot does not transfer the enchantment.
- Changed weapon and inventory text to name fiery, icy, stormcharged, radiant, flamebound, frostbound, stormbound, and sunbound states. Item traits report permanence or remaining victories, and the active affinity reaches combat damage, color, reactions, and resistance rules.
- Added migration for the earlier generic `enchanted` shock weapon, plus deterministic rule, runtime-dialogue, ownership, combat-copy, expiry, layering, and save-roundtrip coverage. Save schema advances to v24.

## v1.82.0 - Fire Behind the Glass

- Recast the landing title as a two-tier forged wordmark using the bundled Libre Baskerville semibold face: spaced `A S H` crowns the larger `& BRIMSTONE`, with independent emboss, ember halo, warm face, center-out forge line, spark lift, and a calmer small-cap subtitle. Reduced Motion retains the finished plaque and disables scale, pulse, and drifting sparks.
- Reworked the title-screen hierarchy around `The Brimstone Hearth`: a warmer translucent frame, serif heading, Old Road eyebrow, finer divider, more atmospheric invitation copy, contextual Continue/New Company wording, and ember-edged primary actions give the opening a deliberate game identity while preserving every existing route and setting.
- Replaced the half-screen foreground rain sheet with four clipped exterior-glass regions aligned to the painted tavern windows. Animated rain and distant lightning now remain beyond the panes instead of falling across patrons, tables, controls, and the room floor.
- Re-composed the title cue as `The Brimstone Overture`, expanding the former 24-second uniform tavern loop into a 36.9-second twelve-bar arrangement with a bowed opening, recurring forged-road lute motif, frame-drum entrance, bowed counter-line, ember bells, restrained weather crest, and returning coda.
- Kept the exact music cue name and procedural fallback contract. The revised 32 kHz stereo PCM16 master is deterministic local synthesis with no external samples, measures -20.0 dBFS RMS / -3.99 dBFS peak, has a -88.41 dBFS sample seam, and can be rebuilt alone with `--music-cue tavern_storm_hearth_ensemble_loop`. Save schema remains v23.

## v1.81.0 - Spoils Worth Choosing

- Rebuilt the loot popup around the state the game actually owns. Rewards are marked **Acquired**, zero-value resource chips disappear, the primary action is **Continue**, and the live inventory atlas now supplies the item art instead of a three-letter placeholder.
- Added a persistent outcome card for each find, separating **Equipped by...** from **Added to inventory** and surfacing a best-fit comparison when the item remains unassigned. **Review equipment** opens Inventory directly on the exact recovered item.
- Reworked Party and Pack into responsive **Equipment** and **Inventory** workspaces. Equipment has a selected-adventurer loadout pane; Inventory has All, Weapons, Armor, and Upgrades filters, upgrade-first sorting, equipped-owner badges, exact damage/speed/range or armor/agility deltas, and a persistent item detail pane.
- Replaced the ambiguous **Try Equip** action with explicit per-adventurer targets. The player can choose any available party member, and the selected inventory object records its equipped owner. Replacing a slot releases the previous inventory item; an item already in use cannot silently duplicate itself onto another character.
- Migrated older saves by linking inventory objects back to matching equipped names when ownership metadata is absent. Save schema advances to v23 while supported v17-v22 campaigns continue to load.
- Expanded layout, rule, runtime, and packaged-player QA. Automated runtime coverage now equips a selected item to a selected adventurer and follows Loot -> Review equipment -> Inventory; deterministic inventory and loot capture modes support direct visual inspection at every supported resolution.

## v1.80.0 - The Road Wears Its New Face

- Pinned the current v1.68 world terrain, v1.77 character/enemy sprites, v1.64.1 Midgaard NPCs, v1.60 portraits, world overlays/UI, and Midgaard terrain in one approved runtime manifest. Builds now fail if any approved art family has a newer unreviewed file, and runtime selection compares candidates across every art root before choosing.
- Reweighted passable world terrain so the three current v1.68 variants own 75% of visible cells instead of letting the preserved foundation dominate. Routes, materials, deterministic placement, and save schema are unchanged.
- Corrected the shield world-map token from the group-party cell to its authored shield cell, enabled role tokens for a one-member party in both Local and Region Map, and retained the group token for normal multi-member parties.
- Expanded structural and semantic validation across all character, enemy, NPC, portrait, token, terrain, overlay, and UI atlases. The complete 4x4 character/enemy mappings and every active Midgaard portrait/world-sprite identity are now locked by tests.
- Unified startup presentation so editor and packaged player both use the current tavern backdrop rather than an unpackaged v0.27 recovery image. Updated the art intake contract to match the live atlas layouts. Save schema remains v22.

## v1.79.0 - The Storm at a Glance

- Built a full lightning progression for ember mages: Arc Spark conducts through hazards, Thunderclap pushes adjacent enemies into collision stuns, Chain Lightning chooses up to four deterministic jumps, Thunder Step shocks the landing zone, and Arcane Tempest strikes a radius-2 formation with center-weighted damage and possible stuns.
- Made three underused martial skills materially tactical. Shield Bash now pushes or causes a damaging wall/unit collision, Smoke Bomb creates true sight-blocking but movement-free smoke rather than poison gas, and Scout Mark breaks Guard while stripping one turn of Ward.
- Reworked Spellbook and Skillbook around immediate decisions. Cards now show one concise effect line plus cost and reach; the selected power shows its live effect, Cost, Reach, Target, exceptional targeting context, and one action. Ready and Known are always available, while Progression appears only when future powers exist.
- Removed ordinary ready badges, projected-resource repetition, tier chips, long rules/tactics blocks, and dormant passive tutorials from the active book view. Armed, blocked, locked, reposition, and unusual footprint states remain explicit.
- Added an original transparent eight-cell lightning icon atlas, dedicated branching lightning delivery, shock impact profiles, numbered chain and radius targeting previews, and a distinct gray-teal smoke field surface. Reduced Motion continues to collapse queued travel and secondary effects.
- Expanded editor and runtime coverage for the new roster, damage falloff, targeting footprints, atlas routing, lightning delivery, and the separate smoke sight/movement contract. Save schema remains v22.

## v1.78.0 - Every Road Has a Voice

- Mastered all 44 original score contexts, adding 27 reproducible stereo loops for every Midgaard district, outer territory, landmark, pursuit state, hostile faction, arcane duel, elite battle, last stand, and demon-lord finale. The two-source crossfade director and procedural safety arrangements remain intact.
- Added 41 original mono effects for spell casts and elder powers, open/close/confirm/tab interface states, equipping and taking items, elixirs, rest, level gains, glass/mud/ash/gravel footsteps, and six new wilderness ambience families. The authored SFX bank now contains 136 files: 81 original and 55 curated CC0-derived masters.
- Wired quiet, semantic feedback into the armory, menus, books, muster, exploration views, loot, camping, elixir use, turn handoff, and party level gains. Party-turn and level-up cues are deliberately coalesced so the six-voice mix keeps combat impacts readable.
- Expanded surface and landmark routing so Glass Warrens, Ash Fen, Green Shrine Road, Gloam Courts, camps, caves, ruins, groves, and arcane sites no longer share a generic wind bed. New footsteps follow their actual world material.
- Rebuilt deterministic manifests, waveform metrics, hashes, loop-seam validation, and the listening-review medley; expanded editor/runtime coverage to all 136 imported effects and 44 imported music masters. Save schema remains v22.

## v1.77.0 - Figures From the Fire

- Replaced the two oldest broadly visible combat sprite families with original 4x4 character and enemy atlases at 256 pixels per cell, covering sixteen party archetypes and sixteen kobold, ratfolk, drow, imp, and demon silhouettes.
- Normalized lower-center foot anchors and preserved at least an 18-pixel transparent gutter around every populated cell, including cross-cell weapons and wings. Runtime-scale and checkerboard QA confirm clean silhouettes without neighboring-cell bleed.
- Kept the established latest-version loader contract and all older art as fallbacks; no gameplay or save-schema change was required.

## v1.76.0 - Power Given Shape

- Rebuilt shared spell and skill feedback as a readable three-beat sequence: a target-local anticipation mark contracts toward the strike, delivery art arrives on the impact beat, and a brief motif-specific aftermath clears before control returns.
- Added distinct visual grammar for fire, frost, shock, holy, nature, void, rift, ascendance, slash, charge, guard, volley, shadow, and smoke. Martial powers retain directional brackets and force lines without inheriting ritual circles, orbiting runes, or magical pedestal art.
- Aligned Meteor Shower, Arcane Tempest, healing/death arcs, Charge, ranged shots, and Volley delivery timing with their authored impact cues. Signature magic keeps layered atlas art, while ordinary powers and martial smoke effects remain deliberately cleaner.
- Added a short impact-aligned battlefield flash, target strike frame, and bounded low-alpha aftermath particles. Toggling Reduced Motion during an effect now clears queued travel, particles, beams, cast geometry, shake, and pulses while leaving one compact target-local confirmation.
- Split epic audio identity so spell casts receive release shimmer while martial skills retain physical low-frequency impact without sounding magical. Reduced auxiliary voice, shimmer, release, resonance, and low-impact layers preserve headroom for the primary authored hit.
- Strengthened combat HUD state language with explicit turn ownership, legal or blocked target titles, a latest-event row in the collapsed Timeline, and text fitting for longer targeting instructions. Save schema remains v22.
- Restored authored transparent tree and rock sprites plus full-strength fire, ice, web, gas, sanctuary, curse, glyph, and rift terrain art. Gas now uses real fume art instead of the generic grass cell, and the Beta Lab ice fixture occupies an open tile so every environment family is visible.
- Removed always-on cover pips, hazard duration cards, unit status frames, duplicate active-unit frames, full-tile threat boxes, enemy-intent boxes, and decorative empty-tile motifs. Inspection details now appear on hover, while selection and danger use one compact corner or rail cue.
- Collapsed each spell or skill beat to a single visual owner: authored impact art replaces duplicate procedural cores and rings, persistent-field placement emits one flourish, and signature impact art is capped at 1.5 tiles so epic effects no longer erase nearby sprites.

## v1.75.0 - Books Made for Battle

- Rebuilt the Spellbook and Skillbook as a responsive two-column combat workspace with a readable header, live resource/action context, four counted filters, a real scrolling card list, and one clear primary action in the detail pane.
- Added deterministic keyboard and controller navigation: W/S or vertical input moves the current card, Home/End and Page Up/Page Down traverse long books, 1-4 select filters, Tab or controller shoulders cycle them, Enter/Space/A activates, Esc/B returns to the board, and the mouse wheel scrolls long detail text. Selection scrolls fully into view and focus returns to the command deck.
- Replaced repeated rules-first copy with current effect, labeled Cost/Range/Target/Impact facts, projected mana or action use, legal-target counts, tactical notes, progression tier, and exact unavailable reasons for level, mana, incapacitation, spent actions, resolution, and target conditions. Books remain open for review when their action cannot currently be used.
- Made card availability use the same target legality as board highlights and resolution. Powers with no valid target explain why instead of opening a dead targeting state, HUD suggestions choose legal targets, and self-only formulas resolve directly from the book.
- Separated the card being browsed from the power already armed for targeting. Each combatant remembers a Spellbook and Skillbook selection; reopening, canceling, filtering, or moving no longer creates duplicate selected rows or orphans a pending power. If moving removes a Focus discount and the spell is no longer affordable, it is safely disarmed without spending mana or the action.
- Hardened transient-state cleanup so loading or changing scenes cannot retain stale pending spell/skill IDs. Expanded editor and runtime coverage across filters, layout, activation states, selection visibility, both real modal paths, cancel/reopen behavior, armed movement, and stale-state cleanup. Save schema remains v22.

## v1.74.0 - Clear Ground, Clear Intent

- Reduced occupied combat tiles to one visual identity layer plus one compact information layer. Fire, ice, gas, web, sanctuary, and curse now use the zone floor beneath their distinct translucent surface, without a second center icon, prop sprite, random motif, or full tactical card.
- Removed the baked dark cards behind tree and stone cover by preferring the transparent foreground symbols. Cover keeps its gameplay-critical integrity pips and lifetime badge; ritual glyphs and demon rifts retain their stronger framed warning treatment.
- Quieted field opacity and flame density so living combatants, health rails, statuses, target highlights, and hover previews remain readable on hazardous tiles.
- Made the command deck name `Spells` or `Skills` directly, describe the next click on selected commands, and reserve the gold End Turn promotion for incapacitated units or turns with both movement and action spent.
- Compacted the side Timeline, added numeric HP/MP meter labels, applied ally/enemy accent colors to inspected cards, and named passive inspection, nearest-enemy, spell-target, skill-target, and enemy-intent contexts honestly.
- Preserved Spellbook and Combat Skills scroll position across card refreshes, added a visible scroll cue, widened the Back/Esc and action controls, and replaced ambiguous Ready/Use/Close copy with Choose Target, Use Now, Return to Target, and Back to Board. Save schema remains v22.

## v1.73.0 - Songs Given Form

- Added 17 original stereo music masters for the complete high-traffic playable slice: tavern, Party Setup, Midgaard streets, throne room, merchant hall, Old Road, salt cisterns, Dusk Market, cave thresholds, generic and sewer combat, ratfolk, kobolds, bosses, Victory, and Defeat.
- Composed the new score as sparse dark-folk chamber music built from weathered plucks, bowed drones, reeds, bronze resonators, frame and hide drums, wood clicks, rain, hearth, and subterranean texture. Exact-name routing preserves every established musical context and its old procedural fallback.
- Added 40 original 48 kHz mono sound effects for healing, wards, light, curses, death magic, poison, field states, spell release, route discoveries, shrines, encounter and outcome stingers, dialogue, royal and shop interactions, shallow-water steps, material contacts, services, and all twelve existing sparse ambience contexts.
- Added a dedicated `Resources/Audio/Music` override bank and music import contract. Stereo source masters remain lossless WAV files in the project and become compact Vorbis clips in the player; missing, invalid, or unloadable tracks retain procedural music automatically.
- Added a deterministic NumPy-based build pipeline with no external samples, plus exact file manifests, hashes, waveform metrics, seam validation, and a listening-review medley. The existing 55 CC0-derived v1.70 effects remain unchanged.
- Expanded editor and runtime coverage for 95 authored SFX, 17 music masters, exact cue and track contracts, import settings, fallback retention, and adaptive music routing. Save schema remains v22.

## v1.72.0 - The Battlefield Speaks

- Certified the combat-clarity and authored-audio overhaul in the newest family playtest package alongside the v1.71 exploration interface. The explicit battlefield layer stack, responsive unit cards, lighter result chips, corrected miss timing, 55 mastered CC0 cues, and procedural safety bank all remain active.
- Made explicit packaged-player capture runs continue rendering without desktop focus, allowing the same 1280x720 and 1920x1080 combat frames used by layout tests to be inspected directly instead of producing paused or black QA captures.
- Re-ran editor rules, runtime boot and waveform checks, packaged-player captures, and the Windows build against the merged release. Save schema remains v22.

## v1.71.0 - A Map You Can Read

- Rebuilt exploration chrome around an explicit readable type scale: titles begin at 18 pixels, body copy at 12, commands at 13, and the complete HUD grows by up to 25% on larger supported displays instead of remaining fixed at tiny pixel sizes.
- Widened the compact and detailed location rails, strengthened the location/danger/view hierarchy, and promoted the current objective into its own visible card during ordinary map play.
- Turned the nearby-action card into a second usable control, added clear E and Q keycaps, exposed numeric HP and mana beside larger party bars, and retained the minimap and recent travel notes whenever vertical space permits.
- Enlarged the bottom command deck, its icons, labels, and hotkeys; gave the primary contextual action stronger emphasis; and replaced abbreviated resource headings with Gold, Supplies, and Elixirs.
- Enlarged and clarified the on-map location strip while synchronizing board reservation and pointer guards with the new rail and command geometry, preventing clicks on expanded chrome from moving the party underneath.
- Expanded editor coverage for supported resolutions, bounded UI scaling, minimum readable typography, rail widths, and control heights. Save schema remains v22.

## v1.70.0 - Where Every Blow Lands

- Reordered the battlefield presentation into explicit ground, actor, effects, and tactical-readout passes. Spell and skill art now resolves beneath combatants while selection corners, target rails, health, and status information remain readable above the action.
- Replaced opaque near-full-cell combat sprite cards and duplicate health decoration with quieter actor pedestals, edge accents, and one authoritative health bar so terrain, occupied-cell targeting, and silhouettes remain visible.
- Compacted floating combat results into lighter chips that search for clear space around living unit cores instead of stacking large black labels across nearby sprites.
- Rebalanced the combat HUD for 1280x720 through ultrawide play: the side rail is narrower, the battlefield uses more of the available frame, and active/target cards reflow status, health, and optional mana rows without overlap.
- Added a curated 55-cue authored-audio layer from explicitly documented CC0 spell, weapon, bow, impact, interface, footstep, door, page, and coin sources. Local Python/NumPy/FFmpeg mastering produces short 48 kHz mono PCM cues with restrained peaks; exact file-level provenance ships in `Docs`.
- Rebuilt the procedural fallback bank at 32 kHz with deterministic filtered noise, layered transients, resonant bodies, longer tails, per-play micro-variation, and soft limiting. Missing or invalid authored cues retain these fallbacks, while cast/release synthesis preserves distinct spell-school identity around the imported impacts.
- Corrected combat-audio outcomes: missed targeted skills keep their release and miss cues but no longer trigger impact, hurt, aftershock, or music-duck layers; staged statuses no longer announce before their parent cast or duplicate its impact voice.
- Expanded editor and runtime checks for compact HUD geometry, semantic skill registrations, waveform health, headroom, and deterministic audio diversity. Save schema remains v22.

## v1.69.0 - Rooms Beyond The Road

- Optimized exploration's shared map and room infrastructure with indexed object lookup, cached zone descriptors, reusable placement data, and one authoritative Midgaard interior repair pass instead of repeated reconstruction.
- Protected King's Hall and the merchant hall with canonical room reservations. Regional road carving and certification now reject those footprints, fixing the quarry-turn and glassward-bend routes that could cross authored interiors.
- Expanded newly generated world maps from 50x32 to 58x46 while preserving every existing save's serialized dimensions and biome layout.
- Added eight deterministic, room-sized regional sites—one in each outer territory—with authored clearing geometry, stable identities, regional surfaces, landmark centers, and certified connections to the named road circuit.
- Promoted a bounded Chapter II route into normal play after the sewer reward: Sluice Steps opens the descent to Dusk Market, the kobold ambush, Smoke Cave, and the Kobold King rooms.
- Kept the broader generic route, encounter, service, faction, and dungeon prototypes gated behind the full prototype content set. Save schema remains v22.

## v1.68.0 - A Thousand Different Footfalls

- Added 68 original world-map terrain tiles: 48 quiet passable-ground variants across all sixteen semantic materials and 20 blocked forest, mire, cliff, red-basalt, and ruined-wall variants.
- Expanded the pinned material atlas to an exact opaque 8x8 sheet with four adjacent variants per material. Coordinate-derived selection keeps maps stable across saves while favoring each material's approved foundation.
- Expanded the exploration terrain atlas to an exact opaque 5x8 sheet. Its first twenty cells preserve the repaired v1.24 contract; the appended bank adds five forest walls, five mire walls, five cliffs, four red walls, and one overgrown ruin wall.
- Normalized every generated cell to 256 pixels, removed generation-grid borders, matched opposing material edges, retained editable Aseprite sources, and saved contact-sheet, tiling, prompt, and machine-readable validation artifacts.
- Added manifest, semantic selection, atlas geometry, full-cell coverage, blocked-variant reachability, and live runtime-loading checks for the new contracts. Save schema remains v22.

## v1.67.0 - Voices Of Midgaard

- Reauthored the complete Midgaard conversation catalog so all 18 named speakers use distinct vocabulary, temperament, and local knowledge instead of sharing one tutorial voice.
- Made advice concrete and state-aware: guards discuss their own posts, merchants explain their craft, townspeople point to real landmarks, and royal dialogue follows the active cistern contract.
- Removed prototype-only Lamp Round and Gate Survey prompts from the production sewer scenario while preserving both errands in the full prototype content set.
- Bundled Libre Baskerville Regular and Semibold under the SIL Open Font License for a period-appropriate dialogue voice with deterministic rendering on every supported machine.
- Enlarged the conversation measure, raised body copy to 18 points with calmer line spacing, strengthened speaker hierarchy, improved title contrast, muted unavailable services, and made topic hints follow mouse or keyboard selection.
- Expanded editor and runtime smoke coverage for the bundled font, responsive dialogue geometry, accessible accent contrast, production quest-board copy, and prototype-topic suppression. Save schema remains v22.

## v1.66.0 - When Fire Lands

- Rebuilt Fireball as a synchronized signature spell: an ember-spark charge condenses into an orb, a rotating atlas projectile follows a lifted arc, and impact art, damage feedback, audio, shake, and board flash now meet on the same arrival frame.
- Promoted Fireball to epic presentation scale with a restrained low impact layer, a longer resolution beat, expanding fire rays, a layered explosion, cross-tile heat flash, smoke lift, falling embers, and a fading scorch echo. Meteor Shower and the Kobold King's Crooked Fireball retain greater physical scale.
- Corrected the spell-animation atlas phase contract. Fire casters no longer display explosion art at their own feet; Fireball now uses spark, projectile, orb, explosion, and smoke cells in semantic order, with the dedicated combat-effects sheet layered into impact and aftermath.
- Added the missing procedural Fire and Frost cast/impact motifs, plus short-lived ability-icon activation stamps for martial skills. Shared epic bursts now mix motes and directional sparks instead of emitting only identical squares.
- Added size, gravity, style, and stable flicker data to combat particles, specialized Fireball smoke/ember layers, and a 384-particle ceiling so clustered splash casts stay bounded.
- Expanded pure-rule and live-runtime smoke coverage for Fireball phase routing, arc/travel timing, arrival alignment, epic mix behavior, aftermath styles, and particle limits. Reduced Motion keeps its existing brief no-travel contract. Save schema remains v22.

## v1.65.0 - Landmarks Of The Old Road

- Added 20 original transparent world-map landmarks with distinct visual families for Green Shrine Road, Old Quarry, Glass Warrens, and Ash Fen/Red Gate.
- Regional context now changes the actual landmark silhouette, not merely its tint: shrines, camps, bridges, caves, caches, ruins, obelisks, and route services use art suited to their surrounding territory.
- Kept Midgaard and interior art authoritative, then placed regional art ahead of generic route and landmark fallbacks so authored town identities remain stable while the outer world gains character.
- Built the sheet through focused image generation, chroma cleanup, deterministic 280-pixel cell composition, and an editable Aseprite source. All 20 cells pass alpha coverage, boundary, and 18-pixel gutter validation.
- Added manifest, semantic-catalog, editor-rule, and runtime-loading coverage for the exact v1.65.0 atlas contract. Save schema remains v22.

## v1.64.1 - The Name In Fire

- Rebuilt the live tavern title as a layered forged-metal treatment: embossed shadow, ember halo, bright title face, and a center-out gold forge line over the oak-and-iron plaque.
- Added a short title reveal with controlled sparks, a brief strike flare, delayed subtitle arrival, and a restrained resting ember pulse.
- Made the treatment presentation-only and input-transparent. Reduced Motion shows the complete layered title immediately, with no scaling or drifting sparks.
- Re-exported the four corrected named-NPC cells through Aseprite with an 18-pixel safe baseline gutter, preserving their appearance while preventing cell-edge bleed and restoring clean release validation.
- Added pure animation-rule coverage for reveal timing, settled readability, bounded glow, forge-line completion, and the reduced-motion fallback. Save schema remains v22.

## v1.64.0 - Faces At The Gate

- Replaced the menu title plaque with a quieter oak-and-iron sign whose decoration no longer crosses the title or subtitle, and revised the responsive title layout around its true 3:1 proportions.
- Rebuilt the Midgaard gate family with larger closed, open, east-facing, and west-facing structures, clearer walkable openings, stronger wall connections, and exact atlas pinning.
- Corrected named world sprites for Tessa, Maud, Edda, and Yara so their gender, role, clothing, and silhouette agree with their established dialogue portraits.
- Centralized named NPC world-atlas mappings in `NpcPortraitCatalog.WorldSpriteIndex` instead of scattering anonymous renderer numbers. `Edna` also resolves safely to Edda's portrait as a spelling alias.
- Simplified the dialogue popup: smaller centered footprint, one clear speaker hierarchy, quieter body/choice surfaces, single-line topic buttons, and one contextual topic hint instead of repeated instructions and dense two-line buttons.
- Audited every pinned runtime art family. World-map materials/props/tokens, city walls/props/interiors, portraits, power icons, spell effects, the game emblem, and roaming threats passed review and were left unchanged.
- Expanded rule and runtime smoke coverage for the corrected art pins, named NPC identities, dialogue geometry, gate/NPC dimensions, cell coverage, and alpha gutters. Save schema remains v22.

## v1.63.0 - Roads With Their Own Songs

- Expanded the soundtrack from 20 to 44 original procedural loops while retaining the intentionally sparse old-school audio direction.
- Added distinct Midgaard district themes for Temple Square, Market Square, Tavern Lane, the gate watch, the cistern approach, King's Hall approach, and the safe outer road.
- Added proximity-driven landmark themes for camps and waystones, shrines, cave and dungeon thresholds, ruins and crypts, arcane sites, ancient groves, and faction camps.
- Added a pursuit score that crossfades in when an alerted roaming threat closes on the party outside Midgaard.
- Added battle themes for ratfolk plague companies, unaffiliated caster duels, elite foes, a low-health last stand, the Kobold King, and demon lords.
- Gave Party Setup, Victory, and Defeat their own music instead of reusing the tavern ensemble or falling silent.
- Centralized adaptive routing in `MusicDirectorRules` and lazy-compose the 24 new tracks only when first heard, avoiding a large launch-time and memory increase.
- Added pure routing tests and live waveform checks for registration, lazy generation, distinct arrangements, mode routing, encounter priority, and safe fallback behavior. Save schema remains v22.

## v1.62.0 - Threats With A Voice

- Added an original 20-cell roaming-threat atlas covering rat, ratfolk, kobold, drow, demon, undead, elite, boss-escort, and encounter-marker silhouettes. The generated sheet was cropped, chroma-key cleaned, cell-normalized in Aseprite, alpha-audited, and pinned by exact runtime filename.
- Replaced the two generic outer-road rat tokens with distinct Gutter Rat and Cistern Ratfolk patrol presentations while retaining the same pathing, encounter, retreat, cooldown, and save behavior.
- Added faction-aware alert, step, attack, cast, hurt, and defeat cues for kobolds, drow, demons, and undead. Existing rat and ratfolk voices now use the same routing contract.
- Added four sparse procedural combat scores selected from the living enemy roster: Kobold Hide Drums, Drow Nightblades, Red Rift War, and Bones Beneath Stone. Boss and sewer-specific scores retain priority.
- Added reusable creature-audio and roaming-threat presentation rules plus pure-rule and live-runtime smoke coverage for every cue family, music selection, atlas dimensions, alpha gutters, and all twenty cells. Save schema remains v22.

## v1.61.0 - Ash & Brimstone

- Renamed the player-facing game, window, executable, package, and release identity to Ash & Brimstone, with a new original title plate and iron-and-ember application emblem. Internal compatibility identifiers remain stable so existing data can migrate safely.
- Added one-time, non-destructive import of the former Ashen Halls save and backup when an Ash & Brimstone save does not yet exist. Save schema is v22.
- Added two reserved Midgaard interiors: King's Hall now leads through a royal corridor to Halvard's throne room, while the market shop leads into a staffed merchant hall.
- Added dedicated interior terrain and prop atlases, stable room-aware floors and walls, royal and merchant thresholds, carpets, dais stones, shop timber, forge details, court furnishings, stock displays, and readable return doors.
- Reworked dialogue response flow so selected answers replace the topic list instead of colliding with it. Halvard's sewer contract now has an explicit acceptance choice, and merchant portrait mappings match Borin, Tessa, and Maud.
- Added independent Music and SFX controls, location-aware throne-room, merchant, sewer, and boss/combat themes, seamless loop edge fades, and dedicated royal-door, shop-door, throne, and merchant cues.
- Reduced random town dressing around important NPCs and entrances so authored people, doors, and quest landmarks remain legible.
- Expanded rule and runtime smoke coverage for room portals, interior terrain contracts, audio settings, art manifests, branded save import, and the new runtime art.

## v1.60.0 - Midgaard Voices and Living Roads

- Reworked named NPC conversations around one persistent topic list. Answers now return naturally to the same choices, Esc backs out one level before closing, W/S and arrow navigation are supported, and Enter/Space invokes the highlighted response without leaking input to exploration.
- Replaced the cramped two-column dialogue choices with full-width response rows, a clearer selected state, better portrait/body proportions, and concise mouse/keyboard guidance.
- Added a new original 5x4 portrait atlas with twenty distinct Midgaard identities. King Halvard and Herald Vann no longer share a face, and the east and west gate guards are now Watchwoman Ilyra and Watchman Rusk with separate portraits, sprites, names, and dialogue.
- Removed duplicate city rewards by unifying Orren's tavern introduction and Kate/Lute's starter provisions. Kate's Diner no longer spends gold when opened; the shared bundle is purchased only through an explicit enabled response.
- Removed player-facing scaffold language from city conversations and rewrote the new guard/service material as in-world advice.
- Added two deterministic roaming rat patrols outside Midgaard. They remain visible on the map, expose bearing/distance/state in the fixed readout, respect safe-road boundaries, telegraph detection, path around blockers, and advance every second successful party step.
- Bumping or using an adjacent patrol starts a normal tactical rat encounter. Victory scatters that exact patrol for 36 travel steps; campaign retreat returns it home with six steps of grace and awards no patrol victory.
- Added saved roaming-threat identity, position, alert, grace, and respawn state. Save schema is v21; supported v17-v20 saves repair the new state deterministically.
- Fixed three stability issues found in the audit: derived-stat repair no longer revives zero-HP heroes, Sanctuary no longer loses its fresh ward on the same start-turn tick, and malformed primary saves are rejected deeply enough for a valid backup to load.
- Expanded pure-rule and live-scene coverage for dialogue navigation, distinct NPC identities, explicit purchases, duplicate population repair, roaming patrol pathing/spawn safety, backup fallback, zero-HP repair, and Sanctuary timing.

## v1.59.0 - Targeting Control and Route Reliability

- Fixed the combat input contradiction where prepared-spell text promised `Esc to clear` but Esc opened Menu instead. Esc now cancels only a genuinely armed spell or martial skill; ordinary combat still opens Menu.
- Added right-click board cancellation and a contextual `Cancel Spell [Esc]` or `Cancel Skill [Esc]` button to the combat command deck.
- Cancellation clears the pending power and returns to ordinary Attack/Shoot mode without spending the action, changing the active unit's position, or altering its remaining movement.
- Made the contextual command strip coexist with `Undo Move [U]`, so a unit can move, reconsider a power, cancel its target, and still undo the movement before committing.
- Mirrored target cancellation in the emergency combat HUD and expanded Help, pure rules, supported-resolution layout, and live uGUI callback coverage. Save schema remains v20.
- Fixed a seed-dependent world-generation gap that could leave one of the eight named outer-road junctions disconnected from Midgaard.
- Added post-generation and load-time route certification. It reconnects only unreachable junctions, restores walkable regional surface materials on repaired cells, and removes incidental non-story blockers from the short connector.
- Strengthened live-scene verification so every named junction must be reachable for the active fresh map and every deterministic seed in the expanded-map sweep.

## v1.58.0 - Tactical Repositioning

- Added a contextual `Undo Move [U]` control to the combat command deck. It appears only after the active party unit spends movement and before that unit commits an action.
- Undo returns the unit to its true turn-start tile, restores the full movement allowance and unmoved casting/Guard stance, cancels any uncommitted spell or skill target, and then disappears.
- Kept the recovery narrow and exploit-resistant: attacking, casting, using a skill, guarding, drinking an elixir, or ending the turn locks the move immediately. A loaded mid-turn save begins a new local undo origin instead of reconstructing hidden history.
- Mirrored the command in the emergency combat HUD and added `U`/Backspace keyboard support plus Help guidance.
- Expanded controller, layout, and live-scene smoke coverage for origin restoration, action lockout, supported-resolution fit, uGUI ownership, and contextual visibility. Save schema remains v20.

## v1.57.0 - Retreat and Recovery

- Added the retreat mechanic described by Midgaard's NPCs. During a normal campaign fight, Esc opens a deliberate two-step `Retreat to Midgaard` action that costs one supply, abandons the encounter and its loot, and cannot accidentally trigger with one click.
- Retreat preserves skill use from the interrupted fight, clears combat without awarding victory or story progress, restores the party at Temple Square, and writes a new safe campaign checkpoint.
- Kept testing isolated: Beta and Martial Labs never expose the campaign retreat route, while zero-supply parties receive a plain disabled reason and may still load their pre-fight checkpoint.
- Hardened Temple Square return by repairing or falling back to a reachable exploration start if an old map lacks its expected anchor.
- Expanded Help, pure rules, and live-scene coverage for offer/affordability rules, confirmation ownership, exact supply cost, combat cleanup, full recovery, and valid return placement. Save schema remains v20.

## v1.56.0 - First-Play Guidance

- Restored the useful compact exploration rail that was lost during the uGUI migration. Close Map now keeps the next waypoint, current objective, nearby sites, and all four party health/mana rows visible while the detailed Location drawer remains optional.
- Rewrote the fresh Chapter I objective and arrival feedback around one immediate task: follow the gold marker to King's Hall, move with WASD/arrows, and use Space or E beside the marked location.
- Added concise first-round plans to the Timeline for Broken Sluice, Foul Runoff, and Cistern Den. The prompts teach formation/ranged fire, mage-and-hazard priority, and final-room control without covering the board or persisting after round one.
- Expanded live-scene smoke coverage for the default compact exploration rail, fresh objective/waypoint copy, story-step updates, all four party rows, and authored combat guidance. Save schema remains v20.

## v1.55.0 - Family Playtest Path

- Simplified the normal Tavern to `Continue`, `New Game`, `Settings`, and `Exit Game`. New Game opens Party Setup, where players can still use Quick Start or customize the four-person party.
- Added quiet campaign checkpoints on first arrival in Midgaard, before each sewer encounter, after combat victories, after the safe-room choice, and after the chapter reward. Batch smoke tests and Beta Labs cannot overwrite a player campaign.
- Made Defeat return to the Tavern, where Continue restores the last safe checkpoint instead of forcing a full restart.
- Turned the Foul Runoff recovery alcove into a meaningful equipment choice: a physical +2 sluicekeeper broadsword with Strength or a shock +2 stormglass ritual staff with Intelligence. The choice is single-claim, auto-equips sensibly, and gates the Cistern Den until resolved.
- Expanded the Journal with safe-room choice state alongside the three sewer rooms, proof bundles, Borin's rat-pelt reward, and the Old Road teaser.
- Added rule coverage for menu shape, checkpoint eligibility, safe-room item identity, and idempotent choice state. The live runtime smoke now walks the production sewer story callbacks from Broken Sluice through Borin's dialogue and loot comparison.
- Save schema remains v20.

## v1.54.0 - Action Integrity and Battlefield Objects

- Added one canonical resolved-action path to the combat controller. Normal attacks, formulas, targeted and instant abilities, rituals, and destructible-cover attacks now commit the active unit's action through the same lifecycle.
- Made rejected resolvers non-destructive: an invalid or failed command leaves movement, action availability, and the choose-action phase intact instead of silently consuming the turn.
- Unified player and enemy feedback when attacking generated trees and stone. Melee and ranged weapons retain their release/contact profiles, while arcing enemy attacks use school-colored travel and cast audio.
- Added compact splinter and rubble break motifs that stay near the impact edge instead of painting procedural bars over battlefield art. Ranged cover attacks no longer play a melee lunge.
- Expanded combat hover information with current and maximum cover integrity, direct-shot blocking, arc behavior, and generated-tree lifetime. Fire, gas, web, ice, sanctuary, and curse previews now report remaining rounds.
- Expanded rule and runtime smoke coverage for accepted/rejected action resolution, tree and stone break motifs, cover material audio, integrity copy, and terrain-duration copy. Save schema remains v20.

## v1.53.0 - Route Chart and Battlefield Breakage

- Turned discovered outer-road junctions into persistent route knowledge. The Location panel now reports the nearest charted marker with a cardinal bearing and Manhattan walking distance when the party is beyond Midgaard guidance.
- Added an Outer Road Chart to the Journal with aggregate progress and one concise row per discovered junction. Unvisited crossroads remain hidden instead of becoming an automatic map reveal.
- Centralized junction discovery keys, chart counts, nearest-marker selection, direction labels, and distance copy in testable exploration rules without changing the save schema.
- Routed attacks against generated tree and stone cover through the existing weapon feedback profiles, preserving slash, heavy, thrust, and projectile timing instead of falling back to generic flashes and sounds.
- Added distinct procedural wood and stone contact layers. Cover destruction adds a restrained break tail while ordinary damage remains lighter.
- Added small edge-safe guard and defeat contact motifs without restoring bars or badges over unit sprites. Reduced Motion still suppresses the animated marks.
- Expanded rule and runtime smoke coverage for route-chart depth isolation, nearest bearings, singular/plural distance copy, cover-material audio, and the live Location guidance line. Save schema remains v20.

## v1.52.0 - Road Signs and Weapon Impacts

- Named all eight authored outer-road junctions and made the shared junction catalog drive generation, discovery, look text, ground markers, and smoke tests.
- Added subtle road-clearing marks beneath the map art, one-time junction banners and log entries, small exploration XP awards, and a restrained procedural wayfinding chime.
- Kept the feature save-compatible by storing junction visits in the existing discovered-zone list. Existing maps gain marker roles only where the corresponding route cell is already walkable.
- Added shared weapon feedback profiles for slash, heavy, thrust, and projectile attacks. Release timing, contact sound, contact volume, and impact motif now classify from the same rule path.
- Added animated slash strokes, thrust points, heavy cracks, arrow contacts, and glancing miss marks, all staged to the matching hit delay and suppressed by Reduced Motion.
- Layered four new procedural weapon-contact sounds over the existing target-material response, so weapon shape and armor material are both audible without replacing the sparse retro sound style.
- Added a quiet spell-release articulation to formula casts while keeping martial powers free of spell sounds.
- Expanded rule and runtime smoke coverage for junction identity/reachability, weapon classification, bounded contact layering, spell-release routing, and the new procedural audio bank. Save schema remains v20.

## v1.51.0 - Expanded Roads

- Expanded newly generated world maps from 46x30 to 50x32 while preserving the existing 11x7 Local Map and 17x9 Region Map viewports, so exploration gains space without shrinking tiles or actors.
- Added a deterministic eight-region road circuit outside Midgaard. Old Quarry, Gloam Courts, Glass Warrens, Dusk Market, Red Gate, Salt Cisterns, Ash Fen, and Green Shrine Road now each receive a connected route backbone plus small junction clearings.
- Increased procedural variety by one district and one loop road, with a small proportional increase in district carving rather than a broad increase in object density.
- Made generation helpers and biome thresholds use each serialized map's own dimensions. Existing 46x30 saves therefore retain their original zoning instead of being reclassified by the new-map constants.
- Limited Midgaard-specific wayfinding to the city and its gate approaches. Once the party travels farther out, the HUD selects a reachable regional landmark and reports its direction and route length.
- Raised the supported window floor to 1280x720 when the display allows it; smaller displays use their full available dimensions. This prevents the party rail and exploration command deck from colliding under an artificially tiny window.
- Added rule and runtime smoke coverage for expansion bounds, route density, all eight connected regions, exact fresh-map dimensions, and legacy biome compatibility. Save schema remains v20.

## v1.50.0 - Midgaard Street Life

- Added an original 20-cell transparent street-life atlas with rain barrels, stalls, carts, laundry, shrines, forge work, votives, tavern tables, guard barriers, sewer tools, civic fixtures, and other district-scale city details.
- Added a separate 16-cell ground-decal atlas for drains, puddles, tracks, leaves, market litter, mosaics, paving repairs, and small memorial details.
- Routed street props deterministically by Midgaard district. New art replaces a portion of existing ambient prop slots instead of increasing tall-object density or changing map generation.
- Kept ground decals sparse, semi-transparent, outside party-adjacent and authored-object cells, and exclusive to Local Map so the Region Map remains readable.
- Preserved editable Aseprite masters, chroma-key sources, prompts, and alpha-validation reports. Runtime loading is pinned through the approved art manifest and rejects missing or malformed sheets.
- Added rule and runtime smoke coverage for atlas dimensions, per-cell alpha bounds, manifest uniqueness, local/wide-map density rules, and live runtime loading. Save schema remains v20.

## v1.49.0 - Storm Tavern and Signature Power Spectacle

- Replaced the opening backdrop with original full-scene pixel art of a busy Midgaard tavern during a storm. Warriors, travelers, common folk, a bright hearth, rain-lit windows, and the open doorway now establish the world before the first click.
- Added restrained title-screen rain streaks and occasional double lightning flashes. Reduced Motion disables the animated storm layer while preserving the painted weather and readable title composition.
- Expanded the procedural title soundscape with separate rain, tavern-crowd, and hearth cues, plus a new 24-second lute, drone, fiddle, cup, and frame-drum ensemble that lands on a clean loop seam.
- Added a validated 4x4 runtime combat-effect atlas and semantic visual motifs for fire, frost, shock, holy, nature, void, rift, transformation, charge, slash, shadow, and smoke. Broad effects render beneath units so combatant art and tactical markers remain readable.
- Gave Death Burst, Greater Demon, Veil Step, Arcane Tempest, Abyssal Ascendance, Rift Seal, Charge, Whirlwind, Execute, Ambush, and Eviscerate distinct cast/impact sound identities. Restored the missing Sleep cue.
- Corrected the runtime art manifest to use the v1.30 Midgaard directional wall atlas and suppressed procedural road strokes beneath semantic city landmarks and gates.
- Expanded rule and runtime smoke coverage for the new effect atlas, motif routing, power identities, tavern ambience waveforms, and title music duration. Save schema remains v20.

## v1.48.0 - Deliberate Services and Workshop Voices

- Replaced the generic Midgaard guard line with Watchman Rusk's reusable three-topic conversation about safe streets, ratfolk formations, and gate discipline. The tavern building now opens Orren's established conversation instead of a separate disposable line.
- Rebuilt Borin, Tessa, and Maud as explicit service conversations. Talking no longer spends gold automatically: the priced fitting, weapon, or enchantment is a visible choice, unavailable services explain their requirement, information topics remain free, and completed one-time services become disabled.
- Preserved the existing hauberk, town-forged weapon, starter enchantment, auto-equip, loot comparison, story-flag, and rat-pelt reward behavior behind the new confirmation step.
- Added original procedural coin, armor-fitting, weapon-draw, and rune-binding sounds and routed each successful service through its own short transaction cadence.
- Expanded live-scene coverage for no-spend-on-open, exact confirmed prices, free information topics, one-time completion flags, disabled repeat purchases, service waveform diversity, and guard/tavern conversation lifecycle. The Fireball spatial-audio test now clears unrelated pending cues before measuring its target pan. Save schema remains v20.

## v1.47.0 - City Voices and Exploration Soundscape

- Replaced the remaining disposable dialogue lines for City Courier Tovan, Novice Healer Sera, wounded traveler Edda, Stable Hand Pell, Kate and the provision seller, and Herald Vann with reusable three-topic conversations. Their answers cover safe routes, tactical healing, rat formations, kobold road signs, provision discipline, royal writs, and progression while preserving existing one-time rewards.
- Added a sparse semantic exploration soundscape driven by the current zone and nearest meaningful landmark. Midgaard can now surface temple bells, market murmur, forge strikes, gate chains, city distance, and cistern drips; outer routes add wind, distant drums, and quarry stone.
- Added original procedural waveform generators for nine ambient families instead of recycling UI or combat beeps. Cues vary subtly in pan and pitch, wait 9-15 seconds between events, avoid recent foreground SFX, and suspend while any modal conversation or menu is open.
- Extended visual smoke tooling so `-ashen-dialogue-smoke` and `-ashen-dialogue-response-smoke` can target `courier`, `novice`, `traveler`, `stable`, `diner`, `herald`, `nessa`, `brann`, or the default `mira` conversation.
- Expanded rule and live-scene tests for landmark/zone routing, ambience cadence and mix bounds, waveform diversity, scheduler activation, and every new production conversation. Save schema remains v20.

## v1.46.0 - Gate Thresholds, Living Conversations, and Sound Identity

- Replaced oversized routine arrival banners with compact top-chrome notices, preserving the large cinematic treatment only for important combat powers. Midgaard names and minor exploration feedback no longer cover gates or map landmarks.
- Refined Midgaard gate composition with orientation-aware approach stone and horizontal road thresholds for east/west passages. Added a deterministic `-ashen-gate-smoke east|west|north|south` visual mode for close-up regression captures.
- Tightened the migrated dialogue layout with a larger portrait, denser readable body copy, and topic hints inside each choice. Nessa, Mira, Orren, Brann, King Halvard, and Yara now support several questions in one conversation and an explicit leave choice without reopening the popup.
- Rebuilt procedural material impacts as distinct transient/resonant voices for flesh, leather, mail, plate, and shields. Rat and ratfolk chatter, attack, casting, injury, and defeat now use separate multi-chirp contours rather than generic oscillator sweeps.
- Added distinct cast voices for holy/mending, light, ember, frost, shock, nature, hex/death, and pact/rift schools, and routed formula impact profiles to those semantic cues.
- Expanded rule and runtime smoke coverage for compact/cinematic banner bounds, cast-school routing, generated waveform differences, and the production multi-topic dialogue lifecycle. Save schema remains v20.

## v1.45.0 - City Conversations and Material Impacts

- Promoted the Aseprite-normalized v1.30 Midgaard gate atlas and enlarged gatehouse composition in Local and Region Map. Open east/west exits now reveal a dark portal breach, threshold, and road continuation instead of appearing pasted over an unbroken wall; sealed north/south gates keep their collision rules.
- Increased ordinary Midgaard prop presence slightly while preserving the quieter floor hierarchy and edge-only interaction cues.
- Added final-page dialogue choices to the migrated uGUI conversation screen, with click-safe modal ownership and 1-4 keyboard selection. Nessa, Mira, Orren, Brann, King Halvard, and Yara now answer quest-aware topics about city work, services, roads, healing, and regional danger.
- Split audio preferences into independent SFX and Music controls in Tavern Settings and the pause menu, while retaining the shared master mute.
- Added procedural flesh, mail, plate, and shield impacts, plus restrained rat/ratfolk chatter, casting, injury, and defeat voices. Weapon resolution now chooses its impact layer from the actual target and Guard state.
- Expanded rule/runtime smoke coverage for v20 migration defaults, gate footprint/art routing, dialogue choice ownership, and the new generated audio clips. Save schema is v20; v17-v19 saves remain loadable and repair missing music volume safely.

## v1.44.0 - Midgaard Voices and Gatehouses

- Reworked Midgaard gate composition so gatehouses occupy a readable multi-tile silhouette, open exits receive a road threshold, sealed north/south gates use sealed art, and permanent progression badges no longer cover the architecture.
- Removed the legacy second-opacity gate redraw and generic prop plinth from gate art. Nearby and quest cues remain edge-only, while east/west traversal and north/south collision rules are unchanged.
- Upgraded NPC conversations with the approved 16-cell portrait atlas, larger uGUI composition, readable sentence-paced pages, dynamic Next/Continue controls, page indicators, keyboard advancement, and preserved modal click protection.
- Added distinct dialogue open/page/close and gate cues, staged bow/thrust/light/heavy weapon releases before impact, and an extra release layer for epic spells.
- Added smooth two-source crossfades between tavern, world-zone, and combat music instead of abruptly stopping and restarting loops.
- Expanded rule and runtime smoke coverage for gate sizing/art mapping, dialogue paging, portrait routing/rendering, audio construction, music crossfade ownership, and multi-page modal input. Save schema remains v19.

## v1.43.0 - Shared Threat Forecasts

- Added one shared normal-attack forecast for legality, ranged/melee mode, line of sight, hit chance, post-mitigation damage range, damage type, Guard state, expected damage, and threat severity.
- Rewired attack hover text, targeting reticles, actual attack validation, enemy reach checks, projected-position AI checks, target scoring, enemy intent, and migrated target cards to consume the same forecast instead of reconstructing partial answers.
- Made enemy intent concrete when a normal strike is ready: the command prompt and target card now show the same hit chance and damage range the attack resolver uses. Severe and potentially lethal attacks receive explicit labels; special powers remain clearly identified as powers rather than fake weapon estimates.
- Replaced the enemy-board `!/?` text boxes with narrow edge rails colored for pressure, direct danger, severe danger, or lethal danger, preserving the generated sprite art beneath them.
- Added pure forecast classification tests and production-path runtime checks for hover/HUD/intent parity. Save schema remains v19.

## v1.42.0 - Enemy Intent and Tactical Roles

- Added one shared enemy-tactics profile for brutes, skirmishers, marksmen, casters, support units, and bosses. Existing BFS movement, target scoring, hazard avoidance, firing-distance choices, and move-plus-attack permissions now consume the same role definitions.
- Made rank matter more cleanly: brutes and ranged specialists can step once and attack, veteran skirmishers gain the same limited option, ordinary skirmishers must choose between a full advance and a strike, and no enemy attacks after a long move.
- Improved target priorities without hidden rule duplication. Marksmen seek exposed casters, brutes pressure ranged backliners, wounded targets attract skirmishers, while Guard, Stealth, resistances, and weaknesses remain meaningful deterrents or opportunities.
- Added a concise enemy-turn intent line and an edge-only focus reticle. The HUD and board use the production target scorer, support priests can identify a wounded ally they are preparing to ward, and no new labels or bars cover sprite bodies.
- Added catalog-wide tactical profile tests plus production-path runtime checks that the enemy HUD target and intent match the AI focus. Save schema remains v19.

## v1.41.0 - Clear Cover and Command Deck

- Removed the procedural body-wide damage strokes that were stamped over generated tree and stone cover. Breakable cover now preserves its sprite art and communicates integrity through compact bottom-edge pips only.
- Rebuilt the migrated combat command deck around the existing generated command atlas. Move, Attack/Shoot, Spellbook or Combat Skills, Guard, Elixir, and End Turn now use large icon wells, readable labels, keycaps, and plain-language secondary text.
- Added one canonical command prompt with hover-specific rules and disabled reasons, separated targeted commands from instant commands, and made End Turn wider and gold-accented when movement or action resources are spent.
- Updated the emergency IMGUI command deck to match the uGUI geometry and hierarchy, while reserving enough board space for the taller bar at supported resolutions.
- Added rule and production-path runtime coverage for command geometry, End Turn promotion, prompt ownership, and generated icon routing. Combat rules and save schema remain unchanged at v19.

## v1.40.0 - Living Battlefields

- Rebuilt persistent combat fields as animated, translucent battlefield surfaces beneath units. Fire tongues, ice glaze and cracks, drifting gas, web geometry, sanctuary runes, and curse fractures now transform the affected tile while keeping sprites readable.
- Kept field icons as smaller center emblems and left duration/status telemetry at tile edges. Reduced Motion freezes the ambient field motion and suppresses placement/activation bursts without hiding the tactical state.
- Added field placement afterglow and start-turn activation feedback with bounded particles, glyphs, flashes, spatial audio, and deterministic pitch variation.
- Gave mend, ember, hex, and pact casting distinct procedural voices. Player formulas and enemy powers now route through matching cast, impact, and aftershock cues, including dedicated fire, ice, gas, web, sanctuary, and curse field sounds.
- Broadened the game soundscape with stone, earth, wood, and water footsteps, a softer NPC dialogue-open cue, a stair/door cue, and a repaired generic hit sound.
- Added rule and production-path runtime coverage for field translucency/motion bounds, Reduced Motion, school audio identity, runtime clip creation, and live field activation. Save schema remains v19.

## v1.39.0 - Ritual Warfare

- Turned kobold glyphs and demon rifts into real battlefield countdowns. If ignored, they open into a queued kobold reinforcement or lesser demon, and unresolved rituals now prevent premature victory.
- Added direct counterplay: any weapon can disrupt a ritual, heavy melee and axes break it faster, ranged attacks work from safety with lower disruption, and compact edge pips/countdowns expose integrity without covering the art.
- Added the level-three mend/ember formula Rift Seal. It instantly closes rituals or unravels hostile fire, ice, gas, web, and curse fields, with dedicated Spellbook identity, targeting language, previews, resonance audio, and sealing effects.
- Gave Arcane Tempest a dedicated lightning-fork flourish across its actual cross footprint while preserving its established damage rules and Reduced Motion cadence.
- Added rule and production-path runtime coverage for ritual victory stakes, disruption balance, field targeting, Rift Seal resolution, ritual reinforcement initiative, and Arcane Tempest presentation. Save schema remains v19.

## v1.38.0 - Elder Magic and Abyssal Ascendance

- Added three high-level caster powers for prototype testing: Veil Step teleports an ember caster to a legal distant tile, Arcane Tempest delivers an epic shock-area strike with a stun chance, and Abyssal Ascendance transforms a pact warlock into a greater-demon battle form for four turns.
- Made demon form mechanically meaningful: transformed warlocks gain physical and pact/death/mind power, reduce incoming damage, receive a brief shield/regen surge, use greater-demon battle art, and visibly revert when the binding ends.
- Expanded the Spellbook into a larger, more stable two-pane modal. School-matching future powers now remain visible as locked cards with unlock levels, while selected details carry target, footprint, path, focus benefit, and elder-tier information without crowding each row.
- Added dedicated targeting, previews, icon routing, caster auras, impact profiles, layered procedural SFX, low-impact resonance, and Reduced Motion behavior for all three elder powers.
- Expanded Beta Lab with level-six Mage and Pact test kits and added production-path runtime coverage for locked spell visibility, teleport movement, transformation bonuses, damage reduction, art switching, and modal click ownership. Save schema remains v19.

## v1.37.0 - Power Crescendo and Unified Combat Skills

- Added caster-origin power auras for player formulas, all martial skills, and named enemy powers. Pixel rings gather behind the acting sprite while small rune motes travel toward the target, then hand off to the existing projectile and impact timeline.
- Made focused casting visible in the migrated combat HUD. An unmoved trained caster now clearly shows `FOCUS -1 MP +1R`, and focused formulas receive a brighter gold-accented casting aura without changing their established rules.
- Routed all 18 warrior, rogue, and ranger skills through one staged audiovisual pipeline. Every skill now has an intentional cast, impact, and optional aftershock identity; legacy success sounds were removed so the new six-voice mix does not double-fire.
- Added reaction crescendos for tactical spell/terrain combinations. Gas ignition, web flare, steam, conduction, and related reactions can promote the impact echo by one visual tier, add bounded particles, play a dedicated resonance layer, and briefly clear more room in the music mix.
- Added subtle epic-only shimmer and low-impact layers while retaining the compact Reduced Motion path and the established action-resolution delays. Damage, targeting, initiative, AI, and save schema remain unchanged at v19.
- Expanded rule and runtime coverage across all martial profiles plus production-path focused Fireball, gas resonance, Aimed Shot, and Kobold King Fireball presentation.

## v1.36.0 - Impact Echoes and Spatial Combat Mix

- Added a board-clipped visual impact language for spells and skills: contracting target brackets lead into a bright impact core, expanding pixel rings, and stronger cardinal shock rays.
- Scaled impact echoes from restrained utility powers through medium battlefield spells to epic finishers and boss magic. Reduced Motion suppresses the animated echo while retaining the compact power cue.
- Added a reusable six-voice procedural SFX pool so cast, impact, aftershock, and nearby combat sounds can overlap without cutting one another off.
- Positioned impact audio across the tactical board with bounded stereo pan and subtle deterministic pitch variation, then added brief intensity-scaled music ducking so Fireball, Meteor Shower, and boss powers remain audible without becoming loud or muddy.
- Expanded pure rule and production-path runtime coverage for impact intensity, echo duration, spatial mix bounds, audio pooling, Fireball telegraphing, and the Kobold King's epic Fireball. Combat rules and save schema remain unchanged at v19.

## v1.35.0 - Enemy Power Telegraphs

- Added named power identities for enemy priests, shamans, wizards, adepts, glass mages, rat mages, drow casters, spores, cinderlings, lesser demons, shades, and the Kobold King.
- Reused the approved spell and skill art in enemy power callouts, giving Death Ball, Bone Hex, Plague Signs, Royal Aegis, King's Charge, Crooked Fireball, Royal Ice Lance, and related powers readable icon-backed warnings.
- Routed enemy specials through the shared projectile-to-impact timeline. Existing beams lead delayed glyphs, floating results, particles, flashes, and staged procedural sound instead of disappearing into an immediate turn change.
- Enemy specials now hold initiative for a short intensity-scaled resolution beat; ordinary attacks and movement remain brisk, and Reduced Motion keeps every special near-instant and steady.
- Added catalog regression coverage plus a production-path Kobold King Fireball runtime smoke test for cue identity, effect order, delayed audio, timeline cleanup, and initiative locking. Combat math, AI choices, and save schema remain unchanged at v19.

## v1.34.0 - Battlefield Resolution Timeline

- Added a shared impact timeline to player formulas and combat skills. Projectiles begin first; target glyphs, damage/healing text, particles, flashes, and status feedback now arrive on the configured impact beat.
- Staggered Meteor Shower strikes across its five-cell footprint and sequenced splash arcs, Whirlwind victims, and Volley targets instead of resolving every visual on the same frame.
- Promoted existing tactical synergies into the power outcome banner. Gas ignition, web flare, steam burst, shock conduction, frostbind, ward cracks, doom echoes, sanctuary clashes, and related reactions are now visible at the moment they occur.
- Delayed outcome text until impact while retaining immediate power identity, clear turn locking, board-only motion, and the near-instant Reduced Motion path.
- Expanded runtime smoke coverage to cast a production-path Fireball into poison gas and verify projectile timing, delayed feedback, queued impact audio, reaction capture, and timeline cleanup. Save schema remains v19.

## v1.33.0 - Signature Impact Cadence

- Added pure impact profiles for every formula and combat skill so cast cue, impact cue, aftershock, particle scale, board shake, and initiative hold share one tested identity.
- Gave Fireball, Meteor Shower, Death Burst, Tree Cover, Heal/Circle Heal, pact summons, Charge, Whirlwind, Execute, Ambush, Eviscerate, and Volley distinct staged audiovisual cadence.
- Replaced same-frame signature sound stacking with a bounded delayed-SFX queue. Meteor now lands before its fire aftershock, Volley separates release from arrow rain, and Reduced Motion collapses each sequence to one restrained impact cue.
- Added delayed aftershock particles, impact flashes, and a short board-only shake for forceful powers. Healing stays steady, tactical controls do not move, and all combat rules remain unchanged.
- Added profile and timing regression coverage, including formula-wide cue ordering and effect bounds. Save schema remains v19.

## v1.32.0 - Power Feedback Continuity

- Carried signature spell identity from the Spellbook onto the battlefield with short-lived, target-tile impact glyphs for all 25 signature formulas.
- Corrected stale spell-animation routing: healing now uses restoration art, cold uses ice, shock uses lightning, and light, sanctuary, cleanse, ward, death, and generic formula fallbacks use their intended cells.
- Reworked floating combat feedback into compact semantic icon-and-text chips. Damage type, healing, resistance, bleed, poison, sleep, web, stealth, stun, marks, and other results no longer share one snowflake backplate.
- Corrected Broadhead Shot and Disrupting Shot impact art to use arrow-impact and stun cues rather than unrelated camouflage and quiver cells.
- Added pure feedback-routing rules, regression coverage, and a deterministic combat-feedback visual-smoke mode. Save schema remains v19.

## v1.31.0 - Signature Power Art

- Added 20 distinct combat-skill icons for warrior, rogue, and ranger powers, including Charge, Whirlwind, Stealth, Volley, and Smoke Bomb.
- Added a dedicated 25-cell signature-spell atlas so major formulas such as Tree Cover, Fireball, Meteor Shower, Death Burst, and the three pact summons no longer collapse into generic school symbols.
- Replaced the opaque legacy magic-symbol sheet with 16 transparent school and utility icons for card fallbacks, formula cues, and spellbook presentation.
- Wired the new art into both migrated spell/skill modals and their emergency IMGUI pickers, with pinned runtime filenames so newer unrelated files cannot silently replace the approved atlases.
- Added repeatable chroma cleanup, cell normalization, Aseprite export, and contract validation for all 61 icons. Save schema remains v19.

## v1.30.0 - Gameplay Overlay Recovery and Art Solidity

- Restored the migrated combat HUD as the production action surface and kept all six commands visible: Move, Attack/Shoot, Spellbook or Combat Skills, Guard, Elixir, and End Turn.
- Preserved the IMGUI tactical board beneath the non-overlapping uGUI chrome, while full-screen Spellbook, Skills, Dialogue, Armory, Loot, Help, and Menu overlays retain exclusive visual and pointer ownership.
- Added fail-open recovery for unavailable Menu, Help, Armory, and Loot panels so a damaged presentation screen cannot leave gameplay trapped behind an invisible overlay.
- Strengthened only the known faint cells in the kobold-route, route-scaffold, and Midgaard-sewer sheets through Aseprite, pinned the repaired v1.30.0 files in the runtime manifest, and added solid-alpha regression checks.
- Expanded runtime boot coverage to open and dismiss a real Midgaard conversation, verify six interactive combat commands, open the spellbook, block click-through, and restore the command bar after closing it.
- Kept save schema v19.

## v1.29.0 - Exploration Art Contracts and Map Hierarchy

- Pinned every active world-map token, NPC, town, city-prop, landmark, biome-prop, gate, wall, and material atlas so an unrelated newer PNG cannot silently replace production art.
- Normalized seven sprite and prop atlases into exact 5x4, 256-pixel cells with safe gutters and added per-cell alpha validation to the rule smoke suite.
- Replaced the quarantined geometric biome placeholders with twenty original environmental props, enabled them sparsely on Local Map only, and kept Region Map focused on navigation.
- Changed material terrain to draw one complete tile per map cell with deterministic mirroring, eliminating the repeated 2x2 poster effect.
- Trimmed sparse city-wall cells to visible bounds so wall runs connect, removed permanent gate card frames, simplified the party token treatment, and condensed distant Region Map actors into semantic markers.
- Kept save schema v19 and retained the tested combat action bar and click-owning NPC dialogue paths.

## v1.28.0 - Exploration Materials and Runtime Art Pinning

- Pinned the approved Midgaard gate, city-wall, and exploration-material atlases so rough historical files can no longer silently override active runtime art.
- Added a 16-cell Aseprite-authored material atlas for city districts, roads, ruins, fen, quarry, glass, ash, cistern, and sewer surfaces. Paths now compose over biome material instead of repainting every route as an unrelated floor.
- Improved Midgaard district identity, city-wall continuity, gate presentation, Local Map object scale, interactive-object opacity, and Region Map framing while preserving the existing save schema.
- Stopped object types from recoloring the ground beneath them and reduced ordinary object frames, allowing NPCs, landmarks, and quest sites to read as sprites rather than full-tile cards.
- Retained and regression-tested the six-command combat bar and click-owning NPC dialogue modal while the tactical board and exploration map continue their incremental uGUI migration.

## v1.27.0 - Overlay Ownership and Exploration Semantics

- Repaired combat HUD ownership so a partially initialized uGUI canvas cannot suppress the production six-command IMGUI action bar. Opening and closing Spellbook or Combat Skills preserves the bar, and fallback buttons now respect modal click shielding.
- Made Dialogue, Combat Skills, Spellbook, Armory, Loot, Pause, and Help transactional: each overlay builds and refreshes while hidden, claims input only after panel geometry, raycasting, controls, and a usable EventSystem are present, and falls back cleanly when construction fails.
- Added full-frame pointer suppression for modal close/open events so clicks cannot pass through a popup into exploration tiles, combat targets, or fallback commands.
- Split exploration surface data into independent material and semantic-role grids. Roads, trails, bridges, city paths, plazas, walls, and biome ground now render and repair without conflating appearance with passability.
- Added deterministic v17/v18 exploration-map migration to save schema v19, plus malformed-primary-save fallback coverage.
- Trimmed route, dungeon, faction, and service scaffold cells to visible art bounds; reinforced only known soft-alpha interactive objects and increased ambient Midgaard prop visibility without restoring opaque square backings.
- Expanded rule/runtime smoke coverage for semantic surface grids, real Midgaard NPC and Quest Board conversations, every migrated modal, the combat action path, and modal-close action-bar recovery.

## v1.26.0 - Gameplay Path Hardening

- Locked the hybrid combat HUD to a testable six-command contract: Move, Attack/Shoot, Spellbook or Combat Skills, Guard, Elixir, and End Turn. The production IMGUI action bar remains visible while the tactical board migration is incomplete.
- Reworked the NPC dialogue smoke path to move beside a real Midgaard target, face it, invoke the normal exploration Use command, and advance the resulting uGUI conversation through its actual Continue button.
- Rebalanced Local Map density to fewer, larger cells and removed 2x2 terrain macro sampling that turned scenic atlas cells into repeated collages.
- Restricted routine terrain selection to calm, zone-compatible ground cells, lowered floor intensity, and removed per-cell alpha checkerboarding so terrain reads as one surface behind gameplay objects.
- Replaced opaque object backing rectangles with restrained contact shadows and offset silhouettes. Quest targets retain a subtle frame while ordinary props and NPCs remain art-first.
- Expanded rule and runtime regression coverage for viewport density, terrain contracts, object grounding, combat commands, and root-rendered conversation ownership. Save schema remains v18.

## v1.25.0 - Gameplay Surface Recovery

- Made the IMGUI action bar the explicit production combat surface while the tactical board remains IMGUI. The inactive migrated HUD is now a view-model host only and cannot become an invisible raycast layer.
- Added a full-screen emergency Spellbook/Combat Skills picker. If the migrated ability modal cannot render, powers remain selectable and the board remains protected from click-through.
- Fixed the normal sewer-slice Midgaard Quest Board dispatch so its advertised Talk action opens City Notices instead of being rejected as prototype route content.
- Added dialogue-to-loot sequencing for armorer, weapon-vendor, and rat-pelt rewards. Spoken lines stay in the scrollable dialogue, then advance to a larger loot comparison that waits for explicit dismissal.
- Increased Midgaard ambient-prop size, opacity, and restrained density; strengthened interactive-object grounding and frames; and made viewport density depend on actual board dimensions.
- Expanded runtime smoke coverage to exercise the Quest Board and dialogue reward handoff through normal overlay ownership, plus the canonical combat command and Spellbook paths. Rule and runtime smoke suites pass. Save schema remains v18.

## v1.24.5 - Combat HUD and Dialogue Recovery

- Restored the complete hybrid combat HUD as a guaranteed visible surface while the tactical board remains IMGUI. Its action bar, active/target cards, top chrome, and Timeline can no longer disappear because an invisible uGUI canvas reports itself healthy.
- Added a compact initiative queue to Timeline, including active-unit emphasis and a clear next-round marker for units that enter the following rebuilt round.
- Restored migrated uGUI NPC conversations as opaque, scrollable, input-owning modals. Dialogue, Spellbook, Combat Skills, Armory, Loot, Help, and Menu now take full-frame ownership above the hybrid board; opening and closing suppress board input, and Space/Enter cannot dismiss a conversation in the same frame that opened it.
- Guarded contextual exploration Use against stale/background callbacks so a hidden control cannot re-enter NPC rewards or interactions beneath dialogue.
- Raised ambient Midgaard and biome-prop visibility, strengthened the grounding and contrast behind thin NPC silhouettes, and reduced visually noisy Midgaard/ruins ground variation.
- Added Unity's built-in Screen Capture module explicitly so player-frame visual smoke capture compiles reliably. Save schema remains v18.

## v1.24.4 - Exploration Route Recovery

- Replaced the collapsed Location tab with a persistent Travel Rail showing the next objective, nearby landmarks, current interaction, and compact party health/mana.
- Made exploration facing a tie-breaker for equally useful adjacent targets while preserving objective priority, and replaced four large neighboring-tile boxes with compact directional edge cues.
- Isolated exploration input transactions so Space/E cannot open combat and immediately spend the first combat turn from the same key frame.
- Added occupancy-aware route certification for critical Midgaard services, gates, stairs, the Dusk Market cave, and prototype route landmarks; unreachable generated targets are relocated or connected after map creation and load repair.
- Pruned disabled patrols and stairs from old sewer-slice maps, prevented sewer-slice descent from bypassing its authored ending, corrected the Old Road waystone zone, and tied Kobold King cave cleanup to the exact Dusk Market cave.
- Reserved semantic Midgaard ground art for the landmark that owns it. Ordinary streets now use calm paving, ambient city props are smaller and sparser, distance shade is capped, and unexplored cells use a coherent fog veil instead of stripe-like marks.
- Raised exploration resource, command, and hotkey type sizes. Save schema remains v18.
- Added exact in-player smoke capture and explicit test-resolution support so visual QA can exercise the rendered game at 1280x720 and 1920x1080 without capturing unrelated desktop content.
- Stacked Beta Lab controls above the canonical combat prompt at narrow resolutions so tester buttons, active-unit text, Audio, and Menu no longer occupy the same strip.

## v1.24.3 - Exploration Readability Repair

- Corrected the 5x4 world-terrain contract so passable moss, mire, quarry, glass, and ash use ground cells while forest, mire, cliff, and red barriers use dedicated blocked cells.
- Removed the unreachable 35-cell forest auto-tiling path and its passable-ground fallback; the enforced 20-cell runtime atlas now has one coherent contract.
- Quarantined the rough biome-prop placeholder sheet, sharply reduced procedural ground accents, and strengthened object plinths so NPCs, camps, caches, shrines, and the party remain the visual focus.
- Increased party-token occupancy in both Local and Region views, snapped map cells to whole pixels, and based adaptive viewport density on the board space left after HUD reservation.
- Reserved map space for both the expanded Location panel and its collapsed tab, compacted the Location detail rows to avoid overlap, and removed duplicate hover frames from the active interaction target.
- Added canonical IMGUI safety surfaces for the hybrid map/combat renderer: exploration now always exposes Action, Party, Journal, Region/Local Map, Details, and Menu; combat always exposes its full action bar, active/target cards, and Timeline; NPC dialogue always opens an opaque, scrollable, input-capturing conversation panel.
- Kept the migrated root uGUI screens mounted for continued migration work, but suppresses their raycasts while the guaranteed hybrid safety surfaces are active. Save schema remains v18.

## v1.24.2 - Playable Surface Recovery

- Rebuilt every migrated uGUI screen as an independently owned root overlay canvas. Combat commands, active/target cards, party and enemy panels, Timeline, Dialogue, Loot, Armory, Pause, Help, Spellbook, and Combat Skills no longer depend on an invalid nested overlay-canvas hierarchy.
- Added emergency IMGUI action and dialogue fallbacks, plus EventSystem/input-module repair, so an incomplete presentation initialization cannot leave combat or conversation unusable.
- Made longer NPC conversations scrollable and blocked closing clicks from reaching the world or combat board beneath them.
- Fixed armorer and weapon-vendor reward sequencing: their spoken response now remains visible in the loot popup instead of one modal silently dismissing the other.
- Raised environmental-object opacity in close and wide exploration views and restored the semantic 5x4 world-terrain atlas as `world-map-exploration-tile-atlas-runtime-v1.24.2.png`.
- Hardened exploration generation and repair so blocking route objects keep a safe approach, east/west Midgaard gates use the same traversal path for mouse and keyboard, camps are usable terrain, and disabled generic encounter markers cannot strand the party.
- Expanded runtime smoke coverage to invoke real Dialogue, Journal/Armory, Loot, Pause, Move, Utility, and Spellbook buttons and verify their canvases are root-rendered and interactive. Save schema remains v18.

## v1.24.1 - uGUI Overlay Recovery

- Restored the complete combat HUD stack, including top chrome, active and target cards, Timeline, four command rows, and utility actions, with automatic reconstruction if a critical uGUI surface is only partially initialized.
- Kept exploration and combat HUDs mounted as dimmed, non-interactive context beneath Dialogue, Spellbook, Combat Skills, Armory, Loot, Pause, and Help while correcting canvas order so every popup remains above the board HUD.
- Restored reliable Midgaard conversations and added a proper Quest Board dialogue window instead of log-only text.
- Blocked the closing click from passing through dialogue, spell, skill, Armory, Loot, Pause, or Help overlays into the exploration/combat board beneath them.
- Raised close-map ambient prop visibility while preserving lower decoration density, so useful world art no longer fades to roughly invisible alpha.
- Expanded runtime smoke coverage through Quick Start, Midgaard NPC dialogue, Beta Lab combat, the full combat HUD, and the Spellbook modal. Save schema remains v18.

## v1.24.0 - Spell and Skill Art Pass

- Added a new 5x5 spellbook atlas with dedicated readable art for healing, summoning, fireball, meteor shower, shields, cleansing, regeneration, resurrection, elemental damage, death/mind magic, shaped terrain, and each formula school.
- Expanded the martial ability atlas from 12 shared cells to 20 cells. Shield Bash, Cleave, Throw Knife, Hamstring, Broadhead Shot, and Disrupting Shot now have distinct art, with reserved Enrage and Hunter Focus art available for future passive UI.
- Added a reusable atlas-normalization tool that finds irregular generated gutters, trims each icon, preserves padding, supports cell replacement, and emits a per-cell validation manifest.
- Added a border-connected chroma cleanup tool for generated sheets so violet spell art can survive key removal without repeating the earlier over-pruning problem.
- Exported both runtime atlases and editable source files through Aseprite. Combat rules and save schema remain unchanged at v18.

## v1.23.5 - Power Outcome Ledger

- Added a pure before/after combat outcome calculator for successful player formulas and martial abilities.
- Power banners now report actual post-mitigation damage and healing plus affected targets, defeats, status applications, cleansed ailments, bound summons, and changed terrain.
- Expanded the uGUI power banner with a dedicated outcome line, preserving the existing icon, sigil, actor/target context, and brief resolution beat without crowding them together.
- Wired the ledger through both board-targeted powers and instant Combat Skills while leaving ordinary attacks and enemy actions unchanged.
- Added rule-smoke coverage for multi-target damage, healing, defeat counting, statuses, cleansing, summons, terrain replacement, summary wording, and supported banner layouts. Combat math, AI, and save schema remain unchanged at v18.

## v1.23.4 - Power Resolution Beat

- Added a short, intensity-scaled resolution beat after successful player spells and combat skills so their beams, glyphs, particles, floating text, and impact callouts finish before initiative advances.
- Kept ordinary movement, attacks, Guard, Elixir, End Turn, and enemy actions immediate; this pacing change is limited to signature powers.
- Disabled board targeting, movement, command hotkeys, and action buttons while a power is resolving, with a clear phase label instead of allowing input to leak into the next turn.
- Added a 0.06-second Reduced Motion path and blocked save/load during the transient beat so a campaign cannot capture a half-resolved turn.
- Added rule-smoke coverage for all three impact tiers, Reduced Motion timing, and resolving-phase command rejection. Combat math, AI, and save schema remain unchanged at v18.

## v1.23.3 - Tactical Power Preview

- Added shared targeting-footprint profiles for formulas and martial abilities, covering single targets, cross areas, placed fields, summon tiles, charge landings, secondary strikes, and self-centered effects.
- Added impact-shape metadata to Spellbook and Combat Skills cards, so power selection explains both path and affected area before the player readies it.
- Added exact hover-only board previews for Charge's validated landing cell, Cleave's deterministic secondary victim, and Volley's five-tile cross plus arcing shot trace.
- Updated formula target badges to distinguish area, field, and binding placement from ordinary direct casting while preserving existing legality and line-of-sight rules.
- Added rule-smoke coverage for sewer-slice targeting profiles and showcase power footprints. Combat math, AI, and save schema remain unchanged at v18.
- Increased the packaging wrapper's Unity success-marker wait from 30 to 90 seconds, preventing a completed Windows build from being reported as failed when this editor installation flushes its log late.

## v1.23.2 - Combat Power Impact

- Added deterministic presentation profiles for formulas and martial abilities: title, sigil, school/class accent, impact intensity, duration, and concise actor/target context.
- Added icon-backed resolution callouts that persist over immediate next-turn banners, so successful spells and skills retain their identity long enough to read.
- Added a restrained intensity-scaled board-edge impact pulse for resolved powers; Reduced Motion uses a short static cue instead.
- Centralized top-left atlas-region conversion and sprite caching in `UiRuntime`, shared by the Spellbook/Combat Skills modal and impact callouts.
- Added rule-smoke coverage for starter, showcase, and epic power tiers plus every active sewer-slice formula and ability. Combat math and save schema remain unchanged at v18.

## v1.23.1 - Class Viability and Spellbook Identity

- Ensured every selectable class has at least two useful level-one powers in normal sewer-slice play: warriors gain Execute, rogues gain Stealth and Ambush, and warlocks gain Bind and Summon Imp alongside the existing starter kits.
- Centralized selectable-class spell schools and formula unlock levels in content catalogs so party setup, combat, and tests use the same progression rules.
- Wired existing formula, pact, ember, and martial ability atlases into the uGUI Spellbook and Combat Skills cards, with formula codes/skill sigils and class-school color accents.
- Kept the combat HUD visible as a dimmed, non-interactive underlay while Spellbook or Combat Skills owns focus.
- Expanded rule-smoke coverage for selectable-class starter viability, sewer-slice content, and modal art identity. Save schema remains v18.

## v1.23.0 - uGUI End-State Screen

- Added `EndStateScreen`, a uGUI Victory/Defeat screen with cleaner ledger, route summary, and action layout.
- Preserved legacy end-state behavior: Defeat offers New Party; Victory offers New Party, Tavern, and development-only Beta Lab.
- Added Victory/Defeat-specific F1 Help copy so end-state help no longer falls back to Tavern Help.
- Added rule-smoke coverage for end-state layout and state-specific content. Save schema remains v18.

## v1.22.0 - uGUI Help Overlay

- Added a first-class uGUI Help overlay opened with F1 and closed with F1, Esc, or Close.
- Replaced log-spam context help with concise mode-specific help pages for Tavern, Party Setup, Exploration, and Combat.
- Routed Help through the shared overlay stack so it blocks board clicks/hotkeys like Pause, Armory, Dialogue, Loot, and combat ability modals.
- Added rule-smoke coverage for Help layout, mode-specific Help content, and input blocking. Save schema remains v18.

## v1.21.1 - Headless Smoke Quit Hotfix

- Added a standalone-player batchmode quit guard so packaged smoke-test launches using `-batchmode -quit` request `Application.Quit()` after the boot marker instead of lingering in the background.
- Scoped the guard away from editor batchmode and normal player launches, so Unity editor tests and ordinary windowed play stay open as expected.
- Added rule-smoke coverage for the quit predicate. Save schema remains v18.

## v1.21.0 - Combat Turn-State Repair Extraction

- Moved active combat turn-state repair into `CombatController`, so loaded/recovered combat state now uses the same controller-owned movement/action availability rules as normal turn flow.
- Kept legacy combat drawing/orchestration responsible only for finding the active unit, then delegating phase, action, and move-point repair to the controller.
- Expanded rule-smoke coverage for repaired movement refill, movement clamping, spent-action preservation, moved-unit handling, and enemy-thinking phase selection. Save schema remains v18.

## v1.20.9 - Build and Package Hygiene

- Hardened `Tools/BuildAndPackageWindows.ps1` against Unity log-flush timing, so a successful build is no longer reported as failed just because the log appears a moment late.
- Trimmed player-package art references by excluding older reference-only and redundant fallback atlas prefixes while keeping preferred runtime atlases available to the loader.
- Updated the packaged art manifest wording to clarify that source, prompt, contact, and older fallback art remain in the development workspace. Save schema remains v18.

## v1.20.8 - Midgaard Start and Gate Traversal Hotfix

- Fixed fresh New Game/Quick Start placement so the party starts on a walkable interior Midgaard plaza tile instead of being repaired to a nearby outside-road component.
- Made Midgaard's east and west gates true pass-through tiles while keeping the sealed north/south gate behavior intact.
- Added runtime boot smoke coverage for fresh-start plaza placement and east/west gate passability. Save schema remains v18.

## v1.20.7 - uGUI Banner Toast Migration

- Added a passive uGUI `BannerToastScreen` for mode-change, save/load, context-help, combat, and exploration banner feedback.
- Routed `ShowBanner` state through the uGUI toast on all screens while keeping the old IMGUI banner only as a recovery fallback.
- Kept the banner canvas non-interactive so it cannot intercept clicks meant for board tiles, menus, or modal buttons.
- Added layout smoke coverage for the new banner toast at supported resolutions. Save schema remains v18.

## v1.20.6 - uGUI Migration Gap Repair

- Restored visible development-only `Beta Testing` access on the uGUI tavern screen; the combat, martial, and kobold route labs are no longer hotkey-only.
- Restored legacy banner feedback over uGUI Tavern and Party Setup screens.
- Aligned tavern hotkeys with the visible actions: Enter continues/begins, B begins, C/N customizes, S opens settings, and T toggles Beta Testing in development builds.
- Added `Docs/UGUI_MIGRATION_AUDIT_v1.20.6.md` to track remaining legacy/uGUI ownership gaps.
- Added layout smoke coverage for the dev-only tavern testing button. Save schema remains v18.

## v1.20.5 - Tavern Start Menu Rebuild

- Reworked the first-screen tavern menu into a clearer title-screen layout with title copy on the left and primary actions on the right.
- Added a direct `Begin the Old Road` action for immediate play, while `Customize Party` opens the muster editor.
- Renamed the old generic `New Game`/`Quit` flow to player-facing `Customize Party` and `Exit Game`.
- Generated and wired a new title-safe tavern backdrop as `tavern-backdrop-runtime-v1.20.5.png`.
- Updated tavern menu rule smoke tests for the revised action set. Save schema remains v18.

## v1.20.4 - Exploration Stuck-State Hardening

- Made keyboard movement into a blocking usable object resolve that exact object, matching adjacent mouse-click behavior for NPCs, caches, shrines, city fixtures, and other interactables.
- Added exploration reachability flood-fill helpers so start/load repair can distinguish a genuinely usable region from a technically open but isolated tile.
- Updated fresh-start and load-position repair to prefer reachable map components with useful interaction targets.
- Expanded runtime exploration smoke coverage for reachable start regions, reachable useful targets, and keyboard bump-use resolution.
- Save schema remains v18.

## v1.20.3 - Exploration Control Stabilization

- Made exploration keyboard movement consume only one cardinal movement input per frame, preventing stacked same-frame moves when multiple movement keys are pressed together.
- Replaced the generic blocked-move "Stone blocks the way" feedback with context-aware messages for map bounds, terrain, blocking objects, and adjacent interactables.
- Added runtime exploration smoke coverage for exact one-tile movement, blocked movement staying in place, and object-block feedback that advertises Space/E contextual use.
- Save schema remains v18.

## v1.20.2 - Exploration Self-Test Hotfix

- Added a runtime world-map exploration self-test that boots the real scene, Quick Starts into Explore, probes the generated map, attempts blocked movement into an interactable, checks look/use text, and uses the target from an adjacent tile.
- Fixed a beta-test finding where fresh Quick Start exploration began on Midgaard's central Market landmark, which became non-standable after the v1.20.1 anti-overlap rules.
- Fresh exploration starts, lab map starts, descents, and repaired loaded exploration saves now relocate the party to a nearby standable tile when the map start tile is occupied by a blocking landmark.
- Updated open-neighbor selection to allow explicitly standable targets while rejecting blocking markers, keeping stairs/recall-style targets usable without letting NPCs or fixtures overlap the party.
- Save schema remains v18.

## v1.20.1 - World Map Collision and Interactions

- Added shared exploration traversal rules so NPCs, caches, camps, shrines, buildings, gates, and most map fixtures block party overlap instead of letting the party stand on top of their art.
- Updated Space/E and adjacent mouse clicks to resolve usable nearby objects directly, so the party can talk to NPCs, open caches, use shrines, and work city fixtures from beside the sprite.
- Kept true stand-on targets like stairs and recall circles intact so descent/anchor behavior remains familiar.
- Routed movement hints and hover text through the same traversal rules, so blocked object tiles no longer advertise themselves as walkable.
- Added rule-smoke coverage for adjacent object use, NPC collision, cache collision, city-wall blocking, and stairs behavior. Save schema remains v18.

## v1.20.0 - World Map Readability Pass

- Added shared exploration readability rules so terrain alpha, decorative density, and focus-tile calmness are intentional and smoke-tested.
- Lowered close-map passable terrain art opacity, especially in Midgaard, so sprites and interactable objects sit above the floor instead of blending into it.
- Reduced close-map decorative prop density and alpha while keeping wide-map context richer.
- Made party/object tiles prefer calmer terrain variants underneath the player and interactables.
- Strengthened party-token contrast pads and object plinths so the player marker, NPCs, caches, shrines, camps, and route objects are easier to find at a glance.
- Added rule-smoke coverage for the readability contract. Save schema remains v18.

## v1.19.12 - Presentation Declutter and Controller Caching

- Removed the duplicate legacy combat turn queue and active-unit readout from the board path now that the uGUI Combat HUD owns turn order, active unit, target, and command state.
- Simplified the combat hover card so it shows tactical action outcome only, leaving target details to the Combat HUD.
- Made exploration Location Details reserve real map-board width when open, so the panel no longer covers clickable map cells while leaving them active.
- Cached the runtime ExplorationController by active GameState/MapData and the CombatController by active GameState/CombatState, with explicit invalidation on new game, map generation, load, combat start, and combat end.
- Added rule-smoke coverage for Details board reservation and controller-cache construction paths. Save schema remains v18.

## v1.19.11 - Sewer Slice First-Play Focus

- Made normal first-play Journal rows focus only on the Midgaard sewer contract, room progress, proof, reward, and post-reward Old Road teaser.
- Kept six-chapter scaffold rows, Midgaard errands, broad contacts, kobold route milestones, and route-scaffold rows available only in the full-prototype/developer content set.
- Disabled prototype route triggers, random route encounters, kobold ambush/cave progression, and route-scaffold actions for normal sewer-slice play.
- Tightened the sewer reward flow so each authored sewer room grants one proof bundle, three proof bundles claim the rat-pelt armor once, and the reward unlocks only the Old Road teaser.
- Added rule-smoke coverage for the full contract acceptance, three-room progress, proof count, idempotent reward claim, and prototype trigger gating. Save schema remains v18.

## v1.19.10 - Gameplay Pause Menu

- Removed the persistent Save, Load, and New buttons from exploration and combat HUD chrome.
- Added a shared pause/menu overlay for Continue, Save, Load, Settings, Return to Tavern, and New Game.
- Added confirmation clicks for Return to Tavern and New Game during an active run so a single stray click cannot wipe the current session.
- Simplified the exploration command bar to contextual Interact plus Menu, while keeping Party, Journal, map width, and details on their keyboard/side-panel paths.
- Kept combat to four primary commands plus one Utility entry; Utility now also opens the shared menu.
- Added layout and input-rule smoke coverage for the pause menu. Save schema remains v18.

## v1.19.9 - Overlay Input Ownership

- Added a shared `UiOverlay` focus rule for Armory, Dialogue, Loot, and combat spell/skill modals.
- Routed Escape through one top-overlay close path before normal gameplay handling.
- Routed exploration and combat board clicks through the same overlay-aware input rule so modal clicks cannot leak to movement or targeting.
- Made major mode changes clear incompatible transient overlays, and made newly opened overlays dismiss conflicting gameplay overlays.
- Added rule-smoke coverage for overlay blocking of keyboard gameplay and board pointer routing. Save schema remains v18.

## v1.19.8 - UI Refresh Stabilization

- Reduced per-frame uGUI churn by syncing only the current major screen and currently visible overlays.
- Added lightweight refresh keys so Tavern, Party Setup, Exploration HUD, Combat HUD, Armory, Dialogue, Loot, and combat ability modals refresh only when their visible state changes.
- Cached Tavern save-existence checks and invalidated the cache after successful save/load events instead of checking the filesystem every visible Tavern frame.
- Replaced recurring Exploration and Combat HUD LINQ list builders with reusable row buffers for party, command, and log views.
- Added rule-smoke coverage for stable presentation refresh keys. Save schema remains v18.

## v1.19.7 - Beta Launch Stability

- Added startup log breadcrumbs for boot start and boot completion so future player logs show whether the game reached the tavern/muster initialization path.
- Documented `-force-d3d11` as a troubleshooting option for rare launch/capture issues, while leaving normal Windows builds on Unity's default graphics backend.
- Kept save schema at v18; this is a launch/build stability patch, not a save migration.

## v1.19.6 - Content-Set Save Stabilization

- Bumped save data to version 18 and added serialized `ContentSetId` so loading restores the same campaign/content rules that created the save.
- Fresh normal games now store the `sewer-slice` content set, while developer labs run with `full-prototype` in memory.
- Blocked developer-lab saves from writing over the normal campaign save slot; lab runs now report that they are not saved.
- Migrated older v17 saves conservatively to `full-prototype` rules so broad prototype campaigns keep their older formula, enemy, and route access.
- Repaired unknown content-set IDs safely back to `sewer-slice` with a warning log note.
- Closed transient overlays before applying loaded state so Armory, dialogue, loot, spellbook, and ability popups cannot remain open across load.

## v1.19.5 - Armory Overlay Migration

- Migrated the Party/Pack/Spells/Journal overlay to the newer uGUI screen path so it no longer depends on the older IMGUI modal.
- Kept Pack `Try Equip` behavior wired through the existing conservative auto-equip check.
- Simplified Journal presentation into readable route/story rows while preserving Midgaard errands, chapter progress, contacts, and scaffold status.
- Added layout smoke coverage for the Armory overlay at 1280x720, 1600x900, 1920x1080, and 2048x1152.
- Kept save data at version 17 because this is UI/presentation only.

## v1.19.4 - Exploration Tile Art Overhaul

- Added `world-map-exploration-tile-atlas-runtime-v1.19.4.png`, a new deterministic runtime terrain sheet with 35 cells for roads, paving, moss, mire, quarry, glass rubble, ash basalt, water, cliffs, bridges, steps, and expanded forest-wall variants.
- Expanded forest-wall rendering to use the new variant cells instead of falling back to repeated old canopy tiles.
- Stopped drawing the large procedural tree/road motifs over successful world-map atlas tiles; those now become small accent marks only.
- Increased non-Midgaard terrain atlas strength so the new sheet reads clearly while keeping Midgaard floors quieter behind NPCs and the party token.
- Kept save data at version 17 because this is art/rendering only.

## v1.19.3 - Exploration Use Targeting

- Added a stronger on-map cue for the exact nearby target that Space/E will use.
- Added a compact Use button to the exploration command bar so mouse players can trigger the same nearby-use action without guessing.
- Kept the cue small and edge-based so it does not cover sprites, NPCs, gates, or quest art.
- Kept save data at version 17 because this is input/UI presentation only.

## v1.19.2 - Exploration Use Shortcut

- Added Space/E as a nearby-use shortcut in exploration.
- The shortcut uses the current tile first, then the best adjacent usable object: objectives, stairs, caves, encounters, caches, shrines, NPCs, shops, gates, route markers, and town services.
- The map strip and Location panel now show the current Space/E action when a usable target is nearby.
- Stairs underfoot descend directly through Space/E; adjacent targets still use the normal step-onto-tile path so existing quest/combat behavior remains centralized.
- Kept save data at version 17 because this is input/UI behavior only.

## v1.19.1 - Exploration Readability Tuning

- Reduced Local Map terrain-art opacity so floor textures stay atmospheric without swallowing sprites and landmarks.
- Reduced decorative biome prop density/opacity, especially in Local Map, so ambient art stops competing with NPCs, gates, stairs, and route objects.
- Softened movement-hint overlays and object-adjacent borders while keeping blocked/passable feedback visible.
- Strengthened the party token's backing and plinth so the player marker separates more cleanly from busy Midgaard and road tiles.
- Kept save data at version 17 because this is presentation-only.

## v1.19.0 - Centered Tavern Opening

- Added a new generated `tavern-backdrop-runtime-v1.19.0.png` opening backdrop with warm tavern art, an old-road doorway, and calmer center space for the menu.
- Rebuilt the tavern landing as a centered title/menu stack instead of a left-side debug-like panel.
- Removed the normal first-screen party preview so the opening page feels like a special entry screen rather than another roster view.
- Softened the tavern backdrop overlay so the new art remains visible while the centered UI stays readable.
- Kept save data at version 17 because this is presentation-only.

## v1.18.4 - Exploration Locator Cues

- Added a stronger non-text party locator on the exploration map with corner brackets, a foot marker, and a subtle inner outline.
- Added small corner cues and pips for adjacent interactable objects, current Midgaard work, route scaffolds, stairs, caves, shrines, caches, and encounters.
- Reduced movement-hint opacity so valid step indicators help navigation without competing with sprites and object art.
- Kept save data at version 17 because this is presentation-only.

## v1.18.3 - Exploration Readability Pass

- Kept Local Map as a true 11x7 close view instead of expanding it on large monitors, preserving larger tiles for travel and NPC/object readability.
- Renamed Close/Wide map labels to Local Map and Region Map for clearer intent.
- Reduced floor-tile and decorative prop opacity in exploration so party tokens, NPCs, gates, and quest objects stand out against Midgaard and road textures.
- Enlarged the party world-map token footprint and strengthened its dark backing so the player marker is easier to find.
- Kept save data at version 17 because this is presentation-only.

## v1.18.2 - Aseprite Runtime Art Workflow

- Located and used the Steam Aseprite install at `C:\Program Files (x86)\Steam\steamapps\common\Aseprite\Aseprite.exe` for the map-art export pass.
- Added `Tools/ExportRuntimeArtWithAseprite.ps1`, a repeatable helper that exports art through Steam Aseprite's native batch path.
- Added native `.aseprite` source files for the biome prop, Midgaard gate, and Midgaard wall atlases under `Docs/ArtReferences/source-*`.
- Re-exported `world-map-biome-prop-atlas-runtime-v1.18.2.png`, `midgaard-gate-atlas-runtime-v1.18.2.png`, and `midgaard-wall-atlas-runtime-v1.18.2.png` through Aseprite.
- Fixed atlas alpha validation to sample top-left authored cells correctly, matching the renderer and atlas guides.
- Kept `.aseprite` sources out of player packages and reduced packaged art to the latest runtime PNG per prefix.
- Kept save data at version 17 because this is tooling/art pipeline work only.

## v1.18.1 - Valid World Map Placeholder Atlases

- Added runtime placeholder art for `world-map-biome-prop-atlas-runtime-v1.18.1.png`, giving forest, mire, quarry, ash, road, and paved regions small transparent ambient props through the new v1.18 biome-prop hook.
- Added `midgaard-gate-atlas-runtime-v1.18.1.png` with transparent north, south, east, and west gate placeholders so the map no longer needs opaque boxed gate sheets.
- Added `midgaard-wall-atlas-runtime-v1.18.1.png` as a true 5x4 square-cell wall sheet, removing the non-square-cell warning from the active runtime wall atlas.
- Added `world-map-biome-prop-atlas-runtime-` to the packaged runtime art whitelist so shared builds keep the new biome prop sheet.
- Kept save data at version 17 because this is art/runtime packaging only.

## v1.18.0 - World Map Aseprite Contracts

- Added a dedicated `world-map-biome-prop-atlas-runtime-*` loader path for transparent 5x4 Aseprite sheets containing small biome props such as trees, reeds, cairns, bones, lanterns, and grass tufts.
- Tightened Midgaard wall validation so 5x4 wall atlases must have square cells instead of a square whole-sheet shape.
- Changed Midgaard gate loading to require transparent cutout art and tightened gate trimming validation so opaque dark panels are rejected instead of drawn onto the map.
- Removed the misleading east/west gate fallback that reused north/south front-gate cells, and added forward-compatible forest-wall variant selection for future expanded tile atlases.
- Staged the v1.18.0 Aseprite action plan, blank biome-prop template, labeled guide, and wall/gate/tile contact sheets as reference-only docs.
- Kept save data at version 17 because this is renderer/art-pipeline work only.

## v1.17.0 - Persistence Service Extraction

- Added `Assets/Scripts/Persistence/SaveService.cs` to own save-path construction, JSON serialization, atomic writes, primary-save checks, and backup fallback reads.
- Kept `AshenHallsGame.SaveGame()` and `LoadGame()` as gameplay/UI orchestration wrappers for repair, validation, logging, banners, and sound.
- Added a build-gated rule smoke test that verifies `SaveService` writes a primary save and falls back to a `.bak` file when the primary is unavailable.
- Kept save data at version 17 because the save file name, JSON structure, and repair path are unchanged.

## v1.16.0 - Runtime Model Extraction

- Extracted save/runtime model types from `AshenHallsGame.cs` into `Assets/Scripts/Domain/RuntimeModels.cs`, including `GameState`, party/stat/item records, map/combat records, and lightweight VFX DTOs.
- Reduced the main MonoBehaviour to focus more on orchestration/rendering while preserving existing public type names, namespaces, constructors, and field names for save compatibility.
- Added Unity metadata for the new runtime model source file.
- Kept save data at version 17 because this is a source-layout change over the same serialized model.

## v1.15.0 - Domain Type Extraction

- Extracted core game enums from `AshenHallsGame.cs` into `Assets/Scripts/Domain/GameEnums.cs`.
- Extracted `EnemyTemplate` and `FormulaDef` into `Assets/Scripts/Content/ContentDefinitions.cs`, reducing coupling between the content catalogs and the main MonoBehaviour.
- Extracted `ColorExtensions` into `Assets/Scripts/Presentation/ColorExtensions.cs`.
- Added Unity metadata for the new Domain and Presentation source folders so source packages remain stable before editor import.
- Kept save data at version 17 because type names, namespaces, and serialized field names are unchanged.

## v1.14.0 - World Map Art Contracts

- Added type-specific world-map art scale/pivot contracts so Midgaard buildings, gates, NPCs, sewer objects, landmarks, and small props no longer all use one generic sprite-fit rule.
- Reduced the party token's map footprint by removing its negative atlas padding and anchoring it through the same trimmed-art path used by other world-map tokens.
- Added an off-by-default F8 exploration art debug overlay that shows tile bounds, object render bounds, and object labels for centering/cutout QA without cluttering normal play.
- Added startup validation for key world-map, Midgaard town, gate, wall, NPC, prop, and token atlas cells so empty/over-pruned critical cells surface in logs.
- Kept save data at version 17 because this is presentation and art-contract hardening only.

## v1.13.0 - World Map Readability Pass

- Added trimmed alpha-bound drawing for world-map token, prop, landmark, Midgaard town, gate, NPC, city-prop, and sewer atlas cells so sprites center around their visible pixels instead of raw atlas-cell margins.
- Added a first-class Midgaard gate orientation path for north, south, east, and west gates, removing competing gate fallbacks from town and wall object mappings.
- Added a low-density biome ambient prop pass for forest/moss, mire, mud, quarry, glass, ash/red, road, and paved regions, with procedural fallbacks when atlas art is unavailable.
- Tightened explore-mode board margins and shortened the exploration command strip so the map grid owns more of the window without overlapping controls.
- Kept save data at version 17 because this is presentation/layout/art-pipeline work, not a save migration.

## v1.12.0 - Combat Rule Smoke Tests

- Added `RuleSmokeTests`, a no-package-manager editor test harness for extracted combat rule seams.
- Covered `AttackRules` damage profiles for skill, hex, stealth, physical enrage, default physical damage type, and non-physical enrage exclusion.
- Covered `CombatGridRules` movement blockers, terrain step costs, supercover diagonal/corner cells, missile line-of-sight blockers, and non-missile arc bypass.
- Wired the smoke tests into the Windows build path through `RuleSmokeTests.RunOrThrow()` so package builds fail before staging if these core rule checks regress.
- Kept save data at version 17 because this is test/build safety scaffolding, not a save migration.

## v1.11.0 - Runtime Art Package Trim

- Tightened Windows packaging so `Docs/ArtReferences` only ships PNGs whose prefixes are actively queried by the runtime art loader.
- Kept the latest two packageable versions per art prefix, giving alpha/visibility-gated atlases one fallback without shipping every historical sheet.
- Added a generated `Docs/PACKAGED_ART.txt` manifest to the staged player build so package contents can be audited without unpacking source/history art.
- Continued excluding prompt, source, and contact/provenance art from player packages while leaving those files available in the source workspace.
- Kept save data at version 17 because this is packaging/build hygiene, not a save migration.

## v1.10.0 - Attack Rule Extraction

- Added `AttackRules` and `AttackDamageProfile` as the next combat-rules extraction seam for normal weapon attack damage.
- Rewired attack previews and actual attack resolution to share the same base damage, skill, hex, enrage, stealth, defense, armor, and damage-type calculation.
- Kept the canonical trait reduction path unchanged: guard, shields, gear reduction, race reduction, resistance, and weakness still apply through `PreviewDamageAfterTraits()` and `DealDamage()`.
- Kept save data at version 17 because this is a source organization and rule-parity pass, not a save migration.

## v1.9.0 - Combat Grid Rules Extraction

- Added `CombatGridRules` as the first combat-rules extraction seam for path-aware reachable movement costs, supercover line tracing, line-of-sight checks, and Manhattan distance.
- Rewired `AshenHallsGame` movement, LOS, and distance wrappers to call the extracted helper while preserving existing combat call paths.
- Kept combat behavior intentionally stable: terrain costs, tree/stone blocking, unit blocking, ranged LOS, and spell arc decisions still use the same wrapper methods.
- Kept save data at version 17 because this is source organization for combat rules, not a save migration.

## v1.8.0 - Content Catalog Extraction

- Moved the formula/spell list into `FormulaCatalog`, preserving all current codes, names, mana costs, ranges, schools, and effects.
- Moved warrior, rogue, and ranger skill definitions into `AbilityCatalog` with a standalone `MartialAbility` data class.
- Moved enemy template IDs and stat rows into `EnemyCatalog` while keeping `EnemyTemplate.For()` as a compatibility wrapper for existing combat generation.
- Kept the v1.7.0 startup catalog validation active so the extracted catalogs are checked on launch.
- Kept save data at version 17 because this is source organization and validation scaffolding, not a save migration.

## v1.7.0 - Content Validation and Story Flag Scaffolding

- Added `StoryFlags` constants for recurring Midgaard and Kobold progression flags while preserving the existing saved string values.
- Added startup validation for formula, martial ability, and enemy template catalogs so duplicate IDs, invalid targets/effects, bad ranges, and incomplete rows are reported early.
- Kept save data at version 17 because this is a source safety pass, not a save migration.

## v1.6.3 - Source Audit Fixes and Midgaard Readability

- Added shared `VersionInfo` constants so the runtime and editor build use the same package version, save version, and beta stage text.
- Updated the packaging script to derive its default version from `VersionInfo.cs`, validate explicit version arguments, and fail when release docs do not mention the active version.
- Fixed the Journal hotkey/button path so `J` and the exploration Journal button open the Journal tab instead of clamping to Spells.
- Removed the unused pre-spellbook `LegacyCastSpell` fallback.
- Made save writes use a temp file plus backup, and let load try the backup if the primary save is unreadable.
- Stopped the Windows build script from recreating and saving `Assets/Scenes/Main.unity` during every build.
- Added `midgaard-tile-atlas-runtime-v1.6.3.png` and reduced exploration floor/ambient prop intensity so party, NPC, and object sprites read more clearly.
- Kept save data at version 17 because this is source/build hardening plus presentation work over existing fields.

## v1.6.2 - Combat Skill Icon Refresh

- Generated and wired `ability-icon-atlas-runtime-v1.6.2.png` as the active transparent 4x3 Combat Skills icon sheet.
- Replaced the older skill icons with clearer warrior, rogue, and ranger symbols for Charge, Execute, Rally/Shield Bash, Whirlwind/Cleave, Stealth, Ambush/Throw Knife, Smoke Bomb, Eviscerate/Hamstring, Aimed Shot, Pinning/Broadhead Shot, Volley, and Scout Mark/Disrupting Shot.
- Validated all 12 icon cells for visible-pixel coverage and centering after chroma-key removal.
- Kept save data at version 17 because this is presentation/art work only.

## v1.6.1 - Weapon and Ability Stat Scaling

- Updated derived weapon damage so bows and ranged weapons scale from AGI, magical focuses/staves/orbs scale from INT, heavy melee scales from STR, and finesse/reach weapons use hybrid stat logic.
- Added small class-appropriate stat bonuses to martial/ranger/rogue ability damage: warrior skills reward STR, ranger shots reward AGI, and rogue cuts reward finesse.
- Updated attack, ability, and Armory gear summaries to show the active scaling stat so testers can see why a build is getting stronger.
- Kept save data at version 17 because these are recalculated derived stats and combat formulas, not new persisted fields.

## v1.6.0 - World Map UI Art Audit

- Audited active world-map, Midgaard, combat command, tavern, and UI atlas families for per-cell visible coverage.
- Generated and wired `world-map-ui-atlas-runtime-v1.6.0.png` as the active transparent 5x4 exploration command atlas.
- Replaced the stale travel-rail metaphor with a dedicated `Location Details` icon while keeping a separate location-pin cell for map markers.
- Kept save data at version 17 because this is presentation/UI/art work only.

## v1.5.9 - Tavern UI Icons, Map Readability, and Combat Sprite Declutter

- Generated and wired `tavern-ui-atlas-runtime-v1.5.9.png` as the active transparent 5x4 tavern/menu icon sheet.
- Reworked the tavern landing layout with a larger title plate, version text inside the title plate, and a cleaner first-screen action stack.
- Moved Beta Lab, Martial Lab, and Kobolds into a `Beta Testing` panel so the normal first screen reads less like a debug dashboard.
- Removed the remaining tiny party sigil from combat board sprites and moved combat status pips to a small edge rail so sprite bodies stay unobstructed.
- Replaced the old Travel rail wording/flow with an optional `Location Details` drawer while keeping exploration on one map experience.
- Increased exploration readability by making Close Map use larger 11x7 tiles, reducing world-sprite padding, and keeping the compact map chrome/command strip stable whether details are open or hidden.
- Kept save data at version 17 because this is presentation/UI/art work only.

## v1.5.8 - Art Audit, Spell Power, and Tavern UI Tuning

- Audited active runtime art families for alpha/visible-pixel coverage; the v1.5.3 world-map replacement sheets remain usable, while the new v1.5.8 spell sheet passes per-cell coverage checks.
- Generated and wired `combat-terrain-atlas-runtime-v1.5.8.png`, preserving the v0.80 terrain atlas while replacing the generic fire cell with a darker burning-stone hazard tile.
- Generated and wired `spell-animation-atlas-runtime-v1.5.8.png` as a transparent 4x4 sheet for fireball, meteor, death, lightning, ice, healing, ward, and summoning effects.
- Made INT matter more directly for casters: higher INT now increases formula damage, healing, summon strength/HP, and hostile status reliability, with previews calling out the stat bonus.
- Cleaned the tavern landing layout so the title plate, primary start action, tester doors, road brief, and party preview have clearer visual hierarchy.
- Converted Midgaard armorer, weapon vendor, and enchanter fallback branches into real dialogue popups instead of log-only interactions.
- Kept save data at version 17 because this is art/UI presentation plus derived combat-stat tuning only.

## v1.5.7 - World Map UI Art Pass

- Generated `world-map-ui-atlas-runtime-v1.5.7.png` as a dedicated transparent 5x4 exploration icon atlas.
- Wired exploration command buttons to atlas cells for camp, recall, descend, elixir, map view, travel focus, journal, and armory actions, with procedural icons kept as fallback.
- Added alpha and per-cell visibility validation for world-map UI icons so bad future sheets fall back instead of drawing broken cells.
- Kept save data at version 17 because this is art/UI presentation work only.

## v1.5.6 - Combat Action Flow Hardening

- Centralized the end-of-action policy used by player actions, enemy turns, and skipped stun/sleep turns so action spending, optional movement spending, modal cleanup, summon binding ticks, party sync, and turn advancement share one path.
- Kept existing action behavior intact for attacks, spells, abilities, guard, elixirs, wait, enemy actions, and lost turns while reducing drift between those paths.
- Kept save data at version 17 because this is combat-flow hardening over existing state fields.

## v1.5.5 - Exploration Atlas Cell Hardening

- Added per-cell visibility validation for vulnerable exploration atlases so one over-pruned or fully opaque cell can fall back instead of breaking a map token, NPC, landmark, or Midgaard object.
- Added atlas-cell range checks for world-map token, prop, landmark, Midgaard town-object, Midgaard prop, and Midgaard NPC draw paths so bad indices no longer silently clamp to the wrong sprite.
- Kept the v1.5.3 regenerated art as the active runtime set; this patch hardens how those sheets are consumed.
- Kept save data at version 17 because this is presentation/runtime-loading hardening only.

## v1.5.4 - Exploration Atlas Contract Hotfix

- Fixed regenerated world-map token atlas indexing so ranger, rogue, priest, mage, warlock, paladin, and spear-warrior roles draw the intended sprites.
- Fixed regenerated Midgaard town-object mappings so tavern, armorer, provisions, diner, and guard fallbacks no longer point at the wrong cells.
- Fixed regenerated Midgaard NPC mappings so tavern keeper, gate captain, royal herald, novice healer, and old-road scout draw the intended NPC sprites.
- Kept save data at version 17 because this is a presentation/mapping hotfix only.

## v1.5.3 - World Map Art Regeneration

- Regenerated the over-pruned world-map token, world-map prop, world-map landmark, Midgaard town, Midgaard prop, and Midgaard NPC runtime atlases from new chroma-key sheets.
- Replaced the broken v0.93-style cutouts where interior dark sprite pixels had been removed, making the party marker, NPCs, landmarks, and city props read as complete sprites again.
- Added visible-pixel validation to vulnerable exploration atlas loading so future transparent sheets that are too aggressively pruned can be skipped with a clear player-log warning.
- Kept save data at version 17 because this is an art/runtime-loading patch only.

## v1.5.2 - Combat Sprite Overlay Cleanup

- Removed party role/mana rails, weapon notches, and caster crosses from normal atlas-drawn combat sprites.
- Simplified combat sprite frames to an outer frame, shadow/pedestal, corner trim, and active outer pulse/foot glow.
- Atlas party sprites now keep only a tiny corner sigil for identity; HP bars, status pips, hover cards, and side panels remain the normal tactical readout.
- Kept the fuller procedural accents only for fallback non-atlas party figures.
- Kept save data at version 17 because this is presentation-only.

## v1.5.1 - Combat Audit Hardening

- Added defensive v17 combat-save repair for active summons with missing summoner IDs, then reapplies pact burden so older saves cannot bypass summon limits.
- Pact summons now fade when their original summoner is no longer alive instead of silently persisting or transferring ownership.
- Added a small `next` marker in the turn strip before previewed next-round units, clarifying why newly summoned units wait for the rebuilt initiative round.
- Replaced rounded-interpolation line of sight with grid supercover LOS for more consistent tree/stone blocking.
- Reduced intrusive combat sprite frame bars and party accent rails so generated character art remains the visual focus.
- Quarantined the legacy pre-spellbook `CastSpell` fallback as `LegacyCastSpell`.
- Build packaging now filters ArtReferences source/prompt/contact files while keeping runtime art, and the PowerShell zip path writes to a temp archive before replacing the final zip.
- Kept save data at version 17 because these are repair/rules/presentation changes over existing fields.

## v1.5.0 - Pact Summon Control

- Added pact burden limits so warlocks cannot flood the board indefinitely: Bound Imp costs 1 burden, Lesser Demon costs 2, and Greater Demon costs 3.
- Higher hex skill and warlock level now raise the active pact burden cap, making summon-heavy builds scale without becoming unlimited.
- Summoned demons now lose binding duration at the end of their own turns instead of before acting, so previewed durations match playable turns more closely.
- Combat readouts and spell previews now show summon HP, claw damage, remaining turns, and pact burden.
- Kept save data at version 17 because the new summon rules derive from existing combat fields.

## v1.4.0 - Warlock Demon Summons and Pact Art

- Reworked pact summoning into a clear warlock ladder: Summon Imp, Summon Lesser Demon, and Summon Greater Demon.
- Made Summon Imp cheaper and available earlier, Lesser Demon a mid-tier stronger body, and Greater Demon a costly elder summon with more HP, defense, damage, resistance, and a stronger on-hit effect.
- Warlock Spellbooks now default to Summon Imp when pact magic is available, while still retaining hex/death control spells.
- Generated and wired `demon-summon-atlas-runtime-v1.4.0.png` for distinct imp, lesser demon, greater demon, portal, sigil, and binding-chain art.
- Generated and wired `pact-spellbook-atlas-runtime-v1.4.0.png` for pact summon and ritual icons in the Spellbook.
- Kept save data at version 17 because formulas, runtime art, and summon stat rules derive from existing fields.

## v1.3.0 - Stable Combat Initiative

- Added a stable per-round initiative queue to combat state so the turn order no longer gets recomputed every time `NextTurn()` advances.
- Updated the turn strip to read from the same queue used by turn advancement, reducing drift between the UI and actual combat flow.
- Dead units are skipped inside the current round, while newly summoned pact allies join on the next rebuilt round instead of reshuffling the current one.
- Added combat save repair for missing/stale initiative queues and missing active units so older v17 saves can resume combat more reliably.
- Kept save data at version 17 because the queue can be rebuilt from existing combat units if absent.

## v1.2.0 - Skills and Spells Expansion

- Added warrior skills: Shield Bash for close stun control and Cleave for a heavy strike that can clip a second nearby enemy.
- Added rogue skills: Throw Knife for modest short-range pressure and Hamstring for a melee bleed/pin setup.
- Added ranger skills: Broadhead Shot for bleed pressure and Disrupting Shot for interrupting dangerous enemies, especially casters.
- Added new formulas: Still Water, Sun Brand, Chain Lightning, Frost Bind, Wither, Dream Smoke, Pact Brand, and the pact summon line later refined in v1.4.0.
- Made warlocks use `hex` as their role and `hex|pact` as their spell-school list, so pact spells no longer corrupt warlock gear/role setup.
- Enlarged the Combat Skills popup so the expanded class decks remain readable at smaller window heights.
- Kept save data at version 17 because this pass changes rules/content but does not change the save schema.

## v1.1.0 - Ranger Shooting Pass

- Made rangers/bow-role party members keep a guaranteed long-range attack profile even if older saves or generated gear leave their stored range too low.
- Changed the combat attack button to say Shoot for free ranged units and Melee when a ranged unit is engaged by an adjacent enemy.
- Normal attacks now choose missile skill only for actual ranged shots; engaged rangers fall back to arms/melee until they move away.
- Updated attack highlights, hover previews, cover attacks, enemy attack checks, and shot line-of-sight validation to use the same effective attack-mode rule.
- Kept save data at version 17 because this pass changes combat rules/UI behavior without changing saved data shape.

## v1.0.0 - Adaptive Map-Focus Camera

- Added adaptive viewport sizing for Map Focus exploration so larger windows show more world tiles instead of only magnifying the fixed 13x7 camera.
- Kept 1280x720 conservative with the existing 13x7 Close Map and 19x11 Wide Map for readability.
- Scales focused Close Map to 15x9 or 17x9 on larger windows, and focused Wide Map to 21x11 or 23x13.
- Updated the View toggle log text to report the current visible tile dimensions.
- Kept the Travel-rail-open view fixed, so detailed panel mode remains predictable.
- Added `Tools/BuildAndPackageWindows.ps1` as the reliable Windows packaging path: Unity stages the player, then PowerShell zips the completed folder after Unity exits.
- Kept save data at version 17 because this pass changes runtime presentation only.

## v0.99.0 - Compact Map-Focus Top Chrome

- Added a compact top chrome for Map Focus exploration, replacing the tall title/resource bar with a shorter route header.
- Moved the exploration board upward while focused, giving the map more vertical space without removing resources or Save/Load/New.
- Added compact resource boxes for Gold, Supplies, and Elixirs in the focused header.
- Kept the full top chrome for combat, defeat/victory, and exploration with the Travel rail open.
- Kept save data at version 17 because this pass changes runtime UI layout only.

## v0.98.0 - Compact Map-Focus Command Strip

- Changed Map Focus exploration to use a shorter compact command strip instead of the taller full action bar.
- Let the exploration board reclaim the saved vertical space while focused, increasing the visible map area without hiding core controls.
- Kept the larger labeled exploration controls when the Travel rail is open, preserving readability for detailed navigation and testing.
- Added compact key hints for Camp, Recall, Descend, Elixir, View, Travel, Journal, and Armory.
- Kept save data at version 17 because this pass changes runtime UI layout only.

## v0.97.0 - Map Focus Default and Top-Strip Look Readout

- Made new exploration sessions start in Map Focus by default, so the world map is the first visual priority after leaving the tavern.
- Added a focused-mode look readout to the top map strip: hovering visible tiles now reports the tile/object context there while the Travel rail is collapsed.
- Reworked the top map strip widths so region, danger, look/underfoot, view mode, and Travel toggle hints fit more predictably across wide and standard windows.
- Reset Close/Wide view to Close Map for fresh starts and route labs, keeping the first exploration view large and local.
- Kept save data at version 17 because this pass changes runtime UI defaults and presentation only.

## v0.96.0 - Exploration Map Focus Toggle

- Added a non-save Map Focus mode for exploration: press Q or use the Focus/Travel command button to collapse or restore the floating Travel rail.
- Added a compact Travel Rail tab while focused, so the map can occupy the screen without permanently losing access to Party, Location, and Latest panels.
- Expanded the exploration action budget to include both the map View toggle and the new Travel HUD toggle without removing Camp, Recall, Descend, Elixir, Journal, or Armory.
- Kept overlay click guards active in both full and collapsed rail states, preventing hidden-map movement through HUD controls.
- Kept save data at version 17 because this pass changes runtime UI state only.

## v0.95.0 - Map-First Exploration Layout

- Changed Explore mode to reserve nearly the full window for the world map instead of permanently subtracting a wide right column from the board.
- Tightened Close Map to a 13x7 local camera and Wide Map to 19x11, increasing tile and sprite readability while preserving a context toggle.
- Replaced the full-height exploration Party/Location/Timeline stack with a compact floating Travel rail: 2x2 Party summary, Location panel, and short Latest log.
- Expanded the exploration command bar to full window width and kept it below the board, so map actions have more readable button space.
- Added map-input guards for the floating Travel rail, preventing clicks over the overlay from moving the party on hidden tiles underneath.
- Kept save data at version 17 because this pass changes layout and presentation only.

## v0.94.0 - Map Art Loader Hardening

- Added alpha-gated loading for exploration sprite/object atlas families, so future mostly opaque token, NPC, landmark, town-object, or prop sheets are skipped instead of replacing readable cutout art.
- Reused the same transparency measurement for startup validation warnings, making map-art problems easier to diagnose from logs.
- Gave Location-panel hover text a distinct readout treatment after v0.93 moved hover details off the map board.
- Hid audio/motion preference controls from the in-game top chrome during Explore and Combat; settings/hotkeys remain the intended place for those controls.
- Kept save data at version 17 because this pass changes runtime art selection and UI presentation only.

## v0.93.0 - World Map Readability and Exploration UI

- Added cleaned v0.93 transparent runtime atlases for world-map party tokens, props, landmarks, Midgaard NPCs, Midgaard town objects, and Midgaard city props; original source/runtime art is preserved.
- Changed Close Map to a tighter 15x9 exploration camera and Wide Map to 21x13 so normal play favors larger local tiles and clearer sprites.
- Reduced exploration floor-art opacity, especially in Midgaard, so terrain texture supports the scene instead of competing with party/NPC/object silhouettes.
- Reworked map object and party-token drawing to use small plinth shadows, thin frames, and objective/progression markers instead of persistent full-tile colored boxes.
- Moved exploration hover detail into the Location panel and left only a tile cursor border on the map, preventing hover text from covering the playfield.
- Kept save data at version 17 because this pass changes runtime art and presentation only.

## v0.92.0 - Responsive Exploration HUD Pass

- Changed exploration side-panel budgeting so the Location panel gets priority height and Timeline shrinks first, because Timeline is scrollable and route/location information is not.
- Reduced top-chrome clutter during play by hiding the SFX Test button outside settings-style surfaces.
- Fixed top-chrome preference control overflow by collapsing the Reduced Motion toggle when there is not enough width, preventing audio controls from colliding with resource boxes.
- Kept save data at version 17 because this pass changes layout and presentation only.

## v0.91.0 - Tavern Composition and Location Panel Budgeting

- Removed the oversized duplicate tavern title centerpiece from the normal landing composition and replaced it with a smaller Old Road brief so the tavern art can carry the screen.
- Changed Tonight's Table to use a contained 2x2 party layout on large windows instead of a wide bottom strip, while retaining the compact horizontal strip on smaller windows.
- Reworked the world-map Location panel to reserve bottom space for Midgaard/Kobold route trackers before sizing the minimap, which prevents tracker labels and progress chips from overlapping.
- Kept the smaller board-clamped hover card from v0.90 and preserved save data at version 17 because this pass is layout-only.

## v0.90.0 - Tavern and World Map UI Cleanup

- Fixed the tavern first screen showing/clipping a fifth default party card by normalizing the party before drawing and laying out the preview from the active 4-person party.
- Reduced the tavern title plate footprint and constrained the Tonight's Table strip to the available content width so it no longer runs off the right edge of the window.
- Reworked the Midgaard Work tracker in the Location panel so status text, wayfinding text, and progress chips reserve separate vertical space instead of overlapping.
- Shortened the Location panel wayfinding line and made the world-map hover card smaller, board-clamped, and cursor-aware so it covers less of the map while scouting.
- Kept save data at version 17 because this pass changes layout and presentation only.

## v0.89.0 - Sewer Art Promotion and Ratfolk Wiring

- Promoted a cleaned `midgaard-sewer-atlas-runtime-v0.89.png` from the v0.88 sewer sheet as a non-destructive sibling asset.
- Rebuilt the sewer sheet as a 5x4 RGBA runtime atlas with audit-bounded cutouts for sewer entrances, rats, ratfolk enemies, rat-pelt rewards, warning lanterns, and blocked-tunnel rubble.
- Wired sewer rat, giant rat, ratfolk cutthroat, rat mage, rat cleric, and rat brute combat sprites and enemy roster portraits to prefer the Midgaard sewer atlas before older fallback enemy atlases.
- Added source, contact-sheet, and notes/provenance files for the v0.89 sewer atlas under `Docs/ArtReferences`; the contact sheet uses a non-runtime filename so it cannot be selected by the art loader.
- Kept save data at version 17 because this pass changes runtime art and presentation mapping only.

## v0.88.0 - Art Cleanup Audit and Sewer Sheet

- Re-read the Aseprite cleanup handoff and reran the provided watcher, closeout, and cleanup-pack validation scripts.
- Confirmed there are still `0` changed starter cells and `0` cleaned cells across the world-map token, world-map prop, kobold route, kobold boss, and kobold cave prop cleanup packs, so no candidate/starter files were promoted.
- Confirmed the three validated v0.80 exports in `03-export-png` already match Unity `Docs/ArtReferences` by SHA-256: `combat-terrain-atlas-runtime-v0.80.png`, `world-map-exploration-tile-atlas-runtime-v0.80.png`, and `world-map-overlay-atlas-runtime-v0.80.png`.
- Documented current art-thread revision status in `Docs/ART_CLEANUP_AND_REVISION_AUDIT_v0.88.md`.
- Generated and installed `midgaard-sewer-atlas-runtime-v0.88.png`, filling an older v0.56 sewer/cistern art hook with updated sewer entrance, rat enemy, rat-pelt reward, and utility prop cells.
- Added source and prompt/provenance files for the v0.88 sewer atlas under `Docs/ArtReferences`.
- Kept save data at version 17 because this pass changes runtime art and package metadata only.

## v0.87.0 - Formula Status Resonance

- Added status resonance for damage and drain formulas so spell setup matters more in combat: fire punishes webbed/bleeding/poisoned targets, shock conducts through webbing or wards, cold pressures bleeding targets, poison rewards bleeding/webbed setup, death/mind spells echo through hexes, and light sears hexed targets.
- Added resonance side effects in combat: fire burns away webbing, shock can stun through webbing or crack wards, cold can reduce bleeding and frostbind, death/mind reinforces a doom echo, and light reduces an existing hex after the hit.
- Updated formula damage previews to include resonance damage and a concise `resonance:` line, keeping hover math aligned with actual damage.
- Updated Spellbook effect copy to call out that statused targets can resonate.
- Kept save data at version 17 because this pass changes combat/spell behavior only and uses existing combat-unit fields.

## v0.86.0 - Scaffold Art Hook Fill

- Re-ran the provided Aseprite cleanup watcher, closeout, and validation scripts. They still report `CHANGED 0` starter cells and `0` cleaned cells, so cleanup promotion remains correctly blocked.
- Audited art-thread revision folders and documented the result in `Docs/ART_THREAD_REVISION_AUDIT_v0.86.md`: three v0.80 exports are already in Unity with matching hashes, while Midgaard/token/prop/kobold v0.80 work files remain candidate-only.
- Generated and installed `service-scaffold-atlas-runtime-v0.86.png`, filling the existing service-site hook for quest boards, trainers, lore libraries, forges, faction camps, vendors, armory/weapon services, shrines, and related city/world placeholders.
- Generated and installed `dungeon-scaffold-atlas-runtime-v0.86.png`, filling the square 4x4 hook for dungeon gates, crypts, groves, portal seals, sewer grates, caves, stairs, traps, rune plinths, boss chambers, and exits.
- Generated and installed `faction-banner-atlas-runtime-v0.86.png`, filling the banner/sigil hook used by journal/contact fallback art and future faction route UI.
- Added source chroma-key PNGs and prompt/provenance notes for all three sheets under `Docs/ArtReferences`.
- Kept save data at version 17 because this pass changes runtime art and package metadata only.

## v0.85.0 - Art Cleanup Workflow and Route Scaffolds

- Read and followed the Aseprite cleanup handoff at `<Ashen Halls workspace>\AsepriteCleanup-v0.80\MAIN-THREAD-HANDOFF-v0.80.md`.
- Ran the provided starter-edit watcher, closeout, and validation scripts. They reported `CHANGED 0` starter cells and `0` cleaned cells across the cleanup packs, so no candidate/starter PNGs were promoted into Unity.
- Opened the curated first-pass Aseprite batch for manual cleanup continuity: world-map prop cells `00/02/03/06` and token cells `01/03/05/06`.
- Generated and installed `route-scaffold-atlas-runtime-v0.85.png`, a new 5x4 transparent runtime sheet for the existing route-scaffold art hook.
- Added the route-scaffold source chroma-key PNG and prompt/provenance note to `Docs/ArtReferences`.
- Kept save data at version 17 because this pass changes runtime art and package metadata only.

## v0.84.0 - Elemental Terrain Pressure

- Added a more consistent terrain reaction layer for combat spells: fire burns web, ignites gas, and melts ice; cold quenches fire into ice and settles gas; shock conducts through ice, gas, and web; sanctuary cleanses curse; and curse can spoil sanctuary.
- Extended spell previews to show target-tile terrain reactions before casting, so Beta Lab stress testing can verify the expected outcome before spending mana.
- Applied hit-terrain reactions to splash victims as well as the primary target, making Fireball, Meteor Shower, Iceburst, and Shock Burst more tactically readable in clustered fights.
- Improved start-turn field effects: fire clears webbed status, sanctuary heals/wards and reduces poison/bleed/hex, and curse cracks shields while applying mind pressure.
- Kept save data at version 17 because this pass changes combat/spell behavior only and uses existing fields.

## v0.83.0 - Optimized Runtime Art Wiring

- Promoted the optimized art-thread sheets as the active runtime mapping set: `world-map-exploration-tile-atlas-runtime-v0.80.png`, `world-map-overlay-atlas-runtime-v0.80.png`, `combat-terrain-atlas-runtime-v0.80.png`, `midgaard-tile-atlas-runtime-v0.80.png`, `midgaard-city-prop-atlas-runtime-v0.79.png`, `midgaard-gate-atlas-runtime-v0.79.png`, and `midgaard-wall-atlas-runtime-v0.78.png`.
- Corrected Midgaard city-floor mapping so market, temple, fountain, diner, tavern, armorer, weapons, enchanter, gate, guard, king, sewer, provisions, rat-pelt, recall, and generic paved districts use matching cells from the v0.80 tile sheet.
- Added deterministic world-map tile variants from the v0.80 exploration sheet so roads, paving, moss, mire, quarry, glass, ash, cliff walls, mire walls, forest walls, and red walls vary by zone without looking random.
- Remapped general combat terrain and hazard cells to the optimized v0.80 combat terrain atlas: fire, ice, web, curse, glyph, stone, road, sewer/cistern, quarry, glass, fen, Dusk Market, courts, and Red Gate battles now point at visually appropriate floor art.
- Kept save data at version 17 because this pass changes runtime art mapping and presentation only.

## v0.82.0 - Enemy Movement Mechanics

- Replaced greedy enemy approach movement with reachable-tile destination planning that reuses the combat movement cost grid.
- Enemy destination scoring now accounts for path cost, attack range, line of sight, enemy role, target guard/stealth, damage resistance/weakness, and hazardous terrain.
- Ranged and caster enemies can make a one-tile reposition before attacking when they are too close to the party or standing in dangerous terrain.
- Brute/melee enemies prioritize getting adjacent more reliably, while ranged enemies value sight lines and safer spacing.
- Enemy movement still respects the existing move-plus-attack policy: only selected roles/ranks can attack after a one-cost move, and longer repositioning remains a visible advance.
- Kept save data at version 17 because this pass changes runtime combat behavior only.

## v0.81.0 - World Map Usability

- Added a runtime exploration view toggle: Close Map keeps the large 17x11 local view, while Wide Map switches to a 23x15 tactical overview for route context.
- Added a visible View button to the exploration command bar and a Tab hotkey for switching map views.
- Added a compact minimap to the Location panel showing map terrain, the current viewport rectangle, the party position, nearby objects, and active objective/scaffold markers.
- Rebalanced exploration side-panel heights so the Location panel carries navigation context instead of being a short text-only box.
- Added the current map mode to the board strip and hover card, and updated F1/README control text.
- Kept save data at version 17 because this pass changes runtime presentation and input only.

## v0.80.0 - World Map Viewability and Title Art

- Changed exploration from a 21x13 camera to a tighter 17x11 camera so world-map tiles, NPCs, gates, props, and the party token read larger.
- Increased party, NPC, town-object, gate, wall, and adventure-object draw sizes by reducing stacked object padding.
- Generated and wired `midgaard-tile-atlas-runtime-v0.80.png`, a 5x4 city-floor sheet with district-specific cobbles, market paving, temple glow, fountain stone, tavern threshold, vendor rows, enchanter runes, gate stones, sewer damp stone, and recall markings.
- Made Midgaard city floors draw the generated tile atlas more strongly and skip older procedural motifs on top of generated city art.
- Increased ambient Midgaard prop visibility and spawn density while still keeping props below NPC/object/player layers.
- Promoted the existing generated Ashen Halls title-card art into a large tavern landing title plate, with Unity-rendered title text for readability.
- Kept save data at version 17 because this pass changes map presentation, runtime art, and tavern title presentation only.

## v0.79.0 - Directional Gates and Midgaard Props

- Generated and wired `midgaard-gate-atlas-runtime-v0.79.png`, a 5x4 directional city-gate sheet with closed gates, open side-facing gates, towers, damaged gates, mossy gates, torch-lit gates, and royal-banner variants.
- Generated and wired `midgaard-city-prop-atlas-runtime-v0.79.png`, a 5x4 Midgaard prop sheet with lamps, stalls, crates, barrels, benches, banners, signposts, wells, fountains, sewer grates, stairs, shrine lamps, weapon racks, armor stands, alchemy bottles, hay bundles, and notice boards.
- Mapped West Gate and East Gate to the new side-facing open gate cells before falling back to older wall/town art, so the usable road exits no longer share one generic gate stamp.
- Added sealed North Gate and South Gate objects at the Midgaard wall centerlines without changing movement rules; they are visual/hover landmarks and use the new closed gate art.
- Added sparse district-aware prop drawing inside Midgaard streets so market, temple, diner, tavern, vendor, gate, guard, king, and sewer paving feels less empty while staying below object/player layers.
- Kept the Beta Lab `Mage` button available for immediate caster/spell testing and kept save data at version 17.

## v0.78.0 - Party Language, Midgaard Walls, and Mage Lab

- Standardized live UI and gameplay text on Party wording, including tavern copy, muster summary, story logs, defeat/victory screens, Party Ledger, Party Gear, Armory tab text, and ability previews.
- Generated and wired `midgaard-wall-atlas-runtime-v0.78.png`, a 5x4 wall sprite sheet with straight walls, corners, gate arches, tower, damaged wall, and mossy wall variants.
- Added Midgaard corner wall objects and directional wall selection for both city-wall terrain tiles and decorative wall objects, so the city wall no longer relies on one generic wall stamp.
- Added a Beta Lab `Mage` button that turns the active party tester into a level 4 ember mage with enough mana and skill to test Fireball, Meteor Shower, Cold Lance/Iceburst, Shock Burst, Fire Floor, and Burn Cover immediately.
- Kept save data at version 17 because this pass changes presentation, runtime art hooks, and beta-test setup only.

## v0.77.0 - Combat UI Declutter and Zone Floors

- Removed the duplicate promoted End Turn status tile so the actual End Turn button is the single obvious turn-finisher when movement or action economy is spent.
- Enlarged command button art and added a small bottom label ribbon so attack, skill, guard, elixir, and end-turn icons fill their button tiles more clearly.
- Simplified combat side panels: Party cards no longer show equipment lines, and Enemy Combatants cards now default to role/range/damage with status shown only when active.
- Changed combat floor selection from depth-based per-tile mixing to clustered zone palettes, so roads, Midgaard, cisterns, markets, quarry, warrens, fen, courts, and red-gate fights look more place-specific.
- Quieted tactical frames on breakable tree/stone cover so rocks no longer carry the small debug-like overlay marks.
- Kept save data at version 17 because this pass changes combat presentation only.

## v0.76.0 - World Map Readability Fix

- Changed exploration from a 23x15 camera to a 21x13 camera, increasing tile size while keeping enough world context around the party.
- Fixed the main overlap bug: world-map objects and the party token now scale inward inside their tile instead of being expanded outside the tile bounds.
- Added type-aware exploration token sizing so Midgaard NPCs, guards, walls, gates, landmarks, buildings, and adventure objects do not all compete at the same visual size.
- Moved exploration movement hints underneath object art so click/movement guidance no longer paints over NPCs and buildings.
- Quieted frames on walls, guards, and NPCs while preserving stronger frames for major locations and objective markers.
- Kept save data at version 17 because this pass changes exploration presentation only.

## v0.75.0 - Basic Combat Mechanics Pass

- Changed temporary combat terrain to tick once per combat round instead of decrementing on every unit's start turn, making Tree Cover, stone, fire, gas, web, ice, sanctuary, and curse fields much easier to reason about in larger fights.
- Added round-start terrain normalization so duplicate same-tile field marks collapse into one authoritative obstacle/hazard with sane duration and integrity.
- Rebalanced terrain durations for the new round-based timing: Tree Cover stays long-lived but breakable, while fire, gas, web, ice, sanctuary, curse, and enemy warning marks use shorter 3-4 round timers.
- Routed enemy turns through a shared enemy-action finish helper so enemy movement, attacks, specials, cover breaks, and advances leave combat state in a more consistent spent-turn shape.
- Gave selected enemies a conservative move-plus-attack rule: elites, brute melee enemies, caster-like ranged enemies, and the Kobold King can attack after one step, while longer repositioning still consumes their visible turn beat.
- Updated Tree Cover, terrain previews, hover notes, help text, and README wording to say rounds where the mechanic now ticks by round.
- Kept save data at version 17 because this pass changes runtime combat rules and repair behavior without adding serialized fields.

## v0.74.0 - Combat Action Framework and Modal Flow

- Centralized player combat action completion so weapon attacks, cover breaks, formula casts, targeted skills, instant skills, guard, elixir, and end turn now share one finish path for action spending, modal cleanup, combat sync, and turn advancement.
- Added shared spell/skill action-card metadata for UI presentation: cost, range, target, path, ready state, usability, summary, and detail are now assembled through one small helper layer.
- Changed the combat Spellbook to preview-first behavior. Clicking a spell tile now selects/previews it safely; the explicit Ready button arms it and returns to board targeting.
- Changed Combat Skills to preview-first behavior. Clicking the row inspects the skill; the Ready/Use chip is the deliberate commit action.
- Tightened combat phase state so opening Spellbook or Skills stays in choose-action mode until a spell or skill is actually readied for targeting.
- Made the Combat Skills modal more defensive at shorter heights by compressing skill rows before they collide with the detail panel.
- Added `Docs/COMBAT_ACTIONS_v0.74.md` to capture the next action-system contract for future spells, abilities, enemy moves, and tests.
- Kept save data at version 17 because this pass changes runtime flow and UI scaffolding only.

## v0.73.0 - Ability Art, Ranger Skills, and Zone Music

- Generated and wired seven new runtime art sheets for combat skills, Spellbook cards, large spell effects, combat command buttons, HUD icons, floating spell bursts, and ranger/missile battlefield effects.
- Expanded Combat Skills beyond warrior and rogue: rangers now have Aimed Shot, Pinning Shot, Volley, and Scout Mark, with missile-skill scaling, line-of-sight rules, arcing volley pressure, and new icon/effect art hooks.
- Added a ranger to the default Quick Start party and updated Martial Lab/Beta Lab testing so warrior, rogue, and ranger ability kits can be stress-tested quickly.
- Added distinct sparse one-shot sound cues for the new ranger skills and mark/arrow effects.
- Added simple procedural music loops for tavern, combat, Midgaard, and major exploration zones while keeping the old-school sparse sound style and the same audio mute/volume controls.
- Updated combat icon mapping so the v0.73 command/HUD/spell/effect sheets are preferred before procedural fallbacks.
- Kept save data at version 17 because this pass changes art, audio, UI mapping, and combat ability behavior without adding serialized fields.

## v0.72.0 - Combat UI, Spellbook, and Skill Polish Audit

- Audited the combat, spell, and martial skill presentation paths for repeated status text, brittle tooltip placement, and sprite-overlay clutter.
- Generated and wired new v0.72 combat UI panel art for tooltip backplates, action-ready/action-used chips, targeting symbols, spell motifs, and melee flourish cells.
- Generated and wired a refreshed v0.72 warrior/rogue ability icon atlas for Charge, Execute, Rally, Whirlwind, Stealth, Ambush, Smoke Bomb, and Eviscerate.
- Reworked combat hover cards so they stay clamped away from the side roster and command bar, use a consistent art-backed panel, and stop repeating the same action-state text already shown in the bottom readout.
- Clarified command button help: Cast now presents as Spellbook until a formula is armed, Skill presents as Skills until an ability is armed, and targeted/instant actions are labeled more truthfully.
- Kept sprite overlays art-first: generated combat sprites remain free of old body-covering equipment/status marks, with details left to HP bars, pips, hover cards, readouts, and side panels.
- Kept save data at version 17 because this pass changes presentation, runtime art, and tooltip behavior only.

## v0.71.0 - Kobold King Art and Warrior-Mage Boss Pass

- Generated and wired a dedicated unique-item atlas for the Sword of Unfathomable Darkness, so the Kobold King reward now has its own blackglass/violet sword icon instead of sharing the generic sword art.
- Generated a refreshed 4x4 Kobold King boss atlas with healthy, wounded, defeated, portrait, casting, throne, treasure, and trophy-style cells.
- Generated a Kobold King encounter art sheet and composed a v0.71 hybrid cave prop atlas that upgrades shield-hall barricades, royal banners, braziers, ice shards, charge trails, retreat shadows, charms, treasure, crown, broken shield, and sword pickup sparkle cells.
- Expanded Varkh's boss AI into a warrior-mage routine: he can ward/heal nearby kobolds, charge a target to stun, retreat back toward the throne line, throw splash fireballs, and drive ice lances that leave slick terrain.
- Updated enemy sidebar/intro text so Varkh reads as a unique warrior-mage king rather than a generic rally brute.
- Kept save data at version 17 because this pass uses existing combat state, enemy roles, item names, and runtime art loading.

## v0.70.0 - Modal Casting Fixes and Combat Loot

- Hardened combat modal input. When Spellbook or Combat Skills is open, hidden command buttons are no longer drawn/clickable, and modal mouse events are consumed before they can leak into the battlefield.
- Added clearer Spellbook affordances: spell tiles now show Ready/Set chips, and the detail page includes an explicit Ready/Return button.
- Added clearer Combat Skills affordances: rows now show visible Ready/Use chips while remaining clickable across the full row.
- Added battle loot generation after normal combat victories. Enemy depth, enemy count, gear-bearing roles, veteran rank, and elite rank all improve drop odds; veteran and elite enemies also raise item rarity floors.
- Added boss-loot scaffolding for named encounters and final-gate relics.
- Replaced the Kobold King generic trophy with the epic Sword of Unfathomable Darkness, described as a stolen adventurer's sword.
- Added real weapon behavior for the Sword of Unfathomable Darkness and future vampiric gear: successful hits drain a small amount of life back to the wielder.
- Kept save data at version 17 because this pass derives new combat behavior from existing item/weapon names and uses existing inventory fields.

## v0.69.0 - Midgaard Conversations and Tactical Ground Spells

- Added a first-pass conversation popup system for map interactions. Dialogue takes input focus, can be closed with Continue, Enter, Space, or Esc, and uses existing Midgaard/NPC art when available.
- Expanded Midgaard's city layout with six additional named contacts: City Courier Tovan, Wounded Traveler Edda, Stable Hand Pell, Royal Herald Vann, Novice Healer Sera, and Old Road Scout Yara.
- Gave new contacts lightweight first-time rewards, route hints, and Timeline entries through existing gold, supplies, elixir, XP, story-flag, and discovered-zone systems.
- Added Hallowed Circle as a priest/mend terrain formula that creates sanctuary ground for holding a position.
- Added Doom Circle as a hex terrain formula that creates cursed ground with mind pressure, hex risk, and movement friction.
- Updated Beta Lab hazard staging, hover previews, terrain warnings, spell colors, icons, and sound routing for the new terrain types.
- Kept save data at version 17 because this pass uses existing story flags, discovered zones, map objects, and combat terrain state.

## v0.68.0 - Runtime Art Audit and Wiring

- Audited runtime art files in `Docs/ArtReferences/` against the Unity loader prefixes and draw helpers.
- Wired `combat-ui-panel-atlas-runtime-*`, which previously existed as a runtime-looking sheet but had no loader. It now adds subtle generated RPG-panel texture to side panels and victory panels without covering the board.
- Changed Armory/item icon loading to prefer `item-inventory-atlas-runtime-*` before older equipment sheets.
- Added an inventory-atlas-specific item cell map so epees, daggers, axes, bows, crossbows, foci, shields, armor, robes, potions, coins, scrolls, rings, and gems point to the intended v0.51 cells.
- Added `Docs/ART_AUDIT.md` to record active runtime sheets, fallback/reference-only sheets, and the remaining art-thread opportunities.
- Kept save data at version 17 because this pass changes presentation, art selection, and docs only.

## v0.67.0 - World and Midgaard Art Fill

- Added refreshed generated runtime art sheets for Midgaard buildings, Midgaard NPC tokens, world-map ground tiles, world landmarks, kobold route markers, kobold smoke-cave props, and kobold-specific combat terrain.
- Wired `kobold-combat-terrain-atlas-runtime-*` into Chapter II combat floors so kobold ambush, smoke-cave, and king-hall fights can use richer cave, shield-hall, hazard, glyph, and rift floor art without replacing the general combat terrain sheet.
- Added beta-safe `glyph` and `demonrift` terrain kinds. Kobold shamans and bone wizards can place them as temporary warning marks that add movement friction and preview future demon-summon pressure.
- Added authored glyph/rift marks to kobold cave and throne-room setups so the new art appears during route testing even before the full summoning rules are balanced.
- Documented the new art prompts and atlas contracts for follow-up cleanup or replacement by the art thread.
- Kept save data at version 17 because this pass changes runtime art, temporary combat terrain behavior, and encounter setup only.

## v0.66.0 - Midgaard City Errands

- Added four named Midgaard NPC map objects: Market Clerk Nessa, Mira of Midgaard, Tavern Keeper Orren, and Gate Captain Brann.
- Added two beginner city errands. Mira's Lamp Round sends the party through the market, diner, tavern, and back to Temple Square. Brann's Gate Survey sends the party to both gates before reporting back.
- Added city-errand rewards through existing systems: supplies, gold, elixirs, world XP, banners, sound cues, and Timeline messages.
- Updated Midgaard wayfinding so active city errands become the current objective and receive objective rings before falling back to the sewer/king quest chain.
- Expanded the Journal with a Midgaard City Errands section and added procedural NPC placeholder tokens with runtime atlas hooks for later art replacement.
- Kept save data at version 17 because the new progress uses existing story flags and appended object enum values only.

## v0.65.0 - World System Scaffolding

- Added a route-scaffold layer to world generation with future-content nodes for quest boards, waystones, training rings, lore libraries, forge sites, faction camps, dungeon gates, crypts, groves, and portal seals.
- Added beta-safe placeholder interactions for those nodes: small recovery, skill-point tests, lore/caster rewards, forge loot, faction supplies, and placeholder dungeon fights.
- Expanded the Journal with a World System Scaffolds section showing each node's unlock depth, route zone, located/tested state, and future purpose.
- Added runtime art hooks for `route-scaffold-atlas-runtime-*`, `dungeon-scaffold-atlas-runtime-*`, `faction-banner-atlas-runtime-*`, and `service-scaffold-atlas-runtime-*`.
- Added procedural fallback glyphs for the new nodes so the build stays readable before dedicated art arrives.
- Kept save data at version 17 because progress uses existing story flags and no new serialized state is required.

## v0.64.0 - Combat Floating Text Readability

- Reworked combat floating labels so simultaneous damage, healing, status, cover, and spell text use deterministic lanes plus collision-aware placement.
- Added wider dark backplates, text shadows, brighter color treatment, and board-edge clamping so labels stay readable without spilling under the combat HUD.
- Kept floating combat text visible under Reduced Motion as short static labels instead of suppressing it entirely.
- Added a small cap on active floating labels to prevent old burst text from piling up during Beta Lab stress tests.
- Kept save data at version 17 because this pass changes combat presentation only.

## v0.63.0 - World Progression and Map Markers

- Added exploration XP for first-time zone discoveries, cache openings, shrine restoration, and chapter descents, all feeding the existing party XP/level loop.
- Made level-up logs more useful: HP/MP gains, stat points, skill points, and newly unlocked spells or martial abilities are now named when a character levels.
- Added formula level gates so combat Spellbooks show learned formulas instead of every spell in a caster's craft from level one. Spell Reference rows now show required level.
- Added a party-growth chip to the Location panel and a learned/next-unlock line to Tavern Muster.
- Fixed zone discovery checks so journal/contact scaffolding recognizes stored `depth:zone` discovery keys.
- Added generated `world-map-progression-overlay-atlas-runtime-v0.63.png` plus source/prompt notes for quest rings, discovery banners, training icons, route gates, stairs, recall, kobold smoke, and final-gate omens.
- Kept save data at version 17 because this pass uses existing XP/level/point/discovery fields and adds runtime art only.

## v0.62.0 - Combat Audit and Character Sprite Refresh

- Added active-unit threat awareness to the combat readout so the current turn plainly says whether enemies can hit now, are pressuring, or pose no direct threat.
- Added subtle red/amber enemy threat frames and corner pips on the combat board. These cues avoid covering the new sprite art while still showing immediate danger.
- Normalized stale combat command selection when the active unit is stunned, spent, or no longer able to use the selected action.
- Improved hotkey feedback: blocked combat hotkeys now write the disabled reason to the Timeline, and Enter/Keypad Enter can end a combat turn.
- Added generated `character-combat-atlas-runtime-v0.62.png` plus the raw `source-character-combat-atlas-v0.62-magenta.png` for larger race/class party combat figures.
- Kept save data at version 17 because this pass changes combat UI, input handling, docs, and runtime art only.

## v0.61.0 - Combat Command Usability

- Made combat command buttons easier to scan with visible mnemonic hotkeys and compact action labels under the icon art.
- Added one-tile combat movement on W/A/S/D and arrow keys, while keeping click-to-move and number hotkeys intact.
- Made End Turn more prominent when the active unit is out of movement, out of action, stunned, or sleeping.
- Updated the command status panel to recommend Space/End Turn in spent states instead of repeating generic move/action text.
- Added `combat-command-icon-atlas-runtime-v0.61.png`, a clearer deterministic command icon sheet for Move, Attack, Cast, Guard, Elixir, and End Turn.
- Kept save data at version 17 because this pass changes combat UI, input handling, docs, and runtime art only.

## v0.60.0 - Sprite Alignment and Combat SFX

- Added exact-grid `enemy-sprite-atlas-runtime-v0.60.png` and `character-combat-atlas-runtime-v0.60.png` sheets derived from the richer generated art, with centered silhouettes, consistent foot anchors, preserved alpha, and subtle outlines for combat readability.
- Added `ability-icon-atlas-runtime-v0.60.png` as a clearer eight-cell Combat Skills icon sheet for Charge, Execute, Rally, Whirlwind, Stealth, Ambush, Smoke Bomb, and Eviscerate.
- Added distinct procedural one-shot SFX for Charge, Whirlwind, Execute, Ambush, Eviscerate, Stealth, Smoke Bomb, Rally, Fireball, and Meteor.
- Routed the newer martial skills and showcase formulas to their dedicated sound effects instead of reusing generic UI, blade, spell, or poison cues.
- Kept save data at version 17 because this pass changes presentation, runtime art, and procedural audio only.

## v0.59.0 - Spell Lab Test Harness

- Improved Beta Lab as the main player-side magic test route.
- Added lab-only spellcraft preparation so Oryn can test ember, hex, and pact formulas while Vesh can test mend formulas without changing the normal campaign party.
- Added `Craft` to the Beta Lab toolbar to restore and re-enable test spellcraft during combat.
- Added `Stage` to the Beta Lab toolbar to cluster marked enemies and reactive hazards for Fireball, Meteor, Shock, Cold, Burn Cover, Tree Cover, gas, web, ice, and stone testing.
- Updated Martial Lab text to include Rally and Smoke Bomb in the staged skill test set.
- Kept save data at version 17 because this pass changes test setup, combat staging, and docs only.

## v0.58.0 - Combat Magic and Ability Utility

- Added warrior Rally as an instant combat skill that braces the warrior, applies a ward, and steadies adjacent allies.
- Added rogue Smoke Bomb as an instant combat skill that grants stealth and drops short-lived gas clouds on open adjacent tiles.
- Added priest Sanctuary Ward and hex Night Veil formulas, making group warding and spell-driven stealth part of combat testing.
- Added stealth as a status that can now be applied by formulas as well as rogue skills.
- Added `ability-icon-atlas-runtime-v0.58.png` with dedicated icons for Charge, Execute, Rally, Whirlwind, Stealth, Ambush, Smoke Bomb, and Eviscerate.
- Kept save data at version 17 because the new tactics reuse existing combat/status fields and runtime art loading.

## v0.57.0 - Martial Lab and Skill Test Flow

- Changed the default Quick Start party so Maer is a true warrior, making the new Skill button visible immediately alongside Selka's rogue skills.
- Added a Martial Lab tester door to the tavern for immediate warrior/rogue combat stress testing.
- Added a staged Martial Lab encounter with promoted warrior/rogue testers, clustered melee targets, a wounded execute target, and a few cover/hazard tiles.
- Expanded the in-combat lab toolbar for Martial Lab with `Promote`, `Wound`, and `Cluster` controls so Whirlwind, Execute, Ambush, and Eviscerate can be tested without restarting.
- Made lab `Spawn` use melee test enemies in Martial Lab and caster-pressure enemies in Beta Lab.
- Kept save data at version 17 because this pass changes default/test setup, combat staging, and documentation only.

## v0.56.0 - Martial Skills and Character Sprites

- Added a martial Skill action for warriors and rogues. The third combat button now opens Combat Skills for those classes and remains Cast for spellcasters.
- Added warrior Enrage as a passive physical-damage bonus below half HP.
- Added warrior Charge, Execute, and level-gated Whirlwind combat skills.
- Added rogue Stealth, Ambush, and level-gated Eviscerate combat skills, including stealth targeting pressure and bleed/stun effects.
- Added hover previews, target highlights, click instructions, status pips, and side-panel/readout wording for martial skills.
- Added generated `character-combat-atlas-runtime-v0.56.png` and source/prompt notes for race/class-aware party combat sprites.
- Updated Tavern Muster to show larger generated character portraits in the roster and preview instead of relying mostly on tiny class icons.
- Kept save data at version 17 because the only serialized addition is optional combat stealth state, which defaults safely on older saves.

## v0.55.0 - Combat Visual Clarity and Title Art

- Added generated `ashen-halls-title-card-runtime-v0.55.png`, `ashen-halls-icon-runtime-v0.55.png`, and `spellbook-open-runtime-v0.55.png` assets under `Docs/ArtReferences/`.
- Wired the generated title plate into the tavern and in-game chrome while keeping exact title text rendered by Unity.
- Replaced the old tiny procedural AH mark with the generated ashen-hall emblem when available, with the procedural mark kept as fallback.
- Reserved real top space for the game chrome and moved the combat turn order into a compact lane above the grid.
- Made floating combat text smaller, shorter-lived, and more tightly stacked so repeated hits/resists do not blanket the sprites.
- Added procedural fallback combat terrain motifs for road, cistern, cave, ice, web, scorch, gas, and deeper dungeon floors when a terrain atlas is missing.
- Added edge-only tactical frames for combat obstacles so tree/stone/hazard state reads without covering the art.
- Enlarged atlas-drawn party combat sprites and added small edge accents for role, sigil, weapon, and casting identity.
- Rebalanced Party, Enemy Combatants, and Timeline side-panel heights so combat details live in the sidebar instead of over the board.
- Rebuilt the combat Spellbook overlay around a generated open-book backing: the left page now uses icon-plus-name spell tiles, while hover/selected spell rules and cast guidance live on the right page.
- Kept save data at version 17 because this pass changes presentation and optional runtime art only.

## v0.54.1 - Midgaard Wayfinding and Safe Roads

- Added an exploration-side Midgaard Work tracker that shows the current first-city quest step across King, Sewer, Armor, and Road.
- Added active errand directions and landmark distances in the Location panel so the King, sewer, armorer, Temple Square, Kate's Diner, and east/west gates are easier to find.
- Added colored objective rings and hover text for current Midgaard errands, including King Hall, the sewer grate, and the armorer/workbench.
- Made safe Midgaard and the nearby lamp road truly safe from random patrol rolls while preserving authored sewer combat.
- Extended the east and west gate approaches with clearer road/outwork tiles so the walled city connects more naturally to the surrounding map.
- Kept save data at version 17 because this pass changes presentation, map repair, and encounter gating only.

## v0.54.0 - Midgaard Start Zone and Rat-Pelt Quest

- Built a deterministic Midgaard start-zone scaffold on depth 1 with Market Square at the start, Temple Square a few tiles north, a fountain and recall circle, Kate's Diner to the southeast route, a tavern, armorer, weapons vendor, enchanter, King's Hall, sewer grate, rat-pelt workbench, town guards, and only east/west city gates.
- Added city-wall blocking around Midgaard so the north and south sides are intentionally closed while the east and west gates connect back into the larger world map.
- Added Temple Square recall: the visible Recall command and `Y` hotkey return the party to the temple/fountain anchor and fully restore the party in exploration.
- Added first quest scaffolding: the King of Midgaard assigns the sewer rat work, the sewer starts a fixed rat/ratfolk combat wave, victory grants rat pelts, and the armorer/workbench turns four pelts into quest-grade rat-pelt armor.
- Added simple one-time beta vendor interactions for Kate's Diner/provisions, tavern support, starter armor, starter weapon, and a starter weapon enchantment.
- Added four generated v0.56 runtime art sheets for Midgaard town landmarks, Midgaard ground tiles, Midgaard NPC/vendor tokens, and Midgaard sewer/rat quest assets, plus source copies and prompt notes.
- Kept save data at version 17; old depth-1 maps are repaired in place to include the new Midgaard scaffold.

## v0.53.1 - Guard Damage Hotfix

- Fixed weapon attacks against guarding targets applying Guard damage reduction twice.
- Updated weapon damage previews so melee/ranged attack ranges use the same single Guard reduction path as actual damage.
- Kept Guard's hit-chance penalty and braced-guard bonus behavior intact.
- Changed attack hover text from a numeric extra-looking Guard subtraction to a simple `guarded` note because the damage range already includes Guard.
- Added `Docs/COMBAT_CLEANUP_TODO.md` with the next small combat cleanup targets.
- Kept save data at version 17 because this pass changes combat math only.

## v0.53.0 - World Map Scale and Art Fill

- Changed exploration from a full-map 46 by 30 render to a centered 23 by 15 viewport, roughly doubling tile size and making the party token, landmarks, and route art much easier to read.
- Enlarged exploration object and party-token drawing so map sprites have a stronger board presence.
- Added edge glints around the exploration viewport when the map continues beyond the visible area.
- Added five generated v0.55 runtime atlas sheets: world-map ground tiles, landmarks, overlays, larger party tokens, and environmental props.
- Mapped cache and encounter objects into the new landmark atlas so treasure and danger markers use the richer art immediately.
- Increased packaged default window size to 2048x1152 on larger displays, with safe fallbacks for 1080p and smaller displays.
- Saved source copies and prompt files beside the new runtime atlases for repeatable no-infographic art generation.
- Kept save data at version 17 because this pass changes presentation and optional runtime art only.

## v0.52.0 - Journal Art Fill

- Added `story-card-atlas-runtime-v0.54.png`, a generated 5 by 4 Journal story-card sheet for chapter cards, route cards, victory/defeat cards, and future chapter placeholders.
- Added `npc-portrait-atlas-runtime-v0.54.png`, a generated 4 by 4 contact portrait sheet for Midgaard, Dusk Market, Green Shrine, Old Quarry, Glass Warrens, Red Gate, kobold, ratfolk, drow, demon, tavern, and later-route contacts.
- Saved source copies and exact prompt files beside both runtime atlases for repeatable no-infographic art generation.
- Updated `Docs/ART_INTAKE.md` so NPC portrait cells 12-15 now have concrete generated contact roles instead of vague reserves.
- Kept save data at version 17 because this pass changes optional art only.

## v0.51.9 - Kobold Route Art Fill

- Added `kobold-route-atlas-runtime-v0.54.png`, a generated 4 by 4 runtime route icon sheet for Chapter II's Kobold Smoke path.
- The route sheet covers Dusk Market ambush, smoke cave, king hall, king portrait marker, shield banner, bone charm, cave drum, route-clear trophy, smoke clue, patrol/slinger/shaman markers, hidden cave trail, royal cache, king-hall danger, and Bone Road exit icons.
- Added the raw magenta chroma-key source and exact prompt beside the runtime PNG so the successful no-infographic prompt can be reused.
- Existing route UI now prefers the dedicated route sheet before falling back to the Kobold King boss atlas cells added in v0.51.8.
- Kept save data at version 17 because this pass changes optional art only.

## v0.51.8 - Kobold King Art Fill

- Added generated runtime art for the special Chapter II boss route:
  - `kobold-boss-atlas-runtime-v0.54.png` with Varkh boss poses, portrait, phase-two art, rally markers, shield-hall props, and victory/defeat icons.
  - `kobold-cave-prop-atlas-runtime-v0.54.png` with smoke-cave tree/stone/web/gas/fire/ice/barricade/stall/stakes/drum/bones/fungi/torch/water/cache props.
- Added raw magenta chroma-key sources and exact generation prompts beside the runtime PNGs for repeatable art-thread iteration.
- Wired extra Kobold King atlas cells into the route tracker, Dusk Market cave marker, Journal route milestones, and low-health boss sprite phase.
- Updated `Docs/ART_INTAKE.md` to document the new live boss cells and route-marker fallback behavior.
- Kept save data at version 17 because this pass changes presentation and optional runtime art only.

## v0.51.7 - Enemy Art Wiring

- Added the generated `enemy-sprite-atlas-runtime-v0.54.png` sheet as the current enemy sprite atlas candidate.
- Changed runtime art selection to prefer the highest versioned atlas filename before using file modified time as a tie-breaker, making art-thread drops more predictable.
- Remapped enemy combat board and roster sprites to the v0.54 grid: kobold scouts/slingers/shamans/wizards/guards/king, ratfolk fighters/casters, drow, imps, and demons now pull from the intended cells.
- Kept sewer rats and unsupported creature shapes on older fallback sheets until a dedicated rat/creature sheet arrives.
- Saved the raw chroma-key source and prompt next to the runtime atlas for future art-thread iteration.
- Kept save data at version 17 because this pass changes presentation and art loading only.

## v0.51.6 - Combat UI Declutter

- Grouped combat commands into targeted actions and instant actions so Guard, Elixir, and End Turn read as immediate choices.
- Simplified the combat command status box to show only the selected action icon, move points, and action readiness.
- Reduced grid hover cards to tactical outcome, tile/cover note, and click instruction instead of repeating the full turn state.
- Moved target badge text toward tile edges with lighter backing so it interferes less with unit art.
- Simplified Spellbook spell cards to icon, name, MP, range, and path tag while keeping detailed rules in the selected spell detail panel.
- Updated combat tooltip placement to use current-frame side-panel bounds.
- Kept save data at version 17 because this pass changes presentation only.

## v0.51.5 - Journal and Story Art Placeholders

- Added a Journal tab to the Party Ledger for chapter state, route milestones, discovered contacts, and story placeholder art.
- Added `J` as a gameplay hotkey and an exploration command-bar Journal button when space allows.
- Added optional runtime loading for `story-card-atlas-runtime-*` as a 5 by 4 story-card sheet.
- Added optional runtime loading for `npc-portrait-atlas-runtime-*` as a 4 by 4 NPC/contact portrait sheet.
- Kept procedural placeholders for missing Journal art so the game remains playable while the art thread fills slots.
- Kept save data at version 17 because this pass uses existing repairable story state and optional art hooks.

## v0.51.4 - World Map Art Runtime Hooks

- Added optional runtime loading for `world-map-exploration-tile-atlas-runtime-*`, `world-map-landmark-atlas-runtime-*`, `world-map-overlay-atlas-runtime-*`, and `world-map-token-sprite-atlas-runtime-*`.
- Exploration ground tiles now check the dedicated world-map tile atlas before falling back to the older environment sheet and procedural motifs.
- Exploration landmarks now check the dedicated landmark atlas before prop/quest/world-object fallback art.
- Adjacent movement hints can use the world-map overlay atlas, while the party marker can use the world-map token sprite atlas.
- Expanded `Docs/ART_INTAKE.md` with cell contracts for the new world-map atlas hooks.
- Kept save data at version 17 because this pass only adds optional art-intake/rendering fallback paths.

## v0.51.3 - Kobold Boss and Cave Art Placeholders

- Added optional runtime loading for `kobold-boss-atlas-runtime-*` as a 4 by 4 dedicated Chapter II boss sheet.
- The Kobold King now checks the dedicated boss atlas before falling back to generic enemy sprite, creature, world-object, or older boss art.
- Added optional runtime loading for `kobold-cave-prop-atlas-runtime-*` as a 4 by 4 Chapter II cave/cover sheet.
- Kobold ambush, smoke-cave, and king-hall combat obstacles now check the cave prop atlas before using older generic terrain art.
- Expanded `Docs/ART_INTAKE.md` with cell contracts for the new boss and cave prop atlases so the parallel art thread can work without another code handoff.
- Kept save data at version 17 because this pass only adds art-intake/rendering fallback paths.

## v0.51.2 - Kobold Route Tracker and Art Intake Hook

- Added a Kobold Route tester door on the tavern first screen so the Chapter II ambush, cave, and king sequence can be stress tested without walking the whole map.
- Added a visible Kobold Smoke route tracker to exploration's Location panel with Ambush, Cave, King, and Road milestones.
- Added special Dusk Market cave marker treatment that changes as the route moves from ambush to smoke cave to king hall.
- Added optional runtime loading for `kobold-route-atlas-runtime-*` as a 4 by 4 route-specific art sheet, leaving the art thread free to drop in polished kobold route icons later.
- Documented the new route atlas cell contract in `Docs/ART_INTAKE.md`.
- Kept save data at version 17 because the new work is UI, testing, and optional art-intake scaffolding.

## v0.51.1 - Chapter II Kobold Story Scaffolding

- Added persistent story flags for authored chapter beats while keeping save data at version 17 through repairable optional fields.
- Added a Dusk Market kobold ambush trigger on depth 2, with a clear story objective update after victory.
- Turned the depth 2 Dusk Market cave into a two-step story route: smoke-cave fight first, then the Kobold King boss fight on return.
- Added authored kobold ambush, cave, and throne-room combat waves with deliberate cover placement instead of generic random encounters.
- Added Varkh, Kobold King as a boss template with unique stats, sidebar tactic text, fallback sprite support, and a rally special that shields nearby kobolds.
- Added old-save/world repair to make sure a Dusk Market cave marker exists for the kobold route.

## v0.51.0 - Scaffolding Fill and v0.52 Art Intake

- Added runtime support for the new v0.52 enemy sprite, spell animation, and world map prop atlases.
- Enemy combat board sprites and enemy roster portraits now prefer `enemy-sprite-atlas-runtime-*` before older creature/world/boss fallbacks.
- Fireball and Meteor glyphs now prefer `spell-animation-atlas-runtime-*` before older spell-effect fallbacks.
- World map objects now prefer `world-map-prop-atlas-runtime-*` before older quest/world-object fallbacks.
- Earned level-up skill points are now spendable from Tavern Muster talent buttons; fresh recruits still get only small starter nudges.
- Hardened load-game repair so a malformed save rolls back to the previous in-memory state instead of leaving the game partially loaded.
- Updated the Unity Windows build helper to clean the versioned output folder before building, reducing stale package/docs/art risk.
- Kept save data at version 17 because this pass repairs scaffolding behavior without changing save fields.

## v0.50.6 - Combat Sprite Art-First Repair

- Confirmed combat action buttons now use the intended atlas lookup order: command icons, combat HUD, combat spellbook, spellbook, combat UI, magic UI, then procedural fallback.
- Stopped drawing legacy equipment, caster, wound, status-body, and sigil overlays on generated atlas combat sprites.
- Kept combat board essentials intact: active outlines, HP bars, status pips, hover cards, side panels, and readouts.
- Kept save data at version 17 because this pass changes presentation only.

## v0.50.5 - Tavern First Screen and Exploration Polish

- Normal startup now goes straight to the tavern instead of holding on a timed splash; the splash remains available for startup recovery/error cases.
- Reworked the tavern menu hierarchy with Begin the Old Road as the hero action, Settings as the home for SFX/motion controls, and Beta Lab as a visually separate tester door.
- Added a compact Tonight's Table party preview to the tavern first screen.
- Exploration no longer shows a large tooltip for unseen tiles; fog hover now only marks the tile.
- Replaced the empty exploration Enemy Combatants panel with a Location panel showing the current zone, danger wording, underfoot anchor, and local flavor.
- Tightened the exploration command bar with a compact compass and icon-led travel actions.
- Kept save data at version 17 because this pass changes presentation and flow only.

## v0.50.4 - Combat Overlay Cleanup

- Removed persistent role, range, caster, guard, ward, web, active, injured, and status-duration text badges from combat unit tiles by default.
- Kept the tactical essentials on the board: colored status frames, active-unit pulse, thin HP bars, small status pips, hover targeting, and side-panel details.
- Simplified the bottom combat command status so it no longer repeats move/action state already shown in the active readout.
- Kept save data at version 17 because this pass changes presentation only.

## v0.50.3 - Path-Aware Combat Movement

- Added a small BFS reachable-tile helper for combat movement.
- Movement highlights, hover previews, target badges, and actual movement now agree on the same path-aware movement cost.
- Units can no longer move through intervening combatants, trees, or stone cover when the landing tile is open.
- Added runtime intake aliases for the art thread's v0.51 drops: `combat-sprite-atlas-*`, `item-inventory-atlas-*`, and `combat-spell-effects-atlas-*`.
- Confirmed combat board clicks return early while Armory or Spellbook overlays are open.
- Kept save data at version 17 because this pass changes combat behavior only.

## v0.50.2 - Art Intake and Parallel Workflow

- Added latest-version art loading for runtime atlases in `Docs/ArtReferences/`; the game now prefers the newest PNG matching each known filename prefix and falls back to the prior pinned file.
- Added `Docs/ART_INTAKE.md` with naming prefixes, grid expectations, art quality targets, and handoff notes for parallel art-generation threads.
- Updated package metadata to v0.50.2 while keeping save data at version 17.

## v0.50.1 - Tavern and Combat UI Readability Repair

- Changed the tavern landing customization button to Customize Party and removed the party-summary line plus route footer from the first playable screen.
- Increased the default window target to 1920x1080 for larger board and sprite presentation.
- Gave combat more horizontal room by narrowing the sidebar cap and increasing combat command button size.
- Restored readable hotkey badges on combat command icons and added aliases for 1-6 plus Z/X/C, F/G/H/R/T, and Space.
- Simplified combat Party cards so they show level, race, class, HP, and MP without equipment text crowding the sidebar.
- Added a generated `combat-terrain-atlas-runtime-v0.50.1.png` sheet for grass, snow, dirt, stone, cave, sewer, ice, web, scorched ground, and red-basalt combat tile texture variants.
- Replaced fragile small generated command/sprite overlay icons with clearer procedural glyphs and text badges.
- Enlarged combat unit sprite frames, status badges, and status duration markers so tile overlays are easier to read.
- Expanded combat hover cards with action state, target identity, HP/status, tile/cover notes, and click instructions.
- Kept save data at version 17 because this pass changes presentation and controls only.

## v0.50.0 - Runtime Graphics Expansion: Creatures, Commands, and Loot

- Added three generated v0.50 runtime sheets under `Docs/ArtReferences/`: consumables/loot, combat command icons, and creature sprites.
- Wired the consumable sheet into the top resource strip, item icon fallback, and cache reward panel so gold, supplies, elixirs, scrolls, herbs, chests, and food have real icon art.
- Rebuilt cache reward presentation into a clearer reward card with icon chips for gold, supplies, and elixirs.
- Wired the combat command sheet into action buttons and hover help so Move, Attack, Cast, Guard, Elixir, and End Turn have clearer dedicated icons.
- Wired the creature sheet into party portraits, enemy roster portraits, and combat board sprites for stronger silhouettes across party roles, kobolds, ratfolk, drow, demons, undead, brutes, and fungus beasts.
- Removed the party preview from the tavern landing screen and left party editing behind Customize Party so the tavern backdrop is no longer buried under management panels.
- Shortened the tavern, muster, exploration, and combat top chrome so it takes much less vertical space.
- Reworked combat action buttons into large icon-only controls with hotkey/cost/availability details moved into hover tooltips and the status readout.
- Kept older art/procedural renderers as fallbacks and kept save data at version 17.

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
- Added generated icons to the tavern menu buttons for Start Game, Customize Party, Beta Lab, Settings, and Exit Game.
- Reworked combat command buttons to be icon-first, with larger action icons, tiny hotkey badges, and compact state labels instead of word-heavy buttons.
- Routed several enemy and world-object visuals through the new enemy/world-object atlas, including kobolds, ratfolk, drow, demons, caves, caches, shrines, final gates, tree cover, and stone cover.
- Improved floating combat text stacking so repeated events on the same tile separate into small lanes with readable backplates.
- Save data remains version 17 because this pass changes presentation, art loading, and procedural audio only.

## v0.48.0 - Victory Route and Final Gate Completion Scaffold

- Added an early tavern route preview panel for the beta finale path, later removed from the landing screen in v0.50.1.
- Expanded late-route story objectives so depth 5 and depth 6 point more clearly toward the Red Gate and final ritual.
- Changed final-boss victory flow so defeating the meteor-crowned encounter ends combat and opens a dedicated beta victory screen.
- Added a victory ledger with survivor count, average level, gold, reached depth, party member rows, and a compact chapter recap.
- Added victory-screen buttons for New Party, Tavern, and Beta Lab so the end of the beta route loops cleanly back into testing.
- Save data remains version 17 because this pass adds route flow and UI, not new saved fields.

## v0.47.2 - Combat UI Readability Pass

- Increased the default window target to 1800x1040 when space allows.
- Replaced the odd in-game top-left header token with a cleaner Ashen Halls mark.
- Renamed the combat side panels to Party and Enemy Combatants.
- Gave combat a dedicated grid sizing helper so the turn queue/readout no longer crowd or overlap the tactical board.
- Enlarged combat sprites by reducing board-cell padding.
- Made side-panel party/enemy rows taller with larger portraits, meters, and readable headers.
- Rebuilt bottom combat buttons as wider commands with larger icons, labels, and sublabels instead of tiny icon art under text.
- Enlarged unit overlay badges and status duration chips so class/range/state marks are more useful at the new resolution.
- Save data remains version 17 because this pass changes UI presentation only.

## v0.47.1 - Tavern Landing and Spellbook Overlay Cleanup

- Added a dedicated post-splash Midgaard tavern landing screen with Start Game, Customize Party, Beta Lab, Settings, and Exit Game.
- Moved the character builder behind Customize Party so the first playable screen no longer looks like the debug/customization editor.
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
- Loaded `item-icon-atlas-runtime-v0.43.png` in the Armory party rows, pack rows, and cache loot panel.
- Loaded `enemy-roster-atlas-runtime-v0.43.png` for richer enemy side-card portraits.
- Added ratfolk scrappers, cutthroats, plague mages, cistern clerics, and brutes.
- Added drow scouts, blade dancers, crossbows, mages, and priests.
- Added lesser demons as Red Gate/Beta Lab pressure enemies.
- Updated zone encounter pools and Beta Lab spawning so the new families can be tested immediately.
- Save data remains version 17 because this pass changes art loading, encounter tables, and enemy templates without changing saved schema.

## v0.42.0 - World Zones and Story Scaffold

- Renamed the home town to Midgaard across the live game UI and exploration flow.
- Added named world zones with danger ratings, summaries, and one-time story discovery entries.
- Added chapter objective text that updates as the party descends.
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
- Loaded the class-icon atlas in game and drew class icons in the tavern roster, tavern portrait preview, party cards, turn queue, combat unit-frame badges, and Armory party tab.
- Kept text class abbreviations beside tiny combat icons so class identity remains readable if the art is small or missing.
- Added visible spell tier labels: starter, apprentice, adept, and elder.
- Surfaced spell tiers in combat spell cards, the selected spell detail panel, and the Spell Reference as a scaffold for later level-gated spell learning.
- Save data remains version 16.

## v0.39.0 - RPG Scaffold, Class Identity, and Pact Summoning

- Reduced new parties to a 4-person tavern party for clearer customization and combat testing.
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
- Improved right-side combat layout on shorter windows with compact party/enemy cards that preserve the full eight-member roster.
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
- `Refill` restores party HP/MP, clears afflictions, tops up elixirs, and wards the party for testing.
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
- Made party cards, enemy cards, top resources, and the combat command bar more defensive on narrow or short windows.
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
- Press `I` to inspect party gear and pack loot.
- Press `C` to open directly to the formula codex.
- Added party gear summaries for weapon range, damage type, hit/power modifiers, armor, guard, weight, and magic warding.
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
- Added keyboard fallbacks for Quick Start and Begin Party.

## v0.16 - Itemization and Customization

- Expanded generated weapons, armor, forms, materials, traits, and combat effects.
- Added cache loot comparison feedback.
- Added clearer party role identity, gear/look rerolls, and party summary.
- Kept audio sparse with simple one-shot sound effects and no music.

## v0.15 and Earlier

- Built the main vertical slice: muster, exploration, tactical combat, formula magic, generated loot, larger world map, enemies, terrain, save/load, and packaged Windows builds.
