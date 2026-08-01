using System;

namespace AshenHalls
{
    public readonly struct CombatPowerIdentity
    {
        public readonly string Title;
        public readonly string Sigil;
        public readonly string Subtitle;
        public readonly string AccentHex;
        public readonly int Intensity;
        public readonly float Duration;

        public CombatPowerIdentity(string title, string sigil, string subtitle, string accentHex, int intensity, float duration)
        {
            Title = title ?? "";
            Sigil = sigil ?? "";
            Subtitle = subtitle ?? "";
            AccentHex = accentHex ?? "d7a84e";
            Intensity = Math.Max(1, Math.Min(3, intensity));
            Duration = Math.Max(0.6f, duration);
        }
    }

    public static class CombatPowerPresentationRules
    {
        public static CombatPowerIdentity ForFormula(FormulaDef formula, string actorName, string targetName)
        {
            return ForFormula(formula, actorName, targetName, false);
        }

        public static CombatPowerIdentity ForFormula(FormulaDef formula, string actorName, string targetName, bool focused)
        {
            if (formula == null) return new CombatPowerIdentity("Spell", "---", actorName ?? "", "9d74c9", 1, 1.05f);
            int intensity = FormulaIntensity(formula);
            string school = FormulaSchoolLabel(formula);
            string target = string.IsNullOrWhiteSpace(targetName) ? FormulaTargetLabel(formula) : targetName;
            string subtitle = JoinParts(actorName, focused ? "FOCUSED " + school : school, target);
            return new CombatPowerIdentity(formula.Name, formula.Code, subtitle, FormulaAccent(formula), intensity, 0.92f + intensity * 0.18f + (focused ? 0.08f : 0f));
        }

        public static CombatPowerIdentity ForAbility(MartialAbility ability, string actorName, string targetName)
        {
            if (ability == null) return new CombatPowerIdentity("Combat Skill", "---", actorName ?? "", "d7a84e", 1, 1.05f);
            int intensity = AbilityIntensity(ability.Id);
            string role = string.IsNullOrWhiteSpace(ability.ClassKey) ? "skill" : ability.ClassKey;
            string target = string.IsNullOrWhiteSpace(targetName) ? (ability.Targeted ? "targeted" : "self") : targetName;
            string subtitle = JoinParts(actorName, role, target);
            return new CombatPowerIdentity(ability.Name, ability.Short, subtitle, AbilityAccent(ability.ClassKey), intensity, 0.92f + intensity * 0.18f);
        }

        public static CombatPowerIdentity ForEnemyPower(string powerKey, string actorName, string targetName)
        {
            string key = NormalizeEnemyPowerKey(powerKey);
            int intensity = EnemyPowerIntensity(key);
            string subtitle = JoinParts(actorName, "enemy power", targetName);
            return new CombatPowerIdentity(
                EnemyPowerTitle(key),
                EnemyPowerSigil(key),
                subtitle,
                EnemyPowerAccent(key),
                intensity,
                0.92f + intensity * 0.18f);
        }

        public static int FormulaIntensity(FormulaDef formula)
        {
            if (formula == null) return 1;
            switch (formula.Code)
            {
                case "RIG": return 1;
                case "RSG":
                case "CLT":
                case "VST": return 2;
                case "VRS": return 2;
                case "AST": return 3;
            }
            if (formula.Code == "FBL" || formula.Code == "MTR" || formula.Code == "RLM" || formula.Code == "IBG" || formula.Code == "DFA") return 3;
            if (formula.Code == "SRF" || formula.Splash || formula.Effect == "summon") return 2;
            return 1;
        }

        public static int AbilityIntensity(string abilityId)
        {
            switch ((abilityId ?? "").ToLowerInvariant())
            {
                case "whirlwind":
                case "volley":
                case "eviscerate":
                case "riftpounce":
                case "abyssalwhirl":
                case "soulrend":
                case "dreadroar":
                    return 3;
                case "charge":
                case "execute":
                case "ambush":
                case "smokebomb":
                case "broadheadshot":
                case "disruptingshot":
                    return 2;
                default:
                    return 1;
            }
        }

        public static string FormulaAccent(FormulaDef formula)
        {
            if (formula == null) return "9d74c9";
            string type = formula.DamageType ?? "";
            string terrain = formula.Terrain ?? "";
            string school = formula.School ?? "";
            if (type == "shock") return "d7b94e";
            if (type == "fire" || terrain == "fire" || school.Contains("ember")) return "df7040";
            if (type == "cold" || terrain == "ice") return "66bdd6";
            if (type == "light" || school.Contains("mend")) return "58b7a5";
            if (terrain == "tree" || terrain == "stone") return "7f9b5c";
            if (type == "death" || type == "mind" || school.Contains("hex") || school.Contains("pact")) return "9d74c9";
            return "b7aa90";
        }

        public static string AbilityAccent(string classKey)
        {
            switch ((classKey ?? "").ToLowerInvariant())
            {
                case "warrior": return "d7a84e";
                case "rogue": return "9d74c9";
                case "ranger": return "58b7a5";
                case "demon": return "c6576d";
                default: return "b7aa90";
            }
        }

        public static int EnemyPowerIntensity(string powerKey)
        {
            switch (NormalizeEnemyPowerKey(powerKey))
            {
                case "deathball":
                case "royalfireball":
                case "royalcharge":
                    return 3;
                case "bonehex":
                case "coldsplinter":
                case "plaguesigns":
                case "darklight":
                case "burningpact":
                case "royalrally":
                case "royalicelance":
                    return 2;
                default:
                    return 1;
            }
        }

        public static string EnemyPowerFormulaArtCode(string powerKey)
        {
            switch (NormalizeEnemyPowerKey(powerKey))
            {
                case "graveward": return "SGW";
                case "bonehex": return "RNH";
                case "deathball": return "RLM";
                case "shocksign": return "RIG";
                case "coldsplinter":
                case "royalicelance": return "RCL";
                case "plaguesigns": return "RPX";
                case "darklight": return "RMB";
                case "venomdust": return "WBP";
                case "cindertrail":
                case "burningpact": return "RDF";
                case "dreamveil": return "DSM";
                case "royalfireball": return "FBL";
                default: return "";
            }
        }

        public static string EnemyPowerAbilityArtId(string powerKey)
        {
            switch (NormalizeEnemyPowerKey(powerKey))
            {
                case "royalrally": return "rally";
                case "royalcharge": return "charge";
                default: return "";
            }
        }

        public static string EnemyPowerTitle(string powerKey)
        {
            switch (NormalizeEnemyPowerKey(powerKey))
            {
                case "graveward": return "Grave Ward";
                case "bonehex": return "Bone Hex";
                case "deathball": return "Death Ball";
                case "shocksign": return "Shock Sign";
                case "coldsplinter": return "Cold Splinter";
                case "plaguesigns": return "Plague Signs";
                case "darklight": return "Dark Light";
                case "venomdust": return "Venom Dust";
                case "cindertrail": return "Cinder Trail";
                case "burningpact": return "Burning Pact";
                case "dreamveil": return "Dream Veil";
                case "royalrally": return "Royal Aegis";
                case "royalcharge": return "King's Charge";
                case "royalfireball": return "Crooked Fireball";
                case "royalicelance": return "Royal Ice Lance";
                default: return "Enemy Power";
            }
        }

        private static string EnemyPowerSigil(string powerKey)
        {
            switch (NormalizeEnemyPowerKey(powerKey))
            {
                case "graveward": return "GRV";
                case "bonehex": return "HEX";
                case "deathball": return "RLM";
                case "shocksign": return "SIG";
                case "coldsplinter": return "ICE";
                case "plaguesigns": return "PLG";
                case "darklight": return "DRK";
                case "venomdust": return "VEN";
                case "cindertrail": return "CND";
                case "burningpact": return "PCT";
                case "dreamveil": return "SLP";
                case "royalrally": return "WRD";
                case "royalcharge": return "CHG";
                case "royalfireball": return "FBL";
                case "royalicelance": return "LNC";
                default: return "FOE";
            }
        }

        private static string EnemyPowerAccent(string powerKey)
        {
            switch (NormalizeEnemyPowerKey(powerKey))
            {
                case "graveward":
                case "shocksign":
                case "royalrally":
                case "royalcharge": return "d7a84e";
                case "coldsplinter":
                case "royalicelance": return "66bdd6";
                case "plaguesigns":
                case "venomdust": return "7f9b5c";
                case "cindertrail":
                case "burningpact":
                case "royalfireball": return "df7040";
                default: return "9d74c9";
            }
        }

        private static string NormalizeEnemyPowerKey(string powerKey)
        {
            return (powerKey ?? "").Replace("_", "").Replace("-", "").Trim().ToLowerInvariant();
        }

        private static string FormulaSchoolLabel(FormulaDef formula)
        {
            string school = formula?.School ?? "spell";
            int separator = school.IndexOf('|');
            return separator >= 0 ? school.Substring(0, separator) : school;
        }

        private static string FormulaTargetLabel(FormulaDef formula)
        {
            if (formula == null) return "target";
            switch (formula.Code)
            {
                case "RIG": return "single target";
                case "RSG": return "adjacent enemies";
                case "CLT": return "jumping targets";
                case "VST": return "destination burst";
                case "AST": return "storm area";
            }
            if (formula.Effect == "summon") return "summon";
            if (formula.Splash) return "area";
            return string.IsNullOrWhiteSpace(formula.Target) ? "target" : formula.Target;
        }

        private static string JoinParts(string first, string second, string third)
        {
            string a = string.IsNullOrWhiteSpace(first) ? "" : first.Trim();
            string b = string.IsNullOrWhiteSpace(second) ? "" : second.Trim();
            string c = string.IsNullOrWhiteSpace(third) ? "" : third.Trim();
            if (a.Length == 0) return b.Length == 0 ? c : c.Length == 0 ? b : b + " / " + c;
            if (b.Length == 0) return c.Length == 0 ? a : a + " / " + c;
            return c.Length == 0 ? a + " / " + b : a + " / " + b + " / " + c;
        }
    }
}
