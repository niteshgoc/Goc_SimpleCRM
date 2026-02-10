# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Project:** SimpleCRM
**Tech Stack:** ASP.NET Core 8 MVC, Razor Views, jQuery, Bootstrap 5, Dapper, MS SQL Server

SimpleCRM is a customer relationship management application built with ASP.NET Core 8.0 MVC. The application uses Dapper for data access with stored procedures and MS SQL Server as the database.

## Running the Project

```bash
# Build the project
dotnet build

# Run the application (defaults to http://localhost:5076)
dotnet run

# Run in watch mode (auto-reload on changes)
dotnet watch run

# Clean build artifacts
dotnet clean
```

## Build Commands

- **Build:** `dotnet build`
- **Run:** `dotnet run`
- **Watch:** `dotnet watch run`
- **Clean:** `dotnet clean`
- **Restore:** `dotnet restore`

## Architecture

### Solution Structure
- **`SimpleCRM.sln`** — Solution file
- **`Program.cs`** — App entry point, configures middleware and services
- **`appsettings.json`** — Configuration (connection strings, app settings)
- **`Controllers/`** — MVC controllers (HTTP request handlers)
- **`Models/`** — Entity models organized by feature (Entity, Request, Response)
- **`Views/`** — Razor views organized by controller
- **`Repos/`** — Repository layer (Interfaces and Dapper implementations)
- **`Data/`** — Database context (DapperContext for SQL connection)
- **`SqlQueries/`** — SQL scripts for tables and stored procedures
- **`wwwroot/`** — Static files (CSS, JS, Bootstrap, jQuery)
- **`.claude/.claude/docs/`** — Detailed documentation (patterns, conventions, examples)

### Data Flow
1. **User Request** → Controller action
2. **Controller** → Calls Repository method
3. **Repository** → Executes stored procedure via Dapper + DapperContext
4. **DapperContext** → Uses connection string from appsettings.json
5. **Database** → MS SQL Server executes stored procedure
6. **Response** → Wrapped in ServiceResponse, returned to View/JSON

### Key Components
- **DapperContext** (`Data/DapperContext.cs`): Database connection factory using `IConfiguration`
- **ServiceResponse** (`Models/ServiceResponse.cs`): Standard response wrapper for all operations
- **Repositories**: Interface + implementation pattern, all methods async with `Task<ServiceResponse<T>>`
- **Stored Procedures**: All CRUD operations use stored procedures prefixed with `ssp_`

## Database Connectivity & Access Policy

**📋 IMPORTANT:** All database connectivity and access rules are defined in [`.claude/dbpolicy/Connection.md`](.claude/dbpolicy/Connection.md).

**You MUST:**
- Read and fully comply with `.claude/dbpolicy/Connection.md`
- Treat that file as the single source of truth for all database-related behavior
- Strictly follow all rules defined in that file for database operations

## Coding Patterns

### Models (3-Model Pattern)
For each database table, create 3 models in `Models/[Entity]/`:
1. **Entity.cs** — Matches database table structure
2. **EntityRequest.cs** — For Insert/Update (includes validation attributes)
3. **EntityResponse.cs** — For API/View responses (can include computed properties)

See [.claude/docs/models.md](.claude/docs/models.md) for detailed patterns.

### Repositories
- Interface in `Repos/Interfaces/I[Entity]Repository.cs`
- Implementation in `Repos/Services/[Entity]Repository.cs`
- All methods return `Task<ServiceResponse<T>>`
- Standard CRUD: InsertUpdate, GetActive, GetById, GetAll, Delete, SoftDelete

See [.claude/docs/repositories.md](.claude/docs/repositories.md) for detailed patterns.

### Controllers & Views (Single Page CRUD)
- **2 MVC actions**: Index() for list page, Details(id) for details
- **6 API endpoints**: InsertUpdate, GetActive, GetAll, GetById, Delete, SoftDelete
- **2 Razor views**: Index.cshtml (list + modals), Details.cshtml
- All CRUD operations via AJAX, no page reload

See [.claude/docs/views-controllers.md](.claude/docs/views-controllers.md) for detailed patterns.

### Database
- Connection string in `appsettings.json` under `ConnectionStrings:DefaultConnection`
- DapperContext reads connection string and provides `IDbConnection`
- All SQL in stored procedures (no inline SQL)
- Stored procedure naming: `ssp_[TableName]_[Action]`

See [.claude/docs/database.md](.claude/docs/database.md) for detailed database conventions.

### Frontend
- jQuery for AJAX and DOM manipulation
- Bootstrap 5 for responsive UI
- Client-side validation (email, phone, required fields)
- Toaster notifications for success/error feedback

See [.claude/docs/frontend.md](.claude/docs/frontend.md) for detailed JavaScript patterns.

## Documentation

Detailed documentation is located in the `.claude/docs/` folder:
- **[models.md](.claude/docs/models.md)** — Model structure and naming conventions
- **[repositories.md](.claude/docs/repositories.md)** — Repository pattern and Dapper usage
- **[views-controllers.md](.claude/docs/views-controllers.md)** — Controller structure and Razor view patterns
- **[database.md](.claude/docs/database.md)** — Database schema, stored procedures, SQL conventions
- **[frontend.md](.claude/docs/frontend.md)** — JavaScript, validation, AJAX patterns

## Flexible Creation Order

**IMPORTANT:** Layer creation order is **RECOMMENDED, NOT REQUIRED**.

### Recommended Order (for new features)
Table → Stored Procedures → Models → Repository → Service → Controller → View → JavaScript

### Execution Independence Rules
- **Any slash command can be executed independently** at any time
- **Controllers can be created first**, even if models, services, or views don't exist yet
- **Views can be created** without controllers
- **JavaScript can be created** without views
- **Services can be created** without repositories
- Missing dependencies result in:
  - TODO comments
  - Placeholder code
  - Minimal compile-ready stubs
- **Never block execution** due to missing dependencies

### What Commands Do NOT Do
- Creating a controller **does NOT** auto-create models, views, or JavaScript
- Creating a view **does NOT** auto-create controllers or JavaScript
- Creating models **does NOT** auto-create repositories or services
- Each command creates **ONLY** the requested artifact

### Documentation Role
- Files in `.claude/docs/` define **HOW** to write code (structure, patterns, conventions)
- Documentation is **NOT** a prerequisite for execution
- Documentation guides style, NOT permission

## Important Notes

- Target framework: .NET 8.0
- Nullable reference types enabled
- Use async/await for all database operations
- Always use parameterized queries (via stored procedures)
- Bootstrap 5 responsive design (`col-md-6 col-12`, `table-responsive`)
- Cache busting: `?v=@FileVersion` on script tags
