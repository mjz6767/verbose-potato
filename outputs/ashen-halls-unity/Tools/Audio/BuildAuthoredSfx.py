#!/usr/bin/env python3
"""Build the small authored SFX override bank used by Ash & Brimstone.

The source packs are intentionally kept outside the Unity Assets tree. This
script decodes only the selected CC0 files, folds them to mono, resamples them
to 48 kHz, trims silence, layers a few composite cues, applies short fades and
soft limiting, then writes exact runtime cue names into Resources.
"""

from __future__ import annotations

import argparse
import csv
import math
import shutil
import subprocess
import sys
import wave
from dataclasses import dataclass
from pathlib import Path

import numpy as np


SAMPLE_RATE = 48_000


@dataclass(frozen=True)
class Layer:
    source: str
    gain: float = 1.0
    offset_ms: float = 0.0
    pitch_semitones: float = 0.0
    start_ms: float = 0.0
    max_ms: float = 0.0
    highpass_hz: float = 45.0


@dataclass(frozen=True)
class Cue:
    layers: tuple[Layer, ...]
    max_ms: float
    peak: float
    fade_in_ms: float = 2.5
    fade_out_ms: float = 35.0
    drive: float = 0.75


SPELL = "lentikula-basic-spell-impacts"
KENNEY_UI = "kenney-interface-sounds/Audio"
KENNEY_IMPACT = "kenney-impact-sounds/Audio"
KENNEY_RPG = "kenney-rpg-audio/Audio"
OGA_BATTLE = "opengameart-battle-sounds/battle_sound_effects"
OGA_SWORD = "opengameart-starninjas-sword-attacks/sword - StarNinjas"
OGA_CLASH = "opengameart-starninjas-sword-clashes"


def layer(source: str, **kwargs: float) -> Layer:
    return Layer(source=source, **kwargs)


CUES: dict[str, Cue] = {
    # Restrained, tactile interface and notification family.
    "ui": Cue((layer(f"{KENNEY_UI}/select_004.ogg"),), 280, 0.66, 1.0, 24.0, 0.45),
    "blocked": Cue((layer(f"{KENNEY_UI}/error_003.ogg"),), 420, 0.72, 1.0, 32.0, 0.55),
    "save": Cue((layer(f"{KENNEY_UI}/confirmation_002.ogg"),), 600, 0.68, 1.0, 55.0, 0.40),
    "turn": Cue((layer(f"{KENNEY_UI}/pluck_002.ogg"),), 520, 0.68, 1.0, 45.0, 0.42),
    "cache": Cue((layer(f"{KENNEY_UI}/confirmation_004.ogg"),), 650, 0.72, 1.0, 55.0, 0.42),
    "formula": Cue((layer(f"{KENNEY_UI}/glass_002.ogg"),), 620, 0.66, 1.0, 55.0, 0.38),

    # Authored elemental impacts. Cast/release cues remain procedural so the
    # two stages stay readable when combat gets busy.
    "fireball": Cue((layer(f"{SPELL}/Fire Spell Impacts/Fire Spell Impact 2.wav", gain=0.96),), 1650, 0.86, 2.0, 90.0, 0.74),
    "meteor": Cue((layer(f"{SPELL}/Fire Spell Impacts/Fire Spell Impact 5.wav"),), 1800, 0.88, 2.0, 110.0, 0.86),
    "fieldfire": Cue((layer(f"{SPELL}/Fire Spell Impacts/Fire Spell Impact 1.wav", gain=0.88, pitch_semitones=1.1),), 1350, 0.78, 3.0, 100.0, 0.66),
    "shock": Cue((layer(f"{SPELL}/Lightning Spell Impacts/Lightning Spell Impact 1.wav"),), 1450, 0.84, 1.0, 75.0, 0.74),
    "tempest": Cue((layer(f"{SPELL}/Lightning Spell Impacts/Lightning Spell Impact 5.wav", gain=1.02, pitch_semitones=-0.7),), 1800, 0.88, 1.0, 100.0, 0.84),
    "ice": Cue((layer(f"{SPELL}/Ice Spell Impacts/Ice Spell Impact 2.wav"),), 1550, 0.82, 1.0, 95.0, 0.66),
    "fieldice": Cue((layer(f"{SPELL}/Ice Spell Impacts/Ice Spell Impact 4.wav", gain=0.90, pitch_semitones=0.8),), 1300, 0.76, 2.0, 90.0, 0.60),
    "tree": Cue((layer(f"{SPELL}/Water Spell Impacts/Water Spell Impact 2.wav", gain=0.92, pitch_semitones=-1.2, highpass_hz=65.0),), 1500, 0.80, 3.0, 110.0, 0.62),

    # Releases, misses, blocks and material contacts.
    "attack": Cue((layer(f"{OGA_BATTLE}/swish_2.wav"),), 420, 0.78, 1.0, 30.0, 0.72),
    "blade": Cue((layer(f"{OGA_BATTLE}/swish_3.wav", pitch_semitones=0.5),), 430, 0.79, 1.0, 30.0, 0.74),
    "swing": Cue((layer(f"{OGA_BATTLE}/swish_4.wav"),), 470, 0.78, 1.0, 34.0, 0.72),
    "swingheavy": Cue((layer(f"{OGA_SWORD}/sword.5.ogg", pitch_semitones=-1.6),), 760, 0.84, 1.0, 48.0, 0.82),
    "miss": Cue((layer(f"{OGA_BATTLE}/swish_2.wav", pitch_semitones=2.2),), 330, 0.68, 1.0, 28.0, 0.58),
    "guard": Cue((layer(f"{OGA_CLASH}/sword_clash.1.ogg"),), 720, 0.82, 1.0, 65.0, 0.78),
    "counter": Cue((
        layer(f"{OGA_SWORD}/sword.1.ogg", gain=0.82, pitch_semitones=0.7),
        layer(f"{OGA_CLASH}/sword_clash.3.ogg", gain=0.55, offset_ms=118.0),
    ), 940, 0.85, 1.0, 70.0, 0.82),
    "hit": Cue((layer(f"{KENNEY_IMPACT}/impactSoft_medium_002.ogg"),), 260, 0.78, 1.0, 28.0, 0.92),
    "bladecontact": Cue((
        layer(f"{OGA_CLASH}/sword_clash.2.ogg", gain=0.90),
        layer(f"{KENNEY_IMPACT}/impactSoft_medium_001.ogg", gain=0.45, offset_ms=12.0),
    ), 780, 0.84, 1.0, 62.0, 0.82),
    "thrustcontact": Cue((layer(f"{OGA_CLASH}/sword_clash.4.ogg", pitch_semitones=0.8),), 680, 0.82, 1.0, 58.0, 0.78),
    "heavycontact": Cue((
        layer(f"{KENNEY_IMPACT}/impactMetal_heavy_003.ogg", gain=0.92),
        layer(f"{KENNEY_IMPACT}/impactSoft_heavy_002.ogg", gain=0.62, offset_ms=8.0),
    ), 620, 0.87, 1.0, 58.0, 0.96),
    "impactflesh": Cue((layer(f"{KENNEY_IMPACT}/impactSoft_medium_004.ogg"),), 280, 0.76, 1.0, 30.0, 0.88),
    "impactleather": Cue((layer(f"{KENNEY_IMPACT}/impactSoft_heavy_001.ogg", pitch_semitones=0.7),), 340, 0.78, 1.0, 34.0, 0.86),
    "impactmail": Cue((layer(f"{KENNEY_IMPACT}/impactMetal_medium_001.ogg"),), 520, 0.82, 1.0, 48.0, 0.82),
    "impactplate": Cue((layer(f"{KENNEY_IMPACT}/impactPlate_heavy_001.ogg"),), 610, 0.85, 1.0, 58.0, 0.88),
    "impactshield": Cue((
        layer(f"{OGA_CLASH}/sword_clash.8.ogg", gain=0.82),
        layer(f"{KENNEY_IMPACT}/impactPlate_medium_003.ogg", gain=0.62, offset_ms=9.0),
    ), 790, 0.86, 1.0, 66.0, 0.88),

    # Martial skill releases and their separately staged contacts.
    "charge": Cue((
        layer(f"{OGA_BATTLE}/swish_4.wav", gain=0.85, pitch_semitones=-0.8),
        layer(f"{KENNEY_IMPACT}/footstep_concrete_003.ogg", gain=0.52, offset_ms=86.0),
    ), 650, 0.82, 1.0, 45.0, 0.78),
    "whirlwind": Cue((
        layer(f"{OGA_BATTLE}/swish_2.wav", gain=0.74, pitch_semitones=-1.2),
        layer(f"{OGA_BATTLE}/swish_3.wav", gain=0.67, offset_ms=82.0, pitch_semitones=0.4),
        layer(f"{OGA_BATTLE}/swish_4.wav", gain=0.58, offset_ms=164.0, pitch_semitones=1.3),
    ), 760, 0.84, 1.0, 48.0, 0.82),
    "execute": Cue((layer(f"{OGA_SWORD}/sword.9.ogg", pitch_semitones=-1.4),), 820, 0.86, 1.0, 58.0, 0.88),
    "ambush": Cue((layer(f"{KENNEY_RPG}/knifeSlice2.ogg", pitch_semitones=1.0),), 620, 0.78, 1.0, 42.0, 0.76),
    "eviscerate": Cue((layer(f"{OGA_SWORD}/sword.2.ogg", pitch_semitones=0.4),), 760, 0.84, 1.0, 52.0, 0.84),
    "chargeimpact": Cue((
        layer(f"{KENNEY_IMPACT}/impactMetal_heavy_004.ogg", gain=0.88),
        layer(f"{KENNEY_IMPACT}/impactPunch_heavy_002.ogg", gain=0.66, offset_ms=6.0),
    ), 590, 0.88, 1.0, 58.0, 0.98),
    "whirlwindimpact": Cue((layer(f"{OGA_CLASH}/sword_clash.7.ogg", pitch_semitones=0.5),), 760, 0.85, 1.0, 62.0, 0.86),
    "executeimpact": Cue((
        layer(f"{OGA_CLASH}/sword_clash.10.ogg", gain=0.88, pitch_semitones=-0.7),
        layer(f"{KENNEY_IMPACT}/impactSoft_heavy_004.ogg", gain=0.65, offset_ms=10.0),
    ), 870, 0.89, 1.0, 72.0, 1.02),
    "ambushimpact": Cue((layer(f"{KENNEY_IMPACT}/impactSoft_medium_003.ogg", pitch_semitones=0.8),), 300, 0.80, 1.0, 32.0, 0.90),
    "eviscerateimpact": Cue((
        layer(f"{OGA_CLASH}/sword_clash.5.ogg", gain=0.78, pitch_semitones=0.7),
        layer(f"{KENNEY_IMPACT}/impactSoft_medium_000.ogg", gain=0.56, offset_ms=7.0),
    ), 720, 0.86, 1.0, 62.0, 0.92),

    # One real bow recording becomes distinct single-shot and multi-shot skills.
    "bow": Cue((layer(f"{OGA_BATTLE}/Bow.wav", pitch_semitones=-0.4),), 620, 0.77, 1.0, 52.0, 0.68),
    "aimedshot": Cue((layer(f"{OGA_BATTLE}/Bow.wav", gain=1.02, pitch_semitones=-1.1),), 680, 0.82, 1.0, 58.0, 0.74),
    "pinning": Cue((layer(f"{OGA_BATTLE}/Bow.wav", pitch_semitones=1.0),), 570, 0.80, 1.0, 48.0, 0.72),
    "volley": Cue((
        layer(f"{OGA_BATTLE}/Bow.wav", gain=0.78, pitch_semitones=-0.8),
        layer(f"{OGA_BATTLE}/Bow.wav", gain=0.65, offset_ms=92.0, pitch_semitones=0.5),
        layer(f"{OGA_BATTLE}/Bow.wav", gain=0.52, offset_ms=176.0, pitch_semitones=1.4),
    ), 980, 0.84, 1.0, 70.0, 0.78),
    "arrowrain": Cue((
        layer(f"{OGA_BATTLE}/Bow.wav", gain=0.72, pitch_semitones=1.5),
        layer(f"{OGA_BATTLE}/Bow.wav", gain=0.62, offset_ms=72.0, pitch_semitones=-0.3),
        layer(f"{OGA_BATTLE}/Bow.wav", gain=0.52, offset_ms=146.0, pitch_semitones=0.7),
        layer(f"{OGA_BATTLE}/Bow.wav", gain=0.44, offset_ms=225.0, pitch_semitones=-1.2),
    ), 1080, 0.85, 1.0, 78.0, 0.80),
    "arrowcontact": Cue((
        layer(f"{KENNEY_IMPACT}/impactWood_light_001.ogg", gain=0.80),
        layer(f"{KENNEY_IMPACT}/impactSoft_medium_001.ogg", gain=0.38, offset_ms=5.0),
    ), 330, 0.79, 1.0, 36.0, 0.86),

    # General exploration feedback replaces the most synthetic everyday cues.
    "footstone": Cue((layer(f"{KENNEY_IMPACT}/footstep_concrete_002.ogg"),), 360, 0.62, 1.0, 30.0, 0.56),
    "footearth": Cue((layer(f"{KENNEY_IMPACT}/footstep_grass_002.ogg"),), 390, 0.60, 1.0, 32.0, 0.54),
    "footwood": Cue((layer(f"{KENNEY_IMPACT}/footstep_wood_002.ogg"),), 400, 0.62, 1.0, 34.0, 0.58),
    "door": Cue((layer(f"{KENNEY_RPG}/doorOpen_1.ogg"),), 1050, 0.70, 2.0, 80.0, 0.64),
    "doorwood": Cue((layer(f"{KENNEY_RPG}/doorOpen_2.ogg", pitch_semitones=-0.5),), 1120, 0.72, 2.0, 90.0, 0.66),
    "gateopen": Cue((
        layer(f"{KENNEY_RPG}/creak3.ogg", gain=0.84, pitch_semitones=-1.1),
        layer(f"{KENNEY_RPG}/doorOpen_2.ogg", gain=0.48, offset_ms=190.0, pitch_semitones=-1.6),
    ), 1500, 0.78, 3.0, 120.0, 0.72),
    "gatebarred": Cue((layer(f"{KENNEY_IMPACT}/impactWood_heavy_003.ogg", pitch_semitones=-0.8),), 680, 0.80, 1.0, 58.0, 0.82),
    "dialoguepage": Cue((layer(f"{KENNEY_RPG}/bookFlip2.ogg"),), 620, 0.58, 1.0, 42.0, 0.46),
    "servicecoin": Cue((layer(f"{KENNEY_RPG}/handleCoins2.ogg"),), 820, 0.65, 1.0, 58.0, 0.52),
}


def find_ffmpeg() -> str:
    candidate = shutil.which("ffmpeg")
    if candidate:
        return candidate
    try:
        import imageio_ffmpeg

        return imageio_ffmpeg.get_ffmpeg_exe()
    except Exception as exc:  # pragma: no cover - environment-dependent fallback
        raise RuntimeError("FFmpeg is required but was not found.") from exc


def decode_audio(ffmpeg: str, path: Path) -> np.ndarray:
    command = [
        ffmpeg,
        "-v",
        "error",
        "-i",
        str(path),
        "-f",
        "f32le",
        "-acodec",
        "pcm_f32le",
        "-ac",
        "1",
        "-ar",
        str(SAMPLE_RATE),
        "pipe:1",
    ]
    result = subprocess.run(command, check=True, stdout=subprocess.PIPE)
    samples = np.frombuffer(result.stdout, dtype="<f4").astype(np.float32, copy=True)
    if samples.size == 0:
        raise RuntimeError(f"Decoded source is empty: {path}")
    return samples


def trim_silence(samples: np.ndarray, threshold: float = 0.00045) -> np.ndarray:
    active = np.flatnonzero(np.abs(samples) >= threshold)
    if active.size == 0:
        return samples[:1].copy()
    pre = int(SAMPLE_RATE * 0.006)
    post = int(SAMPLE_RATE * 0.075)
    start = max(0, int(active[0]) - pre)
    end = min(samples.size, int(active[-1]) + post + 1)
    return samples[start:end].copy()


def pitch_shift_by_speed(samples: np.ndarray, semitones: float) -> np.ndarray:
    if abs(semitones) < 0.001 or samples.size < 2:
        return samples
    ratio = 2.0 ** (semitones / 12.0)
    output_length = max(1, int(round(samples.size / ratio)))
    source_positions = np.arange(samples.size, dtype=np.float64)
    output_positions = np.linspace(0.0, samples.size - 1.0, output_length)
    return np.interp(output_positions, source_positions, samples).astype(np.float32)


def highpass(samples: np.ndarray, cutoff_hz: float) -> np.ndarray:
    if cutoff_hz <= 0.0 or samples.size < 2:
        return samples
    rc = 1.0 / (2.0 * math.pi * cutoff_hz)
    dt = 1.0 / SAMPLE_RATE
    alpha = rc / (rc + dt)
    output = np.empty_like(samples)
    output[0] = samples[0]
    previous_output = float(output[0])
    previous_input = float(samples[0])
    for index in range(1, samples.size):
        current_input = float(samples[index])
        previous_output = alpha * (previous_output + current_input - previous_input)
        output[index] = previous_output
        previous_input = current_input
    return output


def apply_fades(samples: np.ndarray, fade_in_ms: float, fade_out_ms: float) -> None:
    fade_in = min(samples.size, int(SAMPLE_RATE * fade_in_ms / 1000.0))
    fade_out = min(samples.size, int(SAMPLE_RATE * fade_out_ms / 1000.0))
    if fade_in > 1:
        samples[:fade_in] *= np.linspace(0.0, 1.0, fade_in, dtype=np.float32)
    if fade_out > 1:
        samples[-fade_out:] *= np.linspace(1.0, 0.0, fade_out, dtype=np.float32)


def render_cue(
    ffmpeg: str,
    source_root: Path,
    cue: Cue,
    decoded_cache: dict[Path, np.ndarray],
) -> np.ndarray:
    prepared: list[tuple[np.ndarray, int, float]] = []
    final_length = 1
    for item in cue.layers:
        source_path = (source_root / item.source).resolve()
        if not source_path.is_file():
            raise FileNotFoundError(f"Missing authored SFX source: {source_path}")
        if source_path not in decoded_cache:
            decoded_cache[source_path] = decode_audio(ffmpeg, source_path)
        samples = decoded_cache[source_path].copy()
        start = max(0, int(SAMPLE_RATE * item.start_ms / 1000.0))
        if start:
            samples = samples[start:]
        if item.max_ms > 0:
            samples = samples[: max(1, int(SAMPLE_RATE * item.max_ms / 1000.0))]
        samples = trim_silence(samples)
        samples = pitch_shift_by_speed(samples, item.pitch_semitones)
        samples = highpass(samples, item.highpass_hz)
        offset = max(0, int(SAMPLE_RATE * item.offset_ms / 1000.0))
        prepared.append((samples, offset, item.gain))
        final_length = max(final_length, offset + samples.size)

    maximum_length = max(1, int(SAMPLE_RATE * cue.max_ms / 1000.0))
    mix = np.zeros(min(final_length, maximum_length), dtype=np.float32)
    for samples, offset, gain in prepared:
        if offset >= mix.size:
            continue
        count = min(samples.size, mix.size - offset)
        mix[offset : offset + count] += samples[:count] * gain

    mix = trim_silence(mix)
    mix -= float(np.mean(mix))
    apply_fades(mix, cue.fade_in_ms, cue.fade_out_ms)

    peak = float(np.max(np.abs(mix))) if mix.size else 0.0
    if peak <= 1.0e-8:
        raise RuntimeError("Rendered cue is silent.")
    mix *= (cue.peak * 1.10) / peak
    if cue.drive > 0.0:
        mix = np.tanh(mix * cue.drive).astype(np.float32)
    peak = float(np.max(np.abs(mix)))
    mix *= cue.peak / max(peak, 1.0e-8)
    return mix


def write_pcm16(path: Path, samples: np.ndarray) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    pcm = np.clip(np.rint(samples * 32767.0), -32767.0, 32767.0).astype("<i2")
    with wave.open(str(path), "wb") as output:
        output.setnchannels(1)
        output.setsampwidth(2)
        output.setframerate(SAMPLE_RATE)
        output.writeframes(pcm.tobytes())


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser()
    parser.add_argument(
        "--source-root",
        type=Path,
        required=True,
        help="Directory containing the extracted source-pack folders.",
    )
    parser.add_argument(
        "--output",
        type=Path,
        default=Path("Assets/Resources/Audio/Sfx"),
        help="Unity Resources output directory.",
    )
    return parser.parse_args()


def main() -> int:
    args = parse_args()
    source_root = args.source_root.resolve()
    output_root = args.output.resolve()
    if not source_root.is_dir():
        raise FileNotFoundError(f"Source root does not exist: {source_root}")

    ffmpeg = find_ffmpeg()
    decoded_cache: dict[Path, np.ndarray] = {}
    output_root.mkdir(parents=True, exist_ok=True)
    expected_outputs: set[Path] = set()

    print(f"FFmpeg: {ffmpeg}")
    print(f"Source: {source_root}")
    print(f"Output: {output_root}")
    for cue_name in sorted(CUES):
        samples = render_cue(ffmpeg, source_root, CUES[cue_name], decoded_cache)
        output_path = output_root / f"{cue_name}.wav"
        write_pcm16(output_path, samples)
        expected_outputs.add(output_path.resolve())
        peak = float(np.max(np.abs(samples)))
        rms = float(np.sqrt(np.mean(np.square(samples))))
        print(f"{cue_name:20s} {samples.size / SAMPLE_RATE:6.3f}s  peak={peak:0.3f}  rms={rms:0.4f}")

    original_cues: set[str] = set()
    original_manifest = output_root.parents[3] / "Docs" / "ORIGINAL_AUDIO_ASSET_MANIFEST.tsv"
    if original_manifest.is_file():
        with original_manifest.open("r", encoding="utf-8", newline="") as handle:
            for row in csv.DictReader(handle, delimiter="\t"):
                if (row.get("kind") or "").strip().lower() != "sfx":
                    continue
                cue_name = (row.get("cue") or "").strip().lower()
                if cue_name:
                    original_cues.add(cue_name)

    unexpected = [
        path
        for path in output_root.glob("*.wav")
        if path.resolve() not in expected_outputs and path.stem.lower() not in original_cues
    ]
    if unexpected:
        names = ", ".join(path.name for path in unexpected)
        raise RuntimeError(f"Unexpected authored SFX outputs remain: {names}")

    print(f"Built {len(CUES)} authored SFX overrides.")
    return 0


if __name__ == "__main__":
    try:
        raise SystemExit(main())
    except Exception as error:
        print(f"Audio build failed: {error}", file=sys.stderr)
        raise
