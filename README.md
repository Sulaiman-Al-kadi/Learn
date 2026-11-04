# Learn Lab — learn C# & .NET inside VS Code

A personal learning system: lessons, quizzes, and exercises that are **checked automatically**.

## Start here

| | How | You need |
|---|---|---|
| **In the browser** | [Open in GitHub Codespaces](https://codespaces.new/Sulaiman-Al-kadi/Learn) | A GitHub account |
| **On Windows** | Run `powershell -ExecutionPolicy Bypass -File setup.ps1` in the repo folder | .NET 10 SDK, Node.js, VS Code |
| **On macOS or Linux** | Run `bash setup.sh` in the repo folder | .NET 10 SDK, Node.js, VS Code |

The setup script checks those tools, tells you what is missing, installs the extension, and opens the course. [SETUP.md](SETUP.md) has the details and fixes for common problems.

## The curriculum

**10 modules, 42 lessons, 157 checkable items — from `Console.WriteLine` to a tested, secured, containerized .NET backend.** Full concept-by-concept map: **[ROADMAP.md](ROADMAP.md)**.

| Module | Topic |
|---|---|
| 1 | C# Basics — printing through strings, including how memory works, 10 lessons |
| 2 | OOP & SOLID — classes through unit testing, capstone: Library System |
| 3 | Collections & LINQ, capstone: Log Analyzer |
| 4 | Async, Files, JSON & HTTP, capstone: WeatherClient |
| 5 | ASP.NET Core Web Fundamentals, capstone: StudentsApi v1 |
| 6 | EF Core, capstone: StudentsApi v2 (SQLite) |
| 7 | Minimal APIs & Security (JWT/CORS), capstone: secured TaskApi |
| 8 | Advanced — Channels, caching, config, structured logging |
| 9 | Testing, Docker & DevOps (Docker/CI are guided labs) |
| 10 | Your own capstone project |

## Open it
1. Open the repo folder in VS Code (the setup script does this for you).
2. Click the **graduation-cap icon** in the left Activity Bar → the **Learn Lab** panel shows the course tree.
3. Click **Continue** at the top of the panel. It opens your next step: a lesson, a quiz, or an exercise. Every lesson ends with a button that leads to the next step, and exercises have **Check my code** (or `Ctrl+Alt+Enter`).
4. Click **Review** now and then. It asks 5 questions from earlier lessons, and the ones you miss come back sooner.

A quiz passes at 80%, and you can retry just the questions you missed.

## Sidebar buttons
| Button | Does |
|---|---|
| 🧪 beaker | **Check** the current exercise (runs your code, compares output or runs tests) |
| 💡 lightbulb | Reveal the **next hint** (hints are progressive; using them is fine) |
| → arrow | Jump to the **next unfinished** item |
| ↻ | Refresh the course tree (after editing content) |
| right-click an exercise | **Reset** to starter code · **Show Solution** (diff against your code) |

Progress is saved in `.learn/progress.json`. `Learn: Reset All Progress` clears it.

## Asking for help

Open Claude Code in this folder and ask in Arabic or English. `CLAUDE.md` tells it to act as a tutor: it reads your exercise and your last Check result (`.learn/last-check.md`), and gives hints instead of solutions unless you ask for the answer.

## Layout
```
courses/csharp-dotnet/          ← the course (add more courses or languages here)
  course.json
  01-basics/
    01-printing/
      lesson.md                 ← explanation
      quiz.json                 ← questions (mcq / multi / tf)
      exercise/
        README.md               ← the task
        Program.cs              ← YOU edit this
        expected.txt | cases/   ← what the checker compares against
        input.txt               ← fed to your program as if typed
        hints.md                ← "## Hint 1", "## Hint 2", ...
        starter/ solution/      ← for Reset / Show Solution
      exercise-2/, exercise-3/  ← extra practice, same layout
extension/                      ← the VS Code extension source (TypeScript)
```

A web-app exercise (Module 5+) looks slightly different — the app lives in an `App/` subfolder (its own `Sdk="Microsoft.NET.Sdk.Web"` project) so `WebApplicationFactory` can find it, with `Tests.csproj` + `Tests.cs` at the exercise root:
```
exercise/
  App/App.csproj, App/Program.cs, App/*.cs   ← the web app (student file(s) live here)
  Tests.csproj, Tests.cs                      ← ProjectReference to App/, runs via `dotnet test`
  starter/App/..., solution/App/...           ← mirrored for Reset / Show Solution
```

Some Module 9 lessons (Docker, CI/CD) are **guided labs** — `lesson.md` + `quiz.json` only, no `exercise/`, since they need tools (Docker Desktop, a real GitHub repo) this environment doesn't have. Follow the "Do this" steps at the end of those lessons yourself.

## Adding content
Drop a new folder following the layout above — no code changes needed. Exercise check modes:
- **output** — `dotnet run Program.cs`, stdout compared to `expected.txt` (or every `cases/N.in` → `cases/N.out`).
- **test** — `dotnet test` on the exercise's project; `Tests.cs` decides pass/fail. `"minTests": n` in `meta.json` sets the fewest tests that count, for exercises where the learner writes the tests. (Used from the "Methods" lesson onward, including every ASP.NET Core / EF Core exercise via `WebApplicationFactory`.)

## Rebuilding the extension (after changing `extension/src`)
```powershell
cd extension
npm run build
npx vsce package --no-dependencies --allow-missing-repository
code --install-extension learn-lab-0.1.0.vsix --force
```
Then `Developer: Reload Window` in VS Code.
