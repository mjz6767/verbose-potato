using System;

namespace AshenHalls
{
    public readonly struct EncounterGuidance
    {
        public readonly string Title;
        public readonly string Priority;
        public readonly string Plan;
        public readonly string Reminder;

        public EncounterGuidance(string title, string priority, string plan, string reminder)
        {
            Title = title ?? "";
            Priority = priority ?? "";
            Plan = plan ?? "";
            Reminder = reminder ?? "";
        }

        public bool IsValid => !string.IsNullOrWhiteSpace(Title);
    }

    public static class EncounterGuidanceCatalog
    {
        public static bool TryFor(string encounterStyle, out EncounterGuidance guidance)
        {
            switch ((encounterStyle ?? "").ToLowerInvariant())
            {
                case "sewer_broken_sluice":
                    guidance = new EncounterGuidance(
                        "Broken Sluice",
                        "Priority: hold a clean formation.",
                        "Maer holds the lane; Cairn shoots from range.",
                        "Hover a rat to preview hit chance and damage.");
                    return true;
                case "sewer_foul_runoff":
                    guidance = new EncounterGuidance(
                        "Foul Runoff",
                        "Priority: drop the Plague Mage first.",
                        "Skirt gas and web; Ward or Cleanse poison.",
                        "Hover a unit to inspect range, status, and threat.");
                    return true;
                case "sewer_cistern_den":
                    guidance = new EncounterGuidance(
                        "Cistern Den",
                        "Priority: break the spell line.",
                        "Pin the Mage; isolate the Brute behind cover.",
                        "Use the safe-room weapon and spend elixirs early.");
                    return true;
                default:
                    guidance = default;
                    return false;
            }
        }
    }
}
