// Customer Module - CRUD Operations with Validation
(function () {
    'use strict';

    let customers = [];
    let customerModal, deleteModal;

    // Initialize on page load
    $(document).ready(function () {
        initializeModals();
        loadCustomersFromHiddenModel();
        renderCustomerTable();
        bindEvents();
    });

    // Initialize Bootstrap modals
    function initializeModals() {
        customerModal = new bootstrap.Modal(document.getElementById('customerModal'));
        deleteModal = new bootstrap.Modal(document.getElementById('deleteModal'));
    }

    // Load customers from hidden model
    function loadCustomersFromHiddenModel() {
        try {
            const hiddenData = $('#HiddenModel').val();
            if (hiddenData && hiddenData !== 'null') {
                customers = JSON.parse(hiddenData);
            }
        } catch (error) {
            console.error('Error parsing hidden model:', error);
            showToaster('Error loading customer data', 'error');
        }
    }

    // Render customer table
    function renderCustomerTable() {
        const tbody = $('#customerTableBody');
        tbody.empty();

        if (!customers || customers.length === 0) {
            tbody.append('<tr><td colspan="7" class="text-center">No customers found</td></tr>');
            return;
        }

        customers.forEach(customer => {
            const statusBadge = customer.isActive
                ? '<span class="badge bg-success">Active</span>'
                : '<span class="badge bg-secondary">Inactive</span>';

            const row = `
                <tr>
                    <td>${customer.id}</td>
                    <td>${escapeHtml(customer.companyName)}</td>
                    <td>${escapeHtml(customer.email)}</td>
                    <td>${escapeHtml(customer.phone || '-')}</td>
                    <td>${escapeHtml(customer.city || '-')}</td>
                    <td>${statusBadge}</td>
                    <td>
                        <button class="btn btn-sm btn-info btn-view" data-id="${customer.id}" title="View">
                            <i class="bi bi-eye"></i>
                        </button>
                        <button class="btn btn-sm btn-warning btn-edit" data-id="${customer.id}" title="Edit">
                            <i class="bi bi-pencil"></i>
                        </button>
                        <button class="btn btn-sm btn-danger btn-delete" data-id="${customer.id}" data-name="${escapeHtml(customer.companyName)}" title="Delete">
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
        // Add customer button
        $('#btnAddCustomer').on('click', openAddModal);

        // Save customer button
        $('#btnSaveCustomer').on('click', saveCustomer);

        // Confirm delete button
        $('#btnConfirmDelete').on('click', confirmDelete);

        // Table action buttons (using event delegation)
        $('#customerTableBody').on('click', '.btn-view', function () {
            const id = $(this).data('id');
            window.location.href = `/Customer/Details/${id}`;
        });

        $('#customerTableBody').on('click', '.btn-edit', function () {
            const id = $(this).data('id');
            openEditModal(id);
        });

        $('#customerTableBody').on('click', '.btn-delete', function () {
            const id = $(this).data('id');
            const name = $(this).data('name');
            openDeleteModal(id, name);
        });

        // Phone input - allow only numeric
        $('#phone').on('keyup', function () {
            let value = $(this).val();
            value = value.replace(/\D/g, ''); // Remove non-numeric characters
            $(this).val(value);
        });

        // Real-time validation on input change
        $('#companyName, #email, #phone, #city, #address').on('input', function () {
            clearFieldError($(this));
        });

        // Modal close - reset form
        $('#customerModal').on('hidden.bs.modal', resetForm);
    }

    // Open Add Modal
    function openAddModal() {
        resetForm();
        $('#customerId').val('0');
        $('#customerModalLabel').text('Add Customer');
        $('#isActive').prop('checked', true);
        customerModal.show();
    }

    // Open Edit Modal
    function openEditModal(id) {
        const customer = customers.find(c => c.id === id);
        if (!customer) {
            showToaster('Customer not found', 'error');
            return;
        }

        resetForm();
        $('#customerId').val(customer.id);
        $('#companyName').val(customer.companyName);
        $('#email').val(customer.email);
        $('#phone').val(customer.phone || '');
        $('#city').val(customer.city || '');
        $('#address').val(customer.address || '');
        $('#isActive').prop('checked', customer.isActive);
        $('#customerModalLabel').text('Edit Customer');
        customerModal.show();
    }

    // Open Delete Modal
    function openDeleteModal(id, name) {
        $('#deleteCustomerId').val(id);
        $('#deleteCustomerName').text(name);
        deleteModal.show();
    }

    // Save Customer (Add/Update)
    function saveCustomer() {
        if (!validateForm()) {
            return;
        }

        const customerData = {
            id: parseInt($('#customerId').val()),
            companyName: $('#companyName').val().trim(),
            email: $('#email').val().trim(),
            phone: $('#phone').val().trim(),
            city: $('#city').val().trim() || null,
            address: $('#address').val().trim() || null,
            isActive: $('#isActive').is(':checked')
        };

        $.ajax({
            url: '/Customer/InsertUpdate',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(customerData),
            beforeSend: function () {
                $('#btnSaveCustomer').prop('disabled', true).text('Saving...');
            },
            success: function (response) {
                if (response.isSuccess) {
                    showToaster(response.message, 'success');
                    customerModal.hide();
                    reloadCustomers();
                } else {
                    showToaster(response.message, 'error');
                }
            },
            error: function () {
                showToaster('An error occurred while saving customer', 'error');
            },
            complete: function () {
                $('#btnSaveCustomer').prop('disabled', false).text('Save Customer');
            }
        });
    }

    // Confirm Delete
    function confirmDelete() {
        const id = parseInt($('#deleteCustomerId').val());

        $.ajax({
            url: '/Customer/SoftDeleteCustomer',
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
                    reloadCustomers();
                } else {
                    showToaster(response.message, 'error');
                }
            },
            error: function () {
                showToaster('An error occurred while deleting customer', 'error');
            },
            complete: function () {
                $('#btnConfirmDelete').prop('disabled', false).text('Delete');
            }
        });
    }

    // Reload customers from API
    function reloadCustomers() {
        $.ajax({
            url: '/Customer/GetActiveCustomers',
            type: 'GET',
            success: function (response) {
                if (response.isSuccess) {
                    customers = response.data;
                    renderCustomerTable();
                } else {
                    showToaster('Error loading customers', 'error');
                }
            },
            error: function () {
                showToaster('An error occurred while loading customers', 'error');
            }
        });
    }

    // Form Validation
    function validateForm() {
        let isValid = true;

        // Company Name validation
        const companyName = $('#companyName').val().trim();
        if (companyName === '') {
            setFieldError($('#companyName'), 'Company name is required');
            isValid = false;
        } else if (companyName.length > 150) {
            setFieldError($('#companyName'), 'Company name cannot exceed 150 characters');
            isValid = false;
        }

        // Email validation
        const email = $('#email').val().trim();
        if (email === '') {
            setFieldError($('#email'), 'Email is required');
            isValid = false;
        } else if (!isValidEmail(email)) {
            setFieldError($('#email'), 'Please enter a valid email address');
            isValid = false;
        }

        // Phone validation (required, must be numeric and max 10 chars)
        const phone = $('#phone').val().trim();
        if (phone === '') {
            setFieldError($('#phone'), 'Phone is required');
            isValid = false;
        } else if (!/^\d{1,10}$/.test(phone)) {
            setFieldError($('#phone'), 'Phone must be numeric and max 10 digits');
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
        $('#customerForm')[0].reset();
        $('#customerId').val('0');
        $('#customerForm input, #customerForm textarea').removeClass('border-danger');
        $('#customerForm .error-message').text('');
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
