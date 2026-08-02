using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AshenHalls.Editor
{
    public static class SpriteArtRuntimeSmoke
    {
        private const string MainScenePath = "Assets/Scenes/Main.unity";

        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " sprite-art runtime smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " sprite-art runtime smoke failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            try
            {
                Scene scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
                Assert(scene.IsValid() && scene.isLoaded, "Main scene loads");

                AshenHallsGame game = UnityEngine.Object.FindFirstObjectByType<AshenHallsGame>();
                Assert(game != null, "AshenHallsGame exists in Main scene");
                InvokePrivate(game, "Awake");
                AssertV24WorldMapAtlasesAndMappings(game);

                Texture2D characterAtlas = GetPrivateField<Texture2D>(game, "characterCombatAtlas");
                Assert(characterAtlas != null, "player character atlas loads");
                Assert(
                    characterAtlas.name.IndexOf("v1.93.0", StringComparison.OrdinalIgnoreCase) >= 0,
                    "runtime selects the approved v1.93 player atlas");
                Assert(
                    characterAtlas.width == PlayerSpriteCatalog.Columns * 256
                        && characterAtlas.height == PlayerSpriteCatalog.Rows * 256,
                    "runtime player atlas uses the exact 5x7 square-cell contract");
                Assert(
                    InvokePrivate<bool>(game, "IsCharacterCombatAtlas"),
                    "presentation accepts the expanded player atlas");

                string[] classes =
                {
                    "warrior", "rogue", "ranger", "priest", "warlock", "wizard", "paladin"
                };
                string[] roles =
                {
                    "shield", "knife", "bow", "mender", "hex", "ember", "ward"
                };
                string[] races =
                {
                    "human", "dusk elf", "stoneborn", "fenkin", "ashling"
                };
                for (int classRow = 0; classRow < classes.Length; classRow++)
                {
                    for (int raceColumn = 0; raceColumn < races.Length; raceColumn++)
                    {
                        int expected = classRow * PlayerSpriteCatalog.Columns + raceColumn;
                        int actual = InvokePrivate<int>(
                            game,
                            "CharacterCombatAtlasIndex",
                            classes[classRow],
                            races[raceColumn],
                            roles[classRow]);
                        Assert(
                            actual == expected,
                            races[raceColumn] + " " + classes[classRow] + " resolves to cell " + expected);
                    }
                }

                Texture2D npcAtlas = GetPrivateField<Texture2D>(game, "midgaardNpcAtlas");
                Assert(npcAtlas != null, "Midgaard NPC atlas loads");
                Assert(
                    npcAtlas.name.IndexOf("v1.93.0", StringComparison.OrdinalIgnoreCase) >= 0,
                    "runtime selects the approved v1.93 NPC atlas");
                Assert(
                    npcAtlas.width == NpcPortraitCatalog.Columns * 256
                        && npcAtlas.height == NpcPortraitCatalog.Rows * 256,
                    "runtime NPC atlas uses the exact 5x4 square-cell contract");
                Assert(InvokePrivate<bool>(game, "IsMidgaardNpcAtlas"), "presentation accepts only the exact 5x4 NPC contract");
                Dictionary<ObjectType, int> placedNpcCells = new Dictionary<ObjectType, int>
                {
                    { ObjectType.DinerCook, 10 },
                    { ObjectType.Provisioner, 11 },
                    { ObjectType.DockWorker, 14 },
                    { ObjectType.Scholar, 19 }
                };
                foreach (KeyValuePair<ObjectType, int> contact in placedNpcCells)
                {
                    Assert(
                        NpcPortraitCatalog.WorldSpriteIndex(contact.Key, false) == contact.Value,
                        contact.Key + " owns NPC cell " + contact.Value);
                    Assert(
                        InvokePrivate<int>(game, "MidgaardNpcObjectIconIndex", contact.Key, null) == contact.Value,
                        contact.Key + " reaches NPC cell " + contact.Value + " through the live draw adapter");
                }
            }
            finally
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            }
        }

        private static void AssertV24WorldMapAtlasesAndMappings(AshenHallsGame game)
        {
            Texture2D habitatAtlas = GetPrivateField<Texture2D>(game, "worldThreatHabitatAtlas");
            Texture2D citizenAtlas = GetPrivateField<Texture2D>(game, "worldNpcCitizenAtlas");
            Texture2D playerRoleAtlas = GetPrivateField<Texture2D>(game, "playerExplorationRoleAtlas");

            Assert(habitatAtlas != null, "v2.4 world-threat habitat atlas loads");
            Assert(habitatAtlas.name == RuntimeArtManifest.WorldThreatHabitatAtlas, "runtime selects the exact approved v2.4 habitat atlas");
            Assert(habitatAtlas.width == 1536 && habitatAtlas.height == 768, "v2.4 habitat atlas uses the exact 4x2 square-cell contract");
            Assert(InvokePrivate<bool>(game, "IsWorldThreatHabitatAtlas"), "presentation accepts the exact v2.4 habitat atlas");

            Assert(citizenAtlas != null, "v2.4 ambient-citizen atlas loads");
            Assert(citizenAtlas.name == RuntimeArtManifest.WorldNpcCitizenAtlas, "runtime selects the exact approved v2.4 citizen atlas");
            Assert(citizenAtlas.width == 1536 && citizenAtlas.height == 768, "v2.4 citizen atlas uses the exact 4x2 square-cell contract");
            Assert(InvokePrivate<bool>(game, "IsWorldNpcCitizenAtlas"), "presentation accepts the exact v2.4 citizen atlas");

            Assert(playerRoleAtlas != null, "v2.4 player exploration-role atlas loads");
            Assert(playerRoleAtlas.name == RuntimeArtManifest.PlayerExplorationRoleAtlas, "runtime selects the exact approved v2.4 player-role atlas");
            Assert(playerRoleAtlas.width == 1536 && playerRoleAtlas.height == 768, "v2.4 player-role atlas uses the exact 4x2 square-cell contract");
            Assert(InvokePrivate<bool>(game, "IsPlayerExplorationRoleAtlas"), "presentation accepts the exact v2.4 player-role atlas");

            Dictionary<string, int> habitatCells = new Dictionary<string, int>(StringComparer.Ordinal)
            {
                { "rat", 0 },
                { "ratmage", 1 },
                { "kobold", 2 },
                { "koboldshaman", 3 },
                { "drow", 4 },
                { "undead", 5 },
                { "demon", 6 },
                { "waystation", 7 }
            };
            foreach (KeyValuePair<string, int> habitat in habitatCells)
            {
                Assert(
                    WorldThreatHabitatPresentationRules.ArchetypeIndex(habitat.Key) == habitat.Value,
                    habitat.Key + " uses v2.4 habitat cell " + habitat.Value);
            }
            Assert(
                WorldThreatHabitatPresentationRules.AtlasIndex("unknown", RoamingThreatFaction.Kobolds)
                    == WorldThreatHabitatPresentationRules.KoboldAmbushCampIndex,
                "unknown kobold archetypes retain the faction habitat fallback");
            Assert(
                WorldThreatHabitatPresentationRules.AtlasIndex((RoamingThreatDefinition)null)
                    == WorldThreatHabitatPresentationRules.RuinedRoadWaystationIndex,
                "missing threats retain the neutral waystation habitat fallback");
            Assert(WorldThreatHabitatPresentationRules.DrawsBeneathRoamingThreatToken, "habitats remain beneath mobile threat tokens");
            Assert(
                !WorldThreatHabitatPresentationRules.ShouldDrawAtHome(true, true),
                "habitat art never occupies a certified safe road");

            string[] roleTokenOrder =
            {
                "shield", "pike", "bow", "knife", "mender", "ember", "hex", "ward"
            };
            for (int index = 0; index < roleTokenOrder.Length; index++)
            {
                string role = roleTokenOrder[index];
                Assert(ExplorationCharacterArtCatalog.PlayerRoleIndex(role) == index, role + " uses player exploration cell " + index);
                Assert(ExplorationCharacterArtCatalog.PlayerTokenIndex(1, role) == index, "single " + role + " uses the dedicated player atlas");
            }
            Assert(
                ExplorationCharacterArtCatalog.PlayerTokenIndex(4, "shield")
                    == ExplorationCharacterArtCatalog.PartyGroupBypassIndex,
                "multi-character parties bypass the single-role atlas");
            Assert(ExplorationCharacterArtCatalog.UsesPartyGroupToken(4), "multi-character parties preserve their group token");
            Assert(ExplorationArtRules.PartyTokenRole(4, "shield") == "party", "live party-token selection preserves the group fallback role");
            Assert(InvokePrivate<int>(game, "WorldMapTokenSpriteIndex", "party") == 0, "the legacy mixed atlas retains its party-group fallback cell");
            Assert(ExplorationCharacterArtCatalog.PlayerTokenIndex(1, "unknown") < 0, "unknown solo roles fall through to the mixed token atlas");

            AmbientCitizenProfession[] citizenOrder =
            {
                AmbientCitizenProfession.Lamplighter,
                AmbientCitizenProfession.Fishmonger,
                AmbientCitizenProfession.Tailor,
                AmbientCitizenProfession.Mason,
                AmbientCitizenProfession.Apothecary,
                AmbientCitizenProfession.RoadPilgrim,
                AmbientCitizenProfession.Gravedigger,
                AmbientCitizenProfession.CaravanGuide
            };
            for (int index = 0; index < citizenOrder.Length; index++)
            {
                Assert(
                    ExplorationCharacterArtCatalog.CitizenAtlasIndex(citizenOrder[index]) == index,
                    citizenOrder[index] + " uses ambient-citizen cell " + index);
                Assert(
                    ExplorationCharacterArtCatalog.CitizenProfessionAt(index) == citizenOrder[index],
                    "ambient-citizen cell " + index + " round-trips its profession");
            }
            int firstCitizen = ExplorationCharacterArtCatalog.AmbientCitizenIndex("old-quarry", 24680, 17, 9);
            int repeatedCitizen = ExplorationCharacterArtCatalog.AmbientCitizenIndex("old-quarry", 24680, 17, 9);
            Assert(firstCitizen == repeatedCitizen && firstCitizen >= 0, "ambient citizen selection is stable for a world coordinate");
            Assert(
                !ExplorationCharacterArtCatalog.ShouldPlaceAmbientCitizen(
                    "midgaard-grand-hearth",
                    24680,
                    17,
                    9,
                    true,
                    false,
                    false,
                    false,
                    false),
                "ambient citizens stay off the Grand Hearth tutorial lane");
            Assert(
                !ExplorationCharacterArtCatalog.CanPlaceAmbientCitizen(false, false, false, false, true),
                "ambient citizens never occupy an interactable object's cell");

            MapData townHallMap = new MapData
            {
                Width = WorldMapGenerationRules.Width,
                Height = WorldMapGenerationRules.Height,
                Depth = 1
            };
            RectInt townHall = MidgaardInteriorRules.GrandHearthBounds(townHallMap);
            HashSet<string> patronCells = new HashSet<string>(StringComparer.Ordinal);
            Assert(MidgaardInteriorRules.GrandHearthPatrons.Count == 6,
                "Town Hall authors six presentation-only patrons");
            foreach (GrandHearthPatronPlacement placement in MidgaardInteriorRules.GrandHearthPatrons)
            {
                int x = townHall.xMin + placement.OffsetX;
                int y = townHall.yMin + placement.OffsetY;
                Assert(MidgaardInteriorRules.TryGrandHearthPatron(
                        townHallMap,
                        x,
                        y,
                        out AmbientCitizenProfession profession)
                    && profession == placement.Profession,
                    placement.Profession + " resolves at its authored Town Hall cell");
                Assert(ExplorationCharacterArtCatalog.CitizenAtlasIndex(profession) >= 0,
                    placement.Profession + " uses approved v2.4 citizen art in Town Hall");
                Assert(patronCells.Add(x + "," + y),
                    "Town Hall patron cells are unique");
                Assert(!MidgaardInteriorRules.IsGrandHearthCompanyRunner(townHallMap, x, y),
                    placement.Profession + " stays off the first-step company runner");
            }
        }

        private static T GetPrivateField<T>(object target, string fieldName)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
            {
                throw new MissingFieldException(target.GetType().FullName, fieldName);
            }
            return (T)field.GetValue(target);
        }

        private static void InvokePrivate(object target, string methodName, params object[] args)
        {
            InvokePrivate<object>(target, methodName, args);
        }

        private static T InvokePrivate<T>(object target, string methodName, params object[] args)
        {
            MethodInfo method = target.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (method == null)
            {
                throw new MissingMethodException(target.GetType().FullName, methodName);
            }
            object result = method.Invoke(target, args);
            return result == null ? default : (T)result;
        }

        private static void Assert(bool condition, string label)
        {
            if (!condition)
            {
                throw new InvalidOperationException(label);
            }
        }
    }
}
