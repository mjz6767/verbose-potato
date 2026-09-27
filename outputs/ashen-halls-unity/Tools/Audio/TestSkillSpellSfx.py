#!/usr/bin/env python3
"""Read-only regressions for the original skill/spell masters."""
import unittest
import numpy as np
import BuildSkillSpellSfx as bank


class SkillSpellSfxTests(unittest.TestCase):
    def test_saved_bank_reproduces_and_meets_mastering_contract(self):
        self.assertEqual(21, bank.validate())

    def test_school_textures_are_not_pitch_shifted_copies(self):
        keys = [key for key in bank.SPECS if key.startswith("release")]
        centroids = {round(bank.metrics(key)["spectral_centroid_hz"] / 100) for key in keys}
        self.assertGreaterEqual(len(centroids), 6)
        self.assertGreater(bank.metrics("releasefrost")["spectral_centroid_hz"],
                           bank.metrics("releasenature")["spectral_centroid_hz"] * 2)
        self.assertGreater(bank.metrics("releaseshock")["spectral_centroid_hz"],
                           bank.metrics("releasepact")["spectral_centroid_hz"] * 4)

    def test_twinshot_retains_two_separate_transients(self):
        for key in ("twinshotrelease", "twinshothit"):
            signal = bank.render(key)
            energy = lambda a, b: float(np.mean(signal[round(a * bank.RATE):round(b * bank.RATE)] ** 2))
            with self.subTest(key=key):
                first = energy(.005, .030)
                gap = energy(.063, .083)
                second = energy(.088, .113)
                self.assertGreater(first, gap * 5)
                self.assertGreater(second, gap * 4)
                self.assertGreater(second / first, .45)
                self.assertLess(second / first, 1.1)

    def test_preparation_is_quieter_than_the_contact(self):
        for first, second in (("smokeprimer", "smokeburst"), ("bowdraw", "broadheadhit"), ("shieldbrace", "shieldbashhit")):
            self.assertLess(bank.metrics(first)["rms_dbfs"], bank.metrics(second)["rms_dbfs"] - 5)


if __name__ == "__main__":
    unittest.main()
