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
            PassiveHudGraphicsDoNotCompeteForPointerHits();
            PartyVitalWarningsAreBoundedAndReadable();
            NativePartyVitalsFollowSharedPresentationRules();
        }

        private static void CompactBoardHeadingKeepsLocationAndControlsSeparate()
        {
            foreach (float scale in new[] { 1f, 1.25f })
            foreach (float width in new[] { 540f, 629f, 899f, 900f, 1100f, 1400f })
            {
                Rect strip = new Rect(19f, 96f, width * scale, 36f * scale);
                ExplorationRegionStripGeometry columns = ExplorationHudScreenLayout.RegionStrip(strip, scale);
                Require(columns.Compact, "board header keeps one consistent hierarchy at every width");
                Rect[] visible = { columns.Location, columns.View };
                for (int i = 0; i < visible.Length; i++)
                {
                    Rect rect = visible[i];
                    Require(rect.width > 0f && rect.xMin >= strip.xMin && rect.xMax <= strip.xMax
                        && rect.yMin >= strip.yMin && rect.yMax <= strip.yMax,
                        "every visible board header column fits " + width + " at " + scale);
                    if (i > 0) Require(visible[i - 1].xMax < rect.xMin, "board header columns never overlap");
                }
                Require(columns.Danger.width == 0f && columns.Context.width == 0f && columns.Details.width == 0f,
                    "board header leaves danger, actions and Details in their persistent rail/footer homes");
                Require(columns.Location.width >= (width - 179f) * scale - 0.01f,
                    "reclaimed metadata space is available for readable location and hover identity");
                GUIStyle locationStyle = new GUIStyle { font = UiRuntime.DefaultFont, fontSize = Mathf.RoundToInt(14f * scale), fontStyle = FontStyle.Bold };
                Require(locationStyle.CalcSize(new GUIContent("Green Shrine Training Ring")).x <= columns.Location.width,
                    "the full formerly elided destination fits the board header " + width + " at " + scale);
                GUIStyle controlStyle = new GUIStyle { font = UiRuntime.DefaultFont, fontSize = Mathf.RoundToInt(11f * scale), fontStyle = FontStyle.Bold };
                foreach (string status in new[] { "PARTY HERE", "INSPECT 59,59" })
                    Require(controlStyle.CalcSize(new GUIContent(status)).x <= columns.View.width, "board header retains the complete view/focus status");
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

        private static void PassiveHudGraphicsDoNotCompeteForPointerHits()
        {
            GameObject host = new GameObject("World map HUD pointer audit");
            try
            {
                ExplorationHudScreen screen = host.AddComponent<ExplorationHudScreen>();
                screen.Bind(new ExplorationHudScreenBindings
                {
                    View = () => new ExplorationHudView { ViewLabel = "Local Map", ZoneName = "Midgaard" }
                });
                Canvas canvas = Field<Canvas>(screen, "canvas");
                Graphic[] graphics = canvas.GetComponentsInChildren<Graphic>(true);
                Require(graphics.OfType<Text>().All(text => !text.raycastTarget), "labels never create redundant pointer candidates");
                Require(graphics.Count(graphic => graphic.raycastTarget) == 9,
                    "only three enclosing panels and six actionable buttons participate in pointer hit-testing");
                foreach (string panelName in new[] { "topPanel", "sidePanel", "commandPanel" })
                    Require(Field<RectTransform>(screen, panelName).GetComponent<Image>().raycastTarget,
                        "blank " + panelName + " space still blocks map input");
                foreach (Button button in canvas.GetComponentsInChildren<Button>(true))
                    Require(button.targetGraphic != null && button.targetGraphic.raycastTarget,
                        "actual button " + button.name + " retains pointer input");
                Require(canvas.GetComponentsInChildren<Outline>(true).Length == 3,
                    "passive party and log rows do not need extra outlined card geometry");
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        private static void PartyVitalWarningsAreBoundedAndReadable()
        {
            Require(ExplorationPartyVitalsRules.Resolve(51, 100).Tone == ExplorationPartyVitalTone.Stable, "above half HP uses the quiet stable fill");
            Require(ExplorationPartyVitalsRules.Resolve(50, 100).Tone == ExplorationPartyVitalTone.Hurt, "half HP begins injured emphasis");
            Require(ExplorationPartyVitalsRules.Resolve(26, 100).Tone == ExplorationPartyVitalTone.Hurt, "above quarter HP remains injured rather than critical");
            Require(ExplorationPartyVitalsRules.Resolve(25, 100).Tone == ExplorationPartyVitalTone.Critical, "quarter HP begins critical emphasis");
            Require(ExplorationPartyVitalsRules.Resolve(0, 100).Tone == ExplorationPartyVitalTone.Down, "zero HP keeps a distinct empty-bar warning");
            Require(ExplorationPartyVitalsRules.Resolve(-10, 100).Ratio == 0f, "negative HP never draws outside the track");
            Require(ExplorationPartyVitalsRules.Resolve(120, 100).Ratio == 1f, "over-cap HP never overfills the track");
            Require(ExplorationPartyVitalsRules.Resolve(int.MaxValue, int.MaxValue).Ratio == 1f, "large HP values do not overflow ratio calculation");
            foreach (int maximum in new[] { 0, -1, int.MinValue })
            {
                ExplorationPartyVitalPresentation unknown = ExplorationPartyVitalsRules.Resolve(10, maximum);
                Require(unknown.Tone == ExplorationPartyVitalTone.Unknown && unknown.Ratio == 0f && !unknown.NeedsWarning,
                    "invalid maximum HP remains neutral and empty");
            }
            ExplorationPartyVitalPresentation healthy = ExplorationPartyVitalsRules.Resolve(100, 100);
            ExplorationPartyVitalPresentation injured = ExplorationPartyVitalsRules.Resolve(50, 100);
            ExplorationPartyVitalPresentation critical = ExplorationPartyVitalsRules.Resolve(25, 100);
            Require(healthy.Fill.maxColorComponent < injured.Fill.maxColorComponent && injured.Fill.maxColorComponent < critical.Fill.maxColorComponent,
                "visual emphasis increases as HP becomes consequential");
            Require(healthy.Fill.maxColorComponent < 0.4f, "full healthy bars stay dark enough not to dominate the map");
            Require(healthy.Label(100, 100) == "HP 100/100" && critical.Label(25, 100) == "! HP 25/100",
                "warnings retain exact HP numbers and provide a non-color signal");
            Require(ExplorationPartyVitalsRules.Resolve(-1, 100).Label(-1, 100) == "! HP 0/100",
                "empty-bar warning remains visible with clamped numeric HP");
            foreach (int hp in new[] { 100, 50, 25, 0 })
            {
                ExplorationPartyVitalPresentation vital = ExplorationPartyVitalsRules.Resolve(hp, 100);
                Require(ContrastRatio(vital.Text, vital.Fill) >= 4.5f && ContrastRatio(vital.Text, vital.Track) >= 4.5f,
                    "numeric HP keeps readable contrast over both the filled and empty track at " + hp);
            }
        }

        private static void NativePartyVitalsFollowSharedPresentationRules()
        {
            GameObject host = new GameObject("World map HUD vital audit");
            try
            {
                int[] hp = { 100, 50, 25, 0 };
                ExplorationHudPartyMemberView[] party = hp.Select((value, index) => new ExplorationHudPartyMemberView
                {
                    Name = "Member " + index, Hp = value, MaxHp = 100, Mana = 10, MaxMana = 20, ColorHex = "58b7a5"
                }).ToArray();
                ExplorationHudView view = new ExplorationHudView { ViewLabel = "Local Map", Party = party };
                ExplorationHudScreen screen = host.AddComponent<ExplorationHudScreen>();
                screen.Bind(new ExplorationHudScreenBindings { View = () => view });
                screen.SetVisible(true);
                RectTransform rail = Field<RectTransform>(screen, "sidePanel");
                foreach (bool detailsOpen in new[] { false, true })
                {
                    view.DetailsOpen = detailsOpen;
                    screen.Refresh();
                    for (int i = 0; i < party.Length; i++)
                    {
                        Transform row = rail.Find("Party Row " + i);
                        ExplorationPartyVitalPresentation vital = ExplorationPartyVitalsRules.Resolve(party[i].Hp, party[i].MaxHp);
                        Image fill = row.Find("Hp Bg/Hp Fill").GetComponent<Image>();
                        Image track = row.Find("Hp Bg").GetComponent<Image>();
                        Text label = row.Find("Hp Text").GetComponent<Text>();
                        Require(fill.color == vital.Fill && track.color == vital.Track && label.color == vital.Text,
                            "native HP applies shared fill, track and text colors in both rail modes");
                        Require(label.text == vital.Label(party[i].Hp, party[i].MaxHp) && Mathf.Approximately(fill.rectTransform.anchorMax.x, vital.Ratio),
                            "native HP preserves numeric values and proportional fill");
                        Require(row.Find("Mana Bg/Mana Fill").GetComponent<Image>().color != vital.Fill,
                            "HP presentation does not overwrite the mana color");
                    }
                }
                party[2].Hp = 100;
                screen.Refresh();
                Require(rail.Find("Party Row 2/Hp Text").GetComponent<Text>().text == "HP 100/100"
                    && rail.Find("Party Row 2/Hp Bg/Hp Fill").GetComponent<Image>().color == ExplorationPartyVitalsRules.Resolve(100, 100).Fill,
                    "healing removes stale critical emphasis");
                screen.SetVisible(false);
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        private static float ContrastRatio(Color first, Color second)
        {
            float a = RelativeLuminance(first);
            float b = RelativeLuminance(second);
            return (Mathf.Max(a, b) + 0.05f) / (Mathf.Min(a, b) + 0.05f);
        }

        private static float RelativeLuminance(Color color)
        {
            return LinearChannel(color.r) * 0.2126f + LinearChannel(color.g) * 0.7152f + LinearChannel(color.b) * 0.0722f;
        }

        private static float LinearChannel(float value)
        {
            return value <= 0.04045f ? value / 12.92f : Mathf.Pow((value + 0.055f) / 1.055f, 2.4f);
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
