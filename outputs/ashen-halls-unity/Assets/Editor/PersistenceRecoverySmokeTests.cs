using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class PersistenceRecoverySmokeTests
    {
        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " persistence recovery smoke tests passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " persistence recovery smoke tests failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            string root = Path.Combine(Path.GetTempPath(), "AshenHallsRecoverySmoke-" + Guid.NewGuid().ToString("N"));
            try
            {
                BackupRecoverySurvivesAnotherSave(Path.Combine(root, "recovered.json"));
                DirectSavePreservesValidBackup(Path.Combine(root, "invalid-json.json"), "{broken");
                GameState invalidParty = Campaign(90);
                invalidParty.Party.Clear();
                DirectSavePreservesValidBackup(Path.Combine(root, "invalid-party.json"), JsonUtility.ToJson(invalidParty));
                MissingPrimaryIsRepaired(Path.Combine(root, "missing.json"));
                FutureFilesArePreserved(Path.Combine(root, "future.json"));
                InvalidFilesCanBeReplaced(Path.Combine(root, "invalid-only.json"));
            }
            finally
            {
                if (Directory.Exists(root)) Directory.Delete(root, true);
            }
        }

        private static void BackupRecoverySurvivesAnotherSave(string path)
        {
            SaveService.SaveGameState(path, Campaign(1));
            SaveService.SaveGameState(path, Campaign(2));
            string goodBackup = File.ReadAllText(path + ".bak");
            File.WriteAllText(path, "{broken");
            GameState recovered = Load(path, out bool usedBackup);
            Check(usedBackup && recovered.Seed == 1, "corrupt campaign recovers its previous checkpoint");
            Check(File.ReadAllText(path + ".bak") == goodBackup, "healing leaves backup bytes unchanged");
            Check(Load(path, out usedBackup).Seed == 1 && !usedBackup, "recovery repairs the primary");
            SaveService.SaveGameState(path, Campaign(3));
            File.WriteAllText(path, "{broken-again");
            Check(Load(path, out usedBackup).Seed == 1 && usedBackup, "a save after recovery retains a usable backup");
        }

        private static void DirectSavePreservesValidBackup(string path, string invalidJson)
        {
            SaveService.SaveGameState(path, Campaign(10));
            SaveService.SaveGameState(path, Campaign(20));
            string goodBackup = File.ReadAllText(path + ".bak");
            File.WriteAllText(path, invalidJson);
            SaveService.SaveGameState(path, Campaign(30));
            Check(File.ReadAllText(path + ".bak") == goodBackup, "direct save does not rotate invalid primary over a healthy backup");
            Check(Load(path, out bool usedBackup).Seed == 30 && !usedBackup, "direct save writes the current campaign");
            File.WriteAllText(path, "{broken");
            Check(Load(path, out usedBackup).Seed == 10 && usedBackup, "preserved backup remains recoverable");
        }

        private static void MissingPrimaryIsRepaired(string path)
        {
            SaveService.SaveGameState(path, Campaign(40));
            SaveService.SaveGameState(path, Campaign(50));
            File.Delete(path);
            Check(Load(path, out bool usedBackup).Seed == 40 && usedBackup, "backup-only campaign loads");
            Check(Load(path, out usedBackup).Seed == 40 && !usedBackup, "backup-only recovery creates a primary");
        }

        private static void FutureFilesArePreserved(string path)
        {
            SaveService.SaveGameState(path, Campaign(60));
            SaveService.SaveGameState(path, Campaign(70));
            GameState future = Campaign(80);
            future.SaveVersion = VersionInfo.SaveVersion + 1;
            string futureJson = JsonUtility.ToJson(future);
            string compatibleBackup = File.ReadAllText(path + ".bak");
            File.WriteAllText(path, futureJson);
            Check(Load(path, out bool usedBackup).Seed == 60 && usedBackup, "future primary can fall back to a compatible campaign");
            Check(File.ReadAllText(path) == futureJson, "loading does not downgrade a future primary");
            ExpectProtectedSave(path);
            Check(File.ReadAllText(path) == futureJson && File.ReadAllText(path + ".bak") == compatibleBackup,
                "rejected save leaves future primary and compatible backup unchanged");

            File.WriteAllText(path, compatibleBackup);
            File.WriteAllText(path + ".bak", futureJson);
            Check(Load(path, out usedBackup).Seed == 60 && !usedBackup, "compatible primary still loads beside future backup");
            ExpectProtectedSave(path);
            Check(File.ReadAllText(path) == compatibleBackup && File.ReadAllText(path + ".bak") == futureJson,
                "rejected save leaves future backup and compatible primary unchanged");

            File.Delete(path);
            ExpectProtectedSave(path);
            Check(!File.Exists(path) && File.ReadAllText(path + ".bak") == futureJson,
                "older build cannot shadow a future backup-only campaign");
        }

        private static void InvalidFilesCanBeReplaced(string path)
        {
            File.WriteAllText(path, "{broken");
            File.WriteAllText(path + ".bak", "{also-broken");
            SaveService.SaveGameState(path, Campaign(100));
            Check(Load(path, out bool usedBackup).Seed == 100 && !usedBackup, "unrecoverable files do not prevent a fresh save");
        }

        private static GameState Campaign(int seed)
        {
            return new GameState
            {
                SaveVersion = VersionInfo.SaveVersion,
                Mode = GameMode.Tavern,
                Seed = seed,
                Depth = 1,
                Party = new List<PartyMember> { new PartyMember { Id = "recovery-hero", Name = "Maer" } }
            };
        }

        private static GameState Load(string path, out bool usedBackup)
        {
            return SaveService.LoadGameState(path,
                candidate => SaveCandidateRules.IsLoadable(candidate, VersionInfo.SaveVersion), out usedBackup);
        }

        private static void ExpectProtectedSave(string path)
        {
            try
            {
                SaveService.SaveGameState(path, Campaign(90));
            }
            catch (InvalidDataException)
            {
                return;
            }
            throw new InvalidOperationException("Expected future campaign files to block an older writer.");
        }

        private static void Check(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
