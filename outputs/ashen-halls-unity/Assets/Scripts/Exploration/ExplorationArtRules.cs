using System;

namespace AshenHalls
{
    public static class ExplorationArtRules
    {
        private static readonly int[] ForestWallVariants = { 7, 20, 21, 22, 23, 24 };
        private static readonly int[] MireWallVariants = { 8, 25, 26, 27, 28, 29 };
        private static readonly int[] CliffWallVariants = { 9, 30, 31, 32, 33, 34 };
        private static readonly int[] RedWallVariants = { 10, 35, 36, 37, 38 };
        private static readonly int[] RuinsWallVariants = { 9, 39, 30, 31, 32, 33, 34 };

        public static int ViewportWidth(bool wideView, int adaptiveTier)
        {
            adaptiveTier = Clamp(adaptiveTier, 0, 2);
            if (wideView) return adaptiveTier >= 2 ? 21 : adaptiveTier == 1 ? 19 : 17;
            // Local view is for reading nearby actors and interaction targets. A
            // larger window should enlarge the cells, not quietly reveal another
            // district. Region view remains the explicit zoomed-out surface.
            return adaptiveTier >= 1 ? 13 : 11;
        }

        public static int ViewportHeight(bool wideView, int adaptiveTier)
        {
            adaptiveTier = Clamp(adaptiveTier, 0, 2);
            if (wideView) return adaptiveTier >= 2 ? 11 : 9;
            return 7;
        }

        public static int AdaptiveViewportTier(bool wideView, float availableWidth, float availableHeight)
        {
            float innerWidth = Math.Max(1f, availableWidth - 24f);
            float innerHeight = Math.Max(1f, availableHeight - 58f);
            for (int tier = 2; tier >= 1; tier--)
            {
                float cell = Math.Min(
                    innerWidth / ViewportWidth(wideView, tier),
                    innerHeight / ViewportHeight(wideView, tier));
                float minimumCell = wideView
                    ? tier == 2 ? 64f : 58f
                    : tier == 2 ? 86f : 80f;
                if (cell >= minimumCell) return tier;
            }

            return 0;
        }

        public static float PartyTokenPadding(bool wideView)
        {
            return wideView ? 0.18f : 0.12f;
        }

        public static string PartyTokenRole(int partyCount, string leadRole)
        {
            if (partyCount != 1) return "party";
            return string.IsNullOrWhiteSpace(leadRole) ? "shield" : leadRole.Trim().ToLowerInvariant();
        }

        public static int MaterialAtlasIndex(ExplorationMaterial material)
        {
            switch (material)
            {
                case ExplorationMaterial.CityPaving: return 0;
                case ExplorationMaterial.MarketCobbles: return 1;
                case ExplorationMaterial.TempleStone: return 2;
                case ExplorationMaterial.KeepStone: return 3;
                case ExplorationMaterial.NaturalGround:
                case ExplorationMaterial.Forest: return 4;
                case ExplorationMaterial.Moss: return 5;
                case ExplorationMaterial.RuinedPaving:
                case ExplorationMaterial.RuinedWall: return 6;
                case ExplorationMaterial.PackedDirt:
                case ExplorationMaterial.BridgeDeck: return 7;
                case ExplorationMaterial.FenMud: return 8;
                case ExplorationMaterial.ShallowWater:
                case ExplorationMaterial.DeepWater: return 9;
                case ExplorationMaterial.QuarryStone:
                case ExplorationMaterial.Cliff: return 10;
                case ExplorationMaterial.GlassRubble: return 11;
                case ExplorationMaterial.RedAsh:
                case ExplorationMaterial.RedBasalt: return 12;
                case ExplorationMaterial.GloamStone: return 13;
                case ExplorationMaterial.CisternBrick: return 14;
                case ExplorationMaterial.SewerBrick: return 15;
                default: return -1;
            }
        }

        public static int MaterialAtlasVariantIndex(ExplorationMaterial material, int variant)
        {
            int semanticSlot = MaterialAtlasIndex(material);
            if (semanticSlot < 0) return -1;
            return semanticSlot * 4 + PositiveMod(variant, 4);
        }

        public static int MaterialAtlasVariant(ExplorationMaterial material, int roll)
        {
            roll = PositiveMod(roll, 100);
            // Forest shares a legacy semantic slot with open natural ground, but
            // variants 1-3 are the dedicated loam/leaf-litter paintings. Avoid the
            // bright grassy approved cell inside dense woodland.
            if (material == ExplorationMaterial.Forest) return 1 + roll % 3;

            // v1.68 supplies three newer variants beside the preserved foundation
            // painting. Give all four cells an even share so the current art owns
            // most of the visible map without removing the familiar foundation.
            return roll / 25;
        }

        public static int MidgaardTileIndex(int tile, string kind, int roll, bool landmarkCell)
        {
            kind = kind ?? "";
            if (!kind.StartsWith("midgaard", StringComparison.Ordinal)) return -1;
            roll = PositiveMod(roll, 100);
            if (tile == 0 || string.Equals(kind, "midgaardwall", StringComparison.Ordinal)) return 9;

            // Landmark sprites own their semantic read. The terrain layer therefore
            // uses only the five calm paving cells, including beneath a landmark. This
            // prevents a fountain, stall, gate, or spell circle from being painted once
            // into the ground and then a second time as an object above it.
            switch (kind)
            {
                case "midgaard-market": return roll < 72 ? 19 : 15;
                case "midgaard-temple":
                case "midgaard-fountain":
                case "midgaard-recall": return roll < 72 ? 17 : 16;
                case "midgaard-diner":
                case "midgaard-tavern":
                case "midgaard-provisions": return roll < 70 ? 18 : 15;
                case "midgaard-armorer":
                case "midgaard-weapons":
                case "midgaard-gate":
                case "midgaard-guard":
                case "midgaard-king": return roll < 72 ? 16 : 15;
                case "midgaard-enchanter": return roll < 70 ? 17 : 16;
                case "midgaard-sewer":
                case "midgaard-ratquest": return roll < 72 ? 18 : 15;
                case "midgaard-road": return roll < 94 ? 15 : 16;
                case "midgaard-plaza": return roll < 88 ? 17 : 16;
                case "midgaard-paved": return MidgaardPavedIndex(roll);
                default: return 19;
            }
        }

        private static int MidgaardPavedIndex(int roll)
        {
            // One quiet foundation with a restrained secondary variant reads as a
            // city street. Cycling all five authored cells produced a patchwork.
            return roll < 86 ? 15 : 16;
        }

        public static int WorldMapTileIndex(int tile, string kind, int roll, bool roadJunction)
        {
            return WorldMapTileIndex(tile, kind, roll, roadJunction ? 15 : 0, false);
        }

        public static int WorldMapTileIndex(int tile, string kind, int roll, int pathMask, bool quietCell)
        {
            kind = kind ?? "";
            roll = PositiveMod(roll, 100);
            if (tile == 0)
            {
                if (kind == "forestwall") return PickVariant(roll, ForestWallVariants);
                if (kind == "mirewall") return PickVariant(roll, MireWallVariants);
                if (kind == "cliffwall") return PickVariant(roll, CliffWallVariants);
                if (kind == "redwall") return PickVariant(roll, RedWallVariants);
                if (kind == "ruinswall") return PickVariant(roll, RuinsWallVariants);
                return PickVariant(roll, CliffWallVariants);
            }

            // Tile identity must depend only on the map, never on party distance.
            // Cell 13 is a junction and is used only when connectivity proves it.
            if (kind == "road") return CountBits(pathMask) >= 3 ? 13 : 0;
            // Ordinary terrain uses one quiet foundation plus a sparse compatible
            // variant. Cells with baked shrines, stairs, bridges, gates, and other
            // landmarks are reserved for authored objects instead of being rolled
            // into the ground layer.
            if (quietCell) return CalmGroundIndex(kind);
            if (kind == "paved") return roll < 94 ? 1 : 12;
            if (kind == "ruins") return roll < 90 ? 1 : 12;
            if (kind == "gloam") return roll < 92 ? 1 : 12;
            if (kind == "cistern") return roll < 94 ? 3 : 16;
            if (kind == "outworks") return roll < 90 ? 0 : 1;
            if (kind == "moss") return roll < 94 ? 0 : 15;
            if (kind == "mire" || kind == "mud") return roll < 95 ? 3 : 15;
            if (kind == "quarry") return roll < 95 ? 12 : 4;
            // The bright crystal painting is useful as an accent but forms a cyan
            // wallpaper band when repeated. Neutral rubble receives the regional
            // tint; the crystal cell becomes a rare landmark-like variation.
            if (kind == "glass") return roll < 92 ? 12 : 5;
            // Cell 6 is a landmark-scale barred gate, not traversable ash ground.
            // Red reaches use neutral rubble tinted by the region palette instead.
            if (kind == "ash" || kind == "red") return 12;
            return 1;
        }

        private static int PickVariant(int roll, int[] indices)
        {
            if (indices == null || indices.Length == 0) return -1;
            return indices[PositiveMod(roll, indices.Length)];
        }

        private static int CalmGroundIndex(string kind)
        {
            if (kind == "road" || kind == "outworks" || kind == "moss") return 0;
            if (kind == "cistern" || kind == "mire" || kind == "mud") return 3;
            if (kind == "quarry" || kind == "glass" || kind == "ash" || kind == "red") return 12;
            return 1;
        }

        private static int CountBits(int value)
        {
            int count = 0;
            while (value != 0)
            {
                count += value & 1;
                value >>= 1;
            }
            return count;
        }

        public static int TerrainMacroSize(int tile, string kind)
        {
            // Atlas cells are authored as complete map tiles. Splitting one cell
            // over a 2x2 map block turns a tree, ruin, or paving detail into a giant
            // poster and creates the quilt-like seams seen in exploration.
            return 1;
        }

        public static bool CanMirrorTerrain(int tile, string kind)
        {
            if (tile == 0) return false;
            kind = kind ?? "";
            if (kind.StartsWith("midgaard", StringComparison.Ordinal)) return false;
            return kind != "road" && kind != "glass" && kind != "cistern";
        }

        private static int PositiveMod(int value, int divisor)
        {
            int result = value % divisor;
            return result < 0 ? result + divisor : result;
        }

        private static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}
