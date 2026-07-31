using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public static class CombatIconCatalog
    {
        public const int AbilityAtlasColumns = 4;
        public const int ExpandedAbilityAtlasRows = 5;
        public const int AbilityAtlasCellSize = 256;
        public const int AbilityAtlasWidth = AbilityAtlasColumns * AbilityAtlasCellSize;
        public const int AbilityAtlasHeight = ExpandedAbilityAtlasRows * AbilityAtlasCellSize;
        public const int SignatureSpellAtlasColumns = 7;
        public const int SignatureSpellAtlasRows = 7;
        public const int BookStateAtlasColumns = 4;
        public const int BookStateAtlasRows = 3;
        public const int BookStateAtlasCellSize = 64;
        public const int BookStateAtlasWidth = BookStateAtlasColumns * BookStateAtlasCellSize;
        public const int BookStateAtlasHeight = BookStateAtlasRows * BookStateAtlasCellSize;

        public const int BookStateSelectionIndex = 0;
        public const int BookStateTargetingIndex = 1;
        public const int BookStateLockedIndex = 2;
        public const int BookStateLowResourceIndex = 3;
        public const int BookStateNoTargetIndex = 4;
        public const int BookStateActionUsedIndex = 5;
        public const int BookStateDisabledIndex = 6;
        public const int BookStateBlockedIndex = 7;
        public const int BookStateCostIndex = 8;
        public const int BookStateReachIndex = 9;
        public const int BookStateTargetIndex = 10;
        public const int BookStatePreviewIndex = 11;

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
            ["GBX"] = 1,
            ["HLC"] = 2,
            ["OIC"] = 3,
            ["NVC"] = 4,
            ["SRF"] = 5,
            ["TBQ"] = 6,
            ["SGW"] = 7,
            ["TNC"] = 8,
            ["LBC"] = 9,
            ["TBG"] = 10,
            ["OBL"] = 11,
            ["LNH"] = 12,
            ["SWR"] = 13,
            ["SBN"] = 14,
            ["FIF"] = 15,
            ["WBF"] = 16,
            ["BTF"] = 17,
            ["WBI"] = 18,
            ["RCL"] = 19,
            ["RDF"] = 20,
            ["RIG"] = 21,
            ["FBL"] = 22,
            ["RLF"] = 23,
            ["RSG"] = 24,
            ["RBI"] = 25,
            ["MTR"] = 26,
            ["CLT"] = 27,
            ["FRB"] = 28,
            ["VST"] = 29,
            ["AST"] = 30,
            ["WBK"] = 31,
            ["WBP"] = 32,
            ["DMC"] = 33,
            ["RMS"] = 34,
            ["RNH"] = 35,
            ["NVL"] = 36,
            ["RKW"] = 37,
            ["RPX"] = 38,
            ["INH"] = 39,
            ["RMB"] = 40,
            ["RLM"] = 41,
            ["WTR"] = 42,
            ["DSM"] = 43,
            ["IBD"] = 44,
            ["IBF"] = 45,
            ["PBR"] = 46,
            ["IBG"] = 47,
            ["DFA"] = 48
        };

        public static int AbilityIndex(string abilityId)
        {
            return abilityIndices.TryGetValue(abilityId ?? "", out int index) ? index : -1;
        }

        public static int SignatureSpellIndex(string formulaCode)
        {
            return signatureSpellIndices.TryGetValue(formulaCode ?? "", out int index) ? index : -1;
        }

        public static bool IsAbilityAtlasDimensions(int width, int height)
        {
            return width == AbilityAtlasWidth && height == AbilityAtlasHeight;
        }

        public static int AbilityAtlasRows(int width, int height)
        {
            return ExpandedAbilityAtlasRows;
        }

        public static bool IsBookStateAtlasDimensions(int width, int height)
        {
            return width == BookStateAtlasWidth && height == BookStateAtlasHeight;
        }
    }
}
