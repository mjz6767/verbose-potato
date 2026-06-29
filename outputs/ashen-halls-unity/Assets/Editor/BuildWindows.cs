using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AshenHalls.Editor
{
    public static class BuildWindows
    {
        private const string PackageVersion = "v0.50.2";

        public static void Build()
        {
            PerformBuild();
        }

        public static void PerformBuild()
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string outputRoot = Path.GetFullPath(Path.Combine(projectRoot, "..", "ashen-halls-build", "AshenHalls-Windows-" + PackageVersion));
            string zipPath = Path.GetFullPath(Path.Combine(projectRoot, "..", "AshenHalls-Windows-" + PackageVersion + ".zip"));
            string exePath = Path.Combine(outputRoot, "AshenHalls.exe");
            Directory.CreateDirectory(outputRoot);
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "Scenes"));

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            GameObject cameraObject = new GameObject("Main Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.055f, 0.067f, 0.075f);
            camera.orthographic = true;
            camera.orthographicSize = 5f;
            cameraObject.tag = "MainCamera";
            cameraObject.AddComponent<AudioListener>();

            GameObject gameObject = new GameObject("Ashen Halls Runtime");
            gameObject.AddComponent<AshenHalls.AshenHallsGame>();

            string scenePath = "Assets/Scenes/Main.unity";
            EditorSceneManager.SaveScene(scene, scenePath);

            PlayerSettings.productName = "Ashen Halls";
            PlayerSettings.companyName = "High Desert Cosmos";
            PlayerSettings.defaultScreenWidth = 1920;
            PlayerSettings.defaultScreenHeight = 1080;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { scenePath },
                locationPathName = exePath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                Debug.LogError("Ashen Halls build failed: " + report.summary.result);
                EditorApplication.Exit(1);
                return;
            }

            CopyPackageNote(projectRoot, outputRoot, "README_PLAY.txt");
            CopyPackageNote(projectRoot, outputRoot, "CHANGELOG.md");
            CopyPackageNote(projectRoot, outputRoot, "KNOWN_ISSUES.txt");
            CopyDocsFolder(projectRoot, outputRoot);
            CopySiblingToolManifest(projectRoot, outputRoot);

            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }
            ZipFile.CreateFromDirectory(outputRoot, zipPath, System.IO.Compression.CompressionLevel.Optimal, false);

            Debug.Log("Ashen Halls Windows build complete: " + exePath);
            Debug.Log("Ashen Halls package complete: " + zipPath);
            EditorApplication.Exit(0);
        }

        private static void CopyPackageNote(string projectRoot, string outputRoot, string relativePath)
        {
            string source = Path.Combine(projectRoot, relativePath);
            if (!File.Exists(source))
            {
                return;
            }

            string destination = Path.Combine(outputRoot, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(destination));
            File.Copy(source, destination, true);
        }

        private static void CopyDocsFolder(string projectRoot, string outputRoot)
        {
            string docsRoot = Path.Combine(projectRoot, "Docs");
            if (!Directory.Exists(docsRoot))
            {
                return;
            }

            foreach (string source in Directory.GetFiles(docsRoot, "*.*", SearchOption.AllDirectories))
            {
                string relative = Path.Combine("Docs", Path.GetRelativePath(docsRoot, source));
                string destination = Path.Combine(outputRoot, relative);
                Directory.CreateDirectory(Path.GetDirectoryName(destination));
                File.Copy(source, destination, true);
            }
        }

        private static void CopySiblingToolManifest(string projectRoot, string outputRoot)
        {
            string source = Path.GetFullPath(Path.Combine(projectRoot, "..", "tools", "TOOLS_MANIFEST.md"));
            if (!File.Exists(source))
            {
                return;
            }

            string destination = Path.Combine(outputRoot, "Docs", "TOOL_DOWNLOADS.md");
            Directory.CreateDirectory(Path.GetDirectoryName(destination));
            File.Copy(source, destination, true);
        }
    }
}

namespace AshenHalls
{
    public static class BuildWindows
    {
        public static void Build()
        {
            AshenHalls.Editor.BuildWindows.Build();
        }
    }
}
