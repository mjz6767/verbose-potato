#!/usr/bin/env python3
"""Append Rift Bolt and Rift Step to the approved signature spell atlas."""

from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path

from PIL import Image


COLUMNS = 7
BASE_ROWS = 7
OUTPUT_ROWS = 8
CELL_SIZE = 256
PADDING = 16
ALPHA_THRESHOLD = 8


def visible_bounds(image: Image.Image) -> tuple[int, int, int, int]:
    alpha = image.getchannel("A")
    bounds = alpha.point(lambda value: 255 if value >= ALPHA_THRESHOLD else 0).getbbox()
    if bounds is None:
        raise ValueError("generated pact spell icon is empty")
    return bounds


def fit_icon(icon: Image.Image) -> Image.Image:
    maximum = CELL_SIZE - PADDING * 2
    scale = min(maximum / icon.width, maximum / icon.height)
    size = (max(1, round(icon.width * scale)), max(1, round(icon.height * scale)))
    return icon if icon.size == size else icon.resize(size, Image.Resampling.LANCZOS)


def sha256_pixels(image: Image.Image) -> str:
    return hashlib.sha256(image.tobytes()).hexdigest()


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--base", required=True, type=Path)
    parser.add_argument("--pact-source", required=True, type=Path)
    parser.add_argument("--output", required=True, type=Path)
    parser.add_argument("--report", required=True, type=Path)
    args = parser.parse_args()

    with Image.open(args.base) as opened:
        base = opened.convert("RGBA")
    expected_base = (COLUMNS * CELL_SIZE, BASE_ROWS * CELL_SIZE)
    if base.size != expected_base:
        raise ValueError(f"base atlas is {base.size}, expected {expected_base}")
    with Image.open(args.pact_source) as opened:
        source = opened.convert("RGBA")

    atlas = Image.new(
        "RGBA",
        (COLUMNS * CELL_SIZE, OUTPUT_ROWS * CELL_SIZE),
        (0, 0, 0, 0),
    )
    atlas.alpha_composite(base, (0, 0))

    cells: list[dict[str, object]] = []
    fingerprints: set[str] = set()
    for source_index in range(2):
        left = round(source_index * source.width / 2)
        right = round((source_index + 1) * source.width / 2)
        source_region = (left, 0, right, source.height)
        source_cell = source.crop(source_region)
        bounds = visible_bounds(source_cell)
        icon = fit_icon(source_cell.crop(bounds))
        fingerprint = sha256_pixels(icon)
        if fingerprint in fingerprints:
            raise ValueError("new pact spell icons must be visually distinct")
        fingerprints.add(fingerprint)

        atlas_index = BASE_ROWS * COLUMNS + source_index
        x = source_index * CELL_SIZE + (CELL_SIZE - icon.width) // 2
        y = BASE_ROWS * CELL_SIZE + (CELL_SIZE - icon.height) // 2
        atlas.alpha_composite(icon, (x, y))
        cells.append(
            {
                "atlasIndex": atlas_index,
                "formulaCode": "RBT" if source_index == 0 else "VRS",
                "sourceIndex": source_index,
                "sourceRegion": list(source_region),
                "sourceBounds": list(bounds),
                "runtimeBounds": [x, y, x + icon.width, y + icon.height],
                "runtimeSize": list(icon.size),
                "pixelSha256": fingerprint,
            }
        )

    if atlas.crop((0, 0, base.width, base.height)).tobytes() != base.tobytes():
        raise ValueError("existing signature spell atlas pixels changed while appending pact row")

    args.output.parent.mkdir(parents=True, exist_ok=True)
    atlas.save(args.output, optimize=True)
    report = {
        "base": args.base.name,
        "pactSource": args.pact_source.name,
        "output": args.output.name,
        "dimensions": list(atlas.size),
        "grid": [COLUMNS, OUTPUT_ROWS],
        "cellSize": CELL_SIZE,
        "padding": PADDING,
        "preservedBasePixelSha256": sha256_pixels(base),
        "cells": cells,
        "reservedEmptyIndices": [51, 52, 53, 54, 55],
    }
    args.report.parent.mkdir(parents=True, exist_ok=True)
    args.report.write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")
    print(f"Wrote {args.output}")
    print("Preserved 49 existing cells, appended 2 pact spells, and reserved 5 cells")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
