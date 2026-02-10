# Database Connection - READ-ONLY Access

## Access Policy

⚠️ **IMPORTANT: This database is accessed in READ-ONLY mode ONLY**

### Permissions Granted
✅ **SELECT queries only**
✅ Reading table structures
✅ Understanding relationships, keys, and constraints
✅ Schema and data analysis for application design purposes

### Permissions DENIED
❌ CREATE - No table, view, procedure, or object creation
❌ ALTER - No modifications to existing database objects
❌ DROP - No deletion of database objects
❌ INSERT - No data insertion
❌ UPDATE - No data modification
❌ DELETE - No data deletion
❌ TRUNCATE - No table truncation
❌ Any DDL (Data Definition Language) operations
❌ Any DML (Data Manipulation Language) write operations
❌ Any database object creation, modification, or deletion

## Connection Details

**Server:** 156.67.104.130,1433
**Database:** NtsDb
**User:** goc_user
**Purpose:** Read-only analysis and application design

## Usage

This connection is used exclusively for:
- Understanding existing database schema
- Analyzing table structures and relationships
- Designing the .NET Core application based on existing data model
- No modifications will be made to the database

## Connection String (for reference)

```
Server=156.67.104.130,1433;Database=NtsDb;User Id=goc_user;Password=Z7!qM9#L@R2s$Wk9;MultipleActiveResultSets=true;Max Pool Size=32767;TrustServerCertificate=True;Encrypt=False;
```

---

**Last Updated:** 2026-02-10
**Compliance:** All database interactions comply with READ-ONLY restrictions
