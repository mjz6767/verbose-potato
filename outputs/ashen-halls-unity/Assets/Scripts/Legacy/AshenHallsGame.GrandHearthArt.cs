using UnityEngine;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private Texture2D grandHearthFloorAtlas;
        private Texture2D grandHearthSetpieceAtlas;
        private Texture2D grandHearthAmbienceAtlas;

        private void LoadGrandHearthArt()
        {
            grandHearthFloorAtlas = LoadExternalPng(RuntimeArtManifest.GrandHearthFloorAtlas);
            grandHearthSetpieceAtlas = LoadApprovedExternalPngWithAlpha(
                RuntimeArtManifest.GrandHearthSetpieceAtlas,
                0.20f,
                "Grand Hearth set-pieces",
                0.08f);
            grandHearthAmbienceAtlas = LoadApprovedExternalPngWithAlpha(
                RuntimeArtManifest.GrandHearthAmbienceAtlas,
                0.20f,
                "Grand Hearth ambience",
                0.02f);
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

        private bool IsGrandHearthAmbienceAtlas()
        {
            return grandHearthAmbienceAtlas != null
                && grandHearthAmbienceAtlas.width == 1536
                && grandHearthAmbienceAtlas.height == 1024
                && AtlasHasSquareCells(
                    grandHearthAmbienceAtlas,
                    GrandHearthArtCatalog.AmbienceAtlasColumns,
                    GrandHearthArtCatalog.AmbienceAtlasRows,
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

        private bool DrawGrandHearthAmbienceCell(Rect destination, int index, float alpha)
        {
            if (!IsGrandHearthAmbienceAtlas()
                || index < 0
                || index >= GrandHearthArtCatalog.AmbienceAtlasCellCount)
            {
                return false;
            }

            Rect source = InsetAtlasSource(
                AtlasCell(
                    grandHearthAmbienceAtlas,
                    index,
                    GrandHearthArtCatalog.AmbienceAtlasColumns,
                    GrandHearthArtCatalog.AmbienceAtlasRows),
                0.75f);
            return DrawTextureRegionTint(
                grandHearthAmbienceAtlas,
                destination,
                source,
                Color.white.WithAlpha(Mathf.Clamp01(alpha)));
        }

        private bool DrawGrandHearthAmbienceCellRotated(
            Rect destination,
            int index,
            float alpha,
            float degrees)
        {
            Rect source = IsGrandHearthAmbienceAtlas()
                ? InsetAtlasSource(
                    AtlasCell(
                        grandHearthAmbienceAtlas,
                        index,
                        GrandHearthArtCatalog.AmbienceAtlasColumns,
                        GrandHearthArtCatalog.AmbienceAtlasRows),
                    0.75f)
                : Rect.zero;
            if (source.width <= 0f || source.height <= 0f) return false;

            Rect rotatedDrawRect = new Rect(
                destination.center.x - destination.height * 0.5f,
                destination.center.y - destination.width * 0.5f,
                destination.height,
                destination.width);
            Matrix4x4 previous = GUI.matrix;
            try
            {
                GUIUtility.RotateAroundPivot(degrees, destination.center);
                return DrawTextureRegionTint(
                    grandHearthAmbienceAtlas,
                    rotatedDrawRect,
                    source,
                    Color.white.WithAlpha(Mathf.Clamp01(alpha)));
            }
            finally
            {
                GUI.matrix = previous;
            }
        }

        private Rect GrandHearthScreenRect(
            Rect grid,
            float cell,
            Point origin,
            float mapX,
            float mapY,
            float widthInCells,
            float heightInCells)
        {
            return new Rect(
                grid.x + (mapX - origin.X) * cell,
                grid.y + (mapY - origin.Y) * cell,
                widthInCells * cell,
                heightInCells * cell);
        }

        private void DrawGrandHearthPatronShadow(
            Rect cellRect,
            int x,
            int y,
            int tile,
            System.Collections.Generic.HashSet<int> guidanceCells)
        {
            if (!IsGrandHearthAmbienceAtlas()
                || state?.Map == null
                || state.Depth != 1
                || tile != 1
                || x == state.PlayerX && y == state.PlayerY
                || ObjectAt(state.Map, x, y) != null
                || IsExploreGuidanceCell(x, y, guidanceCells)
                || !MidgaardInteriorRules.TryGrandHearthPatron(state.Map, x, y, out _))
            {
                return;
            }

            Rect shadow = new Rect(
                cellRect.x - cellRect.width * 0.04f,
                cellRect.y + cellRect.height * 0.47f,
                cellRect.width * 1.08f,
                cellRect.height * 0.62f);
            DrawGrandHearthAmbienceCell(
                shadow,
                GrandHearthArtCatalog.PatronShadowCell,
                exploreWideView ? 0.36f : 0.48f);
        }

        private void DrawGrandHearthAmbience(
            Rect grid,
            float cell,
            Point origin,
            int viewW,
            int viewH)
        {
            if (!IsGrandHearthAmbienceAtlas() || state?.Map == null || state.Depth != 1) return;

            RectInt room = MidgaardInteriorRules.GrandHearthBounds(state.Map);
            RectInt viewport = new RectInt(origin.X, origin.Y, viewW, viewH);
            if (!room.Overlaps(viewport)) return;

            float wideScale = exploreWideView ? 0.82f : 1f;
            DrawGrandHearthAmbienceCell(
                GrandHearthScreenRect(
                    grid,
                    cell,
                    origin,
                    room.xMin + 0.10f,
                    room.yMin + 0.55f,
                    5.00f,
                    4.30f),
                GrandHearthArtCatalog.HearthLightCell,
                0.28f * wideScale);

            DrawGrandHearthAmbienceCellRotated(
                GrandHearthScreenRect(
                    grid,
                    cell,
                    origin,
                    room.xMax - 4.65f,
                    MidgaardInteriorRules.GrandHearthAisleY(state.Map) - 1.35f,
                    4.85f,
                    2.75f),
                GrandHearthArtCatalog.StormDoorLightCell,
                0.26f * wideScale,
                90f);

            DrawGrandHearthAmbienceCell(
                GrandHearthScreenRect(
                    grid,
                    cell,
                    origin,
                    room.xMax - 4.15f,
                    room.yMin + 0.10f,
                    2.35f,
                    3.70f),
                GrandHearthArtCatalog.WindowReflectionCell,
                0.18f * wideScale);

            float sconceAlpha = exploreWideView ? 0.48f : 0.65f;
            DrawGrandHearthAmbienceCell(
                GrandHearthScreenRect(
                    grid,
                    cell,
                    origin,
                    room.xMin - 0.05f,
                    room.yMin + 2.45f,
                    1.05f,
                    1.35f),
                GrandHearthArtCatalog.WallSconceCell,
                sconceAlpha);
            DrawGrandHearthAmbienceCell(
                GrandHearthScreenRect(
                    grid,
                    cell,
                    origin,
                    room.xMax - 1.00f,
                    room.yMin + 5.40f,
                    1.05f,
                    1.35f),
                GrandHearthArtCatalog.WallSconceCell,
                sconceAlpha);

            DrawGrandHearthAmbienceCell(
                GrandHearthScreenRect(
                    grid,
                    cell,
                    origin,
                    room.xMin + 1.20f,
                    room.yMin + 0.15f,
                    2.20f,
                    3.25f),
                GrandHearthArtCatalog.EmberMotesCell,
                exploreWideView ? 0.52f : 0.72f);
        }
    }
}
