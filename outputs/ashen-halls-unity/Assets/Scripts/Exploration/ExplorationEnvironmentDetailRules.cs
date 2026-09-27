using UnityEngine;

namespace AshenHalls
{
    // Cosmetic only. This never supplies collision, discovery, route, or save data.
    public static class ExplorationEnvironmentDetailRules
    {
        public const int Columns = 4;
        public const int Rows = 2;
        // Preserve the generator's original alpha pixels. Cells are 443.5px;
        // runtime UV sampling is fractional and does not rescale the source.
        public const int Width = 1774;
        public const int Height = 887;
        public const int CellCount = Columns * Rows;

        public static int CellFor(ExplorationMaterial material, int variant)
        {
            int pick = (int)((uint)variant % 3u);
            switch (material)
            {
                case ExplorationMaterial.NaturalGround: return pick == 0 ? 0 : pick == 1 ? 1 : 7;
                case ExplorationMaterial.Forest: return pick == 0 ? 1 : pick == 1 ? 5 : 7;
                case ExplorationMaterial.Moss: return pick == 0 ? 0 : pick == 1 ? 5 : 1;
                case ExplorationMaterial.FenMud: return 4;
                case ExplorationMaterial.QuarryStone:
                case ExplorationMaterial.RuinedPaving:
                case ExplorationMaterial.GloamStone: return 2;
                case ExplorationMaterial.PackedDirt:
                case ExplorationMaterial.RedAsh: return 3;
                case ExplorationMaterial.CityPaving:
                case ExplorationMaterial.MarketCobbles:
                case ExplorationMaterial.TempleStone:
                case ExplorationMaterial.KeepStone: return 6;
                default: return -1;
            }
        }

        public static bool Eligible(bool charted, bool interior, int tile, ExplorationCellRole roles,
            bool occupied, bool protectedFocus, bool guidance, ExplorationMaterial material)
        {
            return charted && !interior && tile == 1 && !occupied && !protectedFocus && !guidance
                && !ExplorationSurfaceRules.IsPath(roles)
                && (roles & (ExplorationCellRole.Plaza | ExplorationCellRole.Threshold)) == 0
                && CellFor(material, 0) >= 0;
        }

        public static bool Selected(int x, int y, int seed, int depth, ExplorationMaterial material)
        {
            uint value = Hash(x, y, seed, depth);
            uint density = material == ExplorationMaterial.CityPaving || material == ExplorationMaterial.MarketCobbles
                || material == ExplorationMaterial.TempleStone || material == ExplorationMaterial.KeepStone ? 18u : 30u;
            if (value % 100u >= density) return false;
            // A local minimum leaves a full-cell breathing space between details.
            // Selection is identical after panning, changing zoom, and reloading.
            return value < Hash(x - 1, y, seed, depth) && value < Hash(x + 1, y, seed, depth)
                && value < Hash(x, y - 1, seed, depth) && value < Hash(x, y + 1, seed, depth);
        }

        public static uint Hash(int x, int y, int seed, int depth)
        {
            unchecked
            {
                uint value = (uint)x * 0x9e3779b9u ^ (uint)y * 0x85ebca6bu
                    ^ (uint)seed * 0xc2b2ae35u ^ (uint)depth * 0x27d4eb2fu;
                value ^= value >> 16;
                value *= 0x7feb352du;
                value ^= value >> 15;
                value *= 0x846ca68bu;
                return value ^ (value >> 16);
            }
        }

        public static Rect DetailRect(Rect cell, uint variation)
        {
            float fraction = 0.78f + variation % 5u * 0.025f;
            float width = cell.width * fraction;
            float height = cell.height * fraction;
            float offsetX = ((variation / 5u) % 3u - 1f) * cell.width * 0.025f;
            return new Rect(cell.center.x - width * 0.5f + offsetX,
                cell.yMax - height - cell.height * 0.04f, width, height);
        }

        public static float Opacity(bool region) => region ? 0.76f : 0.94f;
    }
}
