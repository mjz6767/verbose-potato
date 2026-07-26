# Ashen Halls Design Bible

This is a living note for keeping the game coherent as it grows. It should capture inspiration and decisions, not copied source material.

## Pillars

- Old-school party RPG readability.
- Tactical board decisions every turn.
- Class-specific magic with strange, memorable formula ancestry, but readable spell-card play during beta.
- Generated gear that feels abundant but still understandable through damage ranges, speed, traits, rarity, and stat bonuses.
- Four-character party building with race, class, level, stats, skills, equipment, and visible tactical identity.
- Modern pixel presentation: smoother motion, clearer UI, better silhouettes, sparse sound.
- Current title identity: Ashen Halls remains the project name; The Old Road works as the subtitle for this first playable arc.

## Party Framework

- New games use a four-person tavern party. Four is easier to read, easier to customize, and makes individual class identity matter more.
- Core stats are Strength, Intelligence, Agility, and Health. Keep the list tight unless a future system clearly earns another stat.
- Derived combat fields include HP, mana for spellcasting classes, movement, defense, damage range, attack speed, range, resistance, weakness, and status hooks.
- Attack speed is useful in turn-based combat as initiative, hit/crit pressure, and gear feel. It should not become real-time action speed.
- Level-ups grant XP progression, stat points, and skill points. Early beta spending is simple; later versions should add clearer level-up screens and class-specific spell unlocks.
- Save compatibility is intentionally relaxed during this scaffold phase. Old saves can be discarded until the core systems settle.

## Races

- Human: adaptable baseline and fast learner. Current scaffold gives bonus XP; future pass can add extra training flexibility.
- Dusk Elf: agile and accurate, good for rogues, rangers, and precise casters.
- Stoneborn: strong and hardy, slower but resilient, good for warriors and paladins.
- Fenkin: clever, mobile, and hazard-savvy, good for priests, rangers, and poison/web play.
- Ashling: high-intelligence fire-touched lineage, good for mages, wizards, and dangerous pact builds.
- Race abilities should be passive and readable at first. Activated racial powers can wait until the combat loop has more room.

## Classes

- Warrior: front-line physical class, strong armor, shields, axes, swords, spears, and guard pressure.
- Ranger: ranged pressure, sight lines, bows, light armor, terrain awareness, and later anti-beast/cave utility.
- Rogue: light striker, daggers/epees, high agility, crit pressure, bleed/stun traits, and later trap/cache expertise.
- Wizard: broad scholar-caster with access to multiple arcane crafts, but fragile and gear-dependent.
- Mage: focused elemental caster with fire, ice, shock, burn-cover, and terrain reaction emphasis.
- Warlock: dark-arts class using hex/death/pact magic. Warlocks can bind fragile demons; future upgrades should add stronger demons, pact costs, and risk/reward bargains.
- Priest: recovery, cures, wards, light damage, and terrain shaping such as Tree Cover.
- Paladin: hybrid front-liner with guard, armor, light wards, and limited priest craft.
- Specializations and hybrid classes should build from these eight, not replace them too early.

## Formula Magic

- The old three-key flavor remains useful as internal spell language and lore, but the current playable beta uses readable spell cards to avoid hotkey conflicts.
- Keep spells class-specific. Priests should not feel like mages with teal effects; warlocks should not feel like generic damage casters.
- The spell reference should show enough to cast confidently: craft, mana, range, target, sight rule, and effect.
- Focused casting rewards spellcasters who hold position before speaking a formula.
- Formula terrain reactions should be readable and tactical: fire burns cover and hazards, ice counters fire, and shock rides conductive hazards.
- Circle formulas should give menders occasional party-positioning decisions without becoming full inventory or buff maintenance.
- Tree Cover should preserve the memory of hiding behind generated trees, but it must not be infinitely abusable. It blocks direct arrows and bolts, arcing spells can pass over it, fire can burn it, and enemies can break through it.
- Warlock pact magic starts with Bind Imp: a fragile temporary ally that blocks lanes and attacks. Later pact spells can add stronger demons, self-costs, unreliable bargains, or anti-holy weaknesses.
- Class spell unlocks should eventually be level-gated. The current beta exposes many spells for testing; later versions should reveal them through level, class, and specialization.
- Spell tiers are the current bridge toward that progression: starter, apprentice, adept, and elder labels should teach the intended ladder before the game starts enforcing hard unlocks.
- Beta Lab controls are test harness tools, not lore. They exist to exercise combat/casting systems quickly until the rules are stable.

## Items

- Item names combine quality, material, form, trait, and bonus.
- Items should carry explicit gameplay data: damage minimum/maximum, attack speed, range implications, rarity, stat bonuses, damage type, and trait behavior.
- Forms should matter visually and tactically: reach weapons, bows, foci, light armor, heavy armor, robes, shields, epees, daggers, axes, staves, and spears.
- Traits should be small and readable: bleed, stun, warding, thorns, fire, cold, shock, poison, death, vampiric, and very rare vorpal-style finishing pressure.
- Strength should matter more for heavy weapons and armor. Agility should matter for hit/crit/initiative and light weapons. Intelligence should matter for mana, spell power, and focus gear. Health should matter for HP and durability.
- Loot panels should explain why a character equipped an item, not just that it happened.
- Armory inspection should explain current gear and pack history without becoming busywork.
- Manual equip should stay conservative until the game has a fuller inventory screen.

## Enemies

- Brutes should close distance and threaten fragile party members.
- Ranged enemies should seek sight lines.
- Caster-like enemies should prefer pressure, hazards, and vulnerable clusters.
- Deeper enemies should gain clearer markings, heavier silhouettes, or corrupted armor.
- Every enemy family needs a board-readable silhouette before it needs decoration.
- Veteran and elite ranks should be obvious from name and sprite markings, not hidden math.
- Support enemies should be rare enough to feel interesting and readable enough that the player can decide whether to focus them.
- Early encounter ladder: sewer rats first, then giant rats, then kobold cave scouts/raiders/slingers, then shamans and bone wizards as the player reaches harder cave/ruin fights.
- Loot quality and reward size should scale with encounter depth, enemy rank, and caster/brute danger.
- Kobolds should read as small, low, reptilian cave raiders rather than generic humanoids: snouts, tails, hunched stances, small horns, knives, slings, bone charms, and shield props.
- Kobold roles should be readable from the board: raiders lean forward, slingers have ranged posture, shamans carry a bone/staff silhouette, and shieldbearers are squat and armored.
- Kobold shamans and bone wizards should feel disproportionately dangerous for their size: visible staff/glyph silhouettes, hexes, webs, death splashes, and low HP that invites the player to prioritize them.
- Ratfolk should anchor the Midgaard cistern tier: scrappers and cutthroats swarm, plague mages pressure with poison/gas, cistern clerics heal, and brutes physically block lanes.
- Drow should feel like the intelligent deep-road threat: scouts and crossbows pressure positioning, blade dancers punish fragile backliners, mages use mind/hex pressure, and priests mix wards with dark rites.
- Lesser demons belong near the Red Gate and pact-magic edge cases: hot colors, brute pressure, fire terrain, bleed risk, and fearless behavior.

## Sprite Direction

- Sprites should stay board-readable at tactical scale before becoming decorative.
- Party sprites should visibly communicate role, equipment weight, weapon family, personal look, active turn, and wounded state.
- Enemy sprites should visibly communicate family, rank, damage type, ranged/caster threat, and special status pressure.
- Generated gear should leave small visual evidence on the character: armor weight, robe/cloak shapes, shields, foci, reach weapons, ranged kit, and trait marks.
- Procedural sprite upgrades should remain deterministic so the same character looks consistent across roster, portrait, and combat.
- Imagegen sheets are reference material. The in-game style should translate them into simpler layered pixel figures and eventually into edited LibreSprite sheets.
- Runtime imagegen atlas crops are a bridge, not the final sprite pipeline. They let testers see richer art now, while the long-term target is cleaned transparent sprite sheets with consistent scale, silhouettes, and animation frames.
- Combat-board sprites need stricter rules than concept art: one unit per cell, transparent background, consistent baseline, fixed grid slicing, and bottom-center pivots. Loose collage crops should only be used for reference or decorative UI, not tactical tokens.
- v0.43 adds a new transparent 4x4 runtime combat sheet and a larger enemy roster atlas. This makes the game feel dramatically less placeholder-heavy, but the next art pass should still hand-center, simplify, and animate the best silhouettes.
- Combat frames are part of sprite readability: gold active, red injured, green poison/web, violet mind/hex/sleep/stun, teal ward/recovery, and gray guard.
- Class identity needs dedicated icons. v0.40 loads `class-icon-atlas-runtime-v0.40.png` into tavern, combat, turn queue, and Armory UI while keeping short text labels as tiny-size backups.
- The tavern customization screen should eventually feel like choosing a party at a table: portraits, race/class icon tiles, gear preview, stat allocation, and compact party weakness summary.
- The first screen after splash should be a tavern, not a blank menu: visible bar, grog, patrons, band silhouettes, warm firelight, and customization panels layered over the scene.
- Earlier UI reference sheets remain useful: dark panels, readable gold/teal/ember/violet accents, compact tactical controls, and icon-first combat scanning are the visual objective.

## World Regions

- Midgaard: safe town, recovery, muster memory, old-road tone, and the first anchor for the campaign.
- Roads: connective tissue, camps, caches, ambushes.
- Ruins: tactical rooms, stairs, shrines, enemy variety.
- Deeper halls: stronger item materials, stranger terrain, corrupted enemies.
- Region labels should make the map feel authored even when it is generated.
- Hover look text should clarify play choices without becoming a quest log.
- Exploration objects should be identifiable without text: caches look like chests, shrines look sacred/vertical, encounters look dangerous, stairs look like descents, camps look warm/safe, and Midgaard remains the clear town marker.
- v0.41 loads `world-object-atlas-runtime-v0.41.png` for map objects. This is the preferred direction for generated art: clear atlas order, direct runtime use, procedural fallback, and later hand cleanup.
- v0.43 loads `world-environment-atlas-runtime-v0.43.png` for both world-map object icons and tile overlays, filling the v0.42 zone scaffold with water, roads, caves, walls, town gates, ruins, bridges, crystals, sewer grates, moss paths, and red-gate floor art.
- Exploration tile language should reinforce region identity: roads near Midgaard, moss and trunks near Green Shrine/Gloam, fen banks and dark water in the south, broken paving in market/gate ruins, quarry stone in the northwest, and glassy cold rubble in the northeast.
- Fog of war should feel like an old map emerging from dark rather than a flat black cutoff.
- Landmarks such as obelisks, ruins, bridges, and cave mouths should help the player remember places even before we add quests or shops.

## World Story Scaffold

- Chapter I: The Midgaard Cisterns. The party leaves the gate lamps, tests itself against rats and scouts, gathers supplies, and finds the stair below the Old Road.
- Chapter II: The Green Shrine Road. Broken pilgrim stones and warded ruins point toward the deeper halls.
- Chapter III: The Glass Warrens. Caster enemies, crystal sight lines, and stronger magic pressure should start shaping party tactics.
- Chapter IV: The Red Gate. Corrupted armor, war-road pressure, and dangerous enemy leaders should mark the late vertical slice.
- Named zones should change what the player expects before combat begins: cisterns suggest rats and damp hazards, quarry suggests stone caches, shrine roads suggest recovery and undead pressure, glass warrens suggest magic, fens suggest poison, and the red gate suggests elite violence.
- Zone discoveries should be short atmospheric entries in the Timeline. They set tone and direction without copying old hintbook or manual text.

## Audio

- Music is still mostly absent by design. The tavern is the one current exception: a small low-volume procedural band loop should make the party screen feel alive, while exploration and combat stay sparse.
- Sounds should be short one-shots with simple envelopes.
- Audio controls should remain simple: mute and coarse volume steps are enough.
- During beta, SFX needs an obvious test button so testers can distinguish muted/quiet/broken audio from sparse intentional silence.
- Visual SFX pulse feedback is useful during beta because it proves the game fired the sound even when OS/app output is misrouted.
- Prioritize: click, blocked, move, attack, cast, heal, tree/stone growth, cache, shrine, victory, defeat.
- Future world-map ambience should be location-based and modest: water near rivers, wind on high roads, low ruin rumble, cave dampness, and similar loops without turning the whole game into a continuous soundtrack.
- Silence is acceptable between events; it matches the sparse old-school feel.
