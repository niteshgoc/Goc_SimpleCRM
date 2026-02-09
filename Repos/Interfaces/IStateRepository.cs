using SimpleCRM.Models;
using SimpleCRM.Models.State;

namespace SimpleCRM.Repos.Interfaces
{
    /// <summary>
    /// State repository interface
    /// Defines operations for State master data
    /// </summary>
    public interface IStateRepository
    {
        /// <summary>
        /// Get all active states
        /// Maps to: ssp_tblState_GetActive
        /// </summary>
        /// <returns>ServiceResponse with list of active states</returns>
        Task<ServiceResponse<IEnumerable<StateResponse>>> GetActiveAsync();
    }
}
