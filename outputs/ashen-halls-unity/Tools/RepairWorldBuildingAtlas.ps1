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
$source = Join-Path $projectRoot 'Docs\ArtReferences\midgaard-town-atlas-runtime-v2.21.0.png'
$sourceHash = '593C1DF8EA8142123EA3D2582A87D7900F89F0BFF1376778A7B58CEABAFD7501'
if ((Get-FileHash -LiteralPath $source -Algorithm SHA256).Hash -ne $sourceHash) { throw 'Town source changed: re-review the contour masks before use.' }
$recipe = Join-Path $PSScriptRoot 'RepairBuildingAtlasMatte.lua'
$output = Join-Path $OutputDirectory 'midgaard-town-atlas-runtime-v2.28.0.png'
$editable = Join-Path $OutputDirectory 'source-midgaard-town-atlas-v2.28.0.aseprite'
$log = Join-Path $OutputDirectory 'midgaard-town-atlas-runtime-v2.28.0-aseprite.log'
$errors = Join-Path $OutputDirectory 'midgaard-town-atlas-runtime-v2.28.0-errors.log'
$arguments = '--batch "' + $source + '" --script-param editable="' + $editable + '" --script-param output="' + $output + '" --script "' + $recipe + '"'
$process = Start-Process -FilePath $AsepriteExe -ArgumentList $arguments -WorkingDirectory (Split-Path -Parent $AsepriteExe) -RedirectStandardOutput $log -RedirectStandardError $errors -WindowStyle Hidden -PassThru
if (-not $process.WaitForExit(60000)) { $process.Kill(); $process.WaitForExit(); throw 'Aseprite building cleanup timed out.' }
if ($process.ExitCode -ne 0) { throw ('Aseprite building cleanup failed; inspect ' + $errors) }
$result = Get-Content -LiteralPath $log -Raw | ConvertFrom-Json
$report = [ordered]@{
    tool='Aseprite'; toolVersion=(Get-Item -LiteralPath $AsepriteExe).VersionInfo.FileVersion
    source=[IO.Path]::GetFileName($source); sourceSha256=$sourceHash
    output=[IO.Path]::GetFileName($output); outputSha256=(Get-FileHash -LiteralPath $output -Algorithm SHA256).Hash
    editable=[IO.Path]::GetFileName($editable); editableSha256=(Get-FileHash -LiteralPath $editable -Algorithm SHA256).Hash
    recipeSha256=(Get-FileHash -LiteralPath $recipe -Algorithm SHA256).Hash
    geometry=@($result.width,$result.height); cells=$result.cells
    note='Native Aseprite bounded contour repair and reviewed enclosed sign-gap cleanup on nine architecture cells. Eleven other cells and original smoke regions unchanged. Original source layer hidden, repaired layer visible. No resize, repaint or remap.'
}
$report | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath (Join-Path $OutputDirectory 'midgaard-town-atlas-runtime-v2.28.0-validation.json') -Encoding utf8
Write-Output ('ASEPRITE BUILDING REPAIR: ' + $output)
