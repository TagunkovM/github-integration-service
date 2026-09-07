# GitHub Integration Service

Pet-проект микросервиса на **.NET 10**, который интегрируется с GitHub REST API, получает данные репозиториев и сохраняет их локально.

Проект создан для практики backend-разработки на .NET: работа с внешним API, PostgreSQL, Redis, фоновые задачи, Docker и тестирование.

## Возможности

- добавление GitHub-репозиториев для отслеживания;
- синхронизация информации о репозитории;
- получение Issues, Pull Requests и Commits;
- хранение данных в PostgreSQL;
- кэширование через Redis;
- ручная и фоновая синхронизация;
- обработка ошибок и временной недоступности GitHub API;
- Swagger / OpenAPI;
- Health Checks;
- Docker Compose;
- unit и integration tests;
- CI через GitHub Actions.

## Стек

- .NET 10 / C# 14
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Redis
- GitHub REST API
- HttpClient
- Polly / Microsoft.Extensions.Http.Resilience
- Serilog
- xUnit
- Testcontainers
- Docker / Docker Compose
- GitHub Actions

## Структура проекта

```text
src/
├── GitHubIntegration.Api
├── GitHubIntegration.Application
├── GitHubIntegration.Domain
└── GitHubIntegration.Infrastructure

tests/
├── GitHubIntegration.UnitTests
└── GitHubIntegration.IntegrationTests
```

Проект разделён на слои по принципам Clean Architecture.

## Запуск

### Требования

- Docker Desktop
- GitHub Personal Access Token

### 1. Создайте `.env`

Windows PowerShell:

```powershell
Copy-Item .env.example .env
```

Добавьте GitHub-токен:

```env
GITHUB_TOKEN=github_pat_your_token
```

> Файл `.env` не должен попадать в Git.

### 2. Запустите проект

```powershell
docker compose up --build
```

После запуска Swagger будет доступен по адресу:

```text
http://localhost:8080/swagger
```

## Основные API endpoints

| Метод | Endpoint | Назначение |
|---|---|---|
| `POST` | `/api/repositories` | Добавить репозиторий |
| `GET` | `/api/repositories` | Получить список репозиториев |
| `GET` | `/api/repositories/{owner}/{name}` | Получить репозиторий |
| `DELETE` | `/api/repositories/{owner}/{name}` | Удалить локальные данные |
| `GET` | `/api/repositories/{owner}/{name}/issues` | Получить Issues |
| `GET` | `/api/repositories/{owner}/{name}/pull-requests` | Получить Pull Requests |
| `GET` | `/api/repositories/{owner}/{name}/commits` | Получить Commits |
| `POST` | `/api/repositories/{owner}/{name}/sync` | Запустить синхронизацию |
| `GET` | `/api/sync/{syncId}` | Проверить статус синхронизации |

## Пример

Добавление репозитория `dotnet/runtime`:

```json
{
  "owner": "dotnet",
  "name": "runtime"
}
```

Запрос выполняется через:

```text
POST /api/repositories
```

После добавления сервис создаёт задачу синхронизации и начинает получать данные из GitHub.

## Тесты

```powershell
dotnet test
```

Тесты также запускаются автоматически в GitHub Actions при push и pull request.

## Планы развития

- GitHub OAuth;
- GitHub Webhooks;
- синхронизация Releases и Contributors;
- метрики и расширенный мониторинг.

## Лицензия

Проект распространяется по лицензии MIT.
