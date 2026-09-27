# Honest Weight item and economy pass

This pass adds one independent choice between two +3 signature rewards for Maud's optional Chapter III investigation. The charm's disposition does not restrict the equipment choice. It uses the existing item, equipment, status, damage, enchantment, and save systems; save schema 27 stays unchanged.

## Existing coverage

The preceding nine signatures were three swords, one caster staff, one bow and four armor pieces. Their roles were life drain, defensive bracing, resisted stun, poison protection, death/mind protection, broad nonphysical protection, ward removal, accuracy, and cold protection. There was no named finesse knife or reach weapon. The existing bow and cold wrap remain distinct Edda rewards.

| New reward | Authored values | Existing mechanic | Tradeoff |
| --- | --- | --- | --- |
| +3 Counterweight Knife | 3–7 physical base damage, speed 11, Agility +1 | Measured Cut: a successful basic hit can apply bleed for two turns at 20% base chance | Less damage and no healing compared with Varkh's sword; faster finesse option and a possible setup for Eviscerate |
| +3 Measurekeeper Jack | Armor quality +3, effective armor 3, Agility +1 | Load Bearing: reduce physical damage taken by 1, respecting the one-damage floor | Trades Ratcatcher's Health +1 and poison reduction for physical protection; less defense/stats than Gloam mail while avoiding its heavy movement penalty |

The knife's base damage is subsequently modified by the ordinary weapon bonus, stats, skills, defense, and combat rules. “3–7” is not a promise of final damage. Both pieces enter the Pack for deliberate equipping.

Measured Cut uses the existing hostile gear status rule: `20% + clamp((Intelligence - 10) × 0.6%, 0%, 12%) - MagicResist × 7%`, clamped to 0–100%. At Intelligence 10 against resistance 2 this is 6%; it can reach zero. A missed attack does not proc. Duration refreshes by maximum rather than stacking, and an existing longer bleed is preserved. Normal bleed ticks deal two physical damage on each affected unit's turn, subject to the existing mitigation and one-damage floor. The new item does not replace the rogue's reliable skill-based bleeds. Its description discloses Intelligence and resistance.

Both intrinsic additions use stable signature IDs in `SignatureItemRules`. The knife's existing signature status takes precedence over a status inferred from an elemental name prefix; enchantments still change its affinity. Permanent magic survives temporary magic and expiry. No additional combat hook or proc system was introduced.

## Equipment and economy findings

Equipment has no hard class, Strength, Agility, or level requirement. Classes, roles, forms, and names influence stat scaling, basic-attack profile, range, and fit suggestions. Arms governs melee attacks; Missile governs ranged attacks. Class abilities and formulas remain restricted by their own class/school and level rules. A piece named “focus” does not itself grant focused-casting bonuses: focus comes from casting before moving with the action and full movement still available. Existing role range floors remain: the knife is range one on a rogue, while an Ember/Hex wearer retains range three. Pack range reflects its selected or recommended wearer, not a class-independent weapon requirement.

At depth III an ordinary victory grants 36–56 gold before story extras. Tessa's early weapon costs 32 and Borin's armor costs 28, each once. Temporary enchantment costs 18 for three victories; permanent binding costs 90. The quest's 30-gold disposition is smaller than one ordinary depth-III victory. Retuning saves four gold per repeat Dusk Market supply purchase and exceeds the immediate thirty-gold payout after eight purchases. This is a convenience-versus-immediate-cash choice; neither branch locks the equipment reward.

Random item quality can already reach +5. Combat loot quality floors are depth/2, at least +2 for veterans and +3 for elites, with damage also raised to enemy-power floors. The new +3 rewards provide guaranteed roles rather than universal best gear. Existing later +4/+5 armor and +6 sword rewards are preserved.

Two isolated audit corrections were made:

- Crossbow generation previously matched `bow` first and incorrectly used speed 9. The specific crossbow branch now precedes the generic bow branch, restoring speed 5. Longbows/slings remain 9, knives 11; normal quality modifiers still apply. Existing saved weapon speeds are not rewritten.
- Borin's fixed hauberk already had quality +2, chain's extra defense +1, and Health +1. Its old name said +1 and its purchase review quoted only +2 armor. The new name says +2; the review explicitly quotes three armor as quality +2 plus chain +1. Stats and the 28-gold price are unchanged. Existing saved display names are not rewritten.

Remaining gaps recorded without expanding this pass: there is still no named spear/reach reward; Tessa mentions axes but no role or random weapon pool produces them. Some generic “spell focus” wording describes form rather than granting a separate focus mechanic. These are future content/copy decisions, not mechanics silently added to these rewards.

## Identity, art and verification

IDs `counterweight_knife` and `measurekeeper_jack` append at catalog indices 9 and 10. The original seven-item atlas and Edda's two-cell atlas remain untouched. The new two-cell atlas is `honest-weight-item-atlas-runtime-v2.34.0.png`, 1774×887 RGBA, SHA-256 `D0907CFB27CD776CBA60691AA58376FD13960AB7FD2DDCAD6662ADCE8BF33C52`.

`HonestWeightItemSmoke.RunOrThrow` exercises the actual Pack/loot adapters with renamed stable identities, atlas pin/transparency/seam, real equipment assignment, weapon scaling/range, actual basic-hit and miss paths, hostile status resistance, duration/turn expiry, Eviscerate interaction, physical-only reduction and its floor, enchantment overlay/expiry, and a validated production SaveService roundtrip. It also checks the crossbow family speed and real Borin purchase description.

`HonestWeightContentCapture.Capture` renders native Pack, loot, Journal, both free inspection menus, and every page of Maud's offer/progress/evidence/reviews/reward/aftermath/account/services at 960×600 and 1280×720. Its disposable fixtures do not load or save a campaign or execute a reward transaction. These captures require visual review and are not a full campaign playthrough. The release's QA logs record executed results.

Final native capture review passed for `QA/honest-weight-content/20260927-195418-804`: 62 PNGs, of which 44 match the previously reviewed baseline pixel for pixel, six were corrected, and twelve add service/craft/story coverage. Every changed/new image was directly inspected. The knife's full Agility line now fits at 960×600; preaccept, accepted, and completed Maud menus expose the quest and craft submenu, whose rune/story pages and return controls remain visible at both sizes. `VISUAL_REVIEW.md` and `pixel-comparison.json` in that QA directory record the observations and per-image evidence.
