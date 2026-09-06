using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace AshenHalls.Editor
{
    // Runs the release checks without building or replacing a Windows package.
    public static class ProjectAuditSmoke
    {
        public static void RunRules() => Run(false);

        public static void RunFull() => Run(true);

        private static void Run(bool full)
        {
            if (!Application.isBatchMode)
                throw new InvalidOperationException("Project audit requires batch mode because runtime checks replace the open scene.");

            string suite = full ? "Full" : "Rules";
            try
            {
                Debug.Log("PROJECT AUDIT START: " + suite + " / " + VersionInfo.PackageVersion);
                Check("Rules", RuleSmokeTests.RunOrThrow);
                if (full)
                {
                    Check("Inventory and loot", InventoryLootExperienceSmoke.RunOrThrow);
                    Check("Sprite art", SpriteArtRuntimeSmoke.RunOrThrow);
                    Check("Combat UI", RuntimeBootSmoke.RunCombatUiOrThrow);
                    Check("Runtime boot", RuntimeBootSmoke.RunOrThrow);
                }
                Debug.Log("PROJECT AUDIT PASSED: " + suite);
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogError("PROJECT AUDIT FAILED: " + suite + "\n" + exception);
                EditorApplication.Exit(1);
            }
        }

        private static void Check(string name, Action check)
        {
            Stopwatch timer = Stopwatch.StartNew();
            Debug.Log("PROJECT AUDIT CHECK START: " + name);
            List<string> errors = new List<string>();
            Application.LogCallback captureError = (message, stack, type) =>
            {
                if (type == LogType.Error || type == LogType.Assert || type == LogType.Exception)
                    errors.Add(type + ": " + message);
            };
            Application.logMessageReceived += captureError;
            try
            {
                check();
            }
            finally
            {
                Application.logMessageReceived -= captureError;
            }
            if (errors.Count > 0)
                throw new InvalidOperationException(name + " logged " + errors.Count + " error(s):\n" + string.Join("\n", errors));
            Debug.Log("PROJECT AUDIT CHECK PASSED: " + name + " (" + timer.Elapsed.TotalSeconds.ToString("F1") + " seconds)");
        }
    }
}
