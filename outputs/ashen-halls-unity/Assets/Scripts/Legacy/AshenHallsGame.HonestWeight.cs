using System;
using System.Collections.Generic;
using UnityEngine;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private bool honestWeightServiceBypass;
        private string honestWeightInspectSiteId = "";

        private DialogueChoiceView[] AddHonestWeightChoice(DialogueChoiceView[] choices)
        {
            if (!HonestWeightQuestRules.Available(state?.StoryFlags) && !HonestWeightQuestRules.Accepted(state?.StoryFlags)) return choices;
            // The native dialogue owns four buttons. Keep both services at the root
            // and move the two conversation topics together when the favor appears.
            List<DialogueChoiceView> expanded = new List<DialogueChoiceView>();
            foreach (DialogueChoiceView choice in choices ?? Array.Empty<DialogueChoiceView>())
                if (choice != null && choice.Id != "affinity" && choice.Id != "personal") expanded.Add(choice);
            expanded.Add(MakeDialogueChoice("honestweight", HonestWeightQuestRules.Accepted(state.StoryFlags)
                ? "About the brass weigh-lamp..." : "You look as though a spell has annoyed you.", "Optional investigation on the Bone Road."));
            expanded.Add(MakeDialogueChoice("craft", "About your craft...", "The four runes and Maud's first enchantment."));
            return expanded.ToArray();
        }

        private bool TryResolveHonestWeightMaudChoice(string choice)
        {
            if (choice != "honestweight") return false;
            ShowHonestWeightConversation();
            return true;
        }

        private string HonestWeightMaudGreeting(string fallback)
        {
            if (!HonestWeightQuestRules.Complete(state?.StoryFlags)) return fallback;
            return HonestWeightQuestRules.Retuned(state.StoryFlags)
                ? "The market sent its first honest account. Three arguments about onion prices, none about the scale. A modest triumph. What needs a mark today?"
                : "I hung the empty lamp over my workbench. It gives excellent light and no advice. I find I prefer it. What needs a mark today?";
        }

        private void ShowHonestWeightConversation()
        {
            if (state == null || state.Mode != GameMode.Explore) return;
            List<DialogueChoiceView> choices = new List<DialogueChoiceView>();
            string body;
            if (!HonestWeightQuestRules.Accepted(state.StoryFlags))
            {
                body = "An old weigh-lamp of mine has turned up in the Bone Road's Dusk Market. The traders say every sack comes up short. I suspect the lamp is obeying a very bad sentence. If you pass that way, copy its contract and the maker's calibration at the Old Quarry Forge. Both are in Chapter III. Take your time; no one needs to bleed over a sack of onions.";
                if (HonestWeightQuestRules.Available(state.StoryFlags)) choices.Add(MakePrimaryDialogueChoice("accept", "We'll look at the two inscriptions.", "Chapter III: market and quarry, in either order."));
            }
            else if (HonestWeightQuestRules.Complete(state.StoryFlags))
            {
                body = HonestWeightMaudGreeting("");
                if (!HonestWeightQuestRules.RewardClaimed(state.StoryFlags)) choices.Add(MakePrimaryDialogueChoice("reward", "You mentioned a counterweighted tool.", "Choose one: Counterweight Knife or Measurekeeper Jack."));
                choices.Add(MakeDialogueChoice("account", "How did the traders take it?", "Hear what changed at the market."));
            }
            else if (HonestWeightQuestRules.EvidenceReady(state.StoryFlags))
            {
                body = "'Leave every buyer lighter.' There it is. It was meant to lighten the load on a porter, not the contents of a sack. We can break the weighing charm and take back its thirty-gold working deposit. Or I can write a public measure the traders can check themselves. They'll sell your company supplies for eight gold after that. I won't pretend either choice feeds everyone.";
                choices.Add(MakePrimaryDialogueChoice("review_break", "Show us what breaking the charm would do.", "Review: 30 gold once; market price stays 12 gold."));
                choices.Add(MakeDialogueChoice("review_retune", "Show us how to make it a public measure.", "Review: permanent 8-gold trades; no coin deposit."));
            }
            else
            {
                body = HonestWeightQuestRules.Objective(state.StoryFlags) + " The usual site services are still there. Ask to inspect the lamp or scale; an old visit won't tell us what these particular marks say.";
            }
            choices.Add(MakeDialogueChoice("back", "I'll leave you to your work.", "Return to Maud's usual services."));
            ShowDialogueChoices(HonestWeightQuestRules.Title, "Maud", body, ObjectType.Enchanter, violet, choices.ToArray(), ResolveHonestWeightConversation);
        }

        private void ResolveHonestWeightConversation(string choice)
        {
            if (state == null || state.Mode != GameMode.Explore) return;
            if (choice == "back") { ShowMaudConversation(); return; }
            if (choice == "reward") { ShowHonestWeightRewardChoice(); return; }
            if (choice == "review_break" || choice == "review_retune") { ShowHonestWeightDispositionReview(choice == "review_break" ? "break" : "retune"); return; }
            if (choice == "account")
            {
                ShowDialogueResponse(HonestWeightQuestRules.Title, "Maud", HonestWeightQuestRules.Retuned(state.StoryFlags)
                    ? "They tried onions first. Then iron. Then a child put his hat on it. The lamp treated all three honestly, which is more than I can say for most councils. Eight gold for a supply parcel whenever you visit a Dusk Market stall. They have put it in writing."
                    : "They're back to ordinary weights, which can only cheat in ordinary ways. Nessa sent someone to check them. The lamp's working deposit was never the traders' money; you did the work of freeing it, and I'm glad you took it.",
                    ObjectType.Enchanter, violet, ShowHonestWeightConversation);
                return;
            }
            if (choice != "accept" || !HonestWeightQuestRules.TryAccept(state.StoryFlags)) { ShowHonestWeightConversation(); return; }
            DiscoverZoneHint(HonestWeightQuestRules.QuestDepth, "dusk-market");
            DiscoverZoneHint(HonestWeightQuestRules.QuestDepth, "old-quarry");
            PushLog("Maud asks for the Chapter III Dusk Market lamp contract and Old Quarry Forge calibration. This optional investigation leaves the campaign route unchanged.", Tone.Good);
            AutosaveCheckpoint("Honest Weight accepted");
            ShowDialogueResponse(HonestWeightQuestRules.Title, "Maud", "Thank you. Copy the little words as carefully as the grand ones. 'For' and 'from' have done more damage in my trade than any demon I've met.", ObjectType.Enchanter, violet, ShowHonestWeightConversation);
        }

        private bool TryResolveHonestWeightSite(WorldMapSite site)
        {
            if (honestWeightServiceBypass || state?.Mode != GameMode.Explore
                || !HonestWeightQuestRules.CanInspect(state.StoryFlags, state.Depth, site.Id)) return false;
            honestWeightInspectSiteId = site.Id;
            bool market = site.Id == HonestWeightQuestRules.MarketSite;
            string serviceHint = "Use the usual site service.";
            if (WorldSiteInteractionRules.TryGet(site.Id, out WorldSiteInteractionProfile profile))
                serviceHint = WorldSiteInteractionRules.RewardClaimed(state.StoryFlags, state.Depth, site.Id)
                    ? market ? "Trade " + HonestWeightQuestRules.MarketTradeCost(state.StoryFlags) + " gold for 1 supply." : profile.RepeatSummary
                    : profile.RewardSummary;
            ShowDialogueChoices(HonestWeightQuestRules.Title, market ? "The Brass Weigh-Lamp" : "The Maker's Scale",
                market ? "The lamp hangs over a tray of onions. Its pan rises a little whenever a buyer looks away. Small letters run under the rim; Maud asked you to copy them."
                    : "A brass test weight still hangs from the quarry's cold scale. The maker cut a calibration into the beam. It is the second inscription Maud needs.",
                ObjectType.Obelisk, gold, new[]
                {
                    MakePrimaryDialogueChoice("inspect", market ? "Read the words beneath the pan." : "Copy the maker's calibration.", "Inspect for Maud. No gold or supplies are spent."),
                    MakeDialogueChoice("service", market ? "Use the market's usual service." : "Use the forge's usual service.", serviceHint),
                    MakeDialogueChoice("leave", "Leave it for another visit.", "Neither inspection nor service is used.")
                }, ResolveHonestWeightSiteChoice);
            return true;
        }

        private void ResolveHonestWeightSiteChoice(string choice)
        {
            string siteId = honestWeightInspectSiteId;
            honestWeightInspectSiteId = "";
            if (state?.Mode != GameMode.Explore || !showDialogue || dialogueTitle != HonestWeightQuestRules.Title
                || !HonestWeightQuestRules.CanInspect(state.StoryFlags, state.Depth, siteId)) return;
            MapObject siteObject = state.Map?.FindObjectById("regional-site:" + siteId);
            if (siteObject == null || Mathf.Abs(state.PlayerX - siteObject.X) + Mathf.Abs(state.PlayerY - siteObject.Y) > 1) return;
            CloseDialogue();
            if (choice == "service")
            {
                honestWeightServiceBypass = true;
                try { ResolveExploreObject(siteObject); }
                finally { honestWeightServiceBypass = false; }
                return;
            }
            if (choice != "inspect" || !HonestWeightQuestRules.TryInspect(state.StoryFlags, state.Depth, siteId)) return;
            bool market = siteId == HonestWeightQuestRules.MarketSite;
            string body = market
                ? "'Leave every buyer lighter; hold the balance in trust.' The word 'buyer' is squeezed over an older word: 'bearer.' Someone improved the lamp's profits with a chisel. You copy both versions for Maud."
                : "'A fair load is measured before it is lightened. The brass remembers thirty gold; the beam remembers true weight.' You copy the maker's standard. Maud should be able to end the contract or mend its terms.";
            PushLog(market ? "Copied the Bone Road weigh-lamp's altered contract." : "Copied the Bone Road quarry's original calibration.", Tone.Good);
            AutosaveCheckpoint(market ? "Honest Weight contract copied" : "Honest Weight calibration copied");
            ShowDialogue(HonestWeightQuestRules.Title, market ? "The Brass Weigh-Lamp" : "The Maker's Scale", body, ObjectType.Obelisk, gold);
        }

        private string HonestWeightSiteHint(WorldMapSite site)
        {
            if (!HonestWeightQuestRules.CanInspect(state?.StoryFlags, state?.Depth ?? 0, site.Id)) return "";
            return site.Id == HonestWeightQuestRules.MarketSite
                ? "Maud's optional investigation: inspect the weigh-lamp's contract, or use the normal market service. Inspection is free."
                : "Maud's optional investigation: inspect the maker's calibration, or use the normal forge service. Inspection is free.";
        }

        private void ShowHonestWeightDispositionReview(string disposition)
        {
            if (!HonestWeightQuestRules.EvidenceReady(state?.StoryFlags) || HonestWeightQuestRules.Complete(state.StoryFlags)) { ShowHonestWeightConversation(); return; }
            bool breakCharm = disposition == "break";
            ShowDialogueChoices(HonestWeightQuestRules.Title, "Maud", breakCharm
                ? "I can cut the weighing charm out and recover its thirty-gold working deposit for you. The stalls will use ordinary scales and keep their usual twelve-gold supply price. You'll still receive 120 base XP and your choice of either tool. Is that what you want?"
                : "I can restore the old clause and make the standard visible to everyone. The market keeps the working deposit; your company receives eight-gold supply trades at every Dusk Market. You'll still receive 120 base XP and your choice of either tool. Shall I write it?",
                ObjectType.Enchanter, violet, new[]
                {
                    MakePrimaryDialogueChoice(breakCharm ? "confirm_break" : "confirm_retune", breakCharm ? "Break the charm; take the 30 gold." : "Retune the lamp; secure 8-gold trades.", "Commit this choice. 120 base XP and one tool."),
                    MakeDialogueChoice("back", "Let us think about it.", "Leave the contract unresolved.")
                }, ResolveHonestWeightDisposition);
        }

        private void ResolveHonestWeightDisposition(string choice)
        {
            if (state?.Mode != GameMode.Explore) return;
            string disposition = choice == "confirm_break" ? "break" : choice == "confirm_retune" ? "retune" : "";
            if (!HonestWeightQuestRules.TryResolve(state.StoryFlags, disposition)) { ShowHonestWeightConversation(); return; }
            if (disposition == "break") state.Gold += HonestWeightQuestRules.BrokenGoldReward;
            AwardWorldExperience(HonestWeightQuestRules.ExperienceReward, HonestWeightQuestRules.Title + " resolved");
            PushLog(disposition == "break" ? "Maud breaks the altered weighing charm. Working deposit recovered: 30 gold."
                : "Maud restores the public measure. Dusk Market repeat supply trades now cost 8 gold at every chapter depth.", Tone.Good);
            AutosaveCheckpoint("Honest Weight " + disposition);
            ShowDialogueResponse(HonestWeightQuestRules.Title, "Maud", disposition == "break"
                ? "There. An expensive mistake has become a perfectly good lamp. I can live with that. I have two counterweighted pieces from the same workshop; choose whichever your company can use."
                : "'Leave the bearer lighter, and the bargain plain to see.' I should have written that years ago. The traders agreed to eight gold a parcel. Now, choose one of these counterweighted pieces; you earned it.",
                ObjectType.Enchanter, violet, ShowHonestWeightRewardChoice);
        }

        private void ShowHonestWeightRewardChoice()
        {
            if (!HonestWeightQuestRules.Complete(state?.StoryFlags) || HonestWeightQuestRules.RewardClaimed(state.StoryFlags)) { ShowHonestWeightConversation(); return; }
            ShowDialogueChoices(HonestWeightQuestRules.Title, "Maud", "The knife turns quickly in the hand. The jack spreads the weight of a blow. One is yours; neither has a contract hidden in the lining. I checked.\n\nBoth are +3 gear with +1 agility. The knife has a 20% base bleed chance before resistance. The jack reduces physical damage by 1. Your choice goes into the pack for you to equip.", ObjectType.Enchanter, violet, new[]
            {
                MakePrimaryDialogueChoice("knife", "Take the Counterweight Knife.", "+3 fast knife; +1 agility; base 20% bleed proc."),
                MakeDialogueChoice("jack", "Take the Measurekeeper Jack.", "+3 light armor; +1 agility; physical damage -1."),
                MakeDialogueChoice("later", "Keep them safe while we decide.", "The completed quest and unclaimed reward stay saved.")
            }, ResolveHonestWeightRewardChoice);
        }

        private void ResolveHonestWeightRewardChoice(string choice)
        {
            if (state?.Mode != GameMode.Explore) return;
            if (choice == "later") { ShowMaudConversation(); return; }
            string signatureId = choice == "knife" ? "counterweight_knife" : choice == "jack" ? "measurekeeper_jack" : "";
            InventoryItem reward = SignatureItemCatalog.Create(signatureId);
            if (reward == null || !HonestWeightQuestRules.TryClaimReward(state.StoryFlags)) { ShowHonestWeightConversation(); return; }
            EnsureInventoryList();
            AddInventoryItem(reward);
            AutosaveCheckpoint("Honest Weight tool chosen");
            CloseDialogue();
            ShowLootPanel(reward, 0, 0, 0, "Maud's workshop reward. It goes into the pack for deliberate equipping.", HonestWeightQuestRules.Title);
            PushLog("Maud hands over " + reward.DisplayName + ". The other tool remains in her workshop.", Tone.Good);
        }

        private void AddHonestWeightJournalRows(List<ArmoryRowView> rows)
        {
            if (rows == null || (!HonestWeightQuestRules.Available(state?.StoryFlags) && !HonestWeightQuestRules.Accepted(state?.StoryFlags))) return;
            rows.Add(new ArmoryRowView
            {
                Key = -1, Title = HonestWeightQuestRules.Title,
                Subtitle = HonestWeightQuestRules.RewardClaimed(state.StoryFlags) ? "optional / complete"
                    : HonestWeightQuestRules.Complete(state.StoryFlags) ? "optional / choose reward"
                    : HonestWeightQuestRules.Accepted(state.StoryFlags) ? "optional / investigating" : "optional / available — Maud",
                Detail = HonestWeightQuestRules.Objective(state.StoryFlags),
                AccentHex = ColorHtml(HonestWeightQuestRules.Complete(state.StoryFlags) ? teal : gold),
                Badge = HonestWeightQuestRules.RewardClaimed(state.StoryFlags) ? "DONE" : "OPTIONAL",
                BadgeAccentHex = ColorHtml(HonestWeightQuestRules.Complete(state.StoryFlags) ? teal : gold)
            });
        }
    }
}
