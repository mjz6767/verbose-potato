#!/usr/bin/env python3
"""Focused deterministic contracts for the authored music and runtime QA mixes."""

from __future__ import annotations

import math
import sys
import unittest
from pathlib import Path

import numpy as np


sys.path.insert(0, str(Path(__file__).resolve().parent))
import BuildOriginalAudio as audio  # noqa: E402


class OriginalAudioContractTests(unittest.TestCase):
    def test_music_blueprints_are_unique(self) -> None:
        self.assertEqual([], audio.track_blueprint_collisions())

    def test_track_specs_fit_the_unity_authored_duration_window(self) -> None:
        self.assertEqual([], audio.track_spec_duration_violations())
        self.assertEqual((15.0, 30.1), audio.music_duration_bounds("the_crypt_keeps_its_names_loop"))
        self.assertEqual((15.0, 60.1), audio.music_duration_bounds(audio.TITLE_MUSIC_CUE))

    def test_equal_power_crossfade_preserves_unit_power(self) -> None:
        progress = np.linspace(0.0, 1.0, 257)
        outgoing, incoming = audio.equal_power_crossfade(progress)
        np.testing.assert_allclose(outgoing * outgoing + incoming * incoming, 1.0, atol=1e-12)
        self.assertAlmostEqual(1.0, float(outgoing[0]), places=12)
        self.assertAlmostEqual(0.0, float(incoming[0]), places=12)
        self.assertAlmostEqual(0.0, float(outgoing[-1]), places=12)
        self.assertAlmostEqual(1.0, float(incoming[-1]), places=12)
        self.assertAlmostEqual(math.sqrt(0.5), float(outgoing[128]), places=12)
        self.assertAlmostEqual(math.sqrt(0.5), float(incoming[128]), places=12)

    def test_world_map_preview_routes_local_overview_then_pursuit(self) -> None:
        routes = audio.WORLD_MAP_PREVIEW_ROUTES
        self.assertEqual(
            ("old_road_walk_loop", "ashen_atlas_overview_loop", "footsteps_behind_loop"),
            tuple(route[1] for route in routes),
        )
        self.assertEqual((0.0, 4.5, 12.0), tuple(route[2] for route in routes))
        for index, route in enumerate(routes[1:], start=1):
            self.assertGreater(route[3], 0.0)
            self.assertLess(route[2] + route[3], 20.0)
            self.assertGreater(route[2], routes[index - 1][2] + routes[index - 1][3])

    def test_preview_constants_match_runtime_sources(self) -> None:
        self.assertEqual([], audio.validate_runtime_source_contracts())

    def test_runtime_preview_level_window_is_practical_not_sample_tuned(self) -> None:
        self.assertAlmostEqual(-3.0, audio.db(audio.RUNTIME_PREVIEW_MAX_PEAK), places=6)
        self.assertAlmostEqual(-42.0, audio.db(audio.RUNTIME_PREVIEW_MIN_RMS), places=6)
        self.assertAlmostEqual(-18.0, audio.db(audio.RUNTIME_PREVIEW_MAX_RMS), places=6)
        self.assertGreater(
            audio.db(audio.RUNTIME_PREVIEW_MAX_RMS) - audio.db(audio.RUNTIME_PREVIEW_MIN_RMS),
            20.0,
        )

    def test_epic_combat_deliveries_have_headroom_clean_wraps_and_contrasting_phrases(self) -> None:
        fingerprints = set()
        for cue in audio.EPIC_COMBAT_CUES:
            with self.subTest(cue=cue):
                path = audio.MUSIC_DIR / (cue + ".wav")
                samples, sample_rate = audio.read_pcm16(path)
                self.assertEqual(audio.MUSIC_SAMPLE_RATE, sample_rate)
                self.assertEqual([], audio.validate_epic_combat_audio(samples, sample_rate))
                spec = next(item for item in audio.TRACKS if item.cue == cue)
                self.assertAlmostEqual(audio.expected_track_duration_seconds(spec), samples.shape[1] / sample_rate, places=4)
                metrics = audio.epic_combat_metrics(samples, sample_rate)
                # The intentional withdrawal must survive the PCM master. This
                # also catches replacing the arrangement with a repeated phrase.
                self.assertLess(metrics["phrase_rms_dbfs"][2], metrics["phrase_rms_dbfs"][0] - 3)
                fingerprints.add(audio.sha256(path))
        self.assertEqual(len(audio.EPIC_COMBAT_CUES), len(fingerprints))

    def test_epic_combat_validator_rejects_a_broken_wrap_and_flat_arrangement(self) -> None:
        samples, sample_rate = audio.read_pcm16(audio.MUSIC_DIR / (audio.EPIC_COMBAT_CUES[0] + ".wav"))
        broken = samples.copy()
        broken[:, 0] += .1
        self.assertIn("audible loop boundary discontinuity", audio.validate_epic_combat_audio(broken, sample_rate))
        flat = np.tile(np.array_split(samples, 4, axis=1)[0], (1, 4))
        self.assertIn("missing contrast between the battle and withdrawal phrases", audio.validate_epic_combat_audio(flat, sample_rate))
        broken[0, 0] = np.nan
        self.assertEqual(["non-finite audio samples"], audio.validate_epic_combat_audio(broken, sample_rate))

    def test_epic_combat_score_is_reproducible_from_its_blueprint(self) -> None:
        spec = next(item for item in audio.TRACKS if item.cue == audio.EPIC_COMBAT_CUES[0])
        generated = audio.compose_track(spec)
        written, _ = audio.read_pcm16(audio.MUSIC_DIR / (spec.cue + ".wav"))
        quantized = np.round(generated * 32767).astype("<i2").astype(np.float64) / 32768
        np.testing.assert_array_equal(quantized, written)


if __name__ == "__main__":
    unittest.main()
