# Contributing

Thanks for taking the time to contribute to GitHub Integration Service.

## Development prerequisites

- .NET SDK 10
- Docker with Docker Compose
- Git

## Local setup

1. Fork or clone the repository.
2. Copy `.env.example` to `.env`.
3. Add a GitHub Personal Access Token to `.env`.
4. Start the dependencies and API with:

```bash
docker compose up --build
```

Alternatively, run PostgreSQL and Redis yourself and start the API with `dotnet run`.

## Before opening a pull request

Run the complete test suite:

```bash
dotnet test GitHubIntegration.sln
```

If your change affects the container build, also run:

```bash
docker build -t github-integration-service:local .
```

## Branches

Use a short descriptive branch name, for example:

```text
feature/github-webhooks
fix/rate-limit-reset
refactor/cache-keys
```

## Commits

Prefer small, focused commits with imperative messages. Conventional-style prefixes are encouraged:

```text
feat: add webhook signature validation
fix: handle secondary GitHub rate limit
refactor: simplify repository cache keys
test: cover duplicate commit upsert
chore: update dependencies
```

## Pull requests

A pull request should:

- describe the problem and the chosen solution;
- remain focused on one logical change;
- include tests for new or changed behavior;
- keep secrets and local `.env` files out of the repository;
- pass the GitHub Actions CI workflow.

Security vulnerabilities should not be reported through public issues. Follow [SECURITY.md](SECURITY.md) instead.
