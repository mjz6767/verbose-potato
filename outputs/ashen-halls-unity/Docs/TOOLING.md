# Ashen Halls Tooling

## Project Direction

- Keep the core in Unity/C#.
- Keep assets original.
- Keep music and sound effects original, reproducible, and deliberately mixed. Preserve the established 54-master music library, 161 authored SFX, and deterministic procedural fallbacks unless a reviewed replacement is promoted.
- Avoid Asset Store dependencies until the core loop is more stable.
- Prefer small playable releases over large hidden rewrites.

## Codex Skills and Plugins

- `imagegen`: use for original pixel-art reference sheets, enemy silhouettes, splash/title concepts, UI mood boards, and sprite direction. Treat outputs as concept/reference art unless we deliberately convert them into project assets. v0.28 deliberately starts converting one generated beta combat sheet into a runtime texture atlas.
- Browser/control tooling: useful for local web prototypes or docs, but not reliable for Unity window verification in this project. Use logs plus user playtesting for Unity launch checks.
- Document/PDF tooling: useful when mining the Nahlakh hintbook or similar reference material. Convert discoveries into design notes only; do not copy assets, tables, prose, or exact text.
- Multi-agent review: use separate code, QA, and UX passes with explicit file ownership. Much of the runtime still lives in a large partial `AshenHallsGame` class, so prefer extracting deterministic rules and narrow services over adding more stateful legacy code.

## Recommended External Software

- Unity Hub with Unity 6000.3.18f1: keep the current editor version until a feature milestone requires a change.
- Aseprite or LibreSprite: best next tools for original pixel sprites, enemy variants, tiles, and UI icons.
- Audacity: useful for trimming/exporting tiny one-shot WAVs if procedural sound effects are not enough.
- Bfxr, sfxr, or ChipTone: useful for old-school bleeps, thuds, hits, and spell pings.
- 7-Zip: useful for checking package contents and clean extraction.
- Git for Windows: strongly recommended for diffs, branches, tags, and release safety.

## Downloaded Tool Cache

- Current staged downloads live in `outputs/tools/`.
- `outputs/tools/TOOLS_MANIFEST.md` lists the source links, install notes, and local SHA-256 checksums.
- `outputs/tools/Verify-ToolDownloads.ps1` checks the staged files before install.
- These files are a project convenience only; installers are not run automatically.
- Tool-cache files never enter a release implicitly. Promote a reviewed manifest explicitly to tracked `Docs/TOOL_DOWNLOADS.md` before distribution; the Windows package copies only that committed document.
- LibreSprite 1.2 is staged as a zip archive for future hand-drawn sprite and tile work.
- Generated art-reference sheets live under `Docs/ArtReferences/` and should guide the final pixel language rather than be copied blindly into runtime art. `Docs/ART_INTAKE.md` defines the filename prefixes, grid expectations, and handoff notes for parallel art threads. The runtime now prefers the newest matching atlas filename by prefix, so versioned art drops can be tested without a C# filename edit.

## Running an audit without packaging

Run `Tools/InvokeProjectAudit.ps1` from PowerShell for the complete existing rule, inventory/loot, sprite-art, combat-UI, and RuntimeBoot gates. Use `-Suite Rules` for the deterministic suite alone. The command launches the installed Unity editor in batch mode, retains a unique log under `QA/project-audit/`, and requires both a successful editor exit and the suite's completion marker. It waits for the editor itself rather than long-lived licensing child processes. Run it with normal local Unity license access; an isolated account can fail entitlement checks even when the signed-in workstation is licensed.

These checks do not build or promote a Windows package. Release packaging and final-player visual checks remain separate gates.

For focused native-UI review, run Unity with `-batchmode -force-d3d11 -quit -executeMethod AshenHalls.Editor.PresentationAccessibilityCapture.Capture` and the canonical `-projectPath` plus a unique `-logFile`. This renders pause/settings and Combat Help top/bottom at 960x600 and 1280x720 under `QA/project-audit/`; it requires graphics (omit `-nographics`). The offscreen captures complement the final-player input/visual pass.

## Unity Packages to Consider Later

- Unity Test Framework: consider when isolated EditMode/PlayMode fixtures would improve diagnosis. Keep the existing deterministic RuleSmoke and RuntimeBoot release gates as the integrated safety net.
- Input System: add only if mouse/window scaling, focus, or remapping issues keep recurring.
- TextMeshPro or UI Toolkit: defer until the UI is split into a more formal layer.
- Addressables: defer until we have real external art/audio bundles.

## Asset Rules

- No copied Nahlakh code, assets, exact spell tables, prose, or UI screens.
- Light homage is fine; direct duplication is not.
- Generated art should be reviewed, simplified, and translated into the game's own sprite language.
- Prefer small, readable pixel shapes over high-detail art that collapses on the tactical board.
- The enemy/map concept sheets are allowed to inspire silhouettes, colors, and shape language, but final in-game assets should remain edited, simplified, and original.
