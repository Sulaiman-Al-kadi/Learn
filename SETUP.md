# Setup — running this after cloning/forking

The repo only tracks source (course content + extension `src/`). Build artifacts, dependencies, and the packaged extension are gitignored on purpose, so you need to build them once after cloning.

## Prerequisites

| Tool | Check you have it | Get it |
|---|---|---|
| **.NET SDK 10** | `dotnet --version` → `10.x` | https://dotnet.microsoft.com/download |
| **Node.js** (18+) | `node --version` | https://nodejs.org |
| **VS Code** | — | https://code.visualstudio.com |
| **C# Dev Kit** extension (recommended, not required) | — | install from the VS Code Marketplace |

An internet connection is needed the *first* time you check a `test`-mode exercise (NuGet restores xUnit / EF Core / ASP.NET Core testing packages) — after that they're cached locally (`~/.nuget/packages`) and work offline.

## Build and install the extension

```powershell
git clone https://github.com/Sulaiman-Al-kadi/Learn.git
cd Learn/extension
npm run setup
```

That one command does everything: `npm install` (dependencies), `npm run build` (bundles `src/` → `dist/extension.js`), `npm run package` (produces `learn-lab-0.1.0.vsix`), then installs it into VS Code (`code --install-extension ... --force`).

Then in VS Code: `Ctrl+Shift+P` → **Developer: Reload Window**.

*(Prefer the manual steps? `npm install && npm run build && npm run package && npm run install-extension` — same thing, one command at a time.)*

## Open the course

```powershell
code C:\path\to\Learn      # open the REPO ROOT, not the extension/ subfolder
```

Click the graduation-cap icon in the Activity Bar (left sidebar). If it's not there, reload the window again — the extension only activates once it detects a `course.json` file inside the open folder (`workspaceContains:**/course.json`).

## Verify it actually works

Pick any exercise in the sidebar → **Exercise** → press the beaker icon (🧪) or `Ctrl+Alt+Enter`. You should see a pass/fail result in a few seconds (longer — up to ~10s — the very first time, while NuGet restores).

## After changing extension source (`extension/src/*.ts`)

Re-run `npm run setup` (from `extension/`), then reload the window again — VS Code doesn't hot-reload a packaged extension.

## Common issues

| Symptom | Fix |
|---|---|
| No graduation-cap icon | `Developer: Reload Window`; confirm you opened the repo root, not a subfolder |
| `dotnet: command not found` | Install the .NET 10 SDK, restart your terminal/VS Code |
| First exercise check takes forever / times out | Just NuGet restoring — check your internet connection, then retry |
| Extension doesn't reflect a source change | You edited `.ts` but didn't rebuild/repackage/reinstall — see above |
