# Skill and spell sound audit

## Scope and findings

This local polish pass builds on the v2.36.0 sound bank. It does not replace
music, the established large spell impacts, demon signatures, UI sounds, or
the player's mute, volume and reduced-audio preferences. No external service
or recording was used.

The source audit found that eight school-release keys still selected only
runtime-generated fallbacks. Their cast and impact counterparts already had
authored masters. Several martial profiles also used a preparation sound as
their contact, or added a misleading second action: Cleave landed with an
attack swish, Hamstring left a bow recording, single shots fired another bow
tail, and Smoke Bomb played the identical smoke cue at both stages.

## Changes

- Eight short original school-release masters distinguish flame turbulence,
  ice fractures, electrical arcs, wood fibers, light, shadow, rift pressure,
  and spatial air. Existing demon enhancement continues to select its own
  `demonhexrelease` and `demonriftrelease`; the corresponding base-school
  masters remain compatibility paths, not a claim that demon signatures
  were replaced.
- Thirteen tactical masters give Iron Rebuff, Cleave, Sunder, Hamstring,
  Broadhead Shot and Staggering Shot a material contact; provide a narrow
  throwing-knife release; and separate quiet shield/bow preparation from
  the louder action.
- Smoke Bomb is now one small primer followed by one pressure puff. Its
  duplicate cloud, status tone and shimmer layer are removed.
- Twinshot contains two release transients and two contact transients,
  separated by 85 ms. It no longer resembles an ordinary shot with an
  unrelated delayed bow sound.
- Eleven adjusted skill profiles use two to four total cues. Important
  contact remains in reduced-audio mode; mute and zero level remain silent.
  All new keys retain a procedural fallback if the imported asset is absent.
- Integration exposed a latent fallback bug: float32 `Sin(PI)` is slightly
  negative, so fractional-power smoke, stealth and whirlwind envelopes
  produced a NaN at the last sample. Four envelope expressions now clamp
  their nonnegative sine lobe before exponentiation. No mix/headroom check
  was weakened. The smoke gate now checks the entire resolved bank, every
  new fallback, twelve explicit endpoint fixtures, and every exported plan
  for finite samples and parameters.

## Reproducibility and measurements

`Tools/Audio/BuildSkillSpellSfx.py` writes only the twenty-one new original
masters and their own manifest/preview. `--validate-only` is read-only.
`Tools/Audio/TestSkillSpellSfx.py` exercises four regression groups, including
exact regenerated PCM, unique waveforms, the two Twinshot beats, school
spectral contrast and preparation/contact level separation.

The checked PCM masters are mono, 48 kHz, 16-bit, 0.13–0.50 seconds long.
Their peaks are at most -3.35 dBFS, RMS is -29.12 to -18.03 dBFS, endpoints
are zero, and mean DC offset is below 0.004. The bank adds 568,284 bytes of
WAV data (about 555 KiB). The prior 210 physical masters remain unchanged;
the complete physical SFX bank now contains 231 files.

`Docs/SKILL_SPELL_AUDIO_VALIDATION.json` records per-file SHA-256, sample
duration, peak, RMS, DC and spectral centroid. Measurements support safety
and repeatability, not a claim of subjective listening approval.

## Integrated checks and previews

`SkillSpellAudioSmoke.RunOrThrow()` verifies fallback and imported selection,
semantic mappings, bounded cue count, reduced audio, mute, zero volume and
master health. It also exports representative plan mixes and asserts raw
centered mix headroom before preview normalization:

- `QA/combat-effects/skill-spell-comparison-before.wav`
- `QA/combat-effects/skill-spell-comparison-after.wav`
- `QA/combat-effects/skill-spell-after.wav`
- `QA/combat-effects/school-release-after.wav`

The comparison order is Iron Rebuff, Smoke Bomb, Hamstring, Broadhead Shot,
Twinshot. The before comparison reconstructs their v2.36 source profiles
using preserved original clips with the legacy smoke endpoint repaired;
the after comparison uses current plans.
These are deterministic plan renders, not captured gameplay recordings.
An isolated bank preview and cue-start playlist are generated under
`QA/skill-spell-audio/` for convenient human audition.

The four Python regression groups and read-only exact master reproduction pass.
Unity import, finite sample/fallback checks, representative mix exports and all
thirteen integrated Full audit gates pass in
`QA/project-audit/20260927-224209-70e4ec05/unity.log`. The runtime resource check
now requires all 231 physical masters, including the 21 additions, and verifies
each imported clip replaces its known fallback. The eleven revised skill mixes
peak between 0.378 and 0.516 before normalization. Subjective human audition
remains separate from these deterministic checks.
