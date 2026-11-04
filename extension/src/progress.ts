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

// Leitner box for one quiz question. Key: `<quiz item id>#<question index>`.
export interface ReviewCard { box: number; due: string; }

interface ProgressFile {
  version: 1;
  items: Record<string, ItemProgress>;
  lastItem?: string;
  review?: Record<string, ReviewCard>;
}

export const PASS_RATIO = 0.8;
export const quizPassed = (score: number, total: number) => total > 0 && score / total >= PASS_RATIO;

const REVIEW_DAYS = [1, 3, 7, 21];
const daysFromNow = (days: number) => new Date(Date.now() + days * 86_400_000).toISOString();

// Persists to <workspace>/.learn/progress.json so it travels with the folder.
export class ProgressStore {
  private data: ProgressFile = { version: 1, items: {} };
  private readonly _onDidChange = new vscode.EventEmitter<void>();
  readonly onDidChange = this._onDidChange.event;

  constructor(private readonly file: string) { this.load(); }

  private load() {
    try { this.data = JSON.parse(fs.readFileSync(this.file, 'utf8')); } catch { this.data = { version: 1, items: {} }; }
    // Quizzes used to need a perfect score; results that now pass count as done.
    for (const p of Object.values(this.data.items)) {
      if (p.total && quizPassed(p.score ?? 0, p.total)) p.status = 'done';
    }
  }
  private save() {
    fs.mkdirSync(path.dirname(this.file), { recursive: true });
    fs.writeFileSync(this.file, JSON.stringify(this.data, null, 2));
    this._onDidChange.fire();
  }

  get(id: string): ItemProgress | undefined { return this.data.items[id]; }
  status(id: string): Status { return this.data.items[id]?.status ?? 'todo'; }
  get lastItem(): string | undefined { return this.data.lastItem; }

  setLastItem(id: string) {
    if (this.data.lastItem === id) return;
    this.data.lastItem = id;
    this.save();
  }

  update(id: string, patch: Partial<ItemProgress>) {
    const prev = this.data.items[id] ?? { status: 'todo' as Status, updatedAt: '' };
    // A passed exercise stays passed even if a later attempt fails.
    const downgrade = prev.status === 'done' && patch.status !== 'done';
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

  // ---------- spaced review ----------

  reviewCard(key: string): ReviewCard | undefined { return this.data.review?.[key]; }

  dueReviewKeys(): string[] {
    const now = new Date().toISOString();
    return Object.entries(this.data.review ?? {})
      .filter(([, c]) => c.due <= now)
      .sort(([, a], [, b]) => a.box - b.box || a.due.localeCompare(b.due))
      .map(([key]) => key);
  }

  // A miss comes back tomorrow; each correct review pushes the question further out.
  recordReview(results: { key: string; ok: boolean }[]) {
    const review = (this.data.review ??= {});
    for (const { key, ok } of results) {
      const box = ok ? Math.min((review[key]?.box ?? 0) + 1, REVIEW_DAYS.length - 1) : 0;
      review[key] = { box, due: daysFromNow(REVIEW_DAYS[box]) };
    }
    this.save();
  }
}
