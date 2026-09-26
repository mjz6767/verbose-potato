using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class WorldMapVisualLayoutSmoke
    {
        public static void RunOrThrow()
        {
            foreach (float size in new[] { 24f, 32f, 44f, 64f, 96f, 128f })
            {
                Rect cell = new Rect(33f, 77f, size, size);
                Rect feet = ExplorationMapVisualRules.PartyFootprint(cell);
                Require(feet.xMin >= cell.xMin && feet.xMax <= cell.xMax && feet.yMin >= cell.y + size * 0.8f
                    && feet.yMax <= cell.yMax, "party locator leaves the silhouette and neighboring tiles clear");
                foreach (bool wide in new[] { false, true })
                {
                    float stroke = ExplorationMapVisualRules.RouteStrokeWidth(size, wide);
                    Require(stroke >= 2f && stroke <= 5f, "route remains legible without thick map-wide bands");
                    float core = ExplorationMapVisualRules.RouteCoreWidth(size, wide);
                    Require(core >= 1.5f && core <= stroke, "gold route core keeps readable coverage without growing past its stroke");
                }
            }
            foreach (WorldMapCellAccessKind access in Enum.GetValues(typeof(WorldMapCellAccessKind)))
            {
                bool walkable = ExplorationReadabilityRules.IsWalkableAccess(access);
                Require(ExplorationMapVisualRules.ShouldDrawPassiveMovementCue(access, false, false) == !walkable,
                    "only collision/use/danger cues remain persistent: " + access);
                Require(ExplorationMapVisualRules.ShouldDrawPassiveMovementCue(access, false, true), "hover preserves each access explanation");
            }
            Require(!ExplorationMapVisualRules.ShouldDrawPassiveMovementCue(WorldMapCellAccessKind.UseFromBeside, true, false),
                "current contact uses its E prompt rather than a second movement bracket");
            Require(ExplorationMapVisualRules.ShouldDrawPassiveMovementCue(WorldMapCellAccessKind.EnemyOccupied, true, false),
                "danger cannot be hidden by stale contact identity");

            var source = new List<MapObject>();
            for (int i = 0; i < 1200; i++) source.Add(new MapObject(i % 60, i / 60, ObjectType.Cache, "object-" + i));
            source.Insert(5, null);
            source.Add(new MapObject(2, 3, ObjectType.Cache, "same"));
            source.Add(new MapObject(2, 3, ObjectType.Cache, "same"));
            source.Add(new MapObject(2, 3, ObjectType.Cache, null));
            var buffer = new ExplorationVisibleObjectBuffer();
            var listIdentity = buffer.Items;
            for (int offset = 0; offset < 3; offset++)
            {
                buffer.Rebuild(source, offset, 1, 9, 7);
                MapObject[] expected = source.Where(obj => obj != null && obj.X >= offset && obj.X < offset + 9 && obj.Y >= 1 && obj.Y < 8)
                    .OrderBy(obj => obj.Y).ThenBy(obj => obj.X).ThenBy(obj => obj.Id ?? "", StringComparer.Ordinal).ToArray();
                Require(buffer.Items.Count == expected.Length && buffer.SourceVisits == source.Count, "one scan collects only viewport candidates");
                Require(buffer.Items.Count < source.Count / 10, "large map sorts fewer than one tenth of the source objects in fixture");
                for (int i = 0; i < expected.Length; i++) Require(ReferenceEquals(buffer.Items[i].Object, expected[i]), "visible ordering preserves prior stable depth/ID order");
                Require(ReferenceEquals(listIdentity, buffer.Items), "repaint reuses object buffer storage");
            }
            source[0].X = 3;
            source[0].Y = 4;
            buffer.Rebuild(source, 3, 4, 1, 1);
            Require(buffer.Items.Exists(entry => ReferenceEquals(entry.Object, source[0])), "mutated object positions are refreshed on next repaint");
            buffer.Rebuild(null, 0, 0, 9, 7);
            Require(buffer.Items.Count == 0 && buffer.SourceVisits == 0, "empty or replaced map cannot retain stale sprites");
            ThreatHeadingsRespectLiveVisibility();
            CompactHeadingsDescribeAccessWithoutLoreParsing();
            ProductionInspectionKeepsDetailsAndPrivacy();
        }

        private static void CompactHeadingsDescribeAccessWithoutLoreParsing()
        {
            Require(ExplorationMapVisualRules.InspectionHeading("Ground", WorldMapCellAccessKind.OpenGround, 0, false)
                == "Ground · Underfoot", "current ground describes position rather than offering a redundant move");
            foreach (WorldMapCellAccessKind access in new[] { WorldMapCellAccessKind.OpenGround,
                WorldMapCellAccessKind.SoftScenery, WorldMapCellAccessKind.WalkableFeature })
            {
                Require(ExplorationMapVisualRules.InspectionHeading("Ground", access, 1, false)
                    == "Ground · Click to move", "adjacent walkable headings advertise a legal step: " + access);
                Require(ExplorationMapVisualRules.InspectionHeading("Ground", access, 4, false)
                    == "Ground · Walkable · Range 4", "distant ground does not imply click-to-pathfind: " + access);
                Require(ExplorationMapVisualRules.InspectionHeading("Ground", access, 2, false)
                    == "Ground · Walkable · Range 2", "diagonal ground does not imply a legal one-step move: " + access);
            }
            Require(ExplorationMapVisualRules.InspectionHeading("Cache", WorldMapCellAccessKind.UseFromBeside, 1, false)
                == "Cache · Use from beside", "adjacent blocking contact never promises traversal or click-only use");
            Require(ExplorationMapVisualRules.InspectionHeading("Cache", WorldMapCellAccessKind.UseFromBeside, 4, false)
                == "Cache · Approach to interact", "remote contact requires approach");
            Require(ExplorationMapVisualRules.InspectionHeading("Patrol", WorldMapCellAccessKind.EnemyOccupied, 1, false, "Pursuing")
                == "Patrol · Click to engage · Pursuing", "enemy intent follows its actionable engagement instruction");
            Require(ExplorationMapVisualRules.InspectionHeading("Patrol", WorldMapCellAccessKind.EnemyOccupied, 4, false, "Enemy")
                == "Patrol · Range 4 · Enemy", "remote enemy does not advertise an unavailable attack");
            foreach (WorldMapCellAccessKind access in new[] { WorldMapCellAccessKind.SolidObstacle, WorldMapCellAccessKind.BlockedTerrain })
                Require(ExplorationMapVisualRules.InspectionHeading("Wall", access, 1, false)
                    == "Wall · Blocks movement", "solid terrain keeps an explicit textual warning: " + access);
            foreach (WorldMapCellAccessKind access in Enum.GetValues(typeof(WorldMapCellAccessKind)))
            foreach (int distance in new[] { 0, 1, 4 })
                Require(ExplorationMapVisualRules.InspectionHeading("Focus", access, distance, true, "Marked route")
                    == "Focus · Inspect only · Marked route", "Region headings never advertise Local travel or use");
            Require(ExplorationMapVisualRules.InspectionHeading(null, WorldMapCellAccessKind.OpenGround, 1, false, null)
                == "Ground · Click to move", "missing optional identity/status has a safe compact fallback");
            Require(ExplorationMapVisualRules.InspectionHeading("Name / with lore separator", WorldMapCellAccessKind.OpenGround, 1, false)
                == "Name / with lore separator · Click to move", "heading construction preserves identity instead of parsing detail separators");
        }

        private static void ProductionInspectionKeepsDetailsAndPrivacy()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            GameObject host = new GameObject("Compact map inspection audit");
            host.SetActive(false);
            host.hideFlags = HideFlags.HideAndDontSave;
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                MapData map = new MapData { Width = 60, Height = 60, Depth = 2, StartX = 20, StartY = 20 };
                for (int i = 0; i < map.Width * map.Height; i++) map.Tiles.Add(1);
                GameState state = new GameState { Mode = GameMode.Explore, Depth = 2, Map = map, PlayerX = 20, PlayerY = 20,
                    ReducedMotion = true, MusicMuted = true, SfxMuted = true };
                typeof(AshenHallsGame).GetField("state", flags).SetValue(game, state);
                typeof(AshenHallsGame).GetField("labSaveBlocked", flags).SetValue(game, true);
                typeof(AshenHallsGame).GetField("launchError", flags).SetValue(game, "Compact inspection fixture held");
                string groundName = (string)typeof(AshenHallsGame).GetMethod("ExploreGroundName", flags)
                    .Invoke(game, new object[] { 21, 20 });
                AssertProductionHeading(game, state, 21, 20, false,
                    ExplorationMapVisualRules.InspectionHeading(groundName, WorldMapCellAccessKind.OpenGround, 1, false));
                AssertProductionHeading(game, state, 21, 20, true,
                    ExplorationMapVisualRules.InspectionHeading(groundName, WorldMapCellAccessKind.OpenGround, 1, true));

                foreach (ObjectType type in new[] { ObjectType.Camp, ObjectType.Cache, ObjectType.CityWall })
                {
                    MapObject obj = new MapObject(21, 20, type, "compact-audit-object");
                    map.Objects.Clear();
                    map.Objects.Add(obj);
                    map.InvalidateObjectLookup();
                    string name = (string)typeof(AshenHallsGame).GetMethod("ObjectName", flags, null,
                        new[] { typeof(MapObject) }, null).Invoke(game, new object[] { obj });
                    WorldMapCellAccessKind access = type == ObjectType.Camp
                        ? WorldMapCellAccessKind.WalkableFeature
                        : type == ObjectType.Cache ? WorldMapCellAccessKind.UseFromBeside : WorldMapCellAccessKind.SolidObstacle;
                    foreach (bool wide in new[] { false, true })
                        AssertProductionHeading(game, state, 21, 20, wide,
                            ExplorationMapVisualRules.InspectionHeading(name, access, 1, wide));
                }
                map.Objects.Clear();
                map.InvalidateObjectLookup();
                RoamingThreat patrol = new RoamingThreat { Id = "compact-audit-patrol", Name = "Audit patrol", Depth = 2,
                    X = 21, Y = 20, HomeX = 45, HomeY = 44, Active = true };
                state.RoamingThreats.Add(patrol);
                foreach (bool alerted in new[] { false, true })
                foreach (bool wide in new[] { false, true })
                {
                    patrol.Alerted = alerted;
                    AssertProductionHeading(game, state, 21, 20, wide,
                        ExplorationMapVisualRules.InspectionHeading(patrol.Name, WorldMapCellAccessKind.EnemyOccupied,
                            1, wide, alerted ? "Pursuing" : "Enemy"));
                }

                patrol.X = 45;
                patrol.Y = 45;
                state.DiscoveredZones.Add(ExplorationChartRules.CellKey(2, 45, 45));
                patrol.Alerted = false;
                foreach (bool wide in new[] { false, true })
                {
                    string withHiddenPatrol = AssertProductionHeading(game, state, 45, 45, wide, null);
                    state.RoamingThreats.Clear();
                    string withoutPatrol = AssertProductionHeading(game, state, 45, 45, wide, null);
                    Require(withHiddenPatrol == withoutPatrol, "terrain heading is identical with or without an unseen patrol in either view");
                    state.RoamingThreats.Add(patrol);
                }
                patrol.Alerted = true;
                AssertProductionHeading(game, state, 45, 45, true,
                    ExplorationMapVisualRules.InspectionHeading(patrol.Name, WorldMapCellAccessKind.EnemyOccupied, 50, true, "Pursuing"));

                // Fog exits before actors, objects or access classification. Test an
                // alerted enemy too: live alert status must not reveal uncharted cells.
                patrol.X = 59;
                patrol.Y = 59;
                map.Objects.Add(new MapObject(59, 59, ObjectType.Cache, "compact-hidden-cache"));
                map.InvalidateObjectLookup();
                AssertProductionHeading(game, state, 59, 59, true, "Uncharted ground · Move closer to reveal");
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        private static string AssertProductionHeading(AshenHallsGame game, GameState state, int x, int y, bool wide, string expected)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            typeof(AshenHallsGame).GetField("exploreWideView", flags).SetValue(game, wide);
            string before = JsonUtility.ToJson(state);
            object[] args = { x, y, null };
            string detailed = (string)typeof(AshenHallsGame).GetMethod("ExploreLookLineWithHeading", flags).Invoke(game, args);
            string heading = (string)args[2];
            string legacyDetailed = (string)typeof(AshenHallsGame).GetMethod("ExploreLookLine", flags).Invoke(game, new object[] { x, y });
            Require(!string.IsNullOrEmpty(heading), "production inspection supplies a compact heading in every tested branch");
            Require(expected == null || heading == expected, "production compact heading describes actual access at " + x + "," + y + ": " + heading);
            Require(detailed == legacyDetailed, "compact heading leaves the existing detailed inspection contract unchanged");
            Require(JsonUtility.ToJson(state) == before, "heading/detail inspection never mutates serialized campaign state");
            return heading;
        }

        private static void ThreatHeadingsRespectLiveVisibility()
        {
            GameObject host = new GameObject("Map threat heading audit");
            host.SetActive(false);
            try
            {
                const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                var hidden = new RoamingThreat { Id = "hidden", Active = true, Depth = 1, X = 50, Y = 50 };
                var visible = new RoamingThreat { Id = "visible", Active = true, Depth = 1, X = 5, Y = 6 };
                var state = new GameState { Depth = 1, PlayerX = 5, PlayerY = 5,
                    RoamingThreats = new List<RoamingThreat> { hidden, null, visible } };
                typeof(AshenHallsGame).GetField("state", flags).SetValue(game, state);
                MethodInfo nearest = typeof(AshenHallsGame).GetMethod("NearestVisibleExploreThreat", flags);
                Require(ReferenceEquals(nearest.Invoke(game, null), visible), "nearby warnings retain visible patrol identity");
                visible.Active = false;
                Require(nearest.Invoke(game, null) == null, "no heading leaks a distant hidden patrol after visible danger clears");
                visible.Active = true;
                visible.Depth = 2;
                Require(nearest.Invoke(game, null) == null, "another depth never leaks a threat heading");
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("World map visual layout: " + message);
        }
    }
}
