using System;

namespace AshenHalls
{
    public static class CombatFeedbackRules
    {
        public static int FloatIconIndex(string label, string explicitKind = null)
        {
            string kind = Normalize(explicitKind);
            if (kind.Length > 0)
            {
                int explicitIndex = KindIconIndex(kind);
                if (explicitIndex >= 0) return explicitIndex;
            }

            string text = Normalize(label);
            if (text.Length == 0) return -1;
            if (text[0] == '+') return 2;
            if (text[0] == '-' && text.Length > 1 && char.IsDigit(text[1])) return 0;
            if (ContainsAny(text, "heal", "mend", "regen", "restored", "renew")) return 2;
            if (ContainsAny(text, "cure", "cleanse", "purge")) return 7;
            if (ContainsAny(text, "ward", "shield", "guard", "brace", "resist", "blocked")) return 3;
            if (ContainsAny(text, "poison", "venom", "gas")) return 5;
            if (ContainsAny(text, "bleed", "blood")) return 6;
            if (ContainsAny(text, "sleep", "dream", "wakes")) return 8;
            if (ContainsAny(text, "web", "snare")) return 9;
            if (ContainsAny(text, "fire", "burn", "flame", "meteor", "ember")) return 10;
            if (ContainsAny(text, "cold", "ice", "frost", "frozen")) return 11;
            if (ContainsAny(text, "shock", "lightning", "spark")) return 12;
            if (ContainsAny(text, "death", "drain", "doom", "vorpal")) return 13;
            if (ContainsAny(text, "hex", "curse", "mind", "fear")) return 14;
            if (ContainsAny(text, "stealth", "hidden", "vanish")) return 15;
            if (ContainsAny(text, "rally", "enrage", "focus")) return 16;
            if (ContainsAny(text, "stun", "pinned", "hamstring", "slow", "broken", "stagger")) return 17;
            if (ContainsAny(text, "mark", "weak", "target")) return 18;
            if (ContainsAny(text, "gold", "loot", "cache")) return 19;
            if (ContainsAny(text, "miss", "smoke", "evade")) return 4;
            if (ContainsAny(text, "light", "sun", "hallow", "radiant")) return 1;
            if (ContainsAny(text, "hit", "slash", "volley", "impact")) return 0;
            return -1;
        }

        public static float FloatAlpha(float progress)
        {
            if (progress <= 0.64f) return 1f;
            if (progress >= 1f) return 0f;
            float fade = (progress - 0.64f) / 0.36f;
            return 1f - fade * fade * (3f - 2f * fade);
        }

        public static int RangerImpactIndex(string abilityId)
        {
            switch (Normalize(abilityId))
            {
                case "aimedshot": return 0;
                case "pinningshot": return 1;
                case "volley": return 2;
                case "scoutmark": return 3;
                case "broadheadshot": return 4;
                case "disruptingshot": return 7;
                default: return -1;
            }
        }

        public static int MagicUiIconIndex(FormulaDef formula, string schoolFallback = null)
        {
            string effect = Normalize(formula?.Effect);
            string type = Normalize(formula?.DamageType);
            string terrain = Normalize(formula?.Terrain);
            string status = Normalize(formula?.Status);
            string school = Normalize(formula?.School ?? schoolFallback);
            if (terrain == "tree") return 0;
            if (terrain == "stone") return 1;
            if (effect == "heal" || status == "regen") return 2;
            if (status == "shield" || effect == "ward") return 3;
            if (terrain == "fire" || type == "fire" || school.Contains("ember")) return 4;
            if (terrain == "ice" || type == "cold") return 5;
            if (type == "shock") return 6;
            if (type == "death") return 7;
            if (terrain == "web" || status == "web") return 8;
            if (terrain == "gas" || type == "poison") return 9;
            if (type == "mind" || status == "hex" || status == "sleep" || terrain == "curse" || school.Contains("hex")) return 10;
            if (effect == "summon" || school.Contains("pact")) return 11;
            if (type == "light") return 12;
            if (effect == "cure") return 13;
            if (terrain == "sanctuary") return 14;
            return 15;
        }

        public static string SpellGlyphKind(FormulaDef formula, string requestedKind)
        {
            string kind = string.IsNullOrWhiteSpace(requestedKind)
                ? formula != null && formula.Splash ? "area" : "impact"
                : requestedKind;
            if (formula == null) return kind;
            if (kind == "fireball" || kind == "meteor" || kind.StartsWith("ranger:", StringComparison.OrdinalIgnoreCase)) return kind;

            string effect = Normalize(formula.Effect);
            string type = Normalize(formula.DamageType);
            string terrain = Normalize(formula.Terrain);
            string status = Normalize(formula.Status);
            string school = Normalize(formula.School);
            if (kind == "caster")
            {
                if (effect == "summon" || school.Contains("pact")) return "spellanim:13";
                if (effect == "heal" || effect == "cure" || school.Contains("mend")) return "spellanim:12";
                if (type == "fire" || terrain == "fire" || school.Contains("ember")) return "spellanim:0";
                if (type == "death" || type == "mind" || type == "poison" || school.Contains("hex")) return "spellanim:8";
                if (type == "light" || type == "shock" || type == "cold" || status == "shield" || terrain == "sanctuary") return "spellanim:14";
                return "";
            }

            int signatureIndex = CombatIconCatalog.SignatureSpellIndex(formula.Code);
            if (signatureIndex >= 0) return "signature:" + signatureIndex;
            if (effect == "summon" || school.Contains("pact")) return "spellanim:13";
            if (effect == "heal" || effect == "cure" || school.Contains("mend")) return "spellanim:12";
            if (type == "shock") return "spellanim:10";
            if (type == "cold" || terrain == "ice") return "spellanim:11";
            if (type == "death") return "spellanim:15";
            if (type == "mind" || type == "poison" || school.Contains("hex")) return "spellanim:8";
            if (type == "fire") return "spellanim:2";
            if (terrain == "fire") return "spellanim:3";
            if (terrain == "tree" || terrain == "stone" || terrain == "web" || terrain == "gas" || terrain == "sanctuary" || terrain == "curse" || type == "light" || status == "shield")
            {
                return "magicui:" + MagicUiIconIndex(formula);
            }
            if (effect == "status") return "magicui:" + MagicUiIconIndex(formula);
            return kind;
        }

        private static int KindIconIndex(string kind)
        {
            switch (kind)
            {
                case "physical": return 0;
                case "light": return 1;
                case "heal": return 2;
                case "ward": return 3;
                case "smoke": return 4;
                case "poison": return 5;
                case "bleed": return 6;
                case "cleanse": return 7;
                case "sleep": return 8;
                case "web": return 9;
                case "fire": return 10;
                case "cold": return 11;
                case "shock": return 12;
                case "death": return 13;
                case "mind":
                case "hex": return 14;
                case "stealth": return 15;
                case "rally": return 16;
                case "stun": return 17;
                case "mark": return 18;
                case "loot": return 19;
                default: return -1;
            }
        }

        private static string Normalize(string value)
        {
            return (value ?? "").Trim().ToLowerInvariant();
        }

        private static bool ContainsAny(string value, params string[] needles)
        {
            for (int i = 0; i < needles.Length; i++)
            {
                if (value.Contains(needles[i])) return true;
            }
            return false;
        }
    }
}
