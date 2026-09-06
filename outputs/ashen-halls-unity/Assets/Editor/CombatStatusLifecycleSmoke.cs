using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class CombatStatusLifecycleSmoke
    {
        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " combat status lifecycle smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " combat status lifecycle smoke failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            foreach (UnitSide side in new[] { UnitSide.Party, UnitSide.Enemy })
            {
                WebLastsForAffectedTurns(side, 1);
                WebLastsForAffectedTurns(side, 2);
                FireStillFreesWebbedUnits(side);
                SkippedTurnsAlsoExpireWeb(side);
            }
            PinningShotAndFrostbindSurviveTurnStart();
            SteamChecksBothSides(0.9, 0, false);
            SteamChecksBothSides(0.1, 0, true);
            SteamChecksBothSides(0.1, 100, false);
        }

        private static void WebLastsForAffectedTurns(UnitSide side, int duration)
        {
            using (Fixture fixture = new Fixture(side))
            {
                CombatUnit active = fixture.Active;
                Require((bool)fixture.Invoke("TryApplyStatus", active, "web", duration, fixture.Source, 1f, true),
                    "the authored web effect applies");
                for (int turn = duration; turn > 0; turn--)
                {
                    fixture.BeginAffectedTurn();
                    Require(active.Webbed == turn, "web retains its current turn at start: " + side);
                    Require(fixture.State.Combat.MovePoints == 0, "web blocks movement: " + side);
                    Require(fixture.State.Combat.ActionAvailable, "web preserves the attack/spell action: " + side);
                    fixture.Controller.RepairActiveTurnState(active, side == UnitSide.Enemy);
                    Require(fixture.State.Combat.MovePoints == 0, "turn repair cannot erase a pin: " + side);
                    fixture.CompleteAffectedTurn();
                    Require(active.Webbed == turn - 1, "web expires exactly once on turn completion: " + side);
                    Require(fixture.State.Combat.ActiveId == fixture.Observer.Id, "completion reaches the next combatant");
                }
                fixture.BeginAffectedTurn();
                Require(fixture.State.Combat.MovePoints == 3, "movement returns after the authored web duration: " + side);
            }
        }

        private static void FireStillFreesWebbedUnits(UnitSide side)
        {
            using (Fixture fixture = new Fixture(side))
            {
                fixture.Active.Webbed = 1;
                fixture.State.Combat.Obstacles.Add(new Point(fixture.Active.X, fixture.Active.Y, "fire", 2));
                fixture.BeginAffectedTurn();
                Require(fixture.Active.Webbed == 0, "fire clears the pin immediately: " + side);
                Require(fixture.State.Combat.MovePoints == 3, "fire restores movement for the current turn: " + side);
                Require(fixture.State.Combat.ActionAvailable, "fire-cleared unit keeps its action: " + side);
            }
        }

        private static void SkippedTurnsAlsoExpireWeb(UnitSide side)
        {
            using (Fixture fixture = new Fixture(side))
            {
                fixture.Active.Webbed = 1;
                fixture.Active.Stunned = 1;
                fixture.Invoke("BeginQueuedCombatTurn", fixture.Active);
                Require(fixture.Active.Webbed == 0 && fixture.Active.Stunned == 0,
                    "a stunned turn consumes both authored status durations: " + side);
                Require(fixture.State.Combat.ActiveId == fixture.Observer.Id,
                    "a stunned webbed unit advances to the next combatant: " + side);
            }
        }

        private static void PinningShotAndFrostbindSurviveTurnStart()
        {
            using (Fixture fixture = new Fixture(UnitSide.Enemy))
            {
                fixture.Source.ClassKey = "ranger";
                fixture.Source.Role = "bow";
                Require((bool)fixture.Invoke("UsePinningShot", fixture.Source, fixture.Active), "Pinning Shot resolves");
                Require(fixture.Active.Webbed == 1, "melee Pinning Shot grants its authored one-turn pin");
                fixture.BeginAffectedTurn();
                Require(fixture.State.Combat.MovePoints == 0 && fixture.State.Combat.ActionAvailable,
                    "melee Pinning Shot blocks movement while allowing an attack");
            }
            using (Fixture fixture = new Fixture(UnitSide.Enemy))
            {
                fixture.Active.Bleeding = 2;
                FormulaDef formula = new FormulaDef { Code = "FROSTBIND-TEST", Name = "Frostbind test", DamageType = "cold" };
                fixture.Invoke("ApplyFormulaStatusResonance", formula, fixture.Source, fixture.Active);
                Require(fixture.Active.Webbed == 1, "frostbind grants a one-turn pin");
                fixture.BeginAffectedTurn();
                Require(fixture.State.Combat.MovePoints == 0, "frostbind still binds when the target's turn begins");
            }
        }

        private static void SteamChecksBothSides(double roll, int resistance, bool expectStun)
        {
            using (Fixture fixture = new Fixture(UnitSide.Party))
            {
                fixture.SetField("rng", new FixedRandom(roll));
                fixture.Active.X = 1;
                fixture.Active.Y = 2;
                fixture.Source.X = 3;
                fixture.Source.Y = 2;
                fixture.Observer.Side = UnitSide.Party;
                fixture.Observer.X = 2;
                fixture.Observer.Y = 1;
                foreach (CombatUnit unit in fixture.State.Combat.Units) unit.MagicResist = resistance;
                FormulaDef formula = new FormulaDef { Code = "STEAM-TEST", Name = "Steam test", Terrain = "ice" };
                fixture.Invoke("ApplyTerrainPlacementReaction", formula, fixture.Active, 2, 2, new Point(2, 2, "fire", 2));
                foreach (CombatUnit unit in fixture.State.Combat.Units)
                {
                    Require((unit.Stunned > 0) == expectStun,
                        "steam applies the chance/resistance rule to caster, ally and enemy: " + unit.Name);
                }
            }
        }

        private sealed class Fixture : IDisposable
        {
            private readonly GameObject host;
            private readonly AshenHallsGame game;
            public readonly CombatUnit Active;
            public readonly CombatUnit Source;
            public readonly CombatUnit Observer;
            public readonly GameState State;
            public readonly CombatController Controller;

            public Fixture(UnitSide activeSide)
            {
                host = new GameObject("Combat status lifecycle regression host");
                host.SetActive(false);
                host.hideFlags = HideFlags.HideAndDontSave;
                game = host.AddComponent<AshenHallsGame>();
                Active = Unit("affected", activeSide, 1, 1);
                Source = Unit("source", activeSide == UnitSide.Party ? UnitSide.Enemy : UnitSide.Party, 5, 4);
                Observer = Unit("observer", UnitSide.Enemy, 7, 5);
                State = new GameState
                {
                    Mode = GameMode.Combat, Depth = 1, ReducedMotion = true, SfxMuted = true, MusicMuted = true,
                    Party = new List<PartyMember> { new PartyMember { MaxHp = 1000, Hp = 1000, Skills = new SkillSet() } },
                    Combat = new CombatState
                    {
                        Round = 1, Units = new List<CombatUnit> { Active, Source, Observer },
                        InitiativeQueue = new List<string> { Active.Id, Observer.Id, Source.Id }
                    }
                };
                SetField("state", State);
                SetField("rng", new FixedRandom(0.1));
                Controller = (CombatController)Invoke("CombatLifecycle");
            }

            public void BeginAffectedTurn()
            {
                Controller.BeginTurn(Active, Active.Side == UnitSide.Enemy);
                Require(!(bool)Invoke("ApplyStartTurnEffects", Active), "web does not skip the unit's action");
            }

            public void CompleteAffectedTurn()
            {
                Controller.CompleteAction(Active, false);
                Invoke("CompleteCombatActionAdvance", Active);
            }

            public object Invoke(string name, params object[] arguments)
            {
                MethodInfo method = typeof(AshenHallsGame).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
                Require(method != null, "runtime method exists: " + name);
                return method.Invoke(game, arguments);
            }

            public void SetField(string name, object value)
            {
                FieldInfo field = typeof(AshenHallsGame).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
                Require(field != null, "runtime field exists: " + name);
                field.SetValue(game, value);
            }

            public void Dispose() => UnityEngine.Object.DestroyImmediate(host);

            private static CombatUnit Unit(string id, UnitSide side, int x, int y)
            {
                return new CombatUnit
                {
                    Id = id, Name = id, Side = side, PartyIndex = side == UnitSide.Party ? 0 : -1,
                    X = x, Y = y, Hp = 1000, MaxHp = 1000, Movement = 3, Range = 1,
                    Level = 1, Power = 4, DamageMin = 2, DamageMax = 4, Skills = new SkillSet(),
                    ClassKey = side == UnitSide.Party ? "warrior" : "rat", Role = "arms"
                };
            }
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private sealed class FixedRandom : System.Random
        {
            private readonly double roll;
            public FixedRandom(double roll) { this.roll = roll; }
            public override int Next(int maxValue) => 0;
            public override int Next(int minValue, int maxValue) => minValue;
            public override double NextDouble() => roll;
        }
    }
}
