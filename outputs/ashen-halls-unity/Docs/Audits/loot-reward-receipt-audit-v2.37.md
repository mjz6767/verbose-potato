# Loot reward receipt audit

## Findings

The loot modal presents a completed acquisition, not a pending transfer: inventory
admission and reward totals are already committed by the time it opens. There is
one featured item per receipt plus resource totals, and no inventory-capacity limit.
Consequently, adding Take All, Leave, weight, or capacity controls would misrepresent
the actual rules and risk implying rewards were lost on dismissal.

Inspected the existing 960×600 and 1280×720 captures, including the long Traveler's
Knot signature reward. The main readability problems were repeated trait/rarity
labels, an unstructured full comparison mixed into the item description, duplicated
recommendation prose in a large colored status box, three similarly weighted footer
actions, and a resource-only state that retained an empty equipment-style layout.

## Changes

- Resource rewards now form a readable top summary, retaining exact values and
  singular/plural labels; zero rewards stay hidden.
- Item identity, original item artwork, rarity, and traits form one left-hand card.
- A separate comparison column identifies the same canonical best-fit adventurer
  and upgrade/review grade. The preview prioritizes named form/intrinsic changes,
  base damage, then numerical sacrifices before gains; unchanged values are removed.
  It shows up to five concise lines, with an explicit full-details Inventory cue.
  The complete comparison string and Inventory comparison remain unchanged.
- A quiet saved-status line replaces the large tinted outcome box. Redundant
  acquisition instructions are omitted; authored reward provenance and action
  failures remain available in the status detail.
- Continue has a distinct high-contrast fill and remains the safe default keyboard/
  controller action. Quick equip and review remain separate deliberate actions with
  unchanged bindings, exact item selection, review gates, and save checkpoints.
- Resource-only receipts are 244 pixels tall, with no empty equipment icon/card;
  no-resource item receipts also collapse their unused resource space.

## Regression coverage

`LootRewardPresentationSmoke` checks 24 layout combinations at six resolutions
(960×600 through 3440×1440), non-overlapping hierarchy/action lanes, exact comparison
values/signs, strategic-loss priority, removal of duplicated labels, preserved
provenance/failure messages, safe default focus, explicit one-shot action bindings,
resource-only refresh, and unavailable-action suppression. Existing inventory/loot
identity, ownership, save, review, and runtime tests are retained.

Unity compilation, registered smoke runs and all thirteen Full audit gates pass
in `QA/project-audit/20260927-224209-70e4ec05/unity.log`.
Twelve native production-view captures (four reward scenarios at 960×600,
1280×720 and 1920×1080) pass text-fit assertions and independent visual review in
`QA/loot-experience/20260927-224148-671/`. These save-blocked presentation fixtures
do not claim actual reward acquisition; final-player evidence is recorded in
`Docs/EXPERIENCE_POLISH_AUDIT_2026-09-27.md` after that separate check.

Final v2.37 polish removes the smoke-only duplicate acquisition sentence and tests
Submit on the real production-bound default Continue button for both an available
upgrade and a resource-only receipt. Exact serialized state, item identity and
equipped gear remain unchanged; a stale second Submit also remains harmless.
These checks pass in the final thirteen-gate Full audit:
`QA/project-audit/20260927-230445-5b091473/unity.log`.
