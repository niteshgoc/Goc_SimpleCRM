using Dapper;
using SimpleCRM.Data;
using SimpleCRM.Models;
using SimpleCRM.Models.State;
using SimpleCRM.Repos.Interfaces;
using System.Data;

namespace SimpleCRM.Repos.Services
{
    /// <summary>
    /// State repository implementation
    /// Uses Dapper to execute stored procedures for tblState
    /// </summary>
    public class StateRepository : IStateRepository
    {
        private readonly DapperContext _context;

        public StateRepository(DapperContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all active states using ssp_tblState_GetActive
        /// </summary>
        public async Task<ServiceResponse<IEnumerable<StateResponse>>> GetActiveAsync()
        {
            try
            {
                using var connection = _context.CreateConnection();

                var states = await connection.QueryAsync<StateResponse>(
                    "ssp_tblState_GetActive",
                    commandType: CommandType.StoredProcedure
                );

                return ServiceResponse<IEnumerable<StateResponse>>.Success(
                    states,
                    $"Retrieved {states.Count()} active states"
                );
            }
            catch (Exception ex)
            {
                return ServiceResponse<IEnumerable<StateResponse>>.Failure(
                    $"Error retrieving active states: {ex.Message}"
                );
            }
        }
    }
}
