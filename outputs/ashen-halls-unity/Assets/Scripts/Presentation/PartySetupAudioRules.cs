using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public enum PartySetupAudioAction
    {
        Companion, Race, Class, DetailsOpen, DetailsClose, AttributeUp, AttributeDown,
        Name, Begin, Heraldry, Kit, Training
    }

    public static class PartySetupAudioRules
    {
        public const string MusicCue = "muster_by_firelight_loop";
        public static readonly IReadOnlyList<string> CueKeys = Array.AsReadOnly(new[]
        {
            "folio_companion", "folio_race_human", "folio_race_duskelf", "folio_race_stoneborn",
            "folio_race_fenkin", "folio_race_ashling", "folio_class_rogue", "folio_class_warrior",
            "folio_class_ranger", "folio_class_wizard", "folio_class_mage", "folio_class_warlock",
            "folio_class_priest", "folio_class_paladin", "folio_page_open", "folio_page_close",
            "folio_point_up", "folio_point_down", "folio_name", "folio_begin", "folio_heraldry",
            "folio_kit", "folio_train"
        });

        public static TitleAudioCueProfile CueFor(PartySetupAudioAction action, string identity = "")
        {
            switch (action)
            {
                case PartySetupAudioAction.Companion: return new TitleAudioCueProfile("folio_companion", .28f);
                case PartySetupAudioAction.Race:
                    return CharacterCreationCatalog.TryGetRace(identity, out CharacterCreationChoice race)
                        ? new TitleAudioCueProfile("folio_race_" + race.Key.Replace(" ", ""), .30f)
                        : new TitleAudioCueProfile("", 0f);
                case PartySetupAudioAction.Class:
                    return CharacterCreationCatalog.TryGetClass(identity, out CharacterCreationChoice characterClass)
                        ? new TitleAudioCueProfile("folio_class_" + characterClass.Key, .34f)
                        : new TitleAudioCueProfile("", 0f);
                case PartySetupAudioAction.DetailsOpen: return new TitleAudioCueProfile("folio_page_open", .27f);
                case PartySetupAudioAction.DetailsClose: return new TitleAudioCueProfile("folio_page_close", .25f);
                case PartySetupAudioAction.AttributeUp: return new TitleAudioCueProfile("folio_point_up", .26f);
                case PartySetupAudioAction.AttributeDown: return new TitleAudioCueProfile("folio_point_down", .24f);
                case PartySetupAudioAction.Name: return new TitleAudioCueProfile("folio_name", .28f);
                case PartySetupAudioAction.Begin: return new TitleAudioCueProfile("folio_begin", .40f);
                case PartySetupAudioAction.Heraldry: return new TitleAudioCueProfile("folio_heraldry", .25f);
                case PartySetupAudioAction.Kit: return new TitleAudioCueProfile("folio_kit", .30f);
                case PartySetupAudioAction.Training: return new TitleAudioCueProfile("folio_train", .27f);
                default: return new TitleAudioCueProfile("", 0f);
            }
        }

        public static bool ShouldPlayCue(GameMode mode, bool sfxMuted, int volumePercent, bool captureBlocked)
        {
            return mode == GameMode.Muster && !sfxMuted && volumePercent > 0 && !captureBlocked;
        }

        public static bool IsWorkshopCue(string key) => !string.IsNullOrEmpty(key) && key.StartsWith("folio_", StringComparison.Ordinal);
    }
}
