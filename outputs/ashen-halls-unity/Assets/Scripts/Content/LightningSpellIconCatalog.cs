using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public static class LightningSpellIconCatalog
    {
        public const int AtlasColumns = 4;
        public const int AtlasRows = 2;

        private static readonly Dictionary<string, int> indices =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["RIG"] = 0,
                ["CLT"] = 1,
                ["RSG"] = 5,
                ["AST"] = 3,
                ["VST"] = 4
            };

        public static int LightningIndex(string formulaCode)
        {
            return indices.TryGetValue(formulaCode ?? "", out int index) ? index : -1;
        }
    }
}
