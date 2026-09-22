# SignBridge

**SignBridge** is a full-stack visual learning platform for children, parents, teachers, and administrators. It combines video-backed lessons, quizzes, progress tracking, family visibility, content management, and accessibility-oriented UI in one product.

The project was built as a portfolio application rather than a CRUD-only sample.

## Highlights

- Role-based experience for **Child, Parent, Teacher, and Admin**
- Email/password authentication with JWT and refresh tokens
- Child and Parent public registration
- Video-backed course lessons
- Sequential lesson unlocking
- Quizzes, passing scores, XP, streaks, favorites, and achievements
- Sign dictionary with English/Arabic vocabulary
- Parent-to-child account linking
- Parent progress dashboard
- Teacher content studio:
  - Course
  - Level
  - Lesson
  - Quiz
  - Question
- Admin analytics and user management
- Responsive Angular UI
- Large-text and high-contrast accessibility controls
- SQLite for easy local setup
- PostgreSQL support
- Docker support
- OpenAPI
- Unit tests
- GitHub Actions CI

## Tech stack

### Backend
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core 10
- JWT + refresh tokens
- Role-based authorization
- SQLite
- PostgreSQL
- OpenAPI
- xUnit

### Frontend
- Angular 20
- Standalone components
- Angular Router
- HttpClient
- Route guards
- Authentication interceptor
- Responsive custom CSS

## Architecture

```text
SignBridge/
├── frontend/                      Angular application
├── src/
│   ├── SignBridge.Api             HTTP/API layer
│   ├── SignBridge.Application     Use cases, DTOs, service contracts
│   ├── SignBridge.Domain          Entities and domain rules
│   └── SignBridge.Infrastructure  EF Core, persistence, auth services
├── tests/
│   └── SignBridge.UnitTests
├── .github/workflows/ci.yml
├── Dockerfile
├── docker-compose.yml
└── SignBridge.sln
```

## Product roles

### Child
Children can:
- Browse published courses
- Open unlocked lessons
- Watch lesson videos inside the platform
- Take quizzes
- Earn XP
- Build streaks
- Save favorite lessons
- Browse the sign dictionary
- Unlock achievements
- Track progress

### Parent
Parents can:
- Link a child using email + private code
- View overall progress
- View completed lessons
- See quiz averages
- Review XP and streak
- See course-by-course progress

### Teacher
Teachers can build learning content from the UI:
- Create courses
- Add levels
- Add lessons
- Add video URLs
- Create quizzes
- Add quiz questions
- Publish/unpublish content

### Admin
Admins can:
- View platform metrics
- Search users
- Filter users by role
- Enable/disable accounts
- Access content tools

## Course library

The local seed includes practical visual-learning courses such as:

- Arabic Alphabet - Visual Learning
- English Alphabet in ASL
- Numbers in ASL
- School & Colors in ASL
- Family Signs in ASL
- Math Basics in Sign Language

External video lessons identify their language/source context inside the course. Sign languages are regional, so the platform does not label general visual Arabic literacy content as Arabic Sign Language.

## Local reviewer accounts

| Role | Email | Password |
|---|---|---|
| Child | `child@signbridge.local` | `Child123!` |
| Parent | `parent@signbridge.local` | `Parent123!` |
| Teacher | `teacher@signbridge.local` | `Teacher123!` |
| Admin | `admin@signbridge.local` | `Admin123!` |

The seeded Parent is linked to the seeded Child.

## Run locally

### 1. Backend

From the repository root:

```bash
dotnet restore
dotnet build
dotnet run --project src/SignBridge.Api --urls http://localhost:5187
```

API health check:

```text
http://localhost:5187/api/system/health
```

OpenAPI document:

```text
http://localhost:5187/openapi/v1.json
```

### 2. Frontend

Open another terminal:

```bash
cd frontend
npm install
npm start
```

Open:

```text
http://localhost:4200
```

## Tests

```bash
dotnet test
```

Frontend production build:

```bash
cd frontend
npm run build
```

## Core business rules

- Lessons are sequential inside a level.
- Locked lessons cannot be opened by bypassing the UI and calling the learning API directly.
- A lesson is completed only after reaching its quiz passing score.
- XP is awarded only on the first successful completion.
- Parent progress endpoints verify the parent-child relationship.
- Public registration cannot create Teacher or Admin accounts.
- Refresh tokens are stored as hashes.

## Database

SQLite is the default provider for quick local setup.

For PostgreSQL:

```bash
docker compose up -d db
```

Then configure:

```text
Database__Provider=Postgres
ConnectionStrings__Postgres=Host=localhost;Port=5432;Database=signbridge;Username=postgres;Password=postgres
```

## Video content note

The platform supports embedded external videos and uploaded media. The included course library uses external educational videos as portfolio/sample content and links back to the original source.

For a real educational deployment, curriculum and sign-language variants should be reviewed by qualified Deaf/sign-language educators and media usage should follow the source platform's terms and permissions.

## Production improvements

For a production deployment, the next steps would include:
- Object storage/CDN for course media
- Email verification and password-reset delivery
- EF Core migrations in deployment pipelines
- Rate limiting and stronger audit logging
- Broader integration/end-to-end test coverage
- Production monitoring and observability
- Reviewed regional sign-language curriculum
