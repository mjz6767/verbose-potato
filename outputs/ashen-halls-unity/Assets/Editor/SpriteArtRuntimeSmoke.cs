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
