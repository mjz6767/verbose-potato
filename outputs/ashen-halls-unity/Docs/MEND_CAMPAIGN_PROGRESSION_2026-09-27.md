# Mend campaign progression — 2026-09-27

This pass follows the confirmed campaign omissions and redundant ward in `SKILLS_SPELLS_CONTENT_AUDIT_2026-09-26.md`. Current names differ from the audit: Hold Sign is Word of Stillness, Circle Ward is Covenant Ward, and Still Water is Wellspring Hymn.

## Player-facing change

Priest and Paladin gain four existing Mend spells in the normal campaign. They keep the same school and level gates; no class identity, save schema, or stored spell list changes are needed. Existing characters at the required level gain the options when their Spellbook reads the campaign catalog.

| Unlock | Spell | MP, unfocused / focused | Tactical decision |
| --- | --- | --- | --- |
| 6 | Word of Stillness (`LNH`) | 7 / 6 | Attempt to deny one visible enemy its next turn. No damage; enemy magic resistance can defeat the cast. |
| 9 | Sanctuary Ward (`SGW`) | 8 / 7 | Short group protection: primary ward 2, orthogonally adjacent allies 1. |
| 13 | Covenant Ward (`TBG`) | 9 / 8 | Longer group protection: primary ward 3, orthogonally adjacent allies 2. |
| 14 | Wellspring Hymn (`SWR`) | 10 / 9 | Group recovery over time: primary regeneration 3, orthogonally adjacent allies 2. |

All have base range 4. Holding the full movement budget while the action remains available grants the existing focus benefit: one less MP and one extra range. Focus does not extend these status durations.

The sole effect-value change is Covenant Ward duration 2 → 3. Previously it cost one more MP than Sanctuary Ward for identical outcomes. Sanctuary Ward remains the cheaper group option. Sacred Aegis remains the cheaper way to provide a three-duration ward to one ally (6 MP), and Prayer of Renewal remains the cheaper three-duration regeneration for one ally (8 MP). Area support rewards keeping a formation, with the existing tactical risk of clustering near enemy area attacks.

Stone Bastion (`GBX`) and Circle of Mercy (`LBC`) remain prototype-only. The campaign already has breakable cover and area healing through Verdant Shelter and Dawn Pulse. Activating these duplicates merely to match another class's spell count would dilute the current choices. Named Mend progression beyond Sun Brand at level 16 remains an independent design question.

## Existing resolver limits retained

- Status application refreshes to the larger of old and new duration. Repeated casts cannot add durations; shorter wards do not erase longer protection.
- Splash reaches living allies at Manhattan distance 1 from the chosen ally. Diagonal allies, distant allies, enemies and defeated allies are excluded.
- A ward expires by one at its recipient's turn start. A duration-1 adjacent ward can protect that ally before its next turn, then expires. It is not a promise of one full global combat round.
- Regeneration grants no immediate healing. Each recipient's turn start heals `4 + campaign depth`, capped by that recipient's maximum health, then reduces regeneration duration. At depths 1 and 4 a fully wounded primary receives at most 15 and 24 health over three turns; each adjacent recipient receives at most 10 and 16 over two turns. Incoming damage and available turns still determine the realized benefit.
- Word of Stillness uses the existing hostile status chance: 86% base, plus the capped Intelligence bonus, minus 7 percentage points per point of enemy magic resistance. Its light tag does not turn it into guaranteed control or damage. A resisted cast still spends its mana and action. Blocking cover also prevents the cast, without spending mana.
- Existing max-duration refresh supports repeated control only through another successful, paid cast. Word of Stillness is single-target, so it cannot deny an entire enemy group.

The generic target preview now distinguishes primary and adjacent status durations. Catalog hints describe the practical role of the four spells without claiming guaranteed hostile control.

## Verification scope

`Assets/Editor/MendProgressionSmoke.cs` calls the production Spellbook, casting gate, `CastFormula`, status application and recipient start-turn effects. It checks both Priest and Paladin at the level before and at each unlock, retains full-prototype access, verifies exact mana spending and focus reach, verifies primary/adjacent durations, exclusions and max-refresh, verifies early/late regeneration payoff and health caps, and drives a fixed hostile roll through both a successful and resisted Word of Stillness. The target preview must disclose `3 target / 2 adjacent turns` for Covenant Ward.

This is executable mechanical coverage, not a claim that a full campaign or a multiplayer friends' playtest has been completed. Coordinated Unity execution and results are recorded in release evidence by the parent task. No independent Unity instance was launched for this content pass.
