using System;
using UnityEngine;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private const string ConfirmShopPurchaseChoice = "confirm_purchase";
        private const string KeepBrowsingChoice = "keep_browsing";

        private GameState tessaQuoteState;
        private string tessaQuoteLeadId = "";
        private string tessaQuoteLeadRole = "";
        private InventoryItem tessaQuotedWeapon;

        private DialogueChoiceView MakePrimaryDialogueChoice(
            string id,
            string label,
            string hint,
            bool enabled = true)
        {
            return new DialogueChoiceView
            {
                Id = id,
                Label = label,
                Hint = hint,
                Enabled = enabled,
                Primary = true
            };
        }

        private void ShowKatePurchaseReview(bool provisionStall, ObjectType interactionFocus)
        {
            int cost = provisionStall ? 10 : 12;
            int bundle = provisionStall ? 3 : 4;
            if (HasKateStarterBundle() || state == null || state.Gold < cost)
            {
                ShowKateConversation(provisionStall, null, interactionFocus);
                return;
            }

            string speaker = provisionStall ? "Lute" : "Kate";
            ObjectType focus = KateConversationFocus(provisionStall, interactionFocus);
            string review = provisionStall
                ? $"Lute sets {bundle} wax-sealed parcels on the counter and checks each knot once more. \"Dry seals, honest weight, and one parcel at a time.\" The bundle costs {cost} gold; you would leave with {state.Gold - cost}."
                : $"Kate wraps {bundle} provisions in a clean cloth and rests a hand on the bundle. \"This gets you there, gets you home, and leaves one meal for a delay.\" The bundle costs {cost} gold; you would leave with {state.Gold - cost}.";
            ShowDialogueChoices(
                provisionStall ? "Review Lute's Order" : "Review Kate's Order",
                speaker,
                review,
                focus,
                Hex("d98b6a"),
                new[]
                {
                    MakePrimaryDialogueChoice(
                        ConfirmShopPurchaseChoice,
                        $"Pack {bundle} provisions - {cost} gold",
                        $"Confirm purchase: +{bundle} provisions | {state.Gold - cost} gold remaining."),
                    MakeDialogueChoice(
                        KeepBrowsingChoice,
                        "Not yet - show me the other options",
                        "Return to the shop without spending gold.")
                },
                choice => ResolveKatePurchaseReview(choice, provisionStall, focus));
        }

        private void ResolveKatePurchaseReview(string choice, bool provisionStall, ObjectType interactionFocus)
        {
            if (string.Equals(choice, ConfirmShopPurchaseChoice, StringComparison.Ordinal))
            {
                PurchaseKateStarterBundle(provisionStall, interactionFocus);
                return;
            }

            ShowKateConversation(provisionStall, null, interactionFocus);
        }

        private void ShowBorinPurchaseReview()
        {
            const int cost = 28;
            if (HasStoryFlag(StoryFlags.MidgaardBasicArmorBought) || state == null || state.Gold < cost)
            {
                ShowBorinConversation();
                return;
            }

            InventoryItem offer = MakeTownArmor();
            string armorName = TrimGearName(offer.DisplayName);
            string review =
                $"Borin turns the {armorName} inside out. \"Look at the joins, not the shine.\" "
                + $"It grants +{offer.Bonus} armor and +{offer.HealthBonus} health. Price: {cost} gold. Balance after fitting: {state.Gold - cost} gold.";
            ShowDialogueChoices(
                "Review Borin's Fitting",
                "Borin",
                review,
                ObjectType.Armorer,
                stone,
                new[]
                {
                    MakePrimaryDialogueChoice(
                        ConfirmShopPurchaseChoice,
                        $"Fit the {armorName} - {cost} gold",
                        $"Confirm fitting | {state.Gold - cost} gold remaining."),
                    MakeDialogueChoice(
                        KeepBrowsingChoice,
                        "Let me think",
                        "Return to Borin's services without spending gold.")
                },
                ResolveBorinPurchaseReview);
        }

        private void ResolveBorinPurchaseReview(string choice)
        {
            if (string.Equals(choice, ConfirmShopPurchaseChoice, StringComparison.Ordinal))
            {
                PurchaseBorinHauberk();
                return;
            }

            ShowBorinConversation();
        }

        private void ShowTessaPurchaseReview()
        {
            const int cost = 32;
            if (HasStoryFlag(StoryFlags.MidgaardBasicWeaponBought) || state == null || state.Gold < cost)
            {
                ShowTessaConversation();
                return;
            }

            PartyMember lead = state.Party != null && state.Party.Count > 0 ? state.Party[0] : null;
            InventoryItem offer = GetOrCreateTessaWeaponQuote();
            string leadName = string.IsNullOrWhiteSpace(lead?.Name) ? "your lead fighter" : lead.Name;
            string weaponName = TrimGearName(offer.DisplayName);
            string review =
                $"Tessa lays the {weaponName} across the counter for {leadName}. \"{TessaFitReason(lead?.Role)}\" "
                + $"Damage: {offer.DamageMin}-{offer.DamageMax} {offer.DamageType}. Speed: {offer.AttackSpeed}. Price: {cost} gold. Balance after: {state.Gold - cost} gold.";
            ShowDialogueChoices(
                "Review Tessa's Offer",
                "Tessa",
                review,
                ObjectType.WeaponVendor,
                gold,
                new[]
                {
                    MakePrimaryDialogueChoice(
                        ConfirmShopPurchaseChoice,
                        $"Take the {weaponName} - {cost} gold",
                        $"Confirm purchase for {leadName} | {state.Gold - cost} gold remaining."),
                    MakeDialogueChoice(
                        KeepBrowsingChoice,
                        "Put it back for now",
                        "Return to Tessa's advice without spending gold.")
                },
                ResolveTessaPurchaseReview);
        }

        private void ResolveTessaPurchaseReview(string choice)
        {
            if (string.Equals(choice, ConfirmShopPurchaseChoice, StringComparison.Ordinal))
            {
                PurchaseTessaWeapon();
                return;
            }

            ShowTessaConversation();
        }

        private InventoryItem GetOrCreateTessaWeaponQuote()
        {
            PartyMember lead = state?.Party != null && state.Party.Count > 0 ? state.Party[0] : null;
            string role = string.IsNullOrWhiteSpace(lead?.Role) ? "shield" : lead.Role;
            string leadId = lead?.Id ?? "";
            bool quoteMatches = tessaQuotedWeapon != null
                && ReferenceEquals(tessaQuoteState, state)
                && string.Equals(tessaQuoteLeadId, leadId, StringComparison.Ordinal)
                && string.Equals(tessaQuoteLeadRole, role, StringComparison.Ordinal);
            if (quoteMatches) return tessaQuotedWeapon;

            tessaQuoteState = state;
            tessaQuoteLeadId = leadId;
            tessaQuoteLeadRole = role;
            tessaQuotedWeapon = MakeTownWeapon(role);
            return tessaQuotedWeapon;
        }

        private InventoryItem TakeTessaWeaponQuote(string fallbackRole)
        {
            InventoryItem offer = GetOrCreateTessaWeaponQuote();
            if (offer == null) offer = MakeTownWeapon(string.IsNullOrWhiteSpace(fallbackRole) ? "shield" : fallbackRole);
            tessaQuoteState = null;
            tessaQuoteLeadId = "";
            tessaQuoteLeadRole = "";
            tessaQuotedWeapon = null;
            return offer;
        }

        private static string TessaFitReason(string role)
        {
            switch ((role ?? "").ToLowerInvariant())
            {
                case "bow": return "Light enough to find quickly after the bowstring goes slack.";
                case "pike": return "Long in the reach, balanced enough for a close tunnel.";
                case "knife": return "Quick through the turn, with no extra iron to argue with.";
                case "ember":
                case "hex":
                case "mender": return "A focus should carry the spell without borrowing the hand.";
                default: return "Enough weight for a shield, without dragging your guard open.";
            }
        }
    }
}
