using System;
using System.Collections.Generic;
using UnityEngine;

namespace AshenHalls
{
    /// <summary>Authored character choices and the independent painted-portrait contract.</summary>
    public static class CharacterCreationCatalog
    {
        public const int Columns = 4;
        public const int Rows = 2;

        public static readonly IReadOnlyList<CharacterCreationChoice> Races = Array.AsReadOnly(new[]
        {
            new CharacterCreationChoice("human", "Human", "Resourceful travelers of the hearthlands. +1 Health.", 0, 0, 0, 1),
            new CharacterCreationChoice("dusk elf", "Dusk Elf", "Watchful wanderers of the twilight woods. +2 Agility, -1 Health.", 0, 0, 2, -1),
            new CharacterCreationChoice("stoneborn", "Stoneborn", "Stalwart folk of the mountain halls. +2 Strength, +2 Health, -1 Agility.", 2, 0, -1, 2),
            new CharacterCreationChoice("fenkin", "Fenkin", "Quick-witted folk of the misty wetlands. +1 Intelligence, +1 Agility.", 0, 1, 1, 0),
            new CharacterCreationChoice("ashling", "Ashling", "Ember-touched seekers from the ashlands. +2 Intelligence, -1 Health.", 0, 2, 0, -1)
        });

        // Order is the top-left to bottom-right reading order in every race atlas.
        public static readonly IReadOnlyList<CharacterCreationChoice> Classes = Array.AsReadOnly(new[]
        {
            new CharacterCreationChoice("rogue", "Rogue", "A swift, lightly armored duelist. Favors Agility and Strength; opens with an epee and Arms training."),
            new CharacterCreationChoice("warrior", "Warrior", "A sturdy front-line defender. Favors Strength and Health; opens with a broadsword, armor, and Guard training."),
            new CharacterCreationChoice("ranger", "Ranger", "A long-range archer who pressures distant foes. Favors Agility; opens with a longbow and Missile training."),
            new CharacterCreationChoice("wizard", "Wizard", "A versatile arcanist who studies Ember and Hex magic. Favors Intelligence; opens with an ember focus."),
            new CharacterCreationChoice("mage", "Mage", "An elemental specialist devoted to Ember magic. Favors Intelligence; opens with an ember focus and stronger Ember training."),
            new CharacterCreationChoice("warlock", "Warlock", "A dark caster who studies Hex and Pact magic. Favors Intelligence; opens with a bone focus and Hex training."),
            new CharacterCreationChoice("priest", "Priest", "A healer who sustains the company with Mend magic. Favors Intelligence and Health; opens with a prayer focus."),
            new CharacterCreationChoice("paladin", "Paladin", "An armored oathkeeper who blends melee, Guard, and Mend magic. Favors Strength and Health; opens with a mace and ward shield.")
        });

        public static bool TryGetRace(string key, out CharacterCreationChoice choice) => TryFind(Races, key, out choice);
        public static bool TryGetClass(string key, out CharacterCreationChoice choice) => TryFind(Classes, key, out choice);

        private static bool TryFind(IReadOnlyList<CharacterCreationChoice> choices, string key, out CharacterCreationChoice choice)
        {
            string normalized = (key ?? "").Trim();
            for (int i = 0; i < choices.Count; i++)
            {
                if (!string.Equals(choices[i].Key, normalized, StringComparison.OrdinalIgnoreCase)) continue;
                choice = choices[i];
                return true;
            }
            choice = default;
            return false;
        }

        public static int RaceStatBonus(string race, string stat)
        {
            if (!TryGetRace(race ?? "human", out CharacterCreationChoice choice)) return 0;
            switch (stat)
            {
                case "str": return choice.StrengthBonus;
                case "int": return choice.IntelligenceBonus;
                case "agi": return choice.AgilityBonus;
                case "hea": return choice.HealthBonus;
                default: return 0;
            }
        }

        public static string PortraitAtlasFile(string race, string classKey)
        {
            if (!TryGetRace(race, out CharacterCreationChoice choice) || !TryGetClass(classKey, out _)) return null;
            switch (choice.Key)
            {
                case "human": return RuntimeArtManifest.CharacterPortraitHumanAtlas;
                case "dusk elf": return RuntimeArtManifest.CharacterPortraitDuskElfAtlas;
                case "stoneborn": return RuntimeArtManifest.CharacterPortraitStonebornAtlas;
                case "fenkin": return RuntimeArtManifest.CharacterPortraitFenkinAtlas;
                case "ashling": return RuntimeArtManifest.CharacterPortraitAshlingAtlas;
                default: return null;
            }
        }

        public static int PortraitCell(string race, string classKey)
        {
            if (!TryGetRace(race, out _) || !TryGetClass(classKey, out CharacterCreationChoice choice)) return -1;
            for (int i = 0; i < Classes.Count; i++) if (Classes[i].Key == choice.Key) return i;
            return -1;
        }

        public static bool IsValidAtlas(Texture2D texture)
        {
            // RawImage uses fractional UVs, preserving original paintings whose cell edges fall between pixels.
            return texture != null && texture.width >= Columns * 256 && texture.height >= Rows * 256
                && texture.width == texture.height * 2;
        }
    }

    public readonly struct CharacterCreationChoice
    {
        public readonly string Key;
        public readonly string Name;
        public readonly string Description;
        public readonly int StrengthBonus;
        public readonly int IntelligenceBonus;
        public readonly int AgilityBonus;
        public readonly int HealthBonus;

        public CharacterCreationChoice(string key, string name, string description,
            int strengthBonus = 0, int intelligenceBonus = 0, int agilityBonus = 0, int healthBonus = 0)
        {
            Key = key;
            Name = name;
            Description = description;
            StrengthBonus = strengthBonus;
            IntelligenceBonus = intelligenceBonus;
            AgilityBonus = agilityBonus;
            HealthBonus = healthBonus;
        }
    }
}
