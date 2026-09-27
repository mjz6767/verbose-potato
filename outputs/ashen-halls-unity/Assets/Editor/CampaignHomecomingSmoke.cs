using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class CampaignHomecomingSmoke
    {
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " campaign homecoming smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogError(VersionInfo.ProductName + " campaign homecoming smoke failed: " + exception);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            CompletedCompanyCanReturnHomeAndContinueItsSave();
            OtherEndStatesRetainTheirTavernExit();
        }

        private static void CompletedCompanyCanReturnHomeAndContinueItsSave()
        {
            using (Fixture fixture = new Fixture())
            {
                EndStateView ending = (EndStateView)fixture.Call("BuildEndStateView");
                Require(ending.Victory && ending.ShowTavernButton && ending.ReturnButtonLabel == "Return Home",
                    "the native victory adapter advertises a usable Return Home action");
                string[] partyBefore = ProgressAndGear(fixture.State);
                string[] inventoryBefore = fixture.State.Inventory.Select(JsonUtility.ToJson).ToArray();
                int gold = fixture.State.Gold;
                int supplies = fixture.State.Supplies;
                int elixirs = fixture.State.Elixirs;

                fixture.Call("ReturnEndStateToTavern");
                AssertCompletedHome(fixture.State);
                Require(fixture.State.Party.All(member => member.Hp == member.MaxHp && member.Mana == member.MaxMana),
                    "coming home restores the company through the existing temple recovery");
                Require(partyBefore.SequenceEqual(ProgressAndGear(fixture.State))
                    && inventoryBefore.SequenceEqual(fixture.State.Inventory.Select(JsonUtility.ToJson)),
                    "Return Home retains progression, equipment identities and every carried item");
                Require(fixture.State.Gold == gold && fixture.State.Supplies == supplies && fixture.State.Elixirs == elixirs,
                    "Return Home cannot duplicate victory spoils or consume travel supplies");
                string[] flags = fixture.State.StoryFlags.ToArray();
                fixture.Call("ReturnEndStateToTavern");
                AssertCompletedHome(fixture.State);
                Require(flags.SequenceEqual(fixture.State.StoryFlags) && partyBefore.SequenceEqual(ProgressAndGear(fixture.State)),
                    "a stale repeated ending callback leaves the company home without replaying rewards");

                fixture.TalkToYara();
                fixture.Call("CloseTransientOverlays");
                fixture.Reload();
                AssertCompletedHome(fixture.State);
                Require(partyBefore.SequenceEqual(ProgressAndGear(fixture.State))
                    && inventoryBefore.SequenceEqual(fixture.State.Inventory.Select(JsonUtility.ToJson)),
                    "the production save roundtrip and story repair retain completed company progression and gear");
                Require(!CampaignMilestoneRules.Pending(fixture.State.StoryFlags).Any(),
                    "preclaimed chapter receipts remain satisfied after returning home and loading");
                Require(!MeteorCrownQuestRules.CanFight(fixture.State.StoryFlags, MeteorCrownQuestRules.SovereignStyle),
                    "a completed home save cannot replay the final battle for another reward");
                fixture.TalkToYara();
            }
        }

        private static void OtherEndStatesRetainTheirTavernExit()
        {
            foreach (bool defeat in new[] { false, true })
            {
                using (Fixture fixture = new Fixture())
                {
                    fixture.State.StoryFlags.Remove(StoryFlags.MeteorCrownDefeated);
                    fixture.State.Mode = defeat ? GameMode.Defeat : GameMode.Victory;
                    if (!defeat)
                    {
                        fixture.State.ContentSetId = ContentSetCatalog.FullPrototype;
                        fixture.Set("activeContentSet", ContentSetCatalog.FullPrototype);
                    }
                    EndStateView ending = (EndStateView)fixture.Call("BuildEndStateView");
                    Require(ending.ReturnButtonLabel != "Return Home", "defeat and legacy prototype endings do not promise authored homecoming");
                    int hp = fixture.State.Party[0].Hp;
                    fixture.Call("ReturnEndStateToTavern");
                    Require(fixture.State.Mode == GameMode.Tavern && fixture.State.Depth == 6 && fixture.State.Party[0].Hp == hp,
                        "the existing non-authored ending action still returns to title Tavern without temple recovery");
                }
            }
        }

        private static void AssertCompletedHome(GameState state)
        {
            // JsonUtility can rehydrate an absent nested record as an empty
            // CombatState. The invariant is no combat roster or active actor,
            // rather than the serialized representation of that empty record.
            bool noCombat = state.Combat == null
                || ((state.Combat.Units == null || state.Combat.Units.Count == 0)
                    && string.IsNullOrEmpty(state.Combat.ActiveId)
                    && string.IsNullOrEmpty(state.Combat.EncounterStyle));
            Require(state.Mode == GameMode.Explore && state.Depth == 1 && state.Map?.Depth == 1 && noCombat,
                "Return Home enters an actual Midgaard exploration map without a competing combat; mode=" + state.Mode
                + ", depth=" + state.Depth + ", map=" + state.Map?.Depth + ", combat units=" + state.Combat?.Units?.Count);
            Require(MeteorCrownQuestRules.Complete(state.StoryFlags) && state.StoryChapter == 6
                && state.ActiveStory.StartsWith("Epilogue: The Last Dawn", StringComparison.Ordinal),
                "the home exploration state retains the completed campaign and authored epilogue");
        }

        private static string[] ProgressAndGear(GameState state) => state.Party.Select(member =>
            string.Join("|", member.Id, member.Level, member.Experience, member.StatPoints, member.SkillPoints,
                member.WeaponName, member.WeaponItemId, member.WeaponBonus, member.ArmorName, member.ArmorItemId, member.ArmorBonus,
                JsonUtility.ToJson(member.Stats), JsonUtility.ToJson(member.Skills))).ToArray();

        private sealed class Fixture : IDisposable
        {
            private readonly GameObject host;
            private readonly AshenHallsGame game;
            private readonly string savePath;
            public GameState State => Get<GameState>("state");

            public Fixture()
            {
                savePath = Path.Combine(Path.GetTempPath(), "ashen-homecoming-" + Guid.NewGuid().ToString("N") + ".json");
                host = RemembranceItemSmoke.NewHost("Campaign homecoming fixture");
                game = host.AddComponent<AshenHallsGame>();
                GameState state = new GameState
                {
                    SaveVersion = VersionInfo.SaveVersion, ContentSetId = ContentSetCatalog.SewerSlice,
                    Mode = GameMode.Victory, Depth = 6, StoryChapter = 6, Seed = 733,
                    Gold = 180, Supplies = 5, Elixirs = 2, ReducedMotion = true, SfxMuted = true, MusicMuted = true,
                    StoryFlags = new List<string>
                    {
                        StoryFlags.MidgaardOldRoadScoutMet, StoryFlags.SewerRewardClaimed,
                        StoryFlags.OldRoadTeaserUnlocked, StoryFlags.KoboldKingDefeated,
                        StoryFlags.BoneRoadWatchDefeated, StoryFlags.GloamRitualBroken, StoryFlags.GloamWardenDefeated,
                        StoryFlags.RedGateWarningRecovered, StoryFlags.GlassAndAshFrontierSurveyed,
                        StoryFlags.GlassAndAshExpeditionAccepted, StoryFlags.GlasswardAmbushDefeated,
                        StoryFlags.GlassIndexRecovered, StoryFlags.EmberglassGateKeyRecovered, StoryFlags.GlassAndAshDebriefed,
                        StoryFlags.RedGateAssaultAccepted, StoryFlags.RedGateVanguardDefeated, StoryFlags.OssuaryRoadSealRecovered,
                        StoryFlags.CrownroadMarshalDefeated, StoryFlags.MeteorCrownThresholdSurveyed, StoryFlags.RedGateDebriefed,
                        StoryFlags.MeteorCrownAssaultAccepted, StoryFlags.MeteorCrownEntered,
                        StoryFlags.MeteorCrownSentriesDefeated, StoryFlags.MeteorCrownHeartBroken, StoryFlags.MeteorCrownDefeated
                    },
                    Party = StarterPartyCatalog.All.Select((hero, index) => new PartyMember
                    {
                        Id = "homecoming-" + index, Name = hero.Name, Race = hero.Race, ClassKey = hero.ClassKey, Role = hero.Role,
                        Stats = hero.Stats, Skills = hero.CreateSkills(), Spell = hero.Spell, Level = 16, Experience = 70,
                        StatPoints = 3, SkillPoints = 2, Hp = 25, MaxHp = 70, Mana = 4, MaxMana = 30,
                        Range = hero.Range, Movement = 3, Power = 12, Defense = 8, Agility = 9, AttackSpeed = 8,
                        WeaponName = hero.Role == "bow" ? "longbow" : "sword", ArmorName = "mail", DamageMin = 5, DamageMax = 10
                    }).ToList()
                };
                // The final battle's checkpoint has already paid these receipts.
                // Homecoming must not manufacture a second reward for them.
                state.StoryFlags.AddRange(CampaignMilestoneRules.All.Select(milestone => milestone.RewardFlag));
                Set("state", state);
                Set("activeContentSet", ContentSetCatalog.SewerSlice);
                Set("rng", new System.Random(state.Seed));
                Set("visualSmokeSaveBlocked", true);
                Set("launchError", "Homecoming fixture holds UI presentation");
                state.Map = (MapData)Call("GenerateMap", 6, state.Seed);
                Call("AddInventoryItem", SignatureItemCatalog.CreateUnfathomableSword());
                Call("EnsurePartyCustomization");
                Call("EnsureWorldState", VersionInfo.SaveVersion);
            }

            public void TalkToYara()
            {
                MapObject yara = State.Map.Objects.FirstOrDefault(obj => obj.Type == ObjectType.OldRoadScout);
                Require(yara != null, "Midgaard's actual map contains Yara after the ending");
                State.PlayerX = yara.X;
                State.PlayerY = yara.Y;
                Call("ResolveExploreObject", yara);
                DialogueScreenView view = (DialogueScreenView)Call("BuildDialogueScreenView");
                Require(view.Speaker == "Yara" && view.Body.Contains("kept a lamp"),
                    "the real map interaction reaches Yara's authored homecoming response");
                string[] pages = Get<string[]>("dialoguePages");
                for (int page = 1; page < pages.Length; page++) Call("AdvanceDialogue");
                Require(Get<DialogueChoiceView[]>("dialogueChoices").Any(choice => choice.Id == "home" && choice.Enabled),
                    "the homecoming conversation exposes the road-after-victory topic");
                typeof(AshenHallsGame).GetMethod("ChooseDialogueChoice", Private, null, new[] { typeof(string) }, null)
                    .Invoke(game, new object[] { "home" });
                view = (DialogueScreenView)Call("BuildDialogueScreenView");
                Require(view.Speaker == "Yara" && view.Body.Contains("bridges"),
                    "the player's actual dialogue choice reaches the post-campaign conversation");
            }

            public void Reload()
            {
                Require(SaveService.TrySaveCampaignState(savePath, State, false, out string blockedReason),
                    "the disposable completed-home save is accepted: " + blockedReason);
                GameState loaded = SaveService.LoadGameState(savePath,
                    candidate => SaveCandidateRules.IsLoadable(candidate, VersionInfo.SaveVersion), out bool backup);
                Require(!backup, "the primary homecoming save is loadable without recovery");
                Set("state", loaded);
                Call("EnsureWorldState", VersionInfo.SaveVersion);
            }

            public object Call(string name, params object[] arguments)
            {
                MethodInfo method = typeof(AshenHallsGame).GetMethod(name, Private);
                Require(method != null, "runtime method exists: " + name);
                return method.Invoke(game, arguments);
            }

            public T Get<T>(string name) => (T)typeof(AshenHallsGame).GetField(name, Private).GetValue(game);
            public void Set(string name, object value) => typeof(AshenHallsGame).GetField(name, Private).SetValue(game, value);
            public void Dispose()
            {
                UnityEngine.Object.DestroyImmediate(host);
                foreach (string suffix in new[] { "", ".bak", ".tmp" })
                    if (File.Exists(savePath + suffix)) File.Delete(savePath + suffix);
            }
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("Campaign homecoming: " + message);
        }
    }
}
