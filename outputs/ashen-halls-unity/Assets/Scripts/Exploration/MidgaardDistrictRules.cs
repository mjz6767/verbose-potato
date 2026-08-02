using System;

namespace AshenHalls
{
    public static class MidgaardDistrictRules
    {
        public static ExplorationMaterial MaterialAtOffset(int dx, int dy)
        {
            if (dy <= -6 && Math.Abs(dx) <= 5) return ExplorationMaterial.KeepStone;
            if (dy >= -5 && dy <= -2 && dx >= -3 && dx <= 4) return ExplorationMaterial.TempleStone;
            if (dy >= 4 && dx <= 1) return ExplorationMaterial.SewerBrick;
            if (dy >= 2 && dx >= 2) return ExplorationMaterial.MarketCobbles;
            if (dy >= 0 && dx <= -3) return ExplorationMaterial.MarketCobbles;
            if (dy >= -1 && dy <= 2 && dx >= -2 && dx <= 6) return ExplorationMaterial.MarketCobbles;
            return ExplorationMaterial.CityPaving;
        }

        public static ExplorationCellRole RolesAtOffset(int dx, int dy)
        {
            ExplorationCellRole roles = ExplorationCellRole.City;
            bool secondaryLane = dy == -3 && dx >= -7 && dx <= 7
                || dy == 3 && dx >= -7 && dx <= 7
                || dx == -5 && dy >= -5 && dy <= 5
                || dx == 5 && dy >= -3 && dy <= 5;
            if (secondaryLane) roles |= ExplorationCellRole.Road;

            bool precinct = dy <= -5 && Math.Abs(dx) <= 5
                || dy >= -1 && dy <= 1 && dx >= -2 && dx <= 3
                || dy >= 4 && dy <= 6 && dx >= -6 && dx <= 1;
            if (precinct) roles |= ExplorationCellRole.Plaza;
            return roles;
        }

        public static string DistrictAtOffset(int dx, int dy)
        {
            ExplorationMaterial material = MaterialAtOffset(dx, dy);
            switch (material)
            {
                case ExplorationMaterial.KeepStone: return "Royal Approach";
                case ExplorationMaterial.TempleStone: return "Temple Precinct";
                case ExplorationMaterial.SewerBrick: return "Cistern Quarter";
                case ExplorationMaterial.MarketCobbles:
                    return dy >= 2 && dx >= 2 ? "Wharf Market" : dx <= -3 ? "Tavern Ward" : "Trade Ward";
                default: return "Civic Ward";
            }
        }
    }
}
