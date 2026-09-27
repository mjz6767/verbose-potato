using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class CampaignMilestoneSmoke
    {
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

        public static void RunOrThrow()
        {
            Require(CampaignMilestoneRules.All.Sum(item => item.Experience) == 2650, "authored reward budget");
            Require(!CampaignMilestoneRules.Pending(null).Any(), "empty history earns no chapters");
            using (Fixture fixture = new Fixture())
            {
                fixture.Call("ApplyCampaignMilestoneRewards");
                Require(fixture.TotalXp == 0, "new game has no milestone credit");
                fixture.State.StoryFlags.Add(StoryFlags.SewerRewardClaimed);
                fixture.Call("AutosaveCheckpoint", "milestone regression");
                Require(fixture.TotalXp == 100, "actual checkpoint pays first earned chapter even in isolated batch fixture");
                fixture.Call("AutosaveCheckpoint", "duplicate callback");
                Require(fixture.TotalXp == 100, "checkpoint repetition cannot farm XP");
                foreach (CampaignMilestone item in CampaignMilestoneRules.All)
                    if (!fixture.State.StoryFlags.Contains(item.CompletedFlag)) fixture.State.StoryFlags.Add(item.CompletedFlag);
                fixture.Call("EnsureWorldState", 27);
                Require(fixture.TotalXp == 2650, "old v27 completed flags catch up exactly once on world repair");
                int skills = fixture.State.Party[0].SkillPoints;
                int stats = fixture.State.Party[0].StatPoints;
                fixture.Reload();
                fixture.Call("EnsureWorldState", 27);
                fixture.Call("AutosaveCheckpoint", "reloaded receipt");
                Require(fixture.TotalXp == 2650 && fixture.State.Party[0].SkillPoints == skills
                    && fixture.State.Party[0].StatPoints == stats, "real save receipts preserve XP and growth without duplicate credit");
            }
            using (Fixture fixture = new Fixture())
            {
                fixture.State.StoryFlags.Add(StoryFlags.SewerRewardClaimed);
                fixture.State.Mode = GameMode.Combat;
                fixture.Call("EnsureWorldState", 27);
                Require(fixture.TotalXp == 0 && CampaignMilestoneRules.Pending(fixture.State.StoryFlags).Any(), "mid-combat loading defers credit and leaves combat untouched");
                fixture.State.Mode = GameMode.Explore;
                fixture.State.Party[0].Hp = 0;
                fixture.Call("ApplyCampaignMilestoneRewards");
                Require(fixture.TotalXp == 0 && fixture.State.Party[0].Hp == 0, "all-fallen party defers credit without resurrection");
                fixture.State.Party[0].Hp = 1;
                fixture.Set("activeContentSet", ContentSetCatalog.FullPrototype);
                fixture.Call("ApplyCampaignMilestoneRewards");
                Require(fixture.TotalXp == 0, "prototype progression stays unchanged");
                fixture.Set("activeContentSet", ContentSetCatalog.SewerSlice);
                fixture.Set("betaLabMode", true);
                fixture.Call("ApplyCampaignMilestoneRewards");
                Require(fixture.TotalXp == 0, "lab does not pay chapter credit");
                fixture.Set("betaLabMode", false);
                fixture.Call("ApplyCampaignMilestoneRewards");
                Require(fixture.TotalXp == 100, "deferred credit pays when normal exploration resumes");
            }
        }

        private sealed class Fixture : IDisposable
        {
            private readonly GameObject host;
            private readonly AshenHallsGame game;
            private readonly string savePath = Path.Combine(Path.GetTempPath(), "ashen-milestone-" + Guid.NewGuid().ToString("N") + ".json");
            public GameState State => (GameState)typeof(AshenHallsGame).GetField("state", Private).GetValue(game);
            public int TotalXp => ProgressionRules.CumulativeExperienceToLevel(State.Party[0].Level) + State.Party[0].Experience;
            public Fixture()
            {
                host = new GameObject("Campaign milestone regression");
                host.SetActive(false);
                host.hideFlags = HideFlags.HideAndDontSave;
                game = host.AddComponent<AshenHallsGame>();
                Set("state", new GameState { SaveVersion = 27, ContentSetId = ContentSetCatalog.SewerSlice,
                    Mode = GameMode.Explore, Depth = 1, Seed = 2330, SfxMuted = true, MusicMuted = true, ReducedMotion = true,
                    Party = new List<PartyMember> { new PartyMember { Id = "milestone", Name = "Witness", Race = "ashling", ClassKey = "ranger", Role = "bow",
                        Level = 1, Hp = 28, MaxHp = 28, Stats = new Stats(10, 10, 18, 12), Skills = new SkillSet().Normalize(), WeaponName = "shortbow", ArmorName = "cloth" } } });
                Set("activeContentSet", ContentSetCatalog.SewerSlice);
                Set("visualSmokeSaveBlocked", true);
                Set("rng", new System.Random(2330));
            }
            public object Call(string name, params object[] args) => typeof(AshenHallsGame).GetMethods(Private)
                .Single(method => method.Name == name && method.GetParameters().Length == args.Length).Invoke(game, args);
            public void Set(string name, object value) => typeof(AshenHallsGame).GetField(name, Private).SetValue(game, value);
            public void Reload()
            {
                SaveService.SaveGameState(savePath, State);
                Set("state", SaveService.LoadGameState(savePath, out bool usedBackup));
                Require(!usedBackup, "primary milestone save is readable");
            }
            public void Dispose()
            {
                UnityEngine.Object.DestroyImmediate(host);
                foreach (string suffix in new[] { "", ".bak", ".tmp" })
                    if (File.Exists(savePath + suffix)) File.Delete(savePath + suffix);
            }
        }
        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("Campaign milestone: " + message);
        }
    }
}
