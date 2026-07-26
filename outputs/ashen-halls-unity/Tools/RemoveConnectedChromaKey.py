#!/usr/bin/env python3
"""Remove only border-connected chroma pixels so violet icon art survives."""

from __future__ import annotations

import argparse
from collections import deque
from pathlib import Path

from PIL import Image


def is_magenta_key(red: int, green: int, blue: int) -> bool:
    return (
        red >= 185
        and blue >= 180
        and green <= 72
        and red - green >= 135
        and blue - green >= 130
        and abs(red - blue) <= 34
    )


def main() -> int:
    parser = argparse.ArgumentParser(
        description="Remove border-connected magenta while preserving isolated violet art."
    )
    parser.add_argument("--input", required=True, type=Path)
    parser.add_argument("--output", required=True, type=Path)
    args = parser.parse_args()

    with Image.open(args.input) as opened:
        image = opened.convert("RGBA")
    width, height = image.size
    pixels = image.load()
    visited = bytearray(width * height)
    queue: deque[tuple[int, int]] = deque()

    def try_seed(x: int, y: int) -> None:
        offset = y * width + x
        if visited[offset]:
            return
        red, green, blue, _ = pixels[x, y]
        if not is_magenta_key(red, green, blue):
            return
        visited[offset] = 1
        queue.append((x, y))

    for x in range(width):
        try_seed(x, 0)
        try_seed(x, height - 1)
    for y in range(height):
        try_seed(0, y)
        try_seed(width - 1, y)

    removed = 0
    while queue:
        x, y = queue.popleft()
        red, green, blue, _ = pixels[x, y]
        pixels[x, y] = (red, green, blue, 0)
        removed += 1
        if x > 0:
            try_seed(x - 1, y)
        if x + 1 < width:
            try_seed(x + 1, y)
        if y > 0:
            try_seed(x, y - 1)
        if y + 1 < height:
            try_seed(x, y + 1)

    args.output.parent.mkdir(parents=True, exist_ok=True)
    image.save(args.output, optimize=True)
    print(f"Wrote {args.output}")
    print(f"Removed border-connected chroma pixels: {removed}/{width * height}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
