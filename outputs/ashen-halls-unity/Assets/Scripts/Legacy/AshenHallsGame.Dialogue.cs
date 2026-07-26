using UnityEngine;

namespace AshenHalls
{
    public sealed partial class AshenHallsGame
    {
        private void EnsureDialogueScreen()
        {
            if (dialogueScreen != null && dialogueScreen.IsReady) return;
            if (dialogueScreen != null)
            {
                Destroy(dialogueScreen.gameObject);
                dialogueScreen = null;
            }
            GameObject screen = new GameObject("Dialogue Screen");
            screen.transform.SetParent(transform, false);
            DialogueScreen created = screen.AddComponent<DialogueScreen>();
            try
            {
                created.Bind(new DialogueScreenBindings
                {
                    View = BuildDialogueScreenView,
                    Advance = AdvanceDialogue,
                    Choose = ChooseDialogueChoice
                });
                created.SetVisible(false);
                dialogueScreen = created;
            }
            catch
            {
                created.SetVisible(false);
                screen.SetActive(false);
                Destroy(screen);
                throw;
            }
        }

        private void SyncDialogueScreen()
        {
            bool visible = state != null && CurrentUiOverlay() == UiOverlay.Dialogue && !ShouldShowStartupSplash();
            if (visible && (dialogueScreen == null || !dialogueScreen.IsReady))
            {
                TryInitializePresentationScreen("Dialogue recovery", EnsureDialogueScreen, false);
            }
            if (dialogueScreen == null) return;
            if (!visible)
            {
                dialogueScreen.SetVisible(false);
                return;
            }

            bool refresh = !dialogueScreen.CanOwnModal
                || ShouldRefreshPresentation(ref lastDialogueRefreshKey, DialogueRefreshKey());
            if (refresh)
            {
                dialogueScreen.SetVisible(false);
                dialogueScreen.SetSuppressedByImguiFallback(false);
                try
                {
                    dialogueScreen.Refresh();
                    dialogueScreen.SetVisible(true);
                    Canvas.ForceUpdateCanvases();
                }
                catch (System.Exception ex)
                {
                    dialogueScreen.SetVisible(false);
                    Debug.LogException(new System.InvalidOperationException(VersionInfo.ProductName + " dialogue refresh failed; using recovery popup.", ex));
                }
            }

            if (!dialogueScreen.CanOwnModal) dialogueScreen.SetVisible(false);
        }

        private string DialogueRefreshKey()
        {
            return "dialogue=" + (dialogueTitle ?? "").GetHashCode()
                + ":" + (dialogueSpeaker ?? "").GetHashCode()
                + ":" + (dialogueBody ?? "").GetHashCode()
                + ":" + dialoguePageIndex
                + ":" + dialogueFocus
                + ":" + DialogueChoiceRefreshHash()
                + ":" + dialogueAccentColor.ToString();
        }

        private DialogueScreenView BuildDialogueScreenView()
        {
            int portraitIndex = NpcPortraitCatalog.PortraitIndex(dialogueFocus, dialogueSpeaker);
            return new DialogueScreenView
            {
                Title = dialogueTitle,
                Speaker = dialogueSpeaker,
                Focus = ObjectName(dialogueFocus),
                Body = CurrentDialoguePage(),
                AccentHex = DialogueAccentHex(),
                PageLabel = DialoguePageLabel(),
                ContinueLabel = DialogueContinueLabel(),
                PortraitTexture = portraitIndex >= 0 && IsNpcPortraitAtlas() ? npcPortraitAtlas : null,
                PortraitSource = portraitIndex >= 0 && IsNpcPortraitAtlas() ? NpcPortraitAtlasCell(portraitIndex) : Rect.zero,
                Choices = IsDialogueChoicePage() ? dialogueChoices : System.Array.Empty<DialogueChoiceView>()
            };
        }

        private int DialogueChoiceRefreshHash()
        {
            if (!IsDialogueChoicePage()) return 0;
            int hash = 17;
            for (int i = 0; i < dialogueChoices.Length; i++)
            {
                DialogueChoiceView choice = dialogueChoices[i];
                hash = unchecked(hash * 31 + (choice?.Id ?? "").GetHashCode());
                hash = unchecked(hash * 31 + (choice?.Label ?? "").GetHashCode());
                hash = unchecked(hash * 31 + (choice != null && choice.Enabled ? 1 : 0));
            }
            return hash;
        }

        private string DialogueAccentHex()
        {
            Color accent = DialoguePresentationRules.ReadableAccent(dialogueAccentColor.a <= 0f ? gold : dialogueAccentColor);
            return ColorUtility.ToHtmlStringRGB(accent);
        }

        private bool NeedsEmergencyDialogueFallback()
        {
            bool primaryDialogueReady = dialogueScreen != null
                && dialogueScreen.CanOwnModal;
            return !primaryDialogueReady
                && state != null
                && CurrentUiOverlay() == UiOverlay.Dialogue
                && !ShouldShowStartupSplash();
        }

        private void DrawEmergencyDialogueFallback()
        {
            if (!NeedsEmergencyDialogueFallback()) return;

            Color accent = DialoguePresentationRules.ReadableAccent(dialogueAccentColor.a <= 0f ? gold : dialogueAccentColor);
            bool hasChoices = IsDialogueChoicePage();
            DrawRect(new Rect(0f, 0f, Screen.width, Screen.height), Hex("020303", 0.72f));
            float panelW = Mathf.Min(Mathf.Max(560f, Screen.width * 0.56f), Screen.width - 48f);
            float panelH = Mathf.Min(hasChoices ? 430f : 330f, Screen.height - 64f);
            Rect panel = new Rect((Screen.width - panelW) * 0.5f, (Screen.height - panelH) * 0.5f, panelW, panelH);
            DrawRect(panel, Hex("1a2026", 0.99f));
            DrawBorder(panel, accent, 2);
            DrawRect(new Rect(panel.x, panel.y, 5f, panel.height), accent);

            GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
            {
                font = UiRuntime.DialogueEmphasisFont,
                fontSize = 17,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleLeft
            };
            titleStyle.normal.textColor = DialoguePresentationRules.ReadableTextAccent(accent);
            GUIStyle speakerStyle = new GUIStyle(titleStyle) { fontSize = 20 };
            speakerStyle.normal.textColor = ink;
            GUIStyle bodyStyle = new GUIStyle(GUI.skin.label)
            {
                font = UiRuntime.DialogueFont,
                fontSize = 15,
                wordWrap = true,
                alignment = TextAnchor.UpperLeft,
                clipping = TextClipping.Clip
            };
            bodyStyle.normal.textColor = ink;
            GUIStyle dialogueButtonStyle = new GUIStyle(buttonStyle)
            {
                font = UiRuntime.DialogueEmphasisFont,
                fontStyle = FontStyle.Normal
            };

            GUI.Label(new Rect(panel.x + 24f, panel.y + 14f, panel.width - 48f, 24f), string.IsNullOrWhiteSpace(dialogueTitle) ? "Midgaard" : dialogueTitle, titleStyle);
            GUI.Label(new Rect(panel.x + 24f, panel.y + 52f, panel.width - 48f, 28f), string.IsNullOrWhiteSpace(dialogueSpeaker) ? ObjectName(dialogueFocus) : dialogueSpeaker, speakerStyle);
            GUI.Label(new Rect(panel.x + 24f, panel.y + 86f, panel.width - 48f, 18f), ObjectName(dialogueFocus), CenterLeftStyle(11, muted));

            float choiceBlockHeight = hasChoices ? 94f : 0f;
            Rect body = new Rect(panel.x + 24f, panel.y + 116f, panel.width - 48f, panel.height - 180f - choiceBlockHeight);
            DrawRect(body, Hex("0b1013", 0.88f));
            DrawBorder(body, accent.WithAlpha(0.46f), 1);
            string bodyCopy = CurrentDialoguePage();
            float contentWidth = Mathf.Max(80f, body.width - 36f);
            float contentHeight = Mathf.Max(body.height - 20f, bodyStyle.CalcHeight(new GUIContent(bodyCopy), contentWidth) + 20f);
            dialogueFallbackScroll = GUI.BeginScrollView(Pad(body, 8f), dialogueFallbackScroll, new Rect(0f, 0f, contentWidth, contentHeight));
            GUI.Label(new Rect(8f, 6f, contentWidth - 12f, contentHeight - 10f), bodyCopy, bodyStyle);
            GUI.EndScrollView();

            if (hasChoices)
            {
                float gap = 8f;
                float choiceW = (body.width - gap) * 0.5f;
                float choiceY = body.yMax + 10f;
                int visibleCount = Mathf.Min(4, dialogueChoices.Length);
                for (int i = 0; i < visibleCount; i++)
                {
                    DialogueChoiceView choice = dialogueChoices[i];
                    if (choice == null) continue;
                    Rect choiceRect = new Rect(body.x + (i % 2) * (choiceW + gap), choiceY + (i / 2) * 40f, choiceW, 32f);
                    bool oldEnabled = GUI.enabled;
                    GUI.enabled = choice.Enabled;
                    if (GUI.Button(choiceRect, $"{i + 1}. {choice.Label}", dialogueButtonStyle)) ChooseDialogueChoice(choice.Id);
                    GUI.enabled = oldEnabled;
                }
            }

            Rect button = new Rect(panel.xMax - 146f, panel.yMax - 46f, 118f, 30f);
            if (GUI.Button(button, DialogueContinueLabel(), dialogueButtonStyle)) AdvanceDialogue();
            string page = DialoguePageLabel();
            string hint = string.IsNullOrEmpty(page) ? "Enter, Space, Esc, or Continue" : $"{page}  /  Enter or Space for next";
            GUI.Label(new Rect(panel.x + 24f, panel.yMax - 42f, panel.width - 190f, 24f), hint, CenterLeftStyle(10, muted));
        }
    }
}
