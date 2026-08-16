using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public readonly struct WorldMapJunction
    {
        public readonly string Id;
        public readonly string ZoneId;
        public readonly string Name;
        public readonly string Summary;
        public readonly int X;
        public readonly int Y;

        public WorldMapJunction(string id, string zoneId, string name, string summary, int x, int y)
        {
            Id = id ?? "";
            ZoneId = zoneId ?? "";
            Name = name ?? "";
            Summary = summary ?? "";
            X = x;
            Y = y;
        }
    }

    public readonly struct WorldMapSite
    {
        public readonly string Id;
        public readonly string ZoneId;
        public readonly string Name;
        public readonly string Summary;
        public readonly int X;
        public readonly int Y;
        public readonly int Radius;
        public readonly ObjectType Type;

        public WorldMapSite(
            string id,
            string zoneId,
            string name,
            string summary,
            int x,
            int y,
            int radius,
            ObjectType type)
        {
            Id = id ?? "";
            ZoneId = zoneId ?? "";
            Name = name ?? "";
            Summary = summary ?? "";
            X = x;
            Y = y;
            Radius = Math.Max(0, radius);
            Type = type;
        }
    }

    public static class WorldMapGenerationRules
    {
        public const int Width = 58;
        public const int Height = 46;
        public const int PreviousWidth = 50;
        public const int PreviousHeight = 32;
        public const int LegacyWidth = 46;
        public const int LegacyHeight = 30;
        public const string OldRoadName = "The Old Road";
        public const string OldRoadWestJunctionId = "pilgrim-fork";
        public const string OldRoadEastJunctionId = "lanternless-cross";

        private static readonly object RegionalCacheSync = new object();
        private static readonly Dictionary<RegionalCacheKey, WorldMapJunction[]> RegionalJunctionCache =
            new Dictionary<RegionalCacheKey, WorldMapJunction[]>();
        private static readonly Dictionary<RegionalCacheKey, WorldMapSite[]> RegionalSiteCache =
            new Dictionary<RegionalCacheKey, WorldMapSite[]>();

        public static int StartX(int width)
        {
            if (width <= 0) return 0;
            if (width < 3) return width / 2;
            return Clamp(width / 2, 1, width - 2);
        }

        public static int StartY(int height)
        {
            if (height <= 0) return 0;
            if (height < 3) return height / 2;

            // The one-row southern bias keeps fresh Midgaard clear of the fixed
            // top interior reserve without materially unbalancing the outer regions.
            return Clamp(height / 2 + 1, 1, height - 2);
        }

        public static int DistrictCount(int depth)
        {
            return 8 + Math.Min(Math.Max(0, depth), 5);
        }

        public static int LoopRoadCount(int depth)
        {
            return 4 + Math.Min(Math.Max(0, depth), 5);
        }

        public static int WanderSteps(int depth, int randomExtra)
        {
            int clampedExtra = Math.Max(0, Math.Min(49, randomExtra));
            return 156 + Math.Max(0, depth) * 12 + clampedExtra;
        }

        public static WorldMapJunction[] RegionalJunctions(int width, int height, int startX, int startY)
        {
            if (width < 12 || height < 12) return Array.Empty<WorldMapJunction>();

            RegionalCacheKey key = new RegionalCacheKey(width, height, startX, startY);
            lock (RegionalCacheSync)
            {
                if (RegionalJunctionCache.TryGetValue(key, out WorldMapJunction[] cached)) return cached;

                WorldMapJunction[] junctions = BuildRegionalJunctions(width, height, startX, startY);
                RegionalJunctionCache.Add(key, junctions);
                return junctions;
            }
        }

        public static bool TryOldRoadEndpoints(
            int width,
            int height,
            int startX,
            int startY,
            out WorldMapJunction west,
            out WorldMapJunction east)
        {
            west = default(WorldMapJunction);
            east = default(WorldMapJunction);
            foreach (WorldMapJunction junction in RegionalJunctions(width, height, startX, startY))
            {
                if (string.Equals(junction.Id, OldRoadWestJunctionId, StringComparison.Ordinal))
                {
                    west = junction;
                }
                else if (string.Equals(junction.Id, OldRoadEastJunctionId, StringComparison.Ordinal))
                {
                    east = junction;
                }
            }

            return !string.IsNullOrEmpty(west.Id)
                && !string.IsNullOrEmpty(east.Id)
                && west.Y == east.Y
                && west.X <= east.X;
        }

        public static bool IsOldRoadCenterlineCell(
            int width,
            int height,
            int startX,
            int startY,
            int x,
            int y)
        {
            return TryOldRoadEndpoints(width, height, startX, startY, out WorldMapJunction west, out WorldMapJunction east)
                && y == west.Y
                && x >= west.X
                && x <= east.X;
        }

        public static WorldMapSite[] RegionalSites(int width, int height, int startX, int startY)
        {
            if (width < 45 || height < 28) return Array.Empty<WorldMapSite>();

            RegionalCacheKey key = new RegionalCacheKey(width, height, startX, startY);
            lock (RegionalCacheSync)
            {
                if (RegionalSiteCache.TryGetValue(key, out WorldMapSite[] cached)) return cached;

                WorldMapSite[] sites = BuildRegionalSites(width, height, startX, startY);
                RegionalSiteCache.Add(key, sites);
                return sites;
            }
        }

        private static WorldMapJunction[] BuildRegionalJunctions(int width, int height, int startX, int startY)
        {
            int leftX = Clamp((int)Math.Round((width - 1) * 0.16f), 2, width - 3);
            int rightX = Clamp((width - 1) - leftX, 2, width - 3);
            int topY = Clamp((int)Math.Round((height - 1) * 0.16f), 2, height - 3);
            int bottomY = Clamp((height - 1) - topY, 2, height - 3);
            // Keep the north-corner junctions beyond the one-cell safety margin
            // around Midgaard's embedded rooms on both 50x32 saves and fresh maps.
            int cornerLeftX = Clamp(Math.Max(13, leftX + 4), 2, width - 3);
            int cornerRightX = Clamp(Math.Min(width - 14, rightX - 4), 2, width - 3);
            int centerX = Clamp(startX, 2, width - 3);
            int centerY = Clamp(startY, 2, height - 3);

            return new[]
            {
                new WorldMapJunction("pilgrim-fork", "green-shrine-road", "Pilgrim Fork", "Mossed stones split the shrine road from the western circuit.", leftX, centerY),
                new WorldMapJunction("quarry-turn", "old-quarry", "Quarry Turn", "Chisel marks point toward the abandoned stone yards.", cornerLeftX, topY),
                new WorldMapJunction("courtward-arch", "gloam-courts", "Courtward Arch", "A broken arch frames the dark road into the upper courts.", centerX, topY),
                new WorldMapJunction("glassward-bend", "glass-warrens", "Glassward Bend", "Cold splinters glitter where the road turns toward the warrens.", cornerRightX, topY),
                new WorldMapJunction("lanternless-cross", "dusk-market", "Lanternless Cross", "Four dead lamp posts mark the market road and its ambush lanes.", rightX, centerY),
                new WorldMapJunction("red-mile-junction", "red-gate", "Red Mile Junction", "Red basalt chips warn that the old war gate lies ahead.", cornerRightX, bottomY),
                new WorldMapJunction("sluice-steps", "salt-cisterns", "Sluice Steps", "Wet-cut stairs and drain marks descend toward the cistern road.", centerX, bottomY),
                new WorldMapJunction("fen-causeway", "ash-fen", "Fen Causeway", "Raised stones carry the road over ash water and poison reeds.", cornerLeftX, bottomY)
            };
        }

        private static WorldMapSite[] BuildRegionalSites(int width, int height, int startX, int startY)
        {
            int leftX = Clamp((int)Math.Round((width - 1) * 0.16f), 2, width - 3);
            int rightX = Clamp((width - 1) - leftX, 2, width - 3);
            int topY = Clamp((int)Math.Round((height - 1) * 0.16f), 2, height - 3);
            int bottomY = Clamp((height - 1) - topY, 2, height - 3);
            int cornerLeftX = Clamp(Math.Max(13, leftX + 4), 2, width - 3);
            int cornerRightX = Clamp(Math.Min(width - 14, rightX - 4), 2, width - 3);
            int centerX = Clamp(startX, 2, width - 3);
            int centerY = Clamp(startY, 2, height - 3);

            int westSiteX = ClampSiteCenter(leftX - 3, width, 3);
            int eastSiteX = ClampSiteCenter(rightX + 3, width, 3);
            int northMaxY = (int)Math.Ceiling(height * 0.35f) - 1;
            int topSiteY = ClampSiteCenter(
                Math.Min(
                    Math.Max(8, (int)Math.Round((height - 1) * 0.24f)),
                    northMaxY - 2),
                height,
                2);
            int bottomSmallSiteY = ClampSiteCenter(bottomY + 2, height, 2);
            int bottomLargeSiteY = ClampSiteCenter(bottomY + 2, height, 3);
            int middleMinY = Clamp((int)Math.Ceiling(height * 0.35f), 4, height - 5);
            int middleMaxY = Clamp((int)Math.Floor(height * 0.65f), middleMinY, height - 5);
            int middleSiteMinY = middleMinY + 3;
            int middleSiteMaxY = middleMaxY - 3;
            int westSiteY = ClampSiteCenter(
                Clamp(centerY + 2, middleSiteMinY, middleSiteMaxY),
                height,
                3);
            int eastSiteY = ClampSiteCenter(
                Clamp(centerY - 2, middleSiteMinY, middleSiteMaxY),
                height,
                3);
            int centerMinX = (int)Math.Ceiling(width * 0.34f);
            int centerMaxX = (int)Math.Floor(width * 0.66f);
            int centerSiteX = ClampSiteCenter(Clamp(centerX, centerMinX, centerMaxX), width, 2);

            // The north-corner centers and their radius-two footprints sit wholly
            // outside the fixed throne room (x 2..11) and merchant hall
            // (x width-12..width-3), including on 50x32 maps.
            int cornerLeftSiteX = ClampSiteCenter(Math.Max(14, cornerLeftX + 2), width, 2);
            int cornerRightSiteX = ClampSiteCenter(
                Math.Min(width - 15, cornerRightX - 2),
                width,
                2);

            return new[]
            {
                new WorldMapSite(
                    "green-shrine-training-ring",
                    "green-shrine-road",
                    "Green Shrine Training Ring",
                    "A moss-ringed practice ground where pilgrims drilled shield lines beneath teal lamps.",
                    westSiteX,
                    westSiteY,
                    3,
                    ObjectType.TrainingGround),
                new WorldMapSite(
                    "old-quarry-forge",
                    "old-quarry",
                    "Old Quarry Forge",
                    "A roofless stone forge waits beside cut blocks, bridge irons, and cold anvils.",
                    cornerLeftSiteX,
                    topSiteY,
                    2,
                    ObjectType.ForgeSite),
                new WorldMapSite(
                    "gloam-deep-crypt",
                    "gloam-courts",
                    "Gloam Deep Crypt",
                    "Sunken court stairs descend toward sealed reliquaries and a bone-priest road.",
                    centerSiteX,
                    topSiteY,
                    2,
                    ObjectType.DeepCrypt),
                new WorldMapSite(
                    "glass-lore-library",
                    "glass-warrens",
                    "Glass Lore Library",
                    "Cracked mirror stacks preserve formulae, ward diagrams, and dangerous reflected paths.",
                    cornerRightSiteX,
                    topSiteY,
                    2,
                    ObjectType.LoreLibrary),
                new WorldMapSite(
                    "dusk-market-hideout",
                    "dusk-market",
                    "Dusk Market Hideout",
                    "Canvas roofs and collapsed stalls conceal a scout camp, fence tables, and smoke routes.",
                    eastSiteX,
                    eastSiteY,
                    3,
                    ObjectType.FactionCamp),
                new WorldMapSite(
                    "red-gate-seal",
                    "red-gate",
                    "Red Gate Seal",
                    "A basalt lock of fused sigils bars the war road beyond the broken banners.",
                    cornerRightSiteX,
                    bottomSmallSiteY,
                    2,
                    ObjectType.PortalSeal),
                new WorldMapSite(
                    "salt-cistern-gate",
                    "salt-cisterns",
                    "Salt Cistern Gate",
                    "Flood-scored doors guard chained sluices and a crownward stair cut beneath the old road.",
                    centerSiteX,
                    bottomSmallSiteY,
                    2,
                    ObjectType.DungeonGate),
                new WorldMapSite(
                    "ash-fen-ancient-grove",
                    "ash-fen",
                    "Ash Fen Ancient Grove",
                    "Black-rooted trees rise from poison mire around a dry shrine island.",
                    ClampSiteCenter(cornerLeftSiteX, width, 3),
                    bottomLargeSiteY,
                    3,
                    ObjectType.AncientGrove)
            };
        }

        public static bool TryFindRegionalJunction(
            int width,
            int height,
            int startX,
            int startY,
            int x,
            int y,
            int radius,
            out WorldMapJunction junction)
        {
            int safeRadius = Math.Max(0, radius);
            foreach (WorldMapJunction candidate in RegionalJunctions(width, height, startX, startY))
            {
                if (Math.Abs(candidate.X - x) + Math.Abs(candidate.Y - y) > safeRadius) continue;
                junction = candidate;
                return true;
            }

            junction = default;
            return false;
        }

        private static int ClampSiteCenter(int value, int size, int radius)
        {
            int inset = radius + 2;
            return Clamp(value, inset, size - inset - 1);
        }

        private static int Clamp(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        private readonly struct RegionalCacheKey : IEquatable<RegionalCacheKey>
        {
            private readonly int width;
            private readonly int height;
            private readonly int startX;
            private readonly int startY;

            public RegionalCacheKey(int width, int height, int startX, int startY)
            {
                this.width = width;
                this.height = height;
                this.startX = startX;
                this.startY = startY;
            }

            public bool Equals(RegionalCacheKey other)
            {
                return width == other.width
                    && height == other.height
                    && startX == other.startX
                    && startY == other.startY;
            }

            public override bool Equals(object obj)
            {
                return obj is RegionalCacheKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 31 + width;
                    hash = hash * 31 + height;
                    hash = hash * 31 + startX;
                    return hash * 31 + startY;
                }
            }
        }
    }
}
