using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class HonestWeightQuestSmoke
    {
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
        public static void Run()
        {
            try { RunOrThrow(); Debug.Log(VersionInfo.ProductName + " Honest Weight quest smoke passed."); EditorApplication.Exit(0); }
            catch (Exception exception) { Debug.LogError(VersionInfo.ProductName + " Honest Weight quest smoke failed: " + exception); EditorApplication.Exit(1); }
        }

        public static void RunOrThrow()
        {
            RuleTransitionsAreExplicit();
            MarketPresentationMatchesItsPrice();
            NativeContactChoicesRemainReachable();
            foreach (string disposition in new[] { "break", "retune" })
                foreach (string reward in new[] { "knife", "jack" })
                    ProductionQuest(disposition, reward, reward == "jack", disposition == "retune" && reward == "jack");
        }

        private static void NativeContactChoicesRemainReachable()
        {
            foreach (int stage in new[] { 0, 1, 2, 3 })
            using (Fixture fixture = new Fixture(stage == 3))
            {
                if (stage == 0) fixture.State.StoryFlags.Remove(StoryFlags.KoboldKingDefeated);
                if (stage >= 2) fixture.State.StoryFlags.Add(StoryFlags.HonestWeightAccepted);
                if (stage == 3) fixture.State.StoryFlags.Add(StoryFlags.HonestWeightRetuned);
                fixture.Call("ShowMaudConversation");
                fixture.ExpectChoices(stage == 0
                    ? new[] { "temporary", "permanent", "affinity", "personal" }
                    : new[] { "temporary", "permanent", "honestweight", "craft" });
                foreach (string service in new[] { "temporary", "permanent" })
                {
                    fixture.Choose(service);
                    Require(fixture.Get<bool>("maudPermanentEnchantment") == (service == "permanent"), "visible service selects the advertised binding duration");
                    fixture.ExpectChoices("0", "1", "2", "3");
                    fixture.Choose("0");
                    Require(fixture.NativeChoices().Length == 4, "each existing service reaches the four rune choices");
                    fixture.Call("ShowMaudConversation");
                }
                if (stage > 0)
                {
                    fixture.Choose("honestweight");
                    Require(fixture.Get<string>("dialogueTitle") == HonestWeightQuestRules.Title, "visible quest button opens the favor at every stage");
                    fixture.Choose("back");
                    fixture.Choose("craft");
                    fixture.ExpectChoices("affinity", "personal", "back");
                }
                foreach (string topic in new[] { "affinity", "personal" })
                {
                    fixture.Choose(topic);
                    Require(topic == "affinity" ? fixture.Get<string>("dialogueBody").Contains("Fire catches")
                        : fixture.Get<string>("dialogueBody") == TownConversationCatalog.PersonalStory(NpcId.Maud), "Maud's requested conversation topic opens");
                    fixture.ContinueResponse();
                    fixture.ExpectChoices(stage == 0
                        ? new[] { "temporary", "permanent", "affinity", "personal" }
                        : new[] { "affinity", "personal", "back" });
                }
                if (stage > 0)
                {
                    fixture.Choose("back");
                    fixture.ExpectChoices("temporary", "permanent", "honestweight", "craft");
                }
            }

            using (Fixture fixture = new Fixture(false))
            {
                foreach (bool accepted in new[] { false, true })
                {
                    if (accepted) fixture.State.StoryFlags.Add(StoryFlags.RemembranceAccepted);
                    fixture.Call("ShowEddaConversation");
                    fixture.ExpectChoices("rats", "failure", "escape", "remembrance");
                    fixture.Choose("remembrance");
                    Require(fixture.Get<string>("dialogueTitle") == RemembranceQuestRules.Title, "Edda's visible entry opens Remembrance");
                    fixture.Call("ShowSeraConversation");
                    fixture.ExpectChoices(accepted ? new[] { "circle", "order", "plague", "remembrance" } : new[] { "circle", "order", "plague" });
                    if (accepted) fixture.Choose("remembrance");
                    fixture.Call("ShowBrannConversation", new object[] { null });
                    fixture.ExpectChoices(accepted ? new[] { "watch", "roads", "sealed", "remembrance" } : new[] { "watch", "roads", "sealed" });
                    if (accepted) fixture.Choose("remembrance");
                }
            }
        }

        private static void RuleTransitionsAreExplicit()
        {
            List<string> flags = new List<string>();
            Require(!HonestWeightQuestRules.TryAccept(flags), "the favor opens after Varkh falls");
            flags.Add(StoryFlags.KoboldKingDefeated);
            Require(HonestWeightQuestRules.TryAccept(flags) && !HonestWeightQuestRules.TryAccept(flags), "acceptance is explicit and once only");
            Require(!HonestWeightQuestRules.TryInspect(flags, 2, HonestWeightQuestRules.MarketSite), "a different chapter's market is not the lamp location");
            Require(!HonestWeightQuestRules.TryInspect(flags, 3, "glass-lore-library"), "an unrelated site cannot supply evidence");
            Require(!HonestWeightQuestRules.TryResolve(flags, "retune") && !HonestWeightQuestRules.TryClaimReward(flags), "the contract cannot be settled or rewarded before both inscriptions");
            Require(HonestWeightQuestRules.TryInspect(flags, 3, HonestWeightQuestRules.QuarrySite), "quarry may be inspected first");
            Require(!HonestWeightQuestRules.TryResolve(flags, "break"), "one inscription is insufficient");
            Require(HonestWeightQuestRules.TryInspect(flags, 3, HonestWeightQuestRules.MarketSite), "the other inscription finishes the evidence");
            Require(!HonestWeightQuestRules.TryInspect(flags, 3, HonestWeightQuestRules.MarketSite), "evidence cannot be collected repeatedly");
            Require(!HonestWeightQuestRules.TryResolve(flags, "both"), "invalid settlement leaves the contract open");
            Require(HonestWeightQuestRules.TryResolve(flags, "retune") && !HonestWeightQuestRules.TryResolve(flags, "break"), "the chosen contract cannot be reversed by a stale callback");
            Require(HonestWeightQuestRules.MarketTradeCost(flags) == 8, "retuning supplies the documented continuing benefit");
            Require(HonestWeightQuestRules.TryClaimReward(flags) && !HonestWeightQuestRules.TryClaimReward(flags), "the independent item reward can be claimed only once");
        }

        private static void ProductionQuest(string disposition, string reward, bool previsited, bool postgame)
        {
            using (Fixture fixture = new Fixture(postgame))
            {
                string[] siteOrder = disposition == "retune"
                    ? new[] { HonestWeightQuestRules.QuarrySite, HonestWeightQuestRules.MarketSite }
                    : new[] { HonestWeightQuestRules.MarketSite, HonestWeightQuestRules.QuarrySite };
                if (previsited) foreach (string site in siteOrder) fixture.Visit(site);
                fixture.Call("ShowMaudConversation");
                fixture.Choose("honestweight");
                fixture.Choose("back");
                Require(!HonestWeightQuestRules.Accepted(fixture.State.StoryFlags), "leaving Maud's favor keeps it optional");
                fixture.Choose("honestweight");
                fixture.Choose("accept");
                fixture.Reload();
                Require(HonestWeightQuestRules.Accepted(fixture.State.StoryFlags) && !HonestWeightQuestRules.EvidenceReady(fixture.State.StoryFlags),
                    "acceptance survives loading and prior ordinary site visits never silently inspect the special inscriptions");
                Require(fixture.State.ActiveStory == Fixture.MainObjective && fixture.State.StoryChapter == (postgame ? 6 : 3),
                    "the optional favor preserves the campaign or postgame objective");
                fixture.Call("ResolveHonestWeightSiteChoice", "inspect");
                Require(!HonestWeightQuestRules.ContractRead(fixture.State.StoryFlags) && !HonestWeightQuestRules.CalibrationRead(fixture.State.StoryFlags), "a detached inspection callback cannot recover evidence");

                foreach (string site in siteOrder)
                {
                    fixture.Visit(site);
                    fixture.Choose("leave");
                    Require(!HonestWeightQuestRules.EvidenceReady(fixture.State.StoryFlags), "leaving the site dialog does not inspect it");
                    fixture.Visit(site);
                    bool claimed = WorldSiteInteractionRules.RewardClaimed(fixture.State.StoryFlags, 3, site);
                    int serviceGold = fixture.State.Gold;
                    int serviceSupplies = fixture.State.Supplies;
                    fixture.Choose("service");
                    Require(!fixture.Get<bool>("honestWeightServiceBypass"), "normal service bypass is scoped to one user action");
                    if (site == HonestWeightQuestRules.MarketSite)
                        Require(fixture.State.Gold == serviceGold - (claimed ? 12 : 0)
                            && fixture.State.Supplies == serviceSupplies + (claimed ? 1 : 2),
                            "the quest dialog preserves the normal market first cache and 12-gold repeat trade");
                    fixture.Visit(site);
                    int goldBefore = fixture.State.Gold;
                    int suppliesBefore = fixture.State.Supplies;
                    fixture.Choose("inspect");
                    Require(fixture.State.Gold == goldBefore && fixture.State.Supplies == suppliesBefore, "the advertised free inspection consumes no resources");
                    fixture.Reload();
                    Require(site == HonestWeightQuestRules.MarketSite ? HonestWeightQuestRules.ContractRead(fixture.State.StoryFlags)
                        : HonestWeightQuestRules.CalibrationRead(fixture.State.StoryFlags), "each explicitly copied inscription survives save/load");
                }

                fixture.Call("ShowMaudConversation");
                fixture.Choose("honestweight");
                fixture.Choose("review_" + disposition);
                fixture.Choose("back");
                Require(!HonestWeightQuestRules.Complete(fixture.State.StoryFlags), "settlement review can be declined without spending or deciding");
                fixture.Choose("review_" + disposition);
                int completedGoldBefore = fixture.State.Gold;
                int experienceBefore = TotalExperience(fixture.State.Party[0]);
                fixture.Choose("confirm_" + disposition);
                Require(HonestWeightQuestRules.Complete(fixture.State.StoryFlags)
                    && fixture.State.Gold == completedGoldBefore + (disposition == "break" ? 30 : 0)
                    && TotalExperience(fixture.State.Party[0]) == experienceBefore + HonestWeightQuestRules.ExperienceReward,
                    "the selected settlement grants its advertised gold and exactly 120 base XP once");
                fixture.Call("ShowHonestWeightRewardChoice");
                fixture.Choose("later");
                fixture.Reload();
                Require(HonestWeightQuestRules.Complete(fixture.State.StoryFlags) && !HonestWeightQuestRules.RewardClaimed(fixture.State.StoryFlags), "a completed contract can retain an unclaimed item after loading");
                fixture.Call("ResolveHonestWeightDisposition", "confirm_" + disposition);
                fixture.Call("ResolveHonestWeightDisposition", disposition == "break" ? "confirm_retune" : "confirm_break");
                Require(fixture.State.Gold == completedGoldBefore + (disposition == "break" ? 30 : 0)
                    && TotalExperience(fixture.State.Party[0]) == experienceBefore + HonestWeightQuestRules.ExperienceReward,
                    "duplicate and conflicting settlement callbacks cannot duplicate money or experience");
                fixture.Call("ShowMaudConversation");
                fixture.Choose("honestweight");
                fixture.Choose("reward");
                fixture.Choose(reward);
                string itemId = reward == "knife" ? "counterweight_knife" : "measurekeeper_jack";
                InventoryItem item = fixture.State.Inventory.Single(candidate => candidate.SignatureId == itemId);
                Require(!string.IsNullOrWhiteSpace(item.InstanceId) && string.IsNullOrEmpty(item.EquippedById), "either settlement can award the selected unique item without equipping it silently");
                string instance = item.InstanceId;
                fixture.Reload();
                fixture.Call("ResolveHonestWeightRewardChoice", reward == "knife" ? "jack" : "knife");
                fixture.Call("ResolveHonestWeightRewardChoice", reward);
                Require(fixture.State.Inventory.Count(candidate => candidate.SignatureId == "counterweight_knife" || candidate.SignatureId == "measurekeeper_jack") == 1
                    && fixture.State.Inventory.Single(candidate => candidate.SignatureId == itemId).InstanceId == instance,
                    "deferred reward persists and replay cannot grant another tool");
                foreach (int depth in new[] { 1, 3, 6 })
                {
                    fixture.SetDepth(depth);
                    string rewardFlag = WorldSiteInteractionRules.RewardFlag(depth, HonestWeightQuestRules.MarketSite);
                    if (!fixture.State.StoryFlags.Contains(rewardFlag)) fixture.State.StoryFlags.Add(rewardFlag);
                    int gold = fixture.State.Gold;
                    int supplies = fixture.State.Supplies;
                    fixture.Visit(HonestWeightQuestRules.MarketSite);
                    Require(fixture.State.Gold == gold - (disposition == "retune" ? 8 : 12) && fixture.State.Supplies == supplies + 1,
                        "the actual market charges the chosen contract price across chapter depths " + depth);
                }
                fixture.Call("ShowMaudConversation");
                Require(fixture.Get<string>("dialogueBody").Contains(disposition == "retune" ? "honest account" : "empty lamp"), "Maud's ordinary greeting acknowledges the chosen outcome");
                IReadOnlyList<ArmoryRowView> rows = (IReadOnlyList<ArmoryRowView>)fixture.Call("BuildArmoryJournalRows");
                Require(rows.Any(row => row.Title == HonestWeightQuestRules.Title && row.Subtitle.Contains("complete")), "the optional journal reports durable completion");
                Require(fixture.State.ActiveStory == Fixture.MainObjective, "inspection, rewards, and service discounts never rewrite the main objective");
            }
        }

        private static void MarketPresentationMatchesItsPrice()
        {
            Require(WorldSiteInteractionRules.TryGet(HonestWeightQuestRules.MarketSite, out WorldSiteInteractionProfile market), "market service exists");
            foreach (bool retuned in new[] { false, true })
            {
                List<string> flags = new List<string>();
                if (retuned) flags.Add(StoryFlags.HonestWeightRetuned);
                WorldSiteInteractionProfile visible = WorldSiteInteractionRules.ForStoryState(market, flags);
                string price = retuned ? "8 gold" : "12 gold";
                Require(visible.RepeatGoldCost == (retuned ? 8 : 12) && visible.RepeatSummary.Contains(price)
                    && visible.ClaimedStatus.Contains(price), "repeat summary and claimed status match the effective market price " + price);
                Require(visible.RewardSummary == market.RewardSummary && visible.RewardKind == market.RewardKind
                    && visible.RepeatSupplies == 1, "retuning preserves first-visit loot and supply quantity");
            }
            Require(market.RepeatGoldCost == 12 && market.RepeatSummary.Contains("12 gold"), "state-specific pricing never mutates the shared ordinary profile");
            Require(WorldSiteInteractionRules.TryGet(HonestWeightQuestRules.QuarrySite, out WorldSiteInteractionProfile quarry)
                && ReferenceEquals(quarry, WorldSiteInteractionRules.ForStoryState(quarry, new[] { StoryFlags.HonestWeightRetuned })),
                "retuning does not change unrelated services");
        }

        private static int TotalExperience(PartyMember member) => ProgressionRules.CumulativeExperienceToLevel(member.Level) + member.Experience;

        private sealed class Fixture : IDisposable
        {
            public const string MainObjective = "Preserve the player's current campaign objective.";
            private readonly GameObject host;
            private readonly AshenHallsGame game;
            private readonly string savePath;
            public GameState State => Get<GameState>("state");
            public Fixture(bool postgame)
            {
                host = new GameObject("Honest Weight quest smoke"); host.SetActive(false); host.hideFlags = HideFlags.HideAndDontSave;
                game = host.AddComponent<AshenHallsGame>();
                Set("state", new GameState
                {
                    SaveVersion = VersionInfo.SaveVersion, ContentSetId = ContentSetCatalog.SewerSlice, Mode = GameMode.Explore,
                    Depth = 3, Seed = 727, StoryChapter = postgame ? 6 : 3, Gold = 500, Supplies = 8, Elixirs = 2,
                    ActiveStory = MainObjective, ReducedMotion = true, SfxMuted = true, MusicMuted = true,
                    StoryFlags = new List<string> { StoryFlags.OldRoadTeaserUnlocked, StoryFlags.KoboldKingDefeated,
                        "campaign-milestone-xp:" + StoryFlags.KoboldKingDefeated },
                    Party = Enumerable.Range(0, 4).Select(i => new PartyMember
                    {
                        Id = "honest-weight-" + i, Name = "Witness " + i, Race = "ashling", ClassKey = "rogue", Role = "knife",
                        Level = 8, Stats = new Stats(14, 12, 18, 14), Hp = 60, MaxHp = 60, Mana = 20, MaxMana = 20,
                        Skills = new SkillSet().Normalize(), WeaponName = "knife", ArmorName = "leather jack"
                    }).ToList()
                });
                if (postgame) State.StoryFlags.Add(StoryFlags.MeteorCrownDefeated);
                Set("activeContentSet", ContentSetCatalog.SewerSlice); Set("rng", new System.Random(727));
                Set("visualSmokeSaveBlocked", true); Set("launchError", "Honest Weight fixture holds UI presentation");
                SetDepth(3);
                savePath = Path.Combine(Path.GetTempPath(), "ashen-honest-weight-" + Guid.NewGuid().ToString("N") + ".json");
            }
            public void SetDepth(int depth)
            {
                Call("CloseTransientOverlays"); State.Depth = depth; State.Map = (MapData)Call("GenerateMap", depth, State.Seed);
                Call("InvalidateExplorationController"); Call("EnsureWorldLandmarks"); Call("PlacePlayerAtExplorationStart");
            }
            public void Visit(string siteId)
            {
                Call("CloseTransientOverlays");
                MapObject site = State.Map.FindObjectById("regional-site:" + siteId);
                Require(site != null, "live map includes " + siteId);
                IReadOnlyList<Point> path = (IReadOnlyList<Point>)Call("FindLiveExplorePathToObject", site);
                Require(path != null && path.Count > 0, "quest and service site has a legal live approach: " + siteId);
                State.PlayerX = path[path.Count - 1].X; State.PlayerY = path[path.Count - 1].Y;
                Call("ResolveExploreObject", site);
            }
            public void Choose(string id)
            {
                DialogueChoiceView[] choices = NativeChoices();
                int index = Array.FindIndex(choices, choice => choice.Id == id && choice.Enabled);
                Require(index >= 0, "native conversation exposes " + id);
                Get<DialogueScreen>("dialogueScreen").InvokeChoiceForTest(index);
            }
            public DialogueChoiceView[] NativeChoices()
            {
                FinishPages();
                Call("EnsureDialogueScreen");
                DialogueScreen screen = Get<DialogueScreen>("dialogueScreen");
                screen.Refresh();
                DialogueScreenView view = (DialogueScreenView)Call("BuildDialogueScreenView");
                Require(view.Choices.Length <= 4 && screen.VisibleChoiceCountForTest == view.Choices.Length,
                    "all adapted conversation choices fit the native buttons: " + view.Speaker + " / " + view.Title);
                return view.Choices.Take(screen.VisibleChoiceCountForTest).ToArray();
            }
            public void ExpectChoices(params string[] ids)
            {
                Require(NativeChoices().Select(choice => choice.Id).SequenceEqual(ids), "native conversation includes the expected reachable choices: " + string.Join(",", ids));
            }
            public void ContinueResponse()
            {
                Require(NativeChoices().Length == 0, "a response presents Continue before returning to its topics");
                Get<DialogueScreen>("dialogueScreen").InvokeContinueForTest();
            }
            private void FinishPages()
            {
                string[] pages = Get<string[]>("dialoguePages");
                while (Get<int>("dialoguePageIndex") < (pages?.Length ?? 0) - 1) Call("AdvanceDialogue");
            }
            public void Reload()
            {
                string before = JsonUtility.ToJson(State); SaveService.SaveGameState(savePath, State);
                GameState loaded = SaveService.LoadGameState(savePath, out bool backup);
                Require(!backup && JsonUtility.ToJson(loaded) == before, "production SaveService preserves the exact optional quest state");
                Call("CloseTransientOverlays"); Set("state", loaded); Call("InvalidateExplorationController");
            }
            public object Call(string name, params object[] args)
            {
                MethodInfo method = typeof(AshenHallsGame).GetMethod(name, Private); Require(method != null, "runtime method exists: " + name);
                try { return method.Invoke(game, args); }
                catch (TargetInvocationException exception) { throw new InvalidOperationException("Honest Weight runtime call failed: " + name, exception.InnerException); }
            }
            public T Get<T>(string name) => (T)typeof(AshenHallsGame).GetField(name, Private).GetValue(game);
            public void Set(string name, object value) => typeof(AshenHallsGame).GetField(name, Private).SetValue(game, value);
            public void Dispose()
            {
                UnityEngine.Object.DestroyImmediate(host);
                foreach (string suffix in new[] { "", ".bak", ".tmp" }) if (File.Exists(savePath + suffix)) File.Delete(savePath + suffix);
            }
        }
        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("Honest Weight: " + message);
        }
    }
}
