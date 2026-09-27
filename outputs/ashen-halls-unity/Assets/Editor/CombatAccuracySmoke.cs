using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class CombatAccuracySmoke
    {
        public static void Run()
        {
            try { RunOrThrow(); Debug.Log("Combat accuracy smoke passed."); EditorApplication.Exit(0); }
            catch (Exception ex) { Debug.LogException(ex); EditorApplication.Exit(1); }
        }

        public static void RunOrThrow()
        {
            for (int rank = 0; rank <= 25; rank++)
                Require(CombatAccuracyRules.SkillContribution(rank) == rank, "early accuracy remains unchanged");
            Require(CombatAccuracyRules.SkillContribution(26) == 25
                && CombatAccuracyRules.SkillContribution(28) == 26
                && CombatAccuracyRules.SkillContribution(99) == 49,
                "later mastery has bounded, monotone accuracy contribution");
            int previous = 0;
            for (int rank = 0; rank <= 120; rank++)
            {
                int value = CombatAccuracyRules.SkillContribution(rank);
                Require(value >= previous && value <= 49, "accuracy growth neither reverses nor exceeds its cap");
                previous = value;
            }

            foreach (bool projectile in new[] { false, true })
            {
                int openHit;
                using (Fixture f = new Fixture(projectile, false))
                {
                    CombatAttackForecast forecast = f.Forecast();
                    openHit = forecast.HitChance;
                    Require(forecast.Legal && forecast.HitChance == 95, "mastered attack remains reliable in an open stance");
                    Require((bool)f.Call("Attack", f.Hero, f.Target) && f.Target.Hp < 200,
                        "actual open-stance attack lands at the controlled roll");
                }
                using (Fixture f = new Fixture(projectile, true))
                {
                    CombatAttackForecast forecast = f.Forecast();
                    Require(forecast.Legal && forecast.HitChance < 85 && forecast.HitChance < openHit,
                        "deliberate Guard is visible below the cap against mastered melee and missile attacks");
                    Require((bool)f.Call("Attack", f.Hero, f.Target) && f.Target.Hp == 200,
                        "actual guarded attack misses the same controlled roll that landed without Guard");
                    Require(AttackRules.BuildDamageProfile(f.Hero, f.Target, 99, 0).SkillBonus == 19,
                        "accuracy diminishing returns do not replace damage's actual skill value");
                }
            }
            using (Fixture f = new Fixture(true, true))
            {
                int guarded = (int)f.Call("MartialHitChance", f.Hero, f.Target, 4, "missile");
                Require(guarded < 85 && !(bool)f.Call("RollMartialHit", f.Hero, f.Target, 4, "test shot", "missile"),
                    "missile technique execution uses its calculated guarded chance");
                f.Target.Guarding = false;
                Require((int)f.Call("MartialHitChance", f.Hero, f.Target, 4, "missile") == 96
                    && (bool)f.Call("RollMartialHit", f.Hero, f.Target, 4, "test shot", "missile"),
                    "same shot connects when Guard is removed");
            }
            using (Fixture f = new Fixture(false, false))
            {
                f.Hero.Skills.Arms = 25;
                f.Hero.WeaponBonus = 0;
                f.Hero.Agility = 3;
                f.Hero.AttackSpeed = 5;
                int ordinary = (int)f.Call("AttackHitChance", f.Hero, f.Target);
                Require(ordinary == 56, "low-rank production formula retains its original exact chance");
                f.Hero.Hexed = 1;
                Require((int)f.Call("AttackHitChance", f.Hero, f.Target) == ordinary - 12,
                    "existing accuracy debuffs remain meaningful");
                f.Hero.Hexed = 0;
                f.Hero.Side = UnitSide.Enemy;
                f.Target.Side = UnitSide.Party;
                Require((int)f.Call("AttackHitChance", f.Hero, f.Target) == ordinary,
                    "the same early contribution applies symmetrically to enemy basics");
                f.Hero.Skills.Arms = 99;
                int masteredEnemy = (int)f.Call("AttackHitChance", f.Hero, f.Target);
                Require(masteredEnemy == 80, "enemy mastery receives the same diminished contribution");
                f.Hero.Side = UnitSide.Party;
                f.Target.Side = UnitSide.Enemy;
                Require((int)f.Call("AttackHitChance", f.Hero, f.Target) == masteredEnemy,
                    "diminishing returns are identical for both sides");
            }
        }

        private sealed class Fixture : IDisposable
        {
            private readonly GameObject host = RemembranceItemSmoke.NewHost("Combat accuracy fixture");
            private readonly AshenHallsGame game;
            public readonly CombatUnit Hero, Target;
            public Fixture(bool projectile, bool guarding)
            {
                game = host.AddComponent<AshenHallsGame>();
                string role = projectile ? "bow" : "shield";
                string weapon = projectile ? "practice longbow" : "practice longsword";
                var member = new PartyMember { Id = "accuracy-hero", Name = "Tester", Race = "human",
                    ClassKey = projectile ? "ranger" : "warrior", Role = role, Level = 15,
                    WeaponName = weapon, WeaponDamageType = "physical", WeaponBonus = 4,
                    Skills = new SkillSet { Arms = 99, Missile = 99, Guard = 1 }.Normalize() };
                Hero = new CombatUnit { Id = member.Id, Name = member.Name, Race = member.Race,
                    ClassKey = member.ClassKey, Role = role, PartyIndex = 0, Side = UnitSide.Party,
                    X = 2, Y = 2, Hp = 200, MaxHp = 200, Range = projectile ? 5 : 1,
                    WeaponName = weapon, WeaponBonus = 4, DamageType = "physical", DamageMin = 20, DamageMax = 20,
                    Agility = 8, AttackSpeed = 12, Skills = member.Skills.Clone() };
                Target = new CombatUnit { Id = "accuracy-target", Name = "Defender", Role = "raider", PartyIndex = -1,
                    Side = UnitSide.Enemy, X = projectile ? 5 : 3, Y = 2, Hp = 200, MaxHp = 200,
                    Agility = 7, Defense = 7, Range = 1, Guarding = guarding, GuardBonus = guarding ? 4 : 0,
                    Skills = new SkillSet().Normalize() };
                var state = new GameState { Mode = GameMode.Combat, Depth = 6,
                    ContentSetId = ContentSetCatalog.SewerSlice, SaveVersion = VersionInfo.SaveVersion,
                    ReducedMotion = true, SfxMuted = true, MusicMuted = true, Party = new List<PartyMember> { member },
                    Combat = new CombatState { ActiveId = Hero.Id, Units = new List<CombatUnit> { Hero, Target },
                        ActionAvailable = true, Phase = CombatPhase.ChooseAction } };
                RemembranceItemSmoke.Set(game, "state", state);
                RemembranceItemSmoke.Set(game, "rng", new FixedRandom());
                RemembranceItemSmoke.Set(game, "visualSmokeSaveBlocked", true);
            }
            public CombatAttackForecast Forecast() => (CombatAttackForecast)Call("AttackForecast", Hero, Target);
            public object Call(string method, params object[] args) => RemembranceItemSmoke.Invoke(game, method, args);
            public void Dispose() => UnityEngine.Object.DestroyImmediate(host);
        }

        private sealed class FixedRandom : System.Random
        {
            public override int Next(int maxValue) => Math.Min(85, maxValue - 1);
            public override int Next(int minValue, int maxValue) => minValue;
            public override double NextDouble() => .99;
        }
        private static void Require(bool condition, string message)
        { if (!condition) throw new InvalidOperationException("Combat accuracy: " + message); }
    }
}
