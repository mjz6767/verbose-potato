# Chapter VI: The Last Dawn

The normal campaign now has a final assault after the Chapter V survey and Yara debrief. It uses the existing Meteor Crown region, enemies, and art, while retaining the old full-prototype boss as a separate route.

## Player journey

1. Speak with Yara after reporting the surveyed threshold. Ask about the last road, review the plan, and explicitly accept. Choosing to prepare leaves the descent closed.
2. Return through the five established chapter routes. Interact with Salt Cistern Gate at depth V to enter the final region. The first entry supplies two camp provisions; revisiting never repeats them.
3. Defeat the four-member crownwatch at the depth VI Red Gate Seal. This secures the return road, identifies the heart's conduit, and grants an elixir.
4. Defeat the ritual heart and its three keepers beneath the Glass Lore Library. Its cache grants two supplies and an elixir. The heart encounter never triggers the ending.
5. Reach Salt Cistern Gate, answer Vhal Rakh, and either step back to prepare or begin the final battle. Defeating Vhal Rakh and his three defenders reaches the victory screen and saves a durable campaign completion flag.
6. Choose **Return Home** on the victory screen to return the same company to Temple Square in exploration mode. The completed story remains saved; Yara's homecoming conversation and the remaining optional town errands are reachable in normal play.

Recall, camping, and saved clears remain available between encounters. The route guidance and journal follow the next uncompleted objective after recall or save/load. The finale's NPC writing responds to acceptance, breaking the heart, and completion; the epilogue also acknowledges Edda's optional remembrance when delivered.

## Encounter design

Each fight contains four enemies, compared with Chapter V's five-to-six. The preliminary battles separate the ritual heart from Vhal Rakh so earlier success has a concrete consequence in the final roster. No glyph or demon-rift obstacles create renewable reinforcements. Fire, web, and gas have limited duration; stone offers approach cover without blocking both middle lanes.

The existing depth VI stat scaling is retained. The seven new campaign graduation rewards total **2,650 XP**, including 600 XP on accepting the last assault and 250 XP for breaking the heart. Independent reward receipts let older saves collect earned rewards once and prevent repeated acceptance, visits, or load from granting them again.

The [campaign pacing audit](CAMPAIGN_PACING_2026-09-27.md) measures ten seeds through the production encounter and progression methods: the direct route enters Chapter VI around levels 13–14 and ends at level 15 for human starter heroes and level 14 for the mage. Optional exploration can produce levels 18–19 and is a stated balance risk. These ledgers assume victories and living heroes; they do not prove encounter win rates or tactical balance. Boss-specific attacks and the stationary heart are covered by the separate combat smoke.

## Persistence and integration

New flags are additive: `meteor_crown_assault_accepted`, `meteor_crown_entered`, `meteor_crown_sentries_defeated`, `meteor_crown_heart_broken`, and `meteor_crown_defeated`. Acceptance requires the Chapter V threshold survey and debrief. Victory callbacks enforce entry and encounter order, then reject repeated claims. No save schema change is required.

`MeteorCrownQuestSmoke` exercises the live dialogue choices, real regional-site dispatch, all return routes after recall, unblocked guidance paths to the next objective, actual combat-completion callbacks, no premature heart ending, one-time resources, production SaveService roundtrips (including an in-progress heart fight), final Victory-mode persistence, four completed journal milestones, and legacy prototype separation. It deliberately resolves test battles rather than claiming an autonomous tactical playthrough. The Return Home flow and homecoming dialogue are additionally checked by the dedicated native content validation.

The finale receipt flags are independent of map generation and first-visit service flags. Returning to a recreated depth VI map neither clears victories nor repeats arrival supplies. A new campaign intentionally begins with a new flag list.
