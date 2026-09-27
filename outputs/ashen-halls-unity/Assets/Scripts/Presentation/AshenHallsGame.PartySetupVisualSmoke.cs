using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private bool TryApplyPartySetupVisualSmokeLaunch(string[] args)
        {
            if (args == null || !args.Any(arg => string.Equals(arg, "-ashen-party-setup-smoke", StringComparison.OrdinalIgnoreCase)))
                return false;
            // Keep capture staging isolated from campaign and preference persistence.
            visualSmokeSaveBlocked = true;
            StartNewGame();
            state.ReducedMotion = args.Any(arg => string.Equals(arg, "-ashen-party-reduced-motion", StringComparison.OrdinalIgnoreCase));
            ApplyVisualSmokeSeed(args);
            string memberText = PartySetupSmokeOption(args, "-ashen-party-member", "0");
            if (!int.TryParse(memberText, out int index) || index < 0 || index >= state.Party.Count)
                throw new InvalidOperationException("Party Setup capture member must be a valid zero-based roster index.");
            SelectPartySetupMember(index);
            string race = PartySetupSmokeOption(args, "-ashen-party-race", state.Party[index].Race);
            string classKey = PartySetupSmokeOption(args, "-ashen-party-class", state.Party[index].ClassKey);
            if (!CharacterCreationCatalog.Races.Any(choice => choice.Key == race)
                || !CharacterCreationCatalog.Classes.Any(choice => choice.Key == classKey))
                throw new InvalidOperationException("Party Setup capture requires a supported race and class.");
            SetSelectedMemberRace(race);
            SetSelectedMemberClass(classKey);
            string name = PartySetupSmokeOption(args, "-ashen-party-name", "");
            if (!string.IsNullOrEmpty(name)) SetSelectedMemberName(name);
            SyncPartySetupScreen();
            partySetupScreen.Refresh();
            bool detailsRequested = args.Any(arg => string.Equals(arg, "-ashen-party-details", StringComparison.OrdinalIgnoreCase));
            Button requestedTab = partySetupScreen.ViewCanvas.GetComponentsInChildren<Button>(true)
                .Single(button => button.name == (detailsRequested ? "Details Tab" : "Identity Tab"));
            requestedTab.onClick.Invoke();
            partySetupScreen.Refresh();
            Transform[] pages = partySetupScreen.ViewCanvas.GetComponentsInChildren<Transform>(true);
            bool identityVisible = pages.Single(page => page.name == "Identity Page").gameObject.activeInHierarchy;
            bool detailsVisible = pages.Single(page => page.name == "Details Page").gameObject.activeInHierarchy;
            if (detailsVisible != detailsRequested || identityVisible == detailsRequested)
                throw new InvalidOperationException("Party Setup capture did not activate the requested tab.");
            Debug.Log(VersionInfo.ProductName + " visual smoke mode: party setup. member=" + index
                + ", race=" + race + ", class=" + classKey + ", portrait="
                + CharacterCreationCatalog.PortraitAtlasFile(race, classKey) + ":"
                + CharacterCreationCatalog.PortraitCell(race, classKey) + ", tab="
                + (detailsRequested ? "details" : "identity") + ", persistence=blocked.");
            return true;
        }

        private bool TryCapturePartySetupOffscreen(string path, int width, int height, string[] args)
        {
            if (args == null || !args.Any(arg => string.Equals(arg, "-ashen-party-setup-smoke", StringComparison.OrdinalIgnoreCase)))
                return false;
            if (state == null || state.Mode != GameMode.Muster || partySetupScreen == null)
                throw new InvalidOperationException("Party Setup offscreen capture requires the staged Muster screen.");
            PreparePartySetupCaptureMotion(args);
            PartySetupCaptureRenderer.Write(partySetupScreen, width, height, path);
            Debug.Log(VersionInfo.ProductName + " party setup capture renderer: actual player canvas / offscreen camera / " + width + "x" + height + ".");
            return true;
        }

        private void PreparePartySetupCaptureMotion(string[] args)
        {
            bool transition = args != null && args.Any(arg => string.Equals(arg, "-ashen-party-transition-smoke", StringComparison.OrdinalIgnoreCase));
            partySetupScreen.Refresh();
            partySetupScreen.AdvancePresentation(1f);
            string motion = state.ReducedMotion ? "reduced" : "settled";
            if (transition)
            {
                if (state.ReducedMotion) throw new InvalidOperationException("A transition capture cannot request Reduced Motion.");
                string targetClass = SelectedBuilderMember().ClassKey;
                string previousClass = CharacterCreationCatalog.Classes.First(choice => choice.Key != targetClass).Key;
                SetSelectedMemberClass(previousClass);
                partySetupScreen.Refresh();
                partySetupScreen.AdvancePresentation(1f);
                SetSelectedMemberClass(targetClass);
                partySetupScreen.Refresh();
                partySetupScreen.AdvancePresentation(0.08f);
                motion = "transition";
            }
            PartySetupFolioSnapshot snapshot = partySetupScreen.CaptureMotionSnapshot();
            if (!snapshot.Visible || (transition && (!snapshot.TransitionActive || snapshot.PreviousPortraitAlpha <= 0f))
                || (!transition && (snapshot.TransitionActive || snapshot.PreviousPortraitAlpha != 0f))
                || (state.ReducedMotion && (!snapshot.ReducedMotion || snapshot.AmbientTime != 0f || snapshot.AccentAlpha != 0f)))
                throw new InvalidOperationException("Party Setup capture did not reach its requested motion state: " + motion);
            Debug.Log(VersionInfo.ProductName + " party setup capture motion: " + motion + ".");
        }

        private static string PartySetupSmokeOption(string[] args, string option, string fallback)
        {
            int index = Array.FindIndex(args, arg => string.Equals(arg, option, StringComparison.OrdinalIgnoreCase));
            if (index < 0) return fallback;
            if (index + 1 >= args.Length || string.IsNullOrWhiteSpace(args[index + 1]) || args[index + 1].StartsWith("-"))
                throw new InvalidOperationException("Party Setup capture option has no value: " + option);
            return args[index + 1];
        }
    }
}
