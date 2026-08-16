param(
    [string]$OutputPath = ""
)

$ErrorActionPreference = "Stop"

Add-Type -AssemblyName System.Drawing

$projectRoot = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    $OutputPath = Join-Path $projectRoot "Docs\ArtReferences\title-menu-icon-atlas-runtime-v2.16.0.png"
}
$OutputPath = [System.IO.Path]::GetFullPath($OutputPath)

$columns = 5
$cellSize = 256
$supersample = 4
$width = $columns * $cellSize
$height = $cellSize

function Get-Color {
    param(
        [Parameter(Mandatory = $true)][string]$Hex,
        [int]$Alpha = 255
    )

    $value = $Hex.TrimStart('#')
    if ($value.Length -ne 6) {
        throw "Expected a six-digit RGB color, got '$Hex'."
    }
    return [System.Drawing.Color]::FromArgb(
        $Alpha,
        [Convert]::ToInt32($value.Substring(0, 2), 16),
        [Convert]::ToInt32($value.Substring(2, 2), 16),
        [Convert]::ToInt32($value.Substring(4, 2), 16))
}

$ink = Get-Color "241816"
$iron = Get-Color "51372F"
$parchment = Get-Color "E8BD70"
$parchmentLight = Get-Color "FFE6A6"
$ember = Get-Color "E77830"
$emberLight = Get-Color "FFB64F"
$road = Get-Color "89725B"
$teal = Get-Color "5BA8A2"
$tealLight = Get-Color "A6DDD4"
$wine = Get-Color "813C43"
$shadow = Get-Color "130C0B" 120

$script:graphics = $null

function Get-X {
    param([int]$Cell, [double]$X)
    return [single](($Cell * $cellSize) + $X)
}

function New-RoundPen {
    param([System.Drawing.Color]$Color, [single]$LineWidth)

    $pen = [System.Drawing.Pen]::new($Color, $LineWidth)
    $pen.LineJoin = [System.Drawing.Drawing2D.LineJoin]::Round
    $pen.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
    $pen.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
    return $pen
}

function Get-Points {
    param([int]$Cell, [double[]]$Coordinates)

    if (($Coordinates.Length % 2) -ne 0) {
        throw "Point coordinates must be x/y pairs."
    }
    $points = [System.Drawing.PointF[]]::new($Coordinates.Length / 2)
    for ($i = 0; $i -lt $points.Length; $i++) {
        $points[$i] = [System.Drawing.PointF]::new(
            (Get-X $Cell $Coordinates[$i * 2]),
            [single]$Coordinates[$i * 2 + 1])
    }
    return ,$points
}

function Fill-Polygon {
    param([int]$Cell, [System.Drawing.Color]$Color, [double[]]$Coordinates)

    $brush = [System.Drawing.SolidBrush]::new($Color)
    try { $script:graphics.FillPolygon($brush, (Get-Points $Cell $Coordinates)) }
    finally { $brush.Dispose() }
}

function Stroke-Polygon {
    param([int]$Cell, [System.Drawing.Color]$Color, [single]$LineWidth, [double[]]$Coordinates)

    $pen = New-RoundPen $Color $LineWidth
    try { $script:graphics.DrawPolygon($pen, (Get-Points $Cell $Coordinates)) }
    finally { $pen.Dispose() }
}

function Draw-Lines {
    param([int]$Cell, [System.Drawing.Color]$Color, [single]$LineWidth, [double[]]$Coordinates)

    $pen = New-RoundPen $Color $LineWidth
    try { $script:graphics.DrawLines($pen, (Get-Points $Cell $Coordinates)) }
    finally { $pen.Dispose() }
}

function Fill-Rectangle {
    param([int]$Cell, [System.Drawing.Color]$Color, [double]$X, [double]$Y, [double]$RectWidth, [double]$RectHeight)

    $brush = [System.Drawing.SolidBrush]::new($Color)
    try {
        $script:graphics.FillRectangle($brush, (Get-X $Cell $X), [single]$Y, [single]$RectWidth, [single]$RectHeight)
    }
    finally { $brush.Dispose() }
}

function Stroke-Rectangle {
    param([int]$Cell, [System.Drawing.Color]$Color, [single]$LineWidth, [double]$X, [double]$Y, [double]$RectWidth, [double]$RectHeight)

    $pen = New-RoundPen $Color $LineWidth
    try {
        $script:graphics.DrawRectangle($pen, (Get-X $Cell $X), [single]$Y, [single]$RectWidth, [single]$RectHeight)
    }
    finally { $pen.Dispose() }
}

function Fill-Ellipse {
    param([int]$Cell, [System.Drawing.Color]$Color, [double]$X, [double]$Y, [double]$EllipseWidth, [double]$EllipseHeight)

    $brush = [System.Drawing.SolidBrush]::new($Color)
    try {
        $script:graphics.FillEllipse($brush, (Get-X $Cell $X), [single]$Y, [single]$EllipseWidth, [single]$EllipseHeight)
    }
    finally { $brush.Dispose() }
}

function Stroke-Ellipse {
    param([int]$Cell, [System.Drawing.Color]$Color, [single]$LineWidth, [double]$X, [double]$Y, [double]$EllipseWidth, [double]$EllipseHeight)

    $pen = New-RoundPen $Color $LineWidth
    try {
        $script:graphics.DrawEllipse($pen, (Get-X $Cell $X), [single]$Y, [single]$EllipseWidth, [single]$EllipseHeight)
    }
    finally { $pen.Dispose() }
}

function Draw-Star {
    param(
        [int]$Cell,
        [System.Drawing.Color]$Fill,
        [System.Drawing.Color]$Outline,
        [double]$CenterX,
        [double]$CenterY,
        [double]$OuterRadius,
        [double]$InnerRadius,
        [int]$Points = 8,
        [single]$OutlineWidth = 6
    )

    $coords = [System.Collections.Generic.List[double]]::new()
    for ($i = 0; $i -lt ($Points * 2); $i++) {
        $radius = if (($i % 2) -eq 0) { $OuterRadius } else { $InnerRadius }
        $angle = (-[Math]::PI / 2) + ($i * [Math]::PI / $Points)
        $coords.Add($CenterX + ([Math]::Cos($angle) * $radius))
        $coords.Add($CenterY + ([Math]::Sin($angle) * $radius))
    }
    Fill-Polygon $Cell $Fill $coords.ToArray()
    Stroke-Polygon $Cell $Outline $OutlineWidth $coords.ToArray()
}

function Draw-ContinueIcon {
    $cell = 0
    Fill-Rectangle $cell $shadow 53 54 126 158
    Fill-Rectangle $cell $parchment 45 46 126 158
    Stroke-Rectangle $cell $ink 11 45 46 126 158
    Fill-Ellipse $cell $parchmentLight 31 36 57 32
    Stroke-Ellipse $cell $ink 10 31 36 57 32
    Fill-Ellipse $cell $parchment 128 182 57 32
    Stroke-Ellipse $cell $ink 10 128 182 57 32
    Draw-Lines $cell $wine 8 @(70, 93, 145, 93)
    Draw-Lines $cell $wine 8 @(70, 124, 137, 124)
    Draw-Lines $cell $wine 8 @(70, 155, 122, 155)
    Draw-Lines $cell $shadow 20 @(188, 85, 225, 128, 188, 171)
    Draw-Lines $cell $emberLight 11 @(188, 85, 225, 128, 188, 171)
}

function Draw-NewGameIcon {
    $cell = 1
    Fill-Polygon $cell $shadow @(42, 204, 94, 148, 94, 76, 162, 42, 222, 76, 222, 204)
    Draw-Lines $cell $ink 24 @(48, 199, 48, 91, 76, 91, 76, 63, 180, 63, 180, 91, 208, 91, 208, 199)
    Draw-Lines $cell $parchment 12 @(48, 199, 48, 91, 76, 91, 76, 63, 180, 63, 180, 91, 208, 91, 208, 199)
    Draw-Lines $cell $teal 8 @(94, 68, 94, 196)
    Draw-Lines $cell $teal 8 @(162, 68, 162, 196)
    Fill-Polygon $cell $road @(54, 221, 202, 221, 158, 139, 98, 139)
    Stroke-Polygon $cell $ink 10 @(54, 221, 202, 221, 158, 139, 98, 139)
    Draw-Lines $cell $parchmentLight 7 @(128, 210, 128, 177, 128, 150)
    Draw-Star $cell $emberLight $ink 205 49 27 10 8 6
}

function Draw-SettingsIcon {
    $cell = 2
    $coords = [System.Collections.Generic.List[double]]::new()
    $centerX = 128.0
    $centerY = 128.0
    for ($tooth = 0; $tooth -lt 8; $tooth++) {
        foreach ($part in 0..3) {
            $angle = (-[Math]::PI / 2) + (($tooth * 4 + $part) * (2 * [Math]::PI / 32))
            $radius = if ($part -eq 0 -or $part -eq 1) { 94.0 } else { 72.0 }
            $coords.Add($centerX + ([Math]::Cos($angle) * $radius))
            $coords.Add($centerY + ([Math]::Sin($angle) * $radius))
        }
    }
    Fill-Polygon $cell $shadow ($coords.ToArray() | ForEach-Object { $_ + 5 })
    Fill-Polygon $cell $teal $coords.ToArray()
    Stroke-Polygon $cell $ink 12 $coords.ToArray()
    Fill-Ellipse $cell $ink 79 79 98 98
    Fill-Ellipse $cell $parchmentLight 96 96 64 64
    Stroke-Ellipse $cell $ink 8 96 96 64 64
}

function Draw-ExitIcon {
    $cell = 3
    $door = [System.Collections.Generic.List[double]]::new()
    $door.Add(55); $door.Add(218)
    $door.Add(55); $door.Add(112)
    for ($step = 0; $step -le 8; $step++) {
        $angle = [Math]::PI - ($step * [Math]::PI / 8)
        $door.Add(111 + ([Math]::Cos($angle) * 56))
        $door.Add(112 - ([Math]::Sin($angle) * 56))
    }
    $door.Add(167); $door.Add(218)
    Fill-Polygon $cell $shadow ($door.ToArray() | ForEach-Object { $_ + 5 })
    Fill-Polygon $cell $wine $door.ToArray()
    Stroke-Polygon $cell $ink 13 $door.ToArray()
    Draw-Lines $cell $parchment 7 @(82, 211, 82, 116, 90, 88, 111, 76, 133, 88, 141, 116, 141, 211)
    Fill-Ellipse $cell $parchmentLight 121 139 13 13
    Draw-Lines $cell $ink 20 @(132, 128, 217, 128)
    Draw-Lines $cell $emberLight 10 @(132, 128, 217, 128)
    Fill-Polygon $cell $emberLight @(217, 128, 186, 101, 186, 155)
    Stroke-Polygon $cell $ink 7 @(217, 128, 186, 101, 186, 155)
}

function Draw-TestingIcon {
    $cell = 4
    Fill-Polygon $cell $shadow @(89, 38, 167, 38, 156, 82, 199, 187, 184, 218, 72, 218, 57, 187, 100, 82)
    Fill-Polygon $cell $tealLight @(82, 31, 174, 31, 160, 85, 204, 190, 187, 224, 69, 224, 52, 190, 96, 85)
    Stroke-Polygon $cell $ink 13 @(82, 31, 174, 31, 160, 85, 204, 190, 187, 224, 69, 224, 52, 190, 96, 85)
    Fill-Rectangle $cell $iron 85 23 86 32
    Stroke-Rectangle $cell $ink 9 85 23 86 32
    Fill-Polygon $cell $ember @(65, 167, 191, 167, 204, 190, 187, 224, 69, 224, 52, 190)
    Draw-Lines $cell $parchmentLight 6 @(78, 191, 106, 181, 133, 195, 181, 178)
    Fill-Ellipse $cell $parchmentLight 91 120 16 16
    Fill-Ellipse $cell $parchmentLight 145 143 11 11
    Draw-Star $cell $emberLight $ink 201 64 27 10 8 6
}

$working = [System.Drawing.Bitmap]::new(
    $width * $supersample,
    $height * $supersample,
    [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$script:graphics = [System.Drawing.Graphics]::FromImage($working)
try {
    $script:graphics.Clear([System.Drawing.Color]::Transparent)
    $script:graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $script:graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $script:graphics.CompositingMode = [System.Drawing.Drawing2D.CompositingMode]::SourceOver
    $script:graphics.ScaleTransform($supersample, $supersample)

    Draw-ContinueIcon
    Draw-NewGameIcon
    Draw-SettingsIcon
    Draw-ExitIcon
    Draw-TestingIcon
}
finally {
    $script:graphics.Dispose()
    $script:graphics = $null
}

$final = [System.Drawing.Bitmap]::new($width, $height, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$finalGraphics = [System.Drawing.Graphics]::FromImage($final)
try {
    $finalGraphics.Clear([System.Drawing.Color]::Transparent)
    $finalGraphics.CompositingMode = [System.Drawing.Drawing2D.CompositingMode]::SourceCopy
    $finalGraphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $finalGraphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $finalGraphics.DrawImage($working, [System.Drawing.Rectangle]::new(0, 0, $width, $height))

    $outputDirectory = Split-Path -Parent $OutputPath
    if (-not (Test-Path -LiteralPath $outputDirectory)) {
        $null = New-Item -ItemType Directory -Path $outputDirectory -Force
    }
    $final.Save($OutputPath, [System.Drawing.Imaging.ImageFormat]::Png)
}
finally {
    $finalGraphics.Dispose()
    $final.Dispose()
    $working.Dispose()
}

Write-Host "Wrote deterministic title menu icon atlas: $OutputPath ($($width)x$($height), 5x1 RGBA)"
