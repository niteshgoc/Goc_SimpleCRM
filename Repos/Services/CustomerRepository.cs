using Dapper;
using SimpleCRM.Data;
using SimpleCRM.Models;
using SimpleCRM.Models.Customer;
using SimpleCRM.Repos.Interfaces;
using System.Data;

namespace SimpleCRM.Repos.Services
{
    /// <summary>
    /// Customer repository implementation
    /// Uses Dapper to execute stored procedures from SqlQueries/MasterCustomer.sql
    /// </summary>
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DapperContext _context;

        public CustomerRepository(DapperContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Insert or Update customer using ssp_Customer_InsertUpdate
        /// </summary>
        public async Task<ServiceResponse<int>> InsertUpdateAsync(CustomerRequest request)
        {
            try
            {
                using var connection = _context.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@Id", request.Id);
                parameters.Add("@CompanyName", request.CompanyName);
                parameters.Add("@Email", request.Email);
                parameters.Add("@Phone", request.Phone);
                parameters.Add("@Address", request.Address);
                parameters.Add("@City", request.City);
                parameters.Add("@IsActive", request.IsActive);

                var result = await connection.QuerySingleAsync<int>(
                    "ssp_Customer_InsertUpdate",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                string message = request.Id == 0
                    ? "Customer created successfully"
                    : "Customer updated successfully";

                return ServiceResponse<int>.Success(result, message);
            }
            catch (Exception ex)
            {
                return ServiceResponse<int>.Failure($"Error saving customer: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all active customers using ssp_Customer_GetActive
        /// </summary>
        public async Task<ServiceResponse<IEnumerable<CustomerResponse>>> GetActiveAsync()
        {
            try
            {
                using var connection = _context.CreateConnection();

                var customers = await connection.QueryAsync<CustomerResponse>(
                    "ssp_Customer_GetActive",
                    commandType: CommandType.StoredProcedure
                );

                return ServiceResponse<IEnumerable<CustomerResponse>>.Success(
                    customers,
                    $"Retrieved {customers.Count()} active customers"
                );
            }
            catch (Exception ex)
            {
                return ServiceResponse<IEnumerable<CustomerResponse>>.Failure(
                    $"Error retrieving active customers: {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Get customer by Id using ssp_Customer_GetById
        /// </summary>
        public async Task<ServiceResponse<CustomerResponse>> GetByIdAsync(int id)
        {
            try
            {
                using var connection = _context.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                var customer = await connection.QuerySingleOrDefaultAsync<CustomerResponse>(
                    "ssp_Customer_GetById",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (customer == null)
                {
                    return ServiceResponse<CustomerResponse>.Failure("Customer not found");
                }

                return ServiceResponse<CustomerResponse>.Success(customer, "Customer retrieved successfully");
            }
            catch (Exception ex)
            {
                return ServiceResponse<CustomerResponse>.Failure($"Error retrieving customer: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all customers using ssp_Customer_GetAll
        /// </summary>
        public async Task<ServiceResponse<IEnumerable<CustomerResponse>>> GetAllAsync()
        {
            try
            {
                using var connection = _context.CreateConnection();

                var customers = await connection.QueryAsync<CustomerResponse>(
                    "ssp_Customer_GetAll",
                    commandType: CommandType.StoredProcedure
                );

                return ServiceResponse<IEnumerable<CustomerResponse>>.Success(
                    customers,
                    $"Retrieved {customers.Count()} customers"
                );
            }
            catch (Exception ex)
            {
                return ServiceResponse<IEnumerable<CustomerResponse>>.Failure(
                    $"Error retrieving customers: {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Delete customer permanently using ssp_Customer_Delete
        /// </summary>
        public async Task<ServiceResponse<int>> DeleteAsync(int id)
        {
            try
            {
                using var connection = _context.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                var rowsAffected = await connection.ExecuteAsync(
                    "ssp_Customer_Delete",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (rowsAffected == 0)
                {
                    return ServiceResponse<int>.Failure("Customer not found");
                }

                return ServiceResponse<int>.Success(rowsAffected, "Customer deleted successfully");
            }
            catch (Exception ex)
            {
                return ServiceResponse<int>.Failure($"Error deleting customer: {ex.Message}");
            }
        }

        /// <summary>
        /// Soft delete customer using ssp_Customer_SoftDelete
        /// </summary>
        public async Task<ServiceResponse<int>> SoftDeleteAsync(int id)
        {
            try
            {
                using var connection = _context.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                var rowsAffected = await connection.ExecuteAsync(
                    "ssp_Customer_SoftDelete",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (rowsAffected == 0)
                {
                    return ServiceResponse<int>.Failure("Customer not found");
                }

                return ServiceResponse<int>.Success(rowsAffected, "Customer deactivated successfully");
            }
            catch (Exception ex)
            {
                return ServiceResponse<int>.Failure($"Error deactivating customer: {ex.Message}");
            }
        }
    }
}
