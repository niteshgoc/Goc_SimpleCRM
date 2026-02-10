# Frontend Documentation

## JavaScript Structure

Create `wwwroot/js/[entity].js` for each entity (e.g., `customer.js`).

---

## JavaScript Pattern (IIFE)

Use **Immediately Invoked Function Expression** to avoid global scope pollution:

```javascript
(function () {
    let entities = [];

    $(document).ready(function () {
        loadEntitiesFromHiddenModel();
        renderEntityTable();
        bindEvents();
    });

    // Functions here...
})();
```

---

## Initialization Flow

1. **Load data from hidden model:**
   ```javascript
   function loadEntitiesFromHiddenModel() {
       const hiddenData = $('#HiddenModel').val();
       entities = JSON.parse(hiddenData);
   }
   ```

2. **Render table:**
   ```javascript
   function renderEntityTable() {
       const tbody = $('#customerTableBody');
       tbody.empty();

       entities.forEach(customer => {
           const row = `
               <tr>
                   <td>${customer.companyName}</td>
                   <td>${customer.email || '-'}</td>
                   <td>${customer.phone || '-'}</td>
                   <td><span class="badge bg-${customer.isActive ? 'success' : 'secondary'}">${customer.status}</span></td>
                   <td>
                       <button class="btn btn-sm btn-primary btn-edit" data-id="${customer.id}">Edit</button>
                       <button class="btn btn-sm btn-danger btn-delete" data-id="${customer.id}" data-name="${customer.companyName}">Delete</button>
                   </td>
               </tr>
           `;
           tbody.append(row);
       });
   }
   ```

3. **Bind event handlers:**
   ```javascript
   function bindEvents() {
       $('#btnAddCustomer').on('click', openAddModal);
       $(document).on('click', '.btn-edit', function () {
           const id = $(this).data('id');
           openEditModal(id);
       });
       $(document).on('click', '.btn-delete', function () {
           const id = $(this).data('id');
           const name = $(this).data('name');
           openDeleteModal(id, name);
       });
       $('#btnSaveCustomer').on('click', saveCustomer);
       $('#btnConfirmDelete').on('click', confirmDelete);
   }
   ```

---

## Validation Rules

### Email Validation
- Regex: `/^[^\s@]+@[^\s@]+\.[^\s@]+$/`
- Required field
- Show red border + error message if invalid

```javascript
function isValidEmail(email) {
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return regex.test(email);
}
```

### Phone Validation
- Only numeric characters (filter on `keyup`)
- Max 10 digits
- Required field

```javascript
$('#phone').on('keyup', function () {
    // Remove non-numeric characters
    this.value = this.value.replace(/\D/g, '');

    // Limit to 10 characters
    if (this.value.length > 10) {
        this.value = this.value.slice(0, 10);
    }
});
```

### Company Name Validation
- Required
- Max 150 characters

```javascript
function validateForm() {
    clearErrors();
    let isValid = true;

    const companyName = $('#companyName').val().trim();
    if (!companyName) {
        setFieldError($('#companyName'), 'Company name is required');
        isValid = false;
    }

    const email = $('#email').val().trim();
    if (email && !isValidEmail(email)) {
        setFieldError($('#email'), 'Invalid email format');
        isValid = false;
    }

    const phone = $('#phone').val().trim();
    if (!phone) {
        setFieldError($('#phone'), 'Phone is required');
        isValid = false;
    } else if (!/^\d{1,10}$/.test(phone)) {
        setFieldError($('#phone'), 'Phone must be numeric and max 10 digits');
        isValid = false;
    }

    return isValid;
}
```

---

## Error Display

### Set Field Error
```javascript
function setFieldError($field, message) {
    $field.addClass('is-invalid');
    $field.next('.invalid-feedback').text(message);
}
```

### Clear Errors
```javascript
function clearErrors() {
    $('.is-invalid').removeClass('is-invalid');
    $('.invalid-feedback').text('');
}
```

### Clear on Input Change
```javascript
$('input').on('input', function () {
    $(this).removeClass('is-invalid');
    $(this).next('.invalid-feedback').text('');
});
```

---

## AJAX Operations

### Save Customer (Insert/Update)
```javascript
function saveCustomer() {
    if (!validateForm()) return;

    const customerData = {
        id: parseInt($('#customerId').val()),
        companyName: $('#companyName').val().trim(),
        email: $('#email').val().trim() || null,
        phone: $('#phone').val().trim(),
        isActive: $('#isActive').is(':checked')
    };

    $.ajax({
        url: '/Customer/InsertUpdate',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(customerData),
        success: function (response) {
            if (response.isSuccess) {
                showToaster(response.message, 'success');
                $('#customerModal').modal('hide');
                reloadCustomers();
            } else {
                showToaster(response.message, 'error');
            }
        },
        error: function () {
            showToaster('An error occurred', 'error');
        }
    });
}
```

### Reload Customers
```javascript
function reloadCustomers() {
    $.ajax({
        url: '/Customer/GetActiveCustomers',
        type: 'GET',
        success: function (response) {
            if (response.isSuccess) {
                entities = response.data;
                renderEntityTable();
            }
        }
    });
}
```

### Delete Customer
```javascript
function confirmDelete() {
    const id = $('#btnConfirmDelete').data('id');

    $.ajax({
        url: '/Customer/SoftDeleteCustomer',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ id: id }),
        success: function (response) {
            if (response.isSuccess) {
                showToaster(response.message, 'success');
                $('#deleteModal').modal('hide');
                reloadCustomers();
            } else {
                showToaster(response.message, 'error');
            }
        }
    });
}
```

---

## Toaster Notifications

Use Bootstrap toasts for success/error feedback:

```javascript
function showToaster(message, type) {
    const toastContainer = $('#toastContainer');
    const bgClass = type === 'success' ? 'bg-success' : 'bg-danger';

    const toastHtml = `
        <div class="toast align-items-center text-white ${bgClass} border-0" role="alert">
            <div class="d-flex">
                <div class="toast-body">${message}</div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        </div>
    `;

    toastContainer.append(toastHtml);
    const toastElement = toastContainer.find('.toast').last();
    const toast = new bootstrap.Toast(toastElement);
    toast.show();

    // Remove after hidden
    toastElement.on('hidden.bs.toast', function () {
        $(this).remove();
    });
}
```

**Add to Layout:**
```html
<div id="toastContainer" class="position-fixed top-0 end-0 p-3" style="z-index: 9999;"></div>
```

---

## Modal Operations

### Open Add Modal
```javascript
function openAddModal() {
    $('#customerModalLabel').text('Add Customer');
    $('#customerForm')[0].reset();
    $('#customerId').val(0);
    $('#isActive').prop('checked', true);
    clearErrors();
    $('#customerModal').modal('show');
}
```

### Open Edit Modal
```javascript
function openEditModal(id) {
    const customer = entities.find(c => c.id === id);
    if (!customer) return;

    $('#customerModalLabel').text('Edit Customer');
    $('#customerId').val(customer.id);
    $('#companyName').val(customer.companyName);
    $('#email').val(customer.email || '');
    $('#phone').val(customer.phone || '');
    $('#isActive').prop('checked', customer.isActive);
    clearErrors();
    $('#customerModal').modal('show');
}
```

### Open Delete Modal
```javascript
function openDeleteModal(id, name) {
    $('#deleteCustomerName').text(name);
    $('#btnConfirmDelete').data('id', id);
    $('#deleteModal').modal('show');
}
```

---

## IsActive Field (Switch Button)

**HTML:**
```html
<div class="form-check form-switch">
    <input class="form-check-input" type="checkbox" id="isActive" checked />
    <label class="form-check-label" for="isActive">Active</label>
</div>
```

**JavaScript:**
```javascript
// Get value
const isActive = $('#isActive').is(':checked');

// Set value (default true for new entities)
$('#isActive').prop('checked', true);
```

---

## Client Libraries

Located in `wwwroot/lib/`:

| Library | Purpose |
|---------|---------|
| **Bootstrap 5** | CSS framework for responsive design |
| **jQuery** | DOM manipulation and AJAX |
| **jQuery Validation** | Client-side form validation |
| **jQuery Validation Unobtrusive** | Integration with ASP.NET Core validation attributes |

---

## Cache Busting

Always use version parameter on script tags:

```csharp
@{
    var FileVersion = DateTime.Now.Ticks;
}

@section Scripts {
    <script defer src="/js/customer.js?v=@FileVersion" type="text/javascript"></script>
}
```

**Note:** Version as query parameter (`?v=`), **NOT** in filename.

---

## Responsive Design

- Use Bootstrap grid classes: `col-md-6 col-12`
- Use responsive text alignment: `text-md-end text-start`
- Use `table-responsive` for mobile-friendly tables
- Use `modal-lg` for large modals
- Test on desktop, tablet, and mobile viewports
