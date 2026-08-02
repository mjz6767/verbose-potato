using System;

namespace AshenHalls
{
    public static class CombatMusicPresentationRules
    {
        public const float TrackMinimumDwellSeconds = 6.0f;
        public const float LastStandEnterHoldSeconds = 0.90f;
        public const float LastStandExitHoldSeconds = 2.20f;
        public const float LastStandEnterHealthRatio = 0.35f;
        public const float LastStandExitHealthRatio = 0.46f;

        public static bool IsCriticalPartyHealth(int currentHp, int maximumHp)
        {
            return maximumHp > 0
                && currentHp > 0
                && currentHp <= maximumHp * LastStandEnterHealthRatio;
        }

        public static bool IsRecoveredPartyHealth(int currentHp, int maximumHp)
        {
            return maximumHp > 0
                && currentHp > 0
                && currentHp >= maximumHp * LastStandExitHealthRatio;
        }

        public static bool IsBossCombatTrack(string key)
        {
            string clean = (key ?? "").Trim().ToLowerInvariant();
            return clean == MusicDirectorRules.CombatBoss
                || clean == MusicDirectorRules.CombatKoboldKing
                || clean == MusicDirectorRules.CombatDemonLord;
        }

        public static string StableBaseTrack(string establishedKey, string candidateKey)
        {
            string established = (establishedKey ?? "").Trim().ToLowerInvariant();
            string candidate = (candidateKey ?? "").Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(established))
            {
                return string.IsNullOrEmpty(candidate) ? MusicDirectorRules.CombatGeneric : candidate;
            }

            // A boss reveal may escalate an ordinary encounter once. Once a boss
            // identity is established, faction deaths or critical HP cannot replace it.
            if (!IsBossCombatTrack(established) && IsBossCombatTrack(candidate)) return candidate;
            // Combat state can briefly exist before its unit roster is populated.
            // Let the first authored non-generic identity replace that bootstrap cue,
            // then keep the established encounter score stable afterward.
            if (established == MusicDirectorRules.CombatGeneric
                && !string.IsNullOrEmpty(candidate)
                && candidate != MusicDirectorRules.CombatGeneric)
            {
                return candidate;
            }
            return established;
        }

        public static bool ShouldEnterLastStand(
            string baseTrackKey,
            bool currentlyActive,
            int currentHp,
            int maximumHp,
            float criticalHeldSeconds,
            float selectedTrackDwellSeconds)
        {
            if (currentlyActive || IsBossCombatTrack(baseTrackKey)) return false;
            return IsCriticalPartyHealth(currentHp, maximumHp)
                && criticalHeldSeconds >= LastStandEnterHoldSeconds
                && selectedTrackDwellSeconds >= TrackMinimumDwellSeconds;
        }

        public static bool ShouldExitLastStand(
            bool currentlyActive,
            int currentHp,
            int maximumHp,
            float recoveredHeldSeconds,
            float selectedTrackDwellSeconds)
        {
            if (!currentlyActive) return false;
            return IsRecoveredPartyHealth(currentHp, maximumHp)
                && recoveredHeldSeconds >= LastStandExitHoldSeconds
                && selectedTrackDwellSeconds >= TrackMinimumDwellSeconds;
        }
    }
}
