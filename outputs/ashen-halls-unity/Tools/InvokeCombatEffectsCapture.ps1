param(
    [Parameter(Mandatory = $true)][string]$PlayerPath,
    [ValidateRange(960, 3840)][int]$Width = 1280,
    [ValidateRange(600, 2160)][int]$Height = 720,
    [switch]$Demonic,
    [switch]$Visible
)

$ErrorActionPreference = 'Stop'
$player = (Resolve-Path -LiteralPath $PlayerPath).Path
$projectRoot = [IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))
$runName = [DateTime]::UtcNow.ToString('yyyyMMdd-HHmmss') + '-' + [Guid]::NewGuid().ToString('N').Substring(0, 8)
$outputRoot = Join-Path $projectRoot ('QA\combat-effects\' + $runName)
New-Item -ItemType Directory -Path $outputRoot | Out-Null
$shots = @(
    @{ Power = 'FBL'; Phase = 'cast' },
    @{ Power = 'FBL'; Phase = 'travel' },
    @{ Power = 'FBL'; Phase = 'impact' },
    @{ Power = 'FBL'; Phase = 'aftermath' },
    @{ Power = 'RCL'; Phase = 'travel' },
    @{ Power = 'RBT'; Phase = 'travel' },
    @{ Power = 'charge'; Phase = 'impact' },
    @{ Power = 'whirlwind'; Phase = 'impact' },
    @{ Power = 'volley'; Phase = 'travel' },
    @{ Power = 'FBL'; Phase = 'impact'; Reduced = $true }
)
if ($Demonic) {
    $shots = @(
        @{ Power = 'IBD'; Phase = 'impact' },
        @{ Power = 'IBF'; Phase = 'impact' },
        @{ Power = 'IBG'; Phase = 'cast' },
        @{ Power = 'IBG'; Phase = 'impact' },
        @{ Power = 'IBG'; Phase = 'aftermath' },
        @{ Power = 'DFA'; Phase = 'cast' },
        @{ Power = 'DFA'; Phase = 'impact' },
        @{ Power = 'DFA'; Phase = 'aftermath' },
        @{ Power = 'RLM'; Phase = 'cast' },
        @{ Power = 'RLM'; Phase = 'impact' },
        @{ Power = 'RLM'; Phase = 'aftermath' },
        @{ Power = 'RBT'; Phase = 'travel' },
        @{ Power = 'IBG'; Phase = 'impact'; Reduced = $true },
        @{ Power = 'DFA'; Phase = 'impact'; Reduced = $true }
    )
}
$results = @()
foreach ($shot in $shots) {
    $name = $shot.Power.ToLowerInvariant() + '-' + $shot.Phase
    if ($shot.Reduced) { $name += '-reduced' }
    $png = Join-Path $outputRoot ($name + '-' + $Width + 'x' + $Height + '.png')
    $log = Join-Path $outputRoot ($name + '.log')
    $arguments = '-screen-fullscreen 0 -screen-width ' + $Width + ' -screen-height ' + $Height +
        ' -force-d3d11 -popupwindow -ashen-feedback-smoke -ashen-feedback-power ' + $shot.Power +
        ' -ashen-feedback-phase ' + $shot.Phase + ' -ashen-capture "' + $png +
        '" -ashen-capture-quit -logFile "' + $log + '"'
    if ($shot.Reduced) { $arguments += ' -ashen-feedback-reduced-motion' }
    # Hidden D3D windows can produce black screenshots on some desktops.
    # Visible windows are opt-in for a person running a supervised review.
    $windowStyle = if ($Visible) { 'Normal' } else { 'Hidden' }
    $process = Start-Process -FilePath $player -ArgumentList $arguments -PassThru -WindowStyle $windowStyle
    $deadline = [DateTime]::UtcNow.AddSeconds(45)
    while (-not $process.WaitForExit(1000)) {
        if ([DateTime]::UtcNow -ge $deadline) {
            $process.Kill()
            $process.WaitForExit()
            throw "Combat capture timed out: $name. See $log"
        }
    }
    $logText = Get-Content -LiteralPath $log -Raw
    if ($process.ExitCode -ne 0 -or $logText -notmatch 'complete=True' -or
        $logText -notmatch ('selected feedback capture: power=' + [regex]::Escape($shot.Power) + ', phase=' + $shot.Phase) -or
        $logText -match 'Exception:|visual smoke capture failed:|Error:') {
        throw "Combat capture failed: $name (exit $($process.ExitCode)). See $log"
    }
    if (-not (Test-Path -LiteralPath $png -PathType Leaf)) { throw "Capture missing: $png" }
    $results += [pscustomobject]@{ Power = $shot.Power; Phase = $shot.Phase; ReducedMotion = [bool]$shot.Reduced; Png = $png; Log = $log }
    Write-Host "Combat capture passed: $name"
}
$results | ConvertTo-Json -Depth 3 | Set-Content -LiteralPath (Join-Path $outputRoot 'captures.json') -Encoding utf8
Write-Host "COMBAT EFFECTS CAPTURES: $outputRoot"
