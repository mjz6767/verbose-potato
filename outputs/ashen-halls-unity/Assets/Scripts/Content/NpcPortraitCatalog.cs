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
            if (name.Contains("dock worker") || name.Contains("dockworker")) return NpcId.DockWorker;
            if (name.Contains("scholar")) return NpcId.Scholar;

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
                case ObjectType.DinerCook: return NpcId.Kate;
                case ObjectType.Provisioner: return NpcId.Lute;
                case ObjectType.DockWorker: return NpcId.DockWorker;
                case ObjectType.Scholar: return NpcId.Scholar;
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
                case ObjectType.TownGuard:
                    return WorldSpriteIndex(eastSideGuard ? NpcId.WatchwomanIlyra : NpcId.WatchmanRusk);
                case ObjectType.KingHalvard: return WorldSpriteIndex(NpcId.KingHalvard);
                case ObjectType.MarketClerk: return WorldSpriteIndex(NpcId.Nessa);
                case ObjectType.TempleHealer: return WorldSpriteIndex(NpcId.Mira);
                case ObjectType.TavernKeeper: return WorldSpriteIndex(NpcId.Orren);
                case ObjectType.ArmorerNpc: return WorldSpriteIndex(NpcId.Borin);
                case ObjectType.WeaponMerchantNpc: return WorldSpriteIndex(NpcId.Tessa);
                case ObjectType.GateCaptain: return WorldSpriteIndex(NpcId.CaptainBrann);
                case ObjectType.EnchanterNpc: return WorldSpriteIndex(NpcId.Maud);
                case ObjectType.CityCourier: return WorldSpriteIndex(NpcId.Tovan);
                case ObjectType.WoundedTraveler: return WorldSpriteIndex(NpcId.Edda);
                case ObjectType.StableHand: return WorldSpriteIndex(NpcId.Pell);
                case ObjectType.RoyalHerald: return WorldSpriteIndex(NpcId.HeraldVann);
                case ObjectType.NoviceHealer: return WorldSpriteIndex(NpcId.Sera);
                case ObjectType.OldRoadScout: return WorldSpriteIndex(NpcId.Yara);
                case ObjectType.DinerCook: return WorldSpriteIndex(NpcId.Kate);
                case ObjectType.Provisioner: return WorldSpriteIndex(NpcId.Lute);
                case ObjectType.DockWorker: return WorldSpriteIndex(NpcId.DockWorker);
                case ObjectType.Scholar: return WorldSpriteIndex(NpcId.Scholar);
                default: return -1;
            }
        }

        public static int WorldSpriteIndex(NpcId id)
        {
            switch (id)
            {
                case NpcId.WatchmanRusk: return 0;
                case NpcId.WatchwomanIlyra: return 1;
                case NpcId.KingHalvard: return 2;
                case NpcId.Nessa: return 3;
                case NpcId.Mira: return 4;
                case NpcId.Orren: return 5;
                case NpcId.Borin: return 6;
                case NpcId.Tessa: return 7;
                case NpcId.CaptainBrann: return 8;
                case NpcId.Maud: return 9;
                case NpcId.Kate: return 10;
                case NpcId.Lute: return 11;
                case NpcId.Tovan: return 12;
                case NpcId.Edda: return 13;
                case NpcId.DockWorker: return 14;
                case NpcId.Pell: return 15;
                case NpcId.HeraldVann: return 16;
                case NpcId.Sera: return 17;
                case NpcId.Yara: return 18;
                case NpcId.Scholar: return 19;
                default: return -1;
            }
        }
    }
}
