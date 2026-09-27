using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class WeaponTrainingSmoke
    {
        private static readonly string[] ShotIds = { "aimedshot", "pinningshot", "volley", "broadheadshot", "disruptingshot", "quickshot" };

        public static void Run()
        {
            try { RunOrThrow(); Debug.Log("Weapon training smoke passed."); EditorApplication.Exit(0); }
            catch (Exception exception) { Debug.LogError("Weapon training smoke failed: " + exception); EditorApplication.Exit(1); }
        }

        public static void RunOrThrow()
        {
            int ornaments = UnityEngine.Object.FindObjectsByType<UiOrnament>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length;
            SharedProfilesAndActualEquipmentAgree();
            ReachAndFocusUseSightWithoutMissileSkill();
            RangerShotsRequireAMissileWeaponAtEveryEntry();
            WeaponTradeoffsRequireReviewAndDiscloseLostShots();
            SpellsAndIndependentTechniquesKeepTheirRanges();
            SavedRangesRepairWithoutChangingCombatOrOwnership();
            EnemySummonAndDemonProfilesStayAuthored();
            Require(UnityEngine.Object.FindObjectsByType<UiOrnament>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length == ornaments,
                "native fixtures release their owned root canvases and ornament owners");
        }

        private static void SharedProfilesAndActualEquipmentAgree()
        {
            foreach (string cls in StarterPartyCatalog.SelectableClassKeys)
            using (Fixture f = new Fixture(cls))
            {
                bool ranger = cls == "ranger";
                bool caster = StarterPartyCatalog.SpellSchoolForClass(cls).Length > 0;
                foreach (string form in new[] { "broadsword", "battle axe", "ritual knife", "long spear", "pike", "halberd", "lance", "longbow", "crossbow", "sling", "throwing knives", "throwing darts", "prayer focus", "ember focus", "bone focus", "ritual staff", "stormglass orb", "scepter" })
                {
                    InventoryItem item = f.Equip(form);
                    int expected = new[] { "longbow", "crossbow" }.Contains(form) ? (ranger ? 5 : 4)
                        : new[] { "sling", "throwing knives", "throwing darts" }.Contains(form) ? (ranger ? 4 : 3)
                        : new[] { "long spear", "pike", "halberd", "lance" }.Contains(form) ? 2
                        : new[] { "prayer focus", "ember focus", "bone focus", "ritual staff", "stormglass orb", "scepter" }.Contains(form) ? (caster ? 4 : 1) : 1;
                    Require(f.Member.Range == expected && (int)f.Call("BaseAttackRange", f.Hero) == expected
                        && (int)f.Call("EffectiveWeaponRange", item, f.Member) == expected,
                        cls + "/" + form + " uses one range in inventory, loadout and combat");
                    f.Member.Range = 99;
                    f.Call("RecalculateMember", f.Member);
                    Require(f.Member.Range == expected, "Growth/stat recalculation cannot retain an obsolete range floor");
                    f.Call("SyncEnchantedWeaponToOwner", item);
                    Require(f.Member.Range == expected, "enchantment owner sync preserves the form rule");
                }
            }
            Require(WeaponAttackRules.For(null, "+3 balanced epee of focus", "ember", "mage").Range == 1,
                "balanced does not mean lance, and an enchantment name does not change a blade into a focus");
            Require(WeaponAttackRules.For(new InventoryItem { Form = "knife", DisplayName = "longbow" }, "longbow", "bow", "ranger").Range == 1,
                "authored inventory form wins over a renamed display label");
            Require(WeaponAttackRules.For(null, "+2 Stormglass Conductor", "ember", "mage").Range == 4,
                "name-only legacy signature uses its canonical staff form");
            Require(WeaponAttackRules.For(null, "unknown heirloom", "bow", "ranger").Range == 1,
                "unknown named equipment does not manufacture ranged fire");
            using (Fixture f = new Fixture())
            {
                List<PartyMember> starters = (List<PartyMember>)f.Call("MakeDefaultParty");
                Require(starters.Select(member => member.Range).SequenceEqual(new[] { 1, 5, 4, 4 }),
                    "actual default party normalizes both caster implements to four");
                Require(starters[2].Stats.Intelligence == 22 && starters[3].Skills.Mend == 9,
                    "starter attributes and learned talents are unchanged");
            }
        }

        private static void ReachAndFocusUseSightWithoutMissileSkill()
        {
            foreach (string form in new[] { "long spear", "ritual staff" })
            using (Fixture f = new Fixture("mage"))
            {
                f.Equip(form);
                f.Enemy.X = form == "long spear" ? 4 : 6;
                Point blocker = new Point(3, 2, "stone", 0) { Integrity = 3 };
                f.State.Combat.Obstacles.Add(blocker);
                string combat = JsonUtility.ToJson(f.State.Combat);
                int calls = f.Random.Calls;
                CombatAttackForecast blocked = (CombatAttackForecast)f.Call("AttackForecast", f.Hero, f.Enemy);
                Require(!blocked.Legal && blocked.BlockReason == AttackForecastBlockReason.LineOfSight && !blocked.Ranged,
                    form + " reports blocked sight without claiming Missile skill");
                Require(!(bool)f.Call("Attack", f.Hero, f.Enemy)
                    && JsonUtility.ToJson(f.State.Combat) == combat && f.Random.Calls == calls,
                    "blocked unit attack spends no action, practice, HP, mana or random roll");

                int targetX = f.Enemy.X;
                f.Enemy.X = 9; f.Enemy.Y = 6;
                foreach (string kind in new[] { "tree", "glyph" })
                {
                    Point target = new Point(targetX, 2, kind, kind == "glyph" ? 3 : 0) { Integrity = 3 };
                    f.State.Combat.Obstacles.Add(target);
                    string before = JsonUtility.ToJson(f.State.Combat);
                    Require(!(bool)f.Call("CanAttackCombatObstacle", f.Hero, target)
                        && ((string)f.Call("CombatObstacleAttackBlockReason", f.Hero, target)).Contains("sight"),
                        "obstacle pointer uses the same sight rule");
                    Require(((string)f.Call(kind == "tree" ? "CoverAttackPreview" : "RitualAttackPreview", f.Hero, target)).Contains("blocked"),
                        "obstacle preview explains the obstruction");
                    Require(!(bool)f.Call(kind == "tree" ? "AttackCover" : "AttackRitual", f.Hero, target)
                        && JsonUtility.ToJson(f.State.Combat) == before && f.Random.Calls == calls,
                        "blocked cover/ritual execution cannot damage, practice or spend a roll");
                    f.State.Combat.Obstacles.Remove(target);
                }
                f.State.Combat.Obstacles.Remove(blocker);
                f.Enemy.X = targetX; f.Enemy.Y = 2;
                f.State.Combat.Units.Add(new CombatUnit { Id = "pressure", Side = UnitSide.Enemy, Hp = 40, MaxHp = 40, X = 2, Y = 3 });
                CombatAttackForecast clear = (CombatAttackForecast)f.Call("AttackForecast", f.Hero, f.Enemy);
                Require(clear.Legal && !clear.Ranged && clear.Range == (form == "long spear" ? 2 : 4)
                    && (string)f.Call("AttackSkillName", f.Hero, f.Enemy) == "arms",
                    "focus/reach retain range and Arms while an adjacent foe engages them");
                int hp = f.Enemy.Hp, arms = f.Hero.Skills.Arms, missile = f.Hero.Skills.Missile;
                Require((bool)f.Call("Attack", f.Hero, f.Enemy), "clear form attack succeeds");
                int damage = hp - f.Enemy.Hp;
                Require(damage >= clear.MinDamage && damage <= clear.MaxDamage
                    && f.Hero.Skills.Arms == arms + 1 && f.Hero.Skills.Missile == missile,
                    "first eligible basic attack matches its forecast and practices Arms once in its fresh battle");
            }
        }

        private static void RangerShotsRequireAMissileWeaponAtEveryEntry()
        {
            using (Fixture f = new Fixture("ranger"))
            {
                f.Equip("broadsword"); f.Enemy.X = 5;
                f.Set("showAbilityPanel", true); f.Set("showSpellbook", false);
                f.Call("EnsureCombatAbilityModalScreen");
                CombatAbilityModalScreen screen = f.Get<CombatAbilityModalScreen>("combatAbilityModalScreen");
                screen.Refresh();
                // Hunter's Mark is ready, so the native book initially filters out
                // the unavailable shots. Inspect them through its real All filter.
                screen.SetFilterForTest(CombatAbilityModalFilter.All);
                foreach (string id in ShotIds)
                {
                    MartialAbility ability = AbilityCatalog.For(id);
                    object[] usable = { f.Hero, ability, "" };
                    object[] target = { f.Hero, ability, f.Enemy, f.Enemy.X, f.Enemy.Y, "" };
                    Require(!(bool)f.Call("AbilityUsableNow", usable) && ((string)usable[2]).Contains("Equip"), "shot availability names the missing weapon");
                    Require(!(bool)f.Call("CanTargetAbility", target) && ((string)target[5]).Contains("Equip"), "direct target preview rejects sword shots");
                    f.Call("SelectCombatAbilityModalCard", id); screen.Refresh();
                    CombatAbilityModalView view = (CombatAbilityModalView)f.Call("BuildCombatAbilityModalView");
                    CombatAbilityModalCardView card = view.Cards.Single(value => value.Id == id);
                    Require(!card.Usable && card.ValidTargetCount == 0 && card.DisabledReason.Contains("Equip") && card.CurrentEffect.Contains("Equip")
                        && screen.DetailIdForTest == id && !screen.DetailActionInteractableForTest,
                        "native modal disables the selected shot and exposes the same equipment requirement: id=" + id
                        + ", usable=" + card.Usable + ", targets=" + card.ValidTargetCount + ", reason=" + card.DisabledReason
                        + ", detail=" + screen.DetailIdForTest + ", interactable=" + screen.DetailActionInteractableForTest
                        + ", visible=" + view.Visible + ", overlay=" + f.Call("CurrentUiOverlay") + ", armory=" + f.Get<bool>("showArmory"));
                    string before = JsonUtility.ToJson(f.State.Combat); int calls = f.Random.Calls;
                    screen.InvokeDetailActionForTest();
                    Require(!(bool)f.Call("UseTargetedAbility", f.Hero, id, f.Enemy, f.Enemy.X, f.Enemy.Y)
                        && JsonUtility.ToJson(f.State.Combat) == before && f.Random.Calls == calls,
                        "native and direct rejected shot cannot consume action, apply damage/status, practice or roll RNG");
                }
                Require((bool)f.Call("AbilityUsableNow", f.Hero, AbilityCatalog.For("scoutmark"), "")
                    && (bool)f.Call("CanTargetAbility", f.Hero, AbilityCatalog.For("scoutmark"), f.Enemy, f.Enemy.X, f.Enemy.Y, ""),
                    "Hunter's Mark remains a useful equipment-independent action");
                f.Enemy.X = 3;
                Require(((CombatAttackForecast)f.Call("AttackForecast", f.Hero, f.Enemy)).Legal
                    && (string)f.Call("AttackSkillName", f.Hero, f.Enemy) == "arms", "sword ranger retains a legal Arms attack");
                foreach (string form in new[] { "longbow", "crossbow", "sling", "throwing darts" })
                {
                    f.Equip(form); f.Enemy.X = 7;
                    foreach (string id in ShotIds)
                        Require((bool)f.Call("AbilityUsableNow", f.Hero, AbilityCatalog.For(id), "")
                            && (bool)f.Call("CanTargetAbility", f.Hero, AbilityCatalog.For(id), f.Enemy, f.Enemy.X, f.Enemy.Y, "")
                            && AbilityCatalog.For(id).Range == 6, "compatible missile equipment restores shots at their unchanged technique range");
                }
            }
            using (Fixture f = new Fixture("ranger"))
            {
                f.Equip("longbow"); f.Enemy.X = 6;
                Require((string)f.Call("AttackSkillName", f.Hero, f.Enemy) == "missile", "physical projectile practices Missile at distance");
                int missile = f.Hero.Skills.Missile, arms = f.Hero.Skills.Arms;
                Require((bool)f.Call("Attack", f.Hero, f.Enemy) && f.Hero.Skills.Missile == missile + 1 && f.Hero.Skills.Arms == arms,
                    "fresh projectile battle practices only Missile");
                f.State.Combat.Units.Add(new CombatUnit { Id = "pressure", Side = UnitSide.Enemy, Hp = 40, MaxHp = 40, X = 2, Y = 3 });
                Require((int)f.Call("EffectiveAttackRangeFrom", f.Hero, 2, 2) == 1
                    && !((CombatAttackForecast)f.Call("AttackForecast", f.Hero, f.Enemy)).Legal,
                    "adjacent pressure still suppresses basic physical missile fire");
            }
        }

        private static void SpellsAndIndependentTechniquesKeepTheirRanges()
        {
            foreach (string cls in new[] { "mage", "wizard", "warlock", "priest", "paladin" })
            using (Fixture f = new Fixture(cls))
            {
                FormulaDef formula = (FormulaDef)f.Call("GetFormula", cls == "priest" || cls == "paladin" ? "OBL" : cls == "warlock" ? "RNH" : "FIF");
                Require(formula != null, "formula fixture is authored");
                foreach (string form in new[] { "broadsword", "ritual staff", "long spear" })
                {
                    f.Equip(form); f.State.Combat.MovePoints = f.Hero.Movement; f.State.Combat.Moved = false;
                    Require((bool)f.Call("IsFocusedCaster", f.Hero)
                        && (int)f.Call("EffectiveFormulaRange", formula, f.Hero) == formula.Range + 1,
                        "stationary spell range is independent of the held weapon");
                    f.State.Combat.Moved = true;
                    Require((int)f.Call("EffectiveFormulaRange", formula, f.Hero) == formula.Range,
                        "moving removes only the existing spell focus bonus");
                }
            }
            using (Fixture f = new Fixture("rogue"))
            {
                f.Equip("battle axe"); f.Enemy.X = 5;
                Require((bool)f.Call("AbilityUsableNow", f.Hero, AbilityCatalog.For("throwknife"), "")
                    && (bool)f.Call("CanTargetAbility", f.Hero, AbilityCatalog.For("throwknife"), f.Enemy, 5, 2, "")
                    && AbilityCatalog.For("throwknife").Range == 3, "Serrated Throw remains an independent short-range knife technique");
            }
        }

        private static void WeaponTradeoffsRequireReviewAndDiscloseLostShots()
        {
            using (Fixture f = new Fixture("ranger"))
            {
                InventoryItem bow = f.Equip("longbow");
                f.State.Mode = GameMode.Explore; f.State.Combat = null;
                InventoryItem sword = new InventoryItem { Form = "broadsword", Slot = "weapon", DisplayName = "fine iron broadsword",
                    DamageMin = 20, DamageMax = 30, Bonus = 4, AttackSpeed = 12, DamageType = "physical" };
                f.Call("AddInventoryItem", sword);
                Require(!InventoryEquipmentRules.RequiresDeliberateReview(sword, bow), "ordinary comparison has no intrinsic or enchantment requiring review");
                Require((int)f.Call("InventoryComparisonScore", sword, f.Member) > 3, "ordinary sword is a numerical upgrade under unchanged scoring");
                Require((bool)f.Call("InventoryChangeRequiresReview", sword, f.Member)
                    && (string)f.Call("InventoryGradeLabelFor", sword, f.Member) == "Review"
                    && !(bool)f.Call("InventoryItemIsUpgrade", sword), "loss of range and shooting prevents a blind upgrade badge");
                Require(((string)f.Call("AutoEquipItem", sword)).StartsWith("Review for", StringComparison.Ordinal),
                    "arrival guidance calls a role-changing numerical improvement a review");
                f.Call("ShowLootPanel", sword, 0, 0, 0, "Weapon tradeoff fixture", "Weapon review");
                LootPopupView loot = (LootPopupView)f.Call("BuildLootPopupView");
                Require(!loot.CanQuickEquip && loot.ReviewActionLabel == "Review tradeoff"
                    && loot.Comparison.Contains("Range 5→1") && loot.Comparison.Contains("shooting skills unavailable"),
                    "ordinary sword loot discloses the lost role and routes to review");
                f.Call("EquipLootToBestFit");
                Require(f.Member.WeaponItemId == bow.InstanceId && string.IsNullOrEmpty(sword.EquippedById)
                    && f.Member.Range == 5, "direct quick-equip callback cannot bypass the review gate");
                f.Call("DismissLootPopup");

                InventoryItem crownward = SignatureItemCatalog.CreateCrownwardWarblade();
                f.Call("AddInventoryItem", crownward);
                f.Call("SelectArmoryInventoryItem", crownward);
                ArmoryOverlayScreen screen = f.OpenNativePack();
                screen.Refresh();
                ArmoryDetailView detail = (ArmoryDetailView)f.Call("BuildInventoryItemDetail");
                Require(detail.Actions[0].Detail.StartsWith("Review · Range 5→1; shots disabled", StringComparison.Ordinal)
                    && detail.Actions[0].ButtonLabel == "Equip" && detail.Actions[0].Enabled,
                    "Pack leads with the actual lost role while preserving deliberate equipment choice");
                // Native overlays own separate root canvases, not child canvases.
                Canvas nativeCanvas = (Canvas)typeof(ArmoryOverlayScreen).GetField("canvas", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(screen);
                UnityEngine.UI.Text[] nativeTexts = nativeCanvas.GetComponentsInChildren<UnityEngine.UI.Text>(true);
                UnityEngine.UI.Text nativeComparison = nativeTexts.FirstOrDefault(text => text.name == "Comparison" && text.transform.parent.name == "Equip Target 0");
                ArmoryOverlayView nativeView = (ArmoryOverlayView)f.Call("BuildArmoryOverlayView");
                Require(nativeComparison != null, "native Pack comparison exists: ready=" + screen.IsReady + ", visible=" + screen.IsVisible
                    + ", detail=" + screen.HasVisibleDetailForTest + ", view=" + nativeView.Visible + ", overlay=" + f.Call("CurrentUiOverlay")
                    + ", armory=" + f.Get<bool>("showArmory") + ", splash=" + f.Call("ShouldShowStartupSplash")
                    + ", texts=" + string.Join(" | ", nativeTexts.Select(text => text.name + "@" + text.transform.parent.name + "=" + text.text)));
                Require(screen.HasVisibleDetailForTest && screen.VisibleDetailActionCountForTest > 0
                    && nativeComparison.text == detail.Actions[0].Detail,
                    "the native Pack action receives the complete leading warning");
                screen.InvokeDetailActionForTest(0);
                Require(f.Member.WeaponItemId == crownward.InstanceId && f.Member.Range == 1,
                    "explicit native Pack Equip still permits the chosen melee loadout");
                Require(((string)f.Call("RoleIdentityLine", f.Member)).Contains("melee weapon")
                    && !((string)f.Call("RoleIdentityLine", f.Member)).Contains("missile")
                    && ((string)f.Call("PartySummaryLine")).Contains("ranged 0")
                    && ((string)f.Call("PartyWeaknessLine")).Contains("no ranged weapons"),
                    "party guidance preserves ranger identity while reporting its actual melee equipment");
                string inverse = (string)f.Call("CompactInventoryComparisonLine", bow, f.Member);
                Require(inverse.StartsWith("Range 1→5; shots enabled", StringComparison.Ordinal)
                    && (bool)f.Call("InventoryChangeRequiresReview", bow, f.Member), "the inverse swap discloses restored shooting and changed delivery");
                f.Call("SelectArmoryInventoryItem", bow); screen.Refresh(); screen.InvokeDetailActionForTest(0);
                Require(f.Member.WeaponItemId == bow.InstanceId && f.Member.Range == 5, "the native inverse choice restores the bow");

                PartyMember warrior = (PartyMember)f.Call("MakeHero", "Sword bearer", "human", "warrior", "shield", new Stats(18, 18, 18, 18), "", 99, new SkillSet().Normalize());
                f.State.Party.Add(warrior);
                object[] equip = { sword, warrior, "" };
                Require((bool)f.Call("EquipInventoryItemToMember", equip), "reassignment fixture equips its real replacement sword");
                ArmoryDetailActionView reassignment = (ArmoryDetailActionView)f.Call("BuildInventoryTargetAction", sword, warrior, 0, false);
                Require(reassignment.Detail.StartsWith("Review · Range 5→1; shots disabled", StringComparison.Ordinal)
                    && reassignment.ButtonLabel == "Swap" && reassignment.Enabled,
                    "reassignment also exposes the target's lost shooting while retaining the explicit swap");
            }
        }

        private static void SavedRangesRepairWithoutChangingCombatOrOwnership()
        {
            foreach (string cls in new[] { "ranger", "mage" })
            using (Fixture f = new Fixture(cls))
            {
                InventoryItem item = f.Equip("broadsword");
                f.Member.Range = 5; f.Hero.Range = 5;
                f.Hero.Hp = 31; f.Hero.Mana = Math.Min(4, f.Hero.MaxMana); f.Hero.Shielded = 2;
                string combat = f.CombatWithoutRanges();
                string instance = item.InstanceId, owner = item.EquippedById;
                int inventoryCount = f.State.Inventory.Count;
                f.Reload(VersionInfo.SaveVersion);
                Require(f.Member.Range == 1 && f.Hero.Range == 1 && f.CombatWithoutRanges() == combat,
                    "actual midcombat save/adoption repairs stale range without changing actors or turn resources");
                Require(f.Member.WeaponItemId == instance && f.State.Inventory.Single(value => value.InstanceId == instance).EquippedById == owner
                    && f.State.Inventory.Count == inventoryCount, "range repair retains physical inventory ownership");
                string state = JsonUtility.ToJson(f.State);
                f.Call("NormalizePartyWeaponRanges");
                Require(JsonUtility.ToJson(f.State) == state, "derived-range repair is idempotent");
            }
            using (Fixture f = new Fixture("ranger"))
            {
                InventoryItem item = f.Equip("broadsword");
                f.Member.WeaponItemId = ""; f.Member.Range = 5; f.Hero.Range = 5;
                f.State.Mode = GameMode.Explore; f.State.Combat = null;
                f.State.Map = (MapData)f.Call("GenerateMap", 1, f.State.Seed);
                f.State.PlayerX = f.State.Map.StartX; f.State.PlayerY = f.State.Map.StartY;
                f.Reload(InventoryItemIdentityRules.SchemaVersion - 1);
                Require(f.Member.Range == 1 && f.Member.WeaponItemId == item.InstanceId
                    && f.State.Inventory.Single(value => value.InstanceId == item.InstanceId).EquippedById == f.Member.Id,
                    "old name/stat-based unique equipment repair tolerates obsolete derived range");
            }
        }

        private static void EnemySummonAndDemonProfilesStayAuthored()
        {
            using (Fixture f = new Fixture("ranger"))
            {
                foreach (string role in new[] { "meteorlich", "ritualheart", "drowcrossbow", "bonepriest", "gloamknight", "koboldking", "pike" })
                {
                    CombatUnit enemy = (CombatUnit)f.Call("MakeEnemy", role, 0, "");
                    int authored = enemy.Range;
                    Require((int)f.Call("BaseAttackRange", enemy) == authored
                        && (bool)f.Call("IsRangedAttackProfile", enemy) == (authored > 1 && role != "pike"),
                        "enemy " + role + " retains its authored range/profile");
                    CombatUnit summon = new CombatUnit { Side = UnitSide.Party, Summoned = true, Role = "bound spirit", Range = 4, WeaponName = "spirit claws" };
                    Require((int)f.Call("BaseAttackRange", summon) == 4 && !(bool)f.Call("IsRangedAttackProfile", summon),
                        "party summon retains its original branch");
                }
                f.Equip("longbow"); f.Hero.DemonFormTurns = 2;
                Require((int)f.Call("BaseAttackRange", f.Hero) == 1
                    && (string)f.Call("AttackSkillName", f.Hero, f.Enemy) == "hex",
                    "demon form still overrides equipment with adjacent Hex attacks");
            }
        }

        private sealed class Fixture : IDisposable
        {
            private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
            private readonly GameObject host = new GameObject("Weapon training smoke");
            private ArmoryOverlayScreen detachedArmory;
            private readonly string savePath = Path.Combine(Path.GetTempPath(), "ashen-weapon-training-" + Guid.NewGuid().ToString("N") + ".json");
            public readonly AshenHallsGame Game;
            public readonly CountingRandom Random = new CountingRandom();
            public GameState State => Get<GameState>("state");
            public PartyMember Member => State.Party[0];
            public CombatUnit Hero => State.Combat.Units[0];
            public CombatUnit Enemy => State.Combat.Units[1];

            public Fixture(string classKey = "warrior")
            {
                host.SetActive(false); Game = host.AddComponent<AshenHallsGame>();
                Set("state", new GameState { Seed = 2360, Depth = 1, ContentSetId = ContentSetCatalog.SewerSlice, SaveVersion = VersionInfo.SaveVersion,
                    SfxMuted = true, MusicMuted = true, ReducedMotion = true, Mode = GameMode.Combat });
                Set("activeContentSet", ContentSetCatalog.SewerSlice); Set("visualSmokeSaveBlocked", true); Set("rng", Random);
                string role = (string)Call("RoleForClass", classKey);
                PartyMember member = (PartyMember)Call("MakeHero", "Training hero", "human", classKey, role,
                    new Stats(18, 18, 18, 18), StarterPartyCatalog.SpellSchoolForClass(classKey), 99,
                    new SkillSet { Arms = 8, Missile = 8, Ember = 8, Mend = 8, Hex = 8, Guard = 8 }.Normalize());
                member.Level = 16; State.Party.Add(member); Call("RecalculateMember", member); member.Hp = member.MaxHp; member.Mana = member.MaxMana;
                CombatUnit hero = new CombatUnit { Id = member.Id, Name = member.Name, Side = UnitSide.Party, PartyIndex = 0,
                    Role = role, ClassKey = classKey, Race = member.Race, Level = 16, X = 2, Y = 2, Hp = member.Hp, MaxHp = member.MaxHp,
                    Mana = member.Mana, MaxMana = member.MaxMana, Spell = member.Spell, Skills = member.Skills.Clone(), DamageType = "physical" };
                CombatUnit enemy = new CombatUnit { Id = "training-target", Name = "Training target", Side = UnitSide.Enemy, PartyIndex = -1,
                    Role = "raider", Level = 1, Hp = 500, MaxHp = 500, X = 3, Y = 2, Range = 1, Movement = 3, Skills = new SkillSet().Normalize() };
                State.Combat = new CombatState { Round = 1, EncounterStyle = "rat-sluice", ActiveId = hero.Id, ActionAvailable = true,
                    Phase = CombatPhase.ChooseAction, Units = new List<CombatUnit> { hero, enemy }, InitiativeQueue = new List<string> { hero.Id, enemy.Id } };
                CopyGear(); State.Combat.MovePoints = Hero.Movement;
            }

            public InventoryItem Equip(string form)
            {
                InventoryItem item = new InventoryItem { Form = form, Slot = "weapon", DisplayName = "training " + form, Material = "iron", Trait = "plain",
                    DamageMin = 3, DamageMax = 7, AttackSpeed = 8, DamageType = "physical", Bonus = 1 };
                Call("AddInventoryItem", item);
                object[] args = { item, Member, "" };
                Require((bool)Call("EquipInventoryItemToMember", args), "production equipment assignment: " + args[2]);
                CopyGear(); return item;
            }

            public ArmoryOverlayScreen OpenNativePack()
            {
                Set("launchError", ""); Set("showArmory", false);
                Call("EnsureArmoryOverlayScreen");
                detachedArmory = Get<ArmoryOverlayScreen>("armoryOverlayScreen");
                detachedArmory.transform.SetParent(null, false); detachedArmory.gameObject.SetActive(true);
                Type tabType = typeof(AshenHallsGame).GetNestedType("ArmoryTab", BindingFlags.NonPublic);
                typeof(AshenHallsGame).GetMethod("ToggleArmory", Private, null, new[] { typeof(int) }, null)
                    .Invoke(Game, new object[] { Convert.ToInt32(Enum.Parse(tabType, "Pack")) });
                return detachedArmory;
            }

            private void CopyGear()
            {
                Hero.WeaponName = Member.WeaponName; Hero.Range = Member.Range; Hero.WeaponBonus = Member.WeaponBonus;
                Hero.DamageType = Member.WeaponDamageType; Hero.DamageMin = Member.DamageMin; Hero.DamageMax = Member.DamageMax;
                Hero.AttackSpeed = Member.AttackSpeed; Hero.Agility = Member.Agility; Hero.Movement = Member.Movement; Hero.Power = Member.Power;
            }

            public string CombatWithoutRanges()
            {
                CombatState copy = JsonUtility.FromJson<CombatState>(JsonUtility.ToJson(State.Combat));
                foreach (CombatUnit unit in copy.Units) if (unit.Side == UnitSide.Party && !unit.Summoned) unit.Range = 0;
                return JsonUtility.ToJson(copy);
            }

            public void Reload(int sourceVersion)
            {
                State.SaveVersion = sourceVersion;
                Require(SaveService.TrySaveCampaignState(savePath, State, false, out string reason), "real disposable save: " + reason);
                GameState loaded = SaveService.LoadGameState(savePath, candidate => SaveCandidateRules.IsLoadable(candidate, VersionInfo.SaveVersion), out bool backup);
                Require(!backup, "primary save validates");
                Call("AdoptLoadedGameState", loaded, sourceVersion, ContentSetCatalog.SewerSlice);
            }

            public object Call(string name, params object[] args)
            {
                MethodInfo method = typeof(AshenHallsGame).GetMethod(name, Private);
                Require(method != null, "runtime hook exists: " + name);
                try { return method.Invoke(Game, args); }
                catch (TargetInvocationException exception) { throw new InvalidOperationException("Weapon training runtime call: " + name, exception.InnerException); }
            }
            public T Get<T>(string name) => (T)typeof(AshenHallsGame).GetField(name, Private).GetValue(Game);
            public void Set(string name, object value) => typeof(AshenHallsGame).GetField(name, Private).SetValue(Game, value);
            public void Dispose()
            {
                CombatAbilityModalScreen modal = Get<CombatAbilityModalScreen>("combatAbilityModalScreen");
                if (modal != null)
                {
                    // Its never-active component also owns fallback art and
                    // navigation state beyond the separately tracked canvas.
                    FieldInfo atlasField = typeof(CombatAbilityModalScreen).GetField("generatedBookStateIconAtlas", Private);
                    Texture2D atlas = (Texture2D)atlasField.GetValue(modal);
                    if (atlas != null) UnityEngine.Object.DestroyImmediate(atlas);
                    atlasField.SetValue(modal, null);
                    typeof(CombatAbilityModalScreen).GetMethod("OnDestroy", Private).Invoke(modal, null);
                }
                ReleaseOwnedCanvasRoots(host);
                if (detachedArmory != null)
                {
                    ReleaseOwnedCanvasRoots(detachedArmory.gameObject);
                    UnityEngine.Object.DestroyImmediate(detachedArmory.gameObject);
                }
                UnityEngine.Object.DestroyImmediate(host);
                foreach (string suffix in new[] { "", ".bak", ".tmp" }) if (File.Exists(savePath + suffix)) File.Delete(savePath + suffix);
            }

            private static void ReleaseOwnedCanvasRoots(GameObject root)
            {
                // Unity does not invoke OnDestroy for components which were never
                // active. These fixtures intentionally suppress the game host's Awake.
                foreach (MonoBehaviour component in root.GetComponentsInChildren<MonoBehaviour>(true))
                    if (component != null && component.GetType().Name == "UiOwnedCanvasLifetime")
                        component.GetType().GetMethod("OnDestroy", Private).Invoke(component, null);
            }
        }

        private sealed class CountingRandom : System.Random
        {
            public int Calls;
            public override int Next(int maxValue) { Calls++; return 0; }
            public override int Next(int minValue, int maxValue) { Calls++; return minValue; }
            public override double NextDouble() { Calls++; return .99; }
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("Weapon training: " + message);
        }
    }
}
