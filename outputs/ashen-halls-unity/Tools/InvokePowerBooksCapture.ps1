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
$outputRoot = Join-Path $projectRoot ('QA\power-books\' + $runName)
New-Item -ItemType Directory -Path $outputRoot | Out-Null
$shots = @(
    @{ Name = 'beta-title'; Flags = '-ashen-beta-title-smoke'; Marker = 'beta title smoke passed' },
    @{ Name = 'priest-selected'; Flags = '-ashen-spellbook-smoke -ashen-spell-school mend -ashen-book-state selected'; Marker = 'visual smoke mode: mend spellbook.' },
    @{ Name = 'priest-capstone'; Flags = '-ashen-spellbook-smoke -ashen-spell-school mend -ashen-book-state selected -ashen-book-bottom'; Marker = 'visual smoke mode: mend spellbook.' },
    @{ Name = 'warrior-selected'; Flags = '-ashen-skills-smoke -ashen-skill-class warrior -ashen-book-state selected'; Marker = 'visual smoke mode: combat skills / warrior.' },
    @{ Name = 'warrior-capstone'; Flags = '-ashen-skills-smoke -ashen-skill-class warrior -ashen-book-state selected -ashen-book-bottom'; Marker = 'visual smoke mode: combat skills / warrior.' },
    @{ Name = 'rogue-selected'; Flags = '-ashen-skills-smoke -ashen-skill-class rogue -ashen-book-state selected'; Marker = 'visual smoke mode: combat skills / rogue.' },
    @{ Name = 'ranger-selected'; Flags = '-ashen-skills-smoke -ashen-skill-class ranger -ashen-book-state selected'; Marker = 'visual smoke mode: combat skills / ranger.' }
)
$results = @()
foreach ($shot in $shots) {
    $name = $shot.Name + '-' + $Width + 'x' + $Height
    $png = Join-Path $outputRoot ($name + '.png')
    $log = Join-Path $outputRoot ($name + '.log')
    $arguments = '-screen-fullscreen 0 -screen-width ' + $Width + ' -screen-height ' + $Height +
        ' -force-d3d11 -ashen-seed 2525 ' + $shot.Flags + ' -ashen-capture "' + $png +
        '" -ashen-capture-quit -logFile "' + $log + '"'
    # Visible windows require the supervising user's permission. Hidden D3D
    # screenshots may be black and must still pass the runtime pixel checks.
    $windowStyle = if ($Visible) { 'Normal' } else { 'Hidden' }
    $process = Start-Process -FilePath $player -ArgumentList $arguments -PassThru -WindowStyle $windowStyle
    $deadline = [DateTime]::UtcNow.AddSeconds(60)
    while (-not $process.WaitForExit(1000)) {
        if ([DateTime]::UtcNow -ge $deadline) {
            $process.Kill()
            $process.WaitForExit()
            throw "Power book capture timed out: $name. See $log"
        }
    }
    $logText = Get-Content -LiteralPath $log -Raw
    if ($process.ExitCode -ne 0 -or $logText -notmatch 'complete=True' -or
        $logText -notmatch [regex]::Escape($shot.Marker) -or
        $logText -match 'Exception:|visual smoke capture failed:|Error:|atlas.*missing') {
        throw "Power book capture failed: $name (exit $($process.ExitCode)). See $log"
    }
    if (-not (Test-Path -LiteralPath $png -PathType Leaf)) { throw "Capture missing: $png" }
    $results += [pscustomobject]@{ Scenario = $shot.Name; Png = $png; Log = $log }
    Write-Host "Power book capture passed: $name"
}
& (Join-Path $PSScriptRoot 'NewVisualQaPacket.ps1') `
    -ScreenshotPath @($results.Png) `
    -CaptureLogPath @($results.Log) `
    -OutputDirectory (Join-Path $outputRoot 'visual-qa-packet') `
    -ExpectedCapture @($shots | ForEach-Object { $_.Name + '@' + $Width + 'x' + $Height })
Write-Host "POWER BOOK CAPTURES: $outputRoot"
