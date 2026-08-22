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
        private const int DeterministicCampaignSeed = 51510;

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

        public static void RunRoamingThreats()
        {
            try
            {
                RunRoamingThreatsOrThrow();
                Debug.Log(VersionInfo.ProductName + " roaming-threat runtime smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " roaming-threat runtime smoke failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunRoamingThreatsOrThrow()
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string sceneFullPath = Path.Combine(projectRoot, MainScenePath);
            if (!File.Exists(sceneFullPath)) throw new InvalidOperationException("Main scene is missing: " + MainScenePath);

            try
            {
                Scene scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
                Assert(scene.IsValid() && scene.isLoaded, "Main scene loads for roaming-threat smoke");
                AshenHallsGame game = UnityEngine.Object.FindFirstObjectByType<AshenHallsGame>();
                Assert(game != null, "AshenHallsGame exists for roaming-threat smoke");
                InvokePrivate(game, "Awake");
                InvokePrivate(game, "LateUpdate");
                AssertNoLaunchError(game);
                InvokePrivate(game, "StartNewGame");
                InvokePrivate(game, "LateUpdate");
                InvokePrivate(game, "QuickStart");
                InvokePrivate(game, "LateUpdate");
                AssertMode(game, GameMode.Explore, "roaming-threat smoke reaches Explore");
                GameState state = GetPrivateField<GameState>(game, "state");
                AssertRoamingThreatCombatRuntime(game, state);
                AssertGeneratedRoamingThreatDepthsRuntime(game, state);
            }
            finally
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            }
        }

        public static void RunShopkeepers()
        {
            try
            {
                RunShopkeepersOrThrow();
                Debug.Log(VersionInfo.ProductName + " shopkeeper runtime smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " shopkeeper runtime smoke failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunShopkeepersOrThrow()
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
                AssertNoLaunchError(game);

                InvokePrivate(game, "StartNewGame");
                InvokePrivate(game, "LateUpdate");
                InvokePrivate(game, "QuickStart");
                InvokePrivate(game, "LateUpdate");
                AssertMode(game, GameMode.Explore, "shopkeeper smoke reaches Explore");

                GameState state = GetPrivateField<GameState>(game, "state");
                Assert(state != null, "shopkeeper smoke has a live game state");
                AssertKateServiceConversation(game, state);
                AssertExplicitServiceConversation(game, state, "VisitMidgaardArmorer", StoryFlags.MidgaardBasicArmorBought, 28, "Borin");
                AssertExplicitServiceConversation(game, state, "VisitWeaponVendor", StoryFlags.MidgaardBasicWeaponBought, 32, "Tessa");
                AssertWeaponEnchanterConversation(game, state);
            }
            finally
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
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
                Image authoredCharter = GameObject.Find("Authored Ashen Road Charter")?.GetComponent<Image>();
                Image authoredFocus = GameObject.Find("Focused Charter Ribbon")?.GetComponent<Image>();
                Texture2D liveTitleMenuScrollAtlas = GetPrivateField<Texture2D>(game, "titleMenuScrollArt");
                Texture2D liveTitleMenuFocusAtlas = GetPrivateField<Texture2D>(game, "titleMenuFocusArt");
                Texture2D liveTitleMenuIconAtlas = GetPrivateField<Texture2D>(game, "titleMenuIconAtlas");
                Assert(authoredCharter != null && authoredCharter.type == Image.Type.Sliced && authoredCharter.fillCenter && !authoredCharter.raycastTarget, "title menu renders its nonblocking sliced Ashen Road charter");
                Assert(authoredCharter.sprite != null && authoredCharter.sprite.border == TitleScreenPresentationRules.MenuScrollSpriteBorder, "live title charter uses the approved nine-slice border");
                Assert(authoredCharter.sprite.rect == new Rect(0f, 0f, 1280f, 1280f) && Mathf.Approximately(authoredCharter.sprite.pixelsPerUnit, 1500f), "live title charter keeps its exact source crop and slice scale");
                Assert(authoredFocus != null && authoredFocus.type == Image.Type.Sliced && authoredFocus.fillCenter && !authoredFocus.raycastTarget, "title focus uses its nonblocking sliced leather ribbon");
                Assert(authoredFocus.sprite != null && authoredFocus.sprite.border == TitleScreenPresentationRules.MenuFocusSpriteBorder, "live title focus ribbon keeps its ornamental ends outside the stretchable center");
                Assert(authoredFocus.sprite.rect == TitleScreenPresentationRules.MenuFocusSpriteRect && Mathf.Approximately(authoredFocus.sprite.pixelsPerUnit, 1200f), "live title focus ribbon keeps its exact cropped source and slice scale");
                Assert(ReferenceEquals(authoredCharter.sprite.texture, liveTitleMenuScrollAtlas) && ReferenceEquals(authoredFocus.sprite.texture, liveTitleMenuFocusAtlas), "live title sprites retain the externally owned approved textures");
                Assert(GameObject.Find("Aged Parchment Sheet") == null && GameObject.Find("Top Parchment Roll") == null, "authored title art suppresses the opaque procedural scroll layers");
                Image hearthBloom = GameObject.Find("Hearth Bloom")?.GetComponent<Image>();
                Image hearthFirebox = GameObject.Find("Hearth Firebox Flicker")?.GetComponent<Image>();
                Assert(hearthBloom != null && hearthFirebox != null && !hearthBloom.raycastTarget && !hearthFirebox.raycastTarget, "live title keeps noninteractive room and firebox hearth glows");
                Assert(hearthBloom.sprite != null && ReferenceEquals(hearthBloom.sprite, hearthFirebox.sprite), "room and firebox flicker reuse the same soft radial glow");
                Assert(hearthFirebox.rectTransform.rect.width < hearthBloom.rectTransform.rect.width && hearthFirebox.rectTransform.rect.height < hearthBloom.rectTransform.rect.height, "firebox flicker stays localized inside the broad hearth bloom");
                TavernScreen liveTavernScreen = UnityEngine.Object.FindFirstObjectByType<TavernScreen>();
                MethodInfo titleUpdate = typeof(TavernScreen).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert(liveTavernScreen != null && titleUpdate != null, "live title screen exposes its deterministic focus presentation update");
                titleUpdate.Invoke(liveTavernScreen, null);
                Image[] liveFocusRibbons = UnityEngine.Object.FindObjectsByType<Image>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                    .Where(image => image != null && image.name == "Focused Charter Ribbon")
                    .ToArray();
                Image[] selectedFocusRibbons = liveFocusRibbons.Where(image => image.color.a >= 0.99f).ToArray();
                Assert(liveFocusRibbons.Length == 5 && selectedFocusRibbons.Length == 1, "four core choices plus development Beta Lab render with exactly one opaque focus ribbon");
                Image[] liveTitleIcons = UnityEngine.Object.FindObjectsByType<Image>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                    .Where(image => image != null && image.name == "Relic Icon")
                    .ToArray();
                Assert(liveTitleIcons.Length == 5 && liveTitleIcons.All(image => image.sprite != null && !image.raycastTarget), "every live title action keeps one nonblocking purpose-built glyph");
                Assert(liveTitleIcons.All(image => ReferenceEquals(image.sprite.texture, liveTitleMenuIconAtlas)), "all live title glyphs come from the dedicated approved atlas");
                Button liveContinue = UnityEngine.Object.FindObjectsByType<Button>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                    .SingleOrDefault(button => button != null && button.name == "Continue");
                Button liveBetaLab = UnityEngine.Object.FindObjectsByType<Button>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                    .SingleOrDefault(button => button != null && button.name == "Beta Lab");
                bool liveSaveExists = InvokePrivate<bool>(game, "HasSavedGame");
                Assert(liveContinue != null && liveContinue.interactable == TavernMenuRules.EnableContinue(liveSaveExists), "Continue remains visible and accurately communicates save availability");
                Assert(liveBetaLab != null && liveBetaLab.interactable, "development title scroll exposes the direct Beta Lab action");
                FieldInfo liveTitleBindingsField = typeof(TavernScreen).GetField("bindings", BindingFlags.Instance | BindingFlags.NonPublic);
                TavernScreenBindings liveTitleBindings = liveTitleBindingsField?.GetValue(liveTavernScreen) as TavernScreenBindings;
                Assert(liveTitleBindings?.BetaLab != null && liveTitleBindings.BetaLab.Method.Name == "StartBetaCombatLabFromTitle", "development Beta Lab row dispatches through the save-blocked title lab path");
                Image selectedFocusRibbon = selectedFocusRibbons[0];
                Text selectedCursor = selectedFocusRibbon.transform.parent.GetComponentsInChildren<Text>(true).FirstOrDefault(text => text.name == "Forge Cursor");
                Image selectedRule = selectedFocusRibbon.transform.parent.GetComponentsInChildren<Image>(true).FirstOrDefault(image => image.name == "Focused Forge Rule");
                Assert(selectedCursor != null && selectedCursor.color.a > 0.5f, "selected title choice keeps a visible non-color forge cursor");
                Assert(selectedRule != null && selectedRule.color.a > 0.5f, "selected title choice keeps a visible non-color underline cue");
                AssertTitleMenuFocusReducedMotionRuntime(game, liveTavernScreen, titleUpdate, selectedCursor);
                AssertEventSystemCount(1);
                AssertNoLaunchError(game);
                AssertMode(game, GameMode.Tavern, "startup reaches Tavern");
                AssertSignatureItemMigrationRuntime(game);

                liveBetaLab.onClick.Invoke();
                InvokePrivate(game, "LateUpdate");
                AssertMode(game, GameMode.Combat, "development Beta Lab title row reaches Combat");
                Assert(GetPrivateField<bool>(game, "betaLabMode") && GetPrivateField<bool>(game, "labSaveBlocked"), "development Beta Lab title row enters the intended save-blocked lab state");

                InvokePrivate(game, "StartNewGame");
                InvokePrivate(game, "LateUpdate");
                AssertMode(game, GameMode.Muster, "New Game reaches Muster");
                Assert(!GetPrivateField<bool>(game, "betaLabMode") && !GetPrivateField<bool>(game, "labSaveBlocked"), "New Game resets the temporary Beta Lab state");
                AssertActiveObject("Party Setup Canvas");

                // Runtime smoke must exercise one stable campaign topology.
                // NewMuster intentionally uses Environment.TickCount for real
                // campaigns, which otherwise makes the build gate depend on
                // the millisecond at which Unity happened to start.
                GameState musterState = GetPrivateField<GameState>(game, "state");
                musterState.Seed = DeterministicCampaignSeed;

                InvokePrivate(game, "QuickStart");
                InvokePrivate(game, "LateUpdate");
                AssertMode(game, GameMode.Explore, "Quick Start reaches Explore");
                AssertActiveObject("Exploration HUD Canvas");
                AssertNoLaunchError(game);
                ExplorationHudScreen explorationHud = GetPrivateField<ExplorationHudScreen>(game, "explorationHudScreen");
                Canvas.ForceUpdateCanvases();
                Assert(explorationHud != null, "migrated exploration HUD screen exists");
                Assert(explorationHud.IsVisible, "migrated exploration HUD canvas is visible");
                Assert(!explorationHud.IsSuppressedByImguiFallback, "migrated exploration HUD is not suppressed by the emergency fallback");
                Assert(UiRuntime.HasUsableEventSystem(), "migrated exploration HUD has a usable pointer event system");
                Assert(explorationHud.IsInteractionOwner, "migrated exploration HUD is the visible, interactive owner");
                if (Screen.width >= 960 && Screen.height >= 600)
                {
                    Assert(explorationHud.HasLaidOutHud, $"migrated exploration HUD has renderable command geometry at {Screen.width}x{Screen.height}");
                    Assert(explorationHud.HasUsableHud, "migrated exploration HUD owns the supported viewport without emergency fallback");
                    Assert(InvokePrivate<bool>(game, "HasRenderableGameplayHud", UiOverlay.None),
                        "legacy exploration rendering preserves the visible uGUI chrome instead of painting a full-frame backdrop over it");
                }
                Assert(explorationHud.HasVisibleCompactGuidance, "default exploration rail keeps objective and waypoint visible");
                Assert(explorationHud.VisiblePartyRows == 4, "default exploration rail keeps all four party rows visible");
                Assert(explorationHud.HasExpandedResourceLabelsForTest, "exploration chrome keeps full Gold, Supplies, and Elixirs labels");
                Assert(explorationHud.VisiblePersistentCommandCountForTest == 5, "exploration footer exposes only the five persistent navigation commands");
                Assert(explorationHud.VisibleCompactManaRowsForTest == 0, "compact exploration rows defer secondary mana detail until Details is open");
                ExplorationHudView firstPlayView = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
                Assert(firstPlayView.ObjectiveSummary.IndexOf("Leave Town Hall", StringComparison.OrdinalIgnoreCase) >= 0
                    && firstPlayView.ObjectiveSummary.IndexOf("begin the journey", StringComparison.OrdinalIgnoreCase) >= 0,
                    "fresh-game objective makes leaving Town Hall the first journey step");
                Assert(firstPlayView.WaypointLine.IndexOf("Storm", StringComparison.OrdinalIgnoreCase) >= 0
                    && firstPlayView.WaypointLine.IndexOf("Town Hall", StringComparison.OrdinalIgnoreCase) >= 0,
                    "fresh-game waypoint first points to the Town Hall storm doors");
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
                    && firstPlayGuidanceTarget.IndexOf("Storm", StringComparison.OrdinalIgnoreCase) >= 0,
                    "fresh-game HUD and map share one reachable Town Hall exit plan");
                Assert(InvokePrivate<bool>(game, "CurrentExploreGuidanceIsInteriorExit"),
                    "fresh-game Golden Thread identifies the Grand Hearth storm doors as an interior exit");
                GameState firstPlayState = GetPrivateField<GameState>(game, "state");
                AssertDurableExplorationChartRuntime(game, firstPlayState);
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
                AssertPartyGrowthRuntime(game, firstPlayState);
                AssertExplorationWorldMapRuntime(game);
                AssertCombatPresentationRuntime(game);
            }
            finally
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            }
        }

        private static void AssertDurableExplorationChartRuntime(AshenHallsGame game, GameState state)
        {
            Assert(state?.Map != null && state.DiscoveredZones != null,
                "fresh exploration has a live map and durable discovery ledger");

            string playerCellKey = ExplorationChartRules.CellKey(state.Depth, state.PlayerX, state.PlayerY);
            Assert(InvokePrivate<bool>(game, "IsExploreCellCharted", state.PlayerX, state.PlayerY),
                "live exploration reveal keeps the party cell visible");
            Assert(state.DiscoveredZones.Any(key => string.Equals(key, playerCellKey, StringComparison.OrdinalIgnoreCase)),
                "live exploration reveal records the party cell in the durable discovery ledger");

            Point farUncharted = null;
            int farthestDistance = -1;
            for (int y = 0; y < state.Map.Height; y++)
            for (int x = 0; x < state.Map.Width; x++)
            {
                if (ExplorationChartRules.IsCharted(state.DiscoveredZones, state.Depth, x, y)) continue;
                int distance = Math.Abs(x - state.PlayerX) + Math.Abs(y - state.PlayerY);
                if (distance <= farthestDistance) continue;
                farthestDistance = distance;
                farUncharted = new Point(x, y);
            }

            Assert(farUncharted != null, "fresh exploration retains terrain beyond the revealed chart");
            Assert(!InvokePrivate<bool>(game, "IsExploreCellCharted", farUncharted.X, farUncharted.Y),
                "a far uncharted cell remains hidden from the live exploration view");
            IReadOnlyList<Point> chartBoundaryProbe = new[]
            {
                new Point(state.PlayerX, state.PlayerY),
                farUncharted
            };
            Assert(InvokePrivate<int>(game, "ExploreGuidanceChartedPrefixCount", chartBoundaryProbe) == 1,
                "Golden Thread presentation stops before the first uncharted route cell");
            bool originalWideView = GetPrivateField<bool>(game, "exploreWideView");
            try
            {
                SetPrivateField(game, "exploreWideView", true);
                string farLookLine = InvokePrivate<string>(game, "ExploreLookLine", farUncharted.X, farUncharted.Y);
                Assert(farLookLine.IndexOf("Uncharted", StringComparison.OrdinalIgnoreCase) >= 0,
                    "looking at hidden terrain on the Region Map reports it as Uncharted");
                SetPrivateField(game, "exploreWideView", false);
                string localLookLine = InvokePrivate<string>(game, "ExploreLookLine", farUncharted.X, farUncharted.Y);
                Assert(localLookLine.IndexOf("Uncharted", StringComparison.OrdinalIgnoreCase) < 0,
                    "Local Map look text remains fully detailed instead of inheriting Region Map fog");
            }
            finally
            {
                SetPrivateField(game, "exploreWideView", originalWideView);
            }

            ExplorationHudView chartView = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
            Assert(chartView.FocusHint.IndexOf("Chart", StringComparison.OrdinalIgnoreCase) >= 0,
                "exploration HUD focus guidance publishes chart progress");

            GameState roundTripSource = new GameState
            {
                SaveVersion = state.SaveVersion,
                Depth = state.Depth,
                DiscoveredZones = new List<string> { playerCellKey }
            };
            GameState roundTrip = JsonUtility.FromJson<GameState>(JsonUtility.ToJson(roundTripSource));
            Assert(roundTrip?.DiscoveredZones != null
                && roundTrip.DiscoveredZones.Any(key => string.Equals(key, playerCellKey, StringComparison.OrdinalIgnoreCase))
                && ExplorationChartRules.IsCharted(roundTrip.DiscoveredZones, state.Depth, state.PlayerX, state.PlayerY),
                "durable exploration chart keys survive JSON save/load");
        }

        private static void AssertPartyGrowthRuntime(AshenHallsGame game, GameState state)
        {
            Assert(state?.Party != null && state.Party.Count > 0, "Party Growth runtime has a live party");
            PartyMember originalMember = state.Party[0];
            PartyMember probe = originalMember.CloneForPreview();
            probe.Id = "party-growth-runtime-probe";
            probe.StatPoints = 1;
            probe.SkillPoints = 1;
            PartyGrowthChoice talent = PartyGrowthRules.RelevantTalents(probe).First();
            Stats startingStats = probe.Stats;
            int startingTalent = PartyGrowthRules.ProjectedSkill(probe, new PartyGrowthPlan(), talent);
            int originalTab = GetPrivateField<int>(game, "armoryTab");
            int originalPartyIndex = GetPrivateField<int>(game, "armorySelectedPartyIndex");
            bool originalShowArmory = GetPrivateField<bool>(game, "showArmory");
            GameMode originalMode = state.Mode;
            CombatState originalCombat = state.Combat;
            List<LogEntry> originalLog = state.Log == null ? null : new List<LogEntry>(state.Log);

            try
            {
                state.Party[0] = probe;
                state.Mode = GameMode.Explore;
                state.Combat = null;
                SetPrivateField(game, "armorySelectedPartyIndex", 0);
                SetPrivateField(game, "showArmory", false);
                InvokePrivate(game, "DiscardArmoryGrowthDrafts");
                InvokePrivate(game, "ToggleArmory", 4);
                InvokePrivate(game, "LateUpdate");

                ArmoryOverlayScreen armory = GetPrivateField<ArmoryOverlayScreen>(game, "armoryOverlayScreen");
                Assert(armory != null && armory.IsVisible && armory.HasRenderableGeometry, "Party Growth opens through the live Armory overlay");
                Assert(armory.ActiveTabLabelForTest == "Growth", "the fifth Armory tab has the Growth label");
                Assert(armory.VisibleFilterCountForTest == state.Party.Count, "Growth exposes one member filter per adventurer");
                Assert(armory.VisibleRowCountForTest == 4 + PartyGrowthRules.RelevantTalents(probe).Count, "Growth shows four attributes plus only class-relevant talents");
                Assert(armory.HasVisibleDetailForTest && armory.VisibleDetailActionCountForTest == 2, "Growth presents Apply and Reset beside the preview");
                Assert(armory.FocusedRowIsCommittedForTest, "spendable Growth opens with an interactable committed row focused");

                IReadOnlyList<ArmoryRowView> rows = InvokePrivate<IReadOnlyList<ArmoryRowView>>(game, "BuildArmoryGrowthRows");
                ArmoryRowView strengthRow = rows.Single(row => row.Title == "Strength");
                int strengthVisibleIndex = armory.VisibleRowIndexForKeyForTest(strengthRow.Key);
                Assert(strengthVisibleIndex >= 0 && strengthRow.ActionEnabled, "Strength is a live spendable Growth row");
                armory.InvokeRowActionForTest(strengthVisibleIndex);
                InvokePrivate(game, "LateUpdate");
                Assert(probe.Stats.Strength == startingStats.Strength
                    && probe.StatPoints == 1
                    && probe.SkillPoints == 1,
                    "staging Strength updates only the preview");

                armory.InvokeDetailActionForTest(1);
                InvokePrivate(game, "LateUpdate");
                Assert(probe.Stats.Strength == startingStats.Strength
                    && PartyGrowthRules.ProjectedSkill(probe, new PartyGrowthPlan(), talent) == startingTalent
                    && probe.StatPoints == 1
                    && probe.SkillPoints == 1,
                    "Reset cancels the draft without mutating the member or point totals");

                rows = InvokePrivate<IReadOnlyList<ArmoryRowView>>(game, "BuildArmoryGrowthRows");
                strengthRow = rows.Single(row => row.Title == "Strength");
                armory.InvokeRowActionForTest(armory.VisibleRowIndexForKeyForTest(strengthRow.Key));
                InvokePrivate(game, "LateUpdate");
                rows = InvokePrivate<IReadOnlyList<ArmoryRowView>>(game, "BuildArmoryGrowthRows");
                ArmoryRowView talentRow = rows.Single(row => row.Title == PartyGrowthRules.Label(talent));
                int talentVisibleIndex = armory.VisibleRowIndexForKeyForTest(talentRow.Key);
                Assert(talentVisibleIndex >= 0 && talentRow.ActionEnabled, "the selected class talent is live and spendable");
                armory.InvokeRowActionForTest(talentVisibleIndex);
                InvokePrivate(game, "LateUpdate");
                Assert(probe.Stats.Strength == startingStats.Strength
                    && PartyGrowthRules.ProjectedSkill(probe, new PartyGrowthPlan(), talent) == startingTalent,
                    "combined stat and talent staging remains mutation-free before Apply");

                armory.InvokeDetailActionForTest(0);
                InvokePrivate(game, "LateUpdate");
                Assert(probe.Stats.Strength == startingStats.Strength + 1, "Apply commits exactly +1 Strength");
                Assert(PartyGrowthRules.ProjectedSkill(probe, new PartyGrowthPlan(), talent) == startingTalent + 2, "Apply commits exactly +2 to the class talent");
                Assert(probe.StatPoints == 0 && probe.SkillPoints == 0, "Apply consumes exactly the staged stat and skill points");

                InvokePrivate(game, "CloseArmoryOverlay");
                InvokePrivate(game, "LateUpdate");
                InvokePrivate(game, "ToggleArmory", 4);
                InvokePrivate(game, "LateUpdate");
                rows = InvokePrivate<IReadOnlyList<ArmoryRowView>>(game, "BuildArmoryGrowthRows");
                Assert(armory.ActiveTabLabelForTest == "Growth"
                    && rows.All(row => string.IsNullOrEmpty(row.Badge) || row.Badge.IndexOf("STAGED", StringComparison.OrdinalIgnoreCase) < 0),
                    "reopening Growth keeps applied values and never resurrects a committed draft");

                GameState roundTripSource = new GameState
                {
                    SaveVersion = state.SaveVersion,
                    Party = new List<PartyMember> { probe }
                };
                GameState roundTrip = JsonUtility.FromJson<GameState>(JsonUtility.ToJson(roundTripSource));
                PartyMember loaded = roundTrip.Party.Single();
                Assert(roundTrip.SaveVersion == state.SaveVersion, "Party Growth keeps the existing save schema");
                Assert(loaded.Stats.Strength == probe.Stats.Strength
                    && PartyGrowthRules.ProjectedSkill(loaded, new PartyGrowthPlan(), talent)
                        == PartyGrowthRules.ProjectedSkill(probe, new PartyGrowthPlan(), talent)
                    && loaded.StatPoints == probe.StatPoints
                    && loaded.SkillPoints == probe.SkillPoints,
                    "applied Growth values and remaining points survive JSON save/load");

                probe.StatPoints = 1;
                probe.SkillPoints = 1;
                state.Mode = GameMode.Combat;
                InvokePrivate(game, "MarkUiDirty");
                InvokePrivate(game, "LateUpdate");
                rows = InvokePrivate<IReadOnlyList<ArmoryRowView>>(game, "BuildArmoryGrowthRows");
                int combatStrength = probe.Stats.Strength;
                int combatTalent = PartyGrowthRules.ProjectedSkill(probe, new PartyGrowthPlan(), talent);
                Assert(rows.Count > 0 && rows.All(row => !row.ActionEnabled), "Growth rows are read-only during combat even with points available");
                Assert(armory.FocusedRowIndexForTest < 0, "combat review never focuses a disabled Growth row");
                armory.InvokeDetailActionForTest(0);
                Assert(probe.Stats.Strength == combatStrength
                    && PartyGrowthRules.ProjectedSkill(probe, new PartyGrowthPlan(), talent) == combatTalent
                    && probe.StatPoints == 1
                    && probe.SkillPoints == 1,
                    "combat review cannot apply or consume Growth");
            }
            finally
            {
                SetPrivateField(game, "showArmory", false);
                SetPrivateField(game, "armoryTab", originalTab);
                SetPrivateField(game, "armorySelectedPartyIndex", originalPartyIndex);
                InvokePrivate(game, "DiscardArmoryGrowthDrafts");
                state.Party[0] = originalMember;
                state.Mode = originalMode;
                state.Combat = originalCombat;
                state.Log = originalLog;
                SetPrivateField(game, "showArmory", originalShowArmory);
                InvokePrivate(game, "MarkUiDirty");
                InvokePrivate(game, "LateUpdate");
            }
        }

        private static void AssertRoamingThreatCombatRuntime(AshenHallsGame game, GameState state)
        {
            Assert(state?.Map != null, "roaming-threat runtime has a generated map");
            IReadOnlyList<RoamingThreatDefinition> definitions = RoamingThreatCatalog.ForDepth(
                state.Depth,
                ContentSetCatalog.IsFullPrototype(state.ContentSetId));
            List<RoamingThreat> patrols = state.RoamingThreats
                .Where(threat => threat != null && threat.Depth == state.Depth)
                .OrderBy(threat => threat.Id)
                .ToList();
            Assert(patrols.Count == 4 && patrols.Count == definitions.Count, "four cataloged patrols spawn on the chapter-one world map");
            foreach (RoamingThreatDefinition definition in definitions)
            {
                RoamingThreat patrol = patrols.SingleOrDefault(threat => threat.Id == definition.Id);
                Assert(patrol != null, definition.Id + " spawns with its stable identity");
                Assert(patrol.Archetype == definition.Archetype, definition.Id + " uses its cataloged visible archetype");
                Assert(patrol.X != state.PlayerX || patrol.Y != state.PlayerY, definition.Id + " does not overlap the player");
                WorldZone zone = InvokePrivate<WorldZone>(game, "ZoneFor", patrol.X, patrol.Y, state.Map, state.Depth);
                Assert(zone != null && zone.Danger > 0, definition.Id + " stays outside Midgaard and safe roads");
            }
            for (int i = 0; i < patrols.Count; i++)
            for (int j = i + 1; j < patrols.Count; j++)
            {
                int distance = Math.Abs(patrols[i].HomeX - patrols[j].HomeX) + Math.Abs(patrols[i].HomeY - patrols[j].HomeY);
                Assert(distance >= 7, patrols[i].Id + " and " + patrols[j].Id + " preserve patrol-home spacing");
            }

            string stableSignature = string.Join("|", patrols.Select(threat => threat.Id + ":" + threat.HomeX + "," + threat.HomeY));
            RoamingThreat first = patrols[0];
            state.RoamingThreats.Add(new RoamingThreat
            {
                Id = first.Id,
                Name = "Corrupt duplicate",
                Archetype = first.Archetype,
                Depth = first.Depth,
                X = first.X,
                Y = first.Y,
                HomeX = first.HomeX,
                HomeY = first.HomeY
            });
            InvokePrivate(game, "EnsureRoamingThreats");
            Assert(state.RoamingThreats.Count(threat => threat != null && threat.Id == first.Id) == 1, "duplicate saved patrol identity is removed");
            first.X = -1;
            first.Y = -1;
            first.HomeX = -1;
            first.HomeY = -1;
            InvokePrivate(game, "EnsureRoamingThreats");
            string repairedSignature = string.Join("|", state.RoamingThreats
                .Where(threat => threat != null && threat.Depth == state.Depth)
                .OrderBy(threat => threat.Id)
                .Select(threat => threat.Id + ":" + threat.HomeX + "," + threat.HomeY));
            Assert(stableSignature == repairedSignature, "invalid saved patrol home repairs to the deterministic catalog spawn");

            foreach (RoamingThreatDefinition definition in definitions)
            {
                RoamingThreat patrol = state.RoamingThreats.Single(threat => threat != null && threat.Id == definition.Id);
                InvokePrivate(game, "StartRoamingThreatCombat", patrol);
                Assert(state.Mode == GameMode.Combat && state.Combat != null, definition.Id + " opens live patrol combat");
                Assert(state.Combat.EncounterStyle == "patrol", definition.Id + " retains patrol rewards and victory routing");
                Assert(state.Combat.RoamingThreatId == definition.Id, definition.Id + " binds live defeat state to its stable identity");
                List<CombatUnit> enemies = state.Combat.Units.Where(unit => unit != null && unit.Side == UnitSide.Enemy).ToList();
                Assert(enemies.Count == EncounterCatalog.For(EncounterId.Patrol).EnemyCountForDepth(state.Depth), definition.Id + " retains standard patrol difficulty");
                Assert(enemies.All(enemy => definition.EnemyIds.Contains(enemy.Role)), definition.Id + " combat units come from its explicit roster");
                Assert(enemies.All(enemy => RoamingThreatCatalog.FactionForEnemy(enemy.Role) == definition.Faction), definition.Id + " visible and combat factions agree");
                state.Mode = GameMode.Explore;
                state.Combat = null;
                patrol.Alerted = false;
                InvokePrivate(game, "InvalidateCombatController");
            }
        }

        private static void AssertGeneratedRoamingThreatDepthsRuntime(AshenHallsGame game, GameState state)
        {
            const int seed = 51510;
            HashSet<RoamingThreatFaction> behaviorFactionsExercised = new HashSet<RoamingThreatFaction>();
            foreach (string contentSet in new[] { ContentSetCatalog.SewerSlice, ContentSetCatalog.FullPrototype })
            {
                bool fullPrototype = ContentSetCatalog.IsFullPrototype(contentSet);
                InvokePrivate(game, "SetActiveContentSet", contentSet);
                for (int depth = 2; depth <= 6; depth++)
                {
                    state.Depth = depth;
                    state.Seed = seed;
                    state.Mode = GameMode.Explore;
                    state.Map = InvokePrivate<MapData>(game, "GenerateMap", depth, seed);
                    state.PlayerX = state.Map.StartX;
                    state.PlayerY = state.Map.StartY;
                    state.RoamingThreats = new List<RoamingThreat>();
                    InvokePrivate(game, "InvalidateExplorationController");
                    InvokePrivate(game, "EnsureRoamingThreats");

                    IReadOnlyList<RoamingThreatDefinition> definitions = RoamingThreatCatalog.ForDepth(depth, fullPrototype);
                    List<RoamingThreat> patrols = state.RoamingThreats
                        .Where(threat => threat != null && threat.Depth == depth)
                        .OrderBy(threat => threat.Id)
                        .ToList();
                    int expectedCount = fullPrototype
                        ? (depth <= 3 ? 4 : 5)
                        : depth <= 5 ? 3 : 0;
                    string label = contentSet + " depth " + depth;
                    Assert(definitions.Count == expectedCount, label + " catalog has the expected patrol count");
                    Assert(patrols.Count == expectedCount, label + " generated map instantiates every catalog patrol");

                    bool[,] reachable = ExplorationTraversalRules.ReachableMask(
                        state.Map,
                        state.PlayerX,
                        state.PlayerY);
                    foreach (RoamingThreatDefinition definition in definitions)
                    {
                        RoamingThreat patrol = patrols.SingleOrDefault(threat => threat.Id == definition.Id);
                        Assert(patrol != null, label + " instantiates " + definition.Id);
                        RoamingThreatBehaviorProfile expectedBehavior = definition.BehaviorProfile;
                        RoamingThreatBehaviorProfile liveBehavior = InvokePrivate<RoamingThreatBehaviorProfile>(
                            game,
                            "RoamingThreatBehaviorFor",
                            patrol);
                        Assert(liveBehavior.Id == expectedBehavior.Id, label + " wires " + definition.Id + " to behavior " + expectedBehavior.Id);
                        Assert(liveBehavior.AlertRadius == expectedBehavior.AlertRadius, label + " wires " + definition.Id + " alert radius");
                        Assert(liveBehavior.PursuitCadence == expectedBehavior.PursuitCadence, label + " wires " + definition.Id + " pursuit cadence");
                        Assert(liveBehavior.ReturnCadence == expectedBehavior.ReturnCadence, label + " wires " + definition.Id + " return cadence");
                        Assert(liveBehavior.LeashRadius == expectedBehavior.LeashRadius, label + " wires " + definition.Id + " leash radius");
                        Assert(patrol.Active, label + " starts " + definition.Id + " active");
                        Assert(patrol.X >= 0 && patrol.Y >= 0 && patrol.X < state.Map.Width && patrol.Y < state.Map.Height, label + " keeps " + definition.Id + " in bounds");
                        Assert(reachable[patrol.X, patrol.Y], label + " keeps " + definition.Id + " reachable from the party start");
                        Assert(ExplorationTraversalRules.IsStandable(state.Map, patrol.X, patrol.Y), label + " places " + definition.Id + " on passable terrain");
                        Assert(patrol.X != state.PlayerX || patrol.Y != state.PlayerY, label + " keeps " + definition.Id + " off the player cell");
                        WorldZone zone = InvokePrivate<WorldZone>(game, "ZoneFor", patrol.X, patrol.Y, state.Map, depth);
                        Assert(zone != null && zone.Danger > 0, label + " keeps " + definition.Id + " outside safe zones");
                        if (behaviorFactionsExercised.Add(definition.Faction))
                        {
                            AssertLiveRoamingThreatPursuitCadence(game, state, patrol, expectedBehavior, label + " " + definition.Faction);
                        }
                    }
                    for (int i = 0; i < patrols.Count; i++)
                    for (int j = i + 1; j < patrols.Count; j++)
                    {
                        int distance = Math.Abs(patrols[i].HomeX - patrols[j].HomeX)
                            + Math.Abs(patrols[i].HomeY - patrols[j].HomeY);
                        Assert(distance >= 7, label + " preserves home spacing between " + patrols[i].Id + " and " + patrols[j].Id);
                    }

                    string signature = string.Join("|", patrols.Select(threat => threat.Id + ":" + threat.HomeX + "," + threat.HomeY));
                    InvokePrivate(game, "EnsureRoamingThreats");
                    string repairedSignature = string.Join("|", state.RoamingThreats
                        .Where(threat => threat != null && threat.Depth == depth)
                        .OrderBy(threat => threat.Id)
                        .Select(threat => threat.Id + ":" + threat.HomeX + "," + threat.HomeY));
                    Assert(signature == repairedSignature, label + " population is deterministic and idempotent");

                    foreach (RoamingThreatDefinition definition in definitions)
                    {
                        RoamingThreat patrol = state.RoamingThreats.Single(threat => threat != null && threat.Id == definition.Id);
                        InvokePrivate(game, "StartRoamingThreatCombat", patrol);
                        Assert(state.Mode == GameMode.Combat && state.Combat != null, label + " opens combat for " + definition.Id);
                        Assert(state.Combat.EncounterStyle == "patrol", label + " retains patrol lifecycle for " + definition.Id);
                        Assert(state.Combat.RoamingThreatId == definition.Id, label + " binds combat to " + definition.Id);
                        List<CombatUnit> enemies = state.Combat.Units
                            .Where(unit => unit != null && unit.Side == UnitSide.Enemy)
                            .ToList();
                        Assert(enemies.Count == EncounterCatalog.For(EncounterId.Patrol).EnemyCountForDepth(depth), label + " retains standard patrol difficulty for " + definition.Id);
                        Assert(enemies.All(enemy => definition.EnemyIds.Contains(enemy.Role)), label + " uses the explicit roster for " + definition.Id);
                        Assert(enemies.All(enemy => RoamingThreatCatalog.FactionForEnemy(enemy.Role) == definition.Faction), label + " keeps visible and combat factions aligned for " + definition.Id);
                        state.Mode = GameMode.Explore;
                        state.Combat = null;
                        patrol.Alerted = false;
                        InvokePrivate(game, "InvalidateCombatController");
                    }
                }
            }
            Assert(
                behaviorFactionsExercised.SetEquals(new[]
                {
                    RoamingThreatFaction.Rats,
                    RoamingThreatFaction.Kobolds,
                    RoamingThreatFaction.Drow,
                    RoamingThreatFaction.Undead,
                    RoamingThreatFaction.Demons
                }),
                "generated roaming-threat runtime coverage exercises every faction behavior profile");
        }

        private sealed class RoamingThreatRuntimeSnapshot
        {
            public readonly RoamingThreat Threat;
            public readonly int X;
            public readonly int Y;
            public readonly bool Active;
            public readonly bool Alerted;
            public readonly int GraceSteps;
            public readonly int RespawnSteps;

            public RoamingThreatRuntimeSnapshot(RoamingThreat threat)
            {
                Threat = threat;
                X = threat.X;
                Y = threat.Y;
                Active = threat.Active;
                Alerted = threat.Alerted;
                GraceSteps = threat.GraceSteps;
                RespawnSteps = threat.RespawnSteps;
            }

            public void Restore()
            {
                Threat.X = X;
                Threat.Y = Y;
                Threat.Active = Active;
                Threat.Alerted = Alerted;
                Threat.GraceSteps = GraceSteps;
                Threat.RespawnSteps = RespawnSteps;
            }
        }

        private static void AssertLiveRoamingThreatPursuitCadence(
            AshenHallsGame game,
            GameState state,
            RoamingThreat patrol,
            RoamingThreatBehaviorProfile behavior,
            string label)
        {
            int originalPlayerX = state.PlayerX;
            int originalPlayerY = state.PlayerY;
            int originalExplorationSteps = state.ExplorationSteps;
            GameMode originalMode = state.Mode;
            CombatState originalCombat = state.Combat;
            List<RoamingThreatRuntimeSnapshot> snapshots = state.RoamingThreats
                .Where(threat => threat != null)
                .Select(threat => new RoamingThreatRuntimeSnapshot(threat))
                .ToList();

            try
            {
                foreach (RoamingThreatRuntimeSnapshot snapshot in snapshots)
                {
                    if (snapshot.Threat == patrol) continue;
                    snapshot.Threat.Active = false;
                    snapshot.Threat.Alerted = false;
                    snapshot.Threat.RespawnSteps = 999;
                }

                patrol.Active = true;
                patrol.Alerted = true;
                patrol.GraceSteps = 0;
                patrol.RespawnSteps = 0;
                patrol.X = patrol.HomeX;
                patrol.Y = patrol.HomeY;

                state.PlayerX = -1;
                state.PlayerY = -1;
                MethodInfo cellAvailabilityMethod = FindPrivateMethod(
                    "IsRoamingThreatCellAvailable",
                    new object[] { 0, 0, patrol.Id });
                Assert(cellAvailabilityMethod != null, label + " resolves the live hostile-cell rule");
                bool[,] pursuitCells = new bool[state.Map.Width, state.Map.Height];
                for (int y = 0; y < state.Map.Height; y++)
                for (int x = 0; x < state.Map.Width; x++)
                {
                    pursuitCells[x, y] = (bool)cellAvailabilityMethod.Invoke(
                        game,
                        new object[] { x, y, patrol.Id });
                }

                Point playerCell = null;
                for (int targetDistance = behavior.AlertRadius; targetDistance >= 3 && playerCell == null; targetDistance--)
                {
                    for (int y = 1; y < state.Map.Height - 1 && playerCell == null; y++)
                    for (int x = 1; x < state.Map.Width - 1; x++)
                    {
                        int distance = Math.Abs(x - patrol.HomeX) + Math.Abs(y - patrol.HomeY);
                        if (distance != targetDistance) continue;
                        if (snapshots.Any(snapshot => snapshot.Threat.HomeX == x && snapshot.Threat.HomeY == y)) continue;
                        if (!pursuitCells[x, y]) continue;
                        bool routeFound = RoamingThreatRules.TryNextStep(
                            state.Map.Width,
                            state.Map.Height,
                            patrol.HomeX,
                            patrol.HomeY,
                            x,
                            y,
                            (testX, testY) => pursuitCells[testX, testY],
                            (testX, testY) => testX == x && testY == y,
                            true,
                            out _);
                        if (!routeFound) continue;
                        playerCell = new Point(x, y);
                        break;
                    }
                }

                Assert(playerCell != null, label + " finds a hostile pursuit lane inside its alert radius");
                state.PlayerX = playerCell.X;
                state.PlayerY = playerCell.Y;
                state.ExplorationSteps = 0;
                state.Mode = GameMode.Explore;
                state.Combat = null;

                int startX = patrol.X;
                int startY = patrol.Y;
                for (int step = 1; step <= behavior.PursuitCadence; step++)
                {
                    bool combatStarted = InvokePrivate<bool>(game, "AdvanceRoamingThreatsAfterPartyStep");
                    Assert(!combatStarted && state.Mode == GameMode.Explore && state.Combat == null, label + " pursuit cadence stays in exploration at step " + step);
                    int moved = Math.Abs(patrol.X - startX) + Math.Abs(patrol.Y - startY);
                    if (step < behavior.PursuitCadence)
                    {
                        Assert(moved == 0, label + " waits for pursuit cadence step " + behavior.PursuitCadence);
                    }
                    else
                    {
                        Assert(moved == 1, label + " advances exactly one orthogonal cell on pursuit cadence step " + behavior.PursuitCadence);
                    }
                }
            }
            finally
            {
                foreach (RoamingThreatRuntimeSnapshot snapshot in snapshots) snapshot.Restore();
                state.PlayerX = originalPlayerX;
                state.PlayerY = originalPlayerY;
                state.ExplorationSteps = originalExplorationSteps;
                state.Mode = originalMode;
                state.Combat = originalCombat;
                InvokePrivate(game, "InvalidateExplorationController");
                InvokePrivate(game, "InvalidateCombatController");
            }
        }

        private static void AssertRegionMapBrowsingRuntime(AshenHallsGame game, GameState state)
        {
            bool originalWideView = GetPrivateField<bool>(game, "exploreWideView");
            bool originalHudCollapsed = GetPrivateField<bool>(game, "exploreHudCollapsed");
            MapData originalFocusMap = GetPrivateField<MapData>(game, "exploreRegionFocusMap");
            int originalFocusX = GetPrivateField<int>(game, "exploreRegionFocusX");
            int originalFocusY = GetPrivateField<int>(game, "exploreRegionFocusY");
            int originalHeldX = GetPrivateField<int>(game, "exploreRegionHeldAxisX");
            int originalHeldY = GetPrivateField<int>(game, "exploreRegionHeldAxisY");
            float originalRepeatAt = GetPrivateField<float>(game, "exploreRegionNextRepeatAt");
            bool originalPointerDragging = GetPrivateField<bool>(game, "exploreRegionPointerDragging");
            float originalDragRemainderX = GetPrivateField<float>(game, "exploreRegionDragRemainderX");
            float originalDragRemainderY = GetPrivateField<float>(game, "exploreRegionDragRemainderY");
            int playerX = state.PlayerX;
            int playerY = state.PlayerY;
            int explorationSteps = state.ExplorationSteps;
            string activeWaypoint = state.ActiveRouteWaypointKey;
            string[] storyFlags = state.StoryFlags.ToArray();
            string[] chartEntries = state.DiscoveredZones.ToArray();
            int objectCount = state.Map?.Objects?.Count ?? 0;
            EventSystem selectionEventSystem = EventSystem.current
                ?? (!Application.isPlaying ? UiRuntime.EnsureEventSystemReady() : null);
            GameObject selectedBefore = selectionEventSystem == null ? null : selectionEventSystem.currentSelectedGameObject;

            try
            {
                SetPrivateField(game, "exploreWideView", true);
                InvokePrivate(game, "ResetRegionMapFocusToParty");
                Assert(GetPrivateField<int>(game, "exploreRegionFocusX") == playerX
                    && GetPrivateField<int>(game, "exploreRegionFocusY") == playerY,
                    "Region Map opens with its transient browse focus on the party");

                int viewWidth = InvokePrivate<int>(game, "ExploreViewportWidth");
                int viewHeight = InvokePrivate<int>(game, "ExploreViewportHeight");
                Point partyOrigin = InvokePrivate<Point>(game, "ExploreViewportOrigin", viewWidth, viewHeight);
                Assert(InvokePrivate<bool>(game, "PanRegionMapFocus", 7, -5), "Region Map browse focus pans independently of travel");
                int pannedX = GetPrivateField<int>(game, "exploreRegionFocusX");
                int pannedY = GetPrivateField<int>(game, "exploreRegionFocusY");
                Point pannedOrigin = InvokePrivate<Point>(game, "ExploreViewportOrigin", viewWidth, viewHeight);
                Assert(pannedX != playerX || pannedY != playerY, "Region Map keeps a focus distinct from the party after panning");
                Assert(pannedOrigin.X != partyOrigin.X || pannedOrigin.Y != partyOrigin.Y, "Region Map panning moves the live viewport origin");
                ExplorationHudView pannedView = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
                Assert(pannedView.Title == "Region Map"
                    && pannedView.FocusHint.IndexOf($"{pannedX},{pannedY}", StringComparison.Ordinal) >= 0,
                    "Region Map HUD exposes the browsed coordinates without replacing the exploration HUD owner");

                Assert(InvokePrivate<bool>(game, "IsExploreCellCharted", playerX, playerY), "party cell is charted for Region Map focus comparison");
                InvokePrivate<bool>(game, "SetRegionMapFocus", playerX, playerY);
                ExplorationHudView chartedView = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
                Assert(!string.Equals(chartedView.ZoneName, "Uncharted", StringComparison.OrdinalIgnoreCase), "charted Region Map focus exposes its known location");
                Point fogProbe = new[]
                    {
                        new Point(0, 0),
                        new Point(state.Map.Width - 1, 0),
                        new Point(0, state.Map.Height - 1),
                        new Point(state.Map.Width - 1, state.Map.Height - 1)
                    }
                    .First(point => !InvokePrivate<bool>(game, "IsExploreCellCharted", point.X, point.Y));
                InvokePrivate<bool>(game, "SetRegionMapFocus", fogProbe.X, fogProbe.Y);
                ExplorationHudView foggedView = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
                Assert(foggedView.ZoneName == "Uncharted"
                    && foggedView.ZoneDetail.IndexOf("travel closer", StringComparison.OrdinalIgnoreCase) >= 0,
                    "fogged Region Map focus gives bounded unknown-terrain feedback without revealing travel state");

                InvokePrivate(
                    game,
                    "ApplyVisualSmokeExploreView",
                    (object)new[] { "-ashen-region-pan-smoke", "-ashen-details-smoke" });
                int smokeFocusX = GetPrivateField<int>(game, "exploreRegionFocusX");
                int smokeFocusY = GetPrivateField<int>(game, "exploreRegionFocusY");
                Assert(GetPrivateField<bool>(game, "exploreWideView")
                    && !GetPrivateField<bool>(game, "exploreHudCollapsed")
                    && (smokeFocusX != playerX || smokeFocusY != playerY),
                    "panned Region Map visual-smoke hook deterministically stages browse focus with Details open");

                InvokePrivate(game, "MarkUiDirty");
                InvokePrivate(game, "LateUpdate");
                Canvas.ForceUpdateCanvases();
                ExplorationHudScreen hud = GetPrivateField<ExplorationHudScreen>(game, "explorationHudScreen");
                bool supportedViewport = Screen.width >= 960 && Screen.height >= 600;
                bool hudExists = hud != null;
                bool hudVisible = hudExists && hud.IsVisible;
                bool hudSuppressed = hudExists && hud.IsSuppressedByImguiFallback;
                bool eventSystemUsable = UiRuntime.HasUsableEventSystem();
                bool hudInteractionOwner = hudExists && hud.IsInteractionOwner;
                bool hudLaidOut = hudExists && hud.HasLaidOutHud;
                bool hudUsable = hudExists && hud.HasUsableHud;
                bool renderableHud = InvokePrivate<bool>(game, "HasRenderableGameplayHud", UiOverlay.None);
                bool needsEmergencyFallback = InvokePrivate<bool>(game, "NeedsEmergencyExplorationHudFallback");
                string hudDiagnostics = $"screen={Screen.width}x{Screen.height}, supported={supportedViewport}, "
                    + $"exists={hudExists}, visible={hudVisible}, suppressed={hudSuppressed}, "
                    + $"eventSystem={eventSystemUsable}, interactionOwner={hudInteractionOwner}, "
                    + $"laidOut={hudLaidOut}, usable={hudUsable}, renderable={renderableHud}, "
                    + $"needsEmergencyFallback={needsEmergencyFallback}";
                Assert(hudExists
                    && hudVisible
                    && !hudSuppressed
                    && eventSystemUsable
                    && hudInteractionOwner,
                    "synchronized Region Map browsing preserves the visible, unsuppressed, interactive one-owner exploration HUD ("
                    + hudDiagnostics + ")");
                if (supportedViewport)
                {
                    Assert(hudLaidOut
                        && hudUsable
                        && renderableHud
                        && !needsEmergencyFallback,
                        "synchronized Region Map browsing preserves usable uGUI geometry without emergency fallback at a supported viewport ("
                        + hudDiagnostics + ")");
                }
                FieldInfo mapButtonField = typeof(ExplorationHudScreen).GetField(
                    "mapButton",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                Button mapButton = mapButtonField == null ? null : mapButtonField.GetValue(hud) as Button;
                GameObject mapControl = mapButton == null ? null : mapButton.gameObject;
                string selectionDiagnostics = $"playing={Application.isPlaying}, currentEventSystem={EventSystem.current != null}, "
                    + $"semanticEventSystem={selectionEventSystem != null}, mapField={mapButtonField != null}, "
                    + $"mapButton={mapButton != null}, mapActive={mapControl != null && mapControl.activeInHierarchy}, "
                    + $"mapInteractable={mapButton != null && mapButton.interactable}";
                Assert(mapButton != null
                    && mapControl.activeInHierarchy
                    && mapButton.interactable
                    && selectionEventSystem != null,
                    "Region Map runtime can probe the live semantic footer selection owner (" + selectionDiagnostics + ")");
                selectionEventSystem.SetSelectedGameObject(mapControl);
                Assert(selectionEventSystem.currentSelectedGameObject == mapControl,
                    "pointer-selected Map footer control owns focus before panning starts ("
                    + selectionDiagnostics + ", selected="
                    + (selectionEventSystem.currentSelectedGameObject == null ? "<none>" : selectionEventSystem.currentSelectedGameObject.name)
                    + ")");
                InvokePrivate(game, "ReleaseRegionMapHudSelection");
                Assert(selectionEventSystem.currentSelectedGameObject == null,
                    "Region Map pan ownership releases footer focus so left stick cannot navigate both layers ("
                    + selectionDiagnostics + ", selected="
                    + (selectionEventSystem.currentSelectedGameObject == null ? "<none>" : selectionEventSystem.currentSelectedGameObject.name)
                    + ")");

                Assert(state.PlayerX == playerX && state.PlayerY == playerY, "Region Map focus, fog focus, and visual smoke never move the party");
                Assert(state.ExplorationSteps == explorationSteps, "Region Map browsing never advances exploration time");
                Assert(state.ActiveRouteWaypointKey == activeWaypoint, "Region Map browsing never changes the active waypoint");
                Assert(state.StoryFlags.SequenceEqual(storyFlags), "Region Map browsing never changes story flags");
                Assert(state.DiscoveredZones.SequenceEqual(chartEntries), "Region Map browsing never charts fogged terrain");
                Assert((state.Map?.Objects?.Count ?? 0) == objectCount, "Region Map browsing never mutates world objects");
            }
            finally
            {
                SetPrivateField(game, "exploreWideView", originalWideView);
                SetPrivateField(game, "exploreHudCollapsed", originalHudCollapsed);
                SetPrivateField(game, "exploreRegionFocusMap", originalFocusMap);
                SetPrivateField(game, "exploreRegionFocusX", originalFocusX);
                SetPrivateField(game, "exploreRegionFocusY", originalFocusY);
                SetPrivateField(game, "exploreRegionHeldAxisX", originalHeldX);
                SetPrivateField(game, "exploreRegionHeldAxisY", originalHeldY);
                SetPrivateField(game, "exploreRegionNextRepeatAt", originalRepeatAt);
                SetPrivateField(game, "exploreRegionPointerDragging", originalPointerDragging);
                SetPrivateField(game, "exploreRegionDragRemainderX", originalDragRemainderX);
                SetPrivateField(game, "exploreRegionDragRemainderY", originalDragRemainderY);
                InvokePrivate(game, "MarkUiDirty");
                InvokePrivate(game, "LateUpdate");
                Canvas.ForceUpdateCanvases();
                if (selectionEventSystem != null)
                {
                    selectionEventSystem.SetSelectedGameObject(
                        selectedBefore != null && selectedBefore.activeInHierarchy ? selectedBefore : null);
                }
            }
        }

        private static void AssertHeldExplorationMovementRuntime(AshenHallsGame game, GameState originalState)
        {
            Assert(originalState?.Map != null, "held exploration movement has a live world-map fixture");
            GameState movementState = JsonUtility.FromJson<GameState>(JsonUtility.ToJson(originalState));
            Assert(movementState?.Map != null, "held exploration movement clones the live map without sharing campaign state");

            System.Random originalRng = GetPrivateField<System.Random>(game, "rng");
            bool originalWideView = GetPrivateField<bool>(game, "exploreWideView");
            int originalFacingX = GetPrivateField<int>(game, "exploreFacingX");
            int originalFacingY = GetPrivateField<int>(game, "exploreFacingY");
            string originalLastExploreRegion = GetPrivateField<string>(game, "lastExploreRegion");
            string originalBannerText = GetPrivateField<string>(game, "bannerText");
            float originalBannerUntil = GetPrivateField<float>(game, "bannerUntil");
            int originalHeldX = GetPrivateField<int>(game, "exploreMovementHeldAxisX");
            int originalHeldY = GetPrivateField<int>(game, "exploreMovementHeldAxisY");
            float originalRepeatAt = GetPrivateField<float>(game, "exploreMovementNextRepeatAt");
            bool originalRequiresNeutral = GetPrivateField<bool>(game, "exploreMovementRequiresNeutral");
            MapData originalRegionFocusMap = GetPrivateField<MapData>(game, "exploreRegionFocusMap");
            int originalRegionFocusX = GetPrivateField<int>(game, "exploreRegionFocusX");
            int originalRegionFocusY = GetPrivateField<int>(game, "exploreRegionFocusY");
            int originalRegionHeldX = GetPrivateField<int>(game, "exploreRegionHeldAxisX");
            int originalRegionHeldY = GetPrivateField<int>(game, "exploreRegionHeldAxisY");
            float originalRegionRepeatAt = GetPrivateField<float>(game, "exploreRegionNextRepeatAt");
            List<Tween> liveTweens = GetPrivateField<List<Tween>>(game, "tweens");
            List<Tween> originalTweens = new List<Tween>(liveTweens);
            EventSystem eventSystem = EventSystem.current
                ?? (!Application.isPlaying ? UiRuntime.EnsureEventSystemReady() : null);
            GameObject selectedBefore = eventSystem == null ? null : eventSystem.currentSelectedGameObject;

            try
            {
                SetPrivateField(game, "state", movementState);
                SetPrivateField(game, "rng", new System.Random(movementState.Seed + movementState.Depth * 101));
                SetPrivateField(game, "exploreWideView", false);
                InvokePrivate(game, "CloseTransientOverlays");
                InvokePrivate(game, "InvalidateControllerCaches");

                Assert(TryFindSafeHeldMovementRun(game, movementState, out Point runStart, out int runX, out int runY),
                    "held exploration movement finds a safe two-step production route");
                movementState.PlayerX = runStart.X;
                movementState.PlayerY = runStart.Y;
                InvokePrivate(game, "InvalidateExplorationController");
                InvokePrivate(game, "ResetExploreMovementInput", false);
                int stepsBefore = movementState.ExplorationSteps;
                ExplorationMovementRepeatStep initial = new ExplorationMovementRepeatStep(
                    runX,
                    runY,
                    runX,
                    runY,
                    10f + ExplorationMovementRepeatRules.InitialRepeatDelay,
                    false);
                Assert(InvokePrivate<bool>(game, "ApplyExploreMovementRepeatStep", initial),
                    "initial held movement action enters the production movement seam");
                Assert(movementState.PlayerX == runStart.X + runX
                    && movementState.PlayerY == runStart.Y + runY
                    && movementState.ExplorationSteps == stepsBefore + 1,
                    "initial held movement advances exactly one tile and one exploration step");
                ExplorationMovementRepeatStep repeated = new ExplorationMovementRepeatStep(
                    runX,
                    runY,
                    runX,
                    runY,
                    10f + ExplorationMovementRepeatRules.InitialRepeatDelay + ExplorationMovementRepeatRules.RepeatInterval,
                    true);
                Assert(InvokePrivate<bool>(game, "ApplyExploreMovementRepeatStep", repeated),
                    "held repeat enters the same production movement seam");
                Assert(movementState.PlayerX == runStart.X + runX * 2
                    && movementState.PlayerY == runStart.Y + runY * 2
                    && movementState.ExplorationSteps == stepsBefore + 2,
                    "held repeat advances one more tile and exactly one more exploration step");
                Assert(!GetPrivateField<bool>(game, "exploreMovementRequiresNeutral"),
                    "successful held travel remains armed for its next cadence step");

                Assert(TryFindBlockedHeldMovementApproach(game, movementState, out Point blockedStart, out int blockedX, out int blockedY),
                    "held exploration movement finds a deterministic terrain collision");
                movementState.PlayerX = blockedStart.X;
                movementState.PlayerY = blockedStart.Y;
                InvokePrivate(game, "InvalidateExplorationController");
                InvokePrivate(game, "ResetExploreMovementInput", false);
                LogEntry logBeforeBlocked = movementState.Log.FirstOrDefault();
                string expectedBlockedLine = InvokePrivate<string>(game, "ExploreBlockedMoveLine", blockedX, blockedY);
                ExplorationMovementRepeatStep initialBlocked = new ExplorationMovementRepeatStep(
                    blockedX,
                    blockedY,
                    blockedX,
                    blockedY,
                    20f + ExplorationMovementRepeatRules.InitialRepeatDelay,
                    false);
                Assert(InvokePrivate<bool>(game, "ApplyExploreMovementRepeatStep", initialBlocked),
                    "initial blocked held action is consumed by the production seam");
                LogEntry reportedBlocked = movementState.Log.FirstOrDefault();
                Assert(reportedBlocked != null
                    && !ReferenceEquals(reportedBlocked, logBeforeBlocked)
                    && reportedBlocked.Text == expectedBlockedLine,
                    "initial collision reports one exact blocked-movement log line");
                AssertHeldMovementRequiresNeutral(game, "initial collision");

                InvokePrivate(game, "ResetExploreMovementInput", false);
                int logCountBeforeSilentRepeat = movementState.Log.Count;
                ExplorationMovementRepeatStep repeatedBlocked = new ExplorationMovementRepeatStep(
                    blockedX,
                    blockedY,
                    blockedX,
                    blockedY,
                    20f + ExplorationMovementRepeatRules.InitialRepeatDelay + ExplorationMovementRepeatRules.RepeatInterval,
                    true);
                Assert(InvokePrivate<bool>(game, "ApplyExploreMovementRepeatStep", repeatedBlocked),
                    "blocked held repeat is consumed without falling through to another command");
                Assert(movementState.PlayerX == blockedStart.X
                    && movementState.PlayerY == blockedStart.Y
                    && movementState.Log.Count == logCountBeforeSilentRepeat
                    && ReferenceEquals(movementState.Log.FirstOrDefault(), reportedBlocked),
                    "blocked held repeat stays in place without spamming the exploration log");
                AssertHeldMovementRequiresNeutral(game, "blocked held repeat");

                Assert(TryFindTownGuardBumpApproach(game, movementState, out Point guardStart, out int guardX, out int guardY),
                    "held exploration movement finds a live adjacent-use guard fixture");
                InvokePrivate(game, "CloseTransientOverlays");
                movementState.PlayerX = guardStart.X;
                movementState.PlayerY = guardStart.Y;
                InvokePrivate(game, "InvalidateExplorationController");
                InvokePrivate(game, "ResetExploreMovementInput", false);
                ExplorationMovementRepeatStep initialBump = new ExplorationMovementRepeatStep(
                    guardX,
                    guardY,
                    guardX,
                    guardY,
                    30f + ExplorationMovementRepeatRules.InitialRepeatDelay,
                    false);
                Assert(InvokePrivate<bool>(game, "ApplyExploreMovementRepeatStep", initialBump),
                    "initial held bump enters the production adjacent-use path");
                Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Dialogue
                    && movementState.PlayerX == guardStart.X
                    && movementState.PlayerY == guardStart.Y,
                    "initial held bump talks to the blocking guard exactly once without overlap");
                LogEntry oneShotGuardLog = movementState.Log.FirstOrDefault();
                int guardLogCount = movementState.Log.Count;
                AssertHeldMovementRequiresNeutral(game, "adjacent bump-use");

                InvokePrivate(game, "ResetExploreMovementInput", false);
                ExplorationMovementRepeatStep repeatedBump = new ExplorationMovementRepeatStep(
                    guardX,
                    guardY,
                    guardX,
                    guardY,
                    30f + ExplorationMovementRepeatRules.InitialRepeatDelay + ExplorationMovementRepeatRules.RepeatInterval,
                    true);
                Assert(InvokePrivate<bool>(game, "ApplyExploreMovementRepeatStep", repeatedBump),
                    "held bump repeat is consumed by the production movement seam");
                Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Dialogue
                    && movementState.PlayerX == guardStart.X
                    && movementState.PlayerY == guardStart.Y
                    && movementState.Log.Count == guardLogCount
                    && ReferenceEquals(movementState.Log.FirstOrDefault(), oneShotGuardLog),
                    "held bump repeat neither re-enters the guard nor emits duplicate dialogue logs");
                AssertHeldMovementRequiresNeutral(game, "held adjacent-use repeat");
                InvokePrivate(game, "CloseTransientOverlays");

                ExplorationHudScreen hud = GetPrivateField<ExplorationHudScreen>(game, "explorationHudScreen");
                FieldInfo mapButtonField = typeof(ExplorationHudScreen).GetField(
                    "mapButton",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                Button mapButton = mapButtonField == null ? null : mapButtonField.GetValue(hud) as Button;
                Assert(eventSystem != null && mapButton != null && mapButton.gameObject.activeInHierarchy,
                    "held exploration movement can probe a live HUD selection owner");
                eventSystem.SetSelectedGameObject(mapButton.gameObject);
                InvokePrivate(game, "ReleaseExploreHudSelection");
                Assert(eventSystem.currentSelectedGameObject == null,
                    "held local travel releases HUD selection before movement owns the direction axis");

                StageHeldMovementState(game, 1, 0, 41f);
                movementState.Map = JsonUtility.FromJson<MapData>(JsonUtility.ToJson(movementState.Map));
                InvokePrivate(game, "InvalidateExplorationController");
                AssertHeldMovementRequiresNeutral(game, "map replacement");

                InvokePrivate(game, "ResetExploreMovementInput", false);
                StageHeldMovementState(game, 0, -1, 42f);
                InvokePrivate(game, "ToggleExploreView");
                Assert(GetPrivateField<bool>(game, "exploreWideView"),
                    "held movement reset probe enters Region Map view");
                AssertHeldMovementRequiresNeutral(game, "Region Map view ownership");
                InvokePrivate(game, "ToggleExploreView");
                Assert(!GetPrivateField<bool>(game, "exploreWideView"),
                    "held movement reset probe returns to Local Map view");
                AssertHeldMovementRequiresNeutral(game, "Local Map view return");

                InvokePrivate(game, "ResetExploreMovementInput", false);
                StageHeldMovementState(game, -1, 0, 43f);
                InvokePrivate(game, "OpenHelpOverlay");
                InvokePrivate(game, "Update");
                Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Help,
                    "Help owns exploration input during the held-movement reset probe");
                AssertHeldMovementRequiresNeutral(game, "overlay input ownership");
                InvokePrivate(game, "CloseHelpOverlay");

                InvokePrivate(game, "ResetExploreMovementInput", false);
                Assert(GetPrivateField<int>(game, "exploreMovementHeldAxisX") == 0
                    && GetPrivateField<int>(game, "exploreMovementHeldAxisY") == 0
                    && Mathf.Approximately(GetPrivateField<float>(game, "exploreMovementNextRepeatAt"), 0f)
                    && !GetPrivateField<bool>(game, "exploreMovementRequiresNeutral"),
                    "neutral release clears held state and re-arms local exploration movement");
            }
            finally
            {
                InvokePrivate(game, "CloseTransientOverlays");
                SetPrivateField(game, "state", originalState);
                SetPrivateField(game, "rng", originalRng);
                SetPrivateField(game, "exploreWideView", originalWideView);
                InvokePrivate(game, "InvalidateControllerCaches");
                SetPrivateField(game, "exploreFacingX", originalFacingX);
                SetPrivateField(game, "exploreFacingY", originalFacingY);
                SetPrivateField(game, "lastExploreRegion", originalLastExploreRegion);
                SetPrivateField(game, "bannerText", originalBannerText);
                SetPrivateField(game, "bannerUntil", originalBannerUntil);
                SetPrivateField(game, "exploreMovementHeldAxisX", originalHeldX);
                SetPrivateField(game, "exploreMovementHeldAxisY", originalHeldY);
                SetPrivateField(game, "exploreMovementNextRepeatAt", originalRepeatAt);
                SetPrivateField(game, "exploreMovementRequiresNeutral", originalRequiresNeutral);
                SetPrivateField(game, "exploreRegionFocusMap", originalRegionFocusMap);
                SetPrivateField(game, "exploreRegionFocusX", originalRegionFocusX);
                SetPrivateField(game, "exploreRegionFocusY", originalRegionFocusY);
                SetPrivateField(game, "exploreRegionHeldAxisX", originalRegionHeldX);
                SetPrivateField(game, "exploreRegionHeldAxisY", originalRegionHeldY);
                SetPrivateField(game, "exploreRegionNextRepeatAt", originalRegionRepeatAt);
                liveTweens.Clear();
                liveTweens.AddRange(originalTweens);
                if (eventSystem != null)
                {
                    eventSystem.SetSelectedGameObject(
                        selectedBefore != null && selectedBefore.activeInHierarchy ? selectedBefore : null);
                }
            }
        }

        private static bool TryFindSafeHeldMovementRun(
            AshenHallsGame game,
            GameState state,
            out Point start,
            out int deltaX,
            out int deltaY)
        {
            int[] dx = { 0, 0, -1, 1 };
            int[] dy = { -1, 1, 0, 0 };
            for (int y = 0; y < state.Map.Height; y++)
            for (int x = 0; x < state.Map.Width; x++)
            for (int direction = 0; direction < dx.Length; direction++)
            {
                if (!IsSafeEmptyExploreCell(game, state, x, y)
                    || !IsSafeEmptyExploreCell(game, state, x + dx[direction], y + dy[direction])
                    || !IsSafeEmptyExploreCell(game, state, x + dx[direction] * 2, y + dy[direction] * 2))
                {
                    continue;
                }

                start = new Point(x, y);
                deltaX = dx[direction];
                deltaY = dy[direction];
                return true;
            }

            start = new Point(0, 0);
            deltaX = 0;
            deltaY = 0;
            return false;
        }

        private static bool IsSafeEmptyExploreCell(AshenHallsGame game, GameState state, int x, int y)
        {
            if (state?.Map == null || x < 0 || y < 0 || x >= state.Map.Width || y >= state.Map.Height) return false;
            if (state.Map.FindObjectAt(x, y) != null || !InvokePrivate<bool>(game, "CanStepExplore", x, y)) return false;
            WorldZone zone = InvokePrivate<WorldZone>(game, "ZoneFor", x, y, state.Map, state.Depth);
            return zone != null && zone.Danger <= 0;
        }

        private static bool TryFindBlockedHeldMovementApproach(
            AshenHallsGame game,
            GameState state,
            out Point start,
            out int deltaX,
            out int deltaY)
        {
            int[] dx = { 0, 0, -1, 1 };
            int[] dy = { -1, 1, 0, 0 };
            for (int y = 0; y < state.Map.Height; y++)
            for (int x = 0; x < state.Map.Width; x++)
            {
                if (!IsSafeEmptyExploreCell(game, state, x, y)) continue;
                for (int direction = 0; direction < dx.Length; direction++)
                {
                    int targetX = x + dx[direction];
                    int targetY = y + dy[direction];
                    if (targetX < 0 || targetY < 0 || targetX >= state.Map.Width || targetY >= state.Map.Height) continue;
                    int index = targetY * state.Map.Width + targetX;
                    if (index < 0 || index >= state.Map.Tiles.Count || state.Map.Tiles[index] == 1) continue;
                    if (state.Map.FindObjectAt(targetX, targetY) != null) continue;
                    start = new Point(x, y);
                    deltaX = dx[direction];
                    deltaY = dy[direction];
                    return true;
                }
            }

            start = new Point(0, 0);
            deltaX = 0;
            deltaY = 0;
            return false;
        }

        private static bool TryFindTownGuardBumpApproach(
            AshenHallsGame game,
            GameState state,
            out Point start,
            out int deltaX,
            out int deltaY)
        {
            int[] dx = { 0, 0, -1, 1 };
            int[] dy = { -1, 1, 0, 0 };
            foreach (MapObject guard in state.Map.Objects.Where(candidate => candidate != null && candidate.Type == ObjectType.TownGuard))
            {
                for (int direction = 0; direction < dx.Length; direction++)
                {
                    int standX = guard.X - dx[direction];
                    int standY = guard.Y - dy[direction];
                    if (!IsSafeEmptyExploreCell(game, state, standX, standY)) continue;
                    start = new Point(standX, standY);
                    deltaX = dx[direction];
                    deltaY = dy[direction];
                    return true;
                }
            }

            start = new Point(0, 0);
            deltaX = 0;
            deltaY = 0;
            return false;
        }

        private static void StageHeldMovementState(AshenHallsGame game, int heldX, int heldY, float nextRepeatAt)
        {
            SetPrivateField(game, "exploreMovementHeldAxisX", heldX);
            SetPrivateField(game, "exploreMovementHeldAxisY", heldY);
            SetPrivateField(game, "exploreMovementNextRepeatAt", nextRepeatAt);
            SetPrivateField(game, "exploreMovementRequiresNeutral", false);
        }

        private static void AssertHeldMovementRequiresNeutral(AshenHallsGame game, string context)
        {
            Assert(GetPrivateField<int>(game, "exploreMovementHeldAxisX") == 0
                && GetPrivateField<int>(game, "exploreMovementHeldAxisY") == 0
                && Mathf.Approximately(GetPrivateField<float>(game, "exploreMovementNextRepeatAt"), 0f)
                && GetPrivateField<bool>(game, "exploreMovementRequiresNeutral"),
                context + " clears held movement cadence and requires a neutral release");
        }

        private static void AssertExplorationWorldMapRuntime(AshenHallsGame game)
        {
            GameState state = GetPrivateField<GameState>(game, "state");
            AssertRegionMapBrowsingRuntime(game, state);
            AssertHeldExplorationMovementRuntime(game, state);
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
                "riftpounce", "riftpounceimpact", "abyssalwhirl", "abyssalwhirlimpact",
                "soulrend", "soulrendimpact", "dreadroar", "dreadroarimpact",
                "stealth", "smoke", "rally", "aimedshot", "pinning", "volley", "scoutmark", "arrowrain", "mark",
                "bladecontact", "thrustcontact", "heavycontact", "arrowcontact", "woodcontact", "stonecontact", "wayfind",
                "footglass", "footmud", "footash", "footgravel",
                "servicecoin", "servicearmor", "serviceweapon", "serviceenchant",
                "doorwood", "doorroyal", "thronechime", "shopbell",
                "uiopen", "uiclose", "uiconfirm", "uitab", "itemequip", "itemtake", "elixir", "rest", "levelup",
                "titleforge", "titlereveal", "titlefocus", "titleconfirm", "titleopen", "titleclose",
                "combatstep", "combatguard", "combatturn", "combatcrit",
                "arrowrelease", "thrust", "spell", "fire",
                "combatambsteel", "combatambsewer", "combatambarcane",
                "ambrain", "ambtavern", "ambhearth",
                "ambcity", "ambbell", "ambmarket", "ambforge", "ambgate", "ambdrip", "ambwind", "ambdrum", "ambstone",
                "ambgrove", "ambfen", "ambglass", "ambruin", "ambcave", "ambcamp"
            })
            {
                Assert(soundClips.ContainsKey(key) && soundClips[key] != null, "resolved audio bank contains " + key);
            }
            foreach (string key in new[] { "ui", "uiopen", "titleforge", "titlereveal", "titlefocus", "titleconfirm", "titleopen", "titleclose", "combatstep", "combatguard", "combatturn", "combatcrit", "arrowrelease", "thrust", "spell", "fire", "combatambsteel", "combatambsewer", "combatambarcane", "itemequip", "elixir", "levelup", "footglass", "footmud", "heavycontact", "castember", "castfrost", "charge", "whirlwind", "aimedshot", "arrowrain", "riftpounce", "abyssalwhirl", "soulrend", "dreadroar" })
            {
                Assert(soundClips[key].frequency >= 32000, key + " uses the v1.70 high-resolution audio path");
                Assert(AudioClipHasHealthyHeadroom(soundClips[key]), key + " has audible body, finite samples, and clean output headroom");
            }
            HashSet<string> importedSfxKeys = GetPrivateField<HashSet<string>>(game, "importedSfxKeys");
            AudioClip[] importedSfx = Resources.LoadAll<AudioClip>("Audio/Sfx");
            Assert(importedSfx.Length == 161, "authored SFX resource bank contains 55 curated and 106 original cues");
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
                && importedSfxKeys.Contains("riftseal")
                && importedSfxKeys.Contains("riftpounce")
                && importedSfxKeys.Contains("abyssalwhirl")
                && importedSfxKeys.Contains("soulrend")
                && importedSfxKeys.Contains("dreadroar")
                && importedSfxKeys.Contains(TitleAudioRules.RevealStrikeKey)
                && importedSfxKeys.Contains(TitleAudioRules.RevealChimeKey)
                && importedSfxKeys.Contains(TitleAudioRules.FocusKey)
                && importedSfxKeys.Contains(TitleAudioRules.ConfirmKey)
                && importedSfxKeys.Contains(TitleAudioRules.OpenKey)
                && importedSfxKeys.Contains(TitleAudioRules.CloseKey)
                && importedSfxKeys.Contains(CombatAudioMixRules.StepCue)
                && importedSfxKeys.Contains(CombatAudioMixRules.GuardCue)
                && importedSfxKeys.Contains(CombatAudioMixRules.TurnCue)
                && importedSfxKeys.Contains(CombatAudioMixRules.CriticalCue)
                && importedSfxKeys.Contains(CombatAudioMixRules.SteelAmbienceCue)
                && importedSfxKeys.Contains(CombatAudioMixRules.SewerAmbienceCue)
                && importedSfxKeys.Contains(CombatAudioMixRules.ArcaneAmbienceCue)
                && importedSfxKeys.Contains("arrowrelease")
                && importedSfxKeys.Contains("thrust")
                && importedSfxKeys.Contains("spell")
                && importedSfxKeys.Contains("fire"),
                "core cast schools, signature magic, Demon Arts, title feedback, and combat feedback use original masters");
            HashSet<string> importedMusicKeys = GetPrivateField<HashSet<string>>(game, "importedMusicKeys");
            Dictionary<string, AudioClip> importedMusicClips = GetPrivateField<Dictionary<string, AudioClip>>(game, "importedMusicClips");
            AudioClip[] importedMusic = Resources.LoadAll<AudioClip>("Audio/Music");
            Assert(importedMusic.Length == 54, "original music resource bank contains all 54 routed score contexts");
            Assert(importedMusicKeys.Count == importedMusic.Length, "every original music master passes the runtime metadata contract");
            Assert(importedMusicClips.Count == importedMusic.Length, "every original music master is indexed by exact clip name");
            foreach (AudioClip clip in importedMusic)
            {
                string key = clip.name.ToLowerInvariant();
                Assert(importedMusicKeys.Contains(key), key + " is recorded as an original music master");
                Assert(importedMusicClips.TryGetValue(key, out AudioClip indexed) && indexed == clip, key + " resolves through the music override bank");
                Assert(clip.loadState != AudioDataLoadState.Failed, key + " music data loads");
                Assert(clip.frequency == 32000 && clip.channels == 2, key + " is mastered as 32 kHz stereo");
                float maximumDuration = key == "tavern_storm_hearth_ensemble_loop" ? 60.1f : 30.1f;
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
            Assert(tavernMusic != null && tavernMusic.length >= 59.9f && tavernMusic.length <= 60.1f, "title mode owns the complete 60-second Ash & Brimstone overture");
            Assert(importedMusic.Contains(tavernMusic), "title mode uses its original stereo master instead of the procedural fallback");
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
                MusicDirectorRules.Muster, MusicDirectorRules.Victory, MusicDirectorRules.Defeat, MusicDirectorRules.GrandHearth,
                MusicDirectorRules.WorldMapOverview,
                MusicDirectorRules.GreenShrineTrainingRing, MusicDirectorRules.OldQuarryForge,
                MusicDirectorRules.GloamDeepCrypt, MusicDirectorRules.GlassLoreLibrary,
                MusicDirectorRules.DuskMarketHideout, MusicDirectorRules.RedGateSeal,
                MusicDirectorRules.SaltCisternGate, MusicDirectorRules.AshFenAncientGrove,
                MusicDirectorRules.MidgaardTemple, MusicDirectorRules.MidgaardMarket, MusicDirectorRules.MidgaardTavernLane,
                MusicDirectorRules.MidgaardGateWatch, MusicDirectorRules.MidgaardCisternMouth, MusicDirectorRules.MidgaardRoyalApproach,
                MusicDirectorRules.MidgaardRoad, MusicDirectorRules.RoadsideRest, MusicDirectorRules.SacredGround,
                MusicDirectorRules.UnderstoneThreshold, MusicDirectorRules.ForgottenRuins, MusicDirectorRules.ArcaneThreshold,
                MusicDirectorRules.HuntedRoad, MusicDirectorRules.AncientGrove, MusicDirectorRules.FactionCamp,
                MusicDirectorRules.CombatRatfolk, MusicDirectorRules.CombatArcaneDuel, MusicDirectorRules.CombatElite,
                MusicDirectorRules.CombatLastStand, MusicDirectorRules.CombatKoboldKing, MusicDirectorRules.CombatDemonLord
            };
            Assert(adaptiveMusicFactories.Count == expandedMusicKeys.Length, "adaptive score registers exactly 34 additional themes");
            Assert(expandedMusicKeys.All(adaptiveMusicFactories.ContainsKey), "every expanded score key has a lazy composition factory");
            Assert(adaptiveMusicClips.Count < adaptiveMusicFactories.Count, "procedural fallback score remains lazy when original masters are present");
            foreach (string musicKey in expandedMusicKeys)
            {
                AudioClip routed = InvokePrivate<AudioClip>(game, "MusicClipForKey", musicKey);
                Assert(routed != null && importedMusic.Contains(routed), musicKey + " resolves to an original music master");
            }

            AudioClip musterMusic = InvokePrivate<AudioClip>(game, "MusicClipForKey", MusicDirectorRules.Muster);
            AudioClip grandHearthMusic = InvokePrivate<AudioClip>(game, "MusicClipForKey", MusicDirectorRules.GrandHearth);
            AudioClip worldMapMusic = InvokePrivate<AudioClip>(game, "MusicClipForKey", MusicDirectorRules.WorldMapOverview);
            AudioClip greenShrineMusic = InvokePrivate<AudioClip>(game, "MusicClipForKey", MusicDirectorRules.GreenShrineTrainingRing);
            AudioClip quarryForgeMusic = InvokePrivate<AudioClip>(game, "MusicClipForKey", MusicDirectorRules.OldQuarryForge);
            AudioClip deepCryptMusic = InvokePrivate<AudioClip>(game, "MusicClipForKey", MusicDirectorRules.GloamDeepCrypt);
            AudioClip glassLibraryMusic = InvokePrivate<AudioClip>(game, "MusicClipForKey", MusicDirectorRules.GlassLoreLibrary);
            AudioClip duskHideoutMusic = InvokePrivate<AudioClip>(game, "MusicClipForKey", MusicDirectorRules.DuskMarketHideout);
            AudioClip redGateSealMusic = InvokePrivate<AudioClip>(game, "MusicClipForKey", MusicDirectorRules.RedGateSeal);
            AudioClip saltGateMusic = InvokePrivate<AudioClip>(game, "MusicClipForKey", MusicDirectorRules.SaltCisternGate);
            AudioClip ashFenGroveMusic = InvokePrivate<AudioClip>(game, "MusicClipForKey", MusicDirectorRules.AshFenAncientGrove);
            AudioClip templeMusic = InvokePrivate<AudioClip>(game, "MusicClipForKey", MusicDirectorRules.MidgaardTemple);
            AudioClip pursuitMusic = InvokePrivate<AudioClip>(game, "MusicClipForKey", MusicDirectorRules.HuntedRoad);
            AudioClip ratfolkMusic = InvokePrivate<AudioClip>(game, "MusicClipForKey", MusicDirectorRules.CombatRatfolk);
            AudioClip kingMusic = InvokePrivate<AudioClip>(game, "MusicClipForKey", MusicDirectorRules.CombatKoboldKing);
            AudioClip victoryMusic = InvokePrivate<AudioClip>(game, "MusicClipForKey", MusicDirectorRules.Victory);
            AudioClip defeatMusic = InvokePrivate<AudioClip>(game, "MusicClipForKey", MusicDirectorRules.Defeat);
            Assert(musterMusic != null && musterMusic.name == "muster_by_firelight_loop", "Party Setup owns its quieter firelight theme");
            Assert(grandHearthMusic != null && grandHearthMusic.name == "four_names_by_the_fire_loop", "Grand Hearth owns the quieter main-theme reprise");
            Assert(worldMapMusic != null && worldMapMusic.name == "ashen_atlas_overview_loop", "Region Map owns the original Ashen Atlas overview score");
            Assert(greenShrineMusic != null && greenShrineMusic.name == "sparks_on_the_oathring_loop", "Green Shrine training ring owns its oath-ring score");
            Assert(quarryForgeMusic != null && quarryForgeMusic.name == "anvil_echoes_in_old_stone_loop", "Old Quarry forge owns its anvil score");
            Assert(deepCryptMusic != null && deepCryptMusic.name == "the_crypt_keeps_its_names_loop", "Gloam Deep crypt owns its memorial score");
            Assert(glassLibraryMusic != null && glassLibraryMusic.name == "starlight_in_the_glass_index_loop", "Glass Lore library owns its crystalline score");
            Assert(duskHideoutMusic != null && duskHideoutMusic.name == "lanterns_under_false_names_loop", "Dusk Market hideout owns its covert score");
            Assert(redGateSealMusic != null && redGateSealMusic.name == "embers_at_the_broken_seal_loop", "Red Gate seal owns its omen score");
            Assert(saltGateMusic != null && saltGateMusic.name == "chains_below_bellstone_loop", "Salt Cistern gate owns its understone score");
            Assert(ashFenGroveMusic != null && ashFenGroveMusic.name == "old_sap_under_ash_loop", "Ash Fen grove owns its living-wood score");
            Assert(templeMusic != null && templeMusic.name == "bells_over_temple_square_loop", "Temple Square uses its original bell score");
            Assert(pursuitMusic != null && pursuitMusic.name == "footsteps_behind_loop", "alerted patrols own a pursuit score");
            Assert(ratfolkMusic != null && ratfolkMusic.name == "ratfolk_plague_march_loop", "ratfolk combat owns a plague march");
            Assert(kingMusic != null && kingMusic.name == "crooked_crown_kobold_king_loop", "Kobold King owns a dedicated battle score");
            Assert(victoryMusic != null && defeatMusic != null, "Victory and Defeat no longer fall silent");
            Assert(importedMusic.Contains(musterMusic)
                && importedMusic.Contains(grandHearthMusic)
                && importedMusic.Contains(worldMapMusic)
                && importedMusic.Contains(greenShrineMusic)
                && importedMusic.Contains(quarryForgeMusic)
                && importedMusic.Contains(deepCryptMusic)
                && importedMusic.Contains(glassLibraryMusic)
                && importedMusic.Contains(duskHideoutMusic)
                && importedMusic.Contains(redGateSealMusic)
                && importedMusic.Contains(saltGateMusic)
                && importedMusic.Contains(ashFenGroveMusic)
                && importedMusic.Contains(templeMusic)
                && importedMusic.Contains(pursuitMusic)
                && importedMusic.Contains(ratfolkMusic)
                && importedMusic.Contains(kingMusic)
                && importedMusic.Contains(victoryMusic)
                && importedMusic.Contains(defeatMusic),
                "player-state, exploration, pursuit, and combat contexts choose original music masters");
            Assert(AudioClipsDiffer(musterMusic, templeMusic), "muster and temple themes use distinct arrangements");
            Assert(AudioClipsDiffer(worldMapMusic, grandHearthMusic), "World Map and Grand Hearth use distinct arrangements");
            Assert(AudioClipsDiffer(worldMapMusic, pursuitMusic), "World Map and pursuit retain distinct travel identities");
            Assert(AudioClipsDiffer(worldMapMusic, combatMusic), "World Map and combat retain distinct rhythmic identities");
            Assert(AudioClipsDiffer(pursuitMusic, ratfolkMusic), "pursuit and ratfolk battle themes use distinct rhythms");
            Assert(AudioClipsDiffer(kingMusic, bossCombatMusic), "Kobold King score differs from the generic boss theme");
            Assert(AudioClipsDiffer(victoryMusic, defeatMusic), "Victory and Defeat have opposite musical identities");

            CombatState musicProbe = new CombatState();
            Assert(
                InvokePrivate<string>(game, "ResolveCombatMusicPresentationKey", musicProbe, MusicDirectorRules.CombatGeneric, 40, 40, 0f) == MusicDirectorRules.CombatGeneric,
                "live combat music may begin on the bootstrap generic cue");
            Assert(
                InvokePrivate<string>(game, "ResolveCombatMusicPresentationKey", musicProbe, MusicDirectorRules.CombatDrow, 40, 40, 0.10f) == MusicDirectorRules.CombatDrow,
                "live combat music promotes the bootstrap cue to the first authored faction identity");
            Assert(
                InvokePrivate<string>(game, "ResolveCombatMusicPresentationKey", musicProbe, MusicDirectorRules.CombatKobold, 40, 40, 1f) == MusicDirectorRules.CombatDrow,
                "live combat music remains stable through later roster changes");
            Assert(
                InvokePrivate<string>(game, "ResolveCombatMusicPresentationKey", musicProbe, MusicDirectorRules.CombatDrow, 10, 40, 6.20f) == MusicDirectorRules.CombatDrow,
                "live combat music waits for sustained critical health");
            Assert(
                InvokePrivate<string>(game, "ResolveCombatMusicPresentationKey", musicProbe, MusicDirectorRules.CombatDrow, 10, 40, 7.20f) == MusicDirectorRules.CombatLastStand,
                "live combat music enters last stand after its hold and dwell gates");
            Assert(
                InvokePrivate<string>(game, "ResolveCombatMusicPresentationKey", musicProbe, MusicDirectorRules.CombatDrow, 0, 40, 10f) == MusicDirectorRules.CombatLastStand,
                "a defeated party cannot falsely recover the battle score");
            Assert(
                InvokePrivate<string>(game, "ResolveCombatMusicPresentationKey", musicProbe, MusicDirectorRules.CombatDrow, 22, 40, 12.20f) == MusicDirectorRules.CombatLastStand,
                "last-stand music retains its exit hysteresis and minimum dwell");
            Assert(
                InvokePrivate<string>(game, "ResolveCombatMusicPresentationKey", musicProbe, MusicDirectorRules.CombatDrow, 22, 40, 16.20f) == MusicDirectorRules.CombatDrow,
                "live combat music returns to the encounter identity after sustained recovery");

            CombatState bossMusicProbe = new CombatState();
            Assert(
                InvokePrivate<string>(game, "ResolveCombatMusicPresentationKey", bossMusicProbe, MusicDirectorRules.CombatBoss, 10, 40, 17f) == MusicDirectorRules.CombatBoss,
                "boss combat establishes its authored score");
            Assert(
                InvokePrivate<string>(game, "ResolveCombatMusicPresentationKey", bossMusicProbe, MusicDirectorRules.CombatGeneric, 10, 40, 30f) == MusicDirectorRules.CombatBoss,
                "boss music ignores both roster churn and last-stand pressure");
            CombatState resetMusicProbe = new CombatState();
            Assert(
                InvokePrivate<string>(game, "ResolveCombatMusicPresentationKey", resetMusicProbe, MusicDirectorRules.CombatKobold, 40, 40, 31f) == MusicDirectorRules.CombatKobold,
                "a new encounter resets combat music identity and hysteresis state");
            InvokePrivate(game, "ResetCombatMusicPresentationState");

            InvokePrivate(game, "ResetExplorationMusicPresentationState");
            Assert(
                InvokePrivate<string>(game, "ResolveExplorationMusicPresentationKey", MusicDirectorRules.MidgaardRoad, 100f) == MusicDirectorRules.MidgaardRoad,
                "live exploration music establishes its initial route immediately");
            Assert(
                InvokePrivate<string>(game, "ResolveExplorationMusicPresentationKey", MusicDirectorRules.MidgaardTemple, 101f) == MusicDirectorRules.MidgaardRoad,
                "live exploration music ignores a brief landmark boundary crossing");
            Assert(
                InvokePrivate<string>(game, "ResolveExplorationMusicPresentationKey", MusicDirectorRules.MidgaardTemple, 109f) == MusicDirectorRules.MidgaardTemple,
                "live exploration music promotes a stable calm route after hold and dwell");
            Assert(
                InvokePrivate<string>(game, "ResolveExplorationMusicPresentationKey", MusicDirectorRules.HuntedRoad, 109.1f) == MusicDirectorRules.HuntedRoad,
                "live pursuit music enters immediately");
            Assert(
                InvokePrivate<string>(game, "ResolveExplorationMusicPresentationKey", MusicDirectorRules.WorldMapOverview, 110f) == MusicDirectorRules.HuntedRoad,
                "live World Map route cannot preempt active pursuit");
            Assert(
                InvokePrivate<string>(game, "ResolveExplorationMusicPresentationKey", MusicDirectorRules.WorldMapOverview, 113.1f) == MusicDirectorRules.WorldMapOverview,
                "live pursuit releases to the stable World Map route after its hold");
            InvokePrivate(game, "ResetExplorationMusicPresentationState");

            GameMode modeBeforeMusicProbe = state.Mode;
            bool wideViewBeforeMusicProbe = GetPrivateField<bool>(game, "exploreWideView");
            state.Mode = GameMode.Tavern;
            Assert(InvokePrivate<AudioClip>(game, "DesiredMusicClip").name == "tavern_storm_hearth_ensemble_loop", "live director routes the iconic main-title overture");
            state.Mode = GameMode.Muster;
            Assert(InvokePrivate<AudioClip>(game, "DesiredMusicClip").name == "muster_by_firelight_loop", "live director routes Party Setup music");
            state.Mode = GameMode.Victory;
            Assert(InvokePrivate<AudioClip>(game, "DesiredMusicClip").name == "embers_carry_home_victory_loop", "live director routes Victory music");
            state.Mode = GameMode.Defeat;
            Assert(InvokePrivate<AudioClip>(game, "DesiredMusicClip").name == "ashes_on_the_road_defeat_loop", "live director routes Defeat music");
            List<RoamingThreat> musicProbeThreats = state.RoamingThreats == null
                ? new List<RoamingThreat>()
                : state.RoamingThreats.Where(threat => threat != null).ToList();
            bool[] musicProbeAlertStates = musicProbeThreats.Select(threat => threat.Alerted).ToArray();
            foreach (RoamingThreat threat in musicProbeThreats) threat.Alerted = false;
            state.Mode = GameMode.Explore;
            SetPrivateField(game, "exploreWideView", true);
            InvokePrivate(game, "ResetExplorationMusicPresentationState");
            Assert(InvokePrivate<AudioClip>(game, "DesiredMusicClip") == worldMapMusic, "live Region Map routes the dedicated Ashen Atlas score");
            for (int index = 0; index < musicProbeThreats.Count; index++) musicProbeThreats[index].Alerted = musicProbeAlertStates[index];
            SetPrivateField(game, "exploreWideView", wideViewBeforeMusicProbe);
            state.Mode = modeBeforeMusicProbe;
            InvokePrivate(game, "ResetExplorationMusicPresentationState");
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
            Assert(AudioClipsDiffer(soundClips["riftpounce"], soundClips["abyssalwhirl"]), "rift pounce and abyssal whirl use distinct demon release waveforms");
            Assert(AudioClipsDiffer(soundClips["soulrendimpact"], soundClips["dreadroarimpact"]), "soul rend and dread roar use distinct demon impact waveforms");
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
            Texture2D roadSurfaceAtlas = GetPrivateField<Texture2D>(game, "midgaardRoadSurfaceAtlas");
            Texture2D ambientCitizenAtlas = GetPrivateField<Texture2D>(game, "worldNpcCitizenAtlas");
            Texture2D interiorPropAtlas = GetPrivateField<Texture2D>(game, "midgaardInteriorPropAtlas");
            Texture2D interiorTileAtlas = GetPrivateField<Texture2D>(game, "midgaardInteriorTileAtlas");
            Texture2D gateAtlas = GetPrivateField<Texture2D>(game, "midgaardGateAtlas");
            Texture2D wallAtlas = GetPrivateField<Texture2D>(game, "midgaardWallAtlas");
            Texture2D midgaardTileAtlas = GetPrivateField<Texture2D>(game, "midgaardTileAtlas");
            Texture2D midgaardTownAtlas = GetPrivateField<Texture2D>(game, "midgaardTownAtlas");
            Texture2D cityNpcAtlas = GetPrivateField<Texture2D>(game, "midgaardNpcAtlas");
            Texture2D npcPortraitAtlas = GetPrivateField<Texture2D>(game, "npcPortraitAtlas");
            Texture2D characterSpriteAtlas = GetPrivateField<Texture2D>(game, "characterCombatAtlas");
            Texture2D enemySpriteAtlas = GetPrivateField<Texture2D>(game, "enemySpriteAtlas");
            Texture2D titleCardAtlas = GetPrivateField<Texture2D>(game, "titleCardArt");
            Texture2D gameIconAtlas = GetPrivateField<Texture2D>(game, "gameIconArt");
            Texture2D splashAtlas = GetPrivateField<Texture2D>(game, "splashArt");
            Texture2D tavernBackdropAtlas = GetPrivateField<Texture2D>(game, "tavernBackdropArt");
            Texture2D tavernUiAtlas = GetPrivateField<Texture2D>(game, "tavernUiAtlas");
            Texture2D titleMenuScrollAtlas = GetPrivateField<Texture2D>(game, "titleMenuScrollArt");
            Texture2D titleMenuFocusAtlas = GetPrivateField<Texture2D>(game, "titleMenuFocusArt");
            Texture2D titleMenuIconAtlas = GetPrivateField<Texture2D>(game, "titleMenuIconAtlas");
            Texture2D roamingThreatAtlas = GetPrivateField<Texture2D>(game, "roamingThreatAtlas");
            Texture2D regionalLandmarkAtlas = GetPrivateField<Texture2D>(game, "worldMapRegionLandmarkAtlas");
            Texture2D areaSetpieceAtlas = GetPrivateField<Texture2D>(game, "worldAreaSetpieceAtlas");
            Assert(streetLifeAtlas != null, "v1.50 Midgaard street-life atlas is loaded");
            Assert(streetLifeAtlas.name.IndexOf("v1.50.0", StringComparison.OrdinalIgnoreCase) >= 0, "Midgaard street life uses the pinned v1.50 art contract");
            Assert(streetLifeAtlas.width == 1400 && streetLifeAtlas.height == 1120, "Midgaard street-life atlas is an exact 5x4 grid");
            Assert(pavingDecalAtlas != null, "v1.50 Midgaard paving-decal atlas is loaded");
            Assert(pavingDecalAtlas.name.IndexOf("v1.50.0", StringComparison.OrdinalIgnoreCase) >= 0, "Midgaard paving details use the pinned v1.50 art contract");
            Assert(pavingDecalAtlas.width == 1252 && pavingDecalAtlas.height == 1252, "Midgaard paving-decal atlas is an exact 4x4 grid");
            Assert(roadSurfaceAtlas != null, "v2.21 Midgaard road-surface atlas is loaded");
            Assert(roadSurfaceAtlas.name == RuntimeArtManifest.MidgaardRoadSurfaceAtlas, "Midgaard roads use the exact approved v2.21 material atlas");
            Assert(roadSurfaceAtlas.width == 512 && roadSurfaceAtlas.height == 512, "Midgaard road surfaces use the exact 2x2 grid");
            Assert(ambientCitizenAtlas != null, "v2.21 ambient-citizen atlas is loaded");
            Assert(ambientCitizenAtlas.name == RuntimeArtManifest.WorldNpcCitizenAtlas, "ambient citizens use the exact approved v2.21 art contract");
            Assert(ambientCitizenAtlas.width == 1536 && ambientCitizenAtlas.height == 768, "ambient citizens use the exact 4x2 grid");
            Assert(roamingThreatAtlas != null, "v1.62 roaming-threat atlas is loaded");
            Assert(roamingThreatAtlas.name.IndexOf("v1.62.0", StringComparison.OrdinalIgnoreCase) >= 0, "roaming patrols use the pinned v1.62 art contract");
            Assert(roamingThreatAtlas.width == 1400 && roamingThreatAtlas.height == 1120, "roaming-threat atlas is an exact 5x4 grid");
            Assert(regionalLandmarkAtlas != null, "v1.65 regional-landmark atlas is loaded");
            Assert(regionalLandmarkAtlas.name.IndexOf("v1.65.0", StringComparison.OrdinalIgnoreCase) >= 0, "regional landmarks use the pinned v1.65 art contract");
            Assert(regionalLandmarkAtlas.width == 1400 && regionalLandmarkAtlas.height == 1120, "regional-landmark atlas is an exact 5x4 grid");
            Assert(areaSetpieceAtlas != null, "v2.3 world-area set-piece atlas is loaded");
            Assert(areaSetpieceAtlas.name.IndexOf("v2.3.0", StringComparison.OrdinalIgnoreCase) >= 0, "authored world areas use the pinned v2.3 set-piece contract");
            Assert(areaSetpieceAtlas.width == 1536 && areaSetpieceAtlas.height == 768, "world-area set-piece atlas is an exact 4x2 grid");
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
            Assert(midgaardTownAtlas != null, "v2.21 Midgaard town atlas is loaded");
            Assert(midgaardTownAtlas.name.IndexOf("v2.21.0", StringComparison.OrdinalIgnoreCase) >= 0, "Midgaard buildings use the approved architectural v2.21 art contract");
            Assert(midgaardTownAtlas.width == 1280 && midgaardTownAtlas.height == 1024, "Midgaard town atlas is an exact 5x4 grid");
            Assert(cityNpcAtlas != null, "v2.21 Midgaard NPC atlas is loaded");
            Assert(cityNpcAtlas.name.IndexOf("v2.21.0", StringComparison.OrdinalIgnoreCase) >= 0, "named Midgaard NPCs use the approved coherent v2.21 art contract");
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
            Assert(tavernBackdropAtlas != null, "Grand Hearth title painting is loaded");
            Assert(tavernBackdropAtlas.name.IndexOf("v2.4.0", StringComparison.OrdinalIgnoreCase) >= 0, "title screen uses the pinned Grand Hearth v2.4 painting");
            Assert(tavernBackdropAtlas.width == 1672 && tavernBackdropAtlas.height == 941, "Grand Hearth title painting keeps its authored widescreen canvas");
            Assert(splashAtlas != null && splashAtlas == tavernBackdropAtlas, "startup uses the same current tavern art in editor and packaged player");
            Assert(tavernUiAtlas != null, "Grand Hearth fireplace and fallback icon atlas is loaded");
            Assert(tavernUiAtlas.name.IndexOf("v1.5.9", StringComparison.OrdinalIgnoreCase) >= 0, "the playable hearth and title fallback retain the pinned tavern icon contract");
            Assert(tavernUiAtlas.width == 1402 && tavernUiAtlas.height == 1122, "tavern icon atlas keeps its approved dimensions");
            Assert(titleMenuScrollAtlas != null && titleMenuScrollAtlas.name == RuntimeArtManifest.TitleMenuScroll, "title menu loads the pinned Ashen Road charter");
            Assert(titleMenuScrollAtlas.width == 1280 && titleMenuScrollAtlas.height == 1280, "title charter keeps its exact square nine-slice source");
            Assert(titleMenuScrollAtlas.mipmapCount == 1, "title charter avoids blurry UI mipmaps");
            Assert(titleMenuScrollAtlas.filterMode == FilterMode.Bilinear && titleMenuScrollAtlas.wrapMode == TextureWrapMode.Clamp, "title charter uses smooth clamped UI sampling");
            Assert(titleMenuFocusAtlas != null && titleMenuFocusAtlas.name == RuntimeArtManifest.TitleMenuFocus, "title menu loads the pinned focused-row ribbon");
            Assert(titleMenuFocusAtlas.width == 2048 && titleMenuFocusAtlas.height == 768, "title focus ribbon keeps its exact wide authoring source");
            Assert(titleMenuFocusAtlas.mipmapCount == 1, "title focus ribbon avoids blurry UI mipmaps");
            Assert(titleMenuFocusAtlas.filterMode == FilterMode.Bilinear && titleMenuFocusAtlas.wrapMode == TextureWrapMode.Clamp, "title focus ribbon uses smooth clamped UI sampling");
            Assert(titleMenuIconAtlas != null && titleMenuIconAtlas.name == RuntimeArtManifest.TitleMenuIconAtlas, "title menu loads the pinned purpose-built glyph strip");
            Assert(TitleScreenPresentationRules.SupportsMenuIconArt(titleMenuIconAtlas), "title glyph strip keeps its exact 5x1 runtime geometry");
            Assert(titleMenuIconAtlas.mipmapCount == 1, "small title glyphs avoid blurry UI mipmaps");
            Assert(titleMenuIconAtlas.filterMode == FilterMode.Bilinear && titleMenuIconAtlas.wrapMode == TextureWrapMode.Clamp, "title glyph strip uses smooth clamped UI sampling");
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
            AssertRegionalSiteInteractionsRuntime(game, state);
            AssertRegionalSiteAudioRuntime(game, state, soundClips);
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
            AssertLegacyRegionalSitePresentationGuards(game, state);
            Assert(InvokePrivate<bool>(game, "CanStepExplore", state.PlayerX, state.PlayerY), "party starts on a standable exploration tile");
            Point grandHearthSpawn = MidgaardInteriorRules.GrandHearthSpawn(state.Map);
            Assert(state.PlayerX == grandHearthSpawn.X && state.PlayerY == grandHearthSpawn.Y, "fresh party starts at the authored Grand Hearth company mark");
            WorldZone startingZone = InvokePrivate<WorldZone>(game, "ZoneFor", state.PlayerX, state.PlayerY, state.Map, state.Depth);
            Assert(startingZone != null && startingZone.Id == "midgaard-grand-hearth", "fresh party starts inside the dedicated Grand Hearth zone");
            Assert(startingZone != null && startingZone.Name == MidgaardInteriorRules.GrandHearthDisplayName,
                "fresh party sees the Grand Hearth framed as Town Hall");
            Assert(state.StoryFlags.Contains(StoryFlags.MidgaardGrandHearthEntered), "fresh party records its Grand Hearth arrival");
            Assert(!state.StoryFlags.Contains(StoryFlags.MidgaardGrandHearthDeparted),
                "the journey remains unstarted until the party leaves Town Hall");
            PartyMember defeatedProbe = state.Party[0];
            int defeatedProbeHp = defeatedProbe.Hp;
            defeatedProbe.Hp = 0;
            InvokePrivate(game, "RecalculateMember", defeatedProbe);
            Assert(defeatedProbe.Hp == 0, "derived-stat recalculation preserves a defeated party member");
            defeatedProbe.Hp = defeatedProbeHp;
            Assert(InvokePrivate<int>(game, "ReachableExploreTileCount", state.PlayerX, state.PlayerY) >= 12, "party starts in a useful reachable exploration component");
            Assert(InvokePrivate<bool>(game, "ReachableExploreHasUsefulTarget", state.PlayerX, state.PlayerY), "party starts with a reachable useful exploration target");
            MapObject grandHearthExit = MidgaardInteriorRules.FindById(state.Map, MidgaardInteriorRules.GrandHearthExitId);
            MapObject grandHearthFire = MidgaardInteriorRules.FindById(state.Map, MidgaardInteriorRules.GrandHearthFireId);
            Assert(grandHearthExit != null && grandHearthFire != null, "Grand Hearth has permanent storm doors and a named hearth landmark");
            Assert(InvokePrivate<bool>(game, "TryUseMidgaardPortal", grandHearthExit), "Grand Hearth storm doors open onto Midgaard");
            Assert(InvokePrivate<bool>(game, "IsMidgaardCityCell", state.PlayerX, state.PlayerY, state.Map, state.Depth), "Grand Hearth exit lands safely inside Midgaard");
            Assert(state.StoryFlags.Contains(StoryFlags.MidgaardGrandHearthDeparted), "first departure from the Grand Hearth is recorded");
            ExplorationHudView cityArrivalView = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
            Assert(cityArrivalView.WaypointLine.IndexOf("King", StringComparison.OrdinalIgnoreCase) >= 0, "Golden Thread retargets King's Hall after leaving the Grand Hearth");
            Assert(cityArrivalView.ObjectiveSummary.IndexOf("King Halvard", StringComparison.OrdinalIgnoreCase) >= 0,
                "after leaving Town Hall the journey objective advances to King Halvard");
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
                bool criticalInsideGrandHearth = MidgaardInteriorRules.GrandHearthBounds(state.Map)
                    .Contains(new Vector2Int(critical.X, critical.Y));
                Point criticalOrigin = criticalInsideGrandHearth
                    ? MidgaardInteriorRules.GrandHearthSpawn(state.Map)
                    : new Point(state.PlayerX, state.PlayerY);
                bool[,] criticalRouteMask = criticalInsideGrandHearth
                    ? ExplorationTraversalRules.ReachableMask(state.Map, criticalOrigin.X, criticalOrigin.Y)
                    : routeMask;
                Assert(ExplorationTraversalRules.CanReachObject(criticalRouteMask, state.Map, critical), $"critical Midgaard target {type} is reachable");
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
                        ExplorationTraversalRules.ReachableMask(
                            state.Map,
                            MidgaardInteriorRules.GrandHearthBounds(state.Map).Contains(new Vector2Int(placed.X, placed.Y))
                                ? MidgaardInteriorRules.GrandHearthSpawn(state.Map).X
                                : state.PlayerX,
                            MidgaardInteriorRules.GrandHearthBounds(state.Map).Contains(new Vector2Int(placed.X, placed.Y))
                                ? MidgaardInteriorRules.GrandHearthSpawn(state.Map).Y
                                : state.PlayerY),
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
            IReadOnlyList<RoamingThreatDefinition> patrolDefinitions = RoamingThreatCatalog.ForDepth(
                state.Depth,
                ContentSetCatalog.IsFullPrototype(state.ContentSetId));
            Assert(patrols.Count == patrolDefinitions.Count, "cataloged hostile patrols prowl beyond Midgaard");
            Assert(patrols.Select(threat => threat.Id).Distinct().Count() == patrols.Count, "roaming patrol identities are stable and unique");
            foreach (RoamingThreatDefinition definition in patrolDefinitions)
            {
                RoamingThreat patrol = patrols.SingleOrDefault(threat => threat.Id == definition.Id);
                Assert(patrol != null, definition.Id + " is instantiated from the roaming-threat catalog");
                Assert(patrol.Name == definition.Name, definition.Id + " keeps its cataloged player-facing name");
                Assert(patrol.Archetype == definition.Archetype, definition.Id + " keeps its cataloged art archetype");
                Assert(patrol.Active, patrol.Name + " starts active");
                WorldZone patrolZone = InvokePrivate<WorldZone>(game, "ZoneFor", patrol.X, patrol.Y, state.Map, state.Depth);
                Assert(patrolZone != null && patrolZone.Danger > 0, patrol.Name + " starts outside the safe road");
                Assert(state.Map.Objects.All(obj => obj == null || obj.X != patrol.X || obj.Y != patrol.Y), patrol.Name + " does not overlap a map object");
                Assert(patrol.X != state.PlayerX || patrol.Y != state.PlayerY, patrol.Name + " does not overlap the saved player position");
                Assert(!InvokePrivate<bool>(game, "CanStepExplore", patrol.X, patrol.Y), patrol.Name + " visibly occupies its map tile");
            }
            for (int i = 0; i < patrols.Count; i++)
            for (int j = i + 1; j < patrols.Count; j++)
            {
                int homeDistance = Math.Abs(patrols[i].HomeX - patrols[j].HomeX) + Math.Abs(patrols[i].HomeY - patrols[j].HomeY);
                Assert(homeDistance >= 7, patrols[i].Name + " and " + patrols[j].Name + " keep the established home spacing");
            }
            string patrolSignature = string.Join("|", patrols.Select(threat => $"{threat.Id}:{threat.HomeX},{threat.HomeY}"));
            InvokePrivate(game, "EnsureRoamingThreats");
            string repairedPatrolSignature = string.Join("|", state.RoamingThreats
                .Where(threat => threat != null && threat.Depth == state.Depth)
                .OrderBy(threat => threat.Id)
                .Select(threat => $"{threat.Id}:{threat.HomeX},{threat.HomeY}"));
            Assert(patrolSignature == repairedPatrolSignature, "roaming patrol repair is deterministic and idempotent");

            RoamingThreat duplicateSource = patrols[0];
            state.RoamingThreats.Add(new RoamingThreat
            {
                Id = duplicateSource.Id,
                Name = "Corrupt duplicate",
                Archetype = duplicateSource.Archetype,
                Depth = duplicateSource.Depth,
                X = duplicateSource.X,
                Y = duplicateSource.Y,
                HomeX = duplicateSource.HomeX,
                HomeY = duplicateSource.HomeY,
                Active = true
            });
            InvokePrivate(game, "EnsureRoamingThreats");
            Assert(state.RoamingThreats.Count(threat => threat != null && threat.Id == duplicateSource.Id) == 1, "roaming patrol repair removes duplicate saved identities");

            duplicateSource.HomeX = -1;
            duplicateSource.HomeY = -1;
            duplicateSource.X = -1;
            duplicateSource.Y = -1;
            InvokePrivate(game, "EnsureRoamingThreats");
            string recoveredPatrolSignature = string.Join("|", state.RoamingThreats
                .Where(threat => threat != null && threat.Depth == state.Depth)
                .OrderBy(threat => threat.Id)
                .Select(threat => $"{threat.Id}:{threat.HomeX},{threat.HomeY}"));
            Assert(patrolSignature == recoveredPatrolSignature, "roaming patrol repair deterministically recovers an invalid saved home");

            RoamingThreat habitatProbe = state.RoamingThreats.Single(threat =>
                threat != null && threat.Id == patrolDefinitions[0].Id);
            int habitatProbeX = habitatProbe.X;
            int habitatProbeY = habitatProbe.Y;
            bool habitatProbeActive = habitatProbe.Active;
            try
            {
                habitatProbe.X = -1;
                habitatProbe.Y = -1;
                Assert(
                    !InvokePrivate<bool>(game, "CanStepExplore", habitatProbe.HomeX, habitatProbe.HomeY),
                    habitatProbe.Name + " active lair remains solid while its patrol is away");
                habitatProbe.Active = false;
                Assert(
                    InvokePrivate<bool>(game, "CanStepExplore", habitatProbe.HomeX, habitatProbe.HomeY),
                    habitatProbe.Name + " cleared lair becomes passable aftermath scenery");
            }
            finally
            {
                habitatProbe.X = habitatProbeX;
                habitatProbe.Y = habitatProbeY;
                habitatProbe.Active = habitatProbeActive;
            }
            AssertGuidanceReroutesAroundActiveHabitat(game, state);

            foreach (RoamingThreatDefinition definition in patrolDefinitions)
            {
                RoamingThreat patrol = state.RoamingThreats.Single(threat => threat != null && threat.Id == definition.Id);
                InvokePrivate(game, "StartRoamingThreatCombat", patrol);
                Assert(state.Mode == GameMode.Combat && state.Combat != null, definition.Id + " opens combat through the live roaming-threat path");
                Assert(state.Combat.EncounterStyle == "patrol", definition.Id + " retains patrol victory and reward routing");
                Assert(state.Combat.RoamingThreatId == definition.Id, definition.Id + " binds defeat state to its stable roaming identity");
                List<CombatUnit> enemies = state.Combat.Units.Where(unit => unit != null && unit.Side == UnitSide.Enemy).ToList();
                Assert(enemies.Count == EncounterCatalog.For(EncounterId.Patrol).EnemyCountForDepth(state.Depth), definition.Id + " retains the standard patrol enemy count");
                Assert(enemies.All(enemy => definition.EnemyIds.Contains(enemy.Role)), definition.Id + " live combat uses only its explicit roster");
                Assert(enemies.All(enemy => RoamingThreatCatalog.FactionForEnemy(enemy.Role) == definition.Faction), definition.Id + " visible mob and live combat faction agree");
                state.Mode = GameMode.Explore;
                state.Combat = null;
                patrol.Alerted = false;
                InvokePrivate(game, "InvalidateCombatController");
            }
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
            Assert(look.IndexOf("blocks movement", StringComparison.OrdinalIgnoreCase) >= 0, "adjacent target look text explicitly identifies its collision footprint");
            string standingLook = InvokePrivate<string>(game, "ExploreLookLine", standX, standY);
            Assert(standingLook.IndexOf("walkable", StringComparison.OrdinalIgnoreCase) >= 0, "open ground look text explicitly identifies walkable space");

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
            AssertInventoryEquipmentSwapAndRangeSemantics(game);

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
            int expectedInventoryFilters = state.Inventory.Count(item => item != null) >= 9 ? 4 : 0;
            Assert(armory.VisibleFilterCountForTest == expectedInventoryFilters, "small inventories hide filters; larger inventories expose all, weapon, armor, and upgrade views");
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
            InventoryItem strategicRowProbe = new InventoryItem
            {
                DisplayName = "+20 smoke-test enchanted sabre",
                Form = "sabre",
                Slot = "weapon",
                Trait = "keen",
                Rarity = "relic",
                Bonus = 20,
                DamageMin = 30,
                DamageMax = 45,
                AttackSpeed = 20,
                DamageType = "physical",
                PermanentEnchantmentId = "smoke-review"
            };
            state.Inventory.Add(strategicRowProbe);
            SetPrivateField(game, "armoryPackFilter", 0);
            IReadOnlyList<ArmoryRowView> strategicRows = InvokePrivate<IReadOnlyList<ArmoryRowView>>(game, "BuildArmoryPackRows");
            ArmoryRowView strategicRow = strategicRows.FirstOrDefault(row => row.Key == state.Inventory.IndexOf(strategicRowProbe));
            Assert(strategicRow != null
                && strategicRow.Detail.StartsWith("Review for ", StringComparison.Ordinal)
                && strategicRow.Detail.IndexOf("Upgrade for ", StringComparison.Ordinal) < 0,
                "strategic inventory rows agree with their detail action and never contradict Review with an Upgrade label");
            state.Inventory.Remove(strategicRowProbe);
            InvokePrivate(game, "RunArmoryRowAction", state.Inventory.IndexOf(inventoryProbe));
            InvokePrivate(game, "LateUpdate");
            Assert(armory.VisibleDetailActionCountForTest <= 2, "Inventory reveals only the recommended equip action and optional party chooser");
            if (armory.VisibleDetailActionCountForTest > 1)
            {
                armory.InvokeDetailActionForTest(1);
                InvokePrivate(game, "LateUpdate");
                Assert(armory.VisibleDetailActionCountForTest == state.Party.Count(member => member != null) + 1, "party targets appear only after Choose another");
                Assert(armory.FocusedDetailActionIndexForTest == 0, "opening the party picker moves controller focus to the recommended target");
                bool supportedArmoryViewport = Screen.width >= ArmoryOverlayLayout.MinimumSupportedWidth
                    && Screen.height >= ArmoryOverlayLayout.MinimumSupportedHeight;
                Assert(!supportedArmoryViewport || armory.DetailActionsMeetAccessibleSizingForTest,
                    $"the four-person party picker keeps every action visible and at least 40px tall at supported resolution ({Screen.width}x{Screen.height})");
                Assert(InvokePrivate<bool>(game, "HandleCancelCommand"), "Cancel first collapses the open inventory target picker");
                InvokePrivate(game, "LateUpdate");
                Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Armory
                    && armory.VisibleDetailActionCountForTest <= 2, "collapsing the target picker keeps Inventory open on the concise recommendation");
            }
            InvokePrivate(game, "RunArmoryDetailAction", 0);
            InvokePrivate(game, "LateUpdate");
            Assert(inventoryProbe.EquippedById == state.Party[0].Id, "explicit inventory target action records the exact equipped owner");
            Assert(state.Party[0].WeaponName == inventoryProbe.DisplayName, "explicit inventory target action updates that adventurer's loadout");
            InventoryItem identicalInventoryProbe = new InventoryItem
            {
                DisplayName = inventoryProbe.DisplayName,
                Material = inventoryProbe.Material,
                Form = inventoryProbe.Form,
                Slot = inventoryProbe.Slot,
                Trait = inventoryProbe.Trait,
                Rarity = inventoryProbe.Rarity,
                Bonus = inventoryProbe.Bonus,
                StrengthBonus = inventoryProbe.StrengthBonus,
                IntelligenceBonus = inventoryProbe.IntelligenceBonus,
                AgilityBonus = inventoryProbe.AgilityBonus,
                HealthBonus = inventoryProbe.HealthBonus,
                DamageMin = inventoryProbe.DamageMin,
                DamageMax = inventoryProbe.DamageMax,
                AttackSpeed = inventoryProbe.AttackSpeed,
                DamageType = inventoryProbe.DamageType
            };
            Assert(InvokePrivate<int>(game, "InventoryComparisonScore", identicalInventoryProbe, state.Party[0]) == 0, "identical equipped weapon stats remain an honest sidegrade after role-range floors");
            ArmoryDetailView equippedInventoryDetail = InvokePrivate<ArmoryDetailView>(game, "BuildInventoryItemDetail");
            Assert(equippedInventoryDetail != null
                && equippedInventoryDetail.Actions.Any(action => action.Enabled && action.ButtonLabel == "Swap"), "reviewing equipped gear offers a direct same-slot reassignment instead of a dead end");
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
            InventoryItem popupDuplicate = JsonUtility.FromJson<InventoryItem>(JsonUtility.ToJson(popupProbe));
            state.Inventory.Add(popupDuplicate);
            state.Inventory.Add(popupProbe);
            SetPrivateField(game, "armoryPackFilter", 2);
            InvokePrivate(game, "ShowLootPanel", popupProbe, 3, 1, 0, "Tessa: \"A real reward keeps its own window.\"", "Popup Probe");
            InvokePrivate(game, "LateUpdate");
            Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Loot, "loot reward opens its own overlay");
            AssertRootOverlayCanvas("Loot Popup Canvas");
            Assert(InvokePrivate<bool>(game, "HasRenderableGameplayOverlay", UiOverlay.Loot), "IMGUI yields the frame to visible loot");
            LootPopupScreen lootPopup = GetPrivateField<LootPopupScreen>(game, "lootPopupScreen");
            Assert(lootPopup != null && lootPopup.HasRealIconForTest, "loot reward uses the real inventory item atlas");
            Assert(lootPopup.HasReviewActionForTest, "acquired gear offers a direct equipment review action");
            Assert(lootPopup.PrimaryActionLabelForTest == "Continue", "loot action accurately reflects that rewards are already acquired");
            Assert(lootPopup.ReviewActionLabelForTest == "Compare others", "stored gear names the wider comparison available in Inventory");
            Assert(lootPopup.HasDefaultFocusForTest, "loot opens with Continue owning keyboard and controller Submit");
            LootPopupView popupView = InvokePrivate<LootPopupView>(game, "BuildLootPopupView");
            Assert(popupView.HasItem && popupView.CanReview, "gear reward view exposes the committed item and its review action");
            Assert(popupView.Gold == 3 && popupView.Supplies == 1 && popupView.Elixirs == 0, "gear reward view reports exact resource deltas");
            Assert(popupView.Comparison.IndexOf("Best fit:", StringComparison.Ordinal) >= 0, "stored gear surfaces its best-fit comparison before review");
            lootPopup.InvokeReviewForTest();
            InvokePrivate(game, "LateUpdate");
            Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Armory, "Review equipment opens Inventory on the exact acquired item");
            armory = GetPrivateField<ArmoryOverlayScreen>(game, "armoryOverlayScreen");
            Assert(armory != null && armory.ActiveTabLabelForTest == "Inventory" && armory.HasVisibleDetailForTest, "loot review lands on the inventory comparison pane");
            Assert(GetPrivateField<int>(game, "armorySelectedInventoryIndex") == state.Inventory.IndexOf(popupProbe), "loot review selects the exact committed inventory item");
            int reviewedVisibleRow = armory.VisibleRowIndexForKeyForTest(state.Inventory.IndexOf(popupProbe));
            Assert(GetPrivateField<int>(game, "armoryPackFilter") == 0
                && reviewedVisibleRow == armory.CommittedRowIndexForTest
                && reviewedVisibleRow == armory.FocusedRowIndexForTest
                && armory.IsRowFullyVisibleForTest(reviewedVisibleRow), "loot review clears stale filters and visibly focuses the exact item reference even when names are duplicated");
            InvokePrivate(game, "CloseArmoryOverlay");
            InvokePrivate(game, "LateUpdate");

            InvokePrivate(game, "ShowLootPanel", null, 7, 2, 1, "The company stores are updated.", "Victory spoils");
            InvokePrivate(game, "LateUpdate");
            LootPopupView resourceOnlyView = InvokePrivate<LootPopupView>(game, "BuildLootPopupView");
            lootPopup = GetPrivateField<LootPopupScreen>(game, "lootPopupScreen");
            Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.Loot, "a victory with no gear still presents its spoils");
            Assert(resourceOnlyView.Visible && !resourceOnlyView.HasItem && !resourceOnlyView.CanReview, "resource-only spoils are visible without a false equipment action");
            Assert(resourceOnlyView.Gold == 7 && resourceOnlyView.Supplies == 2 && resourceOnlyView.Elixirs == 1, "resource-only spoils preserve every exact delta");
            Assert(resourceOnlyView.Outcome == "Added to company stores", "resource-only spoils state their completed outcome");
            Assert(lootPopup != null && !lootPopup.HasReviewActionForTest && lootPopup.HasDefaultFocusForTest, "resource-only spoils keep one focused Continue action");
            Assert(InvokePrivate<bool>(game, "HandleCancelCommand"), "the shared Escape / B command owns the visible loot modal");
            InvokePrivate(game, "LateUpdate");
            Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") == UiOverlay.None, "resource-only spoils dismiss cleanly through controller Cancel");

            InventoryItem questProbe = new InventoryItem
            {
                DisplayName = "Smoke-Test Road Seal",
                Mark = "road-marked",
                Material = "emberglass",
                Form = "sealed tally",
                Slot = "quest",
                Trait = "proof",
                Rarity = "quest"
            };
            state.Inventory.Add(questProbe);
            SetPrivateField(game, "armoryPackFilter", 1);
            InvokePrivate(game, "ShowLootPanel", questProbe, 0, 0, 0, "Secured with the route proofs.", "Road proof");
            InvokePrivate(game, "LateUpdate");
            LootPopupView questView = InvokePrivate<LootPopupView>(game, "BuildLootPopupView");
            lootPopup = GetPrivateField<LootPopupScreen>(game, "lootPopupScreen");
            Assert(questView.HasItem && questView.CanReview && questView.Outcome == "Quest item secured", "quest finds remain viewable without pretending to be equipment");
            Assert(questView.Comparison.IndexOf("not equipment", StringComparison.OrdinalIgnoreCase) >= 0, "quest finds explain that they cannot replace a loadout");
            Assert(questView.ReviewActionLabel == "View in inventory" && lootPopup.ReviewActionLabelForTest == "View in inventory", "quest finds use an honest inventory action label");
            lootPopup.InvokeReviewForTest();
            InvokePrivate(game, "LateUpdate");
            armory = GetPrivateField<ArmoryOverlayScreen>(game, "armoryOverlayScreen");
            int reviewedQuestRow = armory.VisibleRowIndexForKeyForTest(state.Inventory.IndexOf(questProbe));
            Assert(GetPrivateField<int>(game, "armoryPackFilter") == 0
                && reviewedQuestRow == armory.CommittedRowIndexForTest
                && reviewedQuestRow == armory.FocusedRowIndexForTest, "quest-item review also clears an incompatible equipment filter and lands on the exact row");
            InvokePrivate(game, "CloseArmoryOverlay");
            InvokePrivate(game, "LateUpdate");

            int minimumWeaponBonus = state.Party.Where(member => member != null && member.Hp > 0).Min(member => member.WeaponBonus);
            InventoryItem weakEqualBonus = new InventoryItem
            {
                DisplayName = "Equal-Bonus Blunted Practice Sword",
                Form = "sword",
                Slot = "weapon",
                Trait = "dull",
                Rarity = "common",
                Bonus = minimumWeaponBonus,
                DamageMin = 1,
                DamageMax = 2,
                AttackSpeed = 1,
                DamageType = "physical"
            };
            state.Inventory.Add(weakEqualBonus);
            string[] weaponNamesBefore = state.Party.Select(member => member?.WeaponName ?? "").ToArray();
            string weakEquipNote = InvokePrivate<string>(game, "AutoEquipItem", weakEqualBonus);
            Assert(string.IsNullOrEmpty(weakEqualBonus.EquippedById), "equal-bonus low-performance loot stays in the pack");
            Assert(state.Party.Select(member => member?.WeaponName ?? "").SequenceEqual(weaponNamesBefore), "rejected loot does not mutate any loadout");
            Assert(weakEquipNote.IndexOf("Kept in the pack", StringComparison.Ordinal) >= 0, "rejected loot explains the safe outcome");

            InventoryItem clearUpgrade = new InventoryItem
            {
                DisplayName = "+9 Smoke-Test Crownward Longbow",
                Form = "longbow",
                Slot = "weapon",
                Trait = "keen",
                Rarity = "relic",
                Bonus = 9,
                DamageMin = 20,
                DamageMax = 30,
                AttackSpeed = 16,
                DamageType = "physical"
            };
            state.Inventory.Add(clearUpgrade);
            string[] clearUpgradeWeaponNamesBefore = state.Party.Select(member => member?.WeaponName ?? "").ToArray();
            string initialUpgradeEquipNote = InvokePrivate<string>(game, "AutoEquipItem", clearUpgrade);
            Assert(string.IsNullOrEmpty(clearUpgrade.EquippedById), "a clear full-score upgrade stays unowned until the player chooses Equip");
            Assert(state.Party.Select(member => member?.WeaponName ?? "").SequenceEqual(clearUpgradeWeaponNamesBefore), "loot acquisition leaves every loadout unchanged before the explicit action");
            Assert(initialUpgradeEquipNote.IndexOf("Clear upgrade", StringComparison.Ordinal) >= 0, "loot guidance explains the full-score recommendation");
            PartyMember downedHighestScore = state.Party
                .Where(member => member != null && member.Hp > 0)
                .OrderByDescending(member => InvokePrivate<int>(game, "InventoryComparisonScore", clearUpgrade, member))
                .ThenByDescending(member => InvokePrivate<int>(game, "WeaponRoleFit", clearUpgrade, member))
                .ThenBy(member => state.Party.IndexOf(member))
                .First();
            int downedHighestScoreHp = downedHighestScore.Hp;
            downedHighestScore.Hp = 0;
            PartyMember expectedBest = state.Party
                .Where(member => member != null && member.Hp > 0)
                .OrderByDescending(member => InvokePrivate<int>(game, "InventoryComparisonScore", clearUpgrade, member))
                .ThenByDescending(member => InvokePrivate<int>(game, "WeaponRoleFit", clearUpgrade, member))
                .ThenBy(member => state.Party.IndexOf(member))
                .First();
            string upgradeEquipNote = InvokePrivate<string>(game, "AutoEquipItem", clearUpgrade);
            Assert(upgradeEquipNote.IndexOf("Kept in the pack", StringComparison.Ordinal) >= 0, "available-target guidance remains a non-mutating recommendation");
            Assert(upgradeEquipNote.IndexOf(expectedBest.Name, StringComparison.Ordinal) >= 0
                && upgradeEquipNote.IndexOf(downedHighestScore.Name, StringComparison.Ordinal) < 0,
                "loot guidance skips the downed highest-score adventurer and names the canonical available target");
            InvokePrivate(game, "ShowLootPanel", clearUpgrade, 0, 0, 0, upgradeEquipNote, "Clear Upgrade");
            InvokePrivate(game, "LateUpdate");
            LootPopupView pendingLootView = InvokePrivate<LootPopupView>(game, "BuildLootPopupView");
            LootPopupScreen clearUpgradePopup = GetPrivateField<LootPopupScreen>(game, "lootPopupScreen");
            Assert(pendingLootView.CanQuickEquip && clearUpgradePopup.HasQuickEquipActionForTest, "clear upgrade exposes the explicit quick-equip action");
            Assert(pendingLootView.QuickEquipActionLabel == "Equip to " + expectedBest.Name
                && clearUpgradePopup.QuickEquipActionLabelForTest == "Equip to " + expectedBest.Name, "quick equip names the exact best-fit adventurer");
            clearUpgradePopup.InvokeQuickEquipForTest();
            InvokePrivate(game, "LateUpdate");
            Assert(clearUpgrade.EquippedById == expectedBest.Id, "the explicit quick action records the exact recommended owner");
            Assert(state.Party.Count(member => member != null && member.WeaponName == clearUpgrade.DisplayName) == 1
                && expectedBest.WeaponName == clearUpgrade.DisplayName, "the explicit quick action changes exactly the recommended loadout");
            Assert(state.Party.Where((member, index) => member != null && member != expectedBest)
                .All(member => member.WeaponName == clearUpgradeWeaponNamesBefore[state.Party.IndexOf(member)]), "quick equip leaves every other weapon loadout unchanged");
            LootPopupView equippedLootView = InvokePrivate<LootPopupView>(game, "BuildLootPopupView");
            Assert(!equippedLootView.CanQuickEquip && equippedLootView.ReviewActionLabel == "Review or reassign", "equipped loot replaces quick equip with the reassignment decision");
            downedHighestScore.Hp = downedHighestScoreHp;
            InvokePrivate(game, "DismissLootPopup");
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
            Assert(state.Inventory.Count(item => item != null
                && item.DisplayName == "+2 Stormglass Conductor"
                && item.SignatureId == SignatureItemCatalog.StormglassConductorId) == 1,
                "safe-room focus enters inventory once with its canonical signature identity");
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
            Assert(chapterTwoJournal.Any(row => row.Title == "Old Road Chapter II" && row.Subtitle == "Eastbound Old Road open"), "production Journal names the live eastbound Chapter II route after Borin's reward");
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
            Assert(restoredStoryGuidance.WaypointLine.IndexOf("Eastbound Old Road", StringComparison.OrdinalIgnoreCase) >= 0
                && restoredStoryGuidance.WaypointLine.IndexOf("Marked:", StringComparison.OrdinalIgnoreCase) < 0,
                "clearing a Journal waypoint immediately restores the Chapter II story thread");
            SetPrivateField(game, "armoryTab", previousArmoryTab);
            MapObject oldRoadDescent = state.Map.FindObjectById("old-road-descent-sluice-steps");
            Assert(oldRoadDescent != null && oldRoadDescent.Type == ObjectType.Stairs, "chapter reward creates one stable eastbound Old Road transition");
            WorldMapJunction[] oldRoadJunctions = WorldMapGenerationRules.RegionalJunctions(
                state.Map.Width,
                state.Map.Height,
                state.Map.StartX,
                state.Map.StartY);
            WorldMapJunction pilgrimFork = oldRoadJunctions.Single(junction => junction.Id == "pilgrim-fork");
            WorldMapJunction lanternlessCross = oldRoadJunctions.Single(junction => junction.Id == "lanternless-cross");
            Assert(oldRoadDescent.X == lanternlessCross.X
                && oldRoadDescent.Y == lanternlessCross.Y
                && oldRoadDescent.Y == state.Map.StartY,
                "the stable Chapter II transition sits at Lanternless Cross on the eastbound Old Road");
            Assert(InvokePrivate<string>(game, "ObjectName", oldRoadDescent) == "Eastbound Old Road",
                "the save-stable transition publishes its new eastbound Old Road identity");
            int oldRoadWestX = Math.Min(pilgrimFork.X, lanternlessCross.X);
            int oldRoadEastX = Math.Max(pilgrimFork.X, lanternlessCross.X);
            for (int x = oldRoadWestX; x <= oldRoadEastX; x++)
            {
                int tileIndex = state.Map.StartY * state.Map.Width + x;
                Assert(tileIndex >= 0
                    && tileIndex < state.Map.Tiles.Count
                    && state.Map.Tiles[tileIndex] == 1,
                    $"Old Road cell {x},{state.Map.StartY} is open");
                ExplorationCellRole oldRoadRoles = ExplorationSurfaceRules.RolesAt(state.Map, x, state.Map.StartY);
                Assert(WorldMapGenerationRules.IsOldRoadCenterlineCell(
                        state.Map.Width,
                        state.Map.Height,
                        state.Map.StartX,
                        state.Map.StartY,
                        x,
                        state.Map.StartY)
                    && (oldRoadRoles & ExplorationCellRole.Road) != 0,
                    $"Old Road cell {x},{state.Map.StartY} retains its semantic Road role");
                Assert((oldRoadRoles & (ExplorationCellRole.Room | ExplorationCellRole.Water | ExplorationCellRole.Hazard)) == 0,
                    $"Old Road cell {x},{state.Map.StartY} has no room, water, or hazard role conflict");
                Assert(state.Map.Objects
                    .Where(obj => obj != null && obj.X == x && obj.Y == state.Map.StartY)
                    .All(obj => ExplorationTraversalRules.CanStandOnObject(obj)),
                    $"Old Road cell {x},{state.Map.StartY} has no blocking scenery; gates and the transition remain standable");
            }
            bool[,] oldRoadReachable = ExplorationTraversalRules.ReachableMask(state.Map, state.PlayerX, state.PlayerY);
            Assert(ExplorationTraversalRules.CanReachObject(oldRoadReachable, state.Map, oldRoadDescent), "Lanternless Cross and its eastbound Old Road transition are reachable from the current Midgaard component");
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
            Assert(InvokePrivate<bool>(game, "CanDescend"), "the named eastbound Old Road marker authorizes the first normal-play transition");
            InvokePrivate(game, "Descend");
            Assert(state.Depth == 2 && state.Map != null, "normal sewer-slice play descends into the Chapter II regional map");
            Assert(state.Map.Width == WorldMapGenerationRules.Width && state.Map.Height == WorldMapGenerationRules.Height, "Chapter II uses the expanded fresh-map dimensions");
            int firstArrivalSupplies = state.Supplies;
            int firstArrivalChapter = state.StoryChapter;
            string firstArrivalProgression = string.Join("|", state.Party.Select(member =>
                $"{member.Id}:{member.Level}:{member.Experience}:{member.SkillPoints}:{member.StatPoints}"));
            Assert((state.ActiveStory ?? "").IndexOf("Chapter II", StringComparison.OrdinalIgnoreCase) >= 0
                && (state.ActiveStory ?? "").IndexOf("Dusk Market", StringComparison.OrdinalIgnoreCase) >= 0
                && InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName").IndexOf("Dusk Market", StringComparison.OrdinalIgnoreCase) >= 0,
                "first Chapter II arrival publishes a depth-aware objective and Dusk Market guidance");

            InvokePrivate(game, "RecallToTempleSquare");
            Assert(state.Depth == 1 && state.Map != null && state.Map.Depth == 1,
                "actual Recall returns Chapter II explorers to the Midgaard map");
            Assert(ExplorationChartRules.IsCharted(
                    state.DiscoveredZones,
                    state.Depth,
                    state.PlayerX,
                    state.PlayerY),
                "Recall records the repaired Temple Square landing in the durable terrain chart");
            Assert((state.ActiveStory ?? "").IndexOf("Chapter II", StringComparison.OrdinalIgnoreCase) >= 0
                && InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName").IndexOf("Eastbound Old Road", StringComparison.OrdinalIgnoreCase) >= 0,
                "Recall preserves the live Chapter II objective while guidance refreshes to the eastbound Old Road");
            MapObject reentryRoad = state.Map.FindObjectById("old-road-descent-sluice-steps");
            Assert(reentryRoad != null && reentryRoad.Type == ObjectType.Stairs,
                "Recall rebuilds the stable Midgaard re-entry marker");
            state.PlayerX = reentryRoad.X;
            state.PlayerY = reentryRoad.Y;
            Assert(InvokePrivate<bool>(game, "CanDescend"), "the recalled party can re-enter Chapter II through the real Old Road transition");
            InvokePrivate(game, "Descend");
            string reentryProgression = string.Join("|", state.Party.Select(member =>
                $"{member.Id}:{member.Level}:{member.Experience}:{member.SkillPoints}:{member.StatPoints}"));
            Assert(state.Depth == 2
                && state.Map != null
                && state.Map.Depth == 2
                && state.Supplies == firstArrivalSupplies
                && state.StoryChapter == firstArrivalChapter
                && reentryProgression == firstArrivalProgression,
                "Recall plus Chapter II re-entry grants no duplicate supplies, experience, levels, points, or chapter advancement");
            Assert((state.ActiveStory ?? "").IndexOf("Chapter II", StringComparison.OrdinalIgnoreCase) >= 0
                && (state.ActiveStory ?? "").IndexOf("Dusk Market", StringComparison.OrdinalIgnoreCase) >= 0
                && InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName").IndexOf("Dusk Market", StringComparison.OrdinalIgnoreCase) >= 0,
                "Chapter II re-entry refreshes the depth-aware objective and live Dusk Market guidance");
            InvokePrivate(game, "EnsureKoboldKingCaveMarker");
            List<MapObject> stableSmokeCaves = state.Map.Objects
                .Where(obj => obj != null && obj.Id == "dusk-market-smoke-cave")
                .ToList();
            Assert(stableSmokeCaves.Count == 1 && stableSmokeCaves[0].Type == ObjectType.Cave,
                "Chapter II promotes exactly one stable Dusk Market smoke cave");
            MapObject smokeCave = stableSmokeCaves[0];
            int smokeCaveX = smokeCave.X;
            int smokeCaveY = smokeCave.Y;
            InvokePrivate(game, "EnsureKoboldKingCaveMarker");
            Assert(state.Map.Objects.Count(obj => obj != null && obj.Id == "dusk-market-smoke-cave") == 1
                && state.Map.FindObjectById("dusk-market-smoke-cave")?.X == smokeCaveX
                && state.Map.FindObjectById("dusk-market-smoke-cave")?.Y == smokeCaveY,
                "re-ensuring the kobold marker preserves one exact stable cave");
            bool[,] chapterTwoReachable = ExplorationTraversalRules.ReachableMask(state.Map, state.PlayerX, state.PlayerY);
            Assert(ExplorationTraversalRules.CanReachObject(chapterTwoReachable, state.Map, smokeCave), "Dusk Market smoke cave remains reachable after descent");
            Assert(InvokePrivate<bool>(game, "IsKoboldStoryCave", smokeCave), "the stable Dusk Market smoke cave owns the Chapter II cave route");
            WorldMapSite duskMarketSite = WorldMapGenerationRules.RegionalSites(
                    state.Map.Width,
                    state.Map.Height,
                    state.Map.StartX,
                    state.Map.StartY)
                .Single(site => site.ZoneId == "dusk-market");
            Assert(smokeCave.X == duskMarketSite.X + 2 && smokeCave.Y == duskMarketSite.Y,
                "the stable Smoke Cave marker sits exactly two cells east of the authored Dusk Market regional site");
            AssertLegacyKoboldCaveMigration(game, state);
            MapObject duskMarketLandmark = state.Map.FindObjectById("regional-site:" + duskMarketSite.Id);
            Assert(duskMarketLandmark != null
                && InvokePrivate<bool>(game, "IsEligibleExploreWaypoint", duskMarketLandmark)
                && !InvokePrivate<bool>(game, "IsEligibleExploreWaypoint", smokeCave),
                "before the ambush, guidance admits Dusk Market but withholds the unrevealed stable cave");
            Assert(!InvokePrivate<bool>(game, "IsKoboldStoryObject", smokeCave),
                "before the ambush, the stable cave does not receive story-route presentation");
            string preAmbushGuidanceTarget = InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName");
            Assert(preAmbushGuidanceTarget.IndexOf("Dusk Market", StringComparison.OrdinalIgnoreCase) >= 0
                && preAmbushGuidanceTarget.IndexOf("Smoke Cave", StringComparison.OrdinalIgnoreCase) < 0,
                "before the ambush, the Golden Thread leads to Dusk Market without revealing the Smoke Cave Mouth");
            MapObject ordinaryDuskCave = new MapObject(smokeCave.X, smokeCave.Y, ObjectType.Cave, "ordinary-dusk-cave");
            Assert(!InvokePrivate<bool>(game, "IsKoboldStoryCave", ordinaryDuskCave), "an ordinary Dusk Market cave cannot impersonate the Chapter II route");
            Assert(!InvokePrivate<bool>(game, "IsKoboldStoryObject", ordinaryDuskCave)
                && InvokePrivate<int>(game, "ProgressionOverlayIndex", ordinaryDuskCave) == 11,
                "ordinary Dusk Market caves remain ordinary cave art without story or special progression overlays");
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
            Point legalAmbushCell = null;
            for (int y = Mathf.Max(1, duskMarketSite.Y - duskMarketSite.Radius); y <= Mathf.Min(state.Map.Height - 2, duskMarketSite.Y + duskMarketSite.Radius); y++)
            for (int x = Mathf.Max(1, duskMarketSite.X - duskMarketSite.Radius); x <= Mathf.Min(state.Map.Width - 2, duskMarketSite.X + duskMarketSite.Radius); x++)
            {
                if (x == smokeCave.X && y == smokeCave.Y) continue;
                if (!InvokePrivate<bool>(game, "IsKoboldAmbushApproachCell", x, y)
                    || !InvokePrivate<bool>(game, "CanStepExplore", x, y)) continue;
                WorldZone candidateZone = InvokePrivate<WorldZone>(game, "ZoneFor", x, y, state.Map, state.Depth);
                if (candidateZone == null || candidateZone.Id != "dusk-market") continue;
                IReadOnlyList<Point> candidatePath = InvokePrivate<List<Point>>(game, "FindLiveExplorePath", x, y);
                if (candidatePath.Count == 0) continue;
                legalAmbushCell = new Point(x, y);
                break;
            }
            Assert(legalAmbushCell != null
                && (legalAmbushCell.X != smokeCave.X || legalAmbushCell.Y != smokeCave.Y),
                "Dusk Market exposes a legal reachable ambush cell away from the solid Smoke Cave object");
            state.PlayerX = legalAmbushCell.X;
            state.PlayerY = legalAmbushCell.Y;
            Assert(InvokePrivate<bool>(game, "CanStepExplore", state.PlayerX, state.PlayerY)
                && !state.StoryFlags.Contains(StoryFlags.KoboldAmbushSprung),
                "the fresh kobold ambush begins from a standable Dusk Market cell");
            Assert(InvokePrivate<bool>(game, "MaybeTriggerKoboldAmbush"), "entering the legal Dusk Market cell triggers the first kobold ambush");
            Assert(state.StoryFlags.Contains(StoryFlags.KoboldAmbushSprung)
                && state.Mode == GameMode.Combat
                && state.Combat?.EncounterStyle == "koboldambush",
                "the legal Dusk Market trigger opens the bounded kobold ambush encounter");
            state.Mode = GameMode.Explore;
            state.Combat = null;
            InvokePrivate(game, "InvalidateCombatController");
            Assert(InvokePrivate<bool>(game, "MaybeTriggerKoboldAmbush"), "an unresolved sprung ambush retriggers after retreat");
            Assert(state.Mode == GameMode.Combat && state.Combat?.EncounterStyle == "koboldambush", "the retry opens the bounded kobold ambush encounter");
            InvokePrivate(game, "ApplyKoboldStoryVictory", "koboldambush");
            Assert(state.StoryFlags.Contains(StoryFlags.KoboldAmbushSurvived), "Chapter II ambush victory advances the promoted route");
            state.Mode = GameMode.Explore;
            state.Combat = null;
            Assert(InvokePrivate<bool>(game, "IsKoboldStoryObject", smokeCave)
                && InvokePrivate<string>(game, "ObjectName", smokeCave) == "Smoke Cave Mouth",
                "after the ambush, the stable cave becomes the named Smoke Cave Mouth story object");
            Assert(TryFindAdjacentProbeTile(game, state, smokeCave, out int smokeApproachX, out int smokeApproachY),
                "the revealed Smoke Cave Mouth has a reachable approach tile");
            state.PlayerX = smokeApproachX;
            state.PlayerY = smokeApproachY;
            Assert(InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName") == "Smoke Cave Mouth",
                "after the ambush, the Golden Thread advances to the revealed Smoke Cave Mouth");
            InvokePrivate(game, "ResolveExploreObject", smokeCave);
            Assert(state.StoryFlags.Contains(StoryFlags.KoboldCaveFound)
                && state.Mode == GameMode.Combat
                && state.Combat?.EncounterStyle == "koboldcave", "the stable smoke cave opens only after the ambush victory");
            InvokePrivate(game, "ApplyKoboldStoryVictory", "koboldcave");
            Assert(state.StoryFlags.Contains(StoryFlags.KoboldCaveCleared), "Chapter II smoke-cave victory advances to the king's hall");
            state.Mode = GameMode.Explore;
            state.Combat = null;
            InvokePrivate(game, "EnsureKoboldKingCaveMarker");
            MapObject varkhMouth = state.Map.FindObjectById("dusk-market-smoke-cave");
            Assert(varkhMouth != null
                && varkhMouth.X == smokeCaveX
                && varkhMouth.Y == smokeCaveY
                && InvokePrivate<string>(game, "ObjectName", varkhMouth) == "Varkh's Hall",
                "clearing the cave advances the same stable mouth to Varkh's Hall");
            Assert(TryFindAdjacentProbeTile(game, state, varkhMouth, out int varkhApproachX, out int varkhApproachY),
                "Varkh's route remains reachable through the same cave mouth");
            state.PlayerX = varkhApproachX;
            state.PlayerY = varkhApproachY;
            Assert(InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName") == "Varkh's Hall",
                "after the cave clear, the Golden Thread advances through the same mouth to Varkh");
            InvokePrivate(game, "ResolveExploreObject", varkhMouth);
            Assert(state.Mode == GameMode.Combat && state.Combat?.EncounterStyle == "koboldking",
                "after the cave clear, using the same Smoke Cave Mouth advances directly to Varkh");
            IReadOnlyList<ArmoryRowView> kingRoadJournal = InvokePrivate<IReadOnlyList<ArmoryRowView>>(game, "BuildArmoryJournalRows");
            Assert(kingRoadJournal.Any(row => row.Title == "Kobold Smoke - Dusk Market Ambush" && row.Subtitle == "complete"), "production Journal retains completed ambush state");
            Assert(kingRoadJournal.Any(row => row.Title == "Kobold Smoke - Smoke Cave" && row.Subtitle == "complete"), "production Journal retains completed smoke-cave state");
            Assert(kingRoadJournal.Any(row => row.Title == "Kobold Smoke - Varkh's Hall" && row.Subtitle == "current"), "production Journal promotes Varkh's Hall as the next live route step");
            InvokePrivate(game, "FinishKoboldKingVictory", 0, 0, 0);
            Assert(state.StoryFlags.Contains(StoryFlags.KoboldKingDefeated), "invoking Varkh's victory completes the bounded kobold chapter");
            Assert(!state.Map.Objects.Any(obj => obj != null && obj.Id == "dusk-market-smoke-cave"),
                "Varkh's defeat removes the completed Smoke Cave marker");
            InvokePrivate(game, "EnsureKoboldKingCaveMarker");
            Assert(!state.Map.Objects.Any(obj => obj != null && obj.Id == "dusk-market-smoke-cave"),
                "the completed Smoke Cave marker is not recreated after Varkh");
            string kingVictoryCopy = state.ActiveStory ?? "";
            Assert(kingVictoryCopy.IndexOf("Chapter III", StringComparison.OrdinalIgnoreCase) >= 0
                && kingVictoryCopy.IndexOf("Bone Road", StringComparison.OrdinalIgnoreCase) >= 0,
                "Varkh's victory immediately opens the named Chapter III route");
            AssertBoneRoadStoryFlow(game, state, duskMarketSite, smokeCaveX, smokeCaveY);
        }

        private static void AssertBoneRoadStoryFlow(
            AshenHallsGame game,
            GameState state,
            WorldMapSite duskMarketSite,
            int stablePassageX,
            int stablePassageY)
        {
            const string boneRoadPassageId = "bone-road-passage-varkh-hall";
            const string glassAndAshPassageId = "glass-and-ash-passage-red-gate";
            InvokePrivate(game, "DismissLootPopup");
            InvokePrivate(game, "LateUpdate");

            List<MapObject> varkhPassages = state.Map.Objects
                .Where(obj => obj != null && obj.Id == boneRoadPassageId)
                .ToList();
            Assert(varkhPassages.Count == 1
                && varkhPassages[0].Type == ObjectType.Stairs
                && varkhPassages[0].X == stablePassageX
                && varkhPassages[0].Y == stablePassageY
                && stablePassageX == duskMarketSite.X + 2
                && stablePassageY == duskMarketSite.Y,
                "Varkh's completion creates exactly one Bone Road passage at the stable Dusk Market anchor");
            MapObject boneRoadPassage = varkhPassages[0];
            bool[,] varkhReachable = ExplorationTraversalRules.ReachableMask(state.Map, state.PlayerX, state.PlayerY);
            Assert(ExplorationTraversalRules.CanReachObject(varkhReachable, state.Map, boneRoadPassage),
                "Varkh's Bone Road passage is reachable from the live Chapter II component");

            // Model the load path for a pre-Chapter-III Varkh save: the old cave
            // survived, the promoted passage did not, and its objective is stale.
            boneRoadPassage.Id = "dusk-market-smoke-cave";
            boneRoadPassage.Type = ObjectType.Cave;
            state.Map.Objects.RemoveAll(obj => obj != null && obj.Id == boneRoadPassageId);
            state.Map.InvalidateObjectLookup();
            state.ActiveStory = "Chapter II complete: seek the next stair.";
            InvokePrivate(game, "EnsureWorldState", state.SaveVersion);
            InvokePrivate(game, "EnsureWorldLandmarks");
            varkhPassages = state.Map.Objects.Where(obj => obj != null && obj.Id == boneRoadPassageId).ToList();
            Assert(varkhPassages.Count == 1
                && !state.Map.Objects.Any(obj => obj != null && obj.Id == "dusk-market-smoke-cave")
                && varkhPassages[0].X == stablePassageX
                && varkhPassages[0].Y == stablePassageY,
                "load repair upgrades a legacy post-Varkh cave into one stable Bone Road passage");
            Assert((state.ActiveStory ?? "").IndexOf("Chapter III", StringComparison.OrdinalIgnoreCase) >= 0
                && (state.ActiveStory ?? "").IndexOf("Bone Road", StringComparison.OrdinalIgnoreCase) >= 0,
                "load repair replaces the stale Varkh objective with the current Bone Road objective");

            state.Map.Objects.RemoveAll(obj => obj != null && obj.Id == boneRoadPassageId);
            state.Map.InvalidateObjectLookup();
            InvokePrivate(game, "EnsureWorldState", state.SaveVersion);
            InvokePrivate(game, "EnsureWorldLandmarks");
            varkhPassages = state.Map.Objects.Where(obj => obj != null && obj.Id == boneRoadPassageId).ToList();
            Assert(varkhPassages.Count == 1, "load repair recreates a missing post-Varkh Bone Road passage exactly once");
            state.Map.Objects.Add(new MapObject(stablePassageX, stablePassageY, ObjectType.Stairs, boneRoadPassageId));
            state.Map.InvalidateObjectLookup();
            InvokePrivate(game, "EnsureWorldState", state.SaveVersion);
            InvokePrivate(game, "EnsureWorldLandmarks");
            varkhPassages = state.Map.Objects.Where(obj => obj != null && obj.Id == boneRoadPassageId).ToList();
            Assert(varkhPassages.Count == 1, "load repair collapses duplicate post-Varkh Bone Road markers");
            boneRoadPassage = varkhPassages[0];

            state.PlayerX = boneRoadPassage.X;
            state.PlayerY = boneRoadPassage.Y;
            state.StoryFlags.Remove(StoryFlags.KoboldKingDefeated);
            Assert(!InvokePrivate<bool>(game, "CanDescend"), "the Bone Road remains locked before Varkh's defeat flag");
            state.StoryFlags.Add(StoryFlags.KoboldKingDefeated);
            Assert(InvokePrivate<bool>(game, "CanDescend"), "Varkh's stable passage authorizes Chapter II to Chapter III travel");
            boneRoadPassage.Id = "wrong-bone-road-stair";
            state.Map.InvalidateObjectLookup();
            Assert(!InvokePrivate<bool>(game, "CanDescend"), "an arbitrary Chapter II stair cannot impersonate the Bone Road passage");
            boneRoadPassage.Id = boneRoadPassageId;
            state.Map.InvalidateObjectLookup();
            Assert(InvokePrivate<bool>(game, "CanDescend"), "restoring the stable Bone Road id restores travel authorization");

            IReadOnlyList<RoamingThreatDefinition> depthTwoPatrols = RoamingThreatCatalog.ForDepth(2, false);
            Assert(depthTwoPatrols.Count == 3
                && depthTwoPatrols.Count(definition => definition.Faction == RoamingThreatFaction.Kobolds) == 2
                && depthTwoPatrols.Count(definition => definition.Faction == RoamingThreatFaction.Rats) == 1
                && depthTwoPatrols.SelectMany(definition => definition.EnemyIds)
                    .All(id => ContentSetCatalog.EnemyActive(ContentSetCatalog.SewerSlice, id)),
                "production Chapter II owns exactly two kobold patrols and one ratfolk patrol with active combat rosters");

            RoamingThreat cooledDepthTwoPatrol = state.RoamingThreats
                .First(threat => threat != null && threat.Active && threat.Depth == 2);
            string cooledDepthTwoPatrolId = cooledDepthTwoPatrol.Id;
            int cooledDepthTwoHomeX = cooledDepthTwoPatrol.HomeX;
            int cooledDepthTwoHomeY = cooledDepthTwoPatrol.HomeY;
            RoamingThreat sameIdOtherDepth = new RoamingThreat
            {
                Id = cooledDepthTwoPatrolId,
                Name = "Other-depth sentinel",
                Archetype = cooledDepthTwoPatrol.Archetype,
                Depth = 1,
                X = 1,
                Y = 1,
                HomeX = 1,
                HomeY = 1,
                Active = true,
                Alerted = true,
                GraceSteps = 77,
                RespawnSteps = 66
            };
            state.RoamingThreats.Insert(0, sameIdOtherDepth);
            InvokePrivate(game, "ResolveRoamingThreatRetreat", cooledDepthTwoPatrolId);
            Assert(sameIdOtherDepth.Active
                && sameIdOtherDepth.Alerted
                && sameIdOtherDepth.GraceSteps == 77
                && sameIdOtherDepth.RespawnSteps == 66,
                "retreat resolution changes only the same-id patrol on the current depth");
            InvokePrivate(game, "ResolveRoamingThreatVictory", cooledDepthTwoPatrolId);
            int cooledDepthTwoRespawnSteps = cooledDepthTwoPatrol.RespawnSteps;
            Assert(!cooledDepthTwoPatrol.Active
                && cooledDepthTwoRespawnSteps == RoamingThreatRules.DefeatRespawnSteps
                && sameIdOtherDepth.Active
                && sameIdOtherDepth.RespawnSteps == 66,
                "defeating a Chapter II patrol records its quiet-road cooldown before leaving the depth");

            int suppliesBeforeFirstBoneRoadArrival = state.Supplies;
            string progressionBeforeFirstBoneRoadArrival = PartyProgressionSignature(state);
            InvokePrivate(game, "Descend");
            Assert(state.Depth == 3
                && state.Map != null
                && state.Map.Depth == 3
                && state.StoryFlags.Contains(StoryFlags.BoneRoadEntered),
                "the stable Varkh passage reaches the production Bone Road chapter");
            Assert(state.Supplies == suppliesBeforeFirstBoneRoadArrival + 2
                && PartyProgressionSignature(state) != progressionBeforeFirstBoneRoadArrival,
                "first Bone Road arrival grants its one-time supplies and chapter experience");

            IReadOnlyList<RoamingThreatDefinition> depthThreePatrols = RoamingThreatCatalog.ForDepth(3, false);
            Assert(depthThreePatrols.Count == 3
                && depthThreePatrols.All(definition =>
                    definition.Faction == RoamingThreatFaction.Drow
                    || definition.Faction == RoamingThreatFaction.Undead)
                && depthThreePatrols.SelectMany(definition => definition.EnemyIds)
                    .All(id => ContentSetCatalog.EnemyActive(ContentSetCatalog.SewerSlice, id)),
                "production Chapter III owns exactly three active drow-or-undead patrol definitions");

            WorldMapSite[] chapterThreeSites = WorldMapGenerationRules.RegionalSites(
                state.Map.Width,
                state.Map.Height,
                state.Map.StartX,
                state.Map.StartY);
            IReadOnlyList<ArmoryRowView> unchartedJournal = InvokePrivate<IReadOnlyList<ArmoryRowView>>(game, "BuildArmoryJournalRows");
            Assert(chapterThreeSites.All(site => unchartedJournal.All(row => row.Title != site.Name)),
                "production Journal withholds every uncharted regional site");

            WorldMapSite cryptSite = chapterThreeSites.Single(site => site.Id == "gloam-deep-crypt");
            MapObject cryptLandmark = state.Map.FindObjectById("regional-site:gloam-deep-crypt");
            Assert(cryptLandmark != null && cryptLandmark.X == cryptSite.X && cryptLandmark.Y == cryptSite.Y,
                "Chapter III owns the exact Gloam Deep Crypt regional site");
            Assert(InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName") == "Gloam Deep Crypt",
                "first Bone Road guidance targets the exact Gloam Deep Crypt");

            Point cryptApproach = null;
            for (int y = Mathf.Max(1, cryptSite.Y - cryptSite.Radius); y <= Mathf.Min(state.Map.Height - 2, cryptSite.Y + cryptSite.Radius); y++)
            for (int x = Mathf.Max(1, cryptSite.X - cryptSite.Radius); x <= Mathf.Min(state.Map.Width - 2, cryptSite.X + cryptSite.Radius); x++)
            {
                if (!InvokePrivate<bool>(game, "CanStepExplore", x, y)) continue;
                IReadOnlyList<Point> path = InvokePrivate<List<Point>>(game, "FindLiveExplorePath", x, y);
                if (path.Count == 0) continue;
                cryptApproach = new Point(x, y);
                break;
            }
            Assert(cryptApproach != null, "Gloam Deep Crypt has a reachable walkable footprint cell for its watch trigger");
            state.PlayerX = cryptApproach.X;
            state.PlayerY = cryptApproach.Y;
            Assert(InvokePrivate<bool>(game, "MaybeTriggerBoneRoadWatch"), "entering the Gloam Deep Crypt footprint triggers the Bone Road watch");
            Assert(state.StoryFlags.Contains(StoryFlags.BoneRoadWatchSprung)
                && state.Mode == GameMode.Combat
                && state.Combat?.EncounterStyle == "bone-road-watch",
                "the footprint trigger opens the exact Bone Road watch encounter");
            state.Mode = GameMode.Explore;
            state.Combat = null;
            InvokePrivate(game, "InvalidateCombatController");
            Assert(InvokePrivate<bool>(game, "MaybeTriggerBoneRoadWatch")
                && state.Combat?.EncounterStyle == "bone-road-watch",
                "an unresolved Bone Road watch retriggers after retreat");

            InvokePrivate(game, "ApplyBoneRoadStoryVictory", "bone-road-watch");
            Assert(state.StoryFlags.Contains(StoryFlags.BoneRoadWatchDefeated)
                && !state.StoryFlags.Contains(StoryFlags.GloamRitualBroken),
                "watch victory advances only to the Gloam ritual step");
            state.Mode = GameMode.Explore;
            state.Combat = null;
            InvokePrivate(game, "InvalidateCombatController");
            Assert(InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName") == "Gloam Deep Crypt",
                "watch victory keeps guidance on the exact crypt entrance");

            InvokePrivate(game, "ResolveExploreObject", cryptLandmark);
            Assert(state.Mode == GameMode.Combat && state.Combat?.EncounterStyle == "gloam-crypt-ritual",
                "first crypt interaction after the watch starts the reliquary ritual encounter");
            Point cryptRitualGlyph = state.Combat.Obstacles.Single(CombatRitualRules.IsRitual);
            string cryptRitualPreview = InvokePrivate<string>(game, "TerrainPreviewLine", cryptRitualGlyph);
            string cryptRitualWarning = InvokePrivate<string>(game, "TerrainLogWarning", cryptRitualGlyph);
            Assert(cryptRitualPreview.IndexOf("reliquary bone priest", StringComparison.OrdinalIgnoreCase) >= 0
                && cryptRitualWarning.IndexOf("reliquary bone priest", StringComparison.OrdinalIgnoreCase) >= 0
                && cryptRitualPreview.IndexOf("kobold", StringComparison.OrdinalIgnoreCase) < 0
                && cryptRitualWarning.IndexOf("kobold", StringComparison.OrdinalIgnoreCase) < 0,
                "Gloam ritual hover and warning copy name the Bone Priest that will actually arrive");
            Assert(InvokePrivate<bool>(game, "TryOpenEnemyRitual", cryptRitualGlyph)
                && state.Combat.Units.Any(unit => unit.Side == UnitSide.Enemy
                    && unit.Origin == "ritual"
                    && unit.Role == "bonepriest"
                    && unit.Name == "Reliquary Bone Priest"),
                "the live Gloam ritual resolves its glyph into a production-active Bone Priest");
            IReadOnlyList<ArmoryRowView> readyCryptJournal = InvokePrivate<IReadOnlyList<ArmoryRowView>>(game, "BuildArmoryJournalRows");
            ArmoryRowView readyCryptRow = readyCryptJournal.Single(row => row.Title == "Gloam Deep Crypt");
            Assert(readyCryptRow.Badge == "READY" && readyCryptRow.Detail.IndexOf("Reliquary Vigil", StringComparison.OrdinalIgnoreCase) >= 0,
                "charted crypt appears in the Journal with its ready regional service");

            InvokePrivate(game, "ApplyBoneRoadStoryVictory", "gloam-crypt-ritual");
            Assert(state.StoryFlags.Contains(StoryFlags.GloamRitualBroken)
                && !state.StoryFlags.Contains(StoryFlags.GloamWardenDefeated),
                "ritual victory advances only to the Ossuary Warden step");
            state.Mode = GameMode.Explore;
            state.Combat = null;
            InvokePrivate(game, "InvalidateCombatController");
            InvokePrivate(game, "ResolveExploreObject", cryptLandmark);
            CombatUnit ossuaryWarden = state.Combat?.Units?.SingleOrDefault(unit => unit.Side == UnitSide.Enemy && unit.Role == "gloamknight");
            Assert(state.Mode == GameMode.Combat
                && state.Combat?.EncounterStyle == "gloam-warden-boss"
                && ossuaryWarden != null
                && ossuaryWarden.Name == "Ossuary Warden",
                "second crypt interaction opens the named Ossuary Warden boss encounter");
            Point wardenRift = state.Combat.Obstacles.Single(point => point.Kind == "demonrift");
            string wardenRiftPreview = InvokePrivate<string>(game, "TerrainPreviewLine", wardenRift);
            string wardenRiftWarning = InvokePrivate<string>(game, "TerrainLogWarning", wardenRift);
            Assert(wardenRiftPreview.IndexOf("Gloam Knight", StringComparison.OrdinalIgnoreCase) >= 0
                && wardenRiftWarning.IndexOf("Gloam Knight", StringComparison.OrdinalIgnoreCase) >= 0
                && wardenRiftPreview.IndexOf("lesser demon", StringComparison.OrdinalIgnoreCase) < 0
                && wardenRiftWarning.IndexOf("lesser demon", StringComparison.OrdinalIgnoreCase) < 0,
                "Warden breach hover and warning copy name the Gloam Knight that will actually arrive");
            Assert(InvokePrivate<bool>(game, "TryOpenEnemyRitual", wardenRift)
                && state.Combat.Units.Any(unit => unit.Side == UnitSide.Enemy
                    && unit.Origin == "ritual"
                    && unit.Role == "gloamknight"
                    && unit.Name == "Rift-bound Gloam Knight"),
                "the live Warden breach resolves into a production-active Gloam Knight instead of prototype demon content");

            int reliquaryMailBefore = state.Inventory.Count(item => item != null
                && item.DisplayName == "+4 Gloam Reliquary Mail"
                && item.SignatureId == SignatureItemCatalog.GloamReliquaryMailId);
            int suppliesBeforeWardenReward = state.Supplies;
            InvokePrivate(game, "ApplyBoneRoadStoryVictory", "gloam-warden-boss");
            Assert(state.StoryFlags.Contains(StoryFlags.GloamWardenDefeated)
                && state.Inventory.Count(item => item != null
                    && item.DisplayName == "+4 Gloam Reliquary Mail"
                    && item.SignatureId == SignatureItemCatalog.GloamReliquaryMailId) == reliquaryMailBefore + 1
                && state.Supplies == suppliesBeforeWardenReward + 1
                && GetPrivateField<string>(game, "lootPanelTitle").IndexOf("Ossuary Warden", StringComparison.OrdinalIgnoreCase) >= 0,
                "Ossuary Warden victory grants its named reliquary reward once");
            InvokePrivate(game, "ApplyBoneRoadStoryVictory", "gloam-warden-boss");
            Assert(state.Inventory.Count(item => item != null
                    && item.DisplayName == "+4 Gloam Reliquary Mail"
                    && item.SignatureId == SignatureItemCatalog.GloamReliquaryMailId) == reliquaryMailBefore + 1
                && state.Supplies == suppliesBeforeWardenReward + 1,
                "replaying the Warden victory callback cannot duplicate its reward");
            state.Mode = GameMode.Explore;
            state.Combat = null;
            InvokePrivate(game, "InvalidateCombatController");
            InvokePrivate(game, "DismissLootPopup");
            InvokePrivate(game, "ResolveExploreObject", cryptLandmark);
            ArmoryRowView claimedCryptRow = InvokePrivate<IReadOnlyList<ArmoryRowView>>(game, "BuildArmoryJournalRows")
                .Single(row => row.Title == "Gloam Deep Crypt");
            Assert(claimedCryptRow.Badge == "CLAIMED", "claiming the charted crypt service updates its Journal row from ready to claimed");
            Assert(claimedCryptRow.ActionEnabled && claimedCryptRow.ActionLabel == "Mark",
                "claimed regional-site Journal rows remain available as route waypoints");

            int previousSiteWaypointArmoryTab = GetPrivateField<int>(game, "armoryTab");
            SetPrivateField(game, "armoryTab", 3);
            InvokePrivate(game, "RunArmoryRowAction", claimedCryptRow.Key);
            Assert(RouteChartRules.IsSiteWaypoint(state.ActiveRouteWaypointKey, state.Depth, cryptSite.Id),
                "Journal Mark action persists the charted Gloam Deep Crypt site waypoint");
            InvokePrivate(game, "EnsureWorldState", VersionInfo.SaveVersion);
            Assert(RouteChartRules.IsSiteWaypoint(state.ActiveRouteWaypointKey, state.Depth, cryptSite.Id),
                "world-state repair preserves a valid charted-site waypoint across load normalization");
            ExplorationHudView markedCryptGuidance = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
            IReadOnlyList<Point> markedCryptPath = InvokePrivate<IReadOnlyList<Point>>(game, "CurrentExploreGuidancePath");
            Assert(InvokePrivate<bool>(game, "CurrentExploreGuidanceIsMarked")
                && InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName") == cryptSite.Name
                && markedCryptGuidance.WaypointLine.IndexOf("Marked:", StringComparison.OrdinalIgnoreCase) >= 0
                && markedCryptGuidance.WaypointLine.IndexOf(cryptSite.Name, StringComparison.OrdinalIgnoreCase) >= 0
                && markedCryptPath.Count > 0
                && markedCryptPath[0].X == state.PlayerX
                && markedCryptPath[0].Y == state.PlayerY,
                "charted-site marking drives one shared nonempty Gloam Deep Crypt HUD and map route");
            ArmoryRowView selectedCryptRow = InvokePrivate<IReadOnlyList<ArmoryRowView>>(game, "BuildArmoryJournalRows")
                .Single(row => row.Title == cryptSite.Name);
            Assert(selectedCryptRow.Selected && selectedCryptRow.ActionEnabled && selectedCryptRow.ActionLabel == "Clear",
                "the marked Gloam Deep Crypt Journal row becomes a selected Clear action");
            InvokePrivate(game, "RunArmoryRowAction", selectedCryptRow.Key);
            Assert(string.IsNullOrEmpty(state.ActiveRouteWaypointKey),
                "clearing the charted-site waypoint removes its transient route key");
            SetPrivateField(game, "armoryTab", previousSiteWaypointArmoryTab);

            WorldMapSite redGateSite = chapterThreeSites.Single(site => site.Id == "red-gate-seal");
            MapObject redGateLandmark = state.Map.FindObjectById("regional-site:red-gate-seal");
            Assert(redGateLandmark != null
                && InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName") == "Red Gate Seal",
                "Warden victory advances guidance to the exact Red Gate Seal");
            InvokePrivate(game, "ResolveExploreObject", redGateLandmark);
            Assert(state.StoryFlags.Contains(StoryFlags.RedGateWarningRecovered)
                && ContentSetCatalog.BoneRoadComplete(state.StoryFlags),
                "Red Gate interaction records the warning and completes Chapter III");
            List<MapObject> glassPassages = state.Map.Objects
                .Where(obj => obj != null && obj.Id == glassAndAshPassageId)
                .ToList();
            Assert(glassPassages.Count == 1
                && glassPassages[0].Type == ObjectType.Stairs
                && glassPassages[0].X == redGateSite.X
                && glassPassages[0].Y == redGateSite.Y - redGateSite.Radius,
                "Red Gate warning creates one Glass-and-Ash frontier at the stable outer-seal anchor");
            bool[,] redGateReachable = ExplorationTraversalRules.ReachableMask(state.Map, state.PlayerX, state.PlayerY);
            Assert(ExplorationTraversalRules.CanReachObject(redGateReachable, state.Map, glassPassages[0])
                && InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName") == "Glass-and-Ash Frontier",
                "the new Glass-and-Ash frontier is reachable and immediately owns guidance");

            state.Map.Objects.RemoveAll(obj => obj != null && obj.Id == glassAndAshPassageId);
            state.Map.InvalidateObjectLookup();
            state.ActiveStory = "Chapter III: stale objective at the crypt.";
            InvokePrivate(game, "EnsureWorldState", state.SaveVersion);
            InvokePrivate(game, "EnsureWorldLandmarks");
            glassPassages = state.Map.Objects.Where(obj => obj != null && obj.Id == glassAndAshPassageId).ToList();
            Assert(glassPassages.Count == 1
                && (state.ActiveStory ?? "").IndexOf("complete", StringComparison.OrdinalIgnoreCase) >= 0
                && (state.ActiveStory ?? "").IndexOf("survey", StringComparison.OrdinalIgnoreCase) >= 0
                && (state.ActiveStory ?? "").IndexOf("frontier", StringComparison.OrdinalIgnoreCase) >= 0,
                "load repair restores a missing Glass-and-Ash frontier and replaces stale travel copy");

            int settledSupplies = state.Supplies;
            int settledChapter = state.StoryChapter;
            string settledProgression = PartyProgressionSignature(state);
            InvokePrivate(game, "RecallToTempleSquare");
            Assert(state.Depth == 1, "Bone Road recall returns to Midgaard before the re-entry probe");
            MapObject oldRoadReentry = state.Map.FindObjectById("old-road-descent-sluice-steps");
            state.PlayerX = oldRoadReentry.X;
            state.PlayerY = oldRoadReentry.Y;
            InvokePrivate(game, "Descend");
            MapObject boneRoadReentry = state.Map.FindObjectById(boneRoadPassageId);
            Assert(state.Depth == 2 && boneRoadReentry != null, "completed campaign can re-enter Varkh's stable Bone Road passage");
            RoamingThreat persistedDepthTwoPatrol = state.RoamingThreats
                .Single(threat => threat != null
                    && threat.Depth == 2
                    && threat.Id == cooledDepthTwoPatrolId);
            Assert(!persistedDepthTwoPatrol.Active
                && persistedDepthTwoPatrol.HomeX == cooledDepthTwoHomeX
                && persistedDepthTwoPatrol.HomeY == cooledDepthTwoHomeY
                && persistedDepthTwoPatrol.RespawnSteps == cooledDepthTwoRespawnSteps
                && InvokePrivate<bool>(game, "CanStepExplore", cooledDepthTwoHomeX, cooledDepthTwoHomeY),
                "recall and re-entry preserve the defeated patrol cooldown, home, and walkable aftermath");
            state.PlayerX = boneRoadReentry.X;
            state.PlayerY = boneRoadReentry.Y;
            InvokePrivate(game, "Descend");
            Assert(state.Depth == 3
                && state.Supplies == settledSupplies
                && state.StoryChapter == settledChapter
                && PartyProgressionSignature(state) == settledProgression,
                "Bone Road re-entry duplicates no supplies, chapter progress, experience, levels, or points");

            MapObject reentryGlassPassage = state.Map.FindObjectById(glassAndAshPassageId);
            Assert(reentryGlassPassage != null, "completed Chapter III regenerates its stable Glass-and-Ash frontier on re-entry");
            state.PlayerX = reentryGlassPassage.X;
            state.PlayerY = reentryGlassPassage.Y;
            Assert(!InvokePrivate<bool>(game, "CanDescend")
                && InvokePrivate<bool>(game, "CanSurveyGlassAndAshFrontier"),
                "the completed Red Gate route is an interactive surveyed frontier, not an empty Chapter IV descent");
            reentryGlassPassage.Id = "wrong-glass-road-stair";
            state.Map.InvalidateObjectLookup();
            Assert(!InvokePrivate<bool>(game, "CanDescend")
                && !InvokePrivate<bool>(game, "CanSurveyGlassAndAshFrontier"),
                "an arbitrary Chapter III stair cannot impersonate the Glass-and-Ash frontier");
            reentryGlassPassage.Id = glassAndAshPassageId;
            state.Map.InvalidateObjectLookup();
            int frontierSupplies = state.Supplies;
            string frontierProgression = PartyProgressionSignature(state);
            InvokePrivate(game, "Descend");
            string completedGuidanceTarget = InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName") ?? "";
            ExplorationHudView completedFrontierView = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
            Assert(state.Depth == 3
                && state.Map != null
                && state.Map.Depth == 3
                && state.StoryFlags.Contains(StoryFlags.GlassAndAshFrontierSurveyed)
                && state.Supplies == frontierSupplies
                && PartyProgressionSignature(state) == frontierProgression
                && (state.ActiveStory ?? "").IndexOf("Return to Midgaard", StringComparison.OrdinalIgnoreCase) >= 0
                && completedFrontierView.ObjectiveSummary.IndexOf("Chapter III is complete", StringComparison.OrdinalIgnoreCase) >= 0
                && completedGuidanceTarget.IndexOf("Glass-and-Ash", StringComparison.OrdinalIgnoreCase) < 0
                && completedGuidanceTarget.IndexOf("Red Gate", StringComparison.OrdinalIgnoreCase) < 0
                && completedGuidanceTarget.IndexOf("Bone Road", StringComparison.OrdinalIgnoreCase) < 0
                && completedGuidanceTarget.IndexOf("Old Road", StringComparison.OrdinalIgnoreCase) < 0,
                "surveying the frontier completes its epilogue without entering unfinished content or duplicating progression");
            InvokePrivate(game, "Descend");
            Assert(state.Depth == 3
                && state.Supplies == frontierSupplies
                && PartyProgressionSignature(state) == frontierProgression,
                "re-surveying the frontier remains safe and reward-free");

            InvokePrivate(game, "RecallToTempleSquare");
            string completedMidgaardTarget = InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName") ?? "";
            ExplorationHudView completedMidgaardView = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
            Assert(state.Depth == 1
                && (state.ActiveStory ?? "").IndexOf("Chapter III complete", StringComparison.OrdinalIgnoreCase) >= 0
                && (state.ActiveStory ?? "").IndexOf("Chapter IV", StringComparison.OrdinalIgnoreCase) < 0
                && (state.ActiveStory ?? "").IndexOf("gate key", StringComparison.OrdinalIgnoreCase) < 0
                && completedMidgaardView.ObjectiveSummary.IndexOf("Yara", StringComparison.OrdinalIgnoreCase) >= 0
                && completedMidgaardView.ObjectiveSummary.IndexOf("Glass Road", StringComparison.OrdinalIgnoreCase) >= 0
                && (completedMidgaardTarget.IndexOf("Yara", StringComparison.OrdinalIgnoreCase) >= 0
                    || completedMidgaardTarget.IndexOf("Old Road Scout", StringComparison.OrdinalIgnoreCase) >= 0
                    || completedMidgaardTarget.IndexOf("Town Hall storm doors", StringComparison.OrdinalIgnoreCase) >= 0)
                && completedMidgaardTarget.IndexOf("Eastbound Old Road", StringComparison.OrdinalIgnoreCase) < 0
                && completedMidgaardTarget.IndexOf("Bone Road", StringComparison.OrdinalIgnoreCase) < 0
                && completedMidgaardTarget.IndexOf("Glass-and-Ash", StringComparison.OrdinalIgnoreCase) < 0,
                "a surveyed Chapter III campaign returns to Yara without silently opening Chapter IV or retaining stale road guidance"
                + $" (story='{state.ActiveStory}', objective='{completedMidgaardView.ObjectiveSummary}', target='{completedMidgaardTarget}')");

            MapObject completedOldRoad = state.Map.FindObjectById("old-road-descent-sluice-steps");
            state.PlayerX = completedOldRoad.X;
            state.PlayerY = completedOldRoad.Y;
            InvokePrivate(game, "Descend");
            string completedDepthTwoTarget = InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName") ?? "";
            Assert(state.Depth == 2
                && (state.ActiveStory ?? "").IndexOf("Chapter III complete", StringComparison.OrdinalIgnoreCase) >= 0
                && (state.ActiveStory ?? "").IndexOf("Chapter IV", StringComparison.OrdinalIgnoreCase) < 0
                && completedDepthTwoTarget.IndexOf("Bone Road", StringComparison.OrdinalIgnoreCase) < 0,
                "completed Chapter III remains the production objective during optional Bone Road re-entry without reopening its guidance");

            AssertGlassAndAshStoryFlow(game, state);
        }

        private static void AssertGlassAndAshStoryFlow(AshenHallsGame game, GameState state)
        {
            const string oldRoadPassageId = "old-road-descent-sluice-steps";
            const string boneRoadPassageId = "bone-road-passage-varkh-hall";
            const string glassRoadPassageId = "glass-and-ash-passage-red-gate";
            const string glassLibraryObjectId = "regional-site:glass-lore-library";
            const string redGateObjectId = "regional-site:red-gate-seal";
            const string mantleName = "+5 Mirrorweave Road Mantle";

            InvokePrivate(game, "RecallToTempleSquare");
            Assert(state.Depth == 1
                && !state.StoryFlags.Contains(StoryFlags.GlassAndAshExpeditionAccepted),
                "a surveyed v2.10 campaign returns to Midgaard before Chapter IV is accepted");
            string yaraGuidance = InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName") ?? "";
            Assert(yaraGuidance.IndexOf("Yara", StringComparison.OrdinalIgnoreCase) >= 0
                || yaraGuidance.IndexOf("Old Road Scout", StringComparison.OrdinalIgnoreCase) >= 0
                || yaraGuidance.IndexOf("Town Hall storm doors", StringComparison.OrdinalIgnoreCase) >= 0,
                "the completed frontier survey guides the party to Yara instead of reopening the road silently");

            string progressionBeforeBriefing = PartyProgressionSignature(state);
            InvokePrivate(game, "ShowYaraConversation");
            InvokePrivate(game, "LateUpdate");
            DialogueScreen dialogue = GetPrivateField<DialogueScreen>(game, "dialogueScreen");
            DialogueChoiceView[] choices = GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices");
            Assert(dialogue != null
                && choices.Length == 3
                && choices[0].Id == "expedition"
                && choices[0].Primary,
                "Yara presents the Glass Road plan as the clear next campaign choice");

            dialogue.InvokeChoiceForTest(0);
            InvokePrivate(game, "LateUpdate");
            choices = GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices");
            string briefing = GetPrivateField<string>(game, "dialogueBody") ?? "";
            Assert(dialogue.VisibleChoiceCountForTest == 2
                && choices.Length == 2
                && choices[0].Id == "accept_expedition"
                && choices[0].Primary
                && briefing.IndexOf("Glass Lore Library", StringComparison.OrdinalIgnoreCase) >= 0
                && briefing.IndexOf("recall", StringComparison.OrdinalIgnoreCase) >= 0,
                "Yara's review names the exact first landmark and retreat rule before campaign mutation");
            Assert(!state.StoryFlags.Contains(StoryFlags.GlassAndAshExpeditionAccepted)
                && PartyProgressionSignature(state) == progressionBeforeBriefing,
                "opening the Glass Road briefing changes no story or party progression");

            dialogue.InvokeChoiceForTest(1);
            InvokePrivate(game, "LateUpdate");
            Assert(!state.StoryFlags.Contains(StoryFlags.GlassAndAshExpeditionAccepted)
                && GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices").Length == 3,
                "backing out of Yara's briefing returns to conversation without accepting Chapter IV");
            dialogue.InvokeChoiceForTest(0);
            InvokePrivate(game, "LateUpdate");
            dialogue.InvokeChoiceForTest(0);
            InvokePrivate(game, "LateUpdate");
            Assert(state.StoryFlags.Contains(StoryFlags.GlassAndAshExpeditionAccepted)
                && (state.ActiveStory ?? "").IndexOf("Chapter IV", StringComparison.OrdinalIgnoreCase) >= 0
                && PartyProgressionSignature(state) != progressionBeforeBriefing,
                "explicit acceptance opens Glass and Ash and awards its one-time briefing experience");
            InvokePrivate(game, "CloseDialogue");
            InvokePrivate(game, "LateUpdate");

            Assert(InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName") == "Eastbound Old Road",
                "accepted Chapter IV guidance begins on Midgaard's named eastbound Old Road");
            MapObject oldRoad = state.Map.FindObjectById(oldRoadPassageId);
            Assert(oldRoad != null, "the Chapter IV return route preserves the stable Old Road marker");
            state.PlayerX = oldRoad.X;
            state.PlayerY = oldRoad.Y;
            InvokePrivate(game, "Descend");
            MapObject boneRoad = state.Map.FindObjectById(boneRoadPassageId);
            Assert(state.Depth == 2
                && boneRoad != null
                && InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName") == "Bone Road Passage",
                "the Glass Road expedition reuses Varkh's stable Chapter II passage without a teleport");
            state.PlayerX = boneRoad.X;
            state.PlayerY = boneRoad.Y;
            InvokePrivate(game, "Descend");
            MapObject glassRoad = state.Map.FindObjectById(glassRoadPassageId);
            Assert(state.Depth == 3
                && glassRoad != null,
                "the accepted expedition regains the stable Red Gate passage on the Bone Road");
            state.PlayerX = glassRoad.X;
            state.PlayerY = glassRoad.Y;
            Assert(!InvokePrivate<bool>(game, "CanSurveyGlassAndAshFrontier")
                && InvokePrivate<bool>(game, "CanDescend"),
                "after Yara's briefing the frontier changes from survey action to authorized crossing");
            glassRoad.Id = "wrong-glass-crossing";
            state.Map.InvalidateObjectLookup();
            Assert(!InvokePrivate<bool>(game, "CanDescend"),
                "an arbitrary Chapter III stair cannot impersonate the accepted Glass Road crossing");
            glassRoad.Id = glassRoadPassageId;
            state.Map.InvalidateObjectLookup();

            int suppliesBeforeFirstCrossing = state.Supplies;
            string progressionBeforeFirstCrossing = PartyProgressionSignature(state);
            InvokePrivate(game, "Descend");
            Assert(state.Depth == 4
                && state.Map != null
                && state.Map.Depth == 4
                && state.StoryFlags.Contains(StoryFlags.GlassAndAshEntered)
                && state.Supplies == suppliesBeforeFirstCrossing + 2
                && PartyProgressionSignature(state) != progressionBeforeFirstCrossing,
                "the exact passage enters authored Chapter IV and grants first-arrival progression once");
            Assert(state.RoamingThreats.Count(threat => threat != null && threat.Depth == 4) == 3
                && state.RoamingThreats.Where(threat => threat != null && threat.Depth == 4)
                    .All(threat => threat.Active),
                "Glass and Ash arrives with exactly three live production patrol bands");
            Assert(!state.Map.Objects.Any(obj => obj != null && obj.Type == ObjectType.Stairs),
                "Chapter IV exposes no generic stair or accidental Chapter V route");

            MapObject glassLibrary = state.Map.FindObjectById(glassLibraryObjectId);
            MapObject farSeal = state.Map.FindObjectById(redGateObjectId);
            Assert(glassLibrary != null
                && farSeal != null
                && InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName") == "Glass Lore Library",
                "first Chapter IV guidance targets the exact Glass Lore Library while preserving the far seal");

            InvokePrivate(game, "ResolveExploreObject", farSeal);
            Assert(state.Mode == GameMode.Explore
                && !state.StoryFlags.Contains(StoryFlags.EmberglassGateKeyRecovered),
                "the far seal cannot skip the missing Mirror Index or start its boss early");

            InvokePrivate(game, "ResolveExploreObject", glassLibrary);
            Assert(state.Mode == GameMode.Combat
                && state.Combat?.EncounterStyle == "glassward-ambush",
                "first library interaction opens the authored Glass-Warren levy");
            state.Mode = GameMode.Explore;
            state.Combat = null;
            InvokePrivate(game, "InvalidateCombatController");
            InvokePrivate(game, "ResolveExploreObject", glassLibrary);
            Assert(state.Mode == GameMode.Combat
                && state.Combat?.EncounterStyle == "glassward-ambush",
                "retreating from the levy leaves its exact encounter available for retry");
            InvokePrivate(game, "ApplyGlassAndAshStoryVictory", "glassward-ambush");
            Assert(state.StoryFlags.Contains(StoryFlags.GlasswardAmbushDefeated)
                && !state.StoryFlags.Contains(StoryFlags.GlassIndexRecovered),
                "levy victory advances only to the Mirror Index step");
            state.Mode = GameMode.Explore;
            state.Combat = null;
            InvokePrivate(game, "InvalidateCombatController");

            int suppliesBeforeIndex = state.Supplies;
            InvokePrivate(game, "ResolveExploreObject", glassLibrary);
            Assert(state.Mode == GameMode.Combat
                && state.Combat?.EncounterStyle == "glass-index-keepers",
                "returning to the library opens the distinct Mirror Index keepers encounter");
            InvokePrivate(game, "ApplyGlassAndAshStoryVictory", "glass-index-keepers");
            Assert(state.StoryFlags.Contains(StoryFlags.GlassIndexRecovered)
                && state.Supplies == suppliesBeforeIndex + 1,
                "Index victory records the true-road map and its one-time field supply");
            InvokePrivate(game, "ApplyGlassAndAshStoryVictory", "glass-index-keepers");
            Assert(state.Supplies == suppliesBeforeIndex + 1,
                "replaying the Index victory callback cannot duplicate its supply reward");
            state.Mode = GameMode.Explore;
            state.Combat = null;
            InvokePrivate(game, "InvalidateCombatController");
            Assert(InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName") == "Red Gate Seal",
                "the recovered Mirror Index advances guidance to the exact far seal");

            int mantleBefore = state.Inventory.Count(item => item != null
                && item.DisplayName == mantleName
                && item.SignatureId == SignatureItemCatalog.MirrorweaveRoadMantleId);
            int suppliesBeforeWarden = state.Supplies;
            InvokePrivate(game, "ResolveExploreObject", farSeal);
            CombatUnit pactWarden = state.Combat?.Units?.FirstOrDefault(unit => unit != null
                && unit.Side == UnitSide.Enemy
                && unit.Role == "lesserdemon");
            Assert(state.Mode == GameMode.Combat
                && state.Combat?.EncounterStyle == "ashen-pact-warden-boss"
                && pactWarden?.Name == "Warden of the Ashen Pact",
                "the Mirror Index opens the named Ashen Pact boss instead of a generic depth-five fight");
            InvokePrivate(game, "ApplyGlassAndAshStoryVictory", "ashen-pact-warden-boss");
            Assert(ContentSetCatalog.GlassAndAshComplete(state.StoryFlags)
                && state.Inventory.Count(item => item != null
                    && item.DisplayName == mantleName
                    && item.SignatureId == SignatureItemCatalog.MirrorweaveRoadMantleId) == mantleBefore + 1
                && state.Supplies == suppliesBeforeWarden + 2
                && state.StoryChapter == 5
                && (state.ActiveStory ?? "").IndexOf("Chapter IV complete", StringComparison.OrdinalIgnoreCase) >= 0,
                "Ashen Pact victory grants the Emberglass key, unique mantle, supplies, and honest chapter endpoint");
            InvokePrivate(game, "ApplyGlassAndAshStoryVictory", "ashen-pact-warden-boss");
            Assert(state.Inventory.Count(item => item != null
                    && item.DisplayName == mantleName
                    && item.SignatureId == SignatureItemCatalog.MirrorweaveRoadMantleId) == mantleBefore + 1
                && state.Supplies == suppliesBeforeWarden + 2,
                "replaying the Ashen Pact callback cannot duplicate the unique reward");
            state.Mode = GameMode.Explore;
            state.Combat = null;
            InvokePrivate(game, "InvalidateCombatController");

            IReadOnlyList<ArmoryRowView> completedJournal = InvokePrivate<IReadOnlyList<ArmoryRowView>>(game, "BuildArmoryJournalRows");
            Assert(completedJournal.Count(row => row.Title.StartsWith("Glass and Ash - ", StringComparison.Ordinal)) == 4
                && completedJournal.Where(row => row.Title.StartsWith("Glass and Ash - ", StringComparison.Ordinal))
                    .All(row => row.Subtitle == "done"),
                "the production Journal records all four Glass and Ash beats as complete");
            Assert(!state.Map.Objects.Any(obj => obj != null && obj.Type == ObjectType.Stairs)
                && !InvokePrivate<bool>(game, "CanDescend"),
                "the completed chapter cannot bypass Yara's separate Chapter V briefing");

            int settledSupplies = state.Supplies;
            string settledProgression = PartyProgressionSignature(state);
            InvokePrivate(game, "RecallToTempleSquare");
            string completedTarget = InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName") ?? "";
            ExplorationHudView awaitingDebriefView = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
            Assert(state.Depth == 1
                && !state.StoryFlags.Contains(StoryFlags.GlassAndAshDebriefed)
                && (state.ActiveStory ?? "").IndexOf("Chapter IV complete", StringComparison.OrdinalIgnoreCase) >= 0
                && awaitingDebriefView.ObjectiveSummary.IndexOf("Yara", StringComparison.OrdinalIgnoreCase) >= 0
                && (completedTarget.IndexOf("Yara", StringComparison.OrdinalIgnoreCase) >= 0
                    || completedTarget.IndexOf("Old Road Scout", StringComparison.OrdinalIgnoreCase) >= 0
                    || completedTarget.IndexOf("Town Hall storm doors", StringComparison.OrdinalIgnoreCase) >= 0)
                && completedTarget.IndexOf("Eastbound Old Road", StringComparison.OrdinalIgnoreCase) < 0
                && completedTarget.IndexOf("Bone Road", StringComparison.OrdinalIgnoreCase) < 0
                && completedTarget.IndexOf("Glass", StringComparison.OrdinalIgnoreCase) < 0,
                "Chapter IV recall routes the recovered key to Yara without reopening a completed road objective");

            InvokePrivate(game, "ShowYaraConversation");
            InvokePrivate(game, "LateUpdate");
            dialogue = GetPrivateField<DialogueScreen>(game, "dialogueScreen");
            ExplorationHudView debriefedView = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
            Assert(state.StoryFlags.Contains(StoryFlags.GlassAndAshDebriefed)
                && (state.ActiveStory ?? "").IndexOf("bounded assault", StringComparison.OrdinalIgnoreCase) >= 0
                && debriefedView.ObjectiveSummary.IndexOf("Red Gate", StringComparison.OrdinalIgnoreCase) >= 0
                && debriefedView.ObjectiveSummary.IndexOf("bring Yara", StringComparison.OrdinalIgnoreCase) < 0
                && (GetPrivateField<string>(game, "dialogueBody") ?? "").IndexOf("brought back a road", StringComparison.OrdinalIgnoreCase) >= 0,
                "Yara's first post-key conversation closes the debrief durably and exposes only a reviewed Chapter V plan");
            InvokePrivate(game, "CloseDialogue");
            InvokePrivate(game, "LateUpdate");

            oldRoad = state.Map.FindObjectById(oldRoadPassageId);
            state.PlayerX = oldRoad.X;
            state.PlayerY = oldRoad.Y;
            InvokePrivate(game, "Descend");
            boneRoad = state.Map.FindObjectById(boneRoadPassageId);
            state.PlayerX = boneRoad.X;
            state.PlayerY = boneRoad.Y;
            InvokePrivate(game, "Descend");
            glassRoad = state.Map.FindObjectById(glassRoadPassageId);
            string completedGlassRoadHint = InvokePrivate<string>(game, "ObjectHint", glassRoad) ?? "";
            Assert(completedGlassRoadHint.IndexOf("revisit", StringComparison.OrdinalIgnoreCase) >= 0
                && completedGlassRoadHint.IndexOf("Red Gate", StringComparison.OrdinalIgnoreCase) >= 0,
                "the completed Glass Road hint points to the revisitable Chapter V country instead of the retired depth-five boundary");
            state.PlayerX = glassRoad.X;
            state.PlayerY = glassRoad.Y;
            Assert(InvokePrivate<bool>(game, "CanDescend"),
                "a completed company may revisit the secured Glass-and-Ash map through the same exact passage");
            InvokePrivate(game, "Descend");
            Assert(state.Depth == 4
                && state.Supplies == settledSupplies
                && PartyProgressionSignature(state) == settledProgression
                && !state.Map.Objects.Any(obj => obj != null && obj.Type == ObjectType.Stairs),
                "Chapter IV re-entry duplicates no progression and exposes no generic Chapter V stair");
            MapObject lockedFarSeal = state.Map.FindObjectById(redGateObjectId);
            int goldBeforeLockedSeal = state.Gold;
            int inventoryBeforeLockedSeal = state.Inventory.Count;
            string storyBeforeLockedSeal = state.ActiveStory;
            InvokePrivate(game, "ResolveExploreObject", lockedFarSeal);
            Assert(state.Depth == 4
                && state.Mode == GameMode.Explore
                && !state.StoryFlags.Contains(StoryFlags.RedGateAssaultAccepted)
                && state.Gold == goldBeforeLockedSeal
                && state.Supplies == settledSupplies
                && state.Inventory.Count == inventoryBeforeLockedSeal
                && state.ActiveStory == storyBeforeLockedSeal
                && PartyProgressionSignature(state) == settledProgression,
                "the far seal remains a mutation-free contract boundary until Yara's Red Gate plan is accepted");

            AssertRedGateStoryFlow(game, state);
            AssertAdvancedFullPrototypeRecallPreservesStory(game, state);
        }

        private static void AssertRedGateStoryFlow(AshenHallsGame game, GameState state)
        {
            const string oldRoadPassageId = "old-road-descent-sluice-steps";
            const string boneRoadPassageId = "bone-road-passage-varkh-hall";
            const string glassRoadPassageId = "glass-and-ash-passage-red-gate";
            const string redGateObjectId = "regional-site:red-gate-seal";
            const string gloamCryptObjectId = "regional-site:gloam-deep-crypt";
            const string saltCisternObjectId = "regional-site:salt-cistern-gate";
            const string warbladeName = "+6 Crownward Emberglass Warblade";

            InvokePrivate(game, "RecallToTempleSquare");
            Assert(state.Depth == 1
                && state.StoryFlags.Contains(StoryFlags.GlassAndAshDebriefed)
                && !state.StoryFlags.Contains(StoryFlags.RedGateAssaultAccepted),
                "a debriefed Chapter IV company returns to Midgaard before Chapter V is accepted");
            string yaraTarget = InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName") ?? "";
            Assert(yaraTarget.IndexOf("Yara", StringComparison.OrdinalIgnoreCase) >= 0
                || yaraTarget.IndexOf("Old Road Scout", StringComparison.OrdinalIgnoreCase) >= 0
                || yaraTarget.IndexOf("Town Hall storm doors", StringComparison.OrdinalIgnoreCase) >= 0,
                "the copied Emberglass key routes the party to Yara's reviewed Red Gate plan");

            string progressionBeforeBriefing = PartyProgressionSignature(state);
            InvokePrivate(game, "ShowYaraConversation");
            InvokePrivate(game, "LateUpdate");
            DialogueScreen dialogue = GetPrivateField<DialogueScreen>(game, "dialogueScreen");
            DialogueChoiceView[] choices = GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices");
            Assert(dialogue != null
                && choices.Length == 3
                && choices[0].Id == "redgateplan"
                && choices[0].Primary,
                "Yara presents the Red Gate plan as the clear next campaign choice");

            dialogue.InvokeChoiceForTest(0);
            InvokePrivate(game, "LateUpdate");
            choices = GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices");
            string briefing = GetPrivateField<string>(game, "dialogueBody") ?? "";
            Assert(dialogue.VisibleChoiceCountForTest == 2
                && choices.Length == 2
                && choices[0].Id == "accept_red_gate"
                && choices[0].Primary
                && briefing.IndexOf("Gloam Deep Crypt", StringComparison.OrdinalIgnoreCase) >= 0
                && briefing.IndexOf("Salt Cistern Gate", StringComparison.OrdinalIgnoreCase) >= 0
                && briefing.IndexOf("recall", StringComparison.OrdinalIgnoreCase) >= 0,
                "Yara's Red Gate review names the ordered sites and retreat boundary before campaign mutation");
            Assert(!state.StoryFlags.Contains(StoryFlags.RedGateAssaultAccepted)
                && PartyProgressionSignature(state) == progressionBeforeBriefing,
                "opening the Red Gate review changes no story or party progression");

            dialogue.InvokeChoiceForTest(1);
            InvokePrivate(game, "LateUpdate");
            Assert(!state.StoryFlags.Contains(StoryFlags.RedGateAssaultAccepted)
                && GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices").Length == 3,
                "backing out of the Red Gate review returns to Yara without accepting Chapter V");
            dialogue.InvokeChoiceForTest(0);
            InvokePrivate(game, "LateUpdate");
            dialogue.InvokeChoiceForTest(0);
            InvokePrivate(game, "LateUpdate");
            Assert(state.StoryFlags.Contains(StoryFlags.RedGateAssaultAccepted)
                && (state.ActiveStory ?? "").IndexOf("Chapter V", StringComparison.OrdinalIgnoreCase) >= 0
                && PartyProgressionSignature(state) != progressionBeforeBriefing,
                "explicit acceptance opens The Red Gate and awards its one-time briefing experience");
            InvokePrivate(game, "CloseDialogue");
            InvokePrivate(game, "LateUpdate");

            Assert(InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName") == "Eastbound Old Road",
                "accepted Chapter V guidance begins on Midgaard's named eastbound Old Road");
            MapObject oldRoad = state.Map.FindObjectById(oldRoadPassageId);
            state.PlayerX = oldRoad.X;
            state.PlayerY = oldRoad.Y;
            InvokePrivate(game, "Descend");
            MapObject boneRoad = state.Map.FindObjectById(boneRoadPassageId);
            Assert(state.Depth == 2 && boneRoad != null,
                "the Red Gate assault reuses the stable Chapter II road passage");
            state.PlayerX = boneRoad.X;
            state.PlayerY = boneRoad.Y;
            InvokePrivate(game, "Descend");
            MapObject glassRoad = state.Map.FindObjectById(glassRoadPassageId);
            Assert(state.Depth == 3 && glassRoad != null,
                "the Red Gate assault regains the surveyed Glass Road on the Bone Road");
            state.PlayerX = glassRoad.X;
            state.PlayerY = glassRoad.Y;
            InvokePrivate(game, "Descend");
            MapObject farSeal = state.Map.FindObjectById(redGateObjectId);
            Assert(state.Depth == 4
                && farSeal != null
                && InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName") == "Red Gate Seal"
                && !state.Map.Objects.Any(obj => obj != null && obj.Type == ObjectType.Stairs),
                "Chapter V returns through authored Chapter IV and targets the far seal without creating a generic stair");

            int suppliesBeforeFirstEntry = state.Supplies;
            string progressionBeforeFirstEntry = PartyProgressionSignature(state);
            InvokePrivate(game, "ResolveExploreObject", farSeal);
            Assert(state.Depth == 5
                && state.Map != null
                && state.Map.Depth == 5
                && state.StoryFlags.Contains(StoryFlags.RedGateEntered)
                && state.Supplies == suppliesBeforeFirstEntry + 2
                && PartyProgressionSignature(state) != progressionBeforeFirstEntry,
                "the exact far seal enters authored Chapter V and grants first-arrival progression once");
            Assert(state.RoamingThreats.Count(threat => threat != null && threat.Depth == 5) == 3
                && state.RoamingThreats.Where(threat => threat != null && threat.Depth == 5).All(threat => threat.Active),
                "The Red Gate arrives with exactly three live production patrol bands");
            Assert(!state.Map.Objects.Any(obj => obj != null && obj.Type == ObjectType.Stairs)
                && !InvokePrivate<bool>(game, "CanDescend"),
                "Chapter V exposes no generic stair or accidental final-chapter route");

            MapObject innerGate = state.Map.FindObjectById(redGateObjectId);
            MapObject gloamCrypt = state.Map.FindObjectById(gloamCryptObjectId);
            MapObject saltCistern = state.Map.FindObjectById(saltCisternObjectId);
            Assert(innerGate != null
                && gloamCrypt != null
                && saltCistern != null
                && InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName") == "Red Gate Seal",
                "first Chapter V guidance targets the inner gate while preserving both later authored sites");

            InvokePrivate(game, "ResolveExploreObject", gloamCrypt);
            Assert(state.Mode == GameMode.Explore
                && !state.StoryFlags.Contains(StoryFlags.OssuaryRoadSealRecovered),
                "Gloam Deep cannot skip the missing crownward tally or start its encounter early");

            InvokePrivate(game, "ResolveExploreObject", innerGate);
            Assert(state.Mode == GameMode.Combat
                && state.Combat?.EncounterStyle == "red-gate-vanguard",
                "first inner-gate interaction opens the authored cinder vanguard");
            state.Mode = GameMode.Explore;
            state.Combat = null;
            InvokePrivate(game, "InvalidateCombatController");
            InvokePrivate(game, "ResolveExploreObject", innerGate);
            Assert(state.Mode == GameMode.Combat
                && state.Combat?.EncounterStyle == "red-gate-vanguard",
                "retreating from the vanguard leaves its exact encounter available for retry");
            InvokePrivate(game, "ApplyRedGateStoryVictory", "red-gate-vanguard");
            Assert(state.StoryFlags.Contains(StoryFlags.RedGateVanguardDefeated)
                && !state.StoryFlags.Contains(StoryFlags.OssuaryRoadSealRecovered),
                "vanguard victory advances only to the ossuary road-seal step");
            state.Mode = GameMode.Explore;
            state.Combat = null;
            InvokePrivate(game, "InvalidateCombatController");
            Assert(InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName") == "Gloam Deep Crypt",
                "the recovered tally advances guidance to the exact Gloam Deep Crypt");

            InvokePrivate(game, "ResolveExploreObject", saltCistern);
            Assert(state.Mode == GameMode.Explore
                && !state.StoryFlags.Contains(StoryFlags.CrownroadMarshalDefeated),
                "Salt Cistern Gate cannot skip the missing ossuary road seal");

            int suppliesBeforeSeal = state.Supplies;
            InvokePrivate(game, "ResolveExploreObject", gloamCrypt);
            Assert(state.Mode == GameMode.Combat
                && state.Combat?.EncounterStyle == "ossuary-road-seal",
                "the recovered tally opens the distinct Ossuary Road Seal encounter");
            InvokePrivate(game, "ApplyRedGateStoryVictory", "ossuary-road-seal");
            Assert(state.StoryFlags.Contains(StoryFlags.OssuaryRoadSealRecovered)
                && state.Supplies == suppliesBeforeSeal + 1,
                "Ossuary victory records the true-road seal and its one-time field supply");
            InvokePrivate(game, "ApplyRedGateStoryVictory", "ossuary-road-seal");
            Assert(state.Supplies == suppliesBeforeSeal + 1,
                "replaying the Ossuary callback cannot duplicate its supply reward");
            state.Mode = GameMode.Explore;
            state.Combat = null;
            InvokePrivate(game, "InvalidateCombatController");
            Assert(InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName") == "Salt Cistern Gate",
                "the recovered road seal advances guidance to the exact Salt Cistern threshold");

            int warbladesBefore = state.Inventory.Count(item => item != null
                && item.DisplayName == warbladeName
                && item.SignatureId == SignatureItemCatalog.CrownwardWarbladeId);
            int suppliesBeforeMarshal = state.Supplies;
            InvokePrivate(game, "ResolveExploreObject", saltCistern);
            CombatUnit marshal = state.Combat?.Units?.FirstOrDefault(unit => unit != null
                && unit.Side == UnitSide.Enemy
                && unit.Role == "lesserdemon");
            Assert(state.Mode == GameMode.Combat
                && state.Combat?.EncounterStyle == "crownroad-marshal-boss"
                && marshal?.Name == "Marshal of the Crownroad",
                "the ossuary seal opens the named Crownroad Marshal instead of the prototype final boss");
            InvokePrivate(game, "ApplyRedGateStoryVictory", "crownroad-marshal-boss");
            Assert(state.StoryFlags.Contains(StoryFlags.CrownroadMarshalDefeated)
                && !ContentSetCatalog.RedGateComplete(state.StoryFlags)
                && state.Inventory.Count(item => item != null
                    && item.DisplayName == warbladeName
                    && item.SignatureId == SignatureItemCatalog.CrownwardWarbladeId) == warbladesBefore + 1
                && state.Supplies == suppliesBeforeMarshal + 2
                && (state.ActiveStory ?? "").IndexOf("Inspect", StringComparison.OrdinalIgnoreCase) >= 0,
                "marshal victory grants the unique warblade and leaves the threshold survey explicit");
            InvokePrivate(game, "ApplyRedGateStoryVictory", "crownroad-marshal-boss");
            Assert(state.Inventory.Count(item => item != null
                    && item.DisplayName == warbladeName
                    && item.SignatureId == SignatureItemCatalog.CrownwardWarbladeId) == warbladesBefore + 1
                && state.Supplies == suppliesBeforeMarshal + 2,
                "replaying the marshal callback cannot duplicate its unique reward");
            state.Mode = GameMode.Explore;
            state.Combat = null;
            InvokePrivate(game, "InvalidateCombatController");
            InvokePrivate(game, "DismissLootPopup");
            InvokePrivate(game, "LateUpdate");

            InvokePrivate(game, "ResolveExploreObject", saltCistern);
            Assert(ContentSetCatalog.RedGateComplete(state.StoryFlags)
                && state.StoryFlags.Contains(StoryFlags.MeteorCrownThresholdSurveyed)
                && state.StoryChapter == 6
                && (state.ActiveStory ?? "").IndexOf("Chapter V complete", StringComparison.OrdinalIgnoreCase) >= 0,
                "surveying the post-marshal threshold completes Chapter V without entering Chapter VI");
            IReadOnlyList<ArmoryRowView> completedJournal = InvokePrivate<IReadOnlyList<ArmoryRowView>>(game, "BuildArmoryJournalRows");
            Assert(completedJournal.Count(row => row.Title.StartsWith("The Red Gate - ", StringComparison.Ordinal)) == 5
                && completedJournal.Where(row => row.Title.StartsWith("The Red Gate - ", StringComparison.Ordinal))
                    .All(row => row.Subtitle == "done"),
                "the production Journal records all five Red Gate beats as complete");
            Assert(!state.Map.Objects.Any(obj => obj != null && obj.Type == ObjectType.Stairs)
                && !InvokePrivate<bool>(game, "CanDescend")
                && state.Mode == GameMode.Explore,
                "the surveyed threshold cannot leak into the unfinished final chapter");

            int settledSupplies = state.Supplies;
            string settledProgression = PartyProgressionSignature(state);
            InvokePrivate(game, "RecallToTempleSquare");
            string completedTarget = InvokePrivate<string>(game, "CurrentExploreGuidanceTargetName") ?? "";
            Assert(state.Depth == 1
                && !state.StoryFlags.Contains(StoryFlags.RedGateDebriefed)
                && (completedTarget.IndexOf("Yara", StringComparison.OrdinalIgnoreCase) >= 0
                    || completedTarget.IndexOf("Old Road Scout", StringComparison.OrdinalIgnoreCase) >= 0
                    || completedTarget.IndexOf("Town Hall storm doors", StringComparison.OrdinalIgnoreCase) >= 0),
                "Chapter V recall routes the marshal's road seal to Yara");

            InvokePrivate(game, "ShowYaraConversation");
            InvokePrivate(game, "LateUpdate");
            Assert(state.StoryFlags.Contains(StoryFlags.RedGateDebriefed)
                && (state.ActiveStory ?? "").IndexOf("final descent remains sealed", StringComparison.OrdinalIgnoreCase) >= 0
                && (GetPrivateField<string>(game, "dialogueBody") ?? "").IndexOf("final road", StringComparison.OrdinalIgnoreCase) >= 0,
                "Yara's first post-threshold conversation closes Chapter V durably and states the honest final boundary");
            InvokePrivate(game, "CloseDialogue");
            InvokePrivate(game, "LateUpdate");

            oldRoad = state.Map.FindObjectById(oldRoadPassageId);
            state.PlayerX = oldRoad.X;
            state.PlayerY = oldRoad.Y;
            InvokePrivate(game, "Descend");
            boneRoad = state.Map.FindObjectById(boneRoadPassageId);
            state.PlayerX = boneRoad.X;
            state.PlayerY = boneRoad.Y;
            InvokePrivate(game, "Descend");
            glassRoad = state.Map.FindObjectById(glassRoadPassageId);
            state.PlayerX = glassRoad.X;
            state.PlayerY = glassRoad.Y;
            InvokePrivate(game, "Descend");
            farSeal = state.Map.FindObjectById(redGateObjectId);
            InvokePrivate(game, "ResolveExploreObject", farSeal);
            Assert(state.Depth == 5
                && state.Supplies == settledSupplies
                && PartyProgressionSignature(state) == settledProgression
                && !state.Map.Objects.Any(obj => obj != null && obj.Type == ObjectType.Stairs)
                && !InvokePrivate<bool>(game, "CanDescend"),
                "Chapter V re-entry duplicates no progression and still exposes no Chapter VI route");
        }

        private static void AssertAdvancedFullPrototypeRecallPreservesStory(AshenHallsGame game, GameState productionState)
        {
            string previousContentSet = GetPrivateField<string>(game, "activeContentSet");
            string previousStateContentSet = productionState.ContentSetId;
            GameState prototypeState = new GameState
            {
                SaveVersion = VersionInfo.SaveVersion,
                ContentSetId = ContentSetCatalog.FullPrototype,
                Mode = GameMode.Explore,
                Depth = 6,
                Seed = 21106,
                StoryChapter = 6,
                ActiveStory = "Chapter VI: Meteor Crown. Preserve the advanced prototype road.",
                Gold = productionState.Gold,
                Supplies = productionState.Supplies,
                Elixirs = productionState.Elixirs,
                Party = productionState.Party == null
                    ? new List<PartyMember>()
                    : productionState.Party.Select(member => member.CloneForPreview()).ToList(),
                StoryFlags = new List<string>
                {
                    StoryFlags.KoboldKingDefeated,
                    StoryFlags.RedGateWarningRecovered
                }
            };

            try
            {
                SetPrivateField(game, "state", prototypeState);
                InvokePrivate(game, "SetActiveContentSet", ContentSetCatalog.FullPrototype);
                prototypeState.Map = InvokePrivate<MapData>(game, "GenerateMap", prototypeState.Depth, prototypeState.Seed);
                prototypeState.PlayerX = prototypeState.Map.StartX;
                prototypeState.PlayerY = prototypeState.Map.StartY;
                InvokePrivate(game, "EnsureWorldState", VersionInfo.SaveVersion);
                Assert(prototypeState.StoryFlags.Contains(StoryFlags.GlassAndAshEntered)
                    && prototypeState.StoryChapter == 6
                    && (prototypeState.ActiveStory ?? "").IndexOf("Chapter VI", StringComparison.OrdinalIgnoreCase) >= 0,
                    "loading an advanced prototype save may repair visited depth four without rewriting its later chapter");

                InvokePrivate(game, "RecallToTempleSquare");
                ExplorationHudView recalledView = InvokePrivate<ExplorationHudView>(game, "BuildExplorationHudView");
                Assert(prototypeState.Depth == 1
                    && prototypeState.StoryChapter == 6
                    && (prototypeState.ActiveStory ?? "").IndexOf("Chapter VI", StringComparison.OrdinalIgnoreCase) >= 0
                    && (prototypeState.ActiveStory ?? "").IndexOf("Chapter IV", StringComparison.OrdinalIgnoreCase) < 0
                    && recalledView.ObjectiveSummary.IndexOf("Chapter IV", StringComparison.OrdinalIgnoreCase) < 0
                    && recalledView.ObjectiveSummary.IndexOf("Glass Road", StringComparison.OrdinalIgnoreCase) < 0,
                    "Recall preserves a pre-v2.11 depth-six prototype objective instead of rewinding it to Glass and Ash");
            }
            finally
            {
                SetPrivateField(game, "state", productionState);
                InvokePrivate(game, "SetActiveContentSet", previousContentSet);
                productionState.ContentSetId = previousStateContentSet;
                InvokePrivate(game, "InvalidateExplorationController");
                InvokePrivate(game, "MarkUiDirty");
            }
        }

        private static string PartyProgressionSignature(GameState state)
        {
            return state?.Party == null
                ? ""
                : string.Join("|", state.Party.Select(member =>
                    $"{member.Id}:{member.Level}:{member.Experience}:{member.SkillPoints}:{member.StatPoints}"));
        }

        private static void AssertLegacyRegionalSitePresentationGuards(AshenHallsGame game, GameState state)
        {
            MapData originalMap = state.Map;
            int originalDepth = state.Depth;
            int originalPlayerX = state.PlayerX;
            int originalPlayerY = state.PlayerY;
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            MethodInfo siteAtMethod = typeof(AshenHallsGame).GetMethod("TryRegionalSiteAt", flags);
            MethodInfo storySiteMethod = typeof(AshenHallsGame).GetMethod("TryStoryRegionalSite", flags);
            Assert(siteAtMethod != null && storySiteMethod != null,
                "legacy regional-site presentation and story probes remain available");

            try
            {
                Vector2Int[] legacySizes =
                {
                    new Vector2Int(WorldMapGenerationRules.LegacyWidth, WorldMapGenerationRules.LegacyHeight),
                    new Vector2Int(WorldMapGenerationRules.PreviousWidth, WorldMapGenerationRules.PreviousHeight)
                };
                foreach (Vector2Int size in legacySizes)
                {
                    int startX = WorldMapGenerationRules.StartX(size.x);
                    int startY = WorldMapGenerationRules.StartY(size.y);
                    MapData map = new MapData
                    {
                        Width = size.x,
                        Height = size.y,
                        Depth = 3,
                        StartX = startX,
                        StartY = startY,
                        Objects = new List<MapObject>()
                    };
                    WorldMapSite projectedSite = WorldMapGenerationRules.RegionalSites(
                            size.x,
                            size.y,
                            startX,
                            startY)
                        .Single(candidate => candidate.Id == WorldSitePresentationRules.GloamDeepCrypt);

                    Assert(!InvokePrivate<bool>(game, "UsesRegionalSiteLayout", map),
                        $"{size.x}x{size.y} saves do not advertise the v1.69 authored regional layout");
                    object[] siteAtArgs = { map, projectedSite.X, projectedSite.Y, default(WorldMapSite) };
                    bool presentedAsSite = (bool)siteAtMethod.Invoke(game, siteAtArgs);
                    Assert(!presentedAsSite,
                        $"{size.x}x{size.y} saves do not gain a phantom current-site label at projected regional coordinates");

                    if (size.x != WorldMapGenerationRules.PreviousWidth
                        || size.y != WorldMapGenerationRules.PreviousHeight)
                    {
                        continue;
                    }

                    state.Map = map;
                    state.Depth = map.Depth;
                    state.PlayerX = projectedSite.X;
                    state.PlayerY = projectedSite.Y;
                    object[] storySiteArgs = { projectedSite.Id, default(WorldMapSite) };
                    bool foundStorySite = (bool)storySiteMethod.Invoke(game, storySiteArgs);
                    WorldMapSite storySite = (WorldMapSite)storySiteArgs[1];
                    Assert(foundStorySite
                        && storySite.Id == projectedSite.Id
                        && storySite.X == projectedSite.X
                        && storySite.Y == projectedSite.Y,
                        "previous-size 50x32 saves retain their deterministic story anchor while presentation-only site labels stay suppressed");
                }
            }
            finally
            {
                state.Map = originalMap;
                state.Depth = originalDepth;
                state.PlayerX = originalPlayerX;
                state.PlayerY = originalPlayerY;
            }
        }

        private static void AssertLegacyKoboldCaveMigration(AshenHallsGame game, GameState state)
        {
            MapData originalMap = state.Map;
            List<RoamingThreat> originalThreats = state.RoamingThreats;
            int originalDepth = state.Depth;
            int originalPlayerX = state.PlayerX;
            int originalPlayerY = state.PlayerY;

            try
            {
                int width = WorldMapGenerationRules.PreviousWidth;
                int height = WorldMapGenerationRules.PreviousHeight;
                int startX = WorldMapGenerationRules.StartX(width);
                int startY = WorldMapGenerationRules.StartY(height);
                MapData legacyMap = new MapData
                {
                    Width = width,
                    Height = height,
                    Depth = 2,
                    StartX = startX,
                    StartY = startY,
                    Tiles = Enumerable.Repeat(1, width * height).ToList(),
                    SurfaceMaterials = Enumerable.Repeat((int)ExplorationMaterial.RuinedPaving, width * height).ToList(),
                    SurfaceRoles = Enumerable.Repeat((int)ExplorationCellRole.None, width * height).ToList(),
                    Objects = new List<MapObject>()
                };
                WorldMapSite staticDuskSite = WorldMapGenerationRules.RegionalSites(width, height, startX, startY)
                    .Single(site => site.ZoneId == "dusk-market");
                Point savedCaveCell = null;
                Point legacyAmbushCell = null;
                for (int y = 1; y < height - 1; y++)
                for (int x = 1; x < width - 1; x++)
                {
                    WorldZone zone = InvokePrivate<WorldZone>(game, "ZoneFor", x, y, legacyMap, 2);
                    if (zone == null || zone.Id != "dusk-market") continue;
                    int siteDistance = Math.Max(Math.Abs(x - staticDuskSite.X), Math.Abs(y - staticDuskSite.Y));
                    if (siteDistance <= staticDuskSite.Radius + 1) continue;
                    if (savedCaveCell == null)
                    {
                        savedCaveCell = new Point(x, y);
                    }
                    else if (Math.Abs(x - savedCaveCell.X) + Math.Abs(y - savedCaveCell.Y) > 1)
                    {
                        legacyAmbushCell = new Point(x, y);
                        break;
                    }
                }
                Assert(savedCaveCell != null && legacyAmbushCell != null,
                    "a previous-size Chapter II map exposes Dusk Market cells outside the unauthored static site footprint");

                MapObject migratedCave = new MapObject(
                    savedCaveCell.X,
                    savedCaveCell.Y,
                    ObjectType.Cave,
                    "dusk-market-smoke-cave");
                legacyMap.Objects.Add(migratedCave);
                legacyMap.InvalidateObjectLookup();
                state.Map = legacyMap;
                state.RoamingThreats = new List<RoamingThreat>();
                state.Depth = 2;
                state.PlayerX = startX;
                state.PlayerY = startY;
                InvokePrivate(game, "InvalidateExplorationController");
                InvokePrivate(game, "EnsureKoboldKingCaveMarker");
                MapObject repairedCave = legacyMap.FindObjectById("dusk-market-smoke-cave");
                Assert(repairedCave != null
                    && repairedCave.X == savedCaveCell.X
                    && repairedCave.Y == savedCaveCell.Y
                    && legacyMap.Objects.Count(obj => obj != null && obj.Id == "dusk-market-smoke-cave") == 1,
                    "previous-size saves preserve one reachable stable Dusk Market cave instead of snapping to unauthored site coordinates");
                InvokePrivate(game, "EnsureKoboldKingCaveMarker");
                Assert(legacyMap.FindObjectById("dusk-market-smoke-cave")?.X == savedCaveCell.X
                    && legacyMap.FindObjectById("dusk-market-smoke-cave")?.Y == savedCaveCell.Y,
                    "previous-size Smoke Cave repair is idempotent");
                Assert(InvokePrivate<bool>(game, "IsKoboldAmbushApproachCell", legacyAmbushCell.X, legacyAmbushCell.Y),
                    "previous-size saves trigger the kobold ambush from the real Dusk Market zone rather than an unauthored site radius");
            }
            finally
            {
                state.Map = originalMap;
                state.RoamingThreats = originalThreats;
                state.Depth = originalDepth;
                state.PlayerX = originalPlayerX;
                state.PlayerY = originalPlayerY;
                InvokePrivate(game, "InvalidateActiveRouteWaypointPath");
                InvokePrivate(game, "InvalidateExplorationController");
            }
        }

        private static void AssertGuidanceReroutesAroundActiveHabitat(AshenHallsGame game, GameState state)
        {
            MapData originalMap = state.Map;
            List<RoamingThreat> originalThreats = state.RoamingThreats;
            List<string> originalDiscoveries = state.DiscoveredZones;
            int originalDepth = state.Depth;
            int originalPlayerX = state.PlayerX;
            int originalPlayerY = state.PlayerY;
            string originalWaypointKey = state.ActiveRouteWaypointKey;

            try
            {
                const int width = WorldMapGenerationRules.LegacyWidth;
                const int height = WorldMapGenerationRules.LegacyHeight;
                int startX = WorldMapGenerationRules.StartX(width);
                int startY = WorldMapGenerationRules.StartY(height);
                WorldMapJunction waypoint = WorldMapGenerationRules.RegionalJunctions(width, height, startX, startY)
                    .Single(candidate => candidate.Id == "quarry-turn");
                int pathStartX = waypoint.X - 4;
                int habitatX = waypoint.X - 2;
                int pathY = waypoint.Y;
                int detourY = pathY + 1;
                MapData probeMap = new MapData
                {
                    Width = width,
                    Height = height,
                    Depth = 1,
                    StartX = startX,
                    StartY = startY,
                    Tiles = Enumerable.Repeat(0, width * height).ToList(),
                    SurfaceMaterials = Enumerable.Repeat((int)ExplorationMaterial.PackedDirt, width * height).ToList(),
                    SurfaceRoles = Enumerable.Repeat((int)ExplorationCellRole.None, width * height).ToList(),
                    Objects = new List<MapObject>()
                };
                for (int x = pathStartX; x <= waypoint.X; x++)
                {
                    int directIndex = pathY * width + x;
                    int detourIndex = detourY * width + x;
                    probeMap.Tiles[directIndex] = 1;
                    probeMap.Tiles[detourIndex] = 1;
                    probeMap.SurfaceRoles[directIndex] = (int)ExplorationCellRole.Road;
                    probeMap.SurfaceRoles[detourIndex] = (int)ExplorationCellRole.Road;
                }

                RoamingThreat habitat = new RoamingThreat
                {
                    Id = "guidance-habitat-probe",
                    Name = "Guidance Habitat Probe",
                    Archetype = "kobolds",
                    Depth = 1,
                    X = -1,
                    Y = -1,
                    HomeX = habitatX,
                    HomeY = pathY,
                    Active = true
                };
                state.Map = probeMap;
                state.RoamingThreats = new List<RoamingThreat> { habitat };
                state.Depth = 1;
                state.PlayerX = pathStartX;
                state.PlayerY = pathY;
                state.DiscoveredZones = new List<string> { RouteChartRules.DiscoveryKey(1, waypoint.Id) };
                state.ActiveRouteWaypointKey = RouteChartRules.WaypointKey(1, waypoint.Id);
                InvokePrivate(game, "InvalidateActiveRouteWaypointPath");

                int activeFingerprint = InvokePrivate<int>(game, "ExploreNavigationTopologyFingerprint");
                IReadOnlyList<Point> activePath = InvokePrivate<IReadOnlyList<Point>>(game, "CurrentExploreGuidancePath").ToArray();
                Assert(!InvokePrivate<bool>(game, "CanStepExplore", habitat.HomeX, habitat.HomeY)
                    && activePath.Count > 0
                    && activePath.All(step => step.X != habitat.HomeX || step.Y != habitat.HomeY),
                    "live Golden Thread guidance detours around an active roaming-habitat home");

                habitat.Active = false;
                int clearedFingerprint = InvokePrivate<int>(game, "ExploreNavigationTopologyFingerprint");
                IReadOnlyList<Point> clearedPath = InvokePrivate<IReadOnlyList<Point>>(game, "CurrentExploreGuidancePath").ToArray();
                string activeSignature = string.Join(">", activePath.Select(step => $"{step.X},{step.Y}"));
                string clearedSignature = string.Join(">", clearedPath.Select(step => $"{step.X},{step.Y}"));
                Assert(InvokePrivate<bool>(game, "CanStepExplore", habitat.HomeX, habitat.HomeY)
                    && clearedFingerprint != activeFingerprint
                    && clearedPath.Any(step => step.X == habitat.HomeX && step.Y == habitat.HomeY)
                    && clearedPath.Count < activePath.Count
                    && clearedSignature != activeSignature,
                    "clearing the habitat changes the navigation fingerprint and refreshes Golden Thread onto the shorter aftermath route");
            }
            finally
            {
                state.Map = originalMap;
                state.RoamingThreats = originalThreats;
                state.DiscoveredZones = originalDiscoveries;
                state.Depth = originalDepth;
                state.PlayerX = originalPlayerX;
                state.PlayerY = originalPlayerY;
                state.ActiveRouteWaypointKey = originalWaypointKey;
                InvokePrivate(game, "InvalidateActiveRouteWaypointPath");
                InvokePrivate(game, "InvalidateExplorationController");
            }
        }

        private static void AssertCombatBoardCursorControllerFlow(
            AshenHallsGame game,
            GameState combatState,
            CombatUnit active,
            CombatHudScreen hud,
            EventSystem eventSystem)
        {
            ActionMode originalAction = GetPrivateField<ActionMode>(game, "selectedAction");
            CombatPhase originalPhase = combatState.Combat.Phase;
            bool originalMoved = combatState.Combat.Moved;
            int originalMovePoints = combatState.Combat.MovePoints;
            int originalX = active.X;
            int originalY = active.Y;
            int originalWebbed = active.Webbed;
            int originalPointerSuppression = GetPrivateField<int>(game, "suppressBoardPointerThroughFrame");
            Vector2Int? originalVisualHover = GetPrivateField<Vector2Int?>(game, "visualSmokeCombatHoverCell");
            bool originalReducedMotion = combatState.ReducedMotion;
            GameObject originalSelection = eventSystem?.currentSelectedGameObject;
            try
            {
                InvokePrivate(game, "ClearCombatBoardCursor", false);
                SetPrivateField(game, "visualSmokeCombatHoverCell", (Vector2Int?)null);
                SetPrivateField(game, "suppressBoardPointerThroughFrame", Time.frameCount - 1);
                active.Webbed = 0;
                combatState.Combat.Moved = false;
                combatState.Combat.Phase = CombatPhase.ChooseAction;
                Assert(combatState.Combat.MovePoints > 0, "controller board-cursor smoke begins with movement available");

                hud.Refresh();
                hud.InvokePointerCommandForTest(ActionMode.Move);
                Assert(hud.PointerOwnsCommandContextForTest
                    && hud.HasFocusedCommand(ActionMode.Move)
                    && hud.OwnsSelection(eventSystem?.currentSelectedGameObject)
                    && GetPrivateField<ActionMode>(game, "selectedAction") == ActionMode.Move
                    && !GetPrivateField<bool>(game, "combatBoardCursorActive")
                    && !GetPrivateField<Vector2Int?>(game, "combatBoardCursorCell").HasValue,
                    "pointer enter, pointer-driven uGUI selection, and Button click keep pointer ownership without activating controller board focus");
                hud.ClearCommandHoverForTest();
                eventSystem?.SetSelectedGameObject(null);
                hud.ClearCommandFocusForTest();
                combatState.Combat.Phase = CombatPhase.ChooseAction;

                hud.FocusCommand(ActionMode.Move);
                Assert(hud.HasFocusedCommand(ActionMode.Move)
                    && hud.OwnsSelection(eventSystem?.currentSelectedGameObject),
                    "controller board-cursor smoke begins from a semantically focused Move command");
                InvokePrivate(game, "RunCombatHudCommand", ActionMode.Move);

                Vector2Int? entryCursor = GetPrivateField<Vector2Int?>(game, "combatBoardCursorCell");
                bool entryCursorActive = GetPrivateField<bool>(game, "combatBoardCursorActive");
                bool hudStillOwnsSelection = eventSystem != null && hud.OwnsSelection(eventSystem.currentSelectedGameObject);
                Assert(entryCursorActive
                    && entryCursor == new Vector2Int(active.X, active.Y)
                    && !hudStillOwnsSelection,
                    "controller activation leaves the HUD palette, exposes the board cursor, and keeps the active tile as its stable entry point; "
                    + $"active={entryCursorActive}, cursor={(entryCursor.HasValue ? entryCursor.Value.ToString() : "none")}, "
                    + $"actor=({active.X}, {active.Y}), focused={hud.FocusedCommandForTest?.ToString() ?? "none"}, "
                    + $"pointerOwns={hud.PointerOwnsCommandContextForTest}, selection={eventSystem?.currentSelectedGameObject?.name ?? "none"}");
                Assert(!InvokePrivate<bool>(game, "CombatBoardCursorCanConfirmFromInput"),
                    "the Submit press that activates controller board focus cannot also confirm it in the same frame");
                SetPrivateField(game, "combatBoardCursorActivatedFrame", Time.frameCount - 1);
                Assert(InvokePrivate<bool>(game, "CombatBoardCursorCanConfirmFromInput"),
                    "board confirmation becomes eligible on the frame after controller activation");

                InvokePrivate(game, "TrackCombatBoardPointerOwnership");
                Assert(GetPrivateField<bool>(game, "combatBoardCursorActive")
                    && GetPrivateField<Vector2Int?>(game, "combatBoardCursorCell") == entryCursor,
                    "a stationary mouse does not steal controller board focus");

                int takeoverX = active.X;
                int takeoverY = active.Y;
                int takeoverMovePoints = combatState.Combat.MovePoints;
                SetPrivateField(game, "combatBoardCursorPointerSample", Vector2.zero);
                SetPrivateField(game, "combatBoardCursorPointerSampled", true);
                Assert(InvokePrivate<bool>(game, "TrackCombatBoardPointerOwnership", new Vector2(3f, 0f))
                    && !GetPrivateField<bool>(game, "combatBoardCursorActive")
                    && GetPrivateField<bool>(game, "combatBoardNavigationSuppressedUntilNeutral")
                    && active.X == takeoverX
                    && active.Y == takeoverY
                    && combatState.Combat.MovePoints == takeoverMovePoints,
                    "deliberate pointer takeover clears board focus, consumes the current hotkey pass, and spends no movement");
                Assert(InvokePrivate<bool>(game, "ConsumeCombatBoardNavigationAfterPointerTakeover", 0.80f, 0f, false)
                    && GetPrivateField<bool>(game, "combatBoardNavigationSuppressedUntilNeutral")
                    && active.X == takeoverX
                    && active.Y == takeoverY
                    && combatState.Combat.MovePoints == takeoverMovePoints,
                    "a still-held stick remains consumed after pointer takeover instead of falling through to quick-step movement");
                Assert(!InvokePrivate<bool>(game, "ConsumeCombatBoardNavigationAfterPointerTakeover", 0f, 0f, false)
                    && !GetPrivateField<bool>(game, "combatBoardNavigationSuppressedUntilNeutral"),
                    "neutral stick input releases pointer-takeover navigation suppression");
                Assert(InvokePrivate<bool>(game, "ActivateCombatBoardCursor", active, false, true),
                    "controller board focus can be deliberately re-entered after pointer takeover and neutral release");

                List<Vector2Int> legalMoves = InvokePrivate<List<Vector2Int>>(game, "CombatBoardCursorCandidates", active);
                Assert(legalMoves.Count > 0
                    && legalMoves.All(cell => InvokePrivate<bool>(game, "CombatBoardCursorCellIsLegal", active, cell.x, cell.y)),
                    "Move target cycling publishes only currently reachable legal cells");
                Assert(InvokePrivate<bool>(game, "CycleCombatBoardCursor", active, 1),
                    "controller target cycling enters the first legal Move destination");
                Vector2Int cycledCursor = GetPrivateField<Vector2Int?>(game, "combatBoardCursorCell").Value;
                Assert(cycledCursor == legalMoves[0], "controller target cycling follows the deterministic rule ordering");

                CombatHudView cursorView = InvokePrivate<CombatHudView>(game, "BuildCombatHudView");
                Assert(cursorView.CommandPrompt.StartsWith("CURSOR ", StringComparison.Ordinal)
                    && cursorView.CommandPrompt.Contains("Submit confirms"),
                    "active board focus publishes visible cursor coordinates and an explicit confirm affordance");

                combatState.ReducedMotion = true;
                Assert(InvokePrivate<bool>(game, "CycleCombatBoardCursor", active, -1)
                    && GetPrivateField<bool>(game, "combatBoardCursorActive"),
                    "Reduced Motion retains deterministic board navigation without requiring animated feedback");
                Assert(InvokePrivate<bool>(game, "CycleCombatBoardCursor", active, 1),
                    "controller target cycling can return to the confirmable destination after wrapping");

                int movePointsBeforeConfirm = combatState.Combat.MovePoints;
                Assert(InvokePrivate<bool>(game, "ConfirmCombatBoardCursor", active),
                    "Submit resolves the board cursor through the production Move command path");
                Assert(active.X == cycledCursor.x
                    && active.Y == cycledCursor.y
                    && combatState.Combat.MovePoints < movePointsBeforeConfirm,
                    "controller board confirmation moves the active unit and spends the exact gameplay movement resource");
                Assert(InvokePrivate<bool>(game, "UndoActiveMovement")
                    && active.X == originalX
                    && active.Y == originalY
                    && combatState.Combat.MovePoints == originalMovePoints,
                    "controller-confirmed movement remains compatible with the existing movement undo path");
                Assert(typeof(GameState).GetField("combatBoardCursorCell", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) == null
                    && typeof(CombatState).GetField("combatBoardCursorCell", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) == null,
                    "board cursor focus remains transient presentation state and does not enter save data");
            }
            finally
            {
                InvokePrivate(game, "ClearCombatBoardCursor", false);
                SetPrivateField(game, "visualSmokeCombatHoverCell", originalVisualHover);
                SetPrivateField(game, "suppressBoardPointerThroughFrame", originalPointerSuppression);
                combatState.ReducedMotion = originalReducedMotion;
                combatState.Combat.Moved = originalMoved;
                combatState.Combat.MovePoints = originalMovePoints;
                combatState.Combat.Phase = originalPhase;
                active.X = originalX;
                active.Y = originalY;
                active.Webbed = originalWebbed;
                SetPrivateField(game, "selectedAction", originalAction);
                eventSystem?.SetSelectedGameObject(originalSelection);
                hud.Refresh();
            }
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
            Texture2D demonSummonIcons = GetPrivateField<Texture2D>(game, "demonSummonAtlas");
            Texture2D powerBookStateIcons = GetPrivateField<Texture2D>(game, "powerBookStateIconAtlas");
            Texture2D combatCommandIcons = GetPrivateField<Texture2D>(game, "combatCommandIconAtlas");
            Texture2D mageWarlockSpellVfx = GetPrivateField<Texture2D>(game, "mageWarlockSpellVfxAtlas");
            Texture2D supportHexSpellVfx = GetPrivateField<Texture2D>(game, "supportHexSpellVfxAtlas");
            Texture2D classSkillVfx = GetPrivateField<Texture2D>(game, "classSkillVfxAtlas");
            Texture2D combatPowerTravelVfx = GetPrivateField<Texture2D>(game, "combatPowerTravelVfxAtlas");
            Texture2D combatPowerAftermathVfx = GetPrivateField<Texture2D>(game, "combatPowerAftermathVfxAtlas");
            Assert(combatTerrain != null && InvokePrivate<bool>(game, "IsCombatTerrainAtlas"), "combat terrain atlas passes the production guard for authored hazards");
            Assert(koboldCombatTerrain != null && InvokePrivate<bool>(game, "IsKoboldCombatTerrainAtlas"), "semantic field atlas passes the production guard for gas, wards, and rituals");
            Assert(biomeProps != null && InvokePrivate<bool>(game, "IsWorldMapBiomePropAtlas"), "transparent biome prop atlas passes the production guard for combat cover");
            Assert(lightningSpellIcons != null && InvokePrivate<bool>(game, "IsLightningSpellIconAtlas"), "dedicated lightning spell atlas passes its production guard");
            Assert(signatureSpellIcons != null && InvokePrivate<bool>(game, "IsSignatureSpellIconAtlas"), "complete signature spell atlas passes its production guard");
            Assert(abilityIcons != null && InvokePrivate<bool>(game, "IsAbilityIconAtlas"), "complete ability atlas passes its production guard");
            Assert(demonSummonIcons != null && demonSummonIcons.width == 1254 && demonSummonIcons.height == 1254,
                "pinned demon summon and transformation atlas loads at runtime");
            Assert(signatureSpellIcons.width == CombatIconCatalog.SignatureSpellAtlasColumns * 256
                && signatureSpellIcons.height == CombatIconCatalog.SignatureSpellAtlasRows * 256,
                "runtime signature spell atlas exposes the expanded 7x8 grid");
            Assert(abilityIcons.width == CombatIconCatalog.AbilityAtlasWidth
                && abilityIcons.height == CombatIconCatalog.AbilityAtlasHeight,
                "runtime ability atlas exposes the expanded 4x7 grid");
            Assert(powerBookStateIcons != null
                && CombatIconCatalog.IsBookStateAtlasDimensions(powerBookStateIcons.width, powerBookStateIcons.height),
                "power-book state atlas preserves the exact 4x3 microicon contract");
            Assert(combatCommandIcons != null
                && CombatIconCatalog.IsCombatCommandAtlasDimensions(combatCommandIcons.width, combatCommandIcons.height),
                "combat command atlas preserves the exact 5x4 integer-cell contract");
            Assert(mageWarlockSpellVfx != null
                && mageWarlockSpellVfx.name == RuntimeArtManifest.MageWarlockSpellVfxAtlas
                && mageWarlockSpellVfx.width == 1254
                && mageWarlockSpellVfx.height == 1254
                && InvokePrivate<bool>(game, "IsMageWarlockSpellVfxAtlas"),
                "runtime loads the pinned square 4x4 mage and warlock spell VFX atlas through its production guard");
            Assert(supportHexSpellVfx != null
                && supportHexSpellVfx.name == RuntimeArtManifest.SupportHexSpellVfxAtlas
                && supportHexSpellVfx.width == 1254
                && supportHexSpellVfx.height == 1254
                && InvokePrivate<bool>(game, "IsSupportHexSpellVfxAtlas"),
                "runtime loads the pinned square 4x4 support and hex spell VFX atlas through its production guard");
            Assert(classSkillVfx != null
                && classSkillVfx.name == RuntimeArtManifest.ClassSkillVfxAtlas
                && classSkillVfx.width == 1254
                && classSkillVfx.height == 1254
                && InvokePrivate<bool>(game, "IsClassSkillVfxAtlas"),
                "runtime loads the pinned square 4x4 class-skill VFX atlas through its production guard");
            Assert(combatPowerTravelVfx != null
                && combatPowerTravelVfx.name == RuntimeArtManifest.CombatPowerTravelVfxAtlas
                && combatPowerTravelVfx.width == 1280
                && combatPowerTravelVfx.height == 1280
                && InvokePrivate<bool>(game, "IsCombatPowerTravelVfxAtlas"),
                "runtime loads the pinned 1280x1280 combat-power travel VFX atlas through its production guard");
            Assert(combatPowerAftermathVfx != null
                && combatPowerAftermathVfx.name == RuntimeArtManifest.CombatPowerAftermathVfxAtlas
                && combatPowerAftermathVfx.width == 1280
                && combatPowerAftermathVfx.height == 1280
                && InvokePrivate<bool>(game, "IsCombatPowerAftermathVfxAtlas"),
                "runtime loads the pinned 1280x1280 combat-power aftermath VFX atlas through its production guard");
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
            int livingEnemies = combatState.Combat.Units.Count(unit => unit != null && unit.Side == UnitSide.Enemy && unit.Hp > 0);
            int livingParty = combatState.Combat.Units.Count(unit => unit != null && unit.Side == UnitSide.Party && unit.Hp > 0);
            Assert(hudView.LivingEnemyCount == livingEnemies
                && hudView.LivingPartyCount == livingParty
                && hudView.ObjectiveLine.Contains($"Defeat {livingEnemies}")
                && !string.IsNullOrWhiteSpace(hudView.Title),
                "combat header identifies the encounter and publishes the live victory objective");
            Assert(!string.IsNullOrWhiteSpace(hudView.CommandPrompt), "production combat model exposes one canonical command prompt");
            Assert(hudView.PhaseLine.StartsWith("YOUR TURN", StringComparison.Ordinal), "player initiative is announced as the primary combat phase cue");
            Assert(hudView.ActiveUnit != null
                && hudView.ActiveUnit.PortraitTexture != null
                && hudView.ActiveUnit.PortraitSource.width > 1f
                && hudView.ActiveUnit.PortraitSource.height > 1f,
                "active combatant view resolves authored portrait geometry");
            Assert(hudView.ActiveUnit == null || hudView.ActiveUnit.StatusLine != "steady", "empty combat conditions use player-facing copy");
            Assert(hudView.ActiveUnit != null
                && hudView.ActiveUnit.StateLine.Contains("DMG")
                && hudView.ActiveUnit.StateLine.Contains("DEF")
                && hudView.ActiveUnit.StateLine.Contains("SPD"), "active combatant card uses its limited space for combat stats instead of repeating the top ribbon");
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
            int pickerCommandIndex = pickerCommand.Mode == ActionMode.Ability
                ? CombatIconCatalog.CombatCommandSkillsIndex
                : CombatIconCatalog.CombatCommandCastIndex;
            Assert(ReferenceEquals(pickerCommand.IconTexture, combatCommandIcons)
                && pickerCommand.IconSource == InvokePrivate<Rect>(game, "CombatCommandIconAtlasCell", pickerCommandIndex)
                && !pickerCommand.Armed,
                "an unarmed power command uses its stable category art instead of an arbitrary learned power");
            Assert(!string.IsNullOrWhiteSpace(hudView.TargetTitle) && hudView.TargetTitle != "Target", "combat side card names inspection or targeting context");
            Rect targetSourceBoardRect = GetPrivateField<Rect>(game, "boardRect");
            Vector2Int? targetSourceSmokeHover = GetPrivateField<Vector2Int?>(game, "visualSmokeCombatHoverCell");
            ActionMode targetSourceAction = GetPrivateField<ActionMode>(game, "selectedAction");
            SetPrivateField(game, "boardRect", Rect.zero);
            SetPrivateField<Vector2Int?>(game, "visualSmokeCombatHoverCell", null);
            SetPrivateField(game, "selectedAction", ActionMode.Move);
            CombatHudView idleTargetView = InvokePrivate<CombatHudView>(game, "BuildCombatHudView");
            Assert(idleTargetView.TargetUnit == null && idleTargetView.TargetSourceLabel == "NONE",
                "idle combat target rail stays neutral instead of presenting an irrelevant enemy warning");
            CombatUnit inspectionTarget = combatState.Combat.Units.First(unit => unit != null && unit.Side == UnitSide.Enemy && unit.Hp > 0);
            SetPrivateField<Vector2Int?>(
                game,
                "visualSmokeCombatHoverCell",
                new Vector2Int(inspectionTarget.X, inspectionTarget.Y));
            CombatHudView hoveredTargetView = InvokePrivate<CombatHudView>(game, "BuildCombatHudView");
            Assert(hoveredTargetView.TargetUnit != null
                && hoveredTargetView.TargetUnit.Name == inspectionTarget.Name
                && hoveredTargetView.TargetUnit.PortraitTexture != null
                && hoveredTargetView.TargetUnit.PortraitSource.width > 1f
                && hoveredTargetView.TargetUnit.PortraitSource.height > 1f
                && hoveredTargetView.TargetSourceLabel == "HOVER",
                "combat target rail resolves authored detail only from direct board inspection");
            SetPrivateField(game, "boardRect", targetSourceBoardRect);
            SetPrivateField(game, "visualSmokeCombatHoverCell", targetSourceSmokeHover);
            SetPrivateField(game, "selectedAction", targetSourceAction);
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

            string attackProbeClass = active.ClassKey;
            string attackProbeRole = active.Role;
            string attackProbeWeapon = active.WeaponName;
            int attackProbeRange = active.Range;
            int attackProbeDemonTurns = active.DemonFormTurns;
            int attackProbeX = active.X;
            int attackProbeY = active.Y;
            Dictionary<CombatUnit, Vector2Int> attackProbeEnemyPositions = combatState.Combat.Units
                .Where(unit => unit != null && unit.Side != active.Side)
                .ToDictionary(unit => unit, unit => new Vector2Int(unit.X, unit.Y));
            active.ClassKey = "ranger";
            active.Role = "bow";
            active.WeaponName = "ashwood longbow";
            active.Range = 5;
            active.DemonFormTurns = 0;
            active.X = 1;
            active.Y = 1;
            foreach (CombatUnit enemy in attackProbeEnemyPositions.Keys)
            {
                enemy.X = 10;
                enemy.Y = 6;
            }
            CombatHudCommandView shootCommand = InvokePrivate<CombatHudView>(game, "BuildCombatHudView")
                .Commands.First(command => command.Mode == ActionMode.Attack);
            Assert(
                shootCommand.Label == "Shoot"
                && ReferenceEquals(shootCommand.IconTexture, combatCommandIcons)
                && shootCommand.IconSource == InvokePrivate<Rect>(game, "CombatCommandIconAtlasCell", CombatHudCommandStyleRules.ShootCommandAtlasIndex),
                "production combat HUD pairs an unengaged ranger's Shoot command with the bow emblem");
            CombatUnit adjacentAttackProbe = attackProbeEnemyPositions.Keys.First();
            adjacentAttackProbe.X = 2;
            adjacentAttackProbe.Y = 1;
            CombatHudCommandView meleeCommand = InvokePrivate<CombatHudView>(game, "BuildCombatHudView")
                .Commands.First(command => command.Mode == ActionMode.Attack);
            Assert(
                meleeCommand.Label == "Melee"
                && ReferenceEquals(meleeCommand.IconTexture, combatCommandIcons)
                && meleeCommand.IconSource == InvokePrivate<Rect>(game, "CombatCommandIconAtlasCell", CombatIconCatalog.CombatCommandAttackIndex),
                "production combat HUD switches an engaged ranger to Melee and the sword emblem");
            active.ClassKey = attackProbeClass;
            active.Role = attackProbeRole;
            active.WeaponName = attackProbeWeapon;
            active.Range = attackProbeRange;
            active.DemonFormTurns = attackProbeDemonTurns;
            active.X = attackProbeX;
            active.Y = attackProbeY;
            foreach (KeyValuePair<CombatUnit, Vector2Int> pair in attackProbeEnemyPositions)
            {
                pair.Key.X = pair.Value.x;
                pair.Key.Y = pair.Value.y;
            }
            IReadOnlyList<ActionMode> fallbackModes = InvokePrivate<IReadOnlyList<ActionMode>>(game, "CombatHudFallbackModes", active);
            Assert(fallbackModes != null && fallbackModes.Count == 6, "production IMGUI action bar exposes six commands");
            Assert(fallbackModes[0] == ActionMode.Move && fallbackModes[1] == ActionMode.Attack && fallbackModes[5] == ActionMode.Wait, "action bar keeps Move/Attack/End Turn in stable positions");
            Assert(hudView.Turns != null && hudView.Turns.Count == 6, "combat HUD publishes the next six initiative turns");
            Assert(hudView.Turns[0].Active, "combat Timeline begins with the active unit");
            hud.Refresh();
            Assert(hud.RoundNumberForTest == hudView.RoundNumber
                && hud.MovePointsForTest == hudView.MovePoints
                && hud.MovePointsMaximumForTest == hudView.MovePointsMaximum
                && hud.ActionReadyForTest == hudView.ActionReady, "rendered combat header matches its authoritative model values");
            Assert(hud.RoundLabelForTest.StartsWith("ROUND", StringComparison.Ordinal)
                && hud.MoveLabelForTest.StartsWith("MOVE", StringComparison.Ordinal)
                && hud.ActionLabelForTest.StartsWith("ACTION", StringComparison.Ordinal), "rendered top stats contain only combat decision labels");
            Assert(hud.ActivePortraitVisibleForTest
                && hud.ActiveCardTitleForTest == "ACTIVE UNIT"
                && !string.IsNullOrWhiteSpace(hud.TargetCardTitleForTest)
                && hud.MenuVisibleForTest,
                "rendered combat rail keeps the active portrait, inspect context, and Menu entry visible");
            int expectedRenderedTurnChips = CombatHudScreenLayout.TurnChipCapacity(
                CombatHudScreenLayout.SideRailWidth(Screen.width));
            Assert(hud.VisibleTurnChipCountForTest == expectedRenderedTurnChips,
                "rendered initiative rail exposes its adaptive readable turn-chip capacity");
            Assert(hud.CommandCapacityForTest == hudView.Commands.Count, "combat command rendering capacity follows the model count");
            Assert(hudView.Commands.All(command => hud.CommandInputSelectableForTest(command.Mode)),
                "all combat commands remain focusable so unavailable reasons are keyboard/controller accessible");
            Assert(hudView.Commands.All(command => hud.CommandSelectedMultiplierForTest(command.Mode) == Color.white),
                "combat focus never multiplies or darkens the semantic command fill");
            CombatHudGeometry renderedCombatGeometry = CombatHudScreenLayout.Calculate(Screen.width, Screen.height);
            bool renderedEndTurnPromotion = hudView.Commands.Any(command => command.Mode == ActionMode.Wait && command.Promoted);
            Rect[] renderedCommandButtons = CombatHudScreenLayout.CommandButtons(
                renderedCombatGeometry.Command.width,
                renderedCombatGeometry.Command.height,
                hudView.Commands.Count,
                renderedEndTurnPromotion);
            float minimumRenderedCommandIcon = renderedCommandButtons.Length == 0
                ? 0f
                : renderedCommandButtons.Min(CombatHudScreenLayout.CommandIconSize);
            Assert(hudView.Commands.All(command => hud.CommandIconSizeForTest(command.Mode) + 0.01f >= minimumRenderedCommandIcon),
                "rendered vertical combat command emblems match the adaptive readable size for the active window");
            CombatHudCommandView initialAttackCommand = hudView.Commands.First(command => command.Mode == ActionMode.Attack);
            if (InvokePrivate<int>(game, "CountLegalAttackTargets", active) <= 0)
            {
                Assert(initialAttackCommand.Blocked
                    && initialAttackCommand.SubLabel.Contains("No target")
                    && hud.CommandUsesBlockedStyleForTest(ActionMode.Attack),
                    "a selected Attack with no legal target stays visibly blocked while remaining focusable");
            }
            Assert(hudView.TimelineExpanded || hudView.Logs.Count == 2 && hud.VisibleLogCount == 0,
                "collapsed combat Timeline keeps recent events in the model while hiding them behind the information drawer");
            EventSystem combatUiEventSystem = EventSystem.current
                ?? (!Application.isPlaying ? UiRuntime.EnsureEventSystemReady() : null);
            combatUiEventSystem?.SetSelectedGameObject(null);
            hud.ClearCommandFocusForTest();
            ActionMode commandModeBeforePreview = GetPrivateField<ActionMode>(game, "selectedAction");
            GameObject selectionBeforeCommandHover = combatUiEventSystem?.currentSelectedGameObject;
            hud.HoverCommandForTest(pickerCommand.Mode);
            Assert(hud.CommandPromptForTest.Contains(pickerCommand.Label)
                && hud.CommandPromptForTest.Contains(pickerCommand.Tooltip)
                && hud.HoveredCommandForTest == pickerCommand.Mode
                && hud.FocusedCommandForTest == null
                && hud.ContextCommandForTest == pickerCommand.Mode
                && hud.PointerOwnsCommandContextForTest
                && combatUiEventSystem?.currentSelectedGameObject == selectionBeforeCommandHover,
                "combat command pointer entry previews one action context without stealing semantic focus");
            Assert(GetPrivateField<ActionMode>(game, "selectedAction") == commandModeBeforePreview, "combat command hover never changes the armed gameplay mode");
            bool combatHudOwnsHoveredControl = hud.OwnsSelection(combatUiEventSystem?.currentSelectedGameObject);
            Assert(!combatHudOwnsHoveredControl
                && CombatInputRoutingRules.ShouldRouteToWorld(combatHudOwnsHoveredControl, CombatHotkeyKind.Navigation)
                && CombatInputRoutingRules.ShouldRouteToWorld(combatHudOwnsHoveredControl, CombatHotkeyKind.Submit),
                "pointer-only command preview leaves board navigation and Submit routed to the world");
            hud.ClearCommandHoverForTest();
            Assert(hud.HoveredCommandForTest == null
                && hud.FocusedCommandForTest == null
                && hud.ContextCommandForTest == null
                && !hud.PointerOwnsCommandContextForTest
                && hud.CommandPromptForTest == hudView.CommandPrompt,
                "leaving a pointer-only command preview restores the canonical combat prompt");
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
            GameObject focusedSelectionBeforeHover = combatUiEventSystem?.currentSelectedGameObject;
            hud.HoverCommandForTest(alternateCommand.Mode);
            Assert(
                hud.FocusedCommandForTest == pickerCommand.Mode
                && hud.HoveredCommandForTest == alternateCommand.Mode
                && hud.ContextCommandForTest == alternateCommand.Mode
                && hud.PointerOwnsCommandContextForTest
                && hud.CommandPromptForTest.Contains(alternateCommand.Tooltip)
                && combatUiEventSystem?.currentSelectedGameObject == focusedSelectionBeforeHover,
                "pointer preview temporarily owns prompt context without moving controller focus or EventSystem selection");
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
            AssertCombatBoardCursorControllerFlow(game, combatState, active, hud, combatUiEventSystem);
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

            InvokePrivate(game, "ExecuteBetaLabToolbarAction", BetaLabToolbarActionId.Mage, active);
            active = InvokePrivate<CombatUnit>(game, "CurrentUnit");
            moveOriginX = active.X;
            moveOriginY = active.Y;
            moveOriginPoints = combatState.Combat.MovePoints;
            undoCandidates = new[]
            {
                new Vector2Int(moveOriginX + 1, moveOriginY),
                new Vector2Int(moveOriginX, moveOriginY + 1),
                new Vector2Int(moveOriginX - 1, moveOriginY),
                new Vector2Int(moveOriginX, moveOriginY - 1)
            };
            undoDestination = undoCandidates
                .Where(candidate => InvokePrivate<bool>(game, "CanStandAt", candidate.x, candidate.y))
                .Select(candidate => (Vector2Int?)candidate)
                .FirstOrDefault();
            Assert(undoDestination.HasValue, "Beta Lab Mage tester has an adjacent undo destination");
            string mageTesterId = active.Id;
            string[] mageFormulaCodes = InvokePrivate<IEnumerable<FormulaDef>>(game, "KnownFormulasFor", active)
                .Select(formula => formula.Code)
                .ToArray();
            Assert(active.Side == UnitSide.Party
                && active.ClassKey == "mage"
                && active.Role == "ember"
                && active.Spell == "ember"
                && active.Level == ProgressionRules.MaximumLevel,
                "Beta Lab Mage preset activates a dedicated maximum-level ember tester");
            Assert(new[] { "FBL", "MTR", "VST", "AST" }.All(mageFormulaCodes.Contains),
                "Beta Lab Mage preset exposes Fireball, Meteor Shower, Thunder Step, and Arcane Tempest");
            Assert(combatState.Combat.ActiveId == active.Id
                && combatState.Combat.Phase == CombatPhase.ChooseAction
                && combatState.Combat.ActionAvailable
                && GetPrivateField<ActionMode>(game, "selectedAction") == ActionMode.Cast
                && !GetPrivateField<bool>(game, "combatAdvancePending")
                && GetPrivateField<float>(game, "aiActAt") < 0f,
                "Beta Lab Mage takeover begins a valid player casting turn with no queued enemy resolution");
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
            EventSystem modalEventSystem = EventSystem.current;
            GameObject modalSelectionBeforeHide = modalEventSystem?.currentSelectedGameObject;
            bool modalOwnedSelectionBeforeHide = modalSelectionBeforeHide != null
                && modalSelectionBeforeHide.transform.IsChildOf(GameObject.Find("Combat Ability Modal Canvas").transform);
            modal.SetVisible(false);
            Assert(!modalOwnedSelectionBeforeHide
                || modalEventSystem.currentSelectedGameObject == null,
                "hiding the spellbook clears its selected row from EventSystem ownership");
            modal.SetVisible(true);
            modal.Refresh();
            Assert(modal.IsVisible
                && (!Application.isPlaying || modal.SelectedRowFocusedForTest),
                "restoring the spellbook re-establishes one visible committed row after focus cleanup");
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
                && modal.SelectedRailCountForTest == 1
                && modal.VisibleTargetingRailCountForTest == 0,
                "spellbook uses authored state microicons with one selector and no redundant right targeting rail");
            Assert(modal.SelectedRailUsesSelectionAccentForTest
                && modal.DetailUsesSelectionChromeForTest,
                "ordinary Spellbook selection uses teal selection chrome rather than armed gold");
            Assert(string.IsNullOrEmpty(modal.DetailContextForTest), "ordinary selected Spellbook detail does not spend space restating selection");
            Assert(modal.DetailTargetLabelForTest.Contains("legal")
                && modal.DetailTargetLabelForTest.Contains("enem"), "Spellbook target chip names the legal target type");
            Assert(modal.FooterHintForTest.Contains("Browse")
                && modal.FooterHintForTest.Contains("Use")
                && modal.FooterHintForTest.Contains("Filter")
                && modal.FooterHintForTest.Contains("Back"), "Spellbook footer keeps a compact complete control legend");
            InvokePrivate(game, "SelectCombatAbilityModalCard", "WBF");
            InvokePrivate(game, "LateUpdate");
            Assert(modal.DetailIconScaleForTest >= 1.15f,
                "flat field spell art receives detail-only safe-area normalization");
            InvokePrivate(game, "SelectCombatAbilityModalCard", "FBL");
            InvokePrivate(game, "LateUpdate");
            Assert(Mathf.Approximately(modal.DetailIconScaleForTest, 1f),
                "non-field spell art keeps its authored detail scale");

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
            CombatAbilityModalCardView arcaneTempestCard = readySpellbookView.Cards[arcaneTempestIndex];
            CombatAbilityModalBookState expectedArcaneTempestState = CombatAbilityModalPresentationRules.ResolveBookState(arcaneTempestCard);
            float pointerPreviewQueuedAt = Time.unscaledTime;
            modal.QueuePointerPreviewForTest(arcaneTempestIndex);
            Assert(modal.PendingPreviewIdForTest == "AST"
                && string.IsNullOrEmpty(modal.PreviewedIdForTest)
                && !modal.CommitPointerPreviewForTest(pointerPreviewQueuedAt + 0.05f),
                "spellbook pointer preview waits through the protected 100 ms dwell");
            Assert(modal.CommitPointerPreviewForTest(pointerPreviewQueuedAt + 0.20f), "spell pointer preview commits after the protected dwell");
            Assert(string.IsNullOrEmpty(modal.PendingPreviewIdForTest), "committed spell preview clears its pending hover state");
            Assert(modal.PreviewedIdForTest == "AST" && modal.DetailIdForTest == "AST", "spell hover previews Arcane Tempest detail without changing selection");
            Assert(modal.SelectedId == "FBL" && modal.SelectedRailCountForTest == 1, "spell hover preserves one committed Fireball selection rail");
            Assert(modal.VisiblePreviewCueCountForTest == 1, "spell hover adds exactly one quiet passive preview cue");
            Assert(modal.SelectedRailUsesSelectionAccentForTest && modal.VisibleTargetingRailCountForTest == 0, "spell hover keeps teal selection chrome and no targeting rail");
            Assert(modal.DetailBookStateForTest == expectedArcaneTempestState, "hovered Arcane Tempest detail retains its authoritative live book state");
            Assert(!modal.DetailActionInteractableForTest && modal.DetailActionLabelForTest == "Preview Only", "hovered spell detail cannot activate until committed");
            Assert(modal.DetailPromptForTest.Contains("Click or focus the card"), "hovered spell detail explains how to commit the preview");
            Assert(GetPrivateField<string>(game, "spellbookSelectedCode") == "FBL"
                && GetPrivateField<string>(game, "pendingFormulaCode") == hoverPendingFormula
                && Mathf.Approximately(modal.ScrollYForTest, hoverScrollY), "spell hover neither persists browse memory, arms a formula, nor moves the list");
            modal.ClearHoverForTest();
            Assert(string.IsNullOrEmpty(modal.PreviewedIdForTest)
                && modal.VisiblePreviewCueCountForTest == 0
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
                && modal.VisibleTargetingRailCountForTest == 0
                && !modal.DetailStatusVisibleForTest
                && modal.DetailContextForTest == "TARGETING ARMED"
                && modal.DetailUsesArmedChromeForTest
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
                && modal.VisibleTargetingRailCountForTest == 0, "browsing another spell keeps one committed-selection rail while armed targeting stays in its badge and chrome");
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
            Assert(futureThunderStep.Locked && futureThunderStep.UnlockLevel == 16, "spellbook shows Thunder Step as a visible future unlock");
            Assert(futureTempest.Locked && futureTempest.UnlockLevel == 20 && futureTempest.Epic, "spellbook shows the elder Arcane Tempest progression card");
            Assert(CombatAbilityModalPresentationRules.RowMeta(futureTempest).StartsWith("Unlocks L20", StringComparison.Ordinal)
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
            Rect suggestedTargetBoardRect = GetPrivateField<Rect>(game, "boardRect");
            Vector2Int? suggestedTargetSmokeHover = GetPrivateField<Vector2Int?>(game, "visualSmokeCombatHoverCell");
            SetPrivateField(game, "boardRect", Rect.zero);
            SetPrivateField<Vector2Int?>(game, "visualSmokeCombatHoverCell", null);
            CombatHudView targetingHudView = InvokePrivate<CombatHudView>(game, "BuildCombatHudView");
            SetPrivateField(game, "boardRect", suggestedTargetBoardRect);
            SetPrivateField(game, "visualSmokeCombatHoverCell", suggestedTargetSmokeHover);
            Assert(targetingHudView.CanCancelTarget && targetingHudView.CancelTargetLabel == "Cancel Spell", "armed formula publishes explicit target cancellation");
            Vector2Int? armedSpellCursor = GetPrivateField<Vector2Int?>(game, "combatBoardCursorCell");
            Assert(targetingHudView.TargetSourceLabel == "CURSOR"
                && GetPrivateField<bool>(game, "combatBoardCursorActive")
                && armedSpellCursor.HasValue
                && InvokePrivate<bool>(game, "CombatBoardCursorCellIsLegal", active, armedSpellCursor.Value.x, armedSpellCursor.Value.y),
                "armed spell target rail labels its deterministic cursor-owned legal target");
            Assert(targetingHudView.Commands[2].Label == "Fireball"
                && targetingHudView.Commands[2].SubLabel.StartsWith("ARMED", StringComparison.Ordinal)
                && !targetingHudView.Commands[2].SubLabel.StartsWith("Choose", StringComparison.Ordinal), "armed spell command names the exact formula and publishes its current legal-target count");
            CombatHudCommandView armedSpellCommand = targetingHudView.Commands.First(command => command.Mode == ActionMode.Cast);
            Assert(armedSpellCommand.Selected
                && armedSpellCommand.Armed
                && ReferenceEquals(armedSpellCommand.IconTexture, signatureSpellIcons)
                && armedSpellCommand.IconSource == InvokePrivate<Rect>(game, "SignatureSpellIconAtlasCell", CombatIconCatalog.SignatureSpellIndex("FBL")),
                "armed spell command returns Fireball art to the deck with an explicit targeting state");
            Assert(hud.CommandStateTagForTest(ActionMode.Cast) == "ARMED", "rendered spell command binds the live ARMED state tag");
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
            List<PowerTravelVfx> stagedPowerTravel = GetPrivateField<List<PowerTravelVfx>>(game, "powerTravelVfx");
            stagedPowerTravel.Clear();
            List<PowerAftermathVfx> stagedPowerAftermath = GetPrivateField<List<PowerAftermathVfx>>(game, "powerAftermathVfx");
            stagedPowerAftermath.Clear();
            List<PowerImpactEcho> impactEchoes = GetPrivateField<List<PowerImpactEcho>>(game, "powerImpactEchoes");
            impactEchoes.Clear();
            List<PowerCastAura> castAuras = GetPrivateField<List<PowerCastAura>>(game, "powerCastAuras");
            castAuras.Clear();
            List<PowerActorPoseBeat> actorPoseBeats = GetPrivateField<List<PowerActorPoseBeat>>(game, "powerActorPoseBeats");
            actorPoseBeats.Clear();
            List<Tween> combatTweens = GetPrivateField<List<Tween>>(game, "tweens");
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
                "riftpounce", "riftpounceimpact", "abyssalwhirl", "abyssalwhirlimpact",
                "soulrend", "soulrendimpact", "dreadroar", "dreadroarimpact",
                "swing", "swingheavy", "thrust", "arrowrelease", "spell", "fire", "bladecontact", "thrustcontact", "heavycontact", "arrowcontact", "woodcontact", "stonecontact", "spellrelease", "wayfind",
                "combatstep", "combatguard", "combatturn", "combatcrit", "combatambsteel", "combatambsewer", "combatambarcane"
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
            Assert(
                InvokePrivate<bool>(game, "CombatPowerSfxProfileTargetsBeneficiary", "NVC")
                && InvokePrivate<bool>(game, "CombatPowerSfxProfileTargetsBeneficiary", "nvl")
                && InvokePrivate<bool>(game, "CombatPowerSfxProfileTargetsBeneficiary", "rally")
                && !InvokePrivate<bool>(game, "CombatPowerSfxProfileTargetsBeneficiary", "FBL"),
                "beneficial cleanse, veil, and rally audio suppress target hurt voices without muting hostile impacts");
            string originalCombatMusicBaseKey = GetPrivateField<string>(game, "combatMusicBaseKey");
            CombatState originalCombatAmbienceEncounter = GetPrivateField<CombatState>(game, "combatAmbienceEncounter");
            int originalCombatAmbienceSequence = GetPrivateField<int>(game, "combatAmbienceSequence");
            float originalLastCombatForegroundSfxAt = GetPrivateField<float>(game, "lastCombatForegroundSfxAt");
            float originalNextCombatAmbienceAt = GetPrivateField<float>(game, "nextCombatAmbienceAt");
            string originalLastSfxKey = GetPrivateField<string>(game, "lastSfxKey");
            bool originalSfxMuted = combatState.SfxMuted;
            bool originalMusicMuted = combatState.MusicMuted;
            bool originalShowPauseMenu = GetPrivateField<bool>(game, "showPauseMenu");
            string originalLaunchError = GetPrivateField<string>(game, "launchError");
            try
            {
                combatState.SfxMuted = false;
                combatState.MusicMuted = false;
                SetPrivateField(game, "showPauseMenu", false);
                SetPrivateField(game, "launchError", "");
                SetPrivateField(game, "combatMusicBaseKey", MusicDirectorRules.CombatSewer);
                SetPrivateField(game, "combatAmbienceEncounter", combatState.Combat);
                SetPrivateField(game, "combatAmbienceSequence", 0);
                SetPrivateField(game, "lastCombatForegroundSfxAt", Time.time - 10f);
                // Time.time is zero in editor smoke runs; zero is due now, while any negative value is the scheduler's uninitialized sentinel.
                SetPrivateField(game, "nextCombatAmbienceAt", Mathf.Max(0f, Time.time));
                SetPrivateField(game, "lastSfxKey", "");
                Assert(!InvokePrivate<bool>(game, "IsStartupSplashVisible"), "combat ambience fixture clears the startup presentation gate");
                Assert(InvokePrivate<UiOverlay>(game, "CurrentUiOverlay") != UiOverlay.Pause, "combat ambience fixture clears the pause gate");
                Assert(!combatState.SfxMuted
                    && (int)scheduledSfx.GetType().GetProperty("Count").GetValue(scheduledSfx) == 0
                    && Math.Abs(GetPrivateField<float>(game, "combatMusicDuckDepth")) < 0.0001f
                    && Time.time - GetPrivateField<float>(game, "lastCombatForegroundSfxAt") >= CombatAudioMixRules.CombatAmbienceForegroundQuietWindow,
                    "combat ambience fixture clears mute, queue, duck, and foreground quiet-window gates");
                InvokePrivate(game, "UpdateCombatAmbience");
                Assert(GetPrivateField<string>(game, "lastSfxKey") == CombatAudioMixRules.SewerAmbienceCue, "live sewer combat dispatches its sparse location-aware ambience");
                Assert(GetPrivateField<float>(game, "nextCombatAmbienceAt") > Time.time + 10f, "live combat ambience reschedules outside foreground attack tails");
            }
            finally
            {
                SetPrivateField(game, "combatMusicBaseKey", originalCombatMusicBaseKey);
                SetPrivateField(game, "combatAmbienceEncounter", originalCombatAmbienceEncounter);
                SetPrivateField(game, "combatAmbienceSequence", originalCombatAmbienceSequence);
                SetPrivateField(game, "lastCombatForegroundSfxAt", originalLastCombatForegroundSfxAt);
                SetPrivateField(game, "nextCombatAmbienceAt", originalNextCombatAmbienceAt);
                SetPrivateField(game, "lastSfxKey", originalLastSfxKey);
                combatState.SfxMuted = originalSfxMuted;
                combatState.MusicMuted = originalMusicMuted;
                SetPrivateField(game, "showPauseMenu", originalShowPauseMenu);
                SetPrivateField(game, "launchError", originalLaunchError);
            }
            InvokePrivate(
                game,
                "QueueSfx",
                "shock",
                0.20f,
                0.24f,
                0.05f,
                1f,
                CombatAudioMixRules.ScheduledSfxPrioritySupporting);
            InvokePrivate(
                game,
                "QueueSfx",
                "shock",
                0.20f,
                0.32f,
                0.08f,
                1.02f,
                CombatAudioMixRules.ScheduledSfxPrioritySupporting);
            Assert(
                (int)scheduledSfx.GetType().GetProperty("Count").GetValue(scheduledSfx) == 1,
                "runtime combat audio coalesces near-identical supporting cues");
            object coalescedCue = scheduledSfx.GetType().GetProperty("Item").GetValue(scheduledSfx, new object[] { 0 });
            Assert(
                (int)coalescedCue.GetType().GetField("Priority", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(coalescedCue)
                    == CombatAudioMixRules.ScheduledSfxPrioritySupporting,
                "runtime combat audio preserves the coalesced cue priority");
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
            stagedPowerTravel.Clear();
            stagedPowerAftermath.Clear();
            impactEchoes.Clear();
            castAuras.Clear();
            actorPoseBeats.Clear();
            combatTweens.RemoveAll(value => value.Id == active.Id && value.Kind == TweenKind.Move);
            stagedGlyphs.Clear();
            stagedFlashes.Clear();
            stagedParticles.Clear();
            scheduledSfx.GetType().GetMethod("Clear").Invoke(scheduledSfx, null);
            CombatPowerOutcomeSnapshot outcomeBefore = CombatPowerOutcomeRules.Capture(combatState.Combat);
            float castStarted = Time.time;
            Assert(InvokePrivate<bool>(game, "CastFormula", active, "FBL", spellTarget, spellTarget.X, spellTarget.Y), "Beta Lab Fireball resolves through production casting path");
            InvokePrivate(game, "SetCombatPowerOutcome", outcomeBefore);
            Assert(stagedFloats.Any(value => value.Start > castStarted + 0.04f), "Fireball damage feedback waits for impact timing");
            PowerTravelVfx[] stagedFireballTravel = stagedPowerTravel
                .Where(value => value.PowerKey == "FBL")
                .ToArray();
            Assert(stagedFireballTravel.Length == 1
                && stagedFireballTravel[0].SourceX == active.X
                && stagedFireballTravel[0].SourceY == active.Y
                && stagedFireballTravel[0].TargetX == spellTarget.X
                && stagedFireballTravel[0].TargetY == spellTarget.Y,
                "Fireball stages exactly one source-to-target authored travel effect");
            Assert(!stagedBeams.Any(value => value.Kind == "fireball"),
                "Fireball no longer duplicates its authored travel with the legacy beam renderer");
            PowerTravelVfx stagedFireball = stagedFireballTravel[0];
            CombatImpactProfile stagedFireballProfile = CombatImpactRules.ForFormula(fireball);
            PowerCastAura stagedFireballAura = castAuras.LastOrDefault(aura =>
                aura != null
                && string.Equals(aura.PowerKey, "FBL", StringComparison.OrdinalIgnoreCase)
                && aura.SourceX == active.X
                && aura.SourceY == active.Y
                && aura.TargetX == spellTarget.X
                && aura.TargetY == spellTarget.Y);
            Assert(stagedFireballAura != null, "focused Fireball stages one exact-key caster-to-target aura");
            AssertProductionPowerActorChoreography(
                "Fireball",
                active,
                "FBL",
                active.X,
                active.Y,
                spellTarget.X,
                spellTarget.Y,
                CombatPowerActorChoreographyKind.Cast,
                actorPoseBeats,
                castAuras,
                stagedPowerTravel,
                combatTweens);
            CombatPowerAnimationTimeline stagedFireballTravelTimeline = CombatPowerAnimationTimelineRules.ForFormula(
                fireball,
                stagedFireball.StableSeed,
                stagedFireball.Intensity,
                false);
            Assert(stagedFireballTravelTimeline.Supported
                && stagedFireballTravelTimeline.HasTravel
                && stagedFireball.StableSeed != 0
                && Math.Abs(stagedFireball.Start - (stagedFireballAura.Start + stagedFireballTravelTimeline.ReleaseAt)) < 0.0001f
                && Math.Abs(stagedFireball.Start + stagedFireball.Duration - (stagedFireballAura.Start + stagedFireballTravelTimeline.ImpactAt)) < 0.0001f,
                "Fireball authored travel begins on its exact release beat and lands on its exact impact beat");
            PowerAftermathVfx[] stagedFireballAftermath = stagedPowerAftermath
                .Where(value => string.Equals(value.PowerKey, "FBL", StringComparison.OrdinalIgnoreCase))
                .ToArray();
            Assert(stagedFireballAftermath.Length == 1, "Fireball stages exactly one authored aftermath effect");
            PowerAftermathVfx stagedFireballTail = stagedFireballAftermath[0];
            CombatPowerAftermathVfxProfile stagedFireballAftermathProfile = CombatPowerAftermathVfxRules.ProfileForFormula(fireball.Code);
            CombatPowerAnimationTimeline stagedFireballAftermathTimeline = CombatPowerAnimationTimelineRules.ForFormula(
                fireball,
                stagedFireballTail.StableSeed,
                stagedFireballTail.Intensity,
                false);
            Assert(stagedFireballAftermathProfile.HasAftermath
                && stagedFireballAftermathProfile.AtlasCell == (int)CombatPowerAftermathKind.Fireball
                && string.Equals(stagedFireballTail.PowerKey, stagedFireballAftermathProfile.Key, StringComparison.OrdinalIgnoreCase)
                && stagedFireballTail.X == spellTarget.X
                && stagedFireballTail.Y == spellTarget.Y
                && stagedFireballTail.Duration > 0f
                && stagedFireballTail.StableSeed != 0
                && stagedFireballAftermathTimeline.HasAftermath
                && Math.Abs(stagedFireballTail.Start - (castStarted + stagedFireballAftermathTimeline.AftermathAt)) < 0.04f,
                "Fireball stages the canonical aftermath atlas cell on its exact deterministic aftermath beat");
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
            Assert(scheduledSfxCount <= CombatAudioMixRules.ScheduledSfxCapacity, "Fireball audio remains inside the bounded combat queue");
            float stagedPan = (float)stagedImpactCue.GetType().GetField("Pan", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(stagedImpactCue);
            float stagedPitch = (float)stagedImpactCue.GetType().GetField("Pitch", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(stagedImpactCue);
            int stagedPriority = (int)stagedImpactCue.GetType().GetField("Priority", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(stagedImpactCue);
            Assert(stagedPan < -0.10f && stagedPan >= -0.85f, "Fireball impact audio follows its left-side battlefield target");
            Assert(stagedPitch >= 0.95f && stagedPitch <= 1.05f, "Fireball impact pitch variation remains restrained");
            Assert(stagedPriority == CombatAudioMixRules.ScheduledSfxPriorityPrimaryImpact, "Fireball primary impact is protected from lower-priority mix crowding");
            Assert(castAuras.Any(aura => aura.SourceX == active.X && aura.SourceY == active.Y && aura.TargetX == spellTarget.X && aura.TargetY == spellTarget.Y && aura.Focused), "focused Fireball stages a caster-to-target power aura");
            Assert(impactEchoes.Any(echo => echo.X == spellTarget.X && echo.Y == spellTarget.Y && echo.Intensity == 3 && echo.ReactionCount >= 1 && !echo.StaticStamp && echo.ImpactAt > echo.Start), "gas ignition promotes Fireball into an animated epic reaction echo");
            CombatUnitPresentationBeat fireballTargetBeat = unitPresentationBeats
                .LastOrDefault(beat => beat != null && beat.UnitId == spellTarget.Id);
            Assert(
                fireballTargetBeat != null
                && (fireballTargetBeat.Kind == CombatUnitPresentationBeatKind.Hit || fireballTargetBeat.Kind == CombatUnitPresentationBeatKind.Defeat)
                && Math.Abs(fireballTargetBeat.ImpactAt - (castStarted + stagedFireballProfile.ImpactDelay)) < 0.06f,
                "Fireball target sprite reaction lands on the canonical impact beat");
            List<ParticleDot> fireballParticles = stagedParticles;
            CombatPowerAftermathVfxPlan stagedFireballAftermathPlan = CombatPowerAftermathVfxRules.PlanForFormula(
                fireball.Code,
                stagedFireballTail.Intensity,
                0f,
                false,
                stagedFireballTail.StableSeed);
            int stagedFireballSmokeCount = (stagedFireballAftermathPlan.ParticleCount + 3) / 4;
            Assert(fireballParticles.Count == stagedFireballAftermathPlan.ParticleCount
                && fireballParticles.Count <= CombatPowerAftermathVfxRules.MaximumParticleCount,
                "Fireball keeps its authored aftermath particle tail inside the global spectacle bound");
            Assert(fireballParticles.Count(value => value.Kind == "smoke") == stagedFireballSmokeCount
                && fireballParticles.Count(value => value.Kind == "ember") == stagedFireballAftermathPlan.ParticleCount - stagedFireballSmokeCount,
                "Fireball leaves a deterministic authored smoke-and-ember tail");
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
            Assert(stagedBeams.Count == 0 && stagedPowerTravel.Count == 0 && stagedPowerAftermath.Count == 0 && impactEchoes.Count == 0 && castAuras.Count == 0 && actorPoseBeats.Count == 0 && fieldParticles.Count == 0 && fieldGlyphs.Count == 0, "enabling Reduced Motion clears queued combat travel, actor choreography, aftermath, and animated spectacle immediately");
            PowerActorPoseBeat reducedFireballActor = InvokePrivate<PowerActorPoseBeat>(
                game,
                "StageCombatPowerActorPose",
                active,
                "FBL",
                active.X,
                active.Y,
                spellTarget.X,
                spellTarget.Y,
                3,
                1776,
                CombatPowerActorPoseRole.Source,
                Time.time);
            Assert(reducedFireballActor == null && actorPoseBeats.Count == 0,
                "Reduced Motion stages no animated Fireball actor beat");
            Assert(fieldFlashes.Count == 1, "Reduced Motion preserves one compact target-local impact confirmation");
            Assert(Math.Abs(GetPrivateField<float>(game, "combatShakeMagnitude")) < 0.0001f, "Reduced Motion clears queued combat shake");
            Assert((int)scheduledSfx.GetType().GetProperty("Count").GetValue(scheduledSfx) == 0, "enabling Reduced Motion clears delayed layered combat cues immediately");
            Assert(Math.Abs(GetPrivateField<float>(game, "combatMusicDuckDepth")) < 0.0001f, "enabling Reduced Motion releases any staged impact music duck");
            int reducedFlashCount = fieldFlashes.Count;
            int expectedReducedReactionCount = GetPrivateField<List<string>>(game, "combatPowerReactions").Count;
            Color reducedStampColor = new Color(0.92f, 0.31f, 0.18f, 1f);
            InvokePrivate(
                game,
                "ApplyCombatImpactFeedback",
                stagedFireballProfile,
                spellTarget.X,
                spellTarget.Y,
                reducedStampColor,
                "fireball");
            Assert(impactEchoes.Count == 1, "a Reduced Motion impact stages one compact static echo");
            PowerImpactEcho reducedStamp = impactEchoes[0];
            Assert(reducedStamp.StaticStamp
                && reducedStamp.X == spellTarget.X
                && reducedStamp.Y == spellTarget.Y
                && reducedStamp.Color == reducedStampColor.ToHex()
                && reducedStamp.Kind == "fireball"
                && reducedStamp.ReactionCount == expectedReducedReactionCount
                && reducedStamp.Intensity == CombatImpactRules.VisualIntensity(stagedFireballProfile, reducedStamp.ReactionCount)
                && Math.Abs(reducedStamp.ImpactAt - reducedStamp.Start) < 0.0001f
                && Math.Abs(reducedStamp.Duration - 0.16f) < 0.0001f,
                "Reduced Motion impact echo is immediate, finite, typed, and atlas-ready");
            CombatImpactArtPlan reducedStampArt = CombatPowerVisualRules.ImpactArtPlan(
                reducedStamp.Kind,
                reducedStamp.Intensity,
                0f);
            Assert(reducedStampArt.HasPrimary, "Reduced Motion static impact resolves one authored primary atlas stamp");
            Assert(stagedBeams.Count == 0
                && stagedPowerTravel.Count == 0
                && stagedPowerAftermath.Count == 0
                && castAuras.Count == 0
                && fieldParticles.Count == 0
                && fieldGlyphs.Count == 0
                && fieldFlashes.Count == reducedFlashCount
                && Math.Abs(GetPrivateField<float>(game, "combatShakeMagnitude")) < 0.0001f
                && GetPrivateField<int>(game, "combatImpactFrameIntensity") == 0
                && Math.Abs(GetPrivateField<float>(game, "pendingCombatPowerOutcomeDelay")) < 0.0001f,
                "Reduced Motion static impact adds no travel, aura, particle, glyph, flash, shake, frame, or delayed outcome stack");
            impactEchoes.Clear();
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
            active.Level = ProgressionRules.MaximumLevel;
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
            Assert(stagedPowerTravel.Any(value => value.PowerKey == "RIG")
                && !stagedBeams.Any(value => value.Kind == "lightning"), "Arc Spark uses its authored lightning travel without a duplicate legacy beam");

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
            Assert(stagedPowerTravel.Any(value => value.PowerKey == "CLT")
                && stagedBeams.Count(value => value.Kind == "lightning") >= 3, "Chain Lightning uses authored primary travel and retains every visible jump between linked targets");
            Assert(stagedFloats.Any(value => value.Text == "ARC")
                && stagedFloats.Any(value => value.Text == "JUMP 4"), "Chain Lightning labels its primary and final jump");

            combatState.Combat.Units.RemoveAll(unit => unit == null || unit.Id != active.Id && unit.Id != spellTarget.Id);
            combatState.Combat.Obstacles.Clear();
            stagedFloats.Clear();
            stagedBeams.Clear();
            stagedPowerTravel.Clear();
            castAuras.Clear();
            actorPoseBeats.Clear();
            combatTweens.RemoveAll(value => value.Id == active.Id && value.Kind == TweenKind.Move);
            active.X = 1;
            active.Y = 1;
            spellTarget.X = 6;
            spellTarget.Y = 3;
            spellTarget.Hp = spellTarget.MaxHp;
            int thunderStepTargetHp = spellTarget.Hp;
            int thunderStepSourceX = active.X;
            int thunderStepSourceY = active.Y;
            active.Mana = active.MaxMana;
            Assert(InvokePrivate<bool>(game, "CastFormula", active, "VST", null, 5, 3), "Thunder Step resolves through the production formula path");
            Assert(active.X == 5 && active.Y == 3, "Thunder Step moves the caster to the chosen open tile");
            Assert(combatState.Combat.MovePoints == 0 && combatState.Combat.Moved, "Thunder Step consumes ordinary movement after its relocation");
            Assert(spellTarget.Hp < thunderStepTargetHp, "Thunder Step shocks enemies beside its destination");
            Assert(stagedPowerTravel.Any(value => value.PowerKey == "VST")
                && !stagedBeams.Any(value => value.Kind == "arc")
                && stagedBeams.Any(value => value.Kind == "lightning"), "Thunder Step uses authored travel and retains its post-arrival lightning accents");
            Assert(stagedFloats.Any(value => value.Text == "THUNDER STEP"), "Thunder Step stages its named landing result");
            Assert(castAuras.Any(aura => aura.SourceX == 1 && aura.SourceY == 1 && aura.TargetX == 5 && aura.TargetY == 3), "Thunder Step stages a destination-aware cast aura");
            AssertProductionPowerActorChoreography(
                "Thunder Step",
                active,
                "VST",
                thunderStepSourceX,
                thunderStepSourceY,
                active.X,
                active.Y,
                CombatPowerActorChoreographyKind.Teleport,
                actorPoseBeats,
                castAuras,
                stagedPowerTravel,
                combatTweens);

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
            Assert(stagedPowerTravel.Any(value => value.PowerKey == "AST")
                && stagedBeams.Count(value => value.Kind == "lightning") >= 2, "Arcane Tempest uses authored center travel and retains visible satellite lightning across its footprint");
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

            InvokePrivate(game, "ExecuteBetaLabToolbarAction", BetaLabToolbarActionId.Warlock, active);
            active = InvokePrivate<CombatUnit>(game, "CurrentUnit");
            combatState.Combat.Obstacles.Clear();
            active.X = 1;
            active.Y = 1;
            spellTarget.X = 4;
            spellTarget.Y = 1;
            spellTarget.Hp = spellTarget.MaxHp;
            active.Mana = active.MaxMana;
            stagedBeams.Clear();
            int riftBoltHp = spellTarget.Hp;
            Assert(InvokePrivate<bool>(game, "CastFormula", active, "RBT", spellTarget, spellTarget.X, spellTarget.Y), "Rift Bolt resolves through the production pact formula path");
            Assert(spellTarget.Hp < riftBoltHp
                && stagedPowerTravel.Any(value => value.PowerKey == "RBT")
                && !stagedBeams.Any(value => value.Kind == "death"), "Rift Bolt deals direct death damage through one authored rift delivery");

            active.Mana = active.MaxMana;
            stagedBeams.Clear();
            stagedPowerTravel.Clear();
            castAuras.Clear();
            actorPoseBeats.Clear();
            combatTweens.RemoveAll(value => value.Id == active.Id && value.Kind == TweenKind.Move);
            int riftStepSourceX = active.X;
            int riftStepSourceY = active.Y;
            Assert(InvokePrivate<bool>(game, "CastFormula", active, "VRS", null, 2, 3), "Rift Step resolves through the production pact formula path");
            Assert(active.X == 2 && active.Y == 3, "Rift Step moves the warlock to the chosen open tile");
            Assert(combatState.Combat.MovePoints == 0 && combatState.Combat.Moved, "Rift Step consumes ordinary movement after its relocation");
            Assert(stagedPowerTravel.Any(value => value.PowerKey == "VRS")
                && !stagedBeams.Any(value => value.Kind == "arc"), "Rift Step stages one authored rift travel without a duplicate legacy arc");
            AssertProductionPowerActorChoreography(
                "Rift Step",
                active,
                "VRS",
                riftStepSourceX,
                riftStepSourceY,
                active.X,
                active.Y,
                CombatPowerActorChoreographyKind.Teleport,
                actorPoseBeats,
                castAuras,
                stagedPowerTravel,
                combatTweens);

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
            bool ascendanceWasFocused = InvokePrivate<bool>(game, "IsFocusedCaster", active);
            int ascendanceManaBefore = active.Mana;
            modal.InvokeSelectedForTest();
            Assert(!modal.IsVisible
                && active.Mana < ascendanceManaBefore
                && !combatState.Combat.ActionAvailable, "Use Now resolves a self-only formula exactly once through the real Spellbook action");
            FormulaDef ascendanceFormula = FormulaCatalog.All.Single(formula => formula.Code == "DFA");
            int advertisedDemonTurns = ascendanceFormula.Duration + (ascendanceWasFocused ? 1 : 0);
            Assert(active.DemonFormTurns == advertisedDemonTurns + 1 && active.Hp > woundedHp, "Abyssal Ascendance stores enough duration for every advertised future transformed action and heals immediately");
            Assert(InvokePrivate<int>(game, "DemonFormAttackBonus", active) == 4, "demon form grants its flat claw power bonus");
            Assert(InvokePrivate<int>(game, "DemonFormDamageReduction", active) == 2, "demon form grants its damage reduction");
            Assert(InvokePrivate<int>(game, "DemonSummonSpriteIndex", active) == 8, "healthy demon form swaps to the greater-demon combat sprite");
            Assert(CreatureAudioRules.FactionFor(active) == "demon"
                && CreatureAudioRules.CueFor(active, "attack") == "demonattack", "transformed warlock routes the demon voice set");

            combatState.Combat.Units.RemoveAll(unit => unit == null || unit.Id != active.Id && unit.Id != spellTarget.Id);
            combatState.Combat.Obstacles.Clear();
            active.X = 1;
            active.Y = 3;
            spellTarget.X = 6;
            spellTarget.Y = 3;
            spellTarget.Hp = spellTarget.MaxHp;
            spellTarget.Defense = 0;
            spellTarget.ArmorBonus = 0;
            spellTarget.Agility = 0;
            spellTarget.Guarding = false;
            spellTarget.GuardBonus = 0;
            spellTarget.Shielded = 0;
            spellTarget.Hexed = 0;
            spellTarget.Resist = "";
            spellTarget.Weakness = "";
            int storedDemonTurns = active.DemonFormTurns;
            active.Skills.Arms = active.Skills.Hex;
            active.DemonFormTurns = 0;
            Vector2Int mortalDeathAttack = InvokePrivate<Vector2Int>(game, "AttackDamagePreview", active, spellTarget);
            active.DemonFormTurns = storedDemonTurns;
            Vector2Int demonDeathAttack = InvokePrivate<Vector2Int>(game, "AttackDamagePreview", active, spellTarget);
            Assert(demonDeathAttack.x == mortalDeathAttack.x + 4 && demonDeathAttack.y == mortalDeathAttack.y + 4,
                "demon flat power reaches the warlock's nonphysical death attack without borrowing warrior enrage");
            Assert(InvokePrivate<string>(game, "ActiveWeaponLabel", active) == "abyssal claws"
                && InvokePrivate<int>(game, "BaseAttackRange", active) == 1
                && InvokePrivate<string>(game, "AttackSkillName", active, spellTarget) == "hex",
                "demon form presents a melee claw attack driven by Hex skill");

            InvokePrivate(game, "CancelCombatResolutionBeat", true);
            combatState.Combat.ActiveId = active.Id;
            combatState.Combat.ActionAvailable = true;
            combatState.Combat.Acted = false;
            combatState.Combat.Phase = CombatPhase.ChooseAction;

            CombatHudView demonHudView = InvokePrivate<CombatHudView>(game, "BuildCombatHudView");
            CombatHudCommandView demonArtsCommand = demonHudView.Commands[2];
            Assert(demonArtsCommand.Mode == ActionMode.Ability
                && demonArtsCommand.Label == "Demon Arts"
                && ReferenceEquals(demonArtsCommand.IconTexture, combatCommandIcons)
                && demonArtsCommand.IconSource == InvokePrivate<Rect>(game, "CombatCommandIconAtlasCell", CombatIconCatalog.CombatCommandSkillsIndex),
                "transformation replaces the bottom Spellbook command with a stable Demon Arts command");

            InvokePrivate(game, "SelectOrRunAction", ActionMode.Ability, active);
            InvokePrivate(game, "LateUpdate");
            CombatAbilityModalView demonArtsView = InvokePrivate<CombatAbilityModalView>(game, "BuildCombatAbilityModalView");
            string[] demonArtIds = { "riftpounce", "abyssalwhirl", "soulrend", "dreadroar" };
            Assert(modal.IsVisible
                && !demonArtsView.Spellbook
                && demonArtsView.Title == "Demon Arts"
                && demonArtsView.Resource.Contains("FORM")
                && demonArtsView.Trait.Contains("ABYSSAL FORM")
                && demonArtsView.Cards.Select(card => card.Id).SequenceEqual(demonArtIds),
                "transformed warlock opens a four-card Demon Arts book with live form context");
            foreach (string demonArtId in demonArtIds)
            {
                CombatAbilityModalCardView card = demonArtsView.Cards.Single(value => value.Id == demonArtId);
                int iconIndex = CombatIconCatalog.AbilityIndex(demonArtId);
                Assert(!card.Locked
                    && card.IconTexture == abilityIcons
                    && card.IconSource == InvokePrivate<Rect>(game, "AbilityIconAtlasCell", iconIndex),
                    demonArtId + " resolves its appended Demon Arts icon cell");
            }
            Assert(demonArtsView.Cards.Single(card => card.Id == "riftpounce").TargetCountKnown
                && demonArtsView.Cards.Single(card => card.Id == "riftpounce").ValidTargetCount > 0,
                "Demon Arts book computes a legal rift landing before arming Rift Pounce");
            InvokePrivate(game, "CloseCombatAbilityModal");

            stagedFloats.Clear();
            stagedBeams.Clear();
            stagedPowerTravel.Clear();
            impactEchoes.Clear();
            castAuras.Clear();
            actorPoseBeats.Clear();
            combatTweens.RemoveAll(value => value.Id == active.Id && value.Kind == TweenKind.Move);
            combatState.Combat.Obstacles.Add(new Point(2, 3, "stone"));
            combatState.Combat.Obstacles.Add(new Point(3, 3, "stone"));
            combatState.Combat.Obstacles.Add(new Point(4, 3, "stone"));
            int pounceTargetHp = spellTarget.Hp;
            int pounceSourceX = active.X;
            int pounceSourceY = active.Y;
            Point expectedPounceLanding = InvokePrivate<Point>(game, "BestRiftPounceLanding", active, spellTarget);
            Assert(InvokePrivate<bool>(game, "UseTargetedAbility", active, "riftpounce", spellTarget, spellTarget.X, spellTarget.Y), "Rift Pounce resolves through the shared targeted ability path");
            Assert(spellTarget.Hp < pounceTargetHp
                && expectedPounceLanding != null
                && active.X == expectedPounceLanding.X
                && active.Y == expectedPounceLanding.Y
                && Math.Abs(active.X - spellTarget.X) + Math.Abs(active.Y - spellTarget.Y) == 1
                && !stagedBeams.Any(value => value.Kind == "arc"),
                "Rift Pounce crosses blocked intervening tiles, lands beside its target, and deals death damage");
            AssertProductionPowerActorChoreography(
                "Rift Pounce",
                active,
                "riftpounce",
                pounceSourceX,
                pounceSourceY,
                active.X,
                active.Y,
                CombatPowerActorChoreographyKind.Teleport,
                actorPoseBeats,
                castAuras,
                stagedPowerTravel,
                combatTweens);

            combatState.Combat.Obstacles.Clear();
            active.X = 5;
            active.Y = 3;
            spellTarget.X = 6;
            spellTarget.Y = 3;
            spellTarget.Hp = spellTarget.MaxHp;
            CombatUnit demonSecond = MakeRuntimeEnemy("demon-art-second", 5, 4);
            combatState.Combat.Units.Add(demonSecond);
            int whirlPrimaryHp = spellTarget.Hp;
            int whirlSecondHp = demonSecond.Hp;
            Assert(InvokePrivate<bool>(game, "UseInstantAbility", active, "abyssalwhirl"), "Abyssal Whirl resolves through the shared instant ability path");
            Assert(spellTarget.Hp < whirlPrimaryHp && demonSecond.Hp < whirlSecondHp, "Abyssal Whirl deals death damage to every adjacent enemy");

            spellTarget.Hp = spellTarget.MaxHp;
            active.Hp = Math.Max(1, active.MaxHp - 24);
            int soulRendCasterHp = active.Hp;
            int soulRendTargetHp = spellTarget.Hp;
            SetPrivateField(game, "rng", new System.Random(1));
            Assert(InvokePrivate<bool>(game, "UseTargetedAbility", active, "soulrend", spellTarget, spellTarget.X, spellTarget.Y), "Soul Rend resolves through the shared targeted ability path");
            Assert(spellTarget.Hp < soulRendTargetHp && active.Hp > soulRendCasterHp, "Soul Rend deals death damage and heals from actual damage dealt");

            spellTarget.Hp = spellTarget.MaxHp;
            demonSecond.Hp = demonSecond.MaxHp;
            spellTarget.Guarding = true;
            spellTarget.GuardBonus = 4;
            spellTarget.Hexed = 0;
            spellTarget.MagicResist = -10;
            demonSecond.Guarding = true;
            demonSecond.GuardBonus = 3;
            demonSecond.Hexed = 0;
            demonSecond.MagicResist = -10;
            int roarPrimaryHp = spellTarget.Hp;
            int roarSecondHp = demonSecond.Hp;
            Assert(InvokePrivate<bool>(game, "UseInstantAbility", active, "dreadroar"), "Dread Roar resolves through the shared instant ability path");
            Assert(!spellTarget.Guarding && spellTarget.GuardBonus == 0 && spellTarget.Hexed >= 3
                && !demonSecond.Guarding && demonSecond.GuardBonus == 0 && demonSecond.Hexed >= 3,
                "Dread Roar strips adjacent guards and applies its mind-resisted hex");
            Assert(spellTarget.Hp == roarPrimaryHp && demonSecond.Hp == roarSecondHp, "Dread Roar remains a control art and deals no hidden damage");

            active.Poisoned = 0;
            active.Bleeding = 0;
            active.Stunned = 0;
            active.Sleeping = 0;
            active.Webbed = 0;
            active.Hexed = 0;
            active.Regenerating = 0;
            active.Shielded = 0;
            combatState.Combat.Obstacles.Clear();
            SetPrivateField(game, "pendingAbilityId", "soulrend");
            for (int futureTurn = 1; futureTurn <= advertisedDemonTurns; futureTurn++)
            {
                InvokePrivate<bool>(game, "ApplyStartTurnEffects", active);
                Assert(active.DemonFormTurns > 0, "Abyssal Ascendance remains active for future transformed turn " + futureTurn);
            }
            InvokePrivate<bool>(game, "ApplyStartTurnEffects", active);
            Assert(active.DemonFormTurns == 0 && string.IsNullOrEmpty(GetPrivateField<string>(game, "pendingAbilityId")),
                "Abyssal Ascendance expires immediately after its advertised future actions and clears stale Demon Arts targeting");
            CombatHudView restoredWarlockHud = InvokePrivate<CombatHudView>(game, "BuildCombatHudView");
            Assert(restoredWarlockHud.Commands[2].Mode == ActionMode.Cast && restoredWarlockHud.Commands[2].Label == "Spells",
                "expiring demon form restores the bottom Spellbook command");
            combatState.Combat.ActionAvailable = true;
            combatState.Combat.Acted = false;
            combatState.Combat.Phase = CombatPhase.ChooseAction;
            InvokePrivate(game, "SelectOrRunAction", ActionMode.Cast, active);
            InvokePrivate(game, "LateUpdate");
            CombatAbilityModalView restoredSpellbook = InvokePrivate<CombatAbilityModalView>(game, "BuildCombatAbilityModalView");
            Assert(modal.IsVisible
                && restoredSpellbook.Spellbook
                && restoredSpellbook.Title.EndsWith("Spellbook", StringComparison.Ordinal)
                && restoredSpellbook.Cards.Any(card => card.Id == "RBT")
                && restoredSpellbook.Cards.Any(card => card.Id == "VRS")
                && !restoredSpellbook.Cards.Any(card => demonArtIds.Contains(card.Id)),
                "expiring demon form restores the pact Spellbook without leaking Demon Arts cards");
            InvokePrivate(game, "CloseCombatAbilityModal");
            combatState.Combat.Units.Remove(demonSecond);

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
            foreach (string classKey in new[] { "warrior", "rogue", "ranger", "demon" })
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
                && modal.SelectedRailCountForTest == 1
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
                && modal.DetailTargetLabelForTest.Contains("enem")
                && modal.SelectedRailUsesSelectionAccentForTest
                && modal.DetailUsesSelectionChromeForTest,
                "ordinary Skillbook selection uses teal chrome and names Aimed Shot's legal enemies");
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
                && modal.VisiblePreviewCueCountForTest == 1
                && modal.VisibleTargetingRailCountForTest == 0
                && modal.SelectedRailUsesSelectionAccentForTest
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
                && modal.VisiblePreviewCueCountForTest == 0
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
            CombatHudCommandView armedSkillCommand = InvokePrivate<CombatHudView>(game, "BuildCombatHudView")
                .Commands.First(command => command.Mode == ActionMode.Ability);
            Assert(armedSkillCommand.Selected
                && armedSkillCommand.Armed
                && armedSkillCommand.Label == "Aimed Shot"
                && armedSkillCommand.SubLabel.StartsWith("ARMED", StringComparison.Ordinal)
                && ReferenceEquals(armedSkillCommand.IconTexture, abilityIcons)
                && armedSkillCommand.IconSource == InvokePrivate<Rect>(game, "AbilityIconAtlasCell", CombatIconCatalog.AbilityIndex("aimedshot")),
                "armed skill command returns Aimed Shot art to the deck with the same targeting contract as spells");
            Assert(InvokePrivate<bool>(game, "CancelCombatTargeting"), "skill targeting can be canceled without spending the action");
            Assert(string.IsNullOrEmpty(GetPrivateField<string>(game, "pendingAbilityId"))
                && combatState.Combat.ActionAvailable, "canceling a skill returns to an action-ready state");
            CombatHudCommandView resetSkillCommand = InvokePrivate<CombatHudView>(game, "BuildCombatHudView")
                .Commands.First(command => command.Mode == ActionMode.Ability);
            Assert(!resetSkillCommand.Armed
                && ReferenceEquals(resetSkillCommand.IconTexture, combatCommandIcons)
                && resetSkillCommand.IconSource == InvokePrivate<Rect>(game, "CombatCommandIconAtlasCell", CombatIconCatalog.CombatCommandSkillsIndex),
                "canceling skill targeting restores the stable Skills category icon");

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
            Assert(impactEchoes.Any(echo => echo.X == spellTarget.X && echo.Kind == "aimedshot"), "Aimed Shot receives its authored ranger-atlas impact echo");
            Assert(stagedGlyphs.Count == 0 && stagedFlashes.Count == 0, "shared Aimed Shot feedback suppresses the legacy ranger glyph and tile flash");
            Assert((int)scheduledSfx.GetType().GetProperty("Count").GetValue(scheduledSfx) >= 1, "Aimed Shot queues staged release and impact audio");

            combatState.Combat.Obstacles.Clear();
            active.Level = ProgressionRules.MaximumLevel;
            active.DamageMin = Math.Max(active.DamageMin, 12);
            active.DamageMax = Math.Max(active.DamageMax, 16);
            active.Skills.Arms = Math.Max(active.Skills.Arms, 40);
            active.Skills.Missile = Math.Max(active.Skills.Missile, 40);
            spellTarget.Defense = 0;
            spellTarget.ArmorBonus = 0;
            spellTarget.Agility = 1;
            spellTarget.MaxHp = Math.Max(spellTarget.MaxHp, 240);

            active.ClassKey = "warrior";
            active.Role = "shield";
            active.X = 1;
            active.Y = 1;
            spellTarget.X = 4;
            spellTarget.Y = 1;
            spellTarget.Hp = spellTarget.MaxHp;
            spellTarget.Stunned = 0;
            spellTarget.Guarding = false;
            spellTarget.GuardBonus = 0;
            spellTarget.Shielded = 0;
            combatState.Combat.Obstacles.Clear();
            combatState.Combat.ActionAvailable = true;
            stagedPowerTravel.Clear();
            castAuras.Clear();
            actorPoseBeats.Clear();
            combatTweens.RemoveAll(value => value.Id == active.Id && value.Kind == TweenKind.Move);
            int chargeSourceX = active.X;
            int chargeSourceY = active.Y;
            Point expectedChargeLanding = InvokePrivate<Point>(game, "BestChargeLanding", active, spellTarget);
            int chargeTargetHp = spellTarget.Hp;
            Assert(InvokePrivate<bool>(game, "UseTargetedAbility", active, "charge", spellTarget, spellTarget.X, spellTarget.Y), "Charge resolves through the centralized martial path");
            Assert(expectedChargeLanding != null
                && active.X == expectedChargeLanding.X
                && active.Y == expectedChargeLanding.Y
                && spellTarget.Hp < chargeTargetHp,
                "Charge follows its real open lane, lands beside the target, and deals damage");
            AssertProductionPowerActorChoreography(
                "Charge",
                active,
                "charge",
                chargeSourceX,
                chargeSourceY,
                active.X,
                active.Y,
                CombatPowerActorChoreographyKind.Dash,
                actorPoseBeats,
                castAuras,
                stagedPowerTravel,
                combatTweens);

            active.X = 1;
            active.Y = 1;
            spellTarget.X = 2;
            spellTarget.Y = 1;
            spellTarget.Hp = spellTarget.MaxHp;
            spellTarget.Stunned = 0;
            spellTarget.Guarding = true;
            spellTarget.GuardBonus = 1;
            spellTarget.Shielded = 3;
            combatState.Combat.ActionAvailable = true;
            SetPrivateField(game, "rng", new System.Random(1));
            int sunderHp = spellTarget.Hp;
            Assert(InvokePrivate<bool>(game, "UseTargetedAbility", active, "sunder", spellTarget, spellTarget.X, spellTarget.Y), "Sunder resolves through the centralized martial path");
            Assert(spellTarget.Hp < sunderHp
                && !spellTarget.Guarding
                && spellTarget.GuardBonus == 0
                && spellTarget.Shielded == 1, "Sunder deals measured damage, breaks Guard, and strips exactly two ward turns");

            active.ClassKey = "rogue";
            active.Role = "rogue";
            active.X = 1;
            active.Y = 1;
            active.Stealthed = 2;
            spellTarget.X = 4;
            spellTarget.Y = 1;
            spellTarget.Hp = spellTarget.MaxHp;
            spellTarget.Guarding = false;
            spellTarget.GuardBonus = 0;
            spellTarget.Shielded = 0;
            combatState.Combat.ActionAvailable = true;
            SetPrivateField(game, "rng", new System.Random(1));
            stagedPowerTravel.Clear();
            castAuras.Clear();
            actorPoseBeats.Clear();
            combatTweens.RemoveAll(value => value.Id == active.Id && value.Kind == TweenKind.Move);
            int shadowstepHp = spellTarget.Hp;
            int shadowstepSourceX = active.X;
            int shadowstepSourceY = active.Y;
            Point expectedShadowstepLanding = InvokePrivate<Point>(game, "BestShadowstepLanding", active, spellTarget);
            Assert(InvokePrivate<bool>(game, "UseTargetedAbility", active, "shadowstep", spellTarget, spellTarget.X, spellTarget.Y), "Shadowstep resolves through the centralized martial path");
            Assert(spellTarget.Hp < shadowstepHp
                && expectedShadowstepLanding != null
                && active.X == expectedShadowstepLanding.X
                && active.Y == expectedShadowstepLanding.Y
                && Math.Abs(active.X - spellTarget.X) + Math.Abs(active.Y - spellTarget.Y) == 1
                && active.Stealthed == 0, "Shadowstep lands beside its target, strikes, and consumes stealth");
            AssertProductionPowerActorChoreography(
                "Shadowstep",
                active,
                "shadowstep",
                shadowstepSourceX,
                shadowstepSourceY,
                active.X,
                active.Y,
                CombatPowerActorChoreographyKind.Teleport,
                actorPoseBeats,
                castAuras,
                stagedPowerTravel,
                combatTweens);

            active.ClassKey = "ranger";
            active.Role = "bow";
            active.X = 1;
            active.Y = 1;
            spellTarget.X = 4;
            spellTarget.Y = 1;
            spellTarget.Hp = spellTarget.MaxHp;
            combatState.Combat.ActionAvailable = true;
            SetPrivateField(game, "rng", new System.Random(1));
            int quickShotHp = spellTarget.Hp;
            int quickShotBeams = stagedBeams.Count(beam => beam.Kind == "shot");
            int quickShotTravel = stagedPowerTravel.Count(travel => travel.PowerKey == "quickshot");
            Assert(InvokePrivate<bool>(game, "UseTargetedAbility", active, "quickshot", spellTarget, spellTarget.X, spellTarget.Y), "Quick Shot resolves through the centralized martial path");
            Assert(spellTarget.Hp < quickShotHp
                && stagedPowerTravel.Count(travel => travel.PowerKey == "quickshot") == quickShotTravel + 1
                && stagedBeams.Count(beam => beam.Kind == "shot") >= quickShotBeams + 1, "Quick Shot uses authored primary travel and retains its independently timed second arrow");

            scheduledSfx.GetType().GetMethod("Clear").Invoke(scheduledSfx, null);
            stagedFloats.Clear();
            stagedBeams.Clear();
            stagedPowerTravel.Clear();
            stagedPowerAftermath.Clear();
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
            Rect intentTargetBoardRect = GetPrivateField<Rect>(game, "boardRect");
            Vector2Int? intentTargetSmokeHover = GetPrivateField<Vector2Int?>(game, "visualSmokeCombatHoverCell");
            SetPrivateField(game, "boardRect", Rect.zero);
            SetPrivateField<Vector2Int?>(game, "visualSmokeCombatHoverCell", null);
            CombatHudView enemyHudView = InvokePrivate<CombatHudView>(game, "BuildCombatHudView");
            SetPrivateField(game, "boardRect", intentTargetBoardRect);
            SetPrivateField(game, "visualSmokeCombatHoverCell", intentTargetSmokeHover);
            CombatAttackForecast bossForecast = InvokePrivate<CombatAttackForecast>(game, "AttackForecast", spellTarget, active);
            Assert(intendedTarget == active, "enemy intent uses the production target scorer");
            Assert(enemyHudView.PhaseLine.StartsWith("ENEMY TURN", StringComparison.Ordinal), "enemy initiative is announced as the primary combat phase cue");
            Assert(enemyHudView.ActionLabel == "ACTION\nENEMY"
                && enemyHudView.ActiveUnit.StateLine.Contains("DMG ")
                && enemyHudView.ActiveUnit.StateLine.Contains("DEF ")
                && enemyHudView.ActiveUnit.StateLine.Contains("SPD ")
                && !enemyHudView.ActiveUnit.StateLine.Contains("ACTION"), "enemy initiative keeps its action state in the header while the active card shows tactical stats");
            Assert(enemyHudView.TargetUnit != null && enemyHudView.TargetUnit.Name == active.Name, "enemy HUD target matches its tactical intent");
            Assert(enemyHudView.CommandPrompt.StartsWith("INTENT:", StringComparison.Ordinal) && enemyHudView.CommandPrompt.Contains(active.Name), "enemy turn publishes a concise target-aware intent line");
            Assert(enemyHudView.TargetSourceLabel == "INTENT", "enemy target rail labels the production AI intent source");
            Assert(enemyHudView.Commands.All(command => !command.Selected && !command.Armed && !command.Promoted),
                "enemy initiative cannot retain a stale player intent rail");
            hud.Refresh();
            Assert(hud.FocusedCommandForTest == null
                && hud.ContextCommandForTest == null
                && hud.CommandPromptForTest.StartsWith("INTENT:", StringComparison.Ordinal),
                "enemy initiative clears stale command focus and renders the canonical enemy intent prompt");
            Assert(bossForecast.Legal, "boss basic attack has a valid threat forecast in the smoke lane");
            Assert(enemyHudView.CommandPrompt.Contains(bossForecast.HitChance + "%") && enemyHudView.CommandPrompt.Contains(bossForecast.MinDamage + "-" + bossForecast.MaxDamage), "enemy intent publishes shared hit and damage estimates");
            Assert(enemyHudView.TargetUnit.StateLine.Contains(bossForecast.HitChance + "%"), "enemy target card shares the same forecast");
            float enemyCastStarted = Time.time;
            Assert(InvokePrivate<bool>(game, "TryKoboldKingFireball", spellTarget, active), "Kobold King fireball resolves through production enemy-power path");
            CombatPowerIdentity enemyCue = GetPrivateField<CombatPowerIdentity>(game, "combatPowerCue");
            Assert(enemyCue.Title == "Crooked Fireball" && enemyCue.Intensity == 3, "enemy power publishes its boss identity");
            PowerCastAura enemyFireballAura = castAuras.SingleOrDefault(aura =>
                aura != null
                && string.Equals(aura.PowerKey, "FBL", StringComparison.OrdinalIgnoreCase)
                && aura.SourceX == spellTarget.X
                && aura.SourceY == spellTarget.Y
                && aura.TargetX == active.X
                && aura.TargetY == active.Y);
            PowerTravelVfx[] enemyFireballTravel = stagedPowerTravel
                .Where(value => string.Equals(value.PowerKey, "FBL", StringComparison.OrdinalIgnoreCase))
                .ToArray();
            Assert(enemyFireballAura != null
                && enemyFireballAura.Intensity == 3
                && enemyFireballTravel.Length == 1
                && enemyFireballTravel[0].SourceX == spellTarget.X
                && enemyFireballTravel[0].SourceY == spellTarget.Y
                && enemyFireballTravel[0].TargetX == active.X
                && enemyFireballTravel[0].TargetY == active.Y,
                "enemy Fireball bridges Crooked Fireball into the exact FBL cast and authored source-to-target travel");
            PowerTravelVfx enemyFireballProjectile = enemyFireballTravel[0];
            CombatPowerAnimationTimeline enemyFireballTimeline = CombatPowerAnimationTimelineRules.For(
                "FBL",
                enemyFireballProjectile.StableSeed,
                enemyFireballProjectile.Intensity,
                false);
            Assert(enemyFireballTimeline.Supported
                && enemyFireballTimeline.HasTravel
                && enemyFireballAura.StableSeed != 0
                && enemyFireballAura.StableSeed == enemyFireballProjectile.StableSeed
                && Math.Abs(enemyFireballAura.ReleaseAt - (enemyFireballAura.Start + enemyFireballTimeline.ReleaseAt)) < 0.0001f
                && Math.Abs(enemyFireballAura.ImpactAt - (enemyFireballAura.Start + enemyFireballTimeline.ImpactAt)) < 0.0001f
                && Math.Abs(enemyFireballProjectile.Start - enemyFireballAura.ReleaseAt) < 0.0001f
                && Math.Abs(enemyFireballProjectile.Start + enemyFireballProjectile.Duration - enemyFireballAura.ImpactAt) < 0.0001f,
                "enemy FBL cast, release, authored travel, and impact share one deterministic stable-seed timeline");
            Assert(!stagedBeams.Any(value => value.Kind == "fireball"),
                "enemy Fireball suppresses the duplicate legacy fireball beam when exact FBL travel is authored");
            Assert(stagedFloats.Any(value => value.Start > enemyCastStarted + 0.04f), "enemy fireball result waits for impact timing");
            Assert((int)scheduledSfx.GetType().GetProperty("Count").GetValue(scheduledSfx) >= 1, "enemy fireball queues delayed impact audio");
            Assert(impactEchoes.Any(echo => echo.X == active.X
                && echo.Y == active.Y
                && string.Equals(echo.Kind, "FBL", StringComparison.OrdinalIgnoreCase)
                && echo.Intensity == 3
                && echo.Duration >= 0.62f),
                "boss Fireball stages its exact FBL impact identity at the party target");
            PowerAftermathVfx enemyFireballAftermath = stagedPowerAftermath.SingleOrDefault(value =>
                string.Equals(value.PowerKey, "FBL", StringComparison.OrdinalIgnoreCase));
            Assert(enemyFireballAftermath != null
                && enemyFireballAftermath.X == active.X
                && enemyFireballAftermath.Y == active.Y
                && enemyFireballAftermath.StableSeed == enemyFireballAura.StableSeed
                && enemyFireballAftermath.Duration > 0f
                && Math.Abs(enemyFireballAftermath.Start - (enemyFireballAura.Start + enemyFireballTimeline.AftermathAt)) < 0.04f,
                "enemy FBL stages the canonical deterministic Fireball aftermath on its authored aftermath beat");
            Assert(GetPrivateField<float>(game, "combatMusicDuckDepth") >= 0.46f, "boss fireball claims stronger space in the combat mix");
            Assert(GetPrivateField<float>(game, "enemyActionResolutionDelay") >= 0.50f, "boss power requests a readable resolution hold");
            Assert(Math.Abs(GetPrivateField<float>(game, "combatVfxImpactDelay")) < 0.0001f, "enemy power restores VFX timeline context");
            InvokePrivate(game, "FinishEnemyCombatAction", spellTarget);
            Assert(GetPrivateField<bool>(game, "combatAdvancePending"), "enemy power locks initiative until its impact resolves");
            Assert(combatState.Combat.Phase == CombatPhase.Resolving, "enemy power exposes the resolving combat phase");
            Assert(GetPrivateField<string>(game, "combatResolutionLabel") == "Crooked Fireball", "enemy resolution names the active power");
            InvokePrivate(game, "CancelCombatResolutionBeat", false);

            actorPoseBeats.Clear();
            combatTweens.Clear();
            stagedPowerTravel.Clear();
            stagedPowerAftermath.Clear();
            impactEchoes.Clear();
            castAuras.Clear();
            combatState.Combat.Obstacles.Clear();
            active.X = 1;
            active.Y = 1;
            active.Hp = active.MaxHp;
            spellTarget.X = 4;
            spellTarget.Y = 1;
            spellTarget.Webbed = 0;
            int royalChargeSourceX = spellTarget.X;
            int royalChargeSourceY = spellTarget.Y;
            Point royalChargeLanding = InvokePrivate<Point>(game, "BestEnemyChargeLanding", spellTarget, active, 5);
            Assert(royalChargeLanding != null, "Kobold King smoke lane has a legal royal charge landing");
            Assert(InvokePrivate<bool>(game, "TryKoboldKingChargeAndRetreat", spellTarget, active),
                "Kobold King charge-and-retreat resolves through the production enemy-power path");
            PowerActorPoseBeat royalChargeActor = actorPoseBeats.SingleOrDefault(value =>
                value != null
                && string.Equals(value.UnitId, spellTarget.Id, StringComparison.Ordinal)
                && string.Equals(value.PowerKey, "charge", StringComparison.OrdinalIgnoreCase)
                && value.Role == CombatPowerActorPoseRole.Source);
            Assert(royalChargeActor != null
                && royalChargeActor.SourceX == royalChargeSourceX
                && royalChargeActor.SourceY == royalChargeSourceY
                && royalChargeActor.LandingX == royalChargeLanding.X
                && royalChargeActor.LandingY == royalChargeLanding.Y
                && royalChargeActor.Duration > 0f,
                "royal charge actor beat owns the exact source-to-landing leg");
            CombatPowerActorPosePlan royalChargePlan = CombatPowerActorPoseRules.For(
                royalChargeActor.PowerKey,
                royalChargeActor.StableSeed,
                royalChargeActor.SourceX,
                royalChargeActor.SourceY,
                royalChargeActor.LandingX,
                royalChargeActor.LandingY,
                royalChargeActor.Intensity,
                false);
            CombatPowerActorPoseFrame royalChargeImpact = royalChargePlan.SourceFrameAt(royalChargePlan.ImpactAt);
            Assert(royalChargePlan.Choreography == CombatPowerActorChoreographyKind.Dash
                && Math.Abs(royalChargeActor.Duration - royalChargePlan.DurationSeconds) < 0.0001f
                && Math.Abs(royalChargeImpact.PositionX - royalChargeLanding.X) < 0.0001f
                && Math.Abs(royalChargeImpact.PositionY - royalChargeLanding.Y) < 0.0001f,
                "royal charge actor reaches its tactical landing on the canonical impact beat");
            Tween[] royalRetreatTweens = combatTweens
                .Where(value => value != null && value.Id == spellTarget.Id && value.Kind == TweenKind.Move)
                .ToArray();
            float royalChargeActorCompleteAt = royalChargeActor.Start + royalChargeActor.Duration;
            Assert(royalRetreatTweens.Length == 1
                && Math.Abs(royalRetreatTweens[0].From.x - royalChargeLanding.X) < 0.0001f
                && Math.Abs(royalRetreatTweens[0].From.y - royalChargeLanding.Y) < 0.0001f
                && Math.Abs(royalRetreatTweens[0].To.x - spellTarget.X) < 0.0001f
                && Math.Abs(royalRetreatTweens[0].To.y - spellTarget.Y) < 0.0001f
                && (spellTarget.X != royalChargeLanding.X || spellTarget.Y != royalChargeLanding.Y)
                && Math.Abs(royalRetreatTweens[0].Start - royalChargeActorCompleteAt) < 0.0001f
                && !royalRetreatTweens.Any(value => value.Start < royalChargeActorCompleteAt - 0.0001f),
                "royal retreat begins exactly after authored charge recovery and has no masked immediate Move tween");

            actorPoseBeats.Clear();
            combatTweens.Clear();
            stagedPowerTravel.Clear();
            stagedPowerAftermath.Clear();
            impactEchoes.Clear();
            castAuras.Clear();
            active.X = 1;
            active.Y = 1;
            active.Hp = active.MaxHp;
            spellTarget.X = 4;
            spellTarget.Y = 1;

            bool enemyOriginalReducedMotion = combatState.ReducedMotion;
            int enemyReducedMotionTargetHp = active.Hp;
            List<Point> enemyReducedMotionObstacles = combatState.Combat.Obstacles.ToList();
            try
            {
                combatState.ReducedMotion = true;
                active.Hp = active.MaxHp;
                stagedFloats.Clear();
                stagedBeams.Clear();
                stagedPowerTravel.Clear();
                stagedPowerAftermath.Clear();
                impactEchoes.Clear();
                castAuras.Clear();
                Assert(InvokePrivate<bool>(game, "TryKoboldKingFireball", spellTarget, active),
                    "Kobold King Fireball remains functional under Reduced Motion");
                Assert(castAuras.Count == 0
                    && stagedPowerTravel.Count == 0
                    && stagedPowerAftermath.Count == 0
                    && !stagedBeams.Any(value => value.Kind == "fireball")
                    && impactEchoes.Count == 1
                    && string.Equals(impactEchoes[0].Kind, "FBL", StringComparison.OrdinalIgnoreCase)
                    && impactEchoes[0].StaticStamp
                    && Math.Abs(impactEchoes[0].ImpactAt - impactEchoes[0].Start) < 0.0001f,
                    "enemy exact-power bridge collapses FBL to one immediate static impact under Reduced Motion");
            }
            finally
            {
                combatState.ReducedMotion = enemyOriginalReducedMotion;
                active.Hp = enemyReducedMotionTargetHp;
                combatState.Combat.Obstacles.Clear();
                combatState.Combat.Obstacles.AddRange(enemyReducedMotionObstacles);
                stagedFloats.Clear();
                stagedBeams.Clear();
                stagedPowerTravel.Clear();
                stagedPowerAftermath.Clear();
                impactEchoes.Clear();
                castAuras.Clear();
            }

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
                (object)new[] { "-ashen-combat-smoke", "-ashen-combat-state", "cursor-cycle" });
            stagedActive = InvokePrivate<CombatUnit>(game, "CurrentUnit");
            Vector2Int? stagedCursor = GetPrivateField<Vector2Int?>(game, "combatBoardCursorCell");
            List<Vector2Int> stagedCycleTargets = InvokePrivate<List<Vector2Int>>(game, "CombatBoardCursorCandidates", stagedActive);
            CombatHudView stagedCursorView = InvokePrivate<CombatHudView>(game, "BuildCombatHudView");
            Assert(GetPrivateField<bool>(game, "combatBoardCursorActive")
                && !GetPrivateField<Vector2Int?>(game, "visualSmokeCombatHoverCell").HasValue
                && GetPrivateField<ActionMode>(game, "selectedAction") == ActionMode.Attack
                && stagedCycleTargets.Count >= 2
                && stagedCursor == stagedCycleTargets[0]
                && stagedCycleTargets.All(cell => InvokePrivate<bool>(game, "CombatBoardCursorCellIsLegal", stagedActive, cell.x, cell.y)),
                "clean cursor-cycle capture stages a visible controller focus on the first of at least two legal weapon targets");
            Assert(stagedCursorView.CommandPrompt.StartsWith("CURSOR ", StringComparison.Ordinal)
                && stagedCursorView.TargetSourceLabel == "CURSOR",
                "clean cursor-cycle capture exposes the cursor prompt and target ownership in the migrated HUD");
            bool stagedOriginalReducedMotion = stagedCombatState.ReducedMotion;
            try
            {
                stagedCombatState.ReducedMotion = true;
                Assert(InvokePrivate<bool>(game, "CycleCombatBoardCursor", stagedActive, 1)
                    && GetPrivateField<Vector2Int?>(game, "combatBoardCursorCell") == stagedCycleTargets[1],
                    "cursor-cycle visual smoke advances deterministically under Reduced Motion");
            }
            finally
            {
                stagedCombatState.ReducedMotion = stagedOriginalReducedMotion;
            }

            InvokePrivate(
                game,
                "StageVisualSmokeCombatState",
                (object)new[] { "-ashen-combat-smoke", "-ashen-combat-state", "spell-aoe" });
            Assert(GetPrivateField<ActionMode>(game, "selectedAction") == ActionMode.Cast
                && GetPrivateField<string>(game, "pendingFormulaCode") == "FBL"
                && GetPrivateField<Vector2Int?>(game, "visualSmokeCombatHoverCell").HasValue, "clean area-spell capture reaches armed targeting deterministically");

            InvokePrivate(game, "PromoteWarlockTester", active);
            CombatUnit warlockTester = InvokePrivate<CombatUnit>(game, "CurrentUnit");
            string[] warlockFormulaCodes = InvokePrivate<IEnumerable<FormulaDef>>(game, "KnownFormulasFor", warlockTester)
                .Select(formula => formula.Code)
                .ToArray();
            Assert(warlockTester.Id != mageTesterId
                && warlockTester.Side == UnitSide.Party
                && warlockTester.ClassKey == "warlock"
                && warlockTester.Role == "hex"
                && warlockTester.Spell == "hex|pact"
                && warlockTester.Level == ProgressionRules.MaximumLevel,
                "Beta Lab Warlock preset activates a dedicated maximum-level hex and pact tester");
            Assert(new[] { "RBT", "IBD", "PBR", "IBF", "VRS", "DMC", "IBG", "RLM", "DFA" }.All(warlockFormulaCodes.Contains),
                "Beta Lab Warlock preset exposes rift, summon, doom, death-burst, and ascendance capstones");
            Assert(combatState.Combat.ActiveId == warlockTester.Id
                && combatState.Combat.Phase == CombatPhase.ChooseAction
                && combatState.Combat.ActionAvailable
                && GetPrivateField<ActionMode>(game, "selectedAction") == ActionMode.Cast
                && string.IsNullOrEmpty(GetPrivateField<string>(game, "pendingFormulaCode"))
                && !GetPrivateField<bool>(game, "combatAdvancePending")
                && GetPrivateField<float>(game, "aiActAt") < 0f,
                "Beta Lab Warlock takeover clears stale targeting and begins a valid player casting turn");

            bool showcaseOriginalReducedMotion = combatState.ReducedMotion;
            bool showcaseOriginalSfxMuted = combatState.SfxMuted;
            try
            {
                combatState.ReducedMotion = false;
                combatState.SfxMuted = false;
                SetPrivateField(game, "betaVfxShowcaseOpen", true);
                SetPrivateField(game, "betaVfxShowcaseIndex", 0);
                InvokePrivate(game, "ReplayBetaVfxShowcase");
                Assert(castAuras.Count == 1
                    && castAuras[0].Kind == "FBL"
                    && stagedPowerTravel.Count == 1
                    && stagedPowerTravel[0].PowerKey == "FBL"
                    && stagedPowerAftermath.Count == 1
                    && string.Equals(stagedPowerAftermath[0].PowerKey, "FBL", StringComparison.OrdinalIgnoreCase)
                    && impactEchoes.Count == 1
                    && impactEchoes[0].Kind == "FBL"
                    && !impactEchoes[0].StaticStamp,
                    "Beta VFX Showcase replays canonical Fireball through authored cast, travel, impact, and aftermath presentation");
                int firstImpactX = impactEchoes[0].X;
                int firstImpactY = impactEchoes[0].Y;
                PowerCastAura firstShowcaseAura = castAuras[0];
                PowerTravelVfx firstShowcaseTravel = stagedPowerTravel[0];
                PowerAftermathVfx firstShowcaseAftermath = stagedPowerAftermath[0];
                PowerImpactEcho firstShowcaseImpact = impactEchoes[0];
                SetPrivateField(game, "combatAdvancePending", true);
                try
                {
                    InvokePrivate(
                        game,
                        "ExecuteBetaLabToolbarAction",
                        BetaLabToolbarActionId.VisualTour,
                        warlockTester);
                    InvokePrivate(game, "ReplayBetaVfxShowcase");
                    Assert(GetPrivateField<bool>(game, "combatAdvancePending")
                        && GetPrivateField<bool>(game, "betaVfxShowcaseOpen")
                        && castAuras.Count == 1
                        && ReferenceEquals(castAuras[0], firstShowcaseAura)
                        && stagedPowerTravel.Count == 1
                        && ReferenceEquals(stagedPowerTravel[0], firstShowcaseTravel)
                        && stagedPowerAftermath.Count == 1
                        && ReferenceEquals(stagedPowerAftermath[0], firstShowcaseAftermath)
                        && impactEchoes.Count == 1
                        && ReferenceEquals(impactEchoes[0], firstShowcaseImpact),
                        "Beta actions cannot clear or replace a production spell presentation while resolution is pending");
                }
                finally
                {
                    SetPrivateField(game, "combatAdvancePending", false);
                }
                CombatPowerAftermathVfxProfile firstShowcaseAftermathProfile = CombatPowerAftermathVfxRules.ProfileFor(firstShowcaseAftermath.PowerKey);
                CombatPowerAnimationTimeline firstShowcaseTimeline = CombatPowerAnimationTimelineRules.For(
                    firstShowcaseAftermath.PowerKey,
                    firstShowcaseAftermath.StableSeed,
                    firstShowcaseAftermath.Intensity,
                    false);
                float firstShowcaseAftermathOffset = firstShowcaseAftermath.Start - firstShowcaseAura.Start;
                Assert(firstShowcaseAftermathProfile.HasAftermath
                    && firstShowcaseAftermathProfile.AtlasCell == (int)CombatPowerAftermathKind.Fireball
                    && firstShowcaseAftermath.StableSeed != 0
                    && Math.Abs(firstShowcaseAftermathOffset - firstShowcaseTimeline.AftermathAt) < 0.0001f,
                    "Beta VFX Showcase Fireball uses the exact deterministic aftermath profile and timeline");
                InvokePrivate(game, "ReplayBetaVfxShowcase");
                PowerAftermathVfx repeatedShowcaseAftermath = stagedPowerAftermath.Single();
                CombatPowerAftermathVfxProfile repeatedShowcaseAftermathProfile = CombatPowerAftermathVfxRules.ProfileFor(repeatedShowcaseAftermath.PowerKey);
                Assert(castAuras.Count == 1
                    && stagedPowerTravel.Count == 1
                    && stagedPowerTravel[0].PowerKey == firstShowcaseTravel.PowerKey
                    && stagedPowerTravel[0].SourceX == firstShowcaseTravel.SourceX
                    && stagedPowerTravel[0].SourceY == firstShowcaseTravel.SourceY
                    && stagedPowerTravel[0].TargetX == firstShowcaseTravel.TargetX
                    && stagedPowerTravel[0].TargetY == firstShowcaseTravel.TargetY
                    && stagedPowerTravel[0].SequenceIndex == firstShowcaseTravel.SequenceIndex
                    && stagedPowerTravel[0].StableSeed == firstShowcaseTravel.StableSeed
                    && Math.Abs(stagedPowerTravel[0].Duration - firstShowcaseTravel.Duration) < 0.0001f
                    && stagedPowerAftermath.Count == 1
                    && repeatedShowcaseAftermath.StableSeed == firstShowcaseAftermath.StableSeed
                    && repeatedShowcaseAftermath.Intensity == firstShowcaseAftermath.Intensity
                    && repeatedShowcaseAftermath.SequenceIndex == firstShowcaseAftermath.SequenceIndex
                    && repeatedShowcaseAftermathProfile.AtlasCell == firstShowcaseAftermathProfile.AtlasCell
                    && string.Equals(repeatedShowcaseAftermathProfile.Key, firstShowcaseAftermathProfile.Key, StringComparison.OrdinalIgnoreCase)
                    && Math.Abs(repeatedShowcaseAftermath.Duration - firstShowcaseAftermath.Duration) < 0.0001f
                    && Math.Abs((repeatedShowcaseAftermath.Start - castAuras[0].Start) - firstShowcaseAftermathOffset) < 0.0001f
                    && impactEchoes.Count == 1
                    && impactEchoes[0].X == firstImpactX
                    && impactEchoes[0].Y == firstImpactY,
                    "Beta VFX Showcase replay deterministically replaces stale travel, impact, and aftermath presentation");

                SetPrivateField(game, "betaVfxShowcaseIndex", CombatVfxShowcaseRules.NextIndex(0));
                InvokePrivate(game, "ReplayBetaVfxShowcase");
                Assert(castAuras.Count == 1
                    && castAuras[0].Kind == "MTR"
                    && stagedPowerTravel.Count == 5
                    && stagedPowerTravel.All(value => value.PowerKey == "MTR")
                    && stagedPowerTravel.Select(value => value.SequenceIndex).OrderBy(value => value).SequenceEqual(new[] { 0, 1, 2, 3, 4 })
                    && impactEchoes.Count == 1
                    && impactEchoes[0].Kind == "MTR",
                    "Beta VFX Showcase Next advances to Meteor Shower and stages its deterministic five-strike travel sequence");

                SetPrivateField(game, "betaVfxShowcaseIndex", CombatVfxShowcaseRules.IndexFor("charge"));
                InvokePrivate(game, "ReplayBetaVfxShowcase");
                Assert(stagedPowerTravel.Count == 1
                    && stagedPowerTravel[0].PowerKey == "charge",
                    "Beta VFX Showcase exercises one representative targeted class-skill travel identity");

                Dictionary<string, CombatPowerActorChoreographyKind> signatureSkillTour =
                    new Dictionary<string, CombatPowerActorChoreographyKind>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["whirlwind"] = CombatPowerActorChoreographyKind.Whirl,
                        ["rally"] = CombatPowerActorChoreographyKind.Brace,
                        ["quickshot"] = CombatPowerActorChoreographyKind.Bow,
                        ["stealth"] = CombatPowerActorChoreographyKind.Vanish,
                        ["sunder"] = CombatPowerActorChoreographyKind.HeavyStrike
                    };
                foreach (KeyValuePair<string, CombatPowerActorChoreographyKind> expected in signatureSkillTour)
                {
                    int showcaseIndex = CombatVfxShowcaseRules.IndexFor(expected.Key);
                    Assert(showcaseIndex >= 0, expected.Key + " belongs to the expanded Beta skill tour");
                    SetPrivateField(game, "betaVfxShowcaseIndex", showcaseIndex);
                    InvokePrivate(game, "ReplayBetaVfxShowcase");
                    PowerActorPoseBeat poseBeat = actorPoseBeats.SingleOrDefault(value =>
                        value != null
                        && value.Role == CombatPowerActorPoseRole.Source
                        && string.Equals(value.PowerKey, expected.Key, StringComparison.OrdinalIgnoreCase));
                    Assert(poseBeat != null, expected.Key + " Beta replay stages one production actor-pose beat");
                    CombatPowerActorPosePlan posePlan = CombatPowerActorPoseRules.ForAbility(
                        expected.Key,
                        poseBeat.StableSeed,
                        poseBeat.SourceX,
                        poseBeat.SourceY,
                        poseBeat.LandingX,
                        poseBeat.LandingY,
                        poseBeat.Intensity,
                        false);
                    CombatPowerActorPoseFrame releaseFrame = posePlan.SourceFrameAt(
                        (posePlan.ReleaseAt + posePlan.ReleaseEndAt) * 0.5f);
                    Assert(posePlan.Choreography == expected.Value
                        && releaseFrame.Phase == CombatPowerActorPosePhase.Release
                        && releaseFrame.IsVisible,
                        expected.Key + " Beta replay reaches its distinct visible release choreography");
                }

                int visualCountBeforeCue = castAuras.Count + stagedPowerTravel.Count + impactEchoes.Count;
                InvokePrivate(game, "CueBetaVfxShowcaseAudio");
                Assert(castAuras.Count + stagedPowerTravel.Count + impactEchoes.Count == visualCountBeforeCue,
                    "Beta VFX Showcase Cue auditions sound without mutating visual or combat state");

                combatState.ReducedMotion = true;
                SetPrivateField(game, "betaVfxShowcaseIndex", 0);
                InvokePrivate(game, "ReplayBetaVfxShowcase");
                Assert(castAuras.Count == 0
                    && stagedPowerTravel.Count == 0
                    && stagedPowerAftermath.Count == 0
                    && impactEchoes.Count == 1
                    && impactEchoes[0].Kind == "FBL"
                    && impactEchoes[0].StaticStamp,
                    "Beta VFX Showcase replays real spell identity through the Reduced Motion static-stamp contract");
            }
            finally
            {
                combatState.ReducedMotion = showcaseOriginalReducedMotion;
                combatState.SfxMuted = showcaseOriginalSfxMuted;
                SetPrivateField(game, "betaVfxShowcaseOpen", false);
                InvokePrivate(game, "ClearBetaVfxShowcasePresentation");
            }
            SetPrivateField(game, "betaLabToolbarFocused", true);
            SetPrivateField(game, "betaVfxShowcaseOpen", true);
            Assert(InvokePrivate<bool>(game, "CancelBetaLabToolbarFocus")
                && !GetPrivateField<bool>(game, "betaVfxShowcaseOpen")
                && GetPrivateField<bool>(game, "betaLabToolbarFocused"),
                "first Beta cancel closes the visual preview without leaking focus into combat");
            Assert(InvokePrivate<bool>(game, "CancelBetaLabToolbarFocus")
                && !GetPrivateField<bool>(game, "betaLabToolbarFocused"),
                "second Beta cancel releases the lab rail and restores combat-command ownership");
            AssertCombatTransientPresentationBoundariesRuntime(game);
        }

        private static PowerActorPoseBeat AssertProductionPowerActorChoreography(
            string label,
            CombatUnit actor,
            string powerKey,
            int sourceX,
            int sourceY,
            int landingX,
            int landingY,
            CombatPowerActorChoreographyKind expectedChoreography,
            List<PowerActorPoseBeat> actorPoseBeats,
            List<PowerCastAura> castAuras,
            List<PowerTravelVfx> powerTravel,
            List<Tween> combatTweens)
        {
            PowerActorPoseBeat[] matchingBeats = actorPoseBeats
                .Where(value => value != null
                    && string.Equals(value.UnitId, actor.Id, StringComparison.Ordinal)
                    && string.Equals(value.PowerKey, powerKey, StringComparison.OrdinalIgnoreCase)
                    && value.Role == CombatPowerActorPoseRole.Source)
                .ToArray();
            Assert(matchingBeats.Length == 1,
                label + " stages exactly one source actor choreography beat");
            PowerActorPoseBeat beat = matchingBeats[0];
            PowerCastAura aura = castAuras.LastOrDefault(value => value != null
                && string.Equals(value.PowerKey, powerKey, StringComparison.OrdinalIgnoreCase)
                && value.StableSeed == beat.StableSeed);
            PowerTravelVfx[] matchingTravel = powerTravel
                .Where(value => value != null
                    && string.Equals(value.PowerKey, powerKey, StringComparison.OrdinalIgnoreCase))
                .ToArray();
            CombatPowerActorPosePlan plan = CombatPowerActorPoseRules.For(
                powerKey,
                beat.StableSeed,
                sourceX,
                sourceY,
                landingX,
                landingY,
                beat.Intensity,
                false);
            CombatPowerAnimationTimeline timeline = CombatPowerAnimationTimelineRules.For(
                powerKey,
                beat.StableSeed,
                beat.Intensity,
                false);

            Assert(plan.Supported
                && timeline.Supported
                && plan.Choreography == expectedChoreography
                && beat.SourceX == sourceX
                && beat.SourceY == sourceY
                && beat.LandingX == landingX
                && beat.LandingY == landingY
                && beat.StableSeed != 0
                && Math.Abs(beat.Duration - plan.DurationSeconds) < 0.0001f,
                label + " records its real source, actual landing, stable seed, and bounded actor duration");
            Assert(aura != null
                && aura.StableSeed == beat.StableSeed
                && Math.Abs(aura.Start - beat.Start) < 0.0001f
                && Math.Abs(plan.ReleaseAt - timeline.ReleaseAt) < 0.0001f
                && Math.Abs(plan.ImpactAt - timeline.ImpactAt) < 0.0001f
                && Math.Abs(aura.ReleaseAt - (beat.Start + plan.ReleaseAt)) < 0.0001f
                && Math.Abs(aura.ImpactAt - (beat.Start + plan.ImpactAt)) < 0.0001f,
                label + " actor windup, release, and impact share the exact cast-aura seed and clock");
            Assert(matchingTravel.Length == 1
                && matchingTravel[0].StableSeed == beat.StableSeed
                && matchingTravel[0].SourceX == sourceX
                && matchingTravel[0].SourceY == sourceY
                && matchingTravel[0].TargetX == landingX
                && matchingTravel[0].TargetY == landingY
                && Math.Abs(matchingTravel[0].Start - (beat.Start + plan.ReleaseAt)) < 0.0001f
                && Math.Abs(matchingTravel[0].Start + matchingTravel[0].Duration - (beat.Start + plan.ImpactAt)) < 0.0001f,
                label + " authored travel begins at actor release and ends at the actor's real landing on impact");
            Assert(!combatTweens.Any(value => value.Id == actor.Id && value.Kind == TweenKind.Move),
                label + " uses authored actor choreography without a legacy Move tween");

            float preReleaseAt = Math.Max(0f, plan.ReleaseAt - Math.Min(0.0001f, plan.ReleaseAt * 0.25f));
            CombatPowerActorPoseFrame preRelease = plan.SourceFrameAt(preReleaseAt);
            CombatPowerActorPoseFrame onRelease = plan.SourceFrameAt(plan.ReleaseAt);
            Assert(preRelease.Phase == CombatPowerActorPosePhase.CastWindup,
                label + " remains in windup until the exact release boundary");

            if (expectedChoreography == CombatPowerActorChoreographyKind.Dash)
            {
                CombatPowerActorPoseFrame midDash = plan.SourceFrameAt((plan.ReleaseAt + plan.ImpactAt) * 0.5f);
                CombatPowerActorPoseFrame onImpact = plan.SourceFrameAt(plan.ImpactAt);
                float minimumX = Math.Min(sourceX, landingX) - 0.0001f;
                float maximumX = Math.Max(sourceX, landingX) + 0.0001f;
                float minimumY = Math.Min(sourceY, landingY) - 0.0001f;
                float maximumY = Math.Max(sourceY, landingY) + 0.0001f;
                Assert(onRelease.Phase == CombatPowerActorPosePhase.Dash
                    && midDash.Phase == CombatPowerActorPosePhase.Dash
                    && midDash.PositionX >= minimumX
                    && midDash.PositionX <= maximumX
                    && midDash.PositionY >= minimumY
                    && midDash.PositionY <= maximumY
                    && Math.Abs(midDash.OffsetX) <= CombatPowerActorPoseRules.MaximumOffset
                    && Math.Abs(midDash.OffsetY) <= CombatPowerActorPoseRules.MaximumOffset
                    && midDash.Scale >= CombatPowerActorPoseRules.MinimumScale
                    && midDash.Scale <= CombatPowerActorPoseRules.MaximumScale
                    && onImpact.Phase == CombatPowerActorPosePhase.Recovery
                    && Math.Abs(onImpact.PositionX - landingX) < 0.0001f
                    && Math.Abs(onImpact.PositionY - landingY) < 0.0001f,
                    label + " pure dash evaluator stays bounded and lands exactly on its impact boundary");
            }
            else if (expectedChoreography == CombatPowerActorChoreographyKind.Teleport)
            {
                float splitEpsilon = Math.Min(0.0001f, (plan.TeleportSplitAt - plan.ReleaseAt) * 0.25f);
                CombatPowerActorPoseFrame lastSource = plan.SourceFrameAt(plan.TeleportSplitAt - splitEpsilon);
                CombatPowerActorPoseFrame firstLanding = plan.SourceFrameAt(plan.TeleportSplitAt);
                CombatPowerActorPoseFrame onImpact = plan.SourceFrameAt(plan.ImpactAt);
                Assert(onRelease.Phase == CombatPowerActorPosePhase.TeleportOut
                    && lastSource.Phase == CombatPowerActorPosePhase.TeleportOut
                    && Math.Abs(lastSource.PositionX - sourceX) < 0.0001f
                    && Math.Abs(lastSource.PositionY - sourceY) < 0.0001f
                    && lastSource.Opacity < 0.02f
                    && firstLanding.Phase == CombatPowerActorPosePhase.TeleportIn
                    && Math.Abs(firstLanding.PositionX - landingX) < 0.0001f
                    && Math.Abs(firstLanding.PositionY - landingY) < 0.0001f
                    && firstLanding.Opacity < 0.0001f
                    && onImpact.Phase == CombatPowerActorPosePhase.Recovery
                    && Math.Abs(onImpact.PositionX - landingX) < 0.0001f
                    && Math.Abs(onImpact.PositionY - landingY) < 0.0001f,
                    label + " pure teleport evaluator vanishes at source, snaps while invisible, and reveals at landing");
            }
            else
            {
                Assert(onRelease.Phase == CombatPowerActorPosePhase.Release,
                    label + " source actor exits windup on the exact canonical release boundary");
            }

            return beat;
        }

        private static void AssertTitleMenuFocusReducedMotionRuntime(
            AshenHallsGame game,
            TavernScreen tavernScreen,
            MethodInfo titleUpdate,
            Text selectedCursor)
        {
            GameState titleState = GetPrivateField<GameState>(game, "state");
            bool originalReducedMotion = titleState.ReducedMotion;
            try
            {
                titleState.ReducedMotion = true;
                titleUpdate.Invoke(tavernScreen, null);
                TitleMenuFocusFrame expected = TitleScreenPresentationRules.EvaluateMenuFocus(Time.unscaledTime, true);
                Vector3 scale = selectedCursor.rectTransform.localScale;
                Assert(Math.Abs(selectedCursor.color.a - expected.CursorAlpha) < 0.0001f
                    && Math.Abs(scale.x - expected.CursorScale) < 0.0001f
                    && Math.Abs(scale.y - expected.CursorScale) < 0.0001f,
                    "live Reduced Motion title focus uses one static alpha and scale frame");
            }
            finally
            {
                titleState.ReducedMotion = originalReducedMotion;
                titleUpdate.Invoke(tavernScreen, null);
            }
        }

        private static void AssertCombatTransientPresentationBoundariesRuntime(AshenHallsGame game)
        {
            StageStaleCombatPresentationBoundarySentinels(game);
            InvokePrivate(game, "StartCombat", "patrol");
            AssertNoStaleCombatPresentationBoundarySentinels(game, "new encounter");

            GameState adoptedSource = GetPrivateField<GameState>(game, "state");
            string adoptedJson = JsonUtility.ToJson(adoptedSource);
            GameState successfullyLoaded = JsonUtility.FromJson<GameState>(adoptedJson);
            GameState invalidLoaded = JsonUtility.FromJson<GameState>(adoptedJson);
            invalidLoaded.Party = null;
            string repairedContentSet = GetPrivateField<string>(game, "activeContentSet");

            StageStaleCombatPresentationBoundarySentinels(game);
            bool invalidLoadRejected = false;
            try
            {
                InvokePrivate(game, "AdoptLoadedGameState", invalidLoaded, invalidLoaded.SaveVersion, repairedContentSet);
            }
            catch (InvalidOperationException)
            {
                invalidLoadRejected = true;
            }
            Assert(invalidLoadRejected, "invalid load adoption is rejected before transient presentation is reset");
            AssertStaleCombatPresentationBoundarySentinelsPresent(game, "rejected load");

            InvokePrivate(game, "AdoptLoadedGameState", successfullyLoaded, successfullyLoaded.SaveVersion, repairedContentSet);
            Assert(ReferenceEquals(GetPrivateField<GameState>(game, "state"), successfullyLoaded),
                "successful load adoption installs the validated state before clearing presentation");
            AssertNoStaleCombatPresentationBoundarySentinels(game, "successful load adoption");
        }

        private static void StageStaleCombatPresentationBoundarySentinels(AshenHallsGame game)
        {
            const string sentinel = "stale-boundary";
            float now = Time.time;
            GetPrivateField<List<Tween>>(game, "tweens").Add(new Tween(sentinel, Vector2.zero, Vector2.one, now, 30f, TweenKind.Move));
            GetPrivateField<List<FloatText>>(game, "floatTexts").Add(new FloatText { Text = sentinel, Start = now, Duration = 30f });
            GetPrivateField<List<ParticleDot>>(game, "particles").Add(new ParticleDot { Kind = sentinel, Start = now, Duration = 30f });
            GetPrivateField<List<BeamEffect>>(game, "beams").Add(new BeamEffect { Kind = sentinel, Start = now, Duration = 30f });
            GetPrivateField<List<CellFlash>>(game, "flashes").Add(new CellFlash { Color = "010203", Start = now, Duration = 30f });
            GetPrivateField<List<CastGlyph>>(game, "castGlyphs").Add(new CastGlyph { Kind = sentinel, Start = now, Duration = 30f });
            GetPrivateField<List<PowerCastAura>>(game, "powerCastAuras").Add(new PowerCastAura { PowerKey = sentinel, Start = now, Duration = 30f });
            GetPrivateField<List<PowerTravelVfx>>(game, "powerTravelVfx").Add(new PowerTravelVfx { PowerKey = sentinel, Start = now, Duration = 30f });
            GetPrivateField<List<PowerImpactEcho>>(game, "powerImpactEchoes").Add(new PowerImpactEcho { Kind = sentinel, Start = now, ImpactAt = now + 1f, Duration = 30f });
            GetPrivateField<List<PowerAftermathVfx>>(game, "powerAftermathVfx").Add(new PowerAftermathVfx { PowerKey = sentinel, Start = now + 1f, Duration = 30f });
            GetPrivateField<List<PowerActorPoseBeat>>(game, "powerActorPoseBeats").Add(new PowerActorPoseBeat
            {
                UnitId = sentinel,
                PowerKey = "FBL",
                Role = CombatPowerActorPoseRole.Source,
                StableSeed = 1776,
                Start = now,
                Duration = 30f
            });
            GetPrivateField<List<CombatUnitPresentationBeat>>(game, "combatUnitPresentationBeats").Add(
                new CombatUnitPresentationBeat(sentinel, CombatUnitPresentationBeatKind.Hit, now + 1f, now + 30f, 1f));
            SetPrivateField(game, "combatPowerCue", new CombatPowerIdentity("Stale boundary cue", "!", sentinel, "ff5500", 3, 30f));
            SetPrivateField(game, "combatPowerCueStarted", now);
            SetPrivateField(game, "combatPowerCueUntil", now + 30f);
            SetPrivateField(game, "combatPowerOutcomeText", "Stale boundary outcome");
            SetPrivateField(game, "combatPowerPulseUntil", now + 30f);
            SetPrivateField(game, "combatShakeMagnitude", 8f);
            SetPrivateField(game, "combatImpactFrameColor", "ff5500");
            InvokePrivate(
                game,
                "QueueSfx",
                "hit",
                0.60f,
                0.50f,
                0f,
                1f,
                CombatAudioMixRules.ScheduledSfxPrioritySupporting);
            object scheduledSfx = GetPrivateField<object>(game, "scheduledSfx");
            Assert((int)scheduledSfx.GetType().GetProperty("Count").GetValue(scheduledSfx) > 0,
                "combat boundary regression stages a delayed stale audio cue");
        }

        private static void AssertStaleCombatPresentationBoundarySentinelsPresent(AshenHallsGame game, string boundary)
        {
            const string sentinel = "stale-boundary";
            Assert(GetPrivateField<List<PowerCastAura>>(game, "powerCastAuras").Any(value => value.PowerKey == sentinel)
                && GetPrivateField<List<PowerTravelVfx>>(game, "powerTravelVfx").Any(value => value.PowerKey == sentinel)
                && GetPrivateField<List<PowerImpactEcho>>(game, "powerImpactEchoes").Any(value => value.Kind == sentinel)
                && GetPrivateField<List<PowerAftermathVfx>>(game, "powerAftermathVfx").Any(value => value.PowerKey == sentinel)
                && GetPrivateField<List<PowerActorPoseBeat>>(game, "powerActorPoseBeats").Any(value => value.UnitId == sentinel)
                && GetPrivateField<CombatPowerIdentity>(game, "combatPowerCue").Title == "Stale boundary cue",
                boundary + " preserves the current encounter presentation when adoption fails");
        }

        private static void AssertNoStaleCombatPresentationBoundarySentinels(AshenHallsGame game, string boundary)
        {
            const string sentinel = "stale-boundary";
            object scheduledSfx = GetPrivateField<object>(game, "scheduledSfx");
            Assert(!GetPrivateField<List<Tween>>(game, "tweens").Any(value => value.Id == sentinel)
                && !GetPrivateField<List<FloatText>>(game, "floatTexts").Any(value => value.Text == sentinel)
                && !GetPrivateField<List<ParticleDot>>(game, "particles").Any(value => value.Kind == sentinel)
                && !GetPrivateField<List<BeamEffect>>(game, "beams").Any(value => value.Kind == sentinel)
                && !GetPrivateField<List<CellFlash>>(game, "flashes").Any(value => value.Duration >= 30f)
                && !GetPrivateField<List<CastGlyph>>(game, "castGlyphs").Any(value => value.Kind == sentinel)
                && !GetPrivateField<List<PowerCastAura>>(game, "powerCastAuras").Any(value => value.PowerKey == sentinel)
                && !GetPrivateField<List<PowerTravelVfx>>(game, "powerTravelVfx").Any(value => value.PowerKey == sentinel)
                && !GetPrivateField<List<PowerImpactEcho>>(game, "powerImpactEchoes").Any(value => value.Kind == sentinel)
                && !GetPrivateField<List<PowerAftermathVfx>>(game, "powerAftermathVfx").Any(value => value.PowerKey == sentinel)
                && !GetPrivateField<List<PowerActorPoseBeat>>(game, "powerActorPoseBeats").Any(value => value.UnitId == sentinel)
                && !GetPrivateField<List<CombatUnitPresentationBeat>>(game, "combatUnitPresentationBeats").Any(value => value.UnitId == sentinel),
                boundary + " removes every stale legacy and authored combat visual layer");
            Assert(string.IsNullOrEmpty(GetPrivateField<CombatPowerIdentity>(game, "combatPowerCue").Title)
                && string.IsNullOrEmpty(GetPrivateField<string>(game, "combatPowerOutcomeText"))
                && Math.Abs(GetPrivateField<float>(game, "combatPowerPulseUntil")) < 0.0001f
                && Math.Abs(GetPrivateField<float>(game, "combatShakeMagnitude")) < 0.0001f
                && (int)scheduledSfx.GetType().GetProperty("Count").GetValue(scheduledSfx) == 0,
                boundary + " removes stale cue, outcome, shake, pulse, and delayed audio state");
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
                Assert(resolvingHud.Commands.All(command => !command.Selected && !command.Armed && !command.Promoted),
                    "round resolution renders no stale player intent or promotion");
                CombatHudScreen roundHud = GetPrivateField<CombatHudScreen>(game, "combatHudScreen");
                Assert(roundHud != null && roundHud.IsReady, "round transition retains the production combat HUD");
                roundHud.Refresh();
                Assert(roundHud.FocusedCommandForTest == null
                    && roundHud.ContextCommandForTest == null,
                    "round resolution clears rendered command focus before input can be submitted");
                roundHud.InvokeCommandForTest(ActionMode.Cast);
                Assert(roundState.Combat.Phase == CombatPhase.Resolving
                    && GetPrivateField<bool>(game, "combatAdvancePending")
                    && !GetPrivateField<bool>(game, "showSpellbook"),
                    "focusable disabled-command submission cannot break the round gate or open a review book");
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
            int ordinaryChoiceHash = InvokePrivate<int>(game, "DialogueChoiceRefreshHash");
            choices[0].Primary = true;
            int primaryChoiceHash = InvokePrivate<int>(game, "DialogueChoiceRefreshHash");
            Assert(primaryChoiceHash != ordinaryChoiceHash, "dialogue refresh notices when a choice becomes the primary action");
            InvokePrivate(game, "LateUpdate");
            dialogue = GetPrivateField<DialogueScreen>(game, "dialogueScreen");
            Assert(dialogue != null && dialogue.IsInteractiveAndVisible, "choice conversation owns the modal layer");
            Assert(dialogue.VisibleChoiceCountForTest == 3, "choice conversation renders each response button");
            Assert(dialogue.ChoiceFontStyleForTest(0) == FontStyle.Bold
                && dialogue.ChoiceOutlineAlphaForTest(0) >= 0.90f,
                "primary dialogue choice renders with emphasized text and outline");
            choices[0].Primary = false;
            InvokePrivate(game, "LateUpdate");
            Assert(dialogue.ChoiceFontStyleForTest(0) == FontStyle.Normal
                && dialogue.ChoiceOutlineAlphaForTest(0) < 0.90f,
                "ordinary dialogue choice clears the primary styling on refresh");
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
            choices = GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices");
            Assert(state.Gold == goldBefore && state.Supplies == suppliesBefore, "Kate's order review never spends gold");
            Assert(dialogue.VisibleChoiceCountForTest == 2
                && choices.Length == 2
                && choices[0].Id == "confirm_purchase"
                && choices[0].Primary,
                "Kate presents a clear primary confirmation and a way back");
            Assert(GetPrivateField<string>(game, "dialogueBody").IndexOf("leave with 68", StringComparison.OrdinalIgnoreCase) >= 0,
                "Kate's order review shows the exact remaining balance");

            dialogue.InvokeChoiceForTest(1);
            InvokePrivate(game, "LateUpdate");
            Assert(dialogue.VisibleChoiceCountForTest == 4 && state.Gold == goldBefore,
                "Kate's order review returns to browsing without a charge");

            dialogue.InvokeChoiceForTest(0);
            InvokePrivate(game, "LateUpdate");
            dialogue.InvokeChoiceForTest(0);
            InvokePrivate(game, "LateUpdate");
            Assert(state.Gold == goldBefore - 12, "Kate spends the advertised price only after the explicit order confirmation");
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
            InvokePrivate(game, "EnsureInventoryEquipmentLinks", false);
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
            string temporaryOriginalType = temporaryTarget.WeaponDamageType;
            string temporaryBaseType = string.IsNullOrWhiteSpace(temporaryTarget.WeaponDamageType)
                ? "physical"
                : temporaryTarget.WeaponDamageType;
            string permanentBaseName = permanentTarget.WeaponName;
            string permanentOriginalType = permanentTarget.WeaponDamageType;
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
            choices = GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices");
            string temporaryReview = GetPrivateField<string>(game, "dialogueBody");
            Assert(state.Gold == goldBefore
                && state.Inventory.Count == inventoryBefore
                && temporaryTarget.WeaponName == temporaryBaseName
                && temporaryTarget.WeaponDamageType == temporaryOriginalType
                && !state.StoryFlags.Contains(StoryFlags.MidgaardWeaponEnchanted),
                "choosing Maud's temporary affinity opens a mutation-free order review");
            Assert(dialogue.VisibleChoiceCountForTest == 2
                && choices.Length == 2
                && choices[0].Id == "confirm_purchase"
                && choices[0].Primary
                && choices[1].Id == "keep_browsing",
                "Maud's temporary review presents one clear confirmation and a way back"
                + $" (visible={dialogue.VisibleChoiceCountForTest}, ids={string.Join(",", choices.Select(choice => choice.Id))}, primary={(choices.Length > 0 && choices[0].Primary)})");
            Assert(temporaryReview.IndexOf(temporaryTarget.Name, StringComparison.OrdinalIgnoreCase) >= 0
                && temporaryReview.IndexOf(InvokePrivate<string>(game, "TrimGearName", temporaryBaseName), StringComparison.OrdinalIgnoreCase) >= 0
                && temporaryReview.IndexOf("fire", StringComparison.OrdinalIgnoreCase) >= 0
                && temporaryReview.IndexOf(WeaponEnchantmentRules.TemporaryVictories + " victories", StringComparison.OrdinalIgnoreCase) >= 0
                && temporaryReview.IndexOf(WeaponEnchantmentRules.TemporaryCost + " gold", StringComparison.OrdinalIgnoreCase) >= 0
                && temporaryReview.IndexOf((goldBefore - WeaponEnchantmentRules.TemporaryCost) + " gold", StringComparison.OrdinalIgnoreCase) >= 0,
                "Maud's temporary review names the target, affinity, duration, price, and remaining balance");

            dialogue.InvokeChoiceForTest(1);
            InvokePrivate(game, "LateUpdate");
            Assert(dialogue.VisibleChoiceCountForTest == 3
                && state.Gold == goldBefore
                && state.Inventory.Count == inventoryBefore
                && temporaryTarget.WeaponName == temporaryBaseName,
                "backing out of Maud's review returns to her services without changing the party");

            dialogue.InvokeChoiceForTest(0);
            InvokePrivate(game, "LateUpdate");
            dialogue.InvokeChoiceForTest(temporaryTargetIndex);
            InvokePrivate(game, "LateUpdate");
            dialogue.InvokeChoiceForTest(0);
            InvokePrivate(game, "LateUpdate");
            choices = GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices");
            Assert(choices.Length == 2 && choices[0].Id == "confirm_purchase",
                "returning to the temporary order reaches the same explicit review");
            dialogue.InvokeChoiceForTest(0);
            InvokePrivate(game, "LateUpdate");
            Assert(state.Gold == goldBefore - WeaponEnchantmentRules.TemporaryCost, "temporary fire spends exactly 18 gold after order confirmation");
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

            dialogue.InvokeChoiceForTest(0);
            InvokePrivate(game, "LateUpdate");
            dialogue.InvokeChoiceForTest(temporaryTargetIndex);
            InvokePrivate(game, "LateUpdate");
            dialogue.InvokeChoiceForTest(2);
            InvokePrivate(game, "LateUpdate");
            choices = GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices");
            string replacementReview = GetPrivateField<string>(game, "dialogueBody");
            Assert(dialogue.VisibleChoiceCountForTest == 2
                && choices.Length == 2
                && choices[0].Id == "confirm_purchase"
                && replacementReview.IndexOf("Replaces Fire temporary", StringComparison.OrdinalIgnoreCase) >= 0,
                "Maud's re-enchantment review keeps the current rune and both decisions visible on one page");
            dialogue.InvokeChoiceForTest(1);
            InvokePrivate(game, "LateUpdate");
            Assert(dialogue.VisibleChoiceCountForTest == 3
                && state.Gold == goldBefore - WeaponEnchantmentRules.TemporaryCost
                && temporaryItem.TemporaryEnchantmentId == "fire",
                "backing out of a replacement rune preserves the current enchantment and balance");

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
            choices = GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices");
            string permanentReview = GetPrivateField<string>(game, "dialogueBody");
            int permanentBalance = goldBefore
                - WeaponEnchantmentRules.TemporaryCost
                - WeaponEnchantmentRules.PermanentCost;
            Assert(state.Gold == goldBefore - WeaponEnchantmentRules.TemporaryCost
                && state.Inventory.Count == inventoryBefore + 1
                && permanentTarget.WeaponName == permanentBaseName
                && permanentTarget.WeaponDamageType == permanentOriginalType,
                "choosing Maud's permanent affinity opens a mutation-free order review");
            Assert(dialogue.VisibleChoiceCountForTest == 2
                && choices.Length == 2
                && choices[0].Id == "confirm_purchase"
                && choices[0].Primary,
                "Maud's permanent review requires an explicit confirmation");
            Assert(permanentReview.IndexOf(permanentTarget.Name, StringComparison.OrdinalIgnoreCase) >= 0
                && permanentReview.IndexOf(InvokePrivate<string>(game, "TrimGearName", permanentBaseName), StringComparison.OrdinalIgnoreCase) >= 0
                && permanentReview.IndexOf("ice", StringComparison.OrdinalIgnoreCase) >= 0
                && permanentReview.IndexOf("permanent", StringComparison.OrdinalIgnoreCase) >= 0
                && permanentReview.IndexOf(WeaponEnchantmentRules.PermanentCost + " gold", StringComparison.OrdinalIgnoreCase) >= 0
                && permanentReview.IndexOf(permanentBalance + " gold", StringComparison.OrdinalIgnoreCase) >= 0,
                "Maud's permanent review names the target, affinity, duration, price, and remaining balance");

            dialogue.InvokeChoiceForTest(0);
            InvokePrivate(game, "LateUpdate");
            Assert(state.Gold == goldBefore
                - WeaponEnchantmentRules.TemporaryCost
                - WeaponEnchantmentRules.PermanentCost, "permanent ice spends exactly 90 additional gold after order confirmation");
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
            choices = GetPrivateField<DialogueChoiceView[]>(game, "dialogueChoices");
            InventoryItem quotedWeapon = label == "Tessa"
                ? GetPrivateField<InventoryItem>(game, "tessaQuotedWeapon")
                : null;
            PartyMember quotedLead = label == "Tessa" && state.Party != null && state.Party.Count > 0
                ? state.Party[0]
                : null;
            Assert(state.Gold == goldBefore, label + " order review does not spend gold");
            Assert(dialogue.VisibleChoiceCountForTest == 2
                && choices.Length == 2
                && choices[0].Id == "confirm_purchase"
                && choices[0].Primary,
                label + " order review presents one clear primary confirmation and a way back");
            Assert(GetPrivateField<string>(game, "dialogueBody").IndexOf((goldBefore - price).ToString(), StringComparison.Ordinal) >= 0,
                label + " order review states the remaining balance");

            dialogue.InvokeChoiceForTest(1);
            InvokePrivate(game, "LateUpdate");
            Assert(dialogue.VisibleChoiceCountForTest == 3 && state.Gold == goldBefore,
                label + " order review returns to browsing without a charge");

            dialogue.InvokeChoiceForTest(0);
            InvokePrivate(game, "LateUpdate");
            dialogue.InvokeChoiceForTest(0);
            InvokePrivate(game, "LateUpdate");
            Assert(state.Gold == goldBefore - price, label + " spends the exact advertised price only after explicit confirmation");
            Assert(state.StoryFlags != null && state.StoryFlags.Contains(completionFlag), label + " records its one-time completion flag");
            if (quotedWeapon != null)
            {
                Assert(state.Inventory != null && state.Inventory.Contains(quotedWeapon),
                    "Tessa delivers the exact weapon shown in her order review");
                Assert(quotedLead != null && string.Equals(quotedWeapon.EquippedById, quotedLead.Id, StringComparison.Ordinal),
                    "Tessa equips the reviewed weapon on the lead adventurer named in the offer");
            }

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
            Point midgaardOrigin = new Point(state.Map.StartX, state.Map.StartY);
            MapObject grandHearthDoor = MidgaardInteriorRules.FindById(state.Map, MidgaardInteriorRules.GrandHearthDoorId);
            if (grandHearthDoor != null
                && MidgaardInteriorRules.TryFindArrival(state.Map, grandHearthDoor, out Point streetLanding))
            {
                midgaardOrigin = streetLanding;
            }
            bool[,] reachable = ExplorationTraversalRules.ReachableMask(state.Map, midgaardOrigin.X, midgaardOrigin.Y);

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
            HashSet<int> setpieceIndices = new HashSet<int>();
            foreach (WorldMapSite site in sites)
            {
                MapObject landmark = state.Map.FindObjectById("regional-site:" + site.Id);
                Assert(landmark != null && landmark.Type == site.Type, $"{site.Name} has its stable authored landmark");
                Assert(ExplorationTraversalRules.CanReachObject(reachable, state.Map, landmark), $"{site.Name} has a reachable interaction approach");
                ExplorationCellRole roles = ExplorationSurfaceRules.RolesAt(state.Map, site.X, site.Y);
                Assert((roles & (ExplorationCellRole.Room | ExplorationCellRole.Clearing)) == (ExplorationCellRole.Room | ExplorationCellRole.Clearing), $"{site.Name} owns a room-sized clearing");
                Assert(!MidgaardInteriorRules.IsReservedCell(state.Map, site.X, site.Y), $"{site.Name} stays clear of Midgaard's embedded rooms");
                Assert(InvokePrivate<string>(game, "ObjectName", landmark) == site.Name, $"{site.Name} publishes its authored map identity");
                int setpieceIndex = WorldAreaSetpiecePresentationRules.IconIndex(site.Id);
                Assert(setpieceIndex >= 0 && setpieceIndex < WorldAreaSetpiecePresentationRules.CellCount, $"{site.Name} resolves a valid authored set-piece cell");
                Assert(setpieceIndices.Add(setpieceIndex), $"{site.Name} owns a distinct authored set-piece cell");
            }
            Assert(setpieceIndices.Count == WorldAreaSetpiecePresentationRules.CellCount, "runtime regional sites cover all eight set-piece cells exactly once");
        }

        private static void AssertRegionalSiteInteractionsRuntime(AshenHallsGame game, GameState state)
        {
            List<string> originalFlags = state.StoryFlags;
            List<InventoryItem> originalInventory = state.Inventory;
            List<LogEntry> originalLog = state.Log;
            int originalGold = state.Gold;
            int originalSupplies = state.Supplies;
            int originalElixirs = state.Elixirs;
            int partyCount = state.Party?.Count ?? 0;
            int[] originalSkillPoints = new int[partyCount];
            int[] originalHp = new int[partyCount];
            int[] originalMana = new int[partyCount];
            for (int i = 0; i < partyCount; i++)
            {
                PartyMember member = state.Party[i];
                if (member == null) continue;
                originalSkillPoints[i] = member.SkillPoints;
                originalHp[i] = member.Hp;
                originalMana[i] = member.Mana;
            }

            try
            {
                state.StoryFlags = new List<string>(originalFlags ?? Enumerable.Empty<string>());
                state.Inventory = new List<InventoryItem>(originalInventory ?? Enumerable.Empty<InventoryItem>());
                state.Log = new List<LogEntry>();
                WorldMapSite[] sites = WorldMapGenerationRules.RegionalSites(
                    state.Map.Width,
                    state.Map.Height,
                    state.Map.StartX,
                    state.Map.StartY);
                Assert(sites.Length == WorldSiteInteractionRules.All.Count, "runtime regional interactions cover every authored site");

                foreach (WorldMapSite site in sites)
                {
                    Assert(WorldSiteInteractionRules.TryGet(site.Id, out WorldSiteInteractionProfile profile), site.Name + " resolves runtime interaction metadata");
                    MapObject landmark = state.Map.FindObjectById(WorldSitePresentationRules.LandmarkObjectIdPrefix + site.Id);
                    Assert(landmark != null, site.Name + " has a runtime interaction landmark");

                    string rewardFlag = WorldSiteInteractionRules.RewardFlag(state.Depth, site.Id);
                    state.StoryFlags.RemoveAll(flag => string.Equals(flag, rewardFlag, StringComparison.Ordinal));
                    string sanitizedSiteId = InvokePrivate<string>(game, "SanitizeFlagPart", site.Id);
                    string legacyChartFlag = "regional_site_" + Math.Max(1, state.Depth) + "_" + sanitizedSiteId + "_charted";
                    string legacyScaffoldFlag = InvokePrivate<string>(game, "RouteScaffoldFlag", landmark);
                    if (!state.StoryFlags.Contains(legacyChartFlag)) state.StoryFlags.Add(legacyChartFlag);
                    if (!string.IsNullOrEmpty(legacyScaffoldFlag) && !state.StoryFlags.Contains(legacyScaffoldFlag))
                    {
                        state.StoryFlags.Add(legacyScaffoldFlag);
                    }
                    Assert(
                        !WorldSiteInteractionRules.RewardClaimed(state.StoryFlags, state.Depth, site.Id),
                        site.Name + " treats v2.3 chart/scaffold flags as history rather than a claimed reward");
                    Assert(
                        InvokePrivate<string>(game, "ExploreContextVerb", landmark, 0, 0) == profile.ReadyVerb,
                        site.Name + " exposes its reward-ready action in the live explore prompt");
                    string readyHint = InvokePrivate<string>(game, "ObjectHint", landmark);
                    Assert(
                        readyHint.Contains("Reward ready")
                        && readyHint.Contains(profile.ServiceName)
                        && readyHint.Contains(profile.ReadyStatus),
                        site.Name + " exposes its ready status and service in the live object hint");

                    state.Gold = 0;
                    state.Supplies = 0;
                    state.Elixirs = 0;
                    foreach (PartyMember member in state.Party.Where(member => member != null))
                    {
                        member.Hp = member.MaxHp;
                        member.Mana = member.MaxMana;
                    }
                    int beforeReward = RegionalSiteRewardMetric(profile, state);
                    Assert(InvokePrivate<bool>(game, "TryResolveRegionalSite", landmark), site.Name + " resolves its live regional interaction");
                    int afterReward = RegionalSiteRewardMetric(profile, state);
                    Assert(afterReward > beforeReward, site.Name + " grants its first depth-scoped benefit despite legacy visit flags");
                    Assert(state.StoryFlags.Count(flag => string.Equals(flag, rewardFlag, StringComparison.Ordinal)) == 1, site.Name + " records exactly one new reward flag");
                    Assert(
                        InvokePrivate<string>(game, "ExploreContextVerb", landmark, 0, 0) == profile.RepeatVerb,
                        site.Name + " changes its live explore action after the reward is claimed");
                    string repeatHint = InvokePrivate<string>(game, "ObjectHint", landmark);
                    Assert(
                        repeatHint.Contains("Repeat service")
                        && repeatHint.Contains(profile.ServiceName)
                        && repeatHint.Contains(profile.ClaimedStatus),
                        site.Name + " exposes its claimed status and repeat service in the live object hint");

                    Assert(InvokePrivate<bool>(game, "TryResolveRegionalSite", landmark), site.Name + " remains safely reusable after its first benefit");
                    int afterRepeat = RegionalSiteRewardMetric(profile, state);
                    Assert(afterRepeat == afterReward, site.Name + " repeat service cannot duplicate its first reward");
                    Assert(state.StoryFlags.Count(flag => string.Equals(flag, rewardFlag, StringComparison.Ordinal)) == 1, site.Name + " repeat service does not duplicate reward state");
                    InvokePrivate(game, "CloseTransientOverlays");
                }
            }
            finally
            {
                state.StoryFlags = originalFlags;
                state.Inventory = originalInventory;
                state.Log = originalLog;
                state.Gold = originalGold;
                state.Supplies = originalSupplies;
                state.Elixirs = originalElixirs;
                for (int i = 0; i < partyCount; i++)
                {
                    PartyMember member = state.Party[i];
                    if (member == null) continue;
                    member.SkillPoints = originalSkillPoints[i];
                    member.Hp = originalHp[i];
                    member.Mana = originalMana[i];
                }
                InvokePrivate(game, "CloseTransientOverlays");
            }
        }

        private static int RegionalSiteRewardMetric(
            WorldSiteInteractionProfile profile,
            GameState state)
        {
            switch (profile.RewardKind)
            {
                case WorldSiteRewardKind.TrainingInsight:
                case WorldSiteRewardKind.GlassFormula:
                    return state.Party == null ? 0 : state.Party.Where(member => member != null).Sum(member => member.SkillPoints);
                case WorldSiteRewardKind.QuarryMail:
                    return state.Inventory?.Count ?? 0;
                case WorldSiteRewardKind.CryptTithe:
                    return state.Gold;
                case WorldSiteRewardKind.MarketCache:
                case WorldSiteRewardKind.CisternStores:
                    return state.Supplies;
                case WorldSiteRewardKind.SealEmber:
                case WorldSiteRewardKind.GroveTonic:
                    return state.Elixirs;
                default:
                    return 0;
            }
        }

        private static void AssertRegionalSiteAudioRuntime(
            AshenHallsGame game,
            GameState state,
            Dictionary<string, AudioClip> soundClips)
        {
            int originalPlayerX = state.PlayerX;
            int originalPlayerY = state.PlayerY;
            GameMode originalMode = state.Mode;
            bool originalWideView = GetPrivateField<bool>(game, "exploreWideView");
            List<RoamingThreat> originalThreats = state.RoamingThreats;
            try
            {
                state.Mode = GameMode.Explore;
                SetPrivateField(game, "exploreWideView", false);
                state.RoamingThreats = new List<RoamingThreat>();
                InvokePrivate(game, "ResetExplorationMusicPresentationState");
                AudioClip huntedMusic = InvokePrivate<AudioClip>(game, "MusicClipForKey", MusicDirectorRules.HuntedRoad);
                Assert(huntedMusic != null, "regional-site pursuit coverage resolves the hunted-road score");

                foreach (WorldMapSite site in WorldMapGenerationRules.RegionalSites(
                    state.Map.Width,
                    state.Map.Height,
                    state.Map.StartX,
                    state.Map.StartY))
                {
                    Assert(WorldSitePresentationRules.TryGet(site.Id, out WorldSitePresentationProfile profile), site.Name + " resolves runtime presentation data");
                    MapObject landmark = state.Map.FindObjectById(
                        WorldSitePresentationRules.LandmarkObjectIdPrefix + site.Id);
                    Assert(landmark != null && landmark.Type == profile.LandmarkType, site.Name + " runtime landmark matches its audio profile");
                    Assert(soundClips.ContainsKey(profile.PrimaryAmbientCue), site.Name + " primary ambience exists in the live SFX bank");
                    Assert(soundClips.ContainsKey(profile.SecondaryAmbientCue), site.Name + " secondary ambience exists in the live SFX bank");
                    Assert(soundClips.ContainsKey(profile.InspectCue), site.Name + " inspect cue exists in the live SFX bank");

                    InvokePrivate(game, "ResetExplorationMusicPresentationState");
                    state.PlayerX = landmark.X;
                    state.PlayerY = landmark.Y;
                    string centerAmbience = InvokePrivate<string>(game, "CurrentExplorationAmbientCue");
                    Assert(profile.UsesAmbientCue(centerAmbience), site.Name + " center resolves its authored ambience fingerprint");
                    AudioClip expectedMusic = InvokePrivate<AudioClip>(game, "MusicClipForKey", profile.MusicKey);
                    AudioClip calmMusic = InvokePrivate<AudioClip>(game, "DesiredMusicClip");
                    Assert(expectedMusic != null && calmMusic == expectedMusic, site.Name + " center resolves its authored calm score");

                    string decorationPrefix = WorldSitePresentationRules.DecorationObjectIdPrefix + site.Id + ":";
                    List<MapObject> audioDecorations = state.Map.Objects
                        .Where(obj => obj != null
                            && !string.IsNullOrEmpty(obj.Id)
                            && obj.Id.StartsWith(decorationPrefix, StringComparison.Ordinal)
                            && (GameAudioCueRules.IsAmbientLandmark(obj.Type)
                                || MusicDirectorRules.IsMusicLandmark(obj.Type)))
                        .ToList();
                    Assert(audioDecorations.Count > 0, site.Name + " runtime template includes an audio-relevant decorative prop");
                    foreach (MapObject decoration in audioDecorations)
                    {
                        state.PlayerX = decoration.X;
                        state.PlayerY = decoration.Y;
                        string decorationAmbience = InvokePrivate<string>(game, "CurrentExplorationAmbientCue");
                        Assert(profile.UsesAmbientCue(decorationAmbience), site.Name + " decorative " + decoration.Type + " cannot hijack its parent ambience");
                        int centerDistance = Math.Abs(decoration.X - landmark.X) + Math.Abs(decoration.Y - landmark.Y);
                        if (centerDistance <= 3)
                        {
                            AudioClip decorationMusic = InvokePrivate<AudioClip>(game, "DesiredMusicClip");
                            Assert(decorationMusic == expectedMusic, site.Name + " decorative " + decoration.Type + " cannot hijack its parent score");
                        }
                    }

                    state.PlayerX = landmark.X;
                    state.PlayerY = landmark.Y;
                    state.RoamingThreats.Add(new RoamingThreat
                    {
                        Id = "regional-site-audio-threat",
                        Depth = state.Depth,
                        X = landmark.X,
                        Y = landmark.Y,
                        Active = true,
                        Alerted = true
                    });
                    AudioClip threatenedMusic = InvokePrivate<AudioClip>(game, "DesiredMusicClip");
                    Assert(threatenedMusic == huntedMusic, site.Name + " yields to alerted-patrol music at runtime");
                    state.RoamingThreats.Clear();
                    InvokePrivate(game, "ResetExplorationMusicPresentationState");
                }
            }
            finally
            {
                state.PlayerX = originalPlayerX;
                state.PlayerY = originalPlayerY;
                state.Mode = originalMode;
                SetPrivateField(game, "exploreWideView", originalWideView);
                state.RoamingThreats = originalThreats;
                InvokePrivate(game, "ResetExplorationMusicPresentationState");
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
            int chartProbeTile = state.Map.Tiles[chartProbe.Y * state.Map.Width + chartProbe.X];
            int terrainRevision = state.Map.TerrainPresentationRevision;
            InvokePrivate(game, "SetTile", state.Map, chartProbe.X, chartProbe.Y, 0);
            IReadOnlyList<Point> blockedWaypointPlan = InvokePrivate<IReadOnlyList<Point>>(game, "CurrentExploreGuidancePath");
            Assert(state.Map.TerrainPresentationRevision != terrainRevision
                && blockedWaypointPlan.Count == 0
                && InvokePrivate<bool>(game, "CurrentExploreGuidanceIsBlocked"),
                "authoritative same-map topology edits invalidate terrain, the marked path cache, and shared guidance");
            int blockedRevision = state.Map.TerrainPresentationRevision;
            InvokePrivate(game, "SetTile", state.Map, chartProbe.X, chartProbe.Y, chartProbeTile);
            IReadOnlyList<Point> restoredWaypointPlan = InvokePrivate<IReadOnlyList<Point>>(game, "CurrentExploreGuidancePath");
            Assert(state.Map.TerrainPresentationRevision != blockedRevision
                && restoredWaypointPlan.Count == 2
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
            Assert(portals.Count == 10, "Midgaard contains five paired interior doorways");
            Assert(portals.Select(portal => portal.Id).Distinct(StringComparer.Ordinal).Count() == portals.Count, "interior doorway identities are unique");
            Assert(MidgaardInteriorRules.BrokenPortalIds(state.Map).Count == 0, "every Midgaard interior doorway has a valid return target");
            Assert(state.Map.Objects.Count(obj => obj != null && obj.Type == ObjectType.KingHalvard) == 1, "throne room contains exactly one King Halvard NPC");
            Assert(state.Map.Objects.Count(obj => obj != null && obj.Type == ObjectType.ArmorerNpc) == 1, "merchant hall contains exactly one armorer");
            Assert(state.Map.Objects.Count(obj => obj != null && obj.Type == ObjectType.WeaponMerchantNpc) == 1, "merchant hall contains exactly one weaponsmith");
            Assert(state.Map.Objects.Count(obj => obj != null && obj.Type == ObjectType.EnchanterNpc) == 1, "merchant hall contains exactly one runesmith");
            Assert(state.Map.Objects.Count(obj => obj != null && obj.Id == MidgaardInteriorRules.GrandHearthFireId) == 1, "Grand Hearth contains exactly one named fireplace");
            Assert(state.Map.Objects.Count(obj => obj != null && obj.Type == ObjectType.TavernKeeper) == 1, "Grand Hearth contains exactly one keeper");
            Assert(state.Map.Objects.Count(obj => obj != null && obj.Type == ObjectType.OldRoadScout) == 1, "Grand Hearth contains exactly one Old Road scout");

            bool[,] exteriorReachable = ExplorationTraversalRules.ReachableMask(state.Map, state.PlayerX, state.PlayerY);
            RectInt throneBounds = MidgaardInteriorRules.ThroneRoomBounds(state.Map);
            RectInt merchantBounds = MidgaardInteriorRules.MerchantHallBounds(state.Map);
            RectInt grandHearthBounds = MidgaardInteriorRules.GrandHearthBounds(state.Map);
            Assert(grandHearthBounds.width == 10 && grandHearthBounds.height == 9,
                "Town Hall uses the safe expanded 10x9 southwest reservation");
            int grandHearthOpenFloor = 0;
            for (int y = grandHearthBounds.yMin + 1; y < grandHearthBounds.yMax - 1; y++)
            for (int x = grandHearthBounds.xMin + 1; x < grandHearthBounds.xMax - 1; x++)
            {
                if (state.Map.Tiles[y * state.Map.Width + x] == 1) grandHearthOpenFloor++;
            }
            Assert(grandHearthOpenFloor == 56,
                "Town Hall exposes one continuous broad 8x7 gathering floor");
            for (int y = 0; y < state.Map.Height; y++)
            for (int x = 0; x < state.Map.Width; x++)
            {
                if (!throneBounds.Contains(new Vector2Int(x, y))
                    && !merchantBounds.Contains(new Vector2Int(x, y))
                    && !grandHearthBounds.Contains(new Vector2Int(x, y)))
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
            MapObject grandHearthDoor = MidgaardInteriorRules.FindById(state.Map, MidgaardInteriorRules.GrandHearthDoorId);
            MapObject grandHearthExit = MidgaardInteriorRules.FindById(state.Map, MidgaardInteriorRules.GrandHearthExitId);
            MapObject grandHearthFire = MidgaardInteriorRules.FindById(state.Map, MidgaardInteriorRules.GrandHearthFireId);
            MapObject grandHearthRegister = MidgaardInteriorRules.FindById(state.Map, MidgaardInteriorRules.GrandHearthRegisterId);
            MapObject grandHearthBanner = MidgaardInteriorRules.FindById(state.Map, MidgaardInteriorRules.GrandHearthBannerId);
            MapObject grandHearthWindow = MidgaardInteriorRules.FindById(state.Map, MidgaardInteriorRules.GrandHearthWindowId);
            MapObject grandHearthCargo = MidgaardInteriorRules.FindById(state.Map, MidgaardInteriorRules.GrandHearthCargoId);
            MapObject grandHearthShelves = MidgaardInteriorRules.FindById(state.Map, MidgaardInteriorRules.GrandHearthShelvesId);
            MapObject grandHearthMapTable = MidgaardInteriorRules.FindById(state.Map, MidgaardInteriorRules.GrandHearthMapTableId);
            MapObject grandHearthRoadChest = MidgaardInteriorRules.FindById(state.Map, MidgaardInteriorRules.GrandHearthRoadChestId);
            MapObject grandHearthKeeper = state.Map.Objects.Single(obj => obj != null && obj.Type == ObjectType.TavernKeeper);
            MapObject grandHearthScout = state.Map.Objects.Single(obj => obj != null && obj.Type == ObjectType.OldRoadScout);
            MapObject grandHearthScholar = state.Map.Objects.Single(obj => obj != null && obj.Type == ObjectType.Scholar);
            Assert(InvokePrivate<int>(game, "MidgaardInteriorTileAtlasIndex", throne.X, throne.Y, 1) == 2, "royal throne uses its authored dais terrain");
            Assert(InvokePrivate<int>(game, "MidgaardInteriorTileAtlasIndex", throneExit.X, throneExit.Y, 1) == 15, "throne-room doorway uses its royal threshold");
            Assert(InvokePrivate<int>(game, "MidgaardInteriorTileAtlasIndex", armorer.X, armorer.Y + 1, 1) == 17, "armorer bay uses its forge floor");
            Assert(InvokePrivate<int>(game, "MidgaardInteriorTileAtlasIndex", enchanter.X, enchanter.Y + 1, 1) == 18, "runesmith bay uses its enchantment floor");
            Assert(InvokePrivate<int>(game, "MidgaardInteriorTileAtlasIndex", armorerExit.X, armorerExit.Y, 1) == 16, "merchant doorway uses its shop threshold");
            Point grandHearthSpawn = MidgaardInteriorRules.GrandHearthSpawn(state.Map);
            Assert(grandHearthDoor != null && grandHearthExit != null
                && grandHearthDoor.TargetId == grandHearthExit.Id
                && grandHearthExit.TargetId == grandHearthDoor.Id,
                "Grand Hearth storm doors form a permanent two-way portal pair");
            Assert(InvokePrivate<int>(game, "MidgaardTownObjectIconIndexFor", ObjectType.Tavern, grandHearthDoor) == 11,
                "Town Hall exterior keeps Tavern behavior while using the civic-hall silhouette");
            Assert(grandHearthFire != null && grandHearthBounds.Contains(new Vector2Int(grandHearthFire.X, grandHearthFire.Y)), "Grand Hearth fireplace remains inside the authored room");
            MapObject[] grandHearthSetpieces =
            {
                grandHearthFire,
                grandHearthExit,
                grandHearthRegister,
                grandHearthBanner,
                grandHearthWindow,
                grandHearthCargo,
                grandHearthShelves
            };
            int[] grandHearthSetpieceCells = { 0, 1, 2, 3, 4, 5, 5 };
            for (int index = 0; index < grandHearthSetpieces.Length; index++)
            {
                MapObject setpiece = grandHearthSetpieces[index];
                Assert(setpiece != null && grandHearthBounds.Contains(new Vector2Int(setpiece.X, setpiece.Y)),
                    "Grand Hearth set-piece " + grandHearthSetpieceCells[index] + " remains inside the authored room");
                Assert(GrandHearthArtCatalog.SetpieceIndex(setpiece.Id) == grandHearthSetpieceCells[index],
                    setpiece.Id + " resolves its pure Grand Hearth set-piece mapping");
                Assert(InvokePrivate<int>(game, "GrandHearthSetpieceAtlasIndex", setpiece) == grandHearthSetpieceCells[index],
                    setpiece.Id + " reaches Grand Hearth set-piece cell " + grandHearthSetpieceCells[index] + " through the live draw adapter");
            }
            Texture2D liveGrandHearthSetpieceAtlas = GetPrivateField<Texture2D>(game, "grandHearthSetpieceAtlas");
            bool originalExploreWideView = GetPrivateField<bool>(game, "exploreWideView");
            Rect grandHearthProbeCell = new Rect(0f, 0f, 100f, 100f);
            SetPrivateField(game, "exploreWideView", false);
            Rect grandHearthFireArt = InvokePrivate<Rect>(game, "ExploreObjectRect", grandHearthProbeCell, grandHearthFire);
            Rect grandHearthDoorArt = InvokePrivate<Rect>(game, "ExploreObjectRect", grandHearthProbeCell, grandHearthExit);
            Assert(grandHearthFireArt.width >= 158f && grandHearthFireArt.width <= 162f
                && grandHearthFireArt.height >= 160f && grandHearthFireArt.height <= 164f,
                "dedicated Grand Hearth art receives its monumental Local Map footprint");
            Assert(grandHearthDoorArt.width >= 118f && grandHearthDoorArt.width <= 122f
                && grandHearthDoorArt.height >= 148f && grandHearthDoorArt.height <= 152f,
                "dedicated storm-door art receives its tall Local Map footprint");
            SetPrivateField<Texture2D>(game, "grandHearthSetpieceAtlas", null);
            Rect fallbackGrandHearthFireArt = InvokePrivate<Rect>(game, "ExploreObjectRect", grandHearthProbeCell, grandHearthFire);
            Rect fallbackGrandHearthDoorArt = InvokePrivate<Rect>(game, "ExploreObjectRect", grandHearthProbeCell, grandHearthExit);
            Assert(fallbackGrandHearthFireArt.width >= 98f && fallbackGrandHearthFireArt.width <= 102f
                && fallbackGrandHearthFireArt.height >= 98f && fallbackGrandHearthFireArt.height <= 102f,
                "missing Grand Hearth atlas restores the bounded legacy fireplace footprint");
            Assert(fallbackGrandHearthDoorArt.width >= 76f && fallbackGrandHearthDoorArt.width <= 80f
                && fallbackGrandHearthDoorArt.height >= 76f && fallbackGrandHearthDoorArt.height <= 80f,
                "missing Grand Hearth atlas restores the bounded legacy storm-door footprint");
            SetPrivateField(game, "grandHearthSetpieceAtlas", liveGrandHearthSetpieceAtlas);
            SetPrivateField(game, "exploreWideView", true);
            Rect wideGrandHearthFireArt = InvokePrivate<Rect>(game, "ExploreObjectRect", grandHearthProbeCell, grandHearthFire);
            Rect wideGrandHearthDoorArt = InvokePrivate<Rect>(game, "ExploreObjectRect", grandHearthProbeCell, grandHearthExit);
            Assert(wideGrandHearthFireArt.width >= 146f && wideGrandHearthFireArt.width <= 150f
                && wideGrandHearthDoorArt.width >= 110f && wideGrandHearthDoorArt.width <= 114f,
                "Region Map keeps both dedicated Grand Hearth set-pieces within their authored wide-view footprints");
            SetPrivateField(game, "exploreWideView", originalExploreWideView);
            Assert(grandHearthMapTable != null && grandHearthBounds.Contains(new Vector2Int(grandHearthMapTable.X, grandHearthMapTable.Y)), "Grand Hearth keeps an Old Road map table off the tutorial lane");
            Assert(grandHearthRoadChest != null && grandHearthBounds.Contains(new Vector2Int(grandHearthRoadChest.X, grandHearthRoadChest.Y)), "Grand Hearth keeps a company road chest off the tutorial lane");
            Assert(InvokePrivate<int>(game, "MidgaardInteriorTileAtlasIndex", grandHearthExit.X, grandHearthExit.Y, 1) == 16, "Grand Hearth storm doors use the timber threshold");
            Assert(InvokePrivate<int>(game, "MidgaardInteriorTileAtlasIndex", grandHearthSpawn.X, grandHearthSpawn.Y, 1) == 1, "fresh party company mark keeps the shared-atlas runner fallback");
            Assert(InvokePrivate<int>(game, "MidgaardInteriorTileAtlasIndex", grandHearthBounds.xMin + 4, grandHearthBounds.yMin + 2, 1) == 4, "Grand Hearth surrounds its runner with warm wood floor");
            Assert(InvokePrivate<int>(game, "MidgaardInteriorPropIconIndex", ObjectType.RoyalLectern, grandHearthMapTable) == 7, "Grand Hearth map table uses the authored cartography desk");
            Assert(InvokePrivate<int>(game, "MidgaardInteriorPropIconIndex", ObjectType.ProvisionShelf, grandHearthRoadChest) == 17, "Grand Hearth road chest uses the authored blue company chest");
            Assert(InvokePrivate<string>(game, "ExploreGroundName", grandHearthSpawn.X, grandHearthSpawn.Y) == "Company runner", "Grand Hearth HUD names the starting floor as the company runner");
            Assert(GrandHearthArtCatalog.TryFloorChoice(
                    state.Map,
                    grandHearthSpawn.X,
                    grandHearthSpawn.Y,
                    1,
                    out GrandHearthFloorChoice liveSpawnFloor)
                && liveSpawnFloor.AtlasIndex == 3
                && !liveSpawnFloor.FlipX
                && !liveSpawnFloor.FlipY,
                "fresh party company mark resolves the unflipped v2.7 medallion floor");
            Assert(GrandHearthArtCatalog.TryFloorChoice(
                    state.Map,
                    grandHearthExit.X,
                    grandHearthExit.Y,
                    1,
                    out GrandHearthFloorChoice liveThresholdFloor)
                && liveThresholdFloor.AtlasIndex == 5
                && !liveThresholdFloor.FlipX
                && !liveThresholdFloor.FlipY,
                "live Town Hall exit resolves the unflipped v2.7 storm threshold floor");
            Assert(GrandHearthArtCatalog.TryFloorChoice(
                    state.Map,
                    grandHearthFire.X,
                    grandHearthFire.Y,
                    1,
                    out GrandHearthFloorChoice liveApronFloor)
                && liveApronFloor.AtlasIndex == 4
                && !liveApronFloor.FlipX
                && !liveApronFloor.FlipY,
                "live Grand Hearth fixture resolves the unflipped v2.7 hearth apron floor");
            Assert(GrandHearthArtCatalog.TryFloorChoice(
                    state.Map,
                    grandHearthBounds.xMin + 4,
                    grandHearthBounds.yMin + 2,
                    1,
                    out GrandHearthFloorChoice liveTimberFloor)
                && (liveTimberFloor.AtlasIndex == 0 || liveTimberFloor.AtlasIndex == 1),
                "live Town Hall open floor resolves one deterministic v2.7 hearthwood variant");

            Assert(MidgaardInteriorRules.GrandHearthPatrons.Count == 6,
                "Town Hall gathering space authors six ambient patrons");
            HashSet<string> patronCells = new HashSet<string>(StringComparer.Ordinal);
            foreach (GrandHearthPatronPlacement placement in MidgaardInteriorRules.GrandHearthPatrons)
            {
                int patronX = grandHearthBounds.xMin + placement.OffsetX;
                int patronY = grandHearthBounds.yMin + placement.OffsetY;
                Assert(grandHearthBounds.Contains(new Vector2Int(patronX, patronY)),
                    $"Town Hall patron {placement.Profession} remains inside the gathering chamber");
                Assert(state.Map.Tiles[patronY * state.Map.Width + patronX] == 1,
                    $"Town Hall patron {placement.Profession} stands on open floor");
                Assert(!MidgaardInteriorRules.IsGrandHearthCompanyRunner(state.Map, patronX, patronY),
                    $"Town Hall patron {placement.Profession} keeps the first-step runner clear");
                Assert(state.Map.FindObjectAt(patronX, patronY) == null,
                    $"Town Hall patron {placement.Profession} cannot impersonate an interactable");
                Assert(ExplorationCharacterArtCatalog.CitizenAtlasIndex(placement.Profession) >= 0,
                    $"Town Hall patron {placement.Profession} resolves approved citizen art");
                Assert(MidgaardInteriorRules.TryGrandHearthPatron(
                        state.Map,
                        patronX,
                        patronY,
                        out AmbientCitizenProfession resolvedProfession)
                    && resolvedProfession == placement.Profession,
                    $"Town Hall patron {placement.Profession} resolves deterministically");
                Assert(patronCells.Add(patronX + "," + patronY),
                    $"Town Hall patron cell {patronX},{patronY} is unique");
            }
            Assert(state.Map.Objects.Count(obj => obj != null
                    && obj.Type == ObjectType.InteriorDoor
                    && grandHearthBounds.Contains(new Vector2Int(obj.X, obj.Y))) == 1,
                "Town Hall has exactly one outbound journey door");

            foreach (ObjectType type in new[]
            {
                ObjectType.KingHalvard,
                ObjectType.ArmorerNpc,
                ObjectType.WeaponMerchantNpc,
                ObjectType.EnchanterNpc,
                ObjectType.TavernKeeper,
                ObjectType.OldRoadScout,
                ObjectType.Scholar
            })
            {
                MapObject npc = state.Map.Objects.Single(obj => obj != null && obj.Type == type);
                WorldZone zone = InvokePrivate<WorldZone>(game, "ZoneFor", npc.X, npc.Y, state.Map, state.Depth);
                Assert(zone != null && zone.Danger == 0, type + " stands in a safe interior zone");
            }

            bool[,] grandHearthReachable = ExplorationTraversalRules.ReachableMask(state.Map, grandHearthSpawn.X, grandHearthSpawn.Y);
            for (int y = 0; y < state.Map.Height; y++)
            for (int x = 0; x < state.Map.Width; x++)
            {
                if (!grandHearthReachable[x, y]) continue;
                Assert(grandHearthBounds.Contains(new Vector2Int(x, y)), $"Grand Hearth flood fill cannot escape through cell {x},{y}");
            }
            Assert(ExplorationTraversalRules.CanReachObject(grandHearthReachable, state.Map, grandHearthExit), "Grand Hearth storm doors are reachable from the company mark");
            Assert(ExplorationTraversalRules.CanReachObject(grandHearthReachable, state.Map, grandHearthFire), "Grand Hearth fire is reachable from the company mark");
            Assert(ExplorationTraversalRules.CanReachObject(grandHearthReachable, state.Map, grandHearthKeeper), "Orren is reachable inside the Grand Hearth");
            Assert(ExplorationTraversalRules.CanReachObject(grandHearthReachable, state.Map, grandHearthScout), "Yara is reachable inside the Grand Hearth");
            Assert(ExplorationTraversalRules.CanReachObject(grandHearthReachable, state.Map, grandHearthScholar), "the scholar is reachable inside the Grand Hearth");

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

        private static void AssertSignatureItemMigrationRuntime(AshenHallsGame game)
        {
            string[] expectedIds =
            {
                SignatureItemCatalog.UnfathomableSwordId,
                SignatureItemCatalog.SluicekeeperBladeId,
                SignatureItemCatalog.StormglassConductorId,
                SignatureItemCatalog.RatcatcherRoadcoatId,
                SignatureItemCatalog.GloamReliquaryMailId,
                SignatureItemCatalog.MirrorweaveRoadMantleId,
                SignatureItemCatalog.CrownwardWarbladeId
            };
            InventoryItem[] liveRewards =
            {
                InvokePrivate<InventoryItem>(game, "MakeSwordOfUnfathomableDarkness"),
                ContentSetCatalog.CreateSewerSafeRoomBlade(),
                ContentSetCatalog.CreateSewerSafeRoomFocus(),
                ContentSetCatalog.CreateSewerSliceReward(),
                InvokePrivate<InventoryItem>(game, "MakeGloamReliquaryMail"),
                ContentSetCatalog.CreateAshglassRoadMantle(),
                ContentSetCatalog.CreateCrownwardEmberglassWarblade()
            };
            Assert(liveRewards.Select(item => item?.SignatureId).SequenceEqual(expectedIds),
                "all seven live reward factories emit their expected signature IDs");

            string[] legacyNames =
            {
                "Sword of Unfathomable Darkness",
                "+2 sluicekeeper fine steel broadsword",
                "+2 etched stormglass ritual staff",
                "+3 stitched rat pelt armor",
                "+4 gloamward reliquary scale mail",
                "+5 ashglass mirrorweave road mantle",
                "+6 crownward emberglass warblade"
            };
            InventoryItem[] legacyItems = liveRewards;
            for (int i = 0; i < legacyItems.Length; i++)
            {
                legacyItems[i].SignatureId = "";
                legacyItems[i].DisplayName = legacyNames[i];
            }

            InventoryItem capturedWeapon = legacyItems[2];
            Assert(WeaponEnchantmentRules.ApplyPermanent(capturedWeapon, "fire"),
                "save-v25 signature probe captures a prefixed weapon enchantment");
            capturedWeapon.EquippedById = "signature-migration-owner";
            PartyMember owner = new PartyMember
            {
                Id = capturedWeapon.EquippedById,
                Name = "Legacy Wielder",
                Stats = new Stats(8, 12, 8, 8),
                WeaponName = capturedWeapon.DisplayName
            };
            InvokePrivate(game, "ApplyInventoryItemToLoadout", capturedWeapon, owner, true);
            InventoryItem equippedLegacyArmor = legacyItems[3];
            equippedLegacyArmor.EquippedById = "signature-migration-armor-owner";
            PartyMember armorOwner = new PartyMember
            {
                Id = equippedLegacyArmor.EquippedById,
                Name = "Legacy Roadcoat Wearer",
                Stats = new Stats(10, 8, 12, 10),
                ArmorName = equippedLegacyArmor.DisplayName
            };
            InvokePrivate(game, "ApplyInventoryItemToLoadout", equippedLegacyArmor, armorOwner, false);
            InventoryItem proceduralLookalike = new InventoryItem
            {
                Mark = "salvaged",
                Material = "stormglass",
                Form = "ritual staff",
                Trait = "storm replica",
                Slot = "weapon",
                DisplayName = "+2 Stormglass Conductor Replica"
            };
            GameState migrationState = new GameState
            {
                SaveVersion = 25,
                Mode = GameMode.Tavern,
                Depth = 1,
                StoryChapter = 1,
                ActiveStory = "Signature item migration probe",
                Party = new List<PartyMember> { owner, armorOwner },
                Inventory = legacyItems.Concat(new[] { proceduralLookalike }).ToList()
            };

            GameState previousState = GetPrivateField<GameState>(game, "state");
            try
            {
                SetPrivateField(game, "state", migrationState);
                InvokePrivate(game, "EnsureWorldState", 25);

                Assert(migrationState.Inventory.Count(item =>
                        item != null && !string.IsNullOrWhiteSpace(item.SignatureId)) == expectedIds.Length
                    && legacyItems.Select(item => item.SignatureId).SequenceEqual(expectedIds)
                    && expectedIds.Distinct(StringComparer.Ordinal).Count() == expectedIds.Length,
                    "save-v25 migration assigns every exact signature ID once");
                Assert(string.IsNullOrEmpty(proceduralLookalike.SignatureId),
                    "save-v25 migration does not misclassify a similarly named procedural staff");
                Assert(Enumerable.Range(0, legacyItems.Length).All(index =>
                        ReferenceEquals(legacyItems[index], capturedWeapon)
                        || legacyItems[index].DisplayName == SignatureItemCatalog.Find(expectedIds[index]).DisplayName),
                    "save-v25 migration canonicalizes every unenchanted legacy display identity");

                Assert(capturedWeapon.PermanentEnchantmentId == "fire"
                    && capturedWeapon.EnchantmentBaseCaptured
                    && capturedWeapon.EnchantmentBaseDisplayName == "+2 Stormglass Conductor"
                    && capturedWeapon.EnchantmentBaseTrait == "storm"
                    && capturedWeapon.EnchantmentBaseDamageType == "shock"
                    && capturedWeapon.DisplayName == "flamebound +2 Stormglass Conductor"
                    && capturedWeapon.EquippedById == owner.Id
                    && owner.WeaponName == capturedWeapon.DisplayName,
                    "save-v25 migration preserves enchantment state and owner while canonicalizing its captured base identity");
                Assert(equippedLegacyArmor.SignatureId == SignatureItemCatalog.RatcatcherRoadcoatId
                    && equippedLegacyArmor.DisplayName == "+3 Ratcatcher’s Roadcoat"
                    && equippedLegacyArmor.EquippedById == armorOwner.Id
                    && armorOwner.ArmorName == equippedLegacyArmor.DisplayName,
                    "save-v25 migration preserves equipped armor ownership while canonicalizing its loadout name");

                string firstPass = JsonUtility.ToJson(migrationState);
                InvokePrivate(game, "EnsureWorldState", 25);
                Assert(JsonUtility.ToJson(migrationState) == firstPass,
                    "save-v25 signature migration is idempotent on a second normalization pass");
            }
            finally
            {
                SetPrivateField(game, "state", previousState);
            }
        }

        private static void AssertInventoryEquipmentSwapAndRangeSemantics(AshenHallsGame game)
        {
            GameState previousState = GetPrivateField<GameState>(game, "state");
            try
            {
                PartyMember arden = new PartyMember
                {
                    Id = "swap-arden",
                    Name = "Arden",
                    Role = "bow",
                    Race = "human",
                    ClassKey = "ranger",
                    Stats = new Stats(12, 10, 14, 12),
                    Skills = new SkillSet().Normalize(),
                    Level = 2,
                    Hp = 30,
                    MaxHp = 30,
                    WeaponName = "Cinder Longbow",
                    WeaponBonus = 4,
                    WeaponDamageType = "fire",
                    WeaponDamageMin = 7,
                    WeaponDamageMax = 12,
                    WeaponAttackSpeed = 11,
                    WeaponStrengthBonus = 1,
                    WeaponIntelligenceBonus = 0,
                    WeaponAgilityBonus = 2,
                    WeaponHealthBonus = 1,
                    ArmorName = "Scout leathers",
                    ArmorBonus = 1,
                    Range = 5
                };
                PartyMember brann = new PartyMember
                {
                    Id = "swap-brann",
                    Name = "Brann",
                    Role = "shield",
                    Race = "dwarf",
                    ClassKey = "warrior",
                    Stats = new Stats(15, 8, 9, 15),
                    Skills = new SkillSet().Normalize(),
                    Level = 2,
                    Hp = 34,
                    MaxHp = 34,
                    WeaponName = "Iron Broadsword",
                    WeaponBonus = 2,
                    WeaponDamageType = "physical",
                    WeaponDamageMin = 5,
                    WeaponDamageMax = 9,
                    WeaponAttackSpeed = 7,
                    WeaponStrengthBonus = 2,
                    WeaponIntelligenceBonus = 1,
                    WeaponAgilityBonus = 0,
                    WeaponHealthBonus = 2,
                    ArmorName = "Iron mail",
                    ArmorBonus = 2,
                    Range = 1
                };
                InventoryItem cinderBow = new InventoryItem
                {
                    EquippedById = arden.Id,
                    DisplayName = arden.WeaponName,
                    Form = "longbow",
                    Slot = "weapon",
                    Bonus = arden.WeaponBonus,
                    StrengthBonus = arden.WeaponStrengthBonus,
                    IntelligenceBonus = arden.WeaponIntelligenceBonus,
                    AgilityBonus = arden.WeaponAgilityBonus,
                    HealthBonus = arden.WeaponHealthBonus,
                    DamageMin = arden.WeaponDamageMin,
                    DamageMax = arden.WeaponDamageMax,
                    AttackSpeed = arden.WeaponAttackSpeed,
                    DamageType = arden.WeaponDamageType,
                    Rarity = "rare"
                };
                InventoryItem broadsword = new InventoryItem
                {
                    EquippedById = brann.Id,
                    DisplayName = brann.WeaponName,
                    Form = "broadsword",
                    Slot = "weapon",
                    Bonus = brann.WeaponBonus,
                    StrengthBonus = brann.WeaponStrengthBonus,
                    IntelligenceBonus = brann.WeaponIntelligenceBonus,
                    AgilityBonus = brann.WeaponAgilityBonus,
                    HealthBonus = brann.WeaponHealthBonus,
                    DamageMin = brann.WeaponDamageMin,
                    DamageMax = brann.WeaponDamageMax,
                    AttackSpeed = brann.WeaponAttackSpeed,
                    DamageType = brann.WeaponDamageType,
                    Rarity = "common"
                };
                GameState linkedState = new GameState
                {
                    Mode = GameMode.Explore,
                    Party = new List<PartyMember> { arden, brann },
                    Inventory = new List<InventoryItem> { cinderBow, broadsword }
                };
                SetPrivateField(game, "state", linkedState);
                InvokePrivate(game, "EnsureInventoryEquipmentLinks", false);
                Assert(InvokePrivate<bool>(game, "EquipInventoryItemToMember", cinderBow, brann, null), "equipped weapon can be reassigned through one atomic swap");
                Assert(linkedState.Inventory.Count == 2, "linked equipment swap preserves inventory membership");
                Assert(cinderBow.EquippedById == brann.Id && broadsword.EquippedById == arden.Id, "linked equipment swap exchanges both exact owner IDs");
                Assert(arden.WeaponName == broadsword.DisplayName
                    && arden.WeaponBonus == broadsword.Bonus
                    && arden.WeaponDamageMin == broadsword.DamageMin
                    && arden.WeaponDamageMax == broadsword.DamageMax
                    && arden.WeaponAttackSpeed == broadsword.AttackSpeed
                    && arden.WeaponDamageType == broadsword.DamageType
                    && arden.WeaponStrengthBonus == broadsword.StrengthBonus
                    && arden.WeaponIntelligenceBonus == broadsword.IntelligenceBonus
                    && arden.WeaponAgilityBonus == broadsword.AgilityBonus
                    && arden.WeaponHealthBonus == broadsword.HealthBonus, "swap transfers the replacement weapon's complete mechanics to the former owner");
                Assert(brann.WeaponName == cinderBow.DisplayName
                    && brann.WeaponBonus == cinderBow.Bonus
                    && brann.WeaponDamageMin == cinderBow.DamageMin
                    && brann.WeaponDamageMax == cinderBow.DamageMax
                    && brann.WeaponAttackSpeed == cinderBow.AttackSpeed
                    && brann.WeaponDamageType == cinderBow.DamageType
                    && brann.WeaponStrengthBonus == cinderBow.StrengthBonus
                    && brann.WeaponIntelligenceBonus == cinderBow.IntelligenceBonus
                    && brann.WeaponAgilityBonus == cinderBow.AgilityBonus
                    && brann.WeaponHealthBonus == cinderBow.HealthBonus, "swap transfers the selected weapon's complete mechanics to its new owner");
                Assert(arden.Range == 5 && brann.Range == 4, "swapped weapons recalculate each adventurer's effective range");

                GameState roundTrip = JsonUtility.FromJson<GameState>(JsonUtility.ToJson(linkedState));
                SetPrivateField(game, "state", roundTrip);
                InvokePrivate(game, "EnsureInventoryEquipmentLinks", false);
                InventoryItem restoredBow = roundTrip.Inventory.Single(item => item.DisplayName == cinderBow.DisplayName);
                InventoryItem restoredSword = roundTrip.Inventory.Single(item => item.DisplayName == broadsword.DisplayName);
                Assert(restoredBow.EquippedById == brann.Id && restoredSword.EquippedById == arden.Id, "equipment swap ownership survives save round-trip and link repair");

                PartyMember syntheticOwner = JsonUtility.FromJson<PartyMember>(JsonUtility.ToJson(arden));
                syntheticOwner.Id = "synthetic-owner";
                syntheticOwner.Name = "Synthetic Owner";
                syntheticOwner.WeaponName = "Cinder Longbow";
                PartyMember legacyTarget = JsonUtility.FromJson<PartyMember>(JsonUtility.ToJson(brann));
                legacyTarget.Id = "legacy-target";
                legacyTarget.Name = "Legacy Target";
                legacyTarget.WeaponName = "Iron Broadsword";
                legacyTarget.WeaponBonus = 2;
                legacyTarget.WeaponDamageMin = 5;
                legacyTarget.WeaponDamageMax = 9;
                legacyTarget.WeaponAttackSpeed = 7;
                InventoryItem syntheticBow = JsonUtility.FromJson<InventoryItem>(JsonUtility.ToJson(cinderBow));
                syntheticBow.EquippedById = syntheticOwner.Id;
                InvokePrivate(game, "ApplyInventoryItemToLoadout", syntheticBow, syntheticOwner, true);
                InvokePrivate(game, "ApplyInventoryItemToLoadout", broadsword, legacyTarget, true);
                GameState syntheticState = new GameState
                {
                    Mode = GameMode.Explore,
                    Party = new List<PartyMember> { syntheticOwner, legacyTarget },
                    Inventory = new List<InventoryItem> { syntheticBow }
                };
                SetPrivateField(game, "state", syntheticState);
                Assert(InvokePrivate<bool>(game, "EquipInventoryItemToMember", syntheticBow, legacyTarget, null), "inventory-backed gear swaps safely with a legacy loadout");
                Assert(syntheticState.Inventory.Count == 1 && syntheticBow.EquippedById == legacyTarget.Id, "legacy-loadout swap creates no duplicate inventory object");
                Assert(syntheticOwner.WeaponName == "Iron Broadsword" && legacyTarget.WeaponName == syntheticBow.DisplayName, "legacy-loadout swap preserves both equipped weapons");

                PartyMember failedOwner = JsonUtility.FromJson<PartyMember>(JsonUtility.ToJson(syntheticOwner));
                failedOwner.Id = "failed-owner";
                failedOwner.WeaponName = syntheticBow.DisplayName;
                PartyMember failedTarget = JsonUtility.FromJson<PartyMember>(JsonUtility.ToJson(legacyTarget));
                failedTarget.Id = "failed-target";
                failedTarget.WeaponName = "";
                InventoryItem failedBow = JsonUtility.FromJson<InventoryItem>(JsonUtility.ToJson(syntheticBow));
                failedBow.EquippedById = failedOwner.Id;
                InvokePrivate(game, "ApplyInventoryItemToLoadout", failedBow, failedOwner, true);
                GameState failedState = new GameState
                {
                    Mode = GameMode.Explore,
                    Party = new List<PartyMember> { failedOwner, failedTarget },
                    Inventory = new List<InventoryItem> { failedBow }
                };
                SetPrivateField(game, "state", failedState);
                string beforeFailure = JsonUtility.ToJson(failedState);
                Assert(!InvokePrivate<bool>(game, "EquipInventoryItemToMember", failedBow, failedTarget, null), "reassignment fails when the target has no same-slot replacement");
                Assert(JsonUtility.ToJson(failedState) == beforeFailure, "failed reassignment leaves every loadout and owner link unchanged");

                PartyMember ranger = new PartyMember
                {
                    Id = "range-ranger",
                    Name = "Range Ranger",
                    Role = "bow",
                    Race = "human",
                    ClassKey = "ranger",
                    Stats = new Stats(10, 10, 14, 12),
                    Skills = new SkillSet().Normalize(),
                    Level = 1,
                    Hp = 28,
                    MaxHp = 28,
                    WeaponName = "Plain Longbow",
                    WeaponBonus = 0,
                    WeaponDamageType = "physical",
                    WeaponDamageMin = 3,
                    WeaponDamageMax = 7,
                    WeaponAttackSpeed = 8,
                    ArmorName = "Scout leathers",
                    ArmorBonus = 1,
                    Range = 5
                };
                InventoryItem longbow = new InventoryItem
                {
                    DisplayName = "Balanced Longbow",
                    Form = "longbow",
                    Slot = "weapon",
                    Bonus = 0,
                    DamageMin = 3,
                    DamageMax = 7,
                    AttackSpeed = 8,
                    DamageType = "physical",
                    Rarity = "common"
                };
                GameState rangeState = new GameState
                {
                    Mode = GameMode.Explore,
                    Party = new List<PartyMember> { ranger },
                    Inventory = new List<InventoryItem> { longbow }
                };
                SetPrivateField(game, "state", rangeState);
                string rangeComparison = InvokePrivate<string>(game, "InventoryComparisonLine", longbow, ranger);
                string rangeSummary = InvokePrivate<string>(game, "CompactInventoryItemSummary", longbow);
                Assert(rangeComparison.IndexOf("RNG 5 (=)", StringComparison.Ordinal) >= 0, "full inventory comparison reports the ranger's effective range floor");
                Assert(rangeSummary.IndexOf("Range 5", StringComparison.Ordinal) >= 0, "inventory summary reports the same effective range");
                Assert(InvokePrivate<bool>(game, "EquipInventoryItemToMember", longbow, ranger, null) && ranger.Range == 5, "equipping the longbow preserves the same effective range shown before the choice");
            }
            finally
            {
                SetPrivateField(game, "state", previousState);
            }
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
