---
description: Create or update frontend JavaScript files following frontend standards
argument-hint: js_path | description
---

## Context

Parse `$ARGUMENTS` to get the following values:

- `[js_path]`: JavaScript file or pattern under `wwwroot/js/`
- `[description]`: Purpose of the JavaScript file

## Supported Patterns

| Pattern | Meaning |
|-------|--------|
| `wwwroot/js/customer.js` | Create or update a single JS file |
| `wwwroot/js/*.js` | Create or update all JS files in folder |
| `wwwroot/js/**/*.js` | Create or update JS files recursively |

## Standards Reference

- JavaScript creation **must follow instructions defined in**:
  - `Docs/frontend.md`
- Coding style, structure, naming, and patterns must strictly comply with documented frontend standards.

## Rules

- JavaScript files must be created under `wwwroot/js/`
- This command manages **ONLY JavaScript files**
- Use **create-or-update behavior**:
  - Create file if it does not exist
  - Update file if it already exists
- Do not include inline HTML or Razor syntax
- Follow patterns defined in `Docs/frontend.md`:
  - jQuery / AJAX usage (if specified)
  - Modular functions
  - Event binding standards
- Preserve existing custom logic unless explicitly instructed
- Add file-level header comments using the provided description

## Task

1. Read JavaScript standards from `Docs/frontend.md`
2. Resolve JS files using the provided path or pattern
3. Generate or update JavaScript structure:
   - Namespace or module pattern
   - DOM-ready initialization
   - AJAX placeholders (if defined in docs)
4. Save `.js` files in `wwwroot/js/`

## Example

### Slash Command Input

```text
/add-js wwwroot/js/customer.js "Customer page client-side logic"
