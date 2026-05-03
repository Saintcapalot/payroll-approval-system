# Payroll Approval Management System

A payroll and approval management system built with ASP.NET Core 8, PostgreSQL, and Entity Framework Core.

## Architecture

Clean Architecture with four layers:

```
Api (Controllers, DTOs, Middleware)
 └── Application (Domain Services)
  └── Domain (Entities, Enums, Interfaces)
 └── Infrastructure (EF Core, Repositories, Migrations)
  └── Domain
```

| Layer | Project | Responsibility |
|---|---|---|
| **Domain** | `PayrollApprovalSystem.Domain` | Entities, enums, repository interfaces. No dependencies. |
| **Application** | `PayrollApprovalSystem.Application` | Business logic services (PayrollCalculation, PayrollGeneration, Approval, Payslip). References Domain. |
| **Infrastructure** | `PayrollApprovalSystem.Infrastructure` | EF Core DbContext, repositories, migrations, DI registration. References Domain. |
| **Api** | `PayrollApprovalSystem.Api` | Controllers, DTOs, Swagger, Serilog, JWT config. References Application + Infrastructure. |

### Tech Stack

| Component | Technology |
|---|---|
| Framework | ASP.NET Core 8 |
| Database | PostgreSQL 16 (Alpine) |
| ORM | Entity Framework Core 8.0.11 |
| DB Provider | Npgsql.EntityFrameworkCore.PostgreSQL |
| Authentication | JWT Bearer Tokens |
| Logging | Serilog (Console + Rolling File) |
| Containerization | Docker + Docker Compose |

### Domain Entities

- **Department** — Organizational units
- **Employee** — Staff members linked to departments
- **PayrollStructure** — Salary components (base, bonus, deductions) per employee
- **Payroll** — Monthly payroll records with status (Draft/Approved)
- **Approval** — Approval/rejection tracking for payrolls
- **Payslip** — Generated payslips for approved payrolls

### Business Rules

1. One active payroll structure per employee
2. No duplicate payroll per month (enforced at DB level)
3. Payroll cannot be changed after approval
4. Payslip can only be generated after approved payroll

## Quick Start (Docker)

1. Copy environment template:

```bash
cp .env.example .env
```

2. Start services:

```bash
docker compose up --build
```

3. Open Swagger:

- http://localhost:8080/swagger

### Development Seed Data

When `ASPNETCORE_ENVIRONMENT=Development`, the API applies EF Core migrations and then seeds demo data automatically after startup.

Seeded records:

- 1 Department: `Engineering`
- 1 Employee: `Demo Employee` (`demo.employee@payroll.local`)
- 1 active PayrollStructure for that employee
  - Base salary: `50000`
  - Bonus: `5000`
  - Deductions: `1500`

The seeding is idempotent, so restarting `docker compose up` will not create duplicate demo rows.

### Swagger Demo Flow

After `docker compose up --build` completes:

1. Open `http://localhost:8080/swagger`
2. Authenticate with any existing JWT flow already configured in the API
3. Call `GET /api/Employee` to confirm the seeded employee is present
4. Use the seeded employee id to generate payroll from `POST /api/Payroll/generate`
5. Approve the payroll through `POST /api/Approval/approve`
6. Generate a payslip via `POST /api/Payslip/generate`
7. Download the PDF from `GET /api/Payslip/{payrollId}/pdf`

## Environment Variables

Defined in `.env.example`:

| Variable | Description |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Development` / `Production` |
| `API_PORT` | API port (default: 8080) |
| `JWT_KEY` | JWT signing key |
| `POSTGRES_PORT` | PostgreSQL port (default: 5432) |
| `POSTGRES_DB` | Database name |
| `POSTGRES_USER` | Database user |
| `POSTGRES_PASSWORD` | Database password |

## Local Development (without Docker)

1. Set connection string and JWT key in `src/PayrollApprovalSystem.Api/appsettings.Development.json`
2. Start PostgreSQL (or use Docker for just the DB: `docker compose up postgres`)
3. Apply migrations and run:

```bash
dotnet ef database update \
  --project src/PayrollApprovalSystem.Infrastructure/PayrollApprovalSystem.Infrastructure.csproj \
  --startup-project src/PayrollApprovalSystem.Api/PayrollApprovalSystem.Api.csproj

dotnet run --project src/PayrollApprovalSystem.Api/PayrollApprovalSystem.Api.csproj
```

## Project Structure

```
payroll-approval-system/
├── src/
│   ├── PayrollApprovalSystem.Domain/          # Entities, Enums, Interfaces
│   ├── PayrollApprovalSystem.Application/     # Domain Services
│   ├── PayrollApprovalSystem.Infrastructure/  # DbContext, Repositories, Migrations
│   └── PayrollApprovalSystem.Api/             # Controllers, DTOs, Config
├── tests/
│   └── PayrollApprovalSystem.Domain.Tests/    # Unit tests (xUnit)
├── docs/
│   ├── database-design.md                     # ER diagrams, schema docs
│   └── infrastructure-setup.md                # Infrastructure documentation
├── docker-compose.yml
├── Dockerfile
└── PayrollApprovalSystem.sln
```

## Testing

Run all tests:

```bash
dotnet test
```

## Team & Responsibilities

| Person | Area | Key Deliverables |
|---|---|---|
| Person 1 (Marcos) | Domain & Business Logic | Entities, services, business rules, unit tests |
| Person 2 | API & Security | Controllers, JWT auth, role-based auth, DTOs, Swagger, integration tests |
| Person 3 | Database & Infrastructure | PostgreSQL, EF Core, migrations, repositories, Docker, logging |

## TODO

### Critical (Blocking)

- [ ] **Register application services in DI** — `PayrollCalculationService`, `PayrollGenerationService`, `ApprovalService`, `PayslipService` are not registered in the DI container. Add to `Program.cs` or `ServiceCollectionExtensions`. Without this, controllers cannot resolve services and the app crashes at runtime.
- [ ] **Add service interfaces** — Extract `IPayrollCalculationService`, `IPayrollGenerationService`, `IApprovalService`, `IPayslipService` for proper DI patterns and testability.
- [ ] **Add filtered unique index** on `PayrollStructure(EmployeeId)` where `IsActive = true` — enforces "one active structure per employee" at DB level. Requires a new migration.

### Important (Person 2 / Person 1)

- [ ] Implement all 5 controllers with action methods (Person 2)
- [ ] Implement JWT authentication middleware and token generation (Person 2)
- [ ] Implement role-based authorization with `[Authorize]` attributes (Person 2)
- [ ] Define all DTO properties (Person 2)
- [ ] Add `RejectPayroll` method to `ApprovalService` (Person 1)
- [ ] Expand unit test coverage — missing tests for approved payroll immutability, rejection, inactive structure (Person 1)
- [ ] Add API integration tests (Person 2)

### Nice to Have

- [ ] Add `.env.example` file for environment variable documentation
- [ ] Add seed data script for demo/exam
- [ ] Add Swagger JWT security definition
- [ ] Add global exception handling middleware
