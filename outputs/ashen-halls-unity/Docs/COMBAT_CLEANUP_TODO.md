# Combat Cleanup TODO

Short follow-up list after the v0.53.1 Guard damage fix, v0.62.0 combat UI audit, and v0.75.0 basic mechanics pass. Keep these as small rules patches rather than one large combat rewrite.

1. Done in v1.3.0: add a stable per-round initiative queue to `CombatState` instead of recomputing `InitiativeOrder()` during every `NextTurn()`.
2. Done in v1.87.0: round-start terrain expiry and ritual openings resolve under one bounded `ROUND N` presentation gate before the reserved next unit begins.
3. Done in v1.5.6 for turn finishing and v1.88.0 for player-command admission: player actions, enemy turns, and skipped stun/sleep turns share one end-of-action policy. `CombatController` now rejects commands during resolution holds and from dead, inactive, or non-party actors before callbacks or state/resource mutation, while End Turn remains valid for a living active party unit in `ChooseAction` or `ChooseTarget` even when incapacitated or action-spent. Formula/ability preview math remains separate where footprints and status outcomes differ.
4. Done in v1.42.0: enemy role/rank profiles now decide one-step move-plus-attack. Brutes, marksmen, casters, support units, bosses, elites, and veteran skirmishers can step once and act; ordinary skirmishers must commit to moving or attacking, and long moves never lead into an attack.
5. Role pass done in v1.42.0: the existing BFS planner now shares preferred range, line-of-sight, close-pressure, and hazard weights for brutes, skirmishers, marksmen, casters, support units, and bosses. Encounter-specific formation or route weights can remain a later tuning pass.
6. Done before v1.58.0: the unused legacy `CastSpell()` path was removed; source search now finds no active or dormant implementation.
7. Done in v1.43.0: normal attack legality, ranged/melee mode, line of sight, hit chance, post-mitigation damage range, expected damage, and threat severity now share one forecast across hover text, target cards, board cues, enemy intent, AI reachability, target scoring, and actual attack validation. Formula/ability previews remain separate because they have distinct footprints and status outcomes.
