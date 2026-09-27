using System;
using System.Collections.Generic;
using UnityEngine;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private const string TessaRoadStockReviewTitle = "Review Tessa's Road Stock";
        private GameState tessaRoadStockQuoteState;
        private string tessaRoadStockQuoteId = "";
        private int tessaRoadStockQuoteGeneration;

        private DialogueChoiceView[] AddTessaRoadStockChoice(DialogueChoiceView[] choices)
        {
            if (!TessaRoadStockRules.Available(state?.StoryFlags)) return choices;
            List<DialogueChoiceView> result = new List<DialogueChoiceView>();
            foreach (DialogueChoiceView choice in choices ?? Array.Empty<DialogueChoiceView>())
                if (choice != null && choice.Id == "buy") result.Add(choice);
            result.Add(MakeDialogueChoice("roadstock", "Show us the road stock.", "Two named weapons, 76 gold each; one of each in stock."));
            result.Add(MakeDialogueChoice("advice", "Help us choose a weapon.", "Weapon forms, range, and the person using them."));
            foreach (DialogueChoiceView choice in choices ?? Array.Empty<DialogueChoiceView>())
                if (choice != null && choice.Id == "personal") result.Add(choice);
            return result.ToArray();
        }

        private void ShowTessaAdvice()
        {
            ShowDialogueChoices("Weapon Vendor", "Tessa",
                "No weapon suits every hand. Tell me how you intend to fight, and I'll tell you what to watch for.",
                ObjectType.WeaponVendor, gold, new[]
                {
                    MakeDialogueChoice("forms", "Which weapon suits which fighter?", "Damage, speed, attributes, and useful equipment."),
                    MakeDialogueChoice("range", "What changes when an enemy closes?", "Bow engagement, spear reach, and class techniques."),
                    MakeDialogueChoice("back", "Let's look at the rack again.", "Return to Tessa's stock and services.")
                }, ResolveTessaDialogueChoice);
        }

        private void ReturnToTessaAdvice()
        {
            if (TessaRoadStockRules.Available(state?.StoryFlags)) ShowTessaAdvice();
            else ShowTessaConversation();
        }

        private void ShowTessaRoadStock()
        {
            tessaRoadStockQuoteGeneration++;
            tessaRoadStockQuoteState = null;
            tessaRoadStockQuoteId = "";
            if (state?.Mode != GameMode.Explore) return;
            if (!TessaRoadStockRules.Available(state.StoryFlags))
            { ShowTessaConversation(); return; }
            List<DialogueChoiceView> choices = new List<DialogueChoiceView>();
            foreach (TessaRoadStockOffer offer in TessaRoadStockRules.All)
            {
                SignatureItemDefinition item = SignatureItemCatalog.Find(offer.SignatureId);
                bool bought = TessaRoadStockRules.Purchased(state.StoryFlags, offer);
                choices.Add(MakeDialogueChoice(offer.SignatureId,
                    bought ? item.DisplayName + " — sold" : item.DisplayName + " — " + offer.Price + " gold",
                    bought ? "Already purchased; check your Pack or equipment." : "Inspect the weapon and price before buying.", !bought));
            }
            choices.Add(MakeDialogueChoice("back", "Back to the counter.", "Nothing is bought by browsing."));
            ShowDialogueChoices("Tessa's Road Stock", "Tessa",
                "With Varkh gone, the bridge carts are moving again. I kept these two aside: a spear that braces well and an axe that bites cleanly. One of each, seventy-six gold apiece. You may buy either, both, or neither; take a look first.",
                ObjectType.WeaponVendor, gold, choices.ToArray(), choice =>
                {
                    if (choice == "back") ShowTessaConversation();
                    else ShowTessaRoadStockReview(choice);
                });
        }

        private void ShowTessaRoadStockReview(string signatureId)
        {
            TessaRoadStockOffer offer = TessaRoadStockRules.Find(signatureId);
            if (state?.Mode != GameMode.Explore) return;
            if (offer == null || !TessaRoadStockRules.Available(state.StoryFlags)
                || TessaRoadStockRules.Purchased(state.StoryFlags, offer)) { ShowTessaRoadStock(); return; }
            SignatureItemDefinition definition = SignatureItemCatalog.Find(signatureId);
            InventoryItem item = SignatureItemCatalog.Create(signatureId);
            tessaRoadStockQuoteState = state;
            tessaRoadStockQuoteId = signatureId;
            int quoteGeneration = ++tessaRoadStockQuoteGeneration;
            bool spear = signatureId == "bridgehook_spear";
            string body = spear
                ? "The bridge guards used the hook to draw a loose plank back into place. 'It buys you a step in a fight too,' Tessa says. 'Basic strikes reach one or two cells along a clear line, including when an enemy is beside you. Your spells and class techniques keep their own range.' "
                : "'That seam is older than I am,' Tessa says, turning the copper-stapled head. 'A patient smith repaired it. You get a slower, heavier physical cut and a steadier hit; the spear has the longer reach.' ";
            body += definition.DisplayName + ": base damage " + item.DamageMin + "–" + item.DamageMax
                + " physical, speed " + item.AttackSpeed + ". "
                + (spear ? "Health +1. Taking Guard gains one extra Guard. " : "Strength +1. Weapon attacks gain +2 hit. ")
                + "Price: " + offer.Price + " gold. You have " + state.Gold + ". "
                + (state.Gold >= offer.Price ? "Balance after purchase: " + (state.Gold - offer.Price) + ". "
                    : "You need " + (offer.Price - state.Gold) + " more gold. ")
                + "It goes into your Pack for you to compare and equip; nothing is replaced automatically.";
            ShowDialogueChoices(TessaRoadStockReviewTitle, "Tessa", body, ObjectType.WeaponVendor, gold, new[]
            {
                MakePrimaryDialogueChoice("confirm", "Buy for " + offer.Price + " gold.", "One purchase; added to the Pack for review.", state.Gold >= offer.Price),
                MakeDialogueChoice("back", "Put it back for now.", "Return to the road stock without spending gold.")
            }, choice =>
            {
                if (quoteGeneration == tessaRoadStockQuoteGeneration) ResolveTessaRoadStockReview(choice);
            });
        }

        private void ResolveTessaRoadStockReview(string choice)
        {
            TessaRoadStockOffer offer = TessaRoadStockRules.Find(tessaRoadStockQuoteId);
            bool valid = ReferenceEquals(tessaRoadStockQuoteState, state) && state?.Mode == GameMode.Explore
                && showDialogue && dialogueTitle == TessaRoadStockReviewTitle;
            tessaRoadStockQuoteGeneration++;
            tessaRoadStockQuoteState = null;
            tessaRoadStockQuoteId = "";
            if (!valid || choice != "confirm" || offer == null || !TessaRoadStockRules.Available(state.StoryFlags)
                || TessaRoadStockRules.Purchased(state.StoryFlags, offer) || state.Gold < offer.Price)
            { ShowTessaRoadStock(); return; }
            InventoryItem item = SignatureItemCatalog.Create(offer.SignatureId);
            if (item == null) { ShowTessaRoadStock(); return; }
            state.Gold -= offer.Price;
            SetStoryFlag(offer.ReceiptFlag);
            EnsureInventoryList();
            AddInventoryItem(item);
            AutosaveCheckpoint("Tessa road stock purchased");
            ShowDialogueThenLoot("Tessa's Road Stock", "Tessa",
                "Tessa wraps the edge before handing it over. 'Try the weight with each of you. A good weapon is still a poor bargain in the wrong hands.'",
                ObjectType.WeaponVendor, gold, item, 0, 0, 0, "Added to the Pack; compare wearers before equipping.", "Tessa's Road Stock");
            PushLog("Bought " + item.DisplayName + " from Tessa for " + offer.Price + " gold.", Tone.Good);
            PlaySfx("servicecoin", .5f);
        }
    }
}
