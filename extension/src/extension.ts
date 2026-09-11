import * as fs from 'fs';
import * as path from 'path';
import * as vscode from 'vscode';
import { Course, Item, ExerciseItem, loadCourses, allItems, findExerciseForFile } from './model';
import { ProgressStore } from './progress';
import { CourseTree, Node } from './tree';
import { getRunner, copyDir, CheckResult } from './runners';
import { QuizPanel, HintsPanel, parseHints } from './panels';

let courses: Course[] = [];
let progress: ProgressStore;
let tree: CourseTree;
let treeView: vscode.TreeView<Node>;
let output: vscode.OutputChannel;
let statusBar: vscode.StatusBarItem;
let currentExercise: ExerciseItem | undefined;

// Virtual documents for the "expected vs actual" diff in output-mode exercises
const diffDocs = new Map<string, string>();
const DIFF_SCHEME = 'learnlab';

export function activate(context: vscode.ExtensionContext) {
  const root = vscode.workspace.workspaceFolders?.[0]?.uri.fsPath;
  if (!root) return;

  const coursesRoot = path.join(root, 'courses');
  output = vscode.window.createOutputChannel('Learn Lab');
  progress = new ProgressStore(path.join(root, '.learn', 'progress.json'));
  reload(coursesRoot);

  tree = new CourseTree(() => courses, progress);
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
  }
}

export function deactivate() {}

// ---------- helpers ----------

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
  vscode.commands.executeCommand('setContext', 'learnLab.hasCurrentExercise', true);
  updateStatusBar();
}

function updateStatusBar() {
  if (!currentExercise) { statusBar.hide(); return; }
  statusBar.text = `$(beaker) Check: ${currentExercise.lesson.title}`;
  statusBar.tooltip = 'Run the checker for the current exercise (Ctrl+Alt+Enter)';
  statusBar.show();
}

async function openItem(item: Item) {
  switch (item.type) {
    case 'lesson': {
      const uri = vscode.Uri.file(item.file);
      await vscode.commands.executeCommand('markdown.showPreview', uri);
      progress.update(item.id, { status: 'done' });   // reading it counts; the quiz is what tests you
      break;
    }
    case 'quiz':
      QuizPanel.show(item, progress);
      break;
    case 'exercise': {
      setCurrent(item);
      const readme = path.join(item.dir, 'README.md');
      const main = path.join(item.dir, item.studentFiles[0]);
      if (fs.existsSync(readme)) await vscode.commands.executeCommand('markdown.showPreview', vscode.Uri.file(readme));
      const doc = await vscode.workspace.openTextDocument(main);
      await vscode.window.showTextDocument(doc, fs.existsSync(readme) ? vscode.ViewColumn.Beside : vscode.ViewColumn.Active);
      break;
    }
  }
}

async function goNext() {
  const items = allItems(courses);
  const startAt = currentExercise ? items.findIndex(i => i.id === currentExercise!.id) + 1 : 0;
  const next = [...items.slice(startAt), ...items.slice(0, startAt)].find(i => progress.status(i.id) !== 'done');
  if (!next) { vscode.window.showInformationMessage('Everything is done. Add more content in courses/ or reset progress.'); return; }
  await openItem(next);
  try { await treeView.reveal({ kind: 'item', item: next } as Node, { select: true, expand: true }); } catch { /* reveal needs getParent; optional */ }
}

async function checkExercise(ex: ExerciseItem | undefined, dotnetPath: () => string) {
  if (!ex) { vscode.window.showInformationMessage('Open an exercise first (click one in the Learn Lab sidebar).'); return; }
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
  if (!ex) return;
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
