using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public static class ExplorationSurfaceRules
    {
        private const int ValidRoleMask = (int)(
            ExplorationCellRole.Trail
            | ExplorationCellRole.Road
            | ExplorationCellRole.Room
            | ExplorationCellRole.Plaza
            | ExplorationCellRole.Threshold
            | ExplorationCellRole.Water
            | ExplorationCellRole.Bridge
            | ExplorationCellRole.Hazard
            | ExplorationCellRole.City
            | ExplorationCellRole.Clearing);

        public static bool HasCompleteGrid(MapData map)
        {
            int count = CellCount(map);
            return count > 0
                && map.SurfaceMaterials != null
                && map.SurfaceRoles != null
                && map.SurfaceMaterials.Count == count
                && map.SurfaceRoles.Count == count;
        }

        public static bool HasValidGrid(MapData map)
        {
            int count = CellCount(map);
            if (count <= 0 || map?.Tiles == null || map.Tiles.Count != count || !HasCompleteGrid(map)) return false;
            for (int i = 0; i < count; i++)
            {
                if (map.Tiles[i] != 0 && map.Tiles[i] != 1) return false;
                if (!Enum.IsDefined(typeof(ExplorationMaterial), map.SurfaceMaterials[i])) return false;
                if ((map.SurfaceRoles[i] & ~ValidRoleMask) != 0) return false;
            }
            return true;
        }

        public static bool IsLoadableMap(MapData map, bool requireSurfaceGrid)
        {
            int count = CellCount(map);
            if (count <= 0 || map.Tiles == null || map.Tiles.Count != count) return false;
            if (map.StartX < 0 || map.StartY < 0 || map.StartX >= map.Width || map.StartY >= map.Height) return false;
            for (int i = 0; i < count; i++)
            {
                if (map.Tiles[i] != 0 && map.Tiles[i] != 1) return false;
            }
            if (map.Objects != null)
            {
                foreach (MapObject obj in map.Objects)
                {
                    if (obj == null || !InBounds(map, obj.X, obj.Y)) return false;
                }
            }
            return !requireSurfaceGrid || HasValidGrid(map);
        }

        public static bool EnsureGrid(MapData map)
        {
            int count = CellCount(map);
            if (count <= 0) return false;

            bool rebuildMaterials = map.SurfaceMaterials == null || map.SurfaceMaterials.Count != count;
            bool rebuildRoles = map.SurfaceRoles == null || map.SurfaceRoles.Count != count;
            if (!rebuildMaterials && !rebuildRoles) return false;

            IReadOnlyList<int> oldMaterials = map.SurfaceMaterials;
            IReadOnlyList<int> oldRoles = map.SurfaceRoles;
            map.SurfaceMaterials = new List<int>(count);
            map.SurfaceRoles = new List<int>(count);
            for (int i = 0; i < count; i++)
            {
                map.SurfaceMaterials.Add((int)ExistingMaterial(oldMaterials, i));
                map.SurfaceRoles.Add((int)ExistingRoles(oldRoles, i));
            }
            RepairConsistency(map);
            return true;
        }

        public static void RepairConsistency(MapData map)
        {
            int count = CellCount(map);
            if (count <= 0 || !HasCompleteGrid(map)) return;
            for (int i = 0; i < count; i++)
            {
                map.SurfaceMaterials[i] = (int)ValidMaterial(map.SurfaceMaterials[i]);
                map.SurfaceRoles[i] = (int)ValidRoles(map.SurfaceRoles[i]);
            }
        }

        public static ExplorationMaterial MaterialAt(MapData map, int x, int y)
        {
            if (!InBounds(map, x, y) || !HasCompleteGrid(map)) return ExplorationMaterial.NaturalGround;
            return ValidMaterial(map.SurfaceMaterials[y * map.Width + x]);
        }

        public static ExplorationCellRole RolesAt(MapData map, int x, int y)
        {
            if (!InBounds(map, x, y) || !HasCompleteGrid(map)) return ExplorationCellRole.None;
            return ValidRoles(map.SurfaceRoles[y * map.Width + x]);
        }

        public static void SetMaterial(MapData map, int x, int y, ExplorationMaterial material)
        {
            if (!InBounds(map, x, y)) return;
            EnsureGrid(map);
            map.SurfaceMaterials[y * map.Width + x] = (int)ValidMaterial((int)material);
        }

        public static void SetRoles(MapData map, int x, int y, ExplorationCellRole roles)
        {
            if (!InBounds(map, x, y)) return;
            EnsureGrid(map);
            map.SurfaceRoles[y * map.Width + x] = (int)ValidRoles((int)roles);
        }

        public static void AddRoles(MapData map, int x, int y, ExplorationCellRole roles)
        {
            SetRoles(map, x, y, RolesAt(map, x, y) | roles);
        }

        public static bool IsPath(ExplorationCellRole roles)
        {
            return (roles & (ExplorationCellRole.Trail | ExplorationCellRole.Road | ExplorationCellRole.Bridge)) != 0;
        }

        public static int PathNeighborMask(MapData map, int x, int y)
        {
            int mask = 0;
            if (IsPassablePath(map, x, y - 1)) mask |= 1;
            if (IsPassablePath(map, x + 1, y)) mask |= 2;
            if (IsPassablePath(map, x, y + 1)) mask |= 4;
            if (IsPassablePath(map, x - 1, y)) mask |= 8;
            return mask;
        }

        private static bool IsPassablePath(MapData map, int x, int y)
        {
            if (!InBounds(map, x, y) || map.Tiles == null) return false;
            int index = y * map.Width + x;
            return index >= 0
                && index < map.Tiles.Count
                && map.Tiles[index] == 1
                && IsPath(RolesAt(map, x, y));
        }

        public static float SmoothBoundaryOffset(int coordinate, int salt, float amplitude)
        {
            int segment = FloorDiv(coordinate, 4);
            float t = PositiveMod(coordinate, 4) / 4f;
            t = t * t * (3f - 2f * t);
            float a = HashUnit(segment, salt) * 2f - 1f;
            float b = HashUnit(segment + 1, salt) * 2f - 1f;
            return (a + (b - a) * t) * Math.Max(0f, amplitude);
        }

        private static ExplorationMaterial ExistingMaterial(IReadOnlyList<int> materials, int index)
        {
            if (materials == null || index < 0 || index >= materials.Count) return ExplorationMaterial.NaturalGround;
            return ValidMaterial(materials[index]);
        }

        private static ExplorationCellRole ExistingRoles(IReadOnlyList<int> roles, int index)
        {
            if (roles == null || index < 0 || index >= roles.Count) return ExplorationCellRole.None;
            return ValidRoles(roles[index]);
        }

        private static ExplorationMaterial ValidMaterial(int raw)
        {
            return Enum.IsDefined(typeof(ExplorationMaterial), raw)
                ? (ExplorationMaterial)raw
                : ExplorationMaterial.NaturalGround;
        }

        private static ExplorationCellRole ValidRoles(int raw)
        {
            return (ExplorationCellRole)(raw & ValidRoleMask);
        }

        private static int CellCount(MapData map)
        {
            if (map == null || map.Width <= 0 || map.Height <= 0) return 0;
            long count = (long)map.Width * map.Height;
            return count > int.MaxValue ? 0 : (int)count;
        }

        private static bool InBounds(MapData map, int x, int y)
        {
            return map != null && x >= 0 && y >= 0 && x < map.Width && y < map.Height;
        }

        private static float HashUnit(int coordinate, int salt)
        {
            unchecked
            {
                int value = coordinate * 92821 + salt * 68917;
                value ^= value << 13;
                value ^= value >> 17;
                value ^= value << 5;
                return (value & 0x7fffffff) / (float)int.MaxValue;
            }
        }

        private static int FloorDiv(int value, int divisor)
        {
            int quotient = value / divisor;
            int remainder = value % divisor;
            return remainder < 0 ? quotient - 1 : quotient;
        }

        private static int PositiveMod(int value, int divisor)
        {
            int result = value % divisor;
            return result < 0 ? result + divisor : result;
        }
    }
}
