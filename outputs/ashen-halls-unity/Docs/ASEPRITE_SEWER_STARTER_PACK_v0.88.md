# Midgaard Sewer Aseprite Starter Pack v0.88

Created a manual Aseprite starter pack for the generated sewer/cistern sheet:

`<Ashen Halls workspace>\AsepriteCleanup-v0.80\09-aseprite-starter-packs\midgaard-sewer-atlas-runtime-v0.88`

Opened in Aseprite:

- `reference\midgaard-sewer-v0.88-old-vs-new-contact.png`
- `reference\midgaard-sewer-v0.88-starter-contact.png`
- `starter-cells\cell-00.png` sewer grate entrance
- `starter-cells\cell-06.png` sewer rat token
- `starter-cells\cell-07.png` giant sewer rat token
- `starter-cells\cell-08.png` ratfolk cutthroat token
- `starter-cells\cell-09.png` ratfolk mage token
- `starter-cells\cell-12.png` rat pelt bundle
- `starter-cells\cell-17.png` warning lantern
- `starter-cells\cell-18.png` blocked tunnel rubble

The baseline hashes are in:

`starter-manifest.csv`

Manual cleanup goal:

- center each sprite/icon inside its cell
- remove obvious grid/border artifacts only when they hurt readability
- keep dark pixel details rather than overpruning silhouettes
- improve rat and ratfolk readability at small map size
- keep the Unity runtime file unchanged until an approved edited version is assembled as a new runtime filename

Promotion rule:

Do not overwrite `midgaard-sewer-atlas-runtime-v0.88.png`. If edited cells are saved and accepted, reassemble as `midgaard-sewer-atlas-runtime-v0.89.png` or later.

## v0.89 main-thread promotion

The main thread promoted a non-destructive derivative as:

- `Docs/ArtReferences/midgaard-sewer-atlas-runtime-v0.89.png`
- `Docs/ArtReferences/source-midgaard-sewer-atlas-v0.89-from-v0.88.png`
- `Docs/ArtReferences/midgaard-sewer-v0.89-contact.png`
- `Docs/ArtReferences/midgaard-sewer-atlas-runtime-v0.89-notes.txt`

This was a PIL/Aseprite-adjacent cleanup pass, not a manual painted-cell approval. It uses audit-bounded cutouts from the v0.88 sheet and keeps `v0.88` as provenance/source. Rat and ratfolk combat/roster rendering now prefers the v0.89 sewer atlas through the existing runtime loader.
