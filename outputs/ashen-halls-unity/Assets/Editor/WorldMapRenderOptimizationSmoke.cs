using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class WorldMapRenderOptimizationSmoke
    {
        private const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;

        public static void RunOrThrow()
        {
            IndexPreservesThreatSemantics();
            ViewportQueriesDoNotRescanTheRoster();
            RenderScopeNeverReplacesLiveGameplayQueries();
        }

        private static void IndexPreservesThreatSemantics()
        {
            RoamingThreat active = Threat("active", 2, 6, 7, 4, 5);
            RoamingThreat cleared = Threat("cleared", 2, 9, 10, 8, 9);
            cleared.Active = false;
            RoamingThreat[] roster =
            {
                active, cleared, null, Threat("other-depth", 3, 12, 13, 14, 15),
                Threat("", 2, 16, 17, 18, 19), Threat(null, 2, 20, 21, 22, 23),
                Threat("duplicate-home", 2, -1, int.MaxValue, 8, 9)
            };
            ExplorationThreatRenderIndex index = new ExplorationThreatRenderIndex();
            index.Rebuild(roster, 2);
            Require(index.HasHome(4, 5) && index.HasActiveHome(4, 5), "active homes retain their blocking presentation");
            Require(index.HasActivePatrol(6, 7), "active patrols use their current position, not home");
            Require(!index.HasActivePatrol(4, 5), "an empty habitat is not painted as an occupied patrol tile");
            Require(index.HasHome(8, 9) && index.HasActiveHome(8, 9), "an active duplicate wins over a cleared home");
            Require(!index.HasActivePatrol(9, 10), "cleared patrols do not occupy terrain");
            Require(!index.HasHome(14, 15) && !index.HasActivePatrol(12, 13), "other depths are excluded");
            Require(index.HasActiveHome(18, 19) && !index.HasActivePatrol(16, 17), "empty IDs preserve the live query's default exclusion");
            Require(index.HasActivePatrol(20, 21), "null IDs preserve the live query's distinct null behavior");
            Require(index.HasActivePatrol(-1, int.MaxValue) && !index.HasActivePatrol(int.MaxValue, -1), "coordinate pairs cannot alias");

            active.Active = false;
            active.HomeX = 30;
            active.HomeY = 31;
            index.Rebuild(new[] { active }, 2);
            Require(!index.HasHome(4, 5) && !index.HasActivePatrol(6, 7), "the next repaint removes old homes and cleared patrols");
            Require(index.HasHome(30, 31) && !index.HasActiveHome(30, 31), "cleared aftermath remains distinct from an active habitat");
            index.Rebuild(roster, 3);
            Require(index.HasHome(14, 15) && !index.HasHome(30, 31), "depth changes rebuild rather than retaining old occupancy");
            index.Rebuild(null, 3);
            Require(!index.HasHome(14, 15) && !index.HasActivePatrol(12, 13), "missing rosters clear all presentation state");
        }

        private static void ViewportQueriesDoNotRescanTheRoster()
        {
            List<RoamingThreat> threats = new List<RoamingThreat>();
            for (int i = 0; i < 32; i++) threats.Add(Threat("threat-" + i, 2, i + 30, 40, i + 30, 39));
            CountingRoster roster = new CountingRoster(threats);
            ExplorationThreatRenderIndex index = new ExplorationThreatRenderIndex();
            index.Rebuild(roster, 2);
            Require(roster.Reads == 32, "each source threat is read once at repaint start");
            for (int y = 0; y < 11; y++)
            for (int x = 0; x < 21; x++)
            {
                Require(!index.HasHome(x, y) && !index.HasActiveHome(x, y) && !index.HasActivePatrol(x, y), "empty viewport stays empty");
            }
            Require(roster.Reads == 32, "693 viewport occupancy queries require no additional roster reads (versus 22,176 worst-case linear visits)");
            index.Rebuild(roster, 2);
            Require(roster.Reads == 64, "every repaint refreshes exactly once instead of using a stale cross-frame cache");
        }

        private static void RenderScopeNeverReplacesLiveGameplayQueries()
        {
            GameObject host = new GameObject("World map render index audit");
            host.SetActive(false);
            host.hideFlags = HideFlags.HideAndDontSave;
            try
            {
                AshenHallsGame game = host.AddComponent<AshenHallsGame>();
                RoamingThreat threat = Threat("live", 2, 6, 7, 4, 5);
                GameState state = new GameState { Depth = 2, Map = new MapData(), RoamingThreats = new List<RoamingThreat> { threat } };
                Set(game, state);
                string before = JsonUtility.ToJson(state);
                Call(game, "BeginExploreWorldArtFrame");
                Require(Query(game, "IsRoamingThreatHomeCell", 4, 5) && Query(game, "IsActiveRoamingThreatHabitatCell", 4, 5), "production home queries use the current frame snapshot");
                Require(Query(game, "IsRoamingThreatRenderCell", 6, 7), "production scenery query uses current patrol occupancy");
                Require(JsonUtility.ToJson(state) == before, "building and reading a render index never changes the campaign");

                threat.Active = false;
                Call(game, "EndExploreWorldArtFrame");
                Require(!Query(game, "IsActiveRoamingThreatHabitatCell", 4, 5) && !Query(game, "IsRoamingThreatRenderCell", 6, 7), "queries outside painting read live state after a clear");
                Call(game, "BeginExploreWorldArtFrame");
                Require(Query(game, "IsRoamingThreatHomeCell", 4, 5) && !Query(game, "IsActiveRoamingThreatHabitatCell", 4, 5), "the next repaint distinguishes aftermath");

                state.Map = new MapData();
                threat.Active = true;
                Require(Query(game, "IsRoamingThreatRenderCell", 6, 7), "a replaced map invalidates an in-flight snapshot");
                Call(game, "BeginExploreWorldArtFrame");
                state.RoamingThreats = new List<RoamingThreat>();
                Require(!Query(game, "IsRoamingThreatHomeCell", 4, 5), "a replaced roster also invalidates a snapshot");
                state.RoamingThreats.Add(threat);
                Call(game, "BeginExploreWorldArtFrame");
                state.Depth = 3;
                Require(!Query(game, "IsRoamingThreatHomeCell", 4, 5), "depth switches cannot leak previous-floor habitat presentation");
                Set(game, null);
                Require(!Query(game, "IsRoamingThreatRenderCell", 6, 7), "an unloaded campaign cannot retain render occupancy");
                Call(game, "EndExploreWorldArtFrame");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(host);
            }
        }

        private static RoamingThreat Threat(string id, int depth, int x, int y, int homeX, int homeY)
        {
            return new RoamingThreat { Id = id, Depth = depth, X = x, Y = y, HomeX = homeX, HomeY = homeY, Active = true };
        }

        private static object Call(AshenHallsGame game, string method, params object[] args)
        {
            return typeof(AshenHallsGame).GetMethod(method, PrivateInstance).Invoke(game, args);
        }

        private static bool Query(AshenHallsGame game, string method, int x, int y) => (bool)Call(game, method, x, y);
        private static void Set(AshenHallsGame game, GameState state) => typeof(AshenHallsGame).GetField("state", PrivateInstance).SetValue(game, state);
        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("World map render optimization: " + message);
        }

        private sealed class CountingRoster : IReadOnlyList<RoamingThreat>
        {
            private readonly List<RoamingThreat> threats;
            public int Reads { get; private set; }
            public int Count => threats.Count;
            public RoamingThreat this[int index] { get { Reads++; return threats[index]; } }
            public CountingRoster(List<RoamingThreat> threats) { this.threats = threats; }
            public IEnumerator<RoamingThreat> GetEnumerator()
            {
                for (int i = 0; i < Count; i++) yield return this[i];
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
