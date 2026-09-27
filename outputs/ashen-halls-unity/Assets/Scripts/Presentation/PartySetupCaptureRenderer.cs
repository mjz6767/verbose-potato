using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace AshenHalls
{
    // Renders the actual setup canvas without depending on a visible OS window.
    // The caller still validates dimensions and pixels with CaptureAcceptanceRules.
    public static class PartySetupCaptureRenderer
    {
        public static void Write(PartySetupScreen screen, int width, int height, string path)
        {
            if (screen == null || screen.ViewCanvas == null)
                throw new InvalidOperationException("Party Setup canvas is unavailable for capture.");
            if (width < 1 || height < 1) throw new ArgumentOutOfRangeException(nameof(width));
            Canvas canvas = screen.ViewCanvas;
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            bool scalerEnabled = scaler != null && scaler.enabled;
            RenderMode previousMode = canvas.renderMode;
            Camera previousCamera = canvas.worldCamera;
            float previousDistance = canvas.planeDistance;
            float previousScale = canvas.scaleFactor;
            Transform[] transforms = canvas.GetComponentsInChildren<Transform>(true);
            int[] layers = new int[transforms.Length];
            for (int i = 0; i < transforms.Length; i++) layers[i] = transforms[i].gameObject.layer;
            GameObject cameraObject = new GameObject("Party Setup offscreen review camera");
            RenderTexture target = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            RenderTexture previousTarget = RenderTexture.active;
            Texture2D pixels = null;
            try
            {
                Camera camera = cameraObject.AddComponent<Camera>();
                camera.enabled = false;
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0.04f, 0.025f, 0.02f, 1f);
                camera.orthographic = true;
                camera.orthographicSize = height * 0.5f;
                camera.nearClipPlane = 0.01f;
                camera.farClipPlane = 10f;
                camera.cullingMask = 1 << 31;
                camera.targetTexture = target;
                target.Create();
                if (scaler != null) scaler.enabled = false;
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camera;
                canvas.planeDistance = 1f;
                canvas.scaleFactor = Mathf.Min(width / 1280f, height / 720f);
                foreach (Transform child in transforms) child.gameObject.layer = 31;
                Canvas.ForceUpdateCanvases();
                screen.ApplyCaptureLayout(width, height);
                Canvas.ForceUpdateCanvases();
                camera.Render();
                Canvas.ForceUpdateCanvases();
                camera.Render();
                RenderTexture.active = target;
                pixels = new Texture2D(width, height, TextureFormat.RGB24, false);
                pixels.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
                pixels.Apply();
                string directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
                File.WriteAllBytes(path, pixels.EncodeToPNG());
            }
            finally
            {
                RenderTexture.active = previousTarget;
                canvas.worldCamera = previousCamera;
                canvas.renderMode = previousMode;
                canvas.planeDistance = previousDistance;
                canvas.scaleFactor = previousScale;
                if (scaler != null) scaler.enabled = scalerEnabled;
                for (int i = 0; i < transforms.Length; i++)
                    if (transforms[i] != null) transforms[i].gameObject.layer = layers[i];
                screen.ApplyCaptureLayout(Screen.width, Screen.height);
                if (pixels != null) Dispose(pixels);
                cameraObject.SetActive(false);
                Dispose(cameraObject);
                target.Release();
                Dispose(target);
            }
        }

        private static void Dispose(UnityEngine.Object value)
        {
            if (Application.isPlaying) UnityEngine.Object.Destroy(value);
            else UnityEngine.Object.DestroyImmediate(value);
        }
    }
}
