using System;
using UnityEngine;

namespace AshenHalls
{
    public readonly struct GrandHearthFloorChoice
    {
        public int AtlasIndex { get; }
        public bool FlipX { get; }
        public bool FlipY { get; }

        public GrandHearthFloorChoice(int atlasIndex, bool flipX, bool flipY)
        {
            AtlasIndex = atlasIndex;
            FlipX = flipX;
            FlipY = flipY;
        }
    }

    public static class GrandHearthArtCatalog
    {
        public const int FloorAtlasColumns = 3;
        public const int FloorAtlasRows = 2;
        public const int FloorAtlasCellCount = FloorAtlasColumns * FloorAtlasRows;

        public const int SetpieceAtlasColumns = 3;
        public const int SetpieceAtlasRows = 2;
        public const int SetpieceAtlasCellCount = SetpieceAtlasColumns * SetpieceAtlasRows;

        public static bool TryFloorChoice(
            MapData map,
            int x,
            int y,
            int tile,
            out GrandHearthFloorChoice choice)
        {
            choice = default;
            if (map == null || map.Depth != 1 || tile != 1) return false;

            RectInt room = MidgaardInteriorRules.GrandHearthBounds(map);
            if (!room.Contains(new Vector2Int(x, y))) return false;

            Point exit = MidgaardInteriorRules.GrandHearthExit(map);
            if (x == exit.X && y == exit.Y)
            {
                choice = new GrandHearthFloorChoice(5, false, false);
                return true;
            }

            if (MidgaardInteriorRules.IsGrandHearthCompanyRunner(map, x, y))
            {
                Point spawn = MidgaardInteriorRules.GrandHearthSpawn(map);
                int runnerIndex = x == spawn.X && y == spawn.Y ? 3 : 2;
                choice = new GrandHearthFloorChoice(runnerIndex, false, false);
                return true;
            }

            int fireX = room.xMin + 2;
            int fireY = room.yMin + 2;
            if (x == fireX && (y == fireY || y == fireY + 1))
            {
                choice = new GrandHearthFloorChoice(4, false, false);
                return true;
            }

            int localX = x - room.xMin;
            int localY = y - room.yMin;
            int pattern = unchecked(localX * 73856093 ^ localY * 19349663);
            choice = new GrandHearthFloorChoice(
                pattern & 1,
                (pattern & 2) != 0,
                (pattern & 4) != 0);
            return true;
        }

        public static int SetpieceIndex(string id)
        {
            if (string.Equals(id, MidgaardInteriorRules.GrandHearthFireId, StringComparison.Ordinal)) return 0;
            if (string.Equals(id, MidgaardInteriorRules.GrandHearthExitId, StringComparison.Ordinal)) return 1;
            if (string.Equals(id, MidgaardInteriorRules.GrandHearthRegisterId, StringComparison.Ordinal)) return 2;
            if (string.Equals(id, MidgaardInteriorRules.GrandHearthBannerId, StringComparison.Ordinal)) return 3;
            if (string.Equals(id, MidgaardInteriorRules.GrandHearthWindowId, StringComparison.Ordinal)) return 4;
            if (string.Equals(id, MidgaardInteriorRules.GrandHearthCargoId, StringComparison.Ordinal)
                || string.Equals(id, MidgaardInteriorRules.GrandHearthShelvesId, StringComparison.Ordinal))
            {
                return 5;
            }
            return -1;
        }
    }
}
