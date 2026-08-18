using System;

namespace AshenHalls
{
    public readonly struct ExplorationMovementRepeatStep
    {
        public readonly int DeltaX;
        public readonly int DeltaY;
        public readonly int HeldX;
        public readonly int HeldY;
        public readonly float NextRepeatAt;
        public readonly bool IsHeldRepeat;

        public ExplorationMovementRepeatStep(
            int deltaX,
            int deltaY,
            int heldX,
            int heldY,
            float nextRepeatAt,
            bool isHeldRepeat)
        {
            DeltaX = deltaX;
            DeltaY = deltaY;
            HeldX = heldX;
            HeldY = heldY;
            NextRepeatAt = nextRepeatAt;
            IsHeldRepeat = isHeldRepeat;
        }

        public bool HasAction => DeltaX != 0 || DeltaY != 0;

        public bool IsInitialOrDirectionChange => HasAction && !IsHeldRepeat;
    }

    public static class ExplorationMovementRepeatRules
    {
        public const float AxisThreshold = 0.60f;
        public const float AxisDominanceHysteresis = 0.10f;
        public const float InitialRepeatDelay = 0.34f;
        public const float RepeatInterval = 0.18f;

        public static ExplorationMovementRepeatStep ResolveAxes(
            float horizontalAxis,
            float verticalAxis,
            int heldX,
            int heldY,
            float nextRepeatAt,
            float now)
        {
            CardinalDirection(
                horizontalAxis,
                verticalAxis,
                heldX,
                heldY,
                out int directionX,
                out int directionY);
            if (directionX == 0 && directionY == 0)
            {
                return new ExplorationMovementRepeatStep(0, 0, 0, 0, 0f, false);
            }

            NormalizeHeldDirection(heldX, heldY, out int normalizedHeldX, out int normalizedHeldY);
            float safeNow = FiniteOrZero(now);
            bool changed = directionX != normalizedHeldX || directionY != normalizedHeldY;
            if (changed)
            {
                return new ExplorationMovementRepeatStep(
                    directionX,
                    directionY,
                    directionX,
                    directionY,
                    SafeDeadline(safeNow, InitialRepeatDelay),
                    false);
            }

            if (!IsFinite(nextRepeatAt))
            {
                return new ExplorationMovementRepeatStep(
                    0,
                    0,
                    directionX,
                    directionY,
                    SafeDeadline(safeNow, InitialRepeatDelay),
                    false);
            }
            if (safeNow < nextRepeatAt)
            {
                return new ExplorationMovementRepeatStep(
                    0,
                    0,
                    directionX,
                    directionY,
                    nextRepeatAt,
                    false);
            }

            return new ExplorationMovementRepeatStep(
                directionX,
                directionY,
                directionX,
                directionY,
                SafeDeadline(safeNow, RepeatInterval),
                true);
        }

        private static void CardinalDirection(
            float horizontalAxis,
            float verticalAxis,
            int heldX,
            int heldY,
            out int directionX,
            out int directionY)
        {
            float safeHorizontal = FiniteOrZero(horizontalAxis);
            float safeVertical = FiniteOrZero(verticalAxis);
            directionX = AxisDirection(safeHorizontal);
            directionY = -AxisDirection(safeVertical);
            if (directionX == 0 || directionY == 0) return;

            float horizontalMagnitude = Math.Abs(safeHorizontal);
            float verticalMagnitude = Math.Abs(safeVertical);
            NormalizeHeldDirection(heldX, heldY, out int normalizedHeldX, out int normalizedHeldY);
            if (normalizedHeldX == directionX
                && horizontalMagnitude + AxisDominanceHysteresis >= verticalMagnitude)
            {
                directionY = 0;
                return;
            }
            if (normalizedHeldY == directionY
                && verticalMagnitude + AxisDominanceHysteresis >= horizontalMagnitude)
            {
                directionX = 0;
                return;
            }

            if (horizontalMagnitude > verticalMagnitude)
            {
                directionY = 0;
                return;
            }
            if (verticalMagnitude > horizontalMagnitude)
            {
                directionX = 0;
                return;
            }

            // Preserve the legacy keyboard priority at an exact diagonal tie.
            directionX = 0;
        }

        private static void NormalizeHeldDirection(
            int heldX,
            int heldY,
            out int normalizedHeldX,
            out int normalizedHeldY)
        {
            normalizedHeldX = Math.Sign(heldX);
            normalizedHeldY = Math.Sign(heldY);
            if (Math.Abs(normalizedHeldX) + Math.Abs(normalizedHeldY) == 1) return;
            normalizedHeldX = 0;
            normalizedHeldY = 0;
        }

        private static int AxisDirection(float axis)
        {
            if (axis >= AxisThreshold) return 1;
            if (axis <= -AxisThreshold) return -1;
            return 0;
        }

        private static float FiniteOrZero(float value)
        {
            return IsFinite(value) ? value : 0f;
        }

        private static float SafeDeadline(float now, float delay)
        {
            double deadline = (double)now + delay;
            return deadline >= float.MaxValue ? float.MaxValue : (float)deadline;
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }
    }
}
