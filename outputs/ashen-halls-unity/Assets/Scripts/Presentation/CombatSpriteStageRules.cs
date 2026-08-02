using UnityEngine;

namespace AshenHalls
{
    public readonly struct CombatSpriteStageGeometry
    {
        public readonly Rect Footprint;
        public readonly Rect FootprintCore;
        public readonly Rect Footlight;
        public readonly Rect FootlightCore;
        public readonly Rect LeftRim;
        public readonly Rect RightRim;
        public readonly Rect ActiveTick;
        public readonly Rect ActiveTickCore;

        public CombatSpriteStageGeometry(
            Rect footprint,
            Rect footprintCore,
            Rect footlight,
            Rect footlightCore,
            Rect leftRim,
            Rect rightRim,
            Rect activeTick,
            Rect activeTickCore)
        {
            Footprint = footprint;
            FootprintCore = footprintCore;
            Footlight = footlight;
            FootlightCore = footlightCore;
            LeftRim = leftRim;
            RightRim = rightRim;
            ActiveTick = activeTick;
            ActiveTickCore = activeTickCore;
        }
    }

    public static class CombatSpriteStageRules
    {
        public static CombatSpriteStageGeometry GeometryFor(Rect anchoredRect)
        {
            float hairline = Mathf.Max(1f, Mathf.Round(anchoredRect.width * 0.018f));
            Rect footprint = new Rect(
                anchoredRect.x + anchoredRect.width * 0.16f,
                anchoredRect.y + anchoredRect.height * 0.785f,
                anchoredRect.width * 0.68f,
                Mathf.Max(4f, anchoredRect.height * 0.105f));
            Rect footprintCore = new Rect(
                anchoredRect.x + anchoredRect.width * 0.27f,
                anchoredRect.y + anchoredRect.height * 0.812f,
                anchoredRect.width * 0.46f,
                Mathf.Max(2f, anchoredRect.height * 0.052f));
            Rect footlight = new Rect(
                anchoredRect.x + anchoredRect.width * 0.20f,
                anchoredRect.y + anchoredRect.height * 0.585f,
                anchoredRect.width * 0.60f,
                anchoredRect.height * 0.235f);
            Rect footlightCore = new Rect(
                anchoredRect.x + anchoredRect.width * 0.32f,
                anchoredRect.y + anchoredRect.height * 0.675f,
                anchoredRect.width * 0.36f,
                anchoredRect.height * 0.135f);
            Rect leftRim = new Rect(
                anchoredRect.x + anchoredRect.width * 0.155f,
                anchoredRect.y + anchoredRect.height * 0.34f,
                hairline,
                anchoredRect.height * 0.36f);
            Rect rightRim = new Rect(
                anchoredRect.xMax - anchoredRect.width * 0.155f - hairline,
                anchoredRect.y + anchoredRect.height * 0.34f,
                hairline,
                anchoredRect.height * 0.36f);
            Rect activeTick = new Rect(
                anchoredRect.x + anchoredRect.width * 0.21f,
                anchoredRect.y + anchoredRect.height * 0.905f,
                anchoredRect.width * 0.58f,
                Mathf.Max(3f, anchoredRect.height * 0.034f));
            Rect activeTickCore = new Rect(
                anchoredRect.x + anchoredRect.width * 0.40f,
                activeTick.y,
                anchoredRect.width * 0.20f,
                activeTick.height);
            return new CombatSpriteStageGeometry(
                footprint,
                footprintCore,
                footlight,
                footlightCore,
                leftRim,
                rightRim,
                activeTick,
                activeTickCore);
        }

        public static float ActivePulse(float now, bool reducedMotion)
        {
            return reducedMotion
                ? 0.64f
                : 0.52f + Mathf.Sin(now * 5.4f) * 0.16f;
        }
    }
}
