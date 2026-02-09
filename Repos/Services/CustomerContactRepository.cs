using Dapper;
using SimpleCRM.Data;
using SimpleCRM.Models;
using SimpleCRM.Models.CustomerContact;
using SimpleCRM.Repos.Interfaces;
using System.Data;

namespace SimpleCRM.Repos.Services
{
    /// <summary>
    /// CustomerContact repository implementation
    /// Uses Dapper to execute stored procedures for tblCustomerContact
    /// </summary>
    public class CustomerContactRepository : ICustomerContactRepository
    {
        private readonly DapperContext _context;

        public CustomerContactRepository(DapperContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Insert or Update customer contact using ssp_tblCustomerContact_InsertUpdate
        /// </summary>
        public async Task<ServiceResponse<int>> InsertUpdateAsync(CustomerContactRequest request)
        {
            try
            {
                using var connection = _context.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@Id", request.Id);
                parameters.Add("@CustomerId", request.CustomerId);
                parameters.Add("@MobileNo", request.MobileNo);
                parameters.Add("@Address", request.Address);
                parameters.Add("@Email", request.Email);
                parameters.Add("@City", request.City);
                parameters.Add("@State", request.State);
                parameters.Add("@PhoneNumber", request.PhoneNumber);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@CreatedBy", request.CreatedBy);
                parameters.Add("@UpdatedBy", request.UpdatedBy);

                var result = await connection.QuerySingleAsync<CustomerContactResponse>(
                    "ssp_tblCustomerContact_InsertUpdate",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                string message = request.Id == 0
                    ? "Customer contact created successfully"
                    : "Customer contact updated successfully";

                return ServiceResponse<int>.Success(result.Id, message);
            }
            catch (Exception ex)
            {
                return ServiceResponse<int>.Failure($"Error saving customer contact: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all active customer contacts using ssp_tblCustomerContact_GetAll
        /// </summary>
        public async Task<ServiceResponse<IEnumerable<CustomerContactResponse>>> GetActiveAsync()
        {
            try
            {
                using var connection = _context.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@IsActive", 1);

                var contacts = await connection.QueryAsync<CustomerContactResponse>(
                    "ssp_tblCustomerContact_GetAll",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return ServiceResponse<IEnumerable<CustomerContactResponse>>.Success(
                    contacts,
                    $"Retrieved {contacts.Count()} active customer contacts"
                );
            }
            catch (Exception ex)
            {
                return ServiceResponse<IEnumerable<CustomerContactResponse>>.Failure(
                    $"Error retrieving active customer contacts: {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Get customer contact by Id using ssp_tblCustomerContact_GetById
        /// </summary>
        public async Task<ServiceResponse<CustomerContactResponse>> GetByIdAsync(int id)
        {
            try
            {
                using var connection = _context.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                var contact = await connection.QuerySingleOrDefaultAsync<CustomerContactResponse>(
                    "ssp_tblCustomerContact_GetById",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (contact == null)
                {
                    return ServiceResponse<CustomerContactResponse>.Failure("Customer contact not found");
                }

                return ServiceResponse<CustomerContactResponse>.Success(contact, "Customer contact retrieved successfully");
            }
            catch (Exception ex)
            {
                return ServiceResponse<CustomerContactResponse>.Failure($"Error retrieving customer contact: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all customer contacts using ssp_tblCustomerContact_GetAll
        /// </summary>
        public async Task<ServiceResponse<IEnumerable<CustomerContactResponse>>> GetAllAsync()
        {
            try
            {
                using var connection = _context.CreateConnection();

                var contacts = await connection.QueryAsync<CustomerContactResponse>(
                    "ssp_tblCustomerContact_GetAll",
                    commandType: CommandType.StoredProcedure
                );

                return ServiceResponse<IEnumerable<CustomerContactResponse>>.Success(
                    contacts,
                    $"Retrieved {contacts.Count()} customer contacts"
                );
            }
            catch (Exception ex)
            {
                return ServiceResponse<IEnumerable<CustomerContactResponse>>.Failure(
                    $"Error retrieving customer contacts: {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Get all customer contacts for a specific customer using ssp_tblCustomerContact_GetAll
        /// </summary>
        public async Task<ServiceResponse<IEnumerable<CustomerContactResponse>>> GetByCustomerIdAsync(int customerId)
        {
            try
            {
                using var connection = _context.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@CustomerId", customerId);

                var contacts = await connection.QueryAsync<CustomerContactResponse>(
                    "ssp_tblCustomerContact_GetAll",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return ServiceResponse<IEnumerable<CustomerContactResponse>>.Success(
                    contacts,
                    $"Retrieved {contacts.Count()} contacts for customer {customerId}"
                );
            }
            catch (Exception ex)
            {
                return ServiceResponse<IEnumerable<CustomerContactResponse>>.Failure(
                    $"Error retrieving customer contacts: {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Delete customer contact permanently using ssp_tblCustomerContact_Delete
        /// </summary>
        public async Task<ServiceResponse<int>> DeleteAsync(int id)
        {
            try
            {
                using var connection = _context.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                var rowsAffected = await connection.ExecuteAsync(
                    "ssp_tblCustomerContact_Delete",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (rowsAffected == 0)
                {
                    return ServiceResponse<int>.Failure("Customer contact not found");
                }

                return ServiceResponse<int>.Success(rowsAffected, "Customer contact deleted successfully");
            }
            catch (Exception ex)
            {
                return ServiceResponse<int>.Failure($"Error deleting customer contact: {ex.Message}");
            }
        }

        /// <summary>
        /// Soft delete customer contact using ssp_tblCustomerContact_SoftDelete
        /// </summary>
        public async Task<ServiceResponse<int>> SoftDeleteAsync(int id)
        {
            try
            {
                using var connection = _context.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                var rowsAffected = await connection.ExecuteAsync(
                    "ssp_tblCustomerContact_SoftDelete",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (rowsAffected == 0)
                {
                    return ServiceResponse<int>.Failure("Customer contact not found");
                }

                return ServiceResponse<int>.Success(rowsAffected, "Customer contact deactivated successfully");
            }
            catch (Exception ex)
            {
                return ServiceResponse<int>.Failure($"Error deactivating customer contact: {ex.Message}");
            }
        }
    }
}
