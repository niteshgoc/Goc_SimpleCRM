// CustomerContact Module - CRUD Operations with Validation
(function () {
    'use strict';

    let customerContacts = [];
    let customers = [];
    let customerContactModal, deleteModal;

    // Initialize on page load
    $(document).ready(function () {
        initializeModals();
        loadCustomerContactsFromHiddenModel();
        loadCustomers();
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

    // Populate customer dropdown
    function populateCustomerDropdown() {
        const dropdown = $('#customerId');
        dropdown.empty();
        dropdown.append('<option value="">Select Customer</option>');

        customers.forEach(customer => {
            dropdown.append(`<option value="${customer.id}">${escapeHtml(customer.companyName)}</option>`);
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
                    <td>${escapeHtml(contact.state || '-')}</td>
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
        $('#state').val(contact.state || '');
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

        const contactData = {
            id: parseInt($('#contactId').val()),
            customerId: parseInt($('#customerId').val()),
            mobileNo: $('#mobileNo').val().trim() || null,
            email: $('#email').val().trim() || null,
            phoneNumber: $('#phoneNumber').val().trim() || null,
            address: $('#address').val().trim() || null,
            city: $('#city').val().trim() || null,
            state: $('#state').val().trim() || null,
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
