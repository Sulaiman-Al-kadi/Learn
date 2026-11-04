import { Marked } from 'marked';
import hljs from 'highlight.js/lib/core';
import csharp from 'highlight.js/lib/languages/csharp';
import json from 'highlight.js/lib/languages/json';
import yaml from 'highlight.js/lib/languages/yaml';
import powershell from 'highlight.js/lib/languages/powershell';
import dockerfile from 'highlight.js/lib/languages/dockerfile';

hljs.registerLanguage('csharp', csharp);
hljs.registerLanguage('json', json);
hljs.registerLanguage('yaml', yaml);
hljs.registerLanguage('powershell', powershell);
hljs.registerLanguage('dockerfile', dockerfile);

export const esc = (s: string) => s.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');

export function highlight(code: string, lang?: string): string {
  return lang && hljs.getLanguage(lang) ? hljs.highlight(code, { language: lang }).value : esc(code);
}

const marked = new Marked({
  renderer: {
    code({ text, lang }) {
      const language = (lang ?? '').trim().split(/\s+/)[0];
      return `<pre data-lang="${esc(language)}"><code>${highlight(text, language)}</code></pre>`;
    },
  },
});

export const md = (s: string) => (marked.parse(s, { async: false }) as string)
  .replace(/<table>/g, '<div class="table"><table>').replace(/<\/table>/g, '</table></div>');

export function nonce() { return Math.random().toString(36).slice(2) + Date.now().toString(36); }

// Webviews get theme colors as CSS variables but not the editor's token colors,
// so code is mapped onto the theme's debug-expression palette, which every theme defines for light and dark.
export const CODE_CSS = `
  .hljs-keyword, .hljs-built_in, .hljs-literal, .hljs-meta { color: var(--vscode-debugTokenExpression-boolean); }
  .hljs-string, .hljs-char, .hljs-attr + .hljs-string { color: var(--vscode-debugTokenExpression-string); }
  .hljs-number { color: var(--vscode-debugTokenExpression-number); }
  .hljs-title, .hljs-type, .hljs-class, .hljs-attr { color: var(--vscode-debugTokenExpression-name); }
  .hljs-comment { color: var(--vscode-descriptionForeground); font-style: italic; }
  .hljs-subst { color: var(--vscode-foreground); }
`;
