using UnityEngine;

namespace AshenHalls
{
    public readonly struct ExplorationMapPointerInput
    {
        public readonly EventType Type;
        public readonly int Button;
        public readonly Vector2 Position;
        public readonly Vector2 Delta;
        public readonly bool HorizontalScroll;

        public ExplorationMapPointerInput(EventType type, int button, Vector2 position, Vector2 delta, bool horizontalScroll)
        {
            Type = type;
            Button = button;
            Position = position;
            Delta = delta;
            HorizontalScroll = horizontalScroll;
        }
    }

    public readonly struct RegionMapNavigationStep
    {
        public readonly int DeltaX;
        public readonly int DeltaY;
        public readonly int HeldX;
        public readonly int HeldY;
        public readonly float NextRepeatAt;

        public RegionMapNavigationStep(
            int deltaX,
            int deltaY,
            int heldX,
            int heldY,
            float nextRepeatAt)
        {
            DeltaX = deltaX;
            DeltaY = deltaY;
            HeldX = heldX;
            HeldY = heldY;
            NextRepeatAt = nextRepeatAt;
        }

        public bool Moved => DeltaX != 0 || DeltaY != 0;
    }

    public readonly struct RegionMapPointerPanStep
    {
        public readonly int DeltaX;
        public readonly int DeltaY;
        public readonly float RemainderX;
        public readonly float RemainderY;

        public RegionMapPointerPanStep(
            int deltaX,
            int deltaY,
            float remainderX,
            float remainderY)
        {
            DeltaX = deltaX;
            DeltaY = deltaY;
            RemainderX = remainderX;
            RemainderY = remainderY;
        }
    }

    public static class RegionMapNavigationRules
    {
        public const float AxisThreshold = 0.60f;
        public const float InitialRepeatDelay = 0.34f;
        public const float RepeatInterval = 0.10f;
        public const float PointerDragThreshold = 7f;

        // Rendering and input share one exact rectangle: a painted edge button
        // never secretly selects the map tile beneath it.
        public static Rect EdgeControlRect(Rect grid, float scale, ExplorationMapEdge edge, bool party)
        {
            scale = Mathf.Max(0.75f, scale);
            if (grid.width < 240f * scale || grid.height < 180f * scale) return Rect.zero;
            float width = (party ? 60f : 32f) * scale;
            float height = 28f * scale;
            float inset = 6f * scale;
            switch (edge)
            {
                case ExplorationMapEdge.West: return new Rect(grid.x + inset, grid.center.y - height * 0.5f, width, height);
                case ExplorationMapEdge.East: return new Rect(grid.xMax - inset - width, grid.center.y - height * 0.5f, width, height);
                case ExplorationMapEdge.North: return new Rect(grid.center.x - width * 0.5f, grid.y + inset, width, height);
                case ExplorationMapEdge.South: return new Rect(grid.center.x - width * 0.5f, grid.yMax - inset - height, width, height);
                default: return Rect.zero;
            }
        }

        public static bool CanPanEdge(ExplorationMapEdge edge, Point origin, int viewWidth, int viewHeight, int mapWidth, int mapHeight)
        {
            if (origin == null || viewWidth <= 0 || viewHeight <= 0) return false;
            switch (edge)
            {
                case ExplorationMapEdge.West: return origin.X > 0;
                case ExplorationMapEdge.East: return origin.X + viewWidth < mapWidth;
                case ExplorationMapEdge.North: return origin.Y > 0;
                case ExplorationMapEdge.South: return origin.Y + viewHeight < mapHeight;
                default: return false;
            }
        }

        public static bool TryHitEdgeControl(Vector2 position, Rect grid, float scale, Point origin,
            int viewWidth, int viewHeight, int mapWidth, int mapHeight, int playerX, int playerY,
            out ExplorationMapEdge edge, out bool party)
        {
            edge = ExplorationMapEdge.None;
            party = false;
            if (!grid.Contains(position)) return false;
            ExplorationMapEdge partyEdge = PartyEdge(playerX, playerY, origin, viewWidth, viewHeight);
            for (int i = (int)ExplorationMapEdge.North; i <= (int)ExplorationMapEdge.West; i++)
            {
                ExplorationMapEdge candidate = (ExplorationMapEdge)i;
                if (!CanPanEdge(candidate, origin, viewWidth, viewHeight, mapWidth, mapHeight)) continue;
                bool isParty = candidate == partyEdge;
                Rect target = EdgeControlRect(grid, scale, candidate, isParty);
                if (target.width <= 0f || !target.Contains(position)) continue;
                edge = candidate;
                party = isParty;
                return true;
            }
            return false;
        }

        public static string EdgeControlLabel(ExplorationMapEdge edge, bool party)
        {
            if (edge == ExplorationMapEdge.None) return "";
            if (party) return "Return to party";
            switch (edge)
            {
                case ExplorationMapEdge.North: return "Pan north";
                case ExplorationMapEdge.East: return "Pan east";
                case ExplorationMapEdge.South: return "Pan south";
                case ExplorationMapEdge.West: return "Pan west";
                default: return "";
            }
        }

        public static ExplorationMapEdge PartyEdge(int playerX, int playerY, Point origin, int viewWidth, int viewHeight)
        {
            if (origin == null || viewWidth <= 0 || viewHeight <= 0) return ExplorationMapEdge.None;
            int dx = playerX < origin.X ? playerX - origin.X
                : playerX >= origin.X + viewWidth ? playerX - (origin.X + viewWidth - 1) : 0;
            int dy = playerY < origin.Y ? playerY - origin.Y
                : playerY >= origin.Y + viewHeight ? playerY - (origin.Y + viewHeight - 1) : 0;
            if (dx != 0 && Mathf.Abs(dx) >= Mathf.Abs(dy))
                return dx < 0 ? ExplorationMapEdge.West : ExplorationMapEdge.East;
            if (dy != 0) return dy < 0 ? ExplorationMapEdge.North : ExplorationMapEdge.South;
            return ExplorationMapEdge.None;
        }

        public static Point EdgePanDelta(ExplorationMapEdge edge, int viewWidth, int viewHeight)
        {
            // One quarter-screen keeps context between clicks without dozens
            // of single-cell presses. ClampFocus still owns map boundaries.
            int horizontal = Mathf.Max(1, viewWidth / 4);
            int vertical = Mathf.Max(1, viewHeight / 4);
            switch (edge)
            {
                case ExplorationMapEdge.West: return new Point(-horizontal, 0);
                case ExplorationMapEdge.East: return new Point(horizontal, 0);
                case ExplorationMapEdge.North: return new Point(0, -vertical);
                case ExplorationMapEdge.South: return new Point(0, vertical);
                default: return new Point(0, 0);
            }
        }

        public static Point ClampFocus(int focusX, int focusY, int mapWidth, int mapHeight)
        {
            int lastX = Mathf.Max(0, mapWidth - 1);
            int lastY = Mathf.Max(0, mapHeight - 1);
            return new Point(
                Mathf.Clamp(focusX, 0, lastX),
                Mathf.Clamp(focusY, 0, lastY));
        }

        public static Point ViewportOrigin(
            int focusX,
            int focusY,
            int viewportWidth,
            int viewportHeight,
            int mapWidth,
            int mapHeight)
        {
            int boundedViewportWidth = Mathf.Clamp(viewportWidth, 1, Mathf.Max(1, mapWidth));
            int boundedViewportHeight = Mathf.Clamp(viewportHeight, 1, Mathf.Max(1, mapHeight));
            Point focus = ClampFocus(focusX, focusY, mapWidth, mapHeight);
            int maxX = Mathf.Max(0, mapWidth - boundedViewportWidth);
            int maxY = Mathf.Max(0, mapHeight - boundedViewportHeight);
            return new Point(
                Mathf.Clamp(focus.X - boundedViewportWidth / 2, 0, maxX),
                Mathf.Clamp(focus.Y - boundedViewportHeight / 2, 0, maxY));
        }

        public static Point ViewportPanFocus(int focusX, int focusY, int deltaX, int deltaY,
            int viewportWidth, int viewportHeight, int mapWidth, int mapHeight)
        {
            Point focus = ClampFocus(focusX, focusY, mapWidth, mapHeight);
            int width = Mathf.Clamp(viewportWidth, 1, Mathf.Max(1, mapWidth));
            int height = Mathf.Clamp(viewportHeight, 1, Mathf.Max(1, mapHeight));
            Point origin = ViewportOrigin(focus.X, focus.Y, width, height, mapWidth, mapHeight);
            int nextX = Mathf.Clamp(origin.X + deltaX, 0, Mathf.Max(0, mapWidth - width));
            int nextY = Mathf.Clamp(origin.Y + deltaY, 0, Mathf.Max(0, mapHeight - height));
            // Pointer gestures move the visible map, not a selected cell that
            // may sit inside a clamped edge. Keeping blocked/untouched axes
            // unchanged also prevents overscroll from building a reversal delay.
            return new Point(
                nextX != origin.X ? nextX + width / 2 : focus.X,
                nextY != origin.Y ? nextY + height / 2 : focus.Y);
        }

        public static RegionMapNavigationStep ResolveAxes(
            float horizontalAxis,
            float verticalAxis,
            int heldX,
            int heldY,
            float nextRepeatAt,
            float now)
        {
            int directionX = AxisDirection(horizontalAxis);
            int directionY = -AxisDirection(verticalAxis);
            if (directionX == 0 && directionY == 0)
            {
                return new RegionMapNavigationStep(0, 0, 0, 0, 0f);
            }

            bool changed = directionX != heldX || directionY != heldY;
            if (changed)
            {
                return new RegionMapNavigationStep(
                    directionX,
                    directionY,
                    directionX,
                    directionY,
                    now + InitialRepeatDelay);
            }
            if (now < nextRepeatAt)
            {
                return new RegionMapNavigationStep(0, 0, directionX, directionY, nextRepeatAt);
            }
            return new RegionMapNavigationStep(
                directionX,
                directionY,
                directionX,
                directionY,
                now + RepeatInterval);
        }

        public static RegionMapPointerPanStep ResolvePointerDrag(
            float pointerDeltaX,
            float pointerDeltaY,
            float cellSize,
            float remainderX,
            float remainderY)
        {
            float boundedCellSize = Mathf.Max(1f, cellSize);
            float accumulatedX = remainderX - pointerDeltaX;
            float accumulatedY = remainderY - pointerDeltaY;
            int deltaX = (int)(accumulatedX / boundedCellSize);
            int deltaY = (int)(accumulatedY / boundedCellSize);
            return new RegionMapPointerPanStep(
                deltaX,
                deltaY,
                accumulatedX - deltaX * boundedCellSize,
                accumulatedY - deltaY * boundedCellSize);
        }

        public static Point ScrollDelta(float scrollY, bool horizontal)
        {
            return ScrollDelta(0f, scrollY, horizontal);
        }

        public static Point ScrollDelta(float scrollX, float scrollY, bool horizontal)
        {
            int directionX = ScrollDirection(scrollX);
            int directionY = ScrollDirection(scrollY);
            // Shift remaps a conventional wheel while a native horizontal
            // trackpad gesture continues to work without a modifier.
            return horizontal
                ? new Point(directionY != 0 ? directionY : directionX, 0)
                : new Point(directionX, directionY);
        }

        public static bool IsPointerDrag(float accumulatedX, float accumulatedY, float interfaceScale = 1f)
        {
            float threshold = PointerDragThreshold * Mathf.Max(0.75f, interfaceScale);
            return accumulatedX * accumulatedX + accumulatedY * accumulatedY >= threshold * threshold;
        }

        public static bool ShouldRouteSubmitToWorld(bool explorationHudOwnsSelection)
        {
            return !explorationHudOwnsSelection;
        }

        public static bool ShouldShowMovementKeycap(bool regionMap)
        {
            return !regionMap;
        }

        private static int AxisDirection(float axis)
        {
            if (axis >= AxisThreshold) return 1;
            if (axis <= -AxisThreshold) return -1;
            return 0;
        }

        private static int ScrollDirection(float delta)
        {
            if (float.IsNaN(delta) || float.IsInfinity(delta)) return 0;
            return delta > 0f ? 1 : delta < 0f ? -1 : 0;
        }
    }
}
