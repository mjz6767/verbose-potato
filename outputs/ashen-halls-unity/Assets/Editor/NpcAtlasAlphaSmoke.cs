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
            VerifyHeadContourClassifier();
            // Source coordinates are top-down, matching Aseprite. These enclosed matte
            // patches survive ordinary outer-background removal and bounding-box trimming.
            VerifyAtlas(
                RuntimeArtManifest.MidgaardNpcAtlas, 1280, 1024, 256,
                new[] { 14, 16, 17, 18, 19 },
                new[] { new Vector2Int(397, 469), new Vector2Int(120, 215), new Vector2Int(655, 713) },
                new[]
                {
                    new ProtectedPixel("chef's cream apron", 130, 654, new Color32(244, 223, 175, 255)),
                    new ProtectedPixel("healer's cream robe", 1170, 180, new Color32(242, 229, 203, 255)),
                    new ProtectedPixel("Mira's near-white ivory hood", 1152, 42, new Color32(248, 249, 229, 255)),
                    // Recovering Sera's hood rescales her complete figure; this
                    // reviewed interior robe sample retains the same ivory color.
                    new ProtectedPixel("Sera's ivory front robe", 634, 926, new Color32(244, 225, 189, 255)),
                    new ProtectedPixel("Maud's pale hair", 1155, 282, new Color32(229, 212, 185, 255)),
                    new ProtectedPixel("guard's near-white spearhead", 74, 41, new Color32(246, 246, 241, 255))
                });
            VerifyAtlas(
                RuntimeArtManifest.WorldNpcCitizenAtlas, 1536, 768, 384,
                new[] { 4, 5, 6, 7 },
                new[] { new Vector2Int(150, 242), new Vector2Int(223, 326), new Vector2Int(1367, 315) },
                new[]
                {
                    new ProtectedPixel("fishmonger's apron", 580, 238, new Color32(219, 180, 128, 255)),
                    new ProtectedPixel("fishmonger's cream sleeve", 645, 134, new Color32(241, 206, 159, 255)),
                    new ProtectedPixel("lamplighter's pale lantern light", 100, 193, new Color32(251, 236, 193, 255))
                });
        }

        private static void VerifyAtlas(string fileName, int width, int height, int cellSize,
            int[] recoveredHeadCells, Vector2Int[] holes, ProtectedPixel[] garments)
        {
            Texture2D atlas = LoadAtlas(fileName);
            try
            {
                Require(atlas.width == width && atlas.height == height, fileName + " preserves its authored cell grid");
                Color32[] pixels = atlas.GetPixels32();
                Require(pixels.Length == width * height, fileName + " exposes every source pixel");
                foreach (int cell in recoveredHeadCells)
                {
                    HeadContour contour = MeasureHeadContour(pixels, width, height, cellSize, cell);
                    Require(contour.FirstRow >= 0, fileName + " cell " + cell + " retains visible head art");
                    Require(!HasAbruptHeadCut(contour),
                        fileName + " cell " + cell + " must retain its complete crown instead of a padded horizontal crop"
                        + " (first run " + contour.FirstRun + ", first-four-row maximum " + contour.MaximumRun + ")");
                }
                foreach (Vector2Int hole in holes)
                {
                    Color32 pixel = PixelFromTop(pixels, width, height, hole.x, hole.y);
                    Require(pixel.a <= 8, fileName + " removes enclosed white matte at " + hole + " (alpha " + pixel.a + ")");
                }

                // Exact accepted colors protect legitimate pale details from a global white
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

        private static bool HasAbruptHeadCut(HeadContour contour)
        {
            // The old equal-height source-grid cuts removed the tops of these nine
            // figures before normalization added valid-looking gutters. Their first
            // substantial silhouette row is already 68-100% as wide as the next rows;
            // intact crowns taper in from a much smaller first row. This is a targeted
            // regression for the reviewed figures, not a universal rule for flat hats.
            return contour.FirstRun >= 20
                && contour.FirstRun * 100 >= contour.MaximumRun * 65;
        }

        private static HeadContour MeasureHeadContour(Color32[] pixels, int width, int height, int cellSize, int cell)
        {
            Require(cellSize > 0 && width % cellSize == 0 && height % cellSize == 0,
                "head contour uses whole square atlas cells");
            int columns = width / cellSize;
            Require(cell >= 0 && cell < columns * (height / cellSize), "head contour cell is inside the atlas");
            int originX = cell % columns * cellSize;
            int originY = cell / columns * cellSize;
            int firstRow = -1;
            int firstRun = 0;
            int maximumRun = 0;
            for (int y = 0; y < cellSize; y++)
            {
                int run = 0;
                int longest = 0;
                int pixelRow = (height - 1 - originY - y) * width;
                for (int x = 0; x < cellSize; x++)
                {
                    // Match the established named-NPC silhouette coverage threshold.
                    run = pixels[pixelRow + originX + x].a >= 24 ? run + 1 : 0;
                    longest = Math.Max(longest, run);
                }
                if (firstRow < 0 && longest > 0)
                {
                    firstRow = y;
                    firstRun = longest;
                }
                if (firstRow < 0) continue;
                maximumRun = Math.Max(maximumRun, longest);
                if (y - firstRow >= 3) break;
            }
            return new HeadContour(firstRow, firstRun, maximumRun);
        }

        private static void VerifyHeadContourClassifier()
        {
            const int size = 32;
            Color32[] rounded = new Color32[size * size];
            Color32[] clipped = new Color32[size * size];
            int[] crownWidths = { 4, 10, 16, 20 };
            for (int row = 0; row < crownWidths.Length; row++)
            {
                int offset = (size - 1 - (row + 4)) * size;
                int left = (size - crownWidths[row]) / 2;
                for (int x = left; x < left + crownWidths[row]; x++)
                    rounded[offset + x] = new Color32(70, 60, 45, 255);
                for (int x = 4; x < 28; x++)
                    clipped[offset + x] = new Color32(70, 60, 45, 255);
            }
            // Soft isolated pixels above the figure cannot make a hard crop look
            // complete. The visible silhouette begins at the alpha-24 threshold.
            clipped[(size - 1 - 2) * size + 16] = new Color32(70, 60, 45, 23);
            HeadContour intact = MeasureHeadContour(rounded, size, size, size, 0);
            HeadContour amputated = MeasureHeadContour(clipped, size, size, size, 0);
            Require(intact.FirstRow == 4 && intact.FirstRun == 4 && intact.MaximumRun == 20,
                "head contour measures top-down crown rows exactly");
            Require(!HasAbruptHeadCut(intact), "a complete tapering crown passes the cutoff regression");
            Require(amputated.FirstRow == 4 && HasAbruptHeadCut(amputated),
                "a cropped crown fails even after transparent padding and soft edge pixels are added");
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

        private struct HeadContour
        {
            public readonly int FirstRow;
            public readonly int FirstRun;
            public readonly int MaximumRun;

            public HeadContour(int firstRow, int firstRun, int maximumRun)
            {
                FirstRow = firstRow;
                FirstRun = firstRun;
                MaximumRun = maximumRun;
            }
        }
    }
}
