# Ashen Halls uGUI Migration Audit v1.20.6

## Current Screen Ownership

- Fully uGUI: Tavern, Party Setup, Help Overlay, Armory, Dialogue, Loot Popup, Pause Menu, Combat Spell/Skill modal, Victory/Defeat end-state screen.
- Hybrid: Exploration and Combat still render the board through legacy IMGUI, while uGUI owns the HUD, command/status panels, and overlays.
- Still legacy IMGUI: Exploration board, Combat board, startup/error splash fallback.

## Fixed In v1.20.6

- Restored visible development-only Beta Testing access on the uGUI Tavern screen.
- Restored banner feedback over Tavern and Party Setup after those screens stopped drawing the normal legacy frame.
- Aligned Tavern hotkeys with visible actions:
  - Enter: Continue if a save exists, otherwise Begin the Old Road.
  - B: Begin the Old Road.
  - C or N: Customize Party.
  - S: Settings.
  - T: Beta Testing in editor/development builds.
- Added layout smoke coverage so the dev-only Beta Testing button and panel fit supported resolutions without overlapping the main menu.

## Fixed In v1.20.7

- Added `BannerToastScreen`, a passive uGUI canvas for banner/toast feedback across Tavern, Party Setup, Explore, Combat, and overlays.
- Kept the old IMGUI banner drawing only as a fallback if the uGUI toast fails to initialize.
- Added layout smoke coverage for the toast panel at 1280x720, 1600x900, 1920x1080, and 2048x1152.

## Fixed In v1.22.0

- Added `HelpOverlayScreen`, a uGUI F1 help panel with mode-specific controls for Tavern, Party Setup, Exploration, and Combat.
- Routed Help through the shared overlay stack so it blocks board input and closes with F1, Esc, or Close.
- Replaced the old F1 log-only help path, reducing Timeline/log clutter during normal play.

## Fixed In v1.23.0

- Added `EndStateScreen`, a uGUI Victory/Defeat screen with a party ledger, route summary, and cleaner action layout.
- Kept legacy end-state behavior available as a fallback if uGUI startup fails.
- Added Victory/Defeat-specific Help copy and smoke coverage for end-state layout/content.

## Fixed In v1.23.1

- Reconnected existing spell and martial ability atlas art to the uGUI Combat Ability modal.
- Added formula codes, skill sigils, and school/class accent colors so power cards have distinct identities at a glance.
- Kept the combat HUD visible as a dimmed, non-interactive underlay while Spellbook or Combat Skills owns focus.

## Fixed In v1.23.5

- Expanded the uGUI power banner with a dedicated actual-outcome line for damage, healing, target count, defeats, statuses, cleansing, summons, and terrain changes.
- Kept outcome calculation outside presentation code so the banner renders measured combat results without owning or duplicating combat rules.

## Fixed In v1.24.1

- Added readiness/visibility contracts and automatic reconstruction for the Exploration HUD, Combat HUD, Dialogue, and Combat Ability modal.
- Preserved Exploration and Combat HUDs as dimmed, non-interactive underlays for every gameplay overlay, with corrected canvas sorting so Armory, Loot, Pause, Help, Dialogue, and power pickers remain on top.
- Added one-frame pointer suppression when gameplay overlays close, preventing uGUI clicks from leaking into the legacy IMGUI boards.
- Added runtime smoke coverage for NPC dialogue, the complete combat HUD stack, and the Spellbook modal.

## Remaining Migration Gaps

- Exploration and Combat boards remain IMGUI by design for now. Replacing them is a larger task because those boards own hover targeting, tile input, sprite drawing, VFX, and debug overlays.
- Exploration and combat overlays now preserve their underlying HUD context. The remaining work is visual refinement and eventual board extraction, not missing overlay ownership.

## Recommended Next Modules

1. Split Exploration and Combat board rendering into explicit render/input modules before attempting a full uGUI/UITK board rewrite.
2. Continue reducing direct screen mutation in the legacy orchestrator by routing presentation changes through commands/view models.
