#!/usr/bin/env python3
"""Build deterministic v2.21 Midgaard architecture, NPC, and road atlases.

Image-generation sources may contain a baked neutral checkerboard.  This tool
removes only bright, near-neutral pixels connected to each source cell edge,
then trims, normalizes, and validates every sprite. Road sources are inset,
normalized to fixed swatches, and edge-blended into exact seamless cells.
"""

from __future__ import annotations

import argparse
import hashlib
import json
from collections import deque
from pathlib import Path

from PIL import Image, ImageDraw, ImageStat


CELL_SIZE = 256
REQUIRED_GUTTER = 18
TARGET_BASELINE = CELL_SIZE - REQUIRED_GUTTER
ALPHA_THRESHOLD = 24

TOWN_COLUMNS = 5
TOWN_ROWS = 4
BUILDING_SOURCE_COLUMNS = 3
BUILDING_SOURCE_ROWS = 3
BUILDING_TARGETS = (0, 1, 3, 4, 5, 6, 7, 11, 14)
BUILDING_NAMES = (
    "market",
    "temple",
    "tavern",
    "armorer",
    "provisions",
    "weaponsmith",
    "enchanter",
    "town-hall",
    "cookhouse",
)

NPC_COLUMNS = 5
NPC_ROWS = 4
NPC_NAMES = (
    "watchman-rusk",
    "watchwoman-ilyra",
    "king-halvard",
    "market-clerk-nessa",
    "healer-mira",
    "tavern-keeper-orren",
    "armorer-borin",
    "weapon-merchant-tessa",
    "captain-brann",
    "enchanter-maud",
    "cook-kate",
    "provisioner-lute",
    "courier-tovan",
    "wounded-traveler-edda",
    "dock-worker",
    "stable-hand-pell",
    "herald-vann",
    "novice-healer-sera",
    "old-road-scout-yara",
    "scholar",
)
STABLE_HAND_INDEX = 15

CITIZEN_CELL_SIZE = 384
CITIZEN_REQUIRED_GUTTER = 20
CITIZEN_TARGET_BASELINE = CITIZEN_CELL_SIZE - CITIZEN_REQUIRED_GUTTER
CITIZEN_COLUMNS = 4
CITIZEN_ROWS = 2
CITIZEN_NAMES = (
    "lamplighter",
    "fishmonger",
    "tailor",
    "mason",
    "apothecary",
    "road-pilgrim",
    "gravedigger",
    "caravan-guide",
)

ROAD_COLUMNS = 2
ROAD_ROWS = 2
ROAD_NAMES = (
    "civic-cobble",
    "civic-setts",
    "old-road-stone-earth",
    "old-road-fine-gravel",
)


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as stream:
        for chunk in iter(lambda: stream.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest().upper()


def cell_bounds(
    image: Image.Image, columns: int, rows: int, index: int
) -> tuple[int, int, int, int]:
    column = index % columns
    row = index // columns
    return (
        round(column * image.width / columns),
        round(row * image.height / rows),
        round((column + 1) * image.width / columns),
        round((row + 1) * image.height / rows),
    )


def is_baked_checker_pixel(pixel: tuple[int, int, int, int]) -> bool:
    red, green, blue, _ = pixel
    return min(red, green, blue) >= 220 and max(red, green, blue) - min(red, green, blue) <= 18


def remove_baked_checkerboard(source: Image.Image) -> Image.Image:
    """Clear bright neutral background connected to a source crop's border."""

    image = source.convert("RGBA")
    width, height = image.size
    pixels = image.load()
    background = bytearray(width * height)
    pending: deque[tuple[int, int]] = deque()

    def enqueue(x: int, y: int) -> None:
        offset = y * width + x
        if background[offset] or not is_baked_checker_pixel(pixels[x, y]):
            return
        background[offset] = 1
        pending.append((x, y))

    for x in range(width):
        enqueue(x, 0)
        enqueue(x, height - 1)
    for y in range(height):
        enqueue(0, y)
        enqueue(width - 1, y)

    while pending:
        x, y = pending.popleft()
        if x > 0:
            enqueue(x - 1, y)
        if x + 1 < width:
            enqueue(x + 1, y)
        if y > 0:
            enqueue(x, y - 1)
        if y + 1 < height:
            enqueue(x, y + 1)

    cleaned = Image.new("RGBA", image.size, (0, 0, 0, 0))
    cleaned_pixels = cleaned.load()
    for y in range(height):
        row_offset = y * width
        for x in range(width):
            if not background[row_offset + x]:
                red, green, blue, _ = pixels[x, y]
                cleaned_pixels[x, y] = (red, green, blue, 255)
    return cleaned


def alpha_bbox(image: Image.Image) -> tuple[int, int, int, int] | None:
    alpha = image.getchannel("A")
    return alpha.point(lambda value: 255 if value > ALPHA_THRESHOLD else 0).getbbox()


def retain_subject_components(image: Image.Image) -> Image.Image:
    """Drop small neighbor-cell fragments while preserving props and shadows."""

    alpha = image.getchannel("A")
    pixels = alpha.load()
    width, height = image.size
    visited = bytearray(width * height)
    components: list[tuple[list[tuple[int, int]], tuple[int, int, int, int]]] = []

    for y in range(height):
        for x in range(width):
            offset = y * width + x
            if visited[offset] or pixels[x, y] <= ALPHA_THRESHOLD:
                continue
            visited[offset] = 1
            pending: deque[tuple[int, int]] = deque(((x, y),))
            points: list[tuple[int, int]] = []
            left = right = x
            top = bottom = y
            while pending:
                px, py = pending.popleft()
                points.append((px, py))
                left = min(left, px)
                right = max(right, px)
                top = min(top, py)
                bottom = max(bottom, py)
                for nx, ny in (
                    (px - 1, py),
                    (px + 1, py),
                    (px, py - 1),
                    (px, py + 1),
                ):
                    if nx < 0 or ny < 0 or nx >= width or ny >= height:
                        continue
                    neighbor = ny * width + nx
                    if visited[neighbor] or pixels[nx, ny] <= ALPHA_THRESHOLD:
                        continue
                    visited[neighbor] = 1
                    pending.append((nx, ny))
            components.append((points, (left, top, right + 1, bottom + 1)))

    if not components:
        return image
    components.sort(key=lambda entry: len(entry[0]), reverse=True)
    main_points, main_bounds = components[0]
    main_size = len(main_points)
    keep = set(main_points)
    for points, bounds in components[1:]:
        touches_crop_edge = (
            bounds[0] == 0
            or bounds[1] == 0
            or bounds[2] == width
            or bounds[3] == height
        )
        if touches_crop_edge:
            # Image-generation contact sheets can spill a piece of the cell
            # above or below into a fixed grid crop.  Legitimate detached
            # props and contact shadows are inset; border-touching secondary
            # components belong to the neighboring source cell.
            continue
        horizontal_overlap = bounds[0] < main_bounds[2] and bounds[2] > main_bounds[0]
        substantial = len(points) >= main_size * 0.10
        if horizontal_overlap or substantial:
            keep.update(points)

    cleaned = Image.new("RGBA", image.size, (0, 0, 0, 0))
    source_pixels = image.load()
    target_pixels = cleaned.load()
    for x, y in keep:
        target_pixels[x, y] = source_pixels[x, y]
    return cleaned


def extract_clean_cell(
    source: Image.Image, columns: int, rows: int, index: int
) -> tuple[Image.Image, tuple[int, int, int, int]]:
    region = cell_bounds(source, columns, rows, index)
    cleaned = retain_subject_components(
        remove_baked_checkerboard(source.crop(region))
    )
    bounds = alpha_bbox(cleaned)
    if bounds is None:
        raise ValueError(f"Source cell {index} has no foreground after checker cleanup")
    return cleaned.crop(bounds), region


def extract_clean_single(source: Image.Image) -> tuple[Image.Image, tuple[int, int, int, int]]:
    cleaned = retain_subject_components(remove_baked_checkerboard(source))
    bounds = alpha_bbox(cleaned)
    if bounds is None:
        raise ValueError("Single-sprite source has no foreground after checker cleanup")
    return cleaned.crop(bounds), (0, 0, source.width, source.height)


def normalize_sprite(
    sprite: Image.Image,
    maximum_width: int,
    maximum_height: int,
    allow_upscale: bool,
    minimum_height: int = 0,
    cell_size: int = CELL_SIZE,
    required_gutter: int = REQUIRED_GUTTER,
    target_baseline: int = TARGET_BASELINE,
) -> tuple[Image.Image, dict[str, object]]:
    source_width, source_height = sprite.size
    scale = min(
        maximum_width / max(1, source_width),
        maximum_height / max(1, source_height),
    )
    if not allow_upscale:
        scale = min(scale, 1.0)
    width = max(1, round(sprite.width * scale))
    height = max(1, round(sprite.height * scale))
    if minimum_height > 0 and height < minimum_height:
        # A few deliberately broad shopfronts need a modest vertical lift to
        # retain a roof-led architectural read inside the fixed 18px side
        # gutters.  Width and baseline stay unchanged.
        height = minimum_height
    if sprite.size != (width, height):
        sprite = sprite.resize((width, height), Image.Resampling.LANCZOS)

    x = (cell_size - width) // 2
    y = target_baseline - height
    if x < required_gutter or y < required_gutter:
        raise ValueError(
            f"Normalized sprite cannot satisfy {required_gutter}px gutters: "
            f"size={(width, height)}, position={(x, y)}"
        )

    cell = Image.new("RGBA", (cell_size, cell_size), (0, 0, 0, 0))
    cell.alpha_composite(sprite, (x, y))
    bounds = alpha_bbox(cell)
    if bounds is None:
        raise ValueError("Normalized sprite is empty")
    alpha = cell.getchannel("A")
    visible_pixels = sum(alpha.histogram()[ALPHA_THRESHOLD + 1 :])
    return cell, {
        "sourceSize": [source_width, source_height],
        "scale": round(scale, 6),
        "scaleX": round(width / max(1, source_width), 6),
        "scaleY": round(height / max(1, source_height), 6),
        "normalizedSize": [width, height],
        "runtimeBounds": list(bounds),
        "runtimeSize": [bounds[2] - bounds[0], bounds[3] - bounds[1]],
        "gutters": {
            "left": bounds[0],
            "top": bounds[1],
            "right": cell_size - bounds[2],
            "bottom": cell_size - bounds[3],
        },
        "visibleFraction": round(visible_pixels / (cell_size * cell_size), 6),
    }


def paste_cell(
    atlas: Image.Image,
    cell: Image.Image,
    index: int,
    columns: int,
    cell_size: int = CELL_SIZE,
) -> None:
    x = (index % columns) * cell_size
    y = (index // columns) * cell_size
    atlas.paste((0, 0, 0, 0), (x, y, x + cell_size, y + cell_size))
    atlas.alpha_composite(cell, (x, y))


def fixed_cell(atlas: Image.Image, index: int, columns: int) -> Image.Image:
    x = (index % columns) * CELL_SIZE
    y = (index // columns) * CELL_SIZE
    return atlas.crop((x, y, x + CELL_SIZE, y + CELL_SIZE))


def make_edge_blended_seamless(source: Image.Image, size: int = CELL_SIZE) -> Image.Image:
    """Blend only opposite edge bands, keeping the source texture's center intact."""

    cell = source.convert("RGBA").resize((size, size), Image.Resampling.LANCZOS)
    pixels = cell.load()
    band = max(8, size // 12)

    for y in range(size):
        for inset in range(band):
            left_x = inset
            right_x = size - 1 - inset
            left = pixels[left_x, y]
            right = pixels[right_x, y]
            average = tuple(round((left[channel] + right[channel]) * 0.5) for channel in range(4))
            weight = (band - inset) / band
            pixels[left_x, y] = tuple(
                round(left[channel] * (1.0 - weight) + average[channel] * weight)
                for channel in range(4)
            )
            pixels[right_x, y] = tuple(
                round(right[channel] * (1.0 - weight) + average[channel] * weight)
                for channel in range(4)
            )

    for x in range(size):
        for inset in range(band):
            top_y = inset
            bottom_y = size - 1 - inset
            top = pixels[x, top_y]
            bottom = pixels[x, bottom_y]
            average = tuple(round((top[channel] + bottom[channel]) * 0.5) for channel in range(4))
            weight = (band - inset) / band
            pixels[x, top_y] = tuple(
                round(top[channel] * (1.0 - weight) + average[channel] * weight)
                for channel in range(4)
            )
            pixels[x, bottom_y] = tuple(
                round(bottom[channel] * (1.0 - weight) + average[channel] * weight)
                for channel in range(4)
            )
    return cell


def opposite_edge_delta(cell: Image.Image) -> float:
    rgba = cell.convert("RGBA")
    width, height = rgba.size
    pixels = rgba.load()
    total = 0
    samples = 0
    for y in range(height):
        for channel in range(4):
            total += abs(pixels[0, y][channel] - pixels[width - 1, y][channel])
            samples += 1
    for x in range(width):
        for channel in range(4):
            total += abs(pixels[x, 0][channel] - pixels[x, height - 1][channel])
            samples += 1
    return round(total / max(1, samples), 6)


def build_town_atlas(
    base: Image.Image, building_source: Image.Image
) -> tuple[Image.Image, list[dict[str, object]]]:
    expected = (TOWN_COLUMNS * CELL_SIZE, TOWN_ROWS * CELL_SIZE)
    if base.size != expected:
        raise ValueError(f"Unexpected town base size: {base.size}, expected {expected}")
    atlas = base.copy()
    records: list[dict[str, object]] = []
    for source_index, (target_index, name) in enumerate(zip(BUILDING_TARGETS, BUILDING_NAMES)):
        sprite, source_region = extract_clean_cell(
            building_source,
            BUILDING_SOURCE_COLUMNS,
            BUILDING_SOURCE_ROWS,
            source_index,
        )
        cell, metrics = normalize_sprite(sprite, 220, 220, True, 188)
        paste_cell(atlas, cell, target_index, TOWN_COLUMNS)
        records.append(
            {
                "name": name,
                "sourceIndex": source_index,
                "targetIndex": target_index,
                "sourceRegion": list(source_region),
                **metrics,
            }
        )
    return atlas, records


def build_npc_atlas(
    npc_source: Image.Image, stable_hand_source: Image.Image
) -> tuple[Image.Image, list[dict[str, object]]]:
    atlas = Image.new(
        "RGBA",
        (NPC_COLUMNS * CELL_SIZE, NPC_ROWS * CELL_SIZE),
        (0, 0, 0, 0),
    )
    records: list[dict[str, object]] = []
    for index, name in enumerate(NPC_NAMES):
        if index == STABLE_HAND_INDEX:
            sprite, source_region = extract_clean_single(stable_hand_source)
            source_label = "stable-hand-source"
        else:
            sprite, source_region = extract_clean_cell(
                npc_source, NPC_COLUMNS, NPC_ROWS, index
            )
            source_label = "npc-sheet"
        cell, metrics = normalize_sprite(sprite, 220, 220, True)
        height = metrics["runtimeSize"][1]
        if height < 204 or height > 220:
            raise ValueError(f"NPC {name} normalized to unexpected height {height}")
        paste_cell(atlas, cell, index, NPC_COLUMNS)
        records.append(
            {
                "name": name,
                "source": source_label,
                "sourceIndex": index,
                "targetIndex": index,
                "sourceRegion": list(source_region),
                **metrics,
            }
        )
    return atlas, records


def build_citizen_atlas(
    citizen_source: Image.Image,
) -> tuple[Image.Image, list[dict[str, object]]]:
    atlas = Image.new(
        "RGBA",
        (CITIZEN_COLUMNS * CITIZEN_CELL_SIZE, CITIZEN_ROWS * CITIZEN_CELL_SIZE),
        (0, 0, 0, 0),
    )
    records: list[dict[str, object]] = []
    for index, name in enumerate(CITIZEN_NAMES):
        sprite, source_region = extract_clean_cell(
            citizen_source,
            CITIZEN_COLUMNS,
            CITIZEN_ROWS,
            index,
        )
        cell, metrics = normalize_sprite(
            sprite,
            344,
            344,
            True,
            cell_size=CITIZEN_CELL_SIZE,
            required_gutter=CITIZEN_REQUIRED_GUTTER,
            target_baseline=CITIZEN_TARGET_BASELINE,
        )
        height = metrics["runtimeSize"][1]
        if height < 334 or height > 344:
            raise ValueError(f"Citizen {name} normalized to unexpected height {height}")
        paste_cell(atlas, cell, index, CITIZEN_COLUMNS, CITIZEN_CELL_SIZE)
        records.append(
            {
                "name": name,
                "sourceIndex": index,
                "targetIndex": index,
                "sourceRegion": list(source_region),
                **metrics,
            }
        )
    return atlas, records


def build_road_surface_atlas(
    road_source: Image.Image,
) -> tuple[Image.Image, list[dict[str, object]]]:
    atlas = Image.new(
        "RGBA",
        (ROAD_COLUMNS * CELL_SIZE, ROAD_ROWS * CELL_SIZE),
        (0, 0, 0, 255),
    )
    records: list[dict[str, object]] = []
    for index, name in enumerate(ROAD_NAMES):
        source_region = cell_bounds(road_source, ROAD_COLUMNS, ROAD_ROWS, index)
        source_cell = road_source.crop(source_region)
        # Image-generated contact sheets often paint a dark divider exactly on
        # the quadrant edge. Inset before mirroring so that divider cannot be
        # folded into the middle of the seamless runtime swatch.
        inset = max(4, round(min(source_cell.size) * 0.018))
        source_cell = source_cell.crop(
            (inset, inset, source_cell.width - inset, source_cell.height - inset)
        )
        cell = make_edge_blended_seamless(source_cell)
        x = (index % ROAD_COLUMNS) * CELL_SIZE
        y = (index // ROAD_COLUMNS) * CELL_SIZE
        atlas.paste(cell, (x, y))
        mean = [round(value, 3) for value in ImageStat.Stat(cell.convert("RGB")).mean]
        records.append(
            {
                "name": name,
                "sourceIndex": index,
                "targetIndex": index,
                "sourceRegion": list(source_region),
                "runtimeSize": [CELL_SIZE, CELL_SIZE],
                "meanRgb": mean,
                "oppositeEdgeMeanDelta": opposite_edge_delta(cell),
            }
        )
    return atlas, records


def make_contact_sheet(
    town: Image.Image,
    npcs: Image.Image,
    citizens: Image.Image,
    roads: Image.Image,
) -> Image.Image:
    margin = 28
    gap = 28
    town_preview = town.resize((TOWN_COLUMNS * 128, TOWN_ROWS * 128), Image.Resampling.LANCZOS)
    npc_preview = npcs.resize((NPC_COLUMNS * 96, NPC_ROWS * 96), Image.Resampling.LANCZOS)
    citizen_preview = citizens.resize((384, 192), Image.Resampling.LANCZOS)
    road_preview = roads.resize((192, 192), Image.Resampling.LANCZOS)
    lower_width = citizen_preview.width + gap + road_preview.width
    width = max(town_preview.width, npc_preview.width, lower_width) + margin * 2
    height = town_preview.height + npc_preview.height + citizen_preview.height + margin * 2 + gap * 2
    sheet = Image.new("RGBA", (width, height), (18, 22, 24, 255))
    draw = ImageDraw.Draw(sheet)
    town_x = (width - town_preview.width) // 2
    npc_x = (width - npc_preview.width) // 2
    sheet.alpha_composite(town_preview, (town_x, margin))
    npc_y = margin + town_preview.height + gap
    sheet.alpha_composite(npc_preview, (npc_x, npc_y))
    draw.line((margin, npc_y - gap // 2, width - margin, npc_y - gap // 2), fill=(92, 111, 108, 255), width=2)
    lower_y = npc_y + npc_preview.height + gap
    lower_x = (width - lower_width) // 2
    sheet.alpha_composite(citizen_preview, (lower_x, lower_y))
    sheet.alpha_composite(road_preview, (lower_x + citizen_preview.width + gap, lower_y))
    draw.line((margin, lower_y - gap // 2, width - margin, lower_y - gap // 2), fill=(92, 111, 108, 255), width=2)
    return sheet


def write_report(
    path: Path,
    kind: str,
    output: Path,
    sources: list[Path],
    records: list[dict[str, object]],
    cell_size: int = CELL_SIZE,
    required_gutter: int = REQUIRED_GUTTER,
    target_baseline: int | None = TARGET_BASELINE,
) -> None:
    report = {
        "kind": kind,
        # Keep the report reproducible when a verifier writes the same atlas
        # into a temporary directory.
        "output": output.name,
        "outputSha256": sha256(output),
        "atlasSize": list(Image.open(output).size),
        "cellSize": cell_size,
        "requiredGutter": required_gutter,
        "targetBaseline": target_baseline,
        "sources": [
            {"path": str(source), "sha256": sha256(source)} for source in sources
        ],
        "cells": records,
    }
    path.write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--town-base", required=True, type=Path)
    parser.add_argument("--building-source", required=True, type=Path)
    parser.add_argument("--npc-source", required=True, type=Path)
    parser.add_argument("--stable-hand-source", required=True, type=Path)
    parser.add_argument("--citizen-source", required=True, type=Path)
    parser.add_argument("--road-source", required=True, type=Path)
    parser.add_argument("--town-output", required=True, type=Path)
    parser.add_argument("--npc-output", required=True, type=Path)
    parser.add_argument("--citizen-output", required=True, type=Path)
    parser.add_argument("--road-output", required=True, type=Path)
    parser.add_argument("--town-report", required=True, type=Path)
    parser.add_argument("--npc-report", required=True, type=Path)
    parser.add_argument("--citizen-report", required=True, type=Path)
    parser.add_argument("--road-report", required=True, type=Path)
    parser.add_argument("--contact-sheet", required=True, type=Path)
    args = parser.parse_args()

    for path in (
        args.town_base,
        args.building_source,
        args.npc_source,
        args.stable_hand_source,
        args.citizen_source,
        args.road_source,
    ):
        if not path.is_file():
            raise FileNotFoundError(path)

    with Image.open(args.town_base) as opened:
        town_base = opened.convert("RGBA")
    with Image.open(args.building_source) as opened:
        building_source = opened.convert("RGBA")
    with Image.open(args.npc_source) as opened:
        npc_source = opened.convert("RGBA")
    with Image.open(args.stable_hand_source) as opened:
        stable_hand_source = opened.convert("RGBA")
    with Image.open(args.citizen_source) as opened:
        citizen_source = opened.convert("RGBA")
    with Image.open(args.road_source) as opened:
        road_source = opened.convert("RGBA")

    town, town_records = build_town_atlas(town_base, building_source)
    npcs, npc_records = build_npc_atlas(npc_source, stable_hand_source)
    citizens, citizen_records = build_citizen_atlas(citizen_source)
    roads, road_records = build_road_surface_atlas(road_source)

    for output in (
        args.town_output,
        args.npc_output,
        args.citizen_output,
        args.road_output,
        args.contact_sheet,
    ):
        output.parent.mkdir(parents=True, exist_ok=True)
    town.save(args.town_output, optimize=True)
    npcs.save(args.npc_output, optimize=True)
    citizens.save(args.citizen_output, optimize=True)
    roads.save(args.road_output, optimize=True)
    make_contact_sheet(town, npcs, citizens, roads).save(args.contact_sheet, optimize=True)
    write_report(
        args.town_report,
        "midgaard-town-atlas",
        args.town_output,
        [args.town_base, args.building_source],
        town_records,
    )
    write_report(
        args.npc_report,
        "midgaard-npc-atlas",
        args.npc_output,
        [args.npc_source, args.stable_hand_source],
        npc_records,
    )
    write_report(
        args.citizen_report,
        "world-npc-citizen-atlas",
        args.citizen_output,
        [args.citizen_source],
        citizen_records,
        CITIZEN_CELL_SIZE,
        CITIZEN_REQUIRED_GUTTER,
        CITIZEN_TARGET_BASELINE,
    )
    write_report(
        args.road_report,
        "midgaard-road-surface-atlas",
        args.road_output,
        [args.road_source],
        road_records,
        CELL_SIZE,
        0,
        None,
    )
    print(
        json.dumps(
            {
                "town": str(args.town_output),
                "townSha256": sha256(args.town_output),
                "npcs": str(args.npc_output),
                "npcSha256": sha256(args.npc_output),
                "citizens": str(args.citizen_output),
                "citizenSha256": sha256(args.citizen_output),
                "roads": str(args.road_output),
                "roadSha256": sha256(args.road_output),
                "contactSheet": str(args.contact_sheet),
            }
        )
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
