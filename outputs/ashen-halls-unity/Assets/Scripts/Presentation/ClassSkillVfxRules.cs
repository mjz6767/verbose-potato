using System;

namespace AshenHalls
{
    public enum ClassSkillVfxPhase
    {
        Cast,
        Travel,
        Impact
    }

    public readonly struct ClassSkillVfxProfile
    {
        public readonly string Key;
        public readonly int CastCell;
        public readonly int TravelCell;
        public readonly int ImpactCell;
        public readonly float BaseScale;
        public readonly float BaseOpacity;
        public readonly float CastSeconds;
        public readonly float TravelSeconds;
        public readonly float ImpactSeconds;
        public readonly int BaseBurstCount;

        public bool Supported => CastCell >= 0 && ImpactCell >= 0;
        public bool HasTravel => TravelCell >= 0;

        public ClassSkillVfxProfile(
            string key,
            int castCell,
            int travelCell,
            int impactCell,
            float baseScale,
            float baseOpacity,
            float castSeconds,
            float travelSeconds,
            float impactSeconds,
            int baseBurstCount)
        {
            Key = string.IsNullOrEmpty(key) ? "skill" : key;
            CastCell = ValidCellOrNone(castCell);
            TravelCell = ValidCellOrNone(travelCell);
            ImpactCell = ValidCellOrNone(impactCell);
            BaseScale = Clamp(baseScale, 0.50f, 2.25f);
            BaseOpacity = Clamp(baseOpacity, 0.20f, 1f);
            CastSeconds = Math.Max(0f, castSeconds);
            TravelSeconds = TravelCell < 0 ? 0f : Math.Max(0f, travelSeconds);
            ImpactSeconds = Math.Max(0f, impactSeconds);
            BaseBurstCount = Math.Max(0, Math.Min(64, baseBurstCount));
        }

        private static int ValidCellOrNone(int cell)
        {
            return ClassSkillVfxRules.IsAtlasCell(cell) ? cell : -1;
        }

        private static float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }

    public readonly struct ClassSkillVfxArtPlan
    {
        public readonly ClassSkillVfxPhase Phase;
        public readonly int PrimaryCell;
        public readonly float PrimaryScale;
        public readonly float PrimaryOpacity;
        public readonly int SecondaryCell;
        public readonly float SecondaryScale;
        public readonly float SecondaryOpacity;
        public readonly float DurationSeconds;
        public readonly int BurstCount;

        public bool HasPrimary => PrimaryCell >= 0;
        public bool HasSecondary => SecondaryCell >= 0;

        public ClassSkillVfxArtPlan(
            ClassSkillVfxPhase phase,
            int primaryCell,
            float primaryScale,
            float primaryOpacity,
            int secondaryCell,
            float secondaryScale,
            float secondaryOpacity,
            float durationSeconds,
            int burstCount)
        {
            Phase = phase;
            PrimaryCell = ValidCellOrNone(primaryCell);
            PrimaryScale = PrimaryCell < 0 ? 0f : Math.Max(0f, primaryScale);
            PrimaryOpacity = PrimaryCell < 0 ? 0f : Clamp01(primaryOpacity);
            SecondaryCell = ValidCellOrNone(secondaryCell);
            SecondaryScale = SecondaryCell < 0 ? 0f : Math.Max(0f, secondaryScale);
            SecondaryOpacity = SecondaryCell < 0 ? 0f : Clamp01(secondaryOpacity);
            DurationSeconds = Math.Max(0f, durationSeconds);
            BurstCount = Math.Max(0, Math.Min(64, burstCount));
        }

        private static int ValidCellOrNone(int cell)
        {
            return ClassSkillVfxRules.IsAtlasCell(cell) ? cell : -1;
        }

        private static float Clamp01(float value)
        {
            return Math.Max(0f, Math.Min(1f, value));
        }
    }

    public static class ClassSkillVfxRules
    {
        public const int AtlasColumns = 4;
        public const int AtlasRows = 4;
        public const int AtlasCellCount = AtlasColumns * AtlasRows;

        public const int ChargeCell = 0;
        public const int ShieldBashCell = 1;
        public const int RallyCell = 2;
        public const int WhirlwindCell = 3;
        public const int CleaveCell = WhirlwindCell;
        public const int ExecuteCell = 4;
        public const int SunderCell = 5;
        public const int StealthCell = 6;
        public const int AmbushCell = 7;
        public const int SmokeBombCell = 8;
        public const int ThrowKnifeCell = 9;
        public const int EviscerateHamstringCell = 10;
        public const int EviscerateCell = EviscerateHamstringCell;
        public const int HamstringCell = EviscerateHamstringCell;
        public const int ShadowstepCell = 11;
        public const int RiftPounceCell = 12;
        public const int AbyssalWhirlCell = 13;
        public const int AbyssalWhirlwindCell = AbyssalWhirlCell;
        public const int SoulRendCell = 14;
        public const int DreadRoarCell = 15;

        public static bool IsAtlasCell(int cell)
        {
            return cell >= 0 && cell < AtlasCellCount;
        }

        public static bool SupportsClass(string classKey)
        {
            string key = Compact(classKey);
            return key == "warrior" || key == "rogue" || key == "demon";
        }

        public static bool IsSupported(string abilityOrVisualKind)
        {
            switch (NormalizeKey(abilityOrVisualKind))
            {
                case "charge":
                case "shieldbash":
                case "rally":
                case "whirlwind":
                case "execute":
                case "sunder":
                case "stealth":
                case "ambush":
                case "smokebomb":
                case "throwknife":
                case "eviscerate":
                case "shadowstep":
                case "riftpounce":
                case "abyssalwhirl":
                case "soulrend":
                case "dreadroar":
                    return true;
                default:
                    return false;
            }
        }

        public static string NormalizeKey(string abilityOrVisualKind)
        {
            string key = Compact(abilityOrVisualKind);
            switch (key)
            {
                case "charge":
                case "chargeimpact":
                case "rush":
                case "warriorcharge":
                    return "charge";
                case "shieldbash":
                case "shieldbashimpact":
                case "shieldslam":
                case "bash":
                    return "shieldbash";
                case "rally":
                case "warriorrally":
                case "battlecry":
                    return "rally";
                case "whirlwind":
                case "whirlwindimpact":
                case "cleave":
                case "cleaveimpact":
                case "spinningcleave":
                case "bladecyclone":
                    return "whirlwind";
                case "execute":
                case "executeimpact":
                case "execution":
                    return "execute";
                case "sunder":
                case "sunderimpact":
                case "armorsunder":
                case "guardbreak":
                    return "sunder";
                case "stealth":
                case "vanish":
                    return "stealth";
                case "ambush":
                case "ambushimpact":
                case "backstab":
                    return "ambush";
                case "smokebomb":
                case "smokecloud":
                    return "smokebomb";
                case "throwknife":
                case "thrownknife":
                case "knifethrow":
                    return "throwknife";
                case "eviscerate":
                case "eviscerateimpact":
                case "hamstring":
                case "hamstringimpact":
                case "bleedingstrike":
                case "deepcut":
                    return "eviscerate";
                case "shadowstep":
                case "shadowstepimpact":
                case "shadowstrike":
                    return "shadowstep";
                case "riftpounce":
                case "riftpounceimpact":
                case "demonpounce":
                    return "riftpounce";
                case "abyssalwhirl":
                case "abyssalwhirlimpact":
                case "abyssalwhirlwind":
                case "demonwhirl":
                    return "abyssalwhirl";
                case "soulrend":
                case "soulrendimpact":
                case "liferip":
                    return "soulrend";
                case "dreadroar":
                case "dreadroarimpact":
                case "demonroar":
                    return "dreadroar";
                case "aimedshot":
                case "pinningshot":
                case "scoutmark":
                case "volley":
                case "broadheadshot":
                case "disruptingshot":
                case "quickshot":
                    // Ranger abilities intentionally retain their dedicated atlas.
                    return key;
                default:
                    return key;
            }
        }

        public static ClassSkillVfxProfile ProfileFor(string abilityOrVisualKind)
        {
            switch (NormalizeKey(abilityOrVisualKind))
            {
                case "charge":
                    return Profile("charge", ChargeCell, ChargeCell, ChargeCell, 1.14f, 0.98f, 0.22f, 0.28f, 0.52f, 24);
                case "shieldbash":
                    return Profile("shieldbash", ShieldBashCell, -1, ShieldBashCell, 1.08f, 0.98f, 0.18f, 0f, 0.44f, 18);
                case "rally":
                    return Profile("rally", RallyCell, -1, RallyCell, 1.18f, 0.94f, 0.32f, 0f, 0.48f, 20);
                case "whirlwind":
                    return Profile("whirlwind", WhirlwindCell, -1, WhirlwindCell, 1.25f, 1f, 0.24f, 0f, 0.62f, 32);
                case "execute":
                    return Profile("execute", ExecuteCell, -1, ExecuteCell, 1.20f, 1f, 0.30f, 0f, 0.58f, 28);
                case "sunder":
                    return Profile("sunder", SunderCell, -1, SunderCell, 1.16f, 0.98f, 0.24f, 0f, 0.54f, 24);
                case "stealth":
                    return Profile("stealth", StealthCell, -1, StealthCell, 0.98f, 0.92f, 0.28f, 0f, 0.46f, 14);
                case "ambush":
                    return Profile("ambush", AmbushCell, -1, AmbushCell, 1.10f, 0.98f, 0.20f, 0f, 0.50f, 22);
                case "smokebomb":
                    return Profile("smokebomb", SmokeBombCell, -1, SmokeBombCell, 1.25f, 0.92f, 0.28f, 0f, 0.58f, 28);
                case "throwknife":
                    return Profile("throwknife", ThrowKnifeCell, ThrowKnifeCell, ThrowKnifeCell, 0.90f, 0.96f, 0.16f, 0.26f, 0.42f, 16);
                case "eviscerate":
                    return Profile("eviscerate", EviscerateHamstringCell, -1, EviscerateHamstringCell, 1.12f, 1f, 0.22f, 0f, 0.52f, 24);
                case "shadowstep":
                    return Profile("shadowstep", ShadowstepCell, ShadowstepCell, ShadowstepCell, 1.15f, 0.98f, 0.22f, 0.26f, 0.52f, 24);
                case "riftpounce":
                    return Profile("riftpounce", RiftPounceCell, RiftPounceCell, RiftPounceCell, 1.30f, 1f, 0.32f, 0.32f, 0.68f, 38);
                case "abyssalwhirl":
                    return Profile("abyssalwhirl", AbyssalWhirlCell, -1, AbyssalWhirlCell, 1.38f, 1f, 0.30f, 0f, 0.72f, 44);
                case "soulrend":
                    return Profile("soulrend", SoulRendCell, SoulRendCell, SoulRendCell, 1.30f, 1f, 0.28f, 0.26f, 0.68f, 38);
                case "dreadroar":
                    return Profile("dreadroar", DreadRoarCell, -1, DreadRoarCell, 1.42f, 1f, 0.38f, 0f, 0.76f, 46);
                default:
                    return Profile("skill", -1, -1, -1, 1f, 0.88f, 0f, 0f, 0f, 0);
            }
        }

        public static ClassSkillVfxArtPlan CastPlan(
            string abilityOrVisualKind,
            int intensity = 1,
            float progress = 0.35f,
            bool reducedMotion = false)
        {
            return BuildPlan(ProfileFor(abilityOrVisualKind), ClassSkillVfxPhase.Cast, intensity, progress, reducedMotion);
        }

        public static ClassSkillVfxArtPlan TravelPlan(
            string abilityOrVisualKind,
            int intensity = 1,
            float progress = 0.35f,
            bool reducedMotion = false)
        {
            return BuildPlan(ProfileFor(abilityOrVisualKind), ClassSkillVfxPhase.Travel, intensity, progress, reducedMotion);
        }

        public static ClassSkillVfxArtPlan ImpactPlan(
            string abilityOrVisualKind,
            int intensity = 1,
            float progress = 0.35f,
            bool reducedMotion = false)
        {
            return BuildPlan(ProfileFor(abilityOrVisualKind), ClassSkillVfxPhase.Impact, intensity, progress, reducedMotion);
        }

        public static float CastDuration(string abilityOrVisualKind)
        {
            return ProfileFor(abilityOrVisualKind).CastSeconds;
        }

        public static float TravelDuration(string abilityOrVisualKind)
        {
            return ProfileFor(abilityOrVisualKind).TravelSeconds;
        }

        public static float ImpactDuration(string abilityOrVisualKind)
        {
            return ProfileFor(abilityOrVisualKind).ImpactSeconds;
        }

        public static float PhaseDuration(ClassSkillVfxProfile profile, ClassSkillVfxPhase phase)
        {
            switch (phase)
            {
                case ClassSkillVfxPhase.Cast: return profile.CastSeconds;
                case ClassSkillVfxPhase.Travel: return profile.TravelSeconds;
                case ClassSkillVfxPhase.Impact: return profile.ImpactSeconds;
                default: return 0f;
            }
        }

        public static float PhaseProgress(float elapsedSeconds, float durationSeconds, bool reducedMotion = false)
        {
            if (reducedMotion || durationSeconds <= 0f) return 1f;
            return Clamp01(Math.Max(0f, elapsedSeconds) / durationSeconds);
        }

        public static float ArtScale(
            ClassSkillVfxProfile profile,
            ClassSkillVfxPhase phase,
            int intensity,
            float progress)
        {
            int tier = ClampIntensity(intensity);
            float t = Clamp01(progress);
            float tierScale = 1f + (tier - 1) * 0.10f;
            float phaseScale;
            switch (phase)
            {
                case ClassSkillVfxPhase.Cast:
                    phaseScale = 0.78f + Smooth01(t) * 0.22f;
                    break;
                case ClassSkillVfxPhase.Travel:
                    phaseScale = 0.72f + (float)Math.Sin(t * Math.PI) * 0.18f;
                    break;
                case ClassSkillVfxPhase.Impact:
                    phaseScale = 0.84f + (float)Math.Sin(t * Math.PI) * 0.36f;
                    break;
                default:
                    phaseScale = 1f;
                    break;
            }
            return Clamp(profile.BaseScale * tierScale * phaseScale, 0.42f, 2.40f);
        }

        public static float ArtOpacity(
            ClassSkillVfxProfile profile,
            ClassSkillVfxPhase phase,
            int intensity,
            float progress)
        {
            int tier = ClampIntensity(intensity);
            float t = Clamp01(progress);
            float pulse = 0.82f + (float)Math.Sin(t * Math.PI) * 0.18f;
            float phaseOpacity = phase == ClassSkillVfxPhase.Travel ? 0.94f : pulse;
            return Clamp01(profile.BaseOpacity * phaseOpacity + (tier - 1) * 0.025f);
        }

        public static int StableVisualHash(string abilityOrVisualKind, int sampleIndex, int channel = 0)
        {
            unchecked
            {
                uint hash = 2166136261u;
                string key = NormalizeKey(abilityOrVisualKind);
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

        public static float StableVisualSample(string abilityOrVisualKind, int sampleIndex, int channel = 0)
        {
            uint hash = unchecked((uint)StableVisualHash(abilityOrVisualKind, sampleIndex, channel));
            return (hash & 0x00ffffffu) / 16777216f;
        }

        public static float StableVisualSignedSample(string abilityOrVisualKind, int sampleIndex, int channel = 0)
        {
            return StableVisualSample(abilityOrVisualKind, sampleIndex, channel) * 2f - 1f;
        }

        private static ClassSkillVfxArtPlan BuildPlan(
            ClassSkillVfxProfile profile,
            ClassSkillVfxPhase phase,
            int intensity,
            float progress,
            bool reducedMotion)
        {
            int tier = ClampIntensity(intensity);
            int primaryCell = PrimaryCell(profile, phase);
            if (!profile.Supported || primaryCell < 0)
            {
                return new ClassSkillVfxArtPlan(phase, -1, 0f, 0f, -1, 0f, 0f, 0f, 0);
            }

            float sampleProgress = reducedMotion ? 0.50f : Clamp01(progress);
            float scale = ArtScale(profile, phase, tier, sampleProgress);
            float opacity = ArtOpacity(profile, phase, tier, sampleProgress);
            int secondaryCell = SecondaryCell(phase, tier, primaryCell, reducedMotion);
            float duration = PhaseDuration(profile, phase);
            if (reducedMotion) duration = Math.Min(0.10f, duration);
            return new ClassSkillVfxArtPlan(
                phase,
                primaryCell,
                scale,
                opacity,
                secondaryCell,
                secondaryCell < 0 ? 0f : Clamp(scale * (phase == ClassSkillVfxPhase.Impact ? 1.28f : 1.14f), 0.50f, 2.55f),
                secondaryCell < 0 ? 0f : Clamp01(opacity * 0.42f),
                duration,
                BurstCount(profile, phase, tier, reducedMotion));
        }

        private static int PrimaryCell(ClassSkillVfxProfile profile, ClassSkillVfxPhase phase)
        {
            switch (phase)
            {
                case ClassSkillVfxPhase.Cast: return profile.CastCell;
                case ClassSkillVfxPhase.Travel: return profile.TravelCell;
                case ClassSkillVfxPhase.Impact: return profile.ImpactCell;
                default: return -1;
            }
        }

        private static int SecondaryCell(
            ClassSkillVfxPhase phase,
            int intensity,
            int primaryCell,
            bool reducedMotion)
        {
            if (reducedMotion) return -1;
            if (phase == ClassSkillVfxPhase.Impact && intensity >= 2) return primaryCell;
            if (phase == ClassSkillVfxPhase.Travel && intensity >= 3) return primaryCell;
            return -1;
        }

        private static int BurstCount(
            ClassSkillVfxProfile profile,
            ClassSkillVfxPhase phase,
            int intensity,
            bool reducedMotion)
        {
            int count = profile.BaseBurstCount + (intensity - 1) * 5;
            if (phase == ClassSkillVfxPhase.Cast) count = (count + 1) / 2;
            if (phase == ClassSkillVfxPhase.Travel) count = (count + 2) / 3;
            if (reducedMotion) count = Math.Max(1, count / 4);
            return Math.Max(0, Math.Min(64, count));
        }

        private static ClassSkillVfxProfile Profile(
            string key,
            int castCell,
            int travelCell,
            int impactCell,
            float baseScale,
            float baseOpacity,
            float castSeconds,
            float travelSeconds,
            float impactSeconds,
            int baseBurstCount)
        {
            return new ClassSkillVfxProfile(
                key,
                castCell,
                travelCell,
                impactCell,
                baseScale,
                baseOpacity,
                castSeconds,
                travelSeconds,
                impactSeconds,
                baseBurstCount);
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

        private static float Smooth01(float value)
        {
            float t = Clamp01(value);
            return t * t * (3f - 2f * t);
        }

        private static float Clamp01(float value)
        {
            return Math.Max(0f, Math.Min(1f, value));
        }

        private static float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }
}
