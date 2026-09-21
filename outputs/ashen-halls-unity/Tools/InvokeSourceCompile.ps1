param(
    [string]$UnityEditorRoot = 'C:\Program Files\Unity\Hub\Editor\6000.3.18f1\Editor',
    [string]$ResponseDirectory = 'Library/Bee/artifacts/1900b0aE.dag'
)

# A source-only fallback, never a replacement for licensed Unity tests/builds.
# Reuse Unity's reference/define/analyzer configuration, refresh the source list,
# and put every output outside Library so a check cannot alter build caches.
$ErrorActionPreference = 'Stop'
$projectRoot = [IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))
$runName = [DateTime]::UtcNow.ToString('yyyyMMdd-HHmmss') + '-' + [Guid]::NewGuid().ToString('N').Substring(0, 8)
$outputRoot = Join-Path $projectRoot ('QA\source-compile\' + $runName)
$dotnet = Join-Path $UnityEditorRoot 'Data\NetCoreRuntime\dotnet.exe'
$compiler = Join-Path $UnityEditorRoot 'Data\DotNetSdkRoslyn\csc.dll'
foreach ($required in @($dotnet, $compiler)) {
    if (-not (Test-Path -LiteralPath $required -PathType Leaf)) { throw "Compiler dependency missing: $required" }
}
New-Item -ItemType Directory -Path $outputRoot | Out-Null
Push-Location $projectRoot
try {
    foreach ($assembly in @('Assembly-CSharp', 'Assembly-CSharp-Editor')) {
        $sourceRoot = if ($assembly -eq 'Assembly-CSharp') { 'Assets/Scripts' } else { 'Assets/Editor' }
        $template = Join-Path $ResponseDirectory ($assembly + '.rsp')
        if (-not (Test-Path -LiteralPath $template -PathType Leaf)) { throw "Unity response file missing: $template" }
        $lines = foreach ($line in Get-Content -LiteralPath $template) {
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
