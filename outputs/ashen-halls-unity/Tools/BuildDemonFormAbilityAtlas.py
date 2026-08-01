#!/usr/bin/env python3
"""Append four generated demon-form icons to the approved combat ability atlas."""

from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path

from PIL import Image


COLUMNS = 4
BASE_ROWS = 5
OUTPUT_ROWS = 6
CELL_SIZE = 256
PADDING = 16
ALPHA_THRESHOLD = 8


def cell_bounds(image: Image.Image, columns: int, rows: int, index: int) -> tuple[int, int, int, int]:
    column = index % columns
    row = index // columns
    return (
        round(column * image.width / columns),
        round(row * image.height / rows),
        round((column + 1) * image.width / columns),
        round((row + 1) * image.height / rows),
    )


def visible_bounds(image: Image.Image) -> tuple[int, int, int, int]:
    alpha = image.getchannel("A")
    bounds = alpha.point(lambda value: 255 if value >= ALPHA_THRESHOLD else 0).getbbox()
    if bounds is None:
        raise ValueError("generated demon-form icon cell is empty")
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
    parser.add_argument("--demon-source", required=True, type=Path)
    parser.add_argument("--output", required=True, type=Path)
    parser.add_argument("--report", required=True, type=Path)
    args = parser.parse_args()

    with Image.open(args.base) as opened:
        base = opened.convert("RGBA")
    expected_base = (COLUMNS * CELL_SIZE, BASE_ROWS * CELL_SIZE)
    if base.size != expected_base:
        raise ValueError(f"base atlas is {base.size}, expected {expected_base}")

    with Image.open(args.demon_source) as opened:
        demon_source = opened.convert("RGBA")

    atlas = Image.new(
        "RGBA",
        (COLUMNS * CELL_SIZE, OUTPUT_ROWS * CELL_SIZE),
        (0, 0, 0, 0),
    )
    atlas.alpha_composite(base, (0, 0))

    cells: list[dict[str, object]] = []
    fingerprints: set[str] = set()
    for source_index in range(COLUMNS):
        source_region = cell_bounds(demon_source, 2, 2, source_index)
        source_cell = demon_source.crop(source_region)
        bounds = visible_bounds(source_cell)
        icon = fit_icon(source_cell.crop(bounds))
        fingerprint = sha256_pixels(icon)
        if fingerprint in fingerprints:
            raise ValueError(f"demon-form icon {source_index} duplicates another icon")
        fingerprints.add(fingerprint)

        x = source_index * CELL_SIZE + (CELL_SIZE - icon.width) // 2
        y = BASE_ROWS * CELL_SIZE + (CELL_SIZE - icon.height) // 2
        atlas.alpha_composite(icon, (x, y))
        cells.append(
            {
                "atlasIndex": BASE_ROWS * COLUMNS + source_index,
                "sourceIndex": source_index,
                "sourceRegion": list(source_region),
                "sourceBounds": list(bounds),
                "runtimeBounds": [x, y, x + icon.width, y + icon.height],
                "runtimeSize": list(icon.size),
                "pixelSha256": fingerprint,
            }
        )

    if atlas.crop((0, 0, base.width, base.height)).tobytes() != base.tobytes():
        raise ValueError("existing ability atlas pixels changed while appending demon row")

    args.output.parent.mkdir(parents=True, exist_ok=True)
    atlas.save(args.output, optimize=True)
    report = {
        "base": args.base.name,
        "demonSource": args.demon_source.name,
        "output": args.output.name,
        "dimensions": list(atlas.size),
        "grid": [COLUMNS, OUTPUT_ROWS],
        "cellSize": CELL_SIZE,
        "padding": PADDING,
        "preservedBasePixelSha256": sha256_pixels(base),
        "cells": cells,
    }
    args.report.parent.mkdir(parents=True, exist_ok=True)
    args.report.write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")
    print(f"Wrote {args.output}")
    print(f"Preserved {BASE_ROWS * COLUMNS} existing cells and appended {len(cells)} demon-form cells")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
