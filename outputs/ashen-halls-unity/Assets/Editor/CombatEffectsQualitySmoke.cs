using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class CombatEffectsQualitySmoke
    {
        private const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;
        private const float Tolerance = 0.0001f;

        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " combat effects quality smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " combat effects quality smoke failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            GameObject host = new GameObject("Combat effects quality regression host");
            host.SetActive(false);
            host.hideFlags = HideFlags.HideAndDontSave;
            UnityEngine.Random.State originalRandom = UnityEngine.Random.state;
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                GameState state = new GameState
                {
                    Mode = GameMode.Combat, SfxMuted = true, MusicMuted = true,
                    Combat = new CombatState()
                };
                Set(game, "state", state);
                Set(game, "rng", new ForbiddenGameplayRandom());
                Set(game, "launchError", "Isolated effects fixture: no scene or persistence");
                UnityEngine.Random.State beforeEffects = UnityEngine.Random.state;

                int powerCount = 0;
                foreach (FormulaDef formula in FormulaCatalog.All)
                {
                    Require(formula != null, "formula catalog entries exist");
                    VerifyPower(game, state, formula.Code, CombatImpactRules.ForFormula(formula), true);
                    powerCount++;
                }
                foreach (string id in new[] { "warrior", "rogue", "ranger", "demon" }
                    .SelectMany(AbilityCatalog.IdsForClass).Distinct())
                {
                    MartialAbility ability = AbilityCatalog.For(id);
                    Require(ability != null, id + " ability definition exists");
                    VerifyPower(game, state, id, CombatImpactRules.ForAbility(ability), false);
                    powerCount++;
                }
                VerifyMotionTransitionAndBounds(game, state);
                Require(beforeEffects.Equals(UnityEngine.Random.state), "combat presentation leaves Unity's random stream unchanged");
                Debug.Log("Combat effects quality verified " + powerCount + " powers at all three intensities, including real runtime travel scheduling.");
            }
            finally
            {
                UnityEngine.Random.state = originalRandom;
                UnityEngine.Object.DestroyImmediate(host);
            }
        }

        private static void VerifyPower(AshenHallsGame game, GameState state, string key, CombatImpactProfile impact, bool formula)
        {
            state.ReducedMotion = false;
            Invoke(game, "ClearBetaVfxShowcasePresentation");
            PowerCastAura aura = StageCast(game, key, impact);
            Require(aura != null, key + " stages a cast aura");
            for (int intensity = 1; intensity <= 3; intensity++)
            {
                CombatPowerAnimationTimeline timeline = CombatPowerAnimationTimelineRules.For(key, aura.StableSeed, intensity);
                CombatPowerSfxPlan sound = formula
                    ? CombatPowerSfxRules.PlanForFormula(key, intensity)
                    : CombatPowerSfxRules.PlanForAbility(key, intensity);
                Require(timeline.Supported, key + " has authored choreography");
                Near(aura.ImpactAt - aura.Start, sound.Impact.Delay, key + " cast and impact sound agree");
                Near(timeline.ImpactAt, sound.Impact.Delay, key + " timeline and impact sound agree");
                Require(timeline.CompleteAt > timeline.ImpactAt, key + " retains a visible impact before completion");

                // Calling the actual staging method catches duration clamping that
                // otherwise lets a fast projectile arrive after its impact sound.
                PowerTravelVfx travel = StageTravel(game, key, intensity, 0, 0f);
                Require((travel != null) == timeline.HasTravel, key + " stages travel only when authored");
                if (travel == null) continue;
                Near(travel.Start, aura.ReleaseAt, key + " travel leaves at the staged release");
                Near(travel.Start + travel.Duration, aura.ImpactAt, key + " travel arrives at the staged impact");
                Near(travel.Start - aura.Start, sound.Release.Delay, key + " travel release and sound agree");
                Require(travel.Duration + Tolerance >= CombatPowerTravelVfxRules.MinimumDurationSeconds,
                    key + " retains the minimum readable travel duration");

                foreach (float offset in new[] { 0.10f, 1.30f })
                {
                    PowerTravelVfx followup = StageTravel(game, key, intensity, 1, offset);
                    Near(followup.Start, travel.Start + offset, key + " followup release preserves sequence offset " + offset);
                    Near(followup.Start + followup.Duration, aura.ImpactAt + offset,
                        key + " followup arrival preserves sequence offset " + offset);
                }
            }

            state.ReducedMotion = true;
            Invoke(game, "ClearBetaVfxShowcasePresentation");
            Require(StageCast(game, key, impact) == null, key + " Reduced Motion suppresses animated cast auras");
            Require(StageTravel(game, key, 3, 0, 0f) == null, key + " Reduced Motion suppresses moving projectiles");
            Require(!(bool)Invoke(game, "StageCombatPowerAftermath", key, impact, 5, 3, Color.white, 3, 0, 0, -1f),
                key + " Reduced Motion suppresses animated aftermath");
            ApplyImpact(game, key, impact);
            List<PowerImpactEcho> echoes = Get<List<PowerImpactEcho>>(game, "powerImpactEchoes");
            Require(echoes.Count == 1 && echoes[0].StaticStamp, key + " Reduced Motion retains one static impact");
            Near(echoes[0].Start, echoes[0].ImpactAt, key + " Reduced Motion impact is immediate");
            Near(echoes[0].Duration, CombatPowerAnimationTimelineRules.ReducedMotionImpactHoldSeconds,
                key + " Reduced Motion impact has a finite hold");
            CombatPowerSfxPlan compact = formula
                ? CombatPowerSfxRules.PlanForFormula(key, 3, true)
                : CombatPowerSfxRules.PlanForAbility(key, 3, true);
            Require(compact.CueCount == 1 && compact.Impact.Enabled && compact.Impact.Delay == 0f,
                key + " Reduced Motion retains one immediate audio cue");
            CombatPowerSfxPlan silent = formula
                ? CombatPowerSfxRules.PlanForFormula(key, 3, true, true)
                : CombatPowerSfxRules.PlanForAbility(key, 3, true, true);
            Require(silent.CueCount == 0, key + " mute suppresses all compact audio cues");
            Require(Get<System.Collections.IList>(game, "scheduledSfx").Count == 0,
                key + " muted runtime queues no delayed audio");
        }

        private static void VerifyMotionTransitionAndBounds(AshenHallsGame game, GameState state)
        {
            FormulaDef formula = Array.Find(FormulaCatalog.All, entry => entry.Code == "FBL");
            CombatImpactProfile impact = CombatImpactRules.ForFormula(formula);
            state.ReducedMotion = false;
            Invoke(game, "ClearBetaVfxShowcasePresentation");
            for (int i = 0; i < 40; i++)
            {
                StageCast(game, "FBL", impact);
                StageTravel(game, "FBL", 3, i, 0f);
                Invoke(game, "StageCombatPowerAftermath", "FBL", impact, 5, 3, Color.white, 3, i, 0, -1f);
            }
            Require(Get<List<PowerCastAura>>(game, "powerCastAuras").Count <= 10, "repeated casts keep aura storage bounded");
            Require(Get<List<PowerTravelVfx>>(game, "powerTravelVfx").Count <= 24, "repeated casts keep travel storage bounded");
            Require(Get<List<PowerAftermathVfx>>(game, "powerAftermathVfx").Count <= 24, "repeated casts keep aftermath storage bounded");
            state.ReducedMotion = true;
            Invoke(game, "ClearCombatMotionForReducedMotion");
            foreach (string field in new[] { "powerCastAuras", "powerTravelVfx", "powerAftermathVfx", "powerActorPoseBeats", "particles", "scheduledSfx" })
            {
                Require(Get<System.Collections.IList>(game, field).Count == 0, "enabling Reduced Motion clears pending " + field);
            }
            Require(Get<float>(game, "combatShakeMagnitude") == 0f, "enabling Reduced Motion clears camera shake");
            Invoke(game, "ClearTransientCombatPresentation");
            Require(Get<List<PowerImpactEcho>>(game, "powerImpactEchoes").Count == 0, "combat teardown clears impact stamps");
        }

        private static PowerCastAura StageCast(AshenHallsGame game, string key, CombatImpactProfile impact)
        {
            return (PowerCastAura)Invoke(game, "StageCombatPowerCast", impact, 1, 2, 5, 3, Color.white, false, key, 0);
        }

        private static PowerTravelVfx StageTravel(AshenHallsGame game, string key, int intensity, int sequence, float offset)
        {
            return (PowerTravelVfx)Invoke(game, "StageCombatPowerTravel", key, 1, 2, 5, 3, Color.white, intensity, -1f, sequence, offset, 0);
        }

        private static void ApplyImpact(AshenHallsGame game, string key, CombatImpactProfile impact)
        {
            typeof(AshenHallsGame).GetMethod("ApplyCombatImpactFeedback", PrivateInstance, null,
                new[] { typeof(CombatImpactProfile), typeof(int), typeof(int), typeof(Color), typeof(string), typeof(string) }, null)
                .Invoke(game, new object[] { impact, 5, 3, Color.white, key, key });
        }

        private static T Get<T>(object source, string name) => (T)source.GetType().GetField(name, PrivateInstance).GetValue(source);
        private static void Set(object source, string name, object value) => source.GetType().GetField(name, PrivateInstance).SetValue(source, value);
        private static object Invoke(object source, string name, params object[] arguments) => source.GetType().GetMethod(name, PrivateInstance).Invoke(source, arguments);
        private static void Near(float actual, float expected, string message)
        {
            Require(!float.IsNaN(actual) && !float.IsInfinity(actual) && Math.Abs(actual - expected) < Tolerance,
                message + ": expected " + expected + ", got " + actual);
        }
        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private sealed class ForbiddenGameplayRandom : System.Random
        {
            private static Exception Failure() => new InvalidOperationException("Combat presentation consumed gameplay randomness.");
            public override int Next() => throw Failure();
            public override int Next(int maxValue) => throw Failure();
            public override int Next(int minValue, int maxValue) => throw Failure();
            public override double NextDouble() => throw Failure();
            public override void NextBytes(byte[] buffer) => throw Failure();
            protected override double Sample() => throw Failure();
        }
    }
}
