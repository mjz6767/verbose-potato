using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AshenHalls
{
    public static class UiRuntime
    {
        public const string DialogueFontResource = "Fonts/LibreBaskerville-Regular";
        public const string DialogueEmphasisFontResource = "Fonts/LibreBaskerville-SemiBold";
        public const string TitleFontResource = "Fonts/CinzelDecorative-Regular";

        private static Font defaultFont;
        private static Font dialogueFont;
        private static Font dialogueEmphasisFont;
        private static Font titleFont;
        private static readonly Dictionary<string, Sprite> atlasSprites = new Dictionary<string, Sprite>();
        private static EventSystem ensuredEventSystem;
        private static int textInputSuppressedThroughFrame = -1;

        public static Font DefaultFont
        {
            get
            {
                if (defaultFont == null) defaultFont = LoadDefaultFont();
                return defaultFont;
            }
        }

        public static Font DialogueFont
        {
            get
            {
                if (dialogueFont == null)
                {
                    dialogueFont = LoadBundledFont(DialogueFontResource, "dialogue")
                        ?? LoadDialogueFallbackFont()
                        ?? DefaultFont;
                }
                return dialogueFont;
            }
        }

        public static Font DialogueEmphasisFont
        {
            get
            {
                if (dialogueEmphasisFont == null)
                {
                    dialogueEmphasisFont = LoadBundledFont(DialogueEmphasisFontResource, "dialogue emphasis")
                        ?? DialogueFont;
                }
                return dialogueEmphasisFont;
            }
        }

        public static Font TitleFont
        {
            get
            {
                if (titleFont == null)
                {
                    titleFont = LoadBundledFont(TitleFontResource, "title display")
                        ?? DialogueEmphasisFont;
                }
                return titleFont;
            }
        }

        public static Sprite AtlasSprite(Texture2D texture, Rect topLeftSource)
        {
            if (texture == null || topLeftSource.width < 1f || topLeftSource.height < 1f) return null;
            Rect bounded = new Rect(
                Mathf.Clamp(topLeftSource.x, 0f, texture.width - 1f),
                Mathf.Clamp(topLeftSource.y, 0f, texture.height - 1f),
                Mathf.Clamp(topLeftSource.width, 1f, texture.width),
                Mathf.Clamp(topLeftSource.height, 1f, texture.height));
            if (bounded.xMax > texture.width) bounded.width = texture.width - bounded.x;
            if (bounded.yMax > texture.height) bounded.height = texture.height - bounded.y;

            string key = $"{texture.GetInstanceID()}:{Mathf.RoundToInt(bounded.x)}:{Mathf.RoundToInt(bounded.y)}:{Mathf.RoundToInt(bounded.width)}:{Mathf.RoundToInt(bounded.height)}";
            if (atlasSprites.TryGetValue(key, out Sprite cached) && cached != null) return cached;

            Rect unitySource = new Rect(bounded.x, texture.height - bounded.y - bounded.height, bounded.width, bounded.height);
            Sprite sprite = Sprite.Create(texture, unitySource, new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
            sprite.name = "UI Atlas Cell " + key;
            atlasSprites[key] = sprite;
            return sprite;
        }

        public static void ConfigureOverlayCanvas(Canvas canvas, int sortingOrder)
        {
            if (canvas == null) return;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
            canvas.sortingOrder = sortingOrder;
            canvas.pixelPerfect = true;

            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler == null) scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            if (canvas.GetComponent<GraphicRaycaster>() == null) canvas.gameObject.AddComponent<GraphicRaycaster>();
        }

        public static Canvas CreateOwnedRootCanvas(MonoBehaviour owner, string name)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            GameObject root = new GameObject(name, typeof(RectTransform));
            Canvas canvas = root.AddComponent<Canvas>();
            UiOwnedCanvasLifetime lifetime = owner.GetComponent<UiOwnedCanvasLifetime>();
            if (lifetime == null) lifetime = owner.gameObject.AddComponent<UiOwnedCanvasLifetime>();
            lifetime.Track(root);
            return canvas;
        }

        public static bool IsRenderableRootOverlay(Canvas canvas)
        {
            if (canvas == null
                || canvas.transform.parent != null
                || canvas.renderMode != RenderMode.ScreenSpaceOverlay
                || !canvas.enabled
                || !canvas.gameObject.activeInHierarchy)
            {
                return false;
            }

            RectTransform root = canvas.transform as RectTransform;
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            return root != null
                && root.rect.width > 1f
                && root.rect.height > 1f
                && raycaster != null
                && raycaster.enabled
                && raycaster.gameObject.activeInHierarchy;
        }

        public static bool CanOwnModal(Canvas canvas, RectTransform panel, CanvasGroup canvasGroup, Selectable primaryControl)
        {
            if (!IsRenderableRootOverlay(canvas)
                || panel == null
                || !panel.gameObject.activeInHierarchy
                || panel.rect.width <= 1f
                || panel.rect.height <= 1f
                || primaryControl == null
                || !primaryControl.IsActive()
                || !primaryControl.IsInteractable())
            {
                return false;
            }

            if (canvasGroup != null
                && (canvasGroup.alpha <= 0.01f || !canvasGroup.interactable || !canvasGroup.blocksRaycasts))
            {
                return false;
            }

            return HasUsableEventSystem();
        }

        public static bool HasUsableEventSystem()
        {
            return IsEventSystemReady(EventSystem.current)
                || (!Application.isPlaying && IsEventSystemReady(ensuredEventSystem));
        }

        public static bool HasTextInputFocus()
        {
            return HasTextInputFocus(Time.frameCount);
        }

        public static bool HasTextInputFocus(int frame)
        {
            if (frame <= textInputSuppressedThroughFrame) return true;
            EventSystem eventSystem = EventSystem.current ?? (!Application.isPlaying ? ensuredEventSystem : null);
            GameObject selected = eventSystem == null ? null : eventSystem.currentSelectedGameObject;
            if (selected == null) return false;
            InputField field = selected.GetComponent<InputField>();
            return field != null && field.IsActive() && field.IsInteractable() && field.isFocused;
        }

        public static void NotifyTextInputEnded()
        {
            // Native Return/Escape may end editing before the game's Update.
            // Consume that frame only; selection itself can persist afterward.
            textInputSuppressedThroughFrame = Time.frameCount;
        }

        public static bool SetCanvasVisible(Canvas canvas, bool visible)
        {
            if (canvas == null) return false;
            bool changed = canvas.gameObject.activeSelf != visible || canvas.enabled != visible;
            if (visible)
            {
                if (!canvas.gameObject.activeSelf) canvas.gameObject.SetActive(true);
                canvas.enabled = true;
                canvas.transform.SetAsLastSibling();
            }
            else
            {
                canvas.enabled = false;
                if (canvas.gameObject.activeSelf) canvas.gameObject.SetActive(false);
            }
            return changed;
        }

        public static bool IsCanvasVisible(Canvas canvas)
        {
            return canvas != null && canvas.enabled && canvas.gameObject.activeInHierarchy;
        }

        public static EventSystem EnsureEventSystemReady()
        {
            EventSystem eventSystem = IsEventSystemUsableHost(EventSystem.current) ? EventSystem.current : null;
            if (eventSystem == null)
            {
                EventSystem[] candidates = UnityEngine.Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (EventSystem candidate in candidates)
                {
                    if (!IsEventSystemUsableHost(candidate)) continue;
                    eventSystem = candidate;
                    break;
                }
            }
            if (eventSystem == null)
            {
                GameObject eventSystemObject = new GameObject("EventSystem");
                eventSystem = eventSystemObject.AddComponent<EventSystem>();
            }

            if (!eventSystem.gameObject.activeSelf) eventSystem.gameObject.SetActive(true);
            eventSystem.enabled = true;

            BaseInputModule inputModule = eventSystem.GetComponent<BaseInputModule>();
            if (inputModule == null) inputModule = eventSystem.gameObject.AddComponent<StandaloneInputModule>();
            inputModule.enabled = true;
            ensuredEventSystem = eventSystem;
            return eventSystem;
        }

        public static bool IsEventSystemReady(EventSystem eventSystem)
        {
            if (eventSystem == null
                || !eventSystem.enabled
                || !eventSystem.gameObject.activeInHierarchy)
            {
                return false;
            }

            // Runtime input must be owned by Unity's current EventSystem. The
            // editor boot smoke invokes lifecycle methods outside Play Mode, where
            // Unity intentionally does not register a current system.
            if (Application.isPlaying && eventSystem != EventSystem.current) return false;

            BaseInputModule inputModule = eventSystem.currentInputModule;
            if (inputModule == null) inputModule = eventSystem.GetComponent<BaseInputModule>();
            return inputModule != null && inputModule.enabled && inputModule.gameObject.activeInHierarchy;
        }

        private static bool IsEventSystemUsableHost(EventSystem eventSystem)
        {
            if (eventSystem == null) return false;
            Transform parent = eventSystem.transform.parent;
            return parent == null || parent.gameObject.activeInHierarchy;
        }

        private static Font LoadDefaultFont()
        {
            try
            {
                Font builtIn = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                if (builtIn != null) return builtIn;
            }
            catch (Exception ex)
            {
                Debug.LogWarning(VersionInfo.ProductName + " UI could not load LegacyRuntime.ttf: " + ex.Message);
            }

            try
            {
                Font fallback = Font.CreateDynamicFontFromOSFont(new[] { "Segoe UI", "Tahoma", "Helvetica", "DejaVu Sans" }, 14);
                if (fallback != null) return fallback;
            }
            catch (Exception ex)
            {
                Debug.LogWarning(VersionInfo.ProductName + " UI could not load an operating-system fallback font: " + ex.Message);
            }

            throw new InvalidOperationException(VersionInfo.ProductName + " UI could not resolve a default font.");
        }

        private static Font LoadBundledFont(string resourcePath, string role)
        {
            try
            {
                Font bundled = Resources.Load<Font>(resourcePath);
                if (bundled != null) return bundled;
                Debug.LogWarning(VersionInfo.ProductName + " UI could not find its bundled " + role + " font at " + resourcePath + ".");
            }
            catch (Exception ex)
            {
                Debug.LogWarning(VersionInfo.ProductName + " UI could not load its bundled " + role + " font: " + ex.Message);
            }
            return null;
        }

        private static Font LoadDialogueFallbackFont()
        {
            try
            {
                return Font.CreateDynamicFontFromOSFont(
                    new[] { "Georgia", "Palatino Linotype", "Book Antiqua", "Times New Roman", "Liberation Serif", "DejaVu Serif" },
                    16);
            }
            catch (Exception ex)
            {
                Debug.LogWarning(VersionInfo.ProductName + " UI could not load an operating-system serif fallback font: " + ex.Message);
                return null;
            }
        }
    }

    internal sealed class UiOwnedCanvasLifetime : MonoBehaviour
    {
        private readonly List<GameObject> ownedRoots = new List<GameObject>();

        public void Track(GameObject root)
        {
            if (root != null && !ownedRoots.Contains(root)) ownedRoots.Add(root);
        }

        private void OnDestroy()
        {
            foreach (GameObject root in ownedRoots)
            {
                if (root == null) continue;
                if (Application.isPlaying) Destroy(root);
                else DestroyImmediate(root);
            }
            ownedRoots.Clear();
        }
    }
}
