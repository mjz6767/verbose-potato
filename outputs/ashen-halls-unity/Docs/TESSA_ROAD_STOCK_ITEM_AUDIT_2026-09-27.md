# Tessa road stock: martial choice and equipment clarity

The v2.35 pass gives Tessa two persistent, independent purchases after Varkh falls: a +3 Bridgehook Spear and a +3 Stonecleft Axe, 76 gold each and one of each per campaign. Purchases enter the Pack for deliberate comparison. They do not replace equipment, require Tessa's introductory purchase, grant XP, or alter the campaign objective. The parent release workflow owns the shop, art integration, and executed validation logs.

## Confirmed gaps and bounded changes

The preceding eleven signatures contained no reach weapon or axe. Spears were already supported in combat and some random loot, but no selectable class maps to the `pike` role; warrior and paladin starter/shop pools instead use `shield` and `ward`. Tessa nevertheless discussed axes absent from all player weapon pools and described a spear engagement penalty the combat rules do not apply.

The new signatures use the existing stable-ID Guard-action and weapon-hit hooks. No new status, resource, equipment requirement, or combat action was added. Ordinary `battle axe` joins only nonstarter martial/general loot pools. Its family has base 3–7 damage and speed 5, modified by the existing quality rules; it receives the existing martial fit score. Starter equipment and Tessa's 32-gold introductory offer retain their original pool distributions. Form-based legacy slot recognition now includes axes while explicit quest/material/key slots remain protected.

| Item | Authored stats | Intrinsic | Practical tradeoff |
| --- | --- | --- | --- |
| +3 Bridgehook Spear | 3–8 physical base damage, speed 7, Health +1 | Holdfast: +1 Guard only when taking the Guard action | Two-cell basic attacks with a defensive action option; lower damage than Stonecleft and no life drain |
| +3 Stonecleft Axe | 5–9 physical base damage, speed 5, Strength +1 | Sure Hew: +2 hit on weapon attacks | Slower physical damage option without spear reach; below Varkh's sword in damage, speed, and sustain |

These are authored item values, not final attack damage. Attributes, weapon quality, skill, defense, armor and combat conditions subsequently apply. At Chapter III's ordinary 36–56 gold victory rewards, one purchase costs roughly two ordinary fights. The player may buy either, both, or neither; the recurring supply and enchantment services remain separate spending choices.

## What reach, weapon forms, and focus actually do

On Maer or a paladin, the spear uses Arms at Manhattan distances one and two. Adjacent pressure does not reduce its reach. A target's Guard counter applies only at distance one, so an attack at distance two avoids that counter. Reach remains a basic-attack property: Ambush, Iron Rebuff and Reaping Cleave retain their authored one-cell ranges. Existing melee reach uses the melee forecast path rather than the missile line-of-sight path; this pass does not redesign that rule.

Class and role precedence still matter. A rogue uses finesse scaling even with the spear or axe; the Counterweight Knife retains its greater speed, Agility bonus and resisted bleed role. A ranger retains its role's unengaged range floor and uses melee/Arms when engaged. Mage and warlock roles retain range three with a knife; priest focus/staff forms offer range four. None of these is a hard equip restriction. On the default party, the spear creates a genuine reach choice for Maer, while Varkh's sword remains a stronger sustain option. In a rogue/paladin party the new weapons offer alternatives without removing the knife's agility/speed advantages or extending class powers.

Pack and loot range labels now identify their wearer, for example `Range 3 for Luma` or `Reach 2 for Maer`. Owned equipment uses its actual owner as the compact summary context. The runtime range values and recommendation scores remain unchanged. Generic `spell focus` behavior wording was removed: focused casting comes from retaining the action and full movement without moving, not from holding a focus. The old generic claim that an item with no listed effects was a clean upgrade was replaced with neutral wording. Tessa's revised guidance separately explains displayed attributes, range, and the focused-casting action state.

Axes already use heavy attack feedback and gain one extra physical ritual-object disruption in melee, within the existing cap of three. That family behavior is retained; Sure Hew itself grants accuracy, not a new damage proc.

## Identity, art and meaningful verification

Stable IDs `bridgehook_spear` and `stonecleft_axe` append at icon indices 11 and 12. Their factories are `SignatureItemCatalog.CreateBridgehookSpear()` and `CreateStonecleftAxe()`. The approved 1774×887 two-cell atlas is `road-stock-item-atlas-runtime-v2.35.0.png`, SHA-256 `8F90383293CEB79EDCA697FD88780E954C55780998D974166998CCF43DF1C035`. Its slender silhouettes require the production loader's 75% minimum transparency and 8% minimum visible fraction; the item smoke calls that loader before testing native adapters.

`RoadStockItemSmoke.RunOrThrow` checks actual equipment ownership and loadouts, two-cell Arms attacks with and without adjacent pressure, real guard-counter behavior, the Guard command, a hit roll that misses without Sure Hew and lands with it, ordinary axe generation, unchanged starter pools, wearer-labelled Pack/loot summaries, focus before/after movement, enchantment expiry, and a validated production SaveService roundtrip. It also checks exact art bytes, visible silhouettes, transparent midpoint columns, and native Pack/loot cell routing after label changes.

`NativeRoadStockSmoke.RunOrThrow` exercises the actual native dialogue buttons and pagination for browse/review/cancel/purchase and advice. It covers both purchase orders for default and rogue/paladin party compositions, insufficient funds, changed eligibility/mode/conversation/campaign instance, duplicate confirmations, independent receipts, unchanged existing equipment/party data, production reload, and persistent sold stock after returning home. Capture review must separately verify the 960×600 and 1280×720 text layouts. The release QA logs record the executed results; this design document does not claim a completed playthrough.

The accompanying CSV records authored item values and the approved shop prices for review. Save schema 27 remains unchanged.
