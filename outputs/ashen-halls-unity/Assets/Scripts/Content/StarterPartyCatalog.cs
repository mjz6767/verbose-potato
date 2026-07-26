using System.Collections.Generic;

namespace AshenHalls
{
    public static class StarterPartyCatalog
    {
        public const int ExpectedPartySize = 4;

        private static readonly string[] selectableClassKeys =
        {
            "rogue", "warrior", "ranger", "wizard", "mage", "warlock", "priest", "paladin"
        };

        private static readonly StarterHeroDef[] heroes =
        {
            new StarterHeroDef("Maer", "human", "warrior", "shield", new Stats(18, 7, 10, 15), "", 1, new SkillSet { Arms = 9, Guard = 7 }),
            new StarterHeroDef("Cairn", "human", "ranger", "bow", new Stats(10, 9, 19, 12), "", 5, new SkillSet { Arms = 3, Missile = 10, Guard = 2 }),
            new StarterHeroDef("Luma", "ashling", "mage", "ember", new Stats(7, 22, 12, 9), "ember", 3, new SkillSet { Ember = 9, Guard = 1 }),
            new StarterHeroDef("Vesh", "fenkin", "priest", "mender", new Stats(7, 20, 9, 14), "mend", 1, new SkillSet { Mend = 9, Guard = 3 })
        };

        public static IReadOnlyList<StarterHeroDef> All => heroes;
        public static IReadOnlyList<string> SelectableClassKeys => selectableClassKeys;

        public static string SpellSchoolForClass(string classKey)
        {
            switch ((classKey ?? "").ToLowerInvariant())
            {
                case "wizard": return "ember|hex";
                case "mage": return "ember";
                case "warlock": return "hex|pact";
                case "priest": return "mend";
                case "paladin": return "mend";
                default: return "";
            }
        }
    }

    public readonly struct StarterHeroDef
    {
        public readonly string Name;
        public readonly string Race;
        public readonly string ClassKey;
        public readonly string Role;
        public readonly Stats Stats;
        public readonly string Spell;
        public readonly int Range;
        private readonly int arms;
        private readonly int missile;
        private readonly int mend;
        private readonly int ember;
        private readonly int hex;
        private readonly int guard;

        public StarterHeroDef(string name, string race, string classKey, string role, Stats stats, string spell, int range, SkillSet skills)
        {
            Name = name;
            Race = race;
            ClassKey = classKey;
            Role = role;
            Stats = stats;
            Spell = spell;
            Range = range;
            arms = skills?.Arms ?? 0;
            missile = skills?.Missile ?? 0;
            mend = skills?.Mend ?? 0;
            ember = skills?.Ember ?? 0;
            hex = skills?.Hex ?? 0;
            guard = skills?.Guard ?? 0;
        }

        public SkillSet CreateSkills()
        {
            return new SkillSet
            {
                Arms = arms,
                Missile = missile,
                Mend = mend,
                Ember = ember,
                Hex = hex,
                Guard = guard
            }.Normalize();
        }
    }
}
