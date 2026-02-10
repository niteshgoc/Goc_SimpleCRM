# Feature: State Selection for Customer Contact (feature-state0902)

## Purpose

Introduce a State master integration for Customer Contact management.

- Load State list dynamically from API
- Store State as INT (StateId) in database
- Auto-select State in UI during edit
- Follow project standards across all layers

---

## Scope of Changes

- **Database** (Table + Stored Procedures)
- **Models** (State entity, request, response)
- **Repository Interfaces** (IStateRepository, ICustomerContactRepository update)
- **Services** (StateRepository, CustomerContactRepository update)
- **API Controller** (GetStatesList endpoint in CustomerContactController)
- **Frontend JavaScript** (State dropdown load and selection logic)
- **MVC View** (Index.cshtml - convert State text input to dropdown)

---

## Database Changes

### 1. Create State Master Table

**Path:** `SqlQueries/Tables/tblState.sql`

```sql
-- =============================================
-- Table: tblState
-- Description: Master table for storing state information
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblState]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[tblState] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [StateName] VARCHAR(100) NOT NULL,
        [StateCode] VARCHAR(10) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedBy] INT NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] INT NULL,
        [UpdatedDate] DATETIME NULL,

        CONSTRAINT [PK_tblState] PRIMARY KEY CLUSTERED ([Id] ASC)
    )

    PRINT 'Table tblState created successfully.'
END
ELSE
BEGIN
    PRINT 'Table tblState already exists.'
END
GO

-- Create index on StateName for better query performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_tblState_StateName')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_tblState_StateName]
    ON [dbo].[tblState] ([StateName])
    INCLUDE ([IsActive])
END
GO

-- =============================================
-- Sample Data: Insert Common Indian States
-- =============================================
IF NOT EXISTS (SELECT * FROM tblState)
BEGIN
    INSERT INTO tblState (StateName, StateCode, IsActive, CreatedBy)
    VALUES
        ('Maharashtra', 'MH', 1, 1),
        ('Gujarat', 'GJ', 1, 1),
        ('Karnataka', 'KA', 1, 1),
        ('Tamil Nadu', 'TN', 1, 1),
        ('Delhi', 'DL', 1, 1),
        ('Uttar Pradesh', 'UP', 1, 1),
        ('West Bengal', 'WB', 1, 1),
        ('Rajasthan', 'RJ', 1, 1),
        ('Madhya Pradesh', 'MP', 1, 1),
        ('Telangana', 'TS', 1, 1),
        ('Andhra Pradesh', 'AP', 1, 1),
        ('Kerala', 'KL', 1, 1),
        ('Punjab', 'PB', 1, 1),
        ('Haryana', 'HR', 1, 1),
        ('Bihar', 'BR', 1, 1),
        ('Odisha', 'OR', 1, 1),
        ('Jharkhand', 'JH', 1, 1),
        ('Assam', 'AS', 1, 1),
        ('Chhattisgarh', 'CG', 1, 1),
        ('Uttarakhand', 'UK', 1, 1)

    PRINT 'Sample state data inserted successfully.'
END
GO
```

### 2. Update Customer Contact Table - Change State from VARCHAR to INT

**Path:** `SqlQueries/Tables/tblCustomerContact.sql`

**Action:** Modify the State column to be INT (foreign key to tblState)

```sql
-- =============================================
-- ALTER TABLE: Change State from VARCHAR to INT
-- =============================================

-- Check if State column is VARCHAR
IF EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'tblCustomerContact'
    AND COLUMN_NAME = 'State'
    AND DATA_TYPE = 'varchar'
)
BEGIN
    -- Step 1: Add new StateId column as INT
    IF NOT EXISTS (
        SELECT * FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'tblCustomerContact'
        AND COLUMN_NAME = 'StateId'
    )
    BEGIN
        ALTER TABLE [dbo].[tblCustomerContact]
        ADD [StateId] INT NULL

        PRINT 'StateId column added to tblCustomerContact.'
    END

    -- Step 2: Drop the old State VARCHAR column
    ALTER TABLE [dbo].[tblCustomerContact]
    DROP COLUMN [State]

    PRINT 'Old State (VARCHAR) column dropped from tblCustomerContact.'

    -- Step 3: Rename StateId to State
    EXEC sp_rename 'tblCustomerContact.StateId', 'State', 'COLUMN'

    PRINT 'StateId renamed to State in tblCustomerContact.'

    -- Step 4: Add foreign key constraint
    IF NOT EXISTS (
        SELECT * FROM sys.foreign_keys
        WHERE name = 'FK_tblCustomerContact_State'
    )
    BEGIN
        ALTER TABLE [dbo].[tblCustomerContact]
        ADD CONSTRAINT [FK_tblCustomerContact_State] FOREIGN KEY ([State])
            REFERENCES [dbo].[tblState]([Id])

        PRINT 'Foreign key constraint added to tblCustomerContact.State.'
    END
END
ELSE
BEGIN
    PRINT 'State column is already INT or does not exist.'
END
GO
```

**Updated table structure:**

```sql
-- Final table structure with State as INT
CREATE TABLE [dbo].[tblCustomerContact] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [CustomerId] INT NOT NULL,
    [MobileNo] VARCHAR(100) NULL,
    [Address] VARCHAR(100) NULL,
    [Email] VARCHAR(100) NULL,
    [City] VARCHAR(100) NULL,
    [State] INT NULL,  -- Changed from VARCHAR(100) to INT
    [PhoneNumber] VARCHAR(100) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedBy] INT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] INT NULL,
    [UpdatedDate] DATETIME NULL,

    CONSTRAINT [PK_tblCustomerContact] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_tblCustomerContact_Customer] FOREIGN KEY ([CustomerId])
        REFERENCES [dbo].[Customer]([Id]),
    CONSTRAINT [FK_tblCustomerContact_State] FOREIGN KEY ([State])
        REFERENCES [dbo].[tblState]([Id])
)
```

### 3. Create State Stored Procedures

#### 3.1 Get All Active States

**Path:** `SqlQueries/StoredProcedures/ssp_tblState_GetActive.sql`

```sql
-- =============================================
-- Stored Procedure: ssp_tblState_GetActive
-- Description: Retrieve all active states
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[ssp_tblState_GetActive]
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT
            [Id],
            [StateName],
            [StateCode],
            [IsActive],
            [CreatedBy],
            [CreatedDate],
            [UpdatedBy],
            [UpdatedDate]
        FROM [dbo].[tblState]
        WHERE [IsActive] = 1
        ORDER BY [StateName] ASC
    END TRY
    BEGIN CATCH
        -- Return error information
        SELECT
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_MESSAGE() AS ErrorMessage,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE() AS ErrorLine
    END CATCH
END
GO

-- =============================================
-- Usage Example:
-- =============================================
-- EXEC ssp_tblState_GetActive
```

### 4. Update Customer Contact Stored Procedures

#### 4.1 Update ssp_tblCustomerContact_GetAll

**Path:** `SqlQueries/StoredProcedures/ssp_tblCustomerContact_GetAll.sql`

**Changes:**
- Add LEFT JOIN with tblState to get StateName
- Return both State (INT) and StateName (VARCHAR) for display

```sql
-- =============================================
-- Stored Procedure: ssp_tblCustomerContact_GetAll
-- Description: Retrieve all customer contact records (with optional filters)
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[ssp_tblCustomerContact_GetAll]
    @CustomerId INT = NULL,
    @IsActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT
            cc.[Id],
            cc.[CustomerId],
            cc.[MobileNo],
            cc.[Address],
            cc.[Email],
            cc.[City],
            cc.[State],
            s.[StateName],
            cc.[PhoneNumber],
            cc.[IsActive],
            cc.[CreatedBy],
            cc.[CreatedDate],
            cc.[UpdatedBy],
            cc.[UpdatedDate],
            -- Join with Customer table for additional context
            c.[CompanyName] as CustomerName,
            c.[Email] as CustomerCode
        FROM [dbo].[tblCustomerContact] cc
        LEFT JOIN [dbo].[Customer] c ON cc.[CustomerId] = c.[Id]
        LEFT JOIN [dbo].[tblState] s ON cc.[State] = s.[Id]
        WHERE
            (@CustomerId IS NULL OR cc.[CustomerId] = @CustomerId)
            AND (@IsActive IS NULL OR cc.[IsActive] = @IsActive)
        ORDER BY cc.[CreatedDate] DESC
    END TRY
    BEGIN CATCH
        -- Return error information
        SELECT
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_MESSAGE() AS ErrorMessage,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE() AS ErrorLine
    END CATCH
END
GO
```

#### 4.2 Update ssp_tblCustomerContact_GetById

**Path:** `SqlQueries/StoredProcedures/ssp_tblCustomerContact_GetById.sql`

**Changes:**
- Add LEFT JOIN with tblState to get StateName
- Return both State (INT) and StateName (VARCHAR) for display

```sql
-- =============================================
-- Stored Procedure: ssp_tblCustomerContact_GetById
-- Description: Retrieve a single customer contact record by ID
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[ssp_tblCustomerContact_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT
            cc.[Id],
            cc.[CustomerId],
            cc.[MobileNo],
            cc.[Address],
            cc.[Email],
            cc.[City],
            cc.[State],
            s.[StateName],
            cc.[PhoneNumber],
            cc.[IsActive],
            cc.[CreatedBy],
            cc.[CreatedDate],
            cc.[UpdatedBy],
            cc.[UpdatedDate],
            -- Join with Customer table for additional context
            c.[CompanyName] as CustomerName,
            c.[Email] as CustomerCode
        FROM [dbo].[tblCustomerContact] cc
        LEFT JOIN [dbo].[Customer] c ON cc.[CustomerId] = c.[Id]
        LEFT JOIN [dbo].[tblState] s ON cc.[State] = s.[Id]
        WHERE cc.[Id] = @Id
    END TRY
    BEGIN CATCH
        -- Return error information
        SELECT
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_MESSAGE() AS ErrorMessage,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE() AS ErrorLine
    END CATCH
END
GO
```

#### 4.3 Update ssp_tblCustomerContact_InsertUpdate

**Path:** `SqlQueries/StoredProcedures/ssp_tblCustomerContact_InsertUpdate.sql`

**Changes:**
- Accept State as INT parameter

```sql
-- =============================================
-- Stored Procedure: ssp_tblCustomerContact_InsertUpdate
-- Description: Insert or update customer contact record
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[ssp_tblCustomerContact_InsertUpdate]
    @Id INT = 0,
    @CustomerId INT,
    @MobileNo VARCHAR(100) = NULL,
    @Address VARCHAR(100) = NULL,
    @Email VARCHAR(100) = NULL,
    @City VARCHAR(100) = NULL,
    @State INT = NULL,  -- Changed from VARCHAR to INT
    @PhoneNumber VARCHAR(100) = NULL,
    @IsActive BIT = 1,
    @CreatedBy INT = NULL,
    @UpdatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @Id = 0
        BEGIN
            -- INSERT new record
            INSERT INTO [dbo].[tblCustomerContact] (
                [CustomerId], [MobileNo], [Address], [Email], [City], [State],
                [PhoneNumber], [IsActive], [CreatedBy], [CreatedDate]
            )
            VALUES (
                @CustomerId, @MobileNo, @Address, @Email, @City, @State,
                @PhoneNumber, @IsActive, @CreatedBy, GETDATE()
            )

            SET @Id = SCOPE_IDENTITY()

            SELECT @Id AS Id, 'Customer contact created successfully' AS Message
        END
        ELSE
        BEGIN
            -- UPDATE existing record
            UPDATE [dbo].[tblCustomerContact]
            SET
                [CustomerId] = @CustomerId,
                [MobileNo] = @MobileNo,
                [Address] = @Address,
                [Email] = @Email,
                [City] = @City,
                [State] = @State,
                [PhoneNumber] = @PhoneNumber,
                [IsActive] = @IsActive,
                [UpdatedBy] = @UpdatedBy,
                [UpdatedDate] = GETDATE()
            WHERE [Id] = @Id

            SELECT @Id AS Id, 'Customer contact updated successfully' AS Message
        END
    END TRY
    BEGIN CATCH
        -- Return error information
        SELECT
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_MESSAGE() AS ErrorMessage,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE() AS ErrorLine
    END CATCH
END
GO
```

---

## Model Changes

### 1. Create State Models

#### 1.1 State Entity Model

**Path:** `Models/State/State.cs`

```csharp
namespace SimpleCRM.Models.State
{
    /// <summary>
    /// State entity model - maps to tblState table
    /// Stores state master data
    /// </summary>
    public class State
    {
        public int Id { get; set; }
        public string StateName { get; set; } = string.Empty;
        public string? StateCode { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
```

#### 1.2 State Response Model

**Path:** `Models/State/StateResponse.cs`

```csharp
namespace SimpleCRM.Models.State
{
    /// <summary>
    /// State response model - for API/View responses
    /// </summary>
    public class StateResponse
    {
        public int Id { get; set; }
        public string StateName { get; set; } = string.Empty;
        public string? StateCode { get; set; }
        public bool IsActive { get; set; }
    }
}
```

### 2. Update CustomerContact Models

#### 2.1 Update CustomerContact Entity

**Path:** `Models/CustomerContact/CustomerContact.cs`

**Change:** Update State from `string?` to `int?`

```csharp
namespace SimpleCRM.Models.CustomerContact
{
    /// <summary>
    /// CustomerContact entity model - maps to tblCustomerContact table
    /// Stores customer contact information including phone, email, and address details
    /// </summary>
    public class CustomerContact
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string? MobileNo { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? City { get; set; }
        public int? State { get; set; }  // Changed from string? to int?
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
```

#### 2.2 Update CustomerContactRequest Model

**Path:** `Models/CustomerContact/CustomerContactRequest.cs`

**Change:** Update State from `string?` to `int?`

```csharp
using System.ComponentModel.DataAnnotations;

namespace SimpleCRM.Models.CustomerContact
{
    /// <summary>
    /// CustomerContact request model - for Insert/Update operations
    /// Includes validation attributes
    /// </summary>
    public class CustomerContactRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Customer is required")]
        public int CustomerId { get; set; }

        [MaxLength(100)]
        public string? MobileNo { get; set; }

        [MaxLength(100)]
        public string? Address { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        public int? State { get; set; }  // Changed from string? to int?

        [MaxLength(100)]
        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }
    }
}
```

#### 2.3 Update CustomerContactResponse Model

**Path:** `Models/CustomerContact/CustomerContactResponse.cs`

**Change:** Add State (INT) and StateName (string) properties

```csharp
namespace SimpleCRM.Models.CustomerContact
{
    /// <summary>
    /// CustomerContact response model - for API/View responses
    /// Includes computed properties and joined data
    /// </summary>
    public class CustomerContactResponse
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string? MobileNo { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? City { get; set; }
        public int? State { get; set; }  // Changed from string? to int?
        public string? StateName { get; set; }  // NEW: Display name from tblState
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        // Joined properties
        public string? CustomerName { get; set; }
        public string? CustomerCode { get; set; }
    }
}
```

---

## Repository Changes

### 1. Create State Repository Interface

**Path:** `Repos/Interfaces/IStateRepository.cs`

```csharp
using SimpleCRM.Models;
using SimpleCRM.Models.State;

namespace SimpleCRM.Repos.Interfaces
{
    /// <summary>
    /// State repository interface
    /// Defines operations for State master data
    /// </summary>
    public interface IStateRepository
    {
        /// <summary>
        /// Get all active states
        /// Maps to: ssp_tblState_GetActive
        /// </summary>
        /// <returns>ServiceResponse with list of active states</returns>
        Task<ServiceResponse<IEnumerable<StateResponse>>> GetActiveAsync();
    }
}
```

### 2. Create State Repository Implementation

**Path:** `Repos/Services/StateRepository.cs`

```csharp
using Dapper;
using SimpleCRM.Data;
using SimpleCRM.Models;
using SimpleCRM.Models.State;
using SimpleCRM.Repos.Interfaces;
using System.Data;

namespace SimpleCRM.Repos.Services
{
    /// <summary>
    /// State repository implementation
    /// Uses Dapper to execute stored procedures for tblState
    /// </summary>
    public class StateRepository : IStateRepository
    {
        private readonly DapperContext _context;

        public StateRepository(DapperContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all active states using ssp_tblState_GetActive
        /// </summary>
        public async Task<ServiceResponse<IEnumerable<StateResponse>>> GetActiveAsync()
        {
            try
            {
                using var connection = _context.CreateConnection();

                var states = await connection.QueryAsync<StateResponse>(
                    "ssp_tblState_GetActive",
                    commandType: CommandType.StoredProcedure
                );

                return ServiceResponse<IEnumerable<StateResponse>>.Success(
                    states,
                    $"Retrieved {states.Count()} active states"
                );
            }
            catch (Exception ex)
            {
                return ServiceResponse<IEnumerable<StateResponse>>.Failure(
                    $"Error retrieving active states: {ex.Message}"
                );
            }
        }
    }
}
```

### 3. Register StateRepository in Program.cs

**Path:** `Program.cs`

**Add after CustomerContactRepository registration:**

```csharp
// Register State Repository
builder.Services.AddScoped<IStateRepository, StateRepository>();
```

---

## Controller Changes

### Update CustomerContactController

**Path:** `Controllers/CustomerContactController.cs`

**Changes:**
1. Inject IStateRepository
2. Add GetStatesList API endpoint

```csharp
using Microsoft.AspNetCore.Mvc;
using SimpleCRM.Models;
using SimpleCRM.Models.CustomerContact;
using SimpleCRM.Repos.Interfaces;

namespace SimpleCRM.Controllers
{
    /// <summary>
    /// CustomerContact controller
    /// Handles customer contact CRUD operations with 2 MVC actions and API endpoints
    /// </summary>
    public class CustomerContactController : Controller
    {
        private readonly ICustomerContactRepository _customerContactRepository;
        private readonly IStateRepository _stateRepository;  // NEW: State repository

        public CustomerContactController(
            ICustomerContactRepository customerContactRepository,
            IStateRepository stateRepository)  // NEW: Inject State repository
        {
            _customerContactRepository = customerContactRepository;
            _stateRepository = stateRepository;
        }

        // ... existing MVC actions ...

        // ... existing API endpoints ...

        /// <summary>
        /// GET API: /CustomerContact/GetStatesList
        /// Get all active states as JSON for dropdown
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetStatesList()
        {
            var result = await _stateRepository.GetActiveAsync();

            return Json(new ServiceResponse
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                Data = result.Data
            });
        }
    }
}
```

---

## View Changes

### Update CustomerContact Index View

**Path:** `Views/CustomerContact/Index.cshtml`

**Changes:**
1. Convert State text input to dropdown (select element)
2. Update table header to show "State" (will show StateName from API)

```cshtml
<!-- In the modal form, replace the State text input with dropdown -->

<!-- OLD CODE (lines 103-107): -->
<!--
<div class="col-md-6 col-12 mb-3">
    <label for="state" class="form-label">State</label>
    <input type="text" class="form-control" id="state" maxlength="100" />
    <div class="error-message text-danger small"></div>
</div>
-->

<!-- NEW CODE: -->
<div class="col-md-6 col-12 mb-3">
    <label for="state" class="form-label">State</label>
    <select class="form-select" id="state">
        <option value="">Select State</option>
        <!-- Populated by JavaScript -->
    </select>
    <div class="error-message text-danger small"></div>
</div>
```

**Complete updated modal section (lines 97-108):**

```cshtml
<div class="row">
    <div class="col-md-6 col-12 mb-3">
        <label for="city" class="form-label">City</label>
        <input type="text" class="form-control" id="city" maxlength="100" />
        <div class="error-message text-danger small"></div>
    </div>
    <div class="col-md-6 col-12 mb-3">
        <label for="state" class="form-label">State</label>
        <select class="form-select" id="state">
            <option value="">Select State</option>
            <!-- Populated by JavaScript -->
        </select>
        <div class="error-message text-danger small"></div>
    </div>
</div>
```

---

## JavaScript Changes

### Update customerContact.js

**Path:** `wwwroot/js/customerContact.js`

**Changes:**
1. Add `states` array to store state list
2. Add `loadStates()` function to fetch states from API
3. Add `populateStateDropdown()` function to populate the dropdown
4. Update `renderCustomerContactTable()` to display `StateName` instead of `State`
5. Update `openEditModal()` to set state dropdown value (auto-select by ID)
6. Update `saveCustomerContact()` to send State as INT
7. Update event bindings to include state dropdown

```javascript
// CustomerContact Module - CRUD Operations with Validation
(function () {
    'use strict';

    let customerContacts = [];
    let customers = [];
    let states = [];  // NEW: State list
    let customerContactModal, deleteModal;

    // Initialize on page load
    $(document).ready(function () {
        initializeModals();
        loadCustomerContactsFromHiddenModel();
        loadCustomers();
        loadStates();  // NEW: Load states
        renderCustomerContactTable();
        bindEvents();
    });

    // Initialize Bootstrap modals
    function initializeModals() {
        customerContactModal = new bootstrap.Modal(document.getElementById('customerContactModal'));
        deleteModal = new bootstrap.Modal(document.getElementById('deleteModal'));
    }

    // Load customer contacts from hidden model
    function loadCustomerContactsFromHiddenModel() {
        try {
            const hiddenData = $('#HiddenModel').val();
            if (hiddenData && hiddenData !== 'null') {
                customerContacts = JSON.parse(hiddenData);
            }
        } catch (error) {
            console.error('Error parsing hidden model:', error);
            showToaster('Error loading customer contact data', 'error');
        }
    }

    // Load customers for dropdown
    function loadCustomers() {
        $.ajax({
            url: '/Customer/GetActiveCustomers',
            type: 'GET',
            success: function (response) {
                if (response.isSuccess) {
                    customers = response.data;
                    populateCustomerDropdown();
                }
            },
            error: function () {
                showToaster('Error loading customers', 'error');
            }
        });
    }

    // NEW: Load states for dropdown
    function loadStates() {
        $.ajax({
            url: '/CustomerContact/GetStatesList',
            type: 'GET',
            success: function (response) {
                if (response.isSuccess) {
                    states = response.data;
                    populateStateDropdown();
                }
            },
            error: function () {
                showToaster('Error loading states', 'error');
            }
        });
    }

    // Populate customer dropdown
    function populateCustomerDropdown() {
        const dropdown = $('#customerId');
        dropdown.empty();
        dropdown.append('<option value="">Select Customer</option>');

        customers.forEach(customer => {
            dropdown.append(`<option value="${customer.id}">${escapeHtml(customer.companyName)}</option>`);
        });
    }

    // NEW: Populate state dropdown
    function populateStateDropdown() {
        const dropdown = $('#state');
        dropdown.empty();
        dropdown.append('<option value="">Select State</option>');

        states.forEach(state => {
            dropdown.append(`<option value="${state.id}">${escapeHtml(state.stateName)}</option>`);
        });
    }

    // Render customer contact table
    function renderCustomerContactTable() {
        const tbody = $('#customerContactTableBody');
        tbody.empty();

        if (!customerContacts || customerContacts.length === 0) {
            tbody.append('<tr><td colspan="8" class="text-center">No customer contacts found</td></tr>');
            return;
        }

        customerContacts.forEach(contact => {
            const statusBadge = contact.isActive
                ? '<span class="badge bg-success">Active</span>'
                : '<span class="badge bg-secondary">Inactive</span>';

            const row = `
                <tr>
                    <td>${contact.id}</td>
                    <td>${escapeHtml(contact.customerName || '-')}</td>
                    <td>${escapeHtml(contact.mobileNo || '-')}</td>
                    <td>${escapeHtml(contact.email || '-')}</td>
                    <td>${escapeHtml(contact.city || '-')}</td>
                    <td>${escapeHtml(contact.stateName || '-')}</td>
                    <td>${statusBadge}</td>
                    <td>
                        <button class="btn btn-sm btn-info btn-view" data-id="${contact.id}" title="View">
                            <i class="bi bi-eye"></i>
                        </button>
                        <button class="btn btn-sm btn-warning btn-edit" data-id="${contact.id}" title="Edit">
                            <i class="bi bi-pencil"></i>
                        </button>
                        <button class="btn btn-sm btn-danger btn-delete" data-id="${contact.id}" title="Delete">
                            <i class="bi bi-trash"></i>
                        </button>
                    </td>
                </tr>
            `;
            tbody.append(row);
        });
    }

    // Bind event handlers
    function bindEvents() {
        // Add customer contact button
        $('#btnAddCustomerContact').on('click', openAddModal);

        // Save customer contact button
        $('#btnSaveCustomerContact').on('click', saveCustomerContact);

        // Confirm delete button
        $('#btnConfirmDelete').on('click', confirmDelete);

        // Table action buttons (using event delegation)
        $('#customerContactTableBody').on('click', '.btn-view', function () {
            const id = $(this).data('id');
            window.location.href = `/CustomerContact/Details/${id}`;
        });

        $('#customerContactTableBody').on('click', '.btn-edit', function () {
            const id = $(this).data('id');
            openEditModal(id);
        });

        $('#customerContactTableBody').on('click', '.btn-delete', function () {
            const id = $(this).data('id');
            openDeleteModal(id);
        });

        // Phone/Mobile input - allow only numeric
        $('#mobileNo, #phoneNumber').on('keyup', function () {
            let value = $(this).val();
            value = value.replace(/\D/g, ''); // Remove non-numeric characters
            $(this).val(value);
        });

        // Real-time validation on input change
        // UPDATED: Added #state to the selector (now dropdown, not text input)
        $('#customerId, #mobileNo, #email, #phoneNumber, #address, #city, #state').on('input change', function () {
            clearFieldError($(this));
        });

        // Modal close - reset form
        $('#customerContactModal').on('hidden.bs.modal', resetForm);
    }

    // Open Add Modal
    function openAddModal() {
        resetForm();
        $('#contactId').val('0');
        $('#customerContactModalLabel').text('Add Customer Contact');
        $('#isActive').prop('checked', true);
        customerContactModal.show();
    }

    // Open Edit Modal
    function openEditModal(id) {
        const contact = customerContacts.find(c => c.id === id);
        if (!contact) {
            showToaster('Customer contact not found', 'error');
            return;
        }

        resetForm();
        $('#contactId').val(contact.id);
        $('#customerId').val(contact.customerId);
        $('#mobileNo').val(contact.mobileNo || '');
        $('#email').val(contact.email || '');
        $('#phoneNumber').val(contact.phoneNumber || '');
        $('#address').val(contact.address || '');
        $('#city').val(contact.city || '');
        $('#state').val(contact.state || '');  // UPDATED: Sets dropdown value by ID (auto-select)
        $('#isActive').prop('checked', contact.isActive);
        $('#customerContactModalLabel').text('Edit Customer Contact');
        customerContactModal.show();
    }

    // Open Delete Modal
    function openDeleteModal(id) {
        const contact = customerContacts.find(c => c.id === id);
        if (contact) {
            const info = `${contact.customerName || 'Unknown'} - ${contact.mobileNo || contact.email || 'No contact info'}`;
            $('#deleteContactInfo').text(info);
        }
        $('#deleteContactId').val(id);
        deleteModal.show();
    }

    // Save Customer Contact (Add/Update)
    function saveCustomerContact() {
        if (!validateForm()) {
            return;
        }

        // UPDATED: Parse state as INT (or null if not selected)
        const stateValue = $('#state').val();
        const contactData = {
            id: parseInt($('#contactId').val()),
            customerId: parseInt($('#customerId').val()),
            mobileNo: $('#mobileNo').val().trim() || null,
            email: $('#email').val().trim() || null,
            phoneNumber: $('#phoneNumber').val().trim() || null,
            address: $('#address').val().trim() || null,
            city: $('#city').val().trim() || null,
            state: stateValue ? parseInt(stateValue) : null,  // UPDATED: INT or null
            isActive: $('#isActive').is(':checked')
        };

        $.ajax({
            url: '/CustomerContact/InsertUpdate',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(contactData),
            beforeSend: function () {
                $('#btnSaveCustomerContact').prop('disabled', true).text('Saving...');
            },
            success: function (response) {
                if (response.isSuccess) {
                    showToaster(response.message, 'success');
                    customerContactModal.hide();
                    reloadCustomerContacts();
                } else {
                    showToaster(response.message, 'error');
                }
            },
            error: function () {
                showToaster('An error occurred while saving customer contact', 'error');
            },
            complete: function () {
                $('#btnSaveCustomerContact').prop('disabled', false).text('Save Contact');
            }
        });
    }

    // Confirm Delete
    function confirmDelete() {
        const id = parseInt($('#deleteContactId').val());

        $.ajax({
            url: '/CustomerContact/SoftDeleteCustomerContact',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ id: id }),
            beforeSend: function () {
                $('#btnConfirmDelete').prop('disabled', true).text('Deleting...');
            },
            success: function (response) {
                if (response.isSuccess) {
                    showToaster(response.message, 'success');
                    deleteModal.hide();
                    reloadCustomerContacts();
                } else {
                    showToaster(response.message, 'error');
                }
            },
            error: function () {
                showToaster('An error occurred while deleting customer contact', 'error');
            },
            complete: function () {
                $('#btnConfirmDelete').prop('disabled', false).text('Delete');
            }
        });
    }

    // Reload customer contacts from API
    function reloadCustomerContacts() {
        $.ajax({
            url: '/CustomerContact/GetActiveCustomerContacts',
            type: 'GET',
            success: function (response) {
                if (response.isSuccess) {
                    customerContacts = response.data;
                    renderCustomerContactTable();
                } else {
                    showToaster('Error loading customer contacts', 'error');
                }
            },
            error: function () {
                showToaster('An error occurred while loading customer contacts', 'error');
            }
        });
    }

    // Form Validation
    function validateForm() {
        let isValid = true;

        // Customer ID validation (required)
        const customerId = $('#customerId').val();
        if (customerId === '' || customerId === '0') {
            setFieldError($('#customerId'), 'Please select a customer');
            isValid = false;
        }

        // Email validation (optional, but must be valid if provided)
        const email = $('#email').val().trim();
        if (email !== '' && !isValidEmail(email)) {
            setFieldError($('#email'), 'Please enter a valid email address');
            isValid = false;
        }

        // Mobile number validation (optional, but must be numeric if provided)
        const mobileNo = $('#mobileNo').val().trim();
        if (mobileNo !== '' && !/^\d+$/.test(mobileNo)) {
            setFieldError($('#mobileNo'), 'Mobile number must be numeric');
            isValid = false;
        }

        // Phone number validation (optional, but must be numeric if provided)
        const phoneNumber = $('#phoneNumber').val().trim();
        if (phoneNumber !== '' && !/^\d+$/.test(phoneNumber)) {
            setFieldError($('#phoneNumber'), 'Phone number must be numeric');
            isValid = false;
        }

        return isValid;
    }

    // Email validation regex
    function isValidEmail(email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    }

    // Set field error
    function setFieldError($field, message) {
        $field.addClass('border-danger');
        $field.next('.error-message').text(message);
    }

    // Clear field error
    function clearFieldError($field) {
        $field.removeClass('border-danger');
        $field.next('.error-message').text('');
    }

    // Reset form
    function resetForm() {
        $('#customerContactForm')[0].reset();
        $('#contactId').val('0');
        $('#customerContactForm input, #customerContactForm select, #customerContactForm textarea').removeClass('border-danger');
        $('#customerContactForm .error-message').text('');
    }

    // Toaster notification
    function showToaster(message, type) {
        const toastClass = type === 'success' ? 'bg-success' : 'bg-danger';
        const toastHtml = `
            <div class="toast align-items-center text-white ${toastClass} border-0" role="alert" aria-live="assertive" aria-atomic="true">
                <div class="d-flex">
                    <div class="toast-body">
                        ${escapeHtml(message)}
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
            </div>
        `;

        // Create toast container if it doesn't exist
        if ($('#toastContainer').length === 0) {
            $('body').append('<div id="toastContainer" class="toast-container position-fixed top-0 end-0 p-3"></div>');
        }

        const $toast = $(toastHtml);
        $('#toastContainer').append($toast);

        const toast = new bootstrap.Toast($toast[0], { delay: 3000 });
        toast.show();

        $toast.on('hidden.bs.toast', function () {
            $(this).remove();
        });
    }

    // HTML escape function
    function escapeHtml(text) {
        if (!text) return '';
        const map = {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#039;'
        };
        return text.toString().replace(/[&<>"']/g, m => map[m]);
    }

})();
```

---

## Summary of Changes

### Files to Create:
1. `SqlQueries/Tables/tblState.sql` - State master table
2. `SqlQueries/StoredProcedures/ssp_tblState_GetActive.sql` - Get active states SP
3. `Models/State/State.cs` - State entity model
4. `Models/State/StateResponse.cs` - State response model
5. `Repos/Interfaces/IStateRepository.cs` - State repository interface
6. `Repos/Services/StateRepository.cs` - State repository implementation

### Files to Update:
1. `SqlQueries/Tables/tblCustomerContact.sql` - Alter State column from VARCHAR to INT
2. `SqlQueries/StoredProcedures/ssp_tblCustomerContact_GetAll.sql` - Add State JOIN
3. `SqlQueries/StoredProcedures/ssp_tblCustomerContact_GetById.sql` - Add State JOIN
4. `SqlQueries/StoredProcedures/ssp_tblCustomerContact_InsertUpdate.sql` - Update State param to INT
5. `Models/CustomerContact/CustomerContact.cs` - Change State to int?
6. `Models/CustomerContact/CustomerContactRequest.cs` - Change State to int?
7. `Models/CustomerContact/CustomerContactResponse.cs` - Add State (int?) and StateName (string?)
8. `Controllers/CustomerContactController.cs` - Add IStateRepository injection and GetStatesList endpoint
9. `Views/CustomerContact/Index.cshtml` - Convert State input to dropdown
10. `wwwroot/js/customerContact.js` - Add State loading, dropdown population, and auto-select logic
11. `Program.cs` - Register IStateRepository

### Key Behavior:
- State dropdown loads from `/CustomerContact/GetStatesList` API
- When editing a contact, State dropdown auto-selects based on `contact.state` ID
- State is saved as INT (foreign key to tblState)
- State is displayed as `StateName` in table and views

---

## Testing Checklist

- [ ] Run `tblState.sql` to create State master table and insert sample data
- [ ] Run `tblCustomerContact.sql` ALTER script to change State to INT
- [ ] Run all updated stored procedures
- [ ] Verify State dropdown loads on page load
- [ ] Verify State dropdown auto-selects when editing existing contact
- [ ] Verify State is saved as INT in database
- [ ] Verify StateName is displayed in table (not ID)
- [ ] Test validation (State is optional, no required validation)
- [ ] Test add new contact with State selected
- [ ] Test add new contact without State selected (should allow null)
- [ ] Test edit contact and change State
- [ ] Test that table displays correct StateName after save

---

## Implementation Order

1. **Database Layer** (Run SQL scripts in order):
   - Create `tblState.sql` (table + sample data)
   - Alter `tblCustomerContact.sql` (change State to INT)
   - Create/Update stored procedures:
     - `ssp_tblState_GetActive.sql`
     - `ssp_tblCustomerContact_GetAll.sql`
     - `ssp_tblCustomerContact_GetById.sql`
     - `ssp_tblCustomerContact_InsertUpdate.sql`

2. **Model Layer**:
   - Create State models (`State.cs`, `StateResponse.cs`)
   - Update CustomerContact models (all 3 files)

3. **Repository Layer**:
   - Create `IStateRepository.cs` and `StateRepository.cs`
   - Register in `Program.cs`

4. **Controller Layer**:
   - Update `CustomerContactController.cs` (inject IStateRepository, add GetStatesList)

5. **View Layer**:
   - Update `Index.cshtml` (convert State input to dropdown)

6. **Frontend Layer**:
   - Update `customerContact.js` (load states, populate dropdown, handle selection)

7. **Testing**:
   - Run application and verify all changes work as expected
