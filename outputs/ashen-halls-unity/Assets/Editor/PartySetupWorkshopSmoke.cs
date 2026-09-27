using System;
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

        internal static void Stage(AshenHallsGame game, string race, string classKey, int member, bool details = false)
        {
            string[] args = new[] { "-ashen-party-setup-smoke", "-ashen-party-race", race,
                "-ashen-party-class", classKey, "-ashen-party-member", member.ToString(), "-ashen-seed", "2828" };
            if (details) args = args.Concat(new[] { "-ashen-party-details" }).ToArray();
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
    }
}
