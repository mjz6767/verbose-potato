using System;

namespace AshenHalls
{
    public enum ExplorationMiniMapMarkerKind
    {
        AuthoredSite,
        CurrentSite,
        Patrol,
        AlertedPatrol
    }

    public static class ExplorationMiniMapPresentationRules
    {
        public static bool ShouldShowAuthoredSite(
            bool discovered,
            bool current,
            int distance,
            int revealRadius)
        {
            if (current || discovered) return true;
            return distance >= 0 && distance <= Math.Max(0, revealRadius);
        }

        public static bool ShouldShowPatrol(
            bool active,
            int patrolDepth,
            int currentDepth,
            bool alerted,
            int distance,
            int revealRadius)
        {
            if (!active || patrolDepth != currentDepth || distance < 0) return false;
            return alerted || distance <= Math.Max(0, revealRadius);
        }

        public static int MarkerPixels(ExplorationMiniMapMarkerKind kind)
        {
            switch (kind)
            {
                case ExplorationMiniMapMarkerKind.CurrentSite: return 8;
                case ExplorationMiniMapMarkerKind.AlertedPatrol: return 8;
                case ExplorationMiniMapMarkerKind.AuthoredSite: return 6;
                default: return 5;
            }
        }

    }
}
