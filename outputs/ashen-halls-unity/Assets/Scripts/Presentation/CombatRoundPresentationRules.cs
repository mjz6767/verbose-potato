using System;

namespace AshenHalls
{
    public readonly struct CombatRoundPresentationSummary
    {
        public readonly int Round;
        public readonly int ExpiredFields;
        public readonly int OpenedRituals;
        public readonly string BannerText;
        public readonly float DurationSeconds;

        public CombatRoundPresentationSummary(
            int round,
            int expiredFields,
            int openedRituals,
            string bannerText,
            float durationSeconds)
        {
            Round = Math.Max(1, round);
            ExpiredFields = Math.Max(0, expiredFields);
            OpenedRituals = Math.Max(0, openedRituals);
            BannerText = bannerText ?? "";
            DurationSeconds = Math.Max(0f, durationSeconds);
        }

        public bool HasFieldChanges => ExpiredFields > 0;

        public bool HasRitualChanges => OpenedRituals > 0;
    }

    public static class CombatRoundPresentationRules
    {
        public const int MaxBannerCharacters = 48;
        public const float StandardDurationSeconds = 0.56f;
        public const float ReducedMotionDurationSeconds = 0.06f;

        public static CombatRoundPresentationSummary Create(
            int round,
            int expiredFields,
            int openedRituals,
            bool reducedMotion)
        {
            int safeRound = Math.Max(1, round);
            int safeExpiredFields = Math.Max(0, expiredFields);
            int safeOpenedRituals = Math.Max(0, openedRituals);
            return new CombatRoundPresentationSummary(
                safeRound,
                safeExpiredFields,
                safeOpenedRituals,
                BuildBannerText(safeRound, safeExpiredFields, safeOpenedRituals),
                Duration(reducedMotion));
        }

        public static string BuildBannerText(int round, int expiredFields, int openedRituals)
        {
            int safeRound = Math.Max(1, round);
            int safeExpiredFields = Math.Max(0, expiredFields);
            int safeOpenedRituals = Math.Max(0, openedRituals);
            string text = "ROUND " + BoundedCount(safeRound, 999);

            if (safeExpiredFields > 0)
            {
                text += safeExpiredFields == 1
                    ? " \u2022 1 field fades"
                    : " \u2022 " + BoundedCount(safeExpiredFields, 9) + " fields fade";
            }

            if (safeOpenedRituals > 0)
            {
                text += safeOpenedRituals == 1
                    ? " \u2022 1 ritual opens"
                    : " \u2022 " + BoundedCount(safeOpenedRituals, 9) + " rituals open";
            }

            return text;
        }

        public static float Duration(bool reducedMotion)
        {
            return reducedMotion ? ReducedMotionDurationSeconds : StandardDurationSeconds;
        }

        private static string BoundedCount(int value, int maximum)
        {
            return value > maximum ? maximum + "+" : Math.Max(0, value).ToString();
        }
    }
}
