using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public static class HonestWeightQuestRules
    {
        public const string Title = "The Honest Weight";
        public const int QuestDepth = 3;
        public const int ExperienceReward = 120;
        public const int BrokenGoldReward = 30;
        public const int OrdinaryMarketTradeCost = 12;
        public const int RetunedMarketTradeCost = 8;
        public const string MarketSite = "dusk-market-hideout";
        public const string QuarrySite = "old-quarry-forge";

        public static bool Available(IReadOnlyCollection<string> flags) => Has(flags, StoryFlags.KoboldKingDefeated);
        public static bool Accepted(IReadOnlyCollection<string> flags) => Has(flags, StoryFlags.HonestWeightAccepted);
        public static bool ContractRead(IReadOnlyCollection<string> flags) => Has(flags, StoryFlags.HonestWeightContractRead);
        public static bool CalibrationRead(IReadOnlyCollection<string> flags) => Has(flags, StoryFlags.HonestWeightCalibrationRead);
        public static bool EvidenceReady(IReadOnlyCollection<string> flags) => Accepted(flags) && ContractRead(flags) && CalibrationRead(flags);
        public static bool Retuned(IReadOnlyCollection<string> flags) => Has(flags, StoryFlags.HonestWeightRetuned);
        public static bool Complete(IReadOnlyCollection<string> flags) => Retuned(flags) || Has(flags, StoryFlags.HonestWeightBroken);
        public static bool RewardClaimed(IReadOnlyCollection<string> flags) => Has(flags, StoryFlags.HonestWeightRewardClaimed);
        public static int MarketTradeCost(IReadOnlyCollection<string> flags) => Retuned(flags) ? RetunedMarketTradeCost : OrdinaryMarketTradeCost;

        public static bool TryAccept(List<string> flags)
        {
            if (flags == null || !Available(flags) || Accepted(flags)) return false;
            flags.Add(StoryFlags.HonestWeightAccepted);
            return true;
        }

        public static bool CanInspect(IReadOnlyCollection<string> flags, int depth, string siteId)
        {
            if (!Accepted(flags) || Complete(flags) || depth != QuestDepth) return false;
            return siteId == MarketSite && !ContractRead(flags) || siteId == QuarrySite && !CalibrationRead(flags);
        }

        public static bool TryInspect(List<string> flags, int depth, string siteId)
        {
            if (!CanInspect(flags, depth, siteId)) return false;
            flags.Add(siteId == MarketSite ? StoryFlags.HonestWeightContractRead : StoryFlags.HonestWeightCalibrationRead);
            return true;
        }

        public static bool TryResolve(List<string> flags, string disposition)
        {
            if (!EvidenceReady(flags) || Complete(flags) || (disposition != "break" && disposition != "retune")) return false;
            flags.Add(disposition == "break" ? StoryFlags.HonestWeightBroken : StoryFlags.HonestWeightRetuned);
            return true;
        }

        public static bool TryClaimReward(List<string> flags)
        {
            if (!Complete(flags) || RewardClaimed(flags)) return false;
            flags.Add(StoryFlags.HonestWeightRewardClaimed);
            return true;
        }

        public static string Objective(IReadOnlyCollection<string> flags)
        {
            if (!Accepted(flags)) return "Maud has heard that one of her old weigh-lamps is cheating the Bone Road traders. Ask at her rune anvil after Varkh falls.";
            if (RewardClaimed(flags)) return Retuned(flags)
                ? "Maud rewrote the lamp as a public true measure. Dusk Market repeat supply trades cost 8 gold, at every chapter depth."
                : "Maud broke the cheating charm. The company received its 30-gold deposit and one counterweighted tool.";
            if (Complete(flags)) return "The lamp is settled. Return to Maud and choose one of her two counterweighted tools.";
            if (EvidenceReady(flags)) return "Return to Maud. Choose to break the charm for 30 gold or retune it for 8-gold Dusk Market supply trades; either choice also awards 120 base XP and one tool.";
            if (!ContractRead(flags) && !CalibrationRead(flags)) return "On the Bone Road (Chapter III), inspect the Dusk Market weigh-lamp and the Old Quarry Forge maker's scale. Either order; earlier visits do not count as these particular inspections.";
            return !ContractRead(flags)
                ? "At Chapter III's Dusk Market, inspect the brass weigh-lamp's contract. Maud already has the quarry calibration."
                : "At Chapter III's Old Quarry Forge, inspect the maker's scale for the missing calibration clause.";
        }

        private static bool Has(IReadOnlyCollection<string> flags, string flag)
        {
            if (flags == null) return false;
            foreach (string value in flags) if (string.Equals(value, flag, StringComparison.Ordinal)) return true;
            return false;
        }
    }
}
