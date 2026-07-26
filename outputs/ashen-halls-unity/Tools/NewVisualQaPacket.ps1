[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string[]]$ScreenshotPath,

    [string[]]$CaptureLogPath = @(),

    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$OutputDirectory,

    [ValidatePattern('^v[0-9]+\.[0-9]+\.[0-9]+$')]
    [string]$ReleaseVersion = 'v1.89.0',

    [ValidateNotNullOrEmpty()]
    [string[]]$ExpectedCapture = @(
        'explore-compact@1280x720',
        'explore-compact@1920x1080',
        'explore-wide@1280x720',
        'explore-wide@1920x1080'
    )
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$validationFailures = [System.Collections.ArrayList]::new()
$validationWarnings = [System.Collections.ArrayList]::new()
$captureRecords = [System.Collections.ArrayList]::new()
$logRecords = [System.Collections.ArrayList]::new()

function Add-ValidationFailure {
    param([string]$Message)
    [void]$script:validationFailures.Add($Message)
}

function Add-ValidationWarning {
    param([string]$Message)
    [void]$script:validationWarnings.Add($Message)
}

function Add-CaptureFailure {
    param(
        [System.Collections.ArrayList]$CaptureIssues,
        [string]$Message
    )

    [void]$CaptureIssues.Add($Message)
    Add-ValidationFailure $Message
}

function Get-SafeBaseName {
    param([AllowEmptyString()][string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path)) {
        return '<unnamed>'
    }

    try {
        $name = [System.IO.Path]::GetFileName($Path)
    }
    catch {
        return '<invalid-name>'
    }

    if ([string]::IsNullOrWhiteSpace($name)) {
        return '<unnamed>'
    }

    return [regex]::Replace($name, '[\u0000-\u001f\u007f]', '_')
}

function Get-Sha256Hex {
    param([byte[]]$Bytes)

    $hasher = [System.Security.Cryptography.SHA256]::Create()
    try {
        $hash = $hasher.ComputeHash($Bytes)
        return ([System.BitConverter]::ToString($hash)).Replace('-', '').ToLowerInvariant()
    }
    finally {
        $hasher.Dispose()
    }
}

function Get-TextSha256Hex {
    param([AllowEmptyString()][string]$Text)
    return Get-Sha256Hex ([System.Text.Encoding]::UTF8.GetBytes($Text))
}

function Read-UInt32BigEndian {
    param(
        [byte[]]$Bytes,
        [int]$Offset
    )

    return [uint64]$Bytes[$Offset] * 16777216L +
        [uint64]$Bytes[$Offset + 1] * 65536L +
        [uint64]$Bytes[$Offset + 2] * 256L +
        [uint64]$Bytes[$Offset + 3]
}

function Get-PngHeaderInfo {
    param([byte[]]$Bytes)

    $expectedSignature = [byte[]](0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a)
    $signatureValid = $Bytes.Length -ge $expectedSignature.Length
    if ($signatureValid) {
        for ($index = 0; $index -lt $expectedSignature.Length; $index++) {
            if ($Bytes[$index] -ne $expectedSignature[$index]) {
                $signatureValid = $false
                break
            }
        }
    }

    $ihdrValid = $false
    $width = $null
    $height = $null
    if ($signatureValid -and $Bytes.Length -ge 24) {
        $chunkLength = Read-UInt32BigEndian $Bytes 8
        $chunkName = [System.Text.Encoding]::ASCII.GetString($Bytes, 12, 4)
        if ($chunkLength -eq 13 -and $chunkName -ceq 'IHDR') {
            $parsedWidth = Read-UInt32BigEndian $Bytes 16
            $parsedHeight = Read-UInt32BigEndian $Bytes 20
            if ($parsedWidth -gt 0 -and
                $parsedHeight -gt 0 -and
                $parsedWidth -le [int]::MaxValue -and
                $parsedHeight -le [int]::MaxValue) {
                $width = [int]$parsedWidth
                $height = [int]$parsedHeight
                $ihdrValid = $true
            }
        }
    }

    return [pscustomobject][ordered]@{
        signatureValid = [bool]$signatureValid
        ihdrValid = [bool]$ihdrValid
        width = $width
        height = $height
    }
}

function ConvertTo-MarkdownCell {
    param([AllowNull()][object]$Value)

    if ($null -eq $Value) {
        return ''
    }

    $text = [string]$Value
    $text = $text.Replace('|', '\|')
    $text = $text.Replace("`r", ' ')
    $text = $text.Replace("`n", ' ')
    return $text
}

$screenshotInputs = @($ScreenshotPath)
$logInputs = @($CaptureLogPath)
$expectedPattern = '^(?<scenario>[a-z0-9][a-z0-9-]*)@(?<width>[1-9][0-9]*)x(?<height>[1-9][0-9]*)$'
$captureNamePattern = '^(?<scenario>[a-z0-9][a-z0-9-]*)-(?<width>[1-9][0-9]*)x(?<height>[1-9][0-9]*)\.png$'

$expectedByKey = @{}
$expectedIndex = 0
foreach ($expected in @($ExpectedCapture)) {
    $expectedIndex++
    $candidate = if ($null -eq $expected) { '' } else { $expected.Trim().ToLowerInvariant() }
    if ($candidate -notmatch $expectedPattern) {
        Add-ValidationFailure "Expected capture entry $expectedIndex must use scenario@WIDTHxHEIGHT."
        continue
    }

    $scenario = $Matches['scenario']
    $width = [int]$Matches['width']
    $height = [int]$Matches['height']
    $key = "$scenario@$($width)x$height"
    if ($expectedByKey.ContainsKey($key)) {
        Add-ValidationFailure "Expected capture '$key' is duplicated."
        continue
    }

    $expectedByKey[$key] = [pscustomobject][ordered]@{
        id = $key
        scenario = $scenario
        width = $width
        height = $height
    }
}

if ($expectedByKey.Count -eq 0) {
    Add-ValidationFailure 'At least one valid expected capture is required.'
}

$logsWereProvided = $logInputs.Count -gt 0
$logEvidenceByCaptureName = @{}
$seenLogInputNames = @{}
$captureSummaryPattern = '(?im)visual smoke capture:\s*path=(?<path>.+?),\s*complete=(?<complete>True|False),\s*requested=(?<requestedWidth>[0-9]+)x(?<requestedHeight>[0-9]+),\s*screen=(?<screenWidth>[0-9]+)x(?<screenHeight>[0-9]+),\s*failure=(?<failure>[A-Za-z][A-Za-z0-9_-]*),\s*png=(?<pngWidth>[0-9]+)x(?<pngHeight>[0-9]+),\s*samples=(?<samples>[0-9]+),\s*nearBlack=(?<nearBlack>[0-9]+),\s*brightness=(?<minimumBrightness>[0-9]+)-(?<maximumBrightness>[0-9]+)\.'
$releaseMarkerPattern = 'boot start\s+' + [regex]::Escape($ReleaseVersion) + '\s+/'

foreach ($logPath in $logInputs) {
    $logFileName = Get-SafeBaseName $logPath
    if ($seenLogInputNames.ContainsKey($logFileName)) {
        Add-ValidationFailure "Capture log basename '$logFileName' is duplicated."
    }
    else {
        $seenLogInputNames[$logFileName] = $true
    }

    $logExists = [System.IO.File]::Exists($logPath)
    $logBytes = $null
    $logHash = $null
    $releaseObserved = $false
    $summaryCount = 0

    if (-not $logExists) {
        Add-ValidationFailure "Capture log '$logFileName' does not exist."
    }
    else {
        try {
            $logBytes = [System.IO.File]::ReadAllBytes($logPath)
            $logHash = Get-Sha256Hex $logBytes
            $logText = [System.Text.Encoding]::UTF8.GetString($logBytes)
            $releaseObserved = [regex]::IsMatch(
                $logText,
                $releaseMarkerPattern,
                [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
            if (-not $releaseObserved) {
                Add-ValidationFailure "Capture log '$logFileName' does not identify $ReleaseVersion."
            }

            $summaryMatches = [regex]::Matches(
                $logText,
                $captureSummaryPattern,
                [System.Text.RegularExpressions.RegexOptions]::IgnoreCase -bor
                    [System.Text.RegularExpressions.RegexOptions]::Multiline)
            $summaryCount = $summaryMatches.Count
            if ($summaryCount -eq 0) {
                Add-ValidationFailure "Capture log '$logFileName' has no parseable visual-smoke capture summary."
            }

            foreach ($summaryMatch in $summaryMatches) {
                $capturedFileName = Get-SafeBaseName $summaryMatch.Groups['path'].Value
                $complete = [string]::Equals(
                    $summaryMatch.Groups['complete'].Value,
                    'True',
                    [System.StringComparison]::OrdinalIgnoreCase)
                $failure = $summaryMatch.Groups['failure'].Value
                $requestedWidth = [int]$summaryMatch.Groups['requestedWidth'].Value
                $requestedHeight = [int]$summaryMatch.Groups['requestedHeight'].Value
                $screenWidth = [int]$summaryMatch.Groups['screenWidth'].Value
                $screenHeight = [int]$summaryMatch.Groups['screenHeight'].Value
                $pngWidth = [int]$summaryMatch.Groups['pngWidth'].Value
                $pngHeight = [int]$summaryMatch.Groups['pngHeight'].Value
                $sampleCount = [int]$summaryMatch.Groups['samples'].Value
                $nearBlackSampleCount = [int]$summaryMatch.Groups['nearBlack'].Value
                $minimumBrightness = [int]$summaryMatch.Groups['minimumBrightness'].Value
                $maximumBrightness = [int]$summaryMatch.Groups['maximumBrightness'].Value
                $overwhelminglyBlack = $sampleCount -gt 0 -and
                    [long]$nearBlackSampleCount * 100L -ge [long]$sampleCount * 98L
                $uniformlyDark = $maximumBrightness -le 24 -and
                    $maximumBrightness - $minimumBrightness -le 8
                $pixelEvidenceAccepted = -not (
                    $sampleCount -le 0 -or
                    $nearBlackSampleCount -lt 0 -or
                    $nearBlackSampleCount -gt $sampleCount -or
                    $minimumBrightness -lt 0 -or
                    $maximumBrightness -gt 255 -or
                    $minimumBrightness -gt $maximumBrightness -or
                    $overwhelminglyBlack -or
                    $uniformlyDark)

                if ($logEvidenceByCaptureName.ContainsKey($capturedFileName)) {
                    Add-ValidationFailure "Capture '$capturedFileName' has duplicate log summaries."
                    continue
                }

                $summary = [pscustomobject][ordered]@{
                    captureFileName = $capturedFileName
                    logFileName = $logFileName
                    logSha256 = $logHash
                    complete = [bool]$complete
                    failure = $failure
                    requestedWidth = $requestedWidth
                    requestedHeight = $requestedHeight
                    screenWidth = $screenWidth
                    screenHeight = $screenHeight
                    pngWidth = $pngWidth
                    pngHeight = $pngHeight
                    sampleCount = $sampleCount
                    nearBlackSampleCount = $nearBlackSampleCount
                    minimumBrightness = $minimumBrightness
                    maximumBrightness = $maximumBrightness
                    pixelEvidenceAccepted = [bool]$pixelEvidenceAccepted
                }
                $logEvidenceByCaptureName[$capturedFileName] = $summary

                if (-not $complete) {
                    Add-ValidationFailure "Capture log marks '$capturedFileName' incomplete."
                }
                if (-not [string]::Equals($failure, 'None', [System.StringComparison]::OrdinalIgnoreCase)) {
                    Add-ValidationFailure "Capture log reports a deterministic failure for '$capturedFileName'."
                }
                if ($requestedWidth -ne $screenWidth -or
                    $requestedHeight -ne $screenHeight -or
                    $requestedWidth -ne $pngWidth -or
                    $requestedHeight -ne $pngHeight) {
                    Add-ValidationFailure "Capture log dimensions disagree for '$capturedFileName'."
                }
                if (-not $pixelEvidenceAccepted) {
                    Add-ValidationFailure "Capture log pixel evidence is invalid for '$capturedFileName'."
                }
            }
        }
        catch {
            Add-ValidationFailure "Capture log '$logFileName' could not be read."
        }
    }

    [void]$logRecords.Add([pscustomobject][ordered]@{
        fileName = $logFileName
        exists = [bool]$logExists
        byteLength = if ($null -eq $logBytes) { $null } else { [long]$logBytes.Length }
        sha256 = $logHash
        releaseVersionObserved = [bool]$releaseObserved
        captureSummaryCount = [int]$summaryCount
    })
}

$seenInputNames = @{}
$seenCaptureKeys = @{}
$acceptedByKey = @{}

foreach ($path in $screenshotInputs) {
    $fileName = Get-SafeBaseName $path
    $captureIssues = [System.Collections.ArrayList]::new()
    $scenario = $null
    $namedWidth = $null
    $namedHeight = $null
    $captureKey = $null
    $fileExists = [System.IO.File]::Exists($path)
    $fileBytes = $null
    $fileHash = $null
    $byteLength = $null
    $pngInfo = [pscustomobject][ordered]@{
        signatureValid = $false
        ihdrValid = $false
        width = $null
        height = $null
    }

    if ($seenInputNames.ContainsKey($fileName)) {
        Add-CaptureFailure $captureIssues "Screenshot basename '$fileName' is duplicated."
    }
    else {
        $seenInputNames[$fileName] = $true
    }

    if ($fileName -match $captureNamePattern) {
        $scenario = $Matches['scenario'].ToLowerInvariant()
        $namedWidth = [int]$Matches['width']
        $namedHeight = [int]$Matches['height']
        $captureKey = "$scenario@$($namedWidth)x$namedHeight"
        if ($seenCaptureKeys.ContainsKey($captureKey)) {
            Add-CaptureFailure $captureIssues "Capture id '$captureKey' is duplicated."
        }
        else {
            $seenCaptureKeys[$captureKey] = $true
        }
    }
    else {
        Add-CaptureFailure $captureIssues "Screenshot '$fileName' must use scenario-WIDTHxHEIGHT.png."
    }

    if (-not $fileExists) {
        Add-CaptureFailure $captureIssues "Screenshot '$fileName' does not exist."
    }
    else {
        try {
            $fileBytes = [System.IO.File]::ReadAllBytes($path)
            $byteLength = [long]$fileBytes.Length
            $fileHash = Get-Sha256Hex $fileBytes
            $pngInfo = Get-PngHeaderInfo $fileBytes
            if (-not $pngInfo.signatureValid) {
                Add-CaptureFailure $captureIssues "Screenshot '$fileName' has an invalid PNG signature."
            }
            elseif (-not $pngInfo.ihdrValid) {
                Add-CaptureFailure $captureIssues "Screenshot '$fileName' has an invalid PNG IHDR."
            }
            elseif ($null -ne $namedWidth -and
                ($pngInfo.width -ne $namedWidth -or $pngInfo.height -ne $namedHeight)) {
                Add-CaptureFailure $captureIssues "Screenshot '$fileName' dimensions do not match its basename."
            }
        }
        catch {
            Add-CaptureFailure $captureIssues "Screenshot '$fileName' could not be read."
        }
    }

    $logEvidence = [pscustomobject][ordered]@{
        provided = [bool]$logsWereProvided
        matched = $false
        logFileName = $null
        logSha256 = $null
        complete = $null
        failure = $null
        requestedWidth = $null
        requestedHeight = $null
        screenWidth = $null
        screenHeight = $null
        pngWidth = $null
        pngHeight = $null
        sampleCount = $null
        nearBlackSampleCount = $null
        minimumBrightness = $null
        maximumBrightness = $null
        pixelEvidenceAccepted = $null
    }

    if ($logsWereProvided) {
        if (-not $logEvidenceByCaptureName.ContainsKey($fileName)) {
            Add-CaptureFailure $captureIssues "Screenshot '$fileName' has no matching supplied capture log."
        }
        else {
            $summary = $logEvidenceByCaptureName[$fileName]
            $logEvidence = [pscustomobject][ordered]@{
                provided = $true
                matched = $true
                logFileName = $summary.logFileName
                logSha256 = $summary.logSha256
                complete = $summary.complete
                failure = $summary.failure
                requestedWidth = $summary.requestedWidth
                requestedHeight = $summary.requestedHeight
                screenWidth = $summary.screenWidth
                screenHeight = $summary.screenHeight
                pngWidth = $summary.pngWidth
                pngHeight = $summary.pngHeight
                sampleCount = $summary.sampleCount
                nearBlackSampleCount = $summary.nearBlackSampleCount
                minimumBrightness = $summary.minimumBrightness
                maximumBrightness = $summary.maximumBrightness
                pixelEvidenceAccepted = $summary.pixelEvidenceAccepted
            }

            if (-not $summary.complete -or
                -not [string]::Equals($summary.failure, 'None', [System.StringComparison]::OrdinalIgnoreCase)) {
                Add-CaptureFailure $captureIssues "Screenshot '$fileName' does not have accepted log evidence."
            }
            if (-not $summary.pixelEvidenceAccepted) {
                Add-CaptureFailure $captureIssues "Screenshot '$fileName' has invalid logged pixel evidence."
            }
            if ($null -ne $namedWidth -and
                ($summary.requestedWidth -ne $namedWidth -or
                    $summary.requestedHeight -ne $namedHeight -or
                    $summary.screenWidth -ne $namedWidth -or
                    $summary.screenHeight -ne $namedHeight -or
                    $summary.pngWidth -ne $namedWidth -or
                    $summary.pngHeight -ne $namedHeight)) {
                Add-CaptureFailure $captureIssues "Screenshot '$fileName' disagrees with its log dimensions."
            }
        }
    }

    $accepted = $captureIssues.Count -eq 0
    $captureId = if ([string]::IsNullOrWhiteSpace($captureKey)) { $fileName } else { $captureKey }
    $record = [pscustomobject][ordered]@{
        id = $captureId
        scenario = $scenario
        namedWidth = $namedWidth
        namedHeight = $namedHeight
        fileName = $fileName
        exists = [bool]$fileExists
        byteLength = $byteLength
        sha256 = $fileHash
        png = $pngInfo
        logEvidence = $logEvidence
        deterministicAccepted = [bool]$accepted
        issues = @($captureIssues | Sort-Object -Unique)
    }
    [void]$captureRecords.Add($record)

    if ($accepted -and -not [string]::IsNullOrWhiteSpace($captureKey)) {
        $acceptedByKey[$captureKey] = $record
    }
}

if ($screenshotInputs.Count -eq 0) {
    Add-ValidationFailure 'At least one explicit screenshot path is required.'
}

foreach ($loggedCaptureName in @($logEvidenceByCaptureName.Keys | Sort-Object)) {
    if (-not $seenInputNames.ContainsKey($loggedCaptureName)) {
        Add-ValidationWarning "Capture log mentions '$loggedCaptureName', which was not an explicit screenshot input."
    }
}

$coverageRecords = [System.Collections.ArrayList]::new()
foreach ($key in @($expectedByKey.Keys | Sort-Object)) {
    $expected = $expectedByKey[$key]
    $covered = $acceptedByKey.ContainsKey($key)
    if (-not $covered) {
        Add-ValidationFailure "Expected capture '$key' is missing or invalid."
    }

    [void]$coverageRecords.Add([pscustomobject][ordered]@{
        id = $key
        scenario = $expected.scenario
        width = $expected.width
        height = $expected.height
        covered = [bool]$covered
        fileName = if ($covered) { $acceptedByKey[$key].fileName } else { $null }
    })
}

foreach ($key in @($acceptedByKey.Keys | Sort-Object)) {
    if (-not $expectedByKey.ContainsKey($key)) {
        Add-ValidationWarning "Capture '$key' is valid but outside the expected matrix."
    }
}

$sortedCaptures = @($captureRecords | Sort-Object id, fileName)
$sortedLogs = @($logRecords | Sort-Object fileName)
$captureSetLines = @(
    foreach ($capture in $sortedCaptures) {
        $hashValue = if ([string]::IsNullOrWhiteSpace($capture.sha256)) { 'unreadable' } else { $capture.sha256 }
        "$($capture.id)|$($capture.fileName)|$hashValue|$($capture.deterministicAccepted)"
    }
)
$captureSetSha256 = Get-TextSha256Hex ($captureSetLines -join "`n")
$failures = @($validationFailures | Sort-Object -Unique)
$warnings = @($validationWarnings | Sort-Object -Unique)
$deterministicPassed = $failures.Count -eq 0
$deterministicExitCode = if ($deterministicPassed) { 0 } else { 2 }

$manifest = [pscustomobject][ordered]@{
    schemaVersion = '1.0'
    releaseVersion = $ReleaseVersion
    generatedBy = 'NewVisualQaPacket.ps1'
    captureSetSha256 = $captureSetSha256
    deterministic = [pscustomobject][ordered]@{
        passed = [bool]$deterministicPassed
        exitCode = [int]$deterministicExitCode
        failures = $failures
        warnings = $warnings
    }
    expectedCoverage = @($coverageRecords)
    captures = $sortedCaptures
    captureLogs = [pscustomobject][ordered]@{
        provided = [bool]$logsWereProvided
        rawContentIncluded = $false
        records = $sortedLogs
    }
    aiReview = [pscustomobject][ordered]@{
        performed = $false
        advisoryOnly = $true
        changesDeterministicStatus = $false
        reviewSchema = 'Docs/AI_VISUAL_QA_REVIEW.schema.json'
        externalTransmissionRequiresExplicitApproval = $true
        futureApiStore = $false
    }
}

$markdown = [System.Text.StringBuilder]::new()
[void]$markdown.AppendLine("# AI Visual QA Packet - $ReleaseVersion")
[void]$markdown.AppendLine()
$statusText = if ($deterministicPassed) { 'PASS' } else { 'FAIL' }
[void]$markdown.AppendLine("Deterministic status: **$statusText**")
[void]$markdown.AppendLine()
[void]$markdown.AppendLine("Capture-set SHA-256: ``$captureSetSha256``")
[void]$markdown.AppendLine()
[void]$markdown.AppendLine('This packet is a local, deterministic preflight plus a complete manual-review aid. AI review is optional and advisory.')
[void]$markdown.AppendLine()
[void]$markdown.AppendLine('## Expected coverage')
[void]$markdown.AppendLine()
[void]$markdown.AppendLine('| Capture | Resolution | Covered | File |')
[void]$markdown.AppendLine('|---|---:|:---:|---|')
foreach ($coverage in @($coverageRecords)) {
    $mark = if ($coverage.covered) { 'yes' } else { 'no' }
    [void]$markdown.AppendLine(
        "| $(ConvertTo-MarkdownCell $coverage.scenario) | $($coverage.width)x$($coverage.height) | $mark | $(ConvertTo-MarkdownCell $coverage.fileName) |")
}

[void]$markdown.AppendLine()
[void]$markdown.AppendLine('## Capture evidence')
[void]$markdown.AppendLine()
[void]$markdown.AppendLine('| Capture id | PNG dimensions | Accepted | Log evidence | SHA-256 |')
[void]$markdown.AppendLine('|---|---:|:---:|:---:|---|')
foreach ($capture in $sortedCaptures) {
    $dimensions = if ($capture.png.ihdrValid) { "$($capture.png.width)x$($capture.png.height)" } else { 'invalid' }
    $acceptedText = if ($capture.deterministicAccepted) { 'yes' } else { 'no' }
    $logText = if (-not $capture.logEvidence.provided) { 'not supplied' } elseif ($capture.logEvidence.matched) { 'matched' } else { 'missing' }
    [void]$markdown.AppendLine(
        "| $(ConvertTo-MarkdownCell $capture.id) | $dimensions | $acceptedText | $logText | $(ConvertTo-MarkdownCell $capture.sha256) |")
}

[void]$markdown.AppendLine()
[void]$markdown.AppendLine('## Deterministic findings')
[void]$markdown.AppendLine()
if ($failures.Count -eq 0) {
    [void]$markdown.AppendLine('- No deterministic failures.')
}
else {
    foreach ($failure in $failures) {
        [void]$markdown.AppendLine("- $failure")
    }
}
if ($warnings.Count -gt 0) {
    [void]$markdown.AppendLine()
    [void]$markdown.AppendLine('Warnings:')
    foreach ($warning in $warnings) {
        [void]$markdown.AppendLine("- $warning")
    }
}

[void]$markdown.AppendLine()
[void]$markdown.AppendLine('## Manual fallback checklist')
[void]$markdown.AppendLine()
[void]$markdown.AppendLine('- [ ] Open every PNG and confirm it is the intended staged game state, not a desktop capture.')
[void]$markdown.AppendLine('- [ ] Check panel overlap, clipping, off-screen controls, and debug artifacts.')
[void]$markdown.AppendLine('- [ ] Check text legibility, contrast, hierarchy, focus, and selected/disabled states.')
[void]$markdown.AppendLine('- [ ] Compare the same scenario across resolutions for missing or materially displaced information.')
[void]$markdown.AppendLine('- [ ] Compare with the last approved same-resolution baseline and disposition every unexpected change.')
[void]$markdown.AppendLine('- [ ] Perform live checks for input, timing, animation, audio, controller behavior, and newcomer comprehension.')

[void]$markdown.AppendLine()
[void]$markdown.AppendLine('## Optional AI second pass')
[void]$markdown.AppendLine()
[void]$markdown.AppendLine('Review only the listed PNGs plus `visual-qa-packet.json`. Never transmit raw capture logs or arbitrary desktop captures.')
[void]$markdown.AppendLine('Validate advisory output against `Docs/AI_VISUAL_QA_REVIEW.schema.json`, record visible evidence and uncertainty, and require human disposition.')
[void]$markdown.AppendLine('A future external adapter must require explicit approval and use `store: false`; model output never changes the deterministic status above.')

$packetJson = $manifest | ConvertTo-Json -Depth 12
$packetMarkdown = $markdown.ToString()
$utf8NoBom = [System.Text.UTF8Encoding]::new($false)

try {
    $resolvedOutputDirectory = [System.IO.Path]::GetFullPath($OutputDirectory)
    [void][System.IO.Directory]::CreateDirectory($resolvedOutputDirectory)
    $jsonOutputPath = [System.IO.Path]::Combine($resolvedOutputDirectory, 'visual-qa-packet.json')
    $markdownOutputPath = [System.IO.Path]::Combine($resolvedOutputDirectory, 'visual-qa-packet.md')
    [System.IO.File]::WriteAllText($jsonOutputPath, $packetJson + "`n", $utf8NoBom)
    [System.IO.File]::WriteAllText($markdownOutputPath, $packetMarkdown, $utf8NoBom)
}
catch {
    Write-Error 'The visual QA packet could not be written to the explicit output directory.'
    exit 1
}

Write-Host "Deterministic visual QA packet: $statusText"
Write-Host 'Wrote visual-qa-packet.json and visual-qa-packet.md.'
exit $deterministicExitCode
