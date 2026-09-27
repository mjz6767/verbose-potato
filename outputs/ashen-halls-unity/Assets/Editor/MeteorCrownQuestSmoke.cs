using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class MeteorCrownQuestSmoke
    {
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

        public static void Run()
        {
            try { RunOrThrow(); Debug.Log(VersionInfo.ProductName + " Meteor Crown quest smoke passed."); EditorApplication.Exit(0); }
            catch (Exception exception) { Debug.LogError(VersionInfo.ProductName + " Meteor Crown quest smoke failed: " + exception); EditorApplication.Exit(1); }
        }

        public static void RunOrThrow()
        {
            RulesRejectPrematureOrRepeatedProgress();
            ProductionFinaleSurvivesRetreatAndSave();
            PrototypeFinaleRemainsSeparate();
        }

        private static void RulesRejectPrematureOrRepeatedProgress()
        {
            List<string> flags = new List<string>();
            Require(!MeteorCrownQuestRules.TryAccept(flags), "final assault cannot start before Chapter V");
            flags.Add(StoryFlags.CrownroadMarshalDefeated);
            flags.Add(StoryFlags.MeteorCrownThresholdSurveyed);
            Require(!MeteorCrownQuestRules.TryAccept(flags), "the survey must be debriefed with Yara");
            flags.Add(StoryFlags.RedGateDebriefed);
            Require(!ContentSetCatalog.AllowMeteorCrownChapter(ContentSetCatalog.SewerSlice, flags), "a debrief is not acceptance");
            Require(MeteorCrownQuestRules.TryAccept(flags), "the reviewed assault can be accepted");
            Require(!MeteorCrownQuestRules.TryAccept(flags), "acceptance cannot repeat");
            Require(!MeteorCrownQuestRules.TryRecordVictory(flags, MeteorCrownQuestRules.SentriesStyle), "a stale fight callback cannot win before entering");
            flags.Add(StoryFlags.MeteorCrownEntered);
            Require(!MeteorCrownQuestRules.TryRecordVictory(flags, MeteorCrownQuestRules.HeartStyle), "heart cannot skip sentries");
            Require(!MeteorCrownQuestRules.TryRecordVictory(flags, MeteorCrownQuestRules.SovereignStyle), "sovereign cannot skip heart");
            foreach (string style in new[] { MeteorCrownQuestRules.SentriesStyle, MeteorCrownQuestRules.HeartStyle, MeteorCrownQuestRules.SovereignStyle })
            {
                Require(MeteorCrownQuestRules.TryRecordVictory(flags, style), "ordered victory advances " + style);
                Require(!MeteorCrownQuestRules.TryRecordVictory(flags, style), "repeated victory cannot reward " + style);
                EncounterDefinition encounter = EncounterCatalog.For(EncounterCatalog.IdForLegacyStyle(style));
                Require(encounter.EnemyCountForDepth(6) == 4 && encounter.EnemyIds.All(id => ContentSetCatalog.EnemyActive(ContentSetCatalog.SewerSlice, id)), "the authored four-enemy roster is production content");
                Require(!encounter.Obstacles.Any(point => point.Kind == "glyph" || point.Kind == "demonrift"), "the last road has bounded enemy counts");
            }
            Require(MeteorCrownQuestRules.Complete(flags), "the final victory completes the six-chapter campaign");
            Require(!MeteorCrownQuestRules.IsEncounter("boss"), "prototype boss style cannot record authored progress");
        }

        private static void ProductionFinaleSurvivesRetreatAndSave()
        {
            using (Fixture fixture = new Fixture())
            {
                fixture.Call("ShowYaraConversation");
                fixture.Choose("plan");
                fixture.Choose("prepare");
                Require(!MeteorCrownQuestRules.Accepted(fixture.State.StoryFlags), "preparing leaves the final descent sealed");
                fixture.Choose("plan");
                fixture.Choose("accept_meteor_crown");
                fixture.Reload();
                Require(MeteorCrownQuestRules.Accepted(fixture.State.StoryFlags), "explicit acceptance survives production save/load");
                int arrivalSupplies = fixture.State.Supplies;
                fixture.Visit(MeteorCrownQuestRules.SovereignSite);
                Require(fixture.State.Depth == 6 && fixture.State.Mode == GameMode.Explore && fixture.State.Combat == null,
                    "actual Chapter V Salt Cistern interaction enters Chapter VI without triggering the prototype boss; depth=" + fixture.State.Depth
                    + ", mode=" + fixture.State.Mode + ", style=" + fixture.State.Combat?.EncounterStyle + ", flags=" + string.Join(",", fixture.State.StoryFlags));
                Require(fixture.State.Supplies == arrivalSupplies + 2, "the first arrival grants the promised camp cache once");
                Require(fixture.State.ActiveStory.Contains("crownwatch"), "entry gives the first objective");
                fixture.RequireLiveGuidance("regional-site:" + MeteorCrownQuestRules.SentriesSite);
                fixture.Visit(MeteorCrownQuestRules.HeartSite);
                fixture.Visit(MeteorCrownQuestRules.SovereignSite);
                Require(fixture.State.Mode == GameMode.Explore && !MeteorCrownQuestRules.HeartBroken(fixture.State.StoryFlags), "out-of-order sites cannot enter later battles");
                fixture.Visit(MeteorCrownQuestRules.SentriesSite);
                Require(fixture.State.Combat?.EncounterStyle == MeteorCrownQuestRules.SentriesStyle, "actual site dispatch starts the crownwatch encounter");
                fixture.WinCurrentBattle();
                Require(MeteorCrownQuestRules.SentriesDefeated(fixture.State.StoryFlags) && fixture.State.Mode == GameMode.Explore, "the first victory advances the story without ending it");
                int sentryElixirs = fixture.State.Elixirs;
                fixture.Call("ApplyMeteorCrownStoryVictory", MeteorCrownQuestRules.SentriesStyle);
                Require(fixture.State.Elixirs == sentryElixirs, "sentry callback replay does not duplicate its elixir");
                fixture.Reload();
                fixture.Call("RecallToTempleSquare");
                Require(fixture.State.Depth == 1 && fixture.State.ActiveStory.StartsWith("Chapter VI", StringComparison.Ordinal), "recall preserves the active finale objective");
                Require((string)fixture.Call("MeteorCrownGuidanceTargetId") == "old-road-descent-sluice-steps", "recall guidance leads back to the eastern route");
                fixture.RequireLiveGuidance("old-road-descent-sluice-steps");
                // Use the same authored route objects and transitions available to the player.
                fixture.FollowCurrentRoute();
                fixture.FollowCurrentRoute();
                fixture.FollowCurrentRoute();
                fixture.Visit(MeteorCrownQuestRules.SentriesSite);
                int revisitedSupplies = fixture.State.Supplies;
                fixture.Visit(MeteorCrownQuestRules.SovereignSite);
                Require(fixture.State.Depth == 6 && fixture.State.Supplies == revisitedSupplies,
                    "all five existing chapter routes can regain the finale without duplicating its arrival cache");
                Require((string)fixture.Call("MeteorCrownCurrentSiteId") == MeteorCrownQuestRules.HeartSite,
                    "saved first victory points re-entry at the remaining heart objective");
                fixture.RequireLiveGuidance("regional-site:" + MeteorCrownQuestRules.HeartSite);
                fixture.Visit(MeteorCrownQuestRules.HeartSite);
                Require(fixture.State.Combat?.EncounterStyle == MeteorCrownQuestRules.HeartStyle
                    && fixture.State.Combat.Units.Any(unit => unit.Role == "ritualheart"), "the actual second encounter contains the ritual heart");
                Require(!(bool)fixture.Call("IsFinalBossCombat"), "the preliminary ritual heart never triggers the campaign ending");
                fixture.Reload();
                Require(fixture.State.Mode == GameMode.Combat && fixture.State.Combat?.EncounterStyle == MeteorCrownQuestRules.HeartStyle
                    && !(bool)fixture.Call("IsFinalBossCombat"), "an in-progress heart battle survives save/load without becoming the final battle");
                fixture.WinCurrentBattle();
                Require(MeteorCrownQuestRules.HeartBroken(fixture.State.StoryFlags) && fixture.State.Mode == GameMode.Explore
                    && !MeteorCrownQuestRules.Complete(fixture.State.StoryFlags), "breaking the heart unlocks Vhal Rakh without ending the campaign");
                int heartSupplies = fixture.State.Supplies;
                int heartElixirs = fixture.State.Elixirs;
                fixture.Call("ApplyMeteorCrownStoryVictory", MeteorCrownQuestRules.HeartStyle);
                Require(fixture.State.Supplies == heartSupplies && fixture.State.Elixirs == heartElixirs, "heart callback replay cannot duplicate its recovery cache");
                fixture.Reload();
                fixture.RequireLiveGuidance("regional-site:" + MeteorCrownQuestRules.SovereignSite);
                fixture.Visit(MeteorCrownQuestRules.SovereignSite);
                Require(NpcPortraitCatalog.Resolve(fixture.Get<ObjectType>("dialogueFocus"), fixture.Get<string>("dialogueSpeaker")) != NpcId.Yara,
                    "Vhal Rakh never borrows Yara's portrait");
                fixture.Choose("rest");
                Require(fixture.State.Mode == GameMode.Explore && !MeteorCrownQuestRules.Complete(fixture.State.StoryFlags), "stepping back from Vhal Rakh leaves a real preparation opportunity");
                fixture.Visit(MeteorCrownQuestRules.SovereignSite);
                fixture.Choose("stand");
                Require(fixture.State.Combat?.EncounterStyle == MeteorCrownQuestRules.SovereignStyle && (bool)fixture.Call("IsFinalBossCombat"),
                    "explicit confrontation starts the correctly recognized authored final battle");
                fixture.WinCurrentBattle();
                Require(fixture.State.Mode == GameMode.Victory && MeteorCrownQuestRules.Complete(fixture.State.StoryFlags), "actual combat completion reaches a durable campaign ending");
                Require(fixture.State.ActiveStory.StartsWith("Epilogue: The Last Dawn", StringComparison.Ordinal), "the final state contains the authored epilogue");
                int inventoryCount = fixture.State.Inventory.Count;
                fixture.Call("FinishAuthoredMeteorCrownVictory");
                Require(fixture.State.Inventory.Count == inventoryCount, "an epilogue replay cannot award a second relic");
                fixture.Reload();
                Require(fixture.State.Mode == GameMode.Victory && MeteorCrownQuestRules.Complete(fixture.State.StoryFlags)
                    && fixture.State.ActiveStory.StartsWith("Epilogue: The Last Dawn", StringComparison.Ordinal), "victory mode, final flag, and ending survive save/load and story repair");
                IReadOnlyList<ArmoryRowView> rows = (IReadOnlyList<ArmoryRowView>)fixture.Call("BuildArmoryJournalRows");
                Require(rows.Count(row => row.Title.StartsWith("The Last Dawn - ", StringComparison.Ordinal) && row.Subtitle == "done") == 4,
                    "all four authored finale journal milestones are complete");
            }
        }

        private static void PrototypeFinaleRemainsSeparate()
        {
            using (Fixture fixture = new Fixture())
            {
                fixture.Set("activeContentSet", ContentSetCatalog.FullPrototype);
                fixture.State.ContentSetId = ContentSetCatalog.FullPrototype;
                Require(!(bool)fixture.Call("TryShowMeteorCrownConversation"), "legacy prototype saves keep their separate route");
                fixture.State.Depth = 6;
                fixture.State.Combat = new CombatState { EncounterStyle = "boss", Units = new List<CombatUnit> { new CombatUnit { Side = UnitSide.Enemy, Role = "meteorlich" } } };
                Require((bool)fixture.Call("IsFinalBossCombat"), "the original prototype boss still ends its own campaign");
                fixture.Set("activeContentSet", ContentSetCatalog.SewerSlice);
                Require(!(bool)fixture.Call("IsFinalBossCombat"), "a prototype boss in a production state cannot bypass authored acceptance and ordered victories");
            }
        }

        private sealed class Fixture : IDisposable
        {
            private readonly GameObject host;
            private readonly AshenHallsGame game;
            private readonly string savePath;
            public GameState State => Get<GameState>("state");

            public Fixture()
            {
                host = new GameObject("Meteor Crown quest smoke");
                host.SetActive(false);
                host.hideFlags = HideFlags.HideAndDontSave;
                game = host.AddComponent<AshenHallsGame>();
                Set("state", new GameState
                {
                    SaveVersion = VersionInfo.SaveVersion, ContentSetId = ContentSetCatalog.SewerSlice,
                    Mode = GameMode.Explore, Depth = 5, StoryChapter = 6, Seed = 727,
                    Gold = 120, Supplies = 8, Elixirs = 2, ReducedMotion = true, SfxMuted = true, MusicMuted = true,
                    StoryFlags = new List<string>
                    {
                        StoryFlags.OldRoadTeaserUnlocked, StoryFlags.KoboldKingDefeated, StoryFlags.GloamWardenDefeated,
                        StoryFlags.RedGateWarningRecovered, StoryFlags.GlassAndAshFrontierSurveyed, StoryFlags.GlassAndAshExpeditionAccepted,
                        StoryFlags.EmberglassGateKeyRecovered, StoryFlags.GlassAndAshDebriefed, StoryFlags.RedGateAssaultAccepted,
                        StoryFlags.RedGateVanguardDefeated, StoryFlags.OssuaryRoadSealRecovered, StoryFlags.CrownroadMarshalDefeated,
                        StoryFlags.MeteorCrownThresholdSurveyed, StoryFlags.RedGateDebriefed
                    },
                    Party = Enumerable.Range(0, 4).Select(i => new PartyMember
                    {
                        Id = "finale-" + i, Name = "Companion " + i, Race = "ashling", ClassKey = "warrior", Role = "shield",
                        Level = 8, Stats = new Stats(18, 14, 14, 18), Hp = 60, MaxHp = 60, Mana = 30, MaxMana = 30,
                        Power = 12, Defense = 8, Range = 1, Movement = 4, Agility = 9, AttackSpeed = 10, DamageMin = 5, DamageMax = 10,
                        Skills = new SkillSet().Normalize(), WeaponName = "sword", ArmorName = "mail"
                    }).ToList()
                });
                Set("activeContentSet", ContentSetCatalog.SewerSlice);
                Set("rng", new System.Random(727));
                Set("visualSmokeSaveBlocked", true);
                Set("launchError", "Meteor Crown fixture holds UI presentation");
                State.Map = (MapData)Call("GenerateMap", 5, 727);
                Call("EnsureWorldLandmarks");
                Call("PlacePlayerAtExplorationStart");
                savePath = Path.Combine(Path.GetTempPath(), "ashen-meteor-crown-" + Guid.NewGuid().ToString("N") + ".json");
            }

            public void Visit(string siteId)
            {
                Call("CloseTransientOverlays");
                MapObject site = State.Map.FindObjectById("regional-site:" + siteId);
                Require(site != null, "live chapter map contains story site " + siteId);
                IReadOnlyList<Point> approach = (IReadOnlyList<Point>)Call("FindLiveExplorePathToObject", site);
                Require(approach != null && approach.Count > 0,
                    "each site visit has a legal live approach path: " + siteId + " from " + State.PlayerX + "," + State.PlayerY);
                // Libraries and gate structures are used from adjacent floor. Placing
                // a fixture directly inside one creates an impossible player position.
                Point arrival = approach[approach.Count - 1];
                State.PlayerX = arrival.X;
                State.PlayerY = arrival.Y;
                Call("ResolveExploreObject", site);
            }

            public void FollowCurrentRoute()
            {
                string target = (string)Call("MeteorCrownGuidanceTargetId");
                MapObject route = State.Map.FindObjectById(target);
                Require(route != null && route.Type == ObjectType.Stairs, "live return journey exposes " + target);
                State.PlayerX = route.X;
                State.PlayerY = route.Y;
                Call("Descend");
            }

            public void RequireLiveGuidance(string expectedObjectId)
            {
                object[] args = { null };
                bool found = (bool)Call("TryBoneRoadStoryGuidancePlan", args);
                object plan = args[0];
                Require(found && plan != null, "live guidance produces the active chapter route");
                Type planType = plan.GetType();
                MapObject target = (MapObject)planType.GetField("TargetObject").GetValue(plan);
                bool blocked = (bool)planType.GetField("RouteBlocked").GetValue(plan);
                IReadOnlyList<Point> path = (IReadOnlyList<Point>)planType.GetField("Path").GetValue(plan);
                Require(target?.Id == expectedObjectId && !blocked && path != null && path.Count > 0,
                    "live map path reaches expected finale objective " + expectedObjectId + "; actual=" + target?.Id
                    + ", blocked=" + blocked + ", count=" + path?.Count + ", party=" + State.PlayerX + "," + State.PlayerY);
                Require(path[0].X == State.PlayerX && path[0].Y == State.PlayerY,
                    "guidance path begins at the actual party position after entry or recall");
                for (int i = 1; i < path.Count; i++)
                    Require(Math.Abs(path[i].X - path[i - 1].X) + Math.Abs(path[i].Y - path[i - 1].Y) == 1,
                        "guidance follows adjacent walkable route steps");
            }

            public void WinCurrentBattle()
            {
                Require(State.Mode == GameMode.Combat && State.Combat != null, "an actual authored battle is running");
                foreach (CombatUnit unit in State.Combat.Units.Where(unit => unit.Side == UnitSide.Enemy)) unit.Hp = 0;
                Call("FinishCombat");
                Call("CloseTransientOverlays");
            }

            public void Choose(string id)
            {
                DialogueChoiceView[] choices = Get<DialogueChoiceView[]>("dialogueChoices");
                Require(choices != null && choices.Any(choice => choice.Id == id && choice.Enabled), "native dialogue exposes " + id);
                string[] pages = Get<string[]>("dialoguePages");
                for (int i = 0; i < (pages?.Length ?? 0) - 1; i++) Call("AdvanceDialogue");
                typeof(AshenHallsGame).GetMethod("ChooseDialogueChoice", Private, null, new[] { typeof(string) }, null).Invoke(game, new object[] { id });
            }

            public void Reload()
            {
                string before = JsonUtility.ToJson(State);
                SaveService.SaveGameState(savePath, State);
                GameState loaded = SaveService.LoadGameState(savePath, out bool backup);
                Require(!backup && JsonUtility.ToJson(loaded) == before, "production save roundtrip preserves exact finale state");
                Call("CloseTransientOverlays");
                Set("state", loaded);
                Call("EnsureWorldState", VersionInfo.SaveVersion);
            }

            public object Call(string name, params object[] args)
            {
                MethodInfo method = typeof(AshenHallsGame).GetMethod(name, Private);
                Require(method != null, "runtime method exists: " + name);
                try { return method.Invoke(game, args); }
                catch (TargetInvocationException exception) { throw new InvalidOperationException("Meteor Crown production call failed: " + name, exception.InnerException); }
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
            if (!condition) throw new InvalidOperationException("Meteor Crown: " + message);
        }
    }
}
