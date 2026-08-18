namespace AshenHalls
{
    public enum ExplorationRoadVisualTier
    {
        None = 0,
        Trail = 1,
        CityStreet = 2,
        Road = 3,
        OldRoad = 4,
        Bridge = 5
    }

    public enum ExplorationRoadJoin
    {
        None = 0,
        Endpoint = 1,
        Straight = 2,
        Corner = 3,
        Tee = 4,
        Cross = 5
    }

    public readonly struct ExplorationRoadVisualPlan
    {
        public readonly ExplorationRoadVisualTier Tier;
        public readonly ExplorationRoadJoin Join;
        public readonly int MainMask;
        public readonly int ConnectorMask;
        public readonly float ShoulderFraction;
        public readonly float CoreFraction;
        public readonly bool DrawCenterWear;

        public ExplorationRoadVisualPlan(
            ExplorationRoadVisualTier tier,
            ExplorationRoadJoin join,
            int mainMask,
            int connectorMask,
            float shoulderFraction,
            float coreFraction,
            bool drawCenterWear)
        {
            Tier = tier;
            Join = join;
            MainMask = mainMask;
            ConnectorMask = connectorMask;
            ShoulderFraction = shoulderFraction;
            CoreFraction = coreFraction;
            DrawCenterWear = drawCenterWear;
        }

        public bool Draw => Tier != ExplorationRoadVisualTier.None;
    }

    public static class ExplorationRoadPresentationRules
    {
        public const int North = 1;
        public const int East = 2;
        public const int South = 4;
        public const int West = 8;
        public const int CardinalMask = North | East | South | West;

        private const ExplorationCellRole RoadRoles = ExplorationCellRole.Road | ExplorationCellRole.Bridge;
        private const ExplorationCellRole TrailAnchorRoles = ExplorationCellRole.Threshold | ExplorationCellRole.Clearing;

        public static ExplorationRoadVisualPlan Resolve(
            MapData map,
            int x,
            int y,
            bool wideView,
            bool oldRoad)
        {
            ExplorationCellRole roles = ExplorationSurfaceRules.RolesAt(map, x, y);
            int roadMask = NeighborMask(map, x, y, RoadRoles, false);
            int trailMask = NeighborMask(map, x, y, ExplorationCellRole.Trail, true);
            int visibleTrailMask = VisibleTrailNeighborMask(map, x, y, wideView);
            return Resolve(roles, wideView, oldRoad, roadMask, trailMask, visibleTrailMask);
        }

        public static ExplorationRoadVisualPlan Resolve(
            ExplorationCellRole roles,
            bool wideView,
            bool oldRoad,
            int roadNeighborMask,
            int trailNeighborMask,
            int visibleTrailNeighborMask)
        {
            roadNeighborMask &= CardinalMask;
            trailNeighborMask &= CardinalMask;
            visibleTrailNeighborMask &= CardinalMask;

            bool road = (roles & ExplorationCellRole.Road) != 0;
            bool bridge = (roles & ExplorationCellRole.Bridge) != 0;
            bool pureTrail = IsPureTrail(roles);
            if (!road && !bridge && !pureTrail) return EmptyPlan();
            if (pureTrail && !ShouldDrawTrail(wideView, roles, roadNeighborMask, trailNeighborMask)) return EmptyPlan();

            ExplorationRoadVisualTier tier;
            int mainMask;
            int connectorMask;
            if (bridge)
            {
                tier = ExplorationRoadVisualTier.Bridge;
                mainMask = roadNeighborMask;
                connectorMask = wideView ? 0 : visibleTrailNeighborMask & ~mainMask;
            }
            else if (road)
            {
                tier = oldRoad
                    ? ExplorationRoadVisualTier.OldRoad
                    : (roles & ExplorationCellRole.City) != 0
                        ? ExplorationRoadVisualTier.CityStreet
                        : ExplorationRoadVisualTier.Road;
                mainMask = oldRoad ? roadNeighborMask & (East | West) : roadNeighborMask;
                int roadBranchMask = oldRoad ? roadNeighborMask & ~mainMask : 0;
                connectorMask = roadBranchMask
                    | (wideView ? 0 : visibleTrailNeighborMask & ~mainMask);
            }
            else
            {
                tier = ExplorationRoadVisualTier.Trail;
                mainMask = visibleTrailNeighborMask | roadNeighborMask;
                connectorMask = 0;
            }

            ExplorationRoadJoin join = JoinForMask(mainMask);
            float shoulder = ShoulderFraction(tier, wideView);
            float core = CoreFraction(tier, wideView);
            bool centerWear = !wideView
                && tier == ExplorationRoadVisualTier.OldRoad
                && join == ExplorationRoadJoin.Straight;
            return new ExplorationRoadVisualPlan(tier, join, mainMask, connectorMask, shoulder, core, centerWear);
        }

        public static bool ShouldDrawTrail(
            bool wideView,
            ExplorationCellRole roles,
            int roadNeighborMask,
            int trailNeighborMask)
        {
            if (!IsPureTrail(roles) || wideView) return false;
            if ((roles & TrailAnchorRoles) != 0) return true;
            roadNeighborMask &= CardinalMask;
            trailNeighborMask &= CardinalMask;
            return roadNeighborMask == 0 || HasCollinearRoadDeparture(roadNeighborMask, trailNeighborMask);
        }

        public static bool HasCollinearRoadDeparture(int roadNeighborMask, int trailNeighborMask)
        {
            roadNeighborMask &= CardinalMask;
            trailNeighborMask &= CardinalMask;
            return ((roadNeighborMask & North) != 0 && (trailNeighborMask & South) != 0)
                || ((roadNeighborMask & East) != 0 && (trailNeighborMask & West) != 0)
                || ((roadNeighborMask & South) != 0 && (trailNeighborMask & North) != 0)
                || ((roadNeighborMask & West) != 0 && (trailNeighborMask & East) != 0);
        }

        public static ExplorationRoadJoin JoinForMask(int mask)
        {
            mask &= CardinalMask;
            int count = BitCount(mask);
            if (count == 0) return ExplorationRoadJoin.None;
            if (count == 1) return ExplorationRoadJoin.Endpoint;
            if (count == 2)
            {
                bool opposite = mask == (North | South) || mask == (East | West);
                return opposite ? ExplorationRoadJoin.Straight : ExplorationRoadJoin.Corner;
            }
            return count == 3 ? ExplorationRoadJoin.Tee : ExplorationRoadJoin.Cross;
        }

        private static bool IsPureTrail(ExplorationCellRole roles)
        {
            return (roles & ExplorationCellRole.Trail) != 0 && (roles & RoadRoles) == 0;
        }

        private static int VisibleTrailNeighborMask(MapData map, int x, int y, bool wideView)
        {
            int mask = 0;
            if (ShouldDrawTrailAt(map, x, y - 1, wideView)) mask |= North;
            if (ShouldDrawTrailAt(map, x + 1, y, wideView)) mask |= East;
            if (ShouldDrawTrailAt(map, x, y + 1, wideView)) mask |= South;
            if (ShouldDrawTrailAt(map, x - 1, y, wideView)) mask |= West;
            return mask;
        }

        private static bool ShouldDrawTrailAt(MapData map, int x, int y, bool wideView)
        {
            if (!IsOpen(map, x, y)) return false;
            ExplorationCellRole roles = ExplorationSurfaceRules.RolesAt(map, x, y);
            int roadMask = NeighborMask(map, x, y, RoadRoles, false);
            int trailMask = NeighborMask(map, x, y, ExplorationCellRole.Trail, true);
            return ShouldDrawTrail(wideView, roles, roadMask, trailMask);
        }

        private static int NeighborMask(
            MapData map,
            int x,
            int y,
            ExplorationCellRole roles,
            bool pureTrail)
        {
            int mask = 0;
            if (HasNeighborRoles(map, x, y - 1, roles, pureTrail)) mask |= North;
            if (HasNeighborRoles(map, x + 1, y, roles, pureTrail)) mask |= East;
            if (HasNeighborRoles(map, x, y + 1, roles, pureTrail)) mask |= South;
            if (HasNeighborRoles(map, x - 1, y, roles, pureTrail)) mask |= West;
            return mask;
        }

        private static bool HasNeighborRoles(
            MapData map,
            int x,
            int y,
            ExplorationCellRole roles,
            bool pureTrail)
        {
            if (!IsOpen(map, x, y)) return false;
            ExplorationCellRole neighborRoles = ExplorationSurfaceRules.RolesAt(map, x, y);
            return pureTrail ? IsPureTrail(neighborRoles) : (neighborRoles & roles) != 0;
        }

        private static bool IsOpen(MapData map, int x, int y)
        {
            if (map == null || map.Tiles == null || x < 0 || y < 0 || x >= map.Width || y >= map.Height) return false;
            int index = y * map.Width + x;
            return index >= 0 && index < map.Tiles.Count && map.Tiles[index] == 1;
        }

        private static float ShoulderFraction(ExplorationRoadVisualTier tier, bool wideView)
        {
            switch (tier)
            {
                case ExplorationRoadVisualTier.Trail: return 0.18f;
                case ExplorationRoadVisualTier.CityStreet: return wideView ? 0.26f : 0.32f;
                case ExplorationRoadVisualTier.Road: return wideView ? 0.34f : 0.42f;
                case ExplorationRoadVisualTier.OldRoad: return wideView ? 0.54f : 0.64f;
                case ExplorationRoadVisualTier.Bridge: return wideView ? 0.38f : 0.46f;
                default: return 0f;
            }
        }

        private static float CoreFraction(ExplorationRoadVisualTier tier, bool wideView)
        {
            switch (tier)
            {
                case ExplorationRoadVisualTier.Trail: return 0.12f;
                case ExplorationRoadVisualTier.CityStreet: return wideView ? 0.17f : 0.21f;
                case ExplorationRoadVisualTier.Road: return wideView ? 0.22f : 0.28f;
                case ExplorationRoadVisualTier.OldRoad: return wideView ? 0.36f : 0.43f;
                case ExplorationRoadVisualTier.Bridge: return wideView ? 0.25f : 0.31f;
                default: return 0f;
            }
        }

        private static int BitCount(int value)
        {
            int count = 0;
            while (value != 0)
            {
                value &= value - 1;
                count++;
            }
            return count;
        }

        private static ExplorationRoadVisualPlan EmptyPlan()
        {
            return new ExplorationRoadVisualPlan(
                ExplorationRoadVisualTier.None,
                ExplorationRoadJoin.None,
                0,
                0,
                0f,
                0f,
                false);
        }
    }
}
