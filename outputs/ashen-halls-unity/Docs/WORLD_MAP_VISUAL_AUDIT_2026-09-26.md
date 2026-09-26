# World map visual audit — September 26, 2026

## Result

The first pass below was implemented and verified locally after v2.25.0. This is an **unreleased preview**, not a replacement for the published package. Save schema remains v27; collision, quests, balance and authored atlas files are unchanged by this map pass.

### Continuation in progress

The user requested a further pass after the verified preview. Additional map-only changes now present in source are not part of the first-pass binary or its recorded hashes:

- Compact hover headings are composed from structured subject/access data alongside the full Details description. Identity and the relevant action/access hint appear first; lore and repeated location metadata stay in Details. The second identical hover-description calculation during repaint is removed.
- Inspection access wording ignores unseen patrol occupancy after the visible-threat check, so even a generic terrain heading does not hint that an unseen enemy is there. Movement/collision still consults live occupancy.
- Healthy party HP bars use a subdued rose fill. Half-health injury and quarter-health critical thresholds increase emphasis; critical/zero HP add a textual `!` warning. Numbers, fill ratios, class accents and mana remain intact. Native and fallback map rails share the rules.

Follow-up Unity/build/visual verification is pending. Static diff checks and an independent source review are complete. Added tests cover compact heading semantics, fog and unseen-patrol privacy in both views, unchanged campaign data, HP thresholds, text contrast, invalid vitals, healing reset, and native/fallback parity. No Unity or player process was launched for this continuation because a separate combat/audio release pass is active in the same project. The first-pass evidence JSON remains an immutable record of the preview it identifies, not a claim that its hashes match subsequently changed source.

Preview: `QA/audit-players/20260926-151512-5270a552/AshAndBrimstone.exe`. It is a Development build and retains the v2.25 version label. Do not mistake it for a new retail release. Exact hashes and coverage are in `Docs/VisualEvidence/world-map-declutter-2026-09-26.json`.

## Concrete visual improvements

- All world layers share the map's clip rectangle. Ordinary roofs, chimney smoke, fireplace art and oversized sprites can no longer cross the header or surrounding panels. Hover and pointer handling stay outside the group in screen coordinates.
- The party has a quiet foot-level locator rather than a full-body box. Ordinary walkable compass marks appear on hover; blocked tiles and enemy warnings remain persistent. The selected NPC keeps its foot cue and E key, without a second service badge and adjacent-use bracket.
- Routes are slimmer, dashed and less visually dominant. The Local next step uses one key marker rather than another large selection box. The gold core remains at least 1.5 pixels wide with stronger contrast; marked destinations, edge cues and fog-prefix rules are preserved.
- Region view uses consistent named-contact role icons even when near the party or an objective. Noninteractive interior patrons are omitted there, while full Local sprites and gameplay occupancy remain unchanged.
- The board heading now owns one useful identity: location, hovered subject, selected Region NPC, or a nearby visible threat warning. Duplicate map-mode, danger/action paragraphs and Details shortcuts are removed. The side rail/footer retain the full objective, vitals, action and Details control. Browsed bearing/distance remains visible when the Region focus leaves the party.
- Softer side-panel chrome and seven fewer row outlines give the map and character art more emphasis.

## Rendering work reduced

- A reusable, rendering-only threat index is rebuilt once per repaint. The deterministic 32-threat fixture requires 32 source reads for 693 viewport queries, versus 22,176 worst-case linear visits previously. Cleared patrols, depth/map changes and null/duplicate entries are covered; gameplay does not use a stale cross-frame cache.
- Visible objects are collected into reused storage before sorting. A 1200-plus-object fixture sorts fewer than one tenth of its objects while preserving the old stable depth/ID ordering, including equal IDs and mutated positions.
- Habitats are culled before zone/material lookup. Region view omits six decorative patron draws in the Town Hall fixture.
- Decorative HUD labels, bars and art no longer participate in pointer hit-testing. Nine intentional panels/buttons remain; blank HUD space still blocks clicks. Collapsed details skip unused text/log construction, and unchanged health fills avoid redundant writes.

These are verified work-count reductions, **not measured FPS or zero-allocation claims**.

## Verification

- Final Full Unity audit: PASS, all five gates (Rules, Inventory/Loot, Sprite Art, Combat UI, Runtime Boot). Log: `QA/project-audit/20260926-151429-a23b54bb/unity.log`.
- Isolated final preview build: PASS. Log: `QA/world-layout-20260926-final-build.log`.
- Eighteen final exact-resolution captures: PASS, with zero packet warnings/failures; all were personally inspected by the primary assistant and separate visual reviewers. Six views per size cover Kate, Lute, Dock and Scholar Local contacts, Kate Region, and a Region landmark/interior.

| Size | Baseline set | Accepted final set |
| --- | --- | --- |
| 960×600 | `20260926-144131-7099c67c` | `20260926-151535-99a3bd58` |
| 1280×720 | `20260926-144246-afa5ba4a` | `20260926-151656-68aaec0c` |
| 1920×1080 | `20260926-144402-94fdf6cc` | `20260926-151810-643e406d` |

All sets are under `QA/world-sprites`. Raw deterministic packets intentionally keep `aiReview.performed=false`; the evidence JSON records the separate subsequent conversational review.

The first visual iteration passed capture acceptance but its thin route was too faint against paving. Review prompted the brighter minimum-width core, and the final matrix was rebuilt and rechecked. An earlier runtime test also caught missing Region bearing/distance; functionality was restored and the existing assertion was not weakened.

Real mouse/keyboard checks in the final save-blocked preview passed: click Kate/open dialogue/Escape; click one adjacent tile/D return; Local wheel safety; Region button/drag/Home recenter; inspect Kate's Region icon/name; Tab back to Local; Details mouse button/blank-panel click blocking/Q close. No runtime errors were found in `QA/world-layout-20260926-live-input.log`. Only the owned fixture process was closed; no campaign save was written. The computer-use skill was used to verify actual input rather than infer it from screenshots.

## Scope and remaining limits

The published v2.25 ZIP hash remains `cb6a0e2170646aa450b6ef3a0f02d0417caf76546f5dce9ad11f185dfb19e0db`. No retail rebuild, source commit or push was performed in this pass. Physical-controller feel, audio listening, full-campaign playthrough and a hardware frame-time profile remain separate follow-ups. Existing sprite artwork is retained; this pass improves its presentation rather than generating replacement assets.
