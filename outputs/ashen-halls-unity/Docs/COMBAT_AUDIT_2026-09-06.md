# v2.24 Battle Discipline combat audit

## Scope

Three Astra reviews covered status/damage resolution, combat input ownership, and saved turn state/timers. This release also incorporates the previously verified September 4-5 improvements to settings, backup recovery, Party Setup, pause/help layout, movement validation, and life-drain reporting. Campaign save schema remains 27.

## Corrected defects

- **One-turn pins expired before they restricted movement.** Webbed was decremented at turn start and movement repair immediately restored the budget. It now decrements at the affected unit's completed turn, including a turn skipped by stun/sleep. Attacks remain available while pinned, and fire still frees the unit immediately.
- **Steam always stunned friendly units.** The environmental reaction treated allies as a beneficial application and skipped its chance/resistance roll. Harmful steam now uses the same chance/resistance path for caster, ally, and enemy.
- **Enemy actions and pending turns could continue behind Help.** The timer resolver now checks modal input ownership before consuming an enemy schedule or completing a pending transition. Closing the modal resumes exactly once.
- **Duplicated saved initiative could repeat an actor forever.** Candidate validation rejects duplicate nonempty queue entries so the healthy backup can load. Missing legacy queues still use established repair.
- **Controller Cancel had two handlers.** The power book and global Cancel could both consume one button press, closing the book and then canceling targeting or opening Pause. Global Cancel now owns that input.
- **Resume Targeting could immediately cast.** Resuming an already armed power now renews the cursor's activation frame so the same submit cannot also confirm the target. The selected power and target remain intact.
- **Drain terrain behavior disagreed with its preview.** Drain now uses the same hit-terrain reaction as other hostile formulas, so its advertised sanctuary interaction resolves on the actual target.

## Regression coverage

`CombatStatusLifecycleSmoke`, `CombatResumeSmoke`, and `CombatInputSmoke` are integrated into RuleSmoke and therefore the Windows build gate. They exercise actual runtime status/turn/action handlers with deterministic random values and timestamps, including both sides, one-/two-turn pins, skipped turns, fire clearing, steam resistance, blocked/resumed timers, backup recovery, book cancellation, and separate target confirmation. Existing drain, persistence, inventory/loot, sprite, combat UI, and full RuntimeBoot gates remain enabled.

## Release evidence

The full Unity audit and all five embedded Windows build gates passed. The canonical packaging workflow built clean source commit `a12cb8babc90e9190301d23c2f68fb27ec88eb92`, checked all 92 selected art PNGs, and booted a clean extraction with exit 0. The 229,972,808-byte archive has SHA-256 `ab6cb675f31bb5abbaf51c089c2697e44c0950888e2bdf178ceecb64b7ea63cf`.

The summary in `Docs/ReleaseEvidence/v2.24.0-summary.json` records those passes and their local evidence hashes. Its documentation-only evidence commit follows the immutable package build, as in prior releases. The first final-player screenshot was correctly rejected as uniformly black with the window hidden. Visible-window approval was requested; final-player visual and live keyboard/pointer review remain pending and are not claimed as passes.

Physical-controller feel, subjective sound/Reduced Motion assessment, and a complete human Chapter I-V playthrough remain manual checks. The focused automated and keyboard/pointer checks do not claim those broader experiences.
