using System.Linq;
using UnityEngine;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        // The normal enemy turn owns action use and the chance roll. These
        // focused boss actions resolve once and return to that same turn flow.
        private bool TryMeteorCrownBossSpecial(CombatUnit enemy, CombatUnit target)
        {
            if (state?.Combat == null || enemy == null || target == null
                || enemy.Side != UnitSide.Enemy || target.Side != UnitSide.Party
                || enemy.Hp <= 0 || target.Hp <= 0
                || enemy.Role != "meteorlich" && enemy.Role != "ritualheart") return false;
            if (Distance(enemy.X, enemy.Y, target.X, target.Y) > enemy.Range
                || !HasLineOfSight(enemy.X, enemy.Y, target.X, target.Y, true)) return false;

            if (enemy.Role == "meteorlich")
            {
                int damage = Mathf.Clamp(enemy.Power - 4, 8, 18);
                EnemyPowerBeat beat = BeginEnemyPowerBeat(enemy, target, "crownfire", target.X, target.Y, ember);
                AddEnemySpecialBeam(enemy, target, beat.ExactPowerKey, ember, "fireball");
                AddTileGlyph(target.X, target.Y, null, "fireball", ember);
                foreach (CombatUnit unit in state.Combat.Units.Where(candidate => candidate != null
                    && candidate.Side == UnitSide.Party && candidate.Hp > 0
                    && Distance(candidate.X, candidate.Y, target.X, target.Y) <= 1
                    && HasLineOfSight(target.X, target.Y, candidate.X, candidate.Y, true)).ToList())
                {
                    DealDamage(unit, unit.Id == target.Id ? damage : Mathf.Max(2, damage / 3), "fire", ember);
                    if (unit.Hp <= 0) ReportUnitDown(unit);
                }
                PushLog(enemy.Name + " casts Crownfire. Its smaller sparks catch adjacent allies; spread out or break his sight line.", Tone.Warn);
                CompleteEnemyPowerBeat(beat);
                return true;
            }

            EnemyPowerBeat pulse = BeginEnemyPowerBeat(enemy, target, "heartpulse", target.X, target.Y, blood);
            AddEnemySpecialBeam(enemy, target, pulse.ExactPowerKey, blood, "death");
            int beforeHp = target.Hp;
            DealDamage(target, Mathf.Clamp(enemy.Power - 5, 6, 14), "death", blood);
            int stolen = Mathf.Min(3, Mathf.Max(0, beforeHp - target.Hp) / 3);
            int restored = Mathf.Min(stolen, Mathf.Max(0, enemy.MaxHp - enemy.Hp));
            enemy.Hp += restored;
            if (restored > 0) AddFloat(enemy.X, enemy.Y, "+" + restored, blood);
            if (target.Hp <= 0) ReportUnitDown(target);
            PushLog("The Ritual Heart pulses at " + target.Name + ". Its tether steals at most three health; stone cover breaks the pulse.", Tone.Warn);
            CompleteEnemyPowerBeat(pulse);
            return true;
        }
    }
}
