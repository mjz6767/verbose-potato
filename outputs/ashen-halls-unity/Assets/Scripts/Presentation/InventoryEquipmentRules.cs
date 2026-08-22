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
            if (IsExplicitNonEquipmentSlot(normalizedSlot)) return false;
            if (normalizedSlot == "weapon") return true;
            if (normalizedSlot.Contains("armor")) return false;

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
            if (IsExplicitNonEquipmentSlot(normalizedSlot)) return false;
            if (normalizedSlot.Contains("weapon") || normalizedSlot.Contains("focus")) return false;
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

        public static bool MatchesWeaponLoadout(InventoryItem item, PartyMember member, int effectiveRange)
        {
            if (item == null || member == null || !IsWeaponSlot(item.Slot, item.Form)) return false;
            if (!string.Equals(item.DisplayName, member.WeaponName, StringComparison.Ordinal)) return false;

            int itemMin = Math.Max(1, item.DamageMin);
            int itemMax = Math.Max(itemMin + 1, item.DamageMax);
            int memberMin = Math.Max(1, member.WeaponDamageMin > 0 ? member.WeaponDamageMin : member.DamageMin);
            int memberMax = Math.Max(memberMin + 1, member.WeaponDamageMax > 0 ? member.WeaponDamageMax : member.DamageMax);
            int itemSpeed = Math.Max(1, item.AttackSpeed);
            int memberSpeed = Math.Max(1, member.WeaponAttackSpeed > 0 ? member.WeaponAttackSpeed : member.AttackSpeed);
            string itemDamageType = string.IsNullOrWhiteSpace(item.DamageType) ? "physical" : item.DamageType.Trim();
            string memberDamageType = string.IsNullOrWhiteSpace(member.WeaponDamageType) ? "physical" : member.WeaponDamageType.Trim();
            return item.Bonus == member.WeaponBonus
                && itemMin == memberMin
                && itemMax == memberMax
                && itemSpeed == memberSpeed
                && Math.Max(1, effectiveRange) == Math.Max(1, member.Range)
                && string.Equals(itemDamageType, memberDamageType, StringComparison.OrdinalIgnoreCase)
                && item.StrengthBonus == member.WeaponStrengthBonus
                && item.IntelligenceBonus == member.WeaponIntelligenceBonus
                && item.AgilityBonus == member.WeaponAgilityBonus
                && item.HealthBonus == member.WeaponHealthBonus;
        }

        public static bool MatchesArmorLoadout(InventoryItem item, PartyMember member, int effectiveDefense)
        {
            return item != null
                && member != null
                && IsArmorSlot(item.Slot, item.Form)
                && string.Equals(item.DisplayName, member.ArmorName, StringComparison.Ordinal)
                && effectiveDefense == member.ArmorBonus
                && item.StrengthBonus == member.ArmorStrengthBonus
                && item.IntelligenceBonus == member.ArmorIntelligenceBonus
                && item.AgilityBonus == member.ArmorAgilityBonus
                && item.HealthBonus == member.ArmorHealthBonus;
        }

        public static bool HasStrategicIdentity(InventoryItem item)
        {
            return item != null
                && (SignatureItemCatalog.Identify(item) != null
                    || HasActiveEnchantment(item)
                    || TacticalBehaviorIdentity(item).Length > 0);
        }

        public static bool RequiresDeliberateReview(InventoryItem next, InventoryItem current)
        {
            if (next == null) return false;

            string nextSignature = SignatureItemCatalog.Identify(next)?.Id ?? "";
            string currentSignature = SignatureItemCatalog.Identify(current)?.Id ?? "";
            bool signatureChanges = (nextSignature.Length > 0 || currentSignature.Length > 0)
                && !Same(nextSignature, currentSignature);
            bool enchantmentChanges = (HasActiveEnchantment(next) || HasActiveEnchantment(current))
                && !Same(EnchantmentIdentity(next), EnchantmentIdentity(current));
            string nextBehavior = TacticalBehaviorIdentity(next);
            string currentBehavior = TacticalBehaviorIdentity(current);
            bool behaviorChanges = (nextBehavior.Length > 0 || currentBehavior.Length > 0)
                && !Same(nextBehavior, currentBehavior);
            return signatureChanges || enchantmentChanges || behaviorChanges;
        }

        public static string StrategicChangeLabel(InventoryItem next, InventoryItem current)
        {
            if (!RequiresDeliberateReview(next, current)) return "";

            SignatureItemDefinition nextSignature = SignatureItemCatalog.Identify(next);
            SignatureItemDefinition currentSignature = SignatureItemCatalog.Identify(current);
            string signatureChange = "";
            if (nextSignature != null && currentSignature == null)
            {
                signatureChange = "Gain " + IntrinsicLabel(nextSignature);
            }
            else if (nextSignature == null && currentSignature != null)
            {
                signatureChange = "Lose " + IntrinsicLabel(currentSignature);
            }
            else if (nextSignature != null && currentSignature != null
                && !Same(nextSignature.Id, currentSignature.Id))
            {
                signatureChange = IntrinsicLabel(nextSignature) + " replaces " + IntrinsicLabel(currentSignature);
            }

            bool nextEnchanted = HasActiveEnchantment(next);
            bool currentEnchanted = HasActiveEnchantment(current);
            string enchantmentChange = "";
            if (!Same(EnchantmentIdentity(next), EnchantmentIdentity(current)))
            {
                enchantmentChange = nextEnchanted && !currentEnchanted
                    ? "Gain enchantment"
                    : !nextEnchanted && currentEnchanted
                        ? "Lose enchantment"
                        : "Enchantment changes";
            }

            string behaviorChange = TacticalBehaviorChangeLabel(next, current);
            return JoinChanges(signatureChange, enchantmentChange, behaviorChange);
        }

        public static int ReassignmentScore(int selectedRecipientDelta, int displacedRecipientDelta)
        {
            // A swap is only an upgrade when neither recipient is sacrificed.
            // The weaker side therefore owns the recommendation grade.
            return Math.Min(selectedRecipientDelta, displacedRecipientDelta);
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

        private static bool IsExplicitNonEquipmentSlot(string normalizedSlot)
        {
            return normalizedSlot == "quest"
                || normalizedSlot == "material"
                || normalizedSlot == "consumable"
                || normalizedSlot == "key"
                || normalizedSlot == "trophy";
        }

        private static bool HasActiveEnchantment(InventoryItem item)
        {
            return item != null
                && (!string.IsNullOrWhiteSpace(item.PermanentEnchantmentId)
                    || !string.IsNullOrWhiteSpace(item.TemporaryEnchantmentId));
        }

        private static string EnchantmentIdentity(InventoryItem item)
        {
            if (item == null) return "";
            return (item.PermanentEnchantmentId ?? "").Trim().ToLowerInvariant()
                + "|" + (item.TemporaryEnchantmentId ?? "").Trim().ToLowerInvariant()
                + "|" + Math.Max(0, item.TemporaryEnchantmentVictoriesRemaining);
        }

        private static string IntrinsicLabel(SignatureItemDefinition definition)
        {
            if (definition == null) return "intrinsic";
            return string.IsNullOrWhiteSpace(definition.IntrinsicName)
                ? "signature intrinsic"
                : definition.IntrinsicName.Trim();
        }

        private static string TacticalBehaviorChangeLabel(InventoryItem next, InventoryItem current)
        {
            string nextIdentity = TacticalBehaviorIdentity(next);
            string currentIdentity = TacticalBehaviorIdentity(current);
            if (Same(nextIdentity, currentIdentity)) return "";

            string gained = TacticalBehaviorDifferenceLabel(nextIdentity, currentIdentity);
            string lost = TacticalBehaviorDifferenceLabel(currentIdentity, nextIdentity);
            if (lost.Length == 0) return "Gain " + gained;
            if (gained.Length == 0) return "Lose " + lost;
            return gained + " replaces " + lost;
        }

        private static string TacticalBehaviorIdentity(InventoryItem item)
        {
            if (item == null
                || !IsEquippable(item)
                || SignatureItemCatalog.Identify(item) != null)
            {
                return "";
            }

            // Live combat equips DisplayName into WeaponName/ArmorName and evaluates those names.
            // Keep review identity on that same boundary so metadata cannot advertise inactive effects.
            string text = (item.DisplayName ?? "").Trim().ToLowerInvariant();
            string identity = "";
            if (IsWeaponSlot(item.Slot, item.Form))
            {
                string damageType = string.IsNullOrWhiteSpace(item.DamageType)
                    ? "physical"
                    : item.DamageType.Trim().ToLowerInvariant();
                if (damageType != "physical") identity = AppendToken(identity, "affinity:" + damageType);
                if (ContainsAny(text, "stunning", "storm", "war hammer")) identity = AppendToken(identity, "stun");
                else if (ContainsAny(text, "bleeding", "vicious", "epee", "sabre", "thorns")) identity = AppendToken(identity, "bleed");
                else if (text.Contains("venom")) identity = AppendToken(identity, "poison");
                else if (ContainsAny(text, "terror", "silence")) identity = AppendToken(identity, "sleep");
                if (text.Contains("vampiric")) identity = AppendToken(identity, "life-drain");
                if (text.Contains("ward shield")) identity = AppendToken(identity, "guard:1");
                return identity;
            }

            int guardBonus = 0;
            if (ContainsAny(text, "tower shield", "kite shield")) guardBonus += 2;
            if (text.Contains("buckler")) guardBonus++;
            if (ContainsAny(text, "warding", "guarding", "anti-magic")) guardBonus++;
            if (guardBonus > 0) identity = AppendToken(identity, "guard:" + guardBonus);
            if (text.Contains("robe")) identity = AppendToken(identity, "caster-guard");

            int physicalReduction = 0;
            if (ContainsAny(text, "plate", "tower shield")) physicalReduction++;
            if (text.Contains("thorns")) physicalReduction++;
            if (physicalReduction > 0) identity = AppendToken(identity, "physical-reduction:" + physicalReduction);
            if (ContainsAny(text, "warding", "anti-magic", "moonstone"))
            {
                identity = AppendToken(identity, "nonphysical-reduction:1");
            }
            return identity;
        }

        private static string TacticalBehaviorDifferenceLabel(string sourceIdentity, string comparisonIdentity)
        {
            if (sourceIdentity.Length == 0) return "";
            string result = "";
            foreach (string token in sourceIdentity.Split('|'))
            {
                if (IdentityContainsToken(comparisonIdentity, token)) continue;
                string label = TacticalBehaviorTokenLabel(token);
                result = result.Length == 0 ? label : result + ", " + label;
            }
            return result;
        }

        private static bool IdentityContainsToken(string identity, string token)
        {
            if (identity.Length == 0 || token.Length == 0) return false;
            foreach (string candidate in identity.Split('|'))
            {
                if (Same(candidate, token)) return true;
            }
            return false;
        }

        private static string TacticalBehaviorTokenLabel(string token)
        {
            if (token.StartsWith("affinity:", StringComparison.Ordinal))
            {
                return token.Substring("affinity:".Length) + " affinity";
            }
            if (token.StartsWith("guard:", StringComparison.Ordinal))
            {
                return "guard +" + token.Substring("guard:".Length);
            }
            if (token.StartsWith("physical-reduction:", StringComparison.Ordinal))
            {
                return "physical reduction +" + token.Substring("physical-reduction:".Length);
            }
            if (token.StartsWith("nonphysical-reduction:", StringComparison.Ordinal))
            {
                return "nonphysical reduction +" + token.Substring("nonphysical-reduction:".Length);
            }
            return token
                .Replace("life-drain", "life drain")
                .Replace("caster-guard", "caster guard");
        }

        private static string AppendToken(string current, string token)
        {
            return current.Length == 0 ? token : current + "|" + token;
        }

        private static bool ContainsAny(string text, params string[] values)
        {
            foreach (string value in values)
            {
                if (text.Contains(value)) return true;
            }
            return false;
        }

        private static string JoinChanges(params string[] changes)
        {
            string result = "";
            foreach (string change in changes)
            {
                if (string.IsNullOrWhiteSpace(change)) continue;
                result = result.Length == 0 ? change : result + "; " + change;
            }
            return result;
        }

        private static bool Same(string left, string right)
        {
            return string.Equals(
                (left ?? "").Trim(),
                (right ?? "").Trim(),
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
