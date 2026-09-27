param(
    [Parameter(Mandatory = $true)][string]$PlayerPath,
    [ValidateRange(960, 3840)][int]$Width = 1280,
    [ValidateRange(600, 2160)][int]$Height = 720,
    [switch]$Visible
)
$ErrorActionPreference = 'Stop'
$player = (Resolve-Path -LiteralPath $PlayerPath).Path
$projectRoot = [IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))
$runName = [DateTime]::UtcNow.ToString('yyyyMMdd-HHmmss') + '-' + [Guid]::NewGuid().ToString('N').Substring(0, 8)
$outputRoot = Join-Path $projectRoot ('QA/experience-polish/' + $runName)
New-Item -ItemType Directory -Path $outputRoot | Out-Null
$shots = @(
    @{Name='loot';Flags='-ashen-loot-smoke';Marker='visual smoke mode: acquired loot.'},
    @{Name='town';Flags='-ashen-contact-smoke lute';Marker='contact=lute,'},
    @{Name='route';Flags='-ashen-route-smoke';Marker='visual smoke mode: route exploration.'},
    @{Name='forest-shrine';Flags='-ashen-explore-smoke -ashen-site-smoke green-shrine-training-ring';Marker='site=green-shrine-training-ring,'},
    @{Name='fen-grove';Flags='-ashen-explore-smoke -ashen-site-smoke ash-fen-ancient-grove';Marker='site=ash-fen-ancient-grove,'},
    @{Name='quarry';Flags='-ashen-explore-smoke -ashen-site-smoke old-quarry-forge';Marker='site=old-quarry-forge,'},
    @{Name='town-region';Flags='-ashen-contact-smoke kate -ashen-region-smoke';Marker='contact=kate,'}
)
$results = @()
foreach ($shot in $shots) {
    $name = $shot.Name + '-' + $Width + 'x' + $Height
    $png = Join-Path $outputRoot ($name + '.png')
    $log = Join-Path $outputRoot ($name + '.log')
    $arguments = '-screen-fullscreen 0 -screen-width ' + $Width + ' -screen-height ' + $Height +
        ' -force-d3d11 -ashen-seed 2525 ' + $shot.Flags + ' -ashen-capture "' + $png +
        '" -ashen-capture-quit -logFile "' + $log + '"'
    $style = if ($Visible) { 'Normal' } else { 'Hidden' }
    $process = Start-Process -FilePath $player -ArgumentList $arguments -WindowStyle $style -PassThru
    if (-not $process.WaitForExit(60000)) {
        $process.Kill()
        $process.WaitForExit()
        throw "Experience capture timed out: $name. See $log"
    }
    $logText = Get-Content -LiteralPath $log -Raw
    if ($process.ExitCode -ne 0 -or $logText -notmatch 'complete=True' -or
        $logText -notmatch [regex]::Escape($shot.Marker) -or
        $logText -match 'Exception:|visual smoke capture failed:|Error:|atlas.*missing|Rejected.*environment') {
        throw "Experience capture failed: $name (exit $($process.ExitCode)). See $log"
    }
    if (-not (Test-Path -LiteralPath $png -PathType Leaf)) { throw "Capture missing: $png" }
    $results += [pscustomobject]@{Scenario=$shot.Name;Png=$png;Log=$log}
    Write-Host "Experience capture passed: $name"
}
& (Join-Path $PSScriptRoot 'NewVisualQaPacket.ps1') -ScreenshotPath @($results.Png) `
    -CaptureLogPath @($results.Log) -OutputDirectory (Join-Path $outputRoot 'visual-qa-packet') `
    -ExpectedCapture @($shots | ForEach-Object { $_.Name + '@' + $Width + 'x' + $Height })
Write-Host "EXPERIENCE CAPTURES: $outputRoot"
