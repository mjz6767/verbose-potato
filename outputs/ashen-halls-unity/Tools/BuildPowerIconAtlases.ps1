param(
    [string]$Version = "v1.31.0",
    [string]$AsepriteExe = ""
)

$ErrorActionPreference = "Stop"

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$ArtRoot = Join-Path $ProjectRoot "Docs\ArtReferences"

if ([string]::IsNullOrWhiteSpace($AsepriteExe)) {
    $AsepriteExe = @(
        "C:\Program Files (x86)\Steam\steamapps\common\Aseprite\Aseprite.exe",
        "C:\Program Files\Steam\steamapps\common\Aseprite\Aseprite.exe",
        (Join-Path $env:ProgramFiles "Aseprite\Aseprite.exe"),
        (Join-Path ${env:ProgramFiles(x86)} "Aseprite\Aseprite.exe")
    ) | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
}

if ([string]::IsNullOrWhiteSpace($AsepriteExe) -or -not (Test-Path -LiteralPath $AsepriteExe)) {
    throw "Aseprite.exe was not found. Install it through Steam or pass -AsepriteExe."
}

Add-Type -AssemblyName System.Drawing

function Get-CellRectangle {
    param(
        [System.Drawing.Bitmap]$Bitmap,
        [int]$Columns,
        [int]$Rows,
        [int]$Index
    )

    $column = $Index % $Columns
    $row = [Math]::Floor($Index / $Columns)
    $left = [Math]::Floor($column * $Bitmap.Width / $Columns)
    $top = [Math]::Floor($row * $Bitmap.Height / $Rows)
    $right = [Math]::Floor(($column + 1) * $Bitmap.Width / $Columns)
    $bottom = [Math]::Floor(($row + 1) * $Bitmap.Height / $Rows)
    return [System.Drawing.Rectangle]::new($left, $top, $right - $left, $bottom - $top)
}

function Get-SignificantCellRectangle {
    param(
        [System.Drawing.Bitmap]$Bitmap,
        [System.Drawing.Rectangle]$CellRect,
        [int]$MinimumArea = 128
    )

    $width = $CellRect.Width
    $height = $CellRect.Height
    $visited = [bool[]]::new($width * $height)
    $globalMinX = $CellRect.Right
    $globalMinY = $CellRect.Bottom
    $globalMaxX = $CellRect.Left - 1
    $globalMaxY = $CellRect.Top - 1

    for ($localY = 0; $localY -lt $height; $localY++) {
        for ($localX = 0; $localX -lt $width; $localX++) {
            $startIndex = $localY * $width + $localX
            if ($visited[$startIndex]) { continue }
            $visited[$startIndex] = $true
            if ($Bitmap.GetPixel($CellRect.X + $localX, $CellRect.Y + $localY).A -lt 24) { continue }

            $queue = [System.Collections.Generic.Queue[int]]::new()
            $queue.Enqueue($startIndex)
            $area = 0
            $componentMinX = $localX
            $componentMaxX = $localX
            $componentMinY = $localY
            $componentMaxY = $localY
            while ($queue.Count -gt 0) {
                $current = $queue.Dequeue()
                $x = $current % $width
                $y = [Math]::Floor($current / $width)
                $area++
                if ($x -lt $componentMinX) { $componentMinX = $x }
                if ($x -gt $componentMaxX) { $componentMaxX = $x }
                if ($y -lt $componentMinY) { $componentMinY = $y }
                if ($y -gt $componentMaxY) { $componentMaxY = $y }

                for ($dy = -1; $dy -le 1; $dy++) {
                    for ($dx = -1; $dx -le 1; $dx++) {
                        if ($dx -eq 0 -and $dy -eq 0) { continue }
                        $nextX = $x + $dx
                        $nextY = $y + $dy
                        if ($nextX -lt 0 -or $nextY -lt 0 -or $nextX -ge $width -or $nextY -ge $height) { continue }
                        $nextIndex = $nextY * $width + $nextX
                        if ($visited[$nextIndex]) { continue }
                        $visited[$nextIndex] = $true
                        if ($Bitmap.GetPixel($CellRect.X + $nextX, $CellRect.Y + $nextY).A -ge 24) {
                            $queue.Enqueue($nextIndex)
                        }
                    }
                }
            }

            if ($area -lt $MinimumArea) { continue }
            $globalMinX = [Math]::Min($globalMinX, $CellRect.X + $componentMinX)
            $globalMinY = [Math]::Min($globalMinY, $CellRect.Y + $componentMinY)
            $globalMaxX = [Math]::Max($globalMaxX, $CellRect.X + $componentMaxX)
            $globalMaxY = [Math]::Max($globalMaxY, $CellRect.Y + $componentMaxY)
        }
    }

    if ($globalMaxX -lt $globalMinX -or $globalMaxY -lt $globalMinY) {
        throw "Atlas cell contains no significant visible component."
    }
    return [System.Drawing.Rectangle]::new($globalMinX, $globalMinY, $globalMaxX - $globalMinX + 1, $globalMaxY - $globalMinY + 1)
}

function Copy-AtlasCell {
    param(
        [System.Drawing.Graphics]$Graphics,
        [System.Drawing.Bitmap]$Source,
        [int]$SourceColumns,
        [int]$SourceRows,
        [int]$SourceIndex,
        [int]$TargetColumns,
        [int]$TargetIndex,
        [int]$CellSize
    )

    $cellRect = Get-CellRectangle -Bitmap $Source -Columns $SourceColumns -Rows $SourceRows -Index $SourceIndex
    # Generated grids can leak a few antialiased pixels across a nominal cut.
    # Inset before trimming so neighboring-cell debris cannot become its own icon fragment.
    $cellRect.Inflate(-8, -8)
    $sourceRect = Get-SignificantCellRectangle -Bitmap $Source -CellRect $cellRect
    $targetColumn = $TargetIndex % $TargetColumns
    $targetRow = [Math]::Floor($TargetIndex / $TargetColumns)
    $safeSize = $CellSize - 36
    $scale = [Math]::Min($safeSize / [double]$sourceRect.Width, $safeSize / [double]$sourceRect.Height)
    $targetWidth = [Math]::Max(1, [Math]::Round($sourceRect.Width * $scale))
    $targetHeight = [Math]::Max(1, [Math]::Round($sourceRect.Height * $scale))
    $targetX = $targetColumn * $CellSize + [Math]::Floor(($CellSize - $targetWidth) / 2)
    $targetY = $targetRow * $CellSize + [Math]::Floor(($CellSize - $targetHeight) / 2)
    $targetRect = [System.Drawing.Rectangle]::new($targetX, $targetY, $targetWidth, $targetHeight)
    $Graphics.DrawImage($Source, $targetRect, $sourceRect, [System.Drawing.GraphicsUnit]::Pixel)
}

function New-AbilityAtlas {
    param([string]$OutputPath)

    $generatedPath = Join-Path $ArtRoot "source-ability-icon-atlas-$Version-alpha.png"
    $approvedPath = Join-Path $ArtRoot "ability-icon-atlas-runtime-v1.24.0.png"
    $generated = [System.Drawing.Bitmap]::new($generatedPath)
    $approved = [System.Drawing.Bitmap]::new($approvedPath)
    try {
        $target = [System.Drawing.Bitmap]::new(1024, 1280, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
        try {
            $graphics = [System.Drawing.Graphics]::FromImage($target)
            try {
                $graphics.Clear([System.Drawing.Color]::Transparent)
                $graphics.CompositingMode = [System.Drawing.Drawing2D.CompositingMode]::SourceCopy
                $graphics.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighSpeed
                $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::NearestNeighbor
                $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::Half
                $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::None

                for ($targetIndex = 0; $targetIndex -lt 20; $targetIndex++) {
                    if ($targetIndex -lt 9) {
                        Copy-AtlasCell -Graphics $graphics -Source $generated -SourceColumns 4 -SourceRows 5 -SourceIndex $targetIndex -TargetColumns 4 -TargetIndex $targetIndex -CellSize 256
                    }
                    elseif ($targetIndex -eq 9) {
                        # Image generation omitted Throw Knife. Preserve the approved production cell.
                        Copy-AtlasCell -Graphics $graphics -Source $approved -SourceColumns 4 -SourceRows 5 -SourceIndex 9 -TargetColumns 4 -TargetIndex 9 -CellSize 256
                    }
                    else {
                        Copy-AtlasCell -Graphics $graphics -Source $generated -SourceColumns 4 -SourceRows 5 -SourceIndex ($targetIndex - 1) -TargetColumns 4 -TargetIndex $targetIndex -CellSize 256
                    }
                }
            }
            finally {
                $graphics.Dispose()
            }
            $target.Save($OutputPath, [System.Drawing.Imaging.ImageFormat]::Png)
        }
        finally {
            $target.Dispose()
        }
    }
    finally {
        $generated.Dispose()
        $approved.Dispose()
    }
}

function New-SpellAtlas {
    param([string]$OutputPath)

    $sourcePath = Join-Path $ArtRoot "source-signature-spell-icon-atlas-$Version-alpha.png"
    $source = [System.Drawing.Bitmap]::new($sourcePath)
    try {
        $target = [System.Drawing.Bitmap]::new(1280, 1280, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
        try {
            $graphics = [System.Drawing.Graphics]::FromImage($target)
            try {
                $graphics.Clear([System.Drawing.Color]::Transparent)
                $graphics.CompositingMode = [System.Drawing.Drawing2D.CompositingMode]::SourceCopy
                $graphics.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighSpeed
                $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::NearestNeighbor
                $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::Half
                $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::None
                for ($index = 0; $index -lt 25; $index++) {
                    Copy-AtlasCell -Graphics $graphics -Source $source -SourceColumns 5 -SourceRows 5 -SourceIndex $index -TargetColumns 5 -TargetIndex $index -CellSize 256
                }
            }
            finally {
                $graphics.Dispose()
            }
            $target.Save($OutputPath, [System.Drawing.Imaging.ImageFormat]::Png)
        }
        finally {
            $target.Dispose()
        }
    }
    finally {
        $source.Dispose()
    }
}

function New-MagicAtlas {
    param([string]$OutputPath)

    $sourcePath = Join-Path $ArtRoot "source-magic-ui-atlas-$Version-alpha.png"
    $source = [System.Drawing.Bitmap]::new($sourcePath)
    try {
        $target = [System.Drawing.Bitmap]::new(1024, 1024, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
        try {
            $graphics = [System.Drawing.Graphics]::FromImage($target)
            try {
                $graphics.Clear([System.Drawing.Color]::Transparent)
                $graphics.CompositingMode = [System.Drawing.Drawing2D.CompositingMode]::SourceCopy
                $graphics.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighSpeed
                $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::NearestNeighbor
                $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::Half
                $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::None
                $sourceOrder = @(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 15, 12, 13, 14)
                for ($targetIndex = 0; $targetIndex -lt 16; $targetIndex++) {
                    Copy-AtlasCell -Graphics $graphics -Source $source -SourceColumns 4 -SourceRows 4 -SourceIndex $sourceOrder[$targetIndex] -TargetColumns 4 -TargetIndex $targetIndex -CellSize 256
                }
            }
            finally {
                $graphics.Dispose()
            }
            $target.Save($OutputPath, [System.Drawing.Imaging.ImageFormat]::Png)
        }
        finally {
            $target.Dispose()
        }
    }
    finally {
        $source.Dispose()
    }
}

function Export-WithAseprite {
    param(
        [string]$RuntimePath,
        [string]$EditablePath
    )

    $asepriteRoot = Split-Path -Parent $AsepriteExe
    $runtimeFullPath = [System.IO.Path]::GetFullPath($RuntimePath)
    $editableFullPath = [System.IO.Path]::GetFullPath($EditablePath)
    $toEditable = "cd /d `"$asepriteRoot`" && `"$AsepriteExe`" -b `"$runtimeFullPath`" --save-as `"$editableFullPath`""
    cmd.exe /c $toEditable
    if ($LASTEXITCODE -ne 0 -or -not (Test-Path -LiteralPath $EditablePath)) {
        throw "Aseprite failed to create $EditablePath"
    }
    $toRuntime = "cd /d `"$asepriteRoot`" && `"$AsepriteExe`" -b `"$editableFullPath`" --save-as `"$runtimeFullPath`""
    cmd.exe /c $toRuntime
    if ($LASTEXITCODE -ne 0 -or -not (Test-Path -LiteralPath $RuntimePath)) {
        throw "Aseprite failed to export $RuntimePath"
    }
}

function Measure-Atlas {
    param(
        [string]$Path,
        [int]$Columns,
        [int]$Rows
    )

    $bitmap = [System.Drawing.Bitmap]::new($Path)
    try {
        $cells = @()
        for ($index = 0; $index -lt ($Columns * $Rows); $index++) {
            $rect = Get-CellRectangle -Bitmap $bitmap -Columns $Columns -Rows $Rows -Index $index
            $visible = 0
            $boundary = 0
            for ($y = 0; $y -lt $rect.Height; $y++) {
                for ($x = 0; $x -lt $rect.Width; $x++) {
                    $alpha = $bitmap.GetPixel($rect.X + $x, $rect.Y + $y).A
                    if ($alpha -ge 24) {
                        $visible++
                        if ($x -eq 0 -or $y -eq 0 -or $x -eq ($rect.Width - 1) -or $y -eq ($rect.Height - 1)) {
                            $boundary++
                        }
                    }
                }
            }
            $cells += [ordered]@{
                index = $index
                visibleFraction = [Math]::Round($visible / [double]($rect.Width * $rect.Height), 4)
                boundaryPixels = $boundary
            }
        }

        return [ordered]@{
            file = [System.IO.Path]::GetFileName($Path)
            width = $bitmap.Width
            height = $bitmap.Height
            columns = $Columns
            rows = $Rows
            cells = $cells
        }
    }
    finally {
        $bitmap.Dispose()
    }
}

$abilityRuntime = Join-Path $ArtRoot "ability-icon-atlas-runtime-$Version.png"
$abilityEditable = Join-Path $ArtRoot "source-ability-icon-atlas-$Version.aseprite"
$spellRuntime = Join-Path $ArtRoot "signature-spell-icon-atlas-runtime-$Version.png"
$spellEditable = Join-Path $ArtRoot "source-signature-spell-icon-atlas-$Version.aseprite"
$magicRuntime = Join-Path $ArtRoot "magic-ui-atlas-runtime-$Version.png"
$magicEditable = Join-Path $ArtRoot "source-magic-ui-atlas-$Version.aseprite"

New-AbilityAtlas -OutputPath $abilityRuntime
New-SpellAtlas -OutputPath $spellRuntime
New-MagicAtlas -OutputPath $magicRuntime
Export-WithAseprite -RuntimePath $abilityRuntime -EditablePath $abilityEditable
Export-WithAseprite -RuntimePath $spellRuntime -EditablePath $spellEditable
Export-WithAseprite -RuntimePath $magicRuntime -EditablePath $magicEditable

$validation = [ordered]@{
    version = $Version
    exportedWith = $AsepriteExe
    ability = Measure-Atlas -Path $abilityRuntime -Columns 4 -Rows 5
    spell = Measure-Atlas -Path $spellRuntime -Columns 5 -Rows 5
    magic = Measure-Atlas -Path $magicRuntime -Columns 4 -Rows 4
}
$validationPath = Join-Path $ArtRoot "power-icon-atlases-runtime-$Version-validation.json"
$validation | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $validationPath -Encoding utf8

Get-Item -LiteralPath $abilityRuntime, $abilityEditable, $spellRuntime, $spellEditable, $magicRuntime, $magicEditable, $validationPath
