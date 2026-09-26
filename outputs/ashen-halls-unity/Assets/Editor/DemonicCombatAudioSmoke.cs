using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class DemonicCombatAudioSmoke
    {
        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log("Demonic combat audio smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError("Demonic combat audio smoke failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            GameObject host = new GameObject("Demonic audio smoke");
            host.SetActive(false);
            host.hideFlags = HideFlags.HideAndDontSave;
            Dictionary<string, AudioClip> clips = null;
            try
            {
                const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                clips = (Dictionary<string, AudioClip>)typeof(AshenHallsGame).GetField("soundClips", flags).GetValue(game);
                typeof(AshenHallsGame).GetMethod("BuildSoundClips", flags).Invoke(game, null);
                string[] keys = { "RKW", "RNH", "WBK", "NVL", "RMS", "INH", "RMB", "WBP", "GRH", "RPX", "DMC", "WTR", "DSM", "RLM", "RBT", "IBD", "SLV", "PBR", "IBF", "VRS", "IBG", "DFA", "ACR" };
                foreach (string key in keys) VerifyPlan(clips, CombatPowerSfxRules.PlanForFormula(key), CombatPowerSfxRules.PlanForFormula(key, reducedAudio: true));
                foreach (string key in new[] { "riftpounce", "abyssalwhirl", "soulrend", "dreadroar" })
                    VerifyPlan(clips, CombatPowerSfxRules.PlanForAbility(key), CombatPowerSfxRules.PlanForAbility(key, reducedAudio: true));

                string[] creatureCues = { "demonalert", "demonstep", "demonattack", "demoncast", "demonhurt", "demondeath" };
                KeyValuePair<string, AudioClip>[] demonicBank = clips.Where(entry => entry.Key.StartsWith("demon", StringComparison.Ordinal)
                    && !creatureCues.Contains(entry.Key)).ToArray();
                foreach (KeyValuePair<string, AudioClip> entry in demonicBank)
                {
                    VerifyClip(entry.Key, entry.Value);
                }
                Assert(demonicBank.Length == 44, "the complete demonic bank includes thirty full cues and fourteen compact confirmations");
                Debug.Log("Demonic bank validated: " + demonicBank.Length + " clips; max sample peak="
                    + demonicBank.Max(entry => Samples(entry.Value).Max(sample => Math.Abs(sample)))
                    + "; minimum RMS=" + demonicBank.Min(entry => Rms(entry.Value)));
                string[] signatureKeys = { "RBT", "IBD", "IBF", "IBG", "RLM", "DFA" };
                Assert(signatureKeys.Select(key => CombatPowerSfxRules.PlanForFormula(key).Impact.Key).Distinct().Count() == signatureKeys.Length,
                    "rift bolt, summon tiers, death burst and ascendance use distinct impact textures");
                AudioClip greater = clips[CombatPowerSfxRules.PlanForFormula("IBG").Impact.Key];
                AudioClip imp = clips[CombatPowerSfxRules.PlanForFormula("IBD").Impact.Key];
                Assert(greater.length > imp.length * 1.5f, "greater summon has an extended gate and creature arrival");
                VerifyDeterminism(game, flags);
                CombatAudioPolishSmoke.ExportPlansPreview(clips, signatureKeys.Select(key => CombatPowerSfxRules.PlanForFormula(key)).ToArray(),
                    "demonic-spells-preview.wav", "Rift Bolt, Summon Imp, Lesser Demon, Greater Demon, Death Burst, Abyssal Ascendance");
                CombatAudioPolishSmoke.ExportPlansPreview(clips, new[] { "riftpounce", "abyssalwhirl", "soulrend", "dreadroar" }
                    .Select(key => CombatPowerSfxRules.PlanForAbility(key)).ToArray(),
                    "demon-arts-preview.wav", "Rift Pounce, Abyssal Whirl, Soul Rend, Dread Roar");
            }
            finally
            {
                if (clips != null)
                    foreach (AudioClip clip in clips.Values)
                        if (clip != null && !AssetDatabase.Contains(clip)) UnityEngine.Object.DestroyImmediate(clip);
                UnityEngine.Object.DestroyImmediate(host);
            }
        }

        private static void VerifyPlan(Dictionary<string, AudioClip> clips, CombatPowerSfxPlan plan, CombatPowerSfxPlan compact)
        {
            Assert(plan.CueCount <= 7, plan.ProfileKey + " stays inside the existing layered voice budget");
            CombatPowerSfxCuePlan[] cues = { plan.Cast, plan.Release, plan.Impact, plan.Aftershock, plan.LowHit, plan.Rumble, plan.Shimmer };
            foreach (CombatPowerSfxCuePlan cue in cues)
                if (cue.Enabled) Assert(clips.ContainsKey(cue.Key), plan.ProfileKey + " resolves " + cue.Key);
            Assert(plan.Impact.Delay - plan.Release.Delay >= CombatPowerTravelVfxRules.MinimumDurationSeconds - 0.00001f,
                plan.ProfileKey + " preserves release-to-impact travel alignment");
            Assert(compact.CueCount == 1 && compact.Impact.Delay == 0f && compact.Impact.Gain <= 0.82f,
                plan.ProfileKey + " keeps a single quiet immediate confirmation in reduced mode");
            Assert(clips.ContainsKey(compact.Impact.Key), "compact impact resolves for " + plan.ProfileKey);
            if (DemonicPowerSfxRules.IsSignatureImpact(plan.Impact.Key))
            {
                Assert(clips[compact.Impact.Key].length <= 0.241f, plan.ProfileKey + " reduced mode omits extended ritual/roar tails");
                Assert(Rms(clips[compact.Impact.Key]) < Rms(clips[plan.Impact.Key]), plan.ProfileKey + " reduced cue has lower measured body level");
            }
            CombatPowerSfxPlan muted = CombatPowerSfxRules.IsSupportedAbility(plan.ProfileKey)
                ? CombatPowerSfxRules.PlanForAbility(plan.ProfileKey, muted: true)
                : CombatPowerSfxRules.PlanForFormula(plan.ProfileKey, muted: true);
            Assert(muted.CueCount == 0, plan.ProfileKey + " respects mute");
        }

        private static void VerifyClip(string key, AudioClip clip)
        {
            float[] samples = Samples(clip);
            Assert(samples.All(sample => !float.IsNaN(sample) && !float.IsInfinity(sample)), key + " is finite");
            Assert(samples.Max(sample => Math.Abs(sample)) < 0.95f, key + " leaves headroom");
            Assert(Rms(clip) > 0.002f, key + " has audible body");
            Assert(Math.Abs(samples[0]) < 0.00001f && Math.Abs(samples[samples.Length - 1]) < 0.00001f, key + " has click-free edges");
        }

        private static void VerifyDeterminism(AshenHallsGame game, BindingFlags flags)
        {
            MethodInfo method = typeof(AshenHallsGame).GetMethod("MakeDemonicSound", flags);
            object[] args = { "determinism-check", "gate", 70f, 38f, 0.30f, 0.30f, false };
            AudioClip first = (AudioClip)method.Invoke(game, args);
            AudioClip second = (AudioClip)method.Invoke(game, args);
            try { Assert(Samples(first).SequenceEqual(Samples(second)), "demonic synthesis is deterministic and does not consume gameplay RNG"); }
            finally { UnityEngine.Object.DestroyImmediate(first); UnityEngine.Object.DestroyImmediate(second); }
        }

        private static float[] Samples(AudioClip clip)
        {
            float[] samples = new float[clip.samples * clip.channels];
            Assert(clip.GetData(samples, 0), clip.name + " is readable");
            return samples;
        }

        private static double Rms(AudioClip clip)
        {
            float[] samples = Samples(clip);
            return Math.Sqrt(samples.Sum(sample => (double)sample * sample) / samples.Length);
        }

        private static void Assert(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
