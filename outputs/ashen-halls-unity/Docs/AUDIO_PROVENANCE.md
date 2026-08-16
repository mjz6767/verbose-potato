# Ash & Brimstone Audio Provenance

## v2.15.0 World Map score and transport polish

`Ashen Atlas` is the score's 54th original stereo master and the first cue composed specifically for the wide World Map. The 76 BPM D-Dorian road arrangement uses the same deterministic local oscillator, resonator, colored-noise, envelope, and delay-network process as the rest of the score. No recording, sample pack, model-produced audio, generative-audio service, network service, or external input contributed to it.

The runtime transition pass changes presentation rather than source provenance. Context-shaped equal-power fades replace complementary smoothstep gains, mute leaves the current transport position alive, and bounded exploration dwell/release hysteresis prevents calm route chatter while retaining immediate pursuit, explicit World Map, and combat priority. Imported masters and deterministic procedural fallbacks remain independently selectable.

`python Tools/Audio/BuildOriginalAudio.py` now rebuilds and validates the full 54-music/106-original-SFX bank. A complete run writes the full score medley, complete title master, exact 20-second title interaction preview, exact 20-second World Map preview, and exact 20-second combat preview under the v2.15 QA prefix. The validation report records preview SHA-256 digests and requires the expected stereo format, duration, finite samples, and peak headroom. Together with the unchanged 55 curated CC0-derived effects, the Unity resource bank contains 161 authored SFX.

The `v2.12.1` title and `v2.12.2` combat labels below remain the provenance labels of those historical slices; v2.15 integrates them without renaming their original masters or evidence.

## v2.12.2 combat feedback and ambience polish

Eleven new 48 kHz mono masters replace important combat fallbacks: `combatstep`, `combatguard`, `combatturn`, `combatcrit`, `arrowrelease`, `thrust`, `spell`, `fire`, `combatambsteel`, `combatambsewer`, and `combatambarcane`. They are original deterministic local synthesis built from oscillators, resonators, physical-noise models, envelopes, and short reflections; no recording, sample pack, model-produced audio, network service, or external input is used. The release, contact, material, reaction, and critical layers now keep distinct spatial ownership so attacks audibly travel from attacker to target.

Combat ambience is encounter-aware and deliberately sparse. Sewer and ratfolk battles use water, chain, and enclosed air; arcane, drow, demon, and undead routes use an unstable magical field; other encounters use distant steel, shield movement, and footwork. With music audible, details wait at least five seconds and recur roughly every 13-18 seconds at low gain. Music-muted play fills the environment somewhat more often, while queued attacks, active music ducks, pauses, and a 1.5-second foreground quiet window suppress ambience so it never competes with action.

The mix policy also prevents a weaker overlapping hit from prolonging a stronger music duck, clears delayed combat layers when Reduced Motion is enabled, and replaces repeated long Tempest/Meteor secondary masters with compact resonance and low-impact beats. The representative runtime composite is peak-budgeted before it is written.

`python Tools/Audio/BuildOriginalAudio.py` rebuilds and validates the full 53-music/106-original-SFX bank. A complete run writes title previews plus a 20-second default-gain combat composite under `QA/ash-and-brimstone-v2.12.2-*-preview.wav`. Together with the unchanged 55 curated CC0-derived effects, the Unity resource bank now contains 161 authored SFX.

## v2.12.1 title mix and interaction polish

The title overture keeps its compatibility-stable basename, exact 60-second duration, D-Dorian harmony, and locally synthesized instrumental palette. Its opening envelope is stronger, sparse high-bronze reflections improve small-speaker readability, stereo staging is wider, and the first forged-road statement begins after the 0.72-second reveal response instead of competing with it. Runtime title gain is raised only for the title and muster routes; the rest of the score keeps its established mix.

Six new 48 kHz mono masters—`titleforge`, `titlereveal`, `titlefocus`, `titleconfirm`, `titleopen`, and `titleclose`—replace generic or combat-oriented menu feedback. They are deterministic oscillator/noise/resonator synthesis, consume no recordings or network input, and remain pitch-locked at runtime. The forged strike deliberately combines low weight with mid/high anvil and bronze energy so it survives laptop speakers. With music audible, the title ambience waits longer and uses only sparse quiet room/hearth details because rain and fire already live inside the music master; music-muted title mode restores the fuller environmental cycle.

That release contained 53 music masters and 95 original SFX. Its raw title master and 20-second default-gain interaction composite remain reproducible from the same generator.

## v2.9 title and regional score expansion

The v2.9 bank contained 53 original stereo music masters. `Ash & Brimstone` replaced the title cue at its compatibility-stable runtime basename with an exact 60-second, 24-bar D-Dorian overture. The forged-road motif answers the established title reveal, then passes among bowed strings, weathered lute, reed, and bronze horns before four staggered company voices converge over travel and battle drums. Hearth on the left and rain beyond the storm road on the right bind the score to the approved title painting. The music deliberately leaves the title screen's existing 0.28-second impact and 0.72-second confirmation one-shots undoubled.

Grand Hearth now routes to the quieter `Four Names by the Fire` reprise instead of replaying the title overture. Eight additional masters give the Green Shrine training ring, Old Quarry forge, Gloam Deep crypt, Glass Lore library, Dusk Market hideout, Red Gate seal, Salt Cistern gate, and Ash Fen ancient grove distinct calm identities. Alerted-patrol and combat priorities remain unchanged.

All ten v2.9 arrangements are original deterministic local synthesis. They use no recordings, samples, model-produced audio, generative-audio service, or network input. The bounded smoothstep loop bridge prevents seam-window overshoot while preserving quiet endpoints; validation checks both endpoint delta and boundary-window peak behavior. `--music-cue <cue>` remains available for focused iteration, while a complete run is required to refresh the manifests and previews.

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

`ORIGINAL_AUDIO_ASSET_MANIFEST.tsv` records every original cue, musical direction, format, duration, peak, RMS, seam delta, and SHA-256 digest. `ORIGINAL_AUDIO_VALIDATION.json` is the matching machine-readable report. A complete generator run writes a score medley, copies the complete title master for direct review of its arc/coda/loop, and renders a separate default-gain title interaction preview.

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

For v2.15, automated release checks must confirm that required keys and track names exist, all 161 imported SFX resolve over procedural fallbacks, all 54 music masters resolve over their procedural score contexts, every declared preview matches its recorded digest/format/duration/peak contract, samples are finite and bounded, representative combat layering stays inside its peak budget, loops meet endpoint and boundary-window budgets, and semantically different files do not collapse to identical waveforms. Human release QA must still listen through the complete title loop, title interaction preview, World Map preview, combat preview, mute/unmute continuity, and representative routed transitions at 25%, 65%, and 100% music/SFX volume for masking, repetition, spatial readability, transition balance, small-speaker clarity, and clean headroom.
