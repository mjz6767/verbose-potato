# Ash & Brimstone Family Playtest Readiness

## Current Candidate

`v1.88.0` is a family-playtest candidate for the Midgaard sewer and bounded Old Road slice. It is not a content-complete game.

## Proven Automatically

- Normal startup reaches the Tavern, New Game reaches Party Setup, and Quick Start reaches exploration.
- The default compact exploration rail keeps the next waypoint, objective, nearby sites, and all four party rows visible. Fresh guidance names King's Hall and advances with sewer progress.
- Midgaard's critical NPCs, gates, sewer, armorer, and regional exits are present and reachable.
- Dialogue, Journal, loot comparison, pause, help, spellbook, ability modal, and combat command overlays own input without clicking through to the board.
- Broken Sluice, Foul Runoff, and Cistern Den use authored encounters.
- Each authored sewer encounter publishes a concise first-round plan beneath the initiative strip, then yields to normal combat information.
- Foul Runoff opens a two-item safe-room choice; one item is recorded, added, and equipped once.
- Three first clears yield exactly three proof bundles.
- Borin consumes proof, grants the rat-pelt armor, and unlocks the Old Road teaser.
- Saves use atomic primary/backup writes. Normal play checkpoints at safe milestones; labs and batch tests cannot overwrite a campaign.
- Normal campaign combat can retreat for one supply through a confirmed Menu action. It abandons loot/progress, restores the party at Temple Square, and checkpoints the recovery.
- Uncommitted movement can be undone from the command deck or with U/Backspace. The unit returns to its turn-start tile with its full movement allowance; committing any action locks the move.
- When a start-turn web expires or burns away, movement repair also replaces the stale zero-point Undo Move snapshot. The unit can move, undo to its original tile with its full allowance, and move again.
- Armed spell and skill targets can be canceled from the command deck, Esc, or right-click without spending the action or disturbing movement/undo state.
- One central player-command gate rejects resolving round and impact holds plus dead, inactive, and non-party actors before callbacks or resource/state mutation. End Turn remains valid for the living active party unit in Choose Action or Choose Target even when stunned, sleeping, or action-spent.
- Fresh and loaded maps certify that all eight named outer-road junctions remain reachable from Midgaard.
- Rule and runtime smoke suites pass in Unity 6000.3.18f1.

## Still Needs Humans

- A new player should understand the objective without reading the README.
- All three fights need balance feedback from players who do not know the systems.
- Spell, skill, terrain, and enemy-turn pacing need subjective feel checks.
- The safe-room choice, defeat retry, and final reward need visual confirmation in the packaged Windows player.
- A clean-folder launch and one complete real playthrough remain the release gate before calling this broadly shareable.

## 15-Minute Family Script

1. Unzip the package and launch `AshAndBrimstone.exe`.
2. Select New Game, inspect Party Setup, then use Quick Start.
3. Find the sewer contract and enter Broken Sluice.
4. Use Move, Attack or Shoot, one Skill, one Spell, Guard, and End Turn.
5. Clear Foul Runoff and choose one recovery-room item.
6. Enter Cistern Den and decide whether the final room feels fair.
7. Return to Borin, claim the reward, and open the Journal.
8. In a second run, deliberately lose a fight and test Tavern > Continue.

## Feedback Questions

- At any point, were you unsure what to do next?
- Which combat command or spell was confusing?
- Did enemy turns feel too slow, too fast, unfair, or trivial?
- Did the safe-room choice feel meaningful?
- Could you read sprites, hazards, targeting, and results without squinting?
- Would you voluntarily play one more route after the teaser?
