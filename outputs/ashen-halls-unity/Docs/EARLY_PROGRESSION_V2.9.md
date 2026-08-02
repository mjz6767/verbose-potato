# Early progression v2.9

Ashen Halls now treats level 20 as the initial campaign cap. The old quadratic curve grew far faster than the game's low-tens combat and exploration rewards, so progression now uses a steady 40-XP threshold increase per level: 100 XP for level 2 through 820 XP for level 20. Reaching the cap requires 8,740 total XP, awards 38 skill points and 29 stat points across the full climb, and leaves no stored overflow XP that could silently create level 21 later.

Existing saves keep save schema v25. Loaded levels below 1 repair to 1, loaded levels above 20 repair to 20, and cap-level XP normalizes to zero without granting retroactive points.

## Martial ladders

Each permanent martial class begins with two choices and gains one new skill at levels 3, 5, 8, 12, and 16.

| Level | Warrior | Rogue | Ranger |
| --- | --- | --- | --- |
| 1 | Charge, Rally | Stealth, Ambush | Aimed Shot, Pinning Shot |
| 3 | Shield Bash | Throw Knife | Scout Mark |
| 5 | Execute | Smoke Bomb | Volley |
| 8 | Cleave | Hamstring | Broadhead Shot |
| 12 | Whirlwind | Eviscerate | Disrupting Shot |
| 16 | Sunder | Shadowstep | Quick Shot |

The new level-16 skills deliberately fill different tactical gaps:

- Sunder is a reliable setup hit. Its moderate damage is below a finisher, but a hit ends Guard and removes up to two ward turns.
- Shadowstep ignores intervening terrain and moves the rogue beside one enemy before a single cut. It requires an open landing tile and consumes stealth.
- Quick Shot rolls two lighter arrows independently. Armor applies to each arrow, keeping it strong against exposed targets without replacing heavy single-shot damage.

## Spell pacing

Every formula now has an explicit unlock level from 1 through 20; formulas no longer inherit an accidental level-3 fallback. The default progression set exposes a curated cross-school ladder rather than ending after the opening levels.

| School | Selected unlocks in the default progression |
| --- | --- |
| Mend | Heal 1, Ward 1, Cleanse 2, Tree Cover 4, Regenerate 5, Rift Seal 10, Dawn Pulse 12, Sun Brand 16 |
| Ember | Fire Spark 1, Arc Spark 1, Ice Slick 2, Fireball 6, Thunderclap 8, Chain Lightning 12, Cinderstorm 15, Thunder Step 16, Ashen Curse 18, Arcane Tempest 20 |
| Hex | Bind 1, Weaken 1, Grave Hook 8, Ashen Curse 18 |
| Pact | Rift Bolt 1, Summon Imp 2, Soul Veil 4, Pact Brand 6, Rift Step 10, Abyssal Ascendance 18 |

The five new formulas use existing audited combat resolvers and bounded status durations:

- Dawn Pulse: 10 MP, level 12. An 11-power mend that heals the chosen ally and echoes half-strength healing to adjacent allies.
- Cinderstorm: 11 MP, level 15. A 16-power fire burst with adjacent splash and a two-turn bleed test on the primary target.
- Grave Hook: 8 MP, level 8. An 11-power single-target death strike with a two-turn bind test.
- Soul Veil: 10 MP, level 4. A two-turn pact ward whose adjacent wards are one turn shorter.
- Ashen Curse: 12 MP, level 18. A 14-power ember/hex splash attack with a two-turn weakening hex test on the primary target. Hybrid casters use their stronger Ember or Hex training.

## Art contract

The ability atlas expands from 4 by 6 to 4 by 7. Sunder, Shadowstep, and Quick Shot occupy cells 24, 25, and 26; cell 27 remains transparent reserve space. The signature-spell atlas remains 7 by 8 and fills its five reserved cells 51 through 55 with Dawn Pulse, Cinderstorm, Grave Hook, Soul Veil, and Ashen Curse.

The v2.9 atlases preserve every earlier mapped cell pixel-for-pixel. Their source prompts, cleaned alpha sheets, bounds, gutters, and hashes are retained with the runtime art references.

## Validation status

Combined RuleSmoke, focused combat-UI runtime smoke, SpriteArtRuntimeSmoke, full RuntimeBoot, the clean Windows build, canonical package integrity gate, clean-extracted packaged boot, and the nine-capture built-player visual matrix pass with the exact 8,740-XP curve, level-cap normalization, unlock tables, icon cells, and live ability/formula paths. Physical-controller review and a complete human playthrough remain manual release checks.
