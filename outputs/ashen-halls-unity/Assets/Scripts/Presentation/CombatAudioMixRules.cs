using System;

namespace AshenHalls
{
    public static class CombatAudioMixRules
    {
        public const int SfxVoiceCount = 8;
        public const int ScheduledSfxCapacity = 16;
        public const int ScheduledSfxPriorityAuxiliary = 0;
        public const int ScheduledSfxPrioritySupporting = 1;
        public const int ScheduledSfxPrioritySecondaryImpact = 2;
        public const int ScheduledSfxPriorityPrimaryImpact = 3;

        public const float ScheduledSfxCoalesceWindow = 0.032f;
        public const float ScheduledSfxCoalescePanDistance = 0.20f;

        public static float StereoPanForColumn(int column, int boardWidth)
        {
            if (boardWidth <= 1) return 0f;
            float normalized = column / (float)(boardWidth - 1);
            return Clamp((normalized * 2f - 1f) * 0.72f, -0.72f, 0.72f);
        }

        public static float StereoPanMidpoint(float sourcePan, float targetPan)
        {
            return Clamp((sourcePan + targetPan) * 0.5f, -0.72f, 0.72f);
        }

        public static float PitchForCue(string key, int column)
        {
            int hash = 17;
            string clean = key ?? "";
            for (int i = 0; i < clean.Length; i++) hash = unchecked(hash * 31 + clean[i]);
            uint mixed = unchecked((uint)(hash + column * 19));
            int variation = (int)(mixed % 7u) - 3;
            return Clamp(1f + variation * 0.015f, 0.95f, 1.05f);
        }

        public static float MusicDuckDepth(CombatImpactProfile profile)
        {
            switch (CombatImpactRules.VisualIntensity(profile))
            {
                case 3:
                    return Clamp(
                        0.38f
                        + Math.Min(0.10f, profile.ShakeMagnitude * 0.012f)
                        + Math.Min(0.04f, profile.BurstCount * 0.0012f),
                        0.42f,
                        0.54f);
                case 2: return 0.28f;
                default: return 0f;
            }
        }

        public static float MusicDuckDepth(CombatImpactProfile profile, int reactionCount)
        {
            float depth = MusicDuckDepth(profile);
            if (reactionCount <= 0) return depth;
            return Math.Max(depth, Math.Min(0.44f, 0.28f + Math.Min(2, reactionCount) * 0.08f));
        }

        public static float MusicDuckDuration(CombatImpactProfile profile)
        {
            if (MusicDuckDepth(profile) <= 0f) return 0f;
            return Clamp(Math.Max(profile.ResolutionDelay, profile.AftershockDelay) + 0.22f, 0.36f, 0.82f);
        }

        public static float MusicDuckDuration(CombatImpactProfile profile, int reactionCount)
        {
            if (MusicDuckDepth(profile, reactionCount) <= 0f) return 0f;
            return Clamp(MusicDuckDuration(profile) + (reactionCount > 0 ? 0.08f : 0f), 0.36f, 0.88f);
        }

        public static float MusicDuckAttackDuration(CombatImpactProfile profile, int reactionCount)
        {
            if (MusicDuckDepth(profile, reactionCount) <= 0f) return 0f;
            return CombatImpactRules.VisualIntensity(profile, reactionCount) >= 3 ? 0.065f : 0.050f;
        }

        public static float MusicDuckHoldDuration(CombatImpactProfile profile, int reactionCount)
        {
            if (MusicDuckDepth(profile, reactionCount) <= 0f) return 0f;
            float baseHold = CombatImpactRules.VisualIntensity(profile, reactionCount) >= 3 ? 0.090f : 0.070f;
            return baseHold + Math.Min(2, Math.Max(0, reactionCount)) * 0.015f;
        }

        public static float MusicDuckReleaseDuration(CombatImpactProfile profile, int reactionCount)
        {
            float duration = MusicDuckDuration(profile, reactionCount);
            if (duration <= 0f) return 0f;
            return Clamp(duration * 0.64f, 0.24f, 0.56f);
        }

        public static float MusicDuckEnvelopeMultiplier(
            float now,
            float attackStartedAt,
            float fullDepthAt,
            float holdUntil,
            float releaseUntil,
            float depth)
        {
            float safeDepth = Clamp(depth, 0f, 0.90f);
            if (safeDepth <= 0f || now < attackStartedAt || now >= releaseUntil) return 1f;

            float floor = 1f - safeDepth;
            if (now < fullDepthAt)
            {
                float duration = Math.Max(0.001f, fullDepthAt - attackStartedAt);
                return 1f - safeDepth * Smooth01((now - attackStartedAt) / duration);
            }

            if (now <= holdUntil) return floor;

            float releaseDuration = Math.Max(0.001f, releaseUntil - holdUntil);
            return floor + safeDepth * Smooth01((now - holdUntil) / releaseDuration);
        }

        public static bool ShouldLayerEpicImpact(CombatImpactProfile profile)
        {
            return CombatImpactRules.VisualIntensity(profile) >= 3;
        }

        public static bool ShouldLayerSpellRelease(CombatImpactProfile profile)
        {
            return !string.IsNullOrEmpty(profile.CastSfx)
                && profile.CastSfx.StartsWith("cast", StringComparison.OrdinalIgnoreCase)
                && profile.ImpactDelay >= 0.06f;
        }

        public static bool ShouldLayerCastShimmer(CombatImpactProfile profile)
        {
            return ShouldLayerEpicImpact(profile) && ShouldLayerSpellRelease(profile);
        }

        public static bool ShouldLayerReaction(int reactionCount)
        {
            return reactionCount > 0;
        }

        public static float AuxiliaryLayerVolume(float volume)
        {
            return Clamp(volume * 0.70f, 0f, 0.72f);
        }

        public static bool ShouldCoalesceScheduledCue(
            string existingKey,
            float existingPlayAt,
            float existingPan,
            int existingPriority,
            string incomingKey,
            float incomingPlayAt,
            float incomingPan,
            int incomingPriority)
        {
            if (existingPriority >= ScheduledSfxPriorityPrimaryImpact
                || incomingPriority >= ScheduledSfxPriorityPrimaryImpact)
            {
                return false;
            }

            return string.Equals(existingKey ?? "", incomingKey ?? "", StringComparison.OrdinalIgnoreCase)
                && Math.Abs(existingPlayAt - incomingPlayAt) <= ScheduledSfxCoalesceWindow
                && Math.Abs(existingPan - incomingPan) <= ScheduledSfxCoalescePanDistance;
        }

        public static int SecondaryImpactBeatCount(CombatImpactProfile profile, int reactionCount)
        {
            string impact = (profile.ImpactSfx ?? "").Trim().ToLowerInvariant();
            string cast = (profile.CastSfx ?? "").Trim().ToLowerInvariant();
            string aftershock = (profile.AftershockSfx ?? "").Trim().ToLowerInvariant();
            if (impact == "tempest") return 3;
            if (impact == "meteor") return 2;
            if (impact == "shock" && cast == "castshock" && aftershock == "resonance") return 2;
            return reactionCount > 0 && CombatImpactRules.VisualIntensity(profile, reactionCount) >= 3 ? 1 : 0;
        }

        public static float SecondaryImpactDelay(CombatImpactProfile profile, int index)
        {
            int safeIndex = Math.Max(0, Math.Min(2, index));
            return Clamp(profile.ImpactDelay + 0.055f + safeIndex * 0.050f, 0f, 0.60f);
        }

        public static float SecondaryImpactPan(float primaryPan, int index)
        {
            int safeIndex = Math.Max(0, Math.Min(2, index));
            float offset = safeIndex == 0 ? -0.24f : safeIndex == 1 ? 0.28f : -0.10f;
            float direction = primaryPan > 0.18f ? -1f : 1f;
            return Clamp(primaryPan * 0.58f + offset * direction, -0.72f, 0.72f);
        }

        public static float SecondaryImpactVolume(CombatImpactProfile profile, int index)
        {
            int safeIndex = Math.Max(0, Math.Min(2, index));
            return Clamp(profile.ImpactVolume * (0.42f - safeIndex * 0.055f), 0.22f, 0.52f);
        }

        public static float SecondaryImpactPitch(float primaryPitch, int index)
        {
            int safeIndex = Math.Max(0, Math.Min(2, index));
            float offset = safeIndex == 0 ? 0.035f : safeIndex == 1 ? -0.025f : 0.055f;
            return Clamp(primaryPitch + offset, 0.90f, 1.10f);
        }

        public static string SecondaryImpactCue(CombatImpactProfile profile, int index)
        {
            string impact = (profile.ImpactSfx ?? "").Trim().ToLowerInvariant();
            if (impact == "meteor" && index > 0) return "impactlow";
            return string.IsNullOrEmpty(profile.ImpactSfx) ? profile.AftershockSfx : profile.ImpactSfx;
        }

        private static float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        private static float Smooth01(float value)
        {
            float clamped = Clamp(value, 0f, 1f);
            return clamped * clamped * (3f - 2f * clamped);
        }
    }
}
