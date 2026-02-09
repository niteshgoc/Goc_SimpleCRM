---
description: Create or update controller files for a database model following controller standards
argument-hint: controller_path | model_name | description
---

## Context

Parse `$ARGUMENTS` to get the following values:

- `[controller_path]`: Controller file or pattern under `Controllers/`
- `[model_name]`: Name of the model that exists in `/SqlQueries/Tables/`
- `[description]`: Purpose of the controller

## Supported Patterns

| Pattern | Meaning |
|-------|--------|
| `Controllers/CustomerController.cs` | Create or update a single controller |
| `Controllers/*.cs` | Create or update all controllers in folder |
| `Controllers/**/*.cs` | Create or update controllers in subfolders |

## Standards Reference

- Controller creation **must follow instructions defined in**:
  - `docs/views-controllers.md`
- Routing, naming, and action patterns must strictly comply with documented standards.

## Rules

- Model must exist in `/SqlQueries/Tables/`
- Controllers must be created under `Controllers/`
- Controller class name must end with `Controller`
- Controller must depend on the corresponding **Service**, not Repository
- Use attribute routing as defined in `docs/views-controllers.md`
- Use async action methods
- Use **create-or-update behavior**:
  - Create file if it does not exist
  - Update file if it already exists
- Preserve existing custom logic unless explicitly instructed
- Add or update XML documentation comments using the provided description

## Task

1. Validate that the model exists in `/SqlQueries/Tables/`
2. Read controller rules from `docs/views-controllers.md`
3. Resolve controller files using the provided path or pattern
4. Inject the corresponding service via constructor
5. Generate or update controller actions based on service methods
6. Save `.cs` files in `Controllers/`

## Example

### Slash Command Input

```text
/add-controller Controllers/CustomerController.cs Customer "Customer API controller"
