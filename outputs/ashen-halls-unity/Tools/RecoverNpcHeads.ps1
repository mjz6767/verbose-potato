param(
    [string]$AsepriteExe = 'C:\Program Files (x86)\Steam\steamapps\common\Aseprite\Aseprite.exe',
    [string]$OutputDirectory = ''
)
$ErrorActionPreference = 'Stop'
$projectRoot = [IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))
if (-not (Test-Path -LiteralPath $AsepriteExe -PathType Leaf)) { throw 'Installed Aseprite is missing.' }
if (-not $OutputDirectory) { $OutputDirectory = Join-Path $projectRoot 'Docs\ArtReferences' }
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
$scriptPath = Join-Path $PSScriptRoot 'RecoverNpcHeads.lua'
$recipes = @(
    @{Kind='named';Family='midgaard-npc';Source='source-midgaard-npcs-v2.21.0.png';SourceHash='D808E36E60722D01634B8256410FAC06BB617EEC22B8751446F8DE80F0EE562C';AtlasHash='4CA876765191A0A149B06FEC34F3952D0DB4B0C810BD90DBF38AEBF0DC3B3DA7'},
    @{Kind='citizen';Family='world-npc-citizen';Source='source-world-npc-citizens-v2.21.0.png';SourceHash='3B530AD083C6072F09927DF9EE2548BBF4E610EA3D0FE76C313F0BAF5AF38EC3';AtlasHash='95382FDF50404024CE0D7F1014E9EEAF7CDD543F701FA986412066A4F53FE638'}
)
foreach ($recipe in $recipes) {
    $source = Join-Path $projectRoot ('Docs\ArtReferences\' + $recipe.Source)
    $atlas = Join-Path $projectRoot ('Docs\ArtReferences\' + $recipe.Family + '-atlas-runtime-v2.26.1.png')
    if ((Get-FileHash -LiteralPath $source -Algorithm SHA256).Hash -ne $recipe.SourceHash -or
        (Get-FileHash -LiteralPath $atlas -Algorithm SHA256).Hash -ne $recipe.AtlasHash) {
        throw 'Inputs changed: re-review source rectangles and matte seeds before recovery.'
    }
    $output = Join-Path $OutputDirectory ($recipe.Family + '-atlas-runtime-v2.32.0.png')
    $editable = Join-Path $OutputDirectory ('source-' + $recipe.Family + '-atlas-v2.32.0.aseprite')
    $log = Join-Path $OutputDirectory ($recipe.Kind + '-head-recovery.log')
    $errors = Join-Path $OutputDirectory ($recipe.Kind + '-head-recovery-errors.log')
    $arguments = '--batch "' + $atlas + '" --script-param kind=' + $recipe.Kind +
        ' --script-param source="' + $source + '" --script-param editable="' + $editable +
        '" --script-param output="' + $output + '" --script "' + $scriptPath + '"'
    $process = Start-Process -FilePath $AsepriteExe -ArgumentList $arguments -WorkingDirectory (Split-Path -Parent $AsepriteExe) `
        -RedirectStandardOutput $log -RedirectStandardError $errors -WindowStyle Hidden -PassThru
    if (-not $process.WaitForExit(60000)) { $process.Kill(); $process.WaitForExit(); throw 'Aseprite recovery timed out.' }
    if ($process.ExitCode -ne 0 -or -not (Test-Path -LiteralPath $output)) { throw "Recovery failed; inspect $errors and $log" }
    $result = Get-Content -LiteralPath $log -Raw | ConvertFrom-Json
    [ordered]@{
        tool='Aseprite';toolVersion=(Get-Item -LiteralPath $AsepriteExe).VersionInfo.FileVersion
        source=$recipe.Source;sourceSha256=$recipe.SourceHash;referenceSha256=$recipe.AtlasHash
        output=[IO.Path]::GetFileName($output);outputSha256=(Get-FileHash -LiteralPath $output -Algorithm SHA256).Hash
        editable=[IO.Path]::GetFileName($editable);editableSha256=(Get-FileHash -LiteralPath $editable -Algorithm SHA256).Hash
        recipeSha256=(Get-FileHash -LiteralPath $scriptPath -Algorithm SHA256).Hash
        result=$result
    } | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath (Join-Path $OutputDirectory ($recipe.Family + '-atlas-runtime-v2.32.0-validation.json')) -Encoding utf8
    Write-Output "Recovered intact NPC silhouettes: $output"
}
