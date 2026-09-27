# Portraits by the Fire — character creation

The character folio presents four illustrated companion cards, a large portrait of the selected recruit, direct race and class choices, and plain-language descriptions. A separate Attributes & details page contains point allocation, training, background, heraldry and starting equipment. Changing a choice updates that recruit; choosing the current class again is harmless. Begin preserves the customized company. Quick Start deliberately selects the default company.

## Original artwork

Built-in ImageGen created five original opaque portrait sheets. Each sheet is 1774 × 887 pixels, arranged in four columns and two rows. Fractional UV cells retain the original generated pixels without image editing or resampling. Each race has eight individually painted portraits; Wizard and Mage have separate paintings.

| Race | Runtime file |
| --- | --- |
| Human | `Docs/ArtReferences/character-portrait-human-atlas-runtime-v2.28.0.png` |
| Dusk Elf | `Docs/ArtReferences/character-portrait-dusk-elf-atlas-runtime-v2.28.0.png` |
| Stoneborn | `Docs/ArtReferences/character-portrait-stoneborn-atlas-runtime-v2.28.0.png` |
| Fenkin | `Docs/ArtReferences/character-portrait-fenkin-atlas-runtime-v2.28.0.png` |
| Ashling | `Docs/ArtReferences/character-portrait-ashling-atlas-runtime-v2.28.0.png` |

Every sheet uses the same top-left reading order: Rogue, Warrior, Ranger, Wizard, Mage, Warlock, Priest, Paladin. Source prompts are preserved verbatim in `CHARACTER_CREATION_ART_PROMPTS_v2.28.md`. `CHARACTER_PORTRAIT_VALIDATION_v2.28.json` records exact image hashes and the 40 unique opaque cell samples. Regenerate that independent report with `Tools/TestCharacterPortraitArt.ps1`.

The art depicts the project's existing races: natural human faces, lilac dusk elves, gray stone-skinned Stoneborn, scaled reptilian Fenkin, and ember-brown Ashlings with round ears. Equipment and lighting distinguish each calling. These illustrations are character portraits; the existing combat and exploration sprite systems continue to represent gameplay equipment and positioning.

## Runtime and verification

`CharacterCreationCatalog` maps validated race/class keys to exact manifest pins and unique cells. Original paintings are loaded once per file and released with the game instance. The roster, large portrait and class previews use the same mapping. Missing paintings leave the text choices usable.

Race bonuses come from the same catalog used by gameplay calculations. Class changes use the existing starter-class rules, and current-class selection does not reset adjustments. Attribute/training controls show availability; Begin requires assigned starting attributes. Names, race, class, background, heraldry, training and gear participate in refresh state.

Focused native-control tests exercise all 40 choices, correct displayed portrait cells, member isolation, naming, point allocation and starting the customized party. Screen captures use the actual Unity canvas rendered through an offscreen camera, allowing image inspection without a foreground game window. The capture tool rejects blank or incorrectly sized results. Editor preview and final-player evidence are recorded separately; neither implies a physical-controller playthrough.

Final integrated results and package hashes are recorded in `ReleaseEvidence/v2.28.0-summary.json` after verification. Save schema remains v27.
