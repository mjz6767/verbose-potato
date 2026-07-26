# Combat Audit v0.62

Scope: presentation and usability only. No save-schema, combat-math, AI, or spell-behavior changes.

## Findings

- The command bar was clearer after v0.61, but the selected action could stay on an unavailable command after movement or action state changed.
- The active readout described the unit well, but it did not answer the most important tactical question: "who can hurt me right now?"
- Keyboard combat was improving, but blocked hotkeys were too quiet for beta testing.
- Board overlays were already mostly art-first; the best next cue was a small edge/frame signal rather than more text on top of sprites.

## Changes

- Added `NormalizeCombatSelection` so stunned, sleeping, fully spent, or otherwise invalid turns fall back to a sensible command, usually End Turn.
- Added active-unit threat summaries: direct enemy hits, pressure threats, or no direct threat.
- Added subtle red and amber enemy threat frames with tiny edge pips.
- Added Timeline feedback when a combat hotkey is blocked.
- Added Enter and Keypad Enter as End Turn hotkeys.

## Deferred Cleanup

- Add a stable per-round initiative queue to `CombatState`.
- Move timed terrain and hazard ticking into a clear end-of-round or global tick.
- Centralize action spending/end-turn flow for Guard, Wait, Elixir, Attack, Cast, skills, and enemy actions.
- Decide which enemy roles can move-plus-attack versus move-or-attack.
- Reuse BFS movement planning for enemy AI.
- Remove or quarantine unused legacy casting paths after a final search.
