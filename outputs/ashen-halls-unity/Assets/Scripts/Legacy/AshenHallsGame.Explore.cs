using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private const string RegionalSiteIdPrefix = "regional-site:";
        private const string RegionalSiteDecorationIdPrefix = "regional-site-decor:";
        private readonly List<int> generatedObjectCandidateIndices = new List<int>(2048);

        private MapData GenerateMap(int depth, int seed)
        {
            System.Random mapRng = new System.Random(seed + depth * 6113);
            MapData map = new MapData
            {
                Width = ExploreW,
                Height = ExploreH,
                Depth = depth,
                Tiles = Enumerable.Repeat(0, ExploreW * ExploreH).ToList(),
                SurfaceMaterials = Enumerable.Repeat((int)ExplorationMaterial.NaturalGround, ExploreW * ExploreH).ToList(),
                SurfaceRoles = Enumerable.Repeat((int)ExplorationCellRole.None, ExploreW * ExploreH).ToList(),
                Objects = new List<MapObject>(),
                StartX = WorldMapGenerationRules.StartX(ExploreW),
                StartY = WorldMapGenerationRules.StartY(ExploreH)
            };

            InitializeWorldRegionSurfaces(map);

            int x = map.StartX;
            int y = map.StartY;
            SetTile(map, x, y, 1);
            int districts = WorldMapGenerationRules.DistrictCount(depth);
            List<Point> anchors = new List<Point> { new Point(x, y) };
            for (int d = 0; d < districts; d++)
            {
                Point anchor = new Point(mapRng.Next(4, map.Width - 4), mapRng.Next(3, map.Height - 3));
                anchors.Add(anchor);
                CarveRoad(map, x, y, anchor.X, anchor.Y, mapRng);
                x = anchor.X;
                y = anchor.Y;
                int steps = WorldMapGenerationRules.WanderSteps(depth, mapRng.Next(50));
                for (int i = 0; i < steps; i++)
                {
                    int dir = mapRng.Next(4);
                    if (dir == 0) x++; else if (dir == 1) x--; else if (dir == 2) y++; else y--;
                    x = Mathf.Clamp(x, 2, map.Width - 3);
                    y = Mathf.Clamp(y, 2, map.Height - 3);
                    SetExploreTile(map, x, y, 1, ExplorationCellRole.None);
                    if (mapRng.NextDouble() < 0.27)
                    {
                        int radius = mapRng.NextDouble() < 0.3 ? 2 : 1;
                        CarveRoom(map, x, y, radius, mapRng);
                    }
                }
            }
            CarveRoad(map, map.StartX, map.StartY, anchors.Last().X, anchors.Last().Y, mapRng);
            int loopRoads = WorldMapGenerationRules.LoopRoadCount(depth);
            for (int i = 0; i < loopRoads && anchors.Count > 2; i++)
            {
                Point a = anchors[mapRng.Next(anchors.Count)];
                Point b = anchors[mapRng.Next(anchors.Count)];
                if (Distance(a.X, a.Y, b.X, b.Y) < 8) continue;
                CarveRoad(map, a.X, a.Y, b.X, b.Y, mapRng);
                if (mapRng.NextDouble() < 0.65)
                {
                    int cx = Mathf.Clamp((a.X + b.X) / 2 + mapRng.Next(-2, 3), 2, map.Width - 3);
                    int cy = Mathf.Clamp((a.Y + b.Y) / 2 + mapRng.Next(-2, 3), 2, map.Height - 3);
                    CarveRoom(map, cx, cy, mapRng.NextDouble() < 0.35 ? 3 : 2, mapRng);
                }
            }

            CarveRegionalRouteCircuit(map, mapRng);
            CarveRegionalSites(map, mapRng);
            FinalizeWorldRegionSurfaces(map);
            List<Point> open = new List<Point>();
            for (int yy = 1; yy < map.Height - 1; yy++)
            for (int xx = 1; xx < map.Width - 1; xx++)
            {
                if (TileAt(map, xx, yy) == 1 && Distance(xx, yy, map.StartX, map.StartY) > 3) open.Add(new Point(xx, yy));
            }
            PlaceRegionalSiteLandmarks(map, open);

            PlaceObjectsInZone(map, open, mapRng, "salt-cisterns", ObjectType.Encounter, 2 + depth / 2);
            PlaceObjectsInZone(map, open, mapRng, "salt-cisterns", ObjectType.Stairs, depth == 1 ? 1 : 0);
            PlaceObjectsInZone(map, open, mapRng, "green-shrine-road", ObjectType.Shrine, 2);
            PlaceObjectsInZone(map, open, mapRng, "green-shrine-road", ObjectType.Camp, 1);
            PlaceObjectsInZone(map, open, mapRng, "old-quarry", ObjectType.Cache, 2);
            PlaceObjectsInZone(map, open, mapRng, "old-quarry", ObjectType.Ruin, 2);
            PlaceObjectsInZone(map, open, mapRng, "dusk-market", ObjectType.Cache, 2);
            PlaceObjectsInZone(map, open, mapRng, "dusk-market", ObjectType.Cave, 1);
            PlaceObjectsInZone(map, open, mapRng, "glass-warrens", ObjectType.Obelisk, 1);
            PlaceObjectsInZone(map, open, mapRng, "glass-warrens", ObjectType.Encounter, 2);
            PlaceObjectsInZone(map, open, mapRng, "ash-fen", ObjectType.Shrine, 1);
            PlaceObjectsInZone(map, open, mapRng, "ash-fen", ObjectType.Encounter, 2);
            PlaceObjectsInZone(map, open, mapRng, "red-gate", ObjectType.Encounter, 2 + depth / 2);
            PlaceObjectsInZone(map, open, mapRng, "red-gate", ObjectType.Ruin, 1 + depth / 2);
            PlaceObjectsInZone(map, open, mapRng, "red-gate", ObjectType.Obelisk, 1);
            PlaceRouteScaffoldObjects(map, open, mapRng, depth);

            PlaceObjects(map, open, mapRng, ObjectType.Cache, 6 + Mathf.Min(depth * 2, 7));
            PlaceObjects(map, open, mapRng, ObjectType.Shrine, 2 + depth / 2);
            PlaceObjects(map, open, mapRng, ObjectType.Encounter, 7 + depth * 2);
            PlaceObjects(map, open, mapRng, ObjectType.Stairs, 1);
            PlaceObjects(map, open, mapRng, ObjectType.Camp, 2 + depth / 2);
            PlaceObjects(map, open, mapRng, ObjectType.Ruin, 5 + Mathf.Min(depth, 4));
            PlaceObjects(map, open, mapRng, ObjectType.Obelisk, 2 + depth / 2);
            PlaceObjects(map, open, mapRng, ObjectType.Cave, 2 + Mathf.Min(depth / 2, 2));
            PlaceObjects(map, open, mapRng, ObjectType.Bridge, 2 + Mathf.Min(depth / 2, 2));
            EnsureMidgaardStartZone(map);
            PruneDisabledGeneratedObjects(map);
            RepairUnsafeGeneratedBlockers(map);
            CertifyRegionalRouteCircuit(map);
            return map;
        }

        private void PlaceRouteScaffoldObjects(MapData map, List<Point> open, System.Random mapRng, int depth)
        {
            if (!ContentSetCatalog.ShowPrototypeScaffold(activeContentSet)) return;
            if (map == null || open == null || mapRng == null) return;
            foreach (RouteScaffoldDef def in RouteScaffoldDefs())
            {
                if (def == null || depth < def.MinDepth || string.IsNullOrEmpty(def.ZoneId)) continue;
                if (def.Type == ObjectType.QuestBoard) continue;
                PlaceObjectsInZone(map, open, mapRng, def.ZoneId, def.Type, 1);
            }
        }

        private void CarveRoad(MapData map, int ax, int ay, int bx, int by, System.Random mapRng)
        {
            int x = ax;
            int y = ay;
            int guard = 0;
            while ((x != bx || y != by) && guard++ < map.Width + map.Height)
            {
                CarveWorldRoadCell(map, x, y, ExplorationCellRole.Road);
                if (x != bx && (y == by || mapRng.NextDouble() < 0.55)) x += Math.Sign(bx - x);
                else if (y != by) y += Math.Sign(by - y);
                CarveWorldRoadCell(map, x, y, ExplorationCellRole.Road);
                if (mapRng.NextDouble() < 0.45)
                {
                    CarveWorldRoadCell(map, x + 1, y, ExplorationCellRole.Trail);
                    CarveWorldRoadCell(map, x, y + 1, ExplorationCellRole.Trail);
                }
            }
        }

        private void CarveWorldRoadCell(MapData map, int x, int y, ExplorationCellRole role)
        {
            if (map == null) return;
            x = Mathf.Clamp(x, 1, map.Width - 2);
            y = Mathf.Clamp(y, 1, map.Height - 2);
            if (MidgaardInteriorRules.IsReservedCell(map, x, y)) return;
            SetExploreTile(map, x, y, 1, role);
        }

        private void CarveRegionalRouteCircuit(MapData map, System.Random mapRng)
        {
            if (map == null || mapRng == null || map.Width < 12 || map.Height < 12) return;

            WorldMapJunction[] junctions = WorldMapGenerationRules.RegionalJunctions(map.Width, map.Height, map.StartX, map.StartY);
            Point[] circuit = junctions.Select(junction => new Point(junction.X, junction.Y)).ToArray();

            for (int i = 0; i < circuit.Length; i++)
            {
                Point from = circuit[i];
                Point to = circuit[(i + 1) % circuit.Length];
                CarveRoad(map, from.X, from.Y, to.X, to.Y, mapRng);
                CarveRoom(map, from.X, from.Y, 1, mapRng);
                SetExploreTile(map, from.X, from.Y, 1, ExplorationCellRole.Road | ExplorationCellRole.Clearing);
            }

            if (map.Depth == 1)
            {
                Point west = circuit[0];
                Point east = circuit[4];
                Point westEntry = new Point(2, Mathf.Clamp(map.StartY - 2, 2, map.Height - 3));
                Point eastEntry = new Point(map.Width - 3, Mathf.Clamp(map.StartY + 2, 2, map.Height - 3));
                CarveRoad(map, westEntry.X, westEntry.Y, west.X, west.Y, mapRng);
                CarveRoad(map, eastEntry.X, eastEntry.Y, east.X, east.Y, mapRng);
            }
            else
            {
                Point west = circuit[0];
                Point north = circuit[2];
                Point east = circuit[4];
                Point south = circuit[6];
                CarveRoad(map, map.StartX, map.StartY, west.X, west.Y, mapRng);
                CarveRoad(map, map.StartX, map.StartY, north.X, north.Y, mapRng);
                CarveRoad(map, map.StartX, map.StartY, east.X, east.Y, mapRng);
                CarveRoad(map, map.StartX, map.StartY, south.X, south.Y, mapRng);
            }
        }

        private void CarveRegionalSites(MapData map, System.Random mapRng)
        {
            if (map == null || mapRng == null) return;
            WorldMapJunction[] junctions = WorldMapGenerationRules.RegionalJunctions(
                map.Width,
                map.Height,
                map.StartX,
                map.StartY);
            foreach (WorldMapSite site in WorldMapGenerationRules.RegionalSites(
                map.Width,
                map.Height,
                map.StartX,
                map.StartY))
            {
                bool hasTemplate = WorldAreaTemplateRules.TryGet(site.Id, site.ZoneId, out WorldAreaTemplate template);
                WorldMapJunction route = junctions.FirstOrDefault(candidate =>
                    string.Equals(candidate.ZoneId, site.ZoneId, StringComparison.Ordinal));
                if (!string.IsNullOrEmpty(route.Id))
                {
                    int approachX = hasTemplate ? site.X + template.ApproachOffsetX : site.X;
                    int approachY = hasTemplate ? site.Y + template.ApproachOffsetY : site.Y;
                    CarveRoad(map, route.X, route.Y, approachX, approachY, mapRng);
                }

                if (hasTemplate)
                {
                    foreach (WorldAreaCellTemplate cell in template.Cells)
                    {
                        int x = site.X + cell.OffsetX;
                        int y = site.Y + cell.OffsetY;
                        if (x < 1 || y < 1 || x >= map.Width - 1 || y >= map.Height - 1) continue;
                        if (MidgaardInteriorRules.IsReservedCell(map, x, y)) continue;
                        SetExploreCell(map, x, y, cell.Open ? 1 : 0, cell.Material, cell.Roles);
                    }
                    continue;
                }

                ExplorationMaterial material = OpenMaterialForZone(site.ZoneId);
                for (int y = site.Y - site.Radius; y <= site.Y + site.Radius; y++)
                for (int x = site.X - site.Radius; x <= site.X + site.Radius; x++)
                {
                    if (x < 1 || y < 1 || x >= map.Width - 1 || y >= map.Height - 1) continue;
                    if (Mathf.Abs(x - site.X) == site.Radius
                        && Mathf.Abs(y - site.Y) == site.Radius)
                    {
                        continue;
                    }
                    if (MidgaardInteriorRules.IsReservedCell(map, x, y)) continue;

                    ExplorationCellRole roles = ExplorationCellRole.Room | ExplorationCellRole.Clearing;
                    if (x == site.X && y == site.Y) roles |= ExplorationCellRole.Threshold;
                    SetExploreTile(map, x, y, 1, roles);
                    ExplorationSurfaceRules.SetMaterial(map, x, y, material);
                }
            }
        }

        private void PlaceRegionalSiteLandmarks(MapData map, List<Point> open)
        {
            if (map?.Objects == null) return;
            foreach (WorldMapSite site in WorldMapGenerationRules.RegionalSites(
                map.Width,
                map.Height,
                map.StartX,
                map.StartY))
            {
                bool addedObject = false;
                string objectId = RegionalSiteObjectId(site);
                if (map.FindObjectById(objectId) == null && ObjectAt(map, site.X, site.Y) == null)
                {
                    MapObject landmark = new MapObject(site.X, site.Y, site.Type, objectId);
                    map.Objects.Add(landmark);
                    ApplyMapObjectSurface(map, landmark);
                    addedObject = true;
                }

                if (WorldAreaTemplateRules.TryGet(site.Id, site.ZoneId, out WorldAreaTemplate template))
                {
                    foreach (WorldAreaObjectTemplate decoration in template.Objects)
                    {
                        int x = site.X + decoration.OffsetX;
                        int y = site.Y + decoration.OffsetY;
                        string decorationId = RegionalSiteDecorationObjectId(site, decoration);
                        if (x < 1 || y < 1 || x >= map.Width - 1 || y >= map.Height - 1) continue;
                        if (MidgaardInteriorRules.IsReservedCell(map, x, y)) continue;
                        if (!template.TryCell(decoration.OffsetX, decoration.OffsetY, out WorldAreaCellTemplate cell)
                            || !cell.Open
                            || map.FindObjectById(decorationId) != null
                            || ObjectAt(map, x, y) != null)
                        {
                            continue;
                        }

                        MapObject prop = new MapObject(x, y, decoration.Type, decorationId);
                        map.Objects.Add(prop);
                        ApplyMapObjectSurface(map, prop);
                        addedObject = true;
                    }
                }

                if (addedObject) map.InvalidateObjectLookup();

                open?.RemoveAll(point =>
                    Mathf.Abs(point.X - site.X) <= site.Radius
                    && Mathf.Abs(point.Y - site.Y) <= site.Radius);
            }
        }

        private string RegionalSiteObjectId(WorldMapSite site)
        {
            return RegionalSiteIdPrefix + site.Id;
        }

        private string RegionalSiteDecorationObjectId(
            WorldMapSite site,
            WorldAreaObjectTemplate decoration)
        {
            return RegionalSiteDecorationIdPrefix + site.Id + ":" + decoration.Key;
        }

        private bool IsRegionalSiteDecoration(MapData map, MapObject obj)
        {
            if (map == null
                || obj == null
                || string.IsNullOrEmpty(obj.Id)
                || !obj.Id.StartsWith(RegionalSiteDecorationIdPrefix, StringComparison.Ordinal))
            {
                return false;
            }

            foreach (WorldMapSite site in WorldMapGenerationRules.RegionalSites(
                map.Width,
                map.Height,
                map.StartX,
                map.StartY))
            {
                if (!WorldAreaTemplateRules.TryGet(site.Id, site.ZoneId, out WorldAreaTemplate template)) continue;
                foreach (WorldAreaObjectTemplate decoration in template.Objects)
                {
                    if (!string.Equals(
                            obj.Id,
                            RegionalSiteDecorationObjectId(site, decoration),
                            StringComparison.Ordinal))
                    {
                        continue;
                    }

                    return obj.Type == decoration.Type
                        && obj.X == site.X + decoration.OffsetX
                        && obj.Y == site.Y + decoration.OffsetY;
                }
            }
            return false;
        }

        private bool TryRegionalSite(MapData map, MapObject obj, out WorldMapSite site)
        {
            site = default;
            if (map == null
                || obj == null
                || string.IsNullOrEmpty(obj.Id)
                || !obj.Id.StartsWith(RegionalSiteIdPrefix, StringComparison.Ordinal))
            {
                return false;
            }

            foreach (WorldMapSite candidate in WorldMapGenerationRules.RegionalSites(
                map.Width,
                map.Height,
                map.StartX,
                map.StartY))
            {
                if (!string.Equals(obj.Id, RegionalSiteObjectId(candidate), StringComparison.Ordinal)) continue;
                site = candidate;
                return true;
            }
            return false;
        }

        private bool TryRegionalSiteAt(MapData map, int x, int y, out WorldMapSite site)
        {
            site = default;
            if (map == null) return false;

            bool found = false;
            int bestDistance = int.MaxValue;
            foreach (WorldMapSite candidate in WorldMapGenerationRules.RegionalSites(
                map.Width,
                map.Height,
                map.StartX,
                map.StartY))
            {
                int dx = Mathf.Abs(x - candidate.X);
                int dy = Mathf.Abs(y - candidate.Y);
                if (Mathf.Max(dx, dy) > candidate.Radius) continue;
                int distance = dx + dy;
                if (found && distance >= bestDistance) continue;
                site = candidate;
                bestDistance = distance;
                found = true;
            }
            return found;
        }

        private bool IsRegionalSiteCell(MapData map, int x, int y)
        {
            if (!UsesRegionalSiteLayout(map))
            {
                return false;
            }

            foreach (WorldMapSite site in WorldMapGenerationRules.RegionalSites(
                map.Width,
                map.Height,
                map.StartX,
                map.StartY))
            {
                if (Mathf.Abs(x - site.X) <= site.Radius
                    && Mathf.Abs(y - site.Y) <= site.Radius)
                {
                    return true;
                }
            }
            return false;
        }

        private bool UsesRegionalSiteLayout(MapData map)
        {
            return map != null
                && map.Width == WorldMapGenerationRules.Width
                && map.Height == WorldMapGenerationRules.Height;
        }

        private void CertifyRegionalRouteCircuit(MapData map)
        {
            if (map?.Tiles == null || map.Width < 12 || map.Height < 12) return;
            Point anchor = FindCriticalRouteAnchor(map);
            if (anchor == null) return;

            bool[,] reachable = ExplorationTraversalRules.ReachableMask(map, anchor.X, anchor.Y);
            foreach (WorldMapJunction junction in WorldMapGenerationRules.RegionalJunctions(
                map.Width,
                map.Height,
                map.StartX,
                map.StartY))
            {
                SetExploreTile(
                    map,
                    junction.X,
                    junction.Y,
                    1,
                    ExplorationCellRole.Road | ExplorationCellRole.Clearing);
                if (reachable[junction.X, junction.Y]) continue;

                Point connectorStart = NearestReachableExplorePoint(reachable, junction.X, junction.Y);
                if (connectorStart == null) return;
                CarveRegionalRouteConnector(map, connectorStart, new Point(junction.X, junction.Y));
                reachable = ExplorationTraversalRules.ReachableMask(map, anchor.X, anchor.Y);
            }

            WorldMapSite[] regionalSites = UsesRegionalSiteLayout(map)
                ? WorldMapGenerationRules.RegionalSites(map.Width, map.Height, map.StartX, map.StartY)
                : Array.Empty<WorldMapSite>();
            foreach (WorldMapSite site in regionalSites)
            {
                Point approach = RegionalSiteApproach(map, site);
                if (approach == null) continue;
                SetExploreTile(
                    map,
                    approach.X,
                    approach.Y,
                    1,
                    ExplorationCellRole.Road | ExplorationCellRole.Clearing | ExplorationCellRole.Room);
                if (reachable[approach.X, approach.Y]) continue;

                Point connectorStart = NearestReachableExplorePoint(reachable, approach.X, approach.Y);
                if (connectorStart == null) return;
                CarveRegionalRouteConnector(map, connectorStart, approach);
                reachable = ExplorationTraversalRules.ReachableMask(map, anchor.X, anchor.Y);
            }
        }

        private Point RegionalSiteApproach(MapData map, WorldMapSite site)
        {
            if (map == null) return null;
            MapObject landmark = map.FindObjectById(RegionalSiteObjectId(site));
            if (landmark == null || ExplorationTraversalRules.CanStandOnObject(landmark))
            {
                return new Point(site.X, site.Y);
            }

            if (WorldAreaTemplateRules.TryGet(site.Id, site.ZoneId, out WorldAreaTemplate template))
            {
                Point authoredApproach = new Point(
                    site.X + Math.Sign(template.ApproachOffsetX),
                    site.Y + Math.Sign(template.ApproachOffsetY));
                if (authoredApproach.X > 0
                    && authoredApproach.Y > 0
                    && authoredApproach.X < map.Width - 1
                    && authoredApproach.Y < map.Height - 1
                    && ExplorationTraversalRules.CanStandOnObject(ObjectAt(map, authoredApproach.X, authoredApproach.Y)))
                {
                    return authoredApproach;
                }
            }

            Point[] approaches =
            {
                new Point(site.X, site.Y + 1),
                new Point(site.X - 1, site.Y),
                new Point(site.X + 1, site.Y),
                new Point(site.X, site.Y - 1)
            };
            return approaches.FirstOrDefault(point =>
                point.X > 0
                && point.Y > 0
                && point.X < map.Width - 1
                && point.Y < map.Height - 1
                && ExplorationTraversalRules.CanStandOnObject(ObjectAt(map, point.X, point.Y)));
        }

        private void CarveRegionalRouteConnector(MapData map, Point from, Point to)
        {
            if (map == null || from == null || to == null) return;
            List<Point> path = FindRegionalRouteConnectorPath(map, from, to);
            if (path == null || path.Count == 0) return;
            foreach (Point point in path)
            {
                OpenRegionalRouteConnectorCell(map, point.X, point.Y);
            }
        }

        private List<Point> FindRegionalRouteConnectorPath(MapData map, Point from, Point to)
        {
            if (map == null || from == null || to == null) return null;
            if (from.X < 1 || from.Y < 1 || from.X >= map.Width - 1 || from.Y >= map.Height - 1) return null;
            if (to.X < 1 || to.Y < 1 || to.X >= map.Width - 1 || to.Y >= map.Height - 1) return null;

            int cellCount = map.Width * map.Height;
            int[] parent = Enumerable.Repeat(-2, cellCount).ToArray();
            Queue<int> frontier = new Queue<int>();
            int start = from.Y * map.Width + from.X;
            int goal = to.Y * map.Width + to.X;
            parent[start] = -1;
            frontier.Enqueue(start);

            int towardX = Math.Sign(to.X - from.X);
            int towardY = Math.Sign(to.Y - from.Y);
            List<Point> directions = new List<Point>(4);
            if (towardX != 0) directions.Add(new Point(towardX, 0));
            if (towardY != 0) directions.Add(new Point(0, towardY));
            if (towardX != 0) directions.Add(new Point(-towardX, 0));
            if (towardY != 0) directions.Add(new Point(0, -towardY));
            if (towardX == 0)
            {
                directions.Add(new Point(1, 0));
                directions.Add(new Point(-1, 0));
            }
            if (towardY == 0)
            {
                directions.Add(new Point(0, 1));
                directions.Add(new Point(0, -1));
            }

            while (frontier.Count > 0 && parent[goal] == -2)
            {
                int current = frontier.Dequeue();
                int x = current % map.Width;
                int y = current / map.Width;
                foreach (Point direction in directions)
                {
                    int nx = x + direction.X;
                    int ny = y + direction.Y;
                    if (nx < 1 || ny < 1 || nx >= map.Width - 1 || ny >= map.Height - 1) continue;
                    int next = ny * map.Width + nx;
                    if (parent[next] != -2 || !CanCarveRegionalRouteCell(map, nx, ny)) continue;
                    parent[next] = current;
                    frontier.Enqueue(next);
                }
            }

            if (parent[goal] == -2) return null;
            List<Point> path = new List<Point>();
            for (int cursor = goal; cursor >= 0; cursor = parent[cursor])
            {
                path.Add(new Point(cursor % map.Width, cursor / map.Width));
            }
            path.Reverse();
            return path;
        }

        private bool CanCarveRegionalRouteCell(MapData map, int x, int y)
        {
            if (map == null
                || IsMidgaardCityCell(x, y, map, map.Depth)
                || MidgaardInteriorRules.IsReservedCell(map, x, y))
            {
                return false;
            }
            MapObject blocker = ObjectAt(map, x, y);
            if (blocker == null || !ExplorationTraversalRules.BlocksMovement(blocker)) return true;
            if (IsRegionalSiteDecoration(map, blocker)) return false;
            return !IsRouteScaffoldObject(blocker.Type);
        }

        private void OpenRegionalRouteConnectorCell(MapData map, int x, int y)
        {
            if (x < 1 || y < 1 || x >= map.Width - 1 || y >= map.Height - 1) return;
            if (IsMidgaardCityCell(x, y, map, map.Depth)
                || MidgaardInteriorRules.IsReservedCell(map, x, y))
            {
                return;
            }
            OpenExploreConnectorSurface(map, x, y, ExplorationCellRole.Road);
            MapObject blocker = ObjectAt(map, x, y);
            if (blocker == null || !ExplorationTraversalRules.BlocksMovement(blocker)) return;
            if (IsRegionalSiteDecoration(map, blocker)) return;
            if (IsMidgaardCityCell(x, y, map, map.Depth) || IsRouteScaffoldObject(blocker.Type)) return;
            map.Objects.Remove(blocker);
        }

        private void CarveRoom(MapData map, int cx, int cy, int radius, System.Random mapRng)
        {
            for (int yy = -radius; yy <= radius; yy++)
            for (int xx = -radius; xx <= radius; xx++)
            {
                if (Mathf.Abs(xx) + Mathf.Abs(yy) <= radius + 1 && mapRng.NextDouble() < 0.82)
                {
                    int x = Mathf.Clamp(cx + xx, 1, map.Width - 2);
                    int y = Mathf.Clamp(cy + yy, 1, map.Height - 2);
                    if (MidgaardInteriorRules.IsReservedCell(map, x, y)) continue;
                    SetExploreTile(map, x, y, 1, ExplorationCellRole.Room);
                }
            }
        }

        private void SetExploreTile(MapData map, int x, int y, int tile, ExplorationCellRole roles)
        {
            SetTile(map, x, y, tile);
            if (map == null || roles == ExplorationCellRole.None) return;

            ExplorationCellRole merged = ExplorationSurfaceRules.RolesAt(map, x, y);
            if ((roles & ExplorationCellRole.Road) != 0)
            {
                merged &= ~ExplorationCellRole.Trail;
            }
            else if ((roles & ExplorationCellRole.Trail) != 0 && (merged & ExplorationCellRole.Road) != 0)
            {
                roles &= ~ExplorationCellRole.Trail;
            }
            ExplorationSurfaceRules.SetRoles(map, x, y, merged | roles);
        }

        private void SetExploreCell(MapData map, int x, int y, int tile, ExplorationMaterial material, ExplorationCellRole roles)
        {
            SetTile(map, x, y, tile);
            ExplorationSurfaceRules.SetMaterial(map, x, y, material);
            ExplorationSurfaceRules.SetRoles(map, x, y, roles);
        }

        private void InitializeWorldRegionSurfaces(MapData map)
        {
            if (map == null) return;
            ExplorationSurfaceRules.EnsureGrid(map);
            for (int y = 0; y < map.Height; y++)
            for (int x = 0; x < map.Width; x++)
            {
                string zoneId = ExploreVisualZoneId(x, y, map, map.Depth);
                ExplorationSurfaceRules.SetMaterial(map, x, y, OpenMaterialForZone(zoneId));
                ExplorationSurfaceRules.SetRoles(map, x, y, ExplorationCellRole.None);
            }
        }

        private void FinalizeWorldRegionSurfaces(MapData map)
        {
            if (map == null) return;
            ExplorationSurfaceRules.EnsureGrid(map);
            for (int y = 0; y < map.Height; y++)
            for (int x = 0; x < map.Width; x++)
            {
                string zoneId = ExploreVisualZoneId(x, y, map, map.Depth);
                if (TileAt(map, x, y) == 0)
                {
                    ExplorationSurfaceRules.SetMaterial(map, x, y, BlockedMaterialForZone(zoneId));
                    continue;
                }

                ExplorationCellRole roles = ExplorationSurfaceRules.RolesAt(map, x, y);
                if (ExplorationSurfaceRules.IsPath(roles) || (roles & ExplorationCellRole.Room) != 0) continue;
                bool shallowWetland = (zoneId == "ash-fen" && MapSurfaceNoise(map, x, y, 43) % 7 == 0)
                    || (zoneId == "salt-cisterns" && MapSurfaceNoise(map, x, y, 71) % 9 == 0);
                if (!shallowWetland) continue;
                ExplorationSurfaceRules.SetMaterial(map, x, y, ExplorationMaterial.ShallowWater);
                ExplorationSurfaceRules.AddRoles(map, x, y, ExplorationCellRole.Water);
            }
        }

        private ExplorationMaterial OpenMaterialForZone(string zoneId)
        {
            switch (zoneId ?? "")
            {
                case "midgaard-road": return ExplorationMaterial.PackedDirt;
                case "old-quarry": return ExplorationMaterial.QuarryStone;
                case "glass-warrens": return ExplorationMaterial.GlassRubble;
                case "ash-fen": return ExplorationMaterial.FenMud;
                case "red-gate": return ExplorationMaterial.RedAsh;
                case "gloam-courts": return ExplorationMaterial.GloamStone;
                case "salt-cisterns": return ExplorationMaterial.CisternBrick;
                case "green-shrine-road": return ExplorationMaterial.Moss;
                case "dusk-market": return ExplorationMaterial.RuinedPaving;
                default: return ExplorationMaterial.NaturalGround;
            }
        }

        private ExplorationMaterial BlockedMaterialForZone(string zoneId)
        {
            switch (zoneId ?? "")
            {
                case "old-quarry":
                case "glass-warrens": return ExplorationMaterial.Cliff;
                case "ash-fen":
                case "salt-cisterns": return ExplorationMaterial.DeepWater;
                case "red-gate": return ExplorationMaterial.RedBasalt;
                case "green-shrine-road":
                case "gloam-courts": return ExplorationMaterial.Forest;
                case "dusk-market": return ExplorationMaterial.RuinedWall;
                default: return ExplorationMaterial.Cliff;
            }
        }

        private int MapSurfaceNoise(MapData map, int x, int y, int salt)
        {
            unchecked
            {
                int value = (map?.Depth ?? 1) * 8191 + x * 92821 + y * 68917 + salt * 19333;
                value ^= value << 13;
                value ^= value >> 17;
                value ^= value << 5;
                return value & 0x7fffffff;
            }
        }

        private void PlaceObjects(MapData map, List<Point> open, System.Random mapRng, ObjectType type, int count)
        {
            for (int i = 0; i < count && open.Count > 0; i++)
            {
                List<int> candidates = generatedObjectCandidateIndices;
                candidates.Clear();
                for (int n = 0; n < open.Count; n++)
                {
                    Point candidate = open[n];
                    if (ObjectAt(map, candidate.X, candidate.Y) == null && CanPlaceGeneratedExploreObject(map, candidate.X, candidate.Y, type)) candidates.Add(n);
                }
                if (candidates.Count == 0) return;
                int index = candidates[mapRng.Next(candidates.Count)];
                Point point = open[index];
                open.RemoveAt(index);
                if (ObjectAt(map, point.X, point.Y) == null)
                {
                    MapObject placed = new MapObject(point.X, point.Y, type);
                    map.Objects.Add(placed);
                    ApplyMapObjectSurface(map, placed);
                }
            }
        }

        private void PlaceObjectsInZone(MapData map, List<Point> open, System.Random mapRng, string zoneId, ObjectType type, int count)
        {
            if (count <= 0 || map == null || open == null || open.Count == 0) return;
            for (int i = 0; i < count; i++)
            {
                List<int> candidates = generatedObjectCandidateIndices;
                candidates.Clear();
                for (int n = 0; n < open.Count; n++)
                {
                    Point point = open[n];
                    if (ZoneIdFor(point.X, point.Y, map, map.Depth) == zoneId
                        && ObjectAt(map, point.X, point.Y) == null
                        && CanPlaceGeneratedExploreObject(map, point.X, point.Y, type))
                    {
                        candidates.Add(n);
                    }
                }
                if (candidates.Count == 0) return;
                int openIndex = candidates[mapRng.Next(candidates.Count)];
                Point chosen = open[openIndex];
                open.RemoveAt(openIndex);
                MapObject placed = new MapObject(chosen.X, chosen.Y, type);
                map.Objects.Add(placed);
                ApplyMapObjectSurface(map, placed);
            }
        }

        private void ApplyMapObjectSurface(MapData map, MapObject obj)
        {
            if (map == null || obj == null || TileAt(map, obj.X, obj.Y) != 1) return;
            if (obj.Type == ObjectType.Bridge)
            {
                ExplorationSurfaceRules.SetMaterial(map, obj.X, obj.Y, ExplorationMaterial.BridgeDeck);
                ExplorationSurfaceRules.AddRoles(map, obj.X, obj.Y, ExplorationCellRole.Road | ExplorationCellRole.Bridge);
            }
        }

        private bool CanPlaceGeneratedExploreObject(MapData map, int x, int y, ObjectType type)
        {
            if (map == null || TileAt(map, x, y) != 1) return false;
            if (MidgaardInteriorRules.IsReservedCell(map, x, y)) return false;
            if (IsRegionalSiteCell(map, x, y)) return false;
            if (WorldMapGenerationRules.TryFindRegionalJunction(map.Width, map.Height, map.StartX, map.StartY, x, y, 1, out _)) return false;
            if (ExplorationTraversalRules.CanStandOnObject(type)) return true;

            int openNeighbors = 0;
            if (ExplorationTraversalRules.IsStandable(map, x, y - 1)) openNeighbors++;
            if (ExplorationTraversalRules.IsStandable(map, x, y + 1)) openNeighbors++;
            if (ExplorationTraversalRules.IsStandable(map, x - 1, y)) openNeighbors++;
            if (ExplorationTraversalRules.IsStandable(map, x + 1, y)) openNeighbors++;
            return openNeighbors >= 3;
        }

        private void PruneDisabledGeneratedObjects(MapData map)
        {
            if (map?.Objects == null || !ContentSetCatalog.IsSewerSlice(activeContentSet)) return;
            map.Objects.RemoveAll(obj => obj != null
                && (obj.Type == ObjectType.Encounter
                    || obj.Type == ObjectType.Stairs && !string.Equals(obj.Id, OldRoadDescentId, StringComparison.Ordinal)));
            map.InvalidateObjectLookup();
        }

        private void RepairUnsafeGeneratedBlockers(MapData map)
        {
            if (map?.Objects == null) return;
            bool movedAny = false;
            foreach (MapObject obj in map.Objects.ToList())
            {
                if (obj == null || ExplorationTraversalRules.CanStandOnObject(obj)) continue;
                if (IsMidgaardCityCell(obj.X, obj.Y, map, map.Depth)) continue;
                if (IsMidgaardInteriorCell(obj.X, obj.Y, map, map.Depth)) continue;
                if (TryRegionalSite(map, obj, out _)) continue;
                if (IsRegionalSiteDecoration(map, obj)) continue;
                if (CanPlaceGeneratedExploreObject(map, obj.X, obj.Y, obj.Type)) continue;

                string zoneId = ZoneIdFor(obj.X, obj.Y, map, map.Depth);
                Point replacement = FindSafeGeneratedObjectTile(map, obj, zoneId)
                    ?? FindSafeGeneratedObjectTile(map, obj, "");
                if (replacement == null) continue;
                obj.X = replacement.X;
                obj.Y = replacement.Y;
                movedAny = true;
            }
            if (movedAny) map.InvalidateObjectLookup();
        }

        private Point FindSafeGeneratedObjectTile(MapData map, MapObject moving, string requiredZoneId)
        {
            if (map == null || moving == null) return null;
            Point best = null;
            int bestScore = int.MaxValue;
            for (int y = 1; y < map.Height - 1; y++)
            for (int x = 1; x < map.Width - 1; x++)
            {
                if (ObjectAt(map, x, y) != null) continue;
                if (IsMidgaardCityCell(x, y, map, map.Depth)) continue;
                if (!string.IsNullOrEmpty(requiredZoneId) && ZoneIdFor(x, y, map, map.Depth) != requiredZoneId) continue;
                if (!CanPlaceGeneratedExploreObject(map, x, y, moving.Type)) continue;
                int score = Distance(x, y, moving.X, moving.Y) * 100 + y * map.Width + x;
                if (score >= bestScore) continue;
                bestScore = score;
                best = new Point(x, y);
            }
            return best;
        }

        private void EnsureMidgaardStartZone(MapData map)
        {
            if (map == null || map.Depth != 1) return;
            if (map.Objects == null) map.Objects = new List<MapObject>();

            int sx = map.StartX;
            int sy = map.StartY;
            int left = MidgaardLeft(map);
            int right = MidgaardRight(map);
            int top = MidgaardTop(map);
            int bottom = MidgaardBottom(map);
            int gateY = sy;

            map.Objects.RemoveAll(o => o.X >= left && o.X <= right && o.Y >= top && o.Y <= bottom);

            for (int y = top; y <= bottom; y++)
            for (int x = left; x <= right; x++)
            {
                int dx = x - sx;
                int dy = y - sy;
                SetExploreCell(
                    map,
                    x,
                    y,
                    1,
                    MidgaardDistrictRules.MaterialAtOffset(dx, dy),
                    MidgaardDistrictRules.RolesAtOffset(dx, dy));
            }

            for (int x = left; x <= right; x++)
            {
                SetExploreCell(map, x, top, 0, ExplorationMaterial.CityWall, ExplorationCellRole.City);
                SetExploreCell(map, x, bottom, 0, ExplorationMaterial.CityWall, ExplorationCellRole.City);
            }
            for (int y = top; y <= bottom; y++)
            {
                SetExploreCell(map, left, y, 0, ExplorationMaterial.CityWall, ExplorationCellRole.City);
                SetExploreCell(map, right, y, 0, ExplorationMaterial.CityWall, ExplorationCellRole.City);
            }

            SetMidgaardRoadCell(map, left, gateY, true);
            SetMidgaardRoadCell(map, right, gateY, true);
            SetMidgaardRoadCell(map, left + 1, gateY, false);
            SetMidgaardRoadCell(map, right - 1, gateY, false);
            for (int x = Mathf.Max(1, left - 5); x <= Mathf.Min(map.Width - 2, right + 5); x++) SetMidgaardRoadCell(map, x, gateY, x == left || x == right);
            CarveMidgaardPath(map, left, gateY, 2, Mathf.Max(2, gateY - 2));
            CarveMidgaardPath(map, right, gateY, map.Width - 3, Mathf.Min(map.Height - 3, gateY + 2));
            CarveMidgaardSquare(map, Mathf.Max(2, left - 2), gateY, 1, ExplorationMaterial.PackedDirt, false);
            CarveMidgaardSquare(map, Mathf.Min(map.Width - 3, right + 2), gateY, 1, ExplorationMaterial.PackedDirt, false);
            for (int y = top + 1; y <= bottom - 1; y++) SetMidgaardRoadCell(map, sx, y, false);
            for (int x = left + 1; x <= right - 1; x++) SetMidgaardRoadCell(map, x, sy, false);
            CarveMidgaardSquare(map, sx, sy, 1, ExplorationMaterial.MarketCobbles, true);
            CarveMidgaardSquare(map, sx, sy - 4, 1, ExplorationMaterial.TempleStone, true);
            CarveMidgaardPath(map, sx, sy, sx, sy - 6);
            CarveMidgaardPath(map, sx, sy, sx, sy + 5);
            CarveMidgaardPath(map, sx, sy + 5, sx + 5, sy + 3);
            CarveMidgaardPath(map, sx - 5, sy + 5, sx + 5, sy + 3);
            CarveMidgaardSquare(map, sx + 2, sy - 1, 1, ExplorationMaterial.MarketCobbles, true);
            CarveMidgaardSquare(map, sx + 2, sy - 4, 1, ExplorationMaterial.TempleStone, true);
            CarveMidgaardSquare(map, sx + 7, sy + 1, 1, ExplorationMaterial.CityPaving, true);
            CarveMidgaardSquare(map, sx - 6, sy + 3, 1, ExplorationMaterial.MarketCobbles, true);
            CarveMidgaardSquare(map, sx - 6, sy - 1, 1, ExplorationMaterial.CityPaving, true);
            CarveMidgaardSquare(map, sx, top + 1, 1, ExplorationMaterial.KeepStone, true);
            CarveMidgaardSquare(map, sx - 5, bottom - 1, 1, ExplorationMaterial.SewerBrick, true);
            CarveMidgaardPath(map, sx, top + 1, sx + 2, top + 2);

            UpsertMapObject(map, sx, sy, ObjectType.Market);
            UpsertMapObject(map, sx - 1, sy, ObjectType.QuestBoard);
            UpsertMapObject(map, sx + 1, sy, ObjectType.MarketClerk);
            UpsertMapObject(map, sx, sy - 5, ObjectType.Temple);
            UpsertMapObject(map, sx, sy - 4, ObjectType.Fountain);
            UpsertMapObject(map, sx - 1, sy - 4, ObjectType.TempleHealer);
            UpsertMapObject(map, sx + 1, sy - 4, ObjectType.RecallCircle);
            UpsertMapObject(map, sx + 5, sy + 3, ObjectType.Diner);
            UpsertMapObject(map, sx + 6, sy + 3, ObjectType.Provisions);
            UpsertMapObject(map, sx - 3, sy + 2, ObjectType.Tavern);
            UpsertMapObject(map, sx - 2, sy + 2, ObjectType.TavernKeeper);
            UpsertMapObject(map, sx - 4, sy, ObjectType.Armorer);
            UpsertMapObject(map, sx + 4, sy, ObjectType.WeaponVendor);
            UpsertMapObject(map, sx + 4, sy + 1, ObjectType.Enchanter);
            UpsertMapObject(map, sx + 2, sy - 1, ObjectType.CityCourier);
            UpsertMapObject(map, sx - 6, sy + 3, ObjectType.WoundedTraveler);
            UpsertMapObject(map, sx + 7, sy + 1, ObjectType.StableHand);
            UpsertMapObject(map, sx + 1, top + 2, ObjectType.RoyalHerald);
            UpsertMapObject(map, sx + 2, sy - 4, ObjectType.NoviceHealer);
            UpsertMapObject(map, sx - 6, sy - 1, ObjectType.OldRoadScout);
            UpsertMapObject(map, sx + 5, sy + 4, ObjectType.DinerCook);
            UpsertMapObject(map, sx + 7, sy + 3, ObjectType.Provisioner);
            UpsertMapObject(map, sx + 2, bottom - 2, ObjectType.DockWorker);
            UpsertMapObject(map, sx - 2, top + 2, ObjectType.Scholar);
            UpsertMapObject(map, left, gateY, ObjectType.WestGate);
            UpsertMapObject(map, right, gateY, ObjectType.EastGate);
            UpsertMapObject(map, left + 1, gateY - 1, ObjectType.TownGuard);
            UpsertMapObject(map, right - 1, gateY - 1, ObjectType.TownGuard);
            UpsertMapObject(map, left + 1, gateY + 1, ObjectType.GateCaptain);
            UpsertMapObject(map, sx, top + 1, ObjectType.KingHall);
            UpsertMapObject(map, sx - 5, bottom - 1, ObjectType.Sewer);
            UpsertMapObject(map, sx - 4, bottom - 1, ObjectType.RatPeltQuest);

            UpsertMapObject(map, left, top, ObjectType.CityWall);
            UpsertMapObject(map, right, top, ObjectType.CityWall);
            UpsertMapObject(map, left, bottom, ObjectType.CityWall);
            UpsertMapObject(map, right, bottom, ObjectType.CityWall);
            for (int x = left + 2; x <= right - 2; x += 4)
            {
                UpsertMapObject(map, x, top, ObjectType.CityWall);
                UpsertMapObject(map, x, bottom, ObjectType.CityWall);
            }
            for (int y = top + 2; y <= bottom - 2; y += 4)
            {
                if (y == gateY) continue;
                UpsertMapObject(map, left, y, ObjectType.CityWall);
                UpsertMapObject(map, right, y, ObjectType.CityWall);
            }

            // Nearby keep and sewer plaza carving can reach the perimeter. Restore
            // the complete wall after all interior paving, then reopen only the two
            // road thresholds. This keeps art, terrain, and collision in agreement.
            for (int x = left; x <= right; x++)
            {
                SetExploreCell(map, x, top, 0, ExplorationMaterial.CityWall, ExplorationCellRole.City);
                SetExploreCell(map, x, bottom, 0, ExplorationMaterial.CityWall, ExplorationCellRole.City);
            }
            for (int y = top; y <= bottom; y++)
            {
                SetExploreCell(map, left, y, 0, ExplorationMaterial.CityWall, ExplorationCellRole.City);
                SetExploreCell(map, right, y, 0, ExplorationMaterial.CityWall, ExplorationCellRole.City);
            }
            SetMidgaardRoadCell(map, left, gateY, true);
            SetMidgaardRoadCell(map, right, gateY, true);
            UpsertMapObject(map, sx, top, ObjectType.NorthGate);
            UpsertMapObject(map, sx, bottom, ObjectType.SouthGate);

            // The throne-room repair relocates the herald and binds every city
            // doorway. Keep those invariants attached to any direct town repair.
            map.InvalidateObjectLookup();
            EnsureMidgaardInteriors(map);
        }

        private void SetMidgaardRoadCell(MapData map, int x, int y, bool threshold)
        {
            bool city = IsMidgaardCityCell(x, y, map, map?.Depth ?? 1);
            ExplorationMaterial currentMaterial = ExplorationSurfaceRules.MaterialAt(map, x, y);
            ExplorationMaterial material = city && currentMaterial != ExplorationMaterial.NaturalGround && currentMaterial != ExplorationMaterial.CityWall
                ? currentMaterial
                : city ? ExplorationMaterial.CityPaving : ExplorationMaterial.PackedDirt;
            ExplorationCellRole roles = ExplorationSurfaceRules.RolesAt(map, x, y) | ExplorationCellRole.Road;
            if (city) roles |= ExplorationCellRole.City;
            if (threshold) roles |= ExplorationCellRole.Threshold;
            SetExploreCell(map, x, y, 1, material, roles);
        }

        private void CarveMidgaardSquare(MapData map, int cx, int cy, int radius, ExplorationMaterial material, bool city)
        {
            for (int y = cy - radius; y <= cy + radius; y++)
            for (int x = cx - radius; x <= cx + radius; x++)
            {
                if (x <= 0 || y <= 0 || x >= map.Width - 1 || y >= map.Height - 1) continue;
                ExplorationCellRole roles = ExplorationSurfaceRules.RolesAt(map, x, y) | ExplorationCellRole.Plaza;
                if (city) roles |= ExplorationCellRole.City;
                SetExploreCell(map, x, y, 1, material, roles);
            }
        }

        private void CarveMidgaardPath(MapData map, int ax, int ay, int bx, int by)
        {
            int x = ax;
            int y = ay;
            int guard = 0;
            while ((x != bx || y != by) && guard++ < map.Width + map.Height)
            {
                if (x > 0 && y > 0 && x < map.Width - 1 && y < map.Height - 1) SetMidgaardRoadCell(map, x, y, false);
                if (x != bx) x += Math.Sign(bx - x);
                else if (y != by) y += Math.Sign(by - y);
            }
            if (bx > 0 && by > 0 && bx < map.Width - 1 && by < map.Height - 1) SetMidgaardRoadCell(map, bx, by, false);
        }

        private void UpsertMapObject(MapData map, int x, int y, ObjectType type)
        {
            if (map == null || x < 0 || y < 0 || x >= map.Width || y >= map.Height) return;
            map.Objects.RemoveAll(o => o.X == x && o.Y == y);
            MapObject placed = new MapObject(x, y, type);
            map.Objects.Add(placed);
            ApplyMapObjectSurface(map, placed);
        }

        private int MidgaardLeft(MapData map) => Mathf.Max(1, (map?.StartX ?? ExploreW / 2) - 10);

        private int MidgaardRight(MapData map) => Mathf.Min((map?.Width ?? ExploreW) - 2, (map?.StartX ?? ExploreW / 2) + 10);

        private int MidgaardTop(MapData map) => Mathf.Max(1, (map?.StartY ?? ExploreH / 2) - 8);

        private int MidgaardBottom(MapData map) => Mathf.Min((map?.Height ?? ExploreH) - 2, (map?.StartY ?? ExploreH / 2) + 7);

        private bool IsMidgaardCityCell(int x, int y, MapData map, int depth)
        {
            if (map == null || depth != 1) return false;
            return x >= MidgaardLeft(map) && x <= MidgaardRight(map) && y >= MidgaardTop(map) && y <= MidgaardBottom(map);
        }

        private bool EnsureExploreSurfaceData(MapData map, int sourceSaveVersion = SaveVersion)
        {
            if (map == null) return false;
            if (map.Objects == null) map.Objects = new List<MapObject>();
            bool rebuilt = ExplorationSurfaceRules.EnsureGrid(map);
            if (!rebuilt)
            {
                ExplorationSurfaceRules.RepairConsistency(map);
                return false;
            }

            MigrateLegacySurfaceGridV18(map);
            return true;
        }

        private void MigrateLegacySurfaceGridV18(MapData map)
        {
            // v17/v18 stored only binary topology. This classifier is the frozen
            // canonical migration for those saves; it never regenerates from Seed.
            InitializeWorldRegionSurfaces(map);
            FinalizeWorldRegionSurfaces(map);
            for (int y = 0; y < map.Height; y++)
            for (int x = 0; x < map.Width; x++)
            {
                bool city = IsMidgaardCityCell(x, y, map, map.Depth);
                if (city)
                {
                    ExplorationSurfaceRules.SetMaterial(
                        map,
                        x,
                        y,
                        TileAt(map, x, y) == 0 ? ExplorationMaterial.CityWall : ExplorationMaterial.CityPaving);
                    ExplorationSurfaceRules.AddRoles(map, x, y, ExplorationCellRole.City);
                }

                if (TileAt(map, x, y) == 1 && IsLegacyPathSurface(map, x, y))
                {
                    ExplorationSurfaceRules.AddRoles(map, x, y, ExplorationCellRole.Trail);
                }
                MapObject obj = ObjectAt(map, x, y);
                if (obj != null && obj.Type == ObjectType.Bridge) ApplyMapObjectSurface(map, obj);
            }
        }

        private bool IsLegacyPathSurface(MapData map, int x, int y)
        {
            if (map == null || TileAt(map, x, y) != 1) return false;
            if (ZoneIdFor(x, y, map, map.Depth) == "midgaard-road") return true;

            bool north = TileAt(map, x, y - 1) == 1;
            bool east = TileAt(map, x + 1, y) == 1;
            bool south = TileAt(map, x, y + 1) == 1;
            bool west = TileAt(map, x - 1, y) == 1;
            int neighbors = (north ? 1 : 0) + (east ? 1 : 0) + (south ? 1 : 0) + (west ? 1 : 0);
            bool corridor = neighbors == 1
                || (neighbors == 2 && ((north && south) || (east && west)));
            bool oldSpine = Mathf.Abs(x - map.StartX) <= 1 && y >= map.StartY - 5 && y <= map.StartY + 1;
            return corridor || oldSpine;
        }

        private ExplorationMaterial ExploreMaterialAt(int x, int y)
        {
            return ExplorationSurfaceRules.MaterialAt(state?.Map, x, y);
        }

        private ExplorationCellRole ExploreRolesAt(int x, int y)
        {
            return ExplorationSurfaceRules.RolesAt(state?.Map, x, y);
        }

        private void EnsureWorldLandmarks()
        {
            if (state?.Map == null) return;
            EnsureExploreSurfaceData(state.Map);
            EnsureRegionalRouteJunctionRoles(state.Map);
            if (state.Map.Objects == null) state.Map.Objects = new List<MapObject>();
            PruneDisabledGeneratedObjects(state.Map);
            if (state.Depth == 1)
            {
                EnsureMidgaardStartZone(state.Map);
            }
            EnsureOldRoadDescentMarker();
            if (state.Depth == 2) EnsureKoboldKingCaveMarker();
            EnsureRouteScaffoldLandmarks();
            int landmarkCount = state.Map.Objects.Count(o => o.Type == ObjectType.Obelisk || o.Type == ObjectType.Ruin || o.Type == ObjectType.Bridge || o.Type == ObjectType.Cave);
            if (landmarkCount < 5)
            {
                List<Point> open = new List<Point>();
                for (int yy = 1; yy < state.Map.Height - 1; yy++)
                for (int xx = 1; xx < state.Map.Width - 1; xx++)
                {
                    if (TileAt(state.Map, xx, yy) == 1 && Distance(xx, yy, state.Map.StartX, state.Map.StartY) > 5 && ObjectAt(state.Map, xx, yy) == null)
                    {
                        open.Add(new Point(xx, yy));
                    }
                }

                System.Random mapRng = new System.Random(state.Seed + state.Depth * 1709 + 27);
                PlaceObjects(state.Map, open, mapRng, ObjectType.Ruin, 3);
                PlaceObjects(state.Map, open, mapRng, ObjectType.Obelisk, 2);
                PlaceObjects(state.Map, open, mapRng, ObjectType.Bridge, 1);
                PlaceObjects(state.Map, open, mapRng, ObjectType.Cave, 1);
                if (state.Depth == 2) EnsureKoboldKingCaveMarker();
                EnsureRouteScaffoldLandmarks();
            }
            CertifyRegionalRouteCircuit(state.Map);
            CertifyCriticalExploreRoutes(state.Map);
            EnsureRoamingThreats();
        }

        private void EnsureRegionalRouteJunctionRoles(MapData map)
        {
            if (map == null || map.Tiles == null) return;
            foreach (WorldMapJunction junction in WorldMapGenerationRules.RegionalJunctions(map.Width, map.Height, map.StartX, map.StartY))
            {
                if (junction.X < 0 || junction.Y < 0 || junction.X >= map.Width || junction.Y >= map.Height) continue;
                if (TileAt(map, junction.X, junction.Y) != 1) continue;
                ExplorationSurfaceRules.AddRoles(map, junction.X, junction.Y, ExplorationCellRole.Road | ExplorationCellRole.Clearing);
            }
        }

        private void EnsureRouteScaffoldLandmarks()
        {
            if (!ContentSetCatalog.ShowPrototypeScaffold(activeContentSet)) return;
            if (state?.Map?.Objects == null) return;

            List<Point> open = new List<Point>();
            for (int yy = 1; yy < state.Map.Height - 1; yy++)
            for (int xx = 1; xx < state.Map.Width - 1; xx++)
            {
                if (TileAt(state.Map, xx, yy) == 1 && Distance(xx, yy, state.Map.StartX, state.Map.StartY) > 4 && ObjectAt(state.Map, xx, yy) == null)
                {
                    open.Add(new Point(xx, yy));
                }
            }

            if (open.Count == 0) return;
            System.Random mapRng = new System.Random(state.Seed + state.Depth * 1931 + 65);
            foreach (RouteScaffoldDef def in RouteScaffoldDefs())
            {
                if (def == null || def.Type == ObjectType.QuestBoard || state.Depth < def.MinDepth) continue;
                bool exists = state.Map.Objects.Any(o => o.Type == def.Type && string.Equals(ZoneIdFor(o.X, o.Y, state.Map, state.Depth), def.ZoneId, StringComparison.OrdinalIgnoreCase));
                if (!exists) PlaceObjectsInZone(state.Map, open, mapRng, def.ZoneId, def.Type, 1);
            }
        }

        private void CertifyCriticalExploreRoutes(MapData map)
        {
            if (map?.Objects == null || map.Tiles == null) return;
            Point anchor = FindCriticalRouteAnchor(map);
            if (anchor == null) return;

            bool[,] reachable = ExplorationTraversalRules.ReachableMask(map, anchor.X, anchor.Y);
            List<MapObject> critical = new List<MapObject>();
            if (map.Depth == 1)
            {
                ObjectType[] cityTypes =
                {
                    ObjectType.KingHall,
                    ObjectType.Sewer,
                    ObjectType.Armorer,
                    ObjectType.TempleHealer,
                    ObjectType.GateCaptain,
                    ObjectType.OldRoadScout,
                    ObjectType.EastGate,
                    ObjectType.WestGate
                };
                foreach (ObjectType type in cityTypes)
                {
                    MapObject cityObject = map.Objects.FirstOrDefault(o => o != null && o.Type == type);
                    if (cityObject != null) critical.Add(cityObject);
                }
            }

            if (ContentSetCatalog.AllowKoboldChapter(activeContentSet, state?.StoryFlags))
            {
                if (map.Depth == 1)
                {
                    MapObject stair = map.FindObjectById(OldRoadDescentId);
                    if (stair == null && ContentSetCatalog.IsFullPrototype(activeContentSet))
                    {
                        stair = map.Objects.FirstOrDefault(o => o != null
                            && o.Type == ObjectType.Stairs
                            && ExplorationTraversalRules.CanReachObject(reachable, map, o))
                            ?? map.Objects.FirstOrDefault(o => o != null && o.Type == ObjectType.Stairs);
                    }
                    if (stair != null) critical.Add(stair);
                }

                if (map.Depth == 2)
                {
                    MapObject storyCave = map.Objects.FirstOrDefault(o => o != null
                        && o.Type == ObjectType.Cave
                        && ZoneIdFor(o.X, o.Y, map, map.Depth) == "dusk-market");
                    if (storyCave != null) critical.Add(storyCave);
                }
            }

            if (ContentSetCatalog.ShowPrototypeScaffold(activeContentSet))
            {
                critical.AddRange(map.Objects.Where(o => o != null && IsRouteScaffoldObject(o.Type)));
            }

            foreach (MapObject obj in critical.Distinct().ToList())
            {
                if (ExplorationTraversalRules.CanReachObject(reachable, map, obj)) continue;
                RepairCriticalExploreObject(map, obj, reachable);
                reachable = ExplorationTraversalRules.ReachableMask(map, anchor.X, anchor.Y);
            }
        }

        private Point FindCriticalRouteAnchor(MapData map)
        {
            if (map == null) return null;
            Point[] preferred =
            {
                new Point(map.StartX, map.StartY + 1),
                new Point(map.StartX, map.StartY - 1),
                new Point(map.StartX - 1, map.StartY),
                new Point(map.StartX + 1, map.StartY),
                new Point(map.StartX, map.StartY)
            };
            foreach (Point point in preferred)
            {
                if (ExplorationTraversalRules.IsStandable(map, point.X, point.Y)) return point;
            }

            Point best = null;
            int bestScore = int.MaxValue;
            for (int y = 1; y < map.Height - 1; y++)
            for (int x = 1; x < map.Width - 1; x++)
            {
                if (!ExplorationTraversalRules.IsStandable(map, x, y)) continue;
                int score = Distance(x, y, map.StartX, map.StartY) * 100 + y * map.Width + x;
                if (score >= bestScore) continue;
                bestScore = score;
                best = new Point(x, y);
            }
            return best;
        }

        private void RepairCriticalExploreObject(MapData map, MapObject obj, bool[,] reachable)
        {
            if (map == null || obj == null || reachable == null) return;
            bool authoredCityObject = IsMidgaardCityCell(obj.X, obj.Y, map, map.Depth);
            string requiredZone = ZoneIdFor(obj.X, obj.Y, map, map.Depth);

            if (!authoredCityObject)
            {
                Point reachablePlacement = FindCriticalExplorePlacement(map, obj, reachable, requiredZone, true)
                    ?? FindCriticalExplorePlacement(map, obj, reachable, "", true);
                if (reachablePlacement != null)
                {
                    obj.X = reachablePlacement.X;
                    obj.Y = reachablePlacement.Y;
                    map.InvalidateObjectLookup();
                    return;
                }

                Point zonedPlacement = FindCriticalExplorePlacement(map, obj, reachable, requiredZone, false);
                if (zonedPlacement != null)
                {
                    obj.X = zonedPlacement.X;
                    obj.Y = zonedPlacement.Y;
                    map.InvalidateObjectLookup();
                }
            }

            Point approach = CriticalExploreApproach(map, obj, reachable);
            Point connectorStart = NearestReachableExplorePoint(reachable, approach?.X ?? obj.X, approach?.Y ?? obj.Y);
            if (approach == null || connectorStart == null) return;
            CarveCriticalExploreConnector(map, connectorStart, approach, obj);
        }

        private Point FindCriticalExplorePlacement(MapData map, MapObject moving, bool[,] reachable, string requiredZone, bool mustAlreadyBeReachable)
        {
            Point best = null;
            int bestScore = int.MaxValue;
            bool standableObject = ExplorationTraversalRules.CanStandOnObject(moving);
            for (int y = 1; y < map.Height - 1; y++)
            for (int x = 1; x < map.Width - 1; x++)
            {
                if (ObjectAt(map, x, y) != null || TileAt(map, x, y) != 1) continue;
                if (IsMidgaardCityCell(x, y, map, map.Depth)) continue;
                if (!string.IsNullOrEmpty(requiredZone) && ZoneIdFor(x, y, map, map.Depth) != requiredZone) continue;
                if (!CanPlaceGeneratedExploreObject(map, x, y, moving.Type)) continue;

                bool reachablePlacement = standableObject
                    ? reachable[x, y]
                    : HasReachableExploreNeighbor(reachable, x, y);
                if (mustAlreadyBeReachable && !reachablePlacement) continue;
                int score = Distance(x, y, moving.X, moving.Y) * 100 + y * map.Width + x;
                if (score >= bestScore) continue;
                bestScore = score;
                best = new Point(x, y);
            }
            return best;
        }

        private bool HasReachableExploreNeighbor(bool[,] reachable, int x, int y)
        {
            return IsReachableExploreCell(reachable, x, y - 1)
                || IsReachableExploreCell(reachable, x - 1, y)
                || IsReachableExploreCell(reachable, x + 1, y)
                || IsReachableExploreCell(reachable, x, y + 1);
        }

        private bool IsReachableExploreCell(bool[,] reachable, int x, int y)
        {
            return reachable != null
                && x >= 0
                && y >= 0
                && x < reachable.GetLength(0)
                && y < reachable.GetLength(1)
                && reachable[x, y];
        }

        private Point CriticalExploreApproach(MapData map, MapObject obj, bool[,] reachable)
        {
            if (ExplorationTraversalRules.CanStandOnObject(obj)) return new Point(obj.X, obj.Y);
            Point[] neighbors =
            {
                new Point(obj.X, obj.Y - 1),
                new Point(obj.X - 1, obj.Y),
                new Point(obj.X + 1, obj.Y),
                new Point(obj.X, obj.Y + 1)
            };
            Point best = null;
            int bestScore = int.MaxValue;
            foreach (Point point in neighbors)
            {
                if (point.X < 1 || point.Y < 1 || point.X >= map.Width - 1 || point.Y >= map.Height - 1) continue;
                MapObject occupant = ObjectAt(map, point.X, point.Y);
                if (occupant != null && ExplorationTraversalRules.BlocksMovement(occupant)) continue;
                int distance = DistanceToReachableExploreCell(reachable, point.X, point.Y);
                if (distance >= bestScore) continue;
                bestScore = distance;
                best = point;
            }
            return best;
        }

        private int DistanceToReachableExploreCell(bool[,] reachable, int x, int y)
        {
            int best = int.MaxValue;
            if (reachable == null) return best;
            for (int yy = 0; yy < reachable.GetLength(1); yy++)
            for (int xx = 0; xx < reachable.GetLength(0); xx++)
            {
                if (!reachable[xx, yy]) continue;
                best = Mathf.Min(best, Distance(x, y, xx, yy));
            }
            return best;
        }

        private Point NearestReachableExplorePoint(bool[,] reachable, int x, int y)
        {
            Point best = null;
            int bestScore = int.MaxValue;
            if (reachable == null) return null;
            for (int yy = 0; yy < reachable.GetLength(1); yy++)
            for (int xx = 0; xx < reachable.GetLength(0); xx++)
            {
                if (!reachable[xx, yy]) continue;
                int score = Distance(x, y, xx, yy) * 100 + yy * reachable.GetLength(0) + xx;
                if (score >= bestScore) continue;
                bestScore = score;
                best = new Point(xx, yy);
            }
            return best;
        }

        private void CarveCriticalExploreConnector(MapData map, Point from, Point to, MapObject protectedObject)
        {
            if (map == null || from == null || to == null) return;
            int x = from.X;
            int y = from.Y;
            int guard = 0;
            while ((x != to.X || y != to.Y) && guard++ < map.Width + map.Height)
            {
                OpenCriticalExploreConnectorCell(map, x, y, protectedObject);
                if (x != to.X) x += Math.Sign(to.X - x);
                else if (y != to.Y) y += Math.Sign(to.Y - y);
            }
            OpenCriticalExploreConnectorCell(map, to.X, to.Y, protectedObject);
        }

        private void OpenCriticalExploreConnectorCell(MapData map, int x, int y, MapObject protectedObject)
        {
            if (x < 1 || y < 1 || x >= map.Width - 1 || y >= map.Height - 1) return;
            if (IsMidgaardCityCell(x, y, map, map.Depth)
                || MidgaardInteriorRules.IsReservedCell(map, x, y))
            {
                return;
            }
            OpenExploreConnectorSurface(map, x, y, ExplorationCellRole.Trail);
            MapObject blocker = ObjectAt(map, x, y);
            if (blocker == null || ReferenceEquals(blocker, protectedObject) || !ExplorationTraversalRules.BlocksMovement(blocker)) return;
            if (IsMidgaardCityCell(x, y, map, map.Depth) || IsRouteScaffoldObject(blocker.Type)) return;
            map.Objects.Remove(blocker);
        }

        private void OpenExploreConnectorSurface(MapData map, int x, int y, ExplorationCellRole role)
        {
            SetExploreTile(map, x, y, 1, role);
            string zoneId = ExploreVisualZoneId(x, y, map, map.Depth);
            ExplorationSurfaceRules.SetMaterial(map, x, y, OpenMaterialForZone(zoneId));
        }

        private void DrawExplore()
        {
            exploreHoverLookLine = "";
            boardRect = GetBoardRect();
            boardRect = ExplorationHudScreenLayout.ReserveDetailsFromBoard(boardRect, Screen.width, Screen.height, !exploreHudCollapsed);
            DrawPanel(boardRect);
            int viewW = Mathf.Min(ExploreViewportWidth(), state.Map.Width);
            int viewH = Mathf.Min(ExploreViewportHeight(), state.Map.Height);
            Point origin = ExploreViewportOrigin(viewW, viewH);
            Rect grid = ExploreBoardInnerRect(boardRect, viewW, viewH);
            float availableWidth = grid.width;
            float availableHeight = grid.height;
            float cell = Mathf.Max(1f, Mathf.Floor(Mathf.Min(availableWidth / viewW, availableHeight / viewH)));
            grid.x = Mathf.Round(grid.x + (availableWidth - cell * viewW) * 0.5f);
            grid.y = Mathf.Round(grid.y + (availableHeight - cell * viewH) * 0.5f);
            grid.width = cell * viewW;
            grid.height = cell * viewH;

            for (int vy = 0; vy < viewH; vy++)
            for (int vx = 0; vx < viewW; vx++)
            {
                int x = origin.X + vx;
                int y = origin.Y + vy;
                Rect c = new Rect(grid.x + vx * cell, grid.y + vy * cell, cell, cell);
                int distance = Distance(x, y, state.PlayerX, state.PlayerY);
                int tile = TileAt(state.Map, x, y);
                DrawRect(c, ExploreTileBaseColor(x, y, tile, true));
                string tileKind = ExploreTileKind(x, y, tile);
                bool drewTileArt = TryDrawExploreEnvironmentTile(c, x, y, tile, tileKind);
                if (!drewTileArt) DrawExploreTileMotif(c, x, y, tile);
                else if (!tileKind.StartsWith("midgaard")
                    && ExplorationReadabilityRules.ShouldDrawProceduralGroundAccent(
                        distance,
                        ObjectAt(state.Map, x, y) != null,
                        exploreWideView,
                        ExploreNoise(x, y, 211)))
                {
                    DrawExploreTileAccent(c, x, y, tile, tileKind);
                }
                DrawExploreMaterialFeather(c, x, y, tile);
                DrawExploreSurfaceOverlay(c, x, y, tile);
                DrawMidgaardPavingDecal(c, x, y, tile, tileKind);
                DrawExploreTileEdges(c, x, y, tile);
                DrawExploreDistanceShade(c, vx, vy, viewW, viewH);
                DrawMidgaardAmbientProp(c, x, y, tile, tileKind);
                DrawExploreBiomeAmbientProp(c, x, y, tile, tileKind);
            }

            DrawExploreTerrainBoundaries(grid, cell, origin, viewW, viewH);
            DrawExploreWaypointTrail(grid, cell, origin, viewW, viewH);
            DrawExploreMovementHints(grid, cell, origin, viewW, viewH);

            foreach (MapObject obj in state.Map.Objects)
            {
                if (!ExplorePointInViewport(obj.X, obj.Y, origin, viewW, viewH)) continue;
                Rect objectCell = new Rect(grid.x + (obj.X - origin.X) * cell, grid.y + (obj.Y - origin.Y) * cell, cell, cell);
                if (obj.Type == ObjectType.CityWall)
                {
                    if (showExploreArtDebug) DrawExploreArtDebugOverlay(objectCell, objectCell, "Wall terrain");
                    continue;
                }
                bool objective = IsCurrentMidgaardObjective(obj);
                int objectDistance = Distance(obj.X, obj.Y, state.PlayerX, state.PlayerY);
                if (ShouldSuppressExploreRegionObject(obj, objectDistance, objective)) continue;
                Rect objectRect;
                if (ShouldUseExploreRegionMarker(obj, objectDistance, objective))
                {
                    objectRect = DrawExploreRegionActorMarker(objectCell, obj);
                }
                else if (TryDrawWorldAreaSetpiece(objectCell, obj, out objectRect))
                {
                    // Authored regional centers own one consolidated visual identity.
                }
                else
                {
                    DrawMidgaardBuildingFootprint(objectCell, obj);
                    objectRect = ExploreObjectRect(objectCell, obj);
                    DrawExploreObject(objectCell, objectRect, obj);
                }
                bool currentTarget = ReferenceEquals(CurrentExploreInteraction().Target, obj);
                if (!currentTarget)
                {
                    if (objective) DrawExploreObjectiveMarker(objectCell, obj);
                    else if (objectDistance <= 1 && ShouldShowNearbyExploreCue(obj)) DrawExploreNearbyObjectCue(objectCell, obj);
                    else DrawExploreProgressionMarker(objectCell, obj);
                }
                if (showExploreArtDebug) DrawExploreArtDebugOverlay(objectCell, objectRect, obj.Type.ToString());
            }

            DrawRoamingThreats(grid, cell, origin, viewW, viewH);

            PartyMember lead = state.Party.Count > 0 ? state.Party[0] : null;
            Color leadColor = lead != null ? MemberColor(lead) : teal;
            string leadSigil = lead != null ? lead.Sigil : "bar";
            string leadRole = lead != null ? lead.Role : "shield";
            string tokenRole = ExplorationArtRules.PartyTokenRole(state.Party.Count, leadRole);
            Rect playerCell = new Rect(grid.x + (state.PlayerX - origin.X) * cell, grid.y + (state.PlayerY - origin.Y) * cell, cell, cell);
            Rect tokenRect;
            if (exploreWideView)
            {
                tokenRect = DrawExploreRegionPartyMarker(playerCell, leadColor, tokenRole, leadSigil);
            }
            else
            {
                tokenRect = ExplorePartyTokenRect(playerCell);
                if (!TryDrawExplorePartyToken(tokenRect, tokenRole, leadColor, leadSigil))
                {
                    DrawToken(tokenRect, leadRole, leadColor, true, "", leadSigil);
                }
                DrawExplorePlayerLocator(playerCell, tokenRect, leadColor);
            }
            DrawExploreGuidanceCues(grid, cell, origin, viewW, viewH);
            DrawExploreUseTargetCue(grid, cell, origin, viewW, viewH);
            if (showExploreArtDebug) DrawExploreArtDebugOverlay(playerCell, tokenRect, "Party");
            UpdateExploreHoverLook(grid, cell, origin, viewW, viewH);
            DrawExploreRegionStrip(grid);
            DrawExploreViewportEdgeHints(grid, origin, viewW, viewH);
            DrawExploreHover(grid, cell, origin, viewW, viewH);
            HandleExploreMouse(grid, cell, origin, viewW, viewH);
        }

        private int ExploreViewportWidth()
        {
            if (IsMidgaardRegionView()) return Mathf.Min(23, state.Map.Width);
            return ExplorationArtRules.ViewportWidth(exploreWideView, ExploreAdaptiveViewportTier());
        }

        private int ExploreViewportHeight()
        {
            if (IsMidgaardRegionView()) return Mathf.Min(17, state.Map.Height);
            return ExplorationArtRules.ViewportHeight(exploreWideView, ExploreAdaptiveViewportTier());
        }

        private bool IsMidgaardRegionView()
        {
            return exploreWideView
                && state?.Map != null
                && state.Depth == 1
                && IsMidgaardCityCell(state.PlayerX, state.PlayerY, state.Map, state.Depth);
        }

        private int ExploreAdaptiveViewportTier()
        {
            float availableWidth = boardRect.width > 0f ? boardRect.width : Screen.width;
            float availableHeight = boardRect.height > 0f ? boardRect.height : Screen.height;
            return ExplorationArtRules.AdaptiveViewportTier(exploreWideView, availableWidth, availableHeight);
        }

        private string ExploreViewLabel()
        {
            return exploreWideView ? "Region Map" : "Local Map";
        }

        private string ExploreViewHint()
        {
            return exploreWideView
                ? $"Route context: {ExploreViewportWidth()}x{ExploreViewportHeight()} map cells; distant ground may remain fogged."
                : $"Local focus: larger {ExploreViewportWidth()}x{ExploreViewportHeight()} tiles for travel and NPCs.";
        }

        private string ExploreHudHint()
        {
            return exploreHudCollapsed ? "Q opens Location details." : "Q returns to the map.";
        }

        private void ToggleExploreView()
        {
            exploreWideView = !exploreWideView;
            PlaySfx("uitab", 0.55f);
            ShowBanner(ExploreViewLabel());
            PushLog($"{ExploreViewLabel()}: {ExploreViewHint()}", Tone.Normal);
        }

        private void ToggleExploreHud()
        {
            exploreHudCollapsed = !exploreHudCollapsed;
            PlaySfx(exploreHudCollapsed ? "uiclose" : "uiopen", 0.5f);
            ShowBanner(exploreHudCollapsed ? "Map View" : "Location Details");
        }

        private void ToggleExploreArtDebug()
        {
            showExploreArtDebug = !showExploreArtDebug;
            PlaySfx("ui", 0.45f);
            ShowBanner(showExploreArtDebug ? "Map Art Debug On" : "Map Art Debug Off");
        }

        private Rect ExplorePartyTokenRect(Rect cell)
        {
            return Pad(cell, cell.width * ExplorationArtRules.PartyTokenPadding(exploreWideView));
        }

        private Rect ExploreObjectRect(Rect cell, MapObject obj)
        {
            if (obj == null) return Pad(cell, cell.width * 0.18f);
            if (IsMidgaardGateType(obj.Type))
            {
                bool sideGate = obj.Type == ObjectType.EastGate || obj.Type == ObjectType.WestGate;
                float width = cell.width * ExplorationArtRules.GateArtWidthInCells(exploreWideView, sideGate);
                float height = cell.height * ExplorationArtRules.GateArtHeightInCells(exploreWideView, sideGate);
                float y = sideGate
                    ? cell.center.y - height * 0.5f
                    : cell.yMax - height + cell.height * ExplorationArtRules.GateArtBaseOffsetInCells();
                return new Rect(
                    cell.center.x - width * 0.5f,
                    y,
                    width,
                    height);
            }
            return Pad(cell, cell.width * ExploreObjectPadding(obj.Type));
        }

        private void DrawMidgaardBuildingFootprint(Rect cell, MapObject obj)
        {
            if (obj == null || !ExplorationArtRules.IsMidgaardBuilding(obj.Type)) return;
            float width = cell.width * (exploreWideView ? 1.18f : 1.48f);
            float height = cell.height * (exploreWideView ? 0.46f : 0.58f);
            Rect foundation = new Rect(
                cell.center.x - width * 0.5f,
                cell.y + cell.height * (exploreWideView ? 0.48f : 0.42f),
                width,
                height);
            Color accent = MidgaardBuildingDistrictAccent(obj.Type);
            DrawRect(foundation, Hex("030506", exploreWideView ? 0.34f : 0.42f));
            DrawRect(
                new Rect(foundation.x, foundation.yMax - Mathf.Max(2f, foundation.height * 0.12f), foundation.width, Mathf.Max(2f, foundation.height * 0.12f)),
                accent.WithAlpha(exploreWideView ? 0.30f : 0.38f));
            DrawBorder(foundation, accent.WithAlpha(exploreWideView ? 0.22f : 0.28f), 1);
            Rect threshold = new Rect(
                cell.center.x - cell.width * 0.27f,
                cell.yMax - Mathf.Max(2f, cell.height * 0.10f),
                cell.width * 0.54f,
                Mathf.Max(2f, cell.height * 0.10f));
            DrawRect(threshold, Color.Lerp(accent, cursorWhite, 0.18f).WithAlpha(0.42f));
        }

        private Color MidgaardBuildingDistrictAccent(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Temple: return frost;
                case ObjectType.Market:
                case ObjectType.Provisions: return gold;
                case ObjectType.Tavern:
                case ObjectType.Diner: return ember;
                case ObjectType.Enchanter: return teal;
                case ObjectType.KingHall: return Color.Lerp(gold, frost, 0.38f);
                case ObjectType.Armorer:
                case ObjectType.WeaponVendor: return line;
                default: return moss;
            }
        }

        private bool IsMidgaardGateType(ObjectType type)
        {
            return type == ObjectType.NorthGate
                || type == ObjectType.SouthGate
                || type == ObjectType.EastGate
                || type == ObjectType.WestGate;
        }

        private bool ShouldSuppressExploreRegionObject(MapObject obj, int distance, bool objective)
        {
            if (!exploreWideView || obj == null || objective || distance <= 2) return false;
            if (IsRegionalSiteDecoration(state?.Map, obj)) return true;
            if (obj.Type == ObjectType.TownGuard) return true;
            return IsMidgaardNpcObject(obj.Type)
                && !WorldMapRegionMarkerCatalog.ShouldShowActor(obj.Type, distance, objective);
        }

        private bool TryDrawWorldAreaSetpiece(Rect cell, MapObject obj, out Rect drawnRect)
        {
            drawnRect = cell;
            if (!TryRegionalSite(state?.Map, obj, out WorldMapSite site)) return false;
            int index = WorldAreaSetpiecePresentationRules.IconIndex(site.Id);
            if (index < 0) return false;

            float size = cell.width * WorldAreaSetpiecePresentationRules.MapScale(exploreWideView);
            drawnRect = new Rect(
                cell.center.x - size * 0.5f,
                cell.yMax - size * WorldAreaSetpiecePresentationRules.BaselineFraction(exploreWideView),
                size,
                size);
            WorldMapArtSpec spec = new WorldMapArtSpec(
                0.98f,
                new Vector2(0.5f, 1f),
                Vector2.zero,
                true);
            return TryDrawWorldAreaSetpieceAtlasIcon(drawnRect, index, Color.white, spec);
        }

        private bool ShouldUseExploreRegionMarker(MapObject obj, int distance, bool objective)
        {
            if (!exploreWideView || obj == null || objective || distance <= 1) return false;
            return obj.Type == ObjectType.TownGuard || IsMidgaardNpcObject(obj.Type);
        }

        private Rect DrawExploreRegionActorMarker(Rect cell, MapObject obj)
        {
            Color accent = ObjectColor(obj.Type);
            float size = Mathf.Max(14f, cell.width * 0.66f);
            Rect marker = new Rect(cell.center.x - size * 0.5f, cell.center.y - size * 0.48f, size, size);
            int artIndex = WorldMapRegionMarkerCatalog.ActorMarkerIndex(obj.Type);
            if (TryDrawWorldMapRegionMarkerAtlasIcon(marker, artIndex, Color.white))
            {
                Rect underline = new Rect(
                    marker.x + marker.width * 0.22f,
                    marker.yMax - Mathf.Max(1f, marker.height * 0.05f),
                    marker.width * 0.56f,
                    Mathf.Max(1f, marker.height * 0.05f));
                DrawRect(underline, accent.WithAlpha(0.78f));
                return marker;
            }

            size = Mathf.Max(12f, cell.width * 0.46f);
            marker = new Rect(cell.center.x - size * 0.5f, cell.center.y - size * 0.42f, size, size);
            DrawRect(marker, Hex("030405", 0.90f));
            DrawBorder(marker, accent.WithAlpha(0.92f), 1);
            Rect inner = Pad(marker, marker.width * 0.20f);
            DrawRect(new Rect(inner.x + inner.width * 0.32f, inner.y, inner.width * 0.36f, inner.height * 0.30f), cursorWhite.WithAlpha(0.92f));
            DrawRect(new Rect(inner.x + inner.width * 0.17f, inner.y + inner.height * 0.34f, inner.width * 0.66f, inner.height * 0.50f), accent.WithAlpha(0.94f));
            DrawRect(new Rect(marker.x + marker.width * 0.18f, marker.yMax - Mathf.Max(2f, marker.height * 0.08f), marker.width * 0.64f, Mathf.Max(2f, marker.height * 0.08f)), accent.WithAlpha(0.74f));
            return marker;
        }

        private Rect DrawExploreRegionPartyMarker(Rect cell, Color color, string role, string sigil)
        {
            float size = Mathf.Max(
                ExplorationArtRules.PartyRegionMarkerMinimumPixels(),
                cell.width * ExplorationArtRules.PartyRegionMarkerScale());
            Rect marker = new Rect(cell.center.x - size * 0.5f, cell.center.y - size * 0.5f, size, size);
            if (!TryDrawExplorePartyToken(marker, role, color, sigil))
            {
                DrawTinyUiIcon(marker, "party", Color.Lerp(color, gold, 0.32f));
            }
            DrawCornerBrackets(Pad(cell, cell.width * 0.13f), teal.WithAlpha(0.94f), 2f, cell.width * 0.18f);
            return marker;
        }

        private float ExploreObjectPadding(ObjectType type)
        {
            if (IsMidgaardNpcObject(type)) return exploreWideView ? 0.18f : 0.09f;
            if (type == ObjectType.CityWall) return 0.06f;
            if (type == ObjectType.EastGate || type == ObjectType.WestGate || type == ObjectType.NorthGate || type == ObjectType.SouthGate) return exploreWideView ? 0.10f : 0.02f;
            if (type == ObjectType.TownGuard) return exploreWideView ? 0.18f : 0.10f;
            if (ExplorationArtRules.IsMidgaardBuilding(type)) return ExplorationArtRules.MidgaardBuildingPadding(exploreWideView);
            if (type == ObjectType.Fountain || type == ObjectType.RecallCircle) return exploreWideView ? 0.18f : 0.10f;
            if (type == ObjectType.Cache || type == ObjectType.Shrine || type == ObjectType.Encounter || type == ObjectType.Stairs || type == ObjectType.Camp || type == ObjectType.Cave) return exploreWideView ? 0.17f : 0.08f;
            return exploreWideView ? 0.19f : 0.11f;
        }

        private void DrawMidgaardAmbientProp(Rect cell, int x, int y, int tile, string kind)
        {
            if (state?.Map == null || state.Depth != 1 || tile == 0 || !IsMidgaardCityCell(x, y, state.Map, state.Depth)) return;
            if (ObjectAt(state.Map, x, y) != null) return;
            if (HasNearbyMidgaardFocusObject(x, y)) return;
            if (x == state.PlayerX && y == state.PlayerY) return;
            kind = kind ?? "";
            if (!kind.StartsWith("midgaard")) return;

            int roll = ExploreNoise(x, y, 79) % 100;
            int index = MidgaardAmbientPropAtlasIndex(kind, x, y, roll);
            if (index < 0) return;

            bool useStreetLife = (ExploreNoise(x, y, 89) & 1) == 0;
            int streetLifeIndex = useStreetLife ? MidgaardStreetLifeAtlasIndex(kind, x, y) : -1;
            float scale = streetLifeIndex >= 0
                ? kind == "midgaard-market" || kind == "midgaard-tavern" ? 0.13f : 0.16f
                : kind == "midgaard-market" || kind == "midgaard-tavern" ? 0.18f : 0.20f;
            Rect propRect = Pad(cell, cell.width * scale);
            float noise = (ExploreNoise(x, y, 83) % 7) / 6f;
            Color tint = Color.white.WithAlpha(ExplorationReadabilityRules.MidgaardPropAlpha(exploreWideView, noise));
            bool drawn = streetLifeIndex >= 0 && TryDrawMidgaardStreetLifeAtlasIcon(propRect, streetLifeIndex, tint);
            if (!drawn) drawn = TryDrawMidgaardCityPropAtlasIcon(propRect, index, tint);
            if (!drawn)
            {
                DrawMidgaardAmbientPropFallback(propRect, index, tint);
            }
        }

        private bool HasNearbyMidgaardFocusObject(int x, int y)
        {
            if (state?.Map?.Objects == null) return false;
            foreach (MapObject obj in state.Map.Objects)
            {
                if (obj == null || obj.Type == ObjectType.CityWall) continue;
                if (Mathf.Abs(obj.X - x) + Mathf.Abs(obj.Y - y) <= 1) return true;
            }
            return false;
        }

        private int MidgaardStreetLifeAtlasIndex(string kind, int x, int y)
        {
            int variant = ExploreNoise(x, y, 97) % 4;
            switch (kind ?? "")
            {
                case "midgaard-market": return PickPropVariant(variant, 1, 12, 14, 2);
                case "midgaard-temple": return PickPropVariant(variant, 4, 6, 15, 16);
                case "midgaard-fountain":
                case "midgaard-recall": return PickPropVariant(variant, 6, 15, 16, 19);
                case "midgaard-diner":
                case "midgaard-provisions": return PickPropVariant(variant, 1, 12, 14, 8);
                case "midgaard-tavern": return PickPropVariant(variant, 8, 0, 3, 18);
                case "midgaard-armorer": return PickPropVariant(variant, 5, 12, 13, 9);
                case "midgaard-weapons": return PickPropVariant(variant, 5, 9, 12, 2);
                case "midgaard-enchanter": return PickPropVariant(variant, 4, 6, 7, 15);
                case "midgaard-gate":
                case "midgaard-guard": return PickPropVariant(variant, 9, 19, 0, 15);
                case "midgaard-king": return PickPropVariant(variant, 15, 19, 6, 9);
                case "midgaard-sewer": return PickPropVariant(variant, 10, 0, 16, 17);
                case "midgaard-ratquest": return PickPropVariant(variant, 10, 11, 18, 0);
                case "midgaard-paved": return PickPropVariant(variant, 0, 3, 16, 19);
                default: return -1;
            }
        }

        private int MidgaardAmbientPropAtlasIndex(string kind, int x, int y, int roll)
        {
            if (roll > (exploreWideView ? 7 : 12)) return -1;
            int variant = ExploreNoise(x, y, 87) % 4;
            switch (kind)
            {
                case "midgaard-market": return PickPropVariant(variant, 1, 2, 3, 14);
                case "midgaard-temple":
                case "midgaard-fountain":
                case "midgaard-recall": return PickPropVariant(variant, 0, 8, 11, 12);
                case "midgaard-diner":
                case "midgaard-provisions": return PickPropVariant(variant, 2, 3, 14, 18);
                case "midgaard-tavern": return PickPropVariant(variant, 4, 6, 14, 19);
                case "midgaard-armorer": return PickPropVariant(variant, 2, 3, 16, 17);
                case "midgaard-weapons": return PickPropVariant(variant, 2, 3, 15, 16);
                case "midgaard-enchanter": return PickPropVariant(variant, 0, 11, 17, 12);
                case "midgaard-gate":
                case "midgaard-guard": return PickPropVariant(variant, 0, 5, 13, 6);
                case "midgaard-king": return PickPropVariant(variant, 0, 5, 13, 11);
                case "midgaard-sewer": return PickPropVariant(variant, 9, 10, 2, 3);
                case "midgaard-ratquest": return PickPropVariant(variant, 9, 2, 3, 19);
                case "midgaard-paved": return PickPropVariant(variant, 0, 4, 6, 12);
                default: return -1;
            }
        }

        private int PickPropVariant(int variant, int a, int b, int c, int d)
        {
            switch (variant & 3)
            {
                case 0: return a;
                case 1: return b;
                case 2: return c;
                default: return d;
            }
        }

        private void DrawMidgaardPavingDecal(Rect cell, int x, int y, int tile, string kind)
        {
            if (state?.Map == null || state.Depth != 1 || tile == 0 || !IsMidgaardCityCell(x, y, state.Map, state.Depth)) return;
            if (HasNearbyMidgaardFocusObject(x, y)) return;
            kind = kind ?? "";
            if (!kind.StartsWith("midgaard")) return;

            bool hasObject = ObjectAt(state.Map, x, y) != null;
            int distance = Distance(x, y, state.PlayerX, state.PlayerY);
            int roll = ExploreNoise(x, y, 101);
            if (!ExplorationReadabilityRules.ShouldDrawMidgaardPavingDecal(exploreWideView, distance, hasObject, roll)) return;

            int propRoll = ExploreNoise(x, y, 79) % 100;
            if (MidgaardAmbientPropAtlasIndex(kind, x, y, propRoll) >= 0) return;

            int index = MidgaardPavingDecalAtlasIndex(kind, x, y);
            if (index < 0) return;
            float noise = (ExploreNoise(x, y, 109) % 11) / 10f;
            Color tint = Color.white.WithAlpha(ExplorationReadabilityRules.MidgaardPavingDecalAlpha(noise));
            TryDrawMidgaardPavingDecalAtlasIcon(Pad(cell, cell.width * 0.10f), index, tint);
        }

        private int MidgaardPavingDecalAtlasIndex(string kind, int x, int y)
        {
            int variant = ExploreNoise(x, y, 107) % 4;
            switch (kind ?? "")
            {
                case "midgaard-market": return PickPropVariant(variant, 6, 7, 13, 15);
                case "midgaard-temple":
                case "midgaard-fountain":
                case "midgaard-recall": return PickPropVariant(variant, 8, 9, 14, 2);
                case "midgaard-diner":
                case "midgaard-provisions":
                case "midgaard-tavern": return PickPropVariant(variant, 2, 3, 7, 15);
                case "midgaard-armorer":
                case "midgaard-weapons":
                case "midgaard-enchanter": return PickPropVariant(variant, 1, 10, 11, 12);
                case "midgaard-gate":
                case "midgaard-guard":
                case "midgaard-king": return PickPropVariant(variant, 0, 9, 10, 12);
                case "midgaard-sewer":
                case "midgaard-ratquest": return PickPropVariant(variant, 0, 1, 2, 11);
                case "midgaard-paved": return PickPropVariant(variant, 2, 3, 6, 11);
                default: return -1;
            }
        }

        private void DrawMidgaardAmbientPropFallback(Rect rect, int index, Color tint)
        {
            Color accent = tint;
            switch (index)
            {
                case 0:
                    DrawRect(new Rect(rect.center.x - rect.width * 0.05f, rect.y + rect.height * 0.12f, rect.width * 0.10f, rect.height * 0.68f), stone.WithAlpha(accent.a));
                    DrawRect(new Rect(rect.x + rect.width * 0.32f, rect.y + rect.height * 0.04f, rect.width * 0.36f, rect.height * 0.24f), ember.WithAlpha(accent.a));
                    break;
                case 9:
                    DrawRect(Pad(rect, rect.width * 0.18f), Hex("050708", accent.a));
                    DrawBorder(Pad(rect, rect.width * 0.18f), stone.WithAlpha(accent.a), 1);
                    for (int i = 0; i < 3; i++) DrawRect(new Rect(rect.x + rect.width * (0.30f + i * 0.16f), rect.y + rect.height * 0.30f, rect.width * 0.04f, rect.height * 0.42f), stone.WithAlpha(accent.a));
                    break;
                default:
                    DrawRect(Pad(rect, rect.width * 0.24f), gold.WithAlpha(accent.a * 0.75f));
                    DrawBorder(Pad(rect, rect.width * 0.24f), stone.WithAlpha(accent.a), 1);
                    break;
            }
        }

        private void DrawExploreBiomeAmbientProp(Rect cell, int x, int y, int tile, string kind)
        {
            if (state?.Map == null || tile == 0) return;
            if (ObjectAt(state.Map, x, y) != null) return;
            if (x == state.PlayerX && y == state.PlayerY) return;
            kind = kind ?? "";
            if (kind.StartsWith("midgaard")) return;

            int roll = ExploreNoise(x, y, 191) % 100;
            if (!ExplorationReadabilityRules.ShouldDrawBiomeAmbientProp(kind, exploreWideView, roll)) return;
            if (roll > Mathf.RoundToInt(BiomeAmbientDensity(kind) * ExploreDecorativeDensityScale())) return;

            int variant = ExploreNoise(x, y, 193) % 4;
            int index = BiomeAmbientPropAtlasIndex(kind, variant);
            float noise = (ExploreNoise(x, y, 197) % 7) / 6f;
            Color tint = BiomeAmbientTint(kind).WithAlpha(ExplorationReadabilityRules.BiomePropAlpha(exploreWideView, noise));
            Rect propRect = Pad(cell, cell.width * BiomeAmbientPadding(kind));
            if (index >= 0 && TryDrawWorldMapBiomePropAtlasIcon(propRect, index, tint))
            {
                return;
            }

            int legacyIndex = LegacyBiomeWorldMapPropAtlasIndex(kind, variant);
            if (legacyIndex >= 0 && TryDrawWorldMapPropAtlasIcon(propRect, legacyIndex, tint))
            {
                return;
            }

            DrawBiomeAmbientPropFallback(propRect, kind, variant, tint);
        }

        private int BiomeAmbientDensity(string kind)
        {
            switch (kind ?? "")
            {
                case "moss": return 20;
                case "mire": return 18;
                case "mud": return 14;
                case "quarry": return 17;
                case "glass": return 11;
                case "ash":
                case "red": return 16;
                case "road":
                case "paved": return 6;
                case "ruins": return 14;
                case "gloam": return 12;
                case "cistern": return 10;
                case "outworks": return 9;
                default: return 9;
            }
        }

        private float ExploreDecorativeAlphaScale()
        {
            return ExplorationReadabilityRules.DecorativeAlphaScale(exploreWideView);
        }

        private float ExploreDecorativeDensityScale()
        {
            return ExplorationReadabilityRules.DecorativeDensityScale(exploreWideView);
        }

        private float BiomeAmbientPadding(string kind)
        {
            switch (kind ?? "")
            {
                case "moss": return 0.21f;
                case "mire": return 0.22f;
                case "quarry": return 0.20f;
                case "ash":
                case "red": return 0.21f;
                default: return 0.24f;
            }
        }

        private int BiomeAmbientPropAtlasIndex(string kind, int variant)
        {
            switch (kind ?? "")
            {
                case "moss": return PickPropVariant(variant, 0, 1, 3, 16);
                case "mire": return PickPropVariant(variant, 5, 7, 6, 18);
                case "mud": return PickPropVariant(variant, 2, 3, 19, 13);
                case "quarry": return PickPropVariant(variant, 8, 9, 13, 14);
                case "glass": return PickPropVariant(variant, 12, 8, 13, 19);
                case "ash":
                case "red": return PickPropVariant(variant, 10, 11, 4, 14);
                case "road":
                case "paved": return PickPropVariant(variant, 15, 17, 8, 19);
                case "ruins": return PickPropVariant(variant, 8, 9, 13, 14);
                case "gloam": return PickPropVariant(variant, 9, 13, 14, 4);
                case "cistern": return PickPropVariant(variant, 5, 6, 9, 14);
                case "outworks": return PickPropVariant(variant, 2, 8, 15, 19);
                default: return -1;
            }
        }

        private int LegacyBiomeWorldMapPropAtlasIndex(string kind, int variant)
        {
            switch (kind ?? "")
            {
                case "moss": return PickPropVariant(variant, 10, 11, 9, 1);
                case "mire": return PickPropVariant(variant, 6, 8, 2, 10);
                case "mud": return PickPropVariant(variant, 2, 6, 9, 1);
                case "quarry": return PickPropVariant(variant, 9, 11, 13, 15);
                case "glass": return PickPropVariant(variant, 11, 5, 13, 9);
                case "ash":
                case "red": return PickPropVariant(variant, 17, 9, 11, 8);
                case "road":
                case "paved": return PickPropVariant(variant, 1, 2, 6, 13);
                case "ruins":
                case "gloam": return PickPropVariant(variant, 9, 11, 13, 15);
                case "cistern": return PickPropVariant(variant, 6, 8, 10, 13);
                case "outworks": return PickPropVariant(variant, 1, 2, 9, 13);
                default: return -1;
            }
        }

        private Color BiomeAmbientTint(string kind)
        {
            switch (kind ?? "")
            {
                case "moss": return Hex("e8f2df");
                case "mire": return Hex("d9eee4");
                case "mud": return Hex("f0e0ce");
                case "quarry": return Hex("e7e8e2");
                case "glass": return Hex("e5f5ff");
                case "ash":
                case "red": return Hex("f1d8cd");
                case "road":
                case "paved": return Hex("f2e7cf");
                case "ruins": return Hex("ebebe4");
                case "gloam": return Hex("ece5f5");
                case "cistern": return Hex("dff3ef");
                case "outworks": return Hex("f0e5d4");
                default: return Color.white;
            }
        }

        private void DrawBiomeAmbientPropFallback(Rect rect, string kind, int variant, Color tint)
        {
            Color accent = tint;
            switch (kind ?? "")
            {
                case "moss":
                    DrawRect(new Rect(rect.center.x - rect.width * 0.035f, rect.y + rect.height * 0.42f, rect.width * 0.07f, rect.height * 0.34f), Hex("5b3a25", accent.a));
                    DrawRect(new Rect(rect.x + rect.width * 0.22f, rect.y + rect.height * 0.18f, rect.width * 0.56f, rect.height * 0.30f), moss.WithAlpha(accent.a));
                    DrawRect(new Rect(rect.x + rect.width * 0.30f, rect.y + rect.height * 0.08f, rect.width * 0.40f, rect.height * 0.22f), Hex("9fbe6b", accent.a * 0.86f));
                    break;
                case "mire":
                    DrawRect(new Rect(rect.x + rect.width * 0.12f, rect.y + rect.height * 0.68f, rect.width * 0.76f, rect.height * 0.08f), poison.WithAlpha(accent.a * 0.55f));
                    DrawRect(new Rect(rect.x + rect.width * 0.24f, rect.y + rect.height * 0.36f, rect.width * 0.06f, rect.height * 0.36f), Hex("7f9d5b", accent.a));
                    DrawRect(new Rect(rect.x + rect.width * 0.48f, rect.y + rect.height * 0.28f, rect.width * 0.06f, rect.height * 0.44f), Hex("7f9d5b", accent.a));
                    DrawRect(new Rect(rect.x + rect.width * 0.66f, rect.y + rect.height * 0.46f, rect.width * 0.08f, rect.height * 0.26f), Hex("8fc27b", accent.a));
                    break;
                case "quarry":
                    DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.58f, rect.width * 0.64f, rect.height * 0.14f), stone.WithAlpha(accent.a));
                    DrawRect(new Rect(rect.x + rect.width * 0.34f, rect.y + rect.height * 0.38f, rect.width * 0.32f, rect.height * 0.20f), Hex("68706b", accent.a));
                    DrawRect(new Rect(rect.x + rect.width * 0.58f, rect.y + rect.height * 0.46f, rect.width * 0.20f, rect.height * 0.16f), Hex("303936", accent.a));
                    break;
                case "ash":
                case "red":
                    DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.68f, rect.width * 0.64f, rect.height * 0.08f), Hex("050708", accent.a * 0.55f));
                    DrawRect(new Rect(rect.x + rect.width * 0.42f, rect.y + rect.height * 0.34f, rect.width * 0.08f, rect.height * 0.36f), Hex("24150e", accent.a));
                    DrawRect(new Rect(rect.x + rect.width * 0.52f, rect.y + rect.height * 0.18f, rect.width * 0.10f, rect.height * 0.28f), ember.WithAlpha(accent.a));
                    break;
                default:
                    DrawRect(Pad(rect, rect.width * 0.34f), accent.WithAlpha(accent.a * 0.74f));
                    DrawBorder(Pad(rect, rect.width * 0.34f), Hex("050708", accent.a * 0.46f), 1);
                    break;
            }
        }

        private bool IsMidgaardNpcObject(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.MarketClerk:
                case ObjectType.TempleHealer:
                case ObjectType.TavernKeeper:
                case ObjectType.GateCaptain:
                case ObjectType.CityCourier:
                case ObjectType.WoundedTraveler:
                case ObjectType.StableHand:
                case ObjectType.RoyalHerald:
                case ObjectType.NoviceHealer:
                case ObjectType.OldRoadScout:
                case ObjectType.KingHalvard:
                case ObjectType.ArmorerNpc:
                case ObjectType.WeaponMerchantNpc:
                case ObjectType.EnchanterNpc:
                case ObjectType.DinerCook:
                case ObjectType.Provisioner:
                case ObjectType.DockWorker:
                case ObjectType.Scholar:
                    return true;
                default:
                    return false;
            }
        }

        private bool IsQuietExploreObject(ObjectType type)
        {
            return type == ObjectType.CityWall
                || type == ObjectType.TownGuard
                || IsMidgaardNpcObject(type)
                || IsMidgaardInteriorDecoration(type);
        }

        private bool ShouldFrameExploreObject(ObjectType type, MapObject obj)
        {
            if (obj != null && IsCurrentMidgaardObjective(obj)) return true;
            if (obj != null && state != null && Distance(obj.X, obj.Y, state.PlayerX, state.PlayerY) <= 1) return true;
            if (type == ObjectType.Encounter || type == ObjectType.Stairs || type == ObjectType.Cave || type == ObjectType.Shrine || type == ObjectType.Cache || type == ObjectType.Camp) return true;
            if (type == ObjectType.KingHall || type == ObjectType.Sewer || type == ObjectType.RatPeltQuest || type == ObjectType.RecallCircle) return true;
            if (type == ObjectType.InteriorDoor || type == ObjectType.KingHalvard) return true;
            return ContentSetCatalog.ShowPrototypeScaffold(activeContentSet) && IsRouteScaffoldObject(type);
        }

        private float ExploreObjectArtPadding(ObjectType type, bool quiet)
        {
            if (type == ObjectType.CityWall) return 0.00f;
            if (type == ObjectType.NorthGate || type == ObjectType.SouthGate || type == ObjectType.EastGate || type == ObjectType.WestGate) return 0.00f;
            if (ExplorationArtRules.IsMidgaardBuilding(type)) return ExplorationArtRules.MidgaardBuildingArtPadding();
            if (type == ObjectType.InteriorDoor || IsMidgaardInteriorDecoration(type)) return 0.02f;
            if (quiet) return 0.03f;
            return 0.03f;
        }

        private void DrawExploreObjectPlinth(Rect rect, Color color, bool quiet, bool framed, float pulse)
        {
            float shadowH = Mathf.Max(3f, rect.height * (quiet ? 0.085f : 0.105f));
            Rect shadow = new Rect(rect.x + rect.width * 0.14f, rect.yMax - shadowH - rect.height * 0.03f, rect.width * 0.72f, shadowH);
            DrawRect(shadow, Hex("020303", framed ? 0.92f : 0.84f));
            float washAlpha = ExplorationReadabilityRules.InteractiveObjectBackdropAlpha(quiet, framed);
            if (washAlpha <= 0f) return;
            Rect wash = Pad(rect, rect.width * (quiet ? 0.16f : 0.13f));
            DrawRect(wash, Hex("020303", washAlpha));
            DrawBorder(wash, color.WithAlpha(quiet ? 0.26f : 0.30f), 1);
            if (!framed) return;
            DrawRect(Pad(rect, rect.width * 0.20f), color.WithAlpha(quiet ? 0.10f : 0.09f + pulse * 0.04f));
        }

        private void DrawExploreObjectFrame(Rect rect, Color color, bool quiet, bool framed, float pulse)
        {
            if (!framed) return;
            float pad = quiet ? rect.width * 0.11f : rect.width * 0.07f;
            DrawBorder(Pad(rect, pad), color.WithAlpha(quiet ? 0.86f : 0.80f + pulse * 0.14f), quiet ? 1 : 2);
        }

        private void DrawExploreArtDebugOverlay(Rect cell, Rect artRect, string label)
        {
            DrawBorder(cell, Hex("f3ead7", 0.34f), 1);
            DrawBorder(artRect, Hex("78d8c5", 0.74f), 1);
            float marker = Mathf.Max(3f, cell.width * 0.045f);
            DrawRect(new Rect(cell.center.x - marker * 0.5f, cell.center.y - marker * 0.5f, marker, marker), Hex("f3ead7", 0.72f));
            Rect tag = new Rect(cell.x + 2f, cell.y + 2f, Mathf.Min(cell.width - 4f, 74f), Mathf.Max(12f, cell.height * 0.16f));
            DrawRect(tag, Hex("030405", 0.82f));
            GUI.Label(tag, FitText(label, tag.width - 4f, CenterLeftStyle(8, cursorWhite)), CenterLeftStyle(8, cursorWhite));
        }

        private void DrawExploreObjectiveMarker(Rect rect, MapObject obj)
        {
            if (!IsCurrentMidgaardObjective(obj)) return;
            Color accent = MidgaardObjectiveColor();
            float pulse = state != null && state.ReducedMotion ? 0.72f : 0.72f + Mathf.Sin(Time.time * 5.0f) * 0.18f;
            Rect ring = Pad(rect, rect.width * 0.08f);
            DrawBorder(ring, accent.WithAlpha(Mathf.Clamp01(pulse)), 2);
            DrawBorder(Pad(ring, rect.width * 0.08f), cursorWhite.WithAlpha(0.48f), 1);
            Rect pip = new Rect(ring.xMax - rect.width * 0.28f, ring.y + rect.height * 0.04f, rect.width * 0.20f, rect.height * 0.20f);
            DrawRect(pip, Hex("050708", 0.86f));
            DrawBorder(pip, accent, 1);
            if (!TryDrawWorldMapProgressionOverlayAtlasIcon(Pad(pip, -pip.width * 0.06f), 2, Color.white.WithAlpha(0.92f)))
            {
                DrawPixelCross(Pad(pip, pip.width * 0.22f), accent);
            }
        }

        private void DrawExploreProgressionMarker(Rect rect, MapObject obj)
        {
            if (obj == null) return;
            // Gate silhouettes and the wall line already communicate their role.
            // Objective and nearby cues remain available without a permanent icon
            // pasted over the gatehouse art.
            if (IsMidgaardGateType(obj.Type)) return;
            int index = ProgressionOverlayIndex(obj);
            if (index < 0) return;
            Color accent = ProgressionOverlayColor(obj);
            Rect mark = new Rect(rect.x + rect.width * 0.04f, rect.yMax - rect.height * 0.30f, rect.width * 0.25f, rect.height * 0.25f);
            DrawRect(mark, Hex("030405", 0.72f));
            DrawBorder(mark, accent.WithAlpha(0.76f), 1);
            if (!TryDrawWorldMapProgressionOverlayAtlasIcon(Pad(mark, -mark.width * 0.08f), index, Color.white.WithAlpha(0.88f)))
            {
                DrawTinyUiIcon(Pad(mark, mark.width * 0.18f), obj.Type == ObjectType.Encounter ? "enemy" : obj.Type == ObjectType.Stairs ? "arrow" : "scroll", accent);
            }
        }

        private void DrawExploreNearbyObjectCue(Rect cell, MapObject obj)
        {
            if (obj == null || state == null) return;
            if (!ShouldShowNearbyExploreCue(obj)) return;
            if (ReferenceEquals(CurrentExploreInteraction().Target, obj)) return;

            int distance = Distance(obj.X, obj.Y, state.PlayerX, state.PlayerY);
            bool currentWork = IsCurrentMidgaardObjective(obj);
            Color accent = currentWork ? MidgaardObjectiveColor() : ObjectColor(obj.Type);
            float pulse = state.ReducedMotion ? 0.72f : 0.72f + Mathf.Sin(Time.time * (currentWork ? 5.2f : 3.6f)) * 0.16f;
            float pad = currentWork ? cell.width * 0.035f : cell.width * 0.11f;
            Rect cue = Pad(cell, pad);
            DrawCornerBrackets(cue, accent.WithAlpha(currentWork ? Mathf.Clamp01(pulse) : 0.68f), currentWork ? 3f : 2f, cell.width * (currentWork ? 0.24f : 0.16f));

            if (distance <= 1)
            {
                Rect pip = new Rect(cue.xMax - cell.width * 0.16f, cue.y + cell.height * 0.055f, cell.width * 0.11f, cell.width * 0.11f);
                DrawRect(pip, Hex("030405", 0.82f));
                DrawBorder(pip, accent.WithAlpha(0.86f), 1);
                DrawRect(Pad(pip, pip.width * 0.34f), accent.WithAlpha(0.78f));
            }
        }

        private void DrawExploreUseTargetCue(Rect grid, float cell, Point origin, int viewW, int viewH)
        {
            ExplorationInteraction interaction = CurrentExploreInteraction();
            MapObject obj = interaction.Target;
            if (!interaction.HasTarget) return;
            if (!ExplorePointInViewport(obj.X, obj.Y, origin, viewW, viewH)) return;

            Rect tile = new Rect(grid.x + (obj.X - origin.X) * cell, grid.y + (obj.Y - origin.Y) * cell, cell, cell);
            Color accent = IsCurrentMidgaardObjective(obj) ? MidgaardObjectiveColor() : ObjectColor(obj.Type);
            float pulse = state.ReducedMotion ? 0.78f : 0.78f + Mathf.Sin(Time.time * 5.4f) * 0.18f;
            Rect ring = Pad(tile, cell * 0.045f);
            DrawCornerBrackets(ring, accent.WithAlpha(Mathf.Clamp01(pulse)), 3f, cell * 0.22f);
            DrawBorder(Pad(ring, cell * 0.055f), cursorWhite.WithAlpha(0.30f), 1);

            Rect chip = new Rect(ring.xMax - cell * 0.31f, ring.y + cell * 0.055f, cell * 0.26f, cell * 0.16f);
            DrawRect(chip, Hex("030405", 0.86f));
            DrawBorder(chip, accent.WithAlpha(0.92f), 1);
            GUI.Label(chip, "E", CenterStyle(Mathf.RoundToInt(Mathf.Clamp(cell * 0.10f, 8f, 12f)), ink));
        }

        private bool ShouldShowNearbyExploreCue(MapObject obj)
        {
            if (obj == null || state == null) return false;
            if (obj.Type == ObjectType.CityWall || obj.Type == ObjectType.TownGuard) return false;
            if (IsCurrentMidgaardObjective(obj)) return true;
            int distance = Distance(obj.X, obj.Y, state.PlayerX, state.PlayerY);
            if (distance <= 1) return true;
            return obj.Type == ObjectType.Stairs
                || obj.Type == ObjectType.Cave
                || obj.Type == ObjectType.Encounter
                || obj.Type == ObjectType.Shrine
                || obj.Type == ObjectType.Cache
                || IsRouteScaffoldObject(obj.Type);
        }

        private void DrawExplorePlayerLocator(Rect cell, Rect tokenRect, Color color)
        {
            if (state == null) return;
            float pulse = state.ReducedMotion ? 0.65f : 0.65f + Mathf.Sin(Time.time * 5.6f) * 0.18f;
            Color accent = Color.Lerp(gold, teal, Mathf.Clamp01(pulse)).WithAlpha(0.92f);
            Rect outer = Pad(cell, cell.width * 0.10f);
            DrawCornerBrackets(outer, accent, 2f, cell.width * 0.17f);

            Rect baseLine = new Rect(cell.center.x - cell.width * 0.18f, cell.yMax - cell.height * 0.09f, cell.width * 0.36f, Mathf.Max(2f, cell.height * 0.026f));
            DrawRect(baseLine, Hex("030405", 0.78f));
            DrawRect(new Rect(baseLine.x + baseLine.width * 0.10f, baseLine.y, baseLine.width * 0.80f, baseLine.height), Color.Lerp(color, ink, 0.20f).WithAlpha(0.88f));
        }

        private void DrawCornerBrackets(Rect rect, Color color, float thickness, float length)
        {
            thickness = Mathf.Max(1f, thickness);
            length = Mathf.Clamp(length, 4f, Mathf.Min(rect.width, rect.height) * 0.45f);

            DrawRect(new Rect(rect.x, rect.y, length, thickness), color);
            DrawRect(new Rect(rect.x, rect.y, thickness, length), color);
            DrawRect(new Rect(rect.xMax - length, rect.y, length, thickness), color);
            DrawRect(new Rect(rect.xMax - thickness, rect.y, thickness, length), color);
            DrawRect(new Rect(rect.x, rect.yMax - thickness, length, thickness), color);
            DrawRect(new Rect(rect.x, rect.yMax - length, thickness, length), color);
            DrawRect(new Rect(rect.xMax - length, rect.yMax - thickness, length, thickness), color);
            DrawRect(new Rect(rect.xMax - thickness, rect.yMax - length, thickness, length), color);
        }

        private int ProgressionOverlayIndex(MapObject obj)
        {
            if (obj == null) return -1;
            if (IsCurrentMidgaardObjective(obj)) return 2;
            switch (obj.Type)
            {
                case ObjectType.Camp: return 5;
                case ObjectType.Shrine: return 8;
                case ObjectType.Obelisk: return 6;
                case ObjectType.Ruin: return 3;
                case ObjectType.Encounter: return 13;
                case ObjectType.Stairs: return state != null && state.Depth >= FinalBossDepth - 1 ? 19 : 16;
                case ObjectType.Cave: return state != null && state.Depth == 2 && ZoneIdFor(obj.X, obj.Y, state.Map, state.Depth) == "dusk-market" ? 17 : 11;
                case ObjectType.Town:
                case ObjectType.Temple:
                case ObjectType.Fountain:
                case ObjectType.RecallCircle: return 15;
                case ObjectType.KingHall:
                case ObjectType.KingHalvard: return 4;
                case ObjectType.NorthGate:
                case ObjectType.SouthGate:
                case ObjectType.EastGate:
                case ObjectType.WestGate: return 11;
                case ObjectType.QuestBoard: return 9;
                case ObjectType.Waystone: return 15;
                case ObjectType.TrainingGround: return 9;
                case ObjectType.LoreLibrary: return 6;
                case ObjectType.ForgeSite: return 10;
                case ObjectType.FactionCamp: return 3;
                case ObjectType.DungeonGate:
                case ObjectType.DeepCrypt: return 11;
                case ObjectType.AncientGrove: return 8;
                case ObjectType.PortalSeal: return 19;
                default: return -1;
            }
        }

        private Color ProgressionOverlayColor(MapObject obj)
        {
            if (obj == null) return gold;
            switch (obj.Type)
            {
                case ObjectType.Encounter: return blood;
                case ObjectType.Shrine:
                case ObjectType.Temple:
                case ObjectType.Fountain:
                case ObjectType.RecallCircle: return teal;
                case ObjectType.Camp: return ember;
                case ObjectType.Stairs:
                case ObjectType.Cave: return frost;
                case ObjectType.KingHall: return gold;
                default: return ObjectColor(obj.Type);
            }
        }

        private bool IsCurrentMidgaardObjective(MapObject obj)
        {
            if (obj == null || !TryCurrentMidgaardObjectiveType(out ObjectType type)) return false;
            return obj.Type == type
                || type == ObjectType.KingHall && obj.Type == ObjectType.KingHalvard
                || type == ObjectType.Armorer && (obj.Type == ObjectType.RatPeltQuest || obj.Type == ObjectType.ArmorerNpc);
        }

        private bool IsCurrentMidgaardObjectiveType(ObjectType target)
        {
            return TryCurrentMidgaardObjectiveType(out ObjectType type) && (type == target || type == ObjectType.Armorer && target == ObjectType.RatPeltQuest);
        }

        private Color MidgaardObjectiveColor()
        {
            if (HasStoryFlag(StoryFlags.MidgaardLampRoundStarted) && !HasStoryFlag(StoryFlags.MidgaardLampRoundComplete)) return teal;
            if (HasStoryFlag(StoryFlags.MidgaardGateSurveyStarted) && !HasStoryFlag(StoryFlags.MidgaardGateSurveyComplete)) return gold;
            if (!HasStoryFlag(StoryFlags.MidgaardRatQuestGiven)) return gold;
            if (!ContentSetCatalog.IsFullPrototype(activeContentSet) && !HasStoryFlag(StoryFlags.SewerCisternDenCleared)) return blood;
            if (!HasStoryFlag(StoryFlags.MidgaardRatPeltArmorMade) && !MidgaardRatPeltsReady()) return blood;
            if (!HasStoryFlag(StoryFlags.MidgaardRatPeltArmorMade)) return poison;
            return teal;
        }

        private Point ExploreViewportOrigin(int viewW, int viewH)
        {
            int maxX = Mathf.Max(0, state.Map.Width - viewW);
            int maxY = Mathf.Max(0, state.Map.Height - viewH);
            int x = Mathf.Clamp(state.PlayerX - viewW / 2, 0, maxX);
            int y = Mathf.Clamp(state.PlayerY - viewH / 2, 0, maxY);
            return new Point(x, y);
        }

        private bool ExplorePointInViewport(int x, int y, Point origin, int viewW, int viewH)
        {
            return x >= origin.X && x < origin.X + viewW && y >= origin.Y && y < origin.Y + viewH;
        }

        private bool TryExploreGridToMap(Rect grid, float cell, Point origin, int viewW, int viewH, Vector2 mouse, out int x, out int y)
        {
            x = -1;
            y = -1;
            if (!grid.Contains(mouse)) return false;
            int vx = Mathf.FloorToInt((mouse.x - grid.x) / cell);
            int vy = Mathf.FloorToInt((mouse.y - grid.y) / cell);
            if (vx < 0 || vx >= viewW || vy < 0 || vy >= viewH) return false;
            x = origin.X + vx;
            y = origin.Y + vy;
            return x >= 0 && x < state.Map.Width && y >= 0 && y < state.Map.Height;
        }

        private void DrawExploreMovementHints(Rect grid, float cell, Point origin, int viewW, int viewH)
        {
            ExplorationInteraction interaction = CurrentExploreInteraction();
            Point[] steps =
            {
                new Point(state.PlayerX, state.PlayerY - 1),
                new Point(state.PlayerX - 1, state.PlayerY),
                new Point(state.PlayerX + 1, state.PlayerY),
                new Point(state.PlayerX, state.PlayerY + 1)
            };

            foreach (Point step in steps)
            {
                if (step.X < 0 || step.X >= state.Map.Width || step.Y < 0 || step.Y >= state.Map.Height) continue;
                if (!ExplorePointInViewport(step.X, step.Y, origin, viewW, viewH)) continue;
                Rect tile = new Rect(grid.x + (step.X - origin.X) * cell, grid.y + (step.Y - origin.Y) * cell, cell, cell);
                if (interaction.HasTarget && interaction.Target.X == step.X && interaction.Target.Y == step.Y) continue;
                bool passable = CanStepExplore(step.X, step.Y);
                Color color = passable ? teal : Hex("8a5c35");
                Rect cue = ExploreMovementCueRect(tile, step);
                DrawRect(Pad(cue, -1f), Hex("030405", 0.62f));
                DrawRect(cue, color.WithAlpha(passable ? 0.62f : 0.42f));
            }
        }

        private Rect ExploreMovementCueRect(Rect tile, Point step)
        {
            float thickness = Mathf.Clamp(tile.width * 0.035f, 2f, 4f);
            float length = Mathf.Clamp(tile.width * 0.18f, 10f, 20f);
            if (step.X < state.PlayerX) return new Rect(tile.xMax - thickness, tile.center.y - length * 0.5f, thickness, length);
            if (step.X > state.PlayerX) return new Rect(tile.x, tile.center.y - length * 0.5f, thickness, length);
            if (step.Y < state.PlayerY) return new Rect(tile.center.x - length * 0.5f, tile.yMax - thickness, length, thickness);
            return new Rect(tile.center.x - length * 0.5f, tile.y, length, thickness);
        }

        private void DrawExploreViewportEdgeHints(Rect grid, Point origin, int viewW, int viewH)
        {
            Color edge = Hex("d7a84e", 0.28f);
            if (origin.X > 0) DrawRect(new Rect(grid.x, grid.y + 38f, 3f, grid.height - 76f), edge);
            if (origin.X + viewW < state.Map.Width) DrawRect(new Rect(grid.xMax - 3f, grid.y + 38f, 3f, grid.height - 76f), edge);
            if (origin.Y > 0) DrawRect(new Rect(grid.x + 38f, grid.y, grid.width - 76f, 3f), edge);
            if (origin.Y + viewH < state.Map.Height) DrawRect(new Rect(grid.x + 38f, grid.yMax - 3f, grid.width - 76f, 3f), edge);
        }

        private void DrawExploreRegionStrip(Rect grid)
        {
            float scale = ExplorationHudScreenLayout.InterfaceScale(Screen.width, Screen.height);
            Rect strip = new Rect(grid.x, Mathf.Max(boardRect.y + 8f * scale, grid.y - 44f * scale), grid.width, 36f * scale);
            DrawRect(strip, Hex("030607", 0.88f));
            DrawRect(new Rect(strip.x, strip.yMax - 3f * scale, strip.width, 3f * scale), Hex("58462c", 0.68f));
            DrawBorder(strip, Hex("52605c", 0.86f), 1);
            WorldZone zone = ZoneAt(state.PlayerX, state.PlayerY);
            string region = zone.Name;
            string underfoot = ExploreUnderfootLine(state.PlayerX, state.PlayerY);
            Rect zoneIcon = new Rect(strip.x + 8f * scale, strip.y + 5f * scale, 26f * scale, 26f * scale);
            if (!TryDrawWorldMapProgressionOverlayAtlasIcon(zoneIcon, ZoneWasDiscovered(ZoneKey(state.Depth, zone.Id)) ? 0 : 1, Color.white.WithAlpha(0.78f)))
            {
                DrawTinyUiIcon(zoneIcon, "scroll", ZoneDangerColor(zone));
            }
            float textStart = zoneIcon.xMax + 9f * scale;
            float leftW = Mathf.Clamp(strip.width * 0.23f, 154f * scale, 300f * scale);
            float dangerW = Mathf.Clamp(strip.width * 0.13f, 92f * scale, 150f * scale);
            float rightW = Mathf.Clamp(strip.width * 0.24f, 224f * scale, 340f * scale);
            float midX = textStart + leftW + dangerW;
            float midW = Mathf.Max(100f * scale, strip.xMax - rightW - midX - 8f * scale);
            string nearbyAction = ExploreNearbyActionLine();
            string nearbyThreat = ExploreNearbyThreatLine();
            string centerLine = exploreHudCollapsed && !string.IsNullOrEmpty(exploreHoverLookLine)
                ? "Look: " + exploreHoverLookLine.Replace("\n", " / ")
                : !string.IsNullOrEmpty(nearbyAction)
                    ? nearbyAction
                    : !string.IsNullOrEmpty(nearbyThreat)
                        ? nearbyThreat
                        : underfoot;
            bool centerEmphasis = exploreHudCollapsed && !string.IsNullOrEmpty(exploreHoverLookLine)
                || !string.IsNullOrEmpty(nearbyAction)
                || !string.IsNullOrEmpty(nearbyThreat);
            int regionSize = ExplorationHudScreenLayout.FontSize(16, Screen.width, Screen.height);
            int bodySize = ExplorationHudScreenLayout.FontSize(12, Screen.width, Screen.height);
            int statusSize = ExplorationHudScreenLayout.FontSize(11, Screen.width, Screen.height);
            float lineY = strip.y + 5f * scale;
            float lineH = 27f * scale;
            GUI.Label(new Rect(textStart, lineY, leftW, lineH), FitText(region, leftW, CenterLeftStyle(regionSize, Hex("e3ba63"))), CenterLeftStyle(regionSize, Hex("e3ba63")));
            GUI.Label(new Rect(textStart + leftW, lineY, dangerW, lineH), FitText(TravelDangerLabel(zone), dangerW, CenterLeftStyle(statusSize, ZoneDangerColor(zone))), CenterLeftStyle(statusSize, ZoneDangerColor(zone)));
            GUI.Label(new Rect(midX, lineY, midW, lineH), FitText(centerLine, midW, CenterLeftStyle(bodySize, centerEmphasis ? ink : Hex("d0c5ae"))), CenterLeftStyle(bodySize, centerEmphasis ? ink : Hex("d0c5ae")));
            float viewW = rightW * 0.50f;
            Rect viewRect = new Rect(strip.xMax - rightW, lineY, viewW, lineH);
            Rect detailsRect = new Rect(viewRect.xMax + 6f * scale, lineY, rightW - viewW - 6f * scale, lineH);
            GUI.Label(viewRect, FitText(ExploreViewLabel(), viewRect.width, CenterRightStyle(statusSize, exploreWideView ? frost : teal)), CenterRightStyle(statusSize, exploreWideView ? frost : teal));
            GUI.Label(detailsRect, FitText(exploreHudCollapsed ? "Q  DETAILS" : "Q  MAP", detailsRect.width, CenterRightStyle(statusSize, exploreHudCollapsed ? teal : Hex("d0c5ae"))), CenterRightStyle(statusSize, exploreHudCollapsed ? teal : Hex("d0c5ae")));
        }

        private void UpdateExploreHoverLook(Rect grid, float cell, Point origin, int viewW, int viewH)
        {
            if (Event.current == null || !grid.Contains(Event.current.mousePosition)) return;
            if (SidePanelRect().Contains(Event.current.mousePosition)) return;
            if (!TryExploreGridToMap(grid, cell, origin, viewW, viewH, Event.current.mousePosition, out int x, out int y)) return;
            exploreHoverLookLine = ExploreLookLine(x, y);
        }

        private void DrawExploreHover(Rect grid, float cell, Point origin, int viewW, int viewH)
        {
            if (Event.current == null || !grid.Contains(Event.current.mousePosition)) return;
            if (SidePanelRect().Contains(Event.current.mousePosition)) return;
            if (!TryExploreGridToMap(grid, cell, origin, viewW, viewH, Event.current.mousePosition, out int x, out int y)) return;

            Rect tile = new Rect(grid.x + (x - origin.X) * cell, grid.y + (y - origin.Y) * cell, cell, cell);
            ExplorationInteraction interaction = CurrentExploreInteraction();
            if (interaction.HasTarget && interaction.Target.X == x && interaction.Target.Y == y) return;
            bool adjacent = Distance(x, y, state.PlayerX, state.PlayerY) <= 1;
            DrawBorder(Pad(tile, cell * 0.04f), adjacent ? cursorWhite.WithAlpha(0.86f) : frost.WithAlpha(0.48f), adjacent ? 2 : 1);

            exploreHoverLookLine = ExploreLookLine(x, y);
        }

        private void HandleExploreKeyboard()
        {
            if (TryHandleExploreMovementInput()) return;
            if (Input.GetKeyDown(KeyCode.F)) ToggleArmory(ArmoryTab.Party);
            if (Input.GetKeyDown(KeyCode.G)) ToggleArmory(ArmoryTab.Spells);
            if (Input.GetKeyDown(KeyCode.H)) UseElixir();
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E)) UseNearbyExploreObject();
            if (Input.GetKeyDown(KeyCode.R)) Camp();
            if (Input.GetKeyDown(KeyCode.Y)) RecallToTempleSquare();
            if (Input.GetKeyDown(KeyCode.T) && CanDescend()) Descend();
            if (Input.GetKeyDown(KeyCode.Tab)) ToggleExploreView();
            if (Input.GetKeyDown(KeyCode.Q)) ToggleExploreHud();
        }

        private bool TryHandleExploreMovementInput()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                TryMoveOrUseExplore(0, -1);
                return true;
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                TryMoveOrUseExplore(0, 1);
                return true;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                TryMoveOrUseExplore(-1, 0);
                return true;
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                TryMoveOrUseExplore(1, 0);
                return true;
            }

            return false;
        }

        private void TryMoveOrUseExplore(int dx, int dy)
        {
            if (Mathf.Abs(dx) + Mathf.Abs(dy) == 1)
            {
                exploreFacingX = dx;
                exploreFacingY = dy;
            }
            if (TryEngageRoamingThreatInDirection(dx, dy)) return;
            if (TryUseBlockedExploreObjectInDirection(dx, dy)) return;
            TryMoveExplore(dx, dy);
        }

        private bool TryUseBlockedExploreObjectInDirection(int dx, int dy)
        {
            if (state?.Map == null || Mathf.Abs(dx) + Mathf.Abs(dy) != 1) return false;
            int x = state.PlayerX + dx;
            int y = state.PlayerY + dy;
            if (x < 0 || y < 0 || x >= state.Map.Width || y >= state.Map.Height) return false;
            if (CanStepExplore(x, y)) return false;

            MapObject obj = ObjectAt(state.Map, x, y);
            if (!ShouldResolveExploreObjectFromAdjacent(obj)) return false;
            ResolveExploreObject(obj);
            return true;
        }

        private void TryMoveExplore(int dx, int dy)
        {
            ExplorationController controller = ExploreController();
            if (controller == null || !controller.TryMove(dx, dy, out ExplorationMoveResult move))
            {
                PushLog(ExploreBlockedMoveLine(dx, dy), Tone.Warn);
                PlaySfx("blocked");
                return;
            }

            CompleteExploreMove(move);
        }

        private string ExploreBlockedMoveLine(int dx, int dy)
        {
            if (state?.Map == null) return "The route is not ready.";
            if (Mathf.Abs(dx) + Mathf.Abs(dy) != 1) return "Move one step at a time.";

            int x = state.PlayerX + dx;
            int y = state.PlayerY + dy;
            if (x < 0 || y < 0 || x >= state.Map.Width || y >= state.Map.Height) return "The map ends here.";

            MapObject obj = ObjectAt(state.Map, x, y);
            if (ExplorationTraversalRules.BlocksMovement(obj))
            {
                string name = ObjectName(obj);
                if (ShouldResolveExploreObjectFromAdjacent(obj))
                {
                    string verb = ExploreContextVerb(obj, dx, dy).ToLowerInvariant();
                    return $"{name}: press Space/E to {verb}.";
                }

                return $"{name} blocks the way.";
            }

            if (TileAt(state.Map, x, y) != 1) return "Terrain blocks the way.";
            return "The path is blocked.";
        }

        private void CompleteExploreMove(ExplorationMoveResult move)
        {
            if (!move.Moved) return;
            string beforeRegion = ExploreRegionName(move.OldX, move.OldY);
            if (!state.ReducedMotion)
            {
                tweens.Add(new Tween("party", move.OldPosition, move.NewPosition, Time.time, 0.16f, TweenKind.Move));
            }
            ExplorationMaterial stepMaterial = ExploreMaterialAt(state.PlayerX, state.PlayerY);
            string stepSfx = GameAudioCueRules.FootstepFor(stepMaterial);
            PlaySfxSpatial(
                stepSfx,
                GameAudioCueRules.FootstepVolume(stepMaterial),
                0f,
                GameAudioCueRules.FootstepPitch(state.PlayerX, state.PlayerY));
            string afterRegion = ExploreRegionName(state.PlayerX, state.PlayerY);
            if (afterRegion != beforeRegion && afterRegion != lastExploreRegion)
            {
                lastExploreRegion = afterRegion;
                WorldZone zone = ZoneAt(state.PlayerX, state.PlayerY);
                PushLog($"The party reaches {afterRegion}: {ZoneDangerText(zone)}.", Tone.Normal);
                DiscoverCurrentZone(false);
                ShowBanner(afterRegion);
            }
            DiscoverRegionalJunction();
            if (AdvanceRoamingThreatsAfterPartyStep()) return;
            if (MaybeTriggerKoboldAmbush()) return;
            ResolveExploreTile();
        }

        private void DiscoverRegionalJunction()
        {
            if (state?.Map == null) return;
            if (!TryRegionalJunctionAt(state.PlayerX, state.PlayerY, 1, out WorldMapJunction junction)) return;
            if (state.DiscoveredZones == null) state.DiscoveredZones = new List<string>();
            string key = RegionalJunctionKey(state.Depth, junction.Id);
            if (state.DiscoveredZones.Contains(key)) return;

            state.DiscoveredZones.Add(key);
            PushLog($"{junction.Name}: {junction.Summary}", Tone.Good);
            AwardWorldExperience(5 + Mathf.Min(5, state.Depth), $"{junction.Name} charted");
            ShowBanner(junction.Name);
            PlaySfxSpatial("wayfind", 0.70f, CombatAudioMixRules.StereoPanForColumn(state.PlayerX, state.Map.Width) * 0.45f, 1f);
        }

        private bool TryRegionalJunctionAt(int x, int y, int radius, out WorldMapJunction junction)
        {
            if (state?.Map == null)
            {
                junction = default;
                return false;
            }

            return WorldMapGenerationRules.TryFindRegionalJunction(
                state.Map.Width,
                state.Map.Height,
                state.Map.StartX,
                state.Map.StartY,
                x,
                y,
                radius,
                out junction);
        }

        private string RegionalJunctionKey(int depth, string junctionId)
        {
            return RouteChartRules.DiscoveryKey(depth, junctionId);
        }

        private string RegionalRouteChartCompactLine()
        {
            if (state?.Map == null || state.DiscoveredZones == null) return "";
            WorldMapJunction[] junctions = WorldMapGenerationRules.RegionalJunctions(
                state.Map.Width,
                state.Map.Height,
                state.Map.StartX,
                state.Map.StartY);
            int charted = RouteChartRules.CountCharted(junctions, state.DiscoveredZones, state.Depth);
            if (charted <= 0) return "";
            if (RouteChartRules.TryResolveWaypoint(
                junctions,
                state.DiscoveredZones,
                state.Depth,
                state.ActiveRouteWaypointKey,
                out WorldMapJunction waypoint))
            {
                IReadOnlyList<Point> path = ActiveRouteWaypointPath();
                string waypointBearing = path.Count == 1
                    ? "here"
                    : path.Count > 1
                        ? ActiveRouteWaypointFirstDirection(path) + " " + RouteChartRules.DistanceLabel(path.Count - 1)
                        : RouteChartRules.DirectionLabel(state.PlayerX, state.PlayerY, waypoint.X, waypoint.Y) + " / route blocked";
                return $"Waypoint: {waypoint.Name} {waypointBearing} / {charted}/{junctions.Length} charted.";
            }
            if (!RouteChartRules.TryNearestCharted(junctions, state.DiscoveredZones, state.Depth, state.PlayerX, state.PlayerY, out RouteChartReading nearest))
            {
                return $"Route chart: {charted}/{junctions.Length} markers.";
            }

            string bearing = nearest.Distance == 0
                ? "here"
                : nearest.Direction + " " + RouteChartRules.DistanceLabel(nearest.Distance);
            return $"Chart: {nearest.Junction.Name} {bearing} / {charted}/{junctions.Length} markers.";
        }

        private void UseNearbyExploreObject()
        {
            if (state == null || state.Mode != GameMode.Explore || !CanAcceptGameplayInput()) return;
            if (TryEngageAdjacentRoamingThreat()) return;
            ExplorationController controller = ExploreController();
            if (controller == null || !controller.TryUseContextualTarget(out ExplorationCommandResult result))
            {
                PushLog("Nothing nearby needs attention.", Tone.Warn);
                PlaySfx("blocked", 0.55f);
                return;
            }

            switch (result.Kind)
            {
                case ExplorationCommandKind.Descend:
                    Descend();
                    break;
                case ExplorationCommandKind.ResolveTile:
                    ResolveExploreTile();
                    break;
                case ExplorationCommandKind.ResolveTarget:
                    ResolveExploreObject(result.Interaction.Target);
                    break;
                case ExplorationCommandKind.Move:
                    CompleteExploreMove(result.Move);
                    break;
            }
        }

        private ExplorationInteraction CurrentExploreInteraction()
        {
            ExplorationController controller = ExploreController();
            return controller == null ? ExplorationInteraction.None : controller.CurrentInteraction();
        }

        private ExplorationController ExploreController()
        {
            if (state == null)
            {
                InvalidateExplorationController();
                return null;
            }

            MapData map = state.Map;
            if (explorationController != null
                && ReferenceEquals(explorationControllerState, state)
                && ReferenceEquals(explorationControllerMap, map))
            {
                explorationController.SetPreferredDirection(exploreFacingX, exploreFacingY);
                return explorationController;
            }

            explorationController = new ExplorationController(
                state,
                IsCurrentMidgaardObjective,
                CanStepExplore,
                ShouldResolveExploreObjectFromAdjacent,
                ExploreContextVerb,
                ExploreContextIcon,
                obj => obj == null ? "" : ObjectName(obj));
            explorationController.SetPreferredDirection(exploreFacingX, exploreFacingY);
            explorationControllerState = state;
            explorationControllerMap = map;
            return explorationController;
        }

        private bool CanStepExplore(int x, int y)
        {
            if (state?.Map == null || x < 0 || y < 0 || x >= state.Map.Width || y >= state.Map.Height) return false;
            if (TileAt(state.Map, x, y) != 1) return false;
            if (!ExplorationTraversalRules.CanStandOnObject(ObjectAt(state.Map, x, y))) return false;
            return RoamingThreatAt(x, y) == null;
        }

        private void EnsureRoamingThreats()
        {
            if (state?.Map == null) return;
            if (state.RoamingThreats == null) state.RoamingThreats = new List<RoamingThreat>();
            bool fullPrototype = ContentSetCatalog.IsFullPrototype(activeContentSet);
            IReadOnlyList<RoamingThreatDefinition> definitions = RoamingThreatCatalog.ForDepth(state.Depth, fullPrototype);
            HashSet<string> activeIds = new HashSet<string>(definitions.Select(definition => definition.Id), StringComparer.Ordinal);
            HashSet<string> retainedIds = new HashSet<string>(StringComparer.Ordinal);
            state.RoamingThreats.RemoveAll(threat =>
                threat == null
                || threat.Depth != state.Depth
                || !activeIds.Contains(threat.Id ?? "")
                || !retainedIds.Add(threat.Id));

            foreach (RoamingThreatDefinition definition in definitions)
            {
                RoamingThreat threat = state.RoamingThreats.FirstOrDefault(candidate => candidate.Id == definition.Id);
                if (threat == null)
                {
                    Point spawn = FindRoamingThreatSpawn(definition, definition.Id);
                    if (spawn == null) continue;
                    threat = new RoamingThreat
                    {
                        Id = definition.Id,
                        Name = definition.Name,
                        Archetype = definition.Archetype,
                        Depth = state.Depth,
                        X = spawn.X,
                        Y = spawn.Y,
                        HomeX = spawn.X,
                        HomeY = spawn.Y,
                        Active = true,
                        Alerted = false,
                        GraceSteps = 2
                    };
                    state.RoamingThreats.Add(threat);
                    continue;
                }

                threat.Name = definition.Name;
                threat.Archetype = definition.Archetype;
                threat.Depth = state.Depth;
                if (!IsRoamingThreatHomeAvailable(threat.HomeX, threat.HomeY, threat.Id))
                {
                    Point repairedHome = FindRoamingThreatSpawn(definition, threat.Id);
                    if (repairedHome == null)
                    {
                        state.RoamingThreats.Remove(threat);
                        continue;
                    }
                    threat.HomeX = repairedHome.X;
                    threat.HomeY = repairedHome.Y;
                }
                if (threat.Active && !IsRoamingThreatCellAvailable(threat.X, threat.Y, threat.Id))
                {
                    threat.X = threat.HomeX;
                    threat.Y = threat.HomeY;
                    threat.Alerted = false;
                    threat.GraceSteps = Mathf.Max(threat.GraceSteps, 2);
                }
            }
        }

        private Point FindRoamingThreatSpawn(RoamingThreatDefinition definition, string ignoreId)
        {
            if (state?.Map == null || definition == null) return null;
            Point routeAnchor = FindCriticalRouteAnchor(state.Map)
                ?? BestOpenNeighbor(state.Map.StartX, state.Map.StartY);
            if (routeAnchor == null) return null;
            bool[,] reachable = ExplorationTraversalRules.ReachableMask(state.Map, routeAnchor.X, routeAnchor.Y);
            List<Point> candidates = new List<Point>();
            for (int y = 1; y < state.Map.Height - 1; y++)
            for (int x = 1; x < state.Map.Width - 1; x++)
            {
                if (!reachable[x, y] || !IsRoamingThreatHomeAvailable(x, y, ignoreId)) continue;
                if (Distance(x, y, state.PlayerX, state.PlayerY) <= 1) continue;
                int distance = Distance(x, y, state.Map.StartX, state.Map.StartY);
                if (distance < 7 || distance > 24) continue;
                if (RoamingThreatOpenNeighborCount(x, y, ignoreId) < 2) continue;
                candidates.Add(new Point(x, y));
            }

            List<Point> preferred = candidates
                .Where(point => string.Equals(
                    ZoneIdFor(point.X, point.Y, state.Map, state.Depth),
                    definition.PreferredZoneId,
                    StringComparison.Ordinal))
                .ToList();
            List<Point> pool = preferred.Count > 0 ? preferred : candidates;
            return pool
                .OrderBy(point => Mathf.Abs(
                    Distance(point.X, point.Y, state.Map.StartX, state.Map.StartY)
                    - definition.TargetDistance))
                .ThenBy(point => RoamingThreatRules.SpawnScore(state.Seed, definition.Slot, point.X, point.Y))
                .FirstOrDefault();
        }

        private int RoamingThreatOpenNeighborCount(int x, int y, string ignoreId)
        {
            int count = 0;
            if (IsRoamingThreatCellAvailable(x - 1, y, ignoreId)) count++;
            if (IsRoamingThreatCellAvailable(x + 1, y, ignoreId)) count++;
            if (IsRoamingThreatCellAvailable(x, y - 1, ignoreId)) count++;
            if (IsRoamingThreatCellAvailable(x, y + 1, ignoreId)) count++;
            return count;
        }

        private bool IsRoamingThreatCellAvailable(int x, int y, string ignoreId = "")
        {
            if (state?.Map == null || x < 0 || y < 0 || x >= state.Map.Width || y >= state.Map.Height) return false;
            if (TileAt(state.Map, x, y) != 1 || ObjectAt(state.Map, x, y) != null) return false;
            if (x == state.PlayerX && y == state.PlayerY) return false;
            WorldZone zone = ZoneFor(x, y, state.Map, state.Depth);
            if (zone == null || zone.Danger <= 0) return false;
            return RoamingThreatAt(x, y, ignoreId) == null;
        }

        private bool IsRoamingThreatHomeAvailable(int x, int y, string ignoreId)
        {
            if (!IsRoamingThreatCellAvailable(x, y, ignoreId)) return false;
            return !state.RoamingThreats.Any(other =>
                other != null
                && other.Id != ignoreId
                && other.Depth == state.Depth
                && Distance(x, y, other.HomeX, other.HomeY) < 7);
        }

        private RoamingThreat RoamingThreatAt(int x, int y, string excludeId = "")
        {
            if (state?.RoamingThreats == null) return null;
            return state.RoamingThreats.FirstOrDefault(threat =>
                threat != null
                && threat.Active
                && threat.Depth == state.Depth
                && threat.Id != excludeId
                && threat.X == x
                && threat.Y == y);
        }

        private RoamingThreatBehaviorProfile RoamingThreatBehaviorFor(RoamingThreat threat)
        {
            if (threat == null) return RoamingThreatRules.DefaultProfile;
            RoamingThreatDefinition definition = RoamingThreatCatalog.Find(
                threat.Id,
                threat.Depth,
                ContentSetCatalog.IsFullPrototype(activeContentSet));
            return definition?.BehaviorProfile ?? RoamingThreatRules.DefaultProfile;
        }

        private bool TryEngageRoamingThreatInDirection(int dx, int dy)
        {
            if (state == null || state.Mode != GameMode.Explore || Mathf.Abs(dx) + Mathf.Abs(dy) != 1) return false;
            RoamingThreat threat = RoamingThreatAt(state.PlayerX + dx, state.PlayerY + dy);
            if (threat == null) return false;
            StartRoamingThreatCombat(threat);
            return true;
        }

        private bool TryEngageAdjacentRoamingThreat()
        {
            if (state?.RoamingThreats == null) return false;
            RoamingThreat threat = state.RoamingThreats
                .Where(candidate =>
                    candidate != null
                    && candidate.Active
                    && candidate.Depth == state.Depth
                    && RoamingThreatRules.IsAdjacent(candidate.X, candidate.Y, state.PlayerX, state.PlayerY))
                .OrderByDescending(candidate => candidate.Alerted)
                .ThenBy(candidate => candidate.Id)
                .FirstOrDefault();
            if (threat == null) return false;
            StartRoamingThreatCombat(threat);
            return true;
        }

        private void StartRoamingThreatCombat(RoamingThreat threat)
        {
            if (threat == null || state == null || state.Mode != GameMode.Explore) return;
            threat.Alerted = true;
            PushLog($"{threat.Name} closes the road. The party forms ranks.", Tone.Warn);
            RoamingThreatDefinition definition = RoamingThreatCatalog.Find(
                threat.Id,
                threat.Depth,
                ContentSetCatalog.IsFullPrototype(activeContentSet));
            EncounterDefinition encounter = RoamingThreatCatalog.BuildEncounter(definition, state.Depth);
            StartCombat(encounter ?? EncounterCatalog.For(EncounterId.Patrol));
            if (state.Combat != null) state.Combat.RoamingThreatId = threat.Id;
        }

        private bool AdvanceRoamingThreatsAfterPartyStep()
        {
            if (state?.Map == null || state.Mode != GameMode.Explore) return false;
            EnsureRoamingThreats();
            state.ExplorationSteps = Mathf.Max(0, state.ExplorationSteps) + 1;
            bool playerIsSafe = ZoneAt(state.PlayerX, state.PlayerY)?.Danger <= 0;

            foreach (RoamingThreat threat in state.RoamingThreats
                         .Where(candidate => candidate != null && candidate.Depth == state.Depth)
                         .OrderBy(candidate => candidate.Id)
                         .ToList())
            {
                RoamingThreatBehaviorProfile behavior = RoamingThreatBehaviorFor(threat);
                if (!threat.Active)
                {
                    if (threat.RespawnSteps > 0) threat.RespawnSteps--;
                    if (threat.RespawnSteps <= 0) RespawnRoamingThreat(threat);
                    continue;
                }

                if (threat.GraceSteps > 0) threat.GraceSteps--;
                int distance = Distance(threat.X, threat.Y, state.PlayerX, state.PlayerY);
                if (!playerIsSafe && threat.GraceSteps <= 0 && distance == 1)
                {
                    StartRoamingThreatCombat(threat);
                    return true;
                }

                if (playerIsSafe)
                {
                    threat.Alerted = false;
                    if (RoamingThreatRules.ShouldReturnHome(state.ExplorationSteps, behavior))
                    {
                        TryMoveRoamingThreat(threat, threat.HomeX, threat.HomeY, false);
                    }
                    continue;
                }

                int homeDistance = Distance(threat.X, threat.Y, threat.HomeX, threat.HomeY);
                if (RoamingThreatRules.ShouldLeash(homeDistance, behavior))
                {
                    threat.Alerted = false;
                    if (homeDistance > 0 && RoamingThreatRules.ShouldReturnHome(state.ExplorationSteps, behavior))
                    {
                        TryMoveRoamingThreat(threat, threat.HomeX, threat.HomeY, false);
                    }
                    continue;
                }

                if (RoamingThreatRules.ShouldAlert(distance, false, threat.GraceSteps, behavior))
                {
                    if (!threat.Alerted)
                    {
                        threat.Alerted = true;
                        PushLog($"{threat.Name} spots the party and begins to track its route.", Tone.Warn);
                        ShowBanner("Patrol alerted");
                        string alertCue = CreatureAudioRules.CueForArchetype(threat.Archetype, "alert");
                        PlaySfxSpatial(
                            string.IsNullOrEmpty(alertCue) ? "encounter" : alertCue,
                            0.72f,
                            GameAudioCueRules.RoamingThreatPan(threat.X, state.Map.Width),
                            1f);
                        continue;
                    }
                    if (RoamingThreatRules.ShouldPursue(state.ExplorationSteps, behavior))
                    {
                        TryMoveRoamingThreat(threat, state.PlayerX, state.PlayerY, true);
                    }
                }
                else if (distance > RoamingThreatRules.DisengageRadius(behavior))
                {
                    threat.Alerted = false;
                }

                if (!threat.Alerted
                    && Distance(threat.X, threat.Y, threat.HomeX, threat.HomeY) > 0
                    && RoamingThreatRules.ShouldReturnHome(state.ExplorationSteps, behavior))
                {
                    TryMoveRoamingThreat(threat, threat.HomeX, threat.HomeY, false);
                }

                if (threat.GraceSteps <= 0
                    && RoamingThreatRules.IsAdjacent(threat.X, threat.Y, state.PlayerX, state.PlayerY))
                {
                    StartRoamingThreatCombat(threat);
                    return true;
                }
            }

            return false;
        }

        private bool TryMoveRoamingThreat(RoamingThreat threat, int targetX, int targetY, bool stopAdjacent)
        {
            if (threat == null || state?.Map == null) return false;
            int oldX = threat.X;
            int oldY = threat.Y;
            bool moved = RoamingThreatRules.TryNextStep(
                state.Map.Width,
                state.Map.Height,
                oldX,
                oldY,
                targetX,
                targetY,
                (x, y) => IsRoamingThreatCellAvailable(x, y, threat.Id),
                (x, y) => x == state.PlayerX && y == state.PlayerY || RoamingThreatAt(x, y, threat.Id) != null,
                stopAdjacent,
                out Point next);
            if (!moved || next == null) return false;

            threat.X = next.X;
            threat.Y = next.Y;
            AddTween(threat.Id, new Vector2(oldX, oldY), new Vector2(threat.X, threat.Y), TweenKind.Move);
            ExplorationMaterial stepMaterial = ExploreMaterialAt(threat.X, threat.Y);
            string stepCue = CreatureAudioRules.CueForArchetype(threat.Archetype, "step");
            bool useSurfaceStep = string.IsNullOrEmpty(stepCue)
                || string.Equals(stepCue, "ratchitter", StringComparison.OrdinalIgnoreCase);
            string resolvedStepCue = useSurfaceStep
                ? GameAudioCueRules.FootstepFor(stepMaterial)
                : stepCue;
            PlaySfxSpatial(
                resolvedStepCue,
                useSurfaceStep ? GameAudioCueRules.RoamingThreatFootstepVolume(stepMaterial) : 0.28f,
                GameAudioCueRules.RoamingThreatPan(threat.X, state.Map.Width),
                useSurfaceStep ? GameAudioCueRules.FootstepPitch(threat.X, threat.Y) * 0.96f : 0.92f);
            return true;
        }

        private void RespawnRoamingThreat(RoamingThreat threat)
        {
            if (threat == null) return;
            RoamingThreatDefinition definition = RoamingThreatCatalog.Find(
                threat.Id,
                threat.Depth,
                ContentSetCatalog.IsFullPrototype(activeContentSet));
            Point spawn = IsRoamingThreatHomeAvailable(threat.HomeX, threat.HomeY, threat.Id)
                ? new Point(threat.HomeX, threat.HomeY)
                : FindRoamingThreatSpawn(definition, threat.Id);
            if (spawn == null)
            {
                threat.RespawnSteps = 1;
                return;
            }

            threat.HomeX = spawn.X;
            threat.HomeY = spawn.Y;
            threat.X = spawn.X;
            threat.Y = spawn.Y;
            threat.Active = true;
            threat.Alerted = false;
            threat.GraceSteps = 2;
            threat.RespawnSteps = 0;
        }

        private void ResolveRoamingThreatVictory(string threatId)
        {
            RoamingThreat threat = state?.RoamingThreats?.FirstOrDefault(candidate => candidate != null && candidate.Id == threatId);
            if (threat == null) return;
            threat.Active = false;
            threat.Alerted = false;
            threat.GraceSteps = 0;
            threat.RespawnSteps = RoamingThreatRules.DefeatRespawnSteps;
            PushLog($"{threat.Name} scatters. The road will stay quiet for a while.", Tone.Good);
        }

        private void ResolveRoamingThreatRetreat(string threatId)
        {
            RoamingThreat threat = state?.RoamingThreats?.FirstOrDefault(candidate => candidate != null && candidate.Id == threatId);
            if (threat == null) return;
            threat.Active = true;
            threat.Alerted = false;
            threat.X = threat.HomeX;
            threat.Y = threat.HomeY;
            threat.GraceSteps = RoamingThreatRules.RetreatGraceSteps;
            threat.RespawnSteps = 0;
        }

        private void DrawRoamingThreats(Rect grid, float cell, Point origin, int viewW, int viewH)
        {
            if (state?.RoamingThreats == null) return;
            foreach (RoamingThreat threat in state.RoamingThreats)
            {
                if (threat == null || !threat.Active || threat.Depth != state.Depth) continue;
                Vector2 draw = RoamingThreatDrawPosition(threat);
                if (draw.x < origin.X - 1f || draw.x >= origin.X + viewW || draw.y < origin.Y - 1f || draw.y >= origin.Y + viewH) continue;
                Rect threatCell = new Rect(
                    grid.x + (draw.x - origin.X) * cell,
                    grid.y + (draw.y - origin.Y) * cell,
                    cell,
                    cell);
                Rect token = Pad(threatCell, threatCell.width * (exploreWideView ? 0.18f : 0.08f));
                Rect shadow = new Rect(token.x + token.width * 0.17f, token.yMax - Mathf.Max(3f, token.height * 0.08f), token.width * 0.66f, Mathf.Max(3f, token.height * 0.08f));
                DrawRect(shadow, Hex("020303", 0.82f));
                int spriteIndex = RoamingThreatSpriteIndex(threat);
                WorldMapArtSpec spec = new WorldMapArtSpec(0.98f, new Vector2(0.5f, 1f), new Vector2(0f, 0.01f), true);
                if (!TryDrawRoamingThreatAtlasIcon(token, spriteIndex, Color.white)
                    && !TryDrawWorldMapTokenSpriteAtlasIcon(token, LegacyRoamingThreatSpriteIndex(threat), Color.white, spec))
                {
                    DrawToken(
                        token,
                        RoamingThreatFallbackRole(threat),
                        RoamingThreatFallbackColor(threat),
                        false,
                        "",
                        "claw");
                }
                Color frame = threat.Alerted ? blood : Hex("d7a84e");
                if (threat.Alerted)
                {
                    float pulse = state.ReducedMotion ? 0.82f : 0.66f + Mathf.Sin(Time.time * 5f) * 0.16f;
                    DrawCornerBrackets(Pad(threatCell, threatCell.width * 0.06f), frame.WithAlpha(pulse), 2f, threatCell.width * 0.18f);
                }
                else
                {
                    DrawBorder(Pad(threatCell, threatCell.width * 0.10f), frame.WithAlpha(0.52f), 1);
                }
                Rect marker = new Rect(threatCell.xMax - threatCell.width * 0.26f, threatCell.y + threatCell.height * 0.04f, threatCell.width * 0.22f, threatCell.height * 0.22f);
                DrawTinyUiIcon(marker, threat.Alerted ? "danger" : "enemy", frame);
                if (showExploreArtDebug) DrawExploreArtDebugOverlay(threatCell, token, threat.Name);
            }
        }

        private Vector2 RoamingThreatDrawPosition(RoamingThreat threat)
        {
            if (threat == null) return Vector2.zero;
            Tween tween = tweens.LastOrDefault(candidate => candidate.Id == threat.Id);
            if (tween == null) return new Vector2(threat.X, threat.Y);
            float progress = Mathf.Clamp01((Time.time - tween.Start) / tween.Duration);
            return Vector2.Lerp(tween.From, tween.To, Mathf.SmoothStep(0f, 1f, progress));
        }

        private int RoamingThreatSpriteIndex(RoamingThreat threat)
        {
            return RoamingThreatPresentationRules.SpriteIndex(threat?.Archetype);
        }

        private int LegacyRoamingThreatSpriteIndex(RoamingThreat threat)
        {
            switch ((threat?.Archetype ?? "").ToLowerInvariant())
            {
                case "kobold":
                case "kobolds":
                case "koboldshaman":
                case "koboldking": return 11;
                case "ratfolk":
                case "ratbrute":
                case "ratcleric":
                case "plaguerats":
                case "ratcaptain": return 12;
                case "drowscout":
                case "drowmage":
                case "drow": return 13;
                case "imp":
                case "boundimp":
                case "lesserdemon":
                case "greaterdemon":
                case "redgatedemon":
                case "demon": return 14;
                case "reaver":
                case "undead":
                case "bonepriest":
                case "revenant": return -1;
                case "rat":
                case "rats":
                case "ratswarm":
                default: return 10;
            }
        }

        private string RoamingThreatFallbackRole(RoamingThreat threat)
        {
            switch (RoamingThreatCatalog.FactionForArchetype(threat?.Archetype))
            {
                case RoamingThreatFaction.Kobolds: return "koboldraider";
                case RoamingThreatFaction.Drow: return "drowscout";
                case RoamingThreatFaction.Undead: return "reaver";
                case RoamingThreatFaction.Demons: return "lesserdemon";
                default: return "ratfolk";
            }
        }

        private Color RoamingThreatFallbackColor(RoamingThreat threat)
        {
            switch (RoamingThreatCatalog.FactionForArchetype(threat?.Archetype))
            {
                case RoamingThreatFaction.Kobolds: return Hex("c58b42");
                case RoamingThreatFaction.Drow: return Hex("8f7bc4");
                case RoamingThreatFaction.Undead: return Hex("8da58a");
                case RoamingThreatFaction.Demons: return blood;
                default: return Hex("b99b75");
            }
        }

        private List<Point> ReachableExploreTilesFrom(int startX, int startY)
        {
            List<Point> reachable = new List<Point>();
            if (state?.Map == null || !CanStepExplore(startX, startY)) return reachable;

            bool[,] seen = new bool[state.Map.Width, state.Map.Height];
            Queue<Point> queue = new Queue<Point>();
            seen[startX, startY] = true;
            queue.Enqueue(new Point(startX, startY));
            int[] dx = { 0, 0, -1, 1 };
            int[] dy = { -1, 1, 0, 0 };

            while (queue.Count > 0)
            {
                Point current = queue.Dequeue();
                reachable.Add(current);
                for (int i = 0; i < dx.Length; i++)
                {
                    int nx = current.X + dx[i];
                    int ny = current.Y + dy[i];
                    if (nx < 0 || ny < 0 || nx >= state.Map.Width || ny >= state.Map.Height) continue;
                    if (seen[nx, ny] || !CanStepExplore(nx, ny)) continue;
                    seen[nx, ny] = true;
                    queue.Enqueue(new Point(nx, ny));
                }
            }

            return reachable;
        }

        private int ReachableExploreTileCount(int startX, int startY)
        {
            return ReachableExploreTilesFrom(startX, startY).Count;
        }

        private bool ReachableExploreHasUsefulTarget(int startX, int startY)
        {
            return ReachableExploreHasUsefulTarget(ReachableExploreTilesFrom(startX, startY));
        }

        private bool ReachableExploreHasUsefulTarget(List<Point> reachable)
        {
            if (state?.Map == null || reachable == null || reachable.Count == 0) return false;
            int[] dx = { 0, 0, -1, 1 };
            int[] dy = { -1, 1, 0, 0 };

            foreach (Point point in reachable)
            {
                MapObject underfoot = ObjectAt(state.Map, point.X, point.Y);
                if (ExplorationInteractionRules.IsUseObject(underfoot)) return true;

                for (int i = 0; i < dx.Length; i++)
                {
                    int nx = point.X + dx[i];
                    int ny = point.Y + dy[i];
                    if (nx < 0 || ny < 0 || nx >= state.Map.Width || ny >= state.Map.Height) continue;
                    if (ExplorationTraversalRules.CanUseFromAdjacent(ObjectAt(state.Map, nx, ny))) return true;
                }
            }

            return false;
        }

        private bool IsExplorationPositionViable(int x, int y)
        {
            List<Point> reachable = ReachableExploreTilesFrom(x, y);
            if (reachable.Count == 0) return false;
            return reachable.Count > 1 || ReachableExploreHasUsefulTarget(reachable);
        }

        private Point BestExplorationRepairPointNear(int originX, int originY)
        {
            if (state?.Map == null) return null;
            Point best = null;
            int bestScore = int.MinValue;

            for (int y = 1; y < state.Map.Height - 1; y++)
            for (int x = 1; x < state.Map.Width - 1; x++)
            {
                if (!CanStepExplore(x, y)) continue;
                List<Point> reachable = ReachableExploreTilesFrom(x, y);
                if (reachable.Count == 0) continue;
                bool hasUsefulTarget = ReachableExploreHasUsefulTarget(reachable);
                int score = reachable.Count
                    + (hasUsefulTarget ? 5000 : 0)
                    - Distance(x, y, originX, originY) * 4
                    - Distance(x, y, state.Map.StartX, state.Map.StartY);
                if (score <= bestScore) continue;
                bestScore = score;
                best = new Point(x, y);
            }

            return best;
        }

        private Point PreferredMidgaardStartPoint()
        {
            if (state?.Map == null || state.Depth != 1) return null;
            int sx = Mathf.Clamp(state.Map.StartX, 0, state.Map.Width - 1);
            int sy = Mathf.Clamp(state.Map.StartY, 0, state.Map.Height - 1);
            Point[] preferred =
            {
                new Point(sx, sy + 1),
                new Point(sx, sy - 1),
                new Point(sx - 1, sy + 1),
                new Point(sx + 1, sy + 1),
                new Point(sx - 1, sy - 1),
                new Point(sx + 1, sy - 1),
                new Point(sx - 2, sy),
                new Point(sx + 2, sy)
            };

            foreach (Point point in preferred)
            {
                if (IsPreferredMidgaardStartTile(point.X, point.Y)) return point;
            }

            return BestMidgaardInteriorRepairPointNear(sx, sy);
        }

        private Point BestMidgaardInteriorRepairPointNear(int originX, int originY)
        {
            if (state?.Map == null || state.Depth != 1) return null;
            Point best = null;
            int bestScore = int.MinValue;
            int left = MidgaardLeft(state.Map);
            int right = MidgaardRight(state.Map);
            int top = MidgaardTop(state.Map);
            int bottom = MidgaardBottom(state.Map);

            for (int y = top + 1; y <= bottom - 1; y++)
            for (int x = left + 1; x <= right - 1; x++)
            {
                if (!IsPreferredMidgaardStartTile(x, y)) continue;
                List<Point> reachable = ReachableExploreTilesFrom(x, y);
                int score = reachable.Count
                    + (ReachableExploreHasUsefulTarget(reachable) ? 5000 : 0)
                    - Distance(x, y, originX, originY) * 8;
                if (score <= bestScore) continue;
                bestScore = score;
                best = new Point(x, y);
            }

            return best;
        }

        private bool IsPreferredMidgaardStartTile(int x, int y)
        {
            if (state?.Map == null || state.Depth != 1) return false;
            if (!IsMidgaardCityCell(x, y, state.Map, state.Depth)) return false;
            int left = MidgaardLeft(state.Map);
            int right = MidgaardRight(state.Map);
            int top = MidgaardTop(state.Map);
            int bottom = MidgaardBottom(state.Map);
            if (x <= left || x >= right || y <= top || y >= bottom) return false;
            if (!CanStepExplore(x, y)) return false;
            return IsExplorationPositionViable(x, y);
        }

        private void PlacePlayerAtExplorationStart()
        {
            if (state?.Map == null) return;
            int sx = Mathf.Clamp(state.Map.StartX, 0, state.Map.Width - 1);
            int sy = Mathf.Clamp(state.Map.StartY, 0, state.Map.Height - 1);
            Point preferred = PreferredMidgaardStartPoint();
            if (preferred != null)
            {
                state.PlayerX = preferred.X;
                state.PlayerY = preferred.Y;
                return;
            }

            if (IsExplorationPositionViable(sx, sy))
            {
                state.PlayerX = sx;
                state.PlayerY = sy;
                return;
            }

            Point start = BestExplorationRepairPointNear(sx, sy) ?? BestOpenNeighbor(sx, sy);
            if (start != null)
            {
                state.PlayerX = start.X;
                state.PlayerY = start.Y;
                return;
            }

            state.PlayerX = sx;
            state.PlayerY = sy;
        }

        private void RepairPlayerExplorationPosition()
        {
            if (state?.Map == null) return;
            if (IsMidgaardInteriorCell(state.PlayerX, state.PlayerY, state.Map, state.Depth)
                && IsExplorationPositionViable(state.PlayerX, state.PlayerY))
            {
                return;
            }

            Point routeAnchor = FindCriticalRouteAnchor(state.Map);
            bool[,] certified = routeAnchor == null
                ? null
                : ExplorationTraversalRules.ReachableMask(state.Map, routeAnchor.X, routeAnchor.Y);
            bool playerOnCertifiedRoute = certified != null
                && state.PlayerX >= 0
                && state.PlayerY >= 0
                && state.PlayerX < certified.GetLength(0)
                && state.PlayerY < certified.GetLength(1)
                && certified[state.PlayerX, state.PlayerY];
            if (playerOnCertifiedRoute && IsExplorationPositionViable(state.PlayerX, state.PlayerY)) return;

            Point repaired = NearestReachableExplorePoint(certified, state.PlayerX, state.PlayerY)
                ?? BestExplorationRepairPointNear(state.PlayerX, state.PlayerY)
                ?? BestOpenNeighbor(state.PlayerX, state.PlayerY)
                ?? BestOpenNeighbor(state.Map.StartX, state.Map.StartY);
            if (repaired == null) return;
            state.PlayerX = repaired.X;
            state.PlayerY = repaired.Y;
        }

        private bool ShouldResolveExploreObjectFromAdjacent(MapObject obj)
        {
            return ExplorationTraversalRules.CanUseFromAdjacent(obj);
        }

        private string ExploreNearbyActionLine()
        {
            return CurrentExploreInteraction().ActionLine;
        }

        private string ExploreNearbyThreatLine()
        {
            if (state?.RoamingThreats == null) return "";
            RoamingThreat threat = state.RoamingThreats
                .Where(candidate => candidate != null && candidate.Active && candidate.Depth == state.Depth)
                .OrderBy(candidate => Distance(candidate.X, candidate.Y, state.PlayerX, state.PlayerY))
                .ThenBy(candidate => candidate.Id)
                .FirstOrDefault();
            if (threat == null) return "";

            int distance = Distance(threat.X, threat.Y, state.PlayerX, state.PlayerY);
            RoamingThreatBehaviorProfile behavior = RoamingThreatBehaviorFor(threat);
            if (distance > RoamingThreatRules.DisengageRadius(behavior)) return "";
            string direction = RouteChartRules.DirectionLabel(state.PlayerX, state.PlayerY, threat.X, threat.Y);
            string range = RouteChartRules.DistanceLabel(distance);
            if (distance == 1) return $"{threat.Name}: adjacent {direction}. Move toward it or press E to engage.";
            if (threat.Alerted) return $"{threat.Name}: pursuing from {direction}, {range}.";
            WorldZone playerZone = ZoneAt(state.PlayerX, state.PlayerY);
            return playerZone != null && playerZone.Danger <= 0
                ? $"{threat.Name}: beyond the ward, {direction} {range}."
                : $"{threat.Name}: prowling {direction}, {range}.";
        }

        private string ExploreContextVerb(MapObject obj, int dx, int dy)
        {
            if (obj == null) return "Use";
            if (MidgaardInteriorRules.IsPortal(obj)) return obj.Type == ObjectType.InteriorDoor ? "Leave" : "Enter";
            if (obj.Type == ObjectType.TownGuard || IsMidgaardNpcObject(obj.Type)) return "Talk";
            bool underfoot = dx == 0 && dy == 0;
            switch (obj.Type)
            {
                case ObjectType.Stairs: return underfoot ? "Descend" : "Enter";
                case ObjectType.Cache: return "Loot";
                case ObjectType.Shrine:
                case ObjectType.Camp:
                case ObjectType.Temple:
                case ObjectType.Fountain:
                case ObjectType.RecallCircle:
                case ObjectType.Waystone:
                    return "Rest";
                case ObjectType.Encounter: return "Engage";
                case ObjectType.Cave:
                case ObjectType.Sewer:
                case ObjectType.DungeonGate:
                case ObjectType.DeepCrypt:
                    return "Enter";
                case ObjectType.Armorer:
                case ObjectType.WeaponVendor:
                case ObjectType.Enchanter:
                case ObjectType.Provisions:
                case ObjectType.Market:
                case ObjectType.Diner:
                    return "Shop";
                case ObjectType.ArmorerNpc:
                case ObjectType.WeaponMerchantNpc:
                case ObjectType.EnchanterNpc:
                    return "Shop";
                case ObjectType.MarketClerk:
                case ObjectType.TempleHealer:
                case ObjectType.TavernKeeper:
                case ObjectType.GateCaptain:
                case ObjectType.CityCourier:
                case ObjectType.WoundedTraveler:
                case ObjectType.StableHand:
                case ObjectType.RoyalHerald:
                case ObjectType.NoviceHealer:
                case ObjectType.OldRoadScout:
                case ObjectType.TownGuard:
                case ObjectType.KingHalvard:
                case ObjectType.QuestBoard:
                case ObjectType.RatPeltQuest:
                    return "Talk";
                default:
                    return "Use";
            }
        }

        private string ExploreContextIcon(MapObject obj)
        {
            if (obj == null) return "target";
            if (MidgaardInteriorRules.IsPortal(obj)) return obj.Type == ObjectType.KingHall ? "quest" : "market";
            if (obj.Type == ObjectType.TownGuard || IsMidgaardNpcObject(obj.Type)) return "talk";
            switch (obj.Type)
            {
                case ObjectType.Stairs: return "stairs";
                case ObjectType.Cache: return "quest";
                case ObjectType.Shrine:
                case ObjectType.Temple:
                case ObjectType.Fountain:
                case ObjectType.RecallCircle:
                case ObjectType.Camp:
                    return "heal";
                case ObjectType.Encounter: return "danger";
                case ObjectType.Cave: return "cave";
                case ObjectType.Sewer: return "sewer";
                case ObjectType.Armorer:
                case ObjectType.WeaponVendor:
                case ObjectType.Enchanter:
                case ObjectType.Provisions:
                case ObjectType.Market:
                case ObjectType.Diner:
                case ObjectType.ArmorerNpc:
                case ObjectType.WeaponMerchantNpc:
                case ObjectType.EnchanterNpc:
                    return "market";
                case ObjectType.MarketClerk:
                case ObjectType.TempleHealer:
                case ObjectType.TavernKeeper:
                case ObjectType.GateCaptain:
                case ObjectType.CityCourier:
                case ObjectType.WoundedTraveler:
                case ObjectType.StableHand:
                case ObjectType.RoyalHerald:
                case ObjectType.NoviceHealer:
                case ObjectType.OldRoadScout:
                case ObjectType.TownGuard:
                case ObjectType.KingHalvard:
                    return "talk";
                default:
                    return "target";
            }
        }

        private void HandleExploreMouse(Rect grid, float cell, Point origin, int viewW, int viewH)
        {
            Event e = Event.current;
            if (e == null || e.type != EventType.MouseDown) return;
            if (IsBoardPointerSuppressed()) return;
            if (!ScreenInputRules.ShouldRouteBoardPointer(
                    CurrentUiOverlay(),
                    UnityEngine.EventSystems.EventSystem.current != null && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(),
                    grid.Contains(e.mousePosition),
                    SidePanelRect().Contains(e.mousePosition),
                    ExploreCommandBarRect().Contains(e.mousePosition))) return;
            if (!TryExploreGridToMap(grid, cell, origin, viewW, viewH, e.mousePosition, out int x, out int y)) return;
            int dx = x - state.PlayerX;
            int dy = y - state.PlayerY;
            if (Mathf.Abs(dx) + Mathf.Abs(dy) == 1)
            {
                TryMoveOrUseExplore(dx, dy);
                e.Use();
            }
        }

        private Color ExploreTileBaseColor(int x, int y, int tile, bool visible)
        {
            if (!visible)
            {
                return tile == 0 ? Hex("05080a") : Hex("0d1110");
            }

            string kind = ExploreTileKind(x, y, tile);
            Color color;
            switch (kind)
            {
                case "midgaard-market": color = Hex("5b5548"); break;
                case "midgaard-temple": color = Hex("6a7067"); break;
                case "midgaard-fountain": color = Hex("52696a"); break;
                case "midgaard-diner": color = Hex("59402f"); break;
                case "midgaard-tavern": color = Hex("4b352b"); break;
                case "midgaard-armorer": color = Hex("54544f"); break;
                case "midgaard-weapons": color = Hex("5a4d42"); break;
                case "midgaard-enchanter": color = Hex("46395c"); break;
                case "midgaard-gate": color = Hex("4f4d45"); break;
                case "midgaardwall": color = Hex("343a3a"); break;
                case "midgaard-guard": color = Hex("464d44"); break;
                case "midgaard-king": color = Hex("5c4b31"); break;
                case "midgaard-sewer": color = Hex("263a36"); break;
                case "midgaard-provisions": color = Hex("55432b"); break;
                case "midgaard-ratquest": color = Hex("3f3528"); break;
                case "midgaard-recall": color = Hex("315e5a"); break;
                case "midgaard-road": color = Hex("494b47"); break;
                case "midgaard-plaza": color = Hex("5a5d56"); break;
                case "midgaard-paved": color = Hex("51534d"); break;
                case "road": color = Hex("5a4027"); break;
                case "outworks": color = Hex("4b4337"); break;
                case "paved": color = Hex("50534b"); break;
                case "ruins": color = Hex("4a4940"); break;
                case "gloam": color = Hex("3e4940"); break;
                case "cistern": color = Hex("304746"); break;
                case "moss": color = Hex("365637"); break;
                case "mire": color = Hex("295653"); break;
                case "mud": color = Hex("453823"); break;
                case "quarry": color = Hex("565b54"); break;
                case "glass": color = Hex("34586a"); break;
                case "ash": color = Hex("4a2b28"); break;
                case "forestwall": color = Hex("1d3d28"); break;
                case "mirewall": color = Hex("14363b"); break;
                case "cliffwall": color = Hex("343c3f"); break;
                case "redwall": color = Hex("4a1d1d"); break;
                case "ruinswall": color = Hex("343632"); break;
                default: color = tile == 0 ? Hex("202829") : ((x + y) % 2 == 0 ? floorA : floorB); break;
            }

            float grain = (ExploreNoise(x, y, 229) % 100) / 99f;
            return grain > 0.5f
                ? Color.Lerp(color, cursorWhite, (grain - 0.5f) * 0.028f)
                : Color.Lerp(color, retroBlack, (0.5f - grain) * 0.020f);
        }

        private string ExploreTileKind(int x, int y, int tile)
        {
            string interiorId = MidgaardInteriorIdAt(x, y, state?.Map, state?.Depth ?? 1);
            if (!string.IsNullOrEmpty(interiorId))
            {
                if (tile == 0) return "midgaardwall";
                return interiorId == "midgaard-throne-room" ? "midgaard-king" : "midgaard-market";
            }
            if (state?.Map != null && IsMidgaardCityCell(x, y, state.Map, state.Depth))
            {
                return MidgaardTileKind(x, y, tile);
            }

            ExplorationMaterial material = ExploreMaterialAt(x, y);
            string zoneId = ZoneIdFor(x, y, state?.Map, state?.Depth ?? 1);
            if (tile == 0)
            {
                if (material == ExplorationMaterial.DeepWater) return "mirewall";
                if (material == ExplorationMaterial.Forest) return "forestwall";
                if (material == ExplorationMaterial.Cliff) return "cliffwall";
                if (material == ExplorationMaterial.RedBasalt) return "redwall";
                if (material == ExplorationMaterial.RuinedWall) return "ruinswall";
                return "stonewall";
            }

            switch (material)
            {
                case ExplorationMaterial.PackedDirt: return "road";
                case ExplorationMaterial.CityPaving: return "paved";
                case ExplorationMaterial.MarketCobbles: return "paved";
                case ExplorationMaterial.TempleStone: return "paved";
                case ExplorationMaterial.KeepStone: return "paved";
                case ExplorationMaterial.SewerBrick: return "cistern";
                case ExplorationMaterial.QuarryStone: return "quarry";
                case ExplorationMaterial.GlassRubble: return "glass";
                case ExplorationMaterial.FenMud: return "mud";
                case ExplorationMaterial.RedAsh: return "ash";
                case ExplorationMaterial.GloamStone: return "gloam";
                case ExplorationMaterial.CisternBrick: return "cistern";
                case ExplorationMaterial.Moss: return "moss";
                case ExplorationMaterial.RuinedPaving: return "ruins";
                case ExplorationMaterial.ShallowWater: return zoneId == "salt-cisterns" ? "cistern" : "mire";
                case ExplorationMaterial.BridgeDeck: return "road";
            }

            zoneId = ExploreVisualZoneId(x, y, state?.Map, state?.Depth ?? 1);
            switch (zoneId)
            {
                case "midgaard-road": return "road";
                case "old-quarry": return "quarry";
                case "glass-warrens": return "glass";
                case "ash-fen": return ExploreNoise(x, y, 7) % 4 == 0 ? "mire" : "mud";
                case "red-gate": return "ash";
                case "gloam-courts": return "gloam";
                case "salt-cisterns": return "cistern";
                case "green-shrine-road": return "moss";
                case "dusk-market": return "ruins";
                case "inner-ash-road": return "outworks";
                default: return "paved";
            }
        }

        private string ExploreVisualZoneId(int x, int y, MapData map, int depth)
        {
            string actualId = ZoneIdFor(x, y, map, depth);
            if (map == null || actualId == "midgaard-city" || actualId == "midgaard-road") return actualId;

            float northLine = map.Height * 0.35f + ExplorationSurfaceRules.SmoothBoundaryOffset(x, 17, 2.4f);
            float southLine = map.Height * 0.65f + ExplorationSurfaceRules.SmoothBoundaryOffset(x, 31, 2.4f);
            float westLine = map.Width * 0.34f + ExplorationSurfaceRules.SmoothBoundaryOffset(y, 47, 2.4f);
            float eastLine = map.Width * 0.66f + ExplorationSurfaceRules.SmoothBoundaryOffset(y, 61, 2.4f);
            bool north = y < northLine;
            bool south = y > southLine;
            bool west = x < westLine;
            bool east = x > eastLine;
            if (north && west) return "old-quarry";
            if (north && east) return "glass-warrens";
            if (south && west) return "ash-fen";
            if (south && east) return "red-gate";
            if (north) return "gloam-courts";
            if (south) return "salt-cisterns";
            if (west) return "green-shrine-road";
            if (east) return "dusk-market";
            return "inner-ash-road";
        }

        private string MidgaardTileKind(int x, int y, int tile)
        {
            if (tile == 0) return "midgaardwall";
            ExplorationMaterial material = ExploreMaterialAt(x, y);
            ExplorationCellRole roles = ExploreRolesAt(x, y);
            if (material == ExplorationMaterial.MarketCobbles) return "midgaard-market";
            if (material == ExplorationMaterial.TempleStone) return "midgaard-temple";
            if (material == ExplorationMaterial.KeepStone) return "midgaard-king";
            if (material == ExplorationMaterial.SewerBrick) return "midgaard-sewer";
            if ((roles & ExplorationCellRole.Plaza) != 0) return "midgaard-plaza";
            if ((roles & ExplorationCellRole.Road) != 0) return "midgaard-road";
            return "midgaard-paved";
        }

        private int ExploreNoise(int x, int y, int salt)
        {
            unchecked
            {
                int seed = x * 92821 + y * 68917 + state.Depth * 8191 + salt * 19333;
                seed ^= seed << 13;
                seed ^= seed >> 17;
                seed ^= seed << 5;
                return seed & 0x7fffffff;
            }
        }

        private void DrawExploreFogMotif(Rect rect, int x, int y)
        {
            int n = ExploreNoise(x, y, 31) % 5;
            DrawRect(rect, Hex("05090b", 0.72f));
            if (n == 0)
            {
                float bandY = rect.y + rect.height * 0.44f;
                DrawRect(new Rect(rect.x, bandY, rect.width, Mathf.Max(1f, rect.height * 0.08f)), Hex("9ba9a3", 0.035f));
                DrawRect(new Rect(rect.x + rect.width * 0.08f, bandY + rect.height * 0.16f, rect.width * 0.62f, Mathf.Max(1f, rect.height * 0.06f)), Hex("c6cec4", 0.035f));
            }
            if (Distance(x, y, state.PlayerX, state.PlayerY) == ExploreRevealRadius + 1)
            {
                DrawBorder(rect, Hex("d9d3c4", 0.06f), 1);
            }
        }

        private void DrawExploreDistanceShade(Rect rect, int viewX, int viewY, int viewW, int viewH)
        {
            float halfW = Mathf.Max(1f, (viewW - 1) * 0.5f);
            float halfH = Mathf.Max(1f, (viewH - 1) * 0.5f);
            float nx = Mathf.Abs(viewX - halfW) / halfW;
            float ny = Mathf.Abs(viewY - halfH) / halfH;
            float edge = Mathf.Clamp01((Mathf.Max(nx, ny) - 0.64f) / 0.36f);
            if (edge > 0f) DrawRect(rect, Hex("050708", edge * (exploreWideView ? 0.14f : 0.09f)));
        }

        private void DrawExploreTileEdges(Rect rect, int x, int y, int tile)
        {
            if (tile != 1) return;
            if (IsMidgaardGateCell(x, y)) return;
            string kind = ExploreTileKind(x, y, tile);
            Color edge = kind == "mire" || kind == "mud" || kind == "cistern" ? Hex("0f2728", 0.34f) : kind == "moss" ? Hex("17251c", 0.32f) : Hex("050708", 0.28f);
            float t = Mathf.Clamp(rect.width * 0.025f, 1f, 3f);
            if (TileAt(state.Map, x, y - 1) == 0) DrawRect(new Rect(rect.x, rect.y, rect.width, t), edge);
            if (TileAt(state.Map, x, y + 1) == 0) DrawRect(new Rect(rect.x, rect.yMax - t, rect.width, t), edge);
            if (TileAt(state.Map, x - 1, y) == 0) DrawRect(new Rect(rect.x, rect.y, t, rect.height), edge);
            if (TileAt(state.Map, x + 1, y) == 0) DrawRect(new Rect(rect.xMax - t, rect.y, t, rect.height), edge);
        }

        private void DrawExploreMaterialFeather(Rect rect, int x, int y, int tile)
        {
            if (state?.Map == null || tile != 1) return;
            ExplorationMaterial current = ExploreMaterialAt(x, y);
            DrawExploreMaterialFeatherSide(rect, x, y, current, -1, 0);
            DrawExploreMaterialFeatherSide(rect, x, y, current, 1, 0);
            DrawExploreMaterialFeatherSide(rect, x, y, current, 0, -1);
            DrawExploreMaterialFeatherSide(rect, x, y, current, 0, 1);
        }

        private void DrawExploreMaterialFeatherSide(
            Rect rect,
            int x,
            int y,
            ExplorationMaterial current,
            int dx,
            int dy)
        {
            int nextX = x + dx;
            int nextY = y + dy;
            bool currentProtected = IsExploreMaterialFeatherProtectedCell(x, y);
            bool neighborProtected = IsExploreMaterialFeatherProtectedCell(nextX, nextY);
            int neighborTile = nextX >= 0
                && nextY >= 0
                && nextX < state.Map.Width
                && nextY < state.Map.Height
                ? TileAt(state.Map, nextX, nextY)
                : 0;
            ExplorationMaterial neighbor = neighborTile == 1
                ? ExploreMaterialAt(nextX, nextY)
                : ExplorationMaterial.NaturalGround;
            if (!ExplorationArtRules.ShouldBlendMaterialEdge(
                current,
                neighbor,
                true,
                neighborTile == 1,
                currentProtected || neighborProtected))
            {
                return;
            }

            if (!TryResolveWorldMapMaterialAtlasEdgeSample(
                neighbor,
                nextX,
                nextY,
                out Rect neighborSource,
                out bool neighborFlipX,
                out bool neighborFlipY))
            {
                return;
            }

            int bandCount = ExplorationArtRules.MaterialBlendBandCount();
            float fraction = ExplorationArtRules.MaterialBlendBandFraction(exploreWideView);
            for (int band = 0; band < bandCount; band++)
            {
                float width = rect.width * fraction;
                float height = rect.height * fraction;
                Rect destination;
                if (dx < 0)
                {
                    destination = new Rect(rect.x + band * width, rect.y, width, rect.height);
                }
                else if (dx > 0)
                {
                    destination = new Rect(rect.xMax - (band + 1) * width, rect.y, width, rect.height);
                }
                else if (dy < 0)
                {
                    destination = new Rect(rect.x, rect.y + band * height, rect.width, height);
                }
                else
                {
                    destination = new Rect(rect.x, rect.yMax - (band + 1) * height, rect.width, height);
                }

                TryDrawWorldMapMaterialAtlasEdgeBand(
                    destination,
                    neighborSource,
                    neighborFlipX,
                    neighborFlipY,
                    dx,
                    dy,
                    band,
                    fraction,
                    ExplorationArtRules.MaterialBlendBandAlpha(band, exploreWideView));
            }
        }

        private bool IsExploreMaterialFeatherProtectedCell(int x, int y)
        {
            if (state?.Map == null || x < 0 || y < 0 || x >= state.Map.Width || y >= state.Map.Height) return true;
            if (IsMidgaardGateCell(x, y)) return true;
            return (ExploreRolesAt(x, y) & ExplorationCellRole.Threshold) != 0;
        }

        private void DrawExploreTerrainBoundaries(Rect grid, float cell, Point origin, int viewW, int viewH)
        {
            for (int vy = 0; vy < viewH; vy++)
            for (int vx = 0; vx < viewW; vx++)
            {
                int x = origin.X + vx;
                int y = origin.Y + vy;
                int tile = TileAt(state.Map, x, y);
                if (vx + 1 < viewW)
                {
                    int nextTile = TileAt(state.Map, x + 1, y);
                    bool gateJoin = IsMidgaardGateCell(x, y) || IsMidgaardGateCell(x + 1, y);
                    float alpha = gateJoin ? 0f : tile != nextTile ? 0.26f : 0f;
                    if (alpha > 0f)
                    {
                        float px = grid.x + (vx + 1) * cell - 1f;
                        DrawRect(new Rect(px, grid.y + vy * cell, 1f, cell), Hex("050708", alpha));
                    }
                }
                if (vy + 1 < viewH)
                {
                    int nextTile = TileAt(state.Map, x, y + 1);
                    bool gateJoin = IsMidgaardGateCell(x, y) || IsMidgaardGateCell(x, y + 1);
                    float alpha = gateJoin ? 0f : tile != nextTile ? 0.26f : 0f;
                    if (alpha > 0f)
                    {
                        float py = grid.y + (vy + 1) * cell - 1f;
                        DrawRect(new Rect(grid.x + vx * cell, py, cell, 1f), Hex("050708", alpha));
                    }
                }
            }
        }

        private bool IsMidgaardGateCell(int x, int y)
        {
            MapObject obj = ObjectAt(state?.Map, x, y);
            return obj != null && IsMidgaardGateType(obj.Type);
        }

        private void DrawExploreSurfaceOverlay(Rect rect, int x, int y, int tile)
        {
            if (tile != 1) return;
            ExplorationCellRole roles = ExploreRolesAt(x, y);
            ExplorationMaterial material = ExploreMaterialAt(x, y);
            if (!ExplorationArtRules.ShouldDrawMaterialPathStroke(material, roles)) return;
            MapObject occupant = ObjectAt(state?.Map, x, y);
            if (occupant != null && (UsesSemanticMidgaardGround(occupant) || IsMidgaardGateType(occupant.Type))) return;

            int mask = ExplorationSurfaceRules.PathNeighborMask(state?.Map, x, y);
            bool city = (roles & ExplorationCellRole.City) != 0;
            bool road = (roles & ExplorationCellRole.Road) != 0;
            float shoulderFraction = city
                ? exploreWideView ? 0.14f : 0.17f
                : road
                    ? exploreWideView ? 0.22f : 0.22f
                    : exploreWideView ? 0.16f : 0.19f;
            float shoulderWidth = rect.width * shoulderFraction;
            float coreWidth = shoulderWidth * 0.68f;
            float highlightWidth = coreWidth * 0.20f;
            Color shoulder = city ? Hex("111719", 0.17f) : Hex("20150e", exploreWideView ? 0.34f : 0.30f);
            Color core = city
                ? Hex("81765f", 0.13f)
                : material == ExplorationMaterial.Moss
                    ? Hex("735637", exploreWideView ? 0.34f : 0.28f)
                    : Hex("8f6841", exploreWideView ? 0.31f : 0.25f);
            Color highlight = city ? Hex("d3c5a0", 0.09f) : Hex("e2bd83", exploreWideView ? 0.22f : 0.15f);
            DrawExplorePathStroke(rect, mask, shoulderWidth, shoulder);
            DrawExplorePathStroke(rect, mask, coreWidth, core);
            DrawExplorePathStroke(rect, mask, highlightWidth, highlight);
            if ((roles & ExplorationCellRole.Clearing) != 0)
            {
                DrawRegionalJunctionGroundMark(rect, x, y);
            }
        }

        private void DrawRegionalJunctionGroundMark(Rect rect, int x, int y)
        {
            Color accent = ZoneDangerColor(ZoneAt(x, y));
            bool hasJunction = TryRegionalJunctionAt(x, y, 0, out WorldMapJunction junction);
            bool known = hasJunction
                && state?.DiscoveredZones != null
                && state.DiscoveredZones.Contains(RegionalJunctionKey(state.Depth, junction.Id));
            bool waypoint = known && RouteChartRules.IsWaypoint(state.ActiveRouteWaypointKey, state.Depth, junction.Id);
            float alpha = waypoint ? 0.74f : known ? 0.24f : 0.36f;
            float inset = rect.width * 0.22f;
            Rect mark = Pad(rect, inset);
            DrawRect(mark, Hex("050708", alpha * 0.42f));
            DrawBorder(mark, (waypoint ? gold : accent).WithAlpha(alpha), waypoint ? 2 : 1);

            float pip = Mathf.Max(2f, rect.width * 0.075f);
            Color markerAccent = waypoint ? gold : accent;
            Color stoneMark = Color.Lerp(markerAccent, cursorWhite, 0.28f).WithAlpha(alpha * 0.88f);
            DrawRect(new Rect(mark.x - pip * 0.35f, mark.center.y - pip * 0.5f, pip, pip), stoneMark);
            DrawRect(new Rect(mark.xMax - pip * 0.65f, mark.center.y - pip * 0.5f, pip, pip), stoneMark);
            DrawRect(new Rect(mark.center.x - pip * 0.5f, mark.y - pip * 0.35f, pip, pip), stoneMark);
            DrawRect(new Rect(mark.center.x - pip * 0.5f, mark.yMax - pip * 0.65f, pip, pip), stoneMark);
            DrawPixelCross(Pad(mark, mark.width * 0.32f), markerAccent.WithAlpha(alpha * 0.78f));
            if (waypoint)
            {
                float pulse = state.ReducedMotion ? 0.72f : 0.72f + Mathf.Sin(Time.time * 4.2f) * 0.16f;
                DrawCornerBrackets(
                    Pad(rect, rect.width * 0.08f),
                    gold.WithAlpha(Mathf.Clamp01(pulse)),
                    Mathf.Max(1f, rect.width * 0.06f),
                    rect.width * 0.24f);
            }
        }

        private void DrawExploreWaypointTrail(Rect grid, float cell, Point origin, int viewW, int viewH)
        {
            ExploreGuidancePlan plan = CurrentExploreGuidancePlan();
            IReadOnlyList<Point> path = plan.Path;
            if (!plan.HasTarget
                || plan.Immediate
                || plan.RouteBlocked
                || path == null
                || path.Count <= 1)
            {
                return;
            }

            int shown = Mathf.Min(
                path.Count,
                ExplorationMapGuidanceRules.VisiblePointLimit(
                    exploreWideView,
                    plan.MarkedWaypoint));
            float outerWidth = Mathf.Max(2f, cell * (exploreWideView ? 0.14f : 0.10f));
            float innerWidth = Mathf.Max(1f, outerWidth * (plan.MarkedWaypoint ? 0.52f : exploreWideView ? 0.46f : 0.38f));
            Color outer = Hex(
                "050708",
                plan.MarkedWaypoint
                    ? exploreWideView ? 0.58f : 0.66f
                    : exploreWideView ? 0.54f : 0.50f);
            Color inner = gold.WithAlpha(
                plan.MarkedWaypoint
                    ? exploreWideView ? 0.52f : 0.64f
                    : exploreWideView ? 0.46f : 0.44f);
            for (int i = 1; i < shown; i++)
            {
                Point from = path[i - 1];
                Point to = path[i];
                if (!ExplorePointInViewport(from.X, from.Y, origin, viewW, viewH)
                    || !ExplorePointInViewport(to.X, to.Y, origin, viewW, viewH))
                {
                    continue;
                }

                Vector2 fromCenter = new Vector2(
                    grid.x + (from.X - origin.X + 0.5f) * cell,
                    grid.y + (from.Y - origin.Y + 0.5f) * cell);
                Vector2 toCenter = new Vector2(
                    grid.x + (to.X - origin.X + 0.5f) * cell,
                    grid.y + (to.Y - origin.Y + 0.5f) * cell);
                DrawExploreWaypointTrailSegment(fromCenter, toCenter, outerWidth, outer);
                DrawExploreWaypointTrailSegment(fromCenter, toCenter, innerWidth, inner);

                float dot = Mathf.Max(2f, innerWidth * 1.45f);
                DrawRect(new Rect(toCenter.x - dot * 0.5f, toCenter.y - dot * 0.5f, dot, dot), inner);
            }
        }

        private void DrawExploreGuidanceCues(
            Rect grid,
            float cell,
            Point origin,
            int viewW,
            int viewH)
        {
            ExploreGuidancePlan plan = CurrentExploreGuidancePlan();
            IReadOnlyList<Point> path = plan.Path;
            if (!plan.HasTarget
                || plan.Immediate
                || plan.RouteBlocked
                || path == null
                || path.Count <= 1)
            {
                return;
            }

            Color accent = gold;
            DrawExploreGuidanceNextStepCue(grid, cell, origin, viewW, viewH, path, accent, plan.MarkedWaypoint);
            int visiblePointLimit = ExplorationMapGuidanceRules.VisiblePointLimit(
                exploreWideView,
                plan.MarkedWaypoint);
            if (ExplorationMapGuidanceRules.TryFindViewportExit(
                    path,
                    origin.X,
                    origin.Y,
                    viewW,
                    viewH,
                    out ExplorationMapExitCue exitCue)
                && ExplorationMapGuidanceRules.IsExitCueWithinVisiblePrefix(
                    exitCue,
                    visiblePointLimit))
            {
                DrawExploreGuidanceEdgeCue(grid, cell, origin, exitCue, accent, plan.MarkedWaypoint);
                return;
            }

            if (plan.MarkedWaypoint) return;
            Point arrival = path[path.Count - 1];
            if (!ExplorePointInViewport(arrival.X, arrival.Y, origin, viewW, viewH)) return;
            Rect arrivalCell = new Rect(
                grid.x + (arrival.X - origin.X) * cell,
                grid.y + (arrival.Y - origin.Y) * cell,
                cell,
                cell);
            DrawCornerBrackets(
                Pad(arrivalCell, cell * 0.12f),
                accent.WithAlpha(0.62f),
                Mathf.Max(1f, cell * 0.035f),
                cell * 0.16f);
            float arrivalDot = Mathf.Max(3f, cell * 0.07f);
            DrawRect(
                new Rect(
                    arrivalCell.center.x - arrivalDot * 0.5f,
                    arrivalCell.center.y - arrivalDot * 0.5f,
                    arrivalDot,
                    arrivalDot),
                accent.WithAlpha(0.72f));
        }

        private void DrawExploreGuidanceNextStepCue(
            Rect grid,
            float cell,
            Point origin,
            int viewW,
            int viewH,
            IReadOnlyList<Point> path,
            Color accent,
            bool markedWaypoint)
        {
            Point step = path[1];
            if (!ExplorePointInViewport(step.X, step.Y, origin, viewW, viewH)) return;
            Rect stepCell = new Rect(
                grid.x + (step.X - origin.X) * cell,
                grid.y + (step.Y - origin.Y) * cell,
                cell,
                cell);
            float pulse = state != null && state.ReducedMotion
                ? 0.78f
                : 0.78f + Mathf.Sin(Time.time * 4.6f) * 0.12f;
            DrawCornerBrackets(
                Pad(stepCell, cell * 0.09f),
                accent.WithAlpha(Mathf.Clamp01(markedWaypoint ? pulse : pulse * 0.84f)),
                Mathf.Max(1f, cell * 0.045f),
                cell * 0.19f);

            string direction = ActiveRouteWaypointFirstDirection(path);
            string movementKey = ExplorationGuidanceRules.MovementKey(direction);
            if (string.IsNullOrEmpty(movementKey)) return;
            float keySize = Mathf.Clamp(cell * 0.24f, 16f, 25f);
            Rect keycap = new Rect(
                stepCell.x + cell * 0.08f,
                stepCell.y + cell * 0.08f,
                keySize,
                keySize);
            DrawRect(keycap, Hex("030405", 0.92f));
            DrawBorder(keycap, accent.WithAlpha(markedWaypoint ? 0.96f : 0.82f), 1);
            GUI.Label(
                keycap,
                movementKey,
                CenterStyle(
                    Mathf.RoundToInt(Mathf.Clamp(keySize * 0.50f, 9f, 12f)),
                    ink));
        }

        private void DrawExploreGuidanceEdgeCue(
            Rect grid,
            float cell,
            Point origin,
            ExplorationMapExitCue cue,
            Color accent,
            bool markedWaypoint)
        {
            Rect anchorCell = new Rect(
                grid.x + (cue.MapX - origin.X) * cell,
                grid.y + (cue.MapY - origin.Y) * cell,
                cell,
                cell);
            float scale = ExplorationHudScreenLayout.InterfaceScale(Screen.width, Screen.height);
            float chipWidth = Mathf.Clamp(cell * 2.15f, 78f * scale, 118f * scale);
            float chipHeight = Mathf.Clamp(cell * 0.42f, 20f * scale, 28f * scale);
            float inset = Mathf.Max(5f * scale, cell * 0.08f);
            float x = Mathf.Clamp(
                anchorCell.center.x - chipWidth * 0.5f,
                grid.x + inset,
                grid.xMax - chipWidth - inset);
            float y = Mathf.Clamp(
                anchorCell.center.y - chipHeight * 0.5f,
                grid.y + inset,
                grid.yMax - chipHeight - inset);

            switch (cue.Edge)
            {
                case ExplorationMapEdge.North:
                    y = grid.y + inset;
                    break;
                case ExplorationMapEdge.East:
                    x = grid.xMax - chipWidth - inset;
                    break;
                case ExplorationMapEdge.South:
                    y = grid.yMax - chipHeight - inset;
                    break;
                case ExplorationMapEdge.West:
                    x = grid.x + inset;
                    break;
            }

            Rect chip = new Rect(x, y, chipWidth, chipHeight);
            DrawRect(chip, Hex("030405", 0.94f));
            DrawBorder(chip, accent.WithAlpha(markedWaypoint ? 0.98f : 0.84f), markedWaypoint ? 2 : 1);
            string edge = cue.Edge == ExplorationMapEdge.North ? "N"
                : cue.Edge == ExplorationMapEdge.East ? "E"
                : cue.Edge == ExplorationMapEdge.South ? "S"
                : "W";
            string prefix = markedWaypoint ? "MARK" : "NEXT";
            string steps = cue.RemainingSteps == 1 ? "1" : cue.RemainingSteps.ToString();
            string label = $"{prefix}  {edge}{steps}";
            GUI.Label(
                chip,
                FitText(
                    label,
                    chip.width - 8f * scale,
                    CenterStyle(ExplorationHudScreenLayout.FontSize(10, Screen.width, Screen.height), ink)),
                CenterStyle(ExplorationHudScreenLayout.FontSize(10, Screen.width, Screen.height), ink));

            float edgeWidth = Mathf.Max(3f, cell * 0.07f);
            switch (cue.Edge)
            {
                case ExplorationMapEdge.North:
                    DrawRect(new Rect(anchorCell.x + cell * 0.28f, grid.y, cell * 0.44f, edgeWidth), accent.WithAlpha(0.88f));
                    break;
                case ExplorationMapEdge.East:
                    DrawRect(new Rect(grid.xMax - edgeWidth, anchorCell.y + cell * 0.28f, edgeWidth, cell * 0.44f), accent.WithAlpha(0.88f));
                    break;
                case ExplorationMapEdge.South:
                    DrawRect(new Rect(anchorCell.x + cell * 0.28f, grid.yMax - edgeWidth, cell * 0.44f, edgeWidth), accent.WithAlpha(0.88f));
                    break;
                case ExplorationMapEdge.West:
                    DrawRect(new Rect(grid.x, anchorCell.y + cell * 0.28f, edgeWidth, cell * 0.44f), accent.WithAlpha(0.88f));
                    break;
            }
        }

        private void DrawExploreWaypointTrailSegment(Vector2 from, Vector2 to, float width, Color color)
        {
            if (Mathf.Approximately(from.x, to.x))
            {
                DrawRect(new Rect(
                    from.x - width * 0.5f,
                    Mathf.Min(from.y, to.y),
                    width,
                    Mathf.Max(width, Mathf.Abs(to.y - from.y))),
                    color);
                return;
            }

            DrawRect(new Rect(
                Mathf.Min(from.x, to.x),
                from.y - width * 0.5f,
                Mathf.Max(width, Mathf.Abs(to.x - from.x)),
                width),
                color);
        }

        private void DrawExplorePathStroke(Rect rect, int mask, float width, Color color)
        {
            float cx = rect.center.x;
            float cy = rect.center.y;
            float half = width * 0.5f;
            DrawRect(new Rect(cx - half, cy - half, width, width), color);
            if ((mask & 1) != 0) DrawRect(new Rect(cx - half, rect.y, width, cy - rect.y), color);
            if ((mask & 2) != 0) DrawRect(new Rect(cx, cy - half, rect.xMax - cx, width), color);
            if ((mask & 4) != 0) DrawRect(new Rect(cx - half, cy, width, rect.yMax - cy), color);
            if ((mask & 8) != 0) DrawRect(new Rect(rect.x, cy - half, cx - rect.x, width), color);
        }

        private string ExploreTerrainFamily(string kind)
        {
            kind = kind ?? "";
            if (kind.StartsWith("midgaard", StringComparison.Ordinal)) return "midgaard";
            if (kind == "road" || kind == "outworks") return "road";
            if (kind == "paved" || kind == "ruins" || kind == "gloam") return "stone";
            if (kind == "mire" || kind == "mud" || kind == "cistern") return "wet";
            return kind;
        }

        private void DrawExploreTileMotif(Rect rect, int x, int y, int tile)
        {
            int n = ExploreNoise(x, y, 3) % 9;
            string kind = ExploreTileKind(x, y, tile);
            if (kind.StartsWith("midgaard")) return;
            if (tile == 0)
            {
                DrawExploreBlockMotif(rect, kind, n);
                return;
            }

            if (kind == "road")
            {
                Color track = Hex("211912", 0.32f);
                bool vertical = TileAt(state.Map, x, y - 1) == 1 || TileAt(state.Map, x, y + 1) == 1;
                bool horizontal = TileAt(state.Map, x - 1, y) == 1 || TileAt(state.Map, x + 1, y) == 1;
                if (horizontal) DrawRect(new Rect(rect.x, rect.y + rect.height * 0.42f, rect.width, rect.height * 0.16f), track);
                if (vertical) DrawRect(new Rect(rect.x + rect.width * 0.42f, rect.y, rect.width * 0.16f, rect.height), track);
                DrawRect(new Rect(rect.x + rect.width * (0.16f + (n % 3) * 0.20f), rect.y + rect.height * 0.18f, rect.width * 0.12f, rect.height * 0.08f), Hex("8a7560", 0.20f));
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.72f, rect.width * 0.45f, rect.height * 0.05f), Hex("15110e", 0.24f));
                return;
            }

            if (kind == "mire" || kind == "mud")
            {
                Color puddle = kind == "mire" ? Hex("3b6b67", 0.36f) : Hex("1f2420", 0.26f);
                DrawRect(new Rect(rect.x + rect.width * 0.14f, rect.y + rect.height * 0.30f, rect.width * 0.58f, rect.height * 0.16f), puddle);
                if (n % 2 == 0) DrawRect(new Rect(rect.x + rect.width * 0.68f, rect.y + rect.height * 0.18f, rect.width * 0.06f, rect.height * 0.42f), Hex("9ca86e", 0.34f));
                if (n % 3 == 0) DrawRect(new Rect(rect.x + rect.width * 0.76f, rect.y + rect.height * 0.34f, rect.width * 0.05f, rect.height * 0.30f), Hex("7f9d5b", 0.28f));
                DrawRect(new Rect(rect.x + rect.width * 0.24f, rect.y + rect.height * 0.68f, rect.width * 0.38f, rect.height * 0.05f), Hex("050708", 0.22f));
                return;
            }

            if (kind == "moss")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.16f, rect.y + rect.height * 0.18f, rect.width * 0.40f, rect.height * 0.14f), Hex("7f9d5b", 0.24f));
                DrawRect(new Rect(rect.x + rect.width * 0.62f, rect.y + rect.height * 0.54f, rect.width * 0.22f, rect.height * 0.16f), Hex("465f39", 0.34f));
                if (n % 3 == 1) DrawRect(new Rect(rect.x + rect.width * 0.36f, rect.y + rect.height * 0.20f, rect.width * 0.05f, rect.height * 0.58f), Hex("a8c06f", 0.20f));
            }
            else if (kind == "quarry")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.12f, rect.y + rect.height * 0.60f, rect.width * 0.52f, rect.height * 0.08f), Hex("15191b", 0.30f));
                DrawRect(new Rect(rect.x + rect.width * 0.64f, rect.y + rect.height * 0.24f, rect.width * 0.16f, rect.height * 0.12f), Hex("a9b0a2", 0.18f));
                if (n % 2 == 0) DrawRect(new Rect(rect.x + rect.width * 0.20f, rect.y + rect.height * 0.22f, rect.width * 0.12f, rect.height * 0.10f), Hex("6b756e", 0.20f));
            }
            else if (kind == "glass")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.22f, rect.y + rect.height * 0.18f, rect.width * 0.06f, rect.height * 0.42f), Hex("9ad6e8", 0.26f));
                DrawRect(new Rect(rect.x + rect.width * 0.58f, rect.y + rect.height * 0.34f, rect.width * 0.24f, rect.height * 0.05f), Hex("d6f4ff", 0.22f));
                DrawRect(new Rect(rect.x + rect.width * 0.34f, rect.y + rect.height * 0.68f, rect.width * 0.34f, rect.height * 0.05f), Hex("15191b", 0.26f));
            }
            else if (kind == "ash")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.62f, rect.width * 0.54f, rect.height * 0.06f), Hex("050708", 0.34f));
                if (n % 3 == 0) DrawRect(new Rect(rect.x + rect.width * 0.58f, rect.y + rect.height * 0.18f, rect.width * 0.06f, rect.height * 0.36f), ember.WithAlpha(0.22f));
            }
            else
            {
                Color seam = Hex("15191b", 0.30f);
                DrawRect(new Rect(rect.x + rect.width * 0.10f, rect.y + rect.height * 0.48f, rect.width * 0.80f, rect.height * 0.05f), seam);
                if (n % 2 == 0) DrawRect(new Rect(rect.x + rect.width * 0.52f, rect.y + rect.height * 0.08f, rect.width * 0.05f, rect.height * 0.78f), seam);
                if (n % 4 == 0) DrawRect(new Rect(rect.x + rect.width * 0.20f, rect.y + rect.height * 0.20f, rect.width * 0.16f, rect.height * 0.10f), Hex("7f9d5b", 0.16f));
            }

            DrawRect(new Rect(rect.x + rect.width * 0.08f, rect.y + rect.height * 0.08f, rect.width * 0.12f, rect.height * 0.06f), Hex("ffffff", 0.035f));
        }

        private void DrawExploreTileAccent(Rect rect, int x, int y, int tile, string kind)
        {
            int n = ExploreNoise(x, y, 17) % 11;
            if (tile == 0)
            {
                Color shade = kind == "forestwall" ? Hex("050708", 0.12f) : Hex("050708", 0.16f);
                if (kind == "forestwall")
                {
                    if (n % 3 == 0) DrawRect(new Rect(rect.x + rect.width * 0.12f, rect.yMax - rect.height * 0.20f, rect.width * 0.42f, rect.height * 0.035f), Hex("7a5233", 0.24f));
                    if (n % 4 == 0) DrawRect(new Rect(rect.x + rect.width * 0.66f, rect.y + rect.height * 0.08f, rect.width * 0.18f, rect.height * 0.045f), Hex("9cb86b", 0.12f));
                }
                DrawRect(new Rect(rect.x, rect.yMax - rect.height * 0.08f, rect.width, rect.height * 0.08f), shade);
                return;
            }

            if (kind == "road")
            {
                bool vertical = TileAt(state.Map, x, y - 1) == 1 || TileAt(state.Map, x, y + 1) == 1;
                bool horizontal = TileAt(state.Map, x - 1, y) == 1 || TileAt(state.Map, x + 1, y) == 1;
                Color rut = Hex("15110e", 0.13f);
                if (horizontal) DrawRect(new Rect(rect.x, rect.y + rect.height * 0.48f, rect.width, Mathf.Max(1f, rect.height * 0.035f)), rut);
                if (vertical) DrawRect(new Rect(rect.x + rect.width * 0.49f, rect.y, Mathf.Max(1f, rect.width * 0.035f), rect.height), rut);
                if (n % 3 == 1) DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.26f, rect.width * 0.16f, rect.height * 0.035f), Hex("d9c08b", 0.12f));
                return;
            }

            if (kind == "moss")
            {
                if (n % 2 == 0) DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.20f, rect.width * 0.22f, rect.height * 0.045f), Hex("b2c77b", 0.12f));
                if (n % 5 == 0) DrawRect(new Rect(rect.x + rect.width * 0.66f, rect.y + rect.height * 0.62f, rect.width * 0.18f, rect.height * 0.05f), Hex("050708", 0.13f));
                return;
            }

            if (kind == "mire" || kind == "mud")
            {
                if (n % 2 == 0) DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.34f, rect.width * 0.42f, rect.height * 0.04f), Hex("9db599", 0.10f));
                DrawRect(new Rect(rect.x + rect.width * 0.10f, rect.yMax - rect.height * 0.12f, rect.width * 0.50f, rect.height * 0.04f), Hex("050708", 0.12f));
                return;
            }

            if (kind == "glass")
            {
                if (n % 3 == 0) DrawRect(new Rect(rect.x + rect.width * 0.55f, rect.y + rect.height * 0.20f, rect.width * 0.20f, rect.height * 0.025f), Hex("d6f4ff", 0.16f));
                return;
            }

            if (kind == "ash")
            {
                if (n % 3 == 0) DrawRect(new Rect(rect.x + rect.width * 0.54f, rect.y + rect.height * 0.22f, rect.width * 0.04f, rect.height * 0.30f), ember.WithAlpha(0.12f));
                return;
            }

            if (kind == "quarry" || kind == "paved")
            {
                if (n % 2 == 0) DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.68f, rect.width * 0.48f, rect.height * 0.035f), Hex("050708", 0.14f));
            }
        }

        private void DrawExploreBlockMotif(Rect rect, string kind, int n)
        {
            if (kind == "forestwall")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.44f, rect.y + rect.height * 0.26f, rect.width * 0.13f, rect.height * 0.54f), Hex("5b3a25", 0.62f));
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.14f, rect.width * 0.64f, rect.height * 0.28f), Hex("465f39", 0.60f));
                DrawRect(new Rect(rect.x + rect.width * 0.28f, rect.y + rect.height * 0.04f, rect.width * 0.44f, rect.height * 0.22f), Hex("7f9d5b", 0.28f));
                if (n % 2 == 0) DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.66f, rect.width * 0.36f, rect.height * 0.06f), Hex("8a5c35", 0.42f));
                return;
            }

            if (kind == "mirewall")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.08f, rect.y + rect.height * 0.32f, rect.width * 0.78f, rect.height * 0.18f), Hex("3b6b67", 0.30f));
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.56f, rect.width * 0.54f, rect.height * 0.12f), Hex("101619", 0.34f));
                DrawRect(new Rect(rect.x + rect.width * 0.72f, rect.y + rect.height * 0.18f, rect.width * 0.06f, rect.height * 0.54f), Hex("7f9d5b", 0.34f));
                return;
            }

            Color shade = kind == "redwall" ? Hex("8f3733", 0.26f) : kind == "cliffwall" ? Hex("a9b0a2", 0.20f) : Hex("6b756e", 0.20f);
            DrawRect(new Rect(rect.x + rect.width * 0.12f, rect.y + rect.height * 0.20f, rect.width * 0.72f, rect.height * 0.10f), shade);
            DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.58f, rect.width * 0.55f, rect.height * 0.08f), shade);
            if (n % 2 == 0) DrawRect(new Rect(rect.x + rect.width * 0.64f, rect.y + rect.height * 0.30f, rect.width * 0.06f, rect.height * 0.42f), Hex("050708", 0.26f));
            if (kind == "redwall" && n % 3 == 0) DrawRect(new Rect(rect.x + rect.width * 0.28f, rect.y + rect.height * 0.40f, rect.width * 0.34f, rect.height * 0.05f), ember.WithAlpha(0.24f));
        }

        private void DrawCombatTileMotif(Rect rect, int x, int y)
        {
            int depth = state != null ? state.Depth : 1;
            int n = Mathf.Abs(x * 31 + y * 47 + state.Combat.Round * 13 + depth * 19) % 12;
            Color crack = Hex("141719", 0.32f);
            Color warm = Hex("d7a84e", 0.08f);
            Color mossMark = Hex("7f9d5b", 0.10f);
            Color ash = Hex("c65c3b", 0.08f);
            Color rune = Hex("58b7a5", 0.12f);
            if (n <= 1) DrawRect(new Rect(rect.x + rect.width * 0.16f, rect.y + rect.height * 0.66f, rect.width * 0.58f, rect.height * 0.05f), crack);
            if (n == 2 || n == 4) DrawRect(new Rect(rect.x + rect.width * 0.70f, rect.y + rect.height * 0.20f, rect.width * 0.05f, rect.height * 0.46f), crack);
            if (n == 3) DrawRect(new Rect(rect.x + rect.width * 0.14f, rect.y + rect.height * 0.18f, rect.width * 0.16f, rect.height * 0.10f), warm);
            if (n == 5 || n == 8)
            {
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.20f, rect.width * 0.12f, rect.height * 0.05f), mossMark);
                DrawRect(new Rect(rect.x + rect.width * 0.26f, rect.y + rect.height * 0.26f, rect.width * 0.18f, rect.height * 0.04f), mossMark);
                DrawRect(new Rect(rect.x + rect.width * 0.12f, rect.y + rect.height * 0.74f, rect.width * 0.22f, rect.height * 0.04f), mossMark);
            }
            if (n == 6 && depth >= 2)
            {
                DrawRect(new Rect(rect.x + rect.width * 0.35f, rect.y + rect.height * 0.35f, rect.width * 0.30f, rect.height * 0.06f), ash);
                DrawRect(new Rect(rect.x + rect.width * 0.43f, rect.y + rect.height * 0.46f, rect.width * 0.20f, rect.height * 0.05f), ash);
                DrawRect(new Rect(rect.x + rect.width * 0.29f, rect.y + rect.height * 0.52f, rect.width * 0.18f, rect.height * 0.04f), ash);
            }
            if (n == 7 && depth >= 3)
            {
                Rect grate = new Rect(rect.x + rect.width * 0.34f, rect.y + rect.height * 0.31f, rect.width * 0.32f, rect.height * 0.32f);
                DrawRect(grate, Hex("050708", 0.22f));
                DrawBorder(grate, Hex("a9b0a2", 0.18f), 1);
                DrawRect(new Rect(grate.x + grate.width * 0.45f, grate.y, grate.width * 0.08f, grate.height), Hex("a9b0a2", 0.16f));
                DrawRect(new Rect(grate.x, grate.y + grate.height * 0.45f, grate.width, grate.height * 0.08f), Hex("a9b0a2", 0.16f));
            }
            if (n == 9)
            {
                DrawRect(new Rect(rect.x + rect.width * 0.28f, rect.y + rect.height * 0.24f, rect.width * 0.10f, rect.height * 0.04f), Hex("d9d3c4", 0.12f));
                DrawRect(new Rect(rect.x + rect.width * 0.34f, rect.y + rect.height * 0.29f, rect.width * 0.04f, rect.height * 0.10f), Hex("d9d3c4", 0.12f));
                DrawRect(new Rect(rect.x + rect.width * 0.63f, rect.y + rect.height * 0.67f, rect.width * 0.14f, rect.height * 0.04f), Hex("d9d3c4", 0.10f));
            }
            if (n == 10 && depth >= 2)
            {
                Rect r = new Rect(rect.x + rect.width * 0.40f, rect.y + rect.height * 0.33f, rect.width * 0.20f, rect.height * 0.20f);
                DrawBorder(r, rune, 1);
                DrawRect(new Rect(r.center.x - rect.width * 0.015f, r.y + r.height * 0.16f, rect.width * 0.03f, r.height * 0.68f), rune);
            }
        }

        private void DrawCombatObstacle(Rect rect, Point obstacle)
        {
            string kind = obstacle != null && !string.IsNullOrEmpty(obstacle.Kind) ? obstacle.Kind : "stone";
            if (!CombatFieldPresentationRules.UsesPropSprite(kind))
            {
                // Persistent fields own the terrain sprite beneath them. Keep their
                // authored fire/ice/web/gas art visible; show duration at a glance.
                DrawPersistentFieldReadout(rect, obstacle);
                return;
            }
            if (kind == "glyph" || kind == "demonrift")
            {
                // Ritual identity is already authored into the dedicated ground tile.
                // Keep one countdown, revealing integrity only when the tile is inspected.
                DrawRitualIntegrityMarks(rect, obstacle);
                return;
            }
            if (IsKoboldRouteCombat())
            {
                int koboldCaveIndex = KoboldCavePropIndex(obstacle);
                Rect routeArt = Pad(rect, rect.width * 0.03f);
                if (koboldCaveIndex >= 0 && TryDrawKoboldCavePropAtlasIcon(routeArt, koboldCaveIndex, Color.white.WithAlpha(0.94f)))
                {
                    if (CombatFieldPresentationRules.UsesAlwaysOnTacticalFrame(kind)) DrawObstacleTacticalFrame(rect, obstacle);
                    if (IsBreakableCover(obstacle)) DrawCoverIntegrityMarks(rect, obstacle);
                    if (CombatRitualRules.IsRitual(obstacle)) DrawRitualIntegrityMarks(rect, obstacle);
                    return;
                }
            }
            int biomePropIndex = CombatCoverBiomePropIndex(kind);
            if (biomePropIndex >= 0
                && TryDrawWorldMapBiomePropAtlasIcon(Pad(rect, rect.width * 0.03f), biomePropIndex, Color.white.WithAlpha(0.96f)))
            {
                if (IsBreakableCover(obstacle)) DrawCoverIntegrityMarks(rect, obstacle);
                return;
            }
            int terrainIcon = TerrainMagicIconIndex(kind);
            if (terrainIcon >= 0 && TryDrawMagicUiAtlasIcon(Pad(rect, rect.width * 0.06f), terrainIcon, Color.white.WithAlpha(0.92f)))
            {
                if (CombatFieldPresentationRules.UsesAlwaysOnTacticalFrame(kind)) DrawObstacleTacticalFrame(rect, obstacle);
                if (IsBreakableCover(obstacle)) DrawCoverIntegrityMarks(rect, obstacle);
                return;
            }

            if (kind == "tree")
            {
                Color trunk = Hex("5b3a25");
                Color trunkLight = Hex("8a5c35");
                Color leaf = moss;
                Color leafLight = Hex("a8c06f");
                Color leafDark = Hex("465f39");
                Rect canopy = Pad(rect, rect.width * 0.12f);
                DrawRect(new Rect(rect.x + rect.width * 0.44f, rect.y + rect.height * 0.43f, rect.width * 0.13f, rect.height * 0.36f), trunk);
                DrawRect(new Rect(rect.x + rect.width * 0.48f, rect.y + rect.height * 0.47f, rect.width * 0.05f, rect.height * 0.26f), trunkLight);
                DrawRect(new Rect(canopy.x + canopy.width * 0.18f, canopy.y + canopy.height * 0.20f, canopy.width * 0.64f, canopy.height * 0.28f), leaf);
                DrawRect(new Rect(canopy.x + canopy.width * 0.08f, canopy.y + canopy.height * 0.34f, canopy.width * 0.84f, canopy.height * 0.26f), leafDark);
                DrawRect(new Rect(canopy.x + canopy.width * 0.26f, canopy.y + canopy.height * 0.08f, canopy.width * 0.48f, canopy.height * 0.24f), leafLight);
                DrawRect(new Rect(canopy.x + canopy.width * 0.34f, canopy.y + canopy.height * 0.48f, canopy.width * 0.34f, canopy.height * 0.20f), leaf);
                DrawRect(new Rect(rect.x + rect.width * 0.28f, rect.y + rect.height * 0.76f, rect.width * 0.44f, rect.height * 0.05f), trunkLight);
                DrawCoverIntegrityMarks(rect, obstacle);
                return;
            }

            Rect rock = Pad(rect, rect.width * 0.22f);
            DrawRect(new Rect(rock.x, rock.y + rock.height * 0.18f, rock.width, rock.height * 0.66f), stone);
            DrawRect(new Rect(rock.x + rock.width * 0.18f, rock.y, rock.width * 0.62f, rock.height * 0.32f), Color.Lerp(stone, ink, 0.12f));
            DrawRect(new Rect(rock.x + rock.width * 0.18f, rock.y + rock.height * 0.60f, rock.width * 0.16f, rock.height * 0.10f), Hex("20272e", 0.8f));
            DrawRect(new Rect(rock.x + rock.width * 0.58f, rock.y + rock.height * 0.36f, rock.width * 0.22f, rock.height * 0.08f), Hex("20272e", 0.8f));
            DrawCoverIntegrityMarks(rect, obstacle);
        }

        private void DrawPersistentFieldSurface(Rect rect, Point obstacle)
        {
            if (obstacle == null) return;
            CombatFieldPresentationProfile profile = CombatFieldPresentationRules.For(obstacle.Kind);
            bool reducedMotion = state != null && state.ReducedMotion;
            float pulse = CombatFieldPresentationRules.Pulse(profile, Time.time, obstacle.X, obstacle.Y, reducedMotion);
            float drift = CombatFieldPresentationRules.Drift(profile, Time.time, obstacle.X, obstacle.Y, reducedMotion);
            Color accent = ObstacleAccent(profile.Kind);
            Rect field = Pad(rect, Mathf.Max(2f, rect.width * 0.035f));
            DrawRect(field, Color.Lerp(Hex("050708"), accent, 0.30f).WithAlpha(profile.SurfaceAlpha * (0.82f + pulse * 0.18f)));

            switch (profile.Kind)
            {
                case "fire":
                    DrawFireFieldSurface(field, accent, pulse, drift, profile.EdgeAlpha);
                    break;
                case "ice":
                    DrawIceFieldSurface(field, accent, pulse, profile.EdgeAlpha);
                    break;
                case "gas":
                    DrawGasFieldSurface(field, accent, pulse, drift, profile.EdgeAlpha);
                    break;
                case "smoke":
                    DrawSmokeFieldSurface(field, accent, pulse, drift, profile.EdgeAlpha);
                    break;
                case "web":
                    DrawWebFieldSurface(field, accent, pulse, profile.EdgeAlpha);
                    break;
                case "sanctuary":
                    DrawSanctuaryFieldSurface(field, accent, pulse, profile.EdgeAlpha);
                    break;
                case "curse":
                    DrawCurseFieldSurface(field, accent, pulse, profile.EdgeAlpha);
                    break;
            }
        }

        private void DrawPersistentFieldReadout(Rect rect, Point obstacle)
        {
            if (obstacle == null) return;
            Color accent = ObstacleAccent(obstacle.Kind);
            if (CombatTileReadoutHovered(rect))
            {
                float railHeight = Mathf.Max(2f, rect.height * 0.035f);
                DrawRect(
                    new Rect(
                        rect.x + rect.width * 0.10f,
                        rect.yMax - railHeight - rect.height * 0.055f,
                        rect.width * 0.80f,
                        railHeight),
                    accent.WithAlpha(0.78f));
            }
            if (obstacle.Duration <= 0 || rect.width < 44f) return;

            string durationLabel = CombatFieldPresentationRules.DurationBadgeLabel(obstacle.Duration);
            bool urgent = CombatFieldPresentationRules.DurationBadgeUrgent(obstacle.Duration);
            Color durationAccent = urgent ? ember : accent;
            float badgeHeight = Mathf.Clamp(rect.height * 0.18f, 13f, 16f);
            float badgeWidth = Mathf.Clamp(badgeHeight * 1.72f, 23f, rect.width * 0.48f);
            Rect badge = new Rect(
                rect.x + rect.width * 0.07f,
                rect.yMax - badgeHeight - rect.height * 0.08f,
                badgeWidth,
                badgeHeight);
            DrawRect(badge, urgent ? Hex("2b1714", 0.90f) : Hex("030405", 0.82f));
            DrawBorder(badge, durationAccent.WithAlpha(urgent ? 0.96f : 0.76f), urgent ? 2 : 1);
            GUI.Label(badge, durationLabel, CenterStyle(8, urgent ? Hex("f2c08f") : cursorWhite));
        }

        private void DrawFireFieldSurface(Rect rect, Color accent, float pulse, float drift, float edgeAlpha)
        {
            DrawRect(new Rect(rect.x, rect.yMax - rect.height * 0.14f, rect.width, rect.height * 0.14f), Hex("7c2f24", 0.24f + pulse * 0.08f));
            DrawBorder(Pad(rect, rect.width * 0.055f), accent.WithAlpha(edgeAlpha * (0.58f + pulse * 0.20f)), 1);
            for (int i = 0; i < 3; i++)
            {
                float wave = 0.45f + 0.55f * Mathf.Abs(Mathf.Sin((drift + i * 0.31f) * Mathf.PI * 2f));
                float width = rect.width * (0.055f + (i % 2) * 0.012f);
                float height = rect.height * (0.10f + wave * 0.13f);
                float x = rect.x + rect.width * (0.20f + i * 0.28f);
                float y = rect.yMax - rect.height * 0.11f - height;
                Color flame = i % 3 == 1 ? gold : i % 3 == 2 ? Hex("f1b06a") : accent;
                DrawRect(new Rect(x, y, width, height), flame.WithAlpha(0.48f + pulse * 0.18f));
                DrawRect(new Rect(x + width * 0.22f, y - rect.height * 0.025f, width * 0.56f, rect.height * 0.045f), flame.WithAlpha(0.36f));
            }
            for (int i = 0; i < 2; i++)
            {
                float x = rect.x + rect.width * (0.30f + i * 0.34f);
                float y = rect.y + rect.height * (0.23f + i * 0.14f);
                DrawRect(new Rect(x, y, Mathf.Max(2f, rect.width * 0.03f), Mathf.Max(2f, rect.height * 0.03f)), gold.WithAlpha(0.30f + pulse * 0.20f));
            }
        }

        private void DrawIceFieldSurface(Rect rect, Color accent, float pulse, float edgeAlpha)
        {
            Rect glaze = Pad(rect, rect.width * 0.055f);
            DrawRect(glaze, Hex("6baec7", 0.12f + pulse * 0.08f));
            DrawBorder(glaze, accent.WithAlpha(edgeAlpha * (0.72f + pulse * 0.18f)), 2);
            DrawBorder(Pad(glaze, glaze.width * 0.10f), Hex("d6f4ff", 0.24f + pulse * 0.16f), 1);
            float line = Mathf.Max(2f, rect.width * 0.025f);
            DrawRect(new Rect(rect.center.x - line * 0.5f, rect.y + rect.height * 0.18f, line, rect.height * 0.28f), Hex("d6f4ff", 0.52f));
            DrawRect(new Rect(rect.center.x, rect.y + rect.height * 0.44f, rect.width * 0.24f, line), Hex("d6f4ff", 0.52f));
            DrawRect(new Rect(rect.x + rect.width * 0.31f, rect.y + rect.height * 0.58f, rect.width * 0.20f, line), accent.WithAlpha(0.46f));
            DrawRect(new Rect(rect.x + rect.width * 0.31f, rect.y + rect.height * 0.58f, line, rect.height * 0.18f), accent.WithAlpha(0.46f));
            DrawRect(new Rect(rect.x + rect.width * 0.66f, rect.y + rect.height * 0.25f, line, rect.height * 0.17f), accent.WithAlpha(0.40f));
        }

        private void DrawGasFieldSurface(Rect rect, Color accent, float pulse, float drift, float edgeAlpha)
        {
            DrawBorder(Pad(rect, rect.width * 0.06f), accent.WithAlpha(edgeAlpha * 0.54f), 1);
            for (int i = 0; i < 4; i++)
            {
                float lane = 0.20f + i * 0.18f;
                float travel = Mathf.Repeat(drift + i * 0.23f, 1f);
                float width = rect.width * (0.22f + (i % 2) * 0.08f);
                float x = Mathf.Lerp(rect.x + rect.width * 0.06f, rect.xMax - rect.width * 0.06f - width, travel);
                float height = rect.height * (0.085f + (i % 2) * 0.025f);
                Color cloud = i % 2 == 0 ? accent : Hex("b2cf72");
                DrawRect(new Rect(x, rect.y + rect.height * lane, width, height), cloud.WithAlpha(0.20f + pulse * 0.15f));
                DrawRect(new Rect(x + width * 0.18f, rect.y + rect.height * lane - height * 0.55f, width * 0.34f, height), cloud.WithAlpha(0.16f + pulse * 0.12f));
            }
        }

        private void DrawSmokeFieldSurface(Rect rect, Color accent, float pulse, float drift, float edgeAlpha)
        {
            DrawBorder(Pad(rect, rect.width * 0.06f), accent.WithAlpha(edgeAlpha * 0.50f), 1);
            Color coolSmoke = Hex("71817f");
            Color paleSmoke = Hex("b7c2bd");
            for (int i = 0; i < 5; i++)
            {
                float lane = 0.16f + i * 0.145f;
                float travel = Mathf.Repeat(drift * 0.72f + i * 0.19f, 1f);
                float width = rect.width * (0.24f + (i % 3) * 0.055f);
                float x = Mathf.Lerp(rect.x - width * 0.14f, rect.xMax - width * 0.86f, travel);
                float height = rect.height * (0.095f + (i % 2) * 0.030f);
                Color cloud = i % 3 == 0 ? paleSmoke : i % 2 == 0 ? accent : coolSmoke;
                DrawRect(new Rect(x, rect.y + rect.height * lane, width, height), cloud.WithAlpha(0.18f + pulse * 0.12f));
                DrawRect(
                    new Rect(x + width * 0.22f, rect.y + rect.height * lane - height * 0.58f, width * 0.46f, height * 1.12f),
                    cloud.WithAlpha(0.14f + pulse * 0.10f));
            }
        }

        private void DrawWebFieldSurface(Rect rect, Color accent, float pulse, float edgeAlpha)
        {
            Rect web = Pad(rect, rect.width * 0.08f);
            Color thread = accent.WithAlpha(0.34f + pulse * 0.18f);
            float line = Mathf.Max(1f, rect.width * 0.018f);
            DrawBorder(web, accent.WithAlpha(edgeAlpha * 0.72f), 1);
            DrawBorder(Pad(web, web.width * 0.16f), thread, 1);
            DrawBorder(Pad(web, web.width * 0.31f), thread, 1);
            DrawRect(new Rect(web.x, web.center.y - line * 0.5f, web.width, line), thread);
            DrawRect(new Rect(web.center.x - line * 0.5f, web.y, line, web.height), thread);
            DrawRect(new Rect(web.x + web.width * 0.20f, web.y + web.height * 0.18f, web.width * 0.60f, line), thread);
            DrawRect(new Rect(web.x + web.width * 0.20f, web.y + web.height * 0.81f, web.width * 0.60f, line), thread);
        }

        private void DrawSanctuaryFieldSurface(Rect rect, Color accent, float pulse, float edgeAlpha)
        {
            Rect rune = Pad(rect, rect.width * 0.08f);
            Color warm = Color.Lerp(accent, gold, 0.44f);
            DrawBorder(rune, warm.WithAlpha(edgeAlpha * (0.76f + pulse * 0.18f)), 2);
            DrawBorder(Pad(rune, rune.width * 0.17f), accent.WithAlpha(0.34f + pulse * 0.18f), 1);
            DrawRect(new Rect(rune.center.x - rune.width * 0.035f, rune.y + rune.height * 0.20f, rune.width * 0.07f, rune.height * 0.60f), warm.WithAlpha(0.42f + pulse * 0.16f));
            DrawRect(new Rect(rune.x + rune.width * 0.20f, rune.center.y - rune.height * 0.035f, rune.width * 0.60f, rune.height * 0.07f), warm.WithAlpha(0.42f + pulse * 0.16f));
            float pip = Mathf.Max(3f, rect.width * 0.055f);
            DrawRect(new Rect(rune.x, rune.y, pip, pip), gold.WithAlpha(0.72f));
            DrawRect(new Rect(rune.xMax - pip, rune.y, pip, pip), gold.WithAlpha(0.72f));
            DrawRect(new Rect(rune.x, rune.yMax - pip, pip, pip), gold.WithAlpha(0.72f));
            DrawRect(new Rect(rune.xMax - pip, rune.yMax - pip, pip, pip), gold.WithAlpha(0.72f));
        }

        private void DrawCurseFieldSurface(Rect rect, Color accent, float pulse, float edgeAlpha)
        {
            Rect rune = Pad(rect, rect.width * 0.07f);
            Color bloodViolet = Color.Lerp(accent, blood, 0.42f);
            DrawBorder(rune, bloodViolet.WithAlpha(edgeAlpha * (0.70f + pulse * 0.24f)), 2);
            DrawBorder(Pad(rune, rune.width * 0.20f), accent.WithAlpha(0.36f + pulse * 0.18f), 1);
            float line = Mathf.Max(2f, rect.width * 0.026f);
            DrawRect(new Rect(rune.x + rune.width * 0.18f, rune.y + rune.height * 0.24f, rune.width * 0.40f, line), bloodViolet.WithAlpha(0.56f));
            DrawRect(new Rect(rune.x + rune.width * 0.55f, rune.y + rune.height * 0.24f, line, rune.height * 0.34f), bloodViolet.WithAlpha(0.56f));
            DrawRect(new Rect(rune.x + rune.width * 0.38f, rune.y + rune.height * 0.55f, rune.width * 0.20f, line), accent.WithAlpha(0.60f));
            DrawRect(new Rect(rune.x + rune.width * 0.38f, rune.y + rune.height * 0.55f, line, rune.height * 0.20f), accent.WithAlpha(0.60f));
            DrawRect(new Rect(rune.x + rune.width * 0.70f, rune.y + rune.height * 0.68f, line * 1.5f, line * 1.5f), blood.WithAlpha(0.68f + pulse * 0.20f));
        }

        private void DrawObstacleTacticalFrame(Rect rect, Point obstacle)
        {
            if (obstacle == null) return;
            Color accent = ObstacleAccent(obstacle.Kind);
            Rect frame = Pad(rect, rect.width * 0.075f);
            float corner = Mathf.Max(6f, rect.width * 0.13f);
            float cornerAlpha = 0.54f;
            DrawRect(new Rect(frame.x, frame.y, corner, 2f), accent.WithAlpha(cornerAlpha));
            DrawRect(new Rect(frame.x, frame.y, 2f, corner), accent.WithAlpha(cornerAlpha));
            DrawRect(new Rect(frame.xMax - corner, frame.y, corner, 2f), accent.WithAlpha(cornerAlpha));
            DrawRect(new Rect(frame.xMax - 2f, frame.y, 2f, corner), accent.WithAlpha(cornerAlpha));
        }

        private Color ObstacleAccent(string kind)
        {
            switch ((kind ?? "").ToLowerInvariant())
            {
                case "tree": return moss;
                case "stone": return Hex("a9b0a2");
                case "fire": return ember;
                case "ice": return frost;
                case "web": return Hex("d9d3c4");
                case "gas": return poison;
                case "smoke": return Hex("8fa7a2");
                case "sanctuary": return teal;
                case "curse": return violet;
                case "glyph": return Hex("d9d3c4");
                case "demonrift": return violet;
                default: return gold;
            }
        }

        private int CombatCoverBiomePropIndex(string kind)
        {
            switch ((kind ?? "").ToLowerInvariant())
            {
                case "tree": return 0;
                case "stone": return 8;
                default: return -1;
            }
        }

        private int CombatObstacleEnemyWorldIndex(string kind)
        {
            switch ((kind ?? "").ToLowerInvariant())
            {
                case "tree": return 17;
                case "stone": return 18;
                default: return -1;
            }
        }

        private int KoboldCavePropIndex(Point obstacle)
        {
            string kind = obstacle != null && !string.IsNullOrEmpty(obstacle.Kind) ? obstacle.Kind.ToLowerInvariant() : "stone";
            bool throneRoom = (state?.Combat?.EncounterStyle ?? "") == "koboldking";
            switch (kind)
            {
                case "tree": return throneRoom ? 7 : 0;
                case "stone": return throneRoom ? 6 : 1;
                case "web": return 2;
                case "gas": return 3;
                case "fire": return 4;
                case "ice": return 5;
                case "glyph": return throneRoom ? 10 : -1;
                case "demonrift": return throneRoom ? 9 : -1;
                default: return -1;
            }
        }

        private void DrawCoverIntegrityMarks(Rect rect, Point obstacle)
        {
            if (!IsBreakableCover(obstacle) || !CombatTileReadoutHovered(rect)) return;
            int max = CoverMaxIntegrity(obstacle.Kind);
            int current = CoverIntegrity(obstacle);
            DrawCoverIntegrityPips(rect, obstacle, current, max);
            // Generated cover art owns the tile body. Damage remains readable through
            // compact edge pips instead of procedural strokes painted over the sprite.
        }

        private void DrawCoverIntegrityPips(Rect rect, Point obstacle, int current, int max)
        {
            if (max <= 0) return;
            Color filled = obstacle.Kind == "tree" ? Hex("a8c06f", 0.92f) : Hex("d9d3c4", 0.86f);
            Color empty = Hex("050708", 0.72f);
            float pip = Mathf.Max(4f, rect.width * 0.075f);
            float gap = Mathf.Max(2f, rect.width * 0.025f);
            float total = max * pip + (max - 1) * gap;
            float start = rect.center.x - total * 0.5f;
            float y = rect.yMax - pip - rect.height * 0.08f;
            for (int i = 0; i < max; i++)
            {
                Rect p = new Rect(start + i * (pip + gap), y, pip, pip);
                DrawRect(p, i < current ? filled : empty);
                DrawBorder(p, Hex("030405", 0.74f), 1);
            }
            if (obstacle.Duration > 0)
            {
                Rect timer = new Rect(rect.xMax - rect.width * 0.26f, rect.y + rect.height * 0.08f, rect.width * 0.20f, Mathf.Max(12f, rect.height * 0.16f));
                DrawRect(timer, Hex("050708", 0.82f));
                DrawBorder(timer, filled, 1);
                GUI.Label(new Rect(timer.x, timer.y - 1f, timer.width, timer.height + 2f), obstacle.Duration.ToString(), CenterStyle(8, cursorWhite));
            }
        }

        private void DrawExploreObject(Rect cell, Rect rect, MapObject obj)
        {
            if (obj == null) return;
            bool routeObject = IsKoboldStoryObject(obj);
            Rect routeArt = Pad(rect, rect.width * 0.02f);
            if (routeObject)
            {
                Rect shadowArt = new Rect(routeArt.x + 2f, routeArt.y + 3f, routeArt.width, routeArt.height);
                TryDrawKoboldRouteArtIcon(shadowArt, KoboldRouteMarkerIndex(), Hex("020303", 0.80f));
            }
            if (routeObject && TryDrawKoboldRouteArtIcon(routeArt, KoboldRouteMarkerIndex(), Color.white))
            {
                Color accent = HasStoryFlag(StoryFlags.KoboldCaveCleared) ? blood : gold;
                float pulse = state != null && state.ReducedMotion ? 0.32f : 0.32f + Mathf.Sin(Time.time * 4.4f) * 0.10f;
                DrawBorder(Pad(rect, rect.width * 0.04f), accent.WithAlpha(0.78f + pulse), 2);
                DrawRect(new Rect(rect.x + rect.width * 0.23f, rect.yMax - rect.height * 0.18f, rect.width * 0.54f, rect.height * 0.06f), accent.WithAlpha(0.82f));
                return;
            }

            DrawExploreObject(cell, rect, obj.Type, obj);
            if (routeObject) DrawKoboldRouteCaveBadge(rect);
        }

        private void DrawExploreObject(Rect rect, ObjectType type)
        {
            DrawExploreObject(rect, rect, type, null);
        }

        private void DrawExploreObject(Rect cell, Rect rect, ObjectType type, MapObject obj)
        {
            Color objectColor = ObjectColor(type);
            bool quiet = IsQuietExploreObject(type);
            bool framed = ShouldFrameExploreObject(type, obj);
            bool gate = IsMidgaardGateType(type);
            float pulse = quiet || state != null && state.ReducedMotion ? 0.35f : 0.35f + Mathf.Sin(Time.time * 3.5f + (int)type) * 0.12f;
            if (gate) DrawMidgaardGateFoundation(cell, rect, type, framed);
            else DrawExploreObjectPlinth(rect, objectColor, quiet, framed, pulse);
            Rect artRect = Pad(rect, rect.width * ExploreObjectArtPadding(type, quiet));
            if (!gate)
            {
                Rect shadowRect = new Rect(artRect.x + 2f, artRect.y + 3f, artRect.width, artRect.height);
                TryDrawWorldObjectIcon(shadowRect, type, obj, Hex("020303", 0.82f));
            }
            if (TryDrawWorldObjectIcon(artRect, type, obj))
            {
                if (!gate && ShouldReinforceExploreObjectAlpha(type))
                {
                    // Some legacy gate/scaffold exports contain a large amount of
                    // semi-transparent core art. A restrained second pass restores
                    // solidity without bringing back opaque square backings.
                    TryDrawWorldObjectIcon(artRect, type, obj, Color.white.WithAlpha(0.62f));
                }
                if (gate) DrawMidgaardGateFrame(rect, objectColor, framed);
                else DrawExploreObjectFrame(rect, objectColor, quiet, framed, pulse);
                return;
            }

            Rect inner = Pad(rect, rect.width * 0.16f);
            switch (type)
            {
                case ObjectType.Cache:
                    DrawRect(new Rect(inner.x + inner.width * 0.08f, inner.y + inner.height * 0.72f, inner.width * 0.84f, inner.height * 0.08f), Hex("050708", 0.45f));
                    DrawRect(new Rect(inner.x + inner.width * 0.03f, inner.y + inner.height * 0.28f, inner.width * 0.94f, inner.height * 0.48f), Hex("5b331f"));
                    DrawRect(new Rect(inner.x + inner.width * 0.10f, inner.y + inner.height * 0.12f, inner.width * 0.80f, inner.height * 0.28f), Hex("9a6a3e"));
                    DrawRect(new Rect(inner.x + inner.width * 0.15f, inner.y + inner.height * 0.42f, inner.width * 0.70f, inner.height * 0.08f), gold);
                    DrawRect(new Rect(inner.x + inner.width * 0.26f, inner.y + inner.height * 0.22f, inner.width * 0.08f, inner.height * 0.52f), Hex("24150e", 0.70f));
                    DrawRect(new Rect(inner.x + inner.width * 0.66f, inner.y + inner.height * 0.22f, inner.width * 0.08f, inner.height * 0.52f), Hex("24150e", 0.70f));
                    DrawRect(new Rect(inner.x + inner.width * 0.44f, inner.y + inner.height * 0.45f, inner.width * 0.12f, inner.height * 0.18f), Hex("050708"));
                    DrawRect(new Rect(inner.x + inner.width * 0.62f, inner.y + inner.height * 0.17f, inner.width * 0.14f, inner.height * 0.07f), Hex("f3ead7", 0.50f));
                    DrawBorder(new Rect(inner.x + inner.width * 0.03f, inner.y + inner.height * 0.18f, inner.width * 0.94f, inner.height * 0.58f), Hex("d9d3c4", 0.48f), 1);
                    break;
                case ObjectType.Shrine:
                    DrawRect(new Rect(inner.x + inner.width * 0.08f, inner.y + inner.height * 0.72f, inner.width * 0.84f, inner.height * 0.12f), stone);
                    DrawRect(new Rect(inner.x + inner.width * 0.18f, inner.y + inner.height * 0.56f, inner.width * 0.64f, inner.height * 0.18f), Hex("303936"));
                    DrawRect(new Rect(inner.x + inner.width * 0.28f, inner.y + inner.height * 0.16f, inner.width * 0.44f, inner.height * 0.48f), Hex("214b47"));
                    DrawRect(new Rect(inner.x + inner.width * 0.34f, inner.y + inner.height * 0.20f, inner.width * 0.32f, inner.height * 0.34f), teal.WithAlpha(0.80f));
                    DrawRect(new Rect(rect.center.x - rect.width * 0.028f, inner.y + inner.height * 0.08f, rect.width * 0.056f, inner.height * 0.60f), ink);
                    DrawRect(new Rect(inner.x + inner.width * 0.28f, rect.center.y - rect.height * 0.030f, inner.width * 0.44f, rect.height * 0.06f), ink);
                    DrawRect(new Rect(inner.x + inner.width * 0.08f, inner.y + inner.height * 0.82f, inner.width * 0.14f, inner.height * 0.08f), gold);
                    DrawRect(new Rect(inner.x + inner.width * 0.78f, inner.y + inner.height * 0.82f, inner.width * 0.14f, inner.height * 0.08f), gold);
                    break;
                case ObjectType.Encounter:
                    DrawRect(new Rect(inner.x + inner.width * 0.20f, inner.y + inner.height * 0.68f, inner.width * 0.60f, inner.height * 0.08f), Hex("050708", 0.45f));
                    DrawRect(new Rect(inner.x + inner.width * 0.12f, inner.y + inner.height * 0.22f, inner.width * 0.76f, inner.height * 0.42f), blood);
                    DrawRect(new Rect(inner.x + inner.width * 0.20f, inner.y + inner.height * 0.08f, inner.width * 0.18f, inner.height * 0.24f), Hex("d9d3c4"));
                    DrawRect(new Rect(inner.x + inner.width * 0.62f, inner.y + inner.height * 0.08f, inner.width * 0.18f, inner.height * 0.24f), Hex("d9d3c4"));
                    DrawRect(new Rect(inner.x + inner.width * 0.32f, inner.y + inner.height * 0.34f, inner.width * 0.09f, inner.height * 0.09f), Hex("050708"));
                    DrawRect(new Rect(inner.x + inner.width * 0.59f, inner.y + inner.height * 0.34f, inner.width * 0.09f, inner.height * 0.09f), Hex("050708"));
                    DrawRect(new Rect(inner.x + inner.width * 0.42f, inner.y + inner.height * 0.52f, inner.width * 0.16f, inner.height * 0.08f), Hex("050708"));
                    DrawRect(new Rect(inner.x + inner.width * 0.22f, inner.y + inner.height * 0.64f, inner.width * 0.13f, inner.height * 0.18f), Hex("d9d3c4", 0.65f));
                    DrawRect(new Rect(inner.x + inner.width * 0.65f, inner.y + inner.height * 0.64f, inner.width * 0.13f, inner.height * 0.18f), Hex("d9d3c4", 0.65f));
                    break;
                case ObjectType.Stairs:
                    DrawRect(new Rect(inner.x + inner.width * 0.08f, inner.y + inner.height * 0.08f, inner.width * 0.84f, inner.height * 0.78f), Hex("030405", 0.84f));
                    DrawBorder(new Rect(inner.x + inner.width * 0.08f, inner.y + inner.height * 0.08f, inner.width * 0.84f, inner.height * 0.78f), stone, 1);
                    for (int i = 0; i < 5; i++)
                    {
                        DrawRect(new Rect(inner.x + inner.width * (0.14f + i * 0.035f), inner.y + inner.height * (0.18f + i * 0.12f), inner.width * (0.72f - i * 0.04f), rect.height * 0.065f), Color.Lerp(stone, ink, i * 0.09f));
                    }
                    DrawRect(new Rect(inner.x + inner.width * 0.12f, inner.y + inner.height * 0.14f, inner.width * 0.10f, inner.height * 0.46f), gold.WithAlpha(0.55f));
                    DrawRect(new Rect(inner.x + inner.width * 0.78f, inner.y + inner.height * 0.14f, inner.width * 0.10f, inner.height * 0.46f), frost.WithAlpha(0.38f));
                    break;
                case ObjectType.Camp:
                    DrawRect(new Rect(inner.x + inner.width * 0.06f, inner.y + inner.height * 0.70f, inner.width * 0.88f, inner.height * 0.08f), Hex("050708", 0.42f));
                    DrawRect(new Rect(inner.x + inner.width * 0.10f, inner.y + inner.height * 0.42f, inner.width * 0.30f, inner.height * 0.28f), Hex("53646a"));
                    DrawRect(new Rect(inner.x + inner.width * 0.18f, inner.y + inner.height * 0.24f, inner.width * 0.24f, inner.height * 0.22f), Hex("6f8376"));
                    DrawRect(new Rect(inner.x + inner.width * 0.52f, inner.y + inner.height * 0.64f, inner.width * 0.38f, inner.height * 0.08f), Hex("5b3a25"));
                    DrawRect(new Rect(inner.x + inner.width * 0.56f, inner.y + inner.height * 0.58f, inner.width * 0.30f, inner.height * 0.07f), Hex("8a5c35"));
                    DrawRect(new Rect(inner.x + inner.width * 0.60f, inner.y + inner.height * 0.38f, inner.width * 0.10f, inner.height * 0.26f), ember);
                    DrawRect(new Rect(inner.x + inner.width * 0.70f, inner.y + inner.height * 0.24f, inner.width * 0.10f, inner.height * 0.40f), gold);
                    DrawRect(new Rect(inner.x + inner.width * 0.80f, inner.y + inner.height * 0.45f, inner.width * 0.09f, inner.height * 0.18f), Hex("d98b6a"));
                    break;
                case ObjectType.Obelisk:
                    DrawRect(new Rect(inner.x + inner.width * 0.24f, inner.y + inner.height * 0.78f, inner.width * 0.52f, inner.height * 0.10f), stone);
                    DrawRect(new Rect(inner.x + inner.width * 0.34f, inner.y + inner.height * 0.24f, inner.width * 0.32f, inner.height * 0.56f), Hex("4d5856"));
                    DrawRect(new Rect(inner.x + inner.width * 0.39f, inner.y + inner.height * 0.12f, inner.width * 0.22f, inner.height * 0.16f), Hex("6f7c76"));
                    DrawRect(new Rect(inner.x + inner.width * 0.47f, inner.y + inner.height * 0.34f, inner.width * 0.06f, inner.height * 0.26f), teal.WithAlpha(0.75f));
                    DrawRect(new Rect(inner.x + inner.width * 0.40f, inner.y + inner.height * 0.48f, inner.width * 0.20f, inner.height * 0.05f), teal.WithAlpha(0.62f));
                    break;
                case ObjectType.Ruin:
                    DrawRect(new Rect(inner.x + inner.width * 0.10f, inner.y + inner.height * 0.72f, inner.width * 0.80f, inner.height * 0.10f), Hex("050708", 0.40f));
                    DrawRect(new Rect(inner.x + inner.width * 0.16f, inner.y + inner.height * 0.34f, inner.width * 0.18f, inner.height * 0.42f), stone);
                    DrawRect(new Rect(inner.x + inner.width * 0.42f, inner.y + inner.height * 0.20f, inner.width * 0.16f, inner.height * 0.56f), Hex("68706b"));
                    DrawRect(new Rect(inner.x + inner.width * 0.66f, inner.y + inner.height * 0.48f, inner.width * 0.16f, inner.height * 0.28f), stone);
                    DrawRect(new Rect(inner.x + inner.width * 0.24f, inner.y + inner.height * 0.26f, inner.width * 0.32f, inner.height * 0.06f), Hex("a9b0a2", 0.30f));
                    DrawRect(new Rect(inner.x + inner.width * 0.58f, inner.y + inner.height * 0.70f, inner.width * 0.28f, inner.height * 0.06f), moss.WithAlpha(0.45f));
                    break;
                case ObjectType.Bridge:
                    DrawRect(new Rect(inner.x + inner.width * 0.04f, inner.y + inner.height * 0.38f, inner.width * 0.92f, inner.height * 0.30f), Hex("3b6b67", 0.35f));
                    DrawRect(new Rect(inner.x + inner.width * 0.08f, inner.y + inner.height * 0.44f, inner.width * 0.84f, inner.height * 0.18f), Hex("5b3a25"));
                    DrawRect(new Rect(inner.x + inner.width * 0.12f, inner.y + inner.height * 0.38f, inner.width * 0.76f, inner.height * 0.06f), Hex("8a5c35"));
                    for (int i = 0; i < 4; i++) DrawRect(new Rect(inner.x + inner.width * (0.18f + i * 0.16f), inner.y + inner.height * 0.39f, inner.width * 0.05f, inner.height * 0.27f), Hex("24150e", 0.54f));
                    DrawRect(new Rect(inner.x + inner.width * 0.06f, inner.y + inner.height * 0.28f, inner.width * 0.05f, inner.height * 0.48f), Hex("9b6b45"));
                    DrawRect(new Rect(inner.x + inner.width * 0.89f, inner.y + inner.height * 0.28f, inner.width * 0.05f, inner.height * 0.48f), Hex("9b6b45"));
                    break;
                case ObjectType.Cave:
                    DrawRect(new Rect(inner.x + inner.width * 0.10f, inner.y + inner.height * 0.70f, inner.width * 0.80f, inner.height * 0.10f), Hex("050708", 0.48f));
                    DrawRect(new Rect(inner.x + inner.width * 0.12f, inner.y + inner.height * 0.34f, inner.width * 0.76f, inner.height * 0.38f), stone);
                    DrawRect(new Rect(inner.x + inner.width * 0.24f, inner.y + inner.height * 0.20f, inner.width * 0.52f, inner.height * 0.30f), Hex("68706b"));
                    DrawRect(new Rect(inner.x + inner.width * 0.30f, inner.y + inner.height * 0.38f, inner.width * 0.40f, inner.height * 0.36f), Hex("030405"));
                    DrawRect(new Rect(inner.x + inner.width * 0.37f, inner.y + inner.height * 0.48f, inner.width * 0.26f, inner.height * 0.22f), Hex("101619"));
                    DrawRect(new Rect(inner.x + inner.width * 0.18f, inner.y + inner.height * 0.70f, inner.width * 0.18f, inner.height * 0.06f), moss.WithAlpha(0.46f));
                    break;
                case ObjectType.Town:
                    DrawRect(new Rect(inner.x + inner.width * 0.03f, inner.y + inner.height * 0.52f, inner.width * 0.94f, inner.height * 0.30f), moss);
                    DrawRect(new Rect(inner.x - rect.width * 0.04f, inner.y + inner.height * 0.32f, inner.width + rect.width * 0.08f, inner.height * 0.24f), gold);
                    DrawRect(new Rect(inner.x + inner.width * 0.12f, inner.y + inner.height * 0.22f, inner.width * 0.17f, inner.height * 0.36f), stone);
                    DrawRect(new Rect(inner.x + inner.width * 0.41f, inner.y + inner.height * 0.18f, inner.width * 0.18f, inner.height * 0.40f), Hex("5f6b64"));
                    DrawRect(new Rect(inner.x + inner.width * 0.70f, inner.y + inner.height * 0.22f, inner.width * 0.17f, inner.height * 0.36f), stone);
                    DrawRect(new Rect(inner.x + inner.width * 0.44f, inner.y + inner.height * 0.60f, inner.width * 0.12f, inner.height * 0.22f), Hex("050708"));
                    DrawRect(new Rect(inner.x + inner.width * 0.18f, inner.y + inner.height * 0.65f, inner.width * 0.08f, inner.height * 0.08f), teal.WithAlpha(0.65f));
                    DrawRect(new Rect(inner.x + inner.width * 0.74f, inner.y + inner.height * 0.65f, inner.width * 0.08f, inner.height * 0.08f), teal.WithAlpha(0.65f));
                    break;
                case ObjectType.MarketClerk:
                case ObjectType.TempleHealer:
                case ObjectType.TavernKeeper:
                case ObjectType.GateCaptain:
                    DrawMidgaardNpcPlaceholder(inner, type);
                    break;
                default:
                    if (IsMidgaardNpcObject(type)) DrawMidgaardNpcPlaceholder(inner, type);
                    else if (IsRouteScaffoldObject(type)) DrawRouteScaffoldPlaceholder(inner, type);
                    break;
            }
        }

        private bool ShouldReinforceExploreObjectAlpha(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Sewer:
                case ObjectType.RatPeltQuest:
                    return true;
                default:
                    return IsRouteScaffoldObject(type);
            }
        }

        private void DrawMidgaardWallTerrainUnderlay(Rect rect, int x, int y)
        {
            if (state?.Map == null) return;

            int left = MidgaardLeft(state.Map);
            int right = MidgaardRight(state.Map);
            int top = MidgaardTop(state.Map);
            int bottom = MidgaardBottom(state.Map);
            bool onLeft = x == left;
            bool onRight = x == right;
            bool onTop = y == top;
            bool onBottom = y == bottom;
            if (!onLeft && !onRight && !onTop && !onBottom) return;

            int inwardX = x;
            int inwardY = y;
            int outwardX = x;
            int outwardY = y;
            if (onLeft)
            {
                inwardX++;
                outwardX--;
            }
            else if (onRight)
            {
                inwardX--;
                outwardX++;
            }
            if (onTop)
            {
                inwardY++;
                outwardY--;
            }
            else if (onBottom)
            {
                inwardY--;
                outwardY++;
            }

            Color outside = MidgaardWallTerrainColorAt(outwardX, outwardY, Hex("202625"));
            Color inside = MidgaardWallTerrainColorAt(inwardX, inwardY, Hex("51534d"));
            bool verticalEdge = (onLeft || onRight) && !onTop && !onBottom;
            bool horizontalEdge = (onTop || onBottom) && !onLeft && !onRight;

            if (verticalEdge)
            {
                Rect outsideHalf = onLeft
                    ? new Rect(rect.x, rect.y, rect.width * 0.5f, rect.height)
                    : new Rect(rect.center.x, rect.y, rect.width * 0.5f, rect.height);
                Rect insideHalf = onLeft
                    ? new Rect(rect.center.x, rect.y, rect.width * 0.5f, rect.height)
                    : new Rect(rect.x, rect.y, rect.width * 0.5f, rect.height);
                DrawRect(outsideHalf, outside);
                DrawRect(insideHalf, inside);
                TryDrawMidgaardWallTerrainSample(
                    outsideHalf,
                    outwardX,
                    outwardY,
                    onLeft ? 0.5f : 0f,
                    0f,
                    0.5f,
                    1f);
                TryDrawMidgaardWallTerrainSample(
                    insideHalf,
                    inwardX,
                    inwardY,
                    onLeft ? 0f : 0.5f,
                    0f,
                    0.5f,
                    1f);
                return;
            }

            if (horizontalEdge)
            {
                Rect outsideHalf = onTop
                    ? new Rect(rect.x, rect.y, rect.width, rect.height * 0.5f)
                    : new Rect(rect.x, rect.center.y, rect.width, rect.height * 0.5f);
                Rect insideHalf = onTop
                    ? new Rect(rect.x, rect.center.y, rect.width, rect.height * 0.5f)
                    : new Rect(rect.x, rect.y, rect.width, rect.height * 0.5f);
                DrawRect(outsideHalf, outside);
                DrawRect(insideHalf, inside);
                TryDrawMidgaardWallTerrainSample(
                    outsideHalf,
                    outwardX,
                    outwardY,
                    0f,
                    onTop ? 0.5f : 0f,
                    1f,
                    0.5f);
                TryDrawMidgaardWallTerrainSample(
                    insideHalf,
                    inwardX,
                    inwardY,
                    0f,
                    onTop ? 0f : 0.5f,
                    1f,
                    0.5f);
                return;
            }

            // Corners preserve the outside field in three quadrants and the city
            // field in the inward quadrant, now with the same authored material
            // texture as their neighboring cells instead of flat color blocks.
            DrawRect(rect, outside);
            TryDrawMidgaardWallTerrainSample(rect, outwardX, outwardY, 0f, 0f, 1f, 1f);
            Rect inner = new Rect(
                onLeft ? rect.center.x : rect.x,
                onTop ? rect.center.y : rect.y,
                rect.width * 0.5f,
                rect.height * 0.5f);
            DrawRect(inner, inside);
            TryDrawMidgaardWallTerrainSample(
                inner,
                inwardX,
                inwardY,
                onLeft ? 0f : 0.5f,
                onTop ? 0f : 0.5f,
                0.5f,
                0.5f);
        }

        private bool TryDrawMidgaardWallTerrainSample(
            Rect destination,
            int sampleX,
            int sampleY,
            float logicalSourceX,
            float logicalSourceY,
            float sourceWidth,
            float sourceHeight)
        {
            if (state?.Map == null
                || sampleX < 0
                || sampleY < 0
                || sampleX >= state.Map.Width
                || sampleY >= state.Map.Height)
            {
                return false;
            }

            ExplorationMaterial material = ExploreMaterialAt(sampleX, sampleY);
            if (!TryResolveWorldMapMaterialAtlasSample(
                material,
                sampleX,
                sampleY,
                HasStaticExploreObjectFootprint(sampleX, sampleY),
                out Rect source,
                out bool flipX,
                out bool flipY))
            {
                return false;
            }

            sourceWidth = Mathf.Clamp(sourceWidth, 0.01f, 1f);
            sourceHeight = Mathf.Clamp(sourceHeight, 0.01f, 1f);
            logicalSourceX = Mathf.Clamp(logicalSourceX, 0f, 1f - sourceWidth);
            logicalSourceY = Mathf.Clamp(logicalSourceY, 0f, 1f - sourceHeight);
            float sourceX = flipX ? 1f - logicalSourceX - sourceWidth : logicalSourceX;
            float sourceY = flipY ? 1f - logicalSourceY - sourceHeight : logicalSourceY;
            Rect croppedSource = new Rect(
                source.x + source.width * sourceX,
                source.y + source.height * sourceY,
                source.width * sourceWidth,
                source.height * sourceHeight);
            int tile = TileAt(state.Map, sampleX, sampleY);
            string kind = ExploreTileKind(sampleX, sampleY, tile);
            float noise = (ExploreNoise(sampleX, sampleY, 43) % 9) / 8f;
            float alpha = ExplorationReadabilityRules.TerrainArtAlpha(tile, kind, exploreWideView, noise);
            return DrawTextureRegionTintVariant(
                worldMapMaterialAtlas,
                destination,
                croppedSource,
                Color.white.WithAlpha(alpha),
                flipX,
                flipY);
        }

        private Color MidgaardWallTerrainColorAt(int x, int y, Color fallback)
        {
            if (state?.Map == null
                || x < 0
                || y < 0
                || x >= state.Map.Width
                || y >= state.Map.Height)
            {
                return fallback;
            }

            int tile = TileAt(state.Map, x, y);
            if (ExploreTileKind(x, y, tile) == "midgaardwall") return fallback;
            return ExploreTileBaseColor(x, y, tile, true);
        }

        private void DrawMidgaardWallFoundation(Rect rect, int atlasIndex, int x, int y)
        {
            DrawMidgaardWallTerrainUnderlay(rect, x, y);

            const int north = 1;
            const int east = 2;
            const int south = 4;
            const int west = 8;
            int connections = ExplorationArtRules.MidgaardWallConnectionMask(atlasIndex);
            if (connections == 0) return;
            bool connectsNorth = (connections & north) != 0;
            bool connectsEast = (connections & east) != 0;
            bool connectsSouth = (connections & south) != 0;
            bool connectsWest = (connections & west) != 0;
            bool horizontal = connectsEast || connectsWest;
            bool vertical = connectsNorth || connectsSouth;

            float horizontalThickness = ExplorationArtRules.MidgaardWallBandThickness(exploreWideView);
            float verticalThickness = ExplorationArtRules.MidgaardWallVerticalBandThickness(exploreWideView);
            float overlap = Mathf.Max(1f, rect.width * 0.035f);
            Color foundation = Hex("111717", 0.98f);
            Color masonry = Hex("303938", 0.96f);
            Color cap = Hex("737b76", 0.78f);
            Color mortar = Hex("171e1e", 0.78f);

            if (horizontal)
            {
                float height = rect.height * horizontalThickness;
                float bandX = connectsWest ? rect.x - overlap : rect.center.x - overlap;
                float width = connectsEast && connectsWest
                    ? rect.width + overlap * 2f
                    : rect.width * 0.5f + overlap * 2f;
                Rect band = new Rect(
                    bandX,
                    rect.yMax - height,
                    width,
                    height + overlap);
                DrawRect(band, foundation);
                DrawRect(
                    new Rect(
                        band.x,
                        band.y + band.height * 0.14f,
                        band.width,
                        band.height * 0.72f),
                    masonry);
                DrawRect(
                    new Rect(
                        band.x,
                        band.y + band.height * 0.10f,
                        band.width,
                        Mathf.Max(1f, band.height * 0.12f)),
                    cap);
                DrawRect(
                    new Rect(
                        band.x,
                        band.y + band.height * 0.58f,
                        band.width,
                        Mathf.Max(1f, band.height * 0.055f)),
                    mortar);
            }

            if (vertical)
            {
                float width = rect.width * verticalThickness;
                float bandY = connectsNorth ? rect.y - overlap : rect.center.y - overlap;
                float height = connectsNorth && connectsSouth
                    ? rect.height + overlap * 2f
                    : rect.height * 0.5f + overlap * 2f;
                Rect band = new Rect(
                    rect.center.x - width * 0.5f,
                    bandY,
                    width,
                    height);
                DrawRect(band, foundation);
                DrawRect(
                    new Rect(
                        band.x + band.width * 0.14f,
                        band.y,
                        band.width * 0.72f,
                        band.height),
                    masonry);
                DrawRect(
                    new Rect(
                        band.x + band.width * 0.10f,
                        band.y,
                        Mathf.Max(1f, band.width * 0.12f),
                        band.height),
                    cap);
                DrawRect(
                    new Rect(
                        band.x + band.width * 0.58f,
                        band.y,
                        Mathf.Max(1f, band.width * 0.055f),
                        band.height),
                    mortar);
            }

            // Subtle coordinate-stable joints keep the continuous foundation from
            // reading as a flat UI bar at region scale.
            if (!exploreWideView && ((x + y) & 1) == 0)
            {
                if (horizontal)
                {
                    DrawRect(
                        new Rect(
                            rect.center.x - Mathf.Max(1f, rect.width * 0.025f),
                            rect.yMax - rect.height * horizontalThickness * 0.46f,
                            Mathf.Max(1f, rect.width * 0.05f),
                            rect.height * horizontalThickness * 0.34f),
                        mortar.WithAlpha(0.52f));
                }
                if (vertical)
                {
                    DrawRect(
                        new Rect(
                            rect.center.x - rect.width * verticalThickness * 0.17f,
                            rect.center.y - Mathf.Max(1f, rect.height * 0.025f),
                            rect.width * verticalThickness * 0.34f,
                            Mathf.Max(1f, rect.height * 0.05f)),
                        mortar.WithAlpha(0.52f));
                }
            }
        }

        private bool CombatTileReadoutHovered(Rect rect)
        {
            if (visualSmokeCombatHoverCell.HasValue && boardRect.width > 0f && boardRect.height > 0f)
            {
                Vector2Int staged = visualSmokeCombatHoverCell.Value;
                if (staged.x >= 0 && staged.x < CombatW && staged.y >= 0 && staged.y < CombatH)
                {
                    Rect grid = CombatBoardInnerRect(boardRect);
                    float cell = Mathf.Min(grid.width / CombatW, grid.height / CombatH);
                    Vector2 stagedCenter = new Vector2(
                        grid.x + (staged.x + 0.5f) * cell,
                        grid.y + (staged.y + 0.5f) * cell);
                    return rect.Contains(stagedCenter);
                }
            }
            return Event.current != null && rect.Contains(Event.current.mousePosition);
        }

        private void DrawMidgaardGateFoundation(Rect cell, Rect rect, ObjectType type, bool framed)
        {
            bool open = type == ObjectType.EastGate || type == ObjectType.WestGate;
            if (!open)
            {
                float sillHeight = Mathf.Max(3f, rect.height * 0.09f);
                Rect groundShadow = new Rect(
                    rect.x + rect.width * 0.10f,
                    rect.yMax - sillHeight * 0.56f,
                    rect.width * 0.80f,
                    sillHeight * 0.48f);
                DrawRect(groundShadow, Hex("020303", 0.90f));
            }

            if (open)
            {
                // East and west gates are passable horizontal thresholds through
                // vertical walls. Build this join in map-cell space: art-rect space
                // is deliberately taller than the cell and used to put the road
                // well below the actual route.
                float wallWidth = cell.width * ExplorationArtRules.MidgaardWallVerticalBandThickness(exploreWideView);
                Rect upperJoin = new Rect(
                    cell.center.x - wallWidth * 0.5f,
                    cell.y - cell.height * 0.14f,
                    wallWidth,
                    cell.height * 0.42f);
                Rect lowerJoin = new Rect(
                    cell.center.x - wallWidth * 0.5f,
                    cell.y + cell.height * 0.72f,
                    wallWidth,
                    cell.height * 0.42f);
                DrawRect(upperJoin, Hex("26302f", 0.90f));
                DrawRect(lowerJoin, Hex("26302f", 0.90f));
            }
            else
            {
                // The north and south landmarks are sealed. A low-contrast backing
                // joins their transparent masonry to the wall without suggesting an
                // open portal behind the closed doors.
                Rect backing = new Rect(
                    rect.center.x - rect.width * 0.20f,
                    rect.y + rect.height * 0.40f,
                    rect.width * 0.40f,
                    rect.height * 0.38f);
                DrawRect(backing, Hex("161c1a", 0.46f));
            }

            if (!open)
            {
                float sillHeight = Mathf.Max(3f, rect.height * 0.09f);
                Rect sill = new Rect(
                    rect.x + rect.width * 0.18f,
                    rect.yMax - sillHeight,
                    rect.width * 0.64f,
                    sillHeight * 0.42f);
                DrawRect(sill, Hex("4d5856", 0.58f));
                DrawRect(new Rect(sill.x, sill.y, sill.width, Mathf.Max(1f, sill.height * 0.18f)), Hex("a9b0a2", 0.30f));
            }
            if (framed) DrawRect(Pad(rect, rect.width * 0.17f), ObjectColor(type).WithAlpha(0.06f));
        }

        private void DrawMidgaardGateFrame(Rect rect, Color color, bool framed)
        {
            if (!framed) return;
            DrawCornerBrackets(Pad(rect, rect.width * 0.055f), color.WithAlpha(0.90f), 2f, rect.width * 0.14f);
        }

        private void DrawRitualIntegrityMarks(Rect rect, Point ritual)
        {
            if (!CombatRitualRules.IsRitual(ritual)) return;
            int max = CombatRitualRules.MaxIntegrity(ritual.Kind);
            int current = RitualIntegrity(ritual);
            Color accent = ObstacleAccent(ritual.Kind);
            Color empty = Hex("050708", 0.84f);
            if (CombatTileReadoutHovered(rect))
            {
                float pip = Mathf.Max(5f, rect.width * 0.08f);
                float gap = Mathf.Max(2f, rect.width * 0.025f);
                float total = max * pip + Mathf.Max(0, max - 1) * gap;
                float start = rect.center.x - total * 0.5f;
                float y = rect.yMax - pip - rect.height * 0.075f;
                for (int i = 0; i < max; i++)
                {
                    Rect mark = new Rect(start + i * (pip + gap), y, pip, pip);
                    DrawRect(mark, i < current ? accent.WithAlpha(0.96f) : empty);
                    DrawBorder(mark, Hex("030405", 0.90f), 1);
                }
            }

            float badgeSize = Mathf.Max(16f, rect.width * 0.21f);
            Rect countdown = new Rect(rect.xMax - badgeSize - rect.width * 0.06f, rect.y + rect.height * 0.06f, badgeSize, badgeSize);
            DrawRect(countdown, Hex("030405", 0.90f));
            DrawBorder(countdown, accent.WithAlpha(0.96f), 1);
            GUI.Label(countdown, Mathf.Max(1, ritual.Duration).ToString(), CenterStyle(Mathf.Clamp(Mathf.RoundToInt(rect.width * 0.12f), 8, 12), cursorWhite));

            if (ritual.Duration <= 1)
            {
                float pulse = state != null && state.ReducedMotion ? 0.46f : 0.48f + Mathf.Sin(Time.time * 4.8f) * 0.16f;
                DrawBorder(Pad(rect, rect.width * 0.045f), accent.WithAlpha(Mathf.Clamp01(pulse)), 2);
            }
        }

        private void DrawMidgaardNpcPlaceholder(Rect inner, ObjectType type)
        {
            Color accent = ObjectColor(type);
            DrawRect(new Rect(inner.x + inner.width * 0.18f, inner.y + inner.height * 0.78f, inner.width * 0.64f, inner.height * 0.08f), Hex("050708", 0.46f));
            DrawRect(new Rect(inner.x + inner.width * 0.38f, inner.y + inner.height * 0.18f, inner.width * 0.24f, inner.height * 0.22f), Hex("d9b18c"));
            DrawRect(new Rect(inner.x + inner.width * 0.31f, inner.y + inner.height * 0.40f, inner.width * 0.38f, inner.height * 0.34f), accent.WithAlpha(0.92f));
            DrawRect(new Rect(inner.x + inner.width * 0.35f, inner.y + inner.height * 0.74f, inner.width * 0.10f, inner.height * 0.12f), Hex("2c211a"));
            DrawRect(new Rect(inner.x + inner.width * 0.55f, inner.y + inner.height * 0.74f, inner.width * 0.10f, inner.height * 0.12f), Hex("2c211a"));
            DrawRect(new Rect(inner.x + inner.width * 0.36f, inner.y + inner.height * 0.24f, inner.width * 0.08f, inner.height * 0.06f), Hex("050708"));
            DrawRect(new Rect(inner.x + inner.width * 0.56f, inner.y + inner.height * 0.24f, inner.width * 0.08f, inner.height * 0.06f), Hex("050708"));

            switch (type)
            {
                case ObjectType.MarketClerk:
                    DrawRect(new Rect(inner.x + inner.width * 0.62f, inner.y + inner.height * 0.46f, inner.width * 0.18f, inner.height * 0.22f), Hex("d9d3c4"));
                    DrawRect(new Rect(inner.x + inner.width * 0.65f, inner.y + inner.height * 0.51f, inner.width * 0.12f, inner.height * 0.03f), gold);
                    break;
                case ObjectType.TempleHealer:
                    DrawRect(new Rect(inner.x + inner.width * 0.44f, inner.y + inner.height * 0.08f, inner.width * 0.12f, inner.height * 0.18f), teal);
                    DrawRect(new Rect(inner.x + inner.width * 0.38f, inner.y + inner.height * 0.14f, inner.width * 0.24f, inner.height * 0.06f), teal);
                    DrawPixelCross(new Rect(inner.x + inner.width * 0.24f, inner.y + inner.height * 0.18f, inner.width * 0.52f, inner.height * 0.48f), frost.WithAlpha(0.62f));
                    break;
                case ObjectType.TavernKeeper:
                    DrawRect(new Rect(inner.x + inner.width * 0.64f, inner.y + inner.height * 0.50f, inner.width * 0.16f, inner.height * 0.16f), Hex("f3ead7"));
                    DrawRect(new Rect(inner.x + inner.width * 0.68f, inner.y + inner.height * 0.58f, inner.width * 0.08f, inner.height * 0.10f), Hex("c98942"));
                    break;
                case ObjectType.GateCaptain:
                    DrawRect(new Rect(inner.x + inner.width * 0.72f, inner.y + inner.height * 0.12f, inner.width * 0.05f, inner.height * 0.68f), stone);
                    DrawRect(new Rect(inner.x + inner.width * 0.64f, inner.y + inner.height * 0.18f, inner.width * 0.20f, inner.height * 0.07f), gold);
                    DrawBorder(new Rect(inner.x + inner.width * 0.28f, inner.y + inner.height * 0.42f, inner.width * 0.44f, inner.height * 0.32f), gold.WithAlpha(0.64f), 1);
                    break;
                case ObjectType.DinerCook:
                    DrawRect(new Rect(inner.x + inner.width * 0.34f, inner.y + inner.height * 0.36f, inner.width * 0.32f, inner.height * 0.07f), Hex("a43f35"));
                    DrawRect(new Rect(inner.x + inner.width * 0.38f, inner.y + inner.height * 0.46f, inner.width * 0.24f, inner.height * 0.25f), Hex("efe4ca"));
                    DrawRect(new Rect(inner.x + inner.width * 0.73f, inner.y + inner.height * 0.34f, inner.width * 0.04f, inner.height * 0.36f), Hex("c9b596"));
                    DrawRect(new Rect(inner.x + inner.width * 0.69f, inner.y + inner.height * 0.30f, inner.width * 0.12f, inner.height * 0.08f), Hex("d9d3c4"));
                    break;
                case ObjectType.Provisioner:
                    DrawRect(new Rect(inner.x + inner.width * 0.34f, inner.y + inner.height * 0.36f, inner.width * 0.32f, inner.height * 0.07f), Hex("d7a84e"));
                    DrawRect(new Rect(inner.x + inner.width * 0.28f, inner.y + inner.height * 0.43f, inner.width * 0.07f, inner.height * 0.34f), Hex("6b4930"));
                    DrawRect(new Rect(inner.x + inner.width * 0.22f, inner.y + inner.height * 0.58f, inner.width * 0.22f, inner.height * 0.20f), Hex("8a5c35"));
                    DrawBorder(new Rect(inner.x + inner.width * 0.67f, inner.y + inner.height * 0.47f, inner.width * 0.16f, inner.height * 0.22f), gold.WithAlpha(0.86f), 1);
                    DrawRect(new Rect(inner.x + inner.width * 0.71f, inner.y + inner.height * 0.51f, inner.width * 0.08f, inner.height * 0.13f), ember.WithAlpha(0.78f));
                    break;
                case ObjectType.DockWorker:
                    DrawRect(new Rect(inner.x + inner.width * 0.35f, inner.y + inner.height * 0.12f, inner.width * 0.30f, inner.height * 0.08f), Hex("456b78"));
                    DrawRect(new Rect(inner.x + inner.width * 0.43f, inner.y + inner.height * 0.07f, inner.width * 0.21f, inner.height * 0.08f), Hex("547f8c"));
                    DrawBorder(new Rect(inner.x + inner.width * 0.65f, inner.y + inner.height * 0.47f, inner.width * 0.22f, inner.height * 0.23f), Hex("b89967"), 2);
                    DrawBorder(new Rect(inner.x + inner.width * 0.69f, inner.y + inner.height * 0.51f, inner.width * 0.14f, inner.height * 0.15f), Hex("b89967"), 1);
                    break;
                case ObjectType.Scholar:
                    DrawBorder(new Rect(inner.x + inner.width * 0.32f, inner.y + inner.height * 0.23f, inner.width * 0.14f, inner.height * 0.09f), frost, 1);
                    DrawBorder(new Rect(inner.x + inner.width * 0.54f, inner.y + inner.height * 0.23f, inner.width * 0.14f, inner.height * 0.09f), frost, 1);
                    DrawRect(new Rect(inner.x + inner.width * 0.46f, inner.y + inner.height * 0.26f, inner.width * 0.08f, inner.height * 0.025f), frost);
                    DrawRect(new Rect(inner.x + inner.width * 0.64f, inner.y + inner.height * 0.48f, inner.width * 0.22f, inner.height * 0.25f), Hex("365f8a"));
                    DrawRect(new Rect(inner.x + inner.width * 0.68f, inner.y + inner.height * 0.52f, inner.width * 0.14f, inner.height * 0.03f), Hex("d9d3c4"));
                    DrawRect(new Rect(inner.x + inner.width * 0.68f, inner.y + inner.height * 0.59f, inner.width * 0.11f, inner.height * 0.025f), Hex("d9d3c4"));
                    break;
            }
        }

        private void DrawRouteScaffoldPlaceholder(Rect inner, ObjectType type)
        {
            Color accent = ObjectColor(type);
            DrawRect(new Rect(inner.x + inner.width * 0.08f, inner.y + inner.height * 0.74f, inner.width * 0.84f, inner.height * 0.08f), Hex("050708", 0.44f));
            switch (type)
            {
                case ObjectType.QuestBoard:
                    DrawRect(new Rect(inner.x + inner.width * 0.20f, inner.y + inner.height * 0.24f, inner.width * 0.60f, inner.height * 0.42f), Hex("5b3a25"));
                    DrawRect(new Rect(inner.x + inner.width * 0.27f, inner.y + inner.height * 0.31f, inner.width * 0.46f, inner.height * 0.08f), gold);
                    DrawRect(new Rect(inner.x + inner.width * 0.27f, inner.y + inner.height * 0.48f, inner.width * 0.34f, inner.height * 0.07f), muted);
                    DrawRect(new Rect(inner.x + inner.width * 0.28f, inner.y + inner.height * 0.66f, inner.width * 0.08f, inner.height * 0.22f), Hex("3a2418"));
                    DrawRect(new Rect(inner.x + inner.width * 0.64f, inner.y + inner.height * 0.66f, inner.width * 0.08f, inner.height * 0.22f), Hex("3a2418"));
                    break;
                case ObjectType.Waystone:
                    DrawRect(new Rect(inner.x + inner.width * 0.36f, inner.y + inner.height * 0.16f, inner.width * 0.28f, inner.height * 0.66f), stone);
                    DrawRect(new Rect(inner.x + inner.width * 0.43f, inner.y + inner.height * 0.25f, inner.width * 0.14f, inner.height * 0.38f), teal.WithAlpha(0.82f));
                    DrawPixelCross(new Rect(inner.x + inner.width * 0.36f, inner.y + inner.height * 0.16f, inner.width * 0.28f, inner.height * 0.66f), teal);
                    break;
                case ObjectType.TrainingGround:
                    DrawBorder(new Rect(inner.x + inner.width * 0.18f, inner.y + inner.height * 0.25f, inner.width * 0.64f, inner.height * 0.42f), accent, 2);
                    DrawRect(new Rect(inner.x + inner.width * 0.32f, inner.y + inner.height * 0.43f, inner.width * 0.36f, inner.height * 0.08f), gold);
                    DrawRect(new Rect(inner.x + inner.width * 0.46f, inner.y + inner.height * 0.25f, inner.width * 0.08f, inner.height * 0.42f), gold);
                    break;
                case ObjectType.LoreLibrary:
                    DrawRect(new Rect(inner.x + inner.width * 0.18f, inner.y + inner.height * 0.26f, inner.width * 0.64f, inner.height * 0.48f), Hex("d9d3c4"));
                    DrawRect(new Rect(inner.x + inner.width * 0.48f, inner.y + inner.height * 0.26f, inner.width * 0.04f, inner.height * 0.48f), frost);
                    DrawRect(new Rect(inner.x + inner.width * 0.26f, inner.y + inner.height * 0.38f, inner.width * 0.18f, inner.height * 0.06f), violet);
                    DrawRect(new Rect(inner.x + inner.width * 0.56f, inner.y + inner.height * 0.50f, inner.width * 0.18f, inner.height * 0.06f), violet);
                    break;
                case ObjectType.ForgeSite:
                    DrawRect(new Rect(inner.x + inner.width * 0.18f, inner.y + inner.height * 0.56f, inner.width * 0.64f, inner.height * 0.18f), stone);
                    DrawRect(new Rect(inner.x + inner.width * 0.30f, inner.y + inner.height * 0.30f, inner.width * 0.40f, inner.height * 0.26f), ember);
                    DrawRect(new Rect(inner.x + inner.width * 0.38f, inner.y + inner.height * 0.18f, inner.width * 0.24f, inner.height * 0.18f), gold);
                    DrawRect(new Rect(inner.x + inner.width * 0.66f, inner.y + inner.height * 0.24f, inner.width * 0.08f, inner.height * 0.42f), Hex("a9b0a2"));
                    break;
                case ObjectType.FactionCamp:
                    DrawRect(new Rect(inner.x + inner.width * 0.18f, inner.y + inner.height * 0.30f, inner.width * 0.28f, inner.height * 0.34f), Hex("53646a"));
                    DrawRect(new Rect(inner.x + inner.width * 0.52f, inner.y + inner.height * 0.24f, inner.width * 0.08f, inner.height * 0.50f), stone);
                    DrawRect(new Rect(inner.x + inner.width * 0.60f, inner.y + inner.height * 0.24f, inner.width * 0.24f, inner.height * 0.20f), accent);
                    break;
                case ObjectType.DungeonGate:
                case ObjectType.DeepCrypt:
                    DrawRect(new Rect(inner.x + inner.width * 0.18f, inner.y + inner.height * 0.24f, inner.width * 0.64f, inner.height * 0.54f), stone);
                    DrawRect(new Rect(inner.x + inner.width * 0.32f, inner.y + inner.height * 0.38f, inner.width * 0.36f, inner.height * 0.40f), Hex("030405"));
                    DrawRect(new Rect(inner.x + inner.width * 0.40f, inner.y + inner.height * 0.25f, inner.width * 0.20f, inner.height * 0.12f), accent);
                    break;
                case ObjectType.AncientGrove:
                    DrawRect(new Rect(inner.x + inner.width * 0.30f, inner.y + inner.height * 0.22f, inner.width * 0.40f, inner.height * 0.34f), moss);
                    DrawRect(new Rect(inner.x + inner.width * 0.42f, inner.y + inner.height * 0.48f, inner.width * 0.16f, inner.height * 0.30f), Hex("5b3a25"));
                    DrawPixelCross(new Rect(inner.x + inner.width * 0.24f, inner.y + inner.height * 0.18f, inner.width * 0.52f, inner.height * 0.42f), poison);
                    break;
                case ObjectType.PortalSeal:
                    DrawBorder(new Rect(inner.x + inner.width * 0.22f, inner.y + inner.height * 0.20f, inner.width * 0.56f, inner.height * 0.56f), blood, 2);
                    DrawRect(new Rect(inner.x + inner.width * 0.44f, inner.y + inner.height * 0.18f, inner.width * 0.12f, inner.height * 0.60f), violet);
                    DrawRect(new Rect(inner.x + inner.width * 0.22f, inner.y + inner.height * 0.44f, inner.width * 0.56f, inner.height * 0.12f), ember);
                    break;
            }
        }

        private bool IsKoboldStoryObject(MapObject obj)
        {
            if (!ContentSetCatalog.AllowKoboldChapter(activeContentSet, state?.StoryFlags)) return false;
            if (obj == null || obj.Type != ObjectType.Cave || state?.Map == null || state.Depth != 2) return false;
            if (HasStoryFlag(StoryFlags.KoboldKingDefeated)) return false;
            return ZoneIdFor(obj.X, obj.Y, state.Map, state.Depth) == "dusk-market";
        }

        private int KoboldRouteMarkerIndex()
        {
            if (HasStoryFlag(StoryFlags.KoboldKingDefeated)) return 7;
            if (HasStoryFlag(StoryFlags.KoboldCaveCleared)) return 2;
            if (HasStoryFlag(StoryFlags.KoboldAmbushSurvived)) return 1;
            return 0;
        }

        private bool TryDrawKoboldRouteArtIcon(Rect rect, int routeIndex, Color tint)
        {
            if (TryDrawKoboldRouteAtlasIcon(rect, routeIndex, tint)) return true;
            int bossIndex = KoboldBossRouteIconIndex(routeIndex);
            return bossIndex >= 0 && TryDrawKoboldBossAtlasIcon(rect, bossIndex, tint);
        }

        private int KoboldBossRouteIconIndex(int routeIndex)
        {
            switch (routeIndex)
            {
                case 0: return 11; // cave drum / ambush warning
                case 1: return 6;  // royal bone charm / smoke cave clue
                case 2: return 4;  // crown-shield rally sigil
                case 3: return 12; // throne room marker
                case 7: return 13; // victory trophy
                default: return 10; // royal standard fallback
            }
        }

        private void DrawKoboldRouteCaveBadge(Rect rect)
        {
            Color accent = HasStoryFlag(StoryFlags.KoboldCaveCleared) ? blood : gold;
            Rect banner = new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.08f, rect.width * 0.64f, rect.height * 0.16f);
            DrawRect(banner, Hex("050708", 0.72f));
            DrawBorder(banner, accent.WithAlpha(0.82f), 1);
            DrawRect(new Rect(banner.x + banner.width * 0.16f, banner.y + banner.height * 0.24f, banner.width * 0.68f, banner.height * 0.22f), accent);
            DrawRect(new Rect(banner.x + banner.width * 0.30f, banner.y + banner.height * 0.58f, banner.width * 0.40f, banner.height * 0.18f), accent.WithAlpha(0.72f));
            DrawRect(new Rect(rect.x + rect.width * 0.42f, rect.y + rect.height * 0.37f, rect.width * 0.16f, rect.height * 0.10f), accent.WithAlpha(0.54f));
        }

        private Color ObjectColor(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Cache: return gold;
                case ObjectType.Shrine: return teal;
                case ObjectType.Encounter: return blood;
                case ObjectType.Stairs: return frost;
                case ObjectType.Camp: return ember;
                case ObjectType.Town: return moss;
                case ObjectType.Obelisk: return teal;
                case ObjectType.Ruin: return stone;
                case ObjectType.Bridge: return Hex("9b6b45");
                case ObjectType.Cave: return Hex("a9b0a2");
                case ObjectType.Market: return gold;
                case ObjectType.MarketClerk: return Hex("d7b15c");
                case ObjectType.CityCourier: return Hex("f0c56c");
                case ObjectType.Temple:
                case ObjectType.Fountain:
                case ObjectType.RecallCircle: return teal;
                case ObjectType.TempleHealer: return Hex("78d8c5");
                case ObjectType.NoviceHealer: return Hex("97dbc2");
                case ObjectType.Diner:
                case ObjectType.Provisions:
                case ObjectType.DinerCook:
                case ObjectType.Provisioner:
                case ObjectType.Tavern: return Hex("d98b6a");
                case ObjectType.TavernKeeper: return Hex("c88954");
                case ObjectType.WoundedTraveler: return blood;
                case ObjectType.Armorer:
                case ObjectType.WeaponVendor:
                case ObjectType.ArmorerNpc:
                case ObjectType.WeaponMerchantNpc:
                case ObjectType.ArmorDisplay:
                case ObjectType.WeaponDisplay: return Hex("a9b0a2");
                case ObjectType.Enchanter:
                case ObjectType.EnchanterNpc:
                case ObjectType.EnchantmentTable: return violet;
                case ObjectType.NorthGate:
                case ObjectType.SouthGate:
                case ObjectType.EastGate:
                case ObjectType.WestGate:
                case ObjectType.TownGuard:
                case ObjectType.GateCaptain:
                case ObjectType.StableHand:
                case ObjectType.DockWorker:
                case ObjectType.OldRoadScout:
                case ObjectType.CityWall: return stone;
                case ObjectType.Scholar: return frost;
                case ObjectType.RoyalHerald: return gold;
                case ObjectType.KingHall:
                case ObjectType.KingHalvard:
                case ObjectType.InteriorDoor:
                case ObjectType.RoyalThrone:
                case ObjectType.RoyalBanner:
                case ObjectType.RoyalLectern:
                case ObjectType.RoyalBrazier:
                case ObjectType.MerchantCounter: return gold;
                case ObjectType.ProvisionShelf: return Hex("d98b6a");
                case ObjectType.Sewer:
                case ObjectType.RatPeltQuest: return poison;
                case ObjectType.QuestBoard: return gold;
                case ObjectType.Waystone: return teal;
                case ObjectType.TrainingGround: return moss;
                case ObjectType.LoreLibrary: return frost;
                case ObjectType.ForgeSite: return stone;
                case ObjectType.FactionCamp: return Hex("d98b6a");
                case ObjectType.DungeonGate: return poison;
                case ObjectType.DeepCrypt: return violet;
                case ObjectType.AncientGrove: return poison;
                case ObjectType.PortalSeal: return blood;
                default: return muted;
            }
        }

        private WorldZone ZoneAt(int x, int y)
        {
            return ZoneFor(x, y, state?.Map, state?.Depth ?? 1);
        }

        private WorldZone ZoneFor(int x, int y, MapData map, int depth)
        {
            return WorldZoneCatalog.For(ZoneIdFor(x, y, map, depth), depth);
        }

        private string ZoneIdFor(int x, int y, MapData map, int depth)
        {
            int mapWidth = map?.Width ?? ExploreW;
            int mapHeight = map?.Height ?? ExploreH;
            int sx = map?.StartX ?? mapWidth / 2;
            int sy = map?.StartY ?? mapHeight / 2;
            string interiorId = MidgaardInteriorIdAt(x, y, map, depth);
            if (!string.IsNullOrEmpty(interiorId)) return interiorId;
            if (IsMidgaardCityCell(x, y, map, depth)) return "midgaard-city";
            bool midgaardApproach = depth == 1
                && map != null
                && Mathf.Abs(y - sy) <= 2
                && x >= MidgaardLeft(map) - 6
                && x <= MidgaardRight(map) + 6;
            if (Distance(x, y, sx, sy) <= 4 || midgaardApproach)
                return "midgaard-road";

            bool north = y < mapHeight * 0.35f;
            bool south = y > mapHeight * 0.65f;
            bool west = x < mapWidth * 0.34f;
            bool east = x > mapWidth * 0.66f;
            if (north && west) return "old-quarry";
            if (north && east) return "glass-warrens";
            if (south && west) return "ash-fen";
            if (south && east) return "red-gate";
            if (north) return "gloam-courts";
            if (south) return "salt-cisterns";
            if (west) return "green-shrine-road";
            if (east) return "dusk-market";
            return "inner-ash-road";
        }

        private IEnumerable<RouteScaffoldDef> RouteScaffoldDefs()
        {
            yield return new RouteScaffoldDef("midgaard-city", ObjectType.QuestBoard, 1, 0, "Midgaard Quest Board", "city errands", "Posts Mira's Lamp Round, Brann's Gate Survey, and future route contracts.", "scroll", gold);
            yield return new RouteScaffoldDef("midgaard-road", ObjectType.Waystone, 1, 1, "Old Road Waystone", "recall anchor", "Future fast-travel and camp-routing node. Currently marks the road and grants a small recovery.", "magic", teal);
            yield return new RouteScaffoldDef("green-shrine-road", ObjectType.TrainingGround, 1, 2, "Green Shrine Training Ring", "skill trainer", "Future priest/warrior tutorial space for Tree Cover, guard work, and low-risk ability practice.", "party", moss);
            yield return new RouteScaffoldDef("old-quarry", ObjectType.ForgeSite, 1, 3, "Old Quarry Forge", "gear workbench", "Future crafting/repair station for heavy armor, shields, reach weapons, and bridge work.", "settings", stone);
            yield return new RouteScaffoldDef("glass-warrens", ObjectType.LoreLibrary, 2, 4, "Glass Lore Library", "spell lesson", "Future formula-study node for wizard paths, mirror puzzles, and caster faction lore.", "magic", frost);
            yield return new RouteScaffoldDef("dusk-market", ObjectType.FactionCamp, 2, 5, "Dusk Market Hideout", "faction contact", "Future rogue/ranger contact hub for scouts, fences, ambush rumors, and smoke-route jobs.", "party", gold);
            yield return new RouteScaffoldDef("salt-cisterns", ObjectType.DungeonGate, 2, 6, "Cistern Gate", "dungeon gate", "Future authored dungeon entrance with room chains, locked doors, keys, and reward tables.", "blocked", poison);
            yield return new RouteScaffoldDef("ash-fen", ObjectType.AncientGrove, 2, 7, "Ash Fen Grove", "hazard grove", "Future nature-hazard route for poison, mire, generated trees, and monster path pressure.", "magic", poison);
            yield return new RouteScaffoldDef("gloam-courts", ObjectType.DeepCrypt, 3, 8, "Gloam Crypt", "crypt route", "Future undead/caster route with ritual rooms, bone priests, and death-resistance rewards.", "enemy", violet);
            yield return new RouteScaffoldDef("red-gate", ObjectType.PortalSeal, 4, 9, "Red Gate Seal", "chapter lock", "Future gate-key and late-campaign branching node before demon and drow pressure peaks.", "danger", blood);
        }

        private RouteScaffoldDef RouteScaffoldFor(MapObject obj)
        {
            if (obj == null) return null;
            string zoneId = ZoneIdFor(obj.X, obj.Y, state?.Map, state?.Depth ?? 1);
            return RouteScaffoldDefs().FirstOrDefault(d => d.Type == obj.Type && string.Equals(d.ZoneId, zoneId, StringComparison.OrdinalIgnoreCase))
                ?? RouteScaffoldDefs().FirstOrDefault(d => d.Type == obj.Type);
        }

        private bool IsRouteScaffoldObject(ObjectType type)
        {
            return RouteScaffoldDefs().Any(d => d.Type == type);
        }

        private string RouteScaffoldFlag(MapObject obj)
        {
            if (obj == null) return "";
            string zoneId = ZoneIdFor(obj.X, obj.Y, state?.Map, state?.Depth ?? 1);
            int depth = state == null ? 1 : state.Depth;
            return $"route_scaffold_{Mathf.Max(1, depth)}_{SanitizeFlagPart(zoneId)}_{SanitizeFlagPart(obj.Type.ToString())}_visited";
        }

        private string SanitizeFlagPart(string text)
        {
            if (string.IsNullOrEmpty(text)) return "none";
            char[] chars = text.ToLowerInvariant().Select(c => char.IsLetterOrDigit(c) ? c : '_').ToArray();
            return new string(chars).Trim('_');
        }

        private string ZoneDangerText(WorldZone zone)
        {
            if (zone == null) return "unknown";
            if (zone.Danger <= 0) return "safe";
            if (zone.Danger == 1) return "low danger";
            if (zone.Danger == 2) return "danger";
            if (zone.Danger == 3) return "high danger";
            return "deadly";
        }

        private string TravelDangerLabel(WorldZone zone)
        {
            if (zone == null) return "Unknown Road";
            if (zone.Danger <= 0) return "Safe Road";
            if (zone.Danger == 1) return "Low Watch";
            if (zone.Danger == 2) return "Danger";
            if (zone.Danger == 3) return "High Risk";
            return "Deadly";
        }

        private string StoryObjectiveForDepth(int depth)
        {
            if (depth <= 1) return "Chapter I: The Midgaard Cisterns. Meet King Halvard, accept the sewer contract, and clear three chambers beneath Midgaard.";
            if (depth == 2) return "Chapter II: Kobold Smoke. Survive the Dusk Market ambush, clear the cave mouth, then break the Kobold King's shield hall.";
            if (depth == 3) return "Chapter III: The Bone Road. Break caster pressure in the Gloam Courts and recover the first Red Gate warning.";
            if (depth == 4) return "Chapter IV: Glass and Ash. Cross the Glass Warrens, survive warlock bargains, and search for a gate key.";
            if (depth == 5) return "Chapter V: The Red Gate. Push through drow priests, bone wizards, and lesser demons to find the sealed descent.";
            if (depth >= FinalBossDepth) return "Chapter VI: Meteor Crown. Break the ritual heart and defeat Vhal Rakh before the Old Road burns open.";
            return "Chapter V: Below the Old Road. The scaffold now points toward bosses, factions, and deeper world-state choices.";
        }

        private string StoryChapterTitle()
        {
            string objective = string.IsNullOrEmpty(state?.ActiveStory) ? StoryObjectiveForDepth(state?.Depth ?? 1) : state.ActiveStory;
            int dot = objective.IndexOf('.');
            return dot > 0 ? objective.Substring(0, dot) : objective;
        }

        private string ExploreRegionName(int x, int y)
        {
            if (TryRegionalSiteAt(state?.Map, x, y, out WorldMapSite site)) return site.Name;
            return ZoneAt(x, y).Name;
        }

        private string ExploreUnderfootLine(int x, int y)
        {
            RoamingThreat threat = RoamingThreatAt(x, y);
            if (threat != null) return $"{threat.Name} / moving enemy / {(threat.Alerted ? "pursuing" : "watching the road")}";
            MapObject obj = ObjectAt(state.Map, x, y);
            if (obj != null) return ObjectName(obj) + ": " + ObjectHint(obj);
            WorldZone zone = ZoneAt(x, y);
            string risk = zone != null && zone.Danger <= 0 ? "safe" : "patrol risk";
            if (TryRegionalSiteAt(state?.Map, x, y, out WorldMapSite site))
            {
                return $"{site.Name} / {ExploreGroundName(x, y)} / {risk}";
            }
            if (TryRegionalJunctionAt(x, y, 0, out WorldMapJunction junction)) return $"{junction.Name} / route junction / {risk}";
            return TileAt(state.Map, x, y) == 1 ? $"{ExploreGroundName(x, y)} / {risk}" : $"{ExploreGroundName(x, y)} / blocks movement";
        }

        private string ExploreLookLine(int x, int y)
        {
            string region = ExploreRegionName(x, y);
            WorldZone zone = ZoneAt(x, y);
            RoamingThreat threat = RoamingThreatAt(x, y);
            if (threat != null)
            {
                int threatDistance = Distance(x, y, state.PlayerX, state.PlayerY);
                string intent = threat.Alerted ? "pursuing the party" : "prowling";
                string action = threatDistance == 1 ? "click or move toward it to engage" : $"range {threatDistance}";
                return $"{threat.Name} / {region} / {intent}\nMoving enemy patrol / {action}";
            }
            MapObject obj = ObjectAt(state.Map, x, y);
            int distance = Distance(x, y, state.PlayerX, state.PlayerY);
            if (obj != null)
            {
                string step = distance == 0
                    ? "underfoot"
                    : distance == 1 && ShouldResolveExploreObjectFromAdjacent(obj)
                        ? "click or Space/E to use"
                        : distance == 1 && CanStepExplore(x, y)
                            ? "click to step onto it"
                            : $"range {distance}";
                string objective = IsCurrentMidgaardObjective(obj) ? " / current work" : "";
                return $"{ObjectName(obj)} / {region} / {ZoneDangerText(zone)}{objective}\n{ObjectHint(obj)} / {step}";
            }

            if (TryRegionalJunctionAt(x, y, 0, out WorldMapJunction junction))
            {
                bool charted = state.DiscoveredZones != null
                    && state.DiscoveredZones.Contains(RegionalJunctionKey(state.Depth, junction.Id));
                string status = charted && RouteChartRules.IsWaypoint(state.ActiveRouteWaypointKey, state.Depth, junction.Id)
                    ? "marked waypoint"
                    : charted ? "charted" : "uncharted";
                return $"{junction.Name} / {region} / {status}\n{junction.Summary}";
            }

            if (TileAt(state.Map, x, y) == 0)
            {
                return $"{ExploreGroundName(x, y)} / {region}\nblocks movement / {zone.Title}";
            }

            string move = distance == 0 ? "current position" : distance == 1 ? "click to move" : $"range {distance}";
            return $"{ExploreGroundName(x, y)} / {region} / {ZoneDangerText(zone)}\n{move} / {zone.Summary}";
        }

        private string ExploreLegendLine()
        {
            return state != null && state.Depth == 1
                ? "gold objective or scaffold / teal safe / red danger / gray gates and walls"
                : "gold cache/scaffold / teal shrine / red danger / gray landmarks";
        }

        private Color ZoneDangerColor(WorldZone zone)
        {
            if (zone == null || zone.Danger <= 0) return teal;
            if (zone.Danger == 1) return moss;
            if (zone.Danger == 2) return gold;
            if (zone.Danger == 3) return ember;
            return blood;
        }

        private string ExploreGroundName(int x, int y)
        {
            int tile = TileAt(state.Map, x, y);
            string kind = ExploreTileKind(x, y, tile);
            switch (kind)
            {
                case "road": return "Old road";
                case "paved": return "Broken paving";
                case "moss": return "Mossy path";
                case "mire": return "Shallow mire";
                case "mud": return "Fen bank";
                case "quarry": return "Quarry stone";
                case "glass": return "Glass rubble";
                case "ash": return "Ash floor";
                case "midgaard-market": return "Market cobbles";
                case "midgaard-temple": return "Temple square";
                case "midgaard-fountain": return "Fountain stones";
                case "midgaard-diner": return "Diner lane";
                case "midgaard-tavern": return "Tavern lane";
                case "midgaard-armorer": return "Armorer row";
                case "midgaard-weapons": return "Weapons row";
                case "midgaard-enchanter": return "Rune paving";
                case "midgaard-gate": return "Gate threshold";
                case "midgaardwall": return "City wall";
                case "midgaard-guard": return "Guard post";
                case "midgaard-king": return "Keep courtyard";
                case "midgaard-sewer": return "Sewer stones";
                case "midgaard-provisions": return "Provision crates";
                case "midgaard-ratquest": return "Pelt marker";
                case "midgaard-recall": return "Recall circle";
                case "midgaard-road": return "City street";
                case "midgaard-plaza": return "Plaza cobbles";
                case "midgaard-paved": return "Town paving";
                case "forestwall": return "Tree wall";
                case "mirewall": return "Dark water";
                case "cliffwall": return "Cliff stone";
                case "redwall": return "Red basalt";
                default: return tile == 1 ? "Ruin floor" : "Stone wall";
            }
        }

        private string ObjectName(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Cache: return "Sealed cache";
                case ObjectType.Shrine: return "Old shrine";
                case ObjectType.Encounter: return "Enemy sign";
                case ObjectType.Stairs: return "Down stairs";
                case ObjectType.Camp: return "Camp mark";
                case ObjectType.Town: return HomeTownName;
                case ObjectType.Obelisk: return "Runed obelisk";
                case ObjectType.Ruin: return "Fallen ruin";
                case ObjectType.Bridge: return "Old bridge";
                case ObjectType.Cave: return "Cave mouth";
                case ObjectType.Market: return "Market Square";
                case ObjectType.MarketClerk: return "Market Clerk Nessa";
                case ObjectType.CityCourier: return "City Courier";
                case ObjectType.Temple: return "Temple Square";
                case ObjectType.TempleHealer: return "Mira of Midgaard";
                case ObjectType.NoviceHealer: return "Novice Healer Sera";
                case ObjectType.Fountain: return "Temple fountain";
                case ObjectType.Diner: return "Kate's Diner";
                case ObjectType.DinerCook: return "Kate";
                case ObjectType.Tavern: return "Midgaard tavern";
                case ObjectType.TavernKeeper: return "Tavern Keeper Orren";
                case ObjectType.WoundedTraveler: return "Wounded Traveler";
                case ObjectType.StableHand: return "Stable Hand Pell";
                case ObjectType.Armorer: return "Basic armorer";
                case ObjectType.WeaponVendor: return "Weapons vendor";
                case ObjectType.Enchanter: return "Weapon enchanter";
                case ObjectType.NorthGate: return "North Gate";
                case ObjectType.SouthGate: return "South Gate";
                case ObjectType.InteriorDoor: return "Interior door";
                case ObjectType.KingHalvard: return "King Halvard";
                case ObjectType.ArmorerNpc: return "Armorer Borin";
                case ObjectType.WeaponMerchantNpc: return "Weaponsmith Tessa";
                case ObjectType.EnchanterNpc: return "Runesmith Maud";
                case ObjectType.RoyalThrone: return "Throne of Midgaard";
                case ObjectType.RoyalBanner: return "Midgaard banner";
                case ObjectType.RoyalLectern: return "Royal lectern";
                case ObjectType.RoyalBrazier: return "Hall brazier";
                case ObjectType.ArmorDisplay: return "Armor display";
                case ObjectType.WeaponDisplay: return "Weapon rack";
                case ObjectType.EnchantmentTable: return "Rune table";
                case ObjectType.ProvisionShelf: return "Provision shelves";
                case ObjectType.MerchantCounter: return "Merchant counter";
                case ObjectType.EastGate: return "East Gate";
                case ObjectType.WestGate: return "West Gate";
                case ObjectType.TownGuard: return "Town guard";
                case ObjectType.GateCaptain: return "Gate Captain Brann";
                case ObjectType.KingHall: return "King's Hall";
                case ObjectType.RoyalHerald: return "Royal Herald";
                case ObjectType.OldRoadScout: return "Old Road Scout Yara";
                case ObjectType.Sewer: return "Sewer grate";
                case ObjectType.CityWall: return "City wall";
                case ObjectType.Provisions: return "Provision stall";
                case ObjectType.Provisioner: return "Provisioner Lute";
                case ObjectType.DockWorker: return "Dock Worker";
                case ObjectType.Scholar: return "Midgaard Scholar";
                case ObjectType.RatPeltQuest: return "Rat-pelt workbench";
                case ObjectType.RecallCircle: return "Recall circle";
                case ObjectType.QuestBoard: return "Quest board";
                case ObjectType.Waystone: return "Old Road waystone";
                case ObjectType.TrainingGround: return "Training ring";
                case ObjectType.LoreLibrary: return "Lore library";
                case ObjectType.ForgeSite: return "Forge site";
                case ObjectType.FactionCamp: return "Faction camp";
                case ObjectType.DungeonGate: return "Dungeon gate";
                case ObjectType.DeepCrypt: return "Deep crypt";
                case ObjectType.AncientGrove: return "Ancient grove";
                case ObjectType.PortalSeal: return "Portal seal";
                default: return "Road mark";
            }
        }

        private string ObjectHint(MapObject obj)
        {
            if (TryRegionalSite(state?.Map, obj, out WorldMapSite regionalSite))
            {
                return regionalSite.Summary;
            }
            string interiorHint = InteriorObjectHint(obj);
            if (!string.IsNullOrEmpty(interiorHint)) return interiorHint;
            if (obj != null && obj.Type == ObjectType.Cave && IsKoboldStoryCave(obj))
            {
                if (!HasStoryFlag(StoryFlags.KoboldCaveCleared)) return "kobold smoke and drum taps";
                if (!HasStoryFlag(StoryFlags.KoboldKingDefeated)) return "path to the Kobold King's hall";
                return "quiet cave, battle spent";
            }
            return ObjectHint(obj == null ? ObjectType.Ruin : obj.Type);
        }

        private string ObjectName(MapObject obj)
        {
            if (obj == null) return "";
            if (string.Equals(obj.Id, OldRoadDescentId, StringComparison.Ordinal))
            {
                return "Sluice Steps";
            }
            if (TryRegionalSite(state?.Map, obj, out WorldMapSite regionalSite))
            {
                return regionalSite.Name;
            }
            string interiorName = InteriorObjectName(obj);
            if (!string.IsNullOrEmpty(interiorName)) return interiorName;
            if (obj.Type == ObjectType.TownGuard && state?.Map != null)
            {
                return obj.X > state.Map.StartX ? "Watchwoman Ilyra" : "Watchman Rusk";
            }
            return ObjectName(obj.Type);
        }

        private string ObjectHint(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Cache: return "sealed loot and gear";
                case ObjectType.Shrine: return "teal recovery light";
                case ObjectType.Encounter: return "enemy tracks and bones";
                case ObjectType.Stairs: return "descend when standing here";
                case ObjectType.Camp: return "tent, coals, and rest";
                case ObjectType.Town: return $"{HomeTownName}'s healing lamps";
                case ObjectType.Obelisk: return "landmark and old ward";
                case ObjectType.Ruin: return "broken cover and memory";
                case ObjectType.Bridge: return "crossing marker";
                case ObjectType.Cave: return "cold side passage";
                case ObjectType.Market: return "central plaza and road hub";
                case ObjectType.MarketClerk: return HasStoryFlag(StoryFlags.MidgaardLampRoundStarted) && !HasStoryFlag(StoryFlags.MidgaardLampRoundMarket) ? "lamp round stop and market ledger" : "market ledger and errand clues";
                case ObjectType.CityCourier: return "safe-lane hints and courier reward";
                case ObjectType.Temple: return "safe temple air and full recovery";
                case ObjectType.TempleHealer: return HasStoryFlag(StoryFlags.MidgaardLampRoundComplete) ? "healer and completed lamp route" : "starts or completes Mira's lamp round";
                case ObjectType.NoviceHealer: return "small healing and priest spell hint";
                case ObjectType.Fountain: return "peaceful water outside the temple";
                case ObjectType.Diner: return "safe food, provisions, and warm lamps";
                case ObjectType.DinerCook: return "warm meals, provisions, and road advice";
                case ObjectType.Tavern: return "rest, rumors, and party regrouping";
                case ObjectType.TavernKeeper: return HasStoryFlag(StoryFlags.MidgaardLampRoundStarted) && !HasStoryFlag(StoryFlags.MidgaardLampRoundTavern) ? "lamp round stop and road rumors" : "rumors, rest, and road bread";
                case ObjectType.WoundedTraveler: return "road warning and emergency elixir";
                case ObjectType.StableHand: return "east-road warning and supplies";
                case ObjectType.Armorer: return "armor service and rat-pelt reward";
                case ObjectType.WeaponVendor: return "early weapons for sale";
                case ObjectType.Enchanter: return "paid weapon enchantment";
                case ObjectType.NorthGate: return "sealed north gatehouse";
                case ObjectType.SouthGate: return "sealed south gatehouse";
                case ObjectType.InteriorDoor: return "leave this interior";
                case ObjectType.KingHalvard: return "receive or review the royal writ";
                case ObjectType.ArmorerNpc: return "armor service and rat-pelt reward";
                case ObjectType.WeaponMerchantNpc: return "early weapons for sale";
                case ObjectType.EnchanterNpc: return "paid weapon enchantment";
                case ObjectType.RoyalThrone: return "the city's old seat";
                case ObjectType.RoyalBanner: return "blue and gold of Midgaard";
                case ObjectType.RoyalLectern: return "sealed writs and road charters";
                case ObjectType.RoyalBrazier: return "warm hall fire";
                case ObjectType.ArmorDisplay: return "samples of Borin's work";
                case ObjectType.WeaponDisplay: return "balanced steel and ashwood";
                case ObjectType.EnchantmentTable: return "quiet rune-light";
                case ObjectType.ProvisionShelf: return "merchant stock";
                case ObjectType.MerchantCounter: return "trade counter";
                case ObjectType.EastGate: return "east road out of Midgaard";
                case ObjectType.WestGate: return "west road out of Midgaard";
                case ObjectType.TownGuard: return "gate watch and quest warnings";
                case ObjectType.GateCaptain: return HasStoryFlag(StoryFlags.MidgaardGateSurveyComplete) ? "survey paid and gates noted" : "starts or completes the gate survey";
                case ObjectType.KingHall: return "the king assigns road work";
                case ObjectType.RoyalHerald: return "royal notices and sewer writ";
                case ObjectType.OldRoadScout: return "west-road hints and zone notes";
                case ObjectType.Sewer: return "rats below the city";
                case ObjectType.CityWall: return "no open gate here";
                case ObjectType.Provisions: return "buy travel food";
                case ObjectType.Provisioner: return "buy weighed and sealed travel food";
                case ObjectType.DockWorker: return "south-quarter hauling and cistern warnings";
                case ObjectType.Scholar: return "city history and spellcraft notes";
                case ObjectType.RatPeltQuest: return ContentSetCatalog.IsFullPrototype(activeContentSet) ? "turn in four rat pelts" : "turn in three sewer proof bundles";
                case ObjectType.RecallCircle: return "recall returns here";
                case ObjectType.QuestBoard: return "future contracts and route work";
                case ObjectType.Waystone: return "future recall and road anchor";
                case ObjectType.TrainingGround: return "future class training";
                case ObjectType.LoreLibrary: return "future spell lessons";
                case ObjectType.ForgeSite: return "future crafting station";
                case ObjectType.FactionCamp: return "future faction contact";
                case ObjectType.DungeonGate: return "future authored dungeon";
                case ObjectType.DeepCrypt: return "future crypt route";
                case ObjectType.AncientGrove: return "future hazard grove";
                case ObjectType.PortalSeal: return "future gate-key lock";
                default: return "something waits";
            }
        }
    }
}
