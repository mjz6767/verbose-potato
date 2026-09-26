param(
    [string]$AsepriteExe = 'C:\Program Files (x86)\Steam\steamapps\common\Aseprite\Aseprite.exe',
    [string]$OutputDirectory = ''
)

$ErrorActionPreference = 'Stop'
$projectRoot = [IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))
if (-not (Test-Path -LiteralPath $AsepriteExe -PathType Leaf)) { throw 'Installed Aseprite was not found.' }
if (-not $OutputDirectory) { $OutputDirectory = Join-Path $projectRoot 'Docs\ArtReferences' }
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
$scriptPath = Join-Path $PSScriptRoot 'RepairNpcAtlasMatte.lua'
$recipes = @(
    @{ Kind='named'; Family='midgaard-npc'; Sha256='7E33AECC377E690A03A62AC070481A6853BDDFF61C2C56E92E4060E1763F6C27' },
    @{ Kind='citizen'; Family='world-npc-citizen'; Sha256='6F45940F1F590D2F4CE04450AD19A0C081611A06897B9B3952A51F2551AFB198' }
)
foreach ($recipe in $recipes) {
    $source = Join-Path $projectRoot ('Docs\ArtReferences\' + $recipe.Family + '-atlas-runtime-v2.21.0.png')
    if ((Get-FileHash -LiteralPath $source -Algorithm SHA256).Hash -ne $recipe.Sha256) {
        throw ('Source pixels changed; re-review the authored cleanup seeds: ' + $source)
    }
    $output = Join-Path $OutputDirectory ($recipe.Family + '-atlas-runtime-v2.26.1.png')
    $editable = Join-Path $OutputDirectory ('source-' + $recipe.Family + '-atlas-v2.26.1.aseprite')
    $log = Join-Path $OutputDirectory ($recipe.Family + '-atlas-runtime-v2.26.1-aseprite.log')
    $errors = Join-Path $OutputDirectory ($recipe.Family + '-atlas-runtime-v2.26.1-errors.log')
    $arguments = '--batch "' + $source + '" --script-param kind=' + $recipe.Kind +
        ' --script-param editable="' + $editable + '" --script-param output="' + $output +
        '" --script "' + $scriptPath + '"'
    $process = Start-Process -FilePath $AsepriteExe -ArgumentList $arguments -WorkingDirectory (Split-Path -Parent $AsepriteExe) `
        -RedirectStandardOutput $log -RedirectStandardError $errors -WindowStyle Hidden -PassThru
    if (-not $process.WaitForExit(60000)) {
        $process.Kill(); $process.WaitForExit()
        throw ('Aseprite cleanup timed out: ' + $recipe.Kind)
    }
    if ($process.ExitCode -ne 0) { throw ('Aseprite cleanup failed; inspect ' + $errors) }
    $result = Get-Content -LiteralPath $log -Raw | ConvertFrom-Json
    $report = [ordered]@{
        tool = 'Aseprite'; toolVersion = (Get-Item -LiteralPath $AsepriteExe).VersionInfo.FileVersion
        source = [IO.Path]::GetFileName($source); sourceSha256 = $recipe.Sha256
        output = [IO.Path]::GetFileName($output); outputSha256 = (Get-FileHash -LiteralPath $output -Algorithm SHA256).Hash
        editable = [IO.Path]::GetFileName($editable); editableSha256 = (Get-FileHash -LiteralPath $editable -Algorithm SHA256).Hash
        recipeSha256 = (Get-FileHash -LiteralPath $scriptPath -Algorithm SHA256).Hash
        geometry = @($result.width,$result.height); cells = $result.cells
        note = 'Native Aseprite cleanup. Original reference layer retained hidden; corrected layer visible. No generated replacement art, character resizing or remapping.'
    }
    $report | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath (Join-Path $OutputDirectory ($recipe.Family + '-atlas-runtime-v2.26.1-validation.json')) -Encoding utf8
    Write-Output ('ASEPRITE NPC REPAIR: ' + $output)
}
