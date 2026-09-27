# NPC heads and town/world-map presentation — September 27, 2026

## Scope

Local, unreleased follow-up to v2.31.0. The user requested repairs for clipped NPC head tops and a visual audit of the town/world map. Existing original artwork, gameplay, audio and save schema v27 are preserved. The v2.32.0 filename suffix identifies this art revision, not a published game version. Earlier v2.31 Windows packages are not replaced.

## Root cause and recovery

The runtime renderer was not cutting the heads. Historical equal-grid source extraction cut through nine figures before padding and normalization; the resulting runtime sprites passed gutter/height checks despite missing their crowns. The intact authoring sheets still contain all missing pixels. This corrects the earlier alpha-audit limitation, which referred to the cropped v2.21 runtime exports as the source.

User-approved native Aseprite 1.3.18.5 recovers reviewed nonuniform rectangles from the hash-locked original sheets. It proves the silhouette is inside the source rectangle **before** trimming, normalizes the full figure to the established height/baseline, removes reviewed neutral matte pockets and softens neutral ground remnants. No new generated art or runtime scaling workaround is used. Originals and previous approved atlases remain unchanged.

| Atlas / cells | Recovered figures | Runtime PNG SHA-256 |
| --- | --- | --- |
| Named: 14, 16–19 | Dock Worker, Herald Vann, Novice Healer Sera, Scout Yara, Scholar | `C4D101694CB9D4465831158CF42BE18D15DDD708D3CCA5851F008C627A48D2DB` |
| Citizen: 4–7 | Apothecary, Road Pilgrim, Gravedigger, Caravan Guide | `A6150C87154D7F7EAA10FD43CF964F998D358D479D9F538ECD09A93D16A50685` |

`Tools/RecoverNpcHeads.ps1` and `RecoverNpcHeads.lua` reproduce the native repair. The two layered Aseprite files retain approved v2.26.1 as a hidden reference; sibling JSON records input, output, editable and recipe hashes. Only the two runtime PNGs enter a player package. Six selected art/provenance files are explicitly made visible to Git despite the local ArtReferences exclusion.

Independent checks confirmed:

- All 1,572,864 RGBA pixels in the 19 untargeted cells remain exact.
- All 28 cells keep completely transparent 18px named / 20px citizen gutters, including soft-alpha pixels. Existing coverage/height limits are unchanged.
- All nine corrected heads pass the new contour regression; all nine historical padded cuts fail. Synthetic rounded and flat-cut heads also exercise the classifier.
- The six existing enclosed-matte probes stay transparent. Eight unaffected protected pale-color probes remain exact. Sera's same ivory color `(244,225,189,255)` is protected at its rescaled location `(634,926)`.
- Reopening and exporting both native sources reproduces all 2,490,368 runtime RGBA pixels, including invisible RGB, exactly. PNG container compression alone differs.
- Primary and independent reviewers inspected both complete runtime atlases; all nine crowns are complete and no new opaque matte pockets were found.

## Town and map presentation

- Off-screen NEXT/MARK text moves from an over-map chip into a reserved header lane. The exact directional edge tick remains at the charted viewport exit; route bearing/count, fog-prefix suppression, Region focus coordinates and hover identity are retained. Roofs and heads no longer sit beneath that text box.
- Passive exterior buildings use subdued neutral ground contact instead of colored threshold strips and borders. Objectives, current interaction targets, hovered buildings and marked waypoints retain their established emphasis. Authored interior foundations are unchanged.
- The presentation uses the already-computed guidance result; no duplicate route calculation or persistent cross-frame cache is introduced.
- Deterministic tests cover all four bearings, full integer counts, missing/invalid cues, compact/wide header sizes, long Region names, all 16 combinations of building focus flags and bounded opacity.

## Game verification

- Full Unity audit: **PASS, all 13 gates**, including the new head-contour and scene-presentation regressions. Log: `QA/project-audit/20260927-072542-05fd2368/unity.log`.
- Isolated Development preview: `QA/audit-players/20260927-072709-ba362dc3/AshAndBrimstone.exe`. It retains the game v2.31.0 label; no release version was changed. Build log: `QA/npc-heads-20260927-build.log`. Packaged NPC PNGs exactly match the hashes above; no native editable sources entered the player.
- All 18 actual-player captures pass exact-resolution, scene-marker, non-black-image and error-log validation, with zero packet warnings/failures. Nine scenes per size cover Kate, Lute, Dock Worker, Scholar, Vann, Sera and Yara Local views, Kate Region and a Region landmark. The capture fixtures are save-blocked and exit themselves.
- 960×600: `QA/world-sprites/20260927-072809-5b461331`; set SHA-256 `c1191cff1aa7c0b29aa0e2e141b6c984f1454bd90b4a55f2819e143e58dbeeae`.
- 1920×1080: `QA/world-sprites/20260927-073033-6c792497`; set SHA-256 `47ca7651c540bd6545a202c9900542d07404a0bed4dcf7e3488462e6698b2fb9`.
- An independent reviewer personally inspected every final screenshot; the primary assistant also inspected seven compact Local views and the full-HD Lute/Scholar views. Recovered crowns are complete, no white matte patches returned, route text stays above artwork, and Region focus/interaction/quest/exit cues remain legible. Art crossing the outer world viewport remains intentionally clipped to that viewport.
- Raw automated visual packets retain their separate `aiReview.performed=false` field. This document and `VisualEvidence/npc-heads-world-map-2026-09-27.json` record the subsequent conversational visual review; they do not relabel automated checks as visual inspection.
- Both v2.31 archives retain their prior SHA-256 values: retail `E92B231C6566F7B8CBC2F6F3E5B68A47A0CDF2725EDBB536B7FCF6AE76AB2338`, beta `A7E6151592E803C77465D02B4EAF5776BC7B869BB914D3B58734D3A5753F6803`.

## Limits

This pass does not claim a retail release, GitHub push, full campaign playthrough, measured FPS improvement, audio listening review or physical-controller testing. Historical build archives and user campaign data are preserved. The art repair intentionally retains legitimate pale clothing, hair, skin, tools and highlights.

One nonblocking wording follow-up remains: the header's `NEXT` describes the route's viewport-exit bearing/count, while the side rail's `NEXT` describes the immediate guidance target. They can show different directions because a route turns. This pre-existing distinction is retained; clearer naming can be considered separately without changing pathfinding. No new live mouse/keyboard-input test is claimed for this capture-based pass.
