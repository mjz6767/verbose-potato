using System;

namespace AshenHalls
{
    public static class WorldZoneCatalog
    {
        private static readonly WorldZone ThroneRoom = Zone(
            "midgaard-throne-room",
            "Throne Room",
            "royal hall",
            0,
            "blue-and-gold banners, old stone, warm braziers, and the royal writ",
            "King Halvard receives the party here before the Old Road opens.");

        private static readonly WorldZone GrandHearth = Zone(
            "midgaard-grand-hearth",
            "Grand Hearth",
            "last fire before the Old Road",
            0,
            "blackened timber, an old stone hearth, rain-blue windows, and a company runner",
            "Every road-company begins here: four names at the fire, one storm-bright door, and Midgaard waiting beyond.");

        private static readonly WorldZone MerchantHall = Zone(
            "midgaard-merchant-hall",
            "Merchant Hall",
            "city shops",
            0,
            "armor stands, weapon racks, provision shelves, and quiet rune-light",
            "Borin, Tessa, and Maud keep Midgaard's practical trade under one roof.");

        private static readonly WorldZone MidgaardCity = Zone(
            "midgaard-city",
            "Midgaard",
            "safe town",
            0,
            "market lamps, temple bells, vendor rows, east and west gates, and old stone walls",
            "Midgaard is the party's starting anchor: speak with the king, gather supplies, then clear the sewer rats.");

        private static readonly WorldZone MidgaardRoad = Zone(
            "midgaard-road",
            "Midgaard Road",
            "safe road",
            0,
            "Midgaard's lamp road, patrol stones, and last warm windows.",
            "This is the party's anchor: heal, regroup, then push outward.");

        private static readonly WorldZone OldQuarry = Zone(
            "old-quarry",
            "Old Quarry",
            "stone yards",
            2,
            "broken quarries, hidden caches, and hard-footed raiders",
            "The quarry road should eventually feed stoneborn lore, bridge repairs, and heavy gear caches.");

        private static readonly WorldZone GlassWarrens = Zone(
            "glass-warrens",
            "Glass Warrens",
            "crystal maze",
            3,
            "glass rubble, cold light, and caster spoor",
            "Glass shines where the Old Road cracked; later chapters can put mage factions and mirror puzzles here.");

        private static readonly WorldZone AshFen = Zone(
            "ash-fen",
            "Ash Fen",
            "poison fen",
            2,
            "mud banks, green fireflies, and sick water",
            "The fen is the first natural hazard zone: poison, mire movement, and shrine recovery pressure.");

        private static readonly WorldZone RedGate = Zone(
            "red-gate",
            "Red Gate",
            "war gate",
            4,
            "red basalt, old banners, and death-cult signs",
            "The Red Gate is the long arc's pressure point: bone wizards, pact risks, and gate keys.");

        private static readonly WorldZone GloamCourts = Zone(
            "gloam-courts",
            "Gloam Courts",
            "upper ruins",
            2,
            "fallen halls, court stones, and watchful dark",
            "These courts bridge rats and kobolds into organized ruin factions.");

        private static readonly WorldZone SaltCisterns = Zone(
            "salt-cisterns",
            "Salt Cisterns",
            "sewers",
            1,
            "damp passages, rat nests, and old sluices",
            "This is the early story's first proving ground: rats, supplies, and sewer stairs.");

        private static readonly WorldZone GreenShrineRoad = Zone(
            "green-shrine-road",
            "Green Shrine Road",
            "pilgrim road",
            1,
            "mossy paths, teal lamps, and old priest stones",
            "A recovery-oriented road for priest lore, shrines, and Tree Cover tutoring.");

        private static readonly WorldZone DuskMarket = Zone(
            "dusk-market",
            "Dusk Market Ruins",
            "market ruins",
            2,
            "collapsed stalls, thieves' marks, and kobold scouts",
            "The market is the rogue/ranger pressure zone: ambushes, caches, and cave mouths.");

        private static readonly WorldZone[] InnerAshRoad =
        {
            Zone("inner-ash-road", "Midgaard Outworks", "central road", 1, "old paving, road shrines, and patrol signs", "The spine of the world map, used to connect chapter objectives."),
            Zone("inner-ash-road", "Midgaard Outworks", "central road", 1, "old paving, road shrines, and patrol signs", "The spine of the world map, used to connect chapter objectives."),
            Zone("inner-ash-road", "Inner Ash Road", "central road", 2, "old paving, road shrines, and patrol signs", "The spine of the world map, used to connect chapter objectives."),
            Zone("inner-ash-road", "Inner Ash Road", "central road", 3, "old paving, road shrines, and patrol signs", "The spine of the world map, used to connect chapter objectives."),
            Zone("inner-ash-road", "Inner Ash Road", "central road", 4, "old paving, road shrines, and patrol signs", "The spine of the world map, used to connect chapter objectives.")
        };

        public static WorldZone For(string id, int depth)
        {
            switch (id ?? "")
            {
                case "midgaard-throne-room": return ThroneRoom;
                case "midgaard-grand-hearth": return GrandHearth;
                case "midgaard-merchant-hall": return MerchantHall;
                case "midgaard-city": return MidgaardCity;
                case "midgaard-road": return MidgaardRoad;
                case "old-quarry": return OldQuarry;
                case "glass-warrens": return GlassWarrens;
                case "ash-fen": return AshFen;
                case "red-gate": return RedGate;
                case "gloam-courts": return GloamCourts;
                case "salt-cisterns": return SaltCisterns;
                case "green-shrine-road": return GreenShrineRoad;
                case "dusk-market": return DuskMarket;
                default: return InnerAshRoad[Math.Max(1, Math.Min(4, depth))];
            }
        }

        private static WorldZone Zone(string id, string name, string title, int danger, string summary, string story)
        {
            return new WorldZone
            {
                Id = id,
                Name = name,
                Title = title,
                Danger = danger,
                Summary = summary,
                Story = story
            };
        }
    }
}
