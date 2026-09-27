using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class MendProgressionSmoke
    {
        private const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly string[] PromotedCodes = { "LNH", "SGW", "TBG", "SWR" };
        private static readonly int[] PromotedLevels = { 6, 9, 13, 14 };

        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " Mend campaign progression smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogError(VersionInfo.ProductName + " Mend campaign progression smoke failed: " + exception);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            CampaignBooksAndUnlocksUseTheSameLadder();
            GroupWardsHaveDistinctDurationsAndBoundedRefresh();
            GroupRegenerationPaysOffOnLivingAlliesTurns();
            HostileControlRespectsResistanceAndOneTurnDuration();
            FocusChangesCostAndReachWithoutInflatingDuration();
        }

        private static void CampaignBooksAndUnlocksUseTheSameLadder()
        {
            foreach (string classKey in new[] { "priest", "paladin" })
            {
                WithBattle(classKey, (game, state, caster, target, adjacent, enemy) =>
                {
                    Set(game, "showSpellbook", true);
                    Set(game, "selectedAction", ActionMode.Cast);
                    for (int index = 0; index < PromotedCodes.Length; index++)
                    {
                        string code = PromotedCodes[index];
                        FormulaDef formula = Formula(code);
                        Require(FormulaCatalog.RequiredLevel(formula) == PromotedLevels[index], code + " retains its authored unlock level");
                        caster.Level = PromotedLevels[index] - 1;
                        CombatAbilityModalView view = (CombatAbilityModalView)Invoke(game, "BuildCombatAbilityModalView");
                        Require(view.Visible && view.Spellbook, classKey + " opens the real campaign Spellbook");
                        CombatAbilityModalCardView card = view.Cards.Single(entry => entry.Id == code);
                        Require(card.Locked, classKey + " sees the future " + formula.Name + " locked one level early");
                        HashSet<string> known = KnownCodes(game, caster);
                        Require(!known.Contains(code), classKey + " cannot know " + code + " before its unlock");
                        CombatUnit mark = code == "LNH" ? enemy : target;
                        int manaBefore = caster.Mana;
                        Require(!Cast(game, caster, code, mark), classKey + " cannot cast " + code + " one level early");
                        Require(caster.Mana == manaBefore, "a level-locked cast spends no mana");

                        caster.Level++;
                        view = (CombatAbilityModalView)Invoke(game, "BuildCombatAbilityModalView");
                        card = view.Cards.Single(entry => entry.Id == code);
                        Require(!card.Locked && KnownCodes(game, caster).Contains(code),
                            classKey + " unlocks " + code + " in both the book and casting rules");
                        if (code != "LNH")
                        {
                            string durations = code == "SGW" ? "2 target / 1 adjacent turns" : "3 target / 2 adjacent turns";
                            Require(card.CurrentEffect.Contains(durations) && !card.CurrentEffect.Contains("resistance"),
                                "the actual support book describes both durations without suggesting friendly resistance");
                        }
                        Require(!view.Cards.Any(entry => entry.Id == "GBX" || entry.Id == "LBC"),
                            "redundant prototype cover and area healing remain outside the campaign book");
                        caster.Mana = caster.MaxMana;
                        Require(Cast(game, caster, code, mark), classKey + " can actually cast " + code + " at its unlock");
                    }
                    caster.Level = ProgressionRules.MaximumLevel;
                    Set(game, "activeContentSet", ContentSetCatalog.FullPrototype);
                    Require(KnownCodes(game, caster).Contains("GBX") && KnownCodes(game, caster).Contains("LBC"),
                        "the excluded prototypes remain available in the full content set");
                });
            }
        }

        private static void GroupWardsHaveDistinctDurationsAndBoundedRefresh()
        {
            WithBattle("priest", (game, state, caster, target, adjacent, enemy) =>
            {
                Require(Cast(game, caster, "SGW", target), "Sanctuary Ward casts through the production dispatcher");
                Require(caster.Mana == 10 && target.Shielded == 2 && adjacent.Shielded == 1,
                    "Sanctuary Ward spends 8 MP for primary 2 / adjacent 1");
                AssertOnlySupportCluster(state, caster, target, adjacent, false);

                caster.Mana = caster.MaxMana;
                Require(Cast(game, caster, "TBG", target), "Covenant Ward casts through the production dispatcher");
                Require(caster.Mana == 9 && target.Shielded == 3 && adjacent.Shielded == 2,
                    "Covenant Ward spends 9 MP for primary 3 / adjacent 2");
                string preview = (string)Invoke(game, "FormulaPreview", caster, Formula("TBG"), target, target.X, target.Y);
                Require(preview.Contains("3 target / 2 adjacent turns"),
                    "the real target preview discloses both primary and adjacent ward durations");
                AssertOnlySupportCluster(state, caster, target, adjacent, false);
                caster.Mana = caster.MaxMana;
                Require(Cast(game, caster, "TBG", target), "Covenant Ward can refresh its existing ward");
                Require(target.Shielded == 3 && adjacent.Shielded == 2, "repeated Covenant Ward does not stack durations");
                caster.Mana = caster.MaxMana;
                Require(Cast(game, caster, "SGW", target), "the cheaper group ward can be cast after Covenant Ward");
                Require(target.Shielded == 3 && adjacent.Shielded == 2, "a shorter ward cannot overwrite a longer ward");

                Invoke(game, "ApplyStartTurnEffects", target);
                Invoke(game, "ApplyStartTurnEffects", adjacent);
                Require(target.Shielded == 2 && adjacent.Shielded == 1,
                    "ward durations expire once at each recipient's own turn start");
                Invoke(game, "ApplyStartTurnEffects", adjacent);
                Require(adjacent.Shielded == 0, "the adjacent Covenant ward expires after two recipient starts");

                target.Shielded = adjacent.Shielded = 0;
                caster.Mana = caster.MaxMana;
                Require(Cast(game, caster, "TBQ", target), "the original single-target ward still casts");
                Require(caster.Mana == 12 && target.Shielded == 3 && adjacent.Shielded == 0,
                    "Sacred Aegis remains the cheapest lasting protection for one ally");
            });
        }

        private static void GroupRegenerationPaysOffOnLivingAlliesTurns()
        {
            foreach (int depth in new[] { 1, 4 })
            {
                WithBattle("paladin", (game, state, caster, target, adjacent, enemy) =>
                {
                    state.Depth = depth;
                    int healPerTurn = 4 + depth;
                    target.Hp = adjacent.Hp = 20;
                    target.MaxHp = adjacent.MaxHp = 100;
                    Require(Cast(game, caster, "SWR", target), "Wellspring Hymn casts at campaign depth " + depth);
                    Require(caster.Mana == 8 && target.Regenerating == 3 && adjacent.Regenerating == 2,
                        "Wellspring Hymn spends 10 MP for primary 3 / adjacent 2");
                    Require(target.Hp == 20 && adjacent.Hp == 20, "regeneration grants no immediate burst healing");
                    AssertOnlySupportCluster(state, caster, target, adjacent, true);
                    caster.Mana = caster.MaxMana;
                    Require(Cast(game, caster, "SWR", target), "Wellspring Hymn can refresh");
                    Require(target.Regenerating == 3 && adjacent.Regenerating == 2, "repeat Hymn refreshes without adding turns");
                    for (int turn = 0; turn < 4; turn++)
                    {
                        Invoke(game, "ApplyStartTurnEffects", target);
                        Invoke(game, "ApplyStartTurnEffects", adjacent);
                    }
                    Require(target.Hp == 20 + 3 * healPerTurn && adjacent.Hp == 20 + 2 * healPerTurn,
                        "early and late sustain totals follow exactly three primary and two adjacent ticks");
                    Require(target.Regenerating == 0 && adjacent.Regenerating == 0, "regeneration expires without an extra tick");

                    target.Hp = target.MaxHp - 1;
                    adjacent.Hp = 0;
                    caster.Mana = caster.MaxMana;
                    Require(Cast(game, caster, "SWR", target), "Hymn can support a living nearly healed ally");
                    Invoke(game, "ApplyStartTurnEffects", target);
                    Require(target.Hp == target.MaxHp, "regeneration cannot heal above maximum health");
                    Require(adjacent.Hp == 0 && adjacent.Regenerating == 0, "group regeneration neither revives nor buffs a defeated ally");
                    target.Regenerating = 0;
                    adjacent.Hp = 20;
                    caster.Mana = caster.MaxMana;
                    Require(Cast(game, caster, "TNC", target), "Prayer of Renewal retains its single-target role");
                    Require(caster.Mana == 10 && target.Regenerating == 3 && adjacent.Regenerating == 0,
                        "single-target Renewal costs 2 MP less than the group Hymn");
                });
            }
        }

        private static void HostileControlRespectsResistanceAndOneTurnDuration()
        {
            WithBattle("priest", (game, state, caster, target, adjacent, enemy) =>
            {
                caster.Level = 6;
                Set(game, "rng", new FixedRandom(0.70));
                enemy.MagicResist = 0;
                Require(Cast(game, caster, "LNH", enemy), "Word of Stillness casts at level six");
                Require(caster.Mana == 11 && enemy.Stunned == 1 && enemy.Hp == enemy.MaxHp,
                    "Word of Stillness spends 7 MP to stun without dealing damage");
                Require((bool)Invoke(game, "ApplyStartTurnEffects", enemy), "an applied Word of Stillness denies the enemy its next turn");
                Require(enemy.Stunned == 0 && !(bool)Invoke(game, "ApplyStartTurnEffects", enemy),
                    "Word of Stillness expires after the one denied turn");

                enemy.MagicResist = 4;
                caster.Mana = caster.MaxMana;
                string preview = (string)Invoke(game, "FormulaPreview", caster, Formula("LNH"), enemy, enemy.X, enemy.Y);
                Require(preview.Contains("58%"), "a resistant enemy's real preview advertises the reduced stun chance");
                Require(Cast(game, caster, "LNH", enemy), "a resisted cast still resolves");
                Require(caster.Mana == 11 && enemy.Stunned == 0, "the same roll fails against resistance and still costs mana");
                Require(state.Log.Any(entry => entry.Text.Contains("resists")), "combat narration discloses the resisted control");

                caster.Mana = caster.MaxMana;
                Require(!Cast(game, caster, "LNH", target), "hostile control cannot target an ally");
                Require(caster.Mana == caster.MaxMana, "an invalid control target costs no mana");
                state.Combat.Obstacles.Add(new Point(2, 1, "stone"));
                Require(!Cast(game, caster, "LNH", enemy), "Word of Stillness respects blocking cover");
                Require(caster.Mana == caster.MaxMana, "blocked control spends no mana");
            });
        }

        private static void FocusChangesCostAndReachWithoutInflatingDuration()
        {
            foreach (string code in PromotedCodes)
            {
                WithBattle("priest", (game, state, caster, target, adjacent, enemy) =>
                {
                    CombatUnit mark = code == "LNH" ? enemy : target;
                    mark.X = 6;
                    mark.Y = 1;
                    int cost = Formula(code).Mana;
                    caster.Mana = cost - 1;
                    Require(!Cast(game, caster, code, mark), code + " is out of unfocused range at five cells");
                    Require(caster.Mana == cost - 1, "an out-of-range cast does not spend mana");
                    state.Combat.Moved = false;
                    state.Combat.MovePoints = caster.Movement;
                    Require(Cast(game, caster, code, mark), code + " focuses to reach five cells with exactly one less MP");
                    Require(caster.Mana == 0, code + " charges the exact focus discount");
                    if (code == "LNH") Require(mark.Stunned == 1, "focus does not extend hostile control");
                    else if (code == "SWR") Require(mark.Regenerating == 3, "focus does not extend regeneration");
                    else Require(mark.Shielded == (code == "TBG" ? 3 : 2), "focus does not extend wards");
                    mark.X = 3;
                    caster.Mana = cost - 2;
                    Require(!Cast(game, caster, code, mark), code + " rejects insufficient mana even while focused");
                    Require(caster.Mana == cost - 2, "insufficient mana leaves the pool untouched");
                });
            }
        }

        private static void AssertOnlySupportCluster(GameState state, CombatUnit caster, CombatUnit target, CombatUnit adjacent, bool regeneration)
        {
            foreach (CombatUnit other in state.Combat.Units.Where(unit => unit != target && unit != adjacent))
            {
                Require((regeneration ? other.Regenerating : other.Shielded) == 0,
                    "group support excludes the distant caster, diagonal/far allies, enemies, and defeated allies: " + other.Id);
            }
        }

        private static void WithBattle(string classKey, Action<AshenHallsGame, GameState, CombatUnit, CombatUnit, CombatUnit, CombatUnit> check)
        {
            GameObject host = new GameObject("Mend campaign progression regression host");
            host.SetActive(false);
            host.hideFlags = HideFlags.HideAndDontSave;
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                CombatUnit caster = Unit("mend-caster", UnitSide.Party, 1, 1);
                caster.ClassKey = classKey;
                caster.Spell = StarterPartyCatalog.SpellSchoolForClass(classKey);
                caster.Level = ProgressionRules.MaximumLevel;
                // Eighteen max MP makes the production fallback INT score 10,
                // allowing resistance checks to use an explicit 86% base chance.
                caster.Mana = caster.MaxMana = 18;
                caster.Movement = 3;
                caster.Skills = new SkillSet { Mend = 24 };
                CombatUnit target = Unit("primary-ally", UnitSide.Party, 3, 1);
                CombatUnit adjacent = Unit("adjacent-ally", UnitSide.Party, 3, 2);
                CombatUnit enemy = Unit("enemy", UnitSide.Enemy, 4, 1);
                CombatUnit defeated = Unit("defeated-ally", UnitSide.Party, 2, 1);
                defeated.Hp = 0;
                GameState state = new GameState
                {
                    Mode = GameMode.Combat, ContentSetId = ContentSetCatalog.SewerSlice, Depth = 1,
                    ReducedMotion = true, SfxMuted = true, MusicMuted = true,
                    Combat = new CombatState
                    {
                        Round = 1, ActiveId = caster.Id, ActionAvailable = true, Moved = true,
                        Phase = CombatPhase.ChooseAction, MovePoints = 2,
                        Units = new List<CombatUnit>
                        {
                            caster, target, adjacent, enemy, defeated,
                            Unit("diagonal-ally", UnitSide.Party, 4, 2),
                            Unit("far-ally", UnitSide.Party, 7, 5)
                        }
                    }
                };
                Set(game, "state", state);
                Set(game, "rng", new FixedRandom(0));
                Set(game, "activeContentSet", ContentSetCatalog.SewerSlice);
                Set(game, "visualSmokeSaveBlocked", true);
                Set(game, "launchError", "Mend regression fixture presentation held");
                check(game, state, caster, target, adjacent, enemy);
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        private static CombatUnit Unit(string id, UnitSide side, int x, int y) => new CombatUnit
        {
            Id = id, Name = id, Side = side, PartyIndex = -1, X = x, Y = y,
            Hp = 60, MaxHp = 60, Level = 1, Skills = new SkillSet(), Color = "#ffffff"
        };

        private static HashSet<string> KnownCodes(AshenHallsGame game, CombatUnit caster) =>
            new HashSet<string>(((IEnumerable<FormulaDef>)Invoke(game, "KnownFormulasFor", caster)).Select(formula => formula.Code));
        private static FormulaDef Formula(string code) => FormulaCatalog.All.Single(formula => formula.Code == code);
        private static bool Cast(AshenHallsGame game, CombatUnit caster, string code, CombatUnit target) =>
            (bool)Invoke(game, "CastFormula", caster, code, target, target.X, target.Y);

        private static void Set(AshenHallsGame game, string name, object value)
        {
            FieldInfo field = typeof(AshenHallsGame).GetField(name, PrivateInstance);
            Require(field != null, "runtime field exists: " + name);
            field.SetValue(game, value);
        }

        private static object Invoke(AshenHallsGame game, string name, params object[] arguments)
        {
            MethodInfo method = typeof(AshenHallsGame).GetMethod(name, PrivateInstance);
            Require(method != null, "runtime method exists: " + name);
            return method.Invoke(game, arguments);
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
