using System;
using System.IO;
using UnityEngine;

namespace AshenHalls
{
    [Serializable]
    public sealed class GameSettingsData
    {
        public int SettingsVersion;
        public bool ReducedMotion;
        public bool SfxMuted;
        public bool MusicMuted;
        public int SfxVolumePercent = GameSettingsRules.DefaultSfxVolumePercent;
        public int MusicVolumePercent = GameSettingsRules.DefaultMusicVolumePercent;
    }

    public static class GameSettingsRules
    {
        public const int DefaultSfxVolumePercent = 100;
        public const int DefaultMusicVolumePercent = 65;
        public const int MinimumVolumePercent = 25;
        public const int MaximumVolumePercent = 100;

        public static GameSettingsData Capture(GameState state)
        {
            GameSettingsData settings = new GameSettingsData();
            if (state != null)
            {
                settings.ReducedMotion = state.ReducedMotion;
                settings.SfxMuted = state.SfxMuted;
                settings.MusicMuted = state.MusicMuted;
                settings.SfxVolumePercent = state.SfxVolumePercent;
                settings.MusicVolumePercent = state.MusicVolumePercent;
            }

            return Normalize(settings);
        }

        public static void Apply(GameSettingsData settings, GameState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            settings = Normalize(settings);
            state.ReducedMotion = settings.ReducedMotion;
            state.SfxMuted = settings.SfxMuted;
            state.MusicMuted = settings.MusicMuted;
            state.SfxVolumePercent = settings.SfxVolumePercent;
            state.MusicVolumePercent = settings.MusicVolumePercent;
        }

        public static GameSettingsData Normalize(GameSettingsData settings)
        {
            settings = settings ?? new GameSettingsData();
            settings.SettingsVersion = GameSettingsService.CurrentVersion;
            if (settings.SfxVolumePercent <= 0)
            {
                settings.SfxVolumePercent = DefaultSfxVolumePercent;
            }
            if (settings.MusicVolumePercent <= 0)
            {
                settings.MusicVolumePercent = DefaultMusicVolumePercent;
            }
            settings.SfxVolumePercent = Mathf.Clamp(
                settings.SfxVolumePercent,
                MinimumVolumePercent,
                MaximumVolumePercent);
            settings.MusicVolumePercent = Mathf.Clamp(
                settings.MusicVolumePercent,
                MinimumVolumePercent,
                MaximumVolumePercent);
            return settings;
        }

        public static bool IsLoadable(GameSettingsData settings)
        {
            return settings != null
                && settings.SettingsVersion >= 1
                && settings.SettingsVersion <= GameSettingsService.CurrentVersion;
        }
    }

    public static class GameSettingsService
    {
        private enum CandidateReadStatus
        {
            Missing,
            Loadable,
            Invalid,
            FutureVersion
        }

        public const int CurrentVersion = 1;
        private const string SettingsFileName = "AshAndBrimstoneSettings.json";

        public static string SettingsPath(string persistentDataPath)
        {
            if (string.IsNullOrWhiteSpace(persistentDataPath))
            {
                throw new ArgumentException("A persistent data path is required.", nameof(persistentDataPath));
            }
            return Path.Combine(persistentDataPath, SettingsFileName);
        }

        public static void Save(string path, GameSettingsData settings)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("A settings path is required.", nameof(path));
            }
            CandidateReadStatus primaryStatus = ReadCandidate(path, out _);
            CandidateReadStatus backupStatus = ReadCandidate(path + ".bak", out _);
            if (primaryStatus == CandidateReadStatus.FutureVersion
                || backupStatus == CandidateReadStatus.FutureVersion)
            {
                throw new InvalidDataException("Application settings were written by a newer version and were left unchanged.");
            }
            string contents = JsonUtility.ToJson(GameSettingsRules.Normalize(settings), true);
            if (primaryStatus == CandidateReadStatus.Invalid
                && backupStatus == CandidateReadStatus.Loadable)
            {
                WritePrimaryPreservingBackup(path, contents);
                return;
            }
            SaveService.WriteAllTextAtomic(path, contents);
        }

        public static bool Exists(string path)
        {
            return !string.IsNullOrWhiteSpace(path)
                && (File.Exists(path) || File.Exists(path + ".bak"));
        }

        public static bool TryLoad(string path, out GameSettingsData settings, out bool usedBackup)
        {
            settings = null;
            usedBackup = false;
            if (string.IsNullOrWhiteSpace(path)) return false;
            CandidateReadStatus primaryStatus = ReadCandidate(path, out settings);
            if (primaryStatus == CandidateReadStatus.Loadable) return true;
            if (ReadCandidate(path + ".bak", out settings) != CandidateReadStatus.Loadable) return false;
            usedBackup = true;
            if (primaryStatus != CandidateReadStatus.FutureVersion)
            {
                TryRepairPrimary(path, settings);
            }
            return true;
        }

        private static CandidateReadStatus ReadCandidate(string path, out GameSettingsData settings)
        {
            settings = null;
            try
            {
                if (!File.Exists(path)) return CandidateReadStatus.Missing;
                GameSettingsData candidate = JsonUtility.FromJson<GameSettingsData>(File.ReadAllText(path));
                if (candidate != null && candidate.SettingsVersion > CurrentVersion)
                {
                    return CandidateReadStatus.FutureVersion;
                }
                if (!GameSettingsRules.IsLoadable(candidate)) return CandidateReadStatus.Invalid;
                settings = GameSettingsRules.Normalize(candidate);
                return CandidateReadStatus.Loadable;
            }
            catch
            {
                return CandidateReadStatus.Invalid;
            }
        }

        private static void TryRepairPrimary(string path, GameSettingsData settings)
        {
            try
            {
                WritePrimaryPreservingBackup(
                    path,
                    JsonUtility.ToJson(GameSettingsRules.Normalize(settings), true));
            }
            catch
            {
                // Recovery already succeeded in memory; on-disk healing is best effort.
            }
        }

        private static void WritePrimaryPreservingBackup(string path, string contents)
        {
            SaveService.WriteAllTextAtomic(path, contents, true);
        }
    }
}
