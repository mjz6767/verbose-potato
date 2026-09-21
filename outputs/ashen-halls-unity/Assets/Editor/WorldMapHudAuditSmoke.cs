using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AshenHalls.Editor
{
    public static class WorldMapHudAuditSmoke
    {
        private const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;

        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " world map HUD audit smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " world map HUD audit smoke failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            AuthoredLocationNamesStayReadable();
            CompactBoardHeadingKeepsLocationAndControlsSeparate();
            ContextualControlsAdvertiseTheirShortcuts();
        }

        private static void CompactBoardHeadingKeepsLocationAndControlsSeparate()
        {
            foreach (float scale in new[] { 1f, 1.25f })
            foreach (float width in new[] { 540f, 629f, 899f, 900f, 1100f, 1400f })
            {
                Rect strip = new Rect(19f, 96f, width * scale, 36f * scale);
                ExplorationRegionStripGeometry columns = ExplorationHudScreenLayout.RegionStrip(strip, scale);
                Require(columns.Compact == (width < 900f), "board header uses its safe compact breakpoint");
                Rect[] visible = columns.Compact
                    ? new[] { columns.Location, columns.View, columns.Details }
                    : new[] { columns.Location, columns.Danger, columns.Context, columns.View, columns.Details };
                for (int i = 0; i < visible.Length; i++)
                {
                    Rect rect = visible[i];
                    Require(rect.width > 0f && rect.xMin >= strip.xMin && rect.xMax <= strip.xMax
                        && rect.yMin >= strip.yMin && rect.yMax <= strip.yMax,
                        "every visible board header column fits " + width + " at " + scale);
                    if (i > 0) Require(visible[i - 1].xMax < rect.xMin, "board header columns never overlap");
                }
                if (columns.Compact)
                    Require(columns.Danger.width == 0f && columns.Context.width == 0f, "compact header omits redundant danger and focus columns");
                GUIStyle locationStyle = new GUIStyle { font = UiRuntime.DefaultFont, fontSize = Mathf.RoundToInt(14f * scale), fontStyle = FontStyle.Bold };
                Require(locationStyle.CalcSize(new GUIContent("Green Shrine Training Ring")).x <= columns.Location.width,
                    "the full formerly elided destination fits the board header " + width + " at " + scale);
                GUIStyle controlStyle = new GUIStyle { font = UiRuntime.DefaultFont, fontSize = Mathf.RoundToInt(11f * scale), fontStyle = FontStyle.Bold };
                foreach (string status in new[] { "PARTY HERE", "INSPECT 59,59", "Local Map" })
                    Require(controlStyle.CalcSize(new GUIContent(status)).x <= columns.View.width, "board header retains the complete view/focus status");
                Require(controlStyle.CalcSize(new GUIContent("Q  DETAILS")).x <= columns.Details.width, "board header retains the details shortcut");
            }
        }

        private static void AuthoredLocationNamesStayReadable()
        {
            int width = WorldMapGenerationRules.Width;
            int height = WorldMapGenerationRules.Height;
            int startX = WorldMapGenerationRules.StartX(width);
            int startY = WorldMapGenerationRules.StartY(height);
            List<string> names = WorldMapGenerationRules.RegionalSites(width, height, startX, startY)
                .Select(site => site.Name)
                .Concat(WorldMapGenerationRules.RegionalJunctions(width, height, startX, startY).Select(junction => junction.Name))
                .Concat(new[] { "Town Hall / Grand Hearth", "Green Shrine Training Ring / Eastern Approach", "Uncharted" })
                .ToList();
            GameObject host = new GameObject("World map HUD heading audit");
            try
            {
                ExplorationHudView view = new ExplorationHudView { ViewLabel = "Region Map", DangerLabel = "DANGEROUS", WaypointLine = "Marked route", ObjectiveSummary = "Explore the Old Road" };
                ExplorationHudScreen screen = host.AddComponent<ExplorationHudScreen>();
                screen.Bind(new ExplorationHudScreenBindings { View = () => view });
                screen.SetVisible(true);
                foreach (Vector2Int size in new[] { new Vector2Int(960, 600), new Vector2Int(1024, 768), new Vector2Int(1280, 720), new Vector2Int(1600, 900), new Vector2Int(1920, 1080), new Vector2Int(2048, 1152) })
                foreach (bool detailsOpen in new[] { false, true })
                {
                    view.DetailsOpen = detailsOpen;
                    float scale = ExplorationHudScreenLayout.InterfaceScale(size.x, size.y);
                    ExplorationHudGeometry geometry = ExplorationHudScreenLayout.Calculate(size.x, size.y, detailsOpen);
                    Rect heading = ExplorationHudScreenLayout.LocationTitle(geometry.Side.width, scale);
                    Rect danger = ExplorationHudScreenLayout.LocationDanger(geometry.Side.width, scale);
                    Require(heading.xMin >= 0f && heading.xMax <= geometry.Side.width, "full location heading remains inside the rail " + size);
                    Require(heading.yMax <= danger.yMin && danger.yMax <= 54f * scale, "location, danger, and NEXT have separate vertical space " + size);
                    Rect details = ExplorationHudScreenLayout.DetailsButton(geometry.Side.width, geometry.Side.height, scale);
                    foreach (Rect party in ExplorationHudScreenLayout.PartyRows(geometry.Side.width, scale, detailsOpen, 4))
                        Require(party.yMax <= details.yMin - 4f * scale + 0.01f, "long headings do not displace any of the four party rows " + size);

                    foreach (string name in names)
                    {
                        view.ZoneName = name;
                        screen.Refresh();
                        Layout(screen, detailsOpen, size);
                        Text title = Field<Text>(screen, "sideTitleText");
                        Require(title.text == name, "native heading preserves the complete destination: " + name);
                        Require(title.resizeTextForBestFit && title.resizeTextMinSize >= 15, "heading fit never uses unreadable microtype " + size);
                        Require(Mathf.Abs(title.rectTransform.rect.height - heading.height) < 0.01f, "native heading uses shared two-line geometry " + size);
                        TextGenerationSettings settings = title.GetGenerationSettings(title.rectTransform.rect.size);
                        settings.resizeTextForBestFit = false;
                        settings.fontSize = title.resizeTextMinSize;
                        settings.verticalOverflow = VerticalWrapMode.Overflow;
                        float textHeight = title.cachedTextGeneratorForLayout.GetPreferredHeight(name, settings) / title.pixelsPerUnit;
                        Require(textHeight <= heading.height + 0.5f, "complete native location fits at its minimum readable size: " + name + " at " + size + " height=" + textHeight);

                        GUIStyle fallbackStyle = new GUIStyle
                        {
                            font = UiRuntime.DefaultFont,
                            fontSize = title.resizeTextMinSize,
                            fontStyle = FontStyle.Bold,
                            wordWrap = true
                        };
                        Require(fallbackStyle.CalcHeight(new GUIContent(name), heading.width) <= heading.height + 0.5f,
                            "complete fallback location fits without ellipsis: " + name + " at " + size);
                    }
                }
                screen.SetVisible(false);
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        private static void ContextualControlsAdvertiseTheirShortcuts()
        {
            GameObject host = new GameObject("World map HUD control audit");
            try
            {
                int uses = 0;
                ExplorationHudView view = new ExplorationHudView { ViewLabel = "Local Map", HasAction = true, ActionLabel = "Enter", ActionTarget = "Town Hall" };
                ExplorationHudScreen screen = host.AddComponent<ExplorationHudScreen>();
                screen.Bind(new ExplorationHudScreenBindings { View = () => view, UseContextual = () => uses++ });
                screen.SetVisible(true);
                screen.Refresh();
                Require(Field<Text>(screen, "actionKeyText").text == "E", "local interaction advertises its keyboard action");
                Require(Field<Text>(screen, "detailsButtonText").text.Contains("Q"), "collapsed details advertises Q");
                Require(screen.ContextualActionLabelForTest == "Enter" && screen.ContextualActionTargetForTest == "Town Hall", "shortcut label does not replace the actual target or action");
                screen.InvokeContextualActionForTest();
                Require(uses == 1, "discoverable contextual action still dispatches once");
                view.DetailsOpen = true;
                view.ViewLabel = "Region Map";
                view.ActionLabel = "Clear Route";
                screen.Refresh();
                Require(Field<Text>(screen, "detailsButtonText").text == "Close · Q", "expanded details keeps its return shortcut");
                Require(Field<Text>(screen, "mapButtonText").text == "Local\nTab / Y", "Region Map always explains how to return to travel");
                foreach (Vector2Int size in new[] { new Vector2Int(960, 600), new Vector2Int(1280, 720), new Vector2Int(1920, 1080) })
                {
                    Layout(screen, true, size);
                    RectTransform key = Field<Text>(screen, "actionKeyText").rectTransform;
                    RectTransform target = Field<Text>(screen, "actionTargetText").rectTransform;
                    Require(target.anchoredPosition.x + target.rect.width <= key.anchoredPosition.x,
                        "action shortcut cannot cover the destination " + size);
                }
                view.HasAction = false;
                screen.Refresh();
                Require(Field<Text>(screen, "actionKeyText").text.Length == 0 && !screen.HasContextualActionForTest,
                    "non-actionable focus cannot advertise an active interaction");
                screen.SetVisible(false);
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        private static void Layout(ExplorationHudScreen screen, bool detailsOpen, Vector2Int size)
        {
            typeof(ExplorationHudScreen).GetMethod("ApplyLayout", PrivateInstance, null, new[] { typeof(bool), typeof(float), typeof(float) }, null)
                .Invoke(screen, new object[] { detailsOpen, (float)size.x, (float)size.y });
        }

        private static T Field<T>(object source, string name)
        {
            return (T)source.GetType().GetField(name, PrivateInstance).GetValue(source);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("World map HUD audit smoke: " + message);
        }
    }
}
