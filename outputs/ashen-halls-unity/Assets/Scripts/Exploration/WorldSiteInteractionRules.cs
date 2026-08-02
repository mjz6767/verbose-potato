using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AshenHalls
{
    public enum WorldSiteRewardKind
    {
        TrainingInsight,
        QuarryMail,
        CryptTithe,
        GlassFormula,
        MarketCache,
        SealEmber,
        CisternStores,
        GroveTonic
    }

    public sealed class WorldSiteInteractionProfile
    {
        public readonly string SiteId;
        public readonly string ServiceName;
        public readonly string ReadyVerb;
        public readonly string RepeatVerb;
        public readonly string ReadyStatus;
        public readonly string ClaimedStatus;
        public readonly string RewardSummary;
        public readonly string RepeatSummary;
        public readonly WorldSiteRewardKind RewardKind;
        public readonly int RepeatSupplyCost;
        public readonly int RepeatGoldCost;
        public readonly int RepeatHealing;
        public readonly int RepeatMana;
        public readonly int RepeatSupplies;

        public WorldSiteInteractionProfile(
            string siteId,
            string serviceName,
            string readyVerb,
            string repeatVerb,
            string readyStatus,
            string claimedStatus,
            string rewardSummary,
            string repeatSummary,
            WorldSiteRewardKind rewardKind,
            int repeatSupplyCost = 0,
            int repeatGoldCost = 0,
            int repeatHealing = 0,
            int repeatMana = 0,
            int repeatSupplies = 0)
        {
            SiteId = siteId ?? "";
            ServiceName = serviceName ?? "";
            ReadyVerb = readyVerb ?? "Use";
            RepeatVerb = repeatVerb ?? "Use";
            ReadyStatus = readyStatus ?? "";
            ClaimedStatus = claimedStatus ?? "";
            RewardSummary = rewardSummary ?? "";
            RepeatSummary = repeatSummary ?? "";
            RewardKind = rewardKind;
            RepeatSupplyCost = Math.Max(0, repeatSupplyCost);
            RepeatGoldCost = Math.Max(0, repeatGoldCost);
            RepeatHealing = Math.Max(0, repeatHealing);
            RepeatMana = Math.Max(0, repeatMana);
            RepeatSupplies = Math.Max(0, repeatSupplies);
        }

        public bool IsInformationalRepeat => RepeatSupplyCost == 0
            && RepeatGoldCost == 0
            && RepeatHealing == 0
            && RepeatMana == 0
            && RepeatSupplies == 0;

        public bool RequiresExplicitRepeatUse => !IsInformationalRepeat;
    }

    public static class WorldSiteInteractionRules
    {
        public const string ChartFlagPrefix = "regional_site_";
        public const string RewardFlagPrefix = "world_site_reward_";

        private static readonly WorldSiteInteractionProfile[] Profiles =
        {
            new WorldSiteInteractionProfile(
                "green-shrine-training-ring",
                "Pilgrim Shield Drill",
                "Train",
                "Rest",
                "The moss ring is ready for one formal lesson on this road depth.",
                "The formal lesson is recorded; guarded practice remains available.",
                "The least-trained living hero gains 1 skill point.",
                "Spend 1 supply to restore 6 health and 3 mana to each living hero.",
                WorldSiteRewardKind.TrainingInsight,
                repeatSupplyCost: 1,
                repeatHealing: 6,
                repeatMana: 3),
            new WorldSiteInteractionProfile(
                "old-quarry-forge",
                "Cold-Forge Appraisal",
                "Claim",
                "Inspect",
                "One sound worked-iron harness remains beneath the quarry scale.",
                "The harness is claimed; the cold anvil still identifies damaged road gear.",
                "Recover one +2 worked-iron quarry mail for the pack.",
                "The anvil confirms the party's gear is serviceable; no materials are consumed.",
                WorldSiteRewardKind.QuarryMail),
            new WorldSiteInteractionProfile(
                "gloam-deep-crypt",
                "Reliquary Vigil",
                "Claim",
                "Rest",
                "An unopened funerary tithe rests behind the central ward.",
                "The tithe is recorded; ward candles can still steady tired spellcasters.",
                "Recover a small depth-scaled funerary tithe.",
                "Spend 1 supply on ward candles to restore 5 mana to each living hero.",
                WorldSiteRewardKind.CryptTithe,
                repeatSupplyCost: 1,
                repeatMana: 5),
            new WorldSiteInteractionProfile(
                "glass-lore-library",
                "Mirror-Formula Study",
                "Study",
                "Inspect",
                "One intact formula can still be translated from the mirror stacks.",
                "The intact formula is copied; the remaining shelves offer tactical notes only.",
                "The least-trained living spellcaster gains 1 skill point.",
                "Review the reflected-path notes without changing party resources.",
                WorldSiteRewardKind.GlassFormula),
            new WorldSiteInteractionProfile(
                "dusk-market-hideout",
                "Scout Cache and Barter",
                "Loot",
                "Trade",
                "A concealed scout cache remains beneath the collapsed west stall.",
                "The scout cache is claimed; the fence still trades one supply for 12 gold.",
                "Recover 2 supplies from the hideout cache.",
                "Trade 12 gold for 1 supply.",
                WorldSiteRewardKind.MarketCache,
                repeatGoldCost: 12,
                repeatSupplies: 1),
            new WorldSiteInteractionProfile(
                "red-gate-seal",
                "Basalt Seal Reading",
                "Study",
                "Inspect",
                "A loose ward ember can be lifted without opening the gate.",
                "The loose ember is secured; the remaining sigils only report the seal's condition.",
                "Recover 1 elixir condensed around the ward ember.",
                "Read the stable outer sigils without disturbing the chapter lock.",
                WorldSiteRewardKind.SealEmber),
            new WorldSiteInteractionProfile(
                "salt-cistern-gate",
                "Sluice Stores",
                "Loot",
                "Rest",
                "A sealed maintenance locker remains above the flood line.",
                "The locker is empty; the dry ledge remains usable for a supplied rest.",
                "Recover 1 supply and 6 gold from the maintenance locker.",
                "Spend 1 supply to restore 8 health and 2 mana to each living hero.",
                WorldSiteRewardKind.CisternStores,
                repeatSupplyCost: 1,
                repeatHealing: 8,
                repeatMana: 2),
            new WorldSiteInteractionProfile(
                "ash-fen-ancient-grove",
                "Dry-Island Infusion",
                "Gather",
                "Rest",
                "One clean tonic has gathered in the grove's dry stone basin.",
                "The tonic is bottled; the dry island remains a supplied recovery point.",
                "Bottle 1 elixir and restore 8 health and 5 mana to each living hero.",
                "Spend 1 supply to restore 4 health and 5 mana to each living hero.",
                WorldSiteRewardKind.GroveTonic,
                repeatSupplyCost: 1,
                repeatHealing: 4,
                repeatMana: 5)
        };

        private static readonly ReadOnlyCollection<WorldSiteInteractionProfile> ReadOnlyProfiles =
            new ReadOnlyCollection<WorldSiteInteractionProfile>(Profiles);
        private static readonly Dictionary<string, WorldSiteInteractionProfile> ProfilesBySite = BuildLookup();

        public static IReadOnlyList<WorldSiteInteractionProfile> All => ReadOnlyProfiles;

        public static bool TryGet(string siteId, out WorldSiteInteractionProfile profile)
        {
            return ProfilesBySite.TryGetValue(siteId ?? "", out profile);
        }

        public static string RewardFlag(int depth, string siteId)
        {
            return RewardFlagPrefix + Math.Max(1, depth) + "_" + Sanitize(siteId) + "_claimed";
        }

        public static string ChartFlag(int depth, string siteId)
        {
            return ChartFlagPrefix + Math.Max(1, depth) + "_" + Sanitize(siteId) + "_charted";
        }

        public static bool RewardClaimed(
            IReadOnlyCollection<string> flags,
            int depth,
            string siteId)
        {
            if (flags == null) return false;
            string expected = RewardFlag(depth, siteId);
            foreach (string flag in flags)
            {
                if (string.Equals(flag, expected, StringComparison.Ordinal)) return true;
            }
            return false;
        }

        public static string Status(WorldSiteInteractionProfile profile, bool rewardClaimed)
        {
            if (profile == null) return "No regional service is recorded here.";
            return rewardClaimed ? profile.ClaimedStatus : profile.ReadyStatus;
        }

        public static string ContextVerb(WorldSiteInteractionProfile profile, bool rewardClaimed)
        {
            if (profile == null) return "Use";
            return rewardClaimed ? profile.RepeatVerb : profile.ReadyVerb;
        }

        private static Dictionary<string, WorldSiteInteractionProfile> BuildLookup()
        {
            Dictionary<string, WorldSiteInteractionProfile> lookup =
                new Dictionary<string, WorldSiteInteractionProfile>(StringComparer.Ordinal);
            foreach (WorldSiteInteractionProfile profile in Profiles)
            {
                lookup.Add(profile.SiteId, profile);
            }
            return lookup;
        }

        private static string Sanitize(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "unknown_site";
            StringBuilder result = new StringBuilder(value.Length);
            bool previousSeparator = false;
            foreach (char raw in value.Trim().ToLowerInvariant())
            {
                if (char.IsLetterOrDigit(raw))
                {
                    result.Append(raw);
                    previousSeparator = false;
                }
                else if (!previousSeparator && result.Length > 0)
                {
                    result.Append('_');
                    previousSeparator = true;
                }
            }
            return result.ToString().Trim('_');
        }
    }
}
