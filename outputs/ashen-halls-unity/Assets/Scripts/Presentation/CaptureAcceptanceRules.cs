using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public readonly struct CapturePixelSample
    {
        public readonly byte Red;
        public readonly byte Green;
        public readonly byte Blue;

        public CapturePixelSample(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public int Brightness => Math.Max(Red, Math.Max(Green, Blue));
    }

    public enum CaptureAcceptanceFailure
    {
        None,
        InvalidRequestedDimensions,
        ScreenDimensionsDifferFromRequest,
        PngDimensionsDifferFromScreen,
        NoPixelSamples,
        NearUniformBlack
    }

    public readonly struct CaptureAcceptanceResult
    {
        public readonly CaptureAcceptanceFailure Failure;
        public readonly int SampleCount;
        public readonly int NearBlackSampleCount;
        public readonly int MinimumBrightness;
        public readonly int MaximumBrightness;

        public CaptureAcceptanceResult(
            CaptureAcceptanceFailure failure,
            int sampleCount = 0,
            int nearBlackSampleCount = 0,
            int minimumBrightness = 0,
            int maximumBrightness = 0)
        {
            Failure = failure;
            SampleCount = sampleCount;
            NearBlackSampleCount = nearBlackSampleCount;
            MinimumBrightness = minimumBrightness;
            MaximumBrightness = maximumBrightness;
        }

        public bool Accepted => Failure == CaptureAcceptanceFailure.None;
    }

    public static class CaptureAcceptanceRules
    {
        public const int NearBlackBrightness = 16;
        public const int UniformDarkBrightness = 24;
        public const int UniformDarkBrightnessRange = 8;
        public const int RejectedNearBlackPercent = 98;

        public static CaptureAcceptanceResult Evaluate(
            int requestedWidth,
            int requestedHeight,
            int screenWidth,
            int screenHeight,
            int pngWidth,
            int pngHeight,
            IReadOnlyList<CapturePixelSample> samples)
        {
            if (requestedWidth <= 0 || requestedHeight <= 0)
            {
                return new CaptureAcceptanceResult(CaptureAcceptanceFailure.InvalidRequestedDimensions);
            }

            if (screenWidth != requestedWidth || screenHeight != requestedHeight)
            {
                return new CaptureAcceptanceResult(CaptureAcceptanceFailure.ScreenDimensionsDifferFromRequest);
            }

            if (pngWidth != screenWidth || pngHeight != screenHeight)
            {
                return new CaptureAcceptanceResult(CaptureAcceptanceFailure.PngDimensionsDifferFromScreen);
            }

            if (samples == null || samples.Count == 0)
            {
                return new CaptureAcceptanceResult(CaptureAcceptanceFailure.NoPixelSamples);
            }

            int nearBlackCount = 0;
            int minimumBrightness = byte.MaxValue;
            int maximumBrightness = byte.MinValue;
            for (int i = 0; i < samples.Count; i++)
            {
                int brightness = samples[i].Brightness;
                if (brightness <= NearBlackBrightness) nearBlackCount++;
                minimumBrightness = Math.Min(minimumBrightness, brightness);
                maximumBrightness = Math.Max(maximumBrightness, brightness);
            }

            bool overwhelminglyBlack = (long)nearBlackCount * 100L
                >= (long)samples.Count * RejectedNearBlackPercent;
            bool uniformlyDark = maximumBrightness <= UniformDarkBrightness
                && maximumBrightness - minimumBrightness <= UniformDarkBrightnessRange;
            CaptureAcceptanceFailure failure = overwhelminglyBlack || uniformlyDark
                ? CaptureAcceptanceFailure.NearUniformBlack
                : CaptureAcceptanceFailure.None;
            return new CaptureAcceptanceResult(
                failure,
                samples.Count,
                nearBlackCount,
                minimumBrightness,
                maximumBrightness);
        }
    }
}
