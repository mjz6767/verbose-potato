using UnityEngine;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private const int MidgaardRoadSurfaceColumns = 2;
        private const int MidgaardRoadSurfaceRows = 2;
        private const int MidgaardRoadSurfaceSize = 512;

        private Texture2D midgaardRoadSurfaceAtlas;

        private void LoadMidgaardRoadSurfaceArt()
        {
            midgaardRoadSurfaceAtlas = LoadExternalPng(RuntimeArtManifest.MidgaardRoadSurfaceAtlas);
            if (!IsMidgaardRoadSurfaceAtlas())
            {
                if (midgaardRoadSurfaceAtlas != null)
                {
                    Debug.LogWarning(
                        $"Rejected Midgaard road surface atlas '{RuntimeArtManifest.MidgaardRoadSurfaceAtlas}' "
                        + $"because it is {midgaardRoadSurfaceAtlas.width}x{midgaardRoadSurfaceAtlas.height}; "
                        + $"the exact contract is {MidgaardRoadSurfaceSize}x{MidgaardRoadSurfaceSize}.");
                }
                midgaardRoadSurfaceAtlas = null;
                return;
            }

            midgaardRoadSurfaceAtlas.filterMode = FilterMode.Bilinear;
            midgaardRoadSurfaceAtlas.wrapMode = TextureWrapMode.Clamp;
            midgaardRoadSurfaceAtlas.anisoLevel = 0;
        }

        private bool IsMidgaardRoadSurfaceAtlas()
        {
            return midgaardRoadSurfaceAtlas != null
                && midgaardRoadSurfaceAtlas.width == MidgaardRoadSurfaceSize
                && midgaardRoadSurfaceAtlas.height == MidgaardRoadSurfaceSize
                && AtlasHasSquareCells(
                    midgaardRoadSurfaceAtlas,
                    MidgaardRoadSurfaceColumns,
                    MidgaardRoadSurfaceRows,
                    1f);
        }

        private bool TryDrawExploreRoadSurfaceTexture(
            Rect cell,
            ExplorationRoadVisualPlan plan,
            float coreWidth)
        {
            if (!IsMidgaardRoadSurfaceAtlas()
                || plan.Tier == ExplorationRoadVisualTier.Bridge
                || plan.Tier == ExplorationRoadVisualTier.Trail)
            {
                return false;
            }

            // Keep every contiguous tier on one seamless swatch. Per-cell
            // random variants would reintroduce visible boundaries at the
            // exact places this pass is meant to join into one road.
            int index = plan.CivicSurface
                ? plan.Tier == ExplorationRoadVisualTier.OldRoad ? 0 : 1
                : plan.Tier == ExplorationRoadVisualTier.OldRoad ? 2 : 3;
            Rect sourcePixels = AtlasCell(
                midgaardRoadSurfaceAtlas,
                index,
                MidgaardRoadSurfaceColumns,
                MidgaardRoadSurfaceRows);
            float alpha = plan.CivicSurface
                ? exploreWideView ? 0.56f : 0.74f
                : exploreWideView ? 0.46f : 0.62f;
            Color tint = Color.white.WithAlpha(alpha);

            float width = Mathf.Clamp(
                coreWidth * 0.94f,
                1f,
                Mathf.Min(cell.width, cell.height) * 0.88f);
            float half = width * 0.5f;
            float cx = cell.center.x;
            float cy = cell.center.y;

            if (!plan.DrawJunctionApron
                && (plan.MainMask == (ExplorationRoadPresentationRules.North | ExplorationRoadPresentationRules.South)
                    || plan.MainMask == (ExplorationRoadPresentationRules.East | ExplorationRoadPresentationRules.West)))
            {
                Rect straight = plan.MainMask == (ExplorationRoadPresentationRules.North | ExplorationRoadPresentationRules.South)
                    ? new Rect(cx - half, cell.y, width, cell.height)
                    : new Rect(cell.x, cy - half, cell.width, width);
                DrawRoadSurfaceTextureClip(cell, straight, sourcePixels, tint);
                return true;
            }

            if (plan.DrawJunctionApron)
            {
                DrawRoadSurfaceTextureClip(
                    cell,
                    new Rect(cx - half, cy - half, width, width),
                    sourcePixels,
                    tint);
            }
            if ((plan.MainMask & ExplorationRoadPresentationRules.North) != 0)
            {
                DrawRoadSurfaceTextureClip(
                    cell,
                    new Rect(cx - half, cell.y, width, Mathf.Max(0f, cy - half - cell.y)),
                    sourcePixels,
                    tint);
            }
            if ((plan.MainMask & ExplorationRoadPresentationRules.East) != 0)
            {
                DrawRoadSurfaceTextureClip(
                    cell,
                    new Rect(cx + half, cy - half, Mathf.Max(0f, cell.xMax - cx - half), width),
                    sourcePixels,
                    tint);
            }
            if ((plan.MainMask & ExplorationRoadPresentationRules.South) != 0)
            {
                DrawRoadSurfaceTextureClip(
                    cell,
                    new Rect(cx - half, cy + half, width, Mathf.Max(0f, cell.yMax - cy - half)),
                    sourcePixels,
                    tint);
            }
            if ((plan.MainMask & ExplorationRoadPresentationRules.West) != 0)
            {
                DrawRoadSurfaceTextureClip(
                    cell,
                    new Rect(cell.x, cy - half, Mathf.Max(0f, cx - half - cell.x), width),
                    sourcePixels,
                    tint);
            }
            return true;
        }

        private void DrawRoadSurfaceTextureClip(
            Rect cell,
            Rect clip,
            Rect sourcePixels,
            Color tint)
        {
            if (clip.width <= 0f || clip.height <= 0f) return;
            Rect source = new Rect(
                sourcePixels.x / midgaardRoadSurfaceAtlas.width,
                1f - (sourcePixels.y + sourcePixels.height) / midgaardRoadSurfaceAtlas.height,
                sourcePixels.width / midgaardRoadSurfaceAtlas.width,
                sourcePixels.height / midgaardRoadSurfaceAtlas.height);
            Color previous = GUI.color;
            GUI.BeginGroup(clip);
            GUI.color = tint;
            GUI.DrawTextureWithTexCoords(
                new Rect(cell.x - clip.x, cell.y - clip.y, cell.width, cell.height),
                midgaardRoadSurfaceAtlas,
                source,
                true);
            GUI.color = previous;
            GUI.EndGroup();
        }
    }
}
