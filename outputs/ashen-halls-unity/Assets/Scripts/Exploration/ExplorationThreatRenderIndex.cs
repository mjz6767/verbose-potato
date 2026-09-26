using System.Collections.Generic;

namespace AshenHalls
{
    // A presentation-only snapshot, rebuilt at the start of every map repaint.
    // It does not decide visibility, traversal, encounters or persisted state.
    public sealed class ExplorationThreatRenderIndex
    {
        private readonly HashSet<long> homes = new HashSet<long>();
        private readonly HashSet<long> activeHomes = new HashSet<long>();
        private readonly HashSet<long> activePatrols = new HashSet<long>();

        public void Rebuild(IReadOnlyList<RoamingThreat> threats, int depth)
        {
            homes.Clear();
            activeHomes.Clear();
            activePatrols.Clear();
            if (threats == null) return;

            for (int i = 0; i < threats.Count; i++)
            {
                RoamingThreat threat = threats[i];
                if (threat == null || threat.Depth != depth) continue;
                long home = CellKey(threat.HomeX, threat.HomeY);
                homes.Add(home);
                if (!threat.Active) continue;
                activeHomes.Add(home);
                // Match RoamingThreatAt's default exclusion exactly. Empty IDs
                // are excluded there; null IDs are not the empty string.
                if (threat.Id != "") activePatrols.Add(CellKey(threat.X, threat.Y));
            }
        }

        public bool HasHome(int x, int y) => homes.Contains(CellKey(x, y));
        public bool HasActiveHome(int x, int y) => activeHomes.Contains(CellKey(x, y));
        public bool HasActivePatrol(int x, int y) => activePatrols.Contains(CellKey(x, y));

        private static long CellKey(int x, int y)
        {
            // Packing both coordinates avoids width-dependent aliases and also
            // preserves the existing lookup behavior for out-of-bounds inputs.
            return ((long)x << 32) | (uint)y;
        }
    }
}
