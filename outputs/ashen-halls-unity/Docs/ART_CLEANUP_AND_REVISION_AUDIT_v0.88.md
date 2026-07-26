# Art Cleanup and Revision Audit v0.88

## Aseprite Cleanup Gate

Source handoff:
`<Ashen Halls workspace>\AsepriteCleanup-v0.80\MAIN-THREAD-HANDOFF-v0.80.md`

Scripts rerun from the provided cleanup workspace:

- `Watch-Starter-Edits.ps1`: `CHANGED 0 MISSING 0 UNCHANGED 40`
- `Closeout-Aseprite-Session.ps1`: all five cleanup packs remain `0 cleaned`
- `Validate-Cell-Cleanup-Packs.ps1`: all five cleanup packs remain `0 cleaned`

Result: no starter/candidate PNGs were promoted to Unity. The blocked packs are still:

- `world-map-token-sprite-atlas-runtime-v0.80`
- `world-map-prop-atlas-runtime-v0.80`
- `kobold-route-atlas-runtime-v0.80`
- `kobold-boss-atlas-runtime-v0.80`
- `kobold-cave-prop-atlas-runtime-v0.80`

## Published v0.80 Revisions

The only validated exports in `03-export-png` are already present in Unity with matching SHA-256 hashes:

- `combat-terrain-atlas-runtime-v0.80.png`
- `world-map-exploration-tile-atlas-runtime-v0.80.png`
- `world-map-overlay-atlas-runtime-v0.80.png`

## Candidate/Revisions Not Promoted

The folders contain useful review material that is not publishable by the current workflow:

- `art-audit-v0.80\cleaned\world-map-token-sprite-atlas-runtime-v0.80*.png`
- `art-audit-v0.80\cleaned\world-map-prop-atlas-runtime-v0.80*.png`
- `AsepriteCleanup-v0.80\07-candidates\midgaard-tile-atlas-runtime-v0.81-candidate.png`
- incomplete candidate atlases under `AsepriteCleanup-v0.80\07-candidates`

These remain reference/candidate files because the handoff explicitly requires saved starter-cell edits, review, and validation before publishing.

## New Gap Fill Added

Generated and installed `Docs/ArtReferences/midgaard-sewer-atlas-runtime-v0.88.png`.

Reason: the sewer/cistern sheet was still using older v0.56 art and is not part of the blocked v0.80 starter-pack cleanup path. The runtime loader already accepts `midgaard-sewer-atlas-runtime-*` and selects the newest versioned file, so no C# loader changes were needed.

Validation:

- Size: `1402x1122`
- Mode: `RGB`
- Loader contract: passes `width >= 768 && height >= 600`
- Runtime cell layout: existing 5x4 atlas slicing
- SHA-256: `CEFB9280D14D74939C8EB003D14D89CB6E138BF5422ABD2E4CB7E8CC8B4F061D`

Prompt/provenance:
`Docs/ArtReferences/midgaard-sewer-atlas-runtime-v0.88-prompt.txt`
