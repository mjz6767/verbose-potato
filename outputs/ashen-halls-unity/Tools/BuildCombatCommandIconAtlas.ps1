param(
    [string]$OutputPath = ""
)

$ErrorActionPreference = "Stop"

Add-Type -AssemblyName System.Drawing

$projectRoot = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    $OutputPath = Join-Path $projectRoot "Docs\ArtReferences\combat-command-icon-atlas-runtime-v1.99.0.png"
}
$OutputPath = [System.IO.Path]::GetFullPath($OutputPath)

$columns = 5
$rows = 4
$cellSize = 256
$supersample = 4
$logicalWidth = $columns * $cellSize
$logicalHeight = $rows * $cellSize

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

$ink = Get-Color "091217"
$deep = Get-Color "10242B"
$pale = Get-Color "F5F0D7"
$teal = Get-Color "42E0D0"
$tealDark = Get-Color "147C7B"
$red = Get-Color "FF5B4E"
$redDark = Get-Color "8E282B"
$violet = Get-Color "B989FF"
$violetDark = Get-Color "542B87"
$gold = Get-Color "F5C85B"
$goldDark = Get-Color "9E641D"
$orange = Get-Color "FF9D3C"
$slate = Get-Color "5D7580"

$script:graphics = $null

function Get-CellOffset {
    param([int]$Index)

    return [int[]]@(
        [int](($Index % $columns) * $cellSize)
        [int]([Math]::Floor($Index / $columns) * $cellSize))
}

function Get-Points {
    param(
        [int]$Index,
        [double[]]$Coordinates
    )

    if (($Coordinates.Length % 2) -ne 0) {
        throw "Point coordinates must be x/y pairs."
    }
    $offset = Get-CellOffset $Index
    $points = [System.Drawing.PointF[]]::new($Coordinates.Length / 2)
    for ($i = 0; $i -lt $points.Length; $i++) {
        $points[$i] = [System.Drawing.PointF]::new(
            [single]($offset[0] + $Coordinates[$i * 2]),
            [single]($offset[1] + $Coordinates[$i * 2 + 1]))
    }
    return ,$points
}

function New-RoundPen {
    param(
        [System.Drawing.Color]$Color,
        [single]$Width
    )

    $pen = [System.Drawing.Pen]::new($Color, $Width)
    $pen.LineJoin = [System.Drawing.Drawing2D.LineJoin]::Round
    $pen.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
    $pen.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
    return $pen
}

function Fill-Polygon {
    param(
        [int]$Index,
        [System.Drawing.Color]$Color,
        [double[]]$Coordinates
    )

    $brush = [System.Drawing.SolidBrush]::new($Color)
    try {
        $script:graphics.FillPolygon($brush, (Get-Points $Index $Coordinates))
    }
    finally {
        $brush.Dispose()
    }
}

function Stroke-Polygon {
    param(
        [int]$Index,
        [System.Drawing.Color]$Color,
        [single]$Width,
        [double[]]$Coordinates
    )

    $pen = New-RoundPen $Color $Width
    try {
        $script:graphics.DrawPolygon($pen, (Get-Points $Index $Coordinates))
    }
    finally {
        $pen.Dispose()
    }
}

function Draw-Lines {
    param(
        [int]$Index,
        [System.Drawing.Color]$Color,
        [single]$Width,
        [double[]]$Coordinates
    )

    $pen = New-RoundPen $Color $Width
    try {
        $script:graphics.DrawLines($pen, (Get-Points $Index $Coordinates))
    }
    finally {
        $pen.Dispose()
    }
}

function Draw-Line {
    param(
        [int]$Index,
        [System.Drawing.Color]$Color,
        [single]$Width,
        [double]$X1,
        [double]$Y1,
        [double]$X2,
        [double]$Y2
    )

    Draw-Lines $Index $Color $Width @($X1, $Y1, $X2, $Y2)
}

function Fill-Ellipse {
    param(
        [int]$Index,
        [System.Drawing.Color]$Color,
        [double]$X,
        [double]$Y,
        [double]$Width,
        [double]$Height
    )

    $offset = Get-CellOffset $Index
    $brush = [System.Drawing.SolidBrush]::new($Color)
    try {
        $script:graphics.FillEllipse($brush, [single]($offset[0] + $X), [single]($offset[1] + $Y), [single]$Width, [single]$Height)
    }
    finally {
        $brush.Dispose()
    }
}

function Stroke-Ellipse {
    param(
        [int]$Index,
        [System.Drawing.Color]$Color,
        [single]$LineWidth,
        [double]$X,
        [double]$Y,
        [double]$Width,
        [double]$Height
    )

    $offset = Get-CellOffset $Index
    $pen = New-RoundPen $Color $LineWidth
    try {
        $script:graphics.DrawEllipse($pen, [single]($offset[0] + $X), [single]($offset[1] + $Y), [single]$Width, [single]$Height)
    }
    finally {
        $pen.Dispose()
    }
}

function Fill-Rectangle {
    param(
        [int]$Index,
        [System.Drawing.Color]$Color,
        [double]$X,
        [double]$Y,
        [double]$Width,
        [double]$Height
    )

    $offset = Get-CellOffset $Index
    $brush = [System.Drawing.SolidBrush]::new($Color)
    try {
        $script:graphics.FillRectangle($brush, [single]($offset[0] + $X), [single]($offset[1] + $Y), [single]$Width, [single]$Height)
    }
    finally {
        $brush.Dispose()
    }
}

function Draw-Bezier {
    param(
        [int]$Index,
        [System.Drawing.Color]$Color,
        [single]$Width,
        [double[]]$Coordinates
    )

    if ($Coordinates.Length -ne 8) {
        throw "Bezier coordinates must contain four x/y points."
    }
    $points = Get-Points $Index $Coordinates
    $pen = New-RoundPen $Color $Width
    try {
        $script:graphics.DrawBezier($pen, $points[0], $points[1], $points[2], $points[3])
    }
    finally {
        $pen.Dispose()
    }
}

function Draw-GlowLine {
    param(
        [int]$Index,
        [System.Drawing.Color]$Color,
        [single]$Width,
        [double[]]$Coordinates,
        [System.Drawing.Color]$CoreColor = $pale
    )

    $glow = [System.Drawing.Color]::FromArgb(62, $Color.R, $Color.G, $Color.B)
    Draw-Lines $Index $glow ($Width + 13) $Coordinates
    Draw-Lines $Index $ink ($Width + 7) $Coordinates
    Draw-Lines $Index $Color $Width $Coordinates
    Draw-Lines $Index $CoreColor ([Math]::Max(2, $Width * 0.28)) $Coordinates
}

function Draw-Diamond {
    param(
        [int]$Index,
        [System.Drawing.Color]$Fill,
        [System.Drawing.Color]$Outline,
        [double]$CenterX,
        [double]$CenterY,
        [double]$RadiusX,
        [double]$RadiusY,
        [single]$OutlineWidth = 8
    )

    $shape = @(
        $CenterX, ($CenterY - $RadiusY),
        ($CenterX + $RadiusX), $CenterY,
        $CenterX, ($CenterY + $RadiusY),
        ($CenterX - $RadiusX), $CenterY)
    Fill-Polygon $Index $Fill $shape
    Stroke-Polygon $Index $Outline $OutlineWidth $shape
}

function Draw-Star {
    param(
        [int]$Index,
        [System.Drawing.Color]$Fill,
        [System.Drawing.Color]$Outline,
        [double]$CenterX,
        [double]$CenterY,
        [double]$OuterRadius,
        [double]$InnerRadius,
        [int]$Points = 8,
        [single]$OutlineWidth = 7
    )

    $coords = [System.Collections.Generic.List[double]]::new()
    for ($i = 0; $i -lt ($Points * 2); $i++) {
        $radius = if (($i % 2) -eq 0) { $OuterRadius } else { $InnerRadius }
        $angle = -[Math]::PI / 2 + $i * [Math]::PI / $Points
        $coords.Add($CenterX + [Math]::Cos($angle) * $radius)
        $coords.Add($CenterY + [Math]::Sin($angle) * $radius)
    }
    Fill-Polygon $Index $Fill $coords.ToArray()
    Stroke-Polygon $Index $Outline $OutlineWidth $coords.ToArray()
}

function Draw-Arrow {
    param(
        [int]$Index,
        [System.Drawing.Color]$Color,
        [double]$X1,
        [double]$Y1,
        [double]$X2,
        [double]$Y2,
        [single]$Width = 10
    )

    Draw-GlowLine $Index $Color $Width @($X1, $Y1, $X2, $Y2) $pale
    $angle = [Math]::Atan2($Y2 - $Y1, $X2 - $X1)
    $head = 24
    $wing = 15
    $backX = $X2 - [Math]::Cos($angle) * $head
    $backY = $Y2 - [Math]::Sin($angle) * $head
    $leftX = $backX + [Math]::Cos($angle + [Math]::PI / 2) * $wing
    $leftY = $backY + [Math]::Sin($angle + [Math]::PI / 2) * $wing
    $rightX = $backX + [Math]::Cos($angle - [Math]::PI / 2) * $wing
    $rightY = $backY + [Math]::Sin($angle - [Math]::PI / 2) * $wing
    Fill-Polygon $Index $Color @($X2, $Y2, $leftX, $leftY, $rightX, $rightY)
    Stroke-Polygon $Index $ink 5 @($X2, $Y2, $leftX, $leftY, $rightX, $rightY)
}

function Draw-MoveIcon {
    $i = 0
    # Forward motion chevrons remain visually separate from the boot at small scale.
    Draw-Lines $i ([System.Drawing.Color]::FromArgb(100, $teal.R, $teal.G, $teal.B)) 17 @(55, 92, 30, 116, 55, 140)
    Draw-Lines $i $teal 8 @(55, 92, 30, 116, 55, 140)
    Draw-Lines $i ([System.Drawing.Color]::FromArgb(90, $teal.R, $teal.G, $teal.B)) 15 @(48, 150, 29, 169, 48, 188)
    Draw-Lines $i $teal 7 @(48, 150, 29, 169, 48, 188)

    $boot = @(74, 38, 139, 38, 141, 116, 162, 139, 203, 153, 224, 176, 218, 202, 190, 218, 91, 216, 57, 195, 57, 168, 88, 149, 87, 106, 70, 83)
    Fill-Polygon $i $deep $boot
    Stroke-Polygon $i $ink 16 $boot
    Stroke-Polygon $i $teal 8 $boot
    Draw-Line $i $pale 6 91 57 126 57
    Draw-Line $i $gold 8 72 183 205 184
    Draw-Line $i $tealDark 8 94 84 137 84
    Draw-Line $i $tealDark 8 94 106 138 106
    Draw-Line $i $pale 5 158 158 199 169
}

function Draw-AttackIcon {
    $i = 1
    foreach ($slash in @(@(37, 80, 91, 134), @(32, 126, 75, 169), @(68, 42, 111, 85))) {
        Draw-Line $i ([System.Drawing.Color]::FromArgb(105, $red.R, $red.G, $red.B)) 18 $slash[0] $slash[1] $slash[2] $slash[3]
        Draw-Line $i $red 7 $slash[0] $slash[1] $slash[2] $slash[3]
    }
    $blade = @(63, 197, 82, 216, 214, 73, 224, 32, 186, 49, 52, 188)
    Fill-Polygon $i $pale $blade
    Stroke-Polygon $i $ink 15 $blade
    Stroke-Polygon $i $red 6 $blade
    Draw-Line $i ([System.Drawing.Color]::FromArgb(170, $slate.R, $slate.G, $slate.B)) 8 77 189 202 57
    Draw-Line $i $pale 4 91 186 205 64
    Draw-Line $i $gold 15 45 175 90 220
    Draw-Line $i $ink 7 45 175 90 220
    Fill-Polygon $i $gold @(45, 197, 59, 211, 41, 229, 28, 216)
    Stroke-Polygon $i $ink 7 @(45, 197, 59, 211, 41, 229, 28, 216)
}

function Draw-SpellIcon {
    $i = 2
    $leftPage = @(34, 113, 111, 101, 128, 119, 128, 218, 105, 202, 38, 210)
    $rightPage = @(222, 113, 145, 101, 128, 119, 128, 218, 151, 202, 218, 210)
    Fill-Polygon $i $pale $leftPage
    Fill-Polygon $i $pale $rightPage
    Stroke-Polygon $i $ink 14 $leftPage
    Stroke-Polygon $i $ink 14 $rightPage
    Stroke-Polygon $i $violet 6 $leftPage
    Stroke-Polygon $i $violet 6 $rightPage
    Draw-Line $i $goldDark 5 128 122 128 213
    foreach ($y in @(139, 160, 181)) {
        Draw-Line $i $violetDark 5 53 $y 105 ($y - 5)
        Draw-Line $i $violetDark 5 151 ($y - 5) 203 $y
    }
    Draw-Star $i $violet $ink 128 61 35 14 8 8
    Draw-Star $i $pale $violetDark 128 61 17 7 8 4
    Draw-Line $i ([System.Drawing.Color]::FromArgb(150, $violet.R, $violet.G, $violet.B)) 8 79 83 60 64
    Draw-Line $i ([System.Drawing.Color]::FromArgb(150, $violet.R, $violet.G, $violet.B)) 8 177 83 196 64
}

function Draw-GuardIcon {
    $i = 3
    $shield = @(128, 31, 210, 64, 201, 143, 176, 191, 128, 224, 80, 191, 55, 143, 46, 64)
    Fill-Polygon $i $deep $shield
    Stroke-Polygon $i $ink 17 $shield
    Stroke-Polygon $i $gold 9 $shield
    $inner = @(128, 51, 186, 76, 179, 136, 160, 173, 128, 199, 96, 173, 77, 136, 70, 76)
    Fill-Polygon $i ([System.Drawing.Color]::FromArgb(210, $tealDark.R, $tealDark.G, $tealDark.B)) $inner
    Stroke-Polygon $i $teal 6 $inner
    Draw-Star $i $gold $ink 128 121 43 16 4 7
    Draw-Star $i $pale $goldDark 128 121 19 8 4 4
}

function Draw-ElixirIcon {
    $i = 4
    Fill-Ellipse $i ([System.Drawing.Color]::FromArgb(85, $red.R, $red.G, $red.B)) 47 68 162 168
    $bottle = @(94, 40, 162, 40, 162, 78, 176, 94, 195, 129, 196, 181, 174, 215, 128, 229, 82, 215, 60, 181, 61, 129, 80, 94, 94, 78)
    Fill-Polygon $i $deep $bottle
    Stroke-Polygon $i $ink 16 $bottle
    Stroke-Polygon $i $gold 8 $bottle
    $liquid = @(75, 145, 181, 145, 185, 181, 165, 204, 128, 216, 91, 204, 71, 181)
    Fill-Polygon $i $red $liquid
    Stroke-Polygon $i $redDark 5 $liquid
    Fill-Rectangle $i $pale 101 26 54 23
    Stroke-Polygon $i $ink 7 @(101, 26, 155, 26, 155, 49, 101, 49)
    Draw-Line $i $pale 7 91 109 78 139
    Draw-Star $i $orange $redDark 185 73 18 7 4 5
    Draw-Star $i $red $ink 58 100 13 5 4 4
}

function Draw-EndTurnIcon {
    $i = 5
    # A descending hourglass communicates committing the current turn, not sleep.
    Draw-Lines $i ([System.Drawing.Color]::FromArgb(90, $gold.R, $gold.G, $gold.B)) 18 @(44, 82, 61, 99, 44, 116)
    Draw-Lines $i $gold 7 @(44, 82, 61, 99, 44, 116)
    Draw-Lines $i ([System.Drawing.Color]::FromArgb(90, $gold.R, $gold.G, $gold.B)) 18 @(212, 82, 195, 99, 212, 116)
    Draw-Lines $i $gold 7 @(212, 82, 195, 99, 212, 116)
    Fill-Rectangle $i $gold 62 31 132 22
    Fill-Rectangle $i $gold 62 203 132 22
    Stroke-Polygon $i $ink 8 @(62, 31, 194, 31, 194, 53, 62, 53)
    Stroke-Polygon $i $ink 8 @(62, 203, 194, 203, 194, 225, 62, 225)
    $glass = @(78, 55, 178, 55, 166, 91, 141, 125, 166, 163, 178, 201, 78, 201, 90, 163, 115, 125, 90, 91)
    Fill-Polygon $i ([System.Drawing.Color]::FromArgb(170, 32, 66, 70)) $glass
    Stroke-Polygon $i $ink 14 $glass
    Stroke-Polygon $i $teal 6 $glass
    Fill-Polygon $i $gold @(91, 70, 165, 70, 148, 94, 128, 115, 108, 94)
    Fill-Polygon $i $gold @(96, 186, 160, 186, 145, 165, 128, 147, 111, 165)
    Draw-Line $i $pale 5 128 115 128 150
}

function Draw-RangedIcon {
    $i = 6
    Draw-Bezier $i ([System.Drawing.Color]::FromArgb(90, $teal.R, $teal.G, $teal.B)) 24 @(76, 34, 199, 69, 199, 187, 76, 222)
    Draw-Bezier $i $ink 16 @(76, 34, 199, 69, 199, 187, 76, 222)
    Draw-Bezier $i $gold 8 @(76, 34, 199, 69, 199, 187, 76, 222)
    Draw-Line $i $pale 5 76 34 104 128
    Draw-Line $i $pale 5 104 128 76 222
    Draw-Arrow $i $teal 37 181 215 72 10
}

function Draw-SkillIcon {
    $i = 7
    # A raised gauntlet and energy bolt distinguish learned skills from the spellbook.
    $fist = @(72, 118, 72, 69, 91, 55, 108, 63, 116, 36, 137, 31, 151, 44, 151, 75, 169, 56, 189, 62, 196, 80, 191, 112, 207, 115, 217, 134, 205, 158, 183, 193, 165, 222, 92, 222, 74, 194, 56, 163, 54, 137)
    Fill-Polygon $i $goldDark $fist
    Stroke-Polygon $i $ink 16 $fist
    Stroke-Polygon $i $gold 8 $fist
    Draw-Line $i $pale 6 92 75 92 116
    Draw-Line $i $pale 6 124 58 124 106
    Draw-Line $i $pale 6 160 78 151 117
    Draw-Line $i $teal 8 81 171 181 171
    Fill-Polygon $i $teal @(190, 25, 142, 96, 171, 94, 145, 143, 215, 69, 184, 72)
    Stroke-Polygon $i $ink 7 @(190, 25, 142, 96, 171, 94, 145, 143, 215, 69, 184, 72)
}

function Draw-TargetingIcon {
    $i = 8
    Stroke-Ellipse $i ([System.Drawing.Color]::FromArgb(75, $teal.R, $teal.G, $teal.B)) 23 43 43 170 170
    Stroke-Ellipse $i $teal 9 43 43 170 170
    Stroke-Ellipse $i $pale 5 79 79 98 98
    foreach ($line in @(@(128, 32, 128, 76), @(128, 180, 128, 224), @(32, 128, 76, 128), @(180, 128, 224, 128))) {
        Draw-GlowLine $i $teal 8 $line $pale
    }
    Draw-Diamond $i $gold $ink 128 128 18 18 6
}

function Draw-BlockedIcon {
    $i = 9
    Stroke-Ellipse $i ([System.Drawing.Color]::FromArgb(75, $red.R, $red.G, $red.B)) 27 38 38 180 180
    Stroke-Ellipse $i $red 12 38 38 180 180
    Draw-Diamond $i $deep $slate 128 128 54 54 8
    Draw-GlowLine $i $red 17 @(57, 199, 199, 57) $pale
}

function Draw-FireIcon {
    $i = 10
    $flame = @(128, 31, 162, 80, 158, 109, 191, 96, 210, 142, 199, 186, 169, 216, 128, 224, 87, 216, 57, 186, 46, 142, 66, 96, 90, 120, 93, 78)
    Fill-Polygon $i $redDark $flame
    Stroke-Polygon $i $ink 16 $flame
    Stroke-Polygon $i $red 8 $flame
    $inner = @(128, 77, 151, 119, 144, 147, 169, 139, 174, 174, 153, 199, 128, 207, 103, 199, 82, 174, 91, 140, 109, 153)
    Fill-Polygon $i $orange $inner
    Stroke-Polygon $i $gold 5 $inner
    Fill-Polygon $i $pale @(128, 128, 145, 164, 128, 192, 111, 164)
}

function Draw-MultiIcon {
    $i = 11
    Draw-Arrow $i $orange 35 188 171 52 11
    Draw-Arrow $i $red 52 218 218 94 11
    Draw-Arrow $i $gold 31 137 151 30 9
    Draw-Line $i ([System.Drawing.Color]::FromArgb(120, $orange.R, $orange.G, $orange.B)) 8 49 161 28 181
    Draw-Line $i ([System.Drawing.Color]::FromArgb(120, $red.R, $red.G, $red.B)) 8 71 199 45 219
}

function Draw-HealIcon {
    $i = 12
    Draw-Star $i $teal $ink 128 75 44 17 8 8
    Draw-Star $i $pale $tealDark 128 75 20 8 8 4
    $leftHand = @(31, 148, 53, 131, 96, 155, 120, 179, 120, 218, 84, 211, 52, 189)
    $rightHand = @(225, 148, 203, 131, 160, 155, 136, 179, 136, 218, 172, 211, 204, 189)
    Fill-Polygon $i $goldDark $leftHand
    Fill-Polygon $i $goldDark $rightHand
    Stroke-Polygon $i $ink 14 $leftHand
    Stroke-Polygon $i $ink 14 $rightHand
    Stroke-Polygon $i $gold 7 $leftHand
    Stroke-Polygon $i $gold 7 $rightHand
    Draw-Line $i $pale 5 59 157 97 177
    Draw-Line $i $pale 5 197 157 159 177
}

function Draw-StealthIcon {
    $i = 13
    $hood = @(128, 28, 182, 50, 210, 98, 204, 177, 174, 218, 82, 218, 52, 177, 46, 98, 74, 50)
    Fill-Polygon $i $violetDark $hood
    Stroke-Polygon $i $ink 17 $hood
    Stroke-Polygon $i $violet 8 $hood
    $face = @(128, 68, 171, 92, 180, 142, 157, 182, 128, 201, 99, 182, 76, 142, 85, 92)
    Fill-Polygon $i $ink $face
    Draw-Lines $i $violet 8 @(91, 139, 112, 131, 128, 139, 144, 131, 165, 139)
    Draw-Line $i $pale 4 101 136 113 133
    Draw-Line $i $pale 4 143 133 155 136
}

function Draw-DashIcon {
    $i = 14
    foreach ($line in @(@(34, 78, 96, 78), @(34, 117, 84, 117), @(38, 158, 98, 158))) {
        Draw-Line $i ([System.Drawing.Color]::FromArgb(95, $teal.R, $teal.G, $teal.B)) 17 $line[0] $line[1] $line[2] $line[3]
        Draw-Line $i $teal 7 $line[0] $line[1] $line[2] $line[3]
    }
    $bolt = @(154, 31, 94, 133, 128, 129, 102, 222, 201, 105, 162, 108, 198, 47)
    Fill-Polygon $i $gold $bolt
    Stroke-Polygon $i $ink 14 $bolt
    Stroke-Polygon $i $orange 6 $bolt
    Draw-Line $i $pale 5 158 50 117 121
}

function Draw-VolleyIcon {
    $i = 15
    Draw-Arrow $i $teal 39 201 176 55 9
    Draw-Arrow $i $gold 72 221 213 79 9
    Draw-Arrow $i $pale 36 151 145 34 8
    Draw-Line $i $tealDark 7 44 179 32 191
    Draw-Line $i $goldDark 7 81 201 66 216
}

function Draw-BreakIcon {
    $i = 16
    $wall = @(40, 54, 216, 54, 216, 207, 40, 207)
    Fill-Polygon $i $slate $wall
    Stroke-Polygon $i $ink 16 $wall
    Stroke-Polygon $i $red 7 $wall
    foreach ($seam in @(@(40, 103, 98, 103), @(152, 103, 216, 103), @(81, 54, 81, 103), @(179, 103, 179, 152), @(40, 153, 104, 153), @(153, 153, 216, 153), @(76, 153, 76, 207))) {
        Draw-Line $i $ink 6 $seam[0] $seam[1] $seam[2] $seam[3]
    }
    Draw-GlowLine $i $red 13 @(151, 36, 116, 103, 145, 102, 111, 162, 137, 158, 105, 216) $pale
}

function Draw-InspectIcon {
    $i = 17
    $eye = @(27, 128, 57, 84, 92, 55, 128, 44, 164, 55, 199, 84, 229, 128, 199, 172, 164, 201, 128, 212, 92, 201, 57, 172)
    Fill-Polygon $i $deep $eye
    Stroke-Polygon $i $ink 15 $eye
    Stroke-Polygon $i $teal 7 $eye
    Fill-Ellipse $i $gold 82 82 92 92
    Stroke-Ellipse $i $ink 10 82 82 92 92
    Fill-Ellipse $i $ink 105 105 46 46
    Fill-Ellipse $i $pale 114 111 13 13
}

function Draw-SelectedIcon {
    $i = 18
    Draw-Diamond $i ([System.Drawing.Color]::FromArgb(220, $tealDark.R, $tealDark.G, $tealDark.B)) $ink 128 128 90 90 16
    Stroke-Polygon $i $teal 8 @(128, 38, 218, 128, 128, 218, 38, 128)
    Draw-Diamond $i $deep $gold 128 128 52 52 7
    Draw-Star $i $pale $teal 128 128 31 12 8 5
    foreach ($spark in @(@(128, 27, 128, 48), @(128, 208, 128, 229), @(27, 128, 48, 128), @(208, 128, 229, 128))) {
        Draw-Line $i $teal 7 $spark[0] $spark[1] $spark[2] $spark[3]
    }
}

function Draw-DangerIcon {
    $i = 19
    $crystal = @(128, 31, 210, 94, 191, 188, 128, 224, 65, 188, 46, 94)
    Fill-Polygon $i $redDark $crystal
    Stroke-Polygon $i $ink 17 $crystal
    Stroke-Polygon $i $red 8 $crystal
    Draw-Lines $i $orange 7 @(128, 43, 128, 210)
    Draw-Lines $i $orange 7 @(57, 96, 128, 128, 199, 96)
    Draw-Lines $i $gold 6 @(72, 181, 128, 128, 184, 181)
    Draw-Star $i $pale $orange 128 128 29 11 4 5
    Draw-Star $i $red $ink 218 53 11 5 4 4
    Draw-Star $i $orange $ink 38 58 10 4 4 4
}

function Measure-Atlas {
    param([System.Drawing.Bitmap]$Bitmap)

    $measurements = @()
    $boundaryAlphaPixels = 0
    for ($index = 0; $index -lt ($columns * $rows); $index++) {
        $column = $index % $columns
        $row = [Math]::Floor($index / $columns)
        $minX = $cellSize
        $minY = $cellSize
        $maxX = -1
        $maxY = -1
        $visible = 0
        for ($localY = 0; $localY -lt $cellSize; $localY++) {
            for ($localX = 0; $localX -lt $cellSize; $localX++) {
                $alpha = $Bitmap.GetPixel($column * $cellSize + $localX, $row * $cellSize + $localY).A
                if ($alpha -eq 0) { continue }
                $visible++
                $minX = [Math]::Min($minX, $localX)
                $minY = [Math]::Min($minY, $localY)
                $maxX = [Math]::Max($maxX, $localX)
                $maxY = [Math]::Max($maxY, $localY)
                if ($localX -eq 0 -or $localY -eq 0 -or $localX -eq ($cellSize - 1) -or $localY -eq ($cellSize - 1)) {
                    $boundaryAlphaPixels++
                }
            }
        }
        if ($visible -eq 0) {
            throw "Atlas cell $index is empty."
        }
        $gutter = [Math]::Min([Math]::Min($minX, $minY), [Math]::Min(($cellSize - 1) - $maxX, ($cellSize - 1) - $maxY))
        $measurements += [pscustomobject]@{
            Index = $index
            Bounds = "$minX,$minY-$maxX,$maxY"
            MinimumGutter = $gutter
            VisiblePixels = $visible
        }
    }
    if ($boundaryAlphaPixels -ne 0) {
        throw "Atlas has $boundaryAlphaPixels nontransparent cell-boundary pixels."
    }
    $minimumGutter = ($measurements | Measure-Object -Property MinimumGutter -Minimum).Minimum
    if ($minimumGutter -lt 18) {
        $violations = ($measurements | Where-Object { $_.MinimumGutter -lt 18 } | ForEach-Object { "cell $($_.Index)=$($_.MinimumGutter) px ($($_.Bounds))" }) -join "; "
        throw "Atlas minimum transparent gutter is $minimumGutter px; expected at least 18 px. Violations: $violations"
    }
    return [pscustomobject]@{
        Width = $Bitmap.Width
        Height = $Bitmap.Height
        PixelFormat = $Bitmap.PixelFormat
        BoundaryAlphaPixels = $boundaryAlphaPixels
        MinimumGutter = $minimumGutter
        Cells = $measurements
    }
}

$highResolution = [System.Drawing.Bitmap]::new(
    $logicalWidth * $supersample,
    $logicalHeight * $supersample,
    [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
try {
    $script:graphics = [System.Drawing.Graphics]::FromImage($highResolution)
    try {
        $script:graphics.Clear([System.Drawing.Color]::Transparent)
        $script:graphics.CompositingMode = [System.Drawing.Drawing2D.CompositingMode]::SourceOver
        $script:graphics.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
        $script:graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
        $script:graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
        $script:graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
        $script:graphics.ScaleTransform($supersample, $supersample)

        Draw-MoveIcon
        Draw-AttackIcon
        Draw-SpellIcon
        Draw-GuardIcon
        Draw-ElixirIcon
        Draw-EndTurnIcon
        Draw-RangedIcon
        Draw-SkillIcon
        Draw-TargetingIcon
        Draw-BlockedIcon
        Draw-FireIcon
        Draw-MultiIcon
        Draw-HealIcon
        Draw-StealthIcon
        Draw-DashIcon
        Draw-VolleyIcon
        Draw-BreakIcon
        Draw-InspectIcon
        Draw-SelectedIcon
        Draw-DangerIcon
    }
    finally {
        $script:graphics.Dispose()
        $script:graphics = $null
    }

    $atlas = [System.Drawing.Bitmap]::new($logicalWidth, $logicalHeight, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    try {
        $resize = [System.Drawing.Graphics]::FromImage($atlas)
        try {
            $resize.Clear([System.Drawing.Color]::Transparent)
            $resize.CompositingMode = [System.Drawing.Drawing2D.CompositingMode]::SourceCopy
            $resize.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
            $resize.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
            $resize.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
            $resize.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
            $resize.DrawImage($highResolution, 0, 0, $logicalWidth, $logicalHeight)
        }
        finally {
            $resize.Dispose()
        }

        $outputDirectory = Split-Path -Parent $OutputPath
        if (-not (Test-Path -LiteralPath $outputDirectory)) {
            New-Item -ItemType Directory -Path $outputDirectory | Out-Null
        }
        $atlas.Save($OutputPath, [System.Drawing.Imaging.ImageFormat]::Png)
        $validation = Measure-Atlas $atlas
        Get-Item -LiteralPath $OutputPath
        $validation | Select-Object Width, Height, PixelFormat, BoundaryAlphaPixels, MinimumGutter
        $validation.Cells | Format-Table -AutoSize
    }
    finally {
        $atlas.Dispose()
    }
}
finally {
    $highResolution.Dispose()
}
