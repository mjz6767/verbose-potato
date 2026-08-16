using System;

namespace AshenHalls
{
    public enum CombatPowerAftermathKind
    {
        None = -1,
        Fireball = 0,
        Meteor = 1,
        Frost = 2,
        Lightning = 3,
        Mend = 4,
        Ward = 5,
        Nature = 6,
        Sun = 7,
        Rift = 8,
        Soul = 9,
        Hex = 10,
        PoisonDream = 11,
        Web = 12,
        Summon = 13,
        Ascendance = 14,
        Martial = 15
    }

    public readonly struct CombatPowerAftermathVfxProfile
    {
        public readonly string Key;
        public readonly int AtlasCell;
        public readonly CombatPowerAftermathKind Kind;
        public readonly float BaseScale;
        public readonly float BaseOpacity;
        public readonly float DurationSeconds;
        public readonly int LayerCount;
        public readonly int ParticleCount;

        public CombatPowerAftermathVfxProfile(
            string key,
            CombatPowerAftermathKind kind,
            float baseScale,
            float baseOpacity,
            float durationSeconds,
            int layerCount,
            int particleCount)
        {
            Key = (key ?? "").Trim().ToLowerInvariant();
            Kind = kind;
            AtlasCell = CombatPowerAftermathVfxRules.IsAtlasCell((int)kind) ? (int)kind : -1;
            BaseScale = AtlasCell < 0 ? 0f : Clamp(baseScale, CombatPowerAftermathVfxRules.MinimumScale, CombatPowerAftermathVfxRules.MaximumScale);
            BaseOpacity = AtlasCell < 0 ? 0f : Clamp(baseOpacity, CombatPowerAftermathVfxRules.MinimumOpacity, CombatPowerAftermathVfxRules.MaximumOpacity);
            DurationSeconds = AtlasCell < 0 ? 0f : Clamp(durationSeconds, CombatPowerAftermathVfxRules.MinimumDurationSeconds, CombatPowerAftermathVfxRules.MaximumDurationSeconds);
            LayerCount = AtlasCell < 0 ? 0 : Math.Max(1, Math.Min(CombatPowerAftermathVfxRules.MaximumLayerCount, layerCount));
            ParticleCount = AtlasCell < 0 ? 0 : Math.Max(0, Math.Min(CombatPowerAftermathVfxRules.MaximumParticleCount, particleCount));
        }

        public bool Supported => AtlasCell >= 0 && Key.Length > 0;
        public bool HasAftermath => Supported && DurationSeconds > 0f;

        private static float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }

    public readonly struct CombatPowerAftermathVfxPlan
    {
        public readonly string Key;
        public readonly int AtlasCell;
        public readonly CombatPowerAftermathKind Kind;
        public readonly float Scale;
        public readonly float Opacity;
        public readonly float DurationSeconds;
        public readonly int LayerCount;
        public readonly int ParticleCount;
        public readonly float Progress;
        public readonly int StableSeed;
        public readonly float Drift;
        public readonly float Pulse;

        public CombatPowerAftermathVfxPlan(
            string key,
            CombatPowerAftermathKind kind,
            float scale,
            float opacity,
            float durationSeconds,
            int layerCount,
            int particleCount,
            float progress,
            int stableSeed,
            float drift,
            float pulse)
        {
            Key = (key ?? "").Trim().ToLowerInvariant();
            Kind = kind;
            AtlasCell = CombatPowerAftermathVfxRules.IsAtlasCell((int)kind) ? (int)kind : -1;
            Scale = AtlasCell < 0 ? 0f : Clamp(scale, CombatPowerAftermathVfxRules.MinimumScale, CombatPowerAftermathVfxRules.MaximumPlanScale);
            Opacity = AtlasCell < 0 ? 0f : Clamp(opacity, 0f, CombatPowerAftermathVfxRules.MaximumOpacity);
            DurationSeconds = AtlasCell < 0 ? 0f : Clamp(durationSeconds, CombatPowerAftermathVfxRules.MinimumDurationSeconds, CombatPowerAftermathVfxRules.MaximumDurationSeconds);
            LayerCount = AtlasCell < 0 ? 0 : Math.Max(1, Math.Min(CombatPowerAftermathVfxRules.MaximumLayerCount, layerCount));
            ParticleCount = AtlasCell < 0 ? 0 : Math.Max(0, Math.Min(CombatPowerAftermathVfxRules.MaximumParticleCount, particleCount));
            Progress = Clamp(progress, 0f, 1f);
            StableSeed = Math.Max(0, stableSeed);
            Drift = Clamp(drift, -1f, 1f);
            Pulse = Clamp(pulse, 0.72f, 1.24f);
        }

        public bool HasAftermath => AtlasCell >= 0 && DurationSeconds > 0f && Opacity > 0f;

        private static float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }

    public static class CombatPowerAftermathVfxRules
    {
        public const int AtlasColumns = 4;
        public const int AtlasRows = 4;
        public const int AtlasCellCount = AtlasColumns * AtlasRows;
        public const float MinimumScale = 0.55f;
        public const float MaximumScale = 2.20f;
        public const float MaximumPlanScale = 2.45f;
        public const float MinimumOpacity = 0.18f;
        public const float MaximumOpacity = 1f;
        public const float MinimumDurationSeconds = 0.34f;
        public const float MaximumDurationSeconds = 1.24f;
        public const int MaximumLayerCount = 3;
        public const int MaximumParticleCount = 48;

        public static bool IsAtlasCell(int cell)
        {
            return cell >= 0 && cell < AtlasCellCount;
        }

        public static bool IsKnownFormula(string formulaCodeOrName)
        {
            return CombatPowerSfxRules.IsSupportedFormula(formulaCodeOrName);
        }

        public static bool IsKnownAbility(string abilityIdOrName)
        {
            return CombatPowerSfxRules.IsSupportedAbility(abilityIdOrName);
        }

        public static bool IsSupported(string powerKeyOrName)
        {
            return ProfileFor(powerKeyOrName).HasAftermath;
        }

        public static bool IsSupportedFormula(string formulaCodeOrName)
        {
            return ProfileForFormula(formulaCodeOrName).HasAftermath;
        }

        public static bool IsSupportedAbility(string abilityIdOrName)
        {
            return ProfileForAbility(abilityIdOrName).HasAftermath;
        }

        public static string NormalizeFormulaKey(string formulaCodeOrName)
        {
            return CombatPowerSfxRules.NormalizeFormulaKey(formulaCodeOrName);
        }

        public static string NormalizeAbilityKey(string abilityIdOrName)
        {
            return CombatPowerSfxRules.NormalizeAbilityKey(abilityIdOrName);
        }

        public static CombatPowerAftermathVfxProfile ProfileFor(string powerKeyOrName)
        {
            if (IsKnownFormula(powerKeyOrName)) return ProfileForFormula(powerKeyOrName);
            if (IsKnownAbility(powerKeyOrName)) return ProfileForAbility(powerKeyOrName);
            return EmptyProfile(powerKeyOrName);
        }

        public static CombatPowerAftermathVfxProfile ProfileForFormula(string formulaCodeOrName)
        {
            string key = NormalizeFormulaKey(formulaCodeOrName);
            switch (key)
            {
                case "fbl": return Profile(key, CombatPowerAftermathKind.Fireball, 1.78f, 0.96f, 1.10f, 3, 36);
                case "mtr": case "cns": return Profile(key, CombatPowerAftermathKind.Meteor, 1.92f, 0.98f, 1.16f, 3, 42);
                case "wbi": case "rcl": case "rbi": case "frb": return Profile(key, CombatPowerAftermathKind.Frost, 1.55f, 0.92f, 0.88f, 2, 24);
                case "rig": case "rsg": case "clt": case "vst": case "ast": return Profile(key, CombatPowerAftermathKind.Lightning, 1.58f, 0.94f, 0.76f, 3, 30);
                case "oic": case "tnc": case "lbc": case "swr": case "dwp": return Profile(key, CombatPowerAftermathKind.Mend, 1.42f, 0.90f, 0.88f, 2, 20);
                case "hlc": case "tbq": case "sgw": case "tbg": case "slv": return Profile(key, CombatPowerAftermathKind.Ward, 1.58f, 0.90f, 1.04f, 2, 18);
                case "gbh": case "gbx": return Profile(key, CombatPowerAftermathKind.Nature, 1.60f, 0.92f, 1.08f, 2, 24);
                case "nvc": case "srf": case "obl": case "lnh": case "sbn": return Profile(key, CombatPowerAftermathKind.Sun, 1.46f, 0.94f, 0.86f, 3, 22);
                case "rbt": case "vrs": return Profile(key, CombatPowerAftermathKind.Rift, 1.48f, 0.94f, 0.92f, 3, 28);
                case "inh": case "rlm": return Profile(key, CombatPowerAftermathKind.Soul, 1.52f, 0.94f, 0.96f, 3, 28);
                case "dmc": case "rnh": case "rmb": case "wtr": case "pbr": case "acr": return Profile(key, CombatPowerAftermathKind.Hex, 1.50f, 0.93f, 0.98f, 3, 26);
                case "wbp": case "rms": case "rpx": case "dsm": case "nvl": return Profile(key, CombatPowerAftermathKind.PoisonDream, 1.54f, 0.90f, 1.02f, 2, 26);
                case "wbk": case "rkw": case "grh": return Profile(key, CombatPowerAftermathKind.Web, 1.46f, 0.94f, 0.94f, 2, 18);
                case "ibd": case "ibf": case "ibg": return Profile(key, CombatPowerAftermathKind.Summon, 1.72f, 0.96f, 1.12f, 3, 34);
                case "dfa": return Profile(key, CombatPowerAftermathKind.Ascendance, 1.98f, 0.98f, 1.20f, 3, 44);
                case "fif": case "wbf": case "btf": case "rdf": case "rlf": return Profile(key, CombatPowerAftermathKind.Fireball, 1.48f, 0.91f, 0.90f, 2, 24);
                default: return EmptyProfile(key);
            }
        }

        public static CombatPowerAftermathVfxProfile ProfileForAbility(string abilityIdOrName)
        {
            string key = NormalizeAbilityKey(abilityIdOrName);
            switch (key)
            {
                case "smokebomb": return Profile(key, CombatPowerAftermathKind.PoisonDream, 1.46f, 0.90f, 0.94f, 2, 24);
                case "scoutmark": return Profile(key, CombatPowerAftermathKind.Hex, 1.32f, 0.90f, 0.82f, 2, 14);
                case "riftpounce": return Profile(key, CombatPowerAftermathKind.Rift, 1.64f, 0.96f, 0.96f, 3, 30);
                case "soulrend": return Profile(key, CombatPowerAftermathKind.Soul, 1.60f, 0.96f, 0.98f, 3, 30);
                case "abyssalwhirl": case "dreadroar": return Profile(key, CombatPowerAftermathKind.Ascendance, 1.82f, 0.97f, 1.08f, 3, 38);
                case "charge": case "rally": case "shieldbash": case "execute": case "cleave": case "whirlwind": case "sunder":
                case "stealth": case "ambush": case "throwknife": case "hamstring": case "eviscerate": case "shadowstep":
                case "aimedshot": case "pinningshot": case "volley": case "broadheadshot": case "disruptingshot": case "quickshot":
                    return Profile(key, CombatPowerAftermathKind.Martial, 1.34f, 0.90f, 0.76f, 2, 20);
                default: return EmptyProfile(key);
            }
        }

        public static CombatPowerAftermathVfxPlan PlanFor(
            string powerKeyOrName,
            int intensity = 1,
            float progress = 0f,
            bool reducedMotion = false,
            int sampleIndex = 0)
        {
            return BuildPlan(ProfileFor(powerKeyOrName), intensity, progress, reducedMotion, sampleIndex);
        }

        public static CombatPowerAftermathVfxPlan PlanForFormula(
            string formulaCodeOrName,
            int intensity = 1,
            float progress = 0f,
            bool reducedMotion = false,
            int sampleIndex = 0)
        {
            return BuildPlan(ProfileForFormula(formulaCodeOrName), intensity, progress, reducedMotion, sampleIndex);
        }

        public static CombatPowerAftermathVfxPlan PlanForAbility(
            string abilityIdOrName,
            int intensity = 1,
            float progress = 0f,
            bool reducedMotion = false,
            int sampleIndex = 0)
        {
            return BuildPlan(ProfileForAbility(abilityIdOrName), intensity, progress, reducedMotion, sampleIndex);
        }

        public static float AftermathProgress(float elapsedSeconds, float durationSeconds)
        {
            if (durationSeconds <= 0f) return 1f;
            return Clamp01(Math.Max(0f, elapsedSeconds) / durationSeconds);
        }

        public static int StableAftermathHash(string powerKeyOrName, int sampleIndex, int channel = 0)
        {
            unchecked
            {
                uint hash = 2166136261u;
                string key = ProfileFor(powerKeyOrName).Key;
                if (key.Length == 0) key = Compact(powerKeyOrName);
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

        public static float StableAftermathSample(string powerKeyOrName, int sampleIndex, int channel = 0)
        {
            uint hash = unchecked((uint)StableAftermathHash(powerKeyOrName, sampleIndex, channel));
            return (hash & 0x00ffffffu) / 16777216f;
        }

        public static float StableAftermathSignedSample(string powerKeyOrName, int sampleIndex, int channel = 0)
        {
            return StableAftermathSample(powerKeyOrName, sampleIndex, channel) * 2f - 1f;
        }

        private static CombatPowerAftermathVfxPlan BuildPlan(
            CombatPowerAftermathVfxProfile profile,
            int intensity,
            float progress,
            bool reducedMotion,
            int sampleIndex)
        {
            if (reducedMotion || !profile.HasAftermath) return EmptyPlan(profile.Key);
            int tier = Math.Max(1, Math.Min(3, intensity));
            float t = Clamp01(progress);
            float fade = 1f - SmoothStep(0.58f, 1f, t) * 0.78f;
            float scaleNoise = 0.96f + StableAftermathSample(profile.Key, sampleIndex, 0) * 0.08f;
            float opacityNoise = 0.96f + StableAftermathSample(profile.Key, sampleIndex, 1) * 0.04f;
            float pulse = 0.94f + (float)Math.Sin((t * 2f + StableAftermathSample(profile.Key, sampleIndex, 2)) * Math.PI) * 0.06f;
            float scale = Clamp(profile.BaseScale * (1f + (tier - 1) * 0.08f) * scaleNoise, MinimumScale, MaximumPlanScale);
            float opacity = Clamp(profile.BaseOpacity * opacityNoise * fade, 0f, MaximumOpacity);
            int layers = Math.Max(1, Math.Min(MaximumLayerCount, profile.LayerCount + (tier >= 3 ? 1 : 0)));
            int particles = Math.Max(0, Math.Min(MaximumParticleCount, profile.ParticleCount + (tier - 1) * 6));
            return new CombatPowerAftermathVfxPlan(
                profile.Key,
                profile.Kind,
                scale,
                opacity,
                profile.DurationSeconds,
                layers,
                particles,
                t,
                StableAftermathHash(profile.Key, sampleIndex, 3),
                StableAftermathSignedSample(profile.Key, sampleIndex, 4),
                pulse);
        }

        private static CombatPowerAftermathVfxProfile Profile(
            string key,
            CombatPowerAftermathKind kind,
            float scale,
            float opacity,
            float duration,
            int layers,
            int particles)
        {
            return new CombatPowerAftermathVfxProfile(key, kind, scale, opacity, duration, layers, particles);
        }

        private static CombatPowerAftermathVfxProfile EmptyProfile(string key)
        {
            return new CombatPowerAftermathVfxProfile(key, CombatPowerAftermathKind.None, 0f, 0f, 0f, 0, 0);
        }

        private static CombatPowerAftermathVfxPlan EmptyPlan(string key)
        {
            return new CombatPowerAftermathVfxPlan(key, CombatPowerAftermathKind.None, 0f, 0f, 0f, 0, 0, 1f, 0, 0f, 1f);
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

        private static float SmoothStep(float edge0, float edge1, float value)
        {
            if (edge1 <= edge0) return value >= edge1 ? 1f : 0f;
            float t = Clamp01((value - edge0) / (edge1 - edge0));
            return t * t * (3f - 2f * t);
        }

        private static float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        private static float Clamp01(float value)
        {
            return Clamp(value, 0f, 1f);
        }
    }
}
