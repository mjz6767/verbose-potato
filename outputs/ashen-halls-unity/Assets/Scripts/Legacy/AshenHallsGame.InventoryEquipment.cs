using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private int armoryPackFilter;
        private int armorySelectedInventoryIndex = -1;
        private int armorySelectedPartyIndex;

        private void EnsureInventoryEquipmentLinks(bool repairMissingLinks = false)
        {
            if (state?.Inventory == null || state.Party == null) return;

            EnsurePartyInventoryIds();
            Dictionary<string, PartyMember> membersById = state.Party
                .Where(member => member != null && !string.IsNullOrWhiteSpace(member.Id))
                .GroupBy(member => member.Id)
                .ToDictionary(group => group.Key, group => group.First());
            HashSet<string> claimedSlots = new HashSet<string>(StringComparer.Ordinal);

            foreach (InventoryItem item in state.Inventory)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.EquippedById)) continue;
                if (!membersById.TryGetValue(item.EquippedById, out PartyMember owner)
                    || !MemberGearMatchesInventoryItem(owner, item))
                {
                    item.EquippedById = "";
                    continue;
                }

                string claim = EquipmentClaimKey(owner, InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form));
                if (!claimedSlots.Add(claim)) item.EquippedById = "";
            }

            if (!repairMissingLinks) return;
            foreach (PartyMember member in state.Party)
            {
                if (member == null) continue;
                LinkMatchingInventoryItem(member, true, claimedSlots);
                LinkMatchingInventoryItem(member, false, claimedSlots);
            }
        }

        private void EnsurePartyInventoryIds()
        {
            if (state?.Party == null) return;
            HashSet<string> used = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < state.Party.Count; i++)
            {
                PartyMember member = state.Party[i];
                if (member == null) continue;
                string id = (member.Id ?? "").Trim();
                if (string.IsNullOrEmpty(id) || used.Contains(id))
                {
                    string stem = string.IsNullOrWhiteSpace(member.Name)
                        ? "adventurer"
                        : new string(member.Name.ToLowerInvariant().Where(char.IsLetterOrDigit).ToArray());
                    if (string.IsNullOrEmpty(stem)) stem = "adventurer";
                    id = $"party-{i + 1}-{stem}";
                    int suffix = 2;
                    while (used.Contains(id)) id = $"party-{i + 1}-{stem}-{suffix++}";
                    member.Id = id;
                }
                used.Add(id);
            }
        }

        private void LinkMatchingInventoryItem(PartyMember member, bool weapon, HashSet<string> claimedSlots)
        {
            if (member == null || state?.Inventory == null) return;
            string claim = EquipmentClaimKey(member, weapon);
            if (claimedSlots.Contains(claim)) return;
            string equippedName = weapon ? member.WeaponName : member.ArmorName;
            if (string.IsNullOrWhiteSpace(equippedName)) return;

            InventoryItem match = state.Inventory.LastOrDefault(item =>
                item != null
                && string.IsNullOrWhiteSpace(item.EquippedById)
                && InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form) == weapon
                && MemberGearMatchesInventoryItem(member, item));
            if (match == null) return;
            match.EquippedById = member.Id;
            claimedSlots.Add(claim);
        }

        private static string EquipmentClaimKey(PartyMember member, bool weapon)
        {
            return (member?.Id ?? "") + (weapon ? "|weapon" : "|armor");
        }

        private bool MemberGearMatchesInventoryItem(PartyMember member, InventoryItem item)
        {
            if (member == null || item == null || !InventoryEquipmentRules.IsEquippable(item)) return false;
            bool weapon = InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form);
            return weapon
                ? InventoryEquipmentRules.MatchesWeaponLoadout(item, member, EffectiveWeaponRange(item, member))
                : InventoryEquipmentRules.MatchesArmorLoadout(item, member, ArmorDefenseBonus(item));
        }

        private PartyMember EquippedMember(InventoryItem item)
        {
            if (item == null || state?.Party == null || string.IsNullOrWhiteSpace(item.EquippedById)) return null;
            return state.Party.FirstOrDefault(member =>
                member != null
                && string.Equals(member.Id, item.EquippedById, StringComparison.Ordinal)
                && MemberGearMatchesInventoryItem(member, item));
        }

        private InventoryItem EquippedInventoryItem(PartyMember member, bool weapon)
        {
            if (member == null || state?.Inventory == null) return null;
            return state.Inventory.LastOrDefault(item =>
                item != null
                && string.Equals(item.EquippedById, member.Id, StringComparison.Ordinal)
                && InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form) == weapon);
        }

        private int EffectiveWeaponRange(InventoryItem item, PartyMember member)
        {
            return Mathf.Max(WeaponRange(item, member), StartingRange(member?.Role));
        }

        private InventoryItem SnapshotMemberEquipment(PartyMember member, bool weapon)
        {
            if (member == null) return null;
            string displayName = weapon ? member.WeaponName : member.ArmorName;
            if (string.IsNullOrWhiteSpace(displayName)) return null;

            InventoryItem snapshot;
            if (weapon)
            {
                snapshot = new InventoryItem
                {
                    Mark = "loadout-snapshot",
                    EquippedById = member.Id,
                    Material = EnchantmentWeaponMaterial(displayName),
                    Form = EnchantmentWeaponForm(displayName),
                    Trait = EnchantmentWeaponTrait(displayName),
                    Slot = "weapon",
                    Bonus = member.WeaponBonus,
                    StrengthBonus = member.WeaponStrengthBonus,
                    IntelligenceBonus = member.WeaponIntelligenceBonus,
                    AgilityBonus = member.WeaponAgilityBonus,
                    HealthBonus = member.WeaponHealthBonus,
                    DamageMin = Mathf.Max(1, member.WeaponDamageMin > 0 ? member.WeaponDamageMin : member.DamageMin),
                    DamageMax = Mathf.Max(
                        Mathf.Max(1, member.WeaponDamageMin > 0 ? member.WeaponDamageMin : member.DamageMin) + 1,
                        member.WeaponDamageMax > 0 ? member.WeaponDamageMax : member.DamageMax),
                    AttackSpeed = Mathf.Max(1, member.WeaponAttackSpeed > 0 ? member.WeaponAttackSpeed : member.AttackSpeed),
                    Rarity = member.WeaponBonus > 2 ? "rare" : member.WeaponBonus > 0 ? "common" : "starter",
                    DamageType = string.IsNullOrWhiteSpace(member.WeaponDamageType) ? "physical" : member.WeaponDamageType,
                    DisplayName = displayName
                };
            }
            else
            {
                snapshot = new InventoryItem
                {
                    Mark = "loadout-snapshot",
                    EquippedById = member.Id,
                    Material = EnchantmentWeaponMaterial(displayName),
                    Form = "armor",
                    Trait = EnchantmentWeaponTrait(displayName),
                    Slot = "armor",
                    Bonus = member.ArmorBonus,
                    StrengthBonus = member.ArmorStrengthBonus,
                    IntelligenceBonus = member.ArmorIntelligenceBonus,
                    AgilityBonus = member.ArmorAgilityBonus,
                    HealthBonus = member.ArmorHealthBonus,
                    Rarity = member.ArmorBonus > 2 ? "rare" : member.ArmorBonus > 0 ? "common" : "starter",
                    DisplayName = displayName
                };
            }

            // Signature identity remains stable beneath Maud's visible enchantment
            // prefixes; RepairIdentity deliberately preserves prefixed display text.
            SignatureItemCatalog.RepairIdentity(snapshot);
            return snapshot;
        }

        private InventoryItem InventorySwapReplacement(PartyMember target, bool weapon)
        {
            return EquippedInventoryItem(target, weapon) ?? SnapshotMemberEquipment(target, weapon);
        }

        private void ApplyInventoryItemToLoadout(InventoryItem item, PartyMember target, bool weapon)
        {
            if (item == null || target == null) return;
            if (weapon)
            {
                target.WeaponName = item.DisplayName;
                target.WeaponBonus = item.Bonus;
                target.WeaponDamageType = string.IsNullOrEmpty(item.DamageType) ? "physical" : item.DamageType;
                target.WeaponDamageMin = Mathf.Max(1, item.DamageMin);
                target.WeaponDamageMax = Mathf.Max(target.WeaponDamageMin + 1, item.DamageMax);
                target.WeaponAttackSpeed = Mathf.Max(1, item.AttackSpeed);
                ApplyGearStatBonuses(target, item, true);
                target.Range = EffectiveWeaponRange(item, target);
                return;
            }

            target.ArmorName = item.DisplayName;
            target.ArmorBonus = ArmorDefenseBonus(item);
            ApplyGearStatBonuses(target, item, false);
        }

        private int InventoryReassignmentScore(InventoryItem item, PartyMember target, PartyMember currentOwner)
        {
            if (item == null || target == null || currentOwner == null || target == currentOwner) return int.MinValue / 4;
            bool weapon = InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form);
            InventoryItem replacement = InventorySwapReplacement(target, weapon);
            if (replacement == null
                || !InventoryEquipmentRules.IsEquippable(replacement)
                || InventoryEquipmentRules.IsWeaponSlot(replacement.Slot, replacement.Form) != weapon)
            {
                return int.MinValue / 4;
            }
            int targetDelta = InventoryComparisonScore(item, target);
            int currentOwnerDelta = InventoryComparisonScore(replacement, currentOwner);
            return InventoryEquipmentRules.ReassignmentScore(targetDelta, currentOwnerDelta);
        }

        private bool EquipInventoryItemToMember(InventoryItem item, PartyMember target, out string result)
        {
            result = "";
            if (item == null || target == null || state?.Party == null)
            {
                result = "That equipment choice is unavailable.";
                return false;
            }
            if (!InventoryEquipmentRules.IsEquippable(item))
            {
                result = $"{item.DisplayName} is not equippable.";
                return false;
            }

            EnsureInventoryList();
            EnsureInventoryEquipmentLinks();
            bool weapon = InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form);
            PartyMember currentOwner = EquippedMember(item);
            if (currentOwner == target)
            {
                result = $"{target.Name} already has {item.DisplayName} equipped.";
                return false;
            }

            if (currentOwner != null)
            {
                InventoryItem replacement = InventorySwapReplacement(target, weapon);
                if (replacement == null
                    || !InventoryEquipmentRules.IsEquippable(replacement)
                    || InventoryEquipmentRules.IsWeaponSlot(replacement.Slot, replacement.Form) != weapon)
                {
                    result = $"{target.Name} has no {InventoryEquipmentRules.SlotLabel(item.Slot, item.Form).ToLowerInvariant()} to exchange with {currentOwner.Name}.";
                    return false;
                }

                string replacementName = replacement.DisplayName;
                InventoryItem linkedReplacement = state.Inventory.Contains(replacement) ? replacement : null;
                ApplyInventoryItemToLoadout(replacement, currentOwner, weapon);
                ApplyInventoryItemToLoadout(item, target, weapon);
                if (linkedReplacement != null) linkedReplacement.EquippedById = currentOwner.Id;
                item.EquippedById = target.Id;
                RecalculateMember(currentOwner);
                RecalculateMember(target);
                result = $"{target.Name} equips {item.DisplayName}; {currentOwner.Name} receives {replacementName}.";
                return true;
            }

            InventoryItem previousItem = EquippedInventoryItem(target, weapon);
            if (previousItem != null && previousItem != item) previousItem.EquippedById = "";
            string old = weapon
                ? string.IsNullOrWhiteSpace(target.WeaponName) ? "their previous weapon" : target.WeaponName
                : string.IsNullOrWhiteSpace(target.ArmorName) ? "their previous armor" : target.ArmorName;

            ApplyInventoryItemToLoadout(item, target, weapon);
            item.EquippedById = target.Id;
            RecalculateMember(target);
            result = weapon
                ? $"{target.Name} equips {item.DisplayName} over {old}. {ItemBehaviorLine(item, target)}"
                : $"{target.Name} wears {item.DisplayName} over {old}. {ItemBehaviorLine(item, target)}";
            return true;
        }

        private int InventoryComparisonScore(InventoryItem item, PartyMember member)
        {
            if (item == null || member == null) return int.MinValue / 4;
            if (!InventoryEquipmentRules.IsEquippable(item)) return int.MinValue / 4;
            bool weapon = InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form);
            if (weapon)
            {
                int newDamage = Mathf.Max(1, item.DamageMin) + Mathf.Max(Mathf.Max(1, item.DamageMin) + 1, item.DamageMax);
                int oldMin = Mathf.Max(1, member.WeaponDamageMin > 0 ? member.WeaponDamageMin : member.DamageMin);
                int oldMax = Mathf.Max(oldMin + 1, member.WeaponDamageMax > 0 ? member.WeaponDamageMax : member.DamageMax);
                int newStats = item.StrengthBonus + item.IntelligenceBonus + item.AgilityBonus + item.HealthBonus;
                int oldStats = member.WeaponStrengthBonus + member.WeaponIntelligenceBonus + member.WeaponAgilityBonus + member.WeaponHealthBonus;
                InventoryItem currentWeapon = EquippedInventoryItem(member, true) ?? new InventoryItem
                {
                    Slot = "weapon",
                    Form = member.WeaponName,
                    DisplayName = member.WeaponName
                };
                int newScore = item.Bonus * 10
                    + newDamage * 3
                    + Mathf.Max(1, item.AttackSpeed)
                    + EffectiveWeaponRange(item, member) * 2
                    + WeaponRoleFit(item, member) * 3
                    + newStats * 4;
                int oldScore = member.WeaponBonus * 10
                    + (oldMin + oldMax) * 3
                    + Mathf.Max(1, member.WeaponAttackSpeed > 0 ? member.WeaponAttackSpeed : member.AttackSpeed)
                    + Mathf.Max(1, member.Range) * 2
                    + WeaponRoleFit(currentWeapon, member) * 3
                    + oldStats * 4;
                return newScore - oldScore;
            }

            int newArmorStats = item.StrengthBonus + item.IntelligenceBonus + item.AgilityBonus + item.HealthBonus;
            int oldArmorStats = member.ArmorStrengthBonus + member.ArmorIntelligenceBonus + member.ArmorAgilityBonus + member.ArmorHealthBonus;
            InventoryItem currentArmor = EquippedInventoryItem(member, false) ?? new InventoryItem
            {
                Slot = "armor",
                Form = member.ArmorName,
                DisplayName = member.ArmorName
            };
            int newArmorScore = ArmorDefenseBonus(item) * 14
                + ArmorAgilityModifier(item.DisplayName) * 3
                + newArmorStats * 4
                - ArmorRolePenalty(item, member) * 2;
            int oldArmorScore = member.ArmorBonus * 14
                + ArmorAgilityModifier(member.ArmorName) * 3
                + oldArmorStats * 4
                - ArmorRolePenalty(currentArmor, member) * 2;
            return newArmorScore - oldArmorScore;
        }

        private PartyMember BestInventoryFit(InventoryItem item, out int partyIndex, out int comparisonScore)
        {
            partyIndex = -1;
            comparisonScore = int.MinValue / 4;
            if (!InventoryEquipmentRules.IsEquippable(item) || state?.Party == null) return null;
            PartyMember best = null;
            for (int i = 0; i < state.Party.Count; i++)
            {
                PartyMember candidate = state.Party[i];
                if (candidate == null) continue;
                int score = InventoryComparisonScore(item, candidate);
                if (best != null && score <= comparisonScore) continue;
                best = candidate;
                partyIndex = i;
                comparisonScore = score;
            }
            return best;
        }

        private PartyMember BestLootInventoryFit(InventoryItem item, out int partyIndex, out int comparisonScore)
        {
            partyIndex = -1;
            comparisonScore = int.MinValue / 4;
            if (!InventoryEquipmentRules.IsEquippable(item) || state?.Party == null) return null;

            bool weapon = InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form);
            int bestRoleFit = int.MinValue;
            PartyMember best = null;
            for (int i = 0; i < state.Party.Count; i++)
            {
                PartyMember candidate = state.Party[i];
                if (candidate == null || candidate.Hp <= 0) continue;
                int score = InventoryComparisonScore(item, candidate);
                int roleFit = weapon ? WeaponRoleFit(item, candidate) : -ArmorRolePenalty(item, candidate);
                if (best != null
                    && (score < comparisonScore || (score == comparisonScore && roleFit <= bestRoleFit)))
                {
                    continue;
                }

                best = candidate;
                partyIndex = i;
                comparisonScore = score;
                bestRoleFit = roleFit;
            }
            return best;
        }

        private int BestInventoryComparisonScore(InventoryItem item)
        {
            PartyMember owner = EquippedMember(item);
            if (owner == null)
            {
                return BestInventoryFit(item, out _, out int score) == null ? int.MinValue / 4 : score;
            }

            int bestScore = int.MinValue / 4;
            if (state?.Party == null) return bestScore;
            foreach (PartyMember target in state.Party)
            {
                if (target == null || target == owner) continue;
                bestScore = Mathf.Max(bestScore, InventoryReassignmentScore(item, target, owner));
            }
            return bestScore;
        }

        private InventoryUpgradeGrade InventoryGradeFor(InventoryItem item, PartyMember member)
        {
            if (item == null || member == null) return InventoryUpgradeGrade.Sidegrade;
            if (EquippedMember(item) == member) return InventoryUpgradeGrade.Sidegrade;
            return InventoryEquipmentRules.Grade(InventoryComparisonScore(item, member));
        }

        private string InventoryGradeLabelFor(InventoryItem item, PartyMember member)
        {
            if (item == null || member == null) return "Review";
            InventoryItem current = InventoryCurrentEquipment(member, InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form));
            return InventoryEquipmentRules.RequiresDeliberateReview(item, current)
                ? "Review"
                : InventoryEquipmentRules.GradeLabel(InventoryGradeFor(item, member));
        }

        private InventoryItem InventoryCurrentEquipment(PartyMember member, bool weapon)
        {
            return EquippedInventoryItem(member, weapon) ?? SnapshotMemberEquipment(member, weapon);
        }

        private bool InventoryItemIsUpgrade(InventoryItem item)
        {
            if (EquippedMember(item) != null || state?.Party == null) return false;
            bool weapon = InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form);
            return state.Party.Any(member =>
            {
                if (member == null
                    || InventoryEquipmentRules.Grade(InventoryComparisonScore(item, member)) != InventoryUpgradeGrade.Upgrade)
                {
                    return false;
                }

                return !InventoryEquipmentRules.RequiresDeliberateReview(
                    item,
                    InventoryCurrentEquipment(member, weapon));
            });
        }

        private string InventoryComparisonLine(InventoryItem item, PartyMember member)
        {
            if (item == null || member == null) return "";
            bool weapon = InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form);
            if (weapon)
            {
                int oldMin = Mathf.Max(1, member.WeaponDamageMin > 0 ? member.WeaponDamageMin : member.DamageMin);
                int oldMax = Mathf.Max(oldMin + 1, member.WeaponDamageMax > 0 ? member.WeaponDamageMax : member.DamageMax);
                int newMin = Mathf.Max(1, item.DamageMin);
                int newMax = Mathf.Max(newMin + 1, item.DamageMax);
                int oldSpeed = Mathf.Max(1, member.WeaponAttackSpeed > 0 ? member.WeaponAttackSpeed : member.AttackSpeed);
                int newSpeed = Mathf.Max(1, item.AttackSpeed);
                int oldRange = Mathf.Max(1, member.Range);
                int newRange = EffectiveWeaponRange(item, member);
                int oldStats = member.WeaponStrengthBonus + member.WeaponIntelligenceBonus + member.WeaponAgilityBonus + member.WeaponHealthBonus;
                int newStats = item.StrengthBonus + item.IntelligenceBonus + item.AgilityBonus + item.HealthBonus;
                InventoryItem currentWeapon = EquippedInventoryItem(member, true) ?? SnapshotMemberEquipment(member, true);
                List<string> comparisons = new List<string>
                {
                    $"{newMin}-{newMax} dmg vs {oldMin}-{oldMax}",
                    ComparisonToken("BON", item.Bonus, member.WeaponBonus),
                    ComparisonToken("SPD", newSpeed, oldSpeed),
                    ComparisonToken("RNG", newRange, oldRange)
                };
                string strategicChange = InventoryEquipmentRules.StrategicChangeLabel(item, currentWeapon);
                if (!string.IsNullOrWhiteSpace(strategicChange)) comparisons.Insert(0, strategicChange);
                if (currentWeapon != null && WeaponRoleFit(item, member) != WeaponRoleFit(currentWeapon, member))
                {
                    comparisons.Add(ComparisonToken("FIT", WeaponRoleFit(item, member), WeaponRoleFit(currentWeapon, member)));
                }
                if (newStats != oldStats) comparisons.Add(ComparisonToken("ATTR", newStats, oldStats));
                return string.Join(" / ", comparisons);
            }

            int newArmor = ArmorDefenseBonus(item);
            int oldArmor = member.ArmorBonus;
            int newAgility = ArmorAgilityModifier(item.DisplayName);
            int oldAgility = ArmorAgilityModifier(member.ArmorName);
            int newArmorStats = item.StrengthBonus + item.IntelligenceBonus + item.AgilityBonus + item.HealthBonus;
            int oldArmorStats = member.ArmorStrengthBonus + member.ArmorIntelligenceBonus + member.ArmorAgilityBonus + member.ArmorHealthBonus;
            InventoryItem currentArmor = EquippedInventoryItem(member, false) ?? SnapshotMemberEquipment(member, false);
            List<string> armorComparisons = new List<string>
            {
                ComparisonToken("ARM", newArmor, oldArmor),
                ComparisonToken("AGI", newAgility, oldAgility)
            };
            string armorStrategicChange = InventoryEquipmentRules.StrategicChangeLabel(item, currentArmor);
            if (!string.IsNullOrWhiteSpace(armorStrategicChange)) armorComparisons.Insert(0, armorStrategicChange);
            if (currentArmor != null && ArmorRolePenalty(item, member) != ArmorRolePenalty(currentArmor, member))
            {
                armorComparisons.Add(ComparisonToken("FIT", -ArmorRolePenalty(item, member), -ArmorRolePenalty(currentArmor, member)));
            }
            if (newArmorStats != oldArmorStats) armorComparisons.Add(ComparisonToken("ATTR", newArmorStats, oldArmorStats));
            return string.Join(" / ", armorComparisons);
        }

        private static string ComparisonToken(string label, int next, int current)
        {
            int delta = next - current;
            string comparison = delta == 0 ? "=" : InventoryEquipmentRules.SignedDelta(delta);
            return $"{label} {next} ({comparison})";
        }

        private string InventoryOwnerLine(InventoryItem item)
        {
            PartyMember owner = EquippedMember(item);
            if (owner != null) return "Equipped by " + owner.Name;
            PartyMember best = BestInventoryFit(item, out _, out _);
            if (best == null) return "Stored in the inventory";
            return $"{InventoryGradeLabelFor(item, best)} for {best.Name}";
        }

        private string InventoryRarityAccent(string rarity)
        {
            switch (InventoryEquipmentRules.RarityRank(rarity))
            {
                case 6: return "#ef7b62";
                case 5: return "#f0c56a";
                case 4: return "#ba8fe7";
                case 3: return "#72aee8";
                case 2: return "#69c7a7";
                default: return "#aeb5ad";
            }
        }

        private static string SignatureItemPresentationLine(InventoryItem item)
        {
            SignatureItemDefinition signature = SignatureItemCatalog.Find(item);
            if (signature == null) return "";

            string intrinsicName = (signature.IntrinsicName ?? "").Trim();
            string intrinsicSummary = (signature.IntrinsicSummary ?? "").Trim();
            if (intrinsicName.Length == 0 && intrinsicSummary.Length == 0) return "Signature";
            if (intrinsicName.Length == 0) return "Signature · " + intrinsicSummary;
            if (intrinsicSummary.Length == 0) return "Signature · " + intrinsicName;
            return $"Signature · {intrinsicName}: {intrinsicSummary}";
        }

        private bool TryGetInventoryItemIcon(InventoryItem item, out Texture2D texture, out Rect uv)
        {
            texture = null;
            uv = Rect.zero;
            if (item == null) return false;

            int uniqueIndex = UniqueItemIconIndex(item.DisplayName);
            if (uniqueIndex >= 0 && IsUniqueItemAtlas())
            {
                texture = uniqueItemAtlas;
                uv = NormalizeAtlasRect(texture, AtlasCell(texture, uniqueIndex, 5, 4));
                return true;
            }

            int consumableIndex = InventoryConsumableIconIndex(item.DisplayName, item.Slot);
            if (consumableIndex >= 0 && inventoryConsumableAtlas != null)
            {
                texture = inventoryConsumableAtlas;
                uv = NormalizeAtlasRect(texture, AtlasCell(texture, consumableIndex, 5, 4));
                return true;
            }

            int itemIndex = ItemIconIndex(item.DisplayName, item.Slot);
            if (itemIndex < 0 || itemIconAtlas == null) return false;
            texture = itemIconAtlas;
            uv = NormalizeAtlasRect(texture, AtlasCell(texture, itemIndex, 5, 4));
            return true;
        }

        private static Rect NormalizeAtlasRect(Texture2D texture, Rect pixelRect)
        {
            if (texture == null || texture.width <= 0 || texture.height <= 0) return Rect.zero;
            return new Rect(
                pixelRect.x / texture.width,
                1f - (pixelRect.y + pixelRect.height) / texture.height,
                pixelRect.width / texture.width,
                pixelRect.height / texture.height);
        }

        private void StageInventoryVisualSmoke(bool showLoot)
        {
            QuickStart();
            EnsureInventoryList();
            InventoryItem[] showcase =
            {
                new InventoryItem
                {
                    DisplayName = "+4 stormglass sabre of haste",
                    Material = "stormglass",
                    Form = "sabre",
                    Trait = "haste",
                    Slot = "weapon",
                    Bonus = 4,
                    AgilityBonus = 1,
                    DamageMin = 4,
                    DamageMax = 9,
                    AttackSpeed = 12,
                    Rarity = "rare",
                    DamageType = "shock"
                },
                new InventoryItem
                {
                    DisplayName = "+3 moonstone prayer focus",
                    Material = "moonstone",
                    Form = "prayer focus",
                    Trait = "warding",
                    Slot = "weapon",
                    Bonus = 3,
                    IntelligenceBonus = 2,
                    DamageMin = 2,
                    DamageMax = 7,
                    AttackSpeed = 8,
                    Rarity = "epic",
                    DamageType = "light"
                },
                new InventoryItem
                {
                    DisplayName = "+3 mithril scout leathers",
                    Material = "mithril",
                    Form = "scout leathers",
                    Trait = "weightless",
                    Slot = "armor",
                    Bonus = 3,
                    AgilityBonus = 2,
                    HealthBonus = 1,
                    Rarity = "rare"
                },
                new InventoryItem
                {
                    DisplayName = "+4 adamantine plate cuirass",
                    Material = "adamantine",
                    Form = "plate cuirass",
                    Trait = "guarding",
                    Slot = "armor",
                    Bonus = 4,
                    StrengthBonus = 1,
                    HealthBonus = 2,
                    Rarity = "epic"
                },
                new InventoryItem
                {
                    DisplayName = "+2 blackglass ritual knife",
                    Material = "blackglass",
                    Form = "ritual knife",
                    Trait = "death",
                    Slot = "weapon",
                    Bonus = 2,
                    IntelligenceBonus = 1,
                    DamageMin = 3,
                    DamageMax = 8,
                    AttackSpeed = 10,
                    Rarity = "uncommon",
                    DamageType = "death"
                }
            };

            foreach (InventoryItem item in showcase) state.Inventory.Add(item);
            EnsureInventoryEquipmentLinks();
            if (state.Party != null && state.Party.Count > 1)
            {
                EquipInventoryItemToMember(showcase[2], state.Party[1], out _);
            }

            InventoryItem selected = showLoot ? showcase[1] : showcase[0];
            armorySelectedInventoryIndex = state.Inventory.IndexOf(selected);
            if (showLoot)
            {
                ShowLootPanel(
                    selected,
                    48,
                    2,
                    1,
                    "Added to inventory. Review the party comparison before assigning it.",
                    "Moonlit Reliquary");
                return;
            }

            armoryTab = (int)ArmoryTab.Pack;
            armoryPackFilter = 0;
            showArmory = true;
            MarkUiDirty();
            SyncArmoryOverlayScreen();
        }
    }
}
