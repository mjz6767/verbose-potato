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

        private readonly struct SwapWeaponLoss
        {
            public readonly PartyMember Member;
            public readonly int Before;
            public readonly int After;
            public readonly bool Shots;
            public bool Any => Member != null && (Shots || After < Before);

            public SwapWeaponLoss(PartyMember member, int before, int after, bool shots)
            {
                Member = member; Before = before; After = after; Shots = shots;
            }
        }

        private SwapWeaponLoss WeaponSwapLoss(InventoryItem next, PartyMember member)
        {
            if (next == null || member == null || !InventoryEquipmentRules.IsWeaponSlot(next.Slot, next.Form)) return default;
            WeaponAttackProfile before = MemberWeaponAttackProfile(member);
            WeaponAttackProfile after = WeaponAttackRules.For(next, next.DisplayName, member.Role, member.ClassKey);
            bool ranger = string.Equals(member.ClassKey, "ranger", StringComparison.OrdinalIgnoreCase)
                || string.Equals(member.Role, "bow", StringComparison.OrdinalIgnoreCase);
            return new SwapWeaponLoss(member, before.Range, after.Range,
                ranger && before.Delivery == WeaponDelivery.Projectile && after.Delivery != WeaponDelivery.Projectile);
        }

        private string CompactSwapWeaponLoss(InventoryItem item, PartyMember recipient, InventoryItem replacement, PartyMember owner)
        {
            SwapWeaponLoss first = WeaponSwapLoss(item, recipient);
            SwapWeaponLoss second = WeaponSwapLoss(replacement, owner);
            if (!first.Any && !second.Any) return "";
            // Keep the most consequential loss first, independent of who owns
            // the selected item. Preserve both losses when the swap reduces both.
            if (!first.Any || second.Any && ((second.Shots && !first.Shots)
                || second.Shots == first.Shots && second.Before - second.After > first.Before - first.After))
            {
                SwapWeaponLoss previous = first; first = second; second = previous;
            }
            if (!second.Any)
            {
                string name = CompactSwapMemberName(first.Member, 6);
                return first.Shots ? $"{name}: {first.Before}→{first.After}; shots disabled"
                    : $"{name}: range {first.Before}→{first.After}";
            }

            string firstName = CompactSwapMemberName(first.Member, 4);
            string secondName = CompactSwapMemberName(second.Member, 4);
            if (string.Equals(firstName, secondName, StringComparison.OrdinalIgnoreCase))
            {
                // Custom names can share their shortened prefix. Party positions
                // distinguish the two without hiding either numerical consequence.
                firstName = "Hero " + (state.Party.IndexOf(first.Member) + 1);
                secondName = "Hero " + (state.Party.IndexOf(second.Member) + 1);
            }
            string shotLoss = first.Shots ? " no shots" : "";
            return first.Shots ? $"{firstName} {first.Before}→{first.After}{shotLoss}; {secondName} {second.Before}→{second.After}"
                : $"Range {firstName} {first.Before}→{first.After}; {secondName} {second.Before}→{second.After}";
        }

        private static string CompactSwapMemberName(PartyMember member, int limit)
        {
            string name = (member?.Name ?? "").Trim();
            if (name.Length == 0) return "Hero";
            return name.Length <= limit ? name : name.Substring(0, limit - 1) + "…";
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
