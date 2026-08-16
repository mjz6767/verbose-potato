using System;

namespace AshenHalls
{
    public enum MageWarlockSpellVfxPhase
    {
        Cast,
        Projectile,
        Impact
    }

    public readonly struct MageWarlockSpellVfxProfile
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

        public MageWarlockSpellVfxProfile(
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
            return cell >= 0 && cell < MageWarlockSpellVfxRules.AtlasCellCount ? cell : -1;
        }

        private static float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }

    public readonly struct MageWarlockSpellVfxArtPlan
    {
        public readonly MageWarlockSpellVfxPhase Phase;
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

        public MageWarlockSpellVfxArtPlan(
            MageWarlockSpellVfxPhase phase,
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
            return cell >= 0 && cell < MageWarlockSpellVfxRules.AtlasCellCount ? cell : -1;
        }

        private static float Clamp01(float value)
        {
            return Math.Max(0f, Math.Min(1f, value));
        }
    }

    public static class MageWarlockSpellVfxRules
    {
        public const int AtlasColumns = 4;
        public const int AtlasRows = 4;
        public const int AtlasCellCount = AtlasColumns * AtlasRows;

        public const int FireCastRuneCell = 0;
        public const int FireballProjectileCell = 1;
        public const int FireballImpactCell = 2;
        public const int MeteorCell = 3;
        public const int FrostLanceCell = 4;
        public const int FrostBurstCell = 5;
        public const int LightningCastRuneCell = 6;
        public const int TempestImpactCell = 7;
        public const int HexRuneCell = 8;
        public const int SoulProjectileCell = 9;
        public const int PactGateCell = 10;
        public const int AbyssalImpactCell = 11;
        public const int LesserSummonCell = 12;
        public const int GreaterSummonCell = 13;
        public const int AscendanceCell = 14;
        public const int DoomCircleCell = 15;

        public static bool IsAtlasCell(int cell)
        {
            return cell >= 0 && cell < AtlasCellCount;
        }

        public static bool IsSupported(string visualOrFormulaKind)
        {
            switch (NormalizeKey(visualOrFormulaKind))
            {
                case "fireball":
                case "meteor":
                case "frost":
                case "tempest":
                case "riftbolt":
                case "lessersummon":
                case "greatersummon":
                case "ascendance":
                case "doomcircle":
                case "soulveil":
                case "pactbrand":
                case "hex":
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
                case "fbl":
                case "fireball":
                case "fireballprojectile":
                case "fireballimpact":
                case "royalfireball":
                case "firecastrune":
                case "fif":
                case "rdf":
                case "rlf":
                case "fire":
                case "ember":
                case "castember":
                    return "fireball";
                case "mtr":
                case "cns":
                case "meteor":
                case "meteorsmall":
                case "meteorshower":
                case "cinderstorm":
                    return "meteor";
                case "rcl":
                case "rbi":
                case "frb":
                case "cold":
                case "ice":
                case "frost":
                case "coldlance":
                case "frostlance":
                case "frostburst":
                case "iceburst":
                case "frostbind":
                case "castfrost":
                    return "frost";
                case "rig":
                case "rsg":
                case "clt":
                case "vst":
                case "ast":
                case "shock":
                case "lightning":
                case "tempest":
                case "arcspark":
                case "thunderclap":
                case "chainlightning":
                case "thunderstep":
                case "arcanetempest":
                case "lightningcastrune":
                case "tempestimpact":
                case "castshock":
                case "casttempest":
                    return "tempest";
                case "rbt":
                case "vrs":
                case "riftbolt":
                case "riftstep":
                case "soulprojectile":
                case "pactgate":
                case "abyssalimpact":
                case "castpact":
                    return "riftbolt";
                case "ibd":
                case "ibf":
                case "summon":
                case "summonimp":
                case "impsummon":
                case "boundimp":
                case "lesserdemon":
                case "lessersummon":
                case "summonlesserdemon":
                    return "lessersummon";
                case "ibg":
                case "greaterdemon":
                case "greatersummon":
                case "summongreaterdemon":
                case "castgreatersummon":
                    return "greatersummon";
                case "dfa":
                case "ascendance":
                case "abyssalascendance":
                case "castascendance":
                case "transform":
                    return "ascendance";
                case "dmc":
                case "doomcircle":
                case "fieldcurse":
                    return "doomcircle";
                case "slv":
                case "soulveil":
                    return "soulveil";
                case "pbr":
                case "acr":
                case "pactbrand":
                case "ashencurse":
                    return "pactbrand";
                case "rlm":
                case "inh":
                case "wtr":
                case "rmb":
                case "rnh":
                case "grh":
                case "hex":
                case "death":
                case "deathburst":
                case "gravehook":
                case "hexrune":
                case "casthex":
                case "castdeathburst":
                case "void":
                case "mind":
                    return "hex";
            }

            if (key.Contains("greatersummon") || key.Contains("greaterdemon")) return "greatersummon";
            if (key.Contains("lessersummon") || key.Contains("lesserdemon") || key.Contains("summonimp") || key.Contains("impsummon")) return "lessersummon";
            if (key.Contains("ascend") || key.Contains("transform")) return "ascendance";
            if (key.Contains("doomcircle")) return "doomcircle";
            if (key.Contains("soulveil")) return "soulveil";
            if (key.Contains("pactbrand")) return "pactbrand";
            if (key.Contains("riftbolt") || key.Contains("pactgate")) return "riftbolt";
            if (key.Contains("meteor")) return "meteor";
            if (key.Contains("fireball")) return "fireball";
            if (key.Contains("frost") || key.Contains("cold") || key.Contains("ice")) return "frost";
            if (key.Contains("tempest") || key.Contains("shock") || key.Contains("lightning")) return "tempest";
            if (key.Contains("deathburst") || key.Contains("death") || key.Contains("hex")) return "hex";
            return string.IsNullOrEmpty(key) ? "spell" : key;
        }

        public static MageWarlockSpellVfxProfile ProfileFor(string visualOrFormulaKind)
        {
            switch (NormalizeKey(visualOrFormulaKind))
            {
                case "fireball":
                    return Profile("fireball", FireCastRuneCell, FireballProjectileCell, FireballImpactCell, FireCastRuneCell, 1.12f, 1f, 0.32f, 0.38f, 0.58f, 28);
                case "meteor":
                    return Profile("meteor", FireCastRuneCell, MeteorCell, MeteorCell, FireballImpactCell, 1.34f, 1f, 0.48f, 0.54f, 0.82f, 38);
                case "frost":
                    return Profile("frost", FrostLanceCell, FrostLanceCell, FrostBurstCell, FrostLanceCell, 1.02f, 0.94f, 0.28f, 0.30f, 0.54f, 24);
                case "tempest":
                    return Profile("tempest", LightningCastRuneCell, LightningCastRuneCell, TempestImpactCell, LightningCastRuneCell, 1.20f, 1f, 0.38f, 0.28f, 0.68f, 36);
                case "riftbolt":
                    return Profile("riftbolt", PactGateCell, SoulProjectileCell, AbyssalImpactCell, PactGateCell, 1.16f, 0.98f, 0.36f, 0.36f, 0.62f, 30);
                case "lessersummon":
                    return Profile("lessersummon", PactGateCell, -1, LesserSummonCell, PactGateCell, 1.28f, 1f, 0.58f, 0f, 0.76f, 34);
                case "greatersummon":
                    return Profile("greatersummon", PactGateCell, -1, GreaterSummonCell, PactGateCell, 1.48f, 1f, 0.74f, 0f, 0.92f, 44);
                case "ascendance":
                    return Profile("ascendance", PactGateCell, -1, AscendanceCell, PactGateCell, 1.52f, 1f, 0.78f, 0f, 1.00f, 48);
                case "doomcircle":
                    return Profile("doomcircle", HexRuneCell, -1, DoomCircleCell, HexRuneCell, 1.40f, 0.98f, 0.62f, 0f, 0.88f, 42);
                case "soulveil":
                    return Profile("soulveil", HexRuneCell, SoulProjectileCell, PactGateCell, HexRuneCell, 1.18f, 0.94f, 0.42f, 0.34f, 0.64f, 30);
                case "pactbrand":
                    return Profile("pactbrand", PactGateCell, SoulProjectileCell, DoomCircleCell, HexRuneCell, 1.34f, 0.98f, 0.52f, 0.34f, 0.82f, 40);
                case "hex":
                    return Profile("hex", HexRuneCell, SoulProjectileCell, AbyssalImpactCell, HexRuneCell, 1.10f, 0.96f, 0.36f, 0.34f, 0.60f, 28);
                default:
                    return Profile("spell", HexRuneCell, SoulProjectileCell, AbyssalImpactCell, HexRuneCell, 1f, 0.88f, 0.30f, 0.30f, 0.48f, 20);
            }
        }

        public static MageWarlockSpellVfxArtPlan CastPlan(
            string visualOrFormulaKind,
            int intensity = 1,
            float progress = 0.35f,
            bool reducedMotion = false)
        {
            return BuildPlan(ProfileFor(visualOrFormulaKind), MageWarlockSpellVfxPhase.Cast, intensity, progress, reducedMotion);
        }

        public static MageWarlockSpellVfxArtPlan ProjectilePlan(
            string visualOrFormulaKind,
            int intensity = 1,
            float progress = 0.35f,
            bool reducedMotion = false)
        {
            return BuildPlan(ProfileFor(visualOrFormulaKind), MageWarlockSpellVfxPhase.Projectile, intensity, progress, reducedMotion);
        }

        public static MageWarlockSpellVfxArtPlan ImpactPlan(
            string visualOrFormulaKind,
            int intensity = 1,
            float progress = 0.35f,
            bool reducedMotion = false)
        {
            return BuildPlan(ProfileFor(visualOrFormulaKind), MageWarlockSpellVfxPhase.Impact, intensity, progress, reducedMotion);
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

        public static float PhaseDuration(MageWarlockSpellVfxProfile profile, MageWarlockSpellVfxPhase phase)
        {
            switch (phase)
            {
                case MageWarlockSpellVfxPhase.Cast: return profile.CastSeconds;
                case MageWarlockSpellVfxPhase.Projectile: return profile.ProjectileSeconds;
                case MageWarlockSpellVfxPhase.Impact: return profile.ImpactSeconds;
                default: return 0f;
            }
        }

        public static float PhaseProgress(float elapsedSeconds, float durationSeconds, bool reducedMotion = false)
        {
            if (reducedMotion || durationSeconds <= 0f) return 1f;
            return Clamp01(Math.Max(0f, elapsedSeconds) / durationSeconds);
        }

        public static float ArtScale(
            MageWarlockSpellVfxProfile profile,
            MageWarlockSpellVfxPhase phase,
            int intensity,
            float progress)
        {
            int tier = ClampIntensity(intensity);
            float t = Clamp01(progress);
            float tierScale = 1f + (tier - 1) * 0.10f;
            float phaseScale;
            switch (phase)
            {
                case MageWarlockSpellVfxPhase.Cast:
                    phaseScale = 0.76f + Smooth01(t) * 0.24f;
                    break;
                case MageWarlockSpellVfxPhase.Projectile:
                    phaseScale = 0.72f + (float)Math.Sin(t * Math.PI) * 0.14f;
                    break;
                case MageWarlockSpellVfxPhase.Impact:
                    phaseScale = 0.84f + (float)Math.Sin(t * Math.PI) * 0.38f;
                    break;
                default:
                    phaseScale = 1f;
                    break;
            }
            return Clamp(profile.BaseScale * tierScale * phaseScale, 0.42f, 2.40f);
        }

        public static float ArtOpacity(
            MageWarlockSpellVfxProfile profile,
            MageWarlockSpellVfxPhase phase,
            int intensity,
            float progress)
        {
            int tier = ClampIntensity(intensity);
            float t = Clamp01(progress);
            float pulse = 0.82f + (float)Math.Sin(t * Math.PI) * 0.18f;
            float phaseOpacity = phase == MageWarlockSpellVfxPhase.Projectile ? 0.94f : pulse;
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

        private static MageWarlockSpellVfxArtPlan BuildPlan(
            MageWarlockSpellVfxProfile profile,
            MageWarlockSpellVfxPhase phase,
            int intensity,
            float progress,
            bool reducedMotion)
        {
            int tier = ClampIntensity(intensity);
            int primaryCell = PrimaryCell(profile, phase);
            if (primaryCell < 0)
            {
                return new MageWarlockSpellVfxArtPlan(phase, -1, 0f, 0f, -1, 0f, 0f, 0f, 0);
            }

            float sampleProgress = reducedMotion ? 0.50f : Clamp01(progress);
            float scale = ArtScale(profile, phase, tier, sampleProgress);
            float opacity = ArtOpacity(profile, phase, tier, sampleProgress);
            int secondaryCell = SecondaryCell(profile, phase, tier, primaryCell, reducedMotion);
            float duration = PhaseDuration(profile, phase);
            if (reducedMotion) duration = Math.Min(0.10f, duration);
            return new MageWarlockSpellVfxArtPlan(
                phase,
                primaryCell,
                scale,
                opacity,
                secondaryCell,
                secondaryCell < 0 ? 0f : Clamp(scale * (phase == MageWarlockSpellVfxPhase.Impact ? 1.30f : 1.16f), 0.50f, 2.55f),
                secondaryCell < 0 ? 0f : Clamp01(opacity * 0.46f),
                duration,
                BurstCount(profile, phase, tier, reducedMotion));
        }

        private static int PrimaryCell(MageWarlockSpellVfxProfile profile, MageWarlockSpellVfxPhase phase)
        {
            switch (phase)
            {
                case MageWarlockSpellVfxPhase.Cast: return profile.CastCell;
                case MageWarlockSpellVfxPhase.Projectile: return profile.ProjectileCell;
                case MageWarlockSpellVfxPhase.Impact: return profile.ImpactCell;
                default: return -1;
            }
        }

        private static int SecondaryCell(
            MageWarlockSpellVfxProfile profile,
            MageWarlockSpellVfxPhase phase,
            int intensity,
            int primaryCell,
            bool reducedMotion)
        {
            if (reducedMotion || phase == MageWarlockSpellVfxPhase.Cast) return -1;
            int secondary = phase == MageWarlockSpellVfxPhase.Projectile
                ? intensity >= 3 ? profile.CastCell : -1
                : intensity >= 2 ? profile.ImpactAccentCell : -1;
            return secondary == primaryCell ? -1 : secondary;
        }

        private static int BurstCount(
            MageWarlockSpellVfxProfile profile,
            MageWarlockSpellVfxPhase phase,
            int intensity,
            bool reducedMotion)
        {
            int count = profile.BaseBurstCount + (intensity - 1) * 5;
            if (phase == MageWarlockSpellVfxPhase.Cast) count = (count + 1) / 2;
            if (phase == MageWarlockSpellVfxPhase.Projectile) count = (count + 2) / 3;
            if (reducedMotion) count = Math.Max(1, count / 3);
            return Math.Max(0, Math.Min(64, count));
        }

        private static MageWarlockSpellVfxProfile Profile(
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
            return new MageWarlockSpellVfxProfile(
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
