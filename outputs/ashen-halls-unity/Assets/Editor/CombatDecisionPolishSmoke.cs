using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class CombatDecisionPolishSmoke
    {
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " combat decision polish smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogError("Combat decision polish smoke failed: " + exception);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            MovementFeedbackMatchesReadiness();
            SkillTargetsRespectReadiness();
            RejectedCursorExplainsTheProblem();
            HealingFeedbackReportsActualRecovery();
            RecoveryPreviewsRespectMissingHealth();
            BlockedBadgesAreSpecific();
        }

        private static void MovementFeedbackMatchesReadiness()
        {
            WithFixture((game, state, active, target) =>
            {
                Set(game, "selectedAction", ActionMode.Move);
                Require((bool)Invoke(game, "CombatBoardCursorCellIsLegal", active, 1, 2), "positive control: an open adjacent destination is legal");
                foreach (string condition in new[] { "Webbed", "Stunned", "Sleeping" })
                {
                    typeof(CombatUnit).GetField(condition).SetValue(active, 1);
                    string before = JsonUtility.ToJson(state);
                    string preview = (string)Invoke(game, "CombatMovementPreview", active, 1, 2);
                    string instruction = (string)Invoke(game, "HoverClickInstruction", active, null, null, 1, 2);
                    Require(preview.Contains(condition) && instruction.Contains(condition), condition + " is explicit in the preview and click instruction");
                    Require(!(bool)Invoke(game, "CombatBoardCursorCellIsLegal", active, 1, 2), condition + " excludes controller targets");
                    Require((int)Invoke(game, "CountReachableMoveDestinations", active) == 0, condition + " removes misleading reachable counts");
                    Require(JsonUtility.ToJson(state) == before, "movement feedback does not mutate combat state");
                    typeof(CombatUnit).GetField(condition).SetValue(active, 0);
                }
                state.Combat.MovePoints = 0;
                Require(((string)Invoke(game, "CombatMovementPreview", active, 1, 2)).Contains("No movement"), "spent movement reports its budget");
                state.Combat.MovePoints = 4;
                Require(((string)Invoke(game, "CombatMovementPreview", active, 1, 1)).Contains("Current tile"), "current tile is not advertised as a move");
                Require(((string)Invoke(game, "CombatMovementPreview", active, target.X, target.Y)).Contains("occupied"), "occupied destination is distinguished from sight blockers");
                state.Combat.Obstacles.Add(new Point(1, 2, "stone", 3));
                Require(((string)Invoke(game, "CombatMovementPreview", active, 1, 2)).Contains("terrain"), "blocking terrain is named");
            });
        }

        private static void SkillTargetsRespectReadiness()
        {
            WithFixture((game, state, active, target) =>
            {
                MartialAbility charge = AbilityCatalog.For("charge");
                Set(game, "selectedAction", ActionMode.Ability);
                Set(game, "pendingAbilityId", "charge");
                Require((bool)Invoke(game, "CombatBoardCursorCellIsLegal", active, target.X, target.Y), "positive control: Charge has a legal lane");
                active.Webbed = 1;
                string before = JsonUtility.ToJson(state);
                Require(!(bool)Invoke(game, "CombatBoardCursorCellIsLegal", active, target.X, target.Y), "webbed Charge is not in controller targets");
                Require((CombatTargetHighlightState)Invoke(game, "CombatTargetHighlightStateAt", active, null, charge, target.X, target.Y) == CombatTargetHighlightState.Blocked,
                    "webbed Charge does not advertise a legal passive highlight");
                Require(((string)Invoke(game, "HoverClickInstruction", active, target, null, target.X, target.Y)).Contains("Webbed"), "Charge instruction names its movement requirement");
                Require((int)Invoke(game, "CountLegalAbilityTargets", charge, active) == 0, "armed Charge count excludes unusable targets");
                Require(JsonUtility.ToJson(state) == before, "skill feedback does not mutate resources or targets");
                active.Webbed = 0;
                state.Combat.ActionAvailable = false;
                Require(!(bool)Invoke(game, "CombatBoardCursorCellIsLegal", active, target.X, target.Y), "a spent action has no skill target");
                Require(((string)Invoke(game, "HoverClickInstruction", active, target, null, target.X, target.Y)).Contains("Action already used"), "spent action does not promise Click to use");
            });
        }

        private static void RejectedCursorExplainsTheProblem()
        {
            WithFixture((game, state, active, target) =>
            {
                target.X = 8;
                Set(game, "selectedAction", ActionMode.Attack);
                Set(game, "combatBoardCursorActive", true);
                Set(game, "combatBoardCursorCell", (Vector2Int?)new Vector2Int(target.X, target.Y));
                int mana = active.Mana;
                int hp = target.Hp;
                Require((bool)Invoke(game, "ConfirmCombatBoardCursor", active), "invalid confirmation is handled");
                Require(state.Log.Any(entry => entry.Text.Contains("out of range")), "invalid cursor names the range problem");
                Require(state.Combat.ActionAvailable && !state.Combat.Acted && state.Combat.MovePoints == 4 && active.Mana == mana && target.Hp == hp,
                    "rejected cursor spends no action, move, mana, or health");
                target.X = 3;
                active.ClassKey = "mage";
                active.Spell = "ember";
                active.Mana = 0;
                Set(game, "selectedAction", ActionMode.Cast);
                Set(game, "pendingFormulaCode", "FBL");
                Require(((string)Invoke(game, "CombatTargetRejectionReason", active, target.X, target.Y)).Contains("mana"), "spell rejection names missing mana");
            });
        }

        private static void HealingFeedbackReportsActualRecovery()
        {
            WithFixture((game, state, caster, target) =>
            {
                caster.ClassKey = "priest";
                caster.Spell = "mend";
                target.Side = UnitSide.Party;
                target.Hp = 99;
                CombatUnit full = Ally("full", 4, 1, 100);
                CombatUnit nearlyFull = Ally("near-full", 3, 2, 99);
                CombatUnit wounded = Ally("wounded", 3, 0, 10);
                state.Combat.Units.AddRange(new[] { full, nearlyFull, wounded });
                FormulaDef formula = Array.Find(FormulaCatalog.All, entry => entry.Code == "LBC");
                Vector2Int range = (Vector2Int)Invoke(game, "FormulaHealPreview", formula, caster);
                int splashAmount = Math.Max(2, range.x / 2);
                string preview = (string)Invoke(game, "FormulaPreview", caster, formula, target, target.X, target.Y);
                Require(preview.Contains("+1-1 HP"), "near-full heal forecast promises only recoverable HP");
                Require((bool)Invoke(game, "ResolveFormula", formula, caster, target, target.X, target.Y), "Circle Heal resolves");
                Require(target.Hp == 100 && nearlyFull.Hp == 100 && full.Hp == 100, "healing retains normal HP caps");
                Require(wounded.Hp == 10 + splashAmount, "splash still uses the full rolled heal, not capped primary recovery");
                List<FloatText> floats = Get<List<FloatText>>(game, "floatTexts");
                Require(floats.Any(entry => entry.X == target.X && entry.Y == target.Y && entry.Text == "+1"), "primary float matches one HP actually restored");
                Require(floats.Any(entry => entry.X == nearlyFull.X && entry.Y == nearlyFull.Y && entry.Text == "+1"), "splash float matches one HP actually restored");
                Require(floats.Any(entry => entry.X == full.X && entry.Y == full.Y && entry.Text == "full HP" && entry.IconIndex == 2), "full ally gets explicit full HP feedback with heal icon");
                Require(state.Log.Any(entry => entry.Text.Contains(target.Name + " recovers 1 HP")), "heal log matches the health delta");
            });
        }

        private static void RecoveryPreviewsRespectMissingHealth()
        {
            Require(CombatFeedbackRules.RecoverableHealth(99, 100, 40) == 1, "recovery caps to missing health");
            Require(CombatFeedbackRules.RecoverableHealth(100, 100, 40) == 0, "full health recovers zero");
            Require(CombatFeedbackRules.RecoverableHealth(99, 100, -1) == 0, "negative recovery is not advertised");
            Require(CombatFeedbackRules.DrainRecovery(10, 100, 1, 80, 1) == 1, "Soul Rend preview caps overkill but retains its minimum");
            Require(CombatFeedbackRules.DrainRecovery(10, 100, 1, 80, 2) == 2, "Drain Life retains its authored minimum");
            Require(CombatFeedbackRules.DrainRecovery(100, 100, 40, 80, 2) == 0, "full caster does not promise drain recovery");
            WithFixture((game, state, caster, target) =>
            {
                caster.ClassKey = "warlock";
                caster.Spell = "pact";
                caster.Hp = 99;
                FormulaDef ascendance = Array.Find(FormulaCatalog.All, entry => entry.Code == "DFA");
                Require(((string)Invoke(game, "FormulaPreview", caster, ascendance, caster, caster.X, caster.Y)).Contains("heal 1 /"), "Ascendance forecast respects the HP cap");
                Require((bool)Invoke(game, "ResolveFormula", ascendance, caster, caster, caster.X, caster.Y), "Ascendance resolves");
                Require(caster.Hp == 100 && state.Log.Any(entry => entry.Text.Contains("recovers 1 HP")), "Ascendance log reports only actual healing");
                target.X = 2;
                target.Hp = 1;
                Set(game, "pendingAbilityId", "soulrend");
                Require(((string)Invoke(game, "AbilityPreview", caster, target, target.X, target.Y)).Contains("heal up to 0 HP"), "Soul Rend forecast respects a full caster");
            });
        }

        private static void BlockedBadgesAreSpecific()
        {
            Require(CombatTargetingRules.BlockedBadge("Choose an empty tile") == "TARGET", "empty is not mistaken for MP");
            Require(CombatTargetingRules.BlockedBadge("Needs 4 more MP (0/4)") == "MANA", "MP still identifies mana");
            Require(CombatTargetingRules.BlockedBadge("Webbed: Charge needs free movement") == "WEB", "webbing is not a sight block");
            Require(CombatTargetingRules.BlockedBadge("No open charge lane") == "PATH", "missing lane gets a path badge");
            Require(CombatTargetingRules.BlockedBadge("Target above 35% HP") == "HP", "Execute explains its health threshold");
            Require(CombatTargetingRules.BlockedBadge("Line of sight blocked") == "LOS", "sight blockage keeps its badge");
            Require(CombatTargetingRules.BlockedBadge("Needs adjacent target") == "RANGE", "adjacency is identified as range");
        }

        private static CombatUnit Ally(string id, int x, int y, int hp) => new CombatUnit
        {
            Id = id, Name = id, Side = UnitSide.Party, PartyIndex = -1,
            X = x, Y = y, Hp = hp, MaxHp = 100, Skills = new SkillSet()
        };

        private static void WithFixture(Action<AshenHallsGame, GameState, CombatUnit, CombatUnit> check)
        {
            GameObject host = new GameObject("Combat decision polish smoke");
            host.SetActive(false);
            host.hideFlags = HideFlags.HideAndDontSave;
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                CombatUnit active = Ally("decision-hero", 1, 1, 100);
                active.Level = 10;
                active.Range = 1;
                active.Mana = active.MaxMana = 100;
                active.Power = 20;
                active.ClassKey = "warrior";
                active.Skills = new SkillSet { Arms = 20, Mend = 20, Hex = 20, Ember = 20 };
                CombatUnit target = Ally("decision-target", 3, 1, 100);
                target.Side = UnitSide.Enemy;
                GameState state = new GameState
                {
                    Mode = GameMode.Combat, Depth = 1, ReducedMotion = true,
                    SfxMuted = true, MusicMuted = true,
                    Combat = new CombatState
                    {
                        ActiveId = active.Id, Phase = CombatPhase.ChooseTarget,
                        MovePoints = 4, ActionAvailable = true,
                        Units = new List<CombatUnit> { active, target }
                    }
                };
                Set(game, "state", state);
                Set(game, "rng", new MinimumRandom());
                Set(game, "launchError", "Combat decision fixture: presentation held");
                check(game, state, active, target);
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        private sealed class MinimumRandom : System.Random
        {
            public override int Next(int minValue, int maxValue) => minValue;
            public override double NextDouble() => 0;
        }

        private static object Invoke(object source, string name, params object[] values) => source.GetType().GetMethod(name, Private).Invoke(source, values);
        private static T Get<T>(object source, string name) => (T)source.GetType().GetField(name, Private).GetValue(source);
        private static void Set(object source, string name, object value) => source.GetType().GetField(name, Private).SetValue(source, value);
        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
