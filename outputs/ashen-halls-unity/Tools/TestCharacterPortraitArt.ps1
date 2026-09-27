param([string]$ReportPath = '')

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$projectRoot = [IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))
$races = @('human', 'dusk-elf', 'stoneborn', 'fenkin', 'ashling')
$classes = @('rogue', 'warrior', 'ranger', 'wizard', 'mage', 'warlock', 'priest', 'paladin')
$portraits = [Collections.Generic.List[object]]::new()
$atlases = [Collections.Generic.List[object]]::new()
$seen = [Collections.Generic.HashSet[string]]::new()
foreach ($race in $races) {
    $name = 'character-portrait-' + $race + '-atlas-runtime-v2.28.0.png'
    $path = Join-Path $projectRoot ('Docs/ArtReferences/' + $name)
    $bitmap = [Drawing.Bitmap]::new($path)
    try {
        if ($bitmap.Width -ne 2 * $bitmap.Height -or $bitmap.Width -lt 1024) {
            throw "Portrait atlas must have a 2:1 canvas, at least 1024 pixels wide: $name"
        }
        $atlases.Add([ordered]@{ file=$name; width=$bitmap.Width; height=$bitmap.Height; sha256=(Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant() })
        for ($cell = 0; $cell -lt 8; $cell++) {
            $column = $cell % 4
            $row = [Math]::Floor($cell / 4)
            # Sample within each UV cell, avoiding shared half-pixel boundaries.
            $samples = [Collections.Generic.List[byte]]::new()
            $minimum = 255
            $maximum = 0
            for ($y = 0; $y -lt 48; $y++) {
                for ($x = 0; $x -lt 48; $x++) {
                    $px = [int][Math]::Floor(($column + ($x + 0.5) / 48) * $bitmap.Width / 4)
                    $py = [int][Math]::Floor(($row + ($y + 0.5) / 48) * $bitmap.Height / 2)
                    $color = $bitmap.GetPixel($px, $py)
                    if ($color.A -ne 255) { throw "Opaque portrait contains a transparent sample: $race / $($classes[$cell])" }
                    $samples.Add($color.R); $samples.Add($color.G); $samples.Add($color.B)
                    $luma = [int](0.2126 * $color.R + 0.7152 * $color.G + 0.0722 * $color.B)
                    $minimum = [Math]::Min($minimum, $luma)
                    $maximum = [Math]::Max($maximum, $luma)
                }
            }
            if ($maximum - $minimum -lt 64) { throw "Portrait lacks sufficient painted content: $race / $($classes[$cell])" }
            $algorithm = [Security.Cryptography.SHA256]::Create()
            try { $hash = [BitConverter]::ToString($algorithm.ComputeHash($samples.ToArray())).Replace('-', '').ToLowerInvariant() }
            finally { $algorithm.Dispose() }
            if (-not $seen.Add($hash)) { throw "Duplicate portrait samples: $race / $($classes[$cell])" }
            $portraits.Add([ordered]@{race=$race; class=$classes[$cell]; cell=$cell; sampleSha256=$hash; luminanceRange=($maximum-$minimum)})
        }
    }
    finally { $bitmap.Dispose() }
}
if ($portraits.Count -ne 40) { throw 'Expected exactly 40 race/class portraits.' }
$report = [ordered]@{
    schemaVersion=1
    release='v2.28.0'
    generator='Built-in ImageGen'
    sourcePrompts='Docs/CHARACTER_CREATION_ART_PROMPTS_v2.28.md'
    artModifiedAfterGeneration=$false
    layout='Four columns, two rows; fractional UV cells preserve original generated pixels.'
    portraitCount=$portraits.Count
    uniqueSampleCount=$seen.Count
    atlases=$atlases.ToArray()
    portraits=$portraits.ToArray()
}
if ($ReportPath) {
    $report | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $ReportPath -Encoding utf8
}
Write-Output ('Character portrait art passed: {0} distinct opaque portraits across {1} atlases.' -f $portraits.Count, $atlases.Count)
