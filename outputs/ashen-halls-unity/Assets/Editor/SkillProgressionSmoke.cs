using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class SkillProgressionSmoke
    {
        public static void Run()
        {
            try { RunOrThrow(); Debug.Log(VersionInfo.ProductName + " skill progression smoke passed."); EditorApplication.Exit(0); }
            catch (Exception exception) { Debug.LogError(VersionInfo.ProductName + " skill progression smoke failed: " + exception); EditorApplication.Exit(1); }
        }

        public static void RunOrThrow()
        {
            ActualAttacksAndMultihitShareOnePractice();
            ActualSpellsAndDifferentSchoolsShareTheRightBudgets();
            PracticeSurvivesSavingAndStartsFreshInTheNextEncounter();
            GuardTrainingChangesOnlyTheDeliberateAction();
            GrowthFinishAndMageFallbackHaveExactNativePreviews();
        }

        private static void ActualAttacksAndMultihitShareOnePractice()
        {
            using (Fixture f = new Fixture("warrior"))
            {
                f.Set("rng", new FixedRandom(99));
                Require((bool)f.Call("Attack", f.Hero, f.Enemy) && f.Enemy.Hp == f.Enemy.MaxHp,
                    "a real missed basic attack resolves without damage");
                Require(f.Hero.Skills.Arms == 11 && f.Member.Skills.Arms == 11, "the miss grants and persists exactly one Arms rank");
                f.Set("rng", new FixedRandom(0));
                Require((bool)f.Call("Attack", f.Hero, f.Enemy) && f.Enemy.Hp < f.Enemy.MaxHp, "a later real hit still deals damage");
                Require((bool)f.Call("UseCleave", f.Hero, f.Enemy), "a real multi-target warrior technique resolves");
                Require(f.Hero.Skills.Arms == 11, "repeated attacks and a larger technique grant share the same Arms practice budget");
                f.Call("ImproveSkill", f.Other, "arms", 3);
                Require(f.Other.Skills.Arms == 11, "each persistent party member owns a separate practice budget");
                f.Call("ImproveSkill", f.Hero, "guard", 3);
                Require(f.Hero.Skills.Guard == 30 && f.Hero.PracticedSkillMask == (1 | 32), "different exercised skills have independent budgets");
                string before = JsonUtility.ToJson(f.Hero.Skills);
                f.Call("ImproveSkill", f.Hero, "invalid", 2);
                f.Call("ImproveSkill", f.Hero, "hex", 0);
                f.Call("ImproveSkill", f.Hero, "hex", -1);
                f.Call("ImproveSkill", f.Enemy, "arms", 3);
                Require(JsonUtility.ToJson(f.Hero.Skills) == before && f.Enemy.Skills.Arms == 1,
                    "unknown, nonpositive and enemy practice requests cannot alter skills");
                CombatUnit summoned = f.Other;
                summoned.Summoned = true;
                summoned.PartyIndex = -1;
                f.Call("ImproveSkill", summoned, "hex", 3);
                Require(summoned.Skills.Hex == 10, "summons cannot generate permanent party practice");
            }
            foreach (bool allMiss in new[] { false, true })
            using (Fixture f = new Fixture("ranger"))
            {
                f.Enemy.X = 5; f.Enemy2.X = 5; f.Enemy2.Y = 3;
                f.Set("rng", new FixedRandom(allMiss ? 99 : 0));
                Require((bool)f.Call(allMiss ? "UseQuickShot" : "UseVolley", f.Hero, f.Enemy), "the actual ranger multi-hit technique resolves");
                Require(f.Hero.Skills.Missile == 11 && f.Member.Skills.Missile == 11,
                    "separate miss rolls or a splash hit count cannot multiply Missile practice");
                if (allMiss) Require(f.Enemy.Hp == f.Enemy.MaxHp, "both Twinshot arrows really miss");
                else Require(f.Enemy.Hp < f.Enemy.MaxHp && f.Enemy2.Hp < f.Enemy2.MaxHp, "Arrowstorm really hits both targets");
            }
        }

        private static void ActualSpellsAndDifferentSchoolsShareTheRightBudgets()
        {
            foreach (string classKey in new[] { "mage", "wizard", "warlock", "priest", "paladin" })
            using (Fixture f = new Fixture(classKey))
            {
                bool mend = classKey == "priest" || classKey == "paladin";
                bool hex = classKey == "warlock";
                string simple = mend ? "OBL" : hex ? "GRH" : "FIF";
                string splash = mend ? "SBN" : hex ? "INH" : "CNS";
                string skill = mend ? "mend" : hex ? "hex" : "ember";
                Require(f.Cast(simple, f.Enemy), classKey + " casts a real single-target formula");
                Require(f.Cast(splash, f.Enemy), classKey + " casts a second real campaign formula");
                Require(f.Skill(skill) == 11, classKey + " gets one rank for its school despite repeated and splash casts");
                if (classKey == "wizard")
                {
                    Require(f.Cast("GRH", f.Enemy) && f.Skill("hex") == 11, "wizard's second school keeps its own budget");
                    Require(f.Hero.PracticedSkillMask == (8 | 16), "wizard practice records only the two exercised schools");
                }
            }
            using (Fixture f = new Fixture("priest"))
            {
                f.Hero.Mana = 0;
                Require(!f.Cast("LNH", f.Enemy) && f.Hero.PracticedSkillMask == 0, "a rejected cast awards no practice");
                f.Hero.Mana = 500;
                f.Enemy.MagicResist = 20;
                Require(f.Cast("LNH", f.Enemy) && f.Enemy.Stunned == 0 && f.Hero.Skills.Mend == 11,
                    "an actually resisted but valid cast still earns its one practice rank");
                Require(f.Cast("TBG", f.Other) && f.Other.Shielded == 3 && f.Hero.Skills.Mend == 11,
                    "support and hostile formulas share the same school budget without weakening the actual ward");
            }
        }

        private static void PracticeSurvivesSavingAndStartsFreshInTheNextEncounter()
        {
            using (Fixture f = new Fixture("mage"))
            {
                f.Member.SkillPoints = 12;
                f.Member.StatPoints = 3;
                f.Member.Skills.Ember = f.Hero.Skills.Ember = 99;
                f.Member.Skills.Mend = f.Hero.Skills.Mend = 103;
                f.Call("ImproveSkill", f.Hero, "ember", 3);
                f.Call("ImproveSkill", f.Hero, "mend", 3);
                Require(f.Hero.Skills.Ember == 99 && f.Hero.Skills.Mend == 103 && f.Hero.PracticedSkillMask == 0,
                    "practice never resets existing capped or older over-cap values");
                f.Call("ImproveSkill", f.Hero, "arms", 3);
                f.Reload();
                f.Call("ImproveSkill", f.Hero, "arms", 3);
                Require(f.Hero.Skills.Arms == 11 && f.Member.Skills.Arms == 11 && f.Hero.PracticedSkillMask == 1,
                    "validated production SaveService preserves the consumed budget as well as the earned rank");
                Require(f.Member.Skills.Ember == 99 && f.Member.Skills.Mend == 103 && f.Member.SkillPoints == 12 && f.Member.StatPoints == 3,
                    "save/reload preserves all old skills and unspent points");

                foreach (int invalid in new[] { -1, 64, 127, int.MaxValue })
                {
                    f.Hero.PracticedSkillMask = invalid;
                    Require(!SaveCandidateRules.IsLoadable(f.State, VersionInfo.SaveVersion), "invalid practice-mask bits fail combat save validation");
                }
                f.Hero.PracticedSkillMask = 1;
                string legacy = Regex.Replace(JsonUtility.ToJson(f.State), "\"PracticedSkillMask\"\\s*:\\s*\\d+\\s*,", "");
                File.WriteAllText(f.SavePath, legacy);
                f.Load();
                Require(f.Hero.PracticedSkillMask == 0 && f.Member.Skills.Arms == 11 && f.Member.SkillPoints == 12,
                    "a legacy combat with no mask loads without changing old skills or points");
                f.Call("ImproveSkill", f.Hero, "arms", 2);
                f.Call("ImproveSkill", f.Hero, "arms", 2);
                Require(f.Hero.Skills.Arms == 12, "a legacy ongoing encounter permits at most one new practice per skill");

                int oldArms = f.Hero.Skills.Arms;
                f.State.Mode = GameMode.Explore;
                f.State.Combat = null;
                f.State.Map = (MapData)f.Call("GenerateMap", 1, f.State.Seed);
                f.Call("PlacePlayerAtExplorationStart");
                typeof(AshenHallsGame).GetMethod("StartCombat", BindingFlags.Instance | BindingFlags.NonPublic,
                    null, new[] { typeof(EncounterId) }, null).Invoke(f.Game, new object[] { EncounterId.MidgaardSewer });
                Require(f.Hero.PracticedSkillMask == 0 && f.Hero.Skills.Arms == oldArms, "actual new encounter construction refreshes only the budget");
                f.Call("ImproveSkill", f.Hero, "arms", 99);
                Require(f.Hero.Skills.Arms == oldArms + 1 && f.Member.SkillPoints == 12,
                    "the next encounter grants one new rank without minting or consuming talent points");
            }
        }

        private static void GuardTrainingChangesOnlyTheDeliberateAction()
        {
            foreach (string classKey in new[] { "priest", "paladin", "warrior" })
            foreach (int skill in new[] { 29, 30, 59, 60, 89, 90, 99 })
            using (Fixture f = new Fixture(classKey))
            {
                f.Hero.Skills.Guard = f.Member.Skills.Guard = skill;
                int defense = f.Hero.Defense;
                int unguardedHit = (int)f.Call("AttackHitChance", f.Enemy, f.Hero);
                int expected = 4 + skill / 30;
                Require((int)f.Call("GuardActionBonus", f.Hero) == expected, "braced Guard publishes the exact training tier for " + classKey + " " + skill);
                f.State.Combat.Moved = true;
                Require((int)f.Call("GuardActionBonus", f.Hero) == expected - 2, "movement keeps the exact two-Guard penalty");
                f.State.Combat.Moved = false;
                Require(f.Hero.Defense == defense && (int)f.Call("AttackHitChance", f.Enemy, f.Hero) == unguardedHit,
                    "training does not passively alter defense or incoming hit chance");
                f.Call("SelectOrRunAction", ActionMode.Guard, f.Hero);
                Require(f.Hero.Guarding && f.Hero.GuardBonus == expected, "the real Guard command uses the advertised training tier");
                Require(f.Hero.Defense == defense, "the executed stance leaves permanent defense unchanged");
            }
            using (Fixture f = new Fixture("paladin"))
            {
                f.Hero.Skills.Guard = 90;
                f.Member.Skills.Guard = 90;
                InventoryItem spear = SignatureItemCatalog.CreateBridgehookSpear();
                f.Call("AddInventoryItem", spear);
                f.Member.WeaponItemId = spear.InstanceId;
                spear.EquippedById = f.Member.Id;
                f.Hero.WeaponName = spear.DisplayName;
                f.Hero.ArmorName = "tower shield";
                Require((int)f.Call("GuardActionBonus", f.Hero) == 10,
                    "the bounded +3 training tier preserves +2 equipment and +1 Holdfast rather than replacing them");
                Require((int)f.Call("GuardActionBonus", f.Hero) == 10, "repeated previews never stack the training benefit");
            }
        }

        private static void GrowthFinishAndMageFallbackHaveExactNativePreviews()
        {
            using (Fixture f = new Fixture("mage"))
            {
                f.State.Mode = GameMode.Explore;
                f.State.Combat = null;
                f.Member.SkillPoints = 12;
                f.Member.Skills.Ember = 99;
                f.Member.Skills.Arms = 98;
                ArmoryOverlayScreen screen = f.OpenNativeGrowth();
                Require(screen != null && screen.IsVisible && screen.HasRenderableGeometry,
                    "Growth opens through the native Armory route: " + f.GrowthDiagnostics());
                IReadOnlyList<ArmoryRowView> rows = (IReadOnlyList<ArmoryRowView>)f.Call("BuildArmoryGrowthRows");
                Require(rows.Single(row => row.Title == "Arms").ActionEnabled && !rows.Single(row => row.Title == "Ember").ActionEnabled,
                    "a capped mage has a real enabled Arms fallback and disabled Ember");
                ArmoryRowView arms = rows.Single(row => row.Title == "Arms");
                screen.InvokeRowActionForTest(screen.VisibleRowIndexForKeyForTest(arms.Key));
                ArmoryDetailView detail = (ArmoryDetailView)f.Call("BuildArmoryGrowthDetail");
                Require(detail.Summary.Contains("Arms +1") && detail.Summary.Contains("98 -> 99")
                    && detail.Summary.Contains("Skill 12 -> 11") && detail.Summary.Contains("Accuracy")
                    && detail.Summary.Contains("basic damage") && detail.Summary.Contains("focus"),
                    "the real preview discloses one rank for one point and current/projected weapon benefits");
                Require(f.Member.Skills.Arms == 98 && f.Member.SkillPoints == 12, "native staging does not mutate persistent growth");
                screen.InvokeDetailActionForTest(0);
                Require(f.Member.Skills.Arms == 99 && f.Member.SkillPoints == 11, "native Apply commits the disclosed cap finish");
                screen.InvokeDetailActionForTest(0);
                Require(f.Member.Skills.Arms == 99 && f.Member.SkillPoints == 11, "a repeated native Apply does not spend again");
            }
            using (Fixture f = new Fixture("priest"))
            {
                f.State.Mode = GameMode.Explore; f.State.Combat = null;
                f.Member.SkillPoints = 2; f.Member.Skills.Guard = 28;
                f.OpenNativeGrowth();
                f.Call("RunArmoryGrowthRowAction", 20000 + (int)PartyGrowthChoice.Guard);
                ArmoryDetailView detail = (ArmoryDetailView)f.Call("BuildArmoryGrowthDetail");
                Require(detail.Summary.Contains("Guard action +0 -> +1") && detail.Summary.Contains("next at 60 (30 ranks)")
                    && detail.Summary.Contains("Moving costs 2 Guard"), "native Guard preview gives the tier crossing and next exact threshold");
                f.Call("RunArmoryGrowthDetailAction", 20101);
                Require(f.Member.Skills.Guard == 28 && f.Member.SkillPoints == 2, "Reset returns the entire uncommitted Guard plan");
            }
            PartyMember boundary = new PartyMember { ClassKey = "mage", Skills = new SkillSet { Ember = 98, Arms = 10 }.Normalize(), SkillPoints = 3 };
            PartyGrowthPlan plan = new PartyGrowthPlan();
            Require(PartyGrowthRules.TryStage(boundary, plan, PartyGrowthChoice.Ember, out _), "one cap finish stages");
            Require(!PartyGrowthRules.TryStage(boundary, plan, PartyGrowthChoice.Ember, out _), "a redundant capped purchase cannot stage");
            Require(PartyGrowthRules.TryApply(boundary, plan, out string summary) && summary.Contains("Ember +1"), "cap summary reports the actual gain");
            Require(!PartyGrowthRules.TryApply(boundary, plan, out _) && boundary.SkillPoints == 2, "a stale cap plan cannot spend twice");
            boundary.Skills.Ember = 98;
            plan = new PartyGrowthPlan(); plan.Increment(PartyGrowthChoice.Ember); plan.Increment(PartyGrowthChoice.Ember);
            Require(!PartyGrowthRules.TryApply(boundary, plan, out _) && boundary.Skills.Ember == 98 && boundary.SkillPoints == 2,
                "an excessive manually assembled cap plan fails atomically");
            foreach (int malformed in new[] { 100, int.MaxValue })
            {
                boundary.Skills.Ember = malformed;
                plan = new PartyGrowthPlan(); plan.Increment(PartyGrowthChoice.Arms);
                Require(!PartyGrowthRules.Validate(boundary, plan, out _)
                    && !PartyGrowthRules.TryApply(boundary, plan, out _)
                    && boundary.Skills.Ember == malformed && boundary.Skills.Arms == 10 && boundary.SkillPoints == 2,
                    "malformed over-cap skill blocks all Growth purchases without resetting it or spending points");
            }
        }

        private sealed class Fixture : IDisposable
        {
            private readonly GameObject host = RemembranceItemSmoke.NewHost("Skill progression fixture");
            private readonly int[] initialCanvasIds = LiveObjectIds<Canvas>();
            private readonly int[] initialOrnamentIds = LiveObjectIds<UiOrnament>();
            public readonly AshenHallsGame Game;
            private ArmoryOverlayScreen detachedArmory;
            public readonly string SavePath = Path.Combine(Path.GetTempPath(), "ashen-skill-progression-" + Guid.NewGuid().ToString("N") + ".json");
            public GameState State => Get<GameState>("state");
            public PartyMember Member => State.Party[0];
            public CombatUnit Hero => State.Combat.Units.First(unit => unit.PartyIndex == 0 && unit.Side == UnitSide.Party);
            public CombatUnit Other => State.Combat.Units.First(unit => unit.PartyIndex == 1 && unit.Side == UnitSide.Party);
            public CombatUnit Enemy => State.Combat.Units.First(unit => unit.Id == "growth-enemy");
            public CombatUnit Enemy2 => State.Combat.Units.First(unit => unit.Id == "growth-enemy-2");

            public Fixture(string classKey)
            {
                Game = host.AddComponent<AshenHallsGame>();
                string role = classKey == "ranger" ? "bow" : classKey == "priest" ? "mender" : classKey == "paladin" ? "ward"
                    : classKey == "warlock" ? "hex" : classKey == "warrior" ? "shield" : "ember";
                PartyMember member = new PartyMember { Id = "growth-hero", Name = "Practice hero", ClassKey = classKey, Role = role, Race = "human",
                    Level = 20, Stats = new Stats(10, 10, 10, 10), Spell = StarterPartyCatalog.SpellSchoolForClass(classKey),
                    Skills = new SkillSet { Arms = 10, Missile = 10, Ember = 10, Mend = 10, Hex = 10, Guard = 29 },
                    Hp = 100, MaxHp = 100, Mana = 500, MaxMana = 500, Movement = 3, Range = classKey == "ranger" ? 5 : 1,
                    WeaponName = classKey == "ranger" ? "training bow" : "training sword", WeaponDamageType = "physical", WeaponDamageMin = 3,
                    WeaponDamageMax = 5, WeaponAttackSpeed = 7, DamageMin = 3, DamageMax = 5, AttackSpeed = 7, Defense = 2, Agility = 4 };
                PartyMember other = member.CloneForPreview(); other.Id = "growth-other"; other.Name = "Other hero";
                CombatUnit hero = Unit(member, 0, 2, 2);
                CombatUnit ally = Unit(other, 1, 1, 2);
                CombatUnit enemy = new CombatUnit { Id = "growth-enemy", Name = "Practice target", Side = UnitSide.Enemy, PartyIndex = -1,
                    X = 3, Y = 2, Hp = 1000, MaxHp = 1000, Skills = new SkillSet().Normalize(), DamageType = "physical", Range = 1, DamageMin = 2, DamageMax = 4 };
                CombatUnit enemy2 = JsonUtility.FromJson<CombatUnit>(JsonUtility.ToJson(enemy)); enemy2.Id = "growth-enemy-2"; enemy2.X = 4;
                Set("state", new GameState { SaveVersion = VersionInfo.SaveVersion, ContentSetId = ContentSetCatalog.SewerSlice,
                    Mode = GameMode.Combat, Depth = 1, Seed = 2360, ReducedMotion = true, SfxMuted = true, MusicMuted = true,
                    Party = new List<PartyMember> { member, other }, Combat = new CombatState { ActiveId = hero.Id, Round = 1,
                        Phase = CombatPhase.ChooseAction, ActionAvailable = true, MovePoints = 3,
                        Units = new List<CombatUnit> { hero, enemy, ally, enemy2 },
                        InitiativeQueue = new List<string> { hero.Id, enemy.Id, ally.Id, enemy2.Id } } });
                Set("activeContentSet", ContentSetCatalog.SewerSlice);
                Set("rng", new FixedRandom(0));
                Set("visualSmokeSaveBlocked", true);
                Set("launchError", "Skill progression fixture holds presentation");
            }

            private static CombatUnit Unit(PartyMember member, int index, int x, int y) => new CombatUnit {
                Id = member.Id, Name = member.Name, Side = UnitSide.Party, PartyIndex = index, ClassKey = member.ClassKey, Role = member.Role,
                Race = member.Race, Level = member.Level, X = x, Y = y, Hp = member.Hp, MaxHp = member.MaxHp,
                Mana = member.Mana, MaxMana = member.MaxMana, Movement = member.Movement, Skills = member.Skills.Clone(), Spell = member.Spell,
                WeaponName = member.WeaponName, DamageType = "physical", DamageMin = 3, DamageMax = 5, Power = 6,
                Range = member.Range, AttackSpeed = 7, Agility = 4, Defense = 2 };
            public int Skill(string skill) => (int)Call("SkillValue", Hero.Skills, skill);
            public ArmoryOverlayScreen OpenNativeGrowth()
            {
                // Keep the game host inactive (so Awake cannot boot/load), but let its actual bound native screen own a visible canvas.
                Set("launchError", "");
                Call("EnsureArmoryOverlayScreen");
                detachedArmory = Get<ArmoryOverlayScreen>("armoryOverlayScreen");
                detachedArmory.transform.SetParent(null, false);
                detachedArmory.gameObject.SetActive(true);
                Call("ToggleArmory", 4);
                return detachedArmory;
            }
            public string GrowthDiagnostics()
            {
                ArmoryOverlayScreen screen = Get<ArmoryOverlayScreen>("armoryOverlayScreen");
                return "screen=" + (screen != null) + ", visible=" + (screen?.IsVisible ?? false)
                    + ", ready=" + (screen?.IsReady ?? false) + ", geometry=" + (screen?.HasRenderableGeometry ?? false)
                    + ", showArmory=" + Get<bool>("showArmory") + ", overlay=" + Call("CurrentUiOverlay")
                    + ", splash=" + Call("ShouldShowStartupSplash");
            }
            public bool Cast(string code, CombatUnit target) => (bool)Call("CastFormula", Hero, code, target, target.X, target.Y);
            public void Reload() { Require(SaveService.TrySaveCampaignState(SavePath, State, false, out string reason), reason); Load(); }
            public void Load() { Set("state", SaveService.LoadGameState(SavePath, value => SaveCandidateRules.IsLoadable(value, VersionInfo.SaveVersion), out bool backup)); Require(!backup, "the primary disposable campaign save validates"); }
            public object Call(string name, params object[] args)
            {
                if (name == "ToggleArmory")
                    return typeof(AshenHallsGame).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic, null,
                        new[] { typeof(int) }, null).Invoke(Game, args);
                return RemembranceItemSmoke.Invoke(Game, name, args);
            }
            public void Set(string name, object value) => RemembranceItemSmoke.Set(Game, name, value);
            public T Get<T>(string name) => (T)typeof(AshenHallsGame).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Game);
            public void Dispose()
            {
                // Unity does not call OnDestroy on components that have never been active. Bound screens on this
                // intentionally inactive game host still own independent active root canvases, so release their exact leases.
                ReleaseOwnedCanvases(host);
                if (detachedArmory != null)
                {
                    ReleaseOwnedCanvases(detachedArmory.gameObject);
                    UnityEngine.Object.DestroyImmediate(detachedArmory.gameObject);
                }
                UnityEngine.Object.DestroyImmediate(host);
                foreach (string suffix in new[] { "", ".bak", ".tmp" }) if (File.Exists(SavePath + suffix)) File.Delete(SavePath + suffix);
                int[] canvases = LiveObjectIds<Canvas>();
                int[] ornaments = LiveObjectIds<UiOrnament>();
                Require(canvases.SequenceEqual(initialCanvasIds) && ornaments.SequenceEqual(initialOrnamentIds),
                    "fixture restores native UI ownership exactly: canvases " + initialCanvasIds.Length + " -> " + canvases.Length
                    + ", ornaments " + initialOrnamentIds.Length + " -> " + ornaments.Length
                    + "; surviving canvases: " + string.Join(", ", UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None).Select(canvas => canvas.name)));
            }
        }

        private static int[] LiveObjectIds<T>() where T : UnityEngine.Object =>
            UnityEngine.Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Select(value => value.GetInstanceID()).OrderBy(value => value).ToArray();

        private static void ReleaseOwnedCanvases(GameObject owner)
        {
            if (owner == null) return;
            foreach (Component component in owner.GetComponentsInChildren<Component>(true))
                if (component != null && component.GetType().Name == "UiOwnedCanvasLifetime")
                    component.GetType().GetMethod("OnDestroy", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(component, null);
        }

        private sealed class FixedRandom : System.Random
        {
            private readonly int value;
            public FixedRandom(int value) { this.value = value; }
            public override int Next(int maxValue) => Math.Min(value, maxValue - 1);
            public override int Next(int minValue, int maxValue) => minValue;
            public override double NextDouble() => value == 0 ? .5 : .99;
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("Skill progression: " + message);
        }
    }
}
