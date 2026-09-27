using System;
using UnityEditor;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class WorldMapScenePresentationSmoke
    {
        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " world map scene presentation smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " world map scene presentation smoke failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            RouteLabelsKeepBearingAndExactCount();
            RouteHeaderKeepsWorldArtAndStatusClear();
            BuildingGroundingKeepsOnlyMeaningfulAccents();
        }

        private static void RouteLabelsKeepBearingAndExactCount()
        {
            ExplorationMapEdge[] edges = { ExplorationMapEdge.North, ExplorationMapEdge.East, ExplorationMapEdge.South, ExplorationMapEdge.West };
            string[] bearings = { "N", "E", "S", "W" };
            for (int i = 0; i < edges.Length; i++)
            foreach (int steps in new[] { 0, 1, 9, 10, 999, 3600, int.MaxValue })
            foreach (bool marked in new[] { false, true })
            {
                ExplorationMapExitCue cue = new ExplorationMapExitCue(edges[i], 12, 17, steps, 4);
                string expected = (marked ? "MARK  " : "NEXT  ") + bearings[i]
                    + steps.ToString(System.Globalization.CultureInfo.InvariantCulture);
                Require(ExplorationScenePresentationRules.RouteExitLabel(cue, marked) == expected,
                    "route header keeps the exact route kind, bearing and remaining count");
                Require(cue.MapX == 12 && cue.MapY == 17 && cue.PathIndex == 4 && cue.RemainingSteps == steps,
                    "formatting never alters the real route exit anchor or path index");
            }
            Require(ExplorationScenePresentationRules.RouteExitLabel(default, false) == "",
                "absent or suppressed exits reserve no route text");
            Require(ExplorationScenePresentationRules.RouteExitLabel(
                    new ExplorationMapExitCue((ExplorationMapEdge)99, 1, 1, 4, 1), true) == "",
                "unknown edge does not advertise a guessed bearing");
        }

        private static void RouteHeaderKeepsWorldArtAndStatusClear()
        {
            foreach (float scale in new[] { 1f, 1.25f })
            foreach (float width in new[] { 420f, 540f, 629f, 899f, 1100f, 1400f })
            foreach (bool wide in new[] { false, true })
            {
                Rect strip = new Rect(19f, 96f, width * scale, 36f * scale);
                Rect world = new Rect(strip.x, strip.yMax + 8f * scale, strip.width, 420f * scale);
                ExplorationRegionStripGeometry original = ExplorationHudScreenLayout.RegionStrip(strip, scale);
                ExplorationSceneHeaderGeometry noCue = ExplorationScenePresentationRules.Header(strip, scale, wide, false);
                Require(noCue.Location == original.Location && noCue.View == original.View && noCue.Route == Rect.zero,
                    "without a route exit, the established location and focus layout is unchanged");

                GUIStyle routeStyle = new GUIStyle
                {
                    font = UiRuntime.DefaultFont,
                    fontSize = Mathf.RoundToInt(10f * scale),
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
                foreach (string label in new[] { "NEXT  N11", "MARK  W3600", "MARK  S2147483647" })
                {
                    float textWidth = routeStyle.CalcSize(new GUIContent(label)).x;
                    ExplorationSceneHeaderGeometry layout = ExplorationScenePresentationRules.Header(strip, scale, wide, true, textWidth);
                    Require(Contains(strip, layout.Route) && !layout.Route.Overlaps(world),
                        "route chip stays wholly in the header, away from roofs and actor heads");
                    Require(Contains(strip, layout.Location) && layout.Location.width >= 80f * scale,
                        "location and hover identity retain a separate readable lane");
                    Require(layout.Location.xMax + 9f * scale <= layout.Route.xMin,
                        "location text never overlaps route text");
                    Require(layout.Route.width >= textWidth + 11f * scale,
                        "full route counts fit without elision or reduced type");
                    Require(layout.View == original.View,
                        "existing PARTY HERE and INSPECT geometry is preserved");
                    if (wide)
                        Require(layout.Route.xMax + 9f * scale <= layout.View.xMin,
                            "Region route chip cannot hide focus coordinates");
                    else if (label == "NEXT  N11")
                        Require(layout.Location == original.Location,
                            "ordinary Local route uses the empty status lane without shortening hover identity");
                    if (wide && width >= 540f)
                    {
                        GUIStyle nameStyle = new GUIStyle { font = UiRuntime.DefaultFont, fontSize = Mathf.RoundToInt(14f * scale), fontStyle = FontStyle.Bold };
                        Require(nameStyle.CalcSize(new GUIContent("Green Shrine Training Ring")).x <= layout.Location.width,
                            "an authored long Region destination still fits alongside the route and focus lanes");
                    }
                }
            }
        }

        private static void BuildingGroundingKeepsOnlyMeaningfulAccents()
        {
            for (int mask = 0; mask < 16; mask++)
            {
                bool emphasized = ExplorationScenePresentationRules.EmphasizeBuilding(
                    (mask & 1) != 0, (mask & 2) != 0, (mask & 4) != 0, (mask & 8) != 0);
                Require(emphasized == (mask != 0),
                    "objective, interaction, hover and marked-route focus each preserve building emphasis");
            }

            foreach (bool wide in new[] { false, true })
            {
                ExplorationBuildingGroundingStyle passive = ExplorationScenePresentationRules.BuildingGrounding(wide, false, false);
                ExplorationBuildingGroundingStyle focused = ExplorationScenePresentationRules.BuildingGrounding(wide, true, false);
                ExplorationBuildingGroundingStyle interior = ExplorationScenePresentationRules.BuildingGrounding(wide, false, true);
                Require(passive.ShadowAlpha > 0f && passive.ShadowAlpha < focused.ShadowAlpha,
                    "passive shops retain quieter neutral ground contact");
                Require(passive.BandAlpha == 0f && passive.BorderAlpha == 0f && passive.ThresholdAlpha == 0f,
                    "passive shops have no colored ground strips or rectangular borders");
                Require(Near(focused.ShadowAlpha, wide ? 0.34f : 0.42f)
                    && Near(focused.BandAlpha, wide ? 0.30f : 0.38f)
                    && Near(focused.BorderAlpha, wide ? 0.22f : 0.28f)
                    && Near(focused.ThresholdAlpha, 0.42f),
                    "meaningful focused buildings preserve the established emphasis strength");
                Require(interior.ShadowAlpha == focused.ShadowAlpha && interior.BandAlpha == focused.BandAlpha
                    && interior.BorderAlpha == focused.BorderAlpha && interior.ThresholdAlpha == focused.ThresholdAlpha,
                    "authored interior grounding remains unchanged");
                foreach (ExplorationBuildingGroundingStyle style in new[] { passive, focused, interior })
                foreach (float alpha in new[] { style.ShadowAlpha, style.BandAlpha, style.BorderAlpha, style.ThresholdAlpha })
                    Require(!float.IsNaN(alpha) && !float.IsInfinity(alpha) && alpha >= 0f && alpha <= 1f,
                        "every grounding opacity stays finite and bounded");
            }
        }

        private static bool Contains(Rect outer, Rect inner)
        {
            const float tolerance = 0.01f;
            return inner.width > 0f && inner.height > 0f
                && inner.xMin >= outer.xMin - tolerance && inner.xMax <= outer.xMax + tolerance
                && inner.yMin >= outer.yMin - tolerance && inner.yMax <= outer.yMax + tolerance;
        }

        private static bool Near(float a, float b) => Mathf.Abs(a - b) < 0.0001f;

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("World map scene presentation smoke: " + message);
        }
    }
}
