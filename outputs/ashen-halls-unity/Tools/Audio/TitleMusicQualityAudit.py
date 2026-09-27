#!/usr/bin/env python3
"""Independent title-score engineering audit; never a score of musical quality.

Measures saved PCM rather than trusting composer metadata. Optional FFmpeg
provides standardized loudness/true peak. Prominence, memorability, convincing
instruments, emotional payoff and fatigue still require human listening.
"""
from __future__ import annotations

import argparse
import hashlib
import json
import math
from pathlib import Path
import re
import subprocess
import wave

import numpy as np

ROOT = Path(__file__).resolve().parents[2]
MASTER = ROOT / "Assets/Resources/Audio/Music/tavern_storm_hearth_ensemble_loop.wav"
DEFAULT_SECTIONS = (("signature", 0, 10), ("company", 10, 20), ("intimate", 20, 30),
                    ("ascent", 30, 40), ("return", 40, 55), ("coda", 55, 60))


def db(value: float) -> float:
    return 20.0 * math.log10(max(float(value), 1e-12))


def rms(audio: np.ndarray) -> float:
    return float(np.sqrt(np.mean(np.square(audio))))


def read_pcm(path: Path) -> tuple[np.ndarray, int]:
    with wave.open(str(path), "rb") as handle:
        if handle.getsampwidth() != 2 or handle.getnchannels() != 2:
            raise ValueError("Expected 16-bit stereo source master")
        rate = handle.getframerate()
        pcm = handle.readframes(handle.getnframes())
    return np.frombuffer(pcm, dtype="<i2").astype(np.float64).reshape(-1, 2).T / 32768.0, rate


def planned_sections() -> tuple:
    try:
        import TitleTheme
    except ModuleNotFoundError:
        return DEFAULT_SECTIONS
    return tuple((name, start * 60 / TitleTheme.BPM, end * 60 / TitleTheme.BPM)
                 for name, start, end in TitleTheme.SECTIONS)


def measure(audio: np.ndarray, rate: int, sections: tuple = DEFAULT_SECTIONS) -> dict:
    if audio.ndim != 2 or audio.shape[0] != 2 or audio.shape[1] < rate:
        raise ValueError("Expected at least one second of two-channel audio")
    if not np.all(np.isfinite(audio)):
        raise ValueError("Non-finite audio samples")
    mono = np.mean(audio, axis=0)
    stereo_rms = rms(audio)
    spectrum = np.abs(np.fft.rfft(audio, axis=1)) ** 2
    freq = np.fft.rfftfreq(audio.shape[1], 1.0 / rate)
    spectral_total = max(float(spectrum.sum()), 1e-20)
    band_energy = {
        name: float(spectrum[:, (freq >= lower) & (freq < upper)].sum()) / spectral_total
        for name, lower, upper in (("sub_0_80", 0, 80), ("bass_80_250", 80, 250),
                                   ("lowmid_250_1000", 250, 1000), ("presence_1000_4000", 1000, 4000),
                                   ("bright_4000_8000", 4000, 8000), ("air_8000_up", 8000, rate / 2 + 1))
    }
    # This idealized bandpass is an audibility proxy, not a speaker simulation.
    mono_fft = np.fft.rfft(mono)
    mono_fft[(freq < 180) | (freq > 6000)] = 0
    bandwidth_proxy = np.fft.irfft(mono_fft, n=mono.size)
    section_stats = {}
    for name, start, end in sections:
        clip = audio[:, round(start * rate):round(end * rate)]
        if clip.size == 0:
            raise ValueError("Empty requested section: " + name)
        section_stats[name] = {"start_seconds": start, "end_seconds": end,
                               "rms_dbfs": db(rms(clip)),
                               "mono_retention": rms(clip.mean(axis=0)) / max(rms(clip), 1e-12)}
    edge = round(rate * .25)
    left_rms, right_rms = rms(audio[0]), rms(audio[1])
    return {
        "sample_rate": rate, "channels": 2, "duration_seconds": audio.shape[1] / rate,
        "peak_dbfs": db(np.max(np.abs(audio))), "rms_dbfs": db(stereo_rms),
        "crest_db": db(np.max(np.abs(audio)) / max(stereo_rms, 1e-12)),
        "mono_retention": rms(mono) / max(stereo_rms, 1e-12),
        "channel_level_difference_db": abs(db(left_rms) - db(right_rms)),
        "channel_correlation": float(np.corrcoef(audio)[0, 1]) if stereo_rms > 1e-10 else 0.0,
        "dc_peak": float(np.max(np.abs(audio.mean(axis=1)))),
        "boundary_delta": float(np.max(np.abs(audio[:, 0] - audio[:, -1]))),
        "loop_edge_level_difference_db": abs(db(rms(audio[:, :edge])) - db(rms(audio[:, -edge:]))),
        "first_five_seconds_rms_dbfs": db(rms(audio[:, :5 * rate])),
        "band_energy_fraction": band_energy,
        "band_limited_mono_rms_dbfs": db(rms(bandwidth_proxy)),
        "band_limited_retention_db": db(rms(bandwidth_proxy) / max(stereo_rms, 1e-12)),
        "sections": section_stats,
    }


def engineering_failures(metrics: dict) -> list[str]:
    errors = []
    checks = (
        (metrics["sample_rate"] == 32000, "Source sample rate must remain 32 kHz"),
        (abs(metrics["duration_seconds"] - 60) < .001, "Title must remain exactly sixty seconds"),
        (metrics["peak_dbfs"] <= -2.9, "Master needs at least 3 dB sample headroom"),
        (-24 <= metrics["rms_dbfs"] <= -14, "Master body outside practical -24 to -14 dBFS window"),
        (metrics["mono_retention"] >= .80, "More than 20 percent RMS body is lost in mono"),
        (metrics["dc_peak"] <= .001, "Excessive DC offset"),
        (metrics["boundary_delta"] <= .002, "Loop endpoint discontinuity"),
        (metrics["channel_level_difference_db"] <= 3.0, "Unbalanced channel levels"),
        (metrics["first_five_seconds_rms_dbfs"] >= metrics["rms_dbfs"] - 7,
         "Opening body more than 7 dB below the whole-track body"),
    )
    errors.extend(message for good, message in checks if not good)
    for name, section in metrics["sections"].items():
        if section["mono_retention"] < .75:
            errors.append("Section loses body in mono: " + name)
    # This checks the agreed arrangement brief, not whether it sounds compelling.
    sections = metrics["sections"]
    if "intimate" in sections and "return" in sections:
        if sections["return"]["rms_dbfs"] - sections["intimate"]["rms_dbfs"] < 3:
            errors.append("Intimate bridge lacks the agreed 3 dB space before the return")
    return errors


def loudness(path: Path) -> dict:
    try:
        import imageio_ffmpeg
        executable = imageio_ffmpeg.get_ffmpeg_exe()
    except (ImportError, RuntimeError):
        return {"available": False}
    result = subprocess.run([executable, "-hide_banner", "-nostats", "-i", str(path),
                             "-af", "ebur128=peak=true", "-f", "null", "-"],
                            capture_output=True, text=True, check=True)
    summary = result.stderr.rsplit("Summary:", 1)[-1]
    patterns = {"integrated_lufs": r"I:\s+(-?[\d.]+) LUFS",
                "loudness_range_lu": r"LRA:\s+(-?[\d.]+) LU",
                "true_peak_dbtp": r"Peak:\s+(-?[\d.]+) dBFS"}
    values = {}
    for key, pattern in patterns.items():
        match = re.search(pattern, summary)
        if not match:
            raise ValueError("FFmpeg summary missing " + key)
        values[key] = float(match.group(1))
    return {"available": True, **values}


def review_advisories(metrics: dict) -> list[str]:
    advisories = []
    if metrics["band_energy_fraction"]["sub_0_80"] > .30:
        advisories.append("Over 30% spectral energy lies below 80 Hz; check bass translation on small speakers.")
    if metrics["band_limited_retention_db"] < -5:
        advisories.append("180 Hz–6 kHz mono retains less than -5 dB of full-range body; check foreground audibility.")
    if metrics["band_energy_fraction"]["bright_4000_8000"] > .12:
        advisories.append("Elevated 4–8 kHz energy; listen for harsh brass, bow noise or cymbal fatigue.")
    if metrics["loop_edge_level_difference_db"] > 6:
        advisories.append("Loop edge windows differ by more than 6 dB; audition harmonic/energy continuity.")
    return advisories


def self_test() -> None:
    rate = 32000
    t = np.arange(rate * 60) / rate
    a = .10 * np.sin(2 * np.pi * 440 * t)
    a[20 * rate:30 * rate] *= .40
    a[-1] = a[0]
    stereo = np.vstack((a, a))
    healthy = measure(stereo, rate)
    assert engineering_failures(healthy) == []
    anti = np.vstack((a, -a))
    assert "More than 20 percent RMS body is lost in mono" in engineering_failures(measure(anti, rate))
    clicked = stereo.copy()
    clicked[:, 0] += .10
    assert "Loop endpoint discontinuity" in engineering_failures(measure(clicked, rate))
    flat = np.tile(stereo[:, :10 * rate], (1, 6))
    assert "Intimate bridge lacks the agreed 3 dB space before the return" in engineering_failures(measure(flat, rate))
    clipped = stereo * 8
    assert "Master needs at least 3 dB sample headroom" in engineering_failures(measure(clipped, rate))
    invalid = stereo.copy()
    invalid[0, 0] = np.nan
    try:
        measure(invalid, rate)
    except ValueError:
        pass
    else:
        raise AssertionError("Non-finite samples were accepted")
    print("Title audit mutation checks passed: phase loss, click, flat arrangement, excessive level, NaN.")


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--input", type=Path, default=MASTER)
    parser.add_argument("--report", type=Path, default=ROOT / "QA/title-quality-audit.json")
    parser.add_argument("--compare", type=Path)
    parser.add_argument("--enforce", action="store_true")
    parser.add_argument("--self-test", action="store_true")
    args = parser.parse_args()
    if args.self_test:
        self_test()
        return 0
    audio, rate = read_pcm(args.input)
    metrics = measure(audio, rate, planned_sections())
    r128 = loudness(args.input)
    errors = engineering_failures(metrics)
    if r128.get("true_peak_dbtp", -3) > -2:
        errors.append("True peak exceeds -2 dBTP")
    report = {"input": str(args.input.resolve()), "sha256": hashlib.sha256(args.input.read_bytes()).hexdigest(),
              "metrics": metrics, "r128": r128, "engineering_failures": errors,
              "listening_advisories": review_advisories(metrics),
              "scope": "Engineering checks are not evidence of an iconic melody, convincing instruments, or emotional impact."}
    if args.compare:
        previous = json.loads(args.compare.read_text(encoding="utf-8"))
        report["comparison"] = {
            "previous_sha256": previous["sha256"],
            "rms_change_db": metrics["rms_dbfs"] - previous["metrics"]["rms_dbfs"],
            "small_speaker_body_change_db": metrics["band_limited_mono_rms_dbfs"] - previous["metrics"]["band_limited_mono_rms_dbfs"],
            "sub_fraction_change": metrics["band_energy_fraction"]["sub_0_80"] - previous["metrics"]["band_energy_fraction"]["sub_0_80"],
        }
    args.report.parent.mkdir(parents=True, exist_ok=True)
    args.report.write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")
    print(json.dumps(report, indent=2))
    return 1 if args.enforce and errors else 0


if __name__ == "__main__":
    raise SystemExit(main())
