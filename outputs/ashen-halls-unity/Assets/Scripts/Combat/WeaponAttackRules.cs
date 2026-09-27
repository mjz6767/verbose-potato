using System;

namespace AshenHalls
{
    public enum WeaponDelivery
    {
        Melee,
        Reach,
        Projectile,
        Focus
    }

    public readonly struct WeaponAttackProfile
    {
        public readonly int Range;
        public readonly WeaponDelivery Delivery;
        public bool UsesMissileAtDistance => Delivery == WeaponDelivery.Projectile;
        public bool RequiresSightAtDistance => Range > 1;
        public bool CollapsesWhenEngaged => Delivery == WeaponDelivery.Projectile;

        public WeaponAttackProfile(int range, WeaponDelivery delivery)
        {
            Range = Math.Max(1, range);
            Delivery = delivery;
        }
    }

    // Class training may improve a compatible implement. It never turns an
    // ordinary blade into a projectile or supplies a cached minimum range.
    public static class WeaponAttackRules
    {
        public static WeaponAttackProfile For(InventoryItem item, string weaponName, string role, string classKey)
        {
            string form = item?.Form;
            if (!TryForm(form, out int range, out WeaponDelivery delivery))
            {
                SignatureItemDefinition signature = SignatureItemCatalog.Identify(item)
                    ?? SignatureItemCatalog.Identify(weaponName);
                if (!TryForm(signature?.Form, out range, out delivery)
                    && !TryForm(item?.DisplayName, out range, out delivery)
                    && !TryForm(weaponName, out range, out delivery))
                {
                    range = 1;
                    delivery = WeaponDelivery.Melee;
                }
            }

            if (delivery == WeaponDelivery.Projectile && IsBowTrained(role, classKey)) range++;
            if (delivery == WeaponDelivery.Focus && !CanChannelFocus(role, classKey)) range = 1;
            return new WeaponAttackProfile(range, delivery);
        }

        public static bool RequiresMissileWeapon(string abilityId)
        {
            switch ((abilityId ?? "").Trim().ToLowerInvariant())
            {
                case "aimedshot":
                case "pinningshot":
                case "volley":
                case "broadheadshot":
                case "disruptingshot":
                case "quickshot": return true;
                default: return false;
            }
        }

        private static bool IsBowTrained(string role, string classKey)
        {
            return Same(role, "bow") || Same(classKey, "ranger");
        }

        private static bool CanChannelFocus(string role, string classKey)
        {
            return Same(role, "ember") || Same(role, "hex") || Same(role, "mender")
                || !string.IsNullOrEmpty(StarterPartyCatalog.SpellSchoolForClass(classKey));
        }

        private static bool TryForm(string value, out int range, out WeaponDelivery delivery)
        {
            string form = (value ?? "").Trim().ToLowerInvariant();
            range = 1;
            delivery = WeaponDelivery.Melee;
            if (form.Length == 0) return false;
            // Thrown knives are projectiles; ritual knives remain blades.
            if (Has(form, "throwing", "thrown", "sling", "dart"))
            {
                range = 3;
                delivery = WeaponDelivery.Projectile;
                return true;
            }
            if (Has(form, "bow"))
            {
                range = 4;
                delivery = WeaponDelivery.Projectile;
                return true;
            }
            if (Has(form, "spear", "pike", "glaive", "halberd") || HasWord(form, "lance"))
            {
                range = 2;
                delivery = WeaponDelivery.Reach;
                return true;
            }
            if (Has(form, "sword", "blade", "knife", "knives", "dagger", "epee", "rapier", "sabre",
                "saber", "axe", "mace", "hammer", "flail", "maul", "club", "bell", "claw", "fist")) return true;
            if (Has(form, "focus", "staff", "orb", "scepter", "sceptre"))
            {
                range = 4;
                delivery = WeaponDelivery.Focus;
                return true;
            }
            return false;
        }

        private static bool HasWord(string value, string word)
        {
            for (int index = value.IndexOf(word, StringComparison.Ordinal); index >= 0;
                index = value.IndexOf(word, index + word.Length, StringComparison.Ordinal))
            {
                int end = index + word.Length;
                if ((index == 0 || !char.IsLetter(value[index - 1]))
                    && (end == value.Length || !char.IsLetter(value[end]))) return true;
            }
            return false;
        }

        private static bool Has(string value, params string[] fragments)
        {
            foreach (string fragment in fragments)
                if (value.IndexOf(fragment, StringComparison.Ordinal) >= 0) return true;
            return false;
        }

        private static bool Same(string value, string expected)
        {
            return string.Equals((value ?? "").Trim(), expected, StringComparison.OrdinalIgnoreCase);
        }
    }
}
