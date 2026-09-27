# Building alpha cleanup — v2.28.0

## Scope

The user requested the same native Aseprite cleanup used for NPCs on buildings with white outlines, with all active UI/character/art work shipped as one combined update. The character-customization chat owns the combined v2.28.0 audit, build, packaging and push. The user explicitly confirmed `mjz6767/verbose-potato`, branch `main`, as the push destination. No independent release is produced by this building-art pass.

## Diagnosis and repair

The current v2.21 town atlas contains opaque near-white contours around nine architecture sprites, plus enclosed checkerboard remnants above hanging shop signs. Those pixels originate in the source art; the runtime uses Point filtering, Clamp wrapping, no mipmaps and alpha-enabled drawing. Sampling changes cannot remove baked foreground pixels. The dedicated gate and wall atlases do not exhibit the same defect; their legitimate pale masonry highlights remain untouched.

`Tools/RepairWorldBuildingAtlas.ps1` uses installed Aseprite 1.3.18.5 and `RepairBuildingAtlasMatte.lua`. It verifies the exact v2.21 source hash, repairs only cells 0, 1, 3, 4, 5, 6, 7, 11 and 14, and preserves the 1280×1024 canvas and stable 5×4 cell mapping. These cells are market, temple, tavern, armorer, provisions, weaponsmith, enchanter, town hall/Grand Hearth exterior, and cookhouse.

Exterior cleanup is restricted to neutral pixels within two pixels of the original transparent edge, followed by one non-progressive fringe pass within that same limit. Reviewed sign-gap seeds handle enclosed matte separately. Authored smoke regions in cells 4, 6 and 14 are explicitly protected. There is no global white key, repaint, resizing, pose shift or color adjustment to the retained art. **7,334 matte pixels are cleared.**

The native Aseprite file retains the unchanged original on a hidden reference layer and the repair on the visible layer. The eleven other town-atlas cells, all NPC atlases, dedicated gate/wall sheets, regional landmarks and interior art are outside the repair scope.

## Reproducible artifacts

- `Docs/ArtReferences/midgaard-town-atlas-runtime-v2.28.0.png`: SHA-256 `E48310A69517121FC8FA0AEE9523D52BA5F78244A2F78738B0B4F05909A430E4`.
- `Docs/ArtReferences/source-midgaard-town-atlas-v2.28.0.aseprite`: layered editable source; SHA-256 `C702FA28DFF8E1D9401D7485921A9A0970F934D77D98604345C62FE59D138438`.
- `Docs/ArtReferences/midgaard-town-atlas-runtime-v2.28.0-validation.json`: tool version, geometry, source/output/native/recipe hashes, per-cell removed counts, protected regions and reviewed gap seeds.
- Exact retained input: `midgaard-town-atlas-runtime-v2.21.0.png`, SHA-256 `593C1DF8EA8142123EA3D2582A87D7900F89F0BFF1376778A7B58CEABAFD7501`.
- No new image-generation prompt: this is an explicitly requested native Aseprite edit. Original generation provenance remains in `source-midgaard-architecture-v2.21.0-prompt.txt` and its source PNG.

Run `Tools/RepairWorldBuildingAtlas.ps1` with installed Aseprite to regenerate, optionally choosing a separate `-OutputDirectory` for inspection. The source hash gate deliberately prevents applying the authored masks to an unrelated future atlas. The prior PNG is retained, not overwritten.

## Verification and release status

- Independent atlas visual review accepted the repair: roof/facade contours and sign-gap remnants are removed, while masonry shapes, shingles, sign icons, flags, chimney bodies and intentionally light smoke remain intact. The gate/wall, regional-landmark and interior families were also inspected before deciding not to alter their highlights.
- Measured architecture coverage is 37.1628–49.6887%, still within the existing 34–55% contract. All existing width/height assertions pass unchanged: market/provisions stay 188px high, temple stays 170px wide; only enchanter visible width changes from 200 to 199px. Every strict 18px safe gutter remains fully transparent. **No existing art threshold needs loosening.**
- All eleven untargeted cells have zero decoded RGBA differences. Fourteen independently sampled smoke, warm/purple window, sign, stone, flag, ivory-canvas and fountain details are unchanged.
- Native Aseprite roundtrip export passes with zero RGBA mismatches across all 1,310,720 pixels. Source-to-repair comparison finds exactly 7,334 changed pixels, all changed from visible matte to fully transparent; no retained color was altered. The tracked report matches the final source, output, native-file and recipe hashes. Earlier ignored QA prototype reports are not release provenance.
- `BuildingAtlasAlphaSmoke.RunOrThrow()` checks 19 removed matte sites (ten enclosed sign-gap samples and nine exterior contours), seven preserved color details, whole-region fingerprints for all three smoke masks, and source fingerprints for all eleven untouched cells. The decoded-PNG fixture cross-check passes. Root reviewed the new C# test; shared integration registers it in the combined audit/build gates. The initially overlong new meta GUID was corrected before the combined Unity run; retain its valid 32-character value.
- Script second-pass review found no implementation blocker. Authored sign-hole flooding deliberately differs from the bounded exterior cleanup; it is safe only for the exact reviewed source. The wrapper enforces that source hash before applying it.

Runtime screenshot and final release verification belong to the combined v2.28.0 release, not to the earlier NPC preview. Do not interpret an atlas-only review as proof that a retail build was tested or published.

## Handoff

Building code/art are frozen for the release owner: the three ArtReferences artifacts above; `Assets/Editor/BuildingAtlasAlphaSmoke.cs` and its meta; both new repair tools; and this report. Release owner owns shared manifest/boot-assert/build-audit integration and combined changelog/ART_INTAKE updates. Map captures will be run on the actual combined player after the owner supplies it. No parallel Unity editor or separate package/push is started by this building-art chat.
