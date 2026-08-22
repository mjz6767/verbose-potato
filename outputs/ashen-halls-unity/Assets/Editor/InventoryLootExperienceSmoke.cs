using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class InventoryLootExperienceSmoke
    {
        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " inventory/loot experience smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " inventory/loot experience smoke failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            ExplicitSlotsOverrideMisleadingForms();
            DuplicateLootCannotMasqueradeAsLoadout();
            OwnershipValidationPrecedesEnchantmentSync();
            LootRecommendationsShareOneTargetRule();
            StrategicChangesRequireReview();
            ReassignmentGradesProtectBothRecipients();
            LootActionsFitSupportedResolutions();
            LootScreenExposesExplicitQuickEquip();
        }

        private static void ExplicitSlotsOverrideMisleadingForms()
        {
            InventoryItem swordRelic = new InventoryItem { Slot = "quest", Form = "ancient sword blade" };
            InventoryItem mailProof = new InventoryItem { Slot = "quest", Form = "chain mail fragment" };
            Require(!InventoryEquipmentRules.IsWeaponSlot(swordRelic.Slot, swordRelic.Form), "quest sword fragment must not become a weapon");
            Require(!InventoryEquipmentRules.IsArmorSlot(mailProof.Slot, mailProof.Form), "quest mail fragment must not become armor");
            Require(!InventoryEquipmentRules.IsEquippable(swordRelic), "explicit quest slot must remain non-equipment");
            Require(InventoryEquipmentRules.IsWeaponSlot("weapon", "quest token"), "explicit weapon slot remains authoritative");
            Require(!InventoryEquipmentRules.IsArmorSlot("weapon", "chain mail fragment"), "explicit weapon slot cannot also become armor");
            Require(InventoryEquipmentRules.IsWeaponSlot("", "broadsword"), "legacy form-only weapon remains supported");
        }

        private static void StrategicChangesRequireReview()
        {
            InventoryItem plainWeapon = new InventoryItem
            {
                Slot = "weapon",
                Form = "broadsword",
                DisplayName = "plain broadsword"
            };
            InventoryItem conductor = SignatureItemCatalog.CreateStormglassConductor();
            InventoryItem conductorCopy = SignatureItemCatalog.CreateStormglassConductor();

            Require(InventoryEquipmentRules.HasStrategicIdentity(conductor), "signature item is strategic gear");
            Require(InventoryEquipmentRules.RequiresDeliberateReview(conductor, plainWeapon), "gaining a signature intrinsic requires review");
            Require(InventoryEquipmentRules.StrategicChangeLabel(conductor, plainWeapon) == "Gain Conduction", "gained intrinsic is named");
            Require(InventoryEquipmentRules.RequiresDeliberateReview(plainWeapon, conductor), "losing a signature intrinsic requires review");
            Require(InventoryEquipmentRules.StrategicChangeLabel(plainWeapon, conductor) == "Lose Conduction", "lost intrinsic is named");
            Require(!InventoryEquipmentRules.RequiresDeliberateReview(conductorCopy, conductor), "the same intrinsic does not create a false warning");

            InventoryItem enchanted = new InventoryItem
            {
                Slot = "weapon",
                Form = "broadsword",
                DisplayName = "flamebound broadsword",
                PermanentEnchantmentId = "fire"
            };
            Require(InventoryEquipmentRules.HasStrategicIdentity(enchanted), "enchanted item is strategic gear");
            Require(InventoryEquipmentRules.RequiresDeliberateReview(plainWeapon, enchanted), "replacing an enchantment requires review");
            Require(InventoryEquipmentRules.StrategicChangeLabel(plainWeapon, enchanted) == "Lose enchantment", "lost enchantment is disclosed");

            InventoryItem vampiric = new InventoryItem
            {
                Slot = "weapon",
                Form = "broadsword",
                DisplayName = "vampiric iron broadsword",
                DamageType = "physical"
            };
            Require(InventoryEquipmentRules.HasStrategicIdentity(vampiric), "ordinary combat behavior is treated as strategic gear");
            Require(InventoryEquipmentRules.RequiresDeliberateReview(plainWeapon, vampiric), "losing life drain requires review");
            Require(InventoryEquipmentRules.StrategicChangeLabel(plainWeapon, vampiric) == "Lose life drain", "lost tactical behavior is named");

            InventoryItem metadataOnly = new InventoryItem
            {
                Slot = "weapon",
                Form = "venom sabre",
                Trait = "storm",
                DisplayName = "plain broadsword",
                DamageType = "physical"
            };
            Require(!InventoryEquipmentRules.HasStrategicIdentity(metadataOnly), "unequipped form and trait metadata cannot advertise inactive combat behavior");
            InventoryItem compoundStatus = new InventoryItem
            {
                Slot = "weapon",
                Form = "broadsword",
                DisplayName = "stormglass sabre of venom",
                DamageType = "physical"
            };
            Require(
                InventoryEquipmentRules.StrategicChangeLabel(plainWeapon, compoundStatus) == "Lose stun",
                "compound weapon names advertise only combat's first status in live precedence order");

            InventoryItem enchantedVampiric = new InventoryItem
            {
                Slot = "weapon",
                Form = "broadsword",
                DisplayName = "vampiric flamebound broadsword",
                DamageType = "physical",
                PermanentEnchantmentId = "fire"
            };
            Require(
                InventoryEquipmentRules.RequiresDeliberateReview(enchanted, enchantedVampiric),
                "matching enchantments do not hide a lost life-drain behavior");
            Require(
                InventoryEquipmentRules.StrategicChangeLabel(enchanted, enchantedVampiric) == "Lose life drain",
                "same-enchantment tactical loss is named without a false enchantment warning");

            InventoryItem plainArmor = Armor("plain iron mail");
            InventoryItem plate = Armor("iron plate");
            InventoryItem tower = Armor("iron tower shield");
            InventoryItem kite = Armor("iron kite shield");
            InventoryItem buckler = Armor("iron buckler");
            InventoryItem guarding = Armor("guarding iron mail");
            InventoryItem robe = Armor("linen robe");
            InventoryItem moonstone = Armor("moonstone mail");
            InventoryItem warding = Armor("warding iron mail");
            InventoryItem metadataWarding = Armor("plain iron mail");
            metadataWarding.Trait = "warding";
            Require(!InventoryEquipmentRules.HasStrategicIdentity(metadataWarding), "armor trait metadata cannot advertise a guard that the equipped name will not grant");
            Require(InventoryEquipmentRules.StrategicChangeLabel(tower, plate) == "Gain guard +2", "tower guard remains distinct from plate reduction");
            Require(InventoryEquipmentRules.StrategicChangeLabel(kite, plainArmor) == "Gain guard +2", "kite-shield guard is disclosed");
            Require(InventoryEquipmentRules.StrategicChangeLabel(buckler, plainArmor) == "Gain guard +1", "buckler guard is disclosed");
            Require(InventoryEquipmentRules.StrategicChangeLabel(guarding, plainArmor) == "Gain guard +1", "guarding armor is disclosed");
            Require(InventoryEquipmentRules.StrategicChangeLabel(robe, plainArmor) == "Gain caster guard", "caster-robe guard is disclosed");
            Require(InventoryEquipmentRules.StrategicChangeLabel(warding, moonstone) == "Gain guard +1", "warding guard remains distinct from shared nonphysical reduction");
        }

        private static void DuplicateLootCannotMasqueradeAsLoadout()
        {
            PartyMember member = new PartyMember
            {
                WeaponName = "+2 iron broadsword",
                WeaponBonus = 2,
                WeaponDamageMin = 4,
                WeaponDamageMax = 8,
                WeaponAttackSpeed = 6,
                WeaponDamageType = "physical",
                WeaponStrengthBonus = 1,
                Range = 1
            };
            InventoryItem exactBackingItem = new InventoryItem
            {
                DisplayName = member.WeaponName,
                Slot = "weapon",
                Form = "broadsword",
                Bonus = 2,
                DamageMin = 4,
                DamageMax = 8,
                AttackSpeed = 6,
                DamageType = "physical",
                StrengthBonus = 1
            };
            InventoryItem newlyLootedDuplicate = new InventoryItem
            {
                DisplayName = member.WeaponName,
                Slot = "weapon",
                Form = "broadsword",
                Bonus = 3,
                DamageMin = 5,
                DamageMax = 9,
                AttackSpeed = 7,
                DamageType = "physical",
                AgilityBonus = 1
            };

            Require(
                InventoryEquipmentRules.MatchesWeaponLoadout(exactBackingItem, member, 1),
                "the exact mechanical fingerprint can repair a legacy ownership link");
            Require(
                !InventoryEquipmentRules.MatchesWeaponLoadout(newlyLootedDuplicate, member, 1),
                "a same-name loot roll with different mechanics cannot masquerade as equipped");

            MethodInfo repairMethod = typeof(AshenHallsGame).GetMethod(
                "EnsureInventoryEquipmentLinks",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Require(repairMethod != null, "ownership-link repair method remains available");
            ParameterInfo[] parameters = repairMethod.GetParameters();
            Require(parameters.Length == 1 && parameters[0].IsOptional, "ownership-link repair has one explicit optional boundary");
            Require(parameters[0].DefaultValue is bool defaultRepair && !defaultRepair, "ordinary Loot and Inventory refreshes validate links without creating them");
        }

        private static void OwnershipValidationPrecedesEnchantmentSync()
        {
            string combatSource = File.ReadAllText(Path.Combine(
                Application.dataPath,
                "Scripts",
                "Legacy",
                "AshenHallsGame.Combat.cs"));
            string normalization = MethodRegion(combatSource, "private void NormalizeWeaponEnchantments(");
            int migration = normalization.IndexOf("MigrateLegacyMaudEnchantment();", StringComparison.Ordinal);
            int validation = normalization.IndexOf("EnsureInventoryEquipmentLinks();", StringComparison.Ordinal);
            int syncLoop = normalization.IndexOf("foreach (InventoryItem item", StringComparison.Ordinal);
            Require(migration >= 0, "legacy enchantment migration remains inside normalization");
            Require(validation > migration, "ownership claims are validated after the explicit legacy migration");
            Require(syncLoop > validation, "ownership claims are validated before any enchantment rebuild can sync into a party loadout");
            string acquisitionRecommendation = MethodRegion(combatSource, "private string AutoEquipItem(");
            Require(
                !acquisitionRecommendation.Contains("EquipInventoryItemToMember("),
                "loot acquisition recommends a target without silently changing the loadout");
            Require(
                acquisitionRecommendation.Contains("Kept in the pack for your decision."),
                "clear upgrades remain available for the explicit loot action");

            string coreSource = File.ReadAllText(Path.Combine(
                Application.dataPath,
                "Scripts",
                "Legacy",
                "AshenHallsGame.Core.cs"));
            string loadBoundary = MethodRegion(coreSource, "private void EnsureWorldState(");
            Require(
                loadBoundary.Contains("EnsureInventoryEquipmentLinks(true);"),
                "only the load boundary opts into missing-link repair");
            string legacyDirectory = Path.Combine(Application.dataPath, "Scripts", "Legacy");
            int explicitRepairCalls = 0;
            foreach (string sourcePath in Directory.GetFiles(legacyDirectory, "AshenHallsGame*.cs"))
            {
                explicitRepairCalls += CountOccurrences(
                    File.ReadAllText(sourcePath),
                    "EnsureInventoryEquipmentLinks(true);");
            }
            Require(
                explicitRepairCalls == 1,
                "missing-link repair has one explicit call site");
        }

        private static void LootRecommendationsShareOneTargetRule()
        {
            string legacyDirectory = Path.Combine(Application.dataPath, "Scripts", "Legacy");
            string inventorySource = File.ReadAllText(Path.Combine(legacyDirectory, "AshenHallsGame.InventoryEquipment.cs"));
            string targetRule = MethodRegion(inventorySource, "private PartyMember BestLootInventoryFit(");
            Require(targetRule.Contains("candidate.Hp <= 0"), "loot recommendations exclude adventurers who cannot equip right now");
            Require(targetRule.Contains("score == comparisonScore && roleFit <= bestRoleFit"), "loot recommendation ties use one explicit role-fit rule");

            string combatSource = File.ReadAllText(Path.Combine(legacyDirectory, "AshenHallsGame.Combat.cs"));
            string acquisitionRecommendation = MethodRegion(combatSource, "private string AutoEquipItem(");
            Require(acquisitionRecommendation.Contains("BestLootInventoryFit(item"), "acquisition guidance uses the canonical loot target");

            string lootSource = File.ReadAllText(Path.Combine(legacyDirectory, "AshenHallsGame.Loot.cs"));
            string popupView = MethodRegion(lootSource, "private LootPopupView BuildLootPopupView(");
            string quickEquip = MethodRegion(lootSource, "private void EquipLootToBestFit(");
            Require(popupView.Contains("BestLootInventoryFit(item"), "the quick-equip label uses the canonical loot target");
            Require(quickEquip.Contains("BestLootInventoryFit(item"), "the quick-equip action uses the canonical loot target");

            string armorySource = File.ReadAllText(Path.Combine(legacyDirectory, "AshenHallsGame.Armory.cs"));
            string packRows = MethodRegion(armorySource, "private IReadOnlyList<ArmoryRowView> BuildArmoryPackRows(");
            Require(packRows.Contains("InventoryGradeLabelFor(item, best)"), "inventory rows use the same strategic Review grade as their detail action");
        }

        private static void ReassignmentGradesProtectBothRecipients()
        {
            int hiddenSacrifice = InventoryEquipmentRules.ReassignmentScore(18, -7);
            int neutralExchange = InventoryEquipmentRules.ReassignmentScore(18, 1);
            int mutualUpgrade = InventoryEquipmentRules.ReassignmentScore(9, 5);

            Require(hiddenSacrifice == -7, "swap score is owned by the harmed recipient");
            Require(InventoryEquipmentRules.Grade(hiddenSacrifice) == InventoryUpgradeGrade.Downgrade, "one severe loss makes the swap a tradeoff");
            Require(InventoryEquipmentRules.Grade(neutralExchange) == InventoryUpgradeGrade.Sidegrade, "one neutral recipient prevents an upgrade claim");
            Require(InventoryEquipmentRules.Grade(mutualUpgrade) == InventoryUpgradeGrade.Upgrade, "both recipients improving is an upgrade");
        }

        private static InventoryItem Armor(string displayName)
        {
            return new InventoryItem
            {
                Slot = "armor",
                Form = displayName,
                DisplayName = displayName
            };
        }

        private static void LootActionsFitSupportedResolutions()
        {
            Vector2Int[] sizes =
            {
                new Vector2Int(960, 600),
                new Vector2Int(1280, 720),
                new Vector2Int(1600, 900),
                new Vector2Int(1920, 1080),
                new Vector2Int(2048, 1152)
            };

            foreach (Vector2Int size in sizes)
            {
                LootPopupGeometry geometry = LootPopupLayout.Calculate(size.x, size.y);
                Require(geometry.Fits(size.x, size.y), $"loot geometry must fit {size.x}x{size.y}");
                Require(geometry.QuickEquipButton.width >= 160f, $"quick-equip action stays readable at {size.x}x{size.y}");
                Require(geometry.QuickEquipButton.xMax <= geometry.ReviewButton.xMin, $"quick-equip and review actions do not overlap at {size.x}x{size.y}");
                Require(geometry.ReviewButton.xMax <= geometry.DismissButton.xMin, $"review and Continue actions do not overlap at {size.x}x{size.y}");
            }
        }

        private static void LootScreenExposesExplicitQuickEquip()
        {
            GameObject root = new GameObject("Inventory Loot Experience Smoke");
            try
            {
                bool quickEquipped = false;
                bool reviewed = false;
                bool dismissed = false;
                LootPopupView view = new LootPopupView
                {
                    Visible = true,
                    HasItem = true,
                    CanReview = true,
                    CanQuickEquip = true,
                    Title = "Cache opened",
                    ItemName = "+2 Stormglass Conductor",
                    ItemType = "Weapon",
                    Rarity = "Quest",
                    TraitLine = "Signature · Conduction",
                    Outcome = "Added to inventory",
                    Comparison = "Best fit: Vesh · Review",
                    QuickEquipActionLabel = "Equip to Vesh",
                    ReviewActionLabel = "Compare others",
                    IconLabel = "WPN"
                };

                LootPopupScreen screen = root.AddComponent<LootPopupScreen>();
                screen.Bind(new LootPopupBindings
                {
                    View = () => view,
                    QuickEquip = () => quickEquipped = true,
                    ReviewInventory = () => reviewed = true,
                    Dismiss = () => dismissed = true
                });
                screen.SetVisible(true);
                screen.Refresh();

                Require(screen.IsReady, "loot screen builds all three actions");
                Require(screen.HasQuickEquipActionForTest, "stored equipment exposes quick equip");
                Require(screen.HasReviewActionForTest, "stored equipment retains compare-all navigation");
                Require(screen.QuickEquipActionLabelForTest == "Equip to Vesh", "quick-equip target is explicit");
                Require(screen.ReviewActionLabelForTest == "Compare others", "review action explains the wider comparison");
                Require(screen.PrimaryActionLabelForTest == "Continue", "Continue remains the safe primary dismissal");

                screen.InvokeQuickEquipForTest();
                screen.InvokeReviewForTest();
                screen.InvokeDismissForTest();
                Require(quickEquipped, "quick-equip binding invokes exactly from its explicit action");
                Require(reviewed, "review binding remains available");
                Require(dismissed, "Continue binding remains available");

                view.CanQuickEquip = false;
                screen.Refresh();
                Require(!screen.HasQuickEquipActionForTest, "quick equip disappears after ownership changes");
                Require(screen.HasReviewActionForTest, "review remains after quick equip disappears");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("Inventory/loot experience smoke failed: " + message);
        }

        private static string MethodRegion(string source, string signature)
        {
            int start = source.IndexOf(signature, StringComparison.Ordinal);
            Require(start >= 0, "expected method exists: " + signature);
            int end = source.IndexOf("\n        private ", start + signature.Length, StringComparison.Ordinal);
            return end < 0 ? source.Substring(start) : source.Substring(start, end - start);
        }

        private static int CountOccurrences(string source, string value)
        {
            int count = 0;
            int at = 0;
            while ((at = source.IndexOf(value, at, StringComparison.Ordinal)) >= 0)
            {
                count++;
                at += value.Length;
            }
            return count;
        }
    }
}
