# Steel & Streets candidate — September 20, 2026

## September 26 continuation

The user requested a retry. Unity now works with access to its existing licensing service outside the sandbox; authentication/license activation was not automated or bypassed. The first licensed pass found one stale release-label assertion expecting v2.24.0. It was updated to the intended v2.25.0 without weakening any gameplay, art or input check.

The complete licensed Full audit subsequently passed: Rules, Inventory and loot, Sprite art, Combat UI and Runtime boot. Exact log: `QA/project-audit/20260926-143756-10536d9a/unity.log`. All newly integrated combat-decision and map-sprite regressions ran.

The final retail package was built from clean source `ed175db4d82ca1d468c466a5ada0f1ab69047a53`; all embedded gates and clean-extracted boot passed. All 92 source/staged/zipped art hashes agree. Both versioned destinations listed below now exist; the 229,997,861-byte ZIP has SHA-256 `cb6a0e2170646aa450b6ef3a0f02d0417caf76546f5dce9ad11f185dfb19e0db`. The previous v2.24 ZIP is unchanged.

Final-player review covers 33 accepted exact-size screenshots: 18 Local/Region NPC/landmark views at 960x600, 1280x720 and 1920x1080, ten combat-effect views at 1280x720, and five combat-decision views at 960x600/1920x1080. All five deterministic packets pass without warnings; subsequent assistant review found no release-blocking issue. Secondary Region metadata truncation and tall roofs/smoke reaching the board header remain cosmetic follow-ups. Static selected-power fixtures can retain the baseline Fireball/Cairn banner and damage float; they certify effect readability, not live labels or timing.

Real keyboard/pointer checks in the final visible retail player passed Local scroll safety, Region pan/return without party movement, Kate dialogue open/close, Details mouse/Q controls, E/Q combat target cycling, and Return attack confirmation with Action Used feedback. Only isolated save-blocked fixtures were used and their owned processes were closed. Physical-controller feel, subjective listening and a full human campaign remain unchecked. Exact provenance, capture hashes, observations and limits are recorded in `Docs/ReleaseEvidence/v2.25.0-summary.json`. This evidence is committed after packaging; the immutable ZIP retains its pre-verification documentation snapshot.

## Historical September 20 status

Implemented as a **source-only, unpublished build candidate**. Candidate v2.25.0 retains save schema 27. The previous combat-effects/world-map improvements are preserved and integrated. The user authorized committing and pushing this source checkpoint after the license-blocked handoff. That source push does not certify or publish a playable v2.25 build. The existing v2.24 and older release artifacts have not been replaced; no archive promotion has been performed.

At that checkpoint, Unity exited with code 198 before starting tests because it could not find an active Editor license. Both ordinary and approved outside-sandbox attempts failed for the same reason. The user was asked to reactivate the existing license through Unity Hub. Authentication/license activation was not automated or bypassed.

## New polish

- Movement previews, reachable counts and target cycling respect webbing, stun, sleep, remaining movement and valid destinations. An unavailable move no longer advertises a valid path/click.
- Armed skill highlights, counts and controller targets use the same readiness prerequisites as execution. Rejected input names range, mana, web, path, occupancy or action restrictions; compact badges no longer mistake “empty” for “MP”.
- Healing/splash feedback, Ascendance recovery logs and drain/Soul Rend forecasts respect recoverable HP. Original rolled splash strength and authored minimum drain recovery are retained; no balance changes.
- Named NPC artwork renders once instead of receiving a duplicate opacity/shadow pass. Foot-level open brackets replace body-enclosing focus boxes; focused Local services receive a small role badge and separate E prompt when space permits.
- Ambient citizens/interior patrons align with the named-NPC footline, with yielding kept within the terrain row. Atlas identities, art files, collision and dialogue behavior are unchanged.
- Region contact capture staging now recenters after moving the fixture party. `Tools/InvokeWorldSpriteCapture.ps1` captures four named contacts plus Region contact/landmark views and creates an acceptance packet. The earlier combat phase capture tool remains available.

## Historical September 20 verification performed

- Independent static second-pass review: no actionable defects found in grounding/focus/interaction ownership or combat readiness/rejection/healing arithmetic.
- `Tools/InvokeSourceCompile.ps1`: PASS, 138 runtime source files and 26 editor source files, zero diagnostics. Uses Unity's existing editor response configuration with refreshed source lists; outputs only under `QA/source-compile`, never into the live build cache. This is source compilation, **not** a Unity runtime test or build.
- Compile output: `QA/source-compile/20260921-035556-64fd3074` (UTC timestamp).
- New `CombatDecisionPolishSmoke` and `WorldMapSpritePolishSmoke` compile and are registered in `RuleSmokeTests`; their execution remains pending.
- Capture-script syntax check: PASS. Existing NPC sheets and an older-player baseline were visually inspected. The baseline request for 1280×720 was rejected correctly: the current sole display is 1080×1920 and Unity rendered 1080×720. No final v2.25 screenshots or wide-layout visual pass are claimed.
- Computer-use discovery did not expose the isolated game process as a targetable window. No live keyboard/pointer action was sent. The owned baseline process was stopped; no user campaign was written.

Failed licensed audit logs (test suites never started):

- `QA/project-audit/20260921-035411-718b8f88/unity.log`
- `QA/project-audit/20260921-035440-5701ffe4/unity.log`

## Original release-continuation checklist

1. Run `Tools/InvokeProjectAudit.ps1 -Suite Full`; fix failures without weakening acceptance.
2. Build an isolated audit player if visual iteration is needed. Inspect combat phases/target feedback and NPCs/landmarks at actually rendered sizes. Current display supports 960×600 and 1080×720; 1280×720/1920×1080 final-player checks require a suitable display. Automated layout geometry is not a replacement for those captures.
3. Replace the unpublished/license-blocked notes only with observed results. Review and commit any validation fixes and updated docs, producing a frozen release snapshot. Preserve the integrated combat/map audit work.
4. Preflight the exact new build target and run `Tools/BuildAndPackageWindows.ps1 -Version v2.25.0` with no dirty-source or skipped gates. Metadata already reads v2.25.0 / bundleVersion 2.25.0 / save 27.
5. Verify final retail-player captures, clean-extracted boot, all art hashes and package manifest. Record immutable source/package/capture evidence in `Docs/ReleaseEvidence/v2.25.0-summary.json` and commit it separately.
6. Recheck remote main and publish through the established non-force source push plus local versioned Windows ZIP/build folder. Do not create a new distribution channel implicitly: the existing GitHub repository has no Releases/upload workflow.

Destinations expected at the historical checkpoint (produced September 26):

- `outputs/AshAndBrimstone-Windows-v2.25.0.zip` (relative to Git repository)
- `outputs/ash-and-brimstone-build/AshAndBrimstone-Windows-v2.25.0`

Physical-controller feel, subjective sound listening and a complete human campaign playthrough remain separate manual follow-ups.
