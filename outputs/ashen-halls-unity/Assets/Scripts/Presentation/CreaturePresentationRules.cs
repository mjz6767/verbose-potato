using System;

namespace AshenHalls
{
    public static class CreatureAudioRules
    {
        public static string FactionFor(CombatUnit unit)
        {
            if (unit == null) return "";
            if (unit.DemonFormTurns > 0) return "demon";
            return FactionForIdentity(
                (unit.Name ?? "") + " "
                + (unit.Role ?? "") + " "
                + (unit.Race ?? "") + " "
                + (unit.ClassKey ?? ""));
        }

        public static string FactionForIdentity(string identity)
        {
            string text = (identity ?? "").Trim().ToLowerInvariant();
            if (text.Length == 0) return "";
            if (text.Contains("ratfolk")
                || text.Contains("ratcutthroat")
                || text.Contains("ratmage")
                || text.Contains("ratcleric")
                || text.Contains("ratbrute")
                || text.Contains("ratcaptain")
                || text.Contains("ratswarm")
                || text.Contains("plaguerats")
                || text.Contains("sewer rat")
                || text.Contains("giant rat")
                || text == "rat"
                || text == "rats"
                || text.StartsWith("rat ", StringComparison.Ordinal))
            {
                return "rat";
            }
            if (text.Contains("kobold")) return "kobold";
            if (text.Contains("drow")) return "drow";
            if (text.Contains("demon")
                || text.Contains("boundimp")
                || text.Contains(" imp")
                || text.StartsWith("imp ", StringComparison.Ordinal)
                || text.Contains("cinderling")
                || text.Contains("gloamknight"))
            {
                return "demon";
            }
            if (text.Contains("reaver")
                || text.Contains("bonepriest")
                || text.Contains("bone priest")
                || text.Contains("revenant")
                || text.Contains("husk")
                || text.Contains("shade")
                || text.Contains("meteorlich")
                || text.Contains("skeleton"))
            {
                return "undead";
            }
            return "";
        }

        public static string CueFor(CombatUnit unit, string action)
        {
            return CueForFaction(FactionFor(unit), action);
        }

        public static string CueForArchetype(string archetype, string action)
        {
            return CueForFaction(FactionForIdentity(archetype), action);
        }

        private static string CueForFaction(string faction, string action)
        {
            string cue = (action ?? "").Trim().ToLowerInvariant();
            if (faction == "rat")
            {
                switch (cue)
                {
                    case "alert":
                    case "step": return "ratchitter";
                    case "attack": return "ratattack";
                    case "cast": return "ratcast";
                    case "hurt": return "ratimpact";
                    case "death": return "ratdeath";
                    default: return "";
                }
            }

            if (faction != "kobold" && faction != "drow" && faction != "demon" && faction != "undead")
            {
                return "";
            }

            switch (cue)
            {
                case "alert":
                case "step":
                case "attack":
                case "cast":
                case "hurt":
                case "death":
                    return faction + cue;
                default:
                    return "";
            }
        }
    }

    public static class RoamingThreatPresentationRules
    {
        public static int SpriteIndex(string archetype)
        {
            switch ((archetype ?? "").Trim().ToLowerInvariant())
            {
                case "rats":
                case "rat": return 0;
                case "ratfolk":
                case "ratbrute": return 1;
                case "ratcleric":
                case "plaguerats": return 2;
                case "kobold":
                case "kobolds": return 5;
                case "koboldshaman": return 4;
                case "imp":
                case "boundimp": return 6;
                case "lesserdemon": return 7;
                case "greaterdemon":
                case "demon": return 8;
                case "drowscout": return 9;
                case "drowmage": return 10;
                case "drow": return 11;
                case "reaver":
                case "undead": return 12;
                case "bonepriest": return 13;
                case "ratswarm": return 14;
                case "ratcaptain": return 15;
                case "koboldking": return 16;
                case "redgatedemon": return 17;
                case "revenant": return 18;
                default: return 19;
            }
        }
    }
}
