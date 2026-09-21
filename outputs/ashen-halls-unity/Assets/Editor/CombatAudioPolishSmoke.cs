using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class CombatAudioPolishSmoke
    {
        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log("Combat audio polish smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError("Combat audio polish smoke failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            Assert(!CombatAudioMixRules.CanReplacePlayingVoice(3, 0.01f, 3), "simultaneous impacts preserve their attack");
            Assert(!CombatAudioMixRules.CanReplacePlayingVoice(3, 0.50f, 1), "decorative tails never replace a primary impact");
            Assert(CombatAudioMixRules.CanReplacePlayingVoice(1, 0.01f, 3), "primary impacts can claim a supporting voice");
            Assert(CombatAudioMixRules.CanReplacePlayingVoice(3, 0.10f, 3), "new impacts can replace an older impact tail");
            Assert(!CombatAudioMixRules.CanReplacePlayingVoice(1, 0.02f, 1), "simultaneous supporting transients remain bounded");
            Assert(CombatAudioMixRules.VoiceCongestionGain(8, 3) == 1f, "busy mix preserves primary impact level");
            Assert(CombatAudioMixRules.VoiceCongestionGain(8, 0) < CombatAudioMixRules.VoiceCongestionGain(8, 1), "crowding reduces decoration first");
            Assert(CombatAudioMixRules.VoiceCongestionGain(0, 0) == 1f, "quiet mix leaves authored gains intact");
            Assert(CombatPowerSfxRules.LowHitCue != CombatPowerSfxRules.RumbleCue, "rumble is a soft tail rather than a repeated thump");

            string[] formulaKeys = { "FBL", "RCL", "RIG", "GBH", "OIC", "RNH", "RBT", "NVL" };
            HashSet<string> releaseKeys = new HashSet<string>(StringComparer.Ordinal);
            foreach (string formula in formulaKeys)
            {
                CombatPowerSfxPlan plan = CombatPowerSfxRules.PlanForFormula(formula);
                Assert(releaseKeys.Add(plan.Release.Key), formula + " has its own elemental release texture");
                Assert(plan.Impact.Delay - plan.Release.Delay >= CombatPowerTravelVfxRules.MinimumDurationSeconds - 0.00001f,
                    formula + " gives visible travel time before audible impact");
                Assert(CombatPowerSfxRules.PlanForFormula(formula, reducedAudio: true).CueCount == 1, formula + " preserves compact accessible audio");
                Assert(CombatPowerSfxRules.PlanForFormula(formula, muted: true).CueCount == 0, formula + " honors mute");
            }

            GameObject host = new GameObject("Combat audio polish smoke host");
            host.SetActive(false);
            host.hideFlags = HideFlags.HideAndDontSave;
            Dictionary<string, AudioClip> clips = null;
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
                clips = (Dictionary<string, AudioClip>)typeof(AshenHallsGame).GetField("soundClips", flags).GetValue(game);
                typeof(AshenHallsGame).GetMethod("BuildSoundClips", flags).Invoke(game, null);
                releaseKeys.Add(CombatPowerSfxRules.RumbleCue);
                foreach (string key in releaseKeys)
                {
                    Assert(clips.TryGetValue(key, out AudioClip clip) && clip != null, "bank resolves " + key);
                    AssertHealthyClip(clip, key);
                }
                ExportPreview(clips);
            }
            finally
            {
                if (clips != null)
                {
                    foreach (AudioClip clip in clips.Values)
                    {
                        if (clip != null && !AssetDatabase.Contains(clip)) UnityEngine.Object.DestroyImmediate(clip);
                    }
                }
                UnityEngine.Object.DestroyImmediate(host);
            }
        }

        private static void AssertHealthyClip(AudioClip clip, string key)
        {
            float[] samples = new float[clip.samples * clip.channels];
            Assert(clip.GetData(samples, 0), key + " samples are readable");
            double energy = 0d;
            float peak = 0f;
            foreach (float sample in samples)
            {
                Assert(!float.IsNaN(sample) && !float.IsInfinity(sample), key + " samples are finite");
                peak = Math.Max(peak, Math.Abs(sample));
                energy += sample * sample;
            }
            Assert(peak < 0.95f, key + " leaves output headroom");
            Assert(Math.Sqrt(energy / samples.Length) > 0.002f, key + " has audible body");
            Assert(Math.Abs(samples[0]) <= 0.00001f && Math.Abs(samples[samples.Length - 1]) < 0.001f,
                key + " starts and ends without a discontinuity");
        }

        private static void ExportPreview(Dictionary<string, AudioClip> clips)
        {
            const int rate = 44100;
            CombatPowerSfxPlan[] plans =
            {
                CombatPowerSfxRules.PlanForFormula("FBL"), CombatPowerSfxRules.PlanForFormula("RCL"),
                CombatPowerSfxRules.PlanForFormula("RBT"), CombatPowerSfxRules.PlanForAbility("charge"),
                CombatPowerSfxRules.PlanForAbility("whirlwind"), CombatPowerSfxRules.PlanForAbility("volley")
            };
            List<float> output = new List<float>();
            foreach (CombatPowerSfxPlan plan in plans)
            {
                CombatPowerSfxCuePlan[] cues = { plan.Cast, plan.Release, plan.Impact, plan.Aftershock, plan.LowHit, plan.Rumble, plan.Shimmer };
                float[] pans = { -0.35f, 0f, 0.35f, 0.25f, 0.17f, 0.10f, -0.25f };
                float duration = 1f;
                foreach (CombatPowerSfxCuePlan cue in cues)
                {
                    if (cue.Enabled && clips.TryGetValue(cue.Key, out AudioClip clip))
                        duration = Math.Max(duration, cue.Delay + clip.length / 0.90f);
                }
                float[] segment = new float[(int)((duration + 0.45f) * rate) * 2];
                for (int channel = 0; channel < cues.Length; channel++)
                {
                    CombatPowerSfxCuePlan cue = cues[channel];
                    if (!cue.Enabled || !clips.TryGetValue(cue.Key, out AudioClip clip)) continue;
                    float[] source = new float[clip.samples * clip.channels];
                    if (!clip.GetData(source, 0)) throw new InvalidOperationException("Preview cannot read " + cue.Key);
                    float pitch = CombatPowerSfxRules.StablePitch(cue, plan.ProfileKey, 1, channel);
                    float left = Mathf.Cos((pans[channel] + 1f) * Mathf.PI * 0.25f);
                    float right = Mathf.Sin((pans[channel] + 1f) * Mathf.PI * 0.25f);
                    int start = (int)(cue.Delay * rate);
                    for (int frame = 0; frame + start < segment.Length / 2; frame++)
                    {
                        float position = frame * pitch * clip.frequency / rate;
                        int sourceFrame = (int)position;
                        if (sourceFrame >= clip.samples - 1) break;
                        float sample = 0f;
                        for (int c = 0; c < clip.channels; c++)
                            sample += Mathf.Lerp(source[sourceFrame * clip.channels + c], source[(sourceFrame + 1) * clip.channels + c], position - sourceFrame);
                        sample *= cue.Gain * 0.78f / clip.channels;
                        segment[(start + frame) * 2] += sample * left;
                        segment[(start + frame) * 2 + 1] += sample * right;
                    }
                }
                output.AddRange(segment);
            }
            float peak = 0f;
            foreach (float sample in output)
            {
                Assert(!float.IsNaN(sample) && !float.IsInfinity(sample), "preview output is finite");
                peak = Math.Max(peak, Math.Abs(sample));
            }
            float gain = peak > 0.90f ? 0.90f / peak : 1f;
            string directory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "QA", "combat-effects"));
            Directory.CreateDirectory(directory);
            string path = Path.Combine(directory, "audio-preview.wav");
            using (BinaryWriter writer = new BinaryWriter(File.Create(path)))
            {
                writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
                writer.Write(36 + output.Count * 2);
                writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt "));
                writer.Write(16); writer.Write((short)1); writer.Write((short)2);
                writer.Write(rate); writer.Write(rate * 4); writer.Write((short)4); writer.Write((short)16);
                writer.Write(System.Text.Encoding.ASCII.GetBytes("data")); writer.Write(output.Count * 2);
                foreach (float sample in output) writer.Write((short)Math.Round(sample * gain * short.MaxValue));
            }
            Debug.Log("Combat audio preview (Fireball, Cold Lance, Rift Bolt, Charge, Whirlwind, Volley): " + path);
        }

        private static void Assert(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
