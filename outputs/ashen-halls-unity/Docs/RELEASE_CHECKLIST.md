# Ash & Brimstone Release Checklist

Use this for each small release zip.

## Before Packaging

- Update the single `PackageVersion` source in `Assets/Scripts/VersionInfo.cs`.
- Confirm `Assets/Editor/BuildWindows.cs` and `Tools/BuildAndPackageWindows.ps1` derive and validate that value.
- Update `README_PLAY.txt`.
- Add a short entry to `CHANGELOG.md`.
- Update `KNOWN_ISSUES.txt` if tester workarounds changed.
- Bump `SaveVersion` only when saved data shape changes.

## Play Smoke Test

- Launch the packaged build outside Unity.
- Confirm the Midgaard tavern first screen appears without a timed startup delay.
- Confirm build/version text identifies the beta scaffold without covering the tavern art.
- Quick Start a party.
- Use Beta Lab when testing combat/casting-heavy releases.
- Press I and confirm the Armory opens, tabs switch, and Esc closes it.
- Outside combat, press C and confirm the Formula Codex opens directly.
- Hover exploration tiles and confirm region/object/movement hints appear.
- Move into a new exploration region and confirm a region banner or chronicle line appears.
- On a new campaign, confirm the map is 58x46 and all eight regional sites and named junctions are reachable without entering a reserved Midgaard interior footprint.
- Chart two outer-road junctions, open Journal, mark each in turn, and confirm Mark/Clear state, the path-aware location readout, and the bounded gold map trail all follow the selected destination.
- Save and load with a waypoint selected, then descend to a new depth and confirm the old-depth waypoint does not leak onto the new map.
- Load a supported existing save and confirm its serialized map dimensions and biome layout are preserved.
- After the sewer reward, confirm Journal tracks Sluice Steps, Dusk Market, Smoke Cave, and Varkh's Hall; follow that route and confirm unrelated prototype routes and scaffolds remain unavailable.
- Enter combat.
- Confirm the opposition panel shows enemy tactic/threat lines.
- Complete one party/enemy round using move, attack, cast, guard, elixir, and wait where possible.
- Let a duration-one field and enemy ritual reach a new round together. Confirm one `ROUND N` summary names both events, board input and enemy AI wait, and the reserved next unit begins exactly once.
- At turn start, verify poison wakes a sleeping unit, ice can stun immediately, web removes movement, fire restores movement after burning web, and lethal automatic damage completes its fall before initiative advances.
- Open both Spellbook and Skillbook; confirm All, Ready, Learned, and Future counts agree with their visible cards.
- Navigate each book with mouse, W/S or arrows, Home/End, Page Up/Page Down, 1-4, Tab, Enter/Space, and a controller where available.
- Arm a targeted spell and skill from the detail action. Reopen each book, browse a different card, move, cancel, and reopen; confirm armed and selected states remain distinct and no resource is spent before target confirmation.
- Inspect a low-MP spell, future unlock, action-used combatant, stunned/sleeping combatant, and a power with no legal target; confirm each primary action has an exact readable reason and cannot mutate combat state.
- Open a cache and confirm the loot comparison panel appears.
- Press 1-6 in combat and confirm each action selection responds.
- Press M, +, and - and confirm SFX preference banners appear.
- Press SFX Test from the UI and confirm click/attack/spell effects are audible.
- Save with F5 and load with F9.
- Confirm SFX and Music controls act independently, zone/combat music crossfades, and sparse exploration ambience pauses under modal screens.
- Speak with one named contact and one service NPC; confirm dialogue owns input, information is free, and a purchase requires an explicit enabled choice.

## Visual Checks

- Check 1280x720.
- Check 1920x1080.
- Reject any capture whose decoded PNG dimensions differ from the requested size or whose sampled frame is uniformly black.
- Confirm panels do not overlap.
- Confirm Gold, Supplies, and Elixirs are spelled out and every exploration party row shows readable numeric HP and MP.
- Confirm long current objectives remain readable in both compact and detailed exploration rails without covering the party rows or Details button.
- Confirm hover previews stay on-screen.
- Confirm sprites, highlights, and turn queue are readable.
- Capture Spellbook and Skillbook at both endpoint resolutions, including a long-list bottom selection, future filter, unavailable card, and armed Return to Target state.
- Confirm 12-pixel body text, 40-pixel minimum controls, list/detail scrollbars, filter counts, legal-target count, and primary action remain legible without clipping.

## Package Checks

- Confirm `AshAndBrimstone.exe` launches from a clean extracted folder.
- Confirm `README_PLAY.txt`, `CHANGELOG.md`, `KNOWN_ISSUES.txt`, and `Docs/` are included.
- If tool downloads are staged, confirm `Docs/TOOL_DOWNLOADS.md` is included.
- Confirm the zip filename matches the package version.
- Keep the last known-good zip until the new one has been tested.
