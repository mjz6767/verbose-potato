#!/usr/bin/env python3
"""Append the v2.9 early-progression skill and spell icons to approved atlases."""

from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path

from PIL import Image


CELL_SIZE = 256
PADDING = 18
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


def visible_bounds(image: Image.Image, label: str) -> tuple[int, int, int, int]:
    alpha = image.getchannel("A")
    bounds = alpha.point(lambda value: 255 if value >= ALPHA_THRESHOLD else 0).getbbox()
    if bounds is None:
        raise ValueError(f"{label} is empty")
    return bounds


def assert_empty(image: Image.Image, label: str) -> None:
    alpha = image.getchannel("A")
    if alpha.point(lambda value: 255 if value >= ALPHA_THRESHOLD else 0).getbbox() is not None:
        raise ValueError(f"{label} must remain empty")


def fit_icon(icon: Image.Image) -> Image.Image:
    maximum = CELL_SIZE - PADDING * 2
    scale = min(maximum / icon.width, maximum / icon.height)
    size = (max(1, round(icon.width * scale)), max(1, round(icon.height * scale)))
    return icon if icon.size == size else icon.resize(size, Image.Resampling.LANCZOS)


def sha256_pixels(image: Image.Image) -> str:
    return hashlib.sha256(image.tobytes()).hexdigest()


def load_rgba(path: Path) -> Image.Image:
    with Image.open(path) as opened:
        return opened.convert("RGBA")


def append_ability_row(base: Image.Image, source: Image.Image) -> tuple[Image.Image, list[dict[str, object]]]:
    columns = 4
    base_rows = 6
    output_rows = 7
    expected = (columns * CELL_SIZE, base_rows * CELL_SIZE)
    if base.size != expected:
        raise ValueError(f"ability base atlas is {base.size}, expected {expected}")

    atlas = Image.new("RGBA", (columns * CELL_SIZE, output_rows * CELL_SIZE), (0, 0, 0, 0))
    atlas.alpha_composite(base, (0, 0))
    labels = ("Sunder", "Shadowstep", "Quick Shot")
    ids = ("sunder", "shadowstep", "quickshot")
    cells: list[dict[str, object]] = []
    fingerprints: set[str] = set()
    for source_index, (label, ability_id) in enumerate(zip(labels, ids)):
        source_region = cell_bounds(source, 2, 2, source_index)
        source_cell = source.crop(source_region)
        bounds = visible_bounds(source_cell, label)
        icon = fit_icon(source_cell.crop(bounds))
        fingerprint = sha256_pixels(icon)
        if fingerprint in fingerprints:
            raise ValueError(f"{label} duplicates another early-progression ability icon")
        fingerprints.add(fingerprint)

        atlas_index = base_rows * columns + source_index
        x = (atlas_index % columns) * CELL_SIZE + (CELL_SIZE - icon.width) // 2
        y = (atlas_index // columns) * CELL_SIZE + (CELL_SIZE - icon.height) // 2
        atlas.alpha_composite(icon, (x, y))
        cells.append(
            {
                "atlasIndex": atlas_index,
                "abilityId": ability_id,
                "sourceIndex": source_index,
                "sourceRegion": list(source_region),
                "sourceBounds": list(bounds),
                "runtimeBounds": [x, y, x + icon.width, y + icon.height],
                "runtimeSize": list(icon.size),
                "pixelSha256": fingerprint,
            }
        )

    assert_empty(source.crop(cell_bounds(source, 2, 2, 3)), "ability source reserve cell")
    assert_empty(atlas.crop(cell_bounds(atlas, columns, output_rows, 27)), "ability atlas reserve cell 27")
    if atlas.crop((0, 0, base.width, base.height)).tobytes() != base.tobytes():
        raise ValueError("existing ability atlas pixels changed while appending the progression row")
    return atlas, cells


def fill_spell_reserve(base: Image.Image, source: Image.Image) -> tuple[Image.Image, list[dict[str, object]]]:
    columns = 7
    rows = 8
    expected = (columns * CELL_SIZE, rows * CELL_SIZE)
    if base.size != expected:
        raise ValueError(f"spell base atlas is {base.size}, expected {expected}")

    for atlas_index in range(51, 56):
        assert_empty(base.crop(cell_bounds(base, columns, rows, atlas_index)), f"spell base reserve cell {atlas_index}")

    atlas = base.copy()
    labels = ("Dawn Pulse", "Cinderstorm", "Grave Hook", "Soul Veil", "Ashen Curse")
    codes = ("DWP", "CNS", "GRH", "SLV", "ACR")
    cells: list[dict[str, object]] = []
    fingerprints: set[str] = set()
    for source_index, (label, code) in enumerate(zip(labels, codes)):
        source_region = cell_bounds(source, 3, 2, source_index)
        source_cell = source.crop(source_region)
        bounds = visible_bounds(source_cell, label)
        icon = fit_icon(source_cell.crop(bounds))
        fingerprint = sha256_pixels(icon)
        if fingerprint in fingerprints:
            raise ValueError(f"{label} duplicates another early-progression spell icon")
        fingerprints.add(fingerprint)

        atlas_index = 51 + source_index
        x = (atlas_index % columns) * CELL_SIZE + (CELL_SIZE - icon.width) // 2
        y = (atlas_index // columns) * CELL_SIZE + (CELL_SIZE - icon.height) // 2
        atlas.alpha_composite(icon, (x, y))
        cells.append(
            {
                "atlasIndex": atlas_index,
                "formulaCode": code,
                "sourceIndex": source_index,
                "sourceRegion": list(source_region),
                "sourceBounds": list(bounds),
                "runtimeBounds": [x, y, x + icon.width, y + icon.height],
                "runtimeSize": list(icon.size),
                "pixelSha256": fingerprint,
            }
        )

    assert_empty(source.crop(cell_bounds(source, 3, 2, 5)), "spell source reserve cell")
    for atlas_index in range(51):
        bounds = cell_bounds(base, columns, rows, atlas_index)
        if atlas.crop(bounds).tobytes() != base.crop(bounds).tobytes():
            raise ValueError(f"existing signature spell cell {atlas_index} changed")
    return atlas, cells


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--ability-base", required=True, type=Path)
    parser.add_argument("--ability-source", required=True, type=Path)
    parser.add_argument("--ability-output", required=True, type=Path)
    parser.add_argument("--spell-base", required=True, type=Path)
    parser.add_argument("--spell-source", required=True, type=Path)
    parser.add_argument("--spell-output", required=True, type=Path)
    parser.add_argument("--report", required=True, type=Path)
    args = parser.parse_args()

    ability_base = load_rgba(args.ability_base)
    ability_source = load_rgba(args.ability_source)
    ability_atlas, ability_cells = append_ability_row(ability_base, ability_source)

    spell_base = load_rgba(args.spell_base)
    spell_source = load_rgba(args.spell_source)
    spell_atlas, spell_cells = fill_spell_reserve(spell_base, spell_source)

    args.ability_output.parent.mkdir(parents=True, exist_ok=True)
    ability_atlas.save(args.ability_output, optimize=True)
    args.spell_output.parent.mkdir(parents=True, exist_ok=True)
    spell_atlas.save(args.spell_output, optimize=True)

    report = {
        "ability": {
            "base": args.ability_base.name,
            "source": args.ability_source.name,
            "output": args.ability_output.name,
            "dimensions": list(ability_atlas.size),
            "grid": [4, 7],
            "cellSize": CELL_SIZE,
            "padding": PADDING,
            "preservedBasePixelSha256": sha256_pixels(ability_base),
            "reservedEmptyIndices": [27],
            "cells": ability_cells,
        },
        "spell": {
            "base": args.spell_base.name,
            "source": args.spell_source.name,
            "output": args.spell_output.name,
            "dimensions": list(spell_atlas.size),
            "grid": [7, 8],
            "cellSize": CELL_SIZE,
            "padding": PADDING,
            "preservedMappedCellCount": 51,
            "preservedBasePixelSha256": sha256_pixels(spell_base),
            "cells": spell_cells,
        },
    }
    args.report.parent.mkdir(parents=True, exist_ok=True)
    args.report.write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")
    print(f"Wrote {args.ability_output} with 3 appended ability icons and reserve cell 27")
    print(f"Wrote {args.spell_output} with 5 filled spell reserve cells")
    print(f"Wrote {args.report}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
