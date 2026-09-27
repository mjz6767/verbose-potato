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
    public static class RemembranceItemSmoke
    {
        private const string AtlasHash = "B987F8E7C4284A16AA0692F09B80BD5F0DF877A2C5F21F941E8F80FA2FC061CF";

        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " remembrance item smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogError(VersionInfo.ProductName + " remembrance item smoke failed: " + exception);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            RewardArtReachesProductionAdapters();
            IntrinsicsSurviveEnchantmentAndCombatIdentity();
        }

        private static void RewardArtReachesProductionAdapters()
        {
            byte[] bytes = File.ReadAllBytes(Path.Combine(Application.dataPath, "..", "Docs", "ArtReferences", RuntimeArtManifest.RemembranceItemAtlas));
            using (SHA256 hash = SHA256.Create())
            {
                Require(BitConverter.ToString(hash.ComputeHash(bytes)).Replace("-", "") == AtlasHash,
                    "the separately approved remembrance art retains its exact file pin");
            }
            Require(RuntimeArtManifest.ApprovedRuntimeFiles.Count(name => name == RuntimeArtManifest.RemembranceItemAtlas) == 1,
                "the distributable manifest includes the remembrance atlas exactly once");
            Texture2D atlas = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            GameObject host = NewHost("Remembrance item adapter smoke");
            try
            {
                Require(atlas.LoadImage(bytes), "remembrance atlas decodes");
                Require(atlas.width == 1774 && atlas.height == 887, "remembrance atlas keeps two square cells");
                Color32[] pixels = atlas.GetPixels32();
                for (int cell = 0; cell < 2; cell++)
                {
                    int visible = 0;
                    for (int y = 0; y < atlas.height; y++)
                    for (int x = cell * 887; x < (cell + 1) * 887; x++)
                        if (pixels[y * atlas.width + x].a >= 32) visible++;
                    Require(visible > 887 * 887 * 0.04f && visible < 887 * 887 * 0.70f,
                        "each keepsake has visible art surrounded by transparent space");
                }

                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                GameState state = new GameState { Mode = GameMode.Explore, Depth = 1, Seed = 2320 };
                Set(game, "state", state);
                Set(game, "remembranceItemAtlas", atlas);
                InventoryItem[] items = { SignatureItemCatalog.CreateTravelersKnot(), SignatureItemCatalog.CreateVigilkeeperWrap() };
                for (int index = 0; index < items.Length; index++)
                {
                    InventoryItem item = items[index];
                    Invoke(game, "AddInventoryItem", item);
                    Require(SignatureItemCatalog.IconIndex(item) == index + 7,
                        "the reward uses its own catalog index after the original seven signature items");
                    item.DisplayName = "A keepsake from Edda " + index;
                    Rect expectedUv = new Rect(index * .5f, 0f, .5f, 1f);
                    object[] iconArguments = { item, null, Rect.zero };
                    Require((bool)Invoke(game, "TryGetInventoryItemIcon", iconArguments), "stable identity resolves art despite a changed display label");
                    Require(ReferenceEquals(iconArguments[1], atlas) && (Rect)iconArguments[2] == expectedUv,
                        "the item resolves to its exact cell in the remembrance atlas");

                    IReadOnlyList<ArmoryRowView> rows = (IReadOnlyList<ArmoryRowView>)Invoke(game, "BuildArmoryPackRows");
                    ArmoryRowView row = rows.First(entry => entry.Key == state.Inventory.IndexOf(item));
                    Require(row.IconTexture == atlas && row.IconUv == expectedUv, "the real Pack row receives the remembrance art");
                    Invoke(game, "SelectArmoryInventoryItem", item);
                    ArmoryDetailView detail = (ArmoryDetailView)Invoke(game, "BuildInventoryItemDetail");
                    Require(detail.IconTexture == atlas && detail.IconUv == expectedUv, "the real Pack detail receives the remembrance art");
                    Require(detail.Summary.Contains(SignatureItemCatalog.Find(item).IntrinsicName), "Pack detail explains the actual signature effect");

                    Set(game, "lootPanelItem", item);
                    Set(game, "lootPanelBody", item.DisplayName);
                    Set(game, "lootPanelRequiresDismissal", true);
                    LootPopupView loot = (LootPopupView)Invoke(game, "BuildLootPopupView");
                    Require(loot.IconTexture == atlas && loot.IconUv == expectedUv && loot.CanReview,
                        "the real loot adapter receives remembrance art and an explicit review route");
                }
                Require(items[0].InstanceId != items[1].InstanceId, "the two keepsakes retain distinct pack identities");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(host);
                UnityEngine.Object.DestroyImmediate(atlas);
            }
        }

        private static void IntrinsicsSurviveEnchantmentAndCombatIdentity()
        {
            GameObject host = NewHost("Remembrance combat intrinsic smoke");
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                InventoryItem bow = SignatureItemCatalog.CreateTravelersKnot();
                InventoryItem wrap = SignatureItemCatalog.CreateVigilkeeperWrap();
                PartyMember member = new PartyMember
                {
                    Id = "remembrance-ranger", Name = "Cairn", Role = "bow", ClassKey = "ranger",
                    Hp = 100, MaxHp = 100, Skills = new SkillSet { Missile = 5 }
                };
                CombatUnit hero = new CombatUnit
                {
                    Id = member.Id, Name = member.Name, PartyIndex = 0, Side = UnitSide.Party,
                    Hp = 100, MaxHp = 100, Role = "bow", ClassKey = "ranger", Range = 5,
                    Agility = 5, AttackSpeed = 6, Skills = member.Skills, X = 1, Y = 2
                };
                CombatUnit enemy = new CombatUnit
                {
                    Id = "remembrance-target", Name = "Guard", PartyIndex = -1, Side = UnitSide.Enemy,
                    Hp = 100, MaxHp = 100, Defense = 2, Agility = 5, X = 6, Y = 2,
                    Skills = new SkillSet()
                };
                GameState state = new GameState
                {
                    Mode = GameMode.Combat, Depth = 1, SfxMuted = true, MusicMuted = true, ReducedMotion = true,
                    Party = new List<PartyMember> { member },
                    Combat = new CombatState { Units = new List<CombatUnit> { hero, enemy } }
                };
                Set(game, "state", state);
                Set(game, "rng", new System.Random(2320));
                Invoke(game, "AddInventoryItem", bow);
                Invoke(game, "AddInventoryItem", wrap);
                hero.WeaponName = "practice bow";
                int ordinaryHit = (int)Invoke(game, "AttackHitChance", hero, enemy);
                Require(ordinaryHit > 18 && ordinaryHit < 93, "hit fixture avoids clamping away the intrinsic");
                member.WeaponItemId = bow.InstanceId;
                member.ArmorItemId = wrap.InstanceId;
                bow.EquippedById = wrap.EquippedById = member.Id;
                hero.ArmorName = wrap.DisplayName;

                foreach (string phase in new[] { "plain", "permanent", "temporary", "expired", "renamed" })
                {
                    if (phase == "permanent") WeaponEnchantmentRules.ApplyPermanent(bow, "fire");
                    if (phase == "temporary") WeaponEnchantmentRules.ApplyTemporary(bow, "storm");
                    if (phase == "expired")
                        for (int victory = 0; victory < WeaponEnchantmentRules.TemporaryVictories; victory++) WeaponEnchantmentRules.AdvanceAfterVictory(bow);
                    if (phase == "renamed") bow.DisplayName = "Edda's keepsake";
                    hero.WeaponName = bow.DisplayName;
                    Require((int)Invoke(game, "AttackHitChance", hero, enemy) == ordinaryHit + 2,
                        phase + " bow supplies exactly two hit points through the real attack preview rule");
                    Require(SignatureItemCatalog.IconIndex(bow) == 7, phase + " bow preserves its art identity");
                    Require(SignatureItemRules.WeaponPowerBonus(bow) == 0, phase + " bow does not add an unadvertised damage bonus");
                }
                hero.ArmorName = wrap.DisplayName = "Edda's winter keepsake";
                foreach (string type in new[] { "cold", "physical", "fire", "poison", "death", "mind", "shock", "light" })
                {
                    hero.Hp = 100;
                    int dealt = (int)Invoke(game, "DealDamage", hero, 10, type, Color.white);
                    Require(dealt == (type == "cold" ? 9 : 10) && hero.Hp == 100 - dealt,
                        "the actual damage path applies cold-only mitigation for " + type);
                }
                hero.Hp = 100;
                Require((int)Invoke(game, "DealDamage", hero, 1, "cold", Color.white) == 1,
                    "the wrap preserves the one-damage combat floor");

                GameState resumed = JsonUtility.FromJson<GameState>(JsonUtility.ToJson(state));
                Set(game, "state", resumed);
                Require((int)Invoke(game, "AttackHitChance", resumed.Combat.Units[0], resumed.Combat.Units[1]) == ordinaryHit + 2,
                    "stable ownership and intrinsic hit survive combat save/load");
                Require((int)Invoke(game, "GearDamageReduction", resumed.Combat.Units[0], "cold") == 1,
                    "stable ownership and intrinsic cold protection survive combat save/load");
                Require(resumed.Inventory[0].PermanentEnchantmentId == "fire" && resumed.Inventory[0].DamageType == "fire",
                    "expired temporary magic leaves the paid permanent enchantment intact");
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        internal static GameObject NewHost(string name)
        {
            GameObject host = new GameObject(name);
            host.SetActive(false);
            host.hideFlags = HideFlags.HideAndDontSave;
            return host;
        }

        internal static object Invoke(object owner, string name, params object[] arguments)
        {
            MethodInfo method = owner.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Require(method != null, "runtime method exists: " + name);
            return method.Invoke(owner, arguments);
        }

        internal static void Set(object owner, string name, object value)
        {
            FieldInfo field = owner.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Require(field != null, "runtime field exists: " + name);
            field.SetValue(owner, value);
        }

        internal static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("Remembrance items: " + message);
        }
    }
}
