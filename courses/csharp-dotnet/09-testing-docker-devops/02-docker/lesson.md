# Module 9, Lesson 02 — Docker (Guided Lab)

> **This lesson is a guided lab, not an auto-checked exercise.** Docker isn't installed in the Learn Lab environment used to build this course, so there's no `dotnet test` to run here — instead, you'll install Docker Desktop yourself and verify each step by actually running it, the same way you'll do it on the job. Read through, then follow the **Do this** steps at the end.

## Why containers

Every exercise so far ran directly on your machine, using whatever .NET SDK you have installed. That's fine for learning, but "works on my machine" is a real, common problem when deploying software: different OS, different installed versions, missing dependencies. A **container** packages your app together with everything it needs to run — the .NET runtime, any files it depends on — into one portable unit that runs identically anywhere Docker is installed: your machine, a teammate's, a cloud server.

**Docker** is the tool that builds and runs containers. A **Dockerfile** is a text file with the instructions for building one.

## A Dockerfile for an ASP.NET Core app

```dockerfile
# ---- Build stage: has the full SDK, used only to compile ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app

# ---- Runtime stage: much smaller, no SDK, just what's needed to RUN ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "YourApp.dll"]
```

| Piece | Meaning |
|---|---|
| `FROM ... AS build` | Start from a base image (a pre-built environment) — here, one with the full .NET SDK, needed to compile. |
| `WORKDIR /src` | Set the working directory inside the container — like `cd`, but inside the image being built. |
| `COPY . .` | Copy your project's files into the container. |
| `RUN dotnet publish ...` | Actually build the app, inside the container. |
| `FROM ... AS runtime` | A **second**, separate stage — this is a **multi-stage build**. |
| `COPY --from=build /app .` | Copy just the *compiled output* from the build stage into this new, much smaller image — the full SDK (hundreds of MB) never ships in the final image, only the runtime and your compiled app. |
| `EXPOSE 8080` | Documents which port the app listens on inside the container. |
| `ENTRYPOINT [...]` | The command that runs when the container starts. |

**Multi-stage builds** exist because you need the full SDK to *compile* but only the smaller runtime to *run* — shipping the SDK in your production image would mean a needlessly large, slower-to-deploy image.

## Building and running it

```powershell
docker build -t my-api .              # build an image, tagged "my-api", from the Dockerfile in this directory
docker run -p 8080:8080 my-api         # run a container from that image, mapping host port 8080 to container port 8080
```

Visit `http://localhost:8080` — your app is now running **inside an isolated container**, with its own filesystem and process space, entirely separate from anything else on your machine.

## `docker-compose` — running multiple containers together

A real app is rarely just one container — an API plus a database plus maybe a cache. `docker-compose.yml` describes a whole multi-container setup declaratively:

```yaml
services:
  api:
    build: .
    ports:
      - "8080:8080"
    depends_on:
      - db
    environment:
      - ConnectionStrings__Default=Server=db;Database=AppDb;User=sa;Password=Your_password123

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=Your_password123
    ports:
      - "1433:1433"
```

```powershell
docker-compose up      # builds/starts every service defined above, together
docker-compose down    # stops and removes them
```

| Piece | Meaning |
|---|---|
| `services:` | Each named block is one container. |
| `build: .` | Build this service's image from the Dockerfile in the current directory (as opposed to `image:`, which pulls a pre-built one). |
| `depends_on` | Start order — `db` starts before `api`. |
| `environment` | Environment variables passed into the container — this is exactly Module 8's configuration system reading from `ConnectionStrings__Default` instead of `appsettings.json`, letting the same app code run against different databases in different environments without code changes. |

One command (`docker-compose up`) now starts your whole API + database stack, consistently, on any machine with Docker installed.

## Summary
- A **Dockerfile** describes how to build a container image for your app.
- **Multi-stage builds**: compile with the full SDK in one stage, ship only the runtime + compiled output in the final, much smaller image.
- `docker build` creates an image; `docker run` starts a container from it.
- **`docker-compose.yml`** declares multiple related containers (API + database + ...) and starts/stops them together with one command.
- Environment variables passed into a container are how the same app image runs correctly across different environments — the same configuration system from Module 8, just sourced differently.

## Do this (needs Docker Desktop installed)
1. Install Docker Desktop for Windows if you haven't already.
2. Pick any exercise's `App/` folder from this course (e.g. Module 7's capstone).
3. Copy the Dockerfile above into that folder, adjusting `YourApp.dll` to match the actual output assembly name (check `bin/Debug/net10.0/` after a `dotnet build` to confirm it).
4. `docker build -t my-api .` then `docker run -p 8080:8080 my-api`.
5. Visit the app in your browser or with `curl` to confirm it's genuinely running inside the container.
6. If that endpoint uses EF Core + SQLite (Module 6), notice the database file lives *inside* the container by default — it disappears when the container is removed. This is intentional groundwork for understanding why real deployments use an external database (like the `docker-compose` example above) rather than one baked into the image.
