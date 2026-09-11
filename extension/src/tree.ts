import * as vscode from 'vscode';
import { Course, Module, Lesson, Item } from './model';
import { ProgressStore, Status } from './progress';

export type Node =
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
    case 'exercise': return new vscode.ThemeIcon(status === 'attempted' ? 'beaker' : 'beaker', status === 'attempted' ? new vscode.ThemeColor('testing.iconQueued') : undefined);
  }
}

export class CourseTree implements vscode.TreeDataProvider<Node> {
  private readonly _onDidChangeTreeData = new vscode.EventEmitter<Node | undefined>();
  readonly onDidChangeTreeData = this._onDidChangeTreeData.event;

  constructor(private courses: () => Course[], private progress: ProgressStore) {
    progress.onDidChange(() => this.refresh());
  }

  refresh() { this._onDidChangeTreeData.fire(undefined); }

  getChildren(node?: Node): Node[] {
    if (!node) return this.courses().map(course => ({ kind: 'course', course }));
    switch (node.kind) {
      case 'course': return node.course.modules.map(module => ({ kind: 'module', module }));
      case 'module': return node.module.lessons.map(lesson => ({ kind: 'lesson', lesson }));
      case 'lesson': return node.lesson.items.map(item => ({ kind: 'item', item }));
      case 'item': return [];
    }
  }

  getParent(node: Node): Node | undefined {
    switch (node.kind) {
      case 'course': return undefined;
      case 'module': return { kind: 'course', course: node.module.course };
      case 'lesson': return { kind: 'module', module: node.lesson.module };
      case 'item': return { kind: 'lesson', lesson: node.item.lesson };
    }
  }

  getTreeItem(node: Node): vscode.TreeItem {
    const C = vscode.TreeItemCollapsibleState;
    switch (node.kind) {
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
        const t = new vscode.TreeItem(node.module.title, allDone ? C.Collapsed : C.Expanded);
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
