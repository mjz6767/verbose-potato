using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class RemembranceQuestSmoke
    {
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " remembrance quest smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " remembrance quest smoke failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            RuleTransitionsRejectShortcuts();
            foreach (string disposition in new[] { "private", "public" })
                foreach (string reward in new[] { "bow", "wrap" })
                    ProductionQuestSurvivesEveryCheckpoint(disposition, reward, false);
            ProductionQuestSurvivesEveryCheckpoint("public", "bow", true);
            LegacyPrototypeClearedSewerCanBeInspected();
        }

        private static void RuleTransitionsRejectShortcuts()
        {
            List<string> flags = new List<string>();
            Require(!RemembranceQuestRules.TryRecover(flags), "cannot recover without accepting and clearing");
            Require(!RemembranceQuestRules.TryReadNames(flags), "cannot transcribe without the recovered roll");
            Require(!RemembranceQuestRules.TryChooseDisposition(flags, "public"), "cannot choose a memorial before reading the names");
            Require(!RemembranceQuestRules.TryDeliver(flags, ObjectType.NoviceHealer), "cannot deliver before the choice");
            Require(!RemembranceQuestRules.TryClaimReward(flags, "bow"), "cannot claim before delivery");
            Require(RemembranceQuestRules.TryAccept(flags), "optional favor can be accepted");
            Require(!RemembranceQuestRules.TryAccept(flags), "acceptance is one time");
            flags.Add(StoryFlags.SewerBrokenSluiceCleared);
            Require(!RemembranceQuestRules.TryRecover(flags), "Broken Sluice alone does not open the Foul Runoff alcove");
            flags.Add(StoryFlags.SewerFoulRunoffCleared);
            Require(!RemembranceQuestRules.Recovered(flags), "room completion does not silently recover the roll");
            Require(RemembranceQuestRules.TryRecover(flags), "cleared alcove can be explicitly searched");
            Require(!RemembranceQuestRules.TryRecover(flags), "roll recovery cannot repeat");
            Require(RemembranceQuestRules.TryReadNames(flags), "Sera can read the recovered names");
            Require(!RemembranceQuestRules.TryChooseDisposition(flags, "invalid"), "unknown dispositions do nothing");
            Require(RemembranceQuestRules.TryChooseDisposition(flags, "private"), "private memorial can be chosen");
            Require(!RemembranceQuestRules.TryChooseDisposition(flags, "public"), "a stale callback cannot reverse Edda's consent");
            Require(!RemembranceQuestRules.TryDeliver(flags, ObjectType.GateCaptain), "private memorial must reach Sera");
            Require(RemembranceQuestRules.TryDeliver(flags, ObjectType.NoviceHealer), "Sera accepts the private memorial");
            Require(!RemembranceQuestRules.TryClaimReward(flags, "both"), "unknown reward does nothing");
            Require(RemembranceQuestRules.TryClaimReward(flags, "wrap"), "one keepsake can be selected");
            Require(!RemembranceQuestRules.TryClaimReward(flags, "bow"), "alternate reward cannot be claimed later");
        }

        private static void ProductionQuestSurvivesEveryCheckpoint(string disposition, string reward, bool acceptedAfterClear)
        {
            using (Fixture fixture = new Fixture())
            {
                if (acceptedAfterClear)
                {
                    // Every real sewer entry accepts this writ before combat. A completed
                    // sewer without its accepted flag would correctly reaccept the main
                    // contract when testing the optional prompt's "continue" branch.
                    ContentSetCatalog.MarkSewerSliceContractAccepted(fixture.State.StoryFlags);
                    fixture.State.StoryFlags.Add(StoryFlags.SewerCisternDenCleared);
                }
                fixture.Call("ShowEddaConversation");
                fixture.Choose("remembrance");
                fixture.Choose("back");
                Require(!RemembranceQuestRules.Accepted(fixture.State.StoryFlags), "declining leaves the favor optional");
                fixture.Choose("remembrance");
                fixture.Choose("accept");
                fixture.Reload();
                Require(RemembranceQuestRules.Accepted(fixture.State.StoryFlags), "acceptance survives the production save service");
                Require(!RemembranceQuestRules.Recovered(fixture.State.StoryFlags), "acceptance after an old clear still requires a site visit");
                Require(fixture.State.ActiveStory == Fixture.CampaignObjective, "acceptance preserves the campaign objective");

                fixture.Call("ResolveRemembranceSewerChoice", "inspect");
                Require(!RemembranceQuestRules.Recovered(fixture.State.StoryFlags), "stale inspection cannot recover away from the entrance prompt");
                ContentSetCatalog.MarkSewerSliceContractAccepted(fixture.State.StoryFlags);
                fixture.State.StoryFlags.Add(StoryFlags.SewerFoulRunoffCleared);
                fixture.Call("EnterMidgaardSewer");
                fixture.Choose("leave");
                Require(!RemembranceQuestRules.Recovered(fixture.State.StoryFlags), "leaving the entrance is harmless");
                if (acceptedAfterClear)
                {
                    fixture.Call("EnterMidgaardSewer");
                    fixture.Choose("contract");
                    Require(fixture.State.Mode == GameMode.Explore && !RemembranceQuestRules.Recovered(fixture.State.StoryFlags),
                        "continuing an already cleared contract does not claim the optional roll or replay combat");
                }
                fixture.Call("EnterMidgaardSewer");
                fixture.Choose("inspect");
                fixture.Reload();
                Require(RemembranceQuestRules.Recovered(fixture.State.StoryFlags), "explicit alcove recovery survives save/load");
                Require(fixture.State.Inventory.Count == 0, "roll uses quest records and never consumes or duplicates the sewer proof");

                fixture.Call("ShowSeraConversation");
                fixture.Choose("remembrance");
                fixture.Choose("read");
                fixture.Reload();
                Require(RemembranceQuestRules.NamesRead(fixture.State.StoryFlags), "transcribed names survive save/load");
                fixture.Call("ShowEddaConversation");
                fixture.Choose("remembrance");
                fixture.Choose(disposition);
                fixture.Reload();
                string choiceFlag = disposition == "private" ? StoryFlags.RemembrancePrivateChosen : StoryFlags.RemembrancePublicChosen;
                Require(fixture.State.StoryFlags.Contains(choiceFlag), "Edda's chosen remembrance survives save/load");

                ObjectType contact = disposition == "private" ? ObjectType.NoviceHealer : ObjectType.GateCaptain;
                ObjectType wrongContact = disposition == "private" ? ObjectType.GateCaptain : ObjectType.NoviceHealer;
                fixture.Call("ResolveRemembranceContactAction", wrongContact, "deliver");
                Require(!RemembranceQuestRules.Delivered(fixture.State.StoryFlags), "wrong contact cannot resolve the chosen disposition");
                fixture.OpenContact(contact);
                fixture.Choose("remembrance");
                fixture.Choose("deliver");
                fixture.Choose("later");
                fixture.Reload();
                Require(RemembranceQuestRules.Delivered(fixture.State.StoryFlags) && !RemembranceQuestRules.RewardClaimed(fixture.State.StoryFlags),
                    "deferring the keepsake saves completion without granting a reward");
                Require(fixture.State.Party[0].Experience == 0 && fixture.State.Inventory.Count == 0,
                    "delivery alone cannot farm XP or silently equip an item");

                // Either contact can retrieve the deferred keepsake; moral choice never gates gear.
                fixture.OpenContact(wrongContact);
                fixture.Choose("remembrance");
                fixture.Choose(reward);
                Require(fixture.State.Inventory.Count == 1 && fixture.State.Party[0].Experience == RemembranceQuestRules.ExperienceReward,
                    "production reward handler grants exactly one keepsake and the bounded XP award");
                InventoryItem item = fixture.State.Inventory[0];
                string expectedId = reward == "bow" ? SignatureItemCatalog.TravelersKnotId : SignatureItemCatalog.VigilkeeperWrapId;
                Require(SignatureItemCatalog.Identify(item)?.Id == expectedId, "correct independently selected signature reward reaches the pack");
                Require(!string.IsNullOrWhiteSpace(item.InstanceId) && string.IsNullOrEmpty(item.EquippedById),
                    "keepsake has durable identity and awaits deliberate equipping");
                string itemInstance = item.InstanceId;
                fixture.Reload();
                Require(fixture.State.Inventory[0].InstanceId == itemInstance, "reward instance identity survives the save service");
                fixture.Call("ResolveRemembranceRewardChoice", contact, reward == "bow" ? "wrap" : "bow");
                fixture.Call("ResolveRemembranceRewardChoice", contact, reward);
                fixture.Call("ResolveRemembranceContactAction", contact, "deliver");
                Require(fixture.State.Inventory.Count == 1 && fixture.State.Party[0].Experience == RemembranceQuestRules.ExperienceReward,
                    "replayed reward and delivery callbacks cannot duplicate gear or experience");
                Require(fixture.State.Gold == 31 && fixture.State.Supplies == 7 && fixture.State.Elixirs == 2,
                    "the optional favor cannot alter the resource economy");
                Require(fixture.State.ActiveStory == Fixture.CampaignObjective && fixture.State.StoryChapter == 1,
                    "the optional favor never takes ownership of campaign progression");

                fixture.Call("ShowEddaConversation");
                string greeting = fixture.Get<string>("dialogueBody");
                Require(greeting.Contains(disposition == "private" ? "place beside the names" : "Brann's notice"),
                    "Edda reacts to the selected completed remembrance");
                fixture.Call("CloseDialogue");
                IReadOnlyList<ArmoryRowView> rows = (IReadOnlyList<ArmoryRowView>)fixture.Call("BuildArmoryJournalRows");
                Require(rows.Any(row => row.Title == RemembranceQuestRules.Title && row.Subtitle.Contains("complete")),
                    "production journal presents saved side quest completion");
                Require(rows.Count > 1 && rows[0].Title != RemembranceQuestRules.Title, "optional journal entry preserves campaign-first ordering");
            }
        }

        private static void LegacyPrototypeClearedSewerCanBeInspected()
        {
            using (Fixture fixture = new Fixture())
            {
                fixture.State.ContentSetId = ContentSetCatalog.FullPrototype;
                fixture.Set("activeContentSet", ContentSetCatalog.FullPrototype);
                fixture.State.StoryFlags.Add(StoryFlags.MidgaardRatPeltArmorMade);
                fixture.Call("ShowEddaConversation");
                fixture.Choose("remembrance");
                fixture.Choose("accept");
                fixture.Call("EnterMidgaardSewer");
                fixture.Choose("inspect");
                Require(RemembranceQuestRules.Recovered(fixture.State.StoryFlags), "older prototype armor turn-in keeps the cleared alcove reachable");
            }
        }

        private sealed class Fixture : IDisposable
        {
            public const string CampaignObjective = "Chapter I: Preserve the main cistern objective.";
            private readonly GameObject host;
            private readonly AshenHallsGame game;
            private readonly string savePath;
            public GameState State => Get<GameState>("state");

            public Fixture()
            {
                host = new GameObject("Remembrance quest regression");
                host.SetActive(false);
                host.hideFlags = HideFlags.HideAndDontSave;
                game = host.AddComponent<AshenHallsGame>();
                Set("state", new GameState
                {
                    SaveVersion = VersionInfo.SaveVersion,
                    Mode = GameMode.Explore, Depth = 1, StoryChapter = 1, Seed = 91027,
                    Gold = 31, Supplies = 7, Elixirs = 2, ActiveStory = CampaignObjective,
                    ReducedMotion = true, SfxMuted = true, MusicMuted = true,
                    Party = new List<PartyMember>
                    {
                        new PartyMember { Id = "remembrance-test", Name = "Witness", Race = "ashling", ClassKey = "ranger", Role = "bow", Level = 1,
                            Stats = new Stats(10, 10, 18, 12), Hp = 28, MaxHp = 28, Skills = new SkillSet().Normalize(), WeaponName = "shortbow", ArmorName = "cloth" }
                    }
                });
                Set("activeContentSet", ContentSetCatalog.SewerSlice);
                Set("rng", new System.Random(91027));
                Set("visualSmokeSaveBlocked", true);
                Set("launchError", "Remembrance fixture holds UI presentation");
                savePath = Path.Combine(Path.GetTempPath(), "ashen-remembrance-" + Guid.NewGuid().ToString("N") + ".json");
            }

            public void OpenContact(ObjectType contact)
            {
                if (contact == ObjectType.NoviceHealer) Call("ShowSeraConversation");
                else Call("ShowBrannConversation", new object[] { null });
            }

            public void Choose(string id)
            {
                DialogueChoiceView[] choices = Get<DialogueChoiceView[]>("dialogueChoices");
                Require(choices != null && choices.Any(choice => choice.Id == id && choice.Enabled), "production dialogue exposes choice " + id);
                string[] pages = Get<string[]>("dialoguePages");
                for (int i = 0; i < (pages?.Length ?? 0) - 1; i++) Call("AdvanceDialogue");
                MethodInfo method = typeof(AshenHallsGame).GetMethod("ChooseDialogueChoice", Private, null, new[] { typeof(string) }, null);
                method.Invoke(game, new object[] { id });
            }

            public void Reload()
            {
                string before = JsonUtility.ToJson(State);
                SaveService.SaveGameState(savePath, State);
                GameState loaded = SaveService.LoadGameState(savePath, out bool backup);
                Require(!backup && JsonUtility.ToJson(loaded) == before, "production save service roundtrip preserves exact quest and campaign data");
                Call("CloseTransientOverlays");
                Set("state", loaded);
            }

            public object Call(string name, params object[] args)
            {
                MethodInfo method = typeof(AshenHallsGame).GetMethod(name, Private);
                Require(method != null, "runtime quest method exists: " + name);
                try { return method.Invoke(game, args); }
                catch (TargetInvocationException ex) { throw new InvalidOperationException("Quest production call failed: " + name, ex.InnerException); }
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
            if (!condition) throw new InvalidOperationException("Remembrance quest: " + message);
        }
    }
}
