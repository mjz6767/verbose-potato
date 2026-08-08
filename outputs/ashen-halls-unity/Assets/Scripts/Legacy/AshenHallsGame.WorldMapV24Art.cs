using System.Collections.Generic;
using System.Linq;
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
            if (state?.Map == null || state.RoamingThreats == null) return;
            bool habitatAtlasReady = IsWorldThreatHabitatAtlas();
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
                bool homeOccupied = threat.Active
                    && threat.X == threat.HomeX
                    && threat.Y == threat.HomeY;
                float size = cell * WorldThreatHabitatPresentationRules.MapScale(
                    exploreWideView,
                    threat.Active,
                    homeOccupied);
                Rect habitatRect = new Rect(
                    homeCell.center.x - size * 0.5f,
                    homeCell.yMax - size,
                    size,
                    size);

                RoamingThreatDefinition definition = RoamingThreatCatalog.Find(
                    threat.Id,
                    threat.Depth,
                    ContentSetCatalog.IsFullPrototype(activeContentSet));
                int index = WorldThreatHabitatPresentationRules.PresentationIndex(
                    threat.Active,
                    definition,
                    threat.Archetype);

                bool onObjectiveRoute = IsExploreGuidanceCell(threat.HomeX, threat.HomeY, guidanceCells);
                float alpha = WorldThreatHabitatPresentationRules.HabitatAlpha(
                    onObjectiveRoute,
                    threat.Active,
                    homeOccupied);
                DrawRoamingThreatHabitatFootprint(homeCell, threat.Active, onObjectiveRoute);
                WorldMapArtSpec spec = new WorldMapArtSpec(
                    0.98f,
                    new Vector2(
                        WorldThreatHabitatPresentationRules.BottomCenterPivotX,
                        WorldThreatHabitatPresentationRules.BottomCenterPivotY),
                    Vector2.zero,
                    true);
                // Preserve the authored world-space scale at viewport edges. A GUI
                // group clips spillover to the map instead of shrinking the whole
                // habitat into a misleading one-cell fallback.
                Rect clippedRect = new Rect(
                    habitatRect.x - grid.x,
                    habitatRect.y - grid.y,
                    habitatRect.width,
                    habitatRect.height);
                bool drawn;
                GUI.BeginGroup(grid);
                try
                {
                    drawn = TryDrawWorldThreatHabitatAtlasIcon(
                        clippedRect,
                        index,
                        Color.white.WithAlpha(alpha),
                        spec);
                    if (WorldThreatHabitatPresentationRules.ShouldDrawFallback(habitatAtlasReady, threat.Active))
                    {
                        DrawRoamingThreatHabitatFallback(clippedRect, onObjectiveRoute);
                        drawn = true;
                    }
                }
                finally
                {
                    GUI.EndGroup();
                }
                if (drawn && showExploreArtDebug)
                {
                    DrawExploreArtDebugOverlay(homeCell, habitatRect, "Habitat: " + threat.Name);
                }
            }
        }

        private void DrawRoamingThreatHabitatFallback(Rect habitatRect, bool onObjectiveRoute)
        {
            Color accent = onObjectiveRoute ? gold : blood;
            float width = Mathf.Max(10f, habitatRect.width * 0.52f);
            float height = Mathf.Max(10f, habitatRect.height * 0.44f);
            Rect shelter = new Rect(
                habitatRect.center.x - width * 0.5f,
                habitatRect.yMax - height * 1.02f,
                width,
                height);
            DrawRect(shelter, Hex("080706", 0.94f));
            DrawRect(
                new Rect(shelter.x - width * 0.08f, shelter.y, width * 1.16f, Mathf.Max(3f, height * 0.18f)),
                accent.WithAlpha(0.92f));
            DrawRect(
                new Rect(shelter.center.x - Mathf.Max(2f, width * 0.06f), shelter.y + height * 0.28f, Mathf.Max(4f, width * 0.12f), height * 0.48f),
                accent.WithAlpha(0.82f));
            DrawBorder(shelter, accent.WithAlpha(0.78f), 2);
        }

        private void DrawRoamingThreatHabitatFootprint(
            Rect homeCell,
            bool active,
            bool onObjectiveRoute)
        {
            Color accent = onObjectiveRoute ? gold : active ? blood : stone;
            Rect footprint = new Rect(
                homeCell.x + homeCell.width * 0.10f,
                homeCell.y + homeCell.height * 0.68f,
                homeCell.width * 0.80f,
                homeCell.height * 0.20f);
            if (active)
            {
                DrawRect(footprint, Hex("020303", 0.84f));
                DrawRect(
                    new Rect(
                        footprint.x + footprint.width * 0.18f,
                        footprint.yMax - Mathf.Max(2f, footprint.height * 0.18f),
                        footprint.width * 0.64f,
                        Mathf.Max(2f, footprint.height * 0.18f)),
                    accent.WithAlpha(0.72f));
                DrawBorder(footprint, accent.WithAlpha(0.44f), 1);
                return;
            }

            // Cleared aftermath is walkable: two low debris shadows deliberately
            // leave the middle open instead of drawing a solid collision slab.
            float segment = footprint.width * 0.28f;
            DrawRect(new Rect(footprint.x, footprint.y, segment, footprint.height * 0.62f), Hex("020303", 0.48f));
            DrawRect(new Rect(footprint.xMax - segment, footprint.y, segment, footprint.height * 0.62f), Hex("020303", 0.48f));
        }

        private bool TryDrawWorldExteriorAmbientCitizen(
            Rect cell,
            int x,
            int y,
            int tile,
            HashSet<int> guidanceCells)
        {
            if (!IsWorldNpcCitizenAtlas()
                || !TryGetWorldAmbientCitizenAt(
                    x,
                    y,
                    tile,
                    guidanceCells,
                    out AmbientCitizenProfession profession,
                    out string district))
            {
                return false;
            }

            int index = ExplorationCharacterArtCatalog.CitizenAtlasIndex(profession);
            if (index < 0) return false;
            bool yieldingToParty = ExplorationCharacterArtCatalog.ExteriorCitizenYieldsToParty(
                x,
                y,
                state.PlayerX,
                state.PlayerY);
            bool overlapsParty = x == state.PlayerX && y == state.PlayerY;
            Rect citizenRect = Pad(
                cell,
                cell.width * ExplorationCharacterArtCatalog.ExteriorCitizenPadding(exploreWideView));
            citizenRect.x += cell.width * ExplorationCharacterArtCatalog.ExteriorCitizenHorizontalOffsetInCells(
                district,
                state.Seed,
                x,
                y,
                state.PlayerX,
                state.PlayerY);
            citizenRect.y += cell.height * ExplorationCharacterArtCatalog.ExteriorCitizenVerticalOffsetInCells(
                x,
                y,
                state.PlayerX,
                state.PlayerY);
            float alpha = ExplorationCharacterArtCatalog.ExteriorCitizenAlpha(exploreWideView, yieldingToParty);
            if (overlapsParty) alpha *= 0.56f;
            WorldMapArtSpec spec = new WorldMapArtSpec(
                0.98f,
                new Vector2(0.5f, 1f),
                new Vector2(0f, 0.01f),
                true);
            bool drawn = TryDrawWorldNpcCitizenAtlasIcon(citizenRect, index, Color.white.WithAlpha(alpha), spec);
            if (drawn && showExploreArtDebug)
            {
                DrawExploreArtDebugOverlay(
                    cell,
                    citizenRect,
                    "Ambient: " + ExplorationCharacterArtCatalog.CitizenDisplayName(profession));
            }
            return drawn;
        }

        private bool TryGetWorldAmbientCitizenAt(
            int x,
            int y,
            int tile,
            HashSet<int> guidanceCells,
            out AmbientCitizenProfession profession,
            out string district)
        {
            profession = AmbientCitizenProfession.Unknown;
            district = "";
            if (!IsWorldNpcCitizenAtlas()
                || state?.Map == null
                || tile != 1
                || IsMidgaardInteriorCell(x, y, state.Map, state.Depth)
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
            district = midgaardCity
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
                district = "";
                return false;
            }

            profession = ExplorationCharacterArtCatalog.AmbientProfession(district, state.Seed, x, y);
            return ExplorationCharacterArtCatalog.CitizenAtlasIndex(profession) >= 0;
        }

        private bool TryDrawGrandHearthPatron(
            Rect cell,
            int x,
            int y,
            int tile,
            HashSet<int> guidanceCells)
        {
            if (!IsWorldNpcCitizenAtlas()
                || x == state.PlayerX && y == state.PlayerY
                || !TryGetGrandHearthPatronAt(x, y, tile, guidanceCells, out AmbientCitizenProfession profession))
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

        private bool TryGetGrandHearthPatronAt(
            int x,
            int y,
            int tile,
            HashSet<int> guidanceCells,
            out AmbientCitizenProfession profession)
        {
            profession = AmbientCitizenProfession.Unknown;
            return IsWorldNpcCitizenAtlas()
                && state?.Map != null
                && state.Depth == 1
                && tile == 1
                && ObjectAt(state.Map, x, y) == null
                && !IsExploreGuidanceCell(x, y, guidanceCells)
                && MidgaardInteriorRules.TryGrandHearthPatron(state.Map, x, y, out profession);
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

        private bool IsActiveRoamingThreatHabitatCell(int x, int y)
        {
            if (state?.RoamingThreats == null) return false;
            return state.RoamingThreats.Any(threat =>
                threat != null
                && threat.Active
                && threat.Depth == state.Depth
                && threat.HomeX == x
                && threat.HomeY == y);
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
