using System;
using System.Linq;
using UnityEngine;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        // Opt-in, save-blocked review uses the same showcase and timeline as play.
        private float StageSelectedCombatFeedbackCapture(string[] args, out float phaseEndAt)
        {
            int powerOption = FindCommandLineOption(args, "-ashen-feedback-power");
            string key = powerOption >= 0 && powerOption + 1 < args.Length ? args[powerOption + 1] : "";
            if (!CombatVfxShowcaseRules.TryGet(key, out CombatVfxShowcaseEntry entry))
                throw new InvalidOperationException("Feedback capture requires a supported showcase power: " + key);

            int phaseOption = FindCommandLineOption(args, "-ashen-feedback-phase");
            string phase = phaseOption >= 0 && phaseOption + 1 < args.Length
                ? args[phaseOption + 1].Trim().ToLowerInvariant() : "impact";
            if (phase != "cast" && phase != "travel" && phase != "impact" && phase != "aftermath")
                throw new InvalidOperationException("Unknown feedback capture phase: " + phase);

            StageVisualSmokeCombatState(Array.Empty<string>());
            state.ReducedMotion = FindCommandLineOption(args, "-ashen-feedback-reduced-motion") >= 0;
            betaVfxShowcaseIndex = CombatVfxShowcaseRules.IndexFor(entry.Id);
            betaVfxShowcaseOpen = false;
            CombatImpactProfile profile = entry.Kind == CombatVfxShowcasePowerKind.Formula
                ? CombatImpactRules.ForFormula(GetFormula(entry.Id))
                : CombatImpactRules.ForAbility(AbilityDef(entry.Id));
            CombatPowerAnimationTimeline timeline = CombatPowerAnimationTimelineRules.For(
                entry.Id, entry.StableSeed, CombatImpactRules.VisualIntensity(profile), state.ReducedMotion);

            CombatPowerAnimationPhase samplePhase = phase == "cast" ? CombatPowerAnimationPhase.Cast
                : phase == "travel" ? CombatPowerAnimationPhase.ReleaseTravel
                : phase == "aftermath" ? CombatPowerAnimationPhase.Aftermath : CombatPowerAnimationPhase.Impact;
            if (timeline.PhaseDuration(samplePhase) <= 0f
                || samplePhase == CombatPowerAnimationPhase.ReleaseTravel && !timeline.HasTravel
                || samplePhase == CombatPowerAnimationPhase.Aftermath && !timeline.HasAftermath)
                throw new InvalidOperationException(entry.Id + " has no " + phase + " phase in this motion mode.");

            float start = Time.time;
            ReplayBetaVfxShowcase();
            if (!powerImpactEchoes.Any())
                throw new InvalidOperationException("Feedback showcase did not stage an impact: " + entry.Id);
            float sampleAt = timeline.PhaseStart(samplePhase) + timeline.PhaseDuration(samplePhase) * 0.5f;
            phaseEndAt = start + timeline.PhaseEnd(samplePhase);
            Debug.Log(VersionInfo.ProductName + " selected feedback capture: power=" + entry.Id
                + ", phase=" + phase + ", reducedMotion=" + state.ReducedMotion + ", sample=" + sampleAt.ToString("F3"));
            return start + sampleAt;
        }
    }
}
