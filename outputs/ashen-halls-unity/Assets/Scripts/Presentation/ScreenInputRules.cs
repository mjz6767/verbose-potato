namespace AshenHalls
{
    public enum UiOverlay
    {
        None,
        Pause,
        Help,
        Armory,
        Dialogue,
        Loot,
        AbilityPicker
    }

    public static class ScreenInputRules
    {
        public static UiOverlay TopOverlay(bool pauseOpen, bool armoryOpen, bool dialogueOpen, bool lootOpen, bool abilityPickerOpen)
        {
            return TopOverlay(pauseOpen, false, armoryOpen, dialogueOpen, lootOpen, abilityPickerOpen);
        }

        public static UiOverlay TopOverlay(bool pauseOpen, bool helpOpen, bool armoryOpen, bool dialogueOpen, bool lootOpen, bool abilityPickerOpen)
        {
            if (helpOpen) return UiOverlay.Help;
            if (pauseOpen) return UiOverlay.Pause;
            if (dialogueOpen) return UiOverlay.Dialogue;
            if (armoryOpen) return UiOverlay.Armory;
            if (abilityPickerOpen) return UiOverlay.AbilityPicker;
            if (lootOpen) return UiOverlay.Loot;
            return UiOverlay.None;
        }

        public static bool CanAcceptGameplayInput(UiOverlay overlay)
        {
            return overlay == UiOverlay.None;
        }

        public static bool ShouldSuppressBoardPointer(int currentFrame, int suppressThroughFrame)
        {
            return currentFrame <= suppressThroughFrame;
        }

        public static bool ShouldRouteBoardPointer(
            UiOverlay overlay,
            bool pointerOverUnityUi,
            bool insideBoard,
            bool insideLegacySidePanel,
            bool insideLegacyCommandBar)
        {
            return CanAcceptGameplayInput(overlay)
                && !pointerOverUnityUi
                && insideBoard
                && !insideLegacySidePanel
                && !insideLegacyCommandBar;
        }
    }
}
