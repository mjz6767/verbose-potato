# Steel and Study balance changes

The v2.35 connected runs completed all six chapters with little late-game pressure. The v2.36 work addresses specific causes in weapon identity, training and accuracy. It does not treat automated victories as a human difficulty estimate.

## Weapon identity

A shared weapon profile replaces repeated class range floors. Ordinary hand weapons reach one cell, polearms reach two, bows/crossbows reach four, and thrown weapons/slings/darts reach three. Ranger training adds one only to real missile weapons. Spell-trained wielders can use a focus or staff for a four-cell Arms strike. Nonadjacent attacks require a clear line; reaching weapons and focuses do not become Missile attacks or collapse under engagement. Physical missile basics still become close Arms strikes when engaged.

The same profile drives equipment comparisons, combat eligibility and repaired derived range on Continue. Default caster implements receive their actual profile from the start; the old untouched starter-range discrepancy is not retained. Spell formulas keep their independent ranges and stationary-focus rules. Six damaging ranger techniques require missile equipment; Hunter's Mark and the rogue's separate Serrated Throw retain their existing equipment freedom.

## Training and accuracy

Practice grants at most one rank per exercised skill, member and encounter. A saved combat mask records consumed opportunities, so Continue cannot reset the budget. Existing skill values and unused Growth points are retained. A legacy midcombat save lacks this new mask and starts with one opportunity for each skill; subsequent saves preserve the actual mask.

Level rewards remain two talent points per level and two skill ranks per normal purchase. A skill at 98 can finish at 99 for one point; the preview and confirmation show its actual one-rank gain, and further spending is refused. The mage gains Arms as a second training choice. Guard training contributes one extra Guard at ranks 30, 60 and 90, only when taking the Guard action. It does not increase passive defense or spell wards. The normal stance difference after movement and equipment intrinsics remain separate.

Accuracy uses the original skill contribution through rank 25. Above 25, each three additional ranks add one accuracy point, up to 49 from rank 99. The same rule applies to ordinary attacks and techniques that make a skill hit roll, on both sides. Damage, healing, critical calculation and other skill effects keep their actual skill value. Existing chance floors, ceilings, status modifiers, equipment, Guard and defense remain in the forecast.

This is intended to keep defense and Guard relevant at high training. It is not a claim that every unguarded attack falls below the chance ceiling. For example, a mastered warrior may retain a 95% open-stance hit while the same guarded opponent lowers it. Early combat accuracy is unchanged when the relevant skill is at most 25.

## Measurement and release limits

The observer separates movement, action resolution and automatic turn processing, and verifies that reading snapshots does not change serialized state or RNG internals. Its detailed report is `Docs/CAMPAIGN_COMBAT_TELEMETRY_2026-09-27.md`. After correcting an outdated ritual-target assumption in the harness, all six candidate runs complete the campaign: 108 required and 34 roaming wins, with no hero deaths or elixir use. The five earlier policy assertion stops are preserved, not counted as combat defeats.

Primary skills finish at Arms 67–72, Missile 88–95, Ember 87–90 and Mend 81–84 rather than all reaching 99. Every earned Growth point is spent; the new mage fallback is covered by focused tests but is not needed in these runs. Cautious policies use eleven camps across the three seeds, compared with two in the baseline; the conservative policies still use none. All six runs retain ample supplies, gold and elixirs.

Across Chapter VI, observed party damage rises from 118 to 215 HP, with no hero reaching zero. Vhal casts Crownfire four times in the candidate, compared with none in the baseline. Final fights still last only two or three rounds. These are combined-rule observations under one strong policy, not isolated effects or proof the ending now has sufficient challenge.

The unchanged item-score policy equips a powerful sword on Cairn in four runs, removing his shooting techniques and causing more melee attacks and guarding. The other two runs keep a crossbow. Luma compensates with more Chain Lightning. This is legal equipment play, but it is not optimized ranger play; the comparison must retain that limitation. The equipment UI now leads comparisons with explicit role/reach consequences and requires deliberate review when delivery changes or reach shrinks. This affects quick-equip eligibility and disclosures; item scoring and explicit Pack assignment remain unchanged.

The baseline identifies additional questions that these changes do not silently solve: long movement consumes the enemy's attack opportunity; specials are attempted before movement; enemy explicit basic-damage ranges omit depth growth even though Power grows; and party defense already includes armor before attack formulas add the armor value again. Those require measured design decisions, not an unreviewed global enemy-stat increase.

Save schema remains 27. Current derived range is recomputed, while equipment identity, carried wounds/mana, spell access, inventory and quest flags remain persistent. Historical v2.35 release evidence and archives stay unchanged.
