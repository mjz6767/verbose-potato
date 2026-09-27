using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AshenHalls.Editor
{
    public static class TownStoryFollowupSmoke
    {
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

        public static void Run()
        {
            try { RunOrThrow(); Debug.Log(VersionInfo.ProductName + " town story follow-up smoke passed."); EditorApplication.Exit(0); }
            catch (Exception exception) { Debug.LogError(VersionInfo.ProductName + " town story follow-up smoke failed: " + exception); EditorApplication.Exit(1); }
        }

        public static void RunOrThrow()
        {
            EventSystem previous = EventSystem.current;
            EventSystem[] existing = UnityEngine.Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            FieldInfo ensuredField = typeof(UiRuntime).GetField("ensuredEventSystem", BindingFlags.Static | BindingFlags.NonPublic);
            object previouslyEnsured = ensuredField.GetValue(null);
            EventSystem system = null;
            bool registered = false;
            try
            {
                system = UiRuntime.EnsureEventSystemReady();
                if (EventSystem.current == null)
                {
                    typeof(EventSystem).GetMethod("OnEnable", Private).Invoke(system, null);
                    registered = true;
                }
                EventSystem.current = system;
                IncompleteFavorsHideAndRejectFollowups();
                foreach (string disposition in new[] { "private", "public" })
                    foreach (bool claimed in new[] { false, true }) EddaFollowupPreservesTheFavor(disposition, claimed);
                foreach (string disposition in new[] { "break", "retune" })
                    foreach (bool claimed in new[] { false, true }) MaudFollowupPreservesTheFavor(disposition, claimed);
                CompletedFollowupsRejectNonExplorationDispatch();
            }
            finally
            {
                if (registered && system != null) typeof(EventSystem).GetMethod("OnDisable", Private).Invoke(system, null);
                if (system != null && !existing.Contains(system)) UnityEngine.Object.DestroyImmediate(system.gameObject);
                if (previous != null) EventSystem.current = previous;
                ensuredField.SetValue(null, previouslyEnsured);
                Require(LiveIds<EventSystem>().SequenceEqual(existing.Select(value => value.GetInstanceID()).OrderBy(value => value))
                    && EventSystem.current == previous, "smoke restores its borrowed event system and current input owner");
            }
        }

        private static void IncompleteFavorsHideAndRejectFollowups()
        {
            for (int stage = 0; stage < 3; stage++)
            using (Fixture fixture = new Fixture())
            {
                if (stage > 0) Require(RemembranceQuestRules.TryAccept(fixture.State.StoryFlags), "prepare accepted remembrance");
                if (stage == 2)
                {
                    fixture.State.StoryFlags.Add(StoryFlags.SewerFoulRunoffCleared);
                    Require(RemembranceQuestRules.TryRecover(fixture.State.StoryFlags)
                        && RemembranceQuestRules.TryReadNames(fixture.State.StoryFlags)
                        && RemembranceQuestRules.TryChooseDisposition(fixture.State.StoryFlags, "private"), "prepare undelivered memorial");
                }
                Snapshot before = fixture.Snapshot();
                fixture.OpenEddaFavor();
                Require(!fixture.NativeChoices().Any(choice => choice.Id == "afterward"), "Edda follow-up is hidden until delivery, stage " + stage);
                fixture.AssertRejected("afterward", "ResolveEddaRemembranceAction");
                fixture.AssertUnchanged(before, "incomplete Edda navigation and direct dispatch");

                if (stage > 0) Require(HonestWeightQuestRules.TryAccept(fixture.State.StoryFlags), "prepare accepted lamp investigation");
                if (stage == 2)
                    Require(HonestWeightQuestRules.TryInspect(fixture.State.StoryFlags, 3, HonestWeightQuestRules.MarketSite)
                        && HonestWeightQuestRules.TryInspect(fixture.State.StoryFlags, 3, HonestWeightQuestRules.QuarrySite), "prepare unresolved lamp with both clues");
                before = fixture.Snapshot();
                fixture.OpenMaudFavor();
                Require(!fixture.NativeChoices().Any(choice => choice.Id == "work"), "Maud follow-up is hidden until settlement, stage " + stage);
                fixture.AssertRejected("work", "ResolveHonestWeightConversation");
                fixture.AssertUnchanged(before, "incomplete Maud navigation and direct dispatch");
            }
        }

        private static void EddaFollowupPreservesTheFavor(string disposition, bool claimed)
        {
            using (Fixture fixture = new Fixture())
            {
                fixture.CompleteRemembrance(disposition, claimed);
                fixture.Reload();
                Snapshot before = fixture.Snapshot();
                fixture.OpenEddaFavor();
                fixture.ExpectChoices("afterward", "back");
                string completedBody = fixture.Body;
                for (int reading = 0; reading < 3; reading++)
                {
                    fixture.Choose("afterward");
                    fixture.ExpectResponse("Edda", RemembranceQuestRules.Title,
                        disposition == "private" ? "wet socks" : "young guard");
                    fixture.ContinueResponse();
                    fixture.ExpectChoices("afterward", "back");
                    Require(fixture.Body == completedBody, "Edda response returns to the same completed favor");
                    fixture.AssertUnchanged(before, "repeated Edda response after save/load");
                }
                fixture.Choose("back");
                fixture.ExpectChoices("rats", "failure", "escape", "remembrance");

                // The personal topic never takes ownership of the existing deferred keepsake route.
                fixture.Call("ShowSeraConversation");
                fixture.Choose("remembrance");
                if (claimed)
                {
                    fixture.ExpectChoices("back");
                    fixture.Choose("back");
                }
                else
                {
                    fixture.ExpectChoices("bow", "wrap", "later");
                    fixture.Choose("later");
                }
                Require(fixture.NativeChoices().Any(choice => choice.Id == "remembrance"), "Sera still returns to her ordinary topics");
                fixture.AssertUnchanged(before, "Edda follow-up preserves pending or claimed rewards and other contact access");
            }
        }

        private static void MaudFollowupPreservesTheFavor(string disposition, bool claimed)
        {
            using (Fixture fixture = new Fixture())
            {
                fixture.CompleteHonestWeight(disposition, claimed);
                fixture.Reload();
                Snapshot before = fixture.Snapshot();
                string[] expected = claimed ? new[] { "account", "work", "back" } : new[] { "reward", "account", "work", "back" };
                fixture.OpenMaudFavor();
                fixture.ExpectChoices(expected);
                string completedBody = fixture.Body;
                for (int reading = 0; reading < 3; reading++)
                {
                    fixture.Choose("work");
                    fixture.ExpectResponse("Maud", HonestWeightQuestRules.Title,
                        disposition == "retune" ? "customer can read" : "younger self");
                    fixture.ContinueResponse();
                    fixture.ExpectChoices(expected);
                    Require(fixture.Body == completedBody, "Maud response returns to the same completed favor");
                    fixture.AssertUnchanged(before, "repeated Maud response after save/load");
                }
                fixture.Choose("account");
                fixture.ExpectResponse("Maud", HonestWeightQuestRules.Title,
                    disposition == "retune" ? "Eight gold" : "ordinary weights");
                fixture.ContinueResponse();
                fixture.ExpectChoices(expected);
                if (!claimed)
                {
                    fixture.Choose("reward");
                    fixture.ExpectChoices("knife", "jack", "later");
                    fixture.Choose("later");
                    fixture.ExpectChoices("temporary", "permanent", "honestweight", "craft");
                    fixture.Choose("honestweight");
                    fixture.ExpectChoices(expected);
                }
                fixture.Choose("back");
                fixture.ExpectChoices("temporary", "permanent", "honestweight", "craft");
                fixture.AssertUnchanged(before, "Maud follow-up preserves account, reward and service navigation");
            }
        }

        private static void CompletedFollowupsRejectNonExplorationDispatch()
        {
            using (Fixture fixture = new Fixture())
            {
                fixture.CompleteRemembrance("public", false);
                fixture.CompleteHonestWeight("retune", false);
                fixture.OpenEddaFavor();
                fixture.State.Mode = GameMode.Combat;
                Snapshot before = fixture.Snapshot();
                string body = fixture.Body;
                fixture.Call("ResolveEddaRemembranceAction", "afterward");
                fixture.Call("ResolveHonestWeightConversation", "work");
                Require(fixture.Body == body && !fixture.Get<bool>("dialogueShowingResponse"), "completed follow-ups cannot dispatch outside exploration");
                fixture.AssertUnchanged(before, "non-exploration follow-up callbacks");
            }
        }

        private sealed class Fixture : IDisposable
        {
            private readonly GameObject host;
            private readonly AshenHallsGame game;
            private readonly CountingRandom random = new CountingRandom(92737);
            private readonly string savePath = Path.Combine(Path.GetTempPath(), "ashen-town-followup-" + Guid.NewGuid().ToString("N") + ".json");
            private readonly int[] canvasIds = LiveIds<Canvas>();
            private readonly int[] ornamentIds = LiveIds<UiOrnament>();
            public GameState State => Get<GameState>("state");
            public string Body => Get<string>("dialogueBody");

            public Fixture()
            {
                host = new GameObject("Town story follow-up smoke");
                host.SetActive(false);
                host.hideFlags = HideFlags.HideAndDontSave;
                game = host.AddComponent<AshenHallsGame>();
                Set("state", new GameState
                {
                    SaveVersion = VersionInfo.SaveVersion, ContentSetId = ContentSetCatalog.SewerSlice,
                    Mode = GameMode.Explore, Depth = 1, StoryChapter = 3, Seed = 92737,
                    Gold = 143, Supplies = 9, Elixirs = 3, ActiveStory = "Continue the company campaign.",
                    ReducedMotion = true, SfxMuted = true, MusicMuted = true,
                    Map = new MapData(), Combat = new CombatState(),
                    StoryFlags = new List<string> { StoryFlags.KoboldKingDefeated },
                    Party = new List<PartyMember> { new PartyMember {
                        Id = "followup-witness", Name = "Witness", Race = "human", ClassKey = "ranger", Role = "bow",
                        Level = 8, Experience = 19, SkillPoints = 3, StatPoints = 2, Stats = new Stats(12, 10, 16, 12),
                        Skills = new SkillSet().Normalize(), Hp = 37, MaxHp = 50, Mana = 11, MaxMana = 20,
                        WeaponName = "shortbow", ArmorName = "leather jack" } }
                });
                Set("activeContentSet", ContentSetCatalog.SewerSlice);
                Set("rng", random);
                Set("visualSmokeSaveBlocked", true);
                Set("launchError", "Town story fixture holds automatic UI presentation");
            }

            public void CompleteRemembrance(string disposition, bool claimed)
            {
                State.StoryFlags.Add(StoryFlags.SewerFoulRunoffCleared);
                Require(RemembranceQuestRules.TryAccept(State.StoryFlags) && RemembranceQuestRules.TryRecover(State.StoryFlags)
                    && RemembranceQuestRules.TryReadNames(State.StoryFlags) && RemembranceQuestRules.TryChooseDisposition(State.StoryFlags, disposition)
                    && RemembranceQuestRules.TryDeliver(State.StoryFlags, disposition == "private" ? ObjectType.NoviceHealer : ObjectType.GateCaptain),
                    "prepare lawful completed remembrance flags");
                if (!claimed) return;
                Require(RemembranceQuestRules.TryClaimReward(State.StoryFlags, "bow"), "prepare previously claimed keepsake");
                InventoryItem reward = SignatureItemCatalog.Create(SignatureItemCatalog.TravelersKnotId);
                reward.InstanceId = "followup-remembrance-reward";
                State.Inventory.Add(reward);
                State.Party[0].Experience += RemembranceQuestRules.ExperienceReward;
            }

            public void CompleteHonestWeight(string disposition, bool claimed)
            {
                Require(HonestWeightQuestRules.TryAccept(State.StoryFlags)
                    && HonestWeightQuestRules.TryInspect(State.StoryFlags, 3, HonestWeightQuestRules.MarketSite)
                    && HonestWeightQuestRules.TryInspect(State.StoryFlags, 3, HonestWeightQuestRules.QuarrySite)
                    && HonestWeightQuestRules.TryResolve(State.StoryFlags, disposition), "prepare lawful completed lamp flags");
                State.Party[0].Experience += HonestWeightQuestRules.ExperienceReward;
                if (disposition == "break") State.Gold += HonestWeightQuestRules.BrokenGoldReward;
                if (!claimed) return;
                Require(HonestWeightQuestRules.TryClaimReward(State.StoryFlags), "prepare previously claimed workshop tool");
                InventoryItem reward = SignatureItemCatalog.Create(SignatureItemCatalog.CounterweightKnifeId);
                reward.InstanceId = "followup-honest-weight-reward";
                State.Inventory.Add(reward);
            }

            public void OpenEddaFavor() { Call("ShowEddaConversation"); Choose("remembrance"); }
            public void OpenMaudFavor() { Call("ShowMaudConversation"); Choose("honestweight"); }

            public DialogueChoiceView[] NativeChoices()
            {
                Call("EnsureDialogueScreen");
                DialogueScreen screen = Get<DialogueScreen>("dialogueScreen");
                while (Get<int>("dialoguePageIndex") < (Get<string[]>("dialoguePages")?.Length ?? 0) - 1)
                {
                    screen.Refresh();
                    screen.InvokeContinueForTest();
                }
                screen.Refresh();
                DialogueScreenView view = (DialogueScreenView)Call("BuildDialogueScreenView");
                Require(view.Choices.Length <= 4 && screen.VisibleChoiceCountForTest == view.Choices.Length,
                    "all production choices fit native buttons: " + view.Speaker + " / " + view.Title);
                return view.Choices;
            }

            public void Choose(string id)
            {
                int index = Array.FindIndex(NativeChoices(), choice => choice.Id == id && choice.Enabled);
                Require(index >= 0, "native dialogue exposes enabled choice " + id);
                Get<DialogueScreen>("dialogueScreen").InvokeChoiceForTest(index);
            }

            public void ExpectChoices(params string[] ids) => Require(NativeChoices().Select(choice => choice.Id).SequenceEqual(ids),
                "native topic order and access: " + string.Join(", ", ids));

            public void ExpectResponse(string speaker, string title, string distinguishingText)
            {
                Require(Get<bool>("dialogueShowingResponse") && Get<string>("dialogueSpeaker") == speaker
                    && Get<string>("dialogueTitle") == title && Body.Contains(distinguishingText), "response follows the saved outcome and speaker");
                Require(NativeChoices().Length == 0 && Get<Action>("dialogueReturnToTopics") != null, "response exposes its production return callback");
            }

            public void ContinueResponse()
            {
                Require(NativeChoices().Length == 0 && Get<bool>("dialogueShowingResponse"), "native return starts on the last response page");
                Get<DialogueScreen>("dialogueScreen").InvokeContinueForTest();
                Require(!Get<bool>("dialogueShowingResponse"), "native Continue executes the return callback");
            }

            public void AssertRejected(string token, string resolver)
            {
                string body = Body;
                string[] choices = NativeChoices().Select(choice => choice.Id).ToArray();
                typeof(AshenHallsGame).GetMethod("ChooseDialogueChoice", Private, null, new[] { typeof(string) }, null)
                    .Invoke(game, new object[] { token });
                Call(resolver, token);
                Require(Body == body && !Get<bool>("dialogueShowingResponse")
                    && NativeChoices().Select(choice => choice.Id).SequenceEqual(choices), "unavailable follow-up rejects both normal and direct dispatch");
            }

            public Snapshot Snapshot() => new Snapshot { StateJson = JsonUtility.ToJson(State), RandomCalls = random.Calls, UnityRandom = UnityEngine.Random.state };
            public void AssertUnchanged(Snapshot before, string reason)
            {
                Require(JsonUtility.ToJson(State) == before.StateJson, reason + " preserves every serialized campaign field");
                Require(ReferenceEquals(Get<System.Random>("rng"), random) && random.Calls == before.RandomCalls
                    && UnityEngine.Random.state.Equals(before.UnityRandom), reason + " preserves gameplay and Unity random state");
            }

            public void Reload()
            {
                string before = JsonUtility.ToJson(State);
                SaveService.SaveGameState(savePath, State);
                GameState loaded = SaveService.LoadGameState(savePath, out bool backup);
                Require(!backup && JsonUtility.ToJson(loaded) == before, "actual SaveService preserves the completed choice and reward state");
                Call("CloseTransientOverlays");
                Set("state", loaded);
            }

            public object Call(string name, params object[] args)
            {
                MethodInfo method = typeof(AshenHallsGame).GetMethod(name, Private);
                Require(method != null, "production method exists: " + name);
                try { return method.Invoke(game, args); }
                catch (TargetInvocationException exception) { throw new InvalidOperationException("Town story runtime call failed: " + name, exception.InnerException); }
            }
            public T Get<T>(string name) => (T)typeof(AshenHallsGame).GetField(name, Private).GetValue(game);
            private void Set(string name, object value) => typeof(AshenHallsGame).GetField(name, Private).SetValue(game, value);

            public void Dispose()
            {
                // An inactive fixture's bound screen can own a separate active root canvas without ever receiving OnDestroy.
                foreach (Component component in host.GetComponentsInChildren<Component>(true))
                    if (component != null && component.GetType().Name == "UiOwnedCanvasLifetime")
                        component.GetType().GetMethod("OnDestroy", Private).Invoke(component, null);
                UnityEngine.Object.DestroyImmediate(host);
                foreach (string suffix in new[] { "", ".bak", ".tmp" }) if (File.Exists(savePath + suffix)) File.Delete(savePath + suffix);
                Require(LiveIds<Canvas>().SequenceEqual(canvasIds) && LiveIds<UiOrnament>().SequenceEqual(ornamentIds),
                    "fixture restores exactly its original canvas and ornament owners");
            }
        }

        private sealed class Snapshot
        {
            public string StateJson;
            public int RandomCalls;
            public UnityEngine.Random.State UnityRandom;
        }

        private sealed class CountingRandom : System.Random
        {
            public int Calls { get; private set; }
            public CountingRandom(int seed) : base(seed) { }
            public override int Next() { Calls++; return base.Next(); }
            public override int Next(int maxValue) { Calls++; return base.Next(maxValue); }
            public override int Next(int minValue, int maxValue) { Calls++; return base.Next(minValue, maxValue); }
            public override double NextDouble() { Calls++; return base.NextDouble(); }
            public override void NextBytes(byte[] buffer) { Calls++; base.NextBytes(buffer); }
        }

        private static int[] LiveIds<T>() where T : UnityEngine.Object =>
            UnityEngine.Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Select(value => value.GetInstanceID()).OrderBy(value => value).ToArray();

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("Town story follow-up: " + message);
        }
    }
}
