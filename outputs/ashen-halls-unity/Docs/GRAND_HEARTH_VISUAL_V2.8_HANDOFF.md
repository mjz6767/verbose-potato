# Grand Hearth Visual v2.8 Handoff

The v2.8 visual pass adds a presentation-only ambience layer to the existing v2.7 Grand Hearth room. It does not change Town Hall geometry, stable object IDs, patrons, collision, the company runner, first spawn, portal behavior, objective flow, combat, or save schema v25.

## Runtime art contract

- Runtime pin: `Docs/ArtReferences/grand-hearth-ambience-atlas-runtime-v2.8.0.png`
- Exact RGBA 1536x1024, 3x2, 512-pixel square cells.
- Every cell has at least 36 transparent pixels on all four sides at alpha > 8.
- Zero visible bright-magenta pixels remain at alpha > 8.
- Row-major: warm hearth light pool; storm-door rain light spill; wrought-iron wall sconce; ember motes and smoke wisp; rain-window reflection; patron contact shadow.
- Runtime SHA-256: `9e78d112ce4e2ad6069f0754ebc127faae2d6d213278c9abf3c63663bda25f6f`.

## Provenance

- `Docs/ArtReferences/source-grand-hearth-ambience-atlas-v2.8.0-generated.png`
- `Docs/ArtReferences/source-grand-hearth-ambience-atlas-v2.8.0-alpha.png`
- `Docs/ArtReferences/grand-hearth-ambience-atlas-runtime-v2.8.0-prompt.txt`
- `Docs/ArtReferences/grand-hearth-ambience-atlas-runtime-v2.8.0-validation.json`

The atlas was generated with built-in ImageGen against the approved v2.7 set-piece atlas and a v2.7 built-player Local Map capture. The flat magenta matte was removed locally with the imagegen skill helper, then every cell was normalized into the exact runtime grid with the project atlas tooling.

## Presentation rules

- Warm light is centered on the monumental hearth and spreads into the gathering floor.
- Cool rain light leads westward from the east storm doors, visually reinforcing the required departure path.
- The rain-blue window throws a restrained reflection into the north-east floor.
- Two brighter wall sconces and a compact ember veil add depth without creating new interactables.
- Each authored patron receives a contact shadow before the citizen sprite is drawn.
- Region Map opacity is reduced so landmarks, route guidance, and the party marker remain primary.
- All placements are deterministic. No atmosphere cell changes movement, interaction, save data, or reduced-motion behavior.

## Candidate validation

- RuleSmoke passes with exact manifest, geometry, alpha-coverage, gutter, magenta-residue, and semantic-cell assertions.
- Focused SpriteArtRuntimeSmoke loads and accepts the exact live atlas.
- Full RuntimeBoot passes with a fixed smoke-test campaign seed while preserving New Game -> Muster -> Town Hall -> Midgaard behavior.
- The v2.8 Windows player builds successfully and includes the exact ambience runtime PNG alongside the v2.7 floor and set-piece sheets.
- Six built-player captures under `QA/v2.8.0-grand-hearth` cover Local and Region at 960x600, 1280x720, and 1920x1080. All finish `complete=True`, `failure=None`; the deterministic visual-QA packet passes with capture-set SHA-256 `ae6d6c73ca8c74d276b207b59f43027a22f6cbcd5540f3d858f15143c7637ab6`.
- Visual review accepts the warm hearth pool, horizontal storm-door spill, restrained north-east window reflection, smaller wall sconces, six readable patrons, continuous runner, and unobstructed departure route.
- Canonical packaging and clean-extracted packaged boot pass. `QA/v2.8.0-release-integrity/release-integrity-manifest.json` records clean tracked source, save v25, `cleanExtractLaunch=true`, `packagedBootMode=batchmode-explore-smoke`, `playerExitCode=0`, `packagedArtCount=83`, and the exact ambience-atlas hash. Physical-controller review and a complete human walkthrough remain manual.
