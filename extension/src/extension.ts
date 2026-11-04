import { execFile } from 'child_process';
import * as fs from 'fs';
import * as path from 'path';
import * as vscode from 'vscode';
import { Course, Item, ExerciseItem, loadCourses, allItems, findExerciseForFile } from './model';
import { ProgressStore } from './progress';
import { CourseTree, Node } from './tree';
import { getRunner, copyDir, CheckResult } from './runners';
import { QuizPanel, HintsPanel, parseHints } from './panels';
import { LessonPanel } from './lessonView';

let courses: Course[] = [];
let progress: ProgressStore;
let tree: CourseTree;
let treeView: vscode.TreeView<Node>;
let output: vscode.OutputChannel;
let statusBar: vscode.StatusBarItem;
let currentExercise: ExerciseItem | undefined;
let currentItem: Item | undefined;
let checking = false;
let workspaceRoot = '';

// Virtual documents for the "expected vs actual" diff in output-mode exercises
const diffDocs = new Map<string, string>();
const DIFF_SCHEME = 'learnlab';

export function activate(context: vscode.ExtensionContext) {
  const root = vscode.workspace.workspaceFolders?.[0]?.uri.fsPath;
  if (!root) return;
  workspaceRoot = root;

  const coursesRoot = path.join(root, 'courses');
  output = vscode.window.createOutputChannel('Learn Lab');
  progress = new ProgressStore(path.join(root, '.learn', 'progress.json'));
  reload(coursesRoot);

  tree = new CourseTree(() => courses, progress, upNext);
  treeView = vscode.window.createTreeView('learnLab.courseTree', { treeDataProvider: tree, showCollapseAll: true });

  statusBar = vscode.window.createStatusBarItem(vscode.StatusBarAlignment.Left, 50);
  statusBar.command = 'learnLab.check';
  updateStatusBar();

  const dotnetPath = () => vscode.workspace.getConfiguration('learnLab').get<string>('dotnetPath') || 'dotnet';

  context.subscriptions.push(
    treeView, output, statusBar,

    vscode.workspace.registerTextDocumentContentProvider(DIFF_SCHEME, { provideTextDocumentContent: uri => diffDocs.get(uri.path) ?? '' }),

    vscode.window.onDidChangeActiveTextEditor(e => {
      if (e) { const ex = findExerciseForFile(courses, e.document.uri.fsPath); if (ex) setCurrent(ex); }
    }),

    // Watch content folder so editing a lesson/quiz shows up without reloading VS Code
    (() => { const w = vscode.workspace.createFileSystemWatcher(new vscode.RelativePattern(coursesRoot, '**/{course,module,lesson,quiz,meta}.json'));
      w.onDidChange(() => refresh(coursesRoot)); w.onDidCreate(() => refresh(coursesRoot)); w.onDidDelete(() => refresh(coursesRoot)); return w; })(),

    vscode.commands.registerCommand('learnLab.refresh', () => refresh(coursesRoot)),
    vscode.commands.registerCommand('learnLab.open', openItem),
    vscode.commands.registerCommand('learnLab.next', goNext),
    vscode.commands.registerCommand('learnLab.continue', goContinue),
    vscode.commands.registerCommand('learnLab.review', () => QuizPanel.showReview(courses, progress)),
    vscode.commands.registerCommand('learnLab.check', (node?: Node | ExerciseItem) => checkExercise(pick(node), dotnetPath)),
    vscode.commands.registerCommand('learnLab.hint', (node?: Node | ExerciseItem) => showHint(pick(node))),
    vscode.commands.registerCommand('learnLab.reset', (node?: Node | ExerciseItem) => resetExercise(pick(node))),
    vscode.commands.registerCommand('learnLab.solution', (node?: Node | ExerciseItem) => showSolution(pick(node))),
    vscode.commands.registerCommand('learnLab.resetProgress', async () => {
      const ok = await vscode.window.showWarningMessage('Reset ALL progress? Your code stays; only the checkmarks are cleared.', { modal: true }, 'Reset');
      if (ok === 'Reset') progress.resetAll();
    }),
  );

  if (courses.length === 0) {
    vscode.window.showWarningMessage(`Learn Lab: no courses found in ${coursesRoot}`);
  } else {
    // Land the learner on the first unfinished item
    const first = allItems(courses).find(i => progress.status(i.id) !== 'done');
    if (first?.type === 'exercise') setCurrent(first);
    checkDotnet(dotnetPath());
    welcome(context);
  }
}

export function deactivate() {}

// ---------- helpers ----------

function checkDotnet(dotnet: string) {
  execFile(dotnet, ['--version'], async (err, stdout) => {
    if (!err && parseInt(stdout) >= 10) return;
    const problem = err ? 'it was not found on this computer' : `this computer has ${stdout.trim()}`;
    const pick = await vscode.window.showWarningMessage(`Learn Lab checks your code with the .NET 10 SDK, but ${problem}.`, 'Download .NET');
    if (pick) vscode.env.openExternal(vscode.Uri.parse('https://dotnet.microsoft.com/download/dotnet/10.0'));
  });
}

async function welcome(context: vscode.ExtensionContext) {
  const untouched = allItems(courses).every(i => progress.status(i.id) === 'todo');
  if (!untouched || context.workspaceState.get('learnLab.welcomed')) return;
  context.workspaceState.update('learnLab.welcomed', true);
  vscode.commands.executeCommand('learnLab.courseTree.focus');
  const pick = await vscode.window.showInformationMessage('Welcome to Learn Lab. Lessons, quizzes, and exercises that check themselves are all in this panel.', 'Start the first lesson');
  if (pick) goContinue();
}

function reload(coursesRoot: string) {
  try { courses = loadCourses(coursesRoot); }
  catch (e) { courses = []; vscode.window.showErrorMessage(`Learn Lab: failed to load courses — ${e}`); }
}
function refresh(coursesRoot: string) { reload(coursesRoot); tree.refresh(); }

function pick(node?: Node | ExerciseItem): ExerciseItem | undefined {
  if (!node) return currentExercise;
  if ('kind' in node) return node.kind === 'item' && node.item.type === 'exercise' ? node.item : currentExercise;
  return node.type === 'exercise' ? node : currentExercise;
}

function setCurrent(ex: ExerciseItem) {
  currentExercise = ex;
  currentItem = ex;
  vscode.commands.executeCommand('setContext', 'learnLab.hasCurrentExercise', true);
  updateStatusBar();
}

function updateStatusBar() {
  if (!currentExercise) { statusBar.hide(); return; }
  statusBar.text = `$(beaker) Check: ${currentExercise.lesson.title}`;
  statusBar.tooltip = 'Run the checker for the current exercise (Ctrl+Alt+Enter)';
  statusBar.show();
}

// First unfinished item from `startAt`, wrapping past the end of the course.
function firstUnfinished(startAt: number): Item | undefined {
  const items = allItems(courses);
  return [...items.slice(startAt), ...items.slice(0, startAt)].find(i => progress.status(i.id) !== 'done');
}

function upNext(): Item | undefined {
  const last = allItems(courses).findIndex(i => i.id === progress.lastItem);
  return firstUnfinished(Math.max(last, 0));
}

// What the lesson panel's button leads to, whether or not `item` itself is finished.
function nextAfter(item: Item): Item | undefined {
  const items = allItems(courses);
  const at = items.findIndex(i => i.id === item.id);
  return [...items.slice(at + 1), ...items.slice(0, at)].find(i => progress.status(i.id) !== 'done');
}

async function openItem(item: Item) {
  currentItem = item;
  progress.setLastItem(item.id);
  const context = { progress, nextAfter };
  switch (item.type) {
    case 'lesson':
      if (progress.status(item.id) === 'todo') progress.update(item.id, { status: 'attempted' });
      LessonPanel.show(item, context, vscode.ViewColumn.Active);
      break;
    case 'quiz':
      QuizPanel.show(item, progress);
      break;
    case 'exercise': {
      setCurrent(item);
      const hasReadme = fs.existsSync(path.join(item.dir, 'README.md'));
      if (hasReadme) LessonPanel.show(item, context, vscode.ViewColumn.One);
      const doc = await vscode.workspace.openTextDocument(path.join(item.dir, item.studentFiles[0]));
      await vscode.window.showTextDocument(doc, hasReadme ? vscode.ViewColumn.Two : vscode.ViewColumn.Active);
      break;
    }
  }
}

async function open(next: Item | undefined) {
  if (!next) { vscode.window.showInformationMessage('Everything is done. Add more content in courses/ or reset progress.'); return; }
  await openItem(next);
  try { await treeView.reveal({ kind: 'item', item: next } as Node, { select: true, expand: true }); } catch { /* reveal needs getParent; optional */ }
}

function goNext() {
  const items = allItems(courses);
  const at = currentItem ? items.findIndex(i => i.id === currentItem!.id) + 1 : 0;
  return open(firstUnfinished(at));
}

function goContinue() { return open(upNext()); }

async function checkExercise(ex: ExerciseItem | undefined, dotnetPath: () => string) {
  if (!ex) { vscode.window.showInformationMessage('Open an exercise first (click one in the Learn Lab sidebar).'); return; }
  if (checking) { vscode.window.showInformationMessage('A check is already running.'); return; }
  checking = true;
  try { await runCheck(ex, dotnetPath); } finally { checking = false; }
}

async function runCheck(ex: ExerciseItem, dotnetPath: () => string) {
  await vscode.workspace.saveAll(false);
  output.clear();
  output.appendLine(`▶ Checking ${ex.lesson.title} / ${ex.title}  (${ex.check === 'output' ? 'compare output' : 'unit tests'})`);
  output.appendLine('');

  const result = await vscode.window.withProgress(
    { location: vscode.ProgressLocation.Notification, title: `Checking ${ex.lesson.title}…`, cancellable: true },
    (_p, token) => getRunner(ex.lesson.module.course.language, dotnetPath).check(ex, token),
  );

  output.appendLine(result.details);
  output.appendLine('');
  output.appendLine(result.passed ? `✔ PASSED — ${result.summary}` : `✘ FAILED — ${result.summary}`);
  if (result.cancelled) return;
  saveLastCheck(ex, result);

  const prev = progress.get(ex.id);
  progress.update(ex.id, { attempts: (prev?.attempts ?? 0) + 1, status: result.passed ? 'done' : 'attempted' });

  if (result.passed) {
    const pick = await vscode.window.showInformationMessage(`✔ ${ex.lesson.title}: ${result.summary}`, 'Next item', 'Show output');
    if (pick === 'Next item') goNext();
    if (pick === 'Show output') output.show(true);
  } else {
    await showFailure(ex, result);
  }
}

// Lets a tutor in the editor (see CLAUDE.md) read what just happened.
function saveLastCheck(ex: ExerciseItem, result: CheckResult) {
  const body = ['# Last check', '', `Exercise: ${path.relative(workspaceRoot, ex.dir)}`, `Student files: ${ex.studentFiles.join(', ')}`, `Result: ${result.passed ? 'PASSED' : 'FAILED'}`, `Summary: ${result.summary}`, '', '~~~', result.details.slice(-6000), '~~~', ''];
  const file = path.join(workspaceRoot, '.learn', 'last-check.md');
  fs.mkdirSync(path.dirname(file), { recursive: true });
  fs.writeFileSync(file, body.join('\n'));
}

async function showFailure(ex: ExerciseItem, result: CheckResult) {
  const actions = ['Show output'];
  if (result.expected !== undefined) actions.unshift('Show diff');
  if (ex.hintsFile) actions.push('Hint');
  const pick = await vscode.window.showErrorMessage(`✘ ${result.summary}`, ...actions);
  if (pick === 'Show output') output.show(true);
  if (pick === 'Hint') showHint(ex);
  if (pick === 'Show diff' && result.expected !== undefined) {
    diffDocs.set('/expected.txt', result.expected + '\n');
    diffDocs.set('/actual.txt', (result.actual ?? '') + '\n');
    await vscode.commands.executeCommand('vscode.diff',
      vscode.Uri.parse(`${DIFF_SCHEME}:/expected.txt`), vscode.Uri.parse(`${DIFF_SCHEME}:/actual.txt`),
      'Expected output  ↔  Your output');
  }
}

function showHint(ex: ExerciseItem | undefined) {
  if (!ex) { vscode.window.showInformationMessage('Open an exercise first (click one in the Learn Lab sidebar).'); return; }
  if (!ex.hintsFile) { vscode.window.showInformationMessage('This exercise has no hints.'); return; }
  const hints = parseHints(ex.hintsFile);
  const used = progress.get(ex.id)?.hintsUsed ?? 0;
  const unlocked = used < hints.length ? progress.bumpHints(ex.id) : used;
  HintsPanel.show(ex, hints, unlocked);
}

async function resetExercise(ex: ExerciseItem | undefined) {
  if (!ex) return;
  if (!ex.starterDir) { vscode.window.showInformationMessage('No starter/ folder for this exercise.'); return; }
  const ok = await vscode.window.showWarningMessage(`Reset "${ex.lesson.title}" to the starter code? Your changes will be lost.`, { modal: true }, 'Reset');
  if (ok !== 'Reset') return;
  copyDir(ex.starterDir, ex.dir);
  progress.update(ex.id, { status: 'attempted' });
  vscode.window.showInformationMessage('Exercise reset.');
}

async function showSolution(ex: ExerciseItem | undefined) {
  if (!ex) return;
  if (!ex.solutionDir) { vscode.window.showInformationMessage('No solution available for this exercise.'); return; }
  const status = progress.status(ex.id);
  if (status !== 'done') {
    const ok = await vscode.window.showWarningMessage('Look at the solution before passing? You learn more by struggling a bit first.', { modal: true }, 'Show it anyway');
    if (ok !== 'Show it anyway') return;
  }
  for (const f of ex.studentFiles) {
    const mine = vscode.Uri.file(path.join(ex.dir, f));
    const sol = vscode.Uri.file(path.join(ex.solutionDir, f));
    if (fs.existsSync(sol.fsPath)) await vscode.commands.executeCommand('vscode.diff', mine, sol, `${f}: yours  ↔  solution`);
  }
}
