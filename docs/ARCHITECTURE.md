# SignBridge Architecture

## Request flow

```text
Angular UI
   ↓
JWT auth interceptor
   ↓
ASP.NET Core API
   ↓
Application services
   ↓
Infrastructure / EF Core
   ↓
SQLite or PostgreSQL
```

## Role boundaries

- **Child**: learning, quizzes, dictionary, favorites, achievements, profile
- **Parent**: linked-child progress only
- **Teacher**: content management
- **Admin**: platform management + content management

## Learning flow

```text
Course
  └── Level
       └── Lesson
            ├── Video / visual material
            ├── Summary
            └── Quiz
                 └── Questions
                      └── Answers
```

A child completes a lesson by passing its quiz. The next lesson is then unlocked and XP is awarded once.

## Security notes

- JWT access tokens authenticate API requests.
- Refresh tokens are persisted as hashes.
- Role policies protect restricted endpoints.
- Parent progress requests verify the actual parent-child relationship.
- Public registration is limited to Child and Parent roles.
