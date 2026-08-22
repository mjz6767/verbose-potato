namespace AshenHalls
{
    public static class MidgaardTownArtCatalog
    {
        public const int Columns = 5;
        public const int Rows = 4;
        public const int CellSize = 256;

        public static int AtlasIndex(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Market: return 0;
                case ObjectType.Temple: return 1;
                case ObjectType.Fountain: return 2;
                case ObjectType.Tavern: return 3;
                case ObjectType.Armorer: return 4;
                case ObjectType.Provisions: return 5;
                case ObjectType.WeaponVendor: return 6;
                case ObjectType.Enchanter: return 7;
                case ObjectType.KingHall: return 11;
                case ObjectType.Sewer: return 12;
                case ObjectType.CityWall: return 13;
                case ObjectType.Diner: return 14;
                case ObjectType.RatPeltQuest: return 15;
                case ObjectType.RecallCircle: return 17;
                default: return -1;
            }
        }

        public static ObjectType PresentationType(ObjectType type, bool grandHearthDoor)
        {
            // The portal keeps its save-stable Tavern type for routing, while
            // its exterior art and skyline weight present the civic Town Hall.
            return grandHearthDoor ? ObjectType.KingHall : type;
        }

        public static bool IsArchitectureCell(int index)
        {
            return index == 0
                || index == 1
                || index == 3
                || index == 4
                || index == 5
                || index == 6
                || index == 7
                || index == 11
                || index == 14;
        }
    }
}
