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

    public readonly struct TitleMenuScrollStyle
    {
        public readonly Color Paper;
        public readonly Color Roll;
        public readonly Color Edge;
        public readonly Color Ink;
        public readonly Color Accent;
        public readonly Color Selection;
        public readonly Color SelectionInk;
        public readonly Color Shadow;

        public TitleMenuScrollStyle(
            Color paper,
            Color roll,
            Color edge,
            Color ink,
            Color accent,
            Color selection,
            Color selectionInk,
            Color shadow)
        {
            Paper = paper;
            Roll = roll;
            Edge = edge;
            Ink = ink;
            Accent = accent;
            Selection = selection;
            SelectionInk = selectionInk;
            Shadow = shadow;
        }
    }

    public readonly struct TitleOpeningFrame
    {
        public readonly float BackdropReveal;
        public readonly float MenuAlpha;
        public readonly float MenuRise;
        public readonly float VignetteAlpha;
        public readonly float HearthPulse;

        public TitleOpeningFrame(
            float backdropReveal,
            float menuAlpha,
            float menuRise,
            float vignetteAlpha,
            float hearthPulse)
        {
            BackdropReveal = Mathf.Clamp01(backdropReveal);
            MenuAlpha = Mathf.Clamp01(menuAlpha);
            MenuRise = Mathf.Max(0f, menuRise);
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
        public const int MenuScrollTextureWidth = 1280;
        public const int MenuScrollTextureHeight = 1280;
        public const float MenuScrollPixelsPerUnit = 1500f;
        public const float MenuScrollReferencePixelsPerUnit = 100f;
        public const int MenuFocusTextureWidth = 2048;
        public const int MenuFocusTextureHeight = 768;
        public const float MenuFocusPixelsPerUnit = 1200f;
        public static Vector4 MenuScrollSpriteBorder => new Vector4(360f, 360f, 360f, 360f);
        public static Rect MenuFocusSpriteRect => new Rect(0f, 192f, 2048f, 384f);
        public static Vector4 MenuFocusSpriteBorder => new Vector4(360f, 96f, 360f, 96f);
        public static float MenuScrollSideInset => MenuScrollSpriteBorder.x
            / (MenuScrollPixelsPerUnit / MenuScrollReferencePixelsPerUnit)
            + 7f;
        public static float MenuScrollTopInset => MenuScrollSpriteBorder.w
            / (MenuScrollPixelsPerUnit / MenuScrollReferencePixelsPerUnit)
            + 8f;
        public static float MenuScrollBottomInset => MenuScrollSpriteBorder.y
            / (MenuScrollPixelsPerUnit / MenuScrollReferencePixelsPerUnit);
        // The approved Grand Hearth painting deliberately leaves these horizontal
        // bands quiet enough for the title plaque and menu. Project them through
        // the cover crop instead of treating screen percentages as art positions.
        public static Rect LogoSafeZoneNormalized => new Rect(0f, 0f, 0.42f, 1f);
        public static Rect MenuSafeZoneNormalized => new Rect(0.73f, 0f, 0.27f, 1f);
        public static TitleMenuScrollStyle MenuScrollStyle => new TitleMenuScrollStyle(
            new Color(0.784f, 0.706f, 0.467f, 0.98f),
            new Color(0.561f, 0.412f, 0.227f, 1f),
            new Color(0.227f, 0.129f, 0.078f, 1f),
            new Color(0.149f, 0.086f, 0.055f, 1f),
            new Color(0.776f, 0.584f, 0.298f, 1f),
            new Color(0.357f, 0.184f, 0.118f, 1f),
            new Color(0.957f, 0.898f, 0.761f, 1f),
            new Color(0.012f, 0.008f, 0.004f, 0.68f));

        public static bool SupportsMenuScrollArt(Texture2D texture)
        {
            return texture != null
                && texture.width == MenuScrollTextureWidth
                && texture.height == MenuScrollTextureHeight;
        }

        public static bool SupportsMenuFocusArt(Texture2D texture)
        {
            return texture != null
                && texture.width == MenuFocusTextureWidth
                && texture.height == MenuFocusTextureHeight;
        }

        public static TitleOpeningFrame Evaluate(float elapsedSeconds, bool reducedMotion)
        {
            if (reducedMotion)
            {
                return new TitleOpeningFrame(1f, 1f, 0f, 0.58f, 0.46f);
            }

            float elapsed = Mathf.Max(0f, elapsedSeconds);
            float backdrop = Smooth01(elapsed / 0.92f);
            float menu = Smooth01((elapsed - MenuRevealAt) / 0.58f);
            float settled = Smooth01((elapsed - 1.10f) / 1.20f);
            float hearth = 0.42f + Mathf.Sin(elapsed * 1.17f) * 0.08f + Mathf.Sin(elapsed * 0.43f + 0.8f) * 0.04f;
            return new TitleOpeningFrame(
                backdrop,
                menu,
                Mathf.Lerp(14f, 0f, menu),
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

        public static float RelativeLuminance(Color color)
        {
            float red = LinearChannel(Mathf.Clamp01(color.r));
            float green = LinearChannel(Mathf.Clamp01(color.g));
            float blue = LinearChannel(Mathf.Clamp01(color.b));
            return red * 0.2126f + green * 0.7152f + blue * 0.0722f;
        }

        public static float ContrastRatio(Color first, Color second)
        {
            float firstLuminance = RelativeLuminance(first);
            float secondLuminance = RelativeLuminance(second);
            float lighter = Mathf.Max(firstLuminance, secondLuminance);
            float darker = Mathf.Min(firstLuminance, secondLuminance);
            return (lighter + 0.05f) / (darker + 0.05f);
        }

        private static float LinearChannel(float channel)
        {
            return channel <= 0.04045f
                ? channel / 12.92f
                : Mathf.Pow((channel + 0.055f) / 1.055f, 2.4f);
        }

        private static float Smooth01(float value)
        {
            value = Mathf.Clamp01(value);
            return value * value * (3f - 2f * value);
        }
    }
}
