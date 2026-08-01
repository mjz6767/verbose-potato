# Demon Form ability icons v2.0.0

The active `ability-icon-atlas-runtime-v2.0.0.png` preserves cells 0-19 of the approved v1.97 sheet exactly and appends one 4-cell row:

- 20 — Rift Pounce
- 21 — Abyssal Whirl
- 22 — Soul Rend
- 23 — Dread Roar

The four new subjects were created with OpenAI's built-in image generation mode from the existing ability-icon and greater-demon art as visual references. The generated chroma-key source is retained as `demon-form-ability-icons-source-v2.0.0.png`. `Tools/BuildDemonFormAbilityAtlas.py` performs deterministic alpha cleanup, despill, trimming, centering, and composition into the final exact 4 by 6, 1024 by 1536 runtime atlas.

## Generation prompt

> Use case: stylized-concept. Asset type: 2x2 game UI icon sheet for Ash & Brimstone demon-form combat skills. Create exactly four distinct square painted fantasy RPG ability icons arranged in a precise 2 columns by 2 rows grid. Image 1 is the required icon rendering/style reference; Image 2 is the required greater-demon visual identity reference. Top-left Rift Pounce: a horned demonic claw and hoof bursting forward through a violet rift. Top-right Abyssal Whirlwind: four crimson-gold demon claws spiraling in a violent circular slash. Bottom-left Soul Rend: a black-red claw tearing a luminous violet soul flame from a cracked heart. Bottom-right Dread Roar: a frontal horned demon maw releasing concentric red-violet shock rings. Highly polished hand-painted retro fantasy game UI icons, crisp silhouettes, dark iron, blood red, ember orange, deep violet, and small pale highlights; match the reference detail density, framing, and material finish while borrowing the demon reference's horns, claws, chains, and infernal palette. Exact 2x2 grid; each icon centered and fully contained inside its equal square quadrant; generous 12 percent internal padding; no overlap; readable at 64 pixels. Perfectly flat solid #00ff00 chroma-key background with no shadows, gradients, texture, frame, dividers, reflections, floor plane, or lighting variation. Do not use #00ff00 in the icons. No text, letters, numbers, borders, logos, watermark, characters, background scenery, photorealism, generic app glyphs, muddy contrast, extra icons, or cropped elements.

The runtime atlas is transparent; the chroma background does not ship or render.
