# Database Documentation

## Connection String

**Location:** `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SimpleCRM;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**DapperContext** reads this connection string via `IConfiguration` and provides `IDbConnection`.

---

## DapperContext

**File:** `Data/DapperContext.cs`

```csharp
using System.Data;
using Microsoft.Data.SqlClient;

namespace SimpleCRM.Data
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")!;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
```

**Registration in Program.cs:**
```csharp
builder.Services.AddSingleton<DapperContext>();
```

---

## Database Naming Conventions

### Tables
- **Singular naming:** `Customer`, `Contact`, `Order` (not plural)
- **PascalCase:** Use uppercase first letters

### Columns
- **Primary Key:** `Id` (always `INT IDENTITY(1,1)`)
- **Foreign Keys:** `[ReferencedTable]Id` (e.g., `CustomerId` in `Contact` table)
- **PascalCase:** Match C# property names exactly

### Stored Procedures
- **Prefix:** `ssp_` (all stored procedures)
- **Naming Pattern:** `ssp_[TableName]_[Action]`
- **Examples:**
  - `ssp_Customer_GetActive`
  - `ssp_Customer_InsertUpdate`
  - `ssp_Customer_Delete`

---

## Table Structure Standards

Every table should have these standard columns:

| Column | Data Type | Constraint | Description |
|--------|-----------|------------|-------------|
| `Id` | `INT` | `IDENTITY(1,1) PRIMARY KEY` | Auto-increment primary key |
| `CreatedAt` | `DATETIME` | `NOT NULL DEFAULT GETDATE()` | Auto-generated timestamp |
| `IsActive` | `BIT` | `NOT NULL DEFAULT 1` | Soft delete flag (1=Active, 0=Deleted) |

**Additional columns:**
- **Required fields:** Use `NOT NULL`
- **Optional fields:** Use `NULL`
- **String fields:** Use `NVARCHAR(length)`

**Example:**
```sql
CREATE TABLE Customer (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CompanyName NVARCHAR(150) NOT NULL,
    Email NVARCHAR(100) NULL,
    Phone NVARCHAR(10) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsActive BIT NOT NULL DEFAULT 1
);
```

---

## CRUD Stored Procedures Pattern

Every table has **6 stored procedures**:

### 1. ssp_[Table]_InsertUpdate
Insert if `@Id = 0`, Update if `@Id > 0`.

**Notes:**
- Do NOT include `@CreatedAt` parameter (auto-generated on INSERT)
- Return the `Id` after INSERT or UPDATE

**Example:**
```sql
CREATE OR ALTER PROCEDURE ssp_Customer_InsertUpdate
    @Id INT = 0,
    @CompanyName NVARCHAR(150),
    @Email NVARCHAR(100) = NULL,
    @Phone NVARCHAR(10) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    BEGIN TRY
        IF @Id = 0
        BEGIN
            INSERT INTO Customer (CompanyName, Email, Phone, IsActive)
            VALUES (@CompanyName, @Email, @Phone, @IsActive);

            SELECT SCOPE_IDENTITY() AS Id;
        END
        ELSE
        BEGIN
            UPDATE Customer
            SET CompanyName = @CompanyName,
                Email = @Email,
                Phone = @Phone,
                IsActive = @IsActive
            WHERE Id = @Id;

            SELECT @Id AS Id;
        END
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
```

---

### 2. ssp_[Table]_GetActive
Get all active records (`WHERE IsActive = 1`).

**Example:**
```sql
CREATE OR ALTER PROCEDURE ssp_Customer_GetActive
AS
BEGIN
    BEGIN TRY
        SELECT Id, CompanyName, Email, Phone, CreatedAt, IsActive
        FROM Customer
        WHERE IsActive = 1
        ORDER BY CreatedAt DESC;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
```

---

### 3. ssp_[Table]_GetById
Get single record by `Id`.

**Example:**
```sql
CREATE OR ALTER PROCEDURE ssp_Customer_GetById
    @Id INT
AS
BEGIN
    BEGIN TRY
        SELECT Id, CompanyName, Email, Phone, CreatedAt, IsActive
        FROM Customer
        WHERE Id = @Id;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
```

---

### 4. ssp_[Table]_GetAll
Get all records (active and inactive).

**Example:**
```sql
CREATE OR ALTER PROCEDURE ssp_Customer_GetAll
AS
BEGIN
    BEGIN TRY
        SELECT Id, CompanyName, Email, Phone, CreatedAt, IsActive
        FROM Customer
        ORDER BY CreatedAt DESC;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
```

---

### 5. ssp_[Table]_Delete
Physical delete (removes record permanently).

**Example:**
```sql
CREATE OR ALTER PROCEDURE ssp_Customer_Delete
    @Id INT
AS
BEGIN
    BEGIN TRY
        DELETE FROM Customer
        WHERE Id = @Id;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
```

---

### 6. ssp_[Table]_SoftDelete
Soft delete (sets `IsActive = 0`).

**Example:**
```sql
CREATE OR ALTER PROCEDURE ssp_Customer_SoftDelete
    @Id INT
AS
BEGIN
    BEGIN TRY
        UPDATE Customer
        SET IsActive = 0
        WHERE Id = @Id;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
```

---

## SQL Files Organization

**Location:** `SqlQueries/` folder
**Naming:** `Master[TableName].sql` (e.g., `MasterCustomer.sql`)
**Contents:** Table creation + all 6 CRUD stored procedures

**Example File Structure:**
```
SqlQueries/
  ├─ MasterCustomer.sql       -- Table + 6 stored procedures
  ├─ MasterContact.sql        -- Table + 6 stored procedures
  └─ MasterOrder.sql          -- Table + 6 stored procedures
```

---

## Stored Procedure Standards

- **Prefix:** All stored procedures start with `ssp_`
- **CREATE OR ALTER:** Always use `CREATE OR ALTER PROCEDURE` for all stored procedures
- **Error Handling:** Wrap all logic in `BEGIN TRY...END TRY` / `BEGIN CATCH...END CATCH`
- **Error Raising:** Use `THROW` in catch block

---

## Calling Stored Procedures with Dapper

**Always use:**
- `CommandType.StoredProcedure`
- Parameter objects (anonymous or `DynamicParameters`)
- Async methods (`QueryAsync`, `ExecuteAsync`, etc.)

**Example:**
```csharp
using var connection = _context.CreateConnection();
var customers = await connection.QueryAsync<CustomerResponse>(
    "ssp_Customer_GetActive",
    commandType: CommandType.StoredProcedure
);
```

**With parameters:**
```csharp
var customer = await connection.QuerySingleOrDefaultAsync<CustomerResponse>(
    "ssp_Customer_GetById",
    new { Id = id },
    commandType: CommandType.StoredProcedure
);
```

---

## Dapper Query Methods

| Method | Use Case | Returns |
|--------|----------|---------|
| `QueryAsync<T>` | SELECT multiple rows | `IEnumerable<T>` |
| `QuerySingleOrDefaultAsync<T>` | SELECT single row | `T` or `null` |
| `ExecuteAsync` | INSERT, UPDATE, DELETE | Rows affected (int) |
| `ExecuteScalarAsync<T>` | Return single value (e.g., new Id) | `T` |

---

## Foreign Key Relationships

**Naming:** `[ReferencedTable]Id`

**Example:** `Contact` table has `CustomerId` foreign key:

```sql
CREATE TABLE Contact (
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CustomerId INT NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    CONSTRAINT FK_Contact_Customer FOREIGN KEY (CustomerId) REFERENCES Customer(Id)
);
```
