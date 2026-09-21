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
$outputRoot = Join-Path $projectRoot ('QA\world-sprites\' + $runName)
New-Item -ItemType Directory -Path $outputRoot | Out-Null
$shots = @(
    @{ Name = 'npc-kate-local'; Flags = '-ashen-contact-smoke kate'; Marker = 'contact=kate,' },
    @{ Name = 'npc-lute-local'; Flags = '-ashen-contact-smoke lute'; Marker = 'contact=lute,' },
    @{ Name = 'npc-dock-local'; Flags = '-ashen-contact-smoke dock'; Marker = 'contact=dock,' },
    @{ Name = 'npc-scholar-local'; Flags = '-ashen-contact-smoke scholar'; Marker = 'contact=scholar,' },
    @{ Name = 'npc-kate-region'; Flags = '-ashen-contact-smoke kate -ashen-region-smoke'; Marker = 'contact=kate,' },
    @{ Name = 'landmark-region'; Flags = '-ashen-explore-smoke -ashen-region-site-smoke'; Marker = 'focused on charted site' }
)
$results = @()
foreach ($shot in $shots) {
    $name = $shot.Name + '-' + $Width + 'x' + $Height
    $png = Join-Path $outputRoot ($name + '.png')
    $log = Join-Path $outputRoot ($name + '.log')
    $arguments = '-screen-fullscreen 0 -screen-width ' + $Width + ' -screen-height ' + $Height +
        ' -force-d3d11 -ashen-seed 2525 ' + $shot.Flags + ' -ashen-capture "' + $png +
        '" -ashen-capture-quit -logFile "' + $log + '"'
    $windowStyle = if ($Visible) { 'Normal' } else { 'Hidden' }
    $process = Start-Process -FilePath $player -ArgumentList $arguments -PassThru -WindowStyle $windowStyle
    $deadline = [DateTime]::UtcNow.AddSeconds(60)
    while (-not $process.WaitForExit(1000)) {
        if ([DateTime]::UtcNow -ge $deadline) {
            $process.Kill()
            $process.WaitForExit()
            throw "World sprite capture timed out: $name. See $log"
        }
    }
    $logText = Get-Content -LiteralPath $log -Raw
    if ($process.ExitCode -ne 0 -or $logText -notmatch 'complete=True' -or
        $logText -notmatch [regex]::Escape($shot.Marker) -or
        $logText -match 'Exception:|visual smoke capture failed:|Error:|atlas.*missing') {
        throw "World sprite capture failed: $name (exit $($process.ExitCode)). See $log"
    }
    if (-not (Test-Path -LiteralPath $png -PathType Leaf)) { throw "Capture missing: $png" }
    $results += [pscustomobject]@{ Scenario = $shot.Name; Png = $png; Log = $log }
    Write-Host "World sprite capture passed: $name"
}
& (Join-Path $PSScriptRoot 'NewVisualQaPacket.ps1') `
    -ScreenshotPath @($results.Png) `
    -CaptureLogPath @($results.Log) `
    -OutputDirectory (Join-Path $outputRoot 'visual-qa-packet') `
    -ExpectedCapture @($shots | ForEach-Object { $_.Name + '@' + $Width + 'x' + $Height })
Write-Host "WORLD SPRITE CAPTURES: $outputRoot"
