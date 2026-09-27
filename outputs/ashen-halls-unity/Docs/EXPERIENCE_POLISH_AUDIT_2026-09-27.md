# Experience polish audit — 27 September 2026

## Scope and baseline

This improvement pass began as a local preview on source `f956ed7` (verified v2.36.0).
It covers skill/spell sound effects, world-map navigation and scenery, and acquired
loot presentation. Save schema v27 and the published retail archives are preserved.
The initial audit player retained its baseline identifier. After preview review,
the user authorized final adjustments, v2.37.0 packaging and one combined push to
`mjz6767/verbose-potato` main. The final release is named Roads and Resonance.

The baseline retail player was captured at 1280×720 with seed 2525:
`QA/experience-baseline-20260927-220546/{loot,town,route}-1280x720.png`.
All three capture processes exited successfully and reported complete images.
Fixtures block save writes and legacy-save imports.

## Findings and implemented changes

### Sound effects

Eight school releases lacked imported masters. Some skills reused their preparation
as contact or added a second shot/cloud after a single action. Twenty-one original,
deterministically authored PCM clips now give the school releases and tactical
actions clearer material identities. Eleven skill plans use two to four cues;
reduced-audio, mute, volume and existing demon-signature routing remain intact.
The bank adds 568,284 WAV bytes. No external audio recording or service was used.

The first integrated audit also exposed a genuine latent fallback defect: float32
rounding makes sine at the final envelope sample slightly negative. Raising it to
a fractional power produced NaN. Clamp the envelope lobe before that power and
check finite source and mixed samples instead of relaxing the headroom threshold.
The initial failed log is retained at
`QA/project-audit/20260927-222358-ee4b2460/unity.log`.

See [sound audit](SKILL_SPELL_AUDIO_AUDIT.md) and its per-master JSON measurements.
Before/after plan renders are deterministic previews, not gameplay recordings or
proof of subjective listening approval.

### Map navigation and graphics

Previously passive edge arrows now pan a quarter viewport, while PARTY recenters.
Painting and input use the same hit rectangles, respect overlays and consume the
click before map-tile selection. Hover explains the pan/recenter control instead
of inspecting the tile underneath, and clears when an overlay owns input.
Pan uses the viewport center on the changed axis,
so the first click also works when the selection is clamped against a map boundary.
Header labels describe off-screen route continuation, including distance from the
edge; they no longer imply that an off-screen marker is the party's immediate step.

One new original transparent environment sheet supplies eight natural clusters.
The ImageGen skill guided asset creation and preservation of the original alpha;
the approved NPC/building art was not regenerated. Sparse deterministic placement
protects paths, doorways, characters, threats, guidance, objectives and unknown
ground, and replaces a competing ambient prop. Redundant procedural rectangular
accents no longer obscure authored terrain. Existing scenery remains the fallback.

- [Original environment atlas](ArtReferences/world-environment-detail-atlas-runtime-v2.37.0.png)
- [Exact generation prompt and actual output geometry](ArtReferences/world-environment-detail-atlas-v2.37.0-prompt.md)
- [Measured alpha, gutters and hash](ArtReferences/world-environment-detail-atlas-runtime-v2.37.0-validation.json)

### Loot clarity

Loot is already awarded when this screen appears. The new layout treats it as a
receipt, not a second transfer: resource totals first, item/art/traits on the left,
concise equipment differences on the right, quiet storage confirmation and a clear
Continue action. Strategic losses and negative stat changes precede gains; complete
comparisons remain in Inventory. Resource-only rewards omit empty equipment wells.
Acquisition, item identity, ownership, deliberate review and save behavior remain
unchanged. See the [loot audit](Audits/loot-reward-receipt-audit-v2.37.md).

### Coordinated work

The other authorized Ashen Halls chat contributed completed Pack displaced-weapon
warnings and Edda/Maud aftermath dialogue. These changes share this preview but do
not add rewards or change saves. Its focused tests and 26 native content captures
passed under `QA/town-and-pack-followups/20260927-221948-385/` before the integrated
audit. Independent reviews are recorded in that directory. See
`PACK_SWAP_DISCLOSURE_2026-09-27.md` and `TOWN_STORY_FOLLOWUPS_2026-09-27.md`.

## Integrated verification

- All thirteen Full Unity gates pass, exit 0:
  `QA/project-audit/20260927-224209-70e4ec05/unity.log`.
  This includes the new audio, scenery, loot, navigation, Pack and story rules;
  positive runtime art loading; exact 231-master SFX admission; runtime boot;
  combat UI; and save-blocked Beta Lab coverage.
- Twelve native loot screenshots pass text-fit, exact-resource, default-focus and
  state-preservation checks at 960×600, 1280×720 and 1920×1080:
  `QA/loot-experience/20260927-224148-671/`. An independent image review found no
  clipping/overlap or missing tradeoff in these fixtures (`visual-review.md`).
- A separate development player built successfully:
  `QA/audit-players/20260927-224332-f185b17b/AshAndBrimstone.exe`.
  Build log: `QA/experience-player-build-20260927-224329.log`.
  Runtime assembly SHA-256:
  `B48EE636D26281D3DA5086AA3B9AFE35AA88B422DF92795BDB88F95C8A97C31C`.
  Its environment atlas matches the approved source hash exactly.
- Seven actual-player captures at 1280×720 pass deterministic image/content/log
  validation in `QA/experience-polish/20260927-224532-8d963636/`:
  loot, town, route, forest shrine, fen grove, quarry and town Region Map.
  Capture-set SHA-256:
  `598ab88b278363b7194b036ea1a97763f34f4f30810d7540caf6bd52ee458c80`.
  The acquired-loot capture is independently reviewed against the retail baseline;
  required review stays visible and no quick-equip bypass appears. Its repeated
  bottom note is a smoke-only fixture string, not a routine acquisition message.
- The published v2.36.0 archive remains byte-identical, SHA-256:
  `2103174AE826006E66B63605635A1F15B779BA11ED239733AC1B3AA062BC7464`.

Retained diagnostic attempts: `20260927-223704-b0e50677` exposed an enlarged
headless navigation fixture overlapping the real sidebar; test geometry now fits
the free board, with production input ownership unchanged.
`20260927-223959-a95e75ab` reached runtime boot and caught the old sound-bank count;
the exact expected count now includes all 21 additions. The first hidden-player
capture `QA/experience-polish/20260927-224422-22beadd6/` was correctly rejected as
uniform black (exit 2). The approved visible-player rerun renders normally; the
rejected image is not validation evidence.

The equivalent seven-image 1920×1080 set also passes in
`QA/experience-polish/20260927-224702-5e6e60ae/`, capture-set SHA-256
`fb7d297c87fa34854e2c02a32d6f43c4d9d1cd227d73de341133ba60f2b4239c`.
Independent direct review of all twelve map images across both sizes found no new
clipping, white fringes, fog leaks, obscured interactions or overlapping headers.
Sparse clusters were legible at both sizes; remaining path geometry prompted the
bounded final polish below.

Live save-protected map testing using the computer-use skill verified first-click
east panning, repeated panning, the off-screen PARTY control returning to the
original view, correct control-hover wording and Tab returning to Local Map.
Log: `QA/experience-live-map-20260927.log`. The user stopped the first loot-input
attempt; no success was claimed for that attempt. Desktop control was explicitly
reauthorized for the final release checks.

## Final release adjustments

- Soften only noncivic earthen path strokes (shoulder opacity ×0.55, core ×0.85).
  Remove an apron only when both connection masks are empty and the tile is not
  civic, bridge or threshold. Keep widths, full cardinal topology, road textures,
  landmark/guidance markings, collisions and saves unchanged. Regression coverage
  enumerates 6,144 view/role/connection combinations and preserves the fallback.
- Remove the repeated smoke-only loot note. Add actual production-bound Submit
  tests for an available equipment upgrade and resource-only receipt. Assert the
  complete state is unchanged, the original equipment remains equipped, the reward
  stays in inventory, and a stale second Submit cannot award or equip anything.
- Advance package and Unity settings to v2.37.0, retaining save schema v27.
- Source-only compilation passes for 175 runtime and 75 editor files with zero
  diagnostics (`QA/source-compile/20260927-230303-27d7f6d4`). Four Python sound
  regression groups and exact read-only master reproduction pass again.
- The final v2.37.0 source passes all thirteen Full audit gates, including the
  expanded road and real-submit regressions, exit 0:
  `QA/project-audit/20260927-230445-5b091473/unity.log`.

Final Full audit, package hashes, clean extraction and release-player evidence are
recorded after packaging in `ReleaseEvidence/v2.37.0-summary.json`. Earlier preview
evidence is not a substitute for final artifact verification. Subjective audio,
physical-controller feel and a full human campaign remain playtest limitations.

## Packaged candidate and publication hold

Clean source `93eed753817d638c48264cffbc3e13e4e5a263cd` built successfully and
repeated all thirteen build gates. The clean-extracted player booted with exit 0;
all 375 ZIP entries passed CRC verification and all 103 packaged PNGs matched the
ZIP, staging folder and source. Candidate ZIP SHA-256:
`2bd420707f05724e676d09f464ec18276504f6716702e5ef9c09d1e82fdbd46e`.

Fourteen retail captures subsequently passed, with every process exiting zero:
`QA/experience-polish/20260927-231846-924eca0a` (720p) and
`QA/experience-polish/20260927-232018-e1c7af38` (1080p). Independent direct inspection
of all twelve maps and both loot images found no new clipping, white fringes,
overlap, hidden interactions or fog leaks. Connected geometric paths remain,
intentionally softened rather than erased. The real Return-key Continue action
also dismissed the save-protected loot receipt correctly on the desktop.

**Publication is held, not complete.** Two earlier retail capture runs exited
with native `0xc0000005` after successfully writing their images. Ordinary Alt-F4
closure of the desktop input test reproduced the same failure without taking a
screenshot. Exact local PDB lookup resolves UnityPlayer RVA `0xB1CBD1` to
`RuntimeStatic<PlatformAccessibilityManager,0>::StaticDestroy +0x51`; the packaged
and installed engine binaries have identical hashes. Managed logs contain no
exception. Thus the failure is not safely dismissible as screenshot-writing or
loot callback behavior. Later script-launched passing runs do not establish
accessibility-interacted shutdown safety.

The symptom and symbol match [Unity's documented UUM-146676 fix in 6000.3.21f1](https://unity.com/releases/editor/whats-new/6000.3.21f1):
a Windows shutdown fault after UI Automation interaction. That is an evidence-based
inference, not a locally verified fix: only 6000.3.18f1 is installed. No speculative
renderer flags, accessibility disabling or replacement screenshot code were
shipped. The user was asked to approve installing the engine patch/retesting or
explicitly accept publication with this known limitation. No remote push occurred.
Full candidate evidence, failed-run paths and remaining limits are preserved in
`ReleaseEvidence/v2.37.0-summary.json`; the immutable candidate ZIP still contains
its earlier source-checkpoint documentation.

### Post-reboot bounded retest

After the user restarted Windows and requested another attempt, no old game
processes remained. One save-protected retail test with a 60-second safety deadline
responded promptly to Return (Continue) and Tab in both directions (Region/Local).
No gameplay freeze was observed in this short test. Normal Alt-F4 closure still
exited `0xc0000005` before the deadline, so no forced kill was required.
Windows recorded native faults at `0xB2B2B5`
(`PlatformAccessibilityNodeRoot::get_BoundingRectangle +0x25`) and `0xB1CBD1`
(the previously identified accessibility-manager teardown). Local symbols were
used again; no engine installation or source workaround was applied. No game
process remains. Log: `QA/post-reboot-retail/20260927-233752/live-loot-map.log`.
The restart did not resolve the publication blocker. This result does not prove
that every freeze reported by the user has the same cause.
