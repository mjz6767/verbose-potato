# Ashen Halls Art References

These images are original generated reference sheets for the v0.24.0 graphics direction. They are not copied game assets and should be treated as concept material.

Later versioned entries in this folder also include approved live runtime assets and their preserved generation, cleanup, prompt, and validation provenance. `Docs/ART_INTAKE.md` and `RuntimeArtManifest` define which exact files are active and package-selected.

## Files

- `enemy-sprites-reference-v0.24.png`: enemy silhouette direction, with special attention to kobold-like raiders, slingers, shamans, shieldbearers, and deeper-halls variants.
- `map-elements-reference-v0.24.png`: exploration tile/object direction for caches, shrines, stairs, camps, hazards, walls, roads, ruins, and ruins details.
- `map-exploration-reference-v0.25.png`: expanded exploration-map direction for regional tiles, roads, bridges, ruins, foliage edges, fog-of-war patches, camps, shrines, caches, and hazard tiles.
- `ui-world-casters-reference-v0.26.png`: compact UI/panel direction, world-map landmark studies, and enemy caster silhouettes for kobold shamans, bone wizards, glass mages, and hex priests.
- `splash-title-reference-v0.27.png`: title/splash direction for Ashen Halls: The Old Road.
- `combat-ui-sprites-reference-v0.27.png`: combat status frames, action bar, turn queue, party/enemy silhouettes, and old-school tactical box language.
- `beta-combat-casting-ui-reference-v0.28.png`: beta combat test art direction with title treatment, tactical board, formula chips, SFX cues, party portraits, and kobold shaman/wizard reference art. v0.28 also loads this file as a runtime texture atlas for visible in-game combat/portrait crops.
- `formula-lab-effects-reference-v0.29.png`: beta formula-lab direction for spell-school icons, mender/ember/hex/death effects, terrain creation, hazards, beta test controls, and SFX feedback. v0.29 loads this file as a runtime texture atlas for casting-panel icons.
- `combat-sprite-sheet-source-v0.29.png`: generated source sheet for centered combat sprites on a flat chroma-key background.
- `combat-sprite-sheet-alpha-v0.29.png`: local chroma-key removal output with transparent background. v0.29 loads this as the main combat board sprite sheet using fixed 4x4 grid slicing and bottom-center anchoring.
- `spell-card-icons-reference-v0.38.png`: generated spell-card icon direction for readable Cast-menu cards.
- `class-icons-tavern-reference-v0.39.png`: generated class-icon and tavern customization direction for Warrior, Ranger, Rogue, Wizard, Mage, Warlock, Priest, Paladin, and the first five race portrait styles. This is the art target for future cropped in-game class icons.
- `class-icon-atlas-runtime-v0.40.png`: generated 4x2 runtime atlas for Warrior, Ranger, Rogue, Wizard, Mage, Warlock, Priest, and Paladin icons. v0.40 loads this sheet directly for tavern, combat, turn queue, and Armory class badges.
- `world-object-atlas-runtime-v0.41.png`: generated 5x2 runtime atlas for exploration-map objects in enum order: cache, shrine, encounter, stairs, camp, town, obelisk, ruin, bridge, cave. v0.41 loads this sheet directly for world-map icons.
- `combat-sprite-sheet-source-v0.43.png`: generated 4x4 combat sprite source on chroma key for the large art update.
- `combat-sprite-sheet-alpha-v0.43.png`: local chroma-key removal output. v0.43 loads this as the preferred transparent combat board sprite sheet.
- `world-environment-atlas-runtime-v0.43.png`: generated 5x4 runtime atlas for world objects plus water, roads, cave floor, walls, sewer grate, moss path, crystal rubble, and red basalt tile overlays.
- `magic-ui-atlas-runtime-v0.43.png`: generated 4x4 runtime atlas for spell icons, terrain obstacles, and combat command glyphs.
- `item-icon-atlas-runtime-v0.43.png`: generated 5x4 runtime atlas for weapons, armor, potions, elixirs, coins, scrolls, rings, boots, and helms.
- `enemy-roster-atlas-runtime-v0.43.png`: generated 5x4 runtime enemy roster atlas for kobolds, rats, ratfolk classes, drow classes, demons, shade, and bone priest portraits.
- `title-backdrop-runtime-v2.4.0.png`: original Grand Hearth opening tableau selected for the v2.4 title screen, with a calm upper-left logo field, storm-lit doorway, four departing adventurers, and a dark right-side menu field. The full generation brief and checksum are recorded in `title-backdrop-runtime-v2.4.0-prompt.txt`.
- `world-threat-habitat-atlas-runtime-v2.4.0.png`: approved exact 4x2 world-map habitat sheet in stable Rat Warren, Plague-Bell Midden, Kobold Ambush Camp, Kobold Shaman Totem Yard, Drow Moon-Silk Watchpost, Undead Ossuary, Demon Breach, and Ruined Road Waystation order. Full provenance is recorded in `Docs/WORLD_MAP_V2.4_ART_HANDOFF.md` and the adjacent prompt and validation files.
- `world-npc-citizen-atlas-runtime-v2.4.0.png`: approved exact 4x2 ambient-citizen sheet in stable lamplighter, fishmonger, tailor, mason, apothecary, road pilgrim, gravedigger, and caravan guide order. These are non-interactive ambience rather than named dialogue identities.
- `player-exploration-role-atlas-runtime-v2.4.0.png`: approved exact 4x2 solo exploration sheet in stable shield, pike, bow, knife, mender, ember, hex, and ward order. Multi-member parties retain the established group marker; full provenance for both character sheets is recorded in `Docs/CHARACTER_ART_V2.4_HANDOFF.md`.

## Usage Notes

- Use these sheets to guide shape language, color, and readability.
- Translate concepts into simpler Ashen Halls sprites before importing them into runtime art. v0.28 starts this bridge with direct texture-atlas crops; v0.29 extends it with formula/effect icons and a proper transparent combat sprite sheet. v0.40 adds a runtime class-icon atlas, v0.41 adds a runtime world-object atlas, and v0.43 greatly expands runtime sprite, item, environment, enemy, and magic/UI atlases. Later passes should clean and redraw these icons in LibreSprite/Aseprite for sharper pixel consistency.
- LibreSprite is staged in `outputs/tools/` and is the preferred free editor for the next hand-drawn sprite-sheet pass.
- Keep all final work original; do not copy Nahlakh assets, UI screens, or exact spell tables.
