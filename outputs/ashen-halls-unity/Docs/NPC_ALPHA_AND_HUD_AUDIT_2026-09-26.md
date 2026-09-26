# NPC transparency and contextual HUD audit — 2026-09-26

## Scope and disposition

User requested less UI clutter and a repair for partial white backgrounds on world NPCs, explicitly approving installed Aseprite. Implemented and verified locally. No published build was overwritten, no campaign save was intentionally loaded or written, and this pass did not commit or push. The user subsequently authorized coordination with the audio-update chat, which owns the combined v2.27.0 release, final validation, commit and publication.

Evidence below applies to the NPC/HUD preview built before the parallel audio/version changes. Its game label is v2.26.0, save schema v27, based on repository HEAD `e9892c28980ad616af991ae8a8a3cb78abdca55d` plus the changes listed here. The v2.26.1 suffix identifies the repaired artwork, not a published game version. Re-run the release gates after combining changes.

## Cause and repair

Opaque neutral pixels were baked into the v2.21 PNGs, including enclosed spaces between staffs/robes/legs and pale footplates. The earlier source-cell border flood removed outer backgrounds but could not reach enclosed islands; later alpha normalization made surviving remnants fully opaque. Runtime geometry and coverage checks alone therefore passed defective art.

`Tools/RepairWorldNpcAtlases.ps1` invokes Aseprite 1.3.18.5 and the hash-locked `RepairNpcAtlasMatte.lua` recipe. Reviewed per-cell negative-space seeds, exterior connectivity and one bounded fringe pass remove matte without a global white color-key. Edge-connected lower-foot neutral remnants become subdued translucent black shadows. No new generated art, resizing, pose shift or cell remapping was used.

| Atlas | Cells / canvas | Matte pixels cleared | Shadow pixels repaired | Runtime PNG SHA-256 |
| --- | --- | ---: | ---: | --- |
| Named NPCs | 20 / 1280×1024 | 36,066 | 14,073 | `4CA876765191A0A149B06FEC34F3952D0DB4B0C810BD90DBF38AEBF0DC3B3DA7` |
| Ambient citizens | 8 / 1536×768 | 35,556 | 14,867 | `95382FDF50404024CE0D7F1014E9EEAF7CDD543F701FA986412066A4F53FE638` |

Native `Docs/ArtReferences/source-*-atlas-v2.26.1.aseprite` files retain the original v2.21 art as a hidden layer and the repair as the visible layer. Their roundtrip PNG exports have different container hashes but **zero decoded RGBA mismatches across all 2,490,368 pixels**, including invisible RGB. Originals remain unchanged. Sibling validation JSON records exact tool, recipe, source, output and native-file hashes.

Independent checks preserved all 435,699 chromatic source pixels (channel spread greater than 22), 19 selected pale-art probes, and all existing coverage/gutter limits. Six known matte probes now have alpha zero. Permanent `NpcAtlasAlphaSmoke` regressions cover those six sites plus nine exact protected-color samples, including near-white spear and hood details. Citizen silhouette bounds may trim one matte pixel at either edge (height 342–344, gutters 20–21); canvas and strict 20px safe gutter remain unchanged. Named bounds and all coverage thresholds were not loosened.

## HUD improvement

The action verb, full destination and E shortcut form one centered content group inside the original full-width hit area. Previously the key drifted hundreds of pixels from its action on wide screens. Unavailable No Route/No Action text uses quieter normal-weight styling with verified readable contrast, and reserves no empty shortcut/icon space. Native and fallback HUDs share the geometry. Controls, resource numbers, objectives, party HP and navigation were retained.

## Verification evidence

- Full Unity audit passed: `QA/project-audit/20260926-191757-e97d169d/unity.log`. Rules, inventory/loot, sprite art, combat UI and runtime boot all passed. The first run exposed an obsolete exact 344px silhouette assertion; the measured one-pixel-per-edge repair contract above replaced it before rerunning successfully.
- Separate development preview: `QA/audit-players/20260926-191906-4dc8f2e8/AshAndBrimstone.exe`. Build log: `QA/npc-matte-repair-build.log`. Initial Unity licensing handshake diagnostics recovered; build completed successfully. Both staged NPC PNG hashes exactly match source; package contains 92 selected runtime PNGs and excludes editable art.
- Six actual-player scenarios at each size: Kate Local, Lute Local, Dock Local, Scholar Local, Kate Region and landmark Region. Captures and logs passed deterministic validation with no warnings. Two independent visual reviewers personally inspected all 12 captures; root inspected the Kate views at both sizes. White wedges/footplates are gone on stone and timber; pale clothes, skin, hair, tools, lanterns, limbs and interaction cues remain intact. No HUD clipping/overlap found, and Region glyph/fog presentation is preserved.
- 960×600: `QA/world-sprites/20260926-192002-bd82bbbd`, capture-set SHA-256 `688b5403da35b32b3bf6e7b2c84b93acccb3aad201593b8927b938284a39fb2c`.
- 1920×1080: `QA/world-sprites/20260926-192120-85aede3b`, capture-set SHA-256 `2fba2e0ff5c233c2a6e47bf4e3df3282090fd5e95a2ed398ac4fb2626e5a5625`.
- Generated visual packets report deterministic checks only; the separate human-readable review record is this document, not an overwritten automated AI-review flag.
- A 1280×720 save-protected Kate fixture booted and was visually inspected through the computer-use skill. The helper twice rejected click attempts because it detected user input; automation stopped and the preview was left open. **No successful live mouse/keyboard interaction is claimed for this pass.** Log: `QA/npc-matte-live-input.log`. HUD geometry and existing interaction behavior are covered by the automated tests above. Audio/controller behavior was not evaluated here.

## Remaining limitation

Several bottom-row named figures (including Scholar) already have clipped head tops in the v2.21 source. The exact same crop remains; this alpha-only cleanup does not reconstruct missing artwork. This is a separate art-quality defect, not a new transparency regression. Tiny intentional bright highlights are retained rather than erased globally.

## Handoff files

- Runtime: `RuntimeArtManifest.cs`, `ExplorationHudScreen.cs`, `AshenHallsGame.ExplorationHud.cs`.
- Tests: new `NpcAtlasAlphaSmoke.cs` and its meta; focused additions to `RuleSmokeTests.cs`, `RuntimeBootSmoke.cs`, `SpriteArtRuntimeSmoke.cs`, `WorldMapHudAuditSmoke.cs`.
- Tools: `AuditNpcMatte.lua`, `RepairNpcAtlasMatte.lua`, `RepairWorldNpcAtlases.ps1`.
- Art: exactly two v2.26.1 runtime PNGs, two layered native Aseprite files and two validation JSON files. These were made visible to Git using intent-to-add despite the local ArtReferences exclusion; logs/prototype PNGs/QA media were not added.
- Docs: `ART_INTAKE.md`, this report and the NPC/HUD Unreleased changelog entries. Parallel audio/version/release edits belong to the coordinating chat and must be preserved.

Published `outputs/AshAndBrimstone-Windows-v2.26.0.zip` remains the prior release (expected SHA-256 `f8c7c1b46328ef1a2e88616ca52cd63115b15d98a6ffe408b19dbf87b7f57a78`). A final combined release requires fresh package and visual verification under its own version.
