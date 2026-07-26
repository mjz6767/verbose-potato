namespace AshenHalls
{
    public static class FormulaCatalog
    {
        public const int SummonedTreeDuration = 8;

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
            new FormulaDef { Code = "DFA", Name = "Abyssal Ascendance", Hint = "take a greater demon's shape for a few brutal turns", School = "pact", Skill = "hex", Mana = 15, Range = 0, Target = "self", Effect = "transform", DamageType = "death", Power = 4, Duration = 4, Arc = true }
        };

        public static int RequiredLevel(FormulaDef formula)
        {
            if (formula == null) return 1;
            switch (formula.Code)
            {
                case "GBH":
                case "OIC":
                case "NVC":
                case "TBQ":
                case "FIF":
                case "WBF":
                case "WBI":
                case "RIG":
                case "WBK":
                case "RKW":
                case "IBD":
                    return 1;
                case "GBX":
                case "SGW":
                case "TNC":
                case "OBL":
                case "LNH":
                case "BTF":
                case "RCL":
                case "RDF":
                case "RSG":
                case "FRB":
                case "WBP":
                case "RMS":
                case "RNH":
                case "NVL":
                case "RPX":
                case "INH":
                case "WTR":
                case "PBR":
                    return 2;
                case "IBF":
                case "SRF":
                case "CLT":
                    return 3;
                case "VST":
                    return 4;
                case "IBG":
                    return 5;
                case "AST":
                case "DFA":
                    return 6;
                default:
                    return 3;
            }
        }
    }
}
