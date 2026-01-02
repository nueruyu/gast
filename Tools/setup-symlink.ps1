$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location (Join-Path $scriptDir "..")

$source = "External\descrio-dotnet\Descrio.Unity\Packages\com.nueruyu.descrio"
$target = "Packages\com.nueruyu.descrio"

if (-not (Test-Path "Packages")) {
    Write-Host "Creating Packages directory..."
    New-Item -ItemType Directory -Path "Packages" | Out-Null
}

if (Test-Path $target) {
    Write-Host "Removing existing link/directory at $target..."
    Remove-Item -Path $target -Force -Recurse
}

if (-not (Test-Path $source)) {
    Write-Error "Source directory not found: $source"
    exit 1
}

Write-Host "Creating symbolic link..."
Write-Host "  Source: $source"
Write-Host "  Target: $target"

try {
    New-Item -ItemType SymbolicLink -Path $target -Target $source | Out-Null
    Write-Host "✓ Symbolic link created successfully!" -ForegroundColor Green
} catch {
    Write-Error "Failed to create symbolic link. You may need to run PowerShell as Administrator."
    Write-Host "`nAlternatively, enable Developer Mode in Windows Settings to create symlinks without admin rights." -ForegroundColor Yellow
    exit 1
}
