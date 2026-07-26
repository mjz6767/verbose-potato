using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class BuildWindows
    {
        private const int PackagedArtVersionsPerPrefix = 1;
        private const string BuildIconAssetPath =
            "Assets/Editor/Branding/ash-and-brimstone-icon-v1.61.0.png";

        private static readonly string[] PackagedArtPrefixes =
        {
            "ashen-halls-title-card-runtime-",
            "ash-and-brimstone-title-card-runtime-",
            "ashen-halls-icon-runtime-",
            "ash-and-brimstone-icon-runtime-",
            "magic-ui-atlas-runtime-",
            "spellbook-open-runtime-",
            "combat-sprite-sheet-alpha-",
            "class-icon-atlas-runtime-",
            "world-environment-atlas-runtime-",
            "item-inventory-atlas-runtime-",
            "item-equipment-atlas-runtime-",
            "item-icon-atlas-runtime-",
            "enemy-roster-atlas-runtime-",
            "combat-ui-atlas-runtime-",
            "combat-ui-panel-atlas-runtime-",
            "spellbook-combat-ui-atlas-runtime-",
            "signature-spell-icon-atlas-runtime-",
            "lightning-spell-icon-atlas-runtime-",
            "ember-spell-effects-atlas-runtime-",
            "combat-spell-effects-atlas-runtime-",
            "spell-animation-atlas-runtime-",
            "combat-spellbook-ui-atlas-runtime-",
            "pact-spellbook-atlas-runtime-",
            "boss-enemy-atlas-runtime-",
            "quest-world-object-atlas-runtime-",
            "world-map-prop-atlas-runtime-",
            "world-map-biome-prop-atlas-runtime-",
            "world-map-exploration-tile-atlas-runtime-",
            "world-map-material-atlas-runtime-",
            "world-map-landmark-atlas-runtime-",
            "world-map-region-landmark-atlas-runtime-",
            "world-map-overlay-atlas-runtime-",
            "world-map-progression-overlay-atlas-runtime-",
            "world-map-ui-atlas-runtime-",
            "world-map-token-sprite-atlas-runtime-",
            "story-card-atlas-runtime-",
            "npc-portrait-atlas-runtime-",
            "route-scaffold-atlas-runtime-",
            "dungeon-scaffold-atlas-runtime-",
            "faction-banner-atlas-runtime-",
            "service-scaffold-atlas-runtime-",
            "character-inventory-ui-atlas-runtime-",
            "unique-item-atlas-runtime-",
            "combat-hud-ui-atlas-runtime-",
            "combat-spell-float-atlas-runtime-",
            "enemy-world-object-atlas-runtime-",
            "roaming-threat-atlas-runtime-",
            "tavern-backdrop-runtime-",
            "tavern-ui-atlas-runtime-",
            "inventory-consumable-atlas-runtime-",
            "combat-command-icon-atlas-runtime-",
            "ability-icon-atlas-runtime-",
            "ranger-ability-effect-atlas-runtime-",
            "enemy-sprite-atlas-runtime-",
            "character-combat-atlas-runtime-",
            "combat-sprite-atlas-runtime-",
            "demon-summon-atlas-runtime-",
            "combat-terrain-atlas-runtime-",
            "kobold-combat-terrain-atlas-runtime-",
            "kobold-route-atlas-runtime-",
            "kobold-boss-atlas-runtime-",
            "kobold-cave-prop-atlas-runtime-",
            "midgaard-town-atlas-runtime-",
            "midgaard-tile-atlas-runtime-",
            "midgaard-wall-atlas-runtime-",
            "midgaard-gate-atlas-runtime-",
            "midgaard-city-prop-atlas-runtime-",
            "midgaard-street-life-atlas-runtime-",
            "midgaard-paving-decal-atlas-runtime-",
            "midgaard-interior-prop-atlas-runtime-",
            "midgaard-interior-tile-atlas-runtime-",
            "midgaard-npc-atlas-runtime-",
            "midgaard-sewer-atlas-runtime-"
        };

        public static void Build()
        {
            PerformBuild();
        }

        public static void PerformBuild()
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string packageVersion = AshenHalls.VersionInfo.PackageVersion;
            ValidateReleaseDocs(projectRoot, packageVersion);
            ValidateApprovedRuntimeArtIsLatest(projectRoot);
            RuleSmokeTests.RunOrThrow();
            Debug.Log(VersionInfo.ProductName + " build rule smoke tests passed.");
            RuntimeBootSmoke.RunOrThrow();
            Debug.Log(VersionInfo.ProductName + " build runtime boot smoke passed.");

            string outputRoot = Path.GetFullPath(Path.Combine(projectRoot, "..", "ash-and-brimstone-build", VersionInfo.ExecutableBaseName + "-Windows-" + packageVersion));
            string zipPath = Path.GetFullPath(Path.Combine(projectRoot, "..", VersionInfo.ExecutableBaseName + "-Windows-" + packageVersion + ".zip"));
            string exePath = Path.Combine(outputRoot, VersionInfo.ExecutableBaseName + ".exe");
            if (Directory.Exists(outputRoot))
            {
                Directory.Delete(outputRoot, true);
            }
            Directory.CreateDirectory(outputRoot);

            string scenePath = "Assets/Scenes/Main.unity";
            string sceneFullPath = Path.Combine(projectRoot, scenePath);
            if (!File.Exists(sceneFullPath))
            {
                throw new BuildFailedException("Build scene is missing: " + scenePath);
            }

            PlayerSettings.productName = VersionInfo.ProductName;
            PlayerSettings.companyName = "High Desert Cosmos";
            PlayerSettings.defaultScreenWidth = 2048;
            PlayerSettings.defaultScreenHeight = 1152;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
            Texture2D buildIcon = LoadBuildIcon();
            if (buildIcon != null)
            {
                int slotCount = PlayerSettings
                    .GetIconSizes(NamedBuildTarget.Standalone, IconKind.Application)
                    .Length;
                if (slotCount > 0)
                {
                    PlayerSettings.SetIcons(
                        NamedBuildTarget.Standalone,
                        Enumerable.Repeat(buildIcon, slotCount).ToArray(),
                        IconKind.Application);
                }
                else
                {
                    Debug.LogWarning(
                        VersionInfo.ProductName + " could not find Windows application icon slots.");
                }
            }
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
                Debug.LogError(VersionInfo.ProductName + " build failed: " + report.summary.result);
                EditorApplication.Exit(1);
                return;
            }

            CopyPackageNote(projectRoot, outputRoot, "README_PLAY.txt");
            CopyPackageNote(projectRoot, outputRoot, "CHANGELOG.md");
            CopyPackageNote(projectRoot, outputRoot, "KNOWN_ISSUES.txt");
            CopyDocsFolder(projectRoot, outputRoot);
            CopySiblingToolManifest(projectRoot, outputRoot);
            WritePackageHint(outputRoot, zipPath);

            Debug.Log(VersionInfo.ProductName + " Windows build complete: " + exePath);
            Debug.Log(VersionInfo.ProductName + " package staging complete: " + outputRoot);
            Debug.Log("Create the distributable zip after Unity exits: " + zipPath);
            EditorApplication.Exit(0);
        }

        private static Texture2D LoadBuildIcon()
        {
            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(BuildIconAssetPath);
            if (icon == null)
            {
                AssetDatabase.ImportAsset(
                    BuildIconAssetPath,
                    ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
                icon = AssetDatabase.LoadAssetAtPath<Texture2D>(BuildIconAssetPath);
            }

            if (icon == null)
            {
                Debug.LogWarning(
                    VersionInfo.ProductName + " build icon is missing or could not be imported: " +
                    BuildIconAssetPath);
                return null;
            }

            return icon;
        }

        private static void ValidateReleaseDocs(string projectRoot, string packageVersion)
        {
            string[] requiredDocs =
            {
                "README_PLAY.txt",
                "CHANGELOG.md",
                "KNOWN_ISSUES.txt"
            };

            foreach (string relativePath in requiredDocs)
            {
                string path = Path.Combine(projectRoot, relativePath);
                if (!File.Exists(path))
                {
                    throw new BuildFailedException("Required release doc is missing: " + relativePath);
                }

                string text = File.ReadAllText(path);
                if (!text.Contains(packageVersion))
                {
                    throw new BuildFailedException(relativePath + " does not mention " + packageVersion + ".");
                }
            }

            string thirdPartyNoticesPath = Path.Combine(projectRoot, "Docs", "THIRD_PARTY_NOTICES.txt");
            string fontLicensePath = Path.Combine(projectRoot, "Docs", "OFL-LibreBaskerville.txt");
            if (!File.Exists(thirdPartyNoticesPath)
                || !File.ReadAllText(thirdPartyNoticesPath).Contains("Libre Baskerville"))
            {
                throw new BuildFailedException("Third-party notices must identify the bundled Libre Baskerville fonts.");
            }
            if (!File.Exists(fontLicensePath)
                || !File.ReadAllText(fontLicensePath).Contains("SIL OPEN FONT LICENSE Version 1.1"))
            {
                throw new BuildFailedException("The Libre Baskerville OFL 1.1 license text is missing.");
            }
        }

        private static void WritePackageHint(string outputRoot, string zipPath)
        {
            string notePath = Path.Combine(outputRoot, "PACKAGE_BUILD_NOTE.txt");
            string note =
                VersionInfo.ProductName + " Windows build staging folder.\n" +
                "Zip this folder after Unity exits to create the distributable package.\n" +
                "Expected zip: " + zipPath + "\n";
            File.WriteAllText(notePath, note);
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

            HashSet<string> packagedArtFileNames = BuildPackagedArtReferenceFileNames(docsRoot);
            foreach (string source in Directory.GetFiles(docsRoot, "*.*", SearchOption.AllDirectories))
            {
                string relative = Path.Combine("Docs", Path.GetRelativePath(docsRoot, source));
                if (ShouldSkipPackagedDoc(relative, packagedArtFileNames))
                {
                    continue;
                }
                string destination = Path.Combine(outputRoot, relative);
                Directory.CreateDirectory(Path.GetDirectoryName(destination));
                File.Copy(source, destination, true);
            }

            WritePackagedArtManifest(outputRoot, packagedArtFileNames);
        }

        private static bool ShouldSkipPackagedDoc(string relativePath, HashSet<string> packagedArtFileNames)
        {
            string normalized = relativePath.Replace('\\', '/');
            string name = Path.GetFileName(normalized).ToLowerInvariant();
            if (!normalized.StartsWith("Docs/ArtReferences/"))
            {
                return false;
            }

            if (name.StartsWith("source-"))
            {
                return true;
            }
            if (name.EndsWith("-prompt.txt") || name.EndsWith("-prompts.txt") || name.Contains("-prompt."))
            {
                return true;
            }
            if (name.EndsWith("prompts.txt") || name.Contains("-contact."))
            {
                return true;
            }

            if (!name.EndsWith(".png"))
            {
                return true;
            }

            return !packagedArtFileNames.Contains(Path.GetFileName(normalized));
        }

        private static HashSet<string> BuildPackagedArtReferenceFileNames(string docsRoot)
        {
            HashSet<string> result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string artRoot = Path.Combine(docsRoot, "ArtReferences");
            if (!Directory.Exists(artRoot))
            {
                return result;
            }

            foreach (string approvedFileName in AshenHalls.RuntimeArtManifest.ApprovedRuntimeFiles)
            {
                string approvedPath = Path.Combine(artRoot, approvedFileName);
                if (!File.Exists(approvedPath))
                {
                    throw new BuildFailedException("Approved runtime art is missing: " + approvedFileName);
                }
                result.Add(approvedFileName);
            }

            foreach (string prefix in PackagedArtPrefixes)
            {
                IEnumerable<string> candidates = Directory.GetFiles(artRoot, prefix + "*.png")
                    .Where(path => !ShouldSkipArtReferenceByName(Path.GetFileName(path)))
                    .OrderByDescending(ArtVersionSortKey)
                    .ThenByDescending(File.GetLastWriteTimeUtc)
                    .ThenByDescending(Path.GetFileName)
                    .Take(PackagedArtVersionsPerPrefix);

                foreach (string path in candidates)
                {
                    result.Add(Path.GetFileName(path));
                }
            }

            return result;
        }

        internal static void ValidateApprovedRuntimeArtIsLatest(string projectRoot)
        {
            string artRoot = Path.Combine(projectRoot, "Docs", "ArtReferences");
            if (!Directory.Exists(artRoot))
            {
                throw new BuildFailedException("Runtime art folder is missing: " + artRoot);
            }

            foreach (string approvedFileName in AshenHalls.RuntimeArtManifest.ApprovedRuntimeFiles)
            {
                string approvedPath = Path.Combine(artRoot, approvedFileName);
                if (!File.Exists(approvedPath))
                {
                    throw new BuildFailedException("Approved runtime art is missing: " + approvedFileName);
                }

                string nameWithoutExtension = Path.GetFileNameWithoutExtension(approvedFileName) ?? "";
                int versionStart = nameWithoutExtension.LastIndexOf("-v", StringComparison.OrdinalIgnoreCase);
                if (versionStart < 0)
                {
                    throw new BuildFailedException("Approved runtime art has no semantic version suffix: " + approvedFileName);
                }

                string familyPrefix = nameWithoutExtension.Substring(0, versionStart + 2);
                string latestPath = Directory.GetFiles(artRoot, familyPrefix + "*.png")
                    .Where(path => !ShouldSkipArtReferenceByName(Path.GetFileName(path)))
                    .OrderByDescending(ArtVersionSortKey)
                    .ThenByDescending(File.GetLastWriteTimeUtc)
                    .ThenByDescending(Path.GetFileName)
                    .FirstOrDefault();
                string latestFileName = string.IsNullOrEmpty(latestPath) ? "" : Path.GetFileName(latestPath);
                if (!string.Equals(approvedFileName, latestFileName, StringComparison.OrdinalIgnoreCase))
                {
                    throw new BuildFailedException(
                        "Approved runtime art pin is stale for '" + familyPrefix + "': "
                        + approvedFileName + " is pinned, but " + latestFileName + " is newest.");
                }
            }
        }

        private static bool ShouldSkipArtReferenceByName(string fileName)
        {
            string name = fileName.ToLowerInvariant();
            return name.StartsWith("source-")
                || name.EndsWith("-prompt.txt")
                || name.EndsWith("-prompts.txt")
                || name.Contains("-prompt.")
                || name.EndsWith("prompts.txt")
                || name.Contains("-contact.");
        }

        private static long ArtVersionSortKey(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path) ?? "";
            int versionStart = name.LastIndexOf("-v", StringComparison.OrdinalIgnoreCase);
            if (versionStart < 0) return -1;
            string version = name.Substring(versionStart + 2);
            long key = 0;
            int segmentCount = 0;
            foreach (string rawPart in version.Split('.'))
            {
                if (segmentCount >= 4) break;
                int value = 0;
                int digitCount = 0;
                foreach (char ch in rawPart)
                {
                    if (!char.IsDigit(ch)) break;
                    value = Math.Min(999, value * 10 + (ch - '0'));
                    digitCount++;
                }
                key = key * 1000 + (digitCount > 0 ? value : 0);
                segmentCount++;
            }
            while (segmentCount++ < 4) key *= 1000;
            return key;
        }

        private static void WritePackagedArtManifest(string outputRoot, HashSet<string> packagedArtFileNames)
        {
            if (packagedArtFileNames.Count <= 0)
            {
                return;
            }

            string docsOutput = Path.Combine(outputRoot, "Docs");
            Directory.CreateDirectory(docsOutput);
            string path = Path.Combine(docsOutput, "PACKAGED_ART.txt");
            List<string> lines = new List<string>
            {
                VersionInfo.ProductName + " packaged runtime art manifest",
                "Only preferred runtime art prefixes used by the game loader are included in the player package.",
                "Historical/source/prompt/contact art and older reference/fallback atlases remain in the source/art-reference workspace.",
                "Packaged versions per prefix: " + PackagedArtVersionsPerPrefix,
                "Packaged PNG count: " + packagedArtFileNames.Count,
                ""
            };
            lines.AddRange(packagedArtFileNames.OrderBy(name => name, StringComparer.OrdinalIgnoreCase));
            File.WriteAllLines(path, lines);
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
