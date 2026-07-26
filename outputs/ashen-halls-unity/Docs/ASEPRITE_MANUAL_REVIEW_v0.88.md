# Aseprite Manual Review v0.88

## Opened For Review

Aseprite was launched with:

- Work copy: `<Ashen Halls workspace>\AsepriteCleanup-v0.80\02-aseprite-work\manual-review-v0.88\midgaard-sewer-atlas-runtime-v0.88-work.png`
- Reference: `<Ashen Halls workspace>\AsepriteCleanup-v0.80\02-aseprite-work\manual-review-v0.88\midgaard-sewer-atlas-runtime-v0.56-reference.png`

The runtime file in Unity was not opened for direct editing.

## Why This Sheet

`midgaard-sewer-atlas-runtime-v0.88.png` is a new generated opaque 5x4 sewer/cistern sheet. It is loader-valid and packaged in v0.88.0, but it may benefit from hand review for:

- cell centering
- noisy borders or baked grid remnants
- rat token readability at small map size
- sewer entrance and rat-pelt quest icon readability
- overly dark cells that flatten against the Midgaard map

The newer route/service/dungeon/faction scaffold sheets already have alpha channels and are acceptable beta scaffolds unless a specific bad cell is found.

## Do Not Edit Directly

Do not overwrite:

`Docs\ArtReferences\midgaard-sewer-atlas-runtime-v0.88.png`

Manual edits should be saved to the work copy first.

## Promotion Plan

After the work copy is edited and saved:

1. Validate size and mode.
2. Compare the edited work copy against the v0.88 runtime source.
3. Copy the approved result into Unity as a new versioned file, likely:
   `Docs\ArtReferences\midgaard-sewer-atlas-runtime-v0.89.png`
4. Add prompt/provenance or edit notes.
5. Build/package the next version.

Save version should remain unchanged unless gameplay data changes.
