using System;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private static bool UsesPlayerWeaponRules(CombatUnit unit)
        {
            return unit != null && unit.Side == UnitSide.Party && !unit.Summoned;
        }

        private WeaponAttackProfile MemberWeaponAttackProfile(PartyMember member)
        {
            InventoryItem item = EquippedInventoryItem(member, true);
            string name = string.IsNullOrWhiteSpace(member?.WeaponName) ? StartingWeapon(member?.Role) : member.WeaponName;
            return WeaponAttackRules.For(item, name, member?.Role, member?.ClassKey);
        }

        private WeaponAttackProfile PlayerWeaponAttackProfile(CombatUnit unit)
        {
            PartyMember member = PartyMemberForUnit(unit);
            InventoryItem item = member == null ? null : EquippedInventoryItem(member, true);
            string name = string.IsNullOrWhiteSpace(unit?.WeaponName) ? StartingWeapon(unit?.Role) : unit.WeaponName;
            return WeaponAttackRules.For(item, name, unit?.Role, unit?.ClassKey);
        }

        private bool BasicAttackRequiresSightAt(CombatUnit unit, int attackerX, int attackerY, int targetX, int targetY)
        {
            if (unit == null || unit.DemonFormTurns > 0 || Distance(attackerX, attackerY, targetX, targetY) <= 1) return false;
            return UsesPlayerWeaponRules(unit)
                ? PlayerWeaponAttackProfile(unit).RequiresSightAtDistance
                : UsesRangedAttackAt(unit, attackerX, attackerY, targetX, targetY);
        }

        private string AbilityWeaponBlockReason(CombatUnit unit, MartialAbility ability)
        {
            if (!UsesPlayerWeaponRules(unit) || ability == null || !WeaponAttackRules.RequiresMissileWeapon(ability.Id)) return "";
            return unit.DemonFormTurns <= 0 && PlayerWeaponAttackProfile(unit).Delivery == WeaponDelivery.Projectile
                ? ""
                : "Equip a bow, crossbow, sling or throwing weapon.";
        }

        private string BasicAttackDeliveryLabel(CombatUnit unit, bool missile)
        {
            if (missile) return "shot";
            if (unit?.DemonFormTurns > 0) return "claw";
            if (UsesPlayerWeaponRules(unit))
            {
                WeaponDelivery delivery = PlayerWeaponAttackProfile(unit).Delivery;
                if (delivery == WeaponDelivery.Focus) return "focus strike";
                if (delivery == WeaponDelivery.Reach) return "reach strike";
            }
            return IsRangedAttackProfile(unit) ? "melee engaged" : "melee";
        }

        private bool InventoryChangeRequiresReview(InventoryItem next, PartyMember member)
        {
            if (next == null || member == null) return false;
            bool weapon = InventoryEquipmentRules.IsWeaponSlot(next.Slot, next.Form);
            InventoryItem current = InventoryCurrentEquipment(member, weapon);
            if (InventoryEquipmentRules.RequiresDeliberateReview(next, current)) return true;
            if (!weapon) return false;
            WeaponAttackProfile before = MemberWeaponAttackProfile(member);
            WeaponAttackProfile after = WeaponAttackRules.For(next, next.DisplayName, member.Role, member.ClassKey);
            return after.Range < before.Range || after.Delivery != before.Delivery;
        }

        private string WeaponFormChangeLabel(InventoryItem next, PartyMember member, bool compact = false)
        {
            if (next == null || member == null || !InventoryEquipmentRules.IsWeaponSlot(next.Slot, next.Form)) return "";
            WeaponAttackProfile before = MemberWeaponAttackProfile(member);
            WeaponAttackProfile after = WeaponAttackRules.For(next, next.DisplayName, member.Role, member.ClassKey);
            if (after.Range == before.Range && after.Delivery == before.Delivery) return "";
            string range = after.Range == before.Range ? "Range " + after.Range : "Range " + before.Range + "→" + after.Range;
            bool ranger = string.Equals(member.ClassKey, "ranger", StringComparison.OrdinalIgnoreCase)
                || string.Equals(member.Role, "bow", StringComparison.OrdinalIgnoreCase);
            if (ranger && before.Delivery == WeaponDelivery.Projectile && after.Delivery != WeaponDelivery.Projectile)
                return range + (compact ? "; shots disabled" : "; shooting skills unavailable");
            if (ranger && before.Delivery != WeaponDelivery.Projectile && after.Delivery == WeaponDelivery.Projectile)
                return range + (compact ? "; shots enabled" : "; supports shooting skills");
            if (after.Delivery == before.Delivery) return range;
            return range + (after.Delivery == WeaponDelivery.Projectile ? "; Missile at range"
                : after.Delivery == WeaponDelivery.Focus ? "; focus uses Arms"
                : after.Delivery == WeaponDelivery.Reach ? "; reach uses Arms" : "; melee uses Arms");
        }

        // Range is derived, not an item identity or a saved combat advantage.
        // Repair after equipment identity/enchantment reconstruction; leave all
        // resources, damage, positions, turn budgets, statuses and actors intact.
        private void NormalizePartyWeaponRanges()
        {
            if (state?.Party != null)
                foreach (PartyMember member in state.Party)
                    if (member != null) member.Range = MemberWeaponAttackProfile(member).Range;
            if (state?.Combat?.Units == null) return;
            foreach (CombatUnit unit in state.Combat.Units)
                if (UsesPlayerWeaponRules(unit))
                    unit.Range = unit.DemonFormTurns > 0 ? 1 : PlayerWeaponAttackProfile(unit).Range;
        }
    }
}
