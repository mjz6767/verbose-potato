# UI layout and finish — v2.28

This pass is part of the single combined v2.28 update coordinated by the character-customization chat. It leaves character creation and building-art edits with their owning chats.

## Presentation changes

- Exploration: more space for the chapter line, clearer separation after the location/danger heading, quieter panel edges and party/log backgrounds, and a single expanded party heading. Class/level subtitles retain sufficient line height at large interface scales. Objectives, all four party members, HP warnings, resource counts and shortcuts remain visible through the existing layouts.
- Pause: Continue is the primary action; Save and Load share a row; secondary actions have lighter borders. Settings controls have larger targets. Explicit navigation follows the grouped layout, skips unavailable Retreat, and moves focus out of hidden settings.
- Loot: the item and comparison lead; repeated “ACQUIRED” copy and three nested resource boxes are removed. Resources retain color accents and labels. The modal fits its short summary instead of growing an empty outcome panel on larger screens. Continue, comparison and quick-equip remain available under their existing rules.
- Pause and loot headers receive one original, static brass-and-ember ornament in reserved space. It never covers text or receives input. See `UI_FINISH_ART_v2.28.md` for the exact image prompt, source, dimensions, alpha validation and hash.
- Button backgrounds now use a single tint, making hover and selection states visible without multiplying two dark fills.

No gameplay, balance, save schema, inventory calculation, external AI request or runtime service is introduced. The generated artwork is a packaged local asset.

## Verification

Source compilation passed at `QA/source-compile/20260927-002054-798b326e`: 150 runtime and 37 editor files, zero diagnostics. The subsequent licensed Unity runs compiled the final visual corrections as well.

`AshenHalls.Editor.UiFinishCapture.Capture` renders isolated native-canvas fixtures at 960×600, 1280×720 and 1920×1080. Cases cover collapsed/expanded exploration with healthy/injured/critical/down party members, pause/settings, equipment loot and resource-only loot. It runs existing accessibility/HUD checks and rejects blank captures before reporting success. These fixtures do not write campaign saves and do not substitute for final-player review of the map underneath the HUD.

Final licensed Unity capture **passed with exit 0**, log `QA/ui-finish-capture-v2.28-final.log`. It executed `PresentationAccessibilitySmoke`, `WorldMapHudAuditSmoke` and `UiFinishSmoke`, then produced 18 nonblank exact-size images in `QA/ui-finish/20260927-003118-917`. The UI-specific check exercises actual directional navigation, settings focus retention/recovery, subtitle text generation at six supported resolutions, non-interactive decorations, and shared texture/sprite destruction. `UiFinishSmoke` is also registered in the combined Full audit and Windows build gates.

Visual review identified and corrected high-resolution class subtitle clipping and excess vertical space in large-screen loot. Final expanded HUD images show class/level text at every capture size while preserving critical/down HP markers and the available Latest log entries. Six pause variants retain readable settings, footer clearance and quiet ornaments. Loot actions, resource labels and summary text fit at every capture size.

The first editor lifetime test exposed cleanup callbacks not running in Edit Mode. `UiOrnament` and the existing `UiOwnedCanvasLifetime` now use `ExecuteAlways`, and the final real Unity lifetime assertions pass. A shared building-check import was also repaired by fixing its new malformed metadata GUID; no building pixels or validation thresholds were altered here.

These are native editor-canvas fixtures with deliberate representative content, including no map behind the HUD. They do not certify physical-controller feel, a full human campaign, or final retail-player composition. Combined packaging and final-player checks remain with the coordinated release owner.
