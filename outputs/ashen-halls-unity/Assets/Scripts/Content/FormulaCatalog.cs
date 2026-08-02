using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public static class FormulaCatalog
    {
        public const int SummonedTreeDuration = 8;

        private static readonly Dictionary<string, int> requiredLevels =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["OIC"] = 1,
                ["TBQ"] = 1,
                ["NVC"] = 2,
                ["OBL"] = 3,
                ["GBH"] = 4,
                ["TNC"] = 5,
                ["LNH"] = 6,
                ["GBX"] = 7,
                ["HLC"] = 8,
                ["SGW"] = 9,
                ["SRF"] = 10,
                ["LBC"] = 11,
                ["DWP"] = 12,
                ["TBG"] = 13,
                ["SWR"] = 14,
                ["SBN"] = 16,

                ["FIF"] = 1,
                ["RIG"] = 1,
                ["WBI"] = 2,
                ["WBF"] = 3,
                ["RCL"] = 4,
                ["BTF"] = 5,
                ["FBL"] = 6,
                ["RDF"] = 7,
                ["RSG"] = 8,
                ["FRB"] = 9,
                ["RBI"] = 10,
                ["RLF"] = 11,
                ["CLT"] = 12,
                ["MTR"] = 14,
                ["CNS"] = 15,
                ["VST"] = 16,
                ["AST"] = 20,

                ["RKW"] = 1,
                ["RNH"] = 1,
                ["WBK"] = 2,
                ["NVL"] = 3,
                ["RMS"] = 4,
                ["INH"] = 5,
                ["RMB"] = 6,
                ["WBP"] = 7,
                ["GRH"] = 8,
                ["RPX"] = 9,
                ["DMC"] = 10,
                ["WTR"] = 11,
                ["DSM"] = 14,
                ["RLM"] = 16,

                ["RBT"] = 1,
                ["IBD"] = 2,
                ["SLV"] = 4,
                ["PBR"] = 6,
                ["IBF"] = 8,
                ["VRS"] = 10,
                ["IBG"] = 14,
                ["DFA"] = 18,

                ["ACR"] = 18
            };

        public static readonly FormulaDef[] All =
        {
            new FormulaDef { Code = "GBH", Name = "Tree Cover", Hint = "grow breakable cover", School = "mend", Skill = "mend", Mana = 7, Range = 4, Target = "tile", Effect = "terrain", Terrain = "tree", Duration = SummonedTreeDuration, Arc = true },
            new FormulaDef { Code = "GBX", Name = "Stone Block", Hint = "raise stone cover", School = "mend", Skill = "mend", Mana = 6, Range = 4, Target = "tile", Effect = "terrain", Terrain = "stone", Duration = 0 },
            new FormulaDef { Code = "HLC", Name = "Hallowed Circle", Hint = "sanctuary ground", School = "mend", Skill = "mend", Mana = 7, Range = 4, Target = "tile", Effect = "terrain", Terrain = "sanctuary", Duration = 4, Arc = true },
            new FormulaDef { Code = "OIC", Name = "Heal", Hint = "heal ally", School = "mend", Skill = "mend", Mana = 5, Range = 4, Target = "ally", Effect = "heal", DamageType = "light", Power = 12 },
            new FormulaDef { Code = "NVC", Name = "Cleanse", Hint = "cure afflictions", School = "mend", Skill = "mend", Mana = 4, Range = 4, Target = "ally", Effect = "cure" },
            new FormulaDef { Code = "SRF", Name = "Rift Seal", Hint = "close rituals and unravel hostile fields", School = "mend|ember", Skill = "mend", Mana = 6, Range = 5, Target = "tile", Effect = "dispel", DamageType = "light", Arc = true },
            new FormulaDef { Code = "TBQ", Name = "Ward", Hint = "protect ally", School = "mend", Skill = "mend", Mana = 6, Range = 4, Target = "ally", Effect = "status", Status = "shield", Duration = 3 },
            new FormulaDef { Code = "SGW", Name = "Sanctuary Ward", Hint = "ward nearby allies", School = "mend", Skill = "mend", Mana = 8, Range = 4, Target = "ally", Effect = "status", Status = "shield", Duration = 2, Splash = true, Arc = true },
            new FormulaDef { Code = "TNC", Name = "Regenerate", Hint = "heal over time", School = "mend", Skill = "mend", Mana = 8, Range = 4, Target = "ally", Effect = "status", Status = "regen", Duration = 3 },
            new FormulaDef { Code = "LBC", Name = "Circle Heal", Hint = "heal nearby allies", School = "mend", Skill = "mend", Mana = 9, Range = 4, Target = "ally", Effect = "heal", DamageType = "light", Power = 8, Splash = true, Arc = true },
            new FormulaDef { Code = "TBG", Name = "Circle Ward", Hint = "ward nearby allies", School = "mend", Skill = "mend", Mana = 9, Range = 4, Target = "ally", Effect = "status", Status = "shield", Duration = 2, Splash = true, Arc = true },
            new FormulaDef { Code = "OBL", Name = "Light Bolt", Hint = "damage with light", School = "mend", Skill = "mend", Mana = 6, Range = 4, Target = "enemy", Effect = "damage", DamageType = "light", Power = 10 },
            new FormulaDef { Code = "LNH", Name = "Hold Sign", Hint = "briefly stun enemy", School = "mend", Skill = "mend", Mana = 7, Range = 4, Target = "enemy", Effect = "status", Status = "stun", DamageType = "light", Duration = 1 },
            new FormulaDef { Code = "SWR", Name = "Still Water", Hint = "steady nearby allies", School = "mend", Skill = "mend", Mana = 10, Range = 4, Target = "ally", Effect = "status", Status = "regen", Duration = 3, Splash = true, Arc = true },
            new FormulaDef { Code = "SBN", Name = "Sun Brand", Hint = "searing light burst", School = "mend", Skill = "mend", Mana = 10, Range = 5, Target = "enemy", Effect = "damage", DamageType = "light", Status = "stun", Power = 13, Duration = 1, Splash = true, Arc = true },

            new FormulaDef { Code = "FIF", Name = "Fire Spark", Hint = "small fire hit", School = "ember", Skill = "ember", Mana = 3, Range = 4, Target = "enemy", Effect = "damage", DamageType = "fire", Power = 9 },
            new FormulaDef { Code = "WBF", Name = "Fire Floor", Hint = "ignite floor", School = "ember", Skill = "ember", Mana = 6, Range = 4, Target = "tile", Effect = "terrain", Terrain = "fire", Duration = 4 },
            new FormulaDef { Code = "BTF", Name = "Burn Cover", Hint = "burn cover", School = "ember", Skill = "ember", Mana = 5, Range = 5, Target = "tile", Effect = "terrain", Terrain = "fire", Duration = 3, Arc = true },
            new FormulaDef { Code = "WBI", Name = "Ice Slick", Hint = "slick ice", School = "ember", Skill = "ember", Mana = 5, Range = 4, Target = "tile", Effect = "terrain", Terrain = "ice", Duration = 4 },
            new FormulaDef { Code = "RCL", Name = "Cold Lance", Hint = "long cold bolt", School = "ember", Skill = "ember", Mana = 6, Range = 6, Target = "enemy", Effect = "damage", DamageType = "cold", Power = 12 },
            new FormulaDef { Code = "RDF", Name = "Flame Jet", Hint = "fire and bleeding", School = "ember", Skill = "ember", Mana = 7, Range = 5, Target = "enemy", Effect = "damage", DamageType = "fire", Status = "bleed", Power = 13, Duration = 2 },
            new FormulaDef { Code = "RIG", Name = "Arc Spark", Hint = "reliable shock that conducts through hazards", School = "ember", Skill = "ember", Mana = 3, Range = 5, Target = "enemy", Effect = "damage", DamageType = "shock", Power = 8 },
            new FormulaDef { Code = "FBL", Name = "Fireball", Hint = "classic splash fire", School = "ember", Skill = "ember", Mana = 7, Range = 5, Target = "enemy", Effect = "damage", DamageType = "fire", Power = 15, Splash = true, Arc = true },
            new FormulaDef { Code = "RLF", Name = "Fireburst", Hint = "splash fire", School = "ember", Skill = "ember", Mana = 8, Range = 5, Target = "enemy", Effect = "damage", DamageType = "fire", Power = 14, Splash = true },
            new FormulaDef { Code = "RSG", Name = "Thunderclap", Hint = "blast and push adjacent enemies", School = "ember", Skill = "ember", Mana = 6, Range = 0, Target = "self", Effect = "thunderclap", DamageType = "shock", Power = 8 },
            new FormulaDef { Code = "RBI", Name = "Iceburst", Hint = "splash cold", School = "ember", Skill = "ember", Mana = 8, Range = 5, Target = "enemy", Effect = "damage", DamageType = "cold", Status = "stun", Power = 11, Duration = 1, Splash = true },
            new FormulaDef { Code = "MTR", Name = "Meteor Shower", Hint = "epic falling fire", School = "ember", Skill = "ember", Mana = 12, Range = 5, Target = "enemy", Effect = "damage", DamageType = "fire", Power = 18, Splash = true, Arc = true },
            new FormulaDef { Code = "CLT", Name = "Chain Lightning", Hint = "jumping shock through nearby enemies", School = "ember", Skill = "ember", Mana = 9, Range = 6, Target = "enemy", Effect = "chain", DamageType = "shock", Power = 14, Arc = true },
            new FormulaDef { Code = "FRB", Name = "Frost Bind", Hint = "cold lance that binds", School = "ember", Skill = "ember", Mana = 8, Range = 5, Target = "enemy", Effect = "damage", DamageType = "cold", Status = "web", Power = 12, Duration = 2 },
            new FormulaDef { Code = "VST", Name = "Thunder Step", Hint = "teleport and shock enemies beside the destination", School = "ember", Skill = "ember", Mana = 8, Range = 6, Target = "tile", Effect = "teleport", DamageType = "shock", Power = 8, Arc = true },
            new FormulaDef { Code = "AST", Name = "Arcane Tempest", Hint = "elder storm tears through a wide enemy formation", School = "ember", Skill = "ember", Mana = 14, Range = 6, Target = "enemy", Effect = "tempest", DamageType = "shock", Power = 18, Arc = true },

            new FormulaDef { Code = "WBK", Name = "Web Snare", Hint = "snare tile", School = "hex", Skill = "hex", Mana = 6, Range = 4, Target = "tile", Effect = "terrain", Terrain = "web", Duration = 4 },
            new FormulaDef { Code = "WBP", Name = "Poison Gas", Hint = "poison hazard", School = "hex", Skill = "hex", Mana = 7, Range = 4, Target = "tile", Effect = "terrain", Terrain = "gas", Duration = 4 },
            new FormulaDef { Code = "DMC", Name = "Doom Circle", Hint = "curse ground", School = "hex", Skill = "hex", Mana = 8, Range = 4, Target = "tile", Effect = "terrain", Terrain = "curse", Duration = 4, Arc = true },
            new FormulaDef { Code = "RMS", Name = "Sleep", Hint = "disable enemy", School = "hex", Skill = "hex", Mana = 6, Range = 5, Target = "enemy", Effect = "status", Status = "sleep", DamageType = "mind", Duration = 2 },
            new FormulaDef { Code = "RNH", Name = "Weaken", Hint = "lower defenses", School = "hex", Skill = "hex", Mana = 7, Range = 5, Target = "enemy", Effect = "status", Status = "hex", DamageType = "mind", Duration = 3 },
            new FormulaDef { Code = "NVL", Name = "Night Veil", Hint = "hide an ally", School = "hex", Skill = "hex", Mana = 6, Range = 4, Target = "ally", Effect = "status", Status = "stealth", DamageType = "mind", Duration = 2 },
            new FormulaDef { Code = "RKW", Name = "Bind", Hint = "web enemy", School = "hex", Skill = "hex", Mana = 5, Range = 4, Target = "enemy", Effect = "status", Status = "web", DamageType = "mind", Duration = 2 },
            new FormulaDef { Code = "RPX", Name = "Poison Burst", Hint = "splash poison", School = "hex", Skill = "hex", Mana = 8, Range = 5, Target = "enemy", Effect = "damage", DamageType = "poison", Status = "poison", Power = 8, Duration = 2, Splash = true },
            new FormulaDef { Code = "INH", Name = "Drain Life", Hint = "damage and heal", School = "hex", Skill = "hex", Mana = 7, Range = 4, Target = "enemy", Effect = "drain", DamageType = "death", Power = 11 },
            new FormulaDef { Code = "RMB", Name = "Mind Break", Hint = "mind hit and hex", School = "hex", Skill = "hex", Mana = 7, Range = 5, Target = "enemy", Effect = "damage", DamageType = "mind", Status = "hex", Power = 10, Duration = 2 },
            new FormulaDef { Code = "RLM", Name = "Death Burst", Hint = "splash death", School = "ember|hex", Skill = "hex", Mana = 9, Range = 5, Target = "enemy", Effect = "damage", DamageType = "death", Power = 16, Splash = true },
            new FormulaDef { Code = "WTR", Name = "Wither", Hint = "death mark and hex", School = "hex", Skill = "hex", Mana = 8, Range = 5, Target = "enemy", Effect = "damage", DamageType = "death", Status = "hex", Power = 13, Duration = 3 },
            new FormulaDef { Code = "DSM", Name = "Dream Smoke", Hint = "sleeping hex cloud", School = "hex", Skill = "hex", Mana = 10, Range = 5, Target = "enemy", Effect = "status", Status = "sleep", DamageType = "mind", Duration = 2, Splash = true, Arc = true },

            new FormulaDef { Code = "IBD", Name = "Summon Imp", Hint = "call a cheap blocking imp", School = "pact", Skill = "hex", Mana = 7, Range = 3, Target = "tile", Effect = "summon", SummonRole = "boundimp", DamageType = "death", Power = 6, Duration = 6, Arc = true },
            new FormulaDef { Code = "IBF", Name = "Summon Lesser Demon", Hint = "call a tougher clawed ally", School = "pact", Skill = "hex", Mana = 12, Range = 3, Target = "tile", Effect = "summon", SummonRole = "lesserdemon", DamageType = "death", Power = 10, Duration = 5, Arc = true },
            new FormulaDef { Code = "PBR", Name = "Pact Brand", Hint = "brand nearby enemies", School = "pact", Skill = "hex", Mana = 9, Range = 5, Target = "enemy", Effect = "status", Status = "hex", DamageType = "death", Duration = 3, Splash = true, Arc = true },
            new FormulaDef { Code = "IBG", Name = "Summon Greater Demon", Hint = "call a brutal elder demon", School = "pact", Skill = "hex", Mana = 17, Range = 3, Target = "tile", Effect = "summon", SummonRole = "greaterdemon", DamageType = "death", Power = 15, Duration = 4, Arc = true },
            new FormulaDef { Code = "DFA", Name = "Abyssal Ascendance", Hint = "take a greater demon's shape for a few brutal turns", School = "pact", Skill = "hex", Mana = 15, Range = 0, Target = "self", Effect = "transform", DamageType = "death", Power = 4, Duration = 4, Arc = true },
            new FormulaDef { Code = "RBT", Name = "Rift Bolt", Hint = "tear a compact death bolt through the rift", School = "pact", Skill = "hex", Mana = 4, Range = 5, Target = "enemy", Effect = "damage", DamageType = "death", Power = 8 },
            new FormulaDef { Code = "VRS", Name = "Rift Step", Hint = "cross between two linked rifts", School = "pact", Skill = "hex", Mana = 6, Range = 5, Target = "tile", Effect = "teleport", DamageType = "death", Arc = true },

            new FormulaDef { Code = "DWP", Name = "Dawn Pulse", Hint = "heal an ally and echo through nearby friends", School = "mend", Skill = "mend", Mana = 10, Range = 4, Target = "ally", Effect = "heal", DamageType = "light", Power = 11, Splash = true, Arc = true },
            new FormulaDef { Code = "CNS", Name = "Cinderstorm", Hint = "fiery area burst with a lingering bleed", School = "ember", Skill = "ember", Mana = 11, Range = 5, Target = "enemy", Effect = "damage", DamageType = "fire", Status = "bleed", Power = 16, Duration = 2, Splash = true, Arc = true },
            new FormulaDef { Code = "GRH", Name = "Grave Hook", Hint = "death strike that binds one enemy", School = "hex", Skill = "hex", Mana = 8, Range = 5, Target = "enemy", Effect = "damage", DamageType = "death", Status = "web", Power = 11, Duration = 2 },
            new FormulaDef { Code = "SLV", Name = "Soul Veil", Hint = "pact ward that spreads to nearby allies", School = "pact", Skill = "hex", Mana = 10, Range = 4, Target = "ally", Effect = "status", Status = "shield", Duration = 2, Splash = true, Arc = true },
            new FormulaDef { Code = "ACR", Name = "Ashen Curse", Hint = "splash fire that leaves a weakening hex", School = "ember|hex", Skill = "hex", Mana = 12, Range = 5, Target = "enemy", Effect = "damage", DamageType = "fire", Status = "hex", Power = 14, Duration = 2, Splash = true, Arc = true }
        };

        public static int RequiredLevel(FormulaDef formula)
        {
            if (formula == null) return ProgressionRules.MinimumLevel;
            return requiredLevels.TryGetValue(formula.Code ?? "", out int required)
                ? required
                : ProgressionRules.MaximumLevel;
        }

        public static bool HasExplicitRequiredLevel(string formulaCode)
        {
            return requiredLevels.ContainsKey(formulaCode ?? "");
        }
    }
}
