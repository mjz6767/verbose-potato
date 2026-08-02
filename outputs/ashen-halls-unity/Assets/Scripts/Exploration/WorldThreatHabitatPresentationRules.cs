using System;

namespace AshenHalls
{
    public static class WorldThreatHabitatPresentationRules
    {
        public const int Columns = 4;
        public const int Rows = 2;
        public const int CellCount = Columns * Rows;
        public const bool PreserveScaleAtViewportEdge = true;

        public const int RatWarrenIndex = 0;
        public const int PlagueBellMiddenIndex = 1;
        public const int KoboldAmbushCampIndex = 2;
        public const int KoboldShamanTotemYardIndex = 3;
        public const int DrowMoonSilkWatchpostIndex = 4;
        public const int UndeadOssuaryIndex = 5;
        public const int DemonBreachIndex = 6;
        public const int RuinedRoadWaystationIndex = 7;

        // Habitat art is a stationary presentation layer. The roaming token and
        // the threat's persisted HomeX/HomeY remain the authoritative state.
        public const bool DrawsBeneathRoamingThreatToken = true;
        public const float BottomCenterPivotX = 0.5f;
        // GUI atlas fitting uses a top-left destination origin, so 1 anchors the
        // shorter fitted dimension to the bottom edge of its destination box.
        public const float BottomCenterPivotY = 1f;

        public static int AtlasIndex(RoamingThreatDefinition definition)
        {
            return definition == null
                ? RuinedRoadWaystationIndex
                : AtlasIndex(definition.Archetype, definition.Faction);
        }

        public static int PresentationIndex(
            bool active,
            RoamingThreatDefinition definition,
            string fallbackArchetype)
        {
            if (!active) return RuinedRoadWaystationIndex;

            int index = definition != null
                ? AtlasIndex(definition)
                : ArchetypeIndex(fallbackArchetype);
            return index >= 0 ? index : RuinedRoadWaystationIndex;
        }

        public static int AtlasIndex(string archetype, RoamingThreatFaction faction)
        {
            int archetypeIndex = ArchetypeIndex(archetype);
            return archetypeIndex >= 0 ? archetypeIndex : FactionIndex(faction);
        }

        public static int ArchetypeIndex(string archetype)
        {
            switch (NormalizeKey(archetype))
            {
                case "rat":
                case "rats":
                case "ratfolk":
                case "ratbrute":
                case "ratswarm":
                case "ratcaptain":
                    return RatWarrenIndex;
                case "plaguerat":
                case "plaguerats":
                case "ratcleric":
                case "ratmage":
                case "plaguebell":
                case "plaguebellmidden":
                    return PlagueBellMiddenIndex;
                case "kobold":
                case "kobolds":
                case "koboldraider":
                case "koboldshield":
                case "koboldslinger":
                case "koboldking":
                    return KoboldAmbushCampIndex;
                case "koboldshaman":
                case "koboldwizard":
                    return KoboldShamanTotemYardIndex;
                case "drow":
                case "drowscout":
                case "drowmage":
                    return DrowMoonSilkWatchpostIndex;
                case "undead":
                case "reaver":
                case "bonepriest":
                case "revenant":
                    return UndeadOssuaryIndex;
                case "demon":
                case "demons":
                case "imp":
                case "boundimp":
                case "lesserdemon":
                case "greaterdemon":
                case "redgatedemon":
                    return DemonBreachIndex;
                case "aftermath":
                case "neutral":
                case "waystation":
                case "ruinedroadwaystation":
                    return RuinedRoadWaystationIndex;
                default:
                    return -1;
            }
        }

        public static int FactionIndex(RoamingThreatFaction faction)
        {
            switch (faction)
            {
                case RoamingThreatFaction.Rats: return RatWarrenIndex;
                case RoamingThreatFaction.Kobolds: return KoboldAmbushCampIndex;
                case RoamingThreatFaction.Drow: return DrowMoonSilkWatchpostIndex;
                case RoamingThreatFaction.Undead: return UndeadOssuaryIndex;
                case RoamingThreatFaction.Demons: return DemonBreachIndex;
                default: return RuinedRoadWaystationIndex;
            }
        }

        public static float MapScale(bool wideView)
        {
            return MapScale(wideView, true);
        }

        public static float MapScale(bool wideView, bool active)
        {
            return MapScale(wideView, active, active);
        }

        public static float MapScale(bool wideView, bool active, bool homeOccupied)
        {
            // Active lairs remain prominent tactical landmarks. Once cleared, the
            // aftermath sits closer to its one-cell footprint so it reads as
            // walkable history rather than a still-occupied encounter.
            if (active && homeOccupied) return wideView ? 1.35f : 1.65f;
            if (active) return wideView ? 1.12f : 1.30f;
            return wideView ? 1.10f : 1.25f;
        }

        public static float HabitatAlpha(bool onObjectiveRoute, bool active, bool homeOccupied)
        {
            if (active && homeOccupied) return onObjectiveRoute ? 1f : 0.78f;
            if (active) return onObjectiveRoute ? 0.60f : 0.46f;
            return 0.52f;
        }

        public static float TintAlpha(bool onObjectiveRoute)
        {
            return onObjectiveRoute ? 1f : 0.62f;
        }

        public static bool ShouldDrawAtHome(bool homeInBounds, bool certifiedSafeRoad)
        {
            return homeInBounds && !certifiedSafeRoad;
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
