---
description: Create or update MVC view files following view standards
argument-hint: view_path | model_name | description
---

## Context

Parse `$ARGUMENTS` to get the following values:

- `[view_path]`: View file or pattern under `Views/`
- `[model_name]`: Name of the model that exists in `/SqlQueries/Tables/`
- `[description]`: Purpose of the view

## Supported Patterns

| Pattern | Meaning |
|-------|--------|
| `Views/Customer/Index.cshtml` | Create or update a single view |
| `Views/Customer/*.cshtml` | Create or update all views in folder |
| `Views/**/*.cshtml` | Create or update views recursively |

## Standards Reference

- View creation **must follow instructions defined in**:
  - `/.claude/docs/views-controllers.md`
- Layout usage, naming conventions, and Razor syntax must strictly comply with documented standards.

## Rules

- Views must be created under `Views/`
- This command generates **ONLY Razor Views**
- No controller logic is generated or modified
- Views must use strongly typed models where required
- Use **create-or-update behavior**:
  - Create file if it does not exist
  - Update file if it already exists
- Preserve custom Razor logic unless explicitly instructed
- Add header comments using the provided description

## Dependency Handling

- **If the model does not exist**: Use `dynamic` or `ViewData` with TODO comment
- **If the controller does not exist**: Generate view structure anyway
- **Never block execution** due to missing dependencies

## Task

1. Check if the model exists in `/SqlQueries/Tables/` (optional reference)
2. Read view standards from `/.claude/docs/views-controllers.md`
3. Resolve view files using the provided path or pattern
4. Generate or update Razor view markup:
   - Model declaration (`@model`) or fallback to `dynamic`
   - Layout usage
   - Basic structure (table/form/details as applicable)
5. Save `.cshtml` files in `Views/`

## Example

### Slash Command Input

```text
/add-view Views/Customer/Index.cshtml Customer "Customer listing view"
