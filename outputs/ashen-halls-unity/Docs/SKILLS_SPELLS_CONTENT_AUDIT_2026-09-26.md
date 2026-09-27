# Skills and spells content audit — 2026-09-26

Source baseline: `project/outputs/ashen-halls-unity`, inspected while the coordinated v2.30 retail release was being prepared. Catalog counts and numbered source references below describe that pre-fix baseline. The lab fixes were prepared separately during the release freeze and preserve the retail package. Catalog sizes and campaign balance are unchanged.

## Access fixes

- The caster toolbar now selects Mage, Warlock, or Priest directly and links to **Skills Lab**.
- Priest begins at maximum level with all 16 Mend spells. **Stage** keeps the Priest selected and supplies nearby wounded, afflicted allies, enemy targets and a dispellable field.
- Skills Lab contains dedicated Warrior, Rogue and Ranger testers at maximum level, with seven skills each. Selecting a class opens its production Skillbook; **Spells Lab** returns to casters.
- Class selection clears stale targeting and turn state, respects resolving actions and preserves the lab's campaign save block. Defeated martial testers require Reset instead of reviving on potentially occupied cells.
- New regression coverage checks actual book cards, all Priest and martial unlocks, staged legal targets, real Heal/Cleanse/Light Bolt/Cleave actions, switching, and save refusal.

These changes repair access. The design findings below remain separate follow-up work; no new skills or spells have been added, and no spell costs or effects have been changed. Validation results are recorded after execution rather than inferred from source review.

## Findings in the pre-fix baseline

Warrior and Priest content exists and has live combat resolvers. The immediate Beta Lab problem is access: martial promotion stops at level 3, Priest craft promotion stops at level 2, and the caster toolbar provides direct Mage/Warlock selection but no Priest selection. The lab therefore hides much of the already-authored Warrior and Priest progression behind level gates. Fixing these paths should precede adding spells merely to increase counts.

The content audit does identify narrower issues: Warrior and Priest stop gaining new named actions at level 16; Priest loses six formulas in the campaign content set; and Circle Ward is mechanically identical to an earlier spell while costing more mana. These are balance/design follow-ups, separate from lab access.

## Coverage

Counts exclude universal Attack, Guard, Move, Elixir, and End Turn. Shared-school spells count once per class. A larger count does not by itself imply a stronger or more complete class.

| Class | Full prototype actions | Default campaign actions | Level-1 named choices | Last named unlock | Main role |
| --- | ---: | ---: | --- | ---: | --- |
| Warrior | 7 skills | 7 skills | Charge, Rally | 16 | Closing distance, protection, melee control and finishing |
| Rogue | 7 skills | 7 skills | Stealth, Ambush | 16 | Stealth, bleed setups and repositioning |
| Ranger | 7 skills | 7 skills | Aimed Shot, Pinning Shot | 16 | Ranged accuracy, pinning, marking and interruption |
| Wizard | 33 spells | 17 spells | Fire Spark, Arc Spark, Bind, Weaken | 20 | Broad Ember/Hex damage, terrain and control |
| Mage | 20 spells | 12 spells | Fire Spark, Arc Spark | 20 | Elemental damage, terrain, area damage and mobility |
| Warlock | 23 spells | 13 spells | Bind, Weaken, Rift Bolt | 18 | Hex control, summons, draining, pact support and transformation |
| Priest | 16 spells | 10 spells | Heal, Ward | 16 | Healing, cleansing, warding, light damage and safe terrain |
| Paladin | 16 spells | 10 spells | Heal, Ward | 16 | Melee/Guard training with Mend support; no separate martial deck |

All eight classes are selectable in the inspected `CharacterCreationCatalog.Classes`; `AshenHallsGame.Tavern.cs:523` passes that entire list into character creation. Wizard receives the union of Ember and Hex formulas, deduplicating shared spells. Paladin and Priest share Mend with different Arms/Guard/Mend training.

There are **56 unique formulas** overall and **25 martial definitions**: 21 permanent-class skills plus 4 Demon Arts. Abyssal Ascendance, a level-18 Warlock spell, temporarily supplies Rift Pounce, Abyssal Whirl, Soul Rend and Dread Roar; those four are not additional permanent Warlock spells.

Evidence: `Assets/Scripts/Content/AbilityCatalog.cs:8`, `FormulaCatalog.cs:75`, `ContentSetCatalog.cs:12`, `StarterPartyCatalog.cs:25`, `CharacterCreationCatalog.cs:23`; temporary form routing is in `Assets/Scripts/Legacy/AshenHallsGame.Combat.cs:5684`.

## Martial progression

All three permanent martial classes share the same authored cadence and all seven skills survive the default-content filter.

| Level | Warrior | Rogue | Ranger |
| --- | --- | --- | --- |
| 1 | Charge; Rally | Stealth; Ambush | Aimed Shot; Pinning Shot |
| 3 | Shield Bash | Throw Knife | Scout Mark |
| 5 | Execute | Smoke Bomb | Volley |
| 8 | Cleave | Hamstring | Broadhead Shot |
| 12 | Whirlwind | Eviscerate | Disrupting Shot |
| 16 | Sunder | Shadowstep | Quick Shot |

Warrior has seven distinct functions: path-aware charge with stun, self-bracing/adjacent ward, shove/collision stun, low-health finisher, two-target strike, adjacent-enemy sweep, and Guard/ward removal. Enrage is an additional automatic damage passive at half health or less, not an eighth selectable skill. Execute requires a target at 35% health or below; Whirlwind requires adjacent foes; Charge requires an open landing path and cannot be used while webbed. A test arena must stage those conditions.

All seven have dispatch and implementation paths: `AshenHallsGame.Combat.cs:16009`, `:16089`, `:16237`, `:16291`, `:16429`, `:16454`, `:16464`, `:16496`, `:16720`; Enrage is at `:16943`. No missing Warrior resolver was found by source inspection.

There is no authored named Warrior taunt, ally-interception or counterattack skill in this catalog. Those would expand tactical identity, but their absence is not a broken unlock. Four levels without a new named action (17–20) also affect Rogue/Ranger, so a Warrior-only capstone change should be weighed against all martial progression. Stat and talent growth continues in those levels.

## Priest spell ladder

| Level | Code | Spell | Default campaign? | Function |
| --- | --- | --- | --- | --- |
| 1 | OIC | Heal | Yes | Single-target healing |
| 1 | TBQ | Ward | Yes | Three-turn single-target ward |
| 2 | NVC | Cleanse | Yes | Removes poison, bleed, web, stun, sleep and hex |
| 3 | OBL | Light Bolt | Yes | Direct light damage |
| 4 | GBH | Tree Cover | Yes | Breakable temporary cover |
| 5 | TNC | Regenerate | Yes | Single-target regeneration |
| 6 | LNH | Hold Sign | No | Enemy stun |
| 7 | GBX | Stone Block | No | Stone cover |
| 8 | HLC | Hallowed Circle | Yes | Sanctuary terrain |
| 9 | SGW | Sanctuary Ward | No | Ward target and adjacent allies |
| 10 | SRF | Rift Seal | Yes | Remove rituals/hostile fields; shared with Ember |
| 11 | LBC | Circle Heal | No | Heal target and half-strength adjacent healing |
| 12 | DWP | Dawn Pulse | Yes | Stronger version of the area-heal pattern |
| 13 | TBG | Circle Ward | No | Same ward effect as Sanctuary Ward, higher cost |
| 14 | SWR | Still Water | No | Regeneration for target and adjacent allies |
| 16 | SBN | Sun Brand | Yes | Splash light damage and primary-target stun chance |

The default Priest is not limited to healing: it has damage, cleansing, cover, regeneration, sanctuary, dispelling and area healing. Its default set does lack an area ward, targeted control before Sun Brand, and any named unlock after level 16. Six removed entries explain the difference between its 16-spell prototype and 10-spell campaign experience. Some removals cut duplicated roles, while others remove tactical options; that curation deserves an explicit design decision.

Priest and Paladin sharing Mend follows `StarterPartyCatalog.SpellSchoolForClass`, rather than being accidental catalog duplication. A desire for class-exclusive prayers is a new identity decision. No revival/resurrection formula is authored; adding it would require rules for defeated-unit targeting and encounter recovery, not just a catalog row.

### Confirmed redundant upgrade

`FormulaCatalog.cs:84` gives Sanctuary Ward (L9) 8 MP, range 4, ally target, shield duration 2, splash and arc. `:87` gives Circle Ward (L13) the same gameplay fields at **9 MP**. Both use the generic status resolver at `AshenHallsGame.Combat.cs:17824`: primary duration 2, adjacent allies duration 1. The shield application at `:19399` refreshes to the larger duration; it does not add or stack it. No formula-code-specific mechanical advantage for Circle Ward was found. It is therefore a strictly more expensive version of the earlier ward in the inspected rules; presentation differences do not repair that.

Suggested minimal follow-up for review: retain Circle Ward's current 9 MP and raise its duration to 3. The existing resolver would then provide primary duration 3 and adjacent duration 2, a distinct longer group ward while Sanctuary Ward remains the cheaper group option and Ward the cheaper single-target option. This uses bounded refresh behavior and requires no new resolver. It remains an untested balance proposal, **not an implemented change**. Verify primary/adjacent durations, focus discount, refresh semantics and early/late combat sustain before accepting.

Circle Heal and Dawn Pulse similarly share a pattern but have an actual power/cost difference (8 power/9 MP versus 11 power/10 MP), so they are not the same strict-downgrade defect.

## Other spell schools

These inventories explain the caster-count differences. Shared formulas appear in each matching school but only once within a class total.

- **Ember, 20 formulas:** Fire Spark and Arc Spark L1; Ice Slick L2; Fire Floor L3; Cold Lance L4; Burn Cover L5; Fireball L6; Flame Jet L7; Thunderclap L8; Frost Bind L9; Iceburst and Rift Seal L10; Fireburst L11; Chain Lightning L12; Meteor Shower L14; Cinderstorm L15; Death Burst and Thunder Step L16; Ashen Curse L18; Arcane Tempest L20.
- **Hex, 15 formulas:** Bind and Weaken L1; Web Snare L2; Night Veil L3; Sleep L4; Drain Life L5; Mind Break L6; Poison Gas L7; Grave Hook L8; Poison Burst L9; Doom Circle L10; Wither L11; Dream Smoke L14; Death Burst L16; Ashen Curse L18.
- **Pact, 8 formulas:** Rift Bolt L1; Summon Imp L2; Soul Veil L4; Pact Brand L6; Summon Lesser Demon L8; Rift Step L10; Summon Greater Demon L14; Abyssal Ascendance L18.

Mend/Ember share Rift Seal. Ember/Hex share Death Burst and Ashen Curse. Warlock has Hex plus Pact. Wizard has Ember plus Hex. Equal school or class counts are not required for balance: resource cost, target availability, multi-turn payoff, damage/control scaling and encounter pressure matter more.

## Access defects and source checks

1. `AshenHallsGame.Tavern.cs:166` and `:210` put both labs in `full-prototype`; campaign filtering is not the reason for lab omissions.
2. `PromoteMemberForMartialTesting` at `Tavern.cs:476` and `PromoteMartialLabUnits` at `Combat.cs:7268` only raise levels to 3. This unlocks Charge/Rally/Shield Bash while leaving Execute/Cleave/Whirlwind/Sunder locked despite the lab claiming skills are unlocked. The same gate affects Rogue/Ranger.
3. `ApplySpellLabCraft` at `Combat.cs:6975` and `:7011` only raises Priest/Paladin to level 2; a fresh Priest therefore knows Heal, Ward and Cleanse while the other 13 full-prototype Mend entries remain level-locked. Mage and Warlock receive separate maximum-level tester kits.
4. `Presentation/BetaLabToolbarRules.cs:191` exposes direct Mage and Warlock actions, with staging copy scoped to those two; Priest and Warrior lack equivalent direct selection in that caster toolbar.
5. Spellbook generation (`CombatAbilityModal.cs:189`) enumerates all active matching-school formulas and marks higher levels locked; Skillbook generation (`:223`) likewise displays the martial catalog with level locks. This is separate from genuinely absent content.
6. Combat spell access checks content set, school and level (`Combat.cs:18607`, `:18690`, `:18705`, `:18732`). All authored effect categories have resolver branches (`:17633` through `:17865`); source inspection found no missing Priest effect handler.
7. The 31-entry Visual-only Tour is a curated effects sampler, not a complete skill/spell index. It contains five of the seven Warrior skills (omitting Shield Bash and Cleave) and only Light Bolt and Hallowed Circle from the Priest list. The direct class books are the complete gameplay inventory; tour membership does not control whether a skill can be used. The effects chat independently confirmed the tour's limited purpose during coordination.

Recommended order: repair lab selection/promotion and stage useful support targets; verify all 7 Warrior and 16 Priest actions through production books; then review the Circle Ward downgrade, deliberate campaign exclusions, and level-17–20 identity/capstone opportunities. Do not add raw spell count to compensate for an access defect.

## Source-audit limits

Counts were computed from `FormulaCatalog` school membership and explicit level mappings intersected with `ContentSetCatalog` allowlists. Martial counts and runtime dispatch were cross-checked in source. Existing progression smoke coverage is in `Assets/Editor/EarlyProgressionRuleSmoke.cs`; this audit did not rerun it or claim player-visible behavior was freshly verified. Coordinated lab changes and current build/player evidence should be recorded separately after the release freeze.

## Executed validation

The focused `BetaLabClassCoverageSmoke.Run` passed in Unity 6000.3.18f1 on 2026-09-26 (local date), exit 0. It verified the actual title entry, 16 visible unlocked Priest cards, seven visible unlocked cards for each martial class, legal staged targets, actual Heal/Cleanse/Light Bolt/Cleave outcomes and resource consumption, stale-target cleanup, both lab switches, resolving-action input locks and campaign save refusal. Log: `QA/beta-class-coverage/focused-smoke-licensed-r2.log`.

The first restricted editor launch could not reach the existing license service; the licensed retry exposed an invalid new test-asset GUID. The GUID was corrected before the successful compile/test run. Neither failed attempt was counted as validation.

All 13 embedded build gates subsequently passed, and the separate `v2.30.0-beta-dev` Windows archive passed a clean-extracted Development-title startup check. Its compiled assembly matches the assembly inside the archive. The existing v2.30 retail ZIP hash remains unchanged. Build source: `fe3c9760e48a8e332adb172cae2349afe2b1b9eb`. Package hashes, exact checks and limitations: `Docs/ReleaseEvidence/v2.30.0-beta-class-coverage.json`.

The hidden D3D player screenshot was uniformly black and was rejected; it is not visual acceptance evidence. The editor offscreen fallback reported a 640x480 batch viewport despite requested larger dimensions and correctly rejected capture below the supported minimum. No fresh screenshot is accepted; the optional capture helper is preserved outside the canonical repository. Automated modal card/geometry coverage passed, but a human visual/controller playcheck and new campaign balance remain untested. The package is a local tester artifact and has not been pushed or uploaded.

## Independent review of staged lab fixes

The staged `Assets/Scripts/Legacy/AshenHallsGame.Combat.cs` and `AshenHallsGame.Tavern.cs` were reviewed against their saved baselines after the content audit. They add a maximum-level Priest selector, support-target staging, maximum-level martial testers, explicit Warrior/Rogue/Ranger selection, a real Rogue in the martial party, and navigation between the two labs. The generic Paladin craft branch preserves Paladin identity. Combat activation reuses the existing turn reset, clears pending board targeting and disabling conditions, and opens the production book.

One introduced collision risk was found and corrected during review: the initial martial selector could fall back to a dead tester and revive it in place, even though a living unit could now occupy that cell. The revised selector and bulk promotion accept only living testers and direct the player to Reset when necessary. The new Priest stage positions form a legal support cluster, put enemy light-spell targets in range, and leave nearby open cells and a gas field for terrain and Rift Seal testing under the normal four-member lab formation.

No further gameplay blocker was found in this source review. Two navigation hints were flagged to match the visible Skills Lab/Spells Lab button labels. This is a source review of staged work, not proof of successful compilation or a played-through lab; runtime verification remains required after integration.
