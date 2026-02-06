# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Project:** Simple CRM
**Tech Stack:** ASP.NET Core 8 MVC, Razor Views, jQuery, Bootstrap 5, Dapper, MS SQL Server

SimpleCRM is an ASP.NET Core 8.0 MVC application for customer relationship management. The project uses the standard MVC pattern with Razor views, Dapper for data access, and MS SQL Server as the database.

## Build and Run Commands

```bash
# Build the project
dotnet build

# Run the application (defaults to http://localhost:5076)
dotnet run

# Run with specific profile
dotnet run --launch-profile http   # HTTP only on port 5076
dotnet run --launch-profile https  # HTTPS on 7213, HTTP on 5076

# Clean build artifacts
dotnet clean

# Restore NuGet packages
dotnet restore

# Build and run in watch mode (auto-reload on file changes)
dotnet watch run
```

## Architecture

### Project Structure

The application follows a layered architecture pattern:

- **Controllers/**: MVC controllers handling HTTP requests and routing
- **Models/**: Entity models organized by feature
  - **Models/[Entity]/**: Entity-specific models (Entity, Request, Response)
  - **Models/ServiceResponse.cs**: Global response wrapper
- **Views/**: Razor views organized by controller
  - **Views/Shared/**: Shared layouts and partial views
- **Repos/**: Repository layer for data access
  - **Repos/Interfaces/**: Repository interfaces (I[Entity]Repository)
  - **Repos/Services/**: Repository implementations using Dapper
- **Services/**: Business logic layer for application services (future)
- **Data/**: Database context (DapperContext)
- **SqlQueries/**: SQL scripts for tables and stored procedures
- **wwwroot/**: Static files (CSS, JS, images, client libraries)

### Middleware Pipeline

The application configures middleware in Program.cs with the following order:
1. Exception handler (non-development environments)
2. HTTPS redirection
3. Static files serving
4. Routing
5. Authorization

### MVC Routing

Default route pattern: `{controller=Home}/{action=Index}/{id?}`
- Routes to HomeController.Index() by default
- Optional ID parameter for resource-specific actions

### Dependency Injection

Services are registered in Program.cs using `builder.Services`. Controllers receive dependencies through constructor injection (see HomeController.cs:11 for ILogger injection example).

### View Layout

- Main layout: Views/Shared/_Layout.cshtml
- Bootstrap 5 navigation and responsive layout
- jQuery, Bootstrap JS, and validation scripts included
- Section for additional scripts: `@section Scripts { }`

## Coding Conventions

### Data Access
- Use async/await for all database calls
- Implement repository pattern with Dapper
- Use parameterized SQL queries only (never string concatenation)
- Repositories should return entity models

### ViewModels
- Use ViewModels for passing data to views
- Suffix all ViewModels with "ViewModel" (e.g., CustomerViewModel)
- Keep ViewModels in Models/ folder alongside entity models

### Models Structure
For each database table, create the following C# model classes in the Models/ folder:

1. **Entity Model** - `[TableName].cs`
   - Represents the database table structure
   - Properties match table columns exactly
   - Nullable properties for optional database fields
   - Example: `Customer.cs`

2. **Request Model** - `[TableName]Request.cs`
   - Used for Insert/Update operations (ssp_[Table]_InsertUpdate)
   - Includes validation attributes (Required, StringLength, EmailAddress, etc.)
   - Excludes auto-generated fields (CreatedAt)
   - Id property: 0 for Insert, >0 for Update
   - Example: `CustomerRequest.cs`

3. **Response Model** - `[TableName]Response.cs`
   - Used for API/View responses
   - Can include computed properties (e.g., Status)
   - Maps from entity model or stored procedure results
   - Example: `CustomerResponse.cs`

4. **ServiceResponse** - `ServiceResponse.cs` (shared across all tables)
   - Generic wrapper for all repository operations
   - Contains: IsSuccess, Message, Data
   - Provides Success() and Failure() static methods
   - Use `ServiceResponse<T>` for typed responses
   - Use `ServiceResponse` (non-generic) when no specific data type needed

### Model Naming Pattern
- Entity: `Customer`, `Contact`, `Order`
- Request: `CustomerRequest`, `ContactRequest`, `OrderRequest`
- Response: `CustomerResponse`, `ContactResponse`, `OrderResponse`
- All models use PascalCase for class and property names

### Model Folder Organization
- **Models/[Entity]/**: Create a subfolder for each entity
  - Example: `Models/Customer/` contains Customer.cs, CustomerRequest.cs, CustomerResponse.cs
- **Models/ServiceResponse.cs**: Keep global/shared models in root Models/ folder
- **Models/ErrorViewModel.cs**: Keep shared ViewModels in root Models/ folder

### Repository Pattern
For each database table, create an interface and implementation in the Repos/ folder:

1. **Interface** - `Repos/Interfaces/I[Entity]Repository.cs`
   - Defines all CRUD operations for the entity
   - Methods map to stored procedures from SqlQueries/Master[Entity].sql
   - All methods return `Task<ServiceResponse<T>>`
   - Example: `ICustomerRepository.cs`

2. **Implementation** - `Repos/Services/[Entity]Repository.cs`
   - Implements the interface using Dapper
   - Injects DapperContext via constructor
   - Calls stored procedures with `CommandType.StoredProcedure`
   - Wraps results in ServiceResponse for consistent error handling
   - Example: `CustomerRepository.cs`

### Standard Repository Methods
Every repository should implement these methods (matching the 6 CRUD stored procedures):

```csharp
Task<ServiceResponse<int>> InsertUpdateAsync([Entity]Request request);
Task<ServiceResponse<IEnumerable<[Entity]Response>>> GetActiveAsync();
Task<ServiceResponse<[Entity]Response>> GetByIdAsync(int id);
Task<ServiceResponse<IEnumerable<[Entity]Response>>> GetAllAsync();
Task<ServiceResponse<int>> DeleteAsync(int id);
Task<ServiceResponse<int>> SoftDeleteAsync(int id);
```

### Repository Registration
Register repositories in Program.cs using Scoped lifetime:
```csharp
builder.Services.AddScoped<I[Entity]Repository, [Entity]Repository>();
```
Example: `builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();`

## Controllers and Views (Single Page CRUD Pattern)

### Controller Structure
Every entity controller has **2 MVC actions** and **6 API endpoints**:

**MVC View Actions (only 2):**
- `Index()` - GET - Display entity list page (data loaded via JavaScript from hidden model)
- `Details(int id)` - GET - Display entity details page

**API Endpoints (for AJAX/JSON):**
- `InsertUpdate([FromBody] [Entity]Request)` - POST - Insert/Update via JSON
- `GetActive[Entities]()` - GET - Get all active entities as JSON
- `GetAll[Entities]()` - GET - Get all entities as JSON
- `Get[Entity]ById(int id)` - GET - Get single entity as JSON
- `Delete[Entity]([FromBody] DeleteRequest)` - POST - Physical delete via JSON
- `SoftDelete[Entity]([FromBody] DeleteRequest)` - POST - Soft delete via JSON

### View Files (only 2)
Create only **2 Razor views** in `Views/[Entity]/`:

1. **Index.cshtml** - Single page for list + Add/Edit/Delete
   - Hidden input with serialized model: `<input id="HiddenModel" type="hidden" value="@CommonHelpers.SerializeObject(Model)" />`
   - Responsive table for entity list
   - Add button opens modal
   - Edit button per row opens modal with auto-filled data
   - Delete button per row shows confirmation modal with entity name
   - Bootstrap modals for Add/Edit and Delete confirmation
   - All operations via AJAX - no page reload
   - After Add/Update/Delete, recall API and re-render table

2. **Details.cshtml** - Simple details view
   - Display all entity properties
   - Back to List button

### JavaScript Structure (1 file per entity)
Create `wwwroot/js/[entity].js` with:

**Initialization:**
- Load data from `#HiddenModel`
- Render table dynamically
- Bind all event handlers

**Validation (client-side with jQuery):**
- **Email**: Proper email format validation, show red border + error message if invalid
- **Phone**: Only numeric characters (keyup event filters non-numeric), max 10 digits
- **Company Name**: Required, max 150 characters
- **Invalid fields**: Red border (`border-danger` class) + error message below input
- **On edit**: Clear error message and red border

**AJAX Operations:**
- Before API call: Validate all fields
- Success: Show success toaster, close modal, reload data
- Error: Show error toaster with message
- All calls use JSON (`contentType: 'application/json'`)

**IsActive Field:**
- Use switch button (`form-check-switch`)
- Default always `true` for new entities

### Index.cshtml Pattern
```html
@using SimpleCRM.Helpers
@model IEnumerable<SimpleCRM.Models.[Entity].[Entity]Response>

@{
    var FileVersion = DateTime.Now.Ticks;
}

<input id="HiddenModel" type="hidden" value="@CommonHelpers.SerializeObject(Model)" />

<!-- Table with dynamic tbody -->
<table id="[entity]Table">
    <tbody id="[entity]TableBody"></tbody>
</table>

<!-- Add/Edit Modal -->
<div class="modal" id="[entity]Modal">
    <form id="[entity]Form">
        <input type="hidden" id="[entity]Id" value="0" />
        <!-- Form fields with validation -->
    </form>
</div>

<!-- Delete Confirmation Modal -->
<div class="modal" id="deleteModal">
    <p>Delete <strong id="delete[Entity]Name"></strong>?</p>
</div>

@section Scripts {
    <script defer src="/js/[entity].js?v=@FileVersion"></script>
}
```

### JavaScript Pattern (wwwroot/js/[entity].js)
```javascript
(function () {
    let entities = [];

    $(document).ready(function () {
        loadEntitiesFromHiddenModel();
        renderEntityTable();
        bindEvents();
    });

    function loadEntitiesFromHiddenModel() {
        const hiddenData = $('#HiddenModel').val();
        entities = JSON.parse(hiddenData);
    }

    function renderEntityTable() {
        // Render table rows from entities array
    }

    function saveEntity() {
        if (!validateForm()) return;

        $.ajax({
            url: '/[Entity]/InsertUpdate',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(entityData),
            success: function (response) {
                if (response.isSuccess) {
                    showToaster(response.message, 'success');
                    reloadEntities();
                }
            }
        });
    }

    function reloadEntities() {
        $.ajax({
            url: '/[Entity]/GetActive[Entities]',
            success: function (response) {
                entities = response.data;
                renderEntityTable();
            }
        });
    }

    function validateForm() {
        // Email validation
        if (!isValidEmail(email)) {
            setFieldError($('#email'), 'Invalid email');
            return false;
        }
        // Phone validation (numeric only, max 10)
        if (!/^\d{1,10}$/.test(phone)) {
            setFieldError($('#phone'), 'Max 10 numeric digits');
            return false;
        }
        return true;
    }

    function showToaster(message, type) {
        // Bootstrap toast notification
    }
})();
```

### Validation Rules (All CRUD Operations)
- **Email**: Regex validation `/^[^\s@]+@[^\s@]+\.[^\s@]+$/`, required
- **Phone**: Numeric only (filter on keyup), max 10 characters, **required**
- **Company Name**: Required, max 150 characters
- **IsActive**: Switch button, default `true`
- **Error Display**: Red border + error message below field
- **Clear Errors**: On input change/edit

### Script Reference Pattern
```csharp
@section Scripts {
    <script defer src="/js/[entity].js?v=@FileVersion" type="text/javascript"></script>
}
```
Note: FileVersion as query parameter for cache busting, not in filename.

### Bootstrap Responsive Design
- Use `col-md-6 col-12` for responsive columns
- Use `text-md-end text-start` for responsive text alignment
- Use `table-responsive` for mobile-friendly tables
- Use `modal-lg` for large modals
- Test on desktop, tablet, and mobile

### Navigation Menu
Add entity link to `Views/Shared/_Layout.cshtml`:
```html
<li class="nav-item">
    <a class="nav-link text-dark" asp-controller="[Entity]" asp-action="Index">[Entities]</a>
</li>
```

### Client-Side
- Use jQuery for client-side interactions
- Place custom JavaScript in wwwroot/js/
- Use unobtrusive validation with data attributes

### Asynchronous Programming
- All controller actions with database calls should be async
- Repository methods should return Task<T> or Task<IEnumerable<T>>
- Service methods should be async when calling repositories

## Database

### Connection String
- Store connection string in appsettings.json under "ConnectionStrings:DefaultConnection"
- Use different connection strings for Development and Production environments

### Naming Conventions
- **Table names**: Singular (e.g., Customer, Contact, Order)
- **Primary keys**: "Id" with IDENTITY(1,1) for auto-increment
- **Foreign keys**: Referenced table name + "Id" (e.g., CustomerId in Contact table)
- **Stored Procedures**: Prefix with "ssp_" (e.g., ssp_Customer_GetActive, ssp_Customer_InsertUpdate)

### Table Structure Standards
- **Primary Key**: Always use `Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY`
- **Required Fields**: Use NOT NULL constraint
- **Optional Fields**: Use NULL for optional columns (Phone, Address, etc.)
- **Audit Fields**: Include `CreatedAt DATETIME NOT NULL DEFAULT GETDATE()`
- **Soft Delete**: Include `IsActive BIT NOT NULL DEFAULT 1` for soft delete capability
- **String Fields**: Use NVARCHAR with appropriate max length

### Stored Procedure Standards
- **Prefix**: All stored procedures must start with `ssp_`
- **CREATE OR ALTER**: Always use `CREATE OR ALTER PROCEDURE` for all stored procedures
- **Naming Pattern**: `ssp_[TableName]_[Action]` (e.g., ssp_Customer_GetActive, ssp_Customer_Delete)
- **Error Handling**: Wrap in TRY-CATCH blocks with proper error raising

### CRUD Stored Procedures Pattern
Every table should have the following stored procedures:
1. **ssp_[Table]_InsertUpdate**: Insert if @Id = 0, Update if @Id > 0
   - Do NOT include CreatedAt parameter (auto-generated on INSERT)
   - Return the Id after INSERT or UPDATE
2. **ssp_[Table]_GetActive**: Get all active records (WHERE IsActive = 1)
3. **ssp_[Table]_GetById**: Get single record by Id
4. **ssp_[Table]_GetAll**: Get all records (active and inactive)
5. **ssp_[Table]_Delete**: Physical delete (removes record)
6. **ssp_[Table]_SoftDelete**: Soft delete (sets IsActive = 0)

### SQL Files Organization
- All SQL scripts are stored in `SqlQueries/` folder
- File naming: `Master[TableName].sql` (e.g., MasterCustomer.sql)
- Each file contains: Table creation + all CRUD stored procedures

### Query Guidelines
- Use stored procedures for all CRUD operations
- Call stored procedures using Dapper with parameter objects
- Always use parameterized queries with Dapper's parameter binding
- Example: `await connection.QueryAsync<Customer>("ssp_Customer_GetActive", commandType: CommandType.StoredProcedure)`

### Dapper Usage
- Use `QueryAsync<T>` for SELECT queries returning multiple rows
- Use `QuerySingleOrDefaultAsync<T>` for single row queries
- Use `ExecuteAsync` for INSERT, UPDATE, DELETE operations
- Leverage Dapper's multi-mapping for joins when needed

## Client Libraries

Located in wwwroot/lib/:
- **Bootstrap 5**: CSS framework for responsive design
- **jQuery**: DOM manipulation and AJAX
- **jQuery Validation**: Client-side form validation
- **jQuery Validation Unobtrusive**: Integration with ASP.NET Core validation attributes

## Configuration

- **appsettings.json**: Main configuration file
- **appsettings.Development.json**: Development-specific overrides
- **launchSettings.json**: Local development profiles and ports

## Important Notes

- Target framework is .NET 8.0 (not .NET 9.0)
- Nullable reference types are enabled
- Implicit usings are enabled
- Use `UseStaticFiles()` for static file serving (not MapStaticAssets which is .NET 9.0+)
