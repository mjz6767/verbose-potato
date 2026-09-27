# The Living Folio — character workshop audio

The workshop uses an original 16-bar, 53.333-second E Dorian score at 72 BPM. Lute, flute, bowed strings, small bells, and a restrained frame drum support eight written two-bar phrases. Two quieter passages leave room for editing, a contrasting middle section changes the melody, and the returning theme ends on the opening harmony. The score replaces the earlier short muster loop on the existing `muster_by_firelight_loop` route.

Twenty-three short original interaction masters cover companion selection, five races, eight classes, opening and closing the details page, adding and returning attribute points, names, heraldry, kit, training, and departure. Race and class sounds use different physical timbres: stone, reeds, embers, glass, steel, wood, or bells. They last 0.23–0.90 seconds, preserve their authored pitch, and use immediate playback without a delayed cue queue.

Successful mutations own their sounds. Refreshing the screen, choosing the already selected companion/race/class, submitting an unchanged name, and changing a disabled attribute produce no new workshop cue. The details tab emits its sound only when the visible page changes. Visual capture suppresses workshop interaction cues. Music and SFX keep the existing independent volume and mute preferences.

The normal title → New Company route selects the imported workshop master and retains the existing 1.35-second equal-power crossfade. At the default 65% music preference, the workshop source gain is 0.23 × 0.65; the master supplies the improved body without overriding preferences. Cold entry to the hearth or workshop uses the shared one-second intro fade. SFX keep the existing 0.78 source gain and restrained per-action gains of 0.24–0.40.

## Reproduction and checks

Run `python Tools/Audio/BuildPartySetupAudio.py` from the Unity project to regenerate the workshop score and cue bank. It uses only the existing NumPy/standard-library synthesis pipeline, deterministic seeds, and original oscillator/noise instruments. It preserves unrelated music masters and updates only the workshop music manifest row. No samples or external services are involved.

Run the same command with `--validate-only` for a read-only check. Validation reconstructs the exact PCM, checks distinct cue hashes, finite samples, restrained peaks, clean cue endpoints and music wrap, eight distinct phrases, and quieter breathing room. `PartySetupAudioSmoke.RunOrThrow` checks the imported Unity resources, actual source gains, mute handling, pitch preservation, and rule coverage. `PartySetupWorkshopSmoke` exercises real controls, no-op selection, refresh silence, capture suppression, and imported music routing.

`Docs/PARTY_SETUP_AUDIO_VALIDATION.json` records the delivered master metrics. Ignored audition files are written under `QA/party-setup-audio-v2.29`: a 32-second music/interaction preview at the default runtime bus gains and a six-second loop-boundary excerpt. These are numerical and routing checks; no subjective listening review is claimed.
