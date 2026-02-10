---
description: Create or update service layer files for a database model
argument-hint: service_path | model_name | description
---

## Context

Parse `$ARGUMENTS` to get the following values:

- `[service_path]`: Service file or pattern under `Repo/Services/`
- `[model_name]`: Name of the model that exists in `/SqlQueries/Tables/`
- `[description]`: Purpose of the service

## Supported Patterns

| Pattern | Meaning |
|-------|--------|
| `Repo/Services/CustomerService.cs` | Create or update a single service |
| `Repo/Services/*.cs` | Create or update all services in folder |
| `Repo/Services/**/*.cs` | Create or update services in subfolders |

## Standards Reference

- Service creation must follow:
  - Clean Architecture principles
  - Repository contracts defined in `Repo/Interfaces/`
  - Documentation standards in `/.claude/docs/repositories.md`

## Rules

- Services must be created under `Repo/Services/`
- Service class name must end with `Service`
- Service must depend on the corresponding repository interface
- Use async methods (`Task`, `Task<T>`)
- Use **create-or-update behavior**:
  - Create file if it does not exist
  - Update file if it already exists
- Preserve existing custom business logic unless explicitly instructed
- Add or update XML documentation comments using the provided description
- Follow clean architecture conventions

## Dependency Handling

- **If the model does not exist**: Proceed with placeholder types or `object`
- **If the repository interface does not exist**: Include constructor injection with TODO comment
- **Never block execution** due to missing dependencies

## Task

1. Check if the model exists in `/SqlQueries/Tables/` (optional reference)
2. Resolve service files using the provided path or pattern
3. Attempt to read the corresponding repository interface (if available)
4. Generate or update service methods based on expected CRUD patterns
5. Inject repository via constructor (with TODO if interface doesn't exist)
6. Save `.cs` files in `Repo/Services/`

## Example

### Slash Command Input

```text
/add-service Repo/Services/CustomerService.cs Customer "Customer business logic service"
