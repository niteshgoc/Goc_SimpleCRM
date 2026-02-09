---
description: Create or update a stored procedure for an existing database model
argument-hint: model_name | description
---

## Context

Parse `$ARGUMENTS` to get the following values:

- `[model_name]`: Name of the model that exists in `/SqlQueries/Tables/`
- `[description]`: Purpose of the stored procedure

## Rules

- Model must exist in `/SqlQueries/Tables/`
- Stored Procedure name is auto-generated as:
  - `sp_{model_name}_save`
- Use `CREATE OR ALTER PROCEDURE`
- Procedure must support INSERT and UPDATE logic
- PRIMARY KEY determines update condition
- Use MySQL-compatible syntax
- Generate a `.sql` file in `/SqlQueries/Tables/sp/`
- File name must match the stored procedure name

## Task

1. Validate that the model exists in `/SqlQueries/Tables/`
2. Read column definitions from the model SQL
3. Generate parameters for all columns except auto-increment PK
4. Create a `CREATE OR ALTER PROCEDURE`
5. Implement:
   - INSERT when PK = 0 or NULL
   - UPDATE when PK > 0
6. Save the SQL file

## Example

### Slash Command Input

```text
/create-sp Customer "Insert or update customer data"
