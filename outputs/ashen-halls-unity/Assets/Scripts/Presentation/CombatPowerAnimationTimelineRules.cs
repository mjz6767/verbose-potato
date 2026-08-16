using System;

namespace AshenHalls
{
    public enum CombatPowerAnimationSourceKind
    {
        Unknown,
        Formula,
        Ability
    }

    public enum CombatPowerAnimationPhase
    {
        Cast,
        ReleaseTravel,
        Impact,
        Aftermath,
        Complete
    }

    public readonly struct CombatPowerAnimationFrame
    {
        public readonly CombatPowerAnimationSourceKind SourceKind;
        public readonly string PowerKey;
        public readonly int StableSeed;
        public readonly int Intensity;
        public readonly bool Supported;
        public readonly bool ReducedMotion;
        public readonly bool HasTravel;
        public readonly bool HasAftermath;
        public readonly CombatPowerAnimationPhase Phase;
        public readonly float ElapsedSeconds;
        public readonly float PhaseStartAt;
        public readonly float PhaseEndAt;
        public readonly float LocalProgress;

        internal CombatPowerAnimationFrame(
            CombatPowerAnimationTimeline timeline,
            CombatPowerAnimationPhase phase,
            float elapsedSeconds,
            float phaseStartAt,
            float phaseEndAt,
            float localProgress)
        {
            SourceKind = timeline.SourceKind;
            PowerKey = timeline.PowerKey;
            StableSeed = timeline.StableSeed;
            Intensity = timeline.Intensity;
            Supported = timeline.Supported;
            ReducedMotion = timeline.ReducedMotion;
            HasTravel = timeline.HasTravel;
            HasAftermath = timeline.HasAftermath;
            Phase = phase;
            ElapsedSeconds = elapsedSeconds;
            PhaseStartAt = phaseStartAt;
            PhaseEndAt = phaseEndAt;
            LocalProgress = localProgress;
        }

        public bool ShowCast => Supported && Phase == CombatPowerAnimationPhase.Cast;
        public bool ShowReleaseTravel => Supported && Phase == CombatPowerAnimationPhase.ReleaseTravel;
        public bool ShowTravel => ShowReleaseTravel && HasTravel;
        public bool ShowImpact => Supported && Phase == CombatPowerAnimationPhase.Impact;
        public bool ShowAftermath => Supported && Phase == CombatPowerAnimationPhase.Aftermath && HasAftermath;
        public bool StaticImpact => ShowImpact && ReducedMotion;
        public bool IsComplete => Phase == CombatPowerAnimationPhase.Complete;
    }

    public readonly struct CombatPowerAnimationTimeline
    {
        public readonly CombatPowerAnimationSourceKind SourceKind;
        public readonly string PowerKey;
        public readonly int StableSeed;
        public readonly int Intensity;
        public readonly bool Supported;
        public readonly bool ReducedMotion;
        public readonly bool HasTravel;
        public readonly bool HasAftermath;
        public readonly float CastAt;
        public readonly float ReleaseAt;
        public readonly float TravelStartAt;
        public readonly float ImpactAt;
        public readonly float AftermathAt;
        public readonly float CompleteAt;

        internal CombatPowerAnimationTimeline(
            CombatPowerAnimationSourceKind sourceKind,
            string powerKey,
            int stableSeed,
            int intensity,
            bool supported,
            bool reducedMotion,
            bool hasTravel,
            bool hasAftermath,
            float releaseAt,
            float impactAt,
            float aftermathAt,
            float completeAt)
        {
            SourceKind = supported ? sourceKind : CombatPowerAnimationSourceKind.Unknown;
            PowerKey = (powerKey ?? "").Trim();
            StableSeed = CombatPowerAnimationTimelineRules.NormalizeStableSeed(stableSeed);
            Intensity = supported ? Math.Max(1, Math.Min(3, intensity)) : 0;
            Supported = supported;
            ReducedMotion = supported && reducedMotion;
            CastAt = 0f;
            ReleaseAt = supported
                ? CombatPowerAnimationTimelineRules.FiniteNonNegative(releaseAt)
                : 0f;
            TravelStartAt = ReleaseAt;
            ImpactAt = supported
                ? Math.Max(ReleaseAt, CombatPowerAnimationTimelineRules.FiniteNonNegative(impactAt))
                : 0f;
            AftermathAt = supported
                ? Math.Max(ImpactAt, CombatPowerAnimationTimelineRules.FiniteNonNegative(aftermathAt))
                : 0f;
            CompleteAt = supported
                ? Math.Max(AftermathAt, CombatPowerAnimationTimelineRules.FiniteNonNegative(completeAt))
                : 0f;
            HasTravel = supported && !ReducedMotion && hasTravel && ImpactAt > TravelStartAt;
            HasAftermath = supported && !ReducedMotion && hasAftermath && CompleteAt > AftermathAt;
        }

        public bool IsEmpty => !Supported;
        public float DurationSeconds => CompleteAt - CastAt;
        public float CastDuration => ReleaseAt - CastAt;
        public float ReleaseTravelDuration => HasTravel ? ImpactAt - TravelStartAt : 0f;
        public float TravelDuration => ReleaseTravelDuration;
        public float ImpactDuration => AftermathAt - ImpactAt;
        public float AftermathDuration => HasAftermath ? CompleteAt - AftermathAt : 0f;

        public float PhaseStart(CombatPowerAnimationPhase phase)
        {
            switch (phase)
            {
                case CombatPowerAnimationPhase.Cast: return CastAt;
                case CombatPowerAnimationPhase.ReleaseTravel: return TravelStartAt;
                case CombatPowerAnimationPhase.Impact: return ImpactAt;
                case CombatPowerAnimationPhase.Aftermath: return AftermathAt;
                default: return CompleteAt;
            }
        }

        public float PhaseEnd(CombatPowerAnimationPhase phase)
        {
            switch (phase)
            {
                case CombatPowerAnimationPhase.Cast: return ReleaseAt;
                case CombatPowerAnimationPhase.ReleaseTravel: return ImpactAt;
                case CombatPowerAnimationPhase.Impact: return AftermathAt;
                case CombatPowerAnimationPhase.Aftermath: return CompleteAt;
                default: return CompleteAt;
            }
        }

        public float PhaseDuration(CombatPowerAnimationPhase phase)
        {
            return Math.Max(0f, PhaseEnd(phase) - PhaseStart(phase));
        }

        public CombatPowerAnimationFrame FrameAt(float elapsedSeconds)
        {
            return CombatPowerAnimationTimelineRules.Evaluate(this, elapsedSeconds);
        }
    }

    public static class CombatPowerAnimationTimelineRules
    {
        public const float CastAt = 0f;
        public const float DefaultReleaseFraction = 0.42f;
        public const float MinimumAftershockLingerSeconds = 0.08f;
        public const float ReducedMotionImpactHoldSeconds = 0.16f;

        public static CombatPowerAnimationTimeline For(
            string powerKeyOrName,
            int stableSeed,
            int intensity = 0,
            bool reducedMotion = false)
        {
            if (CombatPowerSfxRules.IsSupportedFormula(powerKeyOrName))
            {
                return ForFormula(powerKeyOrName, stableSeed, intensity, reducedMotion);
            }

            if (CombatPowerSfxRules.IsSupportedAbility(powerKeyOrName))
            {
                return ForAbility(powerKeyOrName, stableSeed, intensity, reducedMotion);
            }

            return Empty(powerKeyOrName, stableSeed);
        }

        public static CombatPowerAnimationTimeline ForFormula(
            string formulaCodeOrName,
            int stableSeed,
            int intensity = 0,
            bool reducedMotion = false)
        {
            string key = CombatPowerTravelVfxRules.NormalizeFormulaKey(formulaCodeOrName);
            if (!CombatPowerSfxRules.IsSupportedFormula(key)
                || !CombatPowerTravelVfxRules.IsKnownFormula(key))
            {
                return Empty(formulaCodeOrName, stableSeed);
            }

            return ForFormula(FindFormula(key), stableSeed, intensity, reducedMotion);
        }

        public static CombatPowerAnimationTimeline ForFormula(
            FormulaDef formula,
            int stableSeed,
            int intensity = 0,
            bool reducedMotion = false)
        {
            if (formula == null
                || !CombatPowerSfxRules.IsSupportedFormula(formula.Code)
                || !CombatPowerTravelVfxRules.IsKnownFormula(formula.Code))
            {
                return Empty(formula?.Code, stableSeed);
            }

            string key = CombatPowerTravelVfxRules.NormalizeFormulaKey(formula.Code);
            CombatImpactProfile impact = CombatImpactRules.ForFormula(formula);
            CombatPowerSfxPlan sfx = CombatPowerSfxRules.PlanForFormula(key, intensity);
            CombatPowerTravelVfxProfile travel = CombatPowerTravelVfxRules.ProfileForFormula(key);
            CombatPowerAftermathVfxProfile aftermath = CombatPowerAftermathVfxRules.ProfileForFormula(key);
            return Build(
                CombatPowerAnimationSourceKind.Formula,
                key,
                stableSeed,
                sfx,
                impact,
                travel.HasTravel,
                aftermath.HasAftermath,
                aftermath.DurationSeconds,
                reducedMotion);
        }

        public static CombatPowerAnimationTimeline ForAbility(
            string abilityIdOrName,
            int stableSeed,
            int intensity = 0,
            bool reducedMotion = false)
        {
            string key = CombatPowerTravelVfxRules.NormalizeAbilityKey(abilityIdOrName);
            if (!CombatPowerSfxRules.IsSupportedAbility(key)
                || !CombatPowerTravelVfxRules.IsKnownAbility(key))
            {
                return Empty(abilityIdOrName, stableSeed);
            }

            return ForAbility(AbilityCatalog.For(key), stableSeed, intensity, reducedMotion);
        }

        public static CombatPowerAnimationTimeline ForAbility(
            MartialAbility ability,
            int stableSeed,
            int intensity = 0,
            bool reducedMotion = false)
        {
            if (ability == null
                || !CombatPowerSfxRules.IsSupportedAbility(ability.Id)
                || !CombatPowerTravelVfxRules.IsKnownAbility(ability.Id))
            {
                return Empty(ability?.Id, stableSeed);
            }

            string key = CombatPowerTravelVfxRules.NormalizeAbilityKey(ability.Id);
            CombatImpactProfile impact = CombatImpactRules.ForAbility(ability);
            CombatPowerSfxPlan sfx = CombatPowerSfxRules.PlanForAbility(key, intensity);
            CombatPowerTravelVfxProfile travel = CombatPowerTravelVfxRules.ProfileForAbility(key);
            CombatPowerAftermathVfxProfile aftermath = CombatPowerAftermathVfxRules.ProfileForAbility(key);
            return Build(
                CombatPowerAnimationSourceKind.Ability,
                key,
                stableSeed,
                sfx,
                impact,
                travel.HasTravel,
                aftermath.HasAftermath,
                aftermath.DurationSeconds,
                reducedMotion);
        }

        public static CombatPowerAnimationTimeline Empty(string powerKey = "", int stableSeed = 0)
        {
            return new CombatPowerAnimationTimeline(
                CombatPowerAnimationSourceKind.Unknown,
                powerKey,
                stableSeed,
                0,
                false,
                false,
                false,
                false,
                0f,
                0f,
                0f,
                0f);
        }

        public static CombatPowerAnimationFrame Evaluate(
            CombatPowerAnimationTimeline timeline,
            float elapsedSeconds)
        {
            float elapsed = FiniteElapsed(elapsedSeconds, timeline.CompleteAt);
            if (!timeline.Supported)
            {
                return Frame(
                    timeline,
                    CombatPowerAnimationPhase.Complete,
                    elapsed,
                    timeline.CompleteAt,
                    timeline.CompleteAt,
                    1f);
            }

            if (elapsed < timeline.ReleaseAt)
            {
                return FrameForPhase(timeline, CombatPowerAnimationPhase.Cast, elapsed);
            }

            if (timeline.HasTravel && elapsed < timeline.ImpactAt)
            {
                return FrameForPhase(timeline, CombatPowerAnimationPhase.ReleaseTravel, elapsed);
            }

            if (elapsed < timeline.AftermathAt)
            {
                return FrameForPhase(timeline, CombatPowerAnimationPhase.Impact, elapsed);
            }

            if (timeline.HasAftermath && elapsed < timeline.CompleteAt)
            {
                return FrameForPhase(timeline, CombatPowerAnimationPhase.Aftermath, elapsed);
            }

            return Frame(
                timeline,
                CombatPowerAnimationPhase.Complete,
                elapsed,
                timeline.CompleteAt,
                timeline.CompleteAt,
                1f);
        }

        public static float PhaseProgress(float elapsedSeconds, float phaseStartAt, float phaseEndAt)
        {
            float start = FiniteNonNegative(phaseStartAt);
            float end = Math.Max(start, FiniteNonNegative(phaseEndAt));
            if (end <= start) return 1f;
            float elapsed = FiniteElapsed(elapsedSeconds, end);
            return Clamp01((elapsed - start) / (end - start));
        }

        internal static int NormalizeStableSeed(int stableSeed)
        {
            return stableSeed & int.MaxValue;
        }

        internal static float FiniteNonNegative(float value)
        {
            return float.IsNaN(value) || float.IsInfinity(value) ? 0f : Math.Max(0f, value);
        }

        private static CombatPowerAnimationTimeline Build(
            CombatPowerAnimationSourceKind sourceKind,
            string key,
            int stableSeed,
            CombatPowerSfxPlan sfx,
            CombatImpactProfile impact,
            bool authoredTravel,
            bool authoredAftermath,
            float authoredAftermathDuration,
            bool reducedMotion)
        {
            if (reducedMotion)
            {
                return new CombatPowerAnimationTimeline(
                    sourceKind,
                    key,
                    stableSeed,
                    sfx.Intensity,
                    true,
                    true,
                    false,
                    false,
                    0f,
                    0f,
                    ReducedMotionImpactHoldSeconds,
                    ReducedMotionImpactHoldSeconds);
            }

            float impactAt = sfx.Impact.Enabled
                ? FiniteNonNegative(sfx.Impact.Delay)
                : FiniteNonNegative(impact.ImpactDelay);
            float releaseAt = impactAt;
            if (authoredTravel)
            {
                float authoredRelease = sfx.Release.Enabled
                    ? FiniteNonNegative(sfx.Release.Delay)
                    : impactAt * DefaultReleaseFraction;
                releaseAt = Math.Max(0f, Math.Min(impactAt, authoredRelease));
            }

            float impactFrameEnd = impactAt + FiniteNonNegative(CombatImpactRules.ImpactFrameDuration(impact));
            float aftermathAt = Math.Max(
                impactFrameEnd,
                FiniteNonNegative(CombatImpactRules.AftermathDelay(impact)));
            if (sfx.Aftershock.Enabled)
            {
                aftermathAt = Math.Max(aftermathAt, FiniteNonNegative(sfx.Aftershock.Delay));
            }

            float completeAt;
            if (authoredAftermath && authoredAftermathDuration > 0f)
            {
                completeAt = aftermathAt + FiniteNonNegative(authoredAftermathDuration);
            }
            else
            {
                completeAt = Math.Max(impactFrameEnd, FiniteNonNegative(impact.ResolutionDelay));
                if (sfx.Aftershock.Enabled)
                {
                    completeAt = Math.Max(
                        completeAt,
                        FiniteNonNegative(sfx.Aftershock.Delay) + MinimumAftershockLingerSeconds);
                }
                aftermathAt = completeAt;
            }

            return new CombatPowerAnimationTimeline(
                sourceKind,
                key,
                stableSeed,
                sfx.Intensity,
                true,
                false,
                authoredTravel,
                authoredAftermath,
                releaseAt,
                impactAt,
                aftermathAt,
                completeAt);
        }

        private static FormulaDef FindFormula(string canonicalCode)
        {
            FormulaDef[] formulas = FormulaCatalog.All;
            for (int i = 0; i < formulas.Length; i++)
            {
                FormulaDef formula = formulas[i];
                if (formula != null && string.Equals(formula.Code, canonicalCode, StringComparison.Ordinal))
                {
                    return formula;
                }
            }
            return null;
        }

        private static CombatPowerAnimationFrame FrameForPhase(
            CombatPowerAnimationTimeline timeline,
            CombatPowerAnimationPhase phase,
            float elapsed)
        {
            float start = timeline.PhaseStart(phase);
            float end = timeline.PhaseEnd(phase);
            return Frame(timeline, phase, elapsed, start, end, PhaseProgress(elapsed, start, end));
        }

        private static CombatPowerAnimationFrame Frame(
            CombatPowerAnimationTimeline timeline,
            CombatPowerAnimationPhase phase,
            float elapsed,
            float start,
            float end,
            float localProgress)
        {
            return new CombatPowerAnimationFrame(
                timeline,
                phase,
                elapsed,
                start,
                end,
                Clamp01(localProgress));
        }

        private static float FiniteElapsed(float elapsedSeconds, float positiveInfinityFallback)
        {
            if (float.IsNaN(elapsedSeconds) || float.IsNegativeInfinity(elapsedSeconds)) return 0f;
            if (float.IsPositiveInfinity(elapsedSeconds)) return FiniteNonNegative(positiveInfinityFallback);
            return Math.Max(0f, elapsedSeconds);
        }

        private static float Clamp01(float value)
        {
            return Math.Max(0f, Math.Min(1f, value));
        }
    }
}
