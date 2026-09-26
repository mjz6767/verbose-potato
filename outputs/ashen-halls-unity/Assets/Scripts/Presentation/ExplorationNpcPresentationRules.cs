using UnityEngine;

namespace AshenHalls
{
    public static class ExplorationNpcPresentationRules
    {
        public static float NamedObjectPadding(bool wideView)
        {
            return wideView ? 0.14f : 0.05f;
        }

        public static float NamedArtPadding()
        {
            return 0.02f;
        }

        public static float NamedArtScale(bool wideView)
        {
            return wideView ? 1.02f : 1.08f;
        }

        public static float NamedFootlineInCells(bool wideView)
        {
            float objectSize = 1f - 2f * NamedObjectPadding(wideView);
            float artSize = objectSize * (1f - 2f * NamedArtPadding());
            // Mirrors the bottom-anchored named atlas: the existing art spec
            // has a 0.02 vertical offset. Include its centered scale expansion.
            return 0.5f + artSize * (NamedArtScale(wideView) * 0.5f + 0.02f);
        }

        public static float AmbientGroundingOffsetInCells(bool wideView, bool interiorPatron)
        {
            float padding = interiorPatron ? GrandHearthPatronPadding(wideView) : ExteriorAmbientPadding(wideView);
            // The ambient 0.98 scale and 0.01 offset cancel at the feet.
            return NamedFootlineInCells(wideView) - (1f - padding);
        }

        public static Rect AmbientSpriteRect(Rect cell, bool wideView, bool interiorPatron, Vector2 yieldingOffsetInCells)
        {
            float padding = interiorPatron ? GrandHearthPatronPadding(wideView) : ExteriorAmbientPadding(wideView);
            float pad = cell.width * padding;
            Rect sprite = new Rect(cell.x + pad, cell.y + pad, cell.width - pad * 2f, cell.height - pad * 2f);
            sprite.x += cell.width * yieldingOffsetInCells.x;
            sprite.y += cell.height * (AmbientGroundingOffsetInCells(wideView, interiorPatron) + yieldingOffsetInCells.y);
            // A yielding citizen may step sideways, never sink into the next
            // terrain row. Atlas scale/offset keep its footline at yMax.
            sprite.y = Mathf.Min(sprite.y, cell.yMax - sprite.height);
            return sprite;
        }

        public static Rect ContactFootprint(Rect cell, bool wideView)
        {
            float baseline = cell.y + cell.height * NamedFootlineInCells(wideView);
            return new Rect(cell.x + cell.width * 0.07f, baseline - cell.height * 0.08f,
                cell.width * 0.86f, cell.height * 0.08f);
        }

        public static bool ShouldShowContactBadge(bool wideView, float cellPixels, bool focused, bool currentInteraction = false)
        {
            // The active contact already owns the E key and a foot-level cue.
            // Keep its role in the action rail instead of stacking another chip.
            return !wideView && focused && !currentInteraction
                && cellPixels >= 44f && !float.IsInfinity(cellPixels);
        }

        public static Rect ContactBadge(Rect cell)
        {
            float size = Mathf.Clamp(cell.width * 0.22f, 10f, 20f);
            return new Rect(cell.xMax - size - cell.width * 0.02f,
                cell.yMax - size - cell.height * 0.11f, size, size);
        }

        public static Rect ContactUseKey(Rect cell)
        {
            float width = Mathf.Clamp(cell.width * 0.24f, 12f, 22f);
            float height = Mathf.Clamp(cell.height * 0.18f, 11f, 16f);
            return new Rect(cell.xMax - width - cell.width * 0.025f,
                cell.y + cell.height * 0.05f, width, height);
        }

        public static bool ShouldDrawExteriorAmbientCitizen(bool wideView)
        {
            // Region view uses landmark and role markers; full-body passersby
            // become visual noise at that scale.
            return !wideView;
        }

        public static bool ShouldDrawInteriorAmbientPatron(bool wideView)
        {
            // Interior crowds are local scenery, just like exterior passersby.
            // Do not repaint six tiny body sprites on a strategic Region map.
            return !wideView;
        }

        public static bool ShouldUseRegionRoleMarker(bool wideView, bool namedContactOrGuard)
        {
            // One representation per view: proximity and objective importance
            // change emphasis/visibility, not the actor's visual language.
            return wideView && namedContactOrGuard;
        }

        public static float ExteriorAmbientPadding(bool wideView)
        {
            // Ambient citizens remain slightly subordinate to interactive
            // actors through opacity, not a visibly different body scale.
            return wideView ? 0.16f : 0.06f;
        }

        public static float ExteriorAmbientAlpha(bool wideView, bool yieldingToParty)
        {
            if (yieldingToParty) return wideView ? 0.54f : 0.68f;
            return wideView ? 0.64f : 0.82f;
        }

        public static float GrandHearthPatronPadding(bool wideView)
        {
            return wideView ? 0.15f : 0.06f;
        }

        public static float GrandHearthPatronAlpha(bool wideView)
        {
            return wideView ? 0.72f : 0.94f;
        }
    }
}
