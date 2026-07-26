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
- writes `visual-qa-packet.json` and `visual-qa-packet.md`; and
- exits `0` on deterministic success, `2` on validation failure, or `1` on an operational error.

The script does not call a model, use the network, or make a model opinion part of its exit status.

## v1.89 Default Matrix

The default expected matrix covers the Golden Thread exploration guidance at both supported visual-QA resolutions:

- `explore-compact-1280x720.png`
- `explore-compact-1920x1080.png`
- `explore-wide-1280x720.png`
- `explore-wide-1920x1080.png`

Use the exact rendered dimensions in each basename. The script rejects a renamed image whose PNG dimensions do not agree.

For a later release or a focused feature, pass an explicit `-ExpectedCapture` array using `scenario@WIDTHxHEIGHT`. Unexpected valid captures are retained as supplemental evidence and reported as warnings.

## Example

Run this from the Unity project root after creating deterministic in-player captures:

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass

$qaOutput = '..\..\QA\v1.89.0'
$screenshots = @(
    "$qaOutput\explore-compact-1280x720.png",
    "$qaOutput\explore-compact-1920x1080.png",
    "$qaOutput\explore-wide-1280x720.png",
    "$qaOutput\explore-wide-1920x1080.png"
)
$captureLogs = @(
    "$qaOutput\capture-explore-compact-1280x720.log",
    "$qaOutput\capture-explore-compact-1920x1080.log",
    "$qaOutput\capture-explore-wide-1280x720.log",
    "$qaOutput\capture-explore-wide-1920x1080.log"
)

& '.\Tools\NewVisualQaPacket.ps1' `
    -ScreenshotPath $screenshots `
    -CaptureLogPath $captureLogs `
    -OutputDirectory $qaOutput `
    -ReleaseVersion 'v1.89.0'
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
