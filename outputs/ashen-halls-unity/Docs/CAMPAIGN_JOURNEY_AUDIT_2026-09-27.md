# Connected campaign journey audit — 27 September 2026

This audit extends the [fresh-encounter tactical experiment](CAMPAIGN_TACTICAL_AUDIT_2026-09-27.md) to one persistent party across all eighteen required Chapter I–VI encounters. The initial source was verified v2.34 (`192401a`); runs execute the current v2.35 candidate. It is evidence about a bounded automated policy, not a guarantee that a new player completes the campaign.

The final experiment completed all six campaigns through their real ending state: 108 required battles plus 34 roaming battles, with no defeat, hero reaching zero HP or elixir use. Each party retained its actual wounds, mana, rewards, skill practice, equipment and consumables. The policy used only two camps across all six journeys and the three required town-debrief Recalls per journey. No runtime balance change was made for this audit.

## Reproduction and scope

The Editor entry point is `AshenHalls.Editor.CampaignJourneyAudit.Run`. Parent-coordinated Unity runs produce `QA/campaign-journey-report.md` and `QA/campaign-journey-turns.txt`. Reports are saved after every completed or stopped journey. Exit code zero means the six-run experiment finished; the outcome rows determine whether each party actually won.

Each run starts the production default four heroes with the normal 28 gold, five supplies and three elixirs. Production `BeginGame` establishes the Town Hall start and empty campaign progress. One state and one seeded RNG continue through the entire run. The fixture blocks disk persistence independently of `BeginGame` clearing its lab flag.

The harness does not call the old fresh-fight `Prepare` method, seed quest flags, create rewards, award XP, repair wounds or mana, reset RNG between fights, assume a battle victory, or edit combat HP. It shares only the real-turn combat policy and reflection bridge with `CampaignTacticalAudit`.

Actual cardinal exploration steps use `TryMoveOrUseExplore`; real interior portals connect Town Hall and the town buildings. Native movement advances discovery and roaming threats, and can admit approach encounters. Additional roaming battles are fought and counted separately. Authored sites/NPCs are used only after reaching their own tile or an adjacent legal interaction tile. Chapter travel uses underfoot `Descend` or the gated regional-site interaction. Borin consumes actual sewer proof; the cache and Yara acceptances select actual enabled dialogue choices on the final choice page. The final Vhal victory must set both `GameMode.Victory` and the completed campaign flag.

The route surveys the Chapter III frontier before Yara's IV contract; debriefs the earned Emberglass key before accepting V; surveys the defeated marshal's threshold before Yara's VI contract; and clears sentries, heart and sovereign in order. Returning after Recall uses those same prerequisites and travel interactions. Independent review checked this source-level ordering.

## Policy and limits

| Decision | Prudent | Conserve |
|---|---|---|
| Health threshold at a planned-waypoint decision | Any living hero below 80% | Any living hero below 40% |
| Caster mana threshold | Mage or priest below 60% | Mage or priest below 20% |
| Recovery | Spend real camps until threshold; Recall if supplies are exhausted below threshold or anyone is dead | Same actions, lower threshold |
| Required debriefs | Recall to Midgaard, then walk to Yara and back through the chapter gates | Same |

Recovery decisions occur at the main planned-waypoint boundaries, not immediately after every unexpected patrol during a return leg. Recall really restores and revives the party without spending gold or supplies; its cost here is the subsequent measured walking and any encounters on that route. Level-up restoration, world rewards, skill practice, loot and quest caches all remain production behavior. The walker avoids optional Camp, Town and RecallCircle tiles so their automatic restoration cannot silently replace the declared rest policy.

Growth spends two primary attributes for each Health increase. The warrior spends two Arms points for each Guard point; ranger uses Missile, mage Ember and priest Mend. When actual practice caps the preferred talent, an allowed secondary is used, or points remain unspent if no allowed choice can accept the purchase. Each newly earned equippable item is compared with the production `BestInventoryFit`; positive-score upgrades are equipped through the production inventory method. No purchases, optional quest planning or manufactured signature equipment are included.

The shared combat policy uses legal moves, attacks, abilities, support/damage spells, guards, elixirs and enemy turns. It chooses actions from current production previews using a greedy utility score. It has full board information, can optimize a move and action together, and does not simulate player learning. It lacks multi-turn planning, summons, terrain construction, out-of-combat casting and post-action movement.

Navigation knows the authored waypoint identities and the complete map geometry. It chooses a shortest legal route including portals rather than discovering routes through the visible fog or interpreting prose. No elapsed-time exploration update, rendered input/hit testing, human walking delay or save/reload cycle is simulated. Choice callbacks enforce at most four options and an enabled current-page choice; they do not prove the rendered UI can be read or clicked. The random seed remains fixed per journey, but GUID-based equal-score ordering prevents a claim of identical replay across fresh runs.

## Initial stopped experiment

The initial six journeys are retained in `QA/campaign-journey-v235-initial-policy-stop.md` and `QA/campaign-journey-v235-initial-policy-stop-turns.txt`. They reached nine or twelve required victories and three extra roaming victories, then stopped because the audit attempted to buy Missile above the production skill cap. No party was defeated. This was a policy allocation error exposed by carried skill practice, not a failed campaign admission or game defect.

The continuous Growth policy was corrected to use legal fallback talents or retain points. Independent review also identified possible incidental RecallCircle healing; those tiles are now avoided. Parent review found another harness error: mana recovery compared role aliases with class names, so neither the default `ember` mage nor `mender` priest triggered the intended mana threshold. Classification now uses production `StarterPartyCatalog.SpellSchoolForClass(ClassKey)`. At startup the harness asserts that both default casters trigger both thresholds just below the cutoff and do not trigger at or above it, without changing live mana. The initial runs do **not** validate either mana-rest policy and must not be pooled with the revised experiment as an attrition comparison.

## Final connected experiment

The final six-run experiment completed in `QA/campaign-journey-v235-final.log`. Its report and complete action/travel trace are preserved as `QA/campaign-journey-v235-final.md` and `QA/campaign-journey-v235-final-turns.txt`. All six parties completed all eighteen required encounters and entered the actual campaign victory state. This is one uniform run of the final Editor policy, not a table assembled from separate policy revisions.

| Seed | Policy | Required wins | Extra roaming wins | Camps | Recovery / quest Recalls | Steps | Final level by hero | Remaining gold / supplies / elixirs |
|---|---|---:|---:|---:|---|---:|---|---|
| 42 | Prudent | 18/18 | 7 | 0 | 0 / 3 | 754 | 16 / 16 / 15 / 15 | 1301 / 28 / 20 |
| 42 | Conserve | 18/18 | 7 | 0 | 0 / 3 | 754 | 16 / 16 / 15 / 15 | 1301 / 28 / 18 |
| 727 | Prudent | 18/18 | 4 | 1 | 0 / 3 | 704 | 16 / 16 / 15 / 15 | 1174 / 27 / 20 |
| 727 | Conserve | 18/18 | 4 | 0 | 0 / 3 | 704 | 16 / 16 / 15 / 15 | 1174 / 28 / 20 |
| 1337 | Prudent | 18/18 | 6 | 1 | 0 / 3 | 708 | 16 / 16 / 15 / 15 | 1242 / 27 / 26 |
| 1337 | Conserve | 18/18 | 6 | 0 | 0 / 3 | 708 | 16 / 16 / 15 / 15 | 1308 / 28 / 24 |

Hero order is Maer, Cairn, Luma, Vesh. Across these runs, 142 actual battles ended in victory: 108 required and 34 roaming. The trace contains no hero reaching zero HP and no elixir action. Battles lasted two to five rounds (mean 2.48). Every journey used nine real interior portal transitions and forty planned waypoints. The three quest Recalls occur for Yara's IV, V and VI acceptance; none was an emergency recovery. Gold was earned and carried without purchases; the remaining stock is not a suggested shopping budget or a test of shop balance.

The prudent seed-727 camp occurred after the marshal because Vesh had 40/70 mana, below 60%; it spent one supply and raised him to 48/70. The prudent seed-1337 camp occurred after an early road fight when Cairn had 24/43 HP; it spent one supply and raised him to 37/43. Conserve requested neither camp. These are real costs, but the production level-up restores and required town returns leave little attrition for this policy. The seed-727 rest is shortly before a required debrief Recall: the policy is myopic and does not optimize away that redundant camp.

The Chapter VI journeys entered the sentries at levels 15/15/14/14, versus 14/14/13/13 in the earlier fresh mandatory-path setup. Their extra road/discovery rewards and carried skill practice are earned differences. Sentries and heart each lasted two or three rounds; Vhal lasted three rounds in all six completions. End-of-combat Vhal HP was 391–408 of 410–411 total (95.4–99.5%), before any separate persistent level-up restoration. The final casters remained level 15, so these wins did not rely on their level-16 capstones. The detailed tables' `Post-reward XP` field is the current level's remainder, not lifetime XP.

There is no evidence here of a lethal combat difficulty spike for the informed policy, nor of a need to increase global XP, gold, supplies or healing. Remaining supplies substantially exceed the starting five. This does not establish that an inexperienced player will find the game easy: route knowledge, earned build management, greedy previews and tactical legality are supplied by the harness, while learning and UI interpretation are absent. Equal seeds are not a strictly paired statistical experiment once actions and GUID ordering diverge.

Carried practice is material: all six parties finish with Arms/Missile/Ember/Mend respectively at 99, and Luma retains ten to twelve talent points because her sole allowed talent can accept no further purchase. The numeric equipment policy also produces unconventional legal loadouts. Seed 727 equips the earned Crownward Warblade on Cairn; seed 1337 equips it on Luma. Production `EffectiveWeaponRange`, `IsRangedAttackProfile` and `BaseAttackRange` preserve the ranger's ranged profile and minimum range of five even with this sword. The audit did not override this behavior or fabricate a bow; any future weapon-form restriction is a separate game-design/mechanical change. These are useful follow-up questions, not evidence that the current run used forbidden actions.

## Seed-1337 navigation investigation

The intermediate six-run experiment in `QA/campaign-journey-v235-corrected.log` produced four full victories and two navigation stops. Its report and trace remain preserved as `QA/campaign-journey-v235-revised-before-navigation.md` and `QA/campaign-journey-v235-revised-before-navigation-turns.txt`. Both seed-1337 runs stopped after accepting Chapter VI, on the Chapter IV return leg at `(29,24)`, before entering V or VI again. `regional-site:red-gate-seal` still correctly serves as the IV→V gate in that saved context. The Editor-only diagnostic entry `CampaignJourneyAudit.RunSeed1337` repeated the actual journey and saved complete stop-state JSON plus static/dynamic path checks, now preserved in `QA/campaign-journey-1337-blocked-patrol.md` and its turn trace.

A 33-cell static path existed both with and without healing tiles excluded. Its sole dynamic blocker was the live Red Gate Cinder Pact patrol at `(44,36)`, on depth IV. The original policy required a complete route avoiding every live patrol, although production `TryMoveOrUseExplore` supports attacking a patrol from an adjacent cell before checking movement standability. This was a planner limitation, not a disconnected authored gate.

The Editor policy now first seeks a route avoiding patrols. If none exists, it permits planning through a live patrol only on otherwise standable terrain that is not an occupied habitat; walks the legal prefix; invokes actual directional engagement if still blocked when adjacent; fights any triggered patrol with the same carried party; and replans after the real outcome. It never teleports, marks the threat inactive, awards a victory or edits resources. In the final six-run trace, both seed-1337 parties took this fallback and fought one additional real roaming battle; both subsequently completed all three VI encounters. The static map required no runtime repair.

## Release interpretation

The connected default-party path is reachable and mechanically complete for all three tested seeds under both policies, including earned gear, legal quest transitions, required town visits and a patrol blocking a return passage. The experiment finds ample resources and low late-game pressure for an informed player policy. It supports retaining the current accessible campaign for this candidate, while keeping skill-cap pacing, class-based weapon range and automated equipment scoring as specific follow-up topics. It does not replace manual onboarding, narrative/UI checks, save/Continue verification, tests of other party compositions or a broader seed sweep.
