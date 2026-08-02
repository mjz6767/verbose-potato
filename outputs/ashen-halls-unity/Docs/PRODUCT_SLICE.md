# Ash & Brimstone Playable Slice

Ash & Brimstone's first playable loop is a four-person tactical dungeon RPG slice: muster a party, begin inside Town Hall's expanded Grand Hearth gathering chamber, cross its broad floor among six patrons, leave through the storm doors to begin the journey into Midgaard, accept the sewer contract, clear three authored sewer encounters, make one recovery-room gear choice, return proof, earn a visible equipment reward, then follow the opened Old Road from Sluice Steps through Dusk Market and the bounded kobold Chapter II rooms.

## Normal First-Play Content

- Party: Warrior, Ranger, Mage, Priest.
- Priest formulas: `OIC` Heal, `TBQ` Ward, `GBH` Tree Cover, `NVC` Cleanse.
- Mage formulas: `FIF` Fire Spark, `WBI` Ice Slick, `RIG` Shock Bolt, `FBL` Fireball.
- Martial abilities: Warrior Charge, Warrior Rally, Ranger Aimed Shot, Ranger Pinning Shot.
- Enemies: Sewer Rat, Giant Rat, Ratfolk Scrapper, Ratfolk Cutthroat, Ratfolk Plague Mage, Ratfolk Brute.
- Encounters: Broken Sluice, Foul Runoff, Cistern Den.
- World: fresh 58x46 maps with eight deterministic room-sized regional sites, one in each outer territory, connected to the named road circuit.
- Opening chamber: Town Hall's Grand Hearth is a 10 by 9 room with one continuous 8 by 7 gathering floor. It preserves the original first-spawn cell, outbound storm doors, clear company runner, stable IDs, cartography table, blue company road chest, and portal flow.
- Grand Hearth art: v2.7 exact-pins one opaque 3 by 2 floor atlas and one transparent 3 by 2 set-piece atlas. Dark civic wood surrounds an oxblood-and-charcoal company runner and medallion, heat-darkened hearth apron, and storm threshold; the room fixtures receive dedicated monumental-hearth, storm-door, register, banner, rain-blue-window, and stores art. The shared v1.61 interior sheets remain fail-closed fallbacks.
- Grand Hearth finish: floor cells 0/1 use the accepted edge-balanced ImageGen refinement with updated prompt/validation provenance, while fallback setpieces remain footprint-gated and assertion-covered. Six Local/Region built-player views at 960 by 600, 1280 by 720, and 1920 by 1080 confirm one open, seamless, unobstructed gathering hall.
- Grand Hearth atmosphere: v2.8 adds a transparent six-cell companion for hearthlight, storm-door spill, wall sconces, ember haze, window reflection, and patron contact shadows. It is deterministic, non-interactive, and opacity-restrained in Region Map; geometry, route guidance, and gameplay are unchanged.
- Gathering patrons: six authored, deterministic figures reuse approved v2.4 citizen-atlas cells inside Town Hall. They are a separate presentation layer from procedural exterior ambient citizens: walk-through, off the runner, and without a `MapObject`, hover response, Talk prompt, dialogue, collision, or saved identity.
- First journey step: fresh-game objective and NEXT guidance remain on the Town Hall storm doors until the party leaves; only then does the campaign route advance to King Halvard.
- Chapter II route: Sluice Steps descent, Dusk Market kobold ambush, Smoke Cave, and the Kobold King rooms after the sewer reward.
- Recovery choice: +2 Sluicekeeper Blade or +2 Stormglass Focus after Foul Runoff.
- Recovery valve: retreat from a campaign fight for one supply, abandon its loot, and regroup at Temple Square.
- Tactical correction: undo uncommitted movement once or several steps into a turn, then choose a better position before spending the action.
- Targeting recovery: cancel an armed spell or skill from the command deck, with Esc, or by right-clicking the board without spending the action or disturbing movement.
- Combat presentation: a short pointer dwell keeps book hover Preview intentional, Selection and armed Targeting retain separate rails, command tags name BLOCKED / ARMED / NEXT, and melee or ranged actions show the correct sword or bow emblem. Layered impact art, priority/coalesced sound, stable encounter music, normalized sprite footprints, faction rims, and a static Reduced Motion stamp improve feedback without changing combat outcomes.
- World readability: buildings use clearer ground contact and quieter surrounding decoration; movement cues distinguish open ground, adjacent-use features, hard stops, and enemies; large setpieces and habitats remain readable near viewport edges; and ambient citizens yield visually to the party and authored interactions.
- Early progression: level 20 is the initial cap, reached after 8,740 total XP on a steady 100-to-820 threshold curve. Sunder, Shadowstep, Quick Shot, Dawn Pulse, Cinderstorm, Grave Hook, Soul Veil, and Ashen Curse extend the existing ladders with dedicated book art. Save schema remains v25.
- Original score: 53 reproducible masters cover the 60-second title overture, Grand Hearth, exploration, every authored regional site, encounters, victory, and defeat. Missing imported music still falls back safely.
- Golden Thread: one campaign-derived guidance plan drives the persistent NEXT rail and a bounded on-map route with an exact next-step key, visible arrival or viewport continuation, correct Midgaard interior targets, and stronger explicitly marked Journal waypoints—without modal tutorials or automatic walking.
- Party growth: the v2.5 Armory adds a fifth `I > Growth` tab. It stages earned attribute points and only class-relevant talents, previews the resulting party values before commitment, applies or resets the complete draft explicitly, checkpoints immediately after Apply, and remains review-only during combat. Unconfirmed choices are discarded when the Armory closes.
- Progress protection: quiet checkpoints before each room and after each permanent reward.

The bounded kobold Chapter II route is normal playable content. Other prototype classes, encounters, services, factions, dungeons, and generic route scaffolds remain gated behind the full prototype content set. Existing saves retain their serialized map dimensions, biome layout, party stats, talents, and unspent growth points; out-of-range levels and overflow XP normalize safely at the level-20 cap, and save schema remains v25. Steel, Sorcery, and Living Roads v2.9.0 is the current integrated family-playtest candidate. Combined RuleSmoke, focused combat-UI runtime smoke, SpriteArtRuntimeSmoke, and full RuntimeBoot pass. The final Windows build, built-player visual matrix, canonical packaging, clean-extracted packaged boot, physical-controller review, and complete human playthrough remain pending gates.
