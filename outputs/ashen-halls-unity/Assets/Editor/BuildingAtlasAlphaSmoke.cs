using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;

namespace AshenHalls.Editor
{
    public static class BuildingAtlasAlphaSmoke
    {
        private const int AtlasWidth = 1280;
        private const int AtlasHeight = 1024;
        private const int CellSize = 256;

        public static void RunOrThrow()
        {
            Texture2D atlas = LoadAtlas(RuntimeArtManifest.MidgaardTownAtlas);
            try
            {
                Require(atlas.width == AtlasWidth && atlas.height == AtlasHeight,
                    "the town atlas preserves the 5x4 grid of 256px cells");
                Color32[] pixels = atlas.GetPixels32();
                Require(pixels.Length == AtlasWidth * AtlasHeight, "the complete RGBA atlas is readable");

                // Global top-down coordinates match Aseprite. Sign-bracket islands
                // need explicit checks because exterior-only cleanup cannot reach them.
                Vector2Int[] signGaps =
                {
                    new Vector2Int(220, 144), new Vector2Int(978, 136),
                    new Vector2Int(968, 137), new Vector2Int(963, 148),
                    new Vector2Int(1057, 131), new Vector2Int(38, 384),
                    new Vector2Int(293, 381), new Vector2Int(555, 372),
                    new Vector2Int(576, 374), new Vector2Int(1047, 647)
                };
                Vector2Int[] exteriorContours =
                {
                    new Vector2Int(215, 234), new Vector2Int(306, 171),
                    new Vector2Int(806, 111), new Vector2Int(1203, 77),
                    new Vector2Int(182, 313), new Vector2Int(481, 481),
                    new Vector2Int(682, 493), new Vector2Int(490, 733),
                    new Vector2Int(1067, 740)
                };
                AssertTransparent(pixels, signGaps, "enclosed hanging-sign matte");
                AssertTransparent(pixels, exteriorContours, "exterior white contour");

                // Smoke intentionally touches transparency. Hash its whole protected
                // regions, including pale edges, rather than accepting a few dark pixels.
                AssertRegionHash(pixels, new RectInt(1085, 22, 38, 37),
                    "4ec84b5995da434c77b5f1a520e24de219d34f33000881817dd41e7db149df6d", "armorer chimney smoke");
                AssertRegionHash(pixels, new RectInt(403, 280, 38, 37),
                    "3726649fce87df8bb85801f7e951f879b1687be8d2df9a87b8eb3c3c46c668af", "weapon shop chimney smoke");
                AssertRegionHash(pixels, new RectInt(1179, 529, 39, 40),
                    "d83c9191aa248652a5f9746b6d49545e4d43b80551cc77dc7705d69339720579", "diner chimney smoke");
                AssertPixel(pixels, 86, 94, new Color32(22, 27, 27, 255), "roof slate");
                AssertPixel(pixels, 125, 112, new Color32(130, 65, 10, 255), "market brass");
                AssertPixel(pixels, 383, 116, new Color32(120, 72, 28, 255), "temple clock");
                AssertPixel(pixels, 342, 152, new Color32(85, 71, 54, 255), "temple stone");
                AssertPixel(pixels, 984, 160, new Color32(45, 19, 4, 255), "tavern mug sign");
                AssertPixel(pixels, 618, 425, new Color32(93, 68, 154, 255), "enchanter violet glass");
                AssertPixel(pixels, 378, 644, new Color32(215, 154, 23, 255), "Town Hall gold banner");

                AssertNonArchitectureCellsUnchanged(pixels);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(atlas);
            }
        }

        private static void AssertNonArchitectureCellsUnchanged(Color32[] pixels)
        {
            // Reviewed v2.21 source fingerprints: building cleanup must not alter
            // fountains, gates, camp, sewer, walls, quest boards or recall art.
            int[] cells = { 2, 8, 9, 10, 12, 13, 15, 16, 17, 18, 19 };
            string[] sourceHashes =
            {
                "cc6d087dec59f69f96a460debaab933004aa69da8c50a9a0d07b78f664c76c00",
                "9cbefa0800fa427a328c329acba6b5bbac2d0c6d9272d69c94691aa5478ff826",
                "1ecc0759f41a00d97992dc96760106e46306172699398338987a92df6f0bf562",
                "390f64ba045bd9e21f7371e6edb31a80d16881fee2235b3c9cb761dd499f9324",
                "b626c6f2c03963ff158ce075fc427d4b15a4b9b34df6207d4b2f4730023ab0f9",
                "65a7012ff7276971ee16a9dc83934650af251c763b4e10a86ca7d9c43f85dac2",
                "9a94b4ef96385af111f56dbee6408c122b5c8bb252d18ecc02f8528217e5ec7b",
                "d090d08080ef1b6d9f3d0a72b88b084d2af59587bfe54104a2f0a4391f7d71de",
                "68dccd551e806acbcacad56a43bff4973a834dd93c58694180d5eb13094858f8",
                "a725964ae6bd09c1119a60641da60dce8e799bf642b921494ab84e02b0f5a729",
                "8c600a11601705e3f48e896fb042fca8de9b1acb99b9be29a791d081cf600a78"
            };
            for (int i = 0; i < cells.Length; i++)
            {
                int cell = cells[i];
                Require(!MidgaardTownArtCatalog.IsArchitectureCell(cell), "non-architecture preservation fixture stays correctly classified");
                RectInt region = new RectInt(cell % 5 * CellSize, cell / 5 * CellSize, CellSize, CellSize);
                AssertRegionHash(pixels, region, sourceHashes[i], "non-architecture cell " + cell);
            }
        }

        private static void AssertTransparent(Color32[] pixels, Vector2Int[] probes, string label)
        {
            foreach (Vector2Int probe in probes)
            {
                byte alpha = PixelFromTop(pixels, probe.x, probe.y).a;
                Require(alpha <= 8, label + " at " + probe + " is transparent (alpha " + alpha + ")");
            }
        }

        private static void AssertPixel(Color32[] pixels, int x, int y, Color32 expected, string label)
        {
            Color32 actual = PixelFromTop(pixels, x, y);
            Require(actual.r == expected.r && actual.g == expected.g && actual.b == expected.b && actual.a == expected.a,
                label + " preserves its source RGBA at (" + x + ", " + y + ")");
        }

        private static void AssertRegionHash(Color32[] pixels, RectInt region, string expected, string label)
        {
            byte[] rgba = new byte[region.width * region.height * 4];
            int offset = 0;
            for (int y = region.yMin; y < region.yMax; y++)
            for (int x = region.xMin; x < region.xMax; x++)
            {
                Color32 pixel = PixelFromTop(pixels, x, y);
                // Invisible RGB can vary between PNG decoders; all visible RGB and
                // every alpha byte remain exact in this top-down RGBA fingerprint.
                rgba[offset++] = pixel.a == 0 ? (byte)0 : pixel.r;
                rgba[offset++] = pixel.a == 0 ? (byte)0 : pixel.g;
                rgba[offset++] = pixel.a == 0 ? (byte)0 : pixel.b;
                rgba[offset++] = pixel.a;
            }
            using (SHA256 sha = SHA256.Create())
            {
                string actual = BitConverter.ToString(sha.ComputeHash(rgba)).Replace("-", "").ToLowerInvariant();
                Require(string.Equals(actual, expected, StringComparison.Ordinal), label + " preserves every source pixel");
            }
        }

        private static Color32 PixelFromTop(Color32[] pixels, int x, int y)
        {
            Require(x >= 0 && x < AtlasWidth && y >= 0 && y < AtlasHeight, "the authored probe is inside its atlas");
            return pixels[(AtlasHeight - 1 - y) * AtlasWidth + x];
        }

        private static Texture2D LoadAtlas(string fileName)
        {
            string projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
            string path = string.IsNullOrEmpty(projectRoot) ? "" : Path.Combine(projectRoot, "Docs", "ArtReferences", fileName);
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) path = Path.Combine(Application.dataPath, "Docs", "ArtReferences", fileName);
            Require(File.Exists(path), "approved town atlas is missing: " + fileName);
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
            if (!condition) throw new InvalidOperationException("Building alpha smoke: " + message);
        }
    }
}
