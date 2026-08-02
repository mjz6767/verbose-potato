using System;

namespace AshenHalls
{
    public enum WorldMapCellAccessKind
    {
        OpenGround,
        SoftScenery,
        WalkableFeature,
        UseFromBeside,
        SolidObstacle,
        BlockedTerrain,
        EnemyOccupied
    }

    public enum WorldMapMovementCueKind
    {
        OpenTick,
        UseBracket,
        StopBar,
        ThreatDoubleBar
    }

    public static class ExplorationReadabilityRules
    {
        public static WorldMapCellAccessKind ClassifyCellAccess(
            int tile,
            MapObject mapObject,
            bool hasSoftScenery,
            bool enemyOccupied)
        {
            // Threats are authoritative. Ground and object access are then read
            // together so barred wall-tile gates can still advertise adjacent use
            // without implying that the underlying terrain is standable.
            if (enemyOccupied) return WorldMapCellAccessKind.EnemyOccupied;
            if (mapObject != null)
            {
                if (tile == 1 && ExplorationTraversalRules.CanStandOnObject(mapObject))
                {
                    return WorldMapCellAccessKind.WalkableFeature;
                }
                // Barred north/south gates are authored into wall terrain but still
                // resolve when approached. Adjacent-use semantics must outrank the
                // ground tile so the cue matches input handling.
                if (ExplorationTraversalRules.CanUseFromAdjacent(mapObject))
                {
                    return WorldMapCellAccessKind.UseFromBeside;
                }
                return tile == 1
                    ? WorldMapCellAccessKind.SolidObstacle
                    : WorldMapCellAccessKind.BlockedTerrain;
            }
            if (tile != 1) return WorldMapCellAccessKind.BlockedTerrain;
            return hasSoftScenery
                ? WorldMapCellAccessKind.SoftScenery
                : WorldMapCellAccessKind.OpenGround;
        }

        public static bool IsWalkableAccess(WorldMapCellAccessKind access)
        {
            return access == WorldMapCellAccessKind.OpenGround
                || access == WorldMapCellAccessKind.SoftScenery
                || access == WorldMapCellAccessKind.WalkableFeature;
        }

        public static WorldMapMovementCueKind MovementCueKind(WorldMapCellAccessKind access)
        {
            switch (access)
            {
                case WorldMapCellAccessKind.OpenGround:
                case WorldMapCellAccessKind.SoftScenery:
                case WorldMapCellAccessKind.WalkableFeature:
                    return WorldMapMovementCueKind.OpenTick;
                case WorldMapCellAccessKind.UseFromBeside:
                    return WorldMapMovementCueKind.UseBracket;
                case WorldMapCellAccessKind.EnemyOccupied:
                    return WorldMapMovementCueKind.ThreatDoubleBar;
                default:
                    return WorldMapMovementCueKind.StopBar;
            }
        }

        public static float TerrainArtAlpha(int tile, string kind, bool wideView, float noise01)
        {
            kind = kind ?? "";
            // Opacity is a layer contract, not a per-cell texture variant. Random
            // alpha made neighboring copies of the same ground visibly checkerboard.
            noise01 = 0.5f;
            bool wallLike = tile == 0 || string.Equals(kind, "midgaardwall", StringComparison.Ordinal);

            if (wallLike)
            {
                return Lerp(0.90f, 0.96f, noise01);
            }

            if (kind.StartsWith("midgaard", StringComparison.Ordinal))
            {
                return wideView ? Lerp(0.60f, 0.68f, noise01) : Lerp(0.74f, 0.82f, noise01);
            }

            switch (kind)
            {
                case "road":
                case "outworks":
                    return wideView ? Lerp(0.68f, 0.76f, noise01) : Lerp(0.76f, 0.84f, noise01);
                case "paved":
                case "ruins":
                case "gloam":
                    return wideView ? Lerp(0.56f, 0.65f, noise01) : Lerp(0.70f, 0.79f, noise01);
                case "moss":
                case "mire":
                case "mud":
                case "cistern":
                    return wideView ? Lerp(0.61f, 0.70f, noise01) : Lerp(0.75f, 0.84f, noise01);
                case "quarry":
                case "glass":
                case "ash":
                    return wideView ? Lerp(0.62f, 0.71f, noise01) : Lerp(0.77f, 0.86f, noise01);
                default:
                    return wideView ? Lerp(0.59f, 0.68f, noise01) : Lerp(0.74f, 0.82f, noise01);
            }
        }

        public static float InteractiveObjectBackdropAlpha(bool quiet, bool framed)
        {
            // Ordinary actors should sit in the world, not on dark UI cards. The
            // renderer supplies a contact shadow/offset silhouette instead. Only a
            // current target or quest objective receives a restrained backing.
            if (!framed) return 0f;
            return quiet ? 0.18f : 0.22f;
        }

        public static bool ShouldUseStrongObjectFrame(
            bool wideView,
            bool objective,
            bool current,
            bool hovered,
            bool adjacent,
            bool persistentLocalType)
        {
            if (objective || current || hovered || adjacent) return true;
            return !wideView && persistentLocalType;
        }

        public static float JunctionMarkerAlpha(
            bool hasJunction,
            bool waypoint,
            bool charted,
            bool nearby)
        {
            if (!hasJunction) return 0f;
            if (waypoint) return 0.76f;
            if (nearby) return 0.42f;
            if (charted) return 0.26f;
            return 0.10f;
        }

        public static bool IsDetailedCell(int distanceToParty, bool wideView)
        {
            return distanceToParty <= (wideView ? 7 : 8);
        }

        public static float DecorativeAlphaScale(bool wideView)
        {
            return wideView ? 0.78f : 0.92f;
        }

        public static float MidgaardPropAlpha(bool wideView, float noise01)
        {
            // Decorative street life stays legible while remaining visibly behind
            // authored buildings, named actors, and movement affordances.
            return wideView
                ? Lerp(0.56f, 0.66f, noise01)
                : Lerp(0.78f, 0.84f, noise01);
        }

        public static float BiomePropAlpha(bool wideView, float noise01)
        {
            return wideView
                ? Lerp(0.52f, 0.64f, noise01)
                : Lerp(0.80f, 0.88f, noise01);
        }

        public static float DecorativeDensityScale(bool wideView)
        {
            return wideView ? 0.22f : 0.32f;
        }

        public static bool ShouldDrawMidgaardPavingDecal(bool wideView, int distanceToParty, bool hasObject, int roll)
        {
            // Ground marks belong to the close reading view. Keep the party's
            // immediate navigation cells and every authored object cell quiet.
            if (wideView || hasObject || distanceToParty <= 1) return false;
            return PositiveMod(roll, 100) < 10;
        }

        public static float MidgaardPavingDecalAlpha(float noise01)
        {
            return Lerp(0.50f, 0.68f, noise01);
        }

        public static bool ShouldDrawProceduralGroundAccent(int distanceToParty, bool hasObject, bool wideView, int roll)
        {
            if (hasObject || distanceToParty <= 2) return false;
            int threshold = wideView ? 8 : 14;
            return PositiveMod(roll, 100) < threshold;
        }

        public static bool ShouldDrawBiomeAmbientProp(string kind, bool wideView, int roll)
        {
            if (wideView && PositiveMod(roll, 100) >= 12) return false;
            switch (kind ?? "")
            {
                case "moss":
                case "mire":
                case "mud":
                case "quarry":
                case "glass":
                case "ash":
                case "red":
                case "road":
                case "paved":
                case "ruins":
                case "gloam":
                case "cistern":
                case "outworks":
                    return true;
                default:
                    return false;
            }
        }

        public static bool ShouldPreferCalmGroundUnderFocus(int distanceToParty, bool hasObject)
        {
            return hasObject || distanceToParty <= 1;
        }

        private static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * Clamp01(t);
        }

        private static float Clamp01(float value)
        {
            if (value <= 0f) return 0f;
            if (value >= 1f) return 1f;
            return value;
        }

        private static int PositiveMod(int value, int divisor)
        {
            int result = value % divisor;
            return result < 0 ? result + divisor : result;
        }
    }

    public static class WorldMapRegionLandmarkCatalog
    {
        public const int Columns = 5;
        public const int Rows = 4;
        public const int CellCount = Columns * Rows;

        public static int IconIndex(string zoneId, ObjectType type)
        {
            switch ((zoneId ?? "").ToLowerInvariant())
            {
                case "green-shrine-road":
                    switch (type)
                    {
                        case ObjectType.Shrine: return 0;
                        case ObjectType.AncientGrove:
                        case ObjectType.TrainingGround: return 1;
                        case ObjectType.Camp: return 2;
                        case ObjectType.Waystone:
                        case ObjectType.Obelisk: return 3;
                        case ObjectType.Bridge: return 4;
                        default: return -1;
                    }
                case "old-quarry":
                    switch (type)
                    {
                        case ObjectType.ForgeSite:
                        case ObjectType.Obelisk: return 5;
                        case ObjectType.Cache: return 6;
                        case ObjectType.Cave:
                        case ObjectType.DungeonGate: return 7;
                        case ObjectType.Ruin: return 8;
                        case ObjectType.Bridge: return 9;
                        default: return -1;
                    }
                case "glass-warrens":
                    switch (type)
                    {
                        case ObjectType.Obelisk: return 10;
                        case ObjectType.Shrine:
                        case ObjectType.LoreLibrary: return 11;
                        case ObjectType.Cave:
                        case ObjectType.DungeonGate: return 12;
                        case ObjectType.Ruin: return 13;
                        case ObjectType.Cache: return 14;
                        default: return -1;
                    }
                case "ash-fen":
                case "red-gate":
                    switch (type)
                    {
                        case ObjectType.Camp: return 15;
                        case ObjectType.Shrine:
                        case ObjectType.AncientGrove: return 16;
                        case ObjectType.Obelisk: return 17;
                        case ObjectType.Ruin:
                        case ObjectType.Cave:
                        case ObjectType.PortalSeal: return 18;
                        case ObjectType.Cache: return 19;
                        default: return -1;
                    }
                default:
                    return -1;
            }
        }
    }
}
