# Ashen Halls Release Checklist

Use this for each small release zip.

## Before Packaging

- Update `PackageVersion` in `Assets/Scripts/AshenHallsGame.cs`.
- Update `PackageVersion` in `Assets/Editor/BuildWindows.cs`.
- Update `README_PLAY.txt`.
- Add a short entry to `CHANGELOG.md`.
- Update `KNOWN_ISSUES.txt` if tester workarounds changed.
- Bump `SaveVersion` only when saved data shape changes.

## Play Smoke Test

- Launch the packaged build outside Unity.
- Confirm the title splash appears.
- Confirm the splash labels the build as a beta/test build when applicable.
- Quick Start a company.
- Use Beta Lab when testing combat/casting-heavy releases.
- Press I and confirm the Armory opens, tabs switch, and Esc closes it.
- Press C and confirm the Formula Codex opens directly.
- Hover exploration tiles and confirm region/object/movement hints appear.
- Move into a new exploration region and confirm a region banner or chronicle line appears.
- Enter combat.
- Confirm the opposition panel shows enemy tactic/threat lines.
- Complete one party/enemy round using move, attack, cast, guard, elixir, and wait where possible.
- Open a cache and confirm the loot comparison panel appears.
- Press 1-6 in combat and confirm each action selection responds.
- Press M, +, and - and confirm SFX preference banners appear.
- Press SFX Test from the UI and confirm click/attack/spell effects are audible.
- Save with F5 and load with F9.
- Confirm simple sound effects play, can be muted, and there is no background music.

## Visual Checks

- Check 1280x720.
- Check 1920x1080.
- Confirm panels do not overlap.
- Confirm hover previews stay on-screen.
- Confirm sprites, highlights, and turn queue are readable.

## Package Checks

- Confirm `AshenHalls.exe` launches from a clean extracted folder.
- Confirm `README_PLAY.txt`, `CHANGELOG.md`, `KNOWN_ISSUES.txt`, and `Docs/` are included.
- If tool downloads are staged, confirm `Docs/TOOL_DOWNLOADS.md` is included.
- Confirm the zip filename matches the package version.
- Keep the last known-good zip until the new one has been tested.
