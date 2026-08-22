# Midgaard world-art pass — v2.21

## Outcome

This pass makes Midgaard read as a built city instead of a field of vendor
tokens, gives named and ambient NPCs a coherent world-sprite language, and
replaces the repeating block/rail rhythm with continuous material roads.

## Runtime assets

- `midgaard-town-atlas-runtime-v2.21.0.png`
  - Exact 5 by 4, 1280 by 1024, 256-pixel-cell contract.
  - Replaces only architecture cells 0, 1, 3, 4, 5, 6, 7, 11, and 14.
  - Every replacement has a roof mass, wall plane, door/stoop, trade cue,
    contact shadow, and at least an 18-pixel transparent gutter.
  - Fountain, wall, sewer, recall, and other non-building cells are preserved
    from the approved v1.29 atlas.
- `midgaard-npc-atlas-runtime-v2.21.0.png`
  - Exact 5 by 4, 1280 by 1024, 256-pixel-cell contract.
  - Re-authors all twenty named world sprites as one consistent set.
  - Every sprite has a 220-pixel normalized height, shared baseline, at least
    an 18-pixel transparent gutter, restrained contact shadow, and a distinct
    role prop or costume silhouette.
- `world-npc-citizen-atlas-runtime-v2.21.0.png`
  - Exact 4 by 2, 1536 by 768, 384-pixel-cell contract.
  - Re-authors all eight ambient citizens to match the named-NPC outline,
    palette, rendering density, three-quarter view, and neutral grounding.
  - Every sprite has a 344-pixel normalized height, shared baseline, exact
    20-pixel top and bottom gutters, and 25% to 38% visible coverage.
- `midgaard-road-surface-atlas-runtime-v2.21.0.png`
  - Exact 2 by 2, 512 by 512, opaque 256-pixel-cell contract.
  - Supplies two civic cobble and two earthen road materials. Each cell has
    exact matching opposite edges so straight cells join without a tile seam.

The town, named-NPC, and ambient-citizen pins replace their predecessors. The
new road-material pin increases the approved manifest from 57 to 58 files and
the selected packaged PNG inventory from 91 to 92 files.

## Deterministic build

`Tools/BuildMidgaardWorldArtAtlases.py` performs the reproducible assembly.
It removes the generated sprite sources' baked neutral checker background by
clearing only bright neutral pixels connected to each source-cell edge, filters
small cross-cell fragments, normalizes every sprite, preserves non-building
cells, makes road swatches exactly seamless, and emits SHA-256-backed validation
reports and a combined contact sheet.

```powershell
python Tools/BuildMidgaardWorldArtAtlases.py `
  --town-base Docs/ArtReferences/midgaard-town-atlas-runtime-v1.29.0.png `
  --building-source Docs/ArtReferences/source-midgaard-architecture-v2.21.0.png `
  --npc-source Docs/ArtReferences/source-midgaard-npcs-v2.21.0.png `
  --stable-hand-source Docs/ArtReferences/source-midgaard-stable-hand-v2.21.0.png `
  --citizen-source Docs/ArtReferences/source-world-npc-citizens-v2.21.0.png `
  --road-source Docs/ArtReferences/source-midgaard-road-surfaces-v2.21.0.png `
  --town-output Docs/ArtReferences/midgaard-town-atlas-runtime-v2.21.0.png `
  --npc-output Docs/ArtReferences/midgaard-npc-atlas-runtime-v2.21.0.png `
  --citizen-output Docs/ArtReferences/world-npc-citizen-atlas-runtime-v2.21.0.png `
  --road-output Docs/ArtReferences/midgaard-road-surface-atlas-runtime-v2.21.0.png `
  --town-report Docs/ArtReferences/midgaard-town-atlas-runtime-v2.21.0-validation.json `
  --npc-report Docs/ArtReferences/midgaard-npc-atlas-runtime-v2.21.0-validation.json `
  --citizen-report Docs/ArtReferences/world-npc-citizen-atlas-runtime-v2.21.0-validation.json `
  --road-report Docs/ArtReferences/midgaard-road-surface-atlas-runtime-v2.21.0-validation.json `
  --contact-sheet Docs/ArtReferences/midgaard-world-art-v2.21.0-contact-sheet.png
```

Approved output hashes from the current source snapshot:

- Town atlas: `593C1DF8EA8142123EA3D2582A87D7900F89F0BFF1376778A7B58CEABAFD7501`
- Named-NPC atlas: `7E33AECC377E690A03A62AC070481A6853BDDFF61C2C56E92E4060E1763F6C27`
- Ambient-citizen atlas: `6F45940F1F590D2F4CE04450AD19A0C081611A06897B9B3952A51F2551AFB198`
- Road-surface atlas: `428A853DF2FF339740DC5E54D3DB8061C608D1C6E2DA5041B4435A726AEA7E72`

The exact ImageGen prompts are recorded beside the source images:

- `source-midgaard-architecture-v2.21.0-prompt.txt`
- `source-midgaard-npcs-v2.21.0-prompt.txt`
- `source-midgaard-stable-hand-v2.21.0-prompt.txt`
- `source-world-npc-citizens-v2.21.0-prompt.txt`
- `source-midgaard-road-surfaces-v2.21.0-prompt.txt`

## Runtime presentation

- Ordinary shopfronts grow to 1.48 local cells; Temple and Tavern use 1.56,
  and Town Hall uses 1.66. Region silhouettes step down without becoming tiny.
- Named NPCs occupy roughly 93% of a Local cell after both padding stages and
  may use their authored 1.08 scale instead of being silently clamped to 1.0.
- Town Guards use the same padding as other named contacts. Local ambient
  citizens and Grand Hearth patrons render within eight percent of named-NPC
  height; Region patrons remain within three percent. Region view hides
  full-body ambient passersby so roads, landmarks, and named contacts retain a
  strategic hierarchy. Safety, guidance, threshold, and interactable exclusions
  are unchanged.
- Named sprites use their single authored contact shadow instead of stacking a
  generic black plinth beneath it. Grand Hearth patrons likewise keep one
  authored shadow.
- Straight roads are rendered as uninterrupted edge-to-edge strips. Central
  aprons remain only on non-straight cells and subordinate connector joins.
- The Old Road is narrower and less opaque outside town. Inside Midgaard it
  keeps the same topology but uses a cool gray-brown civic cobble palette,
  restrained curbs, irregular seams, and worn ruts. Civic endpoint chips are
  suppressed, so detail cannot land on the absent half of an endpoint cell.
- The four seamless material swatches are clipped to the same topology-aware
  road shape. A contiguous tier keeps one swatch rather than changing texture
  randomly from cell to cell.

## Visual acceptance packet

Capture the same coordinates at 960 by 600, 1280 by 720, and 1920 by 1080:

- `midgaard-civic-local`
- `midgaard-civic-region`
- `midgaard-npc-street`
- `midgaard-road-join-matrix`
- east- and west-gate transitions

Reject any capture with repeated square road nodes, a rail/plank reading, brown
dirt painted over civic cobble, ambient full-body NPCs in Region view, clipped
building roofs, disconnected doorsteps, or named contacts that cannot be
distinguished at 960 by 600.

## Accepted release evidence

The final retail package contains all 58 exact manifest pins and 92 selected
PNGs, then clean-extracts and boots successfully from runtime source commit
`11222c94b27d5541b9160e9a80ec7d55de60e407`. Twenty packaged-player captures
under `QA/v2.21.0-release-visuals-11222c9-rerun2` cover all four Midgaard gate
approaches, Local/Region layouts, named street contacts, combat, Inventory, and
Loot across the supported endpoint/intermediate resolutions. The deterministic
packet passes with zero failures or warnings and capture-set SHA-256
`a66068e10c28a4157117d2de2678cf4965f9f606794e94aeac8bca942e3087c5`.

Independent visual review accepts the city structures, road materials/joins,
NPC-family scale, and responsive layouts without a release blocker. Compact
960 by 600 header elision and one shortened secondary Inventory summary are
nonblocking because the full selected-row values and all actions remain visible.
