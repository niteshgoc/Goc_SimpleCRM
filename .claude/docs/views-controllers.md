# Controllers and Views Documentation

## Single Page CRUD Pattern

Every entity controller has **2 MVC actions** and **6 API endpoints**.

---

## Controller Structure

### MVC View Actions (2)
1. **`Index()`** — GET — Display entity list page
2. **`Details(int id)`** — GET — Display entity details page

### API Endpoints (6) — For AJAX/JSON
1. **`InsertUpdate([FromBody] [Entity]Request)`** — POST — Insert/Update via JSON
2. **`GetActive[Entities]()`** — GET — Get all active entities as JSON
3. **`GetAll[Entities]()`** — GET — Get all entities as JSON
4. **`Get[Entity]ById(int id)`** — GET — Get single entity as JSON
5. **`Delete[Entity]([FromBody] DeleteRequest)`** — POST — Physical delete via JSON
6. **`SoftDelete[Entity]([FromBody] DeleteRequest)`** — POST — Soft delete via JSON

---

## Controller Example

**File:** `Controllers/CustomerController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using SimpleCRM.Models.Customer;
using SimpleCRM.Repos.Interfaces;

namespace SimpleCRM.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        // MVC Actions
        public async Task<IActionResult> Index()
        {
            var response = await _customerRepository.GetActiveAsync();
            return View(response.Data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _customerRepository.GetByIdAsync(id);
            if (!response.IsSuccess)
                return NotFound();
            return View(response.Data);
        }

        // API Endpoints
        [HttpPost]
        public async Task<IActionResult> InsertUpdate([FromBody] CustomerRequest request)
        {
            var response = await _customerRepository.InsertUpdateAsync(request);
            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveCustomers()
        {
            var response = await _customerRepository.GetActiveAsync();
            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var response = await _customerRepository.GetAllAsync();
            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var response = await _customerRepository.GetByIdAsync(id);
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCustomer([FromBody] DeleteRequest request)
        {
            var response = await _customerRepository.DeleteAsync(request.Id);
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> SoftDeleteCustomer([FromBody] DeleteRequest request)
        {
            var response = await _customerRepository.SoftDeleteAsync(request.Id);
            return Json(response);
        }
    }

    public class DeleteRequest
    {
        public int Id { get; set; }
    }
}
```

---

## View Structure

Create **2 Razor views** in `Views/[Entity]/`:

### 1. Index.cshtml — Single page for list + Add/Edit/Delete

**Features:**
- Hidden input with serialized model data
- Responsive table for entity list
- Add button opens modal
- Edit button per row opens modal with auto-filled data
- Delete button shows confirmation modal
- Bootstrap modals for Add/Edit and Delete
- All operations via AJAX — no page reload

**Pattern:**
```html
@using SimpleCRM.Helpers
@model IEnumerable<SimpleCRM.Models.Customer.CustomerResponse>

@{
    ViewData["Title"] = "Customers";
    var FileVersion = DateTime.Now.Ticks;
}

<!-- Hidden model data for JavaScript -->
<input id="HiddenModel" type="hidden" value="@CommonHelpers.SerializeObject(Model)" />

<div class="container mt-4">
    <div class="row mb-3">
        <div class="col-md-6 col-12">
            <h2>Customers</h2>
        </div>
        <div class="col-md-6 col-12 text-md-end text-start">
            <button class="btn btn-primary" id="btnAddCustomer">
                <i class="bi bi-plus-circle"></i> Add Customer
            </button>
        </div>
    </div>

    <!-- Table -->
    <div class="table-responsive">
        <table class="table table-striped table-hover" id="customerTable">
            <thead>
                <tr>
                    <th>Company Name</th>
                    <th>Email</th>
                    <th>Phone</th>
                    <th>Status</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody id="customerTableBody">
                <!-- Dynamically rendered via JavaScript -->
            </tbody>
        </table>
    </div>
</div>

<!-- Add/Edit Modal -->
<div class="modal fade" id="customerModal" tabindex="-1">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="customerModalLabel">Add Customer</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <form id="customerForm">
                    <input type="hidden" id="customerId" value="0" />

                    <div class="mb-3">
                        <label for="companyName" class="form-label">Company Name *</label>
                        <input type="text" class="form-control" id="companyName" maxlength="150" required />
                        <div class="invalid-feedback"></div>
                    </div>

                    <div class="mb-3">
                        <label for="email" class="form-label">Email</label>
                        <input type="email" class="form-control" id="email" />
                        <div class="invalid-feedback"></div>
                    </div>

                    <div class="mb-3">
                        <label for="phone" class="form-label">Phone *</label>
                        <input type="text" class="form-control" id="phone" maxlength="10" required />
                        <div class="invalid-feedback"></div>
                    </div>

                    <div class="form-check form-switch">
                        <input class="form-check-input" type="checkbox" id="isActive" checked />
                        <label class="form-check-label" for="isActive">Active</label>
                    </div>
                </form>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                <button type="button" class="btn btn-primary" id="btnSaveCustomer">Save</button>
            </div>
        </div>
    </div>
</div>

<!-- Delete Confirmation Modal -->
<div class="modal fade" id="deleteModal" tabindex="-1">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Confirm Delete</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <p>Are you sure you want to delete <strong id="deleteCustomerName"></strong>?</p>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                <button type="button" class="btn btn-danger" id="btnConfirmDelete">Delete</button>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    <script defer src="/js/customer.js?v=@FileVersion" type="text/javascript"></script>
}
```

---

### 2. Details.cshtml — Simple details view

**Features:**
- Display all entity properties
- Back to List button

**Pattern:**
```html
@model SimpleCRM.Models.Customer.CustomerResponse

@{
    ViewData["Title"] = "Customer Details";
}

<div class="container mt-4">
    <h2>Customer Details</h2>

    <div class="card">
        <div class="card-body">
            <dl class="row">
                <dt class="col-sm-3">Company Name</dt>
                <dd class="col-sm-9">@Model.CompanyName</dd>

                <dt class="col-sm-3">Email</dt>
                <dd class="col-sm-9">@Model.Email</dd>

                <dt class="col-sm-3">Phone</dt>
                <dd class="col-sm-9">@Model.Phone</dd>

                <dt class="col-sm-3">Status</dt>
                <dd class="col-sm-9">
                    <span class="badge bg-@(Model.IsActive ? "success" : "secondary")">
                        @Model.Status
                    </span>
                </dd>

                <dt class="col-sm-3">Created At</dt>
                <dd class="col-sm-9">@Model.CreatedAt.ToString("yyyy-MM-dd HH:mm")</dd>
            </dl>
        </div>
    </div>

    <div class="mt-3">
        <a asp-action="Index" class="btn btn-secondary">Back to List</a>
    </div>
</div>
```

---

## Navigation Menu

Add entity link to `Views/Shared/_Layout.cshtml`:

```html
<li class="nav-item">
    <a class="nav-link text-dark" asp-controller="Customer" asp-action="Index">Customers</a>
</li>
```

---

## Bootstrap Responsive Design

- Use `col-md-6 col-12` for responsive columns
- Use `text-md-end text-start` for responsive text alignment
- Use `table-responsive` for mobile-friendly tables
- Use `modal-lg` for large modals
- Test on desktop, tablet, and mobile viewports

---

## Script Reference Pattern

Always include cache-busting version parameter:

```csharp
@{
    var FileVersion = DateTime.Now.Ticks;
}

@section Scripts {
    <script defer src="/js/customer.js?v=@FileVersion" type="text/javascript"></script>
}
```

**Note:** Version as query parameter (`?v=`), **not** in filename.

---

## Dependency Injection

Controllers receive repositories via constructor injection:

```csharp
private readonly ICustomerRepository _customerRepository;

public CustomerController(ICustomerRepository customerRepository)
{
    _customerRepository = customerRepository;
}
```

Repositories must be registered in `Program.cs`:
```csharp
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
```
