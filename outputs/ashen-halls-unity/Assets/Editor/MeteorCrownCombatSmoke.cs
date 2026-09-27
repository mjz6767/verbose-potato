using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class MeteorCrownCombatSmoke
    {
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
        public static void Run()
        {
            try { RunOrThrow(); Debug.Log("Meteor Crown boss combat smoke passed."); EditorApplication.Exit(0); }
            catch (Exception error) { Debug.LogError("Meteor Crown boss combat smoke failed: " + error); EditorApplication.Exit(1); }
        }

        public static void RunOrThrow()
        {
            GeneratedCampaignEncountersExcludeUniqueBosses();
            Require(CombatPowerPresentationRules.EnemyPowerTitle("crownfire") == "Crownfire"
                && CombatPowerPresentationRules.EnemyPowerFormulaArtCode("crownfire") == "FBL",
                "Crownfire has its own readable cue using established fireball art");
            Require(CombatPowerPresentationRules.EnemyPowerTitle("heartpulse") == "Heart Pulse"
                && CombatPowerPresentationRules.EnemyPowerFormulaArtCode("heartpulse") == "RBT",
                "Heart Pulse has a focused bolt cue rather than an area explosion");
            foreach (string role in new[] { "meteorlich", "ritualheart" })
            {
                WithFixture(role, (game, state, boss, target, adjacent, distant) =>
                {
                    target.X = 0;
                    Require(!(bool)Call(game, "TryMeteorCrownBossSpecial", boss, target), role + " respects range");
                    target.X = 5;
                    state.Combat.Obstacles.Add(new Point(6, 3, "stone"));
                    Require(!(bool)Call(game, "TryMeteorCrownBossSpecial", boss, target), role + " cannot cast through stone");
                    Require(!(bool)Call(game, "CanEnemySpecialReach", boss, target), role + " threat forecast respects the same cover");
                    Require(target.Hp == 100 && adjacent.Hp == 100, "blocked cast has no damage side effects");
                    state.Combat.Obstacles.Clear();
                    target.Hp = 0;
                    Require(!(bool)Call(game, "TryMeteorCrownBossSpecial", boss, target), role + " ignores fallen targets");
                });
            }
            int ordinaryDamage = CrownfireDamage("");
            int resistantDamage = CrownfireDamage("fire");
            Require(resistantDamage < ordinaryDamage, "Crownfire uses production elemental resistance");
            WithFixture("meteorlich", (game, state, boss, target, adjacent, distant) =>
            {
                boss.Power = 999;
                int count = state.Combat.Units.Count;
                Require((bool)Call(game, "TryMeteorCrownBossSpecial", boss, target), "Crownfire resolves");
                Require(target.Hp >= 82 && target.Hp < 100, "Crownfire primary raw damage caps at18");
                Require(adjacent.Hp >= 94 && adjacent.Hp < 100, "Crownfire adjacency spill caps at6");
                Require(distant.Hp == 100, "separated hero avoids Crownfire spill");
                Require(state.Combat.Units.Count == count && state.Combat.Obstacles.Count == 0,
                    "Crownfire cannot generate reinforcements or persistent fire");
                Require(boss.X == 8 && boss.Y == 3 && target.Hexed == 0 && target.Stunned == 0,
                    "Crownfire creates no surprise movement or disable");
            });
            WithFixture("ritualheart", (game, state, boss, target, adjacent, distant) =>
            {
                boss.Power = 999;
                boss.Hp = 40;
                Require((bool)Call(game, "TryMeteorCrownBossSpecial", boss, target), "Heart Pulse resolves");
                Require(target.Hp >= 86 && target.Hp < 100, "Heart Pulse raw damage caps at14");
                Require(boss.Hp > 40 && boss.Hp <= 43, "heart restores at most3 health");
                Require(adjacent.Hp == 100 && distant.Hp == 100, "heart pulse is focused, not whole-party damage");
                Require(boss.Shielded == 0 && target.Hexed == 0 && target.Stunned == 0,
                    "heart pulse cannot stack shields or disable heroes");
                target.Hp = 1;
                boss.Hp = 40;
                Call(game, "TryMeteorCrownBossSpecial", boss, target);
                Require(target.Hp == 0 && boss.Hp == 40, "overkill cannot manufacture healing");
                target.Hp = 100;
                boss.Hp = boss.MaxHp - 1;
                Call(game, "TryMeteorCrownBossSpecial", boss, target);
                Require(boss.Hp == boss.MaxHp, "pulse healing respects max HP");
            });
            WithFixture("meteorlich", (game, state, boss, target, adjacent, distant) =>
            {
                // Execute the real enemy action dispatcher. An action that handled
                // the special must return without also adding an ordinary attack.
                int before = target.Hp;
                Call(game, "EnemyAct", boss);
                Require(before - target.Hp > 0 && before - target.Hp <= 18,
                    "one production enemy action cannot double-hit after Crownfire");
            });
            WithFixture("ritualheart", (game, state, boss, target, adjacent, distant) =>
            {
                Require((int)Call(game, "UnitMoveAllowance", boss) == 0, "Heart has zero movement in shared movement and pressure rules");
                Require(!(bool)Call(game, "IsEnemyPressureThreatAt", boss, target, 3, 3),
                    "Heart pressure preview excludes a clear target five tiles away");
                Require((bool)Call(game, "IsEnemyPressureThreatAt", boss, target, 4, 3),
                    "Heart pressure preview includes a clear target at its four-tile reach");
                Set(game, "rng", new MaximumRandom());
                target.X = 1;
                adjacent.X = 1;
                distant.X = 0;
                Call(game, "EnemyAct", boss);
                Require(boss.X == 8 && boss.Y == 3, "the Heart stays anchored when its special fails and all targets are distant");
                Require(target.Hp == 100 && adjacent.Hp == 100 && distant.Hp == 100,
                    "the anchored Heart cannot attack outside its reach");
            });
        }

        private static int CrownfireDamage(string resist)
        {
            int damage = 0;
            WithFixture("meteorlich", (game, state, boss, target, adjacent, distant) =>
            {
                target.Resist = resist;
                Call(game, "TryMeteorCrownBossSpecial", boss, target);
                damage = 100 - target.Hp;
            });
            return damage;
        }

        private static void GeneratedCampaignEncountersExcludeUniqueBosses()
        {
            string[] zones =
            {
                "", null, "unknown-zone", "midgaard-throne-room", "midgaard-grand-hearth",
                "midgaard-merchant-hall", "midgaard-city", "midgaard-road", "old-quarry",
                "glass-warrens", "ash-fen", "red-gate", "gloam-courts", "salt-cisterns",
                "green-shrine-road", "dusk-market", "inner-ash-road"
            };
            HashSet<string> ordinaryEnemies = new HashSet<string>
            {
                "sewerrat", "giantrat", "ratfolk", "ratcutthroat", "ratmage", "ratbrute",
                "koboldraider", "koboldslinger", "koboldshield", "koboldshaman",
                "drowscout", "drowcrossbow", "drowmage", "drowpriest", "husk", "reaver",
                "shade", "bonepriest", "gloamknight", "glassmage", "cinderling", "lesserdemon"
            };
            WithFixture("meteorlich", (game, state, boss, target, adjacent, distant) =>
            {
                Set(game, "activeContentSet", ContentSetCatalog.SewerSlice);
                for (int depth = 1; depth <= 6; depth++)
                foreach (string zone in zones)
                {
                    string[] pool = (string[])Call(game, "EnemyPoolForDepth", depth, zone);
                    Require(pool.Length > 0 && Array.IndexOf(pool, "meteorlich") < 0 && Array.IndexOf(pool, "ritualheart") < 0,
                        "generated chapter " + depth + " pool excludes both unique bosses in " + (zone ?? "null"));
                    foreach (string id in pool)
                        Require(ordinaryEnemies.Contains(id), "generated campaign pool keeps its ordinary roster: " + id);
                    if (depth >= 4)
                        Require(pool.Length == 22 && ordinaryEnemies.SetEquals(pool),
                            "later generated encounters retain all 22 original ordinary enemy choices");
                }
                Require(Array.IndexOf(EncounterCatalog.For(EncounterId.MeteorCrownHeart).EnemyIds, "ritualheart") >= 0
                    && Array.IndexOf(EncounterCatalog.For(EncounterId.MeteorCrownSovereign).EnemyIds, "meteorlich") >= 0,
                    "unique bosses remain in their authored campaign battles");
            });
        }

        private static void WithFixture(string role, Action<AshenHallsGame, GameState, CombatUnit, CombatUnit, CombatUnit, CombatUnit> action)
        {
            GameObject host = new GameObject("Meteor Crown combat fixture");
            host.SetActive(false);
            host.hideFlags = HideFlags.HideAndDontSave;
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                CombatUnit boss = Unit(role, UnitSide.Enemy, 8, 3);
                boss.Role = role;
                boss.Power = role == "meteorlich" ? 22 : 19;
                boss.Range = role == "meteorlich" ? 5 : 4;
                boss.DamageType = role == "meteorlich" ? "fire" : "death";
                CombatUnit target = Unit("target", UnitSide.Party, 5, 3);
                CombatUnit adjacent = Unit("adjacent", UnitSide.Party, 5, 4);
                CombatUnit distant = Unit("distant", UnitSide.Party, 3, 6);
                GameState state = new GameState
                {
                    Mode = GameMode.Combat, Depth = 6, ReducedMotion = true, SfxMuted = true, MusicMuted = true,
                    Combat = new CombatState { ActiveId = boss.Id, Units = new List<CombatUnit> { boss, target, adjacent, distant },
                        Phase = CombatPhase.ChooseAction, ActionAvailable = true }
                };
                Set(game, "state", state);
                Set(game, "rng", new MinimumRandom());
                Set(game, "launchError", "Boss combat fixture: presentation held");
                action(game, state, boss, target, adjacent, distant);
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }
        private static CombatUnit Unit(string id, UnitSide side, int x, int y) => new CombatUnit
        {
            Id = id, Name = id, Side = side, PartyIndex = -1, X = x, Y = y, Hp = 100, MaxHp = 100,
            Skills = new SkillSet(), DamageType = "physical"
        };
        private sealed class MinimumRandom : System.Random
        {
            public override int Next(int minValue, int maxValue) => minValue;
            public override double NextDouble() => 0;
        }
        private sealed class MaximumRandom : System.Random
        {
            public override int Next(int minValue, int maxValue) => maxValue - 1;
            public override double NextDouble() => 0.999;
        }
        private static object Call(object source, string name, params object[] values)
        {
            MethodInfo method = source.GetType().GetMethod(name, Private);
            Require(method != null, "production method exists: " + name);
            return method.Invoke(source, values);
        }
        private static void Set(object source, string name, object value) => source.GetType().GetField(name, Private).SetValue(source, value);
        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("Meteor Crown combat: " + message);
        }
    }
}
