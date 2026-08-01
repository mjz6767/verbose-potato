#!/usr/bin/env python3
"""Build Ash & Brimstone's original music and sound-design masters.

The generator is intentionally self-contained: it uses only Python's standard
library and NumPy, consumes no recordings or model-generated audio, and emits
deterministic PCM WAV files suitable for Unity import.
"""

from __future__ import annotations

import argparse
import csv
import hashlib
import json
import math
import wave
from dataclasses import asdict, dataclass
from pathlib import Path
from typing import Iterable, Sequence

import numpy as np


MUSIC_SAMPLE_RATE = 32_000
SFX_SAMPLE_RATE = 48_000
MUSIC_BEATS = 32
STAGE_ROOT = Path(__file__).resolve().parents[2]
MUSIC_DIR = STAGE_ROOT / "Assets" / "Resources" / "Audio" / "Music"
SFX_DIR = STAGE_ROOT / "Assets" / "Resources" / "Audio" / "Sfx"
DOCS_DIR = STAGE_ROOT / "Docs"
QA_DIR = STAGE_ROOT / "QA"


@dataclass(frozen=True)
class TrackSpec:
    cue: str
    title: str
    bpm: float
    root_midi: int
    mode: tuple[int, ...]
    progression: tuple[int, ...]
    palette: str
    energy: float
    direction: str


@dataclass(frozen=True)
class SfxSpec:
    cue: str
    family: str
    duration: float
    direction: str


@dataclass
class AssetMetrics:
    cue: str
    output: str
    kind: str
    title: str
    direction: str
    sample_rate: int
    channels: int
    frames: int
    duration_seconds: float
    peak_dbfs: float
    rms_dbfs: float
    seam_delta_dbfs: str
    sha256: str
    provenance: str = "Original deterministic synthesis; no external samples"


AEOLIAN = (0, 2, 3, 5, 7, 8, 10)
DORIAN = (0, 2, 3, 5, 7, 9, 10)
PHRYGIAN = (0, 1, 3, 5, 7, 8, 10)
HARMONIC_MINOR = (0, 2, 3, 5, 7, 8, 11)
MIXOLYDIAN = (0, 2, 4, 5, 7, 9, 10)


TRACKS: tuple[TrackSpec, ...] = (
    TrackSpec(
        "tavern_storm_hearth_ensemble_loop",
        "The Brimstone Overture",
        78,
        50,
        DORIAN,
        (0, 0, 3, 5, 4, 0, 6, 3, 5, 4, 3, 0),
        "tavern",
        0.56,
        "A twelve-bar title overture with a forged lute motif, bowed counter-line, frame drum, ember bells, hearth, and rain beyond the shutters.",
    ),
    TrackSpec(
        "muster_by_firelight_loop",
        "Muster by Firelight",
        80,
        52,
        DORIAN,
        (0, 4, 3, 5, 0, 6, 3, 4),
        "muster",
        0.38,
        "Patient lute and low strings for choosing a company before the road.",
    ),
    TrackSpec(
        "midgaard_lamps_loop",
        "Lamps of Midgaard",
        84,
        55,
        MIXOLYDIAN,
        (0, 3, 4, 0, 5, 3, 4, 0),
        "city",
        0.42,
        "Restrained civic bells, plucked strings, and a walking pulse under guarded streets.",
    ),
    TrackSpec(
        "midgaard_throne_room_loop",
        "Beneath the Iron Crown",
        75,
        48,
        HARMONIC_MINOR,
        (0, 4, 5, 0, 3, 6, 4, 0),
        "throne",
        0.38,
        "Measured low strings and bronze intervals for King's Hall without heroic excess.",
    ),
    TrackSpec(
        "midgaard_merchant_hall_loop",
        "Lanterns, Ledgers, and Steel",
        88,
        55,
        MIXOLYDIAN,
        (0, 3, 4, 0, 5, 3, 6, 4),
        "merchant",
        0.43,
        "A nimble hammered-string pattern with quiet counter and forge colors.",
    ),
    TrackSpec(
        "old_road_walk_loop",
        "Boots on the Old Road",
        92,
        50,
        DORIAN,
        (0, 6, 3, 4, 0, 5, 3, 6),
        "road",
        0.46,
        "Open fifths, travel drum, and a spare reed melody with room for exploration ambience.",
    ),
    TrackSpec(
        "salt_cistern_drips_loop",
        "Salt Below the Bellstone",
        72,
        47,
        PHRYGIAN,
        (0, 1, 5, 0, 3, 1, 6, 0),
        "sewer",
        0.32,
        "Subterranean drone, water drops, bowed harmonics, and widely spaced low notes.",
    ),
    TrackSpec(
        "dusk_market_ambush_loop",
        "Knives under Dusk Market",
        98,
        50,
        PHRYGIAN,
        (0, 1, 5, 3, 0, 6, 1, 4),
        "dusk",
        0.62,
        "Muted hand drum, plucked ostinato, and a watchful low reed for the ambush route.",
    ),
    TrackSpec(
        "mouth_of_the_deep_loop",
        "The Mouth of the Deep",
        70,
        45,
        PHRYGIAN,
        (0, 1, 4, 0, 5, 3, 1, 0),
        "cave",
        0.30,
        "A threshold cue of bowed stone resonance, breath, and widely separated drops.",
    ),
    TrackSpec(
        "combat_battle_pulse_loop",
        "Steel in the Lantern Smoke",
        112,
        50,
        AEOLIAN,
        (0, 5, 6, 4, 0, 3, 6, 5),
        "combat",
        0.78,
        "Dry frame drums and insistent strings with a clear tactical pulse.",
    ),
    TrackSpec(
        "sewer_hunt_combat_loop",
        "The Cistern Gives Chase",
        104,
        47,
        PHRYGIAN,
        (0, 1, 5, 0, 6, 1, 3, 0),
        "sewercombat",
        0.72,
        "Wet percussion, low bowed ostinato, and metallic echoes for the cistern fights.",
    ),
    TrackSpec(
        "ratfolk_plague_march_loop",
        "Plague March below Midgaard",
        108,
        47,
        PHRYGIAN,
        (0, 1, 5, 0, 6, 3, 1, 0),
        "ratfolk",
        0.80,
        "Scuttling wood clicks, muffled frame drum, and a diseased narrow-range string figure.",
    ),
    TrackSpec(
        "kobold_hide_drums_loop",
        "Hide Drums in the Dark",
        118,
        52,
        DORIAN,
        (0, 3, 6, 4, 0, 5, 3, 6),
        "kobold",
        0.82,
        "Asymmetric hide drums, bone clicks, and darting plucked figures.",
    ),
    TrackSpec(
        "crown_and_ashes_boss_loop",
        "Crown and Ashes",
        96,
        45,
        HARMONIC_MINOR,
        (0, 5, 4, 0, 6, 3, 4, 0),
        "boss",
        0.90,
        "Processional low brass colors, war drum, and rising minor figures for major foes.",
    ),
    TrackSpec(
        "crooked_crown_kobold_king_loop",
        "The Crooked Crown",
        124,
        50,
        HARMONIC_MINOR,
        (0, 5, 6, 0, 3, 4, 6, 0),
        "king",
        0.95,
        "A volatile boss reel: rapid hide drums, crooked fanfare, and snarling low strings.",
    ),
    TrackSpec(
        "embers_carry_home_victory_loop",
        "Embers Carry Home",
        100,
        55,
        MIXOLYDIAN,
        (0, 3, 4, 0, 5, 3, 4, 0),
        "victory",
        0.60,
        "A short, grounded return theme that celebrates without becoming triumphant bombast.",
    ),
    TrackSpec(
        "ashes_on_the_road_defeat_loop",
        "Ashes on the Road",
        68,
        45,
        AEOLIAN,
        (0, 5, 3, 0, 6, 5, 4, 0),
        "defeat",
        0.25,
        "Low drone, isolated bell, and a descending line that leaves space for the retry screen.",
    ),
    TrackSpec(
        "bells_over_temple_square_loop",
        "Bells over Temple Square",
        76,
        55,
        MIXOLYDIAN,
        (0, 4, 3, 0, 5, 4, 2, 0),
        "city",
        0.30,
        "Open bronze bells, bowed fifths, and a patient sacred pulse around Temple Square.",
    ),
    TrackSpec(
        "lanterns_and_ledgers_loop",
        "Lanterns and Ledgers",
        92,
        55,
        MIXOLYDIAN,
        (0, 3, 4, 5, 0, 6, 4, 3),
        "merchant",
        0.45,
        "Quick hammered strings and quiet counter rhythms for Midgaard's working market.",
    ),
    TrackSpec(
        "wet_cobble_reel_loop",
        "Wet Cobble Reel",
        104,
        55,
        DORIAN,
        (0, 3, 4, 6, 0, 5, 3, 4),
        "tavern",
        0.50,
        "A rain-softened street reel carried by plucks, frame drum, and tavern warmth.",
    ),
    TrackSpec(
        "watchfires_on_the_wall_loop",
        "Watchfires on the Wall",
        88,
        50,
        DORIAN,
        (0, 4, 3, 5, 0, 6, 4, 3),
        "road",
        0.46,
        "Measured watch drum and low strings for Midgaard's guarded gates.",
    ),
    TrackSpec(
        "under_the_bellstone_loop",
        "Under the Bellstone",
        72,
        45,
        PHRYGIAN,
        (0, 1, 4, 0, 5, 1, 3, 0),
        "cave",
        0.34,
        "Bell resonance gives way to wet stone and a low descending threshold figure.",
    ),
    TrackSpec(
        "banners_before_the_crown_loop",
        "Banners before the Crown",
        78,
        48,
        HARMONIC_MINOR,
        (0, 4, 5, 0, 3, 6, 5, 4),
        "throne",
        0.44,
        "Restrained processional strings and bronze intervals on the approach to King's Hall.",
    ),
    TrackSpec(
        "last_lamps_east_loop",
        "Last Lamps East",
        86,
        50,
        DORIAN,
        (0, 3, 5, 4, 0, 6, 3, 4),
        "road",
        0.38,
        "A sparse road theme that lets Midgaard's final gate lamps fall behind.",
    ),
    TrackSpec(
        "green_shrine_teal_loop",
        "Green Shrine Teal",
        74,
        55,
        DORIAN,
        (0, 3, 5, 0, 4, 6, 3, 0),
        "muster",
        0.31,
        "Airy reed, leaflike plucks, and open fifths for the green road.",
    ),
    TrackSpec(
        "old_quarry_stone_loop",
        "Old Quarry Stone",
        80,
        45,
        AEOLIAN,
        (0, 5, 3, 0, 6, 4, 3, 0),
        "cave",
        0.40,
        "Low struck stone, bowed weight, and long pauses across the abandoned quarry.",
    ),
    TrackSpec(
        "glass_warrens_shimmer_loop",
        "Glass Warrens Shimmer",
        94,
        57,
        HARMONIC_MINOR,
        (0, 4, 6, 3, 0, 5, 4, 6),
        "city",
        0.41,
        "Brittle bells and bright plucks refract over a quiet, uneasy drone.",
    ),
    TrackSpec(
        "ash_fen_haze_loop",
        "Ash Fen Haze",
        68,
        45,
        PHRYGIAN,
        (0, 1, 3, 0, 5, 1, 6, 0),
        "sewer",
        0.33,
        "Muffled drone, reed breath, and slow pulses moving through wet ash.",
    ),
    TrackSpec(
        "red_gate_omen_loop",
        "Red Gate Omen",
        90,
        45,
        HARMONIC_MINOR,
        (0, 5, 4, 0, 6, 3, 5, 0),
        "boss",
        0.58,
        "A restrained war pulse and rising minor intervals beneath the red walls.",
    ),
    TrackSpec(
        "gloam_courts_echo_loop",
        "Gloam Courts Echo",
        70,
        47,
        AEOLIAN,
        (0, 5, 3, 0, 6, 4, 2, 0),
        "defeat",
        0.30,
        "Hollow bowed lines and distant bell answers among ruined courts.",
    ),
    TrackSpec(
        "a_fire_between_roads_loop",
        "A Fire between Roads",
        76,
        52,
        DORIAN,
        (0, 3, 4, 0, 5, 3, 6, 4),
        "muster",
        0.28,
        "Low hearth, settled strings, and a small plucked figure for a guarded camp.",
    ),
    TrackSpec(
        "old_green_prayer_loop",
        "Old Green Prayer",
        68,
        55,
        DORIAN,
        (0, 3, 5, 0, 4, 6, 3, 0),
        "muster",
        0.27,
        "Slow reed and bell phrases with wide silence around an old roadside shrine.",
    ),
    TrackSpec(
        "names_worn_away_loop",
        "Names Worn Away",
        64,
        47,
        AEOLIAN,
        (0, 5, 3, 0, 6, 4, 1, 0),
        "defeat",
        0.24,
        "Weathered bell, bowed dust, and descending fragments for forgotten ruins.",
    ),
    TrackSpec(
        "glass_and_quiet_stars_loop",
        "Glass and Quiet Stars",
        90,
        57,
        HARMONIC_MINOR,
        (0, 4, 6, 3, 0, 5, 2, 6),
        "city",
        0.37,
        "High glass tones and a slowly turning arcane bass beneath quiet stars.",
    ),
    TrackSpec(
        "footsteps_behind_loop",
        "Footsteps Behind",
        120,
        50,
        PHRYGIAN,
        (0, 1, 5, 3, 0, 6, 1, 4),
        "dusk",
        0.68,
        "Urgent frame drum, clipped plucks, and a narrow pursuit figure for an alerted road.",
    ),
    TrackSpec(
        "roots_remember_loop",
        "Roots Remember",
        70,
        52,
        DORIAN,
        (0, 3, 5, 0, 4, 6, 3, 0),
        "road",
        0.29,
        "Low wood resonance, leaflike plucks, and a patient reed line in the old grove.",
    ),
    TrackSpec(
        "smoke_across_the_road_loop",
        "Smoke across the Road",
        100,
        47,
        AEOLIAN,
        (0, 5, 6, 3, 0, 4, 6, 5),
        "combat",
        0.57,
        "Distant war drum and guarded string motion around a hostile camp.",
    ),
    TrackSpec(
        "drow_nightblades_loop",
        "Drow Nightblades",
        108,
        54,
        PHRYGIAN,
        (0, 1, 5, 3, 0, 6, 4, 1),
        "dusk",
        0.73,
        "Muted blade rhythm, shadowed plucks, and a cool descending combat line.",
    ),
    TrackSpec(
        "red_rift_war_loop",
        "Red Rift War",
        100,
        43,
        HARMONIC_MINOR,
        (0, 5, 4, 6, 0, 3, 5, 4),
        "boss",
        0.88,
        "Heavy war drum, strained low strings, and unstable rising rift intervals.",
    ),
    TrackSpec(
        "bones_beneath_stone_loop",
        "Bones beneath Stone",
        90,
        45,
        PHRYGIAN,
        (0, 1, 5, 0, 6, 3, 1, 0),
        "sewercombat",
        0.70,
        "Dry bone clicks and hollow bowed motion under a deliberate battle pulse.",
    ),
    TrackSpec(
        "sigils_crossed_arcane_duel_loop",
        "Sigils Crossed",
        116,
        57,
        HARMONIC_MINOR,
        (0, 4, 6, 3, 0, 5, 7, 4),
        "combat",
        0.76,
        "Fast glassy figures and crossed rising lines for a dangerous caster duel.",
    ),
    TrackSpec(
        "steel_against_the_chosen_loop",
        "Steel against the Chosen",
        112,
        50,
        AEOLIAN,
        (0, 5, 6, 4, 0, 3, 6, 5),
        "combat",
        0.84,
        "Hard frame drum and broad strings for an elite foe without boss fanfare.",
    ),
    TrackSpec(
        "one_more_turn_last_stand_loop",
        "One More Turn",
        124,
        52,
        DORIAN,
        (0, 3, 4, 5, 0, 6, 4, 3),
        "victory",
        0.88,
        "A rising, urgent pulse that keeps its resolve while the party is near collapse.",
    ),
    TrackSpec(
        "the_rift_walks_demon_lord_loop",
        "The Rift Walks",
        96,
        41,
        HARMONIC_MINOR,
        (0, 5, 4, 6, 0, 3, 7, 5),
        "boss",
        0.98,
        "Massive low strings, ritual war drum, and a broken fanfare for the demon-lord finale.",
    ),
)


SFX: tuple[SfxSpec, ...] = (
    SfxSpec("heal", "magic", 0.62, "Soft holy lift with three resolved overtones."),
    SfxSpec("ward", "magic", 0.56, "Protective glass-and-bronze shimmer with a firm low arrival."),
    SfxSpec("light", "magic", 0.48, "Clean radiant flash with no explosive low end."),
    SfxSpec("curse", "magic", 0.72, "Descending shadow resonance and dry whisper texture."),
    SfxSpec("death", "magic", 0.86, "Hollow downward pull with a restrained sub tail."),
    SfxSpec("web", "magic", 0.42, "Fibrous snap and sticky high-frequency pull."),
    SfxSpec("poison", "magic", 0.68, "Viscous bubbling contour with a sickly minor resonance."),
    SfxSpec("fieldgas", "magic", 0.72, "Expanding alchemical hiss."),
    SfxSpec("fieldsnare", "magic", 0.48, "Roots and cord tightening around a target."),
    SfxSpec("fieldholy", "magic", 0.66, "Sustained sanctuary ring with a quiet choral illusion."),
    SfxSpec("fieldcurse", "magic", 0.74, "Low ritual seal with a reversed shimmer."),
    SfxSpec("spellrelease", "magic", 0.36, "General formula release articulation that stays below the impact."),
    SfxSpec("castmend", "magic", 0.48, "Gathering holy overtones with a soft upward breath."),
    SfxSpec("castlight", "magic", 0.44, "A clean, bright formula intake before radiant release."),
    SfxSpec("castember", "magic", 0.46, "Rising ember grit and a restrained ignition sweep."),
    SfxSpec("castfrost", "magic", 0.48, "Descending crystalline scrape with a cold glass tail."),
    SfxSpec("castshock", "magic", 0.40, "Tight electrical gather with a fast unstable rise."),
    SfxSpec("castnature", "magic", 0.54, "Wood strain, leaf breath, and a low living resonance."),
    SfxSpec("casthex", "magic", 0.54, "Descending whisper texture over a hollow ritual tone."),
    SfxSpec("castpact", "magic", 0.60, "Low rift pressure and an uneasy widening harmonic."),
    SfxSpec("castdeathburst", "magic", 0.72, "A deep inward pull before a death-magic burst."),
    SfxSpec("deathburst", "magic", 0.92, "Hollow sub impact, torn air, and a long dying resonance."),
    SfxSpec("castgreatersummon", "magic", 0.80, "Layered ritual pressure gathering around a greater summoning."),
    SfxSpec("greatersummon", "magic", 1.02, "A heavy rift opening with a broad low arrival."),
    SfxSpec("castascendance", "magic", 0.82, "Unstable rift harmonics climbing around the caster."),
    SfxSpec("ascendance", "magic", 1.04, "A dark transformation bloom with a controlled sub tail."),
    SfxSpec("casttempest", "magic", 0.66, "Electrical threads gathering into an elder storm."),
    SfxSpec("castveil", "magic", 0.54, "A narrow phase sweep that folds inward."),
    SfxSpec("veilstep", "magic", 0.48, "Fast spatial displacement with a crisp re-entry spark."),
    SfxSpec("castseal", "magic", 0.62, "Three bright binding tones gathering around a hostile rift."),
    SfxSpec("riftseal", "magic", 0.78, "A descending closure sweep ending in a firm radiant lock."),
    SfxSpec("castshimmer", "magic", 0.34, "A quiet high release shimmer for epic formula casts."),
    SfxSpec("impactlow", "magic", 0.46, "Short low-frequency reinforcement beneath major physical impacts."),
    SfxSpec("resonance", "magic", 0.52, "A compact harmonic echo for battlefield reactions."),
    SfxSpec("riftpounce", "magic", 0.54, "A compressed rift intake accelerating into a predatory leap."),
    SfxSpec("riftpounceimpact", "magic", 0.72, "A low landing blow split by a sharp spatial rupture."),
    SfxSpec("abyssalwhirl", "magic", 0.68, "A rotating abyssal wind with a dark cyclic blade contour."),
    SfxSpec("abyssalwhirlimpact", "magic", 0.82, "Three circling cuts collapse into a restrained infernal impact."),
    SfxSpec("soulrend", "magic", 0.62, "An inward spectral pull tightening toward a tearing release."),
    SfxSpec("soulrendimpact", "magic", 0.86, "Layered spirit fibers tear downward over a hollow body hit."),
    SfxSpec("dreadroar", "magic", 0.78, "A synthetic demonic throat rises through breath and rift harmonics."),
    SfxSpec("dreadroarimpact", "magic", 0.96, "A broad concussive roar wave with a deep controlled resonance."),
    SfxSpec("wayfind", "world", 0.54, "Two-note navigation discovery chime."),
    SfxSpec("shrine", "world", 0.92, "Weathered shrine resonance with a long, sparse tail."),
    SfxSpec("encounter", "world", 0.62, "Low warning drum and rising tension scrape."),
    SfxSpec("victory", "world", 0.88, "Compact resolved three-note combat stinger."),
    SfxSpec("defeat", "world", 1.04, "Descending combat stinger with a muted final drum."),
    SfxSpec("dialogueopen", "world", 0.28, "Quiet parchment-and-tone opening cue."),
    SfxSpec("dialogueclose", "world", 0.24, "Reversed conversational close cue."),
    SfxSpec("doorroyal", "world", 0.92, "Heavy timber, iron latch, and stone-room tail."),
    SfxSpec("thronechime", "world", 0.94, "Ceremonial bronze interval for the royal hall."),
    SfxSpec("shopbell", "world", 0.48, "Small hand bell with a wooden counter reflection."),
    SfxSpec("footwater", "world", 0.24, "Shallow boot splash with a short stone reflection."),
    SfxSpec("woodcontact", "world", 0.26, "Dense wood strike and splinter tick."),
    SfxSpec("stonecontact", "world", 0.32, "Stone impact with grit and a compact room reflection."),
    SfxSpec("servicearmor", "world", 0.58, "Leather pull, buckle, and fitted plate click."),
    SfxSpec("serviceweapon", "world", 0.54, "Weapon draw, inspection ring, and controlled resheath."),
    SfxSpec("serviceenchant", "magic", 0.76, "Runic binding shimmer over a muted forge tap."),
    SfxSpec("uiopen", "world", 0.22, "A quiet upward parchment-and-bronze panel opening."),
    SfxSpec("uiclose", "world", 0.20, "A soft downward fold that closes an overlay cleanly."),
    SfxSpec("uiconfirm", "world", 0.30, "A grounded two-note confirmation without menu-game brightness."),
    SfxSpec("uitab", "world", 0.16, "A dry page-edge tick for changing tabs or map layers."),
    SfxSpec("itemequip", "world", 0.42, "Leather settle, buckle pull, and one controlled metal lock."),
    SfxSpec("itemtake", "world", 0.38, "Cloth, wood, and a small inventory clasp as loot enters the pack."),
    SfxSpec("elixir", "world", 0.62, "Cork, glass, liquid, and a restrained restorative shimmer."),
    SfxSpec("rest", "world", 0.88, "Hearth crackle, bedroll movement, and a low resolved camp tone."),
    SfxSpec("levelup", "world", 0.82, "A compact rising brass-and-string stinger shared by a party level gain."),
    SfxSpec("footglass", "world", 0.18, "A cautious boot crunch through small glass rubble."),
    SfxSpec("footmud", "world", 0.22, "A short wet pull and soft boot landing in fen mud."),
    SfxSpec("footash", "world", 0.18, "Dry ash compression with a faint granular slip."),
    SfxSpec("footgravel", "world", 0.18, "Loose quarry grit and two restrained stone ticks."),
    SfxSpec("ambcity", "ambient", 1.72, "Distant guarded-street murmur with sparse cart and bell detail."),
    SfxSpec("ambbell", "ambient", 1.80, "Temple bell heard across stone streets."),
    SfxSpec("ambmarket", "ambient", 1.76, "Muted market bed with cloth, crate, and distant voice-like contours."),
    SfxSpec("ambforge", "ambient", 1.78, "Three distant forge blows, fire, and shop-room reflections."),
    SfxSpec("ambgate", "ambient", 1.74, "Gate chain, wind through stone, and a remote guard movement."),
    SfxSpec("ambdrip", "ambient", 1.80, "Irregular cistern drops over a dark wet-room bed."),
    SfxSpec("ambwind", "ambient", 1.80, "Old-road wind with a low, broad spectral shape."),
    SfxSpec("ambdrum", "ambient", 1.78, "Two far-off hide-drum calls carried across the road."),
    SfxSpec("ambstone", "ambient", 1.76, "Quarry grit, one distant fall, and open-air reflections."),
    SfxSpec("ambrain", "ambient", 1.80, "Rain against timber and shutters without thunder masking dialogue."),
    SfxSpec("ambtavern", "ambient", 1.78, "Low room murmur, cup movement, and a single distant laugh contour."),
    SfxSpec("ambhearth", "ambient", 1.80, "Hearth crackle and low flame breath."),
    SfxSpec("ambgrove", "ambient", 1.82, "Leaves, old wood, and one distant natural chime."),
    SfxSpec("ambfen", "ambient", 1.84, "Wet ash, insects, and sparse fen bubbles."),
    SfxSpec("ambglass", "ambient", 1.80, "Fine wind and two brittle glass resonances."),
    SfxSpec("ambruin", "ambient", 1.82, "Open ruin wind, shifting grit, and a distant stone fall."),
    SfxSpec("ambcave", "ambient", 1.84, "Low cave breath with irregular close and distant drops."),
    SfxSpec("ambcamp", "ambient", 1.80, "Small fire, bedroll cloth, and a guarded wood tap."),
)


def stable_seed(name: str) -> int:
    return int.from_bytes(hashlib.sha256(name.encode("utf-8")).digest()[:8], "little")


def db(value: float) -> float:
    return 20.0 * math.log10(max(float(value), 1e-12))


def midi_to_hz(note: float) -> float:
    return 440.0 * (2.0 ** ((note - 69.0) / 12.0))


def equal_power_pan(pan: float) -> tuple[float, float]:
    angle = (max(-1.0, min(1.0, pan)) + 1.0) * math.pi / 4.0
    return math.cos(angle), math.sin(angle)


def envelope(
    frames: int,
    sample_rate: int,
    attack: float,
    release: float,
    decay: float = 0.0,
    sustain: float = 1.0,
) -> np.ndarray:
    env = np.ones(frames, dtype=np.float64)
    attack_frames = min(frames, max(1, int(attack * sample_rate)))
    release_frames = min(frames, max(1, int(release * sample_rate)))
    env[:attack_frames] = np.sin(np.linspace(0.0, math.pi / 2.0, attack_frames)) ** 2
    if decay > 0.0:
        decay_frames = min(max(0, frames - attack_frames), max(1, int(decay * sample_rate)))
        if decay_frames:
            env[attack_frames : attack_frames + decay_frames] = np.linspace(1.0, sustain, decay_frames)
            env[attack_frames + decay_frames :] = sustain
    env[-release_frames:] *= np.cos(np.linspace(0.0, math.pi / 2.0, release_frames)) ** 2
    return env


def colored_noise(frames: int, rng: np.random.Generator, color: float = 1.0) -> np.ndarray:
    """Return deterministic finite colored noise using spectral shaping."""
    white = rng.normal(0.0, 1.0, frames)
    spectrum = np.fft.rfft(white)
    frequencies = np.arange(spectrum.size, dtype=np.float64)
    shaping = np.ones_like(frequencies)
    shaping[1:] = 1.0 / np.power(frequencies[1:], color / 2.0)
    shaped = np.fft.irfft(spectrum * shaping, n=frames)
    shaped -= np.mean(shaped)
    peak = float(np.max(np.abs(shaped)))
    return shaped / max(peak, 1e-9)


def oscillator(
    frequency: float,
    frames: int,
    sample_rate: int,
    waveform: str = "sine",
    phase: float = 0.0,
    vibrato_depth: float = 0.0,
    vibrato_rate: float = 5.0,
) -> np.ndarray:
    t = np.arange(frames, dtype=np.float64) / sample_rate
    phase_curve = 2.0 * math.pi * frequency * t + phase
    if vibrato_depth:
        phase_curve += vibrato_depth * np.sin(2.0 * math.pi * vibrato_rate * t)
    if waveform == "triangle":
        return (2.0 / math.pi) * np.arcsin(np.sin(phase_curve))
    if waveform == "softsaw":
        return (
            np.sin(phase_curve)
            + 0.34 * np.sin(2.0 * phase_curve)
            + 0.16 * np.sin(3.0 * phase_curve)
            + 0.08 * np.sin(4.0 * phase_curve)
        ) / 1.58
    return np.sin(phase_curve)


def pluck(frequency: float, duration: float, sample_rate: int, rng: np.random.Generator) -> np.ndarray:
    frames = max(1, int(duration * sample_rate))
    t = np.arange(frames, dtype=np.float64) / sample_rate
    phase = rng.uniform(0.0, 2.0 * math.pi)
    body = (
        oscillator(frequency, frames, sample_rate, "sine", phase)
        + 0.46 * oscillator(frequency * 2.0, frames, sample_rate, "sine", phase * 0.7)
        + 0.22 * oscillator(frequency * 3.0, frames, sample_rate, "sine", phase * 1.3)
        + 0.10 * oscillator(frequency * 5.0, frames, sample_rate, "sine", phase * 0.4)
    )
    decay = np.exp(-t * (3.4 + frequency / 800.0))
    pick = colored_noise(frames, rng, 0.1) * np.exp(-t * 55.0) * 0.16
    return (body * decay + pick) * envelope(frames, sample_rate, 0.003, 0.08)


def bowed(frequency: float, duration: float, sample_rate: int, rng: np.random.Generator) -> np.ndarray:
    frames = max(1, int(duration * sample_rate))
    phase = rng.uniform(0.0, 2.0 * math.pi)
    tone = oscillator(frequency, frames, sample_rate, "softsaw", phase, 0.022, 4.7)
    tone += 0.16 * oscillator(frequency * 0.5, frames, sample_rate, "sine", phase * 0.8)
    bow_noise = colored_noise(frames, rng, 0.4) * 0.025
    return (tone + bow_noise) * envelope(frames, sample_rate, 0.16, 0.24)


def reed(frequency: float, duration: float, sample_rate: int, rng: np.random.Generator) -> np.ndarray:
    frames = max(1, int(duration * sample_rate))
    phase = rng.uniform(0.0, 2.0 * math.pi)
    tone = oscillator(frequency, frames, sample_rate, "sine", phase, 0.028, 5.2)
    tone += 0.18 * oscillator(frequency * 2.0, frames, sample_rate, "sine", phase * 0.5)
    breath = colored_noise(frames, rng, 0.2) * 0.035
    return (tone + breath) * envelope(frames, sample_rate, 0.07, 0.14)


def bell(frequency: float, duration: float, sample_rate: int, rng: np.random.Generator) -> np.ndarray:
    frames = max(1, int(duration * sample_rate))
    t = np.arange(frames, dtype=np.float64) / sample_rate
    phase = rng.uniform(0.0, 2.0 * math.pi)
    partials = ((1.0, 1.0, 2.2), (2.01, 0.48, 3.0), (2.72, 0.26, 3.8), (4.18, 0.14, 5.2))
    signal = np.zeros(frames, dtype=np.float64)
    for ratio, gain, damping in partials:
        signal += gain * np.sin(2.0 * math.pi * frequency * ratio * t + phase * ratio) * np.exp(-damping * t)
    strike = colored_noise(frames, rng, 0.0) * np.exp(-t * 65.0) * 0.08
    return (signal + strike) * envelope(frames, sample_rate, 0.002, 0.08)


def drum(
    frequency: float,
    duration: float,
    sample_rate: int,
    rng: np.random.Generator,
    hardness: float = 0.5,
) -> np.ndarray:
    frames = max(1, int(duration * sample_rate))
    t = np.arange(frames, dtype=np.float64) / sample_rate
    phase = 2.0 * math.pi * (frequency * t + (frequency * 1.6) * (1.0 - np.exp(-t * 24.0)) / 24.0)
    body = np.sin(phase) * np.exp(-t * (8.0 - 2.5 * hardness))
    skin = colored_noise(frames, rng, 0.2) * np.exp(-t * (32.0 - 10.0 * hardness))
    return (body + skin * (0.16 + 0.18 * hardness)) * envelope(frames, sample_rate, 0.001, 0.04)


def click(duration: float, sample_rate: int, rng: np.random.Generator, bright: bool = False) -> np.ndarray:
    frames = max(1, int(duration * sample_rate))
    t = np.arange(frames, dtype=np.float64) / sample_rate
    base = 1450.0 if bright else 620.0
    signal = oscillator(base, frames, sample_rate, "sine", rng.uniform(0, 2 * math.pi))
    signal += colored_noise(frames, rng, 0.0) * 0.55
    return signal * np.exp(-t * (48.0 if bright else 32.0)) * envelope(frames, sample_rate, 0.001, 0.02)


def mix_circular(
    mix: np.ndarray,
    mono: np.ndarray,
    start_frame: int,
    gain: float,
    pan: float = 0.0,
) -> None:
    frames = mix.shape[1]
    left_gain, right_gain = equal_power_pan(pan)
    indices = (np.arange(mono.size, dtype=np.int64) + start_frame) % frames
    np.add.at(mix[0], indices, mono * gain * left_gain)
    np.add.at(mix[1], indices, mono * gain * right_gain)


def mix_linear(mix: np.ndarray, mono: np.ndarray, start_frame: int, gain: float = 1.0) -> None:
    if start_frame >= mix.size:
        return
    end = min(mix.size, start_frame + mono.size)
    if end > start_frame:
        mix[start_frame:end] += mono[: end - start_frame] * gain


def circular_reverb(stereo: np.ndarray, sample_rate: int, wet: float, width: float = 1.0) -> np.ndarray:
    dry = stereo.copy()
    left = stereo[0].copy()
    right = stereo[1].copy()
    taps = ((0.071, 0.20), (0.113, 0.15), (0.173, 0.11), (0.257, 0.075), (0.389, 0.052))
    for delay_seconds, gain in taps:
        delay = max(1, int(delay_seconds * sample_rate))
        left += np.roll(dry[0], delay) * gain
        right += np.roll(dry[1], delay + int(0.009 * sample_rate)) * gain
        left += np.roll(dry[1], delay + int(0.013 * sample_rate)) * gain * 0.32 * width
        right += np.roll(dry[0], delay) * gain * 0.32 * width
    return dry * (1.0 - wet) + np.vstack((left, right)) * wet


def rotate_to_quiet_seam(stereo: np.ndarray, sample_rate: int) -> np.ndarray:
    window = max(64, int(0.04 * sample_rate))
    mono_energy = np.mean(stereo * stereo, axis=0)
    kernel = np.ones(window, dtype=np.float64) / window
    smoothed = np.convolve(np.r_[mono_energy, mono_energy[: window - 1]], kernel, mode="valid")
    seam = int(np.argmin(smoothed[: stereo.shape[1]]))
    return np.roll(stereo, -seam, axis=1)


def bridge_loop_seam(stereo: np.ndarray, sample_rate: int) -> np.ndarray:
    """Replace 24 ms around the wrap with a smooth cubic Hermite bridge."""
    result = stereo.copy()
    half = max(64, int(0.012 * sample_rate))
    total = half * 2
    u = np.linspace(0.0, 1.0, total, endpoint=False)
    h00 = 2 * u**3 - 3 * u**2 + 1
    h10 = u**3 - 2 * u**2 + u
    h01 = -2 * u**3 + 3 * u**2
    h11 = u**3 - u**2
    for channel in range(result.shape[0]):
        source = result[channel]
        y0 = source[-half]
        y1 = source[half]
        m0 = (source[-half + 1] - source[-half]) * total
        m1 = (source[half + 1] - source[half]) * total
        bridge = h00 * y0 + h10 * m0 + h01 * y1 + h11 * m1
        source[-half:] = bridge[:half]
        source[:half] = bridge[half:]
    return result


def master_audio(audio: np.ndarray, target_rms_db: float, peak_db: float = -3.0) -> np.ndarray:
    result = np.asarray(audio, dtype=np.float64)
    result -= np.mean(result, axis=-1, keepdims=True)
    rms = float(np.sqrt(np.mean(result * result)))
    if rms > 1e-9:
        result *= (10.0 ** (target_rms_db / 20.0)) / rms
    ceiling = 10.0 ** (peak_db / 20.0)
    peak = float(np.max(np.abs(result)))
    if peak > ceiling:
        # A ceiling-referenced tanh catches isolated transients without
        # inflating the body of sparse arrangements toward the peak.
        result = ceiling * np.tanh(result / ceiling)
    return np.clip(result, -ceiling, ceiling)


def scale_note(root_midi: int, mode: Sequence[int], degree: int, octave: int = 0) -> int:
    wrapped = degree % len(mode)
    octaves = degree // len(mode)
    return root_midi + mode[wrapped] + 12 * (octave + octaves)


def add_music_note(
    mix: np.ndarray,
    instrument: str,
    midi_note: int,
    start_beat: float,
    duration_beats: float,
    beat_seconds: float,
    gain: float,
    pan: float,
    rng: np.random.Generator,
) -> None:
    frequency = midi_to_hz(midi_note)
    duration = max(0.05, duration_beats * beat_seconds)
    if instrument == "bowed":
        signal = bowed(frequency, duration, MUSIC_SAMPLE_RATE, rng)
    elif instrument == "reed":
        signal = reed(frequency, duration, MUSIC_SAMPLE_RATE, rng)
    elif instrument == "bell":
        signal = bell(frequency, duration, MUSIC_SAMPLE_RATE, rng)
    else:
        signal = pluck(frequency, duration, MUSIC_SAMPLE_RATE, rng)
    mix_circular(mix, signal, int(start_beat * beat_seconds * MUSIC_SAMPLE_RATE), gain, pan)


def compose_tavern_title_track(spec: TrackSpec) -> np.ndarray:
    """Compose the longer, shaped title arrangement without changing other routes."""
    rng = np.random.default_rng(stable_seed(spec.cue + ":v1.80-title"))
    beat_seconds = 60.0 / spec.bpm
    total_beats = 48
    duration = total_beats * beat_seconds
    frames = int(round(duration * MUSIC_SAMPLE_RATE))
    mix = np.zeros((2, frames), dtype=np.float64)

    # The room is present but quiet: rain remains beyond the shutters while
    # close hearth texture keeps the title screen sheltered rather than wet.
    rain_left = lowpass(colored_noise(frames, rng, 0.52), 11) * 0.0085
    rain_right = np.roll(lowpass(colored_noise(frames, rng, 0.58), 13), 379) * 0.0080
    hearth_noise = colored_noise(frames, rng, 1.65)
    crackle_source = colored_noise(frames, rng, 0.08)
    crackle_mask = (np.abs(crackle_source) > 0.925).astype(np.float64)
    hearth_left = hearth_noise * crackle_mask * 0.024
    hearth_right = np.roll(hearth_noise, 107) * np.roll(crackle_mask, 83) * 0.022
    mix += np.vstack((rain_left + hearth_left, rain_right + hearth_right))

    progression = (0, 0, 3, 5, 4, 0, 6, 3, 5, 4, 3, 0)
    bar_levels = (0.72, 0.78, 0.84, 0.90, 0.94, 0.98, 1.06, 1.10, 1.04, 1.00, 0.92, 0.80)
    for bar, degree in enumerate(progression):
        bar_beat = float(bar * 4)
        level = bar_levels[bar]

        # A dark three-voice bow bed changes inversion as the road motif grows.
        for voice, chord_degree in enumerate((degree, degree + 2, degree + 4)):
            octave = -1 if voice == 0 else 0
            if bar in {6, 7, 8} and voice == 2:
                octave += 1
            note = scale_note(spec.root_midi, spec.mode, chord_degree, octave)
            add_music_note(
                mix,
                "bowed",
                note,
                bar_beat,
                4.20,
                beat_seconds,
                (0.031 + voice * 0.004) * level,
                (-0.46, -0.04, 0.42)[voice],
                rng,
            )

        bass_note = scale_note(spec.root_midi, spec.mode, degree, -2)
        add_music_note(mix, "bowed", bass_note, bar_beat, 3.80, beat_seconds, 0.078 * level, -0.16, rng)
        if bar >= 2:
            add_music_note(
                mix,
                "pluck",
                bass_note + 12,
                bar_beat + 2.45,
                0.70,
                beat_seconds,
                0.050 * level,
                0.10,
                rng,
            )

        # Weathered lute arpeggios enter after the two-bar oath-like opening,
        # leave small gaps for the melody, and thin again during the coda.
        if 2 <= bar <= 10:
            arpeggio = (degree, degree + 4, degree + 2, degree + 5)
            starts = (0.0, 1.35, 2.20, 3.15)
            arpeggio_gain = (0.044 if bar < 6 else 0.051) * level
            if bar == 10:
                arpeggio_gain *= 0.78
            for index, (local_beat, note_degree) in enumerate(zip(starts, arpeggio)):
                note = scale_note(spec.root_midi, spec.mode, note_degree, 0)
                add_music_note(
                    mix,
                    "pluck",
                    note,
                    bar_beat + local_beat,
                    0.58 if index < 2 else 0.46,
                    beat_seconds,
                    arpeggio_gain,
                    (-0.30, 0.27, -0.08, 0.34)[index],
                    rng,
                )

        # A frame-drum heartbeat arrives with the road and gains a soft pickup
        # only in the central lift; the opening and final bar remain spacious.
        if 2 <= bar <= 10:
            for local_beat, gain, pan in ((0.0, 0.118, -0.10), (2.0, 0.068, 0.14)):
                hit = drum(61.0 if local_beat == 0.0 else 82.0, 0.32, MUSIC_SAMPLE_RATE, rng, 0.48)
                mix_circular(
                    mix,
                    hit,
                    int((bar_beat + local_beat) * beat_seconds * MUSIC_SAMPLE_RATE),
                    gain * level,
                    pan,
                )
            if 5 <= bar <= 8:
                pickup = click(0.08, MUSIC_SAMPLE_RATE, rng, bright=False)
                mix_circular(
                    mix,
                    pickup,
                    int((bar_beat + 3.5) * beat_seconds * MUSIC_SAMPLE_RATE),
                    0.032 * level,
                    0.34,
                )

    # The same five-note "forged road" idea is heard as lute, then reed, then
    # returned by the lute in a shorter coda so the loop has a recognizable hook.
    motif_starts = (0.0, 0.78, 1.55, 2.52, 3.28, 4.58, 5.35, 6.55)
    motif_degrees = (0, 2, 4, 3, 2, 0, 6, 4)
    for phrase_start, instrument, gain, transpose in (
        (8.0, "pluck", 0.092, 0),
        (24.0, "reed", 0.078, 0),
    ):
        for index, (local_beat, degree) in enumerate(zip(motif_starts, motif_degrees)):
            note = scale_note(spec.root_midi, spec.mode, degree + transpose, 1 if instrument == "reed" else 0)
            add_music_note(
                mix,
                instrument,
                note,
                phrase_start + local_beat,
                0.68 if instrument == "pluck" else 0.86,
                beat_seconds,
                gain * (1.08 if index in {0, 4} else 1.0),
                (-0.26, 0.20, -0.08, 0.30)[index % 4],
                rng,
            )

    coda_starts = (0.0, 0.82, 1.62, 2.58, 3.42, 4.72, 5.58)
    coda_degrees = (0, 2, 4, 3, 2, 1, 0)
    for index, (local_beat, degree) in enumerate(zip(coda_starts, coda_degrees)):
        note = scale_note(spec.root_midi, spec.mode, degree, 0)
        add_music_note(
            mix,
            "pluck",
            note,
            40.0 + local_beat,
            0.72,
            beat_seconds,
            0.086 * (1.08 if index == 0 else 1.0),
            (-0.22, 0.18, -0.05, 0.24)[index % 4],
            rng,
        )

    # A restrained bowed answer makes the middle section feel composed rather
    # than merely layered, while three bell sparks mark the title's large beats.
    for start_beat, degree, duration_beats in ((28.0, 4, 3.4), (32.0, 5, 3.2), (36.0, 3, 3.5)):
        note = scale_note(spec.root_midi, spec.mode, degree, 1)
        add_music_note(mix, "bowed", note, start_beat, duration_beats, beat_seconds, 0.052, 0.36, rng)
    for start_beat, degree in ((7.55, 4), (23.55, 6), (39.55, 4)):
        note = scale_note(spec.root_midi, spec.mode, degree, 2)
        add_music_note(mix, "bell", note, start_beat, 1.35, beat_seconds, 0.036, 0.48, rng)

    # One low, far-off weather swell supports the central crest without reading
    # as foreground thunder or masking menu feedback.
    thunder_seconds = 3.1
    thunder_frames = int(thunder_seconds * MUSIC_SAMPLE_RATE)
    thunder_env = envelope(thunder_frames, MUSIC_SAMPLE_RATE, 0.62, 1.40)
    thunder = oscillator(43.0, thunder_frames, MUSIC_SAMPLE_RATE, "sine", rng.uniform(0.0, 2.0 * math.pi))
    thunder += lowpass(colored_noise(thunder_frames, rng, 1.15), 29) * 0.38
    thunder *= thunder_env
    thunder_start = int(31.0 * beat_seconds * MUSIC_SAMPLE_RATE)
    mix_circular(mix, thunder, thunder_start, 0.035, -0.34)
    mix_circular(mix, np.roll(thunder, 173), thunder_start, 0.030, 0.42)

    beat_positions = np.arange(frames, dtype=np.float64) / (MUSIC_SAMPLE_RATE * beat_seconds)
    dynamics = np.interp(
        beat_positions,
        (0.0, 8.0, 16.0, 24.0, 32.0, 40.0, 48.0),
        (0.78, 0.88, 0.97, 1.03, 1.08, 0.94, 0.78),
    )
    mix *= dynamics[np.newaxis, :]
    mix = circular_reverb(mix, MUSIC_SAMPLE_RATE, 0.19, 0.96)
    mid = (mix[0] + mix[1]) * 0.5
    side = (mix[0] - mix[1]) * 0.5
    mix = np.vstack((mid + side * 1.28, mid - side * 1.28))
    mix = bridge_loop_seam(mix, MUSIC_SAMPLE_RATE)
    return master_audio(mix, -20.0, -3.0)


def compose_track(spec: TrackSpec) -> np.ndarray:
    if spec.cue == "tavern_storm_hearth_ensemble_loop":
        return compose_tavern_title_track(spec)

    rng = np.random.default_rng(stable_seed(spec.cue))
    beat_seconds = 60.0 / spec.bpm
    duration = MUSIC_BEATS * beat_seconds
    frames = int(round(duration * MUSIC_SAMPLE_RATE))
    mix = np.zeros((2, frames), dtype=np.float64)

    # A quiet periodic bed keeps the score tactile while preserving space for SFX.
    bed_left = colored_noise(frames, rng, 1.25)
    bed_right = np.roll(colored_noise(frames, rng, 1.35), int(0.017 * MUSIC_SAMPLE_RATE))
    bed_gain = 0.010 if spec.palette not in {"sewer", "sewercombat", "tavern"} else 0.018
    mix += np.vstack((bed_left, bed_right)) * bed_gain

    for bar in range(8):
        bar_beat = float(bar * 4)
        degree = spec.progression[bar % len(spec.progression)]
        chord_degrees = (degree, degree + 2, degree + 4)
        for voice, chord_degree in enumerate(chord_degrees):
            note = scale_note(spec.root_midi, spec.mode, chord_degree, -1 if voice == 0 else 0)
            add_music_note(
                mix,
                "bowed",
                note,
                bar_beat,
                4.15,
                beat_seconds,
                0.038 + spec.energy * 0.018,
                (-0.42, 0.0, 0.42)[voice],
                rng,
            )

        bass_note = scale_note(spec.root_midi, spec.mode, degree, -2)
        bass_events = (0.0, 2.0) if spec.energy < 0.85 else (0.0, 1.5, 2.0, 3.5)
        for local_beat in bass_events:
            add_music_note(
                mix,
                "pluck" if spec.palette in {"kobold", "king"} else "bowed",
                bass_note + (12 if local_beat == 3.5 else 0),
                bar_beat + local_beat,
                0.72 if spec.energy > 0.7 else 1.45,
                beat_seconds,
                0.075 + spec.energy * 0.035,
                -0.12,
                rng,
            )

    melody_patterns = {
        "tavern": (0, 2, 4, 3, 2, 0, 6, 4, 0, 3, 5, 4, 2, 1, 0, -1),
        "muster": (0, -1, 2, 3, 4, -1, 3, 2, 0, -1, 4, 5, 3, 2, 0, -1),
        "city": (0, 2, 4, -1, 3, 5, 4, 2, 0, 3, 5, -1, 4, 3, 2, -1),
        "throne": (0, -1, 4, -1, 5, 4, 2, -1, 0, 3, 6, -1, 5, 4, 2, -1),
        "merchant": (0, 2, 4, 5, 3, 2, 0, -1, 3, 5, 4, 2, 1, 2, 3, -1),
        "road": (0, 2, 3, 4, 2, -1, 0, 6, 0, 3, 4, 5, 4, 2, 1, -1),
        "sewer": (0, -1, 1, -1, 5, -1, 3, -1, 0, -1, 6, -1, 1, -1, 0, -1),
        "dusk": (0, 1, 3, -1, 5, 3, 1, -1, 0, 5, 6, 3, 1, 4, 3, -1),
        "cave": (0, -1, 1, -1, 4, -1, 3, -1, 0, -1, 5, -1, 1, -1, 0, -1),
        "combat": (0, 2, 3, 5, 4, 3, 6, 5, 0, 3, 5, 6, 4, 2, 1, 3),
        "sewercombat": (0, 1, 3, 5, 1, 6, 5, 3, 0, 3, 1, 5, 6, 3, 1, 0),
        "ratfolk": (0, 1, 3, 5, 1, 3, 6, 5, 0, 3, 1, 6, 5, 3, 1, 0),
        "kobold": (0, 3, 2, 5, 0, 6, 3, 4, 2, 5, 3, 6, 4, 2, 1, 3),
        "boss": (0, 2, 4, 6, 5, 4, 3, 2, 0, 3, 5, 7, 6, 4, 2, 1),
        "king": (0, 3, 5, 7, 6, 4, 2, 5, 0, 4, 6, 8, 7, 5, 3, 1),
        "victory": (0, 2, 4, 5, 4, 3, 2, 0, 3, 5, 7, 6, 5, 4, 2, 0),
        "defeat": (5, -1, 4, -1, 3, -1, 2, -1, 4, -1, 3, -1, 1, -1, 0, -1),
    }
    pattern = melody_patterns[spec.palette]
    step = 0.5 if spec.energy >= 0.72 else 1.0
    repetitions = int(MUSIC_BEATS / (len(pattern) * step)) + 1
    lead_instrument = "reed" if spec.palette in {"road", "muster", "victory", "dusk"} else "pluck"
    if spec.palette in {"city", "sewer", "cave", "defeat", "throne"}:
        lead_instrument = "bell"
    for index, degree in enumerate(pattern * repetitions):
        start_beat = index * step
        if start_beat >= MUSIC_BEATS or degree < 0:
            continue
        octave = 1 if spec.palette in {"city", "victory", "king"} else 0
        note = scale_note(spec.root_midi, spec.mode, degree, octave)
        duration_beats = 0.42 if step == 0.5 else 0.82
        add_music_note(
            mix,
            lead_instrument,
            note,
            start_beat,
            duration_beats,
            beat_seconds,
            0.050 + spec.energy * 0.038,
            (-0.32, 0.28, -0.08, 0.36)[index % 4],
            rng,
        )

    # Percussion is sparse in travel music and deliberately explicit in combat.
    for beat_index in range(MUSIC_BEATS):
        downbeat = beat_index % 4 == 0
        half_bar = beat_index % 4 == 2
        should_hit = downbeat or (spec.energy > 0.55 and half_bar) or spec.energy > 0.88
        if should_hit:
            drum_frequency = 62.0 if downbeat else 84.0
            hit = drum(drum_frequency, 0.30, MUSIC_SAMPLE_RATE, rng, 0.72 if spec.energy > 0.7 else 0.42)
            mix_circular(
                mix,
                hit,
                int(beat_index * beat_seconds * MUSIC_SAMPLE_RATE),
                (0.105 if downbeat else 0.064) * (0.55 + spec.energy),
                -0.08 if downbeat else 0.16,
            )
        if spec.palette in {"kobold", "king", "combat", "sewercombat", "ratfolk", "dusk"}:
            for offset in (0.5, 1.5):
                if beat_index + offset >= MUSIC_BEATS:
                    continue
                tick = click(0.09, MUSIC_SAMPLE_RATE, rng, bright=spec.palette in {"kobold", "king"})
                mix_circular(
                    mix,
                    tick,
                    int((beat_index + offset) * beat_seconds * MUSIC_SAMPLE_RATE),
                    0.035 + spec.energy * 0.028,
                    0.38 if int(offset * 2) % 2 else -0.38,
                )

    # Palette-specific story details.
    if spec.palette == "tavern":
        rain = colored_noise(frames, rng, 0.35) * 0.014
        hearth = colored_noise(frames, rng, 1.7)
        crackle_mask = (np.abs(colored_noise(frames, rng, 0.1)) > 0.91).astype(np.float64)
        hearth = hearth * crackle_mask * 0.028
        mix += np.vstack((rain + hearth, np.roll(rain, 311) + np.roll(hearth, 97)))
    elif spec.palette in {"sewer", "sewercombat", "cave"}:
        for event_beat in (1.4, 6.7, 10.2, 15.8, 21.3, 26.1, 30.4):
            drip_note = bell(midi_to_hz(spec.root_midi + 24 + int(event_beat) % 5), 0.32, MUSIC_SAMPLE_RATE, rng)
            mix_circular(
                mix,
                drip_note,
                int(event_beat * beat_seconds * MUSIC_SAMPLE_RATE),
                0.026,
                math.sin(event_beat) * 0.72,
            )
    elif spec.palette in {"boss", "king"}:
        for bar in range(8):
            note = scale_note(spec.root_midi, spec.mode, spec.progression[bar] + 4, 0)
            add_music_note(mix, "bowed", note, bar * 4 + 3.0, 1.2, beat_seconds, 0.072, 0.34, rng)

    wet = 0.23 if spec.palette in {"sewer", "sewercombat", "boss", "defeat"} else 0.16
    mix = circular_reverb(mix, MUSIC_SAMPLE_RATE, wet, 0.9)
    mix = rotate_to_quiet_seam(mix, MUSIC_SAMPLE_RATE)
    mix = bridge_loop_seam(mix, MUSIC_SAMPLE_RATE)
    return master_audio(mix, -22.5 + spec.energy * 4.0, -3.0)


def chirp(
    start_hz: float,
    end_hz: float,
    duration: float,
    sample_rate: int,
    curve: float = 1.0,
    phase: float = 0.0,
) -> np.ndarray:
    frames = max(1, int(duration * sample_rate))
    t = np.linspace(0.0, 1.0, frames, endpoint=False)
    frequency = start_hz + (end_hz - start_hz) * np.power(t, curve)
    phases = phase + 2.0 * math.pi * np.cumsum(frequency) / sample_rate
    return np.sin(phases)


def lowpass(signal: np.ndarray, amount: int) -> np.ndarray:
    if amount <= 1:
        return signal
    kernel = np.ones(amount, dtype=np.float64) / amount
    return np.convolve(signal, kernel, mode="same")


def one_shot_reverb(mono: np.ndarray, sample_rate: int, wet: float = 0.16) -> np.ndarray:
    dry = mono.copy()
    result = dry.copy()
    for seconds, gain in ((0.043, 0.22), (0.079, 0.15), (0.137, 0.10), (0.223, 0.055)):
        delay = int(seconds * sample_rate)
        if delay < mono.size:
            result[delay:] += dry[:-delay] * gain
    return dry * (1.0 - wet) + result * wet


DEMON_ABILITY_SFX = frozenset(
    {
        "riftpounce",
        "riftpounceimpact",
        "abyssalwhirl",
        "abyssalwhirlimpact",
        "soulrend",
        "soulrendimpact",
        "dreadroar",
        "dreadroarimpact",
    }
)


def demon_ability_sfx(spec: SfxSpec, rng: np.random.Generator) -> np.ndarray:
    """Synthesize distinct, mix-safe movement and impact identities for demon form."""
    frames = int(spec.duration * SFX_SAMPLE_RATE)
    t = np.arange(frames, dtype=np.float64) / SFX_SAMPLE_RATE
    progress = np.arange(frames, dtype=np.float64) / max(1, frames - 1)
    phase = rng.uniform(0.0, 2.0 * math.pi)
    noise = colored_noise(frames, rng, 0.24)
    low_noise = lowpass(noise, 31)
    edge_noise = noise - lowpass(noise, 7)
    signal = np.zeros(frames, dtype=np.float64)
    cue = spec.cue

    if cue == "riftpounce":
        travel = np.power(np.sin(progress * math.pi), 0.44)
        signal += chirp(74.0, 1180.0, spec.duration, SFX_SAMPLE_RATE, 1.65, phase) * travel * 0.31
        signal += chirp(920.0, 146.0, spec.duration, SFX_SAMPLE_RATE, 0.58, phase * 0.63) * travel * 0.14
        signal += edge_noise * travel * (0.12 + progress * 0.22)
        signal += oscillator(46.0, frames, SFX_SAMPLE_RATE, "sine", phase * 0.31) * np.exp(-t * 9.0) * 0.24
        mix_linear(signal, click(0.12, SFX_SAMPLE_RATE, rng, bright=True), int(spec.duration * 0.72 * SFX_SAMPLE_RATE), 0.24)
    elif cue == "riftpounceimpact":
        body = np.exp(-t * 5.2)
        signal += chirp(710.0, 38.0, spec.duration, SFX_SAMPLE_RATE, 0.54, phase) * body * 0.28
        signal += low_noise * np.exp(-t * 4.0) * 0.22
        signal += drum(49.0, spec.duration, SFX_SAMPLE_RATE, rng, 0.94) * 0.78
        mix_linear(signal, click(0.16, SFX_SAMPLE_RATE, rng, bright=True), int(0.028 * SFX_SAMPLE_RATE), 0.34)
        mix_linear(signal, bell(116.5, spec.duration - 0.12, SFX_SAMPLE_RATE, rng), int(0.12 * SFX_SAMPLE_RATE), 0.12)
    elif cue == "abyssalwhirl":
        rotation = 0.48 + 0.52 * np.power(np.sin(progress * math.pi * 5.0), 2.0)
        broad = np.power(np.sin(progress * math.pi), 0.42)
        signal += chirp(430.0, 116.0, spec.duration, SFX_SAMPLE_RATE, 0.88, phase) * broad * rotation * 0.24
        signal += edge_noise * broad * rotation * 0.34
        signal += oscillator(58.0, frames, SFX_SAMPLE_RATE, "softsaw", phase * 0.42, 0.11, 5.0) * broad * 0.18
        signal += chirp(1380.0, 320.0, spec.duration, SFX_SAMPLE_RATE, 1.1, phase * 0.77) * broad * 0.08
    elif cue == "abyssalwhirlimpact":
        broad = np.power(1.0 - progress, 0.46)
        signal += edge_noise * broad * 0.23
        signal += oscillator(52.0, frames, SFX_SAMPLE_RATE, "softsaw", phase * 0.36) * np.exp(-t * 5.0) * 0.19
        for index, offset in enumerate((0.00, 0.17, 0.34)):
            duration = spec.duration - offset
            cut = chirp(1320.0 - index * 150.0, 76.0 - index * 9.0, duration, SFX_SAMPLE_RATE, 0.62, phase + index)
            cut *= np.exp(-np.arange(cut.size, dtype=np.float64) / SFX_SAMPLE_RATE * 8.6)
            mix_linear(signal, cut, int(offset * SFX_SAMPLE_RATE), 0.25 - index * 0.025)
        mix_linear(signal, drum(46.0, spec.duration - 0.28, SFX_SAMPLE_RATE, rng, 0.86), int(0.28 * SFX_SAMPLE_RATE), 0.58)
    elif cue == "soulrend":
        gather = np.power(np.sin(progress * math.pi * 0.78), 0.64)
        tightening = np.clip(progress / 0.78, 0.0, 1.0)
        signal += chirp(62.0, 1040.0, spec.duration, SFX_SAMPLE_RATE, 1.82, phase) * gather * 0.25
        signal += chirp(460.0, 92.0, spec.duration, SFX_SAMPLE_RATE, 0.72, phase * 0.58) * gather * 0.16
        signal += edge_noise * gather * (0.08 + tightening * 0.27)
        signal += oscillator(43.0, frames, SFX_SAMPLE_RATE, "sine", phase * 0.27) * gather * (1.0 - progress) * 0.18
        mix_linear(signal, click(0.10, SFX_SAMPLE_RATE, rng, bright=False), int(spec.duration * 0.79 * SFX_SAMPLE_RATE), 0.19)
    elif cue == "soulrendimpact":
        dying = np.exp(-t * 3.6)
        signal += chirp(1020.0, 42.0, spec.duration, SFX_SAMPLE_RATE, 0.72, phase) * dying * 0.26
        signal += chirp(630.0, 27.0, spec.duration, SFX_SAMPLE_RATE, 0.48, phase * 0.69) * dying * 0.17
        tear_gate = np.minimum(1.0, t / 0.018) * np.exp(-t * 6.8)
        signal += edge_noise * tear_gate * 0.38 + low_noise * dying * 0.15
        signal += drum(57.0, spec.duration, SFX_SAMPLE_RATE, rng, 0.62) * 0.48
        mix_linear(signal, bell(174.6, spec.duration - 0.16, SFX_SAMPLE_RATE, rng), int(0.16 * SFX_SAMPLE_RATE), 0.10)
    elif cue == "dreadroar":
        rise = np.power(np.sin(progress * math.pi * 0.72), 0.68)
        throat = oscillator(61.0, frames, SFX_SAMPLE_RATE, "softsaw", phase, 0.19, 27.0)
        formant_a = oscillator(97.0, frames, SFX_SAMPLE_RATE, "sine", phase * 0.47, 0.08, 9.0)
        formant_b = oscillator(151.0, frames, SFX_SAMPLE_RATE, "sine", phase * 0.73, 0.05, 13.0)
        breath = low_noise * 0.24 + edge_noise * 0.12
        signal += (throat * 0.31 + formant_a * 0.18 + formant_b * 0.10 + breath) * rise
        signal += chirp(48.0, 88.0, spec.duration, SFX_SAMPLE_RATE, 1.25, phase * 0.22) * rise * 0.17
    elif cue == "dreadroarimpact":
        shock = np.exp(-t * 3.5)
        throat = oscillator(52.0, frames, SFX_SAMPLE_RATE, "softsaw", phase, 0.15, 22.0)
        formants = (
            oscillator(79.0, frames, SFX_SAMPLE_RATE, "sine", phase * 0.51) * 0.18
            + oscillator(121.0, frames, SFX_SAMPLE_RATE, "sine", phase * 0.83) * 0.11
        )
        signal += (throat * 0.31 + formants + low_noise * 0.25 + edge_noise * 0.13) * shock
        signal += chirp(240.0, 34.0, spec.duration, SFX_SAMPLE_RATE, 0.68, phase * 0.39) * shock * 0.22
        signal += drum(42.0, spec.duration, SFX_SAMPLE_RATE, rng, 0.92) * 0.68
        mix_linear(signal, bell(73.4, spec.duration - 0.20, SFX_SAMPLE_RATE, rng), int(0.20 * SFX_SAMPLE_RATE), 0.13)
    else:
        raise ValueError(f"Unhandled demon ability cue: {cue}")

    attack = 0.012 if cue in {"riftpounce", "abyssalwhirl", "soulrend", "dreadroar"} else 0.002
    shaped = signal * envelope(frames, SFX_SAMPLE_RATE, attack, min(0.24, spec.duration * 0.30), sustain=0.78)
    return one_shot_reverb(shaped, SFX_SAMPLE_RATE, 0.17 if cue.endswith("impact") else 0.13)


def magical_sfx(spec: SfxSpec, rng: np.random.Generator) -> np.ndarray:
    if spec.cue in DEMON_ABILITY_SFX:
        return demon_ability_sfx(spec, rng)

    frames = int(spec.duration * SFX_SAMPLE_RATE)
    t = np.arange(frames, dtype=np.float64) / SFX_SAMPLE_RATE
    profiles = {
        "heal": (310, 910, 1.35, 0.18),
        "ward": (220, 620, 0.82, 0.22),
        "light": (720, 1580, 0.72, 0.12),
        "curse": (390, 68, 1.18, 0.24),
        "death": (230, 38, 0.82, 0.30),
        "web": (780, 150, 0.56, 0.14),
        "poison": (190, 112, 1.3, 0.22),
        "fieldgas": (145, 220, 0.75, 0.30),
        "fieldsnare": (420, 92, 0.64, 0.20),
        "fieldholy": (380, 1040, 1.1, 0.22),
        "fieldcurse": (280, 54, 0.9, 0.28),
        "spellrelease": (480, 1280, 0.72, 0.14),
        "serviceenchant": (310, 1180, 1.0, 0.20),
        "castmend": (260, 880, 1.1, 0.14),
        "castlight": (580, 1450, 0.8, 0.10),
        "castember": (140, 980, 1.45, 0.23),
        "castfrost": (1280, 340, 0.75, 0.16),
        "castshock": (1550, 240, 1.7, 0.22),
        "castnature": (120, 420, 1.05, 0.18),
        "casthex": (460, 55, 1.3, 0.23),
        "castpact": (88, 380, 0.9, 0.28),
        "castdeathburst": (340, 42, 0.78, 0.30),
        "deathburst": (220, 32, 0.72, 0.34),
        "castgreatersummon": (72, 440, 0.86, 0.30),
        "greatersummon": (55, 270, 0.78, 0.34),
        "castascendance": (96, 620, 0.9, 0.28),
        "ascendance": (82, 760, 0.82, 0.32),
        "casttempest": (960, 1800, 1.25, 0.24),
        "castveil": (920, 160, 1.1, 0.22),
        "veilstep": (1420, 210, 1.5, 0.18),
        "castseal": (380, 1320, 0.85, 0.16),
        "riftseal": (1100, 220, 1.3, 0.20),
        "castshimmer": (720, 1460, 0.85, 0.10),
        "impactlow": (92, 38, 0.68, 0.32),
        "resonance": (520, 140, 0.85, 0.18),
    }
    start_hz, end_hz, curve, noise_gain = profiles[spec.cue]
    phase = rng.uniform(0.0, 2.0 * math.pi)
    primary = chirp(start_hz, end_hz, spec.duration, SFX_SAMPLE_RATE, curve, phase)
    overtone = chirp(start_hz * 1.51, end_hz * 2.01, spec.duration, SFX_SAMPLE_RATE, curve * 0.9, phase * 0.7)
    noise = colored_noise(frames, rng, 0.45)
    if spec.cue in {"web", "fieldsnare", "castnature"}:
        noise = lowpass(noise, 5)
        pulse = np.maximum(0.0, np.sin(2.0 * math.pi * (18.0 + 22.0 * t) * t))
        signal = primary * 0.38 + noise * noise_gain * pulse
    elif spec.cue in {"poison", "fieldgas"}:
        bubbles = np.sin(2.0 * math.pi * (7.0 + 5.0 * np.sin(t * 9.0)) * t)
        signal = primary * 0.28 + overtone * 0.10 + noise * noise_gain + bubbles * 0.12
    else:
        signal = primary * 0.52 + overtone * 0.22 + noise * noise_gain
    env = envelope(frames, SFX_SAMPLE_RATE, 0.008, min(0.28, spec.duration * 0.38))
    if spec.cue in {
        "curse",
        "death",
        "fieldcurse",
        "casthex",
        "castpact",
        "castdeathburst",
        "deathburst",
        "castgreatersummon",
        "greatersummon",
        "castascendance",
        "ascendance",
    }:
        signal += np.sin(2.0 * math.pi * 48.0 * t) * np.exp(-t * 3.2) * 0.25
    if spec.cue in {
        "heal",
        "ward",
        "light",
        "fieldholy",
        "serviceenchant",
        "castmend",
        "castlight",
        "castseal",
        "riftseal",
        "castshimmer",
    }:
        for offset, ratio in ((0.08, 1.0), (0.18, 1.25), (0.29, 1.5)):
            start = int(offset * SFX_SAMPLE_RATE)
            if start < frames:
                tone = bell(end_hz * ratio * 0.5, spec.duration - offset, SFX_SAMPLE_RATE, rng)
                count = min(frames - start, tone.size)
                signal[start : start + count] += tone[:count] * 0.13
    if spec.cue == "impactlow":
        signal += drum(54.0, spec.duration, SFX_SAMPLE_RATE, rng, 0.88) * 0.72
    elif spec.cue == "resonance":
        signal += bell(246.94, spec.duration, SFX_SAMPLE_RATE, rng) * 0.34
    return one_shot_reverb(signal * env, SFX_SAMPLE_RATE, 0.20)


def world_sfx(spec: SfxSpec, rng: np.random.Generator) -> np.ndarray:
    frames = int(spec.duration * SFX_SAMPLE_RATE)
    t = np.arange(frames, dtype=np.float64) / SFX_SAMPLE_RATE
    signal = np.zeros(frames, dtype=np.float64)

    if spec.cue in {"wayfind", "shrine", "thronechime", "shopbell", "victory", "defeat", "levelup"}:
        note_sets = {
            "wayfind": ((0.00, 523.25), (0.18, 783.99)),
            "shrine": ((0.00, 293.66), (0.21, 440.00), (0.43, 587.33)),
            "thronechime": ((0.00, 196.00), (0.16, 293.66), (0.39, 392.00)),
            "shopbell": ((0.00, 987.77), (0.13, 1318.51)),
            "victory": ((0.00, 392.00), (0.19, 493.88), (0.40, 587.33)),
            "defeat": ((0.00, 329.63), (0.23, 246.94), (0.50, 164.81)),
            "levelup": ((0.00, 293.66), (0.15, 392.00), (0.31, 493.88), (0.50, 659.25)),
        }
        for offset, frequency in note_sets[spec.cue]:
            tone = bell(frequency, max(0.08, spec.duration - offset), SFX_SAMPLE_RATE, rng)
            mix_linear(signal, tone, int(offset * SFX_SAMPLE_RATE), 0.36)
        if spec.cue in {"victory", "defeat", "thronechime", "levelup"}:
            hit = drum(62.0 if spec.cue == "defeat" else 78.0, 0.34, SFX_SAMPLE_RATE, rng, 0.55)
            mix_linear(signal, hit, int(0.02 * SFX_SAMPLE_RATE), 0.32)
    elif spec.cue in {"dialogueopen", "dialogueclose", "uiopen", "uiclose", "uitab"}:
        paper = colored_noise(frames, rng, 0.35)
        paper *= np.sin(np.linspace(0.0, math.pi, frames)) ** 2
        opens = spec.cue in {"dialogueopen", "uiopen"}
        if spec.cue == "uitab":
            tone = chirp(610, 760, spec.duration, SFX_SAMPLE_RATE, 1.4)
            paper_gain = 0.34
            tone_gain = 0.12
        else:
            tone = chirp(310 if opens else 520, 560 if opens else 260, spec.duration, SFX_SAMPLE_RATE, 0.9)
            paper_gain = 0.30
            tone_gain = 0.18
        signal = paper * paper_gain + tone * envelope(frames, SFX_SAMPLE_RATE, 0.012, 0.08) * tone_gain
    elif spec.cue == "uiconfirm":
        for offset, frequency in ((0.00, 392.0), (0.105, 587.33)):
            tone = bell(frequency, max(0.08, spec.duration - offset), SFX_SAMPLE_RATE, rng)
            mix_linear(signal, tone, int(offset * SFX_SAMPLE_RATE), 0.28)
        soft_hit = drum(92.0, 0.18, SFX_SAMPLE_RATE, rng, 0.38)
        mix_linear(signal, soft_hit, 0, 0.18)
    elif spec.cue == "encounter":
        hit = drum(54.0, 0.42, SFX_SAMPLE_RATE, rng, 0.82)
        scrape = chirp(180.0, 720.0, spec.duration, SFX_SAMPLE_RATE, 1.7)
        signal = scrape * envelope(frames, SFX_SAMPLE_RATE, 0.06, 0.12) * 0.18
        mix_linear(signal, hit, 0, 0.72)
    elif spec.cue == "doorroyal":
        timber = lowpass(colored_noise(frames, rng, 1.1), 21)
        groan = chirp(132.0, 46.0, spec.duration, SFX_SAMPLE_RATE, 0.7)
        signal = timber * np.exp(-t * 2.4) * 0.34 + groan * envelope(frames, SFX_SAMPLE_RATE, 0.015, 0.24) * 0.28
        latch = click(0.18, SFX_SAMPLE_RATE, rng, bright=False)
        mix_linear(signal, latch, int(0.12 * SFX_SAMPLE_RATE), 0.45)
    elif spec.cue == "footwater":
        splash = colored_noise(frames, rng, 0.5)
        splash *= np.exp(-t * 12.0)
        low = chirp(180.0, 70.0, spec.duration, SFX_SAMPLE_RATE, 0.7)
        signal = splash * 0.48 + low * np.exp(-t * 15.0) * 0.18
    elif spec.cue in {"footglass", "footmud", "footash", "footgravel"}:
        noise_color = 0.12 if spec.cue == "footglass" else 0.82 if spec.cue == "footmud" else 0.52
        texture = colored_noise(frames, rng, noise_color)
        texture *= np.exp(-t * (13.0 if spec.cue == "footmud" else 20.0))
        signal = texture * (0.32 if spec.cue == "footmud" else 0.24)
        body_hz = 82.0 if spec.cue == "footmud" else 118.0 if spec.cue == "footash" else 156.0
        body = drum(body_hz, spec.duration, SFX_SAMPLE_RATE, rng, 0.36)
        mix_linear(signal, body, 0, 0.28)
        if spec.cue in {"footglass", "footgravel"}:
            for offset, frequency in ((0.035, 1460.0), (0.090, 980.0)):
                tick = bell(frequency, min(0.09, spec.duration - offset), SFX_SAMPLE_RATE, rng)
                mix_linear(signal, tick, int(offset * SFX_SAMPLE_RATE), 0.07 if spec.cue == "footglass" else 0.045)
    elif spec.cue in {"woodcontact", "stonecontact"}:
        tone_hz = 170.0 if spec.cue == "woodcontact" else 92.0
        body = drum(tone_hz, spec.duration, SFX_SAMPLE_RATE, rng, 0.74)
        grit = colored_noise(frames, rng, 0.1) * np.exp(-t * (18.0 if spec.cue == "woodcontact" else 10.0))
        signal = body * 0.65 + grit * (0.28 if spec.cue == "stonecontact" else 0.20)
        for offset in (0.035, 0.073):
            tick = click(0.08, SFX_SAMPLE_RATE, rng, bright=spec.cue == "stonecontact")
            mix_linear(signal, tick, int(offset * SFX_SAMPLE_RATE), 0.18)
    elif spec.cue in {"servicearmor", "serviceweapon", "itemequip"}:
        cloth = colored_noise(frames, rng, 0.8) * envelope(frames, SFX_SAMPLE_RATE, 0.01, 0.12)
        signal = cloth * 0.18
        offsets = (0.12, 0.29, 0.41) if spec.cue in {"servicearmor", "itemequip"} else (0.05, 0.22, 0.43)
        for index, offset in enumerate(offsets):
            metal = bell(480.0 + index * 170.0, min(0.28, spec.duration - offset), SFX_SAMPLE_RATE, rng)
            mix_linear(signal, metal, int(offset * SFX_SAMPLE_RATE), 0.18 if index else 0.28)
    elif spec.cue == "itemtake":
        cloth = lowpass(colored_noise(frames, rng, 0.65), 7)
        cloth *= envelope(frames, SFX_SAMPLE_RATE, 0.008, 0.10)
        signal = cloth * 0.28
        clasp = click(0.14, SFX_SAMPLE_RATE, rng, bright=True)
        wood = drum(142.0, 0.22, SFX_SAMPLE_RATE, rng, 0.42)
        mix_linear(signal, wood, int(0.035 * SFX_SAMPLE_RATE), 0.26)
        mix_linear(signal, clasp, int(0.19 * SFX_SAMPLE_RATE), 0.24)
    elif spec.cue == "elixir":
        liquid = colored_noise(frames, rng, 0.55)
        liquid = lowpass(liquid, 9) * (0.42 + 0.28 * np.sin(2.0 * math.pi * 7.0 * t))
        signal = liquid * envelope(frames, SFX_SAMPLE_RATE, 0.02, 0.14) * 0.28
        cork = click(0.12, SFX_SAMPLE_RATE, rng, bright=False)
        glass = bell(1046.5, 0.28, SFX_SAMPLE_RATE, rng)
        restore = bell(659.25, 0.38, SFX_SAMPLE_RATE, rng)
        mix_linear(signal, cork, int(0.02 * SFX_SAMPLE_RATE), 0.42)
        mix_linear(signal, glass, int(0.16 * SFX_SAMPLE_RATE), 0.12)
        mix_linear(signal, restore, int(0.30 * SFX_SAMPLE_RATE), 0.18)
    elif spec.cue == "rest":
        fire = colored_noise(frames, rng, 1.35)
        crackle = colored_noise(frames, rng, 0.05)
        crackle = np.where(np.abs(crackle) > 0.88, crackle, 0.0)
        cloth = lowpass(colored_noise(frames, rng, 0.75), 13)
        signal = fire * 0.075 + crackle * 0.16 + cloth * envelope(frames, SFX_SAMPLE_RATE, 0.08, 0.20) * 0.11
        settle = bell(196.0, 0.50, SFX_SAMPLE_RATE, rng)
        mix_linear(signal, settle, int(0.30 * SFX_SAMPLE_RATE), 0.14)
    else:
        raise ValueError(f"Unhandled world SFX cue: {spec.cue}")

    return one_shot_reverb(signal, SFX_SAMPLE_RATE, 0.18)


def ambient_sfx(spec: SfxSpec, rng: np.random.Generator) -> np.ndarray:
    frames = int(spec.duration * SFX_SAMPLE_RATE)
    t = np.arange(frames, dtype=np.float64) / SFX_SAMPLE_RATE
    brown = colored_noise(frames, rng, 1.55)
    pink = colored_noise(frames, rng, 0.85)
    signal = brown * 0.055 + pink * 0.018

    if spec.cue == "ambcity":
        murmur = np.sin(2.0 * math.pi * (83.0 + 4.0 * np.sin(t * 1.7)) * t) * 0.025
        signal += murmur
        cart = click(0.22, SFX_SAMPLE_RATE, rng, False)
        mix_linear(signal, cart, int(1.05 * SFX_SAMPLE_RATE), 0.18)
    elif spec.cue == "ambbell":
        signal *= 0.45
        for offset, frequency in ((0.12, 196.0), (0.72, 196.0), (1.31, 146.8)):
            tone = bell(frequency, min(2.1, spec.duration - offset), SFX_SAMPLE_RATE, rng)
            mix_linear(signal, tone, int(offset * SFX_SAMPLE_RATE), 0.34)
    elif spec.cue == "ambmarket":
        signal += lowpass(colored_noise(frames, rng, 0.4), 49) * 0.055
        for offset in (0.28, 0.91, 1.39):
            crate = click(0.18, SFX_SAMPLE_RATE, rng, False)
            mix_linear(signal, crate, int(offset * SFX_SAMPLE_RATE), 0.13)
    elif spec.cue == "ambforge":
        signal += colored_noise(frames, rng, 1.8) * 0.028
        for offset, frequency in ((0.14, 610.0), (0.67, 520.0), (1.24, 680.0)):
            strike = bell(frequency, min(0.9, spec.duration - offset), SFX_SAMPLE_RATE, rng)
            hit = drum(94.0, 0.24, SFX_SAMPLE_RATE, rng, 0.8)
            mix_linear(signal, strike, int(offset * SFX_SAMPLE_RATE), 0.27)
            mix_linear(signal, hit, int(offset * SFX_SAMPLE_RATE), 0.24)
    elif spec.cue == "ambgate":
        wind = colored_noise(frames, rng, 1.0)
        signal += wind * (0.025 + 0.015 * np.sin(t * 1.4))
        for offset in (0.36, 0.50, 1.24):
            chain = click(0.19, SFX_SAMPLE_RATE, rng, True)
            mix_linear(signal, chain, int(offset * SFX_SAMPLE_RATE), 0.13)
    elif spec.cue == "ambdrip":
        signal *= 0.72
        for offset, frequency in ((0.17, 880.0), (0.59, 720.0), (1.01, 1040.0), (1.43, 640.0)):
            drop = bell(frequency, 0.48, SFX_SAMPLE_RATE, rng)
            mix_linear(signal, drop, int(offset * SFX_SAMPLE_RATE), 0.12)
    elif spec.cue == "ambwind":
        gust = colored_noise(frames, rng, 0.72)
        lfo = 0.4 + 0.6 * np.sin(np.linspace(0.0, math.pi, frames)) ** 2
        signal = lowpass(gust, 11) * lfo * 0.12 + brown * 0.045
    elif spec.cue == "ambdrum":
        signal *= 0.55
        for offset, frequency in ((0.21, 54.0), (0.47, 68.0), (1.20, 49.0)):
            hit = drum(frequency, 0.75, SFX_SAMPLE_RATE, rng, 0.38)
            mix_linear(signal, hit, int(offset * SFX_SAMPLE_RATE), 0.32)
    elif spec.cue == "ambstone":
        signal += colored_noise(frames, rng, 1.1) * 0.035
        fall = drum(74.0, 0.72, SFX_SAMPLE_RATE, rng, 0.72)
        mix_linear(signal, fall, int(0.70 * SFX_SAMPLE_RATE), 0.25)
        for offset in (0.75, 0.90, 1.08):
            grit = click(0.12, SFX_SAMPLE_RATE, rng, False)
            mix_linear(signal, grit, int(offset * SFX_SAMPLE_RATE), 0.10)
    elif spec.cue == "ambrain":
        rain = colored_noise(frames, rng, 0.35)
        drops = np.maximum(0.0, colored_noise(frames, rng, 0.0) - 0.72)
        signal = lowpass(rain, 5) * 0.095 + drops * 0.11 + brown * 0.028
    elif spec.cue == "ambtavern":
        murmur = lowpass(colored_noise(frames, rng, 0.6), 65) * 0.095
        signal += murmur
        for offset, frequency in ((0.40, 1020.0), (1.24, 760.0)):
            cup = bell(frequency, 0.36, SFX_SAMPLE_RATE, rng)
            mix_linear(signal, cup, int(offset * SFX_SAMPLE_RATE), 0.10)
    elif spec.cue == "ambhearth":
        flame = colored_noise(frames, rng, 1.25)
        crackle = colored_noise(frames, rng, 0.05)
        sparse = np.where(np.abs(crackle) > 0.86, crackle, 0.0)
        signal = flame * 0.075 + sparse * 0.16
    elif spec.cue == "ambgrove":
        leaves = lowpass(colored_noise(frames, rng, 0.72), 9)
        sway = 0.35 + 0.65 * np.sin(np.linspace(0.0, math.pi, frames)) ** 2
        signal = leaves * sway * 0.10 + brown * 0.032
        wood = bell(293.66, 0.72, SFX_SAMPLE_RATE, rng)
        mix_linear(signal, wood, int(0.92 * SFX_SAMPLE_RATE), 0.08)
    elif spec.cue == "ambfen":
        wet = lowpass(colored_noise(frames, rng, 1.05), 17)
        insects = np.sin(2.0 * math.pi * (3100.0 + 180.0 * np.sin(t * 6.1)) * t)
        signal = wet * 0.10 + insects * (0.008 + 0.006 * np.sin(t * 4.7))
        for offset, frequency in ((0.26, 118.0), (0.88, 96.0), (1.47, 142.0)):
            bubble = chirp(frequency, frequency * 1.8, min(0.20, spec.duration - offset), SFX_SAMPLE_RATE, 0.7)
            mix_linear(signal, bubble, int(offset * SFX_SAMPLE_RATE), 0.10)
    elif spec.cue == "ambglass":
        wind = lowpass(colored_noise(frames, rng, 0.65), 7)
        signal = wind * 0.072 + brown * 0.020
        for offset, frequency in ((0.34, 1318.51), (1.18, 987.77)):
            glass = bell(frequency, min(0.72, spec.duration - offset), SFX_SAMPLE_RATE, rng)
            mix_linear(signal, glass, int(offset * SFX_SAMPLE_RATE), 0.11)
    elif spec.cue == "ambruin":
        wind = lowpass(colored_noise(frames, rng, 0.85), 13)
        signal = wind * 0.085 + brown * 0.030
        fall = drum(68.0, 0.62, SFX_SAMPLE_RATE, rng, 0.52)
        mix_linear(signal, fall, int(0.84 * SFX_SAMPLE_RATE), 0.18)
        for offset in (0.93, 1.09, 1.31):
            grit = click(0.11, SFX_SAMPLE_RATE, rng, bright=False)
            mix_linear(signal, grit, int(offset * SFX_SAMPLE_RATE), 0.07)
    elif spec.cue == "ambcave":
        signal = brown * 0.065 + np.sin(2.0 * math.pi * 48.0 * t) * 0.018
        for offset, frequency, gain in (
            (0.18, 920.0, 0.10),
            (0.63, 680.0, 0.07),
            (1.11, 1080.0, 0.09),
            (1.57, 560.0, 0.06),
        ):
            drop = bell(frequency, min(0.46, spec.duration - offset), SFX_SAMPLE_RATE, rng)
            mix_linear(signal, drop, int(offset * SFX_SAMPLE_RATE), gain)
    elif spec.cue == "ambcamp":
        flame = colored_noise(frames, rng, 1.30)
        crackle = colored_noise(frames, rng, 0.05)
        sparse = np.where(np.abs(crackle) > 0.88, crackle, 0.0)
        cloth = lowpass(colored_noise(frames, rng, 0.70), 15)
        signal = flame * 0.065 + sparse * 0.14 + cloth * 0.035
        tap = click(0.16, SFX_SAMPLE_RATE, rng, bright=False)
        mix_linear(signal, tap, int(1.12 * SFX_SAMPLE_RATE), 0.09)
    else:
        raise ValueError(f"Unhandled ambient cue: {spec.cue}")

    signal *= envelope(frames, SFX_SAMPLE_RATE, 0.12, 0.24)
    return one_shot_reverb(signal, SFX_SAMPLE_RATE, 0.13)


def compose_sfx(spec: SfxSpec) -> np.ndarray:
    rng = np.random.default_rng(stable_seed(spec.cue))
    if spec.family == "magic":
        signal = magical_sfx(spec, rng)
    elif spec.family == "world":
        signal = world_sfx(spec, rng)
    elif spec.family == "ambient":
        signal = ambient_sfx(spec, rng)
    else:
        raise ValueError(f"Unknown SFX family: {spec.family}")
    target = -19.5 if spec.family == "ambient" else -15.5
    mastered = master_audio(signal[np.newaxis, :], target, -3.0)[0]
    fade_frames = min(mastered.size // 4, max(24, int(0.004 * SFX_SAMPLE_RATE)))
    mastered[:fade_frames] *= np.sin(np.linspace(0.0, math.pi / 2.0, fade_frames)) ** 2
    mastered[-fade_frames:] *= np.cos(np.linspace(0.0, math.pi / 2.0, fade_frames)) ** 2
    return mastered


def write_pcm16(path: Path, audio: np.ndarray, sample_rate: int) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    array = np.asarray(audio)
    if array.ndim == 1:
        channels = 1
        interleaved = array
    elif array.ndim == 2:
        channels = array.shape[0]
        interleaved = array.T.reshape(-1)
    else:
        raise ValueError(f"Unsupported audio shape: {array.shape}")
    pcm = np.round(np.clip(interleaved, -0.999969, 0.999969) * 32767.0).astype("<i2")
    with wave.open(str(path), "wb") as wav:
        wav.setnchannels(channels)
        wav.setsampwidth(2)
        wav.setframerate(sample_rate)
        wav.writeframes(pcm.tobytes())


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as handle:
        for block in iter(lambda: handle.read(1024 * 1024), b""):
            digest.update(block)
    return digest.hexdigest()


def metrics_for(
    cue: str,
    title: str,
    direction: str,
    kind: str,
    path: Path,
    audio: np.ndarray,
    sample_rate: int,
) -> AssetMetrics:
    channels = 1 if audio.ndim == 1 else audio.shape[0]
    frames = audio.size if audio.ndim == 1 else audio.shape[1]
    peak = float(np.max(np.abs(audio)))
    rms = float(np.sqrt(np.mean(audio * audio)))
    seam = ""
    if kind == "music":
        stereo = audio if audio.ndim == 2 else audio[np.newaxis, :]
        seam_delta = float(np.max(np.abs(stereo[:, 0] - stereo[:, -1])))
        seam = f"{db(seam_delta):.2f}"
    return AssetMetrics(
        cue=cue,
        output=str(path.relative_to(STAGE_ROOT)).replace("\\", "/"),
        kind=kind,
        title=title,
        direction=direction,
        sample_rate=sample_rate,
        channels=channels,
        frames=frames,
        duration_seconds=round(frames / sample_rate, 4),
        peak_dbfs=round(db(peak), 2),
        rms_dbfs=round(db(rms), 2),
        seam_delta_dbfs=seam,
        sha256=sha256(path),
    )


def build_preview(tracks: Sequence[tuple[TrackSpec, np.ndarray]]) -> Path:
    excerpt_seconds = 5.5
    gap_seconds = 0.24
    excerpt_frames = int(excerpt_seconds * MUSIC_SAMPLE_RATE)
    gap = np.zeros((2, int(gap_seconds * MUSIC_SAMPLE_RATE)), dtype=np.float64)
    sections: list[np.ndarray] = []
    for _, audio in tracks:
        if audio.shape[1] <= excerpt_frames:
            excerpt = audio.copy()
        else:
            start = int(audio.shape[1] * 0.18)
            if start + excerpt_frames > audio.shape[1]:
                start = audio.shape[1] - excerpt_frames
            excerpt = audio[:, start : start + excerpt_frames].copy()
        fade = min(int(0.08 * MUSIC_SAMPLE_RATE), excerpt.shape[1] // 4)
        excerpt[:, :fade] *= np.sin(np.linspace(0.0, math.pi / 2.0, fade)) ** 2
        excerpt[:, -fade:] *= np.cos(np.linspace(0.0, math.pi / 2.0, fade)) ** 2
        sections.extend((excerpt, gap))
    preview = np.concatenate(sections[:-1], axis=1)
    preview = master_audio(preview, -20.0, -3.0)
    path = QA_DIR / "ash-and-brimstone-v1.82-music-preview.wav"
    write_pcm16(path, preview, MUSIC_SAMPLE_RATE)
    return path


def write_manifests(metrics: Sequence[AssetMetrics], preview_path: Path) -> None:
    DOCS_DIR.mkdir(parents=True, exist_ok=True)
    tsv_path = DOCS_DIR / "ORIGINAL_AUDIO_ASSET_MANIFEST.tsv"
    fields = list(asdict(metrics[0]).keys())
    with tsv_path.open("w", encoding="utf-8", newline="") as handle:
        writer = csv.DictWriter(handle, fieldnames=fields, delimiter="\t")
        writer.writeheader()
        for item in metrics:
            writer.writerow(asdict(item))

    report = {
        "release": "v1.82.0",
        "generator": "Tools/Audio/BuildOriginalAudio.py",
        "deterministic": True,
        "external_samples": False,
        "music_count": sum(item.kind == "music" for item in metrics),
        "sfx_count": sum(item.kind == "sfx" for item in metrics),
        "music_sample_rate": MUSIC_SAMPLE_RATE,
        "sfx_sample_rate": SFX_SAMPLE_RATE,
        "preview": str(preview_path.relative_to(STAGE_ROOT)).replace("\\", "/"),
        "assets": [asdict(item) for item in metrics],
    }
    (DOCS_DIR / "ORIGINAL_AUDIO_VALIDATION.json").write_text(
        json.dumps(report, indent=2, ensure_ascii=False) + "\n",
        encoding="utf-8",
    )


def read_pcm16(path: Path) -> tuple[np.ndarray, int]:
    with wave.open(str(path), "rb") as wav:
        if wav.getsampwidth() != 2:
            raise ValueError(f"{path}: expected PCM16")
        channels = wav.getnchannels()
        sample_rate = wav.getframerate()
        frames = wav.getnframes()
        raw = wav.readframes(frames)
    data = np.frombuffer(raw, dtype="<i2").astype(np.float64) / 32768.0
    if channels > 1:
        data = data.reshape(-1, channels).T
    return data, sample_rate


def validate_outputs() -> list[str]:
    errors: list[str] = []
    expected_music = {spec.cue for spec in TRACKS}
    expected_sfx = {spec.cue for spec in SFX}
    actual_music = {path.stem for path in MUSIC_DIR.glob("*.wav")}
    actual_sfx = {path.stem for path in SFX_DIR.glob("*.wav")}
    missing_music = expected_music - actual_music
    missing_sfx = expected_sfx - actual_sfx
    if missing_music:
        errors.append(f"missing original music cues: {sorted(missing_music)}")
    if missing_sfx:
        errors.append(f"missing original SFX cues: {sorted(missing_sfx)}")

    all_audio: list[tuple[Path, int, int]] = []
    all_audio.extend((MUSIC_DIR / f"{cue}.wav", MUSIC_SAMPLE_RATE, 2) for cue in sorted(expected_music))
    all_audio.extend((SFX_DIR / f"{cue}.wav", SFX_SAMPLE_RATE, 1) for cue in sorted(expected_sfx))
    fingerprints: dict[str, Path] = {}
    for path, expected_rate, expected_channels in all_audio:
        try:
            audio, sample_rate = read_pcm16(path)
        except Exception as exc:
            errors.append(f"{path}: unreadable WAV ({exc})")
            continue
        channels = 1 if audio.ndim == 1 else audio.shape[0]
        if sample_rate != expected_rate:
            errors.append(f"{path}: {sample_rate} Hz, expected {expected_rate}")
        if channels != expected_channels:
            errors.append(f"{path}: {channels} channels, expected {expected_channels}")
        if not np.all(np.isfinite(audio)):
            errors.append(f"{path}: non-finite samples")
        peak = float(np.max(np.abs(audio)))
        rms = float(np.sqrt(np.mean(audio * audio)))
        if peak > 0.73:
            errors.append(f"{path}: peak {db(peak):.2f} dBFS exceeds headroom contract")
        if rms < 0.003:
            errors.append(f"{path}: RMS {db(rms):.2f} dBFS is effectively silent")
        fingerprint = hashlib.sha256(np.round(audio * 32767.0).astype("<i2").tobytes()).hexdigest()
        if fingerprint in fingerprints:
            errors.append(f"{path}: waveform duplicates {fingerprints[fingerprint]}")
        fingerprints[fingerprint] = path
        if path.parent == MUSIC_DIR:
            seam_delta = float(np.max(np.abs(audio[:, 0] - audio[:, -1])))
            if seam_delta > 0.025:
                errors.append(f"{path}: loop seam delta {db(seam_delta):.2f} dBFS is too large")
    return errors


def build() -> None:
    MUSIC_DIR.mkdir(parents=True, exist_ok=True)
    SFX_DIR.mkdir(parents=True, exist_ok=True)
    QA_DIR.mkdir(parents=True, exist_ok=True)

    built_tracks: list[tuple[TrackSpec, np.ndarray]] = []
    metrics: list[AssetMetrics] = []
    for spec in TRACKS:
        audio = compose_track(spec)
        path = MUSIC_DIR / f"{spec.cue}.wav"
        write_pcm16(path, audio, MUSIC_SAMPLE_RATE)
        built_tracks.append((spec, audio))
        metrics.append(metrics_for(spec.cue, spec.title, spec.direction, "music", path, audio, MUSIC_SAMPLE_RATE))
        print(f"music  {spec.cue:42s} {audio.shape[1] / MUSIC_SAMPLE_RATE:6.2f}s")

    for spec in SFX:
        audio = compose_sfx(spec)
        path = SFX_DIR / f"{spec.cue}.wav"
        write_pcm16(path, audio, SFX_SAMPLE_RATE)
        metrics.append(metrics_for(spec.cue, spec.cue, spec.direction, "sfx", path, audio, SFX_SAMPLE_RATE))
        print(f"sfx    {spec.cue:42s} {audio.size / SFX_SAMPLE_RATE:6.2f}s")

    preview_path = build_preview(built_tracks)
    write_manifests(metrics, preview_path)
    errors = validate_outputs()
    if errors:
        raise SystemExit("Audio validation failed:\n- " + "\n- ".join(errors))
    print(
        f"Built {len(TRACKS)} music loops and {len(SFX)} SFX; "
        f"preview: {preview_path.relative_to(STAGE_ROOT)}"
    )


def build_music_cue(cue: str) -> None:
    spec = next((item for item in TRACKS if item.cue == cue), None)
    if spec is None:
        raise SystemExit(f"Unknown music cue: {cue}")
    MUSIC_DIR.mkdir(parents=True, exist_ok=True)
    audio = compose_track(spec)
    path = MUSIC_DIR / f"{spec.cue}.wav"
    write_pcm16(path, audio, MUSIC_SAMPLE_RATE)
    written, sample_rate = read_pcm16(path)
    seam_delta = float(np.max(np.abs(written[:, 0] - written[:, -1])))
    if sample_rate != MUSIC_SAMPLE_RATE or written.ndim != 2 or written.shape[0] != 2:
        raise SystemExit(f"{path}: selective music build produced an invalid WAV contract")
    if seam_delta > 0.025:
        raise SystemExit(f"{path}: loop seam delta {db(seam_delta):.2f} dBFS is too large")
    item = metrics_for(spec.cue, spec.title, spec.direction, "music", path, audio, MUSIC_SAMPLE_RATE)
    print(json.dumps(asdict(item), indent=2))


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--check", action="store_true", help="Validate existing outputs without rebuilding.")
    parser.add_argument("--music-cue", help="Rebuild one named music cue without touching the rest of the bank.")
    args = parser.parse_args()
    if args.check and args.music_cue:
        parser.error("--check and --music-cue cannot be combined")
    if args.music_cue:
        build_music_cue(args.music_cue)
        return
    if args.check:
        errors = validate_outputs()
        if errors:
            raise SystemExit("Audio validation failed:\n- " + "\n- ".join(errors))
        print(f"Validated {len(TRACKS)} music loops and {len(SFX)} SFX.")
        return
    build()


if __name__ == "__main__":
    main()
