---
description: Add or update a project feature based on documentation instructions
argument-hint: feature_name | description
---

## Context

Parse `$ARGUMENTS` to get the following values:

- `[feature_name]`: Name of the feature (must match a `.md` file in `/.claude/docs/features/`)
- `[description]`: Purpose or summary of the feature

## Feature Source

- Feature instructions must be read from:
  - `/.claude/docs/features/{feature_name}.md`
- This file is the **single source of truth** for the feature behavior.

## Rules

- Feature file **must exist** in `/.claude/docs/features/`
- The command must **not guess or invent behavior**
- All changes must strictly follow the instructions in the feature MD
- Use **add-or-update behavior**:
  - Add missing components
  - Update existing components to comply with the feature
- Preserve unrelated project logic
- Do not remove existing functionality unless explicitly stated in the feature doc
- Log or summarize applied changes

## Task

1. Validate that `/.claude/docs/features/{feature_name}.md` exists
2. Read and understand feature instructions
3. Identify required changes (e.g.):
   - Database changes
   - Models
   - Repositories
   - Services
   - Controllers
   - Views
   - JavaScript
   - Config or constants
4. Apply changes exactly as described in the feature document
5. Update or create files as needed
6. Ensure the project conforms to the feature standards

## Example

### Slash Command Input

```text
/add-feature user-authentication "Enable user login and role-based access"
