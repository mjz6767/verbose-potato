using System;

namespace AshenHalls
{
    public readonly struct CombatFieldPresentationProfile
    {
        public readonly string Kind;
        public readonly string ActivationSfx;
        public readonly float SurfaceAlpha;
        public readonly float EdgeAlpha;
        public readonly float PulseSpeed;
        public readonly int PlacementBurstCount;
        public readonly float PlacementBurstSpeed;

        public CombatFieldPresentationProfile(
            string kind,
            string activationSfx,
            float surfaceAlpha,
            float edgeAlpha,
            float pulseSpeed,
            int placementBurstCount,
            float placementBurstSpeed)
        {
            Kind = kind ?? "";
            ActivationSfx = activationSfx ?? "";
            SurfaceAlpha = Clamp(surfaceAlpha, 0.08f, 0.34f);
            EdgeAlpha = Clamp(edgeAlpha, 0.30f, 0.92f);
            PulseSpeed = Clamp(pulseSpeed, 0.5f, 5f);
            PlacementBurstCount = Math.Max(4, Math.Min(24, placementBurstCount));
            PlacementBurstSpeed = Clamp(placementBurstSpeed, 0.5f, 1.8f);
        }

        private static float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }

    public static class CombatFieldPresentationRules
    {
        public static bool IsPersistentField(string kind)
        {
            switch (Normalize(kind))
            {
                case "fire":
                case "ice":
                case "gas":
                case "smoke":
                case "web":
                case "sanctuary":
                case "curse":
                    return true;
                default:
                    return false;
            }
        }

        public static bool UsesBaseTileDecoration(string kind)
        {
            return string.IsNullOrEmpty(Normalize(kind));
        }

        public static bool UsesDedicatedGroundSprite(string kind)
        {
            switch (Normalize(kind))
            {
                case "fire":
                case "ice":
                case "gas":
                case "smoke":
                case "web":
                case "sanctuary":
                case "curse":
                case "glyph":
                case "demonrift":
                    return true;
                default:
                    return false;
            }
        }

        public static bool UsesPropSprite(string kind)
        {
            return !IsPersistentField(kind);
        }

        public static bool UsesAlwaysOnTacticalFrame(string kind)
        {
            return false;
        }

        public static string DurationBadgeLabel(int rounds)
        {
            return rounds > 0 ? rounds + "R" : "";
        }

        public static bool DurationBadgeUrgent(int rounds)
        {
            return rounds == 1;
        }

        public static CombatFieldPresentationProfile For(string kind)
        {
            switch (Normalize(kind))
            {
                case "fire": return new CombatFieldPresentationProfile("fire", "fieldfire", 0.18f, 0.72f, 3.8f, 18, 1.46f);
                case "ice": return new CombatFieldPresentationProfile("ice", "fieldice", 0.16f, 0.68f, 2.1f, 14, 1.16f);
                case "gas": return new CombatFieldPresentationProfile("gas", "fieldgas", 0.14f, 0.56f, 1.2f, 13, 0.94f);
                case "smoke": return new CombatFieldPresentationProfile("smoke", "smoke", 0.17f, 0.60f, 1.0f, 15, 0.84f);
                case "web": return new CombatFieldPresentationProfile("web", "fieldsnare", 0.10f, 0.60f, 1.7f, 10, 0.88f);
                case "sanctuary": return new CombatFieldPresentationProfile("sanctuary", "fieldholy", 0.12f, 0.72f, 1.5f, 16, 1.12f);
                case "curse": return new CombatFieldPresentationProfile("curse", "fieldcurse", 0.15f, 0.74f, 2.6f, 16, 1.22f);
                default: return new CombatFieldPresentationProfile(Normalize(kind), "status", 0.12f, 0.48f, 1f, 8, 0.8f);
            }
        }

        public static float Pulse(CombatFieldPresentationProfile profile, float elapsedSeconds, int x, int y, bool reducedMotion)
        {
            if (reducedMotion) return 0.5f;
            double phase = elapsedSeconds * profile.PulseSpeed + x * 0.73d + y * 0.41d;
            return (float)(0.5d + Math.Sin(phase) * 0.5d);
        }

        public static float Drift(CombatFieldPresentationProfile profile, float elapsedSeconds, int x, int y, bool reducedMotion)
        {
            if (reducedMotion) return 0.5f;
            double value = elapsedSeconds * profile.PulseSpeed * 0.17d + x * 0.137d + y * 0.193d;
            return (float)(value - Math.Floor(value));
        }

        private static string Normalize(string value)
        {
            return (value ?? "").Trim().ToLowerInvariant();
        }
    }
}
