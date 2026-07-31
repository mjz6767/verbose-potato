param(
    [string]$UnityExe = "C:\Program Files\Unity\Hub\Editor\6000.3.18f1\Editor\Unity.exe",
    [string]$Version = "",
    [switch]$SkipUnityBuild,
    [int]$BuildLogWaitSeconds = 90,
    [switch]$SkipCleanPackageSmoke,
    [int]$PackageSmokeTimeoutSeconds = 45,
    [switch]$AllowDirtySource
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

function Get-StringSha256 {
    param([string]$Text)

    $sha256 = [System.Security.Cryptography.SHA256]::Create()
    try {
        $bytes = [System.Text.Encoding]::UTF8.GetBytes($Text)
        return ([System.BitConverter]::ToString($sha256.ComputeHash($bytes))).Replace("-", "").ToLowerInvariant()
    } finally {
        $sha256.Dispose()
    }
}

function Get-ReleaseSourceState {
    param([string]$ProjectRoot)

    $gitRoot = (& git -C $ProjectRoot rev-parse --show-toplevel 2>$null)
    if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($gitRoot)) {
        throw "Release packaging requires a Git worktree containing $ProjectRoot"
    }
    $gitRoot = $gitRoot.Trim()

    $gitCommit = (& git -C $gitRoot rev-parse HEAD 2>$null)
    if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($gitCommit)) {
        throw "Could not resolve the release source commit from $gitRoot"
    }

    $trackedStatusLines = @(& git -C $gitRoot status --porcelain --untracked-files=no 2>$null)
    if ($LASTEXITCODE -ne 0) {
        throw "Could not inspect tracked release source state in $gitRoot"
    }

    $unityRelativePath = (& git -C $ProjectRoot rev-parse --show-prefix 2>$null).Trim().TrimEnd('/')
    if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($unityRelativePath)) {
        throw "Could not resolve the Unity project path relative to $gitRoot"
    }
    $releaseInputPathspecs = @(
        "$unityRelativePath/Assets",
        "$unityRelativePath/Packages",
        "$unityRelativePath/ProjectSettings",
        "$unityRelativePath/Docs",
        "$unityRelativePath/Tools",
        "$unityRelativePath/README_PLAY.txt",
        "$unityRelativePath/CHANGELOG.md",
        "$unityRelativePath/KNOWN_ISSUES.txt",
        ":(exclude)$unityRelativePath/Docs/ArtReferences/**"
    )
    $untrackedReleaseInputs = @(
        & git -C $gitRoot ls-files --others -- $releaseInputPathspecs 2>$null
    )
    if ($LASTEXITCODE -ne 0) {
        throw "Could not inspect untracked release inputs in $ProjectRoot"
    }

    $statusLines = @(
        $trackedStatusLines
        $untrackedReleaseInputs | ForEach-Object { "?? $_" }
    )
    return [pscustomobject]@{
        GitRoot = $gitRoot
        Commit = $gitCommit.Trim()
        StatusLines = $statusLines
    }
}

function Get-PackagedArtFiles {
    param([string]$PackageRoot)

    $artRoot = Join-Path $PackageRoot "Docs\ArtReferences"
    $artManifestPath = Join-Path $PackageRoot "Docs\PACKAGED_ART.txt"
    if (-not (Test-Path -LiteralPath $artRoot)) {
        throw "Packaged art folder is missing: $artRoot"
    }
    if (-not (Test-Path -LiteralPath $artManifestPath)) {
        throw "Packaged art manifest is missing: $artManifestPath"
    }

    $manifestLines = @(Get-Content -LiteralPath $artManifestPath)
    $manifestFileNames = @(
        $manifestLines |
            ForEach-Object { $_.Trim() } |
            Where-Object { $_.EndsWith(".png", [System.StringComparison]::OrdinalIgnoreCase) }
    )
    if ($manifestFileNames.Count -eq 0) {
        throw "Packaged art manifest contains no PNG entries: $artManifestPath"
    }
    foreach ($fileName in $manifestFileNames) {
        if ([System.IO.Path]::GetFileName($fileName) -ne $fileName) {
            throw "Packaged art manifest entry must be a bare filename: $fileName"
        }
    }
    $duplicateEntries = @(
        $manifestFileNames |
            Group-Object |
            Where-Object Count -gt 1 |
            Select-Object -ExpandProperty Name
    )
    if ($duplicateEntries.Count -gt 0) {
        throw "Packaged art manifest contains duplicate PNG entries: $($duplicateEntries -join ', ')"
    }
    $declaredCountLines = @(
        $manifestLines | Where-Object { $_ -match '^Packaged PNG count:\s*\d+\s*$' }
    )
    if ($declaredCountLines.Count -ne 1) {
        throw "Packaged art manifest must contain exactly one declared PNG count: $artManifestPath"
    }
    $null = $declaredCountLines[0] -match '^Packaged PNG count:\s*(\d+)\s*$'
    $declaredCount = [int]$Matches[1]
    if ($declaredCount -ne $manifestFileNames.Count) {
        throw "Packaged art manifest declares $declaredCount PNGs but lists $($manifestFileNames.Count)."
    }
    $manifestFileNames = @($manifestFileNames | Sort-Object)

    $actualFileNames = @(
        Get-ChildItem -LiteralPath $artRoot -File -Filter "*.png" |
            Select-Object -ExpandProperty Name |
            Sort-Object -Unique
    )
    $manifestDelta = @(Compare-Object -ReferenceObject $manifestFileNames -DifferenceObject $actualFileNames)
    if ($manifestDelta.Count -gt 0) {
        $details = ($manifestDelta | ForEach-Object { "$($_.SideIndicator) $($_.InputObject)" }) -join "; "
        throw "Docs/PACKAGED_ART.txt does not match packaged PNG content: $details"
    }

    return @($manifestFileNames | ForEach-Object { Get-Item -LiteralPath (Join-Path $artRoot $_) })
}

function Get-PackagedArtSourceState {
    param(
        [string]$ProjectRoot,
        [string]$PackageRoot
    )

    $untracked = @()
    $missing = @()
    $hashMismatch = @()
    foreach ($packagedFile in @(Get-PackagedArtFiles -PackageRoot $PackageRoot)) {
        $relativePath = "Docs/ArtReferences/$($packagedFile.Name)"
        $sourcePath = Join-Path $ProjectRoot $relativePath
        if (-not (Test-Path -LiteralPath $sourcePath)) {
            $missing += $relativePath
            continue
        }

        & git -C $ProjectRoot ls-files --error-unmatch -- $relativePath 2>$null | Out-Null
        if ($LASTEXITCODE -ne 0) {
            $untracked += $relativePath
        }

        $sourceHash = (Get-FileHash -LiteralPath $sourcePath -Algorithm SHA256).Hash
        $packagedHash = (Get-FileHash -LiteralPath $packagedFile.FullName -Algorithm SHA256).Hash
        if ($sourceHash -ne $packagedHash) {
            $hashMismatch += $relativePath
        }
    }

    return [pscustomobject]@{
        Untracked = $untracked
        Missing = $missing
        HashMismatch = $hashMismatch
    }
}

function Assert-ReleaseSourceState {
    param(
        [string]$ProjectRoot,
        [bool]$AllowDirty
    )

    $sourceState = Get-ReleaseSourceState -ProjectRoot $ProjectRoot
    if ($sourceState.StatusLines.Count -gt 0) {
        $preview = ($sourceState.StatusLines | Select-Object -First 20) -join [Environment]::NewLine
        $message =
            "Release inputs are not committed at $($sourceState.Commit):" +
            [Environment]::NewLine + $preview
        if ($sourceState.StatusLines.Count -gt 20) {
            $message += [Environment]::NewLine + "... and $($sourceState.StatusLines.Count - 20) more"
        }
        if (-not $AllowDirty) {
            throw $message + [Environment]::NewLine +
                "Commit the release source or pass -AllowDirtySource for an explicit development package."
        }
        Write-Warning $message
    }
    return $sourceState
}

function Remove-VerifiedTemporaryDirectory {
    param(
        [string]$Path,
        [string]$AllowedRoot
    )

    if (-not (Test-Path -LiteralPath $Path)) {
        return
    }

    $rootFullPath = [System.IO.Path]::GetFullPath($AllowedRoot).TrimEnd(
        [System.IO.Path]::DirectorySeparatorChar,
        [System.IO.Path]::AltDirectorySeparatorChar)
    $targetFullPath = [System.IO.Path]::GetFullPath($Path).TrimEnd(
        [System.IO.Path]::DirectorySeparatorChar,
        [System.IO.Path]::AltDirectorySeparatorChar)
    $rootPrefix = $rootFullPath + [System.IO.Path]::DirectorySeparatorChar
    if ($targetFullPath -eq $rootFullPath -or
        -not $targetFullPath.StartsWith($rootPrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to remove temporary directory outside the package output root: $targetFullPath"
    }

    Remove-Item -LiteralPath $targetFullPath -Recurse -Force
}

function Promote-PackageEvidence {
    param([pscustomobject]$Evidence)

    $evidenceRoot = [System.IO.Path]::GetFullPath($Evidence.EvidenceRoot).TrimEnd(
        [System.IO.Path]::DirectorySeparatorChar,
        [System.IO.Path]::AltDirectorySeparatorChar)
    $candidateRoot = [System.IO.Path]::GetFullPath($Evidence.CandidateEvidenceRoot).TrimEnd(
        [System.IO.Path]::DirectorySeparatorChar,
        [System.IO.Path]::AltDirectorySeparatorChar)
    if (-not $candidateRoot.StartsWith(
        $evidenceRoot + [System.IO.Path]::DirectorySeparatorChar,
        [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to promote package evidence from outside its evidence root: $candidateRoot"
    }

    $backupRoot = Join-Path $evidenceRoot ".previous-$PID"
    if (Test-Path -LiteralPath $backupRoot) {
        throw "Package evidence backup already exists: $backupRoot"
    }
    New-Item -ItemType Directory -Path $backupRoot | Out-Null
    $fileNames = @($Evidence.PlayerLogFileName, $Evidence.ManifestFileName)
    try {
        foreach ($fileName in $fileNames) {
            $targetPath = Join-Path $evidenceRoot $fileName
            if (Test-Path -LiteralPath $targetPath) {
                Move-Item -LiteralPath $targetPath -Destination (Join-Path $backupRoot $fileName)
            }
        }
        foreach ($fileName in $fileNames) {
            $candidatePath = Join-Path $candidateRoot $fileName
            if (-not (Test-Path -LiteralPath $candidatePath)) {
                throw "Candidate package evidence is missing: $candidatePath"
            }
            Move-Item -LiteralPath $candidatePath -Destination (Join-Path $evidenceRoot $fileName)
        }
    } catch {
        New-Item -ItemType Directory -Path $candidateRoot -Force | Out-Null
        foreach ($fileName in $fileNames) {
            $targetPath = Join-Path $evidenceRoot $fileName
            $candidatePath = Join-Path $candidateRoot $fileName
            if ((Test-Path -LiteralPath $targetPath) -and
                -not (Test-Path -LiteralPath $candidatePath)) {
                Move-Item -LiteralPath $targetPath -Destination $candidatePath -Force
            }
        }
        foreach ($fileName in $fileNames) {
            $backupPath = Join-Path $backupRoot $fileName
            if (Test-Path -LiteralPath $backupPath) {
                Move-Item -LiteralPath $backupPath -Destination (Join-Path $evidenceRoot $fileName) -Force
            }
        }
        throw
    }

    foreach ($temporaryDirectory in @($candidateRoot, $backupRoot)) {
        try {
            Remove-VerifiedTemporaryDirectory `
                -Path $temporaryDirectory `
                -AllowedRoot $evidenceRoot
        } catch {
            Write-Warning "Promoted package evidence but could not remove temporary directory: $temporaryDirectory"
        }
    }
}

function Test-CleanWindowsPackage {
    param(
        [string]$PackagePath,
        [string]$OutputsRoot,
        [string]$ProjectRoot,
        [string]$ExecutableBaseName,
        [string]$Version,
        [int]$ExpectedSaveVersion,
        [string]$ExpectedSourceCommit,
        [int]$TimeoutSeconds,
        [string]$ExpectedPackageFileName,
        [bool]$AllowDirtySource
    )

    $evidenceDirectoryName = if ($AllowDirtySource) {
        "$Version-development-release-integrity"
    } else {
        "$Version-release-integrity"
    }
    $evidenceRoot = Join-Path $ProjectRoot "QA\$evidenceDirectoryName"
    New-Item -ItemType Directory -Path $evidenceRoot -Force | Out-Null
    $candidateEvidenceRoot = Join-Path $evidenceRoot ".candidate-$PID"
    if (Test-Path -LiteralPath $candidateEvidenceRoot) {
        throw "Release-integrity candidate evidence already exists: $candidateEvidenceRoot"
    }
    New-Item -ItemType Directory -Path $candidateEvidenceRoot | Out-Null
    $playerLogFileName = "clean-extract-boot.log"
    $manifestFileName = "release-integrity-manifest.json"
    $playerLogPath = Join-Path $candidateEvidenceRoot $playerLogFileName
    $manifestPath = Join-Path $candidateEvidenceRoot $manifestFileName

    $temporaryRoot = Join-Path $OutputsRoot (
        ".package-smoke-" + $ExecutableBaseName + "-" + $Version + "-" + $PID)
    if (Test-Path -LiteralPath $temporaryRoot) {
        throw "Clean-package smoke directory already exists: $temporaryRoot"
    }

    New-Item -ItemType Directory -Path $temporaryRoot | Out-Null
    $evidenceReady = $false
    try {
        Expand-Archive -LiteralPath $PackagePath -DestinationPath $temporaryRoot
        $packageRoot = Join-Path $temporaryRoot "$ExecutableBaseName-Windows-$Version"
        $exePath = Join-Path $packageRoot "$ExecutableBaseName.exe"
        $managedAssemblyPath = Join-Path $packageRoot "${ExecutableBaseName}_Data\Managed\Assembly-CSharp.dll"
        $requiredPackagePaths = @(
            $exePath,
            $managedAssemblyPath,
            (Join-Path $packageRoot "${ExecutableBaseName}_Data"),
            (Join-Path $packageRoot "README_PLAY.txt"),
            (Join-Path $packageRoot "CHANGELOG.md"),
            (Join-Path $packageRoot "KNOWN_ISSUES.txt"),
            (Join-Path $packageRoot "Docs\THIRD_PARTY_NOTICES.txt")
        )
        foreach ($requiredPath in $requiredPackagePaths) {
            if (-not (Test-Path -LiteralPath $requiredPath)) {
                throw "Clean package is missing required content: $requiredPath"
            }
        }

        $playerArguments =
            '-batchmode -quit -force-d3d11 -screen-width 1280 -screen-height 720 -logFile "' + $playerLogPath +
            '" -ashen-explore-smoke'
        $player = Start-Process `
            -FilePath $exePath `
            -ArgumentList $playerArguments `
            -PassThru `
            -WindowStyle Hidden
        $completed = $player.WaitForExit([Math]::Max(1, $TimeoutSeconds) * 1000)
        if (-not $completed) {
            Stop-Process -Id $player.Id -Force -ErrorAction SilentlyContinue
            throw "Clean extracted player did not finish within $TimeoutSeconds seconds."
        }
        if ($player.ExitCode -ne 0) {
            throw "Clean extracted player exited with code $($player.ExitCode). See $playerLogPath"
        }
        if (-not (Test-Path -LiteralPath $playerLogPath)) {
            throw "Clean extracted player did not create a log: $playerLogPath"
        }

        $playerLog = Get-Content -LiteralPath $playerLogPath -Raw
        $requiredLogPatterns = @(
            [regex]::Escape("Ash & Brimstone boot start $Version / save $ExpectedSaveVersion."),
            [regex]::Escape("Ash & Brimstone visual smoke mode: exploration."),
            [regex]::Escape("Ash & Brimstone boot complete: Muster ready."),
            [regex]::Escape("Ash & Brimstone batchmode quit requested after boot.")
        )
        foreach ($requiredPattern in $requiredLogPatterns) {
            if ($playerLog -notmatch $requiredPattern) {
                throw "Clean extracted player log is missing required evidence '$requiredPattern'. See $playerLogPath"
            }
        }
        if ($playerLog -match "Attempting to select .* while already selecting" -or
            $playerLog -match "(NullReferenceException|MissingReferenceException|Unhandled Exception)") {
            throw "Clean extracted player log contains a release-blocking runtime error. See $playerLogPath"
        }

        $sourceState = Assert-ReleaseSourceState `
            -ProjectRoot $ProjectRoot `
            -AllowDirty $AllowDirtySource
        if ($sourceState.Commit -ne $ExpectedSourceCommit) {
            throw "Release source commit changed during packaging: $ExpectedSourceCommit -> $($sourceState.Commit)"
        }

        $packagedArtHashes = @(
            Get-PackagedArtFiles -PackageRoot $packageRoot |
                Sort-Object Name |
                ForEach-Object {
                    [ordered]@{
                        file = $_.Name
                        bytes = $_.Length
                        sha256 = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
                    }
                }
        )

        $packageHash = (Get-FileHash -LiteralPath $PackagePath -Algorithm SHA256).Hash.ToLowerInvariant()
        $exeHash = (Get-FileHash -LiteralPath $exePath -Algorithm SHA256).Hash.ToLowerInvariant()
        $managedAssemblyHash = (Get-FileHash -LiteralPath $managedAssemblyPath -Algorithm SHA256).Hash.ToLowerInvariant()
        $sourceStatus = ($sourceState.StatusLines -join "`n")
        $manifest = [ordered]@{
            schemaVersion = 2
            releaseVersion = $Version
            saveVersion = $ExpectedSaveVersion
            developmentPackage = [bool]$AllowDirtySource
            verifiedAtUtc = [DateTime]::UtcNow.ToString("o")
            sourceCommit = $sourceState.Commit
            sourceStatusScope = "tracked-repository-plus-untracked-release-inputs"
            sourceDirty = $sourceState.StatusLines.Count -gt 0
            sourceStatusEntryCount = $sourceState.StatusLines.Count
            sourceStatusSha256 = Get-StringSha256 -Text $sourceStatus
            packageFile = $ExpectedPackageFileName
            packageBytes = (Get-Item -LiteralPath $PackagePath).Length
            packageSha256 = $packageHash
            executableSha256 = $exeHash
            managedAssemblySha256 = $managedAssemblyHash
            cleanExtractLaunch = $true
            packagedBootMode = "batchmode-explore-smoke"
            playerExitCode = $player.ExitCode
            packagedArtCount = $packagedArtHashes.Count
            packagedArt = $packagedArtHashes
        }
        $manifestJson = $manifest | ConvertTo-Json -Depth 6
        [System.IO.File]::WriteAllText(
            $manifestPath,
            $manifestJson + [Environment]::NewLine,
            [System.Text.UTF8Encoding]::new($false))

        $evidenceReady = $true

        Write-Host "Clean extracted package smoke passed: $Version"
        Write-Host "Candidate release integrity manifest: $manifestPath"
        Write-Host "Package SHA-256: $packageHash"
        return [pscustomobject]@{
            EvidenceRoot = $evidenceRoot
            CandidateEvidenceRoot = $candidateEvidenceRoot
            PlayerLogFileName = $playerLogFileName
            ManifestFileName = $manifestFileName
        }
    } finally {
        Remove-VerifiedTemporaryDirectory -Path $temporaryRoot -AllowedRoot $OutputsRoot
        if (-not $evidenceReady -and (Test-Path -LiteralPath $candidateEvidenceRoot)) {
            Write-Warning "Failed release-integrity evidence was retained for diagnosis: $candidateEvidenceRoot"
        }
    }
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
if ($VersionInfoText -notmatch 'SaveVersion\s*=\s*(\d+)') {
    throw "Could not read SaveVersion from $VersionInfoPath"
}
$SourceSaveVersion = [int]$Matches[1]
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

if (($SkipUnityBuild -or $SkipCleanPackageSmoke) -and -not $AllowDirtySource) {
    throw "-SkipUnityBuild and -SkipCleanPackageSmoke are development-only; pair either with -AllowDirtySource."
}

$initialSourceState = Assert-ReleaseSourceState `
    -ProjectRoot $ProjectRoot `
    -AllowDirty $AllowDirtySource

$OutputsRoot = Resolve-Path (Join-Path $ProjectRoot "..")
$BuildFolder = Join-Path $OutputsRoot "ash-and-brimstone-build\$ExecutableBaseName-Windows-$Version"
$ZipFileName = if ($AllowDirtySource) {
    "$ExecutableBaseName-Windows-$Version-dev.zip"
} else {
    "$ExecutableBaseName-Windows-$Version.zip"
}
$ZipPath = Join-Path $OutputsRoot $ZipFileName
$BuildLog = Join-Path $ProjectRoot "build-$Version.log"

if (-not $SkipUnityBuild) {
    if (-not (Test-Path -LiteralPath $UnityExe)) {
        throw "Unity executable not found: $UnityExe"
    }

    if (Test-Path -LiteralPath $BuildLog) {
        Remove-Item -LiteralPath $BuildLog -Force
    }
    # Start-Process joins argument arrays without preserving quotes around paths.
    # Quote canonical workspace paths explicitly because "Ashen Halls" contains a space.
    $UnityArguments =
        '-batchmode -quit -projectPath "' + $ProjectRoot +
        '" -executeMethod AshenHalls.Editor.BuildWindows.Build -logFile "' + $BuildLog + '"'
    $BuildProcess = Start-Process `
        -FilePath $UnityExe `
        -ArgumentList $UnityArguments `
        -PassThru `
        -WindowStyle Hidden
    $BuildProcess.WaitForExit()
    if ($BuildProcess.ExitCode -ne 0) {
        throw "Unity build process exited with code $($BuildProcess.ExitCode). See $BuildLog"
    }
    $BuildText = Read-UnityBuildLogWithRetry -Path $BuildLog -TimeoutSeconds $BuildLogWaitSeconds
}

$ExePath = Join-Path $BuildFolder "$ExecutableBaseName.exe"
if (-not (Test-Path -LiteralPath $ExePath)) {
    throw "Build folder is missing $ExecutableBaseName.exe: $BuildFolder"
}

$postBuildSourceState = Assert-ReleaseSourceState `
    -ProjectRoot $ProjectRoot `
    -AllowDirty $AllowDirtySource
if ($postBuildSourceState.Commit -ne $initialSourceState.Commit) {
    throw "Release source commit changed during the Unity build: $($initialSourceState.Commit) -> $($postBuildSourceState.Commit)"
}

$packageBuildNotePath = Join-Path $BuildFolder "PACKAGE_BUILD_NOTE.txt"
$packageBuildNote =
    "Ash & Brimstone Windows build staging folder.`n" +
    "Zip this folder after Unity exits to create the distributable package.`n" +
    "Expected zip: $ZipFileName`n"
[System.IO.File]::WriteAllText(
    $packageBuildNotePath,
    $packageBuildNote,
    [System.Text.UTF8Encoding]::new($false))

$packagedArtSourceState = Get-PackagedArtSourceState `
    -ProjectRoot $ProjectRoot `
    -PackageRoot $BuildFolder
if ($packagedArtSourceState.Missing.Count -gt 0) {
    throw "Packaged art has no matching source file: $($packagedArtSourceState.Missing -join ', ')"
}
if ($packagedArtSourceState.HashMismatch.Count -gt 0) {
    throw "Packaged art differs from its source file: $($packagedArtSourceState.HashMismatch -join ', ')"
}
if ($packagedArtSourceState.Untracked.Count -gt 0) {
    $message = "Packaged art is not tracked by Git: $($packagedArtSourceState.Untracked -join ', ')"
    if (-not $AllowDirtySource) {
        throw $message
    }
    Write-Warning $message
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

$packageEvidence = $null
$zipBackupPath = "$ZipPath.previous-$PID"
if (Test-Path -LiteralPath $zipBackupPath) {
    throw "Package backup already exists: $zipBackupPath"
}
$previousZipMoved = $false
$newZipPromoted = $false
try {
    if (-not $SkipCleanPackageSmoke) {
        $packageEvidence = Test-CleanWindowsPackage `
            -PackagePath $TempZipPath `
            -OutputsRoot $OutputsRoot `
            -ProjectRoot $ProjectRoot `
            -ExecutableBaseName $ExecutableBaseName `
            -Version $Version `
            -ExpectedSaveVersion $SourceSaveVersion `
            -ExpectedSourceCommit $initialSourceState.Commit `
            -TimeoutSeconds $PackageSmokeTimeoutSeconds `
            -ExpectedPackageFileName ([System.IO.Path]::GetFileName($ZipPath)) `
            -AllowDirtySource $AllowDirtySource
    }

    if (Test-Path -LiteralPath $ZipPath) {
        Move-Item -LiteralPath $ZipPath -Destination $zipBackupPath
        $previousZipMoved = $true
    }
    Move-Item -LiteralPath $TempZipPath -Destination $ZipPath
    $newZipPromoted = $true
    if ($null -ne $packageEvidence) {
        Promote-PackageEvidence -Evidence $packageEvidence
    }
} catch {
    $promotionError = $_
    try {
        if ($newZipPromoted -and (Test-Path -LiteralPath $ZipPath)) {
            Move-Item -LiteralPath $ZipPath -Destination $TempZipPath -Force
        }
        if ($previousZipMoved -and (Test-Path -LiteralPath $zipBackupPath)) {
            Move-Item -LiteralPath $zipBackupPath -Destination $ZipPath -Force
        }
    } catch {
        throw "Package promotion failed and rollback also failed. Candidate: $TempZipPath. Previous: $zipBackupPath. $($_.Exception.Message)"
    }
    if (Test-Path -LiteralPath $TempZipPath) {
        Write-Warning "Candidate package retained for diagnosis: $TempZipPath"
    }
    throw $promotionError
}

if ($previousZipMoved -and (Test-Path -LiteralPath $zipBackupPath)) {
    try {
        Remove-Item -LiteralPath $zipBackupPath -Force
    } catch {
        Write-Warning "Promoted the new package but could not remove its previous-package backup: $zipBackupPath"
    }
}

$Zip = Get-Item -LiteralPath $ZipPath
Write-Host "Ash & Brimstone package complete: $ZipPath"
Write-Host "Size: $($Zip.Length) bytes"
