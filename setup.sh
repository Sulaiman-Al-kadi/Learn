#!/usr/bin/env bash
# Sets up Learn Lab on macOS or Linux: checks the tools, installs the extension, opens the course.
# Run from anywhere: bash setup.sh   (NO_OPEN=1 skips opening VS Code)
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
missing=()

if command -v dotnet >/dev/null 2>&1; then
  if ! dotnet --list-sdks | grep -qE '^(1[0-9]|[2-9][0-9])\.'; then
    missing+=(".NET 10 SDK (found an older version): https://dotnet.microsoft.com/download/dotnet/10.0")
  fi
else
  missing+=(".NET 10 SDK: https://dotnet.microsoft.com/download/dotnet/10.0")
fi
command -v node >/dev/null 2>&1 || missing+=("Node.js 18 or newer: https://nodejs.org")
command -v code >/dev/null 2>&1 || missing+=("VS Code: https://code.visualstudio.com (then run 'Shell Command: Install code command in PATH')")

if [ "${#missing[@]}" -gt 0 ]; then
  echo "Install these, open a new terminal, and run this script again:"
  printf '  - %s\n' "${missing[@]}"
  exit 1
fi

echo "Building and installing the Learn Lab extension. This takes about a minute..."
(cd "$root/extension" && npm run setup --silent)

echo "Done. In VS Code, click the graduation-cap icon in the left bar."
if [ -z "${NO_OPEN:-}" ]; then code "$root"; fi
