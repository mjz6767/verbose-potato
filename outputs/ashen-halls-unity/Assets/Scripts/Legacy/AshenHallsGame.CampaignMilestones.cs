using System.Linq;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private void ApplyCampaignMilestoneRewards()
        {
            if (state == null || !ContentSetCatalog.IsSewerSlice(activeContentSet) || betaLabMode
                || (state.Mode != GameMode.Explore && state.Mode != GameMode.Victory)
                || state.Party == null || !state.Party.Any(member => member != null && member.Hp > 0)) return;

            foreach (CampaignMilestone milestone in CampaignMilestoneRules.Pending(state.StoryFlags).ToArray())
            {
                AwardWorldExperience(milestone.Experience, milestone.Name);
                SetStoryFlag(milestone.RewardFlag);
            }
        }
    }
}
