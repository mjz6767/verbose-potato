# Campaign pacing review — 2026-09-27

The required Chapter I–V route previously brought the starter party to level 8 across all ten audited seeds. This left level-12/16 martial abilities and most later spells unreachable without extensive optional encounters or repeated fights. The direct nonhuman route supplied 1,612–1,685 XP; level 12 costs 3,300 and level 16 costs 5,700.

The implemented correction grants 2,650 XP in seven individually receipted chapter milestones, using the existing XP curve and survivor rules. Across the same ten seeds, the required campaign now reaches level 12 after the Chapter V debrief, enters the final chapter around levels 13–14, and finishes at human level 15 / mage level 14. Seed 727 reaches 4,633 base XP after the eighteen authored battles. The 250 XP Heart milestone unlocks level-14 tools before the sovereign fight. These rewards also catch up old saves once; they do not make recurring patrols more profitable.

The generous exploration comparison finishes at levels 18–19. This is intentionally visible as a balance risk: returning parties who clear every regional patrol and rediscover every eligible region can overpower a finale built for the direct route. The comparison overcredits regions previously revealed through route hints, so it is neither an expected player path nor a strict upper bound on optional content.

Recovery evidence matters separately from XP. The direct route provides 23 supplies through Chapter V before spending, but a nearly exhausted level-12 caster needs eight or nine camps to refill both health and mana. Free Recall fully heals and revives the party; the return walk is the main recovery cost. The final chapter provides two supplies on entry, an elixir after sentries, and two supplies plus an elixir after the Heart. Full-rest and level-up recovery can make long-term attrition much lighter than individual battle difficulty suggests.

Validation: `AshenHalls.Editor.CampaignPacingSmoke.Run` completed successfully in Unity; source, ten-seed results, and explicit assumptions follow. This ledger calls production factories and reward/recovery handlers. It assumes victories and living party members, so it does not establish win rates, damage pressure, travel friction, or balanced spent-growth builds. The separate boss combat smoke checks attack caps, range, cover, resistance, formation counterplay, Heart immobility, and bounded drain. Human campaign playtesting remains necessary.

---
# Campaign pacing ledger

Generated from current production enemy creation, encounter placement, rank selection, reward, level-up, hero recalculation and camp methods. Four starter heroes, all survive, no repeated patrol farming, no purchased equipment or spent growth points. Victories are assumed; this does not measure win rate, tactics, route travel time or damage taken.

Direct route includes the 15 authored Chapter I–V battles plus chapter-entry XP (30/36/42/48), the Chapter III and V surveys (48/64), Yara's mandatory Glass/Red briefings (12/16), and first inspections of the two/ two/ three required regional sites in III/IV/V. It excludes zone discovery, optional errands, caches, shrines and optional patrols. These explicit world-award assumptions are reviewed against the runtime handlers; they are not dynamically extracted.

Sweep adds each current production patrol once and 11 regional zone discoveries per chapter. This is a generous comparison scenario: some regions may be revealed by hints before discovery and therefore award no XP. It is not an expected player itinerary or an upper bound on every possible optional reward.

## Seed 727 direct route before chapter graduation rewards

| Chapter/battle | Enemy HP / power | XP per survivor | Human level | Mage level | Accumulated base XP |
|---|---:|---:|---:|---:|---:|
| 1: Broken Sluice | 41 / 16 | 15 | 1 | 1 | 15 |
| 1: Foul Runoff | 74 / 29 | 32 | 1 | 1 | 47 |
| 1: Cistern Den | 106 / 39 | 45 | 2 | 1 | 92 |
| 2: Kobold Ambush | 122 / 38 | 61 | 2 | 2 | 183 |
| 2: Smoke Cave | 134 / 47 | 65 | 3 | 3 | 248 |
| 2: Kobold King | 183 / 60 | 83 | 3 | 3 | 331 |
| 3: Bone Road Watch | 141 / 49 | 61 | 4 | 4 | 444 |
| 3: Gloam Crypt Ritual | 179 / 62 | 85 | 4 | 4 | 529 |
| 3: Warden of Gloam | 223 / 75 | 110 | 5 | 4 | 639 |
| 4: Glassward Ambush | 140 / 53 | 69 | 6 | 5 | 844 |
| 4: Keepers of the Glass Index | 170 / 64 | 85 | 6 | 6 | 929 |
| 4: Warden of the Ashen Pact | 239 / 81 | 117 | 6 | 6 | 1064 |
| 5: Red Gate Vanguard | 208 / 78 | 99 | 7 | 7 | 1247 |
| 5: Ossuary Road Seal | 270 / 92 | 128 | 7 | 7 | 1395 |
| 5: Marshal of the Crownroad | 276 / 94 | 133 | 8 | 8 | 1548 |

## Seed matrix at the surveyed Meteor Crown threshold

| Seed | Direct base XP | Direct human / mage | Sweep base XP | Sweep human / mage |
|---:|---:|---|---:|---|
| 0 | 1670 | 8 / 8 | 4033 | 13 / 13 |
| 1 | 1681 | 8 / 8 | 4044 | 13 / 13 |
| 7 | 1639 | 8 / 8 | 3985 | 13 / 13 |
| 19 | 1628 | 8 / 8 | 3960 | 13 / 13 |
| 42 | 1630 | 8 / 8 | 3912 | 13 / 13 |
| 97 | 1685 | 8 / 8 | 4058 | 14 / 13 |
| 227 | 1612 | 8 / 8 | 3902 | 13 / 13 |
| 727 | 1612 | 8 / 8 | 3902 | 13 / 13 |
| 1337 | 1613 | 8 / 8 | 3906 | 13 / 13 |
| 2320 | 1617 | 8 / 8 | 3949 | 13 / 13 |

## Current milestone tuning and authored finale, seed 727 direct route

Milestone rewards below come from CampaignMilestoneRules and the real ApplyCampaignMilestoneRewards handler. Existing XP costs/unlocks and combat rewards remain unchanged. World rewards are inserted before the relevant site battle and chapter completions after the chapter's last victory. The three Chapter VI first inspections each award 22 XP; the dedicated Chapter VI entry currently grants supplies without separate arrival XP.

- Cistern contract complete: 100 XP once.
- Kobold King defeated: 200 XP once.
- Bone Road secured: 350 XP once.
- Glass Road secured: 500 XP once.
- Red Gate report delivered: 650 XP once.
- Last Dawn preparations: 600 XP once.
- Crown tether severed: 250 XP once.

| Chapter/battle | Enemy HP / power | XP per survivor | Human level | Mage level | Accumulated base XP |
|---|---:|---:|---:|---:|---:|
| 1: Broken Sluice | 41 / 16 | 15 | 1 | 1 | 15 |
| 1: Foul Runoff | 74 / 29 | 32 | 1 | 1 | 47 |
| 1: Cistern Den | 106 / 39 | 45 | 2 | 1 | 92 |
| 1: chapter graduation | — | 100 | 2 | 2 | 192 |
| 2: Kobold Ambush | 122 / 38 | 61 | 3 | 3 | 283 |
| 2: Smoke Cave | 134 / 47 | 65 | 3 | 3 | 348 |
| 2: Kobold King | 183 / 60 | 83 | 4 | 4 | 431 |
| 2: chapter graduation | — | 200 | 5 | 4 | 631 |
| 3: Bone Road Watch | 141 / 49 | 61 | 5 | 5 | 744 |
| 3: Gloam Crypt Ritual | 179 / 62 | 85 | 6 | 5 | 829 |
| 3: Warden of Gloam | 223 / 75 | 110 | 6 | 6 | 939 |
| 3: chapter graduation | — | 350 | 7 | 7 | 1353 |
| 4: Glassward Ambush | 140 / 53 | 69 | 8 | 7 | 1494 |
| 4: Keepers of the Glass Index | 170 / 64 | 85 | 8 | 8 | 1579 |
| 4: Warden of the Ashen Pact | 239 / 81 | 117 | 8 | 8 | 1714 |
| 4: chapter graduation | — | 500 | 10 | 9 | 2214 |
| 5: Red Gate Vanguard | 208 / 78 | 99 | 10 | 10 | 2397 |
| 5: Ossuary Road Seal | 270 / 92 | 128 | 10 | 10 | 2545 |
| 5: Marshal of the Crownroad | 276 / 94 | 133 | 11 | 10 | 2698 |
| 5: chapter graduation | — | 650 | 12 | 12 | 3412 |
| 6: Crownwatch at the Last Gate | 210 / 71 | 105 | 14 | 13 | 4139 |
| 6: The Heart Beneath the Glass | 207 / 67 | 93 | 14 | 14 | 4504 |
| 6: Vhal Rakh, the Unfed Crown | 240 / 74 | 107 | 15 | 14 | 4633 |

| Seed | Current direct threshold human / mage | Current direct ending human / mage | Current sweep ending human / mage |
|---:|---|---|---|
| 0 | 12 / 12 | 15 / 14 | 19 / 18 |
| 1 | 12 / 12 | 15 / 14 | 19 / 18 |
| 7 | 12 / 12 | 15 / 14 | 19 / 18 |
| 19 | 12 / 12 | 15 / 14 | 19 / 18 |
| 42 | 12 / 12 | 15 / 14 | 19 / 18 |
| 97 | 12 / 12 | 15 / 14 | 19 / 18 |
| 227 | 12 / 12 | 15 / 14 | 19 / 18 |
| 727 | 12 / 12 | 15 / 14 | 19 / 18 |
| 1337 | 12 / 12 | 15 / 14 | 18 / 18 |
| 2320 | 12 / 12 | 15 / 14 | 19 / 18 |

## Previous unlock gap (seed 727 direct route before milestone tuning)

- warrior: Vanguard Rush L1, Hold the Line L1, Iron Rebuff L3, Executioner's Cut L5, Reaping Cleave L8, Steel Tempest L12 (unreached), Shatterguard L16 (unreached)
- ranger: Hawkeye Shot L1, Pinning Shot L1, Hunter's Mark L3, Arrowstorm L5, Broadhead Shot L8, Staggering Shot L12 (unreached), Twinshot L16 (unreached)
- rogue: Slip the Veil L1, Ambush L1, Serrated Throw L3, Smoke Bomb L5, Hamstring L8, Eviscerate L12 (unreached), Shadowstep L16 (unreached)
- Active formulas above the mage's level: Rift Seal L10, Sanctuary Ward L9, Covenant Ward L13, Wellspring Hymn L14, Sun Brand L16, Chain Lightning L12, Thunder Step L16, Arcane Tempest L20, Doom Circle L10, Abyssal Ascendance L18, Rift Step L10, Dawn Pulse L12, Cinderstorm L15, Ashen Curse L18.
- Level 8 requires 1540 cumulative XP; the direct-route non-human deficit is 0 XP.
- Level 12 requires 3300 cumulative XP; the direct-route non-human deficit is 1688 XP.
- Level 16 requires 5700 cumulative XP; the direct-route non-human deficit is 4088 XP.
- Level 20 requires 8740 cumulative XP; the direct-route non-human deficit is 7128 XP.
- With current milestone tuning the direct ending reaches human L15, mage L14. Optional exploration remains the route to level-16 capstones; level20 is not assumed for this campaign.

## Economy and recovery

Runtime FinishCombat gives 18–38 + 6 × depth gold and a 60% elixir chance. Across the 15 required fights: 540–840 gold (mean 690). Starting 28 + Borin 10 + Varkh 49 bring gross route gold to 627–927 (mean 777), before purchases, caches, sales or optional fights. Starting three + Varkh one + nine expected victory drops produce 13 expected elixirs; random drops are not guaranteed.
Guaranteed supplies: five starting + one Foul Runoff alcove + eight first chapter arrivals + two Varkh + one Ossuary Warden + one Mirror Index + two Ashen Pact + one road seal + two Marshal = 23 before use. Free Recall returns to Temple Square and RestorePartyFully also revives fallen heroes. Travel back to the current objective is the recovery cost. Camp heals living heroes only, spending one supply; it cannot recover a fallen hero. Dead heroes receive no XP at victory.

| Starter at threshold, gear/growth unspent | Level | Max HP / MP | One camp HP / MP restored | Camps from 1 HP / 0 MP to full |
|---|---:|---:|---:|---:|
| Maer | 12 | 85 / 0 | 13 / 0 | 7 |
| Cairn | 12 | 78 / 0 | 13 / 0 | 6 |
| Luma | 12 | 71 / 65 | 13 / 8 | 9 |
| Vesh | 12 | 77 / 62 | 13 / 8 | 8 |

## Authored final battle at depth 6, seed 727

| Enemy | HP | Power | Defense | Range | Magic resist | Weakness |
|---|---:|---:|---:|---:|---:|---|
| Vhal Rakh, Meteor Crown | 78 | 22 | 7 | 5 | 7 | cold / light |
| Old Lesser Demon | 61 | 19 | 7 | 1 | 4 | cold / light |
| Old Drow Priest | 51 | 16 | 5 | 4 | 6 | light / shock |
| Old Cinderling | 50 | 17 | 5 | 3 | 1 | cold |
Total 240 enemy HP; 107 base XP per survivor. This ledger does not establish a tactical win rate.

Interpretation: current chapter rewards put level-12 tools in reach before the finale and level-14 tools before Vhal Rakh. Level-16/20 tools cannot be required on the direct route. Test tactical difficulty with these actual levels, earned quest gear and deliberately spent growth points. The generous exploration scenario reaches level18–19 and may substantially soften the finale; this is a separate playtest case, not proof of balanced combat. Aggregate enemy power is not damage per round.
