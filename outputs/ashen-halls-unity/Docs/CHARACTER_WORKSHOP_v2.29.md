# The Living Folio

The v2.29 character workshop preserves the 40 painted race/class portraits and the direct choices introduced in v2.28. A new candlelit workshop painting, original music, tactile sound cues and short visual transitions add atmosphere around the existing controls.

## Visual presentation

- The new landscape painting loads on the first visible refresh. It covers the canvas without stretching and keeps a quiet center behind the parchment sheet. The prior backdrop remains a fallback.
- The selected portrait changes over 0.28 seconds. A short accent blends ancestry and class colors; the target portrait and all controls immediately retain their final position. Rapid changes replace the pending reveal with the latest choice.
- Eight small embers drift in the margins. Low-amplitude hearthlight, window light and selected-card glows add movement outside the text areas. The effects reuse 24 decorative graphics and never receive input.
- Reduced Motion removes the portrait fade, temporary accents and ember movement. Firelight and selection indications remain static. A live preference change finishes a transition immediately.
- Hiding the screen cancels the previous portrait and pauses the effect clock. Destroying the game releases the artwork and its owned canvas.
- Long role summaries break at a meaningful separator, keeping the Warlock's final stat value with its label.

Artwork provenance and the full built-in ImageGen prompt are in `CHARACTER_WORKSHOP_ART_v2.29.md`; the original five portrait atlases remain unchanged.

## Sound and music

The character-creation score uses a 53.333-second, 16-bar arrangement with contrasting phrases and a quieter middle. Twenty-three original cues cover companions, five races, eight classes, page changes, attributes, names, heraldry, equipment, training and beginning the journey. The sounds are synthesized locally from documented deterministic recipes, with no external recordings or runtime AI service.

Only successful changes emit cues. Repeated choices, rendering refreshes, muted effects and visual capture staging stay silent. The score and cues follow the existing independent music and effects preferences; the title-to-workshop transition retains its established crossfade. Cue pitch stays fixed so each ancestry and calling has a recognizable sound.

The coordinated title-music update adds **The Ember Oath**, a separate sixty-second opening theme. Its dedicated musical signature reaches full level after the opening reveal. The theme shares the normal title transport and volume controls.

## Verification

The Character workshop gate exercises all 40 portrait mappings and native controls, then checks repeated selections, actual animation state, rapid retargeting, live Reduced Motion, hidden-screen cleanup, decoration input isolation, and artwork/canvas disposal. Separate character-audio and title-music gates inspect the imported masters and actual routing/mix behavior. Audio synthesis checks verify reproducibility, distinct cue waveforms, headroom, loop boundaries and phrase variation.

The focused visual matrix captures identity and details at 960×600, 1280×720 and 1920×1080, plus a fixed-time transition and Reduced Motion at 1280×720. Editor previews and final Windows-player evidence are recorded separately. Final results and hashes are in `ReleaseEvidence/v2.29.0-summary.json` after validation; no subjective listening or physical-controller assessment is implied by numerical or image checks.

Save schema remains v27. The v2.28 Windows archive is preserved.
