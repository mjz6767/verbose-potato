using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class NativeRoadStockSmoke
    {
        public static void Run()
        {
            try { RunOrThrow(); Debug.Log(VersionInfo.ProductName + " native road stock smoke passed."); EditorApplication.Exit(0); }
            catch (Exception exception) { Debug.LogError(VersionInfo.ProductName + " native road stock smoke failed: " + exception); EditorApplication.Exit(1); }
        }

        public static void RunOrThrow()
        {
            NativeMenusPreserveServicesAndAdvice();
            FailedOrStaleQuotesNeverSpend();
            ObsoleteNativeCallbacksCannotAuthorizeAnotherQuote();
            foreach (bool martialParty in new[] { false, true })
            foreach (bool axeFirst in new[] { false, true })
                BothOrdersRemainIndependentAcrossSaving(martialParty, axeFirst);
        }

        private static void NativeMenusPreserveServicesAndAdvice()
        {
            using (Fixture f = new Fixture(false))
            {
                f.State.StoryFlags.Remove(StoryFlags.KoboldKingDefeated);
                f.Call("ShowTessaConversation", new object[] { null });
                f.Expect("buy", "forms", "range", "personal");
                string before = JsonUtility.ToJson(f.State);
                f.Call("ShowTessaRoadStockReview", SignatureItemCatalog.BridgehookSpearId);
                f.Call("ResolveTessaRoadStockReview", "confirm");
                Require(JsonUtility.ToJson(f.State) == before, "direct early callbacks cannot bypass the Varkh gate");
                f.State.StoryFlags.Add(StoryFlags.KoboldKingDefeated);
                foreach (bool starterBought in new[] { false, true })
                {
                    if (starterBought) f.State.StoryFlags.Add(StoryFlags.MidgaardBasicWeaponBought);
                    f.Call("ShowTessaConversation", new object[] { null });
                    f.Expect("buy", "roadstock", "advice", "personal");
                    Require(f.NativeChoices().First(choice => choice.Id == "buy").Enabled == !starterBought,
                        "the existing introductory purchase keeps its own receipt");
                    f.Choose("roadstock");
                    f.Expect(SignatureItemCatalog.BridgehookSpearId, SignatureItemCatalog.StonecleftAxeId, "back");
                    f.Choose("back");
                    f.Choose("advice");
                    f.Expect("forms", "range", "back");
                    foreach (string topic in new[] { "forms", "range" })
                    {
                        f.Choose(topic);
                        Require(!string.IsNullOrWhiteSpace(f.Get<string>("dialogueBody")), "the selected advice topic contains text");
                        f.Continue();
                        f.Expect("forms", "range", "back");
                    }
                    f.Choose("back");
                    f.Choose("personal");
                    f.Continue();
                    f.Expect("buy", "roadstock", "advice", "personal");
                }
            }
        }

        private static void FailedOrStaleQuotesNeverSpend()
        {
            using (Fixture f = new Fixture(false))
            {
                f.Call("ShowTessaRoadStock");
                string before = f.PurchaseFingerprint();
                f.Choose(SignatureItemCatalog.BridgehookSpearId);
                Require(f.Get<string>("dialogueBody").Contains("76 gold")
                    && f.Get<string>("dialogueBody").Contains("nothing is replaced automatically"),
                    "the real review discloses price and deliberate equipment handling");
                f.Choose("back");
                f.Call("ResolveTessaRoadStockReview", "confirm");
                Require(f.PurchaseFingerprint() == before, "browsing, declining and a stale declined callback change no campaign resources");

                f.State.Gold = 75;
                f.Choose(SignatureItemCatalog.StonecleftAxeId);
                DialogueChoiceView confirm = f.NativeChoices().First(choice => choice.Id == "confirm");
                Require(!confirm.Enabled && f.Get<string>("dialogueBody").Contains("1 more gold"),
                    "insufficient funds are disclosed and the native purchase button is disabled");
                before = f.PurchaseFingerprint();
                f.Call("ResolveTessaRoadStockReview", "confirm");
                Require(f.PurchaseFingerprint() == before, "a direct insufficient-funds callback cannot spend");

                f.State.Gold = 200;
                f.Call("ShowTessaRoadStockReview", SignatureItemCatalog.BridgehookSpearId);
                f.State.Gold = 75;
                before = f.PurchaseFingerprint();
                f.Choose("confirm");
                Require(f.PurchaseFingerprint() == before, "funds are rechecked when a formerly affordable native quote is confirmed");

                f.State.Gold = 200;
                f.Call("ShowTessaRoadStockReview", SignatureItemCatalog.BridgehookSpearId);
                f.Call("ShowTessaAdvice");
                before = f.PurchaseFingerprint();
                f.Call("ResolveTessaRoadStockReview", "confirm");
                Require(f.PurchaseFingerprint() == before, "a quote cannot be confirmed from another conversation");

                f.Call("ShowTessaRoadStockReview", SignatureItemCatalog.BridgehookSpearId);
                f.State.Mode = GameMode.Combat;
                before = f.PurchaseFingerprint();
                f.Call("ResolveTessaRoadStockReview", "confirm");
                Require(f.PurchaseFingerprint() == before, "combat rejects an outstanding town quote");
                f.State.Mode = GameMode.Explore;

                f.Call("ShowTessaRoadStockReview", SignatureItemCatalog.BridgehookSpearId);
                f.State.StoryFlags.Remove(StoryFlags.KoboldKingDefeated);
                before = f.PurchaseFingerprint();
                f.Call("ResolveTessaRoadStockReview", "confirm");
                Require(f.PurchaseFingerprint() == before, "eligibility is rechecked at confirmation");
                f.State.StoryFlags.Add(StoryFlags.KoboldKingDefeated);

                f.Call("ShowTessaRoadStockReview", SignatureItemCatalog.BridgehookSpearId);
                GameState replacement = JsonUtility.FromJson<GameState>(JsonUtility.ToJson(f.State));
                f.Set("state", replacement);
                before = f.PurchaseFingerprint();
                f.Call("ResolveTessaRoadStockReview", "confirm");
                Require(f.PurchaseFingerprint() == before, "a quote from another loaded campaign instance cannot spend");
            }
        }

        private static void ObsoleteNativeCallbacksCannotAuthorizeAnotherQuote()
        {
            foreach (bool axeFirst in new[] { false, true })
            using (Fixture f = new Fixture(false))
            {
                string first = axeFirst ? SignatureItemCatalog.StonecleftAxeId : SignatureItemCatalog.BridgehookSpearId;
                string second = axeFirst ? SignatureItemCatalog.BridgehookSpearId : SignatureItemCatalog.StonecleftAxeId;
                f.Call("ShowTessaRoadStock");
                f.Choose(first);
                Action<string> oldCallback = f.Get<Action<string>>("dialogueChoiceHandler");
                f.Call("ShowTessaRoadStockReview", second);
                f.NativeChoices();
                Action<string> currentCallback = f.Get<Action<string>>("dialogueChoiceHandler");
                string before = f.PurchaseFingerprint();
                string currentTitle = f.Get<string>("dialogueTitle");
                string currentBody = f.Get<string>("dialogueBody");
                foreach (string staleChoice in new[] { "confirm", "back" })
                {
                    oldCallback(staleChoice);
                    Require(f.PurchaseFingerprint() == before
                        && f.Get<string>("dialogueTitle") == currentTitle && f.Get<string>("dialogueBody") == currentBody
                        && ReferenceEquals(f.Get<Action<string>>("dialogueChoiceHandler"), currentCallback),
                        "a callback from the other offer cannot spend or dismiss the current same-title review");
                }
                f.Choose("confirm");
                Require(f.State.Gold == 124 && f.State.Inventory.Count(item => item.SignatureId == second) == 1
                    && !f.State.Inventory.Any(item => item.SignatureId == first)
                    && TessaRoadStockRules.Purchased(f.State.StoryFlags, TessaRoadStockRules.Find(second))
                    && !TessaRoadStockRules.Purchased(f.State.StoryFlags, TessaRoadStockRules.Find(first)),
                    "the fresh native callback still purchases exactly its reviewed offer after stale callbacks are ignored");

                f.Call("ShowTessaRoadStock");
                f.Choose(first);
                Action<string> cancelledCallback = f.Get<Action<string>>("dialogueChoiceHandler");
                f.Choose("back");
                foreach (bool openAdvice in new[] { false, true })
                {
                    if (openAdvice) f.Call("ShowTessaAdvice");
                    before = f.PurchaseFingerprint();
                    currentTitle = f.Get<string>("dialogueTitle");
                    currentBody = f.Get<string>("dialogueBody");
                    currentCallback = f.Get<Action<string>>("dialogueChoiceHandler");
                    foreach (string staleChoice in new[] { "confirm", "back" })
                    {
                        cancelledCallback(staleChoice);
                        Require(f.PurchaseFingerprint() == before
                            && f.Get<string>("dialogueTitle") == currentTitle && f.Get<string>("dialogueBody") == currentBody
                            && ReferenceEquals(f.Get<Action<string>>("dialogueChoiceHandler"), currentCallback),
                            "a cancelled review callback preserves current stock or advice without charging or recording a receipt");
                    }
                }
                f.Call("ShowTessaRoadStockReview", first);
                f.Choose("confirm");
                Require(f.State.Gold == 48 && TessaRoadStockRules.All.All(offer => TessaRoadStockRules.Purchased(f.State.StoryFlags, offer)),
                    "a fresh review can still purchase the cancelled offer at its exact price");
            }
        }

        private static void BothOrdersRemainIndependentAcrossSaving(bool martialParty, bool axeFirst)
        {
            using (Fixture f = new Fixture(martialParty))
            {
                string[] order = axeFirst
                    ? new[] { SignatureItemCatalog.StonecleftAxeId, SignatureItemCatalog.BridgehookSpearId }
                    : new[] { SignatureItemCatalog.BridgehookSpearId, SignatureItemCatalog.StonecleftAxeId };
                string equipment = f.PartyFingerprint();
                string initialInventory = f.ExistingInventoryFingerprint();
                string objective = f.State.ActiveStory;
                int initialGold = f.State.Gold;
                int boughtCount = 0;
                foreach (string id in order)
                {
                    f.Call("ShowTessaConversation", new object[] { null });
                    f.Choose("roadstock");
                    f.Choose(id);
                    f.Choose("confirm");
                    boughtCount++;
                    TessaRoadStockOffer offer = TessaRoadStockRules.Find(id);
                    InventoryItem item = f.State.Inventory.Single(entry => entry.SignatureId == id);
                    Require(f.State.Gold == initialGold - boughtCount * 76
                        && f.State.StoryFlags.Count(flag => flag == offer.ReceiptFlag) == 1,
                        "each native purchase charges its exact price and records one independent receipt");
                    Require(string.IsNullOrWhiteSpace(item.EquippedById) && !string.IsNullOrWhiteSpace(item.InstanceId)
                        && f.PartyFingerprint() == equipment && f.ExistingInventoryFingerprint() == initialInventory,
                        "the item enters the Pack without replacing, unequipping, or changing any party member");
                    Require(f.State.ActiveStory == objective && !f.State.StoryFlags.Contains(StoryFlags.MidgaardBasicWeaponBought),
                        "road stock neither changes the campaign objective nor requires the introductory purchase");
                    f.Continue();
                    LootPopupView loot = (LootPopupView)f.Call("BuildLootPopupView");
                    Require(loot != null && loot.CanReview, "the actual purchase reaches the native loot tradeoff route");
                    string bought = f.PurchaseFingerprint();
                    f.Call("ResolveTessaRoadStockReview", "confirm");
                    Require(f.PurchaseFingerprint() == bought, "a duplicate confirmation cannot charge or grant again");
                    f.Reload();
                    Require(f.State.Gold == initialGold - boughtCount * 76
                        && f.State.Inventory.Count(entry => entry.SignatureId == id) == 1
                        && TessaRoadStockRules.Purchased(f.State.StoryFlags, offer),
                        "gold, the unique inventory instance and receipt survive validated SaveService reload");
                    f.Call("ShowTessaRoadStock");
                    DialogueChoiceView[] choices = f.NativeChoices();
                    Require(!choices.First(choice => choice.Id == id).Enabled
                        && choices.First(choice => choice.Id == id).Label.Contains("sold"),
                        "purchased stock remains visibly sold after loading");
                    if (boughtCount == 1)
                        Require(choices.First(choice => choice.Id == order[1]).Enabled, "the other weapon remains available independently");
                }
                Require(f.State.Gold == 48 && TessaRoadStockRules.All.All(offer => TessaRoadStockRules.Purchased(f.State.StoryFlags, offer)),
                    "both purchase orders finish at the same price with both items owned");
                f.State.Depth = 1;
                f.State.StoryChapter = 6;
                f.State.StoryFlags.Add(StoryFlags.MeteorCrownDefeated);
                f.Call("ShowTessaRoadStock");
                Require(f.NativeChoices().Where(choice => choice.Id != "back").All(choice => !choice.Enabled),
                    "returning home after the campaign never restocks either weapon");
            }
        }

        private sealed class Fixture : IDisposable
        {
            private readonly GameObject host = RemembranceItemSmoke.NewHost("Native road stock fixture");
            private readonly AshenHallsGame game;
            private readonly string savePath = Path.Combine(Path.GetTempPath(), "ashen-native-road-stock-" + Guid.NewGuid().ToString("N") + ".json");
            public GameState State => Get<GameState>("state");

            public Fixture(bool martialParty)
            {
                game = host.AddComponent<AshenHallsGame>();
                List<PartyMember> party = StarterPartyCatalog.All.Select((hero, index) => new PartyMember
                {
                    Id = "road-shop-" + index, Name = hero.Name, Race = hero.Race, ClassKey = hero.ClassKey, Role = hero.Role,
                    Level = 8, Stats = new Stats(14, 14, 14, 14), Skills = new SkillSet().Normalize(),
                    Spell = hero.Spell, Hp = 80, MaxHp = 80, Mana = 40, MaxMana = 40,
                    WeaponName = "practice sword", WeaponDamageMin = 2, WeaponDamageMax = 5, WeaponAttackSpeed = 7,
                    ArmorName = "practice jack"
                }).ToList();
                if (martialParty)
                {
                    party[0].ClassKey = "paladin"; party[0].Role = "ward"; party[0].Spell = "mend";
                    party[1].ClassKey = "rogue"; party[1].Role = "knife"; party[1].Name = "Rook";
                }
                Set("state", new GameState
                {
                    SaveVersion = VersionInfo.SaveVersion, ContentSetId = ContentSetCatalog.SewerSlice,
                    Mode = GameMode.Explore, Depth = 1, Seed = 2350, StoryChapter = 3, Gold = 200, Supplies = 8, Elixirs = 2,
                    ActiveStory = "Continue along Bone Road.", ReducedMotion = true, SfxMuted = true, MusicMuted = true,
                    StoryFlags = new List<string> { StoryFlags.KoboldKingDefeated, "campaign-milestone-xp:" + StoryFlags.KoboldKingDefeated },
                    Party = party
                });
                Set("activeContentSet", ContentSetCatalog.SewerSlice);
                Set("rng", new System.Random(2350));
                Set("visualSmokeSaveBlocked", true);
                Set("launchError", "Road stock fixture holds presentation");
                foreach (PartyMember member in party)
                {
                    Call("RecalculateMember", member);
                    InventoryItem weapon = (InventoryItem)Call("MakeRoleItem", member.Role, true, true);
                    Call("AddInventoryItem", weapon);
                    object[] equip = { weapon, member, "" };
                    Require((bool)Call("EquipInventoryItemToMember", equip), "fixture equips legitimate starter weapons");
                }
                State.Map = (MapData)Call("GenerateMap", 1, State.Seed);
                Call("EnsureWorldLandmarks");
                Call("PlacePlayerAtExplorationStart");
            }

            public string PartyFingerprint() => string.Join("|", State.Party.Select(JsonUtility.ToJson));
            public string ExistingInventoryFingerprint() => string.Join("|", State.Inventory
                .Where(item => TessaRoadStockRules.Find(item.SignatureId) == null).Select(JsonUtility.ToJson));
            public string PurchaseFingerprint() => State.Gold + "|" + string.Join(",", State.StoryFlags)
                + "|" + PartyFingerprint() + "|" + string.Join("|", State.Inventory.Select(JsonUtility.ToJson));

            public DialogueChoiceView[] NativeChoices()
            {
                string[] pages = Get<string[]>("dialoguePages");
                while (Get<int>("dialoguePageIndex") < (pages?.Length ?? 0) - 1) Call("AdvanceDialogue");
                Call("EnsureDialogueScreen");
                DialogueScreen screen = Get<DialogueScreen>("dialogueScreen");
                screen.Refresh();
                DialogueScreenView view = (DialogueScreenView)Call("BuildDialogueScreenView");
                Require(view.Choices.Length <= 4 && screen.VisibleChoiceCountForTest == view.Choices.Length,
                    "every adapted choice is visible on the native screen");
                return view.Choices;
            }

            public void Choose(string id)
            {
                DialogueChoiceView[] choices = NativeChoices();
                int index = Array.FindIndex(choices, choice => choice.Id == id && choice.Enabled);
                Require(index >= 0, "native conversation exposes an enabled " + id + " choice");
                Get<DialogueScreen>("dialogueScreen").InvokeChoiceForTest(index);
            }

            public void Expect(params string[] ids) => Require(NativeChoices().Select(choice => choice.Id).SequenceEqual(ids),
                "native menu includes " + string.Join(",", ids));
            public void Continue()
            {
                Require(NativeChoices().Length == 0, "response has a native continue control");
                Get<DialogueScreen>("dialogueScreen").InvokeContinueForTest();
            }

            public void Reload()
            {
                Call("CloseTransientOverlays");
                Require(SaveService.TrySaveCampaignState(savePath, State, false, out string reason), "disposable native shop save: " + reason);
                GameState loaded = SaveService.LoadGameState(savePath,
                    candidate => SaveCandidateRules.IsLoadable(candidate, VersionInfo.SaveVersion), out bool backup);
                Require(!backup && loaded != null, "the primary shop save validates without fallback");
                Set("state", loaded);
                Call("InvalidateExplorationController");
            }

            public object Call(string name, params object[] args) => RemembranceItemSmoke.Invoke(game, name, args);
            public T Get<T>(string name) => (T)typeof(AshenHallsGame).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(game);
            public void Set(string name, object value) => RemembranceItemSmoke.Set(game, name, value);
            public void Dispose()
            {
                UnityEngine.Object.DestroyImmediate(host);
                foreach (string suffix in new[] { "", ".bak", ".tmp" }) if (File.Exists(savePath + suffix)) File.Delete(savePath + suffix);
            }
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("Native road stock: " + message);
        }
    }
}
