import * as fs from 'fs';
import * as path from 'path';
import * as vscode from 'vscode';
import { Item, LessonItem, ExerciseItem } from './model';
import { ProgressStore, Status } from './progress';
import { CODE_CSS, esc, md, nonce } from './render';

const icon = (body: string) => `<svg viewBox="0 0 16 16" width="16" height="16" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">${body}</svg>`;
const ICONS = {
  done: icon('<circle cx="8" cy="8" r="6.25"/><path d="m5.2 8.2 2 2 3.6-4.2"/>'),
  started: icon('<circle cx="8" cy="8" r="6.25"/><path d="M8 1.75a6.25 6.25 0 0 1 0 12.5z" fill="currentColor" stroke="none"/>'),
  todo: icon('<circle cx="8" cy="8" r="6.25"/>'),
  arrow: icon('<path d="M3 8h10M9 4l4 4-4 4"/>'),
  copy: icon('<rect x="5.5" y="5.5" width="8" height="8" rx="1.5"/><path d="M10.5 5.5v-2A1.5 1.5 0 0 0 9 2H3.5A1.5 1.5 0 0 0 2 3.5V9a1.5 1.5 0 0 0 1.5 1.5h2"/>'),
};

const STATUS_LABEL: Record<Status, string> = { done: 'done', attempted: 'in progress', todo: 'not started' };
const stepName = (item: Item) => (item.type === 'lesson' ? 'Lesson' : item.type === 'quiz' ? 'Quiz' : item.title.replace(/^Exercise:\s*/, ''));
const nextName = (from: Item, item: Item) => {
  if (item.lesson !== from.lesson) return `the next lesson: ${item.lesson.title}`;
  return item.type === 'quiz' ? 'the quiz' : item.type === 'lesson' ? 'the lesson' : `the exercise: ${stepName(item)}`;
};

type PageItem = LessonItem | ExerciseItem;

export interface LessonContext {
  progress: ProgressStore;
  nextAfter(item: Item): Item | undefined;
}

// The reading surface for lesson.md and exercise README.md: one column, with the lesson's steps always in view.
export class LessonPanel {
  private static panel: vscode.WebviewPanel | undefined;
  private static item: PageItem | undefined;
  private static ctx: LessonContext;

  static show(item: PageItem, ctx: LessonContext, column: vscode.ViewColumn) {
    LessonPanel.ctx = ctx;
    const file = item.type === 'lesson' ? item.file : path.join(item.dir, 'README.md');
    const title = item.type === 'lesson' ? item.lesson.title : stepName(item);
    if (!LessonPanel.panel) {
      const created = vscode.window.createWebviewPanel('learnLab.lesson', title, column, { enableScripts: true, retainContextWhenHidden: true });
      LessonPanel.panel = created;
      const sub = ctx.progress.onDidChange(() => LessonPanel.refresh());
      created.onDidDispose(() => { LessonPanel.panel = undefined; LessonPanel.item = undefined; sub.dispose(); });
      created.webview.onDidReceiveMessage(LessonPanel.onMessage);
    }
    const panel = LessonPanel.panel;
    LessonPanel.item = item;
    panel.title = title;
    panel.webview.html = LessonPanel.render(item, fs.readFileSync(file, 'utf8'));
    panel.reveal(column);
  }

  private static onMessage(m: { type: string; id?: string; text?: string }) {
    const item = LessonPanel.item;
    if (!item) return;
    if (m.type === 'open') {
      const target = item.lesson.items.find(i => i.id === m.id);
      if (target) vscode.commands.executeCommand('learnLab.open', target);
    }
    if (m.type === 'continue') {
      if (item.type === 'lesson') LessonPanel.ctx.progress.update(item.id, { status: 'done' });
      vscode.commands.executeCommand('learnLab.next');
    }
    if (m.type === 'check') vscode.commands.executeCommand('learnLab.check', item);
    if (m.type === 'hint') vscode.commands.executeCommand('learnLab.hint', item);
    if (m.type === 'copy' && m.text !== undefined) vscode.env.clipboard.writeText(m.text);
  }

  private static refresh() {
    const item = LessonPanel.item;
    if (!item) return;
    LessonPanel.panel?.webview.postMessage({ type: 'chrome', steps: LessonPanel.steps(item), actions: LessonPanel.actions(item) });
  }

  private static steps(current: PageItem): string {
    const { progress } = LessonPanel.ctx;
    return current.lesson.items.map(i => {
      const status = progress.status(i.id);
      const glyph = status === 'done' ? ICONS.done : status === 'attempted' ? ICONS.started : ICONS.todo;
      const here = i.id === current.id;
      return `<button class="step ${status}" data-open="${esc(i.id)}"${here ? ' aria-current="step"' : ''} aria-label="${esc(stepName(i))}, ${STATUS_LABEL[status]}">${glyph}<span>${esc(stepName(i))}</span></button>`;
    }).join('');
  }

  private static actions(item: PageItem): string {
    const { progress, nextAfter } = LessonPanel.ctx;
    const next = nextAfter(item);
    const target = next ? esc(nextName(item, next)) : '';
    if (item.type === 'lesson') {
      const read = progress.status(item.id) === 'done';
      const label = read ? (next ? `Continue to ${target}` : 'Finish') : next ? `Mark as read and continue to ${target}` : 'Mark as read';
      return `<button class="primary" data-act="continue">${label}${ICONS.arrow}</button>`;
    }
    if (progress.status(item.id) === 'done') {
      return `<button class="primary" data-act="continue">${next ? `Continue to ${target}` : 'Finish'}${ICONS.arrow}</button><button data-act="check">Check again</button>`;
    }
    return `<button class="primary" data-act="check">Check my code</button>${item.hintsFile ? '<button data-act="hint">Show a hint</button>' : ''}`;
  }

  private static render(item: PageItem, source: string): string {
    const n = nonce();
    const where = `${item.lesson.module.title}  /  ${item.lesson.title}`;
    return `<!DOCTYPE html><html lang="en"><head><meta charset="UTF-8">
<meta http-equiv="Content-Security-Policy" content="default-src 'none'; style-src 'unsafe-inline'; script-src 'nonce-${n}';">
<title>${esc(item.lesson.title)}</title><style>${CSS}${CODE_CSS}</style></head><body>
<div class="bar" aria-hidden="true"><span id="bar"></span></div>
<header><p class="where">${esc(where)}</p><nav id="steps" aria-label="Steps in this lesson">${LessonPanel.steps(item)}</nav></header>
<main><article>${md(source)}</article><footer id="actions">${LessonPanel.actions(item)}</footer></main>
<script nonce="${n}">${SCRIPT.replace('__ID__', JSON.stringify(item.id)).replace('__COPY__', JSON.stringify(ICONS.copy))}</script></body></html>`;
  }
}

const SCRIPT = `
  const vscode = acquireVsCodeApi();
  const id = __ID__;
  const COPY = __COPY__;
  const root = document.documentElement, bar = document.getElementById('bar');
  const state = vscode.getState() || { scroll: {} };

  document.querySelectorAll('pre').forEach(pre => {
    const b = document.createElement('button');
    b.className = 'copy'; b.innerHTML = COPY; b.setAttribute('aria-label', 'Copy code'); b.title = 'Copy code';
    b.addEventListener('click', () => {
      vscode.postMessage({ type: 'copy', text: pre.querySelector('code').textContent });
      b.classList.add('copied'); b.title = 'Copied';
      setTimeout(() => { b.classList.remove('copied'); b.title = 'Copy code'; }, 1200);
    });
    pre.appendChild(b);
  });

  document.addEventListener('click', e => {
    const open = e.target.closest('[data-open]'), act = e.target.closest('[data-act]');
    if (open) vscode.postMessage({ type: 'open', id: open.dataset.open });
    if (act) vscode.postMessage({ type: act.dataset.act });
  });

  window.addEventListener('message', e => {
    if (e.data.type !== 'chrome') return;
    document.getElementById('steps').innerHTML = e.data.steps;
    document.getElementById('actions').innerHTML = e.data.actions;
  });

  function onScroll() {
    const range = root.scrollHeight - root.clientHeight;
    bar.style.transform = 'scaleX(' + (range > 0 ? Math.min(root.scrollTop / range, 1) : 1) + ')';
  }
  let timer;
  window.addEventListener('scroll', () => {
    onScroll();
    clearTimeout(timer);
    timer = setTimeout(() => { state.scroll[id] = root.scrollTop; vscode.setState(state); }, 200);
  }, { passive: true });
  root.scrollTop = state.scroll[id] || 0;
  onScroll();
`;

const CSS = `
  :root { --gap: 1.75rem; --quiet: var(--vscode-descriptionForeground); --line: var(--vscode-widget-border, var(--vscode-panel-border)); }
  * { box-sizing: border-box; }
  html { scroll-padding-top: 5rem; }
  body { margin: 0; padding: 0; color: var(--vscode-foreground); font-family: var(--vscode-font-family); font-size: calc(var(--vscode-font-size) * 1.15); line-height: 1.7; }
  ::selection { background: var(--vscode-editor-selectionBackground); }
  ::-webkit-scrollbar { width: 10px; height: 10px; }
  ::-webkit-scrollbar-thumb { background: var(--vscode-scrollbarSlider-background); }
  ::-webkit-scrollbar-thumb:hover { background: var(--vscode-scrollbarSlider-hoverBackground); }
  :focus-visible { outline: 1px solid var(--vscode-focusBorder); outline-offset: 2px; }

  .bar { position: fixed; inset: 0 0 auto; height: 2px; z-index: 2; }
  .bar span { display: block; height: 100%; background: var(--vscode-progressBar-background); transform: scaleX(0); transform-origin: 0 50%; }

  header { position: sticky; top: 0; z-index: 1; display: flex; flex-wrap: wrap; align-items: center; justify-content: space-between; gap: .25rem 1.5rem; padding: .7rem 1.5rem; background: var(--vscode-editor-background); border-bottom: 1px solid var(--line); font-size: var(--vscode-font-size); }
  .where { margin: 0; color: var(--quiet); white-space: pre-wrap; }
  nav { display: flex; flex-wrap: wrap; gap: .25rem; }
  .step { display: inline-flex; align-items: center; gap: .4rem; padding: .25rem .6rem; color: var(--quiet); background: none; border: 0; border-radius: 4px; font: inherit; cursor: pointer; }
  .step:hover { background: var(--vscode-list-hoverBackground); color: var(--vscode-foreground); }
  .step.done svg { color: var(--vscode-testing-iconPassed); }
  .step[aria-current] { color: var(--vscode-foreground); background: var(--vscode-list-inactiveSelectionBackground); font-weight: 600; }

  main { max-width: calc(72ch + 3rem); margin: 0 auto; padding: 2.5rem 1.5rem 5rem; }
  h1, h2, h3 { line-height: 1.25; font-weight: 600; text-wrap: balance; }
  h1 { margin: 0 0 1.25rem; font-size: 2em; letter-spacing: -.015em; }
  h2 { margin: 3rem 0 .75rem; font-size: 1.4em; }
  h3 { margin: 2.25rem 0 .5rem; font-size: 1.12em; }
  p, ul, ol { margin: 0 0 1.1rem; }
  ul, ol { padding-left: 1.4rem; }
  li { margin-bottom: .4rem; }
  li::marker { color: var(--quiet); }
  a { color: var(--vscode-textLink-foreground); text-underline-offset: 3px; }
  strong { font-weight: 600; }
  hr { margin: 2.5rem 0; border: 0; border-top: 1px solid var(--line); }
  blockquote { margin: 0 0 1.25rem; padding: .75rem 1rem; color: var(--vscode-foreground); background: var(--vscode-textBlockQuote-background); border-radius: 6px; }
  blockquote p:last-child { margin-bottom: 0; }

  code { font-family: var(--vscode-editor-font-family); font-size: .9em; font-variant-ligatures: none; }
  :not(pre) > code { padding: .1em .35em; background: var(--vscode-textCodeBlock-background); border-radius: 4px; }
  pre { position: relative; margin: 0 0 var(--gap); padding: .9rem 1.1rem; overflow-x: auto; background: var(--vscode-textCodeBlock-background); border: 1px solid var(--line); border-radius: 6px; line-height: 1.55; tab-size: 8; }
  @media (min-width: 900px) { pre { margin-inline: -2.5rem; } }
  .copy { position: absolute; top: .4rem; right: .4rem; display: grid; place-items: center; width: 1.8rem; height: 1.8rem; padding: 0; color: var(--quiet); background: var(--vscode-editor-background); border: 1px solid var(--line); border-radius: 4px; cursor: pointer; opacity: 0; }
  pre:hover .copy, .copy:focus-visible { opacity: 1; }
  .copy:hover { color: var(--vscode-foreground); }
  .copy.copied { color: var(--vscode-testing-iconPassed); opacity: 1; }

  .table { margin: 0 0 var(--gap); overflow-x: auto; }
  table { width: 100%; border-collapse: collapse; font-variant-numeric: tabular-nums; }
  th { text-align: left; font-weight: 600; }
  th, td { padding: .5rem .9rem .5rem 0; border-bottom: 1px solid var(--line); vertical-align: top; }

  footer { display: flex; flex-wrap: wrap; align-items: center; gap: .6rem; margin-top: 3.5rem; padding-top: 1.5rem; border-top: 1px solid var(--line); }
  footer button { display: inline-flex; align-items: center; gap: .5rem; padding: .55rem 1rem; color: var(--vscode-button-secondaryForeground); background: var(--vscode-button-secondaryBackground); border: 0; border-radius: 4px; font: inherit; cursor: pointer; }
  footer button:hover { background: var(--vscode-button-secondaryHoverBackground); }
  footer .primary { color: var(--vscode-button-foreground); background: var(--vscode-button-background); }
  footer .primary:hover { background: var(--vscode-button-hoverBackground); }
  @media (prefers-reduced-motion: no-preference) { .copy { transition: opacity .12s ease-out; } }
`;
