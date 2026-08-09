using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AshenHalls
{
    public readonly struct ExplorationHudFallbackRailLayout
    {
        public readonly float NextHeight;
        public readonly float SecondaryHeight;
        public readonly float ObjectiveHeight;
        public readonly float GrowthHeight;
        public readonly float ActionHeight;
        public readonly float SectionGap;
        public readonly float PostActionGap;
        public readonly float GrowthTailGap;
        public readonly float PartyLabelHeight;
        public readonly float PartyRowHeight;
        public readonly float PartyRowStep;
        public readonly int PartyCount;
        public readonly int NextMaxLines;
        public readonly int SecondaryMaxLines;
        public readonly int ObjectiveMaxLines;
        public readonly int GrowthMaxLines;
        public readonly float UsedHeight;

        public ExplorationHudFallbackRailLayout(
            float nextHeight,
            float secondaryHeight,
            float objectiveHeight,
            float growthHeight,
            float actionHeight,
            float sectionGap,
            float postActionGap,
            float growthTailGap,
            float partyLabelHeight,
            float partyRowHeight,
            float partyRowStep,
            int partyCount,
            int nextMaxLines,
            int secondaryMaxLines,
            int objectiveMaxLines,
            int growthMaxLines,
            float usedHeight)
        {
            NextHeight = nextHeight;
            SecondaryHeight = secondaryHeight;
            ObjectiveHeight = objectiveHeight;
            GrowthHeight = growthHeight;
            ActionHeight = actionHeight;
            SectionGap = sectionGap;
            PostActionGap = postActionGap;
            GrowthTailGap = growthTailGap;
            PartyLabelHeight = partyLabelHeight;
            PartyRowHeight = partyRowHeight;
            PartyRowStep = partyRowStep;
            PartyCount = partyCount;
            NextMaxLines = nextMaxLines;
            SecondaryMaxLines = secondaryMaxLines;
            ObjectiveMaxLines = objectiveMaxLines;
            GrowthMaxLines = growthMaxLines;
            UsedHeight = usedHeight;
        }

        public bool Fits(float availableHeight)
        {
            return UsedHeight <= availableHeight + 0.01f;
        }
    }

    public readonly struct ExplorationHudFallbackCommandLayout
    {
        public readonly Rect Panel;
        public readonly Rect Action;
        public readonly Rect Camp;
        public readonly Rect Recall;
        public readonly Rect Descend;
        public readonly Rect Elixir;
        public readonly Rect Map;
        public readonly Rect Journal;
        public readonly Rect Party;
        public readonly Rect Menu;
        public readonly float ContextSeparatorX;
        public readonly float UtilitySeparatorX;

        public ExplorationHudFallbackCommandLayout(
            Rect panel,
            Rect action,
            Rect camp,
            Rect recall,
            Rect descend,
            Rect elixir,
            Rect map,
            Rect journal,
            Rect party,
            Rect menu,
            float contextSeparatorX,
            float utilitySeparatorX)
        {
            Panel = panel;
            Action = action;
            Camp = camp;
            Recall = recall;
            Descend = descend;
            Elixir = elixir;
            Map = map;
            Journal = journal;
            Party = party;
            Menu = menu;
            ContextSeparatorX = contextSeparatorX;
            UtilitySeparatorX = utilitySeparatorX;
        }

        public IReadOnlyList<Rect> Commands => new[]
        {
            Action,
            Camp,
            Recall,
            Descend,
            Elixir,
            Map,
            Journal,
            Party,
            Menu
        };

        public bool Fits()
        {
            foreach (Rect command in Commands)
            {
                if (command.xMin < Panel.xMin - 0.01f
                    || command.yMin < Panel.yMin - 0.01f
                    || command.xMax > Panel.xMax + 0.01f
                    || command.yMax > Panel.yMax + 0.01f)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public static class ExplorationHudFallbackLayoutRules
    {
        private const float CardTopAndBottom = 28f;
        private const float CardLineHeight = 15f;

        public static ExplorationHudFallbackRailLayout CalculateRail(
            bool detailsOpen,
            float innerWidth,
            float availableHeight,
            float scale,
            int bodyFontSize,
            bool hasAction,
            int partyCount,
            string nextText,
            string objectiveText,
            string secondaryText,
            string growthText)
        {
            scale = Mathf.Clamp(scale, 1f, 1.25f);
            partyCount = Mathf.Clamp(partyCount, 0, 4);
            float copyWidth = Mathf.Max(40f * scale, innerWidth - 18f * scale);
            float sectionGap = 7f * scale;
            float postActionGap = 9f * scale;
            float growthTailGap = 9f * scale;
            float partyLabelHeight = 21f * scale;
            float partyRowHeight = 32f * scale;
            float partyRowStep = 35f * scale;
            float actionHeight = !detailsOpen && hasAction ? 56f * scale : 0f;

            float nextMinimum = 54f * scale;
            float secondaryMinimum = 50f * scale;
            float objectiveMinimum = 68f * scale;
            float growthMinimum = detailsOpen ? 44f * scale : 0f;
            float nextHeight = DesiredCardHeight(nextText, copyWidth, bodyFontSize, scale, nextMinimum, 78f * scale);
            float secondaryHeight = DesiredCardHeight(secondaryText, copyWidth, bodyFontSize, scale, secondaryMinimum, 88f * scale);
            float objectiveHeight = DesiredCardHeight(objectiveText, copyWidth, bodyFontSize, scale, objectiveMinimum, 112f * scale);
            float growthHeight = detailsOpen
                ? DesiredCardHeight(growthText, copyWidth, bodyFontSize, scale, growthMinimum, 58f * scale)
                : 0f;

            float fixedHeight = partyLabelHeight + partyCount * partyRowStep;
            fixedHeight += detailsOpen
                ? sectionGap * 3f + growthTailGap
                : sectionGap * 3f + (hasAction ? actionHeight + postActionGap : 0f);
            float cardBudget = Mathf.Max(0f, availableHeight - fixedHeight);
            float overflow = nextHeight + secondaryHeight + objectiveHeight + growthHeight - cardBudget;
            if (overflow > 0f)
            {
                if (detailsOpen) ReduceToMinimum(ref growthHeight, growthMinimum, ref overflow);
                ReduceToMinimum(ref secondaryHeight, secondaryMinimum, ref overflow);
                ReduceToMinimum(ref nextHeight, nextMinimum, ref overflow);
                ReduceToMinimum(ref objectiveHeight, objectiveMinimum, ref overflow);
            }

            float usedHeight = fixedHeight + nextHeight + secondaryHeight + objectiveHeight + growthHeight;
            return new ExplorationHudFallbackRailLayout(
                nextHeight,
                secondaryHeight,
                objectiveHeight,
                growthHeight,
                actionHeight,
                sectionGap,
                postActionGap,
                growthTailGap,
                partyLabelHeight,
                partyRowHeight,
                partyRowStep,
                partyCount,
                MaxLinesForHeight(nextHeight, scale),
                MaxLinesForHeight(secondaryHeight, scale),
                MaxLinesForHeight(objectiveHeight, scale),
                detailsOpen ? MaxLinesForHeight(growthHeight, scale) : 0,
                usedHeight);
        }

        public static ExplorationHudFallbackCommandLayout CalculateCommands(Rect panel, float scale)
        {
            scale = Mathf.Clamp(scale, 1f, 1.25f);
            float padding = 10f * scale;
            float itemGap = 6f * scale;
            float groupGap = 14f * scale;
            float height = 52f * scale;
            float y = panel.y + 8f * scale;
            float actionWidth = Mathf.Clamp(panel.width * 0.245f, 292f * scale, 420f * scale);
            float buttonWidth = Mathf.Max(
                74f * scale,
                (panel.width - padding * 2f - actionWidth - groupGap * 2f - itemGap * 6f) / 8f);

            Rect action = new Rect(panel.x + padding, y, actionWidth, height);
            float travelStart = action.xMax + groupGap;
            Rect camp = new Rect(travelStart, y, buttonWidth, height);
            Rect recall = new Rect(camp.xMax + itemGap, y, buttonWidth, height);
            Rect descend = new Rect(recall.xMax + itemGap, y, buttonWidth, height);
            Rect elixir = new Rect(descend.xMax + itemGap, y, buttonWidth, height);
            float utilityStart = elixir.xMax + groupGap;
            Rect map = new Rect(utilityStart, y, buttonWidth, height);
            Rect journal = new Rect(map.xMax + itemGap, y, buttonWidth, height);
            Rect party = new Rect(journal.xMax + itemGap, y, buttonWidth, height);
            Rect menu = new Rect(party.xMax + itemGap, y, buttonWidth, height);
            return new ExplorationHudFallbackCommandLayout(
                panel,
                action,
                camp,
                recall,
                descend,
                elixir,
                map,
                journal,
                party,
                menu,
                action.xMax + groupGap * 0.5f,
                elixir.xMax + groupGap * 0.5f);
        }

        public static int EstimatedWrappedLines(string text, float width, int fontSize)
        {
            if (string.IsNullOrWhiteSpace(text)) return 1;
            int columnsPerLine = Mathf.Max(8, Mathf.FloorToInt(width / Mathf.Max(1f, fontSize * 0.62f)));
            string[] paragraphs = text.Replace("\r", "").Split('\n');
            int totalLines = 0;
            foreach (string paragraph in paragraphs)
            {
                string[] words = paragraph.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (words.Length == 0)
                {
                    totalLines++;
                    continue;
                }

                int lines = 1;
                int column = 0;
                foreach (string word in words)
                {
                    int remaining = word.Length;
                    if (column > 0 && column + 1 + remaining <= columnsPerLine)
                    {
                        column += 1 + remaining;
                        continue;
                    }
                    if (column > 0)
                    {
                        lines++;
                        column = 0;
                    }
                    while (remaining > columnsPerLine)
                    {
                        lines++;
                        remaining -= columnsPerLine;
                    }
                    column = remaining;
                }
                totalLines += lines;
            }
            return Mathf.Max(1, totalLines);
        }

        public static string BoundedCopy(string text, float width, int fontSize, int maxLines)
        {
            string source = (text ?? "").Trim();
            if (source.Length == 0 || maxLines <= 0) return "";
            if (EstimatedWrappedLines(source, width, fontSize) <= maxLines) return source;

            string compact = string.Join(
                " ",
                source.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
            int low = 1;
            int high = compact.Length;
            int best = 0;
            while (low <= high)
            {
                int middle = (low + high) / 2;
                string candidate = compact.Substring(0, middle).TrimEnd() + "…";
                if (EstimatedWrappedLines(candidate, width, fontSize) <= maxLines)
                {
                    best = middle;
                    low = middle + 1;
                }
                else
                {
                    high = middle - 1;
                }
            }
            if (best <= 0) return "…";
            int wordEnd = compact.LastIndexOf(' ', Mathf.Min(best - 1, compact.Length - 1));
            int length = wordEnd > 0 ? wordEnd : best;
            return compact.Substring(0, length).TrimEnd() + "…";
        }

        private static float DesiredCardHeight(
            string text,
            float width,
            int fontSize,
            float scale,
            float minimum,
            float maximum)
        {
            int lines = EstimatedWrappedLines(text, width, fontSize);
            return Mathf.Clamp((CardTopAndBottom + CardLineHeight * lines) * scale, minimum, maximum);
        }

        private static int MaxLinesForHeight(float height, float scale)
        {
            if (height <= 0f) return 0;
            return Mathf.Max(1, Mathf.FloorToInt((height / scale - CardTopAndBottom) / CardLineHeight));
        }

        private static void ReduceToMinimum(ref float value, float minimum, ref float overflow)
        {
            if (overflow <= 0f) return;
            float reduction = Mathf.Min(overflow, Mathf.Max(0f, value - minimum));
            value -= reduction;
            overflow -= reduction;
        }
    }

    public sealed partial class AshenHallsGame
    {
        private sealed class ExploreGuidancePlan
        {
            public string TargetName = "";
            public MapObject TargetObject;
            public int TargetX;
            public int TargetY;
            public IReadOnlyList<Point> Path = Array.Empty<Point>();
            public string Verb = "";
            public bool MarkedWaypoint;
            public bool Immediate;
            public bool InteriorExit;
            public bool RouteBlocked;

            public bool HasTarget => !string.IsNullOrWhiteSpace(TargetName);
        }

        private readonly List<Point> activeRouteWaypointPathCache = new List<Point>();
        private string activeRouteWaypointPathCacheKey = "";
        private ExploreGuidancePlan exploreGuidancePlanCache;
        private string exploreGuidancePlanCacheKey = "";

        private void EnsureExplorationHudScreen()
        {
            if (explorationHudScreen != null && explorationHudScreen.IsReady) return;
            if (explorationHudScreen != null)
            {
                Destroy(explorationHudScreen.gameObject);
                explorationHudScreen = null;
            }
            UnityEngine.GameObject screen = new UnityEngine.GameObject("Exploration HUD Screen");
            screen.transform.SetParent(transform, false);
            ExplorationHudScreen created = screen.AddComponent<ExplorationHudScreen>();
            created.Bind(new ExplorationHudScreenBindings
            {
                View = BuildExplorationHudView,
                UseContextual = UseNearbyExploreObject,
                OpenParty = () => ToggleArmory(ArmoryTab.Party),
                OpenJournal = () => ToggleArmory(ArmoryTab.Journal),
                ToggleDetails = ToggleExploreHud,
                ToggleView = ToggleExploreView,
                OpenMenu = OpenPauseMenu
            });
            created.SetVisible(false);
            explorationHudScreen = created;
        }

        private void SyncExplorationHudScreen()
        {
            UiOverlay overlay = CurrentUiOverlay();
            bool visible = state != null
                && state.Mode == GameMode.Explore
                && !ShouldShowStartupSplash();
            if (visible && (explorationHudScreen == null || !explorationHudScreen.IsReady))
            {
                TryInitializePresentationScreen("Exploration HUD recovery", EnsureExplorationHudScreen, false);
            }
            if (explorationHudScreen == null) return;
            bool activated = explorationHudScreen.SetVisible(visible);
            explorationHudScreen.SetUnderlay(visible && overlay != UiOverlay.None);
            explorationHudScreen.SetSuppressedByImguiFallback(visible);
            if (visible && (activated || ShouldRefreshPresentation(ref lastExplorationHudRefreshKey, ExplorationHudRefreshKey()))) explorationHudScreen.Refresh();
        }

        private bool NeedsEmergencyExplorationHudFallback()
        {
            return state != null
                && state.Mode == GameMode.Explore
                && CurrentUiOverlay() == UiOverlay.None
                && !ShouldShowStartupSplash();
        }

        private void DrawEmergencyExplorationHudFallback()
        {
            if (!NeedsEmergencyExplorationHudFallback()) return;
            bool previousGuiEnabled = GUI.enabled;
            GUI.enabled = previousGuiEnabled && CanAcceptGameplayInput() && !IsBoardPointerSuppressed();
            ExplorationHudView view = BuildExplorationHudView();
            ExplorationHudGeometry geometry = ExplorationHudScreenLayout.Calculate(Screen.width, Screen.height, view.DetailsOpen);
            float scale = ExplorationHudScreenLayout.InterfaceScale(Screen.width, Screen.height);
            float sidePad = 12f * scale;
            float sideInnerX = geometry.Side.x + sidePad;
            float sideInnerW = geometry.Side.width - sidePad * 2f;

            DrawRect(geometry.Top, Hex("070c0f", 0.98f));
            DrawRect(new Rect(geometry.Top.x, geometry.Top.yMax - 3f * scale, geometry.Top.width, 3f * scale), Hex("58462c", 0.72f));
            DrawBorder(geometry.Top, Hex("52605c", 0.78f), 1);
            float resourceW = 78f * scale;
            float resourceGap = 6f * scale;
            float resourcesW = resourceW * 3f + resourceGap * 2f;
            float resourceX = geometry.Top.xMax - resourcesW - 10f * scale;
            float headerX = geometry.Top.x + 14f * scale;
            float headerW = Mathf.Max(360f * scale, resourceX - headerX - 14f * scale);
            float titleW = headerW * 0.31f;
            float routeW = headerW * 0.40f;
            GUI.Label(new Rect(headerX, geometry.Top.y + 6f * scale, titleW, 35f * scale), FitText(view.Title, titleW, CenterLeftStyle(ExploreHudFont(18), ink)), CenterLeftStyle(ExploreHudFont(18), ink));
            GUI.Label(new Rect(headerX + titleW, geometry.Top.y + 6f * scale, routeW, 35f * scale), FitText(view.RouteLine, routeW, CenterStyle(ExploreHudFont(12), Hex("d0c5ae"))), CenterStyle(ExploreHudFont(12), Hex("d0c5ae")));
            float focusW = headerW - titleW - routeW;
            GUI.Label(new Rect(headerX + titleW + routeW, geometry.Top.y + 6f * scale, focusW, 35f * scale), FitText(view.FocusHint, focusW, CenterRightStyle(ExploreHudFont(12), Hex("66c9b6"))), CenterRightStyle(ExploreHudFont(12), Hex("66c9b6")));
            DrawExploreFallbackResource(new Rect(resourceX, geometry.Top.y + 6f * scale, resourceW, 36f * scale), "Gold", view.Gold, gold);
            DrawExploreFallbackResource(new Rect(resourceX + resourceW + resourceGap, geometry.Top.y + 6f * scale, resourceW, 36f * scale), "Supplies", view.Supplies, moss);
            DrawExploreFallbackResource(new Rect(resourceX + (resourceW + resourceGap) * 2f, geometry.Top.y + 6f * scale, resourceW, 36f * scale), "Elixirs", view.Elixirs, teal);

            DrawRect(geometry.Side, Hex("060a0c", 0.985f));
            DrawRect(new Rect(geometry.Side.x, geometry.Side.y, 4f * scale, geometry.Side.height), teal.WithAlpha(0.84f));
            DrawBorder(geometry.Side, Hex("58b7a5", 0.82f), 1);
            GUI.Label(new Rect(sideInnerX, geometry.Side.y + 8f * scale, sideInnerW, 24f * scale), FitText(view.ZoneName, sideInnerW, CenterLeftStyle(ExploreHudFont(18), Hex("e3ba63"))), CenterLeftStyle(ExploreHudFont(18), Hex("e3ba63")));
            GUI.Label(new Rect(sideInnerX, geometry.Side.y + 34f * scale, sideInnerW, 18f * scale), FitText(view.DangerLabel, sideInnerW, CenterLeftStyle(ExploreHudFont(11), Hex("66c9b6"))), CenterLeftStyle(ExploreHudFont(11), Hex("66c9b6")));
            DrawRect(new Rect(sideInnerX, geometry.Side.y + 57f * scale, sideInnerW, 1f), line.WithAlpha(0.72f));

            Rect detailsButton = new Rect(sideInnerX, geometry.Side.yMax - 40f * scale, sideInnerW, 32f * scale);
            float contentBottom = detailsButton.y - 8f * scale;
            float cursor = geometry.Side.y + 66f * scale;
            int partyCount = Mathf.Min(4, view.Party == null ? 0 : view.Party.Count);
            string secondaryCopy = view.DetailsOpen
                ? ((view.ZoneDetail ?? "") + "\n" + (view.LookLine ?? "")).Trim()
                : view.NearbyLine;
            ExplorationHudFallbackRailLayout rail = ExplorationHudFallbackLayoutRules.CalculateRail(
                view.DetailsOpen,
                sideInnerW,
                Mathf.Max(0f, contentBottom - cursor),
                scale,
                ExploreHudFont(12),
                view.HasAction,
                partyCount,
                view.WaypointLine,
                view.DetailsOpen ? view.ObjectiveLine : view.ObjectiveSummary,
                secondaryCopy,
                view.GrowthLine);
            float copyWidth = Mathf.Max(40f * scale, sideInnerW - 18f * scale);
            if (view.DetailsOpen)
            {
                DrawExploreFallbackInfoCard(
                    new Rect(sideInnerX, cursor, sideInnerW, rail.NextHeight),
                    "NEXT",
                    ExplorationHudFallbackLayoutRules.BoundedCopy(view.WaypointLine, copyWidth, ExploreHudFont(12), rail.NextMaxLines),
                    gold,
                    true);
                cursor += rail.NextHeight + rail.SectionGap;
                DrawExploreFallbackInfoCard(
                    new Rect(sideInnerX, cursor, sideInnerW, rail.SecondaryHeight),
                    "HERE",
                    ExplorationHudFallbackLayoutRules.BoundedCopy(secondaryCopy, copyWidth, ExploreHudFont(12), rail.SecondaryMaxLines),
                    gold,
                    false);
                cursor += rail.SecondaryHeight + rail.SectionGap;
                DrawExploreFallbackInfoCard(
                    new Rect(sideInnerX, cursor, sideInnerW, rail.ObjectiveHeight),
                    "OBJECTIVE",
                    ExplorationHudFallbackLayoutRules.BoundedCopy(view.ObjectiveLine, copyWidth, ExploreHudFont(12), rail.ObjectiveMaxLines),
                    teal,
                    true);
                cursor += rail.ObjectiveHeight + rail.SectionGap;
                DrawExploreFallbackInfoCard(
                    new Rect(sideInnerX, cursor, sideInnerW, rail.GrowthHeight),
                    "PROGRESS",
                    ExplorationHudFallbackLayoutRules.BoundedCopy(view.GrowthLine, copyWidth, ExploreHudFont(12), rail.GrowthMaxLines),
                    moss,
                    false);
                cursor += rail.GrowthHeight + rail.GrowthTailGap;
                GUI.Label(new Rect(sideInnerX, cursor, sideInnerW, 18f * scale), "PARTY", CenterLeftStyle(ExploreHudFont(11), Hex("e3ba63")));
                cursor += rail.PartyLabelHeight;
                for (int i = 0; i < rail.PartyCount; i++)
                {
                    DrawExploreRailPartyRow(new Rect(sideInnerX, cursor, sideInnerW, rail.PartyRowHeight), view.Party[i]);
                    cursor += rail.PartyRowStep;
                }
                float remaining = contentBottom - cursor;
                if (remaining >= 84f * scale)
                {
                    float mapH = Mathf.Min(180f * scale, remaining);
                    if (remaining >= 144f * scale) mapH = Mathf.Min(mapH, remaining * 0.60f);
                    DrawExploreMiniMap(new Rect(sideInnerX, cursor, sideInnerW, mapH));
                    cursor += mapH + rail.SectionGap;
                    remaining = contentBottom - cursor;
                }
                DrawExploreFallbackLatest(sideInnerX, sideInnerW, ref cursor, contentBottom, view.Logs);
            }
            else
            {
                DrawExploreFallbackInfoCard(
                    new Rect(sideInnerX, cursor, sideInnerW, rail.NextHeight),
                    "NEXT",
                    ExplorationHudFallbackLayoutRules.BoundedCopy(view.WaypointLine, copyWidth, ExploreHudFont(12), rail.NextMaxLines),
                    gold,
                    true);
                cursor += rail.NextHeight + rail.SectionGap;
                DrawExploreFallbackInfoCard(
                    new Rect(sideInnerX, cursor, sideInnerW, rail.ObjectiveHeight),
                    "OBJECTIVE",
                    ExplorationHudFallbackLayoutRules.BoundedCopy(view.ObjectiveSummary, copyWidth, ExploreHudFont(12), rail.ObjectiveMaxLines),
                    teal,
                    true);
                cursor += rail.ObjectiveHeight + rail.SectionGap;
                DrawExploreFallbackInfoCard(
                    new Rect(sideInnerX, cursor, sideInnerW, rail.SecondaryHeight),
                    "NEARBY",
                    ExplorationHudFallbackLayoutRules.BoundedCopy(view.NearbyLine, copyWidth, ExploreHudFont(12), rail.SecondaryMaxLines),
                    moss,
                    false);
                cursor += rail.SecondaryHeight + rail.SectionGap;
                if (view.HasAction)
                {
                    Rect ready = new Rect(sideInnerX, cursor, sideInnerW, rail.ActionHeight);
                    if (DrawExploreFallbackAction(ready, view.ActionLabel, view.ActionTarget, true)) UseNearbyExploreObject();
                    cursor += ready.height + rail.PostActionGap;
                }

                GUI.Label(new Rect(sideInnerX, cursor, sideInnerW, 18f * scale), "PARTY", CenterLeftStyle(ExploreHudFont(11), Hex("e3ba63")));
                cursor += rail.PartyLabelHeight;
                for (int i = 0; i < rail.PartyCount; i++)
                {
                    DrawExploreRailPartyRow(new Rect(sideInnerX, cursor, sideInnerW, rail.PartyRowHeight), view.Party[i]);
                    cursor += rail.PartyRowStep;
                }
                float mapH = contentBottom - cursor;
                if (mapH >= 84f * scale)
                {
                    DrawExploreMiniMap(new Rect(sideInnerX, cursor, sideInnerW, Mathf.Min(180f * scale, mapH)));
                }
            }

            if (DrawExploreFallbackToggleButton(detailsButton, view.DetailsOpen)) ToggleExploreHud();

            DrawRect(geometry.Command, Hex("060a0c", 0.99f));
            DrawRect(new Rect(geometry.Command.x, geometry.Command.y, geometry.Command.width, 3f * scale), Hex("58462c", 0.78f));
            DrawBorder(geometry.Command, Hex("52605c", 0.78f), 1);
            ExplorationHudFallbackCommandLayout commandLayout = ExplorationHudFallbackLayoutRules.CalculateCommands(geometry.Command, scale);
            DrawExploreFallbackGroupSeparator(geometry.Command, commandLayout.ContextSeparatorX, ember);
            DrawExploreFallbackGroupSeparator(geometry.Command, commandLayout.UtilitySeparatorX, teal);
            bool oldEnabled = GUI.enabled;
            GUI.enabled = oldEnabled && view.HasAction;
            if (DrawExploreFallbackAction(commandLayout.Action, view.HasAction ? view.ActionLabel : "Explore", view.HasAction ? view.ActionTarget : "No nearby action", view.HasAction)) UseNearbyExploreObject();
            GUI.enabled = oldEnabled;
            GUI.enabled = oldEnabled && state.Supplies > 0;
            if (DrawExploreFallbackCommand(commandLayout.Camp, "Camp", "R", "camp", false)) Camp();
            GUI.enabled = oldEnabled;
            if (DrawExploreFallbackCommand(commandLayout.Recall, "Recall", "Y", "magic", false)) RecallToTempleSquare();
            bool canSurveyFrontier = CanSurveyGlassAndAshFrontier();
            GUI.enabled = oldEnabled && (CanDescend() || canSurveyFrontier);
            string descendLabel = canSurveyFrontier
                ? "Survey"
                : commandLayout.Descend.width < 112f * scale ? "Down" : "Descend";
            if (DrawExploreFallbackCommand(commandLayout.Descend, descendLabel, "T", "arrow", false)) Descend();
            GUI.enabled = oldEnabled;
            GUI.enabled = oldEnabled && state.Elixirs > 0;
            if (DrawExploreFallbackCommand(commandLayout.Elixir, "Elixir", "H", "hp", false)) UseElixir();
            GUI.enabled = oldEnabled;
            if (DrawExploreFallbackCommand(commandLayout.Map, exploreWideView ? "Local" : "Region", "Tab", "scroll", true)) ToggleExploreView();
            if (DrawExploreFallbackCommand(commandLayout.Journal, "Journal", "J", "timeline", true)) ToggleArmory(ArmoryTab.Journal);
            if (DrawExploreFallbackCommand(commandLayout.Party, "Party", "F", "party", true)) ToggleArmory(ArmoryTab.Party);
            if (DrawExploreFallbackCommand(commandLayout.Menu, "Menu", "Esc", "queue", true)) OpenPauseMenu();
            GUI.enabled = previousGuiEnabled;
        }

        private void DrawExploreFallbackResource(Rect rect, string label, string value, Color accent)
        {
            float scale = ExplorationHudScreenLayout.InterfaceScale(Screen.width, Screen.height);
            DrawRect(rect, Hex("0b1114", 0.98f));
            DrawRect(new Rect(rect.x, rect.y, 3f * scale, rect.height), accent.WithAlpha(0.92f));
            DrawBorder(rect, accent.WithAlpha(0.58f), 1);
            GUI.Label(new Rect(rect.x + 8f * scale, rect.y + 2f * scale, rect.width - 14f * scale, 13f * scale), label.ToUpperInvariant(), CenterLeftStyle(ExploreHudFont(11), Hex("d0c5ae")));
            GUI.Label(new Rect(rect.x + 8f * scale, rect.y + 15f * scale, rect.width - 14f * scale, 18f * scale), value ?? "0", CenterLeftStyle(ExploreHudFont(14), ink));
        }

        private void DrawExploreFallbackGroupSeparator(Rect panel, float x, Color accent)
        {
            float scale = ExplorationHudScreenLayout.InterfaceScale(Screen.width, Screen.height);
            Rect divider = new Rect(Mathf.Round(x), panel.y + 12f * scale, Mathf.Max(1f, scale), panel.height - 24f * scale);
            DrawRect(divider, line.WithAlpha(0.52f));
            DrawRect(new Rect(divider.x, divider.y, divider.width, 10f * scale), accent.WithAlpha(0.72f));
        }

        private bool DrawExploreFallbackCommand(Rect rect, string label, string hotkey, string icon, bool utility)
        {
            float scale = ExplorationHudScreenLayout.InterfaceScale(Screen.width, Screen.height);
            Color accent = ExploreCommandAccent(label);
            bool enabled = GUI.enabled;
            DrawRect(rect, enabled
                ? (utility ? Hex("0e1518", 0.97f) : Hex("121a1e", 0.99f))
                : Hex("080d0f", 0.90f));
            DrawRect(new Rect(rect.x, rect.y, rect.width, 3f * scale), accent.WithAlpha(enabled ? (utility ? 0.48f : 0.72f) : 0.12f));
            DrawBorder(rect, enabled ? accent.WithAlpha(utility ? 0.34f : 0.46f) : line.WithAlpha(0.16f), 1);
            float iconSize = Mathf.Clamp(rect.height - 16f * scale, 30f * scale, 36f * scale);
            Rect iconRect = new Rect(rect.x + 7f * scale, rect.y + (rect.height - iconSize) * 0.5f, iconSize, iconSize);
            DrawRect(Pad(iconRect, -2f * scale), enabled ? Hex("070a0c", utility ? 0.68f : 0.82f) : Hex("05080a", 0.54f));
            DrawBorder(Pad(iconRect, -2f * scale), enabled ? accent.WithAlpha(utility ? 0.36f : 0.54f) : line.WithAlpha(0.14f), 1);
            int artIndex = ExploreCommandArtIndex(label);
            Color iconTint = enabled ? (utility ? Hex("d8d5cc") : Color.white) : muted.WithAlpha(0.40f);
            if (!TryDrawWorldMapUiAtlasIcon(iconRect, artIndex, iconTint))
            {
                DrawTinyUiIcon(iconRect, icon, enabled ? accent : muted.WithAlpha(0.42f));
            }
            float textX = iconRect.xMax + 6f * scale;
            float textW = Mathf.Max(24f * scale, rect.xMax - textX - 6f * scale);
            Color labelColor = enabled ? (utility ? Hex("d0c5ae") : ink) : muted.WithAlpha(0.60f);
            Color hotkeyColor = enabled ? accent : muted.WithAlpha(0.50f);
            GUI.Label(new Rect(textX, rect.y + 6f * scale, textW, 21f * scale), FitText(label, textW, CenterLeftStyle(ExploreHudFont(13), labelColor)), CenterLeftStyle(ExploreHudFont(13), labelColor));
            GUI.Label(new Rect(textX, rect.y + 29f * scale, textW, 16f * scale), hotkey, CenterLeftStyle(ExploreHudFont(11), hotkeyColor));
            return GUI.Button(rect, GUIContent.none, GUIStyle.none);
        }

        private int ExploreCommandArtIndex(string label)
        {
            switch ((label ?? "").Trim().ToLowerInvariant())
            {
                case "camp": return 0;
                case "recall": return 1;
                case "descend": return 2;
                case "down": return 2;
                case "elixir": return 3;
                case "local":
                case "region": return 4;
                case "journal": return 6;
                case "party": return 9;
                case "menu": return 15;
                default: return 8;
            }
        }

        private Color ExploreCommandAccent(string label)
        {
            switch ((label ?? "").Trim().ToLowerInvariant())
            {
                case "camp": return ember;
                case "recall": return frost;
                case "descend": return gold;
                case "down": return gold;
                case "elixir": return blood;
                case "local":
                case "region": return teal;
                case "journal": return gold;
                case "party": return moss;
                default: return line;
            }
        }

        private void DrawExploreRailPartyRow(Rect row, ExplorationHudPartyMemberView member)
        {
            if (member == null) return;
            float scale = ExplorationHudScreenLayout.InterfaceScale(Screen.width, Screen.height);
            Color accent = string.IsNullOrWhiteSpace(member.ColorHex) ? teal : Hex(member.ColorHex);
            DrawRect(row, Hex("10161a", 0.98f));
            DrawRect(new Rect(row.x, row.y, 4f * scale, row.height), accent.WithAlpha(0.96f));
            DrawBorder(row, line.WithAlpha(0.45f), 1);
            float nameW = Mathf.Clamp(row.width * 0.43f, 90f * scale, 120f * scale);
            GUI.Label(new Rect(row.x + 10f * scale, row.y + 2f * scale, nameW - 12f * scale, 16f * scale), FitText(member.Name, nameW - 12f * scale, CenterLeftStyle(ExploreHudFont(12), ink)), CenterLeftStyle(ExploreHudFont(12), ink));
            GUI.Label(new Rect(row.x + 10f * scale, row.y + 17f * scale, nameW - 12f * scale, 13f * scale), FitText(member.ClassLine, nameW - 12f * scale, CenterLeftStyle(ExploreHudFont(11), Hex("d0c5ae"))), CenterLeftStyle(ExploreHudFont(11), Hex("d0c5ae")));
            float barX = row.x + nameW;
            float barW = row.xMax - barX - 8f * scale;
            DrawExploreRailBar(new Rect(barX, row.y + 6f * scale, barW, 8f * scale), member.Hp, member.MaxHp, blood);
            GUI.Label(new Rect(barX + 4f * scale, row.y + 2f * scale, barW - 8f * scale, 15f * scale), $"HP {member.Hp}/{member.MaxHp}", CenterRightStyle(ExploreHudFont(11), ink));
            if (member.MaxMana > 0)
            {
                DrawExploreRailBar(new Rect(barX, row.y + 19f * scale, barW, 8f * scale), member.Mana, member.MaxMana, teal);
                GUI.Label(new Rect(barX + 4f * scale, row.y + 15f * scale, barW - 8f * scale, 15f * scale), $"MP {member.Mana}/{member.MaxMana}", CenterRightStyle(ExploreHudFont(11), ink));
            }
        }

        private void DrawExploreRailBar(Rect rect, int value, int maximum, Color fill)
        {
            DrawRect(rect, Hex("030405", 0.90f));
            float ratio = maximum <= 0 ? 0f : Mathf.Clamp01(value / (float)maximum);
            if (ratio > 0f) DrawRect(new Rect(rect.x, rect.y, rect.width * ratio, rect.height), fill.WithAlpha(0.92f));
        }

        private int ExploreHudFont(int baseSize)
        {
            return ExplorationHudScreenLayout.FontSize(baseSize, Screen.width, Screen.height);
        }

        private void DrawExploreFallbackInfoCard(Rect rect, string eyebrow, string text, Color accent, bool emphasized)
        {
            float scale = ExplorationHudScreenLayout.InterfaceScale(Screen.width, Screen.height);
            DrawRect(rect, emphasized ? Hex("11191b", 0.98f) : Hex("0c1215", 0.96f));
            DrawRect(new Rect(rect.x, rect.y, 3f * scale, rect.height), accent.WithAlpha(emphasized ? 0.96f : 0.72f));
            DrawBorder(rect, accent.WithAlpha(emphasized ? 0.56f : 0.30f), 1);
            GUI.Label(new Rect(rect.x + 9f * scale, rect.y + 4f * scale, rect.width - 18f * scale, 15f * scale), eyebrow ?? "", CenterLeftStyle(ExploreHudFont(11), accent));
            GUI.Label(
                new Rect(rect.x + 9f * scale, rect.y + 21f * scale, rect.width - 18f * scale, Mathf.Max(16f * scale, rect.height - 26f * scale)),
                text ?? "",
                WrapStyle(ExploreHudFont(12), emphasized ? ink : Hex("d0c5ae")));
        }

        private bool DrawExploreFallbackAction(Rect rect, string actionLabel, string actionTarget, bool available)
        {
            float scale = ExplorationHudScreenLayout.InterfaceScale(Screen.width, Screen.height);
            Color accent = available ? teal : muted;
            DrawRect(rect, available ? Hex("10201f", 0.99f) : Hex("080d0f", 0.90f));
            DrawRect(new Rect(rect.x, rect.y, 4f * scale, rect.height), accent.WithAlpha(available ? 0.94f : 0.18f));
            DrawBorder(rect, accent.WithAlpha(available ? 0.88f : 0.16f), available ? 2 : 1);
            Rect iconRect = new Rect(rect.x + 10f * scale, rect.y + 10f * scale, 32f * scale, 32f * scale);
            int actionArtIndex = ExploreActionArtIndex(actionLabel, actionTarget);
            if (!TryDrawWorldMapUiAtlasIcon(iconRect, actionArtIndex, available ? Color.white : muted.WithAlpha(0.38f)))
            {
                DrawTinyUiIcon(iconRect, available ? "hand" : "scroll", available ? teal : muted.WithAlpha(0.40f));
            }
            float keyW = 34f * scale;
            Rect key = new Rect(rect.xMax - keyW - 10f * scale, rect.y + 10f * scale, keyW, 32f * scale);
            DrawRect(key, Hex("05090a", 0.94f));
            DrawBorder(key, accent.WithAlpha(available ? 0.76f : 0.18f), 1);
            GUI.Label(key, available ? "E" : "--", CenterStyle(ExploreHudFont(13), available ? ink : muted.WithAlpha(0.52f)));
            float textX = iconRect.xMax + 10f * scale;
            float textW = Mathf.Max(60f * scale, key.x - textX - 8f * scale);
            Color labelColor = available ? accent : muted.WithAlpha(0.60f);
            Color targetColor = available ? ink : muted.WithAlpha(0.52f);
            GUI.Label(new Rect(textX, rect.y + 5f * scale, textW, 20f * scale), FitText((actionLabel ?? "USE").ToUpperInvariant(), textW, CenterLeftStyle(ExploreHudFont(13), labelColor)), CenterLeftStyle(ExploreHudFont(13), labelColor));
            GUI.Label(new Rect(textX, rect.y + 26f * scale, textW, 21f * scale), FitText(actionTarget ?? "", textW, CenterLeftStyle(ExploreHudFont(12), targetColor)), CenterLeftStyle(ExploreHudFont(12), targetColor));
            return GUI.Button(rect, GUIContent.none, GUIStyle.none);
        }

        private int ExploreActionArtIndex(string actionLabel, string actionTarget)
        {
            string identity = ((actionLabel ?? "") + " " + (actionTarget ?? "")).ToLowerInvariant();
            if (identity.Contains("talk") || identity.Contains("speak")) return 12;
            if (identity.Contains("sewer") || identity.Contains("cistern")) return 18;
            if (identity.Contains("cave")) return 19;
            if (identity.Contains("gate") || identity.Contains("enter")) return 17;
            if (identity.Contains("market") || identity.Contains("trade") || identity.Contains("shop")) return 11;
            if (identity.Contains("descend") || identity.Contains("stairs")) return 2;
            if (identity.Contains("danger") || identity.Contains("encounter")) return 13;
            return 8;
        }

        private bool DrawExploreFallbackToggleButton(Rect rect, bool detailsOpen)
        {
            float scale = ExplorationHudScreenLayout.InterfaceScale(Screen.width, Screen.height);
            DrawRect(rect, Hex("151b20", 0.99f));
            DrawRect(new Rect(rect.x, rect.y, 4f * scale, rect.height), teal.WithAlpha(0.92f));
            DrawBorder(rect, teal.WithAlpha(0.68f), 1);
            Rect iconRect = new Rect(rect.x + 8f * scale, rect.y + 5f * scale, rect.height - 10f * scale, rect.height - 10f * scale);
            if (!TryDrawWorldMapUiAtlasIcon(iconRect, 5, Color.white))
            {
                DrawTinyUiIcon(iconRect, "scroll", teal);
            }
            GUI.Label(
                new Rect(iconRect.xMax + 7f * scale, rect.y, rect.width - iconRect.width - 58f * scale, rect.height),
                detailsOpen ? "BACK TO MAP" : "LOCATION DETAILS",
                CenterLeftStyle(ExploreHudFont(13), ink));
            Rect key = new Rect(rect.xMax - 38f * scale, rect.y + 5f * scale, 30f * scale, rect.height - 10f * scale);
            DrawRect(key, Hex("05090a", 0.94f));
            DrawBorder(key, teal.WithAlpha(0.72f), 1);
            GUI.Label(key, "Q", CenterStyle(ExploreHudFont(12), Hex("e3ba63")));
            return GUI.Button(rect, GUIContent.none, GUIStyle.none);
        }

        private void DrawExploreFallbackLatest(float x, float width, ref float cursor, float bottom, IReadOnlyList<ExplorationHudLogView> logs)
        {
            if (logs == null || logs.Count == 0) return;
            float scale = ExplorationHudScreenLayout.InterfaceScale(Screen.width, Screen.height);
            float available = bottom - cursor;
            if (available < 48f * scale) return;
            GUI.Label(new Rect(x, cursor, width, 18f * scale), "LATEST", CenterLeftStyle(ExploreHudFont(11), Hex("e3ba63")));
            cursor += 21f * scale;
            int capacity = Mathf.Min(logs.Count, Mathf.FloorToInt((bottom - cursor + 4f * scale) / (30f * scale)));
            for (int i = 0; i < capacity; i++)
            {
                ExplorationHudLogView log = logs[i];
                Color stripe = string.Equals(log.Tone, "Warn", StringComparison.OrdinalIgnoreCase)
                    ? ember
                    : string.Equals(log.Tone, "Good", StringComparison.OrdinalIgnoreCase) ? teal : moss;
                Rect row = new Rect(x, cursor, width, 26f * scale);
                DrawRect(row, Hex("10161a", 0.96f));
                DrawRect(new Rect(row.x, row.y, 3f * scale, row.height), stripe.WithAlpha(0.92f));
                GUI.Label(new Rect(row.x + 9f * scale, row.y + 3f * scale, row.width - 16f * scale, row.height - 6f * scale), FitText(log.Text, row.width - 16f * scale, CenterLeftStyle(ExploreHudFont(11), Hex("d0c5ae"))), CenterLeftStyle(ExploreHudFont(11), Hex("d0c5ae")));
                cursor += 30f * scale;
            }
        }

        private GUIStyle WrapStyle(int size, Color color)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                fontSize = size,
                wordWrap = true,
                alignment = TextAnchor.UpperLeft,
                clipping = TextClipping.Clip
            };
            style.normal.textColor = color;
            return style;
        }

        private string ExplorationHudRefreshKey()
        {
            if (state == null) return "empty";
            int hash = 17;
            hash = unchecked(hash * 31 + state.PlayerX);
            hash = unchecked(hash * 31 + state.PlayerY);
            hash = unchecked(hash * 31 + state.Depth);
            hash = unchecked(hash * 31 + state.Gold);
            hash = unchecked(hash * 31 + state.Supplies);
            hash = unchecked(hash * 31 + state.Elixirs);
            hash = unchecked(hash * 31 + (exploreHudCollapsed ? 1 : 0));
            hash = unchecked(hash * 31 + (exploreWideView ? 1 : 0));
            hash = unchecked(hash * 31 + (exploreHoverLookLine ?? "").GetHashCode());
            hash = unchecked(hash * 31 + (state.ActiveStory ?? "").GetHashCode());
            hash = unchecked(hash * 31 + (state.ActiveRouteWaypointKey ?? "").GetHashCode());
            if (state.Log != null && state.Log.Count > 0) hash = unchecked(hash * 31 + (state.Log[0].Text ?? "").GetHashCode());
            if (state.Party != null)
            {
                foreach (PartyMember member in state.Party)
                {
                    if (member == null) continue;
                    hash = unchecked(hash * 31 + member.Hp);
                    hash = unchecked(hash * 31 + member.Mana);
                    hash = unchecked(hash * 31 + member.Level);
                }
            }
            return "explore=" + hash;
        }

        private ExplorationHudView BuildExplorationHudView()
        {
            if (state == null)
            {
                return new ExplorationHudView
                {
                    Title = GameTitle,
                    RouteLine = GameSubtitle,
                    FocusHint = "Preparing the road...",
                    ZoneName = HomeTownName,
                    ZoneDetail = "Muster not ready",
                    LookLine = "Preparing the road...",
                    ObjectiveLine = "",
                    ObjectiveSummary = "Preparing the road...",
                    WaypointLine = "No marked route yet.",
                    NearbyLine = "Nothing nearby.",
                    GrowthLine = "",
                    ActionLabel = "No Action",
                    ActionTarget = "Nothing nearby"
                };
            }

            ExplorationInteraction interaction = CurrentExploreInteraction();
            WorldZone zone = state?.Map == null ? null : ZoneAt(state.PlayerX, state.PlayerY);
            MapObject obj = state?.Map == null ? null : ObjectAt(state.Map, state.PlayerX, state.PlayerY);
            bool hasRegionalSite = TryRegionalSiteAt(
                state?.Map,
                state.PlayerX,
                state.PlayerY,
                out WorldMapSite regionalSite);
            string nearbyAction = ExploreNearbyActionLine();
            string lookLine;
            if (!string.IsNullOrEmpty(exploreHoverLookLine))
            {
                lookLine = "Look: " + exploreHoverLookLine.Replace("\n", " / ");
            }
            else if (!string.IsNullOrEmpty(nearbyAction))
            {
                lookLine = nearbyAction;
            }
            else if (obj != null)
            {
                lookLine = $"{ObjectName(obj)}: {ObjectHint(obj)}.";
            }
            else if (ShouldShowMidgaardTracker())
            {
                lookLine = MidgaardWayfindingCompactLine();
            }
            else
            {
                lookLine = "Nearby: roads, fog, and old markers.";
            }

            return new ExplorationHudView
            {
                Title = GameTitle,
                RouteLine = $"{StoryChapterTitle()} / D{state.Depth}",
                FocusHint = $"{ExploreViewLabel()} / {ExploreHudHint()}",
                Gold = state.Gold.ToString(),
                Supplies = state.Supplies.ToString(),
                Elixirs = state.Elixirs.ToString(),
                DetailsOpen = !exploreHudCollapsed,
                ViewLabel = ExploreViewLabel(),
                ZoneName = hasRegionalSite ? regionalSite.Name : zone?.Name ?? HomeTownName,
                ZoneDetail = ExploreLocationDetail(zone, hasRegionalSite),
                DangerLabel = zone == null ? "" : TravelDangerLabel(zone),
                LookLine = lookLine,
                ObjectiveLine = string.IsNullOrEmpty(state.ActiveStory) ? "Follow the road and mark what the party learns." : state.ActiveStory,
                ObjectiveSummary = ExploreObjectiveSummaryLine(),
                WaypointLine = ExploreWaypointLine(),
                NearbyLine = ExploreNearbySummaryLine(),
                GrowthLine = PartyGrowthLine(),
                HasAction = interaction.HasTarget,
                ActionLabel = interaction.HasTarget ? interaction.Verb : "No Action",
                ActionTarget = interaction.HasTarget ? interaction.TargetName : "Nothing nearby",
                Party = BuildExplorationHudPartyViews(),
                Logs = BuildExplorationHudLogViews()
            };
        }

        private string ExploreLocationDetail(
            WorldZone zone,
            bool hasRegionalSite = false)
        {
            if (zone == null || state?.Map == null) return "";
            if (state.Depth == 1 && IsMidgaardCityCell(state.PlayerX, state.PlayerY, state.Map, state.Depth))
            {
                string district = MidgaardDistrictRules.DistrictAtOffset(
                    state.PlayerX - state.Map.StartX,
                    state.PlayerY - state.Map.StartY);
                return $"{district} / {ExploreGroundName(state.PlayerX, state.PlayerY)}";
            }
            if (hasRegionalSite)
            {
                if (TryRegionalSiteAt(
                        state.Map,
                        state.PlayerX,
                        state.PlayerY,
                        out WorldMapSite site)
                    && WorldSiteInteractionRules.TryGet(site.Id, out WorldSiteInteractionProfile interaction))
                {
                    bool rewardClaimed = WorldSiteInteractionRules.RewardClaimed(
                        state.StoryFlags,
                        state.Depth,
                        site.Id);
                    string status = rewardClaimed ? "SERVICE" : "REWARD READY";
                    return $"{status} / {interaction.ServiceName} / {ExploreGroundName(state.PlayerX, state.PlayerY)}";
                }
                return $"{zone.Title} landmark / {ExploreGroundName(state.PlayerX, state.PlayerY)}";
            }
            return $"{zone.Title} / {ExploreGroundName(state.PlayerX, state.PlayerY)}";
        }

        private string ExploreObjectiveSummaryLine()
        {
            if (state == null) return "Prepare the party.";
            if (state.Depth == 1
                && state.Map != null
                && !HasStoryFlag(StoryFlags.MidgaardGrandHearthDeparted)
                && string.Equals(
                    MidgaardInteriorIdAt(state.PlayerX, state.PlayerY, state.Map, state.Depth),
                    MidgaardInteriorRules.GrandHearthZoneId,
                    StringComparison.Ordinal))
            {
                return "Leave Town Hall through the storm doors to begin the journey.";
            }
            if (state.Depth == 1 && TryCurrentMidgaardObjectiveType(out ObjectType target))
            {
                if (target == ObjectType.KingHall) return "Meet King Halvard and accept the sewer contract.";
                if (target == ObjectType.Sewer && ContentSetCatalog.IsSewerSlice(activeContentSet))
                {
                    int cleared = ContentSetCatalog.SewerSliceClearedCount(state.StoryFlags);
                    if (cleared >= 2 && !ContentSetCatalog.HasSewerSafeRoomChoice(state.StoryFlags))
                    {
                        return "Choose one service weapon before entering the Cistern Den.";
                    }
                    EncounterDefinition next = ContentSetCatalog.SewerSliceEncounterForProgress(cleared);
                    return $"Clear {next.Banner} ({cleared + 1}/{ContentSetCatalog.SewerSliceEncounters.Count}).";
                }
                if (target == ObjectType.Armorer) return "Bring three sewer proof bundles to Borin for starter armor.";
                if (target == ObjectType.OldRoadScout)
                {
                    if (ContentSetCatalog.GlassAndAshComplete(state.StoryFlags)
                        && !HasStoryFlag(StoryFlags.GlassAndAshDebriefed))
                    {
                        return "Bring the Emberglass key to Yara and close the Glass Road expedition.";
                    }
                    return HasStoryFlag(StoryFlags.GlassAndAshFrontierSurveyed)
                        && !HasStoryFlag(StoryFlags.GlassAndAshExpeditionAccepted)
                            ? "Bring the frontier survey to Yara and plan the Glass Road crossing."
                            : "Ask the Old Road scout about the newly marked routes.";
                }
                if (target == ObjectType.Stairs)
                {
                    return HasGlassAndAshStoryProgress()
                        ? "Follow the Old Road east and regain the Red Gate passage for Yara's expedition."
                        : "Follow the Old Road east through Lanternless Cross toward Dusk Market.";
                }
            }

            if (TryBoneRoadObjectiveSummary(out string boneRoadObjective))
            {
                return boneRoadObjective;
            }

            string objective = string.IsNullOrWhiteSpace(state.ActiveStory)
                ? StoryObjectiveForDepth(state.Depth)
                : state.ActiveStory;
            int chapterBreak = objective.IndexOf(". ", StringComparison.Ordinal);
            return chapterBreak >= 0 && chapterBreak + 2 < objective.Length
                ? objective.Substring(chapterBreak + 2)
                : objective;
        }

        private string ExploreWaypointLine()
        {
            if (state?.Map?.Objects == null) return "The route is not ready.";
            ExploreGuidancePlan plan = CurrentExploreGuidancePlan();
            if (!plan.HasTarget)
            {
                return ExplorationGuidanceRules.Route("", "", -1);
            }

            if (plan.Immediate)
            {
                return ExplorationGuidanceRules.UseNow(
                    plan.TargetName,
                    plan.Verb,
                    plan.InteriorExit);
            }

            string direction = ActiveRouteWaypointFirstDirection(plan.Path);
            int stepCount = plan.Path == null || plan.Path.Count == 0
                ? -1
                : Mathf.Max(0, plan.Path.Count - 1);
            return ExplorationGuidanceRules.Route(
                plan.TargetName,
                direction,
                stepCount,
                plan.MarkedWaypoint,
                plan.RouteBlocked);
        }

        private ExploreGuidancePlan CurrentExploreGuidancePlan()
        {
            string cacheKey = ExploreGuidancePlanStateKey();
            if (exploreGuidancePlanCache != null
                && string.Equals(exploreGuidancePlanCacheKey, cacheKey, StringComparison.Ordinal))
            {
                return exploreGuidancePlanCache;
            }

            ExploreGuidancePlan plan = BuildExploreGuidancePlan();
            exploreGuidancePlanCache = plan;
            exploreGuidancePlanCacheKey = cacheKey;
            return plan;
        }

        private IReadOnlyList<Point> CurrentExploreGuidancePath()
        {
            return CurrentExploreGuidancePlan().Path;
        }

        private string CurrentExploreGuidanceTargetName()
        {
            return CurrentExploreGuidancePlan().TargetName;
        }

        private bool CurrentExploreGuidanceIsMarked()
        {
            return CurrentExploreGuidancePlan().MarkedWaypoint;
        }

        private bool CurrentExploreGuidanceIsBlocked()
        {
            return CurrentExploreGuidancePlan().RouteBlocked;
        }

        private bool CurrentExploreGuidanceIsImmediate()
        {
            return CurrentExploreGuidancePlan().Immediate;
        }

        private bool CurrentExploreGuidanceIsInteriorExit()
        {
            return CurrentExploreGuidancePlan().InteriorExit;
        }

        private Point CurrentExploreGuidanceTargetPoint()
        {
            ExploreGuidancePlan plan = CurrentExploreGuidancePlan();
            return plan.HasTarget ? new Point(plan.TargetX, plan.TargetY) : null;
        }

        private ExploreGuidancePlan BuildExploreGuidancePlan()
        {
            if (state?.Map?.Objects == null) return new ExploreGuidancePlan();

            if (TryActiveRouteWaypoint(out WorldMapJunction activeWaypoint))
            {
                IReadOnlyList<Point> waypointPath = ActiveRouteWaypointPath();
                return new ExploreGuidancePlan
                {
                    TargetName = activeWaypoint.Name,
                    TargetX = activeWaypoint.X,
                    TargetY = activeWaypoint.Y,
                    Path = waypointPath == null ? Array.Empty<Point>() : waypointPath.ToArray(),
                    MarkedWaypoint = true,
                    RouteBlocked = waypointPath == null || waypointPath.Count == 0
                };
            }

            ExplorationInteraction interaction = CurrentExploreInteraction();
            if (interaction.HasTarget && IsCurrentMidgaardObjective(interaction.Target))
            {
                return ImmediateExploreGuidancePlan(interaction);
            }

            List<MapObject> objectiveTargets = state.Map.Objects
                .Where(obj => obj != null && IsCurrentMidgaardObjective(obj))
                .ToList();
            if (TryNearestReachableExploreTarget(objectiveTargets, out MapObject objective, out IReadOnlyList<Point> objectivePath))
            {
                return TravelExploreGuidancePlan(objective, objectivePath);
            }

            if (ObjectiveIsOutsideCurrentInterior(objectiveTargets)
                && TryCurrentInteriorExit(out MapObject exit, out IReadOnlyList<Point> exitPath))
            {
                return TravelExploreGuidancePlan(exit, exitPath);
            }

            if (TryCurrentMidgaardObjectiveType(out _))
            {
                if (objectiveTargets.Count == 0) return new ExploreGuidancePlan();
                MapObject blockedObjective = objectiveTargets[0];
                return new ExploreGuidancePlan
                {
                    TargetName = ObjectName(blockedObjective),
                    TargetObject = blockedObjective,
                    TargetX = blockedObjective.X,
                    TargetY = blockedObjective.Y,
                    RouteBlocked = true
                };
            }

            if (state.Depth == 2
                && !HasStoryFlag(StoryFlags.KoboldAmbushSurvived)
                && !HasStoryFlag(StoryFlags.KoboldKingDefeated)
                && TryKoboldStorySite(out WorldMapSite duskMarketSite))
            {
                MapObject duskMarket = state.Map.FindObjectById(RegionalSiteObjectId(duskMarketSite));
                if (duskMarket != null)
                {
                    IReadOnlyList<Point> duskPath = FindLiveExplorePathToObject(duskMarket);
                    if (duskPath.Count > 0) return TravelExploreGuidancePlan(duskMarket, duskPath);
                }
            }

            if (TryBoneRoadStoryGuidancePlan(out ExploreGuidancePlan boneRoadPlan))
            {
                return boneRoadPlan;
            }

            bool preferRegionalTargets = state.Depth == 1 && !ShouldUseMidgaardWayfinding();
            IEnumerable<MapObject> eligibleTargets = state.Map.Objects
                .Where(o => o != null && IsExploreGuidanceTarget(o) && IsEligibleExploreWaypoint(o));
            if (preferRegionalTargets)
            {
                eligibleTargets = eligibleTargets.Where(o => !IsMidgaardCityCell(o.X, o.Y, state.Map, state.Depth));
            }

            var route = eligibleTargets
                .Select(o => new
                {
                    Target = o,
                    Path = (IReadOnlyList<Point>)FindLiveExplorePathToObject(o)
                })
                .Where(candidate => candidate.Path.Count > 0)
                .OrderBy(candidate => ExploreWaypointPriority(candidate.Target))
                .ThenBy(candidate => candidate.Path.Count)
                .ThenBy(candidate => candidate.Target.Y)
                .ThenBy(candidate => candidate.Target.X)
                .FirstOrDefault();
            return route == null
                ? new ExploreGuidancePlan()
                : TravelExploreGuidancePlan(route.Target, route.Path);
        }

        private ExploreGuidancePlan ImmediateExploreGuidancePlan(ExplorationInteraction interaction)
        {
            if (!interaction.HasTarget) return new ExploreGuidancePlan();
            MapObject target = interaction.Target;
            return new ExploreGuidancePlan
            {
                TargetName = string.IsNullOrWhiteSpace(interaction.TargetName)
                    ? ExploreGuidanceTargetName(target)
                    : interaction.TargetName,
                TargetObject = target,
                TargetX = target.X,
                TargetY = target.Y,
                Verb = string.IsNullOrWhiteSpace(interaction.Verb) ? "Use" : interaction.Verb,
                Immediate = true,
                InteriorExit = target.Type == ObjectType.InteriorDoor,
                Path = new[] { new Point(state.PlayerX, state.PlayerY) }
            };
        }

        private ExploreGuidancePlan TravelExploreGuidancePlan(
            MapObject target,
            IReadOnlyList<Point> path)
        {
            if (target == null) return new ExploreGuidancePlan();
            IReadOnlyList<Point> safePath = path == null ? Array.Empty<Point>() : path.ToArray();
            bool immediate = safePath.Count == 1;
            return new ExploreGuidancePlan
            {
                TargetName = ExploreGuidanceTargetName(target),
                TargetObject = target,
                TargetX = target.X,
                TargetY = target.Y,
                Path = safePath,
                Verb = immediate ? ExploreGuidanceContextVerb(target) : "",
                Immediate = immediate,
                InteriorExit = target.Type == ObjectType.InteriorDoor,
                RouteBlocked = safePath.Count == 0
            };
        }

        private bool TryBoneRoadStoryGuidancePlan(out ExploreGuidancePlan plan)
        {
            plan = null;
            if (state?.Map == null
                || !ContentSetCatalog.AllowBoneRoadChapter(activeContentSet, state.StoryFlags))
            {
                return false;
            }

            string targetId = "";
            bool glassRoadActive = HasGlassAndAshStoryProgress()
                && !ContentSetCatalog.GlassAndAshComplete(state.StoryFlags);
            if (glassRoadActive)
            {
                if (state.Depth == 1) targetId = OldRoadDescentId;
                else if (state.Depth == 2) targetId = BoneRoadPassageId;
                else if (state.Depth == 3) targetId = GlassAndAshPassageId;
                else if (state.Depth == 4)
                {
                    targetId = HasStoryFlag(StoryFlags.GlassIndexRecovered)
                        ? "regional-site:" + RedGateSealSiteId
                        : "regional-site:" + GlassLoreLibrarySiteId;
                }
            }
            else if (ContentSetCatalog.IsSewerSlice(activeContentSet)
                && HasStoryFlag(StoryFlags.GlassAndAshFrontierSurveyed))
            {
                return false;
            }
            else if (state.Depth == 1 && HasStoryFlag(StoryFlags.KoboldKingDefeated))
            {
                targetId = OldRoadDescentId;
            }
            else if (state.Depth == 2 && HasStoryFlag(StoryFlags.KoboldKingDefeated))
            {
                targetId = BoneRoadPassageId;
            }
            else if (state.Depth == 3)
            {
                if (HasStoryFlag(StoryFlags.RedGateWarningRecovered))
                {
                    if (HasStoryFlag(StoryFlags.GlassAndAshFrontierSurveyed)) return false;
                    targetId = GlassAndAshPassageId;
                }
                else
                {
                    targetId = HasStoryFlag(StoryFlags.GloamWardenDefeated)
                        ? "regional-site:red-gate-seal"
                        : "regional-site:gloam-deep-crypt";
                }
            }

            if (string.IsNullOrWhiteSpace(targetId)) return false;
            MapObject target = state.Map.FindObjectById(targetId);
            if (target == null) return false;

            IReadOnlyList<Point> path = FindLiveExplorePathToObject(target);
            if (path.Count > 0)
            {
                plan = TravelExploreGuidancePlan(target, path);
                return true;
            }

            plan = new ExploreGuidancePlan
            {
                TargetName = ExploreGuidanceTargetName(target),
                TargetObject = target,
                TargetX = target.X,
                TargetY = target.Y,
                RouteBlocked = true
            };
            return true;
        }

        private bool TryBoneRoadObjectiveSummary(out string objective)
        {
            objective = "";
            if (state == null
                || !ContentSetCatalog.AllowBoneRoadChapter(activeContentSet, state.StoryFlags))
            {
                return false;
            }

            if (ShouldPreserveAdvancedFullPrototypeStory())
            {
                objective = AdvancedFullPrototypeObjective();
                return true;
            }

            if (ContentSetCatalog.GlassAndAshComplete(state.StoryFlags))
            {
                objective = HasStoryFlag(StoryFlags.GlassAndAshDebriefed)
                    ? "Chapter IV is complete. Yara copied the Emberglass key; no safe road beyond the far seal is charted yet."
                    : "Chapter IV is complete. Recall to Midgaard and bring Yara the Emberglass key.";
                return true;
            }

            if (HasGlassAndAshStoryProgress())
            {
                if (state.Depth <= 1) objective = "Take the Old Road east and regain the surveyed Red Gate passage.";
                else if (state.Depth == 2) objective = "Pass beneath Varkh's hall and regain the Bone Road.";
                else if (state.Depth == 3) objective = "Cross the surveyed Glass-and-Ash frontier at the Red Gate Seal.";
                else if (HasStoryFlag(StoryFlags.GlassIndexRecovered)) objective = "Carry the Mirror Index south-east and break the Ashen Pact at the far seal.";
                else if (HasStoryFlag(StoryFlags.GlasswardAmbushDefeated)) objective = "Return to the Glass Lore Library and recover its Mirror Index.";
                else objective = "Reach the Glass Lore Library and break the drow levy on its approach.";
                return true;
            }

            if (ContentSetCatalog.IsSewerSlice(activeContentSet)
                && HasStoryFlag(StoryFlags.GlassAndAshFrontierSurveyed))
            {
                objective = "Chapter III is complete. Return to Midgaard and ask Yara to plan the Glass Road crossing.";
                return true;
            }

            if (state.Depth == 1 && HasStoryFlag(StoryFlags.KoboldKingDefeated))
            {
                objective = "Return to Midgaard's eastbound Old Road and resume the march beyond Varkh's hall.";
                return true;
            }
            if (state.Depth == 2 && HasStoryFlag(StoryFlags.KoboldKingDefeated))
            {
                objective = "Take Varkh's east passage onto the Bone Road.";
                return true;
            }
            if (state.Depth != 3) return false;

            if (HasStoryFlag(StoryFlags.RedGateWarningRecovered))
            {
                objective = HasStoryFlag(StoryFlags.GlassAndAshFrontierSurveyed)
                    ? "Chapter III is complete. Return to Midgaard and ask Yara to plan the Glass Road crossing."
                    : "Survey the Glass-and-Ash frontier, then return to Midgaard.";
            }
            else if (HasStoryFlag(StoryFlags.GloamWardenDefeated))
            {
                objective = "Cross the Bone Road to the Red Gate Seal and recover its warning.";
            }
            else if (HasStoryFlag(StoryFlags.GloamRitualBroken))
            {
                objective = "Enter Gloam Deep Crypt and bring down the Ossuary Warden.";
            }
            else if (HasStoryFlag(StoryFlags.BoneRoadWatchDefeated))
            {
                objective = "Follow the courtward road to Gloam Deep Crypt and break the marrow ritual.";
            }
            else if (HasStoryFlag(StoryFlags.BoneRoadWatchSprung))
            {
                objective = "Hold the Bone Road and defeat the grave-scout watch.";
            }
            else
            {
                objective = HasStoryFlag(StoryFlags.BoneRoadEntered)
                    ? "Advance along the Bone Road and find the grave-scout watch."
                    : "Enter the Bone Road through Varkh's east passage.";
            }
            return true;
        }

        private string ExploreGuidanceTargetName(MapObject target)
        {
            if (target == null) return "Road marker";
            if (string.Equals(target.Id, OldRoadDescentId, StringComparison.Ordinal)) return "Eastbound Old Road";
            if (string.Equals(target.Id, BoneRoadPassageId, StringComparison.Ordinal)) return "Bone Road Passage";
            if (string.Equals(target.Id, GlassAndAshPassageId, StringComparison.Ordinal))
            {
                return HasStoryFlag(StoryFlags.GlassAndAshExpeditionAccepted)
                    ? "Glass Road Crossing"
                    : "Glass-and-Ash Frontier";
            }
            return ObjectName(target);
        }

        private string ExploreGuidanceContextVerb(MapObject target)
        {
            if (target == null) return "Use";
            if (string.Equals(target.Id, OldRoadDescentId, StringComparison.Ordinal)) return "Travel east";
            if (string.Equals(target.Id, BoneRoadPassageId, StringComparison.Ordinal)) return "Enter Bone Road";
            if (string.Equals(target.Id, GlassAndAshPassageId, StringComparison.Ordinal))
            {
                if (ContentSetCatalog.GlassAndAshComplete(state?.StoryFlags)) return "Revisit frontier";
                return HasStoryFlag(StoryFlags.GlassAndAshExpeditionAccepted) ? "Cross frontier" : "Survey frontier";
            }
            return ExploreContextVerb(target, 0, 0);
        }

        private List<Point> FindLiveExplorePath(int targetX, int targetY)
        {
            if (state?.Map == null) return new List<Point>();
            return ExplorationTraversalRules.FindPath(
                state.Map,
                state.PlayerX,
                state.PlayerY,
                targetX,
                targetY,
                CanStepExplore);
        }

        private List<Point> FindLiveExplorePathToObject(MapObject target)
        {
            if (state?.Map == null || target == null) return new List<Point>();
            return ExplorationTraversalRules.FindPathToObject(
                state.Map,
                state.PlayerX,
                state.PlayerY,
                target,
                CanStepExplore);
        }

        private string ExploreGuidancePlanStateKey()
        {
            if (state == null) return "empty";
            int hash = 17;
            hash = unchecked(hash * 31 + state.PlayerX);
            hash = unchecked(hash * 31 + state.PlayerY);
            hash = unchecked(hash * 31 + state.Depth);
            hash = unchecked(hash * 31 + state.Seed);
            hash = unchecked(hash * 31 + (state.Map == null ? 0 : state.Map.GetHashCode()));
            hash = unchecked(hash * 31 + ExploreNavigationTopologyFingerprint());
            hash = unchecked(hash * 31 + (state.Inventory?.Count ?? 0));
            if (state.Inventory != null)
            {
                foreach (InventoryItem item in state.Inventory)
                {
                    if (item == null)
                    {
                        hash = unchecked(hash * 31);
                        continue;
                    }
                    hash = unchecked(hash * 31 + (item.Slot ?? "").GetHashCode());
                    hash = unchecked(hash * 31 + (item.Trait ?? "").GetHashCode());
                    hash = unchecked(hash * 31 + (item.Material ?? "").GetHashCode());
                }
            }
            hash = unchecked(hash * 31 + (state.ActiveStory ?? "").GetHashCode());
            hash = unchecked(hash * 31 + (state.ActiveRouteWaypointKey ?? "").GetHashCode());
            hash = unchecked(hash * 31 + (activeContentSet ?? "").GetHashCode());
            if (state.StoryFlags != null)
            {
                foreach (string flag in state.StoryFlags)
                {
                    hash = unchecked(hash * 31 + (flag ?? "").GetHashCode());
                }
            }
            if (state.DiscoveredZones != null)
            {
                foreach (string zone in state.DiscoveredZones)
                {
                    hash = unchecked(hash * 31 + (zone ?? "").GetHashCode());
                }
            }
            return "guidance=" + hash;
        }

        private int ExploreNavigationTopologyFingerprint()
        {
            MapData map = state?.Map;
            if (map == null) return 0;
            int hash = 17;
            hash = unchecked(hash * 31 + map.Width);
            hash = unchecked(hash * 31 + map.Height);
            if (map.Tiles != null)
            {
                hash = unchecked(hash * 31 + map.Tiles.Count);
                for (int i = 0; i < map.Tiles.Count; i++)
                {
                    hash = unchecked(hash * 31 + map.Tiles[i]);
                }
            }
            if (map.Objects != null)
            {
                hash = unchecked(hash * 31 + map.Objects.Count);
                foreach (MapObject obj in map.Objects)
                {
                    if (obj == null)
                    {
                        hash = unchecked(hash * 31);
                        continue;
                    }
                    hash = unchecked(hash * 31 + obj.X);
                    hash = unchecked(hash * 31 + obj.Y);
                    hash = unchecked(hash * 31 + (int)obj.Type);
                    hash = unchecked(hash * 31 + (obj.Id ?? "").GetHashCode());
                }
            }
            if (state.RoamingThreats != null)
            {
                hash = unchecked(hash * 31 + state.RoamingThreats.Count);
                foreach (RoamingThreat threat in state.RoamingThreats
                    .Where(candidate => candidate != null && candidate.Depth == state.Depth)
                    .OrderBy(candidate => candidate.Id ?? "", StringComparer.Ordinal))
                {
                    hash = unchecked(hash * 31 + (threat.Id ?? "").GetHashCode());
                    hash = unchecked(hash * 31 + threat.X);
                    hash = unchecked(hash * 31 + threat.Y);
                    hash = unchecked(hash * 31 + threat.HomeX);
                    hash = unchecked(hash * 31 + threat.HomeY);
                    hash = unchecked(hash * 31 + (threat.Active ? 1 : 0));
                }
            }
            return hash;
        }

        private bool TryNearestReachableExploreTarget(
            IEnumerable<MapObject> candidates,
            out MapObject target,
            out IReadOnlyList<Point> path)
        {
            target = null;
            path = Array.Empty<Point>();
            if (state?.Map == null || candidates == null) return false;

            var nearest = candidates
                .Where(candidate => candidate != null)
                .Select(candidate => new
                {
                    Target = candidate,
                    Path = (IReadOnlyList<Point>)FindLiveExplorePathToObject(candidate)
                })
                .Where(candidate => candidate.Path.Count > 0)
                .OrderBy(candidate => candidate.Path.Count)
                .ThenBy(candidate => candidate.Target.Y)
                .ThenBy(candidate => candidate.Target.X)
                .ThenBy(candidate => candidate.Target.Id ?? "", StringComparer.Ordinal)
                .FirstOrDefault();
            if (nearest == null) return false;

            target = nearest.Target;
            path = nearest.Path;
            return true;
        }

        private bool ObjectiveIsOutsideCurrentInterior(IEnumerable<MapObject> objectiveTargets)
        {
            if (state?.Map == null || objectiveTargets == null) return false;
            string currentInterior = MidgaardInteriorIdAt(state.PlayerX, state.PlayerY, state.Map, state.Depth);
            if (string.IsNullOrWhiteSpace(currentInterior)) return false;
            return objectiveTargets.Any(target =>
                target != null
                && !string.Equals(
                    currentInterior,
                    MidgaardInteriorIdAt(target.X, target.Y, state.Map, state.Depth),
                    StringComparison.Ordinal));
        }

        private bool TryCurrentInteriorExit(out MapObject exit, out IReadOnlyList<Point> path)
        {
            exit = null;
            path = Array.Empty<Point>();
            if (state?.Map?.Objects == null) return false;
            string currentInterior = MidgaardInteriorIdAt(state.PlayerX, state.PlayerY, state.Map, state.Depth);
            if (string.IsNullOrWhiteSpace(currentInterior)) return false;

            return TryNearestReachableExploreTarget(
                state.Map.Objects.Where(candidate =>
                    candidate != null
                    && candidate.Type == ObjectType.InteriorDoor
                    && string.Equals(
                        currentInterior,
                        MidgaardInteriorIdAt(candidate.X, candidate.Y, state.Map, state.Depth),
                        StringComparison.Ordinal)),
                out exit,
                out path);
        }

        private bool TryActiveRouteWaypoint(out WorldMapJunction waypoint)
        {
            if (state?.Map == null)
            {
                waypoint = default;
                return false;
            }

            return RouteChartRules.TryResolveWaypoint(
                WorldMapGenerationRules.RegionalJunctions(
                    state.Map.Width,
                    state.Map.Height,
                    state.Map.StartX,
                    state.Map.StartY),
                state.DiscoveredZones,
                state.Depth,
                state.ActiveRouteWaypointKey,
                out waypoint);
        }

        private IReadOnlyList<Point> ActiveRouteWaypointPath()
        {
            if (!TryActiveRouteWaypoint(out WorldMapJunction waypoint))
            {
                InvalidateActiveRouteWaypointPath();
                return activeRouteWaypointPathCache;
            }

            string cacheKey = $"{state.Map.GetHashCode()}:{state.Seed}:{state.Depth}:{state.Map.Width}:{state.Map.Height}:{state.Map.StartX}:{state.Map.StartY}:"
                + $"{state.PlayerX}:{state.PlayerY}:{ExploreNavigationTopologyFingerprint()}:{state.ActiveRouteWaypointKey}";
            if (string.Equals(activeRouteWaypointPathCacheKey, cacheKey, StringComparison.Ordinal))
            {
                return activeRouteWaypointPathCache;
            }

            activeRouteWaypointPathCacheKey = cacheKey;
            activeRouteWaypointPathCache.Clear();
            List<Point> path = FindLiveExplorePath(waypoint.X, waypoint.Y);
            if (path != null && path.Count > 0) activeRouteWaypointPathCache.AddRange(path);
            return activeRouteWaypointPathCache;
        }

        private void InvalidateActiveRouteWaypointPath()
        {
            activeRouteWaypointPathCacheKey = "";
            activeRouteWaypointPathCache.Clear();
            exploreGuidancePlanCacheKey = "";
            exploreGuidancePlanCache = null;
        }

        private static string ActiveRouteWaypointFirstDirection(IReadOnlyList<Point> path)
        {
            if (path == null || path.Count <= 1) return "HERE";
            Point from = path[0];
            Point to = path[1];
            int dx = to.X - from.X;
            int dy = to.Y - from.Y;
            if (dy < 0) return "N";
            if (dy > 0) return "S";
            if (dx < 0) return "W";
            if (dx > 0) return "E";
            return "HERE";
        }

        private bool IsEligibleExploreWaypoint(MapObject obj)
        {
            if (obj == null) return false;
            if (ContentSetCatalog.IsSewerSlice(activeContentSet)
                && HasStoryFlag(StoryFlags.GlassAndAshFrontierSurveyed)
                && (!ContentSetCatalog.AllowGlassAndAshChapter(activeContentSet, state.StoryFlags)
                    || ContentSetCatalog.GlassAndAshComplete(state.StoryFlags))
                && (string.Equals(obj.Id, OldRoadDescentId, StringComparison.Ordinal)
                    || string.Equals(obj.Id, BoneRoadPassageId, StringComparison.Ordinal)
                    || string.Equals(obj.Id, GlassAndAshPassageId, StringComparison.Ordinal)))
            {
                return false;
            }
            if (string.Equals(obj.Id, BoneRoadPassageId, StringComparison.Ordinal))
            {
                return state.Depth == 2
                    && HasStoryFlag(StoryFlags.KoboldKingDefeated)
                    && ContentSetCatalog.AllowBoneRoadChapter(activeContentSet, state.StoryFlags);
            }
            if (string.Equals(obj.Id, GlassAndAshPassageId, StringComparison.Ordinal))
            {
                return state.Depth == 3
                    && HasStoryFlag(StoryFlags.RedGateWarningRecovered)
                    && (!HasStoryFlag(StoryFlags.GlassAndAshFrontierSurveyed)
                        || ContentSetCatalog.AllowGlassAndAshChapter(activeContentSet, state.StoryFlags)
                            && !ContentSetCatalog.GlassAndAshComplete(state.StoryFlags))
                    && ContentSetCatalog.AllowBoneRoadChapter(activeContentSet, state.StoryFlags);
            }
            if (state.Depth == 4
                && ContentSetCatalog.AllowGlassAndAshChapter(activeContentSet, state.StoryFlags)
                && !ContentSetCatalog.GlassAndAshComplete(state.StoryFlags))
            {
                if (string.Equals(obj.Id, "regional-site:" + GlassLoreLibrarySiteId, StringComparison.Ordinal))
                {
                    return !HasStoryFlag(StoryFlags.GlassIndexRecovered);
                }
                if (string.Equals(obj.Id, "regional-site:" + RedGateSealSiteId, StringComparison.Ordinal))
                {
                    return HasStoryFlag(StoryFlags.GlassIndexRecovered);
                }
            }
            if (state.Depth == 3
                && ContentSetCatalog.AllowBoneRoadChapter(activeContentSet, state.StoryFlags))
            {
                if (string.Equals(obj.Id, "regional-site:gloam-deep-crypt", StringComparison.Ordinal))
                {
                    return !HasStoryFlag(StoryFlags.GloamWardenDefeated);
                }
                if (string.Equals(obj.Id, "regional-site:red-gate-seal", StringComparison.Ordinal))
                {
                    return HasStoryFlag(StoryFlags.GloamWardenDefeated)
                        && !HasStoryFlag(StoryFlags.RedGateWarningRecovered);
                }
            }
            if (IsRouteScaffoldObject(obj.Type) && HasStoryFlag(RouteScaffoldFlag(obj))) return false;
            if (state.Depth == 2 && obj.Type == ObjectType.Cave)
            {
                return HasStoryFlag(StoryFlags.KoboldAmbushSurvived)
                    && !HasStoryFlag(StoryFlags.KoboldKingDefeated)
                    && IsKoboldStoryCave(obj);
            }
            return true;
        }

        private int ExploreWaypointPriority(MapObject obj)
        {
            if (obj == null) return 99;
            if (IsCurrentMidgaardObjective(obj)) return 0;
            if (IsCurrentBoneRoadGuidanceTarget(obj)) return 0;
            if (state.Depth == 2 && !HasStoryFlag(StoryFlags.KoboldKingDefeated) && IsKoboldStoryCave(obj)) return 0;
            if (obj.Type == ObjectType.Stairs) return HasStoryFlag(StoryFlags.KoboldKingDefeated) ? 0 : 2;
            if (IsRouteScaffoldObject(obj.Type)) return 1;
            if (obj.Type == ObjectType.DungeonGate || obj.Type == ObjectType.DeepCrypt || obj.Type == ObjectType.PortalSeal) return 2;
            if (obj.Type == ObjectType.Cave) return 3;
            return 4;
        }

        private bool IsCurrentBoneRoadGuidanceTarget(MapObject obj)
        {
            if (obj == null
                || state == null
                || !ContentSetCatalog.AllowBoneRoadChapter(activeContentSet, state.StoryFlags))
            {
                return false;
            }
            bool glassRoadActive = HasGlassAndAshStoryProgress()
                && !ContentSetCatalog.GlassAndAshComplete(state.StoryFlags);
            if (glassRoadActive)
            {
                if (state.Depth == 1) return string.Equals(obj.Id, OldRoadDescentId, StringComparison.Ordinal);
                if (state.Depth == 2) return string.Equals(obj.Id, BoneRoadPassageId, StringComparison.Ordinal);
                if (state.Depth == 3) return string.Equals(obj.Id, GlassAndAshPassageId, StringComparison.Ordinal);
                if (state.Depth == 4)
                {
                    string targetId = HasStoryFlag(StoryFlags.GlassIndexRecovered)
                        ? "regional-site:" + RedGateSealSiteId
                        : "regional-site:" + GlassLoreLibrarySiteId;
                    return string.Equals(obj.Id, targetId, StringComparison.Ordinal);
                }
                return false;
            }
            if (ContentSetCatalog.IsSewerSlice(activeContentSet)
                && HasStoryFlag(StoryFlags.GlassAndAshFrontierSurveyed))
            {
                return false;
            }
            if (state.Depth == 1)
            {
                return HasStoryFlag(StoryFlags.KoboldKingDefeated)
                    && string.Equals(obj.Id, OldRoadDescentId, StringComparison.Ordinal);
            }
            if (state.Depth == 2)
            {
                return HasStoryFlag(StoryFlags.KoboldKingDefeated)
                    && string.Equals(obj.Id, BoneRoadPassageId, StringComparison.Ordinal);
            }
            if (state.Depth != 3) return false;
            if (HasStoryFlag(StoryFlags.RedGateWarningRecovered))
            {
                return !HasStoryFlag(StoryFlags.GlassAndAshFrontierSurveyed)
                    && string.Equals(obj.Id, GlassAndAshPassageId, StringComparison.Ordinal);
            }
            if (HasStoryFlag(StoryFlags.GloamWardenDefeated))
            {
                return string.Equals(obj.Id, "regional-site:red-gate-seal", StringComparison.Ordinal);
            }
            return string.Equals(obj.Id, "regional-site:gloam-deep-crypt", StringComparison.Ordinal);
        }

        private string ExploreNearbySummaryLine()
        {
            if (state?.Map?.Objects == null) return "No marked sites nearby.";
            ExplorationInteraction interaction = CurrentExploreInteraction();
            ExploreGuidancePlan guidance = CurrentExploreGuidancePlan();
            List<string> nearby = new List<string>();
            if (state.RoamingThreats != null)
            {
                nearby.AddRange(state.RoamingThreats
                    .Where(threat => threat != null
                        && threat.Active
                        && threat.Depth == state.Depth
                        && Distance(threat.X, threat.Y, state.PlayerX, state.PlayerY) <= ExploreRevealRadius)
                    .OrderByDescending(threat => threat.Alerted)
                    .ThenBy(threat => Distance(threat.X, threat.Y, state.PlayerX, state.PlayerY))
                    .Take(2)
                    .Select(threat => $"{(threat.Alerted ? "DANGER" : "Patrol")}: {threat.Name} · compass {ExploreDirectionToPoint(threat.X, threat.Y)}"));
            }

            nearby.AddRange(state.Map.Objects
                .Where(o => o != null
                    && ExplorationInteractionRules.IsUseObject(o)
                    && (!interaction.HasTarget || !ReferenceEquals(interaction.Target, o))
                    && (guidance.TargetObject == null || !ReferenceEquals(guidance.TargetObject, o))
                    && Distance(o.X, o.Y, state.PlayerX, state.PlayerY) <= ExploreRevealRadius)
                .OrderBy(o => IsCurrentMidgaardObjective(o) ? 0 : 1)
                .ThenBy(o => Distance(o.X, o.Y, state.PlayerX, state.PlayerY))
                .Take(Mathf.Max(0, 4 - nearby.Count))
                .Select(o => $"{ObjectName(o)} {ExploreDirectionTo(o)}"));
            return nearby.Count == 0 ? "No other marked sites within sight." : string.Join("\n", nearby);
        }

        private string ExploreDirectionToPoint(int x, int y)
        {
            int dx = x - state.PlayerX;
            int dy = y - state.PlayerY;
            int distance = Mathf.Abs(dx) + Mathf.Abs(dy);
            if (distance <= 0) return "HERE";
            string direction = Mathf.Abs(dx) >= Mathf.Abs(dy)
                ? dx < 0 ? "W" : "E"
                : dy < 0 ? "N" : "S";
            return direction + distance;
        }

        private bool IsExploreGuidanceTarget(MapObject obj)
        {
            if (obj == null || obj.Type == ObjectType.CityWall || obj.Type == ObjectType.TownGuard) return false;
            if (IsCurrentMidgaardObjective(obj)) return true;
            return obj.Type == ObjectType.Stairs
                || obj.Type == ObjectType.Cave
                || obj.Type == ObjectType.DungeonGate
                || obj.Type == ObjectType.DeepCrypt
                || obj.Type == ObjectType.AncientGrove
                || obj.Type == ObjectType.PortalSeal
                || IsRouteScaffoldObject(obj.Type);
        }

        private string ExploreDirectionTo(MapObject target)
        {
            if (target == null || state == null) return "?";
            IReadOnlyList<Point> path = FindLiveExplorePathToObject(target);
            if (path.Count == 0) return "route blocked";
            if (path.Count == 1) return "here";
            Point step = path[1];
            int dx = step.X - state.PlayerX;
            int dy = step.Y - state.PlayerY;
            string direction = dy < 0 ? "N" : dy > 0 ? "S" : dx < 0 ? "W" : "E";
            return $"{direction}{path.Count - 1}";
        }

        private IReadOnlyList<ExplorationHudPartyMemberView> BuildExplorationHudPartyViews()
        {
            if (state?.Party == null) return Array.Empty<ExplorationHudPartyMemberView>();
            explorationHudPartyBuffer.Clear();
            int count = Mathf.Min(4, state.Party.Count);
            for (int i = 0; i < count; i++)
            {
                PartyMember member = state.Party[i];
                if (member == null) continue;
                explorationHudPartyBuffer.Add(new ExplorationHudPartyMemberView
                {
                    Name = member.Name,
                    ClassLine = $"L{member.Level} {DisplayClass(member.ClassKey)}",
                    ColorHex = member.SpriteColor,
                    Hp = member.Hp,
                    MaxHp = member.MaxHp,
                    Mana = member.Mana,
                    MaxMana = member.MaxMana
                });
            }
            return explorationHudPartyBuffer;
        }

        private IReadOnlyList<ExplorationHudLogView> BuildExplorationHudLogViews()
        {
            if (state?.Log == null) return Array.Empty<ExplorationHudLogView>();
            explorationHudLogBuffer.Clear();
            int count = Mathf.Min(3, state.Log.Count);
            for (int i = 0; i < count; i++)
            {
                LogEntry entry = state.Log[i];
                explorationHudLogBuffer.Add(new ExplorationHudLogView
                {
                    Text = entry.Text,
                    Tone = entry.Tone.ToString()
                });
            }
            return explorationHudLogBuffer;
        }
    }
}
