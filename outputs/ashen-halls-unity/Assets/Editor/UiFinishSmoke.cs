using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AshenHalls.Editor
{
    public static class UiFinishSmoke
    {
        public static void RunOrThrow()
        {
            OrnamentsShareAndReleaseArtWithoutInterceptingInput();
            PauseNavigationRetainsUsableFocus();
            ExpandedExplorationPartySubtitlesRemainVisible();
        }

        private static void OrnamentsShareAndReleaseArtWithoutInterceptingInput()
        {
            Require(!Application.isPlaying, "ornament lifetime smoke runs in Edit Mode");
            string path = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Docs", "ArtReferences", RuntimeArtManifest.UiHearthDivider));
            Require(File.Exists(path), "the actual UI divider PNG exists: " + path);
            Require(UnityEngine.Object.FindObjectsByType<UiOrnament>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length == 0,
                "ornament lifetime fixture starts without unrelated owners");
            GameObject host = new GameObject("UI finish ornament lifetime smoke");
            try
            {
                RectTransform first = UiOrnament.Add(host.transform, "First decoration");
                RectTransform second = UiOrnament.Add(host.transform, "Second decoration");
                Image firstImage = first.GetComponent<Image>();
                Image secondImage = second.GetComponent<Image>();
                Sprite sharedSprite = firstImage.sprite;
                Require(firstImage.enabled && secondImage.enabled && sharedSprite != null, "both decorations decode the actual divider art");
                Texture2D sharedTexture = sharedSprite.texture;
                Require(secondImage.sprite == sharedSprite && secondImage.sprite.texture == sharedTexture,
                    "decorations share one sprite and texture");
                Require(!firstImage.raycastTarget && !secondImage.raycastTarget, "decorations cannot intercept clicks");

                UnityEngine.Object.DestroyImmediate(first.gameObject);
                Require(sharedSprite != null && sharedTexture != null && secondImage.sprite == sharedSprite,
                    "removing one decoration preserves the remaining owner's art");
                UnityEngine.Object.DestroyImmediate(second.gameObject);
                Require(sharedSprite == null && sharedTexture == null, "removing the final decoration releases both shared Unity resources");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(host);
            }
        }

        private static void PauseNavigationRetainsUsableFocus()
        {
            EventSystem eventSystem = UiRuntime.EnsureEventSystemReady();
            GameObject previousSelection = eventSystem.currentSelectedGameObject;
            GameObject host = new GameObject("UI finish pause navigation smoke");
            try
            {
                PauseMenuView view = new PauseMenuView
                {
                    Title = "Menu",
                    SettingsOpen = true,
                    ShowRetreat = true,
                    RetreatEnabled = true,
                    MusicLine = "Music 50%"
                };
                PauseMenuScreen screen = host.AddComponent<PauseMenuScreen>();
                screen.Bind(new PauseMenuScreenBindings { View = () => view });
                screen.SetVisible(true);
                screen.Refresh();

                Button resume = Field<Button>(screen, "continueButton");
                Button save = Field<Button>(screen, "saveButton");
                Button load = Field<Button>(screen, "loadButton");
                Button settings = Field<Button>(screen, "settingsButton");
                Button retreat = Field<Button>(screen, "returnButton");
                Button newGame = Field<Button>(screen, "newButton");
                Button music = Field<Button>(screen, "musicValueButton");
                Button motion = Field<Button>(screen, "reducedMotionButton");
                Require(eventSystem.currentSelectedGameObject == resume.gameObject, "opening pause focuses Continue");

                eventSystem.SetSelectedGameObject(save.gameObject);
                Move(eventSystem, MoveDirection.Right);
                Require(eventSystem.currentSelectedGameObject == load.gameObject, "right from Save reaches Load");
                Move(eventSystem, MoveDirection.Left);
                Require(eventSystem.currentSelectedGameObject == save.gameObject, "left from Load reaches Save");

                eventSystem.SetSelectedGameObject(settings.gameObject);
                Move(eventSystem, MoveDirection.Down);
                Require(eventSystem.currentSelectedGameObject == retreat.gameObject, "available Retreat is reachable below Settings");
                view.RetreatEnabled = false;
                screen.Refresh();
                Require(eventSystem.currentSelectedGameObject == settings.gameObject, "losing supplies moves focus off disabled Retreat");
                Move(eventSystem, MoveDirection.Down);
                Require(eventSystem.currentSelectedGameObject == newGame.gameObject, "navigation skips unavailable Retreat");
                Move(eventSystem, MoveDirection.Up);
                Require(eventSystem.currentSelectedGameObject == settings.gameObject, "up navigation also skips unavailable Retreat");

                eventSystem.SetSelectedGameObject(music.gameObject);
                view.MusicLine = "Music 25%";
                screen.Refresh();
                Require(eventSystem.currentSelectedGameObject == music.gameObject, "refresh preserves a usable settings selection");
                Require(music.GetComponentInChildren<Text>().text == "Music 25%", "settings values refresh without moving focus");

                eventSystem.SetSelectedGameObject(motion.gameObject);
                view.SettingsOpen = false;
                screen.Refresh();
                Require(eventSystem.currentSelectedGameObject == settings.gameObject, "collapsing settings restores focus to the Settings action");
                Move(eventSystem, MoveDirection.Down);
                Move(eventSystem, MoveDirection.Down);
                Require(eventSystem.currentSelectedGameObject == resume.gameObject, "closed settings navigation loops back to Continue");
                screen.SetVisible(false);
            }
            finally
            {
                eventSystem.SetSelectedGameObject(previousSelection);
                UnityEngine.Object.DestroyImmediate(host);
            }
        }

        private static void ExpandedExplorationPartySubtitlesRemainVisible()
        {
            GameObject host = new GameObject("UI finish exploration subtitle smoke");
            try
            {
                string[] classLines = { "Human Warrior • L12", "Dusk-elf Ranger • L12", "Stoneborn Cleric • L12", "Ashling Mage • L12" };
                ExplorationHudPartyMemberView[] party = new ExplorationHudPartyMemberView[classLines.Length];
                for (int i = 0; i < party.Length; i++)
                    party[i] = new ExplorationHudPartyMemberView { Name = "Member " + i, ClassLine = classLines[i], Hp = 10, MaxHp = 20 };
                ExplorationHudView view = new ExplorationHudView { DetailsOpen = true, Party = party };
                ExplorationHudScreen screen = host.AddComponent<ExplorationHudScreen>();
                screen.Bind(new ExplorationHudScreenBindings { View = () => view });
                screen.SetVisible(true);
                MethodInfo refresh = typeof(ExplorationHudScreen).GetMethod("Refresh", BindingFlags.Instance | BindingFlags.NonPublic,
                    null, new[] { typeof(float), typeof(float) }, null);
                Require(refresh != null, "exploration fixture can refresh at the target display size");
                RectTransform rail = Field<RectTransform>(screen, "sidePanel");
                foreach (Vector2Int size in new[] { new Vector2Int(960, 600), new Vector2Int(1024, 768), new Vector2Int(1280, 720), new Vector2Int(1600, 900), new Vector2Int(1920, 1080), new Vector2Int(2048, 1152) })
                {
                    refresh.Invoke(screen, new object[] { (float)size.x, (float)size.y });
                    Canvas.ForceUpdateCanvases();
                    for (int i = 0; i < party.Length; i++)
                    {
                        Text subtitle = rail.Find("Party Row " + i + "/Class").GetComponent<Text>();
                        Require(subtitle.gameObject.activeInHierarchy && subtitle.text == classLines[i], "party subtitle remains available at " + size);
                        TextGenerationSettings settings = subtitle.GetGenerationSettings(subtitle.rectTransform.rect.size);
                        subtitle.cachedTextGenerator.Populate(subtitle.text, settings);
                        Require(subtitle.cachedTextGenerator.characterCountVisible > 0, "party subtitle actually generates visible characters at " + size);
                        settings.verticalOverflow = VerticalWrapMode.Overflow;
                        float textHeight = subtitle.cachedTextGeneratorForLayout.GetPreferredHeight(subtitle.text, settings) / subtitle.pixelsPerUnit;
                        Require(textHeight <= subtitle.rectTransform.rect.height + 0.5f, "complete party class and level fit without vertical truncation at " + size);
                    }
                }
                screen.SetVisible(false);
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        private static void Move(EventSystem eventSystem, MoveDirection direction)
        {
            GameObject selected = eventSystem.currentSelectedGameObject;
            Require(selected != null, "directional navigation has a selected action");
            ExecuteEvents.Execute(selected, new AxisEventData(eventSystem) { moveDir = direction }, ExecuteEvents.moveHandler);
        }

        private static T Field<T>(object owner, string name) where T : class
        {
            FieldInfo field = owner.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            T value = field == null ? null : field.GetValue(owner) as T;
            Require(value != null, "expected control exists: " + name);
            return value;
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("UI finish smoke failed: " + message);
        }
    }
}
