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
if (Test-Tool 'node') {
    if ([version]((& node --version).TrimStart('v')) -lt [version]'20.18.0') { $missing += 'Node.js 20.18 or newer (packaging crashes on older versions): https://nodejs.org' }
} else {
    $missing += 'Node.js 20.18 or newer: https://nodejs.org'
}
if (-not (Test-Tool 'code')) { $missing += "VS Code: https://code.visualstudio.com (if it's installed, reinstall with 'Add to PATH' ticked)" }

# EF Core and ASP.NET Core exercises hit the 260-character path limit from deep folders.
if ($root.Length -gt 75) { $missing += "A shorter folder path. This one is $($root.Length) characters and must be under 75, so clone somewhere like C:\src\Learn" }

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

Write-Host 'Done. If VS Code asks whether you trust this folder, choose Yes, then click the graduation-cap icon in the left bar.' -ForegroundColor Green
if (-not $NoOpen) { & code $root }
