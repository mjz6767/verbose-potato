using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class SkillSpellAudioSmoke
    {
        public static void RunOrThrow()
        {
            Assert(SkillSpellAudioRules.CueKeys.Count == 21, "curated skill/spell bank has twenty-one masters");
            Assert(SkillSpellAudioRules.CueKeys.Distinct().Count() == 21, "curated cue keys are unique");
            string[] skills = { "shieldbash", "cleave", "sunder", "throwknife", "smokebomb", "hamstring",
                "aimedshot", "pinningshot", "broadheadshot", "disruptingshot", "quickshot" };
            foreach (string skill in skills)
            {
                CombatPowerSfxPlan plan = CombatPowerSfxRules.PlanForAbility(skill);
                Assert(plan.CoreCueCount >= 2 && plan.CueCount <= 4, skill + " keeps the tactical mix compact");
                Assert(plan.Cast.Key != plan.Impact.Key, skill + " separates preparation from contact");
                Assert(CombatPowerSfxRules.PlanForAbility(skill, reducedAudio: true).CueCount == 1, skill + " supports reduced audio");
                Assert(CombatPowerSfxRules.PlanForAbility(skill, muted: true).CueCount == 0, skill + " honors mute");
                Assert(CombatPowerSfxRules.PlanForAbility(skill, sfxVolumePercent: 0).CueCount == 0, skill + " honors zero level");
                Assert(CombatPowerSfxRules.PlanForAbility(skill, reducedAudio: true).Impact.Key == plan.Impact.Key,
                    skill + " retains its semantic contact in reduced audio");
            }
            Assert(CombatPowerSfxRules.PlanForAbility("smokebomb").CueCount == 2, "one quiet pin and one smoke puff; no repeated cloud or shimmer");
            Assert(CombatPowerSfxRules.PlanForAbility("hamstring").Aftershock.Key != "pinning", "a knife cut cannot fire a bow tail");
            Assert(CombatPowerSfxRules.PlanForAbility("quickshot").Release.Key == "twinshotrelease"
                && CombatPowerSfxRules.PlanForAbility("quickshot").Impact.Key == "twinshothit", "Twinshot carries two authored arrow beats");
            Assert(CombatPowerSfxRules.PlanForAbility("aimedshot").Aftershock.Enabled == false, "single shot does not fire again after impact");
            Assert(CombatPowerSfxRules.PlanForFormula("RBT").Release.Key == "demonriftrelease", "authored school bank preserves demonic enhancement routing");

            GameObject host = new GameObject("Skill spell audio smoke host");
            host.SetActive(false);
            host.hideFlags = HideFlags.HideAndDontSave;
            Dictionary<string, AudioClip> clips = null;
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
                AssertFractionalEnvelopeFallbacks(game, flags);
                clips = (Dictionary<string, AudioClip>)typeof(AshenHallsGame).GetField("soundClips", flags).GetValue(game);
                typeof(AshenHallsGame).GetMethod("BuildSkillSpellSoundClips", flags).Invoke(game, null);
                foreach (string key in SkillSpellAudioRules.CueKeys.Where(key => !key.StartsWith("release", StringComparison.Ordinal)))
                {
                    Assert(clips.TryGetValue(key, out AudioClip fallback) && fallback != null, key + " has an asset-independent fallback");
                    AssertFiniteClip(fallback, key + " procedural fallback");
                }
                foreach (AudioClip fallback in clips.Values) UnityEngine.Object.DestroyImmediate(fallback);
                clips.Clear();
                typeof(AshenHallsGame).GetMethod("BuildSoundClips", flags).Invoke(game, null);
                // Scan the complete resolved bank, not just the imported additions.
                // A one-sample NaN in a legacy fallback contaminates interpolation
                // and a layered mix even when that endpoint is almost inaudible.
                foreach (KeyValuePair<string, AudioClip> pair in clips)
                    AssertFiniteClip(pair.Value, pair.Key);
                HashSet<string> imported = (HashSet<string>)typeof(AshenHallsGame).GetField("importedSfxKeys", flags).GetValue(game);
                foreach (string key in SkillSpellAudioRules.CueKeys)
                {
                    Assert(imported.Contains(key), key + " selects its shipped master");
                    Assert(clips.TryGetValue(key, out AudioClip clip) && AssetDatabase.Contains(clip), key + " resolves to an imported master");
                    AssertHealthyMaster(clip, key);
                }
                CombatPowerSfxPlan[] after = skills.Select(key => CombatPowerSfxRules.PlanForAbility(key)).ToArray();
                AssertFinitePlans(clips, after);
                CombatAudioPolishSmoke.ExportPlansPreview(clips, after, "skill-spell-after.wav", string.Join(", ", skills));
                CombatPowerSfxPlan[] before =
                {
                    Baseline("shieldbash", "guard", "swingheavy", "counter", "impactshield", 1, .16f, .94f, true, false),
                    Baseline("smokebomb", "smoke", "", "smoke", "status", 2, .08f, .98f, false, true),
                    Baseline("hamstring", "eviscerate", "swing", "blade", "pinning", 1, .17f, 1f, true, false),
                    Baseline("broadheadshot", "aimedshot", "arrowrelease", "arrowcontact", "blade", 2, .20f, .96f, true, false),
                    Baseline("quickshot", "aimedshot", "arrowrelease", "arrowcontact", "bow", 2, .24f, 1.06f, true, false)
                };
                AssertFinitePlans(clips, before);
                CombatAudioPolishSmoke.ExportPlansPreview(clips, before, "skill-spell-comparison-before.wav",
                    "v2.36 source profiles (legacy fallback endpoint repaired): Iron Rebuff, Smoke Bomb, Hamstring, Broadhead Shot, Twinshot");
                CombatAudioPolishSmoke.ExportPlansPreview(clips,
                    before.Select(plan => CombatPowerSfxRules.PlanForAbility(plan.ProfileKey)).ToArray(),
                    "skill-spell-comparison-after.wav", "Iron Rebuff, Smoke Bomb, Hamstring, Broadhead Shot, Twinshot");
                CombatPowerSfxPlan[] schools = new[] { "FBL", "RCL", "RIG", "GBH", "OIC", "NVL" }
                    .Select(key => CombatPowerSfxRules.PlanForFormula(key)).ToArray();
                AssertFinitePlans(clips, schools);
                CombatAudioPolishSmoke.ExportPlansPreview(clips, schools,
                    "school-release-after.wav", "Fireball, Frost Lance, Arc Spark, Verdant Shelter, Heal, Night Veil");
            }
            finally
            {
                if (clips != null)
                    foreach (AudioClip clip in clips.Values)
                        if (clip != null && !AssetDatabase.Contains(clip)) UnityEngine.Object.DestroyImmediate(clip);
                UnityEngine.Object.DestroyImmediate(host);
            }
            Debug.Log("Skill and spell audio smoke passed: twenty-one masters, compact semantic skills, fallback/import selection, reduced/mute/zero, raw mix headroom.");
        }

        private static void AssertFractionalEnvelopeFallbacks(AshenHallsGame game, BindingFlags flags)
        {
            // Mathf.PI is float32: Sin(PI) is slightly negative, not exactly zero.
            // A fractional Pow of that final sample used to produce NaN. Exercise
            // real generators at short, original and new-fallback durations.
            MethodInfo skill = typeof(AshenHallsGame).GetMethod("MakeSkillSound", flags);
            MethodInfo generic = typeof(AshenHallsGame).GetMethod("MakeSound", flags);
            foreach (float duration in new[] { .08f, .34f, .46f })
            {
                foreach (string style in new[] { "smoke", "stealth", "whirlwind" })
                {
                    AudioClip clip = (AudioClip)skill.Invoke(game, new object[] { "fractional-" + style, style, 110f, 180f, duration, .24f });
                    try { AssertFiniteClip(clip, style + " fractional envelope " + duration); }
                    finally { UnityEngine.Object.DestroyImmediate(clip); }
                }
                AudioClip smoke = (AudioClip)generic.Invoke(game, new object[] { "fractional-generic-smoke", 110f, 180f, duration, .24f, "smoke" });
                try { AssertFiniteClip(smoke, "generic smoke fractional envelope " + duration); }
                finally { UnityEngine.Object.DestroyImmediate(smoke); }
            }
        }

        private static void AssertFinitePlans(Dictionary<string, AudioClip> clips, IEnumerable<CombatPowerSfxPlan> plans)
        {
            foreach (CombatPowerSfxPlan plan in plans)
            {
                CombatPowerSfxCuePlan[] cues = { plan.Cast, plan.Release, plan.Impact, plan.Aftershock, plan.LowHit, plan.Rumble, plan.Shimmer };
                foreach (CombatPowerSfxCuePlan cue in cues)
                {
                    Assert(Finite(cue.Delay) && Finite(cue.Gain) && Finite(cue.Pitch), plan.ProfileKey + " cue parameters are finite");
                    if (!cue.Enabled) continue;
                    Assert(clips.TryGetValue(cue.Key, out AudioClip clip), plan.ProfileKey + " resolves " + cue.Key);
                    AssertFiniteClip(clip, plan.ProfileKey + "/" + cue.Key);
                }
            }
        }

        private static void AssertFiniteClip(AudioClip clip, string label)
        {
            Assert(clip != null && clip.samples > 0, label + " has sample data");
            float[] samples = new float[clip.samples * clip.channels];
            Assert(clip.GetData(samples, 0), label + " sample data is readable");
            for (int index = 0; index < samples.Length; index++)
                if (!Finite(samples[index]))
                    throw new InvalidOperationException(label + " has a non-finite source sample at " + index + "/" + samples.Length);
        }

        private static bool Finite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }

        private static CombatPowerSfxPlan Baseline(string key, string cast, string release, string impact, string tail,
            int tier, float impactDelay, float pitch, bool low, bool shimmer)
        {
            float releaseDelay = string.IsNullOrEmpty(release) ? 0f : Math.Max(.025f, impactDelay * .42f);
            releaseDelay = Math.Min(releaseDelay, Math.Max(0f, impactDelay - CombatPowerTravelVfxRules.MinimumDurationSeconds));
            return CombatPowerSfxRules.BuildPlan(new CombatPowerSfxProfile(key, tier,
                new CombatPowerSfxCuePlan(CombatPowerSfxPhase.Cast, cast, 0, .40f + tier * .07f, pitch + .015f),
                new CombatPowerSfxCuePlan(CombatPowerSfxPhase.Release, release, releaseDelay, .30f + tier * .055f, pitch + .025f),
                new CombatPowerSfxCuePlan(CombatPowerSfxPhase.Impact, impact, impactDelay, .68f + tier * .11f, pitch),
                new CombatPowerSfxCuePlan(CombatPowerSfxPhase.Aftershock, tail, impactDelay + .07f + tier * .025f, .18f + tier * .065f, pitch - .035f),
                low, false, shimmer));
        }

        private static void AssertHealthyMaster(AudioClip clip, string key)
        {
            Assert(clip.channels == 1 && clip.frequency == 48000 && clip.length >= .12f && clip.length <= .51f,
                key + " uses compact mono 48 kHz PCM");
            float[] data = new float[clip.samples];
            Assert(clip.GetData(data, 0), key + " samples can be read");
            double energy = 0, sum = 0;
            float peak = 0;
            foreach (float value in data)
            {
                Assert(!float.IsNaN(value) && !float.IsInfinity(value), key + " samples are finite");
                peak = Math.Max(peak, Math.Abs(value));
                energy += value * value;
                sum += value;
            }
            Assert(peak <= .681f, key + " retains at least 3.3 dB headroom");
            double rms = Math.Sqrt(energy / data.Length);
            Assert(rms > .019 && rms < .16, key + " level stays inside the authored range");
            Assert(Math.Abs(sum / data.Length) < .004, key + " has bounded DC offset");
            Assert(Math.Abs(data[0]) < .0001 && Math.Abs(data[data.Length - 1]) < .0001, key + " has silent edges");
        }

        private static void Assert(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
