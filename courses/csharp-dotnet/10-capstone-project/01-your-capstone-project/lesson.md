# Module 10 — Capstone Project

No new concepts. This is where you build something of your own choosing, using everything from Modules 1–9, and finish with the same artifacts a real engineering team produces: a working system, a README someone else could actually use, and a written reflection on what you learned. This matches the university plan's Week 8 deliverable exactly.

There's no `Tests.cs` oracle for this one — the "check" is a working system you can demo, plus the checklist at the bottom.

## Pick a scope

Two suggestions from the plan (pick one, or propose your own of similar size):

**Booking system** — rooms/resources/appointments that can be reserved, with conflict checking (can't double-book the same slot).

**Attendance dashboard** — track members/students and their attendance over time, with summary statistics (attendance rate, most/least present, trends).

Keep it **small but real**. A working MVP (Minimum Viable Product) covering one thing properly beats an ambitious system that's half-built. If you're unsure it's small enough, it probably isn't yet — cut scope until you're confident you can finish it.

## Suggested architecture (reuse everything)

| Piece | From |
|---|---|
| Domain classes/records, SOLID-structured services | Module 2 |
| LINQ for any reporting/summary logic | Module 3 |
| Minimal API or MVC controllers | Module 5 |
| EF Core + SQLite for real persistence | Module 6 |
| JWT auth, if your app has real users | Module 7 |
| A background worker, if something should run on a schedule (e.g. daily attendance summary, reminder emails) | Module 8 |
| Integration tests via `WebApplicationFactory` | Module 9 |

You don't have to use every single piece — pick what the actual scope genuinely needs. A booking system needs persistence and probably conflict-checking logic; it may not need a background worker. Don't force in a module's technique just to check a box.

## Suggested 2-week sprint structure (matches the plan's "Sprint Planning & Daily Scrum")

| Days | Focus |
|---|---|
| 1 | Write the scope down: what's in, what's explicitly out. Design your entities and API surface (which endpoints, what they do) on paper first. |
| 2–3 | Domain classes + EF Core setup + migrations/`EnsureCreated`. Get the data model right before building on top of it. |
| 4–6 | Core endpoints (CRUD + your app's one genuinely interesting piece of logic — conflict checking, attendance rate calculation, whatever makes this *your* app and not a generic CRUD template). |
| 7 | Tests — integration tests for your core endpoints (Module 9). |
| 8 | Auth, if applicable. |
| 9 | README + cleanup — this is not an afterthought, budget real time for it. |
| 10 | Buffer / polish / your learning report. |

Even solo, write yourself a short daily note: what you did, what's next, anything blocking you. This is the "Daily Scrum" habit in miniature — it's what makes the eventual learning report easy to write, instead of trying to reconstruct two weeks from memory at the end.

## Git workflow — code review & pull requests

Even working alone, use the real workflow:
1. One feature = one branch = one PR, even to yourself.
2. Write a real PR description: what changed, why, how to test it.
3. Before merging, re-read your own diff as if reviewing someone else's code — you will find things.
4. Small, focused commits with clear messages, not one giant "did stuff" commit at the end.

If anyone else can review your PRs (a peer, a mentor), ask for it — a second pair of eyes is the entire point of code review, and it's worth far more than reviewing your own work in isolation.

## The README (required deliverable)

A README someone unfamiliar with your project could use to actually run it. At minimum:
- What the project does, in 2–3 sentences.
- How to run it (`dotnet run`, connection strings/config needed, seed data if any).
- API surface — endpoints, methods, what they do (or a link to the Swagger UI from Module 7).
- Architecture — a short paragraph or diagram of how the pieces fit together.
- What you'd do next if you kept building this.

## The individual learning report (required deliverable)

Not a project retrospective — a **learning** retrospective. Answer, specifically:
- What C#/.NET concepts from this course did you actually use, and how?
- What was hardest, and what finally made it click?
- What would you do differently if you started over?
- What's the next thing you want to learn that this course didn't cover?

Vague answers ("I learned a lot") aren't useful to you later. Specific ones ("I finally understood why DbContext needs Scoped lifetime when my background worker crashed trying to reuse one across requests") are — they're the kind of thing that comes up naturally in the technical interview the plan's evaluation includes.

## Definition of done
- [ ] Scope written down, deliberately small
- [ ] Working MVP — you can demo it start to finish
- [ ] Persisted data (EF Core), not just in-memory
- [ ] At least a few integration tests covering the core behavior
- [ ] README complete enough for a stranger to run it
- [ ] Git history shows real branches/PRs/commits, not one dump
- [ ] Individual learning report written
- [ ] You can explain every design decision if asked in an interview

## Final evaluation (per the plan)
Project + a 30-minute individual technical interview. Go back through every module's quiz and make sure you can explain *why* each answer is right out loud, not just recognize it multiple-choice style — that's the actual bar a technical interview sets. Good luck — you started this course at `Console.WriteLine("Hello, world!")`.
