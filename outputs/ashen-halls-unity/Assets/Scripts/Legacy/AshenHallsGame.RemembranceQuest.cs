using System;
using System.Collections.Generic;
using UnityEngine;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private bool remembranceInspectingEntrance;

        private DialogueChoiceView[] AddRemembranceChoice(ObjectType contact, DialogueChoiceView[] choices)
        {
            if (state == null || (contact != ObjectType.WoundedTraveler && !RemembranceQuestRules.Accepted(state.StoryFlags))) return choices;
            List<DialogueChoiceView> result = choices == null ? new List<DialogueChoiceView>() : new List<DialogueChoiceView>(choices);
            result.Add(MakeDialogueChoice("remembrance", contact == ObjectType.WoundedTraveler
                    ? RemembranceQuestRules.Accepted(state.StoryFlags) ? "About your companions..." : "Can we do anything for you?"
                    : "About Edda's companions...", RemembranceQuestRules.Title));
            return result.ToArray();
        }

        private string RemembranceGreeting(ObjectType contact, string fallback)
        {
            if (state == null || !RemembranceQuestRules.Accepted(state.StoryFlags)) return fallback;
            bool delivered = RemembranceQuestRules.Delivered(state.StoryFlags);
            bool privateMemorial = HasStoryFlag(StoryFlags.RemembrancePrivateChosen);
            if (contact == ObjectType.WoundedTraveler)
            {
                if (delivered) return privateMemorial
                    ? "Sera left a place beside the names. I think I can sit there awhile. For once I don't have to wonder what I ought to say."
                    : "I read Brann's notice. You kept the part about me running. Thank you. Someone else may need to hear that coming home is allowed.";
                if (RemembranceQuestRules.NamesRead(state.StoryFlags)) return "You found their names? Let me see. Yes. That is Jory's terrible handwriting. I thought I'd never be glad to see it.";
                return "Any luck with the roll? There's no hurry I can put on this. Come back in one piece.";
            }
            if (delivered && contact == ObjectType.NoviceHealer) return privateMemorial
                ? "The four names are in the remembrance book. When Edda comes, I'll leave her to it. Some kinds of help are quiet."
                : "Brann sent me a copy of the notice. I have put the four names in our book too; a warning needn't be the only thing they leave behind.";
            if (delivered && contact == ObjectType.GateCaptain) return privateMemorial
                ? "Sera tells me Edda's friends have their names back. The watch can learn from what happened without asking her to stand before a crowd."
                : "The notice is up. Names first, lessons second. The next party through this gate will know who paid for that advice.";
            return fallback;
        }

        private bool TryResolveRemembranceContactChoice(string choice, ObjectType contact)
        {
            if (choice != "remembrance" || state == null) return false;
            ShowRemembranceContact(contact);
            return true;
        }

        private void ShowRemembranceContact(ObjectType contact)
        {
            if (state == null) return;
            if (contact == ObjectType.WoundedTraveler)
            {
                ShowEddaRemembrance();
                return;
            }
            string speaker = contact == ObjectType.NoviceHealer ? "Sera" : "Brann";
            if (RemembranceQuestRules.Delivered(state.StoryFlags) && !RemembranceQuestRules.RewardClaimed(state.StoryFlags))
            {
                ShowRemembranceRewardChoice(contact);
                return;
            }
            bool readyForNames = contact == ObjectType.NoviceHealer && RemembranceQuestRules.Recovered(state.StoryFlags)
                && !RemembranceQuestRules.NamesRead(state.StoryFlags);
            bool readyForDelivery = !RemembranceQuestRules.Delivered(state.StoryFlags)
                && (contact == ObjectType.NoviceHealer && HasStoryFlag(StoryFlags.RemembrancePrivateChosen)
                    || contact == ObjectType.GateCaptain && HasStoryFlag(StoryFlags.RemembrancePublicChosen));
            string body = readyForNames
                ? "Careful, don't unfold it yet. The temple keeps copies of the work crews. If the ink has run, we may still be able to put names to the marks. Will you let me try?"
                : readyForDelivery
                    ? contact == ObjectType.NoviceHealer
                        ? "A page in the remembrance book, and a lamp Edda can light when she wishes. No speeches. I can do that. Shall we enter the names?"
                        : "I'll post her account as she gave it. No brave last words she never heard. The four names belong at the top. Shall I put it before the watch?"
                    : RemembranceQuestRules.RewardClaimed(state.StoryFlags)
                        ? RemembranceGreeting(contact, RemembranceQuestRules.Objective(state.StoryFlags))
                        : RemembranceQuestRules.Objective(state.StoryFlags);
            List<DialogueChoiceView> choices = new List<DialogueChoiceView>();
            if (readyForNames) choices.Add(MakeDialogueChoice("read", "Match the roll to the temple records.", "Recover the four names."));
            if (readyForDelivery) choices.Add(MakeDialogueChoice("deliver", contact == ObjectType.NoviceHealer ? "Enter the names and leave a lamp." : "Post the names and Edda's account.", "Complete the remembrance; choose one keepsake."));
            choices.Add(MakeDialogueChoice("back", "Thank you. Something else..."));
            ShowDialogueChoices(RemembranceQuestRules.Title, speaker, body, contact, teal, choices.ToArray(),
                choice => ResolveRemembranceContactAction(contact, choice));
        }

        private void ShowEddaRemembrance()
        {
            List<DialogueChoiceView> choices = new List<DialogueChoiceView>();
            string body;
            if (!RemembranceQuestRules.Accepted(state.StoryFlags))
            {
                body = "We kept a roll in a blue oilskin case, behind the sluice wheel in the Foul Runoff alcove. Four names, and mine. I remember their faces. I can't bear the thought of the city forgetting them. If you clear that room, would you look for it?";
                choices.Add(MakeDialogueChoice("accept", "We'll look for their names.", "Optional: inspect the alcove after clearing Foul Runoff."));
            }
            else if (RemembranceQuestRules.NamesRead(state.StoryFlags) && !RemembranceQuestRules.HasDisposition(state.StoryFlags))
            {
                body = "Jory, Nella, Halden, and Vela. Nella used to count us twice before we set out.\n\nSera offered a page in the temple book. Brann asked whether the watch could learn from our mistakes. I'd welcome either, if it's honest. I don't want their last day to turn into a story about five fools.";
                choices.Add(MakeDialogueChoice("private", "A quiet memorial, where you can visit.", "Sera will keep the names in the temple book. Same reward choices."));
                choices.Add(MakeDialogueChoice("public", "Let the watch hear the truth, with their names.", "Brann will post the account, with Edda's consent. Same reward choices."));
            }
            else body = RemembranceQuestRules.Delivered(state.StoryFlags)
                ? RemembranceGreeting(ObjectType.WoundedTraveler, "Thank you for bringing them home in the way we could.")
                : RemembranceQuestRules.Objective(state.StoryFlags);
            choices.Add(MakeDialogueChoice("back", "We'll speak again.", "Leave the optional favor for later."));
            ShowDialogueChoices(RemembranceQuestRules.Title, "Edda", body, ObjectType.WoundedTraveler, blood, choices.ToArray(), ResolveEddaRemembranceAction);
        }

        private void ResolveEddaRemembranceAction(string choice)
        {
            if (state == null || state.Mode != GameMode.Explore) return;
            if (choice == "back") { ShowEddaConversation(); return; }
            bool changed = choice == "accept"
                ? RemembranceQuestRules.TryAccept(state.StoryFlags)
                : RemembranceQuestRules.TryChooseDisposition(state.StoryFlags, choice);
            if (!changed) { ShowEddaRemembrance(); return; }
            string response = choice == "accept"
                ? "Thank you. Blue oilskin, behind the wheel. Ask Sera to read it if the water has got in. And don't go down there just for me while the room is still crawling."
                : choice == "private"
                    ? "A quiet page. Yes. I'd like somewhere I can sit without anyone waiting for me to be brave. Tell Sera that, please."
                    : "Then tell it straight. We chased a retreating enemy; it split us. I ran, and I lived. Put their names above that lesson, so the watch remembers they were people.";
            RemembranceCheckpoint(choice == "accept" ? "favor accepted" : "remembrance chosen");
            ShowDialogueResponse(RemembranceQuestRules.Title, "Edda", response, ObjectType.WoundedTraveler, blood, ShowEddaConversation);
        }

        private void ResolveRemembranceContactAction(ObjectType contact, string choice)
        {
            if (state == null || state.Mode != GameMode.Explore) return;
            if (choice == "back") { ReturnToRemembranceContact(contact); return; }
            if (choice == "read" && contact == ObjectType.NoviceHealer && RemembranceQuestRules.TryReadNames(state.StoryFlags))
            {
                RemembranceCheckpoint("four names recovered");
                ShowDialogueResponse(RemembranceQuestRules.Title, "Sera",
                    "Jory Vale. Nella Fen. Halden Reed. Vela Bell. There, beside Edda's name.\n\nThe last line isn't an order to press on. It says, 'Fall back together.' Whatever happened after that, they tried to bring each other home. Show Edda. She should have a say in what happens to this.",
                    contact, teal, () => ShowRemembranceContact(contact));
                return;
            }
            if (choice == "deliver" && RemembranceQuestRules.TryDeliver(state.StoryFlags, contact))
            {
                RemembranceCheckpoint("names delivered");
                ShowBanner("The Four Names Remembered");
                ShowRemembranceRewardChoice(contact);
                return;
            }
            ShowRemembranceContact(contact);
        }

        private void EnterMidgaardSewer()
        {
            remembranceInspectingEntrance = false;
            if (state != null && RemembranceQuestRules.Accepted(state.StoryFlags)
                && !RemembranceQuestRules.Recovered(state.StoryFlags)
                && RemembranceQuestRules.AlcoveAccessible(state.StoryFlags))
            {
                remembranceInspectingEntrance = true;
                ShowDialogueChoices(RemembranceQuestRules.Title, "The Cleared Alcove",
                    "The watchman lifts the grate. The path back to the cleared Foul Runoff alcove is quiet now. Edda said to look behind the sluice wheel.",
                    ObjectType.Sewer, teal, new[]
                    {
                        MakeDialogueChoice("inspect", "Search behind the sluice wheel.", "Look for the blue oilskin case."),
                        MakeDialogueChoice("contract", "Continue the cistern contract.", "Leave Edda's favor for later."),
                        MakeDialogueChoice("leave", "Stay in Midgaard for now.")
                    }, ResolveRemembranceSewerChoice);
                return;
            }
            EnterMidgaardSewerContract();
        }

        private void ResolveRemembranceSewerChoice(string choice)
        {
            if (state == null || state.Mode != GameMode.Explore || state.Depth != 1 || !remembranceInspectingEntrance) return;
            if (choice != "inspect" && choice != "contract" && choice != "leave") return;
            remembranceInspectingEntrance = false;
            if (choice == "contract") { CloseDialogue(); EnterMidgaardSewerContract(); return; }
            if (choice == "leave") { CloseDialogue(); return; }
            if (!RemembranceQuestRules.TryRecover(state.StoryFlags)) { CloseDialogue(); return; }
            RemembranceCheckpoint("oilskin roll recovered");
            ShowDialogue(RemembranceQuestRules.Title, "Blue Oilskin Case",
                "Behind the wheel, a blue corner catches the lamplight. The case has kept most of the water out, but four lines of ink have blurred. Below them, one cramped sentence survives: 'Fall back together.'\n\nYou secure the roll with the quest records. Sera may be able to match the names to the temple's book.",
                ObjectType.Sewer, teal);
        }

        private void ShowRemembranceRewardChoice(ObjectType contact)
        {
            if (state == null || !RemembranceQuestRules.Delivered(state.StoryFlags) || RemembranceQuestRules.RewardClaimed(state.StoryFlags))
            {
                ReturnToRemembranceContact(contact);
                return;
            }
            ShowDialogueChoices(RemembranceQuestRules.Title, contact == ObjectType.GateCaptain ? "Brann" : "Sera",
                "Edda left these with us before that last trip. 'Let someone bring them home this time,' she said. Take whichever your party can use. We'll keep the other safe for her.",
                contact, gold, new[]
                {
                    MakeDialogueChoice("bow", "Take the +2 Traveler's Knot Bow.", "Physical bow: agility +1, weapon hit +2. +8 XP."),
                    MakeDialogueChoice("wrap", "Take the +2 Vigilkeeper Wrap.", "Armor with an intelligence bonus; cold damage taken -1. +8 XP."),
                    MakeDialogueChoice("later", "Keep them safe while we decide.", "Claim one keepsake here or from the other contact later.")
                }, choice => ResolveRemembranceRewardChoice(contact, choice));
        }

        private void ResolveRemembranceRewardChoice(ObjectType contact, string choice)
        {
            if (state == null || state.Mode != GameMode.Explore) return;
            if (choice == "later") { ReturnToRemembranceContact(contact); return; }
            if (choice != "bow" && choice != "wrap") return;
            if (!RemembranceQuestRules.Delivered(state.StoryFlags) || RemembranceQuestRules.RewardClaimed(state.StoryFlags)) return;
            InventoryItem reward = choice == "bow" ? SignatureItemCatalog.CreateTravelersKnot() : SignatureItemCatalog.CreateVigilkeeperWrap();
            if (reward == null || !RemembranceQuestRules.TryClaimReward(state.StoryFlags, choice)) return;
            EnsureInventoryList();
            AddInventoryItem(reward);
            AwardWorldExperience(RemembranceQuestRules.ExperienceReward, RemembranceQuestRules.Title);
            CloseDialogue();
            ShowLootPanel(reward, 0, 0, 0, "Edda's keepsake. Added to the pack for deliberate equipping.", RemembranceQuestRules.Title);
            RemembranceCheckpoint("keepsake claimed");
            ShowBanner("Names Beneath the Water Complete");
            PlaySfx("questcomplete", 0.66f);
        }

        private void ReturnToRemembranceContact(ObjectType contact)
        {
            if (contact == ObjectType.GateCaptain) ShowBrannConversation();
            else if (contact == ObjectType.NoviceHealer) ShowSeraConversation();
            else ShowEddaConversation();
        }

        private void RemembranceCheckpoint(string reason)
        {
            PushLog(RemembranceQuestRules.Title + ": " + RemembranceQuestRules.Objective(state.StoryFlags), Tone.Good);
            MarkUiDirty();
            AutosaveCheckpoint(RemembranceQuestRules.Title + " - " + reason);
        }

        private void AddRemembranceJournalRows(List<ArmoryRowView> rows)
        {
            if (rows == null || state == null) return;
            rows.Add(new ArmoryRowView
            {
                Key = -1,
                Title = RemembranceQuestRules.Title,
                Subtitle = RemembranceQuestRules.RewardClaimed(state.StoryFlags) ? "optional favor complete"
                    : RemembranceQuestRules.Accepted(state.StoryFlags) ? "optional favor in progress" : "optional favor — Edda",
                Detail = RemembranceQuestRules.Objective(state.StoryFlags),
                AccentHex = ColorHtml(teal)
            });
        }
    }
}
