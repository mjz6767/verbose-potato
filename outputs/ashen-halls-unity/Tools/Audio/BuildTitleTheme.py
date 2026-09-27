#!/usr/bin/env python3
"""Render only the main title and opening music/menu-accent previews.

No recordings, network services, or other score masters are modified. The
shared bank manifest is updated by the coordinating release build after this
tool emits the replacement asset row in Docs/TITLE_THEME_VALIDATION.json.
Previews reconstruct the fade and foreground cues; ambience is excluded.
"""
from __future__ import annotations

import argparse
from dataclasses import asdict
import json
from pathlib import Path
import re

import numpy as np

import BuildOriginalAudio as audio
import TitleTheme

ROOT = Path(__file__).resolve().parents[2]
QA = ROOT / "QA/title-theme-2026-09-27"
MASTER = audio.MUSIC_DIR / (audio.TITLE_MUSIC_CUE + ".wav")
REPORT = ROOT / "Docs/TITLE_THEME_VALIDATION.json"


def runtime_settings() -> dict[str, float]:
    rules = (ROOT / "Assets/Scripts/Presentation/TitleAudioRules.cs").read_text(encoding="utf-8-sig")
    timing = (ROOT / "Assets/Scripts/Presentation/MusicTransitionRules.cs").read_text(encoding="utf-8-sig")

    def number(source: str, pattern: str) -> float:
        match = re.search(pattern, source)
        if not match:
            raise ValueError("Cannot verify the runtime title mix: " + pattern)
        return float(match.group(1))

    return {
        "intro_fade_seconds": number(timing, r"TitleIntroFadeDuration\s*=\s*([0-9.]+)f"),
        "music_source_gain": number(rules, r"TitleMusicSourceGain\s*=\s*([0-9.]+)f"),
        "default_music_fraction": audio.DEFAULT_MUSIC_VOLUME_FRACTION,
        "sfx_source_gain": audio.RUNTIME_SFX_SOURCE_GAIN,
        "forge_gain": number(rules, r'case "impactlow": return new TitleAudioCueProfile\(RevealStrikeKey, ([0-9.]+)f\)'),
        "reveal_gain": number(rules, r'case "uiconfirm": return new TitleAudioCueProfile\(RevealChimeKey, ([0-9.]+)f\)'),
    }


def runtime_opening(samples: np.ndarray, settings: dict[str, float], seconds: float = 20.0) -> np.ndarray:
    frames = round(seconds * audio.MUSIC_SAMPLE_RATE)
    result = samples[:, :frames].copy()
    time = np.arange(frames) / audio.MUSIC_SAMPLE_RATE
    progress = np.clip(time / max(.001, settings["intro_fade_seconds"]), 0, 1)
    fade = progress * progress * (3 - 2 * progress)
    result *= fade * settings["music_source_gain"] * settings["default_music_fraction"]
    for key, when, gain in (
        ("titleforge", .28, settings["forge_gain"]),
        ("titlereveal", .72, settings["reveal_gain"]),
        ("titlefocus", 7.5, .20),
        ("titlefocus", 8.3, .20),
    ):
        cue, rate = audio.read_pcm16(audio.SFX_DIR / (key + ".wav"))
        if cue.ndim != 1:
            raise ValueError(key + " must be mono")
        target = np.arange(round(cue.size * audio.MUSIC_SAMPLE_RATE / rate)) / audio.MUSIC_SAMPLE_RATE
        cue = np.interp(target, np.arange(cue.size) / rate, cue)
        start = round(when * audio.MUSIC_SAMPLE_RATE)
        count = min(cue.size, frames - start)
        if count > 0:
            result[:, start:start + count] += cue[:count] * gain * settings["sfx_source_gain"]
    audio.enforce_runtime_preview_mix_contract("main-title cold opening", result)
    return result


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--check", action="store_true", help="Check exact master reproducibility without modifying files.")
    args = parser.parse_args()
    spec = next(item for item in audio.TRACKS if item.cue == audio.TITLE_MUSIC_CUE)
    rendered = TitleTheme.compose(spec)
    if rendered.shape != (2, 60 * audio.MUSIC_SAMPLE_RATE) or not np.all(np.isfinite(rendered)):
        raise ValueError("Title render must be finite 60-second, 32 kHz stereo audio")
    if float(np.max(np.abs(rendered))) > 10 ** (-3 / 20):
        raise ValueError("Title master exceeds the reserved output headroom")
    if not args.check:
        QA.mkdir(parents=True, exist_ok=True)
        # Keep the previous title as an immutable local comparison on rebuilds.
        previous = QA / "title-before.wav"
        if MASTER.exists() and not previous.exists():
            previous.write_bytes(MASTER.read_bytes())
        audio.write_pcm16(MASTER, rendered, audio.MUSIC_SAMPLE_RATE)
    written, rate = audio.read_pcm16(MASTER)
    if rate != audio.MUSIC_SAMPLE_RATE:
        raise ValueError("Unexpected title sample rate")
    expected = np.round(rendered * 32767).astype("<i2").astype(np.float64) / 32768
    if not np.array_equal(expected, written):
        raise ValueError("Saved title does not reproduce exactly from the authored score")
    settings = runtime_settings()
    opening = runtime_opening(written, settings)
    if not args.check:
        (QA / "the-ember-oath.wav").write_bytes(MASTER.read_bytes())
        audio.write_pcm16(QA / "in-game-opening.wav", opening, rate)
        # Include an untouched loop boundary at the middle of the audition.
        loop_review = np.concatenate((written[:, -rate * 7:], written[:, :rate * 10]), axis=1)
        audio.write_pcm16(QA / "loop-boundary.wav", loop_review, rate)
        old, old_rate = audio.read_pcm16(QA / "title-before.wav")
        if old_rate == rate:
            old_settings = dict(settings, intro_fade_seconds=2.0, forge_gain=.28, reveal_gain=.22)
            old_opening = runtime_opening(old, old_settings)
            comparison = np.concatenate((old_opening, np.zeros((2, rate)), opening), axis=1)
            audio.write_pcm16(QA / "before-after-opening.wav", comparison, rate)
        row = asdict(audio.metrics_for(spec.cue, spec.title,
            spec.direction,
            "music", MASTER, written, rate))
        report = {
            "generator": "Tools/Audio/BuildTitleTheme.py",
            "original_composition": True,
            "external_samples": False,
            "asset": row,
            "score": TitleTheme.metadata(),
            "runtime_mix": settings,
            "exact_pcm_reproducibility": True,
            "preview_scope": "Opening music with source-derived fade/gains and staged title/menu accents; ambience excluded. Not a Unity player recording.",
            "prior_title_sha256": audio.sha256(QA / "title-before.wav"),
            "previews": [{"path": str(path.relative_to(ROOT)).replace("\\", "/"), "sha256": audio.sha256(path)}
                for path in (QA / "the-ember-oath.wav", QA / "in-game-opening.wav", QA / "loop-boundary.wav", QA / "before-after-opening.wav")
                if path.exists()],
            "review_scope": "Structural and signal checks do not establish subjective musical quality. Listening previews supplied for review.",
        }
        REPORT.write_text(json.dumps(report, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    print("Title master: exact 60-second PCM reproduction; opening music/menu mix verified against current source settings (ambience excluded).")


if __name__ == "__main__":
    main()
