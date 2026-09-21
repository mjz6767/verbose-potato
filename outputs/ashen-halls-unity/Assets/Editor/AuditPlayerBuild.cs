using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class AuditPlayerBuild
    {
        public static void Build()
        {
            if (!Application.isBatchMode)
                throw new InvalidOperationException("Audit player builds require batch mode.");
            try
            {
                string project = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
                string directory = Path.Combine(project, "QA", "audit-players",
                    DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid().ToString("N").Substring(0, 8));
                Directory.CreateDirectory(directory);
                string player = Path.Combine(directory, VersionInfo.ExecutableBaseName + ".exe");
                BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
                {
                    scenes = new[] { "Assets/Scenes/Main.unity" },
                    locationPathName = player,
                    target = BuildTarget.StandaloneWindows64,
                    options = BuildOptions.Development
                });
                if (report.summary.result != BuildResult.Succeeded)
                    throw new InvalidOperationException("Audit player build failed: " + report.summary.result);
                // Runtime atlases are intentionally external to Unity's data
                // folder. Use the release selection rules, otherwise an audit
                // player silently renders fallback art instead of the game.
                BuildWindows.CopyDocsFolder(project, directory);
                Debug.Log("AUDIT PLAYER BUILT: " + player);
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogError("AUDIT PLAYER BUILD FAILED: " + exception);
                EditorApplication.Exit(1);
            }
        }
    }
}
