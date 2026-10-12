param(
    [string]$UnityEditorRoot = '',
    [string]$ResponseDirectory = 'Library/Bee/artifacts/1900b0aE.dag'
)

# A source-only fallback, never a replacement for licensed Unity tests/builds.
# Reuse Unity's reference/define/analyzer configuration, refresh the source list,
# and put every output outside Library so a check cannot alter build caches.
$ErrorActionPreference = 'Stop'
$projectRoot = [IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))
. (Join-Path $PSScriptRoot 'UnityEditorResolution.ps1')
$editorOverride = if ([string]::IsNullOrWhiteSpace($UnityEditorRoot)) { '' } else { Join-Path $UnityEditorRoot 'Unity.exe' }
$UnityEditorRoot = Split-Path -Parent (Resolve-ProjectUnityEditor -ProjectRoot $projectRoot -UnityExe $editorOverride)
$requirement = Get-UnityProjectRequirement -VersionText (Get-Content -LiteralPath (Join-Path $projectRoot 'ProjectSettings/ProjectVersion.txt') -Raw)
$runName = [DateTime]::UtcNow.ToString('yyyyMMdd-HHmmss') + '-' + [Guid]::NewGuid().ToString('N').Substring(0, 8)
$outputRoot = Join-Path $projectRoot ('QA\source-compile\' + $runName)
$dotnet = Join-Path $UnityEditorRoot 'Data\NetCoreRuntime\dotnet.exe'
$compiler = Join-Path $UnityEditorRoot 'Data\DotNetSdkRoslyn\csc.dll'
foreach ($required in @($dotnet, $compiler)) {
    if (-not (Test-Path -LiteralPath $required -PathType Leaf)) { throw "Compiler dependency missing: $required" }
}
Push-Location $projectRoot
try {
    # Validate both caches before compiling either assembly. Keep the checked
    # lines in memory so a later cache refresh cannot change the inputs.
    $responseTemplates = @{}
    foreach ($assembly in @('Assembly-CSharp', 'Assembly-CSharp-Editor')) {
        $template = Join-Path $ResponseDirectory ($assembly + '.rsp')
        if (-not (Test-Path -LiteralPath $template -PathType Leaf)) { throw "Unity response file missing: $template" }
        $responseTemplates[$assembly] = @(Get-Content -LiteralPath $template)
        Assert-UnityResponseFileEditor -Requirement $requirement -UnityEditorRoot $UnityEditorRoot `
            -Lines $responseTemplates[$assembly] -ResponseName $template
    }
    New-Item -ItemType Directory -Path $outputRoot | Out-Null
    foreach ($assembly in @('Assembly-CSharp', 'Assembly-CSharp-Editor')) {
        $sourceRoot = if ($assembly -eq 'Assembly-CSharp') { 'Assets/Scripts' } else { 'Assets/Editor' }
        $lines = foreach ($line in $responseTemplates[$assembly]) {
            if ($line -match '^"?Assets[/\\].*\.cs"?$') { continue }
            if ($line -match '^-out:') { '-out:"' + (Join-Path $outputRoot ($assembly + '.dll')) + '"'; continue }
            if ($line -match '^-refout:') { '-refout:"' + (Join-Path $outputRoot ($assembly + '.ref.dll')) + '"'; continue }
            if ($line -match '^-r:.*[/\\]Assembly-CSharp\.ref\.dll"?$') {
                '-r:"' + (Join-Path $outputRoot 'Assembly-CSharp.ref.dll') + '"'
                continue
            }
            $line
        }
        $sources = @(rg --files $sourceRoot -g '*.cs' | Sort-Object)
        if ($LASTEXITCODE -ne 0 -or $sources.Count -eq 0) { throw "No sources found in $sourceRoot" }
        $lines += @($sources | ForEach-Object { '"' + $_.Replace('\', '/') + '"' })
        $responsePath = Join-Path $outputRoot ($assembly + '.rsp')
        [IO.File]::WriteAllLines($responsePath, $lines, [Text.UTF8Encoding]::new($false))
        $diagnostics = @(& $dotnet $compiler ('@' + $responsePath) 2>&1)
        $compileExitCode = $LASTEXITCODE
        [IO.File]::WriteAllLines((Join-Path $outputRoot ($assembly + '.log')), [string[]]$diagnostics, [Text.UTF8Encoding]::new($false))
        $diagnostics | ForEach-Object { Write-Output $_ }
        if ($compileExitCode -ne 0) { throw "$assembly compile failed. See $outputRoot" }
        Write-Host "Source compile passed: $assembly ($($sources.Count) source files; $($diagnostics.Count) diagnostic lines)"
    }
    Write-Host "SOURCE-ONLY COMPILE COMPLETE: $outputRoot"
} finally {
    Pop-Location
}
