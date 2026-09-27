# Pack swap consequence disclosure

This follow-up addresses the bounded reassignment-copy limitation recorded in the v2.36 weapon report. Giving Cairn's bow to another adventurer could previously lead with the recipient's gained range while leaving Cairn's lost shooting techniques unspecified in the compact comparison.

The Pack swap action now checks the actual incoming weapon for both adventurers through the shared weapon profiles. Lost ranger shooting takes priority, followed by decreased basic range. A single affected adventurer is named with the loss, for example `Review · Cairn: 5→1; shots disabled`. If both lose range, both consequences are shown, for example `Review · Range Luma 4→2; Maer 2→1`. A swap with neither loss keeps the existing recipient comparison and displaced-owner context.

The short warning replaces lower-priority numerical/intrinsic deltas only in these affected swap rows; the selected item's existing detail panel remains available. Names may be shortened in compact loss copy, especially when both adventurers are affected. If shortened names collide, `Hero 1`, `Hero 2`, and so forth identify their party positions. Full names remain in the existing action/ownership presentation. A mixed two-person loss uses the shorter phrase `no shots` to retain both range changes.

This is presentation only. Equipment scoring, review gates, explicit Equip/Swap actions, physical-item ownership rules, weapon profiles and combat mechanics are unchanged. The independent loot-screen redesign, screen layout, navigation, audio, version and release files are outside this change.

The changes are limited to `AshenHallsGame.WeaponTraining.cs`, `AshenHallsGame.Armory.cs`, and `WeaponTrainingSmoke.cs`, plus this note. New regressions cover both item-selection directions, a displaced owner losing shooting, the inverse swap, two simultaneous range losses, mixed shot/range loss, long/colliding names, unmutated preview/scoring and the original fallback comparison. They inspect the actual native Pack text and measure the complete warning against its production target-text rectangle at 960×600 and 1280×720 before invoking explicit native swaps.

## Validation

The focused licensed Unity check passed in `QA/coordinated-followup-weapon-training-attempt3.log` (`Weapon training smoke passed.`). It verifies the final native warning widths at both sizes as well as actual swaps and unchanged preview state. Source compilation also passed in `QA/source-compile/20260927-221443-8e946f74` with 175 runtime and 74 Editor sources and zero diagnostics; the later Unity run compiled the final name-shortening fix.

`AshenHalls.Editor.TownAndPackFollowupCapture.Capture` passed in `QA/coordinated-followup-native-capture-attempt3.log`. Its 26 native Editor captures include six Pack views: displaced ranger shooting loss, two simultaneous reach losses, and mixed shooting/reach loss at 960×600 and 1280×720. All six were directly reviewed; the new warnings and Swap/Back controls fit. The captures and per-case review are in `QA/town-and-pack-followups/20260927-221948-385/`.

Earlier attempts caught a long-name overflow (203 pixels of text in a 190-pixel row). The single-person compact name limit was shortened from eight to six characters without relaxing the width check. An earlier import also encountered an unrelated malformed environment-test metadata GUID; its owner repaired it before the passing runs. Failure logs remain available.

These are focused Editor checks and visual evidence, not an integrated player release. Ordinary fixture weapons use the existing WPN text fallback icons. The existing no-loss recipient comparison can still truncate lower-priority trailing text in a narrow row; this change guarantees the new decisive loss warning fits and does not redesign every comparison. The coordinating chat owns combined Full verification, player preview, and any packaging.
