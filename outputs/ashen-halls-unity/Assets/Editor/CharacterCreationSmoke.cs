using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class CharacterCreationSmoke
    {
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

        public static void Run()
        {
            try
            {
                RunRulesOrThrow();
                WithFixture((game, state) => AssertRuntimePortraits(game));
                Debug.Log(VersionInfo.ProductName + " character creation smoke passed: 40 painted combinations, safe direct choices, and live customization refresh.");
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogError("Character creation smoke failed: " + exception);
                EditorApplication.Exit(1);
            }
        }

        public static void RunRulesOrThrow()
        {
            ChoiceCatalogCoversEveryCombination();
            RaceDescriptionsMatchGameplay();
            DirectChoicesPreserveTheCompany();
            RepeatedAndInvalidChoicesDoNotResetCustomization();
            CustomizationMutationsRefreshTheScreen();
            DisabledControlsMatchRealBudgets();
        }

        private static void ChoiceCatalogCoversEveryCombination()
        {
            string[] expectedRaces = { "human", "dusk elf", "stoneborn", "fenkin", "ashling" };
            string[] expectedClasses = { "rogue", "warrior", "ranger", "wizard", "mage", "warlock", "priest", "paladin" };
            Require(expectedRaces.SequenceEqual(CharacterCreationCatalog.Races.Select(choice => choice.Key)), "all five playable races are presented in the intended order");
            Require(expectedClasses.SequenceEqual(CharacterCreationCatalog.Classes.Select(choice => choice.Key)), "all eight classes, including independent Wizard and Mage, are presented in atlas order");
            Require(expectedClasses.SequenceEqual(StarterPartyCatalog.SelectableClassKeys), "portrait catalog covers the live selectable classes");
            HashSet<string> mappings = new HashSet<string>();
            foreach (string race in expectedRaces)
            {
                for (int index = 0; index < expectedClasses.Length; index++)
                {
                    string classKey = expectedClasses[index];
                    string file = CharacterCreationCatalog.PortraitAtlasFile(race, classKey);
                    int cell = CharacterCreationCatalog.PortraitCell(race, classKey);
                    Require(!string.IsNullOrWhiteSpace(file) && cell == index, race + " " + classKey + " owns its painted cell");
                    Require(RuntimeArtManifest.ApprovedRuntimeFiles.Contains(file), "portrait file is pinned for packaging: " + file);
                    Require(mappings.Add(file + ":" + cell), race + " " + classKey + " is not an alias of another portrait");
                }
            }
            Require(mappings.Count == 40, "all forty race/class combinations have unique artwork mappings");
            Require(CharacterCreationCatalog.PortraitAtlasFile("unknown", "warrior") == null
                && CharacterCreationCatalog.PortraitAtlasFile("human", "unknown") == null
                && CharacterCreationCatalog.PortraitCell("human", null) == -1, "invalid identities never masquerade as another portrait");
            Texture2D valid = new Texture2D(1774, 887);
            Texture2D invalid = new Texture2D(1536, 1024);
            try
            {
                Require(CharacterCreationCatalog.IsValidAtlas(valid), "original 1774x887 paintings retain fractional square UV cells");
                Require(!CharacterCreationCatalog.IsValidAtlas(invalid), "non-square portrait cells are rejected");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(valid);
                UnityEngine.Object.DestroyImmediate(invalid);
            }
        }

        private static void RaceDescriptionsMatchGameplay()
        {
            int[][] expected = { new[] { 0, 0, 0, 1 }, new[] { 0, 0, 2, -1 }, new[] { 2, 0, -1, 2 }, new[] { 0, 1, 1, 0 }, new[] { 0, 2, 0, -1 } };
            string[] keys = { "str", "int", "agi", "hea" };
            string[] names = { "Strength", "Intelligence", "Agility", "Health" };
            WithFixture((game, state) =>
            {
                for (int raceIndex = 0; raceIndex < CharacterCreationCatalog.Races.Count; raceIndex++)
                {
                    CharacterCreationChoice race = CharacterCreationCatalog.Races[raceIndex];
                    for (int stat = 0; stat < keys.Length; stat++)
                    {
                        int actual = (int)Invoke(game, "RaceStatBonus", race.Key, keys[stat]);
                        Require(actual == expected[raceIndex][stat], race.Name + " retains its existing " + names[stat] + " gameplay bonus");
                        if (actual != 0) Require(race.Description.Contains((actual > 0 ? "+" : "") + actual + " " + names[stat]), race.Name + " explains every gameplay modifier");
                    }
                }
            });
        }

        private static void DirectChoicesPreserveTheCompany()
        {
            WithFixture((game, state) =>
            {
                Invoke(game, "SelectPartySetupMember", 1);
                PartyMember member = state.Party[1];
                Invoke(game, "SetSelectedMemberName", "Aster");
                string id = member.Id;
                Stats attributes = member.Stats;
                string origin = member.Origin;
                string sigil = member.Sigil;
                string color = member.SpriteColor;
                string[] peers = state.Party.Where((hero, index) => index != 1).Select(JsonUtility.ToJson).ToArray();
                foreach (CharacterCreationChoice race in CharacterCreationCatalog.Races)
                {
                    Invoke(game, "SetSelectedMemberRace", race.Key);
                    foreach (CharacterCreationChoice choice in CharacterCreationCatalog.Classes)
                    {
                        Invoke(game, "SetSelectedMemberClass", choice.Key);
                        PartySetupMemberView view = (PartySetupMemberView)Invoke(game, "SelectedPartySetupMemberView");
                        Require(ReferenceEquals(member, state.Party[1]) && member.Id == id && member.Name == "Aster", "direct choice preserves the chosen recruit and name");
                        Require(member.Race == race.Key && member.ClassKey == choice.Key && view.RaceKey == race.Key && view.ClassKey == choice.Key, "direct choices and displayed identity agree");
                        Require(member.Stats.Equals(attributes) && member.Origin == origin && member.Sigil == sigil && member.SpriteColor == color, "class choice preserves assigned attributes and personal identity");
                        Require(member.Spell == StarterPartyCatalog.SpellSchoolForClass(choice.Key), "direct class selection applies its real spell schools");
                        Require(member.MaxHp > 0 && member.Hp == member.MaxHp && member.Mana == member.MaxMana, "every newly selected vocation starts rested");
                        Require(view.RaceDescription == race.Description && view.ClassDescription == choice.Description, "selected detail text reflects the live choices");
                    }
                }
                Require(peers.SequenceEqual(state.Party.Where((hero, index) => index != 1).Select(JsonUtility.ToJson)), "customizing one recruit leaves all three companions byte-for-byte unchanged");
            });
        }

        private static void RepeatedAndInvalidChoicesDoNotResetCustomization()
        {
            WithFixture((game, state) =>
            {
                PartyMember member = state.Party[0];
                member.Skills.Arms = 12;
                member.WeaponName = "custom favorite blade";
                member.WeaponBonus = 2;
                string before = JsonUtility.ToJson(state);
                string refresh = (string)Invoke(game, "PartySetupRefreshKey");
                Invoke(game, "SetSelectedMemberClass", member.ClassKey);
                Invoke(game, "SetSelectedMemberRace", member.Race);
                Invoke(game, "SetSelectedMemberClass", "not-a-class");
                Invoke(game, "SetSelectedMemberRace", "not-a-race");
                Invoke(game, "SetSelectedMemberClass", (object)null);
                Invoke(game, "SetSelectedMemberRace", "");
                Require(JsonUtility.ToJson(state) == before && (string)Invoke(game, "PartySetupRefreshKey") == refresh, "repeated and invalid choices preserve gear, skills, attributes, and company state");
                Invoke(game, "SetSelectedMemberRace", " DUSK ELF ");
                Require(member.Race == "dusk elf" && member.Skills.Arms == 12 && member.WeaponName == "custom favorite blade", "validated race choice preserves class training and gear");
            });
        }

        private static void CustomizationMutationsRefreshTheScreen()
        {
            WithFixture((game, state) =>
            {
                PartyMember member = state.Party[0];
                AssertRefreshChanges(game, () => Invoke(game, "CycleOrigin", member), "origin selection");
                AssertRefreshChanges(game, () => Invoke(game, "CycleSigil", member), "sigil selection");
                foreach (string key in PartySetupScreenLayout.TalentKeys)
                {
                    AssertRefreshChanges(game, () => Invoke(game, "BoostTalent", member, key), key + " talent training");
                }
                AssertRefreshChanges(game, () => member.SkillPoints++, "earned skill points");
                AssertRefreshChanges(game, () => member.Experience++, "experience");
                AssertRefreshChanges(game, () => member.Level++, "level");
                AssertRefreshChanges(game, () => member.GearIntelligence++, "gear bonus");
                string stable = (string)Invoke(game, "PartySetupRefreshKey");
                Invoke(game, "SelectedPartySetupMemberView");
                Require(stable == (string)Invoke(game, "PartySetupRefreshKey"), "reading the character sheet does not keep invalidating it");
            });
        }

        private static void DisabledControlsMatchRealBudgets()
        {
            WithFixture((game, state) =>
            {
                PartyMember member = state.Party[0];
                PartySetupMemberView view = (PartySetupMemberView)Invoke(game, "SelectedPartySetupMemberView");
                Require(!view.CanIncreaseStats && view.CanDecreaseStats.All(value => value), "a fully assigned recruit can refund points but cannot invent more");
                Require((bool)Invoke(game, "CanBeginPartySetup"), "complete company can begin");
                Invoke(game, "ChangeSelectedStat", -1, -1);
                view = (PartySetupMemberView)Invoke(game, "SelectedPartySetupMemberView");
                Require(view.CanIncreaseStats && view.StatTotal == 49, "refunding one point immediately enables assignment");
                Require(!(bool)Invoke(game, "CanBeginPartySetup"), "one recruit with an unspent point blocks begin");
                Invoke(game, "ChangeSelectedStat", -2, 1);
                view = (PartySetupMemberView)Invoke(game, "SelectedPartySetupMemberView");
                Require(!view.CanIncreaseStats && view.StatTotal == 50, "reassigning the point respects the recruit budget");
                Require((bool)Invoke(game, "CanBeginPartySetup"), "assigning the final point enables begin");
                member.Stats.Strength = 3;
                member.Skills.Arms = 12;
                view = (PartySetupMemberView)Invoke(game, "SelectedPartySetupMemberView");
                Require(!view.CanDecreaseStats[0] && !view.CanBoostTalentByIndex[0] && view.CanBoostTalents, "individual attribute floors and trained talent caps disable only the unavailable controls");
                member.Level = 2;
                member.Experience = 20;
                view = (PartySetupMemberView)Invoke(game, "SelectedPartySetupMemberView");
                Require(!view.CanBoostTalents && view.CanBoostTalentByIndex.All(value => !value), "experienced characters cannot receive free recruit training");
            });
        }

        public static void AssertRuntimePortraits(AshenHallsGame game)
        {
            Dictionary<string, Texture2D> loaded = new Dictionary<string, Texture2D>();
            HashSet<string> paintedCells = new HashSet<string>();
            foreach (CharacterCreationChoice race in CharacterCreationCatalog.Races)
            {
                Texture2D first = null;
                foreach (CharacterCreationChoice choice in CharacterCreationCatalog.Classes)
                {
                    Texture2D texture = (Texture2D)Invoke(game, "PartySetupPortraitAtlas", race.Key, choice.Key);
                    int cell = (int)Invoke(game, "PartySetupPortraitCell", race.Key, choice.Key);
                    Require(CharacterCreationCatalog.IsValidAtlas(texture), race.Name + " portrait atlas loads with the painted 4x2 contract");
                    Require(texture.name == CharacterCreationCatalog.PortraitAtlasFile(race.Key, choice.Key), "runtime uses the exact approved portrait painting");
                    Require(cell == CharacterCreationCatalog.PortraitCell(race.Key, choice.Key), "runtime and catalog agree on portrait cell");
                    if (first == null) first = texture;
                    Require(ReferenceEquals(first, texture), "class previews reuse their cached race texture");
                    Require(paintedCells.Add(PortraitFingerprint(texture, cell)), race.Name + " " + choice.Name + " contains distinct painted pixels");
                }
                loaded[race.Key] = first;
            }
            Require(loaded.Values.Distinct().Count() == 5 && paintedCells.Count == 40, "runtime resolves five distinct atlases and forty distinct paintings");
            Require(Invoke(game, "PartySetupPortraitAtlas", "invalid", "warrior") == null, "invalid requests do not load another race's artwork");
        }

        private static string PortraitFingerprint(Texture2D texture, int cell)
        {
            List<string> samples = new List<string>();
            float left = cell % CharacterCreationCatalog.Columns / (float)CharacterCreationCatalog.Columns;
            float bottom = 1f - (cell / CharacterCreationCatalog.Columns + 1f) / CharacterCreationCatalog.Rows;
            for (int y = 1; y < 8; y++)
            {
                for (int x = 1; x < 8; x++)
                {
                    Color32 color = texture.GetPixelBilinear(left + x / 8f / CharacterCreationCatalog.Columns, bottom + y / 8f / CharacterCreationCatalog.Rows);
                    Require(color.a >= 240, "painted portrait interiors remain opaque");
                    samples.Add(color.r + "," + color.g + "," + color.b);
                }
            }
            Require(samples.Distinct().Count() > 8, "portrait cell contains artwork instead of a flat placeholder");
            return string.Join(";", samples);
        }

        private static void AssertRefreshChanges(AshenHallsGame game, Action change, string reason)
        {
            string before = (string)Invoke(game, "PartySetupRefreshKey");
            change();
            Require(before != (string)Invoke(game, "PartySetupRefreshKey"), reason + " immediately invalidates the displayed sheet");
        }

        private static void WithFixture(Action<AshenHallsGame, GameState> check)
        {
            GameObject host = new GameObject("Character creation smoke");
            host.SetActive(false);
            host.hideFlags = HideFlags.HideAndDontSave;
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                Set(game, "rng", new System.Random(228));
                GameState state = new GameState
                {
                    Mode = GameMode.Muster, Depth = 1, SfxMuted = true, MusicMuted = true,
                    Party = (List<PartyMember>)Invoke(game, "MakeDefaultParty")
                };
                Set(game, "state", state);
                Set(game, "selectedBuilderIndex", 0);
                check(game, state);
                Invoke(game, "ReleasePartySetupPortraitArt");
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        private static object Invoke(object source, string name, params object[] values) => source.GetType().GetMethod(name, Private).Invoke(source, values);
        private static void Set(object source, string name, object value) => source.GetType().GetField(name, Private).SetValue(source, value);
        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
