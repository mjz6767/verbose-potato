# Combat Action Framework Notes v0.74

This pass keeps the current single-scene/immediate-mode architecture, but starts separating combat intent from UI drawing.

## Current Contract

- Player actions that spend the action should finish through `FinishPlayerCombatAction`.
- `FinishPlayerCombatAction` owns action spent state, optional movement ending, modal cleanup, formula/skill selection cleanup, party sync, and turn advancement.
- Spellbook and Combat Skills pages should preview first and only commit through explicit `Ready` or `Use` controls.
- Spell/skill UI should use action-card metadata for cost, range, target, path, ready state, usability, summary, and detail.

## Still To Do

- Move enemy actions through a similar action-result helper.
- Add stable round queues instead of recomputing initiative every turn.
- Move hazard/status duration ticking to a clearer global or end-of-round timing point.
- Convert formula and martial execution into smaller action-result objects so future abilities can share damage, status, movement, animation, and sound paths.
- Add lightweight tests for action spend, modal click safety, spell readying, skill readying, guard, elixir, and wait.

## Testing Focus

- Spellbook tile click should preview only.
- Spellbook `Ready` should close the book and arm the spell.
- Combat Skills row click should preview only.
- Skill `Ready` should close the panel and arm targeted skills.
- Skill `Use` should immediately resolve instant skills.
- Attack, cover break, cast, skill, guard, elixir, and end turn should all advance consistently.
