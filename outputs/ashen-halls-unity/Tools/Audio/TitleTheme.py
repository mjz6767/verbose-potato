#!/usr/bin/env python3
"""The Ember Oath: an original, through-composed sixty-second title miniature.

Only NumPy and the standard library are used. No recordings, remote services,
sample libraries or generated-audio models are involved. ``compose(spec)`` is
the existing audio builder's deterministic floating-point stereo interface.
Pitch numbers in the score are semitones above D4, not random scale walks.
"""

from __future__ import annotations

import hashlib
import math
from dataclasses import dataclass

import numpy as np


SAMPLE_RATE = 32_000
BPM = 96
TOTAL_BEATS = 96
DURATION_SECONDS = 60.0
TITLE_CUE = "tavern_storm_hearth_ensemble_loop"
COMPOSITION_NAME = "The Ember Oath"
TITLE = COMPOSITION_NAME
PICKUP_SECONDS = .35
HOOK_SECONDS = 1.20
SECTIONS = (
    ("signature", 0, 16),
    ("company", 16, 32),
    ("intimate", 32, 48),
    ("ascent", 48, 64),
    ("return", 64, 88),
    ("coda", 88, 96),
)

# D--A--Bb-A: a rising fifth and a leaning semitone, grouped 3+1+1.5+2.5
# eighth notes. The longer final A makes the figure easy to sing and remember.
# Each item is onset in beats, semitone, sounding duration, expressive accent.
MOTIF = ((0, 0, 1.36, 1.03), (1.5, 7, .39, .90),
         (2, 8, .66, 1.00), (2.75, 7, 1.12, .96))
ANSWER = ((4.25, 3, .90, .98), (5.5, 2, .52, .84), (6.5, 0, 1.28, .96))

# The first statement waits for the existing title reveal to clear. A separate
# plucked signature at .35 s creates immediate identity before the horn breath.
MELODY = (
    # 0--10 s: question, then an upward answer that reaches the A dominant.
    (1.92, 0, 1.36), (3.42, 7, .39), (3.92, 8, .66), (4.67, 7, 1.12),
    (6.00, 3, .90), (7.25, 2, .52), (8.00, 0, 1.60),
    (10.00, 3, 1.20), (11.50, 5, .40), (12.00, 7, .65), (12.75, 9, 1.00),
    (14.00, 14, .72), (15.00, 11, .75),
    # 10--20 s: company statement, then the Dorian major-IV answer.
    (16.00, 0, 1.36), (17.50, 7, .39), (18.00, 8, .66), (18.75, 7, 1.12),
    (20.25, 3, .90), (21.50, 2, .52), (22.50, 0, 1.28),
    (24.00, 5, 1.25), (25.50, 9, .40), (26.00, 12, .66), (26.75, 9, 1.00),
    (28.25, 7, .90), (29.50, 5, .50), (30.25, -1, 1.25),
    # 20--30 s: exposed lute remembers the call over Bb/F, with breath between.
    (32.50, 0, 1.30), (34.00, 7, .44), (34.50, 8, .65), (35.25, 7, 1.10),
    (37.00, 3, 1.20), (39.00, 2, .75),
    (40.50, 8, 1.35), (42.25, 7, .75), (43.25, 3, .70),
    (44.50, 2, .70), (45.75, -1, .60), (47.00, -5, .72),
    # 30--40 s: new climbing line, not another literal motif repetition.
    (48.00, 0, .75), (49.00, 3, .75), (50.00, 7, 1.35),
    (52.00, 10, 1.35), (53.50, 14, .45), (54.25, 12, 1.25),
    (56.00, 8, 1.35), (57.50, 12, .45), (58.25, 15, 1.30),
    (60.00, 14, 1.25), (61.50, 11, .55), (62.50, 7, 1.15),
    # 40--50 s: recognizable return and an expanded, lyrical upper answer.
    (64.00, 0, 1.36), (65.50, 7, .39), (66.00, 8, .66), (66.75, 7, 1.12),
    (68.25, 3, .90), (69.50, 2, .52), (70.50, 0, 1.28),
    (72.00, 15, 1.45), (73.75, 14, .65), (74.75, 12, 1.10),
    (76.25, 10, .90), (77.50, 8, .52), (78.25, 7, 1.20),
    # 50--55 s: the high point broadens rather than adding more percussion.
    (80.00, 7, .75), (81.00, 12, .75), (82.00, 15, 1.45),
    (84.00, 14, .80), (85.00, 11, .80), (86.25, 7, 1.35),
    # 55--60 s: a composed tonic coda, not a fade-out or arbitrary quiet cut.
    (88.25, 3, 1.20), (89.75, 2, .60), (90.75, 0, 1.45),
    (93.00, -5, .80), (94.25, 0, 1.60),
)

# MIDI-relative-to-D3 upper-string voicings. Explicit inversions preserve the
# inner lines and leave the central lead room; A7's C# is a real destination.
HARMONY = (
    ((-5, 0, 3), -12, "Dm(add9)"), ((-5, 3, 8), -12, "Bbmaj7/D"),
    ((-7, -4, 0), -7, "Gm"), ((-5, -1, 5), -5, "A7"),
    ((-5, 0, 3), -12, "Dm"), ((-5, -2, 3), -14, "F/C"),
    ((-7, -3, 0), -7, "G"), ((-5, -1, 5), -5, "A7"),
    ((-9, -5, 0), -4, "Bbmaj7"), ((-9, -5, -2), -5, "F/A"),
    ((-4, 0, 7), -7, "Gm9"), ((-5, -1, 5), -5, "A7"),
    ((-5, 0, 3), -12, "Dm"), ((-7, 0, 2), -14, "Cadd9"),
    ((-4, 0, 3), -4, "Bb"), ((-5, -1, 5), -5, "A7"),
    ((-5, 0, 3), -12, "Dm"), ((-5, 0, 3), -4, "Bbmaj7"),
    ((-9, -5, 5), -9, "Fadd9"), ((-7, -4, 2), -7, "Gm6"),
    ((-5, 0, 3), -5, "Dm/A"), ((-5, -1, 5), -5, "A7"),
    ((-5, 0, 3), -12, "Dm(add9)"), ((-5, 0, 2), -12, "D open ninth"),
)


@dataclass(frozen=True)
class Note:
    instrument: str
    midi: float
    beat: float
    duration: float
    gain: float
    pan: float = 0.0
    expression: float = .7


def _seed(name: str) -> int:
    return int.from_bytes(hashlib.sha256(name.encode("utf-8")).digest()[:8], "little")


def _smooth(value: np.ndarray) -> np.ndarray:
    value = np.clip(value, 0, 1)
    return value * value * (3 - 2 * value)


def _envelope(frames: int, attack: float, release: float) -> np.ndarray:
    t = np.arange(frames, dtype=np.float64) / SAMPLE_RATE
    duration = frames / SAMPLE_RATE
    return _smooth(t / max(.001, attack)) * _smooth((duration - t) / max(.001, release))


def _lowpass(signal: np.ndarray, width: int) -> np.ndarray:
    return np.convolve(signal, np.ones(width) / width, mode="same")


def _phase(frequency: float, frames: int, rng: np.random.Generator,
           cents: float = 5.0, detune: float = 0.0) -> np.ndarray:
    """Integrate instantaneous pitch: true cents vibrato, not phase wobble."""
    t = np.arange(frames, dtype=np.float64) / SAMPLE_RATE
    delayed_vibrato = _smooth((t - .12) / .28)
    drift = .9 * np.sin(t * 1.7 + rng.uniform(0, math.pi * 2))
    pitch_cents = detune + drift + cents * delayed_vibrato * np.sin(2 * math.pi * 4.7 * t)
    pitch_cents -= 9.0 * np.exp(-t * 48)  # the tiny initial breath/rosin scoop
    hz = frequency * np.exp2(pitch_cents / 1200)
    return 2 * math.pi * np.cumsum(hz) / SAMPLE_RATE + rng.uniform(0, 2 * math.pi)


def _aerophone(frequency: float, duration: float, expression: float,
               rng: np.random.Generator, reed: bool = False) -> np.ndarray:
    frames = max(32, round(duration * SAMPLE_RATE))
    t = np.arange(frames, dtype=np.float64) / SAMPLE_RATE
    phase = _phase(frequency, frames, rng, 5.3 if reed else 4.1)
    result = np.zeros(frames)
    bloom = _smooth(t / .13)
    # A dark attack opens into the middle partials. The fundamental stays firm;
    # upper energy has a broad body resonance instead of a hard sawtooth buzz.
    weights = ((1, .74), (2, .49), (3, .29), (4, .13), (5, .09), (6, .045), (7, .025))
    for harmonic, weight in weights:
        hz = harmonic * frequency
        resonance = 1 + (.50 if reed else .30) * math.exp(-.5 * ((hz - 1150) / 480) ** 2)
        if reed and harmonic % 2 == 0:
            weight *= .66
        opening = np.ones(frames) if harmonic == 1 else (.52 + .48 * bloom) * (.65 + expression * .35)
        result += np.sin(phase * harmonic) * weight * resonance * opening
    breath = rng.normal(0, 1, frames)
    breath = _lowpass(breath, 9) - _lowpass(breath, 61)
    result += breath * (.021 if reed else .011) * (1 + 2 * np.exp(-t * 35))
    arch = .88 + .12 * np.sin(math.pi * np.minimum(t / max(.1, duration), 1))
    return result * arch * _envelope(frames, .065 if reed else .052, min(.19, duration * .25)) * .72


def _strings(frequency: float, duration: float, expression: float,
             rng: np.random.Generator, short: bool = False, bass: bool = False) -> np.ndarray:
    frames = max(32, round(duration * SAMPLE_RATE))
    t = np.arange(frames, dtype=np.float64) / SAMPLE_RATE
    result = np.zeros(frames)
    for player, detune in enumerate((-3.6, 2.8)):
        phase = _phase(frequency, frames, rng, 3.5 if bass else 5.2, detune)
        for harmonic in range(1, 10):
            hz = frequency * harmonic
            if hz > 6200:
                break
            weight = 1 / harmonic ** 1.33
            if bass and harmonic == 1:
                weight *= .31  # bass is audible on small speakers, not sub-heavy
            body = 1 + .24 * math.exp(-.5 * ((hz - 1400) / 700) ** 2)
            result += np.sin(phase * harmonic + player * .08) * weight * body * .34
    noise = _lowpass(rng.normal(0, 1, frames), 13)
    result += noise * .018 * (1 + np.exp(-t * 18))
    if short:
        result *= np.exp(-t * 5.1) * _envelope(frames, .018, min(.065, duration * .2))
    else:
        swell = .79 + .21 * np.sin(math.pi * np.clip(t / duration, 0, 1))
        result *= swell * _envelope(frames, .16 if not bass else .095, min(.28, duration * .2))
    return result * (.80 + .20 * expression)


def _lute(frequency: float, duration: float, expression: float, rng: np.random.Generator) -> np.ndarray:
    frames = max(32, round(duration * SAMPLE_RATE))
    t = np.arange(frames, dtype=np.float64) / SAMPLE_RATE
    result = np.zeros(frames)
    phase = rng.uniform(-.12, .12)
    for harmonic in range(1, 13):
        hz = frequency * harmonic * math.sqrt(1 + .00013 * harmonic * harmonic)
        if hz > 9000:
            break
        amplitude = math.sin(math.pi * harmonic * .22) / harmonic ** 1.28
        decay = np.exp(-t * (2.1 + harmonic * .53 + frequency / 1800))
        result += np.sin(2 * math.pi * hz * t + phase) * amplitude * decay
    pick = rng.normal(0, 1, frames)
    pick = _lowpass(pick, 5) - _lowpass(pick, 25)
    result += pick * .07 * np.exp(-t * 95)
    return result * _envelope(frames, .0025, min(.09, duration * .2)) * (1.15 + expression * .18)


def _bell(frequency: float, duration: float, rng: np.random.Generator) -> np.ndarray:
    frames = max(32, round(duration * SAMPLE_RATE))
    t = np.arange(frames) / SAMPLE_RATE
    result = np.zeros(frames)
    for ratio, gain, decay in ((1, .70, 1.7), (2.002, .32, 2.8), (3.994, .08, 4.5)):
        result += np.sin(2 * math.pi * frequency * ratio * t) * gain * np.exp(-t * decay)
    return result * _envelope(frames, .002, .10)


def _percussion(kind: str, duration: float, expression: float, rng: np.random.Generator) -> np.ndarray:
    frames = max(32, round(duration * SAMPLE_RATE))
    t = np.arange(frames) / SAMPLE_RATE
    noise = rng.normal(0, 1, frames)
    if kind == "brush":
        noise = noise - _lowpass(noise, 31)
        return noise * np.exp(-t * 24) * _envelope(frames, .007, .035) * .15
    if kind == "bronze":
        noise = _lowpass(noise, 3) - _lowpass(noise, 29)
        return noise * np.exp(-t * 3.1) * _envelope(frames, .026, .35) * .28
    frequency = 78 if kind == "lowdrum" else 148
    phase = 2 * math.pi * (frequency * t + frequency * .35 * (1 - np.exp(-t * 31)) / 31)
    body = np.sin(phase) * np.exp(-t * (8 if kind == "lowdrum" else 14))
    skin = (_lowpass(noise, 3) - _lowpass(noise, 43)) * np.exp(-t * 38) * .19
    return (body * .72 + skin) * _envelope(frames, .002, .05) * (.8 + .2 * expression)


def _synthesize(note: Note, rng: np.random.Generator) -> np.ndarray:
    frequency = 440 * 2 ** ((note.midi - 69) / 12)
    duration = note.duration * 60 / BPM
    if note.instrument in {"horn", "reed"}:
        return _aerophone(frequency, duration, note.expression, rng, note.instrument == "reed")
    if note.instrument in {"strings", "cello", "spiccato"}:
        return _strings(frequency, duration, note.expression, rng,
                        short=note.instrument == "spiccato", bass=note.instrument == "cello")
    if note.instrument == "lute":
        return _lute(frequency, duration, note.expression, rng)
    if note.instrument == "bell":
        return _bell(frequency, duration, rng)
    return _percussion(note.instrument, duration, note.expression, rng)


def score(root_midi: int = 50) -> tuple[Note, ...]:
    """Return the explicit performance plan for independent structure checks."""
    notes: list[Note] = []

    def add(instrument: str, semitone: float, beat: float, duration: float,
            gain: float, pan: float = 0, expression: float = .7) -> None:
        notes.append(Note(instrument, root_midi + semitone, beat, duration, gain, pan, expression))

    # The dry, three-note signature echoes the motif's rising fifth and semitone.
    for when, semitone, gain in ((.56, 12, .071), (.96, 19, .050), (1.28, 20, .040)):
        add("lute", semitone, when, .55, gain, -.08)

    for when, semitone, duration in MELODY:
        intimate = 32 <= when < 48
        returning = 64 <= when < 88
        instrument = "lute" if 32 <= when < 40 else "reed" if intimate or when >= 88 else "horn"
        gain = .133 if instrument == "lute" else .104 if intimate else .133 if returning else .118
        if when >= 88:
            gain = .092
        add(instrument, 12 + semitone, when, duration, gain, -.025,
            .88 if returning else .68 if not intimate else .50)
        if returning and when < 72:
            # The main tune stays central; a quiet bowed octave widens its return.
            add("strings", 24 + semitone, when + .035, duration + .09, .020, .24, .65)
        if 72 <= when < 88 and duration > 1:
            add("horn", semitone, when + .035, duration, .028, -.22, .72)

    for bar, (voicing, bass, label) in enumerate(HARMONY):
        origin = float(bar * 4)
        intimate = 8 <= bar < 12
        returning = 16 <= bar < 22
        coda = bar >= 22
        level = .68 if intimate else .76 if coda else 1.10 if returning else .84 if bar < 4 else 1.0
        if 12 <= bar < 16:
            level = .78 + (bar - 12) * .09

        # The first bar has open fifths; the minor third blooms under the hook.
        for voice, semitone in enumerate(voicing):
            if intimate and voice == 1:
                continue
            onset = origin + (.30 + voice * .06 if bar == 0 else .06 + voice * .027)
            gain = (.024 if intimate else .030) * level
            add("strings", semitone, onset, 4.16, gain, (-.46, .04, .42)[voice], level * .65)
        add("cello", bass, origin + .08, 3.82, .046 * level, -.04, .64)
        if returning:
            add("horn", voicing[0], origin + .18, 3.50, .026, -.28, .63)

        # Hand-played arpeggios use an irregular spacing and rest at the phrase
        # ends; there is no uninterrupted one-note-per-beat synth grid.
        if bar not in (0, 11, 23):
            positions = ((.0, 0), (.83, 1), (1.5, 2), (2.75, 1))
            if intimate:
                positions = ((.1, 0), (1.65, 2), (3.1, 1))
            for index, (position, voice) in enumerate(positions):
                add("lute", voicing[voice] + 12, origin + position, 1.30,
                    .031 * level, -.36 if index % 2 == 0 else .33, .56)

        # Short strings echo the theme's uneven cell only while the company moves.
        if 4 <= bar < 8 or 12 <= bar < 22:
            for index, (position, voice) in enumerate(((0, 0), (1.5, 2), (2, 1), (2.75, 2))):
                add("spiccato", voicing[voice], origin + position, .48,
                    (.038 if index == 0 else .025) * level, -.31 if index % 2 == 0 else .31, .68)

        # No drums in the lyrical bridge or coda. One heartbeat in the initial
        # call makes space for the separate UI forge strike and reveal chime.
        if bar in (2, 3) or 4 <= bar < 8 or 12 <= bar < 22:
            add("lowdrum", 0, origin + .015, .68, .075 * level, -.07)
            if bar >= 4 and bar not in (12, 13):
                add("framedrum", 0, origin + 2.75, .36, .055 * level, .21)
            if returning:
                for position, gain in ((1.5, .022), (3.5, .019)):
                    add("brush", 0, origin + position, .25, gain, .36)
        if bar in (7, 15, 21):
            for index, position in enumerate((3, 3.5, 3.75)):
                add("framedrum", 0, origin + position, .25, .021 + .008 * index, -.2 + .2 * index)

    # Countermelodies enter only in the lead's gaps, with a different contour.
    for when, semitone, duration in ((23.2, 7, .65), (27.8, 12, .55),
                                    (38.3, 7, .65), (43.9, 0, .55),
                                    (55.5, 7, .45), (59.5, 8, .45),
                                    (71.4, 7, .55), (75.7, 7, .50),
                                    (79.5, 5, .45), (83.5, 7, .45)):
        add("reed", 12 + semitone, when, duration, .033, .29, .52)
    for when, semitone, gain in ((16, 24, .013), (48, 19, .011), (64, 24, .020), (88.3, 24, .014)):
        add("bell", semitone, when, 2.3, gain, .38)
    for when, gain in ((16, .017), (64, .026), (80, .023)):
        add("bronze", 0, when, 2.0, gain, -.36)
    return tuple(notes)


def _add_circular(target: np.ndarray, mono: np.ndarray, start: int, gain: float, pan: float) -> None:
    angle = (np.clip(pan, -1, 1) + 1) * math.pi / 4
    channel_gains = (gain * math.cos(angle), gain * math.sin(angle))
    cursor, source = start % target.shape[1], 0
    while source < mono.size:
        count = min(target.shape[1] - cursor, mono.size - source)
        for channel in range(2):
            target[channel, cursor:cursor + count] += mono[source:source + count] * channel_gains[channel]
        source += count
        cursor = 0


def _room(stereo: np.ndarray, seed: int) -> np.ndarray:
    """Diffuse, low-level hall tail plus early reflections, circular at the loop."""
    rng = np.random.default_rng(seed)
    frames = stereo.shape[1]
    ir_frames = int(1.35 * SAMPLE_RATE)
    t = np.arange(ir_frames) / SAMPLE_RATE
    wet = np.zeros_like(stereo)
    for channel in range(2):
        impulse = np.zeros(frames)
        noise = _lowpass(rng.normal(0, 1, ir_frames), 7)
        late = noise * np.exp(-t * 4.6) * _smooth((t - .050) / .07)
        late *= .16 / max(float(np.sqrt(np.sum(late * late))), 1e-9)
        impulse[:ir_frames] = late
        for delay, gain in ((.031, .082), (.053, .061), (.089, .046), (.137, .032)):
            impulse[round((delay + channel * .0037) * SAMPLE_RATE)] += gain
        source = stereo[channel] * .84 + stereo[1 - channel] * .16
        wet[channel] = np.fft.irfft(np.fft.rfft(source) * np.fft.rfft(impulse), n=frames)
    return wet


def _master(stereo: np.ndarray) -> np.ndarray:
    result = stereo.copy()
    result -= np.mean(result, axis=1, keepdims=True)
    # Musical bass retains its upper partials. Clear the sub range before gain
    # staging so it cannot steal headroom from the tune on ordinary speakers.
    frequencies = np.fft.rfftfreq(result.shape[1], 1 / SAMPLE_RATE)
    highpass = 1 - np.exp(-(frequencies / 75.0) ** 4)
    gentle_top = 1 / np.sqrt(1 + (frequencies / 9300) ** 8)
    for channel in range(2):
        result[channel] = np.fft.irfft(np.fft.rfft(result[channel]) * highpass * gentle_top, n=result.shape[1])
    rms = float(np.sqrt(np.mean(result * result)))
    result *= 10 ** (-19.0 / 20) / max(rms, 1e-9)
    ceiling = 10 ** (-3.4 / 20)
    peak = float(np.max(np.abs(result)))
    # Preserve dynamic phrasing; apply no always-on tanh/compressor to the lead.
    if peak > ceiling:
        result *= ceiling / peak
    # Hermite-free bounded bridge prevents both a click and interpolator spikes.
    half = round(.008 * SAMPLE_RATE)
    u = np.arange(half * 2) / (half * 2 - 1)
    ramp = _smooth(u)
    for channel in range(2):
        bridge = result[channel, -half] + (result[channel, half] - result[channel, -half]) * ramp
        result[channel, -half:] = bridge[:half]
        result[channel, :half] = bridge[half:]
    return result


def compose(spec) -> np.ndarray:
    """Render the title cue; maintain the existing sixty-second runtime contract."""
    if spec.cue != TITLE_CUE:
        raise ValueError("TitleTheme can only render the main title cue")
    if not math.isclose(float(spec.bpm), BPM):
        raise ValueError("The authored title requires 96 BPM for its sixty-second form")
    frames = round(DURATION_SECONDS * SAMPLE_RATE)
    rng = np.random.default_rng(_seed("ashen-halls:the-ember-oath:score-1"))
    body = np.zeros((2, frames))
    lead = np.zeros((2, frames))
    for note in score(spec.root_midi):
        signal = _synthesize(note, rng)
        # The lead has the shortest room send and remains close to the listener.
        foreground = abs(note.pan) < .03 and note.instrument in {"horn", "reed", "lute"}
        target = lead if foreground else body
        _add_circular(target, signal, round(note.beat * 60 / BPM * SAMPLE_RATE), note.gain, note.pan)
    mix = body + lead
    mix += _room(body, _seed("ember-oath:chamber")) * .74
    mix += _room(lead, _seed("ember-oath:lead-room")) * .34
    return _master(mix)


def metadata() -> dict[str, object]:
    return {
        "title": TITLE,
        "composition": COMPOSITION_NAME,
        "cue": TITLE_CUE,
        "sample_rate": SAMPLE_RATE,
        "bpm": BPM,
        "root_midi": 50,
        "mode": [0, 2, 3, 5, 7, 8, 11],
        "progression": [0, 5, 3, 4, 0, 2, 3, 4, 5, 2, 3, 4,
                        0, 6, 5, 4, 0, 5, 2, 3, 0, 4, 0, 0],
        "direction": "An original Ember Oath signature, with a leaning-semitone horn call, intimate lute/reed memory, prepared dominant ascent, and dark heroic return.",
        "mode_detail": "D minor with harmonic-minor dominants, a borrowed Dorian major IV, and written chord inversions",
        "duration_seconds": DURATION_SECONDS,
        "pickup_seconds": PICKUP_SECONDS,
        "hook_seconds": HOOK_SECONDS,
        "motif_semitones": [event[1] for event in MOTIF],
        "motif_onset_beats": [event[0] for event in MOTIF],
        "motif": [{"beat": beat, "semitone": semitone, "duration_beats": duration}
                  for beat, semitone, duration, accent in MOTIF],
        "sections": [{"name": name, "start_seconds": start * 60 / BPM,
                      "end_seconds": end * 60 / BPM} for name, start, end in SECTIONS],
        "harmony": [item[2] for item in HARMONY],
        "external_samples": False,
        "provenance": "Original authored score and deterministic synthesis; no external audio or services",
    }
