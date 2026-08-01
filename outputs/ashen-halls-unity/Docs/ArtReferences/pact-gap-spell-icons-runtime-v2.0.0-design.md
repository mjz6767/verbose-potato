# Pact gap spell icons v2.0.0

The active `signature-spell-icon-atlas-runtime-v2.0.0.png` preserves cells 0-48 of the approved v1.97 sheet exactly, appends Rift Bolt at cell 49 and Rift Step at cell 50, and leaves cells 51-55 transparent for later formulas.

The two new subjects were created with OpenAI's built-in image generation mode from the existing spell-icon and greater-demon art as visual references. The generated chroma-key source is retained as `pact-gap-spell-icons-source-v2.0.0.png`. `Tools/BuildPactGapSpellAtlas.py` performs deterministic alpha cleanup, despill, trimming, centering, and composition into the final exact 7 by 8, 1792 by 2048 runtime atlas.

## Generation prompt

> Use case: stylized-concept. Asset type: two game Spellbook icons for new warlock pact formulas. Create exactly two distinct square painted fantasy RPG spell icons arranged side by side in a precise 2 columns by 1 row layout. Image 1 is the required spell-icon rendering/style reference; Image 2 is the required pact/demon visual identity reference. Left Rift Bolt: a compact barbed violet-black projectile tearing out of a small cracked rift, with an ember-white impact point and restrained red sparks. Right Rift Step: a horned boot or armored demonic foot crossing between two linked violet portals, clearly communicating short-range teleportation. Highly polished hand-painted retro fantasy game UI icons, crisp silhouettes, blackened iron, bone, deep violet, blood red, restrained gold, and ember-white highlights; match the reference detail density, framing, and readable 64-pixel silhouette. Exact two equal vertical halves; one centered icon fully contained in each half; generous 12 percent internal padding; no overlap or dividers. Perfectly flat solid #00ff00 chroma-key background with no shadows, gradients, texture, frames, reflections, floor plane, or lighting variation. Do not use #00ff00 in the icons. No text, letters, numbers, borders, logos, watermark, characters, background scenery, generic app glyphs, muddy contrast, extra icons, cropped elements, or photorealism.

The runtime atlas is transparent; the chroma background does not ship or render.
