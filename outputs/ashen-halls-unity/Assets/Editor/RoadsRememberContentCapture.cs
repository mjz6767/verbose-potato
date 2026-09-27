using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AshenHalls.Editor
{
    // Disposable native-view fixtures, including every page and the final choice footer.
    public static class RoadsRememberContentCapture
    {
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
        public static void Capture()
        {
            int exit = 1;
            Texture2D stock = null, portraits = null, honest = null;
            EventSystem previous = EventSystem.current;
            EventSystem[] systems = UnityEngine.Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            EventSystem system = null;
            bool registered = false;
            try
            {
                system = UiRuntime.EnsureEventSystemReady();
                if (EventSystem.current == null) { Invoke(system, "OnEnable"); registered = true; }
                EventSystem.current = system;
                stock = LoadAtlas(RuntimeArtManifest.RoadStockItemAtlas);
                portraits = LoadAtlas(RuntimeArtManifest.NpcPortraitAtlas);
                honest = LoadAtlas(RuntimeArtManifest.HonestWeightItemAtlas);
                string directory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "QA", "roads-remember-content", DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff")));
                Directory.CreateDirectory(directory);
                int count = 0;
                foreach (Vector2Int size in new[] { new Vector2Int(960, 600), new Vector2Int(1280, 720) })
                {
                    foreach (string scenario in new[] { "root-before", "root-unlocked", "root-purchased", "stock", "stock-sold", "spear-review", "axe-review", "spear-low-funds", "axe-low-funds", "cancel", "advice", "forms", "range", "personal" })
                        count += CaptureDialogue(directory, size, portraits, "tessa-" + scenario, scenario == "root-before" ? 1 : 3, (game, state) => ConfigureTessa(game, state, scenario));
                    foreach (int stage in new[] { 1, 5, 7 })
                    foreach (string contact in new[] { "halvard", "brann" })
                    foreach (string topic in contact == "halvard" ? new[] { "root", "duty", "city", "road" } : new[] { "root", "watch", "roads", "sealed" })
                    {
                        string label = stage == 1 ? "paid-writ" : stage == 5 ? "final-plan-ready" : "homecoming";
                        count += CaptureDialogue(directory, size, portraits, contact + "-" + label + "-" + topic, stage, (game, state) =>
                        {
                            if (contact == "halvard") Invoke(game, "ShowHalvardConversation");
                            else Invoke(game, "ShowBrannConversation", new object[] { null });
                            if (topic != "root") Choose(game, topic);
                        });
                    }
                    foreach (int stage in new[] { 0, 3, 6, 7 }) { CaptureJournal(directory, size, stage); count++; }
                    foreach (string id in new[] { "bridgehook_spear", "stonecleft_axe", "counterweight_knife" })
                    {
                        CaptureItem(directory, size, id, id == "counterweight_knife" ? honest : stock, true); count++;
                        if (id != "counterweight_knife") { CaptureItem(directory, size, id, stock, false); count++; }
                    }
                }
                Require(Directory.GetFiles(directory, "*.png").Length == count, "all requested native pages exist");
                File.WriteAllText(Path.Combine(directory, "README.txt"),
                    "Native production views over disposable campaign fixtures; no live save is read or written.\n"
                    + "Every dialogue page is captured at 960x600 and 1280x720, including final native choice footers.\n"
                    + "Tessa: before/unlocked/purchased roots, stock and sold stock, both 76-gold reviews, both insufficient-funds reviews, actual cancellation, advice submenu, forms, range and personal story. Prices come from production rules. Purchased fixtures contain matching receipt flags and items.\n"
                    + "Halvard and Brann: paid writ, final plan awaiting acceptance, and homecoming roots plus every ordinary topic. Journal opens at its actual first row for Chapters I/III/VI and the epilogue, with Maud's available lead when eligible. Both road weapons use native Pack and loot adapters; Counterweight Knife Pack is recaptured for wearer-range text.\n"
                    + "These are rendering fixtures, not connected playthrough evidence. Manual visual review remains required.\n");
                Debug.Log("ROADS REMEMBER CONTENT CAPTURES PASSED: " + directory); exit = 0;
            }
            catch (Exception exception) { Debug.LogError("ROADS REMEMBER CONTENT CAPTURES FAILED: " + exception); }
            finally
            {
                foreach (Texture2D texture in new[] { stock, portraits, honest }) if (texture != null) UnityEngine.Object.DestroyImmediate(texture);
                if (system != null)
                {
                    if (!system.alreadySelecting) system.SetSelectedGameObject(null);
                    if (registered) Invoke(system, "OnDisable");
                    if (!systems.Contains(system)) UnityEngine.Object.DestroyImmediate(system.gameObject);
                }
                if (previous != null) EventSystem.current = previous;
            }
            EditorApplication.Exit(exit);
        }

        private static void ConfigureTessa(AshenHallsGame game, GameState state, string scenario)
        {
            if (scenario == "root-purchased" || scenario == "stock-sold")
            {
                state.StoryFlags.Add(StoryFlags.MidgaardBasicWeaponBought);
                foreach (TessaRoadStockOffer offer in TessaRoadStockRules.All)
                {
                    state.StoryFlags.Add(offer.ReceiptFlag);
                    Invoke(game, "AddInventoryItem", SignatureItemCatalog.Create(offer.SignatureId));
                }
            }
            Invoke(game, "ShowTessaConversation", new object[] { null });
            if (scenario.StartsWith("root", StringComparison.Ordinal)) return;
            if (scenario == "advice" || scenario == "forms" || scenario == "range")
            {
                Choose(game, "advice");
                if (scenario != "advice") Choose(game, scenario);
                return;
            }
            if (scenario == "personal") { Choose(game, "personal"); return; }
            Choose(game, "roadstock");
            if (scenario == "stock" || scenario == "stock-sold") return;
            string id = scenario.StartsWith("axe", StringComparison.Ordinal) ? "stonecleft_axe" : "bridgehook_spear";
            TessaRoadStockOffer selected = TessaRoadStockRules.Find(id);
            if (scenario.EndsWith("low-funds", StringComparison.Ordinal)) state.Gold = selected.Price - 1;
            int gold = state.Gold; string flags = string.Join("|", state.StoryFlags); int items = state.Inventory.Count;
            Choose(game, id);
            if (scenario == "cancel")
            {
                Choose(game, "back");
                Require(state.Gold == gold && string.Join("|", state.StoryFlags) == flags && state.Inventory.Count == items,
                    "actual quote cancellation leaves money, receipts and inventory unchanged");
            }
        }

        private static int CaptureDialogue(string directory, Vector2Int size, Texture2D portraits, string name, int stage, Action<AshenHallsGame, GameState> configure)
        {
            GameObject host = RemembranceItemSmoke.NewHost("Roads remember dialogue fixture");
            GameObject presentation = new GameObject("Roads remember native dialogue");
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                GameState state = NewState(stage); Bind(game, state);
                Set(game, "npcPortraitAtlas", portraits);
                configure(game, state);
                // Configure may have advanced an earlier menu; the resulting node starts on page one.
                Set(game, "dialoguePageIndex", 0);
                DialogueScreen screen = presentation.AddComponent<DialogueScreen>();
                screen.Bind(new DialogueScreenBindings { View = () => (DialogueScreenView)Invoke(game, "BuildDialogueScreenView") });
                int pages = Field<string[]>(game, "dialoguePages").Length;
                Require(pages > 0, name + " has authored pages");
                for (int page = 0; page < pages; page++)
                {
                    DialogueScreenView view = (DialogueScreenView)Invoke(game, "BuildDialogueScreenView");
                    Require(view.PortraitTexture == portraits, name + " resolves the actual named NPC portrait");
                    Require(view.Choices.Length <= 4, name + " cannot hide a fifth choice");
                    if (name.StartsWith("tessa-root", StringComparison.Ordinal) && page == pages - 1)
                        Require(view.Choices.Any(choice => choice.Id == (stage < 3 ? "forms" : "roadstock")), "Tessa's right root entry is visible");
                    if (name.EndsWith("low-funds", StringComparison.Ordinal) && page == pages - 1)
                        Require(view.Choices.Any(choice => choice.Id == "confirm" && !choice.Enabled), "unaffordable purchase stays disabled");
                    screen.SetVisible(true); screen.Refresh();
                    Require(screen.VisibleChoiceCountForTest == view.Choices.Length, "native buttons match the adapted menu");
                    Render(directory, name + "-page" + (page + 1), size, Field<Canvas>(screen, "canvas"), () =>
                    {
                        Layout(screen, new[] { typeof(int), typeof(float), typeof(float) }, view.Choices.Length, (float)size.x, (float)size.y);
                        Invoke(screen, "UpdateBodyContentLayout");
                    });
                    if (page + 1 < pages) Invoke(game, "AdvanceDialogue");
                }
                return pages;
            }
            finally { UnityEngine.Object.DestroyImmediate(presentation); UnityEngine.Object.DestroyImmediate(host); }
        }

        private static void CaptureJournal(string directory, Vector2Int size, int stage)
        {
            GameObject host = RemembranceItemSmoke.NewHost("Roads remember journal fixture");
            GameObject presentation = new GameObject("Roads remember native journal");
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>(); Bind(game, NewState(stage));
                Set(game, "showArmory", true); SetTab(game, "Journal");
                ArmoryOverlayView view = (ArmoryOverlayView)Invoke(game, "BuildArmoryOverlayView");
                Require(view.Rows.Count > 0 && view.Rows[0].Title == "Current journey", "current journey is the actual first journal row");
                if (stage >= 3) Require(view.Rows.Any(row => row.Title == HonestWeightQuestRules.Title && row.Subtitle.Contains("available")), "Maud's available favor is discoverable before acceptance");
                ArmoryOverlayScreen screen = presentation.AddComponent<ArmoryOverlayScreen>();
                screen.Bind(new ArmoryOverlayBindings { View = () => view }); screen.SetVisible(true); screen.Refresh();
                Render(directory, "journal-" + (stage == 0 ? "chapter-I" : stage == 3 ? "chapter-III-maud-lead" : stage == 6 ? "chapter-VI" : "epilogue"), size, Field<Canvas>(screen, "canvas"), () =>
                {
                    Layout(screen, new[] { typeof(bool), typeof(int), typeof(bool), typeof(int), typeof(float), typeof(float) },
                        view.Detail != null, view.Filters.Count, view.Detail?.ExtendedSummary ?? false, view.Detail?.Actions.Count ?? 0, (float)size.x, (float)size.y);
                    screen.ScrollRowIntoViewForTest(0);
                });
            }
            finally { UnityEngine.Object.DestroyImmediate(presentation); UnityEngine.Object.DestroyImmediate(host); }
        }

        private static void CaptureItem(string directory, Vector2Int size, string id, Texture2D atlas, bool pack)
        {
            GameObject host = RemembranceItemSmoke.NewHost("Roads remember item fixture");
            GameObject presentation = new GameObject("Roads remember native item");
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>(); Bind(game, NewState(3));
                Set(game, id == "counterweight_knife" ? "honestWeightItemAtlas" : "roadStockItemAtlas", atlas);
                InventoryItem item = SignatureItemCatalog.Create(id); Invoke(game, "AddInventoryItem", item);
                if (pack)
                {
                    Set(game, "showArmory", true); SetTab(game, "Pack"); Invoke(game, "SelectArmoryInventoryItem", item);
                    ArmoryOverlayView view = (ArmoryOverlayView)Invoke(game, "BuildArmoryOverlayView");
                    Require(view.Detail != null && view.Detail.IconTexture == atlas, "Pack exposes the actual named item art");
                    ArmoryOverlayScreen screen = presentation.AddComponent<ArmoryOverlayScreen>();
                    screen.Bind(new ArmoryOverlayBindings { View = () => view }); screen.SetVisible(true); screen.Refresh();
                    Render(directory, id + "-pack", size, Field<Canvas>(screen, "canvas"), () =>
                        Layout(screen, new[] { typeof(bool), typeof(int), typeof(bool), typeof(int), typeof(float), typeof(float) },
                            true, view.Filters.Count, view.Detail.ExtendedSummary, view.Detail.Actions.Count, (float)size.x, (float)size.y));
                }
                else
                {
                    Set(game, "lootPanelItem", item); Set(game, "lootPanelTitle", "Tessa's Road Stock"); Set(game, "lootPanelBody", item.DisplayName);
                    Set(game, "lootPanelRequiresDismissal", true); Set(game, "lootPanelEquipNote", "Added to the Pack; compare wearers before equipping.");
                    Set(game, "lootPanelTraitLine", Invoke(game, "ItemTraitLine", item));
                    LootPopupView view = (LootPopupView)Invoke(game, "BuildLootPopupView");
                    Require(view.IconTexture == atlas && view.CanReview, "loot exposes the actual art and review action");
                    LootPopupScreen screen = presentation.AddComponent<LootPopupScreen>();
                    screen.Bind(new LootPopupBindings { View = () => view }); screen.SetVisible(true); screen.Refresh();
                    Render(directory, id + "-loot", size, Field<Canvas>(screen, "canvas"), () => Layout(screen, new[] { typeof(float), typeof(float) }, (float)size.x, (float)size.y));
                }
            }
            finally { UnityEngine.Object.DestroyImmediate(presentation); UnityEngine.Object.DestroyImmediate(host); }
        }

        private static GameState NewState(int stage)
        {
            GameState state = new GameState
            {
                SaveVersion = VersionInfo.SaveVersion, ContentSetId = ContentSetCatalog.SewerSlice, Mode = GameMode.Explore,
                Depth = 1, StoryChapter = stage >= 5 ? 6 : stage >= 3 ? 3 : 1, Seed = 2350,
                Gold = 168, Supplies = 5, Elixirs = 2, ReducedMotion = true, SfxMuted = true, MusicMuted = true
            };
            state.StoryFlags.Add(StoryFlags.MidgaardGrandHearthDeparted);
            if (stage >= 1) state.StoryFlags.AddRange(new[] { StoryFlags.MidgaardKingMet, StoryFlags.MidgaardRatQuestGiven, StoryFlags.SewerContractAccepted,
                StoryFlags.SewerBrokenSluiceCleared, StoryFlags.SewerFoulRunoffCleared, StoryFlags.SewerCisternDenCleared,
                StoryFlags.SewerSafeRoomChoiceClaimed, StoryFlags.SewerSafeRoomFocusChosen, StoryFlags.SewerRewardClaimed,
                StoryFlags.MidgaardRatPeltArmorMade, StoryFlags.OldRoadTeaserUnlocked });
            if (stage >= 3) state.StoryFlags.AddRange(new[] { StoryFlags.KoboldAmbushSurvived, StoryFlags.KoboldCaveCleared, StoryFlags.KoboldKingDefeated });
            if (stage >= 5) state.StoryFlags.AddRange(new[] { StoryFlags.BoneRoadEntered, StoryFlags.BoneRoadWatchDefeated, StoryFlags.GloamRitualBroken, StoryFlags.GloamWardenDefeated,
                StoryFlags.RedGateWarningRecovered, StoryFlags.GlassAndAshFrontierSurveyed, StoryFlags.GlassAndAshExpeditionAccepted, StoryFlags.GlassAndAshEntered,
                StoryFlags.GlasswardAmbushDefeated, StoryFlags.GlassIndexRecovered, StoryFlags.EmberglassGateKeyRecovered, StoryFlags.GlassAndAshDebriefed,
                StoryFlags.RedGateAssaultAccepted, StoryFlags.RedGateEntered, StoryFlags.RedGateVanguardDefeated, StoryFlags.OssuaryRoadSealRecovered,
                StoryFlags.CrownroadMarshalDefeated, StoryFlags.MeteorCrownThresholdSurveyed, StoryFlags.RedGateDebriefed });
            if (stage >= 6) state.StoryFlags.Add(StoryFlags.MeteorCrownAssaultAccepted);
            if (stage >= 7) state.StoryFlags.AddRange(new[] { StoryFlags.MeteorCrownEntered, StoryFlags.MeteorCrownSentriesDefeated, StoryFlags.MeteorCrownHeartBroken, StoryFlags.MeteorCrownDefeated });
            foreach (StarterHeroDef hero in StarterPartyCatalog.All)
                state.Party.Add(new PartyMember { Id = "roads-capture-" + hero.ClassKey, Name = hero.Name, Race = hero.Race, ClassKey = hero.ClassKey,
                    Role = hero.Role, Stats = hero.Stats, Skills = hero.CreateSkills(), Spell = hero.Spell,
                    Level = stage >= 5 ? 14 : stage >= 3 ? 8 : 3, Hp = 38, MaxHp = 38, Mana = 20, MaxMana = 20, Range = hero.Range,
                    WeaponName = hero.Role == "bow" ? "plain longbow" : "plain steel sword", ArmorName = "plain traveling coat",
                    DamageMin = 4, DamageMax = 7, AttackSpeed = 8, SpriteColor = "#82a081" });
            return state;
        }

        private static void Bind(AshenHallsGame game, GameState state)
        {
            Set(game, "state", state); Set(game, "rng", new System.Random(state.Seed)); Set(game, "activeContentSet", ContentSetCatalog.SewerSlice);
            Set(game, "visualSmokeSaveBlocked", true); Set(game, "launchError", "Roads remember content fixture presentation held");
            state.Map = (MapData)Invoke(game, "GenerateMap", 1, state.Seed);
            Invoke(game, "EnsureWorldLandmarks"); Invoke(game, "PlacePlayerAtExplorationStart");
            state.ActiveStory = state.StoryFlags.Contains(StoryFlags.MeteorCrownAssaultAccepted) ? (string)Invoke(game, "MeteorCrownObjectiveForProgress")
                : state.StoryFlags.Contains(StoryFlags.RedGateDebriefed) ? (string)Invoke(game, "RedGateObjectiveForProgress")
                : state.StoryFlags.Contains(StoryFlags.KoboldKingDefeated) ? (string)Invoke(game, "BoneRoadObjectiveForProgress")
                : state.StoryFlags.Contains(StoryFlags.SewerRewardClaimed) ? "Chapter II: Kobold Smoke. Follow the Old Road east through Lanternless Cross toward Dusk Market."
                : (string)Invoke(game, "StoryObjectiveForDepth", 1);
        }
        private static void Choose(AshenHallsGame game, string id)
        {
            while (Field<int>(game, "dialoguePageIndex") < (Field<string[]>(game, "dialoguePages")?.Length ?? 0) - 1) Invoke(game, "AdvanceDialogue");
            DialogueScreenView view = (DialogueScreenView)Invoke(game, "BuildDialogueScreenView");
            Require(view.Choices.Length <= 4 && view.Choices.Any(choice => choice.Id == id && choice.Enabled), "visible enabled fixture choice " + id);
            typeof(AshenHallsGame).GetMethod("ChooseDialogueChoice", Private, null, new[] { typeof(string) }, null).Invoke(game, new object[] { id });
        }
        private static void SetTab(AshenHallsGame game, string name) => Set(game, "armoryTab", Convert.ToInt32(Enum.Parse(typeof(AshenHallsGame).GetNestedType("ArmoryTab", BindingFlags.NonPublic), name)));
        private static Texture2D LoadAtlas(string filename)
        {
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            Require(texture.LoadImage(File.ReadAllBytes(Path.Combine(Application.dataPath, "..", "Docs", "ArtReferences", filename))), filename + " decodes");
            texture.filterMode = FilterMode.Bilinear; return texture;
        }
        private static void Render(string directory, string name, Vector2Int size, Canvas canvas, Action layout) => typeof(UiFinishCapture).GetMethod("Render", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { directory, name, size, canvas, layout });
        private static void Layout(object screen, Type[] signature, params object[] args) => screen.GetType().GetMethod("ApplyLayout", Private, null, signature, null).Invoke(screen, args);
        private static T Field<T>(object owner, string name) => (T)owner.GetType().GetField(name, Private).GetValue(owner);
        private static object Invoke(object owner, string name, params object[] args) => RemembranceItemSmoke.Invoke(owner, name, args);
        private static void Set(object owner, string name, object value) => RemembranceItemSmoke.Set(owner, name, value);
        private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException("Roads remember content: " + message); }
    }
}
