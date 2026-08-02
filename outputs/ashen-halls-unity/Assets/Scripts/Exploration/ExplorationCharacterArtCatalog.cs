using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public enum AmbientCitizenProfession
    {
        Unknown = -1,
        Lamplighter = 0,
        Fishmonger = 1,
        Tailor = 2,
        Mason = 3,
        Apothecary = 4,
        RoadPilgrim = 5,
        Gravedigger = 6,
        CaravanGuide = 7
    }

    public static class ExplorationCharacterArtCatalog
    {
        public const int PlayerColumns = 4;
        public const int PlayerRows = 2;
        public const int PlayerCellCount = PlayerColumns * PlayerRows;
        public const int CitizenColumns = 4;
        public const int CitizenRows = 2;
        public const int CitizenCellCount = CitizenColumns * CitizenRows;
        public const int PartyGroupBypassIndex = -1;

        private static readonly AmbientCitizenProfession[] RoyalApproachProfessions =
        {
            AmbientCitizenProfession.Lamplighter,
            AmbientCitizenProfession.Mason,
            AmbientCitizenProfession.RoadPilgrim
        };

        private static readonly AmbientCitizenProfession[] TemplePrecinctProfessions =
        {
            AmbientCitizenProfession.Lamplighter,
            AmbientCitizenProfession.Apothecary,
            AmbientCitizenProfession.RoadPilgrim
        };

        private static readonly AmbientCitizenProfession[] CisternProfessions =
        {
            AmbientCitizenProfession.Lamplighter,
            AmbientCitizenProfession.Mason,
            AmbientCitizenProfession.Gravedigger
        };

        private static readonly AmbientCitizenProfession[] WharfProfessions =
        {
            AmbientCitizenProfession.Fishmonger,
            AmbientCitizenProfession.Lamplighter,
            AmbientCitizenProfession.CaravanGuide
        };

        private static readonly AmbientCitizenProfession[] TavernProfessions =
        {
            AmbientCitizenProfession.Lamplighter,
            AmbientCitizenProfession.Tailor,
            AmbientCitizenProfession.CaravanGuide
        };

        private static readonly AmbientCitizenProfession[] TradeProfessions =
        {
            AmbientCitizenProfession.Fishmonger,
            AmbientCitizenProfession.Tailor,
            AmbientCitizenProfession.CaravanGuide
        };

        private static readonly AmbientCitizenProfession[] CivicProfessions =
        {
            AmbientCitizenProfession.Lamplighter,
            AmbientCitizenProfession.Tailor,
            AmbientCitizenProfession.Mason,
            AmbientCitizenProfession.RoadPilgrim
        };

        private static readonly AmbientCitizenProfession[] QuarryProfessions =
        {
            AmbientCitizenProfession.Mason,
            AmbientCitizenProfession.RoadPilgrim,
            AmbientCitizenProfession.CaravanGuide
        };

        private static readonly AmbientCitizenProfession[] FenProfessions =
        {
            AmbientCitizenProfession.Apothecary,
            AmbientCitizenProfession.RoadPilgrim,
            AmbientCitizenProfession.Gravedigger
        };

        private static readonly AmbientCitizenProfession[] GloamProfessions =
        {
            AmbientCitizenProfession.Tailor,
            AmbientCitizenProfession.Gravedigger,
            AmbientCitizenProfession.RoadPilgrim
        };

        private static readonly AmbientCitizenProfession[] RoadProfessions =
        {
            AmbientCitizenProfession.Lamplighter,
            AmbientCitizenProfession.RoadPilgrim,
            AmbientCitizenProfession.CaravanGuide
        };

        public static int PlayerRoleIndex(string role)
        {
            switch (NormalizeKey(role))
            {
                case "shield": return 0;
                case "pike": return 1;
                case "bow": return 2;
                case "knife": return 3;
                case "mender": return 4;
                case "ember": return 5;
                case "hex": return 6;
                case "ward": return 7;
                default: return -1;
            }
        }

        public static int PlayerTokenIndex(int representedPartyCount, string leadRole)
        {
            return representedPartyCount == 1
                ? PlayerRoleIndex(leadRole)
                : PartyGroupBypassIndex;
        }

        public static bool UsesPartyGroupToken(int representedPartyCount)
        {
            return representedPartyCount != 1;
        }

        public static int CitizenAtlasIndex(AmbientCitizenProfession profession)
        {
            int index = (int)profession;
            return index >= 0 && index < CitizenCellCount ? index : -1;
        }

        public static AmbientCitizenProfession CitizenProfessionAt(int atlasIndex)
        {
            return atlasIndex >= 0 && atlasIndex < CitizenCellCount
                ? (AmbientCitizenProfession)atlasIndex
                : AmbientCitizenProfession.Unknown;
        }

        public static string CitizenAncestry(AmbientCitizenProfession profession)
        {
            switch (profession)
            {
                case AmbientCitizenProfession.Lamplighter:
                case AmbientCitizenProfession.RoadPilgrim:
                    return "Human";
                case AmbientCitizenProfession.Fishmonger:
                case AmbientCitizenProfession.Gravedigger:
                    return "Ashling";
                case AmbientCitizenProfession.Tailor:
                case AmbientCitizenProfession.CaravanGuide:
                    return "Dusk-elf";
                case AmbientCitizenProfession.Mason:
                    return "Stoneborn";
                case AmbientCitizenProfession.Apothecary:
                    return "Fenkin";
                default:
                    return "";
            }
        }

        public static IReadOnlyList<AmbientCitizenProfession> ProfessionsForDistrict(string district)
        {
            switch (NormalizeKey(district))
            {
                case "royalapproach":
                    return RoyalApproachProfessions;
                case "templeprecinct":
                case "greenshrineroad":
                    return TemplePrecinctProfessions;
                case "cisternquarter":
                case "saltcisterns":
                    return CisternProfessions;
                case "wharfmarket":
                    return WharfProfessions;
                case "tavernward":
                case "midgaardgrandhearth":
                    return TavernProfessions;
                case "tradeward":
                case "duskmarket":
                    return TradeProfessions;
                case "civicward":
                case "midgaardcity":
                    return CivicProfessions;
                case "oldquarry":
                    return QuarryProfessions;
                case "ashfen":
                    return FenProfessions;
                case "gloamcourts":
                case "glasswarrens":
                case "redgate":
                    return GloamProfessions;
                default:
                    return RoadProfessions;
            }
        }

        public static AmbientCitizenProfession AmbientProfession(
            string district,
            int worldSeed,
            int x,
            int y)
        {
            IReadOnlyList<AmbientCitizenProfession> candidates = ProfessionsForDistrict(district);
            int selection = StableCoordinateHash(worldSeed, x, y, district, 15485863) % candidates.Count;
            return candidates[selection];
        }

        public static int AmbientCitizenIndex(string district, int worldSeed, int x, int y)
        {
            return CitizenAtlasIndex(AmbientProfession(district, worldSeed, x, y));
        }

        public static bool CanPlaceAmbientCitizen(
            bool isTutorialLane,
            bool isCertifiedSafeRoad,
            bool isGuidanceRoute,
            bool isEntrance,
            bool hasInteractable)
        {
            return !isTutorialLane
                && !isCertifiedSafeRoad
                && !isGuidanceRoute
                && !isEntrance
                && !hasInteractable;
        }

        public static bool ShouldPlaceAmbientCitizen(
            string district,
            int worldSeed,
            int x,
            int y,
            bool isTutorialLane,
            bool isCertifiedSafeRoad,
            bool isGuidanceRoute,
            bool isEntrance,
            bool hasInteractable)
        {
            if (!CanPlaceAmbientCitizen(
                isTutorialLane,
                isCertifiedSafeRoad,
                isGuidanceRoute,
                isEntrance,
                hasInteractable))
            {
                return false;
            }

            int roll = StableCoordinateHash(worldSeed, x, y, district, 32452843) % 100;
            return roll < DensityPercent(district);
        }

        private static int DensityPercent(string district)
        {
            switch (NormalizeKey(district))
            {
                case "royalapproach":
                case "templeprecinct":
                case "wharfmarket":
                case "tavernward":
                case "tradeward":
                case "civicward":
                case "midgaardcity":
                    return 20;
                default:
                    return 12;
            }
        }

        private static int StableCoordinateHash(
            int worldSeed,
            int x,
            int y,
            string district,
            int salt)
        {
            unchecked
            {
                int hash = worldSeed ^ salt;
                hash = hash * 397 ^ x * 193;
                hash = hash * 397 ^ y * 389;
                string key = NormalizeKey(district);
                for (int i = 0; i < key.Length; i++) hash = hash * 31 + key[i];
                hash ^= hash >> 16;
                return hash & int.MaxValue;
            }
        }

        private static string NormalizeKey(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            return value.Trim()
                .ToLowerInvariant()
                .Replace("-", "")
                .Replace("_", "")
                .Replace(" ", "");
        }
    }
}
