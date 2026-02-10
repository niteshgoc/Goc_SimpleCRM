---
description: Create or update repository interface files for a database model following repository standards
argument-hint: interface_path | model_name | description
---

## Context

Parse `$ARGUMENTS` to get the following values:

- `[interface_path]`: Interface file or pattern under `Repo/Interfaces/`
- `[model_name]`: Name of the model that exists in `/SqlQueries/Tables/`
- `[description]`: Purpose of the repository interface

## Supported Patterns

| Pattern | Meaning |
|-------|--------|
| `Repo/Interfaces/ICustomerRepo.cs` | Create or update a single interface |
| `Repo/Interfaces/*.cs` | Create or update multiple interfaces |
| `Repo/Interfaces/**/*.cs` | Create or update interfaces in subfolders |

## Standards Reference

- Interface creation **must follow instructions defined in**:
  - `/.claude/docs/repositories.md`
- Naming, method signatures, and structure must strictly comply with documented repository standards.

## Rules

- Interfaces must be created under `Repo/Interfaces/`
- Interface name must start with `I`
- One interface per model
- Repository methods must follow CRUD standards defined in `/.claude/docs/repositories.md`
- Use async method signatures (`Task`, `Task<T>`)
- Use **create-or-update behavior**:
  - Create file if it does not exist
  - Update file if it already exists
- Do not remove existing custom methods unless explicitly instructed
- Add or update XML documentation comments using the provided description

## Dependency Handling

- **If the model does not exist**: Use generic types (`T`) or `object` with TODO comment
- **Never block execution** due to missing dependencies

## Task

1. Check if the model exists in `/SqlQueries/Tables/` (optional reference)
2. Read repository rules from `/.claude/docs/repositories.md`
3. Resolve interface files using the provided path or pattern
4. Infer CRUD operations from standard patterns or model schema (if available)
5. **Create or update** repository interface methods as per documented standards
6. Save `.cs` files in `Repo/Interfaces/`

## Example

### Slash Command Input

```text
/add-interface Repo/Interfaces/ICustomerRepo.cs Customer "Customer repository interface"
