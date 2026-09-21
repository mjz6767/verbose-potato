# Combat spell and skill effects

This pass coordinates with the active world-map audit in the canonical project. Combat rules and balance are unchanged.

## Changes

- Spell and skill impacts crest near contact, then settle into a softer tail. Smaller duplicate art leaves units and their grid cells easier to read.
- Spell anticipation uses contained arcs. Martial release and strike accents follow the attacker’s direction. Projectile details distinguish fire, frost, lightning, void magic, arrows, and knives.
- Eight elemental release textures distinguish spell families; shadow and demon skills use related textures. A separate soft bass tail replaces duplicate low-hit rumble.
- The sound pool allows at most eight simultaneous cues. Primary impacts can replace decorative layers, retain a short protected attack, and keep their gain when the mix is busy.
- Fast projectile releases preserve the minimum travel duration and arrive with the impact sound. The older feedback capture’s 1.30-second lead-in is also preserved.
- Reduced Motion retains immediate static impact stamps, suppresses animated travel, and uses compact audio. Mute remains respected.

## Review tools

The Beta Lab’s Visual-only Tour previews individual effects. Its existing controls include Q/E to change powers, C for audio, and V for Reduced Motion.

For repeatable player captures, combine `-ashen-feedback-smoke`, `-ashen-capture <absolute.png>`, and `-ashen-capture-quit` with:

- `-ashen-feedback-power FBL` (or another ID in `CombatVfxShowcaseRules`)
- `-ashen-feedback-phase cast|travel|impact|aftermath`
- Optional `-ashen-feedback-reduced-motion` with the impact phase

Unsupported or missed phases fail explicitly. The capture restores the previous time scale after its frozen snapshot. Visual smoke launches block campaign persistence.

`Tools/InvokeCombatEffectsCapture.ps1 -PlayerPath <absolute.exe>` runs ten representative phase captures. Add `-Visible` for a supervised review on desktops that do not render hidden game windows. Blank images and capture errors are rejected.

`AshenHalls.Editor.AuditPlayerBuild.Build` creates a Development player in a unique `QA/audit-players` folder, copies the external runtime artwork using the release selection rules, and logs `AUDIT PLAYER BUILT: <absolute.exe>`. Existing release packages and version metadata are preserved.

## Validation

Three smoke suites are included in the combined project audit: `CombatEffectsQualitySmoke`, `CombatAudioPolishSmoke`, and `CombatImpactReadabilitySmoke`. They cover catalog-wide runtime timing, effect cleanup and limits, Reduced Motion and mute, unchanged gameplay randomness, impact readability, sound sample validity, and voice priorities.

The audio suite exports `QA/combat-effects/audio-preview.wav`, an offline stereo rendering of six sound plans: Fireball, Cold Lance, Rift Bolt, Charge, Whirlwind, and Volley. This preview uses the actual sound bank and planned layers; it does not simulate live voice competition.

- Full combined audit passed: `QA/project-audit/20260908-031655-c0d90e4e/unity.log`. Rules, inventory/loot, sprite art, combat UI, and runtime boot all passed; the effects suite exercised 81 powers at all three intensities.
- Corrected Development review player built successfully with external artwork: `QA/audit-players/20260908-073456-f3f27056/AshAndBrimstone.exe`. Build log: `QA/world-map-audit-2026-09-07/audit-player-build-with-art.log`. The previous `20260908-031816-d17c4c31` scratch build omitted these external assets and should not be used for visual review. The build helper now shares the release artwork-copy routine; the corrected output contains the combat travel, aftermath, spell, and character atlases.
- Hidden capture was rejected as `NearUniformBlack`: `QA/combat-effects/20260908-031851-78b1e572/fbl-cast.log`. This is failed capture evidence, not a visual pass.
- Direct Windows review could not proceed because the Computer Use app approval timed out. Live visual verification remains pending. No accepted screenshots were produced in this pass.
- The ambience runtime fixture now clears audio from preceding test actions before simulating its quiet window; it still verifies actual ambience dispatch. A stale nearest-junction readout assertion was updated to the coordinated map task's grid-range wording; selected route assertions still verify actual walking steps.
