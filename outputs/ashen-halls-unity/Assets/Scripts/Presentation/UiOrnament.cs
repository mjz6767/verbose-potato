using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace AshenHalls
{
    // Decorative only: one shared asset, no hit target, no timing or game state.
    [ExecuteAlways]
    public sealed class UiOrnament : MonoBehaviour
    {
        private static Texture2D texture;
        private static Sprite sprite;
        private static int owners;
        private bool ownsAsset;

        public static RectTransform Add(Transform parent, string name, float opacity = 0.45f)
        {
            GameObject decoration = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            decoration.transform.SetParent(parent, false);
            Image image = decoration.GetComponent<Image>();
            image.raycastTarget = false;
            image.preserveAspect = true;
            image.color = new Color(1f, 1f, 1f, Mathf.Clamp01(opacity));
            UiOrnament owner = decoration.AddComponent<UiOrnament>();
            image.sprite = Acquire();
            image.enabled = image.sprite != null;
            owner.ownsAsset = image.sprite != null;
            return image.rectTransform;
        }

        private static Sprite Acquire()
        {
            if (sprite == null) Load();
            if (sprite != null) owners++;
            return sprite;
        }

        private static void Load()
        {
            string root = Directory.GetParent(Application.dataPath)?.FullName;
            string[] directories = {
                string.IsNullOrEmpty(root) ? null : Path.Combine(root, "Docs", "ArtReferences"),
                Path.Combine(Application.dataPath, "Docs", "ArtReferences")
            };
            foreach (string directory in directories)
            {
                if (directory == null) continue;
                string path = Path.Combine(directory, RuntimeArtManifest.UiHearthDivider);
                if (!File.Exists(path)) continue;
                Texture2D candidate = null;
                try
                {
                    candidate = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    Type conversion = Type.GetType("UnityEngine.ImageConversion, UnityEngine.ImageConversionModule");
                    var decode = conversion?.GetMethod("LoadImage", new[] { typeof(Texture2D), typeof(byte[]) });
                    if (decode == null || !(bool)decode.Invoke(null, new object[] { candidate, File.ReadAllBytes(path) }))
                        continue;
                    Rect visible = VisibleBounds(candidate);
                    if (visible.width < 1f || visible.height < 1f) continue;
                    candidate.name = RuntimeArtManifest.UiHearthDivider;
                    candidate.filterMode = FilterMode.Bilinear;
                    candidate.wrapMode = TextureWrapMode.Clamp;
                    texture = candidate;
                    sprite = Sprite.Create(texture, visible, new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
                    sprite.name = "Hearth divider";
                    candidate = null;
                    return;
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("Could not load the decorative UI divider: " + ex.Message);
                }
                finally
                {
                    if (candidate != null) ReleaseObject(candidate);
                }
            }
        }

        private static Rect VisibleBounds(Texture2D source)
        {
            Color32[] pixels = source.GetPixels32();
            int left = source.width, bottom = source.height, right = -1, top = -1;
            int transparent = 0;
            for (int y = 0; y < source.height; y++)
            for (int x = 0; x < source.width; x++)
            {
                if (pixels[y * source.width + x].a <= 8) { transparent++; continue; }
                left = Math.Min(left, x);
                right = Math.Max(right, x);
                bottom = Math.Min(bottom, y);
                top = Math.Max(top, y);
            }
            // A missing alpha channel must never become a solid header backplate.
            if (right < left || transparent < pixels.Length / 4) return Rect.zero;
            left = Math.Max(0, left - 2);
            bottom = Math.Max(0, bottom - 2);
            right = Math.Min(source.width - 1, right + 2);
            top = Math.Min(source.height - 1, top + 2);
            return new Rect(left, bottom, right - left + 1, top - bottom + 1);
        }

        private void OnDestroy()
        {
            if (!ownsAsset) return;
            ownsAsset = false;
            owners = Math.Max(0, owners - 1);
            if (owners != 0) return;
            if (sprite != null) ReleaseObject(sprite);
            if (texture != null) ReleaseObject(texture);
            sprite = null;
            texture = null;
        }

        private static void ReleaseObject(UnityEngine.Object asset)
        {
            if (Application.isPlaying) Destroy(asset);
            else DestroyImmediate(asset);
        }
    }
}
