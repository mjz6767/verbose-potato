using System;

namespace AshenHalls
{
    public enum CombatPowerTravelPath
    {
        None,
        Straight,
        Arc,
        Vertical,
        Tether,
        Chain,
        Dash,
        Teleport,
        Rain
    }

    public readonly struct CombatPowerTravelVfxProfile
    {
        public readonly string Key;
        public readonly int AtlasCell;
        public readonly CombatPowerTravelPath Path;
        public readonly float BaseScale;
        public readonly float BaseOpacity;
        public readonly float DurationSeconds;
        public readonly int TrailSampleCount;

        public bool Supported => AtlasCell >= 0 && Path != CombatPowerTravelPath.None;
        public bool HasTravel => Supported && DurationSeconds > 0f && TrailSampleCount > 0;

        public CombatPowerTravelVfxProfile(
            string key,
            int atlasCell,
            CombatPowerTravelPath path,
            float baseScale,
            float baseOpacity,
            float durationSeconds,
            int trailSampleCount)
        {
            Key = string.IsNullOrEmpty(key) ? "power" : key;
            AtlasCell = CombatPowerTravelVfxRules.IsAtlasCell(atlasCell) ? atlasCell : -1;
            Path = AtlasCell < 0 ? CombatPowerTravelPath.None : path;
            if (Path == CombatPowerTravelPath.None) AtlasCell = -1;
            BaseScale = AtlasCell < 0 ? 0f : Clamp(baseScale, CombatPowerTravelVfxRules.MinimumScale, CombatPowerTravelVfxRules.MaximumScale);
            BaseOpacity = AtlasCell < 0 ? 0f : Clamp(baseOpacity, CombatPowerTravelVfxRules.MinimumOpacity, CombatPowerTravelVfxRules.MaximumOpacity);
            DurationSeconds = AtlasCell < 0 ? 0f : Clamp(durationSeconds, CombatPowerTravelVfxRules.MinimumDurationSeconds, CombatPowerTravelVfxRules.MaximumDurationSeconds);
            TrailSampleCount = AtlasCell < 0
                ? 0
                : Math.Max(CombatPowerTravelVfxRules.MinimumTrailSamples, Math.Min(CombatPowerTravelVfxRules.MaximumTrailSamples, trailSampleCount));
        }

        private static float Clamp(float value, float minimum, float maximum)
        {
            return Math.Max(minimum, Math.Min(maximum, value));
        }
    }

    public readonly struct CombatPowerTravelVfxPlan
    {
        public readonly string Key;
        public readonly int AtlasCell;
        public readonly CombatPowerTravelPath Path;
        public readonly float Scale;
        public readonly float Opacity;
        public readonly float DurationSeconds;
        public readonly int TrailSampleCount;
        public readonly float Progress;
        public readonly int StableSeed;
        public readonly float LateralJitter;
        public readonly float SpinDegrees;

        public bool Supported => AtlasCell >= 0 && Path != CombatPowerTravelPath.None;
        public bool HasTravel => Supported && DurationSeconds > 0f && TrailSampleCount > 0;

        public CombatPowerTravelVfxPlan(
            string key,
            int atlasCell,
            CombatPowerTravelPath path,
            float scale,
            float opacity,
            float durationSeconds,
            int trailSampleCount,
            float progress,
            int stableSeed,
            float lateralJitter,
            float spinDegrees)
        {
            Key = string.IsNullOrEmpty(key) ? "power" : key;
            AtlasCell = CombatPowerTravelVfxRules.IsAtlasCell(atlasCell) ? atlasCell : -1;
            Path = AtlasCell < 0 ? CombatPowerTravelPath.None : path;
            if (Path == CombatPowerTravelPath.None) AtlasCell = -1;
            Scale = AtlasCell < 0 ? 0f : Clamp(scale, CombatPowerTravelVfxRules.MinimumScale, CombatPowerTravelVfxRules.MaximumPlanScale);
            Opacity = AtlasCell < 0 ? 0f : Clamp(opacity, CombatPowerTravelVfxRules.MinimumOpacity, CombatPowerTravelVfxRules.MaximumOpacity);
            DurationSeconds = AtlasCell < 0 ? 0f : Clamp(durationSeconds, CombatPowerTravelVfxRules.MinimumDurationSeconds, CombatPowerTravelVfxRules.MaximumDurationSeconds);
            TrailSampleCount = AtlasCell < 0
                ? 0
                : Math.Max(CombatPowerTravelVfxRules.MinimumTrailSamples, Math.Min(CombatPowerTravelVfxRules.MaximumPlanTrailSamples, trailSampleCount));
            Progress = AtlasCell < 0 ? 0f : Clamp(progress, 0f, 1f);
            StableSeed = AtlasCell < 0 ? 0 : Math.Max(0, stableSeed);
            LateralJitter = AtlasCell < 0 ? 0f : Clamp(lateralJitter, -1f, 1f);
            SpinDegrees = AtlasCell < 0 ? 0f : Clamp(spinDegrees, -180f, 180f);
        }

        private static float Clamp(float value, float minimum, float maximum)
        {
            return Math.Max(minimum, Math.Min(maximum, value));
        }
    }

    public static class CombatPowerTravelVfxRules
    {
        public const int AtlasColumns = 4;
        public const int AtlasRows = 4;
        public const int AtlasCellCount = AtlasColumns * AtlasRows;

        public const int FireballCometCell = 0;
        public const int MeteorVerticalStreakCell = 1;
        public const int FrostLanceCell = 2;
        public const int LightningLeaderCell = 3;
        public const int MendingWispCell = 4;
        public const int SunLanceCell = 5;
        public const int RiftBoltCell = 6;
        public const int SoulDrainTetherCell = 7;
        public const int WebBolaNetCell = 8;
        public const int PoisonVialVaporCell = 9;
        public const int SleepCrescentMistCell = 10;
        public const int GraveHookChainCell = 11;
        public const int ChargeDashCell = 12;
        public const int ThrownKnifeCell = 13;
        public const int ShadowstepTeleportTrailCell = 14;
        public const int RangerArrowsVolleyCell = 15;

        public const float MinimumScale = 0.35f;
        public const float MaximumScale = 2.20f;
        public const float MaximumPlanScale = 2.40f;
        public const float MinimumOpacity = 0.15f;
        public const float MaximumOpacity = 1f;
        public const float MinimumDurationSeconds = 0.08f;
        public const float MaximumDurationSeconds = 1.25f;
        public const int MinimumTrailSamples = 1;
        public const int MaximumTrailSamples = 16;
        public const int MaximumPlanTrailSamples = 20;

        public static bool IsAtlasCell(int cell)
        {
            return cell >= 0 && cell < AtlasCellCount;
        }

        public static bool IsKnownFormula(string formulaCodeOrName)
        {
            switch (NormalizeFormulaKey(formulaCodeOrName))
            {
                case "GBH": case "GBX": case "HLC": case "OIC": case "NVC": case "SRF": case "TBQ": case "SGW":
                case "TNC": case "LBC": case "TBG": case "OBL": case "LNH": case "SWR": case "SBN": case "FIF":
                case "WBF": case "BTF": case "WBI": case "RCL": case "RDF": case "RIG": case "FBL": case "RLF":
                case "RSG": case "RBI": case "MTR": case "CLT": case "FRB": case "VST": case "AST": case "WBK":
                case "WBP": case "DMC": case "RMS": case "RNH": case "NVL": case "RKW": case "RPX": case "INH":
                case "RMB": case "RLM": case "WTR": case "DSM": case "IBD": case "IBF": case "PBR": case "IBG":
                case "DFA": case "RBT": case "VRS": case "DWP": case "CNS": case "GRH": case "SLV": case "ACR":
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsKnownAbility(string abilityIdOrName)
        {
            switch (NormalizeAbilityKey(abilityIdOrName))
            {
                case "charge": case "execute": case "shieldbash": case "rally": case "cleave": case "whirlwind": case "sunder":
                case "stealth": case "ambush": case "throwknife": case "smokebomb": case "hamstring": case "eviscerate": case "shadowstep":
                case "aimedshot": case "pinningshot": case "volley": case "scoutmark": case "broadheadshot": case "disruptingshot": case "quickshot":
                case "riftpounce": case "abyssalwhirl": case "soulrend": case "dreadroar":
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsSupportedFormula(string formulaCodeOrName)
        {
            return ProfileForFormula(formulaCodeOrName).HasTravel;
        }

        public static bool IsSupportedAbility(string abilityIdOrName)
        {
            return ProfileForAbility(abilityIdOrName).HasTravel;
        }

        public static bool IsSupported(string powerKeyOrName)
        {
            return ProfileFor(powerKeyOrName).HasTravel;
        }

        public static string NormalizeFormulaKey(string formulaCodeOrName)
        {
            string key = StripPresentationDecorators(Compact(formulaCodeOrName));
            switch (key)
            {
                case "gbh": case "treecover": return "GBH";
                case "gbx": case "stoneblock": return "GBX";
                case "hlc": case "hallowedcircle": return "HLC";
                case "oic": case "heal": return "OIC";
                case "nvc": case "cleanse": return "NVC";
                case "srf": case "riftseal": return "SRF";
                case "tbq": case "ward": return "TBQ";
                case "sgw": case "sanctuaryward": return "SGW";
                case "tnc": case "regenerate": case "regeneration": return "TNC";
                case "lbc": case "circleheal": return "LBC";
                case "tbg": case "circleward": return "TBG";
                case "obl": case "lightbolt": return "OBL";
                case "lnh": case "holdsign": return "LNH";
                case "swr": case "stillwater": return "SWR";
                case "sbn": case "sunbrand": return "SBN";
                case "fif": case "firespark": return "FIF";
                case "wbf": case "firefloor": return "WBF";
                case "btf": case "burncover": return "BTF";
                case "wbi": case "iceslick": return "WBI";
                case "rcl": case "coldlance": case "frostlance": return "RCL";
                case "rdf": case "flamejet": return "RDF";
                case "rig": case "arcspark": return "RIG";
                case "fbl": case "fireball": return "FBL";
                case "rlf": case "fireburst": return "RLF";
                case "rsg": case "thunderclap": return "RSG";
                case "rbi": case "iceburst": return "RBI";
                case "mtr": case "meteorshower": case "meteor": return "MTR";
                case "clt": case "chainlightning": return "CLT";
                case "frb": case "frostbind": return "FRB";
                case "vst": case "thunderstep": return "VST";
                case "ast": case "arcanetempest": return "AST";
                case "wbk": case "websnare": return "WBK";
                case "wbp": case "poisongas": return "WBP";
                case "dmc": case "doomcircle": return "DMC";
                case "rms": case "sleep": return "RMS";
                case "rnh": case "weaken": return "RNH";
                case "nvl": case "nightveil": return "NVL";
                case "rkw": case "bind": case "webbind": return "RKW";
                case "rpx": case "poisonburst": return "RPX";
                case "inh": case "drainlife": case "lifedrain": return "INH";
                case "rmb": case "mindbreak": return "RMB";
                case "rlm": case "deathburst": return "RLM";
                case "wtr": case "wither": return "WTR";
                case "dsm": case "dreamsmoke": return "DSM";
                case "ibd": case "summonimp": return "IBD";
                case "ibf": case "summonlesserdemon": return "IBF";
                case "pbr": case "pactbrand": return "PBR";
                case "ibg": case "summongreaterdemon": return "IBG";
                case "dfa": case "abyssalascendance": return "DFA";
                case "rbt": case "riftbolt": return "RBT";
                case "vrs": case "riftstep": return "VRS";
                case "dwp": case "dawnpulse": return "DWP";
                case "cns": case "cinderstorm": return "CNS";
                case "grh": case "gravehook": return "GRH";
                case "slv": case "soulveil": return "SLV";
                case "acr": case "ashencurse": return "ACR";
                default: return key.ToUpperInvariant();
            }
        }

        public static string NormalizeAbilityKey(string abilityIdOrName)
        {
            string key = StripPresentationDecorators(Compact(abilityIdOrName));
            switch (key)
            {
                case "charge": case "warriorcharge": case "rush": return "charge";
                case "execute": case "execution": return "execute";
                case "shieldbash": case "shieldslam": return "shieldbash";
                case "rally": case "battlecry": return "rally";
                case "cleave": case "spinningcleave": return "cleave";
                case "whirlwind": case "bladecyclone": return "whirlwind";
                case "sunder": case "armorsunder": return "sunder";
                case "stealth": case "vanish": return "stealth";
                case "ambush": case "backstab": return "ambush";
                case "throwknife": case "thrownknife": case "knifethrow": return "throwknife";
                case "smokebomb": case "smokecloud": return "smokebomb";
                case "hamstring": case "hamstringstrike": return "hamstring";
                case "eviscerate": case "deepcut": return "eviscerate";
                case "shadowstep": case "shadowstrike": return "shadowstep";
                case "aimedshot": return "aimedshot";
                case "pinningshot": case "pinningarrow": return "pinningshot";
                case "volley": case "arrowvolley": return "volley";
                case "scoutmark": case "huntersmark": return "scoutmark";
                case "broadheadshot": return "broadheadshot";
                case "disruptingshot": case "interruptingshot": return "disruptingshot";
                case "quickshot": case "doubleshot": return "quickshot";
                case "riftpounce": case "demonpounce": return "riftpounce";
                case "abyssalwhirl": case "abyssalwhirlwind": return "abyssalwhirl";
                case "soulrend": case "liferip": return "soulrend";
                case "dreadroar": case "demonroar": return "dreadroar";
                default: return key;
            }
        }

        public static CombatPowerTravelVfxProfile ProfileFor(string powerKeyOrName)
        {
            if (IsKnownFormula(powerKeyOrName)) return ProfileForFormula(powerKeyOrName);
            if (IsKnownAbility(powerKeyOrName)) return ProfileForAbility(powerKeyOrName);
            return NoTravel(string.IsNullOrEmpty(powerKeyOrName) ? "power" : powerKeyOrName);
        }

        public static CombatPowerTravelVfxProfile ProfileForFormula(string formulaCodeOrName)
        {
            string key = NormalizeFormulaKey(formulaCodeOrName);
            switch (key)
            {
                // Mend: tile creation and persistent ground fields intentionally have no travel sprite.
                case "GBH": return NoTravel("GBH");
                case "GBX": return NoTravel("GBX");
                case "HLC": return NoTravel("HLC");
                case "OIC": return Profile("OIC", MendingWispCell, CombatPowerTravelPath.Straight, 0.72f, 0.92f, 0.34f, 6);
                case "NVC": return Profile("NVC", MendingWispCell, CombatPowerTravelPath.Straight, 0.70f, 0.90f, 0.32f, 5);
                case "SRF": return NoTravel("SRF");
                case "TBQ": return Profile("TBQ", MendingWispCell, CombatPowerTravelPath.Arc, 0.76f, 0.92f, 0.36f, 6);
                case "SGW": return Profile("SGW", MendingWispCell, CombatPowerTravelPath.Arc, 0.84f, 0.95f, 0.42f, 8);
                case "TNC": return Profile("TNC", MendingWispCell, CombatPowerTravelPath.Straight, 0.72f, 0.90f, 0.36f, 6);
                case "LBC": return Profile("LBC", MendingWispCell, CombatPowerTravelPath.Arc, 0.82f, 0.96f, 0.42f, 8);
                case "TBG": return Profile("TBG", MendingWispCell, CombatPowerTravelPath.Arc, 0.86f, 0.96f, 0.44f, 8);
                case "OBL": return Profile("OBL", SunLanceCell, CombatPowerTravelPath.Straight, 0.86f, 0.98f, 0.30f, 7);
                case "LNH": return Profile("LNH", SunLanceCell, CombatPowerTravelPath.Straight, 0.82f, 0.96f, 0.32f, 7);
                case "SWR": return Profile("SWR", MendingWispCell, CombatPowerTravelPath.Arc, 0.86f, 0.94f, 0.44f, 8);
                case "SBN": return Profile("SBN", SunLanceCell, CombatPowerTravelPath.Arc, 1.02f, 1f, 0.42f, 10);

                // Ember: ground hazards and self-centered blasts remain impact-only.
                case "FIF": return Profile("FIF", FireballCometCell, CombatPowerTravelPath.Straight, 0.72f, 0.94f, 0.28f, 6);
                case "WBF": return NoTravel("WBF");
                case "BTF": return NoTravel("BTF");
                case "WBI": return NoTravel("WBI");
                case "RCL": return Profile("RCL", FrostLanceCell, CombatPowerTravelPath.Straight, 0.88f, 0.98f, 0.34f, 8);
                case "RDF": return Profile("RDF", FireballCometCell, CombatPowerTravelPath.Straight, 0.88f, 0.98f, 0.34f, 8);
                case "RIG": return Profile("RIG", LightningLeaderCell, CombatPowerTravelPath.Straight, 0.84f, 0.98f, 0.24f, 6);
                case "FBL": return Profile("FBL", FireballCometCell, CombatPowerTravelPath.Arc, 1.12f, 1f, 0.48f, 12);
                case "RLF": return Profile("RLF", FireballCometCell, CombatPowerTravelPath.Straight, 1.00f, 1f, 0.36f, 9);
                case "RSG": return NoTravel("RSG");
                case "RBI": return Profile("RBI", FrostLanceCell, CombatPowerTravelPath.Straight, 1.00f, 1f, 0.38f, 9);
                case "MTR": return Profile("MTR", MeteorVerticalStreakCell, CombatPowerTravelPath.Rain, 1.18f, 1f, 0.62f, 16);
                case "CLT": return Profile("CLT", LightningLeaderCell, CombatPowerTravelPath.Chain, 1.02f, 1f, 0.38f, 12);
                case "FRB": return Profile("FRB", FrostLanceCell, CombatPowerTravelPath.Straight, 0.96f, 1f, 0.36f, 9);
                case "VST": return Profile("VST", ShadowstepTeleportTrailCell, CombatPowerTravelPath.Teleport, 1.04f, 1f, 0.36f, 10);
                case "AST": return Profile("AST", LightningLeaderCell, CombatPowerTravelPath.Rain, 1.22f, 1f, 0.58f, 16);

                // Hex: physicalized bolas, vapor, soul tethers, and hook chains receive distinct paths.
                case "WBK": return NoTravel("WBK");
                case "WBP": return NoTravel("WBP");
                case "DMC": return NoTravel("DMC");
                case "RMS": return Profile("RMS", SleepCrescentMistCell, CombatPowerTravelPath.Arc, 0.88f, 0.94f, 0.42f, 9);
                case "RNH": return Profile("RNH", SleepCrescentMistCell, CombatPowerTravelPath.Straight, 0.78f, 0.90f, 0.36f, 7);
                case "NVL": return Profile("NVL", SleepCrescentMistCell, CombatPowerTravelPath.Arc, 0.84f, 0.92f, 0.40f, 8);
                case "RKW": return Profile("RKW", WebBolaNetCell, CombatPowerTravelPath.Arc, 0.86f, 0.98f, 0.40f, 8);
                case "RPX": return Profile("RPX", PoisonVialVaporCell, CombatPowerTravelPath.Arc, 0.98f, 0.98f, 0.46f, 10);
                case "INH": return Profile("INH", SoulDrainTetherCell, CombatPowerTravelPath.Tether, 0.94f, 1f, 0.48f, 14);
                case "RMB": return Profile("RMB", RiftBoltCell, CombatPowerTravelPath.Straight, 0.88f, 0.98f, 0.34f, 8);
                case "RLM": return Profile("RLM", RiftBoltCell, CombatPowerTravelPath.Straight, 1.08f, 1f, 0.42f, 11);
                case "WTR": return Profile("WTR", SoulDrainTetherCell, CombatPowerTravelPath.Tether, 0.96f, 0.98f, 0.48f, 14);
                case "DSM": return Profile("DSM", SleepCrescentMistCell, CombatPowerTravelPath.Arc, 1.10f, 0.98f, 0.52f, 12);

                // Pact summons and self-transformation are cast/impact events rather than travel events.
                case "IBD": return NoTravel("IBD");
                case "IBF": return NoTravel("IBF");
                case "PBR": return Profile("PBR", RiftBoltCell, CombatPowerTravelPath.Arc, 1.00f, 1f, 0.44f, 10);
                case "IBG": return NoTravel("IBG");
                case "DFA": return NoTravel("DFA");
                case "RBT": return Profile("RBT", RiftBoltCell, CombatPowerTravelPath.Straight, 0.88f, 1f, 0.32f, 8);
                case "VRS": return Profile("VRS", ShadowstepTeleportTrailCell, CombatPowerTravelPath.Teleport, 1.06f, 1f, 0.38f, 11);

                // Later catalog additions are still listed explicitly.
                case "DWP": return Profile("DWP", MendingWispCell, CombatPowerTravelPath.Arc, 0.96f, 0.98f, 0.46f, 10);
                case "CNS": return Profile("CNS", FireballCometCell, CombatPowerTravelPath.Arc, 1.14f, 1f, 0.52f, 13);
                case "GRH": return Profile("GRH", GraveHookChainCell, CombatPowerTravelPath.Chain, 1.02f, 1f, 0.46f, 14);
                case "SLV": return Profile("SLV", RiftBoltCell, CombatPowerTravelPath.Arc, 0.94f, 0.98f, 0.44f, 10);
                case "ACR": return Profile("ACR", FireballCometCell, CombatPowerTravelPath.Arc, 1.20f, 1f, 0.54f, 14);
                default: return NoTravel(string.IsNullOrEmpty(key) ? "formula" : key);
            }
        }

        public static CombatPowerTravelVfxProfile ProfileForAbility(string abilityIdOrName)
        {
            string key = NormalizeAbilityKey(abilityIdOrName);
            switch (key)
            {
                case "charge": return Profile("charge", ChargeDashCell, CombatPowerTravelPath.Dash, 1.02f, 1f, 0.36f, 12);
                case "execute": return NoTravel("execute");
                case "shieldbash": return NoTravel("shieldbash");
                case "rally": return NoTravel("rally");
                case "cleave": return NoTravel("cleave");
                case "whirlwind": return NoTravel("whirlwind");
                case "sunder": return NoTravel("sunder");
                case "stealth": return NoTravel("stealth");
                case "ambush": return NoTravel("ambush");
                case "throwknife": return Profile("throwknife", ThrownKnifeCell, CombatPowerTravelPath.Straight, 0.74f, 1f, 0.26f, 6);
                case "smokebomb": return NoTravel("smokebomb");
                case "hamstring": return NoTravel("hamstring");
                case "eviscerate": return NoTravel("eviscerate");
                case "shadowstep": return Profile("shadowstep", ShadowstepTeleportTrailCell, CombatPowerTravelPath.Teleport, 1.00f, 1f, 0.30f, 10);
                case "aimedshot": return Profile("aimedshot", RangerArrowsVolleyCell, CombatPowerTravelPath.Straight, 0.84f, 1f, 0.28f, 7);
                case "pinningshot": return Profile("pinningshot", RangerArrowsVolleyCell, CombatPowerTravelPath.Straight, 0.82f, 1f, 0.30f, 7);
                case "volley": return Profile("volley", RangerArrowsVolleyCell, CombatPowerTravelPath.Rain, 1.04f, 1f, 0.54f, 16);
                case "scoutmark": return Profile("scoutmark", RangerArrowsVolleyCell, CombatPowerTravelPath.Straight, 0.78f, 0.96f, 0.26f, 6);
                case "broadheadshot": return Profile("broadheadshot", RangerArrowsVolleyCell, CombatPowerTravelPath.Straight, 0.90f, 1f, 0.30f, 8);
                case "disruptingshot": return Profile("disruptingshot", RangerArrowsVolleyCell, CombatPowerTravelPath.Straight, 0.88f, 1f, 0.28f, 7);
                case "quickshot": return Profile("quickshot", RangerArrowsVolleyCell, CombatPowerTravelPath.Straight, 0.80f, 1f, 0.24f, 10);
                case "riftpounce": return Profile("riftpounce", ShadowstepTeleportTrailCell, CombatPowerTravelPath.Teleport, 1.18f, 1f, 0.38f, 13);
                case "abyssalwhirl": return NoTravel("abyssalwhirl");
                case "soulrend": return Profile("soulrend", SoulDrainTetherCell, CombatPowerTravelPath.Tether, 1.08f, 1f, 0.42f, 14);
                case "dreadroar": return NoTravel("dreadroar");
                default: return NoTravel(string.IsNullOrEmpty(key) ? "ability" : key);
            }
        }

        public static CombatPowerTravelVfxPlan PlanFor(
            string powerKeyOrName,
            int intensity = 1,
            float progress = 0.35f,
            bool reducedMotion = false,
            int sampleIndex = 0,
            int stableSeed = 0)
        {
            return BuildPlan(ProfileFor(powerKeyOrName), intensity, progress, reducedMotion, sampleIndex, stableSeed);
        }

        public static CombatPowerTravelVfxPlan PlanForFormula(
            string formulaCodeOrName,
            int intensity = 1,
            float progress = 0.35f,
            bool reducedMotion = false,
            int sampleIndex = 0,
            int stableSeed = 0)
        {
            return BuildPlan(ProfileForFormula(formulaCodeOrName), intensity, progress, reducedMotion, sampleIndex, stableSeed);
        }

        public static CombatPowerTravelVfxPlan PlanForAbility(
            string abilityIdOrName,
            int intensity = 1,
            float progress = 0.35f,
            bool reducedMotion = false,
            int sampleIndex = 0,
            int stableSeed = 0)
        {
            return BuildPlan(ProfileForAbility(abilityIdOrName), intensity, progress, reducedMotion, sampleIndex, stableSeed);
        }

        public static float TravelProgress(float elapsedSeconds, float durationSeconds)
        {
            if (durationSeconds <= 0f) return 1f;
            return Clamp01(Math.Max(0f, elapsedSeconds) / durationSeconds);
        }

        public static float TrailSampleProgress(CombatPowerTravelVfxPlan plan, int trailSampleIndex)
        {
            if (!plan.HasTravel || plan.TrailSampleCount <= 1) return plan.Progress;
            int index = Math.Max(0, Math.Min(plan.TrailSampleCount - 1, trailSampleIndex));
            float spacing = 1f / (plan.TrailSampleCount - 1);
            return Clamp01(plan.Progress - index * spacing * 0.34f);
        }

        public static int StableTravelHash(string powerKeyOrName, int sampleIndex, int channel = 0)
        {
            unchecked
            {
                uint hash = 2166136261u;
                string key;
                if (IsKnownFormula(powerKeyOrName)) key = NormalizeFormulaKey(powerKeyOrName);
                else if (IsKnownAbility(powerKeyOrName)) key = NormalizeAbilityKey(powerKeyOrName);
                else key = Compact(powerKeyOrName);
                for (int i = 0; i < key.Length; i++)
                {
                    hash ^= key[i];
                    hash *= 16777619u;
                }
                hash = AppendInt(hash, sampleIndex);
                hash = AppendInt(hash, channel);
                hash ^= hash >> 16;
                hash *= 2246822519u;
                hash ^= hash >> 13;
                hash *= 3266489917u;
                hash ^= hash >> 16;
                return (int)(hash & 0x7fffffffu);
            }
        }

        public static float StableTravelSample(string powerKeyOrName, int sampleIndex, int channel = 0)
        {
            uint hash = unchecked((uint)StableTravelHash(powerKeyOrName, sampleIndex, channel));
            return (hash & 0x00ffffffu) / 16777216f;
        }

        public static float StableTravelSignedSample(string powerKeyOrName, int sampleIndex, int channel = 0)
        {
            return StableTravelSample(powerKeyOrName, sampleIndex, channel) * 2f - 1f;
        }

        private static CombatPowerTravelVfxPlan BuildPlan(
            CombatPowerTravelVfxProfile profile,
            int intensity,
            float progress,
            bool reducedMotion,
            int sampleIndex,
            int stableSeed)
        {
            // Reduced Motion deliberately suppresses travel rather than replacing it with a moving token.
            if (reducedMotion || !profile.HasTravel) return EmptyPlan(profile.Key);

            int tier = Math.Max(1, Math.Min(3, intensity));
            float t = Clamp01(progress);
            int visualSampleIndex = stableSeed == 0
                ? sampleIndex
                : MixStableSeed(sampleIndex, stableSeed);
            float pulse = 0.90f + (float)Math.Sin(t * Math.PI) * 0.10f;
            float scaleNoise = 0.97f + StableTravelSample(profile.Key, visualSampleIndex, 0) * 0.06f;
            float opacityNoise = 0.96f + StableTravelSample(profile.Key, visualSampleIndex, 1) * 0.04f;
            float scale = Clamp(profile.BaseScale * (1f + (tier - 1) * 0.10f) * pulse * scaleNoise, MinimumScale, MaximumPlanScale);
            float opacity = Clamp(profile.BaseOpacity * opacityNoise + (tier - 1) * 0.018f, MinimumOpacity, MaximumOpacity);
            int trailSamples = Math.Max(MinimumTrailSamples, Math.Min(MaximumPlanTrailSamples, profile.TrailSampleCount + (tier - 1) * 2));
            int seed = StableTravelHash(profile.Key, visualSampleIndex, 2);
            float lateralJitter = StableTravelSignedSample(profile.Key, visualSampleIndex, 3);
            float spinDegrees = StableTravelSignedSample(profile.Key, visualSampleIndex, 4) * SpinRange(profile.Path);
            return new CombatPowerTravelVfxPlan(
                profile.Key,
                profile.AtlasCell,
                profile.Path,
                scale,
                opacity,
                profile.DurationSeconds,
                trailSamples,
                t,
                seed,
                lateralJitter,
                spinDegrees);
        }

        private static int MixStableSeed(int sampleIndex, int stableSeed)
        {
            unchecked
            {
                int mixed = sampleIndex * 397 ^ stableSeed;
                mixed ^= mixed >> 16;
                mixed *= 0x45d9f3b;
                mixed ^= mixed >> 16;
                return mixed;
            }
        }

        private static float SpinRange(CombatPowerTravelPath path)
        {
            switch (path)
            {
                case CombatPowerTravelPath.Arc: return 18f;
                case CombatPowerTravelPath.Vertical: return 8f;
                case CombatPowerTravelPath.Rain: return 12f;
                case CombatPowerTravelPath.Dash: return 4f;
                case CombatPowerTravelPath.Teleport: return 24f;
                default: return 6f;
            }
        }

        private static CombatPowerTravelVfxProfile Profile(
            string key,
            int atlasCell,
            CombatPowerTravelPath path,
            float baseScale,
            float baseOpacity,
            float durationSeconds,
            int trailSampleCount)
        {
            return new CombatPowerTravelVfxProfile(
                key,
                atlasCell,
                path,
                baseScale,
                baseOpacity,
                durationSeconds,
                trailSampleCount);
        }

        private static CombatPowerTravelVfxProfile NoTravel(string key)
        {
            return new CombatPowerTravelVfxProfile(key, -1, CombatPowerTravelPath.None, 0f, 0f, 0f, 0);
        }

        private static CombatPowerTravelVfxPlan EmptyPlan(string key)
        {
            return new CombatPowerTravelVfxPlan(
                key,
                -1,
                CombatPowerTravelPath.None,
                0f,
                0f,
                0f,
                0,
                0f,
                0,
                0f,
                0f);
        }

        private static string StripPresentationDecorators(string key)
        {
            if (string.IsNullOrEmpty(key)) return "";
            string result = key;
            string[] prefixes = { "formula", "spell", "ability", "skill", "cast" };
            for (int i = 0; i < prefixes.Length; i++)
            {
                if (result.StartsWith(prefixes[i], StringComparison.Ordinal) && result.Length > prefixes[i].Length)
                {
                    result = result.Substring(prefixes[i].Length);
                    break;
                }
            }
            string[] suffixes = { "projectile", "travel", "trail", "beam" };
            for (int i = 0; i < suffixes.Length; i++)
            {
                if (result.EndsWith(suffixes[i], StringComparison.Ordinal) && result.Length > suffixes[i].Length)
                {
                    result = result.Substring(0, result.Length - suffixes[i].Length);
                    break;
                }
            }
            return result;
        }

        private static uint AppendInt(uint hash, int value)
        {
            unchecked
            {
                uint data = (uint)value;
                for (int shift = 0; shift < 32; shift += 8)
                {
                    hash ^= (byte)(data >> shift);
                    hash *= 16777619u;
                }
                return hash;
            }
        }

        private static string Compact(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            char[] buffer = new char[value.Length];
            int count = 0;
            for (int i = 0; i < value.Length; i++)
            {
                char character = value[i];
                if (!char.IsLetterOrDigit(character)) continue;
                buffer[count++] = char.ToLowerInvariant(character);
            }
            return count == 0 ? "" : new string(buffer, 0, count);
        }

        private static float Clamp01(float value)
        {
            return Math.Max(0f, Math.Min(1f, value));
        }

        private static float Clamp(float value, float minimum, float maximum)
        {
            return Math.Max(minimum, Math.Min(maximum, value));
        }
    }
}
