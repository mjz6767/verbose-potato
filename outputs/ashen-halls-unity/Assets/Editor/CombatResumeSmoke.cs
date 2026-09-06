using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class CombatResumeSmoke
    {
        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " combat resume smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogError(VersionInfo.ProductName + " combat resume smoke failed: " + exception);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            DuplicateInitiativeFallsBackToAPlayableQueue();
            foreach (string overlay in new[] { "showHelpOverlay", "showArmory", "showPauseMenu", "showDialogue", "showAbilityPanel" })
            {
                VerifyEnemyTimerWaitsForOverlay(overlay);
                VerifyRoundTransitionWaitsForOverlay(overlay);
            }
        }

        private static void DuplicateInitiativeFallsBackToAPlayableQueue()
        {
            string root = Path.Combine(Path.GetTempPath(), "AshenHallsCombatResume-" + Guid.NewGuid().ToString("N"));
            GameObject host = NewHost();
            try
            {
                string path = SaveService.SavePath(root);
                GameState valid = NewCombat();
                SaveService.SaveGameState(path, valid);
                GameState invalid = NewCombat();
                invalid.Combat.InitiativeQueue.Insert(1, "resume-enemy");
                Require(!SaveCandidateRules.IsLoadable(invalid, VersionInfo.SaveVersion),
                    "duplicate active unit in initiative is rejected before it can repeat indefinitely");
                SaveService.SaveGameState(path, invalid);
                GameState loaded = SaveService.LoadGameState(path,
                    candidate => SaveCandidateRules.IsLoadable(candidate, VersionInfo.SaveVersion), out bool usedBackup);
                Require(usedBackup, "duplicate initiative primary falls back to the valid campaign backup");

                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                Set(game, "state", loaded);
                object[] nextArgs = { false };
                CombatUnit next = (CombatUnit)Invoke(game, "NextQueuedCombatUnit", nextArgs);
                Require(next.Id == "resume-hero" && !(bool)nextArgs[0],
                    "recovered enemy turn advances to the hero in the same round");
                loaded.Combat.ActiveId = next.Id;
                next = (CombatUnit)Invoke(game, "NextQueuedCombatUnit", nextArgs);
                Require(next != null && (bool)nextArgs[0] && loaded.Combat.Round == 2,
                    "recovered initiative advances through the round boundary");

                valid.Combat.InitiativeQueue = null;
                Require(SaveCandidateRules.IsLoadable(valid, VersionInfo.SaveVersion),
                    "legacy missing initiative still reaches the runtime queue repair");
                valid.Combat.InitiativeQueue = new List<string>();
                Require(SaveCandidateRules.IsLoadable(valid, VersionInfo.SaveVersion),
                    "legacy empty initiative still reaches the runtime queue repair");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(host);
                if (Directory.Exists(root)) Directory.Delete(root, true);
            }
        }

        private static void VerifyEnemyTimerWaitsForOverlay(string overlay)
        {
            GameObject host = NewHost();
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                GameState state = NewCombat();
                Set(game, "state", state);
                Set(game, "rng", new System.Random(747));
                Set(game, "aiActAt", 1f);
                Set(game, overlay, true);
                int heroHealth = state.Combat.Units[1].Hp;

                Invoke(game, "HandleCombatTimers", 5f);
                Require(Get<float>(game, "aiActAt") == 1f, overlay + " preserves the overdue enemy timer");
                Require(state.Combat.ActiveId == "resume-enemy" && state.Combat.Round == 1,
                    overlay + " keeps the enemy turn and round unchanged");
                Require(state.Combat.Units[1].Hp == heroHealth, overlay + " prevents damage behind the menu");

                Set(game, overlay, false);
                Invoke(game, "HandleCombatTimers", 5f);
                Require(Get<float>(game, "aiActAt") < 0f, "closing " + overlay + " consumes the overdue enemy action once");
                Require(state.Combat != null && state.Combat.ActiveId == "resume-hero",
                    "closing " + overlay + " resumes the next party turn");
                int healthAfterResume = state.Combat.Units[1].Hp;
                Invoke(game, "HandleCombatTimers", 6f);
                Require(state.Combat.ActiveId == "resume-hero" && state.Combat.Units[1].Hp == healthAfterResume,
                    "a consumed enemy timer does not replay on another update");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(host);
            }
        }

        private static void VerifyRoundTransitionWaitsForOverlay(string overlay)
        {
            GameObject host = NewHost();
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                GameState state = NewCombat();
                state.Combat.ActiveId = "resume-hero";
                state.Combat.Phase = CombatPhase.Resolving;
                state.Combat.ActionAvailable = false;
                state.Combat.MovePoints = 0;
                Set(game, "state", state);
                Set(game, "rng", new System.Random(747));
                Set(game, "combatAdvancePending", true);
                Set(game, "combatAdvanceAt", -1f);
                Set(game, "combatAdvanceUnitId", "resume-hero");
                Set(game, "combatAdvanceStartsReservedTurn", true);
                Set(game, overlay, true);

                Invoke(game, "HandleCombatTimers", 5f);
                Require(Get<bool>(game, "combatAdvancePending") && state.Combat.Phase == CombatPhase.Resolving,
                    overlay + " holds the pending round transition");
                Require(!state.Combat.ActionAvailable && state.Combat.MovePoints == 0,
                    overlay + " cannot begin the reserved turn behind the menu");

                Set(game, overlay, false);
                Invoke(game, "HandleCombatTimers", 5f);
                Require(!Get<bool>(game, "combatAdvancePending") && state.Combat.ActiveId == "resume-hero",
                    "closing " + overlay + " completes the reserved turn transition");
                Require(state.Combat.ActionAvailable && state.Combat.MovePoints > 0,
                    "closing " + overlay + " starts the hero with action and movement");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(host);
            }
        }

        private static GameObject NewHost()
        {
            GameObject host = new GameObject("Combat resume regression host");
            host.SetActive(false);
            host.hideFlags = HideFlags.HideAndDontSave;
            return host;
        }

        private static GameState NewCombat()
        {
            CombatUnit enemy = new CombatUnit
            {
                Id = "resume-enemy", Name = "Resume enemy", Role = "rat", Side = UnitSide.Enemy,
                PartyIndex = -1, X = 2, Y = 1, Hp = 100, MaxHp = 100,
                Movement = 2, Power = 4, Range = 1, DamageMin = 4, DamageMax = 4,
                DamageType = "physical", Skills = new SkillSet(), Spell = ""
            };
            CombatUnit hero = new CombatUnit
            {
                Id = "resume-hero", Name = "Resume hero", Role = "shield", ClassKey = "warrior",
                Race = "human", Side = UnitSide.Party, PartyIndex = 0, X = 1, Y = 1,
                Hp = 100, MaxHp = 100, Movement = 3, Power = 4, Range = 1,
                Skills = new SkillSet(), Spell = ""
            };
            return new GameState
            {
                SaveVersion = VersionInfo.SaveVersion, Mode = GameMode.Combat, Depth = 1,
                ReducedMotion = true, SfxMuted = true, MusicMuted = true,
                Party = new List<PartyMember>
                {
                    new PartyMember
                    {
                        Id = hero.Id, Name = hero.Name, Role = hero.Role, Race = hero.Race,
                        ClassKey = hero.ClassKey, Hp = hero.Hp, MaxHp = hero.MaxHp,
                        Stats = new Stats(5, 5, 5, 5), Skills = new SkillSet()
                    }
                },
                Combat = new CombatState
                {
                    Round = 1, ActiveId = enemy.Id, Phase = CombatPhase.EnemyThinking,
                    ActionAvailable = true, MovePoints = 2,
                    Units = new List<CombatUnit> { enemy, hero },
                    InitiativeQueue = new List<string> { enemy.Id, hero.Id }
                }
            };
        }

        private static object Invoke(AshenHallsGame game, string method, params object[] args)
        {
            return typeof(AshenHallsGame).GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(game, args);
        }

        private static void Set(AshenHallsGame game, string field, object value)
        {
            typeof(AshenHallsGame).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(game, value);
        }

        private static T Get<T>(AshenHallsGame game, string field)
        {
            return (T)typeof(AshenHallsGame).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(game);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
