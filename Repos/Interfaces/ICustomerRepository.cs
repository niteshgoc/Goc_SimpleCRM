using SimpleCRM.Models;
using SimpleCRM.Models.Customer;

namespace SimpleCRM.Repos.Interfaces
{
    /// <summary>
    /// Customer repository interface
    /// Defines all CRUD operations for Customer entity
    /// Maps to stored procedures in SqlQueries/MasterCustomer.sql
    /// </summary>
    public interface ICustomerRepository
    {
        /// <summary>
        /// Insert or Update customer
        /// Maps to: ssp_Customer_InsertUpdate
        /// Logic: If Id = 0 then INSERT, else UPDATE
        /// </summary>
        /// <param name="request">Customer data</param>
        /// <returns>ServiceResponse with new/updated customer Id</returns>
        Task<ServiceResponse<int>> InsertUpdateAsync(CustomerRequest request);

        /// <summary>
        /// Get all active customers
        /// Maps to: ssp_Customer_GetActive
        /// </summary>
        /// <returns>ServiceResponse with list of active customers</returns>
        Task<ServiceResponse<IEnumerable<CustomerResponse>>> GetActiveAsync();

        /// <summary>
        /// Get customer by Id
        /// Maps to: ssp_Customer_GetById
        /// </summary>
        /// <param name="id">Customer Id</param>
        /// <returns>ServiceResponse with customer data</returns>
        Task<ServiceResponse<CustomerResponse>> GetByIdAsync(int id);

        /// <summary>
        /// Get all customers (active and inactive)
        /// Maps to: ssp_Customer_GetAll
        /// </summary>
        /// <returns>ServiceResponse with list of all customers</returns>
        Task<ServiceResponse<IEnumerable<CustomerResponse>>> GetAllAsync();

        /// <summary>
        /// Delete customer permanently
        /// Maps to: ssp_Customer_Delete
        /// </summary>
        /// <param name="id">Customer Id</param>
        /// <returns>ServiceResponse with rows affected</returns>
        Task<ServiceResponse<int>> DeleteAsync(int id);

        /// <summary>
        /// Soft delete customer (sets IsActive = 0)
        /// Maps to: ssp_Customer_SoftDelete
        /// </summary>
        /// <param name="id">Customer Id</param>
        /// <returns>ServiceResponse with rows affected</returns>
        Task<ServiceResponse<int>> SoftDeleteAsync(int id);
    }
}
