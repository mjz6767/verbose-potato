using System;

namespace AshenHalls
{
    public enum InventoryUpgradeGrade
    {
        Downgrade = -1,
        Sidegrade = 0,
        Upgrade = 1
    }

    public static class InventoryEquipmentRules
    {
        public static bool IsWeaponSlot(string slot, string form)
        {
            string normalizedSlot = (slot ?? "").Trim().ToLowerInvariant();
            if (normalizedSlot == "weapon") return true;
            if (normalizedSlot == "armor") return false;

            string normalizedForm = (form ?? "").Trim().ToLowerInvariant();
            return normalizedSlot.Contains("weapon")
                || normalizedSlot.Contains("focus")
                || normalizedForm.Contains("sword")
                || normalizedForm.Contains("blade")
                || normalizedForm.Contains("mace")
                || normalizedForm.Contains("hammer")
                || normalizedForm.Contains("flail")
                || normalizedForm.Contains("spear")
                || normalizedForm.Contains("pike")
                || normalizedForm.Contains("glaive")
                || normalizedForm.Contains("halberd")
                || normalizedForm.Contains("bow")
                || normalizedForm.Contains("sling")
                || normalizedForm.Contains("dart")
                || normalizedForm.Contains("knife")
                || normalizedForm.Contains("epee")
                || normalizedForm.Contains("sabre")
                || normalizedForm.Contains("staff")
                || normalizedForm.Contains("focus")
                || normalizedForm.Contains("orb")
                || normalizedForm.Contains("scepter");
        }

        public static bool IsArmorSlot(string slot, string form)
        {
            string normalizedSlot = (slot ?? "").Trim().ToLowerInvariant();
            if (normalizedSlot == "armor" || normalizedSlot.Contains("armor")) return true;

            string normalizedForm = (form ?? "").Trim().ToLowerInvariant();
            return normalizedForm.Contains("armor")
                || normalizedForm.Contains("cuirass")
                || normalizedForm.Contains("hauberk")
                || normalizedForm.Contains("brigandine")
                || normalizedForm.Contains("mail")
                || normalizedForm.Contains("plate");
        }

        public static bool IsEquippable(InventoryItem item)
        {
            return item != null && (IsWeaponSlot(item.Slot, item.Form) || IsArmorSlot(item.Slot, item.Form));
        }

        public static int RarityRank(string rarity)
        {
            switch ((rarity ?? "").Trim().ToLowerInvariant())
            {
                case "starter": return 0;
                case "common": return 1;
                case "scaffold": return 1;
                case "uncommon": return 2;
                case "fine": return 2;
                case "rare": return 3;
                case "epic": return 4;
                case "legendary": return 5;
                case "relic": return 6;
                default: return 1;
            }
        }

        public static string RarityLabel(string rarity)
        {
            string normalized = string.IsNullOrWhiteSpace(rarity) ? "common" : rarity.Trim();
            return char.ToUpperInvariant(normalized[0]) + normalized.Substring(1).ToLowerInvariant();
        }

        public static string SlotLabel(string slot, string form)
        {
            if (IsWeaponSlot(slot, form)) return "Weapon";
            if (IsArmorSlot(slot, form)) return "Armor";
            return string.Equals((slot ?? "").Trim(), "quest", StringComparison.OrdinalIgnoreCase)
                ? "Quest item"
                : "Item";
        }

        public static InventoryUpgradeGrade Grade(int scoreDelta, int sidegradeWindow = 3)
        {
            int window = Math.Max(0, sidegradeWindow);
            if (scoreDelta > window) return InventoryUpgradeGrade.Upgrade;
            if (scoreDelta < -window) return InventoryUpgradeGrade.Downgrade;
            return InventoryUpgradeGrade.Sidegrade;
        }

        public static string GradeLabel(InventoryUpgradeGrade grade)
        {
            switch (grade)
            {
                case InventoryUpgradeGrade.Upgrade: return "Upgrade";
                case InventoryUpgradeGrade.Downgrade: return "Tradeoff";
                default: return "Sidegrade";
            }
        }

        public static string SignedDelta(int value)
        {
            if (value > 0) return "+" + value;
            return value.ToString();
        }

        public static bool MatchesFilter(InventoryItem item, int filter, bool isUpgrade)
        {
            if (item == null) return false;
            if (filter == 1) return IsWeaponSlot(item.Slot, item.Form);
            if (filter == 2) return IsArmorSlot(item.Slot, item.Form);
            if (filter == 3) return IsEquippable(item) && isUpgrade;
            return true;
        }

        public static int SortScore(InventoryItem item, bool equipped, int bestComparisonScore)
        {
            if (item == null) return int.MinValue;
            int score = RarityRank(item.Rarity) * 1000;
            score += Math.Max(-20, Math.Min(20, item.Bonus)) * 25;
            score += Math.Max(-200, Math.Min(200, bestComparisonScore));
            if (bestComparisonScore > 3) score += 5000;
            if (equipped) score -= 10000;
            if (!IsEquippable(item)) score -= 20000;
            return score;
        }
    }
}
