# Ash & Brimstone Soundtrack

The score contains 44 original compositions, all shipped as reproducible stereo masters in v1.78. Music crossfades through two Unity audio sources and is controlled independently from sound effects. Every context also retains its established procedural safety arrangement, so a missing or invalid master cannot leave a game state silent.

## Player States

| Context | Track |
|---|---|
| Tavern / title | The Brimstone Overture |
| Party Setup | Muster by Firelight |
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

Boss identity has highest priority, followed by the last-stand state and encounter composition. Major spell and ability impacts briefly duck the music without stopping it.

## Technical Contract

- `MusicDirectorRules` owns deterministic selection and priority.
- `BuildMusicClips` registers the complete score and its procedural safety bank.
- Exact-name files under `Resources/Audio/Music` replace matching eager themes, while a route-to-name map resolves adaptive themes without composing an unused fallback.
- Music source masters are 32 kHz stereo PCM16 WAV files imported as Vorbis-compressed in-memory clips; short effects retain their separate 48 kHz mono PCM contract.
- The v1.82 title arrangement expands its established tavern cue to a 36.9-second, twelve-bar form with a quiet opening, forged-road lute motif, bowed answer, central lift, and returning coda.
- Procedural themes remain available as deterministic fallbacks and are generated only if their imported master cannot be loaded.
- `MusicTransitionDuration` crossfades state, zone, landmark, pursuit, and combat changes.
- Save schema is unaffected; music mute and volume continue using the existing v23 settings.
