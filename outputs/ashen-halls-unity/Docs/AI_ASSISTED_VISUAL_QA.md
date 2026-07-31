# AI-Assisted Visual QA

Ash & Brimstone uses AI vision only as an optional development-time second reviewer. It is not part of the player, combat rules, saves, or release runtime. Deterministic capture checks and human review remain authoritative.

## What the Packet Builder Does

`Tools/NewVisualQaPacket.ps1`, included in both the source workspace and Windows release package, accepts explicit screenshot paths, optional local capture-log paths, and an explicit output directory. It:

- reads only the named inputs;
- requires screenshot names in `scenario-WIDTHxHEIGHT.png` form;
- validates the PNG signature, `IHDR`, and encoded dimensions;
- checks required scenario/resolution coverage;
- calculates SHA-256 hashes that bind a review to exact evidence;
- optionally corroborates captures against the game's deterministic visual-smoke log summaries;
- derives `releaseVersion` from source `VersionInfo.cs` or requires an explicit packaged-build version, then records that value and its source;
- writes `visual-qa-packet.json` and `visual-qa-packet.md`; and
- exits `0` on deterministic success, `2` on validation failure, or `1` on an operational error.

The script does not call a model, use the network, or make a model opinion part of its exit status.

## v1.98 One-Intent Power-Book Matrix

The v1.98 packet lives under `QA/v1.98.0-one-intent` and binds direct packaged-player captures to the typed state emitted by `-ashen-book-state`. The required set covers committed Selection, passive Preview, and armed Targeting in both books, then distributes Locked, Low Resource, No Target, Action Used, Disabled, and Blocked across the Spellbook and Skillbook at 1280x720 and 1920x1080.

Each capture log must contain the staged tuple (`book`, requested state, filter, committed card, detail card, typed state, availability text, context icon, preview card, and targeting flag), complete with `complete=True` and `failure=None`. A re-entrant `Attempting to select ... while already selecting` warning is a packet blocker even when the PNG otherwise validates.

Use an explicit `-ReleaseVersion 'v1.98.0'` when building the packet from the packaged player. This prevents a copied tool or old package from silently labeling evidence with a stale default.

The final twelve-capture packet passes with zero failures and zero warnings. It records `releaseVersion=v1.98.0`, `releaseVersionSource=parameter`, and capture-set SHA-256 `ff8e2a3d83ffddbf2bb935e54a5206d9c14415cf3068f3a069e8e7bfe72822a9`. The exact matrix, staged tuples, local representative visual review, and remaining physical-input checks are retained in `QA/v1.98.0-one-intent/README.md`.

## v1.92 Accepted Matrix and Evidence

The default expected matrix covers the Golden Thread exploration guidance at both supported visual-QA resolutions:

- `explore-compact-1280x720.png` (Local Map)
- `explore-compact-1920x1080.png` (Local Map)
- `explore-wide-1280x720.png` (Region Map)
- `explore-wide-1920x1080.png` (Region Map)

Use the exact rendered dimensions in each basename. The script rejects a renamed image whose PNG dimensions do not agree.
Add `-ashen-details-smoke` to an exploration capture command for a deterministic Details-open companion image; the capture log records the actual map scale, Details state, guidance target, and guidance text.

For the v1.92 art signoff, also stage `-ashen-gate-smoke east|west|north|south` close-ups and retain them as `gate-east-*`, `gate-west-*`, `gate-north-*`, and `gate-south-*`. Review the full perimeter and compact party marker in at least one 1920x1080 Region Map capture. Pass an explicit `-ExpectedCapture` array using `scenario@WIDTHxHEIGHT` when these are required packet members; unexpected valid captures are retained as supplemental evidence and reported as warnings.

The v1.92 material-art review must confirm that city, market, temple, and keep ground read as four coherent families rather than a one-cell quilt; the packed-dirt east/west approaches no longer alternate between light and dark square paintings; static object footprints stay visually quiet; and three-band material feathering does not cross blocked terrain, gates, or thresholds. Compare both map scales for hard district carpets, grid seams, moire, and loss of wall or route hierarchy.

The v1.92 rule smoke, runtime boot smoke, and Windows build pass. The four canonical Local/Region captures, all four gate close-ups, and the 1280x720 Details-open companion report `complete=True` with `failure=None`. The deterministic packet passes with capture-set SHA-256 `2669b531e4ba7660c37b66cc31ce0672aab5cb9c427182fcfea6019d30e51d65`; the assisted inspection record is `QA/v1.92.0/manual-visual-signoff.md`.

## v1.93 Focused Gate and Wall Evidence

The four canonical Local/Region captures remain the default matrix. The
v1.93 gate/wall packet adds:

- `gate-west-1280x720.png`
- `gate-east-1280x720.png`
- `gate-north-1280x720.png`
- `gate-south-1280x720.png`
- `gate-west-3200x1800.png`
- `gate-east-3200x1800.png`

All ten capture/log pairs report exact requested, rendered, filename, and PNG
dimensions with `complete=True`, `failure=None`, nonzero samples, and non-black
variation. The packet passes with capture-set SHA-256
`6f9c4d6fdaf9aa91caedc742afb889cc2c736de798eb0bc25b57a95751c58cf7`.
An advisory OpenAI Codex / GPT-5 inspection reports no visible blocker: West
presents wilderness left and town right, East presents town left and wilderness
right, both place low bastions above and below a clear horizontal road,
vertical wall backing hugs the authored masonry, and no facade, broad rail,
hidden straight-wall cell, or cross-road sill remains. Confidence is high for
the captured states; uncertainty remains around physical traversal, collision,
clean extraction, and other GPU/display configurations. The pending human
disposition is recorded in
`QA/v1.93.0/codex-assisted-visual-review.md`; the packet and comparison sheet
are retained beside it.

## v1.93 NPC Contact and Combat-Book Continuation Evidence

The focused continuation set is retained in
`QA/v1.93.0-contact-book-continuation`. It contains 14 packaged-player PNG/log
pairs: four adjacent NPC contact views, four opened contact dialogues, and six
Spellbook/Skillbook states spanning 1280x720 and 1920x1080.

All 14 logs report exact requested/rendered/PNG dimensions,
`complete=True`, `failure=None`, and non-black sampling variation. The contact
stager additionally records the exact object and atlas cell:

- Kate: `DinerCook`, cell 10, portrait 12, four choices;
- Lute: `Provisioner`, cell 11, portrait 17, four choices;
- Dock Worker: `DockWorker`, cell 14, portrait 18, no topic choices; and
- Midgaard Scholar: `Scholar`, cell 19, portrait 19, no topic choices.

The six book captures cover future, armed-targeting, and action-used states in
both the Spellbook and Skillbook. Advisory local Codex inspection found no
visible blocker: selection, filter counts, state labels, target counts, CTA
copy, formula/casting detail, and controller footer remained coherent and
legible at both resolutions. No images or logs were transmitted externally.
The complete evidence inventory, observations, atlas SHA-256 matches, and
remaining human checks are recorded in
`QA/v1.93.0-contact-book-continuation/README.md`. This remains pending human
release disposition.

## Example

Run this from the Unity project root after creating deterministic in-player captures:

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass

$qaOutput = '..\..\QA\v1.92.0'
$screenshots = @(
    "$qaOutput\explore-compact-1280x720.png",
    "$qaOutput\explore-compact-1920x1080.png",
    "$qaOutput\explore-wide-1280x720.png",
    "$qaOutput\explore-wide-1920x1080.png"
)
$captureLogs = @(
    "$qaOutput\explore-compact-1280x720.log",
    "$qaOutput\explore-compact-1920x1080.log",
    "$qaOutput\explore-wide-1280x720.log",
    "$qaOutput\explore-wide-1920x1080.log"
)

& '.\Tools\NewVisualQaPacket.ps1' `
    -ScreenshotPath $screenshots `
    -CaptureLogPath $captureLogs `
    -OutputDirectory "$qaOutput\visual-qa-packet" `
    -ReleaseVersion 'v1.92.0'
```

The process-scope execution-policy change is temporary. Do not weaken the user- or machine-level policy.

Capture logs are optional. When any are supplied, every explicit screenshot must have one matching accepted summary, and every supplied log must identify the requested release. The JSON packet contains only log basenames, hashes, counts, and parsed capture facts. It never contains raw log text or absolute paths.

## Deterministic Authority

A packet passes only when every required matrix member is a readable, correctly named PNG with a valid signature and matching `IHDR` dimensions. Supplied log evidence must additionally confirm:

- `complete=True`;
- `failure=None`;
- requested, screen, logged PNG, filename, and decoded dimensions agree; and
- the capture contains valid non-black sampling evidence.

The game's own capture path performs full Unity decoding and content sampling. The packet builder deliberately repeats only portable file/header checks and parses that logged acceptance. Neither layer claims to test visual quality.

All `-ashen-*-smoke` staging launches and `-ashen-capture` launches block campaign writes and legacy-save import. Batch boot also blocks both checkpoints and legacy import. QA staging must never replace or migrate a developer's normal campaign.

The SHA-256 `captureSetSha256` covers every explicit screenshot basename, capture id, file hash, and deterministic acceptance state. Copy that value into any advisory review so results cannot silently drift to different images.

## Optional AI Review

Only start AI review through an explicit developer or user action. Provide:

1. the listed in-player PNGs;
2. the sanitized `visual-qa-packet.json`;
3. the last approved same-scenario/same-resolution PNGs when comparison is requested; and
4. `Docs/AI_VISUAL_QA_REVIEW.schema.json`.

Ask the reviewer to assess only visible evidence:

- overlap and clipping;
- text legibility and contrast;
- information hierarchy;
- focus, selection, disabled, and targeting-state clarity;
- unexpected debug artifacts;
- missing or materially displaced information across resolutions; and
- regressions against the named baseline.

The reviewer must give concrete evidence, affected capture ids, severity, confidence, a bounded recommendation, and uncertainty. `no-blocker-observed` is not a release pass. Every finding remains `pending` until a human dispositions it.

Validate the returned JSON against the schema before accepting it. Record the actual provider/model disclosure rather than describing the review generically as "AI checked."

## Transmission Airlock

The deterministic smoke PNGs contain only staged game content and do not capture the desktop. Confirm that fact before any external transmission.

Do not transmit:

- raw Unity or player logs, which can contain a Windows username, absolute paths, GPU, and driver details;
- arbitrary desktop screenshots;
- saves or user-authored party data;
- environment variables, API keys, or local configuration; or
- additional files merely because they share the QA directory.

For any future external API adapter:

- keep invocation separate and opt-in;
- show the exact outbound basenames before sending;
- require explicit approval;
- use `store: false`;
- request schema-constrained output and validate it application-side;
- disclose the selected model and uncertainty; and
- fail closed without weakening the deterministic/manual fallback.

This repository intentionally contains no network or API adapter for this workflow.

## Complete Non-AI Fallback

If AI review is unavailable or declined:

1. require a passing deterministic packet;
2. open every screenshot;
3. use the manual checklist in `visual-qa-packet.md`;
4. compare matching scenarios at 1280x720 and 1920x1080;
5. compare each changed screen with the last approved same-resolution baseline;
6. disposition every unexpected change; and
7. complete live release-checklist checks for interaction, timing, animation, audio, controllers, and newcomer comprehension.

This fallback is the release process, not a degraded game mode. AI only adds a fallible second set of eyes.
