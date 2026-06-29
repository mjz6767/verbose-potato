# Ashen Halls Art Intake

This project uses generated and hand-cleaned original art atlases from `Docs/ArtReferences/`.
Runtime art loading prefers the newest PNG whose filename starts with a known prefix, then falls back to the older pinned file.

## Runtime Naming

Use these prefixes when creating replacement or expansion atlases:

- `splash-title-reference-v0.xx.png`
- `tavern-backdrop-runtime-v0.xx.png`
- `tavern-ui-atlas-runtime-v0.xx.png`
- `creature-sprite-atlas-runtime-v0.xx.png`
- `combat-sprite-sheet-alpha-v0.xx.png`
- `enemy-roster-atlas-runtime-v0.xx.png`
- `enemy-world-object-atlas-runtime-v0.xx.png`
- `boss-enemy-atlas-runtime-v0.xx.png`
- `combat-terrain-atlas-runtime-v0.xx.png`
- `world-environment-atlas-runtime-v0.xx.png`
- `world-object-atlas-runtime-v0.xx.png`
- `quest-world-object-atlas-runtime-v0.xx.png`
- `item-equipment-atlas-runtime-v0.xx.png`
- `item-icon-atlas-runtime-v0.xx.png`
- `inventory-consumable-atlas-runtime-v0.xx.png`
- `class-icon-atlas-runtime-v0.xx.png`
- `combat-ui-atlas-runtime-v0.xx.png`
- `combat-hud-ui-atlas-runtime-v0.xx.png`
- `combat-command-icon-atlas-runtime-v0.xx.png`
- `spellbook-combat-ui-atlas-runtime-v0.xx.png`
- `combat-spellbook-ui-atlas-runtime-v0.xx.png`
- `magic-ui-atlas-runtime-v0.xx.png`
- `spell-card-icons-reference-v0.xx.png`
- `ember-spell-effects-atlas-runtime-v0.xx.png`
- `epic-spell-effects-atlas-runtime-v0.xx.png`
- `combat-spell-float-atlas-runtime-v0.xx.png`

Use a monotonically increasing version suffix. Example: `creature-sprite-atlas-runtime-v0.51.png`.

## Atlas Layouts

- Creature and combat terrain atlases are currently read as 4 by 4 grids.
- Most UI, item, world object, and icon atlases are currently read as 5 by 4 grids.
- `combat-sprite-sheet-alpha-*` uses the existing combat sprite sheet crop logic, so preserve a compatible 4 by 4-ish composition unless code is updated.
- Keep cells clearly separated. Avoid labels, watermarks, and decorative borders inside cells.
- Keep sprites centered with consistent foot/ground anchors. This matters more than raw detail.

## Art Quality Targets

- Readable at tactical board scale.
- Strong silhouette first, detail second.
- Modern pixel feel without losing old-school CRPG clarity.
- Transparent or flat-background sources are preferred for sprites and objects.
- Terrain should stay usable behind units, highlights, and targeting reticles.

## Handoff Notes

When an art thread creates a new atlas, include:

- Filename and saved path.
- Grid layout.
- Cell inventory.
- Any expected code mapping changes.
- Whether the atlas is a drop-in replacement using an existing prefix or a new art category needing code.

Do not delete older atlases. They remain useful fallback references.
