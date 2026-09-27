using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class RoadStockItemSmoke
    {
        private const string AtlasHash = "8F90383293CEB79EDCA697FD88780E954C55780998D974166998CCF43DF1C035";
        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " road stock item smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogError(VersionInfo.ProductName + " road stock item smoke failed: " + exception);
                EditorApplication.Exit(1);
            }
        }

        public static void RunNewStock()
        {
            try
            {
                RunOrThrow();
                NativeRoadStockSmoke.RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " combined road stock item and native shop smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogError(VersionInfo.ProductName + " combined road stock smoke failed: " + exception);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            ApprovedArtLoadsAndReachesNativeAdapters();
            LoadoutsHaveDistinctBudgetsAndStableIdentity();
            SpearUsesActualReachAndArmsWithoutExtendingAbilities();
            HoldfastAppliesOnlyToTheGuardAction();
            AxeAccuracyChangesActualHitResolution();
            OrdinaryAxesUseTheirFamilyWithoutChangingStarterPools();
            RangeLabelsDiscloseTheirWearerAndFocusIsAnActionState();
            EnchantmentsAndProductionSaveKeepBothIntrinsics();
        }

        private static void ApprovedArtLoadsAndReachesNativeAdapters()
        {
            byte[] bytes = File.ReadAllBytes(Path.Combine(Application.dataPath, "..", "Docs", "ArtReferences", RuntimeArtManifest.RoadStockItemAtlas));
            using (SHA256 hash = SHA256.Create())
                Require(BitConverter.ToString(hash.ComputeHash(bytes)).Replace("-", "") == AtlasHash,
                    "road stock art retains its approved file hash");
            Require(RuntimeArtManifest.ApprovedRuntimeFiles.Count(file => file == RuntimeArtManifest.RoadStockItemAtlas) == 1,
                "the distributable manifest contains the atlas exactly once");
            using (Fixture f = new Fixture())
            {
                Texture2D atlas = (Texture2D)f.Call("LoadApprovedExternalPngWithAlpha",
                    RuntimeArtManifest.RoadStockItemAtlas, .75f, "Tessa road stock icons", .08f);
                try
                {
                    Require(atlas != null && atlas.width == 1774 && atlas.height == 887,
                        "the actual approved runtime loader accepts the slender weapons at its production alpha thresholds");
                    f.Set("roadStockItemAtlas", atlas);
                    Require((bool)f.Call("IsRoadStockItemAtlas"), "the production atlas validator recognizes the loaded art");
                    Color32[] pixels = atlas.GetPixels32();
                    for (int index = 0; index < 2; index++)
                    {
                        int visible = 0;
                        for (int y = 0; y < 887; y++)
                        for (int x = index * 887; x < (index + 1) * 887; x++)
                            if (pixels[y * 1774 + x].a >= 32) visible++;
                        Require(visible > 887 * 887 * .035f && visible < 887 * 887 * .40f,
                            "each cell contains a separate visible silhouette with transparent breathing room");
                        InventoryItem item = index == 0 ? f.Spear : f.Axe;
                        item.DisplayName = "Restored road weapon " + index;
                        Rect uv = new Rect(index * .5f, 0f, .5f, 1f);
                        object[] icon = { item, null, Rect.zero };
                        Require((bool)f.Call("TryGetInventoryItemIcon", icon)
                            && ReferenceEquals(icon[1], atlas) && (Rect)icon[2] == uv,
                            "renamed stable signatures resolve to their own cells");
                        IReadOnlyList<ArmoryRowView> rows = (IReadOnlyList<ArmoryRowView>)f.Call("BuildArmoryPackRows");
                        ArmoryRowView row = rows.First(entry => entry.Key == f.State.Inventory.IndexOf(item));
                        f.Call("SelectArmoryInventoryItem", item);
                        ArmoryDetailView detail = (ArmoryDetailView)f.Call("BuildInventoryItemDetail");
                        Require(row.IconTexture == atlas && row.IconUv == uv && detail.IconTexture == atlas && detail.IconUv == uv
                            && detail.Summary.Contains(SignatureItemCatalog.Find(item).IntrinsicName),
                            "native Pack row and detail expose the loaded art and intrinsic explanation");
                        f.Set("lootPanelItem", item);
                        f.Set("lootPanelBody", item.DisplayName);
                        f.Set("lootPanelRequiresDismissal", true);
                        LootPopupView loot = (LootPopupView)f.Call("BuildLootPopupView");
                        Require(loot.IconTexture == atlas && loot.IconUv == uv && loot.CanReview,
                            "the actual loot adapter retains art and deliberate equipment review");
                    }
                    for (int y = 0; y < 887; y++)
                        Require(pixels[y * 1774 + 886].a == 0 && pixels[y * 1774 + 887].a == 0,
                            "both midpoint columns are transparent");
                }
                finally { if (atlas != null) UnityEngine.Object.DestroyImmediate(atlas); }
            }
        }

        private static void LoadoutsHaveDistinctBudgetsAndStableIdentity()
        {
            using (Fixture f = new Fixture())
            {
                Require(SignatureItemCatalog.IconIndex(f.Spear) == 11 && SignatureItemCatalog.IconIndex(f.Axe) == 12
                    && SignatureItemCatalog.IconIndex(SignatureItemCatalog.CreateMeasurekeeperJack()) == 10,
                    "road weapons append after the existing eleven signatures");
                Require(f.Spear.Bonus == 3 && f.Spear.DamageMin == 3 && f.Spear.DamageMax == 8
                    && f.Spear.AttackSpeed == 7 && f.Spear.HealthBonus == 1 && f.Spear.StrengthBonus == 0,
                    "Bridgehook retains its modest reach and guard budget");
                Require(f.Axe.Bonus == 3 && f.Axe.DamageMin == 5 && f.Axe.DamageMax == 9
                    && f.Axe.AttackSpeed == 5 && f.Axe.StrengthBonus == 1 && f.Axe.HealthBonus == 0,
                    "Stonecleft trades speed and reach for physical damage and accuracy");
                foreach (InventoryItem item in new[] { f.Spear, f.Axe })
                {
                    f.Equip(item);
                    Require(f.Member.WeaponItemId == item.InstanceId && item.EquippedById == f.Member.Id
                        && f.Member.WeaponDamageMin == item.DamageMin && f.Member.WeaponDamageMax == item.DamageMax
                        && f.Member.WeaponAttackSpeed == item.AttackSpeed,
                        "real equipment assignment delivers the authored loadout with reciprocal ownership");
                    InventoryItem copy = SignatureItemCatalog.Create(item.SignatureId);
                    f.Call("AddInventoryItem", copy);
                    Require(copy.InstanceId != item.InstanceId, "physical copies get distinct inventory identities");
                    string expectedId = copy.SignatureId;
                    copy.SignatureId = "";
                    copy.DisplayName = "Tessa's restored road tool";
                    Require(SignatureItemCatalog.RepairIdentity(copy) && copy.SignatureId == expectedId,
                        "structural identity repairs a missing signature without relying on its label");
                    Require(InventoryEquipmentRules.RequiresDeliberateReview(item, SignatureItemCatalog.CreateUnfathomableSword()),
                        "neither weapon silently replaces an existing named item");
                }
                Require(WeaponFeedbackRules.For(f.Spear.DisplayName, false).Kind == WeaponFeedbackKind.Thrust
                    && WeaponFeedbackRules.For(f.Axe.DisplayName, false).Kind == WeaponFeedbackKind.Heavy,
                    "the established audiovisual families already distinguish spear and axe");
            }
        }

        private static void SpearUsesActualReachAndArmsWithoutExtendingAbilities()
        {
            foreach (string classKey in new[] { "warrior", "paladin", "rogue" })
            using (Fixture f = new Fixture(classKey))
            {
                f.Equip(f.Spear);
                f.Enemy.X = 4;
                CombatAttackForecast forecast = (CombatAttackForecast)f.Call("AttackForecast", f.Hero, f.Enemy);
                Require(forecast.Legal && forecast.Range == 2 && !forecast.Ranged
                    && (string)f.Call("AttackSkillName", f.Hero, f.Enemy) == "arms",
                    classKey + " uses a real two-tile Arms basic attack");
                Require((string)f.Call("WeaponPrimaryStatKey", f.Member) == (classKey == "rogue" ? "finesse" : "reach"),
                    "existing class precedence and reach scaling are preserved");
                f.State.Combat.Units.Add(new CombatUnit
                {
                    Id = "adjacent-pressure", Side = UnitSide.Enemy, Hp = 50, MaxHp = 50, X = 2, Y = 3
                });
                Require(((CombatAttackForecast)f.Call("AttackForecast", f.Hero, f.Enemy)).Legal,
                    "adjacent pressure does not collapse a spear's reach");
                f.Enemy.Guarding = true;
                f.Enemy.GuardBonus = 4;
                int before = f.Hero.Hp;
                f.Attack(0);
                Require(f.Enemy.Hp < 100 && f.Hero.Hp == before,
                    "a landed reach hit damages a guard without triggering its adjacent counter");
                f.Enemy.X = 3;
                f.Attack(0);
                Require(f.Hero.Hp == before - 2, "the same spear at one tile still triggers the existing guard counter");
                f.Enemy.X = 5;
                Require(!((CombatAttackForecast)f.Call("AttackForecast", f.Hero, f.Enemy)).Legal,
                    "Bridgehook does not create a three-tile basic attack for martial wearers");
                Require(AbilityCatalog.For("ambush").Range == 1 && AbilityCatalog.For("shieldbash").Range == 1
                    && AbilityCatalog.For("cleave").Range == 1,
                    "reach equipment does not rewrite the authored class ability ranges");
            }
        }

        private static void HoldfastAppliesOnlyToTheGuardAction()
        {
            using (Fixture f = new Fixture("paladin"))
            {
                f.Equip(f.Spear);
                Require(!f.Hero.Guarding && f.Hero.GuardBonus == 0, "equipping Holdfast does not apply passive Guard");
                int fresh = (int)f.Call("GuardActionBonus", f.Hero);
                f.Equip(f.Axe);
                Require(fresh == (int)f.Call("GuardActionBonus", f.Hero) + 1, "Holdfast adds exactly one to the normal braced action");
                f.Equip(f.Spear);
                f.State.Combat.Moved = true;
                f.State.Combat.MovePoints = 1;
                int moved = (int)f.Call("GuardActionBonus", f.Hero);
                Require(moved == fresh - 2, "moving retains the normal reduced Guard stance plus the same one-point intrinsic");
                CombatCommandResult result = ((CombatController)f.Call("CombatLifecycle")).Guard(f.Hero, moved);
                Require(result.Success && f.Hero.Guarding && f.Hero.GuardBonus == moved && !f.State.Combat.ActionAvailable,
                    "the actual Guard command spends its action and applies the advertised value");
            }
        }

        private static void AxeAccuracyChangesActualHitResolution()
        {
            using (Fixture f = new Fixture())
            {
                f.Equip(f.Axe);
                f.Enemy.Agility = 12;
                InventoryItem ordinary = PlainCopy(f.Axe);
                f.Call("AddInventoryItem", ordinary);
                f.Equip(ordinary);
                int ordinaryHit = (int)f.Call("AttackHitChance", f.Hero, f.Enemy);
                Require(ordinaryHit > 18 && ordinaryHit < 93, "accuracy fixture avoids either hit-chance cap");
                SkillSet sameTraining = f.Hero.Skills.Clone();
                f.Attack(ordinaryHit);
                Require(f.Enemy.Hp == 100, "a roll at the ordinary weapon threshold misses");
                Require(f.Hero.Skills.Arms == sameTraining.Arms + 1, "the actual miss still awards normal Arms practice");
                f.Hero.Skills = sameTraining.Clone();
                f.Member.Skills = sameTraining.Clone();
                f.Equip(f.Axe);
                Require((int)f.Call("AttackHitChance", f.Hero, f.Enemy) == ordinaryHit + 2,
                    "Sure Hew reaches the production hit forecast");
                f.Attack(ordinaryHit);
                Require(f.Enemy.Hp < 100, "the same roll lands with Sure Hew in the actual attack path");
                f.Hero.Power = 5;
                Require(CombatRitualRules.PhysicalDisruptionDamage(f.Hero, false) == 2
                    && CombatRitualRules.PhysicalDisruptionDamage(f.Hero, true) == 1,
                    "the existing axe family gains one physical ritual disruption only in melee");
            }
        }

        private static void OrdinaryAxesUseTheirFamilyWithoutChangingStarterPools()
        {
            using (Fixture f = new Fixture())
            {
                Require(InventoryEquipmentRules.IsWeaponSlot("", "battle axe")
                    && !InventoryEquipmentRules.IsWeaponSlot("quest", "battle axe"),
                    "legacy axe forms are recognized while explicit quest-object slots remain protected");
                Require((Vector2Int)f.Call("ItemDamageRange", "battle axe", 0, "keen", "iron") == new Vector2Int(3, 7)
                    && (Vector2Int)f.Call("ItemDamageRange", "battle axe", 3, "keen", "iron") == new Vector2Int(4, 10),
                    "ordinary axes use their explicit damage family and existing quality modifier");
                Require((int)f.Call("ItemAttackSpeed", "battle axe", "plain", "iron", "keen") == 5
                    && (int)f.Call("ItemAttackSpeed", "battle axe", "balanced", "iron", "keen") == 7,
                    "ordinary axes are slow; balanced quality remains meaningful");
                foreach (string role in new[] { "shield", "ward", "" })
                {
                    Require(((string[])f.Call("RoleWeaponForms", role, false)).Contains("battle axe"),
                        "ordinary battle axes are reachable through the martial/general loot pool");
                    Require(!((string[])f.Call("RoleWeaponForms", role, true)).Contains("battle axe"),
                        "the introductory starter and 32-gold shop pools retain their previous distribution");
                }
                f.Set("rng", new LastIndexRandom());
                InventoryItem generated = (InventoryItem)f.Call("MakeRoleItem", "shield", true, false);
                Require(generated.Form == "battle axe" && InventoryEquipmentRules.IsEquippable(generated),
                    "the actual ordinary loot generator can produce and classify an axe");
                Require((int)f.Call("WeaponRoleFit", f.Axe, f.Member) == 5,
                    "martial recommendations recognize axes alongside existing heavy weapons");
            }
        }

        private static void RangeLabelsDiscloseTheirWearerAndFocusIsAnActionState()
        {
            foreach (string classKey in new[] { "warrior", "rogue", "mage", "priest", "ranger" })
            using (Fixture f = new Fixture(classKey))
            {
                InventoryItem knife = SignatureItemCatalog.CreateCounterweightKnife();
                f.Call("AddInventoryItem", knife);
                f.Equip(knife);
                int expected = classKey == "mage" ? 3 : classKey == "ranger" ? 5 : 1;
                string expectedLabel = (expected == 1 ? "Melee" : "Range " + expected) + " for " + f.Member.Name;
                Require((string)f.Call("InventoryWeaponRangeLabel", knife, f.Member) == expectedLabel
                    && ((string)f.Call("CompactInventoryItemSummary", knife)).Contains(expectedLabel)
                    && ((string)f.Call("ItemTraitLine", knife)).Contains(expectedLabel),
                    "Pack and loot label the actual wearer's range instead of presenting a universal knife range");
                if (classKey == "ranger")
                {
                    Require((int)f.Call("BaseAttackRange", f.Hero) == 5
                        && (int)f.Call("EffectiveAttackRangeFrom", f.Hero, f.Hero.X, f.Hero.Y) == 1,
                        "the ranger's existing role range still becomes melee under adjacent pressure");
                }
                if (classKey == "mage" || classKey == "priest")
                {
                    Require((bool)f.Call("IsFocusedCaster", f.Hero), "a caster can focus while holding a knife");
                    f.State.Combat.Moved = true;
                    Require(!(bool)f.Call("IsFocusedCaster", f.Hero), "moving removes focused casting independently of equipment");
                    InventoryItem focus = SignatureItemCatalog.CreateStormglassConductor();
                    f.Call("AddInventoryItem", focus);
                    f.Equip(focus);
                    Require(!(bool)f.Call("IsFocusedCaster", f.Hero)
                        && !((string)f.Call("ItemBehaviorLine", focus, f.Member)).Contains("spell focus"),
                        "a focus form does not grant focused casting or advertise a separate focus effect");
                }
            }
        }

        private static void EnchantmentsAndProductionSaveKeepBothIntrinsics()
        {
            using (Fixture f = new Fixture())
            {
                foreach (InventoryItem item in new[] { f.Spear, f.Axe })
                {
                    WeaponEnchantmentRules.ApplyPermanent(item, "fire");
                    WeaponEnchantmentRules.ApplyTemporary(item, "storm");
                    for (int i = 0; i < WeaponEnchantmentRules.TemporaryVictories; i++)
                        WeaponEnchantmentRules.AdvanceAfterVictory(item);
                    item.DisplayName = "Tessa's restored tool " + item.SignatureId;
                    f.Equip(item);
                    Require(item.DamageType == "fire" && item.PermanentEnchantmentId == "fire",
                        "temporary expiry restores the paid permanent affinity");
                    Require(item == f.Spear ? SignatureItemRules.GuardActionBonus(item) == 1 : SignatureItemRules.WeaponHitBonus(item) == 2,
                        "renaming and enchanting preserve the stable intrinsic");
                }
                f.Reload();
                foreach (InventoryItem item in new[] { f.Spear, f.Axe })
                {
                    f.Equip(item);
                    Require(item.InstanceId == f.Member.WeaponItemId && item.EquippedById == f.Member.Id,
                        "the validated production save preserves equipment ownership");
                    Require(item == f.Spear
                        ? (int)f.Call("GuardActionBonus", f.Hero) == 5
                        : (int)f.Call("WeaponHitBonusForItem", f.Hero.WeaponName, item) == 2,
                        "loaded renamed items resolve their intrinsic through the real combat adapter");
                }
            }
        }

        private static InventoryItem PlainCopy(InventoryItem item)
        {
            InventoryItem plain = JsonUtility.FromJson<InventoryItem>(JsonUtility.ToJson(item));
            plain.SignatureId = "";
            plain.InstanceId = "";
            plain.EquippedById = "";
            plain.Mark = "ordinary";
            plain.DisplayName = "ordinary " + item.Form;
            return plain;
        }

        private sealed class Fixture : IDisposable
        {
            private readonly GameObject host = RemembranceItemSmoke.NewHost("Road stock item fixture");
            private readonly string savePath = Path.Combine(Path.GetTempPath(), "ashen-road-items-" + Guid.NewGuid().ToString("N") + ".json");
            public readonly AshenHallsGame Game;
            public GameState State => Get<GameState>("state");
            public PartyMember Member => State.Party[0];
            public CombatUnit Hero => State.Combat.Units[0];
            public CombatUnit Enemy => State.Combat.Units[1];
            public InventoryItem Spear => State.Inventory.First(item => item.SignatureId == SignatureItemCatalog.BridgehookSpearId);
            public InventoryItem Axe => State.Inventory.First(item => item.SignatureId == SignatureItemCatalog.StonecleftAxeId);

            public Fixture(string classKey = "warrior")
            {
                Game = host.AddComponent<AshenHallsGame>();
                string role = classKey == "rogue" ? "knife" : classKey == "paladin" ? "ward"
                    : classKey == "mage" ? "ember" : classKey == "priest" ? "mender" : classKey == "ranger" ? "bow" : "shield";
                PartyMember member = new PartyMember
                {
                    Id = "road-hero", Name = classKey == "mage" ? "Luma" : classKey == "rogue" ? "Rook" : "Maer",
                    Race = "human", ClassKey = classKey, Role = role, Level = 8,
                    Spell = StarterPartyCatalog.SpellSchoolForClass(classKey), Stats = new Stats(18, 10, 12, 14),
                    Skills = new SkillSet { Arms = 8, Missile = 8, Guard = 8 }.Normalize(),
                    WeaponName = "practice sword", WeaponDamageMin = 2, WeaponDamageMax = 5, WeaponAttackSpeed = 7,
                    ArmorName = "practice jack", Hp = 100, MaxHp = 100
                };
                CombatUnit hero = new CombatUnit
                {
                    Id = member.Id, Name = member.Name, Race = member.Race, ClassKey = classKey, Role = role,
                    Spell = member.Spell, PartyIndex = 0, Side = UnitSide.Party, Level = member.Level,
                    Hp = 100, MaxHp = 100, X = 2, Y = 2, Skills = member.Skills, DamageType = "physical"
                };
                CombatUnit enemy = new CombatUnit
                {
                    Id = "road-target", Name = "Road raider", Role = "raider", PartyIndex = -1,
                    Side = UnitSide.Enemy, Hp = 100, MaxHp = 100, X = 3, Y = 2, Range = 1,
                    Skills = new SkillSet().Normalize()
                };
                Set("state", new GameState
                {
                    SaveVersion = VersionInfo.SaveVersion, ContentSetId = ContentSetCatalog.SewerSlice,
                    Mode = GameMode.Combat, Depth = 3, Seed = 2350, ReducedMotion = true, SfxMuted = true, MusicMuted = true,
                    Party = new List<PartyMember> { member },
                    Combat = new CombatState { ActiveId = hero.Id, Units = new List<CombatUnit> { hero, enemy }, ActionAvailable = true, Phase = CombatPhase.ChooseAction }
                });
                Set("rng", new FixedRandom(0));
                Set("visualSmokeSaveBlocked", true);
                Call("RecalculateMember", member);
                Call("AddInventoryItem", SignatureItemCatalog.CreateBridgehookSpear());
                Call("AddInventoryItem", SignatureItemCatalog.CreateStonecleftAxe());
                Equip(Spear);
                State.Combat.MovePoints = Hero.Movement;
            }

            public void Equip(InventoryItem item)
            {
                bool alreadyEquipped = !string.IsNullOrWhiteSpace(item.InstanceId)
                    && Member.WeaponItemId == item.InstanceId && item.EquippedById == Member.Id;
                object[] args = { item, Member, "" };
                bool assigned = (bool)Call("EquipInventoryItemToMember", args);
                Require(assigned || (alreadyEquipped && Member.WeaponItemId == item.InstanceId && item.EquippedById == Member.Id),
                    "actual equipment assignment or unchanged existing ownership: " + args[2]);
                Hero.WeaponName = Member.WeaponName;
                Hero.DamageType = Member.WeaponDamageType;
                Hero.DamageMin = Member.DamageMin;
                Hero.DamageMax = Member.DamageMax;
                Hero.Power = Member.Power;
                Hero.Range = Member.Range;
                Hero.WeaponBonus = Member.WeaponBonus;
                Hero.AttackSpeed = Member.AttackSpeed;
                Hero.Agility = Member.Agility;
                Hero.Movement = Member.Movement;
            }

            public void Attack(int roll)
            {
                Enemy.Hp = 100;
                Set("rng", new FixedRandom(roll));
                Require((bool)Call("Attack", Hero, Enemy), "the real basic attack accepts its target");
            }

            public void Reload()
            {
                Require(SaveService.TrySaveCampaignState(savePath, State, false, out string reason), "disposable save: " + reason);
                GameState loaded = SaveService.LoadGameState(savePath,
                    candidate => SaveCandidateRules.IsLoadable(candidate, VersionInfo.SaveVersion), out bool backup);
                Require(!backup, "the primary save validates without fallback");
                Set("state", loaded);
            }

            public object Call(string name, params object[] arguments) => RemembranceItemSmoke.Invoke(Game, name, arguments);
            public void Set(string name, object value) => RemembranceItemSmoke.Set(Game, name, value);
            public T Get<T>(string name) => (T)typeof(AshenHallsGame).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Game);
            public void Dispose()
            {
                UnityEngine.Object.DestroyImmediate(host);
                foreach (string suffix in new[] { "", ".bak", ".tmp" })
                    if (File.Exists(savePath + suffix)) File.Delete(savePath + suffix);
            }
        }

        private sealed class FixedRandom : System.Random
        {
            private readonly int roll;
            public FixedRandom(int roll) { this.roll = roll; }
            public override int Next(int maxValue) => Math.Min(roll, maxValue - 1);
            public override int Next(int minValue, int maxValue) => minValue;
            public override double NextDouble() => .99;
        }

        private sealed class LastIndexRandom : System.Random
        {
            public override int Next(int maxValue) => maxValue - 1;
            public override int Next(int minValue, int maxValue) => maxValue - 1;
            public override double NextDouble() => .99;
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("Road stock items: " + message);
        }
    }
}
