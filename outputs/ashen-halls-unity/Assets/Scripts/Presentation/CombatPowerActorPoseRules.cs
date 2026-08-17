using System;

namespace AshenHalls
{
    public enum CombatPowerActorPosePhase
    {
        None,
        CastWindup,
        Release,
        Recovery,
        Dash,
        TeleportOut,
        TeleportIn,
        SummonReveal,
        MorphOut,
        MorphIn,
        TargetHit,
        Complete
    }

    public enum CombatPowerActorPoseRole
    {
        Source,
        Landing,
        Target
    }

    public enum CombatPowerActorChoreographyKind
    {
        Cast,
        Dash,
        Teleport,
        Summon,
        Morph
    }

    public readonly struct CombatPowerActorPoseProfile
    {
        public readonly CombatPowerAnimationSourceKind SourceKind;
        public readonly string PowerKey;
        public readonly CombatPowerActorChoreographyKind Choreography;
        public readonly bool HasTargetHit;
        public readonly bool BeneficialTarget;
        public readonly float Energy;
        public readonly float ReleaseDurationSeconds;
        public readonly float RecoveryDurationSeconds;
        public readonly float TargetHitDurationSeconds;

        public CombatPowerActorPoseProfile(
            CombatPowerAnimationSourceKind sourceKind,
            string powerKey,
            CombatPowerActorChoreographyKind choreography,
            bool hasTargetHit,
            bool beneficialTarget,
            float energy,
            float releaseDurationSeconds,
            float recoveryDurationSeconds,
            float targetHitDurationSeconds)
        {
            SourceKind = sourceKind;
            PowerKey = (powerKey ?? "").Trim().ToLowerInvariant();
            Choreography = choreography;
            HasTargetHit = sourceKind != CombatPowerAnimationSourceKind.Unknown && hasTargetHit;
            BeneficialTarget = HasTargetHit && beneficialTarget;
            Energy = SupportedKey(SourceKind, PowerKey)
                ? Clamp(energy, CombatPowerActorPoseRules.MinimumEnergy, CombatPowerActorPoseRules.MaximumEnergy)
                : 0f;
            ReleaseDurationSeconds = Energy > 0f
                ? Clamp(
                    releaseDurationSeconds,
                    CombatPowerActorPoseRules.MinimumReleaseDurationSeconds,
                    CombatPowerActorPoseRules.MaximumReleaseDurationSeconds)
                : 0f;
            RecoveryDurationSeconds = Energy > 0f
                ? Clamp(
                    recoveryDurationSeconds,
                    CombatPowerActorPoseRules.MinimumRecoveryDurationSeconds,
                    CombatPowerActorPoseRules.MaximumRecoveryDurationSeconds)
                : 0f;
            TargetHitDurationSeconds = HasTargetHit
                ? Clamp(
                    targetHitDurationSeconds,
                    CombatPowerActorPoseRules.MinimumTargetHitDurationSeconds,
                    CombatPowerActorPoseRules.MaximumTargetHitDurationSeconds)
                : 0f;
        }

        public bool Supported => SupportedKey(SourceKind, PowerKey) && Energy > 0f;
        public bool HasMovement =>
            Choreography == CombatPowerActorChoreographyKind.Dash ||
            Choreography == CombatPowerActorChoreographyKind.Teleport;
        public bool HasLandingReveal => Choreography == CombatPowerActorChoreographyKind.Summon;
        public bool HasMorph => Choreography == CombatPowerActorChoreographyKind.Morph;

        private static bool SupportedKey(CombatPowerAnimationSourceKind sourceKind, string key)
        {
            return sourceKind != CombatPowerAnimationSourceKind.Unknown && !string.IsNullOrEmpty(key);
        }

        private static float Clamp(float value, float minimum, float maximum)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) return minimum;
            return Math.Max(minimum, Math.Min(maximum, value));
        }
    }

    public readonly struct CombatPowerActorPoseFrame
    {
        public readonly CombatPowerAnimationSourceKind SourceKind;
        public readonly string PowerKey;
        public readonly int StableSeed;
        public readonly int Intensity;
        public readonly bool Supported;
        public readonly bool ReducedMotion;
        public readonly CombatPowerActorPoseRole Role;
        public readonly CombatPowerActorPosePhase Phase;
        public readonly float ElapsedSeconds;
        public readonly float PhaseStartAt;
        public readonly float PhaseEndAt;
        public readonly float LocalProgress;
        public readonly int SourceX;
        public readonly int SourceY;
        public readonly int LandingX;
        public readonly int LandingY;
        public readonly float PositionX;
        public readonly float PositionY;
        public readonly float OffsetX;
        public readonly float OffsetY;
        public readonly float Scale;
        public readonly float Opacity;

        internal CombatPowerActorPoseFrame(
            CombatPowerActorPosePlan plan,
            CombatPowerActorPoseRole role,
            CombatPowerActorPosePhase phase,
            float elapsedSeconds,
            float phaseStartAt,
            float phaseEndAt,
            float localProgress,
            float positionX,
            float positionY,
            float offsetX,
            float offsetY,
            float scale,
            float opacity)
        {
            SourceKind = plan.SourceKind;
            PowerKey = plan.PowerKey;
            StableSeed = plan.StableSeed;
            Intensity = plan.Intensity;
            Supported = plan.Supported;
            ReducedMotion = plan.ReducedMotion;
            Role = role;
            Phase = phase;
            ElapsedSeconds = CombatPowerActorPoseRules.FiniteElapsed(elapsedSeconds, plan.DurationSeconds);
            PhaseStartAt = CombatPowerActorPoseRules.FiniteNonNegative(phaseStartAt);
            PhaseEndAt = Math.Max(PhaseStartAt, CombatPowerActorPoseRules.FiniteNonNegative(phaseEndAt));
            LocalProgress = CombatPowerActorPoseRules.Clamp01(localProgress);
            SourceX = plan.SourceX;
            SourceY = plan.SourceY;
            LandingX = plan.LandingX;
            LandingY = plan.LandingY;
            PositionX = CombatPowerActorPoseRules.Finite(positionX, role == CombatPowerActorPoseRole.Source ? SourceX : LandingX);
            PositionY = CombatPowerActorPoseRules.Finite(positionY, role == CombatPowerActorPoseRole.Source ? SourceY : LandingY);
            OffsetX = CombatPowerActorPoseRules.Clamp(
                CombatPowerActorPoseRules.Finite(offsetX),
                -CombatPowerActorPoseRules.MaximumOffset,
                CombatPowerActorPoseRules.MaximumOffset);
            OffsetY = CombatPowerActorPoseRules.Clamp(
                CombatPowerActorPoseRules.Finite(offsetY),
                -CombatPowerActorPoseRules.MaximumOffset,
                CombatPowerActorPoseRules.MaximumOffset);
            Scale = CombatPowerActorPoseRules.Clamp(
                CombatPowerActorPoseRules.Finite(scale, 1f),
                CombatPowerActorPoseRules.MinimumScale,
                CombatPowerActorPoseRules.MaximumScale);
            Opacity = CombatPowerActorPoseRules.Clamp01(CombatPowerActorPoseRules.Finite(opacity, 1f));
        }

        public bool HasPose => Supported && Phase != CombatPowerActorPosePhase.Complete;
        public bool IsVisible => Supported && Opacity > 0f && Phase != CombatPowerActorPosePhase.Complete;
        public bool IsComplete => Phase == CombatPowerActorPosePhase.Complete;
        public bool IsMoving =>
            Phase == CombatPowerActorPosePhase.Dash ||
            Phase == CombatPowerActorPosePhase.TeleportOut ||
            Phase == CombatPowerActorPosePhase.TeleportIn;
        public bool IsArrival =>
            Phase == CombatPowerActorPosePhase.TeleportIn ||
            Phase == CombatPowerActorPosePhase.SummonReveal ||
            Phase == CombatPowerActorPosePhase.MorphIn;
        public bool IsStaticFallback => ReducedMotion && HasPose;
    }

    public readonly struct CombatPowerActorPosePlan
    {
        public readonly CombatPowerAnimationSourceKind SourceKind;
        public readonly string PowerKey;
        public readonly CombatPowerActorChoreographyKind Choreography;
        public readonly int StableSeed;
        public readonly int Intensity;
        public readonly bool Supported;
        public readonly bool ReducedMotion;
        public readonly bool HasTargetHit;
        public readonly bool BeneficialTarget;
        public readonly int SourceX;
        public readonly int SourceY;
        public readonly int LandingX;
        public readonly int LandingY;
        public readonly float ReleaseAt;
        public readonly float ImpactAt;
        public readonly float CastWindupEndAt;
        public readonly float ReleaseEndAt;
        public readonly float TeleportSplitAt;
        public readonly float MorphOutStartAt;
        public readonly float MorphInEndAt;
        public readonly float SummonRevealEndAt;
        public readonly float RecoveryStartAt;
        public readonly float RecoveryEndAt;
        public readonly float TargetHitEndAt;
        public readonly float DurationSeconds;
        internal readonly float Energy;

        internal CombatPowerActorPosePlan(
            CombatPowerActorPoseProfile profile,
            CombatPowerAnimationTimeline timeline,
            int sourceX,
            int sourceY,
            int landingX,
            int landingY)
        {
            Supported = profile.Supported && timeline.Supported;
            SourceKind = Supported ? timeline.SourceKind : CombatPowerAnimationSourceKind.Unknown;
            PowerKey = Supported ? timeline.PowerKey : (profile.PowerKey ?? "");
            Choreography = Supported ? profile.Choreography : CombatPowerActorChoreographyKind.Cast;
            StableSeed = timeline.StableSeed;
            Intensity = Supported ? Math.Max(1, Math.Min(3, timeline.Intensity)) : 0;
            ReducedMotion = Supported && timeline.ReducedMotion;
            HasTargetHit = Supported && profile.HasTargetHit;
            BeneficialTarget = HasTargetHit && profile.BeneficialTarget;
            SourceX = sourceX;
            SourceY = sourceY;
            LandingX = landingX;
            LandingY = landingY;
            Energy = Supported
                ? CombatPowerActorPoseRules.Clamp(
                    profile.Energy * (1f + (Intensity - 1) * 0.08f),
                    CombatPowerActorPoseRules.MinimumEnergy,
                    CombatPowerActorPoseRules.MaximumPlanEnergy)
                : 0f;

            ReleaseAt = Supported ? CombatPowerActorPoseRules.FiniteNonNegative(timeline.ReleaseAt) : 0f;
            ImpactAt = Supported
                ? Math.Max(ReleaseAt, CombatPowerActorPoseRules.FiniteNonNegative(timeline.ImpactAt))
                : 0f;

            float releaseDuration = Supported
                ? CombatPowerActorPoseRules.Clamp(
                    profile.ReleaseDurationSeconds * (1f + (Intensity - 1) * 0.06f),
                    CombatPowerActorPoseRules.MinimumReleaseDurationSeconds,
                    CombatPowerActorPoseRules.MaximumReleaseDurationSeconds)
                : 0f;
            float recoveryDuration = Supported
                ? CombatPowerActorPoseRules.Clamp(
                    profile.RecoveryDurationSeconds * (1f + (Intensity - 1) * 0.05f),
                    CombatPowerActorPoseRules.MinimumRecoveryDurationSeconds,
                    CombatPowerActorPoseRules.MaximumRecoveryDurationSeconds)
                : 0f;

            CastWindupEndAt = ReleaseAt;
            ReleaseEndAt = ReleaseAt + releaseDuration;
            TeleportSplitAt = ReleaseAt;
            MorphOutStartAt = ImpactAt;
            MorphInEndAt = ImpactAt;
            SummonRevealEndAt = ImpactAt;
            RecoveryStartAt = ReleaseEndAt;

            if (Supported && !ReducedMotion)
            {
                switch (Choreography)
                {
                    case CombatPowerActorChoreographyKind.Dash:
                        ReleaseEndAt = ReleaseAt;
                        RecoveryStartAt = ImpactAt;
                        break;
                    case CombatPowerActorChoreographyKind.Teleport:
                        ReleaseEndAt = ReleaseAt;
                        TeleportSplitAt = ReleaseAt + (ImpactAt - ReleaseAt) * CombatPowerActorPoseRules.TeleportOutFraction;
                        RecoveryStartAt = ImpactAt;
                        break;
                    case CombatPowerActorChoreographyKind.Summon:
                        SummonRevealEndAt = ImpactAt + CombatPowerActorPoseRules.SummonRevealDuration(Energy);
                        break;
                    case CombatPowerActorChoreographyKind.Morph:
                        MorphOutStartAt = Math.Max(0f, ImpactAt - CombatPowerActorPoseRules.MorphOutDuration(Energy));
                        CastWindupEndAt = MorphOutStartAt;
                        ReleaseEndAt = ImpactAt;
                        MorphInEndAt = ImpactAt + CombatPowerActorPoseRules.MorphInDuration(Energy);
                        RecoveryStartAt = MorphInEndAt;
                        break;
                }
            }
            else if (Supported && ReducedMotion)
            {
                if (Choreography == CombatPowerActorChoreographyKind.Summon)
                {
                    SummonRevealEndAt = ImpactAt + CombatPowerAnimationTimelineRules.ReducedMotionImpactHoldSeconds;
                }
                else if (Choreography == CombatPowerActorChoreographyKind.Morph)
                {
                    MorphInEndAt = ImpactAt + CombatPowerAnimationTimelineRules.ReducedMotionImpactHoldSeconds;
                }
            }

            RecoveryEndAt = Supported
                ? RecoveryStartAt + (ReducedMotion ? 0f : recoveryDuration)
                : 0f;
            TargetHitEndAt = HasTargetHit
                ? ImpactAt + (ReducedMotion
                    ? CombatPowerAnimationTimelineRules.ReducedMotionImpactHoldSeconds
                    : profile.TargetHitDurationSeconds)
                : ImpactAt;

            float duration = Supported ? CombatPowerActorPoseRules.FiniteNonNegative(timeline.CompleteAt) : 0f;
            duration = Math.Max(duration, RecoveryEndAt);
            duration = Math.Max(duration, TargetHitEndAt);
            duration = Math.Max(duration, SummonRevealEndAt);
            duration = Math.Max(duration, MorphInEndAt);
            if (ReducedMotion)
            {
                duration = Math.Max(duration, CombatPowerAnimationTimelineRules.ReducedMotionImpactHoldSeconds);
            }
            DurationSeconds = duration;
        }

        public bool IsEmpty => !Supported;
        public bool HasMovement =>
            Supported &&
            (Choreography == CombatPowerActorChoreographyKind.Dash ||
             Choreography == CombatPowerActorChoreographyKind.Teleport);
        public bool HasLandingReveal => Supported && Choreography == CombatPowerActorChoreographyKind.Summon;
        public bool HasMorph => Supported && Choreography == CombatPowerActorChoreographyKind.Morph;

        public CombatPowerActorPoseFrame FrameAt(
            CombatPowerActorPoseRole role,
            float elapsedSeconds)
        {
            return CombatPowerActorPoseRules.Evaluate(this, role, elapsedSeconds);
        }

        public CombatPowerActorPoseFrame SourceFrameAt(float elapsedSeconds)
        {
            return CombatPowerActorPoseRules.EvaluateSource(this, elapsedSeconds);
        }

        public CombatPowerActorPoseFrame LandingFrameAt(float elapsedSeconds)
        {
            return CombatPowerActorPoseRules.EvaluateLanding(this, elapsedSeconds);
        }

        public CombatPowerActorPoseFrame TargetFrameAt(float elapsedSeconds)
        {
            return CombatPowerActorPoseRules.EvaluateTarget(this, elapsedSeconds);
        }
    }

    public static class CombatPowerActorPoseRules
    {
        public const float MinimumEnergy = 0.72f;
        public const float MaximumEnergy = 1.48f;
        public const float MaximumPlanEnergy = 1.64f;
        public const float MinimumReleaseDurationSeconds = 0.06f;
        public const float MaximumReleaseDurationSeconds = 0.18f;
        public const float MinimumRecoveryDurationSeconds = 0.12f;
        public const float MaximumRecoveryDurationSeconds = 0.36f;
        public const float MinimumTargetHitDurationSeconds = 0.12f;
        public const float MaximumTargetHitDurationSeconds = 0.28f;
        public const float MaximumOffset = 0.22f;
        public const float MinimumScale = 0.68f;
        public const float MaximumScale = 1.18f;
        public const float TeleportOutFraction = 0.52f;

        public static CombatPowerActorPosePlan For(
            string powerKeyOrName,
            int stableSeed,
            int sourceX,
            int sourceY,
            int landingX,
            int landingY,
            int intensity = 0,
            bool reducedMotion = false)
        {
            if (CombatPowerSfxRules.IsSupportedFormula(powerKeyOrName))
            {
                return ForFormula(
                    powerKeyOrName,
                    stableSeed,
                    sourceX,
                    sourceY,
                    landingX,
                    landingY,
                    intensity,
                    reducedMotion);
            }

            if (CombatPowerSfxRules.IsSupportedAbility(powerKeyOrName))
            {
                return ForAbility(
                    powerKeyOrName,
                    stableSeed,
                    sourceX,
                    sourceY,
                    landingX,
                    landingY,
                    intensity,
                    reducedMotion);
            }

            return Empty(powerKeyOrName, stableSeed, sourceX, sourceY, landingX, landingY);
        }

        public static CombatPowerActorPosePlan ForFormula(
            string formulaCodeOrName,
            int stableSeed,
            int sourceX,
            int sourceY,
            int landingX,
            int landingY,
            int intensity = 0,
            bool reducedMotion = false)
        {
            CombatPowerActorPoseProfile profile = ProfileForFormula(formulaCodeOrName);
            CombatPowerAnimationTimeline timeline = CombatPowerAnimationTimelineRules.ForFormula(
                profile.PowerKey,
                stableSeed,
                intensity,
                reducedMotion);
            return new CombatPowerActorPosePlan(profile, timeline, sourceX, sourceY, landingX, landingY);
        }

        public static CombatPowerActorPosePlan ForFormula(
            FormulaDef formula,
            int stableSeed,
            int sourceX,
            int sourceY,
            int landingX,
            int landingY,
            int intensity = 0,
            bool reducedMotion = false)
        {
            return ForFormula(
                formula?.Code,
                stableSeed,
                sourceX,
                sourceY,
                landingX,
                landingY,
                intensity,
                reducedMotion);
        }

        public static CombatPowerActorPosePlan ForAbility(
            string abilityIdOrName,
            int stableSeed,
            int sourceX,
            int sourceY,
            int landingX,
            int landingY,
            int intensity = 0,
            bool reducedMotion = false)
        {
            CombatPowerActorPoseProfile profile = ProfileForAbility(abilityIdOrName);
            CombatPowerAnimationTimeline timeline = CombatPowerAnimationTimelineRules.ForAbility(
                profile.PowerKey,
                stableSeed,
                intensity,
                reducedMotion);
            return new CombatPowerActorPosePlan(profile, timeline, sourceX, sourceY, landingX, landingY);
        }

        public static CombatPowerActorPosePlan ForAbility(
            MartialAbility ability,
            int stableSeed,
            int sourceX,
            int sourceY,
            int landingX,
            int landingY,
            int intensity = 0,
            bool reducedMotion = false)
        {
            return ForAbility(
                ability?.Id,
                stableSeed,
                sourceX,
                sourceY,
                landingX,
                landingY,
                intensity,
                reducedMotion);
        }

        public static CombatPowerActorPoseProfile ProfileFor(string powerKeyOrName)
        {
            if (CombatPowerSfxRules.IsSupportedFormula(powerKeyOrName)) return ProfileForFormula(powerKeyOrName);
            if (CombatPowerSfxRules.IsSupportedAbility(powerKeyOrName)) return ProfileForAbility(powerKeyOrName);
            return EmptyProfile(powerKeyOrName);
        }

        public static CombatPowerActorPoseProfile ProfileForFormula(string formulaCodeOrName)
        {
            string key = CombatPowerTravelVfxRules.NormalizeFormulaKey(formulaCodeOrName);
            if (!CombatPowerSfxRules.IsSupportedFormula(key) || !CombatPowerTravelVfxRules.IsKnownFormula(key))
            {
                return EmptyProfile(key);
            }

            CombatPowerActorChoreographyKind choreography = CombatPowerActorChoreographyKind.Cast;
            switch (key)
            {
                case "VST":
                case "VRS":
                    choreography = CombatPowerActorChoreographyKind.Teleport;
                    break;
                case "IBD":
                case "IBF":
                case "IBG":
                    choreography = CombatPowerActorChoreographyKind.Summon;
                    break;
                case "DFA":
                    choreography = CombatPowerActorChoreographyKind.Morph;
                    break;
            }

            bool beneficial = IsBeneficialFormulaTarget(key);
            bool targetHit = beneficial || IsHostileFormulaTarget(key);
            float energy = FormulaEnergy(key);
            return Profile(
                CombatPowerAnimationSourceKind.Formula,
                key,
                choreography,
                targetHit,
                beneficial,
                energy,
                0.105f,
                0.22f,
                beneficial ? 0.22f : 0.18f);
        }

        public static CombatPowerActorPoseProfile ProfileForAbility(string abilityIdOrName)
        {
            string key = CombatPowerTravelVfxRules.NormalizeAbilityKey(abilityIdOrName);
            if (!CombatPowerSfxRules.IsSupportedAbility(key) || !CombatPowerTravelVfxRules.IsKnownAbility(key))
            {
                return EmptyProfile(key);
            }

            CombatPowerActorChoreographyKind choreography = CombatPowerActorChoreographyKind.Cast;
            switch (key)
            {
                case "charge":
                    choreography = CombatPowerActorChoreographyKind.Dash;
                    break;
                case "shadowstep":
                case "riftpounce":
                    choreography = CombatPowerActorChoreographyKind.Teleport;
                    break;
            }

            MartialAbility ability = AbilityCatalog.For(key);
            bool targetHit = ability != null && ability.Targeted;
            return Profile(
                CombatPowerAnimationSourceKind.Ability,
                key,
                choreography,
                targetHit,
                false,
                AbilityEnergy(key),
                0.085f,
                0.19f,
                0.17f);
        }

        public static CombatPowerActorPosePlan Empty(
            string powerKey = "",
            int stableSeed = 0,
            int sourceX = 0,
            int sourceY = 0,
            int landingX = 0,
            int landingY = 0)
        {
            CombatPowerActorPoseProfile profile = EmptyProfile(powerKey);
            CombatPowerAnimationTimeline timeline = CombatPowerAnimationTimelineRules.Empty(powerKey, stableSeed);
            return new CombatPowerActorPosePlan(profile, timeline, sourceX, sourceY, landingX, landingY);
        }

        public static CombatPowerActorPoseFrame Evaluate(
            CombatPowerActorPosePlan plan,
            CombatPowerActorPoseRole role,
            float elapsedSeconds)
        {
            switch (role)
            {
                case CombatPowerActorPoseRole.Landing:
                    return EvaluateLanding(plan, elapsedSeconds);
                case CombatPowerActorPoseRole.Target:
                    return EvaluateTarget(plan, elapsedSeconds);
                default:
                    return EvaluateSource(plan, elapsedSeconds);
            }
        }

        public static CombatPowerActorPoseFrame EvaluateSource(
            CombatPowerActorPosePlan plan,
            float elapsedSeconds)
        {
            float elapsed = FiniteElapsed(elapsedSeconds, plan.DurationSeconds);
            if (!plan.Supported)
            {
                return CompleteFrame(plan, CombatPowerActorPoseRole.Source, elapsed, plan.SourceX, plan.SourceY);
            }

            if (elapsed >= plan.DurationSeconds)
            {
                float finalX = plan.HasMovement || plan.HasMorph ? plan.LandingX : plan.SourceX;
                float finalY = plan.HasMovement || plan.HasMorph ? plan.LandingY : plan.SourceY;
                return CompleteFrame(plan, CombatPowerActorPoseRole.Source, elapsed, finalX, finalY);
            }

            if (plan.ReducedMotion)
            {
                float x = elapsed >= plan.ImpactAt && (plan.HasMovement || plan.HasMorph) ? plan.LandingX : plan.SourceX;
                float y = elapsed >= plan.ImpactAt && (plan.HasMovement || plan.HasMorph) ? plan.LandingY : plan.SourceY;
                CombatPowerActorPosePhase phase = ReducedSourcePhase(plan);
                return StaticFrame(plan, CombatPowerActorPoseRole.Source, phase, elapsed, 0f, plan.DurationSeconds, x, y);
            }

            switch (plan.Choreography)
            {
                case CombatPowerActorChoreographyKind.Dash:
                    return EvaluateDashSource(plan, elapsed);
                case CombatPowerActorChoreographyKind.Teleport:
                    return EvaluateTeleportSource(plan, elapsed);
                case CombatPowerActorChoreographyKind.Morph:
                    return EvaluateMorphSource(plan, elapsed);
                default:
                    return EvaluateCastSource(plan, elapsed);
            }
        }

        public static CombatPowerActorPoseFrame EvaluateLanding(
            CombatPowerActorPosePlan plan,
            float elapsedSeconds)
        {
            float elapsed = FiniteElapsed(elapsedSeconds, plan.DurationSeconds);
            if (!plan.Supported || !plan.HasLandingReveal || elapsed >= plan.DurationSeconds)
            {
                return CompleteFrame(plan, CombatPowerActorPoseRole.Landing, elapsed, plan.LandingX, plan.LandingY);
            }

            if (elapsed < plan.ImpactAt)
            {
                return HiddenFrame(
                    plan,
                    CombatPowerActorPoseRole.Landing,
                    CombatPowerActorPosePhase.SummonReveal,
                    elapsed,
                    plan.ImpactAt,
                    plan.SummonRevealEndAt,
                    plan.LandingX,
                    plan.LandingY);
            }

            if (elapsed < plan.SummonRevealEndAt)
            {
                if (plan.ReducedMotion)
                {
                    return StaticFrame(
                        plan,
                        CombatPowerActorPoseRole.Landing,
                        CombatPowerActorPosePhase.SummonReveal,
                        elapsed,
                        plan.ImpactAt,
                        plan.SummonRevealEndAt,
                        plan.LandingX,
                        plan.LandingY);
                }

                float progress = PhaseProgress(elapsed, plan.ImpactAt, plan.SummonRevealEndAt);
                float rise = Smooth01(progress);
                float scale = OvershootScale(progress, 0.72f, 1.09f, 1f);
                return Frame(
                    plan,
                    CombatPowerActorPoseRole.Landing,
                    CombatPowerActorPosePhase.SummonReveal,
                    elapsed,
                    plan.ImpactAt,
                    plan.SummonRevealEndAt,
                    progress,
                    plan.LandingX,
                    plan.LandingY,
                    StableSigned(plan, CombatPowerActorPoseRole.Landing, CombatPowerActorPosePhase.SummonReveal, 0) * 0.018f * (1f - rise),
                    0.18f * (1f - rise),
                    scale,
                    SmoothRange(progress, 0f, 0.30f));
            }

            return CompleteFrame(plan, CombatPowerActorPoseRole.Landing, elapsed, plan.LandingX, plan.LandingY);
        }

        public static CombatPowerActorPoseFrame EvaluateTarget(
            CombatPowerActorPosePlan plan,
            float elapsedSeconds)
        {
            float elapsed = FiniteElapsed(elapsedSeconds, plan.DurationSeconds);
            if (!plan.Supported || !plan.HasTargetHit || elapsed < plan.ImpactAt || elapsed >= plan.TargetHitEndAt)
            {
                return CompleteFrame(plan, CombatPowerActorPoseRole.Target, elapsed, plan.LandingX, plan.LandingY);
            }

            if (plan.ReducedMotion)
            {
                return StaticFrame(
                    plan,
                    CombatPowerActorPoseRole.Target,
                    CombatPowerActorPosePhase.TargetHit,
                    elapsed,
                    plan.ImpactAt,
                    plan.TargetHitEndAt,
                    plan.LandingX,
                    plan.LandingY);
            }

            float progress = PhaseProgress(elapsed, plan.ImpactAt, plan.TargetHitEndAt);
            float pulse = (float)Math.Sin(progress * Math.PI);
            if (plan.BeneficialTarget)
            {
                return Frame(
                    plan,
                    CombatPowerActorPoseRole.Target,
                    CombatPowerActorPosePhase.TargetHit,
                    elapsed,
                    plan.ImpactAt,
                    plan.TargetHitEndAt,
                    progress,
                    plan.LandingX,
                    plan.LandingY,
                    StableSigned(plan, CombatPowerActorPoseRole.Target, CombatPowerActorPosePhase.TargetHit, 0) * 0.018f * pulse,
                    -0.055f * plan.Energy * pulse,
                    1f + 0.055f * plan.Energy * pulse,
                    1f);
            }

            DirectionAway(plan, out float awayX, out float awayY);
            float tremor = StableSigned(plan, CombatPowerActorPoseRole.Target, CombatPowerActorPosePhase.TargetHit, 1)
                * 0.012f
                * (1f - progress);
            return Frame(
                plan,
                CombatPowerActorPoseRole.Target,
                CombatPowerActorPosePhase.TargetHit,
                elapsed,
                plan.ImpactAt,
                plan.TargetHitEndAt,
                progress,
                plan.LandingX,
                plan.LandingY,
                awayX * 0.115f * plan.Energy * pulse + tremor,
                awayY * 0.075f * plan.Energy * pulse - 0.025f * pulse,
                1f + 0.038f * plan.Energy * pulse,
                1f);
        }

        public static float PhaseProgress(float elapsedSeconds, float phaseStartAt, float phaseEndAt)
        {
            float start = FiniteNonNegative(phaseStartAt);
            float end = Math.Max(start, FiniteNonNegative(phaseEndAt));
            if (end <= start) return 1f;
            return Clamp01((FiniteElapsed(elapsedSeconds, end) - start) / (end - start));
        }

        public static int StableActorHash(
            string powerKeyOrName,
            int stableSeed,
            CombatPowerActorPoseRole role,
            CombatPowerActorPosePhase phase,
            int channel = 0)
        {
            unchecked
            {
                uint hash = 2166136261u;
                string key = CanonicalOrCompact(powerKeyOrName);
                for (int i = 0; i < key.Length; i++)
                {
                    hash ^= key[i];
                    hash *= 16777619u;
                }
                hash = AppendInt(hash, stableSeed & int.MaxValue);
                hash = AppendInt(hash, (int)role);
                hash = AppendInt(hash, (int)phase);
                hash = AppendInt(hash, channel);
                hash ^= hash >> 16;
                hash *= 2246822519u;
                hash ^= hash >> 13;
                hash *= 3266489917u;
                hash ^= hash >> 16;
                return (int)(hash & 0x7fffffffu);
            }
        }

        public static float StableActorSample(
            string powerKeyOrName,
            int stableSeed,
            CombatPowerActorPoseRole role,
            CombatPowerActorPosePhase phase,
            int channel = 0)
        {
            uint hash = unchecked((uint)StableActorHash(powerKeyOrName, stableSeed, role, phase, channel));
            return (hash & 0x00ffffffu) / 16777216f;
        }

        public static float StableActorSignedSample(
            string powerKeyOrName,
            int stableSeed,
            CombatPowerActorPoseRole role,
            CombatPowerActorPosePhase phase,
            int channel = 0)
        {
            return StableActorSample(powerKeyOrName, stableSeed, role, phase, channel) * 2f - 1f;
        }

        internal static float Finite(float value, float fallback = 0f)
        {
            return float.IsNaN(value) || float.IsInfinity(value) ? fallback : value;
        }

        internal static float FiniteNonNegative(float value)
        {
            return Math.Max(0f, Finite(value));
        }

        internal static float FiniteElapsed(float elapsedSeconds, float positiveInfinityFallback)
        {
            if (float.IsNaN(elapsedSeconds) || float.IsNegativeInfinity(elapsedSeconds)) return 0f;
            if (float.IsPositiveInfinity(elapsedSeconds)) return FiniteNonNegative(positiveInfinityFallback);
            return Math.Max(0f, elapsedSeconds);
        }

        internal static float Clamp(float value, float minimum, float maximum)
        {
            return Math.Max(minimum, Math.Min(maximum, value));
        }

        internal static float Clamp01(float value)
        {
            return Clamp(value, 0f, 1f);
        }

        internal static float SummonRevealDuration(float energy)
        {
            return Clamp(0.30f + energy * 0.055f, 0.32f, 0.40f);
        }

        internal static float MorphOutDuration(float energy)
        {
            return Clamp(0.12f + energy * 0.035f, 0.14f, 0.18f);
        }

        internal static float MorphInDuration(float energy)
        {
            return Clamp(0.22f + energy * 0.045f, 0.24f, 0.30f);
        }

        private static CombatPowerActorPoseFrame EvaluateCastSource(
            CombatPowerActorPosePlan plan,
            float elapsed)
        {
            if (elapsed < plan.CastWindupEndAt)
            {
                return CastWindupFrame(plan, elapsed, plan.CastWindupEndAt);
            }

            if (elapsed < plan.ReleaseEndAt)
            {
                float progress = PhaseProgress(elapsed, plan.ReleaseAt, plan.ReleaseEndAt);
                DirectionToLanding(plan, out float directionX, out float directionY);
                float kick = (float)Math.Sin(progress * Math.PI);
                return Frame(
                    plan,
                    CombatPowerActorPoseRole.Source,
                    CombatPowerActorPosePhase.Release,
                    elapsed,
                    plan.ReleaseAt,
                    plan.ReleaseEndAt,
                    progress,
                    plan.SourceX,
                    plan.SourceY,
                    directionX * 0.115f * plan.Energy * kick,
                    directionY * 0.070f * plan.Energy * kick - 0.018f * kick,
                    1f + 0.065f * plan.Energy * kick,
                    1f);
            }

            if (elapsed < plan.RecoveryEndAt)
            {
                return RecoveryFrame(plan, elapsed, plan.SourceX, plan.SourceY);
            }

            return CompleteFrame(plan, CombatPowerActorPoseRole.Source, elapsed, plan.SourceX, plan.SourceY);
        }

        private static CombatPowerActorPoseFrame EvaluateDashSource(
            CombatPowerActorPosePlan plan,
            float elapsed)
        {
            if (elapsed < plan.CastWindupEndAt)
            {
                return CastWindupFrame(plan, elapsed, plan.CastWindupEndAt);
            }

            if (elapsed < plan.ImpactAt && plan.ImpactAt > plan.ReleaseAt)
            {
                float progress = PhaseProgress(elapsed, plan.ReleaseAt, plan.ImpactAt);
                float move = EaseInOut(progress);
                float arc = (float)Math.Sin(progress * Math.PI);
                float x = Lerp(plan.SourceX, plan.LandingX, move);
                float y = Lerp(plan.SourceY, plan.LandingY, move);
                float side = StableSigned(plan, CombatPowerActorPoseRole.Source, CombatPowerActorPosePhase.Dash, 0);
                return Frame(
                    plan,
                    CombatPowerActorPoseRole.Source,
                    CombatPowerActorPosePhase.Dash,
                    elapsed,
                    plan.ReleaseAt,
                    plan.ImpactAt,
                    progress,
                    x,
                    y,
                    side * 0.022f * arc,
                    -0.10f * plan.Energy * arc,
                    1f + 0.065f * plan.Energy * arc,
                    1f);
            }

            if (elapsed < plan.RecoveryEndAt)
            {
                return RecoveryFrame(plan, elapsed, plan.LandingX, plan.LandingY);
            }

            return CompleteFrame(plan, CombatPowerActorPoseRole.Source, elapsed, plan.LandingX, plan.LandingY);
        }

        private static CombatPowerActorPoseFrame EvaluateTeleportSource(
            CombatPowerActorPosePlan plan,
            float elapsed)
        {
            if (elapsed < plan.CastWindupEndAt)
            {
                return CastWindupFrame(plan, elapsed, plan.CastWindupEndAt);
            }

            if (elapsed < plan.TeleportSplitAt && plan.TeleportSplitAt > plan.ReleaseAt)
            {
                float progress = PhaseProgress(elapsed, plan.ReleaseAt, plan.TeleportSplitAt);
                float fade = Smooth01(progress);
                float jitter = StableSigned(plan, CombatPowerActorPoseRole.Source, CombatPowerActorPosePhase.TeleportOut, 0);
                return Frame(
                    plan,
                    CombatPowerActorPoseRole.Source,
                    CombatPowerActorPosePhase.TeleportOut,
                    elapsed,
                    plan.ReleaseAt,
                    plan.TeleportSplitAt,
                    progress,
                    plan.SourceX,
                    plan.SourceY,
                    jitter * 0.055f * fade,
                    -0.075f * fade,
                    Lerp(1f, 0.78f, fade),
                    1f - fade);
            }

            if (elapsed < plan.ImpactAt && plan.ImpactAt > plan.TeleportSplitAt)
            {
                float progress = PhaseProgress(elapsed, plan.TeleportSplitAt, plan.ImpactAt);
                float appear = Smooth01(progress);
                float jitter = StableSigned(plan, CombatPowerActorPoseRole.Source, CombatPowerActorPosePhase.TeleportIn, 0);
                return Frame(
                    plan,
                    CombatPowerActorPoseRole.Source,
                    CombatPowerActorPosePhase.TeleportIn,
                    elapsed,
                    plan.TeleportSplitAt,
                    plan.ImpactAt,
                    progress,
                    plan.LandingX,
                    plan.LandingY,
                    jitter * 0.055f * (1f - appear),
                    0.075f * (1f - appear),
                    OvershootScale(progress, 0.78f, 1.08f, 1f),
                    appear);
            }

            if (elapsed < plan.RecoveryEndAt)
            {
                return RecoveryFrame(plan, elapsed, plan.LandingX, plan.LandingY);
            }

            return CompleteFrame(plan, CombatPowerActorPoseRole.Source, elapsed, plan.LandingX, plan.LandingY);
        }

        private static CombatPowerActorPoseFrame EvaluateMorphSource(
            CombatPowerActorPosePlan plan,
            float elapsed)
        {
            if (elapsed < plan.MorphOutStartAt)
            {
                return CastWindupFrame(plan, elapsed, plan.MorphOutStartAt);
            }

            if (elapsed < plan.ImpactAt)
            {
                float progress = PhaseProgress(elapsed, plan.MorphOutStartAt, plan.ImpactAt);
                float shrink = Smooth01(progress);
                float direction = StableSigned(plan, CombatPowerActorPoseRole.Source, CombatPowerActorPosePhase.MorphOut, 0);
                return Frame(
                    plan,
                    CombatPowerActorPoseRole.Source,
                    CombatPowerActorPosePhase.MorphOut,
                    elapsed,
                    plan.MorphOutStartAt,
                    plan.ImpactAt,
                    progress,
                    plan.SourceX,
                    plan.SourceY,
                    direction * 0.045f * shrink,
                    0.08f * shrink,
                    Lerp(1f, 0.74f, shrink),
                    Lerp(1f, 0.34f, shrink));
            }

            if (elapsed < plan.MorphInEndAt)
            {
                float progress = PhaseProgress(elapsed, plan.ImpactAt, plan.MorphInEndAt);
                float appear = Smooth01(progress);
                float direction = StableSigned(plan, CombatPowerActorPoseRole.Source, CombatPowerActorPosePhase.MorphIn, 0);
                return Frame(
                    plan,
                    CombatPowerActorPoseRole.Source,
                    CombatPowerActorPosePhase.MorphIn,
                    elapsed,
                    plan.ImpactAt,
                    plan.MorphInEndAt,
                    progress,
                    plan.LandingX,
                    plan.LandingY,
                    direction * 0.045f * (1f - appear),
                    -0.10f * (1f - appear),
                    OvershootScale(progress, 0.74f, 1.13f, 1f),
                    Lerp(0.34f, 1f, appear));
            }

            if (elapsed < plan.RecoveryEndAt)
            {
                return RecoveryFrame(plan, elapsed, plan.LandingX, plan.LandingY);
            }

            return CompleteFrame(plan, CombatPowerActorPoseRole.Source, elapsed, plan.LandingX, plan.LandingY);
        }

        private static CombatPowerActorPoseFrame CastWindupFrame(
            CombatPowerActorPosePlan plan,
            float elapsed,
            float endAt)
        {
            float progress = PhaseProgress(elapsed, 0f, endAt);
            float gather = Smooth01(progress);
            DirectionToLanding(plan, out float directionX, out float directionY);
            float side = StableSigned(plan, CombatPowerActorPoseRole.Source, CombatPowerActorPosePhase.CastWindup, 0);
            float pulse = (float)Math.Sin(progress * Math.PI * 2f) * (1f - progress) * 0.012f;
            return Frame(
                plan,
                CombatPowerActorPoseRole.Source,
                CombatPowerActorPosePhase.CastWindup,
                elapsed,
                0f,
                endAt,
                progress,
                plan.SourceX,
                plan.SourceY,
                -directionX * 0.045f * plan.Energy * gather + side * 0.012f * gather,
                -directionY * 0.028f * plan.Energy * gather + 0.038f * gather,
                1f - 0.045f * plan.Energy * gather + pulse,
                1f);
        }

        private static CombatPowerActorPoseFrame RecoveryFrame(
            CombatPowerActorPosePlan plan,
            float elapsed,
            float x,
            float y)
        {
            float progress = PhaseProgress(elapsed, plan.RecoveryStartAt, plan.RecoveryEndAt);
            float settle = (1f - progress) * (float)Math.Sin(progress * Math.PI * 2f);
            DirectionToLanding(plan, out float directionX, out float directionY);
            return Frame(
                plan,
                CombatPowerActorPoseRole.Source,
                CombatPowerActorPosePhase.Recovery,
                elapsed,
                plan.RecoveryStartAt,
                plan.RecoveryEndAt,
                progress,
                x,
                y,
                -directionX * 0.035f * plan.Energy * settle,
                -directionY * 0.020f * plan.Energy * settle,
                1f + 0.028f * plan.Energy * settle,
                1f);
        }

        private static CombatPowerActorPoseFrame Frame(
            CombatPowerActorPosePlan plan,
            CombatPowerActorPoseRole role,
            CombatPowerActorPosePhase phase,
            float elapsed,
            float start,
            float end,
            float progress,
            float positionX,
            float positionY,
            float offsetX,
            float offsetY,
            float scale,
            float opacity)
        {
            return new CombatPowerActorPoseFrame(
                plan,
                role,
                phase,
                elapsed,
                start,
                end,
                progress,
                positionX,
                positionY,
                offsetX,
                offsetY,
                scale,
                opacity);
        }

        private static CombatPowerActorPoseFrame StaticFrame(
            CombatPowerActorPosePlan plan,
            CombatPowerActorPoseRole role,
            CombatPowerActorPosePhase phase,
            float elapsed,
            float start,
            float end,
            float x,
            float y)
        {
            return Frame(
                plan,
                role,
                phase,
                elapsed,
                start,
                end,
                PhaseProgress(elapsed, start, end),
                x,
                y,
                0f,
                0f,
                1f,
                1f);
        }

        private static CombatPowerActorPoseFrame HiddenFrame(
            CombatPowerActorPosePlan plan,
            CombatPowerActorPoseRole role,
            CombatPowerActorPosePhase phase,
            float elapsed,
            float start,
            float end,
            float x,
            float y)
        {
            return Frame(plan, role, phase, elapsed, start, end, 0f, x, y, 0f, 0f, 1f, 0f);
        }

        private static CombatPowerActorPoseFrame CompleteFrame(
            CombatPowerActorPosePlan plan,
            CombatPowerActorPoseRole role,
            float elapsed,
            float x,
            float y)
        {
            return Frame(
                plan,
                role,
                CombatPowerActorPosePhase.Complete,
                elapsed,
                plan.DurationSeconds,
                plan.DurationSeconds,
                1f,
                x,
                y,
                0f,
                0f,
                1f,
                0f);
        }

        private static CombatPowerActorPosePhase ReducedSourcePhase(CombatPowerActorPosePlan plan)
        {
            switch (plan.Choreography)
            {
                case CombatPowerActorChoreographyKind.Dash: return CombatPowerActorPosePhase.Dash;
                case CombatPowerActorChoreographyKind.Teleport: return CombatPowerActorPosePhase.TeleportIn;
                case CombatPowerActorChoreographyKind.Morph: return CombatPowerActorPosePhase.MorphIn;
                default: return CombatPowerActorPosePhase.Release;
            }
        }

        private static CombatPowerActorPoseProfile Profile(
            CombatPowerAnimationSourceKind sourceKind,
            string key,
            CombatPowerActorChoreographyKind choreography,
            bool hasTargetHit,
            bool beneficialTarget,
            float energy,
            float releaseDuration,
            float recoveryDuration,
            float targetHitDuration)
        {
            return new CombatPowerActorPoseProfile(
                sourceKind,
                key,
                choreography,
                hasTargetHit,
                beneficialTarget,
                energy,
                releaseDuration,
                recoveryDuration,
                targetHitDuration);
        }

        private static CombatPowerActorPoseProfile EmptyProfile(string key)
        {
            return new CombatPowerActorPoseProfile(
                CombatPowerAnimationSourceKind.Unknown,
                (key ?? "").Trim().ToLowerInvariant(),
                CombatPowerActorChoreographyKind.Cast,
                false,
                false,
                0f,
                0f,
                0f,
                0f);
        }

        private static bool IsBeneficialFormulaTarget(string key)
        {
            switch (key)
            {
                case "OIC": case "NVC": case "TBQ": case "SGW": case "TNC": case "LBC":
                case "TBG": case "NVL": case "SWR": case "DWP": case "SLV":
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsHostileFormulaTarget(string key)
        {
            switch (key)
            {
                case "OBL": case "LNH": case "SBN":
                case "FIF": case "RCL": case "RDF": case "RIG": case "FBL": case "RLF":
                case "RBI": case "MTR": case "CLT": case "FRB": case "AST":
                case "RMS": case "RNH": case "RKW": case "RPX": case "INH": case "RMB":
                case "RLM": case "WTR": case "DSM": case "PBR": case "RBT":
                case "CNS": case "GRH": case "ACR":
                    return true;
                default:
                    return false;
            }
        }

        private static float FormulaEnergy(string key)
        {
            switch (key)
            {
                case "FBL": return 1.28f;
                case "MTR": case "AST": case "IBG": return 1.46f;
                case "CNS": case "DWP": case "ACR": case "SLV": case "IBF": return 1.34f;
                case "DFA": return 1.48f;
                case "FIF": case "NVC": case "RBT": return 0.82f;
                default: return 1f;
            }
        }

        private static float AbilityEnergy(string key)
        {
            switch (key)
            {
                case "charge": case "execute": case "whirlwind": case "eviscerate":
                case "shadowstep": case "riftpounce": case "abyssalwhirl": case "soulrend":
                case "dreadroar":
                    return 1.24f;
                case "throwknife": case "quickshot": case "scoutmark": return 0.86f;
                default: return 1f;
            }
        }

        private static string CanonicalOrCompact(string powerKeyOrName)
        {
            if (CombatPowerSfxRules.IsSupportedFormula(powerKeyOrName))
            {
                return CombatPowerTravelVfxRules.NormalizeFormulaKey(powerKeyOrName);
            }
            if (CombatPowerSfxRules.IsSupportedAbility(powerKeyOrName))
            {
                return CombatPowerTravelVfxRules.NormalizeAbilityKey(powerKeyOrName);
            }
            return Compact(powerKeyOrName);
        }

        private static string Compact(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            char[] buffer = new char[value.Length];
            int count = 0;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (!char.IsLetterOrDigit(c)) continue;
                buffer[count++] = char.ToLowerInvariant(c);
            }
            return new string(buffer, 0, count);
        }

        private static float StableSigned(
            CombatPowerActorPosePlan plan,
            CombatPowerActorPoseRole role,
            CombatPowerActorPosePhase phase,
            int channel)
        {
            return StableActorSignedSample(plan.PowerKey, plan.StableSeed, role, phase, channel);
        }

        private static void DirectionToLanding(
            CombatPowerActorPosePlan plan,
            out float directionX,
            out float directionY)
        {
            NormalizeDirection(plan.LandingX - plan.SourceX, plan.LandingY - plan.SourceY, out directionX, out directionY);
            if (directionX == 0f && directionY == 0f)
            {
                directionX = StableSigned(plan, CombatPowerActorPoseRole.Source, CombatPowerActorPosePhase.CastWindup, 2) >= 0f ? 1f : -1f;
            }
        }

        private static void DirectionAway(
            CombatPowerActorPosePlan plan,
            out float directionX,
            out float directionY)
        {
            NormalizeDirection(plan.LandingX - plan.SourceX, plan.LandingY - plan.SourceY, out directionX, out directionY);
            if (directionX == 0f && directionY == 0f)
            {
                directionX = StableSigned(plan, CombatPowerActorPoseRole.Target, CombatPowerActorPosePhase.TargetHit, 2) >= 0f ? 1f : -1f;
            }
        }

        private static void NormalizeDirection(
            float x,
            float y,
            out float normalizedX,
            out float normalizedY)
        {
            double length = Math.Sqrt(x * x + y * y);
            if (length <= 0.0001)
            {
                normalizedX = 0f;
                normalizedY = 0f;
                return;
            }
            normalizedX = (float)(x / length);
            normalizedY = (float)(y / length);
        }

        private static float Smooth01(float value)
        {
            float t = Clamp01(value);
            return t * t * (3f - 2f * t);
        }

        private static float SmoothRange(float value, float start, float end)
        {
            if (end <= start) return value >= end ? 1f : 0f;
            return Smooth01((value - start) / (end - start));
        }

        private static float EaseInOut(float value)
        {
            float t = Clamp01(value);
            return t < 0.5f
                ? 4f * t * t * t
                : 1f - (float)Math.Pow(-2f * t + 2f, 3f) * 0.5f;
        }

        private static float OvershootScale(float progress, float start, float peak, float end)
        {
            float t = Clamp01(progress);
            if (t < 0.68f)
            {
                return Lerp(start, peak, Smooth01(t / 0.68f));
            }
            return Lerp(peak, end, Smooth01((t - 0.68f) / 0.32f));
        }

        private static float Lerp(float start, float end, float progress)
        {
            return start + (end - start) * Clamp01(progress);
        }

        private static uint AppendInt(uint hash, int value)
        {
            unchecked
            {
                hash ^= (byte)value;
                hash *= 16777619u;
                hash ^= (byte)(value >> 8);
                hash *= 16777619u;
                hash ^= (byte)(value >> 16);
                hash *= 16777619u;
                hash ^= (byte)(value >> 24);
                hash *= 16777619u;
                return hash;
            }
        }
    }
}
