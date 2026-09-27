#!/usr/bin/env python3
"""Build the Living Folio score and its original tactile interaction bank.

Reuses the project's deterministic synthesis and mastering pipeline. No samples,
downloads, models, credentials or external services are used.
"""
from __future__ import annotations

import argparse
import csv
from dataclasses import asdict
import hashlib
import json
import math
from pathlib import Path
import re

import numpy as np

import BuildOriginalAudio as original

ROOT = original.STAGE_ROOT
SFX_DIR = original.SFX_DIR
QA = ROOT / "QA/party-setup-audio-v2.29"
RATE = original.SFX_SAMPLE_RATE
MUSIC_CUE = "muster_by_firelight_loop"
SPEC = next(spec for spec in original.TRACKS if spec.cue == MUSIC_CUE)
# Duration, physical/timbral identity, root pitch, authored cue description.
RECIPES = {
    "folio_companion": (.34, "wood", 220, "A leather folio settling with a warm wooden name-token tap."),
    "folio_race_human": (.48, "lute", 294, "Two warm hearth-lute notes."),
    "folio_race_duskelf": (.58, "glass", 523, "A hushed silver harmonic over a fine cloth whisper."),
    "folio_race_stoneborn": (.48, "stone", 110, "A low stone token and softly ringing chisel overtone."),
    "folio_race_fenkin": (.53, "reed", 392, "A wooden reed and rounded water-drop answer."),
    "folio_race_ashling": (.55, "fire", 330, "A contained ember flare and warm ringing cinder."),
    "folio_class_rogue": (.38, "steel", 659, "A restrained paired knife whisper and leather clasp."),
    "folio_class_warrior": (.48, "steel", 196, "A weighty buckle contact and damped sword ring."),
    "folio_class_ranger": (.48, "lute", 440, "A bowstring settling over two wooden plucks."),
    "folio_class_wizard": (.65, "glass", 494, "Three quiet arcane glass notes with a parchment brush."),
    "folio_class_mage": (.59, "fire", 587, "A small rising spark answered by a clear elemental tone."),
    "folio_class_warlock": (.64, "dark", 165, "A brief inward whisper and low violet minor harmony."),
    "folio_class_priest": (.68, "holy", 392, "A soft prayer bell and consonant upper fifth."),
    "folio_class_paladin": (.65, "holy", 262, "A ward-shield contact under a restrained oath-bell chord."),
    "folio_page_open": (.37, "paper", 440, "A page lifting, turning, and settling to the right."),
    "folio_page_close": (.33, "paper", 330, "A lower reverse page turn settling back to the portraits."),
    "folio_point_up": (.23, "token", 523, "A wooden attribute token with an upward two-note answer."),
    "folio_point_down": (.23, "token", 392, "A returned attribute token with a downward answer."),
    "folio_name": (.42, "quill", 349, "A quick quill inscription and two tiny wooden dice contacts."),
    "folio_begin": (.90, "seal", 262, "A firm parchment seal followed by the company's open-fifth departure motif."),
    "folio_heraldry": (.38, "cloth", 294, "A cloth ribbon and a small wax-sigil touch."),
    "folio_kit": (.45, "steel", 247, "Leather straps and two small fitted gear contacts."),
    "folio_train": (.36, "token", 659, "A training mark struck into the folio with a rising answer."),
}


def render(key: str) -> np.ndarray:
    seconds, family, frequency, _ = RECIPES[key]
    rng = np.random.default_rng(original.stable_seed(key + ":living-folio-v2.29"))
    count = round(seconds * RATE)
    signal = np.zeros(count)
    t = np.arange(count) / RATE

    def add(part: np.ndarray, at: float, gain: float) -> None:
        original.mix_linear(signal, part, round(at * RATE), gain)

    def note(hz: float, at: float, length: float, gain: float, kind: str = "pluck") -> None:
        fn = {"pluck": original.pluck, "bell": original.bell, "reed": original.reed}[kind]
        add(fn(hz, length, RATE, rng), at, gain)

    noise = original.lowpass(rng.normal(0, 1, count), 15)
    rustle = noise * (np.exp(-((t - .065) / .045) ** 2) + .55 * np.exp(-((t - .16) / .065) ** 2))
    signal += rustle * (.28 if family in {"paper", "cloth", "quill"} else .065)
    if family in {"paper", "cloth", "quill"}:
        note(frequency, .035, .18, .055)
        if family == "quill":
            add(original.click(.045, RATE, rng), .095, .08)
            add(original.click(.055, RATE, rng), .21, .055)
        if key == "folio_page_close":
            signal = signal[::-1].copy()
    elif family in {"wood", "token", "lute"}:
        note(frequency, .015, seconds * .8, .23)
        ratio = .8 if key == "folio_point_down" else 1.5 if family == "lute" else 1.25
        note(frequency * ratio, .10, max(.08, seconds - .11), .13)
        add(original.drum(135, .12, RATE, rng, .18), .005, .06)
    elif family == "stone":
        add(original.drum(75, .22, RATE, rng, .36), .01, .20)
        note(frequency * 2.72, .025, .35, .09, "bell")
    elif family == "steel":
        note(frequency, .012, seconds - .03, .19, "bell")
        note(frequency * 2.73, .052, seconds * .7, .055, "bell")
        add(original.drum(130 if frequency < 300 else 210, .12, RATE, rng, .35), .005, .055)
    elif family == "glass":
        for index, ratio in enumerate((1, 1.5, 1.25)):
            note(frequency * ratio, .02 + index * .105, seconds - .04 - index * .105, .12 - index * .017, "bell")
    elif family == "reed":
        note(frequency, .005, .31, .13, "reed")
        note(frequency * 2, .14, .33, .055, "bell")
    elif family == "fire":
        fire = rng.normal(0, 1, count)
        signal += (fire - original.lowpass(fire, 31)) * np.exp(-((t - .12) / .075) ** 2) * .035
        note(frequency, .03, seconds - .05, .17)
        note(frequency * 1.5, .14, seconds - .16, .08, "bell")
    elif family == "dark":
        swell = original.bell(frequency * 2, .23, RATE, rng)[::-1]
        add(swell, 0, .06)
        note(frequency, .10, .49, .19, "bell")
        note(frequency * 1.2, .17, .43, .085, "bell")
    elif family in {"holy", "seal"}:
        for index, ratio in enumerate((1, 1.5, 2)):
            note(frequency * ratio, .025 + index * .11, seconds - .05 - index * .11, .15 - index * .027, "bell")
        if family == "seal" or key == "folio_class_paladin":
            add(original.drum(112, .19, RATE, rng, .24), .005, .11)
    signal = original.master_audio(signal, -19.5 if family != "seal" else -18.5, -7.8)
    fade_in, fade_out = round(.006 * RATE), round(.055 * RATE)
    signal[:fade_in] *= np.linspace(0, 1, fade_in)
    signal[-fade_out:] *= np.linspace(1, 0, fade_out)
    signal[0] = signal[-1] = 0
    return signal


def update_music_manifest(audio: np.ndarray) -> None:
    path = original.MUSIC_DIR / (MUSIC_CUE + ".wav")
    row = asdict(original.metrics_for(MUSIC_CUE, SPEC.title, SPEC.direction, "music", path, audio, original.MUSIC_SAMPLE_RATE))
    manifest = original.DOCS_DIR / "ORIGINAL_AUDIO_ASSET_MANIFEST.tsv"
    with manifest.open(encoding="utf-8", newline="") as handle:
        reader = csv.DictReader(handle, delimiter="\t")
        fields, rows = reader.fieldnames, list(reader)
    assert sum(item["cue"] == MUSIC_CUE for item in rows) == 1
    with manifest.open("w", encoding="utf-8", newline="") as handle:
        writer = csv.DictWriter(handle, fieldnames=fields, delimiter="\t")
        writer.writeheader()
        writer.writerows(row if item["cue"] == MUSIC_CUE else item for item in rows)
    report_path = original.DOCS_DIR / "ORIGINAL_AUDIO_VALIDATION.json"
    report = json.loads(report_path.read_text(encoding="utf-8"))
    report["assets"] = [row if item["cue"] == MUSIC_CUE else item for item in report["assets"]]
    # The all-music sampler contains this cue; keep its recorded digest current.
    preview = original.build_preview(original.TRACKS)
    report["preview"] = preview.relative_to(ROOT).as_posix()
    report["preview_sha256"]["preview"] = original.sha256(preview)
    report_path.write_text(json.dumps(report, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")


def validate() -> dict:
    rules = (ROOT / "Assets/Scripts/Presentation/PartySetupAudioRules.cs").read_text(encoding="utf-8")
    declared = set(re.findall(r'"(folio_[a-z_]+)"', rules.split("public static TitleAudioCueProfile")[0]))
    assert declared == set(RECIPES), "runtime and generated cue banks differ"
    signatures, rows = set(), []
    for key, (_, _, _, direction) in RECIPES.items():
        path = SFX_DIR / (key + ".wav")
        audio, rate = original.read_pcm16(path)
        expected = np.rint(render(key) * 32767).astype(np.int16)
        assert rate == RATE and audio.ndim == 1
        assert np.array_equal(np.rint(audio * 32768).astype(np.int16), expected), key + " is not deterministic PCM"
        assert audio[0] == audio[-1] == 0, key + " has a boundary click"
        assert np.isfinite(audio).all() and np.max(np.abs(audio)) < .42 and np.sqrt(np.mean(audio * audio)) > .01
        digest = original.sha256(path)
        assert digest not in signatures, key + " duplicates another cue"
        signatures.add(digest)
        rows.append(asdict(original.metrics_for(key, key, direction, "sfx", path, audio, rate)))
    music_path = original.MUSIC_DIR / (MUSIC_CUE + ".wav")
    music, rate = original.read_pcm16(music_path)
    expected = np.rint(original.compose_track(SPEC) * 32767).astype(np.int16)
    assert rate == 32000 and music.shape[0] == 2
    assert np.array_equal(np.rint(music * 32768).astype(np.int16), expected), "music is not deterministic PCM"
    assert abs(music.shape[1] / rate - 53.3333) < .001
    assert np.max(np.abs(music[:, 0] - music[:, -1])) < .001, "loop seam jumps"
    assert np.max(np.abs(music)) < .60 and np.sqrt(np.mean(music * music)) > .06
    mono_body_ratio = float(np.sqrt(np.mean(np.mean(music, axis=0) ** 2)) / np.sqrt(np.mean(music ** 2)))
    assert mono_body_ratio > .80, "workshop score loses body on mono speakers"
    phrase_frames = round(8 * 60 / SPEC.bpm * rate)
    phrases = [music[:, i * phrase_frames:min((i + 1) * phrase_frames, music.shape[1])] for i in range(8)]
    phrase_rms = [float(np.sqrt(np.mean(phrase * phrase))) for phrase in phrases]
    assert phrase_rms[2] < phrase_rms[0] * .80, "third phrase needs audible breathing room"
    assert len({hashlib.sha256(phrase.tobytes()).hexdigest() for phrase in phrases}) == 8
    return {"release": "v2.29.0", "generator": "Tools/Audio/BuildPartySetupAudio.py", "deterministic": True,
            "external_material": False, "music": asdict(original.metrics_for(MUSIC_CUE, SPEC.title, SPEC.direction, "music", music_path, music, rate)),
            "phrase_rms_dbfs": [round(original.db(value), 2) for value in phrase_rms],
            "mono_body_ratio": round(mono_body_ratio, 5),
            "runtime_music_rms_dbfs": round(original.db(float(np.sqrt(np.mean(music * music))) * .23 * .65), 2),
            "sfx": rows, "listening_review": "Numerical and routing checks only; audition files provided for listening review."}


def write_preview() -> dict:
    music, rate = original.read_pcm16(original.MUSIC_DIR / (MUSIC_CUE + ".wav"))
    preview = original.loop_to_frames(music, 32 * rate) * .23 * .65
    playlist = []
    preview_keys = ("folio_companion", "folio_race_duskelf", "folio_class_ranger", "folio_page_open",
                    "folio_point_down", "folio_point_up", "folio_name", "folio_class_warlock", "folio_class_priest", "folio_begin")
    for i, key in enumerate(preview_keys):
        source, source_rate = original.read_pcm16(SFX_DIR / (key + ".wav"))
        # Match C# action gains, 100% SFX preference, and existing 0.78 source gain.
        gain = .34 if "_class_" in key else .30 if "_race_" in key else {
            "folio_companion": .28, "folio_page_open": .27, "folio_point_down": .24,
            "folio_point_up": .26, "folio_name": .28, "folio_begin": .40,
        }[key]
        pcm = np.interp(np.arange(round(source.size / source_rate * rate)) * source_rate / rate, np.arange(source.size), source)
        when = 1.6 + i * 2.7
        start = round(when * rate)
        length = min(pcm.size, preview.shape[1] - start)
        preview[:, start:start + length] += pcm[:length] * .78 * gain / math.sqrt(2)
        playlist.append({"cue": key, "seconds": round(when, 2)})
    original.enforce_runtime_preview_mix_contract("Living Folio", preview)
    QA.mkdir(parents=True, exist_ok=True)
    output = QA / "living-folio-runtime-preview.wav"
    original.write_pcm16(output, preview, rate)
    wrap = np.concatenate((music[:, -3 * rate:], music[:, :3 * rate]), axis=1)
    original.write_pcm16(QA / "living-folio-loop-boundary.wav", wrap, rate)
    return {"path": output.relative_to(ROOT).as_posix(), "sha256": original.sha256(output), "playlist": playlist,
            "peak_dbfs": round(original.db(float(np.max(np.abs(preview)))), 2),
            "rms_dbfs": round(original.db(float(np.sqrt(np.mean(preview * preview)))), 2)}


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--validate-only", action="store_true")
    args = parser.parse_args()
    if not args.validate_only:
        untouched = {path: original.sha256(path) for path in original.MUSIC_DIR.glob("*.wav") if path.stem != MUSIC_CUE}
        music = original.compose_track(SPEC)
        original.write_pcm16(original.MUSIC_DIR / (MUSIC_CUE + ".wav"), music, original.MUSIC_SAMPLE_RATE)
        template = (SFX_DIR / "itemequip.wav.meta").read_text(encoding="utf-8")
        for key in RECIPES:
            path = SFX_DIR / (key + ".wav")
            original.write_pcm16(path, render(key), RATE)
            meta = Path(str(path) + ".meta")
            if not meta.exists():
                guid = hashlib.md5(("ashen-living-folio/" + key).encode()).hexdigest()
                meta.write_text(re.sub(r"guid: [0-9a-f]+", "guid: " + guid, template), encoding="utf-8")
        saved, _ = original.read_pcm16(original.MUSIC_DIR / (MUSIC_CUE + ".wav"))
        update_music_manifest(saved)
        assert all(original.sha256(path) == digest for path, digest in untouched.items()), "unrelated score changed"
    report = validate()
    if not args.validate_only:
        report["preview"] = write_preview()
        (original.DOCS_DIR / "PARTY_SETUP_AUDIO_VALIDATION.json").write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")
        (QA / "validation.json").write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")
    print(json.dumps({key: value for key, value in report.items() if key not in {"sfx", "music"}}, indent=2))
    print("Validated one reauthored workshop score and 23 distinct interaction masters.")


if __name__ == "__main__":
    main()
