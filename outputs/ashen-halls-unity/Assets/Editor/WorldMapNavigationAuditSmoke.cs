using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class WorldMapNavigationAuditSmoke
    {
        private const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;
        private const int ViewSize = 5;
        private const float CellSize = 20f;

        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " world map navigation audit smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " world map navigation audit smoke failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            ScrollDirectionsSupportBothPointerAxes();
            LocalWheelCannotTravelButLeftClickCan();
            RegionWheelAndClicksOnlyChangeBrowseFocus();
            CancelledRegionClickDoesNotSelectItsOldCell();
            BlockedPointerGestureCannotChangeMapFocus();
            EdgeControlsPanAndRecenterWithoutTravel();
            EdgeControlsRespectBoundsAndInputOwnership();
            EdgePanMovesCameraOnFirstBoundaryClick();
            EdgeHoverDescribesControlsAndRespectsOverlays();
            PointerPansMoveViewportWithoutEdgeDelay();
            ViewportPansPreserveBoundsAndUntouchedAxes();
        }

        private static void PointerPansMoveViewportWithoutEdgeDelay()
        {
            WithFixture(true, (game, state, grid, _) =>
            {
                string before = JsonUtility.ToJson(state);
                foreach (bool drag in new[] { false, true })
                foreach (Vector2 direction in new[] { Vector2.right, Vector2.left, Vector2.up, Vector2.down })
                {
                    int startX = direction.x < 0f || direction.y < 0f ? state.Map.Width - 1 : 0;
                    int startY = direction.x < 0f || direction.y < 0f ? state.Map.Height - 1 : 0;
                    Invoke(game, "SetRegionMapFocus", startX, startY);
                    Vector2 point = grid.center;
                    Func<Point> viewport = () => RegionMapNavigationRules.ViewportOrigin(Focus(game).X, Focus(game).Y,
                        ViewSize, ViewSize, state.Map.Width, state.Map.Height);
                    Point original = viewport();
                    if (drag) Dispatch(game, grid, original, EventType.MouseDown, point, Vector2.zero);
                    Action<Vector2> pan = delta => Require(Dispatch(game, grid, viewport(),
                            drag ? EventType.MouseDrag : EventType.ScrollWheel, point,
                            drag ? -delta * CellSize : delta), "the production pointer handler consumes a Region pan");

                    pan(direction);
                    Point after = viewport();
                    Require(after.X == original.X + (int)direction.x && after.Y == original.Y + (int)direction.y,
                        "the first " + (drag ? "drag" : "wheel notch") + " moves the visible map away from a clamped edge");
                    Point focus = Focus(game);
                    Require((direction.x != 0f || focus.X == startX) && (direction.y != 0f || focus.Y == startY),
                        "pointer panning leaves the unrequested selection axis unchanged");

                    pan(-direction);
                    Point atEdge = Focus(game);
                    for (int i = 0; i < 8; i++) pan(-direction);
                    Require(Focus(game).X == atEdge.X && Focus(game).Y == atEdge.Y,
                        "overscroll cannot push the selected cell into an invisible camera dead zone");
                    pan(direction);
                    after = viewport();
                    Require(after.X == original.X + (int)direction.x && after.Y == original.Y + (int)direction.y,
                        "reversing an overscroll moves the camera on the first pointer step");
                    if (drag)
                    {
                        Point beforeRelease = Focus(game);
                        Dispatch(game, grid, after, EventType.MouseUp, point, Vector2.zero);
                        Require(!Get<bool>(game, "exploreRegionPointerDragging")
                            && Focus(game).X == beforeRelease.X && Focus(game).Y == beforeRelease.Y,
                            "releasing a pan never selects its stale pressed cell");
                    }
                    Require(JsonUtility.ToJson(state) == before,
                        "edge and reverse pointer panning never travel, discover tiles, advance time, or alter campaign state");
                }
            });
        }

        private static void ViewportPansPreserveBoundsAndUntouchedAxes()
        {
            const int mapWidth = 58;
            const int mapHeight = 46;
            foreach (Point size in new[] { new Point(1, 1), new Point(5, 5), new Point(16, 8), new Point(58, 46), new Point(90, 90) })
            foreach (int focusX in new[] { 0, mapWidth / 2, mapWidth - 1 })
            foreach (int focusY in new[] { 0, mapHeight / 2, mapHeight - 1 })
            foreach (int deltaX in new[] { -100, -1, 0, 1, 100 })
            foreach (int deltaY in new[] { -100, -1, 0, 1, 100 })
            {
                Point origin = RegionMapNavigationRules.ViewportOrigin(focusX, focusY, size.X, size.Y, mapWidth, mapHeight);
                Point next = RegionMapNavigationRules.ViewportPanFocus(focusX, focusY, deltaX, deltaY,
                    size.X, size.Y, mapWidth, mapHeight);
                Point after = RegionMapNavigationRules.ViewportOrigin(next.X, next.Y, size.X, size.Y, mapWidth, mapHeight);
                Require(after.X == Mathf.Clamp(origin.X + deltaX, 0, Mathf.Max(0, mapWidth - size.X))
                    && after.Y == Mathf.Clamp(origin.Y + deltaY, 0, Mathf.Max(0, mapHeight - size.Y)),
                    "odd, even, full-map, and oversized viewports pan by the exact bounded requested distance");
                Require((after.X != origin.X || next.X == focusX) && (after.Y != origin.Y || next.Y == focusY),
                    "blocked or untouched viewport axes retain their inspection focus");
            }
        }

        private static void EdgeHoverDescribesControlsAndRespectsOverlays()
        {
            WithFixture(true, (game, state, smallGrid, origin) =>
            {
                Rect grid = EdgeFixtureGrid(smallGrid);
                float scale = ExplorationHudScreenLayout.InterfaceScale(Screen.width, Screen.height);
                string before = JsonUtility.ToJson(state);
                foreach (ExplorationMapEdge edge in new[] { ExplorationMapEdge.North, ExplorationMapEdge.East,
                    ExplorationMapEdge.South, ExplorationMapEdge.West })
                {
                    Rect target = RegionMapNavigationRules.EdgeControlRect(grid, scale, edge, false);
                    Require(RegionMapNavigationRules.TryHitEdgeControl(target.center, grid, scale, origin,
                        ViewSize, ViewSize, state.Map.Width, state.Map.Height, state.PlayerX, state.PlayerY,
                        out ExplorationMapEdge hit, out bool party) && hit == edge && !party,
                        "control hover and input share the same exact visible rectangle");
                    Invoke(game, "UpdateExploreHoverLookAt", target.center, grid, CellSize, origin, ViewSize, ViewSize);
                    Require(Get<string>(game, "exploreHoverHeading") == RegionMapNavigationRules.EdgeControlLabel(edge, false),
                        "edge hover explains its actual pan direction");
                    Require(Get<int>(game, "exploreHoverMapX") == -1 && Get<int>(game, "exploreHoverMapY") == -1,
                        "control hover never inspects or emphasizes the terrain and objects beneath it");
                    Require(Get<string>(game, "exploreHoverLookLine").Contains("party stays in place"),
                        "control hover distinguishes map browsing from party travel");
                }
                Point farOrigin = new Point(state.PlayerX + 6, state.PlayerY - 2);
                Rect partyTarget = RegionMapNavigationRules.EdgeControlRect(grid, scale, ExplorationMapEdge.West, true);
                Invoke(game, "UpdateExploreHoverLookAt", partyTarget.center, grid, CellSize, farOrigin, ViewSize, ViewSize);
                Require(Get<string>(game, "exploreHoverHeading") == "Return to party", "off-screen party hover explains recentering");

                Rect east = RegionMapNavigationRules.EdgeControlRect(grid, scale, ExplorationMapEdge.East, false);
                foreach (string overlay in new[] { "showHelpOverlay", "showPauseMenu", "showArmory", "showDialogue" })
                {
                    Set(game, overlay, true);
                    Invoke(game, "UpdateExploreHoverLookAt", east.center, grid, CellSize, origin, ViewSize, ViewSize);
                    Require(Get<string>(game, "exploreHoverHeading") == "" && Get<string>(game, "exploreHoverLookLine") == "",
                        overlay + " prevents both control and underlying terrain hover");
                    Set(game, overlay, false);
                }
                Set(game, "suppressBoardPointerThroughFrame", Time.frameCount + 1);
                Invoke(game, "UpdateExploreHoverLookAt", east.center, grid, CellSize, origin, ViewSize, ViewSize);
                Require(Get<string>(game, "exploreHoverHeading") == "", "dismissal protection also clears stale control hover");
                Set(game, "suppressBoardPointerThroughFrame", -1);
                Invoke(game, "UpdateExploreHoverLookAt", grid.center, grid, CellSize, origin, ViewSize, ViewSize);
                Require(!Get<string>(game, "exploreHoverHeading").StartsWith("Pan "), "leaving a control cannot leave stale pan wording");
                Require(JsonUtility.ToJson(state) == before, "all hover inspection stays free of campaign mutations");
            });
        }

        private static void EdgePanMovesCameraOnFirstBoundaryClick()
        {
            WithFixture(true, (game, state, smallGrid, _) =>
            {
                Rect grid = EdgeFixtureGrid(smallGrid);
                float scale = ExplorationHudScreenLayout.InterfaceScale(Screen.width, Screen.height);
                foreach (ExplorationMapEdge edge in new[] { ExplorationMapEdge.East, ExplorationMapEdge.South,
                    ExplorationMapEdge.West, ExplorationMapEdge.North })
                {
                    state.PlayerX = edge == ExplorationMapEdge.West ? state.Map.Width - 1 : 0;
                    state.PlayerY = edge == ExplorationMapEdge.North ? state.Map.Height - 1 : 0;
                    Invoke(game, "ResetRegionMapFocusToParty");
                    Point original = RegionMapNavigationRules.ViewportOrigin(state.PlayerX, state.PlayerY,
                        ViewSize, ViewSize, state.Map.Width, state.Map.Height);
                    Require(RegionMapNavigationRules.CanPanEdge(edge, original, ViewSize, ViewSize, state.Map.Width, state.Map.Height),
                        "boundary regression begins with a genuinely available pan direction");
                    Rect target = RegionMapNavigationRules.EdgeControlRect(grid, scale, edge, false);
                    string before = JsonUtility.ToJson(state);
                    Require(Dispatch(game, grid, original, EventType.MouseDown, target.center, Vector2.zero),
                        "boundary arrow consumes its first click");
                    Point focus = Focus(game);
                    Point after = RegionMapNavigationRules.ViewportOrigin(focus.X, focus.Y,
                        ViewSize, ViewSize, state.Map.Width, state.Map.Height);
                    Point delta = RegionMapNavigationRules.EdgePanDelta(edge, ViewSize, ViewSize);
                    Require(after.X == original.X + delta.X && after.Y == original.Y + delta.Y,
                        "the first boundary arrow click pans the camera, not only an off-center selection");
                    Require(JsonUtility.ToJson(state) == before, "boundary camera panning remains a presentation-only action");
                }
            });
        }

        private static void EdgeControlsPanAndRecenterWithoutTravel()
        {
            WithFixture(true, (game, state, smallGrid, origin) =>
            {
                Rect grid = EdgeFixtureGrid(smallGrid);
                float scale = ExplorationHudScreenLayout.InterfaceScale(Screen.width, Screen.height);
                string before = JsonUtility.ToJson(state);
                foreach (ExplorationMapEdge edge in new[] { ExplorationMapEdge.North, ExplorationMapEdge.East, ExplorationMapEdge.South, ExplorationMapEdge.West })
                {
                    Invoke(game, "ResetRegionMapFocusToParty");
                    Rect hit = RegionMapNavigationRules.EdgeControlRect(grid, scale, edge, false);
                    Point delta = RegionMapNavigationRules.EdgePanDelta(edge, ViewSize, ViewSize);
                    Require(hit.width > 0f && grid.Contains(hit.center), "edge controls have a real in-map target");
                    Require(Dispatch(game, grid, origin, EventType.MouseDown, hit.center, Vector2.zero),
                        "visible " + edge + " arrow consumes its click; screen=" + Screen.width + "x" + Screen.height
                        + ", grid=" + grid + ", hit=" + hit + ", side=" + Invoke(game, "SidePanelRect")
                        + ", command=" + Invoke(game, "ExploreCommandBarRect") + ", overlay=" + Invoke(game, "CurrentUiOverlay")
                        + ", suppressed=" + Invoke(game, "IsBoardPointerSuppressed"));
                    Require(Focus(game).X == state.PlayerX + delta.X && Focus(game).Y == state.PlayerY + delta.Y,
                        "edge arrow pans browse focus in its advertised direction");
                    Require(!Get<bool>(game, "exploreRegionPointerDragging"), "edge controls never start an underlying tile gesture");
                    Dispatch(game, grid, origin, EventType.MouseUp, hit.center, Vector2.zero);
                    Require(Focus(game).X == state.PlayerX + delta.X && Focus(game).Y == state.PlayerY + delta.Y,
                        "release cannot select the old map tile under a panned button");
                }

                Point farOrigin = new Point(state.PlayerX + 6, state.PlayerY - 2);
                Invoke(game, "SetRegionMapFocus", state.PlayerX + 8, state.PlayerY);
                Require(RegionMapNavigationRules.PartyEdge(state.PlayerX, state.PlayerY, farOrigin, ViewSize, ViewSize) == ExplorationMapEdge.West,
                    "off-screen party advertises its truthful direction");
                Rect party = RegionMapNavigationRules.EdgeControlRect(grid, scale, ExplorationMapEdge.West, true);
                Dispatch(game, grid, farOrigin, EventType.MouseDown, party.center, Vector2.zero);
                Require(Focus(game).X == state.PlayerX && Focus(game).Y == state.PlayerY,
                    "clicking PARTY recenters rather than taking a map step");
                Require(JsonUtility.ToJson(state) == before, "all edge controls leave time, resources, discoveries, route marks, and save state untouched");
            });
        }

        private static void EdgeControlsRespectBoundsAndInputOwnership()
        {
            Require(!RegionMapNavigationRules.CanPanEdge(ExplorationMapEdge.West, new Point(0, 0), 5, 5, 58, 46)
                && !RegionMapNavigationRules.CanPanEdge(ExplorationMapEdge.North, new Point(0, 0), 5, 5, 58, 46)
                && !RegionMapNavigationRules.CanPanEdge(ExplorationMapEdge.East, new Point(53, 41), 5, 5, 58, 46)
                && !RegionMapNavigationRules.CanPanEdge(ExplorationMapEdge.South, new Point(53, 41), 5, 5, 58, 46),
                "map boundaries never paint or activate outward pan controls");
            Require(RegionMapNavigationRules.EdgeControlRect(new Rect(0, 0, 100, 100), 1f, ExplorationMapEdge.East, false) == Rect.zero,
                "tiny layouts cannot acquire overlapping hidden controls");
            Require(RegionMapNavigationRules.PartyEdge(5, 5, new Point(3, 3), 5, 5) == ExplorationMapEdge.None,
                "an on-screen party never receives an off-screen indicator");
            foreach (string overlay in new[] { "showHelpOverlay", "showPauseMenu", "showArmory", "showDialogue" })
            WithFixture(true, (game, state, smallGrid, origin) =>
            {
                Rect grid = EdgeFixtureGrid(smallGrid);
                Rect target = RegionMapNavigationRules.EdgeControlRect(grid,
                    ExplorationHudScreenLayout.InterfaceScale(Screen.width, Screen.height), ExplorationMapEdge.East, false);
                string before = JsonUtility.ToJson(state);
                Set(game, overlay, true);
                Require(!Dispatch(game, grid, origin, EventType.MouseDown, target.center, Vector2.zero), overlay + " owns input ahead of edge controls");
                Set(game, overlay, false);
                Set(game, "suppressBoardPointerThroughFrame", Time.frameCount + 1);
                Require(!Dispatch(game, grid, origin, EventType.MouseDown, target.center, Vector2.zero), "dismissal suppression protects edge controls too");
                Set(game, "suppressBoardPointerThroughFrame", -1);
                Set(game, "exploreWideView", false);
                Require(!(bool)Invoke(game, "TryHandleExploreRegionEdgeControl", target.center, grid, origin, ViewSize, ViewSize),
                    "Local Map cannot activate Region controls");
                Require(Focus(game).X == state.PlayerX && Focus(game).Y == state.PlayerY && JsonUtility.ToJson(state) == before,
                    "blocked and unsupported edge clicks never pan or mutate campaign state");
            });
        }

        private static void ScrollDirectionsSupportBothPointerAxes()
        {
            Point native = RegionMapNavigationRules.ScrollDelta(2f, 0f, false);
            Require(native.X == 1 && native.Y == 0, "native horizontal trackpad input pans east without Shift");
            Point diagonal = RegionMapNavigationRules.ScrollDelta(-2f, 4f, false);
            Require(diagonal.X == -1 && diagonal.Y == 1, "two-axis trackpad input pans both map axes");
            Point shifted = RegionMapNavigationRules.ScrollDelta(0f, -2f, true);
            Require(shifted.X == -1 && shifted.Y == 0, "Shift preserves horizontal mouse-wheel browsing");
            Point shiftedNative = RegionMapNavigationRules.ScrollDelta(2f, 0f, true);
            Require(shiftedNative.X == 1 && shiftedNative.Y == 0, "Shift does not swallow a native horizontal gesture");
            Point invalid = RegionMapNavigationRules.ScrollDelta(float.NaN, float.PositiveInfinity, false);
            Require(invalid.X == 0 && invalid.Y == 0, "invalid scroll deltas cannot pan the map");
        }

        private static void LocalWheelCannotTravelButLeftClickCan()
        {
            WithFixture(false, (game, state, grid, origin) =>
            {
                Vector2 neighbor = CellCenter(grid, 3, 2);
                string before = JsonUtility.ToJson(state);
                int facingX = Get<int>(game, "exploreFacingX");
                int facingY = Get<int>(game, "exploreFacingY");
                foreach (Vector2 delta in new[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right })
                {
                    Require(!Dispatch(game, grid, origin, EventType.ScrollWheel, neighbor, delta),
                        "Local Map does not consume a wheel event as a travel command");
                    Require(JsonUtility.ToJson(state) == before, "Local Map wheel input leaves party, steps, resources, discoveries, and logs unchanged");
                    Require(Get<int>(game, "exploreFacingX") == facingX && Get<int>(game, "exploreFacingY") == facingY,
                        "Local Map wheel input does not even turn the party toward the pointer");
                }

                int oldX = state.PlayerX;
                int oldY = state.PlayerY;
                int oldSteps = state.ExplorationSteps;
                Require((bool)Invoke(game, "CanStepExplore", oldX + 1, oldY), "Local pointer fixture has a traversable adjacent tile");
                bool consumed = Dispatch(game, grid, origin, EventType.MouseDown, neighbor, Vector2.zero);
                Require(consumed && state.PlayerX == oldX + 1 && state.PlayerY == oldY && state.ExplorationSteps == oldSteps + 1,
                    $"an explicit Local left click still travels exactly one tile and one exploration step; before=({oldX},{oldY})/{oldSteps}, "
                    + $"after=({state.PlayerX},{state.PlayerY})/{state.ExplorationSteps}, consumed={consumed}, overlay={Invoke(game, "CurrentUiOverlay")}, "
                    + $"suppressed={Invoke(game, "IsBoardPointerSuppressed")}, mode={state.Mode}");
            });
        }

        private static void RegionWheelAndClicksOnlyChangeBrowseFocus()
        {
            WithFixture(true, (game, state, grid, origin) =>
            {
                string before = JsonUtility.ToJson(state);
                Vector2 point = CellCenter(grid, 2, 2);
                Dispatch(game, grid, origin, EventType.ScrollWheel, point, Vector2.right);
                Require(Focus(game).X == state.PlayerX + 1 && Focus(game).Y == state.PlayerY,
                    "the actual Region pointer handler routes native horizontal wheel input to browse focus");
                Dispatch(game, grid, origin, EventType.ScrollWheel, point, Vector2.up);
                Require(Focus(game).X == state.PlayerX + 1 && Focus(game).Y == state.PlayerY + 1,
                    "vertical wheel input still pans the Region focus");
                Dispatch(game, grid, origin, EventType.ScrollWheel, point, Vector2.down, EventModifiers.Shift);
                Require(Focus(game).X == state.PlayerX && Focus(game).Y == state.PlayerY + 1,
                    "Shift-wheel pans horizontally in the actual pointer handler");

                Vector2 neighbor = CellCenter(grid, 3, 2);
                Dispatch(game, grid, origin, EventType.MouseDown, neighbor, Vector2.zero);
                Require(Focus(game).Y == state.PlayerY + 1, "Region pointer down waits for click completion before selecting");
                Dispatch(game, grid, origin, EventType.MouseUp, neighbor, Vector2.zero);
                Require(Focus(game).X == state.PlayerX + 1 && Focus(game).Y == state.PlayerY,
                    "completed Region click inspects its cell without traveling");
                Require(JsonUtility.ToJson(state) == before,
                    "Region wheel and clicks never alter campaign state, including time, resources, route marks, discoveries, and logs");
            });
        }

        private static void CancelledRegionClickDoesNotSelectItsOldCell()
        {
            WithFixture(true, (game, state, grid, origin) =>
            {
                string before = JsonUtility.ToJson(state);
                Vector2 edge = new Vector2(grid.xMax - 1f, grid.yMin + 2.5f * CellSize);
                Dispatch(game, grid, origin, EventType.MouseDown, edge, Vector2.zero);
                Dispatch(game, grid, origin, EventType.MouseUp, edge + new Vector2(2f, 0f), Vector2.zero);
                Require(Focus(game).X == state.PlayerX && Focus(game).Y == state.PlayerY,
                    "a click released outside the board cancels instead of selecting its old pressed cell");
                Require(!Get<bool>(game, "exploreRegionPointerDragging"), "cancelled click releases the Region gesture");
                Require(JsonUtility.ToJson(state) == before, "cancelling an off-board click leaves campaign state untouched");
            });
        }

        private static void BlockedPointerGestureCannotChangeMapFocus()
        {
            foreach (string overlayField in new[] { "showHelpOverlay", "showPauseMenu", "showArmory", "showDialogue" })
            {
                WithFixture(true, (game, state, grid, origin) =>
                {
                    string before = JsonUtility.ToJson(state);
                    Vector2 neighbor = CellCenter(grid, 3, 2);
                    Dispatch(game, grid, origin, EventType.MouseDown, neighbor, Vector2.zero);
                    Set(game, overlayField, true);
                    Dispatch(game, grid, origin, EventType.MouseDrag, neighbor, new Vector2(-CellSize * 2f, 0f));
                    Require(!Get<bool>(game, "exploreRegionPointerDragging"), overlayField + " revokes a pending Region gesture");
                    Set(game, overlayField, false);
                    Dispatch(game, grid, origin, EventType.MouseUp, neighbor, Vector2.zero);
                    Require(Focus(game).X == state.PlayerX && Focus(game).Y == state.PlayerY,
                        overlayField + " cannot pan the Region map or leave a stale click to commit after closing");
                    Require(JsonUtility.ToJson(state) == before, overlayField + " pointer cancellation leaves campaign state unchanged");
                });
            }
        }

        private static Rect EdgeFixtureGrid(Rect smallGrid)
        {
            ExplorationHudGeometry geometry = ExplorationHudScreenLayout.Calculate(Screen.width, Screen.height, false);
            float scale = ExplorationHudScreenLayout.InterfaceScale(Screen.width, Screen.height);
            // Batchmode's Game View can be 640x480. A fixed 400px fixture
            // overlaps its real side rail even though the original100px board
            // was clear, so route every edge through an actual free board area.
            Rect grid = new Rect(smallGrid.x, smallGrid.y,
                Mathf.Min(400f, geometry.Side.xMin - smallGrid.x - 8f * scale),
                Mathf.Min(280f, geometry.Command.yMin - smallGrid.y - 8f * scale));
            Require(grid.width >= 240f * scale && grid.height >= 180f * scale
                && !geometry.Side.Overlaps(grid) && !geometry.Command.Overlaps(grid),
                "edge fixture must fit actual board ownership; screen=" + Screen.width + "x" + Screen.height
                + ", grid=" + grid + ", side=" + geometry.Side + ", command=" + geometry.Command);
            return grid;
        }

        private static void WithFixture(bool region, Action<AshenHallsGame, GameState, Rect, Point> check)
        {
            GameObject host = new GameObject("World map navigation regression host");
            host.SetActive(false);
            host.hideFlags = HideFlags.HideAndDontSave;
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                MapData map = new MapData { Width = 58, Height = 46, Depth = 1, StartX = 29, StartY = 24 };
                for (int i = 0; i < map.Width * map.Height; i++) map.Tiles.Add(0);
                // A safe Temple Square lane without encounters, loot, services,
                // or distant floor on which roaming threats could spawn.
                for (int x = map.StartX; x <= map.StartX + 2; x++) map.Tiles[map.StartY * map.Width + x] = 1;
                GameState state = new GameState
                {
                    Mode = GameMode.Explore, Depth = 1, Seed = 9173, Map = map,
                    PlayerX = map.StartX, PlayerY = map.StartY, ExplorationSteps = 4,
                    Gold = 39, Supplies = 3, Elixirs = 2, ReducedMotion = true,
                    SfxMuted = true, MusicMuted = true
                };
                Set(game, "state", state);
                Set(game, "rng", new System.Random(state.Seed));
                Set(game, "labSaveBlocked", true);
                Set(game, "launchError", "World map navigation fixture presentation held");
                Set(game, "exploreHudCollapsed", true);
                Set(game, "exploreWideView", region);
                Set(game, "suppressBoardPointerThroughFrame", -1);
                Invoke(game, "ResetRegionMapFocusToParty");

                ExplorationHudGeometry geometry = ExplorationHudScreenLayout.Calculate(Screen.width, Screen.height, false);
                Rect grid = new Rect(32f, geometry.Top.yMax + 24f, ViewSize * CellSize, ViewSize * CellSize);
                Require(!geometry.Side.Overlaps(grid) && !geometry.Command.Overlaps(grid), "pointer fixture lies in the board, clear of HUD hit targets");
                check(game, state, grid, new Point(map.StartX - 2, map.StartY - 2));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(host);
            }
        }

        private static bool Dispatch(AshenHallsGame game, Rect grid, Point origin, EventType type, Vector2 position, Vector2 delta, EventModifiers modifiers = EventModifiers.None)
        {
            // Unity 6's native Event is scoped to OnGUI. Use the same value
            // snapshot as the live wrapper, with no native Event state.
            ExplorationMapPointerInput pointer = new ExplorationMapPointerInput(
                type, 0, position, delta, (modifiers & EventModifiers.Shift) != 0);
            return (bool)Invoke(game, "HandleExplorePointerInput", pointer, grid, CellSize, origin, ViewSize, ViewSize);
        }

        private static Vector2 CellCenter(Rect grid, int x, int y) => new Vector2(grid.x + (x + 0.5f) * CellSize, grid.y + (y + 0.5f) * CellSize);
        private static Point Focus(AshenHallsGame game) => (Point)Invoke(game, "EnsureRegionMapFocus");
        private static T Get<T>(object source, string name) => (T)source.GetType().GetField(name, PrivateInstance).GetValue(source);
        private static void Set(object source, string name, object value) => source.GetType().GetField(name, PrivateInstance).SetValue(source, value);
        private static object Invoke(object source, string name, params object[] arguments) => source.GetType().GetMethod(name, PrivateInstance).Invoke(source, arguments);
        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
