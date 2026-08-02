# Ash & Brimstone Release Checklist

Use this for each small release zip.

## Before Packaging

- Update the single `PackageVersion` source in `Assets/Scripts/VersionInfo.cs`.
- Confirm `Assets/Editor/BuildWindows.cs` and `Tools/BuildAndPackageWindows.ps1` derive and validate that value.
- Update `README_PLAY.txt`.
- Add a short entry to `CHANGELOG.md`.
- Update `KNOWN_ISSUES.txt` if tester workarounds changed.
- Bump `SaveVersion` only when saved data shape changes.
- For v1.92, `rule-smoke-v1.92.0-dev.log`, `runtime-smoke-v1.92.0-dev.log`, and `build-v1.92.0-dev.log` pass. The complete deterministic visual-QA packet also passes; final distribution packaging must use that same validated source state.
- For v1.93, confirm the current rule/runtime/build logs pass, the ten-capture world-map packet passes, and the source, staged, and zipped v1.93 gate, player-character, and Midgaard NPC atlases have identical hashes. Preserve the accepted v1.92 QA folder and archive.
- For v1.94, confirm the current rule/runtime/combat-UI/build logs pass and retain clean packaged-player captures for movement route, legal attack, blocked attack, armed area spell, Spellbook, and Skillbook at both endpoint resolutions. Every capture log must name its staged state and finish with `complete=True`, `failure=None`.
- For v1.95, confirm the current rule/runtime/combat-UI/build logs pass and retain direct packaged-player captures for projected move danger, unobscured legal/blocked/area targeting, lower-left field countdowns, a six-row 720p Skillbook, and locked Spellbook/Skillbook unlock metadata. Every capture log must name its staged state and finish with `complete=True`, `failure=None`.
- For v1.96, confirm the current rule/runtime/combat-UI/build logs pass and retain direct packaged-player captures of Spellbook and Skillbook Ready, long-list/bottom, Locked, hover-preview, and armed-targeting states. Verify that formula-specific cards never fall back to duplicated generic art and that every capture log finishes with `complete=True`, `failure=None`.
- For v1.97, confirm the current rule/runtime/combat-UI/build logs pass; verify the exact ability, signature-spell, lightning, and power-book-state manifest pins; and retain direct packaged-player comparisons for Preview, Selection, Targeting, Locked, Low Resource, No Target, Action Used, Disabled, and Blocked. Every capture log must finish with `complete=True`, `failure=None`.
- For v1.98, confirm rule, focused combat-UI, full runtime, and build logs pass with no `Attempting to select ... while already selecting` warning. Retain packaged-player evidence for Selection, passive Preview, armed Targeting, Locked, Low Resource, No Target, Action Used, Disabled, and Blocked at both endpoint resolutions; each log must report the typed state/icon tuple and finish with `complete=True`, `failure=None`. `BuildAndPackageWindows.ps1` must also keep the previous good zip until the candidate clean-extracts, boots the packaged exploration path with the exact version marker, and writes `QA/v1.98.0-release-integrity/release-integrity-manifest.json`.
- For v2.4, RuleSmoke, `SpriteArtRuntimeSmoke`, focused combat-UI smoke, full RuntimeBoot, the Windows build/package, and clean-extracted packaged boot pass from the settled source. The exact `WorldThreatHabitatAtlas`, `WorldNpcCitizenAtlas`, and `PlayerExplorationRoleAtlas` pins and all fifteen accepted art/provenance files named by the two v2.4 handoffs are tracked by exact path despite the local ArtReferences exclusion. Retain the exact build log and `QA/v2.4.0-release-integrity/release-integrity-manifest.json` as release evidence.

## Play Smoke Test

- Launch the packaged build outside Unity.
- Confirm the accepted storm-lit Grand Hearth title/menu appears without a timed startup delay, including its full and Reduced Motion presentation; returning to Tavern must preserve its accepted presentation.
- Confirm build/version text identifies the beta scaffold without covering the accepted title or Tavern art.
- Select New Game, complete Muster, and confirm the party's first spawn is inside the authored Grand Hearth before it departs through the storm doors into Midgaard.
- Confirm the Grand Hearth keeps its stable object IDs and room bounds, starting tile label, cartography table using interior-prop cell 7, blue company road chest using cell 17, and working storm-door/portal flow.
- Toggle Local and Region Map in Midgaard. Confirm the town wall reads as one connected perimeter with coherent straight runs, four corners, restrained structural accents, and clean joins into every gate.
- Confirm city, market, temple, and keep ground each read as one coherent material family rather than a one-cell quilt. Static objects should sit on quiet ground, and the three-band transitions between passable material families must not wash across walls, gates, or thresholds.
- Confirm packed dirt outside the east and west gates reads as one restrained approach surface without alternating light/dark squares or baked axis-aligned bands.
- Walk through the east and west road gates, then bump the north and south gates. West must show wilderness left and town right; East must show town left and wilderness right. Both side gates must place their bastions above and below a clear horizontal road, with no front facade, broad vertical rail, hidden straight-wall cell, or sill crossing the passage. Confirm the side passages are visibly and mechanically open while the north and south thresholds are visibly and mechanically sealed.
- Confirm all four gates keep their intended proportions at both map scales, without a black portal, square atlas background, neighboring-cell bleed, or an oversized structure covering nearby roads.
- Confirm the party marker is a compact four-person formation that remains easy to locate in both Local and Region Map without spanning adjacent tiles.
- Stage a one-member party and confirm its Local and Region Map marker uses the correct Shield, Pike, Bow, Knife, Mender, Ember, Hex, or Ward cell from the dedicated v2.4 sheet. Restore a multi-member party and confirm the established group marker returns.
- Inspect exterior ambience and confirm coordinate-stable citizens remain off the Grand Hearth tutorial lane, current guidance, certified safe roads, rooms, water, hazards, entrances, authored regional-site reservations, and interactable cells while suitable non-safe roads remain eligible. Hover and click them to confirm they never offer Talk, dialogue, or another interaction.
- Without reading the README, follow NEXT to King's Hall. Confirm it gives the exact physical pair for the first step (`W / Up`, `S / Down`, `A / Left`, or `D / Right`) and distance; beside the doors it changes to `E / Space`.
- Compare Local and Region Map while traveling. Confirm the restrained automatic thread begins at the party, the next cell shows the same WASD key as NEXT, a visible destination receives an arrival cue, and an offscreen route receives one continuation chip at the first path exit.
- Enter King's Hall and confirm NEXT retargets King Halvard rather than pointing back to the exterior building. Accept the writ and confirm it immediately advances to the sewer.
- With all three proof bundles ready, enter the merchant hall and confirm NEXT retargets Borin. After the reward, confirm it names Sluice Steps.
- Use Beta Lab when testing combat/casting-heavy releases.
- Press I and confirm the Armory opens with the committed row focused, hovering another row does not change that committed selection, tab/filter changes restore a truthful row focus, reopening returns to the committed row, and Esc closes it.
- Outside combat, press C and confirm the Formula Codex opens directly.
- Hover exploration tiles and confirm region/object/movement hints appear.
- Move into a new exploration region and confirm a region banner or chronicle line appears.
- On a new campaign, confirm the map is 58x46 and all eight regional sites and named junctions are reachable without entering a reserved Midgaard interior footprint.
- Move at least one roaming patrol away from its saved home. Confirm the matching active habitat remains fixed at `HomeX`/`HomeY`, draws beneath the moving threat token, stays off certified safe roads, and changes no encounter, movement, or save behavior. Defeat the patrol and confirm the same home presents the neutral ruined-waystation aftermath cell.
- Chart two outer-road junctions, open Journal, mark each in turn, and confirm Mark/Clear state, the path-aware location readout, and the stronger bounded gold map trail all follow the selected destination in both Local and Region Map.
- Confirm a marked Journal waypoint visibly overrides the automatic story route, then clear it and confirm the Golden Thread immediately resumes the story target.
- Save and load with a waypoint selected, then descend to a new depth and confirm the old-depth waypoint does not leak onto the new map.
- Load a supported existing save and confirm its serialized map dimensions and biome layout are preserved.
- After the sewer reward, confirm Journal tracks Sluice Steps, Dusk Market, Smoke Cave, and Varkh's Hall; follow that route and confirm unrelated prototype routes and scaffolds remain unavailable.
- Enter combat.
- Confirm the opposition panel shows enemy tactic/threat lines.
- Complete one party/enemy round using move, attack, cast, guard, elixir, and wait where possible.
- Let a duration-one field and enemy ritual reach a new round together. Confirm one `ROUND N` summary names both events, board input and enemy AI wait, and the reserved next unit begins exactly once.
- At turn start, verify poison wakes a sleeping unit, ice can stun immediately, web removes movement, fire restores movement after burning web, and lethal automatic damage completes its fall before initiative advances.
- Begin a turn webbed, then remove the web through both fire and ordinary expiry. Move once, Undo Move, confirm the unit returns to its turn-start tile with its full allowance, then move again.
- During both the `ROUND N` gate and a staged impact hold, attempt movement and every player command. Confirm no callback fires, no resource or combat state changes, and no turn advances. Then confirm End Turn still works for the living active party unit in Choose Action or Choose Target when stunned, sleeping, or action-spent.
- Open both Spellbook and Skillbook; confirm Ready, Known, Locked, and All counts agree with their visible cards.
- Navigate each book with mouse, W/S or arrows, Home/End, 1-4, Tab, Enter/Space, and a controller where available. Use Page Up/Page Down or the right stick to scroll overflowing detail text without moving the selected row.
- Hover a card other than the committed selection. Confirm the right pane says `PREVIEW`, its disabled action says `Preview Only`, and its prompt tells the player to click or focus the card. Hover alone must not add another strong selection rail, move the list, arm a power, or spend resources.
- With the pointer parked on a different card, navigate by keyboard/controller. Confirm Preview clears, exactly one committed row owns the strong rail and semantic focus, and no re-entrant EventSystem warning appears. Clicking a previewed row must commit and focus it before any later activation can use it.
- Move between combat commands with both pointer and keyboard/controller. Confirm the prompt, highlighted/focused command, and the command Submit would invoke always name the same action.
- Arm a targeted spell and skill from the detail action. Reopen each book, browse a different card, move, cancel, and reopen; confirm armed and selected states remain distinct and no resource is spent before target confirmation.
- Inspect a low-MP spell, future unlock, action-used combatant, stunned/sleeping combatant, and a power with no legal target; confirm each primary action has an exact readable reason and cannot mutate combat state.
- Hover reachable and blocked movement destinations and confirm the shown route/cost agrees with the executed move. Hover legal, out-of-range, and line-of-sight-blocked attacks and confirm the tile shape, tooltip, target card, and click instruction agree.
- Hover several reachable movement destinations and confirm `safe`, `can hit`, and `can reach` summaries agree with the marked `HIT`/`MOVE` enemies. Confirm unit targets use the target rail without a large board card, while empty terrain still has a useful tooltip.
- Open a cache and confirm the loot comparison panel appears.
- Press 1-6 in combat and confirm each action selection responds.
- Press M, +, and - and confirm SFX preference banners appear.
- Press SFX Test from the UI and confirm click/attack/spell effects are audible.
- Save with F5 and load with F9.
- Confirm SFX and Music controls act independently, zone/combat music crossfades, and sparse exploration ambience pauses under modal screens.
- Speak directly with Kate, Lute, the dock worker, and the scholar. Confirm each world figure shows Talk, agrees with the dialogue portrait/speaker, and owns input; information is free, while Kate/Lute purchases require an explicit enabled choice.

## Visual Checks

- v1.92 evidence is retained under `QA/v1.92.0`: four canonical Local/Region captures, four gate close-ups, one Details-open companion, comparison/contact sheets, a passing deterministic packet, and `manual-visual-signoff.md`.
- v1.93 evidence is retained under `QA/v1.93.0`: the four canonical Local/Region captures, all four 1280x720 gate approaches, two 3200x1800 side-gate inspections, before/after and gate contact sheets, a passing deterministic packet, and the advisory `codex-assisted-visual-review.md`. Human disposition remains required.
- v1.93 contact/book continuation evidence is retained under `QA/v1.93.0-contact-book-continuation`: direct staged contact and dialogue views plus focused Spellbook/Skillbook endpoint and state captures. Each accompanying player log must name its exact contact/cell or book state and finish with `complete=True`, `failure=None`.
- Check 1280x720.
- Check 1920x1080.
- Capture all four `-ashen-gate-smoke east|west|north|south` close-ups and one Region Map view of the complete Midgaard perimeter. Include both endpoint resolutions across the set, and ensure at least one Local and one Region capture show the compact party marker beside ordinary map art.
- Compare wall thickness, corners, tower accents, gate joins, and party-marker footprint across Local and Region Map. No masonry break, black block, landscape-stretched side gate, or wide party lineup is acceptable.
- Compare civic ground at Local and Region scale. Reject checkerboards, obvious per-cell seams, hard rectangular district carpets, moire, noisy static-object footprints, or material feathering that weakens a blocked edge, gate, threshold, wall, or route.
- Capture `explore-compact` (Local Map) and `explore-wide` (Region Map) at both endpoint resolutions, plus one 1280x720 Details-open companion using `-ashen-details-smoke`. Confirm each log names the actual map scale, Details state, target, and guidance text. Build the sanitized local packet with `Tools/NewVisualQaPacket.ps1`; deterministic packet failures block review, while any AI opinion remains optional and advisory.
- At both endpoint resolutions, retain Local and Region Map evidence of a habitat home with its patrol moved away, one eligible ambient citizen with no interaction prompt, the citizen-free Grand Hearth/tutorial route, one correct solo-role marker, and the restored multi-member group marker. Reject atlas bleed, square backplates, route obstruction, citizen impersonation, or a habitat drawn above its mobile token.
- Reject any capture whose decoded PNG dimensions differ from the requested size or whose sampled frame is uniformly black.
- Confirm panels do not overlap.
- Confirm Gold, Supplies, and Elixirs are spelled out and every exploration party row shows readable numeric HP and MP.
- Confirm long current objectives remain readable in both compact and detailed exploration rails without covering the party rows or Details button.
- Confirm the Golden Thread keeps its complete exact input, target, first direction, and distance readable without clipping in compact and Details views, even if the rendered copy wraps.
- Confirm hover previews stay on-screen.
- Confirm sprites, highlights, and turn queue are readable.
- Capture Spellbook and Skillbook at both endpoint resolutions, including a long-list bottom selection, Locked filter, unavailable card, and armed `Resume Targeting` state.
- Retain the v1.94 combat/book packet under `QA/v1.94.0-combat-clarity`: six combat captures plus Ready, Locked, Targeting, and Action Used captures for both books. Every player log must finish with `complete=True`, `failure=None`.
- Retain the v1.95 decision-clarity packet under `QA/v1.95.0-decision-clarity`: projected movement, legal/blocked/area targeting, and representative Ready/Locked book captures at both endpoint resolutions. Every player log must finish with `complete=True`, `failure=None`.
- Retain the v1.96 power-art packet under `QA/v1.96.0-power-books`: representative Ready, long-list, Locked, hover-preview, and armed-targeting captures from both books at the supported endpoint resolutions. Compare Fire Floor, Burn Cover, Ice Slick, Thunder Step, the dark Hex suite, and all three class skill families at actual card size.
- Retain the v1.97 power-state packet under `QA/v1.97.0-power-books`: both books at 1280x720 and 1920x1080, all three martial families, representative refreshed Mend/Ember/Hex/Pact formulas, and direct Preview/Selection/Targeting plus unavailable-state comparisons. Confirm the symbol never replaces its text, hover exit restores the committed detail, and no state change moves the list, spends a resource, or arms a different power.
- Retain the v1.98 one-intent packet under `QA/v1.98.0-one-intent`: direct Selection, passive Preview, Targeting, Locked, Low Resource, No Target, Action Used, Disabled, and Blocked captures, split across Spellbook and Skillbook and both endpoint resolutions. Reject any capture log with a re-entrant selection warning or a staged tuple that disagrees with its requested state.
- Use `-ashen-skills-smoke -ashen-skill-class warrior|rogue|ranger` when a deterministic capture must select a specific martial family; Rogue is staged from the lab's spare martial tester without changing normal party composition.
- Confirm 12-pixel body text, 40-pixel minimum controls, list scrollbar, bounded profile/tactical-note text, four filter counts, legal-target count, and primary action remain legible without clipping.

## Package Checks

- Confirm `AshAndBrimstone.exe` launches from a clean extracted folder.
- Confirm `README_PLAY.txt`, `CHANGELOG.md`, `KNOWN_ISSUES.txt`, and `Docs/` are included.
- Confirm `Tools/NewVisualQaPacket.ps1` is included beside the packaged AI visual-QA guide and schema.
- If tool downloads are being documented, explicitly promote and commit the reviewed manifest as `Docs/TOOL_DOWNLOADS.md`; confirm that tracked document is included. The ignored `outputs/tools/` cache must never override package content.
- Confirm the zip filename matches the package version.
- For v2.4, confirm `world-threat-habitat-atlas-runtime-v2.4.0.png`, `world-npc-citizen-atlas-runtime-v2.4.0.png`, and `player-exploration-role-atlas-runtime-v2.4.0.png` each appear exactly once in `Docs/PACKAGED_ART.txt` and the clean-extracted package, with source, staged, and zipped hashes identical. Confirm all fifteen accepted files are tracked in source while the twelve generated/alpha/prompt/validation companions remain absent from the player package.
- For v1.97, confirm the deterministic packet under `QA/v1.97.0-power-books` passes and the source, staged, and zipped ability, signature-spell, lightning, and power-book-state atlas hashes are identical.
- For v1.98, confirm the deterministic packet under `QA/v1.98.0-one-intent` passes, records `releaseVersion` as `v1.98.0` with a truthful source, and binds the validated capture/log set to the final packaged player.
- For v1.98, confirm `QA/v1.98.0-release-integrity/clean-extract-boot.log` reports the exact `v1.98.0 / save 25` marker, staged exploration boot, `Muster ready`, and requested batch quit; confirm the adjacent schema-v2 manifest records `saveVersion=25`, `developmentPackage=false`, `sourceDirty=false`, the unchanged release commit, `cleanExtractLaunch=true`, player exit code 0, the promoted zip hash, the executable hash, and SHA-256 for all 74 PNGs named by `Docs/PACKAGED_ART.txt`.
- Run final packaging without `-AllowDirtySource`, `-SkipUnityBuild`, or `-SkipCleanPackageSmoke`. The default gate must rebuild the player and reject tracked modifications, untracked release inputs, untracked package-selected art, source/staging art hash differences, or disagreement between `Docs/PACKAGED_ART.txt` and the packaged PNG set.
- Confirm the final source, staged, and zipped gate, player-character, and Midgaard NPC atlas hashes match the v1.93 art-pass records, while source prompts and validation JSON remain source-only.
- Confirm the accepted v1.92 archive remains present with SHA-256 `973A50FBAE9EF786DDD1235C959A80F4AC3DEAD106AF78ED426ED65DEB06BE21`.
- Keep the last known-good zip and evidence until the new candidate has passed, then promote the zip and its two evidence files as one rollback-protected operation.
