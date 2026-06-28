# ClinicApp — Clinic Management System

An ASP.NET Core MVC web application for managing clinic operations. It supports multi-role authentication, doctor and patient management, and medical program tracking — built with clean architecture principles on .NET 10.

---

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Domain Model](#domain-model)
- [Roles & Permissions](#roles--permissions)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [API / Routes](#api--routes)

---

## Features

- **Role-based authentication** via cookie sessions (Admin, Doctor, Employee, Patient)
- **Doctor management**: registration, profile editing, specialty, phone
- **Patient management**: registration with AMKA (Greek ID), blood type, date of birth
- **Medical programs**: doctors create programs; patients are enrolled
- **Capability-based authorization**: fine-grained permissions attached to roles
- **Soft deletes**: logical deletion across all entities with audit timestamps
- **Pagination & filtering**: all list views support server-side filtering
- **Structured logging** with Serilog
- **Greek locale support**: database collation set to `Greek_100_CI_AI`

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET 10) |
| Database | SQL Server (Express) |
| ORM | Entity Framework Core 10.0.5 |
| Mapping | AutoMapper 16.1.1 |
| Password hashing | BCrypt.Net-Next 4.1.0 |
| Logging | Serilog 10.0.0 |
| Authentication | ASP.NET Core Cookie Authentication |
| Views | Razor (.cshtml) |

---

## Architecture

The application follows a layered architecture with strict separation of concerns:

```
Controller  →  ApplicationService (Facade)
                ├── UserService
                ├── DoctorService
                ├── PatientService
                └── MedicalProgramService
                        ↓
               IUnitOfWork  →  Repositories  →  EF Core DbContext
```

**Key patterns used:**

- **Repository Pattern** — abstracts data access per entity
- **Unit of Work** — coordinates transactions across repositories
- **Service Layer** — all business logic lives here, not in controllers
- **Facade Pattern** — `ApplicationService` is the single injection point for controllers
- **DTO Pattern** — input/output models are separate from domain entities
- **Soft Deletes** — `IsDeleted`, `DeletedAt` fields on all auditable entities

---

## Project Structure

```
ClinicApp/
├── Controllers/
│   ├── HomeController.cs          Landing page and error handling
│   ├── UserController.cs          Login / logout
│   └── DoctorController.cs        Doctor CRUD
│
├── Services/
│   ├── IApplicationService.cs     Facade interface
│   ├── ApplicationService.cs      Aggregates all services
│   ├── UserService/
│   ├── DoctorService/
│   ├── PatientService/
│   └── MedicalProgramService/
│
├── Repositories/
│   ├── IUnitOfWork.cs
│   ├── UnitOfWork.cs
│   ├── Base/                      Generic base repository (CRUD + audit)
│   ├── UserRepo/
│   ├── DoctorRepo/
│   ├── PatientRepo/
│   ├── MedicalProgramRepo/
│   └── RepositoriesDIExtensions.cs
│
├── Models/
│   ├── BaseEntity.cs              Audit fields interface
│   ├── User.cs
│   ├── Doctor.cs
│   ├── Patient.cs
│   ├── MedicalProgram.cs
│   ├── Role.cs
│   └── Capability.cs
│
├── DTO/                           Input and output data transfer objects
├── Core/
│   ├── PaginatedResult.cs
│   ├── Error.cs
│   └── Filters/                   Filter DTOs for list views
│
├── Exceptions/                    Typed application exceptions
├── Security/                      BCrypt encryption utility
├── Configuration/
│   └── MapperConfig.cs            AutoMapper profile
├── Data/
│   └── ClinicMvcdbfirstContext.cs EF Core DbContext
│
├── Views/
│   ├── Home/
│   ├── User/
│   ├── Doctor/
│   └── Shared/
│
├── Resources/                     Localized error messages
├── Program.cs
├── appsettings.json
└── appsettings.Development.json
```

---

## Domain Model

### Entities

**User**
- `Username` (unique, max 50)
- `Password` (BCrypt hashed)
- `Email` (unique)
- `Firstname`, `Lastname`
- `RoleId` → Role
- `Uuid` (external identifier)
- Audit: `InsertedAt`, `ModifiedAt`, `IsDeleted`, `DeletedAt`

**Doctor** ← one-to-one with User
- `Specialty` (max 50)
- `PhoneNumber` (max 20)
- `Uuid`
- Has many `MedicalProgram`

**Patient** ← one-to-one with User
- `Amka` (unique, max 11 — Greek social security number)
- `DateOfBirth`
- `BloodType` (nullable)
- `Uuid`
- Many-to-many with `MedicalProgram` via `PatientsPrograms`

**MedicalProgram**
- `Title` (max 100)
- `Description` (max 255)
- `DoctorId` → Doctor
- Many-to-many with `Patient`

**Role**
- `Name` (unique)
- Many-to-many with `Capability` via `RolesCapabilities`

**Capability**
- `Name` (unique)
- `Description`

### Database Join Tables

| Table | Purpose |
|---|---|
| `RolesCapabilities` | Role ↔ Capability M-to-M |
| `PatientsPrograms` | Patient ↔ MedicalProgram M-to-M |

---

## Roles & Permissions

| Role | Access |
|---|---|
| **Admin** | Full access — manage doctors, patients, users, programs |
| **Doctor** | View/edit own profile, manage own programs |
| **Employee** | View doctors and patients |
| **Patient** | View own profile and enrolled programs |

Authorization is enforced at the controller/action level via `[Authorize(Roles = "...")]`. Roles have associated `Capability` records for fine-grained permission checks.

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (Express or full)
- A user with access to the target database

### 1. Clone the repository

```bash
git clone <repo-url>
cd ClinicMVCDBFirst
```

### 2. Configure the connection string

Edit `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DevConnection": "Server=localhost\\sqlexpress;Database=ClinicMVCDBFirst;User=<user>;Password=<password>;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

### 3. Apply migrations / create the database

```bash
cd ClinicApp
dotnet ef database update
```

### 4. Run the application

```bash
dotnet run
```

The app will be available at `https://localhost:5001` (or the port shown in `launchSettings.json`).

---

## Configuration

### `appsettings.json` — Base configuration

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug",
      "Override": { "Microsoft": "Information" }
    },
    "WriteTo": [{ "Name": "Console" }]
  }
}
```

Serilog is configured for console output at Debug level. Add file sinks or other outputs here for production.

### `appsettings.Development.json` — Dev overrides

```json
{
  "ConnectionStrings": {
    "DevConnection": "Server=localhost\\sqlexpress;Database=ClinicMVCDBFirst;..."
  }
}
```

### Authentication (Program.cs)

| Setting | Value |
|---|---|
| Login path | `/User/Login` |
| Access denied path | `/Home/AccessDenied` |
| Session timeout | 30 minutes (sliding) |
| Cookie flags | HttpOnly, Secure |

---

## API / Routes

### Home

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/` | Anonymous | Landing page |
| GET | `/Home/Privacy` | Anonymous | Privacy page |
| GET | `/Home/AccessDenied` | Anonymous | 403 page |

### User (Authentication)

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/User/Login` | Anonymous | Login form |
| POST | `/User/Login` | Anonymous | Authenticate |
| POST | `/User/Logout` | Authenticated | Sign out |
| GET | `/User/Index` | Authenticated | Role-based redirect after login |

### Doctor

| Method | Route | Roles | Description |
|---|---|---|---|
| GET | `/Doctor` | Admin, Employee | List doctors (paginated + filtered) |
| GET | `/Doctor/Details/{uuid}` | Admin, Employee, Doctor | View doctor profile |
| GET | `/Doctor/Signup` | Admin | Registration form |
| POST | `/Doctor/Signup` | Admin | Register new doctor |
| GET | `/Doctor/Edit` | Admin, Doctor | Edit profile form |
| POST | `/Doctor/Edit` | Admin, Doctor | Update doctor profile |
| POST | `/Doctor/Delete/{uuid}` | Admin | Soft-delete doctor |

---

## Exception Handling

Custom typed exceptions are mapped to appropriate HTTP responses:

| Exception | Meaning |
|---|---|
| `EntityNotFoundException` | 404 — resource not found |
| `EntityAlreadyExistsException` | 409 — duplicate entity |
| `EntityNotAuthorizedException` | 401 — not authenticated |
| `EntityForbiddenException` | 403 — insufficient permissions |
| `InvalidArgumentException` | 400 — bad input |
| `ServerException` | 500 — unexpected server error |
