using System;

namespace AshenHalls
{
    public enum SupportHexSpellVfxPhase
    {
        Cast,
        Projectile,
        Impact
    }

    public readonly struct SupportHexSpellVfxProfile
    {
        public readonly string Key;
        public readonly int CastCell;
        public readonly int ProjectileCell;
        public readonly int ImpactCell;
        public readonly int ImpactAccentCell;
        public readonly float BaseScale;
        public readonly float BaseOpacity;
        public readonly float CastSeconds;
        public readonly float ProjectileSeconds;
        public readonly float ImpactSeconds;
        public readonly int BaseBurstCount;

        public bool HasProjectile => ProjectileCell >= 0;

        public SupportHexSpellVfxProfile(
            string key,
            int castCell,
            int projectileCell,
            int impactCell,
            int impactAccentCell,
            float baseScale,
            float baseOpacity,
            float castSeconds,
            float projectileSeconds,
            float impactSeconds,
            int baseBurstCount)
        {
            Key = string.IsNullOrEmpty(key) ? "spell" : key;
            CastCell = ValidCellOrNone(castCell);
            ProjectileCell = ValidCellOrNone(projectileCell);
            ImpactCell = ValidCellOrNone(impactCell);
            ImpactAccentCell = ValidCellOrNone(impactAccentCell);
            BaseScale = Clamp(baseScale, 0.50f, 2.25f);
            BaseOpacity = Clamp(baseOpacity, 0.20f, 1f);
            CastSeconds = Math.Max(0f, castSeconds);
            ProjectileSeconds = ProjectileCell < 0 ? 0f : Math.Max(0f, projectileSeconds);
            ImpactSeconds = Math.Max(0f, impactSeconds);
            BaseBurstCount = Math.Max(0, Math.Min(64, baseBurstCount));
        }

        private static int ValidCellOrNone(int cell)
        {
            return SupportHexSpellVfxRules.IsAtlasCell(cell) ? cell : -1;
        }

        private static float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }

    public readonly struct SupportHexSpellVfxArtPlan
    {
        public readonly SupportHexSpellVfxPhase Phase;
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

        public SupportHexSpellVfxArtPlan(
            SupportHexSpellVfxPhase phase,
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
            return SupportHexSpellVfxRules.IsAtlasCell(cell) ? cell : -1;
        }

        private static float Clamp01(float value)
        {
            return Math.Max(0f, Math.Min(1f, value));
        }
    }

    public static class SupportHexSpellVfxRules
    {
        public const int AtlasColumns = 4;
        public const int AtlasRows = 4;
        public const int AtlasCellCount = AtlasColumns * AtlasRows;

        public const int HolyCastRuneCell = 0;
        public const int MendingWispCell = 1;
        public const int HealDawnBloomCell = 2;
        public const int WardDomeCell = 3;
        public const int SunLanceCell = 4;
        public const int SunBrandHoldImpactCell = 5;
        public const int CleanseRiftSealCell = 6;
        public const int NatureStoneCreationCell = 7;
        public const int WebSnapCell = 8;
        public const int PoisonCloudBurstCell = 9;
        public const int SleepDreamMistCell = 10;
        public const int NightVeilCell = 11;
        public const int GraveHookCell = 12;
        public const int DrainLifeCell = 13;
        public const int MindBreakWitherCell = 14;
        public const int AshenCurseCell = 15;

        public static bool IsAtlasCell(int cell)
        {
            return cell >= 0 && cell < AtlasCellCount;
        }

        public static bool IsSupported(string visualOrFormulaKind)
        {
            switch (NormalizeKey(visualOrFormulaKind))
            {
                case "heal":
                case "ward":
                case "sun":
                case "cleanse":
                case "nature":
                case "web":
                case "poison":
                case "sleep":
                case "nightveil":
                case "gravehook":
                case "drainlife":
                case "mindbreak":
                case "ashencurse":
                    return true;
                default:
                    return false;
            }
        }

        public static string NormalizeKey(string visualOrFormulaKind)
        {
            string key = Compact(visualOrFormulaKind);
            switch (key)
            {
                case "oic":
                case "tnc":
                case "lbc":
                case "swr":
                case "dwp":
                case "heal":
                case "healing":
                case "regenerate":
                case "circleheal":
                case "stillwater":
                case "dawnpulse":
                case "mendingwisp":
                case "healdawnbloom":
                case "castmend":
                    return "heal";

                case "hlc":
                case "tbq":
                case "sgw":
                case "tbg":
                case "ward":
                case "shield":
                case "hallowedcircle":
                case "sanctuaryward":
                case "circleward":
                case "warddome":
                case "fieldholy":
                    return "ward";

                case "obl":
                case "lnh":
                case "sbn":
                case "light":
                case "lightbolt":
                case "holdsign":
                case "sunbrand":
                case "sunlance":
                case "sunbrandholdimpact":
                case "castlight":
                    return "sun";

                case "nvc":
                case "srf":
                case "cleanse":
                case "riftseal":
                case "unbinding":
                case "cleanseriftseal":
                case "castseal":
                    return "cleanse";

                case "gbh":
                case "gbx":
                case "treecover":
                case "stoneblock":
                case "nature":
                case "tree":
                case "stone":
                case "naturestonecreation":
                case "castnature":
                    return "nature";

                case "wbk":
                case "rkw":
                case "web":
                case "bind":
                case "websnare":
                case "websnap":
                    return "web";

                case "wbp":
                case "rpx":
                case "poison":
                case "poisongas":
                case "poisonburst":
                case "poisoncloud":
                case "poisoncloudburst":
                case "fieldgas":
                    return "poison";

                case "rms":
                case "dsm":
                case "sleep":
                case "sleepmist":
                case "dreamsmoke":
                case "dreammist":
                case "sleepdreammist":
                    return "sleep";

                case "nvl":
                case "nightveil":
                case "shadowveil":
                    return "nightveil";

                case "grh":
                case "gravehook":
                    return "gravehook";

                case "inh":
                case "drain":
                case "drainlife":
                case "lifedrain":
                    return "drainlife";

                case "rmb":
                case "wtr":
                case "rnh":
                case "mindbreak":
                case "wither":
                case "weaken":
                case "voidhex":
                case "mindbreakwither":
                    return "mindbreak";

                case "acr":
                case "ashencurse":
                    return "ashencurse";
            }

            if (key.Contains("dawn") || key.Contains("circleheal") || key.Contains("regenerat")) return "heal";
            if (key.Contains("ward") || key.Contains("sanctuary")) return "ward";
            if (key.Contains("sunbrand") || key.Contains("holdsign") || key.Contains("lightbolt")) return "sun";
            if (key.Contains("cleanse") || key.Contains("riftseal") || key.Contains("unbinding")) return "cleanse";
            if (key.Contains("treecover") || key.Contains("stoneblock")) return "nature";
            if (key.Contains("web") || key.Contains("bind")) return "web";
            if (key.Contains("poison")) return "poison";
            if (key.Contains("sleep") || key.Contains("dreamsmoke")) return "sleep";
            if (key.Contains("nightveil") || key.Contains("shadowveil")) return "nightveil";
            if (key.Contains("gravehook")) return "gravehook";
            if (key.Contains("drainlife") || key.Contains("lifedrain")) return "drainlife";
            if (key.Contains("mindbreak") || key.Contains("wither") || key.Contains("weaken")) return "mindbreak";
            if (key.Contains("ashencurse")) return "ashencurse";
            return string.IsNullOrEmpty(key) ? "spell" : key;
        }

        public static SupportHexSpellVfxProfile ProfileFor(string visualOrFormulaKind)
        {
            switch (NormalizeKey(visualOrFormulaKind))
            {
                case "heal":
                    return Profile("heal", HolyCastRuneCell, MendingWispCell, HealDawnBloomCell, HolyCastRuneCell, 1.05f, 0.96f, 0.32f, 0.34f, 0.58f, 24);
                case "ward":
                    return Profile("ward", HolyCastRuneCell, MendingWispCell, WardDomeCell, HolyCastRuneCell, 1.18f, 0.95f, 0.38f, 0.34f, 0.68f, 28);
                case "sun":
                    return Profile("sun", HolyCastRuneCell, SunLanceCell, SunBrandHoldImpactCell, HolyCastRuneCell, 1.18f, 1f, 0.42f, 0.36f, 0.70f, 32);
                case "cleanse":
                    return Profile("cleanse", HolyCastRuneCell, SunLanceCell, CleanseRiftSealCell, HolyCastRuneCell, 1.22f, 0.98f, 0.46f, 0.36f, 0.74f, 32);
                case "nature":
                    return Profile("nature", HolyCastRuneCell, -1, NatureStoneCreationCell, HolyCastRuneCell, 1.28f, 0.96f, 0.52f, 0f, 0.80f, 34);
                case "web":
                    return Profile("web", WebSnapCell, WebSnapCell, WebSnapCell, MindBreakWitherCell, 1.12f, 0.95f, 0.34f, 0.30f, 0.60f, 26);
                case "poison":
                    return Profile("poison", PoisonCloudBurstCell, PoisonCloudBurstCell, PoisonCloudBurstCell, -1, 1.18f, 0.94f, 0.38f, 0.34f, 0.72f, 30);
                case "sleep":
                    return Profile("sleep", SleepDreamMistCell, SleepDreamMistCell, SleepDreamMistCell, NightVeilCell, 1.22f, 0.93f, 0.42f, 0.34f, 0.76f, 30);
                case "nightveil":
                    return Profile("nightveil", NightVeilCell, NightVeilCell, NightVeilCell, SleepDreamMistCell, 1.24f, 0.95f, 0.44f, 0.36f, 0.74f, 28);
                case "gravehook":
                    return Profile("gravehook", GraveHookCell, GraveHookCell, GraveHookCell, MindBreakWitherCell, 1.20f, 0.98f, 0.38f, 0.34f, 0.68f, 30);
                case "drainlife":
                    return Profile("drainlife", DrainLifeCell, DrainLifeCell, DrainLifeCell, NightVeilCell, 1.18f, 0.98f, 0.38f, 0.40f, 0.70f, 32);
                case "mindbreak":
                    return Profile("mindbreak", MindBreakWitherCell, MindBreakWitherCell, MindBreakWitherCell, SleepDreamMistCell, 1.22f, 0.98f, 0.42f, 0.36f, 0.72f, 32);
                case "ashencurse":
                    return Profile("ashencurse", AshenCurseCell, AshenCurseCell, AshenCurseCell, MindBreakWitherCell, 1.40f, 1f, 0.54f, 0.40f, 0.86f, 42);
                default:
                    return Profile("spell", -1, -1, -1, -1, 1f, 0.88f, 0f, 0f, 0f, 0);
            }
        }

        public static SupportHexSpellVfxArtPlan CastPlan(
            string visualOrFormulaKind,
            int intensity = 1,
            float progress = 0.35f,
            bool reducedMotion = false)
        {
            return BuildPlan(ProfileFor(visualOrFormulaKind), SupportHexSpellVfxPhase.Cast, intensity, progress, reducedMotion);
        }

        public static SupportHexSpellVfxArtPlan ProjectilePlan(
            string visualOrFormulaKind,
            int intensity = 1,
            float progress = 0.35f,
            bool reducedMotion = false)
        {
            return BuildPlan(ProfileFor(visualOrFormulaKind), SupportHexSpellVfxPhase.Projectile, intensity, progress, reducedMotion);
        }

        public static SupportHexSpellVfxArtPlan ImpactPlan(
            string visualOrFormulaKind,
            int intensity = 1,
            float progress = 0.35f,
            bool reducedMotion = false)
        {
            return BuildPlan(ProfileFor(visualOrFormulaKind), SupportHexSpellVfxPhase.Impact, intensity, progress, reducedMotion);
        }

        public static float CastDuration(string visualOrFormulaKind)
        {
            return ProfileFor(visualOrFormulaKind).CastSeconds;
        }

        public static float ProjectileDuration(string visualOrFormulaKind)
        {
            return ProfileFor(visualOrFormulaKind).ProjectileSeconds;
        }

        public static float ImpactDuration(string visualOrFormulaKind)
        {
            return ProfileFor(visualOrFormulaKind).ImpactSeconds;
        }

        public static float PhaseDuration(SupportHexSpellVfxProfile profile, SupportHexSpellVfxPhase phase)
        {
            switch (phase)
            {
                case SupportHexSpellVfxPhase.Cast: return profile.CastSeconds;
                case SupportHexSpellVfxPhase.Projectile: return profile.ProjectileSeconds;
                case SupportHexSpellVfxPhase.Impact: return profile.ImpactSeconds;
                default: return 0f;
            }
        }

        public static float PhaseProgress(float elapsedSeconds, float durationSeconds, bool reducedMotion = false)
        {
            if (reducedMotion || durationSeconds <= 0f) return 1f;
            return Clamp01(Math.Max(0f, elapsedSeconds) / durationSeconds);
        }

        public static float ArtScale(
            SupportHexSpellVfxProfile profile,
            SupportHexSpellVfxPhase phase,
            int intensity,
            float progress)
        {
            int tier = ClampIntensity(intensity);
            float t = Clamp01(progress);
            float tierScale = 1f + (tier - 1) * 0.095f;
            float phaseScale;
            switch (phase)
            {
                case SupportHexSpellVfxPhase.Cast:
                    phaseScale = 0.72f + Smooth01(t) * 0.26f;
                    break;
                case SupportHexSpellVfxPhase.Projectile:
                    phaseScale = 0.68f + (float)Math.Sin(t * Math.PI) * 0.18f;
                    break;
                case SupportHexSpellVfxPhase.Impact:
                    phaseScale = 0.82f + (float)Math.Sin(t * Math.PI) * 0.40f;
                    break;
                default:
                    phaseScale = 1f;
                    break;
            }
            return Clamp(profile.BaseScale * tierScale * phaseScale, 0.42f, 2.45f);
        }

        public static float ArtOpacity(
            SupportHexSpellVfxProfile profile,
            SupportHexSpellVfxPhase phase,
            int intensity,
            float progress)
        {
            int tier = ClampIntensity(intensity);
            float t = Clamp01(progress);
            float pulse = 0.80f + (float)Math.Sin(t * Math.PI) * 0.20f;
            float phaseOpacity = phase == SupportHexSpellVfxPhase.Projectile ? 0.95f : pulse;
            return Clamp01(profile.BaseOpacity * phaseOpacity + (tier - 1) * 0.025f);
        }

        public static int StableVisualHash(string visualOrFormulaKind, int sampleIndex, int channel = 0)
        {
            unchecked
            {
                uint hash = 2166136261u;
                string key = NormalizeKey(visualOrFormulaKind);
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

        public static float StableVisualSample(string visualOrFormulaKind, int sampleIndex, int channel = 0)
        {
            uint hash = unchecked((uint)StableVisualHash(visualOrFormulaKind, sampleIndex, channel));
            return (hash & 0x00ffffffu) / 16777216f;
        }

        public static float StableVisualSignedSample(string visualOrFormulaKind, int sampleIndex, int channel = 0)
        {
            return StableVisualSample(visualOrFormulaKind, sampleIndex, channel) * 2f - 1f;
        }

        private static SupportHexSpellVfxArtPlan BuildPlan(
            SupportHexSpellVfxProfile profile,
            SupportHexSpellVfxPhase phase,
            int intensity,
            float progress,
            bool reducedMotion)
        {
            int tier = ClampIntensity(intensity);
            int primaryCell = PrimaryCell(profile, phase);
            if (primaryCell < 0)
            {
                return new SupportHexSpellVfxArtPlan(phase, -1, 0f, 0f, -1, 0f, 0f, 0f, 0);
            }

            float sampleProgress = reducedMotion ? 0.50f : Clamp01(progress);
            float scale = ArtScale(profile, phase, tier, sampleProgress);
            float opacity = ArtOpacity(profile, phase, tier, sampleProgress);
            int secondaryCell = SecondaryCell(profile, phase, tier, primaryCell, reducedMotion);
            float duration = PhaseDuration(profile, phase);
            if (reducedMotion) duration = Math.Min(0.10f, duration);
            return new SupportHexSpellVfxArtPlan(
                phase,
                primaryCell,
                scale,
                opacity,
                secondaryCell,
                secondaryCell < 0 ? 0f : Clamp(scale * (phase == SupportHexSpellVfxPhase.Impact ? 1.28f : 1.14f), 0.50f, 2.55f),
                secondaryCell < 0 ? 0f : Clamp01(opacity * 0.44f),
                duration,
                BurstCount(profile, phase, tier, reducedMotion));
        }

        private static int PrimaryCell(SupportHexSpellVfxProfile profile, SupportHexSpellVfxPhase phase)
        {
            switch (phase)
            {
                case SupportHexSpellVfxPhase.Cast: return profile.CastCell;
                case SupportHexSpellVfxPhase.Projectile: return profile.ProjectileCell;
                case SupportHexSpellVfxPhase.Impact: return profile.ImpactCell;
                default: return -1;
            }
        }

        private static int SecondaryCell(
            SupportHexSpellVfxProfile profile,
            SupportHexSpellVfxPhase phase,
            int intensity,
            int primaryCell,
            bool reducedMotion)
        {
            if (reducedMotion || phase == SupportHexSpellVfxPhase.Cast) return -1;
            int secondary = phase == SupportHexSpellVfxPhase.Projectile
                ? intensity >= 3 ? profile.CastCell : -1
                : intensity >= 2 ? profile.ImpactAccentCell : -1;
            return secondary == primaryCell ? -1 : secondary;
        }

        private static int BurstCount(
            SupportHexSpellVfxProfile profile,
            SupportHexSpellVfxPhase phase,
            int intensity,
            bool reducedMotion)
        {
            int count = profile.BaseBurstCount + (intensity - 1) * 5;
            if (phase == SupportHexSpellVfxPhase.Cast) count = (count + 1) / 2;
            if (phase == SupportHexSpellVfxPhase.Projectile) count = (count + 2) / 3;
            if (reducedMotion) count = Math.Max(1, count / 3);
            return Math.Max(0, Math.Min(64, count));
        }

        private static SupportHexSpellVfxProfile Profile(
            string key,
            int castCell,
            int projectileCell,
            int impactCell,
            int impactAccentCell,
            float baseScale,
            float baseOpacity,
            float castSeconds,
            float projectileSeconds,
            float impactSeconds,
            int baseBurstCount)
        {
            return new SupportHexSpellVfxProfile(
                key,
                castCell,
                projectileCell,
                impactCell,
                impactAccentCell,
                baseScale,
                baseOpacity,
                castSeconds,
                projectileSeconds,
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
