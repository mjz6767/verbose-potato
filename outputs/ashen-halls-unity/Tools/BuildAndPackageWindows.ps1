param(
    [string]$UnityExe = "C:\Program Files\Unity\Hub\Editor\6000.3.18f1\Editor\Unity.exe",
    [string]$Version = "",
    [switch]$SkipUnityBuild,
    [int]$BuildLogWaitSeconds = 90
)

$ErrorActionPreference = "Stop"

function Read-UnityBuildLogWithRetry {
    param(
        [string]$Path,
        [int]$TimeoutSeconds
    )

    $deadline = (Get-Date).AddSeconds([Math]::Max(1, $TimeoutSeconds))
    $lastText = ""
    do {
        if (Test-Path -LiteralPath $Path) {
            try {
                $lastText = Get-Content -LiteralPath $Path -Raw
                if ($lastText -match "Scripts have compiler errors|error CS\d+|Build Finished, Result: Failed") {
                    throw "Unity build did not complete successfully. See $Path"
                }
                if ($lastText -match "Build Finished, Result: Success") {
                    return $lastText
                }
            } catch [System.IO.IOException] {
                Start-Sleep -Milliseconds 500
                continue
            }
        }
        Start-Sleep -Milliseconds 500
    } while ((Get-Date) -lt $deadline)

    if (-not (Test-Path -LiteralPath $Path)) {
        throw "Unity build did not produce a log after $TimeoutSeconds seconds: $Path"
    }

    if ([string]::IsNullOrWhiteSpace($lastText)) {
        $lastText = Get-Content -LiteralPath $Path -Raw
    }
    throw "Unity build log did not report success after $TimeoutSeconds seconds. See $Path"
}

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$VersionInfoPath = Join-Path $ProjectRoot "Assets\Scripts\VersionInfo.cs"
if (-not (Test-Path -LiteralPath $VersionInfoPath)) {
    throw "VersionInfo.cs not found: $VersionInfoPath"
}
$VersionInfoText = Get-Content -LiteralPath $VersionInfoPath -Raw
if ($VersionInfoText -notmatch 'PackageVersion\s*=\s*"([^"]+)"') {
    throw "Could not read PackageVersion from $VersionInfoPath"
}
$SourceVersion = $Matches[1]
if ($VersionInfoText -notmatch 'ExecutableBaseName\s*=\s*"([^"]+)"') {
    throw "Could not read ExecutableBaseName from $VersionInfoPath"
}
$ExecutableBaseName = $Matches[1]
if ([string]::IsNullOrWhiteSpace($Version)) {
    $Version = $SourceVersion
} elseif ($Version -ne $SourceVersion) {
    throw "Requested package version '$Version' does not match source PackageVersion '$SourceVersion'"
}

foreach ($RelativeDoc in @("README_PLAY.txt", "CHANGELOG.md", "KNOWN_ISSUES.txt")) {
    $DocPath = Join-Path $ProjectRoot $RelativeDoc
    if (-not (Test-Path -LiteralPath $DocPath)) {
        throw "Required release doc is missing: $RelativeDoc"
    }
    $DocText = Get-Content -LiteralPath $DocPath -Raw
    if ($DocText -notmatch [regex]::Escape($Version)) {
        throw "$RelativeDoc does not mention $Version"
    }
}

$OutputsRoot = Resolve-Path (Join-Path $ProjectRoot "..")
$BuildFolder = Join-Path $OutputsRoot "ash-and-brimstone-build\$ExecutableBaseName-Windows-$Version"
$ZipPath = Join-Path $OutputsRoot "$ExecutableBaseName-Windows-$Version.zip"
$BuildLog = Join-Path $ProjectRoot "build-$Version.log"

if (-not $SkipUnityBuild) {
    if (-not (Test-Path -LiteralPath $UnityExe)) {
        throw "Unity executable not found: $UnityExe"
    }

    if (Test-Path -LiteralPath $BuildLog) {
        Remove-Item -LiteralPath $BuildLog -Force
    }
    $UnityArguments = @(
        "-batchmode",
        "-quit",
        "-projectPath", $ProjectRoot,
        "-executeMethod", "AshenHalls.Editor.BuildWindows.Build",
        "-logFile", $BuildLog
    )
    $BuildProcess = Start-Process -FilePath $UnityExe -ArgumentList $UnityArguments -PassThru -Wait
    if ($BuildProcess.ExitCode -ne 0) {
        throw "Unity build process exited with code $($BuildProcess.ExitCode). See $BuildLog"
    }
    $BuildText = Read-UnityBuildLogWithRetry -Path $BuildLog -TimeoutSeconds $BuildLogWaitSeconds
}

$ExePath = Join-Path $BuildFolder "$ExecutableBaseName.exe"
if (-not (Test-Path -LiteralPath $ExePath)) {
    throw "Build folder is missing $ExecutableBaseName.exe: $BuildFolder"
}

$TempZipPath = "$ZipPath.tmp.zip"
if (Test-Path -LiteralPath $TempZipPath) {
    Remove-Item -LiteralPath $TempZipPath -Force
}

Compress-Archive -LiteralPath $BuildFolder -DestinationPath $TempZipPath -Force

$TempZip = Get-Item -LiteralPath $TempZipPath
if ($TempZip.Length -lt 1MB) {
    throw "Package zip was not created correctly: $TempZipPath"
}

Move-Item -LiteralPath $TempZipPath -Destination $ZipPath -Force
$Zip = Get-Item -LiteralPath $ZipPath
Write-Host "Ash & Brimstone package complete: $ZipPath"
Write-Host "Size: $($Zip.Length) bytes"
