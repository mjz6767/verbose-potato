using System.Collections.Generic;
using UnityEngine;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private Texture2D worldEnvironmentDetailAtlas;

        private void LoadWorldEnvironmentDetailAtlas()
        {
            worldEnvironmentDetailAtlas = null;
            Texture2D texture = LoadApprovedExternalPngWithAlpha(
                RuntimeArtManifest.EnvironmentDetailAtlas, 0.20f, "world environment details", 0.04f);
            if (texture == null) return;
            if (texture.width != ExplorationEnvironmentDetailRules.Width || texture.height != ExplorationEnvironmentDetailRules.Height)
            {
                Debug.LogWarning("Rejected world environment details atlas: expected "
                    + ExplorationEnvironmentDetailRules.Width + "x" + ExplorationEnvironmentDetailRules.Height
                    + ", received " + texture.width + "x" + texture.height + ". Keeping existing scenery.");
                return;
            }
            MeasureAtlasAlpha(texture, out float transparent, out float visible);
            // The shared legacy loader tolerates an unreadable alpha snapshot.
            // New environment details do not: keep the existing scenery unless
            // their transparency can be positively verified.
            if (!(transparent >= 0.20f && visible >= 0.04f))
            {
                Debug.LogWarning("Rejected world environment details atlas because its alpha could not be verified. Keeping existing scenery.");
                return;
            }
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;
            worldEnvironmentDetailAtlas = texture;
        }

        private void ValidateWorldEnvironmentDetailAtlas()
        {
            ValidateSpriteAtlasAlpha(worldEnvironmentDetailAtlas, "world environment details", 0.20f, 0.04f);
            ValidateAtlasCells(worldEnvironmentDetailAtlas, "world environment detail",
                ExplorationEnvironmentDetailRules.Columns, ExplorationEnvironmentDetailRules.Rows, true,
                new[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            ValidateAtlasSquareCells(worldEnvironmentDetailAtlas, "world environment detail",
                ExplorationEnvironmentDetailRules.Columns, ExplorationEnvironmentDetailRules.Rows, 1f);
        }

        private bool TryDrawExploreEnvironmentDetail(Rect cell, int x, int y, int tile, HashSet<int> guidanceCells)
        {
            if (worldEnvironmentDetailAtlas == null || state?.Map == null) return false;
            ExplorationMaterial material = ExploreMaterialAt(x, y);
            if (!ExplorationEnvironmentDetailRules.Selected(x, y, state.Seed, state.Depth, material)) return false;
            bool occupied = ObjectAt(state.Map, x, y) != null || IsRoamingThreatRenderCell(x, y);
            bool protectedFocus = Distance(x, y, state.PlayerX, state.PlayerY) <= 1 || HasNearbyMidgaardFocusObject(x, y);
            if (!ExplorationEnvironmentDetailRules.Eligible(IsExploreCellCharted(x, y),
                IsMidgaardInteriorCell(x, y, state.Map, state.Depth), tile, ExploreRolesAt(x, y),
                occupied, protectedFocus, IsExploreGuidanceCell(x, y, guidanceCells), material)) return false;

            uint variation = ExplorationEnvironmentDetailRules.Hash(x, y, state.Seed, state.Depth);
            int index = ExplorationEnvironmentDetailRules.CellFor(material, (int)(variation >> 8));
            Rect target = ExplorationEnvironmentDetailRules.DetailRect(cell, variation);
            return DrawTextureRegionTintVariant(worldEnvironmentDetailAtlas, target,
                InsetAtlasSource(AtlasCell(worldEnvironmentDetailAtlas, index,
                    ExplorationEnvironmentDetailRules.Columns, ExplorationEnvironmentDetailRules.Rows), 0.75f),
                Color.white.WithAlpha(ExplorationEnvironmentDetailRules.Opacity(exploreWideView)),
                (variation & 1u) != 0u, false);
        }
    }
}
