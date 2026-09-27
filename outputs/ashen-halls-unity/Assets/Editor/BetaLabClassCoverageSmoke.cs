using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class BetaLabClassCoverageSmoke
    {
        private const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly string[] PriestCodes =
        {
            "GBH", "GBX", "HLC", "OIC", "NVC", "SRF", "TBQ", "SGW",
            "TNC", "LBC", "TBG", "OBL", "LNH", "SWR", "SBN", "DWP"
        };

        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " Beta Lab class coverage smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogError(VersionInfo.ProductName + " Beta Lab class coverage smoke failed: " + exception);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            TitleLabExposesCompletePriestKit();
            PriestStageSupportsRealHealingCleansingAndOffense();
            MartialSwitchExposesThreeDedicatedCompleteKits();
            ResolvingActionsKeepExclusiveOwnership();
        }

        private static void TitleLabExposesCompletePriestKit()
        {
            WithTitleLab((game, state) =>
            {
                RequireToolbar(BetaLabKind.Caster, "Mage", "Warlock", "Priest", "MartialLab", "Stage");
                CombatUnit priest = state.Combat.Units.Single(unit => unit.Side == UnitSide.Party && unit.ClassKey == "priest");
                AssertPriestKit(game, state, priest);
                Dictionary<string, string> classesBefore = state.Party.ToDictionary(member => member.Id, member => member.ClassKey);
                PoisonTransientSelection(game);
                Toolbar(game, "Priest");
                Require(ReferenceEquals(priest, Current(game)), "Priest selects the dedicated priest already present in the title lab");
                AssertReadyTurn(game, state, priest, true);
                Require(state.Party.All(member => classesBefore[member.Id] == member.ClassKey), "Priest selection preserves the other dedicated caster kits");
                RequireLabSaveBlocked(game, state);
            });
        }

        private static void PriestStageSupportsRealHealingCleansingAndOffense()
        {
            WithTitleLab((game, state) =>
            {
                Toolbar(game, "Priest");
                CombatUnit priest = Current(game);
                PoisonTransientSelection(game);
                Toolbar(game, "Stage");
                Require(ReferenceEquals(priest, Current(game)) && priest.ClassKey == "priest", "Stage retains the chosen Priest instead of changing to Mage");
                AssertReadyTurn(game, state, priest, true);
                AssertPriestKit(game, state, priest);
                AssertVisibleBook(game, "Cleric Spellbook", true, PriestCodes);
                AssertNoOverlappingLivingUnits(state);
                foreach (string code in PriestCodes)
                {
                    FormulaDef formula = FormulaCatalog.All.Single(entry => entry.Code == code);
                    object card = Invoke(game, "FormulaActionCard", formula, priest);
                    Require(CardUsable(card), "the production Spellbook enables " + formula.Name + ": " + CardReason(card));
                    Require((int)Invoke(game, "CountLegalFormulaTargets", formula, priest) > 0,
                        "Priest Stage provides an actionable target for " + formula.Name);
                }
                Require(state.Combat.Units.Count(unit => unit.Side == UnitSide.Party && unit.Id != priest.Id && unit.Hp > 0 && unit.Hp < unit.MaxHp) >= 2,
                    "Priest Stage provides several injured allies for single and area healing");
                RequireLabSaveBlocked(game, state);

                CombatUnit injured = state.Combat.Units.First(unit => unit.Side == UnitSide.Party && unit.Id != priest.Id && unit.Hp > 0 && unit.Hp < unit.MaxHp);
                int healthBefore = injured.Hp;
                ConfirmFormula(game, priest, "OIC", injured);
                Require(injured.Hp > healthBefore && injured.Hp <= injured.MaxHp, "the production Heal command restores actual health without exceeding the cap");

                Toolbar(game, "Priest");
                Toolbar(game, "Stage");
                priest = Current(game);
                CombatUnit afflicted = state.Combat.Units.First(unit => unit.Side == UnitSide.Party && unit.Id != priest.Id && HarmfulStatuses(unit) > 0);
                ConfirmFormula(game, priest, "NVC", afflicted);
                Require(HarmfulStatuses(afflicted) == 0, "the production Cleanse command removes the staged ally's afflictions");

                Toolbar(game, "Priest");
                Toolbar(game, "Stage");
                priest = Current(game);
                FormulaDef lightBolt = FormulaCatalog.All.Single(entry => entry.Code == "OBL");
                CombatUnit enemy = state.Combat.Units.First(unit => unit.Side == UnitSide.Enemy && unit.Hp > 0
                    && (bool)Invoke(game, "IsFormulaActionable", lightBolt, priest, unit, unit.X, unit.Y));
                int enemyHealth = enemy.Hp;
                Set(game, "rng", new MinimumRandom());
                ConfirmFormula(game, priest, "OBL", enemy);
                Require(enemy.Hp < enemyHealth, "the production Dawnlance command damages the staged enemy");
                RequireLabSaveBlocked(game, state);
            });
        }

        private static void MartialSwitchExposesThreeDedicatedCompleteKits()
        {
            WithTitleLab((game, state) =>
            {
                PoisonTransientSelection(game);
                Toolbar(game, "MartialLab");
                Require(state.Combat.EncounterStyle == "martiallab", "the caster toolbar enters the production martial encounter");
                RequireToolbar(BetaLabKind.Martial, "Warrior", "Rogue", "Ranger", "CasterLab", "Cluster", "Wound");
                Require(Current(game).ClassKey == "warrior", "Martial Lab immediately selects Warrior");
                AssertReadyTurn(game, state, Current(game), false);
                Dictionary<string, string> dedicatedIds = new Dictionary<string, string>();
                foreach (string classKey in new[] { "warrior", "rogue", "ranger" })
                {
                    CombatUnit tester = state.Combat.Units.Single(unit => unit.Side == UnitSide.Party && unit.ClassKey == classKey);
                    dedicatedIds.Add(classKey, tester.Id);
                    Require(tester.Level == ProgressionRules.MaximumLevel, classKey + " is at maximum level when Martial Lab first opens");
                }
                Require(dedicatedIds.Values.Distinct().Count() == 3, "Warrior, Rogue, and Ranger use three distinct party members");

                foreach (string classKey in new[] { "warrior", "rogue", "ranger", "warrior" })
                {
                    PoisonTransientSelection(game);
                    Toolbar(game, char.ToUpperInvariant(classKey[0]) + classKey.Substring(1));
                    CombatUnit tester = Current(game);
                    Require(tester.Id == dedicatedIds[classKey] && tester.ClassKey == classKey, classKey + " selector retains its dedicated identity");
                    AssertReadyTurn(game, state, tester, false);
                    AssertVisibleBook(game, char.ToUpperInvariant(classKey[0]) + classKey.Substring(1) + " Skillbook", false,
                        AbilityCatalog.IdsForClass(classKey).ToArray());
                    Toolbar(game, "Cluster");
                    Toolbar(game, "Wound");
                    List<MartialAbility> abilities = (List<MartialAbility>)Invoke(game, "MartialAbilitiesFor", tester);
                    Require(abilities.Count == 7 && abilities.Select(ability => ability.Id).Distinct().Count() == 7,
                        classKey + " production Skillbook exposes all seven distinct skills");
                    foreach (MartialAbility ability in abilities)
                    {
                        object card = Invoke(game, "AbilityActionCard", ability, tester);
                        Require(CardUsable(card), classKey + " can ready " + ability.Name + ": " + CardReason(card));
                        if (ability.Targeted)
                            Require((int)Invoke(game, "CountLegalAbilityTargets", ability, tester) > 0, classKey + " has a legal staged target for " + ability.Name);
                    }
                    PartyMember member = state.Party[tester.PartyIndex];
                    Require(member.Id == tester.Id && member.ClassKey == classKey && member.Level == ProgressionRules.MaximumLevel,
                        classKey + " party and active combat records agree");
                    RequireLabSaveBlocked(game, state);
                    AssertNoOverlappingLivingUnits(state);
                }

                CombatUnit warrior = Current(game);
                MartialAbility cleave = AbilityCatalog.For("cleave");
                Require((bool)Invoke(game, "PrepareAbility", warrior, cleave.Id), "the production Skillbook arms the level-eight Cleave skill");
                CombatUnit victim = state.Combat.Units.First(unit => unit.Side == UnitSide.Enemy && unit.Hp > 0
                    && (bool)Invoke(game, "CombatBoardCursorCellIsLegal", warrior, unit.X, unit.Y));
                int healthBefore = victim.Hp;
                Set(game, "rng", new MinimumRandom());
                ConfirmTarget(game, warrior, victim);
                Require(victim.Hp < healthBefore && ActionWasSpent(state, warrior),
                    "Cleave damages a real target and consumes exactly the selected warrior action");
                AssertPowerEntriesCleared(game);
                SettleAdvance(game);

                Set(game, "rng", new System.Random(state.Seed));
                Toolbar(game, "CasterLab");
                Require(state.Combat.EncounterStyle == "lab", "the martial toolbar returns to the caster encounter");
                Toolbar(game, "Priest");
                AssertReadyTurn(game, state, Current(game), true);
                AssertPriestKit(game, state, Current(game));
                RequireLabSaveBlocked(game, state);
            });
        }

        private static void ResolvingActionsKeepExclusiveOwnership()
        {
            WithTitleLab((game, state) =>
            {
                Toolbar(game, "Priest");
                foreach (string action in new[] { "Priest", "Stage", "MartialLab", "Reset" })
                {
                    Set(game, "combatAdvancePending", true);
                    string before = JsonUtility.ToJson(state);
                    CombatUnit active = Current(game);
                    Toolbar(game, action);
                    Require(JsonUtility.ToJson(state) == before && ReferenceEquals(Current(game), active)
                        && Get<bool>(game, "combatAdvancePending"), action + " cannot interrupt a resolving combat action");
                    Invoke(game, "CancelCombatResolutionBeat", false);
                }
                Toolbar(game, "MartialLab");
                foreach (string action in new[] { "Warrior", "Rogue", "Ranger", "CasterLab" })
                {
                    Set(game, "combatAdvancePending", true);
                    string before = JsonUtility.ToJson(state);
                    Toolbar(game, action);
                    Require(JsonUtility.ToJson(state) == before && Get<bool>(game, "combatAdvancePending"),
                        action + " respects combat resolution ownership");
                    Invoke(game, "CancelCombatResolutionBeat", false);
                }
                RequireLabSaveBlocked(game, state);
            });
        }

        private static void AssertPriestKit(AshenHallsGame game, GameState state, CombatUnit priest)
        {
            Require(priest.ClassKey == "priest" && priest.Level == ProgressionRules.MaximumLevel, "Priest is ready at maximum level");
            HashSet<string> known = new HashSet<string>(((IEnumerable<FormulaDef>)Invoke(game, "KnownFormulasFor", priest)).Select(formula => formula.Code));
            Require(PriestCodes.All(known.Contains) && known.Count == PriestCodes.Length, "Priest knows all sixteen mend formulas, including shared Rift Seal and every capstone");
            PartyMember member = state.Party[priest.PartyIndex];
            Require(member.Id == priest.Id && member.ClassKey == "priest" && member.Level == priest.Level && member.Spell == priest.Spell,
                "Priest's party and combat records retain the same class, level, and craft");
            foreach (string code in PriestCodes)
            {
                FormulaDef formula = FormulaCatalog.All.Single(entry => entry.Code == code);
                object[] arguments = { priest, code, "" };
                Require((bool)Invoke(game, "CanUseFormula", arguments), "Priest meets the production unlock gate for " + formula.Name + ": " + arguments[2]);
                Require(priest.Mana >= (int)Invoke(game, "EffectiveFormulaMana", formula, priest), "the Priest kit has enough mana for " + formula.Name);
            }
        }

        private static void AssertVisibleBook(AshenHallsGame game, string title, bool spellbook, string[] expectedIds)
        {
            CombatAbilityModalView view = (CombatAbilityModalView)Invoke(game, "BuildCombatAbilityModalView");
            Require(view.Visible && view.Spellbook == spellbook && view.Title == title,
                "the production power modal visibly opens the selected class book: " + title);
            Require(view.Cards.Count == expectedIds.Length && new HashSet<string>(view.Cards.Select(card => card.Id)).SetEquals(expectedIds),
                title + " renders every expected power card without filtering or duplicates");
            Require(view.Cards.All(card => !card.Locked), title + " has no level-locked cards in the maximum-level lab");
        }

        private static void AssertReadyTurn(AshenHallsGame game, GameState state, CombatUnit active, bool spellbook)
        {
            Require(active.Side == UnitSide.Party && !active.Summoned && active.Hp > 0 && state.Combat.ActiveId == active.Id,
                "the selected tester owns a living party turn");
            Require(state.Combat.Phase == CombatPhase.ChooseAction && state.Combat.ActionAvailable && !state.Combat.Acted && state.Combat.MovePoints > 0,
                "the selected tester has a fresh legal action and movement budget");
            Require(Get<bool>(game, "showSpellbook") == spellbook && Get<bool>(game, "showAbilityPanel") != spellbook,
                "the selected class opens its production power book");
            Require(Get<ActionMode>(game, "selectedAction") == (spellbook ? ActionMode.Cast : ActionMode.Ability),
                "the selected class uses the matching command mode");
            AssertPowerEntriesCleared(game);
            Require(!Get<bool>(game, "combatAdvancePending") && Get<float>(game, "aiActAt") < 0f
                && string.IsNullOrEmpty(Get<string>(game, "combatAdvanceUnitId")), "the selected tester inherits no queued enemy or previous-unit turn");
        }

        private static void AssertPowerEntriesCleared(AshenHallsGame game)
        {
            Require(string.IsNullOrEmpty(Get<string>(game, "pendingFormulaCode")) && string.IsNullOrEmpty(Get<string>(game, "pendingAbilityId")),
                "the completed action or class transition clears both armed powers");
            Require(!Get<bool>(game, "combatBoardCursorActive") && !Get<Vector2Int?>(game, "combatBoardCursorCell").HasValue,
                "the completed action or class transition clears stale board targeting");
        }

        private static void ConfirmFormula(AshenHallsGame game, CombatUnit priest, string code, CombatUnit target)
        {
            Require((bool)Invoke(game, "PrepareFormulaCode", priest, code), "the production Spellbook arms " + code);
            int manaBefore = priest.Mana;
            FormulaDef formula = FormulaCatalog.All.Single(entry => entry.Code == code);
            int cost = (int)Invoke(game, "EffectiveFormulaMana", formula, priest);
            ConfirmTarget(game, priest, target);
            GameState state = Get<GameState>(game, "state");
            Require(priest.Mana == manaBefore - cost && ActionWasSpent(state, priest),
                code + " consumes exactly its advertised mana cost and the selected Priest action");
            AssertPowerEntriesCleared(game);
            SettleAdvance(game);
        }

        private static void ConfirmTarget(AshenHallsGame game, CombatUnit active, CombatUnit target)
        {
            Require((bool)Invoke(game, "CombatBoardCursorCellIsLegal", active, target.X, target.Y), "the real controller targeting rules accept the staged target");
            Set(game, "combatBoardCursorCell", (Vector2Int?)new Vector2Int(target.X, target.Y));
            Require((bool)Invoke(game, "ConfirmCombatBoardCursor", active), "the real controller confirm dispatches the armed power");
        }

        private static void SettleAdvance(AshenHallsGame game)
        {
            if (Get<bool>(game, "combatAdvancePending"))
            {
                Set(game, "combatAdvanceAt", Time.time - 1f);
                Invoke(game, "CompletePendingCombatAdvance");
            }
        }

        private static void PoisonTransientSelection(AshenHallsGame game)
        {
            Set(game, "pendingFormulaCode", "FBL");
            Set(game, "pendingAbilityId", "quickshot");
            Set(game, "combatBoardCursorActive", true);
            Set(game, "combatBoardCursorCell", (Vector2Int?)new Vector2Int(11, 7));
            Set(game, "aiActAt", Time.time + 10f);
            Set(game, "combatAdvanceUnitId", "previous-tester");
        }

        private static void RequireLabSaveBlocked(AshenHallsGame game, GameState state)
        {
            Require(Get<bool>(game, "betaLabMode") && Get<bool>(game, "labSaveBlocked"), "all class and lab transitions retain the campaign save block");
            string path = Path.Combine(Path.GetTempPath(), "ashen-beta-class-save-check-" + Guid.NewGuid().ToString("N") + ".json");
            Require(!SaveService.TrySaveCampaignState(path, state, Get<bool>(game, "labSaveBlocked"), out string reason)
                && !string.IsNullOrWhiteSpace(reason) && !File.Exists(path), "the production save service refuses the lab state without writing a campaign file");
        }

        private static void RequireToolbar(BetaLabKind kind, params string[] actions)
        {
            HashSet<string> available = new HashSet<string>(BetaLabToolbarRules.Actions(kind).Select(action => action.Id.ToString()));
            Require(actions.All(available.Contains), kind + " toolbar exposes each class selector and the other lab");
        }

        private static void AssertNoOverlappingLivingUnits(GameState state)
        {
            List<CombatUnit> units = state.Combat.Units.Where(unit => unit.Hp > 0).ToList();
            Require(units.Select(unit => new Vector2Int(unit.X, unit.Y)).Distinct().Count() == units.Count,
                "staging leaves each living unit on a distinct board cell");
        }

        private static int HarmfulStatuses(CombatUnit unit) => unit.Poisoned + unit.Bleeding + unit.Stunned + unit.Sleeping + unit.Webbed + unit.Hexed;
        private static bool ActionWasSpent(GameState state, CombatUnit actor) => state.Combat.ActiveId != actor.Id
            || (!state.Combat.ActionAvailable && state.Combat.Acted);
        private static bool CardUsable(object card) => card != null && (bool)card.GetType().GetField("Usable").GetValue(card);
        private static string CardReason(object card) => card == null ? "missing card" : (string)card.GetType().GetField("DisabledReason").GetValue(card);
        private static CombatUnit Current(AshenHallsGame game) => (CombatUnit)Invoke(game, "CurrentUnit");

        private static void Toolbar(AshenHallsGame game, string action)
        {
            BetaLabToolbarActionId id = (BetaLabToolbarActionId)Enum.Parse(typeof(BetaLabToolbarActionId), action);
            Invoke(game, "ExecuteBetaLabToolbarAction", id, Current(game));
            Get<GameState>(game, "state").SfxMuted = true;
        }

        private static void WithTitleLab(Action<AshenHallsGame, GameState> check)
        {
            GameObject host = new GameObject("Beta Lab class coverage regression host");
            host.SetActive(false);
            host.hideFlags = HideFlags.HideAndDontSave;
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                GameState state = new GameState { Mode = GameMode.Tavern, Seed = 73419, ReducedMotion = true, SfxMuted = true, MusicMuted = true };
                Set(game, "state", state);
                Set(game, "rng", new System.Random(state.Seed));
                // No Awake, campaign load, or UI hierarchy is needed to exercise
                // the actual title handler. Also block settings persistence.
                Set(game, "visualSmokeSaveBlocked", true);
                Set(game, "launchError", "Beta Lab class coverage fixture presentation held");
                Invoke(game, "StartBetaCombatLabFromTitle");
                state.SfxMuted = true;
                Require(state.Mode == GameMode.Combat && state.Combat?.EncounterStyle == "lab", "the production title Beta Lab handler opens caster combat");
                check(game, state);
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        private static T Get<T>(object source, string name)
        {
            FieldInfo field = source.GetType().GetField(name, PrivateInstance);
            Require(field != null, "runtime field exists: " + name);
            return (T)field.GetValue(source);
        }

        private static void Set(object source, string name, object value)
        {
            FieldInfo field = source.GetType().GetField(name, PrivateInstance);
            Require(field != null, "runtime field exists: " + name);
            field.SetValue(source, value);
        }

        private static object Invoke(object source, string name, params object[] arguments)
        {
            MethodInfo method = source.GetType().GetMethod(name, PrivateInstance);
            Require(method != null, "runtime method exists: " + name);
            return method.Invoke(source, arguments);
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
