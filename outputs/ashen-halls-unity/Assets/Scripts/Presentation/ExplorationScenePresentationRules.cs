using System.Globalization;
using UnityEngine;

namespace AshenHalls
{
    public readonly struct ExplorationSceneHeaderGeometry
    {
        public readonly Rect Location;
        public readonly Rect View;
        public readonly Rect Route;

        public ExplorationSceneHeaderGeometry(Rect location, Rect view, Rect route)
        {
            Location = location;
            View = view;
            Route = route;
        }
    }

    public readonly struct ExplorationBuildingGroundingStyle
    {
        public readonly float ShadowAlpha;
        public readonly float BandAlpha;
        public readonly float BorderAlpha;
        public readonly float ThresholdAlpha;

        public ExplorationBuildingGroundingStyle(float shadow, float band, float border, float threshold)
        {
            ShadowAlpha = shadow;
            BandAlpha = band;
            BorderAlpha = border;
            ThresholdAlpha = threshold;
        }
    }

    public static class ExplorationScenePresentationRules
    {
        public static string RouteExitLabel(ExplorationMapExitCue cue, bool markedWaypoint)
        {
            string bearing;
            switch (cue.Edge)
            {
                case ExplorationMapEdge.North: bearing = "N"; break;
                case ExplorationMapEdge.East: bearing = "E"; break;
                case ExplorationMapEdge.South: bearing = "S"; break;
                case ExplorationMapEdge.West: bearing = "W"; break;
                default: return "";
            }
            return (markedWaypoint ? "MARK  " : "NEXT  ") + bearing
                + cue.RemainingSteps.ToString(CultureInfo.InvariantCulture);
        }

        public static ExplorationSceneHeaderGeometry Header(
            Rect strip, float scale, bool wideView, bool hasRouteCue, float routeLabelWidth = 0f)
        {
            ExplorationRegionStripGeometry columns = ExplorationHudScreenLayout.RegionStrip(strip, scale);
            if (!hasRouteCue)
                return new ExplorationSceneHeaderGeometry(columns.Location, columns.View, Rect.zero);

            float gap = 10f * scale;
            float width = Mathf.Max(102f * scale, routeLabelWidth + 12f * scale);
            float right = wideView ? columns.View.xMin - gap : columns.View.xMax;
            Rect route = new Rect(right - width, columns.View.center.y - 11f * scale, width, 22f * scale);
            Rect location = columns.Location;
            location.width = Mathf.Max(0f, Mathf.Min(location.xMax, route.xMin - gap) - location.xMin);
            // Local uses its otherwise empty status slot. Region keeps the exact
            // PARTY HERE / INSPECT slot and reserves a separate route slot.
            // Neither label enters the map's world-art or pointer rectangle.
            return new ExplorationSceneHeaderGeometry(location, columns.View, route);
        }

        public static bool EmphasizeBuilding(bool objective, bool currentInteraction, bool hovered, bool markedWaypoint)
        {
            return objective || currentInteraction || hovered || markedWaypoint;
        }

        public static ExplorationBuildingGroundingStyle BuildingGrounding(
            bool wideView, bool emphasized, bool preserveInteriorFootprint)
        {
            if (emphasized || preserveInteriorFootprint)
                return new ExplorationBuildingGroundingStyle(
                    wideView ? 0.34f : 0.42f,
                    wideView ? 0.30f : 0.38f,
                    wideView ? 0.22f : 0.28f,
                    0.42f);

            // Authored building art already owns its doorway and silhouette.
            // Passive shops need neutral contact, not colored UI-like slabs.
            return new ExplorationBuildingGroundingStyle(wideView ? 0.22f : 0.28f, 0f, 0f, 0f);
        }
    }
}
