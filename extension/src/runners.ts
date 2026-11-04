import { spawn, spawnSync, ChildProcess } from 'child_process';
import * as fs from 'fs';
import * as path from 'path';
import * as vscode from 'vscode';
import { ExerciseItem } from './model';

export interface CheckResult {
  passed: boolean;
  cancelled?: boolean;
  summary: string;   // one line, shown in a notification
  details: string;   // full output, shown in the Output channel
  expected?: string; // output mode: for the diff view
  actual?: string;
}

export interface Runner {
  check(ex: ExerciseItem, token: vscode.CancellationToken): Promise<CheckResult>;
}

// ---------- process helper ----------

const OUTPUT_LIMIT = 1_000_000;  // characters; a runaway print loop must not fill memory
const RUN_TIMEOUT = 30_000;      // one output-mode run; normal runs take about a second
const TEST_TIMEOUT = 180_000;    // includes restore and build
const KILL_GRACE = 3_000;

interface ProcResult { code: number | null; stdout: string; stderr: string; timedOut: boolean; cancelled: boolean; overflow: boolean; missing: boolean; }

// `dotnet run` and `dotnet test` start child processes of their own, so the whole tree has to go.
function killTree(child: ChildProcess) {
  if (process.platform === 'win32' && child.pid) spawnSync('taskkill', ['/PID', String(child.pid), '/T', '/F'], { windowsHide: true });
  else if (child.pid) process.kill(-child.pid, 'SIGKILL');
}

function run(cmd: string, args: string[], cwd: string, stdin: string | undefined, token: vscode.CancellationToken, timeoutMs = RUN_TIMEOUT): Promise<ProcResult> {
  return new Promise(resolve => {
    const child = spawn(cmd, args, { cwd, windowsHide: true, detached: process.platform !== 'win32', env: { ...process.env, DOTNET_CLI_UI_LANGUAGE: 'en', DOTNET_NOLOGO: '1' } });
    let stdout = '', stderr = '', timedOut = false, cancelled = false, overflow = false, finished = false;
    let graceTimer: NodeJS.Timeout | undefined;

    const finish = (code: number | null, missing = false) => {
      if (finished) return;
      finished = true;
      clearTimeout(timer);
      clearTimeout(graceTimer);
      sub.dispose();
      resolve({ code, stdout, stderr, timedOut, cancelled, overflow, missing });
    };
    // Don't wait on `close` alone: a process that survives the kill keeps the pipes open and it would never fire.
    const stop = () => {
      killTree(child);
      graceTimer = setTimeout(() => finish(null), KILL_GRACE);
    };
    const collect = (append: (s: string) => void) => (d: Buffer) => {
      if (stdout.length + stderr.length < OUTPUT_LIMIT) { append(d.toString()); return; }
      if (!overflow) { overflow = true; stop(); }
    };

    const timer = setTimeout(() => { timedOut = true; stop(); }, timeoutMs);
    const sub = token.onCancellationRequested(() => { cancelled = true; stop(); });
    child.stdout.on('data', collect(s => (stdout += s)));
    child.stderr.on('data', collect(s => (stderr += s)));
    child.on('close', code => finish(code));
    child.on('error', err => { stderr += String(err); finish(-1, (err as NodeJS.ErrnoException).code === 'ENOENT'); });
    child.stdin.on('error', () => { /* the program may exit before reading its input */ });
    if (stdin !== undefined) child.stdin.write(stdin);
    child.stdin.end();
  });
}

function normalize(s: string): string {
  return s.replace(/\r\n/g, '\n').split('\n').map(l => l.trimEnd()).join('\n').trimEnd();
}

interface Case { name: string; input: string; expected: string; }

function loadCases(ex: ExerciseItem): Case[] {
  const casesDir = path.join(ex.dir, 'cases');
  if (fs.existsSync(casesDir)) {
    return fs.readdirSync(casesDir).filter(f => f.endsWith('.in')).sort().map(f => {
      const name = f.slice(0, -3);
      const outFile = path.join(casesDir, name + '.out');
      return {
        name,
        input: fs.readFileSync(path.join(casesDir, f), 'utf8'),
        expected: fs.existsSync(outFile) ? fs.readFileSync(outFile, 'utf8') : '',
      };
    });
  }
  return [{
    name: '1',
    input: ex.inputFile ? fs.readFileSync(ex.inputFile, 'utf8') : '',
    expected: ex.expectedFile ? fs.readFileSync(ex.expectedFile, 'utf8') : '',
  }];
}

const DOTNET_MISSING = 'The .NET SDK was not found. Install .NET 10 from https://dotnet.microsoft.com/download, then restart VS Code.';

function stopped(r: ProcResult, seconds: number): Pick<CheckResult, 'summary' | 'cancelled'> | undefined {
  if (r.cancelled) return { summary: 'Check cancelled.', cancelled: true };
  if (r.overflow) return { summary: 'Stopped: your program printed far too much output. Look for a loop that never ends.' };
  if (r.timedOut) return { summary: `Stopped after ${seconds}s. Look for a loop that never ends.` };
  return undefined;
}

// Restore problems are not the learner's code, so they must not read like a code or test failure.
function restoreError(out: string): string | undefined {
  const line = out.split('\n').find(l => /error\s+NU\d+/.test(l));
  if (!line) return undefined;
  const code = line.match(/NU\d+/)?.[0];
  return /NU13\d\d/.test(line) && code !== 'NU1302'
    ? `Could not download the NuGet packages (${code}). Check your internet connection and try again.`
    : `NuGet restore failed (${code}). Show output has the full message.`;
}

// ---------- .NET runner ----------

export class DotnetRunner implements Runner {
  constructor(private dotnet: () => string) {}

  async check(ex: ExerciseItem, token: vscode.CancellationToken): Promise<CheckResult> {
    return ex.check === 'output' ? this.checkOutput(ex, token) : this.checkTests(ex, token);
  }

  // Mode 1: run the program (file-based app: `dotnet run Program.cs`) and compare stdout to expected output.
  // Either a single input.txt/expected.txt pair, or a cases/ folder with N.in + N.out pairs (all must pass).
  private async checkOutput(ex: ExerciseItem, token: vscode.CancellationToken): Promise<CheckResult> {
    const entry = ex.studentFiles[0];
    const hasProject = fs.readdirSync(ex.dir).some(f => f.endsWith('.csproj'));
    const args = hasProject ? ['run', '--nologo'] : ['run', entry];

    const cases = loadCases(ex);
    const log: string[] = [];
    for (const c of cases) {
      const r = await run(this.dotnet(), args, ex.dir, c.input, token);
      const label = cases.length > 1 ? `[case ${c.name}] ` : '';
      log.push(`── ${label}stdin: ${JSON.stringify(c.input)}`);

      if (r.missing) return { passed: false, summary: DOTNET_MISSING, details: r.stderr };
      const stop = stopped(r, RUN_TIMEOUT / 1000);
      if (stop) return { passed: false, ...stop, summary: label + stop.summary, details: log.concat(r.stdout.slice(-2000), r.stderr.slice(-2000)).join('\n') };
      if (r.code !== 0) {
        const text = r.stdout + '\n' + r.stderr;
        const errs = text.split('\n').filter(l => /error|Unhandled exception/i.test(l));
        if (/error CS5001/.test(text)) return { passed: false, summary: `Nothing to run yet. Write your code in ${entry}, then check again.`, details: log.concat(r.stdout, r.stderr).join('\n') };
        const where = text.match(/:line (\d+)/)?.[1];
        const first = errs[0]?.replace(/^.*?\((\d+),(\d+)\):\s*/, 'Line $1: ').replace(/\s*\[.*\]$/, '').trim();
        return {
          passed: false,
          summary: label + (first ? first + (where && !first.startsWith('Line') ? ` (line ${where})` : '') : `Program exited with code ${r.code}`),
          details: log.concat(r.stdout, r.stderr).join('\n'),
        };
      }

      const expected = normalize(c.expected);
      const actual = normalize(r.stdout);
      log.push(actual, '');
      if (expected === actual) continue;

      const eL = expected.split('\n'), aL = actual.split('\n');
      let i = 0;
      while (i < eL.length && i < aL.length && eL[i] === aL[i]) i++;
      const summary = i >= eL.length ? `Extra output on line ${i + 1}: "${aL[i]}"`
        : i >= aL.length ? `Missing line ${i + 1}: expected "${eL[i]}"`
        : `Line ${i + 1} differs. Expected "${eL[i]}" but got "${aL[i]}"`;
      return { passed: false, summary: label + summary, details: log.concat(`--- expected ---`, expected, `--- your output ---`, actual).join('\n'), expected, actual };
    }
    return { passed: true, summary: cases.length > 1 ? `All ${cases.length} cases match.` : 'Output matches exactly.', details: log.join('\n') };
  }

  // Mode 2: `dotnet test` on the exercise project; Tests.cs is the oracle
  private async checkTests(ex: ExerciseItem, token: vscode.CancellationToken): Promise<CheckResult> {
    // The normal console logger prints failing test names and Expected/Actual. It also replaces the compact
    // "Failed: x, Passed: y" line with "Total tests: n" plus separate Passed/Failed lines, parsed below.
    const r = await run(this.dotnet(), ['test', '--nologo', '-v', 'q', '--logger', 'console;verbosity=normal'], ex.dir, undefined, token, TEST_TIMEOUT);
    const out = r.stdout + '\n' + r.stderr;

    if (r.missing) return { passed: false, summary: DOTNET_MISSING, details: out };
    const stop = stopped(r, TEST_TIMEOUT / 1000);
    if (stop) return { passed: false, ...stop, details: out.slice(-4000) };

    const restore = restoreError(out);
    if (restore) return { passed: false, summary: restore, details: out };

    const buildErrors = out.split('\n').filter(l => /error CS\d+/.test(l));
    if (buildErrors.length) {
      const first = buildErrors[0].replace(/^.*?\((\d+),(\d+)\):\s*error\s*(CS\d+):\s*/, 'Line $1 [$3]: ').replace(/\s*\[.*\]\s*$/, '').trim();
      return { passed: false, summary: `Does not compile — ${first}`, details: out };
    }

    const totalLine = out.match(/Total tests:\s*(\d+)/);
    if (!totalLine) {
      // A genuine run always prints "Total tests: n". No such line at all (e.g. zero [Fact]s discovered)
      // must never read as a pass, regardless of exit code.
      // Restore/MSBuild errors (path too long, locked files) also end here, so report those as they are.
      const toolError = out.split('\n').find(l => /:\s*error(\s+[A-Z]+\d+)?\s*:/.test(l));
      if (toolError) {
        const msg = toolError.replace(/^.*?:\s*error(\s+[A-Z]+\d+)?\s*:\s*/, '').replace(/\s*\[[^\]]*\]\s*$/, '').trim();
        return { passed: false, summary: `Build failed before any test ran — ${msg}`, details: out };
      }
      return { passed: false, summary: 'No tests were found — write at least one [Fact].', details: out };
    }
    const total = Number(totalLine[1]);
    const passed = Number(out.match(/^\s*Passed:\s*(\d+)/m)?.[1] ?? 0);
    const failed = Number(out.match(/^\s*Failed:\s*(\d+)/m)?.[1] ?? 0);
    if (total === 0) return { passed: false, summary: 'No tests were found — write at least one [Fact].', details: out };

    if (failed === 0) {
      if (ex.minTests && total < ex.minTests) return { passed: false, summary: `${total} test${total > 1 ? 's' : ''} found, but this exercise asks for at least ${ex.minTests}.`, details: out };
      return { passed: true, summary: `All ${total} tests passed.`, details: out };
    }

    const failedNames = out.split('\n').filter(l => /^\s*Failed\s+.+\[[^\]]+\]\s*$/.test(l)).map(l => l.trim().replace(/\s*\[.*$/, '').replace(/^Failed\s+/, '').replace(/^.*\.(?=[^.(]+(\(|$))/, ''));
    const shown = failedNames.slice(0, 3).join(', ') + (failedNames.length > 3 ? ` and ${failedNames.length - 3} more` : '');
    const firstFailure = out.match(/Expected:[^\r\n]*\r?\n\s*Actual:[^\r\n]*/)?.[0].replace(/\s+/g, ' ').replace(/ Actual:/, ', Actual:');
    return { passed: false, summary: `${passed}/${total} passed. Failing: ${shown}.${firstFailure ? ` First failure: ${firstFailure}` : ''}`, details: out };
  }
}

export function getRunner(language: string, dotnetPath: () => string): Runner {
  switch (language) {
    case 'dotnet': return new DotnetRunner(dotnetPath);
    default: throw new Error(`No runner for language "${language}". Available: dotnet`);
  }
}

// ---------- helpers used by commands ----------

export function copyDir(from: string, to: string) {
  fs.mkdirSync(to, { recursive: true });
  for (const e of fs.readdirSync(from, { withFileTypes: true })) {
    const src = path.join(from, e.name), dst = path.join(to, e.name);
    if (e.isDirectory()) { copyDir(src, dst); continue; }
    fs.copyFileSync(src, dst);
    // Windows keeps the source's old timestamp, and an old timestamp makes `dotnet` reuse the previous build.
    const now = new Date();
    fs.utimesSync(dst, now, now);
  }
}
