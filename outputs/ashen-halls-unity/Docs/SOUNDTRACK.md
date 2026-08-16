# Ash & Brimstone Soundtrack

The score contains 54 original compositions. The v2.9 expansion added a 60-second main-title overture, a quieter Grand Hearth reprise, and dedicated music for all eight authored regional destinations. The historical v2.12.1 title mix brightens and widens the overture's opening, clears its first motif from the reveal chime, adds six pitch-locked title interface cues, and lets the score carry its own rain and hearth before sparse room details enter. The historical v2.12.2 combat pass adds eleven original action and environment masters while protecting the score from crowded impact stacks. v2.15 adds `Ashen Atlas` as a dedicated World Map identity and stabilizes music routing with context-shaped equal-power fades, mute-continuous transport, and exploration hysteresis. v2.16 gives the Gloam Deep crypt, Red Gate seal, Ash Fen ancient grove, and elite combat distinct composition blueprints in both imported and procedural paths, while the routed World Map QA mix now exercises local road, overview, and pursuit at their runtime gains and fade timings. Every stereo master is reproducible from the local audio builder; every context also retains a procedural safety arrangement so a missing or invalid master cannot leave a game state silent.

## Player States

| Context | Track |
|---|---|
| Title / main menu | Ash & Brimstone |
| Grand Hearth | Four Names by the Fire |
| Party Setup | Muster by Firelight |
| World Map overview | Ashen Atlas |
| Victory | Embers Carry Home |
| Defeat | Ashes on the Road |

## Midgaard

| Context | Track |
|---|---|
| General city streets | Midgaard Lamps |
| Temple Square | Bells over Temple Square |
| Market Square | Lanterns and Ledgers |
| Tavern Lane | Wet Cobble Reel |
| City gates | Watchfires on the Wall |
| Sewer approach | Under the Bellstone |
| King's Hall approach | Banners Before the Crown |
| Throne Room | Midgaard Throne Room |
| Merchant Hall | Midgaard Merchant Hall |
| Safe outer road | Last Lamps East |

Local Midgaard themes activate near their associated landmark. Throne Room and Merchant Hall retain their room scores regardless of nearby NPCs.

## Outer Roads

| Context | Track |
|---|---|
| Inner Ash Road | Old Road Walk |
| Salt Cisterns | Salt Cistern Drips |
| Green Shrine Road | Green Shrine Teal |
| Dusk Market | Dusk Market Ambush |
| Old Quarry | Old Quarry Stone |
| Glass Warrens | Glass Warrens Shimmer |
| Ash Fen | Ash Fen Haze |
| Red Gate | Red Gate Omen |
| Gloam Courts | Gloam Courts Echo |
| Camp or waystone | A Fire Between Roads |
| Shrine | Old Green Prayer |
| Cave, stair, or dungeon gate | Mouth of the Deep |
| Ruin or crypt | Names Worn Away |
| Obelisk, lore site, or portal | Glass and Quiet Stars |
| Ancient grove | Roots Remember |
| Faction camp or encounter site | Smoke Across the Road |
| Alerted roaming patrol | Footsteps Behind |

Landmark themes require close proximity. Pursuit music applies only outside Midgaard and yields immediately to combat when the patrol engages.

Opening the wide World Map selects `Ashen Atlas` unless an alerted patrol already owns the more urgent pursuit route. Explicit overview entry and exit transition immediately; bounded dwell/release rules apply to local calm-route and pursuit changes so ordinary landmark movement does not churn the score.

## Authored Regional Sites

| Destination | Track |
|---|---|
| Green Shrine training ring | Sparks on the Oathring |
| Old Quarry forge | Anvil Echoes in Old Stone |
| Gloam Deep crypt | The Crypt Keeps Its Names |
| Glass Lore library | Starlight in the Glass Index |
| Dusk Market hideout | Lanterns under False Names |
| Red Gate seal | Embers at the Broken Seal |
| Salt Cistern gate | Chains below Bellstone |
| Ash Fen ancient grove | Old Sap under Ash |

The site-center score takes priority over its surrounding territory while the area is calm. An alerted patrol still invokes `Footsteps Behind`, and combat continues to take final priority.

## Combat

| Context | Track |
|---|---|
| General battle | Battle Pulse |
| Sewer rats | Sewer Hunt |
| Ratfolk company | Ratfolk Plague March |
| Kobolds | Kobold Hide Drums |
| Drow | Drow Nightblades |
| Demons | Red Rift War |
| Undead | Bones Beneath Stone |
| Unaffiliated spellcasters | Sigils Crossed |
| Unaffiliated elite foes | Steel Against the Chosen |
| Party at 35% total living HP or lower | One More Turn |
| Generic boss | Crown and Ashes |
| Kobold King | Crooked Crown |
| Demon-lord finale | The Rift Walks |

Boss identity has highest priority, followed by the last-stand state and encounter composition. Major spell and ability impacts briefly duck the music without stopping it. Weaker overlapping hits cannot extend a deeper active duck, and Tempest/Meteor secondary beats use compact echoes instead of replaying long primary masters.

Physical attacks now separate the attacker-side release from the target-side contact, armor/material response, creature reaction, and dedicated critical accent. Bow, crossbow, sling, thrust, generic spell, and fire paths use original masters instead of procedural-only fallbacks. Movement, Guard, and acting-turn handoffs also have distinct restrained identities.

Combat ambience stays below the foreground: sewer and ratfolk encounters use wet enclosed detail, arcane and supernatural routes use a quiet unstable field, and other encounters use distant steel and footwork. With score audible, these one-shots wait five seconds and recur roughly every 13-18 seconds; active attacks, impact ducks, pauses, and Reduced Motion transitions suppress or clear competing layers.

## Technical Contract

- `MusicDirectorRules` owns deterministic selection and priority.
- `BuildMusicClips` registers the complete score and its procedural safety bank.
- The legacy `Tavern` route remains an alias of the title route for compatibility; `GrandHearth` now owns a distinct in-world reprise.
- Exact-name files under `Resources/Audio/Music` replace matching eager themes, while a route-to-name map resolves adaptive themes without composing an unused fallback.
- Music source masters are 32 kHz stereo PCM16 WAV files imported as Vorbis-compressed in-memory clips; short effects retain their separate 48 kHz mono PCM contract.
- The v2.12.1 title arrangement remains exactly 60 seconds: 24 bars at 96 BPM in D Dorian. Its brighter forged-road motif begins after the dedicated reveal chime, four instrumental voices represent the company at the threshold, and wider bowed strings, weathered lute, bronze horns, drums, hearth, and rain build toward the storm-road crest before a loop-safe coda.
- Title interaction uses dedicated forged-metal reveal, bronze response, focus, confirm, open, and close masters. These cues are pitch-locked to the score instead of receiving general-purpose runtime pitch variation.
- With title music audible, runtime rain duplication is removed and the first quiet room detail waits 7.5 seconds; muting music restores a fuller rain, room, and hearth ambience bed.
- The authored SFX bank contains 161 files: 55 curated CC0-derived effects and 106 deterministic original masters. Combat additions preserve 48 kHz mono PCM import and exact-name fallback routing; authored profiles cover all 56 formulas and all 25 skills.
- `CombatAudioMixRules` owns semantic direct cues, attacker/target stereo placement, sparse route-aware ambience, bounded epic echoes, and dominant-duck overlap policy.
- Procedural themes remain available as deterministic fallbacks and are generated only if their imported master cannot be loaded.
- `MusicTransitionRules` uses equal-power gains so a crossfade's midpoint does not dip, with faster combat entry and longer title, World Map, Victory, and Defeat transitions. Interrupted fades settle on the stronger audible source.
- Music muting leaves the selected clip and transport position alive; unmuting resumes the same score instead of restarting it. Calm exploration changes require a short stable candidate and minimum route dwell, pursuit enters immediately and releases only after its hold, and explicit World Map changes remain immediate.
- Save schema remains v25; the established music mute and volume preferences remain compatible.
