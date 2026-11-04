# Sets up Learn Lab on Windows: checks the tools, installs the extension, opens the course.
# Run from the repo folder: powershell -ExecutionPolicy Bypass -File setup.ps1
param([switch]$NoOpen)

$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$missing = @()

function Test-Tool($name) { $null -ne (Get-Command $name -ErrorAction SilentlyContinue) }

if (Test-Tool 'dotnet') {
    $sdks = & dotnet --list-sdks
    if (-not ($sdks | Where-Object { [int]($_ -split '\.')[0] -ge 10 })) { $missing += '.NET 10 SDK (found an older version): https://dotnet.microsoft.com/download/dotnet/10.0' }
} else {
    $missing += '.NET 10 SDK: https://dotnet.microsoft.com/download/dotnet/10.0'
}
if (-not (Test-Tool 'node')) { $missing += 'Node.js 18 or newer: https://nodejs.org' }
if (-not (Test-Tool 'code')) { $missing += "VS Code: https://code.visualstudio.com (if it's installed, reinstall with 'Add to PATH' ticked)" }

if ($missing.Count -gt 0) {
    Write-Host 'Install these, open a new terminal, and run this script again:' -ForegroundColor Yellow
    $missing | ForEach-Object { Write-Host "  - $_" }
    exit 1
}

Write-Host 'Building and installing the Learn Lab extension. This takes about a minute...'
Push-Location (Join-Path $root 'extension')
try {
    # npm and code are .cmd files on Windows, so a failure shows up in the exit code.
    & npm run setup --silent
    if ($LASTEXITCODE -ne 0) { throw 'The extension build failed. The output above has the reason.' }
} finally {
    Pop-Location
}

Write-Host 'Done. In VS Code, click the graduation-cap icon in the left bar.' -ForegroundColor Green
if (-not $NoOpen) { & code $root }
