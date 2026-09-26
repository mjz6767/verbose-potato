using System;
using System.Collections.Generic;
using UnityEngine;

namespace AshenHalls
{
    public static class ExplorationMapVisualRules
    {
        public static string InspectionHeading(string name, WorldMapCellAccessKind access, int distance, bool wideView,
            string status = "")
        {
            string action;
            if (wideView) action = "Inspect only";
            else if (access == WorldMapCellAccessKind.EnemyOccupied)
                action = distance == 1 ? "Click to engage" : "Range " + distance;
            else if (access == WorldMapCellAccessKind.UseFromBeside)
                action = distance == 1 ? "Use from beside" : "Approach to interact";
            else if (!ExplorationReadabilityRules.IsWalkableAccess(access)) action = "Blocks movement";
            else action = distance == 0 ? "Underfoot" : distance == 1 ? "Click to move" : "Walkable · Range " + distance;
            // Identity and action come first; long lore stays in the Details rail.
            // Keep safety/access information textual instead of relying on color.
            return (name ?? "Ground") + " · " + action + (string.IsNullOrEmpty(status) ? "" : " · " + status);
        }

        public static Rect PartyFootprint(Rect cell)
        {
            return new Rect(cell.x + cell.width * 0.12f, cell.y + cell.height * 0.84f,
                cell.width * 0.76f, cell.height * 0.12f);
        }

        public static bool ShouldDrawPassiveMovementCue(WorldMapCellAccessKind access, bool currentInteraction, bool hovered)
        {
            // The active contact already owns a foot bracket and E prompt.
            // Open ground needs no permanent compass; hover still explains it.
            // Collision and danger warnings remain visible without hovering.
            if (currentInteraction && access == WorldMapCellAccessKind.UseFromBeside) return false;
            return hovered || !ExplorationReadabilityRules.IsWalkableAccess(access);
        }

        public static float RouteStrokeWidth(float cellPixels, bool wideView)
        {
            return Mathf.Clamp(cellPixels * (wideView ? 0.07f : 0.045f), 2f, 5f);
        }

        public static float RouteCoreWidth(float cellPixels, bool wideView)
        {
            float outer = RouteStrokeWidth(cellPixels, wideView);
            // A sub-pixel gold core disappears into cobblestone seams. Keep
            // contrast in the narrow line instead of restoring large boxes.
            return Mathf.Min(outer, Mathf.Max(1.5f, outer * 0.58f));
        }
    }

    // Reuse storage and sort only viewport candidates. Original index preserves
    // the old LINQ stable order for equal positions/IDs without sorting the map.
    public sealed class ExplorationVisibleObjectBuffer
    {
        public struct Entry
        {
            public MapObject Object;
            public int SourceIndex;
        }

        private sealed class EntryComparer : IComparer<Entry>
        {
            public int Compare(Entry a, Entry b)
            {
                int order = a.Object.Y.CompareTo(b.Object.Y);
                if (order == 0) order = a.Object.X.CompareTo(b.Object.X);
                if (order == 0) order = StringComparer.Ordinal.Compare(a.Object.Id ?? "", b.Object.Id ?? "");
                return order == 0 ? a.SourceIndex.CompareTo(b.SourceIndex) : order;
            }
        }

        private static readonly EntryComparer Comparer = new EntryComparer();
        public readonly List<Entry> Items = new List<Entry>(128);
        public int SourceVisits { get; private set; }

        public void Rebuild(IReadOnlyList<MapObject> source, int x, int y, int width, int height)
        {
            Items.Clear();
            SourceVisits = 0;
            if (source == null || width <= 0 || height <= 0) return;
            for (int i = 0; i < source.Count; i++)
            {
                SourceVisits++;
                MapObject obj = source[i];
                if (obj == null || obj.X < x || obj.Y < y || obj.X >= x + width || obj.Y >= y + height) continue;
                Items.Add(new Entry { Object = obj, SourceIndex = i });
            }
            Items.Sort(Comparer);
        }
    }
}
