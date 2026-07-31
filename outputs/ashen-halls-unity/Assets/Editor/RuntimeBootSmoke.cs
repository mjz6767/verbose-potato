using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AshenHalls;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AshenHalls.Editor
{
    public static class RuntimeBootSmoke
    {
        private const string MainScenePath = "Assets/Scenes/Main.unity";

        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " runtime boot smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " runtime boot smoke failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunCombatUi()
        {
            try
            {
                RunCombatUiOrThrow();
                Debug.Log(VersionInfo.ProductName + " combat UI runtime smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " combat UI runtime smoke failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunCombatUiOrThrow()
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string sceneFullPath = Path.Combine(projectRoot, MainScenePath);
            if (!File.Exists(sceneFullPath))
            {
                throw new InvalidOperationException("Main scene is missing: " + MainScenePath);
            }

            try
            {
                Scene scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
                Assert(scene.IsValid() && scene.isLoaded, "Main scene loads");

                AshenHallsGame game = UnityEngine.Object.FindFirstObjectByType<AshenHallsGame>();
                Assert(game != null, "AshenHallsGame exists in Main scene");

                InvokePrivate(game, "Awake");
                InvokePrivate(game, "LateUpdate");
                AssertEventSystemCount(1);
                AssertNoLaunchError(game);
                AssertCombatPresentationRuntime(game);
            }
            finally
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            }
        }

        public static void RunOrThrow()
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string sceneFullPath = Path.Combine(projectRoot, MainScenePath);
            if (!File.Exists(sceneFullPath))
            {
                throw new InvalidOperationException("Main scene is missing: " + MainScenePath);
            }

            try
            {
                Scene scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
                Assert(scene.IsValid() && scene.isLoaded, "Main scene loads");

                AshenHallsGame game = UnityEngine.Object.FindFirstObjectByType<AshenHallsGame>();
                Assert(game != null, "AshenHallsGame exists in Main scene");

                InvokePrivate(game, "Awake");
                InvokePrivate(game, "LateUpdate");

                AssertActiveObject("Tavern Canvas");
                AssertEventSystemCount(1);
                AssertNoLaunchError(game);
                AssertMode(game, GameMode.Tavern, "startup reaches Tavern");

                InvokePrivate(game, "StartNewGame");
                InvokePrivate(game, "LateUpdate");
                AssertMode(game, GameMode.Muster, "New Game reaches Muster");
                AssertActiveObject("Party Setup Canvas");

                InvokePrivate(game, "QuickStart");
                InvokePrivate(game, "LateUpdate");
                AssertMode(game, GameMode.Explore, "Quick Start reaches Explore");
                AssertActiveObject("Exploration HUD Canvas");
                AssertNoLaunchError(game);
                ExplorationHudScreen explorationHud = GetPrivateField<ExplorationHudScreen>(game, "explorationHudScreen");
                Assert(explorationHud != null && explorationHud.IsVisible, "migrated exploration HUD is active");
                Assert(explorationHud.HasVisibleCompactGuidance, "default exploration rail keeps objective and waypoint visible");
                Assert(explorationHud.VisiblePartyRows == 4, "default exploration rail keeps all four party rows visible");
                Assert(explorationHud.HasExpandedResourceLabelsForTest, "exploration chrome keeps full Gold, Supplies, and Elixirs labels");
                Assert(explorationHud.NumericPartyVitalRowsForTest == 4, "all four exploration party rows publish numeric HP and MP");
                ExplorationHudView firstPlayView = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
                Assert(firstPlayView.ObjectiveSummary.IndexOf("King Halvard", StringComparison.OrdinalIgnoreCase) >= 0, "fresh-game objective names King Halvard");
                Assert(firstPlayView.WaypointLine.IndexOf("King", StringComparison.OrdinalIgnoreCase) >= 0, "fresh-game waypoint points to King's Hall");
                Assert((firstPlayView.WaypointLine.StartsWith("W / Up | ", StringComparison.Ordinal)
                        || firstPlayView.WaypointLine.StartsWith("S / Down | ", StringComparison.Ordinal)
                        || firstPlayView.WaypointLine.StartsWith("A / Left | ", StringComparison.Ordinal)
                        || firstPlayView.WaypointLine.StartsWith("D / Right | ", StringComparison.Ordinal))
                    && firstPlayView.WaypointLine.IndexOf("step", StringComparison.OrdinalIgnoreCase) >= 0,
                    "fresh-game Golden Thread gives an exact movement input, path-aware first step, and distance");
                Assert(firstPlayView.WaypointLine.Length <= ExplorationGuidanceRules.MaxHudLineLength,
                    "fresh-game Golden Thread stays inside its compact HUD copy bound");
                IReadOnlyList<Point> firstPlayGuidancePath = InvokePrivate<IReadOnlyList<Point>>(game, "CurrentExploreGuidancePath");
                string firstPlayGuidanceTarget = InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName");
                Assert(firstPlayGuidancePath.Count > 1
                    && firstPlayGuidanceTarget.IndexOf("King", StringComparison.OrdinalIgnoreCase) >= 0,
                    "fresh-game HUD and map share one reachable King's Hall guidance plan");
                GameState firstPlayState = GetPrivateField<GameState>(game, "state");
                Assert(firstPlayGuidancePath[0].X == firstPlayState.PlayerX
                    && firstPlayGuidancePath[0].Y == firstPlayState.PlayerY
                    && Math.Abs(firstPlayGuidancePath[1].X - firstPlayState.PlayerX)
                        + Math.Abs(firstPlayGuidancePath[1].Y - firstPlayState.PlayerY) == 1,
                    "fresh-game map thread begins on the party and advances by one legal cardinal step");
                Point firstPlayStep = firstPlayGuidancePath[1];
                string firstPlayDirection = firstPlayStep.Y < firstPlayState.PlayerY ? "N"
                    : firstPlayStep.Y > firstPlayState.PlayerY ? "S"
                    : firstPlayStep.X < firstPlayState.PlayerX ? "W"
                    : "E";
                Assert(firstPlayView.WaypointLine.StartsWith(
                        ExplorationGuidanceRules.MovementInput(firstPlayDirection) + " | ",
                        StringComparison.Ordinal),
                    "fresh-game NEXT copy uses the same first step consumed by the on-map keycap and trail");
                Assert(!InvokePrivate<bool>(game, "CurrentExploreGuidanceIsMarked")
                    && !InvokePrivate<bool>(game, "CurrentExploreGuidanceIsBlocked"),
                    "fresh-game automatic guidance is available without impersonating a Journal mark");
                Assert((string.IsNullOrEmpty(firstPlayView.ActionTarget)
                        || firstPlayView.NearbyLine.IndexOf(firstPlayView.ActionTarget, StringComparison.OrdinalIgnoreCase) < 0)
                    && firstPlayView.NearbyLine.IndexOf(firstPlayGuidanceTarget, StringComparison.OrdinalIgnoreCase) < 0,
                    "compact NEARBY adds context without repeating the current action or Golden Thread target");
                InvokePrivate(
                    game,
                    "ApplyVisualSmokeExploreView",
                    (object)new[] { "-ashen-explore-smoke", "-ashen-region-smoke", "-ashen-details-smoke" });
                Assert(
                    GetPrivateField<bool>(game, "exploreWideView")
                    && !GetPrivateField<bool>(game, "exploreHudCollapsed"),
                    "visual smoke flags deterministically stage Region Map with Details open");
                InvokePrivate(game, "MarkUiDirty");
                InvokePrivate(game, "LateUpdate");
                ExplorationHudView firstPlayDetailsView = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
                Assert(firstPlayDetailsView.DetailsOpen
                    && explorationHud.HasVisibleGoldenThreadForTest
                    && explorationHud.GoldenThreadTextForTest == firstPlayView.WaypointLine,
                    "Details keeps the same persistent Golden Thread visible");
                InvokePrivate(
                    game,
                    "ApplyVisualSmokeExploreView",
                    (object)new[] { "-ashen-explore-smoke" });
                Assert(
                    !GetPrivateField<bool>(game, "exploreWideView")
                    && GetPrivateField<bool>(game, "exploreHudCollapsed"),
                    "default exploration smoke returns to Local Map with Details closed");
                InvokePrivate(game, "MarkUiDirty");
                InvokePrivate(game, "LateUpdate");
                AssertExplorationWorldMapRuntime(game);
                AssertCombatPresentationRuntime(game);
            }
            finally
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            }
        }

        private static void AssertExplorationWorldMapRuntime(AshenHallsGame game)
        {
            GameState state = GetPrivateField<GameState>(game, "state");
            Assert(state.MusicVolumePercent == 65, "fresh games keep an independent readable music level");
            Assert(!state.MusicMuted && !state.SfxMuted, "fresh games start with independent music and SFX channels enabled");
            Dictionary<string, AudioClip> soundClips = GetPrivateField<Dictionary<string, AudioClip>>(game, "soundClips");
            foreach (string key in new[]
            {
                "impactflesh", "impactleather", "impactmail", "impactplate", "impactshield",
                "ratchitter", "ratattack", "ratcast", "ratimpact", "ratdeath",
                "koboldalert", "koboldstep", "koboldattack", "koboldcast", "koboldhurt", "kobolddeath",
                "drowalert", "drowstep", "drowattack", "drowcast", "drowhurt", "drowdeath",
                "demonalert", "demonstep", "demonattack", "demoncast", "demonhurt", "demondeath",
                "undeadalert", "undeadstep", "undeadattack", "undeadcast", "undeadhurt", "undeaddeath",
                "castmend", "castlight", "castember", "castfrost", "castshock", "castnature", "casthex", "castpact",
                "castdeathburst", "deathburst", "castgreatersummon", "greatersummon",
                "castveil", "veilstep", "casttempest", "tempest", "castascendance", "ascendance", "castseal", "riftseal",
                "charge", "whirlwind", "execute", "ambush", "eviscerate",
                "chargeimpact", "whirlwindimpact", "executeimpact", "ambushimpact", "eviscerateimpact", "sleep",
                "stealth", "smoke", "rally", "aimedshot", "pinning", "volley", "scoutmark", "arrowrain", "mark",
                "bladecontact", "thrustcontact", "heavycontact", "arrowcontact", "woodcontact", "stonecontact", "wayfind",
                "footglass", "footmud", "footash", "footgravel",
                "servicecoin", "servicearmor", "serviceweapon", "serviceenchant",
                "doorwood", "doorroyal", "thronechime", "shopbell",
                "uiopen", "uiclose", "uiconfirm", "uitab", "itemequip", "itemtake", "elixir", "rest", "levelup",
                "ambrain", "ambtavern", "ambhearth",
                "ambcity", "ambbell", "ambmarket", "ambforge", "ambgate", "ambdrip", "ambwind", "ambdrum", "ambstone",
                "ambgrove", "ambfen", "ambglass", "ambruin", "ambcave", "ambcamp"
            })
            {
                Assert(soundClips.ContainsKey(key) && soundClips[key] != null, "resolved audio bank contains " + key);
            }
            foreach (string key in new[] { "ui", "uiopen", "itemequip", "elixir", "levelup", "footglass", "footmud", "heavycontact", "castember", "castfrost", "charge", "whirlwind", "aimedshot", "arrowrain" })
            {
                Assert(soundClips[key].frequency >= 32000, key + " uses the v1.70 high-resolution audio path");
                Assert(AudioClipHasHealthyHeadroom(soundClips[key]), key + " has audible body, finite samples, and clean output headroom");
            }
            HashSet<string> importedSfxKeys = GetPrivateField<HashSet<string>>(game, "importedSfxKeys");
            AudioClip[] importedSfx = Resources.LoadAll<AudioClip>("Audio/Sfx");
            Assert(importedSfx.Length == 136, "authored SFX resource bank contains 55 curated and 81 original cues");
            Assert(importedSfxKeys.Count == importedSfx.Length, "every authored SFX resource replaces a known runtime cue");
            foreach (AudioClip clip in importedSfx)
            {
                string key = clip.name.ToLowerInvariant();
                Assert(importedSfxKeys.Contains(key), key + " is recorded as an authored override");
                Assert(soundClips.ContainsKey(key) && soundClips[key] == clip, key + " replaces its procedural fallback");
                Assert(clip.loadState != AudioDataLoadState.Failed, key + " audio data loads");
                Assert(clip.frequency == 48000 && clip.channels == 1, key + " is mastered as 48 kHz mono");
                Assert(AudioClipHasHealthyHeadroom(clip), key + " authored master is finite, audible, and below clipping");
            }
            Assert(importedSfxKeys.Contains("castember")
                && importedSfxKeys.Contains("castfrost")
                && importedSfxKeys.Contains("castpact")
                && importedSfxKeys.Contains("riftseal"),
                "core cast schools and signature magic use original masters");
            HashSet<string> importedMusicKeys = GetPrivateField<HashSet<string>>(game, "importedMusicKeys");
            Dictionary<string, AudioClip> importedMusicClips = GetPrivateField<Dictionary<string, AudioClip>>(game, "importedMusicClips");
            AudioClip[] importedMusic = Resources.LoadAll<AudioClip>("Audio/Music");
            Assert(importedMusic.Length == 44, "original music resource bank contains all 44 routed score contexts");
            Assert(importedMusicKeys.Count == importedMusic.Length, "every original music master passes the runtime metadata contract");
            Assert(importedMusicClips.Count == importedMusic.Length, "every original music master is indexed by exact clip name");
            foreach (AudioClip clip in importedMusic)
            {
                string key = clip.name.ToLowerInvariant();
                Assert(importedMusicKeys.Contains(key), key + " is recorded as an original music master");
                Assert(importedMusicClips.TryGetValue(key, out AudioClip indexed) && indexed == clip, key + " resolves through the music override bank");
                Assert(clip.loadState != AudioDataLoadState.Failed, key + " music data loads");
                Assert(clip.frequency == 32000 && clip.channels == 2, key + " is mastered as 32 kHz stereo");
                float maximumDuration = key == "tavern_storm_hearth_ensemble_loop" ? 40f : 30.1f;
                Assert(
                    clip.length >= 15f && clip.length <= maximumDuration,
                    key + " stays inside its authored loop-duration budget");
            }
            AudioClip tavernMusic = GetPrivateField<AudioClip>(game, "tavernMusicClip");
            AudioClip combatMusic = GetPrivateField<AudioClip>(game, "combatMusicClip");
            AudioClip sewerCombatMusic = GetPrivateField<AudioClip>(game, "sewerCombatMusicClip");
            AudioClip bossCombatMusic = GetPrivateField<AudioClip>(game, "bossCombatMusicClip");
            AudioClip koboldCombatMusic = GetPrivateField<AudioClip>(game, "koboldCombatMusicClip");
            AudioClip drowCombatMusic = GetPrivateField<AudioClip>(game, "drowCombatMusicClip");
            AudioClip demonCombatMusic = GetPrivateField<AudioClip>(game, "demonCombatMusicClip");
            AudioClip undeadCombatMusic = GetPrivateField<AudioClip>(game, "undeadCombatMusicClip");
            Assert(tavernMusic != null && tavernMusic.length >= 36.8f && tavernMusic.length <= 37f, "tavern owns the complete 36.9-second Brimstone Overture");
            Assert(importedMusic.Contains(tavernMusic), "tavern uses its original stereo master instead of the procedural fallback");
            Assert(combatMusic != null && sewerCombatMusic != null && bossCombatMusic != null, "combat director owns standard, sewer, and boss music");
            Assert(koboldCombatMusic != null && drowCombatMusic != null && demonCombatMusic != null && undeadCombatMusic != null, "combat director owns four faction scores");
            Assert(AudioClipsDiffer(combatMusic, sewerCombatMusic), "sewer combat has a distinct score");
            Assert(AudioClipsDiffer(combatMusic, bossCombatMusic), "boss combat has a distinct score");
            Assert(AudioClipsDiffer(koboldCombatMusic, drowCombatMusic), "kobold and drow combat scores differ");
            Assert(AudioClipsDiffer(demonCombatMusic, undeadCombatMusic), "demon and undead combat scores differ");
            Dictionary<string, Func<AudioClip>> adaptiveMusicFactories = GetPrivateField<Dictionary<string, Func<AudioClip>>>(game, "adaptiveMusicFactories");
            Dictionary<string, AudioClip> adaptiveMusicClips = GetPrivateField<Dictionary<string, AudioClip>>(game, "adaptiveMusicClips");
            string[] expandedMusicKeys =
            {
                MusicDirectorRules.Muster, MusicDirectorRules.Victory, MusicDirectorRules.Defeat,
                MusicDirectorRules.MidgaardTemple, MusicDirectorRules.MidgaardMarket, MusicDirectorRules.MidgaardTavernLane,
                MusicDirectorRules.MidgaardGateWatch, MusicDirectorRules.MidgaardCisternMouth, MusicDirectorRules.MidgaardRoyalApproach,
                MusicDirectorRules.MidgaardRoad, MusicDirectorRules.RoadsideRest, MusicDirectorRules.SacredGround,
                MusicDirectorRules.UnderstoneThreshold, MusicDirectorRules.ForgottenRuins, MusicDirectorRules.ArcaneThreshold,
                MusicDirectorRules.HuntedRoad, MusicDirectorRules.AncientGrove, MusicDirectorRules.FactionCamp,
                MusicDirectorRules.CombatRatfolk, MusicDirectorRules.CombatArcaneDuel, MusicDirectorRules.CombatElite,
                MusicDirectorRules.CombatLastStand, MusicDirectorRules.CombatKoboldKing, MusicDirectorRules.CombatDemonLord
            };
            Assert(adaptiveMusicFactories.Count == expandedMusicKeys.Length, "adaptive score registers exactly 24 additional themes");
            Assert(expandedMusicKeys.All(adaptiveMusicFactories.ContainsKey), "every expanded score key has a lazy composition factory");
            Assert(adaptiveMusicClips.Count < adaptiveMusicFactories.Count, "procedural fallback score remains lazy when original masters are present");
            foreach (string musicKey in expandedMusicKeys)
            {
                AudioClip routed = InvokePrivate<AudioClip>(game, "MusicClipForKey", musicKey);
                Assert(routed != null && importedMusic.Contains(routed), musicKey + " resolves to an original music master");
            }

            AudioClip musterMusic = InvokePrivate<AudioClip>(game, "MusicClipForKey", MusicDirectorRules.Muster);
            AudioClip templeMusic = InvokePrivate<AudioClip>(game, "MusicClipForKey", MusicDirectorRules.MidgaardTemple);
            AudioClip pursuitMusic = InvokePrivate<AudioClip>(game, "MusicClipForKey", MusicDirectorRules.HuntedRoad);
            AudioClip ratfolkMusic = InvokePrivate<AudioClip>(game, "MusicClipForKey", MusicDirectorRules.CombatRatfolk);
            AudioClip kingMusic = InvokePrivate<AudioClip>(game, "MusicClipForKey", MusicDirectorRules.CombatKoboldKing);
            AudioClip victoryMusic = InvokePrivate<AudioClip>(game, "MusicClipForKey", MusicDirectorRules.Victory);
            AudioClip defeatMusic = InvokePrivate<AudioClip>(game, "MusicClipForKey", MusicDirectorRules.Defeat);
            Assert(musterMusic != null && musterMusic.name == "muster_by_firelight_loop", "Party Setup owns its quieter firelight theme");
            Assert(templeMusic != null && templeMusic.name == "bells_over_temple_square_loop", "Temple Square uses its original bell score");
            Assert(pursuitMusic != null && pursuitMusic.name == "footsteps_behind_loop", "alerted patrols own a pursuit score");
            Assert(ratfolkMusic != null && ratfolkMusic.name == "ratfolk_plague_march_loop", "ratfolk combat owns a plague march");
            Assert(kingMusic != null && kingMusic.name == "crooked_crown_kobold_king_loop", "Kobold King owns a dedicated battle score");
            Assert(victoryMusic != null && defeatMusic != null, "Victory and Defeat no longer fall silent");
            Assert(importedMusic.Contains(musterMusic)
                && importedMusic.Contains(templeMusic)
                && importedMusic.Contains(pursuitMusic)
                && importedMusic.Contains(ratfolkMusic)
                && importedMusic.Contains(kingMusic)
                && importedMusic.Contains(victoryMusic)
                && importedMusic.Contains(defeatMusic),
                "player-state, exploration, pursuit, and combat contexts choose original music masters");
            Assert(AudioClipsDiffer(musterMusic, templeMusic), "muster and temple themes use distinct arrangements");
            Assert(AudioClipsDiffer(pursuitMusic, ratfolkMusic), "pursuit and ratfolk battle themes use distinct rhythms");
            Assert(AudioClipsDiffer(kingMusic, bossCombatMusic), "Kobold King score differs from the generic boss theme");
            Assert(AudioClipsDiffer(victoryMusic, defeatMusic), "Victory and Defeat have opposite musical identities");

            GameMode modeBeforeMusicProbe = state.Mode;
            state.Mode = GameMode.Muster;
            Assert(InvokePrivate<AudioClip>(game, "DesiredMusicClip").name == "muster_by_firelight_loop", "live director routes Party Setup music");
            state.Mode = GameMode.Victory;
            Assert(InvokePrivate<AudioClip>(game, "DesiredMusicClip").name == "embers_carry_home_victory_loop", "live director routes Victory music");
            state.Mode = GameMode.Defeat;
            Assert(InvokePrivate<AudioClip>(game, "DesiredMusicClip").name == "ashes_on_the_road_defeat_loop", "live director routes Defeat music");
            state.Mode = modeBeforeMusicProbe;
            Assert(InvokePrivate<AudioClip>(game, "DesiredMusicClip") != null, "live director restores exploration music");
            InvokePrivate(game, "ToggleSfxMute");
            Assert(state.SfxMuted && !state.MusicMuted, "muting SFX leaves music enabled");
            InvokePrivate(game, "ToggleMusicMute");
            Assert(state.SfxMuted && state.MusicMuted, "music can be muted independently");
            InvokePrivate(game, "ToggleSfxMute");
            Assert(!state.SfxMuted && state.MusicMuted, "reenabling SFX leaves music muted");
            InvokePrivate(game, "ToggleMusicMute");
            Assert(!state.SfxMuted && !state.MusicMuted, "audio channels return to their fresh enabled state");
            Assert(AudioClipsDiffer(soundClips["impactflesh"], soundClips["impactplate"]), "flesh and plate impacts use materially different waveforms");
            Assert(AudioClipsDiffer(soundClips["impactmail"], soundClips["impactshield"]), "mail and shield impacts use materially different waveforms");
            Assert(AudioClipsDiffer(soundClips["ratchitter"], soundClips["ratdeath"]), "rat chatter and defeat use distinct vocal contours");
            Assert(AudioClipsDiffer(soundClips["koboldalert"], soundClips["kobolddeath"]), "kobold alert and defeat voices differ");
            Assert(AudioClipsDiffer(soundClips["drowcast"], soundClips["drowhurt"]), "drow spell and hurt voices differ");
            Assert(AudioClipsDiffer(soundClips["demonattack"], soundClips["undeadattack"]), "demon and undead attacks use distinct vocal textures");
            Assert(AudioClipsDiffer(soundClips["castember"], soundClips["castfrost"]), "fire and frost casting use distinct spell voices");
            Assert(AudioClipsDiffer(soundClips["servicecoin"], soundClips["servicearmor"]), "coin handling and armor fitting use distinct service waveforms");
            Assert(AudioClipsDiffer(soundClips["serviceweapon"], soundClips["serviceenchant"]), "weapon draw and rune binding use distinct service waveforms");
            Assert(AudioClipsDiffer(soundClips["ambbell"], soundClips["ambforge"]), "temple bells and forge strikes use distinct ambience waveforms");
            Assert(AudioClipsDiffer(soundClips["ambdrip"], soundClips["ambdrum"]), "cistern drips and road drums use distinct ambience waveforms");
            Assert(AudioClipsDiffer(soundClips["ambrain"], soundClips["ambhearth"]), "tavern rain and hearth use distinct ambience waveforms");
            Assert(AudioClipsDiffer(soundClips["ambgrove"], soundClips["ambfen"]), "grove and fen ambience use distinct wilderness textures");
            Assert(AudioClipsDiffer(soundClips["ambglass"], soundClips["ambruin"]), "glass and ruin ambience use distinct wilderness textures");
            Assert(AudioClipsDiffer(soundClips["uiopen"], soundClips["uiclose"]), "overlay open and close feedback use distinct contours");
            Assert(AudioClipsDiffer(soundClips["itemequip"], soundClips["itemtake"]), "equipping and taking loot use distinct physical feedback");
            Assert(AudioClipsDiffer(soundClips["footglass"], soundClips["footmud"]), "glass rubble and fen mud use distinct footsteps");
            Assert(AudioClipsDiffer(soundClips["footash"], soundClips["footgravel"]), "ash and gravel use distinct footsteps");
            Assert(AudioClipsDiffer(soundClips["deathburst"], soundClips["riftseal"]), "death burst and rift seal use distinct signature waveforms");
            Assert(AudioClipsDiffer(soundClips["charge"], soundClips["whirlwind"]), "charge and whirlwind use distinct skill releases");
            Assert(AudioClipsDiffer(soundClips["execute"], soundClips["ambush"]), "execute and ambush use distinct skill releases");
            Assert(AudioClipsDiffer(soundClips["aimedshot"], soundClips["volley"]), "aimed shot and volley use distinct ranger releases");
            Assert(AudioClipsDiffer(soundClips["rally"], soundClips["smoke"]), "rally and smoke bomb use distinct utility textures");
            Assert(AudioClipsDiffer(soundClips["whirlwindimpact"], soundClips["eviscerateimpact"]), "whirlwind and eviscerate use distinct martial waveforms");
            Assert(AudioClipsDiffer(soundClips["bladecontact"], soundClips["heavycontact"]), "blade and heavy weapon contacts use distinct waveforms");
            Assert(AudioClipsDiffer(soundClips["thrustcontact"], soundClips["arrowcontact"]), "thrust and projectile contacts use distinct waveforms");
            Assert(AudioClipsDiffer(soundClips["woodcontact"], soundClips["stonecontact"]), "wood and stone cover contacts use distinct waveforms");
            InvokePrivate(game, "UpdateExplorationAmbience");
            Assert(GetPrivateField<string>(game, "lastExplorationAmbienceContext").StartsWith("amb", StringComparison.Ordinal), "exploration schedules a semantic ambient context");
            Texture2D terrainAtlas = GetPrivateField<Texture2D>(game, "worldMapExplorationTileAtlas");
            Texture2D materialAtlas = GetPrivateField<Texture2D>(game, "worldMapMaterialAtlas");
            Texture2D worldOverlayAtlas = GetPrivateField<Texture2D>(game, "worldMapOverlayAtlas");
            Texture2D progressionOverlayAtlas = GetPrivateField<Texture2D>(game, "worldMapProgressionOverlayAtlas");
            Texture2D worldUiAtlas = GetPrivateField<Texture2D>(game, "worldMapUiAtlas");
            Assert(terrainAtlas != null, "world-map terrain atlas is loaded");
            Assert(terrainAtlas.name.IndexOf("v1.68.0", StringComparison.OrdinalIgnoreCase) >= 0, "world-map blocked terrain uses the expanded v1.68 art contract");
            Assert(terrainAtlas.width == 1280 && terrainAtlas.height == 2048, "world-map blocked terrain atlas is an exact 5x8 grid");
            Assert(materialAtlas != null, "world-map material atlas is loaded");
            Assert(materialAtlas.name.IndexOf("v1.92.0", StringComparison.OrdinalIgnoreCase) >= 0, "world-map ground uses the coherent v1.92 material contract");
            Assert(materialAtlas.width == 2048 && materialAtlas.height == 2048, "world-map material atlas is an exact 8x8 grid");
            Assert(worldOverlayAtlas != null && worldOverlayAtlas.name.IndexOf("v0.80", StringComparison.OrdinalIgnoreCase) >= 0, "world-map overlays use the pinned v0.80 contract");
            Assert(worldOverlayAtlas.width == 1280 && worldOverlayAtlas.height == 1024, "world-map overlay atlas is an exact 5x4 grid");
            Assert(progressionOverlayAtlas != null && progressionOverlayAtlas.name.IndexOf("v0.63", StringComparison.OrdinalIgnoreCase) >= 0, "world-map progression overlays use the pinned v0.63 contract");
            Assert(progressionOverlayAtlas.width == 1280 && progressionOverlayAtlas.height == 1024, "world-map progression overlay atlas is an exact 5x4 grid");
            Assert(worldUiAtlas != null && worldUiAtlas.name.IndexOf("v1.6.0", StringComparison.OrdinalIgnoreCase) >= 0, "world-map UI uses the pinned v1.6 contract");
            Assert(worldUiAtlas.width == 1402 && worldUiAtlas.height == 1122, "world-map UI atlas keeps its approved dimensions");
            Texture2D streetLifeAtlas = GetPrivateField<Texture2D>(game, "midgaardStreetLifeAtlas");
            Texture2D pavingDecalAtlas = GetPrivateField<Texture2D>(game, "midgaardPavingDecalAtlas");
            Texture2D interiorPropAtlas = GetPrivateField<Texture2D>(game, "midgaardInteriorPropAtlas");
            Texture2D interiorTileAtlas = GetPrivateField<Texture2D>(game, "midgaardInteriorTileAtlas");
            Texture2D gateAtlas = GetPrivateField<Texture2D>(game, "midgaardGateAtlas");
            Texture2D wallAtlas = GetPrivateField<Texture2D>(game, "midgaardWallAtlas");
            Texture2D midgaardTileAtlas = GetPrivateField<Texture2D>(game, "midgaardTileAtlas");
            Texture2D cityNpcAtlas = GetPrivateField<Texture2D>(game, "midgaardNpcAtlas");
            Texture2D npcPortraitAtlas = GetPrivateField<Texture2D>(game, "npcPortraitAtlas");
            Texture2D characterSpriteAtlas = GetPrivateField<Texture2D>(game, "characterCombatAtlas");
            Texture2D enemySpriteAtlas = GetPrivateField<Texture2D>(game, "enemySpriteAtlas");
            Texture2D titleCardAtlas = GetPrivateField<Texture2D>(game, "titleCardArt");
            Texture2D gameIconAtlas = GetPrivateField<Texture2D>(game, "gameIconArt");
            Texture2D splashAtlas = GetPrivateField<Texture2D>(game, "splashArt");
            Texture2D tavernBackdropAtlas = GetPrivateField<Texture2D>(game, "tavernBackdropArt");
            Texture2D roamingThreatAtlas = GetPrivateField<Texture2D>(game, "roamingThreatAtlas");
            Texture2D regionalLandmarkAtlas = GetPrivateField<Texture2D>(game, "worldMapRegionLandmarkAtlas");
            Assert(streetLifeAtlas != null, "v1.50 Midgaard street-life atlas is loaded");
            Assert(streetLifeAtlas.name.IndexOf("v1.50.0", StringComparison.OrdinalIgnoreCase) >= 0, "Midgaard street life uses the pinned v1.50 art contract");
            Assert(streetLifeAtlas.width == 1400 && streetLifeAtlas.height == 1120, "Midgaard street-life atlas is an exact 5x4 grid");
            Assert(pavingDecalAtlas != null, "v1.50 Midgaard paving-decal atlas is loaded");
            Assert(pavingDecalAtlas.name.IndexOf("v1.50.0", StringComparison.OrdinalIgnoreCase) >= 0, "Midgaard paving details use the pinned v1.50 art contract");
            Assert(pavingDecalAtlas.width == 1252 && pavingDecalAtlas.height == 1252, "Midgaard paving-decal atlas is an exact 4x4 grid");
            Assert(roamingThreatAtlas != null, "v1.62 roaming-threat atlas is loaded");
            Assert(roamingThreatAtlas.name.IndexOf("v1.62.0", StringComparison.OrdinalIgnoreCase) >= 0, "roaming patrols use the pinned v1.62 art contract");
            Assert(roamingThreatAtlas.width == 1400 && roamingThreatAtlas.height == 1120, "roaming-threat atlas is an exact 5x4 grid");
            Assert(regionalLandmarkAtlas != null, "v1.65 regional-landmark atlas is loaded");
            Assert(regionalLandmarkAtlas.name.IndexOf("v1.65.0", StringComparison.OrdinalIgnoreCase) >= 0, "regional landmarks use the pinned v1.65 art contract");
            Assert(regionalLandmarkAtlas.width == 1400 && regionalLandmarkAtlas.height == 1120, "regional-landmark atlas is an exact 5x4 grid");
            Assert(interiorPropAtlas != null, "v1.61 Midgaard interior-prop atlas is loaded");
            Assert(interiorPropAtlas.name.IndexOf("v1.61.0", StringComparison.OrdinalIgnoreCase) >= 0, "Midgaard interiors use the pinned v1.61 art contract");
            Assert(interiorPropAtlas.width == 1400 && interiorPropAtlas.height == 1120, "Midgaard interior props use an exact 5x4 grid");
            Assert(interiorTileAtlas != null, "v1.61 Midgaard interior-tile atlas is loaded");
            Assert(interiorTileAtlas.name.IndexOf("v1.61.0", StringComparison.OrdinalIgnoreCase) >= 0, "Midgaard interior terrain uses the pinned v1.61 art contract");
            Assert(interiorTileAtlas.width == 1400 && interiorTileAtlas.height == 1120, "Midgaard interior terrain uses an exact 5x4 grid");
            Assert(gateAtlas != null, "v1.93 Midgaard gate atlas is loaded");
            Assert(gateAtlas.name.IndexOf("v1.93.0", StringComparison.OrdinalIgnoreCase) >= 0, "Midgaard side gates use the wall-aligned v1.93 art contract");
            Assert(gateAtlas.width == 1280 && gateAtlas.height == 1024, "Midgaard gate atlas is an exact 5x4 grid");
            Assert(wallAtlas != null, "v1.91 Midgaard wall atlas is loaded");
            Assert(wallAtlas.name.IndexOf("v1.91.0", StringComparison.OrdinalIgnoreCase) >= 0, "Midgaard walls use the corrected v1.91 art contract");
            Assert(wallAtlas.width == 1280 && wallAtlas.height == 1024, "Midgaard wall atlas is an exact 5x4 grid");
            Assert(midgaardTileAtlas != null && midgaardTileAtlas.name.IndexOf("v1.6.3", StringComparison.OrdinalIgnoreCase) >= 0, "Midgaard terrain uses the pinned v1.6.3 art contract");
            Assert(midgaardTileAtlas.width == 1400 && midgaardTileAtlas.height == 1120, "Midgaard terrain atlas is an exact 5x4 grid");
            Assert(cityNpcAtlas != null, "v1.93 Midgaard NPC atlas is loaded");
            Assert(cityNpcAtlas.name.IndexOf("v1.93.0", StringComparison.OrdinalIgnoreCase) >= 0, "named Midgaard NPCs use the approved v1.93 art contract");
            Assert(cityNpcAtlas.width == 1280 && cityNpcAtlas.height == 1024, "Midgaard NPC atlas is an exact 5x4 grid");
            Assert(npcPortraitAtlas != null && npcPortraitAtlas.name.IndexOf("v1.60.0", StringComparison.OrdinalIgnoreCase) >= 0, "named Midgaard portraits use the pinned v1.60 art contract");
            Assert(npcPortraitAtlas.width == 1400 && npcPortraitAtlas.height == 1120, "NPC portrait atlas is an exact 5x4 grid");
            Assert(characterSpriteAtlas != null && characterSpriteAtlas.name.IndexOf("v1.93.0", StringComparison.OrdinalIgnoreCase) >= 0, "party combatants use the pinned v1.93 sprite contract");
            Assert(characterSpriteAtlas.width == 1280 && characterSpriteAtlas.height == 1792, "character combat atlas is an exact 5x7 grid");
            Assert(enemySpriteAtlas != null && enemySpriteAtlas.name.IndexOf("v1.77.0", StringComparison.OrdinalIgnoreCase) >= 0, "common enemies use the pinned v1.77 sprite contract");
            Assert(enemySpriteAtlas.width == 1024 && enemySpriteAtlas.height == 1024, "enemy sprite atlas is an exact 4x4 grid");
            Assert(titleCardAtlas != null, "Ash & Brimstone title-card art is loaded");
            Assert(titleCardAtlas.name.IndexOf("v1.64.0", StringComparison.OrdinalIgnoreCase) >= 0, "title card uses the pinned v1.64 art contract");
            Assert(titleCardAtlas.width == 1800 && titleCardAtlas.height == 600, "title card uses the normalized 3:1 banner");
            Assert(gameIconAtlas != null, "Ash & Brimstone emblem art is loaded");
            Assert(gameIconAtlas.name.IndexOf("v1.61.0", StringComparison.OrdinalIgnoreCase) >= 0, "game emblem uses the pinned v1.61 art contract");
            Assert(gameIconAtlas.width == 1254 && gameIconAtlas.height == 1254, "game emblem uses the original square runtime art");
            Assert(splashAtlas != null && splashAtlas == tavernBackdropAtlas, "startup uses the same current tavern art in editor and packaged player");
            Assert(InvokePrivate<int>(game, "WorldMapTokenSpriteIndex", "shield") == 1, "shield party token uses its authored shield cell");
            Assert(InvokePrivate<int>(game, "CharacterCombatAtlasIndex", " ", null, "shield") == 0, "legacy blank class still resolves to the warrior sprite");
            Assert(state?.Map != null, "exploration self-test has a generated map");
            Assert(state.Map.Width == WorldMapGenerationRules.Width && state.Map.Height == WorldMapGenerationRules.Height, "fresh exploration map uses the v1.69 expanded dimensions");
            Assert(state.Map.Objects != null && state.Map.Objects.Count > 0, "exploration map has objects");
            int mapCellCount = state.Map.Width * state.Map.Height;
            Assert(state.Map.SurfaceMaterials != null && state.Map.SurfaceMaterials.Count == mapCellCount, "exploration map has a complete material grid");
            Assert(state.Map.SurfaceRoles != null && state.Map.SurfaceRoles.Count == mapCellCount, "exploration map has a complete role grid");
            Assert(ExplorationSurfaceRules.HasValidGrid(state.Map), "exploration semantic grids satisfy the v19 contract");
            Assert(state.Map.SurfaceRoles.Count(raw => ExplorationSurfaceRules.IsPath((ExplorationCellRole)raw)) >= 12, "generated map contains a readable semantic route network");
            Assert(state.Map.SurfaceRoles.Any(raw => (((ExplorationCellRole)raw) & ExplorationCellRole.Plaza) != 0), "Midgaard contains authored plaza roles");
            Assert(state.Map.SurfaceRoles.Any(raw => ((((ExplorationCellRole)raw) & (ExplorationCellRole.City | ExplorationCellRole.Road)) == (ExplorationCellRole.City | ExplorationCellRole.Road))), "Midgaard contains authored city streets");
            Assert(state.Map.SurfaceMaterials.Any(raw => (ExplorationMaterial)raw == ExplorationMaterial.Forest), "generated world retains blocked forest material independently of passability");
            AssertRegionalRouteCircuit(game, state);
            AssertExpandedMapSeedSweep(game);
            MapData legacyMap = new MapData { Width = 4, Height = 3, Depth = 2, StartX = 1, StartY = 1 };
            legacyMap.Tiles = new List<int>
            {
                0, 0, 0, 0,
                0, 1, 1, 1,
                0, 0, 0, 0
            };
            List<PartyMember> candidateParty = new List<PartyMember> { new PartyMember { Name = "Load Probe" } };
            GameState legacyCandidate = new GameState { SaveVersion = 18, Mode = GameMode.Explore, Depth = 2, Map = legacyMap, Party = candidateParty };
            Assert(InvokePrivate<bool>(game, "IsLoadCandidateValid", legacyCandidate), "v18 map without semantic surfaces remains loadable for migration");
            GameState malformedCurrentCandidate = new GameState { SaveVersion = VersionInfo.SaveVersion, Mode = GameMode.Explore, Depth = 2, Map = legacyMap, Party = candidateParty };
            Assert(!InvokePrivate<bool>(game, "IsLoadCandidateValid", malformedCurrentCandidate), "v19 map without semantic surfaces is rejected before primary save selection");
            GameState emptyPartyCandidate = new GameState { SaveVersion = VersionInfo.SaveVersion, Mode = GameMode.Muster, Party = new List<PartyMember>() };
            Assert(!InvokePrivate<bool>(game, "IsLoadCandidateValid", emptyPartyCandidate), "same-version save with an empty party is rejected before primary save selection");
            Assert(InvokePrivate<bool>(game, "EnsureExploreSurfaceData", legacyMap, 18), "v18 map receives a semantic surface migration");
            string firstMigration = string.Join(",", legacyMap.SurfaceMaterials) + "|" + string.Join(",", legacyMap.SurfaceRoles);
            Assert(!InvokePrivate<bool>(game, "EnsureExploreSurfaceData", legacyMap, 18), "semantic surface migration is idempotent");
            Assert(firstMigration == string.Join(",", legacyMap.SurfaceMaterials) + "|" + string.Join(",", legacyMap.SurfaceRoles), "v18 semantic migration is deterministic");
            MapData legacySizedMap = new MapData
            {
                Width = WorldMapGenerationRules.LegacyWidth,
                Height = WorldMapGenerationRules.LegacyHeight,
                Depth = 2,
                StartX = WorldMapGenerationRules.LegacyWidth / 2,
                StartY = WorldMapGenerationRules.LegacyHeight / 2
            };
            WorldZone legacySizedZone = InvokePrivate<WorldZone>(game, "ZoneFor", 16, 15, legacySizedMap, 2);
            Assert(legacySizedZone != null && legacySizedZone.Id == "inner-ash-road", "legacy map biome boundaries use serialized dimensions instead of v1.69 fresh-map constants");
            Assert(InvokePrivate<bool>(game, "CanStepExplore", state.PlayerX, state.PlayerY), "party starts on a standable exploration tile");
            Assert(InvokePrivate<bool>(game, "IsMidgaardCityCell", state.PlayerX, state.PlayerY, state.Map, state.Depth), "party starts inside Midgaard on a fresh game");
            Assert(Math.Abs(state.PlayerX - state.Map.StartX) <= 3 && Math.Abs(state.PlayerY - state.Map.StartY) <= 3, "party starts near central Midgaard plaza");
            PartyMember defeatedProbe = state.Party[0];
            int defeatedProbeHp = defeatedProbe.Hp;
            defeatedProbe.Hp = 0;
            InvokePrivate(game, "RecalculateMember", defeatedProbe);
            Assert(defeatedProbe.Hp == 0, "derived-stat recalculation preserves a defeated party member");
            defeatedProbe.Hp = defeatedProbeHp;
            Assert(InvokePrivate<int>(game, "ReachableExploreTileCount", state.PlayerX, state.PlayerY) >= 12, "party starts in a useful reachable exploration component");
            Assert(InvokePrivate<bool>(game, "ReachableExploreHasUsefulTarget", state.PlayerX, state.PlayerY), "party starts with a reachable useful exploration target");
            AssertMidgaardGateTraversal(game, state);
            AssertExplorationMovementProbe(game, state);

            List<Point> reachable = InvokePrivate<List<Point>>(game, "ReachableExploreTilesFrom", state.PlayerX, state.PlayerY);
            Assert(reachable.Any(point => !InvokePrivate<bool>(game, "IsMidgaardCityCell", point.X, point.Y, state.Map, state.Depth)), "fresh party can reach the world outside Midgaard");
            AssertRegionalWayfinding(game, state, reachable);
            Assert(!state.Map.Objects.Any(obj => obj != null && obj.Type == ObjectType.Encounter), "sewer-slice map does not contain disabled patrol blockers");
            Assert(!state.Map.Objects.Any(obj => obj != null && obj.Type == ObjectType.Stairs), "sewer-slice map does not expose a chapter-bypass stair");
            bool[,] routeMask = ExplorationTraversalRules.ReachableMask(state.Map, state.PlayerX, state.PlayerY);
            ObjectType[] criticalCityTypes =
            {
                ObjectType.KingHall,
                ObjectType.Sewer,
                ObjectType.Armorer,
                ObjectType.TempleHealer,
                ObjectType.GateCaptain,
                ObjectType.OldRoadScout,
                ObjectType.EastGate,
                ObjectType.WestGate
            };
            foreach (ObjectType type in criticalCityTypes)
            {
                MapObject critical = state.Map.Objects.FirstOrDefault(obj => obj != null && obj.Type == type);
                Assert(critical != null, $"critical Midgaard target {type} exists");
                Assert(ExplorationTraversalRules.CanReachObject(routeMask, state.Map, critical), $"critical Midgaard target {type} is reachable");
            }
            Assert(state.Map.Objects.Count(obj => obj != null && obj.Type == ObjectType.KingHall) == 1, "Midgaard contains exactly one King's Hall");
            Assert(state.Map.Objects.Count(obj => obj != null && obj.Type == ObjectType.RoyalHerald) == 1, "Midgaard contains exactly one Royal Herald");
            AssertMidgaardInteriors(game, state);
            List<MapObject> cityGuards = state.Map.Objects.Where(obj => obj != null && obj.Type == ObjectType.TownGuard).OrderBy(obj => obj.X).ToList();
            Assert(cityGuards.Count == 2, "Midgaard contains two deliberate gate guards");
            Assert(InvokePrivate<string>(game, "ObjectName", cityGuards[0]) == "Watchman Rusk", "west gate guard has Rusk identity");
            Assert(InvokePrivate<string>(game, "ObjectName", cityGuards[1]) == "Watchwoman Ilyra", "east gate guard has Ilyra identity");
            Dictionary<ObjectType, int> newNpcContacts = new Dictionary<ObjectType, int>
            {
                { ObjectType.DinerCook, 10 },
                { ObjectType.Provisioner, 11 },
                { ObjectType.DockWorker, 14 },
                { ObjectType.Scholar, 19 }
            };
            foreach (KeyValuePair<ObjectType, int> contact in newNpcContacts)
            {
                MapObject placed = state.Map.Objects.SingleOrDefault(obj => obj != null && obj.Type == contact.Key);
                Assert(placed != null, contact.Key + " is placed exactly once in Midgaard");
                Assert(InvokePrivate<bool>(game, "IsMidgaardNpcObject", contact.Key), contact.Key + " uses actor-scale map presentation");
                Assert(
                    InvokePrivate<int>(game, "MidgaardNpcObjectIconIndex", contact.Key, placed) == contact.Value,
                    contact.Key + " reaches NPC atlas cell " + contact.Value + " through the live world adapter");
                Assert(
                    ExplorationTraversalRules.CanReachObject(
                        ExplorationTraversalRules.ReachableMask(state.Map, state.PlayerX, state.PlayerY),
                        state.Map,
                        placed),
                    contact.Key + " has a safe reachable Talk position");
            }
            AssertNewNpcContactDialogues(game, state, newNpcContacts);
            Assert(state.RoamingThreats != null, "fresh exploration initializes roaming threat state");
            List<RoamingThreat> patrols = state.RoamingThreats
                .Where(threat => threat != null && threat.Depth == state.Depth)
                .OrderBy(threat => threat.Id)
                .ToList();
            Assert(patrols.Count == 2, "two deliberate hostile patrols prowl beyond Midgaard");
            Assert(patrols.Any(threat => threat.Archetype == "rats"), "west patrol uses the rat-scout presentation");
            Assert(patrols.Any(threat => threat.Archetype == "ratfolk"), "east patrol uses the armored ratfolk presentation");
            Assert(patrols.Select(threat => threat.Id).Distinct().Count() == patrols.Count, "roaming patrol identities are stable and unique");
            foreach (RoamingThreat patrol in patrols)
            {
                Assert(patrol.Active, patrol.Name + " starts active");
                WorldZone patrolZone = InvokePrivate<WorldZone>(game, "ZoneFor", patrol.X, patrol.Y, state.Map, state.Depth);
                Assert(patrolZone != null && patrolZone.Danger > 0, patrol.Name + " starts outside the safe road");
                Assert(state.Map.Objects.All(obj => obj == null || obj.X != patrol.X || obj.Y != patrol.Y), patrol.Name + " does not overlap a map object");
                Assert(!InvokePrivate<bool>(game, "CanStepExplore", patrol.X, patrol.Y), patrol.Name + " visibly occupies its map tile");
            }
            string patrolSignature = string.Join("|", patrols.Select(threat => $"{threat.Id}:{threat.HomeX},{threat.HomeY}"));
            InvokePrivate(game, "EnsureRoamingThreats");
            string repairedPatrolSignature = string.Join("|", state.RoamingThreats
                .Where(threat => threat != null && threat.Depth == state.Depth)
                .OrderBy(threat => threat.Id)
                .Select(threat => $"{threat.Id}:{threat.HomeX},{threat.HomeY}"));
            Assert(patrolSignature == repairedPatrolSignature, "roaming patrol repair is deterministic and idempotent");
            string populationSignature = string.Join("|", state.Map.Objects
                .Where(obj => obj != null && InvokePrivate<bool>(game, "IsMidgaardCityCell", obj.X, obj.Y, state.Map, state.Depth))
                .OrderBy(obj => obj.Y)
                .ThenBy(obj => obj.X)
                .Select(obj => $"{obj.X},{obj.Y}:{obj.Type}"));
            InvokePrivate(game, "EnsureMidgaardStartZone", state.Map);
            string repairedPopulationSignature = string.Join("|", state.Map.Objects
                .Where(obj => obj != null && InvokePrivate<bool>(game, "IsMidgaardCityCell", obj.X, obj.Y, state.Map, state.Depth))
                .OrderBy(obj => obj.Y)
                .ThenBy(obj => obj.X)
                .Select(obj => $"{obj.X},{obj.Y}:{obj.Type}"));
            Assert(populationSignature == repairedPopulationSignature, "Midgaard population repair is deterministic and idempotent");
            foreach (MapObject obj in state.Map.Objects.Where(obj =>
                obj != null
                && !InvokePrivate<bool>(game, "IsMidgaardCityCell", obj.X, obj.Y, state.Map, state.Depth)
                && !InvokePrivate<bool>(game, "IsMidgaardInteriorCell", obj.X, obj.Y, state.Map, state.Depth)
                && (string.IsNullOrEmpty(obj.Id) || !obj.Id.StartsWith("regional-site:", StringComparison.Ordinal))
                && ExplorationTraversalRules.BlocksMovement(obj)))
            {
                bool hasSafeApproach =
                    ExplorationTraversalRules.IsStandable(state.Map, obj.X, obj.Y - 1)
                    || ExplorationTraversalRules.IsStandable(state.Map, obj.X, obj.Y + 1)
                    || ExplorationTraversalRules.IsStandable(state.Map, obj.X - 1, obj.Y)
                    || ExplorationTraversalRules.IsStandable(state.Map, obj.X + 1, obj.Y);
                Assert(hasSafeApproach, $"generated blocker {obj.Type} has a safe approach at {obj.X},{obj.Y}");
            }

            MapObject target = FindAdjacentProbeTarget(game, state, out int standX, out int standY);
            Assert(target != null, "found an adjacent-use probe target on the generated map");
            Assert(ExplorationTraversalRules.CanUseFromAdjacent(target), "probe target can be used from adjacent tile");
            Assert(!ExplorationTraversalRules.CanStandOnObject(target), "probe target blocks overlap");

            state.PlayerX = standX;
            state.PlayerY = standY;
            InvokePrivate(game, "LateUpdate");
            int dx = target.X - standX;
            int dy = target.Y - standY;

            Assert(!InvokePrivate<bool>(game, "CanStepExplore", target.X, target.Y), "blocking probe target is not standable");
            InvokePrivate(game, "TryMoveExplore", dx, dy);
            Assert(state.PlayerX == standX && state.PlayerY == standY, "blocked object move leaves party beside target");
            string blockedLine = InvokePrivate<string>(game, "ExploreBlockedMoveLine", dx, dy);
            Assert(blockedLine.IndexOf("Space/E", StringComparison.OrdinalIgnoreCase) >= 0, "blocked object movement advertises contextual use");
            Assert(blockedLine.IndexOf("Stone blocks", StringComparison.OrdinalIgnoreCase) < 0, "blocked object movement does not report generic stone");

            ExplorationInteraction interaction = InvokePrivate<ExplorationInteraction>(game, "CurrentExploreInteraction");
            Assert(interaction.HasTarget, "adjacent probe exposes a contextual interaction");
            Assert(interaction.Target == target, "contextual interaction selects the adjacent probe target");
            Assert(!interaction.IsUnderfoot, "contextual interaction remains adjacent, not underfoot");

            string look = InvokePrivate<string>(game, "ExploreLookLine", target.X, target.Y);
            Assert(look.IndexOf("Space/E", StringComparison.OrdinalIgnoreCase) >= 0 || look.IndexOf("use", StringComparison.OrdinalIgnoreCase) >= 0, "adjacent target look text advertises use instead of overlap");

            InvokePrivate(game, "UseNearbyExploreObject");
            Assert(state.PlayerX == standX && state.PlayerY == standY, "using adjacent target does not overlap sprite");
            InvokePrivate(game, "LateUpdate");
            Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Dialogue, "talking to a Midgaard NPC opens dialogue");
            AssertActiveObject("Dialogue Canvas");
            DialogueScreen dialogue = UnityEngine.Object.FindFirstObjectByType<DialogueScreen>();
            Assert(dialogue != null && dialogue.IsReady && dialogue.IsVisible, "dialogue uGUI is ready and visible");
            Assert(dialogue.HasRenderableGeometry, "dialogue canvas is a renderable root overlay");
            Assert(dialogue.IsInteractiveAndVisible, "dialogue canvas is opaque and owns pointer input");
            Assert(InvokePrivate<bool>(game, "HasRenderableGameplayOverlay", UiOverlay.Dialogue), "IMGUI yields the frame to visible dialogue");
            Assert(dialogue.HasScrollableBody, "dialogue body has a working scroll viewport");
            Assert(dialogue.BodyFontNameForTest.IndexOf("Baskerville", StringComparison.OrdinalIgnoreCase) >= 0, "dialogue body renders with the bundled old-style serif");
            Assert(dialogue.SpeakerFontNameForTest.IndexOf("Baskerville", StringComparison.OrdinalIgnoreCase) >= 0, "dialogue speaker renders with the bundled old-style serif");
            Assert(dialogue.BodyFontSizeForTest == 18 && dialogue.BodyFontStyleForTest == FontStyle.Normal, "dialogue body uses the readable 18-point regular treatment");
            Assert(dialogue.SpeakerHeightForTest + 0.5f >= dialogue.SpeakerPreferredHeightForTest, "dialogue speaker name has enough height for the old-style serif metrics");
            Assert(InvokePrivate<bool>(game, "IsBoardPointerSuppressed"), "opening dialogue suppresses activation click-through");
            int dialogueGold = state.Gold;
            int dialogueSupplies = state.Supplies;
            InvokePrivate(game, "UseNearbyExploreObject");
            Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Dialogue, "background Use cannot re-enter an NPC while dialogue owns input");
            Assert(state.Gold == dialogueGold && state.Supplies == dialogueSupplies, "blocked background Use cannot duplicate NPC rewards");
            dialogue.InvokeContinueForTest();
            Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.None, "dialogue Continue button closes the overlay");
            Assert(InvokePrivate<bool>(game, "IsBoardPointerSuppressed"), "closing dialogue suppresses board click-through");
            InvokePrivate(game, "LateUpdate");

            AssertQuestBoardDialogue(game, state);

            InvokePrivate(game, "ToggleArmory", 3);
            InvokePrivate(game, "LateUpdate");
            Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Armory, "Journal command opens the Armory overlay on its fourth tab");
            AssertRootOverlayCanvas("Armory Overlay Canvas");
            Assert(InvokePrivate<bool>(game, "HasRenderableGameplayOverlay", UiOverlay.Armory), "IMGUI yields the frame to visible Armory");
            InvokePrivate(game, "CloseArmoryOverlay");
            InvokePrivate(game, "LateUpdate");

            InventoryItem inventoryProbe = new InventoryItem
            {
                DisplayName = "+4 smoke-test stormglass sabre",
                Material = "stormglass",
                Form = "sabre",
                Slot = "weapon",
                Trait = "storm",
                Rarity = "rare",
                Bonus = 4,
                DamageMin = 4,
                DamageMax = 9,
                AttackSpeed = 10,
                DamageType = "shock"
            };
            state.Inventory.Add(inventoryProbe);
            InvokePrivate(game, "ToggleArmory", 1);
            InvokePrivate(game, "LateUpdate");
            ArmoryOverlayScreen armory = GetPrivateField<ArmoryOverlayScreen>(game, "armoryOverlayScreen");
            Assert(armory != null && armory.IsVisible && armory.HasRenderableGeometry, "Inventory opens as a renderable equipment-management overlay");
            Assert(armory.ActiveTabLabelForTest == "Inventory", "Pack tab now uses the clear Inventory label");
            int expectedInventoryFilters = state.Inventory.Count(item => item != null) >= 9 ? 3 : 0;
            Assert(armory.VisibleFilterCountForTest == expectedInventoryFilters, "small inventories hide filters; larger inventories expose only all, weapon, and armor");
            Assert(armory.VisibleRowCountForTest > 0 && armory.HasVisibleDetailForTest, "Inventory opens with a selected item and comparison pane");
            Assert(armory.SelectedRowUsesDirectSelectionForTest, "the selected inventory item uses its full row as the selection target");
            Assert(armory.CommittedRowIndexForTest >= 0
                && armory.FocusedRowIndexForTest == armory.CommittedRowIndexForTest
                && armory.FocusedRowIsCommittedForTest
                && armory.HoveredRowIndexForTest < 0, "Inventory opens with one committed row owning controller focus and no stale hover");
            int committedInventoryRow = armory.CommittedRowIndexForTest;
            if (armory.VisibleRowCountForTest > 1)
            {
                int previewInventoryRow = committedInventoryRow == 0 ? 1 : 0;
                armory.HoverRowForTest(previewInventoryRow);
                Assert(armory.CommittedRowIndexForTest == committedInventoryRow
                    && armory.FocusedRowIndexForTest == committedInventoryRow
                    && armory.HoveredRowIndexForTest == previewInventoryRow, "Inventory hover previews another row without stealing committed focus");
                armory.FocusRowForTest(committedInventoryRow);
                Assert(armory.FocusedRowIsCommittedForTest
                    && armory.HoveredRowIndexForTest < 0, "Inventory navigation focus clears parked pointer preview");
            }
            InvokePrivate(game, "CloseArmoryOverlay");
            InvokePrivate(game, "LateUpdate");
            InvokePrivate(game, "ToggleArmory", 1);
            InvokePrivate(game, "LateUpdate");
            armory = GetPrivateField<ArmoryOverlayScreen>(game, "armoryOverlayScreen");
            Assert(armory != null
                && armory.IsVisible
                && armory.FocusedRowIsCommittedForTest
                && armory.FocusedRowIndexForTest == armory.CommittedRowIndexForTest
                && armory.HoveredRowIndexForTest < 0, "reopening Inventory restores the committed row as the single action focus");
            InvokePrivate(game, "RunArmoryRowAction", state.Inventory.IndexOf(inventoryProbe));
            InvokePrivate(game, "LateUpdate");
            Assert(armory.VisibleDetailActionCountForTest <= 2, "Inventory reveals only the recommended equip action and optional party chooser");
            if (armory.VisibleDetailActionCountForTest > 1)
            {
                armory.InvokeDetailActionForTest(1);
                InvokePrivate(game, "LateUpdate");
                Assert(armory.VisibleDetailActionCountForTest == state.Party.Count(member => member != null) + 1, "party targets appear only after Choose another");
                armory.InvokeDetailActionForTest(0);
                InvokePrivate(game, "LateUpdate");
                Assert(armory.VisibleDetailActionCountForTest <= 2, "Back returns to the concise recommendation");
            }
            InvokePrivate(game, "RunArmoryDetailAction", 0);
            InvokePrivate(game, "LateUpdate");
            Assert(inventoryProbe.EquippedById == state.Party[0].Id, "explicit inventory target action records the exact equipped owner");
            Assert(state.Party[0].WeaponName == inventoryProbe.DisplayName, "explicit inventory target action updates that adventurer's loadout");
            InvokePrivate(game, "CloseArmoryOverlay");
            InvokePrivate(game, "LateUpdate");

            InventoryItem popupProbe = new InventoryItem
            {
                DisplayName = "Smoke-Test Blade",
                Form = "sword",
                Slot = "weapon",
                Trait = "steady",
                Rarity = "fine",
                Bonus = 2,
                DamageMin = 2,
                DamageMax = 6,
                AttackSpeed = 8
            };
            state.Inventory.Add(popupProbe);
            InvokePrivate(game, "ShowLootPanel", popupProbe, 3, 1, 0, "Tessa: \"A real reward keeps its own window.\"", "Popup Probe");
            InvokePrivate(game, "LateUpdate");
            Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Loot, "loot reward opens its own overlay");
            AssertRootOverlayCanvas("Loot Popup Canvas");
            Assert(InvokePrivate<bool>(game, "HasRenderableGameplayOverlay", UiOverlay.Loot), "IMGUI yields the frame to visible loot");
            LootPopupScreen lootPopup = GetPrivateField<LootPopupScreen>(game, "lootPopupScreen");
            Assert(lootPopup != null && lootPopup.HasRealIconForTest, "loot reward uses the real inventory item atlas");
            Assert(lootPopup.HasReviewActionForTest, "acquired gear offers a direct equipment review action");
            Assert(lootPopup.PrimaryActionLabelForTest == "Continue", "loot action accurately reflects that rewards are already acquired");
            lootPopup.InvokeReviewForTest();
            InvokePrivate(game, "LateUpdate");
            Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Armory, "Review equipment opens Inventory on the exact acquired item");
            armory = GetPrivateField<ArmoryOverlayScreen>(game, "armoryOverlayScreen");
            Assert(armory != null && armory.ActiveTabLabelForTest == "Inventory" && armory.HasVisibleDetailForTest, "loot review lands on the inventory comparison pane");
            InvokePrivate(game, "CloseArmoryOverlay");
            InvokePrivate(game, "LateUpdate");

            InventoryItem dialogueReward = new InventoryItem
            {
                DisplayName = "Conversation Reward",
                Form = "focus",
                Slot = "focus",
                Trait = "steady",
                Rarity = "fine"
            };
            InvokePrivate(
                game,
                "ShowDialogueThenLoot",
                "Reward Conversation",
                "Tessa",
                "The spoken line remains in a dialogue window before the reward comparison opens.",
                ObjectType.Armorer,
                Color.cyan,
                dialogueReward,
                2,
                0,
                0,
                "The focus goes into the pack.",
                "Conversation Reward");
            InvokePrivate(game, "LateUpdate");
            Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Dialogue, "NPC reward sequence begins with readable dialogue");
            DialogueScreen rewardDialogue = UnityEngine.Object.FindFirstObjectByType<DialogueScreen>();
            Assert(rewardDialogue != null && rewardDialogue.IsInteractiveAndVisible, "reward dialogue owns input before loot");
            rewardDialogue.InvokeContinueForTest();
            InvokePrivate(game, "LateUpdate");
            Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Loot, "closing reward dialogue advances to loot comparison");
            Assert(GetPrivateField<bool>(game, "lootPanelRequiresDismissal"), "reward loot waits for explicit dismissal");
            InvokePrivate(game, "DismissLootPopup");
            InvokePrivate(game, "LateUpdate");

            InvokePrivate(game, "OpenPauseMenu");
            InvokePrivate(game, "LateUpdate");
            Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Pause, "Menu command opens Pause overlay");
            AssertRootOverlayCanvas("Pause Menu Canvas");
            Assert(InvokePrivate<bool>(game, "HasRenderableGameplayOverlay", UiOverlay.Pause), "IMGUI yields the frame to visible Pause menu");
            InvokePrivate(game, "ClosePauseMenu");
            InvokePrivate(game, "LateUpdate");

            InvokePrivate(game, "OpenHelpOverlay");
            InvokePrivate(game, "LateUpdate");
            Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Help, "F1 Help opens its overlay");
            AssertRootOverlayCanvas("Help Overlay Canvas");
            Assert(InvokePrivate<bool>(game, "HasRenderableGameplayOverlay", UiOverlay.Help), "IMGUI yields the frame to visible Help");
            InvokePrivate(game, "CloseHelpOverlay");
            InvokePrivate(game, "LateUpdate");

            state.PlayerX = standX;
            state.PlayerY = standY;
            InvokePrivate(game, "TryMoveOrUseExplore", dx, dy);
            Assert(state.PlayerX == standX && state.PlayerY == standY, "keyboard bump-use does not overlap blocking target");
            UiOverlay overlayAfterBump = InvokePrivate<UiOverlay>(game, "CurrentUiOverlay");
            bool targetRemoved = state.Map.Objects == null || !state.Map.Objects.Contains(target);
            Assert(overlayAfterBump != UiOverlay.None || targetRemoved || state.Mode == GameMode.Combat, "keyboard bump-use resolves the exact adjacent target");
            InvokePrivate(game, "CloseTopOverlay");

            MapObject stairs = state.Map.Objects.Find(obj => obj != null && obj.Type == ObjectType.Stairs);
            if (stairs != null)
            {
                Assert(InvokePrivate<bool>(game, "CanStepExplore", stairs.X, stairs.Y), "stairs remain standable after collision hardening");
            }

            AssertSewerSliceStoryFlow(game, state);
        }

        private static void AssertSewerSliceStoryFlow(AshenHallsGame game, GameState state)
        {
            InvokePrivate(game, "CloseTopOverlay");
            state.StoryFlags = new List<string>();
            state.Inventory = new List<InventoryItem>();
            ContentSetCatalog.MarkSewerSliceContractAccepted(state.StoryFlags);

            InvokePrivate(game, "ApplyMidgaardStoryVictory", "sewer_broken_sluice");
            Assert(ContentSetCatalog.SewerSliceClearedCount(state.StoryFlags) == 1, "production story flow records Broken Sluice");
            Assert(ContentSetCatalog.CountSewerSliceProof(state.Inventory) == 1, "production story flow grants first sewer proof");
            ExplorationHudView secondRoomView = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
            Assert(secondRoomView.ObjectiveSummary.IndexOf("Foul Runoff", StringComparison.OrdinalIgnoreCase) >= 0, "compact objective advances to Foul Runoff");
            Assert(secondRoomView.WaypointLine.IndexOf("Sewer", StringComparison.OrdinalIgnoreCase) >= 0
                && secondRoomView.WaypointLine.IndexOf(" / ", StringComparison.OrdinalIgnoreCase) >= 0
                && secondRoomView.WaypointLine.IndexOf("step", StringComparison.OrdinalIgnoreCase) >= 0,
                "Golden Thread keeps the next sewer room physically actionable after Broken Sluice");

            InvokePrivate(game, "ApplyMidgaardStoryVictory", "sewer_foul_runoff");
            InvokePrivate(game, "LateUpdate");
            Assert(ContentSetCatalog.SewerSliceClearedCount(state.StoryFlags) == 2, "production story flow records Foul Runoff");
            Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Dialogue, "Foul Runoff opens the safe-room choice");
            DialogueChoiceView[] choices = GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices");
            Assert(choices != null && choices.Length == 2, "safe room offers exactly two readable gear choices");

            InvokePrivate(game, "ChooseDialogueChoice", "focus");
            InvokePrivate(game, "LateUpdate");
            Assert(ContentSetCatalog.HasSewerSafeRoomChoice(state.StoryFlags), "safe-room callback records its claim");
            Assert(state.StoryFlags.Contains(StoryFlags.SewerSafeRoomFocusChosen), "safe-room callback records the selected focus");
            Assert(state.Inventory.Count(item => item != null && item.DisplayName == "+2 etched stormglass ritual staff") == 1, "safe-room focus enters inventory once");
            Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Loot, "safe-room choice opens equipment comparison");
            ExplorationHudView finalRoomView = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
            Assert(finalRoomView.ObjectiveSummary.IndexOf("Cistern Den", StringComparison.OrdinalIgnoreCase) >= 0, "compact objective advances to Cistern Den");
            Assert(finalRoomView.WaypointLine.IndexOf("Sewer", StringComparison.OrdinalIgnoreCase) >= 0,
                "Golden Thread returns to the sewer entrance after the safe-room choice");
            InvokePrivate(game, "DismissLootPopup");
            InvokePrivate(game, "LateUpdate");

            InvokePrivate(game, "ApplyMidgaardStoryVictory", "sewer_cistern_den");
            Assert(ContentSetCatalog.SewerSliceClearedCount(state.StoryFlags) == 3, "production story flow records Cistern Den");
            Assert(ContentSetCatalog.SewerSliceRewardReady(state.StoryFlags, state.Inventory), "three production victories make Borin's reward ready");
            ExplorationHudView rewardReadyView = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
            Assert(rewardReadyView.WaypointLine.IndexOf("Borin", StringComparison.OrdinalIgnoreCase) >= 0,
                "Golden Thread names Borin when all three proof bundles are ready");

            int rewardReadyX = state.PlayerX;
            int rewardReadyY = state.PlayerY;
            MapObject armorerDoor = MidgaardInteriorRules.FindById(state.Map, MidgaardInteriorRules.ArmorerDoorId);
            MapObject armorerNpc = state.Map.Objects.Single(obj => obj != null && obj.Type == ObjectType.ArmorerNpc);
            MapObject armorerExit = MidgaardInteriorRules.FindById(state.Map, MidgaardInteriorRules.ArmorerExitId);
            Assert(InvokePrivate<bool>(game, "TryUseMidgaardPortal", armorerDoor), "reward-ready guidance probe enters the merchant hall");
            ExplorationHudView merchantGuidance = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
            Assert(merchantGuidance.WaypointLine.IndexOf("Armorer Borin", StringComparison.OrdinalIgnoreCase) >= 0
                && merchantGuidance.WaypointLine.IndexOf("Borin's Armory", StringComparison.OrdinalIgnoreCase) < 0,
                "merchant-hall Golden Thread retargets Borin instead of the exterior armory");
            Assert(TryFindAdjacentProbeTile(game, state, armorerNpc, out int borinStandX, out int borinStandY),
                "Borin has a reachable adjacent Golden Thread interaction tile");
            state.PlayerX = borinStandX;
            state.PlayerY = borinStandY;
            ExplorationHudView borinUseGuidance = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
            Assert(borinUseGuidance.WaypointLine.IndexOf("E / Space", StringComparison.OrdinalIgnoreCase) >= 0
                && borinUseGuidance.WaypointLine.IndexOf("Borin", StringComparison.OrdinalIgnoreCase) >= 0,
                "adjacent reward-ready Borin becomes an exact contextual-use instruction");

            Assert(InvokePrivate<bool>(game, "TryCompleteRatPeltArmor"), "production armorer path claims the first reward");
            InvokePrivate(game, "LateUpdate");
            Assert(ContentSetCatalog.SewerSliceComplete(state.StoryFlags), "production armorer path completes the sewer slice");
            Assert(ContentSetCatalog.AllowKoboldChapter(ContentSetCatalog.SewerSlice, state.StoryFlags), "Borin's reward opens the bounded Chapter II route");
            Assert(state.Inventory.Count(item => item != null && item.Material == "rat pelt" && item.Slot == "armor") == 1, "production reward exists once");
            ExplorationHudView merchantExitGuidance = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
            Assert(merchantExitGuidance.WaypointLine.IndexOf("Merchant Hall exit", StringComparison.OrdinalIgnoreCase) >= 0
                && merchantExitGuidance.WaypointLine.IndexOf("Borin", StringComparison.OrdinalIgnoreCase) < 0,
                "completed merchant-hall work redirects the Golden Thread through the reachable interior exit");
            Assert(InvokePrivate<bool>(game, "TryUseMidgaardPortal", armorerExit), "completed merchant-hall guidance probe leaves through its exit");
            Assert(!InvokePrivate<bool>(game, "IsMidgaardInteriorCell", state.PlayerX, state.PlayerY, state.Map, state.Depth),
                "merchant-hall exit guidance returns to Midgaard streets");
            state.PlayerX = rewardReadyX;
            state.PlayerY = rewardReadyY;
            IReadOnlyList<ArmoryRowView> chapterTwoJournal = InvokePrivate<IReadOnlyList<ArmoryRowView>>(game, "BuildArmoryJournalRows");
            Assert(chapterTwoJournal.Any(row => row.Title == "Old Road Chapter II" && row.Subtitle == "Sluice Steps open"), "production Journal names the live Chapter II route after Borin's reward");
            Assert(chapterTwoJournal.Any(row => row.Title == "Kobold Smoke - Dusk Market Ambush" && row.Subtitle == "current"), "production Journal exposes the bounded Dusk Market step without prototype scaffolds");
            Assert(chapterTwoJournal.Any(row => row.Title == "Outer Road Chart"), "production Journal unlocks the route chart with the Old Road");
            Assert(chapterTwoJournal.All(row => ((row.Title ?? "") + " " + (row.Subtitle ?? "") + " " + (row.Detail ?? ""))
                .IndexOf("teaser", StringComparison.OrdinalIgnoreCase) < 0), "production Journal never calls the playable Old Road a future teaser");
            WorldMapJunction journalWaypoint = WorldMapGenerationRules.RegionalJunctions(
                state.Map.Width,
                state.Map.Height,
                state.Map.StartX,
                state.Map.StartY)[0];
            string journalWaypointDiscovery = RouteChartRules.DiscoveryKey(state.Depth, journalWaypoint.Id);
            bool addedJournalWaypointDiscovery = !state.DiscoveredZones.Contains(journalWaypointDiscovery);
            if (addedJournalWaypointDiscovery) state.DiscoveredZones.Add(journalWaypointDiscovery);
            IReadOnlyList<ArmoryRowView> chartedJournal = InvokePrivate<IReadOnlyList<ArmoryRowView>>(game, "BuildArmoryJournalRows");
            ArmoryRowView waypointRow = chartedJournal.Single(row => row.Title == journalWaypoint.Name);
            Assert(waypointRow.ActionLabel == "Mark" && waypointRow.ActionEnabled, "charted production Journal row exposes a usable Mark action");
            int previousArmoryTab = GetPrivateField<int>(game, "armoryTab");
            SetPrivateField(game, "armoryTab", 3);
            InvokePrivate(game, "RunArmoryRowAction", waypointRow.Key);
            Assert(RouteChartRules.IsWaypoint(state.ActiveRouteWaypointKey, state.Depth, journalWaypoint.Id), "Journal Mark action persists the selected route waypoint");
            ExplorationHudView markedGuidance = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
            Assert(markedGuidance.WaypointLine.IndexOf("Marked:", StringComparison.OrdinalIgnoreCase) >= 0
                && (markedGuidance.WaypointLine.IndexOf(" / Up", StringComparison.OrdinalIgnoreCase) >= 0
                    || markedGuidance.WaypointLine.IndexOf(" / Down", StringComparison.OrdinalIgnoreCase) >= 0
                    || markedGuidance.WaypointLine.IndexOf(" / Left", StringComparison.OrdinalIgnoreCase) >= 0
                    || markedGuidance.WaypointLine.IndexOf(" / Right", StringComparison.OrdinalIgnoreCase) >= 0)
                && markedGuidance.WaypointLine.IndexOf(journalWaypoint.Name, StringComparison.OrdinalIgnoreCase) >= 0,
                "explicit Journal waypoint takes visible Golden Thread precedence");
            IReadOnlyList<Point> markedPlanPath = InvokePrivate<IReadOnlyList<Point>>(game, "CurrentExploreGuidancePath");
            Assert(InvokePrivate<bool>(game, "CurrentExploreGuidanceIsMarked")
                && InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName") == journalWaypoint.Name
                && markedPlanPath.Count > 1
                && markedPlanPath[0].X == state.PlayerX
                && markedPlanPath[0].Y == state.PlayerY,
                "the map consumes the same player-selected target and path that replaced story guidance in NEXT");
            int markedProbeX = state.PlayerX;
            int markedProbeY = state.PlayerY;
            state.PlayerX = journalWaypoint.X;
            state.PlayerY = journalWaypoint.Y;
            InvokePrivate(game, "InvalidateActiveRouteWaypointPath");
            ExplorationHudView reachedMarkedGuidance = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
            Assert(reachedMarkedGuidance.WaypointLine.StartsWith("J | Marked: " + journalWaypoint.Name, StringComparison.Ordinal)
                && reachedMarkedGuidance.WaypointLine.IndexOf("open Journal to Clear", StringComparison.OrdinalIgnoreCase) >= 0,
                "reaching a marked waypoint gives the exact action that resumes story guidance");
            state.PlayerX = markedProbeX;
            state.PlayerY = markedProbeY;
            InvokePrivate(game, "InvalidateActiveRouteWaypointPath");
            ArmoryRowView selectedWaypointRow = InvokePrivate<IReadOnlyList<ArmoryRowView>>(game, "BuildArmoryJournalRows")
                .Single(row => row.Title == journalWaypoint.Name);
            Assert(selectedWaypointRow.Selected && selectedWaypointRow.ActionLabel == "Clear", "selected Journal waypoint becomes a highlighted Clear action");
            InvokePrivate(game, "RunArmoryRowAction", selectedWaypointRow.Key);
            Assert(string.IsNullOrEmpty(state.ActiveRouteWaypointKey), "Journal Clear action removes the route waypoint");
            ExplorationHudView restoredStoryGuidance = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
            Assert(restoredStoryGuidance.WaypointLine.IndexOf("Sluice Steps", StringComparison.OrdinalIgnoreCase) >= 0
                && restoredStoryGuidance.WaypointLine.IndexOf("Marked:", StringComparison.OrdinalIgnoreCase) < 0,
                "clearing a Journal waypoint immediately restores the Chapter II story thread");
            SetPrivateField(game, "armoryTab", previousArmoryTab);
            MapObject oldRoadDescent = state.Map.FindObjectById("old-road-descent-sluice-steps");
            Assert(oldRoadDescent != null && oldRoadDescent.Type == ObjectType.Stairs, "chapter reward creates one stable Sluice Steps descent");
            bool[,] oldRoadReachable = ExplorationTraversalRules.ReachableMask(state.Map, state.PlayerX, state.PlayerY);
            Assert(ExplorationTraversalRules.CanReachObject(oldRoadReachable, state.Map, oldRoadDescent), "Sluice Steps is reachable from the current Midgaard component");
            Assert(!ContentSetCatalog.ShowPrototypeScaffold(ContentSetCatalog.SewerSlice)
                && !ContentSetCatalog.AllowPrototypeRouteTriggers(ContentSetCatalog.SewerSlice, state.StoryFlags), "unlocking Chapter II leaves generic prototype systems disabled");
            Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Dialogue, "chapter reward begins with Borin's dialogue");
            DialogueScreen rewardDialogue = UnityEngine.Object.FindFirstObjectByType<DialogueScreen>();
            Assert(rewardDialogue != null && rewardDialogue.IsInteractiveAndVisible, "chapter reward dialogue owns input");
            rewardDialogue.InvokeContinueForTest();
            InvokePrivate(game, "LateUpdate");
            Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Loot, "chapter reward advances to comparison");
            InvokePrivate(game, "DismissLootPopup");
            InvokePrivate(game, "LateUpdate");

            InvokePrivate(game, "ToggleArmory", 3);
            InvokePrivate(game, "LateUpdate");
            ArmoryOverlayScreen journalOverlay = GetPrivateField<ArmoryOverlayScreen>(game, "armoryOverlayScreen");
            int waypointVisibleIndex = journalOverlay == null ? -1 : journalOverlay.VisibleRowIndexForKeyForTest(waypointRow.Key);
            Assert(journalOverlay != null && journalOverlay.IsVisible && waypointVisibleIndex >= 0, "production Journal renders the charted waypoint in the live overlay");
            journalOverlay.ScrollRowIntoViewForTest(waypointVisibleIndex);
            float waypointScrollOffset = journalOverlay.ScrollOffsetForTest;
            Assert(waypointScrollOffset > 0f && journalOverlay.IsRowFullyVisibleForTest(waypointVisibleIndex), "charted waypoint can be scrolled fully into view");
            journalOverlay.InvokeRowActionForTest(waypointVisibleIndex);
            InvokePrivate(game, "LateUpdate");
            Assert(RouteChartRules.IsWaypoint(state.ActiveRouteWaypointKey, state.Depth, journalWaypoint.Id), "live Journal Mark action persists the waypoint");
            Assert(Mathf.Approximately(journalOverlay.ScrollOffsetForTest, waypointScrollOffset)
                && journalOverlay.IsRowFullyVisibleForTest(waypointVisibleIndex), "Mark refresh preserves the acted-on waypoint and Journal scroll");
            journalOverlay.InvokeRowActionForTest(waypointVisibleIndex);
            InvokePrivate(game, "LateUpdate");
            Assert(string.IsNullOrEmpty(state.ActiveRouteWaypointKey)
                && Mathf.Approximately(journalOverlay.ScrollOffsetForTest, waypointScrollOffset), "Clear refresh preserves Journal scroll while removing the waypoint");
            float journalContentHeight = journalOverlay.ContentHeightForTest;
            InvokePrivate(game, "SelectArmoryTab", 0);
            InvokePrivate(game, "LateUpdate");
            Assert(Mathf.Approximately(journalOverlay.ScrollOffsetForTest, 0f), "changing Armory tabs resets list scroll");
            Assert(journalOverlay.ContentHeightForTest <= Mathf.Max(journalOverlay.ViewportHeightForTest, journalContentHeight - 1f), "short tabs size content from visible rows instead of the pooled row count");
            InvokePrivate(game, "CloseArmoryOverlay");
            InvokePrivate(game, "LateUpdate");
            if (addedJournalWaypointDiscovery) state.DiscoveredZones.Remove(journalWaypointDiscovery);

            state.PlayerX = oldRoadDescent.X;
            state.PlayerY = oldRoadDescent.Y;
            Assert(InvokePrivate<bool>(game, "CanDescend"), "the named Sluice Steps marker authorizes the first normal-play descent");
            InvokePrivate(game, "Descend");
            Assert(state.Depth == 2 && state.Map != null, "normal sewer-slice play descends into the Chapter II regional map");
            Assert(state.Map.Width == WorldMapGenerationRules.Width && state.Map.Height == WorldMapGenerationRules.Height, "Chapter II uses the expanded fresh-map dimensions");
            MapObject smokeCave = state.Map.FindObjectById("dusk-market-smoke-cave");
            Assert(smokeCave != null && smokeCave.Type == ObjectType.Cave, "Chapter II promotes one stable Dusk Market smoke cave");
            bool[,] chapterTwoReachable = ExplorationTraversalRules.ReachableMask(state.Map, state.PlayerX, state.PlayerY);
            Assert(ExplorationTraversalRules.CanReachObject(chapterTwoReachable, state.Map, smokeCave), "Dusk Market smoke cave remains reachable after descent");
            Assert(InvokePrivate<bool>(game, "IsKoboldStoryCave", smokeCave), "the stable Dusk Market smoke cave owns the Chapter II cave route");
            MapObject ordinaryDuskCave = new MapObject(smokeCave.X, smokeCave.Y, ObjectType.Cave, "ordinary-dusk-cave");
            Assert(!InvokePrivate<bool>(game, "IsKoboldStoryCave", ordinaryDuskCave), "an ordinary Dusk Market cave cannot impersonate the Chapter II route");
            InvokePrivate(game, "ResolveExploreObject", smokeCave);
            Assert(!state.StoryFlags.Contains(StoryFlags.KoboldCaveFound) && state.Mode == GameMode.Explore, "the smoke cave stays sealed until the Dusk Market ambush is survived");
            Assert(!state.Map.Objects.Any(obj => obj != null && (obj.Type == ObjectType.Encounter || obj.Type == ObjectType.Stairs)), "normal Chapter II map still prunes generic patrol and stair scaffolds");
            ObjectType[] scaffoldTypes =
            {
                ObjectType.Waystone,
                ObjectType.TrainingGround,
                ObjectType.LoreLibrary,
                ObjectType.ForgeSite,
                ObjectType.FactionCamp,
                ObjectType.DungeonGate,
                ObjectType.DeepCrypt,
                ObjectType.AncientGrove,
                ObjectType.PortalSeal
            };
            Assert(state.Map.Objects
                .Where(obj => obj != null && scaffoldTypes.Contains(obj.Type))
                .All(obj => !string.IsNullOrEmpty(obj.Id) && obj.Id.StartsWith("regional-site:", StringComparison.Ordinal)), "only the eight explicitly authored regional sites reuse scaffold landmark types");
            if (!state.StoryFlags.Contains(StoryFlags.KoboldAmbushSprung))
            {
                state.StoryFlags.Add(StoryFlags.KoboldAmbushSprung);
            }
            state.PlayerX = smokeCave.X;
            state.PlayerY = smokeCave.Y;
            Assert(InvokePrivate<bool>(game, "MaybeTriggerKoboldAmbush"), "an unresolved sprung ambush retriggers after retreat");
            Assert(state.Mode == GameMode.Combat && state.Combat?.EncounterStyle == "koboldambush", "the retry opens the bounded kobold ambush encounter");
            InvokePrivate(game, "ApplyKoboldStoryVictory", "koboldambush");
            Assert(state.StoryFlags.Contains(StoryFlags.KoboldAmbushSurvived), "Chapter II ambush victory advances the promoted route");
            state.Mode = GameMode.Explore;
            state.Combat = null;
            InvokePrivate(game, "ResolveExploreObject", smokeCave);
            Assert(state.StoryFlags.Contains(StoryFlags.KoboldCaveFound)
                && state.Mode == GameMode.Combat
                && state.Combat?.EncounterStyle == "koboldcave", "the stable smoke cave opens only after the ambush victory");
            InvokePrivate(game, "ApplyKoboldStoryVictory", "koboldcave");
            Assert(state.StoryFlags.Contains(StoryFlags.KoboldCaveCleared), "Chapter II smoke-cave victory advances to the king's hall");
            IReadOnlyList<ArmoryRowView> kingRoadJournal = InvokePrivate<IReadOnlyList<ArmoryRowView>>(game, "BuildArmoryJournalRows");
            Assert(kingRoadJournal.Any(row => row.Title == "Kobold Smoke - Dusk Market Ambush" && row.Subtitle == "complete"), "production Journal retains completed ambush state");
            Assert(kingRoadJournal.Any(row => row.Title == "Kobold Smoke - Smoke Cave" && row.Subtitle == "complete"), "production Journal retains completed smoke-cave state");
            Assert(kingRoadJournal.Any(row => row.Title == "Kobold Smoke - Varkh's Hall" && row.Subtitle == "current"), "production Journal promotes Varkh's Hall as the next live route step");
        }

        private static void AssertCombatPresentationRuntime(AshenHallsGame game)
        {
            InvokePrivate(game, "StartBetaCombatLab");
            InvokePrivate(game, "LateUpdate");
            AssertMode(game, GameMode.Combat, "Beta Lab reaches Combat");

            CombatHudScreen hud = GetPrivateField<CombatHudScreen>(game, "combatHudScreen");
            Assert(hud != null && hud.IsReady && hud.IsVisible, "migrated combat HUD is active in combat");
            Assert(hud.HasRenderableGeometry && hud.HasUsableCommandBar, "migrated combat HUD exposes an interactive action bar");
            Assert(!InvokePrivate<bool>(game, "NeedsEmergencyCombatHudFallback"), "healthy migrated combat HUD suppresses the recovery action bar");
            CombatHudView hudView = InvokePrivate<CombatHudView>(game, "BuildCombatHudView");
            CombatUnit active = InvokePrivate<CombatUnit>(game, "CurrentUnit");
            GameState combatState = GetPrivateField<GameState>(game, "state");
            Texture2D combatTerrain = GetPrivateField<Texture2D>(game, "combatTerrainAtlas");
            Texture2D koboldCombatTerrain = GetPrivateField<Texture2D>(game, "koboldCombatTerrainAtlas");
            Texture2D biomeProps = GetPrivateField<Texture2D>(game, "worldMapBiomePropAtlas");
            Texture2D lightningSpellIcons = GetPrivateField<Texture2D>(game, "lightningSpellIconAtlas");
            Texture2D signatureSpellIcons = GetPrivateField<Texture2D>(game, "signatureSpellIconAtlas");
            Texture2D abilityIcons = GetPrivateField<Texture2D>(game, "abilityIconAtlas");
            Texture2D powerBookStateIcons = GetPrivateField<Texture2D>(game, "powerBookStateIconAtlas");
            Assert(combatTerrain != null && InvokePrivate<bool>(game, "IsCombatTerrainAtlas"), "combat terrain atlas passes the production guard for authored hazards");
            Assert(koboldCombatTerrain != null && InvokePrivate<bool>(game, "IsKoboldCombatTerrainAtlas"), "semantic field atlas passes the production guard for gas, wards, and rituals");
            Assert(biomeProps != null && InvokePrivate<bool>(game, "IsWorldMapBiomePropAtlas"), "transparent biome prop atlas passes the production guard for combat cover");
            Assert(lightningSpellIcons != null && InvokePrivate<bool>(game, "IsLightningSpellIconAtlas"), "dedicated lightning spell atlas passes its production guard");
            Assert(signatureSpellIcons != null && InvokePrivate<bool>(game, "IsSignatureSpellIconAtlas"), "complete signature spell atlas passes its production guard");
            Assert(abilityIcons != null && InvokePrivate<bool>(game, "IsAbilityIconAtlas"), "complete ability atlas passes its production guard");
            Assert(powerBookStateIcons != null
                && CombatIconCatalog.IsBookStateAtlasDimensions(powerBookStateIcons.width, powerBookStateIcons.height),
                "power-book state atlas preserves the exact 4x3 microicon contract");
            Assert(InvokePrivate<int>(game, "CombatCoverBiomePropIndex", "tree") == 0, "combat tree cover resolves transparent authored world art");
            Assert(InvokePrivate<int>(game, "CombatCoverBiomePropIndex", "stone") == 8, "combat stone cover resolves transparent authored rock art");
            Point gasProbe = new Point(0, 0, "gas", 3);
            Assert(InvokePrivate<int>(game, "KoboldCombatTerrainTextureIndex", 0, 0, gasProbe, 0) == 6, "generic gas fields borrow the authored fume tile instead of grass");
            Point sanctuaryProbe = new Point(0, 0, "sanctuary", 3);
            Assert(InvokePrivate<int>(game, "KoboldCombatTerrainTextureIndex", 0, 0, sanctuaryProbe, 0) == 14, "generic Sanctuary fields resolve authored ward-circle art");
            Point ritualProbe = new Point(0, 0, "glyph", 3);
            Assert(InvokePrivate<int>(game, "KoboldCombatTerrainTextureIndex", 0, 0, ritualProbe, 0) == 4, "generic ritual fields resolve authored ground sigils");
            Point visibleIce = combatState.Combat.Obstacles.FirstOrDefault(obstacle =>
                obstacle != null
                && obstacle.Kind == "ice"
                && !combatState.Combat.Units.Any(unit => unit != null && unit.Hp > 0 && unit.X == obstacle.X && unit.Y == obstacle.Y));
            Assert(visibleIce != null && visibleIce.X == 8 && visibleIce.Y == 6, "Beta Lab exposes authored ice art on an open cell");
            Assert(hudView.Commands != null && hudView.Commands.Count == 6, "production combat model exposes all six command rows");
            Assert(hudView.RoundNumber == combatState.Combat.Round
                && hudView.RoundLabel == $"ROUND\n{combatState.Combat.Round}", "combat header exposes the current round instead of exploration currency");
            Assert(hudView.MovePoints == combatState.Combat.MovePoints
                && hudView.MovePointsMaximum == InvokePrivate<int>(game, "UnitMoveAllowance", active)
                && hudView.MoveLabel == $"MOVE\n{hudView.MovePoints} / {hudView.MovePointsMaximum}", "combat header exposes current and maximum movement");
            Assert(hudView.ActionReady == combatState.Combat.ActionAvailable
                && hudView.ActionLabel == "ACTION\nREADY", "combat header exposes the live action state");
            Assert(!string.IsNullOrWhiteSpace(hudView.CommandPrompt), "production combat model exposes one canonical command prompt");
            Assert(hudView.PhaseLine.StartsWith("YOUR TURN", StringComparison.Ordinal), "player initiative is announced as the primary combat phase cue");
            Assert(hudView.ActiveUnit == null || hudView.ActiveUnit.StatusLine != "steady", "empty combat conditions use player-facing copy");
            Assert(hudView.ActiveUnit != null
                && hudView.ActiveUnit.StateLine.Contains("MOVE")
                && hudView.ActiveUnit.StateLine.Contains("ACTION READY"), "active combatant card exposes movement and action economy at a glance");
            Assert(hudView.ActiveUnit.StatusLine.Contains(InvokePrivate<string>(game, "ActiveThreatSummary", active)), "active combatant card includes the authoritative incoming-threat summary");
            int currentDirectThreats = InvokePrivate<int>(game, "DirectThreatCount", active);
            int currentPressureThreats = Math.Max(0, InvokePrivate<int>(game, "PressureThreatCount", active) - currentDirectThreats);
            Vector2Int projectedCurrentThreats = InvokePrivate<Vector2Int>(
                game,
                "ProjectedMoveThreatCounts",
                active,
                active.X,
                active.Y);
            Assert(projectedCurrentThreats == new Vector2Int(currentDirectThreats, currentPressureThreats), "movement destination threat preview matches the authoritative current-tile threat calculation");
            Assert(
                InvokePrivate<string>(game, "ProjectedMoveThreatSummary", active, active.X, active.Y)
                    == CombatThreatRules.MovementDestinationLabel(projectedCurrentThreats.x, projectedCurrentThreats.y),
                "movement destination threat copy is generated from the projected responsible-enemy counts");
            CombatHudCommandView pickerCommand = hudView.Commands[2];
            Assert(
                pickerCommand.Label == (pickerCommand.Mode == ActionMode.Ability ? "Skills" : "Spells"),
                "third combat command names its actual Skills or Spells panel");
            Assert(!string.IsNullOrWhiteSpace(hudView.TargetTitle) && hudView.TargetTitle != "Target", "combat side card names inspection or targeting context");
            string originalEncounterStyle = combatState.Combat.EncounterStyle;
            int originalRound = combatState.Combat.Round;
            combatState.Combat.EncounterStyle = "sewer_broken_sluice";
            combatState.Combat.Round = 1;
            CombatHudView guidedHudView = InvokePrivate<CombatHudView>(game, "BuildCombatHudView");
            Assert(guidedHudView.TacticalLine.Contains("Cairn shoots"), "authored first-round combat publishes its concise tactical plan");
            combatState.Combat.Round = 2;
            Assert(string.IsNullOrEmpty(InvokePrivate<CombatHudView>(game, "BuildCombatHudView").TacticalLine), "opening tactical plan yields after round one");
            combatState.Combat.EncounterStyle = originalEncounterStyle;
            combatState.Combat.Round = originalRound;
            Assert(hudView.Commands.All(command => command.IconTexture != null && command.IconSource.width > 1f && command.IconSource.height > 1f), "all six migrated combat commands resolve generated atlas art");
            IReadOnlyList<ActionMode> fallbackModes = InvokePrivate<IReadOnlyList<ActionMode>>(game, "CombatHudFallbackModes", active);
            Assert(fallbackModes != null && fallbackModes.Count == 6, "production IMGUI action bar exposes six commands");
            Assert(fallbackModes[0] == ActionMode.Move && fallbackModes[1] == ActionMode.Attack && fallbackModes[5] == ActionMode.Wait, "action bar keeps Move/Attack/End Turn in stable positions");
            Assert(hudView.Turns != null && hudView.Turns.Count > 1, "combat HUD publishes multiple upcoming turns");
            Assert(hudView.Turns[0].Active, "combat Timeline begins with the active unit");
            hud.Refresh();
            Assert(hud.RoundNumberForTest == hudView.RoundNumber
                && hud.MovePointsForTest == hudView.MovePoints
                && hud.MovePointsMaximumForTest == hudView.MovePointsMaximum
                && hud.ActionReadyForTest == hudView.ActionReady, "rendered combat header matches its authoritative model values");
            Assert(hud.RoundLabelForTest.StartsWith("ROUND", StringComparison.Ordinal)
                && hud.MoveLabelForTest.StartsWith("MOVE", StringComparison.Ordinal)
                && hud.ActionLabelForTest.StartsWith("ACTION", StringComparison.Ordinal), "rendered top stats contain only combat decision labels");
            Assert(hud.CommandCapacityForTest == hudView.Commands.Count, "combat command rendering capacity follows the model count");
            Assert(hudView.TimelineExpanded || hud.VisibleLogCount == Math.Min(1, hudView.Logs.Count), "collapsed combat Timeline keeps the latest event visible");
            EventSystem combatUiEventSystem = EventSystem.current
                ?? (!Application.isPlaying ? UiRuntime.EnsureEventSystemReady() : null);
            combatUiEventSystem?.SetSelectedGameObject(null);
            ActionMode commandModeBeforePreview = GetPrivateField<ActionMode>(game, "selectedAction");
            hud.HoverCommandForTest(pickerCommand.Mode);
            Assert(hud.CommandPromptForTest.Contains(pickerCommand.Label)
                && hud.CommandPromptForTest.Contains(pickerCommand.Tooltip)
                && hud.HoveredCommandForTest == pickerCommand.Mode
                && hud.FocusedCommandForTest == pickerCommand.Mode
                && hud.ContextCommandForTest == pickerCommand.Mode
                && hud.PointerOwnsCommandContextForTest, "combat command pointer entry transfers focus and publishes one matching action context");
            Assert(GetPrivateField<ActionMode>(game, "selectedAction") == commandModeBeforePreview, "combat command hover never changes the armed gameplay mode");
            hud.ClearCommandHoverForTest();
            Assert(hud.HoveredCommandForTest == null
                && hud.FocusedCommandForTest == pickerCommand.Mode
                && hud.ContextCommandForTest == pickerCommand.Mode
                && !hud.PointerOwnsCommandContextForTest
                && hud.CommandPromptForTest.Contains(pickerCommand.Tooltip), "leaving a combat command keeps its real focused action and explanation");
            hud.FocusCommand(pickerCommand.Mode);
            Assert(
                hud.FocusedCommandForTest == pickerCommand.Mode,
                $"controller focus reaches {pickerCommand.Mode}; actual focus is {hud.FocusedCommandForTest?.ToString() ?? "none"}");
            bool combatHudOwnsFocusedControl = hud.OwnsSelection(combatUiEventSystem?.currentSelectedGameObject);
            Assert(combatHudOwnsFocusedControl, "combat HUD recognizes its focused command as owned UI input");
            Assert(
                !CombatInputRoutingRules.ShouldRouteToWorld(combatHudOwnsFocusedControl, CombatHotkeyKind.Navigation)
                && !CombatInputRoutingRules.ShouldRouteToWorld(combatHudOwnsFocusedControl, CombatHotkeyKind.Submit)
                && CombatInputRoutingRules.ShouldRouteToWorld(combatHudOwnsFocusedControl, CombatHotkeyKind.Dedicated),
                "focused combat commands reserve navigation and Submit without swallowing dedicated hotkeys");
            Assert(
                hud.CommandPromptForTest.Contains(pickerCommand.Tooltip),
                $"controller focus and mouse hover share the same command explanation; expected '{pickerCommand.Tooltip}', got '{hud.CommandPromptForTest}'");
            CombatHudCommandView alternateCommand = hudView.Commands.First(command =>
                command.Mode != pickerCommand.Mode
                && command.Enabled);
            hud.HoverCommandForTest(alternateCommand.Mode);
            Assert(
                hud.FocusedCommandForTest == alternateCommand.Mode
                && hud.HoveredCommandForTest == alternateCommand.Mode
                && hud.ContextCommandForTest == alternateCommand.Mode
                && hud.PointerOwnsCommandContextForTest
                && hud.CommandPromptForTest.Contains(alternateCommand.Tooltip),
                "pointer entry moves focus, prompt context, and Submit ownership together");
            hud.FocusCommand(pickerCommand.Mode);
            Assert(
                hud.FocusedCommandForTest == pickerCommand.Mode
                && hud.HoveredCommandForTest == null
                && hud.ContextCommandForTest == pickerCommand.Mode
                && !hud.PointerOwnsCommandContextForTest
                && hud.CommandPromptForTest.Contains(pickerCommand.Tooltip),
                "controller navigation clears a parked pointer context and restores one focused command truth");
            hud.SetVisible(false);
            Assert(
                hud.FocusedCommandForTest == null
                && hud.CommandPromptForTest == hudView.CommandPrompt,
                "hiding the combat HUD clears transient command context");
            hud.SetVisible(true);
            hud.Refresh();
            Assert(hud.CommandPromptForTest == hudView.CommandPrompt, "reopening the combat HUD starts from its canonical prompt");
            combatUiEventSystem?.SetSelectedGameObject(null);
            hud.ClearCommandFocusForTest();
            Assert(hud.CommandPromptForTest == hudView.CommandPrompt, "clearing controller focus restores the canonical combat prompt");
            bool originalMoved = combatState.Combat.Moved;
            int originalMovePointsForGuard = combatState.Combat.MovePoints;
            combatState.Combat.Moved = false;
            combatState.Combat.MovePoints = InvokePrivate<int>(game, "UnitMoveAllowance", active);
            int gearGuard = InvokePrivate<int>(game, "GearGuardBonus", active);
            Assert(InvokePrivate<int>(game, "GuardActionBonus", active) == 4 + gearGuard, "fresh stance publishes the exact braced Guard bonus");
            Assert(InvokePrivate<string>(game, "ActionButtonSubLabel", ActionMode.Guard, active) == "Guard +" + (4 + gearGuard), "Guard command preview matches the production bonus");
            combatState.Combat.Moved = true;
            Assert(InvokePrivate<int>(game, "GuardActionBonus", active) == 2 + gearGuard, "moved stance publishes the exact reduced Guard bonus");
            combatState.Combat.Moved = originalMoved;
            combatState.Combat.MovePoints = originalMovePointsForGuard;

            CombatUnit forecastTarget = combatState.Combat.Units.First(unit => unit != null && unit.Side == UnitSide.Enemy && unit.Hp > 0);
            active.X = 1;
            active.Y = 1;
            forecastTarget.X = 2;
            forecastTarget.Y = 1;
            CombatAttackForecast attackForecast = InvokePrivate<CombatAttackForecast>(game, "AttackForecast", active, forecastTarget);
            Assert(attackForecast.Legal && attackForecast.HasOutcome, "production attack forecast is legal for an adjacent enemy");
            Assert(InvokePrivate<bool>(game, "CanEnemyAttack", active, forecastTarget) == attackForecast.Legal, "attack reachability shares forecast legality");
            string attackPreview = InvokePrivate<string>(game, "AttackPreview", active, forecastTarget);
            Assert(attackPreview.Contains(attackForecast.HitChance + "% hit"), "attack hover preview uses forecast hit chance");
            Assert(attackPreview.Contains(attackForecast.MinDamage + "-" + attackForecast.MaxDamage), "attack hover preview uses forecast damage range");
            string targetState = InvokePrivate<string>(game, "CombatHudUnitStateLine", forecastTarget, false);
            Assert(targetState.Contains(attackForecast.HitChance + "%") && targetState.Contains(attackForecast.MinDamage + "-" + attackForecast.MaxDamage), "combat target card exposes the shared attack forecast");
            SetPrivateField(game, "selectedAction", ActionMode.Attack);
            Assert(InvokePrivate<string>(game, "HoverClickInstruction", active, forecastTarget, null, forecastTarget.X, forecastTarget.Y) == "Click to attack", "legal attack hover invites the executable action");
            string legalAttackTitle = InvokePrivate<string>(game, "CombatHudTargetContextTitle", active, forecastTarget, true);
            Assert(legalAttackTitle == InvokePrivate<string>(game, "AttackModeLabel", active) + " Target", "legal weapon hover names the selected attack interaction");
            int legalTargetX = forecastTarget.X;
            int legalTargetY = forecastTarget.Y;
            forecastTarget.X = 11;
            forecastTarget.Y = 7;
            Assert(InvokePrivate<string>(game, "CombatHudTargetContextTitle", active, forecastTarget, true) == "Blocked Target", "illegal weapon hover names its blocked interaction");
            string blockedAttackInstruction = InvokePrivate<string>(game, "HoverClickInstruction", active, forecastTarget, null, forecastTarget.X, forecastTarget.Y);
            Assert(!blockedAttackInstruction.StartsWith("Click", StringComparison.OrdinalIgnoreCase), "illegal attack hover never contradicts its blocked target state");
            forecastTarget.X = legalTargetX;
            forecastTarget.Y = legalTargetY;

            SetPrivateField(game, "suppressBoardPointerThroughFrame", Time.frameCount - 1);
            InvokePrivate(game, "RunCombatHudCommand", ActionMode.Move);
            Assert(GetPrivateField<ActionMode>(game, "selectedAction") == ActionMode.Move, "production combat command path selects Move");

            CombatController combatController = InvokePrivate<CombatController>(game, "CombatLifecycle");
            combatState.Combat.Obstacles.Clear();
            int sanctuaryHp = active.Hp;
            active.Hp = Math.Max(1, active.MaxHp - 5);
            active.Shielded = 0;
            combatState.Combat.Obstacles.Add(new Point(active.X, active.Y, "sanctuary", 2));
            InvokePrivate<bool>(game, "ApplyStartTurnEffects", active);
            Assert(active.Shielded == 1, "Sanctuary grants a ward that survives the current start-turn tick");
            active.Hp = sanctuaryHp;
            active.Shielded = 0;
            combatState.Combat.Obstacles.Clear();

            active.Hp = active.MaxHp;
            active.Poisoned = 1;
            active.Bleeding = 0;
            active.Stunned = 0;
            active.Sleeping = 2;
            active.Webbed = 0;
            active.Regenerating = 0;
            combatController.BeginTurn(active, false);
            bool poisonWakeSkipped = InvokePrivate<bool>(game, "ApplyStartTurnEffects", active);
            Assert(!poisonWakeSkipped
                && active.Sleeping == 0
                && combatState.Combat.ActionAvailable,
                "start-turn poison wakes a sleeping unit and restores its action instead of consuming the turn");

            active.Hp = active.MaxHp;
            active.Poisoned = 0;
            active.Stunned = 0;
            active.Sleeping = 0;
            active.Webbed = 0;
            combatState.Combat.Obstacles.Clear();
            combatState.Combat.Obstacles.Add(new Point(active.X, active.Y, "ice", 2));
            System.Random originalCombatRng = GetPrivateField<System.Random>(game, "rng");
            SetPrivateField(game, "rng", new System.Random(1));
            combatController.BeginTurn(active, false);
            bool iceStunSkipped = InvokePrivate<bool>(game, "ApplyStartTurnEffects", active);
            SetPrivateField(game, "rng", originalCombatRng);
            Assert(iceStunSkipped
                && active.Stunned == 0
                && !combatState.Combat.ActionAvailable,
                "a fresh start-turn ice stun consumes the current turn before its one-turn counter expires");

            active.Hp = active.MaxHp;
            active.Stunned = 0;
            active.Webbed = 0;
            combatState.Combat.Obstacles.Clear();
            combatState.Combat.Obstacles.Add(new Point(active.X, active.Y, "web", 2));
            combatController.BeginTurn(active, false);
            bool webSkipped = InvokePrivate<bool>(game, "ApplyStartTurnEffects", active);
            Assert(!webSkipped
                && active.Webbed == 2
                && combatState.Combat.MovePoints == 0,
                "a fresh start-turn web removes movement immediately without consuming the action");

            active.Hp = active.MaxHp;
            active.Webbed = 2;
            combatState.Combat.Obstacles.Clear();
            combatState.Combat.Obstacles.Add(new Point(active.X, active.Y, "fire", 2));
            combatController.BeginTurn(active, false);
            bool fireFreedSkipped = InvokePrivate<bool>(game, "ApplyStartTurnEffects", active);
            Assert(!fireFreedSkipped
                && active.Webbed == 0
                && combatState.Combat.MovePoints == InvokePrivate<int>(game, "UnitMoveAllowance", active),
                "start-turn fire clears web and restores unspent movement in the same turn");
            int webRecoveryOriginX = active.X;
            int webRecoveryOriginY = active.Y;
            int webRecoveryMovePoints = combatState.Combat.MovePoints;
            Vector2Int[] webRecoveryCandidates =
            {
                new Vector2Int(webRecoveryOriginX + 1, webRecoveryOriginY),
                new Vector2Int(webRecoveryOriginX, webRecoveryOriginY + 1),
                new Vector2Int(webRecoveryOriginX - 1, webRecoveryOriginY),
                new Vector2Int(webRecoveryOriginX, webRecoveryOriginY - 1)
            };
            Vector2Int? webRecoveryDestination = webRecoveryCandidates
                .Where(candidate => InvokePrivate<bool>(game, "CanStandAt", candidate.x, candidate.y))
                .Select(candidate => (Vector2Int?)candidate)
                .FirstOrDefault();
            Assert(webRecoveryDestination.HasValue, "fire-cleared web smoke has an adjacent movement destination");
            InvokePrivate(game, "MoveActiveTo", active, webRecoveryDestination.Value.x, webRecoveryDestination.Value.y);
            Assert(active.X == webRecoveryDestination.Value.x
                && active.Y == webRecoveryDestination.Value.y
                && combatState.Combat.MovePoints < webRecoveryMovePoints,
                "fire-freed unit spends its repaired movement normally");
            Assert(InvokePrivate<bool>(game, "UndoActiveMovement"), "fire-freed movement can be undone");
            Assert(active.X == webRecoveryOriginX
                && active.Y == webRecoveryOriginY
                && combatState.Combat.MovePoints == webRecoveryMovePoints
                && combatState.Combat.ActionAvailable,
                "fire-freed Undo Move restores the turn-start tile and refreshed full budget");
            InvokePrivate(game, "MoveActiveTo", active, webRecoveryDestination.Value.x, webRecoveryDestination.Value.y);
            Assert(active.X == webRecoveryDestination.Value.x && active.Y == webRecoveryDestination.Value.y,
                "fire-freed unit can move again after Undo Move");
            Assert(InvokePrivate<bool>(game, "UndoActiveMovement"), "repeated fire-freed movement remains reversible");
            SetPrivateField(game, "suppressBoardPointerThroughFrame", Time.frameCount - 1);

            active.Hp = sanctuaryHp;
            active.Poisoned = 0;
            active.Bleeding = 0;
            active.Stunned = 0;
            active.Sleeping = 0;
            active.Webbed = 0;
            active.Regenerating = 0;
            active.Shielded = 0;
            combatState.Combat.Obstacles.Clear();
            combatController.BeginTurn(active, false);
            int moveOriginX = active.X;
            int moveOriginY = active.Y;
            int moveOriginPoints = combatState.Combat.MovePoints;
            Vector2Int[] undoCandidates =
            {
                new Vector2Int(moveOriginX + 1, moveOriginY),
                new Vector2Int(moveOriginX, moveOriginY + 1),
                new Vector2Int(moveOriginX - 1, moveOriginY),
                new Vector2Int(moveOriginX, moveOriginY - 1)
            };
            Vector2Int? undoDestination = undoCandidates
                .Where(candidate => InvokePrivate<bool>(game, "CanStandAt", candidate.x, candidate.y))
                .Select(candidate => (Vector2Int?)candidate)
                .FirstOrDefault();
            Assert(undoDestination.HasValue, "combat smoke has an adjacent undo destination");
            int previewMoveCost = InvokePrivate<int>(game, "MoveCostTo", active, undoDestination.Value.x, undoDestination.Value.y);
            IReadOnlyList<Vector2Int> previewMovePath = InvokePrivate<IReadOnlyList<Vector2Int>>(
                game,
                "ReachableMovePath",
                active,
                undoDestination.Value.x,
                undoDestination.Value.y,
                moveOriginPoints);
            Assert(previewMovePath != null
                && previewMovePath.Count == 2
                && previewMovePath[0] == new Vector2Int(moveOriginX, moveOriginY)
                && previewMovePath[previewMovePath.Count - 1] == undoDestination.Value, "runtime movement preview starts at the actor and ends at the executable destination");
            Assert(active.X == moveOriginX
                && active.Y == moveOriginY
                && combatState.Combat.MovePoints == moveOriginPoints, "building a movement preview mutates no combat state");
            InvokePrivate(game, "MoveActiveTo", active, undoDestination.Value.x, undoDestination.Value.y);
            Assert(moveOriginPoints - combatState.Combat.MovePoints == previewMoveCost, "executed movement spends the exact previewed weighted cost");
            InvokePrivate(game, "LateUpdate");
            CombatHudView movedHudView = InvokePrivate<CombatHudView>(game, "BuildCombatHudView");
            Assert(movedHudView.CanUndoMove, "combat HUD offers Undo Move after uncommitted movement");
            Assert(movedHudView.MovePoints == combatState.Combat.MovePoints
                && movedHudView.MovePoints < movedHudView.MovePointsMaximum, "combat header updates immediately after movement spends points");
            Assert(hud.IsUndoMoveVisible, "migrated combat deck renders the contextual Undo Move control");
            hud.InvokeUndoMoveForTest();
            Assert(active.X == moveOriginX && active.Y == moveOriginY, "Undo Move restores the active unit's turn-start tile");
            Assert(combatState.Combat.MovePoints == moveOriginPoints, "Undo Move restores the full movement budget");
            Assert(combatState.Combat.ActionAvailable, "Undo Move preserves the active unit's action");
            InvokePrivate(game, "LateUpdate");
            Assert(!InvokePrivate<CombatHudView>(game, "BuildCombatHudView").CanUndoMove && !hud.IsUndoMoveVisible, "Undo Move hides again at the restored origin");

            InvokePrivate(game, "PromoteMageTester", active);
            CombatHudView focusedHudView = InvokePrivate<CombatHudView>(game, "BuildCombatHudView");
            Assert(focusedHudView.ActiveUnit != null && focusedHudView.ActiveUnit.StateLine.Contains("FOCUS"), "combat HUD exposes the unmoved caster focus benefit");
            Assert(focusedHudView.Commands[2].Label == "Spells" && focusedHudView.Commands[2].SubLabel == "Choose spell", "selected caster command clearly leads into spell choice");
            InvokePrivate(game, "LateUpdate");
            Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.AbilityPicker, "spell command opens ability picker overlay");
            AssertActiveObject("Combat Ability Modal Canvas");
            CombatAbilityModalScreen modal = UnityEngine.Object.FindFirstObjectByType<CombatAbilityModalScreen>();
            Assert(modal != null && modal.IsReady && modal.IsVisible, "combat spellbook uGUI is ready and visible");
            GameObject listViewportObject = GameObject.Find("List Viewport");
            Mask listViewportMask = listViewportObject == null ? null : listViewportObject.GetComponent<Mask>();
            Assert(listViewportMask != null
                && listViewportMask.graphic != null
                && listViewportMask.graphic.color.a > 0f
                && !listViewportMask.showMaskGraphic, "spellbook list viewport writes a hidden nontransparent stencil so card rows remain renderable");
            Assert(!Application.isPlaying
                || EventSystem.current != null && !EventSystem.current.sendNavigationEvents, "spellbook owns controller navigation without a competing automatic uGUI route");
            Assert(modal.HasRenderableGeometry, "combat spellbook canvas is a renderable root overlay");
            Assert(InvokePrivate<bool>(game, "HasRenderableGameplayOverlay", UiOverlay.AbilityPicker), "IMGUI yields the frame to the visible spellbook modal");
            Assert(!InvokePrivate<bool>(game, "NeedsEmergencyCombatAbilityModalFallback"), "healthy spellbook does not draw its recovery picker");
            Assert(hud.IsVisible && !hud.HasUsableCommandBar, "combat HUD remains dimmed and noninteractive beneath the spellbook");
            CombatAbilityModalView readySpellbookView = InvokePrivate<CombatAbilityModalView>(game, "BuildCombatAbilityModalView");
            CombatAbilityModalCardView readyFireball = readySpellbookView.Cards.First(card => card.Id == "FBL");
            Assert(readySpellbookView.StateIconTexture == powerBookStateIcons, "spellbook view carries the authored state microicon atlas");
            foreach (FormulaDef productionFormula in FormulaCatalog.All)
            {
                CombatAbilityModalCardView artProbe = new CombatAbilityModalCardView();
                InvokePrivate(game, "ApplyFormulaModalArt", artProbe, productionFormula);
                int signatureIndex = CombatIconCatalog.SignatureSpellIndex(productionFormula.Code);
                Rect expectedSource = InvokePrivate<Rect>(game, "SignatureSpellIconAtlasCell", signatureIndex);
                Assert(signatureIndex >= 0
                    && artProbe.IconTexture == signatureSpellIcons
                    && artProbe.IconSource.Equals(expectedSource),
                    productionFormula.Code + " resolves its unique signature spell-book cell");
            }
            SetPrivateField<Texture2D>(game, "signatureSpellIconAtlas", null);
            CombatAbilityModalCardView missingSignatureProbe = new CombatAbilityModalCardView();
            InvokePrivate(game, "ApplyFormulaModalArt", missingSignatureProbe, FormulaCatalog.All.First(formula => formula.Code == "FBL"));
            Assert(missingSignatureProbe.IconTexture == null,
                "a missing signature atlas falls back to the visible formula sigil instead of misleading legacy art");
            SetPrivateField(game, "signatureSpellIconAtlas", signatureSpellIcons);
            foreach (string code in new[] { "RIG", "CLT", "RSG", "AST", "VST" })
            {
                CombatAbilityModalCardView lightningCard = readySpellbookView.Cards.First(card => card.Id == code);
                Rect expectedSource = InvokePrivate<Rect>(game, "SignatureSpellIconAtlasCell", CombatIconCatalog.SignatureSpellIndex(code));
                Assert(lightningCard.IconTexture == signatureSpellIcons
                    && lightningCard.IconSource.Equals(expectedSource), code + " spellbook card resolves the authoritative signature atlas");
                Assert(!string.IsNullOrWhiteSpace(lightningCard.RowSummary)
                    && lightningCard.RowSummary.Count(character => character == '\n') == 0, code + " keeps its tactical role on one concise row");
                Assert(!string.IsNullOrWhiteSpace(lightningCard.CurrentEffect)
                    && !lightningCard.CurrentEffect.Contains("CASTING RULES")
                    && !lightningCard.CurrentEffect.Contains("FORMULA NOTE"), code + " selected detail leads with only the live resolved effect");
            }
            Assert(readySpellbookView.Title.EndsWith("Spellbook", StringComparison.Ordinal)
                && readySpellbookView.Actor.Contains(active.Name)
                && readySpellbookView.Actor.Contains("L" + active.Level)
                && readySpellbookView.Resource.Contains("MP")
                && readySpellbookView.ActionState == "ACTION READY", "spellbook exposes structured actor, level, resource, and action-state context");
            Assert(readyFireball.TargetCountKnown && readyFireball.ValidTargetCount > 0, "spellbook computes legal Fireball targets before arming");
            Assert(!string.IsNullOrWhiteSpace(readyFireball.RowSummary)
                && !string.IsNullOrWhiteSpace(readyFireball.CurrentEffect)
                && !readyFireball.CurrentEffect.Contains("CASTING RULES"), "spellbook presents concise row context and the immediately relevant live effect");
            Assert(!string.IsNullOrWhiteSpace(CombatAbilityModalPresentationRules.DetailMeta(readyFireball))
                && !string.IsNullOrWhiteSpace(CombatAbilityModalPresentationRules.DetailNotes(readyFireball)), "spellbook detail adds the live profile and nonduplicated casting guidance");
            Assert(modal.ActiveFilter == CombatAbilityModalPresentationRules.InitialFilter(readySpellbookView.Cards)
                && modal.VisibleCardCount == CombatAbilityModalPresentationRules.Count(readySpellbookView.Cards, modal.ActiveFilter), "spellbook opens on the deterministic useful filter");
            Assert(modal.FilterControlCountForTest == 4, "spellbook exposes Ready, Known, Locked, and All as four real controls");
            Assert(modal.VisibleStatusBadgeCountForTest == 0, "ordinary Ready Spellbook rows stay quiet instead of repeating global state");
            Assert(!modal.UsesGeneratedStateIconAtlasForTest
                && modal.VisibleStateIconCountForTest >= 1
                && modal.VisibleTargetingRailCountForTest == 0,
                "spellbook uses authored state microicons with one selector and no redundant right targeting rail");
            Assert(string.IsNullOrEmpty(modal.DetailContextForTest), "ordinary selected Spellbook detail does not spend space restating selection");
            Assert(modal.DetailTargetLabelForTest.Contains("legal")
                && modal.DetailTargetLabelForTest.Contains("enem"), "Spellbook target chip names the legal target type");
            Assert(modal.FooterHintForTest.Contains("Browse")
                && modal.FooterHintForTest.Contains("Use")
                && modal.FooterHintForTest.Contains("Filter")
                && modal.FooterHintForTest.Contains("Back"), "Spellbook footer keeps a compact complete control legend");

            int browseMana = active.Mana;
            int browseMovePoints = combatState.Combat.MovePoints;
            bool browseActionReady = combatState.Combat.ActionAvailable;
            modal.SetFilterForTest(CombatAbilityModalFilter.All);
            Assert(modal.ActiveFilter == CombatAbilityModalFilter.All
                && modal.VisibleCardCount == readySpellbookView.Cards.Count, "All is a represented view of the complete spellbook");
            foreach (CombatAbilityModalCardView productionFormula in readySpellbookView.Cards)
            {
                InvokePrivate(game, "SelectCombatAbilityModalCard", productionFormula.Id);
                InvokePrivate(game, "LateUpdate");
                Assert(modal.DetailNarrativeFullyPresentedForTest, productionFormula.Name + " detail text fits its regions or remains fully scrollable");
                Assert(!Application.isPlaying || modal.SelectedRowFocusedForTest, productionFormula.Name + " keeps semantic controller focus on its selected row");
            }
            string scrollSelection = modal.SelectedId;
            modal.SetDetailNarrativeForTest(
                string.Join(" ", Enumerable.Repeat(
                    "Guaranteed long-detail regression text keeps controller scrolling deterministic without requiring production copy to stay verbose.",
                    48)),
                "Return to battle only after reviewing the full tactical note.");
            Assert(modal.DetailNarrativeCanScrollForTest, "synthetic long Spellbook detail deterministically exercises the overflow path");
            float scrollTop = modal.DetailNarrativeNormalizedPositionForTest;
            Assert(modal.ScrollDetailPageForTest(-1)
                && modal.DetailNarrativeNormalizedPositionForTest < scrollTop
                && modal.SelectedId == scrollSelection, "Page Down scrolls long Spellbook detail without moving row selection");
            Assert(modal.ScrollDetailPageForTest(1)
                && Mathf.Approximately(modal.DetailNarrativeNormalizedPositionForTest, 1f), "Page Up returns long Spellbook detail to its clamped top");
            Assert(modal.FooterHintForTest.Contains("Details"), "overflowing Spellbook detail advertises its extra scroll controls");
            modal.Refresh();
            modal.MoveSelectionForTest(1000);
            string allBottomSelection = modal.SelectedId;
            float allBottomScroll = modal.ScrollYForTest;
            modal.SetFilterForTest(CombatAbilityModalFilter.All);
            Assert(modal.SelectedId == allBottomSelection
                && Mathf.Approximately(modal.ScrollYForTest, allBottomScroll), "reselecting All is a true no-op for cursor and scroll");
            modal.SetFilterForTest(CombatAbilityModalFilter.Learned);
            modal.SetFilterForTest(CombatAbilityModalFilter.All);
            Assert(modal.SelectedId == allBottomSelection
                && Mathf.Approximately(modal.ScrollYForTest, allBottomScroll), "round-tripping a book view restores its own cursor and scroll");
            InvokePrivate(game, "SelectCombatAbilityModalCard", "FBL");
            Assert(modal.SelectedBookStateForTest == CombatAbilityModalBookState.ReadyNow
                && modal.SelectedBookStateIconIndexForTest == CombatIconCatalog.BookStateSelectionIndex, "committed ready spell resolves the typed selection state and microicon");
            float hoverScrollY = modal.ScrollYForTest;
            string hoverPendingFormula = GetPrivateField<string>(game, "pendingFormulaCode");
            int arcaneTempestIndex = readySpellbookView.Cards.ToList().FindIndex(card => card.Id == "AST");
            Assert(arcaneTempestIndex >= 0, "spellbook hover regression probe resolves Arcane Tempest in the complete book");
            modal.HoverVisibleIndexForTest(arcaneTempestIndex);
            Assert(modal.PreviewedIdForTest == "AST"
                && modal.DetailIdForTest == "AST"
                && modal.SelectedId == "FBL"
                && modal.SelectedRailCountForTest == 1
                && modal.VisibleTargetingRailCountForTest == 0
                && modal.DetailBookStateForTest == CombatAbilityModalBookState.ReadyNow
                && !modal.DetailActionInteractableForTest
                && modal.DetailActionLabelForTest == "Preview Only"
                && modal.DetailPromptForTest.Contains("Click or focus the card"), "spell hover uses one quiet passive preview while preserving one committed selection");
            Assert(GetPrivateField<string>(game, "spellbookSelectedCode") == "FBL"
                && GetPrivateField<string>(game, "pendingFormulaCode") == hoverPendingFormula
                && Mathf.Approximately(modal.ScrollYForTest, hoverScrollY), "spell hover neither persists browse memory, arms a formula, nor moves the list");
            modal.ClearHoverForTest();
            Assert(string.IsNullOrEmpty(modal.PreviewedIdForTest)
                && modal.DetailIdForTest == "FBL"
                && modal.SelectedId == "FBL", "leaving a spell row restores committed detail");
            modal.HoverVisibleIndexForTest(arcaneTempestIndex);
            modal.InvokeDetailActionForTest();
            Assert(modal.IsVisible
                && modal.SelectedId == "FBL"
                && modal.PreviewedIdForTest == "AST"
                && GetPrivateField<string>(game, "pendingFormulaCode") == hoverPendingFormula
                && modal.IsSelectedVisibleForTest(), "passive preview detail cannot become a second activation path");
            modal.SelectVisibleIndexForTest(arcaneTempestIndex);
            Assert(modal.IsVisible
                && modal.SelectedId == "AST"
                && string.IsNullOrEmpty(modal.PreviewedIdForTest)
                && GetPrivateField<string>(game, "pendingFormulaCode") == hoverPendingFormula
                && modal.IsSelectedVisibleForTest()
                && (!Application.isPlaying || modal.SelectedRowFocusedForTest), "clicking the previewed card commits it, clears preview, and transfers semantic focus without arming");
            string previousBrowseSelection = modal.SelectedId;
            modal.MoveSelectionForTest(-1);
            Assert(modal.SelectedId != previousBrowseSelection
                && string.IsNullOrEmpty(modal.PreviewedIdForTest)
                && modal.IsSelectedVisibleForTest()
                && (!Application.isPlaying || modal.SelectedRowFocusedForTest), "keyboard-style spellbook navigation clears stale hover, changes selection, and maintains one focused row");
            Assert(active.Mana == browseMana
                && combatState.Combat.MovePoints == browseMovePoints
                && combatState.Combat.ActionAvailable == browseActionReady, "browsing and selecting spells spends no combat resources");

            InvokePrivate(game, "SelectCombatAbilityModalCard", "FBL");
            Assert(modal.SelectedId == "FBL", "spellbook selection callback updates the primary detail action");
            modal.InvokeSelectedForTest();
            Assert(!modal.IsVisible
                && GetPrivateField<string>(game, "pendingFormulaCode") == "FBL"
                && GetPrivateField<ActionMode>(game, "selectedAction") == ActionMode.Cast
                && combatState.Combat.Phase == CombatPhase.ChooseTarget, "spellbook primary action closes the book and arms Fireball targeting");
            Assert(active.Mana == browseMana
                && combatState.Combat.MovePoints == browseMovePoints
                && combatState.Combat.ActionAvailable == browseActionReady, "arming a spell through the book preserves mana, movement, and the action");

            InvokePrivate(game, "SelectOrRunAction", ActionMode.Cast, active);
            InvokePrivate(game, "LateUpdate");
            CombatAbilityModalView armedSpellbookView = InvokePrivate<CombatAbilityModalView>(game, "BuildCombatAbilityModalView");
            CombatAbilityModalCardView armedFireball = armedSpellbookView.Cards.First(card => card.Id == "FBL");
            Assert(modal.IsVisible
                && armedFireball.Ready
                && armedFireball.Selected
                && modal.SelectedBookStateForTest == CombatAbilityModalBookState.Targeting
                && modal.SelectedBookStateIconIndexForTest == CombatIconCatalog.BookStateTargetingIndex
                && modal.SelectedRailCountForTest == 1
                && modal.TargetingBadgeCountForTest == 1
                && modal.VisibleStatusBadgeCountForTest >= modal.TargetingBadgeCountForTest
                && modal.VisibleStateIconCountForTest >= 2
                && modal.VisibleTargetingRailCountForTest == 1
                && !modal.DetailStatusVisibleForTest
                && modal.DetailContextForTest == "TARGETING ARMED"
                && modal.DetailActionLabelForTest == "Resume Targeting", "reopening an armed spell shows one distinct targeting state and one resume action");
            InvokePrivate(game, "SelectCombatAbilityModalCard", "VST");
            armedSpellbookView = InvokePrivate<CombatAbilityModalView>(game, "BuildCombatAbilityModalView");
            armedFireball = armedSpellbookView.Cards.First(card => card.Id == "FBL");
            CombatAbilityModalCardView browsedThunderStep = armedSpellbookView.Cards.First(card => card.Id == "VST");
            Assert(armedFireball.Ready
                && !armedFireball.Selected
                && browsedThunderStep.Selected
                && modal.SelectedRailCountForTest == 1
                && modal.TargetingBadgeCountForTest == 1
                && modal.VisibleStatusBadgeCountForTest >= modal.TargetingBadgeCountForTest
                && modal.VisibleTargetingRailCountForTest == 1, "browsing another spell keeps one committed-selection rail beside one distinct armed targeting rail");
            InvokePrivate(game, "CloseCombatAbilityModal");
            Assert(InvokePrivate<bool>(game, "CancelCombatTargeting"), "armed spell targeting remains explicitly cancelable after closing the book");
            InvokePrivate(game, "SelectOrRunAction", ActionMode.Cast, active);
            InvokePrivate(game, "LateUpdate");
            Assert(InvokePrivate<CombatAbilityModalView>(game, "BuildCombatAbilityModalView").SelectedId == "VST", "canceling targeting preserves the per-caster browse selection");
            CombatUnit alternateBrowser = combatState.Combat.Units.First(unit => unit != null
                && unit.Side == UnitSide.Party
                && unit.Id != active.Id);
            InvokePrivate(game, "RememberCombatAbilityBrowseSelection", active, true, "VST");
            InvokePrivate(game, "RememberCombatAbilityBrowseSelection", alternateBrowser, true, "FBL");
            InvokePrivate(game, "RememberCombatAbilityBrowseSelection", active, false, "aimedshot");
            InvokePrivate(game, "RememberCombatAbilityBrowseSelection", alternateBrowser, false, "pinningshot");
            Dictionary<string, string> browseSelections = GetPrivateField<Dictionary<string, string>>(game, "combatAbilityBrowseSelections");
            string activeSpellBrowseKey = InvokePrivate<string>(game, "CombatAbilityBrowseSelectionKey", active, true);
            string alternateSpellBrowseKey = InvokePrivate<string>(game, "CombatAbilityBrowseSelectionKey", alternateBrowser, true);
            string activeSkillBrowseKey = InvokePrivate<string>(game, "CombatAbilityBrowseSelectionKey", active, false);
            string alternateSkillBrowseKey = InvokePrivate<string>(game, "CombatAbilityBrowseSelectionKey", alternateBrowser, false);
            Assert(browseSelections[activeSpellBrowseKey] == "VST"
                && browseSelections[alternateSpellBrowseKey] == "FBL"
                && browseSelections[activeSkillBrowseKey] == "aimedshot"
                && browseSelections[alternateSkillBrowseKey] == "pinningshot", "browse memory remains independent across combatants and across both book types");
            int promotedLevel = active.Level;
            active.Level = 1;
            CombatAbilityModalView progressionView = InvokePrivate<CombatAbilityModalView>(game, "BuildCombatAbilityModalView");
            CombatAbilityModalCardView futureThunderStep = progressionView.Cards.First(card => card.Id == "VST");
            CombatAbilityModalCardView futureTempest = progressionView.Cards.First(card => card.Id == "AST");
            Assert(futureThunderStep.Locked && futureThunderStep.UnlockLevel == 4, "spellbook shows Thunder Step as a visible future unlock");
            Assert(futureTempest.Locked && futureTempest.UnlockLevel == 6 && futureTempest.Epic, "spellbook shows the elder Arcane Tempest progression card");
            Assert(CombatAbilityModalPresentationRules.RowMeta(futureTempest).StartsWith("Unlocks L6", StringComparison.Ordinal)
                && CombatAbilityModalPresentationRules.RowMeta(futureTempest).Contains(futureTempest.Cost)
                && CombatAbilityModalPresentationRules.RowMeta(futureTempest).Contains(futureTempest.Range), "locked production Spellbook rows expose unlock level, cost, and reach without opening detail");
            modal.Refresh();
            modal.SetFilterForTest(CombatAbilityModalFilter.Future);
            Assert(modal.ActiveFilter == CombatAbilityModalFilter.Future
                && modal.VisibleCardCount == CombatAbilityModalPresentationRules.Count(progressionView.Cards, CombatAbilityModalFilter.Future)
                && modal.VisibleStatusBadgeCountForTest == 0, "Locked filter renders the real locked cards without repeating a badge on every row");
            modal.SetFilterForTest(CombatAbilityModalFilter.Learned);
            Assert(modal.ActiveFilter == CombatAbilityModalFilter.Learned
                && modal.VisibleCardCount == CombatAbilityModalPresentationRules.Count(progressionView.Cards, CombatAbilityModalFilter.Learned), "Learned filter renders exactly the real unlocked formulas");
            active.Level = promotedLevel;
            modal.Refresh();
            modal.SetFilterForTest(CombatAbilityModalFilter.Ready);
            Assert(modal.ActiveFilter == CombatAbilityModalFilter.Ready
                && modal.VisibleCardCount == CombatAbilityModalPresentationRules.Count(
                    InvokePrivate<CombatAbilityModalView>(game, "BuildCombatAbilityModalView").Cards,
                    CombatAbilityModalFilter.Ready), "Ready filter returns to the currently actionable formulas");

            InvokePrivate(game, "CloseCombatAbilityModal");
            Assert(InvokePrivate<bool>(game, "IsBoardPointerSuppressed"), "closing spellbook suppresses combat-board click-through");
            Assert(!Application.isPlaying
                || EventSystem.current != null && EventSystem.current.sendNavigationEvents, "closing spellbook restores ordinary uGUI navigation");
            InvokePrivate(game, "LateUpdate");
            Assert(hud.HasUsableCommandBar, "playable migrated action bar is restored after closing spellbook");
            Assert(!InvokePrivate<bool>(game, "NeedsEmergencyCombatHudFallback"), "recovery action bar stays hidden after the migrated HUD recovers");

            combatState.Combat.ActionAvailable = false;
            InvokePrivate(game, "LateUpdate");
            CombatHudCommandView reviewSpellbookCommand = InvokePrivate<CombatHudView>(game, "BuildCombatHudView")
                .Commands.First(command => command.Mode == ActionMode.Cast);
            Assert(reviewSpellbookCommand.Enabled && reviewSpellbookCommand.SubLabel == "Review book", "spent-action caster can still inspect the Spellbook from the command deck");
            SetPrivateField(game, "suppressBoardPointerThroughFrame", Time.frameCount - 1);
            hud.InvokeCommandForTest(ActionMode.Cast);
            InvokePrivate(game, "LateUpdate");
            CombatAbilityModalView spentActionBook = InvokePrivate<CombatAbilityModalView>(game, "BuildCombatAbilityModalView");
            CombatAbilityModalCardView spentFireball = spentActionBook.Cards.First(card => card.Id == "FBL");
            Assert(modal.IsVisible
                && modal.ActiveFilter == CombatAbilityModalFilter.Learned
                && modal.VisibleCardCount > 0
                && modal.VisibleStatusBadgeCountForTest == 0
                && spentActionBook.ActionState == "ACTION USED"
                && !spentFireball.Usable
                && spentFireball.DisabledReason == "Action already used.", "review-only Spellbook explains the spent action once without repeating badges across Known");
            active.Stunned = 2;
            CombatAbilityModalView stunnedBook = InvokePrivate<CombatAbilityModalView>(game, "BuildCombatAbilityModalView");
            Assert(stunnedBook.Cards.First(card => card.Id == "FBL").DisabledReason == "Stunned for 2 more turns.", "Spellbook reports incapacitation before generic action usage");
            active.Stunned = 0;
            InvokePrivate(game, "SelectCombatAbilityModalCard", "FBL");
            InvokePrivate(game, "ActivateCombatAbilityModalCard", "FBL");
            Assert(modal.IsVisible
                && string.IsNullOrEmpty(GetPrivateField<string>(game, "pendingFormulaCode"))
                && !combatState.Combat.ActionAvailable, "review-only card activation cannot arm a power or mutate the turn");
            InvokePrivate(game, "CloseCombatAbilityModal");
            combatState.Combat.ActionAvailable = true;
            combatState.Combat.Phase = CombatPhase.ChooseAction;

            FormulaDef focusFireball = FormulaCatalog.All.First(formula => formula.Code == "FBL");
            int focusedFireballCost = InvokePrivate<int>(game, "EffectiveFormulaMana", focusFireball, active);
            Assert(focusedFireballCost < focusFireball.Mana, "unmoved Mage test caster receives the Focus mana discount");
            active.Mana = focusedFireballCost;
            Assert(InvokePrivate<bool>(game, "PrepareFormulaCode", active, "FBL"), "focused caster can arm Fireball at its discounted cost");
            Assert(InvokePrivate<bool>(
                game,
                "TryCombatStep",
                active,
                undoDestination.Value.x - active.X,
                undoDestination.Value.y - active.Y), "focused armed spell accepts the deterministic movement step");
            Assert(string.IsNullOrEmpty(GetPrivateField<string>(game, "pendingFormulaCode"))
                && GetPrivateField<ActionMode>(game, "selectedAction") == ActionMode.Move
                && active.Mana == focusedFireballCost
                && combatState.Combat.ActionAvailable, "moving disarms a spell that lost its Focus affordability without spending mana or action");
            Assert(InvokePrivate<bool>(game, "UndoActiveMovement"), "focus-affordability movement can be undone");
            Assert(active.X == moveOriginX && active.Y == moveOriginY && combatState.Combat.MovePoints == moveOriginPoints, "focus-affordability undo restores the original stance");

            active.Mana = active.MaxMana;
            Assert(InvokePrivate<bool>(game, "PrepareFormulaCode", active, "FBL"), "production spell path arms Fireball targeting");
            Assert(InvokePrivate<bool>(
                game,
                "TryCombatStep",
                active,
                undoDestination.Value.x - active.X,
                undoDestination.Value.y - active.Y), "affordable armed spell accepts a deterministic movement step");
            Assert(GetPrivateField<string>(game, "pendingFormulaCode") == "FBL"
                && GetPrivateField<ActionMode>(game, "selectedAction") == ActionMode.Cast
                && combatState.Combat.Phase == CombatPhase.ChooseTarget, "movement preserves an affordable armed spell and target phase");
            InvokePrivate(game, "LateUpdate");
            CombatHudView targetingHudView = InvokePrivate<CombatHudView>(game, "BuildCombatHudView");
            Assert(targetingHudView.CanCancelTarget && targetingHudView.CancelTargetLabel == "Cancel Spell", "armed formula publishes explicit target cancellation");
            Assert(targetingHudView.Commands[2].Label == "Spells"
                && targetingHudView.Commands[2].SubLabel.Contains("target")
                && !targetingHudView.Commands[2].SubLabel.StartsWith("Choose", StringComparison.Ordinal), "armed spell command publishes its current legal-target count");
            string spellTargetState = InvokePrivate<string>(game, "CombatHudUnitStateLine", forecastTarget, false);
            Assert(spellTargetState.Contains("Fireball")
                && spellTargetState.Contains("fire")
                && spellTargetState.Contains("-"), "armed spell target card carries the resolved Fireball outcome so the map can stay unobscured");
            CombatUnit legalSpellTarget = InvokePrivate<CombatUnit>(game, "SuggestedArmedPowerTarget", active);
            Assert(legalSpellTarget != null && InvokePrivate<string>(game, "CombatHudTargetContextTitle", active, legalSpellTarget, true) == "Spell Target", "legal armed spell hover is labeled honestly");
            Assert(targetingHudView.CanUndoMove, "moving after arming a spell keeps movement undo available");
            Assert(hud.IsCancelTargetVisible && hud.IsUndoMoveVisible, "combat deck fits both contextual recovery controls");
            int armedX = active.X;
            int armedY = active.Y;
            int armedMovePoints = combatState.Combat.MovePoints;
            SetPrivateField(game, "suppressBoardPointerThroughFrame", Time.frameCount - 1);
            hud.InvokeCancelTargetForTest();
            Assert(string.IsNullOrEmpty(GetPrivateField<string>(game, "pendingFormulaCode")), "target cancellation clears the armed formula");
            Assert(GetPrivateField<ActionMode>(game, "selectedAction") == ActionMode.Attack, "target cancellation returns to the ordinary attack mode");
            Assert(combatState.Combat.Phase == CombatPhase.ChooseAction && combatState.Combat.ActionAvailable, "target cancellation preserves the action phase");
            Assert(active.X == armedX && active.Y == armedY && combatState.Combat.MovePoints == armedMovePoints, "target cancellation preserves movement state");
            InvokePrivate(game, "LateUpdate");
            Assert(!hud.IsCancelTargetVisible && hud.IsUndoMoveVisible, "cancel control yields while movement undo remains available");
            SetPrivateField(game, "suppressBoardPointerThroughFrame", Time.frameCount - 1);
            hud.InvokeUndoMoveForTest();
            Assert(active.X == moveOriginX && active.Y == moveOriginY && combatState.Combat.MovePoints == moveOriginPoints, "movement can still be undone after canceling spell targeting");

            FormulaDef fireball = FormulaCatalog.All.First(formula => formula.Code == "FBL");
            CombatUnit spellTarget = combatState.Combat.Units.First(unit => unit != null && unit.Side == UnitSide.Enemy && unit.Hp > 0);
            combatState.Combat.Units.RemoveAll(unit => unit == null || unit.Id != active.Id && unit.Id != spellTarget.Id);
            combatState.Combat.Obstacles.Clear();
            active.X = 1;
            active.Y = 1;
            spellTarget.X = 4;
            spellTarget.Y = 1;
            spellTarget.MaxHp = Math.Max(spellTarget.MaxHp, 100);
            spellTarget.Hp = spellTarget.MaxHp;
            combatState.Combat.Obstacles.Add(new Point(spellTarget.X, spellTarget.Y, "gas", 3));
            Assert(InvokePrivate<bool>(game, "HasFormulaLineOfSight", fireball, active, spellTarget.X, spellTarget.Y), "staged-power smoke lane has Fireball line of sight");
            active.Mana = active.MaxMana;
            List<FloatText> stagedFloats = GetPrivateField<List<FloatText>>(game, "floatTexts");
            stagedFloats.Clear();
            float scheduledFloatProbeStarted = Time.time;
            SetPrivateField(game, "combatVfxImpactDelay", 0.42f);
            InvokePrivate(game, "AddFloat", spellTarget.X, spellTarget.Y, "damage", Color.white, null);
            InvokePrivate(game, "AddFloat", spellTarget.X, spellTarget.Y, "burn", Color.white, null);
            Assert(stagedFloats.Count == 2
                && stagedFloats.Select(value => value.Lane).Distinct().Count() == 2
                && stagedFloats.All(value => value.Start >= scheduledFloatProbeStarted + 0.35f), "future-scheduled outcome text reserves distinct readable lanes");
            SetPrivateField(game, "combatVfxImpactDelay", 0f);
            stagedFloats.Clear();
            List<BeamEffect> stagedBeams = GetPrivateField<List<BeamEffect>>(game, "beams");
            stagedBeams.Clear();
            List<PowerImpactEcho> impactEchoes = GetPrivateField<List<PowerImpactEcho>>(game, "powerImpactEchoes");
            impactEchoes.Clear();
            List<PowerCastAura> castAuras = GetPrivateField<List<PowerCastAura>>(game, "powerCastAuras");
            castAuras.Clear();
            List<CombatUnitPresentationBeat> unitPresentationBeats =
                GetPrivateField<List<CombatUnitPresentationBeat>>(game, "combatUnitPresentationBeats");
            unitPresentationBeats.Clear();
            List<CastGlyph> stagedGlyphs = GetPrivateField<List<CastGlyph>>(game, "castGlyphs");
            stagedGlyphs.Clear();
            List<CellFlash> stagedFlashes = GetPrivateField<List<CellFlash>>(game, "flashes");
            stagedFlashes.Clear();
            List<ParticleDot> stagedParticles = GetPrivateField<List<ParticleDot>>(game, "particles");
            stagedParticles.Clear();
            List<AudioSource> sfxVoices = GetPrivateField<List<AudioSource>>(game, "sfxVoices");
            Assert(
                sfxVoices.Count == CombatAudioMixRules.SfxVoiceCount
                && sfxVoices.Where(voice => voice != null).Distinct().Count() == CombatAudioMixRules.SfxVoiceCount,
                "combat audio owns eight reusable SFX voices");
            Dictionary<string, AudioClip> soundClips = GetPrivateField<Dictionary<string, AudioClip>>(game, "soundClips");
            string[] newAudioCues =
            {
                "castmend", "castember", "casthex", "castpact",
                "fieldfire", "fieldice", "fieldgas", "fieldsnare", "fieldholy", "fieldcurse",
                "footstone", "footearth", "footwood", "footwater", "dialogue", "door",
                "dialogueopen", "dialoguepage", "dialogueclose", "gateopen", "gatebarred",
                "servicecoin", "servicearmor", "serviceweapon", "serviceenchant",
                "swing", "swingheavy", "thrust", "arrowrelease", "bladecontact", "thrustcontact", "heavycontact", "arrowcontact", "woodcontact", "stonecontact", "spellrelease", "wayfind"
            };
            Assert(newAudioCues.All(key => soundClips.ContainsKey(key) && soundClips[key] != null), "expanded spell, field, exploration, and dialogue audio clips build at runtime");
            Assert(GetPrivateField<AudioSource>(game, "musicFadeSource") != null, "music owns a second source for zone and combat crossfades");
            SetPrivateField(game, "combatMusicDuckStartedAt", -1f);
            SetPrivateField(game, "combatMusicDuckFullDepthAt", -1f);
            SetPrivateField(game, "combatMusicDuckHoldUntil", -1f);
            SetPrivateField(game, "combatMusicDuckUntil", -1f);
            SetPrivateField(game, "combatMusicDuckDepth", 0f);
            object scheduledSfx = GetPrivateField<object>(game, "scheduledSfx");
            scheduledSfx.GetType().GetMethod("Clear").Invoke(scheduledSfx, null);
            Point feedbackTree = new Point(2, 1, "tree", 3);
            InvokePrivate(game, "StageCoverImpactFeedback", active, feedbackTree, new Color(0.42f, 0.66f, 0.30f, 1f), true, false, false);
            Assert(stagedBeams.Any(value => value.Kind == "weapon-splinter"), "broken tree cover stages a dedicated splinter motif");
            InvokePrivate(game, "PlayCoverAttackSequence", active, feedbackTree, false, true, false);
            int coverCueCount = (int)scheduledSfx.GetType().GetProperty("Count").GetValue(scheduledSfx);
            bool hasWoodContact = false;
            bool hasBreakTail = false;
            for (int cueIndex = 0; cueIndex < coverCueCount; cueIndex++)
            {
                object cue = scheduledSfx.GetType().GetProperty("Item").GetValue(scheduledSfx, new object[] { cueIndex });
                string cueKey = (string)cue.GetType().GetField("Key", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(cue);
                if (cueKey == "woodcontact") hasWoodContact = true;
                if (cueKey == "breakcover") hasBreakTail = true;
            }
            Assert(hasWoodContact && hasBreakTail, "broken tree cover layers material contact and a delayed break tail");
            Assert(InvokePrivate<string>(game, "CombatHoverTileLine", feedbackTree.X, feedbackTree.Y, feedbackTree).Contains("2/2 integrity"), "cover hover reports current and maximum integrity");
            Assert(InvokePrivate<string>(game, "TerrainPreviewLine", new Point(3, 1, "fire", 2)).Contains("2 rounds left"), "field hover reports its round lifetime");

            stagedBeams.Clear();
            scheduledSfx.GetType().GetMethod("Clear").Invoke(scheduledSfx, null);
            Point feedbackStone = new Point(3, 1, "stone", 0);
            InvokePrivate(game, "StageCoverImpactFeedback", active, feedbackStone, new Color(0.55f, 0.57f, 0.55f, 1f), true, false, false);
            Assert(stagedBeams.Any(value => value.Kind == "weapon-rubble"), "broken stone cover stages a dedicated rubble motif");

            stagedFloats.Clear();
            stagedBeams.Clear();
            impactEchoes.Clear();
            castAuras.Clear();
            stagedGlyphs.Clear();
            stagedFlashes.Clear();
            stagedParticles.Clear();
            scheduledSfx.GetType().GetMethod("Clear").Invoke(scheduledSfx, null);
            CombatPowerOutcomeSnapshot outcomeBefore = CombatPowerOutcomeRules.Capture(combatState.Combat);
            float castStarted = Time.time;
            Assert(InvokePrivate<bool>(game, "CastFormula", active, "FBL", spellTarget, spellTarget.X, spellTarget.Y), "Beta Lab Fireball resolves through production casting path");
            InvokePrivate(game, "SetCombatPowerOutcome", outcomeBefore);
            Assert(stagedFloats.Any(value => value.Start > castStarted + 0.04f), "Fireball damage feedback waits for impact timing");
            Assert(stagedBeams.Any(value => value.Kind == "fireball" && value.Start <= castStarted + 0.04f), "Fireball projectile begins before impact feedback");
            BeamEffect stagedFireball = stagedBeams.Last(value => value.Kind == "fireball");
            CombatImpactProfile stagedFireballProfile = CombatImpactRules.ForFormula(fireball);
            Assert(Math.Abs(stagedFireball.Duration - stagedFireballProfile.ImpactDelay) < 0.0001f, "Fireball projectile duration lands on the canonical impact beat");
            float firstTargetFeedback = stagedFloats
                .Where(value => value.X == spellTarget.X && value.Y == spellTarget.Y && value.Start >= castStarted)
                .Min(value => value.Start);
            Assert(firstTargetFeedback + 0.025f >= stagedFireball.Start + stagedFireball.Duration, "Fireball target feedback does not precede projectile arrival");
            int scheduledSfxCount = (int)scheduledSfx.GetType().GetProperty("Count").GetValue(scheduledSfx);
            Assert(scheduledSfxCount >= 1, "Fireball queues delayed impact audio");
            object stagedImpactCue = null;
            for (int cueIndex = 0; cueIndex < scheduledSfxCount; cueIndex++)
            {
                object candidate = scheduledSfx.GetType().GetProperty("Item").GetValue(scheduledSfx, new object[] { cueIndex });
                string cueKey = (string)candidate.GetType().GetField("Key", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(candidate);
                if (cueKey == stagedFireballProfile.ImpactSfx)
                {
                    stagedImpactCue = candidate;
                    break;
                }
            }
            Assert(stagedImpactCue != null, "Fireball queues its primary target impact cue");
            float stagedPan = (float)stagedImpactCue.GetType().GetField("Pan", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(stagedImpactCue);
            float stagedPitch = (float)stagedImpactCue.GetType().GetField("Pitch", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(stagedImpactCue);
            Assert(stagedPan < -0.10f && stagedPan >= -0.85f, "Fireball impact audio follows its left-side battlefield target");
            Assert(stagedPitch >= 0.95f && stagedPitch <= 1.05f, "Fireball impact pitch variation remains restrained");
            Assert(castAuras.Any(aura => aura.SourceX == active.X && aura.SourceY == active.Y && aura.TargetX == spellTarget.X && aura.TargetY == spellTarget.Y && aura.Focused), "focused Fireball stages a caster-to-target power aura");
            Assert(impactEchoes.Any(echo => echo.X == spellTarget.X && echo.Y == spellTarget.Y && echo.Intensity == 3 && echo.ReactionCount >= 1 && echo.ImpactAt > echo.Start), "gas ignition promotes Fireball into an epic reaction echo");
            CombatUnitPresentationBeat fireballTargetBeat = unitPresentationBeats
                .LastOrDefault(beat => beat != null && beat.UnitId == spellTarget.Id);
            Assert(
                fireballTargetBeat != null
                && (fireballTargetBeat.Kind == CombatUnitPresentationBeatKind.Hit || fireballTargetBeat.Kind == CombatUnitPresentationBeatKind.Defeat)
                && Math.Abs(fireballTargetBeat.ImpactAt - (castStarted + stagedFireballProfile.ImpactDelay)) < 0.06f,
                "Fireball target sprite reaction lands on the canonical impact beat");
            List<ParticleDot> fireballParticles = stagedParticles;
            int expectedFireAftermath = CombatPowerVisualRules.AftermathParticleCount(CombatPowerVisualMotif.Fire, 3);
            Assert(
                fireballParticles.Count > 0 && fireballParticles.Count <= expectedFireAftermath,
                $"Fireball keeps one bounded motif-specific aftermath layer ({fireballParticles.Count}/{expectedFireAftermath})");
            Assert(fireballParticles.All(value => value.Kind == "ember"), "Fireball legacy smoke and burst stacks yield to one authored ember aftermath");
            Assert(stagedGlyphs.Count == 0 && stagedFlashes.Count == 0, "shared Fireball feedback suppresses legacy target glyph and tile-flash overlays");
            bool hasResonanceCue = false;
            for (int cueIndex = 0; cueIndex < scheduledSfxCount; cueIndex++)
            {
                object cue = scheduledSfx.GetType().GetProperty("Item").GetValue(scheduledSfx, new object[] { cueIndex });
                string cueKey = (string)cue.GetType().GetField("Key", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(cue);
                if (cueKey == "resonance") hasResonanceCue = true;
            }
            Assert(hasResonanceCue, "gas ignition adds a dedicated resonance audio layer");
            Assert(GetPrivateField<float>(game, "combatMusicDuckDepth") >= 0.36f, "reaction Fireball briefly ducks combat music for impact clarity");
            float duckFullDepthAt = GetPrivateField<float>(game, "combatMusicDuckFullDepthAt");
            Assert(Math.Abs(duckFullDepthAt - (castStarted + stagedFireballProfile.ImpactDelay)) < 0.06f, "Fireball music duck reaches full depth on the canonical impact beat");
            Assert(GetPrivateField<float>(game, "combatMusicDuckStartedAt") < duckFullDepthAt, "Fireball music duck attacks before impact");
            Assert(GetPrivateField<float>(game, "combatMusicDuckHoldUntil") > duckFullDepthAt, "Fireball music duck preserves a short post-impact hold");
            Assert(GetPrivateField<float>(game, "combatMusicDuckUntil") > Time.time, "Fireball music duck has a live recovery window");
            Assert(GetPrivateField<List<string>>(game, "combatPowerReactions").Contains("Gas ignition"), "Fireball records its gas-terrain combat reaction");
            Assert(GetPrivateField<string>(game, "combatPowerOutcomeText").StartsWith("Gas ignition", StringComparison.Ordinal), "power outcome promotes the gas ignition reaction");
            Assert(Math.Abs(GetPrivateField<float>(game, "combatVfxImpactDelay")) < 0.0001f, "combat VFX timeline context restores after casting");

            List<ParticleDot> fieldParticles = stagedParticles;
            List<CastGlyph> fieldGlyphs = stagedGlyphs;
            List<CellFlash> fieldFlashes = stagedFlashes;
            combatState.ReducedMotion = true;
            InvokePrivate(game, "ClearCombatMotionForReducedMotion");
            Assert(stagedBeams.Count == 0 && impactEchoes.Count == 0 && castAuras.Count == 0 && fieldParticles.Count == 0 && fieldGlyphs.Count == 0, "enabling Reduced Motion clears queued combat travel and animated spectacle immediately");
            Assert(fieldFlashes.Count == 1, "Reduced Motion preserves one compact target-local impact confirmation");
            Assert(Math.Abs(GetPrivateField<float>(game, "combatShakeMagnitude")) < 0.0001f, "Reduced Motion clears queued combat shake");
            combatState.ReducedMotion = false;
            int particlesBeforeField = fieldParticles.Count;
            int glyphsBeforeField = fieldGlyphs.Count;
            int flashesBeforeField = fieldFlashes.Count;
            InvokePrivate(game, "AddFieldActivationFeedback", active, new Point(active.X, active.Y, "fire", 3));
            Assert(fieldParticles.Count > particlesBeforeField, "persistent fire activation keeps one compact particle response");
            Assert(fieldGlyphs.Count == glyphsBeforeField && fieldFlashes.Count == flashesBeforeField, "persistent fire activation no longer stacks a glyph and tile flash over authored terrain");
            Assert(GetPrivateField<string>(game, "lastSfxKey") == "fieldfire", "persistent fire field activation uses its dedicated spatial cue");

            combatState.Combat.Obstacles.Clear();
            active.ClassKey = "rogue";
            active.Role = "rogue";
            active.Spell = "";
            active.Level = Math.Max(active.Level, 2);
            active.X = 1;
            active.Y = 1;
            active.Stealthed = 0;
            spellTarget.X = 4;
            spellTarget.Y = 1;
            spellTarget.MaxHp = Math.Max(spellTarget.MaxHp, 240);
            spellTarget.Hp = spellTarget.MaxHp;
            spellTarget.Poisoned = 0;
            IReadOnlyList<CombatAbilityModalCardView> rogueCards = InvokePrivate<IReadOnlyList<CombatAbilityModalCardView>>(game, "BuildSkillModalCards", active, true);
            CombatAbilityModalCardView smokeBombCard = rogueCards.First(card => card.Id == "smokebomb");
            Assert(smokeBombCard.RowSummary.Contains("sight-blocking")
                && smokeBombCard.CurrentEffect.Contains("blocks direct sight but not movement")
                && !smokeBombCard.CurrentEffect.Contains("poison"), "Smoke Bomb skillbook shows its immediate sight-control effect without poison-gas clutter");
            Assert(InvokePrivate<bool>(game, "UseSmokeBomb", active), "Smoke Bomb resolves through its production field resolver");
            List<Point> smokeClouds = combatState.Combat.Obstacles
                .Where(obstacle => obstacle != null && obstacle.Kind == "smoke")
                .ToList();
            Assert(smokeClouds.Count == 4 && smokeClouds.All(cloud => cloud.Duration == 3), "Smoke Bomb fills every adjacent open tile with three-round smoke");
            Assert(!InvokePrivate<bool>(game, "HasLineOfSight", active.X, active.Y, spellTarget.X, spellTarget.Y, true), "Smoke Bomb blocks the missile sight line through its eastern cloud");
            FormulaDef arcSpark = FormulaCatalog.All.First(formula => formula.Code == "RIG");
            active.Spell = "ember";
            Assert(!InvokePrivate<bool>(game, "HasFormulaLineOfSight", arcSpark, active, spellTarget.X, spellTarget.Y), "Smoke Bomb blocks direct Arc Spark sight");
            Assert(InvokePrivate<bool>(game, "CanStandAt", 2, 1), "Smoke Bomb leaves its occupied field tile open to movement");
            int smokeProbeHp = spellTarget.Hp;
            spellTarget.X = 2;
            spellTarget.Y = 1;
            InvokePrivate<bool>(game, "ApplyStartTurnEffects", spellTarget);
            Assert(spellTarget.Hp == smokeProbeHp && spellTarget.Poisoned == 0, "standing in Smoke Bomb smoke causes neither damage nor poison");
            spellTarget.X = 4;
            spellTarget.Y = 1;

            scheduledSfx.GetType().GetMethod("Clear").Invoke(scheduledSfx, null);
            stagedFloats.Clear();
            stagedBeams.Clear();
            impactEchoes.Clear();
            castAuras.Clear();
            stagedGlyphs.Clear();
            stagedFlashes.Clear();
            stagedParticles.Clear();
            combatState.Combat.Obstacles.Clear();
            active.Level = Math.Max(active.Level, 6);
            active.ClassKey = "mage";
            active.Role = "ember";
            active.Spell = "ember";
            active.Mana = active.MaxMana;
            active.X = 1;
            active.Y = 1;
            combatState.Combat.ActionAvailable = true;
            combatState.Combat.Moved = false;
            combatState.Combat.MovePoints = Math.Max(3, active.Movement);

            spellTarget.MaxHp = Math.Max(spellTarget.MaxHp, 240);
            spellTarget.Hp = spellTarget.MaxHp;
            spellTarget.X = 3;
            spellTarget.Y = 3;
            active.X = 1;
            active.Y = 3;
            active.Mana = active.MaxMana;
            Assert(InvokePrivate<bool>(game, "CastFormula", active, "RIG", spellTarget, spellTarget.X, spellTarget.Y), "Arc Spark resolves through the production formula path");
            Assert(spellTarget.Hp < spellTarget.MaxHp, "Arc Spark deals direct shock damage");
            Assert(stagedBeams.Any(value => value.Kind == "lightning"), "Arc Spark draws its dedicated lightning delivery");

            stagedFloats.Clear();
            stagedBeams.Clear();
            combatState.Combat.Obstacles.Clear();
            active.X = 2;
            active.Y = 3;
            spellTarget.X = 3;
            spellTarget.Y = 3;
            spellTarget.Hp = spellTarget.MaxHp;
            spellTarget.Stunned = 0;
            combatState.Combat.Obstacles.Add(new Point(4, 3, "stone"));
            active.Mana = active.MaxMana;
            Assert(InvokePrivate<bool>(game, "CastFormula", active, "RSG", active, active.X, active.Y), "Thunderclap resolves through its custom production effect");
            Assert(spellTarget.X == 3 && spellTarget.Y == 3
                && spellTarget.Hp < spellTarget.MaxHp
                && spellTarget.Stunned >= 1, "blocked Thunderclap push causes damage and a guaranteed collision stun");
            Assert(stagedFloats.Any(value => value.Text == "COLLISION")
                && GetPrivateField<List<string>>(game, "combatPowerReactions").Contains("Thunder collision"), "Thunderclap publishes its collision result");
            Assert(stagedBeams.Any(value => value.Kind == "lightning"), "Thunderclap radiates visible lightning");

            combatState.Combat.Obstacles.Clear();
            CombatUnit conductiveGap = MakeRuntimeEnemy("conductive-gap", 6, 3);
            combatState.Combat.Units.RemoveAll(unit => unit == null || unit.Id != active.Id && unit.Id != spellTarget.Id);
            combatState.Combat.Units.Add(conductiveGap);
            spellTarget.X = 3;
            spellTarget.Y = 3;
            spellTarget.Hp = spellTarget.MaxHp;
            Assert(InvokePrivate<List<CombatUnit>>(game, "BuildLightningChain", spellTarget).Count == 1, "ordinary Chain Lightning cannot cross a three-tile gap");
            combatState.Combat.Obstacles.Add(new Point(spellTarget.X, spellTarget.Y, "ice", 3));
            Assert(InvokePrivate<List<CombatUnit>>(game, "BuildLightningChain", spellTarget).Count == 2, "conductive terrain extends Chain Lightning across a three-tile gap");

            CombatUnit chainSecond = MakeRuntimeEnemy("chain-second", 5, 3);
            CombatUnit chainThird = MakeRuntimeEnemy("chain-third", 7, 3);
            CombatUnit chainFourth = MakeRuntimeEnemy("chain-fourth", 9, 3);
            CombatUnit chainOutside = MakeRuntimeEnemy("chain-outside", 11, 3);
            combatState.Combat.Obstacles.Clear();
            combatState.Combat.Units.RemoveAll(unit => unit == null || unit.Id != active.Id && unit.Id != spellTarget.Id);
            combatState.Combat.Units.AddRange(new[] { chainSecond, chainThird, chainFourth, chainOutside });
            active.X = 1;
            active.Y = 3;
            active.Mana = active.MaxMana;
            stagedFloats.Clear();
            stagedBeams.Clear();
            Assert(InvokePrivate<bool>(game, "CastFormula", active, "CLT", spellTarget, spellTarget.X, spellTarget.Y), "Chain Lightning resolves through its custom production effect");
            Assert(new[] { spellTarget, chainSecond, chainThird, chainFourth }.All(enemy => enemy.Hp < enemy.MaxHp)
                && chainOutside.Hp == chainOutside.MaxHp, "Chain Lightning damages exactly four deterministic linked targets");
            Assert(stagedBeams.Count(value => value.Kind == "lightning") >= 4, "Chain Lightning stages one visible bolt per linked target");
            Assert(stagedFloats.Any(value => value.Text == "ARC")
                && stagedFloats.Any(value => value.Text == "JUMP 4"), "Chain Lightning labels its primary and final jump");

            combatState.Combat.Units.RemoveAll(unit => unit == null || unit.Id != active.Id && unit.Id != spellTarget.Id);
            combatState.Combat.Obstacles.Clear();
            stagedFloats.Clear();
            stagedBeams.Clear();
            castAuras.Clear();
            active.X = 1;
            active.Y = 1;
            spellTarget.X = 6;
            spellTarget.Y = 3;
            spellTarget.Hp = spellTarget.MaxHp;
            int thunderStepTargetHp = spellTarget.Hp;
            active.Mana = active.MaxMana;
            Assert(InvokePrivate<bool>(game, "CastFormula", active, "VST", null, 5, 3), "Thunder Step resolves through the production formula path");
            Assert(active.X == 5 && active.Y == 3, "Thunder Step moves the caster to the chosen open tile");
            Assert(spellTarget.Hp < thunderStepTargetHp, "Thunder Step shocks enemies beside its destination");
            Assert(stagedBeams.Any(value => value.Kind == "arc")
                && stagedBeams.Any(value => value.Kind == "lightning"), "Thunder Step draws both its travel arc and arrival lightning");
            Assert(stagedFloats.Any(value => value.Text == "THUNDER STEP"), "Thunder Step stages its named landing result");
            Assert(castAuras.Any(aura => aura.SourceX == 1 && aura.SourceY == 1 && aura.TargetX == 5 && aura.TargetY == 3), "Thunder Step stages a destination-aware cast aura");

            combatState.Combat.Obstacles.Clear();
            Point sealedGlyph = new Point(4, 3, "glyph", 2);
            combatState.Combat.Obstacles.Add(sealedGlyph);
            active.Mana = active.MaxMana;
            Assert(InvokePrivate<bool>(game, "CastFormula", active, "SRF", null, sealedGlyph.X, sealedGlyph.Y), "Rift Seal resolves through the production formula path");
            Assert(!combatState.Combat.Obstacles.Contains(sealedGlyph), "Rift Seal removes a ritual mark");
            Assert(GetPrivateField<List<FloatText>>(game, "floatTexts").Any(value => value.Text == "SEALED"), "Rift Seal stages a readable sealing result");
            Assert(GetPrivateField<List<string>>(game, "combatPowerReactions").Contains("Rift sealed"), "Rift Seal records its ritual reaction");

            stagedFloats.Clear();
            stagedBeams.Clear();
            impactEchoes.Clear();
            active.Mana = active.MaxMana;
            spellTarget.Hp = spellTarget.MaxHp;
            CombatUnit tempestNearOne = MakeRuntimeEnemy("tempest-near-one", spellTarget.X + 2, spellTarget.Y);
            CombatUnit tempestNearTwo = MakeRuntimeEnemy("tempest-near-two", spellTarget.X, spellTarget.Y + 2);
            CombatUnit tempestOutside = MakeRuntimeEnemy("tempest-outside", spellTarget.X + 3, spellTarget.Y);
            combatState.Combat.Units.AddRange(new[] { tempestNearOne, tempestNearTwo, tempestOutside });
            int tempestCenterHp = spellTarget.Hp;
            Assert(InvokePrivate<bool>(game, "CastFormula", active, "AST", spellTarget, spellTarget.X, spellTarget.Y), "Arcane Tempest resolves through the production formula path");
            Assert(spellTarget.Hp < tempestCenterHp
                && tempestNearOne.Hp < tempestNearOne.MaxHp
                && tempestNearTwo.Hp < tempestNearTwo.MaxHp
                && tempestOutside.Hp == tempestOutside.MaxHp, "Arcane Tempest damages its center and radius-two enemies but not a radius-three outsider");
            Assert(stagedFloats.Any(value => value.Text == "TEMPEST"), "Arcane Tempest stages its signature battlefield result");
            Assert(stagedBeams.Count(value => value.Kind == "lightning") >= 3, "Arcane Tempest forks visible lightning across its footprint");
            Assert(impactEchoes.Any(echo => echo.X == spellTarget.X && echo.Y == spellTarget.Y && echo.Intensity == 3), "Arcane Tempest owns an epic impact echo");
            combatState.Combat.Units.RemoveAll(unit => unit == tempestNearOne || unit == tempestNearTwo || unit == tempestOutside);
            spellTarget.X = 4;
            spellTarget.Y = 1;
            spellTarget.Stunned = 0;

            Point openingGlyph = new Point(7, 3, "glyph", 1);
            combatState.Combat.Obstacles.Add(openingGlyph);
            int enemyCountBeforeRitual = combatState.Combat.Units.Count(unit => unit.Side == UnitSide.Enemy && unit.Hp > 0);
            InvokePrivate(game, "TickCombatRoundState");
            CombatUnit ritualSpawn = combatState.Combat.Units.FirstOrDefault(unit => unit.Origin == "ritual" && unit.Hp > 0);
            Assert(ritualSpawn != null && ritualSpawn.Role == "koboldraider", "unchecked glyph opens into a kobold reinforcement");
            Assert(combatState.Combat.Units.Count(unit => unit.Side == UnitSide.Enemy && unit.Hp > 0) == enemyCountBeforeRitual + 1, "ritual reinforcement joins the enemy side");
            Assert(combatState.Combat.InitiativeQueue.Contains(ritualSpawn.Id), "ritual reinforcement joins the current round queue");
            combatState.Combat.Units.Remove(ritualSpawn);
            combatState.Combat.InitiativeQueue.Remove(ritualSpawn.Id);

            InvokePrivate(game, "PromoteWarlockTester", active);
            combatState.Combat.ActionAvailable = true;
            combatState.Combat.Moved = false;
            combatState.Combat.MovePoints = Math.Max(3, active.Movement);
            active.Hp = Math.Max(1, active.MaxHp - 12);
            int woundedHp = active.Hp;
            InvokePrivate(game, "LateUpdate");
            CombatAbilityModalView pactProgression = InvokePrivate<CombatAbilityModalView>(game, "BuildCombatAbilityModalView");
            CombatAbilityModalCardView ascendanceCard = pactProgression.Cards.First(card => card.Id == "DFA");
            Assert(modal.IsVisible
                && !ascendanceCard.Targeted
                && !ascendanceCard.Locked
                && ascendanceCard.Epic, "pact spellbook exposes the unlocked elder transformation as an immediate power");
            modal.SetFilterForTest(CombatAbilityModalFilter.All);
            InvokePrivate(game, "SelectCombatAbilityModalCard", "DFA");
            int ascendanceManaBefore = active.Mana;
            modal.InvokeSelectedForTest();
            Assert(!modal.IsVisible
                && active.Mana < ascendanceManaBefore
                && !combatState.Combat.ActionAvailable, "Use Now resolves a self-only formula exactly once through the real Spellbook action");
            Assert(active.DemonFormTurns >= 4 && active.Hp > woundedHp, "Abyssal Ascendance applies duration and immediate healing");
            Assert(InvokePrivate<int>(game, "DemonFormAttackBonus", active) == 4, "demon form grants its physical power bonus");
            Assert(InvokePrivate<int>(game, "DemonFormDamageReduction", active) == 2, "demon form grants its damage reduction");
            Assert(InvokePrivate<int>(game, "DemonSummonSpriteIndex", active) >= 8, "demon form swaps to a greater-demon combat sprite");
            InvokePrivate(game, "CancelCombatResolutionBeat", true);
            combatState.Combat.ActiveId = active.Id;
            combatState.Combat.ActionAvailable = true;
            combatState.Combat.Acted = false;
            combatState.Combat.Phase = CombatPhase.ChooseAction;

            scheduledSfx.GetType().GetMethod("Clear").Invoke(scheduledSfx, null);
            stagedFloats.Clear();
            stagedBeams.Clear();
            impactEchoes.Clear();
            castAuras.Clear();
            stagedGlyphs.Clear();
            stagedFlashes.Clear();
            stagedParticles.Clear();
            combatState.Combat.Obstacles.Clear();
            active.ClassKey = "ranger";
            active.Role = "bow";
            active.Spell = "";
            active.DemonFormTurns = 0;
            active.Level = Math.Max(active.Level, 4);
            active.Range = 6;
            active.WeaponName = "fine longbow";
            active.DamageMin = Math.Max(active.DamageMin, 7);
            active.DamageMax = Math.Max(active.DamageMax, 11);
            if (active.Skills == null) active.Skills = new SkillSet();
            active.Skills.Missile = Math.Max(active.Skills.Missile, 24);
            spellTarget.Hp = spellTarget.MaxHp;
            active.X = 1;
            active.Y = 1;
            spellTarget.X = 4;
            spellTarget.Y = 1;
            combatState.Combat.ActiveId = active.Id;
            combatState.Combat.ActionAvailable = true;
            combatState.Combat.Phase = CombatPhase.ChooseAction;
            combatState.Combat.Moved = false;
            combatState.Combat.MovePoints = Math.Max(3, active.Movement);
            InvokePrivate(game, "SelectOrRunAction", ActionMode.Ability, active);
            InvokePrivate(game, "LateUpdate");
            CombatAbilityModalView skillbookView = InvokePrivate<CombatAbilityModalView>(game, "BuildCombatAbilityModalView");
            CombatAbilityModalCardView aimedShotCard = skillbookView.Cards.First(card => card.Id == "aimedshot");
            Assert(skillbookView.StateIconTexture == powerBookStateIcons, "skillbook view carries the authored state microicon atlas");
            foreach (string classKey in new[] { "warrior", "rogue", "ranger" })
            {
                foreach (string abilityId in AbilityCatalog.IdsForClass(classKey))
                {
                    MartialAbility productionAbility = AbilityCatalog.For(abilityId);
                    CombatAbilityModalCardView artProbe = new CombatAbilityModalCardView();
                    InvokePrivate(game, "ApplyAbilityModalArt", artProbe, productionAbility);
                    int abilityIndex = CombatIconCatalog.AbilityIndex(abilityId);
                    Rect expectedSource = InvokePrivate<Rect>(game, "AbilityIconAtlasCell", abilityIndex);
                    Assert(abilityIndex >= 0
                        && artProbe.IconTexture == abilityIcons
                        && artProbe.IconSource.Equals(expectedSource),
                        productionAbility.Name + " resolves its unique ability atlas cell");
                }
            }
            Assert(modal.IsVisible
                && skillbookView.Title.EndsWith("Skillbook", StringComparison.Ordinal)
                && skillbookView.Actor.Contains(active.Name)
                && skillbookView.Actor.Contains("L" + active.Level)
                && skillbookView.ActionState == "ACTION READY"
                && !modal.UsesGeneratedStateIconAtlasForTest
                && modal.VisibleStateIconCountForTest >= 1
                && modal.VisibleTargetingRailCountForTest == 0, "combat Skills command opens the structured martial skillbook with authored state chrome");
            Assert(aimedShotCard.TargetCountKnown && aimedShotCard.ValidTargetCount > 0
                && !string.IsNullOrWhiteSpace(aimedShotCard.RowSummary)
                && !string.IsNullOrWhiteSpace(aimedShotCard.CurrentEffect)
                && !aimedShotCard.CurrentEffect.Contains("TACTICS"), "skillbook computes legal targets and shows only the live Aimed Shot effect");
            Assert(!string.IsNullOrWhiteSpace(CombatAbilityModalPresentationRules.DetailMeta(aimedShotCard))
                && !string.IsNullOrWhiteSpace(CombatAbilityModalPresentationRules.DetailNotes(aimedShotCard))
                && !CombatAbilityModalPresentationRules.DetailNotes(aimedShotCard).Contains("CURRENT EFFECT"), "skillbook detail adds profile and tactics without repeating its live outcome");
            InvokePrivate(game, "SelectCombatAbilityModalCard", "aimedshot");
            Assert(modal.DetailTargetLabelForTest.Contains("legal")
                && modal.DetailTargetLabelForTest.Contains("enem"), "Skillbook target chip names Aimed Shot's legal enemies");
            List<CombatAbilityModalCardView> visibleSkillCards = skillbookView.Cards
                .Where(card => CombatAbilityModalPresentationRules.MatchesFilter(card, modal.ActiveFilter))
                .ToList();
            int alternateSkillIndex = visibleSkillCards.FindIndex(card => card.Id != "aimedshot");
            Assert(alternateSkillIndex >= 0, "skillbook hover regression probe has a second actionable skill");
            float skillHoverScrollY = modal.ScrollYForTest;
            modal.HoverVisibleIndexForTest(alternateSkillIndex);
            Assert(modal.DetailIdForTest == visibleSkillCards[alternateSkillIndex].Id
                && modal.SelectedId == "aimedshot"
                && GetPrivateField<string>(game, "abilitySelectedId") == "aimedshot"
                && modal.SelectedRailCountForTest == 1
                && !modal.DetailActionInteractableForTest
                && modal.DetailActionLabelForTest == "Preview Only"
                && modal.DetailPromptForTest.Contains("Click or focus the card")
                && Mathf.Approximately(modal.ScrollYForTest, skillHoverScrollY), "skill hover remains a passive preview without changing selection memory or scroll");
            string previewedSkillId = visibleSkillCards[alternateSkillIndex].Id;
            int previewCommitMovePoints = combatState.Combat.MovePoints;
            bool previewCommitAction = combatState.Combat.ActionAvailable;
            modal.InvokeDetailActionForTest();
            Assert(modal.IsVisible
                && modal.SelectedId == "aimedshot"
                && modal.PreviewedIdForTest == previewedSkillId
                && string.IsNullOrEmpty(GetPrivateField<string>(game, "pendingAbilityId"))
                && combatState.Combat.MovePoints == previewCommitMovePoints
                && combatState.Combat.ActionAvailable == previewCommitAction, "skill preview detail cannot commit or arm a second action path");
            modal.SelectVisibleIndexForTest(alternateSkillIndex);
            Assert(modal.IsVisible
                && modal.SelectedId == previewedSkillId
                && string.IsNullOrEmpty(modal.PreviewedIdForTest)
                && string.IsNullOrEmpty(GetPrivateField<string>(game, "pendingAbilityId"))
                && combatState.Combat.MovePoints == previewCommitMovePoints
                && combatState.Combat.ActionAvailable == previewCommitAction
                && (!Application.isPlaying || modal.SelectedRowFocusedForTest), "clicking the skill card commits one focused row without arming or spending");
            InvokePrivate(game, "SelectCombatAbilityModalCard", "aimedshot");
            modal.ClearHoverForTest();
            Assert(modal.DetailIdForTest == "aimedshot"
                && string.IsNullOrEmpty(modal.PreviewedIdForTest), "leaving a skill row restores the committed skill detail");

            modal.SetFilterForTest(CombatAbilityModalFilter.All);
            foreach (CombatAbilityModalCardView productionSkill in skillbookView.Cards)
            {
                InvokePrivate(game, "SelectCombatAbilityModalCard", productionSkill.Id);
                InvokePrivate(game, "LateUpdate");
                Assert(modal.DetailNarrativeFullyPresentedForTest, productionSkill.Name + " skill detail fits its regions or remains fully scrollable");
                Assert(!Application.isPlaying || modal.SelectedRowFocusedForTest, productionSkill.Name + " keeps semantic controller focus on its selected row");
            }

            int rangerLevel = active.Level;
            active.Level = 1;
            modal.Refresh();
            CombatAbilityModalView noviceSkillbook = InvokePrivate<CombatAbilityModalView>(game, "BuildCombatAbilityModalView");
            int futureSkillCount = CombatAbilityModalPresentationRules.Count(noviceSkillbook.Cards, CombatAbilityModalFilter.Future);
            int knownSkillCount = CombatAbilityModalPresentationRules.Count(noviceSkillbook.Cards, CombatAbilityModalFilter.Learned);
            Assert(noviceSkillbook.Actor.Contains("L1")
                && futureSkillCount > 0
                && knownSkillCount > 0
                && futureSkillCount + knownSkillCount == noviceSkillbook.Cards.Count, "level-one Skillbook divides real Known and Locked cards without losing entries");
            modal.SetFilterForTest(CombatAbilityModalFilter.Future);
            Assert(modal.VisibleCardCount == futureSkillCount
                && modal.VisibleStatusBadgeCountForTest == 0, "Skillbook Locked view renders every future martial power without repeating row badges");
            modal.SetFilterForTest(CombatAbilityModalFilter.Learned);
            Assert(modal.VisibleCardCount == knownSkillCount, "Skillbook Known view renders every learned martial power");
            modal.SetFilterForTest(CombatAbilityModalFilter.All);
            Assert(modal.VisibleCardCount == noviceSkillbook.Cards.Count, "Skillbook All view restores the complete martial book");
            active.Level = rangerLevel;
            modal.Refresh();
            modal.SetFilterForTest(CombatAbilityModalFilter.All);
            InvokePrivate(game, "SelectCombatAbilityModalCard", "aimedshot");

            int skillbookHp = spellTarget.Hp;
            int skillbookMovePoints = combatState.Combat.MovePoints;
            modal.InvokeSelectedForTest();
            Assert(!modal.IsVisible
                && GetPrivateField<string>(game, "pendingAbilityId") == "aimedshot"
                && GetPrivateField<ActionMode>(game, "selectedAction") == ActionMode.Ability
                && combatState.Combat.Phase == CombatPhase.ChooseTarget, "skillbook primary action closes the book and arms Aimed Shot");
            Assert(spellTarget.Hp == skillbookHp
                && combatState.Combat.MovePoints == skillbookMovePoints
                && combatState.Combat.ActionAvailable, "arming a skill through the book spends nothing before target confirmation");
            Assert(InvokePrivate<bool>(game, "CancelCombatTargeting"), "skill targeting can be canceled without spending the action");
            Assert(string.IsNullOrEmpty(GetPrivateField<string>(game, "pendingAbilityId"))
                && combatState.Combat.ActionAvailable, "canceling a skill returns to an action-ready state");

            SetPrivateField(game, "pendingFormulaCode", "FBL");
            SetPrivateField(game, "pendingAbilityId", "aimedshot");
            InvokePrivate(game, "CloseTransientOverlays");
            Assert(string.IsNullOrEmpty(GetPrivateField<string>(game, "pendingFormulaCode"))
                && string.IsNullOrEmpty(GetPrivateField<string>(game, "pendingAbilityId")), "transient-overlay cleanup clears stale armed powers before load or scene changes");
            combatState.Combat.ActionAvailable = true;
            combatState.Combat.Phase = CombatPhase.ChooseAction;
            SetPrivateField(game, "rng", new System.Random(1));
            Assert(InvokePrivate<bool>(game, "UseTargetedAbility", active, "aimedshot", spellTarget, spellTarget.X, spellTarget.Y), "Aimed Shot resolves through the centralized martial presentation path");
            Assert(castAuras.Any(aura => aura.SourceX == active.X && aura.TargetX == spellTarget.X && aura.Kind == "aimedshot"), "Aimed Shot stages a caster-origin skill aura");
            Assert(impactEchoes.Any(echo => echo.X == spellTarget.X && echo.Kind == "bow"), "Aimed Shot receives the shared impact echo");
            Assert(stagedGlyphs.Count == 0 && stagedFlashes.Count == 0, "shared Aimed Shot feedback suppresses the legacy ranger glyph and tile flash");
            Assert((int)scheduledSfx.GetType().GetProperty("Count").GetValue(scheduledSfx) >= 1, "Aimed Shot queues staged release and impact audio");

            scheduledSfx.GetType().GetMethod("Clear").Invoke(scheduledSfx, null);
            stagedFloats.Clear();
            stagedBeams.Clear();
            impactEchoes.Clear();
            castAuras.Clear();
            active.MaxHp = Math.Max(active.MaxHp, 120);
            active.Hp = active.MaxHp;
            spellTarget.Role = "koboldking";
            spellTarget.Name = "Kobold King";
            spellTarget.Power = Math.Max(spellTarget.Power, 12);
            spellTarget.Range = Math.Max(spellTarget.Range, 4);
            spellTarget.X = 4;
            spellTarget.Y = 1;
            combatState.Combat.ActiveId = spellTarget.Id;
            combatState.Combat.Phase = CombatPhase.EnemyThinking;
            InvokePrivate(game, "ResetEnemyActionPresentation");
            CombatUnit intendedTarget = InvokePrivate<CombatUnit>(game, "EnemyIntentFocus", spellTarget);
            CombatHudView enemyHudView = InvokePrivate<CombatHudView>(game, "BuildCombatHudView");
            CombatAttackForecast bossForecast = InvokePrivate<CombatAttackForecast>(game, "AttackForecast", spellTarget, active);
            Assert(intendedTarget == active, "enemy intent uses the production target scorer");
            Assert(enemyHudView.PhaseLine.StartsWith("ENEMY TURN", StringComparison.Ordinal), "enemy initiative is announced as the primary combat phase cue");
            Assert(enemyHudView.ActionLabel == "ACTION\nENEMY"
                && enemyHudView.ActiveUnit.StateLine.Contains("ACTION ENEMY")
                && !enemyHudView.ActiveUnit.StateLine.Contains("ACTION READY"), "enemy initiative uses one consistent non-player action state in the header and active card");
            Assert(enemyHudView.TargetUnit != null && enemyHudView.TargetUnit.Name == active.Name, "enemy HUD target matches its tactical intent");
            Assert(enemyHudView.CommandPrompt.StartsWith("INTENT:", StringComparison.Ordinal) && enemyHudView.CommandPrompt.Contains(active.Name), "enemy turn publishes a concise target-aware intent line");
            Assert(bossForecast.Legal, "boss basic attack has a valid threat forecast in the smoke lane");
            Assert(enemyHudView.CommandPrompt.Contains(bossForecast.HitChance + "%") && enemyHudView.CommandPrompt.Contains(bossForecast.MinDamage + "-" + bossForecast.MaxDamage), "enemy intent publishes shared hit and damage estimates");
            Assert(enemyHudView.TargetUnit.StateLine.Contains(bossForecast.HitChance + "%"), "enemy target card shares the same forecast");
            float enemyCastStarted = Time.time;
            Assert(InvokePrivate<bool>(game, "TryKoboldKingFireball", spellTarget, active), "Kobold King fireball resolves through production enemy-power path");
            CombatPowerIdentity enemyCue = GetPrivateField<CombatPowerIdentity>(game, "combatPowerCue");
            Assert(enemyCue.Title == "Crooked Fireball" && enemyCue.Intensity == 3, "enemy power publishes its boss identity");
            Assert(stagedBeams.Any(value => value.Kind == "fireball" && value.Start <= enemyCastStarted + 0.04f), "enemy fireball projectile begins before impact");
            Assert(stagedFloats.Any(value => value.Start > enemyCastStarted + 0.04f), "enemy fireball result waits for impact timing");
            Assert((int)scheduledSfx.GetType().GetProperty("Count").GetValue(scheduledSfx) >= 1, "enemy fireball queues delayed impact audio");
            Assert(castAuras.Any(aura => aura.SourceX == spellTarget.X && aura.SourceY == spellTarget.Y && aura.TargetX == active.X && aura.TargetY == active.Y && aura.Intensity == 3), "boss fireball stages an epic caster-origin aura");
            Assert(impactEchoes.Any(echo => echo.X == active.X && echo.Y == active.Y && echo.Intensity == 3 && echo.Duration >= 0.62f), "boss fireball stages an epic target telegraph and impact echo");
            Assert(GetPrivateField<float>(game, "combatMusicDuckDepth") >= 0.46f, "boss fireball claims stronger space in the combat mix");
            Assert(GetPrivateField<float>(game, "enemyActionResolutionDelay") >= 0.50f, "boss power requests a readable resolution hold");
            Assert(Math.Abs(GetPrivateField<float>(game, "combatVfxImpactDelay")) < 0.0001f, "enemy power restores VFX timeline context");
            InvokePrivate(game, "FinishEnemyCombatAction", spellTarget);
            Assert(GetPrivateField<bool>(game, "combatAdvancePending"), "enemy power locks initiative until its impact resolves");
            Assert(combatState.Combat.Phase == CombatPhase.Resolving, "enemy power exposes the resolving combat phase");
            Assert(GetPrivateField<string>(game, "combatResolutionLabel") == "Crooked Fireball", "enemy resolution names the active power");
            InvokePrivate(game, "CancelCombatResolutionBeat", false);

            AssertRoundTransitionAndStartTurnDefeatRuntime(game);

            SetPrivateField(game, "betaLabMode", false);
            SetPrivateField(game, "labSaveBlocked", false);
            combatState.Supplies = 2;
            InvokePrivate(game, "OpenPauseMenu");
            InvokePrivate(game, "LateUpdate");
            PauseMenuScreen pause = GetPrivateField<PauseMenuScreen>(game, "pauseMenuScreen");
            PauseMenuView retreatView = InvokePrivate<PauseMenuView>(game, "BuildPauseMenuView");
            Assert(pause != null && pause.IsVisible && pause.HasRenderableGeometry, "combat pause menu owns the retreat confirmation");
            Assert(retreatView.ShowRetreat && retreatView.RetreatEnabled, "normal campaign combat offers an affordable retreat");
            InvokePrivate(game, "RequestPauseRetreat");
            Assert(InvokePrivate<PauseMenuView>(game, "BuildPauseMenuView").ConfirmRetreat, "retreat requires a deliberate confirmation");
            InvokePrivate(game, "ConfirmPauseRetreat");
            InvokePrivate(game, "LateUpdate");
            AssertMode(game, GameMode.Explore, "confirmed retreat returns to exploration");
            Assert(combatState.Combat == null, "retreat clears combat without granting victory");
            Assert(combatState.Supplies == 1, "retreat spends exactly one supply");
            Assert(combatState.Party.All(member => member.Hp == member.MaxHp && member.Mana == member.MaxMana), "Temple Square retreat restores the party");
            Assert(combatState.Map.Objects.Any(obj =>
                    (obj.Type == ObjectType.RecallCircle || obj.Type == ObjectType.Fountain || obj.Type == ObjectType.Temple)
                    && combatState.PlayerX == obj.X
                    && combatState.PlayerY == obj.Y),
                "retreat lands on a Temple Square anchor");

            InvokePrivate(game, "StartBetaCombatLab");
            InvokePrivate(
                game,
                "StageVisualSmokeCombatState",
                (object)new[] { "-ashen-combat-smoke", "-ashen-combat-state", "move-path" });
            GameState stagedCombatState = GetPrivateField<GameState>(game, "state");
            CombatUnit stagedActive = InvokePrivate<CombatUnit>(game, "CurrentUnit");
            Vector2Int? stagedHover = GetPrivateField<Vector2Int?>(game, "visualSmokeCombatHoverCell");
            IReadOnlyList<Vector2Int> stagedPath = InvokePrivate<IReadOnlyList<Vector2Int>>(
                game,
                "ReachableMovePath",
                stagedActive,
                stagedHover.Value.x,
                stagedHover.Value.y,
                stagedCombatState.Combat.MovePoints);
            Assert(GetPrivateField<bool>(game, "visualSmokeHideCombatDebug")
                && stagedHover == new Vector2Int(4, 3)
                && GetPrivateField<ActionMode>(game, "selectedAction") == ActionMode.Move, "clean movement capture stages its exact state without the developer toolbar");
            Assert(stagedPath.Count > 2
                && stagedPath[0] == new Vector2Int(stagedActive.X, stagedActive.Y)
                && stagedPath[stagedPath.Count - 1] == stagedHover.Value
                && !stagedPath.Contains(new Vector2Int(3, 4)), "clean movement capture exposes a real route around its blocking stone");

            InvokePrivate(
                game,
                "StageVisualSmokeCombatState",
                (object)new[] { "-ashen-combat-smoke", "-ashen-combat-state", "attack-blocked" });
            stagedActive = InvokePrivate<CombatUnit>(game, "CurrentUnit");
            stagedHover = GetPrivateField<Vector2Int?>(game, "visualSmokeCombatHoverCell");
            CombatUnit stagedTarget = InvokePrivate<CombatUnit>(
                game,
                "UnitAt",
                stagedHover.Value.x,
                stagedHover.Value.y);
            CombatAttackForecast stagedBlockedForecast = InvokePrivate<CombatAttackForecast>(
                game,
                "AttackForecast",
                stagedActive,
                stagedTarget);
            CombatUnit stagedFarTarget = stagedCombatState.Combat.Units
                .Where(unit => unit != null && unit.Side == UnitSide.Enemy && unit.Hp > 0 && unit.Id != stagedTarget.Id)
                .OrderByDescending(unit => Mathf.Abs(unit.X - stagedActive.X) + Mathf.Abs(unit.Y - stagedActive.Y))
                .First();
            CombatTargetHighlightState stagedFarHighlight = InvokePrivate<CombatTargetHighlightState>(
                game,
                "CombatTargetHighlightStateAt",
                stagedActive,
                null,
                null,
                stagedFarTarget.X,
                stagedFarTarget.Y);
            Assert(!stagedBlockedForecast.Legal
                && !InvokePrivate<string>(
                    game,
                    "HoverClickInstruction",
                    stagedActive,
                    stagedTarget,
                    null,
                    stagedTarget.X,
                    stagedTarget.Y).StartsWith("Click", StringComparison.OrdinalIgnoreCase), "clean blocked-attack capture keeps forecast, shape state, and instruction aligned");
            Assert(stagedFarHighlight == CombatTargetHighlightState.Blocked, "target sweep marks an out-of-range enemy as blocked instead of silently omitting it");

            InvokePrivate(
                game,
                "StageVisualSmokeCombatState",
                (object)new[] { "-ashen-combat-smoke", "-ashen-combat-state", "spell-aoe" });
            Assert(GetPrivateField<ActionMode>(game, "selectedAction") == ActionMode.Cast
                && GetPrivateField<string>(game, "pendingFormulaCode") == "FBL"
                && GetPrivateField<Vector2Int?>(game, "visualSmokeCombatHoverCell").HasValue, "clean area-spell capture reaches armed targeting deterministically");
        }

        private static void AssertRoundTransitionAndStartTurnDefeatRuntime(AshenHallsGame game)
        {
            GameState originalState = GetPrivateField<GameState>(game, "state");
            System.Random originalRng = GetPrivateField<System.Random>(game, "rng");
            float originalAiActAt = GetPrivateField<float>(game, "aiActAt");
            List<CombatUnitPresentationBeat> presentationBeats =
                GetPrivateField<List<CombatUnitPresentationBeat>>(game, "combatUnitPresentationBeats");
            List<CombatUnitPresentationBeat> originalPresentationBeats =
                presentationBeats.ToList();

            try
            {
                InvokePrivate(game, "CancelCombatResolutionBeat", false);
                presentationBeats.Clear();

                CombatUnit roundHero = MakeRuntimeEnemy("round-hero", 1, 1);
                roundHero.Side = UnitSide.Party;
                roundHero.PartyIndex = 0;
                roundHero.Name = "Round Hero";
                roundHero.ClassKey = "mage";
                roundHero.Role = "ember";
                roundHero.Spell = "ember";
                roundHero.Level = 6;
                roundHero.Mana = roundHero.MaxMana = 30;
                roundHero.Agility = 30;
                roundHero.AttackSpeed = 18;
                roundHero.Hp = roundHero.MaxHp = 80;

                CombatUnit roundEnemy = MakeRuntimeEnemy("round-last-enemy", 10, 6);
                roundEnemy.Agility = 1;
                roundEnemy.AttackSpeed = 1;

                GameState roundState = new GameState
                {
                    SaveVersion = VersionInfo.SaveVersion,
                    Mode = GameMode.Combat,
                    Depth = 1,
                    Seed = 919,
                    ReducedMotion = false,
                    Party = new List<PartyMember>
                    {
                        new PartyMember
                        {
                            Id = roundHero.Id,
                            Name = roundHero.Name,
                            Hp = roundHero.Hp,
                            MaxHp = roundHero.MaxHp,
                            Skills = roundHero.Skills.Clone()
                        }
                    },
                    Combat = new CombatState
                    {
                        Round = 1,
                        ActiveId = roundEnemy.Id,
                        Phase = CombatPhase.EnemyThinking,
                        InitiativeQueue = new List<string> { roundHero.Id, roundEnemy.Id },
                        Units = new List<CombatUnit> { roundHero, roundEnemy },
                        Obstacles = new List<Point>
                        {
                            new Point(2, 2, "fire", 1),
                            new Point(7, 3, "glyph", 1)
                        }
                    }
                };
                SetPrivateField(game, "state", roundState);
                SetPrivateField(game, "rng", new System.Random(919));
                InvokePrivate(game, "InvalidateControllerCaches");

                int enemyCountBefore = roundState.Combat.Units.Count(unit =>
                    unit != null && unit.Side == UnitSide.Enemy && unit.Hp > 0);
                InvokePrivate(game, "NextTurn");

                CombatUnit ritualSpawn = roundState.Combat.Units.FirstOrDefault(unit =>
                    unit != null && unit.Origin == "ritual" && unit.Hp > 0);
                string roundBanner = GetPrivateField<string>(game, "bannerText");
                Assert(roundState.Combat.Round == 2, "round transition advances the round exactly once");
                Assert(roundState.Combat.Phase == CombatPhase.Resolving
                    && !roundState.Combat.ActionAvailable
                    && roundState.Combat.MovePoints == 0,
                    "round transition blocks action and movement before the reserved turn");
                Assert(roundState.Combat.ActiveId == roundHero.Id,
                    "round transition reserves the first living unit without beginning its turn");
                Assert(GetPrivateField<bool>(game, "combatAdvancePending")
                    && GetPrivateField<bool>(game, "combatAdvanceStartsReservedTurn"),
                    "round transition owns the shared resolution timer");
                Assert(GetPrivateField<float>(game, "aiActAt") < 0f,
                    "round transition cannot schedule enemy AI");
                CombatHudView resolvingHud = InvokePrivate<CombatHudView>(game, "BuildCombatHudView");
                Assert(resolvingHud.Commands.Count > 0
                    && resolvingHud.Commands.All(command => command != null && !command.Enabled),
                    "round transition disables every command, including review-only spell and skill books");
                InvokePrivate(game, "RunCombatHudCommand", ActionMode.Cast);
                Assert(roundState.Combat.Phase == CombatPhase.Resolving
                    && GetPrivateField<bool>(game, "combatAdvancePending")
                    && !GetPrivateField<bool>(game, "showSpellbook"),
                    "combat command dispatch cannot break the round gate or open a review book");
                CombatController roundController = InvokePrivate<CombatController>(game, "CombatLifecycle");
                float reservedAdvanceAt = GetPrivateField<float>(game, "combatAdvanceAt");
                string reservedActiveId = roundState.Combat.ActiveId;
                int reservedHeroX = roundHero.X;
                int reservedHeroY = roundHero.Y;
                int reservedHeroHp = roundHero.Hp;
                int reservedHeroMana = roundHero.Mana;
                int reservedEnemyHp = roundEnemy.Hp;
                roundState.Elixirs = 1;
                roundState.Combat.ActionAvailable = true;
                roundState.Combat.MovePoints = 3;
                int reservedResolverCalls = 0;
                CombatCommandResult reservedMove = roundController.TryMove(roundHero, roundHero.X + 1, roundHero.Y);
                CombatCommandResult reservedUndo = roundController.TryUndoMove(roundHero);
                CombatCommandResult reservedAttack = roundController.TryAttack(roundHero, roundEnemy, (actor, target) =>
                {
                    reservedResolverCalls++;
                    return true;
                });
                CombatCommandResult reservedAbility = roundController.TryUseAbility(roundHero, () =>
                {
                    reservedResolverCalls++;
                    return true;
                });
                CombatCommandResult reservedAction = roundController.TryResolveAction(roundHero, () =>
                {
                    reservedResolverCalls++;
                    return true;
                });
                CombatCommandResult reservedGuard = roundController.Guard(roundHero, 4);
                CombatCommandResult reservedItem = roundController.TryUseItem(roundHero, 18, 6);
                CombatCommandResult reservedEndTurn = roundController.EndTurn(roundHero);
                Assert(!reservedMove.Success
                    && !reservedUndo.Success
                    && !reservedAttack.Success
                    && !reservedAbility.Success
                    && !reservedAction.Success
                    && !reservedGuard.Success
                    && !reservedItem.Success
                    && !reservedEndTurn.Success,
                    "controller rejects every public player command during the reserved round hold");
                Assert(reservedResolverCalls == 0
                    && roundHero.X == reservedHeroX
                    && roundHero.Y == reservedHeroY
                    && roundHero.Hp == reservedHeroHp
                    && roundHero.Mana == reservedHeroMana
                    && roundEnemy.Hp == reservedEnemyHp
                    && roundState.Elixirs == 1
                    && !roundHero.Guarding,
                    "reserved round command rejection cannot invoke callbacks or mutate units and resources");
                Assert(roundState.Combat.Phase == CombatPhase.Resolving
                    && roundState.Combat.ActiveId == reservedActiveId
                    && GetPrivateField<bool>(game, "combatAdvancePending")
                    && Mathf.Approximately(GetPrivateField<float>(game, "combatAdvanceAt"), reservedAdvanceAt),
                    "controller command rejection preserves the reserved unit and owning timer");
                roundState.Combat.ActionAvailable = false;
                roundState.Combat.MovePoints = 0;
                Assert(roundBanner.StartsWith("ROUND 2", StringComparison.Ordinal)
                    && roundBanner.Contains("1 field fades")
                    && roundBanner.Contains("1 ritual opens"),
                    "round transition banner combines bounded field and ritual feedback");
                Assert(!roundState.Combat.Obstacles.Any(point => point.Kind == "fire" || point.Kind == "glyph"),
                    "round transition ticks each duration-one field and ritual exactly once");
                Assert(ritualSpawn != null
                    && roundState.Combat.Units.Count(unit => unit != null && unit.Side == UnitSide.Enemy && unit.Hp > 0) == enemyCountBefore + 1
                    && roundState.Combat.InitiativeQueue.Contains(ritualSpawn.Id),
                    "opened ritual joins combat and initiative before the next turn begins");
                float normalRoundDelay = GetPrivateField<float>(game, "combatAdvanceAt") - Time.time;
                Assert(normalRoundDelay > 0.45f && normalRoundDelay <= 0.65f,
                    "normal round transition keeps one brief readable hold");

                SetPrivateField(game, "combatAdvanceAt", Time.time - 0.01f);
                InvokePrivate(game, "CompletePendingCombatAdvance");
                Assert(!GetPrivateField<bool>(game, "combatAdvancePending")
                    && roundState.Combat.Round == 2
                    && roundState.Combat.ActiveId == roundHero.Id
                    && roundState.Combat.Phase == CombatPhase.ChooseAction
                    && roundState.Combat.ActionAvailable,
                    "round-transition completion begins the reserved party turn exactly once");
                Assert(roundState.Combat.Units.Count(unit => unit != null && unit.Origin == "ritual") == 1,
                    "completing the transition does not tick or open the round twice");

                roundState.ReducedMotion = true;
                roundState.Combat.ActiveId = roundState.Combat.InitiativeQueue
                    .Last(id => roundState.Combat.Units.Any(unit => unit != null && unit.Id == id && unit.Hp > 0));
                InvokePrivate(game, "NextTurn");
                float reducedRoundDelay = GetPrivateField<float>(game, "combatAdvanceAt") - Time.time;
                Assert(roundState.Combat.Round == 3
                    && GetPrivateField<bool>(game, "combatAdvanceStartsReservedTurn")
                    && reducedRoundDelay > 0f
                    && reducedRoundDelay <= 0.10f,
                    "Reduced Motion keeps the same round gate with a compressed hold");
                roundState.Combat.ActiveId = roundEnemy.Id;
                SetPrivateField(game, "combatAdvanceAt", Time.time - 0.01f);
                InvokePrivate(game, "CompletePendingCombatAdvance");
                Assert(!GetPrivateField<bool>(game, "combatAdvancePending")
                    && !GetPrivateField<bool>(game, "combatAdvanceStartsReservedTurn")
                    && roundState.Combat.Phase != CombatPhase.Resolving,
                    "stale reserved-turn identity repairs to a live turn instead of leaving an untimed resolving softlock");

                presentationBeats.Clear();
                CombatUnit survivingHero = MakeRuntimeEnemy("dot-surviving-hero", 1, 1);
                survivingHero.Side = UnitSide.Party;
                survivingHero.PartyIndex = 0;
                survivingHero.Name = "Standing Hero";
                CombatUnit doomedEnemy = MakeRuntimeEnemy("dot-doomed-enemy", 5, 2);
                doomedEnemy.Name = "Poisoned Raider";
                doomedEnemy.Hp = 1;
                doomedEnemy.Poisoned = 1;
                CombatUnit followingEnemy = MakeRuntimeEnemy("dot-following-enemy", 7, 2);

                GameState lethalState = new GameState
                {
                    SaveVersion = VersionInfo.SaveVersion,
                    Mode = GameMode.Combat,
                    Depth = 1,
                    Seed = 920,
                    ReducedMotion = false,
                    Party = new List<PartyMember>
                    {
                        new PartyMember
                        {
                            Id = survivingHero.Id,
                            Name = survivingHero.Name,
                            Hp = survivingHero.Hp,
                            MaxHp = survivingHero.MaxHp,
                            Skills = survivingHero.Skills.Clone()
                        }
                    },
                    Combat = new CombatState
                    {
                        Round = 2,
                        ActiveId = survivingHero.Id,
                        Phase = CombatPhase.ChooseAction,
                        InitiativeQueue = new List<string>
                        {
                            survivingHero.Id,
                            doomedEnemy.Id,
                            followingEnemy.Id
                        },
                        Units = new List<CombatUnit>
                        {
                            survivingHero,
                            doomedEnemy,
                            followingEnemy
                        },
                        Obstacles = new List<Point>()
                    }
                };
                SetPrivateField(game, "state", lethalState);
                SetPrivateField(game, "rng", new System.Random(920));
                InvokePrivate(game, "InvalidateControllerCaches");
                InvokePrivate(game, "BeginQueuedCombatTurn", doomedEnemy);

                Assert(doomedEnemy.Hp == 0
                    && lethalState.Combat.ActiveId == doomedEnemy.Id
                    && lethalState.Combat.Phase == CombatPhase.Resolving,
                    "lethal automatic start-turn damage keeps the fallen unit active during its contact beat");
                Assert(GetPrivateField<bool>(game, "combatAdvancePending")
                    && !GetPrivateField<bool>(game, "combatAdvanceStartsReservedTurn")
                    && GetPrivateField<string>(game, "combatResolutionLabel") == "fall",
                    "lethal start-turn damage queues a defeat hold instead of recursing immediately");
                Assert(presentationBeats.Any(beat =>
                    beat != null
                    && beat.UnitId == doomedEnemy.Id
                    && beat.Kind == CombatUnitPresentationBeatKind.Defeat),
                    "lethal start-turn damage preserves a rendered defeat pose");

                SetPrivateField(game, "combatAdvanceAt", Time.time - 0.01f);
                InvokePrivate(game, "CompletePendingCombatAdvance");
                Assert(lethalState.Combat != null
                    && lethalState.Combat.Round == 2
                    && lethalState.Combat.ActiveId == followingEnemy.Id
                    && lethalState.Combat.Phase == CombatPhase.EnemyThinking
                    && !GetPrivateField<bool>(game, "combatAdvancePending"),
                    "defeat-hold completion advances to the following queued unit without ending or double-ticking combat");
            }
            finally
            {
                InvokePrivate(game, "CancelCombatResolutionBeat", false);
                SetPrivateField(game, "state", originalState);
                SetPrivateField(game, "rng", originalRng);
                SetPrivateField(game, "aiActAt", originalAiActAt);
                InvokePrivate(game, "InvalidateControllerCaches");
                presentationBeats.Clear();
                presentationBeats.AddRange(originalPresentationBeats);
            }
        }

        private static void AssertQuestBoardDialogue(AshenHallsGame game, GameState state)
        {
            MapObject board = state.Map.Objects.Find(obj => obj != null && obj.Type == ObjectType.QuestBoard);
            Assert(board != null, "Midgaard quest board exists in normal sewer-slice play");
            Assert(TryFindAdjacentProbeTile(game, state, board, out int standX, out int standY), "quest board has a reachable talk position");
            state.PlayerX = standX;
            state.PlayerY = standY;
            InvokePrivate(game, "UseNearbyExploreObject");
            InvokePrivate(game, "LateUpdate");
            Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Dialogue, "normal Midgaard quest board opens dialogue instead of prototype-blocking");
            Assert(GetPrivateField<string>(game, "dialogueTitle").IndexOf("Quest Board", StringComparison.OrdinalIgnoreCase) >= 0, "quest board dialogue has the expected title");
            string body = GetPrivateField<string>(game, "dialogueBody");
            Assert(body.IndexOf("Halvard", StringComparison.OrdinalIgnoreCase) >= 0 && body.IndexOf("services", StringComparison.OrdinalIgnoreCase) >= 0, "production quest board lists current sewer work and useful city services");
            Assert(body.IndexOf("Lamp Round", StringComparison.OrdinalIgnoreCase) < 0 && body.IndexOf("Gate Survey", StringComparison.OrdinalIgnoreCase) < 0, "production quest board hides prototype-only errands");
            DialogueScreen dialogue = GetPrivateField<DialogueScreen>(game, "dialogueScreen");
            Assert(dialogue != null && dialogue.IsInteractiveAndVisible && dialogue.HasRenderableGeometry, "quest-board interaction presents an interactive dialogue popup");
            dialogue.InvokeContinueForTest();
            InvokePrivate(game, "LateUpdate");
            Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") != UiOverlay.Dialogue, "dialogue Continue closes the NPC popup");
            Assert(InvokePrivate<bool>(game, "IsBoardPointerSuppressed"), "closing NPC dialogue prevents click-through into exploration");

            string longSpeech = "The west road is open, but the shrine stones only mark the first safe mile. "
                + "Carry food, keep one elixir, and turn back when market drums answer from beyond the wall. "
                + "If the quarry lights go dark, return to Midgaard and report it. "
                + "The watch can hold a gate, but it cannot pull a proud party out of every ravine on the Old Road.";
            InvokePrivate(game, "ShowDialogue", "Gate Captain", "Brann", longSpeech, ObjectType.GateCaptain, new Color(0.55f, 0.62f, 0.60f, 1f));
            InvokePrivate(game, "LateUpdate");
            dialogue = GetPrivateField<DialogueScreen>(game, "dialogueScreen");
            string[] pages = GetPrivateField<string[]>(game, "dialoguePages");
            Assert(dialogue != null && dialogue.HasPortraitArt, "NPC conversation renders the approved portrait atlas instead of initials");
            Assert(pages.Length >= 2, "long NPC conversation is split into readable pages");
            dialogue.InvokeContinueForTest();
            InvokePrivate(game, "LateUpdate");
            Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Dialogue, "Next advances a conversation without closing it");
            Assert(GetPrivateField<int>(game, "dialoguePageIndex") == 1, "dialogue advances exactly one page");
            for (int pageGuard = 0; pageGuard < pages.Length + 2 && InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Dialogue; pageGuard++)
            {
                dialogue.InvokeContinueForTest();
                InvokePrivate(game, "LateUpdate");
            }
            Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") != UiOverlay.Dialogue, "final conversation page closes cleanly");
            Assert(InvokePrivate<bool>(game, "IsBoardPointerSuppressed"), "final conversation dismissal still protects the map from click-through");

            string selectedChoice = "";
            DialogueChoiceView[] choices =
            {
                new DialogueChoiceView { Id = "duty", Label = "Current duty", Enabled = true },
                new DialogueChoiceView { Id = "roads", Label = "The roads", Enabled = true },
                new DialogueChoiceView { Id = "leave", Label = "Leave", Enabled = true }
            };
            Action<string> choose = id => selectedChoice = id;
            InvokePrivate(game, "ShowDialogueChoices", "Gate Captain", "Brann", "What do you need?", ObjectType.GateCaptain, Color.gray, choices, choose);
            InvokePrivate(game, "LateUpdate");
            dialogue = GetPrivateField<DialogueScreen>(game, "dialogueScreen");
            Assert(dialogue != null && dialogue.IsInteractiveAndVisible, "choice conversation owns the modal layer");
            Assert(dialogue.VisibleChoiceCountForTest == 3, "choice conversation renders each response button");
            dialogue.InvokeChoiceForTest(1);
            InvokePrivate(game, "LateUpdate");
            Assert(selectedChoice == "roads", "dialogue choice click resolves the selected response id");
            Assert(dialogue.VisibleChoiceCountForTest == 0, "resolved choices cannot be clicked twice");
            Assert(InvokePrivate<bool>(game, "IsBoardPointerSuppressed"), "dialogue choice click cannot leak into exploration");
            InvokePrivate(game, "CloseDialogue");
            InvokePrivate(game, "LateUpdate");

            InvokePrivate(game, "VisitMarketClerk");
            InvokePrivate(game, "LateUpdate");
            dialogue = GetPrivateField<DialogueScreen>(game, "dialogueScreen");
            Assert(dialogue.VisibleChoiceCountForTest == 3, "production Nessa conversation exposes three hinted topics");
            dialogue.InvokeChoiceForTest(0);
            InvokePrivate(game, "LateUpdate");
            Assert(dialogue.VisibleChoiceCountForTest == 0, "NPC answer gives the response its own uncluttered page");
            AdvanceDialogueResponseToChoices(game, dialogue, 3, "Back to topics restores the familiar topic deck");
            dialogue.MoveChoiceSelection(1);
            Assert(dialogue.SelectedChoiceIndexForTest == 1, "dialogue selection moves through the vertical response list");
            dialogue.InvokeSelectedChoice();
            InvokePrivate(game, "LateUpdate");
            Assert(dialogue.VisibleChoiceCountForTest == 0, "keyboard-selected topic opens the same uncluttered response page");
            Assert(InvokePrivate<bool>(game, "ReturnDialogueToTopics"), "response can step back to the NPC greeting");
            InvokePrivate(game, "LateUpdate");
            Assert(dialogue.VisibleChoiceCountForTest == 3, "returning from a keyboard-selected response restores topics");
            dialogue.InvokeContinueForTest();
            InvokePrivate(game, "LateUpdate");
            Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") != UiOverlay.Dialogue, "explicit Leave button closes the conversation cleanly");

            AssertProductionTopicConversation(game, "VisitCityCourier", "Tovan");
            AssertProductionTopicConversation(game, "VisitNoviceHealer", "Sera");
            AssertProductionTopicConversation(game, "VisitWoundedTraveler", "Edda");
            AssertProductionTopicConversation(game, "VisitStableHand", "Pell");
            AssertKateServiceConversation(game, state);
            AssertProductionTopicConversation(game, "VisitRoyalHerald", "Vann");
            AssertProductionTopicConversation(game, "VisitTownGuard", "Rusk");
            AssertProductionTopicConversation(game, "VisitMidgaardTavern", "Orren");

            AssertExplicitServiceConversation(game, state, "VisitMidgaardArmorer", StoryFlags.MidgaardBasicArmorBought, 28, "Borin");
            AssertExplicitServiceConversation(game, state, "VisitWeaponVendor", StoryFlags.MidgaardBasicWeaponBought, 32, "Tessa");
            AssertWeaponEnchanterConversation(game, state);
            AssertPrototypeDialogueScaffold(game, state);
        }

        private static void AssertProductionTopicConversation(AshenHallsGame game, string visitMethod, string label, params object[] args)
        {
            InvokePrivate(game, visitMethod, args);
            InvokePrivate(game, "LateUpdate");
            DialogueScreen dialogue = GetPrivateField<DialogueScreen>(game, "dialogueScreen");
            Assert(dialogue != null && dialogue.VisibleChoiceCountForTest == 3, label + " exposes three authored topics");
            dialogue.InvokeChoiceForTest(0);
            InvokePrivate(game, "LateUpdate");
            Assert(dialogue.VisibleChoiceCountForTest == 0, label + " answer replaces the topic list while it is being read");
            AdvanceDialogueResponseToChoices(game, dialogue, 3, label + " returns to its authored topics");
            dialogue.InvokeContinueForTest();
            InvokePrivate(game, "LateUpdate");
            Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") != UiOverlay.Dialogue, label + " conversation leaves cleanly");
        }

        private static void AdvanceDialogueResponseToChoices(
            AshenHallsGame game,
            DialogueScreen dialogue,
            int expectedChoiceCount,
            string assertion)
        {
            string[] responsePages = GetPrivateField<string[]>(game, "dialoguePages") ?? Array.Empty<string>();
            int guardLimit = Math.Max(2, responsePages.Length + 1);
            for (int responsePageGuard = 0;
                responsePageGuard < guardLimit
                    && dialogue.VisibleChoiceCountForTest == 0
                    && InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Dialogue;
                responsePageGuard++)
            {
                dialogue.InvokeContinueForTest();
                InvokePrivate(game, "LateUpdate");
            }
            Assert(dialogue.VisibleChoiceCountForTest == expectedChoiceCount, assertion);
        }

        private static void AssertPrototypeDialogueScaffold(AshenHallsGame game, GameState state)
        {
            string previousContentSet = GetPrivateField<string>(game, "activeContentSet");
            string previousStateContentSet = state.ContentSetId;
            List<string> previousFlags = state.StoryFlags;
            List<LogEntry> previousLog = state.Log;
            int previousGold = state.Gold;
            int previousSupplies = state.Supplies;
            int previousElixirs = state.Elixirs;
            int[] previousHp = state.Party.Select(member => member.Hp).ToArray();
            int[] previousMana = state.Party.Select(member => member.Mana).ToArray();
            string previousBanner = GetPrivateField<string>(game, "bannerText");
            float previousBannerUntil = GetPrivateField<float>(game, "bannerUntil");

            try
            {
                state.StoryFlags = new List<string>(previousFlags);
                state.StoryFlags.RemoveAll(flag =>
                    flag == StoryFlags.MidgaardLampRoundStarted
                    || flag == StoryFlags.MidgaardLampRoundComplete
                    || flag == StoryFlags.MidgaardGateSurveyStarted
                    || flag == StoryFlags.MidgaardGateSurveyComplete);
                state.Log = new List<LogEntry>(previousLog);
                state.ContentSetId = ContentSetCatalog.FullPrototype;
                SetPrivateField(game, "activeContentSet", ContentSetCatalog.FullPrototype);

                InvokePrivate(game, "VisitTempleHealer");
                InvokePrivate(game, "LateUpdate");
                DialogueChoiceView[] choices = GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices");
                Assert(state.StoryFlags.Contains(StoryFlags.MidgaardLampRoundStarted), "full-prototype Mira still starts the optional lamp round");
                Assert(choices.Any(choice => choice != null && choice.Id == "lamp"), "full-prototype Mira still exposes lamp-round dialogue");
                InvokePrivate(game, "CloseDialogue");
                InvokePrivate(game, "LateUpdate");

                InvokePrivate(game, "VisitGateCaptain");
                InvokePrivate(game, "LateUpdate");
                choices = GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices");
                Assert(state.StoryFlags.Contains(StoryFlags.MidgaardGateSurveyStarted), "full-prototype Brann still starts the optional gate survey");
                Assert(choices.Any(choice => choice != null && choice.Id == "survey"), "full-prototype Brann still exposes gate-survey dialogue");
            }
            finally
            {
                if (InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Dialogue)
                {
                    InvokePrivate(game, "CloseDialogue");
                    InvokePrivate(game, "LateUpdate");
                }

                SetPrivateField(game, "activeContentSet", previousContentSet);
                state.ContentSetId = previousStateContentSet;
                state.StoryFlags = previousFlags;
                state.Log = previousLog;
                state.Gold = previousGold;
                state.Supplies = previousSupplies;
                state.Elixirs = previousElixirs;
                for (int index = 0; index < state.Party.Count && index < previousHp.Length; index++)
                {
                    state.Party[index].Hp = previousHp[index];
                    state.Party[index].Mana = previousMana[index];
                }
                SetPrivateField(game, "bannerText", previousBanner);
                SetPrivateField(game, "bannerUntil", previousBannerUntil);
            }
        }

        private static void AssertKateServiceConversation(AshenHallsGame game, GameState state)
        {
            state.StoryFlags.Remove(StoryFlags.MidgaardKateBundleBought);
            state.StoryFlags.Remove(StoryFlags.MidgaardProvisionBundleBought);
            state.Gold = 80;
            int goldBefore = state.Gold;
            int suppliesBefore = state.Supplies;

            InvokePrivate(game, "VisitKatesDiner", false);
            InvokePrivate(game, "LateUpdate");
            DialogueScreen dialogue = GetPrivateField<DialogueScreen>(game, "dialogueScreen");
            DialogueChoiceView[] choices = GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices");
            Assert(dialogue != null && dialogue.VisibleChoiceCountForTest == 4, "Kate opens a four-choice service conversation");
            Assert(choices.Length == 4 && choices[0].Enabled, "Kate exposes an explicit affordable purchase");
            Assert(state.Gold == goldBefore && state.Supplies == suppliesBefore, "talking to Kate never buys provisions automatically");

            dialogue.InvokeChoiceForTest(1);
            InvokePrivate(game, "LateUpdate");
            Assert(state.Gold == goldBefore && state.Supplies == suppliesBefore, "Kate's advice does not spend gold");
            Assert(dialogue.VisibleChoiceCountForTest == 0, "Kate's advice gets an uncluttered response page");
            AdvanceDialogueResponseToChoices(game, dialogue, 4, "Kate's response returns to the service list");

            dialogue.InvokeChoiceForTest(0);
            InvokePrivate(game, "LateUpdate");
            Assert(state.Gold == goldBefore - 12, "Kate spends the advertised price only after confirmation");
            Assert(state.Supplies == suppliesBefore + 4, "Kate grants the advertised provision bundle");
            Assert(state.StoryFlags.Contains(StoryFlags.MidgaardKateBundleBought)
                && state.StoryFlags.Contains(StoryFlags.MidgaardProvisionBundleBought), "Kate and Lute share one introductory bundle state");
            choices = GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices");
            Assert(choices.Length == 4 && !choices[0].Enabled, "completed Kate bundle disables at both service endpoints");
            InvokePrivate(game, "CloseDialogue");
            InvokePrivate(game, "LateUpdate");

            int goldAfter = state.Gold;
            int suppliesAfter = state.Supplies;
            InvokePrivate(game, "VisitKatesDiner", true);
            InvokePrivate(game, "LateUpdate");
            choices = GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices");
            Assert(choices.Length == 4 && !choices[0].Enabled, "Lute recognizes Kate's completed starter bundle");
            Assert(state.Gold == goldAfter && state.Supplies == suppliesAfter, "visiting the second food endpoint cannot duplicate the reward");
            InvokePrivate(game, "CloseDialogue");
            InvokePrivate(game, "LateUpdate");
        }

        private static void AssertWeaponEnchanterConversation(AshenHallsGame game, GameState state)
        {
            InvokePrivate(game, "DismissLootPopupSilently");
            if (InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Dialogue)
            {
                InvokePrivate(game, "CloseDialogue");
                InvokePrivate(game, "LateUpdate");
            }

            InvokePrivate(game, "EnsureInventoryList");
            InvokePrivate(game, "EnsurePartyInventoryIds");
            InvokePrivate(game, "EnsureInventoryEquipmentLinks");
            Assert(state.Party != null && state.Party.Count >= 3, "Maud has at least two distinct party weapons to test");
            PartyMember[] starterTargets = state.Party
                .Where(member =>
                    member != null
                    && !string.IsNullOrWhiteSpace(member.WeaponName)
                    && !state.Inventory.Any(item =>
                        item != null
                        && string.Equals(item.EquippedById, member.Id, StringComparison.Ordinal)
                        && InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form)))
                .Take(2)
                .ToArray();
            Assert(starterTargets.Length == 2, "Maud can create inventory records for two starter weapons");

            PartyMember temporaryTarget = starterTargets[0];
            PartyMember permanentTarget = starterTargets[1];
            int temporaryTargetIndex = state.Party.IndexOf(temporaryTarget);
            int permanentTargetIndex = state.Party.IndexOf(permanentTarget);
            string temporaryBaseName = temporaryTarget.WeaponName;
            string temporaryBaseType = string.IsNullOrWhiteSpace(temporaryTarget.WeaponDamageType)
                ? "physical"
                : temporaryTarget.WeaponDamageType;
            string permanentBaseName = permanentTarget.WeaponName;
            int inventoryBefore = state.Inventory.Count;
            state.StoryFlags.Remove(StoryFlags.MidgaardWeaponEnchanted);
            state.Gold = 240;
            int goldBefore = state.Gold;

            InvokePrivate(game, "VisitWeaponEnchanter");
            InvokePrivate(game, "LateUpdate");
            DialogueScreen dialogue = GetPrivateField<DialogueScreen>(game, "dialogueScreen");
            DialogueChoiceView[] choices = GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices");
            Assert(dialogue != null && dialogue.VisibleChoiceCountForTest == 3, "Maud opens a three-choice temporary, permanent, and affinity service");
            Assert(choices.Length == 3
                && choices[0].Id == "temporary"
                && choices[1].Id == "permanent"
                && choices[2].Id == "affinity", "Maud's opening service choices are explicit");
            Assert(choices[0].Enabled && choices[1].Enabled, "Maud enables both affordable enchantment services");
            Assert(state.Gold == goldBefore, "opening Maud's service never spends gold");

            dialogue.InvokeChoiceForTest(2);
            InvokePrivate(game, "LateUpdate");
            Assert(state.Gold == goldBefore, "Maud's affinity information costs nothing");
            Assert(dialogue.VisibleChoiceCountForTest == 0, "Maud's affinity information uses an uncluttered response page");
            AdvanceDialogueResponseToChoices(game, dialogue, 3, "Maud's affinity information returns to the service menu");

            dialogue.InvokeChoiceForTest(0);
            InvokePrivate(game, "LateUpdate");
            choices = GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices");
            Assert(dialogue.VisibleChoiceCountForTest == Math.Min(4, state.Party.Count), "temporary enchantment opens one target choice per party weapon");
            Assert(choices[temporaryTargetIndex].Id == temporaryTargetIndex.ToString()
                && choices[temporaryTargetIndex].Enabled, "temporary enchantment enables the selected starter weapon");

            dialogue.InvokeChoiceForTest(temporaryTargetIndex);
            InvokePrivate(game, "LateUpdate");
            choices = GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices");
            Assert(dialogue.VisibleChoiceCountForTest == 4
                && string.Join(",", choices.Select(choice => choice.Id)) == "fire,ice,storm,radiance", "temporary enchantment opens all four affinity choices");

            dialogue.InvokeChoiceForTest(0);
            InvokePrivate(game, "LateUpdate");
            Assert(state.Gold == goldBefore - WeaponEnchantmentRules.TemporaryCost, "temporary fire spends exactly 18 gold after affinity confirmation");
            Assert(state.Inventory.Count == inventoryBefore + 1, "temporary fire creates one inventory item for the starter weapon");
            InventoryItem temporaryItem = state.Inventory.Single(item =>
                item != null
                && string.Equals(item.EquippedById, temporaryTarget.Id, StringComparison.Ordinal)
                && InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form));
            Assert(temporaryItem.DisplayName == "fiery " + temporaryBaseName
                && temporaryItem.Trait.StartsWith("fiery", StringComparison.Ordinal)
                && temporaryItem.DamageType == "fire", "temporary fire changes the linked item's text and affinity");
            Assert(temporaryItem.TemporaryEnchantmentId == "fire"
                && temporaryItem.TemporaryEnchantmentVictoriesRemaining == 3, "temporary fire records its three-victory duration");
            Assert(temporaryTarget.WeaponName == temporaryItem.DisplayName
                && temporaryTarget.WeaponDamageType == "fire", "temporary fire synchronizes item text and affinity to the party member");
            Assert(state.StoryFlags.Contains(StoryFlags.MidgaardWeaponEnchanted), "Maud records completed enchantment work");

            GameMode modeBeforeCombatProbe = state.Mode;
            CombatState combatBeforeProbe = state.Combat;
            bool labModeBeforeProbe = GetPrivateField<bool>(game, "betaLabMode");
            InvokePrivate(game, "StartCombat", "patrol");
            CombatUnit temporaryCombatUnit = state.Combat.Units.Single(unit =>
                unit != null && string.Equals(unit.Id, temporaryTarget.Id, StringComparison.Ordinal));
            Assert(temporaryCombatUnit.WeaponName == temporaryItem.DisplayName
                && temporaryCombatUnit.DamageType == "fire", "combat copies the enchanted weapon text and fire affinity");
            state.Combat = combatBeforeProbe;
            state.Mode = modeBeforeCombatProbe;
            SetPrivateField(game, "betaLabMode", labModeBeforeProbe);
            InvokePrivate(game, "InvalidateCombatController");

            InvokePrivate(game, "VisitWeaponEnchanter");
            InvokePrivate(game, "LateUpdate");
            dialogue = GetPrivateField<DialogueScreen>(game, "dialogueScreen");
            choices = GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices");
            Assert(dialogue.VisibleChoiceCountForTest == 3
                && choices[0].Enabled
                && choices[1].Enabled, "Maud remains available after a temporary enchantment");

            dialogue.InvokeChoiceForTest(1);
            InvokePrivate(game, "LateUpdate");
            choices = GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices");
            Assert(choices[permanentTargetIndex].Id == permanentTargetIndex.ToString()
                && choices[permanentTargetIndex].Enabled, "permanent enchantment enables a different starter weapon");

            dialogue.InvokeChoiceForTest(permanentTargetIndex);
            InvokePrivate(game, "LateUpdate");
            choices = GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices");
            Assert(dialogue.VisibleChoiceCountForTest == 4 && choices[1].Id == "ice", "permanent enchantment reaches the ice affinity choice");

            dialogue.InvokeChoiceForTest(1);
            InvokePrivate(game, "LateUpdate");
            Assert(state.Gold == goldBefore
                - WeaponEnchantmentRules.TemporaryCost
                - WeaponEnchantmentRules.PermanentCost, "permanent ice spends exactly 90 additional gold");
            Assert(state.Inventory.Count == inventoryBefore + 2, "permanent ice creates one linked item for the other starter weapon");
            InventoryItem permanentItem = state.Inventory.Single(item =>
                item != null
                && string.Equals(item.EquippedById, permanentTarget.Id, StringComparison.Ordinal)
                && InventoryEquipmentRules.IsWeaponSlot(item.Slot, item.Form));
            Assert(permanentItem.DisplayName == "frostbound " + permanentBaseName
                && permanentItem.DamageType == "cold"
                && permanentItem.PermanentEnchantmentId == "ice", "permanent ice changes and persists the second weapon's text and affinity");
            Assert(permanentTarget.WeaponName == permanentItem.DisplayName
                && permanentTarget.WeaponDamageType == "cold", "permanent ice synchronizes the item to its party member");

            InvokePrivate(game, "CloseDialogue");
            InvokePrivate(game, "LateUpdate");
            InvokePrivate(game, "VisitWeaponEnchanter");
            InvokePrivate(game, "LateUpdate");
            choices = GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices");
            Assert(choices.Length == 3
                && choices[0].Enabled
                && choices[1].Enabled, "Maud's temporary and permanent services remain repeatable after completed work");
            InvokePrivate(game, "CloseDialogue");
            InvokePrivate(game, "LateUpdate");

            InvokePrivate(game, "AdvanceTemporaryWeaponEnchantmentsAfterVictory");
            Assert(temporaryItem.TemporaryEnchantmentVictoriesRemaining == 2
                && temporaryTarget.WeaponName == temporaryItem.DisplayName, "first victory advances and synchronizes temporary fire");
            InvokePrivate(game, "AdvanceTemporaryWeaponEnchantmentsAfterVictory");
            Assert(temporaryItem.TemporaryEnchantmentVictoriesRemaining == 1
                && temporaryTarget.WeaponDamageType == "fire", "second victory leaves temporary fire active");
            InvokePrivate(game, "AdvanceTemporaryWeaponEnchantmentsAfterVictory");
            Assert(temporaryItem.TemporaryEnchantmentVictoriesRemaining == 0
                && string.IsNullOrEmpty(temporaryItem.TemporaryEnchantmentId), "third victory expires temporary fire");
            Assert(temporaryItem.DisplayName == temporaryBaseName
                && temporaryItem.DamageType == temporaryBaseType
                && temporaryTarget.WeaponName == temporaryBaseName
                && temporaryTarget.WeaponDamageType == temporaryBaseType, "expired temporary fire restores and synchronizes the starter weapon");
            Assert(permanentItem.DisplayName == "frostbound " + permanentBaseName
                && permanentItem.DamageType == "cold"
                && permanentItem.PermanentEnchantmentId == "ice"
                && permanentTarget.WeaponName == permanentItem.DisplayName
                && permanentTarget.WeaponDamageType == "cold", "three victories leave permanent ice unchanged");
        }

        private static void AssertExplicitServiceConversation(
            AshenHallsGame game,
            GameState state,
            string visitMethod,
            string completionFlag,
            int price,
            string label)
        {
            InvokePrivate(game, "DismissLootPopupSilently");
            if (InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Dialogue) InvokePrivate(game, "CloseDialogue");
            state.Gold = 240;
            int goldBefore = state.Gold;

            InvokePrivate(game, visitMethod);
            InvokePrivate(game, "LateUpdate");
            DialogueScreen dialogue = GetPrivateField<DialogueScreen>(game, "dialogueScreen");
            DialogueChoiceView[] choices = GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices");
            Assert(dialogue != null && dialogue.VisibleChoiceCountForTest == 3, label + " service opens as a three-choice conversation");
            Assert(state.Gold == goldBefore, label + " does not spend gold merely by opening the service");
            Assert(choices.Length == 3 && choices[0].Enabled, label + " enables the explicitly priced service when affordable");

            dialogue.InvokeChoiceForTest(1);
            InvokePrivate(game, "LateUpdate");
            Assert(state.Gold == goldBefore, label + " information topic does not trigger a purchase");
            Assert(dialogue.VisibleChoiceCountForTest == 0, label + " information answer uses an uncluttered response page");
            AdvanceDialogueResponseToChoices(game, dialogue, 3, label + " response returns to the service menu");

            dialogue.InvokeChoiceForTest(0);
            InvokePrivate(game, "LateUpdate");
            Assert(state.Gold == goldBefore - price, label + " spends the exact advertised price only after confirmation");
            Assert(state.StoryFlags != null && state.StoryFlags.Contains(completionFlag), label + " records its one-time completion flag");

            if (InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Dialogue)
            {
                InvokePrivate(game, "CloseDialogue");
                InvokePrivate(game, "LateUpdate");
            }
            InvokePrivate(game, "DismissLootPopupSilently");
            InvokePrivate(game, "LateUpdate");

            int goldAfter = state.Gold;
            InvokePrivate(game, visitMethod);
            InvokePrivate(game, "LateUpdate");
            choices = GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices");
            Assert(choices.Length == 3 && !choices[0].Enabled, label + " disables the completed one-time service");
            InvokePrivate(game, "CloseDialogue");
            InvokePrivate(game, "LateUpdate");
            Assert(state.Gold == goldAfter, label + " cannot charge twice through the disabled service choice");
            InvokePrivate(game, "CloseDialogue");
            InvokePrivate(game, "LateUpdate");
        }

        private static bool AudioClipsDiffer(AudioClip left, AudioClip right)
        {
            if (left == null || right == null) return false;
            int count = Math.Min(left.samples, right.samples);
            if (count <= 0) return false;
            float[] a = new float[count];
            float[] b = new float[count];
            if (!left.GetData(a, 0) || !right.GetData(b, 0))
            {
                return left != right && !string.Equals(left.name, right.name, StringComparison.Ordinal);
            }
            double difference = 0d;
            for (int i = 0; i < count; i += 11) difference += Math.Abs(a[i] - b[i]);
            return difference > 0.25d;
        }

        private static bool AudioClipHasHealthyHeadroom(AudioClip clip)
        {
            if (clip == null || clip.samples < 32) return false;
            float[] samples = new float[clip.samples * Math.Max(1, clip.channels)];
            if (!clip.GetData(samples, 0)) return false;
            double energy = 0d;
            float peak = 0f;
            for (int i = 0; i < samples.Length; i++)
            {
                float sample = samples[i];
                if (float.IsNaN(sample) || float.IsInfinity(sample)) return false;
                float magnitude = Math.Abs(sample);
                peak = Math.Max(peak, magnitude);
                energy += sample * sample;
            }
            double rms = Math.Sqrt(energy / Math.Max(1, samples.Length));
            return peak >= 0.035f && peak <= 0.985f && rms >= 0.004d && rms <= 0.55d;
        }

        private static void AssertRegionalRouteCircuit(AshenHallsGame game, GameState state)
        {
            string[] expectedZones =
            {
                "old-quarry",
                "gloam-courts",
                "glass-warrens",
                "green-shrine-road",
                "dusk-market",
                "ash-fen",
                "salt-cisterns",
                "red-gate"
            };
            HashSet<string> routedZones = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> reachableRoutedZones = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            bool[,] reachable = ExplorationTraversalRules.ReachableMask(state.Map, state.PlayerX, state.PlayerY);

            for (int y = 0; y < state.Map.Height; y++)
            for (int x = 0; x < state.Map.Width; x++)
            {
                ExplorationCellRole roles = ExplorationSurfaceRules.RolesAt(state.Map, x, y);
                if (!ExplorationSurfaceRules.IsPath(roles)) continue;
                WorldZone zone = InvokePrivate<WorldZone>(game, "ZoneFor", x, y, state.Map, state.Depth);
                if (zone == null || string.IsNullOrEmpty(zone.Id)) continue;
                routedZones.Add(zone.Id);
                if (reachable[x, y]) reachableRoutedZones.Add(zone.Id);
            }

            foreach (string zoneId in expectedZones)
            {
                Assert(routedZones.Contains(zoneId), $"regional circuit lays a semantic route through {zoneId}");
                Assert(reachableRoutedZones.Contains(zoneId), $"regional circuit keeps {zoneId} reachable from Midgaard");
            }

            WorldMapJunction[] junctions = WorldMapGenerationRules.RegionalJunctions(state.Map.Width, state.Map.Height, state.Map.StartX, state.Map.StartY);
            Assert(junctions.Length == 8, "regional circuit exposes eight named junctions");
            foreach (WorldMapJunction junction in junctions)
            {
                ExplorationCellRole roles = ExplorationSurfaceRules.RolesAt(state.Map, junction.X, junction.Y);
                Assert((roles & (ExplorationCellRole.Road | ExplorationCellRole.Clearing)) == (ExplorationCellRole.Road | ExplorationCellRole.Clearing), $"{junction.Name} owns a road clearing marker");
                Assert(reachable[junction.X, junction.Y], $"{junction.Name} is reachable from the exploration start");
                Assert(!MidgaardInteriorRules.IsReservedCell(state.Map, junction.X, junction.Y), $"{junction.Name} stays outside the embedded room reservations");
            }

            WorldMapSite[] sites = WorldMapGenerationRules.RegionalSites(
                state.Map.Width,
                state.Map.Height,
                state.Map.StartX,
                state.Map.StartY);
            Assert(sites.Length == 8, "expanded world exposes one authored site in every outer zone");
            foreach (WorldMapSite site in sites)
            {
                MapObject landmark = state.Map.FindObjectById("regional-site:" + site.Id);
                Assert(landmark != null && landmark.Type == site.Type, $"{site.Name} has its stable authored landmark");
                Assert(ExplorationTraversalRules.CanReachObject(reachable, state.Map, landmark), $"{site.Name} has a reachable interaction approach");
                ExplorationCellRole roles = ExplorationSurfaceRules.RolesAt(state.Map, site.X, site.Y);
                Assert((roles & (ExplorationCellRole.Room | ExplorationCellRole.Clearing)) == (ExplorationCellRole.Room | ExplorationCellRole.Clearing), $"{site.Name} owns a room-sized clearing");
                Assert(!MidgaardInteriorRules.IsReservedCell(state.Map, site.X, site.Y), $"{site.Name} stays clear of Midgaard's embedded rooms");
                Assert(InvokePrivate<string>(game, "ObjectName", landmark) == site.Name, $"{site.Name} publishes its authored map identity");
            }
        }

        private static void AssertExpandedMapSeedSweep(AshenHallsGame game)
        {
            int[] seeds = { 17, 101, 777, 15151, 51510, 93017 };
            string[] expectedZones =
            {
                "old-quarry",
                "gloam-courts",
                "glass-warrens",
                "green-shrine-road",
                "dusk-market",
                "ash-fen",
                "salt-cisterns",
                "red-gate"
            };

            foreach (int seed in seeds)
            {
                MapData map = InvokePrivate<MapData>(game, "GenerateMap", 1, seed);
                Assert(map != null, $"seed {seed} generates a map");
                Assert(map.Width == WorldMapGenerationRules.Width && map.Height == WorldMapGenerationRules.Height, $"seed {seed} keeps expanded dimensions");
                Assert(ExplorationSurfaceRules.HasValidGrid(map), $"seed {seed} keeps complete semantic surfaces");
                Point start = NearestStandableMapPoint(map, map.StartX, map.StartY);
                Assert(start != null, $"seed {seed} has a standable Midgaard start");
                bool[,] reachable = ExplorationTraversalRules.ReachableMask(map, start.X, start.Y);
                HashSet<string> reachableRouteZones = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                for (int y = 0; y < map.Height; y++)
                for (int x = 0; x < map.Width; x++)
                {
                    if (!reachable[x, y] || !ExplorationSurfaceRules.IsPath(ExplorationSurfaceRules.RolesAt(map, x, y))) continue;
                    WorldZone zone = InvokePrivate<WorldZone>(game, "ZoneFor", x, y, map, map.Depth);
                    if (zone != null && !string.IsNullOrEmpty(zone.Id)) reachableRouteZones.Add(zone.Id);
                }

                foreach (string zoneId in expectedZones)
                {
                    Assert(reachableRouteZones.Contains(zoneId), $"seed {seed} keeps the {zoneId} route connected");
                }
                foreach (WorldMapJunction junction in WorldMapGenerationRules.RegionalJunctions(
                    map.Width,
                    map.Height,
                    map.StartX,
                    map.StartY))
                {
                    Assert(
                        reachable[junction.X, junction.Y],
                        $"seed {seed} keeps {junction.Name} reachable from Midgaard");
                }
                foreach (WorldMapSite site in WorldMapGenerationRules.RegionalSites(
                    map.Width,
                    map.Height,
                    map.StartX,
                    map.StartY))
                {
                    MapObject landmark = map.FindObjectById("regional-site:" + site.Id);
                    Assert(landmark != null && landmark.Type == site.Type, $"seed {seed} preserves {site.Name}'s stable landmark");
                    Assert(ExplorationTraversalRules.CanReachObject(reachable, map, landmark), $"seed {seed} keeps {site.Name} reachable from Midgaard");
                    Assert(!MidgaardInteriorRules.IsReservedCell(map, site.X, site.Y), $"seed {seed} keeps {site.Name} outside room reservations");
                }
            }
        }

        private static Point NearestStandableMapPoint(MapData map, int originX, int originY)
        {
            Point best = null;
            int bestDistance = int.MaxValue;
            for (int y = 0; y < map.Height; y++)
            for (int x = 0; x < map.Width; x++)
            {
                if (!ExplorationTraversalRules.IsStandable(map, x, y)) continue;
                int distance = Math.Abs(x - originX) + Math.Abs(y - originY);
                if (distance >= bestDistance) continue;
                bestDistance = distance;
                best = new Point(x, y);
            }
            return best;
        }

        private static void AssertRegionalWayfinding(AshenHallsGame game, GameState state, List<Point> reachable)
        {
            Assert(InvokePrivate<bool>(game, "ShouldUseMidgaardWayfinding"), "fresh party receives Midgaard wayfinding inside the city");
            int left = InvokePrivate<int>(game, "MidgaardLeft", state.Map);
            int right = InvokePrivate<int>(game, "MidgaardRight", state.Map);
            int top = InvokePrivate<int>(game, "MidgaardTop", state.Map);
            int bottom = InvokePrivate<int>(game, "MidgaardBottom", state.Map);
            Point regionalProbe = reachable.FirstOrDefault(point => point.X < left - 6
                || point.X > right + 6
                || point.Y < top - 4
                || point.Y > bottom + 4);
            Assert(regionalProbe != null, "expanded map exposes a reachable point beyond the Midgaard guidance approaches");

            int originalX = state.PlayerX;
            int originalY = state.PlayerY;
            state.PlayerX = regionalProbe.X;
            state.PlayerY = regionalProbe.Y;
            Assert(!InvokePrivate<bool>(game, "ShouldUseMidgaardWayfinding"), "regional travel releases the city-only tracker");
            string waypoint = InvokePrivate<string>(game, "ExploreWaypointLine");
            Assert((waypoint.StartsWith("W / Up | ", StringComparison.Ordinal)
                    || waypoint.StartsWith("S / Down | ", StringComparison.Ordinal)
                    || waypoint.StartsWith("A / Left | ", StringComparison.Ordinal)
                    || waypoint.StartsWith("D / Right | ", StringComparison.Ordinal))
                && (waypoint.Contains(" | North | ")
                    || waypoint.Contains(" | South | ")
                    || waypoint.Contains(" | West | ")
                    || waypoint.Contains(" | East | "))
                && !waypoint.Contains("Route blocked"), "regional travel selects a reachable world landmark");

            WorldMapJunction chartProbe = WorldMapGenerationRules.RegionalJunctions(state.Map.Width, state.Map.Height, state.Map.StartX, state.Map.StartY)[0];
            string chartKey = RouteChartRules.DiscoveryKey(state.Depth, chartProbe.Id);
            bool addedChartKey = !state.DiscoveredZones.Contains(chartKey);
            if (addedChartKey) state.DiscoveredZones.Add(chartKey);
            state.PlayerX = chartProbe.X + 1;
            state.PlayerY = chartProbe.Y;
            string chartLine = InvokePrivate<string>(game, "RegionalRouteChartCompactLine");
            Assert(chartLine.Contains(chartProbe.Name) && chartLine.Contains("W 1 step"), "location readout turns a discovered junction into useful bearing and distance guidance");
            string originalWaypointKey = state.ActiveRouteWaypointKey;
            state.ActiveRouteWaypointKey = RouteChartRules.WaypointKey(state.Depth, chartProbe.Id);
            InvokePrivate(game, "InvalidateActiveRouteWaypointPath");
            IReadOnlyList<Point> waypointPath = InvokePrivate<IReadOnlyList<Point>>(game, "ActiveRouteWaypointPath");
            Assert(waypointPath.Count == 2, "selected junction builds a one-step walkable route from the adjacent probe");
            string selectedWaypointLine = InvokePrivate<string>(game, "ExploreWaypointLine");
            Assert(selectedWaypointLine.StartsWith("A / Left | Marked: " + chartProbe.Name, StringComparison.Ordinal)
                && selectedWaypointLine.Contains(" | West | 1 step"), "selected junction overrides automatic guidance with the exact westbound input");
            int chartProbeTileIndex = chartProbe.Y * state.Map.Width + chartProbe.X;
            int chartProbeTile = state.Map.Tiles[chartProbeTileIndex];
            state.Map.Tiles[chartProbeTileIndex] = 0;
            IReadOnlyList<Point> blockedWaypointPlan = InvokePrivate<IReadOnlyList<Point>>(game, "CurrentExploreGuidancePath");
            Assert(blockedWaypointPlan.Count == 0
                && InvokePrivate<bool>(game, "CurrentExploreGuidanceIsBlocked"),
                "same-map topology edits invalidate both the marked path cache and shared guidance plan");
            state.Map.Tiles[chartProbeTileIndex] = chartProbeTile;
            IReadOnlyList<Point> restoredWaypointPlan = InvokePrivate<IReadOnlyList<Point>>(game, "CurrentExploreGuidancePath");
            Assert(restoredWaypointPlan.Count == 2
                && !InvokePrivate<bool>(game, "CurrentExploreGuidanceIsBlocked"),
                "restoring a route cell rebuilds the marked map thread without manual cache repair");
            string selectedChartLine = InvokePrivate<string>(game, "RegionalRouteChartCompactLine");
            Assert(selectedChartLine.StartsWith("Waypoint: " + chartProbe.Name, StringComparison.Ordinal)
                && selectedChartLine.Contains("W 1 step"), "location readout promotes the selected waypoint above the nearest-marker fallback");
            state.ActiveRouteWaypointKey = originalWaypointKey;
            InvokePrivate(game, "InvalidateActiveRouteWaypointPath");
            if (addedChartKey) state.DiscoveredZones.Remove(chartKey);
            state.PlayerX = originalX;
            state.PlayerY = originalY;
        }

        private static void AssertMidgaardInteriors(AshenHallsGame game, GameState state)
        {
            List<MapObject> portals = state.Map.Objects.Where(MidgaardInteriorRules.IsPortal).ToList();
            Assert(portals.Count == 8, "Midgaard contains four paired interior doorways");
            Assert(portals.Select(portal => portal.Id).Distinct(StringComparer.Ordinal).Count() == portals.Count, "interior doorway identities are unique");
            Assert(MidgaardInteriorRules.BrokenPortalIds(state.Map).Count == 0, "every Midgaard interior doorway has a valid return target");
            Assert(state.Map.Objects.Count(obj => obj != null && obj.Type == ObjectType.KingHalvard) == 1, "throne room contains exactly one King Halvard NPC");
            Assert(state.Map.Objects.Count(obj => obj != null && obj.Type == ObjectType.ArmorerNpc) == 1, "merchant hall contains exactly one armorer");
            Assert(state.Map.Objects.Count(obj => obj != null && obj.Type == ObjectType.WeaponMerchantNpc) == 1, "merchant hall contains exactly one weaponsmith");
            Assert(state.Map.Objects.Count(obj => obj != null && obj.Type == ObjectType.EnchanterNpc) == 1, "merchant hall contains exactly one runesmith");

            bool[,] exteriorReachable = ExplorationTraversalRules.ReachableMask(state.Map, state.PlayerX, state.PlayerY);
            RectInt throneBounds = MidgaardInteriorRules.ThroneRoomBounds(state.Map);
            RectInt merchantBounds = MidgaardInteriorRules.MerchantHallBounds(state.Map);
            for (int y = 0; y < state.Map.Height; y++)
            for (int x = 0; x < state.Map.Width; x++)
            {
                if (!throneBounds.Contains(new Vector2Int(x, y))
                    && !merchantBounds.Contains(new Vector2Int(x, y)))
                {
                    continue;
                }
                if (state.Map.Tiles[y * state.Map.Width + x] != 1) continue;
                Assert(!exteriorReachable[x, y], $"embedded interior floor {x},{y} cannot leak into the overworld flood fill");
            }

            MapObject throne = state.Map.Objects.Single(obj => obj != null && obj.Type == ObjectType.RoyalThrone);
            MapObject throneExit = MidgaardInteriorRules.FindById(state.Map, MidgaardInteriorRules.ThroneRoomExitId);
            MapObject king = state.Map.Objects.Single(obj => obj != null && obj.Type == ObjectType.KingHalvard);
            MapObject armorer = state.Map.Objects.Single(obj => obj != null && obj.Type == ObjectType.ArmorerNpc);
            MapObject enchanter = state.Map.Objects.Single(obj => obj != null && obj.Type == ObjectType.EnchanterNpc);
            MapObject armorerExit = MidgaardInteriorRules.FindById(state.Map, MidgaardInteriorRules.ArmorerExitId);
            Assert(InvokePrivate<int>(game, "MidgaardInteriorTileAtlasIndex", throne.X, throne.Y, 1) == 2, "royal throne uses its authored dais terrain");
            Assert(InvokePrivate<int>(game, "MidgaardInteriorTileAtlasIndex", throneExit.X, throneExit.Y, 1) == 15, "throne-room doorway uses its royal threshold");
            Assert(InvokePrivate<int>(game, "MidgaardInteriorTileAtlasIndex", armorer.X, armorer.Y + 1, 1) == 17, "armorer bay uses its forge floor");
            Assert(InvokePrivate<int>(game, "MidgaardInteriorTileAtlasIndex", enchanter.X, enchanter.Y + 1, 1) == 18, "runesmith bay uses its enchantment floor");
            Assert(InvokePrivate<int>(game, "MidgaardInteriorTileAtlasIndex", armorerExit.X, armorerExit.Y, 1) == 16, "merchant doorway uses its shop threshold");

            foreach (ObjectType type in new[] { ObjectType.KingHalvard, ObjectType.ArmorerNpc, ObjectType.WeaponMerchantNpc, ObjectType.EnchanterNpc })
            {
                MapObject npc = state.Map.Objects.Single(obj => obj != null && obj.Type == type);
                WorldZone zone = InvokePrivate<WorldZone>(game, "ZoneFor", npc.X, npc.Y, state.Map, state.Depth);
                Assert(zone != null && zone.Danger == 0, type + " stands in a safe interior zone");
            }

            int oldX = state.PlayerX;
            int oldY = state.PlayerY;
            Assert(MidgaardInteriorRules.TryFindArrival(state.Map, throneExit, out Point throneLanding), "throne room has a safe landing");
            bool[,] throneReachable = ExplorationTraversalRules.ReachableMask(state.Map, throneLanding.X, throneLanding.Y);
            for (int y = 0; y < state.Map.Height; y++)
            for (int x = 0; x < state.Map.Width; x++)
            {
                if (!throneReachable[x, y]) continue;
                Assert(throneBounds.Contains(new Vector2Int(x, y)), $"throne-room flood fill cannot escape through cell {x},{y}");
            }
            state.PlayerX = throneLanding.X;
            state.PlayerY = throneLanding.Y;
            InvokePrivate(game, "RepairPlayerExplorationPosition");
            Assert(state.PlayerX == throneLanding.X && state.PlayerY == throneLanding.Y, "load repair preserves a valid position inside an isolated interior");

            MapObject kingDoor = MidgaardInteriorRules.FindById(state.Map, MidgaardInteriorRules.KingHallDoorId);
            Assert(InvokePrivate<bool>(game, "TryUseMidgaardPortal", kingDoor), "King's Hall doorway resolves");
            WorldZone throneZone = InvokePrivate<WorldZone>(game, "ZoneFor", state.PlayerX, state.PlayerY, state.Map, state.Depth);
            Assert(throneZone != null && throneZone.Id == "midgaard-throne-room", "King's Hall doorway enters the throne room");
            ExplorationHudView throneGuidance = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
            Assert(throneGuidance.WaypointLine.IndexOf("King Halvard", StringComparison.OrdinalIgnoreCase) >= 0
                && throneGuidance.WaypointLine.IndexOf("King's Hall", StringComparison.OrdinalIgnoreCase) < 0,
                "throne-room Golden Thread retargets Halvard instead of pointing back to the exterior hall");
            Assert(TryFindAdjacentProbeTile(game, state, king, out int kingStandX, out int kingStandY),
                "King Halvard has a reachable adjacent Golden Thread interaction tile");
            state.PlayerX = kingStandX;
            state.PlayerY = kingStandY;
            ExplorationHudView kingUseGuidance = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
            Assert(kingUseGuidance.WaypointLine.IndexOf("E / Space", StringComparison.OrdinalIgnoreCase) >= 0
                && kingUseGuidance.WaypointLine.IndexOf("King Halvard", StringComparison.OrdinalIgnoreCase) >= 0,
                "adjacent Halvard becomes an exact contextual-use instruction");
            IReadOnlyList<Point> kingUsePath = InvokePrivate<IReadOnlyList<Point>>(game, "CurrentExploreGuidancePath");
            Point kingUseTarget = InvokePrivate<Point>(game, "CurrentExploreGuidanceTargetPoint");
            Assert(InvokePrivate<bool>(game, "CurrentExploreGuidanceIsImmediate")
                && kingUsePath.Count == 1
                && kingUseTarget != null
                && kingUseTarget.X == king.X
                && kingUseTarget.Y == king.Y,
                "adjacent Halvard owns one immediate plan and therefore draws no travel trail");

            if (!state.StoryFlags.Contains(StoryFlags.MidgaardRatQuestGiven))
            {
                List<string> flags = new List<string>(state.StoryFlags);
                string activeStory = state.ActiveStory;
                InvokePrivate(game, "VisitKingHall");
                Assert(!state.StoryFlags.Contains(StoryFlags.MidgaardRatQuestGiven), "meeting Halvard does not silently accept the royal writ");
                Assert(!state.StoryFlags.Contains(StoryFlags.MidgaardSecondQuestGiven), "a first royal audience cannot skip ahead to the deeper-road quest");
                Assert(GetPrivateField<string>(game, "bannerText") == "Royal Audience", "a pre-contract audience is not announced as completed work");
                InvokePrivate(game, "ResolveHalvardDialogueChoice", "accept");
                Assert(state.StoryFlags.Contains(StoryFlags.MidgaardRatQuestGiven), "accepting the royal writ starts the sewer contract");
                ExplorationHudView acceptedInteriorGuidance = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
                Assert(acceptedInteriorGuidance.WaypointLine.IndexOf("Doors to Midgaard", StringComparison.OrdinalIgnoreCase) >= 0
                    && acceptedInteriorGuidance.WaypointLine.IndexOf("King Halvard", StringComparison.OrdinalIgnoreCase) < 0,
                    "accepted royal work redirects the Golden Thread through the reachable throne-room exit");
                Point acceptedExitTarget = InvokePrivate<Point>(game, "CurrentExploreGuidanceTargetPoint");
                Assert(InvokePrivate<bool>(game, "CurrentExploreGuidanceIsInteriorExit")
                    && acceptedExitTarget != null
                    && acceptedExitTarget.X == throneExit.X
                    && acceptedExitTarget.Y == throneExit.Y,
                    "interior-exit NEXT copy and map thread share the same doorway target");
                InvokePrivate(game, "CloseDialogue");
                Assert(InvokePrivate<bool>(game, "TryUseMidgaardPortal", throneExit), "guided throne-room exit resolves");
                Assert(!InvokePrivate<bool>(game, "IsMidgaardInteriorCell", state.PlayerX, state.PlayerY, state.Map, state.Depth),
                    "throne-room exit guidance returns to Midgaard streets");
                ExplorationHudView acceptedStreetGuidance = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
                Assert(acceptedStreetGuidance.WaypointLine.IndexOf("Sewer", StringComparison.OrdinalIgnoreCase) >= 0
                    && acceptedStreetGuidance.WaypointLine.IndexOf(" / ", StringComparison.OrdinalIgnoreCase) >= 0
                    && acceptedStreetGuidance.WaypointLine.IndexOf("step", StringComparison.OrdinalIgnoreCase) >= 0,
                    "leaving after Halvard's writ advances the Golden Thread to the sewer");
                state.StoryFlags = flags;
                state.ActiveStory = activeStory;
            }
            else
            {
                Assert(InvokePrivate<bool>(game, "TryUseMidgaardPortal", throneExit), "throne room exit resolves");
                Assert(!InvokePrivate<bool>(game, "IsMidgaardInteriorCell", state.PlayerX, state.PlayerY, state.Map, state.Depth),
                    "throne room exit returns to Midgaard streets");
            }
            state.PlayerX = oldX;
            state.PlayerY = oldY;
        }

        private static void AssertMidgaardGateTraversal(AshenHallsGame game, GameState state)
        {
            MapObject eastGate = state.Map.Objects.Find(obj => obj != null && obj.Type == ObjectType.EastGate);
            MapObject westGate = state.Map.Objects.Find(obj => obj != null && obj.Type == ObjectType.WestGate);
            MapObject northGate = state.Map.Objects.Find(obj => obj != null && obj.Type == ObjectType.NorthGate);
            MapObject southGate = state.Map.Objects.Find(obj => obj != null && obj.Type == ObjectType.SouthGate);

            Assert(eastGate != null, "Midgaard has an east gate");
            Assert(westGate != null, "Midgaard has a west gate");
            Assert(northGate != null, "Midgaard has a north gate");
            Assert(southGate != null, "Midgaard has a south gate");
            Assert(InvokePrivate<bool>(game, "CanStepExplore", eastGate.X, eastGate.Y), "east gate is passable");
            Assert(InvokePrivate<bool>(game, "CanStepExplore", westGate.X, westGate.Y), "west gate is passable");

            int left = InvokePrivate<int>(game, "MidgaardLeft", state.Map);
            int right = InvokePrivate<int>(game, "MidgaardRight", state.Map);
            int top = InvokePrivate<int>(game, "MidgaardTop", state.Map);
            int bottom = InvokePrivate<int>(game, "MidgaardBottom", state.Map);
            Assert(westGate.X == left && westGate.Y == state.Map.StartY, "west gate occupies the generated town's west midpoint");
            Assert(eastGate.X == right && eastGate.Y == state.Map.StartY, "east gate occupies the generated town's east midpoint");
            Assert(northGate.X == state.Map.StartX && northGate.Y == top, "north gate occupies the generated town's north midpoint");
            Assert(southGate.X == state.Map.StartX && southGate.Y == bottom, "south gate occupies the generated town's south midpoint");

            ExplorationCellRole openGateRoles =
                ExplorationCellRole.City | ExplorationCellRole.Road | ExplorationCellRole.Threshold;
            Assert(ExplorationSurfaceRules.MaterialAt(state.Map, westGate.X, westGate.Y) == ExplorationMaterial.CityPaving
                && (ExplorationSurfaceRules.RolesAt(state.Map, westGate.X, westGate.Y) & openGateRoles) == openGateRoles,
                "west gate is a city-paved road threshold");
            Assert(ExplorationSurfaceRules.MaterialAt(state.Map, eastGate.X, eastGate.Y) == ExplorationMaterial.CityPaving
                && (ExplorationSurfaceRules.RolesAt(state.Map, eastGate.X, eastGate.Y) & openGateRoles) == openGateRoles,
                "east gate is a city-paved road threshold");
            Assert(ExplorationSurfaceRules.MaterialAt(state.Map, northGate.X, northGate.Y) == ExplorationMaterial.CityWall,
                "north gate retains sealed city-wall material");
            Assert(ExplorationSurfaceRules.MaterialAt(state.Map, southGate.X, southGate.Y) == ExplorationMaterial.CityWall,
                "south gate retains sealed city-wall material");

            List<Point> passablePerimeter = new List<Point>();
            for (int x = left; x <= right; x++)
            {
                if (InvokePrivate<bool>(game, "CanStepExplore", x, top)) passablePerimeter.Add(new Point(x, top));
                if (InvokePrivate<bool>(game, "CanStepExplore", x, bottom)) passablePerimeter.Add(new Point(x, bottom));
            }
            for (int y = top + 1; y < bottom; y++)
            {
                if (InvokePrivate<bool>(game, "CanStepExplore", left, y)) passablePerimeter.Add(new Point(left, y));
                if (InvokePrivate<bool>(game, "CanStepExplore", right, y)) passablePerimeter.Add(new Point(right, y));
            }
            Assert(passablePerimeter.Count == 2
                && passablePerimeter.Any(point => point.X == westGate.X && point.Y == westGate.Y)
                && passablePerimeter.Any(point => point.X == eastGate.X && point.Y == eastGate.Y),
                "east and west gates are the generated town perimeter's only passable cells");

            Rect probeCell = new Rect(0f, 0f, 100f, 100f);
            Rect eastGateArt = InvokePrivate<Rect>(game, "ExploreObjectRect", probeCell, eastGate);
            Rect southGateArt = InvokePrivate<Rect>(game, "ExploreObjectRect", probeCell, southGate);
            Assert(eastGateArt.width >= 76f && eastGateArt.width <= 80f
                && eastGateArt.height >= 168f && eastGateArt.height <= 172f,
                "open side gate receives a compact wall-aligned art footprint");
            Assert(Mathf.Abs(eastGateArt.center.y - probeCell.center.y) < 0.01f,
                "open side gate remains centered on its road threshold");
            Assert(southGateArt.width >= 200f && southGateArt.width <= 204f
                && southGateArt.height >= 156f && southGateArt.height <= 160f,
                "sealed city gate receives a bounded wall-scale art footprint");

            SetPrivateField(game, "exploreWideView", true);
            Rect wideEastGateArt = InvokePrivate<Rect>(game, "ExploreObjectRect", probeCell, eastGate);
            Rect wideSouthGateArt = InvokePrivate<Rect>(game, "ExploreObjectRect", probeCell, southGate);
            Assert(wideEastGateArt.width >= 68f && wideEastGateArt.width <= 72f
                && wideEastGateArt.height >= 148f && wideEastGateArt.height <= 152f
                && Mathf.Abs(wideEastGateArt.center.y - probeCell.center.y) < 0.01f,
                "Region Map side gate keeps its compact wall-aligned footprint and centered threshold");
            Assert(wideSouthGateArt.width >= 176f && wideSouthGateArt.width <= 180f
                && wideSouthGateArt.height >= 138f && wideSouthGateArt.height <= 142f,
                "Region Map sealed gate remains bounded and wall-scale");
            SetPrivateField(game, "exploreWideView", false);

            List<Point> reachable = InvokePrivate<List<Point>>(game, "ReachableExploreTilesFrom", state.PlayerX, state.PlayerY);
            Assert(reachable.Any(point => point.X == eastGate.X && point.Y == eastGate.Y), "east gate is reachable from the starting plaza");
            Assert(reachable.Any(point => point.X == westGate.X && point.Y == westGate.Y), "west gate is reachable from the starting plaza");

            int originalX = state.PlayerX;
            int originalY = state.PlayerY;
            state.PlayerX = eastGate.X - 1;
            state.PlayerY = eastGate.Y;
            Assert(InvokePrivate<bool>(game, "CanStepExplore", state.PlayerX, state.PlayerY), "east gate interior approach is passable");
            InvokePrivate(game, "TryMoveOrUseExplore", 1, 0);
            Assert(state.PlayerX == eastGate.X && state.PlayerY == eastGate.Y, "shared mouse/keyboard movement enters a passable gate before interaction");
            InvokePrivate(game, "CloseTopOverlay");
            state.PlayerX = westGate.X + 1;
            state.PlayerY = westGate.Y;
            Assert(InvokePrivate<bool>(game, "CanStepExplore", state.PlayerX, state.PlayerY), "west gate interior approach is passable");
            InvokePrivate(game, "TryMoveOrUseExplore", -1, 0);
            Assert(state.PlayerX == westGate.X && state.PlayerY == westGate.Y, "shared mouse/keyboard movement enters the west gate");
            InvokePrivate(game, "CloseTopOverlay");
            state.PlayerX = originalX;
            state.PlayerY = originalY;

            if (northGate != null) Assert(!InvokePrivate<bool>(game, "CanStepExplore", northGate.X, northGate.Y), "north gate remains sealed");
            if (southGate != null) Assert(!InvokePrivate<bool>(game, "CanStepExplore", southGate.X, southGate.Y), "south gate remains sealed");
        }

        private static void AssertExplorationMovementProbe(AshenHallsGame game, GameState state)
        {
            int startX = state.PlayerX;
            int startY = state.PlayerY;
            int[] dx = { 0, 0, -1, 1 };
            int[] dy = { -1, 1, 0, 0 };

            for (int i = 0; i < dx.Length; i++)
            {
                state.PlayerX = startX;
                state.PlayerY = startY;
                int x = startX + dx[i];
                int y = startY + dy[i];
                bool canStep = InvokePrivate<bool>(game, "CanStepExplore", x, y);

                InvokePrivate(game, "TryMoveExplore", dx[i], dy[i]);
                if (canStep)
                {
                    Assert(state.PlayerX == x && state.PlayerY == y, $"movement probe {i} moves exactly one tile");
                }
                else
                {
                    Assert(state.PlayerX == startX && state.PlayerY == startY, $"blocked movement probe {i} keeps position");
                }
            }

            state.PlayerX = startX;
            state.PlayerY = startY;
        }

        private static void AssertNewNpcContactDialogues(
            AshenHallsGame game,
            GameState state,
            IReadOnlyDictionary<ObjectType, int> contactCells)
        {
            int originalX = state.PlayerX;
            int originalY = state.PlayerY;
            int originalFacingX = GetPrivateField<int>(game, "exploreFacingX");
            int originalFacingY = GetPrivateField<int>(game, "exploreFacingY");
            int originalGold = state.Gold;
            int originalSupplies = state.Supplies;
            Dictionary<ObjectType, string> expectedSpeakers = new Dictionary<ObjectType, string>
            {
                { ObjectType.DinerCook, "Kate" },
                { ObjectType.Provisioner, "Lute" },
                { ObjectType.DockWorker, "Dock Worker" },
                { ObjectType.Scholar, "Midgaard Scholar" }
            };
            Dictionary<ObjectType, int> expectedPortraits = new Dictionary<ObjectType, int>
            {
                { ObjectType.DinerCook, 12 },
                { ObjectType.Provisioner, 17 },
                { ObjectType.DockWorker, 18 },
                { ObjectType.Scholar, 19 }
            };

            foreach (KeyValuePair<ObjectType, int> contactCell in contactCells)
            {
                ObjectType type = contactCell.Key;
                MapObject contact = state.Map.Objects.Single(obj => obj != null && obj.Type == type);
                Assert(TryFindAdjacentProbeTile(game, state, contact, out int standX, out int standY), type + " has a deterministic adjacent interaction tile");
                state.PlayerX = standX;
                state.PlayerY = standY;
                SetPrivateField(game, "exploreFacingX", contact.X - standX);
                SetPrivateField(game, "exploreFacingY", contact.Y - standY);
                InvokePrivate(game, "InvalidateExplorationController");

                ExplorationInteraction interaction = InvokePrivate<ExplorationInteraction>(game, "CurrentExploreInteraction");
                Assert(interaction.HasTarget && ReferenceEquals(interaction.Target, contact), type + " is the exact live exploration target");
                Assert(interaction.Verb == "Talk" && interaction.Icon == "talk", type + " publishes Talk with the dialogue icon");
                Assert(InvokePrivate<string>(game, "ObjectName", contact).Length > 0, type + " retains a named world identity");
                Assert(InvokePrivate<string>(game, "ObjectHint", contact).Length > 0, type + " retains authored contact guidance");

                InvokePrivate(game, "UseNearbyExploreObject");
                InvokePrivate(game, "LateUpdate");
                Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Dialogue, type + " opens dialogue through the live Use path");
                Assert(GetPrivateField<ObjectType>(game, "dialogueFocus") == type, type + " preserves its exact dialogue focus");
                string speaker = GetPrivateField<string>(game, "dialogueSpeaker");
                Assert(speaker == expectedSpeakers[type], type + " opens with the expected speaker identity");
                Assert(NpcPortraitCatalog.PortraitIndex(type, speaker) == expectedPortraits[type], type + " resolves the expected portrait cell");

                DialogueScreen dialogue = GetPrivateField<DialogueScreen>(game, "dialogueScreen");
                int expectedChoices = type == ObjectType.DinerCook || type == ObjectType.Provisioner ? 4 : 0;
                Assert(dialogue != null && dialogue.IsVisible && dialogue.HasPortraitArt, type + " renders an authored portrait in the live dialogue");
                Assert(dialogue.VisibleChoiceCountForTest == expectedChoices, type + " exposes the correct conversation shape");
                Assert(state.Gold == originalGold && state.Supplies == originalSupplies, type + " Talk does not silently buy or grant provisions");
                InvokePrivate(game, "CloseDialogue");
                InvokePrivate(game, "LateUpdate");
                Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") != UiOverlay.Dialogue, type + " dialogue closes cleanly");
            }

            state.PlayerX = originalX;
            state.PlayerY = originalY;
            SetPrivateField(game, "exploreFacingX", originalFacingX);
            SetPrivateField(game, "exploreFacingY", originalFacingY);
            InvokePrivate(game, "InvalidateExplorationController");
        }

        private static MapObject FindAdjacentProbeTarget(AshenHallsGame game, GameState state, out int standX, out int standY)
        {
            standX = state?.PlayerX ?? 0;
            standY = state?.PlayerY ?? 0;
            if (state?.Map?.Objects == null) return null;

            foreach (MapObject obj in state.Map.Objects)
            {
                if (!IsPreferredProbeObject(obj)) continue;
                if (TryFindAdjacentProbeTile(game, state, obj, out standX, out standY)) return obj;
            }

            foreach (MapObject obj in state.Map.Objects)
            {
                if (!ExplorationTraversalRules.CanUseFromAdjacent(obj) || ExplorationTraversalRules.CanStandOnObject(obj)) continue;
                if (TryFindAdjacentProbeTile(game, state, obj, out standX, out standY)) return obj;
            }

            return null;
        }

        private static bool TryFindAdjacentProbeTile(AshenHallsGame game, GameState state, MapObject target, out int standX, out int standY)
        {
            int originalX = state.PlayerX;
            int originalY = state.PlayerY;
            int[] dx = { 0, -1, 1, 0 };
            int[] dy = { -1, 0, 0, 1 };

            for (int i = 0; i < dx.Length; i++)
            {
                int x = target.X - dx[i];
                int y = target.Y - dy[i];
                if (!InvokePrivate<bool>(game, "CanStepExplore", x, y)) continue;

                state.PlayerX = x;
                state.PlayerY = y;
                ExplorationInteraction interaction = InvokePrivate<ExplorationInteraction>(game, "CurrentExploreInteraction");
                if (interaction.HasTarget && interaction.Target == target)
                {
                    standX = x;
                    standY = y;
                    state.PlayerX = originalX;
                    state.PlayerY = originalY;
                    return true;
                }
            }

            state.PlayerX = originalX;
            state.PlayerY = originalY;
            standX = originalX;
            standY = originalY;
            return false;
        }

        private static bool IsPreferredProbeObject(MapObject obj)
        {
            if (obj == null) return false;
            switch (obj.Type)
            {
                case ObjectType.MarketClerk:
                case ObjectType.TempleHealer:
                case ObjectType.TavernKeeper:
                case ObjectType.GateCaptain:
                case ObjectType.CityCourier:
                case ObjectType.WoundedTraveler:
                case ObjectType.StableHand:
                case ObjectType.RoyalHerald:
                case ObjectType.NoviceHealer:
                case ObjectType.OldRoadScout:
                case ObjectType.TownGuard:
                    return true;
                default:
                    return false;
            }
        }

        private static CombatUnit MakeRuntimeEnemy(string id, int x, int y)
        {
            return new CombatUnit
            {
                Id = "runtime-" + id,
                PartyIndex = -1,
                Side = UnitSide.Enemy,
                Name = id,
                Role = "sewerrat",
                Race = "ratfolk",
                ClassKey = "enemy",
                Rank = "normal",
                Origin = "runtime-smoke",
                X = x,
                Y = y,
                Hp = 240,
                MaxHp = 240,
                Level = 1,
                Movement = 3,
                Power = 1,
                Defense = 0,
                Agility = 1,
                Range = 1,
                AttackSpeed = 1,
                DamageMin = 1,
                DamageMax = 2,
                Spell = "",
                Skills = new SkillSet(),
                Color = "8d6c55",
                DamageType = "physical",
                Resist = "",
                Weakness = "",
                MagicResist = 0
            };
        }

        private static void InvokePrivate(AshenHallsGame game, string methodName, params object[] args)
        {
            InvokePrivate<object>(game, methodName, args);
        }

        private static T InvokePrivate<T>(AshenHallsGame game, string methodName, params object[] args)
        {
            MethodInfo method = FindPrivateMethod(methodName, args);
            if (method == null)
            {
                throw new InvalidOperationException("Missing runtime method: " + methodName);
            }

            try
            {
                object result = method.Invoke(game, args);
                if (typeof(T) == typeof(object)) return default;
                return (T)result;
            }
            catch (TargetInvocationException ex)
            {
                throw new InvalidOperationException(methodName + " failed.", ex.InnerException ?? ex);
            }
        }

        private static MethodInfo FindPrivateMethod(string methodName, object[] args)
        {
            int argCount = args?.Length ?? 0;
            foreach (MethodInfo method in typeof(AshenHallsGame).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (method.Name != methodName) continue;
                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length != argCount) continue;
                bool compatible = true;
                for (int i = 0; i < parameters.Length; i++)
                {
                    object arg = args[i];
                    Type parameterType = parameters[i].ParameterType;
                    if (arg == null)
                    {
                        compatible = !parameterType.IsValueType || Nullable.GetUnderlyingType(parameterType) != null;
                    }
                    else
                    {
                        compatible = parameterType.IsInstanceOfType(arg);
                    }
                    if (!compatible) break;
                }
                if (!compatible) continue;
                return method;
            }

            return null;
        }

        private static void AssertMode(AshenHallsGame game, GameMode expected, string label)
        {
            GameState state = GetPrivateField<GameState>(game, "state");
            Assert(state != null, label + " has state");
            Assert(state.Mode == expected, $"{label}: expected {expected}, got {state.Mode}");
        }

        private static void AssertNoLaunchError(AshenHallsGame game)
        {
            string launchError = GetPrivateField<string>(game, "launchError");
            Assert(string.IsNullOrEmpty(launchError), "no launch error is present: " + launchError);
        }

        private static T GetPrivateField<T>(AshenHallsGame game, string fieldName)
        {
            FieldInfo field = typeof(AshenHallsGame).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
            {
                throw new InvalidOperationException("Missing runtime field: " + fieldName);
            }

            return (T)field.GetValue(game);
        }

        private static void SetPrivateField<T>(AshenHallsGame game, string fieldName, T value)
        {
            FieldInfo field = typeof(AshenHallsGame).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
            {
                throw new InvalidOperationException("Missing runtime field: " + fieldName);
            }
            field.SetValue(game, value);
        }

        private static void AssertActiveObject(string objectName)
        {
            GameObject found = GameObject.Find(objectName);
            Assert(found != null, objectName + " exists");
            Assert(found.activeInHierarchy, objectName + " is active");
        }

        private static void AssertRootOverlayCanvas(string objectName)
        {
            GameObject found = GameObject.Find(objectName);
            Assert(found != null && found.activeInHierarchy, objectName + " exists and is active");
            Canvas canvas = found.GetComponent<Canvas>();
            Assert(canvas != null, objectName + " has a Canvas");
            Assert(canvas.transform.parent == null, objectName + " is not nested beneath another canvas host");
            Assert(canvas.renderMode == RenderMode.ScreenSpaceOverlay, objectName + " renders as a screen-space overlay");
            Assert(found.GetComponent<UnityEngine.UI.GraphicRaycaster>() != null, objectName + " accepts pointer input");
        }

        private static void AssertEventSystemCount(int expected)
        {
            EventSystem[] eventSystems = UnityEngine.Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            Assert(eventSystems.Length == expected, $"expected {expected} active EventSystem, got {eventSystems.Length}");
        }

        private static void Assert(bool condition, string label)
        {
            if (!condition)
            {
                throw new InvalidOperationException(label);
            }
        }
    }
}
