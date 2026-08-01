# Ash & Brimstone Family Playtest Readiness

## Current Candidate

`v1.99.0` is the current family-playtest candidate for the Midgaard sewer and bounded Old Road slice. It rebuilds the bottom combat-command strip around large readable emblems, responsive focusable controls, and separate semantic, focused, unavailable, selected, and genuinely armed states. Cast and Skills show the exact pending formula or ability only while it is armed, and both power books give selected art a larger responsive detail hero. RuleSmoke, focused combat-UI runtime smoke, full RuntimeBoot, the Windows build/package, and clean-extracted packaged boot pass. Six development-player captures at 1280x720 and 1920x1080 also exit cleanly and visually confirm the representative command and book states. Physical-controller review and a complete real playthrough from a clean extraction remain release gates. Combat mechanics and save schema v25 are unchanged. It is not a content-complete game.

## Proven Automatically

- Normal startup reaches the Tavern, New Game reaches Party Setup, and Quick Start reaches exploration.
- Spellbook art retains a stable one-to-one FormulaCatalog mapping across all 49 formulas. The exact v1.97 7 by 7 sheet, 4 by 2 lightning subset, and 4 by 5 skills sheet preserve their established semantic cells, while the 4 by 3 book-state contract maps Selection, Targeting, Locked, Low Resource, No Target, Action Used, Disabled, Blocked, Cost, Reach, Target, and Preview to twelve distinct cells.
- The exact v1.99 command sheet is a transparent 5 by 4, 1280 by 1024 atlas with 256-pixel cells and safe gutters. Move, Attack, Cast, Guard, Elixir, End Turn, and Skills retain stable cells 0, 1, 2, 3, 4, 5, and 7; deterministic checks reject irregular or transposed geometry, missing live-cell coverage, and boundary bleed.
- The default compact exploration rail keeps the next waypoint, objective, nearby sites, and all four party rows visible.
- The persistent Golden Thread names the exact physical movement pair (`W / Up`, `S / Down`, `A / Left`, or `D / Right`) or `E / Space` interaction, target, direction, and distance. The same plan paints a bounded automatic trail, next-step key, destination or edge continuation on Local and Region Map; retargets Halvard and Borin inside isolated interiors; uses reachable exits for outside work; advances with sewer progress; and yields to an explicit Journal waypoint until that mark is cleared.
- The exact v1.93 gate atlas is active beside the retained v1.91 wall and party-token atlases. Automated checks cover atlas dimensions, visible bounds, safe gutters, directional West/East identity, a fully transparent side-gate passage, compact gate footprints at both map scales, wall connection masks, separate narrow vertical foundations, compact party-marker geometry, and the unchanged open-east/open-west plus sealed-north/sealed-south traversal contract.
- The active `world-map-material-atlas-runtime-v1.92.0.png` replaces cells 0-15 with coherent city, market, temple, and keep families and cells 28-31 with matched packed-dirt orientations for gate approaches; gives each family's four variants equal coordinate-deterministic selection; uses a quiet variant beneath static exploration-object footprints; and protects blocked cells, gates, and thresholds from the renderer's three-band material feather. Cells 16-27 and 32-63 remain pixel-identical to v1.68.
- Midgaard's critical NPCs, gates, sewer, armorer, and regional exits are present and reachable.
- Dialogue, Journal, loot comparison, pause, help, spellbook, ability modal, and combat command overlays own input without clicking through to the board.
- Broken Sluice, Foul Runoff, and Cistern Den use authored encounters.
- Each authored sewer encounter publishes a concise first-round plan beneath the initiative strip, then yields to normal combat information.
- Foul Runoff opens a two-item safe-room choice; one item is recorded, added, and equipped once.
- Three first clears yield exactly three proof bundles.
- Borin consumes proof, grants the rat-pelt armor, and opens Sluice Steps toward the playable Dusk Market route. Production Journal tests reject stale teaser language in titles, subtitles, and details.
- Saves use atomic primary/backup writes. Normal play checkpoints at safe milestones; labs and batch tests cannot overwrite a campaign.
- Normal campaign combat can retreat for one supply through a confirmed Menu action. It abandons loot/progress, restores the party at Temple Square, and checkpoints the recovery.
- Uncommitted movement can be undone from the command deck or with U/Backspace. The unit returns to its turn-start tile with its full movement allowance; committing any action locks the move.
- When a start-turn web expires or burns away, movement repair also replaces the stale zero-point Undo Move snapshot. The unit can move, undo to its original tile with its full allowance, and move again.
- Armed spell and skill targets can be canceled from the command deck, Esc, or right-click without spending the action or disturbing movement/undo state.
- Combat now leads with Round, remaining Move, and Action state; party and enemy turns use distinct action wording, the active card summarizes immediate threat, and command subtitles name their legal choice counts.
- Reachable movement hover draws the exact weighted route used by execution. Legal targets use foreground corner brackets, blocked or out-of-range targets use foreground crossed marks, and the tile, hover panel, target card, and click instruction share one legality forecast.
- Reachable movement destinations also report projected danger without moving the actor in simulation. Responsible enemies are marked `HIT` or `MOVE`; unit forecasts stay in the target rail rather than covering their battlefield footprint; and persistent fields reserve a lower-left `nR` countdown with an urgent `1R` state.
- Focused combat controls own navigation and Submit, preventing Enter/Space from also ending the turn or WASD/arrows from also moving the actor. Spellbook and Skillbook fit six complete 720p rows, count only fully visible cards in their scroll readout, and expose unlock level directly on locked rows.
- Spellbook and Skillbook use Ready, Known, Locked, and All views with a wider reading pane, typed legal-target or impact copy, quieter repeated statuses, `Back to Battle [Esc]`, `Resume Targeting`, and controller/Page Up/Page Down detail scrolling.
- Spellbook and Skillbook use a guarded selection-origin arbiter. Hover can preview another card without changing committed memory, scroll, resources, or action state; preview detail has no disappearing secondary activation path; pointer click or navigation commits exactly one focused row; and armed Targeting remains visually independent. Typed state resolution covers Ready Now, Targeting, Locked, Low Resource, No Target, Action Used, Disabled, Blocked, and the unavailable fallback with exact microicon mappings.
- Combat commands keep pointer ownership, EventSystem focus, prompt context, and Submit aligned. Inventory hover is visual-only, controller focus clears stale hover, and opening/reopening or changing tabs, filters, or committed selection restores focus to the committed visible row.
- Combat command controls now scale with the bottom panel and keep larger 56-pixel-or-greater icon wells on supported layouts. Unavailable commands remain focusable so their reason can be read; semantic color remains on the command, keyboard/controller focus adds a separate ivory outline, and gold commitment appears only for a genuinely armed action rather than hover, focus, or a no-target Attack.
- Cast and Skills retain stable category art until a real formula or ability is pending, then show that exact power art in the command strip. Canceling targeting restores the category emblem instead of leaving stale power art, and Skills no longer defaults to Whirlwind. Spellbook and Skillbook detail panes now give the selected power a larger responsive hero icon with a restrained category halo.
- One central player-command gate rejects resolving round and impact holds plus dead, inactive, and non-party actors before callbacks or resource/state mutation. End Turn remains valid for the living active party unit in Choose Action or Choose Target even when stunned, sleeping, or action-spent.
- Fresh and loaded maps certify that all eight named outer-road junctions remain reachable from Midgaard.
- The v1.99 source, player, and editor assemblies compile directly from Unity's generated response files. Unity RuleSmoke, focused combat-UI runtime smoke, full RuntimeBoot, the Windows build/package, and clean-extracted packaged exploration boot also pass in Unity 6000.3.18f1.
- The v1.99 rule smoke, focused combat-UI smoke, runtime boot smoke, and Windows build/package are the latest complete Unity 6000.3.18f1 gate set, with no `Attempting to select ... while already selecting` warning. Runtime coverage exercises passive preview and row commit in both books, mouse/controller handoff in the command deck, committed-row restoration in Inventory, all 49 production formula art cells, all 18 live skill art cells, and the typed power-state precedence/icon table.
- The Windows packaging tool retains the prior good zip until the candidate zip clean-extracts, boots the exact packaged v1.99 exploration path in batch mode, exits successfully, and writes a release-integrity manifest with the committed release-source fingerprint plus package, executable, and every `Docs/PACKAGED_ART.txt` PNG SHA-256 hash. Final packaging rejects dirty release inputs, untracked package-selected art, source/staging hash drift, and packaged-art-manifest drift.
- Direct v1.98 packaged-player evidence covers Selection, passive Preview, and armed Targeting in both books plus Locked, Low Resource, No Target, Action Used, Disabled, and Blocked across 1280x720 and 1920x1080. All twelve capture logs report `complete=True`, `failure=None`, exact typed state/icon tuples, and no re-entrant selection warning. The deterministic packet passes under `QA/v1.98.0-one-intent` with capture-set SHA-256 `ff8e2a3d83ffddbf2bb935e54a5206d9c14415cf3068f3a069e8e7bfe72822a9`.
- Direct v1.97 packaged-player evidence covers Ember, Mend, Hex, Pact, Warrior, Rogue, and Ranger books at 1280x720 and 1920x1080, including hover-preview, armed-targeting, long-list, and locked states. All thirteen capture logs report `complete=True`, `failure=None`; the deterministic packet passes under `QA/v1.97.0-power-books`, and all four source, staged, and zipped atlas hashes match.
- Direct v1.96 packaged-player evidence covers Ready, long-list, Locked, hover-preview, and armed-targeting states for both Spellbook and Skillbook at 1280x720 and 1920x1080, plus dedicated Rogue and Ranger long-list views at real card size. All twelve capture logs report `complete=True` with `failure=None`; evidence is retained under `QA/v1.96.0-power-books`.
- Direct v1.95 player evidence covers projected move danger at 1280x720 and 1920x1080, unobscured legal and line-of-sight-blocked attacks, an armed Fireball area footprint, the six-row Ready view at 1280x720, and locked Spellbook/Skillbook unlock metadata at 1920x1080. All nine capture logs report `complete=True` with `failure=None`; evidence is retained under `QA/v1.95.0-decision-clarity`.
- The focused combat-UI smoke also passes. Direct v1.94 packaged-player evidence covers combat overview, movement route at 1280x720 and 1920x1080, legal and line-of-sight-blocked attacks, armed Fireball area targeting, and Ready/Locked/Targeting/Action Used states for both books. All fourteen capture logs report `complete=True` with `failure=None`; evidence is retained under `QA/v1.94.0-combat-clarity`.
- Direct v1.93 in-player evidence covers Local and Region Map at 1280x720 and 1920x1080, all four gate approaches at 1280x720, and east/west gate close-ups at 3200x1800. Every capture log reports `complete=True` with `failure=None`; the deterministic packet, before/after gate comparison, and pending-human-disposition Codex review are retained under `QA/v1.93.0`.
- The accepted v1.92 ground-material packet, its nine captures, manual signoff, and archive remain retained unchanged under `QA/v1.92.0` and `outputs/AshAndBrimstone-Windows-v1.92.0.zip`.

## Still Needs Validation and Humans

- Repeat the six-state v1.99 combat/book visual matrix in the clean-extracted packaged build. The development-player review is clean, while an equivalent packaged-player matrix remains manual evidence to collect.
- Walk through West from wilderness-left to town-right and East from town-left to wilderness-right, then bump the north and south gates and confirm their visible state agrees with their passability.
- A real new player still needs to confirm that the Golden Thread is sufficient without reading the README.
- All three fights need balance feedback from players who do not know the systems.
- Spell, skill, terrain, and enemy-turn pacing need subjective feel checks.
- A physical controller still needs a human pass for book row navigation, right-stick detail scrolling, targeting resume/cancel, and comfort at both endpoint resolutions.
- The v1.99 interaction contract still needs a physical mouse/controller handoff check: with the pointer parked over one card or combat command, D-pad/stick movement must leave exactly one truthful focus and prompt. Preview must remain quieter than committed Selection, armed Targeting must remain distinct, unavailable commands must remain focusable for explanation, and Low Resource, No Target, Action Used, Disabled, and Blocked must remain understandable without color.
- The safe-room choice, defeat retry, and final reward need visual confirmation in the packaged Windows player.
- One complete real playthrough from a clean extracted v1.99 folder remains the release gate before calling this broadly shareable; the automated v1.99 clean-extract exploration boot passes independently.

## 15-Minute Family Script

1. Unzip the package and launch `AshAndBrimstone.exe`.
2. Select New Game, inspect Party Setup, then use Quick Start. Do not read the README; follow the persistent NEXT instruction.
3. Enter King's Hall, accept Halvard's writ, and follow NEXT to Broken Sluice.
4. Use Move, Attack or Shoot, one Skill, one Spell, Guard, Elixir when available, and End Turn. Confirm Cast/Skills show category art before arming and the exact power art while targeting.
5. Clear Foul Runoff and choose one recovery-room item.
6. Enter Cistern Den and decide whether the final room feels fair.
7. Return to Borin, claim the reward, and open the Journal.
8. In a second run, deliberately lose a fight and test Tavern > Continue.

## Feedback Questions

- At any point, were you unsure what to do next?
- Did NEXT always name the physical place or person you expected, with an input you understood?
- Which combat command or spell was confusing?
- Did enemy turns feel too slow, too fast, unfair, or trivial?
- Did the safe-room choice feel meaningful?
- Could you read sprites, hazards, targeting, and results without squinting?
- Would you voluntarily play one more route after the teaser?
