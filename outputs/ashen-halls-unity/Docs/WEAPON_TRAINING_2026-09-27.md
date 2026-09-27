# Weapon form and basic attack rules

Candidate: v2.36, following the unchanged v2.35 telemetry baseline. The goal is a weapon whose reach follows its form, consistent inventory/combat previews, and ranger techniques that require an actual missile weapon. This document records the scoped mechanical change; the dated v2.35 item and journey reports remain historical evidence of the former behavior.

## Why the previous behavior was misleading

`WeaponRange` fell back to a class range for unrecognized or melee forms; `EffectiveWeaponRange` applied another starting-role floor; `RecalculateMember` retained the greatest previous range; and `BaseAttackRange` separately restored a ranger minimum of five. `IsRangedAttackProfile` also treated every ranger as a projectile user. A ranger's sword could therefore shoot from five cells, while a caster's knife could strike from three without a sight check. Equipping a high-damage sword could retain the class's ranged advantage. Ranger shot techniques separately accepted every weapon and used that weapon's damage, so changing only basic attacks would leave the same contradiction in the skillbook.

The old untouched default mage/priest focuses started at three/one cells, although reequipping those forms gave a caster four. The new rule deliberately normalizes these derived values instead of preserving a starter-only exception. Starter attributes, skills and weapon damage are unchanged.

## Shared profiles

| Form | Basic range | Distant attack skill | Clear sight | Adjacent enemy |
| --- | --- | --- | --- | --- |
| Sword, axe, ordinary knife and other hand weapons | 1 | Arms | Adjacent target | Stays 1 |
| Spear, pike, glaive, halberd, lance | 2 | Arms | Required through intervening sight-blocking terrain | Keeps reach 2 |
| Bow or crossbow | 4; 5 with ranger/bow training | Missile | Required | Basic fire becomes range-1 Arms |
| Sling, thrown knives or darts | 3; 4 with ranger/bow training | Missile | Required | Basic fire becomes range-1 Arms |
| Focus, staff, orb or scepter | 4 for a spell-trained class; 1 otherwise | Arms | Required beyond an adjacent cell | Keeps its basic range |

Focus channeling includes mage, wizard, warlock, priest and paladin; an implement does not teach spells to a martial class. Equipment itself has no new class restriction. A ritual knife remains a knife, and a ritual bell remains a hand weapon. All point-to-point basic sight tests use the established terrain LOS function, including cover and ritual targets. A target obstacle does not block the strike against itself; intervening terrain can.

`WeaponAttackRules` resolves a recognized inventory form first, then a known signature's canonical form, then saved/display weapon text. Unknown named weapons default to melee. The rule distinguishes physical projectile delivery, focus delivery and reach, so a distant focus or spear strike checks LOS without silently using Missile skill or acquiring the bow's engagement penalty. Existing class-first attribute scaling and damage formulas are unchanged by this patch; class training may still determine a weapon's damage stat.

Actual default basic ranges become Maer 1, Cairn 5, Luma 4 and Vesh 4. Inventory summaries, equipment assignment, enchantment synchronization, stat recalculation and player combat forecasts use the same profile. The display name alone cannot override an explicit inventory form.

## Techniques and spells

Hawkeye Shot, Pinning Shot, Arrowstorm, Broadhead Shot, Staggering Shot and Twinshot require a bow, crossbow, sling or throwing weapon. Their existing six-cell ranges, costs, damage and engagement rules remain. Both ability availability and target legality return the same equipment requirement, covering the skillbook, target previews and execution. A refusal spends no action or random roll and grants no practice, damage or status effect. The descriptions use shot/projectile wording where needed for the admitted missile forms.

Hunter's Mark remains usable with any weapon. A ranger holding a sword retains basic Arms attacks, movement, Guard, elixirs and that marking technique. The rogue's Serrated Throw remains a separate short-range knife technique available alongside another held weapon; its three-cell range and existing formula are unchanged. Other martial ranges are not extended by a spear.

An equipment-blocked shot displays the exact required weapon in the selected card's main effect area, replacing an unusable damage preview. This keeps the reason visible at the compact resolution without scrolling to tactical notes. The usual damage preview returns when compatible equipment is held.

## Equipment decisions

The fit score is unchanged. A numerically stronger blade can still be the highest-scoring item for a ranger, but changing delivery or reducing basic range now requires deliberate review. This member-aware rule is used by loot's quick-equip preview and commit, upgrade labels/filtering, and both sides of reassignment. Direct Pack Equip/Swap remains available after comparison; no new class restriction or confirmation dialog is added.

The full and compact comparisons lead with the actual consequence, before intrinsic or damage changes. Cairn moving from a longbow to the Crownward Warblade sees `Range 5→1; shooting skills unavailable`; returning to the bow sees `Range 1→5; supports shooting skills`. The compact Pack rows use `Range 5→1; shots disabled` and `Range 1→5; shots enabled` so the decisive word fits at 960 pixels. The full wording was directly observed clipping that word in the first capture, prompting the shorter compact copy rather than a smaller font. This fixes two old disclosure gaps: an ordinary sword could previously bypass strategic-item review, and the compact comparison's first-two-change limit could hide the range loss behind intrinsic and damage changes. Technique level requirements still apply after compatible equipment is restored.

Loot arrival guidance uses the same review rule instead of calling a role-changing weapon a clear upgrade. Party summaries count actual ranged weapon profiles; a ranger holding a sword retains the ranger class label and reports melee equipment with Arms rather than ranged pressure with Missile.

One compact reassignment limitation remains: both recipients are checked by the review rule, but the leading consequence describes the selected recipient. If Cairn gives his bow to Maer and receives Maer's sword, Maer's gained range appears first while Cairn is identified only as needing Review with the replacement sword; Cairn's lost shooting techniques are not spelled out in that compact line. The opposite direction, selecting Cairn to receive a sword, explicitly displays his loss and is tested. A follow-up should prioritize any displaced owner's lost capability in the compact swap copy. This release retains the review gate and explicit Swap decision; it does not claim complete two-recipient consequence disclosure.

Spell range remains `formula.Range` plus the existing stationary focused-casting bonus. A caster's sword or staff does not change spell range, spell eligibility, mana cost or the focus action state. Focused Casting still depends on an unspent action and movement, not the item being named focus.

Authored enemy and summoned-unit range/profile branches remain intact. Demon form still overrides held equipment with range-1 Hex attacks. Enemy AI and boss signatures are outside this weapon change.

## Saved loadouts

Range is derived data. Existing weapons, physical item IDs, ownership, enchantments, learned skills and damage values remain; obsolete ranger-sword or caster-blade range is recomputed from the held form. Range normalization follows equipment/signature repair and includes the party's saved combat units, including downed heroes. It does not reconstruct actors or alter HP, mana, positions, initiative, turn budgets, statuses or practice state. Demon-form actors retain their adjacent override.

For saves predating inventory instance IDs, the existing unique name/base-stat ownership match ignores only cached range; every other identity and authored-stat comparison remains. This prevents a corrected derived range from orphaning an otherwise unique historical item. Ambiguous matches remain ambiguous. The range repair is idempotent and needs no new persistent field. The coordinated release's practice-budget schema change is independent.

## Verification

`WeaponTrainingSmoke.RunOrThrow` is registered in the Rules audit after `CombatAccuracySmoke`. Its coverage includes all eight selectable classes across eighteen forms; actual equipment assignment and recalculation; first eligible Arms/Missile practice in separate fresh battles; blocked unit, cover and ritual attacks; six disabled native ranger cards plus direct execution; unchanged spell and independent technique ranges; real SaveService roundtrips through loaded-state adoption; old ownership reconstruction; and enemy/summon/demon isolation. It checks failed-action state and RNG preservation, not only the profile's return values.

`RoadStockItemSmoke` now expects every knife to have melee range and retains its actual spear/Guard/enchantment checks. The ranger longbow still reports five through compatible training.

The focused Unity weapon smoke passed in `QA/weapon-training-v236-native-all.log`. The disabled-card fixture explicitly uses the native All filter: Hunter's Mark remains ready, so the book's normal Ready filter correctly excludes equipment-blocked shots. The test selects each visible shot, checks its detail identity and disabled action, and invokes both native and direct paths without relaxing refusal assertions.

The final focused weapon smoke passed in `QA/weapon-training-v236-final.log`. Its disclosure regression uses a high-scoring ordinary bow-to-sword change to prove quick-equip refusal in preview and direct callback. It checks the actual native Crownward Pack action text and invokes deliberate Equip, then the inverse bow choice; reassignment must also lead with the role loss. It also checks the main-effect equipment requirement and the actual held-weapon party guidance.

The final 36-image native capture at `QA/steel-and-study-content/20260927-213431-357` passed the documented visual review. Twenty-eight decoded images were identical to the already reviewed set; four changed main weapon/skillbook views and four new comparison views were reviewed directly. Both compact bow-to-sword and inverse warnings show the complete decisive wording at 960×600 and 1280×720. `VISUAL_REVIEW.md` in that directory records the combined review.

Older presentation fixtures needed coherent equipment. `CombatInputSmoke` had a ranger with no role or weapon name; it now supplies an actual longbow, while preserving its separate-confirmation checks. `RuntimeBootSmoke` temporarily renamed a live actor to a bow while leaving its linked inventory form unchanged. Its synthetic ranger probes now update the backing item form/name and member label too; the temporary HUD probe restores these values afterward. Its available-recipient quick-equip probe now assigns actual ordinary longbows before offering the higher-stat longbow, preserving the test's skip-downed and explicit-commit purpose without asking a changed delivery to bypass review. These are controlled presentation fixtures, not evidence of a legal campaign gear acquisition.

The combat HUD probe also rebuilds its view after restoring the temporary ranger state, because production command views share a reused buffer; comparing a cached ranger command with restored caster legality was a stale fixture comparison. Its selected-Attack/no-target check now explicitly selects Attack for that probe and restores the prior action, instead of assuming the turn policy left Attack selected. The expectation still requires the blocked model, no-target sublabel, native blocked styling and focusable command. The real inventory swap regression now expects range one for the ranger receiving a broadsword and range four for the warrior receiving a longbow, retaining all its ownership/stat checks.

The final full-suite corrections were fixture assumptions, not additional production defects. The selected-Attack probe previously depended on which action the turn policy had left selected. The duplicated blade-loot review probe previously expected `Compare others` even when the new equipment rule correctly required a form tradeoff. That fixture now equips real ordinary bows before offering the blade, making every possible best-fit recipient a known delivery change, and requires native `Review tradeoff` without quick equip. The later same-form longbow probe explicitly checks `Compare others` in both model and native UI. Exact-item selection, duplicate-row focus, skip-downed recommendation and equipment-commit checks remain intact.

The native Pack regression uses the real bound screen in an active presentation root while the game host remains inactive to avoid boot/loading side effects. It opens through the normal Armory toggle and invokes the rendered equipment actions. Text assertions inspect the overlay's separately owned root canvas, as production creates that canvas outside the screen component's hierarchy. Fixture disposal explicitly releases only the canvases tracked by its own lifetime components, because Unity does not invoke destruction callbacks for never-active components. It also releases the modal's generated fallback art and navigation ownership. A before/after ornament-owner assertion guards this cleanup.

Final source-only compilation passed 170 runtime and 69 Editor files with zero diagnostics in `QA/source-compile/20260927-215108-8c6f6e43`. The final licensed Unity Full audit passed **all 13 gates** in `QA/project-audit/20260927-215134-1d1c6f94/unity.log`, including Rules, UI lifetime isolation, combat UI runtime and full runtime boot. These final gates include the corrected fixture setups and all weapon, review, persistence and presentation regressions described above.

## Connected campaign observation

The corrected candidate journey observation completed all six campaigns (three seeds, two recovery policies), totaling 108 mandatory and 34 roaming victories with no hero downed. The candidate combines these weapon rules with the coordinated practice, Growth, Guard and accuracy changes; it cannot isolate each change's effect. The complete outcomes and limitations are in `Docs/CAMPAIGN_COMBAT_TELEMETRY_2026-09-27.md` and `QA/campaign-journey-v236-candidate-report.md`.

The unchanged greedy equipment policy chooses the Crownward Warblade for Cairn on seeds 727 and 1337, accepting the now-real loss of shooting. This is a legal explicit equipment choice, not a representative human preference or a reason to secretly change fit scoring. The disclosure fix addresses that decision in the player UI. All six endings remain low risk under the tested policy; the evidence supports coherent weapon costs and continued completion, not a solved endgame difficulty curve. The initial candidate harness incorrectly duplicated the former ritual sight rule; its stops are preserved and explained in the telemetry report, with the corrected run using production obstacle legality.
