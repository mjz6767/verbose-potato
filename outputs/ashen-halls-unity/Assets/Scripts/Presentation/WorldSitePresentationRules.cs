using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AshenHalls
{
    public readonly struct WorldSitePresentationProfile
    {
        public readonly string SiteId;
        public readonly string ZoneId;
        public readonly ObjectType LandmarkType;
        public readonly string PrimaryAmbientCue;
        public readonly string SecondaryAmbientCue;
        public readonly string MusicKey;
        public readonly string InspectCue;

        public WorldSitePresentationProfile(
            string siteId,
            string zoneId,
            ObjectType landmarkType,
            string primaryAmbientCue,
            string secondaryAmbientCue,
            string musicKey,
            string inspectCue)
        {
            SiteId = siteId ?? "";
            ZoneId = zoneId ?? "";
            LandmarkType = landmarkType;
            PrimaryAmbientCue = primaryAmbientCue ?? "";
            SecondaryAmbientCue = secondaryAmbientCue ?? "";
            MusicKey = musicKey ?? "";
            InspectCue = inspectCue ?? "ui";
        }

        public string AmbientCueFor(int sequence)
        {
            return (sequence & 1) == 0 ? PrimaryAmbientCue : SecondaryAmbientCue;
        }

        public bool UsesAmbientCue(string cue)
        {
            return string.Equals(cue, PrimaryAmbientCue, StringComparison.Ordinal)
                || string.Equals(cue, SecondaryAmbientCue, StringComparison.Ordinal);
        }
    }

    public static class WorldSitePresentationRules
    {
        public const string LandmarkObjectIdPrefix = "regional-site:";
        public const string DecorationObjectIdPrefix = "regional-site-decor:";

        public const string GreenShrineTrainingRing = "green-shrine-training-ring";
        public const string OldQuarryForge = "old-quarry-forge";
        public const string GloamDeepCrypt = "gloam-deep-crypt";
        public const string GlassLoreLibrary = "glass-lore-library";
        public const string DuskMarketHideout = "dusk-market-hideout";
        public const string RedGateSeal = "red-gate-seal";
        public const string SaltCisternGate = "salt-cistern-gate";
        public const string AshFenAncientGrove = "ash-fen-ancient-grove";

        private static readonly WorldSitePresentationProfile[] Profiles =
        {
            Profile(
                GreenShrineTrainingRing,
                "green-shrine-road",
                ObjectType.TrainingGround,
                "ambforge",
                "ambgrove",
                MusicDirectorRules.GreenShrineTrainingRing,
                "guard"),
            Profile(
                OldQuarryForge,
                "old-quarry",
                ObjectType.ForgeSite,
                "ambforge",
                "ambstone",
                MusicDirectorRules.OldQuarryForge,
                "servicearmor"),
            Profile(
                GloamDeepCrypt,
                "gloam-courts",
                ObjectType.DeepCrypt,
                "ambruin",
                "ambcave",
                MusicDirectorRules.GloamDeepCrypt,
                "door"),
            Profile(
                GlassLoreLibrary,
                "glass-warrens",
                ObjectType.LoreLibrary,
                "ambglass",
                "ambruin",
                MusicDirectorRules.GlassLoreLibrary,
                "formula"),
            Profile(
                DuskMarketHideout,
                "dusk-market",
                ObjectType.FactionCamp,
                "ambdrum",
                "ambcamp",
                MusicDirectorRules.DuskMarketHideout,
                "ambush"),
            Profile(
                RedGateSeal,
                "red-gate",
                ObjectType.PortalSeal,
                "ambgate",
                "ambglass",
                MusicDirectorRules.RedGateSeal,
                "riftseal"),
            Profile(
                SaltCisternGate,
                "salt-cisterns",
                ObjectType.DungeonGate,
                "ambdrip",
                "ambcave",
                MusicDirectorRules.SaltCisternGate,
                "gateopen"),
            Profile(
                AshFenAncientGrove,
                "ash-fen",
                ObjectType.AncientGrove,
                "ambgrove",
                "ambfen",
                MusicDirectorRules.AshFenAncientGrove,
                "castnature")
        };

        private static readonly ReadOnlyCollection<WorldSitePresentationProfile> ReadOnlyProfiles =
            new ReadOnlyCollection<WorldSitePresentationProfile>(Profiles);
        private static readonly Dictionary<string, WorldSitePresentationProfile> ProfilesById = BuildLookup();

        public static IReadOnlyList<WorldSitePresentationProfile> All => ReadOnlyProfiles;

        public static bool TryGet(string siteId, out WorldSitePresentationProfile profile)
        {
            return ProfilesById.TryGetValue((siteId ?? "").Trim(), out profile);
        }

        public static bool TryGetForLandmarkObjectId(
            string objectId,
            out WorldSitePresentationProfile profile)
        {
            profile = default;
            if (string.IsNullOrEmpty(objectId)
                || IsDecorationObjectId(objectId)
                || !objectId.StartsWith(LandmarkObjectIdPrefix, StringComparison.Ordinal))
            {
                return false;
            }

            return TryGet(objectId.Substring(LandmarkObjectIdPrefix.Length), out profile);
        }

        public static bool IsDecorationObjectId(string objectId)
        {
            return !string.IsNullOrEmpty(objectId)
                && objectId.StartsWith(DecorationObjectIdPrefix, StringComparison.Ordinal);
        }

        public static string ExploreMusicKey(string siteId, string zoneId, bool threatAlerted)
        {
            string zone = (zoneId ?? "").Trim().ToLowerInvariant();
            if (threatAlerted)
            {
                return MusicDirectorRules.ExploreTrackKey(zone, default, false, true);
            }

            if (TryGet(siteId, out WorldSitePresentationProfile profile)) return profile.MusicKey;
            return string.IsNullOrEmpty(zone) ? "road" : zone;
        }

        public static string InspectCueFor(string siteId)
        {
            return TryGet(siteId, out WorldSitePresentationProfile profile)
                ? profile.InspectCue
                : "ui";
        }

        private static Dictionary<string, WorldSitePresentationProfile> BuildLookup()
        {
            Dictionary<string, WorldSitePresentationProfile> lookup =
                new Dictionary<string, WorldSitePresentationProfile>(StringComparer.Ordinal);
            foreach (WorldSitePresentationProfile profile in Profiles)
            {
                lookup.Add(profile.SiteId, profile);
            }
            return lookup;
        }

        private static WorldSitePresentationProfile Profile(
            string siteId,
            string zoneId,
            ObjectType landmarkType,
            string primaryAmbientCue,
            string secondaryAmbientCue,
            string musicKey,
            string inspectCue)
        {
            return new WorldSitePresentationProfile(
                siteId,
                zoneId,
                landmarkType,
                primaryAmbientCue,
                secondaryAmbientCue,
                musicKey,
                inspectCue);
        }
    }
}
