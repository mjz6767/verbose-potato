using UnityEngine;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private Texture2D grandHearthFloorAtlas;
        private Texture2D grandHearthSetpieceAtlas;

        private void LoadGrandHearthArt()
        {
            grandHearthFloorAtlas = LoadExternalPng(RuntimeArtManifest.GrandHearthFloorAtlas);
            grandHearthSetpieceAtlas = LoadApprovedExternalPngWithAlpha(
                RuntimeArtManifest.GrandHearthSetpieceAtlas,
                0.20f,
                "Grand Hearth set-pieces",
                0.08f);
        }

        private bool IsGrandHearthFloorAtlas()
        {
            return grandHearthFloorAtlas != null
                && grandHearthFloorAtlas.width == 1536
                && grandHearthFloorAtlas.height == 1024
                && AtlasHasSquareCells(
                    grandHearthFloorAtlas,
                    GrandHearthArtCatalog.FloorAtlasColumns,
                    GrandHearthArtCatalog.FloorAtlasRows,
                    0.01f);
        }

        private bool IsGrandHearthSetpieceAtlas()
        {
            return grandHearthSetpieceAtlas != null
                && grandHearthSetpieceAtlas.width == 1536
                && grandHearthSetpieceAtlas.height == 1024
                && AtlasHasSquareCells(
                    grandHearthSetpieceAtlas,
                    GrandHearthArtCatalog.SetpieceAtlasColumns,
                    GrandHearthArtCatalog.SetpieceAtlasRows,
                    0.01f);
        }

        private bool TryDrawGrandHearthFloorTile(Rect rect, int x, int y, int tile)
        {
            if (!IsGrandHearthFloorAtlas()
                || !GrandHearthArtCatalog.TryFloorChoice(
                    state?.Map,
                    x,
                    y,
                    tile,
                    out GrandHearthFloorChoice choice))
            {
                return false;
            }

            Rect source = InsetAtlasSource(
                AtlasCell(
                    grandHearthFloorAtlas,
                    choice.AtlasIndex,
                    GrandHearthArtCatalog.FloorAtlasColumns,
                    GrandHearthArtCatalog.FloorAtlasRows),
                0.75f);
            Color tint = Color.white.WithAlpha(exploreWideView ? 0.94f : 0.99f);
            return DrawTextureRegionTintVariant(
                grandHearthFloorAtlas,
                rect,
                source,
                tint,
                choice.FlipX,
                choice.FlipY);
        }

        private int GrandHearthSetpieceAtlasIndex(MapObject obj)
        {
            return obj == null ? -1 : GrandHearthArtCatalog.SetpieceIndex(obj.Id);
        }

        private bool TryDrawGrandHearthSetpieceAtlasIcon(Rect rect, MapObject obj, Color tint)
        {
            int index = GrandHearthSetpieceAtlasIndex(obj);
            if (!IsGrandHearthSetpieceAtlas() || index < 0) return false;

            return TryDrawTrimmedExplorationAtlasCell(
                grandHearthSetpieceAtlas,
                rect,
                index,
                GrandHearthArtCatalog.SetpieceAtlasColumns,
                GrandHearthArtCatalog.SetpieceAtlasRows,
                tint,
                "Grand Hearth set-piece",
                0.08f,
                0.92f,
                new WorldMapArtSpec(1f, new Vector2(0.5f, 1f), Vector2.zero, false));
        }
    }
}
