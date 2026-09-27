using System;
using System.Collections.Generic;

namespace AshenHalls
{
    // Optional progress lives entirely in the existing serialized story flags.
    // Recovering the roll requires an explicit visit; clearing a room alone never recovers it.
    public static class RemembranceQuestRules
    {
        public const string Title = "Names Beneath the Water";
        public const int ExperienceReward = 8;

        public static bool Has(ICollection<string> flags, string flag) => flags != null && flags.Contains(flag);
        public static bool Accepted(ICollection<string> flags) => Has(flags, StoryFlags.RemembranceAccepted);
        public static bool Recovered(ICollection<string> flags) => Has(flags, StoryFlags.RemembranceLedgerRecovered);
        public static bool NamesRead(ICollection<string> flags) => Has(flags, StoryFlags.RemembranceNamesRead);
        public static bool Delivered(ICollection<string> flags) => Has(flags, StoryFlags.RemembranceDelivered);
        public static bool RewardClaimed(ICollection<string> flags) => Has(flags, StoryFlags.RemembranceRewardClaimed);
        public static bool HasDisposition(ICollection<string> flags) => Has(flags, StoryFlags.RemembrancePrivateChosen)
            || Has(flags, StoryFlags.RemembrancePublicChosen);

        public static bool AlcoveAccessible(ICollection<string> flags)
        {
            return Has(flags, StoryFlags.SewerFoulRunoffCleared)
                || Has(flags, StoryFlags.SewerCisternDenCleared)
                || Has(flags, StoryFlags.MidgaardRatPeltsCollected)
                || Has(flags, StoryFlags.MidgaardRatPeltArmorMade)
                || Has(flags, StoryFlags.SewerRewardClaimed);
        }

        public static bool TryAccept(ICollection<string> flags)
        {
            return flags != null && !Accepted(flags) && Add(flags, StoryFlags.RemembranceAccepted);
        }

        public static bool TryRecover(ICollection<string> flags)
        {
            return Accepted(flags) && AlcoveAccessible(flags) && !Recovered(flags)
                && Add(flags, StoryFlags.RemembranceLedgerRecovered);
        }

        public static bool TryReadNames(ICollection<string> flags)
        {
            return Accepted(flags) && Recovered(flags) && !NamesRead(flags)
                && Add(flags, StoryFlags.RemembranceNamesRead);
        }

        public static bool TryChooseDisposition(ICollection<string> flags, string choice)
        {
            if (!Accepted(flags) || !NamesRead(flags) || HasDisposition(flags)) return false;
            if (choice == "private") return Add(flags, StoryFlags.RemembrancePrivateChosen);
            if (choice == "public") return Add(flags, StoryFlags.RemembrancePublicChosen);
            return false;
        }

        public static bool TryDeliver(ICollection<string> flags, ObjectType contact)
        {
            if (!Accepted(flags) || !NamesRead(flags) || Delivered(flags)) return false;
            bool correctContact = contact == ObjectType.NoviceHealer && Has(flags, StoryFlags.RemembrancePrivateChosen)
                || contact == ObjectType.GateCaptain && Has(flags, StoryFlags.RemembrancePublicChosen);
            return correctContact && Add(flags, StoryFlags.RemembranceDelivered);
        }

        public static bool TryClaimReward(ICollection<string> flags, string choice)
        {
            if (!Delivered(flags) || RewardClaimed(flags) || (choice != "bow" && choice != "wrap")) return false;
            Add(flags, choice == "bow" ? StoryFlags.RemembranceBowChosen : StoryFlags.RemembranceWrapChosen);
            return Add(flags, StoryFlags.RemembranceRewardClaimed);
        }

        public static string Objective(ICollection<string> flags)
        {
            if (!Accepted(flags)) return "Edda, the wounded traveler west of the tavern, has a favor to ask about her lost companions.";
            if (RewardClaimed(flags)) return Has(flags, StoryFlags.RemembrancePrivateChosen)
                ? "The four names rest in the temple book. Edda has a quiet place to remember them."
                : "Brann has posted the four names and Edda's account. New parties can learn from their last stand.";
            if (Delivered(flags)) return "Choose one keepsake from Sera or Brann: Traveler's Knot bow or Vigilkeeper Wrap.";
            if (Has(flags, StoryFlags.RemembrancePrivateChosen)) return "Take the four names to Sera at Temple Square for the private memorial.";
            if (Has(flags, StoryFlags.RemembrancePublicChosen)) return "Bring Edda's account to Gate Captain Brann at West Gate for the public warning.";
            if (NamesRead(flags)) return "Return to Edda and decide together how her companions will be remembered.";
            if (Recovered(flags)) return "Ask Sera, the novice healer at Temple Square, to read the water-damaged roll.";
            return AlcoveAccessible(flags)
                ? "Return to the Midgaard Sewer entrance and inspect the cleared maintenance alcove for Edda's roll."
                : "Clear Foul Runoff, then return to the Midgaard Sewer entrance to inspect the maintenance alcove.";
        }

        private static bool Add(ICollection<string> flags, string flag)
        {
            if (flags == null || flags.Contains(flag)) return false;
            flags.Add(flag);
            return true;
        }
    }
}
