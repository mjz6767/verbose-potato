using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AshenHalls.Editor
{
    // Native production views over disposable fixtures; no live campaign is read or written.
    public static class HonestWeightContentCapture
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
                if (EventSystem.current == null) { Invoke(fixtureSystem, "OnEnable"); registered = true; }
                EventSystem.current = fixtureSystem;
                atlas = LoadAtlas(RuntimeArtManifest.HonestWeightItemAtlas);
                portraits = LoadAtlas(RuntimeArtManifest.NpcPortraitAtlas);
                string directory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "QA", "honest-weight-content",
                    DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff")));
                Directory.CreateDirectory(directory);
                int count = 0;
                foreach (Vector2Int size in new[] { new Vector2Int(960, 600), new Vector2Int(1280, 720) })
                {
                    foreach (bool knife in new[] { true, false })
                    {
                        CaptureItem(directory, size, knife, atlas, true);
                        CaptureItem(directory, size, knife, atlas, false);
                        count += 2;
                    }
                    count += CaptureDialogue(directory, size, portraits, "maud-offer", 0, "");
                    count += CaptureDialogue(directory, size, portraits, "maud-progress", 2, "");
                    count += CaptureDialogue(directory, size, portraits, "maud-evidence", 3, "");
                    count += CaptureDialogue(directory, size, portraits, "maud-break-review", 3, "break");
                    count += CaptureDialogue(directory, size, portraits, "maud-retune-review", 3, "retune");
                    count += CaptureDialogue(directory, size, portraits, "maud-reward-choice", 4, "reward");
                    count += CaptureDialogue(directory, size, portraits, "maud-broken-aftermath", 4, "");
                    count += CaptureDialogue(directory, size, portraits, "maud-retuned-aftermath", 5, "");
                    count += CaptureDialogue(directory, size, portraits, "maud-broken-account", 4, "account");
                    count += CaptureDialogue(directory, size, portraits, "maud-retuned-account", 5, "account");
                    count += CaptureDialogue(directory, size, portraits, "maud-offer-services", 0, "services");
                    count += CaptureDialogue(directory, size, portraits, "maud-accepted-services", 2, "services");
                    count += CaptureDialogue(directory, size, portraits, "maud-restored-services", 6, "services");
                    count += CaptureDialogue(directory, size, portraits, "maud-craft-topics", 6, "craft");
                    count += CaptureDialogue(directory, size, portraits, "maud-rune-explanation", 6, "runes");
                    count += CaptureDialogue(directory, size, portraits, "maud-first-enchantment", 6, "story");
                    count += CaptureDialogue(directory, size, portraits, "market-inspection-choice", 1, "market");
                    count += CaptureDialogue(directory, size, portraits, "quarry-inspection-choice", 1, "quarry");
                    CaptureJournal(directory, size, 2);
                    CaptureJournal(directory, size, 6);
                    count += 2;
                }
                Require(Directory.GetFiles(directory, "*.png").Length == count, "all native captures exist");
                File.WriteAllText(Path.Combine(directory, "README.txt"),
                    "Native Honest Weight dialogue, Pack, loot and Journal adapters over disposable fixtures.\n"
                    + "Includes every page of the offer, progress, evidence, both consequence reviews, independent reward choice, both aftermaths/accounts, all three Maud root states, craft topics, rune explanation, personal story and both inspection menus.\n"
                    + "Knife and jack represent separate mutually exclusive rewards; both are captured in Pack and loot at 960x600 and 1280x720.\n"
                    + "No quest transaction callback is executed and no campaign save is read or written. Visual review is required.\n");
                Debug.Log("HONEST WEIGHT CONTENT CAPTURES PASSED: " + directory);
                exitCode = 0;
            }
            catch (Exception exception) { Debug.LogError("HONEST WEIGHT CONTENT CAPTURES FAILED: " + exception); }
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

        private static void CaptureItem(string directory, Vector2Int size, bool knife, Texture2D atlas, bool pack)
        {
            GameObject fixture = RemembranceItemSmoke.NewHost("Honest Weight item fixture");
            GameObject presentation = new GameObject("Honest Weight native item capture");
            try
            {
                AshenHallsGame game = fixture.AddComponent<AshenHallsGame>();
                Bind(game, NewState(6));
                Set(game, "honestWeightItemAtlas", atlas);
                InventoryItem item = knife ? SignatureItemCatalog.CreateCounterweightKnife() : SignatureItemCatalog.CreateMeasurekeeperJack();
                Invoke(game, "AddInventoryItem", item);
                string name = knife ? "counterweight-knife" : "measurekeeper-jack";
                if (pack)
                {
                    Set(game, "showArmory", true);
                    SetTab(game, "Pack");
                    Invoke(game, "SelectArmoryInventoryItem", item);
                    ArmoryOverlayView view = (ArmoryOverlayView)Invoke(game, "BuildArmoryOverlayView");
                    Require(view.Detail != null && view.Detail.IconTexture == atlas, "Pack detail contains approved reward art");
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
                    Set(game, "lootPanelTitle", HonestWeightQuestRules.Title);
                    Set(game, "lootPanelBody", item.DisplayName);
                    Set(game, "lootPanelRequiresDismissal", true);
                    Set(game, "lootPanelEquipNote", "Maud's workshop reward. Added to the pack for deliberate equipping.");
                    Set(game, "lootPanelTraitLine", Invoke(game, "ItemTraitLine", item));
                    LootPopupView view = (LootPopupView)Invoke(game, "BuildLootPopupView");
                    Require(view.IconTexture == atlas && view.CanReview, "loot exposes the art and inventory review action");
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

        private static int CaptureDialogue(string directory, Vector2Int size, Texture2D portraits, string name, int progress, string response)
        {
            GameObject fixture = RemembranceItemSmoke.NewHost("Honest Weight dialogue fixture");
            GameObject presentation = new GameObject("Honest Weight native dialogue capture");
            try
            {
                AshenHallsGame game = fixture.AddComponent<AshenHallsGame>();
                GameState state = NewState(progress);
                Bind(game, state);
                Set(game, "npcPortraitAtlas", portraits);
                bool site = response == "market" || response == "quarry";
                if (site)
                {
                    state.Depth = HonestWeightQuestRules.QuestDepth;
                    string id = response == "market" ? HonestWeightQuestRules.MarketSite : HonestWeightQuestRules.QuarrySite;
                    WorldMapSite worldSite = new WorldMapSite(id, response == "market" ? "dusk-market" : "old-quarry", name, "", 4, 4, 1, ObjectType.Obelisk);
                    Require((bool)Invoke(game, "TryResolveHonestWeightSite", worldSite), "the actual optional inspection menu opens");
                }
                else if (response == "break" || response == "retune") Invoke(game, "ShowHonestWeightDispositionReview", response);
                else if (response == "reward") Invoke(game, "ShowHonestWeightRewardChoice");
                else if (response == "services") Invoke(game, "ShowMaudConversation");
                else if (response == "craft" || response == "runes" || response == "story")
                {
                    Invoke(game, "ShowMaudConversation");
                    Invoke(game, "ResolveMaudDialogueChoice", "craft");
                    if (response != "craft") Invoke(game, "ResolveMaudDialogueChoice", response == "runes" ? "affinity" : "personal");
                }
                else
                {
                    Invoke(game, "ShowHonestWeightConversation");
                    if (response == "account") Invoke(game, "ResolveHonestWeightConversation", response);
                }
                DialogueScreen screen = presentation.AddComponent<DialogueScreen>();
                screen.Bind(new DialogueScreenBindings { View = () => (DialogueScreenView)Invoke(game, "BuildDialogueScreenView") });
                int pages = Field<string[]>(game, "dialoguePages").Length;
                Require(pages > 0, name + " has actual pages");
                for (int page = 0; page < pages; page++)
                {
                    DialogueScreenView view = (DialogueScreenView)Invoke(game, "BuildDialogueScreenView");
                    Require(site ? view.PortraitTexture == null : view.Speaker == "Maud" && view.PortraitTexture == portraits,
                        name + " uses the appropriate speaker identity");
                    Require(page < pages - 1 ? view.Choices.Length == 0 : view.Choices.Length > 0 || !string.IsNullOrEmpty(view.ContinueLabel),
                        name + " holds interactive choices until the final page");
                    if (response == "services" && page == pages - 1)
                        Require(view.Choices.Length == 4
                            && new[] { "temporary", "permanent", "honestweight", "craft" }.All(id => view.Choices.Any(choice => choice.Id == id)),
                            "Maud's native service choices keep both purchases, the quest and craft topics accessible");
                    if (response == "craft" && page == pages - 1)
                        Require(new[] { "affinity", "personal", "back" }.All(id => view.Choices.Any(choice => choice.Id == id)),
                            "the native craft submenu exposes both earlier conversations and the return route");
                    screen.SetVisible(true);
                    screen.Refresh();
                    Render(directory, name + "-page" + (page + 1), size, Field<Canvas>(screen, "canvas"), () =>
                    {
                        Layout(screen, new[] { typeof(int), typeof(float), typeof(float) }, view.Choices.Length, (float)size.x, (float)size.y);
                        Invoke(screen, "UpdateBodyContentLayout");
                    });
                    if (page + 1 < pages) Invoke(game, "AdvanceDialogue");
                }
                if (response == "runes" || response == "story")
                {
                    Invoke(game, "AdvanceDialogue");
                    DialogueScreenView returned = (DialogueScreenView)Invoke(game, "BuildDialogueScreenView");
                    Require(new[] { "affinity", "personal", "back" }.All(id => returned.Choices.Any(choice => choice.Id == id)),
                        "the actual finished response returns to the reachable craft submenu");
                }
                return pages;
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(presentation);
                UnityEngine.Object.DestroyImmediate(fixture);
            }
        }

        private static void CaptureJournal(string directory, Vector2Int size, int progress)
        {
            GameObject fixture = RemembranceItemSmoke.NewHost("Honest Weight Journal fixture");
            GameObject presentation = new GameObject("Honest Weight native Journal capture");
            try
            {
                AshenHallsGame game = fixture.AddComponent<AshenHallsGame>();
                Bind(game, NewState(progress));
                Set(game, "showArmory", true);
                SetTab(game, "Journal");
                ArmoryOverlayView view = (ArmoryOverlayView)Invoke(game, "BuildArmoryOverlayView");
                int row = view.Rows.ToList().FindIndex(entry => entry.Title == HonestWeightQuestRules.Title);
                Require(row >= 0 && view.Rows[row].Subtitle.Contains(progress == 6 ? "complete" : "investigating"),
                    "the real Journal shows the optional quest's current state");
                ArmoryOverlayScreen screen = presentation.AddComponent<ArmoryOverlayScreen>();
                screen.Bind(new ArmoryOverlayBindings { View = () => view });
                screen.SetVisible(true);
                screen.Refresh();
                Render(directory, progress == 6 ? "journal-honest-weight-complete" : "journal-honest-weight-progress", size, Field<Canvas>(screen, "canvas"), () =>
                {
                    Layout(screen, new[] { typeof(bool), typeof(int), typeof(bool), typeof(int), typeof(float), typeof(float) },
                        view.Detail != null, view.Filters.Count, view.Detail?.ExtendedSummary ?? false, view.Detail?.Actions.Count ?? 0,
                        (float)size.x, (float)size.y);
                    screen.ScrollRowIntoViewForTest(row);
                });
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(presentation);
                UnityEngine.Object.DestroyImmediate(fixture);
            }
        }

        private static GameState NewState(int progress)
        {
            GameState state = new GameState
            {
                SaveVersion = VersionInfo.SaveVersion, Mode = GameMode.Explore, Depth = 1, StoryChapter = 3, Seed = 2340,
                Gold = 146, Supplies = 5, Elixirs = 2, ReducedMotion = true, SfxMuted = true, MusicMuted = true
            };
            state.StoryFlags.AddRange(new[] { StoryFlags.MidgaardKingMet, StoryFlags.SewerContractAccepted,
                StoryFlags.SewerBrokenSluiceCleared, StoryFlags.SewerFoulRunoffCleared, StoryFlags.SewerCisternDenCleared,
                StoryFlags.SewerRewardClaimed, StoryFlags.OldRoadTeaserUnlocked, StoryFlags.KoboldKingDefeated, StoryFlags.BoneRoadEntered });
            if (progress >= 1) state.StoryFlags.Add(StoryFlags.HonestWeightAccepted);
            if (progress >= 2) state.StoryFlags.Add(StoryFlags.HonestWeightContractRead);
            if (progress >= 3) state.StoryFlags.Add(StoryFlags.HonestWeightCalibrationRead);
            if (progress == 4) state.StoryFlags.Add(StoryFlags.HonestWeightBroken);
            if (progress >= 5) state.StoryFlags.Add(StoryFlags.HonestWeightRetuned);
            if (progress >= 6) state.StoryFlags.Add(StoryFlags.HonestWeightRewardClaimed);
            foreach (StarterHeroDef hero in StarterPartyCatalog.All)
                state.Party.Add(new PartyMember
                {
                    Id = "honest-capture-" + hero.ClassKey, Name = hero.Name, Race = hero.Race, ClassKey = hero.ClassKey,
                    Role = hero.Role, Stats = hero.Stats, Skills = hero.CreateSkills(), Spell = hero.Spell,
                    Level = 7, Hp = 38, MaxHp = 38, Mana = 20, MaxMana = 20, Range = hero.Range,
                    WeaponName = hero.Role == "bow" ? "plain longbow" : "plain steel sword",
                    ArmorName = "plain traveling coat", DamageMin = 4, DamageMax = 7, AttackSpeed = 8, SpriteColor = "#82a081"
                });
            return state;
        }

        private static void Bind(AshenHallsGame game, GameState state)
        {
            Set(game, "state", state);
            Set(game, "rng", new System.Random(state.Seed));
            Set(game, "activeContentSet", ContentSetCatalog.SewerSlice);
            Set(game, "visualSmokeSaveBlocked", true);
            Set(game, "launchError", "Honest Weight content fixture presentation held");
        }

        private static void SetTab(AshenHallsGame game, string name)
        {
            Type tab = typeof(AshenHallsGame).GetNestedType("ArmoryTab", BindingFlags.NonPublic);
            Require(tab != null, "the native Armory tab enum exists");
            Set(game, "armoryTab", Convert.ToInt32(Enum.Parse(tab, name)));
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
            MethodInfo method = typeof(UiFinishCapture).GetMethod("Render", BindingFlags.Static | BindingFlags.NonPublic);
            Require(method != null, "shared native renderer exists");
            method.Invoke(null, new object[] { directory, name, size, canvas, layout });
        }
        private static void Layout(object screen, Type[] signature, params object[] args)
        {
            MethodInfo method = screen.GetType().GetMethod("ApplyLayout", Private, null, signature, null);
            Require(method != null, "native requested-size layout exists");
            method.Invoke(screen, args);
        }
        private static T Field<T>(object owner, string name) => (T)owner.GetType().GetField(name, Private).GetValue(owner);
        private static object Invoke(object owner, string name, params object[] args) => RemembranceItemSmoke.Invoke(owner, name, args);
        private static void Set(object owner, string name, object value) => RemembranceItemSmoke.Set(owner, name, value);
        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("Honest Weight content: " + message);
        }
    }
}
