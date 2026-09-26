#!/usr/bin/env python3
"""Original deterministic everyday sound design, without recordings or services.

Build: python Tools/Audio/BuildEverydaySfx.py
Audit saved masters: python Tools/Audio/BuildEverydaySfx.py --validate-only
Every master is 48 kHz mono PCM, with stable seeds and preserved Unity GUIDs.
"""
from __future__ import annotations

import argparse
import csv
import hashlib
import json
import math
from pathlib import Path
import wave

import numpy as np

ROOT = Path(__file__).resolve().parents[2]
OUTPUT = ROOT / "Assets/Resources/Audio/Sfx"
MANIFEST = ROOT / "Docs/EVERYDAY_AUDIO_ASSET_MANIFEST.tsv"
QA = ROOT / "QA/everyday-audio"
RATE = 48_000
SURFACES = {
    # RMS tracks the existing surface masters: their runtime gains differ.
    "footstone": (0.28, 0.0504), "footearth": (0.30, 0.0405),
    "footwood": (0.29, 0.0964), "footwater": (0.30, 0.1459),
    "footglass": (0.24, 0.1655), "footmud": (0.29, 0.1651),
    "footash": (0.24, 0.1647), "footgravel": (0.25, 0.1651),
}
EVENTS = {
    "equipblade": (0.56, "A controlled blade draw, steel ring, and leather catch."),
    "equipbow": (0.48, "A wooden bow grip settling with a short muted string pluck."),
    "equipstaff": (0.66, "Wooden staff seating and a low arcane glass resonance."),
    "equipcloth": (0.44, "A layered cloth pull and dry leather fastening."),
    "equipmail": (0.55, "Several small interlocking chain rings settling into cloth."),
    "equipplate": (0.57, "A fitted metal plate and two damped buckle contacts."),
    "chestopen": (0.86, "Chest latch, creaking timber, and a hollow lid settling."),
    "lootrare": (0.96, "Pack clasp opening into a restrained three-note relic shimmer."),
    "zonediscover": (1.16, "A low compass bell and open-fifth road answer."),
    "questcomplete": (1.32, "A parchment seal and warm resolved three-note company motif."),
}
KEYS = tuple(EVENTS) + tuple(f"{key}__v{i}" for key in SURFACES for i in (1, 2))


def seed(key: str) -> int:
    return int.from_bytes(hashlib.sha256(key.encode()).digest()[:8], "little")


def tone(hz: float, seconds: float, decay: float = 8.0, metal: bool = False) -> np.ndarray:
    t = np.arange(round(seconds * RATE)) / RATE
    result = np.zeros(t.size)
    ratios = (1, 2.73, 4.13, 6.31) if metal else (1, 2, 3)
    for i, ratio in enumerate(ratios):
        result += np.sin(2 * np.pi * hz * ratio * t) * np.exp(-t * decay * (1 + i * 0.3)) / (1 + i * 2.1)
    return result * np.minimum(t / 0.002, 1)


def noise(rng: np.random.Generator, frames: int, width: int = 12) -> np.ndarray:
    raw = rng.normal(0, 1, frames)
    colored = np.convolve(raw, np.ones(width) / width, mode="same")
    return colored / max(float(np.std(colored)), 0.0001)


def add(output: np.ndarray, part: np.ndarray, at: float, gain: float) -> None:
    start = round(at * RATE)
    size = min(part.size, output.size - start)
    if size > 0:
        output[start:start + size] += part[:size] * gain


def render(key: str) -> np.ndarray:
    rng = np.random.default_rng(seed(key))
    base = key.split("__")[0]
    seconds = SURFACES[base][0] if base in SURFACES else EVENTS[key][0]
    frames = round(seconds * RATE)
    t = np.arange(frames) / RATE
    output = np.zeros(frames)
    if base in SURFACES:
        wet = base in ("footwater", "footmud")
        bright = base in ("footglass", "footgravel")
        wood = base == "footwood"
        hz = (100 if wet else 170 if wood else 125) * rng.uniform(0.89, 1.12)
        body = tone(hz, seconds, 32 if bright else 25)
        grit = noise(rng, frames, 4 if bright else 30 if wet else 14)
        grit *= np.exp(-t * (13 if wet else 24))
        output = body * 0.40 + grit * (0.18 if wood else 0.35)
        # Heel/toe contacts and granular settling vary in time as well as timbre.
        for i in range(4 if bright else 2):
            at = 0.035 + i * rng.uniform(0.025, 0.042)
            contact = tone(rng.uniform(850, 2100) if bright else hz * 1.63,
                           0.10, 48 if bright else 32, metal=bright)
            add(output, contact, at, 0.10 if bright else 0.14)
        if wet:
            splash = np.sin(2 * np.pi * (340 * t + 380 * t * t)) * np.exp(-t * 19)
            output += splash * 0.17
        target = SURFACES[base][1]
    else:
        cloth = noise(rng, frames, 22) * np.sin(np.pi * np.clip(t / seconds, 0, 1)) ** 2
        output += cloth * (0.11 if key == "equipcloth" else 0.025)
        if key == "equipblade":
            scrape = noise(rng, frames, 5) * np.exp(-((t - 0.14) / 0.065) ** 2)
            output += scrape * 0.085
            add(output, tone(740, 0.42, 13, True), 0.04, 0.15)
            add(output, tone(168, 0.18, 32), 0.31, 0.27)
        elif key == "equipbow":
            add(output, tone(185, 0.32, 15), 0.045, 0.30)
            add(output, tone(98, 0.19, 24), 0.24, 0.22)
        elif key == "equipstaff":
            add(output, tone(125, 0.28, 18), 0.02, 0.24)
            add(output, tone(392, 0.50, 7, True), 0.12, 0.12)
        elif key == "equipcloth":
            add(output, tone(178, 0.11, 42), 0.29, 0.16)
        elif key == "equipmail":
            for i in range(7):
                add(output, tone(rng.uniform(710, 1260), 0.18, 29, True),
                    0.025 + i * 0.053, 0.12 - i * 0.009)
        elif key == "equipplate":
            add(output, tone(254, 0.40, 12, True), 0.025, 0.27)
            add(output, tone(580, 0.19, 27, True), 0.24, 0.13)
            add(output, tone(412, 0.15, 34, True), 0.36, 0.10)
        elif key == "chestopen":
            add(output, tone(720, 0.14, 30, True), 0.018, 0.14)
            creak = np.sin(2 * np.pi * (118 * t - 32 * t * t) + 1.2 * np.sin(2 * np.pi * 27 * t))
            output += creak * np.exp(-((t - 0.30) / 0.15) ** 2) * 0.10
            add(output, tone(93, 0.28, 15), 0.55, 0.24)
        else:
            notes = {"lootrare": (293.66, 440, 587.33),
                     "zonediscover": (196, 293.66, 392),
                     "questcomplete": (293.66, 392, 587.33)}[key]
            spacing = 0.15 if key == "lootrare" else 0.22
            for i, hz in enumerate(notes):
                at = 0.055 + i * spacing
                add(output, tone(hz, seconds - at, 5.8, True), at, 0.13 - i * 0.01)
            add(output, tone(98, 0.22, 22), 0.02, 0.12)
        target = 0.12 if key.startswith("equip") else 0.105
    output -= np.mean(output)
    output *= np.minimum(t / 0.005, 1) * np.minimum((seconds - t) / 0.060, 1)
    output *= target / max(float(np.sqrt(np.mean(output * output))), 1e-10)
    if np.max(np.abs(output)) > 0.78:
        output = 0.78 * np.tanh(output / 0.78)
    output[0] = output[-1] = 0
    return output


def write_wav(path: Path, samples: np.ndarray) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    with wave.open(str(path), "wb") as handle:
        handle.setnchannels(1)
        handle.setsampwidth(2)
        handle.setframerate(RATE)
        handle.writeframes(np.rint(np.clip(samples, -1, 1) * 32767).astype("<i2").tobytes())


def metrics(path: Path) -> dict:
    with wave.open(str(path), "rb") as handle:
        assert (handle.getnchannels(), handle.getsampwidth(), handle.getframerate()) == (1, 2, RATE), path
        samples = np.frombuffer(handle.readframes(handle.getnframes()), dtype="<i2").astype(float) / 32768
    return {"cue": path.stem, "output": path.relative_to(ROOT).as_posix(),
            "kind": "sfx", "frames": samples.size, "sample_rate": RATE,
            "duration_seconds": round(samples.size / RATE, 4),
            "peak": round(float(np.max(np.abs(samples))), 6),
            "rms": round(float(np.sqrt(np.mean(samples * samples))), 6),
            "sha256": hashlib.sha256(path.read_bytes()).hexdigest(),
            "provenance": "Original deterministic synthesis; no external samples"}


def validate() -> list[dict]:
    with MANIFEST.open(encoding="utf-8", newline="") as handle:
        saved = {row["cue"]: row for row in csv.DictReader(handle, delimiter="\t")}
    assert set(saved) == set(KEYS), "Everyday manifest cue coverage changed"
    rows = []
    digests = set()
    for key in KEYS:
        path = OUTPUT / f"{key}.wav"
        info = metrics(path)
        assert info["sha256"] == saved[key]["sha256"], f"Stale manifest: {key}"
        assert info["sha256"] not in digests, f"Duplicate waveform: {key}"
        assert 0.005 < info["rms"] < 0.22 and info["peak"] < 0.80, f"Unsafe level: {key}"
        expected = np.rint(render(key) * 32767).astype("<i2")
        with wave.open(str(path), "rb") as handle:
            pcm = np.frombuffer(handle.readframes(handle.getnframes()), dtype="<i2")
        assert np.array_equal(pcm, expected), f"Non-deterministic master: {key}"
        assert pcm[0] == pcm[-1] == 0, f"Boundary click: {key}"
        base = key.split("__")[0]
        if base in SURFACES:
            base_rms = metrics(OUTPUT / f"{base}.wav")["rms"]
            assert abs(20 * math.log10(info["rms"] / base_rms)) < 2.5, f"Surface loudness mismatch: {key}"
        digests.add(info["sha256"])
        rows.append(info)
    return rows


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--validate-only", action="store_true")
    args = parser.parse_args()
    if not args.validate_only:
        template = (OUTPUT / "itemequip.wav.meta").read_text(encoding="utf-8")
        rows, playlist, segments = [], [], []
        elapsed = 0.0
        for key in KEYS:
            samples = render(key)
            path = OUTPUT / f"{key}.wav"
            write_wav(path, samples)
            meta = Path(str(path) + ".meta")
            if not meta.exists():
                lines = template.splitlines()
                lines[1] = "guid: " + hashlib.md5(("ashen-everyday-audio/" + key).encode()).hexdigest()
                meta.write_text("\n".join(lines) + "\n", encoding="utf-8")
            info = metrics(path)
            rows.append(info)
            # Listen at representative runtime SFX gain; steps use their material gain.
            base = key.split("__")[0]
            gain = {"footstone": .82, "footearth": .84, "footwood": .54,
                    "footwater": .36, "footglass": .30, "footmud": .32,
                    "footash": .30, "footgravel": .34}.get(base, .55)
            playlist.append({"cue": key, "start_seconds": round(elapsed, 3)})
            segments.extend([samples * .78 * gain, np.zeros(round(RATE * .28))])
            elapsed += samples.size / RATE + .28
        with MANIFEST.open("w", encoding="utf-8", newline="") as handle:
            writer = csv.DictWriter(handle, fieldnames=list(rows[0]), delimiter="\t")
            writer.writeheader()
            writer.writerows(rows)
        QA.mkdir(parents=True, exist_ok=True)
        write_wav(QA / "everyday-sfx-preview.wav", np.concatenate(segments))
        (QA / "preview-playlist.json").write_text(json.dumps(playlist, indent=2) + "\n", encoding="utf-8")
    rows = validate()
    print(f"Validated {len(rows)} distinct original everyday masters: deterministic PCM, boundary fades, headroom, surface balance.")


if __name__ == "__main__":
    main()
