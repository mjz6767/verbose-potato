using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class AudioRuntimeRobustnessSmoke
    {
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log("Audio runtime robustness smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError("Audio runtime robustness smoke failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            VariantCyclesStayComplete();
            using (Fixture fixture = new Fixture())
            {
                DelayedCuesRespectContext(fixture);
                SettingsCancelMutedTails(fixture);
                VariantsKeepSemanticIdentity(fixture);
                MusicRetargetPreservesEnvelopes(fixture);
            }
        }

        private static void VariantCyclesStayComplete()
        {
            for (int count = 2; count <= AudioRuntimeRules.MaximumCueVariants; count++)
            {
                for (uint seed = 0; seed < 96; seed++)
                {
                    HashSet<int> seen = new HashSet<int>();
                    int previous = -1;
                    for (int serial = 0; serial < count * 3; serial++)
                    {
                        int current = AudioRuntimeRules.VariantIndex(seed * 7919, serial, count);
                        Require(current >= 0 && current < count, "variant index remains in range");
                        Require(current != previous, "variant cycle never repeats an adjacent take");
                        if (serial < count) seen.Add(current);
                        previous = current;
                    }
                    Require(seen.Count == count, "every authored take is heard before the cycle repeats");
                }
            }
            Require(AudioRuntimeRules.VariantIndex(7, int.MaxValue, 3) < 3, "long sessions cannot overflow variant selection");
            Require(AudioRuntimeRules.IsScheduledCueExpired(2f, float.NaN), "invalid timestamps cannot hold the queue");
            Require(!AudioRuntimeRules.IsScheduledCueExpired(2.10f, 2f), "normal frame jitter retains cues");
            Require(AudioRuntimeRules.IsScheduledCueExpired(2.40f, 2f), "long stalls discard stale transients");
        }

        private static void DelayedCuesRespectContext(Fixture fixture)
        {
            GameState state = fixture.State;
            state.Mode = GameMode.Explore;
            fixture.Queue("servicecoin");
            fixture.Invoke("UpdateScheduledSfx");
            Require(fixture.QueueCount == 1, "noncombat service tail remains queued before its due time");
            fixture.MakeDue();
            fixture.Invoke("UpdateScheduledSfx");
            Require(fixture.QueueCount == 0 && fixture.Get<string>("lastSfxKey") == "servicecoin",
                "noncombat service tail reaches the runtime voice dispatcher");

            fixture.Queue("hit");
            state.Mode = GameMode.Tavern;
            fixture.Invoke("UpdateScheduledSfx");
            Require(fixture.QueueCount == 0, "mode changes discard the previous mode's delayed sound");
            state.Mode = GameMode.Explore;
            fixture.Queue("hit");
            state.Map = new MapData();
            fixture.Invoke("UpdateScheduledSfx");
            Require(fixture.QueueCount == 0, "interior/map changes discard a previous location's delayed sound");
            fixture.Queue("hit");
            fixture.Set("state", new GameState { Mode = GameMode.Explore, Map = state.Map });
            fixture.Invoke("UpdateScheduledSfx");
            Require(fixture.QueueCount == 0, "loading another state discards old delayed sounds");
            fixture.Set("state", state);

            state.Mode = GameMode.Combat;
            state.Combat = new CombatState();
            fixture.Queue("hit");
            state.Combat = new CombatState();
            fixture.Queue("servicecoin");
            Require(fixture.QueueCount == 1, "a new encounter can claim queue space without stale encounter cues");
            fixture.MakeDue();
            fixture.Invoke("UpdateScheduledSfx");
            Require(fixture.Get<string>("lastSfxKey") == "servicecoin", "only the current encounter's cue dispatches");

            state.Mode = GameMode.Explore;
            fixture.Queue("hit");
            fixture.MakeDue(AudioRuntimeRules.MaximumScheduledCueLateness + 0.10f);
            fixture.Set("lastSfxKey", "sentinel");
            fixture.Invoke("UpdateScheduledSfx");
            Require(fixture.QueueCount == 0 && fixture.Get<string>("lastSfxKey") == "sentinel",
                "resuming after a long stall does not burst obsolete sounds");
            fixture.Queue("hit");
            fixture.Set("showPauseMenu", true);
            fixture.Invoke("UpdateScheduledSfx");
            fixture.Queue("hit");
            Require(fixture.QueueCount == 0, "pausing discards delayed sounds and blocks new delayed sounds");
            fixture.Set("showPauseMenu", false);
        }

        private static void SettingsCancelMutedTails(Fixture fixture)
        {
            fixture.State.Mode = GameMode.Explore;
            fixture.Queue("servicecoin");
            fixture.Set("combatMusicDuckDepth", 0.3f);
            fixture.Invoke("ToggleSfxMute");
            Require(fixture.State.SfxMuted && fixture.QueueCount == 0, "SFX settings mute immediately cancels a waiting service tail");
            Require(fixture.Sfx.volume == 0f, "SFX mute silences the existing voice");
            Require(fixture.Get<IDictionary>("sfxVoicePlayback").Count == 0, "muted voices release their playback ownership");
            Require(fixture.Get<float>("combatMusicDuckDepth") == 0f, "muted attacks cannot keep ducking the score");
            fixture.Queue("hit");
            Require(fixture.QueueCount == 0, "muted scheduling cannot leave latent audio");
            fixture.Invoke("ToggleSfxMute");
            Require(!fixture.State.SfxMuted && fixture.QueueCount == 0, "unmuting cannot revive discarded tails");

            fixture.State.SfxVolumePercent = 0;
            fixture.State.MusicVolumePercent = 0;
            fixture.Invoke("ApplyAudioSettings");
            Near(fixture.Sfx.volume, 0.78f, "legacy zero SFX level still means the existing default");
            float expectedMusic = TitleAudioRules.MusicSourceGain(GameMode.Explore) * 0.65f;
            Near(fixture.Music.volume, expectedMusic, "legacy zero music level still means the existing default");
            fixture.State.SfxVolumePercent = 100;
            fixture.State.MusicVolumePercent = 65;
        }

        private static void VariantsKeepSemanticIdentity(Fixture fixture)
        {
            fixture.Invoke("RebuildSfxVariantBank");
            HashSet<AudioClip> heard = new HashSet<AudioClip>();
            AudioClip previous = null;
            for (int serial = 0; serial < 6; serial++)
            {
                fixture.Invoke("PlaySfxSpatial", "footstone", 0.5f, 0f, 1f, 1);
                AudioClip resolved = (AudioClip)fixture.Invoke("ResolveSfxPlaybackClip", "footstone", serial);
                Require(resolved != previous, "runtime variant bank rotates footstep recordings");
                heard.Add(resolved);
                previous = resolved;
                fixture.Invoke("PlaySfxSpatial", "hit", 0.3f, 0f, 1f, 1);
            }
            Require(heard.Count == 3, "runtime variant bank visits base recording and both authored alternatives");
            Require(fixture.Get<Dictionary<string, int>>("sfxCuePlaybackSerial")["footstone"] == 6,
                "interleaved combat/UI sounds cannot reset footstep variation");
            fixture.Invoke("PlaySfxSpatial", "footstone", 0.5f, 0f, 1f, 1);
            Require(fixture.Get<string>("lastSfxKey") == "footstone", "variant playback retains the semantic cue for ambience and diagnostics");
            Require((AudioClip)fixture.Invoke("ResolveSfxPlaybackClip", "hit", 9) == fixture.Clips["hit"],
                "a cue without alternatives retains its authored base clip");
            HashSet<string> imported = fixture.Get<HashSet<string>>("importedSfxKeys");
            imported.Add("footstone");
            fixture.Invoke("RebuildSfxVariantBank");
            for (int serial = 0; serial < 3; serial++)
                Require((AudioClip)fixture.Invoke("ResolveSfxPlaybackClip", "footstone", serial) == fixture.Clips["footstone"],
                    "missing imported variants keep the calibrated base instead of raw fallbacks");
            imported.Clear();
        }

        private static void MusicRetargetPreservesEnvelopes(Fixture fixture)
        {
            fixture.State.Mode = GameMode.Tavern;
            AudioClip first = fixture.Clips["hit"];
            AudioClip second = fixture.Clips["servicecoin"];
            AudioClip third = fixture.Clips["footstone"];
            foreach (float progress in new[] { 0f, 0.24f, 0.51f, 0.82f, 1f })
            {
                fixture.StageTransition(first, second, progress);
                AudioSource outgoing = fixture.Get<AudioSource>("musicSource");
                AudioSource incoming = fixture.Get<AudioSource>("musicFadeSource");
                float outgoingGain = outgoing.volume;
                float incomingGain = incoming.volume;
                float outgoingTime = outgoing.time;
                float incomingTime = incoming.time;
                fixture.Invoke("RetargetMusicTransition", first);
                fixture.Invoke("ApplyAudioSettings");
                Require(fixture.Get<AudioSource>("musicSource") == incoming && fixture.Get<AudioSource>("musicFadeSource") == outgoing,
                    "returning to the old route reverses live source roles");
                Near(outgoing.volume, outgoingGain, "outgoing clip preserves its gain at reversal");
                Near(incoming.volume, incomingGain, "incoming clip preserves its gain at reversal");
                Near(outgoing.time, outgoingTime, "reversal preserves the first playhead");
                Near(incoming.time, incomingTime, "reversal preserves the second playhead");

                fixture.StageTransition(first, second, progress);
                outgoing = fixture.Get<AudioSource>("musicSource");
                incoming = fixture.Get<AudioSource>("musicFadeSource");
                outgoingGain = outgoing.volume;
                incomingGain = incoming.volume;
                fixture.Invoke("RetargetMusicTransition", third);
                fixture.Invoke("ApplyAudioSettings");
                Require(outgoing.clip == first && incoming.clip == second, "third route waits for an available source without cutting either current clip");
                Near(outgoing.volume, outgoingGain, "third-route retarget preserves outgoing gain");
                Near(incoming.volume, incomingGain, "third-route retarget preserves incoming gain");
                float remaining = fixture.Get<float>("activeMusicTransitionDuration")
                    - (Time.unscaledTime - fixture.Get<float>("musicTransitionStartedAt"));
                Require(remaining <= AudioRuntimeRules.MusicRetargetSettleSeconds + 0.001f, "latest route gets a free source within the bounded settle window");
            }

            fixture.StageTransition(first, second, 0.45f);
            fixture.State.MusicMuted = true;
            fixture.Invoke("ApplyAudioSettings");
            Require(fixture.Music.volume == 0f && fixture.Fade.volume == 0f, "music mute silences both crossfade sources");
            Require(MusicTransitionRules.ShouldKeepTransportAlive(true, true, false, true), "muted music retains its transport contract");
            fixture.Invoke("RetargetMusicTransition", first);
            fixture.State.MusicMuted = false;
            fixture.Invoke("ApplyAudioSettings");
            float expected = TitleAudioRules.MusicSourceGain(GameMode.Tavern) * 0.65f;
            MusicCrossfadeGains gains = MusicTransitionRules.EqualPowerCrossfade(0.45f);
            Near(fixture.Music.volume, expected * gains.Outgoing, "unmuting a reversed fade restores the first clip's continuing envelope");
            Near(fixture.Fade.volume, expected * gains.Incoming, "unmuting a reversed fade restores the second clip's continuing envelope");
            fixture.Set("musicTransitionStartedAt", Time.unscaledTime - fixture.Get<float>("activeMusicTransitionDuration"));
            fixture.Invoke("CompleteMusicTransition");
            fixture.Invoke("ApplyAudioSettings");
            Require(fixture.Get<AudioSource>("musicSource").clip == first, "completion promotes the desired clip after reversal");
            Near(fixture.Get<AudioSource>("musicSource").volume, expected, "completed fade reaches the unchanged route gain");
            Require(fixture.Get<AudioSource>("musicFadeSource").clip == null, "completed fade releases its spare source");
        }

        private static void Near(float actual, float expected, string message)
        {
            Require(Math.Abs(actual - expected) < 0.002f, message + " (" + actual + " vs " + expected + ")");
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private sealed class Fixture : IDisposable
        {
            private readonly GameObject host;
            private readonly AshenHallsGame game;
            public readonly GameState State;
            public readonly AudioSource Sfx;
            public readonly AudioSource Music;
            public readonly AudioSource Fade;
            public readonly Dictionary<string, AudioClip> Clips;
            public int QueueCount => Get<IList>("scheduledSfx").Count;

            public Fixture()
            {
                host = new GameObject("Audio runtime robustness smoke host");
                host.SetActive(false);
                host.hideFlags = HideFlags.HideAndDontSave;
                game = host.AddComponent<AshenHallsGame>();
                Sfx = host.AddComponent<AudioSource>();
                Music = host.AddComponent<AudioSource>();
                Fade = host.AddComponent<AudioSource>();
                State = new GameState { Mode = GameMode.Explore, Map = new MapData() };
                Set("state", State);
                Set("visualSmokeSaveBlocked", true);
                Set("audioSource", Sfx);
                Set("musicSource", Music);
                Set("musicFadeSource", Fade);
                Get<List<AudioSource>>("sfxVoices").Add(Sfx);
                Clips = Get<Dictionary<string, AudioClip>>("soundClips");
                foreach (string key in new[] { "hit", "servicecoin", "footstone", "footstone__v1", "footstone__v2" })
                    Clips[key] = AudioClip.Create("smoke-" + key, 2205, 1, 22050, false);
            }

            public void Queue(string key)
            {
                Invoke("QueueSfx", key, 0.2f, 0.5f, 0f, 1f, 1);
            }

            public void MakeDue(float late = 0.01f)
            {
                IList queue = Get<IList>("scheduledSfx");
                for (int index = 0; index < queue.Count; index++)
                {
                    object cue = queue[index];
                    cue.GetType().GetField("PlayAt").SetValue(cue, Time.time - late);
                    queue[index] = cue;
                }
            }

            public void StageTransition(AudioClip first, AudioClip second, float progress)
            {
                Set("musicSource", Music);
                Set("musicFadeSource", Fade);
                Music.clip = first;
                Fade.clip = second;
                Set("musicTransitionActive", true);
                Set("activeMusicTransitionDuration", 1f);
                Set("musicTransitionStartedAt", Time.unscaledTime - progress);
                Invoke("ApplyAudioSettings");
            }

            public T Get<T>(string name) => (T)typeof(AshenHallsGame).GetField(name, Private).GetValue(game);
            public void Set(string name, object value) => typeof(AshenHallsGame).GetField(name, Private).SetValue(game, value);
            public object Invoke(string name, params object[] args) => typeof(AshenHallsGame).GetMethod(name, Private).Invoke(game, args);

            public void Dispose()
            {
                UnityEngine.Object.DestroyImmediate(host);
                foreach (AudioClip clip in Clips.Values) UnityEngine.Object.DestroyImmediate(clip);
            }
        }
    }
}
