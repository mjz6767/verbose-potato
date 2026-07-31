# Character and NPC Sprite Pass v1.93.0

This pass removes the remaining player-character race fallback art and fills
the four reserved Midgaard contact cells with named NPC identities.

## Runtime art

- `Docs/ArtReferences/character-combat-atlas-runtime-v1.93.0.png`
  - Exact transparent 5 by 7 atlas, 1280 by 1792, with 256-pixel cells.
  - Columns are human, dusk elf, stoneborn, fenkin, and ashling.
  - Rows are warrior, rogue, ranger, priest, warlock, wizard/mage, and
    paladin.
  - Adds 19 original race/class sprites.
  - Copies all 16 approved v1.77 sprites pixel-for-pixel into their new
    semantic positions.
- `Docs/ArtReferences/midgaard-npc-atlas-runtime-v1.93.0.png`
  - Exact transparent 5 by 4 atlas, 1280 by 1024, with 256-pixel cells.
  - Replaces only the former reserved cells: Kate at 10, Lute at 11, the
    dock worker at 14, and the scholar at 19.
  - The other 16 v1.81 cells are pixel-identical.

## Source and validation

- Built-in image generation created flat-magenta source sheets.
- The installed local chroma-key helper removed backgrounds with soft matte
  and despill.
- `Tools/BuildExpandedCharacterNpcAtlases.py` finds transparent source-grid
  lanes, normalizes sprites to the shared baseline and gutter contract,
  preserves approved cells, assembles both runtime atlases, and writes
  `Docs/ArtReferences/sprite-art-runtime-v1.93.0-validation.json`.
- Exact generation prompts and both chroma-key and alpha sources are retained
  beside the runtime atlases.

The runtime manifest pins both v1.93 atlases. Player selection now maps every
supported race/class combination deterministically. Kate, Lute, the dock
worker, and the scholar also have explicit placed-world object types,
role-appropriate reachable positions, world-sprite mappings, Talk context,
dialogue focus, and ambient audio routing. The NPC loader accepts only the
exact 1280 by 1024 sheet, and validation checks all 20 square cells before
runtime use.

Because the four added cells were reserved in older same-size sheets, the
Midgaard NPC loader deliberately has no family fallback. If the exact semantic
sheet is missing or invalid, actor rendering returns to role-aware procedural
figures rather than clamping the appended enum values to unrelated generic art.
Dedicated `-ashen-contact-smoke` and `-ashen-contact-dialogue-smoke` launches
stage Kate, Lute, the dock worker, or the scholar beside the party and validate
the exact Talk target, sprite cell, dialogue focus, speaker, portrait, and
choice count.

The added object values are append-only and the existing Midgaard repair pass
places them deterministically, so this wiring does not change the save schema
or combat rules.
