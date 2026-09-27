# The Honest Weight

Maud offers this optional investigation after Varkh's defeat. It remains available after the campaign ending and does not replace the current campaign objective. The small magical contract connects her established interest in useful everyday enchantment with a choice about the road's working economy.

When the favor is available, Maud's four visible root choices are temporary temper, permanent binding, the weigh-lamp favor, and "About your craft...". The craft submenu preserves her rune explanation and apprenticeship story, then returns to her services. Before the favor opens, her original four choices remain directly available. The quest entry therefore remains visible before acceptance, during the investigation, and after completion.

## Route and choice

Visit **Chapter III's Dusk Market** and **Chapter III's Old Quarry Forge**, in either order. The market lamp's inscription has been altered from "leave the bearer lighter" to "leave every buyer lighter"; it has been quietly shortening traders' sacks. The original quarry calibration reveals both the public weight standard and the charm's thirty-gold working deposit.

Each site offers a free, explicit quest inspection alongside its normal service. Earlier ordinary visits do not recover these specific inscriptions. Declining an inspection leaves it available. Using a service does not inspect the evidence or bypass its ordinary cost, and each service's first reward remains independently tracked.

Return to Maud with both inscriptions and review either settlement before confirming:

- **Break the charm:** receive its 30-gold deposit once. Dusk Market repeat supply trades remain 12 gold.
- **Restore a public measure:** keep the lamp working under corrected terms. Dusk Market repeat supply trades permanently cost 8 gold across all chapter depths. No deposit is paid out.

Both settlements award **120 base XP once** and the same independent choice of **Counterweight Knife** or **Measurekeeper Jack**. The choice of equipment does not depend on the moral or economic resolution. The item can be deferred and collected from Maud later; it enters the pack without automatically replacing equipped gear.

The two outcomes create an immediate versus continuing benefit: the retuned measure saves more than thirty gold after eight repeat supply purchases. The effect changes only the Dusk Market repeat trade; it neither alters first-visit caches nor creates a resource resale loop.

## Persistence and validation

The six additive flags record acceptance, both separate inspections, the mutually exclusive settlement, and the single item claim. The lamp itself is quest state rather than a tradable inventory item. A settlement cannot be reversed or repeated by a delayed callback; an item reward can be claimed only after settlement. Site callbacks also require the live quest dialog and a legal nearby site in Chapter III.

`HonestWeightQuestSmoke` covers both settlements and both rewards, opposite inspection orders, sites visited before accepting, legal site approaches, explicit inspection, normal service access, declining a settlement review, deferred item choice, production SaveService roundtrips, duplicate and conflicting callbacks, actual repeat trades at three chapter depths, and Maud's final reactions and journal entry. It verifies that optional quest operations preserve the existing main objective. The fixture includes late acceptance with the campaign completion flag present.

Native UI/art capture and full project validation are separate release checks. The quest has no new mandatory battle and imposes no level, class, consumable, or repeated-farming requirement beyond opening the Bone Road.

The smoke drives every quest choice through the native screen's real button callbacks and asserts that its visible button count matches the adapted dialogue choices. It also checks Maud's existing services and conversation return paths, plus Edda, Sera, and Brann's Remembrance entries, against the four-button capacity.
