#!/usr/bin/env python3
"""Original, deterministic skill contacts and school release textures.

Only these twenty-one masters are written. No recordings, services, music edits,
or changes to existing cast/impact assets. Run --validate-only for a read-only
PCM/hash/source reproduction audit. Comparison previews are isolated cues at
representative gain, not a claim to be a live gameplay recording.
"""
from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path
import wave

import numpy as np

ROOT = Path(__file__).resolve().parents[2]
OUTPUT = ROOT / "Assets/Resources/Audio/Sfx"
QA = ROOT / "QA/skill-spell-audio"
MANIFEST = ROOT / "Docs/SKILL_SPELL_AUDIO_VALIDATION.json"
RATE = 48_000
SPECS = {
    "releaseember": (.21, .058, "Dry ember ignition and turbulent flame; no tonal laser sweep."),
    "releasefrost": (.23, .052, "Tiny ice fractures and sparse inharmonic glass."),
    "releaseshock": (.16, .058, "Three irregular electrical arcs with a controlled short buzz."),
    "releasenature": (.24, .060, "Twisting wood fibers, leaf breath and an earthy resonance."),
    "releaseholy": (.25, .052, "An airy open-fifth light gesture with a soft bronze edge."),
    "releasehex": (.25, .056, "Inward whisper and descending, hollow shadow body."),
    "releasepact": (.27, .060, "Low rift pressure with a widening dissonant harmonic."),
    "releaseveil": (.20, .050, "A reversed air fold with a dry spatial re-entry tick."),
    "shieldbashhit": (.42, .130, "Shield wood and leather body under two damped iron modes."),
    "cleavehit": (.38, .130, "A broad steel cut settling into a dense body contact."),
    "sunderhit": (.49, .135, "A stressed plate strike followed by small armor fragments."),
    "hamstringhit": (.27, .105, "A close, dry slicing contact without a bow or magical tail."),
    "kniferelease": (.18, .055, "A narrow air cut and a light rotating metal whisper."),
    "smokeprimer": (.13, .035, "A small flask pin and cloth-handling tick before the burst."),
    "smokeburst": (.50, .105, "One pressure puff expanding into a low, soft smoke cloud."),
    "bowdraw": (.18, .037, "Restrained wooden limb tension, distinct from a fired bow."),
    "twinshotrelease": (.27, .060, "Two string releases, 85 ms apart, matching Twinshot's two arrows."),
    "twinshothit": (.35, .115, "Two compact wood-and-body arrow contacts, 85 ms apart."),
    "broadheadhit": (.33, .125, "A heavier arrowhead bite and short fibrous body tail."),
    "staggerhit": (.40, .125, "A blunt arrow strike with a restrained metallic interruption."),
    "shieldbrace": (.20, .037, "A quiet leather grip and wooden shield brace before contact."),
}


def rng_for(key: str) -> np.random.Generator:
    return np.random.default_rng(int.from_bytes(hashlib.sha256(key.encode()).digest()[:8], "little"))


def tone(t: np.ndarray, hz: float, decay: float, ratios=(1., 2., 3.)) -> np.ndarray:
    return sum(np.sin(2 * np.pi * hz * ratio * t) * np.exp(-t * decay * (1 + i * .3))
               / (1 + i * 2.2) for i, ratio in enumerate(ratios))


def noise(rng, size, width):
    samples = np.convolve(rng.normal(0, 1, size), np.ones(width) / width, mode="same")
    return samples / max(float(np.std(samples)), .00001)


def add(out, part, seconds, gain=1.):
    start = round(seconds * RATE)
    size = min(part.size, out.size - start)
    if size > 0:
        out[start:start + size] += part[:size] * gain


def render(key: str) -> np.ndarray:
    seconds, target, _ = SPECS[key]
    rng = rng_for(key)
    frames = round(seconds * RATE)
    t = np.arange(frames) / RATE
    p = t / seconds
    air = noise(rng, frames, 3)
    grit = noise(rng, frames, 22)
    soft = noise(rng, frames, 120)
    attack = 1 - np.exp(-t * 550)
    out = np.zeros(frames)
    if key == "releaseember":
        out = (grit * .54 + soft * .20) * np.exp(-t * 16) * attack
        out += tone(t, 180, 22) * .14
    elif key == "releasefrost":
        for start, hz, gain in ((0, 1880, .40), (.035, 2710, .30), (.073, 1330, .20)):
            add(out, tone(t, hz, 34, (1., 1.47, 2.73)) + air * np.exp(-t * 110) * .16, start, gain)
    elif key == "releaseshock":
        for start, gain in ((0, 1.), (.034, .67), (.079, .42)):
            arc = (air * .5 + np.sin(2 * np.pi * (810 * t + 560 * t * t)) * .15) * np.exp(-t * 90)
            add(out, arc, start, gain)
    elif key == "releasenature":
        out = (grit * .25 + tone(t, 137, 18, (1., 2.17, 4.23)) * .40) * attack
        for start in (.025, .067, .115):
            add(out, grit * np.exp(-t * 105), start, .13)
    elif key == "releaseholy":
        out = (tone(t, 587.33, 15, (1., 1.5, 3.)) * .32 + air * np.exp(-t * 16) * .045) * attack
    elif key == "releasehex":
        out = (grit * .36 + np.sin(2 * np.pi * (215 * t - 170 * t * t)) * .23) * np.sin(np.pi * p) ** 1.3
    elif key == "releasepact":
        out = (tone(t, 94, 8, (1., 1.49, 2.13)) * .28 + soft * .38) * np.sin(np.pi * p) ** .8
    elif key == "releaseveil":
        out = grit * np.sin(np.pi * p) ** 2 * .32
        add(out, tone(t, 710, 80) * .08, .11)
    elif key in ("shieldbashhit", "sunderhit", "staggerhit"):
        hz = {"shieldbashhit": 142, "sunderhit": 208, "staggerhit": 178}[key]
        out = tone(t, hz, 21) * .40 + soft * np.exp(-t * 31) * .24
        out += tone(t, hz * 4.7, 16, (1., 1.37, 2.71)) * (.26 if key == "sunderhit" else .12)
        out += air * np.exp(-t * 120) * .13
        if key == "sunderhit":
            for start, ring in ((.082, 1870), (.144, 2470), (.211, 1560)):
                add(out, tone(t, ring, 44, (1., 2.71)), start, .065)
    elif key in ("cleavehit", "hamstringhit", "broadheadhit"):
        hz = {"cleavehit": 134, "hamstringhit": 192, "broadheadhit": 116}[key]
        out = tone(t, hz, 28) * .4 + grit * np.exp(-t * 40) * .32
        out += air * np.exp(-t * 83) * (.20 if key == "hamstringhit" else .09)
        if key == "cleavehit":
            out += tone(t, 960, 25, (1., 1.42, 2.6)) * .10
    elif key == "kniferelease":
        out = air * np.sin(np.pi * p) ** 2 * np.exp(-t * 16) * .35
        out += tone(t, 1910, 34, (1., 1.47)) * .035
    elif key == "smokeprimer":
        out = tone(t, 1420, 100, (1., 2.73)) * .11 + grit * np.exp(-t * 43) * .2
    elif key == "smokeburst":
        out = (soft * .44 + grit * .12) * attack * np.exp(-t * 11)
        out += tone(t, 106, 24) * .17
    elif key in ("bowdraw", "shieldbrace"):
        out = (tone(t, 196, 8, (1., 2.31)) * .16 + grit * .13) * np.sin(np.pi * p) ** 2
        if key == "shieldbrace":
            out = (tone(t, 146, 23) * .23 + grit * np.exp(-t * 16) * .25) * attack
    elif key in ("twinshotrelease", "twinshothit"):
        if key == "twinshotrelease":
            gesture = tone(t, 340, 65, (1., 2.03, 3.91)) * .31 + air * np.exp(-t * 65) * .2
        else:
            gesture = tone(t, 173, 50, (1., 2.26)) * .39 + grit * np.exp(-t * 72) * .24
        gesture *= np.minimum(t / .0015, 1)
        out = gesture.copy()
        add(out, gesture, .085, .86)
    else:
        raise ValueError(key)
    if key.endswith("hit"):
        # Controlled saturation increases contact body instead of allowing a
        # single tiny noise spike to dictate a whisper-quiet overall master.
        out = np.tanh(out * 2.5)
    out -= np.mean(out)
    # Bound both edges after removing DC; use a real tail fade, not hard clipping.
    out *= np.minimum(t / .003, 1) * np.minimum((seconds - t - 1 / RATE) / min(.055, seconds * .24), 1).clip(0, 1)
    rms = float(np.sqrt(np.mean(out * out)))
    out *= min(target / max(rms, 1e-9), .68 / max(float(np.max(np.abs(out))), 1e-9))
    out[0] = out[-1] = 0
    return out


def write_wav(path, samples):
    path.parent.mkdir(parents=True, exist_ok=True)
    with wave.open(str(path), "wb") as handle:
        handle.setparams((1, 2, RATE, len(samples), "NONE", "not compressed"))
        handle.writeframes(np.rint(samples * 32767).astype("<i2").tobytes())


def read_pcm(path):
    with wave.open(str(path), "rb") as handle:
        assert (handle.getnchannels(), handle.getsampwidth(), handle.getframerate()) == (1, 2, RATE), path
        return np.frombuffer(handle.readframes(handle.getnframes()), dtype="<i2")


def metrics(key):
    path = OUTPUT / f"{key}.wav"
    pcm = read_pcm(path)
    samples = pcm.astype(float) / 32768
    peak, rms = float(np.max(np.abs(samples))), float(np.sqrt(np.mean(samples ** 2)))
    spectrum = np.abs(np.fft.rfft(samples)) ** 2
    frequencies = np.fft.rfftfreq(samples.size, 1 / RATE)
    return {"cue": key, "direction": SPECS[key][2], "duration_seconds": len(pcm) / RATE,
            "peak_dbfs": round(20 * np.log10(max(peak, 1e-9)), 3),
            "rms_dbfs": round(20 * np.log10(max(rms, 1e-9)), 3),
            "dc": round(float(np.mean(samples)), 8),
            "spectral_centroid_hz": round(float(np.sum(spectrum * frequencies) / np.sum(spectrum)), 1),
            "sha256": hashlib.sha256(path.read_bytes()).hexdigest()}


def validate():
    saved = {row["cue"]: row for row in json.loads(MANIFEST.read_text())["masters"]}
    assert set(saved) == set(SPECS), "Manifest bank differs from the source recipe"
    hashes = set()
    for key in SPECS:
        pcm = read_pcm(OUTPUT / f"{key}.wav")
        assert np.array_equal(pcm, np.rint(render(key) * 32767).astype("<i2")), key + " is not reproducible"
        info = metrics(key)
        assert info == saved[key], key + " manifest is stale"
        assert info["sha256"] not in hashes, key + " duplicates another master"
        hashes.add(info["sha256"])
        assert pcm[0] == pcm[-1] == 0, key + " edge is discontinuous"
        assert abs(info["dc"]) < .004, key + " DC offset is too high"
        assert info["peak_dbfs"] <= -3.3, key + " has insufficient headroom"
        assert -34 < info["rms_dbfs"] < -16, key + " level outside authored range"
        assert np.max(np.abs(pcm[-96:])) < 1000, key + " abrupt tail"
    return len(hashes)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--validate-only", action="store_true")
    args = parser.parse_args()
    if not args.validate_only:
        template = (OUTPUT / "itemequip.wav.meta").read_text()
        playlist, sequence = [], []
        elapsed = 0.
        for key in SPECS:
            samples = render(key)
            path = OUTPUT / f"{key}.wav"
            write_wav(path, samples)
            meta = Path(str(path) + ".meta")
            if not meta.exists():
                lines = template.splitlines()
                lines[1] = "guid: " + hashlib.md5(("ashen-skill-spell/" + key).encode()).hexdigest()
                meta.write_text("\n".join(lines) + "\n", encoding="utf-8")
            playlist.append({"cue": key, "start_seconds": round(elapsed, 3)})
            sequence.extend((samples * .78 * .75, np.zeros(round(.3 * RATE))))
            elapsed += samples.size / RATE + .3
        payload = {"generator": "Tools/Audio/BuildSkillSpellSfx.py", "sample_rate": RATE,
                   "channels": 1, "source": "original deterministic synthesis; no external recordings or service",
                   "masters": [metrics(key) for key in SPECS]}
        MANIFEST.write_text(json.dumps(payload, indent=2) + "\n", encoding="utf-8")
        write_wav(QA / "skill-spell-isolated-preview.wav", np.concatenate(sequence))
        (QA / "preview-playlist.json").write_text(json.dumps(playlist, indent=2) + "\n", encoding="utf-8")
    print(f"Validated {validate()} original skill/spell masters: exact source reproduction, unique PCM, headroom, edges, DC and levels.")


if __name__ == "__main__":
    main()
