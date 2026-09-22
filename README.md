# SignBridge

SignBridge is an ASP.NET Core backend for a sign-language learning platform aimed at children and their parents.

The project focuses on backend concerns that usually appear in a real product: authentication, role-based access, parent/child linking, structured learning content, sequential lesson unlocking, quizzes, progress tracking, favorites, and reporting.

## Stack

- .NET 10 / ASP.NET Core Web API
- Entity Framework Core 10
- SQLite by default for a zero-setup demo
- PostgreSQL supported through configuration
- JWT authentication + refresh tokens
- Role-based authorization
- xUnit unit tests
- Built-in OpenAPI document
- Docker support

## Projects

```text
src/
  SignBridge.Api
  SignBridge.Application
  SignBridge.Domain
  SignBridge.Infrastructure

tests/
  SignBridge.UnitTests
```

### Dependency direction

```text
Api -> Application
Api -> Infrastructure
Infrastructure -> Application
Infrastructure -> Domain
Application -> Domain
Domain -> nothing
```

## Main roles

- `Admin`
- `Teacher`
- `Parent`
- `Child`

Public registration only allows `Parent` and `Child`. Admin and Teacher accounts are created by administration.

## Demo accounts

| Role | Email | Password |
|---|---|---|
| Admin | admin@signbridge.local | Admin123! |
| Teacher | teacher@signbridge.local | Teacher123! |
| Parent | parent@signbridge.local | Parent123! |
| Child | child@signbridge.local | Child123! |

The demo parent is already linked to the demo child.

## Run locally

Requirements:

- .NET 10 SDK

```bash
dotnet restore
dotnet run --project src/SignBridge.Api
```

The console prints the listening URL. The OpenAPI document is available at:

```text
/openapi/v1.json
```

The SQLite database is created automatically inside the API project folder the first time the application starts.

## Run tests

```bash
dotnet test
```

## PostgreSQL

SQLite is the default so the project can be reviewed immediately.

To use PostgreSQL, set:

```text
Database__Provider=Postgres
ConnectionStrings__Postgres=Host=localhost;Port=5432;Database=signbridge;Username=postgres;Password=postgres
```

A sample PostgreSQL container is included:

```bash
docker compose up -d db
```

## Important business rules

### Lesson unlocking

Lessons are sequential inside a level.

- The first published lesson in a level is unlocked.
- A later lesson is unlocked only when the previous published lesson is completed.
- The API enforces the rule. The frontend cannot bypass it.

### Quiz completion

- Each lesson can have one quiz.
- The lesson has a configurable passing score.
- A passed quiz completes the lesson.
- A failed quiz keeps the lesson in progress.
- Every attempt is stored for reporting.

### Parent-child security

A parent can only view progress for a linked child. The API verifies the relationship on every parent progress request.

### Child linking

A child has a private link code. A parent needs both the child's email and link code to create the relationship.

### Refresh tokens

Refresh tokens are random values. Only their SHA-256 hashes are stored in the database.

## Suggested review flow

1. Login as the demo child.
2. Read `/api/learning/courses`.
3. Open the first lesson.
4. Submit its quiz.
5. Verify that the next lesson becomes unlocked.
6. Login as the parent.
7. Read the child's progress report.
8. Login as Teacher or Admin and create new learning content.

## Example login

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "child@signbridge.local",
    "password": "Child123!"
  }'
```

Use the returned access token:

```bash
curl http://localhost:5000/api/learning/courses \
  -H "Authorization: Bearer YOUR_TOKEN"
```

## API overview

### Authentication

```text
POST /api/auth/register
POST /api/auth/login
POST /api/auth/refresh
```

### Child learning

```text
GET    /api/learning/courses
GET    /api/learning/courses/{courseId}
GET    /api/learning/lessons/{lessonId}
GET    /api/learning/lessons/{lessonId}/quiz
POST   /api/learning/lessons/{lessonId}/quiz/submit
POST   /api/learning/lessons/{lessonId}/complete
POST   /api/learning/lessons/{lessonId}/favorite
DELETE /api/learning/lessons/{lessonId}/favorite
GET    /api/learning/favorites
```

### Parent

```text
POST /api/parents/children/link
GET  /api/parents/children
GET  /api/parents/children/{childId}/progress
```

### Teacher/Admin content management

```text
POST  /api/content/courses
POST  /api/content/courses/{courseId}/levels
POST  /api/content/levels/{levelId}/lessons
POST  /api/content/lessons/{lessonId}/quiz
POST  /api/content/quizzes/{quizId}/questions
PATCH /api/content/courses/{courseId}/publish
PATCH /api/content/lessons/{lessonId}/publish
```

## Notes

The repository deliberately keeps the architecture clear and small. There is no generic repository abstraction because EF Core already provides a useful unit-of-work and repository-style API. Business rules live in services where they can be tested and reviewed directly.
