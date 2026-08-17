using System;
using System.Collections.Generic;
using UnityEngine;

namespace AshenHalls
{
    public enum CombatUnitPresentationBeatKind
    {
        Hit,
        Defeat,
        Reveal,
        Unbind
    }

    public sealed class CombatUnitPresentationBeat
    {
        public readonly string UnitId;
        public readonly CombatUnitPresentationBeatKind Kind;
        public readonly float ImpactAt;
        public readonly float Until;
        public readonly float RecoilDirection;

        public CombatUnitPresentationBeat(
            string unitId,
            CombatUnitPresentationBeatKind kind,
            float impactAt,
            float until,
            float recoilDirection)
        {
            UnitId = (unitId ?? "").Trim();
            Kind = kind;
            ImpactAt = Math.Max(0f, impactAt);
            Until = Math.Max(ImpactAt, until);
            RecoilDirection = Mathf.Clamp(recoilDirection, -1f, 1f);
        }
    }

    public readonly struct CombatUnitPresentationPose
    {
        public readonly float OffsetX;
        public readonly float OffsetY;
        public readonly float Scale;
        public readonly float Alpha;

        public CombatUnitPresentationPose(float offsetX, float offsetY, float scale, float alpha)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;
            Scale = Mathf.Clamp(scale, 0.05f, 1.25f);
            Alpha = Mathf.Clamp01(alpha);
        }

        public static CombatUnitPresentationPose Identity =>
            new CombatUnitPresentationPose(0f, 0f, 1f, 1f);
    }

    public static class CombatUnitPresentationRules
    {
        public const int MaxActiveBeats = 32;

        public static CombatUnitPresentationBeat Create(
            string unitId,
            CombatUnitPresentationBeatKind kind,
            float impactAt,
            float recoilDirection = 0f)
        {
            float duration;
            switch (kind)
            {
                case CombatUnitPresentationBeatKind.Defeat:
                    duration = 0.34f;
                    break;
                case CombatUnitPresentationBeatKind.Reveal:
                    duration = 0.30f;
                    break;
                case CombatUnitPresentationBeatKind.Unbind:
                    duration = 0.38f;
                    break;
                default:
                    duration = 0.18f;
                    break;
            }

            return new CombatUnitPresentationBeat(
                unitId,
                kind,
                impactAt,
                impactAt + duration,
                recoilDirection);
        }

        public static void AddBounded(
            List<CombatUnitPresentationBeat> beats,
            CombatUnitPresentationBeat beat,
            float now)
        {
            if (beats == null || beat == null || string.IsNullOrEmpty(beat.UnitId)) return;
            PruneAndBound(beats, now);

            if (beat.Kind == CombatUnitPresentationBeatKind.Defeat || beat.Kind == CombatUnitPresentationBeatKind.Unbind)
            {
                beats.RemoveAll(candidate =>
                    candidate != null &&
                    string.Equals(candidate.UnitId, beat.UnitId, StringComparison.Ordinal));
            }
            else
            {
                beats.RemoveAll(candidate =>
                    candidate != null &&
                    candidate.Kind == beat.Kind &&
                    string.Equals(candidate.UnitId, beat.UnitId, StringComparison.Ordinal));
            }

            beats.Add(beat);
            PruneAndBound(beats, now);
        }

        public static void PruneAndBound(List<CombatUnitPresentationBeat> beats, float now)
        {
            if (beats == null) return;
            beats.RemoveAll(beat =>
                beat == null ||
                string.IsNullOrEmpty(beat.UnitId) ||
                now > beat.Until);

            while (beats.Count > MaxActiveBeats)
            {
                int earliestIndex = 0;
                float earliestUntil = beats[0].Until;
                for (int i = 1; i < beats.Count; i++)
                {
                    if (beats[i].Until >= earliestUntil) continue;
                    earliestUntil = beats[i].Until;
                    earliestIndex = i;
                }
                beats.RemoveAt(earliestIndex);
            }
        }

        public static float RemainingHoldDuration(
            IList<CombatUnitPresentationBeat> beats,
            float now)
        {
            if (beats == null) return 0f;
            float latest = now;
            for (int i = 0; i < beats.Count; i++)
            {
                CombatUnitPresentationBeat beat = beats[i];
                if (beat == null || beat.Until <= now) continue;
                latest = Math.Max(latest, beat.Until);
            }
            return Mathf.Clamp(latest - now, 0f, 0.85f);
        }

        public static bool TryGetBeat(
            IList<CombatUnitPresentationBeat> beats,
            string unitId,
            float now,
            out CombatUnitPresentationBeat result)
        {
            result = null;
            if (beats == null || string.IsNullOrEmpty(unitId)) return false;

            CombatUnitPresentationBeat pendingReveal = null;
            CombatUnitPresentationBeat active = null;
            CombatUnitPresentationBeat future = null;
            for (int i = 0; i < beats.Count; i++)
            {
                CombatUnitPresentationBeat beat = beats[i];
                if (beat == null ||
                    now > beat.Until ||
                    !string.Equals(beat.UnitId, unitId, StringComparison.Ordinal))
                {
                    continue;
                }

                if (beat.Kind == CombatUnitPresentationBeatKind.Defeat || beat.Kind == CombatUnitPresentationBeatKind.Unbind)
                {
                    if (result == null || beat.ImpactAt > result.ImpactAt) result = beat;
                    continue;
                }

                if (beat.Kind == CombatUnitPresentationBeatKind.Reveal && now < beat.ImpactAt)
                {
                    if (pendingReveal == null || beat.ImpactAt < pendingReveal.ImpactAt) pendingReveal = beat;
                    continue;
                }

                if (now >= beat.ImpactAt)
                {
                    if (active == null || beat.ImpactAt > active.ImpactAt) active = beat;
                }
                else if (future == null || beat.ImpactAt < future.ImpactAt)
                {
                    future = beat;
                }
            }

            if (result != null) return true;
            result = pendingReveal ?? active ?? future;
            return result != null;
        }

        public static bool ShouldRenderActor(
            bool alive,
            CombatUnitPresentationBeat beat,
            float now)
        {
            if (beat == null || now > beat.Until) return alive;
            if (beat.Kind == CombatUnitPresentationBeatKind.Defeat || beat.Kind == CombatUnitPresentationBeatKind.Unbind) return true;
            if (beat.Kind == CombatUnitPresentationBeatKind.Reveal && now < beat.ImpactAt) return false;
            return alive;
        }

        public static bool ShouldRenderTacticalOverlay(
            bool alive,
            CombatUnitPresentationBeat beat,
            float now)
        {
            if (!alive) return false;
            return beat == null ||
                beat.Kind != CombatUnitPresentationBeatKind.Reveal ||
                now >= beat.ImpactAt;
        }

        public static CombatUnitPresentationPose PoseFor(
            CombatUnitPresentationBeat beat,
            float now,
            bool reducedMotion)
        {
            if (beat == null || reducedMotion || now < beat.ImpactAt || now > beat.Until)
            {
                return CombatUnitPresentationPose.Identity;
            }

            float span = Mathf.Max(0.01f, beat.Until - beat.ImpactAt);
            float progress = Mathf.Clamp01((now - beat.ImpactAt) / span);
            switch (beat.Kind)
            {
                case CombatUnitPresentationBeatKind.Reveal:
                {
                    float rise = Smooth01(progress);
                    float scale = progress < 0.64f
                        ? Mathf.Lerp(0.46f, 1.08f, Smooth01(progress / 0.64f))
                        : Mathf.Lerp(1.08f, 1f, Smooth01((progress - 0.64f) / 0.36f));
                    return new CombatUnitPresentationPose(
                        0f,
                        0.18f * (1f - rise),
                        scale,
                        Smooth01(progress / 0.28f));
                }
                case CombatUnitPresentationBeatKind.Defeat:
                {
                    float recoil = Mathf.Sin(Mathf.Clamp01(progress / 0.42f) * Mathf.PI);
                    float fall = SmoothRange(progress, 0.18f, 1f);
                    float fade = 1f - SmoothRange(progress, 0.38f, 1f);
                    return new CombatUnitPresentationPose(
                        beat.RecoilDirection * recoil * 0.14f,
                        fall * 0.22f,
                        Mathf.Lerp(1f, 0.78f, fall),
                        fade);
                }
                case CombatUnitPresentationBeatKind.Unbind:
                {
                    float tighten = SmoothRange(progress, 0f, 0.62f);
                    float fade = 1f - SmoothRange(progress, 0.18f, 1f);
                    float twist = Mathf.Sin(progress * Mathf.PI * 2f) * (1f - progress);
                    return new CombatUnitPresentationPose(
                        beat.RecoilDirection * twist * 0.09f,
                        -progress * 0.20f,
                        Mathf.Lerp(1f, 0.38f, tighten),
                        fade);
                }
                default:
                {
                    float recoil = Mathf.Sin(progress * Mathf.PI);
                    return new CombatUnitPresentationPose(
                        beat.RecoilDirection * recoil * 0.11f,
                        -Mathf.Sin(progress * Mathf.PI) * 0.025f,
                        1f + Mathf.Sin(progress * Mathf.PI) * 0.035f,
                        1f);
                }
            }
        }

        public static Rect ApplyPose(Rect rect, CombatUnitPresentationPose pose)
        {
            float width = rect.width * pose.Scale;
            float height = rect.height * pose.Scale;
            return new Rect(
                rect.center.x - width * 0.5f + pose.OffsetX * rect.width,
                rect.center.y - height * 0.5f + pose.OffsetY * rect.height,
                width,
                height);
        }

        private static float Smooth01(float value)
        {
            float t = Mathf.Clamp01(value);
            return t * t * (3f - 2f * t);
        }

        private static float SmoothRange(float value, float start, float end)
        {
            if (end <= start) return value >= end ? 1f : 0f;
            return Smooth01((value - start) / (end - start));
        }
    }
}
