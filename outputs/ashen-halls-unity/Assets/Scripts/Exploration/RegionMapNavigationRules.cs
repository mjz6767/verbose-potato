using UnityEngine;

namespace AshenHalls
{
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
            int direction = scrollY > 0f ? 1 : scrollY < 0f ? -1 : 0;
            return horizontal ? new Point(direction, 0) : new Point(0, direction);
        }

        private static int AxisDirection(float axis)
        {
            if (axis >= AxisThreshold) return 1;
            if (axis <= -AxisThreshold) return -1;
            return 0;
        }
    }
}
