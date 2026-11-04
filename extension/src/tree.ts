import * as vscode from 'vscode';
import { Course, Module, Lesson, Item } from './model';
import { ProgressStore, Status } from './progress';

export type Node =
  | { kind: 'continue' }
  | { kind: 'review' }
  | { kind: 'course'; course: Course }
  | { kind: 'module'; module: Module }
  | { kind: 'lesson'; lesson: Lesson }
  | { kind: 'item'; item: Item };

function itemIds(x: Course | Module | Lesson): string[] {
  if ('modules' in x) return x.modules.flatMap(itemIds);
  if ('lessons' in x) return x.lessons.flatMap(itemIds);
  return x.items.map(i => i.id);
}

function statusIcon(status: Status): vscode.ThemeIcon {
  switch (status) {
    case 'done': return new vscode.ThemeIcon('pass-filled', new vscode.ThemeColor('testing.iconPassed'));
    case 'attempted': return new vscode.ThemeIcon('circle-large-filled', new vscode.ThemeColor('testing.iconQueued'));
    default: return new vscode.ThemeIcon('circle-large-outline');
  }
}

function typeIcon(item: Item, status: Status): vscode.ThemeIcon {
  if (status === 'done') return statusIcon('done');
  switch (item.type) {
    case 'lesson': return new vscode.ThemeIcon('book');
    case 'quiz': return new vscode.ThemeIcon('question');
    case 'exercise': return new vscode.ThemeIcon('beaker', status === 'attempted' ? new vscode.ThemeColor('testing.iconQueued') : undefined);
  }
}

const itemLabel = (item: Item) => (item.type === 'lesson' ? 'Read the lesson' : item.type === 'quiz' ? 'Quiz' : item.title.replace(/^Exercise:\s*/, ''));

export class CourseTree implements vscode.TreeDataProvider<Node> {
  private readonly _onDidChangeTreeData = new vscode.EventEmitter<Node | undefined>();
  readonly onDidChangeTreeData = this._onDidChangeTreeData.event;

  constructor(private courses: () => Course[], private progress: ProgressStore, private upNext: () => Item | undefined) {
    progress.onDidChange(() => this.refresh());
  }

  refresh() { this._onDidChangeTreeData.fire(undefined); }

  getChildren(node?: Node): Node[] {
    if (!node) {
      const top: Node[] = [];
      if (this.upNext()) top.push({ kind: 'continue' });
      if (this.courses().some(c => itemIds(c).some(id => id.endsWith('/quiz') && this.progress.get(id)?.total))) top.push({ kind: 'review' });
      return [...top, ...this.courses().map(course => ({ kind: 'course', course }) as Node)];
    }
    switch (node.kind) {
      case 'course': return node.course.modules.map(module => ({ kind: 'module', module }));
      case 'module': return node.module.lessons.map(lesson => ({ kind: 'lesson', lesson }));
      case 'lesson': return node.lesson.items.map(item => ({ kind: 'item', item }));
      default: return [];
    }
  }

  getParent(node: Node): Node | undefined {
    switch (node.kind) {
      case 'module': return { kind: 'course', course: node.module.course };
      case 'lesson': return { kind: 'module', module: node.lesson.module };
      case 'item': return { kind: 'lesson', lesson: node.item.lesson };
      default: return undefined;
    }
  }

  getTreeItem(node: Node): vscode.TreeItem {
    const C = vscode.TreeItemCollapsibleState;
    switch (node.kind) {
      case 'continue': {
        const next = this.upNext()!;
        const t = new vscode.TreeItem('Continue', C.None);
        t.description = `${next.lesson.title}: ${itemLabel(next)}`;
        t.tooltip = 'Open the next unfinished step';
        t.iconPath = new vscode.ThemeIcon('play', new vscode.ThemeColor('testing.iconPassed'));
        t.command = { command: 'learnLab.continue', title: 'Continue' };
        return t;
      }
      case 'review': {
        const due = this.progress.dueReviewKeys().length;
        const t = new vscode.TreeItem('Review', C.None);
        t.description = due ? `${due} due` : '5 questions';
        t.tooltip = 'A short round of questions from earlier lessons';
        t.iconPath = new vscode.ThemeIcon('history');
        t.command = { command: 'learnLab.review', title: 'Review' };
        return t;
      }
      case 'course': {
        const s = this.progress.summary(itemIds(node.course));
        const t = new vscode.TreeItem(node.course.title, C.Expanded);
        t.description = `${s.done}/${s.total}`;
        t.iconPath = new vscode.ThemeIcon('mortar-board');
        t.contextValue = 'course';
        return t;
      }
      case 'module': {
        const s = this.progress.summary(itemIds(node.module));
        const allDone = s.total > 0 && s.done === s.total;
        const here = this.upNext()?.lesson.module.id === node.module.id;
        const t = new vscode.TreeItem(node.module.title, here ? C.Expanded : C.Collapsed);
        t.description = `${s.done}/${s.total}`;
        t.iconPath = allDone ? statusIcon('done') : new vscode.ThemeIcon('folder-library');
        t.contextValue = 'module';
        return t;
      }
      case 'lesson': {
        const s = this.progress.summary(itemIds(node.lesson));
        const allDone = s.total > 0 && s.done === s.total;
        const started = node.lesson.items.some(i => this.progress.status(i.id) !== 'todo');
        const t = new vscode.TreeItem(node.lesson.title, allDone ? C.Collapsed : started ? C.Expanded : C.Collapsed);
        t.description = `${s.done}/${s.total}`;
        t.iconPath = allDone ? statusIcon('done') : started ? statusIcon('attempted') : statusIcon('todo');
        t.contextValue = 'lesson';
        return t;
      }
      case 'item': {
        const item = node.item;
        const p = this.progress.get(item.id);
        const status = p?.status ?? 'todo';
        const t = new vscode.TreeItem(item.title, C.None);
        t.iconPath = typeIcon(item, status);
        t.contextValue = item.type;
        t.command = { command: 'learnLab.open', title: 'Open', arguments: [item] };
        if (item.type === 'quiz' && p?.total) t.description = `${p.score}/${p.total}`;
        if (item.type === 'exercise') {
          const bits: string[] = [];
          if (p?.hintsUsed) bits.push(`${p.hintsUsed} hint${p.hintsUsed > 1 ? 's' : ''}`);
          if (p?.attempts) bits.push(`${p.attempts} run${p.attempts > 1 ? 's' : ''}`);
          t.description = bits.join(', ');
          t.tooltip = `${item.check === 'output' ? 'Checked by comparing output' : 'Checked by unit tests'}`;
        }
        return t;
      }
    }
  }
}
