# Ashen Halls v0.65 World System Scaffolding

This pass adds code-first placeholders for larger world systems without requiring final art yet. The goal is to make later route, quest, shop, training, faction, and dungeon work attach to named hooks instead of one-off special cases.

## New World Nodes

- `QuestBoard`: Midgaard contract board for future errands, bounties, tester toggles, and quest selection.
- `Waystone`: road anchor for future recall, fast travel, camp routing, and world-state markers.
- `TrainingGround`: class/skill training hook for future ability lessons and practice encounters.
- `LoreLibrary`: formula-study hook for spell unlocks, library events, and caster factions.
- `ForgeSite`: crafting/repair hook for armor, shields, reach weapons, and route workbenches.
- `FactionCamp`: NPC/faction hook for scouts, fences, reputation, supplies, and route rumors.
- `DungeonGate`: authored dungeon entrance hook for rooms, locks, keys, and reward tables.
- `DeepCrypt`: undead/caster route hook for ritual rooms and death-resistance rewards.
- `AncientGrove`: hazard/puzzle hook for poison, mire, generated trees, and monster path pressure.
- `PortalSeal`: late-campaign gate-key hook for the Red Gate and final-route locks.

## Current Beta Behavior

The nodes are intentionally simple:

- Generic scaffold nodes appear by zone and minimum depth only in the full prototype content set; they remain hidden from the normal production route.
- They show hover names, hints, progression pips, and placeholder glyphs.
- First visits set story flags using existing save fields.
- Some nodes grant small recovery, XP, supplies, skill points, or a test item.
- Dungeon/crypt nodes can start a placeholder combat once.
- The Journal lists every scaffold node and whether it is locked, located, or tested.

Save data remains version 22 because progress uses the existing `StoryFlags` list.

## v1.69 Normal-Route Promotion

The first bounded world-route expansion now lives outside the generic scaffold gate:

- Fresh maps are 58x46 and contain eight deterministic room-sized regional sites, one per outer territory.
- Regional site rooms and the named road circuit are certified reachable while respecting reserved King's Hall and merchant-hall footprints.
- Completing the sewer reward opens the named Sluice Steps descent into Dusk Market, the kobold ambush, Smoke Cave, and the Kobold King rooms.
- No other generic service, faction, encounter, route, or dungeon scaffold is promoted into normal play.
- Existing saves keep their serialized map dimensions and biome layout. Save schema remains v22.

## Art Hooks

Drop future PNG atlases into `Docs/ArtReferences/` with these prefixes:

- `route-scaffold-atlas-runtime-v0.xx.png`
- `dungeon-scaffold-atlas-runtime-v0.xx.png`
- `service-scaffold-atlas-runtime-v0.xx.png`
- `faction-banner-atlas-runtime-v0.xx.png`

See `Docs/ART_INTAKE.md` for exact cell contracts.

## Next Coding Targets

- Promote later dungeon room chains one bounded route at a time after their objectives, rewards, and return paths are authored.
- Add a quest-board modal for accepting and tracking route work.
- Add service panels for training, forge repair/crafting, lore study, and faction camp rumors.
- Add faction/reputation data once choices matter enough to serialize.
- Add route rewards that reference authored dungeon completion instead of first-touch flags.
