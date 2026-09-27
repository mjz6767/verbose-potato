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
    public static class HonestWeightItemSmoke
    {
        private const string AtlasHash = "D0907CFB27CD776CBA60691AA58376FD13960AB7FD2DDCAD6662ADCE8BF33C52";

        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " honest weight item smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogError(VersionInfo.ProductName + " honest weight item smoke failed: " + exception);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            RewardArtReachesProductionAdapters();
            EquipmentHasBoundedRolesAndStableIdentity();
            KnifeBleedUsesActualHitResistanceAndTurnRules();
            JackMitigationAndEnchantedIdentitySurviveSaving();
            CrossbowSpeedAndBorinQuoteMatchTheirEquipment();
        }

        private static void RewardArtReachesProductionAdapters()
        {
            byte[] bytes = File.ReadAllBytes(Path.Combine(Application.dataPath, "..", "Docs", "ArtReferences", RuntimeArtManifest.HonestWeightItemAtlas));
            using (SHA256 hash = SHA256.Create())
                Require(BitConverter.ToString(hash.ComputeHash(bytes)).Replace("-", "") == AtlasHash,
                    "the approved Honest Weight art retains its exact file pin");
            Require(RuntimeArtManifest.ApprovedRuntimeFiles.Count(name => name == RuntimeArtManifest.HonestWeightItemAtlas) == 1,
                "the distributable manifest contains the new atlas exactly once");
            Texture2D atlas = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            GameObject host = RemembranceItemSmoke.NewHost("Honest Weight art adapter fixture");
            try
            {
                Require(atlas.LoadImage(bytes) && atlas.width == 1774 && atlas.height == 887,
                    "the atlas decodes into two distinct square cells");
                Color32[] pixels = atlas.GetPixels32();
                for (int cell = 0; cell < 2; cell++)
                {
                    int visible = 0;
                    for (int y = 0; y < atlas.height; y++)
                    for (int x = cell * 887; x < (cell + 1) * 887; x++)
                        if (pixels[y * atlas.width + x].a >= 32) visible++;
                    Require(visible > 887 * 887 * .04f && visible < 887 * 887 * .70f,
                        "each signature has substantive art and transparent breathing room");
                }
                for (int y = 0; y < atlas.height; y++)
                    Require(pixels[y * atlas.width + 886].a < 32 && pixels[y * atlas.width + 887].a < 32,
                        "neither silhouette crosses the atlas seam");
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                GameState state = new GameState { Mode = GameMode.Explore, Depth = 3, Seed = 2340 };
                RemembranceItemSmoke.Set(game, "state", state);
                RemembranceItemSmoke.Set(game, "honestWeightItemAtlas", atlas);
                InventoryItem[] items = { SignatureItemCatalog.CreateCounterweightKnife(), SignatureItemCatalog.CreateMeasurekeeperJack() };
                for (int index = 0; index < items.Length; index++)
                {
                    InventoryItem item = items[index];
                    RemembranceItemSmoke.Invoke(game, "AddInventoryItem", item);
                    item.DisplayName = "Maud's restored keepsake " + index;
                    Rect expected = new Rect(index * .5f, 0f, .5f, 1f);
                    object[] icon = { item, null, Rect.zero };
                    Require((bool)RemembranceItemSmoke.Invoke(game, "TryGetInventoryItemIcon", icon)
                        && ReferenceEquals(icon[1], atlas) && (Rect)icon[2] == expected,
                        "renamed signatures resolve to their own new-atlas cell");
                    IReadOnlyList<ArmoryRowView> rows = (IReadOnlyList<ArmoryRowView>)RemembranceItemSmoke.Invoke(game, "BuildArmoryPackRows");
                    ArmoryRowView row = rows.First(entry => entry.Key == state.Inventory.IndexOf(item));
                    Require(row.IconTexture == atlas && row.IconUv == expected, "the real Pack row receives the approved art");
                    RemembranceItemSmoke.Invoke(game, "SelectArmoryInventoryItem", item);
                    ArmoryDetailView detail = (ArmoryDetailView)RemembranceItemSmoke.Invoke(game, "BuildInventoryItemDetail");
                    Require(detail.IconTexture == atlas && detail.IconUv == expected
                        && detail.Summary.Contains(SignatureItemCatalog.Find(item).IntrinsicName),
                        "the real Pack detail receives the art and intrinsic explanation");
                    RemembranceItemSmoke.Set(game, "lootPanelItem", item);
                    RemembranceItemSmoke.Set(game, "lootPanelBody", item.DisplayName);
                    RemembranceItemSmoke.Set(game, "lootPanelRequiresDismissal", true);
                    LootPopupView loot = (LootPopupView)RemembranceItemSmoke.Invoke(game, "BuildLootPopupView");
                    Require(loot.IconTexture == atlas && loot.IconUv == expected && loot.CanReview,
                        "the real loot adapter exposes the art and deliberate review route");
                }
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(host);
                UnityEngine.Object.DestroyImmediate(atlas);
            }
        }

        private static void EquipmentHasBoundedRolesAndStableIdentity()
        {
            using (Fixture f = new Fixture())
            {
                Require(SignatureItemCatalog.IconIndex(f.Knife) == 9 && SignatureItemCatalog.IconIndex(f.Jack) == 10,
                    "the new signatures append after both earlier atlases");
                Require(SignatureItemCatalog.IconIndex(SignatureItemCatalog.CreateVigilkeeperWrap()) == 8,
                    "the earlier keepsake identity remains unchanged");
                Require(f.Knife.Bonus == 3 && f.Knife.DamageMin == 3 && f.Knife.DamageMax == 7
                    && f.Knife.AttackSpeed == 11 && f.Knife.AgilityBonus == 1,
                    "the knife retains its approved finesse budget");
                Require(f.Jack.Bonus == 3 && f.Jack.AgilityBonus == 1 && f.Jack.HealthBonus == 0,
                    "the jack trades the Ratcatcher coat's health and poison protection for physical protection");
                Require(f.Member.WeaponItemId == f.Knife.InstanceId && f.Member.ArmorItemId == f.Jack.InstanceId
                    && f.Knife.EquippedById == f.Member.Id && f.Jack.EquippedById == f.Member.Id,
                    "actual equipment selection maintains reciprocal ownership");
                Require(f.Member.WeaponDamageMin == 3 && f.Member.WeaponDamageMax == 7
                    && f.Member.WeaponAttackSpeed == 11 && f.Member.ArmorBonus == 3 && f.Member.GearAgility == 2,
                    "the actual loadout uses declared weapon, armor and stat values");
                Require((string)f.Call("WeaponPrimaryStatKey", f.Member) == "finesse"
                    && (int)f.Call("BaseAttackRange", f.Hero) == 1
                    && (string)f.Call("AttackSkillName", f.Hero, f.Enemy) == "arms",
                    "the knife is a one-tile finesse/Arms weapon for the rogue");

                InventoryItem mail = SignatureItemCatalog.CreateGloamReliquaryMail();
                f.Call("AddInventoryItem", mail);
                int mobileMovement = f.Member.Movement;
                f.Equip(mail);
                Require(f.Member.ArmorBonus == 5 && f.Member.Movement < mobileMovement,
                    "later mail gives more defense while the jack preserves movement");
                f.Equip(f.Jack);
                InventoryItem duplicate = SignatureItemCatalog.CreateCounterweightKnife();
                f.Call("AddInventoryItem", duplicate);
                Require(duplicate.SignatureId == f.Knife.SignatureId && duplicate.InstanceId != f.Knife.InstanceId,
                    "physical copies have distinct ownership identities");
                Require(InventoryEquipmentRules.RequiresDeliberateReview(f.Knife, SignatureItemCatalog.CreateUnfathomableSword())
                    && InventoryEquipmentRules.RequiresDeliberateReview(f.Jack, mail),
                    "the new signatures require deliberate comparison before replacing other named gear");
                foreach (InventoryItem item in new[] { duplicate, SignatureItemCatalog.CreateMeasurekeeperJack() })
                {
                    string id = item.SignatureId;
                    item.SignatureId = "";
                    item.DisplayName = "A restored tool";
                    Require(SignatureItemCatalog.RepairIdentity(item) && item.SignatureId == id,
                        "the authored structural identity can repair a missing signature ID");
                }
            }
        }

        private static void KnifeBleedUsesActualHitResistanceAndTurnRules()
        {
            using (Fixture f = new Fixture())
            {
                Require((string)f.Call("GearOnHitStatus", f.Knife.DisplayName, f.Knife) == "bleed"
                    && Near((float)f.Call("GearOnHitChance", f.Knife.DisplayName, f.Knife), .20f),
                    "the basic attack path resolves Measured Cut by signature");
                Require(Near((float)f.Call("StatusApplyChance", f.Enemy, "bleed", f.Hero, .20f, true), .20f),
                    "Intelligence ten against zero resistance gives exactly the advertised base chance");
                f.Attack(.01);
                Require(f.Enemy.Bleeding == 2, "a successful basic hit can apply two turns of bleeding");
                int bleedingPayoff = (int)f.Call("EviscerateRawDamage", f.Hero, f.Enemy, false);
                f.Enemy.Bleeding = 0;
                Require(bleedingPayoff == (int)f.Call("EviscerateRawDamage", f.Hero, f.Enemy, false) + 4,
                    "the knife's bleed feeds the existing rogue finisher without a new damage rule");
                f.Attack(.21);
                Require(f.Enemy.Bleeding == 0, "a landed hit above the proc threshold does not guarantee bleed");
                f.Attack(.01, 99);
                Require(f.Enemy.Bleeding == 0, "a missed basic attack cannot apply Measured Cut");
                f.Enemy.MagicResist = 2;
                Require(Near((float)f.Call("StatusApplyChance", f.Enemy, "bleed", f.Hero, .20f, true), .06f),
                    "two resistance points reduce the actual chance to six percent");
                f.Attack(.10);
                Require(f.Enemy.Bleeding == 0, "enemy resistance rejects a roll that would pass without resistance");
                f.Member.Stats.Intelligence = 30;
                Require(Near((float)f.Call("StatusApplyChance", f.Enemy, "bleed", f.Hero, .20f, true), .18f),
                    "the existing bounded Intelligence adjustment also applies to this gear proc");
                f.Member.Stats.Intelligence = 10;
                f.Enemy.MagicResist = 0;
                f.Enemy.Bleeding = 3;
                f.Attack(.01);
                Require(f.Enemy.Bleeding == 3, "a knife hit cannot stack or shorten a longer bleed");
                f.Enemy.Bleeding = 0;
                f.Attack(.01);
                int before = f.Enemy.Hp;
                f.Call("ApplyStartTurnEffects", f.Enemy);
                f.Call("ApplyStartTurnEffects", f.Enemy);
                Require(f.Enemy.Bleeding == 0 && f.Enemy.Hp == before - 4,
                    "two turns pay two physical damage each through real status ticking and then expire");
                f.Call("ApplyStartTurnEffects", f.Enemy);
                Require(f.Enemy.Hp == before - 4, "expired bleeding causes no additional damage");
                Require(SignatureItemCatalog.Find(f.Knife).IntrinsicSummary.Contains("Intelligence")
                    && SignatureItemCatalog.Find(f.Knife).IntrinsicSummary.Contains("resistance"),
                    "the catalog discloses the proc's actual uncertainty");
            }
        }

        private static void JackMitigationAndEnchantedIdentitySurviveSaving()
        {
            using (Fixture f = new Fixture())
            {
                foreach (string phase in new[] { "plain", "permanent", "temporary", "expired", "renamed" })
                {
                    if (phase == "permanent") WeaponEnchantmentRules.ApplyPermanent(f.Knife, "fire");
                    if (phase == "temporary") WeaponEnchantmentRules.ApplyTemporary(f.Knife, "storm");
                    if (phase == "expired")
                        for (int victory = 0; victory < WeaponEnchantmentRules.TemporaryVictories; victory++)
                            WeaponEnchantmentRules.AdvanceAfterVictory(f.Knife);
                    if (phase == "renamed") f.Knife.DisplayName = "Maud's restored tool";
                    f.SyncWeapon();
                    f.Enemy.Bleeding = 0;
                    f.Attack(.01);
                    Require(f.Enemy.Bleeding == 2 && SignatureItemCatalog.IconIndex(f.Knife) == 9,
                        phase + " knife keeps both signature art identity and the actual bleed proc");
                    Require((string)f.Call("GearOnHitStatus", f.Hero.WeaponName, f.Knife) == "bleed",
                        phase + " elemental enchantment does not replace the signature's one on-hit status");
                }
                Require(f.Knife.PermanentEnchantmentId == "fire" && f.Knife.DamageType == "fire",
                    "expired temporary magic restores the paid permanent affinity");
                f.Jack.DisplayName = f.Member.ArmorName = f.Hero.ArmorName = "Maud's restored coat";
                foreach (string type in new[] { "physical", "poison", "cold", "fire", "shock", "death", "mind", "light" })
                {
                    f.Hero.Hp = 100;
                    int dealt = (int)f.Call("DealDamage", f.Hero, 10, type, Color.white);
                    Require(dealt == (type == "physical" ? 9 : 10) && f.Hero.Hp == 100 - dealt,
                        "the actual renamed jack mitigates only physical damage: " + type);
                }
                f.Hero.Hp = 100;
                Require((int)f.Call("DealDamage", f.Hero, 1, "physical", Color.white) == 1,
                    "the jack preserves the one-damage floor");
                f.Hero.Bleeding = 2;
                int before = f.Hero.Hp;
                f.Call("ApplyStartTurnEffects", f.Hero);
                Require(f.Hero.Hp == before - 1 && f.Hero.Bleeding == 1,
                    "physical protection also works on existing bleed ticks without making them harmless");
                f.Reload();
                Require(SignatureItemCatalog.IconIndex(f.Knife) == 9 && SignatureItemCatalog.IconIndex(f.Jack) == 10
                    && f.Member.WeaponItemId == f.Knife.InstanceId && f.Member.ArmorItemId == f.Jack.InstanceId,
                    "the validated production save retains stable signatures, art indices and equipment links");
                Require((int)f.Call("GearDamageReduction", f.Hero, "physical") == 1,
                    "loaded combat resolves the renamed armor's physical intrinsic through its owner");
                f.Enemy.Bleeding = 0;
                f.Attack(.01);
                Require(f.Enemy.Bleeding == 2, "the loaded renamed enchanted knife still applies its intrinsic");
            }
        }

        private static void CrossbowSpeedAndBorinQuoteMatchTheirEquipment()
        {
            using (Fixture f = new Fixture())
            {
                foreach (string form in new[] { "crossbow", "longbow", "sling", "knife", "war hammer" })
                {
                    int speed = form == "crossbow" || form == "war hammer" ? 5 : form == "knife" ? 11 : 9;
                    Require((int)f.Call("ItemAttackSpeed", form, "plain", "iron", "keen") == speed,
                        "generated base speed honors its specific weapon family: " + form);
                    Require((int)f.Call("ItemAttackSpeed", form, "balanced", "iron", "keen") == speed + 2,
                        "balanced quality still modifies that family's speed: " + form);
                }
                InventoryItem armor = (InventoryItem)f.Call("MakeTownArmor");
                Require(armor.Bonus == 2 && armor.DisplayName.StartsWith("+2 ", StringComparison.Ordinal)
                    && (int)f.Call("ArmorDefenseBonus", armor) == 3 && armor.HealthBonus == 1,
                    "Borin's name distinguishes quality +2 from the actual three defense including chain");
                f.State.Gold = 100;
                f.Call("ShowBorinPurchaseReview");
                string body = f.Get<string>("dialogueBody");
                Require(body.Contains("3 armor (+2 quality, +1 chain)") && body.Contains("+1 health")
                    && body.Contains("28 gold"),
                    "the actual purchase review quotes the delivered armor, health and unchanged price");
            }
        }

        private sealed class Fixture : IDisposable
        {
            private readonly GameObject host = RemembranceItemSmoke.NewHost("Honest Weight item fixture");
            private readonly string savePath = Path.Combine(Path.GetTempPath(), "ashen-honest-items-" + Guid.NewGuid().ToString("N") + ".json");
            public readonly AshenHallsGame Game;
            public GameState State => Get<GameState>("state");
            public PartyMember Member => State.Party[0];
            public CombatUnit Hero => State.Combat.Units[0];
            public CombatUnit Enemy => State.Combat.Units[1];
            public InventoryItem Knife => State.Inventory.First(item => item.SignatureId == SignatureItemCatalog.CounterweightKnifeId);
            public InventoryItem Jack => State.Inventory.First(item => item.SignatureId == SignatureItemCatalog.MeasurekeeperJackId);

            public Fixture()
            {
                Game = host.AddComponent<AshenHallsGame>();
                PartyMember member = new PartyMember
                {
                    Id = "honest-rogue", Name = "Rook", Race = "human", ClassKey = "rogue", Role = "knife", Level = 8,
                    Stats = new Stats(10, 10, 15, 10), Skills = new SkillSet { Arms = 8 }.Normalize(),
                    WeaponName = "practice knife", WeaponDamageMin = 2, WeaponDamageMax = 5, WeaponAttackSpeed = 11,
                    ArmorName = "practice jack", Hp = 100, MaxHp = 100
                };
                CombatUnit hero = new CombatUnit
                {
                    Id = member.Id, Name = member.Name, Race = member.Race, ClassKey = member.ClassKey, Role = member.Role,
                    PartyIndex = 0, Side = UnitSide.Party, Level = member.Level, Hp = 100, MaxHp = 100,
                    Range = 1, X = 2, Y = 2, Skills = member.Skills, DamageType = "physical"
                };
                CombatUnit enemy = new CombatUnit
                {
                    Id = "honest-target", Name = "Road raider", Role = "raider", PartyIndex = -1,
                    Side = UnitSide.Enemy, Hp = 100, MaxHp = 100, X = 3, Y = 2, Range = 1,
                    Skills = new SkillSet().Normalize()
                };
                Set("state", new GameState
                {
                    SaveVersion = VersionInfo.SaveVersion, ContentSetId = ContentSetCatalog.SewerSlice,
                    Mode = GameMode.Combat, Depth = 3, Seed = 2340, ReducedMotion = true, SfxMuted = true, MusicMuted = true,
                    Party = new List<PartyMember> { member },
                    Combat = new CombatState { ActiveId = hero.Id, Units = new List<CombatUnit> { hero, enemy } }
                });
                Set("rng", new FixedRandom(.01));
                Call("RecalculateMember", member);
                InventoryItem knife = SignatureItemCatalog.CreateCounterweightKnife();
                InventoryItem jack = SignatureItemCatalog.CreateMeasurekeeperJack();
                Call("AddInventoryItem", knife);
                Call("AddInventoryItem", jack);
                Equip(knife);
                Equip(jack);
                SyncWeapon();
                Hero.ArmorName = Member.ArmorName;
                Hero.Agility = Member.Agility;
                Hero.AttackSpeed = Member.AttackSpeed;
            }

            public void Equip(InventoryItem item)
            {
                object[] args = { item, Member, "" };
                Require((bool)Call("EquipInventoryItemToMember", args), "actual equipment assignment: " + args[2]);
            }

            public void SyncWeapon()
            {
                Member.WeaponName = Hero.WeaponName = Knife.DisplayName;
                Member.WeaponDamageType = Hero.DamageType = Knife.DamageType;
                Hero.DamageMin = Member.DamageMin;
                Hero.DamageMax = Member.DamageMax;
                Hero.Power = Member.Power;
            }

            public void Attack(double statusRoll, int attackRoll = 0)
            {
                Enemy.Hp = 100;
                Set("rng", new FixedRandom(statusRoll, attackRoll));
                Require((bool)Call("Attack", Hero, Enemy), "the actual basic attack accepts the target");
            }

            public void Reload()
            {
                Require(SaveService.TrySaveCampaignState(savePath, State, false, out string reason),
                    "disposable item save succeeds: " + reason);
                GameState loaded = SaveService.LoadGameState(savePath,
                    candidate => SaveCandidateRules.IsLoadable(candidate, VersionInfo.SaveVersion), out bool backup);
                Require(!backup, "the primary item save passes normal save validation");
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
            private readonly double statusRoll;
            private readonly int attackRoll;
            public FixedRandom(double statusRoll, int attackRoll = 0) { this.statusRoll = statusRoll; this.attackRoll = attackRoll; }
            public override int Next(int maxValue) => Math.Min(attackRoll, maxValue - 1);
            public override int Next(int minValue, int maxValue) => minValue;
            public override double NextDouble() => statusRoll;
        }

        private static bool Near(float left, float right) => Mathf.Abs(left - right) < .0001f;
        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("Honest Weight items: " + message);
        }
    }
}
