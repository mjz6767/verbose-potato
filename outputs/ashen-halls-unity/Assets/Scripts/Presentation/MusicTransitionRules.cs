using System;

namespace AshenHalls
{
    public enum MusicTransitionContext
    {
        Explore = 0,
        Title = 1,
        WorldMapExplore = 2,
        Combat = 3,
        Victory = 4,
        Defeat = 5
    }

    public readonly struct MusicCrossfadeGains
    {
        public readonly float Outgoing;
        public readonly float Incoming;

        public MusicCrossfadeGains(float outgoing, float incoming)
        {
            Outgoing = MusicTransitionRules.Clamp01(outgoing);
            Incoming = MusicTransitionRules.Clamp01(incoming);
        }
    }

    public readonly struct MusicTransitionTiming
    {
        public readonly float TransitionDuration;
        public readonly float IntroFadeDuration;

        public MusicTransitionTiming(float transitionDuration, float introFadeDuration)
        {
            TransitionDuration = MusicTransitionRules.ClampDuration(transitionDuration);
            IntroFadeDuration = MusicTransitionRules.ClampDuration(introFadeDuration);
        }
    }

    public enum ExplorationMusicSwitchReason
    {
        NoCandidate = 0,
        SameRoute = 1,
        InitialRoute = 2,
        ExplicitWorldMapViewChange = 3,
        PursuitEntered = 4,
        PursuitReleaseHolding = 5,
        PursuitReleased = 6,
        CalmCandidateHolding = 7,
        CurrentRouteDwelling = 8,
        CalmRouteStable = 9
    }

    public readonly struct ExplorationMusicSwitchInput
    {
        public readonly string CurrentRoute;
        public readonly string CandidateRoute;
        public readonly float CandidateHeldSeconds;
        public readonly float CurrentDwellSeconds;
        public readonly float PursuitReleaseHeldSeconds;
        public readonly bool ExplicitWorldMapViewChange;

        public ExplorationMusicSwitchInput(
            string currentRoute,
            string candidateRoute,
            float candidateHeldSeconds,
            float currentDwellSeconds,
            float pursuitReleaseHeldSeconds,
            bool explicitWorldMapViewChange)
        {
            CurrentRoute = MusicTransitionRules.NormalizeRouteKey(currentRoute);
            CandidateRoute = MusicTransitionRules.NormalizeRouteKey(candidateRoute);
            CandidateHeldSeconds = MusicTransitionRules.ClampObservedSeconds(candidateHeldSeconds);
            CurrentDwellSeconds = MusicTransitionRules.ClampObservedSeconds(currentDwellSeconds);
            PursuitReleaseHeldSeconds = MusicTransitionRules.ClampObservedSeconds(pursuitReleaseHeldSeconds);
            ExplicitWorldMapViewChange = explicitWorldMapViewChange;
        }
    }

    public readonly struct ExplorationMusicSwitchDecision
    {
        public readonly bool ShouldSwitch;
        public readonly ExplorationMusicSwitchReason Reason;
        public readonly float HoldRemaining;

        public ExplorationMusicSwitchDecision(
            bool shouldSwitch,
            ExplorationMusicSwitchReason reason,
            float holdRemaining)
        {
            ShouldSwitch = shouldSwitch;
            Reason = reason;
            HoldRemaining = shouldSwitch
                ? 0f
                : MusicTransitionRules.ClampObservedSeconds(holdRemaining);
        }
    }

    public static class MusicTransitionRules
    {
        public const float TitleTransitionDuration = 1.35f;
        public const float TitleIntroFadeDuration = 2.00f;
        public const float WorldMapExploreTransitionDuration = 1.10f;
        public const float WorldMapExploreIntroFadeDuration = 1.25f;
        public const float ExploreTransitionDuration = 0.85f;
        public const float ExploreIntroFadeDuration = 1.00f;
        public const float CombatTransitionDuration = 0.45f;
        public const float CombatIntroFadeDuration = 0.35f;
        public const float VictoryTransitionDuration = 1.15f;
        public const float VictoryIntroFadeDuration = 0.75f;
        public const float DefeatTransitionDuration = 1.40f;
        public const float DefeatIntroFadeDuration = 0.95f;

        public const float CalmCandidateHoldDuration = 1.50f;
        public const float CalmCurrentDwellDuration = 8.00f;
        public const float PursuitReleaseHoldDuration = 3.00f;

        private const float MaximumDuration = 60.00f;
        private const float MaximumObservedSeconds = 3600.00f;

        public static MusicCrossfadeGains EqualPowerCrossfade(float progress)
        {
            float safeProgress = Clamp01(progress);
            double angle = safeProgress * Math.PI * 0.5d;
            return new MusicCrossfadeGains(
                (float)Math.Cos(angle),
                (float)Math.Sin(angle));
        }

        public static MusicTransitionTiming TimingFor(MusicTransitionContext context)
        {
            switch (context)
            {
                case MusicTransitionContext.Title:
                    return new MusicTransitionTiming(TitleTransitionDuration, TitleIntroFadeDuration);
                case MusicTransitionContext.WorldMapExplore:
                    return new MusicTransitionTiming(
                        WorldMapExploreTransitionDuration,
                        WorldMapExploreIntroFadeDuration);
                case MusicTransitionContext.Combat:
                    return new MusicTransitionTiming(CombatTransitionDuration, CombatIntroFadeDuration);
                case MusicTransitionContext.Victory:
                    return new MusicTransitionTiming(VictoryTransitionDuration, VictoryIntroFadeDuration);
                case MusicTransitionContext.Defeat:
                    return new MusicTransitionTiming(DefeatTransitionDuration, DefeatIntroFadeDuration);
                case MusicTransitionContext.Explore:
                default:
                    return new MusicTransitionTiming(ExploreTransitionDuration, ExploreIntroFadeDuration);
            }
        }

        public static float TransitionDurationFor(MusicTransitionContext context)
        {
            return TimingFor(context).TransitionDuration;
        }

        public static float IntroFadeDurationFor(MusicTransitionContext context)
        {
            return TimingFor(context).IntroFadeDuration;
        }

        public static bool ShouldKeepTransportAlive(
            bool hasState,
            bool hasDesiredClip,
            bool splashVisible,
            bool musicMuted)
        {
            // Mute controls gain, not transport. Keeping the playhead alive avoids
            // restarting a title, exploration, or combat cue when sound returns.
            return hasState && hasDesiredClip && !splashVisible;
        }

        public static ExplorationMusicSwitchDecision EvaluateExplorationSwitch(
            ExplorationMusicSwitchInput input)
        {
            string current = NormalizeRouteKey(input.CurrentRoute);
            string candidate = NormalizeRouteKey(input.CandidateRoute);
            if (candidate.Length == 0)
            {
                return new ExplorationMusicSwitchDecision(
                    false,
                    ExplorationMusicSwitchReason.NoCandidate,
                    0f);
            }

            if (string.Equals(current, candidate, StringComparison.Ordinal))
            {
                return new ExplorationMusicSwitchDecision(
                    false,
                    ExplorationMusicSwitchReason.SameRoute,
                    0f);
            }

            if (current.Length == 0)
            {
                return new ExplorationMusicSwitchDecision(
                    true,
                    ExplorationMusicSwitchReason.InitialRoute,
                    0f);
            }

            bool currentIsPursuit = IsPursuitRoute(current);
            bool candidateIsPursuit = IsPursuitRoute(candidate);
            if (candidateIsPursuit)
            {
                return new ExplorationMusicSwitchDecision(
                    true,
                    ExplorationMusicSwitchReason.PursuitEntered,
                    0f);
            }

            if (currentIsPursuit)
            {
                float releaseHeld = ClampObservedSeconds(input.PursuitReleaseHeldSeconds);
                float remaining = Math.Max(0f, PursuitReleaseHoldDuration - releaseHeld);
                return remaining <= 0f
                    ? new ExplorationMusicSwitchDecision(
                        true,
                        ExplorationMusicSwitchReason.PursuitReleased,
                        0f)
                    : new ExplorationMusicSwitchDecision(
                        false,
                        ExplorationMusicSwitchReason.PursuitReleaseHolding,
                        remaining);
            }

            // Explicit map changes are immediate for calm routes, but pursuit
            // always keeps priority and must clear its release hold first.
            if (input.ExplicitWorldMapViewChange)
            {
                return new ExplorationMusicSwitchDecision(
                    true,
                    ExplorationMusicSwitchReason.ExplicitWorldMapViewChange,
                    0f);
            }

            float candidateHeld = ClampObservedSeconds(input.CandidateHeldSeconds);
            float currentDwell = ClampObservedSeconds(input.CurrentDwellSeconds);
            float candidateRemaining = Math.Max(0f, CalmCandidateHoldDuration - candidateHeld);
            float dwellRemaining = Math.Max(0f, CalmCurrentDwellDuration - currentDwell);
            if (candidateRemaining > 0f)
            {
                return new ExplorationMusicSwitchDecision(
                    false,
                    ExplorationMusicSwitchReason.CalmCandidateHolding,
                    Math.Max(candidateRemaining, dwellRemaining));
            }

            if (dwellRemaining > 0f)
            {
                return new ExplorationMusicSwitchDecision(
                    false,
                    ExplorationMusicSwitchReason.CurrentRouteDwelling,
                    dwellRemaining);
            }

            return new ExplorationMusicSwitchDecision(
                true,
                ExplorationMusicSwitchReason.CalmRouteStable,
                0f);
        }

        public static ExplorationMusicSwitchDecision EvaluateExplorationSwitch(
            string currentRoute,
            string candidateRoute,
            float candidateHeldSeconds,
            float currentDwellSeconds,
            float pursuitReleaseHeldSeconds,
            bool explicitWorldMapViewChange)
        {
            return EvaluateExplorationSwitch(new ExplorationMusicSwitchInput(
                currentRoute,
                candidateRoute,
                candidateHeldSeconds,
                currentDwellSeconds,
                pursuitReleaseHeldSeconds,
                explicitWorldMapViewChange));
        }

        public static bool ShouldSwitchExplorationMusic(
            string currentRoute,
            string candidateRoute,
            float candidateHeldSeconds,
            float currentDwellSeconds,
            float pursuitReleaseHeldSeconds,
            bool explicitWorldMapViewChange)
        {
            return EvaluateExplorationSwitch(
                currentRoute,
                candidateRoute,
                candidateHeldSeconds,
                currentDwellSeconds,
                pursuitReleaseHeldSeconds,
                explicitWorldMapViewChange).ShouldSwitch;
        }

        public static bool IsPursuitRoute(string routeKey)
        {
            return string.Equals(
                NormalizeRouteKey(routeKey),
                MusicDirectorRules.HuntedRoad,
                StringComparison.Ordinal);
        }

        public static string NormalizeRouteKey(string routeKey)
        {
            return (routeKey ?? "").Trim().ToLowerInvariant();
        }

        internal static float Clamp01(float value)
        {
            return ClampFinite(value, 0f, 1f);
        }

        internal static float ClampDuration(float value)
        {
            return ClampFinite(value, 0f, MaximumDuration);
        }

        internal static float ClampObservedSeconds(float value)
        {
            return ClampFinite(value, 0f, MaximumObservedSeconds);
        }

        private static float ClampFinite(float value, float minimum, float maximum)
        {
            if (float.IsNaN(value) || float.IsNegativeInfinity(value)) return minimum;
            if (float.IsPositiveInfinity(value)) return maximum;
            return Math.Max(minimum, Math.Min(maximum, value));
        }
    }
}
