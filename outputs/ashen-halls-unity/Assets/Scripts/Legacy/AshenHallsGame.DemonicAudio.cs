using System;
using UnityEngine;

namespace AshenHalls
{
    public partial class AshenHallsGame
    {
        private void BuildDemonicSoundClips()
        {
            AddDemonicSound("demonwhisper", "gather", 145f, 92f, 0.28f, 0.23f);
            AddDemonicSound("demoninvoke", "ritual", 72f, 118f, 0.32f, 0.26f);
            AddDemonicSound("demonsoulgather", "gather", 215f, 62f, 0.34f, 0.26f);
            AddDemonicSound("demongateinvoke", "ritual", 48f, 112f, 0.42f, 0.28f);
            AddDemonicSound("demonascendcast", "ascend", 66f, 184f, 0.40f, 0.27f);
            AddDemonicSound("demonpouncecast", "release", 110f, 265f, 0.19f, 0.23f);
            AddDemonicSound("demonwhirlcast", "whirl", 156f, 74f, 0.23f, 0.24f);
            AddDemonicSound("demonroarcast", "gather", 56f, 104f, 0.24f, 0.24f);
            AddDemonicSound("demonhexrelease", "release", 320f, 115f, 0.16f, 0.23f);
            AddDemonicSound("demonriftrelease", "release", 82f, 430f, 0.18f, 0.23f);
            AddDemonicSound("demondeathburst", "burst", 146f, 42f, 0.66f, 0.38f);
            AddDemonicSound("demonriftbolt", "tear", 340f, 90f, 0.36f, 0.33f);
            AddDemonicSound("demonimpbreach", "gate", 164f, 84f, 0.42f, 0.30f);
            AddDemonicSound("demonlesserbreach", "gate", 105f, 54f, 0.58f, 0.34f);
            AddDemonicSound("demongreaterbreach", "gate", 70f, 38f, 0.84f, 0.38f);
            AddDemonicSound("demonascendance", "ascend", 64f, 224f, 0.88f, 0.36f);
            AddDemonicSound("demonsoulpull", "tear", 420f, 118f, 0.39f, 0.30f);
            AddDemonicSound("demonbrand", "ritualhit", 124f, 55f, 0.51f, 0.33f);
            AddDemonicSound("demonveil", "choir", 142f, 204f, 0.44f, 0.28f);
            AddDemonicSound("demonriftstep", "tear", 420f, 64f, 0.31f, 0.31f);
            AddDemonicSound("demonpouncehit", "slam", 155f, 46f, 0.48f, 0.37f);
            AddDemonicSound("demonwhirlhit", "whirl", 178f, 57f, 0.59f, 0.34f);
            AddDemonicSound("demonrendhit", "tear", 510f, 62f, 0.52f, 0.36f);
            AddDemonicSound("demonroarhit", "roar", 82f, 48f, 0.78f, 0.36f);
            AddDemonicSound("demonsoultail", "choir", 240f, 84f, 0.56f, 0.22f);
            AddDemonicSound("demonrifttail", "tail", 128f, 54f, 0.39f, 0.20f);
            AddDemonicSound("demongatetail", "roar", 48f, 36f, 0.62f, 0.23f);
            AddDemonicSound("demonchoir", "choir", 112f, 167f, 0.72f, 0.23f);
            AddDemonicSound("demonrumble", "rumble", 76f, 35f, 0.56f, 0.24f);
            AddDemonicSound("demonritual", "ritual", 192f, 128f, 0.28f, 0.19f);
        }

        private void AddDemonicSound(string key, string texture, float from, float to, float duration, float gain)
        {
            soundClips[key] = MakeDemonicSound(key, texture, from, to, duration, gain, false);
            if (DemonicPowerSfxRules.IsSignatureImpact(key))
                soundClips[key + "compact"] = MakeDemonicSound(key + "compact", texture, from, to,
                    Mathf.Min(duration, 0.24f), gain * 0.66f, true);
        }

        private AudioClip MakeDemonicSound(string key, string texture, float from, float to, float duration, float gain, bool compact)
        {
            const int rate = 32000;
            int count = Mathf.CeilToInt(rate * duration);
            float[] dry = new float[count];
            float[] output = new float[count];
            uint noiseState = StableAudioSeed(key + "|demonic-ritual");
            float phase = 0f, choirPhase = 0f, upperPhase = 0f;
            float lowNoise = 0f, airNoise = 0f, formantA = 0f, formantB = 0f, priorA = 0f, priorB = 0f;
            bool gather = texture == "gather" || texture == "ritual";
            bool impact = DemonicPowerSfxRules.IsSignatureImpact(key.Replace("compact", ""));
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)rate;
                float p = i / (float)(count - 1);
                float noise = NextAudioNoise(ref noiseState);
                lowNoise += (noise - lowNoise) * 0.012f;
                airNoise += (noise - airNoise) * 0.085f;
                float grit = airNoise - lowNoise;
                float air = noise - airNoise;
                float frequency = Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, p));
                frequency *= 1f + 0.008f * Mathf.Sin(t * 31f) + lowNoise * 0.018f;
                phase += 2f * Mathf.PI * frequency / rate;
                choirPhase += 2f * Mathf.PI * (frequency * 0.749f + 1.6f) / rate;
                upperPhase += 2f * Mathf.PI * (frequency * 1.503f + 2.3f) / rate;
                float fundamental = Mathf.Sin(phase);
                float throat = fundamental * 0.48f + Mathf.Sin(phase * 2f) * 0.24f
                    + Mathf.Sin(phase * 3f) * 0.16f + Mathf.Sin(phase * 5f) * 0.07f;
                float breath = grit * 0.8f + air * 0.05f;
                // Two damped vocal resonances produce a throaty, nonverbal creature voice.
                float excitation = throat * 0.52f + breath * 0.48f;
                float a = 1.962f * Mathf.Cos(2f * Mathf.PI * (460f + p * 220f) / rate) * formantA
                    - 0.962361f * priorA + excitation * 0.026f;
                priorA = formantA; formantA = a;
                float b = 1.94f * Mathf.Cos(2f * Mathf.PI * (1190f - p * 370f) / rate) * formantB
                    - 0.9409f * priorB + excitation * 0.038f;
                priorB = formantB; formantB = b;
                float vocal = Mathf.Clamp(a * 0.28f + b * 0.22f, -0.9f, 0.9f);
                float choir = Mathf.Sin(choirPhase) * 0.44f + Mathf.Sin(upperPhase) * 0.24f
                    + Mathf.Sin(phase * 0.503f) * 0.24f;
                float sub = Mathf.Sin(phase * 0.5f);
                float envelope = Mathf.Pow(1f - p, 0.72f);
                float body = 0f;
                switch (texture)
                {
                    case "gather":
                        envelope = Mathf.Pow(Mathf.Sin(p * Mathf.PI), 2f) * (0.24f + p * 0.76f);
                        body = vocal * 0.70f + choir * 0.24f + breath * 0.55f;
                        break;
                    case "ritual":
                    case "ritualhit":
                        float bell = Mathf.Sin(phase * 3.17f) * 0.18f + Mathf.Sin(phase * 4.73f) * 0.11f;
                        body = choir * 0.38f + sub * 0.22f + vocal * 0.34f + bell * Mathf.Exp(-p * 2.8f);
                        envelope *= texture == "ritual" ? Mathf.SmoothStep(0f, 1f, p * 4f) : 1f;
                        break;
                    case "release":
                        envelope = Mathf.Pow(Mathf.Sin(p * Mathf.PI), 2f);
                        body = grit * 1.25f + vocal * 0.33f + choir * 0.18f + air * 0.12f;
                        break;
                    case "gate":
                        float hinges = AudioDecayPulse(t, 0.075f, 16f) + AudioDecayPulse(t, duration * 0.43f, 12f) * 0.60f;
                        body = sub * 0.34f + choir * 0.22f + vocal * 0.60f + grit * hinges * 0.76f;
                        body += Mathf.Sin(phase * 2.37f) * hinges * 0.13f;
                        break;
                    case "burst":
                        body = sub * 0.46f + fundamental * 0.19f + vocal * 0.46f + grit * 0.68f;
                        body += choir * AudioDecayPulse(t, 0.07f, 5f) * 0.35f;
                        break;
                    case "ascend":
                        body = choir * (0.42f + p * 0.28f) + vocal * 0.56f + sub * 0.22f;
                        body += Mathf.Sin(phase * 3.01f) * p * 0.15f;
                        envelope = Mathf.Pow(1f - p, 0.42f);
                        break;
                    case "tear":
                        float tear = Mathf.Exp(-t * 40f) + AudioDecayPulse(t, duration * 0.25f, 31f) * 0.66f;
                        body = vocal * 0.53f + grit * (0.50f + tear) + air * tear * 0.12f + sub * 0.20f;
                        body += choir * p * 0.32f;
                        break;
                    case "slam":
                        body = sub * 0.50f + fundamental * 0.16f + vocal * 0.48f + grit * 0.55f;
                        break;
                    case "whirl":
                        float blades = 0.35f + 0.65f * Mathf.Pow(Mathf.Sin(p * Mathf.PI * 3f), 4f);
                        body = (grit * 0.80f + air * 0.11f + vocal * 0.43f) * blades + sub * 0.26f;
                        break;
                    case "roar":
                        float growl = 0.68f + 0.32f * Mathf.Sin(t * 2f * Mathf.PI * 23f);
                        body = vocal * 0.88f + throat * growl * 0.22f + sub * 0.32f + grit * 0.34f;
                        envelope = Mathf.Pow(1f - p, 0.55f);
                        break;
                    case "choir":
                        body = choir * 0.70f + vocal * 0.42f + breath * 0.22f;
                        envelope *= Mathf.SmoothStep(0f, 1f, t / 0.035f);
                        break;
                    case "rumble":
                        body = fundamental * 0.43f + sub * 0.31f + lowNoise * 0.70f;
                        envelope *= Mathf.SmoothStep(0f, 1f, t / 0.045f);
                        break;
                    default:
                        body = choir * 0.35f + vocal * 0.40f + lowNoise * 0.65f;
                        envelope *= Mathf.SmoothStep(0f, 1f, t / 0.018f);
                        break;
                }
                float contact = impact ? Mathf.Exp(-t * 85f) * (grit * 0.72f + fundamental * 0.20f) : 0f;
                dry[i] = (body * envelope + contact) * gain * (compact ? 0.90f : 1.35f);
            }
            double mean = 0d;
            for (int i = 0; i < count; i++)
            {
                // Short, decorrelated reflections live inside a single voice rather than consuming extra sources.
                float diffuse = !compact && !gather ? DelayedDemonicSample(dry, i, 0.043f, rate) * 0.17f
                    - DelayedDemonicSample(dry, i, 0.071f, rate) * 0.10f
                    + DelayedDemonicSample(dry, i, 0.113f, rate) * 0.075f : 0f;
                output[i] = SoftLimitAudio(dry[i] + diffuse);
                mean += output[i];
            }
            float dc = (float)(mean / count);
            for (int i = 0; i < count; i++)
            {
                float attack = Mathf.Clamp01(i / (rate * (impact ? 0.0015f : 0.004f)));
                float release = Mathf.Clamp01((count - 1 - i) / (rate * 0.022f));
                output[i] = (output[i] - dc) * attack * release;
            }
            AudioClip clip = AudioClip.Create(key, count, 1, rate, false);
            clip.SetData(output, 0);
            return clip;
        }

        private static float DelayedDemonicSample(float[] samples, int index, float seconds, int rate)
        {
            int delayed = index - (int)(seconds * rate);
            return delayed >= 0 ? samples[delayed] : 0f;
        }
    }
}
