# Setup — running this after cloning/forking

The repo only tracks source (course content + extension `src/`). Build artifacts, dependencies, and the packaged extension are gitignored on purpose, so you need to build them once after cloning.

## Prerequisites

| Tool | Check you have it | Get it |
|---|---|---|
| **.NET SDK 10** | `dotnet --version` → `10.x` | https://dotnet.microsoft.com/download |
| **Node.js** (20.18+; packaging crashes on 18) | `node --version` | https://nodejs.org |
| **VS Code** with its `code` command on PATH | `code --version` (first line is a VS Code version, e.g. `1.10x.x`) | https://code.visualstudio.com |
| **C# Dev Kit** extension (recommended, not required) | — | install from the VS Code Marketplace |

An internet connection is needed the *first* time you check a `test`-mode exercise (NuGet restores xUnit / EF Core / ASP.NET Core testing packages) — after that they're cached locally (`~/.nuget/packages`) and work offline.

## Build and install the extension

```powershell
git clone https://github.com/Sulaiman-Al-kadi/Learn.git
cd Learn/extension
npm run setup
```

That one command does everything: `npm install` (dependencies), `npm run build` (bundles `src/` → `dist/extension.js`), `npm run package` (produces `learn-lab.vsix`), then installs it into VS Code (`code --install-extension ... --force`).

Then in VS Code: `Ctrl+Shift+P` → **Developer: Reload Window**.

> **Have Cursor, VS Code Insiders, or VSCodium installed too?** They can put their own `code` command on PATH, so the install silently lands in *that* editor. Check with `where code` (Windows) / `which code` (macOS/Linux). If it isn't VS Code's, install the package from inside VS Code instead: Extensions view → `···` menu → **Install from VSIX…** → pick `extension/learn-lab.vsix`.

*(Prefer the manual steps? `npm install && npm run build && npm run package && npm run install-extension` — same thing, one command at a time.)*

## Open the course

```powershell
code C:\path\to\Learn      # open the REPO ROOT, not the extension/ subfolder
```

When VS Code asks whether you trust the authors of the files in this folder, choose **Yes, I trust the authors** — Learn Lab runs `dotnet` on the exercise code, so VS Code keeps it disabled in Restricted Mode.

Click the graduation-cap icon in the Activity Bar (left sidebar). If it's not there, reload the window again — the extension only activates once it detects a `course.json` file inside the open folder (`workspaceContains:**/course.json`).

## Verify it actually works

Pick any exercise in the sidebar → **Exercise** → press the beaker icon (🧪) or `Ctrl+Alt+Enter`. You should see a pass/fail result in a few seconds (longer — up to ~10s — the very first time, while NuGet restores).

## After changing extension source (`extension/src/*.ts`)

Re-run `npm run setup` (from `extension/`), then reload the window again — VS Code doesn't hot-reload a packaged extension.

## Common issues

| Symptom | Fix |
|---|---|
| No graduation-cap icon | `Developer: Reload Window`; confirm you opened the repo root, not a subfolder |
| No icon, and the status bar says **Restricted Mode** | Click it → **Trust** (see *Open the course*) |
| `npm run setup` said "successfully installed" but VS Code has no Learn Lab | `code` on your PATH belongs to another editor — use **Install from VSIX…** (see the note under *Build and install*) |
| `npm run package` fails with `ReferenceError: File is not defined` | Node.js is too old — install Node 20.18 or newer |
| `dotnet: command not found` | Install the .NET 10 SDK, restart your terminal/VS Code |
| First exercise check takes forever / times out | Just NuGet restoring — check your internet connection, then retry |
| Check says *"exceeds the OS max path limit"* or *"The filename or extension is too long"* (EF Core / ASP.NET Core exercises) | Windows' 260-character path limit — the repo folder path must be under ~75 characters. Clone somewhere short, e.g. `C:\src\Learn` |
| Extension doesn't reflect a source change | You edited `.ts` but didn't rebuild/repackage/reinstall — see above |
