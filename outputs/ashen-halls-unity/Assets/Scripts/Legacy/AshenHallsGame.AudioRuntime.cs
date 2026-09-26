using System;
using System.Collections.Generic;
using UnityEngine;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private void RebuildSfxVariantBank()
        {
            sfxClipVariants.Clear();
            foreach (KeyValuePair<string, AudioClip> pair in soundClips)
            {
                if (pair.Key.Contains("__v") || pair.Value == null) continue;
                List<AudioClip> variants = null;
                for (int index = 1; index < AudioRuntimeRules.MaximumCueVariants; index++)
                {
                    string variantKey = pair.Key + "__v" + index;
                    if (!soundClips.TryGetValue(variantKey, out AudioClip variant)
                        || variant == null || variant == pair.Value) continue;
                    // Imported masters are level-matched as a family. If an optional
                    // master is missing, retain the calibrated base, not a raw fallback.
                    if (importedSfxKeys.Contains(pair.Key) && !importedSfxKeys.Contains(variantKey)) continue;
                    if (variants != null && variants.Contains(variant)) continue;
                    if (variants == null) variants = new List<AudioClip> { pair.Value };
                    variants.Add(variant);
                }
                if (variants != null) sfxClipVariants[pair.Key] = variants.ToArray();
            }
        }

        private AudioClip ResolveSfxPlaybackClip(string key, int serial)
        {
            if (sfxClipVariants.TryGetValue(key, out AudioClip[] variants))
            {
                int index = AudioRuntimeRules.VariantIndex(StableAudioSeed(key), serial, variants.Length);
                if (variants[index] != null) return variants[index];
            }
            return soundClips[key];
        }

        private bool ScheduledSfxContextMatches(ScheduledSfxCue cue)
        {
            return state != null && ReferenceEquals(cue.State, state)
                && cue.Mode == state.Mode && ReferenceEquals(cue.Map, state.Map)
                && (cue.Mode != GameMode.Combat || ReferenceEquals(cue.Encounter, state.Combat));
        }

        private void StopSfxPlayback()
        {
            scheduledSfx.Clear();
            sfxVoicePlayback.Clear();
            if (audioSource != null) audioSource.Stop();
            foreach (AudioSource voice in sfxVoices)
                if (voice != null && voice != audioSource) voice.Stop();
            combatMusicDuckStartedAt = -1f;
            combatMusicDuckFullDepthAt = -1f;
            combatMusicDuckHoldUntil = -1f;
            combatMusicDuckUntil = -1f;
            combatMusicDuckDepth = 0f;
        }
    }
}
