using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class TownConversationSmoke
    {
        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " town conversation smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError("Town conversation smoke failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            ProgressDoesNotCongratulateUnfinishedWork();
            ServiceMemoryAndQuotesStayHonest();
            GreetingsKeepChoicesOnTheFirstPage();
            RoadNewsNamesTheActualQuestContact();
        }

        private static readonly NpcId[] speakers =
        {
            NpcId.Orren, NpcId.Kate, NpcId.Lute, NpcId.Borin, NpcId.Tessa, NpcId.Maud
        };

        private static readonly string[] progress =
        {
            StoryFlags.SewerContractAccepted, StoryFlags.SewerCisternDenCleared,
            StoryFlags.KoboldKingDefeated, StoryFlags.GloamWardenDefeated,
            StoryFlags.EmberglassGateKeyRecovered, StoryFlags.CrownroadMarshalDefeated
        };

        private static GameState State()
        {
            return new GameState
            {
                Gold = 240,
                Supplies = 6,
                Party = new List<PartyMember> { new PartyMember { Name = "Test", WeaponName = "iron sword" } }
            };
        }

        private static void ProgressDoesNotCongratulateUnfinishedWork()
        {
            Require(TownConversationCatalog.Stage(null) == TownStoryStage.Newcomers, "missing state has a safe introduction");
            GameState state = State();
            state.StoryFlags.AddRange(new[]
            {
                StoryFlags.SewerBrokenSluiceCleared, StoryFlags.KoboldCaveFound,
                StoryFlags.BoneRoadEntered, StoryFlags.GlassAndAshEntered, StoryFlags.RedGateEntered
            });
            Require(TownConversationCatalog.Stage(state) == TownStoryStage.Newcomers,
                "arriving at a chapter or clearing an early room does not announce its victory");
            for (int i = 0; i < progress.Length; i++)
            {
                state.StoryFlags.Add(progress[i]);
                Require((int)TownConversationCatalog.Stage(state) == i + 1,
                    "the newest completed milestone wins over older story flags");
            }
            state.StoryFlags.Reverse();
            Require(TownConversationCatalog.Stage(state) == TownStoryStage.RedGateVictory,
                "saved flag ordering cannot send the town back to an earlier chapter");
            foreach (string completedLegacyFlag in new[] { StoryFlags.MidgaardRatPeltArmorMade, StoryFlags.SewerRewardClaimed })
            {
                state.StoryFlags.Clear();
                state.StoryFlags.Add(completedLegacyFlag);
                Require(TownConversationCatalog.Stage(state) == TownStoryStage.CisternVictory,
                    "older completed sewer rewards still earn the town's recognition");
            }
        }

        private static void ServiceMemoryAndQuotesStayHonest()
        {
            GameState state = State();
            Require(TownConversationCatalog.Greeting(NpcId.Kate, state).Contains("12 gold"), "Kate quotes the actual cost");
            Require(TownConversationCatalog.Greeting(NpcId.Lute, state).Contains("10 gold"), "Lute quotes the actual cost");
            Require(TownConversationCatalog.Greeting(NpcId.Borin, state).Contains("28 gold"), "Borin quotes the actual cost");
            Require(TownConversationCatalog.Greeting(NpcId.Tessa, state).Contains("32 gold"), "Tessa quotes the actual cost");
            string maud = TownConversationCatalog.Greeting(NpcId.Maud, state);
            Require(maud.Contains(WeaponEnchantmentRules.TemporaryCost + " gold")
                && maud.Contains(WeaponEnchantmentRules.TemporaryVictories + " victories")
                && maud.Contains(WeaponEnchantmentRules.PermanentCost + " gold"), "Maud keeps both prices and the temporary duration explicit");
            foreach (string flag in new[] { StoryFlags.MidgaardKateBundleBought, StoryFlags.MidgaardProvisionBundleBought })
            {
                state.StoryFlags.Clear();
                state.StoryFlags.Add(flag);
                Require(!TownConversationCatalog.Greeting(NpcId.Kate, state).Contains("12 gold")
                    && !TownConversationCatalog.Greeting(NpcId.Lute, state).Contains("10 gold"),
                    "either endpoint recognizes the shared completed order instead of pitching unavailable food");
            }
            state.StoryFlags.AddRange(new[] { StoryFlags.MidgaardBasicArmorBought, StoryFlags.MidgaardBasicWeaponBought });
            Require(!TownConversationCatalog.Greeting(NpcId.Borin, state).Contains("28 gold")
                && !TownConversationCatalog.Greeting(NpcId.Tessa, state).Contains("32 gold"), "completed one-time equipment services stop selling the old offer");
            state.Party.Clear();
            Require(!TownConversationCatalog.Greeting(NpcId.Maud, state).Contains("gold"), "Maud does not offer work without a weapon");
            string before = JsonUtility.ToJson(state);
            foreach (NpcId speaker in speakers)
            {
                TownConversationCatalog.Greeting(speaker, state);
                TownConversationCatalog.PersonalStory(speaker);
            }
            TownConversationCatalog.OrrenRoadNews(state);
            TownConversationCatalog.OrrenCisternNews(state);
            Require(before == JsonUtility.ToJson(state), "reading authored content changes no persistent state");
        }

        private static void GreetingsKeepChoicesOnTheFirstPage()
        {
            GameState state = State();
            for (int stage = 0; stage <= progress.Length; stage++)
            {
                if (stage > 0) state.StoryFlags.Add(progress[stage - 1]);
                foreach (int gold in new[] { 0, 240, int.MaxValue })
                foreach (bool purchased in new[] { false, true })
                {
                    state.Gold = gold;
                    foreach (string flag in new[]
                    {
                        StoryFlags.MidgaardKateBundleBought, StoryFlags.MidgaardBasicArmorBought,
                        StoryFlags.MidgaardBasicWeaponBought, StoryFlags.MidgaardWeaponEnchanted
                    })
                    {
                        state.StoryFlags.Remove(flag);
                        if (purchased) state.StoryFlags.Add(flag);
                    }
                    foreach (NpcId speaker in speakers)
                    {
                        string greeting = TownConversationCatalog.Greeting(speaker, state);
                        Require(!string.IsNullOrWhiteSpace(greeting), "each town speaker has a greeting");
                        Require(DialoguePagingRules.Paginate(greeting).Length == 1,
                            speaker + " keeps services visible without paging, stage=" + stage + ", length=" + greeting.Length);
                    }
                }
            }
        }

        private static void RoadNewsNamesTheActualQuestContact()
        {
            GameState state = State();
            foreach (string flag in progress)
            {
                state.StoryFlags.Add(flag);
                if (TownConversationCatalog.Stage(state) < TownStoryStage.KoboldVictory) continue;
                string news = TownConversationCatalog.OrrenRoadNews(state);
                Require(news.Contains("Yara") && !news.Contains("Halvard"),
                    "later road planning and debrief advice points to Scout Yara");
            }
            Require(TownConversationCatalog.OrrenRoadNews(state).Contains("Salt Cistern Gate"),
                "defeating the Marshal does not skip the threshold survey");
            state.StoryFlags.Add(StoryFlags.MeteorCrownThresholdSurveyed);
            Require(TownConversationCatalog.OrrenRoadNews(state).Contains("Take your survey back"),
                "a completed threshold survey directs the party home to debrief");
            state.StoryFlags.Add(StoryFlags.RedGateDebriefed);
            Require(!TownConversationCatalog.OrrenRoadNews(state).Contains("Take your survey back"),
                "a completed debrief does not keep requesting the same report");
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
