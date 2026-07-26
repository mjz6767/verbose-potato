using System;
using System.Linq;

namespace AshenHalls
{
    public static class CombatRitualRules
    {
        public static bool IsRitual(Point point)
        {
            return point != null && IsRitual(point.Kind);
        }

        public static bool IsRitual(string kind)
        {
            string normalized = Normalize(kind);
            return normalized == "glyph" || normalized == "demonrift";
        }

        public static int DefaultCountdown(string kind)
        {
            return Normalize(kind) == "demonrift" ? 4 : Normalize(kind) == "glyph" ? 3 : 0;
        }

        public static int MaxIntegrity(string kind)
        {
            return Normalize(kind) == "demonrift" ? 3 : Normalize(kind) == "glyph" ? 2 : 0;
        }

        public static string DisplayName(string kind)
        {
            return Normalize(kind) == "demonrift" ? "demon rift" : Normalize(kind) == "glyph" ? "summoning glyph" : "ritual";
        }

        public static string SpawnRole(string kind)
        {
            return Normalize(kind) == "demonrift" ? "lesserdemon" : Normalize(kind) == "glyph" ? "koboldraider" : "";
        }

        public static bool IsDispelableField(Point point)
        {
            if (point == null) return false;
            string kind = Normalize(point.Kind);
            return IsRitual(kind)
                || kind == "curse"
                || kind == "gas"
                || kind == "smoke"
                || kind == "web"
                || kind == "fire"
                || kind == "ice";
        }

        public static bool KeepsCombatAlive(CombatState combat)
        {
            return combat?.Obstacles != null && combat.Obstacles.Any(IsRitual);
        }

        public static int PhysicalDisruptionDamage(CombatUnit attacker, bool ranged)
        {
            if (attacker == null) return 0;
            int damage = 1;
            if (attacker.Power >= 10) damage++;
            string weapon = (attacker.WeaponName ?? "").ToLowerInvariant();
            if (!ranged && (weapon.Contains("axe") || weapon.Contains("hammer") || weapon.Contains("maul"))) damage++;
            if (string.Equals(attacker.DamageType, "light", StringComparison.OrdinalIgnoreCase)
                || string.Equals(attacker.DamageType, "shock", StringComparison.OrdinalIgnoreCase)) damage++;
            return Math.Max(1, Math.Min(3, damage));
        }

        private static string Normalize(string value)
        {
            return (value ?? "").Replace("_", "").Replace("-", "").Trim().ToLowerInvariant();
        }
    }
}
