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

        private void EnsureInventoryEquipmentLinks()
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
                && string.Equals(item.DisplayName, equippedName, StringComparison.Ordinal));
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
            if (member == null || item == null) return false;
            bool weapon = InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form);
            string equippedName = weapon ? member.WeaponName : member.ArmorName;
            return string.Equals(equippedName, item.DisplayName, StringComparison.Ordinal);
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

            EnsureInventoryEquipmentLinks();
            PartyMember currentOwner = EquippedMember(item);
            if (currentOwner != null && currentOwner != target)
            {
                result = $"{currentOwner.Name} is already using {item.DisplayName}. Equip something else there before moving it.";
                return false;
            }
            if (currentOwner == target)
            {
                result = $"{target.Name} already has {item.DisplayName} equipped.";
                return false;
            }

            bool weapon = InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form);
            InventoryItem previousItem = EquippedInventoryItem(target, weapon);
            if (previousItem != null && previousItem != item) previousItem.EquippedById = "";
            string old = weapon
                ? string.IsNullOrWhiteSpace(target.WeaponName) ? "their previous weapon" : target.WeaponName
                : string.IsNullOrWhiteSpace(target.ArmorName) ? "their previous armor" : target.ArmorName;

            if (weapon)
            {
                target.WeaponName = item.DisplayName;
                target.WeaponBonus = item.Bonus;
                target.WeaponDamageType = string.IsNullOrEmpty(item.DamageType) ? "physical" : item.DamageType;
                target.WeaponDamageMin = Mathf.Max(1, item.DamageMin);
                target.WeaponDamageMax = Mathf.Max(target.WeaponDamageMin + 1, item.DamageMax);
                target.WeaponAttackSpeed = Mathf.Max(1, item.AttackSpeed);
                ApplyGearStatBonuses(target, item, true);
                target.Range = WeaponRange(item, target);
            }
            else
            {
                target.ArmorName = item.DisplayName;
                target.ArmorBonus = ArmorDefenseBonus(item);
                ApplyGearStatBonuses(target, item, false);
            }

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
            bool weapon = InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form);
            if (weapon)
            {
                int newDamage = Mathf.Max(1, item.DamageMin) + Mathf.Max(Mathf.Max(1, item.DamageMin) + 1, item.DamageMax);
                int oldMin = Mathf.Max(1, member.WeaponDamageMin > 0 ? member.WeaponDamageMin : member.DamageMin);
                int oldMax = Mathf.Max(oldMin + 1, member.WeaponDamageMax > 0 ? member.WeaponDamageMax : member.DamageMax);
                int newStats = item.StrengthBonus + item.IntelligenceBonus + item.AgilityBonus + item.HealthBonus;
                int oldStats = member.WeaponStrengthBonus + member.WeaponIntelligenceBonus + member.WeaponAgilityBonus + member.WeaponHealthBonus;
                int newScore = item.Bonus * 10
                    + newDamage * 3
                    + Mathf.Max(1, item.AttackSpeed)
                    + WeaponRange(item, member) * 2
                    + WeaponRoleFit(item, member) * 3
                    + newStats * 4;
                int oldScore = member.WeaponBonus * 10
                    + (oldMin + oldMax) * 3
                    + Mathf.Max(1, member.WeaponAttackSpeed > 0 ? member.WeaponAttackSpeed : member.AttackSpeed)
                    + Mathf.Max(1, member.Range) * 2
                    + oldStats * 4;
                return newScore - oldScore;
            }

            int newArmorStats = item.StrengthBonus + item.IntelligenceBonus + item.AgilityBonus + item.HealthBonus;
            int oldArmorStats = member.ArmorStrengthBonus + member.ArmorIntelligenceBonus + member.ArmorAgilityBonus + member.ArmorHealthBonus;
            int newArmorScore = ArmorDefenseBonus(item) * 14
                + ArmorAgilityModifier(item.DisplayName) * 3
                + newArmorStats * 4
                - ArmorRolePenalty(item, member) * 2;
            int oldArmorScore = member.ArmorBonus * 14
                + ArmorAgilityModifier(member.ArmorName) * 3
                + oldArmorStats * 4;
            return newArmorScore - oldArmorScore;
        }

        private PartyMember BestInventoryFit(InventoryItem item, out int partyIndex, out int comparisonScore)
        {
            partyIndex = -1;
            comparisonScore = int.MinValue / 4;
            if (item == null || state?.Party == null) return null;
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

        private int BestInventoryComparisonScore(InventoryItem item)
        {
            return BestInventoryFit(item, out _, out int score) == null ? int.MinValue / 4 : score;
        }

        private InventoryUpgradeGrade InventoryGradeFor(InventoryItem item, PartyMember member)
        {
            if (item == null || member == null) return InventoryUpgradeGrade.Sidegrade;
            if (EquippedMember(item) == member) return InventoryUpgradeGrade.Sidegrade;
            return InventoryEquipmentRules.Grade(InventoryComparisonScore(item, member));
        }

        private bool InventoryItemIsUpgrade(InventoryItem item)
        {
            PartyMember best = BestInventoryFit(item, out _, out int score);
            return best != null && InventoryEquipmentRules.Grade(score) == InventoryUpgradeGrade.Upgrade;
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
                int newRange = WeaponRange(item, member);
                return $"{newMin}-{newMax} dmg vs {oldMin}-{oldMax} / "
                    + ComparisonToken("SPD", newSpeed, oldSpeed) + " / "
                    + ComparisonToken("RNG", newRange, oldRange);
            }

            int newArmor = ArmorDefenseBonus(item);
            int oldArmor = member.ArmorBonus;
            int newAgility = ArmorAgilityModifier(item.DisplayName);
            int oldAgility = ArmorAgilityModifier(member.ArmorName);
            return ComparisonToken("ARM", newArmor, oldArmor) + " / "
                + ComparisonToken("AGI", newAgility, oldAgility);
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
            PartyMember best = BestInventoryFit(item, out _, out int score);
            if (best == null) return "Stored in the inventory";
            return $"{InventoryEquipmentRules.GradeLabel(InventoryEquipmentRules.Grade(score))} for {best.Name}";
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
