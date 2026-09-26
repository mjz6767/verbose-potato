# Everyday sound design

The everyday bank adds 26 original 48 kHz mono masters: six material-specific equipment sounds, a chest latch and lid, a rare-loot reveal, a region discovery motif, a quest resolution motif, and two extra articulations for each of eight walking surfaces. The existing base effects remain available. Demonic combat sounds are unchanged.

`Tools/Audio/BuildEverydaySfx.py` builds these files locally from deterministic colored noise, decaying resonators, envelopes and layered contacts. It uses no recordings, external service, or generated-audio model. Each stable cue-name seed reproduces the exact PCM master. The separate `EVERYDAY_AUDIO_ASSET_MANIFEST.tsv` records the format, peak, RMS, source path and SHA-256 of every new master.

Run the generator without arguments to rebuild, or pass `--validate-only` to audit saved masters. Validation compares every sample against a deterministic rebuild, checks uniqueness, zero-valued boundaries, headroom and audibility, and requires each walking variant to remain within 2.5 dB of its existing base master. That calibration matters because the runtime intentionally gives different surface families different gains.

The generator writes `QA/everyday-audio/everyday-sfx-preview.wav` at representative in-game gains, with cue start times in `preview-playlist.json`. The first ten cues are equipment, chest, rare loot, discovery and quest completion; the remaining sixteen are paired walking variants. Human listening remains useful for tone, repetition and small-speaker clarity.

`EverydayAudioSmoke` checks semantic equipment/loot selection, fallback registration, Unity resource resolution, sample health, distinct step articulations and surface loudness. New fallback keys are registered before imported overrides; missing variant masters reuse the matching base surface. Equipment selection responds to item form/material, rare or signature loot receives its own reveal, sealed caches use the chest sound, newly discovered regions receive a brief compass motif, and the rat-pelt quest reward gets a resolution phrase.
