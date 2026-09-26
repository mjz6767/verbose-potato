using System;

namespace AshenHalls
{
    public static class DemonicPowerSfxRules
    {
        public static bool IsDemonic(string key)
        {
            switch ((key ?? "").ToLowerInvariant())
            {
                case "rkw": case "rnh": case "wbk": case "nvl": case "rms": case "inh":
                case "rmb": case "wbp": case "grh": case "rpx": case "dmc": case "wtr":
                case "dsm": case "rlm": case "rbt": case "ibd": case "slv": case "pbr":
                case "ibf": case "vrs": case "ibg": case "dfa": case "acr":
                case "riftpounce": case "abyssalwhirl": case "soulrend": case "dreadroar":
                    return true;
                default: return false;
            }
        }

        public static CombatPowerSfxProfile Enhance(CombatPowerSfxProfile profile)
        {
            if (!IsDemonic(profile.Key)) return profile;
            string cast = profile.Cast.Key;
            string release = profile.Release.Key;
            string impact = profile.Impact.Key;
            string tail = profile.Aftershock.Key;
            switch (cast)
            {
                case "casthex": cast = "demonwhisper"; break;
                case "castpact": cast = "demoninvoke"; break;
                case "castdeathburst": cast = "demonsoulgather"; break;
                case "castgreatersummon": cast = "demongateinvoke"; break;
                case "castascendance": cast = "demonascendcast"; break;
                case "riftpounce": cast = "demonpouncecast"; break;
                case "abyssalwhirl": cast = "demonwhirlcast"; break;
                case "soulrend": cast = "demonsoulgather"; break;
                case "dreadroar": cast = "demonroarcast"; break;
            }
            if (release == "releasehex") release = "demonhexrelease";
            if (release == "releasepact") release = "demonriftrelease";
            switch (profile.Key)
            {
                case "rlm": impact = "demondeathburst"; tail = "demonsoultail"; break;
                case "rbt": impact = "demonriftbolt"; tail = "demonrifttail"; break;
                case "ibd": impact = "demonimpbreach"; tail = "demonrifttail"; break;
                case "ibf": impact = "demonlesserbreach"; tail = "demongatetail"; break;
                case "ibg": impact = "demongreaterbreach"; tail = "demongatetail"; break;
                case "dfa": impact = "demonascendance"; tail = "demonchoir"; break;
                case "inh": impact = "demonsoulpull"; break;
                case "grh": impact = "demonsoulpull"; tail = "demonsoultail"; break;
                case "dmc": case "pbr": impact = "demonbrand"; tail = "demonchoir"; break;
                case "slv": impact = "demonveil"; tail = "demonrifttail"; break;
                case "vrs": impact = "demonriftstep"; tail = "demonrifttail"; break;
                case "riftpounce": impact = "demonpouncehit"; tail = "demongatetail"; break;
                case "abyssalwhirl": impact = "demonwhirlhit"; tail = "demonsoultail"; break;
                case "soulrend": impact = "demonrendhit"; tail = "demonsoultail"; break;
                case "dreadroar": impact = "demonroarhit"; tail = "demonchoir"; break;
            }
            return new CombatPowerSfxProfile(profile.Key, profile.Intensity,
                Rekey(profile.Cast, cast), Rekey(profile.Release, release),
                Rekey(profile.Impact, impact), Rekey(profile.Aftershock, tail),
                profile.LayerLowHit, profile.LayerRumble, profile.LayerShimmer);
        }

        public static string ReducedCue(string key)
        {
            return IsSignatureImpact(key) ? key + "compact" : key;
        }

        public static bool IsSignatureImpact(string key)
        {
            switch (key)
            {
                case "demondeathburst": case "demonriftbolt": case "demonimpbreach":
                case "demonlesserbreach": case "demongreaterbreach": case "demonascendance":
                case "demonsoulpull": case "demonbrand": case "demonveil": case "demonriftstep":
                case "demonpouncehit": case "demonwhirlhit": case "demonrendhit": case "demonroarhit":
                    return true;
                default: return false;
            }
        }

        private static CombatPowerSfxCuePlan Rekey(CombatPowerSfxCuePlan cue, string key)
        {
            return cue.Enabled ? new CombatPowerSfxCuePlan(cue.Phase, key, cue.Delay, cue.Gain, cue.Pitch) : cue;
        }
    }
}
