using SimpleCRM.Models;
using SimpleCRM.Models.CustomerContact;

namespace SimpleCRM.Repos.Interfaces
{
    /// <summary>
    /// CustomerContact repository interface
    /// Defines all CRUD operations for CustomerContact entity
    /// Maps to stored procedures for tblCustomerContact
    /// </summary>
    public interface ICustomerContactRepository
    {
        /// <summary>
        /// Insert or Update customer contact
        /// Maps to: ssp_tblCustomerContact_InsertUpdate
        /// Logic: If Id = 0 then INSERT, else UPDATE
        /// </summary>
        /// <param name="request">Customer contact data</param>
        /// <returns>ServiceResponse with new/updated customer contact Id</returns>
        Task<ServiceResponse<int>> InsertUpdateAsync(CustomerContactRequest request);

        /// <summary>
        /// Get all active customer contacts
        /// Maps to: ssp_tblCustomerContact_GetAll (with IsActive = 1)
        /// </summary>
        /// <returns>ServiceResponse with list of active customer contacts</returns>
        Task<ServiceResponse<IEnumerable<CustomerContactResponse>>> GetActiveAsync();

        /// <summary>
        /// Get customer contact by Id
        /// Maps to: ssp_tblCustomerContact_GetById
        /// </summary>
        /// <param name="id">Customer contact Id</param>
        /// <returns>ServiceResponse with customer contact data</returns>
        Task<ServiceResponse<CustomerContactResponse>> GetByIdAsync(int id);

        /// <summary>
        /// Get all customer contacts (active and inactive)
        /// Maps to: ssp_tblCustomerContact_GetAll
        /// </summary>
        /// <returns>ServiceResponse with list of all customer contacts</returns>
        Task<ServiceResponse<IEnumerable<CustomerContactResponse>>> GetAllAsync();

        /// <summary>
        /// Get all customer contacts for a specific customer
        /// Maps to: ssp_tblCustomerContact_GetAll (with CustomerId filter)
        /// </summary>
        /// <param name="customerId">Customer Id</param>
        /// <returns>ServiceResponse with list of customer contacts</returns>
        Task<ServiceResponse<IEnumerable<CustomerContactResponse>>> GetByCustomerIdAsync(int customerId);

        /// <summary>
        /// Delete customer contact permanently
        /// Maps to: ssp_tblCustomerContact_Delete
        /// </summary>
        /// <param name="id">Customer contact Id</param>
        /// <returns>ServiceResponse with rows affected</returns>
        Task<ServiceResponse<int>> DeleteAsync(int id);

        /// <summary>
        /// Soft delete customer contact (sets IsActive = 0)
        /// Maps to: ssp_tblCustomerContact_SoftDelete
        /// </summary>
        /// <param name="id">Customer contact Id</param>
        /// <returns>ServiceResponse with rows affected</returns>
        Task<ServiceResponse<int>> SoftDeleteAsync(int id);
    }
}
