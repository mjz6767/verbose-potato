#!/usr/bin/env python3
"""Normalize transparent sprite cells onto a stable grid without pruning detail."""

from __future__ import annotations

import argparse
import json
from pathlib import Path

from PIL import Image


def alpha_bbox(image: Image.Image, threshold: int) -> tuple[int, int, int, int] | None:
    alpha = image.getchannel("A")
    return alpha.point(lambda value: 255 if value > threshold else 0).getbbox()


def main() -> int:
    parser = argparse.ArgumentParser(
        description="Center and baseline-align each cell of a transparent sprite atlas."
    )
    parser.add_argument("--input", required=True, type=Path)
    parser.add_argument("--output", required=True, type=Path)
    parser.add_argument("--columns", type=int, default=5)
    parser.add_argument("--rows", type=int, default=4)
    parser.add_argument("--max-width", type=int, default=224)
    parser.add_argument("--max-height", type=int, default=220)
    parser.add_argument("--baseline", type=int, default=258)
    parser.add_argument(
        "--vertical-align",
        choices=("baseline", "center"),
        default="baseline",
        help="Align each normalized sprite to the shared baseline or cell center.",
    )
    parser.add_argument("--alpha-threshold", type=int, default=16)
    parser.add_argument("--report", type=Path)
    args = parser.parse_args()

    with Image.open(args.input) as opened:
        source = opened.convert("RGBA")

    if source.width % args.columns or source.height % args.rows:
        raise ValueError(
            f"{source.width}x{source.height} is not divisible by "
            f"{args.columns}x{args.rows}"
        )

    cell_width = source.width // args.columns
    cell_height = source.height // args.rows
    if args.max_width > cell_width or args.max_height > cell_height:
        raise ValueError("Normalized bounds must fit within one atlas cell")
    if args.vertical_align == "baseline" and (args.baseline < 1 or args.baseline > cell_height):
        raise ValueError("Baseline must fall within one atlas cell")

    output = Image.new("RGBA", source.size, (0, 0, 0, 0))
    cells: list[dict[str, object]] = []
    for index in range(args.columns * args.rows):
        column = index % args.columns
        row = index // args.columns
        left = column * cell_width
        top = row * cell_height
        cell = source.crop((left, top, left + cell_width, top + cell_height))
        bbox = alpha_bbox(cell, args.alpha_threshold)
        if bbox is None:
            cells.append({"index": index, "empty": True})
            continue

        sprite = cell.crop(bbox)
        scale = min(
            args.max_width / max(1, sprite.width),
            args.max_height / max(1, sprite.height),
        )
        width = max(1, round(sprite.width * scale))
        height = max(1, round(sprite.height * scale))
        if (width, height) != sprite.size:
            sprite = sprite.resize((width, height), Image.Resampling.LANCZOS)

        x = left + (cell_width - width) // 2
        if args.vertical_align == "center":
            y = top + (cell_height - height) // 2
        else:
            y = top + min(args.baseline - height, cell_height - height)
            y = max(top, y)
        output.alpha_composite(sprite, (x, y))

        alpha_histogram = sprite.getchannel("A").histogram()
        visible = sum(alpha_histogram[args.alpha_threshold + 1 :])
        cells.append(
            {
                "index": index,
                "empty": False,
                "sourceBounds": list(bbox),
                "normalizedBounds": [x - left, y - top, width, height],
                "visibleFraction": round(visible / (cell_width * cell_height), 4),
            }
        )

    args.output.parent.mkdir(parents=True, exist_ok=True)
    output.save(args.output, optimize=True)

    report = {
        "file": args.output.name,
        "dimensions": [output.width, output.height],
        "grid": [args.columns, args.rows],
        "cellDimensions": [cell_width, cell_height],
        "alphaThreshold": args.alpha_threshold,
        "maxSpriteDimensions": [args.max_width, args.max_height],
        "baseline": args.baseline,
        "verticalAlignment": args.vertical_align,
        "cells": cells,
    }
    if args.report:
        args.report.parent.mkdir(parents=True, exist_ok=True)
        args.report.write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")

    print(f"Wrote {args.output}")
    print(f"Normalized {sum(not cell['empty'] for cell in cells)} cells")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
