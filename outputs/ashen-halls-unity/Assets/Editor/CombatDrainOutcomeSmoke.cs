using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class CombatDrainOutcomeSmoke
    {
        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " combat drain/outcome smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " combat drain/outcome smoke failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            DrainUsesHealthActuallyRemoved();
            VerifyDrain(false, 1, 10, 2, true);
            VerifyDrain(false, 1000, 10, null, true);
            BeneficialStatusRemovalIsNotACure();
            HarmfulStatusRemovalStillCounts();
        }

        private static void DrainUsesHealthActuallyRemoved()
        {
            foreach (bool soulRend in new[] { false, true })
            {
                VerifyDrain(soulRend, 1, 10, soulRend ? 1 : 2);
                VerifyDrain(soulRend, 8, 10, 4);
                VerifyDrain(soulRend, 8, 99, 1);
                VerifyDrain(soulRend, 8, 100, 0);
                VerifyDrain(soulRend, 1000, 10, null);
            }
        }

        private static void VerifyDrain(bool soulRend, int targetHealth, int casterHealth, int? expectedHealing, bool sanctuary = false)
        {
            GameObject host = new GameObject("Combat drain regression host");
            host.SetActive(false);
            host.hideFlags = HideFlags.HideAndDontSave;
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                CombatUnit caster = new CombatUnit
                {
                    Id = "drain-caster", Name = "Drain caster", Side = UnitSide.Party,
                    PartyIndex = -1, Hp = casterHealth, MaxHp = 100, X = 1, Y = 1,
                    Level = 5, Power = 20, DamageMax = 20, ClassKey = "warlock",
                    Skills = new SkillSet { Hex = 30 }, Spell = "hex"
                };
                CombatUnit target = new CombatUnit
                {
                    Id = "drain-target", Name = "Drain target", Side = UnitSide.Enemy,
                    PartyIndex = -1, Hp = targetHealth, MaxHp = targetHealth, X = 2, Y = 1,
                    Skills = new SkillSet()
                };
                GameState state = new GameState
                {
                    Mode = GameMode.Combat, Depth = 1, ReducedMotion = true,
                    SfxMuted = true, MusicMuted = true,
                    Combat = new CombatState { Units = new List<CombatUnit> { caster, target } }
                };
                SetField(game, "state", state);
                SetField(game, "rng", new MinimumRandom());
                Point targetSanctuary = new Point(target.X, target.Y, "sanctuary", 3);
                Point distantSanctuary = new Point(6, 4, "sanctuary", 3);
                if (sanctuary)
                {
                    caster.Mana = caster.MaxMana = 20;
                    state.Combat.Obstacles.Add(targetSanctuary);
                    state.Combat.Obstacles.Add(distantSanctuary);
                }

                CombatPowerOutcomeSnapshot outcomeBefore = CombatPowerOutcomeRules.Capture(state.Combat);
                bool resolved;
                if (soulRend)
                {
                    resolved = (bool)Invoke(game, "UseSoulRend", caster, target);
                }
                else
                {
                    FormulaDef formula = sanctuary ? Array.Find(FormulaCatalog.All, entry => entry.Code == "INH") : new FormulaDef
                    {
                        Code = "DRAIN-REGRESSION", Name = "Drain regression", Effect = "drain",
                        Target = "enemy", Skill = "hex", School = "hex", DamageType = "death",
                        Power = 30
                    };
                    if (sanctuary)
                    {
                        Require(formula != null, "authored Drain Life formula exists");
                        string preview = (string)Invoke(game, "FormulaPreview", caster, formula, target, target.X, target.Y);
                        Require(preview.Contains("breaks sanctuary"), "Drain Life's real target preview promises the sanctuary reaction");
                    }
                    resolved = (bool)Invoke(game, "ResolveFormula", formula, caster, target, target.X, target.Y);
                }

                string label = (soulRend ? "Soul Rend" : "Drain formula") + " against " + targetHealth + " HP";
                Require(resolved, label + " resolves");
                if (sanctuary)
                {
                    Require(!state.Combat.Obstacles.Contains(targetSanctuary), "Drain Life breaks sanctuary under its target, including on a lethal hit");
                    Require(state.Combat.Obstacles.Contains(distantSanctuary), "Drain Life preserves other sanctuary tiles");
                    Require(CombatPowerOutcomeRules.Compare(outcomeBefore, state.Combat).TerrainChanges == 1,
                        "the outcome records exactly the promised sanctuary removal");
                }
                Require(target.Hp < targetHealth, label + " connects with the runtime target");
                int expected = expectedHealing ?? Math.Min(100 - casterHealth, (targetHealth - target.Hp) / 2);
                Require(caster.Hp == casterHealth + expected, label + " heals only from removed health and respects the health cap");
                List<FloatText> floats = (List<FloatText>)typeof(AshenHallsGame)
                    .GetField("floatTexts", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(game);
                List<FloatText> healingFloats = floats.FindAll(value =>
                    value.X == caster.X && value.Y == caster.Y && value.Text.StartsWith("+", StringComparison.Ordinal));
                Require(expected > 0
                    ? healingFloats.Count == 1 && healingFloats[0].Text == "+" + expected
                    : healingFloats.Count == 0,
                    label + " displays only health actually restored");
                if (targetHealth <= 8) Require(target.Hp == 0, label + " remains a lethal hit");
                else Require(target.Hp > 0, label + " preserves the ordinary nonlethal case");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(host);
            }
        }

        private static void BeneficialStatusRemovalIsNotACure()
        {
            Action<CombatUnit, int>[] statuses =
            {
                (unit, value) => unit.Shielded = value,
                (unit, value) => unit.Regenerating = value,
                (unit, value) => unit.Stealthed = value
            };
            foreach (Action<CombatUnit, int> setStatus in statuses)
            {
                CombatUnit unit = new CombatUnit { Id = "beneficial", Hp = 10 };
                CombatState combat = new CombatState { Units = new List<CombatUnit> { unit } };
                CombatPowerOutcomeSnapshot before = CombatPowerOutcomeRules.Capture(combat);
                setStatus(unit, 2);
                CombatPowerOutcome applied = CombatPowerOutcomeRules.Compare(before, combat);
                Require(applied.StatusesApplied == 1, "applying a beneficial effect remains visible");

                before = CombatPowerOutcomeRules.Capture(combat);
                setStatus(unit, 0);
                CombatPowerOutcome removed = CombatPowerOutcomeRules.Compare(before, combat);
                Require(removed.AilmentsCleared == 0, "consuming stealth or losing a buff does not claim an ailment cure");
                Require(removed.AffectedUnits == 1, "buff removal still records the affected unit");
                Require(!removed.Summary.Contains("ailment"), "the player summary does not describe buff removal as a cure");
            }
        }

        private static void HarmfulStatusRemovalStillCounts()
        {
            CombatUnit unit = new CombatUnit
            {
                Id = "afflicted", Hp = 10, Poisoned = 2, Bleeding = 2, Stunned = 2,
                Sleeping = 2, Webbed = 2, Hexed = 2, Shielded = 2, Regenerating = 2, Stealthed = 2
            };
            CombatState combat = new CombatState { Units = new List<CombatUnit> { unit } };
            CombatPowerOutcomeSnapshot before = CombatPowerOutcomeRules.Capture(combat);
            unit.Poisoned = unit.Bleeding = unit.Stunned = unit.Sleeping = unit.Webbed = unit.Hexed = 0;
            unit.Shielded = unit.Regenerating = unit.Stealthed = 0;
            CombatPowerOutcome result = CombatPowerOutcomeRules.Compare(before, combat);
            Require(result.AilmentsCleared == 6, "only the six harmful effects count as cleared ailments");
            Require(result.AffectedUnits == 1, "multiple status changes count one affected unit");
        }

        private static object Invoke(AshenHallsGame game, string name, params object[] arguments)
        {
            MethodInfo method = typeof(AshenHallsGame).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Require(method != null, "runtime method exists: " + name);
            return method.Invoke(game, arguments);
        }

        private static void SetField(AshenHallsGame game, string name, object value)
        {
            FieldInfo field = typeof(AshenHallsGame).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Require(field != null, "runtime field exists: " + name);
            field.SetValue(game, value);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private sealed class MinimumRandom : System.Random
        {
            public override int Next(int maxValue) => 0;
            public override int Next(int minValue, int maxValue) => minValue;
            public override double NextDouble() => 0d;
        }
    }
}
