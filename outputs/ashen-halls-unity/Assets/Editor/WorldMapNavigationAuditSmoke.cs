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
