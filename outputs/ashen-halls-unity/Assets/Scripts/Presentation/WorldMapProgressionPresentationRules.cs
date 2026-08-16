using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public static class WorldMapProgressionPresentationRules
    {
        public const int LockedGateIcon = 11;
        public const int ClearedSiteIcon = 12;
        public const int RedGateSealIcon = 18;
        public const int MeteorCrownThresholdIcon = 19;

        public static int ChapterFiveSiteIcon(
            string objectId,
            IReadOnlyCollection<string> storyFlags)
        {
            if (string.IsNullOrWhiteSpace(objectId)) return -1;

            string redGateId = WorldSitePresentationRules.LandmarkObjectIdPrefix
                + WorldSitePresentationRules.RedGateSeal;
            if (string.Equals(objectId, redGateId, StringComparison.Ordinal))
            {
                return HasFlag(storyFlags, StoryFlags.RedGateVanguardDefeated)
                    ? ClearedSiteIcon
                    : RedGateSealIcon;
            }

            string cryptId = WorldSitePresentationRules.LandmarkObjectIdPrefix
                + WorldSitePresentationRules.GloamDeepCrypt;
            if (string.Equals(objectId, cryptId, StringComparison.Ordinal))
            {
                return HasFlag(storyFlags, StoryFlags.OssuaryRoadSealRecovered)
                    ? ClearedSiteIcon
                    : LockedGateIcon;
            }

            string cisternId = WorldSitePresentationRules.LandmarkObjectIdPrefix
                + WorldSitePresentationRules.SaltCisternGate;
            if (string.Equals(objectId, cisternId, StringComparison.Ordinal))
            {
                if (HasFlag(storyFlags, StoryFlags.MeteorCrownThresholdSurveyed))
                {
                    return ClearedSiteIcon;
                }
                return HasFlag(storyFlags, StoryFlags.CrownroadMarshalDefeated)
                    ? MeteorCrownThresholdIcon
                    : LockedGateIcon;
            }

            return -1;
        }

        private static bool HasFlag(IReadOnlyCollection<string> storyFlags, string flag)
        {
            if (storyFlags == null) return false;
            foreach (string candidate in storyFlags)
            {
                if (string.Equals(candidate, flag, StringComparison.Ordinal)) return true;
            }
            return false;
        }
    }
}
