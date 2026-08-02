# Ash & Brimstone Audio Provenance

## v2.9 title and regional score expansion

The current bank contains 53 original stereo music masters. `Ash & Brimstone` replaces the title cue at its compatibility-stable runtime basename with an exact 60-second, 24-bar D-Dorian overture. The forged-road motif answers the established title reveal, then passes among bowed strings, weathered lute, reed, and bronze horns before four staggered company voices converge over travel and battle drums. Hearth on the left and rain beyond the storm road on the right bind the score to the approved title painting. The music deliberately leaves the title screen's existing 0.28-second impact and 0.72-second confirmation one-shots undoubled.

Grand Hearth now routes to the quieter `Four Names by the Fire` reprise instead of replaying the title overture. Eight additional masters give the Green Shrine training ring, Old Quarry forge, Gloam Deep crypt, Glass Lore library, Dusk Market hideout, Red Gate seal, Salt Cistern gate, and Ash Fen ancient grove distinct calm identities. Alerted-patrol and combat priorities remain unchanged.

All ten v2.9 arrangements are original deterministic local synthesis. They use no recordings, samples, model-produced audio, generative-audio service, or network input. The bounded smoothstep loop bridge prevents seam-window overshoot while preserving quiet endpoints; validation checks both endpoint delta and boundary-window peak behavior. `python Tools/Audio/BuildOriginalAudio.py` rebuilds and validates the full 53-music/89-original-SFX bank. `--music-cue <cue>` remains available for focused iteration, while a complete run is required to refresh the manifests and previews.

## v1.82 title overture

In v1.82, `The Brimstone Overture` replaced the former 24-second tavern master at the same exact runtime cue name. That historical 36.9-second stereo arrangement had a two-bar bowed opening, a recurring forged-road lute motif, frame-drum entrance, bowed counter-line, ember-bell markers, one restrained distant-weather swell, a central dynamic crest, and a returning coda.

That v1.82 master was also entirely original deterministic local synthesis, with no recordings, samples, model-produced audio, generative-audio service, or network input. The current generator preserves the same provenance contract while producing the v2.9 arrangement described above.

## v1.78 original score and sound-design expansion

The v1.78 expansion introduced three complementary audio layers:

- 44 original stereo music masters cover every player state, Midgaard district, outer territory, landmark, pursuit state, hostile faction, combat escalation, Victory, and Defeat.
- 81 original mono sound-effect masters cover magic, terrain, routes, services, rooms, items, semantic interface states, turn handoff, level gains, material footsteps, and eighteen sparse ambience cues.
- The 55 v1.70 CC0-derived short effects remain unchanged, bringing the authored SFX bank to 136 files. Every cue and music context retains its deterministic procedural fallback.

No recording, sample pack, generative-audio service, model-produced audio, or Asset Store package contributed to the 125 original files. `Tools/Audio/BuildOriginalAudio.py` composes them locally from oscillators, deterministic colored noise, envelopes, inharmonic resonators, drum models, circular delay networks, and soft limiting. NumPy is the only non-standard dependency.

Music source masters live at `Assets/Resources/Audio/Music/<procedural-clip-name>.wav`. Their filenames intentionally match the established procedural score names, allowing the runtime to prefer a master while preserving the old clip as a fallback. Unity keeps the 32 kHz stereo WAV source files lossless in the project and imports them as Vorbis-compressed in-memory music for the player.

New effects live at `Assets/Resources/Audio/Sfx/<runtime-cue-key>.wav` and use the existing exact-basename routing contract. They are 48 kHz mono PCM files, no longer than 1.84 seconds. The v1.86 combat mix uses eight reusable voices so staged invocation, release, impact, reaction, and aftermath layers retain stable spatial placement.

`ORIGINAL_AUDIO_ASSET_MANIFEST.tsv` records every original cue, musical direction, format, duration, peak, RMS, seam delta, and SHA-256 digest. `ORIGINAL_AUDIO_VALIDATION.json` is the matching machine-readable report. A complete v2.9 generator run writes `QA/ash-and-brimstone-v2.9-music-preview.wav` in manifest order and copies the complete title master to `QA/ash-and-brimstone-v2.9-title-preview.wav` for direct review of its arc, coda, and loop.

## v1.70 shipping source

The earlier authored sound-effect bank remains intact:

- 55 short authored overrides are mastered from explicitly selected CC0 recordings and sound-design assets.
- Every other cue retained the original deterministic synthesis path in `Assets/Scripts/Legacy/AshenHallsGame.ArtAudio.cs`.
- Missing, invalid, or unloadable authored files automatically keep their procedural fallback.
- Music remained generated by the project's original procedural score system in v1.70.

The authored source material comes from lentikula's Basic Spell Impacts, Kenney's Interface Sounds, Impact Sounds, and RPG Audio, artisticdude's Battle Sound Effects, and StarNinjas' sword attack/clash packs. Each source is released under Creative Commons CC0 1.0. See `THIRD_PARTY_NOTICES.txt` for source and license links, and `AUDIO_ASSET_MANIFEST.tsv` for the exact cue-to-source mapping.

No generative-audio service or Asset Store package contributed samples to v1.70. The source packs were curated, transformed, and mastered locally with Python, NumPy, and FFmpeg.

## Reproducibility

- `Tools/Audio/BuildOriginalAudio.py` rebuilds all original music and sound effects bit-for-bit from stable cue-name seeds and checks file sets, format, headroom, audibility, uniqueness, and music loop seams. Its `--music-cue` option supports a focused deterministic title-track rebuild.
- `Tools/Audio/BuildAuthoredSfx.py` decodes only the selected source files, folds them to mono, resamples to 48 kHz, trims silence, layers composite cues, removes low rumble, applies short boundary fades, and soft-limits below full scale.
- Short mastered files live at `Assets/Resources/Audio/Sfx/<runtime-cue-key>.wav`; filenames are the routing contract.
- Unity imports the complete SFX bank as preloaded mono PCM with preserved sample rate. Runtime tests verify every override, peak/RMS health, and fallback retention.
- Synthesis seeds derive from stable cue names and semantic sound families.
- Rebuilding a cue with the same code and parameters produces the same waveform.
- Runtime pitch and stereo placement may vary within bounded presentation rules, without mutating the mastered source clip.

## Review expectations

Automated checks confirm that required keys and track names exist, all 144 imported SFX resolve over procedural fallbacks, all 53 music masters resolve over their procedural score contexts, samples are finite and bounded, loops meet endpoint and boundary-window budgets, and semantically different files do not collapse to identical waveforms. Human release QA should still listen through the complete title loop and all nine new in-world routes at 25%, 65%, and 100% music volume for masking, repetition, transition balance, and whether the title retains enough presence at the default setting.
