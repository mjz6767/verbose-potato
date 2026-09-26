using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class EverydayAudioSmoke
    {
        public static void Run()
        {
            try { RunOrThrow(); Debug.Log("Everyday audio smoke passed."); EditorApplication.Exit(0); }
            catch (Exception ex) { Debug.LogError("Everyday audio smoke failed: " + ex); EditorApplication.Exit(1); }
        }

        public static void RunOrThrow()
        {
            Assert(GameAudioCueRules.EquipmentCueFor(null) == "itemequip", "missing item retains generic fallback");
            Assert(GameAudioCueRules.EquipmentCueFor(new InventoryItem { Slot = "quest", Form = "iron sword" }) == "itemequip",
                "quest materials do not pretend to be equipment");
            AssertEquip("weapon", "longbow", "wood", "equipbow");
            AssertEquip("weapon", "ritual staff", "ash", "equipstaff");
            AssertEquip("weapon", "sword", "iron", "equipblade");
            AssertEquip("armor", "hide coat", "leather", "equipcloth");
            AssertEquip("armor", "chain mail", "steel", "equipmail");
            AssertEquip("armor", "cuirass", "bronze", "equipplate");
            Assert(GameAudioCueRules.LootCueFor(null) == "itemtake", "resource-only rewards use pack sound");
            Assert(GameAudioCueRules.LootCueFor(new InventoryItem { Rarity = "common" }) == "itemtake", "common loot remains restrained");
            Assert(GameAudioCueRules.LootCueFor(new InventoryItem { Rarity = " RARE " }) == "lootrare", "rare loot announces its quality");
            Assert(GameAudioCueRules.LootCueFor(new InventoryItem { SignatureId = "named-relic" }) == "lootrare", "signature reward gets relic sound");

            string[] keys = GameAudioCueRules.EverydayCueKeys.Concat(GameAudioCueRules.VariedFootstepKeys
                .SelectMany(key => new[] { key + "__v1", key + "__v2" })).ToArray();
            Assert(keys.Length == GameAudioCueRules.EverydayMasterCount && keys.Distinct().Count() == keys.Length,
                "declared everyday master count and unique keys agree");
            GameObject host = new GameObject("Everyday audio smoke host");
            host.SetActive(false);
            host.hideFlags = HideFlags.HideAndDontSave;
            var generated = new HashSet<AudioClip>();
            Dictionary<string, AudioClip> clips = null;
            try
            {
                const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                clips = (Dictionary<string, AudioClip>)typeof(AshenHallsGame).GetField("soundClips", flags).GetValue(game);
                foreach (string key in GameAudioCueRules.VariedFootstepKeys)
                {
                    AudioClip fallback = AudioClip.Create(key + "_fallback", 480, 1, 48000, false);
                    clips[key] = fallback;
                    generated.Add(fallback);
                }
                typeof(AshenHallsGame).GetMethod("BuildEverydaySoundClips", flags).Invoke(game, null);
                foreach (string key in keys)
                {
                    Assert(clips.TryGetValue(key, out AudioClip fallback) && fallback != null, "fallback resolves " + key);
                    generated.Add(fallback);
                }
                foreach (string key in GameAudioCueRules.VariedFootstepKeys)
                    Assert(clips[key + "__v1"] == clips[key] && clips[key + "__v2"] == clips[key],
                        key + " keeps its base fallback when optional variant masters are missing");

                typeof(AshenHallsGame).GetMethod("BuildSoundClips", flags).Invoke(game, null);
                foreach (string key in keys)
                {
                    AudioClip master = Resources.Load<AudioClip>("Audio/Sfx/" + key);
                    Assert(master != null && clips[key] == master, "runtime resolves imported master " + key);
                    Assert(master.channels == 1 && master.frequency == 48000, key + " uses preserved mono PCM");
                    float[] samples = Samples(master);
                    Assert(samples.All(sample => !float.IsNaN(sample) && !float.IsInfinity(sample)), key + " is finite");
                    Assert(samples.Max(sample => Math.Abs(sample)) < .80f && Rms(samples) > .005, key + " has audible body and headroom");
                    Assert(samples[0] == 0f && samples[samples.Length - 1] == 0f, key + " has silent boundaries");
                }
                foreach (string key in GameAudioCueRules.VariedFootstepKeys)
                {
                    float[] first = Samples(clips[key + "__v1"]);
                    float[] second = Samples(clips[key + "__v2"]);
                    Assert(!first.SequenceEqual(second), key + " variants contain distinct articulation");
                    double baseRms = Rms(Samples(clips[key]));
                    Assert(Math.Abs(20d * Math.Log10(Rms(first) / baseRms)) < 2.5d
                        && Math.Abs(20d * Math.Log10(Rms(second) / baseRms)) < 2.5d,
                        key + " variants preserve calibrated surface loudness");
                }
            }
            finally
            {
                if (clips != null) foreach (AudioClip clip in clips.Values) generated.Add(clip);
                foreach (AudioClip clip in generated)
                    if (clip != null && !AssetDatabase.Contains(clip)) UnityEngine.Object.DestroyImmediate(clip);
                UnityEngine.Object.DestroyImmediate(host);
            }
            Debug.Log("Everyday audio bank validated: " + keys.Length + " masters, semantic equipment and loot routing, eight varied surfaces, retained fallbacks.");
        }

        private static void AssertEquip(string slot, string form, string material, string expected)
        {
            Assert(GameAudioCueRules.EquipmentCueFor(new InventoryItem { Slot = slot, Form = form, Material = material }) == expected,
                form + " resolves its material equipment sound");
        }
        private static float[] Samples(AudioClip clip)
        {
            float[] samples = new float[clip.samples * clip.channels];
            Assert(clip.GetData(samples, 0), clip.name + " is readable");
            return samples;
        }
        private static double Rms(float[] samples) { return Math.Sqrt(samples.Sum(value => (double)value * value) / samples.Length); }
        private static void Assert(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
    }
}
