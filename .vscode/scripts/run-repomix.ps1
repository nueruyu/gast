# SCRIPT DESCRIPTION:
# This script runs repomix with a specified config file and copies the output to the clipboard.
# It is designed to be called from a VS Code task.

# Acknowledge the config file path passed as an argument.
param (
    [Parameter(Mandatory=$true)]
    [string]$ConfigPath
)

# Determine the project's root directory based on the script's location.
# Assumes this script is in a subdirectory of .vscode (e.g., .vscode/scripts).
$projectRoot = Split-Path -Path $PSScriptRoot -Parent | Split-Path -Parent

Write-Host "--------------------------------------------------"
Write-Host "Starting Repomix Task..."
Write-Host "Using config file: $ConfigPath"

# STEP 1: Read the JSON config file to find the output file name.
try {
    $config = Get-Content -Path $ConfigPath -Raw | ConvertFrom-Json
    $outputFileName = $config.output.filePath
    
    if (-not $outputFileName) {
        Write-Error "ERROR: 'output.file' not found in the config file."
        exit 1
    }
    
    $outputFilePath = Join-Path -Path $projectRoot -ChildPath $outputFileName
}
catch {
    Write-Error "ERROR: Failed to read or parse the config file. Check the path and JSON format."
    Write-Error $_.Exception.Message
    exit 1
}

Write-Host "Output file will be: $outputFileName"

# STEP 2: Execute the repomix command.
Write-Host "Running repomix..."
# Change directory to project root to ensure correct relative path resolution.
Push-Location -Path $projectRoot
npx repomix --config $ConfigPath
Pop-Location

# Check if the command executed successfully.
if ($LASTEXITCODE -ne 0) {
    Write-Error "ERROR: repomix command failed."
    exit 1
}

Write-Host "Repomix execution finished."

# STEP 3: Copy the output file to the clipboard.
if (Test-Path $outputFilePath) {
    Write-Host "Copying output file to clipboard..."
    Set-Clipboard -Path $outputFilePath
    Write-Host "SUCCESS: Output file '$outputFileName' has been copied to the clipboard."
    Write-Host "--------------------------------------------------"
}
else {
    Write-Error "ERROR: Output file not found at: $outputFilePath"
    exit 1
}