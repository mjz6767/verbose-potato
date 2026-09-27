#!/usr/bin/env python3
"""Delivery and musical-identity contracts for the separately authored title."""

from __future__ import annotations

import hashlib
import json
import unittest
from types import SimpleNamespace

import numpy as np

import TitleTheme as title


class TitleThemeTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls) -> None:
        cls.spec = SimpleNamespace(cue=title.TITLE_CUE, bpm=96, root_midi=50)
        cls.master = title.compose(cls.spec)

    def test_delivery_is_audible_stereo_with_headroom_and_a_clean_wrap(self) -> None:
        audio = self.master
        self.assertEqual((2, 60 * 32_000), audio.shape)
        self.assertTrue(np.all(np.isfinite(audio)))
        self.assertLess(np.max(np.abs(np.mean(audio, axis=1))), .001)
        self.assertLessEqual(np.max(np.abs(audio)), 10 ** (-3 / 20))
        self.assertGreater(np.sqrt(np.mean(audio * audio)), 10 ** (-24 / 20))
        self.assertLess(np.max(np.abs(audio[:, 0] - audio[:, -1])), .002)
        mono_rms = np.sqrt(np.mean(np.mean(audio, axis=0) ** 2))
        stereo_rms = np.sqrt(np.mean(audio * audio))
        self.assertGreater(mono_rms / stereo_rms, .8)

    def test_rebuild_produces_identical_pcm_without_global_rng_state(self) -> None:
        before = np.round(self.master.T * 32767).astype("<i2").tobytes()
        np.random.seed(713)
        np.random.random(127)
        after = np.round(title.compose(self.spec).T * 32767).astype("<i2").tobytes()
        self.assertEqual(hashlib.sha256(before).digest(), hashlib.sha256(after).digest())

    def test_signature_survives_the_opening_company_and_return(self) -> None:
        # Protect the tune the player learns, including the unequal rhythm and
        # the leaning semitone; an arbitrary scale or a new seed is not a theme.
        notes = title.MELODY
        for start in (1.92, 16.0, 64.0):
            phrase = [event for event in notes if start <= event[0] < start + 4]
            self.assertEqual([0, 7, 8, 7], [event[1] for event in phrase])
            np.testing.assert_allclose([event[0] - start for event in phrase], [0, 1.5, 2, 2.75])
        self.assertEqual(1.2, title.HOOK_SECONDS)
        self.assertLess(title.PICKUP_SECONDS, title.HOOK_SECONDS)

    def test_intimate_section_removes_drums_and_returns_with_dynamic_space(self) -> None:
        plan = title.score()
        percussion = {"lowdrum", "framedrum", "brush", "bronze"}
        self.assertFalse(any(note.instrument in percussion and 32 <= note.beat < 48 for note in plan))
        self.assertFalse(any(note.instrument in percussion and note.beat >= 88 for note in plan))
        self.assertTrue(any(note.instrument == "lute" and abs(note.pan) < .03 and 32 <= note.beat < 40 for note in plan))
        sr = title.SAMPLE_RATE
        intimate = self.master[:, 20 * sr:30 * sr]
        returned = self.master[:, 40 * sr:55 * sr]
        contrast = 20 * np.log10(np.sqrt(np.mean(returned ** 2)) / np.sqrt(np.mean(intimate ** 2)))
        self.assertGreater(contrast, 3)

    def test_metadata_and_input_contract_can_be_used_by_the_shared_builder(self) -> None:
        payload = json.loads(json.dumps(title.metadata()))
        self.assertEqual("The Ember Oath", payload["title"])
        self.assertEqual(24, len(payload["progression"]))
        self.assertEqual(60, payload["sections"][-1]["end_seconds"])
        self.assertEqual(0, payload["sections"][0]["start_seconds"])
        for voicing, bass, label in title.HARMONY:
            if label == "A7":
                # D-relative pitch classes A/C#/G establish a real dominant.
                self.assertTrue({7, 11, 5}.issubset({note % 12 for note in (*voicing, bass)}))
        with self.assertRaises(ValueError):
            title.compose(SimpleNamespace(cue="old_road_walk_loop", bpm=96, root_midi=50))
        with self.assertRaises(ValueError):
            title.compose(SimpleNamespace(cue=title.TITLE_CUE, bpm=92, root_midi=50))


if __name__ == "__main__":
    unittest.main()
