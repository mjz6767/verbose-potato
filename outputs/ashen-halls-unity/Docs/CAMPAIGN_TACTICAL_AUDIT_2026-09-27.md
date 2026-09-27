# Campaign tactical audit — 27 September 2026

The baseline and tuned experiments completed **126 independent fights**, all victories with four surviving default heroes and **no elixirs consumed**. The retained Chapter VI tuning lengthens fights for an unspent party and lets the bosses exert more pressure. A fully trained party using this policy still wins quickly; the proposed four-to-six-round target for that build was not met.

This is evidence about one capable automated policy, not a human win-rate estimate or proof of campaign completion from a new game.

## Evidence and reproduction

- Entry point: `AshenHalls.Editor.CampaignTacticalAudit.Run` in `Assets/Editor/CampaignTacticalAudit.cs`.
- Initial licensed Unity run: `QA/campaign-tactical-v234.log`, successful exit 0, 63 fights completed.
- Preserved initial results: `QA/campaign-tactical-v234-baseline.md`.
- Preserved initial action trace: `QA/campaign-tactical-v234-baseline-turns.txt`.
- Successful tuned Unity run: `QA/campaign-tactical-v234-tuned.log` (exit 0); preserved results and trace: `QA/campaign-tactical-v234-tuned.md` and `QA/campaign-tactical-v234-tuned-turns.txt`.
- Current-run outputs: `QA/campaign-tactical-report.md` and `QA/campaign-tactical-turns.txt`.
- Reward/level evidence: `QA/campaign-pacing-report.md` and `Docs/CAMPAIGN_PACING_2026-09-27.md`.

The existing pacing ledger's Chapter I route uses the three real sewer encounters. Its historical values remain valid for the v2.33 baseline; the new HP-dependent reward difference is quantified below.

## What was actually exercised

For each of seven encounters, seeds 42, 727 and 1337 were run with three builds: starter gear with growth unspent, quest gear with growth unspent, and the same quest gear with growth spent. These are separate fully rested encounter trials, with three available elixirs per trial. Previous victories are assumed only to construct the entering XP and item history.

The fixture uses the default party factory, actual enemy templates and ranks, authored placements and obstacles, combat XP calculation, actual XP awards and current chapter milestone receipts. It does not grant extra XP to reach a preferred level. It excludes optional patrol/discovery XP and previous combat skill practice. One explicit optional item choice is the sewer safe-room Stormglass Conductor; later equipment is earned by preceding mandatory quests. The new Honest Weight rewards are not included.

Growth is spent through `PartyGrowthRules`. Two of every three attribute points go to the relevant attacking attribute, with the remainder going to Health. Warrior talent spending splits Arms and Guard 2:1; ranger, mage and priest invest in Missile, Ember and Mend respectively. Equipment is admitted and assigned through the actual inventory methods. Neither stats nor gear are changed during a fight.

Each accepted player command uses `CombatController` and actual `Attack`, `CastFormula`, martial ability or ritual-disruption resolution. Movement uses production reachable costs and `MoveActiveTo`. Enemy turns use `EnemyAct` and its normal finishing path. Initiative, start/end-turn statuses, hazards, ritual reinforcements, real hit rolls, damage traits and action budgets remain active. Only presentation-time waits are fast-forwarded. Candidate movement previews use a detached copy of the acting unit; no damage, HP, enemy removal or forced victory is introduced.

The policy chooses among damage, healing, ward, cure, regeneration, physical ritual disruption and Rift Seal. It favors reachable damage and finishing wounded enemies, penalizes hazardous positions and crowding, and forfeits post-action movement. It uses class strikes freely where the production game permits them.

## Initial outcomes

Every cell below is three victories with four survivors. HP is remaining combat-unit HP divided by entering HP, before persistent-party XP level-ups can refill anyone. Rounds are the production round counter at completion.

| Encounter | Entering levels, humans / casters | Starter-unspent rounds / HP | Quest-gear-unspent rounds / HP | Quest-gear-spent rounds / HP |
|---|---|---|---|---|
| Varkh | 3 / 3 | 6.3 / 83% | 5.3 / 89% | 5.7 / 87% |
| Gloam Warden | 5–6 / 5 | 5.3 / 93% | 4.7 / 93% | 4.3 / 100% |
| Ashen Pact Warden | 8 / 8 | 3.3 / 92% | 3.3 / 93% | 2.7 / 96% |
| Crownroad Marshal | 11 / 10 | 4.3 / 92% | 4.0 / 95% | 3.0 / 100% |
| Crownwatch | 14 / 13 | 2.0 / 94% | 2.0 / 95% | 2.0 / 100% |
| Ritual Heart | 14 / 13 | 3.0 / 100% | 3.0 / 100% | 2.0 / 100% |
| Vhal Rakh | 14 / 14 | 3.3 / 93% | 3.3 / 96% | 3.0 / 98% |

Mean ending HP can conceal a wounded individual. For example, the seed-42 starter Varkh trial drove Maer to 14 HP before a legitimate heal; it finished at 108/167 total HP after a real summoned demon joined the battle. That is a meaningful early threat despite the eventual win.

The initial fixture did not populate every preceding Chapter VI story flag. Its Vhal victory therefore ran the generic encounter-completion path. Its combat damage, turns and outcome remain valid encounter evidence, but that run does **not** prove the epilogue was reached. The current harness populates legal prior story history, asserts Chapter VI admission, and requires both `GameMode.Victory` and the final completion flag after a won Vhal fight. It also labels trace rows with the round captured before acting; the preserved initial trace labels could advance to the next round during the action's lifecycle continuation.

Trace HP/MP snapshots are taken after the action and its normal turn continuation. They can therefore include the next actor's start-turn poison, bleed or regeneration; the action label alone is not a damage-source attribution log.

## Why the initial finale was soft

At seed 727 with spent growth, Cairn's first Broadhead Shot removed 50 of Vhal's 78 HP. Vhal survived long enough to deliver one 18-damage Crownfire, but the party finished with 371/379 HP and 153/170 MP. This was a legal level-14 ranger using a starter longbow, not an invented late-game weapon.

In Crownwatch, a first shot left the crossbowman at 4 HP. The enemy approach placed the knight, mage and cinderling close enough for Chain Lightning to kill the knight and crossbowman and leave the other two at 11 and 10 HP. The fight ended at full party health in the next round. The mage's available level-12 spell worked as designed; the small, clustered enemy group gave it exceptional value.

The stationary Heart had 68 HP and range 4. After its companions were removed, a range-6 Chain Lightning killed it from outside its pulse reach. Its weakness to shock is readable and useful, but a one-cast kill from a permanently safe distance prevented the encounter's own mechanic from creating pressure.

These traces support testing authored Chapter VI endurance and formation changes. They do not justify globally reducing XP or delaying unlocks again. The current milestone ladder is what makes the intended late-game choices reachable.

## Retained Chapter VI tuning

The retained trial keeps four enemies per encounter and all existing boss damage caps, resistances, weaknesses and cover rules. `ApplyMeteorCrownEncounterTuning` in the Meteor Crown combat partial applies only during authored Chapter VI setup, through `ApplyEncounterPlacements`:

| Encounter | HP change | Placements in roster order |
|---|---|---|
| Crownwatch | +24 HP per enemy | (8,0), (10,7), (8,3), (10,4) |
| Heart | Heart 160 HP; companions +18 | (9,3), (9,7), (7,0), (7,5) |
| Vhal | Vhal 210 HP; companions +28 | (9,3), (7,0), (10,7), (8,6) |

The Heart keeps movement zero and its 14-damage pulse / three-health drain caps, with range increased to 6 so a long-range attacker can provoke a real reply. Vhal retains range 5 and Crownfire's 18 primary / six adjacent damage caps. Existing obstacles remain. The wider initial spacing reduces immediate chain deletion, though the production AI can still regroup. HP floors preserve rank differences and any higher legitimate maximum, cannot stack on repeated setup, preserve existing wounds and never resurrect a defeated unit.

At seed 727 the actual authored HP values are:

| Encounter | HP by roster | Total HP before → after | XP per survivor before → after |
|---|---|---:|---:|
| Crownwatch | Crossbow 80, mage 65, knight 87, cinderling 74 | 210 → 306 | 105 → 117 |
| Heart | Heart 160, bone priest 60, reaver 64, shade 69 | 207 → 353 | 93 → 112 |
| Vhal | Vhal 210, lesser demon 89, drow priest 79, cinderling 78 | 240 → 456 | 107 → 134 |

The higher HP also yields **58 legitimate additional base XP** through the existing victory formula. Seed 727's direct-route ending is now 4,691 base XP, previously 4,633; ending levels remain human 15 / nonhuman 14. Entering levels in these three-seed tactical trials did not change. No separate XP bonus, stat-growth rule or formula unlock was changed.

The actual rerun produced the following means, with three wins, four survivors and zero elixirs in every row:

| Encounter / build | Rounds before → after | Ending HP before → after |
|---|---:|---:|
| Crownwatch / starter-unspent | 2.0 → 3.3 | 94% → 91% |
| Crownwatch / quest-gear-unspent | 2.0 → 3.0 | 95% → 93% |
| Crownwatch / quest-gear-spent | 2.0 → 2.0 | 100% → 97% |
| Heart / starter-unspent | 3.0 → 4.3 | 100% → 97% |
| Heart / quest-gear-unspent | 3.0 → 4.0 | 100% → 100% |
| Heart / quest-gear-spent | 2.0 → 3.0 | 100% → 98% |
| Vhal / starter-unspent | 3.3 → 5.3 | 93% → 87% |
| Vhal / quest-gear-unspent | 3.3 → 4.3 | 96% → 89% |
| Vhal / quest-gear-spent | 3.0 → 3.0 | 98% → 94% |

All **nine tuned Vhal trials** reached the real `GameMode.Victory` and `MeteorCrownDefeated` flag. The legal-history fixture correction affects the post-combat story path, not the combat damage rules. Chapter II–V numerical rows remained unchanged in this rerun. The round-label correction changes trace attribution, not initiative processing.

The four-to-six-round aspiration for the prepared build was **not achieved**. Retaining this limited pass is still preferable to increasing HP solely to force that number: the seed-727 prepared party legitimately dealt 78 with Frost Lance and 77 with Dawnlance into Vhal's readable weaknesses, then 45 with Vanguard Rush. Repeated Rush and Staggering Shot can also deny turns under the existing martial rules. Larger isolated HP increases can extend cleanup without adding a meaningful decision. Global spell, XP or cooldown redesign is not supported by this small experiment.

The smoke suite now checks exact authored HP floors, repeat-setup idempotence and wound preservation, non-leakage to the prototype Final Gate or other depths/content sets, unchanged offensive power and magic resistance, valid/spread placements, stationary Heart movement, range-six pulse/forecast agreement, range-seven rejection and preserved stone-cover counters. The existing tests still cover original pulse/fire damage caps and elemental resistance.

## Limits and next checks

The policy has full board information and instant access to production previews. It is not an optimal solver and does not model novice uncertainty, formula discovery, misclicks or reading time. It omits summons, terrain construction, enchantments, multi-turn planning, retreats and post-action repositioning. Spell/ability resolution is exact, but utility estimates for martial hit chance, statuses and splash are approximate. It cannot establish a statistical human win rate from three seeds.

The decision policy is deterministic for a given state. Production factories generate random GUIDs, which can resolve otherwise exact initiative or chain-target ties differently between runs; exact replay is not guaranteed solely by the seed. Those identifiers are not overwritten by the harness.

Fresh rest makes these encounter-accessibility tests. They do not establish consecutive-fight resource sustain or that the party can earn all preceding victories. A subsequent campaign-through run should carry actual wounds, mana, casualties, combat skill practice and rewards between battles and use the real Recall/camp choices. Failed or timed-out runs must remain in the report rather than receiving recovery edits during combat.

Independent source review of the Honest Weight integration found no mechanical blocker: the quest preserves usual site services; evidence requires explicit inspection at depth III and current adjacency; resolution and item-choice flags prevent repeated rewards; and the discount uses the same adjusted service profile in charging and presentation. Its optional gear and 120 XP are outside this tactical baseline.
