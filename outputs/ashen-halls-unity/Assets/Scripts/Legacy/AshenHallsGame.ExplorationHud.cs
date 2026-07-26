using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private readonly List<Point> activeRouteWaypointPathCache = new List<Point>();
        private string activeRouteWaypointPathCacheKey = "";

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
            float statusW = sideInnerW * 0.55f;
            GUI.Label(new Rect(sideInnerX, geometry.Side.y + 34f * scale, statusW, 18f * scale), FitText(view.DangerLabel, statusW, CenterLeftStyle(ExploreHudFont(11), Hex("66c9b6"))), CenterLeftStyle(ExploreHudFont(11), Hex("66c9b6")));
            GUI.Label(new Rect(sideInnerX + statusW, geometry.Side.y + 34f * scale, sideInnerW - statusW, 18f * scale), FitText(view.ViewLabel, sideInnerW - statusW, CenterRightStyle(ExploreHudFont(11), exploreWideView ? frost : teal)), CenterRightStyle(ExploreHudFont(11), exploreWideView ? frost : teal));
            DrawRect(new Rect(sideInnerX, geometry.Side.y + 57f * scale, sideInnerW, 1f), line.WithAlpha(0.72f));

            Rect detailsButton = new Rect(sideInnerX, geometry.Side.yMax - 40f * scale, sideInnerW, 32f * scale);
            float contentBottom = detailsButton.y - 8f * scale;
            float cursor = geometry.Side.y + 66f * scale;
            float sectionGap = 7f * scale;
            if (view.DetailsOpen)
            {
                float hereH = 72f * scale;
                DrawExploreFallbackInfoCard(new Rect(sideInnerX, cursor, sideInnerW, hereH), "HERE", (view.ZoneDetail + "\n" + view.LookLine).Trim(), gold, false);
                cursor += hereH + sectionGap;
                float objectiveH = 82f * scale;
                DrawExploreFallbackInfoCard(new Rect(sideInnerX, cursor, sideInnerW, objectiveH), "OBJECTIVE", view.ObjectiveLine, teal, true);
                cursor += objectiveH + sectionGap;
                float growthH = 38f * scale;
                DrawExploreFallbackInfoCard(new Rect(sideInnerX, cursor, sideInnerW, growthH), "PROGRESS", view.GrowthLine, moss, false);
                cursor += growthH + 9f * scale;
                GUI.Label(new Rect(sideInnerX, cursor, sideInnerW, 18f * scale), "PARTY", CenterLeftStyle(ExploreHudFont(11), Hex("e3ba63")));
                cursor += 21f * scale;
                int count = Mathf.Min(4, view.Party == null ? 0 : view.Party.Count);
                for (int i = 0; i < count; i++)
                {
                    DrawExploreRailPartyRow(new Rect(sideInnerX, cursor, sideInnerW, 32f * scale), view.Party[i]);
                    cursor += 35f * scale;
                }
                float remaining = contentBottom - cursor;
                if (remaining >= 84f * scale)
                {
                    float mapH = Mathf.Min(180f * scale, remaining);
                    if (remaining >= 144f * scale) mapH = Mathf.Min(mapH, remaining * 0.60f);
                    DrawExploreMiniMap(new Rect(sideInnerX, cursor, sideInnerW, mapH));
                    cursor += mapH + sectionGap;
                    remaining = contentBottom - cursor;
                }
                DrawExploreFallbackLatest(sideInnerX, sideInnerW, ref cursor, contentBottom, view.Logs);
            }
            else
            {
                float nextH = 62f * scale;
                DrawExploreFallbackInfoCard(new Rect(sideInnerX, cursor, sideInnerW, nextH), "NEXT", view.WaypointLine, gold, true);
                cursor += nextH + sectionGap;
                float objectiveH = 76f * scale;
                DrawExploreFallbackInfoCard(new Rect(sideInnerX, cursor, sideInnerW, objectiveH), "OBJECTIVE", view.ObjectiveSummary, teal, true);
                cursor += objectiveH + sectionGap;
                float nearbyH = 58f * scale;
                DrawExploreFallbackInfoCard(new Rect(sideInnerX, cursor, sideInnerW, nearbyH), "NEARBY", view.NearbyLine, moss, false);
                cursor += nearbyH + sectionGap;
                if (view.HasAction)
                {
                    Rect ready = new Rect(sideInnerX, cursor, sideInnerW, 56f * scale);
                    if (DrawExploreFallbackAction(ready, view.ActionLabel, view.ActionTarget, true)) UseNearbyExploreObject();
                    cursor += ready.height + 9f * scale;
                }

                GUI.Label(new Rect(sideInnerX, cursor, sideInnerW, 18f * scale), "PARTY", CenterLeftStyle(ExploreHudFont(11), Hex("e3ba63")));
                cursor += 21f * scale;
                int count = Mathf.Min(4, view.Party == null ? 0 : view.Party.Count);
                for (int i = 0; i < count; i++)
                {
                    DrawExploreRailPartyRow(new Rect(sideInnerX, cursor, sideInnerW, 32f * scale), view.Party[i]);
                    cursor += 35f * scale;
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
            float gap = 7f * scale;
            const int secondaryCount = 8;
            float actionW = Mathf.Clamp(geometry.Command.width * 0.22f, 270f * scale, 390f * scale);
            float smallW = Mathf.Max(82f * scale, (geometry.Command.width - actionW - 20f * scale - gap * secondaryCount) / secondaryCount);
            float x = geometry.Command.x + 10f * scale;
            Rect actionRect = new Rect(x, geometry.Command.y + 8f * scale, actionW, 52f * scale);
            bool oldEnabled = GUI.enabled;
            GUI.enabled = oldEnabled && view.HasAction;
            if (DrawExploreFallbackAction(actionRect, view.HasAction ? view.ActionLabel : "Explore", view.HasAction ? view.ActionTarget : "No nearby action", view.HasAction)) UseNearbyExploreObject();
            GUI.enabled = oldEnabled;
            x += actionW + gap;
            GUI.enabled = oldEnabled && state.Supplies > 0;
            if (DrawExploreFallbackCommand(new Rect(x, geometry.Command.y + 8f * scale, smallW, 52f * scale), "Camp", "R", "camp")) Camp();
            GUI.enabled = oldEnabled;
            x += smallW + gap;
            if (DrawExploreFallbackCommand(new Rect(x, geometry.Command.y + 8f * scale, smallW, 52f * scale), "Recall", "Y", "magic")) RecallToTempleSquare();
            x += smallW + gap;
            GUI.enabled = oldEnabled && CanDescend();
            if (DrawExploreFallbackCommand(new Rect(x, geometry.Command.y + 8f * scale, smallW, 52f * scale), "Descend", "T", "arrow")) Descend();
            GUI.enabled = oldEnabled;
            x += smallW + gap;
            GUI.enabled = oldEnabled && state.Elixirs > 0;
            if (DrawExploreFallbackCommand(new Rect(x, geometry.Command.y + 8f * scale, smallW, 52f * scale), "Elixir", "H", "hp")) UseElixir();
            GUI.enabled = oldEnabled;
            x += smallW + gap;
            if (DrawExploreFallbackCommand(new Rect(x, geometry.Command.y + 8f * scale, smallW, 52f * scale), exploreWideView ? "Local" : "Region", "Tab", "scroll")) ToggleExploreView();
            x += smallW + gap;
            if (DrawExploreFallbackCommand(new Rect(x, geometry.Command.y + 8f * scale, smallW, 52f * scale), "Journal", "J", "timeline")) ToggleArmory(ArmoryTab.Journal);
            x += smallW + gap;
            if (DrawExploreFallbackCommand(new Rect(x, geometry.Command.y + 8f * scale, smallW, 52f * scale), "Party", "F", "party")) ToggleArmory(ArmoryTab.Party);
            x += smallW + gap;
            if (DrawExploreFallbackCommand(new Rect(x, geometry.Command.y + 8f * scale, smallW, 52f * scale), "Menu", "Esc", "queue")) OpenPauseMenu();
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

        private bool DrawExploreFallbackCommand(Rect rect, string label, string hotkey, string icon)
        {
            float scale = ExplorationHudScreenLayout.InterfaceScale(Screen.width, Screen.height);
            DrawRect(rect, Hex("141b20", 0.99f));
            DrawRect(new Rect(rect.x, rect.y, rect.width, 3f * scale), Hex("58462c", 0.68f));
            DrawBorder(rect, GUI.enabled ? line.WithAlpha(0.82f) : line.WithAlpha(0.40f), 1);
            float iconSize = Mathf.Clamp(rect.height - 18f * scale, 28f * scale, 32f * scale);
            Rect iconRect = new Rect(rect.x + 7f * scale, rect.y + (rect.height - iconSize) * 0.5f, iconSize, iconSize);
            DrawRect(Pad(iconRect, -2f * scale), Hex("070a0c", 0.82f));
            DrawBorder(Pad(iconRect, -2f * scale), gold.WithAlpha(0.58f), 1);
            DrawTinyUiIcon(iconRect, icon, gold);
            float textX = iconRect.xMax + 6f * scale;
            float textW = Mathf.Max(24f * scale, rect.xMax - textX - 6f * scale);
            GUI.Label(new Rect(textX, rect.y + 6f * scale, textW, 21f * scale), FitText(label, textW, CenterLeftStyle(ExploreHudFont(13), ink)), CenterLeftStyle(ExploreHudFont(13), ink));
            GUI.Label(new Rect(textX, rect.y + 29f * scale, textW, 16f * scale), hotkey, CenterLeftStyle(ExploreHudFont(11), Hex("e3ba63")));
            return GUI.Button(rect, GUIContent.none, GUIStyle.none);
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
            DrawRect(rect, available ? Hex("10201f", 0.99f) : Hex("101519", 0.96f));
            DrawRect(new Rect(rect.x, rect.y, 4f * scale, rect.height), accent.WithAlpha(0.94f));
            DrawBorder(rect, accent.WithAlpha(available ? 0.88f : 0.42f), available ? 2 : 1);
            Rect iconRect = new Rect(rect.x + 10f * scale, rect.y + 10f * scale, 32f * scale, 32f * scale);
            DrawTinyUiIcon(iconRect, available ? "hand" : "scroll", available ? teal : muted);
            float keyW = 34f * scale;
            Rect key = new Rect(rect.xMax - keyW - 10f * scale, rect.y + 10f * scale, keyW, 32f * scale);
            DrawRect(key, Hex("05090a", 0.94f));
            DrawBorder(key, accent.WithAlpha(0.76f), 1);
            GUI.Label(key, available ? "E" : "--", CenterStyle(ExploreHudFont(13), available ? ink : muted));
            float textX = iconRect.xMax + 10f * scale;
            float textW = Mathf.Max(60f * scale, key.x - textX - 8f * scale);
            GUI.Label(new Rect(textX, rect.y + 5f * scale, textW, 20f * scale), FitText((actionLabel ?? "USE").ToUpperInvariant(), textW, CenterLeftStyle(ExploreHudFont(13), accent)), CenterLeftStyle(ExploreHudFont(13), accent));
            GUI.Label(new Rect(textX, rect.y + 26f * scale, textW, 21f * scale), FitText(actionTarget ?? "", textW, CenterLeftStyle(ExploreHudFont(12), ink)), CenterLeftStyle(ExploreHudFont(12), ink));
            return GUI.Button(rect, GUIContent.none, GUIStyle.none);
        }

        private bool DrawExploreFallbackToggleButton(Rect rect, bool detailsOpen)
        {
            float scale = ExplorationHudScreenLayout.InterfaceScale(Screen.width, Screen.height);
            DrawRect(rect, Hex("151b20", 0.99f));
            DrawRect(new Rect(rect.x, rect.y, 4f * scale, rect.height), teal.WithAlpha(0.92f));
            DrawBorder(rect, teal.WithAlpha(0.68f), 1);
            GUI.Label(
                new Rect(rect.x + 12f * scale, rect.y, rect.width - 54f * scale, rect.height),
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
                ZoneName = zone?.Name ?? HomeTownName,
                ZoneDetail = zone == null ? "" : $"{zone.Title} / {ExploreGroundName(state.PlayerX, state.PlayerY)}",
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

        private string ExploreObjectiveSummaryLine()
        {
            if (state == null) return "Prepare the party.";
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
                if (target == ObjectType.OldRoadScout) return "Ask the Old Road scout about the newly marked routes.";
                if (target == ObjectType.Stairs) return "Follow the Salt Cisterns road to Sluice Steps and descend toward Dusk Market.";
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
            if (TryActiveRouteWaypoint(out WorldMapJunction activeWaypoint))
            {
                IReadOnlyList<Point> path = ActiveRouteWaypointPath();
                if (path.Count == 1) return $"Marked: {activeWaypoint.Name} / here. Open Journal to clear it.";
                if (path.Count > 1)
                {
                    return $"Marked: {activeWaypoint.Name} / {ActiveRouteWaypointFirstDirection(path)} {RouteChartRules.DistanceLabel(path.Count - 1)}.";
                }

                string direct = RouteChartRules.DirectionLabel(
                    state.PlayerX,
                    state.PlayerY,
                    activeWaypoint.X,
                    activeWaypoint.Y);
                return $"Marked: {activeWaypoint.Name} / {direct} / route blocked.";
            }
            if (ShouldUseMidgaardWayfinding()) return MidgaardWayfindingCompactLine();

            bool preferRegionalTargets = state.Depth == 1 && !ShouldUseMidgaardWayfinding();
            IEnumerable<MapObject> eligibleTargets = state.Map.Objects
                .Where(o => o != null && IsExploreGuidanceTarget(o) && IsEligibleExploreWaypoint(o));
            if (preferRegionalTargets)
            {
                eligibleTargets = eligibleTargets.Where(o => !IsMidgaardCityCell(o.X, o.Y, state.Map, state.Depth));
            }

            MapObject target = eligibleTargets
                .Select(o => new { Target = o, Length = ExploreRouteLength(o) })
                .Where(candidate => candidate.Length >= 0)
                .OrderBy(candidate => ExploreWaypointPriority(candidate.Target))
                .ThenBy(candidate => candidate.Length)
                .ThenBy(candidate => candidate.Target.Y)
                .ThenBy(candidate => candidate.Target.X)
                .Select(candidate => candidate.Target)
                .FirstOrDefault();
            return target == null
                ? "No marked objective in the visible region. Open Journal for the chapter route."
                : $"Next: {ObjectName(target)} {ExploreDirectionTo(target)}.";
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
                + $"{state.PlayerX}:{state.PlayerY}:{state.Map.Objects?.Count ?? 0}:{state.ActiveRouteWaypointKey}";
            if (string.Equals(activeRouteWaypointPathCacheKey, cacheKey, StringComparison.Ordinal))
            {
                return activeRouteWaypointPathCache;
            }

            activeRouteWaypointPathCacheKey = cacheKey;
            activeRouteWaypointPathCache.Clear();
            List<Point> path = ExplorationTraversalRules.FindPath(
                state.Map,
                state.PlayerX,
                state.PlayerY,
                waypoint.X,
                waypoint.Y);
            if (path != null && path.Count > 0) activeRouteWaypointPathCache.AddRange(path);
            return activeRouteWaypointPathCache;
        }

        private void InvalidateActiveRouteWaypointPath()
        {
            activeRouteWaypointPathCacheKey = "";
            activeRouteWaypointPathCache.Clear();
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
            if (IsRouteScaffoldObject(obj.Type) && HasStoryFlag(RouteScaffoldFlag(obj))) return false;
            if (state.Depth == 2 && obj.Type == ObjectType.Cave)
            {
                return !HasStoryFlag(StoryFlags.KoboldKingDefeated) && IsKoboldStoryCave(obj);
            }
            return true;
        }

        private int ExploreWaypointPriority(MapObject obj)
        {
            if (obj == null) return 99;
            if (IsCurrentMidgaardObjective(obj)) return 0;
            if (state.Depth == 2 && !HasStoryFlag(StoryFlags.KoboldKingDefeated) && IsKoboldStoryCave(obj)) return 0;
            if (obj.Type == ObjectType.Stairs) return HasStoryFlag(StoryFlags.KoboldKingDefeated) ? 0 : 2;
            if (IsRouteScaffoldObject(obj.Type)) return 1;
            if (obj.Type == ObjectType.DungeonGate || obj.Type == ObjectType.DeepCrypt || obj.Type == ObjectType.PortalSeal) return 2;
            if (obj.Type == ObjectType.Cave) return 3;
            return 4;
        }

        private int ExploreRouteLength(MapObject target)
        {
            if (target == null || state?.Map == null) return -1;
            IReadOnlyList<Point> path = ExplorationTraversalRules.FindPathToObject(state.Map, state.PlayerX, state.PlayerY, target);
            return path.Count == 0 ? -1 : Mathf.Max(0, path.Count - 1);
        }

        private string ExploreNearbySummaryLine()
        {
            if (state?.Map?.Objects == null) return "No marked sites nearby.";
            ExplorationInteraction interaction = CurrentExploreInteraction();
            string[] nearby = state.Map.Objects
                .Where(o => o != null
                    && ExplorationInteractionRules.IsUseObject(o)
                    && Distance(o.X, o.Y, state.PlayerX, state.PlayerY) <= ExploreRevealRadius)
                .OrderBy(o => interaction.HasTarget && ReferenceEquals(interaction.Target, o) ? 0 : IsCurrentMidgaardObjective(o) ? 1 : 2)
                .ThenBy(o => Distance(o.X, o.Y, state.PlayerX, state.PlayerY))
                .Take(4)
                .Select(o => $"{ObjectName(o)} {ExploreDirectionTo(o)}")
                .ToArray();
            return nearby.Length == 0 ? "No marked sites within sight." : string.Join("\n", nearby);
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
            IReadOnlyList<Point> path = ExplorationTraversalRules.FindPathToObject(state.Map, state.PlayerX, state.PlayerY, target);
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
