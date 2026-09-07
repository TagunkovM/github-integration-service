# GitHub Integration Service Design

## Goal
Build a portfolio-grade .NET 10 microservice that tracks GitHub repositories and synchronizes repository metadata, issues, pull requests, and commits from the GitHub REST API.

## Architecture
Clean Architecture with four production projects: Domain, Application, Infrastructure, and Api. Application depends on abstractions for persistence, caching, the GitHub client, and the sync queue. Infrastructure implements these abstractions with EF Core/PostgreSQL, Redis, HttpClient, and an in-memory Channel queue. Api exposes REST endpoints and hosts the background sync worker.

## Authentication
MVP uses a GitHub Personal Access Token loaded from configuration/environment (`GitHub__Token`). OAuth is explicitly out of scope for this iteration and is the planned next extension.

## Data
Tracked repository has 1:N relations to Issue, PullRequest, Commit, and SyncHistory. Repository GitHub id/full name, issue number per repository, pull request number per repository, and commit SHA per repository are unique.

## API
- POST `/api/repositories`
- GET `/api/repositories`
- GET `/api/repositories/{owner}/{name}`
- DELETE `/api/repositories/{owner}/{name}`
- GET `/api/repositories/{owner}/{name}/issues`
- GET `/api/repositories/{owner}/{name}/pull-requests`
- GET `/api/repositories/{owner}/{name}/commits`
- POST `/api/repositories/{owner}/{name}/sync`
- GET `/api/sync/{syncId}`
- GET `/health`, `/health/live`, `/health/ready`

## Sync flow
A sync request creates a persisted SyncHistory row, places its id on a bounded Channel queue, and returns immediately. A BackgroundService consumes ids, invokes the application synchronization processor, calls GitHub, upserts local rows, updates LastSyncedAt, and marks the sync success/failure.

## GitHub API
Use REST API version `2026-03-10`, `Accept: application/vnd.github+json`, and a Bearer PAT. The HTTP client uses Microsoft.Extensions.Http.Resilience (Polly) with the standard resilience pipeline. GitHub errors are translated into a typed external service exception; API exception middleware returns Problem Details.

## Cache
Redis is an optimization, not the source of truth. Cache repository reads and collection queries. Collection cache keys include the repository sync version (`LastSyncedAt`) so a completed sync naturally changes keys. Redis failures degrade to database reads.

## Testing
Unit tests cover pagination, mapping/filtering, and cache key behavior. Integration tests use a PostgreSQL Testcontainer to verify persistence/upsert behavior. Docker-dependent tests can be skipped by developers without Docker.
