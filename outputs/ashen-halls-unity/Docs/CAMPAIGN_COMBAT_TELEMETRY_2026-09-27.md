# Combat pressure telemetry — 27 September 2026

This Editor-only experiment explains the low late-campaign pressure in the [connected v2.35 journey](CAMPAIGN_JOURNEY_AUDIT_2026-09-27.md). The v2.35 report and trace remain unchanged. `CampaignJourneyAudit.RunTelemetryV236` runs the same six persistent journeys against the unchanged baseline runtime and writes separate `QA/campaign-journey-v236-baseline-*` files.

**Evidence preservation:** the retained baseline used the verified v2.35 runtime at checkpoint `9f763ad4351a9096f654b432a88fb89e6d823073` (packaged source `e75a2863e43835762cc19d5f1feb887b80e49c3e`), with the Editor observer added before candidate runtime changes. `RunTelemetryV236` names an output path; it does not restore or pin that runtime. Do not rerun this named baseline entry from the candidate checkout, because it would overwrite the retained baseline with candidate behavior. Any reproduction must use the baseline runtime and a separate output destination.

## Questions and measurements

The question is whether low damage to the party comes from few enemy attack opportunities, inaccurate attacks, mitigation, healing, or an unusually strong player action. Those causes must be measured separately.

The observer records battle entry/end skill vectors for every fight. Detailed phases cover Chapters IV–VI: player movement, player action resolution, enemy AI resolution, player/enemy lifecycle continuation, and queued turn/round advancement. Each record contains immutable unit and obstacle state, actor/target identities, existing new combat log lines and floating-feedback serials. Movement and action timing are therefore distinguishable from the next actor's automatic regeneration, damage-over-time or skipped turn.

For each active actor and living opponent, the record includes production basic-attack legality, its block reason, range, distance, line of sight, hit chance and damage forecast. Enemy snapshots add their chosen target, strict special reach, special chance parameter and attack eligibility after zero through five-or-more movement points. The observed move budget comes from production remaining movement. Pre/post snapshots show whether an enemy gained a legal attack or special opportunity after moving but could not use it under current turn rules.

Each unit includes its actual skill values, attack profile, defense, agility, wounds, mana and statuses. Damage-reduction snapshots report current Guard, Ward, gear, race and demon-form deductions by damage type, plus resistance, weakness and Hex state. These are conditions and production forecasts, not a replay of the damage RNG.

## Observation guarantees and limits

The observer never invokes an attack, cast, movement, status application, skill improvement, random-number method or mutation. The shared harness wraps the single already-existing production action with observation before and after it; it does not repeat or replace that action. Each snapshot compares the entire serialized game state and the recursively read internal fields of its `System.Random` before and after all helper reads. A mismatch stops the journey as an observation failure. Presentation fields and caches are not claimed to be immutable; they do not supply gameplay outcomes to this policy.

The helper reads are basic attack forecasts, enemy target scoring, line of sight, special reach, movement/attack eligibility and damage-reduction functions. The observer does not call spell previews. The existing tactical policy still computes its own normal spell previews to choose actions. All policy actions, support spells and enemy turns remain production calls.

Floating `-damage` text comes from production damage resolution after mitigation and before HP is capped at zero. It can include overkill. Positive float text sometimes reports offered healing rather than effective healing. The summary's HP loss/restoration is the net change for each unit within each observed phase; simultaneous damage and healing within one phase require inspecting its feedback and log. Counterattacks share their triggering action phase. Coordinate-to-unit associations can be ambiguous after pushes or when a corpse shares a tile, so JSONL retains all candidate unit IDs instead of silently choosing one.

The runtime does not retain raw damage rolls or failed special-chance rolls. The observer therefore does not claim exact rolled raw damage, mitigation percentages, or the number of hidden special attempts. Special reach is geometric eligibility, not proof a cast was attempted. Illegal basic forecasts intentionally report zero hit/damage; zero in such a record does not mean the enemy has zero accuracy. Skill deltas also cannot reveal practice attempts after a talent reaches its cap.

The `combat.jsonl` file is the detailed evidence; `combat.md` is a summary and skill ledger. Ordinary journey `report.md` and `turns.txt` are also retained. Equal seeds are not a proof of identical replay because fresh GUID ties can differ. The observer's state/RNG checks are the direct non-interference check.

## Baseline hypotheses from the preserved v2.35 trace

In seed 42 prudent, Vhal starts at 210 HP, moves from `(9,3)` to `(6,3)` on his first AI turn and deals no damage. He subsequently takes a 100-damage Frost Lance and a 56-damage Charge, then dies to Broadhead Shot before his second actor turn. His encounter ending at high party HP does not, by itself, implicate excessive party damage reduction.

The same journey's 160-HP Ritual Heart takes 112 Chain Lightning damage before its first turn, pulses Luma for 14, restores three HP, then dies to the next round's Volley. Here the special works and causes damage, but burst damage limits how many times it can act.

Source inspection shows that enemy specials are tried before movement. A movement-only enemy turn can therefore finish in special range without retrying that special. Basic attacks after movement are separately governed by `EnemyCanAttackAfterMove`; the existing long-move limit is deliberate prior behavior, not automatically a defect. The baseline measurement will quantify how often these conditions occur before any runtime tuning.

## Baseline run

The baseline completed all six actual campaigns, preserving 108 required and 34 roaming victories. Source compilation passed before execution; `QA/campaign-journey-v236-baseline.log` records completion. No sampling operation changed the serialized game state or RNG fingerprint. An independent offline check reconciled every VI battle's starting-to-ending party HP with the sum of observed phase losses minus restoration, with no accounting mismatch. The baseline and candidate paths remain separate.

Across the eighteen Chapter VI fights, enemies received **84 AI action windows**. Seventy-one caused no party HP loss.

| Observed enemy result | Windows | Interpretation |
|---|---:|---|
| Advance without attack | 40 | Movement/positioning consumed much of the short encounter. |
| Attack cover | 15 | Production attack feedback here concerns obstacles, not hero damage. |
| Basic attack | 17 | Six hits and eleven misses. |
| Offensive special | 7 | Six Heart Pulses and one Cinder Trail. |
| Support special | 5 | Five Grave Wards. |

Forty-six windows spent movement that the existing rules prohibit following with a basic attack. Eighteen windows gained geometric special reach after moving; specials are only tried before that movement in the baseline. These categories overlap and are not additional turns. Among legal basic forecasts present before enemy actions, hit chances ranged from 18% to 50%. The seventeen actual basic attempts had six hits, causing only thirteen party HP loss in total. The sample is small and equal-policy branches share much of their random history; these are observed counts, not estimated population rates.

The six Hearts each acted once, each landed a 14-HP pulse and each restored at most three HP through it. Their combined 84 damage was the largest source of party pressure. They entered those turns with only 48–69 HP remaining from their 160 maximum. Burst damage limited exposure to the working special.

Vhal received nine actor windows across the six final battles: four long advances and five basic attacks. Four basic attacks missed; the fifth dealt seven fire damage. **He cast no Crownfire.** In the four long advances he still lacked geometric special reach after moving, so simply allowing a post-move cast would not fix those positions. In both seed-727 policy branches, a one-step move created special reach to two heroes, but the existing flow proceeded to a basic attack that missed. Later eligible windows still produced no cast; the failed chance rolls are not exposed and must not be inferred from the snapshot.

Only four automatic turns were lost to stun in all eighteen VI battles, affecting companions rather than either unique boss. Stun locking is not the dominant measured explanation in this baseline.

| Player action | Uses in VI | Actual enemy HP removed within its action phase |
|---|---:|---:|
| Cairn: Broadhead Shot | 31 | 2015 |
| Luma: Chain Lightning | 17 | 1921 |
| Luma: Frost Lance | 6 | 574 |
| Maer: Charge | 18 | 521 |
| Vesh: Dawnlance | 9 | 489 |
| Cairn: Disrupting Shot | 13 | 417 |

Broadhead Shot and Chain Lightning alone removed 3936 of the 6695 observed enemy HP losses, approximately 59%. The denominator includes all action and lifecycle phases and enemy healing; these are actual removed HP, not inflated overkill feedback. The complete JSONL records other actions and their outcomes.

The party lost 118 HP and recovered 55 during VI combat phases. All 55 observed recovery came from automatic regeneration, including a ten-point regeneration float when only five HP was missing; this confirms why numeric feedback cannot be substituted for effective healing. The six Heart Pulses account for 84 loss, basic hits thirteen, Cinder Trail seventeen and lifecycle effects four. Fullparty percentages at the ending therefore hide both low incoming opportunity and a smaller amount of real regeneration.

The full 142-fight entry/end skill ledger isolates actual combat practice from Growth purchases between battles. Its per-campaign primary-skill totals were:

| Hero / primary skill | Ranks gained from practice | Fights with an observed positive gain | Level when first observed at 99 |
|---|---:|---:|---:|
| Maer / Arms | 54–60 | 16–17 | 12–15 |
| Cairn / Missile | 57–59 | 11–13 | 9 |
| Luma / Ember | 54–58 | 14–16 | 9–11 |
| Vesh / Mend | 34–38 | 14–17 | 14–15 |

These are gained ranks, not inferred practice attempts. Later actions at 99 contribute zero to this ledger even when they exercise the skill. The old practice rate therefore demonstrably consumed much of the skill ladder before the finale, independently of the HP-pressure finding.

Mitigation matters for particular hits but does not explain the many empty attack windows. One Drow Mage hit Vesh for one mind damage while Ward supplied three and gear supplied one additional reduction. An Old Cinderling hit Luma for one fire damage even without Ward or gear reduction; her race contributed only one. The observer does not know those exact raw rolls. Source inspection supplies a separate, auditable reason for weak basics: `MakeEnemy` scales `Power` with depth but sets nonzero `DamageMin`/`DamageMax` from template power and rank alone. `AttackRules.BuildDamageProfile` uses those explicit damage values rather than the depth-scaled fallback Power, then subtracts target Defense and ArmorBonus. Party Defense already includes ArmorBonus in `RecalculateMember`, so armor contributes twice here and to basic hit defense. The VI enemies' recorded normalized talents are 1, which also explains why a later accuracy curve affecting ranks above 25 will leave their skill contribution unchanged. These are design/implementation questions retained for review, not silently corrected in an observational experiment.

## Candidate comparison protocol

`CampaignJourneyAudit.RunTelemetryV236Candidate` writes only `campaign-journey-v236-candidate-*` files. It uses the same travel, recovery and tactical scoring policy. Its Growth fallback allows Luma to attempt production-legal Arms spending after Ember is capped; rejected purchases leave points unspent. The recorded baseline retains those mage points. The candidate runtime changes shared weapon reach/sight/technique eligibility, limits practice to one rank per exercised skill per encounter, adds bounded deliberate-Guard training, makes Arms available to mages, and diminishes accuracy contribution above rank 25 to `25 + (rank - 25) / 3`. Enemy AI, enemy HP and direct damage formulas are unchanged. This combined run is not an isolated causal estimate for each change.

The first candidate observation completed but contained five **policy assertion stops**, not combat defeats: both seed-42 and seed-727 policies stopped after seven mandatory wins, and seed-1337 prudent stopped after eight; seed-1337 conserve completed. The harness duplicated ritual eligibility using the old projectile-only sight condition. Production correctly rejected a focus/reach ritual attack through cover. `CampaignTacticalAudit.BestChoice` now calls production `CanAttackCombatObstacle` for ritual candidates, including detached prospective movement positions. No runtime rule changed for this repair. All ten original report, log, trace, telemetry and stop-state files are preserved under `QA/campaign-journey-v236-candidate-obsolete-ritual-policy*`.

## Corrected candidate outcomes

The parent confirmed the corrected observation process exited successfully; `QA/campaign-journey-v236-candidate.log` records six completed observations. All six actually reached the production campaign ending: **108 mandatory and 34 roaming victories**, matching the baseline's 142 fights. No hero reached zero HP in either trace, and neither policy used an elixir. All snapshot state/RNG invariants passed. An independent offline check again reconciled each VI battle's entry/end party HP against observed phase losses and restoration.

| Seed / policy | Camps, baseline → candidate | Candidate final HP / maximum | Candidate final mana / maximum | Candidate gold / supplies / elixirs |
|---|---:|---:|---:|---:|
| 42 / prudent | 0 → 3 | 389 / 409 | 119 / 175 | 1366 / 25 / 29 |
| 42 / conserve | 0 → 0 | 389 / 409 | 119 / 175 | 1366 / 28 / 29 |
| 727 / prudent | 1 → 6 | 395 / 412 | 139 / 173 | 1146 / 22 / 26 |
| 727 / conserve | 0 → 0 | 395 / 412 | 139 / 173 | 1166 / 28 / 25 |
| 1337 / prudent | 1 → 2 | 370 / 410 | 100 / 177 | 1246 / 26 / 22 |
| 1337 / conserve | 0 → 0 | 400 / 412 | 114 / 177 | 1251 / 28 / 21 |

All six finish at levels `16/16/15/15`; each uses the same three required quest Recalls and zero recovery Recalls. Walking counts remain 754, 704 and 708 steps for seeds 42, 727 and 1337, respectively. The eleven candidate camps are the prudent threshold policy's choices, not proof eleven camps are necessary: conserve completes without any, and three prudent camps occur just before a mandatory Yara Recall that fully restores the party. Remaining supplies and elixirs still show a large surplus. These resource counts retain the connected journey's disclosed recovery and navigation limits.

| Chapter VI measure, all eighteen fights | Baseline | Candidate |
|---|---:|---:|
| Enemy AI windows | 84 | 96 |
| Windows causing no party HP loss | 71 | 79 |
| Advances without attack | 40 | 46 |
| Attacks against cover | 15 | 13 |
| Basic attacks: hits / misses | 6 / 11 | 5 / 16 |
| Offensive / support specials | 7 / 5 | 12 / 4 |
| Movement disallows following basic | 46 | 56 |
| Special reach first appears after moving | 18 | 32 |
| Actual party HP lost / restored | 118 / 55 | 215 / 70 |
| Net party HP loss | 63 | 145 |
| Actual enemy HP lost / restored | 6695 / 89 | 6670 / 64 |

The candidate's 215 lost HP comprises Heart Pulse 69, Crownfire 68, Cinder Trail 26, Burning Pact 20, basic attacks seven and lifecycle effects 25. All seventy restored HP came from automatic regeneration. There were nine stun skips, including two Heart and two Vhal skips; the baseline had four companion skips. Legal pre-action enemy basic forecasts ranged from 18% to 47%, and the five actual basic hits caused only one or three damage each. Enemy basics remain weak in these encounters even when they connect.

Vhal receives eight candidate AI windows: four Crownfire casts, two missed basics and two long advances. Both seed-727 policies expose a hero to two casts of seventeen damage each. Both seed-42 Vhals miss Luma with a basic attack and die before another action. Both seed-1337 Vhals advance three points and gain special reach only after moving; they cannot attack after that long advance and later lose a turn to stun. Final encounter rounds are `2/2/3/3/3/3`, compared with six three-round baseline fights. Crownfire is demonstrably reachable with unchanged AI, while its limited opportunities remain a separate question.

The candidate Hearts receive eight AI windows: five pulses for thirteen or fourteen damage and three missed basics. Their first-turn HP ranges from 58 to 160, compared with 48–69 in the baseline. Heart encounters now last `3/3/3/2/3/3` rounds rather than `2/2/3/3/2/3`. Those changes provide some longer exposure, without establishing a difficult finale.

## Skill and equipment interpretation

The candidate skill ledger verifies exactly one rank per positive-gain skill/encounter pair. No default hero finishes with a primary skill at 99:

| Hero / primary skill | Candidate combat-practice ranks | Candidate final skill | Baseline final skill |
|---|---:|---:|---:|
| Maer / Arms | 18–23 | 67–72 | 99 |
| Cairn / Missile | 18–25 | 88–95 | 99 |
| Luma / Ember | 22–25 | 87–90 | 99 |
| Vesh / Mend | 16–19 | 81–84 | 99 |

All candidate Growth points are spent legally. Luma never reaches the Ember cap, so the newly allowed Arms fallback makes no purchased-Arms difference in these runs. Baseline Luma had ten to twelve unspent talent points after reaching 99. The connected evidence directly supports a more gradual skill ladder while preserving the earned level and spell-unlock route.

Equipment selection materially changes the action mix. The unchanged `BestInventoryFit` score values basic damage, range, role fit and attributes; it does not fully price the loss of class techniques. Seed 42 equips an earned crossbow on Cairn and retains shooting techniques. Both seed-727 and seed-1337 policies equip the Crownward Emberglass Warblade on him, with real one-cell reach and no shooting techniques. Across those four VI runs Cairn makes sixteen basic attacks and nineteen Guards. This is legal play by the fixed equipment heuristic, not an optimized ranger build, and it prevents attributing all changes in pressure to the accuracy or practice rules.

| Player action in VI | Baseline uses / removed enemy HP | Candidate uses / removed enemy HP |
|---|---:|---:|
| Luma: Chain Lightning | 17 / 1921 | 26 / 2556 |
| Cairn: Broadhead Shot | 31 / 2015 | 10 / 740 |
| Maer: Charge | 18 / 521 | 24 / 639 |
| Vesh: Dawnlance | 9 / 489 | 17 / 694 |
| Luma: Frost Lance | 6 / 574 | 10 / 768 |
| Cairn: basic attack | 0 / 0 | 16 / 183 |
| Cairn: Guard | 1 / 0 | 19 / 0 |

Chain Lightning alone now removes about 38% of observed enemy HP, compensating for much of the lost ranger technique output. Equipment, changed RNG consumption, encounter roster choices and fresh GUID ties also change rewards and later decisions. The 96-window sample does not isolate individual rule effects or establish a population hit rate.

## Recommendation and retained follow-ups

Retain the bounded v2.36 progression and weapon-rule changes, with the existing limitation that this policy still finds a low-risk route through the finale. All six journeys finish; skill ranks remain below their cap; equipment shape now has its intended mechanical cost. This supports accessibility and rule coherence, not a claim that endgame challenge is solved.

Keep enemy AI and damage unchanged for this release. A later controlled experiment can separately examine a single special roll retained across a legal one-step reposition, without giving long moves an extra action or rolling twice. Explicit enemy damage values versus depth-scaled Power, and the double contribution of party armor, also need a stated design decision before adjustment. A second equipment policy that preserves ranger technique access would help distinguish encounter balance from `BestInventoryFit` scoring. The current evidence does not justify blanket healing/Ward reductions, more global XP changes or additional boss HP increases.

`QA/record_v236_evidence.py` records the actual six-row outcomes and battle counts, hashes both telemetry sets and the preserved policy failures, validates the unchanged v2.35 archive and existing art, and inspects every supplied capture's dimensions and hash. It is an ignored local helper. Final release evidence is written only after the parent completes packaging and supplies the final audit, compile and capture paths.
