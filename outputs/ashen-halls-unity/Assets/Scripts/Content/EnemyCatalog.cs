namespace AshenHalls
{
    public static class EnemyCatalog
    {
        public static readonly string[] Ids =
        {
            "sewerrat", "giantrat", "adept", "husk", "reaver", "spore", "shade", "glassmage", "thornbeast", "mirearcher",
            "bonepriest", "cinderling", "gloamknight", "koboldraider", "koboldslinger", "koboldshaman", "koboldwizard",
            "koboldshield", "koboldking", "ratfolk", "ratcutthroat", "ratmage", "ratcleric", "ratbrute", "drowscout",
            "drowblade", "drowcrossbow", "drowmage", "drowpriest", "lesserdemon", "meteorlich", "ritualheart"
        };

        public static EnemyTemplate For(string kind)
        {
            switch (kind)
            {
                case "sewerrat": return new EnemyTemplate { Name = "Sewer Rat", Hp = 8, Power = 4, Defense = 0, Agility = 8, Range = 1, Color = "#7a6f5f", DamageType = "physical", Weakness = "fire" };
                case "giantrat": return new EnemyTemplate { Name = "Giant Rat", Hp = 13, Power = 5, Defense = 1, Agility = 7, Range = 1, Color = "#8f7b64", DamageType = "poison", Weakness = "fire", StatusOnHit = "poison" };
                case "adept": return new EnemyTemplate { Name = "Dust Adept", Hp = 15, Power = 7, Defense = 1, Agility = 6, Range = 3, Color = "#7b8c99", DamageType = "shock", Resist = "mind", Weakness = "physical", MagicResist = 2 };
                case "husk": return new EnemyTemplate { Name = "Iron Husk", Hp = 28, Power = 8, Defense = 5, Agility = 2, Range = 1, Color = "#8d9387", DamageType = "physical", Resist = "physical|poison", Weakness = "shock", MagicResist = 1, Fearless = true };
                case "reaver": return new EnemyTemplate { Name = "Grave Reaver", Hp = 22, Power = 9, Defense = 3, Agility = 5, Range = 1, Color = "#a34d52", DamageType = "physical", Weakness = "cold", StatusOnHit = "bleed" };
                case "spore": return new EnemyTemplate { Name = "Vermin Spore", Hp = 18, Power = 7, Defense = 2, Agility = 3, Range = 2, Color = "#7f9d5b", DamageType = "poison", Resist = "poison", Weakness = "fire", StatusOnHit = "poison" };
                case "shade": return new EnemyTemplate { Name = "Candle Shade", Hp = 20, Power = 8, Defense = 2, Agility = 7, Range = 1, Color = "#6f617e", DamageType = "death", Resist = "death|physical", Weakness = "fire", MagicResist = 3, Fearless = true };
                case "glassmage": return new EnemyTemplate { Name = "Glass Mage", Hp = 16, Power = 8, Defense = 1, Agility = 5, Range = 4, Color = "#9ad6e8", DamageType = "cold", Resist = "cold|shock", Weakness = "physical", MagicResist = 4 };
                case "thornbeast": return new EnemyTemplate { Name = "Thorn Beast", Hp = 30, Power = 10, Defense = 4, Agility = 3, Range = 1, Color = "#8f6f42", DamageType = "physical", Resist = "poison", Weakness = "fire", StatusOnHit = "bleed", Fearless = true };
                case "mirearcher": return new EnemyTemplate { Name = "Mire Archer", Hp = 17, Power = 7, Defense = 2, Agility = 6, Range = 4, Color = "#6f8f64", DamageType = "poison", Resist = "poison", Weakness = "fire", StatusOnHit = "poison" };
                case "bonepriest": return new EnemyTemplate { Name = "Bone Priest", Hp = 18, Power = 6, Defense = 2, Agility = 4, Range = 3, Color = "#d9d3c4", DamageType = "light", Resist = "death|mind", Weakness = "shock", MagicResist = 4 };
                case "cinderling": return new EnemyTemplate { Name = "Cinderling", Hp = 19, Power = 8, Defense = 2, Agility = 6, Range = 3, Color = "#c65c3b", DamageType = "fire", Resist = "fire", Weakness = "cold", StatusOnHit = "bleed" };
                case "gloamknight": return new EnemyTemplate { Name = "Gloam Knight", Hp = 32, Power = 10, Defense = 5, Agility = 3, Range = 1, Color = "#6f617e", DamageType = "death", Resist = "death|physical", Weakness = "light|shock", MagicResist = 2, Fearless = true };
                case "koboldraider": return new EnemyTemplate { Name = "Kobold Raider", Hp = 16, Power = 7, Defense = 2, Agility = 7, Range = 1, Color = "#7f9d5b", DamageType = "physical", Weakness = "cold", StatusOnHit = "bleed" };
                case "koboldslinger": return new EnemyTemplate { Name = "Kobold Slinger", Hp = 14, Power = 6, Defense = 1, Agility = 8, Range = 4, Color = "#8f8f58", DamageType = "physical", Weakness = "shock", StatusOnHit = "stun" };
                case "koboldshaman": return new EnemyTemplate { Name = "Kobold Shaman", Hp = 17, Power = 7, Defense = 1, Agility = 5, Range = 4, Color = "#6f8f64", DamageType = "mind", Resist = "poison|mind", Weakness = "physical", StatusOnHit = "hex", MagicResist = 3 };
                case "koboldwizard": return new EnemyTemplate { Name = "Kobold Bone Wizard", Hp = 18, Power = 9, Defense = 1, Agility = 6, Range = 4, Color = "#8d6dcc", DamageType = "death", Resist = "death|mind", Weakness = "physical|light", StatusOnHit = "hex", MagicResist = 4 };
                case "koboldshield": return new EnemyTemplate { Name = "Kobold Shieldbearer", Hp = 22, Power = 7, Defense = 4, Agility = 4, Range = 1, Color = "#7c6f45", DamageType = "physical", Resist = "physical", Weakness = "fire", MagicResist = 1 };
                case "koboldking": return new EnemyTemplate { Name = "Varkh, Kobold King", Hp = 48, Power = 12, Defense = 5, Agility = 6, Range = 2, Color = "#b88742", DamageType = "physical", Resist = "physical|mind", Weakness = "cold|light", StatusOnHit = "stun", MagicResist = 4, Fearless = true };
                case "ratfolk": return new EnemyTemplate { Name = "Ratfolk Scrapper", Hp = 14, Power = 6, Defense = 1, Agility = 7, Range = 1, Color = "#8f7b64", DamageType = "physical", Weakness = "fire", StatusOnHit = "bleed" };
                case "ratcutthroat": return new EnemyTemplate { Name = "Ratfolk Cutthroat", Hp = 15, Power = 7, Defense = 1, Agility = 9, Range = 1, Color = "#7a6f5f", DamageType = "poison", Weakness = "fire", StatusOnHit = "poison" };
                case "ratmage": return new EnemyTemplate { Name = "Ratfolk Plague Mage", Hp = 16, Power = 7, Defense = 1, Agility = 6, Range = 4, Color = "#8fc27b", DamageType = "poison", Resist = "poison|mind", Weakness = "fire", StatusOnHit = "poison", MagicResist = 3 };
                case "ratcleric": return new EnemyTemplate { Name = "Ratfolk Cistern Cleric", Hp = 17, Power = 5, Defense = 2, Agility = 4, Range = 3, Color = "#d9d3c4", DamageType = "light", Resist = "poison|mind", Weakness = "shock", MagicResist = 3 };
                case "ratbrute": return new EnemyTemplate { Name = "Ratfolk Brute", Hp = 28, Power = 9, Defense = 4, Agility = 3, Range = 1, Color = "#9b6b45", DamageType = "physical", Resist = "physical", Weakness = "fire", StatusOnHit = "stun" };
                case "drowscout": return new EnemyTemplate { Name = "Drow Scout", Hp = 18, Power = 7, Defense = 2, Agility = 9, Range = 1, Color = "#8d6dcc", DamageType = "physical", Resist = "mind", Weakness = "light" };
                case "drowblade": return new EnemyTemplate { Name = "Drow Blade Dancer", Hp = 22, Power = 9, Defense = 2, Agility = 10, Range = 1, Color = "#8d6dcc", DamageType = "physical", Resist = "mind", Weakness = "light", StatusOnHit = "bleed" };
                case "drowcrossbow": return new EnemyTemplate { Name = "Drow Crossbow", Hp = 18, Power = 8, Defense = 2, Agility = 8, Range = 5, Color = "#8d6dcc", DamageType = "physical", Resist = "mind", Weakness = "shock" };
                case "drowmage": return new EnemyTemplate { Name = "Drow Mage", Hp = 17, Power = 9, Defense = 1, Agility = 7, Range = 4, Color = "#8d6dcc", DamageType = "mind", Resist = "mind|death", Weakness = "light|physical", StatusOnHit = "hex", MagicResist = 4 };
                case "drowpriest": return new EnemyTemplate { Name = "Drow Priest", Hp = 20, Power = 7, Defense = 2, Agility = 5, Range = 4, Color = "#d9d3c4", DamageType = "death", Resist = "mind|death", Weakness = "light|shock", StatusOnHit = "hex", MagicResist = 5 };
                case "lesserdemon": return new EnemyTemplate { Name = "Lesser Demon", Hp = 30, Power = 10, Defense = 4, Agility = 5, Range = 1, Color = "#c65c3b", DamageType = "fire", Resist = "fire|death", Weakness = "cold|light", StatusOnHit = "bleed", MagicResist = 3, Fearless = true };
                case "meteorlich": return new EnemyTemplate { Name = "Vhal Rakh, Meteor Crown", Hp = 54, Power = 14, Defense = 4, Agility = 7, Range = 5, Color = "#d98b6a", DamageType = "fire", Resist = "fire|death|mind", Weakness = "cold|light", StatusOnHit = "hex", MagicResist = 7, Fearless = true };
                case "ritualheart": return new EnemyTemplate { Name = "Ritual Heart", Hp = 44, Power = 11, Defense = 6, Agility = 1, Range = 4, Color = "#b94b56", DamageType = "death", Resist = "death|poison|mind", Weakness = "light|shock", StatusOnHit = "bleed", MagicResist = 6, Fearless = true };
                default: return new EnemyTemplate { Name = "Fallen Sentry", Hp = 18, Power = 6, Defense = 2, Agility = 4, Range = 1, Color = "#9b6b45", DamageType = "physical", Weakness = "death" };
            }
        }
    }
}
