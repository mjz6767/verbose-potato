param(
    [Parameter(Mandatory = $true)]
    [string]$Source,

    [Parameter(Mandatory = $true)]
    [string]$Output,

    [string]$AsepriteExe = ""
)

$ErrorActionPreference = "Stop"

$ProjectRoot = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($AsepriteExe)) {
    $Candidates = @(
        "C:\Program Files (x86)\Steam\steamapps\common\Aseprite\Aseprite.exe",
        "C:\Program Files\Steam\steamapps\common\Aseprite\Aseprite.exe",
        (Join-Path $env:ProgramFiles "Aseprite\Aseprite.exe"),
        (Join-Path ${env:ProgramFiles(x86)} "Aseprite\Aseprite.exe")
    )

    $AsepriteExe = $Candidates | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
}

if ([string]::IsNullOrWhiteSpace($AsepriteExe) -or -not (Test-Path -LiteralPath $AsepriteExe)) {
    throw "Aseprite.exe was not found. Install it through Steam or pass -AsepriteExe."
}

$SourcePath = (Resolve-Path -LiteralPath $Source).Path
$OutputParent = Split-Path -Parent $Output
if (-not [string]::IsNullOrWhiteSpace($OutputParent) -and -not (Test-Path -LiteralPath $OutputParent)) {
    New-Item -ItemType Directory -Path $OutputParent | Out-Null
}
$OutputPath = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($Output)

$AsepriteRoot = Split-Path -Parent $AsepriteExe
$Command = "cd /d ""$AsepriteRoot"" && ""$AsepriteExe"" -b ""$SourcePath"" --save-as ""$OutputPath"""
cmd.exe /c $Command
if ($LASTEXITCODE -ne 0) {
    throw "Aseprite export failed with exit code $LASTEXITCODE"
}

if (-not (Test-Path -LiteralPath $OutputPath)) {
    throw "Aseprite did not create output: $OutputPath"
}

Get-Item -LiteralPath $OutputPath
