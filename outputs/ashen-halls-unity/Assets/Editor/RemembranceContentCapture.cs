using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AshenHalls.Editor
{
    // Production adapters and native screens over disposable campaign fixtures.
    // No campaign save is loaded, and no transaction/checkpoint callback is invoked.
    public static class RemembranceContentCapture
    {
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

        public static void Capture()
        {
            int exitCode = 1;
            Texture2D atlas = null;
            Texture2D portraits = null;
            EventSystem previousSystem = EventSystem.current;
            EventSystem[] previousSystems = UnityEngine.Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            EventSystem fixtureSystem = null;
            bool registered = false;
            try
            {
                fixtureSystem = UiRuntime.EnsureEventSystemReady();
                if (EventSystem.current == null)
                {
                    Invoke(fixtureSystem, "OnEnable");
                    registered = true;
                }
                EventSystem.current = fixtureSystem;
                atlas = LoadAtlas(RuntimeArtManifest.RemembranceItemAtlas);
                portraits = LoadAtlas(RuntimeArtManifest.NpcPortraitAtlas);
                string directory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "QA", "remembrance-content",
                    DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff")));
                Directory.CreateDirectory(directory);
                foreach (Vector2Int size in new[] { new Vector2Int(960, 600), new Vector2Int(1280, 720) })
                foreach (bool bow in new[] { true, false })
                {
                    CaptureItem(directory, size, bow, atlas, false);
                    CaptureItem(directory, size, bow, atlas, true);
                }
                int captureCount = 8;
                foreach (Vector2Int size in new[] { new Vector2Int(960, 600), new Vector2Int(1280, 720) })
                {
                    captureCount += CaptureConversation(directory, portraits, false, size);
                    captureCount += CaptureConversation(directory, portraits, true, size);
                }
                Require(Directory.GetFiles(directory, "*.png").Length == captureCount, "every production content capture exists");
                File.WriteAllText(Path.Combine(directory, "README.txt"),
                    "Disposable fixture captures of production dialogue, Pack, and loot adapters.\n"
                    + "Bow and wrap images represent separate, mutually exclusive reward branches.\n"
                    + "All dialogue pages, equipment, and loot use the native 960x600 and 1280x720 layouts.\n"
                    + "No campaign saves were read or written. Visual review is required.\n");
                Debug.Log("REMEMBRANCE CONTENT CAPTURES PASSED: " + directory);
                exitCode = 0;
            }
            catch (Exception exception)
            {
                Debug.LogError("REMEMBRANCE CONTENT CAPTURES FAILED: " + exception);
            }
            finally
            {
                if (atlas != null) UnityEngine.Object.DestroyImmediate(atlas);
                if (portraits != null) UnityEngine.Object.DestroyImmediate(portraits);
                if (fixtureSystem != null)
                {
                    if (!fixtureSystem.alreadySelecting) fixtureSystem.SetSelectedGameObject(null);
                    if (registered) Invoke(fixtureSystem, "OnDisable");
                    if (!previousSystems.Contains(fixtureSystem)) UnityEngine.Object.DestroyImmediate(fixtureSystem.gameObject);
                }
                if (previousSystem != null) EventSystem.current = previousSystem;
            }
            EditorApplication.Exit(exitCode);
        }

        private static void CaptureItem(string directory, Vector2Int size, bool bow, Texture2D atlas, bool pack)
        {
            GameObject fixture = RemembranceItemSmoke.NewHost("Remembrance production game fixture");
            GameObject presentation = new GameObject("Remembrance native item capture");
            try
            {
                AshenHallsGame game = fixture.AddComponent<AshenHallsGame>();
                GameState state = NewState();
                Set(game, "state", state);
                Set(game, "rng", new System.Random(2320));
                Set(game, "remembranceItemAtlas", atlas);
                InventoryItem item = bow ? SignatureItemCatalog.CreateTravelersKnot() : SignatureItemCatalog.CreateVigilkeeperWrap();
                Invoke(game, "AddInventoryItem", item);
                string name = bow ? "travelers-knot" : "vigilkeeper-wrap";
                if (pack)
                {
                    Set(game, "showArmory", true);
                    Set(game, "armoryTab", 1);
                    Invoke(game, "SelectArmoryInventoryItem", item);
                    ArmoryOverlayView view = (ArmoryOverlayView)Invoke(game, "BuildArmoryOverlayView");
                    Require(view.Detail != null && view.Detail.IconTexture == atlas, "production Pack has its actual reward art");
                    ArmoryOverlayScreen screen = presentation.AddComponent<ArmoryOverlayScreen>();
                    screen.Bind(new ArmoryOverlayBindings { View = () => view });
                    screen.SetVisible(true);
                    screen.Refresh();
                    Render(directory, name + "-pack", size, Field<Canvas>(screen, "canvas"), () =>
                        Layout(screen, new[] { typeof(bool), typeof(int), typeof(bool), typeof(int), typeof(float), typeof(float) },
                            true, view.Filters.Count, view.Detail.ExtendedSummary, view.Detail.Actions.Count, (float)size.x, (float)size.y));
                }
                else
                {
                    Set(game, "lootPanelItem", item);
                    Set(game, "lootPanelTitle", RemembranceQuestRules.Title);
                    Set(game, "lootPanelBody", item.DisplayName);
                    Set(game, "lootPanelRequiresDismissal", true);
                    Set(game, "lootPanelEquipNote", "Edda's keepsake. Added to the pack for deliberate equipping.");
                    Set(game, "lootPanelTraitLine", Invoke(game, "ItemTraitLine", item));
                    LootPopupView view = (LootPopupView)Invoke(game, "BuildLootPopupView");
                    Require(view.IconTexture == atlas && view.CanReview, "production reward offers the actual art and review action");
                    LootPopupScreen screen = presentation.AddComponent<LootPopupScreen>();
                    screen.Bind(new LootPopupBindings { View = () => view });
                    screen.SetVisible(true);
                    screen.Refresh();
                    Render(directory, name + "-loot", size, Field<Canvas>(screen, "canvas"), () =>
                        Layout(screen, new[] { typeof(float), typeof(float) }, (float)size.x, (float)size.y));
                }
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(presentation);
                UnityEngine.Object.DestroyImmediate(fixture);
            }
        }

        private static int CaptureConversation(string directory, Texture2D portraits, bool reward, Vector2Int size)
        {
            GameObject fixture = RemembranceItemSmoke.NewHost("Remembrance production dialogue fixture");
            GameObject presentation = new GameObject("Remembrance native dialogue capture");
            try
            {
                AshenHallsGame game = fixture.AddComponent<AshenHallsGame>();
                GameState state = NewState();
                Set(game, "state", state);
                Set(game, "npcPortraitAtlas", portraits);
                if (reward)
                {
                    state.StoryFlags.Add(StoryFlags.RemembranceDelivered);
                    Invoke(game, "ShowRemembranceRewardChoice", ObjectType.GateCaptain);
                }
                else Invoke(game, "ShowEddaRemembrance");
                DialogueScreen screen = presentation.AddComponent<DialogueScreen>();
                screen.Bind(new DialogueScreenBindings { View = () => (DialogueScreenView)Invoke(game, "BuildDialogueScreenView") });
                int pageCount = Field<string[]>(game, "dialoguePages").Length;
                for (int page = 0; page < pageCount; page++)
                {
                    DialogueScreenView view = (DialogueScreenView)Invoke(game, "BuildDialogueScreenView");
                    int expectedChoices = page == pageCount - 1 ? reward ? 3 : 2 : 0;
                    Require(view.PortraitTexture == portraits && view.Choices.Length == expectedChoices,
                        "the actual quest conversation exposes its portrait and choices only on its final page");
                    screen.SetVisible(true);
                    screen.Refresh();
                    string name = (reward ? "brann-keepsake-choice" : "edda-remembrance-offer") + "-page" + (page + 1);
                    Render(directory, name, size, Field<Canvas>(screen, "canvas"), () =>
                    {
                        Layout(screen, new[] { typeof(int), typeof(float), typeof(float) }, view.Choices.Length, (float)size.x, (float)size.y);
                        Invoke(screen, "UpdateBodyContentLayout");
                    });
                    if (page + 1 < pageCount) Invoke(game, "AdvanceDialogue");
                }
                return pageCount;
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(presentation);
                UnityEngine.Object.DestroyImmediate(fixture);
            }
        }

        private static GameState NewState()
        {
            GameState state = new GameState
            {
                Mode = GameMode.Explore, Depth = 1, Seed = 2320,
                Gold = 52, Supplies = 5, Elixirs = 2, ReducedMotion = true, SfxMuted = true, MusicMuted = true
            };
            foreach (StarterHeroDef hero in StarterPartyCatalog.All)
            {
                state.Party.Add(new PartyMember
                {
                    Id = "capture-" + hero.ClassKey, Name = hero.Name, Race = hero.Race, ClassKey = hero.ClassKey,
                    Role = hero.Role, Stats = hero.Stats, Skills = hero.CreateSkills(), Spell = hero.Spell,
                    Level = 2, Hp = 28, MaxHp = 28, Mana = 12, MaxMana = 12, Range = hero.Range,
                    WeaponName = hero.Role == "bow" ? "plain shortbow" : "plain iron sword",
                    ArmorName = "plain traveling coat", DamageMin = 2, DamageMax = 4, AttackSpeed = 8,
                    SpriteColor = "#82a081"
                });
            }
            return state;
        }

        private static Texture2D LoadAtlas(string filename)
        {
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            Require(texture.LoadImage(File.ReadAllBytes(Path.Combine(Application.dataPath, "..", "Docs", "ArtReferences", filename))), filename + " decodes");
            texture.filterMode = FilterMode.Bilinear;
            return texture;
        }

        private static void Render(string directory, string name, Vector2Int size, Canvas canvas, Action layout)
        {
            MethodInfo render = typeof(UiFinishCapture).GetMethod("Render", BindingFlags.Static | BindingFlags.NonPublic);
            Require(render != null, "shared native canvas renderer exists");
            render.Invoke(null, new object[] { directory, name, size, canvas, layout });
        }

        private static void Layout(object screen, Type[] signature, params object[] arguments)
        {
            MethodInfo method = screen.GetType().GetMethod("ApplyLayout", Private, null, signature, null);
            Require(method != null, "native layout overload exists");
            method.Invoke(screen, arguments);
        }

        private static T Field<T>(object owner, string name) => (T)owner.GetType().GetField(name, Private).GetValue(owner);
        private static object Invoke(object owner, string name, params object[] arguments) => RemembranceItemSmoke.Invoke(owner, name, arguments);
        private static void Set(object owner, string name, object value) => RemembranceItemSmoke.Set(owner, name, value);
        private static void Require(bool condition, string message) => RemembranceItemSmoke.Require(condition, message);
    }
}
