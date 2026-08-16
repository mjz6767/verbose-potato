using System;

namespace AshenHalls
{
    public enum CombatPowerSfxPhase
    {
        Cast,
        Release,
        Impact,
        Aftershock,
        Accent
    }

    public readonly struct CombatPowerSfxCuePlan
    {
        public readonly CombatPowerSfxPhase Phase;
        public readonly string Key;
        public readonly float Delay;
        public readonly float Gain;
        public readonly float Pitch;

        public CombatPowerSfxCuePlan(
            CombatPowerSfxPhase phase,
            string key,
            float delay,
            float gain,
            float pitch)
        {
            Phase = phase;
            Key = (key ?? "").Trim().ToLowerInvariant();
            Delay = Clamp(delay, 0f, 0.60f);
            Gain = Clamp(gain, 0f, 1.40f);
            Pitch = Clamp(pitch, 0.90f, 1.10f);
        }

        public bool Enabled => Key.Length > 0 && Gain > 0f;

        public static CombatPowerSfxCuePlan None(CombatPowerSfxPhase phase)
        {
            return new CombatPowerSfxCuePlan(phase, "", 0f, 0f, 1f);
        }

        private static float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }

    public readonly struct CombatPowerSfxProfile
    {
        public readonly string Key;
        public readonly int Intensity;
        public readonly CombatPowerSfxCuePlan Cast;
        public readonly CombatPowerSfxCuePlan Release;
        public readonly CombatPowerSfxCuePlan Impact;
        public readonly CombatPowerSfxCuePlan Aftershock;
        public readonly bool LayerLowHit;
        public readonly bool LayerRumble;
        public readonly bool LayerShimmer;

        public CombatPowerSfxProfile(
            string key,
            int intensity,
            CombatPowerSfxCuePlan cast,
            CombatPowerSfxCuePlan release,
            CombatPowerSfxCuePlan impact,
            CombatPowerSfxCuePlan aftershock,
            bool layerLowHit,
            bool layerRumble,
            bool layerShimmer)
        {
            Key = string.IsNullOrWhiteSpace(key) ? "power" : key.Trim().ToLowerInvariant();
            Intensity = ClampIntensity(intensity);
            Cast = cast;
            Release = release;
            Impact = impact;
            Aftershock = aftershock;
            LayerLowHit = layerLowHit;
            LayerRumble = layerRumble;
            LayerShimmer = layerShimmer;
        }

        private static int ClampIntensity(int intensity)
        {
            return Math.Max(1, Math.Min(3, intensity));
        }
    }

    public readonly struct CombatPowerSfxPlan
    {
        public readonly string ProfileKey;
        public readonly int Intensity;
        public readonly bool ReducedAudio;
        public readonly CombatPowerSfxCuePlan Cast;
        public readonly CombatPowerSfxCuePlan Release;
        public readonly CombatPowerSfxCuePlan Impact;
        public readonly CombatPowerSfxCuePlan Aftershock;
        public readonly CombatPowerSfxCuePlan LowHit;
        public readonly CombatPowerSfxCuePlan Rumble;
        public readonly CombatPowerSfxCuePlan Shimmer;

        public CombatPowerSfxPlan(
            string profileKey,
            int intensity,
            bool reducedAudio,
            CombatPowerSfxCuePlan cast,
            CombatPowerSfxCuePlan release,
            CombatPowerSfxCuePlan impact,
            CombatPowerSfxCuePlan aftershock,
            CombatPowerSfxCuePlan lowHit,
            CombatPowerSfxCuePlan rumble,
            CombatPowerSfxCuePlan shimmer)
        {
            ProfileKey = string.IsNullOrWhiteSpace(profileKey) ? "power" : profileKey.Trim().ToLowerInvariant();
            Intensity = Math.Max(1, Math.Min(3, intensity));
            ReducedAudio = reducedAudio;
            Cast = cast;
            Release = release;
            Impact = impact;
            Aftershock = aftershock;
            LowHit = lowHit;
            Rumble = rumble;
            Shimmer = shimmer;
        }

        public bool UsesLowHit => LowHit.Enabled;
        public bool UsesRumble => Rumble.Enabled;
        public bool UsesShimmer => Shimmer.Enabled;

        public int CoreCueCount => Enabled(Cast) + Enabled(Release) + Enabled(Impact) + Enabled(Aftershock);
        public int AccentCueCount => Enabled(LowHit) + Enabled(Rumble) + Enabled(Shimmer);
        public int CueCount => CoreCueCount + AccentCueCount;

        private static int Enabled(CombatPowerSfxCuePlan cue)
        {
            return cue.Enabled ? 1 : 0;
        }
    }

    public static class CombatPowerSfxRules
    {
        public const int MendFormulaProfileCount = 16;
        public const int MageFormulaProfileCount = 17;
        public const int WarlockFormulaProfileCount = 22;
        public const int CrossSchoolFormulaProfileCount = 1;
        public const int FormulaProfileCount = MendFormulaProfileCount
            + MageFormulaProfileCount
            + WarlockFormulaProfileCount
            + CrossSchoolFormulaProfileCount;
        public const int AbilityProfileCount = 25;

        public const string GenericSpellReleaseCue = "spellrelease";
        public const string LowHitCue = "impactlow";
        public const string RumbleCue = "impactlow";
        public const string ShimmerCue = "castshimmer";

        public static CombatPowerSfxProfile ProfileForFormula(string formulaCodeOrName)
        {
            switch (NormalizeFormulaKey(formulaCodeOrName))
            {
                // Mend / support: living cover, restorative light, wards, cleansing, and sun magic.
                case "gbh": return Spell("gbh", "castnature", "tree", "ward", 2, 0.31f, 0.96f, false, true, true);
                case "gbx": return Spell("gbx", "castnature", "stone", "breakcover", 2, 0.28f, 0.90f, true, true, false);
                case "hlc": return Spell("hlc", "castmend", "fieldholy", "ward", 2, 0.32f, 1.02f, false, false, true);
                case "oic": return Spell("oic", "castmend", "heal", "light", 1, 0.24f, 1.04f, false, false, true);
                case "nvc": return Spell("nvc", "castlight", "light", "status", 1, 0.20f, 1.07f, false, false, true);
                case "srf": return Spell("srf", "castseal", "riftseal", "resonance", 2, 0.34f, 0.98f, true, false, true);
                case "tbq": return Spell("tbq", "castmend", "ward", "fieldholy", 1, 0.23f, 1.02f, false, false, true);
                case "sgw": return Spell("sgw", "castmend", "ward", "fieldholy", 2, 0.31f, 0.98f, false, false, true);
                case "tnc": return Spell("tnc", "castnature", "heal", "tree", 2, 0.29f, 0.97f, false, false, true);
                case "lbc": return Spell("lbc", "castmend", "heal", "fieldholy", 3, 0.35f, 1.00f, false, false, true, 0.04f);
                case "tbg": return Spell("tbg", "castmend", "ward", "fieldholy", 3, 0.36f, 0.96f, false, true, true, 0.04f);
                case "obl": return Spell("obl", "castlight", "light", "resonance", 2, 0.26f, 1.06f, true, false, true);
                case "lnh": return Spell("lnh", "castlight", "light", "status", 2, 0.27f, 1.02f, true, false, true);
                case "swr": return Spell("swr", "castnature", "heal", "ward", 3, 0.38f, 0.93f, false, false, true, 0.04f);
                case "sbn": return Spell("sbn", "castlight", "light", "fieldholy", 3, 0.38f, 1.00f, true, true, true, 0.06f);
                case "dwp": return Spell("dwp", "castmend", "heal", "fieldholy", 3, 0.37f, 1.03f, false, true, true, 0.06f);

                // Mage / Ember: compact cantrips grow into long-travel canonical powers.
                case "fif": return Spell("fif", "castember", "fire", "", 1, 0.16f, 1.05f, false, false, false);
                case "rig": return Spell("rig", "castshock", "shock", "", 1, 0.12f, 1.04f, false, false, false);
                case "wbi": return Spell("wbi", "castfrost", "fieldice", "ice", 1, 0.18f, 1.05f, false, false, false);
                case "wbf": return Spell("wbf", "castember", "fieldfire", "fire", 1, 0.20f, 0.98f, false, false, false);
                case "rcl": return Spell("rcl", "castfrost", "ice", "fieldice", 1, 0.24f, 1.04f, false, false, false);
                case "btf": return Spell("btf", "castember", "fieldfire", "breakcover", 1, 0.26f, 0.96f, true, false, false);
                case "fbl": return Spell("fbl", "castember", "fireball", "fieldfire", 3, 0.40f, 0.98f, true, true, true, 0.06f);
                case "rdf": return Spell("rdf", "castember", "fire", "bladecontact", 1, 0.24f, 1.02f, false, false, false);
                case "rsg": return Spell("rsg", "castshock", "shock", "resonance", 2, 0.15f, 0.97f, true, true, true);
                case "frb": return Spell("frb", "castfrost", "ice", "fieldsnare", 1, 0.26f, 1.02f, false, false, false);
                case "rbi": return Spell("rbi", "castfrost", "ice", "fieldice", 2, 0.30f, 0.98f, true, false, true);
                case "rlf": return Spell("rlf", "castember", "fire", "fieldfire", 2, 0.30f, 1.00f, true, false, false);
                case "clt": return Spell("clt", "castshock", "shock", "resonance", 2, 0.18f, 1.02f, true, false, true);
                case "mtr": return Spell("mtr", "castember", "meteor", "fieldfire", 3, 0.46f, 0.92f, true, true, true, 0.10f);
                case "cns": return Spell("cns", "castember", "fieldfire", "bladecontact", 3, 0.34f, 0.96f, true, true, true, 0.04f);
                case "vst": return Spell("vst", "castshock", "veilstep", "resonance", 2, 0.24f, 1.04f, true, false, true);
                case "ast": return Spell("ast", "casttempest", "tempest", "resonance", 3, 0.34f, 0.94f, true, true, true, 0.10f);

                // Warlock / Hex and Pact: whispers, soul strikes, gates, and transformations.
                case "rkw": return Spell("rkw", "casthex", "fieldsnare", "web", 1, 0.20f, 0.98f, false, false, false);
                case "rnh": return Spell("rnh", "casthex", "curse", "fieldcurse", 1, 0.22f, 0.96f, false, false, false);
                case "wbk": return Spell("wbk", "casthex", "fieldsnare", "web", 1, 0.20f, 1.02f, false, false, false);
                case "nvl": return Spell("nvl", "castveil", "status", "curse", 1, 0.18f, 1.04f, false, false, true);
                case "rms": return Spell("rms", "casthex", "sleep", "fieldcurse", 1, 0.24f, 0.94f, false, false, true);
                case "inh": return Spell("inh", "casthex", "death", "heal", 1, 0.26f, 0.96f, false, false, false);
                case "rmb": return Spell("rmb", "casthex", "curse", "death", 1, 0.24f, 0.92f, false, false, false);
                case "wbp": return Spell("wbp", "casthex", "fieldgas", "poison", 1, 0.24f, 0.98f, false, false, false);
                case "grh": return Spell("grh", "casthex", "death", "fieldsnare", 2, 0.28f, 0.94f, true, false, false);
                case "rpx": return Spell("rpx", "casthex", "poison", "fieldgas", 2, 0.30f, 0.98f, true, false, false);
                case "dmc": return Spell("dmc", "casthex", "fieldcurse", "resonance", 2, 0.28f, 0.90f, true, true, true);
                case "wtr": return Spell("wtr", "casthex", "death", "fieldcurse", 1, 0.26f, 0.92f, false, false, false);
                case "dsm": return Spell("dsm", "casthex", "sleep", "fieldcurse", 2, 0.32f, 0.94f, true, false, true);
                case "rlm": return Spell("rlm", "castdeathburst", "deathburst", "fieldcurse", 3, 0.18f, 0.90f, true, true, true, 0.08f);
                case "rbt": return Spell("rbt", "castpact", "death", "resonance", 1, 0.24f, 0.98f, false, false, false);
                case "ibd": return Spell("ibd", "castpact", "death", "encounter", 2, 0.28f, 0.94f, true, false, true);
                case "slv": return Spell("slv", "castpact", "ward", "resonance", 2, 0.28f, 1.02f, false, false, true);
                case "pbr": return Spell("pbr", "castpact", "fieldcurse", "resonance", 2, 0.30f, 0.92f, true, true, true);
                case "ibf": return Spell("ibf", "castpact", "death", "encounter", 2, 0.30f, 0.92f, true, true, true);
                case "vrs": return Spell("vrs", "castpact", "veilstep", "resonance", 2, 0.25f, 0.96f, true, false, true);
                case "ibg": return Spell("ibg", "castgreatersummon", "greatersummon", "encounter", 3, 0.34f, 0.88f, true, true, true, 0.08f);
                case "dfa": return Spell("dfa", "castascendance", "ascendance", "resonance", 3, 0.30f, 0.90f, true, true, true, 0.08f);

                // The cross-school capstone deliberately carries ember and hex layers.
                case "acr": return Spell("acr", "castember", "fieldcurse", "fieldfire", 3, 0.34f, 0.94f, true, true, true, 0.06f);
                default: return FallbackSpellProfile();
            }
        }

        public static CombatPowerSfxProfile ProfileForAbility(string abilityIdOrName)
        {
            switch (NormalizeAbilityKey(abilityIdOrName))
            {
                // Warrior.
                case "charge": return Skill("charge", "charge", "swingheavy", "chargeimpact", "crit", 2, 0.18f, 0.96f, true, true, false);
                case "rally": return Skill("rally", "rally", "guard", "ward", "resonance", 1, 0.10f, 1.02f, false, false, true);
                case "shieldbash": return Skill("shieldbash", "guard", "swingheavy", "counter", "impactshield", 1, 0.16f, 0.94f, true, false, false);
                case "execute": return Skill("execute", "execute", "swingheavy", "executeimpact", "blade", 2, 0.18f, 0.92f, true, true, false);
                case "cleave": return Skill("cleave", "blade", "swingheavy", "attack", "bladecontact", 1, 0.16f, 0.98f, true, false, false);
                case "whirlwind": return Skill("whirlwind", "whirlwind", "swing", "whirlwindimpact", "attack", 3, 0.10f, 0.94f, true, true, false, 0.04f);
                case "sunder": return Skill("sunder", "swingheavy", "swingheavy", "impactshield", "breakcover", 2, 0.18f, 0.90f, true, true, false);

                // Rogue.
                case "stealth": return Skill("stealth", "stealth", "", "status", "", 1, 0.08f, 1.04f, false, false, true);
                case "ambush": return Skill("ambush", "ambush", "thrust", "ambushimpact", "crit", 2, 0.17f, 1.04f, true, false, false);
                case "throwknife": return Skill("throwknife", "blade", "thrust", "bladecontact", "", 1, 0.20f, 1.06f, false, false, false);
                case "smokebomb": return Skill("smokebomb", "smoke", "", "smoke", "status", 2, 0.08f, 0.98f, false, false, true);
                case "hamstring": return Skill("hamstring", "eviscerate", "swing", "blade", "pinning", 1, 0.17f, 1.00f, true, false, false);
                case "eviscerate": return Skill("eviscerate", "eviscerate", "swingheavy", "eviscerateimpact", "death", 3, 0.18f, 0.94f, true, true, false, 0.04f);
                case "shadowstep": return Skill("shadowstep", "stealth", GenericSpellReleaseCue, "ambushimpact", "bladecontact", 2, 0.22f, 1.02f, true, false, true);

                // Ranger.
                case "aimedshot": return Skill("aimedshot", "aimedshot", "arrowrelease", "arrowcontact", "bow", 1, 0.20f, 1.02f, false, false, false);
                case "pinningshot": return Skill("pinningshot", "pinning", "arrowrelease", "arrowcontact", "fieldsnare", 1, 0.20f, 0.98f, false, false, false);
                case "scoutmark": return Skill("scoutmark", "scoutmark", "arrowrelease", "mark", "status", 1, 0.20f, 1.05f, false, false, true);
                case "volley": return Skill("volley", "volley", "arrowrelease", "arrowrain", "bow", 3, 0.34f, 0.98f, true, true, false, 0.04f);
                case "broadheadshot": return Skill("broadheadshot", "aimedshot", "arrowrelease", "arrowcontact", "blade", 2, 0.20f, 0.96f, true, false, false);
                case "disruptingshot": return Skill("disruptingshot", "aimedshot", "arrowrelease", "counter", "shock", 2, 0.20f, 1.04f, true, false, true);
                case "quickshot": return Skill("quickshot", "aimedshot", "arrowrelease", "arrowcontact", "bow", 2, 0.24f, 1.06f, true, false, false);

                // Warlock demon-form skills.
                case "riftpounce": return Skill("riftpounce", "riftpounce", GenericSpellReleaseCue, "riftpounceimpact", "resonance", 3, 0.22f, 0.92f, true, true, true, 0.06f);
                case "abyssalwhirl": return Skill("abyssalwhirl", "abyssalwhirl", "swingheavy", "abyssalwhirlimpact", "resonance", 3, 0.12f, 0.90f, true, true, false, 0.06f);
                case "soulrend": return Skill("soulrend", "soulrend", "swingheavy", "soulrendimpact", "resonance", 3, 0.18f, 0.90f, true, true, true, 0.06f);
                case "dreadroar": return Skill("dreadroar", "dreadroar", GenericSpellReleaseCue, "dreadroarimpact", "resonance", 3, 0.12f, 0.88f, true, true, true, 0.08f);
                default: return FallbackAbilityProfile();
            }
        }

        public static CombatPowerSfxPlan PlanForFormula(
            string formulaCodeOrName,
            int requestedIntensity = 0,
            bool reducedAudio = false,
            bool muted = false,
            int sfxVolumePercent = 100)
        {
            return BuildPlan(
                ProfileForFormula(formulaCodeOrName),
                requestedIntensity,
                reducedAudio,
                muted,
                sfxVolumePercent);
        }

        public static CombatPowerSfxPlan PlanForAbility(
            string abilityIdOrName,
            int requestedIntensity = 0,
            bool reducedAudio = false,
            bool muted = false,
            int sfxVolumePercent = 100)
        {
            return BuildPlan(
                ProfileForAbility(abilityIdOrName),
                requestedIntensity,
                reducedAudio,
                muted,
                sfxVolumePercent);
        }

        public static CombatPowerSfxPlan BuildPlan(
            CombatPowerSfxProfile profile,
            int requestedIntensity = 0,
            bool reducedAudio = false,
            bool muted = false,
            int sfxVolumePercent = 100)
        {
            int intensity = requestedIntensity <= 0
                ? profile.Intensity
                : Math.Max(profile.Intensity, ClampIntensity(requestedIntensity));
            float masterGain = muted ? 0f : Clamp(sfxVolumePercent, 0, 100) / 100f;
            float intensityGain = intensity == 3 ? 1.04f : intensity == 2 ? 0.97f : 0.90f;

            if (reducedAudio)
            {
                CombatPowerSfxCuePlan compact = profile.Impact.Enabled
                    ? profile.Impact
                    : profile.Release.Enabled ? profile.Release : profile.Cast;
                float compactGain = Math.Min(0.82f, compact.Gain * masterGain * intensityGain);
                CombatPowerSfxCuePlan impact = compact.Enabled && compactGain > 0f
                    ? new CombatPowerSfxCuePlan(CombatPowerSfxPhase.Impact, compact.Key, 0f, compactGain, compact.Pitch)
                    : CombatPowerSfxCuePlan.None(CombatPowerSfxPhase.Impact);
                return new CombatPowerSfxPlan(
                    profile.Key,
                    intensity,
                    true,
                    CombatPowerSfxCuePlan.None(CombatPowerSfxPhase.Cast),
                    CombatPowerSfxCuePlan.None(CombatPowerSfxPhase.Release),
                    impact,
                    CombatPowerSfxCuePlan.None(CombatPowerSfxPhase.Aftershock),
                    CombatPowerSfxCuePlan.None(CombatPowerSfxPhase.Accent),
                    CombatPowerSfxCuePlan.None(CombatPowerSfxPhase.Accent),
                    CombatPowerSfxCuePlan.None(CombatPowerSfxPhase.Accent));
            }

            CombatPowerSfxCuePlan cast = Mix(profile.Cast, masterGain, intensityGain);
            CombatPowerSfxCuePlan release = Mix(profile.Release, masterGain, intensityGain);
            CombatPowerSfxCuePlan impactCue = Mix(profile.Impact, masterGain, intensityGain);
            CombatPowerSfxCuePlan aftershock = Mix(profile.Aftershock, masterGain, intensityGain);
            CombatPowerSfxCuePlan lowHit = profile.LayerLowHit && intensity >= 2 && impactCue.Enabled
                ? Accent(LowHitCue, impactCue.Delay + 0.015f, (0.18f + intensity * 0.055f) * masterGain, impactCue.Pitch * 0.92f)
                : CombatPowerSfxCuePlan.None(CombatPowerSfxPhase.Accent);
            CombatPowerSfxCuePlan rumble = profile.LayerRumble && intensity >= 2 && impactCue.Enabled
                ? Accent(RumbleCue, impactCue.Delay + 0.080f, (0.14f + intensity * 0.050f) * masterGain, impactCue.Pitch * 0.90f)
                : CombatPowerSfxCuePlan.None(CombatPowerSfxPhase.Accent);
            CombatPowerSfxCuePlan shimmer = profile.LayerShimmer && intensity >= 1 && cast.Enabled
                ? Accent(ShimmerCue, Math.Max(0f, release.Delay - 0.030f), (0.15f + intensity * 0.035f) * masterGain, cast.Pitch * 1.04f)
                : CombatPowerSfxCuePlan.None(CombatPowerSfxPhase.Accent);

            return new CombatPowerSfxPlan(
                profile.Key,
                intensity,
                false,
                cast,
                release,
                impactCue,
                aftershock,
                lowHit,
                rumble,
                shimmer);
        }

        public static bool IsSupportedFormula(string formulaCodeOrName)
        {
            return IsCanonicalFormulaKey(NormalizeFormulaKey(formulaCodeOrName));
        }

        public static bool IsSupportedAbility(string abilityIdOrName)
        {
            return IsCanonicalAbilityKey(NormalizeAbilityKey(abilityIdOrName));
        }

        public static string NormalizeFormulaKey(string formulaCodeOrName)
        {
            string key = Compact(formulaCodeOrName);
            switch (key)
            {
                case "treecover":
                case "growtree": return "gbh";
                case "stoneblock":
                case "raisestone": return "gbx";
                case "hallowedcircle":
                case "sanctuarycircle": return "hlc";
                case "heal":
                case "mend": return "oic";
                case "cleanse":
                case "purify": return "nvc";
                case "riftseal":
                case "sealrift": return "srf";
                case "ward": return "tbq";
                case "sanctuaryward": return "sgw";
                case "regenerate":
                case "regeneration":
                case "regen": return "tnc";
                case "circleheal":
                case "healingcircle": return "lbc";
                case "circleward":
                case "wardingcircle": return "tbg";
                case "lightbolt":
                case "holybolt": return "obl";
                case "holdsign":
                case "signofholding": return "lnh";
                case "stillwater": return "swr";
                case "sunbrand": return "sbn";
                case "dawnpulse": return "dwp";
                case "firespark":
                case "flamespark": return "fif";
                case "arcspark": return "rig";
                case "iceslick": return "wbi";
                case "firefloor": return "wbf";
                case "coldlance":
                case "frostlance": return "rcl";
                case "burncover": return "btf";
                case "fireball": return "fbl";
                case "flamejet": return "rdf";
                case "thunderclap": return "rsg";
                case "frostbind": return "frb";
                case "iceburst": return "rbi";
                case "fireburst": return "rlf";
                case "chainlightning": return "clt";
                case "meteor":
                case "meteorshower": return "mtr";
                case "cinderstorm": return "cns";
                case "thunderstep": return "vst";
                case "tempest":
                case "arcanetempest": return "ast";
                case "bind": return "rkw";
                case "weaken": return "rnh";
                case "websnare": return "wbk";
                case "nightveil": return "nvl";
                case "sleep": return "rms";
                case "drainlife":
                case "lifedrain": return "inh";
                case "mindbreak": return "rmb";
                case "poisongas": return "wbp";
                case "gravehook": return "grh";
                case "poisonburst": return "rpx";
                case "doomcircle": return "dmc";
                case "wither": return "wtr";
                case "dreamsmoke": return "dsm";
                case "deathburst": return "rlm";
                case "riftbolt": return "rbt";
                case "summonimp":
                case "impsummon":
                case "boundimp": return "ibd";
                case "soulveil": return "slv";
                case "pactbrand": return "pbr";
                case "summonlesserdemon":
                case "lesserdemon": return "ibf";
                case "riftstep": return "vrs";
                case "summongreaterdemon":
                case "greaterdemon": return "ibg";
                case "abyssalascendance":
                case "ascendance": return "dfa";
                case "ashencurse": return "acr";
                default: return key;
            }
        }

        public static string NormalizeAbilityKey(string abilityIdOrName)
        {
            string key = Compact(abilityIdOrName);
            switch (key)
            {
                case "chg": return "charge";
                case "rly": return "rally";
                case "bsh": return "shieldbash";
                case "exe": return "execute";
                case "clv": return "cleave";
                case "ww": return "whirlwind";
                case "sun": return "sunder";
                case "stl": return "stealth";
                case "amb": return "ambush";
                case "thr": return "throwknife";
                case "smk": return "smokebomb";
                case "ham": return "hamstring";
                case "evs": return "eviscerate";
                case "shd": return "shadowstep";
                case "aim": return "aimedshot";
                case "pin": return "pinningshot";
                case "mrk": return "scoutmark";
                case "vol": return "volley";
                case "brd": return "broadheadshot";
                case "dis": return "disruptingshot";
                case "qsh": return "quickshot";
                case "rpt": return "riftpounce";
                case "awh": return "abyssalwhirl";
                case "srd": return "soulrend";
                case "drr": return "dreadroar";
                default: return key;
            }
        }

        public static int StableAudioHash(string powerKey, int sampleIndex, int channel = 0)
        {
            unchecked
            {
                uint hash = 2166136261u;
                string key = Compact(powerKey);
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

        public static float StableAudioSample(string powerKey, int sampleIndex, int channel = 0)
        {
            uint hash = unchecked((uint)StableAudioHash(powerKey, sampleIndex, channel));
            return (hash & 0x00ffffffu) / 16777216f;
        }

        public static float StableAudioSignedSample(string powerKey, int sampleIndex, int channel = 0)
        {
            return StableAudioSample(powerKey, sampleIndex, channel) * 2f - 1f;
        }

        public static int StableSfxHash(string powerKey, int sampleIndex, int channel = 0)
        {
            return StableAudioHash(powerKey, sampleIndex, channel);
        }

        public static float StableSfxSample(string powerKey, int sampleIndex, int channel = 0)
        {
            return StableAudioSample(powerKey, sampleIndex, channel);
        }

        public static float StablePitch(
            CombatPowerSfxCuePlan cue,
            string profileKey,
            int sampleIndex,
            int channel = 0,
            float maximumVariation = 0.025f)
        {
            float variation = Clamp(maximumVariation, 0f, 0.08f);
            return Clamp(
                cue.Pitch + StableAudioSignedSample(profileKey, sampleIndex, channel) * variation,
                0.90f,
                1.10f);
        }

        private static CombatPowerSfxProfile Spell(
            string key,
            string castCue,
            string impactCue,
            string aftershockCue,
            int intensity,
            float impactDelay,
            float pitch,
            bool lowHit,
            bool rumble,
            bool shimmer,
            float gainBoost = 0f)
        {
            int tier = ClampIntensity(intensity);
            float impact = Clamp(impactDelay, 0.08f, 0.52f);
            float release = Math.Max(0.035f, impact * (tier >= 3 ? 0.46f : 0.42f));
            float aftershock = string.IsNullOrEmpty(aftershockCue)
                ? 0f
                : Math.Min(0.60f, impact + 0.08f + tier * 0.025f);
            return new CombatPowerSfxProfile(
                key,
                tier,
                Cue(CombatPowerSfxPhase.Cast, castCue, 0f, 0.42f + tier * 0.065f + gainBoost * 0.30f, pitch + 0.015f),
                Cue(CombatPowerSfxPhase.Release, GenericSpellReleaseCue, release, 0.16f + tier * 0.045f, pitch + 0.035f),
                Cue(CombatPowerSfxPhase.Impact, impactCue, impact, 0.70f + tier * 0.12f + gainBoost, pitch),
                Cue(CombatPowerSfxPhase.Aftershock, aftershockCue, aftershock, 0.20f + tier * 0.075f + gainBoost * 0.25f, pitch - 0.035f),
                lowHit,
                rumble,
                shimmer);
        }

        private static CombatPowerSfxProfile Skill(
            string key,
            string castCue,
            string releaseCue,
            string impactCue,
            string aftershockCue,
            int intensity,
            float impactDelay,
            float pitch,
            bool lowHit,
            bool rumble,
            bool shimmer,
            float gainBoost = 0f)
        {
            int tier = ClampIntensity(intensity);
            float impact = Clamp(impactDelay, 0.06f, 0.42f);
            float release = string.IsNullOrEmpty(releaseCue) ? 0f : Math.Max(0.025f, impact * 0.42f);
            float aftershock = string.IsNullOrEmpty(aftershockCue)
                ? 0f
                : Math.Min(0.60f, impact + 0.07f + tier * 0.025f);
            return new CombatPowerSfxProfile(
                key,
                tier,
                Cue(CombatPowerSfxPhase.Cast, castCue, 0f, 0.40f + tier * 0.07f + gainBoost * 0.25f, pitch + 0.015f),
                Cue(CombatPowerSfxPhase.Release, releaseCue, release, 0.30f + tier * 0.055f, pitch + 0.025f),
                Cue(CombatPowerSfxPhase.Impact, impactCue, impact, 0.68f + tier * 0.11f + gainBoost, pitch),
                Cue(CombatPowerSfxPhase.Aftershock, aftershockCue, aftershock, 0.18f + tier * 0.065f + gainBoost * 0.25f, pitch - 0.035f),
                lowHit,
                rumble,
                shimmer);
        }

        private static CombatPowerSfxProfile FallbackSpellProfile()
        {
            return new CombatPowerSfxProfile(
                "spell",
                1,
                Cue(CombatPowerSfxPhase.Cast, "formula", 0f, 0.46f, 1f),
                Cue(CombatPowerSfxPhase.Release, GenericSpellReleaseCue, 0.05f, 0.18f, 1.02f),
                Cue(CombatPowerSfxPhase.Impact, "spell", 0.16f, 0.76f, 1f),
                CombatPowerSfxCuePlan.None(CombatPowerSfxPhase.Aftershock),
                false,
                false,
                false);
        }

        private static CombatPowerSfxProfile FallbackAbilityProfile()
        {
            return new CombatPowerSfxProfile(
                "skill",
                1,
                Cue(CombatPowerSfxPhase.Cast, "ui", 0f, 0.42f, 1f),
                Cue(CombatPowerSfxPhase.Release, "swing", 0.04f, 0.34f, 1f),
                Cue(CombatPowerSfxPhase.Impact, "attack", 0.12f, 0.74f, 1f),
                CombatPowerSfxCuePlan.None(CombatPowerSfxPhase.Aftershock),
                false,
                false,
                false);
        }

        private static CombatPowerSfxCuePlan Cue(
            CombatPowerSfxPhase phase,
            string key,
            float delay,
            float gain,
            float pitch)
        {
            return string.IsNullOrEmpty(key)
                ? CombatPowerSfxCuePlan.None(phase)
                : new CombatPowerSfxCuePlan(phase, key, delay, gain, pitch);
        }

        private static CombatPowerSfxCuePlan Accent(string key, float delay, float gain, float pitch)
        {
            return Cue(CombatPowerSfxPhase.Accent, key, delay, gain, pitch);
        }

        private static CombatPowerSfxCuePlan Mix(CombatPowerSfxCuePlan cue, float masterGain, float intensityGain)
        {
            if (!cue.Enabled || masterGain <= 0f) return CombatPowerSfxCuePlan.None(cue.Phase);
            return new CombatPowerSfxCuePlan(
                cue.Phase,
                cue.Key,
                cue.Delay,
                cue.Gain * masterGain * intensityGain,
                cue.Pitch);
        }

        private static bool IsCanonicalFormulaKey(string key)
        {
            switch (key)
            {
                case "gbh": case "gbx": case "hlc": case "oic": case "nvc": case "srf":
                case "tbq": case "sgw": case "tnc": case "lbc": case "tbg": case "obl":
                case "lnh": case "swr": case "sbn": case "dwp":
                case "fif": case "rig": case "wbi": case "wbf": case "rcl": case "btf":
                case "fbl": case "rdf": case "rsg": case "frb": case "rbi": case "rlf":
                case "clt": case "mtr": case "cns": case "vst": case "ast":
                case "rkw": case "rnh": case "wbk": case "nvl": case "rms": case "inh":
                case "rmb": case "wbp": case "grh": case "rpx": case "dmc": case "wtr":
                case "dsm": case "rlm": case "rbt": case "ibd": case "slv": case "pbr":
                case "ibf": case "vrs": case "ibg": case "dfa": case "acr":
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsCanonicalAbilityKey(string key)
        {
            switch (key)
            {
                case "charge": case "rally": case "shieldbash": case "execute": case "cleave":
                case "whirlwind": case "sunder": case "stealth": case "ambush": case "throwknife":
                case "smokebomb": case "hamstring": case "eviscerate": case "shadowstep":
                case "aimedshot": case "pinningshot": case "scoutmark": case "volley":
                case "broadheadshot": case "disruptingshot": case "quickshot": case "riftpounce":
                case "abyssalwhirl": case "soulrend": case "dreadroar":
                    return true;
                default:
                    return false;
            }
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

        private static int ClampIntensity(int intensity)
        {
            return Math.Max(1, Math.Min(3, intensity));
        }

        private static int Clamp(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        private static float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }
}
