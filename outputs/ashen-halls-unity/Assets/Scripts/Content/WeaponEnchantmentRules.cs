using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public sealed class WeaponEnchantmentDefinition
    {
        public string Id { get; }
        public string Name { get; }
        public string MenuLabel { get; }
        public string EffectHint { get; }
        public string ResultLine { get; }
        public string DamageType { get; }
        public string TemporaryPrefix { get; }
        public string PermanentPrefix { get; }

        public WeaponEnchantmentDefinition(
            string id,
            string name,
            string effectHint,
            string resultLine,
            string damageType,
            string temporaryPrefix,
            string permanentPrefix)
        {
            Id = id ?? "";
            Name = name ?? "";
            MenuLabel = Name;
            EffectHint = effectHint ?? "";
            ResultLine = resultLine ?? "";
            DamageType = damageType ?? "physical";
            TemporaryPrefix = temporaryPrefix ?? "";
            PermanentPrefix = permanentPrefix ?? "";
        }
    }

    public static class WeaponEnchantmentRules
    {
        public const int TemporaryCost = 18;
        public const int PermanentCost = 90;
        public const int TemporaryVictories = 3;

        private static readonly IReadOnlyList<WeaponEnchantmentDefinition> Definitions =
            Array.AsReadOnly(new[]
            {
                new WeaponEnchantmentDefinition(
                    "fire",
                    "Fire",
                    "Fire affinity burns webbing and ignites gas.",
                    "burns with fire",
                    "fire",
                    "fiery",
                    "flamebound"),
                new WeaponEnchantmentDefinition(
                    "ice",
                    "Ice",
                    "Cold affinity quenches flame and binds bleeding foes.",
                    "bites with cold",
                    "cold",
                    "icy",
                    "frostbound"),
                new WeaponEnchantmentDefinition(
                    "storm",
                    "Storm",
                    "Shock affinity conducts through ice, gas, and webs.",
                    "crackles with shock",
                    "shock",
                    "stormcharged",
                    "stormbound"),
                new WeaponEnchantmentDefinition(
                    "radiance",
                    "Radiance",
                    "Light affinity punishes foes that are weak to holy damage.",
                    "shines with light",
                    "light",
                    "radiant",
                    "sunbound")
            });

        public static IReadOnlyList<WeaponEnchantmentDefinition> All => Definitions;

        public static WeaponEnchantmentDefinition Find(string id)
        {
            string normalized = (id ?? "").Trim();
            foreach (WeaponEnchantmentDefinition definition in Definitions)
            {
                if (string.Equals(definition.Id, normalized, StringComparison.OrdinalIgnoreCase))
                {
                    return definition;
                }
            }
            return null;
        }

        public static bool ApplyTemporary(InventoryItem item, string id)
        {
            return Apply(item, id, false);
        }

        public static bool ApplyPermanent(InventoryItem item, string id)
        {
            return Apply(item, id, true);
        }

        public static bool AdvanceAfterVictory(InventoryItem item)
        {
            if (item == null
                || Find(item.TemporaryEnchantmentId) == null
                || item.TemporaryEnchantmentVictoriesRemaining <= 0)
            {
                return false;
            }

            string before = StateSignature(item);
            item.TemporaryEnchantmentVictoriesRemaining =
                Math.Max(0, item.TemporaryEnchantmentVictoriesRemaining - 1);
            if (item.TemporaryEnchantmentVictoriesRemaining <= 0)
            {
                item.TemporaryEnchantmentId = "";
            }
            Rebuild(item);
            return !string.Equals(before, StateSignature(item), StringComparison.Ordinal);
        }

        public static void Rebuild(InventoryItem item)
        {
            if (item == null) return;

            WeaponEnchantmentDefinition permanent = Find(item.PermanentEnchantmentId);
            WeaponEnchantmentDefinition temporary =
                item.TemporaryEnchantmentVictoriesRemaining > 0
                    ? Find(item.TemporaryEnchantmentId)
                    : null;
            if (permanent == null) item.PermanentEnchantmentId = "";
            if (temporary == null)
            {
                item.TemporaryEnchantmentId = "";
                item.TemporaryEnchantmentVictoriesRemaining = 0;
            }

            if (permanent == null && temporary == null)
            {
                RestoreBase(item);
                ClearEnchantment(item);
                return;
            }

            EnsureBaseCaptured(item, permanent, temporary);
            RestoreBase(item);
            if (permanent != null)
            {
                item.DisplayName = Prefix(permanent.PermanentPrefix, item.DisplayName);
                item.Trait = Prefix(permanent.PermanentPrefix, item.Trait);
                item.DamageType = permanent.DamageType;
            }
            if (temporary != null)
            {
                item.DisplayName = Prefix(temporary.TemporaryPrefix, item.DisplayName);
                item.Trait = Prefix(temporary.TemporaryPrefix, item.Trait);
                item.DamageType = temporary.DamageType;
            }
        }

        public static string StatusText(InventoryItem item)
        {
            if (!IsEnchanted(item)) return "";
            WeaponEnchantmentDefinition permanent = Find(item.PermanentEnchantmentId);
            WeaponEnchantmentDefinition temporary =
                item.TemporaryEnchantmentVictoriesRemaining > 0
                    ? Find(item.TemporaryEnchantmentId)
                    : null;
            string permanentText = permanent == null ? "" : permanent.Name + " (permanent)";
            if (temporary == null) return permanentText;

            int victories = Math.Max(0, item.TemporaryEnchantmentVictoriesRemaining);
            string temporaryText = temporary.Name
                + " (temporary, "
                + victories
                + (victories == 1 ? " victory remaining)" : " victories remaining)");
            return string.IsNullOrEmpty(permanentText)
                ? temporaryText
                : temporaryText + " over " + permanentText;
        }

        public static bool IsEnchanted(InventoryItem item)
        {
            return item != null
                && (Find(item.PermanentEnchantmentId) != null
                    || (item.TemporaryEnchantmentVictoriesRemaining > 0
                        && Find(item.TemporaryEnchantmentId) != null));
        }

        private static bool Apply(InventoryItem item, string id, bool permanent)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (!InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form))
            {
                throw new ArgumentException("Only weapon items can be enchanted.", nameof(item));
            }

            WeaponEnchantmentDefinition definition = Find(id);
            if (definition == null)
            {
                throw new ArgumentException("Unknown weapon enchantment affinity: " + (id ?? ""), nameof(id));
            }

            string before = StateSignature(item);
            if (item.EnchantmentBaseCaptured)
            {
                RestoreBase(item);
            }
            else
            {
                CaptureBase(item);
            }

            if (permanent)
            {
                item.PermanentEnchantmentId = definition.Id;
            }
            else
            {
                item.TemporaryEnchantmentId = definition.Id;
                item.TemporaryEnchantmentVictoriesRemaining = TemporaryVictories;
            }
            Rebuild(item);
            return !string.Equals(before, StateSignature(item), StringComparison.Ordinal);
        }

        private static void CaptureBase(InventoryItem item)
        {
            item.EnchantmentBaseDisplayName = item.DisplayName ?? "";
            item.EnchantmentBaseTrait = item.Trait ?? "";
            item.EnchantmentBaseDamageType = string.IsNullOrWhiteSpace(item.DamageType)
                ? "physical"
                : item.DamageType;
            item.EnchantmentBaseCaptured = true;
        }

        private static void EnsureBaseCaptured(
            InventoryItem item,
            WeaponEnchantmentDefinition permanent,
            WeaponEnchantmentDefinition temporary)
        {
            if (item.EnchantmentBaseCaptured) return;

            string displayName = item.DisplayName;
            string trait = item.Trait;
            if (temporary != null)
            {
                displayName = RemovePrefix(displayName, temporary.TemporaryPrefix);
                trait = RemovePrefix(trait, temporary.TemporaryPrefix);
            }
            if (permanent != null)
            {
                displayName = RemovePrefix(displayName, permanent.PermanentPrefix);
                trait = RemovePrefix(trait, permanent.PermanentPrefix);
            }
            item.EnchantmentBaseDisplayName = displayName;
            item.EnchantmentBaseTrait = trait;

            string activeDamageType = temporary != null
                ? temporary.DamageType
                : permanent?.DamageType;
            item.EnchantmentBaseDamageType =
                string.Equals(item.DamageType, activeDamageType, StringComparison.OrdinalIgnoreCase)
                    ? "physical"
                    : string.IsNullOrWhiteSpace(item.DamageType) ? "physical" : item.DamageType;
            item.EnchantmentBaseCaptured = true;
        }

        private static void RestoreBase(InventoryItem item)
        {
            if (item == null || !item.EnchantmentBaseCaptured) return;
            item.DisplayName = item.EnchantmentBaseDisplayName ?? "";
            item.Trait = item.EnchantmentBaseTrait ?? "";
            item.DamageType = string.IsNullOrWhiteSpace(item.EnchantmentBaseDamageType)
                ? "physical"
                : item.EnchantmentBaseDamageType;
        }

        private static void ClearEnchantment(InventoryItem item)
        {
            if (item == null) return;
            item.PermanentEnchantmentId = "";
            item.TemporaryEnchantmentId = "";
            item.TemporaryEnchantmentVictoriesRemaining = 0;
            item.EnchantmentBaseCaptured = false;
            item.EnchantmentBaseDisplayName = "";
            item.EnchantmentBaseTrait = "";
            item.EnchantmentBaseDamageType = "";
        }

        private static string Prefix(string prefix, string value)
        {
            string normalizedPrefix = (prefix ?? "").Trim();
            string normalizedValue = (value ?? "").Trim();
            if (string.IsNullOrEmpty(normalizedPrefix)) return normalizedValue;
            return string.IsNullOrEmpty(normalizedValue)
                ? normalizedPrefix
                : normalizedPrefix + " " + normalizedValue;
        }

        private static string RemovePrefix(string value, string prefix)
        {
            string normalizedValue = (value ?? "").Trim();
            string normalizedPrefix = (prefix ?? "").Trim();
            if (string.IsNullOrEmpty(normalizedPrefix)) return normalizedValue;
            string expected = normalizedPrefix + " ";
            return normalizedValue.StartsWith(expected, StringComparison.OrdinalIgnoreCase)
                ? normalizedValue.Substring(expected.Length)
                : normalizedValue;
        }

        private static string StateSignature(InventoryItem item)
        {
            if (item == null) return "";
            return string.Join(
                "\u001f",
                item.DisplayName ?? "",
                item.Trait ?? "",
                item.DamageType ?? "",
                item.PermanentEnchantmentId ?? "",
                item.TemporaryEnchantmentId ?? "",
                item.TemporaryEnchantmentVictoriesRemaining.ToString(),
                item.EnchantmentBaseCaptured ? "1" : "0",
                item.EnchantmentBaseDisplayName ?? "",
                item.EnchantmentBaseTrait ?? "",
                item.EnchantmentBaseDamageType ?? "");
        }
    }
}
