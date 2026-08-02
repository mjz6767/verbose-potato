using System.Collections.Generic;
using UnityEngine;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private const int V24WorldMapAtlasColumns = 4;
        private const int V24WorldMapAtlasRows = 2;
        private const int V24WorldMapAtlasWidth = 1536;
        private const int V24WorldMapAtlasHeight = 768;

        private Texture2D worldThreatHabitatAtlas;
        private Texture2D worldNpcCitizenAtlas;
        private Texture2D playerExplorationRoleAtlas;

        private void LoadV24WorldMapArtAtlases()
        {
            worldThreatHabitatAtlas = LoadExactV24WorldMapAtlas(
                RuntimeArtManifest.WorldThreatHabitatAtlas,
                "world threat habitats",
                0.30f);
            worldNpcCitizenAtlas = LoadExactV24WorldMapAtlas(
                RuntimeArtManifest.WorldNpcCitizenAtlas,
                "world NPC citizens",
                0.20f);
            playerExplorationRoleAtlas = LoadExactV24WorldMapAtlas(
                RuntimeArtManifest.PlayerExplorationRoleAtlas,
                "player exploration roles",
                0.18f);
        }

        private Texture2D LoadExactV24WorldMapAtlas(string fileName, string label, float minimumVisibleFraction)
        {
            Texture2D texture = LoadApprovedExternalPngWithAlpha(fileName, 0.20f, label, minimumVisibleFraction);
            if (texture == null) return null;
            if (texture.width == V24WorldMapAtlasWidth && texture.height == V24WorldMapAtlasHeight) return texture;

            Debug.LogWarning(
                $"Rejected approved {label} atlas '{fileName}' because it is {texture.width}x{texture.height}; "
                + $"the v2.4 runtime contract is exactly {V24WorldMapAtlasWidth}x{V24WorldMapAtlasHeight}.");
            return null;
        }

        private bool IsWorldThreatHabitatAtlas()
        {
            return IsExactV24WorldMapAtlas(worldThreatHabitatAtlas);
        }

        private bool IsWorldNpcCitizenAtlas()
        {
            return IsExactV24WorldMapAtlas(worldNpcCitizenAtlas);
        }

        private bool IsPlayerExplorationRoleAtlas()
        {
            return IsExactV24WorldMapAtlas(playerExplorationRoleAtlas);
        }

        private bool IsExactV24WorldMapAtlas(Texture2D texture)
        {
            return texture != null
                && texture.width == V24WorldMapAtlasWidth
                && texture.height == V24WorldMapAtlasHeight;
        }

        private void ValidateV24WorldMapArtContracts()
        {
            ValidateSpriteAtlasAlpha(worldThreatHabitatAtlas, "world threat habitats", 0.20f, 0.30f);
            ValidateSpriteAtlasAlpha(worldNpcCitizenAtlas, "world NPC citizens", 0.20f, 0.20f);
            ValidateSpriteAtlasAlpha(playerExplorationRoleAtlas, "player exploration roles", 0.20f, 0.18f);
            int[] allCells = { 0, 1, 2, 3, 4, 5, 6, 7 };
            ValidateAtlasCells(worldThreatHabitatAtlas, "world threat habitat", V24WorldMapAtlasColumns, V24WorldMapAtlasRows, true, allCells);
            ValidateAtlasCells(worldNpcCitizenAtlas, "world NPC citizen", V24WorldMapAtlasColumns, V24WorldMapAtlasRows, true, allCells);
            ValidateAtlasCells(playerExplorationRoleAtlas, "player exploration role", V24WorldMapAtlasColumns, V24WorldMapAtlasRows, true, allCells);
            ValidateAtlasSquareCells(worldThreatHabitatAtlas, "world threat habitat", V24WorldMapAtlasColumns, V24WorldMapAtlasRows, 1f);
            ValidateAtlasSquareCells(worldNpcCitizenAtlas, "world NPC citizen", V24WorldMapAtlasColumns, V24WorldMapAtlasRows, 1f);
            ValidateAtlasSquareCells(playerExplorationRoleAtlas, "player exploration role", V24WorldMapAtlasColumns, V24WorldMapAtlasRows, 1f);
        }

        private bool TryDrawWorldThreatHabitatAtlasIcon(
            Rect rect,
            int index,
            Color tint,
            WorldMapArtSpec spec)
        {
            if (!IsWorldThreatHabitatAtlas() || index < 0 || index >= WorldThreatHabitatPresentationRules.CellCount) return false;
            return TryDrawTrimmedExplorationAtlasCell(
                worldThreatHabitatAtlas,
                rect,
                index,
                V24WorldMapAtlasColumns,
                V24WorldMapAtlasRows,
                tint,
                "world threat habitat",
                0.40f,
                0.55f,
                spec);
        }

        private bool TryDrawWorldNpcCitizenAtlasIcon(
            Rect rect,
            int index,
            Color tint,
            WorldMapArtSpec spec)
        {
            if (!IsWorldNpcCitizenAtlas() || index < 0 || index >= ExplorationCharacterArtCatalog.CitizenCellCount) return false;
            return TryDrawTrimmedExplorationAtlasCell(
                worldNpcCitizenAtlas,
                rect,
                index,
                V24WorldMapAtlasColumns,
                V24WorldMapAtlasRows,
                tint,
                "ambient world citizen",
                0.20f,
                0.40f,
                spec);
        }

        private bool TryDrawPlayerExplorationRoleAtlasIcon(
            Rect rect,
            int index,
            Color tint,
            WorldMapArtSpec spec)
        {
            if (!IsPlayerExplorationRoleAtlas() || index < 0 || index >= ExplorationCharacterArtCatalog.PlayerCellCount) return false;
            return TryDrawTrimmedExplorationAtlasCell(
                playerExplorationRoleAtlas,
                rect,
                index,
                V24WorldMapAtlasColumns,
                V24WorldMapAtlasRows,
                tint,
                "player exploration role",
                0.18f,
                0.50f,
                spec);
        }

        private void DrawRoamingThreatHabitats(
            Rect grid,
            float cell,
            Point origin,
            int viewW,
            int viewH,
            HashSet<int> guidanceCells)
        {
            if (state?.Map == null || state.RoamingThreats == null || !IsWorldThreatHabitatAtlas()) return;
            foreach (RoamingThreat threat in state.RoamingThreats)
            {
                if (threat == null || threat.Depth != state.Depth) continue;
                bool homeInBounds = threat.HomeX >= 0
                    && threat.HomeY >= 0
                    && threat.HomeX < state.Map.Width
                    && threat.HomeY < state.Map.Height;
                ExplorationCellRole roles = homeInBounds
                    ? ExplorationSurfaceRules.RolesAt(state.Map, threat.HomeX, threat.HomeY)
                    : ExplorationCellRole.None;
                WorldZone zone = homeInBounds ? ZoneFor(threat.HomeX, threat.HomeY, state.Map, state.Depth) : null;
                bool certifiedSafeRoad = ExplorationSurfaceRules.IsPath(roles)
                    && (zone == null || zone.Danger <= 0);
                if (!WorldThreatHabitatPresentationRules.ShouldDrawAtHome(homeInBounds, certifiedSafeRoad)) continue;
                if (!ExplorePointInViewport(threat.HomeX, threat.HomeY, origin, viewW, viewH)) continue;

                Rect homeCell = new Rect(
                    grid.x + (threat.HomeX - origin.X) * cell,
                    grid.y + (threat.HomeY - origin.Y) * cell,
                    cell,
                    cell);
                float size = cell * WorldThreatHabitatPresentationRules.MapScale(exploreWideView);
                Rect habitatRect = new Rect(
                    homeCell.center.x - size * 0.5f,
                    homeCell.yMax - size,
                    size,
                    size);
                float safeInset = Mathf.Max(1f, cell * 0.04f);
                bool fitsViewport = habitatRect.xMin >= grid.xMin + safeInset
                    && habitatRect.yMin >= grid.yMin + safeInset
                    && habitatRect.xMax <= grid.xMax - safeInset
                    && habitatRect.yMax <= grid.yMax - safeInset;
                if (!fitsViewport) habitatRect = Pad(homeCell, homeCell.width * 0.03f);

                RoamingThreatDefinition definition = RoamingThreatCatalog.Find(
                    threat.Id,
                    threat.Depth,
                    ContentSetCatalog.IsFullPrototype(activeContentSet));
                int index = WorldThreatHabitatPresentationRules.PresentationIndex(
                    threat.Active,
                    definition,
                    threat.Archetype);

                bool onObjectiveRoute = IsExploreGuidanceCell(threat.HomeX, threat.HomeY, guidanceCells);
                float alpha = WorldThreatHabitatPresentationRules.TintAlpha(onObjectiveRoute);
                if (threat.Active) alpha = Mathf.Max(alpha, 0.78f);
                else alpha = Mathf.Min(alpha, 0.52f);
                WorldMapArtSpec spec = new WorldMapArtSpec(
                    0.98f,
                    new Vector2(
                        WorldThreatHabitatPresentationRules.BottomCenterPivotX,
                        WorldThreatHabitatPresentationRules.BottomCenterPivotY),
                    Vector2.zero,
                    true);
                if (TryDrawWorldThreatHabitatAtlasIcon(habitatRect, index, Color.white.WithAlpha(alpha), spec)
                    && showExploreArtDebug)
                {
                    DrawExploreArtDebugOverlay(homeCell, habitatRect, "Habitat: " + threat.Name);
                }
            }
        }

        private bool TryDrawWorldAmbientCitizen(
            Rect cell,
            int x,
            int y,
            int tile,
            HashSet<int> guidanceCells)
        {
            if (TryDrawGrandHearthPatron(cell, x, y, tile, guidanceCells)) return true;

            if (state?.Map == null
                || tile != 1
                || !IsWorldNpcCitizenAtlas()
                || IsMidgaardInteriorCell(x, y, state.Map, state.Depth)
                || x == state.PlayerX && y == state.PlayerY
                || RoamingThreatAt(x, y) != null
                || IsRoamingThreatHomeCell(x, y))
            {
                return false;
            }

            ExplorationCellRole roles = ExplorationSurfaceRules.RolesAt(state.Map, x, y);
            bool path = ExplorationSurfaceRules.IsPath(roles);
            bool guidanceRoute = IsExploreGuidanceCell(x, y, guidanceCells);
            bool tutorialLane = ExplorationCharacterArtCatalog.IsNewGameTutorialLane(
                state.Depth,
                HasStoryFlag(StoryFlags.MidgaardRatQuestGiven),
                guidanceRoute);
            WorldZone zone = ZoneFor(x, y, state.Map, state.Depth);
            bool certifiedSafeRoad = path && (zone == null || zone.Danger <= 0);
            bool hasInteractable = ObjectAt(state.Map, x, y) != null;
            bool siteReserved = IsRegionalSiteCell(state.Map, x, y);
            bool midgaardCity = IsMidgaardCityCell(x, y, state.Map, state.Depth);
            string district = midgaardCity
                ? MidgaardDistrictRules.DistrictAtOffset(x - state.Map.StartX, y - state.Map.StartY)
                : zone?.Id ?? ZoneIdFor(x, y, state.Map, state.Depth);
            if (!ExplorationCharacterArtCatalog.ShouldPlaceAmbientCitizen(
                    district,
                    state.Seed,
                    x,
                    y,
                    roles,
                    tutorialLane,
                    certifiedSafeRoad,
                    guidanceRoute,
                    hasInteractable,
                    siteReserved))
            {
                return false;
            }

            int index = ExplorationCharacterArtCatalog.AmbientCitizenIndex(district, state.Seed, x, y);
            if (index < 0) return false;
            Rect citizenRect = Pad(cell, cell.width * (exploreWideView ? 0.20f : 0.07f));
            float alpha = exploreWideView ? 0.72f : 0.88f;
            WorldMapArtSpec spec = new WorldMapArtSpec(
                0.98f,
                new Vector2(0.5f, 1f),
                new Vector2(0f, 0.01f),
                true);
            bool drawn = TryDrawWorldNpcCitizenAtlasIcon(citizenRect, index, Color.white.WithAlpha(alpha), spec);
            if (drawn && showExploreArtDebug)
            {
                AmbientCitizenProfession profession = ExplorationCharacterArtCatalog.CitizenProfessionAt(index);
                DrawExploreArtDebugOverlay(cell, citizenRect, "Ambient: " + profession);
            }
            return drawn;
        }

        private bool TryDrawGrandHearthPatron(
            Rect cell,
            int x,
            int y,
            int tile,
            HashSet<int> guidanceCells)
        {
            if (state?.Map == null
                || state.Depth != 1
                || tile != 1
                || !IsWorldNpcCitizenAtlas()
                || x == state.PlayerX && y == state.PlayerY
                || ObjectAt(state.Map, x, y) != null
                || IsExploreGuidanceCell(x, y, guidanceCells)
                || !MidgaardInteriorRules.TryGrandHearthPatron(state.Map, x, y, out AmbientCitizenProfession profession))
            {
                return false;
            }

            int index = ExplorationCharacterArtCatalog.CitizenAtlasIndex(profession);
            if (index < 0) return false;
            Rect patronRect = Pad(cell, cell.width * (exploreWideView ? 0.18f : 0.05f));
            float alpha = exploreWideView ? 0.76f : 0.94f;
            WorldMapArtSpec spec = new WorldMapArtSpec(
                0.98f,
                new Vector2(0.5f, 1f),
                new Vector2(0f, 0.01f),
                true);
            bool drawn = TryDrawWorldNpcCitizenAtlasIcon(
                patronRect,
                index,
                Color.white.WithAlpha(alpha),
                spec);
            if (drawn && showExploreArtDebug)
            {
                DrawExploreArtDebugOverlay(cell, patronRect, "Town Hall patron: " + profession);
            }
            return drawn;
        }

        private bool IsRoamingThreatHomeCell(int x, int y)
        {
            if (state?.RoamingThreats == null) return false;
            foreach (RoamingThreat threat in state.RoamingThreats)
            {
                if (threat != null
                    && threat.Depth == state.Depth
                    && threat.HomeX == x
                    && threat.HomeY == y)
                {
                    return true;
                }
            }
            return false;
        }

        private HashSet<int> BuildCurrentExploreGuidanceCellSet()
        {
            IReadOnlyList<Point> path = CurrentExploreGuidancePath();
            HashSet<int> cells = new HashSet<int>();
            if (path == null || state?.Map == null) return cells;
            for (int i = 0; i < path.Count; i++)
            {
                Point step = path[i];
                if (step != null) cells.Add(ExploreGuidanceCellKey(step.X, step.Y));
            }
            return cells;
        }

        private bool IsExploreGuidanceCell(int x, int y, HashSet<int> guidanceCells)
        {
            return guidanceCells != null && guidanceCells.Contains(ExploreGuidanceCellKey(x, y));
        }

        private int ExploreGuidanceCellKey(int x, int y)
        {
            int width = state?.Map?.Width ?? V24WorldMapAtlasWidth;
            return y * width + x;
        }
    }
}
