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


if __name__ == "__main__":
    unittest.main()
