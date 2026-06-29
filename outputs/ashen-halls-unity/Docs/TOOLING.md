# Ashen Halls Tooling

## Project Direction

- Keep the core in Unity/C#.
- Keep assets original.
- Keep audio sparse: simple sound effects only, no music for now.
- Avoid Asset Store dependencies until the core loop is more stable.
- Prefer small playable releases over large hidden rewrites.

## Codex Skills and Plugins

- `imagegen`: use for original pixel-art reference sheets, enemy silhouettes, splash/title concepts, UI mood boards, and sprite direction. Treat outputs as concept/reference art unless we deliberately convert them into project assets. v0.28 deliberately starts converting one generated beta combat sheet into a runtime texture atlas.
- Browser/control tooling: useful for local web prototypes or docs, but not reliable for Unity window verification in this project. Use logs plus user playtesting for Unity launch checks.
- Document/PDF tooling: useful when mining the Nahlakh hintbook or similar reference material. Convert discoveries into design notes only; do not copy assets, tables, prose, or exact text.
- Multi-agent review: useful for separate code review or encounter/item brainstorming, but keep implementation centralized while the game is still mostly one large Unity script.

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
- The Windows release package copies the manifest into `Docs/TOOL_DOWNLOADS.md` when the manifest is present.
- LibreSprite 1.2 is staged as a zip archive for future hand-drawn sprite and tile work.
- Generated art-reference sheets live under `Docs/ArtReferences/` and should guide the final pixel language rather than be copied blindly into runtime art. The v0.25 sheet is focused on exploration-map tiles, fog, landmarks, and region identity; the v0.26 sheet is focused on compact UI, map landmarks, and dangerous caster silhouettes; the v0.27 sheets focus on title/splash mood, brighter combat frames, and party/enemy sprite readability; the v0.28 sheet is the first runtime atlas bridge for visible combat/portrait crops.

## Unity Packages to Consider Later

- Unity Test Framework: first package to add when we start automated checks for item generation, save/load, combat math, and party validation.
- Input System: add only if mouse/window scaling, focus, or remapping issues keep recurring.
- TextMeshPro or UI Toolkit: defer until the UI is split into a more formal layer.
- Addressables: defer until we have real external art/audio bundles.

## Asset Rules

- No copied Nahlakh code, assets, exact spell tables, prose, or UI screens.
- Light homage is fine; direct duplication is not.
- Generated art should be reviewed, simplified, and translated into the game's own sprite language.
- Prefer small, readable pixel shapes over high-detail art that collapses on the tactical board.
- The enemy/map concept sheets are allowed to inspire silhouettes, colors, and shape language, but final in-game assets should remain edited, simplified, and original.
