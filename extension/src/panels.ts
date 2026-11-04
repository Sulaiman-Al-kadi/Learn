import * as fs from 'fs';
import * as vscode from 'vscode';
import { ExerciseItem, QuizItem, allItems, Course } from './model';
import { ProgressStore, quizPassed } from './progress';
import { CODE_CSS, esc, highlight, md, nonce } from './render';

// ---------- quiz.json schema ----------
// {
//   "title": "...",
//   "questions": [
//     { "type": "mcq",   "question": "markdown", "code": "optional csharp", "options": ["a","b"], "answer": 1, "explanation": "markdown" },
//     { "type": "multi", ...same, "answer": [0, 2] },
//     { "type": "tf",    "question": "...", "answer": true, "explanation": "..." }
//   ]
// }
export interface Question {
  type: 'mcq' | 'multi' | 'tf';
  question: string;
  code?: string;
  options?: string[];
  answer: number | number[] | boolean;
  explanation?: string;
}
interface Quiz { title?: string; questions: Question[]; }

const readQuiz = (item: QuizItem): Quiz => JSON.parse(fs.readFileSync(item.file, 'utf8'));
const inner = (s: string) => md(s).trim().replace(/^<p>|<\/p>$/g, '');

function shell(title: string, body: string, script = ''): string {
  const n = nonce();
  return `<!DOCTYPE html><html><head><meta charset="UTF-8">
<meta http-equiv="Content-Security-Policy" content="default-src 'none'; style-src 'unsafe-inline'; script-src 'nonce-${n}';">
<title>${esc(title)}</title>
<style>
  body { font-family: var(--vscode-font-family); font-size: calc(var(--vscode-font-size) * 1.1); color: var(--vscode-foreground); padding: 0 24px 40px; max-width: 72ch; margin: 0 auto; line-height: 1.6; }
  ::selection { background: var(--vscode-editor-selectionBackground); }
  :focus-visible { outline: 1px solid var(--vscode-focusBorder); outline-offset: 2px; }
  h1 { font-size: 1.6em; font-weight: 600; margin: 1.4em 0 .3em; }
  h2 { font-size: 1.2em; margin-top: 1.6em; }
  pre, code { font-family: var(--vscode-editor-font-family); font-size: .92em; font-variant-ligatures: none; }
  pre { background: var(--vscode-textCodeBlock-background); padding: 10px 14px; border-radius: 6px; overflow-x: auto; }
  code { background: var(--vscode-textCodeBlock-background); padding: 1px 4px; border-radius: 3px; }
  pre code { background: none; padding: 0; font-size: 1em; }
  .q { border: 1px solid var(--vscode-panel-border); border-radius: 6px; padding: 12px 16px; margin: 16px 0; }
  .q p { margin: .3em 0 .7em; }
  .q.correct { border-color: var(--vscode-testing-iconPassed); }
  .q.wrong { border-color: var(--vscode-testing-iconFailed); }
  .num { color: var(--vscode-descriptionForeground); font-weight: 600; }
  input { accent-color: var(--vscode-button-background); }
  label { display: block; padding: 6px 8px; border-radius: 4px; cursor: pointer; }
  label:hover { background: var(--vscode-list-hoverBackground); }
  label.right { background: color-mix(in srgb, var(--vscode-testing-iconPassed) 20%, transparent); }
  label.picked-wrong { background: color-mix(in srgb, var(--vscode-testing-iconFailed) 20%, transparent); }
  .expl { margin-top: 10px; padding: 8px 12px; border-radius: 6px; background: var(--vscode-textBlockQuote-background); }
  .expl p { margin: 0; }
  button { background: var(--vscode-button-background); color: var(--vscode-button-foreground); border: none; padding: 8px 18px; border-radius: 4px; cursor: pointer; font: inherit; margin: 0 8px 8px 0; }
  button:hover { background: var(--vscode-button-hoverBackground); }
  button.secondary { background: var(--vscode-button-secondaryBackground); color: var(--vscode-button-secondaryForeground); }
  button.secondary:hover { background: var(--vscode-button-secondaryHoverBackground); }
  .score { font-size: 1.3em; font-weight: 600; margin: 20px 0 12px; font-variant-numeric: tabular-nums; }
  .hint { padding: 8px 14px; margin: 14px 0; border-radius: 6px; background: var(--vscode-textBlockQuote-background); }
  .locked { color: var(--vscode-descriptionForeground); font-style: italic; }
  ${CODE_CSS}
</style></head><body>${body}<script nonce="${n}">${script}</script></body></html>`;
}

function shuffled<T>(xs: T[]): T[] {
  const a = [...xs];
  for (let i = a.length - 1; i > 0; i--) {
    const j = Math.floor(Math.random() * (i + 1));
    [a[i], a[j]] = [a[j], a[i]];
  }
  return a;
}

function isCorrect(q: Question, a: unknown): boolean {
  if (q.type === 'multi') {
    const want = [...(q.answer as number[])].sort().join(','), got = Array.isArray(a) ? [...a].sort().join(',') : '';
    return want === got;
  }
  return a === q.answer;
}

// ---------- Quiz session (a lesson quiz or a review set) ----------

interface QuizSession {
  panelId: string;
  title: string;
  intro: string;
  questions: Question[];
  doneLabel: string;
  // `correct` holds indices answered right at any point in this session.
  onGraded(correct: Set<number>, latest: { index: number; ok: boolean }[]): { passed: boolean };
  onDone(panel: vscode.WebviewPanel): void;
}

export class QuizPanel {
  private static panels = new Map<string, vscode.WebviewPanel>();

  static show(item: QuizItem, progress: ProgressStore) {
    const quiz = readQuiz(item);
    QuizPanel.open({
      panelId: item.id,
      title: quiz.title ?? `Quiz: ${item.lesson.title}`,
      intro: `${quiz.questions.length} questions. You pass at 80%, and you can retry just the ones you miss.`,
      questions: quiz.questions,
      doneLabel: 'Continue',
      onGraded(correct, latest) {
        const total = quiz.questions.length;
        const best = Math.max(correct.size, progress.get(item.id)?.score ?? 0);
        const passed = quizPassed(best, total);
        progress.update(item.id, { score: best, total, status: passed ? 'done' : 'attempted' });
        const missed = latest.filter(r => !r.ok).map(r => ({ key: `${item.id}#${r.index}`, ok: false }));
        if (missed.length) progress.recordReview(missed);
        return { passed: quizPassed(correct.size, total) };
      },
      onDone() { vscode.commands.executeCommand('learnLab.next'); },
    });
  }

  static showReview(courses: Course[], progress: ProgressStore) {
    const cards = pickReview(courses, progress);
    if (!cards.length) { vscode.window.showInformationMessage('Nothing to review yet. Finish a quiz first.'); return; }
    QuizPanel.panels.get('review')?.dispose();
    QuizPanel.open({
      panelId: 'review',
      title: 'Review',
      intro: `${cards.length} questions from earlier lessons. What you miss comes back tomorrow; what you get right comes back later.`,
      questions: cards.map(c => c.question),
      doneLabel: 'Done',
      onGraded(_correct, latest) {
        progress.recordReview(latest.map(r => ({ key: cards[r.index].key, ok: r.ok })));
        return { passed: true };
      },
      onDone(panel) { panel.dispose(); },
    });
  }

  private static open(session: QuizSession) {
    const existing = QuizPanel.panels.get(session.panelId);
    if (existing) { existing.reveal(); return; }

    const panel = vscode.window.createWebviewPanel('learnLab.quiz', session.title, vscode.ViewColumn.Active, { enableScripts: true, retainContextWhenHidden: true });
    QuizPanel.panels.set(session.panelId, panel);
    panel.onDidDispose(() => QuizPanel.panels.delete(session.panelId));

    const correct = new Set<number>();
    const all = session.questions.map((_, i) => i);
    panel.webview.html = QuizPanel.render(session, all);

    panel.webview.onDidReceiveMessage((msg: { type: string; answers?: Record<string, unknown> }) => {
      if (msg.type === 'submit' && msg.answers) {
        const results = Object.entries(msg.answers).map(([i, a]) => ({ index: Number(i), ok: isCorrect(session.questions[Number(i)], a) }));
        for (const r of results) if (r.ok) correct.add(r.index);
        const { passed } = session.onGraded(correct, results);
        panel.webview.postMessage({
          type: 'graded', score: correct.size, total: session.questions.length, passed,
          doneLabel: session.doneLabel,
          missed: all.filter(i => !correct.has(i)).length,
          results: results.map(r => {
            const q = session.questions[r.index];
            return { i: r.index, ok: r.ok, type: q.type, want: q.answer, explanation: md(q.explanation ?? '') };
          }),
        });
      }
      if (msg.type === 'retry') panel.webview.html = QuizPanel.render(session, all.filter(i => !correct.has(i)));
      if (msg.type === 'next') session.onDone(panel);
    });
  }

  private static render(session: QuizSession, indices: number[]): string {
    const qs = indices.map(i => {
      const q = session.questions[i];
      const options = q.type === 'tf'
        ? ['True', 'False'].map((o, j) => `<label data-q="${i}" data-o="${j}"><input type="radio" name="q${i}" value="${j === 0}"> ${o}</label>`)
        : shuffled((q.options ?? []).map((o, j) => ({ o, j }))).map(({ o, j }) => `<label data-q="${i}" data-o="${j}"><input type="${q.type === 'multi' ? 'checkbox' : 'radio'}" name="q${i}" value="${j}"> ${inner(o)}</label>`);
      return `<div class="q" id="q${i}">
        <div><span class="num">Q${i + 1}.</span> ${md(q.question)}</div>
        ${q.code ? `<pre><code>${highlight(q.code, 'csharp')}</code></pre>` : ''}
        ${q.type === 'multi' ? '<div class="num">(select all that apply)</div>' : ''}
        ${options.join('')}
        <div class="expl" id="e${i}" hidden></div>
      </div>`;
    }).join('');

    const intro = indices.length === session.questions.length ? session.intro : `Retrying the ${indices.length} you missed.`;
    const body = `<h1>${esc(session.title)}</h1><p class="num">${esc(intro)}</p><div id="result"></div>${qs}
      <button id="submit">Submit answers</button>`;

    const script = `
      const vscode = acquireVsCodeApi();
      const shown = ${JSON.stringify(indices.map(i => ({ i, type: session.questions[i].type })))};
      document.getElementById('submit').addEventListener('click', () => {
        const answers = {};
        let missing = -1;
        for (const { i, type } of shown) {
          const els = [...document.querySelectorAll('input[name="q' + i + '"]:checked')];
          if (!els.length) { if (missing < 0) missing = i; continue; }
          answers[i] = type === 'tf' ? els[0].value === 'true' : type === 'multi' ? els.map(e => Number(e.value)) : Number(els[0].value);
        }
        if (missing >= 0) {
          const q = document.getElementById('q' + missing);
          q.scrollIntoView({ behavior: 'smooth' });
          q.style.borderColor = 'var(--vscode-editorWarning-foreground)';
          return;
        }
        vscode.postMessage({ type: 'submit', answers });
      });
      window.addEventListener('message', ev => {
        const m = ev.data; if (m.type !== 'graded') return;
        m.results.forEach(r => {
          const q = document.getElementById('q' + r.i); q.classList.add(r.ok ? 'correct' : 'wrong');
          q.querySelectorAll('label').forEach(l => {
            const o = Number(l.dataset.o), inp = l.querySelector('input'); inp.disabled = true;
            const isRight = r.type === 'tf' ? (o === 0) === r.want : Array.isArray(r.want) ? r.want.includes(o) : o === r.want;
            if (isRight) l.classList.add('right'); else if (inp.checked) l.classList.add('picked-wrong');
          });
          const e = document.getElementById('e' + r.i); if (r.explanation) { e.innerHTML = r.explanation; e.hidden = false; }
        });
        document.getElementById('submit').remove();
        const res = document.getElementById('result');
        res.innerHTML = '<div class="score">Score: ' + m.score + ' / ' + m.total + (m.missed === 0 ? ', all correct' : m.passed ? ', passed' : '') + '</div>'
          + (m.passed ? '<button id="next"></button>' : '')
          + (m.missed && m.doneLabel !== 'Done' ? '<button id="retry" class="' + (m.passed ? 'secondary' : '') + '">Retry ' + m.missed + ' missed</button>' : '');
        const n = document.getElementById('next'); if (n) { n.textContent = m.doneLabel; n.addEventListener('click', () => vscode.postMessage({ type: 'next' })); }
        const r = document.getElementById('retry'); if (r) r.addEventListener('click', () => vscode.postMessage({ type: 'retry' }));
        window.scrollTo({ top: 0, behavior: 'smooth' });
      });`;
    return shell(session.title, body, script);
  }
}

// ---------- Review ----------

const REVIEW_SIZE = 5;

function pickReview(courses: Course[], progress: ProgressStore): { key: string; question: Question }[] {
  const quizzes = allItems(courses).filter((i): i is QuizItem => i.type === 'quiz' && Boolean(progress.get(i.id)?.total));
  const pool = new Map<string, Question>();
  for (const item of quizzes) readQuiz(item).questions.forEach((question, n) => pool.set(`${item.id}#${n}`, question));

  const due = progress.dueReviewKeys().filter(key => pool.has(key));
  const fresh = shuffled([...pool.keys()].filter(key => !progress.reviewCard(key)));
  return [...due, ...fresh].slice(0, REVIEW_SIZE).map(key => ({ key, question: pool.get(key)! }));
}

// ---------- Hints ----------

export function parseHints(file: string): string[] {
  const text = fs.readFileSync(file, 'utf8');
  return text.split(/^##\s+.*$/m).map(s => s.trim()).filter(Boolean);
}

export class HintsPanel {
  private static panel: vscode.WebviewPanel | undefined;
  private static itemId: string | undefined;

  static show(ex: ExerciseItem, hints: string[], unlocked: number) {
    if (!HintsPanel.panel || HintsPanel.itemId !== ex.id) {
      HintsPanel.panel?.dispose();
      HintsPanel.panel = vscode.window.createWebviewPanel('learnLab.hints', `Hints: ${ex.lesson.title}`, vscode.ViewColumn.Beside, { enableScripts: true, retainContextWhenHidden: true });
      HintsPanel.itemId = ex.id;
      HintsPanel.panel.onDidDispose(() => { HintsPanel.panel = undefined; HintsPanel.itemId = undefined; });
      HintsPanel.panel.webview.onDidReceiveMessage((m: { type: string }) => {
        if (m.type === 'more') vscode.commands.executeCommand('learnLab.hint', ex);
      });
    }
    const p = HintsPanel.panel;
    const shown = hints.slice(0, unlocked).map((h, i) => `<div class="hint"><strong>Hint ${i + 1} of ${hints.length}</strong>${md(h)}</div>`).join('');
    const rest = unlocked < hints.length
      ? `<p class="locked">${hints.length - unlocked} more hint${hints.length - unlocked > 1 ? 's' : ''} available.</p><button id="more" class="secondary">Reveal next hint</button>`
      : `<p class="locked">That was the last hint. Still stuck? Right-click the exercise → <em>Show Solution</em>.</p>`;
    p.webview.html = shell('Hints', `<h1>Hints — ${esc(ex.lesson.title)}</h1><p class="num">Try for a few minutes before each hint. Hints used are recorded, and that is fine.</p>${shown}${rest}`,
      `const vscode = acquireVsCodeApi(); const b = document.getElementById('more'); if (b) b.addEventListener('click', () => vscode.postMessage({ type: 'more' }));`);
    p.reveal(vscode.ViewColumn.Beside, true);
  }
}
