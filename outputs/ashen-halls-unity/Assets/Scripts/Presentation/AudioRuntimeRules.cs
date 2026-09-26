using System;

namespace AshenHalls
{
    public static class AudioRuntimeRules
    {
        public const int MaximumCueVariants = 8;
        public const float MusicRetargetSettleSeconds = 0.18f;
        public const float MaximumScheduledCueLateness = 0.25f;

        public static float RetargetTransitionDuration(float currentDuration, float progress)
        {
            float duration = MusicTransitionRules.ClampDuration(currentDuration);
            float remainingFraction = 1f - MusicTransitionRules.Clamp01(progress);
            return remainingFraction <= 0f ? duration
                : Math.Min(duration, MusicRetargetSettleSeconds / remainingFraction);
        }

        public static bool IsScheduledCueExpired(float now, float playAt)
        {
            return float.IsNaN(now) || float.IsInfinity(now)
                || float.IsNaN(playAt) || float.IsInfinity(playAt)
                || now - playAt > MaximumScheduledCueLateness;
        }

        public static int VariantIndex(uint seed, int serial, int count)
        {
            count = Math.Max(1, Math.Min(MaximumCueVariants, count));
            if (count == 1) return 0;
            int offset = (int)(seed % (uint)count);
            int stride = 1 + (int)((seed >> 8) % (uint)(count - 1));
            // A coprime stride visits every take exactly once before repeating.
            while (GreatestCommonDivisor(stride, count) != 1)
                stride = stride % (count - 1) + 1;
            return (int)((offset + (long)Math.Max(0, serial) * stride) % count);
        }

        private static int GreatestCommonDivisor(int left, int right)
        {
            while (right != 0)
            {
                int remainder = left % right;
                left = right;
                right = remainder;
            }
            return left;
        }
    }
}
