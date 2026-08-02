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


def decontaminate_magenta_edges(image: Image.Image, passes: int) -> int:
    """Recover antialiased foreground color from a flat magenta matte.

    The connected-key pass removes the solid backdrop. Image generators often
    leave one or two composited edge pixels behind, though. Reconstructing the
    edge alpha from the known (255, 0, 255) matte avoids a purple fringe while
    preserving the opaque interior of each sprite.
    """
    width, height = image.size
    changed = 0
    for _ in range(max(0, passes)):
        source = image.copy()
        source_pixels = source.load()
        target_pixels = image.load()
        pass_changed = 0
        for y in range(height):
            for x in range(width):
                red, green, blue, opacity = source_pixels[x, y]
                if opacity <= 0:
                    continue

                neighbors = (
                    source_pixels[x - 1, y][3] if x > 0 else 0,
                    source_pixels[x + 1, y][3] if x + 1 < width else 0,
                    source_pixels[x, y - 1][3] if y > 0 else 0,
                    source_pixels[x, y + 1][3] if y + 1 < height else 0,
                )
                if min(neighbors) >= 250:
                    continue

                matte_alpha = max(
                    green / 255.0,
                    (255 - red) / 255.0,
                    (255 - blue) / 255.0,
                )
                matte_alpha = max(0.0, min(1.0, matte_alpha))
                if matte_alpha >= 0.995:
                    continue
                if matte_alpha <= 0.025:
                    target_pixels[x, y] = (0, 0, 0, 0)
                    pass_changed += 1
                    continue

                inverse = 1.0 - matte_alpha
                clean_red = round((red - inverse * 255.0) / matte_alpha)
                clean_green = round(green / matte_alpha)
                clean_blue = round((blue - inverse * 255.0) / matte_alpha)
                clean_opacity = round(opacity * matte_alpha)
                target_pixels[x, y] = (
                    max(0, min(255, clean_red)),
                    max(0, min(255, clean_green)),
                    max(0, min(255, clean_blue)),
                    max(0, min(255, clean_opacity)),
                )
                pass_changed += 1
        changed += pass_changed
        if pass_changed == 0:
            break
    return changed


def main() -> int:
    parser = argparse.ArgumentParser(
        description="Remove border-connected magenta while preserving isolated violet art."
    )
    parser.add_argument("--input", required=True, type=Path)
    parser.add_argument("--output", required=True, type=Path)
    parser.add_argument(
        "--decontaminate-edge-passes",
        type=int,
        default=0,
        help="Remove antialiased magenta matte from this many sprite-edge rings.",
    )
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

    decontaminated = decontaminate_magenta_edges(
        image,
        args.decontaminate_edge_passes,
    )

    args.output.parent.mkdir(parents=True, exist_ok=True)
    image.save(args.output, optimize=True)
    print(f"Wrote {args.output}")
    print(f"Removed border-connected chroma pixels: {removed}/{width * height}")
    if args.decontaminate_edge_passes > 0:
        print(f"Decontaminated sprite-edge pixels: {decontaminated}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
