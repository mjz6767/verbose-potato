using System;

namespace AshenHalls
{
    public enum NpcId
    {
        Unknown = -1,
        KingHalvard = 0,
        HeraldVann = 1,
        Nessa = 2,
        WatchmanRusk = 3,
        WatchwomanIlyra = 4,
        Tovan = 5,
        Mira = 6,
        Sera = 7,
        Orren = 8,
        Edda = 9,
        Pell = 10,
        CaptainBrann = 11,
        Kate = 12,
        Borin = 13,
        Tessa = 14,
        Maud = 15,
        Yara = 16,
        Lute = 17,
        DockWorker = 18,
        Scholar = 19
    }

    public static class NpcPortraitCatalog
    {
        public const int Columns = 5;
        public const int Rows = 4;

        public static NpcId Resolve(ObjectType focus, string speaker)
        {
            string name = (speaker ?? "").Trim().ToLowerInvariant();
            if (name.Contains("halvard")) return NpcId.KingHalvard;
            if (name.Contains("vann")) return NpcId.HeraldVann;
            if (name.Contains("nessa")) return NpcId.Nessa;
            if (name.Contains("ilyra")) return NpcId.WatchwomanIlyra;
            if (name.Contains("rusk") || name == "town guard" || name.Contains("watchman")) return NpcId.WatchmanRusk;
            if (name.Contains("tovan")) return NpcId.Tovan;
            if (name.Contains("mira")) return NpcId.Mira;
            if (name.Contains("sera")) return NpcId.Sera;
            if (name.Contains("orren")) return NpcId.Orren;
            if (name.Contains("edda") || name.Contains("edna")) return NpcId.Edda;
            if (name.Contains("pell")) return NpcId.Pell;
            if (name.Contains("brann")) return NpcId.CaptainBrann;
            if (name == "kate") return NpcId.Kate;
            if (name.Contains("borin")) return NpcId.Borin;
            if (name.Contains("tessa")) return NpcId.Tessa;
            if (name.Contains("maud")) return NpcId.Maud;
            if (name.Contains("yara")) return NpcId.Yara;
            if (name.Contains("lute") || name.Contains("provision")) return NpcId.Lute;

            switch (focus)
            {
                case ObjectType.KingHall:
                case ObjectType.KingHalvard:
                    return NpcId.KingHalvard;
                case ObjectType.RoyalHerald: return NpcId.HeraldVann;
                case ObjectType.MarketClerk: return NpcId.Nessa;
                case ObjectType.TownGuard: return NpcId.WatchmanRusk;
                case ObjectType.CityCourier: return NpcId.Tovan;
                case ObjectType.TempleHealer: return NpcId.Mira;
                case ObjectType.NoviceHealer: return NpcId.Sera;
                case ObjectType.TavernKeeper:
                case ObjectType.Tavern:
                    return NpcId.Orren;
                case ObjectType.WoundedTraveler: return NpcId.Edda;
                case ObjectType.StableHand: return NpcId.Pell;
                case ObjectType.GateCaptain: return NpcId.CaptainBrann;
                case ObjectType.Diner: return NpcId.Kate;
                case ObjectType.Armorer:
                case ObjectType.ArmorerNpc:
                case ObjectType.RatPeltQuest:
                    return NpcId.Borin;
                case ObjectType.WeaponVendor:
                case ObjectType.WeaponMerchantNpc:
                    return NpcId.Tessa;
                case ObjectType.Enchanter:
                case ObjectType.EnchanterNpc:
                    return NpcId.Maud;
                case ObjectType.OldRoadScout: return NpcId.Yara;
                case ObjectType.Provisions: return NpcId.Lute;
                default: return NpcId.Unknown;
            }
        }

        public static int PortraitIndex(ObjectType focus, string speaker)
        {
            return (int)Resolve(focus, speaker);
        }

        public static int WorldSpriteIndex(ObjectType type, bool eastSideGuard)
        {
            switch (type)
            {
                case ObjectType.TownGuard: return eastSideGuard ? 1 : 0;
                case ObjectType.KingHalvard: return 2;
                case ObjectType.MarketClerk: return 3;
                case ObjectType.TempleHealer: return 4;
                case ObjectType.TavernKeeper: return 5;
                case ObjectType.ArmorerNpc: return 6;
                case ObjectType.WeaponMerchantNpc: return 7;
                case ObjectType.GateCaptain: return 8;
                case ObjectType.EnchanterNpc: return 9;
                case ObjectType.CityCourier: return 12;
                case ObjectType.WoundedTraveler: return 13;
                case ObjectType.StableHand: return 15;
                case ObjectType.RoyalHerald: return 16;
                case ObjectType.NoviceHealer: return 17;
                case ObjectType.OldRoadScout: return 18;
                default: return -1;
            }
        }
    }
}
