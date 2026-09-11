import { spawn } from 'child_process';
import * as fs from 'fs';
import * as path from 'path';
import * as vscode from 'vscode';
import { ExerciseItem } from './model';

export interface CheckResult {
  passed: boolean;
  summary: string;   // one line, shown in a notification
  details: string;   // full output, shown in the Output channel
  expected?: string; // output mode: for the diff view
  actual?: string;
}

export interface Runner {
  check(ex: ExerciseItem, token: vscode.CancellationToken): Promise<CheckResult>;
}

// ---------- process helper ----------

interface ProcResult { code: number | null; stdout: string; stderr: string; timedOut: boolean; }

function run(cmd: string, args: string[], cwd: string, stdin: string | undefined, token: vscode.CancellationToken, timeoutMs = 120_000): Promise<ProcResult> {
  return new Promise(resolve => {
    const child = spawn(cmd, args, { cwd, shell: process.platform === 'win32', env: { ...process.env, DOTNET_CLI_UI_LANGUAGE: 'en', DOTNET_NOLOGO: '1' } });
    let stdout = '', stderr = '', timedOut = false;
    const timer = setTimeout(() => { timedOut = true; child.kill(); }, timeoutMs);
    token.onCancellationRequested(() => child.kill());
    child.stdout.on('data', d => (stdout += d));
    child.stderr.on('data', d => (stderr += d));
    child.on('close', code => { clearTimeout(timer); resolve({ code, stdout, stderr, timedOut }); });
    child.on('error', err => { clearTimeout(timer); resolve({ code: -1, stdout, stderr: String(err), timedOut }); });
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

      if (r.timedOut) return { passed: false, summary: `${label}Timed out after 120s (infinite loop? waiting for input?)`, details: log.concat(r.stdout, r.stderr).join('\n') };
      if (r.code !== 0) {
        const errs = (r.stdout + '\n' + r.stderr).split('\n').filter(l => /error|Unhandled exception/i.test(l));
        return {
          passed: false,
          summary: label + (errs[0]?.replace(/^.*?\((\d+),(\d+)\):\s*/, 'Line $1: ').replace(/\s*\[.*\]$/, '').trim() || `Program exited with code ${r.code}`),
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
    const r = await run(this.dotnet(), ['test', '--nologo', '-v', 'q'], ex.dir, undefined, token, 180_000);
    const out = r.stdout + '\n' + r.stderr;

    if (r.timedOut) return { passed: false, summary: 'Timed out after 180s', details: out };

    const buildErrors = out.split('\n').filter(l => /error CS\d+/.test(l));
    if (buildErrors.length) {
      const first = buildErrors[0].replace(/^.*?\((\d+),(\d+)\):\s*error\s*(CS\d+):\s*/, 'Line $1 [$3]: ').replace(/\s*\[.*\]$/, '').trim();
      return { passed: false, summary: `Does not compile — ${first}`, details: out };
    }

    const m = out.match(/Failed:\s*(\d+),\s*Passed:\s*(\d+),\s*Skipped:\s*(\d+),\s*Total:\s*(\d+)/);
    if (!m) {
      // A genuine all-pass run always prints the "Failed: X, Passed: Y, ..." summary line.
      // No summary at all (e.g. zero [Fact]s discovered) must never read as a pass, regardless of exit code.
      return { passed: false, summary: 'No tests were found — write at least one [Fact].', details: out };
    }
    const [, failed, passed, , total] = m.map(Number);
    if (total === 0) return { passed: false, summary: 'No tests were found — write at least one [Fact].', details: out };
    if (failed === 0) return { passed: true, summary: `All ${total} tests passed.`, details: out };

    const failedNames = out.split('\n').filter(l => /^\s*Failed\s+\S+/.test(l)).map(l => l.trim().replace(/\s*\[.*$/, '').replace(/^Failed\s+/, '')).slice(0, 3);
    return { passed: false, summary: `${passed}/${total} passed. Failing: ${failedNames.join(', ')}`, details: out };
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
    if (e.isDirectory()) copyDir(src, dst); else fs.copyFileSync(src, dst);
  }
}
