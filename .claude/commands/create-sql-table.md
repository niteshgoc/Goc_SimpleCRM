---
description: Create a SQL table file using table name and description
argument-hint: table_name | description
---

## Context

Parse `$ARGUMENTS` to get the following values:

- `[table_name]`: Name of the table (snake_case recommended)
- `[description]`: Short description of what the table is used for

## Rules

- Always use `CREATE TABLE IF NOT EXISTS`
- PRIMARY KEY is **mandatory**
- Default PRIMARY KEY column:
  - `id INT NOT NULL AUTO_INCREMENT`
- SQL must be **MySQL compatible**
- Prompt the user to define remaining columns based on the description
- Generate a separate `.sql` file inside `/SqlQueries/Tables/`
- File name must match the table name

## Task

1. Accept only:
   - Table name
   - Table description
2. Infer or ask for required columns based on the description
3. Automatically add:
   - `id INT AUTO_INCREMENT PRIMARY KEY`
4. Generate a valid SQL `CREATE TABLE` statement
5. Save the SQL in `/SqlQueries/Tables/{table_name}.sql`
6. Include a usage example

## Example

### Slash Command Input

```text
/sql-create-table users "Stores application user accounts"
