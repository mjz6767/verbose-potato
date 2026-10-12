# Resolve the editor declared by the project, without starting it or upgrading it.
function Get-UnityProjectRequirement {
    param([Parameter(Mandatory = $true)][string]$VersionText)
    $versionKeys = [regex]::Matches($VersionText, '(?m)^[\t ]*m_EditorVersion[\t ]*:')
    $versions = [regex]::Matches($VersionText, '(?m)^m_EditorVersion:[\t ]*(\d+\.\d+\.\d+[abfp]\d+)[\t ]*\r?$')
    if ($versionKeys.Count -ne 1 -or $versions.Count -ne 1) { throw 'ProjectVersion.txt must declare exactly one valid m_EditorVersion.' }
    $version = $versions[0].Groups[1].Value
    $revisionKeys = [regex]::Matches($VersionText, '(?m)^[\t ]*m_EditorVersionWithRevision[\t ]*:')
    $revisions = [regex]::Matches($VersionText, '(?m)^m_EditorVersionWithRevision:[\t ]*(\d+\.\d+\.\d+[abfp]\d+)[\t ]+\(([a-fA-F0-9]+)\)[\t ]*\r?$')
    if ($revisionKeys.Count -ne 1 -or $revisions.Count -ne 1 -or $revisions[0].Groups[1].Value -ne $version) {
        throw 'ProjectVersion.txt must declare a matching editor revision.'
    }
    return [pscustomobject]@{ Version = $version; Revision = $revisions[0].Groups[2].Value }
}

# Cached compiler arguments must belong to the selected editor too. Changing only
# the compiler executable could otherwise validate against an older Unity API.
function Assert-UnityResponseFileEditor {
    param(
        [Parameter(Mandatory = $true)]$Requirement,
        [Parameter(Mandatory = $true)][string]$UnityEditorRoot,
        [Parameter(Mandatory = $true)][string[]]$Lines,
        [string]$ResponseName = 'Unity compiler response file'
    )
    $regenerate = 'Run the declared Unity editor to regenerate compiler response files before source-only compilation.'
    $numericVersion = [regex]::Match($Requirement.Version, '^\d+\.\d+\.\d+').Value
    $expectedDefine = 'UNITY_' + $numericVersion.Replace('.', '_')
    $patchDefines = @(
        foreach ($line in $Lines) {
            if ($line -match '^\s*[-/]define:(.+)$') {
                foreach ($symbol in ($Matches[1].Trim('"') -split '[;,]')) {
                    if ($symbol.Trim() -match '^UNITY_\d+_\d+_\d+$') { $symbol.Trim() }
                }
            }
        }
    )
    if ($patchDefines.Count -ne 1 -or $patchDefines[0] -cne $expectedDefine) {
        throw "Stale Unity response file ${ResponseName}: expected exactly one $expectedDefine patch define. $regenerate"
    }

    $editorPrefix = [IO.Path]::GetFullPath($UnityEditorRoot).Replace('\', '/').TrimEnd('/') + '/'
    $coreReferences = @{}
    foreach ($line in $Lines) {
        if ($line -notmatch '^\s*[-/](r|reference|analyzer):(?:"([^"\r\n]+)"|([^"\r\n]+))\s*$') { continue }
        $kind = $Matches[1]
        $referencePath = if ($Matches[2]) { $Matches[2] } else { $Matches[3].Trim() }
        $normalized = [IO.Path]::GetFullPath($referencePath).Replace('\', '/')
        $fileName = [IO.Path]::GetFileName($normalized)
        $isCore = $fileName -in @('UnityEngine.CoreModule.dll', 'UnityEditor.CoreModule.dll')
        $editorOwned = $isCore -or $normalized -match '/Editor/Data/' -or
            $normalized -match '/Data/(Managed|NetStandard|UnityReferenceAssemblies|Tools/BuildPipeline)/'
        if ($editorOwned -and -not $normalized.StartsWith($editorPrefix, [StringComparison]::OrdinalIgnoreCase)) {
            throw "Stale Unity response file ${ResponseName}: editor-owned $kind path is outside the selected editor: $referencePath. $regenerate"
        }
        if ($isCore -and $kind -in @('r', 'reference')) { $coreReferences[$fileName] = $true }
    }
    foreach ($core in @('UnityEngine.CoreModule.dll', 'UnityEditor.CoreModule.dll')) {
        if (-not $coreReferences.ContainsKey($core)) {
            throw "Stale Unity response file ${ResponseName}: missing $core reference from the selected editor. $regenerate"
        }
    }
}

function Assert-UnityEditorProductVersion {
    param(
        [Parameter(Mandatory = $true)]$Requirement,
        [Parameter(Mandatory = $true)][string]$ProductVersion
    )
    $expected = $Requirement.Version + '_' + $Requirement.Revision
    if ($ProductVersion -ne $expected) {
        throw "Unity editor mismatch: project requires $expected but selected executable reports $ProductVersion. Install the declared editor or deliberately update the project pin before building."
    }
}

function Resolve-ProjectUnityEditor {
    param(
        [Parameter(Mandatory = $true)][string]$ProjectRoot,
        [string]$UnityExe = '',
        [string]$EditorInstallRoot = (Join-Path $env:ProgramFiles 'Unity/Hub/Editor')
    )
    $versionFile = Join-Path $ProjectRoot 'ProjectSettings/ProjectVersion.txt'
    $requirement = Get-UnityProjectRequirement -VersionText (Get-Content -LiteralPath $versionFile -Raw -ErrorAction Stop)
    if ([string]::IsNullOrWhiteSpace($UnityExe)) {
        $UnityExe = Join-Path $EditorInstallRoot ($requirement.Version + '/Editor/Unity.exe')
    }
    if (-not (Test-Path -LiteralPath $UnityExe -PathType Leaf)) {
        throw "Unity $($requirement.Version) is required by ProjectVersion.txt but was not found at: $UnityExe. No fallback editor will be launched."
    }
    $editor = Get-Item -LiteralPath $UnityExe -ErrorAction Stop
    Assert-UnityEditorProductVersion -Requirement $requirement -ProductVersion $editor.VersionInfo.ProductVersion
    return $editor.FullName
}
