# Repository Pattern Documentation

## Repository Structure

For each database table, create an **interface** and **implementation** in the `Repos/` folder.

---

## 1. Interface — `Repos/Interfaces/I[Entity]Repository.cs`

Defines all CRUD operations for the entity.

**Characteristics:**
- All methods return `Task<ServiceResponse<T>>`
- Methods map to stored procedures from `SqlQueries/Master[Entity].sql`
- All methods are async

**Example:** `ICustomerRepository.cs`
```csharp
using SimpleCRM.Models;
using SimpleCRM.Models.Customer;

namespace SimpleCRM.Repos.Interfaces
{
    public interface ICustomerRepository
    {
        Task<ServiceResponse<int>> InsertUpdateAsync(CustomerRequest request);
        Task<ServiceResponse<IEnumerable<CustomerResponse>>> GetActiveAsync();
        Task<ServiceResponse<CustomerResponse>> GetByIdAsync(int id);
        Task<ServiceResponse<IEnumerable<CustomerResponse>>> GetAllAsync();
        Task<ServiceResponse<int>> DeleteAsync(int id);
        Task<ServiceResponse<int>> SoftDeleteAsync(int id);
    }
}
```

---

## 2. Implementation — `Repos/Services/[Entity]Repository.cs`

Implements the interface using **Dapper** and **DapperContext**.

**Characteristics:**
- Injects `DapperContext` via constructor
- Calls stored procedures with `CommandType.StoredProcedure`
- Wraps results in `ServiceResponse` for consistent error handling
- All methods use `try-catch` for error handling

**Example:** `CustomerRepository.cs`
```csharp
using Dapper;
using SimpleCRM.Data;
using SimpleCRM.Models;
using SimpleCRM.Models.Customer;
using SimpleCRM.Repos.Interfaces;
using System.Data;

namespace SimpleCRM.Repos.Services
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DapperContext _context;

        public CustomerRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<int>> InsertUpdateAsync(CustomerRequest request)
        {
            try
            {
                using var connection = _context.CreateConnection();
                var parameters = new
                {
                    request.Id,
                    request.CompanyName,
                    request.Email,
                    request.Phone,
                    request.IsActive
                };

                var result = await connection.ExecuteScalarAsync<int>(
                    "ssp_Customer_InsertUpdate",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var message = request.Id == 0 ? "Customer added successfully" : "Customer updated successfully";
                return ServiceResponse<int>.Success(result, message);
            }
            catch (Exception ex)
            {
                return ServiceResponse<int>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<IEnumerable<CustomerResponse>>> GetActiveAsync()
        {
            try
            {
                using var connection = _context.CreateConnection();
                var customers = await connection.QueryAsync<CustomerResponse>(
                    "ssp_Customer_GetActive",
                    commandType: CommandType.StoredProcedure
                );

                return ServiceResponse<IEnumerable<CustomerResponse>>.Success(customers);
            }
            catch (Exception ex)
            {
                return ServiceResponse<IEnumerable<CustomerResponse>>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<CustomerResponse>> GetByIdAsync(int id)
        {
            try
            {
                using var connection = _context.CreateConnection();
                var customer = await connection.QuerySingleOrDefaultAsync<CustomerResponse>(
                    "ssp_Customer_GetById",
                    new { Id = id },
                    commandType: CommandType.StoredProcedure
                );

                if (customer == null)
                    return ServiceResponse<CustomerResponse>.Failure("Customer not found");

                return ServiceResponse<CustomerResponse>.Success(customer);
            }
            catch (Exception ex)
            {
                return ServiceResponse<CustomerResponse>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<IEnumerable<CustomerResponse>>> GetAllAsync()
        {
            try
            {
                using var connection = _context.CreateConnection();
                var customers = await connection.QueryAsync<CustomerResponse>(
                    "ssp_Customer_GetAll",
                    commandType: CommandType.StoredProcedure
                );

                return ServiceResponse<IEnumerable<CustomerResponse>>.Success(customers);
            }
            catch (Exception ex)
            {
                return ServiceResponse<IEnumerable<CustomerResponse>>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<int>> DeleteAsync(int id)
        {
            try
            {
                using var connection = _context.CreateConnection();
                var result = await connection.ExecuteAsync(
                    "ssp_Customer_Delete",
                    new { Id = id },
                    commandType: CommandType.StoredProcedure
                );

                return ServiceResponse<int>.Success(result, "Customer deleted successfully");
            }
            catch (Exception ex)
            {
                return ServiceResponse<int>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<int>> SoftDeleteAsync(int id)
        {
            try
            {
                using var connection = _context.CreateConnection();
                var result = await connection.ExecuteAsync(
                    "ssp_Customer_SoftDelete",
                    new { Id = id },
                    commandType: CommandType.StoredProcedure
                );

                return ServiceResponse<int>.Success(result, "Customer deactivated successfully");
            }
            catch (Exception ex)
            {
                return ServiceResponse<int>.Failure($"Error: {ex.Message}");
            }
        }
    }
}
```

---

## Standard Repository Methods

Every repository implements these **6 CRUD methods** (matching stored procedures):

| Method | Return Type | Stored Procedure | Description |
|--------|-------------|------------------|-------------|
| `InsertUpdateAsync(request)` | `Task<ServiceResponse<int>>` | `ssp_[Table]_InsertUpdate` | Insert (Id=0) or Update (Id>0) |
| `GetActiveAsync()` | `Task<ServiceResponse<IEnumerable<Response>>>` | `ssp_[Table]_GetActive` | Get all active records |
| `GetByIdAsync(id)` | `Task<ServiceResponse<Response>>` | `ssp_[Table]_GetById` | Get single record by Id |
| `GetAllAsync()` | `Task<ServiceResponse<IEnumerable<Response>>>` | `ssp_[Table]_GetAll` | Get all records (active + inactive) |
| `DeleteAsync(id)` | `Task<ServiceResponse<int>>` | `ssp_[Table]_Delete` | Physical delete (remove record) |
| `SoftDeleteAsync(id)` | `Task<ServiceResponse<int>>` | `ssp_[Table]_SoftDelete` | Soft delete (set IsActive=0) |

---

## Dapper Usage Patterns

### QueryAsync<T> — Multiple rows
```csharp
var customers = await connection.QueryAsync<CustomerResponse>(
    "ssp_Customer_GetActive",
    commandType: CommandType.StoredProcedure
);
```

### QuerySingleOrDefaultAsync<T> — Single row
```csharp
var customer = await connection.QuerySingleOrDefaultAsync<CustomerResponse>(
    "ssp_Customer_GetById",
    new { Id = id },
    commandType: CommandType.StoredProcedure
);
```

### ExecuteAsync — Insert/Update/Delete (returns rows affected)
```csharp
var rowsAffected = await connection.ExecuteAsync(
    "ssp_Customer_Delete",
    new { Id = id },
    commandType: CommandType.StoredProcedure
);
```

### ExecuteScalarAsync<T> — Returns single value (e.g., new Id)
```csharp
var newId = await connection.ExecuteScalarAsync<int>(
    "ssp_Customer_InsertUpdate",
    parameters,
    commandType: CommandType.StoredProcedure
);
```

---

## DapperContext Usage

**Inject** `DapperContext` in constructor:
```csharp
private readonly DapperContext _context;

public CustomerRepository(DapperContext context)
{
    _context = context;
}
```

**Create connection** in each method:
```csharp
using var connection = _context.CreateConnection();
```

---

## Repository Registration in Program.cs

Register repositories with **Scoped** lifetime:
```csharp
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
```

---

## Error Handling Pattern

All methods use `try-catch` and return `ServiceResponse`:

```csharp
try
{
    using var connection = _context.CreateConnection();
    var result = await connection.QueryAsync<T>(...);
    return ServiceResponse<T>.Success(result);
}
catch (Exception ex)
{
    return ServiceResponse<T>.Failure($"Error: {ex.Message}");
}
```

---

## Naming Conventions

- **Interface:** `I[Entity]Repository` (e.g., `ICustomerRepository`)
- **Implementation:** `[Entity]Repository` (e.g., `CustomerRepository`)
- **Location:**
  - Interfaces: `Repos/Interfaces/`
  - Implementations: `Repos/Services/`
