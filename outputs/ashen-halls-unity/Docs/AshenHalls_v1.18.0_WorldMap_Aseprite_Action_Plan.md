# Ashen Halls v1.17.0 World Map + Aseprite Action Plan

## Summary

The v1.17.0 source shows that several prior recommendations were implemented correctly: `SaveService` exists, domain/runtime models and enums were extracted, `WorldMapArtSpec` exists, trimmed exploration atlas drawing exists, `DrawExploreBiomeAmbientProp` exists, `ExploreBoardInnerRect` is tighter, gate orientation is centralized, and F8 art-debug overlay exists.

The remaining world-map quality gap is now mostly an **art contract problem**, with a few targeted renderer fixes. Aseprite is worthwhile here. This is exactly the kind of problem Aseprite is good at: transparent sprite cleanup, cell-grid consistency, hand-authored tree/gate/wall silhouettes, and edge/corner tile variants.

## Source findings that still matter

### 1. Wall atlas validation has a code-contract bug

`IsMidgaardWallAtlas()` currently checks that the whole atlas is nearly square:

```csharp
return midgaardWallAtlas != null && Mathf.Abs(midgaardWallAtlas.width - midgaardWallAtlas.height) < 8 && midgaardWallAtlas.width >= 768;
```

But `MidgaardWallAtlasCell()` treats the sheet as a 5x4 atlas:

```csharp
return AtlasCell(midgaardWallAtlas, index, 5, 4);
```

A 5x4 sheet with square cells should **not** be square overall. It should have width:height about 5:4. The active wall atlas is `1254x1254`, so its 5x4 cells are approximately `250.8x313.5`, which is tall/narrow. This can make wall tiles scale oddly and misalign with square map cells.

**Fix:** replace whole-atlas-square validation with cell-aspect validation:

```csharp
private bool AtlasHasSquareCells(Texture2D texture, int columns, int rows, float tolerance = 2f)
{
    if (texture == null || columns <= 0 || rows <= 0) return false;
    float cellW = texture.width / (float)columns;
    float cellH = texture.height / (float)rows;
    return Mathf.Abs(cellW - cellH) <= tolerance;
}

private bool IsMidgaardWallAtlas()
{
    return midgaardWallAtlas != null
        && midgaardWallAtlas.width >= 768
        && midgaardWallAtlas.height >= 600
        && AtlasHasSquareCells(midgaardWallAtlas, 5, 4, 3f);
}
```

### 2. Gate atlas art is still opaque, so trimming cannot work

The active gate atlas is `midgaard-gate-atlas-runtime-v0.93.png`, size `1402x1122`, with 5x4 near-square cells. The dimensions are fine. The problem is alpha: the atlas is 0% transparent / 100% visible. The gate art is inside dark square panels.

`TryDrawTrimmedExplorationAtlasCell()` trims based on alpha. If the whole cell is opaque, the trim source remains the whole cell.

**Fix:** make the gate atlas a transparent object sheet in Aseprite, then load it with alpha validation:

```csharp
midgaardGateAtlas = LoadLatestExternalPngWithAlpha(
    "midgaard-gate-atlas-runtime-",
    "",
    0.20f,
    "Midgaard gates",
    0.08f);
```

And tighten gate cell validation:

```csharp
return TryDrawTrimmedExplorationAtlasCell(
    midgaardGateAtlas,
    rect,
    index,
    5,
    4,
    tint,
    "Midgaard gate",
    0.08f,
    0.92f,
    spec);
```

### 3. East/west fallback still reuses north/south art

The new orientation path is good:

```csharp
North -> 0
South -> 1
East  -> 6
West  -> 7
```

But fallback still does:

```csharp
East -> 8
West -> 9
```

Those are reused town-gate fallback cells, not true east/west gates.

**Fix:** either disable east/west fallback or add procedural side-gate fallback.

```csharp
private int GateTownFallbackAtlasIndex(GateOrientation orientation)
{
    switch (orientation)
    {
        case GateOrientation.North: return 8;
        case GateOrientation.South: return 9;
        case GateOrientation.East: return -1;
        case GateOrientation.West: return -1;
        default: return -1;
    }
}
```

### 4. Biome ambient props exist but borrow landmark art

`DrawExploreBiomeAmbientProp()` is a good addition. But `BiomeAmbientPropAtlasIndex()` currently maps forest/mire/quarry/ash ambience to `worldMapPropAtlas`, which contains major landmarks and large props, not small biome detail.

Example current mapping:

```csharp
case "moss": return PickPropVariant(variant, 10, 11, 9, 1);
case "mire": return PickPropVariant(variant, 6, 8, 2, 10);
case "quarry": return PickPropVariant(variant, 9, 11, 13, 15);
```

Those cells are too semantically large: shrines, obelisks, caves, camps, stairs, ruins, etc.

**Fix:** add a dedicated transparent biome prop atlas.

### 5. Forest walls still repeat a single tile

`WorldMapExplorationTileIndex()` still does:

```csharp
if (kind == "forestwall") return 7;
```

This creates visible repetition. Forest/tree walls need edge-aware variants.

## Recommended Aseprite atlas contracts

### A. `world-map-biome-prop-atlas-runtime-v1.17.1.png`

- Grid: 5 columns x 4 rows.
- Recommended size: `1400x1120`.
- Cell size: `280x280`.
- Background: fully transparent.
- Runtime art should stay mostly inside a `220x220` safe region.
- Local shadows should be semi-transparent pixels, not dark opaque squares.

Suggested cells:

| Cell | Prop |
|---:|---|
| 0 | broadleaf tree |
| 1 | conifer |
| 2 | stump/log |
| 3 | shrub |
| 4 | dead tree |
| 5 | reeds |
| 6 | fungus cluster |
| 7 | mire grass |
| 8 | rubble pile |
| 9 | broken column |
| 10 | ash vent |
| 11 | scorched stump |
| 12 | crystal shard |
| 13 | cairn |
| 14 | bones/skull pile |
| 15 | signpost |
| 16 | herb cluster |
| 17 | lantern post |
| 18 | mossy root arch |
| 19 | small grass/ground tuft |

### B. `midgaard-gate-atlas-runtime-v1.17.1.png`

- Grid: 5x4.
- Recommended size: `1400x1120`.
- Cell size: `280x280`.
- Background: transparent.
- Cells 0, 1, 6, 7 are required by code.
- East/west gates should be side-on wall openings, not recycled north/south gates.

Recommended required cells:

| Cell | Use |
|---:|---|
| 0 | north gate |
| 1 | south gate |
| 6 | east gate |
| 7 | west gate |
| 8 | north fallback/variant |
| 9 | south fallback/variant |
| 10 | fortified north variant |
| 11 | fortified south variant |
| 12 | east ruined/closed variant |
| 13 | west ruined/closed variant |

### C. `midgaard-wall-atlas-runtime-v1.17.1.png`

For wall **tile** art:

- Grid: 5x4.
- Size: `1400x1120` or `1280x1024`.
- Cells should be square.
- It may be opaque because it is drawn as terrain/tile art.

For wall **object/accent** art:

- Use a separate transparent atlas if you want towers, banners, corner caps, or wall-top decorations.
- Do not draw opaque object panels on top of wall tiles.

### D. Forest/tree wall variants

Either expand `world-map-exploration-tile-atlas-runtime` or create a dedicated forest-wall tile atlas. Minimum cells:

| Variant | Use |
|---|---|
| solid canopy | fully surrounded forest wall |
| north edge | open/passable below or above, depending y convention |
| south edge | opposite edge |
| east edge | side edge |
| west edge | side edge |
| inner corners | four inner corner cases |
| outer corners | four outer corner cases |
| road gap | trail break through trees |
| dense canopy | random variation |
| dead-tree canopy | random variation |

## Aseprite workflow

### 1. Create/import the sheet

Use `1400x1120`, set grid to `280x280`, and enable snap-to-grid.

Suggested layer stack:

1. `guide_grid` — not exported.
2. `cell_labels` — not exported.
3. `shadows` — semi-transparent only.
4. `base_silhouette`.
5. `midtones`.
6. `highlights`.
7. `edge_polish`.

### 2. Clean existing gate alpha

For `midgaard-gate-atlas-runtime-v0.93.png`:

1. Open the PNG in Aseprite.
2. Set grid to `280x280` if using a 1400-ish sheet.
3. Use Magic Wand on the dark backing in each cell.
4. Delete the backing to alpha.
5. Repaint any lost internal gate shadows on the `shadows` layer using semi-transparent pixels.
6. Export as `midgaard-gate-atlas-runtime-v1.17.1.png`.

Do not leave a full-cell black/navy rectangle behind the gate.

### 3. Rebuild wall sheet dimensions

The current wall sheet is square overall, but the renderer treats it as 5x4. Recreate it as `1400x1120` or `1280x1024` and copy each wall cell into a true square cell.

### 4. Export commands

Example local commands if `aseprite` is on PATH:

```powershell
aseprite -b .\Docs\ArtReferences\world-map-biome-prop-atlas-v1.17.1.aseprite --save-as .\Docs\ArtReferences\world-map-biome-prop-atlas-runtime-v1.17.1.png
aseprite -b .\Docs\ArtReferences\midgaard-gate-atlas-v1.17.1.aseprite --save-as .\Docs\ArtReferences\midgaard-gate-atlas-runtime-v1.17.1.png
aseprite -b .\Docs\ArtReferences\midgaard-wall-atlas-v1.17.1.aseprite --save-as .\Docs\ArtReferences\midgaard-wall-atlas-runtime-v1.17.1.png
```

## Codex task list

### Task 1: Fix wall atlas validation

Add `AtlasHasSquareCells(texture, columns, rows, tolerance)` and change `IsMidgaardWallAtlas()` to validate 5x4 square cells instead of square whole-atlas dimensions.

### Task 2: Load gates with alpha validation

Change gate loading to `LoadLatestExternalPngWithAlpha`, set a minimum transparent fraction, and change the max visible fraction in `TryDrawMidgaardGateAtlasIcon` from `1.00f` to about `0.92f`.

### Task 3: Remove wrong east/west fallback

Return `-1` for east/west in `GateTownFallbackAtlasIndex`, or add a procedural side-gate fallback.

### Task 4: Add a biome prop atlas

Add:

```csharp
private Texture2D worldMapBiomePropAtlas;
```

Load:

```csharp
worldMapBiomePropAtlas = LoadLatestExternalPngWithAlpha(
    "world-map-biome-prop-atlas-runtime-",
    "",
    0.20f,
    "world map biome props",
    0.08f);
```

Add `IsWorldMapBiomePropAtlas`, `WorldMapBiomePropAtlasCell`, and `TryDrawWorldMapBiomePropAtlasIcon` using `TryDrawTrimmedExplorationAtlasCell`.

Then change `DrawExploreBiomeAmbientProp()` to use the biome atlas instead of `worldMapPropAtlas`.

### Task 5: Add forest-wall edge selection

Replace:

```csharp
if (kind == "forestwall") return 7;
```

with a small bitmask/autotile resolver based on neighboring passable cells.

### Task 6: Improve F8 art debug

Make the F8 overlay show:

- object type,
- atlas key,
- atlas index,
- source rect,
- trimmed rect,
- spec scale/pivot/offset.

### Task 7: Make viewport width aspect-aware

Close map should become 13x7 or 15x7 on wide monitors instead of always 11x7. This will let the map fill more of the screen without changing tile size too aggressively.

## Priority order

1. Fix `IsMidgaardWallAtlas` contract bug.
2. Create transparent gate atlas in Aseprite.
3. Disable wrong east/west fallback.
4. Create the dedicated biome prop atlas.
5. Add tree/forest wall variants.
6. Improve art validation so this cannot regress.
7. Then extract world-map rendering into its own renderer/catalog files.

## Bottom line

Use Aseprite. The code scaffolding is now good enough that hand-fixing the atlas contracts will pay off immediately. The fastest visible improvement is transparent, orientation-specific gate art plus true tree/biome props. The highest-risk current bug is the wall atlas contract mismatch: the code is asking for 5x4 cells but validating a square whole sheet.
