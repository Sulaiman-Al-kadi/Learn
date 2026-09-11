import * as fs from 'fs';
import * as vscode from 'vscode';
import { marked } from 'marked';
import { QuizItem, ExerciseItem } from './model';
import { ProgressStore } from './progress';

// ---------- quiz.json schema ----------
// {
//   "title": "...",
//   "questions": [
//     { "type": "mcq",   "question": "markdown", "code": "optional csharp", "options": ["a","b"], "answer": 1, "explanation": "markdown" },
//     { "type": "multi", ...same, "answer": [0, 2] },
//     { "type": "tf",    "question": "...", "answer": true, "explanation": "..." }
//   ]
// }
interface Question {
  type: 'mcq' | 'multi' | 'tf';
  question: string;
  code?: string;
  options?: string[];
  answer: number | number[] | boolean;
  explanation?: string;
}
interface Quiz { title?: string; questions: Question[]; }

const md = (s: string) => marked.parse(s, { async: false }) as string;
const esc = (s: string) => s.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');

function nonce() { return Math.random().toString(36).slice(2) + Date.now().toString(36); }

function shell(webview: vscode.Webview, title: string, body: string, script = ''): string {
  const n = nonce();
  return `<!DOCTYPE html><html><head><meta charset="UTF-8">
<meta http-equiv="Content-Security-Policy" content="default-src 'none'; style-src 'unsafe-inline'; script-src 'nonce-${n}';">
<title>${esc(title)}</title>
<style>
  body { font-family: var(--vscode-font-family); font-size: var(--vscode-font-size); color: var(--vscode-foreground); padding: 0 24px 40px; max-width: 860px; line-height: 1.55; }
  h1 { font-size: 1.5em; border-bottom: 1px solid var(--vscode-panel-border); padding-bottom: 6px; }
  h2 { font-size: 1.2em; margin-top: 1.6em; }
  pre, code { font-family: var(--vscode-editor-font-family); font-size: var(--vscode-editor-font-size); }
  pre { background: var(--vscode-textCodeBlock-background); padding: 10px 14px; border-radius: 4px; overflow-x: auto; }
  code { background: var(--vscode-textCodeBlock-background); padding: 1px 4px; border-radius: 3px; }
  pre code { background: none; padding: 0; }
  .q { border: 1px solid var(--vscode-panel-border); border-radius: 6px; padding: 12px 16px; margin: 16px 0; }
  .q.correct { border-color: var(--vscode-testing-iconPassed); }
  .q.wrong { border-color: var(--vscode-testing-iconFailed); }
  .num { color: var(--vscode-descriptionForeground); font-weight: 600; }
  label { display: block; padding: 6px 8px; border-radius: 4px; cursor: pointer; }
  label:hover { background: var(--vscode-list-hoverBackground); }
  label.right { background: color-mix(in srgb, var(--vscode-testing-iconPassed) 20%, transparent); }
  label.picked-wrong { background: color-mix(in srgb, var(--vscode-testing-iconFailed) 20%, transparent); }
  .expl { margin-top: 10px; padding: 8px 12px; border-left: 3px solid var(--vscode-textLink-foreground); background: var(--vscode-textBlockQuote-background); }
  button { background: var(--vscode-button-background); color: var(--vscode-button-foreground); border: none; padding: 8px 18px; border-radius: 3px; cursor: pointer; font-size: 1em; margin-right: 8px; }
  button:hover { background: var(--vscode-button-hoverBackground); }
  button.secondary { background: var(--vscode-button-secondaryBackground); color: var(--vscode-button-secondaryForeground); }
  .score { font-size: 1.3em; font-weight: 600; margin: 20px 0; }
  .hint { border-left: 3px solid var(--vscode-editorLightBulb-foreground); padding: 4px 14px; margin: 14px 0; background: var(--vscode-textBlockQuote-background); }
  .locked { opacity: .6; font-style: italic; }
</style></head><body>${body}<script nonce="${n}">${script}</script></body></html>`;
}

// ---------- Quiz ----------

export class QuizPanel {
  private static panels = new Map<string, vscode.WebviewPanel>();

  static show(item: QuizItem, progress: ProgressStore) {
    const existing = QuizPanel.panels.get(item.id);
    if (existing) { existing.reveal(); return; }

    const quiz: Quiz = JSON.parse(fs.readFileSync(item.file, 'utf8'));
    const title = quiz.title ?? `Quiz: ${item.lesson.title}`;
    const panel = vscode.window.createWebviewPanel('learnLab.quiz', title, vscode.ViewColumn.Active, { enableScripts: true });
    QuizPanel.panels.set(item.id, panel);
    panel.onDidDispose(() => QuizPanel.panels.delete(item.id));

    panel.webview.html = QuizPanel.render(panel.webview, title, quiz);

    panel.webview.onDidReceiveMessage((msg: { type: string; answers?: unknown[] }) => {
      if (msg.type === 'submit' && msg.answers) {
        const { score, results } = QuizPanel.grade(quiz, msg.answers);
        progress.update(item.id, { score, total: quiz.questions.length, status: score === quiz.questions.length ? 'done' : 'attempted' });
        panel.webview.postMessage({ type: 'graded', score, results, explanations: quiz.questions.map(q => md(q.explanation ?? '')) });
      }
      if (msg.type === 'retry') panel.webview.html = QuizPanel.render(panel.webview, title, quiz);
      if (msg.type === 'next') vscode.commands.executeCommand('learnLab.next');
    });
  }

  private static grade(quiz: Quiz, answers: unknown[]) {
    const results = quiz.questions.map((q, i) => {
      const a = answers[i];
      if (q.type === 'tf') return a === q.answer;
      if (q.type === 'multi') {
        const want = [...(q.answer as number[])].sort().join(','), got = Array.isArray(a) ? [...a].sort().join(',') : '';
        return want === got;
      }
      return a === q.answer;
    });
    return { score: results.filter(Boolean).length, results };
  }

  private static render(webview: vscode.Webview, title: string, quiz: Quiz): string {
    const qs = quiz.questions.map((q, i) => {
      const opts = q.type === 'tf'
        ? ['True', 'False'].map((o, j) => `<label data-q="${i}" data-o="${j}"><input type="radio" name="q${i}" value="${j === 0}"> ${o}</label>`).join('')
        : (q.options ?? []).map((o, j) => `<label data-q="${i}" data-o="${j}"><input type="${q.type === 'multi' ? 'checkbox' : 'radio'}" name="q${i}" value="${j}"> ${md(o).replace(/^<p>|<\/p>\s*$/g, '')}</label>`).join('');
      return `<div class="q" id="q${i}">
        <div><span class="num">Q${i + 1}.</span> ${md(q.question)}</div>
        ${q.code ? `<pre><code>${esc(q.code)}</code></pre>` : ''}
        ${q.type === 'multi' ? '<div class="num">(select all that apply)</div>' : ''}
        ${opts}
        <div class="expl" id="e${i}" style="display:none"></div>
      </div>`;
    }).join('');

    const body = `<h1>${esc(title)}</h1><p class="num">${quiz.questions.length} questions. Answer all, then submit.</p>${qs}
      <div id="result"></div>
      <button id="submit">Submit answers</button>`;

    const script = `
      const vscode = acquireVsCodeApi();
      const types = ${JSON.stringify(quiz.questions.map(q => q.type))};
      const answersKey = ${JSON.stringify(quiz.questions.map(q => q.answer))};
      document.getElementById('submit').addEventListener('click', () => {
        const answers = types.map((t, i) => {
          const els = [...document.querySelectorAll('input[name="q' + i + '"]:checked')];
          if (t === 'tf') return els.length ? els[0].value === 'true' : null;
          if (t === 'multi') return els.map(e => Number(e.value));
          return els.length ? Number(els[0].value) : null;
        });
        const missing = answers.findIndex(a => a === null || (Array.isArray(a) && a.length === 0));
        if (missing >= 0) { document.getElementById('q' + missing).scrollIntoView({ behavior: 'smooth' }); document.getElementById('q' + missing).style.borderColor = 'var(--vscode-editorWarning-foreground)'; return; }
        vscode.postMessage({ type: 'submit', answers });
      });
      window.addEventListener('message', ev => {
        const m = ev.data; if (m.type !== 'graded') return;
        m.results.forEach((ok, i) => {
          const q = document.getElementById('q' + i); q.classList.add(ok ? 'correct' : 'wrong');
          const want = answersKey[i];
          q.querySelectorAll('label').forEach(l => {
            const o = Number(l.dataset.o); const inp = l.querySelector('input'); inp.disabled = true;
            const isRight = types[i] === 'tf' ? (o === 0) === want : Array.isArray(want) ? want.includes(o) : o === want;
            if (isRight) l.classList.add('right'); else if (inp.checked) l.classList.add('picked-wrong');
          });
          const e = document.getElementById('e' + i); if (m.explanations[i]) { e.innerHTML = m.explanations[i]; e.style.display = 'block'; }
        });
        document.getElementById('submit').remove();
        const all = m.score === m.results.length;
        document.getElementById('result').innerHTML =
          '<div class="score">Score: ' + m.score + ' / ' + m.results.length + (all ? ' — perfect!' : '') + '</div>' +
          (all ? '<button id="next">Continue to next item →</button>' : '<button id="retry">Try again</button>') ;
        const n = document.getElementById('next'); if (n) n.addEventListener('click', () => vscode.postMessage({ type: 'next' }));
        const r = document.getElementById('retry'); if (r) r.addEventListener('click', () => vscode.postMessage({ type: 'retry' }));
        window.scrollTo({ top: 0, behavior: 'smooth' });
      });`;
    return shell(webview, title, body, script);
  }
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
      HintsPanel.panel = vscode.window.createWebviewPanel('learnLab.hints', `Hints: ${ex.lesson.title}`, vscode.ViewColumn.Beside, { enableScripts: true });
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
    p.webview.html = shell(p.webview, 'Hints', `<h1>Hints — ${esc(ex.lesson.title)}</h1><p class="num">Try for a few minutes before each hint. Hints used are recorded, and that is fine.</p>${shown}${rest}`,
      `const vscode = acquireVsCodeApi(); const b = document.getElementById('more'); if (b) b.addEventListener('click', () => vscode.postMessage({ type: 'more' }));`);
    p.reveal(vscode.ViewColumn.Beside, true);
  }
}
