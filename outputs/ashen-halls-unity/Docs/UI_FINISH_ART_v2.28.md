# UI finish art v2.28

## Delivered asset

- Runtime candidate: `Docs/ArtReferences/ui-hearth-divider-runtime-v2.28.png`
- Single original hearth-themed horizontal divider: aged muted brass scrollwork, charcoal spear tips, and a small ember diamond.
- Placement: the reserved right side of pause and loot headers, separate from heading text and interactive controls.
- Source and runtime candidate are the same unmodified RGBA PNG. No crop, recoloring, scaling, alpha cleanup, or post-generation image editing was performed.
- Dimensions: **2172 × 724** pixels, **3:1** aspect ratio.
- SHA-256: `4D130586CDB212224E29A909B6075FB851E4F36FFB16493F211C06F569DD04C3`.

## Generation provenance

Generated with the **built-in ImageGen tool**, intent `generate`, use case `stylized-concept`, with `transparent_background: true`. No CLI/API fallback, API key, or external image-editing tool was used. The exact underlying image model identifier was not exposed by the tool and is not claimed here.

Original tool output retained at:

`C:/Users/markz/.codex/generated_images/01a0e033-5b14-7b33-8511-d271d3732d9e/exec-ae98ac3a-69e9-453d-bcd1-8bce2d0fb42d.png`

Style context inspected locally: `Docs/ArtReferences/tavern-ui-atlas-runtime-v1.5.9.png` and its prompt, `Docs/ART_INTAKE.md`, and `Docs/GRAND_HEARTH_ART_V2.7_HANDOFF.md`. The generation request was text-only; no local image was supplied as an input image.

### Exact generation prompt

```text
Use case: stylized-concept
Asset type: A single production-ready transparent horizontal UI divider ornament for Ashen Halls, a dark fantasy pixel-art tactical RPG.
Primary request: Create one original, quiet, elegant hearth-themed horizontal divider flourish suitable beneath a small dialog or inventory heading, readable when rendered only 100-220 pixels wide. The ornament is one symmetric continuous design: an extremely small warm ember diamond at the exact center, two delicate weathered brass scroll branches extending horizontally and tapering to charcoal-gray fine spear tips. Keep the composition restrained and airy; ornamentation should support text rather than compete with it.
Style/medium: Painterly pixel art with crisp visible pixel clusters, carefully shaded aged metal, a classic dark fantasy computer RPG interface finish. Not photorealistic, not shiny 3D, not a vector illustration.
Color palette: muted old brass and warm gray against charcoal recesses; only a tiny low-saturation ember-orange center. Minimal highlights, no white or bright-yellow sparkles, no teal.
Composition/framing: Wide 3:1 canvas, preferably 1536x512. Center the single ornament exactly. Entire motif occupies the central 80% of canvas width and at most 30% of canvas height, leaving generous completely transparent gutters on all four edges. Preserve transparent open space within every curl. View straight-on. Single isolated motif only, no duplicates or additional assets.
Scene/backdrop: True transparent RGBA alpha background, including all empty interior gaps. No visible background at all.
Constraints: No text, letters, numbers, runes, logos, watermark, panels, frames, button, border, backdrop, shadow plate, floor, faux transparency, gray checkerboard, white or black background. No large flame, fire glow cloud, particles, detached sparks, jewels, skulls, crowns, wings, or heavy ornate crest. Do not clip any extremity. Deliver an isolated subtle UI sprite with genuinely transparent alpha.
```

## Inspection and alpha validation

The generated result was visually inspected: one centered symmetric ornament, with readable silhouette and a small warm center; no text, frame, panel, floor, or visible checkerboard. The saved PNG was read without modification using System.Drawing:

| Property | Result |
| --- | --- |
| Pixel format | Format32bppArgb |
| Fully transparent pixels | 1,472,964 (93.6685%) |
| Fully opaque pixels | 234 |
| Partially transparent pixels | 99,330 |
| Pixels with alpha > 8 | 76,565 |
| Mean alpha among pixels with alpha > 8 | 226.48 / 255 |
| Pixels with alpha >= 192 | 66,881 |
| Bounds at alpha > 8, inclusive, top-left origin | x=161..2009, y=257..462 |
| Clear gutter at alpha > 8 | left 161, right 162, top 257, bottom 261 pixels |
| Visible pixels touching outer image edge | 0 |
| Top-left alpha | 0 |
| Center alpha | 253 |

The inner open spaces and outer gutters use genuine alpha, not a baked background. Pixel values have been preserved directly from ImageGen.

## Integration guidance and remaining validation

`UiOrnament` preserves the original PNG and its alpha. It uses a sprite rectangle around the visible alpha bounds with a two-pixel safety margin, preserving the roughly 8.8:1 motif aspect ratio inside each reserved header rectangle. The pause and loot placements use restrained tint opacity below the heading's visual priority. The ornament has no raycast target, animation, or gameplay state. Missing or opaque artwork leaves an empty decorative area; the controls remain available. A reference-counted cache releases the shared sprite and texture when the last owning screen is destroyed.

The character-customization chat coordinates the single combined v2.28 release, manifest pin, package inventory, and final build. The UI pass supplies `ExplorationHudScreen`, `PauseMenuScreen`, `LootPopupScreen`, `UiOrnament`, and the `UiFinishCapture` offscreen review fixture. Unity/runtime/build validation is recorded below only after execution. The ArtReferences directory may be locally ignored: promote this exact PNG explicitly when accepted, without broadly adding the art workspace.

## Integrated review

The final licensed native-canvas run passed with exit 0 (`QA/ui-finish-capture-v2.28-final.log`), including actual artwork decoding, shared sprite/texture lifetime, and non-raycast assertions. Captures under `QA/ui-finish/20260927-003118-917` show the static ornament in pause and loot headers at 960×600, 1280×720 and 1920×1080. It remains visibly subordinate to headings, keeps clear of text, and has no alpha backplate or clipping. The image bytes and hash above remain unchanged. See `UI_FINISH_v2.28.md` for the layout and validation scope.
