using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private const string KingHalvardId = "midgaard-king-halvard";
        private const string RoyalHeraldInteriorId = "midgaard-herald-vann";
        private const string ArmorerNpcId = "midgaard-armorer-borin";
        private const string WeaponMerchantNpcId = "midgaard-weapon-merchant-tessa";
        private const string EnchanterNpcId = "midgaard-enchanter-maud";

        private RectInt ThroneRoomBounds(MapData map)
        {
            return MidgaardInteriorRules.ThroneRoomBounds(map);
        }

        private RectInt MerchantHallBounds(MapData map)
        {
            return MidgaardInteriorRules.MerchantHallBounds(map);
        }

        private bool IsMidgaardInteriorCell(int x, int y, MapData map, int depth)
        {
            return map != null && depth == 1 && MidgaardInteriorRules.IsInteriorCell(map, x, y);
        }

        private bool IsMidgaardInteriorDecoration(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.RoyalThrone:
                case ObjectType.RoyalBanner:
                case ObjectType.RoyalLectern:
                case ObjectType.RoyalBrazier:
                case ObjectType.ArmorDisplay:
                case ObjectType.WeaponDisplay:
                case ObjectType.EnchantmentTable:
                case ObjectType.ProvisionShelf:
                case ObjectType.MerchantCounter:
                    return true;
                default:
                    return false;
            }
        }

        private string MidgaardInteriorIdAt(int x, int y, MapData map, int depth)
        {
            if (map == null || depth != 1) return "";
            Vector2Int point = new Vector2Int(x, y);
            if (ThroneRoomBounds(map).Contains(point)) return "midgaard-throne-room";
            if (MerchantHallBounds(map).Contains(point)) return "midgaard-merchant-hall";
            return "";
        }

        private int MidgaardInteriorTileAtlasIndex(int x, int y, int tile)
        {
            MapData map = state?.Map;
            if (map == null || state.Depth != 1) return -1;

            string interiorId = MidgaardInteriorIdAt(x, y, map, state.Depth);
            if (string.IsNullOrEmpty(interiorId)) return -1;

            ExplorationCellRole roles = ExplorationSurfaceRules.RolesAt(map, x, y);
            if ((roles & ExplorationCellRole.Threshold) != 0)
            {
                return interiorId == "midgaard-throne-room" ? 15 : 16;
            }

            RectInt room = interiorId == "midgaard-throne-room"
                ? ThroneRoomBounds(map)
                : MerchantHallBounds(map);
            bool left = x == room.xMin;
            bool right = x == room.xMax - 1;
            bool top = y == room.yMin;
            bool bottom = y == room.yMax - 1;
            if (left || right || top || bottom)
            {
                int wallStart = interiorId == "midgaard-throne-room" ? 5 : 10;
                if ((left || right) && (top || bottom)) return wallStart + 4;
                if (top) return wallStart;
                if (right) return wallStart + 1;
                if (bottom) return wallStart + 2;
                return wallStart + 3;
            }

            if (tile == 0) return interiorId == "midgaard-throne-room" ? 9 : 14;

            MapObject obj = ObjectAt(map, x, y);
            if (interiorId == "midgaard-throne-room")
            {
                if (obj?.Type == ObjectType.RoyalThrone) return 2;
                int centerX = room.xMin + room.width / 2;
                return x == centerX ? 1 : 0;
            }

            int armorerX = room.xMin + 2;
            int enchanterX = room.xMin + 8;
            if (x == armorerX && y == room.yMin + 3) return 17;
            if (x == enchanterX && y == room.yMin + 3) return 18;
            if (y == room.yMin + 1 || ((x + y) & 7) == 0) return 4;
            return 3;
        }

        private void EnsureMidgaardInteriors(MapData map)
        {
            if (map == null || map.Depth != 1) return;
            if (map.Objects == null) map.Objects = new List<MapObject>();

            RectInt throne = ThroneRoomBounds(map);
            RectInt merchants = MerchantHallBounds(map);
            ClearInteriorReservation(map, throne);
            ClearInteriorReservation(map, merchants);
            CarveInteriorRoom(map, throne, ExplorationMaterial.KeepStone);
            CarveInteriorRoom(map, merchants, ExplorationMaterial.MarketCobbles);

            int throneDoorX = throne.xMin + throne.width / 2;
            int throneDoorY = throne.yMax - 1;
            SetExploreCell(
                map,
                throneDoorX,
                throneDoorY,
                1,
                ExplorationMaterial.KeepStone,
                ExplorationCellRole.City | ExplorationCellRole.Room | ExplorationCellRole.Threshold);

            int merchantDoorY = merchants.yMax - 1;
            int armorerDoorX = merchants.xMin + 2;
            int weaponDoorX = merchants.xMin + 5;
            int enchanterDoorX = merchants.xMin + 8;
            foreach (int doorX in new[] { armorerDoorX, weaponDoorX, enchanterDoorX })
            {
                SetExploreCell(
                    map,
                    doorX,
                    merchantDoorY,
                    1,
                    ExplorationMaterial.MarketCobbles,
                    ExplorationCellRole.City | ExplorationCellRole.Room | ExplorationCellRole.Threshold);
            }

            int sx = map.StartX;
            int sy = map.StartY;
            MapObject kingHall = ObjectAt(map, sx, MidgaardTop(map) + 1);
            ConfigurePortal(
                kingHall,
                MidgaardInteriorRules.KingHallDoorId,
                MidgaardInteriorRules.ThroneRoomExitId);
            ConfigurePortal(
                ObjectAt(map, sx - 4, sy),
                MidgaardInteriorRules.ArmorerDoorId,
                MidgaardInteriorRules.ArmorerExitId);
            ConfigurePortal(
                ObjectAt(map, sx + 4, sy),
                MidgaardInteriorRules.WeaponDoorId,
                MidgaardInteriorRules.WeaponExitId);
            ConfigurePortal(
                ObjectAt(map, sx + 4, sy + 1),
                MidgaardInteriorRules.EnchanterDoorId,
                MidgaardInteriorRules.EnchanterExitId);

            map.Objects.RemoveAll(obj => obj != null && obj.Type == ObjectType.RoyalHerald);

            UpsertNamedMapObject(
                map,
                throneDoorX,
                throneDoorY,
                ObjectType.InteriorDoor,
                MidgaardInteriorRules.ThroneRoomExitId,
                MidgaardInteriorRules.KingHallDoorId);
            UpsertNamedMapObject(map, throneDoorX, throne.yMin + 1, ObjectType.RoyalThrone, "midgaard-royal-throne");
            UpsertNamedMapObject(map, throneDoorX, throne.yMin + 2, ObjectType.KingHalvard, KingHalvardId);
            UpsertNamedMapObject(map, throne.xMin + 2, throne.yMin + 3, ObjectType.RoyalHerald, RoyalHeraldInteriorId);
            UpsertNamedMapObject(map, throne.xMax - 3, throne.yMin + 3, ObjectType.RoyalLectern, "midgaard-royal-lectern");
            UpsertNamedMapObject(map, throne.xMin + 1, throne.yMin + 1, ObjectType.RoyalBanner, "midgaard-royal-banner-west");
            UpsertNamedMapObject(map, throne.xMax - 2, throne.yMin + 1, ObjectType.RoyalBanner, "midgaard-royal-banner-east");
            UpsertNamedMapObject(map, throne.xMin + 2, throne.yMin + 1, ObjectType.RoyalBrazier, "midgaard-royal-brazier-west");
            UpsertNamedMapObject(map, throne.xMax - 3, throne.yMin + 1, ObjectType.RoyalBrazier, "midgaard-royal-brazier-east");

            UpsertNamedMapObject(
                map,
                armorerDoorX,
                merchantDoorY,
                ObjectType.InteriorDoor,
                MidgaardInteriorRules.ArmorerExitId,
                MidgaardInteriorRules.ArmorerDoorId);
            UpsertNamedMapObject(
                map,
                weaponDoorX,
                merchantDoorY,
                ObjectType.InteriorDoor,
                MidgaardInteriorRules.WeaponExitId,
                MidgaardInteriorRules.WeaponDoorId);
            UpsertNamedMapObject(
                map,
                enchanterDoorX,
                merchantDoorY,
                ObjectType.InteriorDoor,
                MidgaardInteriorRules.EnchanterExitId,
                MidgaardInteriorRules.EnchanterDoorId);
            UpsertNamedMapObject(map, armorerDoorX, merchants.yMin + 2, ObjectType.ArmorerNpc, ArmorerNpcId);
            UpsertNamedMapObject(map, weaponDoorX, merchants.yMin + 2, ObjectType.WeaponMerchantNpc, WeaponMerchantNpcId);
            UpsertNamedMapObject(map, enchanterDoorX, merchants.yMin + 2, ObjectType.EnchanterNpc, EnchanterNpcId);
            UpsertNamedMapObject(map, merchants.xMin + 1, merchants.yMin + 1, ObjectType.ArmorDisplay, "midgaard-armor-display");
            UpsertNamedMapObject(map, merchants.xMin + 4, merchants.yMin + 1, ObjectType.WeaponDisplay, "midgaard-weapon-display");
            UpsertNamedMapObject(map, merchants.xMin + 7, merchants.yMin + 1, ObjectType.EnchantmentTable, "midgaard-enchantment-table");
            UpsertNamedMapObject(map, merchants.xMax - 2, merchants.yMax - 3, ObjectType.ProvisionShelf, "midgaard-provision-shelf");

            string[] broken = MidgaardInteriorRules.BrokenPortalIds(map).ToArray();
            if (broken.Length > 0)
            {
                Debug.LogWarning(VersionInfo.ProductName + " repaired Midgaard interiors with broken portal targets: " + string.Join(", ", broken));
            }
        }

        private void ClearInteriorReservation(MapData map, RectInt room)
        {
            int minX = Mathf.Max(0, room.xMin - 1);
            int maxX = Mathf.Min(map.Width - 1, room.xMax);
            int minY = Mathf.Max(0, room.yMin - 1);
            int maxY = Mathf.Min(map.Height - 1, room.yMax);
            map.Objects.RemoveAll(obj =>
                obj != null
                && obj.X >= minX
                && obj.X <= maxX
                && obj.Y >= minY
                && obj.Y <= maxY);
            map.InvalidateObjectLookup();
            for (int y = minY; y <= maxY; y++)
            for (int x = minX; x <= maxX; x++)
            {
                SetExploreCell(map, x, y, 0, ExplorationMaterial.CityWall, ExplorationCellRole.City | ExplorationCellRole.Room);
            }
        }

        private void CarveInteriorRoom(MapData map, RectInt room, ExplorationMaterial floor)
        {
            for (int y = room.yMin; y < room.yMax; y++)
            for (int x = room.xMin; x < room.xMax; x++)
            {
                bool border = x == room.xMin || x == room.xMax - 1 || y == room.yMin || y == room.yMax - 1;
                SetExploreCell(
                    map,
                    x,
                    y,
                    border ? 0 : 1,
                    border ? ExplorationMaterial.CityWall : floor,
                    ExplorationCellRole.City | ExplorationCellRole.Room);
            }
        }

        private void ConfigurePortal(MapObject obj, string id, string targetId)
        {
            if (obj == null) return;
            obj.Id = id;
            obj.TargetId = targetId;
        }

        private void UpsertNamedMapObject(
            MapData map,
            int x,
            int y,
            ObjectType type,
            string id,
            string targetId = "")
        {
            if (map == null || x < 0 || y < 0 || x >= map.Width || y >= map.Height) return;
            map.Objects.RemoveAll(obj =>
                obj != null
                && (obj.X == x && obj.Y == y
                    || !string.IsNullOrWhiteSpace(id) && string.Equals(obj.Id, id, StringComparison.Ordinal)));
            MapObject placed = new MapObject(x, y, type, id, targetId);
            map.Objects.Add(placed);
            map.InvalidateObjectLookup();
            ApplyMapObjectSurface(map, placed);
        }

        private bool TryUseMidgaardPortal(MapObject portal)
        {
            if (state?.Map == null || !MidgaardInteriorRules.IsPortal(portal)) return false;
            MapObject destination = MidgaardInteriorRules.FindById(state.Map, portal.TargetId);
            if (destination == null || !MidgaardInteriorRules.TryFindArrival(state.Map, destination, out Point arrival))
            {
                PushLog("The doorway does not yet connect to a safe landing.", Tone.Warn);
                ShowBanner("Doorway blocked");
                PlaySfx("blocked", 0.62f);
                return true;
            }

            int oldX = state.PlayerX;
            int oldY = state.PlayerY;
            state.PlayerX = arrival.X;
            state.PlayerY = arrival.Y;
            lastExploreRegion = ExploreRegionName(state.PlayerX, state.PlayerY);
            if (!state.ReducedMotion)
            {
                tweens.Add(new Tween("party", new Vector2(oldX, oldY), new Vector2(arrival.X, arrival.Y), Time.time, 0.18f, TweenKind.Move));
            }

            string zoneId = MidgaardInteriorIdAt(arrival.X, arrival.Y, state.Map, state.Depth);
            bool enteringThrone = zoneId == "midgaard-throne-room";
            bool enteringMerchants = zoneId == "midgaard-merchant-hall";
            if (enteringThrone)
            {
                SetStoryFlag(StoryFlags.MidgaardThroneRoomEntered);
                PushLog("The doors open onto a bannered stone hall. King Halvard waits beyond the herald's lectern.", Tone.Good);
                ShowBanner("Throne Room");
                PlaySfx("doorroyal", 0.76f);
                QueueSfx("thronechime", 0.11f, 0.46f);
            }
            else if (enteringMerchants)
            {
                SetStoryFlag(StoryFlags.MidgaardMerchantHallEntered);
                PushLog("Warm forge light, weapon racks, and rune-glow divide Midgaard's merchant hall into three working bays.", Tone.Normal);
                ShowBanner("Merchant Hall");
                PlaySfx("doorwood", 0.70f);
                QueueSfx("shopbell", 0.09f, 0.42f);
            }
            else
            {
                PushLog("The party steps back into Midgaard's streets.", Tone.Normal);
                ShowBanner(HomeTownName);
                PlaySfx("doorwood", 0.66f);
            }
            return true;
        }

        private string InteriorObjectName(MapObject obj)
        {
            if (obj == null) return "";
            switch (obj.Type)
            {
                case ObjectType.InteriorDoor:
                    return obj.Id == MidgaardInteriorRules.ThroneRoomExitId ? "Doors to Midgaard" : "Merchant Hall exit";
                case ObjectType.KingHalvard: return "King Halvard";
                case ObjectType.ArmorerNpc: return "Armorer Borin";
                case ObjectType.WeaponMerchantNpc: return "Weaponsmith Tessa";
                case ObjectType.EnchanterNpc: return "Runesmith Maud";
                case ObjectType.RoyalThrone: return "Throne of Midgaard";
                case ObjectType.RoyalBanner: return "Midgaard banner";
                case ObjectType.RoyalLectern: return "Royal lectern";
                case ObjectType.RoyalBrazier: return "Hall brazier";
                case ObjectType.ArmorDisplay: return "Armor display";
                case ObjectType.WeaponDisplay: return "Weapon rack";
                case ObjectType.EnchantmentTable: return "Rune table";
                case ObjectType.ProvisionShelf: return "Provision shelves";
                case ObjectType.MerchantCounter: return "Merchant counter";
            }

            if (obj.Id == MidgaardInteriorRules.KingHallDoorId) return "King's Hall doors";
            if (obj.Id == MidgaardInteriorRules.ArmorerDoorId) return "Borin's Armory";
            if (obj.Id == MidgaardInteriorRules.WeaponDoorId) return "Tessa's Weapons";
            if (obj.Id == MidgaardInteriorRules.EnchanterDoorId) return "Maud's Runes";
            return "";
        }

        private string InteriorObjectHint(MapObject obj)
        {
            if (obj == null) return "";
            switch (obj.Type)
            {
                case ObjectType.InteriorDoor: return "leave this interior";
                case ObjectType.KingHalvard: return "receive or review the royal writ";
                case ObjectType.ArmorerNpc: return "armor service and rat-pelt reward";
                case ObjectType.WeaponMerchantNpc: return "early weapons for sale";
                case ObjectType.EnchanterNpc: return "paid weapon enchantment";
                case ObjectType.RoyalThrone: return "the city's old seat";
                case ObjectType.RoyalBanner: return "blue and gold of Midgaard";
                case ObjectType.RoyalLectern: return "sealed writs and road charters";
                case ObjectType.RoyalBrazier: return "warm hall fire";
                case ObjectType.ArmorDisplay: return "samples of Borin's work";
                case ObjectType.WeaponDisplay: return "balanced steel and ashwood";
                case ObjectType.EnchantmentTable: return "quiet rune-light";
                case ObjectType.ProvisionShelf: return "merchant stock";
                case ObjectType.MerchantCounter: return "trade counter";
            }

            if (obj.Id == MidgaardInteriorRules.KingHallDoorId) return "enter the throne room";
            if (obj.Id == MidgaardInteriorRules.ArmorerDoorId
                || obj.Id == MidgaardInteriorRules.WeaponDoorId
                || obj.Id == MidgaardInteriorRules.EnchanterDoorId)
            {
                return "enter the merchant hall";
            }
            return "";
        }
    }
}
