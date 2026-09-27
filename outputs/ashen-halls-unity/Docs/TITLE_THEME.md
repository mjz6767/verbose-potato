# The Ember Oath

The title now has its own original theme, composed around a four-note horn call: D–A–B♭–A. Its unequal rhythm and leaning semitone recur across the arrangement so the opening has a musical identity players can recognize on returning to the game.

The 60-second score at 96 BPM moves through six passages:

| Time | Passage |
| --- | --- |
| 0–10 seconds | Plucked pickup at 0.35 seconds; horn signature at 1.20 seconds |
| 10–20 seconds | Company theme with developed accompaniment |
| 20–30 seconds | Intimate lute and reed passage; percussion withdraws |
| 30–40 seconds | Harmonic ascent with a prepared dominant |
| 40–55 seconds | Full returning theme with an expanded answer |
| 55–60 seconds | Tonic coda, natural release, and room tails into the next loop |

All notes, instrument synthesis, performance envelopes, and room treatment are authored locally. No external recordings or services are used. The existing title resource key is retained for compatibility; tavern music continues to use its separate cue.

## First-launch presentation

The title source remains at 0.29, with the saved default music level of 65%. Its initial SmoothStep fade is one second. The forge strike uses gain 0.20 and the reveal chime 0.14; the chime finishes at 1.16 seconds before the horn enters at 1.20. Settings changes and mute preserve the track position. The title ambience is delayed so it does not cover the opening signature.

`BuildTitleTheme.py` reads the live source settings when making its opening preview. The preview includes the music fade, logo effects, and two example focus interactions. Tavern/hearth ambience is excluded. This is a deterministic reconstruction of the foreground mix, not a recording of a running Unity player.

## Validation and listening

The saved master is 60 seconds, stereo PCM16 at 32 kHz. Its SHA-256 is `df8da67eec83d50a5d85306ba455c1a60b800208e68f77ae240e578294c1c052`. A fresh render reproduces the saved PCM exactly.

Five dedicated tests cover delivery, deterministic rebuilding, motif recurrence, the quiet passage and returning dynamics, and harmony/input contracts. The independent saved-audio audit reports:

- −5.0 dBTP true peak, −19.0 dBFS RMS, and −16.2 LUFS integrated loudness.
- 98.6% mono retention, with a 4.0 dB lift from the intimate passage to the return.
- 0.16% sub-bass energy below 80 Hz, down from 38.4%; the small-speaker bandwidth measurement gains 3.1 dB against the previous title.
- No engineering failures. A 9.3 dB difference between the closing and opening quarter-second windows is retained as a listening advisory: the last reed releases before the seam, followed by an intentional phrase breath, while string and room tails wrap around it.

These checks establish reproducibility, structure, and signal quality. Memorability, instrument character, emotional effect, and repeated-listening fatigue require listening judgment.

Local audition files are in `QA/title-theme-2026-09-27/`:

- `the-ember-oath.wav`: the exact shipped master.
- `in-game-opening.wav`: 20 seconds of opening music and menu accents at current gains, with ambience excluded.
- `loop-boundary.wav`: seven seconds before the boundary followed by ten seconds after it.
- `before-after-opening.wav`: previous opening, one second of silence, then the new opening.

Run from the Unity project directory, with `-B` to keep the package source free of Python caches:

```powershell
python -B Tools/Audio/BuildTitleTheme.py --check
python -B Tools/Audio/TestTitleTheme.py
python -B Tools/Audio/TitleMusicQualityAudit.py --self-test
python -B Tools/Audio/TitleMusicQualityAudit.py --input Assets/Resources/Audio/Music/tavern_storm_hearth_ensemble_loop.wav --report QA/title-quality-final.json --enforce
```

The shared audio builder dispatches the title cue to `TitleTheme.compose`. `BuildTitleTheme.py` can rebuild only the title and listening previews; it emits the replacement asset row in `Docs/TITLE_THEME_VALIDATION.json` for the coordinated bank manifest. `TitleMusicExperienceSmoke` additionally verifies the imported Unity clip, title routing, opening audibility, settings/mute continuity, and presentation timing as part of the release audit.
