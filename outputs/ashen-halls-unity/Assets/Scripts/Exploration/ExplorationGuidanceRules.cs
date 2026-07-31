using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public readonly struct ExplorationGuidanceRoute
    {
        public readonly string TargetName;
        public readonly string FirstDirection;
        public readonly int StepCount;
        public readonly bool RouteBlocked;

        public ExplorationGuidanceRoute(
            string targetName,
            string firstDirection,
            int stepCount,
            bool routeBlocked = false)
        {
            TargetName = targetName;
            FirstDirection = firstDirection;
            StepCount = stepCount;
            RouteBlocked = routeBlocked;
        }

        public bool HasTarget => !string.IsNullOrWhiteSpace(TargetName);
    }

    public static class ExplorationGuidanceRules
    {
        public const int MaxHudLineLength = 72;

        public static string UseNow(string targetName, string contextualVerb, bool interiorExit = false)
        {
            string target = Clean(targetName);
            if (interiorExit)
            {
                return target.Length == 0
                    ? "E / Space | Leave this interior"
                    : Compose("E / Space | Leave via ", target, "");
            }

            if (target.Length == 0) return "E / Space | Use nearby objective";
            string verb = Truncate(Clean(contextualVerb), 20);
            if (verb.Length == 0) verb = "Use";
            return Compose("E / Space | " + verb + " ", target, "");
        }

        public static string Route(
            string targetName,
            string firstDirection,
            int stepCount,
            bool markedWaypoint = false,
            bool routeBlocked = false)
        {
            string target = Clean(targetName);
            if (target.Length == 0) return "WASD / arrows | No guided route is available";

            string prefix = markedWaypoint ? "Marked: " : "";
            string direction = NormalizeDirection(firstDirection);
            if (routeBlocked || stepCount < 0 || direction.Length == 0 && stepCount > 0)
            {
                return Compose("WASD / arrows | " + prefix, target, " | Route blocked");
            }

            if (stepCount == 0)
            {
                return markedWaypoint
                    ? Compose("J | Marked: ", target, " | Here - open Journal to Clear")
                    : Compose("WASD / arrows | ", target, " | Here");
            }
            string distance = stepCount == 1 ? "1 step" : stepCount + " steps";
            return Compose(
                MovementInput(direction) + " | " + prefix,
                target,
                " | " + DirectionName(direction) + " | " + distance);
        }

        public static string PreferredRoute(
            ExplorationGuidanceRoute objectiveRoute,
            ExplorationGuidanceRoute markedRoute)
        {
            if (markedRoute.HasTarget)
            {
                return Route(
                    markedRoute.TargetName,
                    markedRoute.FirstDirection,
                    markedRoute.StepCount,
                    true,
                    markedRoute.RouteBlocked);
            }

            return Route(
                objectiveRoute.TargetName,
                objectiveRoute.FirstDirection,
                objectiveRoute.StepCount,
                false,
                objectiveRoute.RouteBlocked);
        }

        private static string Compose(string prefix, string target, string suffix)
        {
            prefix = prefix ?? "";
            suffix = suffix ?? "";
            target = Clean(target);
            int targetBudget = Math.Max(1, MaxHudLineLength - prefix.Length - suffix.Length);
            return prefix + Truncate(target, targetBudget) + suffix;
        }

        private static string Clean(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            return string.Join(" ", value.Split((char[])null, StringSplitOptions.RemoveEmptyEntries));
        }

        private static string NormalizeDirection(string direction)
        {
            string value = Clean(direction).ToUpperInvariant();
            switch (value)
            {
                case "N":
                case "S":
                case "E":
                case "W":
                    return value;
                default:
                    return "";
            }
        }

        public static string MovementInput(string direction)
        {
            switch (NormalizeDirection(direction))
            {
                case "N": return "W / Up";
                case "S": return "S / Down";
                case "E": return "D / Right";
                case "W": return "A / Left";
                default: return "WASD / arrows";
            }
        }

        public static string MovementKey(string direction)
        {
            switch (NormalizeDirection(direction))
            {
                case "N": return "W";
                case "S": return "S";
                case "E": return "D";
                case "W": return "A";
                default: return "";
            }
        }

        public static string DirectionName(string direction)
        {
            switch (NormalizeDirection(direction))
            {
                case "N": return "North";
                case "S": return "South";
                case "E": return "East";
                case "W": return "West";
                default: return "";
            }
        }

        private static string Truncate(string value, int maximumLength)
        {
            value = value ?? "";
            if (maximumLength <= 0) return "";
            if (value.Length <= maximumLength) return value;
            if (maximumLength <= 3) return new string('.', maximumLength);
            return value.Substring(0, maximumLength - 3).TrimEnd() + "...";
        }
    }

    public enum ExplorationMapEdge
    {
        None,
        North,
        East,
        South,
        West
    }

    public readonly struct ExplorationMapExitCue
    {
        public readonly ExplorationMapEdge Edge;
        public readonly int MapX;
        public readonly int MapY;
        public readonly int RemainingSteps;
        public readonly int PathIndex;

        public ExplorationMapExitCue(
            ExplorationMapEdge edge,
            int mapX,
            int mapY,
            int remainingSteps,
            int pathIndex)
        {
            Edge = edge;
            MapX = mapX;
            MapY = mapY;
            RemainingSteps = Math.Max(0, remainingSteps);
            PathIndex = Math.Max(0, pathIndex);
        }
    }

    public static class ExplorationMapGuidanceRules
    {
        public static int VisiblePointLimit(bool regionMap, bool markedWaypoint)
        {
            if (markedWaypoint) return regionMap ? 25 : 14;
            return regionMap ? 18 : 10;
        }

        public static bool IsExitCueWithinVisiblePrefix(
            ExplorationMapExitCue cue,
            int visiblePointLimit)
        {
            return cue.Edge != ExplorationMapEdge.None
                && visiblePointLimit > 0
                && cue.PathIndex < visiblePointLimit;
        }

        public static bool TryFindViewportExit(
            IReadOnlyList<Point> path,
            int originX,
            int originY,
            int viewWidth,
            int viewHeight,
            out ExplorationMapExitCue cue)
        {
            cue = default;
            if (path == null || path.Count < 2 || viewWidth <= 0 || viewHeight <= 0)
            {
                return false;
            }

            for (int i = 0; i < path.Count; i++)
            {
                Point point = path[i];
                if (point == null) return false;
                if (i == 0) continue;
                Point previous = path[i - 1];
                if (Math.Abs(point.X - previous.X) + Math.Abs(point.Y - previous.Y) != 1)
                {
                    return false;
                }
            }

            for (int i = 1; i < path.Count; i++)
            {
                Point previous = path[i - 1];
                Point current = path[i];
                if (!Inside(previous, originX, originY, viewWidth, viewHeight)
                    || Inside(current, originX, originY, viewWidth, viewHeight))
                {
                    continue;
                }

                ExplorationMapEdge edge = ExitEdge(
                    current,
                    originX,
                    originY,
                    viewWidth,
                    viewHeight);
                if (edge == ExplorationMapEdge.None) continue;
                cue = new ExplorationMapExitCue(
                    edge,
                    previous.X,
                    previous.Y,
                    path.Count - i,
                    i - 1);
                return true;
            }

            return false;
        }

        private static bool Inside(
            Point point,
            int originX,
            int originY,
            int viewWidth,
            int viewHeight)
        {
            return point.X >= originX
                && point.Y >= originY
                && point.X < originX + viewWidth
                && point.Y < originY + viewHeight;
        }

        private static ExplorationMapEdge ExitEdge(
            Point point,
            int originX,
            int originY,
            int viewWidth,
            int viewHeight)
        {
            if (point.X < originX) return ExplorationMapEdge.West;
            if (point.X >= originX + viewWidth) return ExplorationMapEdge.East;
            if (point.Y < originY) return ExplorationMapEdge.North;
            if (point.Y >= originY + viewHeight) return ExplorationMapEdge.South;
            return ExplorationMapEdge.None;
        }
    }
}
