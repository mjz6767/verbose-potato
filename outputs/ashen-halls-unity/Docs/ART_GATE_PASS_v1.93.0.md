# Midgaard Side-Gate Pass v1.93.0

This pass replaces the remaining east/west front-elevation lookalikes with
true side gates through Midgaard's north-south perimeter wall. Traversal and
save data do not change.

## Runtime art

- `Docs/ArtReferences/midgaard-gate-atlas-runtime-v1.93.0.png`
  - Exact transparent 5 by 4 atlas, 1280 by 1024, with 256-pixel cells.
  - Preserves all eighteen non-side-gate cells from v1.91 pixel-for-pixel.
  - Cell 6 is West: wilderness left, town right.
  - Cell 7 is East: town left, wilderness right.
  - Each side cell contains two low bastions joined to the vertical wall
    above and below a clear left-right road opening.
  - Side-cell local visible bounds are max-exclusive `[97,16,159,240]`, or
    62 by 224 pixels. Runtime trims that transparent cell padding and
    preserves the authored aspect ratio.

The runtime atlas SHA-256 is:

`734A84A928D43A23E29EC5F2D96160ADC5936F63DD848A18790376F1386004CB`

## Source and processing

- Built-in image generation, not CLI generation, created and refined the
  chroma-key source.
- `source-midgaard-side-gate-v1.93.0-chromakey.png` retains the selected
  built-in source.
- `source-midgaard-side-gate-v1.93.0-alpha.png` is the locally keyed source.
  The local helper detected generated key color `#f405e6`.
- ImageMagick performed deterministic trim, reduction, centering, mirroring,
  and crop-based atlas reassembly.
- `midgaard-gate-atlas-runtime-v1.93.0-prompt.txt` records the exact initial
  prompt and both precise edit prompts.
- `midgaard-gate-atlas-runtime-v1.93.0-validation.json` records the exact
  sources, hashes, changed/preserved cells, bounds, coverage, passage, and
  directional checks.

## Orientation and passage contract

- Runtime maps `West` to cell 6 and `East` to cell 7. North and South remain
  mapped to the preserved sealed cell 0.
- Cell 7 is an exact horizontal mirror of cell 6 for alpha and every visible
  RGB pixel.
- The authored visible-pixel centroid leans townward: right of center in the
  West cell and left of center in the East cell. This per-cell identity makes
  an accidental east/west swap fail deterministic tests.
- Local rows 104 through 151 in both side cells are alpha-zero across the
  complete 256-pixel width. The road cannot be covered by a gatehouse lintel,
  wall wing, or opaque atlas panel.
- Each side cell exceeds the per-cell visible-coverage floor and retains at
  least 16 pixels of transparent gutter.

The continuous wall foundation and material threshold remain renderer-owned.
Horizontal wall runs retain a 0.56-cell Local / 0.52-cell Region foundation;
vertical runs use a separate 0.36 / 0.34 foundation that hugs their narrower
authored masonry. Open east/west gate cells suppress the otherwise continuous
straight-wall tile, sample neighboring dirt and civic materials into the two
threshold halves, and draw only the wall joins above and below the road. No
renderer sill, lintel, or foundation crosses the travel lane. East and west
remain passable; north and south remain blocked.

## In-player QA

- `rule-smoke-v1.93.0-gate-refine.log` passes the atlas, direction, passage,
  footprint, wall-thickness, traversal, and preservation contracts.
- The current Windows build passes its embedded rule and runtime boot suites.
- `gate-west-3200x1800.png` and `gate-east-3200x1800.png` both have accepted
  packaged-player logs with exact requested, rendered, and decoded dimensions,
  `complete=True`, and `failure=None`.
- The ten-capture packet under `QA/v1.93.0/visual-qa-packet` passes with Local
  and Region views, all four 1280x720 gate approaches, and both 3200x1800 side
  gates. Physical traversal and a clean extracted-folder walkthrough remain
  human checks.
