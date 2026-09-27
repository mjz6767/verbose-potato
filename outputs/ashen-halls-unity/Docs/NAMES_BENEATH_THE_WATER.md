# Names Beneath the Water

An optional Chapter I story that stays available to returning parties. The town remembers people as well as victories. Edda asks for help recovering a crew roll; Sera restores the names; Edda consents to either a private remembrance or a public account delivered to Brann. Both resolutions offer the same reward choice, so a social decision does not hide a stronger item.

## Playing it

1. Speak to Edda west of the tavern and ask what you can do for her.
2. Clear Foul Runoff as part of the normal cistern contract. Return to the Sewer entrance and choose to search behind the sluice wheel. Parties that already finished the contract can search immediately after accepting.
3. Ask Sera to compare the damaged roll with the temple records.
4. Return to Edda and decide together how to remember her companions.
5. Deliver the private remembrance to Sera or the public account to Brann.
6. Choose one keepsake, or leave the decision for later. Either Sera or Brann can reopen the reward choice.

The Journal tracks this favor after the main campaign entries. It never replaces the campaign objective or changes chapter access. The keepsake enters the Pack without changing anyone's equipment. The reward grants 8 base world XP once, using the established party/race XP rules.

## Equipment

| Keepsake | Base equipment | Intrinsic |
| --- | --- | --- |
| +2 Traveler's Knot Bow | Physical longbow, damage 3–6, speed 9, +1 Agility | Steady Hand: +2 hit on weapon attacks, subject to normal hit caps |
| +2 Vigilkeeper Wrap | Armor +2, +1 Intelligence | Cold Vigil: reduce cold damage taken by 1 |

These are early-game sidegrades. They sit beside the existing +2 sewer weapons and below the campaign's later +3 to +6 reward ladder. The bow gives modest reliable accuracy rather than the Stormglass Conductor's stun; the wrap protects against one damage family rather than the Mirrorweave mantle's broad protection. No item is required to progress.

The two original item illustrations use the built-in imagegen tool. The exact prompt is [saved alongside the atlas](ArtReferences/remembrance-item-atlas-runtime-v2.32.0-prompt.txt); the [runtime PNG](ArtReferences/remembrance-item-atlas-runtime-v2.32.0.png) is the unmodified transparent output. It uses two square cells in a 1774×887 sheet, exact-pinned in RuntimeArtManifest. The previous seven-item atlas remains unchanged. Both the native inventory UI and fallback gear renderer use the same stable signature identities; missing art falls back to existing generic item icons.

## Dialogue and balance

Orren, Kate, Lute, Borin, Tessa and Maud have local authored conversations that acknowledge campaign progress or earlier purchases. Four new personal-history topics and two distinct food histories give the contacts lives outside their services. Advice follows the real quest giver and current chapter state.

Ritual reinforcements remain dangerous but do not increase victory XP or become random equipment sources. Original enemies, including authored ritual bosses, retain their existing rewards. This removes the incentive to let a caster summon indefinitely.

## Persistence and checks

Quest progress uses new string flags under save schema v27. Acceptance, recovery, decoding, disposition, delivery and reward claims each validate their prerequisites and checkpoint changes. Repeated callbacks cannot duplicate equipment or XP. The two endings are exclusive; equipment choice is independent of the ending.

Focused checks cover both endings and both items, acceptance before and after clearing, invalid/repeated callbacks, deferring a reward, save/load, the separate Journal row, production combat intrinsics, enchantment identity, exact atlas selection, conversation state and the ritual reward exploit. Release evidence records which gates actually ran and the scope of visual review. A complete human Chapter I–V playthrough remains a separate playtest.
