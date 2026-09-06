using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AshenHalls.Editor
{
    public static class PresentationAccessibilitySmoke
    {
        public static void Run()
        {
            try
            {
                RunOrThrow();
                Debug.Log(VersionInfo.ProductName + " presentation accessibility smoke passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " presentation accessibility smoke failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        public static void RunOrThrow()
        {
            PauseControlsAndStatusRemainSeparate();
            NativeTextSelectionOwnsShortcutsThroughSubmission();
            HelpKeepsAllContentReachable();
        }

        private static void PauseControlsAndStatusRemainSeparate()
        {
            foreach (Vector2Int size in new[] { new Vector2Int(960, 600), new Vector2Int(1280, 720), new Vector2Int(1920, 1080) })
            {
                foreach (bool settingsOpen in new[] { false, true })
                {
                    PauseMenuGeometry geometry = PauseMenuScreenLayout.Calculate(size.x, size.y, settingsOpen);
                    Require(geometry.Fits(size.x, size.y), "pause panel fits " + size);
                    Rect status = PauseMenuScreenLayout.StatusRect(geometry.Panel);
                    Rect settings = PauseMenuScreenLayout.SettingsRect(geometry.Panel);
                    Require(FitsLocal(status, geometry.Panel), "pause status stays inside panel " + size);
                    bool compact = PauseMenuScreenLayout.UseCompactSettings(geometry.Panel, settingsOpen);
                    Rect previous = new Rect();
                    for (int i = 0; i < 6; i++)
                    {
                        Rect button = PauseMenuScreenLayout.ButtonRect(geometry.Panel.width, i, compact);
                        Require(FitsLocal(button, geometry.Panel), "pause action fits " + size);
                        Require(!button.Overlaps(status), "status never covers a pause action " + size);
                        Require(i == 0 || !button.Overlaps(previous), "pause actions stay separate " + size);
                        if (settingsOpen) Require(!button.Overlaps(settings), "settings never cover a pause action " + size);
                        previous = button;
                    }
                    if (settingsOpen)
                    {
                        Require(FitsLocal(settings, geometry.Panel), "settings remain inside the panel " + size);
                        Require(!settings.Overlaps(status), "status never covers settings " + size);
                        Require(settings.height >= 152f, "Reduced Motion retains its complete hit target " + size);
                    }
                }
            }
        }

        private static void NativeTextSelectionOwnsShortcutsThroughSubmission()
        {
            EventSystem eventSystem = UiRuntime.EnsureEventSystemReady();
            GameObject previous = eventSystem.currentSelectedGameObject;
            System.Reflection.FieldInfo guardField = typeof(UiRuntime).GetField("textInputSuppressedThroughFrame", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            int previousGuard = (int)guardField.GetValue(null);
            GameObject fieldObject = new GameObject("Shortcut ownership smoke", typeof(RectTransform), typeof(InputField));
            try
            {
                InputField field = fieldObject.GetComponent<InputField>();
                eventSystem.SetSelectedGameObject(fieldObject);
                // Drive native editing state directly because these editor
                // contracts execute without the player's input update loop.
                typeof(InputField).GetField("m_AllowInput", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(field, true);
                Require(UiRuntime.HasTextInputFocus(Time.frameCount + 1), "active native text input owns keyboard shortcuts");
                field.DeactivateInputField();
                Require(!field.isFocused, "native submit can finish editing before the game's Update");
                UiRuntime.NotifyTextInputEnded();
                Require(UiRuntime.HasTextInputFocus(), "end-edit guard protects Return after native editing ends");
                Require(!UiRuntime.HasTextInputFocus(Time.frameCount + 1), "shortcuts resume next frame even when the field remains selected");
                field.interactable = false;
                Require(!UiRuntime.HasTextInputFocus(Time.frameCount + 1), "disabled text fields cannot capture shortcuts");
                field.interactable = true;
                fieldObject.SetActive(false);
                Require(!UiRuntime.HasTextInputFocus(Time.frameCount + 1), "hidden text fields cannot capture shortcuts");
                eventSystem.SetSelectedGameObject(null);
                Require(!UiRuntime.HasTextInputFocus(Time.frameCount + 1), "normal shortcuts resume after text selection clears");
            }
            finally
            {
                eventSystem.SetSelectedGameObject(previous);
                UnityEngine.Object.DestroyImmediate(fieldObject);
                guardField.SetValue(null, previousGuard);
            }
        }

        private static void HelpKeepsAllContentReachable()
        {
            foreach (Vector2Int size in new[] { new Vector2Int(960, 600), new Vector2Int(1280, 720), new Vector2Int(1920, 1080) })
                Require(HelpOverlayLayout.Calculate(size.x, size.y).Fits(size.x, size.y), "help geometry fits " + size);

            GameObject host = new GameObject("Help accessibility smoke");
            try
            {
                HelpOverlayView view = new HelpOverlayView
                {
                    Title = "Long help",
                    Lines = Enumerable.Range(1, 100).Select(i => "Instruction " + i + ": every help paragraph must remain reachable.").ToArray()
                };
                HelpOverlayScreen help = host.AddComponent<HelpOverlayScreen>();
                help.Bind(new HelpOverlayBindings { View = () => view });
                help.SetVisible(true);
                help.Refresh();
                ScrollRect scroll = (ScrollRect)typeof(HelpOverlayScreen).GetField("bodyScroll", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(help);
                Require(scroll != null && scroll.vertical && !scroll.horizontal, "help exposes vertical scrolling");
                Require(scroll.viewport.GetComponent<RectMask2D>() != null, "help clips scroll content inside its body");
                Require(scroll.verticalScrollbar != null && scroll.verticalScrollbar.gameObject.activeInHierarchy, "help has a visible draggable scrollbar");
                Text body = scroll.content.GetComponent<Text>();
                Require(body.text.Contains("Instruction 100"), "the final help paragraph is retained");
                Require(scroll.content.rect.height >= body.preferredHeight - 1f, "scroll content includes the full rendered text height");
                Require(scroll.content.rect.height > scroll.viewport.rect.height, "long help extends beyond the viewport");
                scroll.verticalNormalizedPosition = 0f;
                Canvas.ForceUpdateCanvases();
                Require(Mathf.Abs(scroll.verticalNormalizedPosition) < 0.01f, "the last help paragraph can be scrolled into view");
                view.Lines = new[] { "A different help page" };
                help.Refresh();
                Require(scroll.content.GetComponent<Text>().text.Contains("different help"), "changing help updates the reachable content");
                help.SetVisible(false);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(host);
            }
        }

        private static bool FitsLocal(Rect child, Rect parent)
        {
            return child.xMin >= 0f && child.yMin >= 0f && child.xMax <= parent.width && child.yMax <= parent.height;
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
