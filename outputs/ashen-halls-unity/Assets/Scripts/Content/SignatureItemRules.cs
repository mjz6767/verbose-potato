using System;

namespace AshenHalls
{
    public static class SignatureItemRules
    {
        public const float ConductionStunChance = 0.30f;

        public static int GuardActionBonus(InventoryItem weapon)
        {
            return Is(weapon, SignatureItemCatalog.SluicekeeperBladeId) ? 1 : 0;
        }

        public static string BasicHitStatus(InventoryItem weapon)
        {
            return Is(weapon, SignatureItemCatalog.StormglassConductorId) ? "stun" : "";
        }

        public static float BasicHitStatusChance(InventoryItem weapon)
        {
            return Is(weapon, SignatureItemCatalog.StormglassConductorId)
                ? ConductionStunChance
                : 0f;
        }

        public static int DamageReduction(InventoryItem armor, string damageType)
        {
            string type = string.IsNullOrWhiteSpace(damageType)
                ? "physical"
                : damageType.Trim();
            if (Is(armor, SignatureItemCatalog.RatcatcherRoadcoatId))
            {
                return Same(type, "poison") ? 1 : 0;
            }
            if (Is(armor, SignatureItemCatalog.GloamReliquaryMailId))
            {
                return Same(type, "death") || Same(type, "mind") ? 1 : 0;
            }
            if (Is(armor, SignatureItemCatalog.MirrorweaveRoadMantleId))
            {
                return Same(type, "physical") ? 0 : 1;
            }
            return 0;
        }

        public static int WeaponHitBonus(InventoryItem weapon)
        {
            return Is(weapon, SignatureItemCatalog.UnfathomableSwordId) ? 3 : 0;
        }

        public static int WeaponPowerBonus(InventoryItem weapon)
        {
            return Is(weapon, SignatureItemCatalog.UnfathomableSwordId) ? 2 : 0;
        }

        public static int LifeDrainAmount(InventoryItem weapon, int dealtDamage)
        {
            if (!Is(weapon, SignatureItemCatalog.UnfathomableSwordId) || dealtDamage <= 0)
            {
                return 0;
            }
            return Math.Min(3, Math.Max(1, 1 + dealtDamage / 8));
        }

        public static int WardTurnsRemovedOnBasicHit(
            InventoryItem weapon,
            int currentWardTurns,
            bool successfulHit)
        {
            return successfulHit
                && currentWardTurns > 0
                && Is(weapon, SignatureItemCatalog.CrownwardWarbladeId)
                    ? 1
                    : 0;
        }

        public static int WardTurnsRemovedOnSuccessfulBasicHit(
            InventoryItem weapon,
            int currentWardTurns)
        {
            return WardTurnsRemovedOnBasicHit(weapon, currentWardTurns, true);
        }

        public static bool HasIntrinsic(InventoryItem item, string signatureId)
        {
            return Is(item, signatureId);
        }

        private static bool Is(InventoryItem item, string signatureId)
        {
            SignatureItemDefinition definition = SignatureItemCatalog.Identify(item);
            return definition != null && Same(definition.Id, signatureId);
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
