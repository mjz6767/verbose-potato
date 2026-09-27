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
            new CharacterCreationChoice("human", "Human", "+1 Health", 0, 0, 0, 1),
            new CharacterCreationChoice("dusk elf", "Dusk Elf", "+2 Agility, -1 Health", 0, 0, 2, -1),
            new CharacterCreationChoice("stoneborn", "Stoneborn", "+2 Strength, +2 Health, -1 Agility", 2, 0, -1, 2),
            new CharacterCreationChoice("fenkin", "Fenkin", "+1 Intelligence, +1 Agility", 0, 1, 1, 0),
            new CharacterCreationChoice("ashling", "Ashling", "+2 Intelligence, -1 Health", 0, 2, 0, -1)
        });

        // Order is the top-left to bottom-right reading order in every race atlas.
        public static readonly IReadOnlyList<CharacterCreationChoice> Classes = Array.AsReadOnly(new[]
        {
            new CharacterCreationChoice("rogue", "Rogue", "Fast melee. Favors Agility and Strength."),
            new CharacterCreationChoice("warrior", "Warrior", "Front line. Favors Strength and Health."),
            new CharacterCreationChoice("ranger", "Ranger", "Long-range bows. Favors Agility."),
            new CharacterCreationChoice("wizard", "Wizard", "Ember and Hex spells. Favors Intelligence."),
            new CharacterCreationChoice("mage", "Mage", "Stronger Ember magic. Favors Intelligence."),
            new CharacterCreationChoice("warlock", "Warlock", "Hex and Pact magic. Favors Intelligence."),
            new CharacterCreationChoice("priest", "Priest", "Mend healing. Favors Intelligence and Health."),
            new CharacterCreationChoice("paladin", "Paladin", "Melee, Guard and Mend. Favors Strength and Health.")
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
