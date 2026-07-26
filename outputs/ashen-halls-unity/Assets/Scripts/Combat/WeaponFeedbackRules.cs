using System;

namespace AshenHalls
{
    public enum WeaponFeedbackKind
    {
        Slash,
        Heavy,
        Thrust,
        Projectile
    }

    public readonly struct WeaponFeedbackProfile
    {
        public readonly WeaponFeedbackKind Kind;
        public readonly string ReleaseCue;
        public readonly string ContactCue;
        public readonly string VisualKind;
        public readonly float ReleaseVolume;
        public readonly float ContactVolume;
        public readonly float ImpactDelay;

        public WeaponFeedbackProfile(
            WeaponFeedbackKind kind,
            string releaseCue,
            string contactCue,
            string visualKind,
            float releaseVolume,
            float contactVolume,
            float impactDelay)
        {
            Kind = kind;
            ReleaseCue = releaseCue ?? "swing";
            ContactCue = contactCue ?? "bladecontact";
            VisualKind = visualKind ?? "weapon-slash";
            ReleaseVolume = Clamp(releaseVolume, 0f, 1f);
            ContactVolume = Clamp(contactVolume, 0f, 1f);
            ImpactDelay = Clamp(impactDelay, 0f, 0.20f);
        }

        private static float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }

    public static class WeaponFeedbackRules
    {
        public static WeaponFeedbackProfile For(string weaponName, bool ranged)
        {
            string weapon = (weaponName ?? "").Trim().ToLowerInvariant();
            if (ranged || ContainsAny(weapon, "bow", "crossbow", "sling"))
            {
                return new WeaponFeedbackProfile(
                    WeaponFeedbackKind.Projectile,
                    "arrowrelease",
                    "arrowcontact",
                    "weapon-arrow",
                    0.62f,
                    0.42f,
                    0.085f);
            }

            if (ContainsAny(weapon, "axe", "hammer", "maul", "great", "mace", "flail"))
            {
                return new WeaponFeedbackProfile(
                    WeaponFeedbackKind.Heavy,
                    "swingheavy",
                    "heavycontact",
                    "weapon-heavy",
                    0.58f,
                    0.52f,
                    0.075f);
            }

            if (ContainsAny(weapon, "spear", "pike", "epee", "dagger", "rapier", "lance"))
            {
                return new WeaponFeedbackProfile(
                    WeaponFeedbackKind.Thrust,
                    "thrust",
                    "thrustcontact",
                    "weapon-thrust",
                    0.52f,
                    0.40f,
                    0.055f);
            }

            return new WeaponFeedbackProfile(
                WeaponFeedbackKind.Slash,
                "swing",
                "bladecontact",
                "weapon-slash",
                0.52f,
                0.38f,
                0.065f);
        }

        public static float ContactVolume(WeaponFeedbackProfile profile, bool critical, bool guarding)
        {
            float volume = profile.ContactVolume;
            if (guarding) volume += 0.10f;
            if (critical) volume += 0.12f;
            return Math.Min(0.82f, volume);
        }

        public static int PresentationBurstCount(WeaponFeedbackProfile profile, bool critical)
        {
            int count;
            switch (profile.Kind)
            {
                case WeaponFeedbackKind.Heavy: count = 8; break;
                case WeaponFeedbackKind.Projectile: count = 5; break;
                default: count = 6; break;
            }
            return critical ? count + 4 : count;
        }

        public static string CoverContactCue(string coverKind)
        {
            return string.Equals((coverKind ?? "").Trim(), "tree", StringComparison.OrdinalIgnoreCase)
                ? "woodcontact"
                : "stonecontact";
        }

        public static string CoverBreakVisualKind(string coverKind)
        {
            return string.Equals((coverKind ?? "").Trim(), "tree", StringComparison.OrdinalIgnoreCase)
                ? "weapon-splinter"
                : "weapon-rubble";
        }

        public static float CoverContactVolume(WeaponFeedbackProfile profile, bool broken)
        {
            float volume = profile.ContactVolume + (broken ? 0.16f : 0.06f);
            return Math.Min(0.78f, volume);
        }

        private static bool ContainsAny(string value, params string[] fragments)
        {
            foreach (string fragment in fragments)
            {
                if (value.IndexOf(fragment, StringComparison.Ordinal) >= 0) return true;
            }
            return false;
        }
    }
}
