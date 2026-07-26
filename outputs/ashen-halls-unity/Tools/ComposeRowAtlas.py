#!/usr/bin/env python3
"""Compose horizontal sprite-source rows into a normalized transparent atlas."""

from __future__ import annotations

import argparse
import json
from pathlib import Path

from PIL import Image


def alpha_bbox(image: Image.Image, threshold: int) -> tuple[int, int, int, int] | None:
    alpha = image.getchannel("A")
    return alpha.point(lambda value: 255 if value >= threshold else 0).getbbox()


def main() -> int:
    parser = argparse.ArgumentParser(
        description="Split each transparent source row into equal slots and normalize the sprites."
    )
    parser.add_argument("--input", action="append", required=True, type=Path)
    parser.add_argument("--output", required=True, type=Path)
    parser.add_argument("--columns", type=int, default=5)
    parser.add_argument("--cell-size", type=int, default=280)
    parser.add_argument("--max-width", type=int, default=236)
    parser.add_argument("--max-height", type=int, default=220)
    parser.add_argument("--baseline", type=int, default=262)
    parser.add_argument("--alpha-threshold", type=int, default=8)
    parser.add_argument("--report", type=Path)
    args = parser.parse_args()

    if args.columns <= 0 or not args.input:
        raise ValueError("At least one source row and one column are required")
    if args.max_width > args.cell_size or args.max_height > args.cell_size:
        raise ValueError("Normalized sprite bounds must fit within one atlas cell")
    if args.baseline <= 0 or args.baseline > args.cell_size:
        raise ValueError("Baseline must fall inside the atlas cell")

    rows: list[Image.Image] = []
    for path in args.input:
        with Image.open(path) as opened:
            rows.append(opened.convert("RGBA"))

    output = Image.new(
        "RGBA",
        (args.columns * args.cell_size, len(rows) * args.cell_size),
        (0, 0, 0, 0),
    )
    report_cells: list[dict[str, object]] = []

    for row_index, source in enumerate(rows):
        for column in range(args.columns):
            slot_left = round(column * source.width / args.columns)
            slot_right = round((column + 1) * source.width / args.columns)
            slot = source.crop((slot_left, 0, slot_right, source.height))
            bbox = alpha_bbox(slot, args.alpha_threshold)
            index = row_index * args.columns + column
            if bbox is None:
                raise ValueError(f"Source row {row_index} slot {column} is empty")

            sprite = slot.crop(bbox)
            scale = min(
                args.max_width / max(1, sprite.width),
                args.max_height / max(1, sprite.height),
            )
            width = max(1, round(sprite.width * scale))
            height = max(1, round(sprite.height * scale))
            if (width, height) != sprite.size:
                sprite = sprite.resize((width, height), Image.Resampling.LANCZOS)

            cell_left = column * args.cell_size
            cell_top = row_index * args.cell_size
            x = cell_left + (args.cell_size - width) // 2
            y = cell_top + args.baseline - height
            output.alpha_composite(sprite, (x, y))

            visible = sum(
                1
                for alpha in sprite.getchannel("A").getdata()
                if alpha >= args.alpha_threshold
            )
            report_cells.append(
                {
                    "index": index,
                    "source": args.input[row_index].name,
                    "sourceSlot": [slot_left, 0, slot_right - slot_left, source.height],
                    "sourceBounds": list(bbox),
                    "normalizedBounds": [
                        x - cell_left,
                        y - cell_top,
                        width,
                        height,
                    ],
                    "visibleFraction": round(
                        visible / (args.cell_size * args.cell_size), 4
                    ),
                }
            )

    args.output.parent.mkdir(parents=True, exist_ok=True)
    output.save(args.output, optimize=True)

    if args.report:
        report = {
            "file": args.output.name,
            "dimensions": list(output.size),
            "grid": [args.columns, len(rows)],
            "cellDimensions": [args.cell_size, args.cell_size],
            "alphaThreshold": args.alpha_threshold,
            "maxSpriteDimensions": [args.max_width, args.max_height],
            "baseline": args.baseline,
            "cells": report_cells,
        }
        args.report.parent.mkdir(parents=True, exist_ok=True)
        args.report.write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")

    print(f"Wrote {args.output}")
    print(f"Composed {len(report_cells)} normalized cells")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
