using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AshenHalls.Editor
{
    // Isolated presentation fixtures: no campaign or input state is written.
    public static class UiFinishCapture
    {
        private const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;

        public static void Capture()
        {
            try
            {
                PresentationAccessibilitySmoke.RunOrThrow();
                WorldMapHudAuditSmoke.RunOrThrow();
                UiFinishSmoke.RunOrThrow();
                string directory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "QA", "ui-finish", DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff")));
                Directory.CreateDirectory(directory);
                foreach (Vector2Int size in new[] { new Vector2Int(960, 600), new Vector2Int(1280, 720), new Vector2Int(1920, 1080) })
                {
                    foreach (bool expanded in new[] { false, true })
                    {
                        CapturePause(directory, size, expanded);
                        CaptureExploration(directory, size, expanded);
                    }
                    CaptureLoot(directory, size, true);
                    CaptureLoot(directory, size, false);
                }
                Debug.Log("UI FINISH CAPTURES PASSED: " + directory);
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError("UI FINISH CAPTURES FAILED: " + ex);
                EditorApplication.Exit(1);
            }
        }

        private static void CapturePause(string directory, Vector2Int size, bool settings)
        {
            GameObject host = new GameObject("UI finish pause fixture");
            try
            {
                PauseMenuScreen screen = host.AddComponent<PauseMenuScreen>();
                screen.Bind(new PauseMenuScreenBindings { View = () => new PauseMenuView {
                    Title = "Menu", RouteLine = "Chapter I • Midgaard • Town Hall",
                    SaveLine = "Campaign checkpoint ready", AudioLine = "Audio: On",
                    SfxLine = "SFX: 75%", MusicLine = "Music: 50%",
                    MotionLine = "Reduced Motion: Off", SettingsOpen = settings
                }});
                screen.SetVisible(true);
                screen.Refresh();
                Render(directory, settings ? "pause-settings" : "pause", size, Field<Canvas>(screen, "canvas"), () =>
                    Layout(screen, new[] { typeof(bool), typeof(float), typeof(float) }, settings, (float)size.x, (float)size.y));
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        private static void CaptureExploration(string directory, Vector2Int size, bool details)
        {
            GameObject host = new GameObject("UI finish exploration fixture");
            try
            {
                ExplorationHudScreen screen = host.AddComponent<ExplorationHudScreen>();
                screen.Bind(new ExplorationHudScreenBindings { View = () => new ExplorationHudView {
                    Title = "Local Map", RouteLine = "Chapter I • Midgaard", FocusHint = "Town Hall / Grand Hearth",
                    Gold = "128", Supplies = "6", Elixirs = "2", DetailsOpen = details,
                    ViewLabel = "Local Map", ZoneName = "Town Hall / Grand Hearth", ZoneDetail = "A warm shelter on the Old Road.",
                    DangerLabel = "SAFE", DangerColorHex = "58b7a5", LookLine = "Kate tends the company register.",
                    ObjectiveSummary = "Speak with Kate", ObjectiveLine = "Speak with Kate at the Grand Hearth to begin your journey.",
                    WaypointLine = "Kate • 3 steps east", NearbyLine = "Kate • Company stores • Storm doors", GrowthLine = "Party L1 / Next level: 60 XP",
                    HasAction = true, ActionLabel = "Talk", ActionTarget = "Kate",
                    Party = new[] {
                        new ExplorationHudPartyMemberView { Name = "Cairn", ClassLine = "Human Warrior • L1", ColorHex = "c18c61", Hp = 32, MaxHp = 32, Mana = 6, MaxMana = 6 },
                        new ExplorationHudPartyMemberView { Name = "Seren", ClassLine = "Dusk-elf Ranger • L1", ColorHex = "82a081", Hp = 10, MaxHp = 24, Mana = 8, MaxMana = 8 },
                        new ExplorationHudPartyMemberView { Name = "Orin", ClassLine = "Stoneborn Cleric • L1", ColorHex = "ada8cf", Hp = 4, MaxHp = 28, Mana = 3, MaxMana = 14 },
                        new ExplorationHudPartyMemberView { Name = "Mira", ClassLine = "Ashling Mage • L1", ColorHex = "cc8263", Hp = 0, MaxHp = 18, Mana = 6, MaxMana = 18 }
                    },
                    Logs = new[] {
                        new ExplorationHudLogView { Text = "Kate welcomes the company to the Grand Hearth.", Tone = "good" },
                        new ExplorationHudLogView { Text = "The company recovered 12 gold and a supply.", Tone = "loot" },
                        new ExplorationHudLogView { Text = "A new route is marked on the map.", Tone = "info" }
                    }
                }});
                screen.SetVisible(true);
                screen.Refresh();
                Render(directory, details ? "exploration-details" : "exploration", size, Field<Canvas>(screen, "canvas"), () =>
                    typeof(ExplorationHudScreen).GetMethod("Refresh", PrivateInstance, null, new[] { typeof(float), typeof(float) }, null)
                        .Invoke(screen, new object[] { (float)size.x, (float)size.y }));
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        private static void CaptureLoot(string directory, Vector2Int size, bool item)
        {
            GameObject host = new GameObject("UI finish loot fixture");
            Texture2D icon = null;
            try
            {
                if (item)
                {
                    string path = Path.Combine(Application.dataPath, "..", "Docs", "ArtReferences", RuntimeArtManifest.UniqueItemAtlas);
                    icon = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    if (!icon.LoadImage(File.ReadAllBytes(path))) throw new InvalidOperationException("Loot fixture icon could not be decoded.");
                    icon.filterMode = FilterMode.Bilinear;
                }
                LootPopupScreen screen = host.AddComponent<LootPopupScreen>();
                screen.Bind(new LootPopupBindings { View = () => new LootPopupView {
                    Visible = true, HasItem = item, CanReview = item, CanQuickEquip = item,
                    Title = "Loot recovered", ItemName = item ? "Roadwarden's Longsword" : "Victory spoils",
                    ItemType = "Sword", Rarity = "Rare", TraitLine = item ? "A balanced blade with a weathered brass guard." : "Gold and supplies were added to the company stores.",
                    Comparison = item ? "Cairn: +2 attack • current weapon: Iron Sword" : "",
                    Outcome = item ? "Stored in inventory" : "Added to company stores",
                    EquipNote = item ? "Choose Equip best fit to arm Cairn, or compare equipment in the inventory." : "Ready for the next leg of the journey.",
                    Gold = 12, Supplies = 1, Elixirs = 1, AccentHex = "d7a84e",
                    IconTexture = icon, IconUv = new Rect(0f, 0.75f, 0.2f, 0.25f), IconLabel = "SPOILS"
                }});
                screen.SetVisible(true);
                screen.Refresh();
                Render(directory, item ? "loot-equipment" : "loot-supplies", size, Field<Canvas>(screen, "canvas"), () =>
                    Layout(screen, new[] { typeof(float), typeof(float) }, (float)size.x, (float)size.y));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(host);
                if (icon != null) UnityEngine.Object.DestroyImmediate(icon);
            }
        }

        private static void Render(string directory, string name, Vector2Int size, Canvas canvas, Action layout)
        {
            GameObject cameraObject = new GameObject("Offscreen UI finish camera");
            RenderTexture target = new RenderTexture(size.x, size.y, 24, RenderTextureFormat.ARGB32);
            RenderTexture previous = RenderTexture.active;
            Texture2D pixels = null;
            try
            {
                Camera camera = cameraObject.AddComponent<Camera>();
                camera.enabled = false;
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0.06f, 0.08f, 0.10f, 1f);
                camera.orthographic = true;
                camera.orthographicSize = size.y * 0.5f;
                camera.nearClipPlane = 0.01f;
                camera.farClipPlane = 10f;
                camera.cullingMask = 1 << 31;
                camera.targetTexture = target;
                target.Create();
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camera;
                canvas.planeDistance = 1f;
                foreach (Transform child in canvas.GetComponentsInChildren<Transform>(true)) child.gameObject.layer = 31;
                Canvas.ForceUpdateCanvases();
                layout();
                Canvas.ForceUpdateCanvases();
                camera.Render();
                Canvas.ForceUpdateCanvases();
                camera.Render();
                RenderTexture.active = target;
                pixels = new Texture2D(size.x, size.y, TextureFormat.RGB24, false);
                pixels.ReadPixels(new Rect(0f, 0f, size.x, size.y), 0, 0);
                pixels.Apply();
                int readablePixels = 0;
                foreach (Color32 pixel in pixels.GetPixels32())
                    if (pixel.r > 100 || pixel.g > 100 || pixel.b > 100) readablePixels++;
                if (readablePixels < 500) throw new InvalidOperationException("Blank or unreadable UI capture: " + name);
                string file = Path.Combine(directory, name + "-" + size.x + "x" + size.y + ".png");
                File.WriteAllBytes(file, pixels.EncodeToPNG());
                Debug.Log("UI finish capture: " + file);
            }
            finally
            {
                RenderTexture.active = previous;
                canvas.worldCamera = null;
                if (pixels != null) UnityEngine.Object.DestroyImmediate(pixels);
                UnityEngine.Object.DestroyImmediate(cameraObject);
                target.Release();
                UnityEngine.Object.DestroyImmediate(target);
            }
        }

        private static void Layout(object screen, Type[] signature, params object[] arguments)
        {
            MethodInfo method = screen.GetType().GetMethod("ApplyLayout", PrivateInstance, null, signature, null);
            if (method == null) throw new InvalidOperationException("Capture layout overload missing: " + screen.GetType().Name);
            method.Invoke(screen, arguments);
        }

        private static T Field<T>(object screen, string name)
        {
            return (T)screen.GetType().GetField(name, PrivateInstance).GetValue(screen);
        }
    }
}
