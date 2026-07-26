# Art Thread Revision Audit v0.86

Audit date: 2026-06-30

## Completed Revisions Already In Unity

The Aseprite cleanup workspace contains three publishable v0.80 exports under:

`<Ashen Halls workspace>\AsepriteCleanup-v0.80\03-export-png`

All three are already present in Unity `Docs\ArtReferences` with matching SHA-256 hashes:

- `combat-terrain-atlas-runtime-v0.80.png`
- `world-map-exploration-tile-atlas-runtime-v0.80.png`
- `world-map-overlay-atlas-runtime-v0.80.png`

These were previously incorporated by the v0.83 runtime art wiring pass. No additional copy was needed.

## Generated But Not Publishable Yet

The art thread also generated candidate/work revisions for prior art. These are useful references, but they are not final runtime assets under the cleanup guardrails:

- `02-aseprite-work\midgaard-tile-atlas-runtime-v0.80-work.png`
- `02-aseprite-work\world-map-prop-atlas-runtime-v0.80-work.png`
- `02-aseprite-work\world-map-token-sprite-atlas-runtime-v0.80-work.png`
- `02-aseprite-work\kobold-route-atlas-runtime-v0.80-work.png`
- `02-aseprite-work\kobold-boss-atlas-runtime-v0.80-work.png`
- `02-aseprite-work\kobold-cave-prop-atlas-runtime-v0.80-work.png`
- `07-candidates\alpha-cleanup-candidates-v0.80\...\*-candidate-atlas-v0.80.png`

These remain candidate/starter/reference material only. They should not be published directly.

## Validation State

The current cleanup validation still reports:

- `CHANGED 0` starter cells
- `world-map-prop-atlas-runtime-v0.80`: `0/20 cleaned`
- `world-map-token-sprite-atlas-runtime-v0.80`: `0/20 cleaned`
- `kobold-route-atlas-runtime-v0.80`: `0/16 cleaned`
- `kobold-boss-atlas-runtime-v0.80`: `0/16 cleaned`
- `kobold-cave-prop-atlas-runtime-v0.80`: `0/16 cleaned`

`04-reference\cleaned-export-validation-v0.80.csv` confirms that token, prop, kobold route, kobold boss, kobold cave prop, and Midgaard v0.81-style cleanup exports are still missing.

## Next Safe Action

Continue manual Aseprite cleanup from the starter packs. After saving real edits, rerun:

```powershell
.\Watch-Starter-Edits.ps1
.\Review-Starter-Edits.ps1 -Pack world-map-prop-atlas-runtime-v0.80 -Cells 0,2,3,6 -Approve
.\Review-Starter-Edits.ps1 -Pack world-map-token-sprite-atlas-runtime-v0.80 -Cells 1,3,5,6 -Approve
.\Validate-Cell-Cleanup-Packs.ps1
```

Only then assemble and publish cleaned runtime atlases.
