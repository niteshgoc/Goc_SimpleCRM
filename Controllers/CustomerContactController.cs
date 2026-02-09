using Microsoft.AspNetCore.Mvc;
using SimpleCRM.Models;
using SimpleCRM.Models.CustomerContact;
using SimpleCRM.Repos.Interfaces;

namespace SimpleCRM.Controllers
{
    /// <summary>
    /// CustomerContact controller
    /// Handles customer contact CRUD operations with 2 MVC actions and 6 API endpoints
    /// </summary>
    public class CustomerContactController : Controller
    {
        private readonly ICustomerContactRepository _customerContactRepository;

        public CustomerContactController(ICustomerContactRepository customerContactRepository)
        {
            _customerContactRepository = customerContactRepository;
        }

        // ==================== MVC VIEW ACTIONS ====================

        /// <summary>
        /// GET: CustomerContact/Index
        /// Display customer contact list page (data loaded via JavaScript)
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var result = await _customerContactRepository.GetActiveAsync();

            if (!result.IsSuccess)
            {
                return View(new List<CustomerContactResponse>());
            }

            return View(result.Data);
        }

        /// <summary>
        /// GET: CustomerContact/Details/5
        /// Display customer contact details page
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            var result = await _customerContactRepository.GetByIdAsync(id);

            if (!result.IsSuccess || result.Data == null)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        // ==================== API ENDPOINTS ====================

        /// <summary>
        /// POST API: /CustomerContact/InsertUpdate
        /// Insert or update customer contact via JSON
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> InsertUpdate([FromBody] CustomerContactRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Json(new ServiceResponse
                {
                    IsSuccess = false,
                    Message = "Validation failed",
                    Data = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            var result = await _customerContactRepository.InsertUpdateAsync(request);

            return Json(new ServiceResponse
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                Data = result.Data
            });
        }

        /// <summary>
        /// GET API: /CustomerContact/GetActiveCustomerContacts
        /// Get all active customer contacts as JSON
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetActiveCustomerContacts()
        {
            var result = await _customerContactRepository.GetActiveAsync();

            return Json(new ServiceResponse
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                Data = result.Data
            });
        }

        /// <summary>
        /// GET API: /CustomerContact/GetAllCustomerContacts
        /// Get all customer contacts (active and inactive) as JSON
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllCustomerContacts()
        {
            var result = await _customerContactRepository.GetAllAsync();

            return Json(new ServiceResponse
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                Data = result.Data
            });
        }

        /// <summary>
        /// GET API: /CustomerContact/GetCustomerContactById/5
        /// Get customer contact by Id as JSON
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCustomerContactById(int id)
        {
            var result = await _customerContactRepository.GetByIdAsync(id);

            return Json(new ServiceResponse
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                Data = result.Data
            });
        }

        /// <summary>
        /// GET API: /CustomerContact/GetContactsByCustomerId/5
        /// Get all contacts for a specific customer as JSON
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetContactsByCustomerId(int customerId)
        {
            var result = await _customerContactRepository.GetByCustomerIdAsync(customerId);

            return Json(new ServiceResponse
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                Data = result.Data
            });
        }

        /// <summary>
        /// POST API: /CustomerContact/DeleteCustomerContact
        /// Delete customer contact via JSON (physical delete)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DeleteCustomerContact([FromBody] DeleteRequest request)
        {
            if (request.Id <= 0)
            {
                return Json(new ServiceResponse
                {
                    IsSuccess = false,
                    Message = "Invalid customer contact Id"
                });
            }

            var result = await _customerContactRepository.DeleteAsync(request.Id);

            return Json(new ServiceResponse
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                Data = result.Data
            });
        }

        /// <summary>
        /// POST API: /CustomerContact/SoftDeleteCustomerContact
        /// Soft delete customer contact via JSON (set IsActive = 0)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SoftDeleteCustomerContact([FromBody] DeleteRequest request)
        {
            if (request.Id <= 0)
            {
                return Json(new ServiceResponse
                {
                    IsSuccess = false,
                    Message = "Invalid customer contact Id"
                });
            }

            var result = await _customerContactRepository.SoftDeleteAsync(request.Id);

            return Json(new ServiceResponse
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                Data = result.Data
            });
        }
    }
}
