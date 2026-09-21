# World Map experience audit — September 7, 2026

## Scope

Local travel, Region browsing, route guidance, discovery-safe inspection, and responsive map HUDs. The starting tree was clean at `4783a27840ce8be14075fe4b7fd3939fa154b92c` (v2.24.0, save schema 27). A separate user-requested combat-effects task is editing disjoint files in the same workspace; Unity access and the combined verification pass are coordinated. This audit does not replace the existing v2.24 release package or change campaign schemas.

## Findings corrected

- **A mouse-wheel event could move the party.** Unity wheel events report button zero, and Local Map handling could fall through to the adjacent-cell click path. Travel now requires an explicit left `MouseDown`. The regression verifies unchanged serialized campaign state for scrolling in all four directions and a positive left-click moving exactly one cell/step.
- **Region gestures could outlive input ownership.** Active gestures now abort when a modal or another game mode owns input, and releasing a click outside the board cancels it. Native horizontal/two-axis trackpad scroll is supported alongside Shift-wheel.
- **Region inspection advertised Local actions.** Adjacent terrain, objects, and patrols no longer promise click-to-move/use/engage while browsing. The descriptions identify inspection and point back to Local Map; Local hints remain actionable.
- **Remembered terrain exposed live distant patrols.** Header and inspection text now follow the same near-or-alerted visibility rule as map markers. Browsing does not reveal or otherwise change campaign state.
- **The next-step highlight could appear beyond a fog boundary.** Next-step, route-prefix, and edge cues now share discovery-aware rules. Null, discontinuous, or invalid coordinates do not create a visible shortcut.
- **Unrouted bearings were called walking steps.** Journal chart/site rows and the compact nearest-marker summary now label Manhattan estimates as grid range. Actual computed walking paths still report steps; the Journal explains that terrain can lengthen a route.
- **Long destinations were clipped in the side rail.** Native and fallback location headings now wrap within the existing header, using a readable size floor without displacing party rows. Contextual actions expose E, Details exposes Q, and map-toggle hints consistently advertise Tab/Y.
- **Compact board headers truncated the current location.** Below the wide-layout threshold, the board strip gives its name more room and omits duplicated danger/context columns. Local/Party Here/Inspect status and Q remain visible. Wide layouts retain all five nonoverlapping columns.

## Regression coverage

`WorldMapNavigationAuditSmoke`, `WorldMapInspectionAuditSmoke`, `WorldMapGuidanceAuditSmoke`, and `WorldMapHudAuditSmoke` are wired into `RuleSmokeTests`. They combine deterministic geometry/route rules with the actual pointer, inspection, and Journal row-building handlers. HUD measurements cover authored landmark/junction names, both detail modes, and 960×600 through 2048×1152.

Pointer tests use immutable input snapshots consumed by the production handler, not synthetic native Unity events outside OnGUI. A positive click must be consumed and move exactly one tile/step, so a wheel test cannot pass merely because all events were ignored. Header tests also measure compact/wide threshold boundaries at two interface scales.

## Verification

Completed September 8, 2026, on the combined, uncommitted world-map and separately requested combat-effects working tree:

- **Full automated audit: PASS.** Rules, inventory/loot, sprite art, combat UI, and runtime boot all passed in `QA/project-audit/20260908-031655-c0d90e4e/unity.log`. No runtime code changed between that pass and the final player build.
- **Development preview build: PASS.** `QA/audit-players/20260908-073456-f3f27056/AshAndBrimstone.exe`, with build log `QA/world-map-audit-2026-09-07/audit-player-build-with-art.log`. The first preview omitted external art; the audit helper now reuses the normal build's documentation/art-copy selection. All 92 final staged PNGs match their source hashes. The earlier `20260908-031816-d17c4c31` player and `after` captures are superseded, not accepted final visual evidence.
- **Eight final captures: PASS and visually reviewed by the assistant.** Local and Region at 960×600 and 1920×1080, expanded Local and Region details at 1280×720, and Region landmark-selection and uncharted-focus views at 1280×720. Full location names, contextual shortcuts, party rows, and the inspected footer/detail layouts are readable without observed overlap. These are rendered scenario fixtures, not live-input playthroughs.
- **Capture packet: PASS, zero warnings/failures.** `QA/world-map-audit-2026-09-07/final-f3f27056/visual-qa-packet/visual-qa-packet.json`; capture-set SHA-256 `16607468e2b941d3b0f2667a62398f2c4154684f1485f18e5fa2e72659ff64a7`. Its v2.24.0 label is the unchanged source version, not a promoted release. Its automatic AI-review flag remains false; this separate audit records the subsequent in-conversation visual inspection.
- **Live keyboard/mouse/controller review: pending.** The user authorized visible test windows. The computer-use skill's application approval timed out before any real input could be sent or verified. Programmatic visible captures and handler regressions passed, but do not replace a human input, campaign, or controller playthrough.
- **Existing release preserved.** No version/schema bump, commit, push, retail-build replacement, or release promotion. The existing v2.24.0 ZIP still hashes to `ab6cb675f31bb5abbaf51c089c2697e44c0950888e2bdf178ceecb64b7ea63cf`. Audit fixtures block campaign saves.

Hashes and explicit review limits are recorded in `Docs/ReleaseEvidence/world-map-audit-2026-09-07.json`. The preview includes the separately coordinated combat-effects work; these map screenshots do not certify that work's live visual/audio experience.
