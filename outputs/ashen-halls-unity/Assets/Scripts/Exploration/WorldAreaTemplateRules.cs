using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AshenHalls
{
    public readonly struct WorldAreaCellTemplate
    {
        public readonly int OffsetX;
        public readonly int OffsetY;
        public readonly bool Open;
        public readonly ExplorationMaterial Material;
        public readonly ExplorationCellRole Roles;

        public WorldAreaCellTemplate(
            int offsetX,
            int offsetY,
            bool open,
            ExplorationMaterial material,
            ExplorationCellRole roles)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;
            Open = open;
            Material = material;
            Roles = roles;
        }
    }

    public readonly struct WorldAreaObjectTemplate
    {
        public readonly string Key;
        public readonly int OffsetX;
        public readonly int OffsetY;
        public readonly ObjectType Type;

        public WorldAreaObjectTemplate(string key, int offsetX, int offsetY, ObjectType type)
        {
            Key = key ?? "";
            OffsetX = offsetX;
            OffsetY = offsetY;
            Type = type;
        }
    }

    public sealed class WorldAreaTemplate
    {
        private readonly Dictionary<long, WorldAreaCellTemplate> cellsByOffset;

        public string SiteId { get; }
        public string ZoneId { get; }
        public int Radius { get; }
        public int ApproachOffsetX { get; }
        public int ApproachOffsetY { get; }
        public IReadOnlyList<WorldAreaCellTemplate> Cells { get; }
        public IReadOnlyList<WorldAreaObjectTemplate> Objects { get; }
        public string Signature { get; }

        internal WorldAreaTemplate(
            string siteId,
            string zoneId,
            int radius,
            int approachOffsetX,
            int approachOffsetY,
            WorldAreaCellTemplate[] cells,
            WorldAreaObjectTemplate[] objects)
        {
            SiteId = siteId ?? "";
            ZoneId = zoneId ?? "";
            Radius = Math.Max(0, radius);
            ApproachOffsetX = approachOffsetX;
            ApproachOffsetY = approachOffsetY;
            WorldAreaCellTemplate[] authoredCells = cells ?? Array.Empty<WorldAreaCellTemplate>();
            WorldAreaObjectTemplate[] authoredObjects = objects ?? Array.Empty<WorldAreaObjectTemplate>();
            Cells = new ReadOnlyCollection<WorldAreaCellTemplate>(authoredCells);
            Objects = new ReadOnlyCollection<WorldAreaObjectTemplate>(authoredObjects);
            cellsByOffset = new Dictionary<long, WorldAreaCellTemplate>(authoredCells.Length);
            foreach (WorldAreaCellTemplate cell in authoredCells)
            {
                cellsByOffset[OffsetKey(cell.OffsetX, cell.OffsetY)] = cell;
            }
            Signature = BuildSignature(authoredCells, authoredObjects);
        }

        public bool TryCell(int offsetX, int offsetY, out WorldAreaCellTemplate cell)
        {
            return cellsByOffset.TryGetValue(OffsetKey(offsetX, offsetY), out cell);
        }

        private static string BuildSignature(
            IReadOnlyList<WorldAreaCellTemplate> cells,
            IReadOnlyList<WorldAreaObjectTemplate> objects)
        {
            StringBuilder signature = new StringBuilder(cells.Count * 12 + objects.Count * 12);
            foreach (WorldAreaCellTemplate cell in cells)
            {
                signature.Append(cell.OffsetX).Append(',')
                    .Append(cell.OffsetY).Append(':')
                    .Append(cell.Open ? '1' : '0').Append(':')
                    .Append((int)cell.Material).Append(':')
                    .Append((int)cell.Roles).Append(';');
            }
            signature.Append('|');
            foreach (WorldAreaObjectTemplate decoration in objects)
            {
                signature.Append(decoration.OffsetX).Append(',')
                    .Append(decoration.OffsetY).Append(':')
                    .Append((int)decoration.Type).Append(';');
            }
            return signature.ToString();
        }

        private static long OffsetKey(int x, int y)
        {
            return ((long)x << 32) ^ (uint)y;
        }
    }

    public static class WorldAreaTemplateRules
    {
        private static readonly WorldAreaTemplate[] AuthoredTemplates =
        {
            Create(
                "green-shrine-training-ring",
                "green-shrine-road",
                3,
                new[]
                {
                    "##...#r",
                    "#.....r",
                    "..ppp..",
                    "..pcp.a",
                    "..ppp..",
                    "#..r..#",
                    "##...##"
                },
                ExplorationMaterial.NaturalGround,
                ExplorationMaterial.Moss,
                ExplorationMaterial.PackedDirt,
                ExplorationMaterial.Forest,
                new[]
                {
                    new WorldAreaObjectTemplate("west-marker", -2, 0, ObjectType.Obelisk),
                    new WorldAreaObjectTemplate("east-marker", 2, -1, ObjectType.Obelisk),
                    new WorldAreaObjectTemplate("practice-frame", 0, -2, ObjectType.Ruin)
                }),
            Create(
                "old-quarry-forge",
                "old-quarry",
                2,
                new[]
                {
                    "##a##",
                    "##r.#",
                    "#.cp#",
                    "#...#",
                    "#####"
                },
                ExplorationMaterial.QuarryStone,
                ExplorationMaterial.RuinedPaving,
                ExplorationMaterial.PackedDirt,
                ExplorationMaterial.Cliff,
                new[]
                {
                    new WorldAreaObjectTemplate("broken-yard", -1, 0, ObjectType.Ruin),
                    new WorldAreaObjectTemplate("forge-stack", 1, -1, ObjectType.Obelisk),
                    new WorldAreaObjectTemplate("loading-frame", 1, 1, ObjectType.Bridge)
                }),
            Create(
                "gloam-deep-crypt",
                "gloam-courts",
                2,
                new[]
                {
                    "##a##",
                    "#.r.#",
                    ".pcp.",
                    "#...#",
                    "##.##"
                },
                ExplorationMaterial.GloamStone,
                ExplorationMaterial.TempleStone,
                ExplorationMaterial.GloamStone,
                ExplorationMaterial.Forest,
                new[]
                {
                    new WorldAreaObjectTemplate("west-pillar", -1, -1, ObjectType.Obelisk),
                    new WorldAreaObjectTemplate("east-pillar", 1, -1, ObjectType.Obelisk),
                    new WorldAreaObjectTemplate("fallen-masonry", -2, 0, ObjectType.Ruin)
                }),
            Create(
                "glass-lore-library",
                "glass-warrens",
                2,
                new[]
                {
                    "#.a.#",
                    "..r..",
                    "#.c.#",
                    ".....",
                    "#.#.#"
                },
                ExplorationMaterial.GlassRubble,
                ExplorationMaterial.GloamStone,
                ExplorationMaterial.RuinedPaving,
                ExplorationMaterial.Cliff,
                new[]
                {
                    new WorldAreaObjectTemplate("shattered-stack", -1, -2, ObjectType.Ruin),
                    new WorldAreaObjectTemplate("west-lens", -2, 1, ObjectType.Obelisk),
                    new WorldAreaObjectTemplate("east-lens", 2, 1, ObjectType.Obelisk)
                }),
            Create(
                "dusk-market-hideout",
                "dusk-market",
                3,
                new[]
                {
                    "##...##",
                    "#..#..#",
                    "...#...",
                    "arrcrr.",
                    "...r...",
                    "r..r..#",
                    "r#...##"
                },
                ExplorationMaterial.RuinedPaving,
                ExplorationMaterial.MarketCobbles,
                ExplorationMaterial.PackedDirt,
                ExplorationMaterial.RuinedWall,
                new[]
                {
                    new WorldAreaObjectTemplate("west-stall", -1, -2, ObjectType.Ruin),
                    new WorldAreaObjectTemplate("dead-lantern", 1, -1, ObjectType.Obelisk),
                    new WorldAreaObjectTemplate("alley-barricade", 2, 1, ObjectType.Ruin),
                    new WorldAreaObjectTemplate("market-frame", -2, 2, ObjectType.Bridge)
                }),
            Create(
                "red-gate-seal",
                "red-gate",
                2,
                new[]
                {
                    "rrarr",
                    "##r.r",
                    "#.c.#",
                    "#.b.#",
                    "##.##"
                },
                ExplorationMaterial.RedAsh,
                ExplorationMaterial.RuinedPaving,
                ExplorationMaterial.RedAsh,
                ExplorationMaterial.RedBasalt,
                new[]
                {
                    new WorldAreaObjectTemplate("west-seal", -1, 0, ObjectType.Obelisk),
                    new WorldAreaObjectTemplate("east-seal", 1, 0, ObjectType.Obelisk),
                    new WorldAreaObjectTemplate("broken-causeway", 1, 1, ObjectType.Ruin)
                }),
            Create(
                "salt-cistern-gate",
                "salt-cisterns",
                2,
                new[]
                {
                    "bbabb",
                    "~.b.~",
                    "~.c.~",
                    "~...~",
                    "~~~~~"
                },
                ExplorationMaterial.CisternBrick,
                ExplorationMaterial.SewerBrick,
                ExplorationMaterial.BridgeDeck,
                ExplorationMaterial.DeepWater,
                new[]
                {
                    new WorldAreaObjectTemplate("west-sluice", -1, -1, ObjectType.Ruin),
                    new WorldAreaObjectTemplate("east-sluice", 1, -1, ObjectType.Obelisk),
                    new WorldAreaObjectTemplate("canal-span", 0, 1, ObjectType.Bridge)
                }),
            Create(
                "ash-fen-ancient-grove",
                "ash-fen",
                3,
                new[]
                {
                    "~rra~~~",
                    "rrrbrrr",
                    "~.....~",
                    "~..c..~",
                    "~.....~",
                    "~~...~~",
                    "~~~~~~~"
                },
                ExplorationMaterial.FenMud,
                ExplorationMaterial.Moss,
                ExplorationMaterial.BridgeDeck,
                ExplorationMaterial.DeepWater,
                new[]
                {
                    new WorldAreaObjectTemplate("west-stone", -2, 0, ObjectType.Obelisk),
                    new WorldAreaObjectTemplate("east-root", 2, 0, ObjectType.Ruin),
                    new WorldAreaObjectTemplate("grove-span", 0, 2, ObjectType.Bridge)
                })
        };

        private static readonly ReadOnlyCollection<WorldAreaTemplate> ReadOnlyTemplates =
            new ReadOnlyCollection<WorldAreaTemplate>(AuthoredTemplates);
        private static readonly Dictionary<string, WorldAreaTemplate> TemplatesBySiteAndZone = BuildLookup();

        public static IReadOnlyList<WorldAreaTemplate> All => ReadOnlyTemplates;

        public static bool TryGet(string siteId, string zoneId, out WorldAreaTemplate template)
        {
            return TemplatesBySiteAndZone.TryGetValue(LookupKey(siteId, zoneId), out template);
        }

        private static Dictionary<string, WorldAreaTemplate> BuildLookup()
        {
            Dictionary<string, WorldAreaTemplate> lookup =
                new Dictionary<string, WorldAreaTemplate>(StringComparer.Ordinal);
            foreach (WorldAreaTemplate template in AuthoredTemplates)
            {
                lookup.Add(LookupKey(template.SiteId, template.ZoneId), template);
            }
            return lookup;
        }

        private static WorldAreaTemplate Create(
            string siteId,
            string zoneId,
            int radius,
            IReadOnlyList<string> rows,
            ExplorationMaterial groundMaterial,
            ExplorationMaterial featureMaterial,
            ExplorationMaterial pathMaterial,
            ExplorationMaterial blockedMaterial,
            WorldAreaObjectTemplate[] objects)
        {
            int width = radius * 2 + 1;
            if (rows == null || rows.Count != width)
            {
                throw new InvalidOperationException(siteId + " must define a square regional-site template.");
            }

            List<WorldAreaCellTemplate> cells = new List<WorldAreaCellTemplate>(width * width);
            int approachOffsetX = 0;
            int approachOffsetY = 0;
            int approachCount = 0;
            for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                string row = rows[rowIndex] ?? "";
                if (row.Length != width)
                {
                    throw new InvalidOperationException(siteId + " has an invalid regional-site row width.");
                }

                for (int columnIndex = 0; columnIndex < row.Length; columnIndex++)
                {
                    int offsetX = columnIndex - radius;
                    int offsetY = rowIndex - radius;
                    if (row[columnIndex] == 'a')
                    {
                        approachOffsetX = offsetX;
                        approachOffsetY = offsetY;
                        approachCount++;
                    }
                    cells.Add(CellFor(
                        row[columnIndex],
                        offsetX,
                        offsetY,
                        groundMaterial,
                        featureMaterial,
                        pathMaterial,
                        blockedMaterial));
                }
            }

            bool cardinalBoundaryApproach =
                Math.Abs(approachOffsetX) == radius && approachOffsetY == 0
                || Math.Abs(approachOffsetY) == radius && approachOffsetX == 0;
            if (approachCount != 1 || !cardinalBoundaryApproach)
            {
                throw new InvalidOperationException(siteId + " must define one cardinal boundary approach.");
            }

            return new WorldAreaTemplate(
                siteId,
                zoneId,
                radius,
                approachOffsetX,
                approachOffsetY,
                cells.ToArray(),
                objects);
        }

        private static WorldAreaCellTemplate CellFor(
            char marker,
            int offsetX,
            int offsetY,
            ExplorationMaterial groundMaterial,
            ExplorationMaterial featureMaterial,
            ExplorationMaterial pathMaterial,
            ExplorationMaterial blockedMaterial)
        {
            switch (marker)
            {
                case '#':
                    return new WorldAreaCellTemplate(offsetX, offsetY, false, blockedMaterial, ExplorationCellRole.None);
                case '~':
                    return new WorldAreaCellTemplate(
                        offsetX,
                        offsetY,
                        false,
                        blockedMaterial,
                        ExplorationCellRole.Water | ExplorationCellRole.Hazard);
                case '.':
                    return new WorldAreaCellTemplate(
                        offsetX,
                        offsetY,
                        true,
                        groundMaterial,
                        ExplorationCellRole.Room | ExplorationCellRole.Clearing);
                case 'p':
                    return new WorldAreaCellTemplate(
                        offsetX,
                        offsetY,
                        true,
                        featureMaterial,
                        ExplorationCellRole.Room | ExplorationCellRole.Plaza | ExplorationCellRole.Clearing);
                case 'r':
                    return new WorldAreaCellTemplate(
                        offsetX,
                        offsetY,
                        true,
                        pathMaterial,
                        ExplorationCellRole.Road | ExplorationCellRole.Room | ExplorationCellRole.Clearing);
                case 'b':
                    return new WorldAreaCellTemplate(
                        offsetX,
                        offsetY,
                        true,
                        ExplorationMaterial.BridgeDeck,
                        ExplorationCellRole.Road | ExplorationCellRole.Bridge | ExplorationCellRole.Clearing);
                case 'a':
                    ExplorationCellRole approachRoles =
                        ExplorationCellRole.Road | ExplorationCellRole.Threshold | ExplorationCellRole.Clearing;
                    if (pathMaterial == ExplorationMaterial.BridgeDeck)
                    {
                        approachRoles |= ExplorationCellRole.Bridge;
                    }
                    return new WorldAreaCellTemplate(offsetX, offsetY, true, pathMaterial, approachRoles);
                case 'c':
                    return new WorldAreaCellTemplate(
                        offsetX,
                        offsetY,
                        true,
                        featureMaterial,
                        ExplorationCellRole.Room | ExplorationCellRole.Threshold | ExplorationCellRole.Clearing);
                default:
                    throw new InvalidOperationException("Unknown regional-site template marker: " + marker);
            }
        }

        private static string LookupKey(string siteId, string zoneId)
        {
            return (siteId ?? "") + "\n" + (zoneId ?? "");
        }
    }
}
