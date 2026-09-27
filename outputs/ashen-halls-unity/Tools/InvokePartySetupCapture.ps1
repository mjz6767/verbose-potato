param(
    [Parameter(Mandatory = $true)][string]$PlayerPath,
    [switch]$AllCombinations
)

$ErrorActionPreference = 'Stop'
$player = (Resolve-Path -LiteralPath $PlayerPath).Path
$projectRoot = [IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))
$runName = [DateTime]::UtcNow.ToString('yyyyMMdd-HHmmss') + '-' + [Guid]::NewGuid().ToString('N').Substring(0, 8)
$outputRoot = Join-Path $projectRoot ('QA\party-setup\player-' + $runName)
New-Item -ItemType Directory -Path $outputRoot -Force | Out-Null
$examples = @(
    @{ Race = 'human'; Class = 'warrior'; Member = 0; Details = $false },
    @{ Race = 'dusk elf'; Class = 'ranger'; Member = 1; Details = $false },
    @{ Race = 'stoneborn'; Class = 'paladin'; Member = 2; Details = $false },
    @{ Race = 'fenkin'; Class = 'rogue'; Member = 3; Details = $false },
    @{ Race = 'ashling'; Class = 'warlock'; Member = 0; Details = $false },
    @{ Race = 'human'; Class = 'mage'; Member = 1; Details = $true }
)
$shots = @(
    foreach ($size in @(@{ W = 960; H = 600 }, @{ W = 1280; H = 720 }, @{ W = 1920; H = 1080 })) {
        foreach ($example in $examples) {
            [pscustomobject]@{ Race = $example.Race; Class = $example.Class; Member = $example.Member; Details = $example.Details; W = $size.W; H = $size.H }
        }
    }
    if ($AllCombinations) {
        foreach ($race in @('human', 'dusk elf', 'stoneborn', 'fenkin', 'ashling')) {
            foreach ($classKey in @('rogue', 'warrior', 'ranger', 'wizard', 'mage', 'warlock', 'priest', 'paladin')) {
                [pscustomobject]@{ Race = $race; Class = $classKey; Member = 0; Details = $false; W = 1280; H = 720 }
            }
        }
    }
)
$results = @()
$completedNames = @{}
foreach ($shot in $shots) {
    $scenario = $shot.Race.Replace(' ', '-') + '-' + $shot.Class + $(if ($shot.Details) { '-details' } else { '' })
    $name = $scenario + '-' + $shot.W + 'x' + $shot.H
    if ($completedNames.ContainsKey($name)) { continue }
    $png = Join-Path $outputRoot ($name + '.png')
    $log = Join-Path $outputRoot ($name + '.log')
    $arguments = '-screen-fullscreen 0 -screen-width ' + $shot.W + ' -screen-height ' + $shot.H +
        ' -force-d3d11 -ashen-seed 2828 -ashen-party-setup-smoke -ashen-party-race "' + $shot.Race +
        '" -ashen-party-class ' + $shot.Class + ' -ashen-party-member ' + $shot.Member +
        $(if ($shot.Details) { ' -ashen-party-details' } else { '' }) +
        ' -ashen-capture "' + $png + '" -ashen-capture-quit -logFile "' + $log + '"'
    $process = Start-Process -FilePath $player -ArgumentList $arguments -PassThru -WindowStyle Hidden
    $deadline = [DateTime]::UtcNow.AddSeconds(60)
    while (-not $process.WaitForExit(1000)) {
        if ([DateTime]::UtcNow -ge $deadline) {
            $process.Kill()
            $process.WaitForExit()
            throw "Party Setup capture timed out: $name. See $log"
        }
    }
    $logText = Get-Content -LiteralPath $log -Raw
    $identity = 'race=' + $shot.Race + ', class=' + $shot.Class + ','
    $tabMarker = 'tab=' + $(if ($shot.Details) { 'details,' } else { 'identity,' })
    if ($process.ExitCode -ne 0 -or $logText -notmatch 'complete=True' -or $logText -notmatch 'failure=None' -or
        $logText -notmatch [regex]::Escape($identity) -or $logText -notmatch [regex]::Escape($tabMarker) -or
        $logText -notmatch 'actual player canvas / offscreen camera' -or
        $logText -match 'Exception:|visual smoke capture failed:|Error:') {
        throw "Party Setup capture failed: $name (exit $($process.ExitCode)). See $log"
    }
    if (-not (Test-Path -LiteralPath $png -PathType Leaf)) { throw "Capture missing: $png" }
    $results += [pscustomobject]@{ Scenario = $scenario; Width = $shot.W; Height = $shot.H; Png = $png; Log = $log }
    $completedNames[$name] = $true
    Write-Host "Party Setup capture passed: $name (offscreen player canvas)"
}
& (Join-Path $PSScriptRoot 'NewVisualQaPacket.ps1') `
    -ScreenshotPath @($results.Png) `
    -CaptureLogPath @($results.Log) `
    -OutputDirectory (Join-Path $outputRoot 'visual-qa-packet') `
    -ExpectedCapture @($results | ForEach-Object { $_.Scenario + '@' + $_.Width + 'x' + $_.Height })
Write-Host "PARTY SETUP CAPTURES: $outputRoot"
