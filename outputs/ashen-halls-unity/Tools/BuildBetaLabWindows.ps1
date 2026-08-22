param(
    [string]$UnityExe = "C:\Program Files\Unity\Hub\Editor\6000.3.18f1\Editor\Unity.exe",
    [string]$LogPath = ""
)

$ErrorActionPreference = "Stop"

$projectRoot = [System.IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))
if (-not (Test-Path -LiteralPath $UnityExe -PathType Leaf)) {
    throw "Unity editor is missing: $UnityExe"
}

if ([string]::IsNullOrWhiteSpace($LogPath)) {
    $LogPath = Join-Path $projectRoot "build-beta-lab.log"
} else {
    $LogPath = [System.IO.Path]::GetFullPath($LogPath)
}

Write-Host "Building the opt-in Beta Development player..."
Write-Host "Project: $projectRoot"
Write-Host "Log: $LogPath"

$unityArguments =
    '-batchmode -nographics -quit -projectPath "' + $projectRoot +
    '" -executeMethod AshenHalls.Editor.BuildWindows.BuildBeta -logFile "' + $LogPath + '"'
$unityProcess = Start-Process `
    -FilePath $UnityExe `
    -ArgumentList $unityArguments `
    -PassThru `
    -WindowStyle Hidden
$unityProcess.WaitForExit()
$unityExitCode = $unityProcess.ExitCode
if ($unityExitCode -ne 0) {
    throw "Beta Lab Windows build failed with exit code $unityExitCode. See $LogPath"
}

$logText = Get-Content -LiteralPath $LogPath -Raw
if ($logText -notmatch "Beta Development Windows build complete") {
    throw "Unity exited without the expected Beta Development completion marker. See $LogPath"
}

$completionLine = @(
    Get-Content -LiteralPath $LogPath |
        Where-Object { $_ -match "Beta Development Windows build complete:" }
) | Select-Object -Last 1

if ([string]::IsNullOrWhiteSpace($completionLine) -or
    $completionLine -notmatch "Beta Development Windows build complete:\s*(.+\.exe)\s*$") {
    throw "Could not resolve the built Beta Development player from $LogPath"
}

$playerExe = [System.IO.Path]::GetFullPath($Matches[1].Trim())
$outputRoot = [System.IO.Path]::GetFullPath((Split-Path -Parent $playerExe))
$expectedBuildRoot = [System.IO.Path]::GetFullPath(
    (Join-Path (Split-Path -Parent $projectRoot) "ash-and-brimstone-build"))
$expectedPrefix = $expectedBuildRoot.TrimEnd(
    [System.IO.Path]::DirectorySeparatorChar,
    [System.IO.Path]::AltDirectorySeparatorChar) + [System.IO.Path]::DirectorySeparatorChar
if (-not $outputRoot.StartsWith($expectedPrefix, [System.StringComparison]::OrdinalIgnoreCase) -or
    -not $outputRoot.EndsWith("-beta-dev", [System.StringComparison]::OrdinalIgnoreCase) -or
    -not (Test-Path -LiteralPath $playerExe -PathType Leaf)) {
    throw "Refusing to smoke/package an unexpected Beta output: $outputRoot"
}

$artifactName = Split-Path -Leaf $outputRoot
$packageRoot = Split-Path -Parent $expectedBuildRoot
$zipPath = Join-Path $packageRoot ($artifactName + ".zip")
Write-Host "Creating the distinct Beta Development archive: $zipPath"
Compress-Archive `
    -LiteralPath $outputRoot `
    -DestinationPath $zipPath `
    -CompressionLevel Optimal `
    -Force
if (-not (Test-Path -LiteralPath $zipPath -PathType Leaf) -or
    (Get-Item -LiteralPath $zipPath).Length -le 0) {
    throw "Beta Development archive was not created: $zipPath"
}
$zipHash = (Get-FileHash -LiteralPath $zipPath -Algorithm SHA256).Hash.ToLowerInvariant()

$qaRoot = Join-Path $projectRoot "QA\beta-development"
New-Item -ItemType Directory -Force -Path $qaRoot | Out-Null
$betaSmokeLog = Join-Path $qaRoot ($artifactName + "-clean-extract.log")
$extractRoot = [System.IO.Path]::GetFullPath(
    (Join-Path $projectRoot ("Temp\BetaPackageSmoke-" + [Guid]::NewGuid().ToString("N"))))
$allowedExtractPrefix = [System.IO.Path]::GetFullPath(
    (Join-Path $projectRoot "Temp")).TrimEnd(
        [System.IO.Path]::DirectorySeparatorChar,
        [System.IO.Path]::AltDirectorySeparatorChar) + [System.IO.Path]::DirectorySeparatorChar
if (-not $extractRoot.StartsWith($allowedExtractPrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
    throw "Refusing to extract the Beta archive outside the Unity Temp directory: $extractRoot"
}

try {
    New-Item -ItemType Directory -Force -Path $extractRoot | Out-Null
    Expand-Archive -LiteralPath $zipPath -DestinationPath $extractRoot -Force
    $extractedPlayerExe = Join-Path (Join-Path $extractRoot $artifactName) (Split-Path -Leaf $playerExe)
    if (-not (Test-Path -LiteralPath $extractedPlayerExe -PathType Leaf)) {
        throw "Clean-extracted Beta player is missing: $extractedPlayerExe"
    }

    Write-Host "Verifying the clean-extracted Development player exposes the guarded title Beta Lab..."
    $playerArguments =
        '-batchmode -nographics -quit -ashen-beta-title-smoke -logFile "' + $betaSmokeLog + '"'
    $playerProcess = Start-Process `
        -FilePath $extractedPlayerExe `
        -ArgumentList $playerArguments `
        -PassThru `
        -WindowStyle Hidden
    $playerProcess.WaitForExit()
    $playerExitCode = $playerProcess.ExitCode
    if ($playerExitCode -ne 0) {
        throw "Clean-extracted Beta player smoke failed with exit code $playerExitCode. See $betaSmokeLog"
    }
    $betaSmokeText = Get-Content -LiteralPath $betaSmokeLog -Raw
    if ($betaSmokeText -notmatch "beta title smoke passed: development title exposes Beta Lab") {
        throw "Clean-extracted player did not emit the guarded Beta-title marker. See $betaSmokeLog"
    }
}
finally {
    if (Test-Path -LiteralPath $extractRoot) {
        Remove-Item -LiteralPath $extractRoot -Recurse -Force
    }
}

Write-Host "Beta Lab build passed all embedded gates."
Write-Host $completionLine
Write-Host "Clean-extracted Beta title smoke passed: $betaSmokeLog"
Write-Host "Archive: $zipPath"
Write-Host "SHA256: $zipHash"
Write-Host "This Development artifact exposes the title Beta Lab; lab sessions remain save-blocked."
