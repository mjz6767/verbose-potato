# Early progression power icons v2.9.0

OpenAI's built-in image generation mode created two project-bound chroma-key source sheets from the approved v2.0 ability and signature-spell atlases as visual references. No API/CLI fallback was used.

`Tools/BuildEarlyProgressionIconAtlases.py` removes no content from the approved atlases. It trims, fits, centers, and appends the cleaned source icons deterministically, records pixel hashes and source/runtime bounds, and verifies that every pre-v2.9 mapped cell remains byte-identical.

## Runtime contracts

- `ability-icon-atlas-runtime-v2.9.0.png`: 4 columns by 7 rows, 256-pixel cells, 1024 by 1792 pixels.
  - 24 - Sunder
  - 25 - Shadowstep
  - 26 - Quick Shot
  - 27 - intentionally transparent reserve cell
- `signature-spell-icon-atlas-runtime-v2.9.0.png`: unchanged 7 columns by 8 rows, 256-pixel cells, 1792 by 2048 pixels.
  - 51 - Dawn Pulse (`DWP`)
  - 52 - Cinderstorm (`CNS`)
  - 53 - Grave Hook (`GRH`)
  - 54 - Soul Veil (`SLV`)
  - 55 - Ashen Curse (`ACR`)

## Ability source prompt

> Use case: stylized-concept. Asset type: 2x2 source sheet for three new Ash & Brimstone Skillbook ability icons. Image 1 is a style reference only. Create exactly three distinct square painted fantasy RPG ability icons in a precise 2 columns by 2 rows grid: Sunder, a heavy notched warhammer splitting blackened iron; Shadowstep, a hood and dagger passing between violet crescent shadows; Quick Shot, two parallel broadhead arrows through one cyan-gold sight ring. Keep the fourth cell empty. Match the premium forged-grimoire, hand-painted 32-bit retro fantasy icon style, with crisp silhouettes, 3-5 value bands, rim light, 12 percent padding, and 56-pixel readability. Use a perfectly flat solid #00ff00 chroma-key background. No text, frames, scenery, watermark, contact shadows, cropped elements, or art crossing cells.

## Spell source prompt

> Use case: stylized-concept. Asset type: 3x2 source sheet for five new Ash & Brimstone Spellbook icons. Image 1 is a style reference only. Create exactly five distinct square icons: Dawn Pulse, an ivory-gold sunrise warding three allies; Cinderstorm, three cinders around a white-orange flame core; Grave Hook, a bone hook pulling a violet soul shard through a broken ring; Soul Veil, a horned shield wrapped by a violet-red spectral veil; Ashen Curse, an emblem split between ember flame and a bone-violet thorn rune. Keep the sixth cell empty. Match the approved forged-grimoire 32-bit painted style, with strong silhouettes, 3-5 value bands, rim light, 12 percent padding, and 56-pixel readability. Use a perfectly flat solid #00ff00 chroma-key background. No text, frames, scenery, watermark, contact shadows, cropped elements, or art crossing cells.

The retained `*-chromakey.png` files are untouched generation outputs. The `*-alpha.png` files were processed with the installed image-generation skill's chroma-key helper using border sampling, soft matte, threshold 10/64, and despill. Detailed bounds and hashes are in `early-progression-icon-atlases-runtime-v2.9.0-validation.json`.
