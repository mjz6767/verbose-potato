using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class WorldMapInspectionAuditSmoke
    {
        private const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;

        public static void RunOrThrow()
        {
            GameObject host = new GameObject("World Map inspection audit");
            host.SetActive(false);
            host.hideFlags = HideFlags.HideAndDontSave;
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                MapData map = new MapData { Width = 60, Height = 60, Depth = 2, StartX = 20, StartY = 20 };
                for (int i = 0; i < map.Width * map.Height; i++) map.Tiles.Add(1);
                GameState state = new GameState
                {
                    Mode = GameMode.Explore, Depth = 2, Map = map, PlayerX = 20, PlayerY = 20,
                    ReducedMotion = true, MusicMuted = true, SfxMuted = true
                };
                Set(game, "state", state);
                Set(game, "labSaveBlocked", true);
                Set(game, "launchError", "Map audit fixture presentation held");

                AssertHintPair(game, state, "click to move", "inspect only", "adjacent ground");

                map.Objects.Add(new MapObject(21, 20, ObjectType.Camp, "map-audit-camp"));
                map.InvalidateObjectLookup();
                AssertHintPair(game, state, "click to step onto it", "inspect only", "walkable object");

                map.Objects[0] = new MapObject(21, 20, ObjectType.Cache, "map-audit-cache");
                map.InvalidateObjectLookup();
                AssertHintPair(game, state, "Space/E/A to use from beside", "use from beside in Local Map", "blocking usable object");

                map.Objects.Clear();
                map.InvalidateObjectLookup();
                state.RoamingThreats.Add(new RoamingThreat
                {
                    Id = "map-audit-patrol", Name = "Audit patrol", Depth = 2,
                    X = 21, Y = 20, HomeX = 21, HomeY = 20, Active = true
                });
                AssertHintPair(game, state, "click or move toward it to engage", "inspect only", "adjacent patrol");

                state.RoamingThreats.Clear();
                Set(game, "exploreWideView", true);
                RoamingThreat distant = new RoamingThreat
                {
                    Id = "hidden-audit-patrol", Name = "Hidden audit patrol", Depth = 2,
                    X = 45, Y = 45, HomeX = 45, HomeY = 44, Active = true
                };
                state.RoamingThreats.Add(distant);
                state.DiscoveredZones.Add(ExplorationChartRules.CellKey(2, 45, 45));
                string beforeDistant = JsonUtility.ToJson(state);
                Require(!Look(game, 45, 45).Contains(distant.Name), "remembered terrain does not reveal a distant unalerted patrol");
                string underfoot = (string)typeof(AshenHallsGame).GetMethod("ExploreUnderfootLine", PrivateInstance)
                    .Invoke(game, new object[] { 45, 45 });
                Require(!underfoot.Contains(distant.Name), "Region header does not reveal a hidden patrol either");
                Require(JsonUtility.ToJson(state) == beforeDistant, "distant inspection does not change patrol or discovery state");
                distant.Alerted = true;
                Require(Look(game, 45, 45).Contains(distant.Name), "an alerted patrol remains visible consistently with map markers");
                distant.Alerted = false;
                distant.X = 59;
                distant.Y = 59;
                map.Objects.Add(new MapObject(59, 59, ObjectType.Cache, "hidden-audit-cache"));
                map.InvalidateObjectLookup();
                string beforeFog = JsonUtility.ToJson(state);
                string fog = Look(game, 59, 59);
                Require(fog.StartsWith("Uncharted ground", StringComparison.Ordinal), "unseen Region terrain keeps neutral copy");
                Require(fog.IndexOf("patrol", StringComparison.OrdinalIgnoreCase) < 0, "unseen Region copy reveals no patrol information");
                Require(JsonUtility.ToJson(state) == beforeFog, "fog inspection does not reveal terrain or alter campaign state");

                state.DiscoveredZones.Add(ExplorationChartRules.CellKey(2, 47, 45));
                IReadOnlyList<Point> path = new[] { new Point(45, 45), new Point(46, 45), new Point(47, 45) };
                int prefix = (int)typeof(AshenHallsGame).GetMethod("ExploreGuidanceChartedPrefixCount", PrivateInstance)
                    .Invoke(game, new object[] { path });
                Require(prefix == 1, "live chart prefix stops at the first fog gap without reappearing at a known cell");
                Require(!ExplorationMapGuidanceRules.ShouldShowNextStepCue(path, true, prefix), "Region next-step cue stays out of fog");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(host);
            }
        }

        private static void AssertHintPair(AshenHallsGame game, GameState state, string localHint, string regionHint, string label)
        {
            string before = JsonUtility.ToJson(state);
            Set(game, "exploreWideView", false);
            string local = Look(game, 21, 20);
            Require(local.IndexOf(localHint, StringComparison.Ordinal) >= 0, label + " preserves its actionable Local hint: " + local);
            Set(game, "exploreWideView", true);
            string region = Look(game, 21, 20);
            Require(region.IndexOf(regionHint, StringComparison.Ordinal) >= 0, label + " explains Region inspection: " + region);
            Require(region.IndexOf("click to", StringComparison.OrdinalIgnoreCase) < 0
                && region.IndexOf("click or move", StringComparison.OrdinalIgnoreCase) < 0
                && region.IndexOf("Space/E/A to use", StringComparison.OrdinalIgnoreCase) < 0,
                label + " does not advertise Local travel/use in Region");
            Require(JsonUtility.ToJson(state) == before, label + " inspection leaves all serialized campaign state unchanged");
        }

        private static string Look(AshenHallsGame game, int x, int y)
        {
            return (string)typeof(AshenHallsGame).GetMethod("ExploreLookLine", PrivateInstance).Invoke(game, new object[] { x, y });
        }

        private static void Set(AshenHallsGame game, string field, object value)
        {
            typeof(AshenHallsGame).GetField(field, PrivateInstance).SetValue(game, value);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("World Map inspection audit: " + message);
        }
    }
}
