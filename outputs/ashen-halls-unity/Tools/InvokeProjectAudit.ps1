param(
    [ValidateSet('Rules', 'Full')]
    [string]$Suite = 'Full',
    [string]$UnityExe = 'C:\Program Files\Unity\Hub\Editor\6000.3.18f1\Editor\Unity.exe',
    [ValidateRange(1, 120)]
    [int]$TimeoutMinutes = 30
)

$ErrorActionPreference = 'Stop'
$projectRoot = [System.IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))
if (-not (Test-Path -LiteralPath $UnityExe -PathType Leaf)) {
    throw "Unity Editor is missing: $UnityExe"
}
if (-not (Test-Path -LiteralPath (Join-Path $projectRoot 'ProjectSettings\ProjectVersion.txt'))) {
    throw "The script must run from the canonical Unity project's Tools folder. Resolved: $projectRoot"
}

$runId = [DateTime]::UtcNow.ToString('yyyyMMdd-HHmmss') + '-' + [Guid]::NewGuid().ToString('N').Substring(0, 8)
$auditRoot = Join-Path $projectRoot ('QA\project-audit\' + $runId)
New-Item -ItemType Directory -Path $auditRoot -Force | Out-Null
$logPath = Join-Path $auditRoot 'unity.log'
$method = 'AshenHalls.Editor.ProjectAuditSmoke.Run' + $Suite
$arguments = '-batchmode -nographics -quit -projectPath "' + $projectRoot +
    '" -executeMethod ' + $method + ' -logFile "' + $logPath + '"'

Write-Host "Running $Suite project audit. Log: $logPath"
# Wait on this exact editor process. Start-Process -Wait also waits for long-lived
# licensing descendants and can keep the shell blocked after Unity has exited.
$auditProcess = Start-Process -FilePath $UnityExe -ArgumentList $arguments -PassThru -WindowStyle Hidden
$deadline = [DateTime]::UtcNow.AddMinutes($TimeoutMinutes)
$nextProgress = [DateTime]::UtcNow.AddSeconds(30)
while (-not $auditProcess.WaitForExit(1000)) {
    if ([DateTime]::UtcNow -ge $deadline) {
        $auditProcess.Kill()
        $auditProcess.WaitForExit()
        throw "Project audit exceeded $TimeoutMinutes minutes. The launched editor was stopped. See $logPath"
    }
    if ([DateTime]::UtcNow -ge $nextProgress) {
        Write-Host "Project audit is still running. Log: $logPath"
        $nextProgress = [DateTime]::UtcNow.AddSeconds(30)
    }
}
$auditExitCode = $auditProcess.ExitCode
if (-not (Test-Path -LiteralPath $logPath -PathType Leaf)) {
    throw "Unity exited with code $auditExitCode without producing the audit log: $logPath"
}
$logText = Get-Content -LiteralPath $logPath -Raw
if ($auditExitCode -ne 0) {
    throw "Unity audit exited with code $auditExitCode. See $logPath"
}
if ($logText -notmatch ('(?m)^PROJECT AUDIT PASSED: ' + [regex]::Escape($Suite) + '\s*$')) {
    throw "Unity exited without the expected $Suite audit success marker. See $logPath"
}
if ($logText -match 'PROJECT AUDIT FAILED:|Scripts have compiler errors|error CS\d+|No valid Unity Editor license found') {
    throw "Unity log contains a failure despite its exit code. See $logPath"
}
Write-Host "Project audit passed: $Suite. Log: $logPath"
