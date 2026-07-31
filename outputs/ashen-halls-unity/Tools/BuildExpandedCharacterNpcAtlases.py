#!/usr/bin/env python3
"""Assemble the v1.93 expanded player and completed Midgaard NPC atlases."""

from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path

from PIL import Image, ImageChops


CELL_SIZE = 256
ALPHA_THRESHOLD = 24
MAX_SPRITE_WIDTH = 220
MAX_SPRITE_HEIGHT = 218
TARGET_BASELINE = 238
REQUIRED_GUTTER = 18

PLAYER_COLUMNS = 5
PLAYER_ROWS = 7
PLAYER_VARIANT_COLUMNS = 5
PLAYER_VARIANT_ROWS = 4
NPC_COLUMNS = 5
NPC_ROWS = 4
NPC_SOURCE_COLUMNS = 2
NPC_SOURCE_ROWS = 2

# The expanded atlas is organized by visual class rows and race columns:
# human, dusk elf, stoneborn, fenkin, ashling.
# Tuples are (source family, source cell).
PLAYER_CELL_SOURCES: tuple[tuple[tuple[str, int], ...], ...] = (
    (("base", 0), ("base", 15), ("base", 1), ("variant", 0), ("variant", 1)),
    (("base", 2), ("base", 3), ("variant", 2), ("base", 14), ("variant", 3)),
    (("base", 4), ("base", 5), ("variant", 4), ("variant", 5), ("variant", 6)),
    (("base", 7), ("variant", 7), ("variant", 8), ("base", 6), ("variant", 9)),
    (("base", 8), ("base", 9), ("variant", 10), ("variant", 11), ("variant", 12)),
    (("base", 10), ("variant", 13), ("variant", 14), ("variant", 15), ("base", 11)),
    (("base", 12), ("variant", 16), ("base", 13), ("variant", 17), ("variant", 18)),
)

# New source quartet order: Kate, Lute, dock worker, scholar.
NPC_REPLACEMENTS: dict[int, int] = {10: 0, 11: 1, 14: 2, 19: 3}


def alpha_bbox(image: Image.Image) -> tuple[int, int, int, int] | None:
    alpha = image.getchannel("A")
    return alpha.point(
        lambda value: 255 if value > ALPHA_THRESHOLD else 0
    ).getbbox()


def transparent_grid_boundaries(
    image: Image.Image, columns: int, rows: int
) -> tuple[list[int], list[int]]:
    """Find interior grid cuts in transparent lanes near the nominal grid."""

    mask = image.getchannel("A").point(
        lambda value: 255 if value > ALPHA_THRESHOLD else 0
    )

    def boundaries_for_axis(length: int, count: int, horizontal: bool) -> list[int]:
        if count <= 1:
            return [0, length]

        blank: list[bool] = []
        for coordinate in range(length):
            strip = (
                mask.crop((0, coordinate, image.width, coordinate + 1))
                if horizontal
                else mask.crop((coordinate, 0, coordinate + 1, image.height))
            )
            blank.append(strip.getbbox() is None)

        boundaries = [0]
        nominal_cell = length / count
        window = max(8, round(nominal_cell * 0.35))
        for boundary_index in range(1, count):
            expected = boundary_index * nominal_cell
            start = max(1, round(expected) - window)
            end = min(length - 1, round(expected) + window)
            runs: list[tuple[int, int]] = []
            run_start: int | None = None
            for coordinate in range(start, end + 1):
                if blank[coordinate] and run_start is None:
                    run_start = coordinate
                elif not blank[coordinate] and run_start is not None:
                    runs.append((run_start, coordinate))
                    run_start = None
            if run_start is not None:
                runs.append((run_start, end + 1))
            runs = [run for run in runs if run[1] - run[0] >= 2]
            if not runs:
                axis = "row" if horizontal else "column"
                raise ValueError(
                    f"No transparent {axis} separator near grid boundary "
                    f"{boundary_index}"
                )
            selected = min(
                runs,
                key=lambda run: abs(((run[0] + run[1]) / 2) - expected),
            )
            boundaries.append(round((selected[0] + selected[1]) / 2))

        boundaries.append(length)
        if any(right <= left for left, right in zip(boundaries, boundaries[1:])):
            raise ValueError(f"Invalid transparent grid boundaries: {boundaries}")
        return boundaries

    return (
        boundaries_for_axis(image.width, columns, horizontal=False),
        boundaries_for_axis(image.height, rows, horizontal=True),
    )


def segmented_cell(
    image: Image.Image,
    columns: int,
    index: int,
    x_boundaries: list[int],
    y_boundaries: list[int],
) -> Image.Image:
    column = index % columns
    row = index // columns
    left = x_boundaries[column]
    right = x_boundaries[column + 1]
    top = y_boundaries[row]
    bottom = y_boundaries[row + 1]
    return image.crop((left, top, right, bottom))


def fixed_cell(image: Image.Image, columns: int, index: int) -> Image.Image:
    column = index % columns
    row = index // columns
    left = column * CELL_SIZE
    top = row * CELL_SIZE
    return image.crop((left, top, left + CELL_SIZE, top + CELL_SIZE))


def normalize_sprite(
    source: Image.Image, source_index: int
) -> tuple[Image.Image, dict[str, object]]:
    source_bounds = alpha_bbox(source)
    if source_bounds is None:
        raise ValueError(f"Source cell {source_index} is empty")

    sprite = source.crop(source_bounds)
    scale = min(
        1.0,
        MAX_SPRITE_WIDTH / max(1, sprite.width),
        MAX_SPRITE_HEIGHT / max(1, sprite.height),
    )
    width = max(1, round(sprite.width * scale))
    height = max(1, round(sprite.height * scale))
    if sprite.size != (width, height):
        sprite = sprite.resize((width, height), Image.Resampling.LANCZOS)

    x = (CELL_SIZE - width) // 2
    y = TARGET_BASELINE - height
    if x < REQUIRED_GUTTER or y < REQUIRED_GUTTER:
        raise ValueError(
            f"Source cell {source_index} cannot satisfy the required gutter"
        )

    cell = Image.new("RGBA", (CELL_SIZE, CELL_SIZE), (0, 0, 0, 0))
    cell.alpha_composite(sprite, (x, y))
    bounds = alpha_bbox(cell)
    if bounds is None:
        raise ValueError(f"Normalized source cell {source_index} is empty")

    gutters = {
        "left": bounds[0],
        "top": bounds[1],
        "right": CELL_SIZE - bounds[2],
        "bottom": CELL_SIZE - bounds[3],
    }
    if min(gutters.values()) < REQUIRED_GUTTER:
        raise ValueError(
            f"Source cell {source_index} violates gutter contract: {gutters}"
        )

    alpha = cell.getchannel("A")
    visible = sum(alpha.histogram()[ALPHA_THRESHOLD + 1 :])
    return cell, {
        "source_index": source_index,
        "source_bbox": list(source_bounds),
        "scale": round(scale, 6),
        "runtime_bbox": list(bounds),
        "runtime_size": [width, height],
        "gutters": gutters,
        "visible_fraction": round(visible / (CELL_SIZE * CELL_SIZE), 6),
    }


def paste_cell(atlas: Image.Image, cell: Image.Image, index: int, columns: int) -> None:
    left = (index % columns) * CELL_SIZE
    top = (index // columns) * CELL_SIZE
    atlas.paste((0, 0, 0, 0), (left, top, left + CELL_SIZE, top + CELL_SIZE))
    atlas.alpha_composite(cell, (left, top))


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as stream:
        for chunk in iter(lambda: stream.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest().upper()


def require_new_or_forced(path: Path, force: bool) -> None:
    if path.exists() and not force:
        raise FileExistsError(f"Refusing to overwrite existing output: {path}")


def validate_transparent_corners(image: Image.Image, label: str) -> None:
    corners = (
        image.getpixel((0, 0))[3],
        image.getpixel((image.width - 1, 0))[3],
        image.getpixel((0, image.height - 1))[3],
        image.getpixel((image.width - 1, image.height - 1))[3],
    )
    if any(corners):
        raise ValueError(f"{label} does not have transparent corners: {corners}")


def main() -> int:
    parser = argparse.ArgumentParser(
        description=(
            "Build the 5x7 player atlas and replace four reserved cells in the "
            "5x4 Midgaard NPC atlas."
        )
    )
    parser.add_argument("--player-base", required=True, type=Path)
    parser.add_argument("--player-variants", required=True, type=Path)
    parser.add_argument("--player-output", required=True, type=Path)
    parser.add_argument("--npc-base", required=True, type=Path)
    parser.add_argument("--npc-contacts", required=True, type=Path)
    parser.add_argument("--npc-output", required=True, type=Path)
    parser.add_argument("--report", required=True, type=Path)
    parser.add_argument("--force", action="store_true")
    args = parser.parse_args()

    for output in (args.player_output, args.npc_output, args.report):
        require_new_or_forced(output, args.force)

    with Image.open(args.player_base) as opened:
        player_base = opened.convert("RGBA")
    with Image.open(args.player_variants) as opened:
        player_variants = opened.convert("RGBA")
    with Image.open(args.npc_base) as opened:
        npc_base = opened.convert("RGBA")
    with Image.open(args.npc_contacts) as opened:
        npc_contacts = opened.convert("RGBA")

    if player_base.size != (CELL_SIZE * 4, CELL_SIZE * 4):
        raise ValueError(f"Unexpected player base dimensions: {player_base.size}")
    if npc_base.size != (CELL_SIZE * NPC_COLUMNS, CELL_SIZE * NPC_ROWS):
        raise ValueError(f"Unexpected NPC base dimensions: {npc_base.size}")

    validate_transparent_corners(player_variants, "Player variant source")
    validate_transparent_corners(npc_contacts, "NPC contact source")

    player_variant_x, player_variant_y = transparent_grid_boundaries(
        player_variants, PLAYER_VARIANT_COLUMNS, PLAYER_VARIANT_ROWS
    )
    npc_contact_x, npc_contact_y = transparent_grid_boundaries(
        npc_contacts, NPC_SOURCE_COLUMNS, NPC_SOURCE_ROWS
    )

    unused_variant = segmented_cell(
        player_variants,
        PLAYER_VARIANT_COLUMNS,
        19,
        player_variant_x,
        player_variant_y,
    )
    if alpha_bbox(unused_variant) is not None:
        raise ValueError("Player variant source cell 19 must remain empty")

    player_output = Image.new(
        "RGBA",
        (CELL_SIZE * PLAYER_COLUMNS, CELL_SIZE * PLAYER_ROWS),
        (0, 0, 0, 0),
    )
    player_cells: list[dict[str, object]] = []
    copied_player_cells: list[dict[str, int]] = []
    for row, sources in enumerate(PLAYER_CELL_SOURCES):
        for column, (family, source_index) in enumerate(sources):
            destination_index = row * PLAYER_COLUMNS + column
            if family == "base":
                cell = fixed_cell(player_base, 4, source_index)
                details: dict[str, object] = {
                    "destination_index": destination_index,
                    "source_family": family,
                    "source_index": source_index,
                    "pixel_identical_copy": True,
                }
                copied_player_cells.append(
                    {
                        "source_index": source_index,
                        "destination_index": destination_index,
                    }
                )
            else:
                source = segmented_cell(
                    player_variants,
                    PLAYER_VARIANT_COLUMNS,
                    source_index,
                    player_variant_x,
                    player_variant_y,
                )
                cell, details = normalize_sprite(source, source_index)
                details.update(
                    {
                        "destination_index": destination_index,
                        "source_family": family,
                    }
                )

            paste_cell(player_output, cell, destination_index, PLAYER_COLUMNS)
            player_cells.append(details)

    for copied in copied_player_cells:
        source = fixed_cell(player_base, 4, copied["source_index"])
        destination = fixed_cell(
            player_output, PLAYER_COLUMNS, copied["destination_index"]
        )
        if ImageChops.difference(source, destination).getbbox() is not None:
            raise ValueError(
                "Approved player base cell changed during assembly: "
                f"{copied['source_index']} -> {copied['destination_index']}"
            )

    npc_output = npc_base.copy()
    npc_cells: list[dict[str, object]] = []
    for destination_index, source_index in NPC_REPLACEMENTS.items():
        source = segmented_cell(
            npc_contacts,
            NPC_SOURCE_COLUMNS,
            source_index,
            npc_contact_x,
            npc_contact_y,
        )
        cell, details = normalize_sprite(source, source_index)
        details["destination_index"] = destination_index
        paste_cell(npc_output, cell, destination_index, NPC_COLUMNS)
        npc_cells.append(details)

    untouched_npc_cells = [
        index for index in range(NPC_COLUMNS * NPC_ROWS) if index not in NPC_REPLACEMENTS
    ]
    for index in untouched_npc_cells:
        if ImageChops.difference(
            fixed_cell(npc_base, NPC_COLUMNS, index),
            fixed_cell(npc_output, NPC_COLUMNS, index),
        ).getbbox() is not None:
            raise ValueError(f"Untouched NPC cell {index} changed during assembly")

    for atlas, columns, rows, label in (
        (player_output, PLAYER_COLUMNS, PLAYER_ROWS, "player"),
        (npc_output, NPC_COLUMNS, NPC_ROWS, "NPC"),
    ):
        validate_transparent_corners(atlas, label)
        for index in range(columns * rows):
            cell = fixed_cell(atlas, columns, index)
            bounds = alpha_bbox(cell)
            if bounds is None:
                raise ValueError(f"{label} runtime cell {index} is empty")
            gutters = (
                bounds[0],
                bounds[1],
                CELL_SIZE - bounds[2],
                CELL_SIZE - bounds[3],
            )
            if min(gutters) < REQUIRED_GUTTER:
                raise ValueError(
                    f"{label} runtime cell {index} violates gutter contract: {gutters}"
                )

    args.player_output.parent.mkdir(parents=True, exist_ok=True)
    args.npc_output.parent.mkdir(parents=True, exist_ok=True)
    args.report.parent.mkdir(parents=True, exist_ok=True)
    player_output.save(args.player_output, optimize=True)
    npc_output.save(args.npc_output, optimize=True)

    report = {
        "player": {
            "base": str(args.player_base),
            "variant_source": str(args.player_variants),
            "output": str(args.player_output),
            "dimensions": list(player_output.size),
            "grid": [PLAYER_COLUMNS, PLAYER_ROWS],
            "cell_size": CELL_SIZE,
            "race_columns": [
                "human",
                "dusk elf",
                "stoneborn",
                "fenkin",
                "ashling",
            ],
            "class_rows": [
                "warrior",
                "rogue",
                "ranger",
                "priest",
                "warlock",
                "wizard or mage",
                "paladin",
            ],
            "source_grid_boundaries": {
                "x": player_variant_x,
                "y": player_variant_y,
            },
            "cells": player_cells,
            "approved_base_cells_pixel_identical": True,
            "approved_base_cell_count": len(copied_player_cells),
            "sha256": sha256(args.player_output),
        },
        "npc": {
            "base": str(args.npc_base),
            "contact_source": str(args.npc_contacts),
            "output": str(args.npc_output),
            "dimensions": list(npc_output.size),
            "grid": [NPC_COLUMNS, NPC_ROWS],
            "cell_size": CELL_SIZE,
            "mapping": {
                "Kate": 10,
                "Lute": 11,
                "DockWorker": 14,
                "Scholar": 19,
            },
            "source_grid_boundaries": {
                "x": npc_contact_x,
                "y": npc_contact_y,
            },
            "replacements": npc_cells,
            "untouched_cells_pixel_identical": True,
            "untouched_cell_count": len(untouched_npc_cells),
            "sha256": sha256(args.npc_output),
        },
        "normalization": {
            "alpha_threshold": ALPHA_THRESHOLD,
            "max_sprite_size": [MAX_SPRITE_WIDTH, MAX_SPRITE_HEIGHT],
            "target_baseline": TARGET_BASELINE,
            "required_gutter": REQUIRED_GUTTER,
        },
    }
    args.report.write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")

    print(f"Wrote {args.player_output}")
    print(f"Wrote {args.npc_output}")
    print(f"Wrote {args.report}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
