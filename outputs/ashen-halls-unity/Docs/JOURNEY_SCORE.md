# Everyday journey score — 26 September 2026

Eight frequently heard music cues now have separately authored eight-bar arrangements. Each moves through a melodic statement, an answering phrase, a quieter reflective passage, and a returning motif with a pickup into the next loop. The inner string voices choose close chord inversions, while plucks, flute, low strings, brass and percussion establish different places and encounters.

| Cue | Musical character | Seconds | Sampler start |
| --- | --- | ---: | ---: |
| `four_names_by_the_fire_loop` | Intimate lute, a soft flute reply and warm chamber strings at the Grand Hearth | 26.667 | 0.0 |
| `midgaard_lamps_loop` | A rounded flute theme, civic bell accents and moving plucked strings | 22.857 | 12.4 |
| `old_road_walk_loop` | A recognizable travel call, measured frame drum and open answering melody | 20.870 | 24.8 |
| `a_fire_between_roads_loop` | Sparse lute and flute over settled strings; no drum pulse | 25.263 | 37.2 |
| `combat_battle_pulse_loop` | A short bronze-horn call, accented short strings and restrained tactical drum fills | 17.143 | 49.6 |
| `sewer_hunt_combat_loop` | Clipped low strings, displaced rhythmic accents and isolated wet-stone bells | 18.462 | 62.0 |
| `kobold_hide_drums_loop` | Darting lute, asymmetric hide-drum replies and bone-click accents | 16.271 | 74.4 |
| `crown_and_ashes_boss_loop` | Broad minor horn phrases, low processional drum and a quieter string response | 20.000 | 86.8 |

All original cue IDs, exact frame counts, tempos, stereo PCM16 encoding and 32 kHz import contracts are preserved. The title and 46 other music masters, including the three existing demonic/arcane epic scores, retain their hashes. Existing Unity `.meta` files remain unchanged.

## Rebuild and validation

Run from the canonical Unity project:

```text
python Tools/Audio/BuildOriginalAudio.py --journey-score
python Tools/Audio/TestOriginalAudioContracts.py
python Tools/Audio/BuildOriginalAudio.py --check
```

The selective builder updates only the eight masters and their provenance rows, then refreshes the existing music sampler, World Map mix and combat mix from the current files. It also creates `QA/journey-score-2026-09-26/listening-preview.wav` and an accompanying JSON report. Each sampler entry contains eight opening seconds, a 0.2-second gap, then the actual final two seconds joined to the first two seconds to audition the wrap. The QA directory is intentionally ignored by Git; `Docs/JOURNEY_SCORE_VALIDATION.json` retains its digest, measurements and all 46 preserved-master digests.

The delivered scores measure -22.72 to -19.8 dBFS RMS, have at least 4.71 dB peak headroom, and retain their body when summed to mono. Loop discontinuities are below -62 dBFS. Two-bar phrase contrast ranges from 5.86 to 9.21 dB. Circular note and reverb tails preserve musical timing; a bounded 24 ms bridge removes sample discontinuities without rotating the downbeat.

The twelve contract tests include exact PCM reproducibility for chamber and combat arrangements, duration/mono checks for all eight deliveries, and deliberate faulty click, repeated-phrase and reversed-phase cases. The base bank audit covers its 54 loops and 106 base SFX; separately generated everyday SFX are allowed in the SFX directory and use their own validator.

These are original deterministic synthesized compositions without external recordings, samples or audio-model calls. Numerical checks verify the delivery contract; the supplied sampler allows subjective listening review, which is not represented as an automated test result.
