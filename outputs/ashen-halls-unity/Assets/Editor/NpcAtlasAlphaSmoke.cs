using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class NpcAtlasAlphaSmoke
    {
        public static void RunOrThrow()
        {
            // Source coordinates are top-down, matching Aseprite. These enclosed matte
            // patches survive ordinary outer-background removal and bounding-box trimming.
            VerifyAtlas(
                RuntimeArtManifest.MidgaardNpcAtlas, 1280, 1024,
                new[] { new Vector2Int(397, 469), new Vector2Int(120, 215), new Vector2Int(655, 713) },
                new[]
                {
                    new ProtectedPixel("chef's cream apron", 130, 654, new Color32(244, 223, 175, 255)),
                    new ProtectedPixel("healer's cream robe", 1170, 180, new Color32(242, 229, 203, 255)),
                    new ProtectedPixel("Mira's near-white ivory hood", 1152, 42, new Color32(248, 249, 229, 255)),
                    new ProtectedPixel("Sera's ivory front robe", 632, 949, new Color32(244, 225, 189, 255)),
                    new ProtectedPixel("Maud's pale hair", 1155, 282, new Color32(229, 212, 185, 255)),
                    new ProtectedPixel("guard's near-white spearhead", 74, 41, new Color32(246, 246, 241, 255))
                });
            VerifyAtlas(
                RuntimeArtManifest.WorldNpcCitizenAtlas, 1536, 768,
                new[] { new Vector2Int(150, 242), new Vector2Int(223, 326), new Vector2Int(1367, 315) },
                new[]
                {
                    new ProtectedPixel("fishmonger's apron", 580, 238, new Color32(219, 180, 128, 255)),
                    new ProtectedPixel("fishmonger's cream sleeve", 645, 134, new Color32(241, 206, 159, 255)),
                    new ProtectedPixel("lamplighter's pale lantern light", 100, 193, new Color32(251, 236, 193, 255))
                });
        }

        private static void VerifyAtlas(string fileName, int width, int height, Vector2Int[] holes, ProtectedPixel[] garments)
        {
            Texture2D atlas = LoadAtlas(fileName);
            try
            {
                Require(atlas.width == width && atlas.height == height, fileName + " preserves its authored cell grid");
                Color32[] pixels = atlas.GetPixels32();
                Require(pixels.Length == width * height, fileName + " exposes every source pixel");
                foreach (Vector2Int hole in holes)
                {
                    Color32 pixel = PixelFromTop(pixels, width, height, hole.x, hole.y);
                    Require(pixel.a <= 8, fileName + " removes enclosed white matte at " + hole + " (alpha " + pixel.a + ")");
                }

                // Exact source colors protect legitimate pale details from a global white
                // color-key, desaturation, tint, or destructive silhouette cleanup.
                foreach (ProtectedPixel garment in garments)
                {
                    Color32 pixel = PixelFromTop(pixels, width, height, garment.X, garment.Y);
                    Color32 expected = garment.Color;
                    Require(pixel.r == expected.r && pixel.g == expected.g && pixel.b == expected.b && pixel.a == expected.a,
                        fileName + " preserves " + garment.Label + " at (" + garment.X + ", " + garment.Y + ") exactly");
                }
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(atlas);
            }
        }

        private static Color32 PixelFromTop(Color32[] pixels, int width, int height, int x, int y)
        {
            Require(x >= 0 && x < width && y >= 0 && y < height, "authored probe is inside the atlas");
            return pixels[(height - 1 - y) * width + x];
        }

        private static Texture2D LoadAtlas(string fileName)
        {
            string projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
            string path = string.IsNullOrEmpty(projectRoot) ? "" : Path.Combine(projectRoot, "Docs", "ArtReferences", fileName);
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) path = Path.Combine(Application.dataPath, "Docs", "ArtReferences", fileName);
            Require(File.Exists(path), "approved cleaned-alpha atlas is missing: " + fileName);

            Type imageConversion = Type.GetType("UnityEngine.ImageConversion, UnityEngine.ImageConversionModule");
            MethodInfo loadImage = imageConversion?.GetMethod("LoadImage", new[] { typeof(Texture2D), typeof(byte[]) });
            Require(loadImage != null, "Unity image loader is available");
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            try
            {
                Require((bool)loadImage.Invoke(null, new object[] { texture, File.ReadAllBytes(path) }), "could not decode " + fileName);
                texture.name = fileName;
                return texture;
            }
            catch
            {
                UnityEngine.Object.DestroyImmediate(texture);
                throw;
            }
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("NPC alpha smoke: " + message);
        }

        private struct ProtectedPixel
        {
            public readonly string Label;
            public readonly int X;
            public readonly int Y;
            public readonly Color32 Color;

            public ProtectedPixel(string label, int x, int y, Color32 color)
            {
                Label = label;
                X = x;
                Y = y;
                Color = color;
            }
        }
    }
}
