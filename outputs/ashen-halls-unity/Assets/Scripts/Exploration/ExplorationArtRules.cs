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
            return wideView ? 0.09f : 0.10f;
        }

        public static float PartyRegionMarkerScale()
        {
            return 0.82f;
        }

        public static float PartyRegionMarkerMinimumPixels()
        {
            return 22f;
        }

        public static bool IsMidgaardBuilding(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Market:
                case ObjectType.Temple:
                case ObjectType.Diner:
                case ObjectType.Tavern:
                case ObjectType.Armorer:
                case ObjectType.Provisions:
                case ObjectType.WeaponVendor:
                case ObjectType.Enchanter:
                case ObjectType.KingHall:
                    return true;
                default:
                    return false;
            }
        }

        public static float MidgaardBuildingPadding(bool wideView)
        {
            // Buildings carry district identity at both zoom levels. The former
            // generic-object padding made shop sprites notably smaller than the
            // tavern and temple even though they share the same authored atlas.
            return wideView ? 0.08f : 0f;
        }

        public static float MidgaardBuildingSpriteScale(bool wideView)
        {
            // Region view still needs an almost cell-filling silhouette; local view
            // can let roofs and awnings rise beyond the cell without changing the
            // deterministic interaction footprint beneath them.
            return wideView ? 1.22f : 1.34f;
        }

        public static float MidgaardBuildingArtPadding()
        {
            return 0.01f;
        }

        public static float MidgaardBuildingFoundationWidthInCells(bool wideView)
        {
            // A building painting may rise beyond its cell, but its ground contact
            // must still communicate the one-cell movement footprint accurately.
            return wideView ? 0.94f : 0.98f;
        }

        public static float MidgaardBuildingFoundationHeightInCells(bool wideView)
        {
            return wideView ? 0.38f : 0.46f;
        }

        public static float MidgaardBuildingVerticalOffset(bool wideView)
        {
            // Scaling from the destination centre would grow just as far into the
            // street as toward the roofline. Shift upward to keep doorsteps and
            // market counters visually tied to their map cell.
            return wideView ? -0.10f : -0.16f;
        }

        public static string PartyTokenRole(int partyCount, string leadRole)
        {
            if (partyCount != 1) return "party";
            return string.IsNullOrWhiteSpace(leadRole) ? "shield" : leadRole.Trim().ToLowerInvariant();
        }

        public static float GateArtWidthInCells(bool wideView, bool sideGate)
        {
            // East and west use a wall-aligned pair of bastions with a transparent
            // horizontal lane. Keep their objective frame close to the wall instead
            // of restoring the old front-elevation landmark footprint.
            if (wideView) return sideGate ? 0.70f : 1.78f;
            return sideGate ? 0.78f : 2.02f;
        }

        public static float GateArtHeightInCells(bool wideView, bool sideGate)
        {
            if (wideView) return sideGate ? 1.50f : 1.40f;
            return sideGate ? 1.70f : 1.58f;
        }

        public static float GateArtBaseOffsetInCells()
        {
            return 0.08f;
        }

        public static float MidgaardWallBandThickness(bool wideView)
        {
            return wideView ? 0.52f : 0.56f;
        }

        public static float MidgaardWallVerticalBandThickness(bool wideView)
        {
            // The authored north-south wall cells are deliberately narrow. A
            // separate vertical foundation prevents the old broad gray rails
            // from showing on both sides of the masonry while horizontal runs
            // retain their substantial parapet depth.
            return wideView ? 0.34f : 0.36f;
        }

        public static int MidgaardWallConnectionMask(int atlasIndex)
        {
            // N=1, E=2, S=4, W=8. The atlas's top corner paintings are ordered
            // top-right (4), top-left (5), then bottom-left/right (6/7).
            switch (atlasIndex)
            {
                case 0:
                case 1:
                case 8:
                    return 2 | 8;
                case 2:
                case 3:
                case 9:
                    return 1 | 4;
                case 4:
                    return 4 | 8;
                case 5:
                    return 2 | 4;
                case 6:
                    return 1 | 2;
                case 7:
                    return 1 | 8;
                default:
                    return 0;
            }
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

            // Each semantic bank is authored as four compatible variants. Keep the
            // coordinate-derived distribution even so Local and Region views share
            // one stable material field without movement-dependent texture changes.
            return roll / 25;
        }

        public static bool ShouldBlendMaterialEdge(
            ExplorationMaterial current,
            ExplorationMaterial neighbor,
            bool currentOpen,
            bool neighborOpen,
            bool protectedThreshold)
        {
            if (!currentOpen || !neighborOpen || protectedThreshold) return false;
            int currentSlot = MaterialAtlasIndex(current);
            int neighborSlot = MaterialAtlasIndex(neighbor);
            return currentSlot >= 0 && neighborSlot >= 0 && currentSlot != neighborSlot;
        }

        public static int MaterialBlendBandCount()
        {
            return 3;
        }

        public static float MaterialBlendBandFraction(bool wideView)
        {
            return wideView ? 0.075f : 0.065f;
        }

        public static float MaterialBlendBandAlpha(int band, bool wideView)
        {
            switch (Clamp(band, 0, MaterialBlendBandCount() - 1))
            {
                case 0: return wideView ? 0.46f : 0.42f;
                case 1: return wideView ? 0.23f : 0.20f;
                default: return 0.08f;
            }
        }

        public static float MaterialBlendSourceStart(
            bool neighborOnNegativeAxis,
            int band,
            float bandFraction,
            bool sourceFlipped)
        {
            bandFraction = Math.Max(0.01f, Math.Min(0.20f, bandFraction));
            band = Math.Max(0, band);
            float logicalStart = neighborOnNegativeAxis
                ? 1f - (band + 1) * bandFraction
                : band * bandFraction;
            logicalStart = Math.Max(0f, Math.Min(1f - bandFraction, logicalStart));
            if (sourceFlipped)
            {
                logicalStart = 1f - logicalStart - bandFraction;
            }
            return Math.Max(0f, Math.Min(1f - bandFraction, logicalStart));
        }

        public static bool MaterialBlendDrawFlip(bool sourceFlipped)
        {
            // A feather extends away from the shared edge into the current cell.
            // Reverse the sampled neighbor strip so its boundary texel remains on
            // that shared edge rather than landing on the strip's interior side.
            return !sourceFlipped;
        }

        public static bool ShouldDrawMaterialPathStroke(
            ExplorationMaterial material,
            ExplorationCellRole roles)
        {
            if (!ExplorationSurfaceRules.IsPath(roles)) return false;
            // The 3x3 approach clearings outside Midgaard can retain generated
            // road roles beneath their Plaza role. Painting every connection there
            // produces a dark tic-tac-toe grid over otherwise coherent packed dirt.
            return material != ExplorationMaterial.PackedDirt
                || (roles & ExplorationCellRole.Plaza) == 0;
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
