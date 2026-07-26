# Sprite Art Pass v1.81.0

This pass replaces four stylistically outdated Midgaard world NPC sprites
and the full dedicated Kobold King atlas.

## Installed art

- `Docs/ArtReferences/midgaard-npc-atlas-runtime-v1.81.0.png`
  - Replaces cells 7 Tessa, 9 Maud, 13 Edda, and 18 Yara.
  - The other 16 cells are pixel-identical to v1.64.1.
  - The runtime manifest and release-contract checks pin v1.81.0.
- `Docs/ArtReferences/kobold-boss-atlas-runtime-v1.81.0.png`
  - Preserves all 16 existing Varkh state, portrait, icon, and route-fallback
    meanings.
  - The existing latest-version loader selects it ahead of v0.71.

Both assets retain 256 px cells, transparent gutters, and their established
runtime anchors. No gameplay or save-schema changes were required.

The exact generation prompts and validation JSON are stored beside each
atlas. Full comparisons, smoke-test logs, and runtime-scale contact sheets
are in the external `sprite-art-v1.81-stage` handoff package.
