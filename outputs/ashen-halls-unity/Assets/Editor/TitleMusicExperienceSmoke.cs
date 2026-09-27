using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class TitleMusicExperienceSmoke
    {
        private const string Cue = "tavern_storm_hearth_ensemble_loop";
        private const string AssetPath = "Assets/Resources/Audio/Music/" + Cue + ".wav";
        private const float MainHookAt = 1.20f;
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log("Title music experience smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError("Title music experience smoke failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            AudioClip title = Resources.Load<AudioClip>("Audio/Music/" + Cue);
            Require(title != null && title.name == Cue, "the title Resources route resolves its authored master");
            Require(title.channels == 2 && title.frequency == 32000, "the imported title retains its stereo 32 kHz master");
            Near(title.length, 60f, 0.05f, "the imported title contains the complete sixty-second arrangement");
            Require(title.loadState != AudioDataLoadState.Failed, "the imported title has not failed audio decoding");
            AudioImporter importer = AssetImporter.GetAtPath(AssetPath) as AudioImporter;
            Require(importer != null && !importer.forceToMono, "the importer preserves the ensemble's stereo placement");
            Require(importer.defaultSampleSettings.loadType == AudioClipLoadType.CompressedInMemory
                && importer.defaultSampleSettings.preloadAudioData, "the title retains the established preloaded music transport");

            VerifyOpeningMix(title);
            VerifyMasterOpening();
        }

        private static void VerifyOpeningMix(AudioClip title)
        {
            GameObject host = new GameObject("Title music experience smoke host");
            host.SetActive(false);
            host.hideFlags = HideFlags.HideAndDontSave;
            AudioClip fallback = null;
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                AudioSource source = host.AddComponent<AudioSource>();
                AudioSource fadeSource = host.AddComponent<AudioSource>();
                source.loop = true;
                source.playOnAwake = false;
                fadeSource.loop = true;
                fadeSource.playOnAwake = false;
                GameState state = new GameState { Mode = GameMode.Tavern, MusicVolumePercent = 65 };
                Set(game, "state", state);
                Set(game, "visualSmokeSaveBlocked", true);
                Set(game, "musicSource", source);
                Set(game, "musicFadeSource", fadeSource);
                fallback = AudioClip.Create(Cue, 2205, 1, 22050, false);
                Set(game, "tavernMusicClip", fallback);
                Invoke(game, "LoadImportedMusicOverrides");
                Invoke(game, "ApplyImportedMusicOverrides");
                Require((AudioClip)Invoke(game, "DesiredMusicClip") == title,
                    "the live title director replaces its procedural fallback with the loaded title master");
                Require((AudioClip)Invoke(game, "MusicClipForKey", MusicDirectorRules.Title) == title,
                    "the named title route reaches the same imported master");
                Require(!(bool)Invoke(game, "IsStartupSplashVisible"), "a healthy launch does not play the opening behind a splash");
                Require((MusicTransitionContext)Invoke(game, "CurrentMusicTransitionContext") == MusicTransitionContext.Title,
                    "a title cold start selects the title-specific envelope");

                source.clip = title;
                Set(game, "musicIntroFadeClip", title);
                float intro = MusicTransitionRules.IntroFadeDurationFor(MusicTransitionContext.Title);
                Near(intro, 1f, 0.0001f, "the title cold-start fade gives the signature an audible entrance");
                Near(TitleAudioRules.MusicSourceGain(GameMode.Tavern), 0.29f, 0.0001f,
                    "the title retains its calibrated source gain");
                float fullGain = TitleAudioRules.MusicSourceGain(GameMode.Tavern) * 0.65f;
                foreach (float elapsed in new[] { 0f, 0.35f, 0.90f, MainHookAt })
                {
                    Set(game, "musicIntroFadeActive", true);
                    Set(game, "activeMusicIntroFadeDuration", intro);
                    Set(game, "musicIntroFadeStartedAt", Time.unscaledTime - elapsed);
                    Invoke(game, "ApplyAudioSettings");
                    float expected = fullGain * Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / intro));
                    Near(source.volume, expected, 0.001f, "live title envelope agrees at " + elapsed + " seconds");
                    if (elapsed >= 0.90f) Require(source.volume >= fullGain * 0.97f,
                        "the score is substantially audible before the main horn hook");
                }

                Set(game, "musicIntroFadeActive", true);
                Set(game, "musicIntroFadeStartedAt", Time.unscaledTime - 0.6f);
                float started = Get<float>(game, "musicIntroFadeStartedAt");
                Invoke(game, "ToggleTavernSettings");
                Invoke(game, "ToggleMusicMute");
                Require(state.MusicMuted && source.volume == 0f && fadeSource.volume == 0f,
                    "the title's live settings mute silences both music sources");
                Require(source.clip == title && Get<AudioClip>(game, "musicIntroFadeClip") == title
                    && Get<float>(game, "musicIntroFadeStartedAt") == started,
                    "opening settings and muting retain the imported title and its current envelope");
                Require(MusicTransitionRules.ShouldKeepTransportAlive(true, true, false, true),
                    "the muted title keeps the music transport alive");
                Invoke(game, "ToggleMusicMute");
                Require(!state.MusicMuted && Get<float>(game, "musicIntroFadeStartedAt") == started,
                    "unmuting the title does not restart its cold-start fade");
                Near(source.volume, fullGain * Mathf.SmoothStep(0f, 1f, 0.6f), 0.001f,
                    "unmuting restores the continuing title envelope");

                TitleAudioCueProfile strike = TitleAudioRules.PresentationCue("impactlow", 0.14f);
                TitleAudioCueProfile chime = TitleAudioRules.PresentationCue("uiconfirm", 0.16f);
                Near(strike.Volume, 0.20f, 0.0001f, "the forge accent leaves the signature pickup room");
                Near(chime.Volume, 0.14f, 0.0001f, "the reveal chime supports rather than masks the theme");
                AudioClip reveal = Resources.Load<AudioClip>("Audio/Sfx/" + chime.Key);
                Require(reveal != null && TitleScreenPresentationRules.RevealChimeAt + reveal.length <= MainHookAt,
                    "the authored reveal chime clears before the main horn statement");
                Require(TitleAudioRules.InitialAmbienceDelay(GameMode.Tavern, true) > 6f,
                    "extra tavern ambience waits for the opening theme statement to finish");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(host);
                if (fallback != null) UnityEngine.Object.DestroyImmediate(fallback);
            }
        }

        private static void VerifyMasterOpening()
        {
            // CompressedInMemory clips do not promise GetData access. Inspect the
            // exact PCM master imported above without changing its import settings.
            string path = Path.Combine(Application.dataPath, "Resources/Audio/Music/" + Cue + ".wav");
            float[] samples = ReadPcm16Stereo(path, out int sampleRate);
            Require(sampleRate == 32000 && samples.Length == 60 * sampleRate * 2,
                "the source master exactly matches the imported sixty-second stereo arrangement");
            float peak = 0f;
            foreach (float sample in samples) peak = Math.Max(peak, Math.Abs(sample));
            Require(peak < 0.95f && peak > 0.1f, "the title master has audible body and leaves output headroom");
            double pickup = RuntimeRms(samples, sampleRate, 0.35f, 0.90f, false);
            double opening = RuntimeRms(samples, sampleRate, 0.90f, 2f, false);
            double hook = RuntimeRms(samples, sampleRate, MainHookAt, 6f, false);
            double monoHook = RuntimeRms(samples, sampleRate, MainHookAt, 6f, true);
            Require(pickup > 0.002, "the signature pickup is audible through the real cold-start fade");
            Require(opening > 0.006, "the actual master has audible music immediately after 0.9 seconds");
            Require(hook > 0.010, "the opening theme statement has a readable body at default game volume");
            Require(monoHook >= hook * 0.70, "the opening hook survives mono laptop playback without phase cancellation");
            Debug.Log("Title opening runtime RMS: pickup=" + pickup.ToString("F5")
                + ", 0.9–2s=" + opening.ToString("F5") + ", hook=" + hook.ToString("F5")
                + ", mono hook=" + monoHook.ToString("F5"));
        }

        private static double RuntimeRms(float[] samples, int rate, float start, float end, bool mono)
        {
            int first = (int)(start * rate);
            int last = Math.Min(samples.Length / 2, (int)(end * rate));
            float duration = MusicTransitionRules.IntroFadeDurationFor(MusicTransitionContext.Title);
            double energy = 0d;
            for (int frame = first; frame < last; frame++)
            {
                float progress = Mathf.Clamp01(frame / (float)rate / duration);
                float gain = TitleAudioRules.MusicSourceGain(GameMode.Tavern) * 0.65f * Mathf.SmoothStep(0f, 1f, progress);
                double left = samples[frame * 2] * gain;
                double right = samples[frame * 2 + 1] * gain;
                energy += mono ? Math.Pow((left + right) * 0.5, 2) : (left * left + right * right) * 0.5;
            }
            return Math.Sqrt(energy / Math.Max(1, last - first));
        }

        private static float[] ReadPcm16Stereo(string path, out int sampleRate)
        {
            sampleRate = 0;
            using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
            {
                Require(Encoding.ASCII.GetString(reader.ReadBytes(4)) == "RIFF", "title master has a WAV container");
                reader.ReadUInt32();
                Require(Encoding.ASCII.GetString(reader.ReadBytes(4)) == "WAVE", "title master has the expected WAV format");
                bool formatValid = false;
                while (reader.BaseStream.Position + 8 <= reader.BaseStream.Length)
                {
                    string id = Encoding.ASCII.GetString(reader.ReadBytes(4));
                    uint size = reader.ReadUInt32();
                    long next = reader.BaseStream.Position + size + (size & 1);
                    Require(next <= reader.BaseStream.Length, "title master chunk remains inside its file");
                    if (id == "fmt ")
                    {
                        Require(size >= 16, "title master contains a complete PCM format");
                        ushort format = reader.ReadUInt16();
                        ushort channels = reader.ReadUInt16();
                        sampleRate = reader.ReadInt32();
                        reader.ReadUInt32();
                        reader.ReadUInt16();
                        ushort bits = reader.ReadUInt16();
                        formatValid = format == 1 && channels == 2 && bits == 16;
                    }
                    else if (id == "data")
                    {
                        Require(formatValid && size % 4 == 0, "title master contains interleaved sixteen-bit stereo PCM");
                        float[] samples = new float[size / 2];
                        for (int index = 0; index < samples.Length; index++) samples[index] = reader.ReadInt16() / 32768f;
                        return samples;
                    }
                    reader.BaseStream.Position = next;
                }
            }
            throw new InvalidOperationException("Title master contains no PCM data chunk.");
        }

        private static object Invoke(AshenHallsGame game, string name, params object[] args)
            => typeof(AshenHallsGame).GetMethod(name, Private).Invoke(game, args);
        private static T Get<T>(AshenHallsGame game, string name)
            => (T)typeof(AshenHallsGame).GetField(name, Private).GetValue(game);
        private static void Set(AshenHallsGame game, string name, object value)
            => typeof(AshenHallsGame).GetField(name, Private).SetValue(game, value);
        private static void Near(float actual, float expected, float tolerance, string message)
            => Require(Math.Abs(actual - expected) <= tolerance, message + " (" + actual + " vs " + expected + ")");
        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
