using System;
using System.Collections.Generic;
using System.Text;

namespace AshenHalls
{
    public static class DialoguePagingRules
    {
        public const int DefaultSoftLimit = 250;
        public const int DefaultHardLimit = 340;

        public static string[] Paginate(string body, int softLimit = DefaultSoftLimit, int hardLimit = DefaultHardLimit)
        {
            string normalized = Normalize(body);
            if (string.IsNullOrEmpty(normalized)) return new[] { "..." };

            softLimit = Math.Max(80, softLimit);
            hardLimit = Math.Max(softLimit, hardLimit);
            List<string> beats = SentenceBeats(normalized, hardLimit);
            List<string> pages = new List<string>();
            StringBuilder page = new StringBuilder();
            foreach (string beat in beats)
            {
                if (string.IsNullOrWhiteSpace(beat)) continue;
                int joinedLength = page.Length + (page.Length > 0 ? 1 : 0) + beat.Length;
                if (page.Length > 0 && joinedLength > softLimit)
                {
                    pages.Add(page.ToString().Trim());
                    page.Length = 0;
                }

                if (page.Length > 0) page.Append(' ');
                page.Append(beat.Trim());
            }

            if (page.Length > 0) pages.Add(page.ToString().Trim());
            return pages.Count == 0 ? new[] { "..." } : pages.ToArray();
        }

        private static List<string> SentenceBeats(string body, int hardLimit)
        {
            List<string> beats = new List<string>();
            StringBuilder sentence = new StringBuilder();
            for (int i = 0; i < body.Length; i++)
            {
                char c = body[i];
                sentence.Append(c);
                bool paragraphBreak = c == '\n';
                bool sentenceBreak = c == '.' || c == '!' || c == '?';
                if (!paragraphBreak && !sentenceBreak && sentence.Length < hardLimit) continue;

                AddHardWrappedBeat(beats, sentence.ToString(), hardLimit);
                sentence.Length = 0;
                while (i + 1 < body.Length && char.IsWhiteSpace(body[i + 1])) i++;
            }

            AddHardWrappedBeat(beats, sentence.ToString(), hardLimit);
            return beats;
        }

        private static void AddHardWrappedBeat(List<string> beats, string value, int hardLimit)
        {
            string remaining = (value ?? "").Trim();
            while (remaining.Length > hardLimit)
            {
                int split = remaining.LastIndexOf(' ', hardLimit);
                if (split < hardLimit / 2) split = hardLimit;
                beats.Add(remaining.Substring(0, split).Trim());
                remaining = remaining.Substring(split).Trim();
            }
            if (remaining.Length > 0) beats.Add(remaining);
        }

        private static string Normalize(string body)
        {
            if (string.IsNullOrWhiteSpace(body)) return "";
            string value = body.Replace("\r\n", "\n").Replace('\r', '\n').Trim();
            StringBuilder result = new StringBuilder(value.Length);
            bool pendingSpace = false;
            foreach (char c in value)
            {
                if (c == '\n')
                {
                    while (result.Length > 0 && result[result.Length - 1] == ' ') result.Length--;
                    if (result.Length > 0 && result[result.Length - 1] != '\n') result.Append('\n');
                    pendingSpace = false;
                }
                else if (char.IsWhiteSpace(c))
                {
                    pendingSpace = true;
                }
                else
                {
                    if (pendingSpace && result.Length > 0 && result[result.Length - 1] != '\n') result.Append(' ');
                    result.Append(c);
                    pendingSpace = false;
                }
            }
            return result.ToString().Trim();
        }
    }
}
