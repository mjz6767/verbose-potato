# Combat Effects v2.9 Art Handoff

This presentation-only candidate refreshes the existing 4x4 combat-effect art
without adding abilities or changing damage, targeting, unlocks, save data, or
the progression task's spell and skill icon atlases.

## Runtime contract

- Active manifest pin: `combat-spell-effects-atlas-runtime-v2.9.0.png`
- Exact RGBA size: 1280x1280.
- Grid: 4 columns by 4 rows; 320-pixel square cells.
- Every populated cell is normalized to at most 280x280, leaving at least 20
  transparent pixels on every side at alpha greater than 8.
- Runtime SHA-256: `F3FE0D14FD6BE218AB64AE177FDA741A4D6F7694BBFF96C89C576CC866D5A92A`.
- The displayed arrangement and cell meanings match the accepted v0.73 atlas.

Displayed rows, left to right:

1. Ember firebolt; fire impact bloom; descending meteor; meteor impact crown.
2. Frost shard; ice eruption; chain lightning; thunder nova.
3. Healing column; holy ward dome; void orb; collapsing void impact.
4. Binding vines; stone eruption; smoke cloud; martial arrow/target streak.

## Provenance

- `Docs/ArtReferences/source-combat-spell-effects-atlas-v2.9.0-chromakey.png`
- `Docs/ArtReferences/source-combat-spell-effects-atlas-v2.9.0-alpha.png`
- `Docs/ArtReferences/combat-spell-effects-atlas-runtime-v2.9.0.png`
- `Docs/ArtReferences/combat-spell-effects-atlas-runtime-v2.9.0-prompt.txt`
- `Docs/ArtReferences/combat-spell-effects-atlas-runtime-v2.9.0-validation.json`

The atlas was created with built-in ImageGen using the accepted v0.73 sheet as
the strict visual-layout reference. The imagegen skill helper removed the flat
green matte locally, and the project normalization tool centered each cell on
the exact runtime grid. `Docs/ArtReferences/*` is locally excluded, so the
release owner must deliberately force-add these five approved files.

## Integrated boundary status

- `RuntimeArtManifest.EpicSpellEffectsAtlas` exact-pins this approved runtime.
- `CombatPowerVisualRules` retains the stable semantic mappings while adding
  presentation timing and layered motifs; gameplay semantics are unchanged.
- Focused checks cover the exact pin, 1280x1280 geometry, gutters, and live
  impact routes.
- Combined RuleSmoke, focused combat-UI runtime smoke, SpriteArtRuntimeSmoke,
  and full RuntimeBoot pass with the exact pin and live mappings.
- The clean Windows build, staged/packaged-art integrity comparison, canonical
  package, and clean-extracted packaged boot pass. The nine-capture built-player
  matrix also passes deterministic validation and manual visual review under
  `QA/v2.9.0-release-visuals-741451a`.
