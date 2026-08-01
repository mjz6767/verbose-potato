using UnityEngine;

namespace AshenHalls
{
    public readonly struct AttackDamageProfile
    {
        public readonly int BaseMinDamage;
        public readonly int BaseMaxDamage;
        public readonly int SkillBonus;
        public readonly int HexShift;
        public readonly int EnrageBonus;
        public readonly int FlatPowerBonus;
        public readonly int StealthBonus;
        public readonly int TargetDefense;
        public readonly int TargetArmorBonus;
        public readonly string DamageType;

        public AttackDamageProfile(
            int baseMinDamage,
            int baseMaxDamage,
            int skillBonus,
            int hexShift,
            int enrageBonus,
            int flatPowerBonus,
            int stealthBonus,
            int targetDefense,
            int targetArmorBonus,
            string damageType)
        {
            BaseMinDamage = Mathf.Max(1, baseMinDamage);
            BaseMaxDamage = Mathf.Max(BaseMinDamage, baseMaxDamage);
            SkillBonus = skillBonus;
            HexShift = hexShift;
            EnrageBonus = Mathf.Max(0, enrageBonus);
            FlatPowerBonus = Mathf.Max(0, flatPowerBonus);
            StealthBonus = Mathf.Max(0, stealthBonus);
            TargetDefense = Mathf.Max(0, targetDefense);
            TargetArmorBonus = Mathf.Max(0, targetArmorBonus);
            DamageType = string.IsNullOrWhiteSpace(damageType) ? "physical" : damageType;
        }

        public int MinRawDamage => RawDamageForBaseRoll(BaseMinDamage);
        public int MaxRawDamage => RawDamageForBaseRoll(BaseMaxDamage);

        public int RawDamageForBaseRoll(int baseRoll)
        {
            return Mathf.Max(1, baseRoll + SkillBonus + HexShift + EnrageBonus + FlatPowerBonus + StealthBonus - TargetDefense - TargetArmorBonus);
        }
    }

    public static class AttackRules
    {
        public static AttackDamageProfile BuildDamageProfile(CombatUnit attacker, CombatUnit target, int skillValue, int warriorEnrageBonus, int flatPowerBonus = 0)
        {
            if (attacker == null)
            {
                return new AttackDamageProfile(1, 1, 0, 0, 0, 0, 0, 0, 0, "physical");
            }

            int minDamage = attacker.DamageMin > 0 ? attacker.DamageMin : Mathf.Max(1, attacker.Power - 2);
            int maxDamage = attacker.DamageMax > 0 ? attacker.DamageMax : Mathf.Max(minDamage + 1, attacker.Power + 4);
            string damageType = string.IsNullOrWhiteSpace(attacker.DamageType) ? "physical" : attacker.DamageType;
            int enrageBonus = damageType == "physical" ? warriorEnrageBonus : 0;
            int hexShift = (target != null && target.Hexed > 0 ? 2 : 0) - (attacker.Hexed > 0 ? 2 : 0);
            int stealthBonus = attacker.Stealthed > 0 ? 3 : 0;

            return new AttackDamageProfile(
                minDamage,
                maxDamage,
                Mathf.Max(0, skillValue) / 5,
                hexShift,
                enrageBonus,
                flatPowerBonus,
                stealthBonus,
                target?.Defense ?? 0,
                target?.ArmorBonus ?? 0,
                damageType);
        }
    }
}
