using Microsoft.AspNetCore.Mvc;
using SimpleCRM.Models;
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

        // ==================== MVC VIEW ACTIONS ====================

        /// <summary>
        /// GET: Customer/Index
        /// Display customer list page (data loaded via JavaScript)
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var result = await _customerRepository.GetActiveAsync();

            if (!result.IsSuccess)
            {
                return View(new List<CustomerResponse>());
            }

            return View(result.Data);
        }

        /// <summary>
        /// GET: Customer/Details/5
        /// Display customer details page
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            var result = await _customerRepository.GetByIdAsync(id);

            if (!result.IsSuccess || result.Data == null)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        // ==================== API ENDPOINTS ====================

        /// <summary>
        /// POST API: /Customer/InsertUpdate
        /// \
        /// 
        /// Insert or update customer via JSON
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> InsertUpdate([FromBody] CustomerRequest request)
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

            var result = await _customerRepository.InsertUpdateAsync(request);

            return Json(new ServiceResponse
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                Data = result.Data
            });
        }

        /// <summary>
        /// GET API: /Customer/GetActiveCustomers
        /// Get all active customers as JSON
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetActiveCustomers()
        {
            var result = await _customerRepository.GetActiveAsync();

            return Json(new ServiceResponse
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                Data = result.Data
            });
        }

        /// <summary>
        /// GET API: /Customer/GetAllCustomers
        /// Get all customers (active and inactive) as JSON
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var result = await _customerRepository.GetAllAsync();

            return Json(new ServiceResponse
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                Data = result.Data
            });
        }

        /// <summary>
        /// GET API: /Customer/GetCustomerById/5
        /// Get customer by Id as JSON
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var result = await _customerRepository.GetByIdAsync(id);

            return Json(new ServiceResponse
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                Data = result.Data
            });
        }

        /// <summary>
        /// POST API: /Customer/DeleteCustomer
        /// Delete customer via JSON (physical delete)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DeleteCustomer([FromBody] DeleteRequest request)
        {
            if (request.Id <= 0)
            {
                return Json(new ServiceResponse
                {
                    IsSuccess = false,
                    Message = "Invalid customer Id"
                });
            }

            var result = await _customerRepository.DeleteAsync(request.Id);

            return Json(new ServiceResponse
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                Data = result.Data
            });
        }

        /// <summary>
        /// POST API: /Customer/SoftDeleteCustomer
        /// Soft delete customer via JSON (set IsActive = 0)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SoftDeleteCustomer([FromBody] DeleteRequest request)
        {
            if (request.Id <= 0)
            {
                return Json(new ServiceResponse
                {
                    IsSuccess = false,
                    Message = "Invalid customer Id"
                });
            }

            var result = await _customerRepository.SoftDeleteAsync(request.Id);

            return Json(new ServiceResponse
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                Data = result.Data
            });
        }
    }

    /// <summary>
    /// Request model for delete operations
    /// </summary>
    public class DeleteRequest
    {
        public int Id { get; set; }
    }
}
