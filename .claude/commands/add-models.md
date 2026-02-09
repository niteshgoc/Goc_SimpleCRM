---
description: Generate C# model classes from database table definitions
argument-hint: Models/path-pattern | description
---

## Context

Parse `$ARGUMENTS` to get the following values:

- `[path_pattern]`: File or folder pattern under `Models/`
  - Supports single file, wildcard, and recursive patterns
- `[description]`: Optional description for model purpose

## Supported Patterns

| Pattern | Meaning |
|-------|--------|
| `Models/Customer.cs` | Add a single model file |
| `Models/*.cs` | Add all model files in the folder |
| `Models/**/*.cs` | Add model files in folder and subfolders |

## Rules

- Each model must map to a table inside `/SqlQueries/Tables/`
- Table name should match model name (pluralization allowed)
- Generate properties based on table columns
- Primary key maps to `Id` property
- SQL types must be converted to C# types
- Files must be created under `/Models/`
- Do not overwrite existing model files unless explicitly allowed

## Task

1. Read table definitions from `/SqlQueries/Tables/`
2. Resolve model files using the provided path pattern
3. Generate C# model classes for each table
4. Add XML summary comments using the description
5. Save generated files under `/Models/`

## Example

### Slash Command Inputs

#### Add Single Model

```text
/add-models Models/Customer.cs "Customer master data"
