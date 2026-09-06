using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AshenHalls
{
    public static class SaveCandidateRules
    {
        public static bool IsLoadable(GameState candidate, int maximumSaveVersion)
        {
            if (candidate == null || candidate.SaveVersion < 17 || candidate.SaveVersion > maximumSaveVersion) return false;
            if (!Enum.IsDefined(typeof(GameMode), candidate.Mode)) return false;
            if (candidate.Party == null || candidate.Party.Count == 0) return false;
            for (int i = 0; i < candidate.Party.Count; i++)
            {
                if (candidate.Party[i] == null) return false;
            }

            bool hasMapPayload = HasMapPayload(candidate.Map);
            if (candidate.Mode == GameMode.Explore && !hasMapPayload) return false;
            if (hasMapPayload
                && !ExplorationSurfaceRules.IsLoadableMap(candidate.Map, candidate.SaveVersion >= 19)) return false;
            if (candidate.Mode == GameMode.Combat && !HasLoadableCombat(candidate)) return false;
            return true;
        }

        private static bool HasLoadableCombat(GameState candidate)
        {
            CombatState combat = candidate.Combat;
            if (combat == null
                || combat.Units == null
                || combat.Units.Count == 0
                || combat.Obstacles == null
                || !Enum.IsDefined(typeof(CombatPhase), combat.Phase))
            {
                return false;
            }

            HashSet<string> unitIds = new HashSet<string>(StringComparer.Ordinal);
            HashSet<int> partyIndexes = new HashSet<int>();
            int adoptablePartyCount = Math.Min(candidate.Party.Count, StarterPartyCatalog.ExpectedPartySize);
            HashSet<string> partyIds = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < adoptablePartyCount; i++)
            {
                string partyId = candidate.Party[i].Id;
                if (string.IsNullOrWhiteSpace(partyId)
                    || !string.Equals(partyId, partyId.Trim(), StringComparison.Ordinal)
                    || !partyIds.Add(partyId))
                {
                    return false;
                }
            }

            bool hasLivingUnit = false;
            for (int i = 0; i < combat.Units.Count; i++)
            {
                CombatUnit unit = combat.Units[i];
                if (unit == null
                    || string.IsNullOrWhiteSpace(unit.Id)
                    || !string.Equals(unit.Id, unit.Id.Trim(), StringComparison.Ordinal)
                    || !unitIds.Add(unit.Id)
                    || !Enum.IsDefined(typeof(UnitSide), unit.Side)
                    || unit.Skills == null)
                {
                    return false;
                }

                if (unit.Side == UnitSide.Party)
                {
                    if (unit.Summoned)
                    {
                        if (unit.PartyIndex >= 0) return false;
                    }
                    else if (unit.PartyIndex < 0
                        || unit.PartyIndex >= adoptablePartyCount
                        || !partyIndexes.Add(unit.PartyIndex)
                        || !string.Equals(unit.Id, candidate.Party[unit.PartyIndex].Id, StringComparison.Ordinal))
                    {
                        return false;
                    }
                }

                if (unit.Hp > 0) hasLivingUnit = true;
            }

            for (int i = 0; i < combat.Obstacles.Count; i++)
            {
                if (combat.Obstacles[i] == null) return false;
            }

            if (combat.InitiativeQueue != null)
            {
                HashSet<string> queuedIds = new HashSet<string>(StringComparer.Ordinal);
                foreach (string id in combat.InitiativeQueue)
                {
                    // Advancement locates the active unit's first queue entry.
                    // A duplicate would repeatedly return that same unit's turn.
                    if (!string.IsNullOrEmpty(id) && !queuedIds.Add(id)) return false;
                }
            }

            return hasLivingUnit;
        }

        private static bool HasMapPayload(MapData map)
        {
            return map != null
                && (map.Width > 0
                    || map.Height > 0
                    || (map.Tiles != null && map.Tiles.Count > 0)
                    || (map.Objects != null && map.Objects.Count > 0));
        }
    }

    public static class SaveService
    {
        private enum CandidateReadStatus
        {
            Missing,
            Loadable,
            Invalid,
            FutureVersion
        }

        private const string SaveFileName = "AshAndBrimstoneSaveV2.json";
        private const string LegacySaveFileName = "AshenHallsSaveV2.json";

        public static string SavePath(string persistentDataPath)
        {
            return Path.Combine(persistentDataPath, SaveFileName);
        }

        public static string LegacySavePath(string legacyPersistentDataPath)
        {
            return Path.Combine(legacyPersistentDataPath, LegacySaveFileName);
        }

        public static bool TryImportLegacySave(string currentPath, string legacyPath)
        {
            if (string.IsNullOrWhiteSpace(currentPath)
                || string.IsNullOrWhiteSpace(legacyPath)
                || string.Equals(
                    Path.GetFullPath(currentPath),
                    Path.GetFullPath(legacyPath),
                    StringComparison.OrdinalIgnoreCase)
                || SaveExists(currentPath)
                || !SaveExists(legacyPath))
            {
                return false;
            }

            string directory = Path.GetDirectoryName(currentPath);
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
            if (File.Exists(legacyPath)) File.Copy(legacyPath, currentPath, false);
            if (File.Exists(legacyPath + ".bak")) File.Copy(legacyPath + ".bak", currentPath + ".bak", false);
            return SaveExists(currentPath);
        }

        public static bool SaveExists(string path)
        {
            return !string.IsNullOrEmpty(path) && (File.Exists(path) || File.Exists(path + ".bak"));
        }

        public static void SaveGameState(string path, GameState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            CandidateReadStatus primaryStatus = ReadCampaignCandidate(path);
            CandidateReadStatus backupStatus = ReadCampaignCandidate(path + ".bak");
            if (primaryStatus == CandidateReadStatus.FutureVersion
                || backupStatus == CandidateReadStatus.FutureVersion)
            {
                throw new InvalidDataException("Saved campaign was written by a newer version and was left unchanged.");
            }
            bool preserveBackup = primaryStatus == CandidateReadStatus.Invalid
                && backupStatus == CandidateReadStatus.Loadable;
            WriteAllTextAtomic(path, JsonUtility.ToJson(state, true), preserveBackup);
        }

        public static bool TrySaveCampaignState(string path, GameState state, bool blockedByDeveloperLab, out string blockedReason)
        {
            blockedReason = "";
            if (blockedByDeveloperLab)
            {
                blockedReason = "Lab runs are not saved. Return to the tavern and start a normal campaign to save progress.";
                return false;
            }

            SaveGameState(path, state);
            return true;
        }

        public static GameState LoadGameState(string path, out bool usedBackup)
        {
            return LoadGameState(path, null, out usedBackup);
        }

        public static GameState LoadGameState(string path, Func<GameState, bool> validator, out bool usedBackup)
        {
            usedBackup = false;
            if (TryLoadGameState(path, validator, out GameState loaded, out Exception primaryFailure))
            {
                return loaded;
            }

            string backupPath = path + ".bak";
            if (TryLoadGameState(backupPath, validator, out loaded, out Exception backupFailure))
            {
                usedBackup = true;
                if (loaded.SaveVersion <= VersionInfo.SaveVersion
                    && ReadCampaignCandidate(path) != CandidateReadStatus.FutureVersion)
                {
                    try
                    {
                        WriteAllTextAtomic(path, JsonUtility.ToJson(loaded, true), true);
                    }
                    catch
                    {
                        // In-memory recovery remains usable even when on-disk healing fails.
                    }
                }
                return loaded;
            }

            if (!File.Exists(backupPath)) throw primaryFailure;
            throw new InvalidDataException(
                "Primary and backup save files are unreadable or invalid.",
                new AggregateException(primaryFailure, backupFailure));
        }

        private static bool TryLoadGameState(
            string path,
            Func<GameState, bool> validator,
            out GameState loaded,
            out Exception failure)
        {
            loaded = null;
            failure = null;
            try
            {
                GameState candidate = JsonUtility.FromJson<GameState>(File.ReadAllText(path));
                if (candidate == null || candidate.SaveVersion == default)
                {
                    throw new InvalidDataException("Save file parsed to an empty/default game state.");
                }
                if (validator != null && !validator(candidate))
                {
                    throw new InvalidDataException("Save file failed validation.");
                }

                loaded = candidate;
                return true;
            }
            catch (Exception ex)
            {
                failure = ex;
                return false;
            }
        }

        private static CandidateReadStatus ReadCampaignCandidate(string path)
        {
            if (!File.Exists(path)) return CandidateReadStatus.Missing;
            if (!TryLoadGameState(path, null, out GameState candidate, out _))
            {
                return CandidateReadStatus.Invalid;
            }
            if (candidate.SaveVersion > VersionInfo.SaveVersion) return CandidateReadStatus.FutureVersion;
            return SaveCandidateRules.IsLoadable(candidate, VersionInfo.SaveVersion)
                ? CandidateReadStatus.Loadable
                : CandidateReadStatus.Invalid;
        }

        internal static void WriteAllTextAtomic(string path, string contents, bool preserveBackup = false)
        {
            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);

            string tempPath = path + ".tmp";
            string backupPath = path + ".bak";
            try
            {
                File.WriteAllText(tempPath, contents);
                if (File.Exists(path))
                {
                    try
                    {
                        File.Replace(tempPath, path, preserveBackup ? null : backupPath, true);
                    }
                    catch
                    {
                        if (!preserveBackup) File.Copy(path, backupPath, true);
                        File.Copy(tempPath, path, true);
                        File.Delete(tempPath);
                    }
                }
                else
                {
                    File.Move(tempPath, path);
                }
            }
            catch
            {
                try
                {
                    if (File.Exists(tempPath)) File.Delete(tempPath);
                }
                catch
                {
                    // Best-effort cleanup only; keep the original exception.
                }

                throw;
            }
        }
    }
}
