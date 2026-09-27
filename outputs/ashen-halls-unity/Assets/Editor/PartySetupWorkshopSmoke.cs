using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AshenHalls.Editor
{
    public static class PartySetupWorkshopSmoke
    {
        private const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;

        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " party setup workshop smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " party setup workshop smoke failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            try
            {
                AshenHallsGame game = OpenGame();
                AssertNewGameMusicRoute(game);
                Stage(game, "human", "warrior", 0);
                PartySetupScreen screen = Field<PartySetupScreen>(game, "partySetupScreen");
                GameState state = Field<GameState>(game, "state");
                Require(Field<bool>(game, "visualSmokeSaveBlocked"), "capture staging blocks campaign persistence");
                Require(state.Mode == GameMode.Muster, "capture staging reaches the real setup screen");
                Require(screen.ViewCanvas.gameObject.activeInHierarchy, "setup canvas is visible");
                // Edit-mode lifecycle checks use the same ensured host as UiRuntime:
                // Unity does not register EventSystem.current outside Play Mode.
                EventSystem navigation = EventSystem.current ?? (!Application.isPlaying ? UiRuntime.EnsureEventSystemReady() : null);
                Require(navigation != null && navigation.currentSelectedGameObject == Button(screen, "Roster 0").gameObject,
                    "opening setup gives keyboard navigation a visible roster starting point");
                AssertQuietRefreshAndUnchangedChoices(game, screen, state);
                string untouchedFirst = JsonUtility.ToJson(state.Party[0]);
                Click(screen, "Roster 2");
                Require(Field<int>(game, "selectedBuilderIndex") == 2, "native roster control selects its own character");
                string selectedRace = state.Party[2].Race;
                string selectedClass = state.Party[2].ClassKey;
                string focusRace = CharacterCreationCatalog.Races.First(choice => choice.Key != selectedRace).Key;
                Button raceFocus = Button(screen, "Race " + focusRace);
                string raceMarkers = RaceSelectionMarkers(screen);
                navigation.SetSelectedGameObject(raceFocus.gameObject);
                screen.Refresh();
                Require(navigation.currentSelectedGameObject == raceFocus.gameObject,
                    "refresh retains native keyboard focus on an unchosen race");
                Require(raceFocus.colors.selectedColor != raceFocus.colors.normalColor,
                    "keyboard focus has a distinct visible color from an unchosen race card");
                Require(state.Party[2].Race == selectedRace && state.Party[2].ClassKey == selectedClass
                    && RaceSelectionMarkers(screen) == raceMarkers,
                    "browsing with keyboard focus leaves the chosen race, class and selection markers unchanged");
                InputField name = screen.ViewCanvas.GetComponentsInChildren<InputField>(true).Single(field => field.name == "Name Field");
                name.onEndEdit.Invoke("Bram Moonwatcher");
                screen.Refresh();
                Require(state.Party[2].Name == "Bram Moonwatcher", "native name submission updates selected character");
                foreach (CharacterCreationChoice race in CharacterCreationCatalog.Races)
                {
                    Click(screen, "Race " + race.Key);
                    foreach (CharacterCreationChoice vocation in CharacterCreationCatalog.Classes)
                    {
                        Click(screen, "Class " + vocation.Key);
                        Require(state.Party[2].Race == race.Key && state.Party[2].ClassKey == vocation.Key,
                            "native choice reaches " + race.Key + " / " + vocation.Key);
                        Require(state.Party[2].Name == "Bram Moonwatcher", "choice changes retain the authored name");
                        Require(state.Party[2].Stats.Total == 50, "choice changes retain a valid starting point budget");
                        RawImage portrait = Field<RawImage>(screen, "selectedPortrait");
                        Require(portrait.enabled && portrait.texture != null
                            && portrait.texture.name == CharacterCreationCatalog.PortraitAtlasFile(race.Key, vocation.Key),
                            "large portrait loads the selected race atlas");
                        int cell = CharacterCreationCatalog.PortraitCell(race.Key, vocation.Key);
                        float centerX = (cell % 4 + 0.5f) / 4f;
                        float centerY = 1f - (cell / 4 + 0.5f) / 2f;
                        Require(Mathf.Abs(portrait.uvRect.center.x - centerX) < 0.0001f
                            && Mathf.Abs(portrait.uvRect.center.y - centerY) < 0.0001f,
                            "large portrait displays the selected class cell");
                    }
                }
                Require(JsonUtility.ToJson(state.Party[0]) == untouchedFirst, "editing one recruit preserves another recruit");
                AssertFolioPresentation(screen, state);
                Click(screen, "Race dusk elf");
                Click(screen, "Class ranger");
                navigation.SetSelectedGameObject(Button(screen, "Class ranger").gameObject);
                Click(screen, "Details Tab");
                Require(navigation.currentSelectedGameObject == Button(screen, "Details Tab").gameObject,
                    "hiding the focused identity card moves focus to the visible details tab");
                navigation.SetSelectedGameObject(Button(screen, "Stat Down 0").gameObject);
                Click(screen, "Identity Tab");
                Require(navigation.currentSelectedGameObject == Button(screen, "Identity Tab").gameObject,
                    "hiding the focused attribute control moves focus to the visible identity tab");
                Click(screen, "Details Tab");
                Click(screen, "Stat Down 0");
                Require(state.Party[2].Stats.Total == 49, "attribute decrement releases a point");
                Require(!Button(screen, "Begin").interactable, "Begin stays unavailable while a recruit has an unspent point");
                Click(screen, "Stat Up 0");
                Require(state.Party[2].Stats.Total == 50 && Button(screen, "Begin").interactable, "allocating the point restores Begin");
                string[] chosenParty = state.Party.Select(RecruitIdentity).ToArray();
                Click(screen, "Begin");
                Invoke(game, "LateUpdate");
                Require(state.Mode == GameMode.Explore, "native Begin enters the campaign");
                Require(state.Party[2].Race == "dusk elf" && state.Party[2].ClassKey == "ranger"
                    && state.Party[2].Name == "Bram Moonwatcher", "Begin retains the customized recruit");
                for (int i = 0; i < state.Party.Count; i++)
                    Require(RecruitIdentity(state.Party[i]) == chosenParty[i], "Begin retains recruit " + i + " without quick-start replacement");
                Require(Field<bool>(game, "visualSmokeSaveBlocked"), "Begin cannot save the capture party");
                AssertWorkshopDisposal(game, screen);
            }
            finally
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            }
        }

        internal static AshenHallsGame OpenGame()
        {
            if (!Application.isBatchMode) throw new InvalidOperationException("Party setup review requires batch mode because it replaces the scene.");
            EditorSceneManager.OpenScene("Assets/Scenes/Main.unity", OpenSceneMode.Single);
            AshenHallsGame game = UnityEngine.Object.FindFirstObjectByType<AshenHallsGame>();
            Require(game != null, "Main scene includes the game");
            Invoke(game, "Awake");
            Require(string.IsNullOrEmpty(Field<string>(game, "launchError")), "game initializes without a launch error");
            return game;
        }

        internal static void Stage(AshenHallsGame game, string race, string classKey, int member, bool details = false, bool reducedMotion = false)
        {
            string[] args = new[] { "-ashen-party-setup-smoke", "-ashen-party-race", race,
                "-ashen-party-class", classKey, "-ashen-party-member", member.ToString(), "-ashen-seed", "2828" };
            if (details) args = args.Concat(new[] { "-ashen-party-details" }).ToArray();
            if (reducedMotion) args = args.Concat(new[] { "-ashen-party-reduced-motion" }).ToArray();
            Require((bool)Invoke(game, "TryApplyPartySetupVisualSmokeLaunch", (object)args), "setup capture hook accepts staging");
            Invoke(game, "LateUpdate");
            PartySetupScreen screen = Field<PartySetupScreen>(game, "partySetupScreen");
            Require(Field<RectTransform>(screen, "identityPage").gameObject.activeInHierarchy == !details
                && Field<RectTransform>(screen, "detailsPage").gameObject.activeInHierarchy == details,
                "capture staging displays its requested " + (details ? "details" : "identity") + " tab after screen synchronization");
        }

        internal static T Field<T>(object owner, string name)
        {
            FieldInfo field = owner.GetType().GetField(name, PrivateInstance);
            if (field == null) throw new InvalidOperationException("Review field is missing: " + name);
            return (T)field.GetValue(owner);
        }

        internal static object Invoke(object owner, string name, params object[] args)
        {
            MethodInfo method = owner.GetType().GetMethod(name, PrivateInstance);
            if (method == null) throw new InvalidOperationException("Review method is missing: " + name);
            return method.Invoke(owner, args);
        }

        private static Button Button(PartySetupScreen screen, string name)
        {
            Button button = screen.ViewCanvas.GetComponentsInChildren<Button>(true).SingleOrDefault(item => item.name == name);
            Require(button != null, "native button exists: " + name);
            return button;
        }

        private static void Click(PartySetupScreen screen, string name)
        {
            Button button = Button(screen, name);
            Require(button.gameObject.activeInHierarchy && button.interactable, "native button is available: " + name);
            button.onClick.Invoke();
            screen.Refresh();
        }

        internal static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("Party Setup: " + message);
        }

        private static string RecruitIdentity(PartyMember member)
        {
            return member.Id + "|" + member.Name + "|" + member.Race + "|" + member.ClassKey + "|" + JsonUtility.ToJson(member.Stats);
        }

        private static string RaceSelectionMarkers(PartySetupScreen screen)
        {
            return string.Join("|", screen.ViewCanvas.GetComponentsInChildren<Text>(true)
                .Where(text => text.name == "Race Selection").Select(text => text.text));
        }

        private static void AssertQuietRefreshAndUnchangedChoices(AshenHallsGame game, PartySetupScreen screen, GameState state)
        {
            bool wasMuted = state.SfxMuted;
            int previousVolume = state.SfxVolumePercent;
            bool wasCaptureBlocked = Field<bool>(game, "visualSmokeSaveBlocked");
            SetField(game, "visualSmokeSaveBlocked", false);
            state.SfxMuted = false;
            state.SfxVolumePercent = 75;
            try
            {
                Invoke(game, "ApplyAudioSettings");
                string before = AudioPlaybackSnapshot(game);
                screen.Refresh();
                screen.Refresh();
                Click(screen, "Race " + state.Party[0].Race);
                Click(screen, "Class " + state.Party[0].ClassKey);
                Click(screen, "Roster 0");
                Click(screen, "Identity Tab");
                Require(AudioPlaybackSnapshot(game) == before,
                    "refresh and unchanged race, class, roster and tab choices do not replay sound effects");

                string originalRace = state.Party[0].Race;
                string alternateRace = CharacterCreationCatalog.Races.First(choice => choice.Key != originalRace).Key;
                Click(screen, "Race " + alternateRace);
                Require(AudioPlaybackSnapshot(game) != before, "a real race change produces an audible selection cue");
                Click(screen, "Race " + originalRace);
                state.SfxMuted = true;
                Invoke(game, "ApplyAudioSettings");
                before = AudioPlaybackSnapshot(game);
                Click(screen, "Race " + alternateRace);
                Require(state.Party[0].Race == alternateRace && AudioPlaybackSnapshot(game) == before,
                    "muted customization still changes the recruit without emitting a sound cue");
                Click(screen, "Race " + originalRace);
                state.SfxMuted = false;
                SetField(game, "visualSmokeSaveBlocked", true);
                before = AudioPlaybackSnapshot(game);
                Click(screen, "Race " + alternateRace);
                Require(state.Party[0].Race == alternateRace && AudioPlaybackSnapshot(game) == before,
                    "capture staging keeps real selection changes silent");
                Click(screen, "Race " + originalRace);
            }
            finally
            {
                state.SfxMuted = wasMuted;
                state.SfxVolumePercent = previousVolume;
                SetField(game, "visualSmokeSaveBlocked", wasCaptureBlocked);
                Invoke(game, "ApplyAudioSettings");
            }
        }

        private static void AssertNewGameMusicRoute(AshenHallsGame game)
        {
            GameState titleState = Field<GameState>(game, "state");
            Require(titleState.Mode == GameMode.Tavern, "music route review begins on the real title screen");
            bool previousMuted = titleState.MusicMuted;
            int previousVolume = titleState.MusicVolumePercent;
            SetField(game, "visualSmokeSaveBlocked", true);
            try
            {
                AudioClip title = (AudioClip)Invoke(game, "DesiredMusicClip");
                Require(title != null && title.name == "tavern_storm_hearth_ensemble_loop",
                    "the title screen initially selects its own overture");
                titleState.MusicMuted = false;
                titleState.MusicVolumePercent = 50;
                Invoke(game, "StartNewGame");
                GameState workshopState = Field<GameState>(game, "state");
                AudioClip workshopMaster = Resources.Load<AudioClip>("Audio/Music/muster_by_firelight_loop");
                Require(workshopState.Mode == GameMode.Muster && workshopMaster != null
                    && ReferenceEquals(Invoke(game, "DesiredMusicClip"), workshopMaster),
                    "New Game changes the live music director from the title overture to the imported workshop score");
                Require(Mathf.Abs(workshopMaster.length - 53.3333f) < 0.05f,
                    "the live workshop score uses the complete authored 53-second arrangement");
                Require(workshopState.MusicVolumePercent == 50 && !workshopState.MusicMuted,
                    "New Game retains the player's chosen music preference");
                Invoke(game, "UpdateTavernMusic");
                AudioSource source = Field<AudioSource>(game, "musicSource");
                Require(source != null && ReferenceEquals(source.clip, workshopMaster),
                    "the live music transport adopts the selected workshop master");
                // Complete the real intro envelope without waiting on an editor frame loop.
                float introDuration = Field<float>(game, "activeMusicIntroFadeDuration");
                SetField(game, "musicIntroFadeStartedAt", Time.unscaledTime - introDuration - 1f);
                Invoke(game, "ApplyAudioSettings");
                Require(Mathf.Abs(source.volume - TitleAudioRules.MusterMusicSourceGain * 0.50f) < 0.0001f,
                    "workshop playback applies its quieter source gain and the player's 50-percent preference");
                Dictionary<string, AudioClip> clips = Field<Dictionary<string, AudioClip>>(game, "soundClips");
                foreach (string key in PartySetupAudioRules.CueKeys)
                    Require(clips.TryGetValue(key, out AudioClip clip)
                        && ReferenceEquals(clip, Resources.Load<AudioClip>("Audio/Sfx/" + key)),
                        "the initialized game resolves the imported workshop sound " + key);
            }
            finally
            {
                GameState current = Field<GameState>(game, "state");
                current.MusicMuted = previousMuted;
                current.MusicVolumePercent = previousVolume;
                Invoke(game, "ApplyAudioSettings");
            }
        }

        private static string AudioPlaybackSnapshot(AshenHallsGame game)
        {
            return string.Join("|", Field<Dictionary<string, int>>(game, "sfxCuePlaybackSerial")
                .OrderBy(pair => pair.Key, StringComparer.Ordinal).Select(pair => pair.Key + ":" + pair.Value));
        }

        private static void AssertFolioPresentation(PartySetupScreen screen, GameState state)
        {
            bool previousReducedMotion = state.ReducedMotion;
            try
            {
                state.ReducedMotion = false;
                screen.Refresh();
                screen.AdvancePresentation(1f);
                PartySetupFolioSnapshot settled = screen.CaptureMotionSnapshot();
                Require(settled.Visible && !settled.ReducedMotion && !settled.TransitionActive,
                    "visible folio settles its opening presentation");
                Require(settled.DecorativeGraphicCount > 0, "folio exposes its authored decorative layer");
                Graphic[] decorations = screen.ViewCanvas.GetComponentsInChildren<Graphic>(true)
                    .Where(graphic => graphic is FolioLightGraphic || graphic.name == "Previous Portrait Reveal"
                        || graphic.name.StartsWith("Margin Ember ", StringComparison.Ordinal)
                        || graphic.name.StartsWith("Portrait Corner ", StringComparison.Ordinal)).ToArray();
                Require(decorations.Length == settled.DecorativeGraphicCount
                    && decorations.All(graphic => !graphic.raycastTarget && graphic.GetComponent<Selectable>() == null),
                    "every actual decorative graphic remains outside pointer and keyboard input");
                Click(screen, "Class warrior");
                screen.AdvancePresentation(0.08f);
                PartySetupFolioSnapshot changing = screen.CaptureMotionSnapshot();
                Require(changing.TransitionActive && changing.PreviousPortraitAlpha > 0f && changing.PreviousPortraitAlpha < 1f
                    && changing.AccentAlpha > 0f, "a real class change animates its previous painting and selection accent");
                screen.Refresh();
                screen.Refresh();
                Click(screen, "Class warrior");
                PartySetupFolioSnapshot unchanged = screen.CaptureMotionSnapshot();
                Require(Mathf.Approximately(unchanged.PreviousPortraitAlpha, changing.PreviousPortraitAlpha)
                    && Mathf.Approximately(unchanged.AccentAlpha, changing.AccentAlpha)
                    && Mathf.Approximately(unchanged.AmbientTime, changing.AmbientTime),
                    "refresh and an unchanged choice do not restart or advance animation");
                Click(screen, "Class mage");
                Click(screen, "Class priest");
                Click(screen, "Class wizard");
                screen.AdvancePresentation(1f);
                PartySetupFolioSnapshot completed = screen.CaptureMotionSnapshot();
                Require(!completed.TransitionActive && completed.PreviousPortraitAlpha == 0f && completed.AccentAlpha == 0f,
                    "rapid class changes settle without leaving a previous portrait or accent behind");
                Require(completed.AmbientTime > settled.AmbientTime, "the visible ambient presentation advances");
                Require(completed.DecorativeGraphicCount == settled.DecorativeGraphicCount,
                    "rapid customization reuses a bounded decorative layer");
                RawImage portrait = Field<RawImage>(screen, "selectedPortrait");
                int wizardCell = CharacterCreationCatalog.PortraitCell(state.Party[2].Race, "wizard");
                Require(Mathf.Abs(portrait.uvRect.center.x - (wizardCell % 4 + 0.5f) / 4f) < 0.0001f
                    && Mathf.Abs(portrait.uvRect.center.y - (1f - (wizardCell / 4 + 0.5f) / 2f)) < 0.0001f,
                    "rapid changes finish on the latest requested portrait cell");

                state.ReducedMotion = true;
                screen.Refresh();
                Click(screen, "Class warlock");
                PartySetupFolioSnapshot reduced = screen.CaptureMotionSnapshot();
                screen.AdvancePresentation(5f);
                PartySetupFolioSnapshot reducedLater = screen.CaptureMotionSnapshot();
                Require(reduced.ReducedMotion && !reduced.TransitionActive && reduced.PreviousPortraitAlpha == 0f
                    && reduced.AccentAlpha == 0f && reduced.AmbientTime == 0f,
                    "Reduced Motion makes character changes immediate and removes ambient animation");
                Require(reducedLater.AmbientTime == 0f && !reducedLater.TransitionActive
                    && Mathf.Approximately(reducedLater.FirelightAlpha, reduced.FirelightAlpha)
                    && Mathf.Approximately(reducedLater.SelectionGlowAlpha, reduced.SelectionGlowAlpha),
                    "Reduced Motion remains visually still across presentation updates");

                state.ReducedMotion = false;
                screen.Refresh();
                Click(screen, "Class ranger");
                screen.AdvancePresentation(0.08f);
                screen.SetVisible(false);
                PartySetupFolioSnapshot hidden = screen.CaptureMotionSnapshot();
                screen.AdvancePresentation(5f);
                PartySetupFolioSnapshot hiddenLater = screen.CaptureMotionSnapshot();
                Require(!hidden.Visible && !hidden.TransitionActive && hidden.PreviousPortraitAlpha == 0f && hidden.AccentAlpha == 0f,
                    "closing the workshop clears active character transitions");
                Require(!hiddenLater.Visible && Mathf.Approximately(hiddenLater.AmbientTime, hidden.AmbientTime),
                    "a hidden workshop does not advance its ambient presentation");
                screen.SetVisible(true);
                screen.Refresh();
                screen.AdvancePresentation(1f);
                Require(screen.CaptureMotionSnapshot().Visible && !screen.CaptureMotionSnapshot().TransitionActive,
                    "reopening the workshop settles cleanly after hiding mid-transition");
                Require(screen.CaptureMotionSnapshot().DecorativeGraphicCount == settled.DecorativeGraphicCount,
                    "reopening the workshop does not allocate duplicate decoration");
            }
            finally
            {
                state.ReducedMotion = previousReducedMotion;
                screen.SetVisible(true);
                screen.Refresh();
                screen.AdvancePresentation(1f);
            }
        }

        private static void SetField(object owner, string name, object value)
        {
            FieldInfo field = owner.GetType().GetField(name, PrivateInstance);
            if (field == null) throw new InvalidOperationException("Review field is missing: " + name);
            field.SetValue(owner, value);
        }

        private static void AssertWorkshopDisposal(AshenHallsGame game, PartySetupScreen screen)
        {
            Texture2D backdrop = (Texture2D)Invoke(game, "PartySetupWorkshopBackdrop");
            Require(backdrop != null && backdrop.name == RuntimeArtManifest.CharacterWorkshopBackdrop,
                "workshop loads its exact approved landscape painting");
            Require(ReferenceEquals(backdrop, Invoke(game, "PartySetupWorkshopBackdrop")),
                "repeated workshop backdrop requests reuse the cached texture");
            Texture2D[] portraits = Field<Dictionary<string, Texture2D>>(game, "partySetupPortraitAtlases").Values.ToArray();
            Require(portraits.Length == 5 && portraits.All(texture => texture != null), "the complete portrait cache is available before disposal");
            Canvas canvas = screen.ViewCanvas;
            Require(!canvas.gameObject.activeInHierarchy, "entering the campaign hides the workshop before disposal");
            // This edit-mode fixture invokes Awake explicitly. The ordinary game
            // component likewise needs its real teardown callback invoked explicitly;
            // Unity only drives the ExecuteAlways canvas owner automatically here.
            Invoke(game, "OnDestroy");
            UnityEngine.Object.DestroyImmediate(game.gameObject);
            Require(canvas == null, "disposing the game releases the workshop's owned root canvas");
            Require(backdrop == null && portraits.All(texture => texture == null),
                "disposing the game releases cached workshop backdrop and portrait textures");
        }
    }
}
