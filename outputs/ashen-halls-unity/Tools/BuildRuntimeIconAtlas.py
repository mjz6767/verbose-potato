#!/usr/bin/env python3
"""Normalize generated icon sheets into deterministic runtime atlases."""

from __future__ import annotations

import argparse
import json
from dataclasses import dataclass
from pathlib import Path

from PIL import Image


@dataclass(frozen=True)
class CellSource:
    path: Path
    columns: int
    rows: int
    index: int


def parse_replacement(value: str) -> tuple[int, CellSource]:
    parts = value.split("|")
    if len(parts) != 5:
        raise argparse.ArgumentTypeError(
            "replacement must be TARGET|PATH|COLUMNS|ROWS|SOURCE_INDEX"
        )
    target, path, columns, rows, source_index = parts
    return int(target), CellSource(
        Path(path), int(columns), int(rows), int(source_index)
    )


def cell_bounds(image: Image.Image, columns: int, rows: int, index: int) -> tuple[int, int, int, int]:
    if index < 0 or index >= columns * rows:
        raise ValueError(f"cell index {index} is outside {columns}x{rows}")
    column = index % columns
    row = index // columns
    left = round(column * image.width / columns)
    right = round((column + 1) * image.width / columns)
    top = round(row * image.height / rows)
    bottom = round((row + 1) * image.height / rows)
    return left, top, right, bottom


def load_cell(source: CellSource, alpha_threshold: int) -> tuple[Image.Image, tuple[int, int, int, int]]:
    with Image.open(source.path) as opened:
        image = opened.convert("RGBA")
    crop = image.crop(cell_bounds(image, source.columns, source.rows, source.index))
    alpha = crop.getchannel("A")
    mask = alpha.point(lambda value: 255 if value >= alpha_threshold else 0)
    bounds = mask.getbbox()
    if bounds is None:
        raise ValueError(f"cell {source.index} in {source.path} has no visible pixels")
    return crop.crop(bounds), bounds


def trim_region(
    image: Image.Image,
    region: tuple[int, int, int, int],
    alpha_threshold: int,
) -> tuple[Image.Image, tuple[int, int, int, int]]:
    crop = image.crop(region)
    alpha = crop.getchannel("A")
    mask = alpha.point(lambda value: 255 if value >= alpha_threshold else 0)
    bounds = mask.getbbox()
    if bounds is None:
        raise ValueError(f"region {region} has no visible pixels")
    return crop.crop(bounds), bounds


def valley_boundaries(projection: list[int], segments: int, search_fraction: float) -> list[int]:
    length = len(projection)
    step = length / segments
    radius = max(2, round(step * search_fraction))
    boundaries = [0]
    for boundary_index in range(1, segments):
        expected = round(step * boundary_index)
        start = max(boundaries[-1] + 1, expected - radius)
        end = min(length - 1, expected + radius)

        def score(position: int) -> tuple[int, int]:
            window_start = max(0, position - 2)
            window_end = min(length, position + 3)
            return sum(projection[window_start:window_end]), abs(position - expected)

        boundaries.append(min(range(start, end + 1), key=score))
    boundaries.append(length)
    return boundaries


def adaptive_regions(
    image: Image.Image,
    columns: int,
    rows: int,
    alpha_threshold: int,
) -> list[tuple[int, int, int, int]]:
    alpha = image.getchannel("A")
    pixels = alpha.load()
    row_projection = [
        sum(1 for x in range(image.width) if pixels[x, y] >= alpha_threshold)
        for y in range(image.height)
    ]
    row_bounds = valley_boundaries(row_projection, rows, 0.38)
    regions: list[tuple[int, int, int, int]] = []
    for row in range(rows):
        top = row_bounds[row]
        bottom = row_bounds[row + 1]
        column_projection = [
            sum(1 for y in range(top, bottom) if pixels[x, y] >= alpha_threshold)
            for x in range(image.width)
        ]
        column_bounds = valley_boundaries(column_projection, columns, 0.42)
        for column in range(columns):
            regions.append(
                (
                    column_bounds[column],
                    top,
                    column_bounds[column + 1],
                    bottom,
                )
            )
    return regions


def fit_icon(icon: Image.Image, maximum: int) -> Image.Image:
    scale = min(maximum / icon.width, maximum / icon.height, 1.0)
    width = max(1, round(icon.width * scale))
    height = max(1, round(icon.height * scale))
    if (width, height) == icon.size:
        return icon
    return icon.resize((width, height), Image.Resampling.LANCZOS)


def main() -> int:
    parser = argparse.ArgumentParser(
        description="Extract, trim, center, and validate cells from a generated icon sheet."
    )
    parser.add_argument("--source", required=True, type=Path)
    parser.add_argument("--output", required=True, type=Path)
    parser.add_argument("--columns", required=True, type=int)
    parser.add_argument("--rows", required=True, type=int)
    parser.add_argument("--cell-size", type=int, default=256)
    parser.add_argument("--padding", type=int, default=16)
    parser.add_argument("--alpha-threshold", type=int, default=8)
    parser.add_argument(
        "--adaptive-grid",
        action="store_true",
        help="Find low-occupancy gutters instead of assuming perfectly even generated cells.",
    )
    parser.add_argument(
        "--allow-empty",
        action="store_true",
        help="Preserve deliberately empty source cells instead of failing the atlas build.",
    )
    parser.add_argument(
        "--replace",
        action="append",
        default=[],
        type=parse_replacement,
        metavar="TARGET|PATH|COLUMNS|ROWS|SOURCE_INDEX",
    )
    parser.add_argument("--report", type=Path)
    args = parser.parse_args()

    if args.columns < 1 or args.rows < 1:
        parser.error("columns and rows must be positive")
    if args.cell_size <= args.padding * 2:
        parser.error("cell size must be larger than twice the padding")

    replacements = dict(args.replace)
    default_source = CellSource(args.source, args.columns, args.rows, 0)
    with Image.open(args.source) as opened:
        default_image = opened.convert("RGBA")
    source_regions = (
        adaptive_regions(
            default_image,
            args.columns,
            args.rows,
            args.alpha_threshold,
        )
        if args.adaptive_grid
        else [
            cell_bounds(default_image, args.columns, args.rows, index)
            for index in range(args.columns * args.rows)
        ]
    )
    atlas = Image.new(
        "RGBA",
        (args.columns * args.cell_size, args.rows * args.cell_size),
        (0, 0, 0, 0),
    )
    maximum = args.cell_size - args.padding * 2
    report_cells: list[dict[str, object]] = []

    for index in range(args.columns * args.rows):
        source = replacements.get(index)
        try:
            if source is None:
                source = CellSource(default_source.path, args.columns, args.rows, index)
                source_region = source_regions[index]
                icon, source_bounds = trim_region(
                    default_image,
                    source_region,
                    args.alpha_threshold,
                )
            else:
                with Image.open(source.path) as replacement_image:
                    source_region = cell_bounds(
                        replacement_image, source.columns, source.rows, source.index
                    )
                icon, source_bounds = load_cell(source, args.alpha_threshold)
        except ValueError:
            if not args.allow_empty:
                raise
            report_cells.append(
                {
                    "index": index,
                    "source": str(source.path if source is not None else default_source.path),
                    "sourceIndex": source.index if source is not None else index,
                    "sourceRegion": source_region,
                    "sourceBounds": None,
                    "runtimeBounds": None,
                    "size": [0, 0],
                    "empty": True,
                }
            )
            continue
        icon = fit_icon(icon, maximum)
        column = index % args.columns
        row = index // args.columns
        x = column * args.cell_size + (args.cell_size - icon.width) // 2
        y = row * args.cell_size + (args.cell_size - icon.height) // 2
        atlas.alpha_composite(icon, (x, y))
        report_cells.append(
            {
                "index": index,
                "source": str(source.path),
                "sourceIndex": source.index,
                "sourceRegion": source_region,
                "sourceBounds": source_bounds,
                "runtimeBounds": [x, y, x + icon.width, y + icon.height],
                "size": [icon.width, icon.height],
            }
        )

    args.output.parent.mkdir(parents=True, exist_ok=True)
    atlas.save(args.output, optimize=True)

    report = {
        "source": str(args.source),
        "output": str(args.output),
        "grid": [args.columns, args.rows],
        "cellSize": args.cell_size,
        "padding": args.padding,
        "atlasSize": list(atlas.size),
        "cells": report_cells,
    }
    report_path = args.report or args.output.with_suffix(".json")
    report_path.write_text(json.dumps(report, indent=2), encoding="utf-8")
    print(json.dumps({"output": str(args.output), "size": atlas.size, "cells": len(report_cells)}))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
