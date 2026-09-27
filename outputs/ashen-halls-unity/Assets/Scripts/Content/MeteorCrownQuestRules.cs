using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public static class MeteorCrownQuestRules
    {
        public const string Title = "The Last Dawn";
        public const string SentriesStyle = "meteor-crown-sentries";
        public const string HeartStyle = "meteor-crown-heart";
        public const string SovereignStyle = "meteor-crown-sovereign";
        public const string SentriesSite = "red-gate-seal";
        public const string HeartSite = "glass-lore-library";
        public const string SovereignSite = "salt-cistern-gate";

        public static bool CanAccept(IReadOnlyCollection<string> flags)
        {
            return ContentSetCatalog.RedGateComplete(flags) && Has(flags, StoryFlags.RedGateDebriefed);
        }

        public static bool Accepted(IReadOnlyCollection<string> flags) => Has(flags, StoryFlags.MeteorCrownAssaultAccepted);
        public static bool Complete(IReadOnlyCollection<string> flags) => Has(flags, StoryFlags.MeteorCrownDefeated);
        public static bool SentriesDefeated(IReadOnlyCollection<string> flags) => Has(flags, StoryFlags.MeteorCrownSentriesDefeated);
        public static bool HeartBroken(IReadOnlyCollection<string> flags) => Has(flags, StoryFlags.MeteorCrownHeartBroken);

        public static bool TryAccept(List<string> flags)
        {
            if (flags == null || !CanAccept(flags) || Accepted(flags)) return false;
            flags.Add(StoryFlags.MeteorCrownAssaultAccepted);
            return true;
        }

        public static bool CanFight(IReadOnlyCollection<string> flags, string style)
        {
            if (!Accepted(flags) || !CanAccept(flags) || !Has(flags, StoryFlags.MeteorCrownEntered) || Complete(flags)) return false;
            if (Same(style, SentriesStyle)) return !SentriesDefeated(flags);
            if (Same(style, HeartStyle)) return SentriesDefeated(flags) && !HeartBroken(flags);
            if (Same(style, SovereignStyle)) return SentriesDefeated(flags) && HeartBroken(flags);
            return false;
        }

        public static bool TryRecordVictory(List<string> flags, string style)
        {
            if (!CanFight(flags, style)) return false;
            flags.Add(Same(style, SentriesStyle) ? StoryFlags.MeteorCrownSentriesDefeated
                : Same(style, HeartStyle) ? StoryFlags.MeteorCrownHeartBroken : StoryFlags.MeteorCrownDefeated);
            return true;
        }

        public static bool IsEncounter(string style) => Same(style, SentriesStyle) || Same(style, HeartStyle) || Same(style, SovereignStyle);

        public static string CurrentSite(IReadOnlyCollection<string> flags)
        {
            if (!Accepted(flags) || Complete(flags)) return "";
            return !SentriesDefeated(flags) ? SentriesSite : !HeartBroken(flags) ? HeartSite : SovereignSite;
        }

        private static bool Same(string left, string right) => string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
        private static bool Has(IReadOnlyCollection<string> flags, string flag)
        {
            if (flags == null) return false;
            foreach (string value in flags) if (value == flag) return true;
            return false;
        }
    }
}
