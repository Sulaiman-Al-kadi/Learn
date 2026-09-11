import * as fs from 'fs';
import * as path from 'path';
import * as vscode from 'vscode';

export type Status = 'todo' | 'attempted' | 'done';

export interface ItemProgress {
  status: Status;
  score?: number;
  total?: number;
  hintsUsed?: number;
  attempts?: number;
  updatedAt: string;
}

interface ProgressFile { version: 1; items: Record<string, ItemProgress>; }

// Persists to <workspace>/.learn/progress.json so it travels with the folder.
export class ProgressStore {
  private data: ProgressFile = { version: 1, items: {} };
  private readonly _onDidChange = new vscode.EventEmitter<void>();
  readonly onDidChange = this._onDidChange.event;

  constructor(private readonly file: string) { this.load(); }

  private load() {
    try { this.data = JSON.parse(fs.readFileSync(this.file, 'utf8')); } catch { this.data = { version: 1, items: {} }; }
  }
  private save() {
    fs.mkdirSync(path.dirname(this.file), { recursive: true });
    fs.writeFileSync(this.file, JSON.stringify(this.data, null, 2));
    this._onDidChange.fire();
  }

  get(id: string): ItemProgress | undefined { return this.data.items[id]; }
  status(id: string): Status { return this.data.items[id]?.status ?? 'todo'; }

  update(id: string, patch: Partial<ItemProgress>) {
    const prev = this.data.items[id] ?? { status: 'todo' as Status, updatedAt: '' };
    // A passed exercise stays passed even if a later attempt fails.
    const downgrade = prev.status === 'done' && patch.status === 'attempted';
    const status = patch.status && !downgrade ? patch.status : prev.status;
    this.data.items[id] = { ...prev, ...patch, status, updatedAt: new Date().toISOString() };
    this.save();
  }

  bumpHints(id: string): number {
    const n = (this.data.items[id]?.hintsUsed ?? 0) + 1;
    this.update(id, { hintsUsed: n, status: this.status(id) === 'done' ? 'done' : 'attempted' });
    return n;
  }

  resetAll() { this.data = { version: 1, items: {} }; this.save(); }

  summary(ids: string[]): { done: number; total: number } {
    return { done: ids.filter(id => this.status(id) === 'done').length, total: ids.length };
  }
}
