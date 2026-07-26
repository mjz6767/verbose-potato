using System;

namespace AshenHalls
{
    public static class GameAudioCueRules
    {
        public static string FootstepFor(ExplorationMaterial material)
        {
            switch (material)
            {
                case ExplorationMaterial.ShallowWater:
                case ExplorationMaterial.DeepWater:
                    return "footwater";
                case ExplorationMaterial.BridgeDeck:
                    return "footwood";
                case ExplorationMaterial.GlassRubble:
                    return "footglass";
                case ExplorationMaterial.FenMud:
                    return "footmud";
                case ExplorationMaterial.RedAsh:
                    return "footash";
                case ExplorationMaterial.QuarryStone:
                case ExplorationMaterial.RuinedPaving:
                case ExplorationMaterial.Cliff:
                    return "footgravel";
                case ExplorationMaterial.NaturalGround:
                case ExplorationMaterial.PackedDirt:
                case ExplorationMaterial.Moss:
                case ExplorationMaterial.Forest:
                    return "footearth";
                default:
                    return "footstone";
            }
        }

        public static float FootstepPitch(int x, int y)
        {
            unchecked
            {
                int hash = x * 73856093 ^ y * 19349663;
                int step = (hash & 0x7fffffff) % 9;
                return 0.96f + step * 0.01f;
            }
        }

        public static bool IsAmbientLandmark(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Market:
                case ObjectType.MarketClerk:
                case ObjectType.Provisions:
                case ObjectType.Diner:
                case ObjectType.Tavern:
                case ObjectType.TavernKeeper:
                case ObjectType.CityCourier:
                case ObjectType.StableHand:
                case ObjectType.WeaponVendor:
                case ObjectType.WeaponMerchantNpc:
                case ObjectType.Temple:
                case ObjectType.TempleHealer:
                case ObjectType.NoviceHealer:
                case ObjectType.Fountain:
                case ObjectType.Armorer:
                case ObjectType.ArmorerNpc:
                case ObjectType.Enchanter:
                case ObjectType.EnchanterNpc:
                case ObjectType.ForgeSite:
                case ObjectType.TrainingGround:
                case ObjectType.EastGate:
                case ObjectType.WestGate:
                case ObjectType.NorthGate:
                case ObjectType.SouthGate:
                case ObjectType.GateCaptain:
                case ObjectType.TownGuard:
                case ObjectType.CityWall:
                case ObjectType.Sewer:
                case ObjectType.KingHalvard:
                case ObjectType.RoyalThrone:
                case ObjectType.RoyalBrazier:
                case ObjectType.MerchantCounter:
                case ObjectType.Camp:
                case ObjectType.Waystone:
                case ObjectType.Shrine:
                case ObjectType.AncientGrove:
                case ObjectType.Cave:
                case ObjectType.Stairs:
                case ObjectType.DungeonGate:
                case ObjectType.Ruin:
                case ObjectType.DeepCrypt:
                case ObjectType.Obelisk:
                case ObjectType.LoreLibrary:
                case ObjectType.PortalSeal:
                case ObjectType.FactionCamp:
                case ObjectType.Encounter:
                    return true;
                default:
                    return false;
            }
        }

        public static string AmbientFor(string zoneId, ObjectType? nearbyLandmark)
        {
            string zone = (zoneId ?? "").Trim().ToLowerInvariant();
            if (zone == "midgaard-throne-room") return "ambbell";
            if (zone == "midgaard-merchant-hall")
            {
                switch (nearbyLandmark)
                {
                    case ObjectType.ArmorerNpc:
                    case ObjectType.WeaponMerchantNpc:
                    case ObjectType.EnchanterNpc:
                    case ObjectType.ForgeSite:
                        return "ambforge";
                    default:
                        return "ambmarket";
                }
            }
            if (zone == "midgaard-city")
            {
                switch (nearbyLandmark)
                {
                    case ObjectType.Temple:
                    case ObjectType.TempleHealer:
                    case ObjectType.NoviceHealer:
                    case ObjectType.Fountain:
                        return "ambbell";
                    case ObjectType.Armorer:
                    case ObjectType.Enchanter:
                    case ObjectType.ForgeSite:
                    case ObjectType.TrainingGround:
                        return "ambforge";
                    case ObjectType.EastGate:
                    case ObjectType.WestGate:
                    case ObjectType.NorthGate:
                    case ObjectType.SouthGate:
                    case ObjectType.GateCaptain:
                    case ObjectType.TownGuard:
                    case ObjectType.CityWall:
                        return "ambgate";
                    case ObjectType.Sewer:
                        return "ambdrip";
                    case ObjectType.Market:
                    case ObjectType.MarketClerk:
                    case ObjectType.Provisions:
                    case ObjectType.Diner:
                    case ObjectType.Tavern:
                    case ObjectType.TavernKeeper:
                    case ObjectType.CityCourier:
                    case ObjectType.StableHand:
                    case ObjectType.WeaponVendor:
                        return "ambmarket";
                    default:
                        return "ambcity";
                }
            }

            switch (nearbyLandmark)
            {
                case ObjectType.Camp:
                case ObjectType.Waystone:
                    return "ambcamp";
                case ObjectType.Shrine:
                case ObjectType.AncientGrove:
                    return "ambgrove";
                case ObjectType.Cave:
                case ObjectType.Stairs:
                case ObjectType.DungeonGate:
                    return "ambcave";
                case ObjectType.Ruin:
                case ObjectType.DeepCrypt:
                    return "ambruin";
                case ObjectType.Obelisk:
                case ObjectType.LoreLibrary:
                case ObjectType.PortalSeal:
                    return "ambglass";
                case ObjectType.FactionCamp:
                case ObjectType.Encounter:
                    return "ambdrum";
            }

            if (zone == "salt-cisterns") return "ambdrip";
            if (zone == "dusk-market") return "ambdrum";
            if (zone == "old-quarry") return "ambstone";
            if (zone == "green-shrine-road") return "ambgrove";
            if (zone == "glass-warrens") return "ambglass";
            if (zone == "ash-fen") return "ambfen";
            if (zone == "gloam-courts") return "ambruin";
            if (zone == "red-gate") return "ambgate";
            return "ambwind";
        }

        public static float AmbientInterval(int seed, int sequence)
        {
            int hash = StableMix(seed, sequence);
            return 9f + hash % 7;
        }

        public static float AmbientPan(int seed, int sequence)
        {
            int hash = StableMix(seed ^ 0x2c9277b5, sequence + 17);
            return -0.24f + (hash % 49) * 0.01f;
        }

        public static float AmbientPitch(int seed, int sequence)
        {
            int hash = StableMix(seed ^ 0x14f42a75, sequence + 31);
            return 0.96f + (hash % 9) * 0.01f;
        }

        public static float AmbientVolume(string cue)
        {
            switch ((cue ?? "").ToLowerInvariant())
            {
                case "ambforge": return 0.24f;
                case "ambgate": return 0.22f;
                case "ambbell": return 0.20f;
                case "ambdrum": return 0.19f;
                case "ambdrip": return 0.18f;
                case "ambmarket": return 0.17f;
                case "ambstone": return 0.17f;
                case "ambcamp": return 0.16f;
                case "ambcave": return 0.16f;
                case "ambgrove": return 0.15f;
                case "ambfen": return 0.14f;
                case "ambglass": return 0.14f;
                case "ambruin": return 0.14f;
                default: return 0.15f;
            }
        }

        private static int StableMix(int seed, int sequence)
        {
            unchecked
            {
                int hash = seed * 73856093 ^ sequence * 19349663 ^ 83492791;
                hash ^= hash >> 13;
                return hash & 0x7fffffff;
            }
        }
    }
}
