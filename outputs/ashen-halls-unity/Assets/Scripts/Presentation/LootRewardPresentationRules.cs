using System;
using System.Collections.Generic;

namespace AshenHalls
{
    // Presentation only: rewards are committed before this receipt is opened.
    // The complete equipment comparison remains available through Inventory.
    public static class LootRewardPresentationRules
    {
        public static bool HasResources(LootPopupView view)
        {
            return view != null && (view.Gold > 0 || view.Supplies > 0 || view.Elixirs > 0);
        }

        public static string ItemDetails(string traits, string rarity)
        {
            string text = (traits ?? "").Trim();
            text = text.Replace("Traits: ", "").Replace("Details: ", "");
            string suffix = " / " + (rarity ?? "").Trim();
            if (suffix.Length > 3 && text.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                text = text.Substring(0, text.Length - suffix.Length);
            return text.Replace(" / ", "  ·  ");
        }

        public static string ComparisonPreview(string comparison)
        {
            string text = (comparison ?? "").Trim();
            if (text.Length == 0) return "";
            string[] parts = text.Split(new[] { " / " }, StringSplitOptions.RemoveEmptyEntries);
            var lines = new List<string>();
            // Form changes and strategic gains/losses precede numerical details in
            // the canonical comparison. Never hide those behind equal-stat noise.
            for (int priority = 0; priority < 3; priority++)
            foreach (string part in parts)
            {
                string value = part.Trim();
                if (value.EndsWith("(=)", StringComparison.Ordinal)) continue;
                // Safety/readability: after named form/intrinsic changes and base
                // damage, numerical sacrifices precede gains in a short preview.
                bool numeric = IsNumericComparison(value);
                int tokenPriority = !numeric ? 0 : value.IndexOf("(-", StringComparison.Ordinal) >= 0 ? 1 : 2;
                if (tokenPriority != priority || lines.Count == 5) continue;
                lines.Add(ReadableStat(value));
            }
            if (lines.Count == 0) return "Same core stats as current equipment.";
            return string.Join("\n", lines);
        }

        public static string StorageDetail(string note, string outcome)
        {
            string text = (note ?? "").Trim();
            if (text.Length == 0 || string.Equals(text, (outcome ?? "").Trim(), StringComparison.OrdinalIgnoreCase)) return "";
            // The recommendation and saved-status line already communicate these
            // routine acquisition instructions. Retain authored reward narrative.
            if ((text.IndexOf(" — ", StringComparison.Ordinal) >= 0
                    && (text.EndsWith("Kept in the pack.", StringComparison.OrdinalIgnoreCase)
                        || text.EndsWith("Kept in the pack for your decision.", StringComparison.OrdinalIgnoreCase)))
                || text.Equals("The company stores are updated.", StringComparison.OrdinalIgnoreCase)
                || text.Equals("Recovered from the defeated mob.", StringComparison.OrdinalIgnoreCase)) return "";
            text = text.Replace("Added to the pack for deliberate equipping.", "")
                .Replace("It goes into the pack for deliberate equipping.", "").Trim();
            return text;
        }

        private static string ReadableStat(string token)
        {
            string[] from = { "BON ", "SPD ", "RNG ", "ARM ", "AGI ", "FIT ", "ATTR " };
            string[] to = { "Bonus ", "Speed ", "Range ", "Armor ", "Agility ", "Role fit ", "Attributes " };
            for (int i = 0; i < from.Length; i++)
                if (token.StartsWith(from[i], StringComparison.Ordinal)) return to[i] + token.Substring(from[i].Length);
            return token;
        }

        private static bool IsNumericComparison(string token)
        {
            foreach (string prefix in new[] { "BON ", "SPD ", "RNG ", "ARM ", "AGI ", "FIT ", "ATTR " })
                if (token.StartsWith(prefix, StringComparison.Ordinal)) return true;
            return false;
        }
    }
}
