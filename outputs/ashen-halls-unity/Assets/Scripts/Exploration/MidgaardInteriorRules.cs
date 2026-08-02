using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AshenHalls
{
    public static class MidgaardInteriorRules
    {
        public const string KingHallDoorId = "midgaard-king-hall-door";
        public const string ThroneRoomExitId = "midgaard-throne-room-exit";
        public const string GrandHearthDoorId = "midgaard-grand-hearth-door";
        public const string GrandHearthExitId = "midgaard-grand-hearth-exit";
        public const string GrandHearthFireId = "midgaard-grand-hearth-fire";
        public const string GrandHearthCargoId = "midgaard-grand-hearth-cargo";
        public const string GrandHearthWindowId = "midgaard-grand-hearth-window";
        public const string GrandHearthMapTableId = "midgaard-grand-hearth-map-table";
        public const string GrandHearthRoadChestId = "midgaard-grand-hearth-road-chest";
        public const string ArmorerDoorId = "midgaard-armorer-door";
        public const string ArmorerExitId = "midgaard-armorer-exit";
        public const string WeaponDoorId = "midgaard-weapons-door";
        public const string WeaponExitId = "midgaard-weapons-exit";
        public const string EnchanterDoorId = "midgaard-enchanter-door";
        public const string EnchanterExitId = "midgaard-enchanter-exit";

        private static readonly int[] ArrivalX = { 0, 0, -1, 1 };
        private static readonly int[] ArrivalY = { -1, 1, 0, 0 };

        public static RectInt ThroneRoomBounds(MapData map)
        {
            return new RectInt(2, 1, 10, 7);
        }

        public static RectInt MerchantHallBounds(MapData map)
        {
            int left = Mathf.Max(2, (map?.Width ?? WorldMapGenerationRules.Width) - 12);
            return new RectInt(left, 1, 10, 8);
        }

        public static RectInt GrandHearthBounds(MapData map)
        {
            int height = map?.Height ?? WorldMapGenerationRules.Height;
            return new RectInt(2, Mathf.Max(1, height - 10), 9, 8);
        }

        public static Point GrandHearthSpawn(MapData map)
        {
            RectInt room = GrandHearthBounds(map);
            return new Point(room.xMin + 4, room.yMin + room.height / 2);
        }

        public static Point GrandHearthExit(MapData map)
        {
            RectInt room = GrandHearthBounds(map);
            return new Point(room.xMax - 1, room.yMin + room.height / 2);
        }

        public static bool IsInteriorCell(MapData map, int x, int y)
        {
            if (map == null || map.Depth != 1) return false;
            Vector2Int point = new Vector2Int(x, y);
            return ThroneRoomBounds(map).Contains(point)
                || MerchantHallBounds(map).Contains(point)
                || GrandHearthBounds(map).Contains(point);
        }

        public static bool IsReservedCell(MapData map, int x, int y)
        {
            if (map == null || map.Depth != 1) return false;
            return IsInsideReservation(ThroneRoomBounds(map), x, y)
                || IsInsideReservation(MerchantHallBounds(map), x, y)
                || IsInsideReservation(GrandHearthBounds(map), x, y);
        }

        private static bool IsInsideReservation(RectInt room, int x, int y)
        {
            return x >= room.xMin - 1
                && x <= room.xMax
                && y >= room.yMin - 1
                && y <= room.yMax;
        }

        public static bool IsPortal(MapObject obj)
        {
            return obj != null
                && !string.IsNullOrWhiteSpace(obj.Id)
                && !string.IsNullOrWhiteSpace(obj.TargetId);
        }

        public static MapObject FindById(MapData map, string id)
        {
            return map?.FindObjectById(id);
        }

        public static bool HasValidTarget(MapData map, MapObject portal)
        {
            return IsPortal(portal) && FindById(map, portal.TargetId) != null;
        }

        public static bool TryFindArrival(MapData map, MapObject destination, out Point arrival)
        {
            arrival = null;
            if (map == null || destination == null) return false;
            for (int i = 0; i < ArrivalX.Length; i++)
            {
                int x = destination.X + ArrivalX[i];
                int y = destination.Y + ArrivalY[i];
                if (!ExplorationTraversalRules.IsStandable(map, x, y)) continue;
                arrival = new Point(x, y);
                return true;
            }
            return false;
        }

        public static IReadOnlyList<string> BrokenPortalIds(MapData map)
        {
            if (map?.Objects == null) return Array.Empty<string>();
            return map.Objects
                .Where(IsPortal)
                .Where(portal => !HasValidTarget(map, portal))
                .Select(portal => portal.Id)
                .OrderBy(id => id, StringComparer.Ordinal)
                .ToArray();
        }
    }
}
