using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AshenHalls.Editor
{
    // Isolated presentation fixtures. No live save, game-host Awake, combat or reward transaction.
    public static class TownAndPackFollowupCapture
    {
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly StringBuilder Evidence = new StringBuilder();

        public static void Capture()
        {
            int exit = 1;
            Texture2D portraits = null;
            try
            {
                using (NavigationScope navigation = new NavigationScope())
                {
                    Evidence.Clear();
                    portraits = LoadAtlas(RuntimeArtManifest.NpcPortraitAtlas);
                    string directory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "QA", "town-and-pack-followups", DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff")));
                    Directory.CreateDirectory(directory);
                    int count = 0;
                    foreach (Vector2Int size in new[] { new Vector2Int(960, 600), new Vector2Int(1280, 720) })
                    {
                        foreach (bool privateMemorial in new[] { true, false }) count += CaptureEdda(directory, size, privateMemorial, portraits);
                        foreach (bool retuned in new[] { false, true }) count += CaptureMaud(directory, size, retuned, portraits);
                        foreach (string scenario in new[] { "displaced-shooter", "both-lose-reach", "shots-and-reach" })
                        { CaptureSwap(directory, size, scenario); count++; }
                    }
                    Require(count >= 26 && Directory.GetFiles(directory, "*.png").Length == count, "exact observed capture count matches all requested pages");
                    File.WriteAllText(Path.Combine(directory, "README.txt"),
                        "Town and Pack follow-ups: " + count + " native PNG captures.\n"
                        + "Window sizes: 960x600 and 1280x720. Every authored dialogue page is captured.\n"
                        + "Edda: private/public completed, reward-claimed submenu, then afterward response and real Back to topics callback.\n"
                        + "Maud: broken/retuned completed submenu with reward pending, same submenu with reward claimed, then work response and real Back to topics callback.\n"
                        + "Pack: actual linked equipment; Cairn longbow/Maer sword, Luma staff/Maer spear, Cairn sling/Luma staff. Each frame shows the real target picker before any swap.\n"
                        + "Fixtures supply completed quest history and linked plain weapons. They do not claim connected play, earned loot, reward delivery, or human visual review.\n"
                        + "Active production DialogueScreen and ArmoryOverlayScreen render their production adapters; game hosts remain inactive. Existing portrait atlas and native item presentation only; no new artwork.\n"
                        + "Owned canvases are explicitly released, and prior EventSystem selection/navigation state is restored. No live save is loaded or written.\n\n"
                        + Evidence);
                    Debug.Log("TOWN AND PACK FOLLOWUP CAPTURES PASSED: " + count + " images; " + directory);
                    exit = 0;
                }
            }
            catch (Exception exception) { Debug.LogError("TOWN AND PACK FOLLOWUP CAPTURES FAILED: " + exception); }
            finally { if (portraits != null) UnityEngine.Object.DestroyImmediate(portraits); }
            EditorApplication.Exit(exit);
        }

        private static int CaptureEdda(string directory, Vector2Int size, bool privateMemorial, Texture2D portraits)
        {
            using (Fixture f = new Fixture())
            {
                f.Set("npcPortraitAtlas", portraits);
                f.State.StoryFlags.AddRange(new[] { StoryFlags.RemembranceAccepted, StoryFlags.RemembranceLedgerRecovered,
                    StoryFlags.RemembranceNamesRead, privateMemorial ? StoryFlags.RemembrancePrivateChosen : StoryFlags.RemembrancePublicChosen,
                    StoryFlags.RemembranceDelivered, StoryFlags.RemembranceRewardClaimed, StoryFlags.RemembranceBowChosen });
                string before = JsonUtility.ToJson(f.State);
                f.Call("ShowEddaConversation"); Choose(f, "remembrance");
                string stem = "edda-" + (privateMemorial ? "private" : "public");
                int count = CaptureDialogue(directory, stem + "-submenu", size, f, portraits, "Edda", new[] { "afterward", "back" });
                Choose(f, "afterward");
                Require(f.Get<string>("dialogueBody").Contains(privateMemorial ? "Jory" : "Nella"), "Edda's response follows the completed disposition");
                count += CaptureDialogue(directory, stem + "-afterward", size, f, portraits, "Edda", Array.Empty<string>());
                f.Call("AdvanceDialogue"); AssertChoices(f, "afterward", "back");
                Require(before == JsonUtility.ToJson(f.State), "Edda's read-only follow-up does not change campaign state");
                return count;
            }
        }

        private static int CaptureMaud(string directory, Vector2Int size, bool retuned, Texture2D portraits)
        {
            using (Fixture f = new Fixture())
            {
                f.Set("npcPortraitAtlas", portraits);
                f.State.StoryFlags.AddRange(new[] { StoryFlags.HonestWeightAccepted, StoryFlags.HonestWeightContractRead,
                    StoryFlags.HonestWeightCalibrationRead, retuned ? StoryFlags.HonestWeightRetuned : StoryFlags.HonestWeightBroken });
                string stem = "maud-" + (retuned ? "retuned" : "broken");
                string pending = JsonUtility.ToJson(f.State);
                f.Call("ShowMaudConversation"); Choose(f, "honestweight");
                int count = CaptureDialogue(directory, stem + "-reward-pending", size, f, portraits, "Maud", new[] { "reward", "account", "work", "back" });
                Require(pending == JsonUtility.ToJson(f.State), "pending reward menu does not grant a reward");
                // Rendering a second saved-state fixture, not simulating collection of the tool.
                f.State.StoryFlags.Add(StoryFlags.HonestWeightRewardClaimed);
                string claimed = JsonUtility.ToJson(f.State);
                f.Call("ShowMaudConversation"); Choose(f, "honestweight");
                count += CaptureDialogue(directory, stem + "-reward-claimed", size, f, portraits, "Maud", new[] { "account", "work", "back" });
                Choose(f, "work");
                Require(f.Get<string>("dialogueBody").Contains(retuned ? "customer" : "younger self"), "Maud's work response follows the completed disposition");
                count += CaptureDialogue(directory, stem + "-work", size, f, portraits, "Maud", Array.Empty<string>());
                f.Call("AdvanceDialogue"); AssertChoices(f, "account", "work", "back");
                Require(claimed == JsonUtility.ToJson(f.State), "Maud's read-only follow-up does not change campaign state");
                return count;
            }
        }

        private static int CaptureDialogue(string directory, string name, Vector2Int size, Fixture f, Texture2D portraits, string speaker, string[] finalChoices)
        {
            GameObject presentation = new GameObject("Town follow-up native dialogue");
            try
            {
                DialogueScreen screen = presentation.AddComponent<DialogueScreen>();
                screen.Bind(new DialogueScreenBindings { View = () => (DialogueScreenView)f.Call("BuildDialogueScreenView"),
                    Advance = () => f.Call("AdvanceDialogue"), Choose = id => Choose(f, id) });
                int pages = f.Get<string[]>("dialoguePages").Length;
                Require(pages > 0 && f.Get<int>("dialoguePageIndex") == 0, "capture begins on the first authored page");
                for (int page = 0; page < pages; page++)
                {
                    DialogueScreenView view = (DialogueScreenView)f.Call("BuildDialogueScreenView");
                    Require(view.Speaker == speaker && view.PortraitTexture == portraits, "native dialogue uses the actual speaker and portrait");
                    if (page == pages - 1) AssertChoices(f, finalChoices);
                    else Require(view.Choices.Length == 0, "topic choices wait for the final authored page");
                    if (finalChoices.Length == 0 && page == pages - 1)
                        Require(view.ContinueLabel == "Back to topics", "response exposes its real return callback");
                    screen.SetVisible(true); screen.Refresh();
                    string frame = name + "-page" + (page + 1);
                    Render(directory, frame, size, Field<Canvas>(screen, "canvas"), () =>
                    {
                        Layout(screen, new[] { typeof(int), typeof(float), typeof(float) }, view.Choices.Length, (float)size.x, (float)size.y);
                        Invoke(screen, "UpdateBodyContentLayout");
                    });
                    Evidence.AppendLine(frame + " " + size.x + "x" + size.y + " | " + view.Body);
                    Evidence.AppendLine("  Choices: " + string.Join("; ", view.Choices.Select(choice => choice.Id + " / " + choice.Label)) + "; Continue: " + view.ContinueLabel);
                    if (page + 1 < pages) f.Call("AdvanceDialogue");
                }
                return pages;
            }
            finally { DestroyOwned(presentation); }
        }

        private static void CaptureSwap(string directory, Vector2Int size, string scenario)
        {
            using (Fixture f = new Fixture())
            {
                PartyMember maer = f.State.Party.Single(member => member.Name == "Maer");
                PartyMember cairn = f.State.Party.Single(member => member.Name == "Cairn");
                PartyMember luma = f.State.Party.Single(member => member.Name == "Luma");
                PartyMember owner, recipient;
                InventoryItem selected;
                string expected;
                if (scenario == "displaced-shooter")
                {
                    owner = cairn; recipient = maer;
                    selected = f.Equip(cairn, "longbow"); f.Equip(maer, "broadsword");
                    expected = "Review · Cairn: 5→1; shots disabled";
                }
                else if (scenario == "both-lose-reach")
                {
                    owner = luma; recipient = maer;
                    selected = f.Equip(luma, "ritual staff"); f.Equip(maer, "long spear");
                    expected = "Review · Range Luma 4→2; Maer 2→1";
                }
                else
                {
                    owner = cairn; recipient = luma;
                    selected = f.Equip(cairn, "sling"); f.Equip(luma, "ritual staff");
                    expected = "Review · Cai… 4→1 no shots; Luma 4→3";
                }
                int recipientIndex = f.State.Party.IndexOf(recipient);
                ArmoryDetailActionView actual = (ArmoryDetailActionView)f.Call("BuildInventoryTargetAction", selected, owner, recipientIndex, false);
                Require(actual.Detail == expected && actual.ButtonLabel == "Swap", "production swap preview names both relevant consequences: " + actual.Detail);
                f.Set("showArmory", true);
                f.Set("armoryTab", Convert.ToInt32(Enum.Parse(typeof(AshenHallsGame).GetNestedType("ArmoryTab", BindingFlags.NonPublic), "Pack")));
                f.Call("SelectArmoryInventoryItem", selected); f.Set("armoryInventoryTargetPickerOpen", true);
                string before = JsonUtility.ToJson(f.State);
                ArmoryOverlayView view = (ArmoryOverlayView)f.Call("BuildArmoryOverlayView");
                Require(view.Detail.Actions.Single(action => action.Key == recipientIndex).Detail == expected, "native target list includes the real recipient disclosure");
                GameObject presentation = new GameObject("Town follow-up native Pack");
                try
                {
                    ArmoryOverlayScreen screen = presentation.AddComponent<ArmoryOverlayScreen>();
                    screen.Bind(new ArmoryOverlayBindings { View = () => (ArmoryOverlayView)f.Call("BuildArmoryOverlayView") });
                    screen.SetVisible(true); screen.Refresh();
                    Canvas canvas = Field<Canvas>(screen, "canvas");
                    int actionIndex = view.Detail.Actions.ToList().FindIndex(action => action.Key == recipientIndex);
                    Render(directory, "pack-" + scenario, size, canvas, () =>
                    {
                        Layout(screen, new[] { typeof(bool), typeof(int), typeof(bool), typeof(int), typeof(float), typeof(float) },
                            true, view.Filters.Count, view.Detail.ExtendedSummary, view.Detail.Actions.Count, (float)size.x, (float)size.y);
                        Canvas.ForceUpdateCanvases();
                        UnityEngine.UI.Text comparison = canvas.GetComponentsInChildren<UnityEngine.UI.Text>(true)
                            .Single(text => text.name == "Comparison" && text.transform.parent.name == "Equip Target " + actionIndex);
                        Require(comparison.gameObject.activeInHierarchy && comparison.text == expected
                            && comparison.preferredWidth <= comparison.rectTransform.rect.width + .5f,
                            "entire recipient warning fits its visible native row at " + size + ": " + expected);
                    });
                    Evidence.AppendLine("pack-" + scenario + " " + size.x + "x" + size.y + " | Selected " + owner.Name + "'s " + selected.Form + "; target " + recipient.Name + ": " + expected);
                    Require(before == JsonUtility.ToJson(f.State), "capture previews the swap without committing it");
                }
                finally { DestroyOwned(presentation); }
            }
        }

        private sealed class Fixture : IDisposable
        {
            private readonly GameObject host = RemembranceItemSmoke.NewHost("Town and Pack follow-up inactive fixture");
            public readonly AshenHallsGame Game;
            public readonly GameState State;
            public Fixture()
            {
                Game = host.AddComponent<AshenHallsGame>();
                State = new GameState { SaveVersion = VersionInfo.SaveVersion, ContentSetId = ContentSetCatalog.SewerSlice,
                    Mode = GameMode.Explore, Depth = 1, StoryChapter = 3, Seed = 2361, Gold = 168, Supplies = 5, Elixirs = 2,
                    ReducedMotion = true, SfxMuted = true, MusicMuted = true };
                Set("state", State); Set("activeContentSet", ContentSetCatalog.SewerSlice); Set("rng", new System.Random(State.Seed));
                Set("visualSmokeSaveBlocked", true); Set("launchError", "Town and Pack fixture presentation held");
                State.Party = (List<PartyMember>)Call("MakeDefaultParty");
                State.StoryFlags.AddRange(new[] { StoryFlags.MidgaardGrandHearthDeparted, StoryFlags.MidgaardKingMet, StoryFlags.MidgaardRatQuestGiven,
                    StoryFlags.SewerContractAccepted, StoryFlags.SewerBrokenSluiceCleared, StoryFlags.SewerFoulRunoffCleared, StoryFlags.SewerCisternDenCleared,
                    StoryFlags.SewerSafeRoomChoiceClaimed, StoryFlags.SewerSafeRoomFocusChosen, StoryFlags.SewerRewardClaimed,
                    StoryFlags.MidgaardRatPeltArmorMade, StoryFlags.OldRoadTeaserUnlocked, StoryFlags.KoboldAmbushSurvived,
                    StoryFlags.KoboldCaveCleared, StoryFlags.KoboldKingDefeated });
                State.Map = (MapData)Call("GenerateMap", 1, State.Seed);
                Call("EnsureWorldLandmarks"); Call("PlacePlayerAtExplorationStart");
                Require(!host.activeInHierarchy, "game host remains inactive and never boots");
            }
            public InventoryItem Equip(PartyMember member, string form)
            {
                InventoryItem item = new InventoryItem { Slot = "weapon", Form = form, DisplayName = "plain " + form,
                    Material = "iron", Trait = "plain", DamageMin = 3, DamageMax = 7, AttackSpeed = 8, DamageType = "physical" };
                Call("AddInventoryItem", item);
                object[] args = { item, member, "" };
                Require((bool)Call("EquipInventoryItemToMember", args), "production linked equipment assignment: " + args[2]);
                Require(item.EquippedById == member.Id && member.WeaponItemId == item.InstanceId, "both ends of equipment ownership are real");
                return item;
            }
            public object Call(string name, params object[] args) => Invoke(Game, name, args);
            public T Get<T>(string name) => Field<T>(Game, name);
            public void Set(string name, object value) => RemembranceItemSmoke.Set(Game, name, value);
            public void Dispose() => DestroyOwned(host);
        }

        private sealed class NavigationScope : IDisposable
        {
            private readonly EventSystem previous = EventSystem.current;
            private readonly EventSystem[] systems = UnityEngine.Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            private readonly Dictionary<EventSystem, bool> enabled = new Dictionary<EventSystem, bool>();
            private readonly Dictionary<EventSystem, bool> navigation = new Dictionary<EventSystem, bool>();
            private readonly Dictionary<EventSystem, GameObject> selected = new Dictionary<EventSystem, GameObject>();
            private readonly FieldInfo ensured = typeof(UiRuntime).GetField("ensuredEventSystem", BindingFlags.Static | BindingFlags.NonPublic);
            private readonly FieldInfo suppression = typeof(UiRuntime).GetField("textInputSuppressedThroughFrame", BindingFlags.Static | BindingFlags.NonPublic);
            private readonly object oldEnsured, oldSuppression;
            private readonly GameObject host;
            private readonly EventSystem own;
            private readonly bool registered;
            public NavigationScope()
            {
                oldEnsured = ensured.GetValue(null); oldSuppression = suppression.GetValue(null);
                foreach (EventSystem system in systems)
                { enabled[system] = system.enabled; navigation[system] = system.sendNavigationEvents; selected[system] = system.currentSelectedGameObject; system.enabled = false; }
                host = new GameObject("Town and Pack capture EventSystem");
                own = host.AddComponent<EventSystem>(); host.AddComponent<StandaloneInputModule>();
                // The setter succeeds only when Unity has already registered the system.
                EventSystem.current = own;
                if (EventSystem.current != own) { Invoke(own, "OnEnable"); registered = true; }
                EventSystem.current = own;
                Require(EventSystem.current == own, "capture owns its isolated navigation system");
            }
            public void Dispose()
            {
                if (own != null && !own.alreadySelecting) own.SetSelectedGameObject(null);
                if (registered && own != null) Invoke(own, "OnDisable");
                if (host != null) UnityEngine.Object.DestroyImmediate(host);
                foreach (EventSystem system in systems)
                {
                    if (system == null) continue;
                    system.enabled = enabled[system]; system.sendNavigationEvents = navigation[system];
                    if (!system.alreadySelecting && system.currentSelectedGameObject != selected[system]) system.SetSelectedGameObject(selected[system]);
                }
                if (previous != null) EventSystem.current = previous;
                ensured.SetValue(null, oldEnsured); suppression.SetValue(null, oldSuppression);
            }
        }

        private static void AssertChoices(Fixture f, params string[] expected)
        {
            DialogueScreenView view = (DialogueScreenView)f.Call("BuildDialogueScreenView");
            Require(view.Choices.Length <= 4 && view.Choices.All(choice => choice.Enabled)
                && view.Choices.Select(choice => choice.Id).SequenceEqual(expected), "completed menu matches its visible native choices");
        }
        private static void Choose(Fixture f, string id)
        {
            while (f.Get<int>("dialoguePageIndex") < f.Get<string[]>("dialoguePages").Length - 1) f.Call("AdvanceDialogue");
            DialogueScreenView view = (DialogueScreenView)f.Call("BuildDialogueScreenView");
            Require(view.Choices.Length <= 4 && view.Choices.Any(choice => choice.Id == id && choice.Enabled), "enabled native choice " + id);
            typeof(AshenHallsGame).GetMethod("ChooseDialogueChoice", Private, null, new[] { typeof(string) }, null).Invoke(f.Game, new object[] { id });
        }
        private static void DestroyOwned(GameObject owner)
        {
            if (owner == null) return;
            // Inactive hosts never receive OnDestroy; release only roots registered to this fixture.
            foreach (MonoBehaviour component in owner.GetComponentsInChildren<MonoBehaviour>(true))
                if (component != null && component.GetType().Name == "UiOwnedCanvasLifetime") Invoke(component, "OnDestroy");
            UnityEngine.Object.DestroyImmediate(owner);
        }
        private static Texture2D LoadAtlas(string filename)
        {
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            Require(texture.LoadImage(File.ReadAllBytes(Path.Combine(Application.dataPath, "..", "Docs", "ArtReferences", filename))), filename + " decodes");
            texture.filterMode = FilterMode.Bilinear; return texture;
        }
        private static void Render(string directory, string name, Vector2Int size, Canvas canvas, Action layout)
        {
            RenderMode mode = canvas.renderMode; Camera camera = canvas.worldCamera; float distance = canvas.planeDistance;
            Dictionary<GameObject, int> layers = canvas.GetComponentsInChildren<Transform>(true).ToDictionary(child => child.gameObject, child => child.gameObject.layer);
            try { typeof(UiFinishCapture).GetMethod("Render", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { directory, name, size, canvas, layout }); }
            finally
            {
                canvas.renderMode = mode; canvas.worldCamera = camera; canvas.planeDistance = distance;
                foreach (KeyValuePair<GameObject, int> entry in layers) if (entry.Key != null) entry.Key.layer = entry.Value;
            }
        }
        private static void Layout(object screen, Type[] signature, params object[] args) => screen.GetType().GetMethod("ApplyLayout", Private, null, signature, null).Invoke(screen, args);
        private static T Field<T>(object owner, string name) => (T)owner.GetType().GetField(name, Private).GetValue(owner);
        private static object Invoke(object owner, string name, params object[] args) => RemembranceItemSmoke.Invoke(owner, name, args);
        private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException("Town and Pack follow-up capture: " + message); }
    }
}
