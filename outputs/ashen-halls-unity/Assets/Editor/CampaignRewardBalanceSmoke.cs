using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class CampaignRewardBalanceSmoke
    {
        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " campaign reward balance smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogError(VersionInfo.ProductName + " campaign reward balance smoke failed: " + exception);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            AuthoredEnemyExperienceStaysStable();
            ReinforcementsDoNotGenerateRewards();
            RealRitualBreachesPreserveEncounterRewards();
        }

        private static void AuthoredEnemyExperienceStaysStable()
        {
            CombatUnit rat = Enemy("ratfolk", 18, 4, 1, 1);
            CombatUnit veteran = Enemy("drowcrossbow", 32, 7, 2, 5);
            veteran.Rank = "veteran";
            CombatUnit elite = Enemy("bonepriest", 40, 9, 3, 4);
            elite.Rank = "elite";
            CombatUnit[] enemies = { rat, veteran, elite };
            Require(CombatRewardRules.ExperienceForVictory(enemies, 4) == 43,
                "ordinary, veteran, elite, ranged, and caster XP retain the authored 173 / 4 reward");
            Require(CombatRewardRules.ExperienceForVictory(enemies, 1) == 173,
                "party-size sharing retains all original enemy difficulty bonuses");
            Require(CombatRewardRules.ExperienceForVictory(new[] { rat }, 4) == 12,
                "small encounters keep their minimum 12 XP reward");
            Require(CombatRewardRules.ExperienceForVictory(null, 4) == 0,
                "absent combat cannot award experience");
        }

        private static void ReinforcementsDoNotGenerateRewards()
        {
            CombatUnit[] reinforcements =
            {
                Enemy("gloamknight", 200, 40, 20, 1),
                Enemy("bonepriest", 200, 40, 20, 5),
                Enemy("lesserdemon", 200, 40, 20, 1),
                Enemy("koboldraider", 200, 40, 20, 1)
            };
            reinforcements[0].Origin = "ritual";
            reinforcements[1].Rank = " RITUAL ";
            reinforcements[2].Summoned = true;
            reinforcements[3].Side = UnitSide.Party;
            foreach (CombatUnit reinforcement in reinforcements)
            {
                Require(!CombatRewardRules.IsEncounterRewardEnemy(reinforcement),
                    "ritual origin, legacy ritual rank, summons, and party allies cannot enter the reward pool");
            }
            Require(!CombatRewardRules.IsEncounterRewardEnemy(null), "missing units cannot enter the reward pool");
            Require(CombatRewardRules.ExperienceForVictory(reinforcements, 4) == 0,
                "a reinforcement-only battle cannot generate minimum XP");

            CombatUnit boss = Enemy("ritualheart", 100, 16, 6, 5);
            Require(CombatRewardRules.IsEncounterRewardEnemy(boss),
                "the authored Ritual Heart boss remains eligible; its role is not a reinforcement marker");
            Require(CombatRewardRules.ExperienceForVictory(new[] { boss }, 4) == 30,
                "the Ritual Heart keeps its full boss and caster experience");
        }

        private static void RealRitualBreachesPreserveEncounterRewards()
        {
            GameObject host = new GameObject("Campaign reward balance smoke");
            host.SetActive(false);
            host.hideFlags = HideFlags.HideAndDontSave;
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                CombatUnit original = Enemy("ratfolk", 18, 4, 1, 1);
                GameState state = new GameState
                {
                    Mode = GameMode.Combat,
                    Depth = 3,
                    Seed = 727,
                    ReducedMotion = true,
                    SfxMuted = true,
                    MusicMuted = true,
                    Party = new List<PartyMember>
                    {
                        new PartyMember(), new PartyMember(), new PartyMember(), new PartyMember()
                    },
                    Combat = new CombatState
                    {
                        EncounterStyle = "gloam-warden-boss",
                        Units = new List<CombatUnit> { original }
                    }
                };
                Set(game, "state", state);
                Set(game, "rng", new System.Random(727));
                int originalExperience = (int)Invoke(game, "CombatExperienceReward");
                List<CombatUnit> ordinaryEnemies = new List<CombatUnit>(state.Combat.Units);

                foreach (string kind in new[] { "glyph", "demonrift", "glyph" })
                {
                    Point ritual = new Point(7, 4, kind, 1);
                    state.Combat.Obstacles.Add(ritual);
                    Require((bool)Invoke(game, "TryOpenEnemyRitual", ritual), "production ritual opens: " + kind);
                    CombatUnit reinforcement = state.Combat.Units.Last();
                    Require(reinforcement.Origin == "ritual" && reinforcement.Rank == "ritual",
                        "production ritual marks the reinforcement durably");
                    reinforcement.Hp = 0;
                }

                Require((int)Invoke(game, "CombatExperienceReward") == originalExperience,
                    "allowing and defeating three actual ritual breaches does not outpay early disruption");
                List<CombatUnit> breachedEnemies = new List<CombatUnit>(state.Combat.Units);
                int generatedItems = 0;
                int emptyRolls = 0;
                for (int seed = 0; seed < 64; seed++)
                {
                    state.Combat.Units = ordinaryEnemies;
                    Set(game, "rng", new System.Random(seed));
                    InventoryItem ordinaryLoot = (InventoryItem)Invoke(game, "MakeCombatLootItem", "patrol", false);
                    state.Combat.Units = breachedEnemies;
                    Set(game, "rng", new System.Random(seed));
                    InventoryItem breachedLoot = (InventoryItem)Invoke(game, "MakeCombatLootItem", "patrol", false);
                    Require((ordinaryLoot == null) == (breachedLoot == null),
                        "breaches do not raise random gear drop chance at seed " + seed);
                    if (ordinaryLoot == null)
                    {
                        emptyRolls++;
                        continue;
                    }
                    generatedItems++;
                    Require(JsonUtility.ToJson(ordinaryLoot) == JsonUtility.ToJson(breachedLoot),
                        "breaches do not change gear source, type, or power at seed " + seed);
                }
                Require(generatedItems > 0 && emptyRolls > 0,
                    "the deterministic loot matrix exercises both item and no-item outcomes");

                GameState resumed = JsonUtility.FromJson<GameState>(JsonUtility.ToJson(state));
                Set(game, "state", resumed);
                Require((int)Invoke(game, "CombatExperienceReward") == originalExperience,
                    "saving and resuming a breached encounter preserves reward eligibility");
                resumed.Combat.Units.RemoveAt(0);
                Set(game, "rng", new System.Random(0));
                Require((int)Invoke(game, "CombatExperienceReward") == 0,
                    "only reinforcement corpses cannot produce XP");
                Require(Invoke(game, "MakeCombatLootItem", "patrol", false) == null,
                    "only reinforcement corpses cannot produce equipment");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(host);
            }
        }

        private static CombatUnit Enemy(string role, int hp, int power, int defense, int range)
        {
            return new CombatUnit
            {
                Id = "reward-" + role,
                Name = role,
                Side = UnitSide.Enemy,
                PartyIndex = -1,
                Role = role,
                Hp = 0,
                MaxHp = hp,
                Power = power,
                Defense = defense,
                Range = range,
                DamageType = "physical",
                Skills = new SkillSet()
            };
        }

        private static object Invoke(AshenHallsGame game, string name, params object[] arguments)
        {
            MethodInfo method = typeof(AshenHallsGame).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Require(method != null, "runtime method exists: " + name);
            return method.Invoke(game, arguments);
        }

        private static void Set(AshenHallsGame game, string name, object value)
        {
            FieldInfo field = typeof(AshenHallsGame).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Require(field != null, "runtime field exists: " + name);
            field.SetValue(game, value);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("Campaign reward balance: " + message);
        }
    }
}
