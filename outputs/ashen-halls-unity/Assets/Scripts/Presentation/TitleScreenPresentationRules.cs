using System;
using System.Collections.Generic;
using UnityEngine;

namespace AshenHalls
{
    public enum TitleMenuChoiceKind
    {
        Continue,
        NewGame,
        Settings,
        Exit,
        Testing
    }

    public readonly struct TitleOpeningFrame
    {
        public readonly float BackdropReveal;
        public readonly float MenuAlpha;
        public readonly float MenuRise;
        public readonly float ChronicleAlpha;
        public readonly float VignetteAlpha;
        public readonly float HearthPulse;

        public TitleOpeningFrame(
            float backdropReveal,
            float menuAlpha,
            float menuRise,
            float chronicleAlpha,
            float vignetteAlpha,
            float hearthPulse)
        {
            BackdropReveal = Mathf.Clamp01(backdropReveal);
            MenuAlpha = Mathf.Clamp01(menuAlpha);
            MenuRise = Mathf.Max(0f, menuRise);
            ChronicleAlpha = Mathf.Clamp01(chronicleAlpha);
            VignetteAlpha = Mathf.Clamp01(vignetteAlpha);
            HearthPulse = Mathf.Clamp01(hearthPulse);
        }
    }

    public readonly struct TitleBackdropProjection
    {
        public readonly Rect CoverRect;

        public TitleBackdropProjection(Rect coverRect)
        {
            CoverRect = coverRect;
        }

        public Rect ProjectNormalized(Rect normalizedArtRect)
        {
            return new Rect(
                CoverRect.x + normalizedArtRect.x * CoverRect.width,
                CoverRect.y + normalizedArtRect.y * CoverRect.height,
                normalizedArtRect.width * CoverRect.width,
                normalizedArtRect.height * CoverRect.height);
        }
    }

    public static class TitleScreenPresentationRules
    {
        public const float RevealStrikeAt = 0.28f;
        public const float RevealChimeAt = 0.72f;
        public const float MenuRevealAt = 0.62f;
        public const float ChronicleRevealAt = 1.28f;
        public const float IdleChronicleSeconds = 7.5f;

        private static readonly string[] chronicleLines =
        {
            "Four names by the fire. One road beyond the rain.",
            "Midgaard keeps its lamps for those who find the way home.",
            "The Old Road remembers every oath - and every silence.",
            "Beyond the last gate, ash falls where stars once burned."
        };

        public static IReadOnlyList<string> ChronicleLines => chronicleLines;

        public static TitleOpeningFrame Evaluate(float elapsedSeconds, bool reducedMotion)
        {
            if (reducedMotion)
            {
                return new TitleOpeningFrame(1f, 1f, 0f, 1f, 0.58f, 0.46f);
            }

            float elapsed = Mathf.Max(0f, elapsedSeconds);
            float backdrop = Smooth01(elapsed / 0.92f);
            float menu = Smooth01((elapsed - MenuRevealAt) / 0.58f);
            float chronicle = Smooth01((elapsed - ChronicleRevealAt) / 0.72f);
            float settled = Smooth01((elapsed - 1.10f) / 1.20f);
            float hearth = 0.42f + Mathf.Sin(elapsed * 1.17f) * 0.08f + Mathf.Sin(elapsed * 0.43f + 0.8f) * 0.04f;
            return new TitleOpeningFrame(
                backdrop,
                menu,
                Mathf.Lerp(14f, 0f, menu),
                chronicle,
                Mathf.Lerp(0.74f, 0.58f, settled),
                hearth);
        }

        public static bool MenuInteractive(TitleOpeningFrame frame)
        {
            return frame.MenuAlpha >= 0.99f;
        }

        public static bool CrossedCue(
            float previousElapsed,
            float currentElapsed,
            float cueAt,
            bool reducedMotion,
            bool alreadyPlayed)
        {
            if (reducedMotion || alreadyPlayed) return false;
            float previous = Mathf.Max(0f, previousElapsed);
            float current = Mathf.Max(previous, currentElapsed);
            return previous < cueAt && current >= cueAt;
        }

        public static int ChronicleIndex(float elapsedSeconds, bool reducedMotion)
        {
            if (chronicleLines.Length == 0 || reducedMotion) return 0;
            float active = Mathf.Max(0f, elapsedSeconds - ChronicleRevealAt);
            return Mathf.FloorToInt(active / IdleChronicleSeconds) % chronicleLines.Length;
        }

        public static float ChronicleCycleAlpha(float elapsedSeconds, bool reducedMotion)
        {
            if (reducedMotion) return 1f;
            float active = Mathf.Max(0f, elapsedSeconds - ChronicleRevealAt);
            float phase = Mathf.Repeat(active, IdleChronicleSeconds) / IdleChronicleSeconds;
            float fadeIn = Smooth01(phase / 0.10f);
            float fadeOut = Smooth01((1f - phase) / 0.12f);
            return Mathf.Min(fadeIn, fadeOut);
        }

        public static int MenuIconIndex(TitleMenuChoiceKind kind)
        {
            switch (kind)
            {
                case TitleMenuChoiceKind.Continue: return 7;
                case TitleMenuChoiceKind.NewGame: return 2;
                case TitleMenuChoiceKind.Settings: return 4;
                case TitleMenuChoiceKind.Exit: return 5;
                case TitleMenuChoiceKind.Testing: return 8;
                default: return -1;
            }
        }

        public static TitleBackdropProjection ProjectBackdrop(
            float screenWidth,
            float screenHeight,
            float artWidth,
            float artHeight)
        {
            float width = Mathf.Max(1f, screenWidth);
            float height = Mathf.Max(1f, screenHeight);
            float sourceWidth = Mathf.Max(1f, artWidth);
            float sourceHeight = Mathf.Max(1f, artHeight);
            float scale = Mathf.Max(width / sourceWidth, height / sourceHeight);
            float coveredWidth = sourceWidth * scale;
            float coveredHeight = sourceHeight * scale;
            return new TitleBackdropProjection(new Rect(
                (width - coveredWidth) * 0.5f,
                (height - coveredHeight) * 0.5f,
                coveredWidth,
                coveredHeight));
        }

        public static bool Overlaps(Rect first, Rect second, float padding = 0f)
        {
            Rect expandedFirst = new Rect(
                first.x - padding,
                first.y - padding,
                first.width + padding * 2f,
                first.height + padding * 2f);
            return expandedFirst.Overlaps(second);
        }

        private static float Smooth01(float value)
        {
            value = Mathf.Clamp01(value);
            return value * value * (3f - 2f * value);
        }
    }
}
