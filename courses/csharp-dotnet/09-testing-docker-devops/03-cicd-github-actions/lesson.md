# Module 9 Capstone — CI/CD with GitHub Actions (Guided Lab)

> Like lesson 02, this is a **guided lab** — running a real pipeline needs a real GitHub repository, which is outside what this local environment can execute. Read through, then follow the **Do this** steps with your own repo (from Week 1's Git lab, if you did the earlier course's Git setup — otherwise `git init` a new one).

## What CI/CD actually means

- **Continuous Integration (CI)**: every time code is pushed, automatically build it and run its tests — catching broken code immediately, before it reaches anyone else, rather than discovering it days later.
- **Continuous Deployment/Delivery (CD)**: automatically package and ship the working result (here, a Docker image, lesson 02) somewhere — a registry, a server — without a human manually repeating those steps every time.

Together: **every push is automatically built, tested, and (if everything passes) packaged** — exactly the university plan's Week 7 deliverable, "automated pipeline building, testing, and pushing image."

## A GitHub Actions workflow

Workflows live in `.github/workflows/*.yml` in your repository. Here's one that builds, tests, and (on `main`) builds and pushes a Docker image:

```yaml
name: CI

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    steps:
      - name: Check out code
        uses: actions/checkout@v4

      - name: Set up .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore --configuration Release

      - name: Test
        run: dotnet test --no-build --configuration Release --verbosity normal

  build-and-push-image:
    needs: build-and-test                 # only runs if build-and-test succeeded
    if: github.ref == 'refs/heads/main'    # only on the main branch, not every PR
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Log in to Docker Hub
        uses: docker/login-action@v3
        with:
          username: ${{ secrets.DOCKERHUB_USERNAME }}
          password: ${{ secrets.DOCKERHUB_TOKEN }}

      - name: Build and push
        uses: docker/build-push-action@v6
        with:
          push: true
          tags: yourusername/your-api:latest
```

| Piece | Meaning |
|---|---|
| `on: push / pull_request` | What triggers this workflow — every push to `main`, and every PR targeting it. |
| `jobs:` | Independent units of work; can run in parallel, or depend on each other via `needs:`. |
| `runs-on: ubuntu-latest` | A fresh, temporary virtual machine for each run — nothing persists between runs unless you explicitly cache/save it. |
| `steps:` | Sequential actions within a job — each `uses:` runs a reusable, published action; each `run:` executes a shell command directly. |
| `needs: build-and-test` | This job only starts if `build-and-test` succeeded — don't push an image built from code that doesn't even pass its tests. |
| `${{ secrets.DOCKERHUB_TOKEN }}` | A **secret** — configured in the repo's GitHub settings, never committed to source (same principle as Module 7's JWT signing key). |

## Why `needs:` matters — the whole point of "automated pipeline"

Without `needs: build-and-test`, a failing test wouldn't stop a broken image from being pushed and potentially deployed. The dependency between jobs is what makes this a genuine *pipeline* — each stage gates the next, so a failure anywhere upstream stops everything downstream automatically, with zero manual intervention required.

## What happens on a failing test

If `dotnet test` fails, GitHub Actions marks the whole `build-and-test` job (and the run) as failed — visible directly on the PR, blocking merge if you've configured a **branch protection rule** requiring it to pass. This is the automated safety net: a broken change literally cannot reach `main` without someone explicitly overriding the protection.

## Summary
- **CI**: automatically build + test every push. **CD**: automatically package/ship the result if CI passes.
- A GitHub Actions workflow (`.github/workflows/*.yml`) defines `jobs`, each with sequential `steps` — `uses:` for reusable actions, `run:` for shell commands.
- `needs:` chains jobs so a later stage (like pushing a Docker image) only runs if an earlier one (build + test) succeeded — this dependency is what makes it a real pipeline, not just several independent scripts.
- Secrets (Docker Hub credentials, API keys) are configured in GitHub's repo settings, referenced via `${{ secrets.NAME }}`, never committed to source.

## Do this
1. In a GitHub repository (push any exercise from this course to it if you haven't already), create `.github/workflows/ci.yml` with the `build-and-test` job above (skip the Docker push job if you don't have a Docker Hub account — that's fine, the build+test half is the core lesson).
2. Push a commit. Watch it run under your repo's **Actions** tab.
3. Deliberately break a test (change an assertion to something wrong), push again, and watch the run fail — this is the safety net actually working.
4. Fix it, push again, watch it pass.
5. (Optional, needs a Docker Hub account) Add `DOCKERHUB_USERNAME`/`DOCKERHUB_TOKEN` as repository secrets (Settings → Secrets and variables → Actions), add the `build-and-push-image` job, and watch a real image get pushed automatically after tests pass.
