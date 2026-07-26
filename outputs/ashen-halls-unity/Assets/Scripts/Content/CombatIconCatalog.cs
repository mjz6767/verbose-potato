using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public static class CombatIconCatalog
    {
        public const int AbilityAtlasColumns = 4;
        public const int ExpandedAbilityAtlasRows = 5;

        private static readonly Dictionary<string, int> abilityIndices = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["charge"] = 0,
            ["execute"] = 1,
            ["rally"] = 2,
            ["whirlwind"] = 3,
            ["shieldbash"] = 4,
            ["cleave"] = 5,
            ["stealth"] = 6,
            ["ambush"] = 7,
            ["eviscerate"] = 8,
            ["throwknife"] = 9,
            ["hamstring"] = 10,
            ["aimedshot"] = 11,
            ["pinningshot"] = 12,
            ["volley"] = 13,
            ["scoutmark"] = 14,
            ["broadheadshot"] = 15,
            ["disruptingshot"] = 16,
            ["enrage"] = 17,
            ["hunterfocus"] = 18,
            ["smokebomb"] = 19
        };

        private static readonly Dictionary<string, int> signatureSpellIndices = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["GBH"] = 0,
            ["HLC"] = 1,
            ["OIC"] = 2,
            ["TBQ"] = 3,
            ["SBN"] = 4,
            ["FIF"] = 5,
            ["FBL"] = 6,
            ["MTR"] = 7,
            ["CLT"] = 8,
            ["FRB"] = 9,
            ["WBK"] = 10,
            ["WBP"] = 11,
            ["RMS"] = 12,
            ["INH"] = 13,
            ["RLM"] = 14,
            ["IBD"] = 15,
            ["IBF"] = 16,
            ["IBG"] = 17,
            ["PBR"] = 18,
            ["DMC"] = 19,
            ["RCL"] = 20,
            ["RDF"] = 21,
            ["RSG"] = 22,
            ["LBC"] = 23,
            ["TNC"] = 24,
            ["VST"] = 8,
            ["AST"] = 22,
            ["DFA"] = 17,
            ["SRF"] = 4
        };

        public static int AbilityIndex(string abilityId)
        {
            return abilityIndices.TryGetValue(abilityId ?? "", out int index) ? index : -1;
        }

        public static int SignatureSpellIndex(string formulaCode)
        {
            return signatureSpellIndices.TryGetValue(formulaCode ?? "", out int index) ? index : -1;
        }

        public static int AbilityAtlasRows(int width, int height)
        {
            if (width <= 0 || height <= 0) return 2;
            if (height >= width * 1.10f) return ExpandedAbilityAtlasRows;
            return height >= width * 0.65f ? 3 : 2;
        }
    }
}
