using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class PartySetupWorkshopCapture
    {
        public static void Capture()
        {
            try
            {
                PartySetupWorkshopSmoke.RunOrThrow();
                AshenHallsGame game = PartySetupWorkshopSmoke.OpenGame();
                string directory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "QA", "party-setup",
                    "editor-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff")));
                Directory.CreateDirectory(directory);
                int count = 0;
                HashSet<string> capturedNames = new HashSet<string>(StringComparer.Ordinal);
                string[] races = { "human", "dusk elf", "stoneborn", "fenkin", "ashling", "human" };
                string[] classes = { "warrior", "ranger", "paladin", "rogue", "warlock", "mage" };
                foreach (Vector2Int size in new[] { new Vector2Int(960, 600), new Vector2Int(1280, 720), new Vector2Int(1920, 1080) })
                    for (int i = 0; i < races.Length; i++)
                    {
                        if (CaptureOne(game, directory, races[i], classes[i], i % 4, i == 5, size, capturedNames)) count++;
                    }
                if (Environment.GetCommandLineArgs().Contains("-ashen-capture-all-portraits"))
                    foreach (CharacterCreationChoice race in CharacterCreationCatalog.Races)
                        foreach (CharacterCreationChoice vocation in CharacterCreationCatalog.Classes)
                        {
                            if (CaptureOne(game, directory, race.Key, vocation.Key, 0, false, new Vector2Int(1280, 720), capturedNames)) count++;
                        }
                Debug.Log(VersionInfo.ProductName + " party setup editor captures passed: " + count + " / " + directory);
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " party setup editor captures failed: " + ex);
                EditorApplication.Exit(1);
            }
            finally
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            }
        }

        private static bool CaptureOne(AshenHallsGame game, string directory, string race, string classKey, int member, bool details, Vector2Int size, HashSet<string> capturedNames)
        {
            string name = race.Replace(' ', '-') + "-" + classKey + (details ? "-details" : "") + "-" + size.x + "x" + size.y;
            if (!capturedNames.Add(name)) return false;
            PartySetupWorkshopSmoke.Stage(game, race, classKey, member, details);
            PartySetupScreen screen = PartySetupWorkshopSmoke.Field<PartySetupScreen>(game, "partySetupScreen");
            string path = Path.Combine(directory, name + ".png");
            PartySetupCaptureRenderer.Write(screen, size.x, size.y, path);
            Texture2D pixels = new Texture2D(2, 2, TextureFormat.RGB24, false);
            try
            {
                PartySetupWorkshopSmoke.Require(pixels.LoadImage(File.ReadAllBytes(path)), "captured PNG decodes");
                Color32[] colors = pixels.GetPixels32();
                List<CapturePixelSample> samples = new List<CapturePixelSample>();
                for (int y = 0; y < 24; y++)
                    for (int x = 0; x < 32; x++)
                    {
                        Color32 color = colors[Math.Min(pixels.height - 1, (y * pixels.height + pixels.height / 2) / 24) * pixels.width
                            + Math.Min(pixels.width - 1, (x * pixels.width + pixels.width / 2) / 32)];
                        samples.Add(new CapturePixelSample(color.r, color.g, color.b));
                    }
                CaptureAcceptanceResult result = CaptureAcceptanceRules.Evaluate(size.x, size.y, size.x, size.y, pixels.width, pixels.height, samples);
                PartySetupWorkshopSmoke.Require(result.Accepted, "offscreen pixels accepted for " + name + ": " + result.Failure);
                Debug.Log("Party Setup editor capture " + path + "; complete=True, failure=None, renderer=offscreen-editor-canvas.");
            }
            finally { UnityEngine.Object.DestroyImmediate(pixels); }
            return true;
        }
    }
}
