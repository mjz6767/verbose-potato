$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'UnityEditorResolution.ps1')
$checks = 0
function Assert-Rejected {
    param([scriptblock]$Action, [string]$ExpectedMessage)
    try { & $Action; throw 'Expected rejection did not occur.' }
    catch {
        if ($_.Exception.Message -notlike ('*' + $ExpectedMessage + '*')) { throw }
    }
    $script:checks++
}

$requirement = Get-UnityProjectRequirement -VersionText "m_EditorVersion: 6000.3.18f1`nm_EditorVersionWithRevision: 6000.3.18f1 (5ebeb53e4c07)`n"
Assert-UnityEditorProductVersion -Requirement $requirement -ProductVersion '6000.3.18f1_5ebeb53e4c07'
$checks++
Assert-Rejected { Assert-UnityEditorProductVersion -Requirement $requirement -ProductVersion '6000.3.21f1_c02631ffc030' } 'editor mismatch'
Assert-Rejected { Assert-UnityEditorProductVersion -Requirement $requirement -ProductVersion '6000.3.18f1_000000000000' } 'editor mismatch'
Assert-Rejected { Get-UnityProjectRequirement -VersionText 'm_EditorVersion: ../unexpected' } 'valid m_EditorVersion'
Assert-Rejected { Get-UnityProjectRequirement -VersionText "m_EditorVersion: 6000.3.18f1`nm_EditorVersion: 6000.3.18f1" } 'exactly one'
Assert-Rejected { Get-UnityProjectRequirement -VersionText "m_EditorVersion: 6000.3.18f1`nm_EditorVersionWithRevision: 6000.3.21f1 (c02631ffc030)" } 'matching editor revision'
Assert-Rejected { Get-UnityProjectRequirement -VersionText 'm_EditorVersion: 6000.3.18f1' } 'matching editor revision'
Assert-Rejected { Get-UnityProjectRequirement -VersionText "m_EditorVersion: 6000.3.18f1`nm_EditorVersion: garbage`nm_EditorVersionWithRevision: 6000.3.18f1 (5ebeb53e4c07)" } 'exactly one'
Assert-Rejected { Get-UnityProjectRequirement -VersionText "m_EditorVersion: 6000.3.18f1`nm_EditorVersionWithRevision: 6000.3.18f1 (5ebeb53e4c07)`nm_EditorVersionWithRevision: garbage" } 'matching editor revision'
Assert-Rejected { Get-UnityProjectRequirement -VersionText "m_EditorVersion:`n6000.3.18f1`nm_EditorVersionWithRevision: 6000.3.18f1 (5ebeb53e4c07)" } 'valid m_EditorVersion'
$projectRoot = Split-Path -Parent $PSScriptRoot
Assert-Rejected { Resolve-ProjectUnityEditor -ProjectRoot $projectRoot -UnityExe (Join-Path $projectRoot 'QA/nonexistent-editor/Unity.exe') } 'No fallback editor'
$resolved = Resolve-ProjectUnityEditor -ProjectRoot $projectRoot
$explicit = Resolve-ProjectUnityEditor -ProjectRoot $projectRoot -UnityExe $resolved
if ($resolved -ne $explicit) { throw 'Default and explicit matching editor resolution differ.' }
$checks++
$fixtureRoot = 'C:/UnityEditors/6000.3.18f1/Editor'
$fixtureLines = @(
    '-define:UNITY_6000_3_18',
    '-r:"C:/UnityEditors/6000.3.18f1/Editor/Data/Managed/UnityEngine/UnityEngine.CoreModule.dll"',
    '-r:"C:/UnityEditors/6000.3.18f1/Editor/Data/Managed/UnityEngine/UnityEditor.CoreModule.dll"',
    '-analyzer:"C:/UnityEditors/6000.3.18f1/Editor/Data/Tools/BuildPipeline/Unity.SourceGenerators.dll"',
    '-r:"Library/Bee/artifacts/example/UnityEngine.UI.ref.dll"'
)
Assert-UnityResponseFileEditor -Requirement $requirement -UnityEditorRoot $fixtureRoot -Lines $fixtureLines
$checks++
$windowsLines = @($fixtureLines | ForEach-Object { $_.Replace('/', '\') })
Assert-UnityResponseFileEditor -Requirement $requirement -UnityEditorRoot $fixtureRoot.ToUpperInvariant() -Lines $windowsLines
$checks++
Assert-Rejected { Assert-UnityResponseFileEditor -Requirement $requirement -UnityEditorRoot $fixtureRoot -Lines @($fixtureLines -replace 'UNITY_6000_3_18', 'UNITY_6000_3_21') } 'patch define'
Assert-Rejected { Assert-UnityResponseFileEditor -Requirement $requirement -UnityEditorRoot $fixtureRoot -Lines @($fixtureLines + '-define:UNITY_6000_3_21') } 'patch define'
Assert-Rejected { Assert-UnityResponseFileEditor -Requirement $requirement -UnityEditorRoot $fixtureRoot -Lines @($fixtureLines -replace '/6000.3.18f1/', '/6000.3.21f1/') } 'outside the selected editor'
$staleAnalyzer = @($fixtureLines | ForEach-Object { if ($_ -match '^-analyzer:') { $_.Replace('/6000.3.18f1/', '/6000.3.21f1/') } else { $_ } })
Assert-Rejected { Assert-UnityResponseFileEditor -Requirement $requirement -UnityEditorRoot $fixtureRoot -Lines $staleAnalyzer } 'outside the selected editor'
Assert-Rejected { Assert-UnityResponseFileEditor -Requirement $requirement -UnityEditorRoot $fixtureRoot -Lines @($fixtureLines + '-r:"C:/UnityEditors/6000.3.21f1/Editor/Data/NetStandard/ref/netstandard.dll"') } 'outside the selected editor'
Assert-Rejected { Assert-UnityResponseFileEditor -Requirement $requirement -UnityEditorRoot $fixtureRoot -Lines @($fixtureLines + '-r:"C:/UnityEditors/6000.3.18f1/Editor-Other/Data/Managed/UnityEngine.AudioModule.dll"') } 'outside the selected editor'
foreach ($core in @('UnityEngine.CoreModule.dll', 'UnityEditor.CoreModule.dll')) {
    $missingCore = @($fixtureLines | Where-Object { -not $_.Contains($core) })
    Assert-Rejected { Assert-UnityResponseFileEditor -Requirement $requirement -UnityEditorRoot $fixtureRoot -Lines $missingCore } 'missing'
}
$projectRequirement = Get-UnityProjectRequirement -VersionText (Get-Content -LiteralPath (Join-Path $projectRoot 'ProjectSettings/ProjectVersion.txt') -Raw)
Push-Location $projectRoot
try {
    foreach ($assembly in @('Assembly-CSharp', 'Assembly-CSharp-Editor')) {
        $cache = Join-Path $projectRoot ('Library/Bee/artifacts/1900b0aE.dag/' + $assembly + '.rsp')
        Assert-UnityResponseFileEditor -Requirement $projectRequirement -UnityEditorRoot (Split-Path -Parent $resolved) `
            -Lines @(Get-Content -LiteralPath $cache) -ResponseName $cache
        $checks++
    }
} finally { Pop-Location }
Write-Host "Unity editor resolution checks passed: $checks. Editor: $resolved"
