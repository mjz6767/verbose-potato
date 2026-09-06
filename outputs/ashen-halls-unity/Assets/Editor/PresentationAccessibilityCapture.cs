using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AshenHalls.Editor
{
    public static class PresentationAccessibilityCapture
    {
        private const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;

        public static void Capture()
        {
            try
            {
                PresentationAccessibilitySmoke.RunOrThrow();
                string directory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "QA", "project-audit", "ui-review-2026-09-05", DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff")));
                Directory.CreateDirectory(directory);
                foreach (Vector2Int size in new[] { new Vector2Int(960, 600), new Vector2Int(1280, 720) })
                {
                    CapturePause(directory, size, false);
                    CapturePause(directory, size, true);
                    CaptureHelp(directory, size);
                }
                Debug.Log(VersionInfo.ProductName + " presentation accessibility captures passed: " + directory);
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError(VersionInfo.ProductName + " presentation accessibility captures failed: " + ex);
                EditorApplication.Exit(1);
            }
        }

        private static void CapturePause(string directory, Vector2Int size, bool settings)
        {
            GameObject host = new GameObject("Pause capture host");
            try
            {
                PauseMenuScreen screen = host.AddComponent<PauseMenuScreen>();
                screen.Bind(new PauseMenuScreenBindings
                {
                    View = () => new PauseMenuView
                    {
                        Title = "Menu",
                        RouteLine = "Chapter I • Midgaard • Town Hall",
                        SaveLine = "Campaign checkpoint ready",
                        AudioLine = "Audio: On",
                        SfxLine = "SFX: 75%",
                        MusicLine = "Music: 50%",
                        MotionLine = "Reduced Motion: Off",
                        SettingsOpen = settings
                    }
                });
                screen.SetVisible(true);
                screen.Refresh();
                Canvas canvas = Field<Canvas>(screen, "canvas");
                Render(directory, (settings ? "pause-settings-" : "pause-") + size.x + "x" + size.y, size, canvas, () =>
                {
                    typeof(PauseMenuScreen).GetMethod("ApplyLayout", PrivateInstance, null, new[] { typeof(bool), typeof(float), typeof(float) }, null)
                        .Invoke(screen, new object[] { settings, (float)size.x, (float)size.y });
                });
                screen.SetVisible(false);
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        private static void CaptureHelp(string directory, Vector2Int size)
        {
            GameObject host = new GameObject("Help capture host");
            try
            {
                HelpOverlayScreen screen = host.AddComponent<HelpOverlayScreen>();
                screen.Bind(new HelpOverlayBindings { View = () => HelpOverlayContent.Build(GameMode.Combat, false, 3, "Midgaard") });
                screen.SetVisible(true);
                screen.Refresh();
                Canvas canvas = Field<Canvas>(screen, "canvas");
                foreach (bool bottom in new[] { false, true })
                {
                    Render(directory, "combat-help-" + (bottom ? "bottom-" : "top-") + size.x + "x" + size.y, size, canvas, () =>
                    {
                        typeof(HelpOverlayScreen).GetMethod("ApplyLayout", PrivateInstance, null, new[] { typeof(float), typeof(float) }, null)
                            .Invoke(screen, new object[] { (float)size.x, (float)size.y });
                        ScrollRect scroll = Field<ScrollRect>(screen, "bodyScroll");
                        Text body = scroll.content.GetComponent<Text>();
                        scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Max(scroll.viewport.rect.height, body.preferredHeight));
                        Canvas.ForceUpdateCanvases();
                        scroll.verticalNormalizedPosition = bottom ? 0f : 1f;
                        // A direct editor render has no player LateUpdate to
                        // synchronize the thumb after programmatic scrolling.
                        scroll.Rebuild(CanvasUpdate.PostLayout);
                    });
                }
                screen.SetVisible(false);
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        private static void Render(string directory, string name, Vector2Int size, Canvas canvas, Action layout)
        {
            GameObject cameraObject = new GameObject("Offscreen UI review camera");
            RenderTexture target = new RenderTexture(size.x, size.y, 24, RenderTextureFormat.ARGB32);
            Texture2D pixels = null;
            RenderTexture previous = RenderTexture.active;
            try
            {
                Camera camera = cameraObject.AddComponent<Camera>();
                camera.enabled = false;
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0.06f, 0.08f, 0.10f, 1f);
                camera.orthographic = true;
                camera.orthographicSize = size.y * 0.5f;
                camera.nearClipPlane = 0.01f;
                camera.farClipPlane = 10f;
                camera.cullingMask = 1 << 31;
                camera.targetTexture = target;
                target.Create();
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camera;
                canvas.planeDistance = 1f;
                foreach (Transform child in canvas.GetComponentsInChildren<Transform>(true)) child.gameObject.layer = 31;
                Canvas.ForceUpdateCanvases();
                layout();
                Canvas.ForceUpdateCanvases();
                camera.Render();
                Canvas.ForceUpdateCanvases();
                camera.Render();
                RenderTexture.active = target;
                pixels = new Texture2D(size.x, size.y, TextureFormat.RGB24, false);
                pixels.ReadPixels(new Rect(0f, 0f, size.x, size.y), 0, 0);
                pixels.Apply();
                string path = Path.Combine(directory, name + ".png");
                File.WriteAllBytes(path, pixels.EncodeToPNG());
                Debug.Log("Presentation capture " + path + " (" + size.x + "x" + size.y + ")");
            }
            finally
            {
                RenderTexture.active = previous;
                canvas.worldCamera = null;
                if (pixels != null) UnityEngine.Object.DestroyImmediate(pixels);
                UnityEngine.Object.DestroyImmediate(cameraObject);
                target.Release();
                UnityEngine.Object.DestroyImmediate(target);
            }
        }

        private static T Field<T>(object source, string name)
        {
            return (T)source.GetType().GetField(name, PrivateInstance).GetValue(source);
        }
    }
}
