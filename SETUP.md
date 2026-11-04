# Setup

There are two ways to run this course. Use your own copy of the repo if you want to keep your answers: click **Fork** on GitHub, then use your fork below.

## In the browser (nothing to install)

1. On the repo page, click **Code → Codespaces → Create codespace on main**.
2. Wait a few minutes for the first build.
3. Click the graduation-cap icon in the left bar. If it isn't there yet, run **Developer: Reload Window** from the Command Palette (`Ctrl+Shift+P`).

Codespaces has a free monthly allowance on personal GitHub accounts. Stop the codespace when you finish a session so that it doesn't use up your hours.

## On your computer

You need:

| Tool | Check with | Get it |
|---|---|---|
| **.NET 10 SDK** | `dotnet --version` shows `10.x` | https://dotnet.microsoft.com/download/dotnet/10.0 |
| **Node.js 20.18+** | `node --version` | https://nodejs.org |
| **VS Code** | `code --version` | https://code.visualstudio.com |

Then clone the repo and run the setup script from its folder.

**Windows:**

```powershell
git clone https://github.com/Sulaiman-Al-kadi/Learn.git
cd Learn
powershell -ExecutionPolicy Bypass -File setup.ps1
```

**macOS or Linux:**

```bash
git clone https://github.com/Sulaiman-Al-kadi/Learn.git
cd Learn
bash setup.sh
```

The script checks the tools, builds and installs the Learn Lab extension, and opens the folder in VS Code. When VS Code asks whether you trust the authors of the files in this folder, choose **Yes, I trust the authors**: Learn Lab runs `dotnet` on the exercise code, so VS Code keeps it disabled in Restricted Mode. Learn Lab then asks to start the first lesson. VS Code also offers to install C# Dev Kit, which gives you code completion and red underlines while you type.

## Checking that it works

Open the first exercise and press **Check my code**. An unsolved exercise shows a failure message within a few seconds, and that means the checker is working.

The first check of a unit-test exercise needs the internet, because NuGet downloads the test packages. After that they are cached and work offline.

## Keeping your work

- Your answers are normal files in the exercise folders. Commit them to your fork to keep them.
- Your progress is in `.learn/progress.json`, which git ignores. Copy that file if you move to another computer.
- Before pulling course updates, commit your answers. If a pull conflicts with an exercise you edited, keep your version.

## Getting help from a tutor

Open [Claude Code](https://claude.com/claude-code) in this folder and ask about a lesson or an exercise, in any language. `CLAUDE.md` tells it to act as a tutor: it reads your exercise and your last Check result, and gives hints instead of solutions unless you ask for the answer.

## Common problems

| Problem | Fix |
|---|---|
| No graduation-cap icon | Run **Developer: Reload Window**, and make sure you opened the repo folder itself, not `extension/` |
| No icon, and the status bar says **Restricted Mode** | Click it, then choose **Trust** |
| Setup says "successfully installed", but VS Code has no Learn Lab | Cursor, VS Code Insiders, or VSCodium may own the `code` command. Check with `where code` (Windows) or `which code` (macOS/Linux). In VS Code, open the Extensions view, then `···` → **Install from VSIX…**, and pick `extension/learn-lab.vsix` |
| `ReferenceError: File is not defined` while packaging | Node.js is too old. Install 20.18 or newer |
| Check says "exceeds the OS max path limit" or "The filename or extension is too long" | Windows' 260-character path limit. Clone somewhere short, such as `C:\src\Learn` |
| The script says `code` is missing | In VS Code, run **Shell Command: Install 'code' command in PATH**. On Windows, reinstall with **Add to PATH** ticked |
| The script says a tool is missing right after installing it | Open a new terminal so that it sees the updated PATH |
| "Could not download the NuGet packages" | Check your internet connection and try again |
| Lots of red underlines in an exercise | Wait a minute after opening the folder while C# Dev Kit loads the projects |

## Changing the extension

After editing `extension/src/*.ts`, run `npm run setup` in `extension/`, then reload the VS Code window. VS Code doesn't hot-reload a packaged extension.
