# Progression Notes v0.63

Scope: make the existing progression loop visible and useful without changing the save schema.

## XP Sources

- Combat victories still provide the main XP reward.
- First-time zone discoveries now grant small exploration XP.
- Caches and shrines grant small XP rewards when consumed.
- Descending to a new chapter/depth grants a milestone XP reward.

## Level Rewards

- Level-ups recalculate HP and mana from stats, gear, race, class, and level.
- Level-ups grant stat points and skill points. Active campaigns spend them through `I > Growth`; pre-departure Muster retains its existing spending controls.
- Level-up log lines now name newly unlocked spells or martial abilities.

## Spell and Ability Learning

- Warrior and rogue abilities keep their existing level gates.
- Caster formulas are now level-gated in the combat Spellbook.
- Spell Reference still shows the whole formula book, but each row names its required level.
- Beta balance: most starter spells are level 1, utility/pressure spells open around level 2, and larger splash/summon/showcase formulas open around level 3.

## World Map Art

- `world-map-progression-overlay-atlas-runtime-v0.63.png` adds campaign-readable markers for discoveries, objectives, route gates, training, stairs, recall, kobold smoke, and final-gate omens.
- The older world-map overlay atlas remains reserved for movement and cursor hints.
