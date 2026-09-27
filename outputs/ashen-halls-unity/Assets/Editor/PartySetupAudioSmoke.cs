using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class PartySetupAudioSmoke
    {
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

        public static void Run()
        {
            try { RunOrThrow(); Debug.Log("Party setup audio smoke passed."); EditorApplication.Exit(0); }
            catch (Exception ex) { Debug.LogError("Party setup audio smoke failed: " + ex); EditorApplication.Exit(1); }
        }

        public static void RunOrThrow()
        {
            EveryChoiceHasItsOwnCue();
            ImportedMastersAreAudibleAndClean();
            RuntimeMixHonorsPreferences();
        }

        private static void EveryChoiceHasItsOwnCue()
        {
            var keys = new HashSet<string>(PartySetupAudioRules.CueKeys, StringComparer.Ordinal);
            Require(keys.Count == 23, "workshop bank contains 23 unique cues");
            var selected = new HashSet<string>(StringComparer.Ordinal);
            foreach (CharacterCreationChoice race in CharacterCreationCatalog.Races)
                Require(selected.Add(PartySetupAudioRules.CueFor(PartySetupAudioAction.Race, race.Key).Key), "each race has a distinct sound");
            foreach (CharacterCreationChoice role in CharacterCreationCatalog.Classes)
                Require(selected.Add(PartySetupAudioRules.CueFor(PartySetupAudioAction.Class, role.Key).Key), "each class has a distinct sound");
            foreach (PartySetupAudioAction action in Enum.GetValues(typeof(PartySetupAudioAction)))
            {
                if (action == PartySetupAudioAction.Race || action == PartySetupAudioAction.Class) continue;
                TitleAudioCueProfile cue = PartySetupAudioRules.CueFor(action);
                Require(cue.Volume >= .20f && cue.Volume <= .40f, "interaction cue preserves restrained mix headroom");
                Require(selected.Add(cue.Key), "each workshop action has a distinct cue");
            }
            Require(keys.SetEquals(selected), "the registered bank covers every reachable workshop action exactly");
            Require(PartySetupAudioRules.CueFor(PartySetupAudioAction.Race, "missing").Key == "", "unknown race is silent");
            Require(PartySetupAudioRules.CueFor(PartySetupAudioAction.Class, "missing").Key == "", "unknown class is silent");
            foreach (GameMode mode in Enum.GetValues(typeof(GameMode)))
                Require(PartySetupAudioRules.ShouldPlayCue(mode, false, 100, false) == (mode == GameMode.Muster), "workshop cue stays in its own screen");
            Require(!PartySetupAudioRules.ShouldPlayCue(GameMode.Muster, true, 100, false), "SFX mute suppresses workshop cues");
            Require(!PartySetupAudioRules.ShouldPlayCue(GameMode.Muster, false, 0, false), "zero volume suppresses workshop cues");
            Require(!PartySetupAudioRules.ShouldPlayCue(GameMode.Muster, false, 100, true), "visual capture does not emit workshop cues");
        }

        private static void ImportedMastersAreAudibleAndClean()
        {
            var signatures = new HashSet<ulong>();
            foreach (string key in PartySetupAudioRules.CueKeys)
            {
                AudioClip clip = Resources.Load<AudioClip>("Audio/Sfx/" + key);
                Require(clip != null && clip.frequency == 48000 && clip.channels == 1, key + " imports as a mono 48 kHz master");
                Require(clip.length >= .20f && clip.length <= 1f, key + " remains a short immediate interaction");
                float[] pcm = ReadPcm(clip);
                double squareSum = 0;
                float peak = 0;
                ulong signature = 14695981039346656037UL;
                foreach (float sample in pcm)
                {
                    Require(!float.IsNaN(sample) && !float.IsInfinity(sample), key + " contains only finite samples");
                    peak = Mathf.Max(peak, Mathf.Abs(sample));
                    squareSum += sample * sample;
                    unchecked { signature = (signature ^ (uint)(int)Math.Round(sample * 32768)) * 1099511628211UL; }
                }
                Require(peak < .42f && Math.Sqrt(squareSum / pcm.Length) > .01, key + " is audible with ample peak headroom");
                Require(Mathf.Abs(pcm[0]) < .0001f && Mathf.Abs(pcm[pcm.Length - 1]) < .0001f, key + " has no boundary click");
                Require(signatures.Add(signature), key + " has an independently authored waveform");
            }
            AudioClip music = Resources.Load<AudioClip>("Audio/Music/" + PartySetupAudioRules.MusicCue);
            Require(music != null && music.frequency == 32000 && music.channels == 2, "workshop score imports in stereo at 32 kHz");
            Require(Mathf.Abs(music.length - 53.333333f) < .002f, "workshop score retains all 16 bars");
            Require(music.loadType == AudioClipLoadType.CompressedInMemory, "workshop music retains its memory-efficient runtime import");
            // Compressed music does not promise GetData access. Validate its source
            // master without changing the shipped import settings to satisfy a test.
            string path = Path.Combine(Application.dataPath, "Resources/Audio/Music/" + PartySetupAudioRules.MusicCue + ".wav");
            float[] samples = ReadMusicMaster(path, out int rate);
            Require(rate == 32000 && samples.Length == 1706667 * 2, "PCM master matches the full imported stereo arrangement");
            double squares = 0;
            float musicPeak = 0;
            foreach (float sample in samples)
            {
                Require(!float.IsNaN(sample) && !float.IsInfinity(sample), "workshop music contains finite PCM");
                squares += sample * sample;
                musicPeak = Mathf.Max(musicPeak, Mathf.Abs(sample));
            }
            double rms = Math.Sqrt(squares / samples.Length);
            Require(rms > .06 && musicPeak < .60f, "score balances audible body and interaction headroom");
            Require(rms * TitleAudioRules.MusterMusicSourceGain * .65f > .008, "default music bus keeps the score above the authored audibility floor");
            Require(Mathf.Abs(samples[0] - samples[samples.Length - 2]) < .001f
                && Mathf.Abs(samples[1] - samples[samples.Length - 1]) < .001f, "both music channels meet cleanly at the loop seam");
        }

        private static void RuntimeMixHonorsPreferences()
        {
            var host = new GameObject("Party setup audio smoke host");
            host.SetActive(false);
            host.hideFlags = HideFlags.HideAndDontSave;
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                AudioSource sfx = host.AddComponent<AudioSource>();
                AudioSource music = host.AddComponent<AudioSource>();
                AudioSource fade = host.AddComponent<AudioSource>();
                var state = new GameState { Mode = GameMode.Muster, MusicVolumePercent = 65, SfxVolumePercent = 100 };
                Set(game, "state", state);
                Set(game, "visualSmokeSaveBlocked", true);
                Set(game, "audioSource", sfx);
                Set(game, "musicSource", music);
                Set(game, "musicFadeSource", fade);
                Invoke(game, "ApplyAudioSettings");
                Near(music.volume, .23f * .65f, "default workshop music preference reaches the actual source");
                Near(sfx.volume, .78f, "default interaction preference reaches the actual source");
                state.MusicVolumePercent = 25;
                state.SfxVolumePercent = 25;
                Invoke(game, "ApplyAudioSettings");
                Near(music.volume, .23f * .25f, "music slider scales the workshop score");
                Near(sfx.volume, .78f * .25f, "SFX slider scales workshop interactions");
                state.MusicMuted = state.SfxMuted = true;
                Invoke(game, "ApplyAudioSettings");
                Near(music.volume, 0f, "music mute is respected");
                Near(sfx.volume, 0f, "SFX mute is respected");
                foreach (string key in PartySetupAudioRules.CueKeys)
                    Near((float)Invoke(game, "SfxPlaybackPitchVariation", key, 7), 1f, "workshop cues preserve authored pitch");
                Require((MusicTransitionContext)Invoke(game, "CurrentMusicTransitionContext") == MusicTransitionContext.Title, "muster shares the hearth transition context");
                Near(MusicTransitionRules.TitleTransitionDuration, 1.35f, "title-to-workshop crossfade remains intact");
                Near(MusicTransitionRules.TitleIntroFadeDuration, 1f, "cold hearth entry reaches its melody within one second");
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        private static float[] ReadPcm(AudioClip clip)
        {
            clip.LoadAudioData();
            var pcm = new float[clip.samples * clip.channels];
            Require(clip.GetData(pcm, 0), clip.name + " exposes its imported PCM for validation");
            return pcm;
        }

        private static float[] ReadMusicMaster(string path, out int rate)
        {
            rate = 0;
            using (var reader = new BinaryReader(File.OpenRead(path)))
            {
                Require(Encoding.ASCII.GetString(reader.ReadBytes(4)) == "RIFF", "music master uses a WAV container");
                reader.ReadUInt32();
                Require(Encoding.ASCII.GetString(reader.ReadBytes(4)) == "WAVE", "music master has a WAVE header");
                bool validFormat = false;
                while (reader.BaseStream.Position + 8 <= reader.BaseStream.Length)
                {
                    string chunk = Encoding.ASCII.GetString(reader.ReadBytes(4));
                    uint size = reader.ReadUInt32();
                    long next = reader.BaseStream.Position + size + (size & 1);
                    Require(next <= reader.BaseStream.Length, "music chunk fits its container");
                    if (chunk == "fmt ")
                    {
                        Require(size >= 16, "music has a complete PCM format");
                        ushort format = reader.ReadUInt16();
                        ushort channels = reader.ReadUInt16();
                        rate = reader.ReadInt32();
                        reader.ReadUInt32();
                        reader.ReadUInt16();
                        validFormat = format == 1 && channels == 2 && reader.ReadUInt16() == 16;
                    }
                    else if (chunk == "data")
                    {
                        Require(validFormat && size % 4 == 0, "music contains interleaved sixteen-bit stereo PCM");
                        var result = new float[size / 2];
                        for (int i = 0; i < result.Length; i++) result[i] = reader.ReadInt16() / 32768f;
                        return result;
                    }
                    reader.BaseStream.Position = next;
                }
            }
            throw new InvalidOperationException("Workshop music master contains no PCM data chunk.");
        }

        private static void Set(AshenHallsGame game, string name, object value) => typeof(AshenHallsGame).GetField(name, Private).SetValue(game, value);
        private static object Invoke(AshenHallsGame game, string name, params object[] args) => typeof(AshenHallsGame).GetMethod(name, Private).Invoke(game, args);
        private static void Near(float actual, float expected, string message) => Require(Mathf.Abs(actual - expected) < .0001f, message);
        private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
    }
}
