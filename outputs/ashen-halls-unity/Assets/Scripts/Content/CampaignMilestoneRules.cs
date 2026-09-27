using System.Collections.Generic;
using System.Linq;

namespace AshenHalls
{
    public sealed class CampaignMilestone
    {
        public string CompletedFlag { get; }
        public string RewardFlag { get; }
        public string Name { get; }
        public int Experience { get; }

        public CampaignMilestone(string completedFlag, string name, int experience)
        {
            CompletedFlag = completedFlag;
            RewardFlag = "campaign-milestone-xp:" + completedFlag;
            Name = name;
            Experience = experience;
        }
    }

    public static class CampaignMilestoneRules
    {
        // Separate receipts let older saves receive earned chapter rewards once.
        public static readonly IReadOnlyList<CampaignMilestone> All = new[]
        {
            new CampaignMilestone(StoryFlags.SewerRewardClaimed, "Cistern contract complete", 100),
            new CampaignMilestone(StoryFlags.KoboldKingDefeated, "Kobold King defeated", 200),
            new CampaignMilestone(StoryFlags.RedGateWarningRecovered, "Bone Road secured", 350),
            new CampaignMilestone(StoryFlags.EmberglassGateKeyRecovered, "Glass Road secured", 500),
            new CampaignMilestone(StoryFlags.RedGateDebriefed, "Red Gate report delivered", 650),
            new CampaignMilestone(StoryFlags.MeteorCrownAssaultAccepted, "Last Dawn preparations", 600),
            new CampaignMilestone(StoryFlags.MeteorCrownHeartBroken, "Crown tether severed", 250)
        };

        public static IEnumerable<CampaignMilestone> Pending(IReadOnlyCollection<string> flags)
        {
            return flags == null ? Enumerable.Empty<CampaignMilestone>()
                : All.Where(milestone => flags.Contains(milestone.CompletedFlag) && !flags.Contains(milestone.RewardFlag));
        }
    }
}
