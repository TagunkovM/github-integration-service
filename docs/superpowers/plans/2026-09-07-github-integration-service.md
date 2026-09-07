# GitHub Integration Service Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a runnable .NET 10 GitHub integration microservice that tracks repositories and synchronizes metadata, issues, pull requests, and commits.

**Architecture:** Clean Architecture split into Domain, Application, Infrastructure, and Api. Application defines contracts and orchestration; Infrastructure implements PostgreSQL/Redis/GitHub integration; Api owns HTTP and background hosting.

**Tech Stack:** .NET 10, C# 14, ASP.NET Core, EF Core 10, PostgreSQL, Redis, HttpClientFactory, Microsoft.Extensions.Http.Resilience (Polly), Serilog, Swagger, Docker Compose, xUnit, Testcontainers.

**Spec:** `docs/superpowers/specs/2026-09-07-github-integration-design.md`

## Global Constraints
- Target framework: `net10.0`.
- GitHub REST API version: `2026-03-10`.
- Authentication MVP: Personal Access Token from configuration; no OAuth implementation.
- PostgreSQL is the source of truth; Redis is best-effort cache only.
- Sync must run asynchronously through a background queue.

---

### Task 1: Solution and domain model
**Files:** solution/build props plus `src/GitHubIntegration.Domain/**`.
- [ ] Create solution/project files targeting net10.0.
- [ ] Add Repository, Issue, PullRequest, Commit, SyncHistory entities and enums.
- [ ] Add uniqueness and relationship requirements to persistence design.

### Task 2: Application contracts and pure behavior
**Files:** `src/GitHubIntegration.Application/**`, `tests/GitHubIntegration.UnitTests/**`.
- [ ] Write unit tests for pagination normalization, PR filtering from issues, and cache versioning.
- [ ] Implement DTOs, external GitHub data contracts, cache keys, mappings, service interfaces, and application services.
- [ ] Keep application free of EF Core, Redis, and ASP.NET dependencies.

### Task 3: Persistence and cache
**Files:** `src/GitHubIntegration.Infrastructure/Persistence/**`, `Caching/**`, integration tests.
- [ ] Write a PostgreSQL Testcontainers test that verifies issue upsert by repository+number.
- [ ] Implement AppDbContext model configuration and EfIntegrationStore.
- [ ] Implement Redis cache as best-effort ICacheService.

### Task 4: GitHub HTTP client and resilience
**Files:** `src/GitHubIntegration.Infrastructure/GitHub/**`, dependency injection registration.
- [ ] Configure typed HttpClient with GitHub headers and Bearer token.
- [ ] Add Polly-based standard resilience handler.
- [ ] Implement repository/issues/pulls/commits requests and JSON mapping.
- [ ] Respect configured page size/max pages and filter pull requests out of the issues stream.

### Task 5: Background synchronization
**Files:** application sync services, infrastructure channel queue, API worker.
- [ ] Persist SyncHistory before enqueueing.
- [ ] Consume jobs in BackgroundService scopes.
- [ ] Upsert remote data and complete/fail SyncHistory with counters and error message.

### Task 6: HTTP API and operational endpoints
**Files:** `src/GitHubIntegration.Api/**`.
- [ ] Add controllers for repositories/data/sync.
- [ ] Add exception middleware returning Problem Details.
- [ ] Add Swagger, Serilog, PostgreSQL readiness health check, and liveness endpoint.
- [ ] Initialize database for local/demo startup.

### Task 7: Containerization and documentation
**Files:** `Dockerfile`, `docker-compose.yml`, `.env.example`, `README.md`.
- [ ] Add multi-stage .NET 10 Docker image.
- [ ] Compose API, PostgreSQL, Redis with health checks and environment wiring.
- [ ] Document local/Docker startup, PAT setup, endpoints, and curl examples.
- [ ] Run solution tests/build when a .NET 10 SDK is available.
