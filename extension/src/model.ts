import * as fs from 'fs';
import * as path from 'path';

// ---------- Content model ----------
// Convention over configuration: the folder layout IS the course.
//
// courses/<course>/course.json            { title, language, description? }
// courses/<course>/<module>/module.json   { title? }            (optional)
// courses/<course>/<module>/<lesson>/
//     lesson.json                          { title? }            (optional)
//     lesson.md                            -> Lesson item
//     quiz.json                            -> Quiz item
//     exercise*/                           -> Exercise item(s)
//         meta.json  { title?, check: "output"|"test", student: ["Program.cs"], input?, expected? }
//         starter/   copies of student files (for Reset)
//         solution/  reference solution     (for Show Solution)
//         hints.md   "## Hint 1" ... sections

export type ItemType = 'lesson' | 'quiz' | 'exercise';

export interface Course {
  id: string;
  dir: string;
  title: string;
  language: string; // runner key, e.g. "dotnet"
  description?: string;
  modules: Module[];
}

export interface Module {
  id: string;
  dir: string;
  title: string;
  course: Course;
  lessons: Lesson[];
}

export interface Lesson {
  id: string;
  dir: string;
  title: string;
  module: Module;
  items: Item[];
}

export interface ItemBase {
  id: string; // globally unique: course/module/lesson/kind
  type: ItemType;
  title: string;
  lesson: Lesson;
}

export interface LessonItem extends ItemBase { type: 'lesson'; file: string; }
export interface QuizItem extends ItemBase { type: 'quiz'; file: string; }
export interface ExerciseItem extends ItemBase {
  type: 'exercise';
  dir: string;
  check: 'output' | 'test';
  studentFiles: string[]; // relative to dir
  inputFile?: string;     // stdin for output mode
  expectedFile?: string;  // expected stdout for output mode
  hintsFile?: string;
  solutionDir?: string;
  starterDir?: string;
}
export type Item = LessonItem | QuizItem | ExerciseItem;

// ---------- Loading ----------

function readJson<T>(file: string, fallback: T): T {
  try { return JSON.parse(fs.readFileSync(file, 'utf8')) as T; } catch { return fallback; }
}

function titleFromFolder(name: string): string {
  // "02-variables-and-types" -> "02 Variables And Types"
  return name.split(/[-_]/).map(w => (w ? w[0].toUpperCase() + w.slice(1) : w)).join(' ');
}

function sortedDirs(dir: string): string[] {
  if (!fs.existsSync(dir)) return [];
  return fs.readdirSync(dir, { withFileTypes: true })
    .filter(d => d.isDirectory() && !d.name.startsWith('.') && !d.name.startsWith('_'))
    .map(d => d.name)
    .sort();
}

export function loadCourses(coursesRoot: string): Course[] {
  return sortedDirs(coursesRoot)
    .filter(id => fs.existsSync(path.join(coursesRoot, id, 'course.json')))
    .map(id => loadCourse(path.join(coursesRoot, id), id));
}

function loadCourse(dir: string, id: string): Course {
  const meta = readJson<{ title?: string; language?: string; description?: string }>(path.join(dir, 'course.json'), {});
  const course: Course = {
    id, dir,
    title: meta.title ?? titleFromFolder(id),
    language: meta.language ?? 'dotnet',
    description: meta.description,
    modules: [],
  };
  course.modules = sortedDirs(dir).map(mid => loadModule(path.join(dir, mid), mid, course));
  return course;
}

function loadModule(dir: string, id: string, course: Course): Module {
  const meta = readJson<{ title?: string }>(path.join(dir, 'module.json'), {});
  const mod: Module = { id, dir, title: meta.title ?? titleFromFolder(id), course, lessons: [] };
  mod.lessons = sortedDirs(dir).map(lid => loadLesson(path.join(dir, lid), lid, mod));
  return mod;
}

interface ExerciseMeta { title?: string; check?: 'output' | 'test'; student?: string[]; input?: string; expected?: string; }

function loadLesson(dir: string, id: string, module: Module): Lesson {
  const meta = readJson<{ title?: string }>(path.join(dir, 'lesson.json'), {});
  const lesson: Lesson = { id, dir, title: meta.title ?? titleFromFolder(id), module, items: [] };
  const base = `${module.course.id}/${module.id}/${id}`;

  const lessonMd = path.join(dir, 'lesson.md');
  if (fs.existsSync(lessonMd)) {
    lesson.items.push({ id: `${base}/lesson`, type: 'lesson', title: 'Read the lesson', lesson, file: lessonMd });
  }

  const quiz = path.join(dir, 'quiz.json');
  if (fs.existsSync(quiz)) {
    lesson.items.push({ id: `${base}/quiz`, type: 'quiz', title: 'Quiz', lesson, file: quiz });
  }

  for (const ex of sortedDirs(dir).filter(d => d.startsWith('exercise'))) {
    const exDir = path.join(dir, ex);
    const m = readJson<ExerciseMeta>(path.join(exDir, 'meta.json'), {});
    const has = (f: string) => (fs.existsSync(path.join(exDir, f)) ? path.join(exDir, f) : undefined);
    lesson.items.push({
      id: `${base}/${ex}`,
      type: 'exercise',
      title: m.title ?? (ex === 'exercise' ? 'Exercise' : titleFromFolder(ex)),
      lesson,
      dir: exDir,
      check: m.check ?? (has('expected.txt') ? 'output' : 'test'),
      studentFiles: m.student ?? ['Program.cs'],
      inputFile: m.input ? path.join(exDir, m.input) : has('input.txt'),
      expectedFile: m.expected ? path.join(exDir, m.expected) : has('expected.txt'),
      hintsFile: has('hints.md'),
      solutionDir: has('solution'),
      starterDir: has('starter'),
    });
  }
  return lesson;
}

// Flatten all items in course order (used for "Next")
export function allItems(courses: Course[]): Item[] {
  const out: Item[] = [];
  for (const c of courses) for (const m of c.modules) for (const l of m.lessons) out.push(...l.items);
  return out;
}

export function findExerciseForFile(courses: Course[], filePath: string): ExerciseItem | undefined {
  const norm = path.resolve(filePath).toLowerCase();
  for (const it of allItems(courses)) {
    if (it.type === 'exercise' && norm.startsWith(path.resolve(it.dir).toLowerCase() + path.sep)) return it;
  }
  return undefined;
}
