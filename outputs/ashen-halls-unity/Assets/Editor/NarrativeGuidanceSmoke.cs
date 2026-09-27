using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class NarrativeGuidanceSmoke
    {
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

        public static void Run()
        {
            try { RunOrThrow(); Debug.Log(VersionInfo.ProductName + " narrative guidance smoke passed."); EditorApplication.Exit(0); }
            catch (Exception exception) { Debug.LogError(VersionInfo.ProductName + " narrative guidance smoke failed: " + exception); EditorApplication.Exit(1); }
        }

        public static void RunOrThrow()
        {
            CompletedWritDoesNotSendTheCompanyBackForProof();
            RoadBriefingsFollowTheSavedCampaign();
            OptionalLeadAndCurrentJourneyRemainVisible();
            PrototypeSurveyAndRemembranceRemainReachable();
        }

        private static void CompletedWritDoesNotSendTheCompanyBackForProof()
        {
            using (Fixture fixture = new Fixture())
            {
                fixture.Call("ShowHalvardConversation");
                fixture.ExpectChoices("accept", "duty", "city", "decline");
                fixture.Choose("duty");
                Require(fixture.Body.Contains("Three chambers"), "unaccepted writ explains the actual first contract");
                Require(!fixture.State.StoryFlags.Contains(StoryFlags.SewerContractAccepted), "asking for terms does not accept the writ");
                fixture.CompleteCisterns(false);
                fixture.Call("ShowBrannConversation", new object[] { null });
                fixture.Choose("watch");
                Require(fixture.Body.Contains("Take the proof to Borin"), "proof-ready company still receives the correct hand-in instruction");

                fixture.ClaimCisternReward();
                Require(ContentSetCatalog.CountSewerSliceProof(fixture.State.Inventory) == 0, "claimed proof is consumed");
                string snapshot = JsonUtility.ToJson(fixture.State);
                fixture.Call("ShowHalvardConversation");
                Require(fixture.Body.Contains("closed and paid"), "king acknowledges the consumed proof and completed payment");
                fixture.Choose("duty");
                Require(fixture.Body.Contains("Lanternless Cross") && fixture.Body.Contains("eastbound"), "king directs the completed company along the playable Chapter II route");
                Require(!fixture.Body.Contains("stair below the Salt Cisterns"), "normal campaign does not advertise the legacy descent");
                fixture.Call("ShowHalvardConversation"); fixture.Choose("road");
                Require(fixture.Body.Contains("East Gate") && fixture.Body.Contains("Dusk Market"), "road explanation names the actual eastbound exit");
                fixture.Call("ShowBrannConversation", new object[] { null });
                Require(!fixture.Body.Contains("roads can wait"), "captain no longer bars a completed contract in his greeting");
                fixture.Choose("watch");
                Require(fixture.Body.Contains("signed off") && fixture.Body.Contains("Lanternless Cross")
                    && !fixture.Body.Contains("Take the proof to Borin"), "captain does not repeat the paid proof hand-in");
                Require(JsonUtility.ToJson(fixture.State) == snapshot, "reading the closed writ and road briefings changes no campaign or reward data");
            }
        }

        private static void RoadBriefingsFollowTheSavedCampaign()
        {
            using (Fixture fixture = new Fixture())
            {
                fixture.CompleteCisterns(true);
                var beats = new[]
                {
                    new Beat(StoryFlags.KoboldKingDefeated, "Bone Road"),
                    new Beat(StoryFlags.GloamWardenDefeated, "Red Gate Seal"),
                    new Beat(StoryFlags.RedGateWarningRecovered, "Survey"),
                    new Beat(StoryFlags.GlassAndAshFrontierSurveyed, "Yara"),
                    new Beat(StoryFlags.GlassAndAshExpeditionAccepted, "Glass Lore Library"),
                    new Beat(StoryFlags.EmberglassGateKeyRecovered, "Bring the Emberglass key"),
                    new Beat(StoryFlags.GlassAndAshDebriefed, "review the Red Gate assault"),
                    new Beat(StoryFlags.RedGateAssaultAccepted, "ossuary road seal"),
                    new Beat(StoryFlags.CrownroadMarshalDefeated, "survey the sealed threshold"),
                    new Beat(StoryFlags.MeteorCrownThresholdSurveyed, "marshal's seal"),
                    new Beat(StoryFlags.RedGateDebriefed, "review the final assault"),
                    new Beat(StoryFlags.MeteorCrownAssaultAccepted, "crownwatch"),
                    new Beat(StoryFlags.MeteorCrownHeartBroken, "Vhal Rakh is the last position"),
                    new Beat(StoryFlags.MeteorCrownDefeated, "no further royal order")
                };
                foreach (Beat beat in beats)
                {
                    fixture.State.StoryFlags.Add(beat.Flag);
                    string before = JsonUtility.ToJson(fixture.State);
                    fixture.Call("ShowHalvardConversation"); fixture.Choose("duty");
                    Require(fixture.Body.Contains(beat.Expected), "king's current duty follows durable flag " + beat.Flag);
                    fixture.Call("ShowBrannConversation", new object[] { null }); fixture.Choose("watch");
                    Require(fixture.Body.Contains(beat.Expected), "captain's current watch follows durable flag " + beat.Flag);
                    Require(JsonUtility.ToJson(fixture.State) == before, "reading the next road never accepts or advances it: " + beat.Flag);
                }
                fixture.Call("ShowHalvardConversation");
                Require(fixture.Body.Contains("Vhal Rakh is gone"), "king welcomes the final company home");
                fixture.Call("VisitGateCaptain");
                Require(fixture.Body.Contains("Stand easy"), "actual captain visit reaches the homecoming greeting");
                fixture.Choose("roads");
                Require(fixture.Body.Contains("no royal order left"), "postgame roads remain available without inventing a new duty");
            }
        }

        private static void OptionalLeadAndCurrentJourneyRemainVisible()
        {
            using (Fixture fixture = new Fixture())
            {
                fixture.CompleteCisterns(true);
                Require(!fixture.Journal().Any(row => row.Title == HonestWeightQuestRules.Title), "Maud's lead stays absent before Varkh falls");
                fixture.State.StoryFlags.Add(StoryFlags.KoboldKingDefeated);
                string snapshot = JsonUtility.ToJson(fixture.State);
                IReadOnlyList<ArmoryRowView> rows = fixture.Journal();
                ArmoryRowView favor = rows.Single(row => row.Title == HonestWeightQuestRules.Title);
                Require(favor.Subtitle.Contains("available") && favor.Detail.Contains("Maud") && favor.Badge == "OPTIONAL",
                    "eligible unaccepted favor has a named journal lead");
                Require(!HonestWeightQuestRules.Accepted(fixture.State.StoryFlags) && JsonUtility.ToJson(fixture.State) == snapshot,
                    "discovering the lead does not accept it or alter the main objective");
                Require(rows[0].Title == "Current journey" && rows[0].Detail.Contains("Old Road"),
                    "production journal starts with the current Chapter III return route; actual=" + rows[0].Title + ": " + rows[0].Detail);
                fixture.State.StoryFlags.AddRange(new[]
                {
                    StoryFlags.RedGateWarningRecovered, StoryFlags.GlassAndAshFrontierSurveyed,
                    StoryFlags.GlassAndAshExpeditionAccepted, StoryFlags.EmberglassGateKeyRecovered,
                    StoryFlags.GlassAndAshDebriefed, StoryFlags.RedGateAssaultAccepted,
                    StoryFlags.CrownroadMarshalDefeated, StoryFlags.MeteorCrownThresholdSurveyed,
                    StoryFlags.RedGateDebriefed
                });
                string summary = (string)fixture.Call("ExploreObjectiveSummaryLine");
                Require(summary.Contains("final Meteor Crown assault") && summary.Contains("accept"), "Yara's VI handoff explains the pending acceptance");
                rows = fixture.Journal();
                Require(rows[0].Title == "Current journey" && rows[0].Detail == summary, "journal and HUD agree about the pending final plan");
                fixture.State.StoryFlags.Add(StoryFlags.MeteorCrownAssaultAccepted);
                fixture.Call("EnsureOldRoadDescentMarker");
                Require(((string)fixture.Call("ExploreObjectiveSummaryLine")).Contains("final assault"), "VI return route does not fall back to Chapter V's assault name");
                fixture.State.StoryFlags.Add(StoryFlags.MeteorCrownDefeated);
                fixture.State.ActiveStory = "Epilogue: The Last Dawn. Vhal Rakh is defeated, and the company has come home.";
                rows = fixture.Journal();
                Require(rows[0].Title == "Current journey" && rows[0].Detail.Contains("Vhal Rakh is defeated")
                    && rows[0].Badge == "COMPLETE", "postgame journal leads with the completed journey");
            }
        }

        private static void PrototypeSurveyAndRemembranceRemainReachable()
        {
            using (Fixture fixture = new Fixture())
            {
                fixture.State.StoryFlags.Add(StoryFlags.RemembranceAccepted);
                fixture.Call("ShowBrannConversation", new object[] { null });
                fixture.ExpectChoices("watch", "roads", "sealed", "remembrance");
                fixture.Choose("remembrance");
                Require(fixture.Get<string>("dialogueTitle") == RemembranceQuestRules.Title, "captain's optional favor stays native-button reachable");
                fixture.State.ContentSetId = ContentSetCatalog.FullPrototype;
                fixture.Set("activeContentSet", ContentSetCatalog.FullPrototype);
                fixture.Call("ShowBrannConversation", new object[] { null });
                fixture.ExpectChoices("survey", "roads", "sealed", "remembrance");
                fixture.Choose("survey");
                Require(fixture.Body.Contains("survey still requires"), "prototype retains its actual gate survey");
            }
        }

        private readonly struct Beat
        {
            public readonly string Flag;
            public readonly string Expected;
            public Beat(string flag, string expected) { Flag = flag; Expected = expected; }
        }

        private sealed class Fixture : IDisposable
        {
            private readonly GameObject host;
            private readonly AshenHallsGame game;
            public GameState State => Get<GameState>("state");
            public string Body => Get<string>("dialogueBody");
            public Fixture()
            {
                host = new GameObject("Narrative guidance fixture"); host.SetActive(false); host.hideFlags = HideFlags.HideAndDontSave;
                game = host.AddComponent<AshenHallsGame>();
                Set("state", new GameState
                {
                    SaveVersion = VersionInfo.SaveVersion, ContentSetId = ContentSetCatalog.SewerSlice,
                    Mode = GameMode.Explore, Depth = 1, StoryChapter = 1, Seed = 727,
                    ActiveStory = "Chapter I: The Midgaard Cisterns. Meet King Halvard.",
                    Gold = 31, Supplies = 7, Elixirs = 2, ReducedMotion = true, SfxMuted = true, MusicMuted = true,
                    StoryFlags = new List<string> { StoryFlags.MidgaardGrandHearthDeparted }, Inventory = new List<InventoryItem>(),
                    Party = new List<PartyMember> { new PartyMember { Id = "narrative-hero", Name = "Witness", Race = "ashling", ClassKey = "ranger", Role = "bow", Level = 1,
                        Stats = new Stats(10, 10, 18, 12), Hp = 28, MaxHp = 28, Skills = new SkillSet().Normalize(), WeaponName = "shortbow", ArmorName = "cloth" } }
                });
                Set("activeContentSet", ContentSetCatalog.SewerSlice); Set("rng", new System.Random(727));
                Set("visualSmokeSaveBlocked", true); Set("launchError", "Narrative fixture holds UI presentation");
                State.Map = (MapData)Call("GenerateMap", 1, 727);
                Call("EnsureWorldLandmarks"); Call("PlacePlayerAtExplorationStart");
            }
            public void CompleteCisterns(bool claim)
            {
                ContentSetCatalog.MarkSewerSliceContractAccepted(State.StoryFlags);
                foreach (EncounterDefinition encounter in ContentSetCatalog.SewerSliceEncounters)
                {
                    ContentSetCatalog.MarkSewerSliceEncounterCleared(State.StoryFlags, encounter.LegacyStyle);
                    State.Inventory.Add(ContentSetCatalog.CreateSewerSliceProof());
                }
                if (claim)
                {
                    ClaimCisternReward();
                }
            }
            public void ClaimCisternReward()
            {
                // The content rule consumes proof and marks the receipt; the actual
                // armorer also sets ArmorMade and creates the eastbound route marker.
                // Guidance must observe that complete production transaction.
                Require((bool)Call("TryCompleteRatPeltArmor"), "fixture completes Borin's actual proof transaction");
                Require(State.StoryFlags.Contains(StoryFlags.MidgaardRatPeltArmorMade)
                    && State.Map.FindObjectById("old-road-descent-sluice-steps") != null,
                    "paid-writ fixture contains the real armor completion and opened road");
                Call("CloseTransientOverlays");
            }
            public IReadOnlyList<ArmoryRowView> Journal() => (IReadOnlyList<ArmoryRowView>)Call("BuildArmoryJournalRows");
            public void ExpectChoices(params string[] ids) => Require(NativeChoices().Select(choice => choice.Id).SequenceEqual(ids), "native choices remain reachable: " + string.Join(",", ids));
            public void Choose(string id)
            {
                DialogueChoiceView[] choices = NativeChoices();
                int index = Array.FindIndex(choices, choice => choice.Id == id && choice.Enabled);
                Require(index >= 0, "native button exists for " + id);
                Get<DialogueScreen>("dialogueScreen").InvokeChoiceForTest(index);
            }
            private DialogueChoiceView[] NativeChoices()
            {
                while (Get<int>("dialoguePageIndex") < (Get<string[]>("dialoguePages")?.Length ?? 0) - 1) Call("AdvanceDialogue");
                Call("EnsureDialogueScreen");
                DialogueScreen screen = Get<DialogueScreen>("dialogueScreen"); screen.Refresh();
                DialogueScreenView view = (DialogueScreenView)Call("BuildDialogueScreenView");
                Require(view.Choices.Length <= 4 && view.Choices.Length == screen.VisibleChoiceCountForTest, "every adapted choice is actually visible");
                return view.Choices;
            }
            public object Call(string name, params object[] args)
            {
                MethodInfo method = typeof(AshenHallsGame).GetMethod(name, Private);
                Require(method != null, "runtime method exists: " + name);
                try { return method.Invoke(game, args); }
                catch (TargetInvocationException exception) { throw new InvalidOperationException("Narrative runtime call failed: " + name, exception.InnerException); }
            }
            public T Get<T>(string name) => (T)typeof(AshenHallsGame).GetField(name, Private).GetValue(game);
            public void Set(string name, object value) => typeof(AshenHallsGame).GetField(name, Private).SetValue(game, value);
            public void Dispose() => UnityEngine.Object.DestroyImmediate(host);
        }
        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("Narrative guidance: " + message);
        }
    }
}
