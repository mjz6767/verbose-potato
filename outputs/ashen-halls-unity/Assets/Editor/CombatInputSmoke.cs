using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class CombatInputSmoke
    {
        private const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;

        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " combat input ownership smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " combat input ownership smoke failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            ControllerCancelHasOneInputOwner();
            foreach (bool spell in new[] { true, false })
            {
                GlobalCancelClosesOnlyTheBook(spell);
                ResumeRequiresASeparateConfirmation(spell);
            }
        }

        private static void ControllerCancelHasOneInputOwner()
        {
            UnityEngine.Object inputSettings = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0];
            SerializedProperty axes = new SerializedObject(inputSettings).FindProperty("m_Axes");
            bool controllerCancelFound = false;
            for (int i = 0; i < axes.arraySize; i++)
            {
                SerializedProperty axis = axes.GetArrayElementAtIndex(i);
                if (axis.FindPropertyRelative("m_Name").stringValue != "Cancel") continue;
                controllerCancelFound = axis.FindPropertyRelative("altPositiveButton").stringValue == "joystick button 1"
                    || axis.FindPropertyRelative("positiveButton").stringValue == "joystick button 1";
                if (controllerCancelFound) break;
            }
            Require(controllerCancelFound, "controller Back reaches the global Cancel input route");
            // Legacy Input has no injectable key events in an EditMode runner.
            // Check the ownership wiring as well as the real state transitions below.
            string source = File.ReadAllText(Path.Combine(Application.dataPath, "Scripts", "Presentation", "CombatAbilityModalScreen.cs"));
            Require(!source.Contains("Input.GetKeyDown(KeyCode.JoystickButton1)"), "the power book does not independently dispatch the global controller Cancel");
        }

        private static void GlobalCancelClosesOnlyTheBook(bool spell)
        {
            WithFixture(spell, (game, state, active, target, code) =>
            {
                Require((bool)Invoke(game, "HandleCancelCommand"), "global Cancel handles the open " + (spell ? "Spellbook" : "Skillbook"));
                Require(!Get<bool>(game, "showSpellbook") && !Get<bool>(game, "showAbilityPanel"), "Cancel closes the book");
                Require(Get<string>(game, spell ? "pendingFormulaCode" : "pendingAbilityId") == code, "Cancel preserves the underlying armed power");
                Require(Get<bool>(game, "combatBoardCursorActive"), "Cancel preserves board targeting");
                Require(!Get<bool>(game, "showPauseMenu"), "Cancel does not also open Pause");
                Require(state.Combat.ActionAvailable && !state.Combat.Acted && active.Mana == 100 && target.Hp == 100, "closing the book spends no action, mana, or health");
            });
        }

        private static void ResumeRequiresASeparateConfirmation(bool spell)
        {
            WithFixture(spell, (game, state, active, target, code) =>
            {
                Require((bool)Invoke(game, "CombatBoardCursorCanConfirmFromInput"), "fixture starts with an old armed cursor that could confirm");
                Invoke(game, "ActivateCombatAbilityModalCard", code);
                Require(!Get<bool>(game, "showSpellbook") && !Get<bool>(game, "showAbilityPanel"), "Resume Targeting closes the book");
                Require(Get<string>(game, spell ? "pendingFormulaCode" : "pendingAbilityId") == code, "Resume retains the selected power");
                Require(Get<Vector2Int?>(game, "combatBoardCursorCell") == new Vector2Int(target.X, target.Y), "Resume preserves the inspected target cell");
                Require(!(bool)Invoke(game, "CombatBoardCursorCanConfirmFromInput"), "Resume cannot confirm the target on the same input frame");
                Require(state.Combat.ActionAvailable && !state.Combat.Acted && state.Combat.MovePoints == 4 && active.Mana == 100 && target.Hp == 100, "Resume spends no combat resources");
                Set(game, "combatBoardCursorActivatedFrame", Time.frameCount - 1);
                Require((bool)Invoke(game, "CombatBoardCursorCanConfirmFromInput"), "a later input frame can confirm the resumed cursor");
            });
        }

        private static void WithFixture(bool spell, Action<AshenHallsGame, GameState, CombatUnit, CombatUnit, string> check)
        {
            GameObject host = new GameObject("Combat input regression host");
            host.SetActive(false);
            host.hideFlags = HideFlags.HideAndDontSave;
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                CombatUnit active = new CombatUnit
                {
                    Id = "input-hero", Name = "Input hero", Side = UnitSide.Party, PartyIndex = -1,
                    Hp = 100, MaxHp = 100, Mana = 100, MaxMana = 100, Level = 10,
                    X = 1, Y = 1, Range = 6, Power = 10,
                    ClassKey = spell ? "mage" : "ranger", Spell = spell ? "ember" : "",
                    Skills = new SkillSet { Ember = 20, Missile = 20 }
                };
                CombatUnit target = new CombatUnit
                {
                    Id = "input-target", Name = "Input target", Side = UnitSide.Enemy, PartyIndex = -1,
                    Hp = 100, MaxHp = 100, X = 4, Y = 1, Skills = new SkillSet()
                };
                GameState state = new GameState
                {
                    Mode = GameMode.Combat, Depth = 1, SfxMuted = true, MusicMuted = true,
                    Combat = new CombatState
                    {
                        ActiveId = active.Id, Phase = CombatPhase.ChooseTarget,
                        MovePoints = 4, ActionAvailable = true,
                        Units = new List<CombatUnit> { active, target }
                    }
                };
                Set(game, "state", state);
                // Keep UI creation out of this isolated handler fixture; no Awake,
                // player saves, scene changes, or rendering are required.
                Set(game, "launchError", "Combat input fixture presentation held");
                Set(game, "showSpellbook", spell);
                Set(game, "showAbilityPanel", !spell);
                string code = spell ? "FBL" : "aimedshot";
                Set(game, spell ? "pendingFormulaCode" : "pendingAbilityId", code);
                Set(game, "selectedAction", spell ? ActionMode.Cast : ActionMode.Ability);
                Set(game, "combatBoardCursorActive", true);
                Set(game, "combatBoardCursorCell", (Vector2Int?)new Vector2Int(target.X, target.Y));
                Set(game, "combatBoardCursorActivatedFrame", Time.frameCount - 10);
                check(game, state, active, target, code);
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        private static T Get<T>(object source, string name) => (T)source.GetType().GetField(name, PrivateInstance).GetValue(source);
        private static void Set(object source, string name, object value) => source.GetType().GetField(name, PrivateInstance).SetValue(source, value);
        private static object Invoke(object source, string name, params object[] arguments) => source.GetType().GetMethod(name, PrivateInstance).Invoke(source, arguments);
        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
