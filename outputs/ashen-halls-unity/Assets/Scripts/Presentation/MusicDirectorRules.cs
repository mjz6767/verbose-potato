using System;

namespace AshenHalls
{
    public static class MusicDirectorRules
    {
        public const string Title = "tavern";
        public const string Tavern = Title;
        public const string GrandHearth = "grand-hearth";
        public const string WorldMapOverview = "world-map-overview";
        public const string Muster = "muster";
        public const string Victory = "victory";
        public const string Defeat = "defeat";

        public const string GreenShrineTrainingRing = "green-shrine-training-ring";
        public const string OldQuarryForge = "old-quarry-forge";
        public const string GloamDeepCrypt = "gloam-deep-crypt";
        public const string GlassLoreLibrary = "glass-lore-library";
        public const string DuskMarketHideout = "dusk-market-hideout";
        public const string RedGateSeal = "red-gate-seal";
        public const string SaltCisternGate = "salt-cistern-gate";
        public const string AshFenAncientGrove = "ash-fen-ancient-grove";

        public const string CombatGeneric = "combat-generic";
        public const string CombatSewer = "combat-sewer";
        public const string CombatBoss = "combat-boss";
        public const string CombatKobold = "combat-kobold";
        public const string CombatDrow = "combat-drow";
        public const string CombatDemon = "combat-demon";
        public const string CombatUndead = "combat-undead";
        public const string CombatRatfolk = "combat-ratfolk";
        public const string CombatArcaneDuel = "combat-arcane-duel";
        public const string CombatElite = "combat-elite";
        public const string CombatLastStand = "combat-last-stand";
        public const string CombatKoboldKing = "combat-kobold-king";
        public const string CombatDemonLord = "combat-demon-lord";

        public const string MidgaardTemple = "midgaard-temple";
        public const string MidgaardMarket = "midgaard-market-square";
        public const string MidgaardTavernLane = "midgaard-tavern-lane";
        public const string MidgaardGateWatch = "midgaard-gate-watch";
        public const string MidgaardCisternMouth = "midgaard-cistern-mouth";
        public const string MidgaardRoyalApproach = "midgaard-royal-approach";
        public const string MidgaardRoad = "midgaard-road";
        public const string RoadsideRest = "roadside-rest";
        public const string SacredGround = "sacred-ground";
        public const string UnderstoneThreshold = "understone-threshold";
        public const string ForgottenRuins = "forgotten-ruins";
        public const string ArcaneThreshold = "arcane-threshold";
        public const string HuntedRoad = "hunted-road";
        public const string AncientGrove = "ancient-grove";
        public const string FactionCamp = "faction-camp";

        public static string ExploreTrackKey(
            string zoneId,
            ObjectType landmark,
            bool hasLandmark,
            bool threatAlerted)
        {
            string zone = (zoneId ?? "").Trim().ToLowerInvariant();
            if (threatAlerted && zone != "midgaard-city" && !zone.StartsWith("midgaard-", StringComparison.Ordinal))
            {
                return HuntedRoad;
            }

            if (zone == "midgaard-throne-room" || zone == "midgaard-merchant-hall")
            {
                return zone;
            }

            if (zone == "midgaard-grand-hearth") return GrandHearth;

            if (zone == "midgaard-city")
            {
                if (!hasLandmark) return zone;
                if (IsTempleLandmark(landmark)) return MidgaardTemple;
                if (IsMarketLandmark(landmark)) return MidgaardMarket;
                if (IsTavernLandmark(landmark)) return MidgaardTavernLane;
                if (IsGateLandmark(landmark)) return MidgaardGateWatch;
                if (landmark == ObjectType.Sewer || landmark == ObjectType.RatPeltQuest) return MidgaardCisternMouth;
                if (IsRoyalLandmark(landmark)) return MidgaardRoyalApproach;
                return zone;
            }

            if (zone == "midgaard-road") return MidgaardRoad;
            if (!hasLandmark) return string.IsNullOrEmpty(zone) ? "road" : zone;

            switch (landmark)
            {
                case ObjectType.Camp:
                case ObjectType.Waystone:
                    return RoadsideRest;
                case ObjectType.Shrine:
                case ObjectType.Temple:
                    return SacredGround;
                case ObjectType.AncientGrove:
                    return AncientGrove;
                case ObjectType.Cave:
                case ObjectType.Stairs:
                case ObjectType.DungeonGate:
                    return UnderstoneThreshold;
                case ObjectType.Ruin:
                case ObjectType.DeepCrypt:
                    return ForgottenRuins;
                case ObjectType.Obelisk:
                case ObjectType.LoreLibrary:
                case ObjectType.PortalSeal:
                    return ArcaneThreshold;
                case ObjectType.FactionCamp:
                case ObjectType.Encounter:
                    return FactionCamp;
                default:
                    return string.IsNullOrEmpty(zone) ? "road" : zone;
            }
        }

        public static string CombatTrackKey(
            string encounterStyle,
            string dominantFaction,
            bool hasRatfolk,
            bool hasCaster,
            bool hasElite,
            bool hasGreaterDemon,
            bool partyCritical)
        {
            string style = (encounterStyle ?? "").Trim().ToLowerInvariant();
            string faction = (dominantFaction ?? "").Trim().ToLowerInvariant();
            if (style.Contains("koboldking") || style.Contains("kobold king")) return CombatKoboldKing;

            bool boss = IsBossStyle(style);
            if (boss && (hasGreaterDemon || faction == "demon")) return CombatDemonLord;
            if (boss) return CombatBoss;
            if (partyCritical) return CombatLastStand;
            if (hasRatfolk) return CombatRatfolk;
            if (style.Contains("sewer") || style.Contains("rat") || style.Contains("cistern")) return CombatSewer;

            switch (faction)
            {
                case "demon": return CombatDemon;
                case "drow": return CombatDrow;
                case "kobold": return CombatKobold;
                case "undead": return CombatUndead;
                case "rat": return CombatSewer;
            }

            if (hasElite) return CombatElite;
            if (hasCaster) return CombatArcaneDuel;
            return CombatGeneric;
        }

        public static bool IsCriticalPartyHealth(int currentHp, int maximumHp)
        {
            return maximumHp > 0 && currentHp > 0 && currentHp <= maximumHp * 0.35f;
        }

        public static bool IsMusicLandmark(ObjectType type)
        {
            return IsTempleLandmark(type)
                || IsMarketLandmark(type)
                || IsTavernLandmark(type)
                || IsGateLandmark(type)
                || IsRoyalLandmark(type)
                || type == ObjectType.Sewer
                || type == ObjectType.RatPeltQuest
                || type == ObjectType.Camp
                || type == ObjectType.Waystone
                || type == ObjectType.Shrine
                || type == ObjectType.AncientGrove
                || type == ObjectType.Cave
                || type == ObjectType.Stairs
                || type == ObjectType.DungeonGate
                || type == ObjectType.Ruin
                || type == ObjectType.DeepCrypt
                || type == ObjectType.Obelisk
                || type == ObjectType.LoreLibrary
                || type == ObjectType.PortalSeal
                || type == ObjectType.FactionCamp
                || type == ObjectType.Encounter;
        }

        public static int LandmarkPriority(ObjectType type)
        {
            if (IsRoyalLandmark(type)) return 90;
            if (type == ObjectType.Sewer || type == ObjectType.RatPeltQuest) return 85;
            if (IsTempleLandmark(type) || IsMarketLandmark(type) || IsTavernLandmark(type)) return 80;
            if (IsGateLandmark(type)) return 70;
            if (type == ObjectType.PortalSeal || type == ObjectType.DeepCrypt || type == ObjectType.DungeonGate) return 65;
            if (type == ObjectType.AncientGrove || type == ObjectType.LoreLibrary || type == ObjectType.FactionCamp) return 60;
            return IsMusicLandmark(type) ? 50 : 0;
        }

        private static bool IsBossStyle(string style)
        {
            return style.Contains("king")
                || style.Contains("boss")
                || style.Contains("final")
                || style.Contains("crown");
        }

        private static bool IsTempleLandmark(ObjectType type)
        {
            return type == ObjectType.Temple
                || type == ObjectType.TempleHealer
                || type == ObjectType.NoviceHealer
                || type == ObjectType.Fountain
                || type == ObjectType.RecallCircle;
        }

        private static bool IsMarketLandmark(ObjectType type)
        {
            return type == ObjectType.Market
                || type == ObjectType.MarketClerk
                || type == ObjectType.Armorer
                || type == ObjectType.WeaponVendor
                || type == ObjectType.Enchanter
                || type == ObjectType.Provisions
                || type == ObjectType.Provisioner
                || type == ObjectType.ArmorerNpc
                || type == ObjectType.WeaponMerchantNpc
                || type == ObjectType.EnchanterNpc;
        }

        private static bool IsTavernLandmark(ObjectType type)
        {
            return type == ObjectType.Tavern
                || type == ObjectType.TavernKeeper
                || type == ObjectType.Diner
                || type == ObjectType.DinerCook;
        }

        private static bool IsGateLandmark(ObjectType type)
        {
            return type == ObjectType.EastGate
                || type == ObjectType.WestGate
                || type == ObjectType.NorthGate
                || type == ObjectType.SouthGate
                || type == ObjectType.TownGuard
                || type == ObjectType.GateCaptain
                || type == ObjectType.DockWorker;
        }

        private static bool IsRoyalLandmark(ObjectType type)
        {
            return type == ObjectType.KingHall
                || type == ObjectType.KingHalvard
                || type == ObjectType.RoyalHerald
                || type == ObjectType.Scholar;
        }
    }
}
