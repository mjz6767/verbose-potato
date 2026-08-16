using System;
using UnityEngine;

namespace AshenHalls
{
    public enum ExplorationMiniMapMarkerKind
    {
        AuthoredSite,
        CurrentSite,
        Patrol,
        AlertedPatrol
    }

    public static class ExplorationMiniMapPresentationRules
    {
        public static Color32 UnchartedTerrainPixel()
        {
            return new Color32(3, 6, 7, 255);
        }

        public static bool ShouldShowAuthoredSite(
            bool discovered,
            bool current,
            int distance,
            int revealRadius)
        {
            if (current || discovered) return true;
            return distance >= 0 && distance <= Math.Max(0, revealRadius);
        }

        public static bool ShouldShowPatrol(
            bool active,
            int patrolDepth,
            int currentDepth,
            bool alerted,
            int distance,
            int revealRadius)
        {
            if (!active || patrolDepth != currentDepth || distance < 0) return false;
            return alerted || distance <= Math.Max(0, revealRadius);
        }

        public static int MarkerPixels(ExplorationMiniMapMarkerKind kind)
        {
            switch (kind)
            {
                case ExplorationMiniMapMarkerKind.CurrentSite: return 8;
                case ExplorationMiniMapMarkerKind.AlertedPatrol: return 8;
                case ExplorationMiniMapMarkerKind.AuthoredSite: return 6;
                default: return 5;
            }
        }

        public static long TerrainCacheKey(
            int mapIdentity,
            int width,
            int height,
            int depth,
            int playerX,
            int playerY,
            int terrainFingerprint)
        {
            long hash = 1469598103934665603L;
            hash = Mix(hash, mapIdentity);
            hash = Mix(hash, width);
            hash = Mix(hash, height);
            hash = Mix(hash, depth);
            hash = Mix(hash, playerX);
            hash = Mix(hash, playerY);
            return Mix(hash, terrainFingerprint);
        }

        public static int PixelIndexForTopDownMapCell(int x, int y, int width, int height)
        {
            if (width <= 0 || height <= 0) return -1;
            if (x < 0 || x >= width || y < 0 || y >= height) return -1;
            return (height - 1 - y) * width + x;
        }

        private static long Mix(long hash, int value)
        {
            return unchecked((hash ^ (uint)value) * 1099511628211L);
        }

    }

    /// <summary>
    /// Owns a point-filtered terrain raster for the exploration mini map.
    /// The caller is responsible for changing the <c>stableKey</c> whenever
    /// terrain colors change without a width or height change.
    /// </summary>
    public sealed class ExplorationMiniMapTerrainCache : IDisposable
    {
        public delegate Color32 PixelProvider(int x, int y);

        private const string TextureName = "exploration-mini-map-terrain-cache";

        private Texture2D texture;
        private Color32[] generatedPixels;
        private long cachedStableKey;
        private int cachedWidth;
        private int cachedHeight;
        private int rebuildCount;
        private bool hasCachedRaster;
        private bool disposed;

        public Texture2D Texture => texture;
        public int RebuildCount => rebuildCount;
        public bool HasCachedRaster => hasCachedRaster && texture != null;
        public long CachedStableKey => cachedStableKey;
        public int Width => HasCachedRaster ? cachedWidth : 0;
        public int Height => HasCachedRaster ? cachedHeight : 0;

        public bool IsCurrent(long stableKey, int width, int height)
        {
            return !disposed &&
                   HasCachedRaster &&
                   cachedStableKey == stableKey &&
                   cachedWidth == width &&
                   cachedHeight == height;
        }

        /// <summary>
        /// Returns the current raster or rebuilds it from a provider. Provider rows
        /// use Texture2D pixel order: y=0 is the bottom row. The backing Color32
        /// buffer is allocated only when the required pixel count changes.
        /// </summary>
        public Texture2D GetOrBuild(
            long stableKey,
            int width,
            int height,
            PixelProvider pixelProvider)
        {
            ThrowIfDisposed();
            if (pixelProvider == null) throw new ArgumentNullException(nameof(pixelProvider));

            int pixelCount = ValidatedPixelCount(width, height);
            if (IsCurrent(stableKey, width, height)) return texture;

            EnsureGeneratedPixelBuffer(pixelCount);
            for (int y = 0; y < height; y++)
            {
                int rowStart = y * width;
                for (int x = 0; x < width; x++)
                    generatedPixels[rowStart + x] = pixelProvider(x, y);
            }

            return Commit(stableKey, width, height, generatedPixels);
        }

        /// <summary>
        /// Returns the current raster or rebuilds it from an exact-size pixel array.
        /// Pixels are copied into Unity's texture storage and remain owned by the caller.
        /// </summary>
        public Texture2D GetOrBuild(
            long stableKey,
            int width,
            int height,
            Color32[] pixels)
        {
            ThrowIfDisposed();
            if (pixels == null) throw new ArgumentNullException(nameof(pixels));

            int pixelCount = ValidatedPixelCount(width, height);
            if (pixels.Length != pixelCount)
            {
                throw new ArgumentException(
                    $"Expected exactly {pixelCount} pixels for a {width}x{height} raster.",
                    nameof(pixels));
            }

            if (IsCurrent(stableKey, width, height)) return texture;
            return Commit(stableKey, width, height, pixels);
        }

        /// <summary>
        /// Releases the owned texture and invalidates the cached key. The reusable
        /// provider buffer is retained until Dispose is called.
        /// </summary>
        public void Clear()
        {
            if (disposed) return;
            ReleaseTexture();
            ResetCachedRaster();
        }

        public void Dispose()
        {
            if (disposed) return;

            ReleaseTexture();
            generatedPixels = null;
            ResetCachedRaster();
            disposed = true;
        }

        private Texture2D Commit(
            long stableKey,
            int width,
            int height,
            Color32[] pixels)
        {
            // Do not advertise stale content if a Unity upload throws midway.
            hasCachedRaster = false;
            if (texture == null || texture.width != width || texture.height != height)
            {
                ReleaseTexture();
                texture = CreateTexture(width, height);
            }

            texture.SetPixels32(pixels);
            texture.Apply(false, false);

            cachedStableKey = stableKey;
            cachedWidth = width;
            cachedHeight = height;
            hasCachedRaster = true;
            rebuildCount++;
            return texture;
        }

        private static Texture2D CreateTexture(int width, int height)
        {
            return new Texture2D(width, height, TextureFormat.RGBA32, false, true)
            {
                name = TextureName,
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                anisoLevel = 0,
                hideFlags = HideFlags.DontSave
            };
        }

        private void EnsureGeneratedPixelBuffer(int pixelCount)
        {
            if (generatedPixels == null || generatedPixels.Length != pixelCount)
                generatedPixels = new Color32[pixelCount];
        }

        private static int ValidatedPixelCount(int width, int height)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width), width, "Width must be positive.");
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height), height, "Height must be positive.");
            return checked(width * height);
        }

        private void ReleaseTexture()
        {
            Texture2D released = texture;
            texture = null;
            if (released == null) return;

            if (Application.isPlaying)
                UnityEngine.Object.Destroy(released);
            else
                UnityEngine.Object.DestroyImmediate(released);
        }

        private void ResetCachedRaster()
        {
            hasCachedRaster = false;
            cachedStableKey = default;
            cachedWidth = 0;
            cachedHeight = 0;
        }

        private void ThrowIfDisposed()
        {
            if (disposed) throw new ObjectDisposedException(nameof(ExplorationMiniMapTerrainCache));
        }
    }
}
