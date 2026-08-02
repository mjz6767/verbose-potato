namespace AshenHalls
{
    public static class WorldMapRegionMarkerCatalog
    {
        public const int Columns = 5;
        public const int Rows = 4;

        public static int ActorMarkerIndex(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.TownGuard:
                case ObjectType.GateCaptain:
                case ObjectType.RoyalHerald:
                case ObjectType.KingHalvard:
                    return 0;
                case ObjectType.CityCourier:
                case ObjectType.OldRoadScout:
                case ObjectType.Scholar:
                    return 1;
                case ObjectType.ArmorerNpc:
                case ObjectType.WeaponMerchantNpc:
                    return 2;
                case ObjectType.WoundedTraveler:
                    return 3;
                case ObjectType.TavernKeeper:
                case ObjectType.DinerCook:
                    return 5;
                case ObjectType.TempleHealer:
                case ObjectType.NoviceHealer:
                    return 11;
                case ObjectType.StableHand:
                case ObjectType.DockWorker:
                    return 14;
                case ObjectType.MarketClerk:
                case ObjectType.Provisioner:
                    return 10;
                case ObjectType.EnchanterNpc:
                    return 19;
                default:
                    return 1;
            }
        }

        public static bool ShouldShowActor(ObjectType type, int distance, bool objective)
        {
            if (objective || distance <= 3) return true;
            if (distance > 7) return false;
            switch (type)
            {
                case ObjectType.MarketClerk:
                case ObjectType.TempleHealer:
                case ObjectType.TavernKeeper:
                case ObjectType.GateCaptain:
                case ObjectType.CityCourier:
                case ObjectType.KingHalvard:
                case ObjectType.ArmorerNpc:
                case ObjectType.WeaponMerchantNpc:
                case ObjectType.EnchanterNpc:
                    return true;
                default:
                    return false;
            }
        }
    }
}
