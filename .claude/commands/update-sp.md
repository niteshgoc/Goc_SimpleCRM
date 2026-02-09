
---

## ✅ Slash Command 2: Create or Update SP (Custom Name)

```md
---
description: Create or update a named stored procedure for an existing database model
argument-hint: model_name | sp_name | description
---

## Context

Parse `$ARGUMENTS` to get the following values:

- `[model_name]`: Name of the model that exists in `/SqlQueries/Tables/`
- `[sp_name]`: Custom stored procedure name
- `[description]`: Purpose of the stored procedure

## Rules

- Model must exist in `/SqlQueries/Tables/`
- Stored Procedure name is provided explicitly
- Always use `CREATE OR ALTER PROCEDURE`
- Procedure logic is derived from model columns
- PRIMARY KEY is used where applicable
- SQL must be MySQL-compatible
- Generate a `.sql` file in `/SqlQueries/Tables/sp/`
- File name must match the stored procedure name

## Task

1. Validate that the model exists in `/SqlQueries/Tables/`
2. Read column definitions from the model SQL
3. Generate SP parameters based on table schema
4. Create a `CREATE OR ALTER PROCEDURE`
5. Save the SQL file

## Example

### Slash Command Input

```text
/create-sp Customer sp_customer_get_by_id "Get customer by id"
