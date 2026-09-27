using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AshenHalls.Editor
{
    // Native presentation fixtures only. No game controller, campaign, or save is created.
    public static class UiDeclutterCapture
    {
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

        public static void Capture()
        {
            EventSystem previousSystem = EventSystem.current;
            GameObject previousSelection = previousSystem == null ? null : previousSystem.currentSelectedGameObject;
            EventSystem[] existingSystems = UnityEngine.Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            EventSystem fixtureSystem = null;
            bool registeredForFixture = false;
            bool ownsFixtureSystem = false;
            int exitCode = 1;
            try
            {
                fixtureSystem = UiRuntime.EnsureEventSystemReady();
                Require(fixtureSystem != null, "fixture has an event system");
                ownsFixtureSystem = !existingSystems.Contains(fixtureSystem);
                // EventSystem is not ExecuteAlways: AddComponent in Edit Mode can
                // yield a valid component without its OnEnable registration.
                if (EventSystem.current == null)
                {
                    Invoke(fixtureSystem, "OnEnable", Type.EmptyTypes);
                    registeredForFixture = true;
                }
                EventSystem.current = fixtureSystem;
                Require(EventSystem.current == fixtureSystem, "fixture event system is registered for native navigation");
                string directory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "QA", "ui-declutter",
                    DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff")));
                Directory.CreateDirectory(directory);
                int count = 0;
                foreach (Vector2Int size in new[] { new Vector2Int(960, 600), new Vector2Int(1280, 720) })
                {
                    CaptureTavernSettings(directory, size, fixtureSystem);
                    CaptureEquipment(directory, size, fixtureSystem);
                    count += 2;
                }
                CaptureHelp(directory, true);
                CaptureHelp(directory, false);
                count += 2;
                Require(Directory.GetFiles(directory, "*.png").Length == count && count == 6, "all six requested captures exist");
                Debug.Log("UI DECLUTTER CAPTURES PASSED: " + count + " / " + directory);
                exitCode = 0;
            }
            catch (Exception ex)
            {
                Debug.LogError("UI DECLUTTER CAPTURES FAILED: " + ex);
            }
            finally
            {
                if (fixtureSystem != null)
                {
                    if (!fixtureSystem.alreadySelecting) fixtureSystem.SetSelectedGameObject(null);
                    if (registeredForFixture) Invoke(fixtureSystem, "OnDisable", Type.EmptyTypes);
                    if (ownsFixtureSystem) UnityEngine.Object.DestroyImmediate(fixtureSystem.gameObject);
                }
                if (previousSystem != null)
                {
                    EventSystem.current = previousSystem;
                    if (!previousSystem.alreadySelecting)
                        previousSystem.SetSelectedGameObject(previousSelection != null && previousSelection.activeInHierarchy ? previousSelection : null);
                }
            }
            EditorApplication.Exit(exitCode);
        }

        private static void CaptureTavernSettings(string directory, Vector2Int size, EventSystem eventSystem)
        {
            GameObject host = new GameObject("Declutter tavern settings fixture");
            Texture2D backdrop = null;
            Sprite[] ownedSprites = Array.Empty<Sprite>();
            try
            {
                backdrop = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                string artPath = Path.Combine(Application.dataPath, "..", "Docs", "ArtReferences", RuntimeArtManifest.TavernBackdrop);
                Require(backdrop.LoadImage(File.ReadAllBytes(artPath)), "title backdrop decodes");
                backdrop.filterMode = FilterMode.Bilinear;
                TavernScreen screen = host.AddComponent<TavernScreen>();
                screen.Bind(new TavernScreenBindings
                {
                    Title = VersionInfo.ProductName, VersionLine = () => VersionInfo.PackageVersion,
                    BackdropArt = backdrop, SettingsVisible = () => true,
                    HasSavedGame = () => true, DeveloperTestingVisible = () => false,
                    TestingVisible = () => false, ReducedMotion = () => true,
                    AudioMuted = () => false, MusicMuted = () => false,
                    VolumePercent = () => 75, MusicVolumePercent = () => 50
                });
                screen.SetVisible(true);
                screen.Refresh();
                Canvas canvas = Field<Canvas>(screen, "canvas");
                ownedSprites = canvas.GetComponentsInChildren<Image>(true).Select(image => image.sprite)
                    .Where(sprite => sprite != null && sprite.texture == backdrop).Distinct().ToArray();
                Render(directory, "tavern-settings", size, canvas, () =>
                {
                    Invoke(screen, "ApplyLayout", new[] { typeof(bool), typeof(bool) }, true, false);
                    Invoke(screen, "UpdateTitleAnimation", Type.EmptyTypes);
                    Invoke(screen, "UpdateOpeningPresentation", Type.EmptyTypes);
                    Invoke(screen, "UpdateStormMotion", Type.EmptyTypes);
                    Invoke(screen, "UpdateAtmosphereMotion", Type.EmptyTypes);
                    RectTransform settings = Field<RectTransform>(screen, "settingsPanel");
                    Require(Mathf.Abs(settings.rect.height - 280f) < .1f, "settings captures use the tightened native panel");
                    foreach (Button button in settings.GetComponentsInChildren<Button>())
                        Require(Fits(button.GetComponent<RectTransform>(), settings), "settings control stays inside panel: " + button.name);
                    Require(!Field<Text>(screen, "settingsHintText").gameObject.activeSelf
                        && !Field<Text>(screen, "settingsStateText").gameObject.activeSelf, "repeated settings prose remains hidden");
                    eventSystem.SetSelectedGameObject(Field<Button>(screen, "musicValueButton").gameObject);
                });
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(host);
                foreach (Sprite sprite in ownedSprites) if (sprite != null) UnityEngine.Object.DestroyImmediate(sprite);
                if (backdrop != null) UnityEngine.Object.DestroyImmediate(backdrop);
            }
        }

        private static void CaptureEquipment(string directory, Vector2Int size, EventSystem eventSystem)
        {
            GameObject host = new GameObject("Declutter equipment fixture");
            try
            {
                string[] names = { "Cairn", "Seren", "Orin", "Mira" };
                string[] classes = { "Warrior", "Ranger", "Priest", "Mage" };
                string[] races = { "Human", "Dusk Elf", "Stoneborn", "Ashling" };
                string[] weapons = { "Iron Sword", "Longbow", "Prayer Focus", "Ember Focus" };
                string[] colors = { "c18c61", "82a081", "ada8cf", "cc8263" };
                var rows = new List<ArmoryRowView>();
                for (int i = 0; i < names.Length; i++)
                    rows.Add(new ArmoryRowView
                    {
                        Key = i, Title = names[i], Subtitle = "L1 " + races[i] + " " + classes[i] + "  •  HP 24/24  •  MP 8/8",
                        Detail = "Weapon  " + weapons[i] + "\nArmor     Leather Coat",
                        Badge = classes[i], AccentHex = colors[i], BadgeAccentHex = colors[i], IconLabel = names[i].Substring(0, 1),
                        ActionLabel = i == 1 ? "Viewing" : "Inspect", ActionEnabled = true, Selected = i == 1
                    });
                ArmoryOverlayView view = new ArmoryOverlayView
                {
                    Visible = true, ActiveTab = 0, Title = "Party", Rows = rows, Summary = "128 gold  •  6 supplies",
                    Detail = new ArmoryDetailView
                    {
                        Eyebrow = "EQUIPMENT", Title = "Seren", Subtitle = "L1 Dusk Elf Ranger", IconLabel = "S", AccentHex = "82a081",
                        ExtendedSummary = true,
                        Summary = "WEAPON  Longbow\nRanged / 4–7 dmg / range 6\n\nARMOR  Leather Coat\nArmor 2 / evasion +1"
                    }
                };
                int invokedKey = -1;
                ArmoryOverlayScreen screen = host.AddComponent<ArmoryOverlayScreen>();
                screen.Bind(new ArmoryOverlayBindings { View = () => view, RunRowAction = key => invokedKey = key });
                screen.SetVisible(true);
                screen.Refresh();
                Render(directory, "equipment-selected", size, Field<Canvas>(screen, "canvas"), () =>
                {
                    Invoke(screen, "ApplyLayout", new[] { typeof(bool), typeof(int), typeof(bool), typeof(int), typeof(float), typeof(float) },
                        true, 0, true, 0, (float)size.x, (float)size.y);
                    screen.FocusRowForTest(1);
                    screen.ScrollRowIntoViewForTest(0);
                    Transform row = Field<RectTransform>(screen, "contentRoot").Find("Row 1");
                    Require(row != null && !row.Find("Action").gameObject.activeSelf && !row.Find("Badge").gameObject.activeSelf,
                        "selected equipment row omits duplicate action and class badge");
                    Require(eventSystem.currentSelectedGameObject == row.gameObject && row.GetComponent<Button>().interactable,
                        "selected equipment row keeps a visible native focus target");
                    screen.InvokeFocusedRowForTest();
                    Require(invokedKey == 1, "selected equipment row still submits its own companion");
                });
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        private static void CaptureHelp(string directory, bool top)
        {
            GameObject host = new GameObject("Declutter help fixture");
            try
            {
                HelpOverlayScreen screen = host.AddComponent<HelpOverlayScreen>();
                HelpOverlayView view = HelpOverlayContent.Build(GameMode.Combat, false, 4, "Midgaard");
                screen.Bind(new HelpOverlayBindings { View = () => view });
                screen.SetVisible(true);
                screen.Refresh();
                Render(directory, top ? "help-top" : "help-bottom", new Vector2Int(960, 600), Field<Canvas>(screen, "canvas"), () =>
                {
                    Invoke(screen, "ApplyLayout", new[] { typeof(float), typeof(float) }, 960f, 600f);
                    Canvas.ForceUpdateCanvases();
                    Text text = Field<Text>(screen, "bodyText");
                    RectTransform viewport = Field<RectTransform>(screen, "bodyViewport");
                    // Refresh uses this same preferred-height sizing after assigning the live help text.
                    text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Max(viewport.rect.height, text.preferredHeight));
                    Require(text.rectTransform.rect.height > viewport.rect.height + 20f, "compact combat help has genuine scrollable content");
                    Canvas.ForceUpdateCanvases();
                    ScrollRect scroll = Field<ScrollRect>(screen, "bodyScroll");
                    scroll.StopMovement();
                    scroll.verticalNormalizedPosition = top ? 1f : 0f;
                    Canvas.ForceUpdateCanvases();
                    scroll.Rebuild(CanvasUpdate.PostLayout);
                    Require(Mathf.Abs(scroll.verticalNormalizedPosition - (top ? 1f : 0f)) < .01f, "help reaches requested scroll edge");
                });
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        private static void Render(string directory, string name, Vector2Int size, Canvas canvas, Action layout)
        {
            // Reuse the existing offscreen native-canvas renderer, PNG validation,
            // RenderTexture restoration, and camera disposal without altering that helper.
            MethodInfo render = typeof(UiFinishCapture).GetMethod("Render", BindingFlags.Static | BindingFlags.NonPublic);
            Require(render != null, "shared UI capture renderer is available");
            render.Invoke(null, new object[] { directory, name, size, canvas, layout });
        }

        private static bool Fits(RectTransform child, RectTransform parent)
        {
            Vector3[] corners = new Vector3[4];
            child.GetWorldCorners(corners);
            return corners.All(corner =>
            {
                Vector3 local = parent.InverseTransformPoint(corner);
                return local.x >= parent.rect.xMin - .5f && local.x <= parent.rect.xMax + .5f
                    && local.y >= parent.rect.yMin - .5f && local.y <= parent.rect.yMax + .5f;
            });
        }

        private static T Field<T>(object owner, string name) where T : class
        {
            T value = owner.GetType().GetField(name, Private)?.GetValue(owner) as T;
            Require(value != null, "native field exists: " + name);
            return value;
        }

        private static object Invoke(object owner, string name, Type[] signature, params object[] arguments)
        {
            MethodInfo method = owner.GetType().GetMethod(name, Private, null, signature, null);
            Require(method != null, "native layout method exists: " + name);
            return method.Invoke(owner, arguments);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("UI declutter capture: " + message);
        }
    }
}
