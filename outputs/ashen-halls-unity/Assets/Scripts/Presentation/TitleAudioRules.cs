using System;

namespace AshenHalls
{
    public enum TitleMenuAudioAction
    {
        Focus,
        Confirm,
        Open,
        Close,
        Blocked,
        Exit
    }

    public readonly struct TitleAudioCueProfile
    {
        public readonly string Key;
        public readonly float Volume;

        public TitleAudioCueProfile(string key, float volume)
        {
            Key = key ?? "";
            Volume = Math.Max(0f, Math.Min(1f, volume));
        }
    }

    public readonly struct TitleAmbienceProfile
    {
        public readonly string Key;
        public readonly float Volume;
        public readonly float Pan;
        public readonly float Pitch;

        public TitleAmbienceProfile(string key, float volume, float pan, float pitch)
        {
            Key = key ?? "";
            Volume = Math.Max(0f, Math.Min(1f, volume));
            Pan = Math.Max(-0.85f, Math.Min(0.85f, pan));
            Pitch = Math.Max(0.90f, Math.Min(1.10f, pitch));
        }
    }

    public static class TitleAudioRules
    {
        public const string RevealStrikeKey = "titleforge";
        public const string RevealChimeKey = "titlereveal";
        public const string FocusKey = "titlefocus";
        public const string ConfirmKey = "titleconfirm";
        public const string OpenKey = "titleopen";
        public const string CloseKey = "titleclose";
        public const string HearthAmbienceKey = "ambhearth";

        public const float TitleMusicSourceGain = 0.29f;
        public const float MusterMusicSourceGain = 0.23f;
        public const float CombatMusicSourceGain = 0.22f;
        public const float WorldMapMusicSourceGain = 0.18f;
        public const float StandardMusicSourceGain = 0.20f;

        public static TitleAudioCueProfile PresentationCue(string requestedKey, float fallbackVolume)
        {
            switch ((requestedKey ?? "").Trim().ToLowerInvariant())
            {
                case "impactlow": return new TitleAudioCueProfile(RevealStrikeKey, 0.28f);
                case "uiconfirm": return new TitleAudioCueProfile(RevealChimeKey, 0.22f);
                case "uitab": return MenuCue(TitleMenuAudioAction.Focus);
                default: return new TitleAudioCueProfile(requestedKey, fallbackVolume);
            }
        }

        public static TitleAudioCueProfile MenuCue(TitleMenuAudioAction action)
        {
            switch (action)
            {
                case TitleMenuAudioAction.Focus:
                    return new TitleAudioCueProfile(FocusKey, 0.20f);
                case TitleMenuAudioAction.Confirm:
                    return new TitleAudioCueProfile(ConfirmKey, 0.30f);
                case TitleMenuAudioAction.Open:
                    return new TitleAudioCueProfile(OpenKey, 0.26f);
                case TitleMenuAudioAction.Close:
                    return new TitleAudioCueProfile(CloseKey, 0.25f);
                case TitleMenuAudioAction.Blocked:
                    return new TitleAudioCueProfile("blocked", 0.42f);
                case TitleMenuAudioAction.Exit:
                    return new TitleAudioCueProfile(ConfirmKey, 0.34f);
                default:
                    return new TitleAudioCueProfile(FocusKey, 0.18f);
            }
        }

        public static float MusicSourceGain(GameMode mode, bool worldMapOpen = false)
        {
            if (mode == GameMode.Tavern) return TitleMusicSourceGain;
            if (mode == GameMode.Muster) return MusterMusicSourceGain;
            if (mode == GameMode.Combat) return CombatMusicSourceGain;
            if (mode == GameMode.Explore && worldMapOpen) return WorldMapMusicSourceGain;
            return StandardMusicSourceGain;
        }

        public static float InitialAmbienceDelay(GameMode mode, bool musicAudible)
        {
            if (mode == GameMode.Tavern) return musicAudible ? 7.5f : 1.5f;
            if (mode == GameMode.Muster) return 1.8f;
            return 2.8f;
        }

        public static float AmbienceInterval(GameMode mode, bool musicAudible, int sequence)
        {
            int step = Math.Max(0, sequence);
            if (mode == GameMode.Tavern && musicAudible) return 10.5f + (step % 4) * 1.15f;
            if (mode == GameMode.Tavern) return 3.8f + (step % 4) * 0.55f;
            return 3.2f + (step % 4) * 0.48f;
        }

        public static TitleAmbienceProfile Ambience(GameMode mode, bool musicAudible, int sequence)
        {
            int step = Math.Max(0, sequence);
            if (mode == GameMode.Tavern && musicAudible)
            {
                return step % 2 == 0
                    ? new TitleAmbienceProfile("ambtavern", 0.075f, -0.10f, 1f)
                    : new TitleAmbienceProfile(HearthAmbienceKey, 0.055f, -0.60f, 1f);
            }

            switch (step % 5)
            {
                case 0:
                case 3:
                    return new TitleAmbienceProfile("ambrain", mode == GameMode.Tavern ? 0.26f : 0.30f, 0.58f, 0.98f);
                case 1:
                case 4:
                    return new TitleAmbienceProfile("ambtavern", mode == GameMode.Tavern ? 0.22f : 0.26f, -0.08f, 1f);
                default:
                    return new TitleAmbienceProfile(HearthAmbienceKey, mode == GameMode.Tavern ? 0.18f : 0.22f, -0.62f, 1.02f);
            }
        }

        public static bool LocksPitch(string cueKey)
        {
            string key = (cueKey ?? "").Trim().ToLowerInvariant();
            return key == RevealStrikeKey
                || key == RevealChimeKey
                || key == FocusKey
                || key == ConfirmKey
                || key == OpenKey
                || key == CloseKey;
        }
    }
}
