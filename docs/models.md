# Models Documentation

## 3-Model Pattern

For each database table, create the following C# model classes in `Models/[Entity]/` folder:

### 1. Entity Model — `[TableName].cs`
Represents the database table structure exactly.

**Characteristics:**
- Properties match table columns (same names, types)
- Nullable properties for optional database fields
- No validation attributes
- Example: `Customer.cs`

**Example:**
```csharp
namespace SimpleCRM.Models.Customer
{
    public class Customer
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
```

---

### 2. Request Model — `[TableName]Request.cs`
Used for Insert/Update operations via `ssp_[Table]_InsertUpdate`.

**Characteristics:**
- Includes validation attributes (`Required`, `StringLength`, `EmailAddress`, etc.)
- Excludes auto-generated fields (`CreatedAt`)
- `Id = 0` for Insert, `Id > 0` for Update
- Example: `CustomerRequest.cs`

**Example:**
```csharp
using System.ComponentModel.DataAnnotations;

namespace SimpleCRM.Models.Customer
{
    public class CustomerRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Company name is required")]
        [StringLength(150, ErrorMessage = "Company name cannot exceed 150 characters")]
        public string CompanyName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [StringLength(10, ErrorMessage = "Phone cannot exceed 10 digits")]
        public string? Phone { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
```

---

### 3. Response Model — `[TableName]Response.cs`
Used for API/View responses.

**Characteristics:**
- Can include computed properties (e.g., `Status` derived from `IsActive`)
- Maps from entity model or stored procedure results
- Example: `CustomerResponse.cs`

**Example:**
```csharp
namespace SimpleCRM.Models.Customer
{
    public class CustomerResponse
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        // Computed property
        public string Status => IsActive ? "Active" : "Inactive";
    }
}
```

---

## ServiceResponse (Shared Wrapper)

**File:** `Models/ServiceResponse.cs` (root Models folder, not in entity subfolder)

Generic wrapper for all repository operations.

**Properties:**
- `IsSuccess` — Operation succeeded?
- `Message` — Success/error message
- `Data` — Result data (generic type `T`)

**Static Methods:**
- `ServiceResponse<T>.Success(T data, string message)` — Create success response
- `ServiceResponse<T>.Failure(string message)` — Create error response

**Example:**
```csharp
namespace SimpleCRM.Models
{
    public class ServiceResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static ServiceResponse<T> Success(T data, string message = "Success")
        {
            return new ServiceResponse<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
        }

        public static ServiceResponse<T> Failure(string message)
        {
            return new ServiceResponse<T>
            {
                IsSuccess = false,
                Message = message
            };
        }
    }
}
```

---

## Naming Conventions

### Model Names
- **Entity:** `Customer`, `Contact`, `Order`
- **Request:** `CustomerRequest`, `ContactRequest`, `OrderRequest`
- **Response:** `CustomerResponse`, `ContactResponse`, `OrderResponse`

### Folder Organization
- **Entity-specific models:** `Models/[Entity]/` subfolder
  - Example: `Models/Customer/` contains `Customer.cs`, `CustomerRequest.cs`, `CustomerResponse.cs`
- **Shared models:** `Models/` root folder
  - Example: `Models/ServiceResponse.cs`, `Models/ErrorViewModel.cs`

### Property Naming
- Use **PascalCase** for all class and property names
- Match database column names exactly in Entity models
- Use nullable types (`string?`, `int?`) for optional fields

---

## Validation Attributes

Use in **Request models** only:

| Attribute | Usage | Example |
|-----------|-------|---------|
| `[Required]` | Field is mandatory | `[Required(ErrorMessage = "Name is required")]` |
| `[StringLength]` | Max length | `[StringLength(150)]` |
| `[EmailAddress]` | Valid email format | `[EmailAddress]` |
| `[Phone]` | Valid phone format | `[Phone]` |
| `[Range]` | Numeric range | `[Range(1, 100)]` |

---

## ViewModels

For passing data to views (non-entity data):

**Naming:** Suffix with `ViewModel`
**Location:** `Models/` root folder
**Example:** `Models/ErrorViewModel.cs`

```csharp
namespace SimpleCRM.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
```
